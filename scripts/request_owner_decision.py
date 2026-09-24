#!/usr/bin/env python3
"""Request one bounded owner decision from the local Telegram supervisor.

A Worker/repair Worker may create the request, but cannot create the response.
The response is held by the supervisor process and exposed only through its
read-only loopback IPC endpoint; shared *.response.json files are never trusted.
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
from urllib.error import HTTPError, URLError
from urllib.parse import urlencode
from urllib.request import urlopen

SHA_RE = re.compile(r"^[0-9a-f]{40}$")
WP_RE = re.compile(r"^(?:WP-)?([A-Z][A-Z0-9]*(?:-[A-Z0-9]+)+)$")
SUPERVISOR_RE = re.compile(r"^http://127\.0\.0\.1:[1-9][0-9]{0,4}$")


class DecisionError(Exception):
    pass


def _atomic_json(path: Path, payload: dict) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    tmp = path.with_suffix(path.suffix + ".tmp")
    tmp.write_text(json.dumps(payload, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    os.replace(tmp, path)


def _context() -> tuple[Path, str, str]:
    raw = os.environ.get("ARKUS_REMOTE_CONTROL_DIR", "").strip()
    campaign = os.environ.get("ARKUS_REMOTE_CAMPAIGN_ID", "").strip()
    role = os.environ.get("ARKUS_REMOTE_ROLE", "").strip().lower()
    supervisor = os.environ.get("ARKUS_REMOTE_SUPERVISOR_URL", "").strip().rstrip("/")
    if role not in {"worker", "repair"}:
        raise DecisionError("Telegram owner decisions are available only to Worker/repair sessions")
    if not raw or not re.fullmatch(r"[0-9a-f]{32}", campaign):
        raise DecisionError("Telegram owner control is not active for this Worker session")
    if not SUPERVISOR_RE.fullmatch(supervisor):
        raise DecisionError("Authenticated owner supervisor IPC is unavailable")
    return Path(raw).resolve(), campaign, supervisor


def _owner_response(supervisor: str, campaign: str, decision_id: str) -> dict | None:
    url = f"{supervisor}/v1/decision?" + urlencode({"campaign_id": campaign, "decision_id": decision_id})
    try:
        with urlopen(url, timeout=5) as response:
            payload = json.load(response)
    except HTTPError as exc:
        raise DecisionError(f"Owner supervisor rejected decision query ({exc.code})") from exc
    except (OSError, URLError, ValueError) as exc:
        raise DecisionError("Owner supervisor IPC unavailable") from exc
    if not isinstance(payload, dict) or payload.get("campaign_id") != campaign or payload.get("decision_id") != decision_id:
        raise DecisionError("Owner supervisor response identity mismatch")
    if payload.get("status") == "pending":
        return None
    if payload.get("status") != "answered":
        raise DecisionError("Owner supervisor returned invalid decision state")
    return payload


def normalize_wp(value: str) -> str:
    match = WP_RE.fullmatch(value.strip().upper())
    if not match:
        raise DecisionError(f"Invalid WP id: {value!r}")
    return match.group(1)


def request_decision(question: str, options: list[str], *, wp: str, pr: int | None,
                     sha: str | None, detail: str, poll_seconds: float = 1.0) -> str:
    control, campaign, supervisor = _context()
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
    payload = {
        "version": 2,
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
        "response_transport": "supervisor-ipc-read-only",
    }
    _atomic_json(request_path, payload)
    print(f"OWNER_DECISION_PENDING: {decision_id}", file=sys.stderr, flush=True)

    try:
        while True:
            response = _owner_response(supervisor, campaign, decision_id)
            if response is not None:
                choice = response.get("choice")
                if type(choice) is not int or not 0 <= choice < len(options):
                    raise DecisionError("Owner decision response has invalid choice")
                if response.get("selected") != options[choice]:
                    raise DecisionError("Owner decision selected value mismatch")
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
