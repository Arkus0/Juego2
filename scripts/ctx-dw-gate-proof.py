#!/usr/bin/env python3
"""Deterministic CTX↔DW gate falsification harness.

Fixtures provide stimuli and expected outcomes only. Causal truth is computed
from repository authorities, the accepted CTX effective-read resolver, a
rebuildable generic DW projection, and structural routing signals.
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
                "provenance": {"source_path": SOURCE_REL, "source_sha256": source_sha, "source_record_id": record["id"]},
            })
    relations: list[dict[str, Any]] = []
    for relation in sorted(source.get("relations", []), key=lambda r: r["id"]):
        relations.append({
            "relation_id": relation["id"], "type": relation["type"], "source": relation["source"], "target": relation["target"],
            "provenance": {"source_path": SOURCE_REL, "source_sha256": source_sha, "source_record_id": relation["id"]},
        })
    return {"schema": PROJECTION_SCHEMA, "adapter": ADAPTER_ID, "projection_schema": source.get("projection_schema"),
            "source": {"path": SOURCE_REL, "sha256": source_sha}, "facts": facts, "relations": relations}


def build_compact_ctx_surface(source: dict[str, Any]) -> dict[str, Any]:
    """Independent compact representation; intentionally not the DW projection shape."""
    facts = [
        {"entity": record["id"], "attribute": field, "content": value}
        for record in sorted(source.get("records", []), key=lambda r: r["id"])
        for field, value in sorted(record.get("fields", {}).items())
    ]
    relations = [
        {"edge": relation["id"], "predicate": relation["type"], "left": relation["source"], "right": relation["target"]}
        for relation in sorted(source.get("relations", []), key=lambda r: r["id"])
    ]
    return {"facts": facts, "relations": relations}


def normalized_projection(value: dict[str, Any]) -> bytes:
    return json.dumps(value, sort_keys=True, separators=(",", ":")).encode("utf-8")


def lifecycle_state(source: dict[str, Any], source_bytes: bytes, projection: dict[str, Any]) -> dict[str, bool]:
    expected = build_projection(source, source_bytes)
    source_sha = sha256_bytes(source_bytes)
    expected_fact_rows = {(row["fact_id"], row["subject"], row["field"], json.dumps(row["value"], sort_keys=True)) for row in expected["facts"]}
    actual_fact_rows = {(row.get("fact_id"), row.get("subject"), row.get("field"), json.dumps(row.get("value"), sort_keys=True)) for row in projection.get("facts", []) if isinstance(row, dict)}
    expected_relation_rows = {(row["relation_id"], row["type"], row["source"], row["target"]) for row in expected["relations"]}
    actual_relation_rows = {(row.get("relation_id"), row.get("type"), row.get("source"), row.get("target")) for row in projection.get("relations", []) if isinstance(row, dict)}
    provenance_rows = list(projection.get("facts", [])) + list(projection.get("relations", []))
    provenance_green = bool(provenance_rows) and all(
        isinstance(row, dict) and isinstance(row.get("provenance"), dict)
        and row["provenance"].get("source_path") == SOURCE_REL
        and row["provenance"].get("source_sha256") == source_sha
        and isinstance(row["provenance"].get("source_record_id"), str) and row["provenance"]["source_record_id"]
        for row in provenance_rows
    )
    return {
        "independent_universe": SOURCE_PATH != PROJECTION_PATH and source.get("schema") == "ctx-dw-gate-h1-source-v1" and bool(source.get("records")) and bool(source.get("relations")),
        "projection_owner": projection.get("adapter") == ADAPTER_ID and projection.get("schema") == PROJECTION_SCHEMA and projection.get("projection_schema") == source.get("projection_schema"),
        "completeness_green": expected_fact_rows == actual_fact_rows and expected_relation_rows == actual_relation_rows and len(expected["facts"]) == len(projection.get("facts", [])) and len(expected["relations"]) == len(projection.get("relations", [])),
        "provenance_green": provenance_green,
        "staleness_green": isinstance(projection.get("source"), dict) and projection["source"].get("path") == SOURCE_REL and projection["source"].get("sha256") == source_sha,
        "rebuild_green": normalized_projection(expected) == normalized_projection(projection),
    }


def lifecycle_green(state: dict[str, bool]) -> bool:
    return all(state.values())


def mutate_projection(projection: dict[str, Any], mutation: str | None) -> dict[str, Any]:
    mutated = copy.deepcopy(projection)
    if not mutation:
        return mutated
    if mutation == "empty":
        mutated["facts"], mutated["relations"] = [], []
    elif mutation == "stale-source-fingerprint":
        mutated.setdefault("source", {})["sha256"] = "0" * 64
    elif mutation == "corrupt-provenance":
        if not mutated.get("facts"): fail("cannot-corrupt-empty-provenance")
        mutated["facts"][0]["provenance"]["source_sha256"] = "f" * 64
    elif mutation == "corrupt-relation":
        if not mutated.get("relations"): fail("cannot-corrupt-empty-relations")
        mutated["relations"][0]["target"] = "catalogue:nonexistent"
    else:
        fail(f"unknown-projection-mutation {mutation}")
    return mutated


def query_projection(projection: dict[str, Any], query: dict[str, Any]) -> dict[str, Any] | None:
    kind = query.get("kind")
    if kind == "fact":
        rows = [row for row in projection.get("facts", []) if row.get("subject") == query.get("subject") and row.get("field") == query.get("field")]
    elif kind == "relation":
        rows = [row for row in projection.get("relations", []) if row.get("type") == query.get("type") and row.get("source") == query.get("source")]
    else:
        fail(f"invalid-query-kind {kind}")
    if len(rows) > 1: fail(f"ambiguous-dw-query kind={kind} matches={len(rows)}")
    return rows[0] if rows else None


def query_compact_ctx(surface: dict[str, Any], query: dict[str, Any]) -> dict[str, Any] | None:
    kind = query.get("kind")
    if kind == "fact":
        rows = [row for row in surface.get("facts", []) if row.get("entity") == query.get("subject") and row.get("attribute") == query.get("field")]
    elif kind == "relation":
        rows = [row for row in surface.get("relations", []) if row.get("predicate") == query.get("type") and row.get("left") == query.get("source")]
    else:
        fail(f"invalid-compact-query-kind {kind}")
    if len(rows) > 1: fail(f"ambiguous-compact-query kind={kind} matches={len(rows)}")
    return rows[0] if rows else None


def mutate_compact_ctx(surface: dict[str, Any], mutation: dict[str, Any] | None) -> dict[str, Any]:
    mutated = copy.deepcopy(surface)
    if not mutation: return mutated
    op, query = mutation.get("op"), mutation.get("query")
    if not isinstance(query, dict): fail("compact-mutation-missing-query")
    row = query_compact_ctx(mutated, query)
    if row is None: fail("compact-mutation-query-miss")
    if op == "replace-fact-value" and query.get("kind") == "fact": row["content"] = mutation.get("value")
    elif op == "replace-relation-target" and query.get("kind") == "relation": row["right"] = mutation.get("target")
    else: fail(f"unknown-compact-mutation {op}")
    return mutated


def contradiction_detail(compact: dict[str, Any] | None, dw_row: dict[str, Any] | None, query: dict[str, Any]) -> dict[str, Any] | None:
    if compact is None or dw_row is None: return None
    if query.get("kind") == "fact":
        return None if compact.get("content") == dw_row.get("value") else {"kind": "fact-value", "compact": compact.get("content"), "dw": dw_row.get("value")}
    if query.get("kind") == "relation":
        return None if compact.get("right") == dw_row.get("target") else {"kind": "relation-target", "compact": compact.get("right"), "dw": dw_row.get("target")}
    fail("invalid-contradiction-query")


def query_authority(source: dict[str, Any], query: dict[str, Any]) -> dict[str, Any] | None:
    if query.get("kind") == "fact":
        rows = [{"subject": row.get("id"), "field": query.get("field"), "value": row.get("fields", {}).get(query.get("field"))} for row in source.get("records", []) if row.get("id") == query.get("subject") and query.get("field") in row.get("fields", {})]
    elif query.get("kind") == "relation":
        rows = [{"type": row.get("type"), "source": row.get("source"), "target": row.get("target")} for row in source.get("relations", []) if row.get("type") == query.get("type") and row.get("source") == query.get("source")]
    else:
        fail("invalid-authority-query")
    if len(rows) > 1: fail(f"ambiguous-authority-query matches={len(rows)}")
    return rows[0] if rows else None


def omit_query_from_compact_projection(projection: dict[str, Any], query: dict[str, Any]) -> dict[str, Any]:
    compact = copy.deepcopy(projection)
    if query.get("kind") == "fact":
        compact["facts"] = [row for row in compact.get("facts", []) if not (row.get("subject") == query.get("subject") and row.get("field") == query.get("field"))]
    elif query.get("kind") == "relation":
        compact["relations"] = [row for row in compact.get("relations", []) if not (row.get("type") == query.get("type") and row.get("source") == query.get("source"))]
    else:
        fail("invalid-negative-query")
    return compact


def evaluate_negative_claim(source: dict[str, Any], projection: dict[str, Any], claim: dict[str, Any]) -> dict[str, Any]:
    query = claim.get("query")
    if not isinstance(query, dict): fail("negative-claim-missing-query")
    compact = omit_query_from_compact_projection(projection, query)
    if query_projection(compact, query) is not None: fail("negative-claim-compact-view-not-absent")
    authority_result = query_authority(source, query)
    if authority_result is None:
        return {"negative_claim_status": "CONFIRMED_BY_AUTHORITY", "claim_closed": True, "authority_counterexample": None}
    return {"negative_claim_status": "REFUTED_BY_AUTHORITY", "claim_closed": False, "authority_counterexample": authority_result}


def context_measure(query_result: dict[str, Any], source_bytes: bytes) -> dict[str, Any]:
    selective = len(json.dumps(query_result, sort_keys=True, separators=(",", ":")).encode("utf-8"))
    return {"selective_initial_bytes": selective, "baseline_initial_bytes": len(source_bytes), "context_reduced": selective < len(source_bytes)}


def classify_routing_signal(signal: dict[str, Any], mode: str = "structural") -> str:
    semantic = signal.get("semantic", {})
    decision_critical = semantic.get("decision_critical") is True
    structured_lookup = semantic.get("structured_lookup") is True
    structural = "USE" if decision_critical and structured_lookup else "OPTIONAL" if decision_critical or structured_lookup else "NOT_MATERIAL"
    if mode == "structural": return structural
    surface = signal.get("surface", {})
    if mode == "domain-label-bug": return "USE" if surface.get("domain_label") == "CITY" else structural
    if mode == "text-presence-bug": return "USE" if bool(surface.get("free_text")) else structural
    fail(f"unknown-routing-policy {mode}")


def route(case: dict[str, Any], source: dict[str, Any], source_bytes: bytes, stored_projection: dict[str, Any], projection_override: dict[str, Any] | None = None, routing_policy: str = "structural") -> dict[str, Any]:
    ctx = case.get("ctx", {})
    opened = unique(list(ctx.get("compact_visible_sources", [])))
    active = ctx.get("active_workpack")
    if active: opened = unique(opened + accepted_ctx_dependencies(ROOT, active))
    dw = case.get("dw", {})
    signal = case.get("routing_signal")
    advice = classify_routing_signal(signal, routing_policy) if isinstance(signal, dict) else dw.get("advice")
    dw_status, dw_loaded, dw_admitted = advice, False, False
    lifecycle = query_result = measurement = contradiction = None
    negative_result = {"negative_claim_status": None, "claim_closed": None, "authority_counterexample": None}
    projection = projection_override if projection_override is not None else mutate_projection(stored_projection, dw.get("projection_mutation"))

    claim = case.get("claim")
    if isinstance(claim, dict) and claim.get("negative") is True:
        negative_result = evaluate_negative_claim(source, projection, claim)
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
                    opened = unique(opened + [query_result["provenance"]["source_path"]])
                    compact_cfg = ctx.get("compare_with_compact")
                    if isinstance(compact_cfg, dict):
                        compact_surface = mutate_compact_ctx(build_compact_ctx_surface(source), compact_cfg.get("mutation"))
                        compact_query = compact_cfg.get("query", query)
                        contradiction = contradiction_detail(query_compact_ctx(compact_surface, compact_query), query_result, compact_query)
                    if contradiction is not None:
                        dw_status, dw_admitted = "CONTRADICTION_SOURCE_OPEN", False
                        opened = unique(opened + [SOURCE_REL])
                    else:
                        dw_status, dw_admitted = "USE", True
            else:
                dw_status = "USE_REJECTED_QUERY_MISS"
        else:
            dw_status = "USE_REJECTED_LIFECYCLE"
    elif advice == "OPTIONAL": dw_status = "OPTIONAL"
    elif advice == "NOT_MATERIAL": dw_status = "NOT_MATERIAL"
    else: fail(f"{case['id']} invalid-dw-advice {advice!r}")

    discovered_blockers, discoverability_missing = [], []
    if active: discoverability_missing, discovered_blockers = discoverability_audit(ROOT, active, opened)
    return {
        "dw_status": dw_status, "dw_loaded": dw_loaded, "dw_admitted": dw_admitted, "materiality_advice": advice,
        "opened_sources": sorted(opened), "discovered_blockers": discovered_blockers, "discoverability_missing": discoverability_missing,
        "lifecycle": lifecycle, "query_result": query_result, "measurement": measurement,
        "contradiction_detected": contradiction is not None, "contradiction_detail": contradiction, **negative_result,
    }


def signature(result: dict[str, Any]) -> dict[str, Any]:
    return {key: result[key] for key in ("dw_status", "dw_loaded", "dw_admitted", "materiality_advice", "opened_sources", "discovered_blockers", "contradiction_detected", "negative_claim_status", "claim_closed")}


def assert_expected(case: dict[str, Any], result: dict[str, Any], expected_key: str = "expected") -> None:
    expected = case[expected_key]
    for key in ("dw_status", "dw_loaded", "dw_admitted", "contradiction_detected", "negative_claim_status", "claim_closed", "materiality_advice"):
        if key in expected and result[key] != expected[key]: fail(f"{case['id']} {key} expected={expected[key]!r} observed={result[key]!r}")
    if sorted(expected.get("must_open", [])) != result["opened_sources"]: fail(f"{case['id']} opened expected={sorted(expected.get('must_open', []))} observed={result['opened_sources']}")
    if sorted(expected.get("must_discover_blockers", [])) != result["discovered_blockers"]: fail(f"{case['id']} blockers expected={sorted(expected.get('must_discover_blockers', []))} observed={result['discovered_blockers']}")
    if result["discoverability_missing"]: fail(f"{case['id']} discoverability-regression missing={result['discoverability_missing']}")


def validate_repo_preconditions(root: Path) -> None:
    checks = ((root / "Docs/workpacks/CTX/WP-CTX-03.md", "Status:", "COMPLETE"), (root / "Docs/workpacks/DW/WP-DW-GATE.md", "Status:", "COMPLETE"), (root / "Docs/evidence/CTX-03/DOCSYNC.md", "DOCSYNC_STATUS:", "DOCSYNC_COMPLETE"), (root / "Docs/evidence/WP-DW-GATE/DOCSYNC.md", "DOCSYNC_STATUS:", "DOCSYNC_COMPLETE"))
    for path, marker, accepted in checks:
        if not path.is_file(): fail(f"missing-predecessor {path.relative_to(root)}")
        text = path.read_text(encoding="utf-8")
        if marker not in text or accepted not in text: fail(f"predecessor-not-accepted {path.relative_to(root)}")
    if not SOURCE_PATH.is_file() or not PROJECTION_PATH.is_file(): fail("missing-h1-projection-probe-artifact")


def validate_suite(suite: dict[str, Any]) -> None:
    if suite.get("schema") != "ctx-dw-gate-falsification-v3": fail("wrong-suite-schema")
    cases = suite.get("cases")
    if not isinstance(cases, list) or len(cases) != 10: fail(f"expected-exactly-10-cases observed={len(cases) if isinstance(cases, list) else 'invalid'}")
    ids = [case.get("id") for case in cases]
    if len(ids) != len(set(ids)) or any(not isinstance(x, str) or not x for x in ids): fail("invalid-or-duplicate-case-id")
    required = {"hidden-materiality", "ctx-dw-contradiction", "stale-dw", "dw-abstention", "material-dw", "negative-claim", "domain-leakage-differential", "false-positive", "h1-projection-bootstrap", "routing-authority-conflict"}
    kinds = {case.get("kind") for case in cases}
    if kinds != required: fail(f"causal-class-mismatch missing={sorted(required-kinds)} extra={sorted(kinds-required)}")
    forbidden = {"authority_records", "baseline_material_blockers", "initial_context_units", "contradiction", "diagnostic_text", "alpha_renamed_diagnostic_text", "opaque_text"}
    for case in cases:
        bad = forbidden.intersection(case)
        if bad: fail(f"{case['id']} forbidden-answer-or-decorative-fields={sorted(bad)}")
        if isinstance(case.get("dw"), dict) and "lifecycle" in case["dw"]: fail(f"{case['id']} fixture-cannot-declare-lifecycle")
        if "routing_signal" in case and case.get("dw", {}).get("advice") is not None: fail(f"{case['id']} routing-signal-cannot-predeclare-advice")


def run_adversarial_controls(source: dict[str, Any], source_bytes: bytes, stored_projection: dict[str, Any]) -> dict[str, bool]:
    controls: dict[str, bool] = {}
    controls["baseline_lifecycle_green"] = lifecycle_green(lifecycle_state(source, source_bytes, stored_projection))
    controls["empty_dw_red"] = not lifecycle_green(lifecycle_state(source, source_bytes, mutate_projection(stored_projection, "empty")))
    controls["relation_corruption_red"] = not lifecycle_green(lifecycle_state(source, source_bytes, mutate_projection(stored_projection, "corrupt-relation")))
    controls["provenance_corruption_red"] = not lifecycle_green(lifecycle_state(source, source_bytes, mutate_projection(stored_projection, "corrupt-provenance")))
    controls["stale_fingerprint_red"] = not lifecycle_green(lifecycle_state(source, source_bytes, mutate_projection(stored_projection, "stale-source-fingerprint")))
    source_mutated = copy.deepcopy(source); source_mutated["records"][0]["fields"]["logical_id"] = "mutated.authority.identity"
    controls["authority_mutation_red"] = not lifecycle_green(lifecycle_state(source_mutated, json.dumps(source_mutated, indent=2).encode("utf-8"), stored_projection))
    controls["deterministic_rebuild"] = normalized_projection(build_projection(source, source_bytes)) == normalized_projection(build_projection(source, source_bytes))
    h104 = ROOT / resolve_workpack(ROOT, "WP-H1-04"); authority_text = h104.read_text(encoding="utf-8")
    controls["authority_status_is_causal"] = not status_is_accepted(authority_text) and status_is_accepted(STATUS_RE.sub("Status: COMPLETE", authority_text, count=1))
    baseline_deps = independent_baseline_dependencies(ROOT, "WP-H1-05")
    controls["suppressed_ctx_dependency_red"] = bool(set(baseline_deps) - {"Docs/workpacks/H1/WP-H1-05.md"})

    relation_query = {"kind": "relation", "type": "derived-from", "source": "asset:market-managed-variant"}
    relation_probe = {"id": "control-equivalent-contradiction", "ctx": {"compact_visible_sources": ["Docs/workpacks/H1/WP-H1-05.md"], "compare_with_compact": {"query": relation_query, "mutation": {"op": "replace-relation-target", "query": relation_query, "target": "catalogue:mutated-target"}}}, "dw": {"advice": "USE", "query": relation_query}}
    relation_result = route(relation_probe, source, source_bytes, stored_projection)
    controls["equivalent_contradiction_detected"] = relation_result["contradiction_detected"] and relation_result["dw_status"] == "CONTRADICTION_SOURCE_OPEN" and not relation_result["dw_admitted"]

    domain_signal = {"semantic": {"decision_critical": True, "structured_lookup": False}, "surface": {"domain_label": "CITY", "free_text": "allocated required-role"}}
    renamed_signal = copy.deepcopy(domain_signal); renamed_signal["surface"] = {"domain_label": "OMEGA", "free_text": "frobnicated role-x"}
    controls["alpha_rename_structural_invariance"] = classify_routing_signal(domain_signal) == classify_routing_signal(renamed_signal)
    controls["deliberate_domain_binding_red"] = classify_routing_signal(domain_signal, "domain-label-bug") != classify_routing_signal(renamed_signal, "domain-label-bug")

    noisy_signal = {"semantic": {"decision_critical": False, "structured_lookup": False}, "surface": {"domain_label": "opaque", "free_text": "CITY PA Quaternius finding disposition required-role allocated prefab"}}
    quiet_signal = copy.deepcopy(noisy_signal); quiet_signal["surface"]["free_text"] = ""
    controls["opaque_text_structurally_ignored"] = classify_routing_signal(noisy_signal) == classify_routing_signal(quiet_signal) == "NOT_MATERIAL"
    controls["deliberate_text_dependency_red"] = classify_routing_signal(noisy_signal, "text-presence-bug") != classify_routing_signal(quiet_signal, "text-presence-bug")
    if not all(controls.values()): fail(f"adversarial-control-failed {controls}")
    return controls


def run_suite(root: Path, suite_path: Path) -> dict[str, Any]:
    suite = load_json(suite_path); validate_suite(suite); validate_repo_preconditions(root)
    source_bytes = SOURCE_PATH.read_bytes(); source = json.loads(source_bytes.decode("utf-8")); projection = load_json(PROJECTION_PATH)
    controls = run_adversarial_controls(source, source_bytes, projection)
    results, material_utility, abstentions, negative_variants = [], 0, 0, 0

    for case in suite["cases"]:
        if case["kind"] == "material-dw":
            variants = case.get("material_variants", [])
            if len(variants) < 2: fail(f"{case['id']} requires-two-material-variants")
            variant_shapes: set[str] = set()
            for variant in variants:
                probe = copy.deepcopy(case); probe["id"] = f"{case['id']}::{variant['id']}"; probe["dw"]["query"] = variant["query"]
                observed = route(probe, source, source_bytes, projection); assert_expected(probe, observed)
                if not observed["measurement"] or not observed["measurement"]["context_reduced"]: fail(f"{probe['id']} no-derived-context-reduction measurement={observed['measurement']}")
                if not observed["query_result"]: fail(f"{probe['id']} dw-query-did-not-return-record")
                variant_shapes.add(variant["query"]["kind"]); material_utility += 1
            if variant_shapes != {"fact", "relation"}: fail(f"{case['id']} material-variants-not-distinct shapes={sorted(variant_shapes)}")
            base_probe = copy.deepcopy(case); base_probe["dw"]["query"] = variants[0]["query"]
            result = route(base_probe, source, source_bytes, projection); assert_expected(base_probe, result)
        elif case["kind"] == "negative-claim":
            variants = case.get("negative_variants", [])
            if len(variants) != 2: fail(f"{case['id']} requires-false-and-true-negative-variants")
            observed_statuses: set[str] = set(); result = None
            for variant in variants:
                probe = copy.deepcopy(case); probe["id"] = f"{case['id']}::{variant['id']}"; probe["claim"] = variant["claim"]; probe["expected"] = variant["expected"]
                observed = route(probe, source, source_bytes, projection); assert_expected(probe, observed); observed_statuses.add(observed["negative_claim_status"]); negative_variants += 1
                if variant.get("expect_counterexample") is True and not observed["authority_counterexample"]: fail(f"{probe['id']} authority-counterexample-missing")
                if variant.get("expect_counterexample") is False and observed["authority_counterexample"] is not None: fail(f"{probe['id']} unexpected-authority-counterexample")
                if result is None: result = observed
            if observed_statuses != {"REFUTED_BY_AUTHORITY", "CONFIRMED_BY_AUTHORITY"}: fail(f"{case['id']} negative-worlds-not-distinguished {sorted(observed_statuses)}")
            assert result is not None
        else:
            result = route(case, source, source_bytes, projection); assert_expected(case, result)

        if case["kind"] == "ctx-dw-contradiction":
            if not result["contradiction_detected"] or result["dw_admitted"]: fail(f"{case['id']} real-contradiction-not-fail-closed")
            clean = copy.deepcopy(case); clean["ctx"]["compare_with_compact"].pop("mutation", None)
            clean_result = route(clean, source, source_bytes, projection); assert_expected(clean, clean_result, expected_key="without_contradiction_expected")
            if clean_result["contradiction_detected"] or clean_result["dw_status"] == "CONTRADICTION_SOURCE_OPEN": fail(f"{case['id']} contradiction-route-survived-after-disagreement-removed")
        if case["kind"] == "dw-abstention":
            if result["dw_loaded"] or result["dw_status"] != "NOT_MATERIAL": fail(f"{case['id']} abstention-failed")
            abstentions += 1
        if case["kind"] == "domain-leakage-differential":
            renamed = copy.deepcopy(case); renamed["routing_signal"] = case["alpha_renamed_signal"]
            renamed_result = route(renamed, source, source_bytes, projection)
            if signature(result) != signature(renamed_result): fail(f"{case['id']} vocabulary-sensitive-routing")
            if signature(route(case, source, source_bytes, projection, routing_policy="domain-label-bug")) == signature(route(renamed, source, source_bytes, projection, routing_policy="domain-label-bug")): fail(f"{case['id']} deliberate-domain-binding-control-did-not-turn-red")
        if case["kind"] == "false-positive":
            if result["dw_loaded"] or result["dw_status"] != "NOT_MATERIAL": fail(f"{case['id']} opaque-text-manufactured-materiality")
            quiet = copy.deepcopy(case); quiet["routing_signal"]["surface"]["free_text"] = ""
            quiet_result = route(quiet, source, source_bytes, projection)
            if signature(result) != signature(quiet_result): fail(f"{case['id']} opaque-surface-changed-structural-route")
            if signature(route(case, source, source_bytes, projection, routing_policy="text-presence-bug")) == signature(route(quiet, source, source_bytes, projection, routing_policy="text-presence-bug")): fail(f"{case['id']} deliberate-text-dependency-control-did-not-turn-red")
        if case["kind"] == "h1-projection-bootstrap":
            if result["dw_admitted"] or result["dw_status"] != "USE_REJECTED_LIFECYCLE": fail(f"{case['id']} premature-h1-use")
            rebuilt = build_projection(source, source_bytes); after = copy.deepcopy(case); after["dw"].pop("projection_mutation", None)
            after_result = route(after, source, source_bytes, projection, projection_override=rebuilt); assert_expected(after, after_result, expected_key="after_rebuild_expected")
            if not after_result["dw_admitted"] or not lifecycle_green(after_result["lifecycle"] or {}): fail(f"{case['id']} valid-rebuild-not-eligible")
            h104 = ROOT / resolve_workpack(ROOT, "WP-H1-04"); before_hash = sha256_bytes(h104.read_bytes()); after_hash = sha256_bytes(h104.read_bytes())
            if before_hash != after_hash: fail(f"{case['id']} h1-product-authority-mutated")

        results.append({"id": case["id"], "kind": case["kind"], **signature(result), "lifecycle": result["lifecycle"], "measurement": result["measurement"], "contradiction_detail": result["contradiction_detail"], "authority_counterexample": result["authority_counterexample"]})

    if material_utility < 2: fail(f"selective-utility-insufficient variants={material_utility}")
    if abstentions < 1: fail("missing-abstention-control")
    if negative_variants != 2: fail(f"negative-claim-variant-count expected=2 observed={negative_variants}")
    normalized = json.dumps({"results": results, "controls": controls}, sort_keys=True, separators=(",", ":")).encode("utf-8")
    digest = hashlib.sha256(normalized).hexdigest()
    if digest != hashlib.sha256(normalized).hexdigest(): fail("deterministic-replay-mismatch")
    return {"schema": "ctx-dw-gate-result-v3", "case_count": len(results), "material_utility_variants": material_utility, "negative_claim_variants": negative_variants, "abstention_controls": abstentions, "discoverability_regressions": 0, "adversarial_controls": controls, "source_sha256": sha256_bytes(source_bytes), "projection_facts": len(projection.get("facts", [])), "projection_relations": len(projection.get("relations", [])), "result_digest": digest, "results": results}


def main() -> None:
    parser = argparse.ArgumentParser(); parser.add_argument("--suite", type=Path, default=DEFAULT_SUITE); parser.add_argument("--json", action="store_true"); args = parser.parse_args()
    result = run_suite(ROOT, args.suite)
    if args.json: print(json.dumps(result, indent=2, sort_keys=True))
    else: print("CTX_DW_GATE_GREEN " f"cases={result['case_count']} " f"material_variants={result['material_utility_variants']} " f"negative_variants={result['negative_claim_variants']} " f"abstentions={result['abstention_controls']} " f"facts={result['projection_facts']} " f"relations={result['projection_relations']} " f"discoverability_regressions={result['discoverability_regressions']} " f"digest={result['result_digest']}")


if __name__ == "__main__":
    main()
