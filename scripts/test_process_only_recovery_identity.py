#!/usr/bin/env python3
"""Causal regressions for exact review-cycle identity and retry idempotence."""

from __future__ import annotations

import argparse
import importlib.util
import sys
import unittest
from pathlib import Path
from unittest.mock import AsyncMock, patch

HERE = Path(__file__).parent


def load(name: str, filename: str):
    spec = importlib.util.spec_from_file_location(name, HERE / filename)
    if spec is None or spec.loader is None:
        raise RuntimeError(f"cannot load {filename}")
    module = importlib.util.module_from_spec(spec)
    sys.modules[spec.name] = module
    spec.loader.exec_module(module)
    return module


process = load("process_recovery_identity_test", "local_wp_autopilot_process.py")
adoption = process.adoption

SHA = "a" * 40
REVIEW_A = "1" * 32
REVIEW_B = "2" * 32
AT_A = "2026-09-25T10:00:00Z"
AT_B = "2026-09-25T10:01:00Z"


def pr_record(fail_cycle: int = 0) -> dict:
    return {
        "number": 192,
        "state": "open",
        "merged": False,
        "draft": False,
        "head": {"sha": SHA},
        "body": (
            "WP: `WP-H1-05`\n"
            "Worker state: `FROZEN_FOR_REVIEW`\n"
            f"Candidate HEAD SHA: `{SHA}`\n"
            "Worker pre-review: `CLEAN`\n"
            f"Frozen candidate SHA: `{SHA}`\n"
            "Branch frozen: `YES`\n"
            "Worker verdict: `IN_REVIEW`\n"
            f"fail_cycle: `{fail_cycle}`\n"
        ),
    }


def review_item(review_id: str, at: str) -> dict:
    body = adoption.safe.render_review(
        wp="WP-H1-05",
        pr=192,
        candidate_sha=SHA,
        verdict="FAIL",
        review_id=review_id,
    )
    return {
        "user": {"login": "Arkus0"},
        "submitted_at": at,
        "commit_id": SHA,
        "pull_request_url": "https://api.github.com/repos/Arkus0/Juego2/pulls/192",
        "body": body,
    }


def adoption_comment(review_id: str, cycle: int, at: str) -> dict:
    return {
        "user": {"login": "github-actions[bot]"},
        "created_at": at,
        "body": (
            "ARKUS_LOCAL_AUTOPILOT\n"
            "State: REVIEW_VERDICT_ADOPTED\n"
            f"Target SHA: {SHA}\n"
            f"Review ID: {review_id}\n"
            "Verdict: FAIL\n"
            f"Fail cycle: {cycle}\n"
        ),
    }


def repair_comment(review_id: str, at: str) -> dict:
    return {
        "user": {"login": "github-actions[bot]"},
        "created_at": at,
        "body": (
            "ARKUS_AUTOMATION_V2\n"
            "State: REPAIR_REQUIRED\n"
            f"Key: adopted-repair-required:192:{SHA}:{review_id}\n"
            f"Target SHA: {SHA}\n"
        ),
    }


class ExactReviewCycleTests(unittest.TestCase):
    def test_late_repair_marker_from_previous_review_cannot_complete_new_fail_cycle(self):
        current = pr_record(fail_cycle=2)
        with patch.object(adoption, "gh_json", return_value=current):
            verdicts = adoption.authoritative_verdicts(
                [review_item(REVIEW_A, AT_A), review_item(REVIEW_B, AT_B)], [])
        verdict_b = verdicts[-1]

        adopted_b = adoption_comment(REVIEW_B, 2, "2026-09-25T10:01:01Z")
        late_repair_a = repair_comment(REVIEW_A, "2026-09-25T10:01:02Z")
        wrong_comments = [adopted_b, late_repair_a]

        self.assertFalse(adoption.repair_marker_after_verdict(wrong_comments, SHA, AT_B))
        with patch.object(process.autopilot, "gh_pages", return_value=wrong_comments):
            self.assertFalse(process._adoption_complete(192, verdict_b))

        repair_b = repair_comment(REVIEW_B, "2026-09-25T10:01:03Z")
        exact_comments = wrong_comments + [repair_b]
        self.assertTrue(adoption.repair_marker_after_verdict(exact_comments, SHA, AT_B))
        with patch.object(process.autopilot, "gh_pages", return_value=exact_comments), \
             patch.object(process.autopilot, "gh_json", return_value=current):
            self.assertTrue(process._adoption_complete(192, verdict_b))


class DurableReasoningRetryTests(unittest.IsolatedAsyncioTestCase):
    async def _exercise_retry(self, role: str, prompt: str) -> None:
        durable = {"done": False}
        attempts = {"count": 0}

        async def reasoning(*_args, **_kwargs):
            durable["done"] = True
            return "done"

        async def one_attempt(_args):
            await process.bounded_codex_role(
                Path("."), Path("."), role, prompt, "gpt-test", "high")
            attempts["count"] += 1
            if attempts["count"] == 1:
                raise ConnectionError("connection reset after durable role completion")

        def local_markers(_pr: int):
            if role == "fail-audit" and durable["done"]:
                return [{
                    "state": "FAIL_AUDIT_COMPLETE",
                    "target sha": SHA,
                    "fail count": "1",
                }]
            return []

        def markers(_pr: int):
            if role == "protocol-fix" and durable["done"]:
                return [{"state": "REVIEW_READY", "target sha": SHA}]
            return []

        def latest_marker(rows, state, sha=None):
            for row in reversed(rows):
                if row.get("state") != state:
                    continue
                if sha is not None and row.get("target sha") != sha:
                    continue
                return row
            return None

        original = AsyncMock(side_effect=reasoning)
        old_work_only = process.WORK_ONLY
        process.WORK_ONLY = False
        try:
            with patch.object(process, "process_main_async", new=one_attempt), \
                 patch.object(process.remote, "ORIGINAL_CODEX_ROLE", new=original), \
                 patch.object(process.autopilot, "gh_json", return_value=pr_record()), \
                 patch.object(process.autopilot, "local_markers", side_effect=local_markers), \
                 patch.object(process.autopilot, "markers", side_effect=markers), \
                 patch.object(process.autopilot, "latest_marker", side_effect=latest_marker), \
                 patch.object(process.autopilot, "ready_context_matches", side_effect=lambda _root, _pr, marker: bool(marker)), \
                 patch.object(process.asyncio, "sleep", new=AsyncMock()):
                await process._run_with_retries(argparse.Namespace())
        finally:
            process.WORK_ONLY = old_work_only

        self.assertEqual(attempts["count"], 2)
        self.assertEqual(original.await_count, 1)

    async def test_global_retry_does_not_duplicate_completed_fail_audit(self):
        await self._exercise_retry(
            "fail-audit",
            f"PR #192\nFAIL material #1\nSHA {SHA}",
        )

    async def test_global_retry_does_not_duplicate_completed_protocol_fix(self):
        await self._exercise_retry(
            "protocol-fix",
            f"PR #192\nPRODUCT_SHA {SHA}\nSHA {SHA}",
        )


if __name__ == "__main__":
    unittest.main()
