#!/usr/bin/env python3
"""Causal wiring controls for Batch B process workflows."""
from __future__ import annotations

from pathlib import Path
import sys

ROOT = Path(__file__).resolve().parents[1]
REUSE = ROOT / ".github/workflows/receipt-freeze-reuse.yml"
GATE = ROOT / ".github/workflows/process-integrity-shadow.yml"


def validate_reuse(text: str) -> list[str]:
    required = {
        "exact candidate checkout": "ref: ${{ steps.identity.outputs.sha }}\n          path: candidate",
        "canonical live handoff lint": "validate-worker-handoff.py \\\n            --head-sha \"${sha}\" --repo-root candidate",
        "complete reuse identity": "reuse-context.py resolve",
        "reuse digest read": "reuse_digest=\"$(jq -r '.reuse_context_digest' REUSE_CONTEXT.json)\"",
        "review-ready key bound to reuse digest": 'key="review-ready:${PR}:${SHA}:${REUSE_CONTEXT_DIGEST}"',
    }
    return [label for label, needle in required.items() if needle not in text]


def validate_gate(text: str) -> list[str]:
    errors: list[str] = []
    if "  pull_request:\n" not in text:
        errors.append("unfiltered pull_request trigger missing")
    if "paths:" in text or "paths-ignore:" in text:
        errors.append("stable process gate contains path filtering")
    if "name: Stable process merge gate" not in text:
        errors.append("stable merge-gate check name missing")
    if "scripts/reuse-context.py self-test" not in text:
        errors.append("reuse-context causal self-test not wired into stable gate")
    return errors


def self_test() -> None:
    reuse = REUSE.read_text(encoding="utf-8")
    gate = GATE.read_text(encoding="utf-8")
    if validate_reuse(reuse):
        raise AssertionError(f"live reuse workflow invalid before mutation controls: {validate_reuse(reuse)}")
    if validate_gate(gate):
        raise AssertionError(f"live gate invalid before mutation controls: {validate_gate(gate)}")

    mutations = [
        ("--repo-root candidate", "--repo-root process-oracle", validate_reuse),
        ("reuse-context.py resolve", "reuse-context.py disabled", validate_reuse),
        ('key="review-ready:${PR}:${SHA}:${REUSE_CONTEXT_DIGEST}"', 'key="review-ready:${PR}:${SHA}:${CONTEXT_DIGEST}"', validate_reuse),
    ]
    for old, new, checker in mutations:
        mutated = reuse.replace(old, new, 1)
        if not checker(mutated):
            raise AssertionError(f"reuse wiring mutation stayed GREEN: {old}")

    filtered = gate.replace("  pull_request:\n", "  pull_request:\n    paths:\n      - 'scripts/**'\n", 1)
    if not validate_gate(filtered):
        raise AssertionError("path-filtered stable gate stayed GREEN")

    print("BATCH_B_WORKFLOW_WIRING_SELF_TEST_GREEN")


def main() -> int:
    try:
        reuse_errors = validate_reuse(REUSE.read_text(encoding="utf-8"))
        gate_errors = validate_gate(GATE.read_text(encoding="utf-8"))
        if reuse_errors or gate_errors:
            for error in reuse_errors + gate_errors:
                print(f"BATCH_B_WIRING_ERROR: {error}", file=sys.stderr)
            return 1
        self_test()
        return 0
    except (OSError, AssertionError) as exc:
        print(f"BATCH_B_WIRING_ERROR: {exc}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
