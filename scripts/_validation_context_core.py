#!/usr/bin/env python3
"""Canonical validation-context identity for Automation V2.

GitHub Actions is orchestration only. This script owns the fail-closed mapping
from one concrete PR + exact SHA to every mutable PR-body fact that changes the
mechanical validation policy: effective WP, PROCESS_ONLY classification and the
accepted non-foundational proof class. It also owns the stable digest and the
matching rules used before durable evidence can be reused by REVIEW_READY or
PASS preflight.
"""
from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path
import re
import sys
from typing import Any

SCHEMA = "ARKUS_VALIDATION_CONTEXT_V1"
SHA_RE = re.compile(r"^[0-9a-f]{40}$", re.I)
DIGEST_RE = re.compile(r"^[0-9a-f]{64}$", re.I)
WP_RE = re.compile(r"^WP-[A-Z0-9]+(?:-[A-Z0-9]+)+$")
AUTHORITY_RE = re.compile(
    r"^(WP|Mode|WORKFLOW_MODE|Class):\s*`?([^`\r\n]+?)`?\s*$", re.I | re.M
)
NON_FOUNDATIONAL_RE = re.compile(r"NON-(?:PRODUCT-)?FOUNDATIONAL", re.I)
SUMMARY_FIELD_RE = re.compile(r"^([^:\r\n]+):\s*(.*?)\s*$", re.M)


def _authority_rows(body: str) -> list[tuple[str, str]]:
    return [(name.upper(), value.strip()) for name, value in AUTHORITY_RE.findall(body)]


def _single_authority(body: str, names: set[str], label: str) -> str | None:
    rows = [(name, value) for name, value in _authority_rows(body) if name in names]
    if len(rows) > 1:
        rendered = ", ".join(f"{name}={value!r}" for name, value in rows)
        raise ValueError(f"ambiguous {label} authority: expected at most one field, found {rendered}")
    return rows[0][1] if rows else None


def _context_payload(values: dict[str, str]) -> dict[str, str]:
    return {
        "schema": SCHEMA,
        "pr_number": str(values["pr_number"]),
        "head_sha": values["head_sha"].lower(),
        "wp": values["wp"],
        "process_only": values["process_only"],
        "non_foundational": values["non_foundational"],
    }


def context_digest(values: dict[str, str]) -> str:
    canonical = json.dumps(
        _context_payload(values), sort_keys=True, separators=(",", ":"), ensure_ascii=True
    ).encode("utf-8")
    return hashlib.sha256(canonical).hexdigest()


def resolve_context(pr: dict[str, Any], target_sha: str) -> dict[str, str]:
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
    wp = _single_authority(body, {"WP"}, "WP")
    mode = _single_authority(body, {"MODE", "WORKFLOW_MODE"}, "process-mode")
    class_value = _single_authority(body, {"CLASS"}, "Class")

    if wp is not None:
        wp = wp.strip()
        if not WP_RE.fullmatch(wp):
            raise ValueError(f"WP field is invalid: {wp!r}")

    process_only = bool(mode and mode.strip().upper() == "PROCESS_ONLY")
    non_foundational = bool(
        (mode and NON_FOUNDATIONAL_RE.search(mode))
        or (class_value and NON_FOUNDATIONAL_RE.search(class_value))
    )
    if wp is None and not process_only:
        raise ValueError("non-PROCESS_ONLY validation requires an explicit WP field")

    values = {
        "pr_number": str(number),
        "head_sha": head,
        "wp": wp or "NONE",
        "process_only": "true" if process_only else "false",
        "non_foundational": "true" if non_foundational else "false",
    }
    values["context_digest"] = context_digest(values)
    return values


def write_record(path: Path, values: dict[str, str]) -> None:
    record = _context_payload(values)
    record["context_digest"] = context_digest(values)
    path.write_text(json.dumps(record, indent=2, sort_keys=True) + "\n", encoding="utf-8")


def load_record(path: Path) -> dict[str, str]:
    raw = json.loads(path.read_text(encoding="utf-8"))
    if not isinstance(raw, dict):
        raise ValueError("validation context record must be a JSON object")
    required = {
        "schema",
        "pr_number",
        "head_sha",
        "wp",
        "process_only",
        "non_foundational",
        "context_digest",
    }
    if set(raw) != required:
        raise ValueError(
            f"validation context record keys differ from canonical schema: {sorted(raw)}"
        )
    values = {key: str(raw[key]) for key in required}
    if values["schema"] != SCHEMA:
        raise ValueError(f"unsupported validation context schema: {values['schema']!r}")
    if not SHA_RE.fullmatch(values["head_sha"]):
        raise ValueError("validation context record has invalid head_sha")
    if values["wp"] != "NONE" and not WP_RE.fullmatch(values["wp"]):
        raise ValueError("validation context record has invalid wp")
    for key in ("process_only", "non_foundational"):
        if values[key] not in {"true", "false"}:
            raise ValueError(f"validation context record has invalid {key}")
    if not DIGEST_RE.fullmatch(values["context_digest"]):
        raise ValueError("validation context record has invalid context_digest")
    expected = context_digest(values)
    if values["context_digest"].lower() != expected:
        raise ValueError(
            f"validation context record digest mismatch: {values['context_digest']} != {expected}"
        )
    values["context_digest"] = values["context_digest"].lower()
    values["head_sha"] = values["head_sha"].lower()
    return values


def require_record_match(record: dict[str, str], current: dict[str, str]) -> None:
    for key in (
        "pr_number",
        "head_sha",
        "wp",
        "process_only",
        "non_foundational",
        "context_digest",
    ):
        if record[key] != current[key]:
            raise ValueError(
                f"validation context mismatch for {key}: recorded {record[key]!r} != current {current[key]!r}"
            )


def make_check_summary(record: dict[str, str], run_id: str, workflow: str) -> str:
    if not str(run_id).isdigit() or int(run_id) <= 0:
        raise ValueError(f"validation run id is invalid: {run_id!r}")
    workflow = workflow.strip()
    if not workflow or "\n" in workflow or "\r" in workflow:
        raise ValueError("validation workflow name is invalid")
    return "\n".join(
        [
            SCHEMA,
            f"PR: {record['pr_number']}",
            f"Candidate SHA: {record['head_sha']}",
            f"Effective WP: {record['wp']}",
            f"Process Only: {record['process_only']}",
            f"Non Foundational: {record['non_foundational']}",
            f"Context Digest: {record['context_digest']}",
            f"Validation Run ID: {run_id}",
            f"Validation Workflow: {workflow}",
        ]
    )


def parse_check_summary(summary: str) -> dict[str, str]:
    lines = summary.splitlines()
    if not lines or lines[0].strip() != SCHEMA:
        raise ValueError("validation-context check summary lacks canonical schema marker")
    fields: dict[str, list[str]] = {}
    for key, value in SUMMARY_FIELD_RE.findall("\n".join(lines[1:])):
        fields.setdefault(key.strip(), []).append(value.strip())
    required = {
        "PR",
        "Candidate SHA",
        "Effective WP",
        "Process Only",
        "Non Foundational",
        "Context Digest",
        "Validation Run ID",
        "Validation Workflow",
    }
    if set(fields) != required:
        raise ValueError(
            f"validation-context check summary fields differ from canonical set: {sorted(fields)}"
        )
    duplicates = sorted(key for key, values in fields.items() if len(values) != 1)
    if duplicates:
        raise ValueError(f"validation-context check summary has duplicate fields: {duplicates}")
    out = {key: values[0] for key, values in fields.items()}
    if not SHA_RE.fullmatch(out["Candidate SHA"]):
        raise ValueError("validation-context check summary has invalid Candidate SHA")
    if out["Process Only"] not in {"true", "false"}:
        raise ValueError("validation-context check summary has invalid Process Only")
    if out["Non Foundational"] not in {"true", "false"}:
        raise ValueError("validation-context check summary has invalid Non Foundational")
    if not DIGEST_RE.fullmatch(out["Context Digest"]):
        raise ValueError("validation-context check summary has invalid Context Digest")
    if not out["Validation Run ID"].isdigit() or int(out["Validation Run ID"]) <= 0:
        raise ValueError("validation-context check summary has invalid Validation Run ID")
    return out


def require_check_summary_match(
    summary: str,
    current: dict[str, str],
    *,
    expected_run_id: str | None = None,
    expected_workflow: str | None = None,
) -> dict[str, str]:
    fields = parse_check_summary(summary)
    expected = {
        "PR": current["pr_number"],
        "Candidate SHA": current["head_sha"],
        "Effective WP": current["wp"],
        "Process Only": current["process_only"],
        "Non Foundational": current["non_foundational"],
        "Context Digest": current["context_digest"],
    }
    for key, value in expected.items():
        if fields[key] != value:
            raise ValueError(
                f"validation-context check mismatch for {key}: {fields[key]!r} != {value!r}"
            )
    if expected_run_id is not None and fields["Validation Run ID"] != str(expected_run_id):
        raise ValueError(
            f"validation-context check run {fields['Validation Run ID']} != expected {expected_run_id}"
        )
    if expected_workflow is not None and fields["Validation Workflow"] != expected_workflow:
        raise ValueError(
            f"validation-context check workflow {fields['Validation Workflow']!r} != expected {expected_workflow!r}"
        )
    return fields


def receipt_identity(receipt: str) -> tuple[str, str]:
    wp_matches = re.findall(r"^WP:\s*(\S.*?)\s*$", receipt, re.M)
    sha_matches = re.findall(r"^Candidate SHA:\s*([0-9a-fA-F]{40})\s*$", receipt, re.M)
    if len(wp_matches) != 1:
        raise ValueError(f"receipt must contain exactly one WP field, found {len(wp_matches)}")
    if len(sha_matches) != 1:
        raise ValueError(
            f"receipt must contain exactly one exact Candidate SHA field, found {len(sha_matches)}"
        )
    wp = wp_matches[0].strip().strip("`")
    if not WP_RE.fullmatch(wp):
        raise ValueError(f"receipt WP is invalid: {wp!r}")
    return wp, sha_matches[0].lower()


def require_receipt_match(receipt: str, requested_wp: str, target_sha: str) -> None:
    if not WP_RE.fullmatch(requested_wp):
        raise ValueError(f"requested WP is invalid: {requested_wp!r}")
    target = target_sha.lower()
    if not SHA_RE.fullmatch(target):
        raise ValueError(f"target SHA is invalid: {target_sha!r}")
    receipt_wp, receipt_sha = receipt_identity(receipt)
    if receipt_wp != requested_wp:
        raise ValueError(
            f"receipt WP {receipt_wp} does not match requested WP {requested_wp}"
        )
    if receipt_sha != target:
        raise ValueError(
            f"receipt candidate SHA {receipt_sha} does not match requested SHA {target}"
        )


def _must_fail(fn, needle: str, label: str) -> None:
    try:
        fn()
    except ValueError as exc:
        if needle not in str(exc):
            raise AssertionError(f"{label}: unexpected error: {exc}") from exc
    else:
        raise AssertionError(f"{label}: expected failure")


def run_self_test() -> None:
    sha = "a" * 40
    base = {"number": 77, "head": {"sha": sha}}

    product = dict(base, body="WP: WP-H1-02\nMode: WORKPACK\n")
    product_ctx = resolve_context(product, sha)
    assert product_ctx["wp"] == "WP-H1-02"
    assert product_ctx["process_only"] == "false"
    assert product_ctx["non_foundational"] == "false"
    assert DIGEST_RE.fullmatch(product_ctx["context_digest"])

    city = dict(
        base,
        body="WP: WP-CITY-02\nMode: REMOTE / PRODUCT PREPRODUCTION / NON-FOUNDATIONAL\n",
    )
    city_ctx = resolve_context(city, sha)
    assert city_ctx["process_only"] == "false"
    assert city_ctx["non_foundational"] == "true"
    assert city_ctx["context_digest"] != product_ctx["context_digest"]

    process = dict(
        base,
        body=(
            "WORKFLOW_MODE: PROCESS_ONLY\n"
            "Class: PROCESS_ONLY / NON-PRODUCT-FOUNDATIONAL\n"
            "WP: WP-CTX-03\n"
        ),
    )
    process_ctx = resolve_context(process, sha)
    assert process_ctx["wp"] == "WP-CTX-03"
    assert process_ctx["process_only"] == "true"
    assert process_ctx["non_foundational"] == "true"
    assert process_ctx["context_digest"] != product_ctx["context_digest"]

    maintenance = dict(base, body="WORKFLOW_MODE: PROCESS_ONLY\n")
    got = resolve_context(maintenance, sha)
    assert got["wp"] == "NONE"
    assert got["process_only"] == "true"
    assert got["non_foundational"] == "false"

    _must_fail(
        lambda: resolve_context(product, "b" * 40),
        "!= PR HEAD",
        "mismatched manual candidate SHA",
    )
    _must_fail(
        lambda: resolve_context(dict(base, body="ordinary PR\n"), sha),
        "explicit WP",
        "product PR without WP",
    )
    _must_fail(
        lambda: resolve_context(dict(base, body="WP: WP-H1-01\nWP: WP-H1-01\n"), sha),
        "ambiguous WP authority",
        "duplicate identical WP",
    )
    _must_fail(
        lambda: resolve_context(dict(base, body="WP: WP-H1-01\nWP: WP-H1-02\n"), sha),
        "ambiguous WP authority",
        "conflicting WP",
    )
    _must_fail(
        lambda: resolve_context(
            dict(base, body="WP: WP-H1-02\nWORKFLOW_MODE: PROCESS_ONLY\nMode: PROCESS_ONLY\n"), sha
        ),
        "ambiguous process-mode authority",
        "duplicate identical mode",
    )
    _must_fail(
        lambda: resolve_context(
            dict(base, body="WP: WP-H1-02\nWORKFLOW_MODE: PROCESS_ONLY\nMode: PRODUCT\n"), sha
        ),
        "ambiguous process-mode authority",
        "conflicting mode",
    )
    _must_fail(
        lambda: resolve_context(
            dict(base, body="WP: WP-H1-02\nClass: FOUNDATIONAL\nClass: FOUNDATIONAL\n"), sha
        ),
        "ambiguous Class authority",
        "duplicate class",
    )

    record = _context_payload(process_ctx)
    record["context_digest"] = process_ctx["context_digest"]
    require_record_match(record, process_ctx)
    _must_fail(
        lambda: require_record_match(record, product_ctx),
        "validation context mismatch",
        "same-SHA process-to-product mutation",
    )

    product_record = _context_payload(product_ctx)
    product_record["context_digest"] = product_ctx["context_digest"]
    _must_fail(
        lambda: require_record_match(product_record, city_ctx),
        "validation context mismatch",
        "same-SHA foundational-to-non-foundational mutation",
    )

    summary = make_check_summary(process_ctx, "123", "Arkus Candidate Validation")
    require_check_summary_match(
        summary,
        process_ctx,
        expected_run_id="123",
        expected_workflow="Arkus Candidate Validation",
    )
    _must_fail(
        lambda: require_check_summary_match(summary, product_ctx),
        "validation-context check mismatch",
        "stale same-SHA context check",
    )

    receipt = (
        "EXECUTION_RECEIPT_V1\n"
        "WP: WP-HK-00\n"
        f"Candidate SHA: {sha}\n"
        "Result: GREEN\n"
    )
    require_receipt_match(receipt, "WP-HK-00", sha)
    _must_fail(
        lambda: require_receipt_match(receipt, "WP-H1-01", sha),
        "does not match requested WP",
        "HK-00 receipt for H1",
    )
    _must_fail(
        lambda: require_receipt_match(receipt, "WP-HK-00", "b" * 40),
        "does not match requested SHA",
        "receipt SHA mismatch",
    )

    print("validation-context self-test: PASS")


def write_outputs(path: Path, values: dict[str, str]) -> None:
    with path.open("a", encoding="utf-8") as handle:
        for key in (
            "pr_number",
            "head_sha",
            "wp",
            "process_only",
            "non_foundational",
            "context_digest",
        ):
            handle.write(f"{key}={values[key]}\n")


def main() -> int:
    parser = argparse.ArgumentParser()
    sub = parser.add_subparsers(dest="command", required=True)

    sub.add_parser("self-test")

    resolve = sub.add_parser("resolve")
    resolve.add_argument("--pr-json", type=Path, required=True)
    resolve.add_argument("--target-sha", required=True)
    resolve.add_argument("--github-output", type=Path)
    resolve.add_argument("--record", type=Path)

    compare = sub.add_parser("record-match")
    compare.add_argument("--pr-json", type=Path, required=True)
    compare.add_argument("--target-sha", required=True)
    compare.add_argument("--record", type=Path, required=True)

    summary = sub.add_parser("check-summary")
    summary.add_argument("--record", type=Path, required=True)
    summary.add_argument("--run-id", required=True)
    summary.add_argument("--workflow", required=True)

    check = sub.add_parser("check-match")
    check.add_argument("--pr-json", type=Path, required=True)
    check.add_argument("--target-sha", required=True)
    check.add_argument("--summary", type=Path, required=True)
    check.add_argument("--run-id")
    check.add_argument("--workflow")

    receipt = sub.add_parser("receipt-match")
    receipt.add_argument("--receipt", type=Path, required=True)
    receipt.add_argument("--requested-wp", required=True)
    receipt.add_argument("--target-sha", required=True)

    args = parser.parse_args()
    try:
        if args.command == "self-test":
            run_self_test()
            return 0
        if args.command == "resolve":
            pr = json.loads(args.pr_json.read_text(encoding="utf-8"))
            values = resolve_context(pr, args.target_sha)
            if args.github_output:
                write_outputs(args.github_output, values)
            if args.record:
                write_record(args.record, values)
            print(json.dumps(values, sort_keys=True))
            return 0
        if args.command == "record-match":
            pr = json.loads(args.pr_json.read_text(encoding="utf-8"))
            current = resolve_context(pr, args.target_sha)
            require_record_match(load_record(args.record), current)
            print(f"validation context record match: {current['context_digest']}")
            return 0
        if args.command == "check-summary":
            print(make_check_summary(load_record(args.record), args.run_id, args.workflow))
            return 0
        if args.command == "check-match":
            pr = json.loads(args.pr_json.read_text(encoding="utf-8"))
            current = resolve_context(pr, args.target_sha)
            fields = require_check_summary_match(
                args.summary.read_text(encoding="utf-8"),
                current,
                expected_run_id=args.run_id,
                expected_workflow=args.workflow,
            )
            print(json.dumps(fields, sort_keys=True))
            return 0
        if args.command == "receipt-match":
            require_receipt_match(
                args.receipt.read_text(encoding="utf-8"),
                args.requested_wp,
                args.target_sha,
            )
            print(
                f"receipt identity match: WP={args.requested_wp} SHA={args.target_sha.lower()}"
            )
            return 0
    except (OSError, json.JSONDecodeError, ValueError) as exc:
        print(f"validation-context error: {exc}", file=sys.stderr)
        return 2
    return 2


if __name__ == "__main__":
    raise SystemExit(main())
