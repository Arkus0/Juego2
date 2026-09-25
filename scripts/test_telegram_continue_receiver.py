#!/usr/bin/env python3
"""Offline tests for owner-only Telegram continue authorization."""

import importlib.util
import sys
import unittest
from datetime import datetime, timezone
from pathlib import Path
from unittest.mock import patch

SCRIPT = Path(__file__).with_name("telegram_continue_receiver.py")
spec = importlib.util.spec_from_file_location("telegram_continue_receiver", SCRIPT)
module = importlib.util.module_from_spec(spec)
sys.modules[spec.name] = module
spec.loader.exec_module(module)


class ReceiverTests(unittest.TestCase):
    def test_only_exact_private_owner_click(self):
        sha = "a" * 40
        query = {"data": f"arkus:continue:123:{sha}:2",
                 "from": {"id": 42}, "message": {"chat": {"type": "private", "id": 42}}}
        self.assertTrue(module.core.authorized_click(query, 42, 123, sha, 2))
        query["from"]["id"] = 43
        self.assertFalse(module.core.authorized_click(query, 42, 123, sha, 2))
        query["from"]["id"] = 42
        query["message"]["chat"]["type"] = "group"
        self.assertFalse(module.core.authorized_click(query, 42, 123, sha, 2))
        query["message"]["chat"]["type"] = "private"
        self.assertFalse(module.core.authorized_click(query, 42, 123, "b" * 40, 2))

    def test_marker_is_exact_sha_and_count(self):
        sha = "a" * 40
        body = f"ARKUS_LOCAL_AUTOPILOT\nState: SECOND_FAIL_OFFERED\nTarget SHA: {sha}\nFail count: 2\n"
        self.assertTrue(module.core.marker(body, "SECOND_FAIL_OFFERED", sha, 2))
        self.assertFalse(module.core.marker(body, "SECOND_FAIL_OFFERED", "b" * 40, 2))
        self.assertFalse(module.core.marker(body, "SECOND_FAIL_OFFERED", sha, 3))

    def test_rejects_moved_pr_and_accepts_exact_offer(self):
        sha = "a" * 40
        pr = {"state": "open", "merged": False, "draft": False,
              "head": {"sha": sha}, "body": f"WP: WP-H1-06\nFrozen candidate SHA: {sha}\n"}
        offer = {"body": f"ARKUS_LOCAL_AUTOPILOT\nState: SECOND_FAIL_OFFERED\nTarget SHA: {sha}\nFail count: 2\n",
                 "user": {"login": "Arkus0"},
                 "created_at": datetime.now(timezone.utc).isoformat()}
        previous_sha = "b" * 40
        def review(candidate_sha, review_id):
            return {"body": module.safe.render_review(wp="WP-H1-06", pr=123,
                        candidate_sha=candidate_sha, verdict="FAIL", review_id=review_id),
                    "commit_id": candidate_sha, "user": {"login": "Arkus0"}}
        fail = review(sha, "1" * 32)
        previous_fail = review(previous_sha, "2" * 32)
        with patch.object(module.core, "github", side_effect=[pr, [[offer]], [[fail, previous_fail]]]):
            self.assertEqual(module.validate_current(123, sha, 2), "ready")
        forged_offer = dict(offer, user={"login": "public-commenter"})
        with patch.object(module.core, "github", side_effect=[pr, [[forged_offer]], [[fail, previous_fail]]]), \
             self.assertRaisesRegex(module.core.ReceiverError, "No matching second-FAIL offer"):
            module.validate_current(123, sha, 2)
        with patch.object(module.core, "github", side_effect=[pr, [[offer, fail, previous_fail]], [[fail, previous_fail]]]):
            self.assertEqual(module.validate_current(123, sha, 2), "ready")
        expired = {"body": f"ARKUS_LOCAL_AUTOPILOT\nState: CONTINUE_EXPIRED\nTarget SHA: {sha}\nFail count: 2\n",
                   "user": {"login": "github-actions[bot]"}}
        with patch.object(module.core, "github", side_effect=[pr, [[offer, expired]]]), \
             self.assertRaisesRegex(module.core.ReceiverError, "expired or became unavailable"):
            module.validate_current(123, sha, 2)
        stale_offer = dict(offer, created_at="2020-01-01T00:00:00Z")
        with patch.object(module.core, "github", side_effect=[pr, [[stale_offer]]]), \
             self.assertRaisesRegex(module.core.ReceiverError, "offer is stale"):
            module.validate_current(123, sha, 2)
        with patch.object(module.core, "github", side_effect=[pr, [[offer]], [[fail, fail]]]), \
             self.assertRaisesRegex(module.core.ReceiverError, "FAIL count changed"):
            module.validate_current(123, sha, 2)
        moved = dict(pr, head={"sha": "b" * 40})
        with patch.object(module.core, "github", return_value=moved), \
             self.assertRaisesRegex(module.core.ReceiverError, "HEAD moved"):
            module.validate_current(123, sha, 2)

    def test_existing_owner_continue_does_not_start_long_poll(self):
        sha = "a" * 40
        with patch.dict(module.core.os.environ, {"TELEGRAM_BOT_TOKEN": "test-only", "TELEGRAM_CHAT_ID": "42"}), \
             patch.object(module.core, "telegram", return_value={"url": ""}) as telegram_call, \
             patch.object(module, "validate_current", return_value="already"):
            self.assertEqual(module.receive(123, sha, 2, 100), "already")
        telegram_call.assert_called_once_with("test-only", "getWebhookInfo", {})


if __name__ == "__main__":
    unittest.main()
