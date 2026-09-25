#!/usr/bin/env python3
"""Deterministic capability firewall for Arkus agent/owner outputs.

Human-readable prose is never authority here.  A privileged caller must carry
exactly one ARKUS_INTENT_V1 JSON line and that line is checked against trusted
runtime context (canonical WP/PR/current SHA and the expected role/source).

This module is deliberately small and side-effect free.  GitHub/Telegram
adapters remain responsible for proving their external source and for the
actual mutation after this broker authorizes the intent.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import re
from typing import Any

SCHEMA = "ARKUS_INTENT_V1"
PREFIX = SCHEMA + " "
SHA_RE = re.compile(r"^[0-9a-f]{40}$")
REVIEW_ID_RE = re.compile(r"^[0-9a-f]{32}$")
WP_RE = re.compile(r"^(?:WP-)?([A-Z][A-Z0-9]*(?:-[A-Z0-9]+)+)$")
REVIEW_VERDICTS = {"PASS", "FAIL", "PROTOCOL_FIX", "REVIEW_BLOCKED"}


class SafeOutputError(ValueError):
    pass


def normalize_wp(value: str) -> str:
    match = WP_RE.fullmatch((value or "").strip().upper())
    if not match:
        raise SafeOutputError(f"invalid WP identity: {value!r}")
    return "WP-" + match.group(1)


def campaign_id(wp: str, pr: int) -> str:
    if type(pr) is not int or pr < 1:
        raise SafeOutputError("PR must be a positive integer")
    return f"arkus:{normalize_wp(wp)}:pr:{pr}"


def _canonical_json(value: dict[str, Any]) -> str:
    return json.dumps(value, sort_keys=True, separators=(",", ":"), ensure_ascii=True)


def intent_key(intent: dict[str, Any]) -> str:
    return hashlib.sha256(_canonical_json(intent).encode("utf-8")).hexdigest()


def decision_slot(intent: dict[str, Any]) -> str:
    """Identity of the one decision position this intent occupies.

    Review IDs deliberately participate in a review slot: Arkus permits one
    independently-authorized same-SHA appeal with a fresh review ID and the
    lifecycle controller owns that sequencing rule.  Owner continuation has
    exactly one slot for a candidate/fail-count pair.
    """
    payload = intent["payload"]
    if intent["kind"] == "REVIEW_VERDICT":
        raw = [intent["campaign_id"], intent["candidate_sha"], intent["kind"], payload["review_id"]]
    else:
        raw = [intent["campaign_id"], intent["candidate_sha"], intent["kind"], str(payload["fail_count"])]
    return hashlib.sha256("\0".join(raw).encode("utf-8")).hexdigest()


def parse_intent(body: str) -> dict[str, Any]:
    lines = [line for line in (body or "").splitlines() if line.startswith(PREFIX)]
    if len(lines) != 1:
        raise SafeOutputError(f"exactly one {SCHEMA} line required, found {len(lines)}")
    raw = lines[0][len(PREFIX):]
    if len(raw.encode("utf-8")) > 4096:
        raise SafeOutputError("safe-output intent exceeds 4096 bytes")
    try:
        value = json.loads(raw)
    except json.JSONDecodeError as exc:
        raise SafeOutputError(f"invalid {SCHEMA} JSON") from exc
    if not isinstance(value, dict):
        raise SafeOutputError("safe-output intent must be a JSON object")
    required = {"schema", "kind", "role", "campaign_id", "wp", "pr", "candidate_sha", "payload"}
    if set(value) != required:
        raise SafeOutputError(f"intent keys must be exactly {sorted(required)}")
    if value["schema"] != SCHEMA:
        raise SafeOutputError("intent schema mismatch")
    value["wp"] = normalize_wp(value["wp"])
    if type(value["pr"]) is not int or value["pr"] < 1:
        raise SafeOutputError("intent PR must be a positive integer")
    if not isinstance(value["candidate_sha"], str) or not SHA_RE.fullmatch(value["candidate_sha"].lower()):
        raise SafeOutputError("intent candidate SHA must be exact 40-hex")
    value["candidate_sha"] = value["candidate_sha"].lower()
    if value["campaign_id"] != campaign_id(value["wp"], value["pr"]):
        raise SafeOutputError("intent campaign binding mismatch")
    if not isinstance(value["payload"], dict):
        raise SafeOutputError("intent payload must be an object")

    kind = value["kind"]
    role = value["role"]
    payload = value["payload"]
    if kind == "REVIEW_VERDICT":
        if role != "REVIEWER":
            raise SafeOutputError("REVIEW_VERDICT requires REVIEWER role")
        if set(payload) != {"verdict", "review_id"}:
            raise SafeOutputError("review payload must contain only verdict and review_id")
        verdict = str(payload["verdict"]).upper()
        review_id = str(payload["review_id"]).lower()
        if verdict not in REVIEW_VERDICTS or not REVIEW_ID_RE.fullmatch(review_id):
            raise SafeOutputError("invalid review verdict identity")
        payload["verdict"] = verdict
        payload["review_id"] = review_id
    elif kind == "OWNER_DECISION":
        if role != "OWNER":
            raise SafeOutputError("OWNER_DECISION requires OWNER role")
        if set(payload) != {"action", "fail_count", "source_id"}:
            raise SafeOutputError("owner payload must contain only action, fail_count and source_id")
        if payload["action"] != "CONTINUE":
            raise SafeOutputError("unsupported owner decision")
        if type(payload["fail_count"]) is not int or payload["fail_count"] not in (2, 3):
            raise SafeOutputError("owner continuation fail_count must be 2 or 3")
        if not isinstance(payload["source_id"], (str, int)) or str(payload["source_id"]).strip() == "":
            raise SafeOutputError("owner source_id is required")
        payload["source_id"] = str(payload["source_id"])
    else:
        raise SafeOutputError(f"unsupported intent kind: {kind!r}")
    return value


def authorize(intent: dict[str, Any], *, kind: str, role: str, wp: str, pr: int,
              candidate_sha: str, review_id: str | None = None,
              fail_count: int | None = None, source_id: str | int | None = None) -> dict[str, Any]:
    """Bind a parsed intent to trusted runtime facts; return it if authorized."""
    expected_wp = normalize_wp(wp)
    expected_sha = (candidate_sha or "").lower()
    if not SHA_RE.fullmatch(expected_sha):
        raise SafeOutputError("trusted candidate SHA is invalid")
    expected_campaign = campaign_id(expected_wp, pr)
    checks = {
        "kind": (intent.get("kind"), kind),
        "role": (intent.get("role"), role),
        "wp": (intent.get("wp"), expected_wp),
        "pr": (intent.get("pr"), pr),
        "campaign_id": (intent.get("campaign_id"), expected_campaign),
        "candidate_sha": (intent.get("candidate_sha"), expected_sha),
    }
    for name, (actual, expected) in checks.items():
        if actual != expected:
            raise SafeOutputError(f"trusted {name} mismatch: {actual!r} != {expected!r}")
    payload = intent["payload"]
    if kind == "REVIEW_VERDICT" and review_id is not None:
        if payload["review_id"] != review_id.lower():
            raise SafeOutputError("trusted review ID mismatch")
    if kind == "OWNER_DECISION":
        if fail_count is None or payload["fail_count"] != fail_count:
            raise SafeOutputError("trusted owner fail_count mismatch")
        if source_id is None or payload["source_id"] != str(source_id):
            raise SafeOutputError("trusted owner source ID mismatch")
    return intent


def review_from_body(body: str, *, wp: str, pr: int, candidate_sha: str,
                     review_id: str | None = None) -> dict[str, Any]:
    return authorize(parse_intent(body), kind="REVIEW_VERDICT", role="REVIEWER",
                     wp=wp, pr=pr, candidate_sha=candidate_sha, review_id=review_id)


def owner_from_body(body: str, *, wp: str, pr: int, candidate_sha: str,
                    fail_count: int, source_id: str | int) -> dict[str, Any]:
    return authorize(parse_intent(body), kind="OWNER_DECISION", role="OWNER",
                     wp=wp, pr=pr, candidate_sha=candidate_sha,
                     fail_count=fail_count, source_id=source_id)


def render_review(*, wp: str, pr: int, candidate_sha: str, verdict: str, review_id: str) -> str:
    intent = {
        "schema": SCHEMA, "kind": "REVIEW_VERDICT", "role": "REVIEWER",
        "campaign_id": campaign_id(wp, pr), "wp": normalize_wp(wp), "pr": pr,
        "candidate_sha": candidate_sha.lower(),
        "payload": {"verdict": verdict.upper(), "review_id": review_id.lower()},
    }
    parse_intent(PREFIX + _canonical_json(intent))
    return PREFIX + _canonical_json(intent)


def render_owner(*, wp: str, pr: int, candidate_sha: str, fail_count: int,
                 source_id: str | int) -> str:
    intent = {
        "schema": SCHEMA, "kind": "OWNER_DECISION", "role": "OWNER",
        "campaign_id": campaign_id(wp, pr), "wp": normalize_wp(wp), "pr": pr,
        "candidate_sha": candidate_sha.lower(),
        "payload": {"action": "CONTINUE", "fail_count": fail_count, "source_id": str(source_id)},
    }
    parse_intent(PREFIX + _canonical_json(intent))
    return PREFIX + _canonical_json(intent)


def self_test() -> None:
    sha = "a" * 40
    other = "b" * 40
    rid = "c" * 32
    wp, pr = "WP-H1-07", 204
    line = render_review(wp=wp, pr=pr, candidate_sha=sha, verdict="FAIL", review_id=rid)
    body = "human rationale\n" + line + "\nmore prose"
    intent = review_from_body(body, wp=wp, pr=pr, candidate_sha=sha, review_id=rid)
    assert intent["payload"]["verdict"] == "FAIL"
    assert len(intent_key(intent)) == 64 and len(decision_slot(intent)) == 64

    def rejected(fn) -> None:
        try:
            fn()
        except SafeOutputError:
            return
        raise AssertionError("negative control unexpectedly authorized")

    rejected(lambda: review_from_body(body, wp=wp, pr=pr, candidate_sha=other, review_id=rid))
    rejected(lambda: review_from_body(body, wp="WP-H1-08", pr=pr, candidate_sha=sha, review_id=rid))
    rejected(lambda: review_from_body(body, wp=wp, pr=pr + 1, candidate_sha=sha, review_id=rid))
    rejected(lambda: parse_intent(body + "\n" + line))
    spoof = line.replace('"role":"REVIEWER"', '"role":"WORKER"')
    rejected(lambda: parse_intent(spoof))
    contradiction = render_review(wp=wp, pr=pr, candidate_sha=sha, verdict="PASS", review_id=rid)
    assert decision_slot(parse_intent(contradiction)) == decision_slot(intent)
    assert intent_key(parse_intent(contradiction)) != intent_key(intent)

    owner = render_owner(wp=wp, pr=pr, candidate_sha=sha, fail_count=2, source_id=9182)
    owner_intent = owner_from_body(owner, wp=wp, pr=pr, candidate_sha=sha, fail_count=2, source_id=9182)
    assert owner_intent["payload"]["action"] == "CONTINUE"
    rejected(lambda: owner_from_body(owner, wp=wp, pr=pr, candidate_sha=sha, fail_count=3, source_id=9182))
    rejected(lambda: owner_from_body(owner, wp=wp, pr=pr, candidate_sha=sha, fail_count=2, source_id=9183))
    rejected(lambda: owner_from_body(owner, wp=wp, pr=pr, candidate_sha=other, fail_count=2, source_id=9182))


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--self-test", action="store_true")
    args = parser.parse_args()
    if args.self_test:
        self_test()
        print("ARKUS_SAFE_OUTPUT_SELF_TEST: PASS")
        return 0
    parser.error("only --self-test is a public CLI; adapters import this module")
    return 2


if __name__ == "__main__":
    raise SystemExit(main())
