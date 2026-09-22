#!/usr/bin/env python3
"""Resolve exact validation context and prove receipt compatibility.

GitHub Actions is orchestration only. This script owns the fail-closed mapping
from one concrete PR + exact SHA to the effective workpack/process mode, and
owns the WP/SHA identity check required before any durable receipt is reused.
"""
from __future__ import annotations

import argparse
import json
from pathlib import Path
import re
import sys

SHA_RE = re.compile(r"^[0-9a-f]{40}$", re.I)
WP_RE = re.compile(r"^WP-[A-Z0-9]+(?:-[A-Z0-9]+)+$")
PROCESS_ONLY_RE = re.compile(
    r"^(?:Mode|WORKFLOW_MODE):\s*`?PROCESS_ONLY`?\s*$", re.I | re.M
)


def field(body: str, name: str) -> str | None:
    match = re.search(
        rf"^{re.escape(name)}:\s*`?([^`\r\n]+?)`?\s*$", body, re.I | re.M
    )
    return match.group(1).strip() if match else None


def resolve_context(pr: dict, target_sha: str) -> dict[str, str]:
    target = target_sha.strip().lower()
    if not SHA_RE.fullmatch(target):
        raise ValueError(f"candidate SHA is not exact: {target_sha!r}")

    head = str(((pr.get("head") or {}).get("sha") or "")).lower()
    if not SHA_RE.fullmatch(head):
        raise ValueError("PR head SHA is missing or invalid")
    if head != target:
        raise ValueError(f"candidate SHA {target} != PR HEAD {head}")

    body = str(pr.get("body") or "")
    process_only = bool(PROCESS_ONLY_RE.search(body))
    wp = field(body, "WP")
    if wp is not None:
        wp = wp.strip()
        if not WP_RE.fullmatch(wp):
            raise ValueError(f"WP field is invalid: {wp!r}")
    elif not process_only:
        raise ValueError("non-PROCESS_ONLY validation requires an explicit WP field")

    number = pr.get("number")
    if number is None:
        raise ValueError("PR number is missing")

    return {
        "pr_number": str(number),
        "head_sha": head,
        "wp": wp or "NONE",
        "process_only": "true" if process_only else "false",
    }


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


def run_self_test() -> None:
    sha = "a" * 40
    base = {"number": 77, "head": {"sha": sha}}

    product = dict(base, body="WP: WP-H1-02\n")
    got = resolve_context(product, sha)
    assert got["wp"] == "WP-H1-02" and got["process_only"] == "false"

    process = dict(base, body="WORKFLOW_MODE: PROCESS_ONLY\nWP: WP-CTX-03\n")
    got = resolve_context(process, sha)
    assert got["wp"] == "WP-CTX-03" and got["process_only"] == "true"

    maintenance = dict(base, body="WORKFLOW_MODE: PROCESS_ONLY\n")
    got = resolve_context(maintenance, sha)
    assert got["wp"] == "NONE" and got["process_only"] == "true"

    try:
        resolve_context(product, "b" * 40)
    except ValueError as exc:
        assert "!= PR HEAD" in str(exc)
    else:
        raise AssertionError("mismatched manual candidate SHA must fail")

    try:
        resolve_context(dict(base, body="ordinary PR\n"), sha)
    except ValueError as exc:
        assert "explicit WP" in str(exc)
    else:
        raise AssertionError("non-PROCESS_ONLY PR without WP must fail")

    receipt = (
        "EXECUTION_RECEIPT_V1\n"
        "WP: WP-HK-00\n"
        f"Candidate SHA: {sha}\n"
        "Result: GREEN\n"
    )
    require_receipt_match(receipt, "WP-HK-00", sha)

    try:
        require_receipt_match(receipt, "WP-H1-01", sha)
    except ValueError as exc:
        assert "does not match requested WP" in str(exc)
    else:
        raise AssertionError("HK-00 receipt must not validate an H1 request")

    try:
        require_receipt_match(receipt, "WP-HK-00", "b" * 40)
    except ValueError as exc:
        assert "does not match requested SHA" in str(exc)
    else:
        raise AssertionError("receipt SHA mismatch must fail")

    print("validation-context self-test: PASS")


def write_outputs(path: Path, values: dict[str, str]) -> None:
    with path.open("a", encoding="utf-8") as handle:
        for key in ("pr_number", "head_sha", "wp", "process_only"):
            handle.write(f"{key}={values[key]}\n")


def main() -> int:
    parser = argparse.ArgumentParser()
    sub = parser.add_subparsers(dest="command", required=True)

    sub.add_parser("self-test")

    resolve = sub.add_parser("resolve")
    resolve.add_argument("--pr-json", type=Path, required=True)
    resolve.add_argument("--target-sha", required=True)
    resolve.add_argument("--github-output", type=Path)

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
            print(json.dumps(values, sort_keys=True))
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
