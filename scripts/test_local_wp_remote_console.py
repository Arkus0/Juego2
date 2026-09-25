#!/usr/bin/env python3
"""Remote-console regressions across the effective shim + base implementation."""

from __future__ import annotations

import importlib.util
from pathlib import Path
import sys
import unittest

HERE = Path(__file__).resolve().parent


def _load(name: str, path: Path):
    spec = importlib.util.spec_from_file_location(name, path)
    if spec is None or spec.loader is None:
        raise RuntimeError(path)
    loaded = importlib.util.module_from_spec(spec)
    sys.modules[name] = loaded
    spec.loader.exec_module(loaded)
    return loaded


legacy = _load("arkus_local_wp_remote_console_regressions", HERE / "_test_local_wp_remote_console_core.py")
RemoteConsoleTests = legacy.RemoteConsoleTests


def test_owner_continue_marker_is_campaign_bound(self):
    # local_wp_autopilot_remote.py is an intentional shim over *_remote_base.py;
    # the invariant must be present in the effective implementation, not
    # necessarily duplicated textually in the shim.
    source = (HERE / "local_wp_autopilot_remote.py").read_text(encoding="utf-8")
    source += "\n" + (HERE / "local_wp_autopilot_remote_base.py").read_text(encoding="utf-8")
    self.assertIn('row.get("campaign id") == campaign', source)


RemoteConsoleTests.test_owner_continue_marker_is_campaign_bound = test_owner_continue_marker_is_campaign_bound


if __name__ == "__main__":
    unittest.main(module=legacy)
