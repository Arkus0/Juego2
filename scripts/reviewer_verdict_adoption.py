#!/usr/bin/env python3
"""Safe-output front door for durable Reviewer verdict adoption.

The accepted adoption implementation is loaded into this module namespace so
its tested API and monkeypatch seams remain intact. Its one authority oracle is
then replaced: only ARKUS_INTENT_V1 from an owner-authored GitHub PR review,
bound to that review's commit_id, can be adopted. Legacy prose is never enough.
"""

from __future__ import annotations

import importlib.util
from pathlib import Path
import re
import sys
from typing import Any

HERE = Path(__file__).resolve().parent
_public_name = __name__
__name__ = "_arkus_reviewer_verdict_adoption_core_loaded"
exec(compile((HERE / "_reviewer_verdict_adoption_core.py").read_text(encoding="utf-8"),
             str(HERE / "_reviewer_verdict_adoption_core.py"), "exec"), globals())
__name__ = _public_name
_real_main = main
_repair_review_identity: dict[tuple[str, str], str] = {}


def _load_safe():
    path = HERE / "arkus_safe_output.py"
    spec = importlib.util.spec_from_file_location("arkus_safe_output_adoption", path)
    if spec is None or spec.loader is None:
        raise RuntimeError("safe-output broker unavailable")
    module = importlib.util.module_from_spec(spec)
    sys.modules[spec.name] = module
    spec.loader.exec_module(module)
    return module


safe = _load_safe()


def authoritative_verdicts(reviews: list[dict[str, Any]], comments: list[dict[str, Any]]) -> list[dict[str, str]]:
    for item in comments:
        if safe.SCHEMA in (item.get("body") or ""):
            raise AdoptionError("Reviewer safe output must be a GitHub PR review, not an issue comment")

    by_id: dict[str, dict[str, str]] = {}
    pr_number: int | None = None
    wp: str | None = None
    for item in reviews:
        body = item.get("body") or ""
        actor = (item.get("user") or {}).get("login")
        if safe.SCHEMA not in body:
            if actor == "Arkus0" and structured_verdict(body) is not None:
                raise AdoptionError("legacy structured Reviewer verdict is non-authoritative without ARKUS_INTENT_V1")
            continue
        if actor != "Arkus0":
            raise AdoptionError(f"Reviewer safe output has invalid authority: {actor or 'missing'}")
        match = re.search(r"/pulls/([1-9][0-9]*)$", item.get("pull_request_url") or "")
        if not match:
            raise AdoptionError("Reviewer safe output lacks a canonical PR URL")
        item_pr = int(match.group(1))
        if pr_number is None:
            pr_number = item_pr
            pr_obj = gh_json("api", f"repos/{REPO}/pulls/{item_pr}")
            if not isinstance(pr_obj, dict):
                raise AdoptionError("PR lookup returned malformed data")
            try:
                wp = safe.normalize_wp(body_field(pr_obj.get("body") or "", "WP") or "")
            except safe.SafeOutputError as exc:
                raise AdoptionError(f"invalid safe-output WP binding: {exc}") from exc
        elif item_pr != pr_number:
            raise AdoptionError("mixed PR identities in Reviewer evidence")
        commit_id = (item.get("commit_id") or "").lower()
        try:
            intent = safe.parse_intent(body)
            safe.authorize(intent, kind="REVIEW_VERDICT", role="REVIEWER",
                           wp=wp or "", pr=item_pr, candidate_sha=commit_id)
        except safe.SafeOutputError as exc:
            raise AdoptionError(f"Reviewer safe output rejected: {exc}") from exc
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
        if prior and (prior["sha"], prior["verdict"], prior["decision_slot"]) != \
                (row["sha"], row["verdict"], row["decision_slot"]):
            raise AdoptionError(f"contradictory safe-output Reviewer ID {row['id']}")
        if not prior or row["at"] < prior["at"]:
            by_id[row["id"]] = row

    rows = sorted(by_id.values(), key=lambda row: row["at"])
    _repair_review_identity.clear()
    for row in rows:
        identity = (row["sha"].lower(), row["at"])
        prior_id = _repair_review_identity.get(identity)
        if prior_id and prior_id != row["id"]:
            raise AdoptionError(
                f"ambiguous Reviewer identity for repair transition at {row['sha']} {row['at']}")
        _repair_review_identity[identity] = row["id"].lower()
    return rows


def repair_marker_after_verdict(comments: list[dict[str, Any]], sha: str,
                                verdict_at: str) -> bool:
    """Match REPAIR_REQUIRED to the exact Reviewer cycle, not merely SHA/time."""
    sha = sha.lower()
    review_id = _repair_review_identity.get((sha, verdict_at))
    if not review_id:
        raise AdoptionError("repair transition lookup lacks exact Reviewer identity")
    key_re = re.compile(
        rf"^adopted-repair-required:[1-9][0-9]*:{re.escape(sha)}:{re.escape(review_id)}$")
    for item in comments:
        if (item.get("user") or {}).get("login") != "github-actions[bot]":
            continue
        body = item.get("body") or ""
        fields = marker_fields(body)
        if ("ARKUS_AUTOMATION_V2" in body and
                fields.get("state") == "REPAIR_REQUIRED" and
                fields.get("target sha", "").lower() == sha and
                (item.get("created_at") or "") >= verdict_at and
                key_re.fullmatch(fields.get("key", ""))):
            return True
    return False


def main() -> int:
    return _real_main()


if __name__ == "__main__":
    raise SystemExit(main())
