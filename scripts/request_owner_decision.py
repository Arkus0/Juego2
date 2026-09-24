#!/usr/bin/env python3
"""Request one bounded owner decision from the local Telegram remote console.

This helper never talks to Telegram or GitHub directly. A Worker/repair Worker
invokes it only when ARKUS_REMOTE_CONTROL_DIR is present. It writes one durable
local request, waits without an application-level timeout, and prints the exact
owner-selected option. Reviewer independence is intentionally outside scope.
"""

from __future__ import annotations

import argparse
import json
import os
import re
import sys
import time
import uuid
from datetime import datetime, timezone
from pathlib import Path

SHA_RE = re.compile(r"^[0-9a-f]{40}$")
WP_RE = re.compile(r"^(?:WP-)?([A-Z][A-Z0-9]*(?:-[A-Z0-9]+)+)$")


class DecisionError(Exception):
    pass


def _atomic_json(path: Path, payload: dict) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    tmp = path.with_suffix(path.suffix + ".tmp")
    tmp.write_text(json.dumps(payload, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    os.replace(tmp, path)


def _control_dir() -> Path:
    raw = os.environ.get("ARKUS_REMOTE_CONTROL_DIR", "").strip()
    campaign = os.environ.get("ARKUS_REMOTE_CAMPAIGN_ID", "").strip()
    if not raw or not re.fullmatch(r"[0-9a-f]{32}", campaign):
        raise DecisionError("Telegram owner control is not active for this Worker session")
    return Path(raw).resolve()


def normalize_wp(value: str) -> str:
    match = WP_RE.fullmatch(value.strip().upper())
    if not match:
        raise DecisionError(f"Invalid WP id: {value!r}")
    return match.group(1)


def request_decision(question: str, options: list[str], *, wp: str, pr: int | None,
                     sha: str | None, detail: str, poll_seconds: float = 1.0) -> str:
    control = _control_dir()
    campaign = os.environ["ARKUS_REMOTE_CAMPAIGN_ID"].strip()
    question = question.strip()
    detail = detail.strip()
    options = [item.strip() for item in options]
    if not question or len(question) > 800:
        raise DecisionError("Question must contain 1..800 characters")
    if not (2 <= len(options) <= 3) or any(not item or len(item) > 240 for item in options):
        raise DecisionError("Owner decision requires exactly 2 or 3 non-empty options (max 240 chars each)")
    if len(set(options)) != len(options):
        raise DecisionError("Owner decision options must be distinct")
    wp = normalize_wp(wp)
    if pr is not None and pr < 1:
        raise DecisionError("PR must be a positive integer")
    if sha is not None:
        sha = sha.lower()
        if not SHA_RE.fullmatch(sha):
            raise DecisionError("SHA must be an exact 40-hex commit")

    decision_id = uuid.uuid4().hex
    request_path = control / "decisions" / f"{decision_id}.json"
    response_path = control / "decisions" / f"{decision_id}.response.json"
    payload = {
        "version": 1,
        "decision_id": decision_id,
        "campaign_id": campaign,
        "created_at": datetime.now(timezone.utc).isoformat(),
        "wp": wp,
        "pr": pr,
        "sha": sha,
        "question": question,
        "detail": detail[:1200],
        "options": options,
        "pid": os.getpid(),
    }
    _atomic_json(request_path, payload)
    print(f"OWNER_DECISION_PENDING: {decision_id}", file=sys.stderr, flush=True)

    try:
        while True:
            if response_path.exists():
                try:
                    response = json.loads(response_path.read_text(encoding="utf-8"))
                except (OSError, json.JSONDecodeError) as exc:
                    raise DecisionError("Owner decision response is malformed") from exc
                if response.get("decision_id") != decision_id or response.get("campaign_id") != campaign:
                    raise DecisionError("Owner decision response identity mismatch")
                choice = response.get("choice")
                if type(choice) is not int or not 0 <= choice < len(options):
                    raise DecisionError("Owner decision response has invalid choice")
                payload["completed_at"] = datetime.now(timezone.utc).isoformat()
                payload["selected_index"] = choice
                _atomic_json(request_path, payload)
                return options[choice]
            time.sleep(max(0.2, poll_seconds))
    except KeyboardInterrupt as exc:
        payload["abandoned_at"] = datetime.now(timezone.utc).isoformat()
        _atomic_json(request_path, payload)
        raise DecisionError("Owner decision wait interrupted") from exc


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--question", required=True)
    parser.add_argument("--option", action="append", required=True, dest="options")
    parser.add_argument("--wp", required=True)
    parser.add_argument("--pr", type=int)
    parser.add_argument("--sha")
    parser.add_argument("--detail", default="")
    args = parser.parse_args()
    try:
        selected = request_decision(args.question, args.options, wp=args.wp, pr=args.pr,
                                    sha=args.sha, detail=args.detail)
    except DecisionError as exc:
        print(f"OWNER_DECISION_ERROR: {exc}", file=sys.stderr)
        return 2
    print(selected)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
