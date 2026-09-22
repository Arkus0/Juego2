#!/usr/bin/env python3
"""Independent defect-injection and adoption-wiring controls for Context Capsule v1.

The representative expectations in this file are deliberately test-only. They
are an independent oracle for the actual H1/CITY/PA boundaries selected by
WP-CTX-02; they are not imported by the production checker and are not a
production semantic registry. Generic natural-language equivalence remains a
Reviewer/escalation responsibility.
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


# Test-only oracle for the actual representative boundaries selected by CTX-02.
# Exact prose belongs here precisely so production validation does not acquire a
# semantic registry. A change to these expectations is visible review evidence.
REPRESENTATIVE_ORACLE = {
    "WP-HK-GATE": {
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


def assert_representative_semantics(capsule: dict) -> None:
    cid = capsule.get("capsule_id")
    expected = REPRESENTATIVE_ORACLE.get(cid)
    if expected is None:
        raise checker.CapsuleError(f"independent representative oracle has no fixture for {cid}")

    actual_exports = statement_map(capsule, "exported_guarantees")
    if actual_exports != expected["exports"]:
        raise checker.CapsuleError(
            f"independent representative oracle: {cid} exported_guarantees material content mismatch"
        )
    actual_exclusions = statement_map(capsule, "exclusions_nonclaims")
    if actual_exclusions != expected["exclusions"]:
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
    index = checker.load_json(repo_root / "Docs/engineering/context-capsules/index.json")
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


def validate_adoption_wiring(repo_root: Path) -> None:
    profile_path = repo_root / "Docs/engineering/context-bootstrap-profiles.json"
    try:
        profiles = json.loads(profile_path.read_text(encoding="utf-8"))
    except (FileNotFoundError, json.JSONDecodeError) as exc:
        raise checker.CapsuleError("adoption wiring cannot read context-bootstrap-profiles.json") from exc
    if profiles.get("capsule_protocol") != "Docs/engineering/CONTEXT_CAPSULE_V1.md":
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
        ["Docs/engineering/CONTEXT_CAPSULE_V1.md", "WP-CTX-02"],
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


def run_audit_cli(repo: Path) -> subprocess.CompletedProcess[str]:
    return subprocess.run(
        [
            sys.executable,
            str(CHECKER_PATH),
            "--audit-index",
            "--repo-root",
            str(repo),
            "--index",
            "Docs/engineering/context-capsules/index.json",
        ],
        check=False,
        capture_output=True,
        text=True,
    )


def assert_audit_red(capsule_path: Path, mutated_capsule: dict, needle: str) -> None:
    capsule_path.write_text(json.dumps(mutated_capsule), encoding="utf-8")
    result = run_audit_cli(capsule_path.parents[3])
    if result.returncode == 0:
        raise AssertionError(f"expected --audit-index to fail for {needle}")
    if needle not in result.stderr:
        raise AssertionError(f"unexpected --audit-index failure for {needle!r}: {result.stderr}")


def run_representative_controls() -> None:
    index, capsules = load_indexed_capsules(ROOT)
    required = set(REPRESENTATIVE_ORACLE)
    missing = sorted(required - set(capsules))
    if missing:
        raise checker.CapsuleError(
            f"independent representative oracle: indexed representative capsule(s) missing: {missing}"
        )

    # Positive controls use the actual indexed capsules, not a synthetic inventory.
    for cid in sorted(required):
        capsule = capsules[cid]
        checker.validate_capsule(ROOT, capsule)
        assert_representative_semantics(capsule)

    # Cycle-3 causal blocker: preserve ID + source pointer + all bound fingerprints,
    # change only the material statement. Production validation intentionally does
    # not pretend to prove prose equivalence; the external representative oracle REDs.
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

    # Symmetric negative/non-claim control: same IDs/pointers/fingerprints, invented
    # opposite statement must be caught by the independent representative oracle.
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

    # Same substitution class exists inside CTX-02's semantic claim for reopen,
    # escalation and directional values. Non-empty/internally-distinct inventions
    # remain structurally valid but must RED against representative source review.
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

    # Self-confirmation closure: fingerprints only matter after authority paths are
    # independently constrained away from the capsule/CTX-02 generated layer.
    identity_self_confirmation = copy.deepcopy(capsules["WP-HK-GATE"])
    reaudit_path = ROOT / "Docs/evidence/CTX-02/TRUST_BOUNDARY_REAUDIT.md"
    identity_self_confirmation["identity_source"] = {
        "path": "Docs/evidence/CTX-02/TRUST_BOUNDARY_REAUDIT.md",
        "git_blob_sha": blob_sha(reaudit_path.read_bytes()),
    }
    expect_failure(
        lambda: checker.validate_capsule(ROOT, identity_self_confirmation),
        "canonical external workpack",
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

    # CITY's representative non-compressible contract names one exact source. A
    # different valid/fingerprinted external file cannot satisfy that obligation.
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

    # PA disposition equality is source-derived only if the source itself is the
    # independently discovered canonical result. Rebind PA-03 to a complete PA-02
    # disposition surface: the individual table can be internally coherent, while
    # chain validation must still RED on canonical-source identity.
    pa_rebound = copy.deepcopy(capsules["WP-PA-03"])
    pa02 = capsules["WP-PA-02"]
    pa_rebound["disposition_source"] = copy.deepcopy(pa02["disposition_source"])
    pa_rebound["dispositions"] = copy.deepcopy(pa02["dispositions"])
    checker.validate_capsule(ROOT, pa_rebound)
    rebound_capsules = dict(capsules)
    rebound_capsules["WP-PA-03"] = pa_rebound
    expect_failure(
        lambda: checker.validate_pa_chain(ROOT, index, rebound_capsules),
        "disposition_source must be canonical accepted PA result",
    )


def run_synthetic_regressions() -> None:
    """Keep the prior two Reviewer FAIL families and one-of-many omission controls."""
    with tempfile.TemporaryDirectory() as td:
        repo = Path(td)
        (repo / "Docs/workpacks/PA").mkdir(parents=True)
        (repo / "Docs/research/living-world/results").mkdir(parents=True)
        (repo / "Docs/engineering/context-capsules").mkdir(parents=True)

        wp = repo / "Docs/workpacks/PA/WP-PA-03.md"
        result = repo / "Docs/research/living-world/results/PA-03.md"
        wp_text = (
            "# WP-PA-03\n\nStatus: **COMPLETE**\n"
            "Accepted candidate: `1111111111111111111111111111111111111111`\n"
            "Independent review: **PASS**, review `12345`\n"
            "Merged: PR `#1`, merge commit `2222222222222222222222222222222222222222`\n"
        )
        wp.write_text(wp_text, encoding="utf-8")
        result.write_text(
            "# PA-03\n\n## 4. Minimal relationship vocabulary recommendation\n\n"
            "| Relationship/mechanism | Status | Juego2 recommendation |\n"
            "|---|---|---|\n"
            "| directed trust | **ADOPT** | material decision input |\n"
            "| default global/N-hop social traversal to discover targets | **REJECT** | inherited PA-02 bounded-discovery intent |\n"
            "\n## 5. Next\n",
            encoding="utf-8",
        )
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
                {
                    "id": "behavioral-effect",
                    "statement": "Relationship state can change action or target choice.",
                },
            ],
            "exclusions_nonclaims": [
                {
                    "id": "no-global-n-hop",
                    "statement": "Default global/N-hop target discovery is rejected.",
                },
                {"id": "no-implicit-symmetry", "statement": "Actor stance is not implicitly symmetric."},
            ],
            "reopen_conditions": ["concrete contradictory evidence"],
            "escalate_if": ["material exact source semantics are needed"],
            "disposition_source": {
                **source,
                "section": "## 4. Minimal relationship vocabulary recommendation",
                "key_column": 0,
                "status_column": 1,
            },
            "dispositions": [
                {"source_key": "directed trust", "status": "ADOPT"},
                {
                    "source_key": "default global/N-hop social traversal to discover targets",
                    "status": "REJECT",
                },
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

        checker.validate_capsule(repo, capsule)

        expected_exports = {"directed-affect", "behavioral-effect"}
        actual_exports = {row["id"] for row in capsule["exported_guarantees"]}
        if actual_exports != expected_exports:
            raise AssertionError("synthetic baseline export oracle malformed")
        one_positive_missing = copy.deepcopy(capsule)
        one_positive_missing["exported_guarantees"] = [one_positive_missing["exported_guarantees"][0]]
        checker.validate_capsule(repo, one_positive_missing)
        if {row["id"] for row in one_positive_missing["exported_guarantees"]} == expected_exports:
            raise AssertionError("synthetic one-positive omission did not remove a material id")

        expected_exclusions = {"no-global-n-hop", "no-implicit-symmetry"}
        one_exclusion_missing = copy.deepcopy(capsule)
        one_exclusion_missing["exclusions_nonclaims"] = [one_exclusion_missing["exclusions_nonclaims"][1]]
        checker.validate_capsule(repo, one_exclusion_missing)
        if {row["id"] for row in one_exclusion_missing["exclusions_nonclaims"]} == expected_exclusions:
            raise AssertionError("synthetic one-exclusion omission did not remove a material id")

        capsule_path = repo / "Docs/engineering/context-capsules/WP-PA-03.json"
        index_path = repo / "Docs/engineering/context-capsules/index.json"
        index = {
            "schema": checker.INDEX_SCHEMA,
            "authority": checker.AUTHORITY,
            "entries": [
                {
                    "capsule_id": "WP-PA-03",
                    "path": "Docs/engineering/context-capsules/WP-PA-03.json",
                }
            ],
            "coverage_rules": {
                "pa_accepted_result_chain": {
                    "result_glob": "Docs/research/living-world/results/PA-*.md",
                    "workpack_template": "Docs/workpacks/PA/WP-PA-{NN}.md",
                }
            },
        }
        capsule_path.write_text(json.dumps(capsule), encoding="utf-8")
        index_path.write_text(json.dumps(index), encoding="utf-8")
        baseline_cli = run_audit_cli(repo)
        if baseline_cli.returncode != 0:
            raise AssertionError(f"baseline --audit-index unexpectedly failed: {baseline_cli.stderr}")

        malformed_reopen = copy.deepcopy(capsule)
        malformed_reopen["reopen_conditions"] = [None]
        assert_audit_red(
            capsule_path,
            malformed_reopen,
            "reopen_conditions[0] must be a non-empty string",
        )

        blank_reopen = copy.deepcopy(capsule)
        blank_reopen["reopen_conditions"] = ["   "]
        assert_audit_red(
            capsule_path,
            blank_reopen,
            "reopen_conditions[0] must be a non-empty string",
        )

        malformed_escalation = copy.deepcopy(capsule)
        malformed_escalation["escalate_if"] = [None]
        assert_audit_red(
            capsule_path,
            malformed_escalation,
            "escalate_if[0] must be a non-empty string",
        )

        blank_escalation = copy.deepcopy(capsule)
        blank_escalation["escalate_if"] = [""]
        assert_audit_red(
            capsule_path,
            blank_escalation,
            "escalate_if[0] must be a non-empty string",
        )

        fail_wp_text = wp_text.replace("**PASS**", "**FAIL**")
        wp.write_text(fail_wp_text, encoding="utf-8")
        fail_review = copy.deepcopy(capsule)
        fail_review["identity_source"]["git_blob_sha"] = blob_sha(wp.read_bytes())
        assert_audit_red(capsule_path, fail_review, "independent PASS review verdict")
        wp.write_text(wp_text, encoding="utf-8")

        missing_surface = copy.deepcopy(capsule)
        missing_surface.pop("disposition_source", None)
        missing_surface.pop("dispositions", None)
        assert_audit_red(
            capsule_path,
            missing_surface,
            "accepted PA chain capsule requires disposition_source",
        )


def run() -> None:
    validate_adoption_wiring(ROOT)
    run_representative_controls()
    run_synthetic_regressions()
    print("context-capsule independent controls: PASS")


if __name__ == "__main__":
    run()
