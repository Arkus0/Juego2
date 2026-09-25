#!/usr/bin/env python3
"""Durably adopt an already-published independent Reviewer verdict.

GitHub is the source of truth. This helper is intended to run under GITHUB_TOKEN
inside GitHub Actions after a local controller requests adoption. It never
creates a Reviewer verdict; it only re-validates an existing exact-SHA verdict
and records the missing process transition idempotently.
"""

from __future__ import annotations

import argparse
import importlib.util
import json
from pathlib import Path
import re
import subprocess
import sys
from typing import Any

REPO = "Arkus0/Juego2"
SHA_RE = re.compile(r"^[0-9a-f]{40}$")
REVIEW_ID_RE = re.compile(r"^[0-9a-f]{32}$")
VERDICTS = {"PASS", "FAIL", "PROTOCOL_FIX", "REVIEW_BLOCKED"}


class AdoptionError(Exception):
    pass


def _load_validation_context():
    path = Path(__file__).with_name("validation-context.py")
    spec = importlib.util.spec_from_file_location("arkus_reviewer_adoption_validation_context", path)
    if spec is None or spec.loader is None:
        raise RuntimeError("canonical validation-context resolver unavailable")
    module = importlib.util.module_from_spec(spec)
    sys.modules[spec.name] = module
    spec.loader.exec_module(module)
    return module


validation_context = _load_validation_context()


def gh_json(*args: str) -> Any:
    result = subprocess.run(["gh", *args], capture_output=True, text=True,
                            encoding="utf-8", errors="replace", check=False)
    if result.returncode:
        raise AdoptionError(f"gh failed ({result.returncode}): {result.stderr[-500:]}")
    return json.loads(result.stdout) if result.stdout.strip() else None


def gh_pages(path: str) -> list[dict[str, Any]]:
    pages = gh_json("api", "--paginate", "--slurp", path)
    if not isinstance(pages, list) or any(not isinstance(page, list) for page in pages):
        raise AdoptionError(f"unexpected paginated GitHub response for {path}")
    return [row for page in pages for row in page]


def normalized_line(raw: str) -> str:
    line = re.sub(r"^#{1,6}\s*", "", raw.strip())
    return line.replace("**", "").replace("__", "").replace("`", "").strip()


def structured_verdict(body: str) -> dict[str, str] | None:
    lines = [normalized_line(raw) for raw in body.splitlines()]
    has_any = any(re.match(r"^(Reviewer verdict|Reviewed candidate SHA|Autopilot review ID):", line, re.I)
                  for line in lines)
    if not has_any:
        return None
    verdicts, shas, review_ids = [], [], []
    for line in lines:
        match = re.fullmatch(r"Reviewer verdict:\s*(PASS|FAIL|PROTOCOL_FIX|REVIEW_BLOCKED)\.?", line, re.I)
        if match:
            verdicts.append(match.group(1).upper())
        match = re.fullmatch(r"Reviewed candidate SHA:\s*([0-9a-fA-F]{40})\.?", line, re.I)
        if match:
            shas.append(match.group(1).lower())
        match = re.fullmatch(r"Autopilot review ID:\s*([^\s.]+)\.?", line, re.I)
        if match:
            review_ids.append(match.group(1).lower())
    if len(verdicts) != 1 or len(shas) != 1 or len(review_ids) != 1:
        raise AdoptionError("ambiguous or incomplete structured Reviewer verdict")
    if verdicts[0] not in VERDICTS or not REVIEW_ID_RE.fullmatch(review_ids[0]):
        raise AdoptionError("invalid Reviewer verdict identity")
    return {"verdict": verdicts[0], "sha": shas[0], "id": review_ids[0]}


def authoritative_verdicts(reviews: list[dict[str, Any]], comments: list[dict[str, Any]]) -> list[dict[str, str]]:
    by_id: dict[str, dict[str, str]] = {}
    for item in reviews + comments:
        parsed = structured_verdict(item.get("body") or "")
        if parsed is None:
            continue
        actor = (item.get("user") or {}).get("login")
        if actor != "Arkus0":
            raise AdoptionError(f"structured Reviewer verdict has invalid authority: {actor or 'missing'}")
        row = dict(parsed, at=item.get("submitted_at") or item.get("created_at") or "")
        prior = by_id.get(row["id"])
        if prior and (prior["sha"], prior["verdict"]) != (row["sha"], row["verdict"]):
            raise AdoptionError(f"contradictory duplicate Reviewer ID {row['id']}")
        if not prior or row["at"] < prior["at"]:
            by_id[row["id"]] = row
    return sorted(by_id.values(), key=lambda row: row["at"])


def body_field(body: str, name: str, *, required: bool = True) -> str | None:
    matches = re.findall(rf"^{re.escape(name)}:\s*`?([^`\r\n]+?)`?\s*$", body,
                         re.MULTILINE | re.IGNORECASE)
    if not matches and not required:
        return None
    if len(matches) != 1:
        raise AdoptionError(f"{name} must occur exactly once, found {len(matches)}")
    return matches[0].strip()


def marker_fields(body: str) -> dict[str, str]:
    result: dict[str, str] = {}
    for raw in body.splitlines():
        if ":" not in raw:
            continue
        key, value = raw.split(":", 1)
        result[key.strip().lower()] = value.strip().strip("`*_ .")
    return result


def review_ready_context_matches(pr: dict[str, Any], item: dict[str, Any], sha: str) -> bool:
    fields = marker_fields(item.get("body") or "")
    try:
        context = validation_context.resolve_context(pr, sha)
    except (OSError, ValueError, AttributeError, KeyError) as exc:
        raise AdoptionError(f"validation context invalid: {exc}") from exc
    digest = context["context_digest"]
    return (
        fields.get("state") == "REVIEW_READY" and
        fields.get("target sha", "").lower() == sha and
        fields.get("key") == f"review-ready:{pr['number']}:{sha}:{digest}" and
        fields.get("validation context digest", "").lower() == digest and
        fields.get("effective wp") == context["wp"] and
        fields.get("process only", "").lower() == context["process_only"] and
        fields.get("non foundational", "").lower() == context["non_foundational"]
    )


def current_review_ready(comments: list[dict[str, Any]], pr: dict[str, Any], sha: str,
                         before: str) -> dict[str, Any] | None:
    candidates = []
    for item in comments:
        if (item.get("user") or {}).get("login") != "github-actions[bot]":
            continue
        body = item.get("body") or ""
        if "ARKUS_AUTOMATION_V2" not in body:
            continue
        if ((item.get("created_at") or "") <= before and
                review_ready_context_matches(pr, item, sha)):
            candidates.append(item)
    return max(candidates, key=lambda item: item.get("created_at") or "") if candidates else None


def adoption_marker(comments: list[dict[str, Any]], review_id: str) -> dict[str, str] | None:
    found = []
    for item in comments:
        if (item.get("user") or {}).get("login") != "github-actions[bot]":
            continue
        body = item.get("body") or ""
        if "ARKUS_LOCAL_AUTOPILOT" not in body:
            continue
        fields = marker_fields(body)
        if fields.get("state") == "REVIEW_VERDICT_ADOPTED" and fields.get("review id", "").lower() == review_id:
            found.append(fields)
    if len(found) > 1:
        first = {(row.get("target sha"), row.get("verdict"), row.get("fail cycle")) for row in found}
        if len(first) != 1:
            raise AdoptionError(f"conflicting adoption markers for Reviewer ID {review_id}")
    return found[-1] if found else None


def repair_marker_exists(comments: list[dict[str, Any]], sha: str) -> bool:
    for item in comments:
        if (item.get("user") or {}).get("login") != "github-actions[bot]":
            continue
        body = item.get("body") or ""
        if "ARKUS_AUTOMATION_V2" not in body:
            continue
        fields = marker_fields(body)
        if fields.get("state") == "REPAIR_REQUIRED" and fields.get("target sha", "").lower() == sha:
            return True
    return False


def repair_marker_after_verdict(comments: list[dict[str, Any]], sha: str,
                                verdict_at: str) -> bool:
    """Detect a canonical bot transition already completed for this FAIL cycle."""
    for item in comments:
        if (item.get("user") or {}).get("login") != "github-actions[bot]":
            continue
        fields = marker_fields(item.get("body") or "")
        if ("ARKUS_AUTOMATION_V2" in (item.get("body") or "") and
                fields.get("state") == "REPAIR_REQUIRED" and
                fields.get("target sha", "").lower() == sha and
                (item.get("created_at") or "") >= verdict_at):
            return True
    return False


def fail_cycle(body: str) -> int:
    raw = body_field(body, "fail_cycle", required=False)
    if raw is None:
        return 0
    if not raw.isdecimal():
        raise AdoptionError("fail_cycle must be a non-negative integer")
    return int(raw)


def set_fail_cycle(body: str, value: int) -> str:
    pattern = re.compile(r"^([ \t]*fail_cycle:\s*)`?([0-9]+)`?\s*$", re.MULTILINE | re.IGNORECASE)
    matches = list(pattern.finditer(body))
    if len(matches) > 1:
        raise AdoptionError("fail_cycle occurs more than once")
    replacement = rf"\g<1>`{value}`"
    if matches:
        return pattern.sub(replacement, body, count=1)
    suffix = "" if body.endswith("\n") else "\n"
    return body + suffix + f"fail_cycle: `{value}`\n"


def post_comment(pr: int, body: str) -> None:
    gh_json("api", "--method", "POST", f"repos/{REPO}/issues/{pr}/comments", "-f", f"body={body}")


def update_pr_body(pr: int, body: str) -> None:
    gh_json("api", "--method", "PATCH", f"repos/{REPO}/pulls/{pr}", "-f", f"body={body}")


def validate_pr(pr: dict[str, Any], target_sha: str) -> str:
    if pr.get("state") != "open" or pr.get("merged") or pr.get("draft"):
        raise AdoptionError("Reviewer verdict adoption requires an open non-draft PR")
    body = pr.get("body") or ""
    head = ((pr.get("head") or {}).get("sha") or "").lower()
    if head != target_sha or not SHA_RE.fullmatch(head):
        raise AdoptionError(f"target SHA {target_sha} is not current PR HEAD {head or 'missing'}")
    for name, expected in {
        "Worker state": "FROZEN_FOR_REVIEW",
        "Branch frozen": "YES",
        "Worker verdict": "IN_REVIEW",
        "Worker pre-review": "CLEAN",
    }.items():
        if (body_field(body, name) or "").upper() != expected:
            raise AdoptionError(f"{name} is not {expected}")
    for name in ("Candidate HEAD SHA", "Frozen candidate SHA"):
        if (body_field(body, name) or "").lower() != target_sha:
            raise AdoptionError(f"{name} does not match current exact SHA")
    return body


def adopt(pr_number: int, target_sha: str, review_id: str, expected_verdict: str) -> dict[str, Any]:
    target_sha = target_sha.lower()
    review_id = review_id.lower()
    expected_verdict = expected_verdict.upper()
    if not SHA_RE.fullmatch(target_sha) or not REVIEW_ID_RE.fullmatch(review_id) or expected_verdict not in {"PASS", "FAIL"}:
        raise AdoptionError("invalid adoption request identity")

    pr = gh_json("api", f"repos/{REPO}/pulls/{pr_number}")
    if not isinstance(pr, dict):
        raise AdoptionError("PR lookup returned malformed data")
    body = validate_pr(pr, target_sha)
    reviews = gh_pages(f"repos/{REPO}/pulls/{pr_number}/reviews?per_page=100")
    comments = gh_pages(f"repos/{REPO}/issues/{pr_number}/comments?per_page=100")
    verdicts = authoritative_verdicts(reviews, comments)
    matching = [row for row in verdicts if row["id"] == review_id]
    if len(matching) != 1:
        raise AdoptionError(f"Reviewer ID {review_id} does not resolve uniquely")
    verdict = matching[0]
    if verdict["sha"] != target_sha:
        raise AdoptionError(f"Reviewer ID {review_id} names wrong SHA {verdict['sha']}")
    if verdict["verdict"] != expected_verdict:
        raise AdoptionError(f"Reviewer ID {review_id} verdict conflict: {verdict['verdict']}")
    ready = current_review_ready(comments, pr, target_sha, verdict["at"])
    if ready is None:
        raise AdoptionError("no current context-bound authoritative REVIEW_READY marker predates the Reviewer verdict")
    ready_at = ready.get("created_at") or ""
    for row in verdicts:
        if row["at"] >= ready_at and row["sha"] != target_sha:
            raise AdoptionError(f"wrong-SHA Reviewer verdict appeared in the current REVIEW_READY cycle: {row['sha']}")

    existing = adoption_marker(comments, review_id)
    if existing:
        if (existing.get("target sha", "").lower(), existing.get("verdict", "").upper()) != (target_sha, expected_verdict):
            raise AdoptionError("existing adoption marker contradicts requested verdict")
        if expected_verdict == "FAIL":
            cycle_raw = existing.get("fail cycle", "")
            if not cycle_raw.isdecimal() or int(cycle_raw) < 1:
                raise AdoptionError("existing FAIL adoption marker has invalid fail cycle")
            target_cycle = int(cycle_raw)
            if fail_cycle(body) != target_cycle:
                body = set_fail_cycle(body, target_cycle)
                update_pr_body(pr_number, body)
            if not repair_marker_after_verdict(comments, target_sha, verdict["at"]):
                post_comment(pr_number,
                    "ARKUS_AUTOMATION_V2\nState: REPAIR_REQUIRED\n"
                    f"Key: adopted-repair-required:{pr_number}:{target_sha}:{review_id}\n"
                    f"Target SHA: {target_sha}\n"
                    "Detail: independently published exact-SHA FAIL adopted from GitHub; start a fresh Repair Worker.\n")
        return {"status": "already-adopted", "verdict": expected_verdict,
                "fail_cycle": int(existing.get("fail cycle", "0") or 0)}

    if expected_verdict == "PASS":
        post_comment(pr_number,
            "ARKUS_LOCAL_AUTOPILOT\nState: REVIEW_VERDICT_ADOPTED\n"
            f"Target SHA: {target_sha}\nReview ID: {review_id}\nVerdict: PASS\n"
            "Detail: authoritative exact-SHA Reviewer PASS adopted from GitHub; PR body verdict text is non-authoritative.\n")
        return {"status": "adopted", "verdict": "PASS", "fail_cycle": fail_cycle(body)}

    already_transitioned = repair_marker_after_verdict(comments, target_sha, verdict["at"])
    current_cycle = fail_cycle(body)
    if already_transitioned:
        # The bot transition proves that this FAIL was consumed. Count distinct
        # owner-authored FAIL IDs to repair a lagging body without incrementing it.
        accepted_fails = sum(row["verdict"] == "FAIL" and row["at"] <= verdict["at"]
                             for row in verdicts)
        target_cycle = max(current_cycle, accepted_fails)
        if target_cycle < 1:
            raise AdoptionError("REPAIR_REQUIRED exists without a reconstructible FAIL cycle")
    else:
        target_cycle = current_cycle + 1
    # The ledger is written first. If a later write is interrupted, rerunning the
    # same request reconciles body + REPAIR_REQUIRED to this recorded target cycle
    # instead of incrementing a second time.
    post_comment(pr_number,
        "ARKUS_LOCAL_AUTOPILOT\nState: REVIEW_VERDICT_ADOPTED\n"
        f"Target SHA: {target_sha}\nReview ID: {review_id}\nVerdict: FAIL\nFail cycle: {target_cycle}\n"
        "Detail: authoritative exact-SHA Reviewer FAIL adopted from GitHub; idempotent repair transition follows.\n")
    if current_cycle != target_cycle:
        body = set_fail_cycle(body, target_cycle)
        update_pr_body(pr_number, body)
    if not repair_marker_after_verdict(comments, target_sha, verdict["at"]):
        post_comment(pr_number,
            "ARKUS_AUTOMATION_V2\nState: REPAIR_REQUIRED\n"
            f"Key: adopted-repair-required:{pr_number}:{target_sha}:{review_id}\n"
            f"Target SHA: {target_sha}\n"
            "Detail: independently published exact-SHA FAIL adopted from GitHub; start a fresh Repair Worker.\n")
    return {"status": "adopted", "verdict": "FAIL", "fail_cycle": target_cycle}


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--pr", type=int, required=True)
    parser.add_argument("--sha", required=True)
    parser.add_argument("--review-id", required=True)
    parser.add_argument("--verdict", choices=("PASS", "FAIL"), required=True)
    args = parser.parse_args()
    try:
        result = adopt(args.pr, args.sha, args.review_id, args.verdict)
    except (AdoptionError, OSError, json.JSONDecodeError) as exc:
        print(f"ADOPTION_STOP: {exc}", flush=True)
        return 2
    print(json.dumps(result, sort_keys=True))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
