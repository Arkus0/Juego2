#!/usr/bin/env python3
"""Fail-closed recovery helpers for interrupted local WP Worker sessions.

Recovery artifacts are local liveness/salvage data only. They are never PASS
or acceptance evidence and never mutate the repository checkout.
"""

from __future__ import annotations

import json
import os
import re
import subprocess
import zipfile
from dataclasses import dataclass
from datetime import datetime, timezone
from pathlib import Path
from typing import Any

SHA_RE = re.compile(r"^[0-9a-f]{40}$")

CHECKPOINT_GUIDANCE = """
DURABLE WORKER CHECKPOINT CONTRACT (liveness only; never PASS evidence):
- After the bounded dependency/authority bootstrap, establish the canonical draft PR and push its branch BEFORE expensive implementation, Unity/test execution, or broad source inspection. Do not wait until the workpack is nearly complete to create the PR.
- Once that canonical PR exists, keep using that exact PR/branch. Never create a replacement PR merely because a Worker session or quota window ends.
- Commit and push after every coherent material block (for example adoption/provenance, implementation/API, mapping/discovery, tests, effective proof/evidence) and before any long-running/expensive operation. A WIP checkpoint may be incomplete and makes no acceptance claim.
- Before invoking an owner-decision wait, first commit and push every coherent repository change that is already understood. Do not leave a large completed block only in the working tree while waiting for Telegram.
- Do not manufacture tiny/noise commits just to satisfy cadence. The goal is bounded loss: a killed Codex session may lose at most the current small in-progress block, not the previous material blocks.
- Never reset, clean, discard, overwrite, or silently replace pre-existing recovery bytes. Reconcile them explicitly and preserve provenance.
""".strip()


class RecoveryError(RuntimeError):
    pass


@dataclass(frozen=True)
class RecoveryCheckout:
    pr: int
    branch: str
    remote_head: str
    local_head: str
    ahead_commits: int
    dirty_status: str


def _fields(body: str) -> dict[str, str]:
    out: dict[str, str] = {}
    for line in body.splitlines():
        if ":" in line:
            key, value = line.split(":", 1)
            out[key.strip().strip("*_#` ").lower()] = value.strip().strip("`*_ .")
    return out


def is_resumable_worker_pr(pr: dict[str, Any] | None) -> bool:
    if not pr or pr.get("state") != "open" or pr.get("merged") or pr.get("merged_at"):
        return False
    body = _fields(pr.get("body") or "")
    frozen = body.get("frozen candidate sha", "").lower()
    return (body.get("worker state", "").upper() == "ACTIVE" and
            body.get("worker verdict", "").upper() == "IN_PROGRESS" and
            not SHA_RE.fullmatch(frozen))


def bootstrap_prompt(wp: str) -> str:
    return (
        f"$implement-workpack Bootstrap Worker ownership for {wp}. Perform only the bounded "
        "dependency/authority bootstrap needed to establish durable ownership. Create the "
        "canonical Worker branch, write/commit the predecessor-contract evidence required by "
        "the protocol if it is not already available, push the branch, and open the one "
        "canonical DRAFT PR with Worker state ACTIVE, Worker verdict IN_PROGRESS, and Frozen "
        "candidate SHA NONE. Stop this bootstrap role immediately after that PR exists. Do not "
        "start material implementation, Unity execution, third-party asset import/adoption, "
        "broad source inspection, validation, Reviewer work, or another WP in this bootstrap "
        "role. The controller will launch the expensive Worker only after the PR is durable.\n\n"
        + CHECKPOINT_GUIDANCE
    )


def recovery_prompt(wp: str, pr: dict[str, Any], checkout: RecoveryCheckout) -> str:
    return (
        f"$implement-workpack Resume interrupted Worker {wp} on the EXISTING canonical PR "
        f"#{pr['number']}. Do not create, close, supersede, or replace that PR. Continue on "
        f"branch {checkout.branch!r}. The GitHub PR head at recovery admission is "
        f"{checkout.remote_head}; the preserved local HEAD is {checkout.local_head} "
        f"({checkout.ahead_commits} local commit(s) ahead). The checkout may intentionally "
        "contain dirty/untracked bytes from the interrupted session. Inspect and reconcile "
        "those bytes before editing; do not reset/clean/discard them. Preserve valid prior "
        "work, promptly commit+push each coherent recovered/material block to the SAME PR, "
        "then continue the authoritative workpack. Finish through the normal REVIEW_READY "
        "handoff, or emit the protocol-required BLOCKED/HUMAN_ACTION_REQUIRED state. Do not "
        "review the PR and do not start another WP.\n\n" + CHECKPOINT_GUIDANCE
    )


def _git(root: Path, *args: str, check: bool = True, text: bool = True) -> subprocess.CompletedProcess:
    proc = subprocess.run(
        ("git", *args), cwd=root, capture_output=True, check=False,
        text=text, encoding="utf-8" if text else None, errors="replace" if text else None,
    )
    if check and proc.returncode:
        stderr = proc.stderr if text else proc.stderr.decode("utf-8", "replace")
        raise RecoveryError(f"git {' '.join(args)} failed ({proc.returncode}): {stderr[-600:]}")
    return proc


def inspect_resumable_checkout(root: Path, pr: dict[str, Any], repo_full_name: str) -> RecoveryCheckout:
    """Admit only the canonical branch at remote HEAD or a local fast-forward of it.

    Dirty/untracked bytes and unpushed local commits are intentionally allowed here.
    Behind/diverged/wrong-branch states remain fail-closed so recovery cannot silently
    choose between two authorities.
    """
    root = root.resolve()
    head = pr.get("head") or {}
    repo = head.get("repo") or {}
    remote_head = (head.get("sha") or "").lower()
    branch = head.get("ref") or ""
    number = pr.get("number")
    if (repo.get("full_name") != repo_full_name or not isinstance(number, int) or
            not branch or not SHA_RE.fullmatch(remote_head)):
        raise RecoveryError(f"PR #{number or '?'} has no same-repository canonical recovery head")

    # Fetching the named head cannot mutate the worktree. It makes the exact GitHub
    # head object available locally before the ancestry decision.
    _git(root, "fetch", "origin", branch)
    local_branch = _git(root, "branch", "--show-current").stdout.strip()
    local_head = _git(root, "rev-parse", "HEAD").stdout.strip().lower()
    if local_branch != branch:
        raise RecoveryError(
            f"PR #{number} recovery checkout is on {local_branch or 'detached HEAD'}, expected {branch}")
    if not SHA_RE.fullmatch(local_head):
        raise RecoveryError(f"PR #{number} recovery checkout has invalid local HEAD")

    ancestor = _git(root, "merge-base", "--is-ancestor", remote_head, local_head, check=False)
    if ancestor.returncode == 1:
        raise RecoveryError(
            f"PR #{number} recovery checkout is behind or diverged from GitHub head; manual reconciliation required")
    if ancestor.returncode != 0:
        raise RecoveryError(f"PR #{number} could not prove remote-head ancestry")

    ahead_raw = _git(root, "rev-list", "--count", f"{remote_head}..{local_head}").stdout.strip()
    try:
        ahead = int(ahead_raw)
    except ValueError as exc:
        raise RecoveryError(f"PR #{number} recovery ahead-count is malformed: {ahead_raw!r}") from exc
    dirty = _git(root, "status", "--porcelain=v1", "--untracked-files=all").stdout.rstrip()
    return RecoveryCheckout(number, branch, remote_head, local_head, ahead, dirty)


def _safe_state_root(root: Path, state: Path) -> tuple[Path, Path]:
    root = root.resolve()
    state = state.resolve()
    if state == root or root in state.parents:
        raise RecoveryError("Recovery state directory must remain outside the repository checkout")
    return root, state


def _write_git_bytes(root: Path, path: Path, *args: str) -> None:
    proc = _git(root, *args, text=False)
    path.write_bytes(proc.stdout)


def _untracked_paths(root: Path) -> list[str]:
    raw = _git(root, "ls-files", "--others", "--exclude-standard", "-z", text=False).stdout
    names = []
    for item in raw.split(b"\0"):
        if not item:
            continue
        name = os.fsdecode(item)
        candidate = Path(name)
        if candidate.is_absolute() or ".." in candidate.parts:
            raise RecoveryError(f"Unsafe untracked recovery path: {name!r}")
        names.append(name)
    return names


def snapshot_worktree(root: Path, state: Path, reason: str,
                      pr: dict[str, Any] | None = None,
                      checkout: RecoveryCheckout | None = None) -> Path:
    """Save a non-authoritative salvage bundle without modifying the checkout."""
    root, state = _safe_state_root(root, state)
    recovery_root = state / "recovery"
    recovery_root.mkdir(parents=True, exist_ok=True)
    stamp = datetime.now(timezone.utc).strftime("%Y%m%dT%H%M%S.%fZ")
    pr_number = checkout.pr if checkout else (pr or {}).get("number")
    label = f"pr{pr_number}" if pr_number else "unowned"
    target = recovery_root / f"{stamp}-{label}"
    target.mkdir(parents=False, exist_ok=False)

    branch = _git(root, "branch", "--show-current").stdout.strip()
    head = _git(root, "rev-parse", "HEAD").stdout.strip().lower()
    status = _git(root, "status", "--porcelain=v1", "--untracked-files=all").stdout.rstrip()
    remote_head = checkout.remote_head if checkout else ""
    ahead = checkout.ahead_commits if checkout else None

    metadata = {
        "version": 1,
        "created_at": datetime.now(timezone.utc).isoformat(),
        "reason": reason,
        "repository_root": str(root),
        "pr": pr_number,
        "branch": branch,
        "local_head": head,
        "remote_head": remote_head,
        "ahead_commits": ahead,
        "status": status,
        "authoritative_evidence": False,
    }
    (target / "metadata.json").write_text(
        json.dumps(metadata, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")

    # A bundle makes even committed-but-unpushed objects recoverable if a later
    # operator accidentally moves the branch. Dirty bytes are captured separately.
    _git(root, "bundle", "create", str(target / "committed.bundle"), "HEAD")
    _write_git_bytes(root, target / "working.patch", "diff", "--binary", "HEAD")
    if remote_head and remote_head != head:
        _write_git_bytes(root, target / "remote-to-local.patch", "diff", "--binary", f"{remote_head}..{head}")

    untracked = _untracked_paths(root)
    if untracked:
        with zipfile.ZipFile(target / "untracked.zip", "w", compression=zipfile.ZIP_DEFLATED) as archive:
            for name in untracked:
                source = root / name
                if source.is_file():
                    archive.write(source, arcname=name)
        (target / "untracked.txt").write_text("\n".join(untracked) + "\n", encoding="utf-8")

    (target / "RESTORE.txt").write_text(
        "LOCAL RECOVERY ARTIFACT — NOT PASS/ACCEPTANCE EVIDENCE\n"
        "Inspect metadata.json first. Restore only into an explicit recovery clone/worktree.\n"
        "Committed objects: git clone committed.bundle <dir>\n"
        "Tracked/index/worktree delta: inspect/apply working.patch\n"
        "Untracked bytes (if present): inspect untracked.zip before extraction\n",
        encoding="utf-8",
    )
    return target
