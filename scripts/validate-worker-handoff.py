#!/usr/bin/env python3
"""Mechanical validator for the canonical Worker -> Reviewer PR handoff.

This validates only structure/coherence that CI can establish without making
semantic Reviewer judgments. PROCESS_ONLY maintenance/DocSync PRs that never
enter the Worker lifecycle are ignored; PROCESS_ONLY workpacks that publish
Worker lifecycle fields are validated normally.

After CTX-03, final Worker pre-review evidence may be a durable GitHub PR issue
comment created after the last repository/evidence byte mutation. When such a
pointer is used, this validator proves that the comment really exists on the
same repository/PR and binds CLEAN to the exact candidate SHA. It still does
not decide whether the Worker's semantic pre-review judgment was correct.
"""

from __future__ import annotations

import argparse
import json
import os
from pathlib import Path
import re
import sys
import tempfile
import urllib.error
import urllib.request

SHA_RE = re.compile(r"^[0-9a-f]{40}$", re.I)
PROCESS_ONLY_RE = re.compile(
    r"^(?:Mode|WORKFLOW_MODE):\s*`?PROCESS_ONLY`?\s*$", re.I | re.M
)
PRE_REVIEW_COMMENT_RE = re.compile(
    r"^https://github\.com/([^/]+)/([^/]+)/pull/(\d+)#issuecomment-(\d+)$",
    re.I,
)

REQUIRED_FIELDS = (
    "WP",
    "Contract",
    "Baseline SHA",
    "Active Worker",
    "Worker state",
    "Worker history",
    "Transfer SHA",
    "Predecessor contract check",
    "Candidate HEAD SHA",
    "Worker pre-review",
    "Worker pre-review evidence",
    "Frozen candidate SHA",
    "Branch frozen",
    "Worker verdict",
    "Reviewer verdict",
    "Reviewed candidate SHA",
    "Evidence",
    "fail_cycle",
)

LIFECYCLE_SENTINELS = (
    "WP",
    "Contract",
    "Worker state",
    "Candidate HEAD SHA",
    "Worker pre-review",
    "Frozen candidate SHA",
    "Branch frozen",
)


def field(body: str, name: str) -> str | None:
    match = re.search(
        rf"^{re.escape(name)}:\s*`?([^`\r\n]+?)`?\s*$", body, re.I | re.M
    )
    return match.group(1).strip() if match else None


def is_none(value: str | None) -> bool:
    return value is not None and value.strip().upper() == "NONE"


def local_pointer(value: str | None, root: Path) -> Path | None:
    """Resolve a repository-local pointer when possible."""
    if value is None or is_none(value):
        return None
    value = value.strip().strip("`")
    if re.match(r"^https?://", value, re.I):
        return None

    raw = value.split("#", 1)[0].strip()
    match = re.match(r"^(.+?\.(?:md|json|txt|ya?ml|toml|xml))(?:@.+)?$", raw, re.I)
    if match:
        raw = match.group(1)
    return root / raw


def contract_has_dependency(contract_path: Path | None) -> bool:
    if contract_path is None or not contract_path.is_file():
        return False
    text = contract_path.read_text(encoding="utf-8")
    match = re.search(r"^Depends on:\s*(.+?)\s*$", text, re.I | re.M)
    if not match:
        return False
    value = match.group(1).strip().strip("`*_ ")
    return value.lower() not in {"none", "n/a", "na", "-", "nothing"}


def external_pre_review_errors(
    pointer: str,
    head_sha: str,
    comment_body: str,
    current_repo: str | None,
    current_pr: str | int | None,
    fetched_html_url: str | None = None,
    fetched_issue_url: str | None = None,
) -> list[str]:
    errors: list[str] = []
    match = PRE_REVIEW_COMMENT_RE.fullmatch(pointer.strip())
    if not match:
        return ["Worker pre-review evidence URL must be a GitHub PR issue-comment URL"]

    owner, repo, pr_number, _comment_id = match.groups()
    pointer_repo = f"{owner}/{repo}"
    if current_repo and pointer_repo.lower() != current_repo.lower():
        errors.append(
            f"Worker pre-review evidence points to {pointer_repo}, expected {current_repo}"
        )
    if current_pr is not None and str(pr_number) != str(current_pr):
        errors.append(
            f"Worker pre-review evidence points to PR #{pr_number}, expected PR #{current_pr}"
        )
    if fetched_html_url and fetched_html_url.rstrip("/") != pointer.strip().rstrip("/"):
        errors.append("Fetched pre-review comment URL does not equal handoff pointer")
    if fetched_issue_url and not fetched_issue_url.rstrip("/").endswith(f"/issues/{pr_number}"):
        errors.append("Fetched pre-review comment belongs to a different issue/PR")

    clean = re.search(r"^WORKER_PRE_REVIEW:\s*CLEAN\s*$", comment_body, re.I | re.M)
    candidate = re.search(
        r"^Candidate SHA:\s*([0-9a-fA-F]{40})\s*$", comment_body, re.M
    )
    findings = re.search(
        r"^WORKER_PRE_REVIEW_FINDINGS_FIXED:\s*(\d+)\s*$", comment_body, re.I | re.M
    )
    evidence = re.search(
        r"^WORKER_PRE_REVIEW_EVIDENCE:\s*(\S.*?)\s*$", comment_body, re.I | re.M
    )
    if clean is None:
        errors.append("External Worker pre-review comment does not contain WORKER_PRE_REVIEW: CLEAN")
    if candidate is None:
        errors.append("External Worker pre-review comment lacks exact Candidate SHA")
    elif candidate.group(1).lower() != head_sha.lower():
        errors.append(
            f"External Worker pre-review Candidate SHA {candidate.group(1).lower()} != PR HEAD {head_sha.lower()}"
        )
    if findings is None:
        errors.append("External Worker pre-review comment lacks WORKER_PRE_REVIEW_FINDINGS_FIXED")
    if evidence is None or not evidence.group(1).strip():
        errors.append("External Worker pre-review comment lacks WORKER_PRE_REVIEW_EVIDENCE")
    return errors


def current_pr_from_event() -> str | None:
    direct = os.environ.get("PR") or os.environ.get("CURRENT_PR_NUMBER")
    if direct:
        return direct
    event_path = os.environ.get("GITHUB_EVENT_PATH")
    if not event_path:
        return None
    try:
        payload = json.loads(Path(event_path).read_text(encoding="utf-8"))
    except (OSError, json.JSONDecodeError):
        return None
    number = (payload.get("pull_request") or {}).get("number")
    if number is None:
        number = (payload.get("issue") or {}).get("number")
    return str(number) if number is not None else None


def fetch_external_pre_review(pointer: str) -> tuple[str, str | None, str | None]:
    match = PRE_REVIEW_COMMENT_RE.fullmatch(pointer.strip())
    if not match:
        raise ValueError("not a GitHub PR issue-comment URL")
    owner, repo, _pr_number, comment_id = match.groups()
    url = f"https://api.github.com/repos/{owner}/{repo}/issues/comments/{comment_id}"
    headers = {
        "Accept": "application/vnd.github+json",
        "User-Agent": "arkus-worker-handoff-lint",
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
    return (
        str(payload.get("body") or ""),
        None if payload.get("html_url") is None else str(payload["html_url"]),
        None if payload.get("issue_url") is None else str(payload["issue_url"]),
    )


def validate(
    body: str,
    head_sha: str,
    root: Path,
    *,
    external_comment_body: str | None = None,
    current_repo: str | None = None,
    current_pr: str | int | None = None,
    fetched_html_url: str | None = None,
    fetched_issue_url: str | None = None,
) -> list[str]:
    errors: list[str] = []
    process_only = bool(PROCESS_ONLY_RE.search(body))
    lifecycle = any(field(body, name) is not None for name in LIFECYCLE_SENTINELS)

    if process_only and not lifecycle:
        return errors
    if not lifecycle:
        return [
            "Ready non-PROCESS_ONLY PR is missing the canonical Worker handoff; "
            "publish all Required PR handoff fields before review."
        ]

    values = {name: field(body, name) for name in REQUIRED_FIELDS}
    for name, value in values.items():
        if value is None or not value.strip():
            errors.append(f"Missing required handoff field: {name}")
    if errors:
        return errors

    head_sha = head_sha.lower()
    if not SHA_RE.fullmatch(head_sha):
        errors.append(f"HEAD SHA is not an exact 40-character SHA: {head_sha!r}")

    for name in ("Baseline SHA", "Candidate HEAD SHA", "Frozen candidate SHA"):
        value = values[name]
        assert value is not None
        if not SHA_RE.fullmatch(value):
            errors.append(f"{name} must be an exact 40-character SHA, got {value!r}")

    for name in ("Transfer SHA", "Reviewed candidate SHA"):
        value = values[name]
        assert value is not None
        if not is_none(value) and not SHA_RE.fullmatch(value):
            errors.append(f"{name} must be NONE or an exact 40-character SHA, got {value!r}")

    candidate = (values["Candidate HEAD SHA"] or "").lower()
    frozen = (values["Frozen candidate SHA"] or "").lower()
    if SHA_RE.fullmatch(head_sha) and candidate != head_sha:
        errors.append(f"Candidate HEAD SHA {candidate} != PR HEAD {head_sha}")
    if SHA_RE.fullmatch(head_sha) and frozen != head_sha:
        errors.append(f"Frozen candidate SHA {frozen} != PR HEAD {head_sha}")

    expected = {
        "Worker state": "FROZEN_FOR_REVIEW",
        "Worker pre-review": "CLEAN",
        "Branch frozen": "YES",
        "Worker verdict": "IN_REVIEW",
        "Reviewer verdict": "PENDING",
    }
    for name, required in expected.items():
        got = (values[name] or "").upper()
        if got != required:
            errors.append(f"{name} must be {required} at Ready/freeze, got {values[name]!r}")

    fail_cycle = values["fail_cycle"] or ""
    if not re.fullmatch(r"\d+", fail_cycle):
        errors.append(f"fail_cycle must be a non-negative integer, got {fail_cycle!r}")

    pointer_fields = (
        "Contract",
        "Predecessor contract check",
        "Worker pre-review evidence",
        "Evidence",
    )
    pointers: dict[str, Path | None] = {}
    for name in pointer_fields:
        value = values[name]
        if name != "Predecessor contract check" and is_none(value):
            errors.append(f"{name} may not be NONE at Ready/freeze")
            pointers[name] = None
            continue
        path = local_pointer(value, root)
        pointers[name] = path
        if path is not None and not path.exists():
            errors.append(f"{name} points to missing repository path: {path.relative_to(root)}")

    contract_path = pointers.get("Contract")
    predecessor_value = values["Predecessor contract check"]
    if contract_has_dependency(contract_path) and is_none(predecessor_value):
        errors.append("Contract declares a dependency but Predecessor contract check is NONE")

    pred_path = pointers.get("Predecessor contract check")
    if pred_path is not None and pred_path.is_file():
        pred_text = pred_path.read_text(encoding="utf-8")
        if "PREDECESSOR_CONTRACT_CHECK" not in pred_text.upper():
            errors.append(
                "Predecessor contract check evidence does not contain PREDECESSOR_CONTRACT_CHECK"
            )

    pre_value = values["Worker pre-review evidence"] or ""
    pre_path = pointers.get("Worker pre-review evidence")
    if pre_path is not None and pre_path.is_file():
        pre_text = pre_path.read_text(encoding="utf-8")
        if not re.search(r"WORKER_PRE_REVIEW:\s*CLEAN", pre_text, re.I):
            errors.append("Worker pre-review evidence does not contain WORKER_PRE_REVIEW: CLEAN")
    elif re.match(r"^https?://", pre_value, re.I):
        if external_comment_body is None:
            errors.append("External Worker pre-review evidence was not fetched/validated")
        else:
            errors += external_pre_review_errors(
                pre_value,
                head_sha,
                external_comment_body,
                current_repo,
                current_pr,
                fetched_html_url,
                fetched_issue_url,
            )

    return errors


def make_body(head: str, contract: str, pred: str, pre: str, evidence: str) -> str:
    return f"""WP: WP-TEST-01
Contract: {contract}
Baseline SHA: {'1' * 40}
Active Worker: test-worker
Worker state: FROZEN_FOR_REVIEW
Worker history: test-worker
Transfer SHA: NONE
Predecessor contract check: {pred}
Candidate HEAD SHA: {head}
Worker pre-review: CLEAN
Worker pre-review evidence: {pre}
Frozen candidate SHA: {head}
Branch frozen: YES
Worker verdict: IN_REVIEW
Reviewer verdict: PENDING
Reviewed candidate SHA: NONE
Evidence: {evidence}
fail_cycle: 0
"""


def self_test() -> None:
    head = "a" * 40
    with tempfile.TemporaryDirectory() as td:
        root = Path(td)
        (root / "contract-none.md").write_text("Depends on: none\n", encoding="utf-8")
        (root / "contract-dep.md").write_text("Depends on: WP-X\n", encoding="utf-8")
        (root / "pred.md").write_text("# PREDECESSOR_CONTRACT_CHECK\n", encoding="utf-8")
        (root / "bad-pred.md").write_text("# predecessor notes\n", encoding="utf-8")
        (root / "pre.md").write_text("WORKER_PRE_REVIEW: CLEAN\n", encoding="utf-8")
        (root / "evidence.md").write_text("evidence\n", encoding="utf-8")

        valid = make_body(head, "contract-none.md", "NONE", "pre.md", "evidence.md")
        assert validate(valid, head, root) == []

        dep_valid = make_body(head, "contract-dep.md", "pred.md", "pre.md", "evidence.md")
        assert validate(dep_valid, head, root) == []

        missing_pred = make_body(head, "contract-dep.md", "NONE", "pre.md", "evidence.md")
        assert any("declares a dependency" in e for e in validate(missing_pred, head, root))

        bad_pred = make_body(head, "contract-dep.md", "bad-pred.md", "pre.md", "evidence.md")
        assert any("does not contain PREDECESSOR_CONTRACT_CHECK" in e for e in validate(bad_pred, head, root))

        missing_field = valid.replace("Worker history: test-worker\n", "")
        assert any("Worker history" in e for e in validate(missing_field, head, root))

        docsync = "WORKFLOW_MODE: PROCESS_ONLY\nDocumentation reconciliation only.\n"
        assert validate(docsync, head, root) == []

        partial_process = "WORKFLOW_MODE: PROCESS_ONLY\nWorker state: FROZEN_FOR_REVIEW\n"
        assert validate(partial_process, head, root)

        external = "https://github.com/Arkus0/Juego2/pull/121#issuecomment-123"
        ext_body = f"""WORKER_PRE_REVIEW: CLEAN
Candidate SHA: {head}
WORKER_PRE_REVIEW_FINDINGS_FIXED: 12
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/CTX-03/ + exact CI runs
"""
        external_valid = make_body(head, "contract-dep.md", "pred.md", external, "evidence.md")
        assert validate(
            external_valid,
            head,
            root,
            external_comment_body=ext_body,
            current_repo="Arkus0/Juego2",
            current_pr="121",
            fetched_html_url=external,
            fetched_issue_url="https://api.github.com/repos/Arkus0/Juego2/issues/121",
        ) == []
        wrong_sha = ext_body.replace(head, "b" * 40)
        assert any(
            "Candidate SHA" in e
            for e in validate(
                external_valid,
                head,
                root,
                external_comment_body=wrong_sha,
                current_repo="Arkus0/Juego2",
                current_pr="121",
            )
        )
        assert any(
            "expected PR #121" in e
            for e in validate(
                external_valid.replace("pull/121", "pull/122"),
                head,
                root,
                external_comment_body=ext_body,
                current_repo="Arkus0/Juego2",
                current_pr="121",
            )
        )

    print("worker-handoff self-test: PASS")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--head-sha", default=os.environ.get("TARGET_SHA", ""))
    parser.add_argument("--repo-root", default=".")
    parser.add_argument("--body-env", default="PR_BODY")
    parser.add_argument("--self-test", action="store_true")
    args = parser.parse_args()

    if args.self_test:
        self_test()
        return 0

    body = os.environ.get(args.body_env, "")
    pointer = field(body, "Worker pre-review evidence") or ""
    external_body = None
    fetched_html_url = None
    fetched_issue_url = None
    prefetch_errors: list[str] = []
    if re.match(r"^https?://", pointer, re.I):
        try:
            external_body, fetched_html_url, fetched_issue_url = fetch_external_pre_review(pointer)
        except (ValueError, RuntimeError) as exc:
            prefetch_errors.append(str(exc))

    errors = prefetch_errors + validate(
        body,
        args.head_sha,
        Path(args.repo_root).resolve(),
        external_comment_body=external_body,
        current_repo=os.environ.get("GITHUB_REPOSITORY"),
        current_pr=current_pr_from_event(),
        fetched_html_url=fetched_html_url,
        fetched_issue_url=fetched_issue_url,
    )
    if errors:
        print("Worker handoff lint: FAIL", file=sys.stderr)
        for error in errors:
            print(f"- {error}", file=sys.stderr)
        return 2

    print("Worker handoff lint: GREEN")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
