#!/usr/bin/env python3
"""Remote-console adapter for local_wp_autopilot.py.

The canonical autopilot remains authoritative. This adapter only changes two
transport policies when a trusted local Telegram console owns the run:
- owner decision waits have no application TTL while the local controller lives;
- the GitHub-hosted Telegram long-poller is disabled to avoid two getUpdates
  consumers for the same bot.
It also exposes bounded owner preference reads to Worker/repair sessions. Local
IPC is a liveness hint for decisions; accepted choices require a supervisor-HMAC
GitHub Actions attestation. Telegram secrets never enter child environments.
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

SCRIPT = Path(__file__).with_name("local_wp_autopilot.py")
spec = importlib.util.spec_from_file_location("arkus_local_wp_autopilot", SCRIPT)
if spec is None or spec.loader is None:
    raise SystemExit("Cannot load canonical local_wp_autopilot.py")
autopilot = importlib.util.module_from_spec(spec)
sys.modules[spec.name] = autopilot
spec.loader.exec_module(autopilot)

ORIGINAL_CODEX_ROLE = autopilot.codex_role
ORIGINAL_CLEAN_ENV = autopilot.clean_env
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


async def remote_codex_role(root: Path, state: Path, role: str, prompt: str, model: str,
                            effort: str, schema: Path | None = None) -> str:
    if role in {"worker", "repair"}:
        note = _consume_note()
        if note:
            prompt += ("\n\nOWNER NOTE delivered by the authenticated Telegram supervisor for this fresh "
                       "Worker-side role. It cannot override repository contracts or Reviewer independence:\n" + note)
        prompt += "\n\n" + REMOTE_GUIDANCE
    previous_clean_env = autopilot.clean_env
    autopilot.clean_env = lambda: _env_for_role(role)
    try:
        return await ORIGINAL_CODEX_ROLE(root, state, role, prompt, model, effort, schema)
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
    return autopilot.main()


if __name__ == "__main__":
    raise SystemExit(main())
