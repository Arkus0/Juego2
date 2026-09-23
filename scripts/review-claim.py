#!/usr/bin/env python3
"""Exact-SHA Reviewer claim lease policy.

GitHub Actions is the only writer of REVIEW_CLAIMED / REVIEW_CLAIM_RELEASED.
Reviewer sessions may request/release a lease but never self-grant one. Leases
exist only to prevent duplicate review work; they are never semantic verdicts.
"""
from __future__ import annotations

import argparse
from datetime import datetime, timezone
import json
from pathlib import Path
import re
import sys

BOT = "github-actions[bot]"
SHA_RE = re.compile(r"^[0-9a-f]{40}$", re.I)
STATE_RE = re.compile(r"^State:\s*(REVIEW_CLAIMED|REVIEW_CLAIM_RELEASED)\s*$", re.M)


def field(body: str, name: str) -> str | None:
    values = re.findall(rf"^{re.escape(name)}:\s*(.+?)\s*$", body, re.M)
    if len(values) > 1:
        raise ValueError(f"{name}: ambiguous field count {len(values)}")
    return values[0].strip() if values else None


def parse_time(value: str) -> datetime:
    parsed = datetime.fromisoformat(value.replace("Z", "+00:00"))
    if parsed.tzinfo is None:
        raise ValueError("lease timestamp must include timezone")
    return parsed.astimezone(timezone.utc)


def bot_owned(comment: dict) -> bool:
    return str(((comment.get("user") or {}).get("login") or "")) == BOT


def active_claim(comments: list[dict], target_sha: str, now: datetime) -> dict | None:
    target = target_sha.lower()
    claims: dict[str, dict] = {}
    for comment in sorted(comments, key=lambda item: int(item.get("id") or 0)):
        if not bot_owned(comment):
            continue
        body = str(comment.get("body") or "")
        state = STATE_RE.search(body)
        if state is None:
            continue
        if (field(body, "Target SHA") or "").lower() != target:
            continue
        claim_id = field(body, "Claim ID") or ""
        if not claim_id:
            continue
        if state.group(1) == "REVIEW_CLAIM_RELEASED":
            claims.pop(claim_id, None)
            continue
        expires_raw = field(body, "Expires At")
        if not expires_raw:
            continue
        try:
            expires = parse_time(expires_raw)
        except ValueError:
            continue
        if expires <= now:
            continue
        claims[claim_id] = {
            "claim_id": claim_id,
            "expires_at": expires,
            "comment_id": int(comment.get("id") or 0),
        }
    return max(claims.values(), key=lambda x: x["comment_id"]) if claims else None


def decide(comments: list[dict], request_id: str, target_sha: str, now: datetime) -> dict[str, str]:
    if not request_id.isdigit() or int(request_id) <= 0:
        raise ValueError("request id must be a positive GitHub comment id")
    if not SHA_RE.fullmatch(target_sha):
        raise ValueError("target SHA must be exact 40-hex")
    active = active_claim(comments, target_sha, now)
    if active is None or active["claim_id"] == request_id:
        return {
            "decision": "GRANT",
            "claim_id": request_id,
            "active_claim_id": active["claim_id"] if active else "NONE",
            "active_expires_at": active["expires_at"].isoformat().replace("+00:00", "Z") if active else "NONE",
        }
    return {
        "decision": "DENY",
        "claim_id": request_id,
        "active_claim_id": active["claim_id"],
        "active_expires_at": active["expires_at"].isoformat().replace("+00:00", "Z"),
    }


def can_release(comments: list[dict], claim_id: str, target_sha: str, now: datetime) -> bool:
    active = active_claim(comments, target_sha, now)
    return bool(active and active["claim_id"] == claim_id)


def bot_comment(comment_id: int, body: str) -> dict:
    return {"id": comment_id, "user": {"login": BOT}, "body": body}


def self_test() -> None:
    now = datetime(2026, 9, 23, 3, 0, tzinfo=timezone.utc)
    sha = "a" * 40
    claim = f"ARKUS_AUTOMATION_V2\nState: REVIEW_CLAIMED\nClaim ID: 100\nTarget SHA: {sha}\nExpires At: 2026-09-23T04:00:00Z\n"
    active = bot_comment(10, claim)
    denied = decide([active], "101", sha, now)
    assert denied["decision"] == "DENY" and denied["active_claim_id"] == "100"
    assert can_release([active], "100", sha, now)

    forged = {"id": 11, "user": {"login": "Arkus0"}, "body": claim}
    assert decide([forged], "101", sha, now)["decision"] == "GRANT"

    expired = bot_comment(10, claim.replace("04:00:00", "02:59:59"))
    assert decide([expired], "101", sha, now)["decision"] == "GRANT"

    released = bot_comment(12, f"ARKUS_AUTOMATION_V2\nState: REVIEW_CLAIM_RELEASED\nClaim ID: 100\nTarget SHA: {sha}\n")
    assert decide([active, released], "101", sha, now)["decision"] == "GRANT"
    assert not can_release([active, released], "100", sha, now)

    other_sha = bot_comment(10, claim.replace(sha, "b" * 40))
    assert decide([other_sha], "101", sha, now)["decision"] == "GRANT"
    print("REVIEW_CLAIM_SELF_TEST_GREEN")


def main() -> int:
    parser = argparse.ArgumentParser()
    sub = parser.add_subparsers(dest="command", required=True)
    sub.add_parser("self-test")
    p = sub.add_parser("decide")
    p.add_argument("--comments-json", required=True)
    p.add_argument("--request-id", required=True)
    p.add_argument("--target-sha", required=True)
    p.add_argument("--now", required=True)
    p.add_argument("--github-output")
    r = sub.add_parser("can-release")
    r.add_argument("--comments-json", required=True)
    r.add_argument("--claim-id", required=True)
    r.add_argument("--target-sha", required=True)
    r.add_argument("--now", required=True)
    args = parser.parse_args()
    try:
        if args.command == "self-test":
            self_test()
            return 0
        comments = json.loads(Path(args.comments_json).read_text(encoding="utf-8"))
        if not isinstance(comments, list):
            raise ValueError("comments JSON must be a list")
        now = parse_time(args.now)
        if args.command == "can-release":
            print("YES" if can_release(comments, args.claim_id, args.target_sha.lower(), now) else "NO")
            return 0
        result = decide(comments, args.request_id, args.target_sha.lower(), now)
        if args.github_output:
            with Path(args.github_output).open("a", encoding="utf-8") as out:
                for key, value in result.items():
                    out.write(f"{key}={value}\n")
        print(json.dumps(result, sort_keys=True))
        return 0
    except (ValueError, OSError, json.JSONDecodeError) as exc:
        print(f"REVIEW_CLAIM_ERROR: {exc}", file=sys.stderr)
        return 2


if __name__ == "__main__":
    raise SystemExit(main())
