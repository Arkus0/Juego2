#!/usr/bin/env python3
"""Worker-recovery regressions across the effective remote shim + base."""

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


legacy = _load("arkus_local_wp_worker_recovery_regressions", HERE / "_test_local_wp_worker_recovery_core.py")
RecoveryTests = legacy.RecoveryTests


def test_remote_adapter_wires_resume_snapshot_and_checkpoint_contract(self):
    adapter = (HERE / "local_wp_autopilot_remote.py").read_text(encoding="utf-8")
    adapter += "\n" + (HERE / "local_wp_autopilot_remote_base.py").read_text(encoding="utf-8")
    self.assertIn("autopilot.main_async = remote_main_async", adapter)
    self.assertIn("recovery.is_resumable_worker_pr", adapter)
    self.assertIn("recovery.inspect_resumable_checkout", adapter)
    self.assertIn("recovery.snapshot_worktree", adapter)
    self.assertIn("recovery.CHECKPOINT_GUIDANCE", adapter)
    self.assertIn("recovery.bootstrap_prompt", adapter)
    self.assertIn('"gpt-6-luna", "high"', adapter)
    self.assertIn("Interrupted Worker recovery did not preserve canonical PR", adapter)


RecoveryTests.test_remote_adapter_wires_resume_snapshot_and_checkpoint_contract = \
    test_remote_adapter_wires_resume_snapshot_and_checkpoint_contract


if __name__ == "__main__":
    unittest.main(module=legacy)
