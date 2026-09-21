#!/usr/bin/env python3
"""Mechanical checks for Context Bootstrap v1.

The accepted-state index is a derived navigation projection. Its persisted freshness
anchor is deliberately non-self-referential: a DocSync projection records the main
commit it was derived from, and is fresh only while that commit is the first parent
of the current main commit that persists the projection.
"""

from __future__ import annotations

import argparse
import json
from pathlib import Path
import re
import sys
import tempfile

SHA_RE = re.compile(r"^[0-9a-f]{40}$", re.I)
REQUIRED_PROFILES = {"worker", "repair_worker", "reviewer", "planner_gate", "docsync", "h1_local_executor"}
CANDIDATE_PHASE = "CANDIDATE"
PERSISTED_PHASE = "DOCSYNC_PERSISTED"
FRESHNESS_CONTRACT = "docsync-first-parent-v1"

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
        if not isinstance(escalations, list) or not escalations or not all(isinstance(x, str) and x for x in escalations):
            raise BootstrapError(f"profile {name} requires non-empty must_escalate_if")
    if profiles["h1_local_executor"].get("escalation_result") != "REMOTE_DECISION_REQUIRED":
        raise BootstrapError("h1_local_executor must remain fail-closed to REMOTE_DECISION_REQUIRED")

def validate_index_shape(data: dict) -> tuple[str, str]:
    if data.get("schema") != "arkus.accepted-state-index@1":
        raise BootstrapError("unexpected accepted-state index schema")
    if data.get("authority") != "DERIVED_NAVIGATION_ONLY":
        raise BootstrapError("accepted-state index must remain DERIVED_NAVIGATION_ONLY")
    if data.get("freshness_contract") != FRESHNESS_CONTRACT:
        raise BootstrapError(f"freshness_contract must be {FRESHNESS_CONTRACT}")
    phase = data.get("projection_phase")
    if phase not in {CANDIDATE_PHASE, PERSISTED_PHASE}:
        raise BootstrapError("projection_phase must be CANDIDATE or DOCSYNC_PERSISTED")
    generated = data.get("generated_from_main_sha")
    if not isinstance(generated, str) or not SHA_RE.fullmatch(generated):
        raise BootstrapError("generated_from_main_sha must be an exact 40-character SHA")
    return phase, generated.lower()

def evaluate_source_anchor(data: dict, expected_source_sha: str) -> dict:
    phase, generated = validate_index_shape(data)
    if not SHA_RE.fullmatch(expected_source_sha):
        raise BootstrapError("expected source SHA must be an exact 40-character SHA")
    matched = generated == expected_source_sha.lower()
    return {"source_anchor": "MATCH" if matched else "MISMATCH", "projection_phase": phase, "generated_from_main_sha": generated, "expected_source_sha": expected_source_sha.lower(), "source_anchor_valid": matched}

def evaluate_live_index(data: dict, current_main_sha: str, current_main_parent_sha: str) -> dict:
    phase, generated = validate_index_shape(data)
    if not SHA_RE.fullmatch(current_main_sha):
        raise BootstrapError("current main SHA must be an exact 40-character SHA")
    if not SHA_RE.fullmatch(current_main_parent_sha):
        raise BootstrapError("current main first-parent SHA must be an exact 40-character SHA")
    current = current_main_sha.lower()
    parent = current_main_parent_sha.lower()
    if current == parent:
        raise BootstrapError("current main SHA and its first parent must differ")
    if phase != PERSISTED_PHASE:
        fresh = False
        reason = "projection_phase_not_persisted"
    elif generated != parent:
        fresh = False
        reason = "source_anchor_not_current_main_first_parent"
    else:
        fresh = True
        reason = "docsync_projection_persisted_from_current_main_first_parent"
    return {"state_index": "FRESH" if fresh else "STALE", "projection_phase": phase, "freshness_contract": FRESHNESS_CONTRACT, "generated_from_main_sha": generated, "current_main_sha": current, "current_main_first_parent_sha": parent, "mutable_hints_usable": fresh, "immutable_history_only": not fresh, "reason": reason, "on_stale": "RECONSTRUCT_FROM_LIVE_GITHUB_AND_AUTHORITATIVE_CONTRACTS"}

def run_self_test() -> None:
    valid_profiles = {"schema": "arkus.context-bootstrap-profiles@1", "profiles": {name: {"initial_reads": ["x"], "must_escalate_if": ["missing context"], **({"escalation_result": "REMOTE_DECISION_REQUIRED"} if name == "h1_local_executor" else {})} for name in REQUIRED_PROFILES}}
    validate_profiles(valid_profiles)
    anchor = "a" * 40
    persisted = {"schema": "arkus.accepted-state-index@1", "authority": "DERIVED_NAVIGATION_ONLY", "freshness_contract": FRESHNESS_CONTRACT, "projection_phase": PERSISTED_PHASE, "generated_from_main_sha": anchor}
    assert evaluate_source_anchor(persisted, anchor)["source_anchor_valid"] is True
    fresh = evaluate_live_index(persisted, "b" * 40, anchor)
    assert fresh["state_index"] == "FRESH" and fresh["mutable_hints_usable"] is True
    advanced = evaluate_live_index(persisted, "c" * 40, "b" * 40)
    assert advanced["state_index"] == "STALE" and advanced["mutable_hints_usable"] is False
    candidate = dict(persisted); candidate["projection_phase"] = CANDIDATE_PHASE
    candidate_result = evaluate_live_index(candidate, "b" * 40, anchor)
    assert candidate_result["state_index"] == "STALE" and candidate_result["reason"] == "projection_phase_not_persisted"
    mismatch = dict(persisted); mismatch["generated_from_main_sha"] = "d" * 40
    assert evaluate_live_index(mismatch, "b" * 40, anchor)["state_index"] == "STALE"
    broken_profiles = json.loads(json.dumps(valid_profiles)); del broken_profiles["profiles"]["reviewer"]
    try:
        validate_profiles(broken_profiles)
    except BootstrapError:
        pass
    else:
        raise AssertionError("missing profile did not fail closed")
    with tempfile.TemporaryDirectory() as temp_dir:
        try:
            load_json(Path(temp_dir) / "missing.json")
        except BootstrapError:
            pass
        else:
            raise AssertionError("missing compact context did not fail closed")
    print("context-bootstrap self-test: PASS")

def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--profiles", default="Docs/engineering/context-bootstrap-profiles.json")
    parser.add_argument("--index", default="Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json")
    parser.add_argument("--expected-source-sha")
    parser.add_argument("--current-main-sha")
    parser.add_argument("--current-main-parent-sha")
    parser.add_argument("--require-source-match", action="store_true")
    parser.add_argument("--require-fresh", action="store_true")
    parser.add_argument("--self-test", action="store_true")
    args = parser.parse_args()
    try:
        if args.self_test:
            run_self_test(); return 0
        profiles = load_json(Path(args.profiles)); index = load_json(Path(args.index))
        validate_profiles(profiles); validate_index_shape(index)
        ran = False
        if args.expected_source_sha:
            source_result = evaluate_source_anchor(index, args.expected_source_sha); print(json.dumps(source_result, indent=2, sort_keys=True)); ran = True
            if args.require_source_match and not source_result["source_anchor_valid"]: return 2
        elif args.require_source_match:
            raise BootstrapError("--require-source-match requires --expected-source-sha")
        live_args = (args.current_main_sha, args.current_main_parent_sha)
        if any(live_args) and not all(live_args):
            raise BootstrapError("live freshness requires both --current-main-sha and --current-main-parent-sha")
        if all(live_args):
            live_result = evaluate_live_index(index, args.current_main_sha, args.current_main_parent_sha); print(json.dumps(live_result, indent=2, sort_keys=True)); ran = True
            if args.require_fresh and not live_result["mutable_hints_usable"]: return 2
        elif args.require_fresh:
            raise BootstrapError("--require-fresh requires current main SHA and first-parent SHA")
        if not ran:
            raise BootstrapError("provide --expected-source-sha for candidate/source validation or both live main SHA arguments")
        return 0
    except BootstrapError as exc:
        print(f"context-bootstrap: FAIL: {exc}", file=sys.stderr); return 2

if __name__ == "__main__":
    raise SystemExit(main())
