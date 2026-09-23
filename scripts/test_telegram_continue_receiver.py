#!/usr/bin/env python3
"""Offline tests for owner-only Telegram continue authorization."""

import importlib.util
import sys
import unittest
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
        self.assertTrue(module.authorized_click(query, 42, 123, sha, 2))
        query["from"]["id"] = 43
        self.assertFalse(module.authorized_click(query, 42, 123, sha, 2))
        query["from"]["id"] = 42
        query["message"]["chat"]["type"] = "group"
        self.assertFalse(module.authorized_click(query, 42, 123, sha, 2))
        query["message"]["chat"]["type"] = "private"
        self.assertFalse(module.authorized_click(query, 42, 123, "b" * 40, 2))

    def test_marker_is_exact_sha_and_count(self):
        sha = "a" * 40
        body = f"ARKUS_LOCAL_AUTOPILOT\nState: SECOND_FAIL_OFFERED\nTarget SHA: {sha}\nFail count: 2\n"
        self.assertTrue(module.marker(body, "SECOND_FAIL_OFFERED", sha, 2))
        self.assertFalse(module.marker(body, "SECOND_FAIL_OFFERED", "b" * 40, 2))
        self.assertFalse(module.marker(body, "SECOND_FAIL_OFFERED", sha, 3))

    def test_rejects_moved_pr_and_accepts_exact_offer(self):
        sha = "a" * 40
        pr = {"state": "open", "merged": False, "draft": False,
              "head": {"sha": sha}, "body": f"Frozen candidate SHA: {sha}\n"}
        offer = {"body": f"ARKUS_LOCAL_AUTOPILOT\nState: SECOND_FAIL_OFFERED\nTarget SHA: {sha}\nFail count: 2\n",
                 "user": {"login": "Arkus0"}}
        previous_sha = "b" * 40
        fail = {"body": f"Reviewer verdict: FAIL\nReviewed candidate SHA: {sha}\nAutopilot review ID: {'1' * 32}\n",
                "user": {"login": "Arkus0"}}
        previous_fail = {"body": f"Reviewer verdict: FAIL\nReviewed candidate SHA: {previous_sha}\nAutopilot review ID: {'2' * 32}\n",
                         "user": {"login": "Arkus0"}}
        with patch.object(module, "github", side_effect=[pr, [[offer]], [[fail, previous_fail]]]):
            self.assertEqual(module.validate_current(123, sha, 2), "ready")
        forged_offer = dict(offer, user={"login": "public-commenter"})
        with patch.object(module, "github", side_effect=[pr, [[forged_offer]], [[fail, previous_fail]]]), \
             self.assertRaisesRegex(module.ReceiverError, "No matching second-FAIL offer"):
            module.validate_current(123, sha, 2)
        with patch.object(module, "github", side_effect=[pr, [[offer, fail, previous_fail]], [[fail, previous_fail]]]):
            self.assertEqual(module.validate_current(123, sha, 2), "ready")
        with patch.object(module, "github", side_effect=[pr, [[offer]], [[fail, fail]]]), \
             self.assertRaisesRegex(module.ReceiverError, "FAIL count changed"):
            module.validate_current(123, sha, 2)
        moved = dict(pr, head={"sha": "b" * 40})
        with patch.object(module, "github", return_value=moved), \
             self.assertRaisesRegex(module.ReceiverError, "HEAD moved"):
            module.validate_current(123, sha, 2)

    def test_existing_owner_continue_does_not_start_long_poll(self):
        sha = "a" * 40
        with patch.dict(module.os.environ, {"TELEGRAM_BOT_TOKEN": "test-only", "TELEGRAM_CHAT_ID": "42"}), \
             patch.object(module, "telegram", return_value={"url": ""}) as telegram_call, \
             patch.object(module, "validate_current", return_value="already"):
            self.assertEqual(module.receive(123, sha, 2, 100), "already")
        telegram_call.assert_called_once_with("test-only", "getWebhookInfo", {})


if __name__ == "__main__":
    unittest.main()
