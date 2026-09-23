#!/usr/bin/env python3
import argparse
import hashlib
import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
DW = ROOT / "Docs" / "workpacks" / "DW"
EVIDENCE = ROOT / "Docs" / "evidence"
GATE = EVIDENCE / "WP-DW-GATE"

def fail(message: str) -> None:
    raise SystemExit(f"DW_GATE_EVIDENCE_RED {message}")

def load_json(path: Path):
    if not path.is_file():
        fail(f"missing-json {path.relative_to(ROOT)}")
    try:
        return json.loads(path.read_text(encoding="utf-8"))
    except Exception as exc:
        fail(f"invalid-json {path.relative_to(ROOT)} {exc}")

def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--digest-only", action="store_true")
    args = parser.parse_args()

    # Accepted predecessor contracts remain the stage authority.
    for n in range(6):
        wp = DW / f"WP-DW-0{n}.md"
        if not wp.is_file():
            fail(f"missing-workpack {wp.relative_to(ROOT)}")
        text = wp.read_text(encoding="utf-8")
        if not re.search(r"^Status:\s*.*(?:COMPLETE|ACCEPTED)", text, flags=re.MULTILINE):
            fail(f"predecessor-not-accepted WP-DW-0{n}")
        evidence_dir = EVIDENCE / f"WP-DW-0{n}"
        if not evidence_dir.is_dir():
            fail(f"missing-evidence-dir WP-DW-0{n}")

    residual_path = EVIDENCE / "WP-DW-05" / "PREDECESSOR_RESIDUAL_INVENTORY.json"
    limitations_path = EVIDENCE / "WP-DW-05" / "LIMITATIONS_MANIFEST.json"
    dw05_handoff_path = EVIDENCE / "WP-DW-05" / "H2_BOUNDARY_INPUT_V1.json"
    dw04_result_path = EVIDENCE / "WP-DW-04" / "TRIAL_RESULT.json"
    handoff_path = GATE / "H2_HANDOFF_V1.json"
    proof_path = GATE / "PROOF_MATRIX.md"

    residual = load_json(residual_path)
    limitations = load_json(limitations_path)
    dw05_handoff = load_json(dw05_handoff_path)
    dw04_result = load_json(dw04_result_path)
    handoff = load_json(handoff_path)

    clauses = residual.get("clauses")
    if not isinstance(clauses, list) or not clauses:
        fail("empty-predecessor-residual-inventory")
    clause_keys = []
    predecessor_ids = set()
    for clause in clauses:
        try:
            source = clause["source"]
            anchor = clause["anchor"]
            limitation_id = clause["limitation_id"]
        except Exception:
            fail("malformed-predecessor-residual-clause")
        key = (source, anchor, limitation_id)
        if key in clause_keys:
            fail(f"duplicate-predecessor-residual {limitation_id}")
        clause_keys.append(key)
        predecessor_ids.add(limitation_id)
        source_path = ROOT / source
        if not source_path.is_file():
            fail(f"missing-residual-source {source}")
        if anchor not in source_path.read_text(encoding="utf-8"):
            fail(f"residual-anchor-missing {source}::{limitation_id}")

    limitation_rows = limitations.get("limitations")
    if not isinstance(limitation_rows, list) or not limitation_rows:
        fail("empty-limitations-manifest")
    limitation_ids = [row.get("id") for row in limitation_rows]
    if any(not isinstance(x, str) or not x for x in limitation_ids):
        fail("malformed-limitation-id")
    if len(limitation_ids) != len(set(limitation_ids)):
        fail("duplicate-limitation-id")
    missing_from_classification = sorted(predecessor_ids - set(limitation_ids))
    if missing_from_classification:
        fail(f"predecessor-limitations-unclassified {missing_from_classification}")

    if handoff.get("limitations_manifest") != "Docs/evidence/WP-DW-05/LIMITATIONS_MANIFEST.json":
        fail("h2-handoff-limitations-source-mismatch")
    if handoff.get("limitations_reference_mode") != "WHOLE_FILE_AUTHORITATIVE_REFERENCE":
        fail("h2-handoff-limitations-reference-not-whole-file")
    if handoff.get("limitations_count") != len(limitation_ids):
        fail(f"h2-handoff-limitation-reconciliation-mismatch expected={len(limitation_ids)} observed={handoff.get('limitations_count')}")
    if handoff.get("predecessor_residual_inventory") != "Docs/evidence/WP-DW-05/PREDECESSOR_RESIDUAL_INVENTORY.json":
        fail("h2-handoff-predecessor-residual-source-mismatch")
    if handoff.get("predecessor_residual_clause_count") != len(clauses):
        fail(f"h2-handoff-residual-reconciliation-mismatch expected={len(clauses)} observed={handoff.get('predecessor_residual_clause_count')}")

    claims = handoff.get("claims", {})
    required_false = (
        "arbitrary_domain_universality",
        "external_repository_consumability",
        "general_dw_adoption_authorized",
        "h2_scope_binding",
    )
    for key in required_false:
        if claims.get(key) is not False:
            fail(f"h2-handoff-overclaim {key}")

    for key in ("demonstrated_public_capabilities", "required_h2_boundary_considerations", "optional_downstream_opportunities", "explicit_non_claims"):
        value = handoff.get(key)
        if not isinstance(value, list) or not value:
            fail(f"h2-handoff-empty-section {key}")

    # Gate must preserve, not silently strengthen, the accepted DW-05 planning boundary.
    accepted_claims = dw05_handoff.get("claims", {})
    for key in required_false:
        if accepted_claims.get(key) is not False:
            fail(f"accepted-dw05-boundary-unexpected {key}")
    if not set(dw05_handoff.get("required_consequences", [])).issubset(set(handoff["required_h2_boundary_considerations"])):
        fail("h2-handoff-dropped-required-dw05-consequence")
    if not set(dw05_handoff.get("optional_opportunities", [])).issubset(set(handoff["optional_downstream_opportunities"])):
        fail("h2-handoff-dropped-optional-dw05-opportunity")

    if dw04_result.get("disposition") != "PASS" or dw04_result.get("structural") is not True:
        fail("dw04-accepted-result-not-pass")
    if float(dw04_result.get("saving", -1)) < 0.30:
        fail("dw04-accepted-context-reduction-below-threshold")
    if int(dw04_result.get("provider_request_count", -1)) != 36:
        fail("dw04-accepted-paired-run-count-mismatch")

    roadmap = (ROOT / "Docs" / "ROADMAP.md").read_text(encoding="utf-8")
    if "final H2 public/external-boundary acceptance must consume accepted `DW-GATE` evidence" not in roadmap:
        fail("roadmap-dw-h2-interlock-missing")
    if "WP-DW-GATE` | composed second-consumer readiness and explicit H2 planning consequence" not in roadmap:
        fail("roadmap-dw-gate-sequence-missing")

    proof = proof_path.read_text(encoding="utf-8") if proof_path.is_file() else ""
    for required in (
        "FOUNDATIONAL_PROOF_VERDICT: READY",
        "UNRESOLVED_PROOF_OBLIGATIONS: 0",
        "KNOWN_UNDETECTED_DEFECT_CLASSES: 0",
        "PREDECESSOR_RESIDUAL_RECONCILIATION: COMPLETE",
        "H2_HANDOFF_RECONCILIATION: COMPLETE",
    ):
        if required not in proof:
            fail(f"proof-matrix-marker-missing {required}")

    digest_paths = [
        residual_path,
        limitations_path,
        dw05_handoff_path,
        dw04_result_path,
        handoff_path,
        proof_path,
        ROOT / "Docs" / "ROADMAP.md",
    ]
    h = hashlib.sha256()
    for path in digest_paths:
        rel = path.relative_to(ROOT).as_posix().encode("utf-8")
        data = path.read_bytes()
        h.update(len(rel).to_bytes(4, "big"))
        h.update(rel)
        h.update(len(data).to_bytes(8, "big"))
        h.update(data)
    digest = h.hexdigest()

    if args.digest_only:
        print(digest)
        return
    print(
        "DW_GATE_EVIDENCE_GREEN "
        f"predecessor_residual_clauses={len(clauses)} "
        f"limitations={len(limitation_ids)} "
        f"dw04_provider_requests={dw04_result['provider_request_count']} "
        f"digest={digest}"
    )

if __name__ == "__main__":
    main()
