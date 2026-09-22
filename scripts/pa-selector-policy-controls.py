#!/usr/bin/env python3
"""Causal controls for checker-owned PA-06+ selector convention."""
from __future__ import annotations

import importlib.util
from pathlib import Path
import sys

ROOT = Path(__file__).resolve().parents[1]
CHECKER = ROOT / "scripts/context-capsule-check.py"
SPEC = importlib.util.spec_from_file_location("context_capsule_check", CHECKER)
if SPEC is None or SPEC.loader is None:
    raise RuntimeError(f"cannot import {CHECKER}")
mod = importlib.util.module_from_spec(SPEC)
SPEC.loader.exec_module(mod)

selectors = mod.CANONICAL_PA_DISPOSITION_SELECTORS
expected = {
    "section": "## Canonical disposition table",
    "key_column": 0,
    "status_column": 1,
}

errors: list[str] = []
for wp in ("WP-PA-06", "WP-PA-07", "WP-PA-99"):
    got = selectors.get(wp)
    if got != expected:
        errors.append(f"{wp}: standard selector mismatch: {got!r}")

if selectors.get("WP-PA-05") != {
    "section": "## 6. Mechanism dispositions",
    "key_column": 0,
    "status_column": 1,
}:
    errors.append("WP-PA-05 reviewed exceptional selector changed")

for bad in ("WP-PA-5", "WP-PA-X", "WP-DW-06", "WP-PA-005"):
    if selectors.get(bad) is not None:
        errors.append(f"non-canonical future PA id received fallback selector: {bad}")

first = selectors.get("WP-PA-06")
if first is not None:
    first["section"] = "CORRUPTED"
if selectors.get("WP-PA-06") != expected:
    errors.append("future PA selector is mutable/shared across calls")

if errors:
    for error in errors:
        print(f"PA_SELECTOR_ERROR: {error}", file=sys.stderr)
    raise SystemExit(1)

print("PA_SELECTOR_POLICY_GREEN legacy=PA-01..05 future=PA-06+")
