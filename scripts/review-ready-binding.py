#!/usr/bin/env python3
"""Select and validate the current REVIEW_READY binding.

Normal Candidate Validation binds terminal review state to the validation-context
digest. Exact-SHA receipt reuse binds it to the stronger reuse-context digest.
This oracle is shared by closure and PASS transition so those consumers cannot
drift apart.
"""
from __future__ import annotations

import argparse
import importlib.util
import json
from pathlib import Path
import re
import sys
import tempfile
from typing import Any

HERE = Path(__file__).resolve().parent
SHA_RE = re.compile(r"^[0-9a-f]{40}$", re.I)
DIGEST_RE = re.compile(r"^[0-9a-f]{64}$", re.I)
ALLOWED_WORKFLOWS = {"Arkus Candidate Validation", "Arkus Receipt Freeze Reuse"}


class BindingError(ValueError):
    pass


def load_reuse_module():
    path = HERE / "reuse-context.py"
    spec = importlib.util.spec_from_file_location("arkus_reuse_context", path)
    if spec is None or spec.loader is None:
        raise BindingError(f"cannot import {path}")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


def field(body: str, name: str, *, required: bool = True) -> str | None:
    values = re.findall(rf"^{re.escape(name)}:\s*(.*?)\s*$", body, re.M)
    if not values and not required:
        return None
    if len(values) != 1:
        raise BindingError(f"{name}: expected exactly one field, got {len(values)}")
    return values[0].strip()


def bool_text(value: Any) -> str:
    if isinstance(value, bool):
        return "true" if value else "false"
    text = str(value).strip().lower()
    if text not in {"true", "false"}:
        raise BindingError(f"expected boolean value, got {value!r}")
    return text


def parse_marker(body: str) -> dict[str, str | None]:
    if body.splitlines().count("ARKUS_AUTOMATION_V2") != 1:
        raise BindingError("marker must contain exactly one ARKUS_AUTOMATION_V2 line")
    if field(body, "State") != "REVIEW_READY":
        raise BindingError("marker state is not REVIEW_READY")
    workflow = field(body, "Validation Workflow")
    if workflow not in ALLOWED_WORKFLOWS:
        raise BindingError(f"unsupported validation workflow: {workflow!r}")
    target = (field(body, "Target SHA") or "").lower()
    context_digest = (field(body, "Validation Context Digest") or "").lower()
    run_id = field(body, "Validation Run ID") or ""
    if not SHA_RE.fullmatch(target):
        raise BindingError("marker Target SHA is not exact 40-hex")
    if not DIGEST_RE.fullmatch(context_digest):
        raise BindingError("marker Validation Context Digest is not 64-hex")
    if not run_id.isdigit() or int(run_id) <= 0:
        raise BindingError("marker Validation Run ID is not positive integer")
    return {
        "key": field(body, "Key"),
        "target_sha": target,
        "wp": field(body, "Effective WP"),
        "process_only": (field(body, "Process Only") or "").lower(),
        "non_foundational": (field(body, "Non Foundational") or "").lower(),
        "context_digest": context_digest,
        "reuse_context_digest": (field(body, "Reuse Context Digest", required=False) or "").lower() or None,
        "validation_run_id": run_id,
        "validation_workflow": workflow,
    }


def current_reuse_digest(pr: dict[str, Any], target_sha: str) -> str:
    reuse = load_reuse_module()
    resolved = reuse.resolve(pr, target_sha)
    value = str(resolved.get("reuse_context_digest") or "").lower()
    if not DIGEST_RE.fullmatch(value):
        raise BindingError("current reuse-context digest is invalid")
    return value


def validate_marker(
    *,
    body: str,
    pr: dict[str, Any],
    target_sha: str,
    context: dict[str, Any],
    reuse_digest: str | None = None,
) -> dict[str, str]:
    marker = parse_marker(body)
    pr_number = pr.get("number")
    if not isinstance(pr_number, int) or pr_number <= 0:
        raise BindingError("PR number missing/invalid")
    head = str(((pr.get("head") or {}).get("sha") or "")).lower()
    target = target_sha.lower()
    if not SHA_RE.fullmatch(target) or head != target:
        raise BindingError("target SHA does not equal live PR HEAD")

    expected_context = str(context.get("context_digest") or "").lower()
    if not DIGEST_RE.fullmatch(expected_context):
        raise BindingError("current validation-context digest is invalid")
    expected_wp = str(context.get("wp") or "")
    expected_process = bool_text(context.get("process_only"))
    expected_non_foundational = bool_text(context.get("non_foundational"))

    if marker["target_sha"] != target:
        raise BindingError("marker target SHA mismatch")
    if marker["wp"] != expected_wp:
        raise BindingError("marker WP mismatch")
    if marker["process_only"] != expected_process:
        raise BindingError("marker process class mismatch")
    if marker["non_foundational"] != expected_non_foundational:
        raise BindingError("marker proof class mismatch")
    if marker["context_digest"] != expected_context:
        raise BindingError("marker validation-context digest mismatch")

    workflow = str(marker["validation_workflow"])
    if workflow == "Arkus Candidate Validation":
        if marker["reuse_context_digest"] is not None:
            raise BindingError("Candidate Validation marker must not carry Reuse Context Digest")
        binding_digest = expected_context
        resolved_reuse = "NONE"
    else:
        resolved_reuse = reuse_digest or current_reuse_digest(pr, target)
        if not DIGEST_RE.fullmatch(resolved_reuse):
            raise BindingError("resolved reuse-context digest is invalid")
        if marker["reuse_context_digest"] != resolved_reuse:
            raise BindingError("marker reuse-context digest mismatch")
        binding_digest = resolved_reuse

    expected_key = f"review-ready:{pr_number}:{target}:{binding_digest}"
    if marker["key"] != expected_key:
        raise BindingError(f"marker key mismatch: expected {expected_key}")

    return {
        "pr": str(pr_number),
        "target_sha": target,
        "wp": expected_wp,
        "process_only": expected_process,
        "non_foundational": expected_non_foundational,
        "context_digest": expected_context,
        "reuse_context_digest": resolved_reuse,
        "binding_digest": binding_digest,
        "validation_run_id": str(marker["validation_run_id"]),
        "validation_workflow": workflow,
        "review_ready_key": expected_key,
        "review_ready_closed_key": f"review-ready-closed:{pr_number}:{target}:{binding_digest}",
    }


def select_marker(
    *,
    comments: list[dict[str, Any]],
    pr: dict[str, Any],
    target_sha: str,
    context: dict[str, Any],
    before: str | None = None,
) -> tuple[dict[str, Any], dict[str, str]]:
    reuse_digest: str | None = None
    candidates: list[tuple[str, int, dict[str, Any], dict[str, str]]] = []
    for comment in comments:
        if str(((comment.get("user") or {}).get("login") or "")) != "github-actions[bot]":
            continue
        created = str(comment.get("created_at") or "")
        if before and created and created > before:
            continue
        body = str(comment.get("body") or "")
        if "State: REVIEW_READY" not in body or "ARKUS_AUTOMATION_V2" not in body:
            continue
        try:
            parsed = parse_marker(body)
            if parsed["validation_workflow"] == "Arkus Receipt Freeze Reuse" and reuse_digest is None:
                reuse_digest = current_reuse_digest(pr, target_sha)
            binding = validate_marker(
                body=body,
                pr=pr,
                target_sha=target_sha,
                context=context,
                reuse_digest=reuse_digest,
            )
        except BindingError:
            continue
        candidates.append((created, int(comment.get("id") or 0), comment, binding))
    if not candidates:
        raise BindingError("no current context-bound REVIEW_READY marker")
    _created, _id, comment, binding = max(candidates, key=lambda row: (row[0], row[1]))
    return comment, binding


def self_test() -> None:
    sha = "a" * 40
    context_digest = "c" * 64
    pr = {"number": 77, "head": {"sha": sha}, "draft": False, "body": "WP: WP-HK-00\n"}
    context = {
        "wp": "WP-HK-00",
        "process_only": False,
        "non_foundational": False,
        "context_digest": context_digest,
    }
    candidate_body = "\n".join(
        [
            "ARKUS_AUTOMATION_V2",
            "State: REVIEW_READY",
            f"Key: review-ready:77:{sha}:{context_digest}",
            f"Target SHA: {sha}",
            "Effective WP: WP-HK-00",
            "Process Only: false",
            "Non Foundational: false",
            f"Validation Context Digest: {context_digest}",
            "Validation Run ID: 123",
            "Validation Workflow: Arkus Candidate Validation",
        ]
    )
    got = validate_marker(body=candidate_body, pr=pr, target_sha=sha, context=context)
    assert got["binding_digest"] == context_digest

    reuse_digest = current_reuse_digest(pr, sha)
    reuse_body = candidate_body.replace(
        f"Key: review-ready:77:{sha}:{context_digest}",
        f"Key: review-ready:77:{sha}:{reuse_digest}",
    ).replace(
        "Validation Run ID: 123\nValidation Workflow: Arkus Candidate Validation",
        f"Reuse Context Digest: {reuse_digest}\nValidation Run ID: 456\nValidation Workflow: Arkus Receipt Freeze Reuse",
    )
    got_reuse = validate_marker(
        body=reuse_body,
        pr=pr,
        target_sha=sha,
        context=context,
        reuse_digest=reuse_digest,
    )
    assert got_reuse["binding_digest"] == reuse_digest

    for mutated, needle in (
        (reuse_body.replace(reuse_digest, "d" * 64, 1), "key mismatch"),
        (reuse_body.replace(f"Reuse Context Digest: {reuse_digest}", "Reuse Context Digest: " + "e" * 64), "reuse-context digest mismatch"),
        (candidate_body.replace("Arkus Candidate Validation", "Unknown Validation"), "unsupported validation workflow"),
    ):
        try:
            validate_marker(body=mutated, pr=pr, target_sha=sha, context=context, reuse_digest=reuse_digest)
        except BindingError as exc:
            if needle not in str(exc):
                raise AssertionError(f"expected {needle!r}, got {exc!r}") from exc
        else:
            raise AssertionError(f"mutation stayed GREEN: {needle}")
    print("REVIEW_READY_BINDING_SELF_TEST_GREEN")


def main() -> int:
    parser = argparse.ArgumentParser()
    sub = parser.add_subparsers(dest="command", required=True)
    sub.add_parser("self-test")
    select = sub.add_parser("select")
    select.add_argument("--comments-json", required=True)
    select.add_argument("--pr-json", required=True)
    select.add_argument("--context-record", required=True)
    select.add_argument("--target-sha", required=True)
    select.add_argument("--before")
    select.add_argument("--record", required=True)
    select.add_argument("--marker-record")
    args = parser.parse_args()
    try:
        if args.command == "self-test":
            self_test()
            return 0
        comments = json.loads(Path(args.comments_json).read_text(encoding="utf-8"))
        pr = json.loads(Path(args.pr_json).read_text(encoding="utf-8"))
        context = json.loads(Path(args.context_record).read_text(encoding="utf-8"))
        if not isinstance(comments, list) or not isinstance(pr, dict) or not isinstance(context, dict):
            raise BindingError("invalid input JSON shape")
        marker, binding = select_marker(
            comments=comments,
            pr=pr,
            target_sha=args.target_sha.lower(),
            context=context,
            before=args.before,
        )
        Path(args.record).write_text(json.dumps(binding, indent=2, sort_keys=True) + "\n", encoding="utf-8")
        if args.marker_record:
            Path(args.marker_record).write_text(json.dumps(marker, indent=2, sort_keys=True) + "\n", encoding="utf-8")
        print(binding["binding_digest"])
        return 0
    except (BindingError, OSError, json.JSONDecodeError) as exc:
        print(f"REVIEW_READY_BINDING_ERROR: {exc}", file=sys.stderr)
        return 2


if __name__ == "__main__":
    raise SystemExit(main())
