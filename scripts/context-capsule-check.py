#!/usr/bin/env python3
"""Accepted-contract capsule validator entrypoint.

Keeps the reviewed CTX-02 validator implementation byte-identical while extending
its checker-owned PA disposition-selector policy without requiring one code edit
per future PA workpack.
"""
from __future__ import annotations

import importlib.util
from pathlib import Path
import re

_IMPL_PATH = Path(__file__).with_name("_context_capsule_check_impl.py")
_SPEC = importlib.util.spec_from_file_location("_context_capsule_check_impl", _IMPL_PATH)
if _SPEC is None or _SPEC.loader is None:
    raise RuntimeError(f"could not load capsule checker implementation: {_IMPL_PATH}")
_IMPL = importlib.util.module_from_spec(_SPEC)
_SPEC.loader.exec_module(_IMPL)

# PA-01..04 remain byte-frozen in the underlying reviewed checker. PA-05 keeps
# its already-reviewed exceptional selector. PA-06+ use one checker-owned
# convention: the authoritative accepted PA result must expose this exact section
# and columns. The capsule/index still cannot choose its own completeness oracle.
_STANDARD_PA_SELECTOR = {
    "section": "## Canonical disposition table",
    "key_column": 0,
    "status_column": 1,
}


class _CanonicalPaSelectors(dict):
    def get(self, key, default=None):
        value = super().get(key)
        if value is not None:
            return value
        match = re.fullmatch(r"WP-PA-(\d+)", str(key))
        if match and int(match.group(1)) >= 6:
            return dict(_STANDARD_PA_SELECTOR)
        return default


_selectors = _CanonicalPaSelectors(_IMPL.CANONICAL_PA_DISPOSITION_SELECTORS)
_selectors["WP-PA-05"] = {
    "section": "## 6. Mechanism dispositions",
    "key_column": 0,
    "status_column": 1,
}
_IMPL.CANONICAL_PA_DISPOSITION_SELECTORS = _selectors

for _name in dir(_IMPL):
    if not _name.startswith("__"):
        globals()[_name] = getattr(_IMPL, _name)

if __name__ == "__main__":
    raise SystemExit(_IMPL.main())
