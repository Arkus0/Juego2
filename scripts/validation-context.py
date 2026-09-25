#!/usr/bin/env python3
"""Canonical validation-context API plus safe-output gate for review events.

All programmatic APIs are re-exported unchanged from the proven validation
context core.  Only the CLI entry used by Arkus State Transitions gains a
review-event capability firewall: legacy verdict fields are accepted solely as
an exact mirror of an authorized ARKUS_INTENT_V1 PR-review envelope.
"""

from __future__ import annotations

import importlib.util
import json
import os
from pathlib import Path
import re
import sys
import tempfile

HERE = Path(__file__).resolve().parent


def _load(name: str, path: Path):
    spec = importlib.util.spec_from_file_location(name, path)
    if spec is None or spec.loader is None:
        raise RuntimeError(f"cannot load {path}")
    module = importlib.util.module_from_spec(spec)
    sys.modules[name] = module
    spec.loader.exec_module(module)
    return module


core = _load("arkus_validation_context_core", HERE / "_validation_context_core.py")
safe = _load("arkus_safe_output_validation_context", HERE / "arkus_safe_output.py")

# Preserve the existing imported API for candidate validation, receipt reuse,
# CTX/DW probes and helpers.  This wrapper changes no context semantics.
for _name, _value in vars(core).items():
    if not _name.startswith("__") and _name not in {"main"}:
        globals().setdefault(_name, _value)


def _field(body: str, name: str) -> str:
    matches = re.findall(rf"^{re.escape(name)}:\s*`?([^`\r\n]+?)`?\s*$", body,
                         re.MULTILINE | re.IGNORECASE)
    if len(matches) != 1:
        raise safe.SafeOutputError(f"{name} mirror must occur exactly once, found {len(matches)}")
    return matches[0].strip()


def _safe_review_event_gate() -> None:
    if os.environ.get("GITHUB_WORKFLOW") != "Arkus State Transitions":
        return
    event_name = os.environ.get("GITHUB_EVENT_NAME", "")
    if event_name not in {"pull_request_review", "issue_comment"}:
        return
    if event_name == "issue_comment":
        raise safe.SafeOutputError("Reviewer authority requires a commit-bound GitHub PR review; issue comments are non-authoritative")

    event_path = os.environ.get("GITHUB_EVENT_PATH", "")
    if not event_path:
        raise safe.SafeOutputError("GitHub review event payload unavailable")
    with open(event_path, encoding="utf-8") as fh:
        event = json.load(fh)
    pr = event.get("pull_request") or {}
    review = event.get("review") or {}
    actor = (review.get("user") or {}).get("login")
    association = str(review.get("author_association") or "").upper()
    if actor != "Arkus0" or association != "OWNER":
        raise safe.SafeOutputError(
            f"Reviewer authority requires owner-authored PR review; got actor={actor or 'missing'} association={association or 'missing'}"
        )

    body = review.get("body") or ""
    pr_number = pr.get("number")
    commit_id = (review.get("commit_id") or "").lower()
    wp = _field(pr.get("body") or "", "WP")

    intent = safe.parse_intent(body)
    safe.authorize(intent, kind="REVIEW_VERDICT", role="REVIEWER",
                   wp=wp, pr=pr_number, candidate_sha=commit_id)

    # The legacy workflow still transports PASS/FAIL through two fields.  They
    # have no independent authority: both must mirror the already-authorized
    # intent exactly or this mandatory context resolver fails closed.
    legacy_verdict = _field(body, "Reviewer verdict").upper()
    legacy_sha = _field(body, "Reviewed candidate SHA").lower()
    payload = intent["payload"]
    if payload["verdict"] not in {"PASS", "FAIL"}:
        raise safe.SafeOutputError("direct State Transitions accepts only final PASS/FAIL safe outputs")
    if legacy_verdict != payload["verdict"] or legacy_sha != intent["candidate_sha"]:
        raise safe.SafeOutputError("legacy review fields do not exactly mirror the authorized safe output")


def _safe_review_event_self_test() -> None:
    sha = "a" * 40
    review_id = "b" * 32
    body = (
        "Reviewer verdict: FAIL\n"
        f"Reviewed candidate SHA: {sha}\n"
        + safe.render_review(
            wp="WP-H1-07", pr=207, candidate_sha=sha,
            verdict="FAIL", review_id=review_id,
        )
        + "\n"
    )
    event = {
        "pull_request": {"number": 207, "body": "WP: WP-H1-07\n"},
        "review": {
            "user": {"login": "Arkus0"},
            "author_association": "OWNER",
            "commit_id": sha,
            "body": body,
        },
    }
    names = ("GITHUB_WORKFLOW", "GITHUB_EVENT_NAME", "GITHUB_EVENT_PATH")
    prior = {name: os.environ.get(name) for name in names}
    try:
        with tempfile.TemporaryDirectory() as tmp:
            event_path = Path(tmp) / "event.json"
            os.environ["GITHUB_WORKFLOW"] = "Arkus State Transitions"
            os.environ["GITHUB_EVENT_NAME"] = "pull_request_review"
            os.environ["GITHUB_EVENT_PATH"] = str(event_path)

            event_path.write_text(json.dumps(event), encoding="utf-8")
            _safe_review_event_gate()

            for association in ("MEMBER", "COLLABORATOR"):
                event["review"]["author_association"] = association
                event["review"]["user"] = {"login": "mallory"}
                event_path.write_text(json.dumps(event), encoding="utf-8")
                try:
                    _safe_review_event_gate()
                except safe.SafeOutputError as exc:
                    if "owner-authored" not in str(exc):
                        raise AssertionError(f"unexpected non-owner rejection: {exc}") from exc
                else:
                    raise AssertionError(f"{association} pull_request_review unexpectedly authorized")

            event["review"]["author_association"] = "MEMBER"
            event["review"]["user"] = {"login": "Arkus0"}
            event_path.write_text(json.dumps(event), encoding="utf-8")
            try:
                _safe_review_event_gate()
            except safe.SafeOutputError as exc:
                if "owner-authored" not in str(exc):
                    raise AssertionError(f"unexpected association rejection: {exc}") from exc
            else:
                raise AssertionError("owner login with non-OWNER association unexpectedly authorized")
    finally:
        for name, value in prior.items():
            if value is None:
                os.environ.pop(name, None)
            else:
                os.environ[name] = value


def main() -> int:
    try:
        _safe_review_event_gate()
        if sys.argv[1:] == ["self-test"]:
            _safe_review_event_self_test()
    except (safe.SafeOutputError, OSError, ValueError, json.JSONDecodeError, AssertionError) as exc:
        print(f"SAFE_OUTPUT_STOP: {exc}", file=sys.stderr)
        return 2
    return core.main()


if __name__ == "__main__":
    raise SystemExit(main())
