#!/usr/bin/env python3
"""Causal controls for the PA-06+ checker-owned disposition convention."""
from __future__ import annotations

import importlib.util
from pathlib import Path

HERE = Path(__file__).resolve().parent
path = HERE / "context-capsule-check.py"
spec = importlib.util.spec_from_file_location("capsule_entry", path)
if spec is None or spec.loader is None:
    raise SystemExit("could not load capsule checker entrypoint")
mod = importlib.util.module_from_spec(spec)
spec.loader.exec_module(mod)

expected = {
    "section": "## Canonical disposition table",
    "key_column": 0,
    "status_column": 1,
}

assert mod.CANONICAL_PA_DISPOSITION_SELECTORS.get("WP-PA-05") == {
    "section": "## 6. Mechanism dispositions",
    "key_column": 0,
    "status_column": 1,
}
assert mod.CANONICAL_PA_DISPOSITION_SELECTORS.get("WP-PA-06") == expected
assert mod.CANONICAL_PA_DISPOSITION_SELECTORS.get("WP-PA-99") == expected
assert mod.CANONICAL_PA_DISPOSITION_SELECTORS.get("WP-DW-06") is None
assert mod.CANONICAL_PA_DISPOSITION_SELECTORS.get("WP-PA-05X") is None

# Mutation-style control: changing the convention must be observable here.
wrong = dict(expected)
wrong["status_column"] = 2
assert mod.CANONICAL_PA_DISPOSITION_SELECTORS.get("WP-PA-06") != wrong

print("PA_DISPOSITION_SELECTOR_CONTROLS_GREEN")
