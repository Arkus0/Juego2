#!/usr/bin/env python3
"""Regression test for Telegram owner-decision presentation."""

from __future__ import annotations

import importlib.util
import json
import sys
import tempfile
import unittest
from pathlib import Path
from unittest.mock import patch

HERE = Path(__file__).parent


def load_console():
    spec = importlib.util.spec_from_file_location(
        "local_wp_remote_console_ui_test", HERE / "local_wp_remote_console.py")
    module = importlib.util.module_from_spec(spec)
    sys.modules[spec.name] = module
    spec.loader.exec_module(module)
    return module


module = load_console()


class DecisionPresentationTests(unittest.TestCase):
    def test_full_options_are_in_message_and_buttons_are_short_choices(self):
        with tempfile.TemporaryDirectory() as tmp:
            control = Path(tmp) / "control"
            console = module.RemoteConsole(Path(tmp), control, "supervisor-secret", 42)
            console.proc = object()
            console.current_wp = "H1-05"
            console.campaign_id = "a" * 32

            decision_id = "b" * 32
            sha = "c" * 40
            options = [
                "Usar la variante norte con toda la explicación completa que no debe truncarse en el botón",
                "Mantener la variante oeste como alternativa preferida para este workpack",
            ]
            question = "¿Qué variante prefieres?"
            digest = module._core.owner_auth.decision_request_digest(
                console.campaign_id, decision_id, "H1-05", 123, sha,
                question, "", options)
            row = {
                "version": 3,
                "decision_id": decision_id,
                "campaign_id": console.campaign_id,
                "request_digest": digest,
                "wp": "H1-05",
                "pr": 123,
                "sha": sha,
                "question": question,
                "detail": "",
                "options": options,
            }
            path = control / "decisions" / f"{decision_id}.json"
            path.parent.mkdir(parents=True)
            path.write_text(json.dumps(row), encoding="utf-8")

            with patch.object(console, "send") as send:
                console.advertise_decisions()

            send.assert_called_once()
            message, markup = send.call_args.args
            self.assertIn(f"1. {options[0]}", message)
            self.assertIn(f"2. {options[1]}", message)
            self.assertEqual(markup["inline_keyboard"][0][0]["text"], "1️⃣ Elegir opción 1")
            self.assertEqual(markup["inline_keyboard"][1][0]["text"], "2️⃣ Elegir opción 2")
            self.assertNotIn(options[0][:48], markup["inline_keyboard"][0][0]["text"])
            self.assertEqual(
                markup["inline_keyboard"][0][0]["callback_data"],
                f"arkus:decision:{decision_id}:0",
            )
            self.assertEqual(
                markup["inline_keyboard"][1][0]["callback_data"],
                f"arkus:decision:{decision_id}:1",
            )


if __name__ == "__main__":
    unittest.main()
