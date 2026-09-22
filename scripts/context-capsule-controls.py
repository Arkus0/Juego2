#!/usr/bin/env python3
"""Independent defect-injection and adoption-wiring controls for Context Capsule v1.

Representative semantic expectations are deliberately test-only. They are
independent of the audited capsule/index and are not imported by the production
checker. Generic natural-language equivalence remains a Reviewer/escalation
responsibility.
"""
from __future__ import annotations

import copy
import hashlib
import importlib.util
import json
from pathlib import Path
import subprocess
import sys
import tempfile

ROOT = Path(__file__).resolve().parents[1]
CHECKER_PATH = ROOT / "scripts" / "context-capsule-check.py"
spec = importlib.util.spec_from_file_location("context_capsule_check", CHECKER_PATH)
if spec is None or spec.loader is None:
    raise RuntimeError("could not load context-capsule-check.py")
checker = importlib.util.module_from_spec(spec)
spec.loader.exec_module(checker)

FULL_VALIDATION_COMMANDS = [
    "python3 scripts/context-capsule-check.py --self-test",
    "python3 scripts/context-capsule-controls.py",
    "python3 scripts/context-capsule-omission-controls.py",
    "python3 scripts/context-capsule-pa-semantic-controls.py",
    "python3 scripts/context-capsule-check.py --audit-index --repo-root .",
]

# Test-only oracle for the actual representative boundaries selected by CTX-02.
# Exact prose/path expectations live here so production validation does not grow
# a general semantic registry. Changing these fixtures is review-visible and must
# be justified against the external authoritative sources.
REPRESENTATIVE_ORACLE = {
    "WP-HK-GATE": {
        "authority_paths": {
            "Docs/workpacks/HK/WP-HK-GATE.md",
            "Docs/evidence/WP-HK-GATE/VERDICT.md",
            "Docs/evidence/WP-HK-GATE/PROOF_MATRIX.md",
            "Docs/evidence/WP-HK-GATE/RESIDUAL_RISK.md",
        },
        "mandatory_reads": set(),
        "exports": {
            "h0-authoring-readiness": (
                "H0 passed the accepted AI-authoring readiness gate and may be consumed as the engine-neutral authoring foundation.",
                "WP-HK-GATE / Accepted gate result",
            ),
            "engine-neutral-boundary": (
                "Canonical H0 semantics remain engine-neutral; Unity is downstream of the accepted bridge boundary.",
                "WP-HK-GATE / Product-boundary proof",
            ),
            "unity-work-authorized": (
                "Independent HK-GATE PASS authorizes the downstream Unity bridge track, not direct gameplay implementation.",
                "WP-HK-GATE / PASS consequence",
            ),
        },
        "exclusions": {
            "no-gameplay-authorization": (
                "HK-GATE PASS does not authorize gameplay implementation directly.",
                "WP-HK-GATE / PASS consequence",
            ),
            "no-new-concurrency-claim": (
                "Per-resource concurrency, automatic disjoint-write merge and multi-process coordination remain post-GATE unless later evidence changes ownership.",
                "WP-HK-GATE / PASS consequence",
            ),
        },
        "reopen": [
            "Concrete evidence shows the accepted gate claim is false or does not cover the effective downstream path.",
            "A downstream Unity integration requires changing an accepted H0 engine-neutral boundary rather than adding a conforming adapter.",
        ],
        "escalate": [
            "The consumer needs an exact H0 semantic/proof detail not carried by this boundary capsule; open the bound proof matrix/residual/verdict and any transitive source they invoke.",
            "Live GitHub or accepted completion metadata contradicts this accepted identity.",
            "A material question could change whether HK-GATE actually covers the downstream claim.",
        ],
        "directional": {},
    },
    "WP-CITY-03": {
        "authority_paths": {"Docs/workpacks/CITY/WP-CITY-03.md"},
        "mandatory_reads": {checker.CITY_PRODUCT_SEED},
        "exports": {
            "selected-retained-seed": (
                "CITY-03 accepted one exact retained keeper seed and owns its hard playable boundary.",
                "WP-CITY-03 / Objective + Work",
            ),
            "city04-executes-not-redesigns": (
                "CITY-04 receives a bounded seed specification and must build/measure that seed rather than choose a different city slice.",
                "WP-CITY-03 / Definition of Done",
            ),
        },
        "exclusions": {
            "not-spatial-spec": (
                "This capsule does not encode, summarize or replace the seed geometry, streets, parcels, scenarios, expansion seams or measurement pack.",
                "Docs/production/CITY_PRODUCT_SEED.md",
            ),
            "no-unity-authority": (
                "Unity realization cannot become the source of CITY semantics.",
                "WP-CITY-04 / Scope",
            ),
        },
        "reopen": [
            "Measured CITY-04 evidence falsifies a downstream CITY-03-owned hypothesis and routes revision to CITY-03.",
            "Concrete evidence appears to contradict an accepted CITY-00 fact; CITY-04 must stop and raise an explicit predecessor amendment.",
        ],
        "escalate": [
            "Any geometry, route, site, scenario, measurement or expansion-seam detail is needed.",
            "The consumer would otherwise construct from capsule prose instead of the exact product seed.",
            "Live accepted identity or source fingerprints do not match.",
        ],
        "directional": {},
    },
    "WP-PA-03": {
        "authority_paths": {"Docs/research/living-world/results/PA-03.md"},
        "mandatory_reads": set(),
        "exports": {
            "directed-affect": (
                "Trust, affinity and fear are directed stances; changing A→B does not mirror or mutate B→A.",
                "PA-03 / Directionality and reciprocity",
            ),
            "relationships-change-behavior": (
                "Material relationship differences can change an explainable action, target or opportunity choice rather than only dialogue flavour.",
                "WP-PA-03 / Acceptance",
            ),
            "action-specific-consumers": (
                "Relationship consumers are action-specific; there is no universal sign/formula across all actions.",
                "PA-03 / Relationship semantics are action-specific",
            ),
            "current-state-not-biography": (
                "PA-03 owns current causal relationship state and bounded provenance, not unbounded relationship biography.",
                "PA-03 / Current relationship state vs biography",
            ),
            "bounded-social-discovery-inherited": (
                "PA-03 preserves PA-02 bounded discovery by rejecting default global/N-hop social traversal for target discovery.",
                "PA-03 / Minimal relationship vocabulary recommendation",
            ),
        },
        "exclusions": {
            "no-universal-friendship-score": (
                "A universal friendship/opinion scalar as canonical social state is rejected.",
                "PA-03 / vocabulary table",
            ),
            "no-implicit-symmetry": (
                "Implicit symmetry for actor stance is rejected.",
                "PA-03 / vocabulary table",
            ),
            "no-unbounded-edge-biography": (
                "Unbounded relationship biography inside every edge is rejected.",
                "PA-03 / vocabulary table",
            ),
            "no-global-n-hop": (
                "Default global/N-hop social traversal is rejected because it violates inherited PA-02 bounded-discovery intent.",
                "PA-03 / vocabulary table",
            ),
            "additional-affect-is-later": (
                "Respect, loyalty, attraction, jealousy and other dimensions remain LATER until a concrete behavior needs them.",
                "PA-03 / vocabulary table + anti-inflation rule",
            ),
        },
        "reopen": [
            "Concrete counterexample shows the accepted directed/action-specific semantics are false or inapplicable.",
            "A later accepted behavior cannot be expressed cleanly by adopted dimensions/roles/obligations and therefore justifies reopening a LATER dimension.",
        ],
        "escalate": [
            "A consumer needs exact relationship semantics beyond the exported invariants/status table.",
            "A relationship disposition would be reclassified, especially LATER/REJECT.",
            "A consumer proposes mirrored stance, global/N-hop target discovery, or another contradiction with inherited PA-02.",
            "Live accepted identity/source bytes contradict the capsule.",
        ],
        "directional": {
            "trust": ("trust(A -> B)", "trust(B -> A)"),
            "affinity": ("affinity(A -> B)", "affinity(B -> A)"),
            "fear": ("fear(A -> B)", "fear(B -> A)"),
        },
    },
}


def blob_sha(data: bytes) -> str:
    return hashlib.sha1(f"blob {len(data)}\0".encode() + data).hexdigest()


def expect_failure(fn, needle: str) -> None:
    try:
        fn()
    except checker.CapsuleError as exc:
        if needle not in str(exc):
            raise AssertionError(f"expected {needle!r}, got {exc!r}") from exc
    else:
        raise AssertionError(f"expected failure containing {needle!r}")


def statement_map(capsule: dict, field: str) -> dict[str, tuple[str, str | None]]:
    result: dict[str, tuple[str, str | None]] = {}
    for row in capsule.get(field, []):
        if not isinstance(row, dict) or not isinstance(row.get("id"), str):
            continue
        result[row["id"]] = (row.get("statement"), row.get("source_pointer"))
    return result


def directional_map(capsule: dict) -> dict[str, tuple[str, str]]:
    result: dict[str, tuple[str, str]] = {}
    for row in capsule.get("directional_semantics", []):
        if not isinstance(row, dict) or not isinstance(row.get("id"), str):
            continue
        if row.get("must_remain_distinct") is not True:
            raise checker.CapsuleError(
                f"independent representative oracle: {capsule.get('capsule_id')} directional {row.get('id')} lost must_remain_distinct"
            )
        result[row["id"]] = (row.get("forward"), row.get("reverse"))
    return result


def source_paths(capsule: dict, field: str) -> set[str]:
    return {
        row.get("path")
        for row in capsule.get(field, [])
        if isinstance(row, dict) and isinstance(row.get("path"), str)
    }


def assert_representative_inventory(capsules: dict[str, dict]) -> None:
    missing = sorted(set(REPRESENTATIVE_ORACLE) - set(capsules))
    if missing:
        raise checker.CapsuleError(
            f"independent representative oracle: indexed representative capsule(s) missing: {missing}"
        )


def assert_representative_semantics(capsule: dict) -> None:
    cid = capsule.get("capsule_id")
    expected = REPRESENTATIVE_ORACLE.get(cid)
    if expected is None:
        raise checker.CapsuleError(f"independent representative oracle has no fixture for {cid}")
    if source_paths(capsule, "authoritative_sources") != expected["authority_paths"]:
        raise checker.CapsuleError(
            f"independent representative oracle: {cid} authoritative source inventory mismatch"
        )
    if source_paths(capsule, "mandatory_source_reads") != expected["mandatory_reads"]:
        raise checker.CapsuleError(
            f"independent representative oracle: {cid} mandatory source-read inventory mismatch"
        )
    if statement_map(capsule, "exported_guarantees") != expected["exports"]:
        raise checker.CapsuleError(
            f"independent representative oracle: {cid} exported_guarantees material content mismatch"
        )
    if statement_map(capsule, "exclusions_nonclaims") != expected["exclusions"]:
        raise checker.CapsuleError(
            f"independent representative oracle: {cid} exclusions_nonclaims material content mismatch"
        )
    if capsule.get("reopen_conditions") != expected["reopen"]:
        raise checker.CapsuleError(
            f"independent representative oracle: {cid} reopen_conditions material content mismatch"
        )
    if capsule.get("escalate_if") != expected["escalate"]:
        raise checker.CapsuleError(
            f"independent representative oracle: {cid} escalate_if material content mismatch"
        )
    if directional_map(capsule) != expected["directional"]:
        raise checker.CapsuleError(
            f"independent representative oracle: {cid} directional_semantics material content mismatch"
        )


def load_indexed_capsules(repo_root: Path) -> tuple[dict, dict[str, dict]]:
    index = checker.load_json(repo_root / checker.CANONICAL_INDEX_PATH)
    capsules: dict[str, dict] = {}
    for entry in index.get("entries", []):
        if not isinstance(entry, dict):
            continue
        cid = entry.get("capsule_id")
        path = entry.get("path")
        if isinstance(cid, str) and isinstance(path, str):
            capsules[cid] = checker.load_json(repo_root / path)
    return index, capsules


def require_text(path: Path, needles: list[str]) -> None:
    try:
        text = path.read_text(encoding="utf-8")
    except FileNotFoundError as exc:
        raise checker.CapsuleError(f"adoption wiring missing file: {path.relative_to(ROOT)}") from exc
    for needle in needles:
        if needle not in text:
            raise checker.CapsuleError(
                f"adoption wiring missing {needle!r} in {path.relative_to(ROOT)}"
            )


def require_full_surface(path: Path) -> None:
    require_text(path, FULL_VALIDATION_COMMANDS)


def validate_adoption_wiring(repo_root: Path) -> None:
    profile_path = repo_root / "Docs/engineering/context-bootstrap-profiles.json"
    try:
        profiles = json.loads(profile_path.read_text(encoding="utf-8"))
    except (FileNotFoundError, json.JSONDecodeError) as exc:
        raise checker.CapsuleError("adoption wiring cannot read context-bootstrap-profiles.json") from exc
    if profiles.get("capsule_protocol") != checker.CANONICAL_PROTOCOL_PATH:
        raise checker.CapsuleError("bootstrap profile root must discover CONTEXT_CAPSULE_V1.md")
    role_map = profiles.get("profiles")
    if not isinstance(role_map, dict):
        raise checker.CapsuleError("bootstrap profiles missing profiles object")
    required_conditional = {
        "worker": "accepted_contract_capsules",
        "repair_worker": "accepted_contract_capsules",
        "reviewer": "accepted_contract_capsules",
        "docsync": "accepted_pa_result_after_ctx02",
    }
    for role, key in required_conditional.items():
        role_data = role_map.get(role)
        conditional = role_data.get("conditional_reads") if isinstance(role_data, dict) else None
        if not isinstance(conditional, dict) or key not in conditional:
            raise checker.CapsuleError(f"bootstrap profile {role} no longer discovers capsule path {key}")

    require_text(
        repo_root / "Docs/engineering/CONTEXT_BOOTSTRAP_V1.md",
        [checker.CANONICAL_PROTOCOL_PATH, "WP-CTX-02"],
    )
    require_text(
        repo_root / "Docs/engineering/WORKER_REVIEW_PROTOCOL.md",
        ["Version: 1.9", "CONTEXT_CAPSULE_V1.md", "validated capsule"],
    )
    require_text(repo_root / "AGENTS.md", ["CONTEXT_CAPSULE_V1.md", "WP-CTX-02"])
    for relative in (
        ".agents/skills/implement-workpack/SKILL.md",
        ".agents/skills/repair-workpack/SKILL.md",
        ".agents/skills/validate-workpack/SKILL.md",
        ".agents/skills/update-handoff/SKILL.md",
    ):
        require_text(repo_root / relative, ["CONTEXT_CAPSULE_V1.md"])

    # The protocol, DocSync skill and CI workflow all claim/define the canonical
    # validation surface. They must name the same complete command set.
    require_full_surface(repo_root / checker.CANONICAL_PROTOCOL_PATH)
    require_full_surface(repo_root / ".agents/skills/update-handoff/SKILL.md")
    workflow = repo_root / ".github/workflows/context-capsule-validation.yml"
    require_full_surface(workflow)

    # Any other role guide that explicitly calls a command list the full CTX-02
    # validation surface inherits the same exact command-set obligation.
    for skill in sorted((repo_root / ".agents/skills").glob("*/SKILL.md")):
        text = skill.read_text(encoding="utf-8")
        if "full CTX-02 validation surface" in text:
            require_full_surface(skill)


def run_audit_cli(repo: Path, index: str = checker.CANONICAL_INDEX_PATH) -> subprocess.CompletedProcess[str]:
    return subprocess.run(
        [
            sys.executable,
            str(CHECKER_PATH),
            "--audit-index",
            "--repo-root",
            str(repo),
            "--index",
            index,
        ],
        check=False,
        capture_output=True,
        text=True,
    )


def assert_cli_red(repo: Path, needle: str, *, index: str = checker.CANONICAL_INDEX_PATH) -> None:
    result = run_audit_cli(repo, index=index)
    if result.returncode == 0:
        raise AssertionError(f"expected --audit-index to fail for {needle}")
    if needle not in result.stderr:
        raise AssertionError(f"unexpected --audit-index failure for {needle!r}: {result.stderr}")


def assert_audit_red(capsule_path: Path, mutated_capsule: dict, needle: str) -> None:
    capsule_path.write_text(json.dumps(mutated_capsule), encoding="utf-8")
    assert_cli_red(capsule_path.parents[3], needle)


def run_representative_controls() -> None:
    index, capsules = load_indexed_capsules(ROOT)
    assert_representative_inventory(capsules)

    # A coordinated index omission of a representative boundary must not redefine
    # the independent test oracle's expected inventory.
    missing_inventory = dict(capsules)
    missing_inventory.pop("WP-CITY-03")
    expect_failure(
        lambda: assert_representative_inventory(missing_inventory),
        "indexed representative capsule(s) missing",
    )

    for cid in sorted(REPRESENTATIVE_ORACLE):
        capsule = capsules[cid]
        checker.validate_capsule(ROOT, capsule)
        assert_representative_semantics(capsule)

    guarantee_inversion = copy.deepcopy(capsules["WP-HK-GATE"])
    guarantee_row = next(
        row
        for row in guarantee_inversion["exported_guarantees"]
        if row["id"] == "h0-authoring-readiness"
    )
    guarantee_pointer = guarantee_row["source_pointer"]
    guarantee_row["statement"] = "HK-GATE authorizes direct gameplay implementation without a downstream bridge boundary."
    assert guarantee_row["source_pointer"] == guarantee_pointer
    checker.validate_capsule(ROOT, guarantee_inversion)
    expect_failure(
        lambda: assert_representative_semantics(guarantee_inversion),
        "exported_guarantees material content mismatch",
    )

    exclusion_inversion = copy.deepcopy(capsules["WP-CITY-03"])
    exclusion_row = next(
        row
        for row in exclusion_inversion["exclusions_nonclaims"]
        if row["id"] == "not-spatial-spec"
    )
    exclusion_pointer = exclusion_row["source_pointer"]
    exclusion_row["statement"] = (
        "This capsule fully replaces CITY_PRODUCT_SEED.md and is sufficient for geometry and construction."
    )
    assert exclusion_row["source_pointer"] == exclusion_pointer
    checker.validate_capsule(ROOT, exclusion_inversion)
    expect_failure(
        lambda: assert_representative_semantics(exclusion_inversion),
        "exclusions_nonclaims material content mismatch",
    )

    reopen_inversion = copy.deepcopy(capsules["WP-HK-GATE"])
    reopen_inversion["reopen_conditions"][0] = (
        "Never reopen HK-GATE even when concrete evidence contradicts its accepted claim."
    )
    checker.validate_capsule(ROOT, reopen_inversion)
    expect_failure(
        lambda: assert_representative_semantics(reopen_inversion),
        "reopen_conditions material content mismatch",
    )

    escalation_inversion = copy.deepcopy(capsules["WP-CITY-03"])
    escalation_inversion["escalate_if"][0] = (
        "Do not open the product seed when geometry, routes, sites or measurements are needed."
    )
    checker.validate_capsule(ROOT, escalation_inversion)
    expect_failure(
        lambda: assert_representative_semantics(escalation_inversion),
        "escalate_if material content mismatch",
    )

    directional_invention = copy.deepcopy(capsules["WP-PA-03"])
    trust = next(
        row for row in directional_invention["directional_semantics"] if row["id"] == "trust"
    )
    trust["forward"] = "trust(A -> C)"
    checker.validate_capsule(ROOT, directional_invention)
    expect_failure(
        lambda: assert_representative_semantics(directional_invention),
        "directional_semantics material content mismatch",
    )

    # Whole-source inventory omission can otherwise preserve valid fingerprints for
    # the remaining sources while silently narrowing Reviewer reconstruction.
    source_omission = copy.deepcopy(capsules["WP-HK-GATE"])
    source_omission["authoritative_sources"] = [
        row
        for row in source_omission["authoritative_sources"]
        if row["path"] != "Docs/evidence/WP-HK-GATE/PROOF_MATRIX.md"
    ]
    checker.validate_capsule(ROOT, source_omission)
    expect_failure(
        lambda: assert_representative_semantics(source_omission),
        "authoritative source inventory mismatch",
    )

    identity_self_confirmation = copy.deepcopy(capsules["WP-HK-GATE"])
    reaudit_path = ROOT / "Docs/evidence/CTX-02/TRUST_BOUNDARY_REAUDIT.md"
    identity_self_confirmation["identity_source"] = {
        "path": "Docs/evidence/CTX-02/TRUST_BOUNDARY_REAUDIT.md",
        "git_blob_sha": blob_sha(reaudit_path.read_bytes()),
    }
    expect_failure(
        lambda: checker.validate_capsule(ROOT, identity_self_confirmation),
        "checker-owned canonical workpack",
    )

    authority_self_confirmation = copy.deepcopy(capsules["WP-HK-GATE"])
    hk_capsule_path = ROOT / "Docs/engineering/context-capsules/WP-HK-GATE.json"
    authority_self_confirmation["authoritative_sources"] = [
        {
            "path": "Docs/engineering/context-capsules/WP-HK-GATE.json",
            "git_blob_sha": blob_sha(hk_capsule_path.read_bytes()),
            "kind": "self_authored_false_authority",
        }
    ]
    expect_failure(
        lambda: checker.validate_capsule(ROOT, authority_self_confirmation),
        "self-confirmation is forbidden",
    )

    wrong_city_read = copy.deepcopy(capsules["WP-CITY-03"])
    city_wp_path = ROOT / "Docs/workpacks/CITY/WP-CITY-03.md"
    wrong_city_read["mandatory_source_reads"] = [
        {
            "path": "Docs/workpacks/CITY/WP-CITY-03.md",
            "git_blob_sha": blob_sha(city_wp_path.read_bytes()),
            "noncompressible": True,
            "reason": "valid external bytes, but not the required construction specification",
        }
    ]
    expect_failure(
        lambda: checker.validate_capsule(ROOT, wrong_city_read),
        checker.CITY_PRODUCT_SEED,
    )

    # Rebinding the PA selector/source is now rejected by production validation
    # itself before an internally coherent alternate table can become its own oracle.
    pa_rebound = copy.deepcopy(capsules["WP-PA-03"])
    pa02 = capsules["WP-PA-02"]
    pa_rebound["disposition_source"] = copy.deepcopy(pa02["disposition_source"])
    pa_rebound["dispositions"] = copy.deepcopy(pa02["dispositions"])
    expect_failure(
        lambda: checker.validate_capsule(ROOT, pa_rebound),
        "checker-owned canonical PA result",
    )


def run_synthetic_regressions() -> None:
    """Exercise the production CLI against class-level defect injections."""
    with tempfile.TemporaryDirectory() as td:
        repo = Path(td)
        (repo / "Docs/workpacks/PA").mkdir(parents=True)
        (repo / "Docs/workpacks/CITY").mkdir(parents=True)
        (repo / "Docs/research/living-world/results").mkdir(parents=True)
        (repo / "Docs/engineering/context-capsules").mkdir(parents=True)
        (repo / "Docs/production").mkdir(parents=True)

        wp = repo / "Docs/workpacks/PA/WP-PA-03.md"
        result = repo / "Docs/research/living-world/results/PA-03.md"
        wp_text = (
            "# WP-PA-03\n\nStatus: **COMPLETE**\n"
            "Accepted candidate: `1111111111111111111111111111111111111111`\n"
            "Independent review: **PASS**, review `12345`\n"
            "Merged: PR `#1`, merge commit `2222222222222222222222222222222222222222`\n"
        )
        result.write_text(
            "# PA-03\n\n## 4. Minimal relationship vocabulary recommendation\n\n"
            "| Relationship/mechanism | Status | Juego2 recommendation |\n"
            "|---|---|---|\n"
            "| directed trust | **ADOPT** | material decision input |\n"
            "| default global/N-hop social traversal to discover targets | **REJECT** | inherited PA-02 bounded-discovery intent |\n"
            "\n## 4B. Alternate valid-looking table\n\n"
            "| Relationship/mechanism | Status | Juego2 recommendation |\n"
            "|---|---|---|\n"
            "| directed trust | **ADOPT** | alternate table |\n"
            "| default global/N-hop social traversal to discover targets | **REJECT** | alternate table |\n"
            "\n## 5. Next\n",
            encoding="utf-8",
        )
        wp.write_text(wp_text, encoding="utf-8")
        source = {
            "path": "Docs/research/living-world/results/PA-03.md",
            "git_blob_sha": blob_sha(result.read_bytes()),
        }
        capsule = {
            "schema": checker.SCHEMA,
            "authority": checker.AUTHORITY,
            "capsule_id": "WP-PA-03",
            "track": "PA",
            "content_mode": "structured_disposition",
            "accepted_identity": {
                "reviewed_candidate_sha": "1" * 40,
                "merge_sha": "2" * 40,
                "review_id": "12345",
            },
            "identity_source": {
                "path": "Docs/workpacks/PA/WP-PA-03.md",
                "git_blob_sha": blob_sha(wp.read_bytes()),
            },
            "authoritative_sources": [source],
            "exported_guarantees": [
                {"id": "directed-affect", "statement": "A->B is independent from B->A."},
                {"id": "behavioral-effect", "statement": "Relationship state can change action or target choice."},
            ],
            "exclusions_nonclaims": [
                {"id": "no-global-n-hop", "statement": "Default global/N-hop target discovery is rejected."},
                {"id": "no-implicit-symmetry", "statement": "Actor stance is not implicitly symmetric."},
            ],
            "reopen_conditions": ["concrete contradictory evidence"],
            "escalate_if": ["material exact source semantics are needed"],
            "disposition_source": {
                **source,
                **checker.CANONICAL_PA_DISPOSITION_SELECTORS["WP-PA-03"],
            },
            "dispositions": [
                {"source_key": "directed trust", "status": "ADOPT"},
                {"source_key": "default global/N-hop social traversal to discover targets", "status": "REJECT"},
            ],
            "directional_semantics": [
                {
                    "id": "trust",
                    "forward": "trust(A -> B)",
                    "reverse": "trust(B -> A)",
                    "must_remain_distinct": True,
                }
            ],
            "mandatory_source_reads": [],
        }

        city_wp_text = wp_text.replace("WP-PA-03", "WP-CITY-03")
        city_wp = repo / "Docs/workpacks/CITY/WP-CITY-03.md"
        city_wp.write_text(city_wp_text, encoding="utf-8")
        seed = repo / checker.CITY_PRODUCT_SEED
        seed.write_text("exact spatial spec", encoding="utf-8")
        city_capsule = {
            "schema": checker.SCHEMA,
            "authority": checker.AUTHORITY,
            "capsule_id": "WP-CITY-03",
            "track": "CITY",
            "content_mode": "boundary_summary",
            "accepted_identity": {
                "reviewed_candidate_sha": "1" * 40,
                "merge_sha": "2" * 40,
                "review_id": "12345",
            },
            "identity_source": {
                "path": "Docs/workpacks/CITY/WP-CITY-03.md",
                "git_blob_sha": blob_sha(city_wp.read_bytes()),
            },
            "authoritative_sources": [
                {
                    "path": "Docs/workpacks/CITY/WP-CITY-03.md",
                    "git_blob_sha": blob_sha(city_wp.read_bytes()),
                }
            ],
            "exported_guarantees": [{"id": "city", "statement": "bounded seed"}],
            "exclusions_nonclaims": [{"id": "not-seed", "statement": "capsule is not geometry"}],
            "reopen_conditions": ["measured contradiction"],
            "escalate_if": ["geometry needed"],
            "mandatory_source_reads": [
                {
                    "path": checker.CITY_PRODUCT_SEED,
                    "git_blob_sha": blob_sha(seed.read_bytes()),
                    "noncompressible": True,
                    "reason": "construction specification",
                }
            ],
        }

        capsule_path = repo / checker.canonical_capsule_path("WP-PA-03")
        city_path = repo / checker.canonical_capsule_path("WP-CITY-03")
        index_path = repo / checker.CANONICAL_INDEX_PATH
        index = {
            "schema": checker.INDEX_SCHEMA,
            "authority": checker.AUTHORITY,
            "protocol": checker.CANONICAL_PROTOCOL_PATH,
            "entries": [
                {"capsule_id": "WP-PA-03", "path": checker.canonical_capsule_path("WP-PA-03")},
                {"capsule_id": "WP-CITY-03", "path": checker.canonical_capsule_path("WP-CITY-03")},
            ],
            "coverage_rules": {
                "pa_accepted_result_chain": {
                    "workpack_glob": checker.PA_WORKPACK_GLOB,
                    "result_template": checker.PA_RESULT_TEMPLATE,
                }
            },
        }
        capsule_path.write_text(json.dumps(capsule), encoding="utf-8")
        city_path.write_text(json.dumps(city_capsule), encoding="utf-8")
        index_path.write_text(json.dumps(index), encoding="utf-8")

        baseline_cli = run_audit_cli(repo)
        if baseline_cli.returncode != 0:
            raise AssertionError(f"baseline --audit-index unexpectedly failed: {baseline_cli.stderr}")

        malformed_reopen = copy.deepcopy(capsule)
        malformed_reopen["reopen_conditions"] = [None]
        assert_audit_red(capsule_path, malformed_reopen, "reopen_conditions[0] must be a non-empty string")
        capsule_path.write_text(json.dumps(capsule), encoding="utf-8")

        blank_escalation = copy.deepcopy(capsule)
        blank_escalation["escalate_if"] = [""]
        assert_audit_red(capsule_path, blank_escalation, "escalate_if[0] must be a non-empty string")
        capsule_path.write_text(json.dumps(capsule), encoding="utf-8")

        fail_wp_text = wp_text.replace("**PASS**", "**FAIL**")
        wp.write_text(fail_wp_text, encoding="utf-8")
        fail_review = copy.deepcopy(capsule)
        fail_review["identity_source"]["git_blob_sha"] = blob_sha(wp.read_bytes())
        assert_audit_red(capsule_path, fail_review, "independent PASS review verdict")
        wp.write_text(wp_text, encoding="utf-8")
        capsule_path.write_text(json.dumps(capsule), encoding="utf-8")

        missing_surface = copy.deepcopy(capsule)
        missing_surface.pop("disposition_source", None)
        missing_surface.pop("dispositions", None)
        assert_audit_red(capsule_path, missing_surface, "accepted PA capsule requires disposition_source")
        capsule_path.write_text(json.dumps(capsule), encoding="utf-8")

        # Current Reviewer blocker: retain COMPLETE WP-PA-04, omit its result,
        # capsule and index entry, then try to hide it by changing workpack_glob.
        wp04 = repo / "Docs/workpacks/PA/WP-PA-04.md"
        wp04.write_text(wp_text.replace("WP-PA-03", "WP-PA-04"), encoding="utf-8")
        narrow = copy.deepcopy(index)
        narrow["coverage_rules"]["pa_accepted_result_chain"]["workpack_glob"] = (
            "Docs/workpacks/PA/WP-PA-0[1-3].md"
        )
        index_path.write_text(json.dumps(narrow), encoding="utf-8")
        assert_cli_red(repo, "checker-owned canonical selector")

        none_selector = copy.deepcopy(index)
        none_selector["coverage_rules"]["pa_accepted_result_chain"]["workpack_glob"] = (
            "Docs/workpacks/PA/NO-MATCH-*.md"
        )
        index_path.write_text(json.dumps(none_selector), encoding="utf-8")
        assert_cli_red(repo, "checker-owned canonical selector")

        broad = copy.deepcopy(index)
        broad["coverage_rules"]["pa_accepted_result_chain"]["workpack_glob"] = "Docs/workpacks/PA/*.md"
        index_path.write_text(json.dumps(broad), encoding="utf-8")
        assert_cli_red(repo, "checker-owned canonical selector")

        index_path.write_text(json.dumps(index), encoding="utf-8")
        assert_cli_red(repo, "accepted PA canonical result missing for COMPLETE workpack(s): WP-PA-04")
        wp04.unlink()

        redirected_template = copy.deepcopy(index)
        redirected_template["coverage_rules"]["pa_accepted_result_chain"]["result_template"] = (
            "Docs/research/living-world/alternate/PA-{NN}.md"
        )
        index_path.write_text(json.dumps(redirected_template), encoding="utf-8")
        assert_cli_red(repo, "checker-owned canonical template")
        index_path.write_text(json.dumps(index), encoding="utf-8")

        redirected_protocol = copy.deepcopy(index)
        redirected_protocol["protocol"] = "Docs/engineering/ALTERNATE_PROTOCOL.md"
        index_path.write_text(json.dumps(redirected_protocol), encoding="utf-8")
        assert_cli_red(repo, "checker-owned canonical path")
        index_path.write_text(json.dumps(index), encoding="utf-8")

        # Index entries cannot point at alternate valid-looking objects.
        alt_capsule_rel = "Docs/engineering/context-capsules/alternate-WP-PA-03.json"
        alt_capsule = repo / alt_capsule_rel
        alt_capsule.write_text(json.dumps(capsule), encoding="utf-8")
        redirected_entry = copy.deepcopy(index)
        redirected_entry["entries"][0]["path"] = alt_capsule_rel
        index_path.write_text(json.dumps(redirected_entry), encoding="utf-8")
        assert_cli_red(repo, "checker-owned canonical capsule path")
        index_path.write_text(json.dumps(index), encoding="utf-8")

        # A different index file cannot replace the audit oracle input.
        alt_index_rel = "Docs/engineering/context-capsules/alternate-index.json"
        (repo / alt_index_rel).write_text(json.dumps(index), encoding="utf-8")
        assert_cli_red(repo, "checker-owned canonical index", index=alt_index_rel)

        # External same-name workpack rebinding is rejected; basename matching is
        # insufficient because the canonical root is checker-owned.
        (repo / "Docs/workpacks/ALT").mkdir(parents=True)
        alt_wp = repo / "Docs/workpacks/ALT/WP-PA-03.md"
        alt_wp.write_text(wp_text, encoding="utf-8")
        rebound_identity = copy.deepcopy(capsule)
        rebound_identity["identity_source"] = {
            "path": "Docs/workpacks/ALT/WP-PA-03.md",
            "git_blob_sha": blob_sha(alt_wp.read_bytes()),
        }
        assert_audit_red(capsule_path, rebound_identity, "checker-owned canonical workpack")
        capsule_path.write_text(json.dumps(capsule), encoding="utf-8")

        # CITY-specific oracle cannot be hidden by mutating the subject's track and
        # deleting the mandatory read in the same defect.
        hidden_city = copy.deepcopy(city_capsule)
        hidden_city["track"] = "H1"
        hidden_city["mandatory_source_reads"] = []
        assert_audit_red(city_path, hidden_city, "checker-owned canonical track")
        city_path.write_text(json.dumps(city_capsule), encoding="utf-8")

        wrong_mode = copy.deepcopy(city_capsule)
        wrong_mode["content_mode"] = "structured_disposition"
        assert_audit_red(city_path, wrong_mode, "checker-owned canonical mode")
        city_path.write_text(json.dumps(city_capsule), encoding="utf-8")

        # PA table/column selector is also checker-owned. The source intentionally
        # contains a second valid-looking table with identical keys/statuses, so an
        # old self-selected selector would have passed source equality.
        alternate_table = copy.deepcopy(capsule)
        alternate_table["disposition_source"]["section"] = "## 4B. Alternate valid-looking table"
        assert_audit_red(capsule_path, alternate_table, "checker-owned canonical selector")
        capsule_path.write_text(json.dumps(capsule), encoding="utf-8")

        alternate_column = copy.deepcopy(capsule)
        alternate_column["disposition_source"]["status_column"] = 2
        assert_audit_red(capsule_path, alternate_column, "checker-owned canonical selector")
        capsule_path.write_text(json.dumps(capsule), encoding="utf-8")

        final_cli = run_audit_cli(repo)
        if final_cli.returncode != 0:
            raise AssertionError(f"restored baseline --audit-index unexpectedly failed: {final_cli.stderr}")


def run() -> None:
    validate_adoption_wiring(ROOT)
    run_representative_controls()
    run_synthetic_regressions()
    print("context-capsule independent controls: PASS")


if __name__ == "__main__":
    run()
