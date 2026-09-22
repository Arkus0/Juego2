#!/usr/bin/env python3
"""Complete mutable-input identity for same-SHA receipt reuse.

The ordinary validation-context digest intentionally describes routing policy.
Receipt reuse needs a stronger identity: every mutable PR-body byte consumed by
handoff validation plus mutable external Worker-pre-review comment content.
Using the complete body is a safe superset: harmless edits may invalidate reuse,
but a consumed field can never change while the reuse identity stays equal.
"""
from __future__ import annotations

import argparse
import hashlib
import json
import os
from pathlib import Path
import re
import sys
import urllib.error
import urllib.request
from typing import Any

SCHEMA = "ARKUS_REUSE_CONTEXT_V1"
SHA_RE = re.compile(r"^[0-9a-f]{40}$", re.I)
POINTER_RE = re.compile(
    r"^https://github\.com/([^/]+)/([^/]+)/pull/(\d+)#issuecomment-(\d+)$", re.I
)
FIELD_RE = re.compile(r"^Worker pre-review evidence:\s*`?([^`\r\n]+?)`?\s*$", re.I | re.M)


def sha256_text(value: str) -> str:
    return hashlib.sha256(value.encode("utf-8")).hexdigest()


def _single_pointer(body: str) -> str:
    values = FIELD_RE.findall(body)
    if len(values) > 1:
        raise ValueError(f"ambiguous Worker pre-review evidence: found {len(values)} fields")
    return values[0].strip() if values else "NONE"


def fetch_comment(pointer: str) -> tuple[str, str, str]:
    match = POINTER_RE.fullmatch(pointer)
    if not match:
        raise ValueError("external Worker pre-review pointer is not a canonical GitHub PR issue-comment URL")
    owner, repo, pr_number, comment_id = match.groups()
    url = f"https://api.github.com/repos/{owner}/{repo}/issues/comments/{comment_id}"
    headers = {
        "Accept": "application/vnd.github+json",
        "User-Agent": "arkus-reuse-context",
        "X-GitHub-Api-Version": "2022-11-28",
    }
    token = os.environ.get("GH_TOKEN") or os.environ.get("GITHUB_TOKEN")
    if token:
        headers["Authorization"] = f"Bearer {token}"
    request = urllib.request.Request(url, headers=headers)
    try:
        with urllib.request.urlopen(request, timeout=15) as response:
            payload = json.loads(response.read().decode("utf-8"))
    except (urllib.error.URLError, urllib.error.HTTPError, TimeoutError, json.JSONDecodeError) as exc:
        raise RuntimeError(f"cannot fetch external Worker pre-review comment: {exc}") from exc
    body = str(payload.get("body") or "")
    html_url = str(payload.get("html_url") or "")
    issue_url = str(payload.get("issue_url") or "")
    if html_url.rstrip("/") != pointer.rstrip("/"):
        raise ValueError("fetched external pre-review comment URL does not equal pointer")
    if not issue_url.rstrip("/").endswith(f"/issues/{pr_number}"):
        raise ValueError("fetched external pre-review comment belongs to a different PR")
    return body, html_url, issue_url


def context_payload(
    pr: dict[str, Any],
    target_sha: str,
    *,
    external_body_override: str | None = None,
) -> dict[str, str]:
    target = target_sha.strip().lower()
    if not SHA_RE.fullmatch(target):
        raise ValueError(f"candidate SHA is not exact: {target_sha!r}")
    head = str(((pr.get("head") or {}).get("sha") or "")).lower()
    if not SHA_RE.fullmatch(head):
        raise ValueError("PR head SHA is missing or invalid")
    if head != target:
        raise ValueError(f"candidate SHA {target} != PR HEAD {head}")
    number = pr.get("number")
    if not isinstance(number, int) or number <= 0:
        raise ValueError("PR number is missing or invalid")

    body = str(pr.get("body") or "")
    pointer = _single_pointer(body)
    external_hash = "NONE"
    if pointer != "NONE" and re.match(r"^https?://", pointer, re.I):
        if external_body_override is None:
            external_body, _html, _issue = fetch_comment(pointer)
        else:
            external_body = external_body_override
        external_hash = sha256_text(external_body)

    payload = {
        "schema": SCHEMA,
        "pr_number": str(number),
        "head_sha": head,
        "draft": "true" if bool(pr.get("draft")) else "false",
        "body_sha256": sha256_text(body),
        "external_pre_review_pointer": pointer,
        "external_pre_review_sha256": external_hash,
    }
    return payload


def digest(payload: dict[str, str]) -> str:
    canonical = json.dumps(payload, sort_keys=True, separators=(",", ":"), ensure_ascii=True)
    return sha256_text(canonical)


def resolve(pr: dict[str, Any], target_sha: str, *, external_body_override: str | None = None) -> dict[str, str]:
    payload = context_payload(pr, target_sha, external_body_override=external_body_override)
    out = dict(payload)
    out["reuse_context_digest"] = digest(payload)
    return out


def write_record(path: Path, values: dict[str, str]) -> None:
    path.write_text(json.dumps(values, indent=2, sort_keys=True) + "\n", encoding="utf-8")


def self_test() -> None:
    sha = "a" * 40
    pointer = "https://github.com/Arkus0/Juego2/pull/77#issuecomment-123"
    body = f"""WP: WP-HK-00
Contract: Docs/workpacks/HK/WP-HK-00.md
Candidate HEAD SHA: {sha}
Worker pre-review evidence: {pointer}
"""
    pr = {"number": 77, "head": {"sha": sha}, "draft": False, "body": body}
    ext = f"WORKER_PRE_REVIEW: CLEAN\nCandidate SHA: {sha}\n"
    first = resolve(pr, sha, external_body_override=ext)
    second = resolve(pr, sha, external_body_override=ext)
    if first != second:
        raise AssertionError("identical inputs did not produce stable reuse context")

    edited = dict(pr, body=body.replace("WP-HK-00.md", "WP-HK-00-v2.md"))
    if resolve(edited, sha, external_body_override=ext)["reuse_context_digest"] == first["reuse_context_digest"]:
        raise AssertionError("PR-body Contract edit did not invalidate reuse digest")

    candidate_edit = dict(pr, body=body.replace(f"Candidate HEAD SHA: {sha}", f"Candidate HEAD SHA: {'b' * 40}"))
    if resolve(candidate_edit, sha, external_body_override=ext)["reuse_context_digest"] == first["reuse_context_digest"]:
        raise AssertionError("PR-body candidate-field edit did not invalidate reuse digest")

    ext_mutated = ext + "WORKER_PRE_REVIEW_EVIDENCE: changed\n"
    if resolve(pr, sha, external_body_override=ext_mutated)["reuse_context_digest"] == first["reuse_context_digest"]:
        raise AssertionError("external pre-review comment mutation did not invalidate reuse digest")

    draft_pr = dict(pr, draft=True)
    if resolve(draft_pr, sha, external_body_override=ext)["reuse_context_digest"] == first["reuse_context_digest"]:
        raise AssertionError("Draft-state mutation did not invalidate reuse digest")

    try:
        resolve(pr, "b" * 40, external_body_override=ext)
    except ValueError as exc:
        if "!= PR HEAD" not in str(exc):
            raise
    else:
        raise AssertionError("mismatched target SHA stayed GREEN")

    duplicate = dict(pr, body=body + f"Worker pre-review evidence: {pointer}\n")
    try:
        resolve(duplicate, sha, external_body_override=ext)
    except ValueError as exc:
        if "ambiguous" not in str(exc):
            raise
    else:
        raise AssertionError("duplicate external pointer stayed GREEN")

    print("REUSE_CONTEXT_SELF_TEST_GREEN")


def main() -> int:
    parser = argparse.ArgumentParser()
    sub = parser.add_subparsers(dest="command", required=True)
    sub.add_parser("self-test")
    resolve_p = sub.add_parser("resolve")
    resolve_p.add_argument("--pr-json", required=True)
    resolve_p.add_argument("--target-sha", required=True)
    resolve_p.add_argument("--record")
    resolve_p.add_argument("--github-output")
    args = parser.parse_args()

    try:
        if args.command == "self-test":
            self_test()
            return 0
        pr = json.loads(Path(args.pr_json).read_text(encoding="utf-8"))
        values = resolve(pr, args.target_sha)
        if args.record:
            write_record(Path(args.record), values)
        if args.github_output:
            with Path(args.github_output).open("a", encoding="utf-8") as out:
                for key, value in values.items():
                    out.write(f"{key}={value}\n")
        print(values["reuse_context_digest"])
        return 0
    except (ValueError, RuntimeError, OSError, json.JSONDecodeError) as exc:
        print(f"REUSE_CONTEXT_ERROR: {exc}", file=sys.stderr)
        return 2


if __name__ == "__main__":
    raise SystemExit(main())
