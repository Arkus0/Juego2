#!/usr/bin/env python3
"""Safe-output front door for durable Reviewer verdict adoption.

Only ARKUS_INTENT_V1 carried by an owner-authored GitHub PR review and bound to
that review's GitHub commit_id can become an authoritative verdict.  Legacy
free-text verdict fields and issue comments are intentionally non-authoritative.
"""

from __future__ import annotations

import importlib.util
from pathlib import Path
import re
import sys
from typing import Any

HERE = Path(__file__).resolve().parent


def _load(name: str, path: Path):
    spec = importlib.util.spec_from_file_location(name, path)
    if spec is None or spec.loader is None:
        raise RuntimeError(f"cannot load {path}")
    module = importlib.util.module_from_spec(spec)
    sys.modules[name] = module
    spec.loader.exec_module(module)
    return module


core = _load("arkus_reviewer_verdict_adoption_core", HERE / "_reviewer_verdict_adoption_core.py")
safe = _load("arkus_safe_output_adoption", HERE / "arkus_safe_output.py")


def authoritative_verdicts(reviews: list[dict[str, Any]], comments: list[dict[str, Any]]) -> list[dict[str, str]]:
    for item in comments:
        if safe.SCHEMA in (item.get("body") or ""):
            raise core.AdoptionError("Reviewer safe output must be a GitHub PR review, not an issue comment")

    by_id: dict[str, dict[str, str]] = {}
    pr_number: int | None = None
    pr_obj: dict[str, Any] | None = None
    wp: str | None = None
    for item in reviews:
        body = item.get("body") or ""
        if safe.SCHEMA not in body:
            continue
        actor = (item.get("user") or {}).get("login")
        if actor != "Arkus0":
            raise core.AdoptionError(f"Reviewer safe output has invalid authority: {actor or 'missing'}")
        match = re.search(r"/pulls/([1-9][0-9]*)$", item.get("pull_request_url") or "")
        if not match:
            raise core.AdoptionError("Reviewer safe output lacks a canonical PR URL")
        item_pr = int(match.group(1))
        if pr_number is None:
            pr_number = item_pr
            pr_obj = core.gh_json("api", f"repos/{core.REPO}/pulls/{item_pr}")
            if not isinstance(pr_obj, dict):
                raise core.AdoptionError("PR lookup returned malformed data")
            try:
                wp = safe.normalize_wp(core.body_field(pr_obj.get("body") or "", "WP") or "")
            except safe.SafeOutputError as exc:
                raise core.AdoptionError(f"invalid safe-output WP binding: {exc}") from exc
        elif item_pr != pr_number:
            raise core.AdoptionError("mixed PR identities in Reviewer evidence")
        commit_id = (item.get("commit_id") or "").lower()
        try:
            intent = safe.parse_intent(body)
            safe.authorize(intent, kind="REVIEW_VERDICT", role="REVIEWER",
                           wp=wp or "", pr=item_pr, candidate_sha=commit_id)
        except safe.SafeOutputError as exc:
            raise core.AdoptionError(f"Reviewer safe output rejected: {exc}") from exc
        payload = intent["payload"]
        row = {
            "id": payload["review_id"],
            "sha": intent["candidate_sha"],
            "verdict": payload["verdict"],
            "at": item.get("submitted_at") or "",
            "intent_key": safe.intent_key(intent),
            "decision_slot": safe.decision_slot(intent),
        }
        prior = by_id.get(row["id"])
        if prior and (prior["sha"], prior["verdict"], prior["decision_slot"]) != (row["sha"], row["verdict"], row["decision_slot"]):
            raise core.AdoptionError(f"contradictory safe-output Reviewer ID {row['id']}")
        if not prior or row["at"] < prior["at"]:
            by_id[row["id"]] = row
    return sorted(by_id.values(), key=lambda row: row["at"])


core.authoritative_verdicts = authoritative_verdicts


def main() -> int:
    return core.main()


if __name__ == "__main__":
    raise SystemExit(main())
