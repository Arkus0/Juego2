#!/usr/bin/env python3
"""Remote-console adapter for local_wp_autopilot.py.

The canonical autopilot remains authoritative. This adapter only changes two
transport policies when a trusted local Telegram console owns the run:
- owner decision waits have no application TTL while the local controller lives;
- the GitHub-hosted Telegram long-poller is disabled to avoid two getUpdates
  consumers for the same bot.
It also exposes the bounded owner-decision helper to Worker/repair sessions.
"""

from __future__ import annotations

import asyncio
import importlib.util
import os
import re
import sys
from pathlib import Path

SCRIPT = Path(__file__).with_name("local_wp_autopilot.py")
spec = importlib.util.spec_from_file_location("arkus_local_wp_autopilot", SCRIPT)
if spec is None or spec.loader is None:
    raise SystemExit("Cannot load canonical local_wp_autopilot.py")
autopilot = importlib.util.module_from_spec(spec)
sys.modules[spec.name] = autopilot
spec.loader.exec_module(autopilot)

ORIGINAL_CODEX_ROLE = autopilot.codex_role

REMOTE_GUIDANCE = """
REMOTE OWNER CONTROL (transport only): this Worker/repair session is running under the opt-in local Telegram console. If, and only if, progress is blocked on a bounded owner preference that does not waive evidence, acceptance criteria, Reviewer independence, security, exact-SHA integrity, or a required physical-PC observation, you may ask exactly 2 or 3 concrete alternatives with:
  python scripts/request_owner_decision.py --wp <WP> [--pr <PR>] [--sha <HEAD_SHA>] --question "..." --option "..." --option "..." [--option "..."] --detail "why owner preference is needed"
Wait for the command to return and continue using exactly the selected option. Do not use this mechanism for architectural uncertainty that requires a human investigation, unavailable mandatory evidence, fourth FAIL, REVIEW_BLOCKED, or any condition that the accepted protocol says must stop; those remain BLOCKED/HUMAN_ACTION_REQUIRED. Never ask the owner to choose a Reviewer verdict.
""".strip()


def _consume_note() -> str:
    raw = os.environ.get("ARKUS_REMOTE_CONTROL_DIR", "").strip()
    if not raw:
        return ""
    path = Path(raw) / "pending-owner-note.txt"
    if not path.exists():
        return ""
    try:
        note = path.read_text(encoding="utf-8").strip()
        path.unlink()
    except OSError:
        return ""
    return note[:4000]


async def remote_codex_role(root: Path, state: Path, role: str, prompt: str, model: str,
                            effort: str, schema: Path | None = None) -> str:
    if role in {"worker", "repair"}:
        note = _consume_note()
        if note:
            prompt += ("\n\nOWNER NOTE delivered by the authenticated Telegram console for this fresh "
                       "Worker-side role. It cannot override repository contracts or Reviewer independence:\n" + note)
        prompt += "\n\n" + REMOTE_GUIDANCE
    return await ORIGINAL_CODEX_ROLE(root, state, role, prompt, model, effort, schema)


async def wait_for_owner_continue_forever(pr: int, sha: str, fail_count: int, timeout: int = 0) -> None:
    del timeout
    while True:
        current = autopilot.gh_json("api", f"repos/{autopilot.REPO}/pulls/{pr}")
        if current.get("state") != "open" or current.get("head", {}).get("sha", "").lower() != sha:
            raise autopilot.StopFlow("PR changed while awaiting Telegram owner decision")
        rows = autopilot.local_markers(pr)
        if any(row.get("state") == "OWNER_CONTINUE" and row.get("target sha") == sha and
               row.get("fail count") == str(fail_count) for row in rows):
            return
        await asyncio.sleep(30)


def no_hosted_receiver(pr: int, sha: str, fail_count: int) -> None:
    del pr, sha, fail_count


def main() -> int:
    if not os.environ.get("ARKUS_REMOTE_CONTROL_DIR") or not re.fullmatch(
            r"[0-9a-f]{32}", os.environ.get("ARKUS_REMOTE_CAMPAIGN_ID", "")):
        print("REMOTE_AUTOPILOT_STOP: trusted local Telegram console environment missing", file=sys.stderr)
        return 2
    autopilot.fresh_offer = lambda created_at, now=None: True
    autopilot.dispatch_continue_receiver = no_hosted_receiver
    autopilot.wait_for_owner_continue = wait_for_owner_continue_forever
    autopilot.codex_role = remote_codex_role
    return autopilot.main()


if __name__ == "__main__":
    raise SystemExit(main())
