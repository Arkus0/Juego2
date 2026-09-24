#!/usr/bin/env python3
"""Accepted-contract capsule validator entrypoint.

Keeps the reviewed CTX-02 validator implementation byte-identical while extending
its checker-owned PA disposition-selector registry for newly accepted PA work.
"""
from __future__ import annotations

import importlib.util
from pathlib import Path

_IMPL_PATH = Path(__file__).with_name("_context_capsule_check_impl.py")
_SPEC = importlib.util.spec_from_file_location("_context_capsule_check_impl", _IMPL_PATH)
if _SPEC is None or _SPEC.loader is None:
    raise RuntimeError(f"could not load capsule checker implementation: {_IMPL_PATH}")
_IMPL = importlib.util.module_from_spec(_SPEC)
_SPEC.loader.exec_module(_IMPL)

_IMPL.CANONICAL_PA_DISPOSITION_SELECTORS["WP-PA-05"] = {
    "section": "## 6. Mechanism dispositions",
    "key_column": 0,
    "status_column": 1,
}
_IMPL.CANONICAL_PA_DISPOSITION_SELECTORS["WP-PA-06"] = {
    "section": "## 5. Mechanism dispositions",
    "key_column": 0,
    "status_column": 1,
}

for _name in dir(_IMPL):
    if not _name.startswith("__"):
        globals()[_name] = getattr(_IMPL, _name)

if __name__ == "__main__":
    raise SystemExit(_IMPL.main())
