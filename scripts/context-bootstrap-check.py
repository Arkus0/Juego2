#!/usr/bin/env python3
"""Mechanical checks for Context Bootstrap v1.

This script deliberately does not decide product/workpack semantics. It validates the
machine-readable boot-profile shape and the exact main-SHA freshness contract of the
derived accepted-state index. Live GitHub must supply current_main_sha.
"""

from __future__ import annotations

import argparse
import json
from pathlib import Path
import re
import sys
import tempfile

SHA_RE = re.compile(r"^[0-9a-f]{40}$", re.I)
REQUIRED_PROFILES = {
    "worker",
    "repair_worker",
    "reviewer",
    "planner_gate",
    "docsync",
    "h1_local_executor",
}


class BootstrapError(ValueError):
    pass


def load_json(path: Path) -> dict:
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except FileNotFoundError as exc:
        raise BootstrapError(f"missing compact context: {path}") from exc
    except json.JSONDecodeError as exc:
        raise BootstrapError(f"malformed JSON: {path}: {exc}") from exc
    if not isinstance(data, dict):
        raise BootstrapError(f"expected object root: {path}")
    return data


def validate_profiles(data: dict) -> None:
    if data.get("schema") != "arkus.context-bootstrap-profiles@1":
        raise BootstrapError("unexpected bootstrap profile schema")
    profiles = data.get("profiles")
    if not isinstance(profiles, dict):
        raise BootstrapError("profiles must be an object")
    missing = sorted(REQUIRED_PROFILES - set(profiles))
    if missing:
        raise BootstrapError(f"missing required profiles: {', '.join(missing)}")
    for name in sorted(REQUIRED_PROFILES):
        profile = profiles[name]
        if not isinstance(profile, dict):
            raise BootstrapError(f"profile {name} must be an object")
        reads = profile.get("initial_reads")
        escalations = profile.get("must_escalate_if")
        if not isinstance(reads, list) or not reads or not all(isinstance(x, str) and x for x in reads):
            raise BootstrapError(f"profile {name} requires non-empty initial_reads")
        if not isinstance(escalations, list) or not escalations or not all(
            isinstance(x, str) and x for x in escalations
        ):
            raise BootstrapError(f"profile {name} requires non-empty must_escalate_if")
    if profiles["h1_local_executor"].get("escalation_result") != "REMOTE_DECISION_REQUIRED":
        raise BootstrapError("h1_local_executor must remain fail-closed to REMOTE_DECISION_REQUIRED")


def evaluate_index(data: dict, current_main_sha: str) -> dict:
    if data.get("schema") != "arkus.accepted-state-index@1":
        raise BootstrapError("unexpected accepted-state index schema")
    if data.get("authority") != "DERIVED_NAVIGATION_ONLY":
        raise BootstrapError("accepted-state index must remain DERIVED_NAVIGATION_ONLY")

    generated = data.get("generated_from_main_sha")
    if not isinstance(generated, str) or not SHA_RE.fullmatch(generated):
        raise BootstrapError("generated_from_main_sha must be an exact 40-character SHA")
    if not SHA_RE.fullmatch(current_main_sha):
        raise BootstrapError("current main SHA must be an exact 40-character SHA")

    fresh = generated.lower() == current_main_sha.lower()
    return {
        "state_index": "FRESH" if fresh else "STALE",
        "generated_from_main_sha": generated.lower(),
        "current_main_sha": current_main_sha.lower(),
        "mutable_hints_usable": fresh,
        "on_stale": "RECONSTRUCT_FROM_LIVE_GITHUB_AND_AUTHORITATIVE_CONTRACTS",
    }


def run_self_test() -> None:
    valid_profiles = {
        "schema": "arkus.context-bootstrap-profiles@1",
        "profiles": {
            name: {
                "initial_reads": ["x"],
                "must_escalate_if": ["missing context"],
                **({"escalation_result": "REMOTE_DECISION_REQUIRED"} if name == "h1_local_executor" else {}),
            }
            for name in REQUIRED_PROFILES
        },
    }
    validate_profiles(valid_profiles)

    exact = "a" * 40
    other = "b" * 40
    base_index = {
        "schema": "arkus.accepted-state-index@1",
        "authority": "DERIVED_NAVIGATION_ONLY",
        "generated_from_main_sha": exact,
    }
    assert evaluate_index(base_index, exact)["mutable_hints_usable"] is True
    stale = evaluate_index(base_index, other)
    assert stale["state_index"] == "STALE"
    assert stale["mutable_hints_usable"] is False

    broken_profiles = json.loads(json.dumps(valid_profiles))
    del broken_profiles["profiles"]["reviewer"]
    try:
        validate_profiles(broken_profiles)
    except BootstrapError:
        pass
    else:
        raise AssertionError("missing profile did not fail closed")

    with tempfile.TemporaryDirectory() as temp_dir:
        missing = Path(temp_dir) / "missing.json"
        try:
            load_json(missing)
        except BootstrapError:
            pass
        else:
            raise AssertionError("missing compact context did not fail closed")

    print("context-bootstrap self-test: PASS")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "--profiles",
        default="Docs/engineering/context-bootstrap-profiles.json",
    )
    parser.add_argument(
        "--index",
        default="Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json",
    )
    parser.add_argument("--current-main-sha")
    parser.add_argument("--require-fresh", action="store_true")
    parser.add_argument("--self-test", action="store_true")
    args = parser.parse_args()

    try:
        if args.self_test:
            run_self_test()
            return 0
        if not args.current_main_sha:
            raise BootstrapError("--current-main-sha is required; obtain it from live GitHub")

        profiles = load_json(Path(args.profiles))
        index = load_json(Path(args.index))
        validate_profiles(profiles)
        result = evaluate_index(index, args.current_main_sha)
        print(json.dumps(result, indent=2, sort_keys=True))
        if args.require_fresh and not result["mutable_hints_usable"]:
            return 2
        return 0
    except BootstrapError as exc:
        print(f"context-bootstrap: FAIL: {exc}", file=sys.stderr)
        return 2


if __name__ == "__main__":
    raise SystemExit(main())
