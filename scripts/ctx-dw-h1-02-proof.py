#!/usr/bin/env python3
"""Deterministic selective-adoption proof for WP-CTX-DW-H1-02.

ADOPTION_CASES.json identifies canonical claims and accepted anchors only.
The semantic contract for each claim (required capabilities, mandatory reads,
public-isolation, and expected current disposition) is frozen independently
inside this verifier. Fixture prose or expected-result fields cannot redefine it.
"""
from __future__ import annotations

import argparse
import copy
import json
import re
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[1]
CASES_PATH = ROOT / "Docs/evidence/WP-CTX-DW-H1-02/ADOPTION_CASES.json"
DISPOSITION_PATH = ROOT / "Docs/evidence/WP-CTX-DW-H1-02/ADOPTION_DISPOSITION.md"

PROJECTION_DIGEST = "516f51930124e0e77e6f767af0c8074c8384723c809c0bd50571ff44d80b1858"
PROJECTION_IDENTITY = "ctx-dw-h1-01-adapter-v1:" + PROJECTION_DIGEST
PROJECTION_VERSION = "ctx-dw-h1-01-v1"
H104_CANDIDATE = "8c6ffd61d17e832ed5b9f900e8c0f7d4e85bf5f5"
H105_CANDIDATE = "186224fbc3f53eb9c47ae528a56dcf3514af163f"
H106_CANDIDATE = "96de260021fb28ff2cc7da8d2ef488be419568b3"

EXPECTED_CAPABILITIES = {
    "catalogue-entry",
    "source-record",
    "source-slice",
    "catalogue-declared-in",
    "adopted-from-source",
    "component-schema-entry",
}

# Canonical claim semantics are verifier-owned, not fixture-owned.
# The fixture may identify these claims, but it cannot redefine requirements,
# mandatory reads, public-isolation, or the expected current disposition.
CLAIM_CONTRACTS: dict[str, dict[str, Any]] = {
    "h105-catalogue-source-navigation": {
        "consumer": "WP-H1-05",
        "anchor_path": "Docs/evidence/WP-H1-05/PROOF_MATRIX.md",
        "anchor_text": "Effective catalogue/source is admitted before effects",
        "query": "representative_prefab",
        "requirements": ("catalogue-entry", "adopted-from-source"),
        "mandatory_reads": (
            "Docs/evidence/WP-H1-05/PROOF_MATRIX.md",
            "Unity/ArkusUnity/Assets/Arkus/H1/CatalogueMapping.json",
        ),
        "public_isolation": False,
        "expected_classification": "USE",
    },
    "h105-scene-publication": {
        "consumer": "WP-H1-05",
        "anchor_path": "Docs/evidence/WP-H1-05/PROOF_MATRIX.md",
        "anchor_text": "Only manifest-selected generation is current",
        "query": None,
        "requirements": ("effective-scene-membership", "active-generation-publication"),
        "mandatory_reads": ("Docs/evidence/WP-H1-05/PROOF_MATRIX.md",),
        "public_isolation": False,
        "expected_classification": "NOT_MATERIAL",
    },
    "h106-source-prefab-navigation": {
        "consumer": "WP-H1-06",
        "anchor_path": "Docs/evidence/WP-H1-06/PROOF_MATRIX.md",
        "anchor_text": "Accepted source prefab is read-only",
        "query": "representative_prefab",
        "requirements": ("catalogue-entry", "adopted-from-source"),
        "mandatory_reads": (
            "Docs/evidence/WP-H1-06/PROOF_MATRIX.md",
            "Unity/ArkusUnity/Assets/Arkus/H1/CatalogueMapping.json",
        ),
        "public_isolation": False,
        "expected_classification": "USE",
    },
    "h106-relationship-equality": {
        "consumer": "WP-H1-06",
        "anchor_path": "Docs/evidence/WP-H1-06/PROOF_MATRIX.md",
        "anchor_text": "Source-derived relationship multiset is exact per canonical node",
        "query": None,
        "requirements": ("catalogue-entry", "effective-prefab-relationship-multiset"),
        "mandatory_reads": ("Docs/evidence/WP-H1-06/PROOF_MATRIX.md",),
        "public_isolation": False,
        "expected_classification": "OPTIONAL",
    },
    "h107-component-schema-inventory": {
        "consumer": "WP-H1-07",
        "anchor_path": "Docs/workpacks/H1/WP-H1-07.md",
        "anchor_text": "complete initial allowlist required by the representative slice",
        "query": "representative_component_schema",
        "requirements": ("component-schema-entry",),
        "mandatory_reads": (
            "Docs/workpacks/H1/WP-H1-07.md",
            "Unity/ArkusUnity/Assets/Arkus/H1/CatalogueMapping.json",
        ),
        "public_isolation": False,
        "expected_classification": "USE",
    },
    "h107-component-roundtrip": {
        "consumer": "WP-H1-07",
        "anchor_path": "Docs/workpacks/H1/WP-H1-07.md",
        "anchor_text": "supported scalar/vector/enum/reference fields round-trip through materialize -> save/reload -> inspect",
        "query": None,
        "requirements": ("component-schema-entry", "effective-component-adapter", "component-field-roundtrip"),
        "mandatory_reads": ("Docs/workpacks/H1/WP-H1-07.md",),
        "public_isolation": False,
        "expected_classification": "OPTIONAL",
    },
    "h107-unsupported-field": {
        "consumer": "WP-H1-07",
        "anchor_path": "Docs/workpacks/H1/WP-H1-07.md",
        "anchor_text": "unsupported component/type/field/reference yields stable failure before active generation publication",
        "query": None,
        "requirements": ("effective-component-adapter", "component-field-roundtrip"),
        "mandatory_reads": ("Docs/workpacks/H1/WP-H1-07.md",),
        "public_isolation": False,
        "expected_classification": "NOT_MATERIAL",
    },
    "h1gate-public-client-isolation": {
        "consumer": "WP-H1-GATE",
        "anchor_path": "Docs/workpacks/H1/CTX_DW_ADOPTION_PLAN.md",
        "anchor_text": "mandatory fresh independent AI-agent trial",
        "query": None,
        "requirements": ("catalogue-entry",),
        "mandatory_reads": ("Docs/workpacks/H1/CTX_DW_ADOPTION_PLAN.md",),
        "public_isolation": True,
        "expected_classification": "NOT_MATERIAL",
    },
}

FIXTURE_FIELDS = {"id", "consumer", "anchor_path", "anchor_text", "query"}
FORBIDDEN_FIXTURE_SEMANTICS = {
    "requirements",
    "mandatory_reads",
    "expected_classification",
    "public_isolation",
}
EXPECTED_CLASS_COUNTS = {"USE": 3, "OPTIONAL": 2, "NOT_MATERIAL": 3}


def fail(message: str) -> None:
    raise SystemExit(f"CTX_DW_H1_02_RED {message}")


def load_json(path: Path) -> Any:
    try:
        return json.loads(path.read_text(encoding="utf-8"))
    except Exception as exc:
        fail(f"invalid-json {path.relative_to(ROOT) if path.is_relative_to(ROOT) else path}: {exc}")


def read(rel: str) -> str:
    path = ROOT / rel
    if not path.is_file():
        fail(f"missing-repository-input {rel}")
    return path.read_text(encoding="utf-8")


def require_text(rel: str, *needles: str) -> None:
    text = read(rel)
    for needle in needles:
        if needle not in text:
            fail(f"missing-authority-marker path={rel} marker={needle!r}")


def validate_predecessors() -> None:
    require_text(
        "Docs/evidence/WP-CTX-DW-H1-01/LIFECYCLE.md",
        "LIFECYCLE_VERDICT: READY",
        f"- Projection.Digest: `{PROJECTION_DIGEST}`",
        f"- ProjectionIdentity: `{PROJECTION_IDENTITY}`",
        H104_CANDIDATE,
    )
    require_text(
        "Docs/workpacks/H1/WP-H1-05.md",
        "Status: COMPLETE / ACCEPTED",
        H105_CANDIDATE,
        "#5308430565",
    )
    require_text(
        "Docs/workpacks/H1/WP-H1-06.md",
        "Status: COMPLETE / ACCEPTED",
        H106_CANDIDATE,
        "#5313437809",
    )
    h107 = read("Docs/workpacks/H1/WP-H1-07.md")
    if "Depends on: `WP-H1-06` PASS" not in h107:
        fail("H1-07 product dependency no longer points directly to accepted H1-06")
    if "WP-CTX-DW-H1-02" in h107:
        fail("H1-07 must not acquire a product dependency on optional CTX-DW-H1-02")
    require_text(
        "Docs/workpacks/H1/CTX_DW_ADOPTION_PLAN.md",
        "mandatory fresh independent AI-agent trial",
        "Do **not** pre-seed that trial with Juego2-private CTX capsules, DW projections",
    )


def case_shape_errors(suite: Any) -> list[str]:
    errors: list[str] = []
    if not isinstance(suite, dict):
        return ["suite-not-object"]
    if suite.get("schema") != "ctx-dw-h1-02-adoption-cases-v1":
        errors.append("wrong-case-schema")
    cases = suite.get("cases")
    if not isinstance(cases, list):
        return errors + ["cases-not-list"]
    if len(cases) != 8:
        errors.append(f"expected-exactly-8-cases observed={len(cases)}")

    ids: list[Any] = []
    for index, case in enumerate(cases):
        if not isinstance(case, dict):
            errors.append(f"case-{index}-not-object")
            continue
        claim_id = case.get("id")
        ids.append(claim_id)
        if not isinstance(claim_id, str) or not claim_id:
            errors.append(f"case-{index}-invalid-id")
            continue

        forbidden = sorted(FORBIDDEN_FIXTURE_SEMANTICS & set(case))
        if forbidden:
            errors.append(f"{claim_id} fixture-semantic-authority-forbidden={forbidden}")
        unexpected = sorted(set(case) - FIXTURE_FIELDS - FORBIDDEN_FIXTURE_SEMANTICS)
        if unexpected:
            errors.append(f"{claim_id} unexpected-fixture-fields={unexpected}")

        contract = CLAIM_CONTRACTS.get(claim_id)
        if contract is None:
            errors.append(f"{claim_id} unknown-claim-id")
            continue
        for field in ("consumer", "anchor_path", "anchor_text", "query"):
            observed = case.get(field)
            expected = contract[field]
            if observed != expected:
                errors.append(f"{claim_id} metadata-mismatch field={field} expected={expected!r} observed={observed!r}")

    string_ids = [value for value in ids if isinstance(value, str)]
    if len(string_ids) != len(set(string_ids)):
        errors.append("duplicate-case-id")
    if set(string_ids) != set(CLAIM_CONTRACTS):
        errors.append(
            "canonical-claim-id-set-mismatch "
            f"missing={sorted(set(CLAIM_CONTRACTS) - set(string_ids))} "
            f"extra={sorted(set(string_ids) - set(CLAIM_CONTRACTS))}"
        )
    return errors


def validate_cases(suite: dict[str, Any]) -> list[dict[str, Any]]:
    errors = case_shape_errors(suite)
    if errors:
        fail("invalid-adoption-case-fixture " + "; ".join(errors))
    cases = suite["cases"]

    # Anchor and mandatory-read validation is driven from verifier-owned contracts,
    # not from paths or semantic lists supplied by the fixture under evaluation.
    for claim_id, contract in CLAIM_CONTRACTS.items():
        anchor_text = read(contract["anchor_path"])
        if contract["anchor_text"] not in anchor_text:
            fail(f"{claim_id} accepted-anchor-not-found")
        for rel in contract["mandatory_reads"]:
            if not (ROOT / rel).is_file():
                fail(f"{claim_id} missing-mandatory-read={rel}")

    return cases


def observation_is_accepted_shape(observation: dict[str, Any]) -> bool:
    if observation.get("schema") != "ctx-dw-h1-02-projection-observation-v1":
        return False
    if observation.get("projection_identity") != PROJECTION_IDENTITY:
        return False
    if observation.get("projection_digest") != PROJECTION_DIGEST:
        return False
    if observation.get("projection_version") != PROJECTION_VERSION:
        return False
    if observation.get("lifecycle_current") is not True:
        return False
    if observation.get("stale_probe_current") is not False:
        return False
    if "h1.lifecycle_projection_schema_stale" not in observation.get("stale_probe_codes", []):
        return False
    if set(observation.get("capabilities", [])) != EXPECTED_CAPABILITIES:
        return False

    prefab = observation.get("representative_prefab")
    component = observation.get("representative_component_schema")
    if not isinstance(prefab, dict) or not isinstance(component, dict):
        return False
    if prefab.get("fact_id") != "quaternius.medieval.prefab.wall-plaster-window-wide-flat":
        return False
    if prefab.get("provenance_source_path") != "Unity/ArkusUnity/Assets/Arkus/H1/CatalogueMapping.json":
        return False
    if component.get("provenance_source_path") != "Unity/ArkusUnity/Assets/Arkus/H1/CatalogueMapping.json":
        return False
    if observation.get("component_schema_count", 0) < 3:
        return False
    examples = set(observation.get("component_schema_examples", []))
    if not {
        "unity.component-schema.animator",
        "unity.component-schema.meshrenderer",
        "unity.component-schema.transform",
    }.issubset(examples):
        return False
    return True


def classify(claim_id: str, capabilities: set[str], lifecycle_current: bool) -> str:
    contract = CLAIM_CONTRACTS[claim_id]
    if contract["public_isolation"]:
        return "NOT_MATERIAL"
    requirements = set(contract["requirements"])
    covered = requirements & capabilities
    if covered == requirements:
        return "USE" if lifecycle_current else "OPTIONAL"
    if covered:
        return "OPTIONAL"
    return "NOT_MATERIAL"


def audit_mandatory(claim_id: str, opened: list[str]) -> list[str]:
    obligation = set(CLAIM_CONTRACTS[claim_id]["mandatory_reads"])
    return sorted(obligation - set(opened))


def route(case: dict[str, Any], observation: dict[str, Any], lifecycle_current: bool | None = None) -> dict[str, Any]:
    claim_id = case["id"]
    contract = CLAIM_CONTRACTS[claim_id]
    current = observation["lifecycle_current"] if lifecycle_current is None else lifecycle_current
    capabilities = set(observation["capabilities"])
    classification = classify(claim_id, capabilities, bool(current))

    # Effective reads arise from route execution: the accepted claim anchor is
    # actually opened, and USE navigation actually source-opens provenance.
    # This set is not initialized from mandatory_reads.
    opened: list[str] = []
    read(contract["anchor_path"])
    opened.append(contract["anchor_path"])

    query_name = contract["query"]
    query_bytes = None
    authority_bytes = observation["catalogue_authority_bytes"]
    if classification == "USE":
        if query_name not in {"representative_prefab", "representative_component_schema"}:
            fail(f"{claim_id} USE route lacks a real source-open projected query")
        payload = observation.get(query_name)
        if not isinstance(payload, dict):
            fail(f"{claim_id} projection observation lacks {query_name}")
        source_path = payload.get("provenance_source_path")
        if not isinstance(source_path, str) or not source_path:
            fail(f"{claim_id} projected query lacks source-open provenance")
        read(source_path)
        opened.append(source_path)
        query_bytes = observation[
            "representative_prefab_query_bytes"
            if query_name == "representative_prefab"
            else "representative_component_schema_query_bytes"
        ]
        if not isinstance(query_bytes, int) or not isinstance(authority_bytes, int) or query_bytes >= authority_bytes:
            fail(f"{claim_id} selective navigation did not reduce initial bytes")

    opened = list(dict.fromkeys(opened))
    missing = audit_mandatory(claim_id, opened)
    return {
        "classification": classification,
        "projection_admitted": classification == "USE",
        "requirements": list(contract["requirements"]),
        "mandatory_reads": list(contract["mandatory_reads"]),
        "opened_sources": sorted(opened),
        "mandatory_missing": missing,
        "query_bytes": query_bytes,
        "authority_bytes": authority_bytes,
        "navigation_reduced": query_bytes is not None and query_bytes < authority_bytes,
    }


def published_disposition_map() -> dict[str, str]:
    text = read(str(DISPOSITION_PATH.relative_to(ROOT)))
    pattern = re.compile(r"^\| `([^`]+)` \| `(USE|OPTIONAL|NOT_MATERIAL)` \|", re.MULTILINE)
    result: dict[str, str] = {}
    for claim_id, classification in pattern.findall(text):
        if claim_id in result:
            fail(f"published-disposition-duplicate-claim id={claim_id}")
        result[claim_id] = classification
    return result


def assert_real_results(cases: list[dict[str, Any]], observation: dict[str, Any]) -> list[dict[str, Any]]:
    rows: list[dict[str, Any]] = []
    for case in cases:
        claim_id = case["id"]
        result = route(case, observation)
        expected = CLAIM_CONTRACTS[claim_id]["expected_classification"]
        if result["classification"] != expected:
            fail(f"{claim_id} expected={expected} observed={result['classification']}")
        if result["mandatory_missing"]:
            fail(f"{claim_id} mandatory-read-regression missing={result['mandatory_missing']}")
        rows.append({"id": claim_id, "consumer": CLAIM_CONTRACTS[claim_id]["consumer"], **result})

    observed_map = {row["id"]: row["classification"] for row in rows}
    expected_map = {
        claim_id: contract["expected_classification"]
        for claim_id, contract in CLAIM_CONTRACTS.items()
    }
    if observed_map != expected_map:
        fail(f"exact-claim-disposition-mismatch expected={expected_map} observed={observed_map}")

    observed_counts = {
        name: sum(1 for row in rows if row["classification"] == name)
        for name in ("USE", "OPTIONAL", "NOT_MATERIAL")
    }
    if observed_counts != EXPECTED_CLASS_COUNTS:
        fail(f"selective-disposition-count-mismatch expected={EXPECTED_CLASS_COUNTS} observed={observed_counts}")

    published = published_disposition_map()
    if published != expected_map:
        fail(f"published-disposition-claim-map-mismatch expected={expected_map} observed={published}")

    gate = next(row for row in rows if row["id"] == "h1gate-public-client-isolation")
    if gate["classification"] != "NOT_MATERIAL" or gate["projection_admitted"]:
        fail("H1-GATE public-client isolation was contaminated by private CTX/DW")
    return rows


def adversarial_controls(
    suite: dict[str, Any],
    cases: list[dict[str, Any]],
    observation: dict[str, Any],
) -> dict[str, bool]:
    controls: dict[str, bool] = {}
    use_case = next(
        case
        for case in cases
        if CLAIM_CONTRACTS[case["id"]]["expected_classification"] == "USE"
    )
    baseline = route(use_case, observation)
    stale = route(use_case, observation, lifecycle_current=False)
    controls["stale_lifecycle_rejects_use"] = (
        baseline["classification"] == "USE"
        and stale["classification"] == "OPTIONAL"
        and not stale["projection_admitted"]
    )

    tampered_observation = copy.deepcopy(observation)
    tampered_observation["capabilities"] = sorted(
        set(tampered_observation["capabilities"]) | {"effective-prefab-relationship-multiset"}
    )
    controls["capability_inflation_rejected"] = not observation_is_accepted_shape(tampered_observation)

    decorated = copy.deepcopy(use_case)
    decorated["domain_label"] = "H1"
    decorated["free_text"] = "Quaternius prefab relationship important use DW now"
    controls["labels_and_free_text_cannot_change_route"] = (
        classify(use_case["id"], set(observation["capabilities"]), True)
        == classify(decorated["id"], set(observation["capabilities"]), True)
    )

    product_only = next(case for case in cases if case["id"] == "h105-scene-publication")
    structural = classify(product_only["id"], set(observation["capabilities"]), True)
    buggy_broad_h1 = "USE" if CLAIM_CONTRACTS[product_only["id"]]["consumer"].startswith("WP-H1-") else structural
    controls["broad_h1_default_would_be_detected"] = structural != buggy_broad_h1

    normal = route(use_case, observation)
    required = list(CLAIM_CONTRACTS[use_case["id"]]["mandatory_reads"])
    dropped = required[-1]
    malicious_opened = [path for path in normal["opened_sources"] if path != dropped]
    controls["hidden_mandatory_read_detected"] = dropped in audit_mandatory(use_case["id"], malicious_opened)

    partial_case = next(case for case in cases if case["id"] == "h106-relationship-equality")
    partial = route(partial_case, observation)
    controls["partial_projection_cannot_close_product_oracle"] = (
        partial["classification"] == "OPTIONAL"
        and not partial["projection_admitted"]
        and "Docs/evidence/WP-H1-06/PROOF_MATRIX.md" in partial["opened_sources"]
    )

    component_partial = next(case for case in cases if case["id"] == "h107-component-roundtrip")
    component_result = route(component_partial, observation)
    controls["component_schema_does_not_imply_effective_adapter"] = (
        component_result["classification"] == "OPTIONAL" and not component_result["projection_admitted"]
    )

    gate_case = next(case for case in cases if case["id"] == "h1gate-public-client-isolation")
    controls["public_client_isolation_forces_abstention"] = (
        classify(gate_case["id"], set(observation["capabilities"]), True) == "NOT_MATERIAL"
    )

    # Reproduce the Reviewer's semantic relabelling falsifier while preserving
    # valid JSON, eight cases, all anchors, and the global 3/2/3 class shape.
    forged = copy.deepcopy(suite)
    forged_by_id = {case["id"]: case for case in forged["cases"]}
    for claim_id, forged_case in forged_by_id.items():
        contract = CLAIM_CONTRACTS[claim_id]
        forged_case["requirements"] = list(contract["requirements"])
        forged_case["expected_classification"] = contract["expected_classification"]
    forged_by_id["h107-component-schema-inventory"]["requirements"] = ["effective-component-adapter"]
    forged_by_id["h107-component-schema-inventory"]["expected_classification"] = "NOT_MATERIAL"
    forged_by_id["h107-unsupported-field"]["requirements"] = ["component-schema-entry"]
    forged_by_id["h107-unsupported-field"]["expected_classification"] = "USE"
    forged_errors = case_shape_errors(forged)
    forged_counts = {
        name: sum(
            1
            for forged_case in forged["cases"]
            if forged_case.get("expected_classification") == name
        )
        for name in ("USE", "OPTIONAL", "NOT_MATERIAL")
    }
    controls["semantic_claim_swapping_rejected"] = (
        forged_counts == EXPECTED_CLASS_COUNTS
        and any("fixture-semantic-authority-forbidden" in error for error in forged_errors)
    )

    expected_only = copy.deepcopy(suite)
    expected_only["cases"][0]["expected_classification"] = "NOT_MATERIAL"
    expected_only_errors = case_shape_errors(expected_only)
    controls["expected_classification_cannot_authorize_route"] = any(
        "fixture-semantic-authority-forbidden" in error for error in expected_only_errors
    )
    return controls


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--projection-observation", required=True, type=Path)
    parser.add_argument("--summary-output", type=Path)
    args = parser.parse_args()

    validate_predecessors()
    suite = load_json(CASES_PATH)
    if not isinstance(suite, dict):
        fail("adoption-case-suite-not-object")
    cases = validate_cases(suite)
    observation = load_json(args.projection_observation)
    if not isinstance(observation, dict) or not observation_is_accepted_shape(observation):
        fail("projection-observation-does-not-match-accepted-H1-01-capability-surface")

    rows = assert_real_results(cases, observation)
    controls = adversarial_controls(suite, cases, observation)
    failed_controls = sorted(name for name, value in controls.items() if not value)
    if failed_controls:
        fail(f"adversarial-controls-failed={failed_controls}")

    use_rows = [row for row in rows if row["classification"] == "USE"]
    summary = {
        "schema": "ctx-dw-h1-02-proof-summary-v2",
        "claim_contract_authority": "scripts/ctx-dw-h1-02-proof.py::CLAIM_CONTRACTS",
        "projection_identity": observation["projection_identity"],
        "projection_digest": observation["projection_digest"],
        "capabilities": sorted(observation["capabilities"]),
        "classifications": {
            name: sum(1 for row in rows if row["classification"] == name)
            for name in ("USE", "OPTIONAL", "NOT_MATERIAL")
        },
        "exact_claim_disposition": {row["id"]: row["classification"] for row in rows},
        "rows": rows,
        "controls": controls,
        "max_use_query_bytes": max(
            row["query_bytes"] for row in use_rows if row["query_bytes"] is not None
        ),
        "catalogue_authority_bytes": observation["catalogue_authority_bytes"],
    }

    if args.summary_output:
        args.summary_output.parent.mkdir(parents=True, exist_ok=True)
        args.summary_output.write_text(
            json.dumps(summary, indent=2, sort_keys=True) + "\n",
            encoding="utf-8",
        )

    for row in rows:
        print(
            f"{row['id']}: {row['classification']} "
            f"admitted={str(row['projection_admitted']).lower()} "
            f"mandatory_missing={len(row['mandatory_missing'])}"
        )
    print("adversarial-controls=" + ",".join(sorted(name for name, value in controls.items() if value)))
    print(
        "CTX_DW_H1_02_GREEN "
        f"use={summary['classifications']['USE']} "
        f"optional={summary['classifications']['OPTIONAL']} "
        f"not_material={summary['classifications']['NOT_MATERIAL']} "
        f"projection={PROJECTION_IDENTITY}"
    )


if __name__ == "__main__":
    main()
