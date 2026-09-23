#!/usr/bin/env python3
"""Receive one owner-only Telegram continue click for a frozen Juego2 PR.

Runs in GitHub Actions with the repository's existing Telegram secrets. It
records authorization only; it never reviews, repairs, merges or starts AI.
"""

from __future__ import annotations

import argparse
import json
import os
import re
import subprocess
import time
from datetime import datetime
from urllib.error import URLError
from urllib.request import Request, urlopen

REPO = "Arkus0/Juego2"
SHA_RE = re.compile(r"[0-9a-f]{40}\Z")
REVIEW_ID_RE = re.compile(r"[0-9a-f]{32}\Z")
CONTINUE_OFFER_TTL = 21000


class ReceiverError(Exception):
    pass


def github(path: str, *, body: str | None = None):
    cmd = ["gh", "api", "--paginate", "--slurp", path] if body is None and "?per_page=" in path else ["gh", "api", path]
    if body is not None:
        cmd = ["gh", "api", "--method", "POST", path, "-f", f"body={body}"]
    result = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", check=False)
    if result.returncode:
        raise ReceiverError(f"GitHub request failed ({result.returncode})")
    return json.loads(result.stdout) if result.stdout.strip() else None


def telegram(token: str, method: str, payload: dict):
    request = Request(f"https://api.telegram.org/bot{token}/{method}",
                      data=json.dumps(payload).encode("utf-8"),
                      headers={"Content-Type": "application/json"})
    try:
        with urlopen(request, timeout=45) as response:
            data = json.load(response)
    except (OSError, URLError, ValueError):
        # Never print an exception containing the token-bearing URL.
        raise ReceiverError(f"Telegram {method} failed") from None
    if not data.get("ok"):
        raise ReceiverError(f"Telegram {method} rejected the request")
    return data.get("result")


def flatten_comments(raw):
    if not isinstance(raw, list):
        raise ReceiverError("Unexpected comments response")
    return [item for page in raw for item in page]


def verdict_fields(body: str) -> dict[str, str]:
    result = {}
    for raw in body.splitlines():
        if ":" in raw:
            key, value = raw.split(":", 1)
            result[key.strip().strip("*_#` ").lower()] = value.strip().strip("`*_ .")
    return result


def marker(body: str, state: str, sha: str, fail_count: int) -> bool:
    return ("ARKUS_LOCAL_AUTOPILOT" in body and
            f"State: {state}" in body.splitlines() and
            f"Target SHA: {sha}" in body.splitlines() and
            f"Fail count: {fail_count}" in body.splitlines())


def authorized_click(query: dict, chat_id: int, pr: int, sha: str, fail_count: int) -> bool:
    message = query.get("message") or {}
    chat = message.get("chat") or {}
    sender = query.get("from") or {}
    expected = f"arkus:continue:{pr}:{sha}:{fail_count}"
    return (query.get("data") == expected and chat.get("type") == "private" and
            chat.get("id") == chat_id and sender.get("id") == chat_id)


def fresh_offer(created_at: str, now: float | None = None) -> bool:
    try:
        stamp = datetime.fromisoformat(created_at.replace("Z", "+00:00")).timestamp()
    except (ValueError, TypeError):
        return False
    age = (time.time() if now is None else now) - stamp
    return -120 <= age <= CONTINUE_OFFER_TTL


def validate_current(pr: int, sha: str, fail_count: int) -> str:
    current = github(f"repos/{REPO}/pulls/{pr}")
    if current.get("state") != "open" or current.get("merged") or current.get("draft"):
        raise ReceiverError("PR is no longer open and Ready")
    if current.get("head", {}).get("sha", "").lower() != sha:
        raise ReceiverError("PR HEAD moved")
    body = current.get("body") or ""
    if not re.search(rf"^Frozen candidate SHA:\s*`?{sha}`?\s*$", body, re.MULTILINE | re.IGNORECASE):
        raise ReceiverError("Frozen SHA mismatch")
    raw = github(f"repos/{REPO}/issues/{pr}/comments?per_page=100")
    comments = flatten_comments(raw)
    offers = [item for item in comments if (item.get("user") or {}).get("login") == "Arkus0" and
              marker(item.get("body") or "", "SECOND_FAIL_OFFERED", sha, fail_count)]
    continued = any((item.get("user") or {}).get("login") == "github-actions[bot]" and
                    marker(item.get("body") or "", "OWNER_CONTINUE", sha, fail_count) for item in comments)
    expired = any((item.get("user") or {}).get("login") == "github-actions[bot]" and
                  any(marker(item.get("body") or "", state, sha, fail_count)
                      for state in ("CONTINUE_EXPIRED", "CONTINUE_UNAVAILABLE")) for item in comments)
    if not offers:
        raise ReceiverError("No matching second-FAIL offer")
    if not all(fresh_offer(item.get("created_at", "")) for item in offers):
        raise ReceiverError("Owner-continue offer is stale; PC required")
    if expired:
        raise ReceiverError("Owner-continue decision expired or became unavailable; PC required")
    if continued:
        return "already"
    reviews = flatten_comments(github(f"repos/{REPO}/pulls/{pr}/reviews?per_page=100"))
    verdicts_by_id = {}
    for item in reviews + comments:
        if (item.get("user") or {}).get("login") != "Arkus0":
            continue
        body = item.get("body") or ""
        record = verdict_fields(body)
        reviewed_sha = record.get("reviewed candidate sha", "").lower()
        review_id = record.get("autopilot review id", "").lower()
        verdict = record.get("reviewer verdict", "").upper()
        if verdict in {"PASS", "FAIL"} and SHA_RE.fullmatch(reviewed_sha) and REVIEW_ID_RE.fullmatch(review_id):
            # One tagged independent review is one verdict even if mirrored in a comment.
            if review_id in verdicts_by_id and verdicts_by_id[review_id] != (verdict, reviewed_sha):
                raise ReceiverError("Contradictory duplicate review ID")
            verdicts_by_id[review_id] = (verdict, reviewed_sha)
    count = sum(verdict == "FAIL" for verdict, _ in verdicts_by_id.values())
    if count != fail_count or count < 2 or count >= 4:
        raise ReceiverError("FAIL count changed or fourth-FAIL ceiling reached")
    return "ready"


def receive(pr: int, sha: str, fail_count: int, max_seconds: int) -> str:
    token = os.environ.get("TELEGRAM_BOT_TOKEN", "").strip()
    raw_chat = os.environ.get("TELEGRAM_CHAT_ID", "").strip()
    if not token or not raw_chat.isdecimal() or not SHA_RE.fullmatch(sha) or fail_count not in (2, 3):
        raise ReceiverError("Missing private-chat config or invalid PR/SHA/count")
    chat_id = int(raw_chat)
    if telegram(token, "getWebhookInfo", {}).get("url"):
        raise ReceiverError("Bot has a webhook; getUpdates cannot be used")
    if validate_current(pr, sha, fail_count) == "already":
        return "already"
    deadline = time.monotonic() + max_seconds
    offset = None
    while time.monotonic() < deadline:
        params = {"timeout": 25, "limit": 100, "allowed_updates": ["callback_query"]}
        if offset is not None:
            params["offset"] = offset
        updates = telegram(token, "getUpdates", params)
        if not isinstance(updates, list):
            raise ReceiverError("Unexpected Telegram updates")
        for update in updates:
            offset = max(offset or 0, int(update["update_id"]) + 1)
            query = update.get("callback_query") or {}
            if not authorized_click(query, chat_id, pr, sha, fail_count):
                continue
            try:
                status = validate_current(pr, sha, fail_count)
            except ReceiverError:
                telegram(token, "answerCallbackQuery", {"callback_query_id": query["id"], "text": "Ya no es válido para este PR/SHA.", "show_alert": True})
                continue
            if status == "already":
                telegram(token, "answerCallbackQuery", {"callback_query_id": query["id"], "text": "Ya estaba autorizado."})
                return "already"
            body = ("ARKUS_LOCAL_AUTOPILOT\nState: OWNER_CONTINUE\n"
                    f"Target SHA: {sha}\nFail count: {fail_count}\n"
                    f"Telegram update ID: {update['update_id']}\n"
                    "Detail: owner private-chat button authorized unattended repairs up to but not beyond the fourth FAIL; no PASS or proof claim.\n")
            github(f"repos/{REPO}/issues/{pr}/comments", body=body)
            telegram(token, "answerCallbackQuery", {"callback_query_id": query["id"], "text": "Continuación autorizada. El cuarto FAIL exigirá PC."})
            return "continued"
    return "expired"


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--pr", type=int, required=True)
    parser.add_argument("--sha", required=True)
    parser.add_argument("--fail-count", type=int, required=True)
    parser.add_argument("--max-seconds", type=int, default=20500)
    args = parser.parse_args()
    try:
        result = receive(args.pr, args.sha.lower(), args.fail_count, args.max_seconds)
    except ReceiverError as exc:
        try:
            body = ("ARKUS_LOCAL_AUTOPILOT\nState: CONTINUE_UNAVAILABLE\n"
                    f"Target SHA: {args.sha.lower()}\nFail count: {args.fail_count}\n"
                    f"Detail: {exc}\n")
            github(f"repos/{REPO}/issues/{args.pr}/comments", body=body)
        except ReceiverError:
            pass
        print(f"TELEGRAM_CONTINUE_STOP: {exc}")
        return 2
    if result == "expired":
        try:
            body = ("ARKUS_LOCAL_AUTOPILOT\nState: CONTINUE_EXPIRED\n"
                    f"Target SHA: {args.sha.lower()}\nFail count: {args.fail_count}\n")
            github(f"repos/{REPO}/issues/{args.pr}/comments", body=body)
        except ReceiverError:
            pass
    print(f"TELEGRAM_CONTINUE: {result}")
    return 0 if result in {"continued", "already"} else 3


if __name__ == "__main__":
    raise SystemExit(main())
