#!/usr/bin/env python3
"""Safe-output front door for the local ChatGPT-subscription Arkus autopilot.

The accepted controller source is loaded into this module's own namespace so
its public/test surface stays identical. Only authority-bearing Reviewer seams
are then replaced with ARKUS_INTENT_V1. Human-readable legacy verdict fields
may mirror the decision for the existing GitHub workflow, but they cannot
create authority by themselves.
"""

from __future__ import annotations

import importlib.util
from pathlib import Path
import re
import sys
from typing import Any

HERE = Path(__file__).resolve().parent

# Execute the accepted lifecycle implementation in this exact module namespace.
# Temporarily changing __name__ suppresses the core's script entry point while
# preserving function.__globals__ == this module, so existing monkeypatch-based
# regressions continue to exercise the real lifecycle rather than a facade.
_public_name = __name__
__name__ = "_arkus_local_wp_autopilot_core_loaded"
exec(compile((HERE / "_local_wp_autopilot_core.py").read_text(encoding="utf-8"),
             str(HERE / "_local_wp_autopilot_core.py"), "exec"), globals())
__name__ = _public_name

_real_codex_role = codex_role
_real_main = main


def _load_safe():
    path = HERE / "arkus_safe_output.py"
    spec = importlib.util.spec_from_file_location("arkus_safe_output_runtime", path)
    if spec is None or spec.loader is None:
        raise RuntimeError("safe-output broker unavailable")
    module = importlib.util.module_from_spec(spec)
    sys.modules[spec.name] = module
    spec.loader.exec_module(module)
    return module


safe = _load_safe()


def _canonical_wp(pr_obj: dict[str, Any]) -> str:
    raw = fields(pr_obj.get("body") or "").get("wp", "")
    try:
        return safe.normalize_wp(raw)
    except safe.SafeOutputError as exc:
        raise StopFlow(f"PR #{pr_obj.get('number')} has invalid safe-output WP binding: {exc}") from exc


def _has_legacy_reviewer_claim(body: str) -> bool:
    parsed = fields(body)
    return any(name in parsed for name in
               ("reviewer verdict", "reviewed candidate sha", "autopilot review id"))


def reviewed_verdicts(pr: int) -> list[dict[str, str]]:
    """Return only Reviewer intents bound to a GitHub PR-review commit."""
    pr_obj = gh_json("api", f"repos/{REPO}/pulls/{pr}")
    wp = _canonical_wp(pr_obj)
    reviews = gh_pages(f"repos/{REPO}/pulls/{pr}/reviews?per_page=100")
    by_id: dict[str, dict[str, str]] = {}
    for item in reviews:
        body = item.get("body") or ""
        actor = (item.get("user") or {}).get("login")
        if safe.SCHEMA not in body:
            if actor == "Arkus0" and _has_legacy_reviewer_claim(body):
                raise StopFlow(
                    f"PR #{pr} has a manual/untagged legacy Reviewer verdict; "
                    "ARKUS_INTENT_V1 is required before autopilot adoption")
            continue
        if actor != "Arkus0":
            raise StopFlow(f"PR #{pr} safe-output Reviewer intent has invalid authority: {actor or 'missing'}")
        try:
            intent = safe.parse_intent(body)
            commit_id = (item.get("commit_id") or "").lower()
            intent = safe.authorize(intent, kind="REVIEW_VERDICT", role="REVIEWER",
                                    wp=wp, pr=pr, candidate_sha=commit_id)
        except safe.SafeOutputError as exc:
            raise StopFlow(f"PR #{pr} rejected Reviewer safe output: {exc}") from exc
        payload = intent["payload"]
        review_id = payload["review_id"]
        row = {
            "id": review_id,
            "sha": intent["candidate_sha"],
            "verdict": payload["verdict"],
            "body": body,
            "at": item.get("submitted_at") or "",
            "intent_key": safe.intent_key(intent),
            "decision_slot": safe.decision_slot(intent),
        }
        prior = by_id.get(review_id)
        if prior and (prior["sha"], prior["verdict"], prior["decision_slot"]) != \
                (row["sha"], row["verdict"], row["decision_slot"]):
            raise StopFlow(f"Contradictory safe-output Reviewer ID {review_id} on PR #{pr}")
        if not prior or row["at"] < prior["at"]:
            by_id[review_id] = row
    return sorted(by_id.values(), key=lambda row: row["at"])


def reviewed_fails(pr: int) -> list[dict[str, str]]:
    return [row for row in reviewed_verdicts(pr) if row["verdict"] == "FAIL"]


def _review_prompt_contract(prompt: str, role: str) -> str:
    pr_match = re.search(r"PR #(\d+)", prompt)
    sha_match = re.search(r"\bSHA ([0-9a-f]{40})\b", prompt, re.IGNORECASE)
    id_match = re.search(r"Autopilot review ID:\s*([0-9a-f]{32})", prompt, re.IGNORECASE)
    if not (pr_match and sha_match and id_match):
        raise StopFlow("Reviewer launch lacks exact PR/SHA/review-id for safe-output binding")
    pr = int(pr_match.group(1))
    sha = sha_match.group(1).lower()
    rid = id_match.group(1).lower()
    pr_obj = gh_json("api", f"repos/{REPO}/pulls/{pr}")
    wp = _canonical_wp(pr_obj)
    campaign = safe.campaign_id(wp, pr)
    allowed = ("PASS", "FAIL") if role == "appeal-reviewer" else tuple(sorted(safe.REVIEW_VERDICTS))
    choices = [safe.render_review(wp=wp, pr=pr, candidate_sha=sha,
                                  verdict=verdict, review_id=rid)
               for verdict in allowed]
    return (
        "\n\nSAFE-OUTPUT AUTHORITY CONTRACT (mandatory):\n"
        "Publish the result as a GitHub PULL REQUEST REVIEW (not an issue comment) attached to the exact reviewed commit. "
        "Human rationale is free-form, but machine authority is exactly one ARKUS_INTENT_V1 line. "
        "Choose exactly one of these lines and include it verbatim; do not edit identity fields:\n"
        + "\n".join(choices) +
        "\nFor compatibility with State Transitions, also include the legacy lines requested earlier, "
        "but make Reviewer verdict and Reviewed candidate SHA exactly mirror the chosen intent. "
        "Those legacy lines are rejected without this envelope. "
        f"Campaign binding: {campaign}. Missing, duplicate, stale or mutated envelopes fail closed."
    )


async def codex_role(root: Path, state: Path, role: str, prompt: str, model: str, effort: str,
                     schema: Path | None = None, assets_root: Path | None = None) -> str:
    if role in {"reviewer", "appeal-reviewer"}:
        prompt += _review_prompt_contract(prompt, role)
    return await _real_codex_role(root, state, role, prompt, model, effort, schema, assets_root)


def main() -> int:
    return _real_main()


if __name__ == "__main__":
    raise SystemExit(main())
