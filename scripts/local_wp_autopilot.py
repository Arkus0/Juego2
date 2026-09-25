#!/usr/bin/env python3
"""Safe-output front door for the local ChatGPT-subscription Arkus autopilot.

The historical controller remains an implementation core.  This canonical
entry point replaces its free-text Reviewer oracle with ARKUS_INTENT_V1 and
injects the machine-authoritative envelope requirement into every Reviewer
role.  Old `Reviewer verdict:` prose may remain readable to humans but is not
consumed as authority.
"""

from __future__ import annotations

import importlib.util
import json
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


core = _load("arkus_local_wp_autopilot_core", HERE / "_local_wp_autopilot_core.py")
safe = _load("arkus_safe_output_runtime", HERE / "arkus_safe_output.py")
_real_codex_role = core.codex_role


def _canonical_wp(pr_obj: dict[str, Any]) -> str:
    raw = core.fields(pr_obj.get("body") or "").get("wp", "")
    try:
        return safe.normalize_wp(raw)
    except safe.SafeOutputError as exc:
        raise core.StopFlow(f"PR #{pr_obj.get('number')} has invalid safe-output WP binding: {exc}") from exc


def reviewed_verdicts(pr: int) -> list[dict[str, str]]:
    """Return only Reviewer intents externally bound to a GitHub review commit."""
    pr_obj = core.gh_json("api", f"repos/{core.REPO}/pulls/{pr}")
    wp = _canonical_wp(pr_obj)
    reviews = core.gh_pages(f"repos/{core.REPO}/pulls/{pr}/reviews?per_page=100")
    by_id: dict[str, dict[str, str]] = {}
    for item in reviews:
        if (item.get("user") or {}).get("login") != "Arkus0":
            continue
        body = item.get("body") or ""
        if safe.SCHEMA not in body:
            # Legacy prose is intentionally non-authoritative after this front door.
            continue
        try:
            intent = safe.parse_intent(body)
            commit_id = (item.get("commit_id") or "").lower()
            intent = safe.authorize(
                intent, kind="REVIEW_VERDICT", role="REVIEWER",
                wp=wp, pr=pr, candidate_sha=commit_id,
            )
        except safe.SafeOutputError as exc:
            raise core.StopFlow(f"PR #{pr} rejected Reviewer safe output: {exc}") from exc
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
        if prior and (prior["sha"], prior["verdict"], prior["decision_slot"]) != (row["sha"], row["verdict"], row["decision_slot"]):
            raise core.StopFlow(f"Contradictory safe-output Reviewer ID {review_id} on PR #{pr}")
        if not prior or row["at"] < prior["at"]:
            by_id[review_id] = row
    return sorted(by_id.values(), key=lambda row: row["at"])


def _review_prompt_contract(prompt: str) -> str:
    pr_match = re.search(r"PR #(\d+)", prompt)
    sha_match = re.search(r"\bSHA ([0-9a-f]{40})\b", prompt, re.IGNORECASE)
    id_match = re.search(r"Autopilot review ID:\s*([0-9a-f]{32})", prompt, re.IGNORECASE)
    if not (pr_match and sha_match and id_match):
        raise core.StopFlow("Reviewer launch lacks exact PR/SHA/review-id for safe-output binding")
    pr = int(pr_match.group(1))
    sha = sha_match.group(1).lower()
    rid = id_match.group(1).lower()
    pr_obj = core.gh_json("api", f"repos/{core.REPO}/pulls/{pr}")
    wp = _canonical_wp(pr_obj)
    campaign = safe.campaign_id(wp, pr)
    allowed = ("PASS", "FAIL") if "appeal-reviewer" in prompt.lower() else tuple(sorted(safe.REVIEW_VERDICTS))
    choices = []
    for verdict in allowed:
        choices.append(safe.render_review(wp=wp, pr=pr, candidate_sha=sha, verdict=verdict, review_id=rid))
    return (
        "\n\nSAFE-OUTPUT AUTHORITY CONTRACT (mandatory):\n"
        "The human rationale may use normal prose, but prose and legacy `Reviewer verdict:` fields are non-authoritative. "
        "Publish the result as a GitHub PULL REQUEST REVIEW (not an issue comment) attached to the exact reviewed commit. "
        "Include exactly one of the following ARKUS_INTENT_V1 lines verbatim, matching your chosen verdict; do not edit its identity fields.\n"
        + "\n".join(choices) +
        f"\nCampaign binding: {campaign}. Any missing/duplicate/mutated envelope is rejected fail-closed."
    )


async def codex_role(root: Path, state: Path, role: str, prompt: str, model: str, effort: str,
                     schema: Path | None = None, assets_root: Path | None = None) -> str:
    if role in {"reviewer", "appeal-reviewer"}:
        prompt += _review_prompt_contract(prompt)
    return await _real_codex_role(root, state, role, prompt, model, effort, schema, assets_root)


# Patch the implementation core before entering it.  Functions defined in the
# core resolve these globals at runtime, so merge/repair/DocSync consume only
# broker-authorized verdicts while retaining the already-tested lifecycle.
core.reviewed_verdicts = reviewed_verdicts
core.reviewed_fails = lambda pr: [row for row in reviewed_verdicts(pr) if row["verdict"] == "FAIL"]
core.codex_role = codex_role


def main() -> int:
    return core.main()


if __name__ == "__main__":
    raise SystemExit(main())
