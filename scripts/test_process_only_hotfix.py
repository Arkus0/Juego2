#!/usr/bin/env python3
"""PROCESS_ONLY regressions for Telegram state adoption and /work liveness."""

from __future__ import annotations

import argparse
import importlib.util
import json
import sys
import tempfile
import unittest
from pathlib import Path
from unittest.mock import AsyncMock, patch

HERE = Path(__file__).parent


def load(name: str, filename: str):
    spec = importlib.util.spec_from_file_location(name, HERE / filename)
    module = importlib.util.module_from_spec(spec)
    sys.modules[spec.name] = module
    assert spec.loader is not None
    spec.loader.exec_module(module)
    return module


console_module = load("process_hotfix_console_test", "local_wp_remote_console.py")
process = load("process_hotfix_autopilot_test", "local_wp_autopilot_process.py")
adoption = load("process_hotfix_adoption_test", "reviewer_verdict_adoption.py")

SHA = "a" * 40
WRONG_SHA = "b" * 40
REVIEW_ID = "1" * 32
CAMPAIGN = "c" * 32


def pr_body(sha: str = SHA, fail_cycle: int = 0) -> str:
    return (
        "WP: `WP-H1-05`\n"
        "Worker state: `FROZEN_FOR_REVIEW`\n"
        f"Candidate HEAD SHA: `{sha}`\n"
        "Worker pre-review: `CLEAN`\n"
        f"Frozen candidate SHA: `{sha}`\n"
        "Branch frozen: `YES`\n"
        "Worker verdict: `IN_REVIEW`\n"
        "Reviewer verdict: `PENDING`\n"
        f"fail_cycle: `{fail_cycle}`\n"
    )


def review_item(verdict: str = "FAIL", sha: str = SHA, review_id: str = REVIEW_ID,
                actor: str = "Arkus0", at: str = "2026-09-24T18:00:00Z") -> dict:
    return {
        "user": {"login": actor},
        "created_at": at,
        "submitted_at": at,
        "body": (
            f"Reviewer verdict: {verdict}\n"
            f"Reviewed candidate SHA: {sha}\n"
            f"Autopilot review ID: {review_id}\n"
        ),
    }


def ready_comment(sha: str = SHA, at: str = "2026-09-24T17:59:00Z") -> dict:
    return {
        "user": {"login": "github-actions[bot]"},
        "created_at": at,
        "body": f"ARKUS_AUTOMATION_V2\nState: REVIEW_READY\nTarget SHA: {sha}\n",
    }


def adoption_comment(verdict: str = "FAIL", cycle: int = 1) -> dict:
    extra = f"Fail cycle: {cycle}\n" if verdict == "FAIL" else ""
    return {
        "user": {"login": "github-actions[bot]"},
        "created_at": "2026-09-24T18:00:02Z",
        "body": (
            "ARKUS_LOCAL_AUTOPILOT\nState: REVIEW_VERDICT_ADOPTED\n"
            f"Target SHA: {SHA}\nReview ID: {REVIEW_ID}\nVerdict: {verdict}\n{extra}"
        ),
    }


def repair_comment() -> dict:
    return {
        "user": {"login": "github-actions[bot]"},
        "created_at": "2026-09-24T18:00:03Z",
        "body": f"ARKUS_AUTOMATION_V2\nState: REPAIR_REQUIRED\nTarget SHA: {SHA}\n",
    }


class TelegramDecisionRegressionTests(unittest.TestCase):
    def _decision(self, campaign: str, ident: str, **extra) -> dict:
        options = ["A", "B"]
        row = {
            "version": 3,
            "campaign_id": campaign,
            "decision_id": ident,
            "wp": "H1-05",
            "pr": 192,
            "sha": SHA,
            "question": "Choose",
            "detail": "fixture",
            "options": options,
        }
        row.update(extra)
        row["request_digest"] = console_module._core.owner_auth.decision_request_digest(
            campaign, ident, row["wp"], row["pr"], row["sha"], row["question"], row["detail"], options)
        return row

    def test_status_and_advertisement_share_only_current_valid_pending_universe(self):
        with tempfile.TemporaryDirectory() as tmp:
            control = Path(tmp) / "control"
            decisions = control / "decisions"
            decisions.mkdir(parents=True)
            console = console_module.RemoteConsole(Path(tmp), control, "token", 42)
            console.proc = object()
            console.current_wp = "H1-05"
            console.campaign_id = CAMPAIGN

            rows = [
                self._decision(CAMPAIGN, "1" * 32),
                self._decision("d" * 32, "2" * 32),
                self._decision(CAMPAIGN, "3" * 32, completed_at="done"),
                self._decision(CAMPAIGN, "4" * 32, abandoned_at="gone"),
            ]
            invalid = self._decision(CAMPAIGN, "5" * 32)
            invalid["request_digest"] = "0" * 64
            rows.append(invalid)
            for row in rows:
                (decisions / f"{row['decision_id']}.json").write_text(json.dumps(row), encoding="utf-8")
            (decisions / f"{'6' * 32}.response.json").write_text("{}", encoding="utf-8")

            self.assertEqual(console.pending_decision_count(), 1)
            self.assertIn("Decisiones pendientes válidas: 1", console.status())
            with patch.object(console, "send") as send:
                console.advertise_decisions()
            self.assertEqual(send.call_count, 1)
            self.assertEqual(console.sent_decisions, {"1" * 32})

    def test_run_and_work_select_distinct_boundaries(self):
        with tempfile.TemporaryDirectory() as tmp:
            console = console_module.RemoteConsole(Path(tmp), Path(tmp) / "control", "token", 42)
            with patch.object(console, "launch") as launch:
                console.command("/work H1-05")
                launch.assert_called_once_with("H1-05", "work")
            with patch.object(console, "launch") as launch:
                console.command("/run H1-05")
                launch.assert_called_once_with("H1-05", "run")

    def test_work_command_uses_process_wrapper_and_work_only_flag(self):
        root = Path("C:/Juego2")
        assets = Path("C:/Juego2-Assets")
        work = console_module.autopilot_command(root, assets, "H1-05", "work")
        run = console_module.autopilot_command(root, assets, "H1-05", "run")
        self.assertIn("local_wp_autopilot_process.py", work[1])
        self.assertIn("--work-only", work)
        self.assertNotIn("--work-only", run)

    def test_work_review_ready_message_contains_wp_sha_and_run_handoff(self):
        class Finished:
            returncode = 0
            def poll(self): return 0

        with tempfile.TemporaryDirectory() as tmp:
            root = Path(tmp)
            console = console_module.RemoteConsole(root, root / "control", "token", 42)
            log = root / "work.log"
            log.write_text(f"WORK_BOUNDARY REVIEW_READY WP=H1-05 SHA={SHA}\n", encoding="utf-8")
            console.proc = Finished()
            console.command_mode = "work"
            console.current_wp = "H1-05"
            console.campaign_id = CAMPAIGN
            console.log_path = log
            console.active = True
            with patch.object(console, "send") as send:
                console.check_child()
            self.assertEqual(console.boundary_state, "REVIEW_READY")
            self.assertEqual(console.boundary_sha, SHA)
            message = send.call_args.args[0]
            self.assertIn("H1-05", message)
            self.assertIn(SHA, message)
            self.assertIn("/run H1-05", message)
            self.assertIn("no invoca Reviewer", message)


class ReviewerAdoptionRegressionTests(unittest.TestCase):
    def test_structured_verdict_fails_closed_on_wrong_review_id_and_ambiguity(self):
        with self.assertRaises(adoption.AdoptionError):
            adoption.structured_verdict(
                f"Reviewer verdict: FAIL\nReviewed candidate SHA: {SHA}\nAutopilot review ID: not-valid\n")
        with self.assertRaises(adoption.AdoptionError):
            adoption.structured_verdict(
                f"Reviewer verdict: FAIL\nReviewer verdict: PASS\nReviewed candidate SHA: {SHA}\nAutopilot review ID: {REVIEW_ID}\n")

    def test_invalid_authority_and_conflicting_verdicts_fail_closed(self):
        with self.assertRaisesRegex(adoption.AdoptionError, "invalid authority"):
            adoption.authoritative_verdicts([], [review_item(actor="mallory")])
        first = review_item("FAIL")
        second = review_item("PASS", at="2026-09-24T18:01:00Z")
        with self.assertRaisesRegex(adoption.AdoptionError, "contradictory"):
            adoption.authoritative_verdicts([], [first, second])

    def test_wrong_sha_adoption_is_rejected(self):
        current = {"state": "open", "merged": False, "draft": False,
                   "head": {"sha": SHA}, "body": pr_body()}
        with patch.object(adoption, "gh_json", return_value=current), \
             patch.object(adoption, "gh_pages", side_effect=[[review_item(sha=WRONG_SHA)], [ready_comment()]]), \
             self.assertRaisesRegex(adoption.AdoptionError, "wrong SHA"):
            adoption.adopt(192, SHA, REVIEW_ID, "FAIL")

    def test_h1_05_pr_192_pending_body_adopts_exact_sha_fail(self):
        current = {"state": "open", "merged": False, "draft": False,
                   "head": {"sha": SHA}, "body": pr_body()}
        verdict = {"id": REVIEW_ID, "sha": SHA, "verdict": "FAIL", "at": "2026-09-24T18:00:00Z"}
        ready = {"state": "REVIEW_READY", "target sha": SHA,
                 "_created_at": "2026-09-24T17:59:00Z"}
        with patch.object(process, "_authoritative_verdicts", return_value=[verdict]), \
             patch.object(process.autopilot, "gh_json", return_value=current), \
             patch.object(process.autopilot, "markers", return_value=[ready]), \
             patch.object(process.autopilot, "local_markers", return_value=[]), \
             patch.object(process, "_ensure_adopted") as ensure:
            rows = process.strict_reviewed_verdicts(192)
        self.assertEqual(rows, [verdict])
        ensure.assert_called_once_with(192, verdict)
        self.assertIn("Reviewer verdict: `PENDING`", current["body"])

    def test_wrong_sha_in_current_review_ready_cycle_fails_closed(self):
        current = {"state": "open", "merged": False, "draft": False,
                   "head": {"sha": SHA}, "body": pr_body()}
        wrong = {"id": REVIEW_ID, "sha": WRONG_SHA, "verdict": "FAIL", "at": "2026-09-24T18:00:00Z"}
        ready = {"state": "REVIEW_READY", "target sha": SHA,
                 "_created_at": "2026-09-24T17:59:00Z"}
        with patch.object(process, "_authoritative_verdicts", return_value=[wrong]), \
             patch.object(process.autopilot, "gh_json", return_value=current), \
             patch.object(process.autopilot, "markers", return_value=[ready]), \
             self.assertRaisesRegex(process.autopilot.StopFlow, "wrong-SHA"):
            process.strict_reviewed_verdicts(192)

    def test_same_fail_adoption_is_idempotent_and_does_not_increment_twice(self):
        current0 = {"state": "open", "merged": False, "draft": False,
                    "head": {"sha": SHA}, "body": pr_body(fail_cycle=0)}
        reviews = [review_item()]
        comments0 = [ready_comment()]
        with patch.object(adoption, "gh_json", return_value=current0), \
             patch.object(adoption, "gh_pages", side_effect=[reviews, comments0]), \
             patch.object(adoption, "post_comment") as post, \
             patch.object(adoption, "update_pr_body") as update:
            first = adoption.adopt(192, SHA, REVIEW_ID, "FAIL")
        self.assertEqual(first["fail_cycle"], 1)
        self.assertEqual(post.call_count, 2)  # adoption ledger + REPAIR_REQUIRED
        self.assertIn("fail_cycle: `1`", update.call_args.args[1])

        current1 = dict(current0, body=pr_body(fail_cycle=1))
        comments1 = [ready_comment(), adoption_comment(), repair_comment()]
        with patch.object(adoption, "gh_json", return_value=current1), \
             patch.object(adoption, "gh_pages", side_effect=[reviews, comments1]), \
             patch.object(adoption, "post_comment") as post, \
             patch.object(adoption, "update_pr_body") as update:
            second = adoption.adopt(192, SHA, REVIEW_ID, "FAIL")
        self.assertEqual(second["fail_cycle"], 1)
        post.assert_not_called()
        update.assert_not_called()


class WorkRunRoutingTests(unittest.IsolatedAsyncioTestCase):
    async def test_work_never_invokes_reviewer(self):
        old = process.WORK_ONLY
        process.WORK_ONLY = True
        try:
            with patch.object(process, "_current_identity_from_prompt", return_value=("H1-05", 192, SHA)), \
                 patch.object(process.remote, "remote_codex_role", new=AsyncMock()) as delegate:
                with self.assertRaises(process.WorkBoundary) as stopped:
                    await process.bounded_codex_role(Path("."), Path("."), "reviewer", "prompt", "model", "high")
            self.assertEqual(stopped.exception.kind, "REVIEW_READY")
            delegate.assert_not_awaited()
        finally:
            process.WORK_ONLY = old

    async def test_run_on_review_ready_allows_reviewer(self):
        old = process.WORK_ONLY
        process.WORK_ONLY = False
        try:
            delegate = AsyncMock(return_value="done")
            with patch.object(process.remote, "remote_codex_role", new=delegate):
                result = await process.bounded_codex_role(Path("."), Path("."), "reviewer", "prompt", "model", "high")
            self.assertEqual(result, "done")
            delegate.assert_awaited_once()
        finally:
            process.WORK_ONLY = old

    async def test_work_on_repair_required_executes_repair(self):
        old = process.WORK_ONLY
        process.WORK_ONLY = True
        try:
            delegate = AsyncMock(return_value="repaired")
            with patch.object(process.remote, "remote_codex_role", new=delegate):
                result = await process.bounded_codex_role(Path("."), Path("."), "repair", "prompt", "model", "high")
            self.assertEqual(result, "repaired")
            self.assertEqual(delegate.await_args.args[2], "repair")
        finally:
            process.WORK_ONLY = old

    async def test_run_and_work_reuse_existing_canonical_pr_in_remote_recovery(self):
        existing = {"number": 192, "state": "open"}
        for work_only in (False, True):
            args = argparse.Namespace(
                dry_run=False, wp="H1-05", root=".", state=".", assets_root="/assets",
                adopt=False, one_wp=True, work_only=work_only,
            )
            adopted = AsyncMock()
            with patch.object(process.autopilot, "require_repo"), \
                 patch.object(process.autopilot, "resolve_assets_root", return_value=Path("/assets")), \
                 patch.object(process.autopilot, "run", return_value=""), \
                 patch.object(process.remote, "_resume_candidate", return_value=None), \
                 patch.object(process.autopilot, "canonical_pr", return_value=existing), \
                 patch.object(process.remote, "_run_canonical_adopted", new=adopted), \
                 patch.object(process.autopilot, "prepare_new_wp") as bootstrap:
                await process.remote.remote_main_async(args)
            adopted.assert_awaited_once_with(args)
            bootstrap.assert_not_called()


if __name__ == "__main__":
    unittest.main()
