#!/usr/bin/env python3
"""Reviewer-cycle-4 omission controls for Context Capsule v1.

These tests deliberately keep a one-of-many capsule structurally valid while
removing one material guarantee/exclusion from an actual indexed representative
capsule. The production checker is expected to remain GREEN on shape/source
integrity; the independent representative semantic oracle must RED. That split
proves CTX-02 detects silent semantic narrowing without pretending production
code can generically prove natural-language completeness.
"""
from __future__ import annotations

import copy
import importlib.util
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
CONTROLS_PATH = ROOT / "scripts" / "context-capsule-controls.py"
spec = importlib.util.spec_from_file_location("context_capsule_controls", CONTROLS_PATH)
if spec is None or spec.loader is None:
    raise RuntimeError("could not load context-capsule-controls.py")
controls = importlib.util.module_from_spec(spec)
spec.loader.exec_module(controls)
checker = controls.checker


def assert_oracle_red(capsule: dict, needle: str) -> None:
    try:
        controls.assert_representative_semantics(capsule)
    except checker.CapsuleError as exc:
        if needle not in str(exc):
            raise AssertionError(f"expected representative RED containing {needle!r}, got {exc!r}") from exc
    else:
        raise AssertionError(f"expected representative semantic oracle to RED with {needle!r}")


def omit_one(rows: list[dict], target_id: str) -> list[dict]:
    remaining = [row for row in rows if row.get("id") != target_id]
    if len(remaining) != len(rows) - 1 or not remaining:
        raise AssertionError(f"omission fixture did not remove exactly one of many rows: {target_id}")
    return remaining


def run() -> None:
    _, capsules = controls.load_indexed_capsules(ROOT)
    baseline = capsules.get("WP-HK-GATE")
    if baseline is None:
        raise checker.CapsuleError("omission control requires actual indexed WP-HK-GATE capsule")

    # Prove the real representative baseline is valid under both layers first.
    checker.validate_capsule(ROOT, baseline)
    controls.assert_representative_semantics(baseline)

    # Reviewer #5274389905 B1: remove one material positive while other positives
    # remain. Production shape/source checks intentionally stay GREEN; the same
    # independent oracle used by CTX-02's representative controls must RED.
    guarantee_missing = copy.deepcopy(baseline)
    guarantee_missing["exported_guarantees"] = omit_one(
        guarantee_missing["exported_guarantees"], "unity-work-authorized"
    )
    checker.validate_capsule(ROOT, guarantee_missing)
    assert_oracle_red(guarantee_missing, "exported_guarantees material content mismatch")

    # Symmetric first-class negative/non-claim omission.
    exclusion_missing = copy.deepcopy(baseline)
    exclusion_missing["exclusions_nonclaims"] = omit_one(
        exclusion_missing["exclusions_nonclaims"], "no-new-concurrency-claim"
    )
    checker.validate_capsule(ROOT, exclusion_missing)
    assert_oracle_red(exclusion_missing, "exclusions_nonclaims material content mismatch")

    print("context-capsule representative omission controls: PASS (defect injections RED)")


if __name__ == "__main__":
    run()
