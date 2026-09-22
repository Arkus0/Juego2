#!/usr/bin/env python3
"""Regression guard for Arkus Main Safety pull-request coverage.

Batch A intentionally runs Main Safety for every pull request. Build/restore behavior can be
changed by repository inputs that are easy to omit from a path allow-list (for example
Directory.Build.props, Directory.Packages.props, NuGet.config, imported targets, or future
lock/config files). This guard rejects reintroducing paths/paths-ignore under pull_request.
"""

from __future__ import annotations

import argparse
from pathlib import Path
import re
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
        stripped = raw.strip()
        if stripped == "on:":
            on_index = index
            on_indent = _indent(raw)
            break

    if on_index is None or on_indent is None:
        return ["missing top-level on: block"]

    pr_index = None
    pr_indent = None
    pr_inline = ""
    for index in range(on_index + 1, len(lines)):
        raw = lines[index]
        stripped = raw.strip()
        if not stripped or stripped.startswith("#"):
            continue
        indent = _indent(raw)
        if indent <= on_indent:
            break
        match = re.match(r"^pull_request\s*:(.*)$", stripped)
        if match:
            pr_index = index
            pr_indent = indent
            pr_inline = match.group(1).strip()
            break

    if pr_index is None or pr_indent is None:
        return ["on: block does not declare pull_request"]

    if re.search(r"\bpaths(?:-ignore)?\s*:", pr_inline):
        errors.append("pull_request must not use paths or paths-ignore filtering")

    for index in range(pr_index + 1, len(lines)):
        raw = lines[index]
        stripped = raw.strip()
        if not stripped or stripped.startswith("#"):
            continue
        indent = _indent(raw)
        if indent <= pr_indent:
            break
        if re.match(r"^paths(?:-ignore)?\s*:", stripped):
            errors.append("pull_request must not use paths or paths-ignore filtering")
            break

    return errors


def self_test() -> None:
    cases = [
        (
            "unfiltered",
            "on:\n  push:\n    branches: [main]\n  pull_request:\n  workflow_dispatch:\n",
            False,
        ),
        (
            "paths",
            "on:\n  pull_request:\n    paths:\n      - 'src/**'\n",
            True,
        ),
        (
            "paths-ignore",
            "on:\n  pull_request:\n    paths-ignore:\n      - 'Docs/**'\n",
            True,
        ),
        (
            "inline-filter",
            "on:\n  pull_request: {paths: ['src/**']}\n",
            True,
        ),
        (
            "missing-pr",
            "on:\n  push:\n    branches: [main]\n",
            True,
        ),
    ]
    for name, text, should_fail in cases:
        failed = bool(validate(text))
        if failed != should_fail:
            raise AssertionError(f"self-test {name!r} expected fail={should_fail}, got {failed}")


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
        print(
            "error: Arkus Main Safety must run on every pull request so new build/restore inputs cannot bypass pre-merge validation.",
            file=sys.stderr,
        )
        return 1

    print("MAIN_SAFETY_PR_COVERAGE_GREEN")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
