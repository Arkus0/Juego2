#!/usr/bin/env python3
"""Deterministic CTX↔DW gate falsification harness.

Proof infrastructure only: semantic authority remains in declared authoritative
sources. This is not product/runtime routing code.
"""
from __future__ import annotations

import argparse
import copy
import hashlib
import json
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[1]
DEFAULT_SUITE = ROOT / "Docs" / "evidence" / "WP-CTX-DW-GATE" / "FALSIFICATION_SUITE.json"
LIFECYCLE_KEYS = (
    "independent_universe",
    "projection_owner",
    "completeness_green",
    "provenance_green",
    "staleness_green",
    "rebuild_green",
)


def fail(message: str) -> None:
    raise SystemExit(f"CTX_DW_GATE_RED {message}")


def load_json(path: Path) -> Any:
    try:
        return json.loads(path.read_text(encoding="utf-8"))
    except Exception as exc:
        fail(f"invalid-json {path}: {exc}")


def unique(values):
    return list(dict.fromkeys(values))


def lifecycle_green(dw: dict[str, Any]) -> bool:
    lifecycle = dw.get("lifecycle", {})
    return all(lifecycle.get(key) is True for key in LIFECYCLE_KEYS)


def authoritative_blockers(case: dict[str, Any], opened_sources: list[str]) -> list[str]:
    records = case.get("authority_records", {})
    found: list[str] = []
    for source in opened_sources:
        found.extend(records.get(source, {}).get("blockers", []))
    return sorted(set(found))


def route(case: dict[str, Any]) -> dict[str, Any]:
    ctx = case["ctx"]
    dw = case["dw"]
    claim = case.get("claim", {})
    opened = unique(ctx.get("mandatory_sources", []) + ctx.get("claim_required_sources", []))
    dw_loaded = False
    dw_admitted = False
    dw_status = dw["advice"]

    # Negative/completeness claims fail closed toward the authoritative universe.
    if claim.get("negative") is True and claim.get("completeness_oracle_green") is not True:
        opened = unique(opened + claim.get("authoritative_universe_sources", []))

    # Compact surfaces never vote against one another: contradiction source-opens.
    if case.get("ctx_dw_contradiction") is True:
        opened = unique(opened + case.get("contradiction_authority_sources", []))
        dw_status = "CONTRADICTION_SOURCE_OPEN"
    elif dw["advice"] == "USE":
        if lifecycle_green(dw):
            dw_admitted = True
            dw_loaded = True
            dw_status = "USE"
            source = dw.get("decision_critical_provenance_source")
            if source:
                opened = unique(opened + [source])
        else:
            dw_status = "USE_REJECTED_LIFECYCLE"
            opened = unique(opened + dw.get("authority_fallback_sources", []))
    elif dw["advice"] == "OPTIONAL":
        dw_status = "OPTIONAL"
    elif dw["advice"] == "NOT_MATERIAL":
        dw_status = "NOT_MATERIAL"
    else:
        fail(f"{case['id']} invalid-dw-advice {dw['advice']}")

    # Reviewer expansion is always additive; Worker routing can never forbid it.
    opened = unique(opened + case.get("reviewer_expansion_sources", []))
    blockers = authoritative_blockers(case, opened)
    units = case.get("initial_context_units", {})
    selective = int(units.get("selective", len(opened)))
    baseline = int(units.get("baseline", selective))
    return {
        "dw_status": dw_status,
        "dw_loaded": dw_loaded,
        "dw_admitted": dw_admitted,
        "opened_sources": sorted(opened),
        "discovered_blockers": blockers,
        "selective_initial_units": selective,
        "baseline_initial_units": baseline,
        "context_reduced": selective < baseline,
    }


def signature(result: dict[str, Any]) -> dict[str, Any]:
    return {key: result[key] for key in (
        "dw_status", "dw_loaded", "dw_admitted", "opened_sources", "discovered_blockers"
    )}


def assert_expected(case: dict[str, Any], result: dict[str, Any]) -> None:
    expected = case["expected"]
    for key in ("dw_status", "dw_loaded", "dw_admitted"):
        if result[key] != expected[key]:
            fail(f"{case['id']} {key} expected={expected[key]!r} observed={result[key]!r}")
    if sorted(expected.get("opened_sources", [])) != result["opened_sources"]:
        fail(f"{case['id']} opened-sources expected={sorted(expected.get('opened_sources', []))} observed={result['opened_sources']}")
    if sorted(expected.get("discovered_blockers", [])) != result["discovered_blockers"]:
        fail(f"{case['id']} blockers expected={sorted(expected.get('discovered_blockers', []))} observed={result['discovered_blockers']}")


def validate_repo_preconditions(root: Path) -> None:
    checks = (
        (root / "Docs/workpacks/CTX/WP-CTX-03.md", "Status:", "COMPLETE"),
        (root / "Docs/workpacks/DW/WP-DW-GATE.md", "Status:", "COMPLETE"),
        (root / "Docs/evidence/CTX-03/DOCSYNC.md", "DOCSYNC_STATUS:", "DOCSYNC_COMPLETE"),
        (root / "Docs/evidence/WP-DW-GATE/DOCSYNC.md", "DOCSYNC_STATUS:", "DOCSYNC_COMPLETE"),
    )
    for path, marker, accepted in checks:
        if not path.is_file():
            fail(f"missing-predecessor {path.relative_to(root)}")
        text = path.read_text(encoding="utf-8")
        if marker not in text or accepted not in text:
            fail(f"predecessor-not-accepted {path.relative_to(root)}")


def validate_authority_paths(root: Path, suite: dict[str, Any]) -> None:
    declared = set()
    for case in suite["cases"]:
        declared.update(case.get("authority_records", {}).keys())
        declared.update(case.get("ctx", {}).get("mandatory_sources", []))
        declared.update(case.get("ctx", {}).get("claim_required_sources", []))
        declared.update(case.get("claim", {}).get("authoritative_universe_sources", []))
        declared.update(case.get("contradiction_authority_sources", []))
        declared.update(case.get("dw", {}).get("authority_fallback_sources", []))
        source = case.get("dw", {}).get("decision_critical_provenance_source")
        if source:
            declared.add(source)
        declared.update(case.get("reviewer_expansion_sources", []))
    for source in sorted(declared):
        if not (root / source).is_file():
            fail(f"declared-authority-missing {source}")


def validate_suite(suite: dict[str, Any]) -> None:
    if suite.get("schema") != "ctx-dw-gate-falsification-v1":
        fail("wrong-suite-schema")
    cases = suite.get("cases")
    if not isinstance(cases, list) or len(cases) != 10:
        fail(f"expected-exactly-10-cases observed={len(cases) if isinstance(cases, list) else 'invalid'}")
    ids = [case.get("id") for case in cases]
    if len(ids) != len(set(ids)) or any(not isinstance(x, str) or not x for x in ids):
        fail("invalid-or-duplicate-case-id")
    required = {
        "hidden-materiality", "ctx-dw-contradiction", "stale-dw", "dw-abstention",
        "material-dw", "negative-claim", "domain-leakage-differential",
        "false-positive", "h1-projection-bootstrap", "routing-authority-conflict",
    }
    kinds = {case.get("kind") for case in cases}
    if kinds != required:
        fail(f"causal-class-mismatch missing={sorted(required-kinds)} extra={sorted(kinds-required)}")


def run_suite(root: Path, suite_path: Path) -> dict[str, Any]:
    suite = load_json(suite_path)
    validate_suite(suite)
    validate_repo_preconditions(root)
    validate_authority_paths(root, suite)
    results = []
    material_utility = 0
    abstentions = 0

    for case in suite["cases"]:
        result = route(case)
        assert_expected(case, result)
        baseline = sorted(case.get("baseline_material_blockers", []))
        if result["discovered_blockers"] != baseline:
            fail(f"{case['id']} discoverability-regression baseline={baseline} observed={result['discovered_blockers']}")

        if case["kind"] == "dw-abstention":
            if result["dw_loaded"] or result["dw_status"] != "NOT_MATERIAL":
                fail(f"{case['id']} abstention-failed")
            abstentions += 1

        if case["kind"] == "material-dw":
            variants = case.get("material_variants", [])
            if len(variants) < 2:
                fail(f"{case['id']} requires-two-material-variants")
            for variant in variants:
                probe = copy.deepcopy(case)
                probe["id"] = f"{case['id']}::{variant['id']}"
                probe["dw"]["decision_critical_provenance_source"] = variant["decision_critical_provenance_source"]
                probe["expected"] = variant["expected"]
                probe["baseline_material_blockers"] = variant.get("baseline_material_blockers", [])
                probe["initial_context_units"] = variant["initial_context_units"]
                observed = route(probe)
                assert_expected(probe, observed)
                if observed["discovered_blockers"] != sorted(probe["baseline_material_blockers"]):
                    fail(f"{probe['id']} discoverability-regression")
                if not observed["context_reduced"]:
                    fail(f"{probe['id']} no-selective-context-reduction")
                material_utility += 1

        if case["kind"] == "domain-leakage-differential":
            renamed = copy.deepcopy(case)
            renamed["diagnostic_text"] = case.get("alpha_renamed_diagnostic_text", "")
            if signature(result) != signature(route(renamed)):
                fail(f"{case['id']} vocabulary-sensitive-routing")

        if case["kind"] == "false-positive":
            if result["dw_loaded"] or result["dw_status"] != "NOT_MATERIAL":
                fail(f"{case['id']} opaque-text-manufactured-materiality")

        if case["kind"] == "h1-projection-bootstrap":
            if result["dw_admitted"] or result["dw_status"] != "USE_REJECTED_LIFECYCLE":
                fail(f"{case['id']} premature-h1-use")
            after = copy.deepcopy(case)
            after["dw"]["lifecycle"] = {key: True for key in LIFECYCLE_KEYS}
            after["expected"] = case["after_rebuild_expected"]
            after_result = route(after)
            assert_expected(after, after_result)
            if not after_result["dw_admitted"]:
                fail(f"{case['id']} valid-h1-projection-not-eligible")
            if case.get("product_oracle_before") != case.get("product_oracle_after"):
                fail(f"{case['id']} h1-product-oracle-mutated")

        results.append({"id": case["id"], "kind": case["kind"], **result})

    if material_utility < 2:
        fail(f"selective-utility-insufficient variants={material_utility}")
    if abstentions < 1:
        fail("missing-abstention-control")

    normalized = json.dumps(results, sort_keys=True, separators=(",", ":")).encode("utf-8")
    digest = hashlib.sha256(normalized).hexdigest()
    replay = hashlib.sha256(json.dumps(results, sort_keys=True, separators=(",", ":")).encode("utf-8")).hexdigest()
    if digest != replay:
        fail("deterministic-replay-mismatch")
    return {
        "schema": "ctx-dw-gate-result-v1",
        "case_count": len(results),
        "material_utility_variants": material_utility,
        "abstention_controls": abstentions,
        "discoverability_regressions": 0,
        "result_digest": digest,
        "results": results,
    }


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--suite", type=Path, default=DEFAULT_SUITE)
    parser.add_argument("--json", action="store_true")
    args = parser.parse_args()
    result = run_suite(ROOT, args.suite)
    if args.json:
        print(json.dumps(result, indent=2, sort_keys=True))
    else:
        print(
            "CTX_DW_GATE_GREEN "
            f"cases={result['case_count']} "
            f"material_variants={result['material_utility_variants']} "
            f"abstentions={result['abstention_controls']} "
            f"discoverability_regressions={result['discoverability_regressions']} "
            f"digest={result['result_digest']}"
        )


if __name__ == "__main__":
    main()
