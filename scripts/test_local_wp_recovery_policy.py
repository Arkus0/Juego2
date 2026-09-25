#!/usr/bin/env python3
"""Offline recovery cases across the work-plane / owner-console boundary."""

from __future__ import annotations

import importlib.util
import argparse
import json
import sys
import tempfile
import unittest
from pathlib import Path
from unittest.mock import AsyncMock, Mock, patch

HERE = Path(__file__).parent


def load(name: str, filename: str):
    spec = importlib.util.spec_from_file_location(name, HERE / filename)
    module = importlib.util.module_from_spec(spec)
    sys.modules[spec.name] = module
    spec.loader.exec_module(module)
    return module


policy = load("arkus_recovery_policy_test", "local_wp_recovery_policy.py")
console_module = load("arkus_recovery_console_test", "local_wp_remote_console.py")
auth = load("arkus_recovery_auth_test", "owner_control_auth.py")
adoption = load("arkus_recovery_adoption_test", "reviewer_verdict_adoption.py")
process = load("arkus_recovery_process_test", "local_wp_autopilot_process.py")
remote = load("arkus_recovery_remote_test", "local_wp_autopilot_remote.py")
decision_helper = load("arkus_recovery_decision_test", "request_owner_decision.py")
continue_receiver = load("arkus_recovery_continue_receiver_test", "telegram_continue_receiver.py")

SHA = "a" * 40
CAMPAIGN = "b" * 32
DECISION = "c" * 32


def reviewer_review(review_id: str, *, sha: str = SHA,
                    at: str = "2026-09-25T04:00:00Z", verdict: str = "FAIL") -> dict:
    intent = adoption.safe.render_review(wp="WP-H1-06", pr=195,
        candidate_sha=sha, verdict=verdict, review_id=review_id)
    return {"user": {"login": "Arkus0"}, "created_at": at, "submitted_at": at,
            "commit_id": sha,
            "pull_request_url": "https://api.github.com/repos/Arkus0/Juego2/pulls/195",
            "body": f"Reviewer verdict: {verdict}\nReviewed candidate SHA: {sha}\n"
                    f"Autopilot review ID: {review_id}\n{intent}\n"}


class GitHubFixture:
    def __init__(self, *, sha: str = SHA, comments: list[dict] | None = None):
        self.sha = sha
        self.comments = comments or []
        self.dispatches: list[dict] = []
        self.pr = {"number": 195, "state": "open", "merged": False,
                   "body": "WP: WP-H1-06\nReviewer verdict: PENDING\nfail_cycle: 2\n",
                   "head": {"sha": sha, "ref": "worker/h1-06"}}

    def __call__(self, *args, **kwargs):
        path = args[-1]
        if any("dispatches" in arg for arg in args):
            self.dispatches.append(kwargs["input_json"])
            return None
        if "pulls?state=all" in path:
            return [[self.pr]]
        if "pulls/195" in path:
            return self.pr
        if "issues/195/comments" in path:
            return [self.comments]
        raise AssertionError(args)


def campaign_file(control: Path, *, status: str, pid: int | None = None,
                  log_path: str | None = None, secret: str = "secret") -> None:
    payload = {
        "version": 1, "campaign_id": CAMPAIGN, "wp": "H1-06", "mode": "run",
        "status": status, "reason": "Foundational circuit breaker" if status == "HARD_BLOCKER" else "",
        "pid": pid, "log_path": log_path, "blocked_sha": SHA,
        "paused": False, "stop_after_wp": False,
    }
    payload["proof"] = policy.campaign_proof(secret, payload)
    console_module.atomic_json(control / "campaign.json", payload)


def decision_file(control: Path, *, campaign: str = CAMPAIGN, sha: str = SHA,
                  completed: bool = False) -> None:
    options = ["Revisar el contrato", "Abandonar campaña"]
    digest = auth.decision_request_digest(campaign, DECISION, "H1-06", 195, sha,
                                          "¿Cómo proceder?", "Bloqueo de prueba", options)
    row = {"version": 3, "campaign_id": campaign, "decision_id": DECISION,
           "request_digest": digest, "wp": "H1-06", "pr": 195, "sha": sha,
           "question": "¿Cómo proceder?", "detail": "Bloqueo de prueba", "options": options}
    if completed:
        row["completed_at"] = "2026-09-25T04:00:00Z"
    console_module.atomic_json(control / "decisions" / f"{DECISION}.json", row)


class RecoveryPolicyTests(unittest.TestCase):
    def test_transient_child_and_network_failure_pause_without_hard_block(self):
        self.assertEqual(policy.classify_failure(TimeoutError()), policy.Disposition.RETRY)
        self.assertEqual(policy.classify_failure(Exception("App Server closed unexpectedly")),
                         policy.Disposition.RETRY)
        self.assertEqual(policy.child_disposition("child exited before receipt", 2),
                         policy.Disposition.RETRY)
        self.assertEqual(policy.child_disposition("WORK_PLANE_RESULT: PAUSED_RECOVERABLE", 2),
                         policy.Disposition.PAUSE)
        self.assertEqual(policy.child_disposition("WORK_PLANE_RESULT: HARD_BLOCKER", 2),
                         policy.Disposition.HARD)

    def test_old_target_blocker_is_ignored_but_unbound_blocker_fails_closed(self):
        rows = [{"state": "BLOCKED", "target sha": "d" * 40}]
        self.assertIsNone(policy.active_blocker(rows, SHA))
        rows.append({"state": "HUMAN_ACTION_REQUIRED"})
        self.assertIsNotNone(policy.active_blocker(rows, SHA))

    def test_h106_circuit_breaker_keeps_status_decision_and_control_plane_after_restart(self):
        with tempfile.TemporaryDirectory() as temp:
            control = Path(temp) / "control"
            campaign_file(control, status="HARD_BLOCKER", secret="test-supervisor-secret")
            decision_file(control)
            fixture = GitHubFixture()
            console = console_module.RemoteConsole(Path(temp), control, "test-supervisor-secret", 42)
            with patch.object(console_module._core, "gh", side_effect=fixture), \
                 patch.object(console, "send") as send, patch.object(console, "answer_callback"):
                console.restore_campaign()
                self.assertIsNone(console.proc)
                self.assertEqual(console.campaign_status, "HARD_BLOCKER")
                self.assertIn("HARD_BLOCKER", console.status())
                self.assertEqual(console.pending_decision_count(), 1)
                console.advertise_decisions()
                self.assertIn(DECISION, console._decision_snapshots)
                console.handle_decision({"id": "owner-click"}, DECISION, 0)
                self.assertEqual(len(fixture.dispatches), 1)
                self.assertEqual(fixture.dispatches[0]["client_payload"]["target_sha"], SHA)
                console.command("/resume")
                self.assertIsNone(console.proc)
                self.assertIn("HARD_BLOCKER", console.status())
                console.command("/help")
                console.command("/abandon")
                console.command("/status")
                self.assertFalse(console.active)
                self.assertGreaterEqual(send.call_count, 5)

    def test_stale_or_attested_decisions_do_not_block_recovery(self):
        with tempfile.TemporaryDirectory() as temp:
            control = Path(temp) / "control"
            console = console_module.RemoteConsole(Path(temp), control, "secret", 42)
            console.active, console.current_wp, console.campaign_id = True, "H1-06", CAMPAIGN
            decision_file(control, campaign="d" * 32)
            fixture = GitHubFixture()
            with patch.object(console_module._core, "gh", side_effect=fixture):
                self.assertEqual(console.pending_decision_count(), 0)
                decision_file(control, sha="e" * 40)
                self.assertEqual(console.pending_decision_count(), 0)
                decision_file(control, completed=True)
                self.assertEqual(console.pending_decision_count(), 0)
                decision_file(control)
                digest = json.loads((control / "decisions" / f"{DECISION}.json").read_text())["request_digest"]
                fixture.comments.append({"user": {"login": "github-actions[bot]"}, "body":
                    "ARKUS_LOCAL_AUTOPILOT\nState: OWNER_DECISION\n"
                    f"Campaign ID: {CAMPAIGN}\nDecision ID: {DECISION}\n"
                    f"Request Digest: {digest}\nTarget SHA: {SHA}\n"
                    f"Choice: 0\nSelected digest: {auth.decision_selected_digest(['Revisar el contrato', 'Abandonar campaña'][0])}\n"
                    "Authority Proof: supervisor-HMAC-v1\n"})
                self.assertEqual(console.pending_decision_count(), 0)

    def test_conflicting_bot_marker_stops_wp_but_not_status(self):
        with tempfile.TemporaryDirectory() as temp:
            control = Path(temp) / "control"
            campaign_file(control, status="PAUSED_RECOVERABLE")
            decision_file(control)
            fixture = GitHubFixture(comments=[{"user": {"login": "github-actions[bot]"},
                "body": "ARKUS_LOCAL_AUTOPILOT\nState: OWNER_DECISION\n"
                        f"Campaign ID: {CAMPAIGN}\nDecision ID: {DECISION}\n"
                        f"Target SHA: {'d' * 40}\n"}])
            console = console_module.RemoteConsole(Path(temp), control, "secret", 42)
            console.active, console.current_wp, console.campaign_id = True, "H1-06", CAMPAIGN
            with patch.object(console_module._core, "gh", side_effect=fixture):
                self.assertEqual(console.pending_decision_count(), 0)
                self.assertIn("HARD_BLOCKER", console.status())

    def test_restart_resumes_same_campaign_and_does_not_launch_twice(self):
        with tempfile.TemporaryDirectory() as temp:
            control = Path(temp) / "control"
            log = control / "interrupted.log"
            log.parent.mkdir(parents=True)
            log.write_text("WORK_PLANE_RESULT: PAUSED_RECOVERABLE\n", encoding="utf-8")
            campaign_file(control, status="RUNNING", pid=99999999, log_path=str(log))
            fixture = GitHubFixture()
            console = console_module.RemoteConsole(Path(temp), control, "secret", 42)
            launches = []

            def launch(wp, mode, *, resume=False):
                launches.append((wp, mode, resume, console.campaign_id))
                console.proc = Mock(pid=1234)
                console.campaign_status = "RUNNING"
                console._persist_campaign()

            with patch.object(console_module._core, "gh", side_effect=fixture), \
                 patch.object(console, "_pid_alive", return_value=False), \
                 patch.object(console, "launch", side_effect=launch):
                console.restore_campaign()
                console.restore_campaign()
            self.assertEqual(launches, [("H1-06", "run", True, CAMPAIGN)])
            self.assertEqual(fixture.dispatches, [])

    def test_worker_modified_campaign_hint_cannot_authorize_resume(self):
        with tempfile.TemporaryDirectory() as temp:
            control = Path(temp) / "control"
            campaign_file(control, status="PAUSED_RECOVERABLE")
            path = control / "campaign.json"
            row = json.loads(path.read_text(encoding="utf-8"))
            row["wp"] = "H1-07"
            path.write_text(json.dumps(row), encoding="utf-8")
            console = console_module.RemoteConsole(Path(temp), control, "secret", 42)
            with patch.object(console, "launch") as launch:
                console.restore_campaign()
            launch.assert_not_called()
            self.assertIn("HARD_BLOCKER", console.status())

    def test_launch_persists_authenticated_campaign_and_surviving_child_is_not_duplicated(self):
        with tempfile.TemporaryDirectory() as temp:
            root = Path(temp) / "checkout"
            assets = Path(temp) / "assets"
            root.mkdir()
            assets.mkdir()
            control = Path(temp) / "control"
            first = console_module.RemoteConsole(root, control, "secret", 42, assets)
            child = Mock(pid=4321)
            with patch.object(first, "_controller_busy", return_value=False), \
                 patch.object(first, "_ensure_ipc", return_value="http://127.0.0.1:12345"), \
                 patch.object(console_module._core.subprocess, "Popen", return_value=child), \
                 patch.object(first, "send"):
                first.launch("H1-06", "run")
            first.log_handle.close()
            row = json.loads((control / "campaign.json").read_text(encoding="utf-8"))
            self.assertTrue(policy.valid_campaign_proof("secret", row))
            self.assertEqual(row["pid"], 4321)

            second = console_module.RemoteConsole(root, control, "secret", 42, assets)
            with patch.object(console_module._core, "gh", side_effect=GitHubFixture()), \
                 patch.object(second, "_pid_alive", return_value=True), \
                 patch.object(second, "launch") as launch:
                second.restore_campaign()
            launch.assert_not_called()
            self.assertEqual(second.campaign_status, "DETACHED")

    def test_bad_update_does_not_consume_next_owner_command(self):
        with tempfile.TemporaryDirectory() as temp:
            console = console_module.RemoteConsole(Path(temp), Path(temp) / "control", "secret", 42)
            updates = [{"update_id": 1}, {"update_id": 2}]
            with patch.object(console, "_get_updates", return_value=updates), \
                 patch.object(console, "handle_update", side_effect=[RuntimeError("work plane died"), None]) as handle:
                console.poll_once()
            self.assertEqual(handle.call_count, 2)
            self.assertEqual(console.offset, 3)

    def test_detached_child_reconciles_terminal_log_before_any_relaunch(self):
        for result, expected_launch, expected_status in (
                ("HARD_BLOCKER", False, "HARD_BLOCKER"),
                ("PAUSED_RECOVERABLE", True, "PAUSED_RECOVERABLE")):
            with self.subTest(result=result), tempfile.TemporaryDirectory() as temp:
                root = Path(temp)
                control = root / "control"
                log = control / "prior.log"
                log.parent.mkdir(parents=True)
                log.write_text(f"WORK_PLANE_RESULT: {result}\n", encoding="utf-8")
                campaign_file(control, status="RUNNING", pid=123,
                              log_path=str(log))
                console = console_module.RemoteConsole(root, control, "secret", 42)
                with patch.object(console_module._core, "gh", side_effect=GitHubFixture()), \
                     patch.object(console, "_pid_alive", side_effect=[True, False]), \
                     patch.object(console, "_controller_busy", return_value=False), \
                     patch.object(console, "launch") as launch:
                    console.restore_campaign()
                    self.assertEqual(console.campaign_status, "DETACHED")
                    console.reconcile_detached()
                self.assertEqual(console.campaign_status, expected_status)
                self.assertEqual(launch.call_count, int(expected_launch))
                if expected_launch:
                    self.assertEqual(launch.call_args.kwargs, {"resume": True})

    def test_terminal_campaign_does_not_reopen_as_blocker_after_restart(self):
        for status in ("COMPLETE", "ABANDONED"):
            with self.subTest(status=status), tempfile.TemporaryDirectory() as temp:
                control = Path(temp) / "control"
                campaign_file(control, status=status)
                console = console_module.RemoteConsole(Path(temp), control, "secret", 42)
                with patch.object(console_module._core, "gh") as github, \
                     patch.object(console, "launch") as launch:
                    console.restore_campaign()
                self.assertFalse(console.active)
                self.assertEqual(console.campaign_status, "IDLE")
                github.assert_not_called()
                launch.assert_not_called()

    def test_old_continue_offer_remains_usable_only_through_local_owner_path(self):
        current = {"number": 195, "state": "open", "merged": False, "draft": False,
                   "head": {"sha": SHA},
                   "body": f"WP: WP-H1-06\nFrozen candidate SHA: {SHA}\n"}
        offer = {"user": {"login": "Arkus0"}, "created_at": "2020-01-01T00:00:00Z",
                 "body": f"ARKUS_LOCAL_AUTOPILOT\nState: SECOND_FAIL_OFFERED\n"
                         f"Target SHA: {SHA}\nFail count: 2\n"}
        reviews = [reviewer_review("e" * 32, sha="d" * 40), reviewer_review("f" * 32)]

        def github(path):
            if path.endswith("pulls/195"):
                return current
            if "issues/195/comments" in path:
                return [[offer]]
            if "pulls/195/reviews" in path:
                return [reviews]
            raise AssertionError(path)

        with patch.object(continue_receiver.core, "github", side_effect=github), \
             self.assertRaisesRegex(continue_receiver.core.ReceiverError, "stale"):
            continue_receiver.validate_current(195, SHA, 2)
        with patch.object(continue_receiver.core, "github", side_effect=github), \
             patch.object(continue_receiver.core, "fresh_offer", return_value=True):
            self.assertEqual(continue_receiver.validate_current(195, SHA, 2), "ready")

    def test_work_plane_failure_is_durable_and_status_remains_available(self):
        with tempfile.TemporaryDirectory() as temp:
            control = Path(temp) / "control"
            log = control / "work.log"
            log.parent.mkdir(parents=True)
            log.write_text("REMOTE_AUTOPILOT_STOP: Foundational circuit breaker\n"
                           "WORK_PLANE_RESULT: HARD_BLOCKER\n", encoding="utf-8")
            console = console_module.RemoteConsole(Path(temp), control, "secret", 42)
            console.active, console.current_wp, console.campaign_id = True, "H1-06", CAMPAIGN
            console.log_path = log
            console.proc = Mock(pid=123, returncode=2)
            console.proc.poll.return_value = 2
            with patch.object(console, "_github_campaign_pr", return_value=GitHubFixture().pr), \
                 patch.object(console, "send"):
                console.check_child()
                self.assertIsNone(console.proc)
                self.assertIn("HARD_BLOCKER", console.status())
                console.command("/status")
            self.assertEqual(json.loads((control / "campaign.json").read_text())["status"],
                             "HARD_BLOCKER")

    def test_exact_sha_and_reviewer_authority_remain_fail_closed(self):
        owner = reviewer_review("f" * 32)
        with self.assertRaises(adoption.AdoptionError):
            adoption.authoritative_verdicts([dict(owner, user={"login": "other"})], [])
        with self.assertRaises(adoption.AdoptionError):
            adoption.authoritative_verdicts([], [owner])
        with patch.object(adoption, "gh_json", return_value={"body": "WP: WP-H1-06\n"}), \
             self.assertRaises(adoption.AdoptionError):
            adoption.authoritative_verdicts([owner, reviewer_review("f" * 32, verdict="PASS")], [])

    def test_existing_bot_repair_transition_never_increments_fail_cycle_again(self):
        review_id = "f" * 32
        at = "2026-09-25T04:00:00Z"
        body = f"WP: WP-H1-06\nReviewer verdict: PENDING\nfail_cycle: 2\nFrozen candidate SHA: {SHA}\n"
        pr = {"number": 195, "state": "open", "head": {"sha": SHA}, "body": body}
        verdict = reviewer_review(review_id, at=at)
        repair = {"user": {"login": "github-actions[bot]"},
                  "created_at": "2026-09-25T04:01:00Z",
                  "body": f"ARKUS_AUTOMATION_V2\nState: REPAIR_REQUIRED\nTarget SHA: {SHA}\n"}
        comments = [repair]
        posted = []

        def pages(path):
            return [verdict] if "/reviews" in path else comments

        def post(_pr, text):
            posted.append(text)
            comments.append({"user": {"login": "github-actions[bot]"}, "body": text,
                             "created_at": "2026-09-25T04:02:00Z"})

        with patch.object(adoption, "gh_json", return_value=pr), \
             patch.object(adoption, "gh_pages", side_effect=pages), \
             patch.object(adoption, "validate_pr", return_value=body), \
             patch.object(adoption, "current_review_ready", return_value={"created_at": "2026-09-25T03:59:00Z"}), \
             patch.object(adoption, "post_comment", side_effect=post), \
             patch.object(adoption, "update_pr_body") as update:
            first = adoption.adopt(195, SHA, review_id, "FAIL")
            second = adoption.adopt(195, SHA, review_id, "FAIL")
        self.assertEqual((first["fail_cycle"], second["fail_cycle"]), (2, 2))
        self.assertEqual(len(posted), 1)  # Only the missing derived adoption ledger.
        self.assertTrue(all("fail_cycle: `2`" in call.args[1] for call in update.call_args_list))

    def test_interrupted_adoption_ledger_reconciles_body_without_duplicate_repair(self):
        review_id = "f" * 32
        body = f"WP: WP-H1-06\nfail_cycle: 1\nFrozen candidate SHA: {SHA}\n"
        pr = {"number": 195, "state": "open", "head": {"sha": SHA}, "body": body}
        comments = [
            {"user": {"login": "github-actions[bot]"}, "created_at": "2026-09-25T04:01:00Z",
             "body": f"ARKUS_LOCAL_AUTOPILOT\nState: REVIEW_VERDICT_ADOPTED\nTarget SHA: {SHA}\nReview ID: {review_id}\nVerdict: FAIL\nFail cycle: 2\n"},
            {"user": {"login": "github-actions[bot]"}, "created_at": "2026-09-25T04:01:01Z",
             "body": f"ARKUS_AUTOMATION_V2\nState: REPAIR_REQUIRED\nTarget SHA: {SHA}\n"},
        ]
        with patch.object(adoption, "gh_json", return_value=pr), \
             patch.object(adoption, "gh_pages", side_effect=lambda path: [reviewer_review(review_id)] if "/reviews" in path else comments), \
             patch.object(adoption, "validate_pr", return_value=body), \
             patch.object(adoption, "current_review_ready", return_value={"created_at": "2026-09-25T03:59:00Z"}), \
             patch.object(adoption, "post_comment") as post, \
             patch.object(adoption, "update_pr_body") as update:
            result = adoption.adopt(195, SHA, review_id, "FAIL")
        self.assertEqual(result["fail_cycle"], 2)
        self.assertIn("fail_cycle: `2`", update.call_args.args[1])
        post.assert_not_called()

    def test_stale_body_cycle_rebuilds_from_two_exact_owner_fails(self):
        current_id = "f" * 32
        earlier_id = "e" * 32
        body = f"WP: WP-H1-06\nfail_cycle: 1\nFrozen candidate SHA: {SHA}\n"
        pr = {"number": 195, "state": "open", "head": {"sha": SHA}, "body": body}
        comments = [
            {"user": {"login": "github-actions[bot]"}, "created_at": "2026-09-25T04:01:00Z",
             "body": f"ARKUS_AUTOMATION_V2\nState: REPAIR_REQUIRED\nTarget SHA: {SHA}\n"},
        ]
        with patch.object(adoption, "gh_json", return_value=pr), \
             patch.object(adoption, "gh_pages", side_effect=lambda path: [
                 reviewer_review(earlier_id, sha="d" * 40, at="2026-09-25T03:00:00Z"),
                 reviewer_review(current_id)] if "/reviews" in path else comments), \
             patch.object(adoption, "validate_pr", return_value=body), \
             patch.object(adoption, "current_review_ready", return_value={"created_at": "2026-09-25T03:59:00Z"}), \
             patch.object(adoption, "post_comment"), \
             patch.object(adoption, "update_pr_body") as update:
            result = adoption.adopt(195, SHA, current_id, "FAIL")
        self.assertEqual(result["fail_cycle"], 2)
        self.assertIn("fail_cycle: `2`", update.call_args.args[1])

    def test_old_same_sha_repair_marker_does_not_satisfy_new_appeal_fail(self):
        old = {"user": {"login": "github-actions[bot]"},
               "created_at": "2026-09-25T04:00:00Z",
               "body": f"ARKUS_AUTOMATION_V2\nState: REPAIR_REQUIRED\nTarget SHA: {SHA}\n"}
        self.assertFalse(adoption.repair_marker_after_verdict(
            [old], SHA, "2026-09-25T04:01:00Z"))

    def test_completed_roles_are_detected_before_retry(self):
        review_id = "d" * 32
        audit = {"state": "FAIL_AUDIT_COMPLETE", "target sha": SHA, "fail count": "2",
                 "audit json": '{"classification":"valid"}'}
        started = {"state": "PROTOCOL_FIX_STARTED", "target sha": SHA,
                   "review id": review_id, "_created_at": "2026-09-25T04:00:01Z"}
        ready = {"state": "REVIEW_READY", "target sha": SHA,
                 "_created_at": "2026-09-25T04:10:00Z"}
        protocol_verdict = {"id": review_id, "sha": SHA, "verdict": "PROTOCOL_FIX",
                            "at": "2026-09-25T04:00:00Z"}
        pr = {"head": {"sha": SHA}, "body": f"Frozen candidate SHA: {SHA}\n", "state": "open"}
        with patch.object(remote.autopilot, "gh_json", return_value=pr), \
             patch.object(remote.autopilot, "local_markers", return_value=[audit, started]), \
             patch.object(remote.autopilot, "markers", return_value=[ready]), \
             patch.object(remote.autopilot, "ready_context_matches", return_value=True), \
             patch.object(remote.autopilot, "reviewed_verdicts", return_value=[protocol_verdict]):
            self.assertTrue(remote._role_side_effect_already_complete(
                "fail-audit", "Audita el FAIL material #2 del PR #195 / H1-06"))
            self.assertTrue(remote._role_side_effect_already_complete(
                "protocol-fix", f"Corrige PROTOCOL_FIX del Reviewer ID {review_id} en PR #195. "
                                f"Mantén PRODUCT_SHA {SHA}"))
            self.assertTrue(remote._role_side_effect_already_complete(
                "reviewer", f"Reviewer PR #195 Autopilot review ID: {review_id}"))

    def test_pending_request_reuses_identity_after_worker_restart(self):
        with tempfile.TemporaryDirectory() as temp:
            control = Path(temp) / "control"
            decision_file(control)
            with patch.object(decision_helper, "_context", return_value=(control, CAMPAIGN, "http://127.0.0.1:1")), \
                 patch.object(decision_helper, "_verified_owner_response", return_value={
                     "choice": 0, "selected": "Revisar el contrato"}), \
                 patch.object(decision_helper.uuid, "uuid4") as new_id:
                result = decision_helper.request_decision(
                    "¿Cómo proceder?", ["Revisar el contrato", "Abandonar campaña"],
                    wp="H1-06", pr=195, sha=SHA, detail="Bloqueo de prueba")
            self.assertEqual(result, "Revisar el contrato")
            new_id.assert_not_called()
            self.assertTrue(json.loads((control / "decisions" / f"{DECISION}.json").read_text())["completed_at"])

    def test_owner_choice_remains_bound_after_supervisor_restart(self):
        with tempfile.TemporaryDirectory() as temp:
            control = Path(temp) / "control"
            campaign_file(control, status="HARD_BLOCKER")
            decision_file(control)
            fixture = GitHubFixture()
            first = console_module.RemoteConsole(Path(temp), control, "secret", 42)
            with patch.object(console_module._core, "gh", side_effect=fixture), \
                 patch.object(first, "send"), patch.object(first, "answer_callback"):
                first.restore_campaign()
                first.advertise_decisions()
                first.handle_decision({"id": "first-click"}, DECISION, 0)
            self.assertEqual(len(fixture.dispatches), 1)
            second = console_module.RemoteConsole(Path(temp), control, "secret", 42)
            with patch.object(console_module._core, "gh", side_effect=fixture), \
                 patch.object(second, "send"), patch.object(second, "answer_callback") as answer:
                second.restore_campaign()
                second.advertise_decisions()
                second.handle_decision({"id": "changed-choice"}, DECISION, 1)
            self.assertEqual(len(fixture.dispatches), 1)
            self.assertIn("otra opción", answer.call_args.args[1])

    def test_conflicting_bot_owner_attestations_fail_closed(self):
        options = ["A", "B"]
        digest = auth.decision_request_digest(CAMPAIGN, DECISION, "H1-06", 195, SHA,
                                              "Choose", "detail", options)
        comments = []
        for choice in (0, 1):
            comments.append({"user": {"login": "github-actions[bot]", "type": "Bot"},
                             "body": "ARKUS_LOCAL_AUTOPILOT\nState: OWNER_DECISION\n"
                             f"Target SHA: {SHA}\nCampaign ID: {CAMPAIGN}\nDecision ID: {DECISION}\n"
                             f"Request digest: {digest}\nChoice: {choice}\n"
                             f"Selected digest: {auth.decision_selected_digest(options[choice])}\n"
                             "Authority proof: supervisor-HMAC-v1\n"})
        with patch.object(decision_helper, "_github_comments", return_value=comments), \
             self.assertRaises(decision_helper.DecisionError):
            decision_helper._github_owner_attestation(195, SHA, CAMPAIGN, DECISION, digest, options)
        wrong_identity = dict(comments[0], body=comments[0]["body"].replace(
            f"Target SHA: {SHA}", f"Target SHA: {'d' * 40}"))
        with patch.object(decision_helper, "_github_comments", return_value=[wrong_identity]), \
             self.assertRaises(decision_helper.DecisionError):
            decision_helper._github_owner_attestation(195, SHA, CAMPAIGN, DECISION, digest, options)

    def test_pending_pr_body_verdict_reconciles_from_exact_github_review(self):
        review_id = "f" * 32
        current = {"number": 195, "state": "open", "merged": False,
                   "head": {"sha": SHA},
                   "body": f"WP: WP-H1-06\nFrozen candidate SHA: {SHA}\nReviewer verdict: PENDING\n"}
        review = reviewer_review(review_id)
        ready = {"state": "REVIEW_READY", "target sha": SHA,
                 "_created_at": "2026-09-25T03:59:00Z"}
        with patch.object(process, "_raw_review_items", return_value=([review], [])), \
             patch.object(process.adoption, "gh_json", return_value=current), \
             patch.object(process.autopilot, "gh_json", return_value=current), \
             patch.object(process.autopilot, "markers", return_value=[ready]), \
             patch.object(process.autopilot, "local_markers", return_value=[]), \
             patch.object(process.autopilot, "ready_context_matches", return_value=True), \
             patch.object(process, "_adoption_complete", side_effect=[False, True]), \
             patch.object(process, "_dispatch_adoption") as dispatch:
            verdicts = process.strict_reviewed_verdicts(195)
        self.assertEqual(verdicts[0]["verdict"], "FAIL")
        dispatch.assert_called_once()

    def test_possible_existing_pr_with_stale_body_cannot_create_second_pr(self):
        with tempfile.TemporaryDirectory() as temp:
            console = console_module.RemoteConsole(Path(temp), Path(temp) / "control", "secret", 42)
            console.current_wp = "H1-06"
            fixture = GitHubFixture()
            fixture.pr["body"] = "Reviewer verdict: PENDING\n"
            with patch.object(console_module._core, "gh", side_effect=fixture), \
                 self.assertRaises(console_module.ConsoleError):
                console._github_campaign_pr()


class ProcessRetryTests(unittest.IsolatedAsyncioTestCase):
    async def test_transient_retry_reenters_reconstruction_but_hard_does_not(self):
        args = argparse.Namespace(wp="H1-06")
        with patch.object(process, "process_main_async", new=AsyncMock(side_effect=[
                TimeoutError("GitHub timeout"), None])) as main, \
             patch.object(process.asyncio, "sleep", new=AsyncMock()):
            await process._run_with_retries(args)
        self.assertEqual(main.await_count, 2)
        with patch.object(process, "process_main_async", new=AsyncMock(
                side_effect=process.autopilot.StopFlow("contradictory exact SHA"))) as main:
            with self.assertRaises(process.autopilot.StopFlow):
                await process._run_with_retries(args)
        self.assertEqual(main.await_count, 1)


if __name__ == "__main__":
    unittest.main()