#!/usr/bin/env python3
"""WP-H1-GATE independent evidence verifier.

The scenario driver writes facts, but this verifier does not trust them. It recomputes every gate verdict from sources
it does not control:

* the stage universe is parsed from `Docs/engineering/H1_UNITY_PARITY_GATE.md`, not from a driver declaration;
* the required public capability roles come from the ADR-H1-004 table, pinned by digest;
* every stage fact is recomputed from the raw public request/response transcript;
* every Unity process is reconciled against the product launcher's own operation ledger;
* the gate workflow is statically checked to launch Unity only through the public hosts.

Subcommands:
  evidence  --dir DIR --candidate-sha SHA   verify a complete gate evidence bundle
  workflow  [--file PATH]                   static no-private-Unity-path and stage-execution check
  scope     --base SHA                      candidate diff stays inside Gate-owned paths (no product change)
  reconcile                                 residual/dependency reconciliation against discovered sources
  trial     --dir DIR --candidate-sha SHA   fresh AI-agent trial transcript binding and public-only use

Every RED names stable reason codes on stderr (`H1_GATE_VERIFY_RED <code> ...`).
"""

import argparse
import base64
import hashlib
import json
import os
import re
import subprocess
import sys
from collections import Counter
from pathlib import Path

# Negative controls point the verifier at a mutated copy of the governing documents through this override.
ROOT = Path(os.environ.get("H1_GATE_VERIFY_ROOT") or Path(__file__).resolve().parents[1])
PARITY_DOC = "Docs/engineering/H1_UNITY_PARITY_GATE.md"
ADR = "Docs/architecture/ADR-H1-004-PUBLIC-EDITOR-EXECUTION-SEAM.md"
RECONCILIATION = "Docs/evidence/WP-H1-GATE/RECONCILIATION.json"
WORKFLOW = ".github/workflows/h1-gate-validation.yml"
TRIAL_WORKFLOW = ".github/workflows/h1-gate-ai-trial.yml"
SCENE = "arkus.h1-05.scene.potes"
EDITOR = ("6000.3.24f1", "4e7b9b5b6244")
ENTRY_POINT = "Arkus.H1.Editor.H1EditorWorker.Run"

# ADR-H1-004 "Public capability ownership" table -> the composed keys that realize each role. The table text is pinned:
# if the accepted ADR changes, this verifier fails closed until the mapping is re-reviewed.
ADR_ROLE_TABLE_SHA256 = "9bb55a6a0d8cb1de4f54acdc0fb797ecd107e4e959b15607d9cc8ba24facddd4"
ADR_ROLES = {
    "H1-03A": ["unity.host.project-profile.inspect@1.0", "unity.lifecycle.operation-status@1.0"],
    "H1-04": ["unity.host.catalogue.query@1.0", "unity.host.catalogue.get@1.0", "unity.host.catalogue.resolve@1.0"],
    "H1-05": ["unity.projection.plan@1.0", "unity.host.projection.materialize@1.0", "unity.host.projection.observe@1.0"],
    "H1-08": ["unity.projection.plan@1.0", "unity.host.projection.materialize@1.0"],
    "H1-09": ["unity.host.projection.drift@1.0", "unity.host.projection.import-proposal@1.0", "unity.host.projection.rematerialize@1.0"],
    "H1-10": ["unity.host.checkpoint.capture@1.0", "unity.host.checkpoint.current@1.0", "unity.host.checkpoint.restore@1.0",
              "unity.host.projection.clean-rebuild@1.0"],
}
H0_REQUIRED = ["authoring.change.plan@1.0", "authoring.change.dry-run@1.0", "authoring.change.apply@1.0",
               "authoring.snapshot.export@1.0", "authoring.snapshot.import@1.0", "authoring.journal.read@1.0",
               "authoring.diff.compare@1.0", "unity.binding.compile@1.0"]
EDITOR_BOUND = {
    "unity.host.project-profile.inspect@1.0", "unity.host.hierarchy-probe.inspect@1.0",
    "unity.host.catalogue.query@1.0", "unity.host.catalogue.get@1.0", "unity.host.catalogue.resolve@1.0",
    "unity.host.projection.materialize@1.0", "unity.host.projection.observe@1.0",
    "unity.host.projection.drift@1.0", "unity.host.projection.import-proposal@1.0", "unity.host.projection.rematerialize@1.0",
    "unity.host.projection.clean-rebuild@1.0",
}
# Host-local composed capabilities that dispatch Editor-bound capabilities through the same composed contract.
NESTED = {
    "unity.host.checkpoint.capture@1.0": ["unity.host.projection.observe@1.0"],
    "unity.host.checkpoint.restore@1.0": ["unity.host.projection.clean-rebuild@1.0", "unity.host.projection.observe@1.0"],
}
WORKER_EXECUTORS = {
    "arkus.h1.worker.project-profile.inspect@1", "arkus.h1.worker.hierarchy-probe.inspect@1",
    "arkus.h1.worker.catalogue.query@1", "arkus.h1.worker.catalogue.get@1", "arkus.h1.worker.catalogue.resolve@1",
    "arkus.h1.worker.projection.materialize@1", "arkus.h1.worker.projection.observe@1",
    "arkus.h1.worker.projection.reconcile@1", "arkus.h1.worker.projection.clean-rebuild@1",
}
TRANSPORTS = ("reference", "mcp")


def sha(value):
    return hashlib.sha256(value.encode("utf-8") if isinstance(value, str) else value).hexdigest()


def canonical_json(value):
    return json.dumps(value, sort_keys=True, separators=(",", ":"), ensure_ascii=False)


class Findings:
    def __init__(self):
        self.codes = []

    def red(self, code, detail=""):
        self.codes.append((code, detail))

    def check(self, condition, code, detail=""):
        if not condition:
            self.red(code, detail)
        return bool(condition)

    def finish(self, label):
        if self.codes:
            for code, detail in self.codes:
                print(f"H1_GATE_VERIFY_RED {code} {detail}".rstrip(), file=sys.stderr)
            sys.exit(1)
        print(f"H1_GATE_VERIFY_GREEN {label}")


# ----- independent universes ---------------------------------------------------------------------------------------

def parity_stages(root=ROOT):
    text = (root / PARITY_DOC).read_text(encoding="utf-8")
    section = text.split("## Deterministic stages", 1)[1].split("\n## ", 1)[0]
    stages = [int(match.group(1)) for match in re.finditer(r"(?m)^(\d+)\. ", section)]
    return stages


def adr_role_digest(root=ROOT):
    text = (root / ADR).read_text(encoding="utf-8")
    section = text.split("## Public capability ownership", 1)[1].split("\n## ", 1)[0]
    rows = [line.strip() for line in section.splitlines() if line.startswith("| H1-")]
    return sha("\n".join(rows)), [row.split("|")[1].strip() for row in rows]


def required_capabilities(root=ROOT):
    keys = set(H0_REQUIRED)
    for role in adr_role_digest(root)[1]:
        keys.update(ADR_ROLES.get(role, [f"<unmapped ADR role {role}>"]))
    return sorted(keys)


# ----- transcript helpers ------------------------------------------------------------------------------------------

def load_transcript(path):
    records = []
    for line in Path(path).read_text(encoding="utf-8").splitlines():
        if line.strip():
            records.append(json.loads(line))
    return records


def calls(records, capability=None, stage=None, session=None, label=None, status=None):
    out = []
    for record in records:
        if record.get("event") != "call":
            continue
        if capability and record["capability"] != capability:
            continue
        if stage and record["stage"] != stage:
            continue
        if session and record["session"] != session:
            continue
        if label and not record["label"].startswith(label):
            continue
        if status and record["outcome"].get("status") != status:
            continue
        out.append(record)
    return out


def result(record):
    return record["outcome"].get("result", {}) if record["outcome"].get("status") == "success" else None


def error_code(record):
    return record["outcome"].get("error", {}).get("machineCode") if record["outcome"].get("status") == "error" else None


def one(findings, rows, code, detail=""):
    findings.check(len(rows) >= 1, code, detail)
    return rows[0] if rows else None


def last(rows):
    return rows[-1] if rows else None


# ----- stage checks (transcript-derived) ---------------------------------------------------------------------------

def check_s03(f, records, session, t):
    inspect = [r for r in calls(records, "unity.host.project-profile.inspect@1.0", "S03", session) if result(r)]
    row = one(f, inspect, f"S03.{t}.no-effective-inspection")
    if not row:
        return
    value = result(row)
    f.check((value.get("editorVersion"), value.get("editorRevision")) == EDITOR, f"S03.{t}.editor-identity")
    f.check(value.get("mainThread") is True and value.get("entryPoint") == ENTRY_POINT, f"S03.{t}.worker-proof")
    status = [r for r in calls(records, "unity.lifecycle.operation-status@1.0", "S03", session)
              if r["arguments"].get("invocationId") == value.get("invocationId") and result(r)]
    f.check(status and result(status[-1]).get("status") == "completed", f"S03.{t}.lifecycle-recovery")


def check_s04(f, records, session, t, required):
    events = [r for r in records if r.get("event") == "discovery" and r.get("session") == session]
    row = one(f, events, f"S04.{t}.no-discovery")
    if row:
        missing = [key for key in required if key not in row["keys"]]
        f.check(not missing, f"S04.{t}.required-capability-absent", ",".join(missing))


def compiled_bindings(records, session):
    bindings = {}
    for record in calls(records, "unity.binding.compile@1.0", "S06", session, status="success"):
        bindings[record["arguments"]["subjectId"]] = (record["arguments"]["binding"], result(record)["extensionMutation"])
    return bindings


def check_s05_s08(f, records, session, t):
    bindings = compiled_bindings(records, session)
    f.check(len(bindings) >= 12, f"S06.{t}.compiled-slice-too-small", str(len(bindings)))
    selected = {("scene", SCENE)}
    for binding, _ in bindings.values():
        selected.add((binding["source"]["kind"], binding["source"]["logicalId"]))
        for component in binding["components"]:
            if component["kind"] == "renderer":
                selected.add(("material", component["materialId"]))
            if component["kind"] == "animator":
                selected.add(("animation-clip", component["clipId"]))
    resolved = {(r["arguments"]["kind"], r["arguments"]["logicalId"]) for r in
                calls(records, "unity.host.catalogue.resolve@1.0", "S05", session, status="success")}
    unresolved = sorted(f"{kind}:{logical}" for kind, logical in selected - resolved)
    f.check(not unresolved, f"S05.{t}.selected-id-not-resolved", ",".join(unresolved))
    pages = calls(records, "unity.host.catalogue.query@1.0", "S05", session, status="success")
    if f.check(pages, f"S05.{t}.catalogue-not-queried"):
        total = result(pages[0])["total"]
        seen = {(e["kind"], e["logicalId"]) for p in pages for e in result(p)["entries"]}
        f.check(len(seen) == total and len({result(p)["snapshotToken"] for p in pages}) == 1, f"S05.{t}.catalogue-pages-incomplete")

    apply = last(calls(records, "authoring.change.apply@1.0", "S07", session, label="apply:h1-gate.slice.create", status="success"))
    if f.check(apply, f"S07.{t}.no-authored-apply"):
        request = apply["arguments"]
        for label in ("plan:h1-gate.slice.create", "dry-run:h1-gate.slice.create"):
            prior = [r for r in calls(records, stage="S07", session=session, label=label, status="success")
                     if r["seq"] < apply["seq"] and r["arguments"] == request]
            f.check(prior, f"S07.{t}.{label.split(':')[0]}-missing-before-apply")
        payloads = {op.get("payloadBase64") for op in request["operations"] if op["kind"] == "put-extension"}
        objects = {op["id"] for op in request["operations"] if op["kind"] == "put-object"}
        f.check(all(mutation["payloadBase64"] in payloads for _, mutation in bindings.values()), f"S07.{t}.compiled-extension-not-applied")
        f.check(set(bindings) <= objects, f"S07.{t}.bound-object-not-authored")
    plan = last(calls(records, "unity.projection.plan@1.0", "S08", session, status="success"))
    if f.check(plan, f"S08.{t}.no-plan"):
        f.check(sorted(result(plan)["objectIds"]) == sorted(bindings), f"S08.{t}.plan-universe-differs")
    f.check(calls(records, "world.validation.current@1.0", "S08", session, status="success"), f"S08.{t}.no-preflight-validation")
    return result(plan) if plan else None, bindings


def observation_summary(value):
    return {k: value.get(k) for k in ("generationId", "inputDigest", "canonicalHash", "catalogueFingerprint", "graphDigest", "realizationDigest")}


def parity_report(record):
    value = result(record) if record else None
    return bool(value) and value.get("parity") is True and not value.get("items") and not value.get("diagnostics")


def check_s09_s11(f, records, session, t, plan):
    first = last(calls(records, "unity.host.projection.materialize@1.0", "S09", session, status="success"))
    if not f.check(first and plan, f"S09.{t}.no-effective-materialization"):
        return None, None
    m = result(first)
    f.check(m["active"] and m["current"] and m["inputDigest"] == plan["inputDigest"] and m["canonicalHash"] == plan["canonicalHash"],
            f"S09.{t}.observation-not-plan")
    f.check(sorted(n["objectId"] for n in m["nodes"]) == sorted(plan["objectIds"]), f"S09.{t}.observed-node-universe")
    observe = last(calls(records, "unity.host.projection.observe@1.0", "S09", session, status="success"))
    f.check(observe and observation_summary(result(observe)) == observation_summary(m), f"S09.{t}.observe-disagrees")
    f.check(parity_report(last(calls(records, "unity.host.projection.drift@1.0", "S10", session))), f"S10.{t}.first-publication-not-at-parity")
    s10_observed = result(observe) if observe else {"nodes": []}
    depth = {}
    parents = {n["objectId"]: n["parentObjectId"] for n in s10_observed["nodes"]}
    for key in parents:
        d, cursor = 1, parents[key]
        while cursor:
            d, cursor = d + 1, parents.get(cursor, "")
        depth[key] = d
    f.check(depth and max(depth.values()) >= 3, f"S10.{t}.hierarchy-depth")
    f.check(sum(1 for n in s10_observed["nodes"] if any("|clip=" in row for row in n["componentRows"])) >= 3, f"S10.{t}.humanoid-clips")
    f.check(sum(1 for n in s10_observed["nodes"] if n.get("relationships")) >= 1, f"S10.{t}.prefab-relationships")
    second = [r for r in calls(records, "unity.host.projection.materialize@1.0", "S11", session, status="success")]
    if f.check(second, f"S11.{t}.second-materialization-not-executed"):
        s = result(second[-1])
        for key in ("inputDigest", "canonicalHash", "catalogueFingerprint", "graphDigest", "realizationDigest"):
            f.check(s[key] == m[key], f"S11.{t}.semantic-delta", key)
    f.check(parity_report(last(calls(records, "unity.host.projection.drift@1.0", "S11", session))), f"S11.{t}.not-at-parity")
    return m, (result(second[-1]) if second else None)


def check_s12(f, records, session, t, second):
    applies = calls(records, "authoring.change.apply@1.0", "S12", session, status="success")
    f.check(len(applies) >= 3, f"S12.{t}.invalid-and-repair-not-authored")
    plans = calls(records, "unity.projection.plan@1.0", "S12", session)
    codes = [error_code(r) for r in plans if error_code(r)]
    f.check("projection.source-missing" in codes and "projection.reference-missing" in codes, f"S12.{t}.preflight-diagnostics", ",".join(map(str, codes)))
    rejected = [r for r in calls(records, "unity.host.projection.materialize@1.0", "S12", session) if error_code(r)]
    f.check(len(rejected) >= 2, f"S12.{t}.invalid-materialize-not-refused")
    retained = last(calls(records, "unity.host.projection.observe@1.0", "S12", session, label="observe:after-invalid", status="success"))
    if f.check(retained and second, f"S12.{t}.no-post-rejection-observation"):
        r = result(retained)
        f.check(r["generationId"] == second["generationId"] and r["current"] is False and r["graphDigest"] == second["graphDigest"],
                f"S12.{t}.false-publication")
    repaired = [r for r in calls(records, "unity.host.projection.materialize@1.0", "S12", session, status="success")]
    f.check(repaired and repaired[-1]["seq"] > (retained["seq"] if retained else 0), f"S12.{t}.repair-not-materialized")
    same = last(calls(records, "authoring.diff.compare@1.0", "S12", session, status="success"))
    f.check(same and result(same).get("sameAuthorableState") is True, f"S12.{t}.repair-not-authored-content")
    return [c for c in codes] + [error_code(r) for r in rejected]


def check_s13(f, records, session, t):
    edits = [r for r in records if r.get("event") == "operator-action" and r.get("action") == "unity-managed-edit" and r.get("session") == session]
    edit = one(f, edits, f"S13.{t}.no-unity-edit-stimulus")
    drifts = calls(records, "unity.host.projection.drift@1.0", "S13", session, status="success")
    after_edit = [r for r in drifts if edit and r["seq"] > edit["seq"]]
    if f.check(after_edit, f"S13.{t}.edit-not-observed"):
        items = [i for i in result(after_edit[0]).get("items", []) if i.get("objectId") == (edit or {}).get("object")]
        f.check(items and "transform" in items[0].get("fields", []), f"S13.{t}.edit-not-classified-as-supported-drift")
    proposal = last(calls(records, "unity.host.projection.import-proposal@1.0", "S13", session, status="success"))
    stale = [r for r in calls(records, "authoring.change.plan@1.0", "S13", session, label="plan:stale-proposal") if error_code(r)]
    code = error_code(stale[-1]) if stale else None
    if f.check(proposal and result(proposal).get("available") is True, f"S13.{t}.no-proposal"):
        request = result(proposal)["mutationRequest"]
        f.check(stale and stale[-1]["arguments"] == request, f"S13.{t}.stale-conflict-not-forced")
        disposition = stale[-1]["outcome"]["error"].get("context", {}).get("recovery", {}).get("disposition") if stale else None
        f.check(disposition == "same-lineage-replan", f"S13.{t}.no-hk08b-recovery")
        recovered = [r for r in calls(records, "authoring.change.apply@1.0", "S13", session, label="apply:recovered-proposal", status="success")]
        f.check(recovered and recovered[-1]["arguments"]["operations"] == request["operations"], f"S13.{t}.proposal-not-applied-through-h0")
        writer = [r for r in calls(records, "authoring.change.apply@1.0", "S13", session, label="apply:h1-gate.concurrent-writer", status="success")]
        f.check(writer and stale and writer[-1]["seq"] < stale[-1]["seq"], f"S13.{t}.no-concurrent-writer")
    remat = last(calls(records, "unity.host.projection.rematerialize@1.0", "S13", session, status="success"))
    f.check(remat, f"S13.{t}.not-rematerialized")
    f.check(parity_report(last(calls(records, "unity.host.projection.drift@1.0", "S13", session, label="drift:after-rematerialize"))), f"S13.{t}.not-at-parity")
    return code


def check_s14(f, records, t):
    a, b = f"{t}-a", f"{t}-b"
    capture = last(calls(records, "unity.host.checkpoint.capture@1.0", "S14", a, status="success"))
    if not f.check(capture, f"S14.{t}.no-checkpoint"):
        return None
    captured = result(capture)
    exits = [r for r in records if r.get("event") == "host-exit" and r.get("session") == a]
    removal = [r for r in records if r.get("event") == "operator-action" and r.get("action") == "remove-generated-output"]
    starts = [r for r in records if r.get("event") == "host-start" and r.get("session") == b]
    ordered = exits and removal and starts and capture["seq"] < exits[0]["seq"] < removal[0]["seq"] < starts[0]["seq"]
    f.check(ordered, f"S14.{t}.fresh-process-order")
    f.check(removal and any("ManagedScenes" in p for p in removal[0].get("paths", [])), f"S14.{t}.generated-output-not-removed")
    fresh = [r for r in records if r.get("event") == "call" and r.get("session") == b]
    restore = [r for r in fresh if r["capability"] == "unity.host.checkpoint.restore@1.0" and result(r)]
    if not f.check(restore, f"S14.{t}.no-public-restore"):
        return captured
    before = [r for r in fresh if r["seq"] < restore[0]["seq"]]
    # Canonical recovery truth must come from the checkpoint's H0 snapshot import only: no authoring or Unity-derived
    # canonical reconstruction may precede restore in the fresh process.
    forbidden = [r["capability"] for r in before if r["capability"].startswith(("authoring.change.", "authoring.snapshot.import", "unity.host.projection.import-proposal"))]
    f.check(not forbidden, f"S14.{t}.canonical-recovered-outside-checkpoint", ",".join(forbidden))
    empty = [r for r in before if r["capability"] == "world.summary@1.0" and result(r)]
    f.check(empty and result(empty[0])["world"]["revision"] == 0, f"S14.{t}.fresh-process-not-empty")
    restored = result(restore[0])
    f.check(restored.get("lineageDisposition") == "new-local-lineage", f"S14.{t}.restore-not-h0-snapshot-import")
    summaries = [r for r in fresh if r["capability"] == "world.summary@1.0" and r["seq"] > restore[0]["seq"] and result(r)]
    f.check(summaries and result(summaries[0])["world"]["hash"] == captured["canonicalHash"], f"S14.{t}.restored-canonical-differs")
    f.check([r for r in fresh if r["capability"] == "unity.host.projection.clean-rebuild@1.0" and result(r)], f"S14.{t}.no-composed-rebuild")
    observed = [r for r in fresh if r["capability"] == "unity.host.projection.observe@1.0" and r["stage"] == "S14" and result(r)]
    f.check(observed and result(observed[-1])["graphDigest"] == captured["observationGraphDigest"], f"S14.{t}.rebuild-graph-digest")
    f.check(parity_report(last([r for r in fresh if r["capability"] == "unity.host.projection.drift@1.0"])), f"S14.{t}.rebuild-not-at-parity")
    recapture = [r for r in fresh if r["capability"] == "unity.host.checkpoint.capture@1.0" and result(r)]
    f.check(recapture and result(recapture[-1])["observationReconstructionDigest"] == captured["observationReconstructionDigest"],
            f"S14.{t}.reconstruction-digest")
    return captured


def check_s16(f, records, ledger, t):
    loaded = [r for r in calls(records, "unity.host.projection.observe@1.0", "S16", f"{t}-b", status="success")]
    f.check(loaded and result(loaded[-1])["current"] is True, f"S16.{t}.slice-not-loaded")
    captures = [row for row in ledger if row["capability"] == "unity.host.projection.observe@1.0" and row["editorLogCaptured"]]
    f.check(captures, f"S16.{t}.no-editor-capture")
    owned = [row["invocationId"] for row in ledger if row["ownedErrorLines"]]
    f.check(not owned, f"S16.{t}.owned-error-diagnostics", ",".join(owned))


def check_ledger(f, records, ledger, t):
    """Every Unity process of the run belongs to a public composed call; nothing launched Unity privately."""
    expected = Counter()
    for record in records:
        if record.get("event") != "call":
            continue
        capability = record["capability"]
        launched = record["outcome"].get("status") == "success" or bool(record["outcome"].get("error", {}).get("context", {}).get("invocationId"))
        if capability in EDITOR_BOUND and launched:
            expected[capability] += 1
        if capability in NESTED and record["outcome"].get("status") == "success":
            for nested in NESTED[capability]:
                expected[nested] += 1
    actual = Counter(row["capability"] for row in ledger)
    f.check(set(actual) <= EDITOR_BOUND, f"LEDGER.{t}.non-public-unity-capability", ",".join(sorted(set(actual) - EDITOR_BOUND)))
    f.check(actual == expected, f"LEDGER.{t}.unity-launch-not-reconciled", canonical_json({"expected": expected, "actual": actual}))
    for row in ledger:
        f.check(row["entryPoint"] == ENTRY_POINT and row["executor"] in WORKER_EXECUTORS, f"LEDGER.{t}.non-product-launch", row["invocationId"])
        if row["resultPresent"]:
            f.check(row["resultExecutor"] == row["executor"] and row["mainThread"] == "true", f"LEDGER.{t}.result-identity", row["invocationId"])


def check_hosts(f, records, t):
    for record in records:
        if record.get("event") == "host-exit":
            f.check(record["exitCode"] == 0 and not record["extraStdout"] and not record["rawUnityOnHostStderr"], f"HOST.{t}.unclean-exit", record["session"])
        if record.get("event") == "host-start":
            command = " ".join(record.get("command", []))
            f.check(command.endswith("--h1-unity") and ("Arkus.Harness.Cli.dll" in command) == (t == "reference") and
                    "-executeMethod" not in command and "Editor/Unity" not in command, f"HOST.{t}.not-public-host", record["session"])


def world_anchor(records, session):
    summaries = calls(records, "world.summary@1.0", "S07", session, status="success")
    return result(summaries[-1]).get("world") if summaries else None


def verify_evidence(directory, candidate, root=ROOT):
    f = Findings()
    directory = Path(directory)
    stages = parity_stages(root)
    f.check(stages == list(range(1, len(stages) + 1)) and len(stages) >= 17, "UNIVERSE.stage-list-malformed", str(stages))
    digest, roles = adr_role_digest(root)
    f.check(digest == ADR_ROLE_TABLE_SHA256, "UNIVERSE.adr-role-table-changed", digest)
    required = required_capabilities(root)
    handled = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17}
    f.check(set(stages) <= handled, "UNIVERSE.stage-without-oracle", ",".join(str(s) for s in sorted(set(stages) - handled)))

    receipts = {}
    for number in (1, 2, 17):
        path = directory / f"stage-{number:02d}.json"
        if f.check(path.is_file(), f"S{number:02d}.receipt-missing"):
            receipts[number] = json.loads(path.read_text(encoding="utf-8"))
            f.check(receipts[number].get("candidateSha") == candidate and receipts[number].get("result") == "GREEN", f"S{number:02d}.receipt-not-green-for-candidate")
    if 1 in receipts:
        r = receipts[1]
        f.check(r.get("headSha") == candidate and r.get("cleanTree") is True and r.get("editor") == "6000.3.24f1 (4e7b9b5b6244)" and
                r.get("packagesLockMatchesCandidate") is True and r.get("vaultVerified") is True, "S01.clean-baseline-facts")
    if 2 in receipts:
        f.check(receipts[2].get("failed") == 0 and receipts[2].get("passed", 0) > 0 and receipts[2].get("filter") == "none", "S02.remote-validation-facts")
    if 17 in receipts:
        f.check(all(receipts[17].get(k) == "GREEN" for k in ("reconciliation", "scope", "workflow", "negativeControls")), "S17.closure-facts")

    bootstrap = directory / "transcript-bootstrap.jsonl"
    if f.check(bootstrap.is_file(), "S03.bootstrap-transcript-missing"):
        records = load_transcript(bootstrap)
        check_s03(f, records, "reference-bootstrap", "bootstrap")
        check_s04(f, records, "reference-bootstrap", "bootstrap", required)
        check_hosts(f, records, "reference")

    derived = {}
    for t in TRANSPORTS:
        path = directory / f"transcript-{t}.jsonl"
        ledger_path = directory / f"ledger-{t}.json"
        if not f.check(path.is_file() and ledger_path.is_file(), f"EVIDENCE.{t}.transcript-or-ledger-missing"):
            continue
        records = load_transcript(path)
        ledger = json.loads(ledger_path.read_text(encoding="utf-8"))
        check_hosts(f, records, t)
        check_s03(f, records, f"{t}-a", t)
        check_s04(f, records, f"{t}-a", t, required)
        check_s04(f, records, f"{t}-b", t, required)
        plan, bindings = check_s05_s08(f, records, f"{t}-a", t)
        first, second = check_s09_s11(f, records, f"{t}-a", t, plan)
        codes = check_s12(f, records, f"{t}-a", t, second)
        stale = check_s13(f, records, f"{t}-a", t)
        captured = check_s14(f, records, t)
        check_s16(f, records, ledger, t)
        check_ledger(f, records, ledger, t)
        derived[t] = {
            "canonical": world_anchor(records, f"{t}-a"),
            "plan": plan and {k: plan.get(k) for k in ("inputDigest", "canonicalHash", "catalogueFingerprint")},
            "first": first and {k: first.get(k) for k in ("inputDigest", "canonicalHash", "graphDigest")},
            "codes": codes, "stale": stale,
            "captured": captured and {k: captured.get(k) for k in ("canonicalHash", "revision", "planInputDigest", "observationGraphDigest", "observationReconstructionDigest")},
        }

    # Stage 15: equivalence recomputed from both transcripts, not taken from the driver's comparison file.
    if all(t in derived for t in TRANSPORTS):
        for key in ("canonical", "plan", "first", "codes", "stale", "captured"):
            f.check(derived["reference"][key] == derived["mcp"][key] and derived["reference"][key] is not None, "S15.transport-divergence", key)
    equivalence = directory / "transport-equivalence.json"
    if f.check(equivalence.is_file(), "S15.equivalence-record-missing"):
        f.check(json.loads(equivalence.read_text(encoding="utf-8")).get("result") == "GREEN", "S15.equivalence-record-red")
    f.finish(f"evidence candidate={candidate} stages={len(stages)} transports={','.join(TRANSPORTS)}")


# ----- workflow static check ---------------------------------------------------------------------------------------

FORBIDDEN_WORKFLOW = [
    (r"-executeMethod", "direct-editor-entry-point"),
    (r"/opt/unity/Editor/Unity", "direct-editor-binary"),
    (r"--testPlatforms|-runTests|unity-test-runner|testFilter", "unity-test-helper"),
    (r"PINNED_GAMECI_BIN[}\"]*\s+test\b", "gameci-test-run"),
    (r"h1-1[01]-unity-stage\.sh|H1RepresentativeSliceTests|H1ProjectionReconciliationComposedTests", "predecessor-private-stage"),
]
REQUIRED_WORKFLOW = [
    (r"h1-gate-scenario\.py bootstrap --transport reference", "S03-S04-bootstrap"),
    (r"h1-gate-scenario\.py scenario --transport reference", "reference-scenario"),
    (r"h1-gate-scenario\.py scenario --transport mcp", "mcp-scenario"),
    (r"h1-gate-scenario\.py compare", "S15-compare"),
    (r"h1-gate-verify\.py evidence", "evidence-verifier"),
    (r"h1-gate-negative-controls\.py evidence", "evidence-negative-controls"),
    (r"h1-gate-verify\.py reconcile", "reconciliation"),
    (r"dotnet test tests/Arkus\.Harness\.Tests/Arkus\.Harness\.Tests\.csproj", "S02-remote-validation"),
    (r"verify_h1_asset_vault\.py", "S01-vault-verify"),
]


def strip_comments(text):
    return "\n".join(line for line in text.splitlines() if not line.lstrip().startswith("#"))


def verify_workflow(path, root=ROOT):
    f = Findings()
    text = strip_comments((root / path).read_text(encoding="utf-8"))
    for workflow in sorted({path, TRIAL_WORKFLOW}):
        body = strip_comments((root / workflow).read_text(encoding="utf-8"))
        for pattern, code in FORBIDDEN_WORKFLOW:
            f.check(not re.search(pattern, body), f"WORKFLOW.private-unity-path.{code}", workflow)
    for pattern, code in REQUIRED_WORKFLOW:
        f.check(re.search(pattern, text), f"WORKFLOW.stage-not-executed.{code}")
    unity_image_uses = re.findall(r"\$\{\{? ?env\.PINNED_UNITY_IMAGE ?\}?\}|\$\{PINNED_UNITY_IMAGE\}|\"\$\{PINNED_UNITY_IMAGE\}\"", text)
    f.check(len(unity_image_uses) >= 1, "WORKFLOW.unity-image-not-pinned")
    f.finish(f"workflow {path}")


# ----- diff scope --------------------------------------------------------------------------------------------------

GATE_OWNED = [
    r"^Docs/evidence/WP-H1-GATE/",
    r"^Docs/workpacks/H1/WP-H1-GATE\.md$",
    r"^scripts/h1-gate-[a-z-]+\.(py|sh)$",
    r"^scripts/arkus-verify-exact-sha-base\.sh$",
    r"^\.github/workflows/h1-gate-[a-z-]+\.yml$",
]


def verify_scope(base, root=ROOT):
    f = Findings()
    changed = subprocess.check_output(["git", "diff", "--name-only", f"{base}...HEAD"], cwd=root, text=True).split()
    outside = [path for path in changed if not any(re.search(pattern, path) for pattern in GATE_OWNED)]
    f.check(not outside, "SCOPE.product-or-predecessor-path-changed", ",".join(outside))
    if "scripts/arkus-verify-exact-sha-base.sh" in changed:
        diff = subprocess.check_output(["git", "diff", f"{base}...HEAD", "--", "scripts/arkus-verify-exact-sha-base.sh"], cwd=root, text=True)
        added = [line[1:] for line in diff.splitlines() if line.startswith("+") and not line.startswith("+++")]
        removed = [line for line in diff.splitlines() if line.startswith("-") and not line.startswith("---")]
        f.check(not removed and all("WP-H1-GATE" in line or "h1-gate-verify-exact-sha" in line or line.strip() in ("", "return", "fi", ";;")
                                    for line in added), "SCOPE.dispatcher-change-not-additive-gate-route")
    f.finish(f"scope base={base} changed={len(changed)}")


# ----- residual / dependency reconciliation ------------------------------------------------------------------------

RESIDUAL_SOURCES = [
    ("Docs/workpacks/H1/WP-H1-*.md", r"^## Residual risks"),
    ("Docs/evidence/WP-H1-*/RESIDUAL_RISK.md", None),
    ("Docs/evidence/WP-H1-*/*.md", r"^## .*[Rr]esidual"),
    ("Docs/engineering/H1_RISK_AND_RESIDUAL_PLAN.md", None),
]
LEDGER = "Docs/engineering/RESIDUAL_LEDGER.md"


def markdown_items(text, paragraphs=False):
    """Table data rows and top-level list items; in a selected residual section, prose paragraphs too."""
    items, in_table, paragraph = [], False, []

    def flush():
        if paragraphs and paragraph:
            items.append(" ".join(paragraph))
        paragraph.clear()

    for line in text.splitlines():
        stripped = line.strip()
        if stripped.startswith("|"):
            flush()
            if not in_table:
                in_table = True
                continue
            if re.fullmatch(r"\|[\s:|-]+\|", stripped):
                continue
            items.append(stripped)
            continue
        in_table = False
        if re.match(r"^(- |\* |\d+\. )", line):
            flush()
            items.append(stripped)
        elif not stripped or stripped.startswith("#"):
            flush()
        elif not line.startswith(" "):
            paragraph.append(stripped)
        elif items and not paragraph:
            items[-1] = items[-1] + " " + stripped
        else:
            paragraph.append(stripped)
    flush()
    return items


def section(text, heading_pattern):
    if heading_pattern is None:
        return text
    out, capture, level = [], False, 0
    for line in text.splitlines():
        heading = re.match(r"^(#+) ", line)
        if heading:
            if capture and len(heading.group(1)) <= level:
                capture = False
            if re.match(heading_pattern, line):
                capture, level = True, len(heading.group(1))
                continue
        if capture:
            out.append(line)
    return "\n".join(out)


def discover_residuals(root=ROOT):
    discovered = {}
    for pattern, heading in RESIDUAL_SOURCES:
        for path in sorted(root.glob(pattern)):
            relative = path.relative_to(root).as_posix()
            if relative.startswith("Docs/evidence/WP-H1-GATE/"):
                continue
            if pattern.endswith("/*.md") and path.name == "RESIDUAL_RISK.md":
                continue
            for item in markdown_items(section(path.read_text(encoding="utf-8"), heading), paragraphs=heading is not None):
                discovered[f"{relative}#{sha(item)[:16]}"] = item
    ledger = (root / LEDGER).read_text(encoding="utf-8")
    for line in ledger.splitlines():
        match = re.match(r"^\| `(R-[A-Z0-9-]+)` \|", line)
        if match and re.search(r"\bH1\b|H1-|GATE", line.split("|", 3)[3] if line.count("|") > 3 else line):
            discovered[f"{LEDGER}#{match.group(1)}"] = line.strip()
    return discovered


def discover_dependencies(root=ROOT):
    return sorted(path.stem for path in (root / "Docs/workpacks/H1").glob("WP-H1-*.md") if path.stem != "WP-H1-GATE")


VALID_DISPOSITIONS = {"CARRIED_FORWARD", "CLOSED_BY_ACCEPTED_OWNER", "CONFIRMED_BY_GATE", "SATISFIED_BY_GATE_EVIDENCE", "PROCESS_RECORD"}


def verify_reconcile(root=ROOT, reconciliation_path=None):
    f = Findings()
    path = Path(reconciliation_path) if reconciliation_path else root / RECONCILIATION
    data = json.loads(path.read_text(encoding="utf-8"))
    discovered = discover_residuals(root)
    rows = {row["key"]: row for row in data["residuals"]}
    missing = sorted(set(discovered) - set(rows))
    stale = sorted(set(rows) - set(discovered) - {row["key"] for row in data["residuals"] if row.get("externalSource")})
    f.check(not missing, "RECONCILE.inherited-residual-omitted", ",".join(missing[:10]))
    f.check(not stale, "RECONCILE.residual-row-without-source", ",".join(stale[:10]))
    for key, row in rows.items():
        f.check(row.get("disposition") in VALID_DISPOSITIONS and row.get("owner") and row.get("basis"), "RECONCILE.row-unclassified", key)
        if key in discovered:
            f.check(row.get("itemSha256") == sha(discovered[key]), "RECONCILE.source-text-changed", key)
    external = [row for row in data["residuals"] if row.get("externalSource")]
    f.check(any("5845036679" in row["externalSource"] for row in external), "RECONCILE.h1-11-owner-residual-omitted")
    dependencies = {row["wp"]: row for row in data["dependencies"]}
    expected = discover_dependencies(root)
    f.check(sorted(dependencies) == expected, "RECONCILE.dependency-omitted", ",".join(sorted(set(expected) ^ set(dependencies))))
    for wp, row in dependencies.items():
        f.check(re.fullmatch(r"[0-9a-f]{40}", row.get("acceptedCandidate", "")) and row.get("verdict") and row.get("mergeSha"), "RECONCILE.dependency-identity", wp)
    stage_rows = {row["stage"] for row in data["stages"]}
    f.check(stage_rows == set(parity_stages(root)), "RECONCILE.gate-stage-omitted", ",".join(str(s) for s in sorted(set(parity_stages(root)) ^ stage_rows)))
    f.finish(f"reconcile residuals={len(rows)} discovered={len(discovered)} dependencies={len(dependencies)} stages={len(stage_rows)}")


# ----- AI trial ----------------------------------------------------------------------------------------------------

def verify_trial(directory, candidate, root=ROOT):
    f = Findings()
    directory = Path(directory)
    record_path = directory / "trial-record.json"
    transcript_path = directory / "trial-transcript.jsonl"
    if not f.check(record_path.is_file() and transcript_path.is_file(), "TRIAL.artifacts-missing"):
        f.finish("trial")
    record = json.loads(record_path.read_text(encoding="utf-8"))
    events = load_transcript(transcript_path)
    f.check(record.get("candidateSha") == candidate, "TRIAL.sha-mismatch", str(record.get("candidateSha")))
    f.check(record.get("transcriptSha256") == sha(transcript_path.read_bytes()), "TRIAL.transcript-digest-mismatch")
    f.check(record.get("transport") == "MCP_STDIO" and record.get("implementationSourceAccess") == "NONE", "TRIAL.not-public-mcp")
    discovered = set()
    offered = set()
    issued = set()
    for event in events:
        if event.get("event") == "discovery":
            discovered = set(event.get("canonicalKeys", []))
            offered = set(event.get("offeredToolNames", []))
        if event.get("event") == "model-message":
            for call in event.get("toolCalls", []):
                issued.add(call["id"])
        if event.get("event") == "tool-call":
            f.check(event.get("callId") in issued, "TRIAL.call-not-issued-by-model", str(event.get("callId")))
            f.check(event.get("toolName") in offered, "TRIAL.hidden-or-private-call", str(event.get("toolName")))
            f.check(event.get("canonicalKey") in discovered, "TRIAL.call-outside-discovery", str(event.get("canonicalKey")))
    used = {e.get("canonicalKey") for e in events if e.get("event") == "tool-call"}
    for key in ("unity.host.projection.materialize@1.0", "unity.host.projection.observe@1.0",
                "unity.host.checkpoint.capture@1.0", "authoring.change.apply@1.0"):
        f.check(key in used, "TRIAL.required-public-flow-not-exercised", key)
    f.check(any(k in used for k in ("unity.host.checkpoint.restore@1.0", "unity.host.projection.clean-rebuild@1.0")), "TRIAL.no-rebuild")
    diagnostics = [e for e in events if e.get("event") == "tool-result" and e.get("status") == "error" and str(e.get("machineCode", "")).startswith(("projection.", "unity."))]
    f.check(diagnostics, "TRIAL.no-structured-unity-diagnostic-consumed")
    final = record.get("final", {})
    f.check(re.fullmatch(r"[0-9a-f]{64}", str(final.get("canonicalHash", ""))) and final.get("activeGenerationId") and final.get("graphDigest"),
            "TRIAL.no-closing-evidence")
    f.check(record.get("verdict") == "PASS", "TRIAL.verdict-not-pass", str(record.get("verdict")))
    f.finish(f"trial candidate={candidate} calls={len([e for e in events if e.get('event') == 'tool-call'])}")


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    sub = parser.add_subparsers(dest="command", required=True)
    e = sub.add_parser("evidence"); e.add_argument("--dir", required=True); e.add_argument("--candidate-sha", required=True)
    w = sub.add_parser("workflow"); w.add_argument("--file", default=WORKFLOW)
    s = sub.add_parser("scope"); s.add_argument("--base", required=True)
    r = sub.add_parser("reconcile"); r.add_argument("--file", default=None)
    t = sub.add_parser("trial"); t.add_argument("--dir", required=True); t.add_argument("--candidate-sha", required=True)
    sub.add_parser("adr-digest")
    args = parser.parse_args()
    if args.command == "evidence":
        verify_evidence(args.dir, args.candidate_sha)
    elif args.command == "workflow":
        verify_workflow(args.file)
    elif args.command == "scope":
        verify_scope(args.base)
    elif args.command == "reconcile":
        verify_reconcile(reconciliation_path=args.file)
    elif args.command == "trial":
        verify_trial(args.dir, args.candidate_sha)
    elif args.command == "adr-digest":
        print(adr_role_digest()[0], adr_role_digest()[1])


if __name__ == "__main__":
    main()
