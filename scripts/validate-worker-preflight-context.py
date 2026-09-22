#!/usr/bin/env python3
"""Validate and describe the exact PR+SHA context of delegated Worker preflight.

The delegated preflight is allowed to prove execution only for the pull request event that
created the run. It must never be reusable as evidence for another PR or another candidate
SHA, even if repository contents happen to be identical.
"""

from __future__ import annotations

import argparse
import json
from pathlib import Path
import re
import sys
from typing import Any

SHA_RE = re.compile(r"^[0-9a-f]{40}$")


def validate_event(event: dict[str, Any], checkout_sha: str, expected_repository: str) -> tuple[int, str]:
    errors: list[str] = []

    repository = ((event.get("repository") or {}).get("full_name") or "").strip()
    pr = event.get("pull_request") or {}
    number = event.get("number")
    head_sha = ((pr.get("head") or {}).get("sha") or "").strip().lower()
    checkout_sha = checkout_sha.strip().lower()

    if repository != expected_repository:
        errors.append(f"repository mismatch: event={repository!r} expected={expected_repository!r}")
    if not isinstance(number, int) or number <= 0:
        errors.append("event is not bound to a positive pull request number")
    if not SHA_RE.fullmatch(head_sha):
        errors.append("pull_request.head.sha is missing or not an exact 40-character SHA")
    if not SHA_RE.fullmatch(checkout_sha):
        errors.append("checked-out HEAD is not an exact 40-character SHA")
    if head_sha and checkout_sha and head_sha != checkout_sha:
        errors.append(f"candidate mismatch: event head={head_sha} checkout={checkout_sha}")

    if errors:
        raise ValueError("; ".join(errors))
    return number, head_sha


def self_test() -> None:
    sha_a = "a" * 40
    sha_b = "b" * 40
    good = {
        "number": 140,
        "repository": {"full_name": "Arkus0/Juego2"},
        "pull_request": {"head": {"sha": sha_a}},
    }

    number, sha = validate_event(good, sha_a, "Arkus0/Juego2")
    assert number == 140 and sha == sha_a

    bad_cases = [
        ({**good, "number": None}, sha_a, "Arkus0/Juego2", "missing PR"),
        (good, sha_b, "Arkus0/Juego2", "stale/wrong candidate"),
        (good, sha_a, "Other/Repo", "cross-repository reuse"),
        ({**good, "pull_request": {}}, sha_a, "Arkus0/Juego2", "missing head SHA"),
    ]
    for event, checkout, repository, name in bad_cases:
        try:
            validate_event(event, checkout, repository)
        except ValueError:
            continue
        raise AssertionError(f"self-test unexpectedly accepted {name}")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--self-test", action="store_true")
    parser.add_argument("--event-path", type=Path)
    parser.add_argument("--checkout-sha")
    parser.add_argument("--expected-repository", default="Arkus0/Juego2")
    parser.add_argument("--run-id")
    parser.add_argument("--run-attempt")
    args = parser.parse_args()

    if args.self_test:
        self_test()
        print("WORKER_PREFLIGHT_CONTEXT_SELF_TEST_GREEN")
        return 0

    if not args.event_path or not args.checkout_sha:
        parser.error("--event-path and --checkout-sha are required outside --self-test")

    try:
        event = json.loads(args.event_path.read_text(encoding="utf-8"))
        number, candidate_sha = validate_event(event, args.checkout_sha, args.expected_repository)
    except (OSError, json.JSONDecodeError, ValueError) as exc:
        print(f"error: delegated Worker preflight context invalid: {exc}", file=sys.stderr)
        return 1

    print("WORKER_PREFLIGHT_CONTEXT_GREEN")
    print(f"Repository: {args.expected_repository}")
    print(f"PR: {number}")
    print(f"Candidate SHA: {candidate_sha}")
    if args.run_id:
        print(f"Workflow run ID: {args.run_id}")
    if args.run_attempt:
        print(f"Workflow run attempt: {args.run_attempt}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
