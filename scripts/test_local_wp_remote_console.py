#!/usr/bin/env python3
"""Offline tests for the local Telegram owner console transport."""

from __future__ import annotations

import importlib.util
import os
import sys
import tempfile
import unittest
from pathlib import Path
from unittest.mock import patch

SCRIPT = Path(__file__).with_name("local_wp_remote_console.py")
spec = importlib.util.spec_from_file_location("local_wp_remote_console", SCRIPT)
module = importlib.util.module_from_spec(spec)
sys.modules[spec.name] = module
spec.loader.exec_module(module)


class RemoteConsoleTests(unittest.TestCase):
    def test_only_exact_private_owner_is_authorized(self):
        update = {"message": {"chat": {"type": "private", "id": 42},
                              "from": {"id": 42}, "text": "/status"}}
        self.assertTrue(module.private_owner(update, 42))
        update["message"]["from"]["id"] = 41
        self.assertFalse(module.private_owner(update, 42))
        update["message"]["from"]["id"] = 42
        update["message"]["chat"]["type"] = "group"
        self.assertFalse(module.private_owner(update, 42))

    def test_child_env_strips_telegram_secret(self):
        with tempfile.TemporaryDirectory() as tmp, patch.dict(
                os.environ, {"TELEGRAM_BOT_TOKEN": "secret", "TELEGRAM_CHAT_ID": "42", "KEEP": "yes"}, clear=False):
            env = module.child_env(Path(tmp), "a" * 32)
        self.assertNotIn("TELEGRAM_BOT_TOKEN", env)
        self.assertNotIn("TELEGRAM_CHAT_ID", env)
        self.assertEqual(env["KEEP"], "yes")
        self.assertEqual(env["ARKUS_REMOTE_CAMPAIGN_ID"], "a" * 32)

    def test_parse_next_is_exact(self):
        self.assertEqual(module.parse_next("x\nStopped after 1 completed WP(s); next=CTX-DW-H1-01\n"), "CTX-DW-H1-01")
        self.assertIsNone(module.parse_next("Stopped after 1 completed WP(s); next=NONE\n"))
        self.assertIsNone(module.parse_next("unrelated"))

    def test_callback_shapes_are_bounded(self):
        sha = "a" * 40
        self.assertIsNotNone(module.CONTINUE_RE.fullmatch(f"arkus:continue:123:{sha}:2"))
        self.assertIsNone(module.CONTINUE_RE.fullmatch(f"arkus:continue:123:{sha}:4"))
        self.assertIsNotNone(module.DECISION_RE.fullmatch(f"arkus:decision:{'b' * 32}:2"))
        self.assertIsNone(module.DECISION_RE.fullmatch(f"arkus:decision:{'b' * 32}:3"))


if __name__ == "__main__":
    unittest.main()
