#!/usr/bin/env python3
"""Request one bounded owner decision from the local Telegram supervisor.

A Worker/repair Worker may create the request, but cannot authenticate its own
answer. Loopback IPC is only a liveness hint: the accepted choice must also be
attested by github-actions[bot] after GitHub verifies the supervisor-only HMAC
for the exact campaign/request/PR/SHA/choice tuple.
"""

from __future__ import annotations

import argparse
import json
import os
import re
import ssl
import sys
import time
import uuid
from datetime import datetime, timezone
from pathlib import Path
from urllib.error import HTTPError, URLError
from urllib.parse import urlencode
from urllib.request import HTTPSHandler, ProxyHandler, Request, build_opener, urlopen

import owner_control_auth as owner_auth

REPO = "Arkus0/Juego2"
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
    """Read the local supervisor response as a non-authoritative liveness hint."""
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


def _github_page(url: str) -> tuple[list[dict], str]:
    context = ssl.create_default_context()
    opener = build_opener(ProxyHandler({}), HTTPSHandler(context=context))
    request = Request(url, headers={
        "Accept": "application/vnd.github+json",
        "User-Agent": "Arkus-local-owner-decision",
        "X-GitHub-Api-Version": "2022-11-28",
    })
    try:
        with opener.open(request, timeout=10) as response:
            payload = json.load(response)
            link = response.headers.get("Link", "")
    except (OSError, URLError, ValueError) as exc:
        raise DecisionError("GitHub owner-decision attestation unavailable") from exc
    if not isinstance(payload, list):
        raise DecisionError("GitHub owner-decision attestation malformed")
    return payload, link


def _github_comments(pr: int) -> list[dict]:
    base = f"https://api.github.com/repos/{REPO}/issues/{pr}/comments?per_page=100"
    first, link = _github_page(base)
    match = re.search(r'<([^>]+)>;\s*rel="last"', link)
    if not match or match.group(1) == base:
        return first
    last, _ = _github_page(match.group(1))
    return last


def _marker_fields(body: str) -> dict[str, str]:
    fields: dict[str, str] = {}
    for raw in body.splitlines():
        if ":" not in raw:
            continue
        key, value = raw.split(":", 1)
        fields[key.strip().lower()] = value.strip()
    return fields


def _github_owner_attestation(pr: int, sha: str, campaign: str, decision_id: str,
                              request_digest: str, options: list[str]) -> int | None:
    for comment in reversed(_github_comments(pr)):
        user = comment.get("user") or {}
        if user.get("login") != "github-actions[bot]" or user.get("type") != "Bot":
            continue
        body = comment.get("body") or ""
        if "ARKUS_LOCAL_AUTOPILOT" not in body.splitlines():
            continue
        fields = _marker_fields(body)
        if (fields.get("state") != "OWNER_DECISION" or
                fields.get("target sha", "").lower() != sha or
                fields.get("campaign id", "").lower() != campaign or
                fields.get("decision id", "").lower() != decision_id or
                fields.get("request digest", "").lower() != request_digest or
                fields.get("authority proof") != "supervisor-HMAC-v1"):
            continue
        try:
            choice = int(fields.get("choice", ""))
        except ValueError:
            continue
        if not 0 <= choice < len(options):
            continue
        try:
            selected_digest = owner_auth.decision_selected_digest(options[choice])
        except owner_auth.OwnerProofError:
            continue
        if fields.get("selected digest", "").lower() != selected_digest:
            continue
        return choice
    return None


def _verified_owner_response(supervisor: str, campaign: str, decision_id: str, pr: int,
                             sha: str, request_digest: str, options: list[str]) -> dict | None:
    # A substituted loopback endpoint may claim any answer. That claim is never
    # authority; only the bot-authored GitHub attestation can select the option.
    try:
        _owner_response(supervisor, campaign, decision_id)
    except DecisionError:
        pass
    choice = _github_owner_attestation(pr, sha, campaign, decision_id, request_digest, options)
    if choice is None:
        return None
    return {"choice": choice, "selected": options[choice]}


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
    if pr is None or pr < 1:
        raise DecisionError("Authenticated owner decisions require an exact positive PR")
    if sha is None:
        raise DecisionError("Authenticated owner decisions require the exact PR HEAD SHA")
    sha = sha.lower()
    if not SHA_RE.fullmatch(sha):
        raise DecisionError("SHA must be an exact 40-hex commit")

    decision_id = uuid.uuid4().hex
    try:
        request_digest = owner_auth.decision_request_digest(
            campaign, decision_id, wp, pr, sha, question, detail, options)
    except owner_auth.OwnerProofError as exc:
        raise DecisionError(str(exc)) from exc
    request_path = control / "decisions" / f"{decision_id}.json"
    payload = {
        "version": 3,
        "decision_id": decision_id,
        "campaign_id": campaign,
        "request_digest": request_digest,
        "created_at": datetime.now(timezone.utc).isoformat(),
        "wp": wp,
        "pr": pr,
        "sha": sha,
        "question": question,
        "detail": detail,
        "options": options,
        "pid": os.getpid(),
        "response_transport": "github-actions-bot-attestation",
    }
    _atomic_json(request_path, payload)
    print(f"OWNER_DECISION_PENDING: {decision_id}", file=sys.stderr, flush=True)

    try:
        while True:
            response = _verified_owner_response(
                supervisor, campaign, decision_id, pr, sha, request_digest, options)
            if response is not None:
                choice = response.get("choice")
                if type(choice) is not int or not 0 <= choice < len(options):
                    raise DecisionError("Owner decision attestation has invalid choice")
                if response.get("selected") != options[choice]:
                    raise DecisionError("Owner decision attestation selected value mismatch")
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
    parser.add_argument("--pr", type=int, required=True)
    parser.add_argument("--sha", required=True)
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
