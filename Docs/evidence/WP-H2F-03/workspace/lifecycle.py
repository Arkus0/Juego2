"""WP-H2F-03 composed H1 public lifecycle over the integrated NON_KEEPER fixture, with the whole selected stack present.

  python lifecycle.py run   [--root C:\\Juego2-H2F03-W]     # L1..L7 (see WORKER_PLAN.md), writes results/lifecycle_*.json
  python lifecycle.py compare [--root ...]                   # re-derive the per-family comparison from recorded witnesses

Only the fixed public `--h1-unity` reference host is used for H1 work (the accepted H1-GATE topology, local launcher).
This client reuses the Gate's public-wire helpers (scripts/h1-gate-scenario.py) and never launches Unity for H1: every
H1 Unity process is launched by the product's fixed launcher behind a public capability. The client launches Unity
itself only for the H2F-03 evidence steps (fixture build, witnesses, declared restoration steps), always while no H1
operation is in flight. Two operator actions touch the project directly, both declared: the managed Transform edit that
simulates a human Unity edit (L3) and the removal of H1's generated output before the clean rebuild (L4).
"""

from __future__ import annotations

import argparse
import csv
import hashlib
import importlib.util
import json
import shutil
import subprocess
import sys
import time
from pathlib import Path
from types import SimpleNamespace

HERE = Path(__file__).resolve().parent
REPO = HERE.parents[3]
DEFAULT_ROOT = Path(r"C:\Juego2-H2F03-W")
PROJECT = "Unity/ArkusUnity"


def load(name: str, path: Path):
    spec = importlib.util.spec_from_file_location(name, path)
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


gate = load("h1_gate_scenario", REPO / "scripts/h1-gate-scenario.py")
ws = load("h2f03_workspace", HERE / "workspace.py")
SLICE = json.loads((HERE / "fixture_slice.json").read_text(encoding="utf-8"))
SCENE = SLICE["sceneLogicalId"]
assert SCENE == gate.SCENE, "the fixture uses the accepted fixed H1 managed scene"


def nodes():
    """The slice in the Gate's node-tuple layout, so the Gate's public binding helpers are reused unchanged."""
    out = []
    for n in SLICE["nodes"]:
        link = (n["linkRelation"], n["linkTarget"]) if n["linkRelation"] else None
        out.append((n["id"], n["container"] or None, n["type"], n["kind"], n["source"], tuple(n["positionMm"]), n["yawMilliDeg"],
                    tuple(n["scalePpm"]), n["material"] or None, n["clip"] or None, link))
    return out


NODES = nodes()


def sha_file(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest() if path.is_file() else "absent"


def sha_text(text: str) -> str:
    return hashlib.sha256(text.encode("utf-8")).hexdigest()


def write(path: Path, value) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, indent=2, sort_keys=True) + "\n", encoding="utf-8")


class Run:
    def __init__(self, root: Path):
        self.root = root
        self.results = ws.results(root)
        self.h1 = self.results / "h1"
        self.h1.mkdir(parents=True, exist_ok=True)
        self.transcript = gate.Transcript(self.h1 / "transcript.jsonl")
        self.launcher = gate.Launcher(SimpleNamespace(launcher="local", unity_image="", license_home="", dotnet=shutil.which("dotnet") or "dotnet"))
        self.facts = {"schemaId": "juego2.h2f03.lifecycle-facts@1", "candidateSha": (root / ws.MARKER).read_text().strip(), "stages": {}}
        self.started = time.time() - 5

    def stage(self, key, value):
        self.facts["stages"][key] = value
        write(self.results / "lifecycle_facts.json", self.facts)

    def unity(self, method: str, name: str, *extra: str) -> None:
        self.transcript.write({"event": "evidence-unity", "method": method, "name": name, "extra": list(extra)})
        code = ws.unity(self.root, method, name, False, list(extra))
        if code != 0:
            raise gate.GateError(f"evidence Unity step {name} ({method}) exited {code}")

    def operator(self, stage, action, **fields):
        self.transcript.write({"event": "operator-action", "stage": stage, "action": action, **fields})


# ------------------------------------------------------------------ public H1 steps

def author_slice(host):
    objects, documents, dependencies = [], [], {}
    for node in NODES:
        compiled, _ = gate.compile_binding(host, node, "L1")
        dependencies[node[0]] = {"canonical": compiled.get("canonicalDependencies", []), "catalogue": compiled.get("catalogueDependencies", [])}
        documents.append(dict(compiled["documentMutation"]))
        operation = {"kind": "put-object", "id": node[0], "typeId": node[2]}
        if node[1]:
            operation["containerId"] = node[1]
        objects.append(operation)
    applied = gate.author(host, "L1", "h2f03.slice.create", objects + documents)
    snapshot = host.ok("authoring.snapshot.export", {}, "L1", "snapshot.export:authored")
    revision, digest = gate.anchor(host, "L1")
    return {"dependencies": dependencies, "revision": revision, "canonicalHash": digest, "appliedDigest": gate.sha(gate.canonical_json(applied))}, snapshot


def resolve_catalogue(host):
    ids = set()
    for node in NODES:
        ids.add((node[3], node[4]))
        if node[8]:
            ids.add(("material", node[8]))
        if node[9]:
            ids.add(("animation-clip", node[9]))
    resolved = {}
    for kind, logical in sorted(ids):
        entry = host.ok("unity.host.catalogue.resolve", {"logicalId": logical, "kind": kind}, "L1", f"catalogue.resolve:{logical}")
        entry = entry.get("entry", entry)
        gate.require(entry.get("logicalId") == logical and entry.get("compatible", True) is not False, f"catalogue did not resolve {kind}:{logical}")
        resolved[logical] = {"kind": kind, "path": entry.get("path"), "nativeGuid": entry.get("nativeGuid")}
    page = host.ok("unity.host.catalogue.query", {"pageSize": 1, "offset": 0}, "L1", "catalogue.query:fingerprint")
    return {"fingerprint": page["fingerprint"], "total": page["total"], "resolved": resolved}


def materialize(host, stage, label, plan):
    result = host.ok("unity.host.projection.materialize", {"sceneLogicalId": SCENE}, stage, label)
    gate.require(result["active"] is True and result["current"] is True, f"{label}: not the active current generation")
    gate.require(result["inputDigest"] == plan["inputDigest"] and result["canonicalHash"] == plan["canonicalHash"], f"{label}: not bound to the plan")
    gate.require(sorted(n["objectId"] for n in result["nodes"]) == sorted(plan["objectIds"]), f"{label}: node set differs from plan")
    return result


def parity(host, stage, label):
    report = gate.drift(host, stage, label)
    gate.require_parity(report, label)
    return {"state": report.get("state"), "parity": report.get("parity"), "items": len(report.get("items", []))}


# ------------------------------------------------------------------ file-level witnesses

def file_witness(root: Path) -> dict:
    p = root / PROJECT

    def files(*patterns, exclude=()):
        rows = []
        for pattern in patterns:
            for path in sorted(p.glob(pattern)):
                rel = path.relative_to(p).as_posix()
                if path.is_file() and not rel.endswith(".meta") and not any(rel.startswith(e) for e in exclude):
                    rows.append(f"{rel}={sha_file(path)}")
        return rows

    lock = json.loads((p / "Packages/packages-lock.json").read_text(encoding="utf-8"))
    receipt = p / "Assets/Plugins/GameCreator/.juego2-provisioning.json"
    receipt_json = json.loads(receipt.read_text(encoding="utf-8")) if receipt.is_file() else {}
    settings_dir = p / "Assets/Plugins/GameCreator/Data/Resources/Settings"
    fixture = p / "Assets/H2F03Fixture"
    families = {
        "pkg.manifest_lock": files("Packages/manifest.json", "Packages/packages-lock.json"),
        "settings.project": files("ProjectSettings/*.asset", "ProjectSettings/*.txt", "ProjectSettings/*.json", exclude=("ProjectSettings/Arkus/",)),
        "urp.pipeline_assets": files("Assets/Juego2/Foundation/Rendering/*.asset"),
        "urp.look_preset": files("Assets/Juego2/Foundation/Rendering/J2_LookPreset.asset", "Assets/Juego2/Foundation/Rendering/J2_LookVolumeProfile.asset",
                                 "Assets/Juego2/Foundation/Rendering/J2_Sky_Overcast.mat", "Assets/Juego2/Foundation/Rendering/J2_OvercastSky.png"),
        "shaders.project": files("Assets/Juego2/Foundation/Shaders/*.shader"),
        "materials.retained": files("Assets/Juego2/**/*.mat", "Assets/Arkus/CITY/Materials/*.mat", "Assets/H2F03Fixture/Materials/*"),
        "h1.catalogue_snapshot": [f"EFFECTIVE_INVENTORY.json={sha_file(root / 'Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json')}",
                                  f"CatalogueMapping.json={sha_file(p / 'Assets/Arkus/H1/CatalogueMapping.json')}"],
        "worldbuilding.profiles": files("Assets/H2F03Fixture/Profiles/*.asset"),
        "terrain": files("Assets/H2F03Fixture/H2F03_Terrain.asset"),
        "navmesh.data": files("Assets/H2F03Fixture/H2F03_NavMesh.asset"),
        "navmesh.agent": files("ProjectSettings/NavMeshAreas.asset"),
        "input.asset": files("Assets/Juego2/Foundation/Input/J2_Input.inputactions"),
        "gc2.S10": files("Assets/Juego2/Foundation/Input/J2_Input.inputactions"),
        "presets.character_camera": files("Assets/Juego2/Foundation/Presets/*.asset", "Assets/H2F03Fixture/Presets/*.asset"),
        "anim.controllers": files("Assets/H2F03Fixture/Characters/*.controller"),
        "rigging": files("Assets/H2F03Fixture/Characters/J2_Citizen_ContactRig.prefab"),
        "import.humanoid": [f"Townsfolk_Forastero.fbx.meta={sha_file(p / 'Assets/Arkus/ART/Derived/Characters/Townsfolk_Forastero.fbx.meta')}"],
        "import.ual": [f"{name}.meta={sha_file(p / rel)}" for name, rel in (
            ("UAL1.fbx", "Assets/Arkus/ART/External/UAL/UAL1.fbx.meta"), ("UAL2.fbx", "Assets/H2F01Inputs/Animation/UAL2.fbx.meta"),
            ("UAL1_RM.fbx", "Assets/H2F01Inputs/Animation/UAL1_RM.fbx.meta"), ("UAL2_RM.fbx", "Assets/H2F01Inputs/Animation/UAL2_RM.fbx.meta"))],
        "gc2.S1": [f"version={(p / 'Assets/Plugins/GameCreator/Packages/Core/Editor/Version.txt').read_text().strip() if (p / 'Assets/Plugins/GameCreator/Packages/Core/Editor/Version.txt').is_file() else 'absent'}",
                   f"packageSha256={receipt_json.get('packageSha256', receipt_json.get('sha256', 'absent'))}",
                   f"receiptKeys={','.join(sorted(receipt_json))}"],
        "gc2.S8": files("Assets/Plugins/GameCreator/Packages/Core/Runtime/Characters/**/Skeleton*.asset"),
        "gc2.define": [f"csc.rsp={(p / 'Assets/csc.rsp').read_text(encoding='utf-8').strip() if (p / 'Assets/csc.rsp').is_file() else 'absent'}"],
        "gc2.S2": sorted(x.name for x in settings_dir.glob("*.asset")) if settings_dir.is_dir() else ["absent"],
        "gc2.adapter_code": files("Assets/Juego2/Gc2Adapter/**/*.cs", "Assets/Juego2/Gc2Adapter/**/*.asmdef"),
        "cinemachine": [f"lockHasCinemachine={'com.unity.cinemachine' in lock.get('dependencies', {})}"],
        "fixture.scene": files("Assets/H2F03Fixture/H2F03_IntegrationFixture_NONKEEPER.unity"),
    }
    return {family: {"facts": rows, "digest": sha_text(family + "\n" + "\n".join(rows))} for family, rows in families.items()}


def witness(run: Run, label: str) -> dict:
    run.unity("Juego2.H2F03.Evidence.H2F03Witness.Capture", f"witness_{label}", "-h2f03-label", label)
    unity = json.loads((run.results / f"witness_unity_{label}.json").read_text(encoding="utf-8"))
    files = file_witness(run.root)
    merged = {"schemaId": "juego2.h2f03.lifecycle-witness@1", "label": label, "unityErrors": unity.get("errors", []),
              "unity": {r["family"]: {"present": r["present"], "digest": r["digest"], "facts": r["facts"]} for r in unity["records"]},
              "files": files}
    write(run.results / f"lifecycle_witness_{label}.json", merged)
    return merged


# ------------------------------------------------------------------ the run

def run_lifecycle(root: Path) -> int:
    run = Run(root)
    host = gate.PublicHost("reference", "h2f03-a", run.launcher, root, run.transcript)
    try:
        profile, universe = gate.stage3_inspect(host)
        run.stage("L1.inspect", {"profile": profile, "effectiveUniverse": universe})
        keys = host.discover("L1")
        missing = [k for k in gate.REQUIRED_CAPABILITIES if k not in keys]
        gate.require(not missing, f"discovery omitted {missing}")
        catalogue = resolve_catalogue(host)
        authored, authored_snapshot = author_slice(host)
        host.ok("world.validation.current", {}, "L1", "world.validation.current")
        plan = host.ok("unity.projection.plan", {"sceneLogicalId": SCENE}, "L1", "projection.plan")
        gate.require(sorted(plan["objectIds"]) == sorted(n[0] for n in NODES), "projection plan does not cover exactly the fixture slice")
        first = materialize(host, "L1", "materialize#1", plan)
        observed = host.ok("unity.host.projection.observe", {"sceneLogicalId": SCENE}, "L1", "observe#1")
        gate.require(gate.summary_of(observed) == gate.summary_of(first), "observe disagrees with materialize")
        run.stage("L1", {"catalogue": catalogue, "authored": authored, "plan": {k: plan[k] for k in ("inputDigest", "canonicalHash", "catalogueFingerprint")},
                         "materialized": gate.summary_of(first), "observed": gate.summary_of(observed), "drift": parity(host, "L1", "drift:first")})

        # W2: the retained realization sidecar is built around the published projection (host idle, H1 untouched)
        run.unity("Juego2.H2F03.Evidence.H2F03Fixture.BuildSidecar", "w2_build_sidecar")
        run.stage("W2", json.loads((run.results / "fixture_build.json").read_text(encoding="utf-8-sig")))
        run.stage("L1.drift_with_sidecar", parity(host, "L1", "drift:sidecar-present"))

        # L2: witnesses before the lifecycle
        witness(run, "before")

        # L3: one legal change inside H1 authority: a human-style edit of one managed Transform -> reconcile
        change = SLICE["legalChange"]
        scene = gate.published_scene(root, first["generationId"])
        stimulus = gate.edit_transform_x(scene, change["objectId"], change["deltaMm"])
        run.operator("L3", "unity-managed-edit", **stimulus)
        report = gate.drift(host, "L3", "drift:after-edit")
        changed = [i for i in report.get("items", []) if i.get("objectId") == change["objectId"]]
        gate.require(len(changed) == 1 and "transform" in changed[0].get("fields", []), f"edit not observed as drift: {gate.canonical_json(report)[:800]}")
        gate.require(len(report.get("items", [])) == 1, f"drift reports more than the edited node: {gate.canonical_json(report)[:800]}")
        proposal = host.ok("unity.host.projection.import-proposal", {"sceneLogicalId": SCENE}, "L3", "import-proposal")
        gate.require(proposal.get("available") is True, "supported edit produced no proposal")
        request = proposal["mutationRequest"]
        host.ok("authoring.change.plan", request, "L3", "plan:proposal")
        host.ok("authoring.change.dry-run", request, "L3", "dry-run:proposal")
        host.ok("authoring.change.apply", request, "L3", "apply:proposal")
        remat = host.ok("unity.host.projection.rematerialize", {"sceneLogicalId": SCENE}, "L3", "rematerialize")
        after_edit = host.ok("unity.host.projection.observe", {"sceneLogicalId": SCENE}, "L3", "observe:after-reconcile")
        moved = next(n for n in after_edit["nodes"] if n["objectId"] == change["objectId"])
        original = next(n for n in NODES if n[0] == change["objectId"])
        gate.require(moved["positionMm"]["x"] == original[5][0] + change["deltaMm"], "reconciled edit is not canonical truth")
        run.stage("L3", {"stimulus": stimulus, "driftItems": len(report.get("items", [])), "proposalKey": request.get("idempotencyKey"),
                         "rematerializeDigest": gate.sha(gate.canonical_json(remat)), "observed": gate.summary_of(after_edit),
                         "movedPositionMm": moved["positionMm"], "drift": parity(host, "L3", "drift:after-rematerialize")})

        # L4: checkpoint, then remove H1's generated Unity output
        captured = host.ok("unity.host.checkpoint.capture", {"sceneLogicalId": SCENE}, "L4", "checkpoint.capture")
        gate.require(captured["state"] == "ready", "checkpoint not ready")
        pre_revision, pre_hash = gate.anchor(host, "L4")
    finally:
        host.close()
    removed = gate.remove_generated_output(root)
    run.operator("L4", "remove-generated-output", paths=removed)
    run.stage("L4", {"captured": captured, "preCloseAnchor": {"revision": pre_revision, "hash": pre_hash}, "removed": removed})

    # L5: a fresh host process with no canonical state restores the checkpoint and clean-rebuilds the projection
    fresh = gate.PublicHost("reference", "h2f03-b", run.launcher, root, run.transcript)
    try:
        fresh.discover("L5")
        empty = fresh.ok("world.summary", {}, "L5", "world.summary:fresh")
        gate.require(empty["world"]["revision"] == 0 and empty["objectCount"] == 0, "fresh host is not empty")
        restored = fresh.ok("unity.host.checkpoint.restore", {"sceneLogicalId": SCENE}, "L5", "checkpoint.restore")
        revision, digest = gate.anchor(fresh, "L5")
        gate.require(digest == captured["canonicalHash"] == pre_hash, "restored canonical hash differs")
        rebuilt = fresh.ok("unity.host.projection.clean-rebuild", {"sceneLogicalId": SCENE}, "L5", "clean-rebuild")
        observed = fresh.ok("unity.host.projection.observe", {"sceneLogicalId": SCENE}, "L5", "observe:rebuilt")
        gate.require(observed["current"] is True and observed["graphDigest"] == captured["observationGraphDigest"], "rebuilt graph differs from checkpoint")
        drift = parity(fresh, "L5", "drift:rebuilt-with-sidecar")
        recaptured = fresh.ok("unity.host.checkpoint.capture", {"sceneLogicalId": SCENE}, "L5", "checkpoint.capture:rebuilt")
        gate.require(recaptured["observationReconstructionDigest"] == captured["observationReconstructionDigest"], "rebuilt reconstruction digest differs")
        run.stage("L5", {"restored": restored, "anchor": {"revision": revision, "hash": digest}, "rebuilt": gate.summary_of(rebuilt) if "graphDigest" in rebuilt else rebuilt,
                         "observed": gate.summary_of(observed), "drift": drift, "recaptured": recaptured})
    finally:
        fresh.close()
    audit = gate.ledger_audit(root, run.started)
    write(run.h1 / "ledger.json", audit)
    ops = root / gate.OPERATIONS
    if ops.is_dir():
        shutil.copytree(ops, run.h1 / "operations", dirs_exist_ok=True)
    run.stage("H1.ledger", {"unityProcesses": len(audit), "ownedErrorLines": sum(len(r["ownedErrorLines"]) for r in audit),
                            "unread": [r["invocationId"] for r in audit if not r["editorLogRead"]]})

    # L6: generated and external state through its declared restoration steps (never hand repair)
    restoration = []
    run.unity("Juego2.H2F03.Evidence.H2F03Witness.RealizeCollision", "l6_realize_collision")
    restoration.append("projection.collision: J2CollisionRealizer re-realized from the current projection")
    nav = root / PROJECT / "Assets/H2F03Fixture/H2F03_NavMesh.asset"
    before_nav = sha_file(nav)
    nav.unlink()
    (root / PROJECT / "Assets/H2F03Fixture/H2F03_NavMesh.asset.meta").unlink(missing_ok=True)
    run.operator("L6", "delete-generated", paths=["Assets/H2F03Fixture/H2F03_NavMesh.asset"])
    run.unity("Juego2.H2F03.Evidence.H2F03Witness.BakeNavMesh", "l6_bake_navmesh")
    restoration.append(f"navmesh.data: deleted and re-baked (file sha before {before_nav[:12]}, after {sha_file(nav)[:12]})")
    receipt_before = sha_file(root / PROJECT / "Assets/Plugins/GameCreator/.juego2-provisioning.json")
    gate.require(ws.gc2(root, "remove") == 0, "GC2 removal failed")
    run.operator("L6", "gc2-remove", note="returns to the Core-absent state")
    gate.require(ws.gc2(root, "gc2") == 0, "GC2 re-provisioning failed")
    restoration.append(f"gc2.S1/S8/define: removed and re-provisioned through scripts/h2f02-provision.py (receipt {receipt_before[:12]} -> "
                       f"{sha_file(root / PROJECT / 'Assets/Plugins/GameCreator/.juego2-provisioning.json')[:12]})")
    library = root / PROJECT / "Library"
    shutil.rmtree(library)
    run.operator("L6", "delete-library", note="every Library artefact (imports, compiled assemblies, H1 lifecycle scratch) is generated")
    restoration.append("Library: deleted; the next editor session reimports everything from committed/lawful inputs")
    run.stage("L6", {"steps": restoration})

    # L7: witnesses after, and the per-family comparison against the H2F-02 matrix
    witness(run, "after")
    comparison = compare(root)
    run.stage("L7", {"result": comparison["result"], "mismatches": comparison["mismatches"]})
    print(f"H2F03_LIFECYCLE_{comparison['result']} families={len(comparison['families'])} mismatches={comparison['mismatches']}")
    return 0 if comparison["result"] == "GREEN" else 1


# ------------------------------------------------------------------ comparison against the H2F-02 matrix

ADDENDUM = [
    {"family_id": "projection.collision", "lifecycle_class": "GENERATED_TRANSIENT",
     "rule": "J2CollisionRealizer proxies in the sidecar, derived from the current H1 projection; re-realized after any rematerialize that moves proxied geometry (IsStale)"},
    {"family_id": "fixture.scene", "lifecycle_class": "RETAINED_REALIZATION", "rule": "the sidecar scene file itself is never written by H1"},
]
RUNTIME_ONLY = {"gc2.S5", "gc2.S13"}
FIRST_WINDOWED = {"gc2.S2"}


def compare(root: Path) -> dict:
    results = ws.results(root)
    before = json.loads((results / "lifecycle_witness_before.json").read_text(encoding="utf-8"))
    after = json.loads((results / "lifecycle_witness_after.json").read_text(encoding="utf-8"))
    facts = json.loads((results / "lifecycle_facts.json").read_text(encoding="utf-8"))
    with (root / "Docs/evidence/WP-H2F-02/H1_LIFECYCLE_MATRIX.csv").open(encoding="utf-8") as handle:
        matrix = list(csv.DictReader(handle))
    rows = [{"family_id": r["family_id"], "lifecycle_class": r["lifecycle_class"], "source": "H2F-02"} for r in matrix]
    rows += [dict(r, source="H2F-03 addendum") for r in ADDENDUM]
    change = SLICE["legalChange"]
    families, mismatches = [], []

    def get(witness, family):
        return witness["unity"].get(family), witness["files"].get(family)

    for row in rows:
        family, cls = row["family_id"], row["lifecycle_class"]
        ub, fb = get(before, family)
        ua, fa = get(after, family)
        outcome, detail = "", ""
        if family == "h1.managed_projection":
            rb = set(ub["facts"]) if ub else set()
            ra = set(ua["facts"]) if ua else set()
            gone, new = sorted(rb - ra), sorted(ra - rb)
            only_change = all(change["objectId"] in r for r in gone + new) and len(gone) == len(new) and len(gone) >= 1
            l5 = facts["stages"].get("L5", {})
            ok = only_change and l5.get("drift", {}).get("parity") is True and l5.get("observed", {}).get("current") is True
            outcome = "H1_MANAGED: observed/reconciled/rematerialized/clean-rebuilt under the accepted H1 contract" if ok else "MISMATCH"
            detail = f"rows differing only for {change['objectId']} (the reconciled legal change): {len(gone)}; rebuilt graph == checkpoint; drift parity"
        elif cls == "H1_MANAGED_PROJECTION":
            ok = fb and fa and fb["digest"] == fa["digest"]
            outcome = "H1_MANAGED: unchanged across the cycle" if ok else "MISMATCH"
        elif cls in ("RETAINED_PROJECT_CONFIGURATION", "RETAINED_REALIZATION"):
            present = [w for w in (ub, fb) if w]
            if not present:
                outcome, ok = "NOT_PRESENT", True
                detail = "family has no instance in this fixture"
            else:
                ok = all((x and y and x["digest"] == y["digest"]) for x, y in ((ub, ua), (fb, fa)) if x or y)
                outcome = "RETAINED: present and unchanged, never surfaced as H1 drift" if ok else "MISMATCH"
        elif cls == "GENERATED_TRANSIENT":
            if family in RUNTIME_ONLY:
                ok = bool(ub) and bool(ua) and not ub["present"] and not ua["present"]
                outcome = "GENERATED: absent from serialized state before and after; created at play (see play evidence)" if ok else "MISMATCH"
            elif family in FIRST_WINDOWED:
                ok = True
                outcome = "GENERATED: removed with the vendor root; regenerated at the first windowed editor session (see play evidence)"
                detail = f"before={fb['facts'] if fb else None} after={fa['facts'] if fa else None}"
            elif family == "projection.collision":
                fa_ = dict(x.split("=", 1) for x in ua["facts"]) if ua else {}
                fb_ = dict(x.split("=", 1) for x in ub["facts"]) if ub else {}
                ok = fa_.get("stale") == "False" and fa_.get("in") == fa_.get("currentProjectionIn") and fa_.get("out") == fa_.get("recomputedOut") \
                    and fb_.get("stale") == "False"
                outcome = "GENERATED: re-realized by its documented step; not stale against the rebuilt projection" if ok else "MISMATCH"
                detail = f"input digest before {fb_.get('in', '')[:12]} after {fa_.get('in', '')[:12]} (the reconciled fence move changes it)"
            elif family == "navmesh.data":
                def tri(w):
                    return dict(x.split("=", 1) for x in w["facts"]).get("triangulation") if w else None
                ok = tri(ub) is not None and tri(ub) == tri(ua)
                outcome = "GENERATED: deleted and re-baked; identical triangulation" if ok else "MISMATCH"
            elif family == "gc2.define":
                ok = fb and fa and fb["digest"] == fa["digest"] and "absent" not in fa["facts"][0]
                outcome = "GENERATED: rewritten by provisioning; identical" if ok else "MISMATCH"
            else:
                present = [w for w in (ub, fb) if w]
                if not present or all(not w.get("present", True) for w in (ub,) if w):
                    ok = True
                    outcome = "NOT_PRESENT: not used by the fixture (declared generated state absent before and after)"
                else:
                    ok = all((x and y and x["digest"] == y["digest"]) for x, y in ((ub, ua), (fb, fa)) if x or y)
                    outcome = "GENERATED: reproduced identically after the Library/tool restoration" if ok else "MISMATCH"
        elif cls == "EXTERNAL_MANUAL_INPUT":
            ok = fb and fa and fb["digest"] == fa["digest"] and "absent" not in "".join(fa["facts"])
            outcome = "EXTERNAL: provisioned again through the declared hash-gated path; identical" if ok else "MISMATCH"
        elif cls == "NOT_ADMITTED":
            flags = [w["present"] for w in (ub, ua) if w]
            ok = not any(flags) and (fa is None or "True" not in "".join(fa["facts"]))
            outcome = "NOT_ADMITTED: absent before and after" if ok else "MISMATCH"
        else:
            ok, outcome = False, "UNCLASSIFIED"
        families.append({"family": family, "class": cls, "source": row["source"], "outcome": outcome, "ok": bool(ok), "detail": detail,
                         "before": {"unity": ub["digest"] if ub else None, "files": fb["digest"] if fb else None},
                         "after": {"unity": ua["digest"] if ua else None, "files": fa["digest"] if fa else None}})
        if not ok:
            mismatches.append(family)
    result = {"schemaId": "juego2.h2f03.lifecycle-comparison@1", "families": families, "mismatches": mismatches,
              "unityWitnessErrors": {"before": before.get("unityErrors", []), "after": after.get("unityErrors", [])},
              "result": "GREEN" if not mismatches and not before.get("unityErrors") and not after.get("unityErrors") else "RED"}
    write(results / "lifecycle_comparison.json", result)
    return result


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("mode", choices=["run", "compare"])
    parser.add_argument("--root", type=Path, default=DEFAULT_ROOT)
    args = parser.parse_args()
    root = args.root.resolve()
    try:
        if args.mode == "run":
            return run_lifecycle(root)
        result = compare(root)
        print(f"H2F03_LIFECYCLE_{result['result']} mismatches={result['mismatches']}")
        return 0 if result["result"] == "GREEN" else 1
    except gate.GateError as exc:
        print(f"H2F03_LIFECYCLE_RED {exc}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    sys.exit(main())
