#!/usr/bin/env python3
"""Fail-closed live Worker-handoff guard for Reviewer PASS preflight.

Historical exact-SHA checks are reusable only for immutable inputs. The Worker
handoff also consumes mutable PR-body metadata and, for CTX-era pre-review
pointers, mutable GitHub issue-comment evidence. This guard deliberately reruns
the canonical Worker handoff oracle against the latest PR JSON and the exact
reviewed candidate tree immediately before merge.
"""
from __future__ import annotations

import argparse
import importlib.util
import json
import os
from pathlib import Path
import re
import sys
import tempfile
from types import ModuleType

SCRIPT_DIR = Path(__file__).resolve().parent


def load_module(filename: str, name: str) -> ModuleType:
    spec = importlib.util.spec_from_file_location(name, SCRIPT_DIR / filename)
    if spec is None or spec.loader is None:
        raise RuntimeError(f"cannot load canonical oracle {filename}")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


HANDOFF = load_module("validate-worker-handoff.py", "arkus_worker_handoff")
CONTEXT = load_module("validation-context.py", "arkus_validation_context")


def validate_live(pr: dict, target_sha: str, repo_root: Path) -> list[str]:
    target = target_sha.strip().lower()
    body = str(pr.get("body") or "")
    number = pr.get("number")
    head = str(((pr.get("head") or {}).get("sha") or "")).lower()

    errors: list[str] = []
    if not HANDOFF.SHA_RE.fullmatch(target):
        return [f"PASS preflight target SHA is not exact: {target_sha!r}"]
    if head != target:
        return [f"PASS preflight PR HEAD {head!r} != reviewed SHA {target}"]
    if not isinstance(number, int) or number <= 0:
        return ["PASS preflight PR number is missing or invalid"]

    pointer = HANDOFF.field(body, "Worker pre-review evidence") or ""
    external_body = None
    fetched_html_url = None
    fetched_issue_url = None
    if re.match(r"^https?://", pointer, re.I):
        try:
            external_body, fetched_html_url, fetched_issue_url = HANDOFF.fetch_external_pre_review(pointer)
        except (ValueError, RuntimeError) as exc:
            errors.append(str(exc))

    errors += HANDOFF.validate(
        body,
        target,
        repo_root.resolve(),
        external_comment_body=external_body,
        current_repo=os.environ.get("GITHUB_REPOSITORY"),
        current_pr=number,
        fetched_html_url=fetched_html_url,
        fetched_issue_url=fetched_issue_url,
    )
    return errors


def require_live(pr: dict, target_sha: str, repo_root: Path) -> None:
    errors = validate_live(pr, target_sha, repo_root)
    if errors:
        raise ValueError("live Worker handoff is RED:\n- " + "\n- ".join(errors))


def run_self_test() -> None:
    sha = "a" * 40
    other = "b" * 40
    with tempfile.TemporaryDirectory() as td:
        root = Path(td)
        (root / "contract.md").write_text("Depends on: none\n", encoding="utf-8")
        (root / "pre.md").write_text("WORKER_PRE_REVIEW: CLEAN\n", encoding="utf-8")
        (root / "evidence.md").write_text("evidence\n", encoding="utf-8")

        body = HANDOFF.make_body(sha, "contract.md", "NONE", "pre.md", "evidence.md")
        pr = {"number": 77, "head": {"sha": sha}, "body": body}
        original_context = CONTEXT.resolve_context(pr, sha)
        assert HANDOFF.validate(body, sha, root) == []

        # Negative control for the stale-success class: the durable
        # PR+SHA+WP+mode/proof digest is unchanged, but a handoff-only input
        # mutates after REVIEW_READY. The live oracle must make PASS preflight RED.
        mutations = {
            "Contract": body.replace("Contract: contract.md", "Contract: missing-contract.md"),
            "Candidate HEAD SHA": body.replace(
                f"Candidate HEAD SHA: {sha}", f"Candidate HEAD SHA: {other}"
            ),
            "Worker pre-review evidence": body.replace(
                "Worker pre-review evidence: pre.md",
                "Worker pre-review evidence: missing-pre.md",
            ),
        }
        for label, mutated_body in mutations.items():
            mutated_pr = {"number": 77, "head": {"sha": sha}, "body": mutated_body}
            mutated_context = CONTEXT.resolve_context(mutated_pr, sha)
            assert mutated_context["context_digest"] == original_context["context_digest"], label
            assert HANDOFF.validate(mutated_body, sha, root), label

        # The same class also covers mutable external evidence behind an
        # unchanged pointer: historical GREEN cannot substitute for a live fetch.
        external = "https://github.com/Arkus0/Juego2/pull/77#issuecomment-123"
        external_body = HANDOFF.make_body(
            sha, "contract.md", "NONE", external, "evidence.md"
        )
        external_pr = {"number": 77, "head": {"sha": sha}, "body": external_body}
        assert CONTEXT.resolve_context(external_pr, sha)["context_digest"] == original_context["context_digest"]
        good_comment = (
            "WORKER_PRE_REVIEW: CLEAN\n"
            f"Candidate SHA: {sha}\n"
            "WORKER_PRE_REVIEW_FINDINGS_FIXED: 1\n"
            "WORKER_PRE_REVIEW_EVIDENCE: exact candidate audit\n"
        )
        assert HANDOFF.validate(
            external_body,
            sha,
            root,
            external_comment_body=good_comment,
            current_repo="Arkus0/Juego2",
            current_pr=77,
            fetched_html_url=external,
            fetched_issue_url="https://api.github.com/repos/Arkus0/Juego2/issues/77",
        ) == []
        stale_comment = good_comment.replace(sha, other)
        assert HANDOFF.validate(
            external_body,
            sha,
            root,
            external_comment_body=stale_comment,
            current_repo="Arkus0/Juego2",
            current_pr=77,
            fetched_html_url=external,
            fetched_issue_url="https://api.github.com/repos/Arkus0/Juego2/issues/77",
        )

    print("PASS preflight live-handoff self-test: PASS")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--self-test", action="store_true")
    parser.add_argument("--pr-json", type=Path)
    parser.add_argument("--target-sha")
    parser.add_argument("--repo-root", type=Path)
    args = parser.parse_args()

    if args.self_test:
        run_self_test()
        return 0
    if args.pr_json is None or args.target_sha is None or args.repo_root is None:
        parser.error("--pr-json, --target-sha and --repo-root are required unless --self-test is used")

    try:
        pr = json.loads(args.pr_json.read_text(encoding="utf-8"))
        require_live(pr, args.target_sha, args.repo_root)
    except (OSError, json.JSONDecodeError, ValueError, RuntimeError) as exc:
        print(f"PASS preflight live Worker handoff: FAIL: {exc}", file=sys.stderr)
        return 2

    print("PASS preflight live Worker handoff: GREEN")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
