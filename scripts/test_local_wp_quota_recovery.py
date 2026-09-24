#!/usr/bin/env python3
import importlib.util
import sys
import unittest
from pathlib import Path

SCRIPT = Path(__file__).with_name("local_wp_quota_recovery.py")
spec = importlib.util.spec_from_file_location("local_wp_quota_recovery", SCRIPT)
module = importlib.util.module_from_spec(spec)
sys.modules[spec.name] = module
spec.loader.exec_module(module)


class QuotaRecoveryTests(unittest.TestCase):
    @staticmethod
    def payload(primary_used=20, secondary_used=20, reached=None,
                primary_reset=2000, secondary_reset=9000):
        bucket = {
            "primary": {
                "usedPercent": primary_used,
                "windowDurationMins": 300,
                "resetsAt": primary_reset,
            },
            "secondary": {
                "usedPercent": secondary_used,
                "windowDurationMins": 10080,
                "resetsAt": secondary_reset,
            },
        }
        if reached is not None:
            bucket["rateLimitReachedType"] = reached
        return {"rateLimitsByLimitId": {"codex": bucket}}

    def test_short_window_exhaustion_is_auto_waitable(self):
        payload = self.payload(primary_used=100, secondary_used=40, reached="primary")
        self.assertEqual(
            module.classify_after_session_failure(payload, 1000),
            ("wait_short", 2000),
        )

    def test_explicit_short_reached_survives_rounded_percentage(self):
        payload = self.payload(primary_used=96, secondary_used=40, reached="primary")
        self.assertEqual(
            module.classify_after_session_failure(payload, 1000),
            ("wait_short", 2000),
        )

    def test_general_floor_wins_over_simultaneous_short_limit(self):
        payload = self.payload(primary_used=100, secondary_used=98, reached="primary")
        self.assertEqual(
            module.classify_after_session_failure(payload, 1000),
            ("stop_general", None),
        )

    def test_explicit_general_reached_stops_even_if_rounding_is_above_floor(self):
        payload = self.payload(primary_used=30, secondary_used=96, reached="secondary")
        self.assertEqual(
            module.classify_after_session_failure(payload, 1000),
            ("stop_general", None),
        )

    def test_non_quota_role_failure_is_not_retried(self):
        payload = self.payload(primary_used=40, secondary_used=40)
        self.assertEqual(
            module.classify_after_session_failure(payload, 1000),
            ("not_quota", None),
        )

    def test_nearly_exhausted_short_window_without_reached_signal_is_not_quota(self):
        payload = self.payload(primary_used=98, secondary_used=40)
        self.assertEqual(
            module.classify_after_session_failure(payload, 1000),
            ("not_quota", None),
        )

    def test_unknown_reached_shape_fails_closed(self):
        payload = self.payload(primary_used=40, secondary_used=40, reached="mystery")
        self.assertEqual(
            module.classify_after_session_failure(payload, 1000),
            ("unknown_reached", None),
        )

    def test_stale_short_reset_is_not_retried(self):
        payload = self.payload(primary_used=100, secondary_used=40,
                               reached="primary", primary_reset=900)
        with self.assertRaisesRegex(module.QuotaRecoveryError, "stale"):
            module.classify_after_session_failure(payload, 1000)

    def test_missing_general_window_fails_closed(self):
        payload = {"rateLimitsByLimitId": {"codex": {
            "primary": {"usedPercent": 100, "windowDurationMins": 300, "resetsAt": 2000},
            "rateLimitReachedType": "primary",
        }}}
        with self.assertRaisesRegex(module.QuotaRecoveryError, "General quota window"):
            module.classify_after_session_failure(payload, 1000)


if __name__ == "__main__":
    unittest.main()
