#!/usr/bin/env python3
"""Classify mechanical verifier results without turning protocol/infra red into WP FAIL."""
from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path

OUTCOMES = {"PASS", "FAIL", "REVIEW_BLOCKED", "NOT_APPLICABLE", "INFRA_ERROR"}


def load(path: Path):
    return json.loads(path.read_text(encoding="utf-8"))


def classify(registry: dict, state: dict) -> dict:
    by_name = {
        row["github_check_name"]: (key, row)
        for key, row in registry["registered_verifiers"].items()
    }
    rows = []
    affecting = []
    operational = []

    for check in state.get("checks", []):
        name = str(check.get("name") or "")
        conclusion = str(check.get("conclusion") or "").lower()
        reported = str(check.get("reported_outcome") or "").upper() or None
        registered = by_name.get(name)

        if registered is None:
            outcome = "INFRA_ERROR" if conclusion not in {"success", "neutral", "skipped"} else "NOT_APPLICABLE"
            rows.append({
                "name": name,
                "registered": False,
                "outcome": outcome,
                "affects_wp_decision": False,
                "counts_as_wp_fail": False,
                "detail": "unregistered verifier/check cannot establish a WP defect; a red unknown check blocks review operationally until triaged or explicitly registered"
            })
            if outcome == "INFRA_ERROR":
                operational.append(outcome)
            continue

        verifier_id, contract = registered
        requires_structured = bool(contract.get("requires_structured_outcome", False))
        if conclusion in {"queued", "in_progress", "pending", "", "cancelled"}:
            outcome = "REVIEW_BLOCKED"
        elif requires_structured:
            if reported not in OUTCOMES:
                outcome = "INFRA_ERROR"
            else:
                outcome = reported
                if conclusion == "success" and reported in {"FAIL", "REVIEW_BLOCKED", "INFRA_ERROR"}:
                    outcome = "INFRA_ERROR"
                if conclusion == "failure" and reported in {"PASS", "NOT_APPLICABLE"}:
                    outcome = "INFRA_ERROR"
        else:
            if conclusion in {"success", "neutral"}:
                outcome = reported if reported in {"PASS", "NOT_APPLICABLE"} else "PASS"
            elif conclusion == "skipped":
                outcome = "NOT_APPLICABLE"
            else:
                outcome = str(contract["failure_outcome"]).upper()

        if outcome not in OUTCOMES:
            raise ValueError(f"invalid outcome {outcome!r} for {verifier_id}")
        row = {
            "id": verifier_id,
            "name": name,
            "registered": True,
            "outcome": outcome,
            "affects_wp_decision": True,
            "counts_as_wp_fail": outcome == "FAIL",
        }
        rows.append(row)
        affecting.append(outcome)

    required = state.get("required_registered_verifiers") or []
    observed_ids = {r.get("id") for r in rows if r.get("registered")}
    for verifier_id in required:
        if verifier_id not in registry["registered_verifiers"]:
            rows.append({
                "id": verifier_id,
                "registered": False,
                "outcome": "INFRA_ERROR",
                "affects_wp_decision": False,
                "counts_as_wp_fail": False,
                "detail": "required verifier id is not registered; registry/contract error"
            })
            operational.append("INFRA_ERROR")
            continue
        if verifier_id not in observed_ids:
            rows.append({
                "id": verifier_id,
                "registered": True,
                "outcome": "REVIEW_BLOCKED",
                "affects_wp_decision": True,
                "counts_as_wp_fail": False,
                "detail": "required registered verifier has no completed observation"
            })
            affecting.append("REVIEW_BLOCKED")

    if "FAIL" in affecting:
        overall = "FAIL"
    elif "REVIEW_BLOCKED" in affecting:
        overall = "REVIEW_BLOCKED"
    elif "INFRA_ERROR" in affecting or operational:
        overall = "INFRA_ERROR"
    elif affecting and all(x == "NOT_APPLICABLE" for x in affecting):
        overall = "NOT_APPLICABLE"
    else:
        overall = "PASS"

    return {
        "schema": "arkus.mechanical-verifier-decision@1",
        "overall": overall,
        "wp_failed_mechanically": "FAIL" in affecting,
        "independent_review_authorized": overall in {"PASS", "NOT_APPLICABLE"},
        "semantic_review_still_required": True,
        "verifiers": rows,
    }


def self_test() -> None:
    reg = {
        "registered_verifiers": {
            "handoff": {"github_check_name": "Worker handoff lint", "failure_outcome": "REVIEW_BLOCKED"},
            "causal": {"github_check_name": "CTX process envelope", "failure_outcome": "FAIL", "requires_structured_outcome": True},
        }
    }
    s = {"checks": [
        {"name": "Worker handoff lint", "conclusion": "failure"},
        {"name": "CTX process envelope", "conclusion": "success", "reported_outcome": "PASS"},
    ], "required_registered_verifiers": ["handoff", "causal"]}
    assert classify(reg, s)["overall"] == "REVIEW_BLOCKED"

    s["checks"][0]["conclusion"] = "success"
    s["checks"][1] = {"name": "CTX process envelope", "conclusion": "failure", "reported_outcome": "FAIL"}
    assert classify(reg, s)["overall"] == "FAIL"

    # A red verifier that is not registered cannot become a WP failure, but it
    # also cannot be silently ignored to authorize review: it is an operational
    # INFRA_ERROR until triaged or explicitly registered.
    s = {"checks": [{"name": "Experimental verifier", "conclusion": "failure"}]}
    out = classify(reg, s)
    assert out["overall"] == "INFRA_ERROR" and not out["wp_failed_mechanically"]
    assert not out["independent_review_authorized"]
    assert out["verifiers"][0]["outcome"] == "INFRA_ERROR"

    # A registered causal verifier that crashes without its structured result is infra, not FAIL.
    s = {"checks": [{"name": "CTX process envelope", "conclusion": "failure"}], "required_registered_verifiers": ["causal"]}
    assert classify(reg, s)["overall"] == "INFRA_ERROR"

    # An unregistered required ID is a registry/infrastructure error, never a WP defect.
    s = {"checks": [], "required_registered_verifiers": ["not-registered"]}
    out = classify(reg, s)
    assert out["overall"] == "INFRA_ERROR" and not out["wp_failed_mechanically"]

    # N/A is neutral, never synthetic proof.
    s = {"checks": [{"name": "CTX process envelope", "conclusion": "skipped", "reported_outcome": "NOT_APPLICABLE"}], "required_registered_verifiers": ["causal"]}
    assert classify(reg, s)["overall"] == "NOT_APPLICABLE"

    print("mechanical-verifier-classifier self-test: PASS")


def main() -> int:
    p = argparse.ArgumentParser()
    p.add_argument("--registry", type=Path, default=Path("Docs/engineering/mechanical-verifier-registry.json"))
    p.add_argument("--state", type=Path)
    p.add_argument("--output", type=Path)
    p.add_argument("--self-test", action="store_true")
    args = p.parse_args()
    if args.self_test:
        self_test(); return 0
    if args.state is None:
        p.error("--state is required")
    try:
        result = classify(load(args.registry), load(args.state))
    except Exception as exc:
        print(f"MECHANICAL_VERIFIER_DECISION: INFRA_ERROR\n{exc}", file=sys.stderr)
        return 23
    text = json.dumps(result, indent=2, sort_keys=True) + "\n"
    if args.output:
        args.output.write_text(text, encoding="utf-8")
    print(text, end="")
    return {"PASS": 0, "NOT_APPLICABLE": 0, "FAIL": 20, "REVIEW_BLOCKED": 21, "INFRA_ERROR": 23}[result["overall"]]


if __name__ == "__main__":
    raise SystemExit(main())
