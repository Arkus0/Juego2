#!/usr/bin/env python3
"""Regression tests for bounded soft owner-decision waits."""

from __future__ import annotations

import importlib.util
import json
import os
import sys
import tempfile
import unittest
from pathlib import Path
from unittest.mock import Mock, patch

HERE = Path(__file__).parent


def load(name: str, filename: str):
    spec = importlib.util.spec_from_file_location(name, HERE / filename)
    module = importlib.util.module_from_spec(spec)
    sys.modules[spec.name] = module
    spec.loader.exec_module(module)
    return module


load("owner_control_auth", "owner_control_auth.py")
decision = load("request_owner_decision_timeout_test", "request_owner_decision.py")


class OwnerDecisionTimeoutTests(unittest.TestCase):
    def _env(self, control: str) -> dict[str, str]:
        return {
            "ARKUS_REMOTE_CONTROL_DIR": control,
            "ARKUS_REMOTE_CAMPAIGN_ID": "a" * 32,
            "ARKUS_REMOTE_SUPERVISOR_URL": "http://127.0.0.1:12345",
            "ARKUS_REMOTE_ROLE": "worker",
        }

    def test_default_timeout_is_exactly_180_seconds(self):
        self.assertEqual(decision.OWNER_DECISION_TIMEOUT_SECONDS, 180.0)

    def test_no_answer_delegates_back_to_same_worker(self):
        with tempfile.TemporaryDirectory() as tmp:
            decision_id = "d" * 32
            with patch.dict(os.environ, self._env(tmp), clear=True), \
                 patch.object(decision.uuid, "uuid4", return_value=Mock(hex=decision_id)), \
                 patch.object(decision, "_verified_owner_response", return_value=None), \
                 patch.object(decision.time, "monotonic", side_effect=[1000.0, 1180.0]), \
                 patch.object(decision.time, "sleep", return_value=None):
                result = decision.request_decision(
                    "Choose", ["A", "B"], wp="H1-04", pr=123, sha="b" * 40,
                    detail="soft preference", poll_seconds=0)

            self.assertEqual(result, decision.OWNER_TIMEOUT_DELEGATION)
            row = json.loads((Path(tmp) / "decisions" / f"{decision_id}.json").read_text(encoding="utf-8"))
            self.assertEqual(row["owner_timeout_seconds"], 180)
            self.assertEqual(row["abandon_reason"], "owner-timeout-delegated-to-worker")
            self.assertEqual(row["delegation_instruction"], decision.OWNER_TIMEOUT_DELEGATION)
            self.assertIn("abandoned_at", row)
            self.assertNotIn("selected_index", row)

    def test_attested_owner_choice_wins_at_timeout_boundary(self):
        with tempfile.TemporaryDirectory() as tmp:
            with patch.dict(os.environ, self._env(tmp), clear=True), \
                 patch.object(decision.uuid, "uuid4", return_value=Mock(hex="e" * 32)), \
                 patch.object(decision, "_verified_owner_response",
                              return_value={"choice": 1, "selected": "B"}), \
                 patch.object(decision.time, "monotonic", side_effect=[1000.0]):
                result = decision.request_decision(
                    "Choose", ["A", "B"], wp="H1-04", pr=123, sha="c" * 40,
                    detail="soft preference", poll_seconds=0)
            self.assertEqual(result, "B")


if __name__ == "__main__":
    unittest.main()
