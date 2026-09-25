#!/usr/bin/env python3
import argparse
import importlib.util
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path
from unittest.mock import AsyncMock, patch

SCRIPT = Path(__file__).with_name("local_wp_autopilot_remote.py")
spec = importlib.util.spec_from_file_location("local_wp_autopilot_remote_test", SCRIPT)
module = importlib.util.module_from_spec(spec)
sys.modules[spec.name] = module
spec.loader.exec_module(module)


class ShortQuotaResumeTests(unittest.IsolatedAsyncioTestCase):
    @staticmethod
    def git(cwd: Path, *args: str) -> str:
        return subprocess.check_output(
            ["git", *args], cwd=cwd, text=True, stderr=subprocess.DEVNULL
        ).strip()

    @staticmethod
    def git_run(cwd: Path, *args: str) -> None:
        subprocess.run(
            ["git", *args], cwd=cwd, check=True,
            stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL,
        )

    async def test_short_quota_failure_retries_same_role(self):
        original = AsyncMock(side_effect=[module.autopilot.StopFlow("quota exit"), "ok"])
        decision = AsyncMock(return_value=("wait_short", 2000))
        wait = AsyncMock()
        with patch.object(module, "ORIGINAL_CODEX_ROLE", original), \
             patch.object(module, "_failed_role_quota_decision", decision), \
             patch.object(module, "_wait_for_short_reset", wait), \
             patch.object(module, "_role_side_effect_already_complete", return_value=False):
            result = await module.remote_codex_role(
                Path("."), Path("state"), "fail-audit", "Audit PR #185 / H1-04",
                "gpt-6-luna", "xhigh")
        self.assertEqual(result, "ok")
        self.assertEqual(original.await_count, 2)
        self.assertIn("SHORT-QUOTA RECOVERY", original.await_args_list[1].args[3])
        wait.assert_awaited_once_with(2000, "H1-04", 185)

    async def test_non_quota_failure_is_not_retried(self):
        original = AsyncMock(side_effect=module.autopilot.StopFlow("network"))
        with patch.object(module, "ORIGINAL_CODEX_ROLE", original), \
             patch.object(module, "_failed_role_quota_decision", AsyncMock(return_value=("not_quota", None))):
            with self.assertRaisesRegex(module.autopilot.StopFlow, "network"):
                await module.remote_codex_role(
                    Path("."), Path("state"), "fail-audit", "Audit PR #185 / H1-04",
                    "gpt-6-luna", "xhigh")
        self.assertEqual(original.await_count, 1)

    async def test_general_floor_stops_without_retry(self):
        original = AsyncMock(side_effect=module.autopilot.StopFlow("quota exit"))
        with patch.object(module, "ORIGINAL_CODEX_ROLE", original), \
             patch.object(module, "_failed_role_quota_decision", AsyncMock(return_value=("stop_general", None))), \
             patch.object(module.autopilot, "notify") as notify:
            with self.assertRaisesRegex(module.autopilot.StopFlow, "General quota") as caught:
                await module.remote_codex_role(
                    Path("."), Path("state"), "fail-audit", "Audit PR #185 / H1-04",
                    "gpt-6-luna", "xhigh")
        self.assertTrue(caught.exception.notified)
        notify.assert_called_once()
        self.assertEqual(original.await_count, 1)

    async def test_durable_reviewer_verdict_prevents_duplicate_retry(self):
        original = AsyncMock(side_effect=module.autopilot.StopFlow("quota exit"))
        wait = AsyncMock()
        prompt = ("Reviewer independiente NUEVO del PR #185 / H1-04. "
                  "Autopilot review ID: " + "a" * 32)
        with patch.object(module, "ORIGINAL_CODEX_ROLE", original), \
             patch.object(module, "_failed_role_quota_decision", AsyncMock(return_value=("wait_short", 2000))), \
             patch.object(module, "_role_side_effect_already_complete", return_value=True), \
             patch.object(module, "_wait_for_short_reset", wait):
            result = await module.remote_codex_role(
                Path("."), Path("state"), "reviewer", prompt, "gpt-6-sol", "xhigh")
        self.assertEqual(result, "")
        self.assertEqual(original.await_count, 1)
        wait.assert_not_awaited()

    async def test_rechecks_durable_reviewer_verdict_after_wait(self):
        original = AsyncMock(side_effect=module.autopilot.StopFlow("quota exit"))
        wait = AsyncMock()
        prompt = ("Reviewer independiente NUEVO del PR #185 / H1-04. "
                  "Autopilot review ID: " + "b" * 32)
        with patch.object(module, "ORIGINAL_CODEX_ROLE", original), \
             patch.object(module, "_failed_role_quota_decision", AsyncMock(return_value=("wait_short", 2000))), \
             patch.object(module, "_role_side_effect_already_complete", side_effect=[False, True]) as completed, \
             patch.object(module, "_wait_for_short_reset", wait):
            result = await module.remote_codex_role(
                Path("."), Path("state"), "reviewer", prompt, "gpt-6-sol", "xhigh")
        self.assertEqual(result, "")
        self.assertEqual(original.await_count, 1)
        self.assertEqual(completed.call_count, 2)
        wait.assert_awaited_once_with(2000, "H1-04", 185)

    async def test_worker_failure_snapshots_before_short_retry(self):
        original = AsyncMock(side_effect=[module.autopilot.StopFlow("quota exit"), "ok"])
        with patch.object(module, "ORIGINAL_CODEX_ROLE", original), \
             patch.object(module, "_consume_note", return_value=""), \
             patch.object(module, "_best_effort_snapshot") as snapshot, \
             patch.object(module, "_failed_role_quota_decision", AsyncMock(return_value=("wait_short", 2000))), \
             patch.object(module, "_wait_for_short_reset", AsyncMock()), \
             patch.object(module, "_role_side_effect_already_complete", return_value=False):
            result = await module.remote_codex_role(
                Path("."), Path("state"), "worker", "Resume Worker H1-04 on PR #185",
                "gpt-6-sol", "xhigh")
        self.assertEqual(result, "ok")
        snapshot.assert_called_once()

    async def test_canonical_handoff_forces_adopt_then_restores_flag(self):
        args = argparse.Namespace(adopt=False, root=".", wp=None)
        seen = []

        async def fake_main(received):
            seen.append(received.adopt)

        with patch.object(module, "ORIGINAL_MAIN_ASYNC", fake_main):
            await module._run_canonical_adopted(args)
        self.assertEqual(seen, [True])
        self.assertFalse(args.adopt)

    async def test_remote_admission_refreshes_stale_origin_main_before_new_resume_decision(self):
        class AdmissionObserved(RuntimeError):
            pass

        with tempfile.TemporaryDirectory() as tmp:
            base = Path(tmp)
            origin = base / "origin.git"
            seed = base / "seed"
            checkout = base / "checkout"
            seed.mkdir()

            self.git_run(base, "init", "--bare", str(origin))
            self.git_run(seed, "init")
            self.git_run(seed, "config", "user.name", "Arkus Test")
            self.git_run(seed, "config", "user.email", "arkus-test@example.invalid")
            (seed / "gate.txt").write_text("old\n", encoding="utf-8")
            self.git_run(seed, "add", "gate.txt")
            self.git_run(seed, "commit", "-m", "old main")
            self.git_run(seed, "branch", "-M", "main")
            self.git_run(seed, "remote", "add", "origin", str(origin))
            self.git_run(seed, "push", "-u", "origin", "main")
            self.git_run(base, "--git-dir", str(origin), "symbolic-ref", "HEAD", "refs/heads/main")
            self.git_run(base, "clone", str(origin), str(checkout))

            stale = self.git(checkout, "rev-parse", "origin/main")
            (seed / "gate.txt").write_text("new\n", encoding="utf-8")
            self.git_run(seed, "commit", "-am", "advance main")
            self.git_run(seed, "push", "origin", "main")
            fresh = self.git(seed, "rev-parse", "HEAD")
            self.assertNotEqual(stale, fresh)
            self.assertEqual(self.git(checkout, "rev-parse", "origin/main"), stale)

            def observe_admission(root: Path, wp: str):
                self.assertEqual(wp, "H1-04")
                self.assertEqual(self.git(root, "rev-parse", "origin/main"), fresh)
                raise AdmissionObserved

            args = argparse.Namespace(
                dry_run=False,
                wp="H1-04",
                root=str(checkout),
                state=str(base / "state"),
                assets_root=None,
                adopt=False,
            )
            with patch.object(module.autopilot, "require_repo", return_value=None), \
                 patch.object(module.autopilot, "resolve_assets_root", return_value=None), \
                 patch.object(module, "_resume_candidate", side_effect=observe_admission):
                with self.assertRaises(AdmissionObserved):
                    await module.remote_main_async(args)

            self.assertEqual(self.git(checkout, "rev-parse", "origin/main"), fresh)


if __name__ == "__main__":
    unittest.main()
