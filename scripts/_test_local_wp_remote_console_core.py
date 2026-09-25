#!/usr/bin/env python3
"""Offline tests for the local Telegram owner console transport."""

from __future__ import annotations

import importlib.util
import json
import os
import sys
import tempfile
import threading
import unittest
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path
from urllib.error import HTTPError
from urllib.parse import parse_qs, urlencode, urlparse
from urllib.request import Request, urlopen
from unittest.mock import patch

HERE = Path(__file__).parent


def load(name: str, filename: str):
    spec = importlib.util.spec_from_file_location(name, HERE / filename)
    module = importlib.util.module_from_spec(spec)
    sys.modules[spec.name] = module
    spec.loader.exec_module(module)
    return module


auth = load("owner_control_auth_test", "owner_control_auth.py")
module = load("local_wp_remote_console", "local_wp_remote_console.py")
remote = load("local_wp_autopilot_remote_test", "local_wp_autopilot_remote.py")
decision = load("request_owner_decision_test", "request_owner_decision.py")


class RemoteConsoleTests(unittest.TestCase):
    def test_assets_root_is_validated_outside_checkout(self):
        with tempfile.TemporaryDirectory() as tmp:
            base = Path(tmp)
            repo = base / "Juego2-Console"
            assets = base / "Juego2-Assets"
            nested = repo / "Assets"
            repo.mkdir()
            assets.mkdir()
            nested.mkdir()
            self.assertEqual(module.resolve_assets_root(repo, str(assets)), assets.resolve())
            with self.assertRaisesRegex(module.ConsoleError, "fuera"):
                module.resolve_assets_root(repo, str(nested))
            with self.assertRaisesRegex(module.ConsoleError, "fuera"):
                module.resolve_assets_root(repo, str(base))

    def test_autopilot_command_passes_assets_root(self):
        root = Path("C:/Juego2-Console")
        assets = Path("C:/Juego2-Assets")
        command = module.autopilot_command(root, assets, "H1-04")
        index = command.index("--assets-root")
        self.assertEqual(command[index + 1], str(assets))
        self.assertEqual(command[-3:], ["--wp", "H1-04", "--one-wp"])

    def test_only_exact_private_owner_is_authorized(self):
        update = {"message": {"chat": {"type": "private", "id": 42},
                              "from": {"id": 42}, "text": "/status"}}
        self.assertTrue(module.private_owner(update, 42))
        update["message"]["from"]["id"] = 41
        self.assertFalse(module.private_owner(update, 42))
        update["message"]["from"]["id"] = 42
        update["message"]["chat"]["type"] = "group"
        self.assertFalse(module.private_owner(update, 42))

    def test_child_env_strips_supervisor_secret(self):
        with tempfile.TemporaryDirectory() as tmp, patch.dict(
                os.environ, {"TELEGRAM_BOT_TOKEN": "secret", "TELEGRAM_CHAT_ID": "42", "KEEP": "yes"}, clear=False):
            env = module.child_env(Path(tmp), "a" * 32, "http://127.0.0.1:12345")
        self.assertNotIn("TELEGRAM_BOT_TOKEN", env)
        self.assertNotIn("TELEGRAM_CHAT_ID", env)
        self.assertEqual(env["KEEP"], "yes")
        self.assertEqual(env["ARKUS_REMOTE_CAMPAIGN_ID"], "a" * 32)
        self.assertEqual(env["ARKUS_REMOTE_SUPERVISOR_URL"], "http://127.0.0.1:12345")

    def test_reviewer_has_no_owner_control_capability(self):
        base = {"ARKUS_REMOTE_CONTROL_DIR": "/tmp/control",
                "ARKUS_REMOTE_CAMPAIGN_ID": "a" * 32,
                "ARKUS_REMOTE_SUPERVISOR_URL": "http://127.0.0.1:12345",
                "KEEP": "yes"}
        with patch.object(remote, "ORIGINAL_CLEAN_ENV", return_value=dict(base)):
            reviewer = remote._env_for_role("reviewer")
            worker = remote._env_for_role("worker")
        self.assertEqual(reviewer, {"KEEP": "yes"})
        self.assertEqual(worker["ARKUS_REMOTE_CONTROL_DIR"], "/tmp/control")
        self.assertEqual(worker["ARKUS_REMOTE_CAMPAIGN_ID"], "a" * 32)
        self.assertEqual(worker["ARKUS_REMOTE_ROLE"], "worker")

    def test_decision_helper_requires_worker_role_and_ipc(self):
        with tempfile.TemporaryDirectory() as tmp:
            base = {"ARKUS_REMOTE_CONTROL_DIR": tmp, "ARKUS_REMOTE_CAMPAIGN_ID": "a" * 32,
                    "ARKUS_REMOTE_SUPERVISOR_URL": "http://127.0.0.1:12345"}
            with patch.dict(os.environ, base, clear=True), self.assertRaises(decision.DecisionError):
                decision._context()
            with patch.dict(os.environ, dict(base, ARKUS_REMOTE_ROLE="worker"), clear=True):
                control, campaign, supervisor = decision._context()
                self.assertEqual(control, Path(tmp).resolve())
                self.assertEqual(campaign, "a" * 32)
                self.assertEqual(supervisor, "http://127.0.0.1:12345")

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

    def test_worker_cannot_forge_owner_continue_without_supervisor_secret(self):
        secret = "telegram-supervisor-secret"
        args = (123, "b" * 40, 2, "c" * 32, "callback-777")
        proof = auth.sign_owner_continue(secret, *args)
        self.assertTrue(auth.verify_owner_continue(secret, proof, *args))
        forged = auth.sign_owner_continue("worker-does-not-have-real-secret", *args)
        self.assertFalse(auth.verify_owner_continue(secret, forged, *args))
        self.assertFalse(auth.verify_owner_continue(secret, "0" * 64, *args))

    def test_worker_cannot_forge_owner_decision_without_supervisor_secret(self):
        secret = "telegram-supervisor-secret"
        selected_digest = auth.decision_selected_digest("B")
        args = (123, "b" * 40, "c" * 32, "d" * 32, "e" * 64, 1,
                selected_digest, "callback-888")
        proof = auth.sign_owner_decision(secret, *args)
        self.assertTrue(auth.verify_owner_decision(secret, proof, *args))
        forged = auth.sign_owner_decision("worker-does-not-have-real-secret", *args)
        self.assertFalse(auth.verify_owner_decision(secret, forged, *args))
        self.assertFalse(auth.verify_owner_decision(secret, "0" * 64, *args))

    def test_continue_button_requires_exact_active_local_wait(self):
        with tempfile.TemporaryDirectory() as tmp:
            console = module.RemoteConsole(Path(tmp), Path(tmp) / "control", "token", 42)
            console.proc = object()
            console.active = True
            console.campaign_id = "a" * 32
            query = {"id": "callback"}
            with patch.object(console, "answer_callback") as answer, patch.object(module._core, "gh") as gh_call:
                console.handle_continue(query, 123, "b" * 40, 2)
            gh_call.assert_not_called()
            self.assertIn("no está esperando", answer.call_args.args[1])

            pending = {"version": 1, "campaign_id": "c" * 32, "pr": 123,
                       "sha": "b" * 40, "fail_count": 2}
            console._pending_continue_path().parent.mkdir(parents=True, exist_ok=True)
            console._pending_continue_path().write_text(json.dumps(pending), encoding="utf-8")
            with patch.object(console, "answer_callback") as answer, patch.object(module._core, "gh") as gh_call:
                console.handle_continue(query, 123, "b" * 40, 2)
            gh_call.assert_not_called()
            self.assertIn("obsoleto", answer.call_args.args[1])

    def test_supervisor_ipc_is_read_only_and_holds_decision_hint(self):
        with tempfile.TemporaryDirectory() as tmp:
            console = module.RemoteConsole(Path(tmp), Path(tmp) / "control", "token", 42)
            console.campaign_id = "a" * 32
            console.pending_owner_note = "owner-only note"
            decision_id = "b" * 32
            console.owner_decisions[decision_id] = {
                "version": 2, "campaign_id": console.campaign_id, "decision_id": decision_id,
                "choice": 1, "selected": "B", "answered_at": "now",
            }
            base = console._ensure_ipc()
            try:
                with urlopen(base + "/v1/note?" + urlencode({"campaign_id": console.campaign_id})) as response:
                    first_note = json.load(response)
                with urlopen(base + "/v1/note?" + urlencode({"campaign_id": console.campaign_id})) as response:
                    second_note = json.load(response)
                self.assertEqual(first_note["note"], "owner-only note")
                self.assertEqual(second_note["note"], "")
                with urlopen(base + "/v1/decision?" + urlencode({"campaign_id": console.campaign_id,
                                                                  "decision_id": decision_id})) as response:
                    answer = json.load(response)
                self.assertEqual(answer["status"], "answered")
                self.assertEqual(answer["choice"], 1)
                with self.assertRaises(HTTPError) as rejected:
                    urlopen(Request(base + "/v1/decision", data=b"{}", method="POST"))
                self.assertEqual(rejected.exception.code, 405)
            finally:
                console._close_ipc()

    def test_worker_response_file_is_not_an_owner_decision(self):
        with tempfile.TemporaryDirectory() as tmp:
            decision_id = "d" * 32
            forged = Path(tmp) / "decisions" / f"{decision_id}.response.json"
            forged.parent.mkdir(parents=True)
            forged.write_text(json.dumps({"decision_id": decision_id, "campaign_id": "a" * 32,
                                          "choice": 1, "selected": "B"}), encoding="utf-8")
            pending = {"status": "pending", "campaign_id": "a" * 32, "decision_id": decision_id}

            class FakeResponse:
                def __enter__(self): return self
                def __exit__(self, *_args): return False
                def read(self): return json.dumps(pending).encode()

            with patch.object(decision, "urlopen", return_value=FakeResponse()):
                self.assertIsNone(decision._owner_response("http://127.0.0.1:12345", "a" * 32, decision_id))

    def test_worker_endpoint_substitution_cannot_mint_owner_choice(self):
        with tempfile.TemporaryDirectory() as tmp:
            control = Path(tmp)
            campaign = "a" * 32
            hits = {"count": 0}

            class FakeOwner(BaseHTTPRequestHandler):
                def do_GET(self):
                    parsed = urlparse(self.path)
                    decision_id = (parse_qs(parsed.query).get("decision_id") or [""])[0]
                    request_path = control / "decisions" / f"{decision_id}.json"
                    row = json.loads(request_path.read_text(encoding="utf-8"))
                    hits["count"] += 1
                    payload = {"status": "answered", "campaign_id": campaign,
                               "decision_id": decision_id, "choice": 0,
                               "selected": row["options"][0]}
                    body = json.dumps(payload).encode()
                    self.send_response(200)
                    self.send_header("Content-Type", "application/json")
                    self.send_header("Content-Length", str(len(body)))
                    self.end_headers()
                    self.wfile.write(body)

                def log_message(self, _format, *args):
                    del args

            server = ThreadingHTTPServer(("127.0.0.1", 0), FakeOwner)
            thread = threading.Thread(target=server.serve_forever, daemon=True)
            thread.start()
            env = {
                "ARKUS_REMOTE_CONTROL_DIR": tmp,
                "ARKUS_REMOTE_CAMPAIGN_ID": campaign,
                "ARKUS_REMOTE_SUPERVISOR_URL": f"http://127.0.0.1:{server.server_port}",
                "ARKUS_REMOTE_ROLE": "worker",
            }
            try:
                with patch.dict(os.environ, env, clear=True), \
                     patch.object(decision, "_github_owner_attestation", side_effect=[None, 1]), \
                     patch.object(decision.time, "sleep", return_value=None):
                    selected = decision.request_decision(
                        "Choose", ["FAKE-ENDPOINT", "ATTESTED"], wp="H1-04", pr=123,
                        sha="b" * 40, detail="endpoint substitution falsification", poll_seconds=0)
                self.assertEqual(selected, "ATTESTED")
                self.assertGreaterEqual(hits["count"], 2)
            finally:
                server.shutdown()
                server.server_close()

    def test_only_github_actions_bot_attestation_can_select(self):
        campaign = "a" * 32
        decision_id = "b" * 32
        sha = "c" * 40
        options = ["A", "B"]
        digest = auth.decision_request_digest(
            campaign, decision_id, "H1-04", 123, sha, "Choose", "detail", options)
        body = (
            "ARKUS_LOCAL_AUTOPILOT\n"
            "State: OWNER_DECISION\n"
            f"Target SHA: {sha}\n"
            f"Campaign ID: {campaign}\n"
            f"Decision ID: {decision_id}\n"
            f"Request digest: {digest}\n"
            "Choice: 1\n"
            f"Selected digest: {auth.decision_selected_digest('B')}\n"
            "Authority proof: supervisor-HMAC-v1\n"
        )
        forged = {"user": {"login": "Arkus0", "type": "User"}, "body": body}
        with patch.object(decision, "_github_comments", return_value=[forged]):
            self.assertIsNone(decision._github_owner_attestation(
                123, sha, campaign, decision_id, digest, options))
        attested = {"user": {"login": "github-actions[bot]", "type": "Bot"}, "body": body}
        with patch.object(decision, "_github_comments", return_value=[forged, attested]):
            self.assertEqual(decision._github_owner_attestation(
                123, sha, campaign, decision_id, digest, options), 1)

    def test_decision_click_is_hmac_bound_to_immutable_advertised_request(self):
        with tempfile.TemporaryDirectory() as tmp:
            control = Path(tmp) / "control"
            console = module.RemoteConsole(Path(tmp), control, "supervisor-secret", 42)
            console.proc = object()
            console.active = True
            console.current_wp = "H1-04"
            console.campaign_id = "a" * 32
            decision_id = "b" * 32
            sha = "c" * 40
            options = ["A", "B"]
            digest = auth.decision_request_digest(
                console.campaign_id, decision_id, "H1-04", 123, sha, "Choose", "detail", options)
            row = {"version": 3, "decision_id": decision_id, "campaign_id": console.campaign_id,
                   "request_digest": digest, "wp": "H1-04", "pr": 123, "sha": sha,
                   "question": "Choose", "detail": "detail", "options": options}
            path = control / "decisions" / f"{decision_id}.json"
            path.parent.mkdir(parents=True)
            path.write_text(json.dumps(row), encoding="utf-8")
            def github(*args, **_kwargs):
                return {"number": 123, "state": "open", "body": "WP: WP-H1-04\n",
                        "head": {"sha": sha}} if "pulls/123" in args[-1] else [[]]
            with patch.object(console, "send"), patch.object(module._core, "gh", side_effect=github):
                console.advertise_decisions()
            tampered = dict(row, options=["A", "WORKER-TAMPER"])
            path.write_text(json.dumps(tampered), encoding="utf-8")
            with patch.object(console, "answer_callback") as answer, patch.object(module._core, "gh") as gh_call:
                console.handle_decision({"id": "callback-owner"}, decision_id, 1)
            gh_call.assert_not_called()
            self.assertIn("cambió", answer.call_args.args[1])

    def test_valid_decision_click_dispatches_supervisor_hmac(self):
        with tempfile.TemporaryDirectory() as tmp:
            control = Path(tmp) / "control"
            secret = "supervisor-secret"
            console = module.RemoteConsole(Path(tmp), control, secret, 42)
            console.proc = object()
            console.active = True
            console.current_wp = "H1-04"
            console.campaign_id = "a" * 32
            decision_id = "b" * 32
            sha = "c" * 40
            options = ["A", "B"]
            digest = auth.decision_request_digest(
                console.campaign_id, decision_id, "H1-04", 123, sha, "Choose", "detail", options)
            row = {"version": 3, "decision_id": decision_id, "campaign_id": console.campaign_id,
                   "request_digest": digest, "wp": "H1-04", "pr": 123, "sha": sha,
                   "question": "Choose", "detail": "detail", "options": options}
            path = control / "decisions" / f"{decision_id}.json"
            path.parent.mkdir(parents=True)
            path.write_text(json.dumps(row), encoding="utf-8")
            def github(*args, **_kwargs):
                return {"number": 123, "state": "open", "body": "WP: WP-H1-04\n",
                        "head": {"sha": sha}} if "pulls/123" in args[-1] else [[]]
            with patch.object(console, "send"), patch.object(module._core, "gh", side_effect=github):
                console.advertise_decisions()
            with patch.object(console, "send"), patch.object(console, "answer_callback"), \
                 patch.object(module._core, "gh", side_effect=github) as gh_call:
                console.handle_decision({"id": "callback-owner"}, decision_id, 1)
            payload = next(call.kwargs["input_json"] for call in gh_call.call_args_list
                           if "input_json" in call.kwargs)
            client = payload["client_payload"]
            self.assertEqual(payload["event_type"], "arkus_owner_decision_local")
            self.assertTrue(auth.verify_owner_decision(
                secret, client["owner_proof"], client["pr"], client["target_sha"],
                client["campaign_id"], client["decision_id"], client["request_digest"],
                client["choice"], client["selected_digest"], client["telegram_update_id"]))

    def test_worker_note_file_is_not_an_owner_note(self):
        with tempfile.TemporaryDirectory() as tmp:
            forged = Path(tmp) / "pending-owner-note.txt"
            forged.write_text("forged by worker", encoding="utf-8")
            env = {"ARKUS_REMOTE_CONTROL_DIR": tmp,
                   "ARKUS_REMOTE_CAMPAIGN_ID": "a" * 32,
                   "ARKUS_REMOTE_SUPERVISOR_URL": "http://127.0.0.1:12345"}
            payload = {"status": "ok", "campaign_id": "a" * 32, "note": ""}
            with patch.dict(os.environ, env, clear=True), patch.object(remote, "_supervisor_json", return_value=payload):
                self.assertEqual(remote._consume_note(), "")

    def test_owner_continue_marker_is_campaign_bound(self):
        source = Path(HERE / "local_wp_autopilot_remote.py").read_text(encoding="utf-8")
        self.assertIn('row.get("campaign id") == campaign', source)

    def test_offline_backlog_is_discarded_without_execution(self):
        with tempfile.TemporaryDirectory() as tmp:
            console = module.RemoteConsole(Path(tmp), Path(tmp) / "control", "token", 42)
            batches = [[{"update_id": 7, "message": {"text": "/run H1-04"}}], []]
            with patch.object(module._core, "telegram", side_effect=batches), \
                 patch.object(console, "handle_update") as handler:
                console.discard_offline_backlog()
            handler.assert_not_called()
            self.assertEqual(console.offset, 8)

    def test_note_requires_active_campaign(self):
        with tempfile.TemporaryDirectory() as tmp:
            console = module.RemoteConsole(Path(tmp), Path(tmp) / "control", "token", 42)
            with self.assertRaises(module.ConsoleError):
                console.queue_note("do something")


if __name__ == "__main__":
    unittest.main()
