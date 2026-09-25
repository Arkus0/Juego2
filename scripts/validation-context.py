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


def main() -> int:
    try:
        _safe_review_event_gate()
    except (safe.SafeOutputError, OSError, ValueError, json.JSONDecodeError) as exc:
        print(f"SAFE_OUTPUT_STOP: {exc}", file=sys.stderr)
        return 2
    return core.main()


if __name__ == "__main__":
    raise SystemExit(main())
