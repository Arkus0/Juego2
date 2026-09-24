#!/usr/bin/env python3
import importlib.util
import json
import subprocess
import sys
import tempfile
import unittest
import zipfile
from pathlib import Path

SCRIPT = Path(__file__).with_name("local_wp_worker_recovery.py")
spec = importlib.util.spec_from_file_location("local_wp_worker_recovery", SCRIPT)
module = importlib.util.module_from_spec(spec)
sys.modules[spec.name] = module
spec.loader.exec_module(module)

REPO = "Arkus0/Juego2"


def git(root: Path, *args: str) -> str:
    proc = subprocess.run(("git", *args), cwd=root, text=True, capture_output=True, check=True)
    return proc.stdout.strip()


class RecoveryTests(unittest.TestCase):
    def make_repo(self, base: Path):
        remote = base / "remote.git"
        work = base / "work"
        subprocess.run(("git", "init", "--bare", str(remote)), check=True, capture_output=True)
        subprocess.run(("git", "clone", str(remote), str(work)), check=True, capture_output=True)
        git(work, "config", "user.email", "test@example.invalid")
        git(work, "config", "user.name", "Recovery Test")
        git(work, "switch", "-c", "worker/h1-04")
        (work / "seed.txt").write_text("seed\n", encoding="utf-8")
        git(work, "add", "seed.txt")
        git(work, "commit", "-m", "seed")
        git(work, "push", "-u", "origin", "worker/h1-04")
        return remote, work, git(work, "rev-parse", "HEAD")

    @staticmethod
    def pr(sha: str, body: str | None = None):
        return {
            "number": 185,
            "state": "open",
            "merged": False,
            "body": body or "Worker state: ACTIVE\nWorker verdict: IN_PROGRESS\nFrozen candidate SHA: NONE\n",
            "head": {"sha": sha, "ref": "worker/h1-04", "repo": {"full_name": REPO}},
        }

    def test_only_active_unfrozen_worker_is_resumable(self):
        sha = "a" * 40
        self.assertTrue(module.is_resumable_worker_pr(self.pr(sha)))
        frozen = self.pr(sha, f"Worker state: ACTIVE\nWorker verdict: IN_PROGRESS\nFrozen candidate SHA: {sha}\n")
        self.assertFalse(module.is_resumable_worker_pr(frozen))
        done = self.pr(sha, "Worker state: COMPLETE\nWorker verdict: REVIEW_READY\nFrozen candidate SHA: NONE\n")
        self.assertFalse(module.is_resumable_worker_pr(done))
        closed = self.pr(sha)
        closed["state"] = "closed"
        self.assertFalse(module.is_resumable_worker_pr(closed))

    def test_recovery_accepts_exact_or_local_ahead_and_dirty(self):
        with tempfile.TemporaryDirectory() as tmp:
            _, work, remote_sha = self.make_repo(Path(tmp))
            exact = module.inspect_resumable_checkout(work, self.pr(remote_sha), REPO)
            self.assertEqual(exact.ahead_commits, 0)
            self.assertEqual(exact.dirty_status, "")

            (work / "material.txt").write_text("checkpoint\n", encoding="utf-8")
            git(work, "add", "material.txt")
            git(work, "commit", "-m", "local material checkpoint")
            (work / "material.txt").write_text("checkpoint\nin progress\n", encoding="utf-8")
            (work / "untracked.bin").write_bytes(b"\x00recovery\xff")
            recovered = module.inspect_resumable_checkout(work, self.pr(remote_sha), REPO)
            self.assertEqual(recovered.ahead_commits, 1)
            self.assertIn("material.txt", recovered.dirty_status)
            self.assertIn("untracked.bin", recovered.dirty_status)

    def test_recovery_refuses_wrong_branch_and_divergence(self):
        with tempfile.TemporaryDirectory() as tmp:
            base = Path(tmp)
            _, work, remote_sha = self.make_repo(base)
            git(work, "switch", "-c", "wrong")
            with self.assertRaisesRegex(module.RecoveryError, "expected worker/h1-04"):
                module.inspect_resumable_checkout(work, self.pr(remote_sha), REPO)

            git(work, "switch", "worker/h1-04")
            # Move the local branch to an unrelated root commit. The remote PR
            # head is no longer an ancestor, so recovery must not choose a side.
            git(work, "checkout", "--orphan", "diverged")
            git(work, "rm", "-rf", ".")
            (work / "other.txt").write_text("other\n", encoding="utf-8")
            git(work, "add", "other.txt")
            git(work, "commit", "-m", "unrelated")
            diverged = git(work, "rev-parse", "HEAD")
            git(work, "branch", "-f", "worker/h1-04", diverged)
            git(work, "switch", "worker/h1-04")
            with self.assertRaisesRegex(module.RecoveryError, "behind or diverged"):
                module.inspect_resumable_checkout(work, self.pr(remote_sha), REPO)

    def test_snapshot_preserves_bundle_tracked_and_untracked_without_mutation(self):
        with tempfile.TemporaryDirectory() as tmp:
            base = Path(tmp)
            _, work, remote_sha = self.make_repo(base)
            (work / "committed.txt").write_text("committed\n", encoding="utf-8")
            git(work, "add", "committed.txt")
            git(work, "commit", "-m", "local unpushed")
            (work / "seed.txt").write_text("seed\ndirty\n", encoding="utf-8")
            (work / "untracked.bin").write_bytes(b"\x00\x01\xff")
            checkout = module.inspect_resumable_checkout(work, self.pr(remote_sha), REPO)
            before_head = git(work, "rev-parse", "HEAD")
            before_status = git(work, "status", "--porcelain=v1", "--untracked-files=all")
            state = base / "state"
            snap = module.snapshot_worktree(work, state, "unit-test", self.pr(remote_sha), checkout)
            self.assertTrue((snap / "committed.bundle").is_file())
            self.assertTrue((snap / "working.patch").is_file())
            self.assertTrue((snap / "remote-to-local.patch").is_file())
            self.assertTrue((snap / "untracked.zip").is_file())
            meta = json.loads((snap / "metadata.json").read_text(encoding="utf-8"))
            self.assertEqual(meta["remote_head"], remote_sha)
            self.assertEqual(meta["ahead_commits"], 1)
            self.assertFalse(meta["authoritative_evidence"])
            with zipfile.ZipFile(snap / "untracked.zip") as archive:
                self.assertEqual(archive.read("untracked.bin"), b"\x00\x01\xff")
            self.assertEqual(git(work, "rev-parse", "HEAD"), before_head)
            self.assertEqual(git(work, "status", "--porcelain=v1", "--untracked-files=all"), before_status)

    def test_remote_adapter_wires_resume_snapshot_and_checkpoint_contract(self):
        adapter = Path(__file__).with_name("local_wp_autopilot_remote.py").read_text(encoding="utf-8")
        self.assertIn("autopilot.main_async = remote_main_async", adapter)
        self.assertIn("recovery.is_resumable_worker_pr", adapter)
        self.assertIn("recovery.inspect_resumable_checkout", adapter)
        self.assertIn("recovery.snapshot_worktree", adapter)
        self.assertIn("recovery.CHECKPOINT_GUIDANCE", adapter)
        self.assertIn("recovery.bootstrap_prompt", adapter)
        self.assertIn('"gpt-6-luna", "high"', adapter)
        self.assertIn("Interrupted Worker recovery did not preserve canonical PR", adapter)

    def test_checkpoint_and_resume_prompts_are_explicit(self):
        guidance = module.CHECKPOINT_GUIDANCE
        self.assertIn("canonical draft PR", guidance)
        bootstrap = module.bootstrap_prompt("H1-04")
        self.assertIn("canonical DRAFT PR", bootstrap)
        self.assertIn("Stop this bootstrap role immediately", bootstrap)
        self.assertIn("Do not start material implementation", bootstrap)
        self.assertIn("Commit locally after every coherent material block", guidance)
        self.assertIn("Do not push every tiny checkpoint", guidance)
        self.assertIn("always before an owner-decision wait", guidance)
        sha = "a" * 40
        checkout = module.RecoveryCheckout(185, "worker/h1-04", sha, sha, 0, "")
        prompt = module.recovery_prompt("H1-04", self.pr(sha), checkout)
        self.assertIn("EXISTING canonical PR #185", prompt)
        self.assertIn("Do not create, close, supersede, or replace", prompt)
        self.assertIn("do not reset/clean/discard", prompt)


if __name__ == "__main__":
    unittest.main()
