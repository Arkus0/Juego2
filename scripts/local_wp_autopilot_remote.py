#!/usr/bin/env python3
"""Remote-console adapter for local_wp_autopilot.py.

The canonical autopilot remains authoritative. This adapter only changes
transport/liveness policies when a trusted local Telegram console owns the run:
- owner decision waits have no application TTL while the local controller lives;
- the GitHub-hosted Telegram long-poller is disabled to avoid two getUpdates
  consumers for the same bot;
- interrupted ACTIVE/IN_PROGRESS Workers resume on their same canonical PR;
- Worker-side sessions use durable, bounded commit/push checkpoints.

It also exposes bounded owner preference reads to Worker/repair sessions. Local
IPC and recovery snapshots are liveness aids only; accepted choices still require
a supervisor-HMAC GitHub Actions attestation, and recovery artifacts are never
PASS/acceptance evidence. Telegram secrets never enter child environments.
"""

from __future__ import annotations

import asyncio
import importlib.util
import json
import os
import re
import sys
from datetime import datetime, timezone
from pathlib import Path
from urllib.error import URLError
from urllib.parse import urlencode
from urllib.request import urlopen

import local_wp_worker_recovery as recovery

SCRIPT = Path(__file__).with_name("local_wp_autopilot.py")
spec = importlib.util.spec_from_file_location("arkus_local_wp_autopilot", SCRIPT)
if spec is None or spec.loader is None:
    raise SystemExit("Cannot load canonical local_wp_autopilot.py")
autopilot = importlib.util.module_from_spec(spec)
sys.modules[spec.name] = autopilot
spec.loader.exec_module(autopilot)

ORIGINAL_CODEX_ROLE = autopilot.codex_role
ORIGINAL_CLEAN_ENV = autopilot.clean_env
ORIGINAL_MAIN_ASYNC = autopilot.main_async
REMOTE_ENV_KEYS = ("ARKUS_REMOTE_CONTROL_DIR", "ARKUS_REMOTE_CAMPAIGN_ID",
                   "ARKUS_REMOTE_ROLE", "ARKUS_REMOTE_SUPERVISOR_URL")

REMOTE_GUIDANCE = """
REMOTE OWNER CONTROL (transport only): this Worker/repair session is running under the opt-in local Telegram console. If, and only if, progress is blocked on a bounded owner preference that does not waive evidence, acceptance criteria, Reviewer independence, security, exact-SHA integrity, or a required physical-PC observation, you may ask exactly 2 or 3 concrete alternatives with:
  python scripts/request_owner_decision.py --wp <WP> --pr <PR> --sha <HEAD_SHA> --question "..." --option "..." --option "..." [--option "..."] --detail "why owner preference is needed"
Wait for the command to return and continue using exactly the selected option. The helper accepts a choice only after github-actions[bot] attests a supervisor-only HMAC for the exact campaign/request/PR/SHA/choice. Do not use this mechanism for architectural uncertainty that requires a human investigation, unavailable mandatory evidence, fourth FAIL, REVIEW_BLOCKED, or any condition that the accepted protocol says must stop; those remain BLOCKED/HUMAN_ACTION_REQUIRED. Never ask the owner to choose a Reviewer verdict.
""".strip()


def _control_dir() -> Path:
    raw = os.environ.get("ARKUS_REMOTE_CONTROL_DIR", "").strip()
    if not raw:
        raise autopilot.StopFlow("Remote control directory missing")
    return Path(raw).resolve()


def _campaign() -> str:
    campaign = os.environ.get("ARKUS_REMOTE_CAMPAIGN_ID", "").strip()
    if not re.fullmatch(r"[0-9a-f]{32}", campaign):
        raise autopilot.StopFlow("Remote campaign identity missing")
    return campaign


def _supervisor_url() -> str:
    url = os.environ.get("ARKUS_REMOTE_SUPERVISOR_URL", "").strip().rstrip("/")
    if not re.fullmatch(r"http://127\.0\.0\.1:[1-9][0-9]{0,4}", url):
        raise autopilot.StopFlow("Trusted loopback owner supervisor endpoint missing")
    return url


def _supervisor_json(path: str, **params: str) -> dict:
    query = urlencode(params)
    try:
        with urlopen(f"{_supervisor_url()}{path}?{query}", timeout=5) as response:
            payload = json.load(response)
    except (OSError, URLError, ValueError) as exc:
        raise autopilot.StopFlow("Owner supervisor IPC unavailable") from exc
    if not isinstance(payload, dict):
        raise autopilot.StopFlow("Owner supervisor IPC returned malformed data")
    return payload


def _atomic_json(path: Path, payload: dict) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    tmp = path.with_suffix(path.suffix + ".tmp")
    tmp.write_text(json.dumps(payload, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    os.replace(tmp, path)


def _consume_note() -> str:
    campaign = _campaign()
    payload = _supervisor_json("/v1/note", campaign_id=campaign)
    if payload.get("status") != "ok" or payload.get("campaign_id") != campaign:
        raise autopilot.StopFlow("Owner note provenance mismatch")
    note = payload.get("note", "")
    if not isinstance(note, str):
        raise autopilot.StopFlow("Owner note payload malformed")
    return note.strip()[:4000]


def _env_for_role(role: str) -> dict[str, str]:
    env = dict(ORIGINAL_CLEAN_ENV())
    if role in {"worker", "repair"}:
        env["ARKUS_REMOTE_ROLE"] = role
        return env
    for name in REMOTE_ENV_KEYS:
        env.pop(name, None)
    return env


def _best_effort_snapshot(root: Path, state: Path, reason: str) -> Path | None:
    """Preserve current local bytes without turning salvage into authority."""
    try:
        snapshot = recovery.snapshot_worktree(root, state, reason)
        print(f"LOCAL_RECOVERY_SNAPSHOT: {snapshot}", flush=True)
        return snapshot
    except (OSError, recovery.RecoveryError) as exc:
        # Never mask the causal Worker failure with a secondary salvage failure.
        print(f"LOCAL_RECOVERY_SNAPSHOT_WARNING: {exc}", file=sys.stderr, flush=True)
        return None


async def remote_codex_role(root: Path, state: Path, role: str, prompt: str, model: str,
                            effort: str, schema: Path | None = None,
                            assets_root: Path | None = None) -> str:
    if role in {"worker", "repair"}:
        note = _consume_note()
        if note:
            prompt += ("\n\nOWNER NOTE delivered by the authenticated Telegram supervisor for this fresh "
                       "Worker-side role. It cannot override repository contracts or Reviewer independence:\n" + note)
        prompt += "\n\n" + REMOTE_GUIDANCE
        prompt += "\n\n" + recovery.CHECKPOINT_GUIDANCE
    previous_clean_env = autopilot.clean_env
    autopilot.clean_env = lambda: _env_for_role(role)
    try:
        try:
            return await ORIGINAL_CODEX_ROLE(root, state, role, prompt, model, effort, schema,
                                             assets_root)
        except autopilot.StopFlow:
            if role in {"worker", "repair"}:
                _best_effort_snapshot(root, state, f"{role}-session-failed")
            raise
    finally:
        autopilot.clean_env = previous_clean_env


async def wait_for_owner_continue_forever(pr: int, sha: str, fail_count: int, timeout: int = 0) -> None:
    del timeout
    campaign = _campaign()
    pending = _control_dir() / "pending-owner-continue.json"
    payload = {
        "version": 1,
        "campaign_id": campaign,
        "pr": pr,
        "sha": sha,
        "fail_count": fail_count,
        "created_at": datetime.now(timezone.utc).isoformat(),
    }
    _atomic_json(pending, payload)
    try:
        while True:
            current = autopilot.gh_json("api", f"repos/{autopilot.REPO}/pulls/{pr}")
            if current.get("state") != "open" or current.get("head", {}).get("sha", "").lower() != sha:
                raise autopilot.StopFlow("PR changed while awaiting Telegram owner decision")
            rows = autopilot.local_markers(pr)
            if any(row.get("state") == "OWNER_CONTINUE" and row.get("target sha") == sha and
                   row.get("fail count") == str(fail_count) and row.get("campaign id") == campaign
                   for row in rows):
                return
            await asyncio.sleep(30)
    finally:
        try:
            if pending.exists():
                current = json.loads(pending.read_text(encoding="utf-8"))
                if current.get("campaign_id") == campaign and current.get("pr") == pr:
                    pending.unlink()
        except (OSError, json.JSONDecodeError):
            pass


def no_hosted_receiver(pr: int, sha: str, fail_count: int) -> None:
    del pr, sha, fail_count


def _resume_candidate(root: Path, wp: str) -> tuple[dict, recovery.RecoveryCheckout] | None:
    pr = autopilot.canonical_pr(wp)
    if not recovery.is_resumable_worker_pr(pr):
        return None
    # canonical_pr may have been obtained from a paginated list. Re-read the PR
    # immediately before admission so branch/head/body state is fresh.
    current = autopilot.gh_json("api", f"repos/{autopilot.REPO}/pulls/{pr['number']}")
    if not recovery.is_resumable_worker_pr(current):
        return None
    try:
        checkout = recovery.inspect_resumable_checkout(root, current, autopilot.REPO)
    except recovery.RecoveryError as exc:
        raise autopilot.StopFlow(str(exc)) from exc
    return current, checkout


async def remote_main_async(args) -> None:
    """Resume interrupted remote Workers before canonical post-Worker handling.

    The canonical controller deliberately remains strict once a Worker has sealed a
    candidate. This shim only intercepts the one liveness state that is otherwise
    ambiguous: a single open canonical PR explicitly owned by an ACTIVE/IN_PROGRESS
    Worker with no frozen candidate yet.
    """
    if args.dry_run or not args.wp:
        await ORIGINAL_MAIN_ASYNC(args)
        return

    root = Path(args.root).resolve()
    state = Path(args.state).resolve()
    autopilot.require_repo(root)
    assets_root = autopilot.resolve_assets_root(root, args.assets_root)
    wp = autopilot.normalize_wp(args.wp)
    resume = _resume_candidate(root, wp)
    if resume is None:
        existing = autopilot.canonical_pr(wp)
        if existing is not None:
            # A non-resumable existing PR belongs to the canonical lifecycle; never
            # reinterpret a frozen/blocked/reviewing state as Worker recovery.
            await ORIGINAL_MAIN_ASYNC(args)
            return

        # Guarantee durable ownership before the expensive Worker. The bootstrap is
        # deliberately a separate bounded turn: if quota/session death happens later,
        # there is already a canonical PR/branch to adopt.
        autopilot.prepare_new_wp(root)
        wp_file = autopilot.wp_path(root, wp)
        wp_text = wp_file.read_text(encoding="utf-8")
        autopilot.assert_dependencies(root, wp_text)
        async with autopilot.AppServer() as app:
            await app.assert_models()
            await autopilot.quota_before_reasoning(app, wp, None)
            await autopilot.codex_role(
                root, state, "worker", recovery.bootstrap_prompt(wp),
                "gpt-6-luna", "high", assets_root=assets_root)
        created = autopilot.canonical_pr(wp)
        if not created or created.get("state") != "open":
            snapshot = _best_effort_snapshot(root, state, "worker-bootstrap-ended-without-pr")
            suffix = f"; work preserved at {snapshot}" if snapshot else ""
            raise autopilot.StopFlow(
                f"Worker bootstrap ended without a canonical open PR for {wp}{suffix}")
        current = autopilot.gh_json("api", f"repos/{autopilot.REPO}/pulls/{created['number']}")
        if not recovery.is_resumable_worker_pr(current):
            raise autopilot.StopFlow(
                f"New canonical PR #{created['number']} is not ACTIVE/IN_PROGRESS after Worker bootstrap")
        try:
            checkout = recovery.inspect_resumable_checkout(root, current, autopilot.REPO)
        except recovery.RecoveryError as exc:
            raise autopilot.StopFlow(str(exc)) from exc
    else:
        current, checkout = resume
    try:
        snapshot = recovery.snapshot_worktree(root, state, "pre-worker-resume", current, checkout)
    except (OSError, recovery.RecoveryError) as exc:
        raise autopilot.StopFlow(f"Cannot preserve interrupted Worker before resume: {exc}") from exc
    print(f"LOCAL_RECOVERY_SNAPSHOT: {snapshot}", flush=True)

    wp_file = autopilot.wp_path(root, wp)
    wp_text = wp_file.read_text(encoding="utf-8")
    autopilot.assert_dependencies(root, wp_text)

    async with autopilot.AppServer() as app:
        await app.assert_models()
        await autopilot.quota_before_reasoning(app, wp, current["number"])
        await autopilot.codex_role(
            root, state, "worker", recovery.recovery_prompt(wp, current, checkout),
            "gpt-6-sol", autopilot.effort_for_worker(wp_text), assets_root=assets_root)

    survivor = autopilot.canonical_pr(wp)
    if not survivor or survivor.get("state") != "open" or survivor.get("number") != current["number"]:
        raise autopilot.StopFlow(
            f"Interrupted Worker recovery did not preserve canonical PR #{current['number']} for {wp}")
    survivor = autopilot.gh_json("api", f"repos/{autopilot.REPO}/pulls/{survivor['number']}")
    if recovery.is_resumable_worker_pr(survivor):
        try:
            after = recovery.inspect_resumable_checkout(root, survivor, autopilot.REPO)
            snapshot = recovery.snapshot_worktree(root, state, "worker-resume-returned-in-progress",
                                                  survivor, after)
        except (OSError, recovery.RecoveryError) as exc:
            raise autopilot.StopFlow(
                f"Recovered Worker returned still ACTIVE/IN_PROGRESS and salvage failed: {exc}") from exc
        raise autopilot.StopFlow(
            f"Recovered Worker returned while PR #{survivor['number']} is still ACTIVE/IN_PROGRESS; "
            f"work preserved at {snapshot}")

    # Once the Worker has sealed/blocked the candidate, hand control back to the
    # unchanged canonical lifecycle (Reviewer/repair/merge/DocSync exact-SHA rules).
    await ORIGINAL_MAIN_ASYNC(args)


def main() -> int:
    try:
        _control_dir()
        _campaign()
        _supervisor_url()
    except autopilot.StopFlow as exc:
        print(f"REMOTE_AUTOPILOT_STOP: {exc}", file=sys.stderr)
        return 2
    autopilot.fresh_offer = lambda created_at, now=None: True
    autopilot.dispatch_continue_receiver = no_hosted_receiver
    autopilot.wait_for_owner_continue = wait_for_owner_continue_forever
    autopilot.codex_role = remote_codex_role
    autopilot.main_async = remote_main_async
    return autopilot.main()


if __name__ == "__main__":
    raise SystemExit(main())
