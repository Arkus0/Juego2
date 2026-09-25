#!/usr/bin/env python3
"""Safe-output front door for Telegram owner continuation decisions."""

from __future__ import annotations

import importlib.util
from pathlib import Path
import re
import sys
import time
from typing import Any

HERE = Path(__file__).resolve().parent


def _load(name: str, path: Path):
    spec = importlib.util.spec_from_file_location(name, path)
    if spec is None or spec.loader is None:
        raise RuntimeError(f"cannot load {path}")
    module = importlib.util.module_from_spec(spec)
    sys.modules[name] = module
    spec.loader.exec_module(module)
    return module


core = _load("arkus_telegram_continue_receiver_core", HERE / "_telegram_continue_receiver_core.py")
safe = _load("arkus_safe_output_owner", HERE / "arkus_safe_output.py")


def _wp_from_pr(pr_obj: dict[str, Any]) -> str:
    body = pr_obj.get("body") or ""
    matches = re.findall(r"^WP:\s*`?([^`\r\n]+?)`?\s*$", body, re.MULTILINE | re.IGNORECASE)
    if len(matches) != 1:
        raise core.ReceiverError(f"WP must occur exactly once, found {len(matches)}")
    try:
        return safe.normalize_wp(matches[0].strip())
    except safe.SafeOutputError as exc:
        raise core.ReceiverError(f"invalid owner-decision WP binding: {exc}") from exc


def _safe_fail_count(pr: int, wp: str, reviews: list[dict[str, Any]]) -> int:
    by_id: dict[str, tuple[str, str, str]] = {}
    for item in reviews:
        body = item.get("body") or ""
        if safe.SCHEMA not in body:
            continue
        if (item.get("user") or {}).get("login") != "Arkus0":
            raise core.ReceiverError("Reviewer safe output has invalid authority")
        commit_id = (item.get("commit_id") or "").lower()
        try:
            intent = safe.parse_intent(body)
            safe.authorize(intent, kind="REVIEW_VERDICT", role="REVIEWER",
                           wp=wp, pr=pr, candidate_sha=commit_id)
        except safe.SafeOutputError as exc:
            raise core.ReceiverError(f"Reviewer safe output rejected: {exc}") from exc
        payload = intent["payload"]
        rid = payload["review_id"]
        row = (payload["verdict"], intent["candidate_sha"], safe.decision_slot(intent))
        if rid in by_id and by_id[rid] != row:
            raise core.ReceiverError("Contradictory duplicate safe-output review ID")
        by_id[rid] = row
    return sum(verdict == "FAIL" for verdict, _, _ in by_id.values())


def validate_current(pr: int, sha: str, fail_count: int) -> str:
    current = core.github(f"repos/{core.REPO}/pulls/{pr}")
    if current.get("state") != "open" or current.get("merged") or current.get("draft"):
        raise core.ReceiverError("PR is no longer open and Ready")
    if current.get("head", {}).get("sha", "").lower() != sha:
        raise core.ReceiverError("PR HEAD moved")
    body = current.get("body") or ""
    if not re.search(rf"^Frozen candidate SHA:\s*`?{sha}`?\s*$", body, re.MULTILINE | re.IGNORECASE):
        raise core.ReceiverError("Frozen SHA mismatch")
    wp = _wp_from_pr(current)

    raw = core.github(f"repos/{core.REPO}/issues/{pr}/comments?per_page=100")
    comments = core.flatten_comments(raw)
    offers = [item for item in comments if (item.get("user") or {}).get("login") == "Arkus0" and
              core.marker(item.get("body") or "", "SECOND_FAIL_OFFERED", sha, fail_count)]
    continued = any((item.get("user") or {}).get("login") == "github-actions[bot]" and
                    core.marker(item.get("body") or "", "OWNER_CONTINUE", sha, fail_count) for item in comments)
    expired = any((item.get("user") or {}).get("login") == "github-actions[bot]" and
                  any(core.marker(item.get("body") or "", state, sha, fail_count)
                      for state in ("CONTINUE_EXPIRED", "CONTINUE_UNAVAILABLE")) for item in comments)
    if not offers:
        raise core.ReceiverError("No matching second-FAIL offer")
    if not all(core.fresh_offer(item.get("created_at", "")) for item in offers):
        raise core.ReceiverError("Owner-continue offer is stale; PC required")
    if expired:
        raise core.ReceiverError("Owner-continue decision expired or became unavailable; PC required")
    if continued:
        return "already"

    reviews = core.flatten_comments(core.github(f"repos/{core.REPO}/pulls/{pr}/reviews?per_page=100"))
    count = _safe_fail_count(pr, wp, reviews)
    if count != fail_count or count < 2 or count >= 4:
        raise core.ReceiverError("safe-output FAIL count changed or fourth-FAIL ceiling reached")
    return "ready"


def receive(pr: int, sha: str, fail_count: int, max_seconds: int) -> str:
    token = core.os.environ.get("TELEGRAM_BOT_TOKEN", "").strip()
    raw_chat = core.os.environ.get("TELEGRAM_CHAT_ID", "").strip()
    if not token or not raw_chat.isdecimal() or not core.SHA_RE.fullmatch(sha) or fail_count not in (2, 3):
        raise core.ReceiverError("Missing private-chat config or invalid PR/SHA/count")
    chat_id = int(raw_chat)
    if core.telegram(token, "getWebhookInfo", {}).get("url"):
        raise core.ReceiverError("Bot has a webhook; getUpdates cannot be used")
    if validate_current(pr, sha, fail_count) == "already":
        return "already"
    deadline = time.monotonic() + max_seconds
    offset = None
    while time.monotonic() < deadline:
        params: dict[str, Any] = {"timeout": 25, "limit": 100, "allowed_updates": ["callback_query"]}
        if offset is not None:
            params["offset"] = offset
        updates = core.telegram(token, "getUpdates", params)
        if not isinstance(updates, list):
            raise core.ReceiverError("Unexpected Telegram updates")
        for update in updates:
            offset = max(offset or 0, int(update["update_id"]) + 1)
            query = update.get("callback_query") or {}
            if not core.authorized_click(query, chat_id, pr, sha, fail_count):
                continue
            try:
                status = validate_current(pr, sha, fail_count)
            except core.ReceiverError:
                core.telegram(token, "answerCallbackQuery", {"callback_query_id": query["id"], "text": "Ya no es válido para este PR/SHA.", "show_alert": True})
                continue
            if status == "already":
                core.telegram(token, "answerCallbackQuery", {"callback_query_id": query["id"], "text": "Ya estaba autorizado."})
                return "already"

            current = core.github(f"repos/{core.REPO}/pulls/{pr}")
            wp = _wp_from_pr(current)
            source_id = update["update_id"]
            line = safe.render_owner(wp=wp, pr=pr, candidate_sha=sha,
                                     fail_count=fail_count, source_id=source_id)
            try:
                intent = safe.owner_from_body(line, wp=wp, pr=pr, candidate_sha=sha,
                                              fail_count=fail_count, source_id=source_id)
            except safe.SafeOutputError as exc:
                raise core.ReceiverError(f"owner safe output rejected: {exc}") from exc
            body = ("ARKUS_LOCAL_AUTOPILOT\nState: OWNER_CONTINUE\n"
                    f"Target SHA: {sha}\nFail count: {fail_count}\n"
                    f"Telegram update ID: {source_id}\n"
                    f"Safe output intent key: {safe.intent_key(intent)}\n"
                    f"Safe output decision slot: {safe.decision_slot(intent)}\n"
                    f"{line}\n"
                    "Detail: owner private-chat button authorized unattended repairs up to but not beyond the fourth FAIL; no PASS or proof claim.\n")
            core.github(f"repos/{core.REPO}/issues/{pr}/comments", body=body)
            core.telegram(token, "answerCallbackQuery", {"callback_query_id": query["id"], "text": "Continuación autorizada. El cuarto FAIL exigirá PC."})
            return "continued"
    return "expired"


core.validate_current = validate_current
core.receive = receive


def main() -> int:
    return core.main()


if __name__ == "__main__":
    raise SystemExit(main())
