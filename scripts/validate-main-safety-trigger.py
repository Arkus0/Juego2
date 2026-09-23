#!/usr/bin/env python3
"""Regression guard for Arkus Main Safety pull-request execution coverage.

Main Safety must run on every pull request. Trigger filters are insufficient because arbitrary
repository files can become build inputs, and an unconditional trigger is still not enough if
the safety job itself is gated. This checker therefore protects both the unfiltered PR trigger
and the unconditional, fail-closed `jobs.build-test` execution path.
"""

from __future__ import annotations

import argparse
from pathlib import Path
import re
import sys

WORKFLOW = Path(".github/workflows/main-safety.yml")


def _indent(line: str) -> int:
    return len(line) - len(line.lstrip(" "))


def _find_direct_child(
    lines: list[str], parent_index: int, parent_indent: int, key: str
) -> tuple[int | None, int | None]:
    """Find an exact YAML mapping key directly below a parent indentation level."""
    for index in range(parent_index + 1, len(lines)):
        raw = lines[index]
        stripped = raw.strip()
        if not stripped or stripped.startswith("#"):
            continue
        indent = _indent(raw)
        if indent <= parent_indent:
            break
        if indent == parent_indent + 2 and stripped == f"{key}:":
            return index, indent
    return None, None


def _validate_unfiltered_pull_request(lines: list[str]) -> list[str]:
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


def _validate_unconditional_build_test(lines: list[str]) -> list[str]:
    """Require the canonical Main Safety job to be an unconditional fail-closed PR gate."""
    errors: list[str] = []

    jobs_index = None
    jobs_indent = None
    for index, raw in enumerate(lines):
        if raw.strip() == "jobs:":
            jobs_index = index
            jobs_indent = _indent(raw)
            break
    if jobs_index is None or jobs_indent is None:
        return ["missing top-level jobs: block"]

    job_index, job_indent = _find_direct_child(lines, jobs_index, jobs_indent, "build-test")
    if job_index is None or job_indent is None:
        return ["jobs block does not declare build-test safety job"]

    property_indent = job_indent + 2
    for index in range(job_index + 1, len(lines)):
        raw = lines[index]
        stripped = raw.strip()
        if not stripped or stripped.startswith("#"):
            continue
        indent = _indent(raw)
        if indent <= job_indent:
            break
        if indent != property_indent:
            continue

        if stripped.startswith("if:"):
            errors.append("jobs.build-test must not declare if:; Main Safety must run on every PR")
        elif stripped.startswith("needs:"):
            errors.append("jobs.build-test must not depend on another job that can suppress PR execution")
        elif re.match(r"^continue-on-error\s*:\s*true\s*$", stripped, re.IGNORECASE):
            errors.append("jobs.build-test must fail closed; continue-on-error: true is forbidden")

    return errors


def validate(text: str) -> list[str]:
    lines = text.splitlines()
    return _validate_unfiltered_pull_request(lines) + _validate_unconditional_build_test(lines)


def self_test() -> None:
    good = """on:
  push:
    branches: [main]
  # All PRs must execute Main Safety.
  pull_request:
  workflow_dispatch:

jobs:
  build-test:
    name: Safety build and test
    runs-on: ubuntu-24.04
    steps:
      - name: Restore
        run: dotnet restore Juego2.sln --locked-mode
"""
    assert validate(good) == [], validate(good)

    filtered_cases = (
        "paths:\n      - 'src/**'",
        "paths-ignore:\n      - 'Docs/**'",
        "branches: [main]",
        "branches-ignore: [legacy]",
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

    missing_trigger = good.replace("  pull_request:\n", "")
    if not validate(missing_trigger):
        raise AssertionError("self-test accepted workflow without pull_request trigger")

    missing_job = good.replace("  build-test:\n", "  other-job:\n")
    if not validate(missing_job):
        raise AssertionError("self-test accepted workflow without canonical build-test job")

    job_guards = (
        "    if: github.event_name == 'push'\n",
        "    if: github.event_name != 'pull_request'\n",
        "    needs: gate\n",
        "    continue-on-error: true\n",
    )
    for guard in job_guards:
        broken = good.replace(
            "    name: Safety build and test\n",
            guard + "    name: Safety build and test\n",
        )
        if not validate(broken):
            raise AssertionError(f"self-test accepted suppressing/non-blocking build-test guard: {guard.strip()}")


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
