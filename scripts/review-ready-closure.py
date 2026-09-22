#!/usr/bin/env python3
"""Fail-closed mechanical validator for Worker -> Reviewer terminal closure.

The script deliberately proves lifecycle facts only. It never interprets the
candidate or substitutes for independent Reviewer judgment.
"""
from __future__ import annotations

import argparse
import json
import re
import sys
from dataclasses import dataclass
from pathlib import Path

SHA_RE = re.compile(r"^[0-9a-f]{40}$", re.I)


@dataclass(frozen=True)
class State:
    pr_draft: bool
    pr_head_sha: str
    candidate_sha: str
    frozen_sha: str
    worker_state: str
    worker_pre_review: str
    branch_frozen: str
    worker_verdict: str
    reviewer_verdict: str
    handoff_check: str
    freeze_check: str
    marker_sha: str | None
    final_head_sha: str


def norm(value: str | None) -> str:
    return (value or "").strip()


def validate(state: State) -> list[str]:
    errors: list[str] = []
    head = norm(state.pr_head_sha).lower()
    candidate = norm(state.candidate_sha).lower()
    frozen = norm(state.frozen_sha).lower()
    final_head = norm(state.final_head_sha).lower()
    marker = norm(state.marker_sha).lower() if state.marker_sha is not None else ""

    for name, value in (
        ("PR HEAD", head),
        ("Candidate HEAD SHA", candidate),
        ("Frozen candidate SHA", frozen),
        ("final live HEAD", final_head),
    ):
        if not SHA_RE.fullmatch(value):
            errors.append(f"{name} must be an exact 40-character SHA")

    if state.pr_draft:
        errors.append("PR is Draft; REVIEW_READY closure requires Ready state")

    expected = {
        "Worker state": (state.worker_state, "FROZEN_FOR_REVIEW"),
        "Worker pre-review": (state.worker_pre_review, "CLEAN"),
        "Branch frozen": (state.branch_frozen, "YES"),
        "Worker verdict": (state.worker_verdict, "IN_REVIEW"),
        "Reviewer verdict": (state.reviewer_verdict, "PENDING"),
        "Worker handoff lint": (state.handoff_check, "SUCCESS"),
        "Freeze exact-SHA validation": (state.freeze_check, "SUCCESS"),
    }
    for label, (actual, wanted) in expected.items():
        if norm(actual).upper() != wanted:
            errors.append(f"{label} must be {wanted}, got {actual!r}")

    if SHA_RE.fullmatch(head):
        if candidate != head:
            errors.append("Candidate HEAD SHA does not equal live PR HEAD")
        if frozen != head:
            errors.append("Frozen candidate SHA does not equal live PR HEAD")

    # This is intentionally mandatory even when every prerequisite gate is green.
    # A planned/pending state transition is not durable lifecycle evidence.
    if not marker:
        errors.append("matching durable Automation V2 REVIEW_READY marker is absent")
    elif not SHA_RE.fullmatch(marker):
        errors.append("REVIEW_READY marker target is not an exact SHA")
    elif marker != frozen:
        errors.append("REVIEW_READY marker targets a different SHA")

    # final_head_sha must be obtained by a live read performed *after* the marker
    # that supplied marker_sha. The caller/workflow owns that temporal ordering.
    if SHA_RE.fullmatch(final_head) and SHA_RE.fullmatch(frozen) and final_head != frozen:
        errors.append("live PR HEAD moved after REVIEW_READY marker observation")

    return errors


def from_json(path: Path) -> State:
    raw = json.loads(path.read_text(encoding="utf-8"))
    return State(
        pr_draft=bool(raw["pr_draft"]),
        pr_head_sha=str(raw["pr_head_sha"]),
        candidate_sha=str(raw["candidate_sha"]),
        frozen_sha=str(raw["frozen_sha"]),
        worker_state=str(raw["worker_state"]),
        worker_pre_review=str(raw["worker_pre_review"]),
        branch_frozen=str(raw["branch_frozen"]),
        worker_verdict=str(raw["worker_verdict"]),
        reviewer_verdict=str(raw["reviewer_verdict"]),
        handoff_check=str(raw["handoff_check"]),
        freeze_check=str(raw["freeze_check"]),
        marker_sha=None if raw.get("marker_sha") is None else str(raw["marker_sha"]),
        final_head_sha=str(raw["final_head_sha"]),
    )


def good_state() -> State:
    sha = "a" * 40
    return State(
        pr_draft=False,
        pr_head_sha=sha,
        candidate_sha=sha,
        frozen_sha=sha,
        worker_state="FROZEN_FOR_REVIEW",
        worker_pre_review="CLEAN",
        branch_frozen="YES",
        worker_verdict="IN_REVIEW",
        reviewer_verdict="PENDING",
        handoff_check="SUCCESS",
        freeze_check="SUCCESS",
        marker_sha=sha,
        final_head_sha=sha,
    )


def self_test() -> None:
    good = good_state()
    assert validate(good) == []

    missing_marker = State(**{**good.__dict__, "marker_sha": None})
    assert any("marker is absent" in e for e in validate(missing_marker))

    wrong_marker = State(**{**good.__dict__, "marker_sha": "b" * 40})
    assert any("different SHA" in e for e in validate(wrong_marker))

    moved = State(**{**good.__dict__, "pr_head_sha": "c" * 40, "final_head_sha": "c" * 40})
    moved_errors = validate(moved)
    assert any("Candidate HEAD SHA" in e for e in moved_errors)
    assert any("Frozen candidate SHA" in e for e in moved_errors)
    assert any("moved after REVIEW_READY" in e for e in moved_errors)

    ctx01_incomplete = State(**{**good.__dict__, "handoff_check": "FAILURE"})
    assert any("Worker handoff lint" in e for e in validate(ctx01_incomplete))

    ctx02_missing_pred = State(**{**good.__dict__, "handoff_check": "FAILURE"})
    assert validate(ctx02_missing_pred)
    assert validate(good) == []

    no_freeze = State(**{**good.__dict__, "freeze_check": "SKIPPED"})
    assert any("Freeze exact-SHA validation" in e for e in validate(no_freeze))

    print("review-ready-closure self-test: PASS")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--state-json", type=Path)
    parser.add_argument("--self-test", action="store_true")
    args = parser.parse_args()

    if args.self_test:
        self_test()
        return 0
    if args.state_json is None:
        parser.error("--state-json is required unless --self-test is used")

    try:
        state = from_json(args.state_json)
        errors = validate(state)
    except (OSError, KeyError, TypeError, ValueError, json.JSONDecodeError) as exc:
        print(f"REVIEW_READY_CLOSURE_OUTCOME: INFRA_ERROR\n- malformed state input: {exc}", file=sys.stderr)
        return 23

    if errors:
        print("REVIEW_READY_CLOSURE_OUTCOME: REVIEW_BLOCKED", file=sys.stderr)
        for error in errors:
            print(f"- {error}", file=sys.stderr)
        return 21

    print("REVIEW_READY_CLOSURE_OUTCOME: PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
