#!/usr/bin/env python3
"""Small regression guard for Arkus Main Safety pull-request coverage.

Main Safety is intended to run on every pull request. This checker only protects the workflow
trigger shape against accidental reintroduction of pull-request filters. It does not model or
prove general GitHub Actions execution semantics.
"""

from __future__ import annotations

import argparse
from pathlib import Path
import sys

WORKFLOW = Path(".github/workflows/main-safety.yml")


def _indent(line: str) -> int:
    return len(line) - len(line.lstrip(" "))


def validate(text: str) -> list[str]:
    lines = text.splitlines()
    errors: list[str] = []

    on_index = None
    on_indent = None
    for index, raw in enumerate(lines):
        if raw.strip() == "on:":
            on_index = index
            on_indent = _indent(raw)
            break
    if on_index is None or on_indent is None:
        return ["missing top-level on: block"]

    pr_index = None
    pr_indent = None
    for index in range(on_index + 1, len(lines)):
        raw = lines[index]
        stripped = raw.strip()
        if not stripped or stripped.startswith("#"):
            continue
        indent = _indent(raw)
        if indent <= on_indent:
            break
        if stripped.startswith("pull_request:"):
            pr_index = index
            pr_indent = indent
            if stripped != "pull_request:":
                errors.append("pull_request must use the unfiltered default trigger")
            break

    if pr_index is None or pr_indent is None:
        return errors + ["on: block does not declare pull_request"]

    for index in range(pr_index + 1, len(lines)):
        raw = lines[index]
        stripped = raw.strip()
        if not stripped or stripped.startswith("#"):
            continue
        indent = _indent(raw)
        if indent <= pr_indent:
            break
        errors.append(
            "pull_request must remain unfiltered; nested trigger configuration found: " + stripped
        )

    return errors


def self_test() -> None:
    good = """on:
  push:
    branches: [main]
  pull_request:
  workflow_dispatch:
"""
    assert validate(good) == []

    filtered_cases = (
        "paths:\n      - 'src/**'",
        "paths-ignore:\n      - 'Docs/**'",
        "branches: [main]",
        "types: [opened]",
    )
    for nested in filtered_cases:
        broken = good.replace("  pull_request:\n", f"  pull_request:\n    {nested}\n")
        if not validate(broken):
            raise AssertionError(f"self-test accepted filtered pull_request trigger: {nested}")

    for inline in ("{}", "[]", "null", "{branches: [main]}"):
        broken = good.replace("  pull_request:\n", f"  pull_request: {inline}\n")
        if not validate(broken):
            raise AssertionError(f"self-test accepted inline pull_request configuration: {inline}")

    missing = good.replace("  pull_request:\n", "")
    if not validate(missing):
        raise AssertionError("self-test accepted workflow without pull_request trigger")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--self-test", action="store_true")
    args = parser.parse_args()

    if args.self_test:
        self_test()
        print("MAIN_SAFETY_TRIGGER_SELF_TEST_GREEN")
        return 0

    if not WORKFLOW.is_file():
        print(f"error: missing {WORKFLOW}", file=sys.stderr)
        return 2

    errors = validate(WORKFLOW.read_text(encoding="utf-8"))
    if errors:
        for error in errors:
            print(f"error: {error}", file=sys.stderr)
        return 1

    print("MAIN_SAFETY_PR_COVERAGE_GREEN")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
