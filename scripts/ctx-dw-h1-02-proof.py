#!/usr/bin/env python3
"""Deterministic selective-adoption proof for WP-CTX-DW-H1-02.

The fixtures identify real accepted claim anchors and their material capability
requirements. Classification is computed from the capability surface emitted by
the accepted H1-01 projection, never from workpack/domain labels or fixture prose.
"""
from __future__ import annotations

import argparse
import copy
import json
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[1]
CASES_PATH = ROOT / "Docs/evidence/WP-CTX-DW-H1-02/ADOPTION_CASES.json"

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


def validate_cases(suite: dict[str, Any]) -> list[dict[str, Any]]:
    if suite.get("schema") != "ctx-dw-h1-02-adoption-cases-v1":
        fail("wrong-case-schema")
    cases = suite.get("cases")
    if not isinstance(cases, list) or len(cases) != 8:
        fail(f"expected-exactly-8-cases observed={len(cases) if isinstance(cases, list) else 'invalid'}")
    ids = [case.get("id") for case in cases]
    if len(ids) != len(set(ids)) or any(not isinstance(value, str) or not value for value in ids):
        fail("invalid-or-duplicate-case-id")

    for case in cases:
        for field in ("consumer", "anchor_path", "anchor_text", "requirements", "mandatory_reads", "expected_classification"):
            if field not in case:
                fail(f"{case['id']} missing-field={field}")
        if case["expected_classification"] not in {"USE", "OPTIONAL", "NOT_MATERIAL"}:
            fail(f"{case['id']} invalid-expected-classification")
        requirements = case["requirements"]
        mandatory = case["mandatory_reads"]
        if not isinstance(requirements, list) or not requirements or any(not isinstance(value, str) or not value for value in requirements):
            fail(f"{case['id']} invalid-requirements")
        if not isinstance(mandatory, list) or not mandatory or any(not isinstance(value, str) or not value for value in mandatory):
            fail(f"{case['id']} invalid-mandatory-reads")
        anchor_text = read(case["anchor_path"])
        if case["anchor_text"] not in anchor_text:
            fail(f"{case['id']} accepted-anchor-not-found")
        for rel in mandatory:
            if not (ROOT / rel).is_file():
                fail(f"{case['id']} missing-mandatory-read={rel}")

    consumers = {case["consumer"] for case in cases}
    if not {"WP-H1-05", "WP-H1-06", "WP-H1-07", "WP-H1-GATE"}.issubset(consumers):
        fail("consumer-shape-coverage-incomplete")
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


def classify(case: dict[str, Any], capabilities: set[str], lifecycle_current: bool) -> str:
    if case.get("public_isolation") is True:
        return "NOT_MATERIAL"
    requirements = set(case["requirements"])
    covered = requirements & capabilities
    if covered == requirements:
        return "USE" if lifecycle_current else "OPTIONAL"
    if covered:
        return "OPTIONAL"
    return "NOT_MATERIAL"


def audit_mandatory(case: dict[str, Any], opened: list[str]) -> list[str]:
    return sorted(set(case["mandatory_reads"]) - set(opened))


def route(case: dict[str, Any], observation: dict[str, Any], lifecycle_current: bool | None = None) -> dict[str, Any]:
    current = observation["lifecycle_current"] if lifecycle_current is None else lifecycle_current
    capabilities = set(observation["capabilities"])
    classification = classify(case, capabilities, bool(current))
    opened = list(dict.fromkeys(case["mandatory_reads"]))
    query_name = case.get("query")
    query_bytes = None
    authority_bytes = observation["catalogue_authority_bytes"]
    if classification == "USE":
        if query_name not in {"representative_prefab", "representative_component_schema"}:
            fail(f"{case['id']} USE route lacks a real source-open projected query")
        payload = observation.get(query_name)
        if not isinstance(payload, dict):
            fail(f"{case['id']} projection observation lacks {query_name}")
        source_path = payload.get("provenance_source_path")
        if not isinstance(source_path, str) or not source_path:
            fail(f"{case['id']} projected query lacks source-open provenance")
        opened.append(source_path)
        query_bytes = observation[
            "representative_prefab_query_bytes"
            if query_name == "representative_prefab"
            else "representative_component_schema_query_bytes"
        ]
        if not isinstance(query_bytes, int) or not isinstance(authority_bytes, int) or query_bytes >= authority_bytes:
            fail(f"{case['id']} selective navigation did not reduce initial bytes")

    opened = list(dict.fromkeys(opened))
    missing = audit_mandatory(case, opened)
    return {
        "classification": classification,
        "projection_admitted": classification == "USE",
        "opened_sources": sorted(opened),
        "mandatory_missing": missing,
        "query_bytes": query_bytes,
        "authority_bytes": authority_bytes,
        "navigation_reduced": query_bytes is not None and query_bytes < authority_bytes,
    }


def assert_real_results(cases: list[dict[str, Any]], observation: dict[str, Any]) -> list[dict[str, Any]]:
    rows: list[dict[str, Any]] = []
    for case in cases:
        result = route(case, observation)
        expected = case["expected_classification"]
        if result["classification"] != expected:
            fail(f"{case['id']} expected={expected} observed={result['classification']}")
        if result["mandatory_missing"]:
            fail(f"{case['id']} mandatory-read-regression missing={result['mandatory_missing']}")
        rows.append({"id": case["id"], "consumer": case["consumer"], **result})

    classes = {row["classification"] for row in rows}
    if classes != {"USE", "OPTIONAL", "NOT_MATERIAL"}:
        fail(f"selective-disposition-collapsed classes={sorted(classes)}")
    for consumer in ("WP-H1-05", "WP-H1-06"):
        if not any(row["consumer"] == consumer and row["classification"] == "USE" for row in rows):
            fail(f"{consumer} lacks a real admitted USE observation")
    h107_classes = {row["classification"] for row in rows if row["consumer"] == "WP-H1-07"}
    if h107_classes != {"USE", "OPTIONAL", "NOT_MATERIAL"}:
        fail(f"H1-07 guidance is not claim-selective observed={sorted(h107_classes)}")
    gate = next(row for row in rows if row["consumer"] == "WP-H1-GATE")
    if gate["classification"] != "NOT_MATERIAL" or gate["projection_admitted"]:
        fail("H1-GATE public-client isolation was contaminated by private CTX/DW")
    return rows


def adversarial_controls(cases: list[dict[str, Any]], observation: dict[str, Any]) -> dict[str, bool]:
    controls: dict[str, bool] = {}
    use_case = next(case for case in cases if case["expected_classification"] == "USE")
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
        classify(use_case, set(observation["capabilities"]), True)
        == classify(decorated, set(observation["capabilities"]), True)
    )

    product_only = next(case for case in cases if case["id"] == "h105-scene-publication")
    structural = classify(product_only, set(observation["capabilities"]), True)
    buggy_broad_h1 = "USE" if product_only["consumer"].startswith("WP-H1-") else structural
    controls["broad_h1_default_would_be_detected"] = structural != buggy_broad_h1

    normal = route(use_case, observation)
    malicious_opened = normal["opened_sources"][1:] if len(normal["opened_sources"]) > 1 else []
    controls["hidden_mandatory_read_detected"] = bool(audit_mandatory(use_case, malicious_opened))

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

    gate_case = next(case for case in cases if case["consumer"] == "WP-H1-GATE")
    controls["public_client_isolation_forces_abstention"] = (
        classify(gate_case, set(observation["capabilities"]), True) == "NOT_MATERIAL"
    )
    return controls


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--projection-observation", required=True, type=Path)
    parser.add_argument("--summary-output", type=Path)
    args = parser.parse_args()

    validate_predecessors()
    cases = validate_cases(load_json(CASES_PATH))
    observation = load_json(args.projection_observation)
    if not isinstance(observation, dict) or not observation_is_accepted_shape(observation):
        fail("projection-observation-does-not-match-accepted-H1-01-capability-surface")

    rows = assert_real_results(cases, observation)
    controls = adversarial_controls(cases, observation)
    failed_controls = sorted(name for name, value in controls.items() if not value)
    if failed_controls:
        fail(f"adversarial-controls-failed={failed_controls}")

    use_rows = [row for row in rows if row["classification"] == "USE"]
    summary = {
        "schema": "ctx-dw-h1-02-proof-summary-v1",
        "projection_identity": observation["projection_identity"],
        "projection_digest": observation["projection_digest"],
        "capabilities": sorted(observation["capabilities"]),
        "classifications": {name: sum(1 for row in rows if row["classification"] == name) for name in ("USE", "OPTIONAL", "NOT_MATERIAL")},
        "rows": rows,
        "controls": controls,
        "max_use_query_bytes": max(row["query_bytes"] for row in use_rows if row["query_bytes"] is not None),
        "catalogue_authority_bytes": observation["catalogue_authority_bytes"],
    }

    if args.summary_output:
        args.summary_output.parent.mkdir(parents=True, exist_ok=True)
        args.summary_output.write_text(json.dumps(summary, indent=2, sort_keys=True) + "\n", encoding="utf-8")

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
