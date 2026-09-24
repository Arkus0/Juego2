#!/usr/bin/env python3
"""Regression tests for remote short-quota pre-role liveness."""

import unittest

import local_wp_autopilot_remote as remote


def limits(short_used=97, long_used=20, short_reset=2000, long_reset=5000,
           reached=None):
    bucket = {
        "primary": {
            "usedPercent": short_used,
            "windowDurationMins": 300,
            "resetsAt": short_reset,
        },
        "secondary": {
            "usedPercent": long_used,
            "windowDurationMins": 10080,
            "resetsAt": long_reset,
        },
    }
    if reached is not None:
        bucket["rateLimitReachedType"] = reached
    return {"rateLimitsByLimitId": {"codex": bucket}}


class ShortQuotaGuardTests(unittest.TestCase):
    def test_short_floor_waits_without_explicit_reached_marker(self):
        self.assertEqual(
            remote.autopilot.quota_decision(limits(short_used=97), 1000),
            ("wait_short", 2000),
        )

    def test_explicit_short_reached_waits_instead_of_stopping_campaign(self):
        self.assertEqual(
            remote.autopilot.quota_decision(
                limits(short_used=96, reached="primary"), 1000),
            ("wait_short", 2000),
        )

    def test_general_floor_wins_over_short_reached(self):
        self.assertEqual(
            remote.autopilot.quota_decision(
                limits(short_used=99, long_used=98, reached="primary"), 1000),
            ("stop_general", None),
        )

    def test_explicit_general_reached_stops_even_above_percentage_floor(self):
        self.assertEqual(
            remote.autopilot.quota_decision(
                limits(short_used=20, long_used=95, reached="secondary"), 1000),
            ("stop_general", None),
        )

    def test_unknown_reached_type_fails_closed(self):
        with self.assertRaisesRegex(remote.autopilot.StopFlow, "Unknown rate limit reached type"):
            remote.autopilot.quota_decision(limits(reached="mystery"), 1000)

    def test_stale_short_reset_fails_closed(self):
        with self.assertRaisesRegex(remote.autopilot.StopFlow, "reset time is stale"):
            remote.autopilot.quota_decision(
                limits(short_used=99, short_reset=900, reached="primary"), 1000)


if __name__ == "__main__":
    unittest.main()
