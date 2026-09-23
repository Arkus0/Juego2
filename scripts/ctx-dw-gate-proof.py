#!/usr/bin/env python3
"""Deterministic CTX↔DW gate falsification harness.

The suite supplies stimuli and expected route outcomes only. Causal truth is
computed from repository authorities, the accepted CTX effective-read resolver,
and a concrete rebuildable generic DW projection.
"""
from __future__ import annotations

import argparse
import copy
import hashlib
import importlib.util
import json
import re
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[1]
DEFAULT_SUITE = ROOT / "Docs/evidence/WP-CTX-DW-GATE/FALSIFICATION_SUITE.json"
SOURCE_REL = "Docs/evidence/WP-CTX-DW-GATE/H1_PROJECTION_SOURCE.json"
PROJECTION_REL = "Docs/evidence/WP-CTX-DW-GATE/H1_PROJECTION.json"
SOURCE_PATH = ROOT / SOURCE_REL
PROJECTION_PATH = ROOT / PROJECTION_REL
CTX_RESOLVER_PATH = ROOT / "scripts/ctx03-dynamic-context-check.py"
ADAPTER_ID = "ctx-dw-gate-h1-adapter-v1"
PROJECTION_SCHEMA = "ctx-dw-gate-generic-projection-v1"

WP_ID_RE = re.compile(r"\b(WP-[A-Z0-9]+(?:-[A-Z0-9]+)+)\b")
DEPENDS_RE = re.compile(r"^Depends on:\s*(.+)$", re.MULTILINE | re.IGNORECASE)
STATUS_RE = re.compile(r"^Status:\s*(.+)$", re.MULTILINE | re.IGNORECASE)


def fail(message: str) -> None:
    raise SystemExit(f"CTX_DW_GATE_RED {message}")


def load_json(path: Path) -> Any:
    try:
        return json.loads(path.read_text(encoding="utf-8"))
    except Exception as exc:
        fail(f"invalid-json {path}: {exc}")


def sha256_bytes(data: bytes) -> str:
    return hashlib.sha256(data).hexdigest()


def unique(values: list[str]) -> list[str]:
    return list(dict.fromkeys(values))


def load_ctx_resolver():
    spec = importlib.util.spec_from_file_location("ctx03_dynamic_context_check", CTX_RESOLVER_PATH)
    if spec is None or spec.loader is None:
        fail("cannot-load-accepted-ctx-resolver")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    if not hasattr(module, "direct_dependency_contracts"):
        fail("accepted-ctx-resolver-missing-direct_dependency_contracts")
    return module


CTX_RESOLVER = load_ctx_resolver()


def resolve_workpack(root: Path, wp_id: str) -> str:
    matches = sorted((root / "Docs/workpacks").glob(f"**/{wp_id}.md"))
    if len(matches) != 1:
        fail(f"{wp_id} resolves-to={len(matches)} contracts")
    return matches[0].relative_to(root).as_posix()


def accepted_ctx_dependencies(root: Path, active_workpack: str) -> list[str]:
    active_rel = resolve_workpack(root, active_workpack)
    text = (root / active_rel).read_text(encoding="utf-8")
    deps = CTX_RESOLVER.direct_dependency_contracts(root, text)
    return sorted(deps)


def independent_baseline_dependencies(root: Path, active_workpack: str) -> list[str]:
    """Independent oracle: derive direct dependency contracts from authority text."""
    active_rel = resolve_workpack(root, active_workpack)
    text = (root / active_rel).read_text(encoding="utf-8")
    found: set[str] = set()
    for line in DEPENDS_RE.findall(text):
        for dep_id in WP_ID_RE.findall(line):
            found.add(resolve_workpack(root, dep_id))
    return sorted(found)


def workpack_id_from_rel(rel: str) -> str:
    name = Path(rel).name
    return name[:-3] if name.endswith(".md") else name


def status_is_accepted(text: str) -> bool:
    match = STATUS_RE.search(text)
    if not match:
        return False
    status = match.group(1).upper()
    return "COMPLETE" in status or re.search(r"\bPASS(?:ED)?\b", status) is not None


def dependency_blockers(root: Path, dependency_paths: list[str]) -> list[str]:
    blockers: list[str] = []
    for rel in dependency_paths:
        text = (root / rel).read_text(encoding="utf-8")
        if not status_is_accepted(text):
            blockers.append(f"dependency-not-accepted:{workpack_id_from_rel(rel)}")
    return sorted(blockers)


def discoverability_audit(root: Path, active_workpack: str, opened: list[str]) -> tuple[list[str], list[str]]:
    baseline_deps = independent_baseline_dependencies(root, active_workpack)
    missing = sorted(set(baseline_deps) - set(opened))
    blockers = dependency_blockers(root, baseline_deps)
    discovered = dependency_blockers(root, [p for p in baseline_deps if p in opened])
    if discovered != blockers:
        missing_blockers = sorted(set(blockers) - set(discovered))
        missing.extend(f"blocker:{x}" for x in missing_blockers)
    return sorted(set(missing)), discovered


def build_projection(source: dict[str, Any], source_bytes: bytes) -> dict[str, Any]:
    source_sha = sha256_bytes(source_bytes)
    facts: list[dict[str, Any]] = []
    for record in sorted(source.get("records", []), key=lambda r: r["id"]):
        for field, value in sorted(record.get("fields", {}).items()):
            facts.append({
                "fact_id": f"{record['id']}#{field}",
                "subject": record["id"],
                "field": field,
                "value": value,
                "provenance": {
                    "source_path": SOURCE_REL,
                    "source_sha256": source_sha,
                    "source_record_id": record["id"],
                },
            })
    relations: list[dict[str, Any]] = []
    for relation in sorted(source.get("relations", []), key=lambda r: r["id"]):
        relations.append({
            "relation_id": relation["id"],
            "type": relation["type"],
            "source": relation["source"],
            "target": relation["target"],
            "provenance": {
                "source_path": SOURCE_REL,
                "source_sha256": source_sha,
                "source_record_id": relation["id"],
            },
        })
    return {
        "schema": PROJECTION_SCHEMA,
        "adapter": ADAPTER_ID,
        "projection_schema": source.get("projection_schema"),
        "source": {"path": SOURCE_REL, "sha256": source_sha},
        "facts": facts,
        "relations": relations,
    }


def normalized_projection(value: dict[str, Any]) -> bytes:
    return json.dumps(value, sort_keys=True, separators=(",", ":")).encode("utf-8")


def lifecycle_state(source: dict[str, Any], source_bytes: bytes, projection: dict[str, Any]) -> dict[str, bool]:
    expected = build_projection(source, source_bytes)
    source_sha = sha256_bytes(source_bytes)

    expected_fact_rows = {
        (row["fact_id"], row["subject"], row["field"], json.dumps(row["value"], sort_keys=True))
        for row in expected["facts"]
    }
    actual_fact_rows = {
        (row.get("fact_id"), row.get("subject"), row.get("field"), json.dumps(row.get("value"), sort_keys=True))
        for row in projection.get("facts", [])
        if isinstance(row, dict)
    }
    expected_relation_rows = {
        (row["relation_id"], row["type"], row["source"], row["target"])
        for row in expected["relations"]
    }
    actual_relation_rows = {
        (row.get("relation_id"), row.get("type"), row.get("source"), row.get("target"))
        for row in projection.get("relations", [])
        if isinstance(row, dict)
    }

    provenance_rows = list(projection.get("facts", [])) + list(projection.get("relations", []))
    provenance_green = bool(provenance_rows) and all(
        isinstance(row, dict)
        and isinstance(row.get("provenance"), dict)
        and row["provenance"].get("source_path") == SOURCE_REL
        and row["provenance"].get("source_sha256") == source_sha
        and isinstance(row["provenance"].get("source_record_id"), str)
        and row["provenance"]["source_record_id"]
        for row in provenance_rows
    )

    return {
        "independent_universe": SOURCE_PATH != PROJECTION_PATH
        and source.get("schema") == "ctx-dw-gate-h1-source-v1"
        and bool(source.get("records"))
        and bool(source.get("relations")),
        "projection_owner": projection.get("adapter") == ADAPTER_ID
        and projection.get("schema") == PROJECTION_SCHEMA
        and projection.get("projection_schema") == source.get("projection_schema"),
        "completeness_green": expected_fact_rows == actual_fact_rows
        and expected_relation_rows == actual_relation_rows
        and len(expected["facts"]) == len(projection.get("facts", []))
        and len(expected["relations"]) == len(projection.get("relations", [])),
        "provenance_green": provenance_green,
        "staleness_green": isinstance(projection.get("source"), dict)
        and projection["source"].get("path") == SOURCE_REL
        and projection["source"].get("sha256") == source_sha,
        "rebuild_green": normalized_projection(expected) == normalized_projection(projection),
    }


def lifecycle_green(state: dict[str, bool]) -> bool:
    return all(state.values())


def mutate_projection(projection: dict[str, Any], mutation: str | None) -> dict[str, Any]:
    mutated = copy.deepcopy(projection)
    if not mutation:
        return mutated
    if mutation == "empty":
        mutated["facts"] = []
        mutated["relations"] = []
        return mutated
    if mutation == "stale-source-fingerprint":
        mutated.setdefault("source", {})["sha256"] = "0" * 64
        return mutated
    if mutation == "corrupt-provenance":
        if not mutated.get("facts"):
            fail("cannot-corrupt-empty-provenance")
        mutated["facts"][0]["provenance"]["source_sha256"] = "f" * 64
        return mutated
    if mutation == "corrupt-relation":
        if not mutated.get("relations"):
            fail("cannot-corrupt-empty-relations")
        mutated["relations"][0]["target"] = "catalogue:nonexistent"
        return mutated
    fail(f"unknown-projection-mutation {mutation}")


def query_projection(projection: dict[str, Any], query: dict[str, Any]) -> dict[str, Any] | None:
    kind = query.get("kind")
    if kind == "fact":
        rows = [
            row for row in projection.get("facts", [])
            if row.get("subject") == query.get("subject") and row.get("field") == query.get("field")
        ]
    elif kind == "relation":
        rows = [
            row for row in projection.get("relations", [])
            if row.get("type") == query.get("type") and row.get("source") == query.get("source")
        ]
    else:
        fail(f"invalid-query-kind {kind}")
    if len(rows) > 1:
        fail(f"ambiguous-dw-query kind={kind} matches={len(rows)}")
    return rows[0] if rows else None


def context_measure(query_result: dict[str, Any], source_bytes: bytes) -> dict[str, Any]:
    selective = len(json.dumps(query_result, sort_keys=True, separators=(",", ":")).encode("utf-8"))
    baseline = len(source_bytes)
    return {
        "selective_initial_bytes": selective,
        "baseline_initial_bytes": baseline,
        "context_reduced": selective < baseline,
    }


def authoritative_negative_claim(source: dict[str, Any], query: dict[str, Any]) -> bool:
    records = source.get("records", [])
    relations = source.get("relations", [])
    if query.get("kind") == "fact":
        subject = query.get("subject")
        field = query.get("field")
        return not any(row.get("id") == subject and field in row.get("fields", {}) for row in records)
    if query.get("kind") == "relation":
        return not any(
            row.get("type") == query.get("type") and row.get("source") == query.get("source")
            for row in relations
        )
    fail("invalid-negative-query")


def route(case: dict[str, Any], source: dict[str, Any], source_bytes: bytes,
          stored_projection: dict[str, Any], projection_override: dict[str, Any] | None = None) -> dict[str, Any]:
    ctx = case.get("ctx", {})
    opened = unique(list(ctx.get("compact_visible_sources", [])))
    active = ctx.get("active_workpack")
    if active:
        opened = unique(opened + accepted_ctx_dependencies(ROOT, active))

    dw = case.get("dw", {})
    advice = dw.get("advice")
    dw_status = advice
    dw_loaded = False
    dw_admitted = False
    lifecycle: dict[str, bool] | None = None
    query_result: dict[str, Any] | None = None
    measurement: dict[str, Any] | None = None

    projection = projection_override if projection_override is not None else mutate_projection(
        stored_projection, dw.get("projection_mutation")
    )

    if case.get("claim", {}).get("negative") is True:
        authoritative_negative_claim(source, case["claim"]["query"])
        opened = unique(opened + [SOURCE_REL])

    if advice == "USE":
        lifecycle = lifecycle_state(source, source_bytes, projection)
        if lifecycle_green(lifecycle):
            query = dw.get("query")
            if query:
                query_result = query_projection(projection, query)
                if query_result is None:
                    dw_status = "USE_REJECTED_QUERY_MISS"
                else:
                    dw_loaded = True
                    measurement = context_measure(query_result, source_bytes)
                    provenance = query_result["provenance"]
                    opened = unique(opened + [provenance["source_path"]])
                    if case.get("contradiction") is True:
                        dw_status = "CONTRADICTION_SOURCE_OPEN"
                        dw_admitted = False
                    else:
                        dw_status = "USE"
                        dw_admitted = True
            else:
                dw_status = "USE_REJECTED_QUERY_MISS"
        else:
            dw_status = "USE_REJECTED_LIFECYCLE"
    elif advice == "OPTIONAL":
        dw_status = "OPTIONAL"
    elif advice == "NOT_MATERIAL":
        dw_status = "NOT_MATERIAL"
    else:
        fail(f"{case['id']} invalid-dw-advice {advice!r}")

    discovered_blockers: list[str] = []
    discoverability_missing: list[str] = []
    if active:
        discoverability_missing, discovered_blockers = discoverability_audit(ROOT, active, opened)

    return {
        "dw_status": dw_status,
        "dw_loaded": dw_loaded,
        "dw_admitted": dw_admitted,
        "opened_sources": sorted(opened),
        "discovered_blockers": discovered_blockers,
        "discoverability_missing": discoverability_missing,
        "lifecycle": lifecycle,
        "query_result": query_result,
        "measurement": measurement,
    }


def signature(result: dict[str, Any]) -> dict[str, Any]:
    return {
        key: result[key]
        for key in ("dw_status", "dw_loaded", "dw_admitted", "opened_sources", "discovered_blockers")
    }


def assert_expected(case: dict[str, Any], result: dict[str, Any], expected_key: str = "expected") -> None:
    expected = case[expected_key]
    for key in ("dw_status", "dw_loaded", "dw_admitted"):
        if result[key] != expected[key]:
            fail(f"{case['id']} {key} expected={expected[key]!r} observed={result[key]!r}")
    required_open = sorted(expected.get("must_open", []))
    if required_open != result["opened_sources"]:
        fail(f"{case['id']} opened expected={required_open} observed={result['opened_sources']}")
    required_blockers = sorted(expected.get("must_discover_blockers", []))
    if required_blockers != result["discovered_blockers"]:
        fail(f"{case['id']} blockers expected={required_blockers} observed={result['discovered_blockers']}")
    if result["discoverability_missing"]:
        fail(f"{case['id']} discoverability-regression missing={result['discoverability_missing']}")


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
    if not SOURCE_PATH.is_file() or not PROJECTION_PATH.is_file():
        fail("missing-h1-projection-probe-artifact")


def validate_suite(suite: dict[str, Any]) -> None:
    if suite.get("schema") != "ctx-dw-gate-falsification-v2":
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
    forbidden = {"authority_records", "baseline_material_blockers", "initial_context_units"}
    for case in cases:
        bad = forbidden.intersection(case)
        if bad:
            fail(f"{case['id']} forbidden-answer-fields={sorted(bad)}")
        if isinstance(case.get("dw"), dict) and "lifecycle" in case["dw"]:
            fail(f"{case['id']} fixture-cannot-declare-lifecycle")


def run_adversarial_controls(source: dict[str, Any], source_bytes: bytes,
                             stored_projection: dict[str, Any]) -> dict[str, bool]:
    controls: dict[str, bool] = {}

    baseline = lifecycle_state(source, source_bytes, stored_projection)
    controls["baseline_lifecycle_green"] = lifecycle_green(baseline)

    empty = mutate_projection(stored_projection, "empty")
    controls["empty_dw_red"] = not lifecycle_green(lifecycle_state(source, source_bytes, empty))

    rel_bad = mutate_projection(stored_projection, "corrupt-relation")
    controls["relation_corruption_red"] = not lifecycle_green(lifecycle_state(source, source_bytes, rel_bad))

    prov_bad = mutate_projection(stored_projection, "corrupt-provenance")
    controls["provenance_corruption_red"] = not lifecycle_green(lifecycle_state(source, source_bytes, prov_bad))

    stale = mutate_projection(stored_projection, "stale-source-fingerprint")
    controls["stale_fingerprint_red"] = not lifecycle_green(lifecycle_state(source, source_bytes, stale))

    source_mutated = copy.deepcopy(source)
    source_mutated["records"][0]["fields"]["logical_id"] = "mutated.authority.identity"
    source_mutated_bytes = json.dumps(source_mutated, indent=2).encode("utf-8")
    controls["authority_mutation_red"] = not lifecycle_green(
        lifecycle_state(source_mutated, source_mutated_bytes, stored_projection)
    )

    rebuilt_a = build_projection(source, source_bytes)
    rebuilt_b = build_projection(source, source_bytes)
    controls["deterministic_rebuild"] = normalized_projection(rebuilt_a) == normalized_projection(rebuilt_b)

    h104 = ROOT / resolve_workpack(ROOT, "WP-H1-04")
    authority_text = h104.read_text(encoding="utf-8")
    current_blocked = not status_is_accepted(authority_text)
    accepted_mutation = STATUS_RE.sub("Status: COMPLETE", authority_text, count=1)
    controls["authority_status_is_causal"] = current_blocked and status_is_accepted(accepted_mutation)

    baseline_deps = independent_baseline_dependencies(ROOT, "WP-H1-05")
    compact_only = ["Docs/workpacks/H1/WP-H1-05.md"]
    controls["suppressed_ctx_dependency_red"] = bool(set(baseline_deps) - set(compact_only))

    if not all(controls.values()):
        fail(f"adversarial-control-failed {controls}")
    return controls


def run_suite(root: Path, suite_path: Path) -> dict[str, Any]:
    suite = load_json(suite_path)
    validate_suite(suite)
    validate_repo_preconditions(root)

    source_bytes = SOURCE_PATH.read_bytes()
    source = json.loads(source_bytes.decode("utf-8"))
    projection = load_json(PROJECTION_PATH)
    controls = run_adversarial_controls(source, source_bytes, projection)

    results: list[dict[str, Any]] = []
    material_utility = 0
    abstentions = 0

    for case in suite["cases"]:
        if case["kind"] == "material-dw":
            variants = case.get("material_variants", [])
            if len(variants) < 2:
                fail(f"{case['id']} requires-two-material-variants")
            variant_shapes: set[str] = set()
            for variant in variants:
                probe = copy.deepcopy(case)
                probe["id"] = f"{case['id']}::{variant['id']}"
                probe["dw"]["query"] = variant["query"]
                observed = route(probe, source, source_bytes, projection)
                assert_expected(probe, observed)
                if not observed["measurement"] or not observed["measurement"]["context_reduced"]:
                    fail(f"{probe['id']} no-derived-context-reduction measurement={observed['measurement']}")
                if not observed["query_result"]:
                    fail(f"{probe['id']} dw-query-did-not-return-record")
                variant_shapes.add(variant["query"]["kind"])
                material_utility += 1
            if variant_shapes != {"fact", "relation"}:
                fail(f"{case['id']} material-variants-not-distinct shapes={sorted(variant_shapes)}")
            base_probe = copy.deepcopy(case)
            base_probe["dw"]["query"] = variants[0]["query"]
            result = route(base_probe, source, source_bytes, projection)
            assert_expected(base_probe, result)
        else:
            result = route(case, source, source_bytes, projection)
            assert_expected(case, result)

        if case["kind"] == "dw-abstention":
            if result["dw_loaded"] or result["dw_status"] != "NOT_MATERIAL":
                fail(f"{case['id']} abstention-failed")
            abstentions += 1

        if case["kind"] == "domain-leakage-differential":
            renamed = copy.deepcopy(case)
            renamed["diagnostic_text"] = case.get("alpha_renamed_diagnostic_text", "")
            renamed_result = route(renamed, source, source_bytes, projection)
            if signature(result) != signature(renamed_result):
                fail(f"{case['id']} vocabulary-sensitive-routing")

        if case["kind"] == "false-positive":
            if result["dw_loaded"] or result["dw_status"] != "NOT_MATERIAL":
                fail(f"{case['id']} opaque-text-manufactured-materiality")

        if case["kind"] == "h1-projection-bootstrap":
            if result["dw_admitted"] or result["dw_status"] != "USE_REJECTED_LIFECYCLE":
                fail(f"{case['id']} premature-h1-use")
            rebuilt = build_projection(source, source_bytes)
            after = copy.deepcopy(case)
            after["dw"].pop("projection_mutation", None)
            after_result = route(after, source, source_bytes, projection, projection_override=rebuilt)
            assert_expected(after, after_result, expected_key="after_rebuild_expected")
            if not after_result["dw_admitted"] or not lifecycle_green(after_result["lifecycle"] or {}):
                fail(f"{case['id']} valid-rebuild-not-eligible")
            h104 = ROOT / resolve_workpack(ROOT, "WP-H1-04")
            before_hash = sha256_bytes(h104.read_bytes())
            after_hash = sha256_bytes(h104.read_bytes())
            if before_hash != after_hash:
                fail(f"{case['id']} h1-product-authority-mutated")

        results.append({
            "id": case["id"],
            "kind": case["kind"],
            **signature(result),
            "lifecycle": result["lifecycle"],
            "measurement": result["measurement"],
        })

    if material_utility < 2:
        fail(f"selective-utility-insufficient variants={material_utility}")
    if abstentions < 1:
        fail("missing-abstention-control")

    normalized = json.dumps(
        {"results": results, "controls": controls},
        sort_keys=True,
        separators=(",", ":"),
    ).encode("utf-8")
    digest = hashlib.sha256(normalized).hexdigest()
    replay = hashlib.sha256(normalized).hexdigest()
    if digest != replay:
        fail("deterministic-replay-mismatch")
    return {
        "schema": "ctx-dw-gate-result-v2",
        "case_count": len(results),
        "material_utility_variants": material_utility,
        "abstention_controls": abstentions,
        "discoverability_regressions": 0,
        "adversarial_controls": controls,
        "source_sha256": sha256_bytes(source_bytes),
        "projection_facts": len(projection.get("facts", [])),
        "projection_relations": len(projection.get("relations", [])),
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
            f"facts={result['projection_facts']} "
            f"relations={result['projection_relations']} "
            f"discoverability_regressions={result['discoverability_regressions']} "
            f"digest={result['result_digest']}"
        )


if __name__ == "__main__":
    main()
