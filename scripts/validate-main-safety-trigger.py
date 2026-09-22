#!/usr/bin/env python3
"""Regression guard for Arkus Main Safety pull-request path coverage.

Main Safety intentionally remains path-filtered so documentation-only/process-only changes do
not pay for a full .NET build. The filter must nevertheless cover current and future central
MSBuild/NuGet inputs: project/solution files, Directory.* props/targets, arbitrary imported
.props/.targets, NuGet.config and package lock files. This checker makes that allow-list an
explicit tested contract instead of an easy-to-forget workflow detail.
"""

from __future__ import annotations

import argparse
from pathlib import Path
import re
import sys

WORKFLOW = Path(".github/workflows/main-safety.yml")

REQUIRED_PATHS = {
    ".github/workflows/main-safety.yml",
    "global.json",
    "Juego2.sln",
    "**/*.sln",
    "**/*.csproj",
    "src/**",
    "tests/**",
    "scripts/**",
    "Directory.Build.props",
    "Directory.Packages.props",
    "Directory.*.props",
    "Directory.*.targets",
    "**/Directory.*.props",
    "**/Directory.*.targets",
    "**/*.props",
    "**/*.targets",
    "NuGet.config",
    "**/NuGet.config",
    "packages.lock.json",
    "**/packages.lock.json",
    "**/*.lock.json",
}


def _indent(line: str) -> int:
    return len(line) - len(line.lstrip(" "))


def _strip_scalar(value: str) -> str:
    value = value.strip()
    if len(value) >= 2 and value[0] == value[-1] and value[0] in {"'", '"'}:
        return value[1:-1]
    return value


def extract_pr_paths(text: str) -> tuple[set[str], list[str]]:
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
        return set(), ["missing top-level on: block"]

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
        if re.match(r"^pull_request\s*:", stripped):
            pr_index = index
            pr_indent = indent
            break
    if pr_index is None or pr_indent is None:
        return set(), ["on: block does not declare pull_request"]

    paths_index = None
    paths_indent = None
    for index in range(pr_index + 1, len(lines)):
        raw = lines[index]
        stripped = raw.strip()
        if not stripped or stripped.startswith("#"):
            continue
        indent = _indent(raw)
        if indent <= pr_indent:
            break
        if re.match(r"^paths-ignore\s*:", stripped):
            errors.append("pull_request must not use paths-ignore")
        if re.match(r"^paths\s*:\s*$", stripped):
            paths_index = index
            paths_indent = indent
            break

    if paths_index is None or paths_indent is None:
        errors.append("pull_request must declare the reviewed paths allow-list")
        return set(), errors

    paths: set[str] = set()
    for index in range(paths_index + 1, len(lines)):
        raw = lines[index]
        stripped = raw.strip()
        if not stripped or stripped.startswith("#"):
            continue
        indent = _indent(raw)
        if indent <= paths_indent:
            break
        if not stripped.startswith("-"):
            errors.append(f"unexpected entry under pull_request.paths: {stripped}")
            continue
        paths.add(_strip_scalar(stripped[1:].strip()))

    return paths, errors


def validate(text: str) -> list[str]:
    paths, errors = extract_pr_paths(text)
    missing = sorted(REQUIRED_PATHS - paths)
    if missing:
        errors.append("missing Main Safety PR paths: " + ", ".join(missing))
    return errors


def self_test() -> None:
    all_paths = "\n".join(f"      - '{path}'" for path in sorted(REQUIRED_PATHS))
    good = f"on:\n  push:\n    branches: [main]\n  pull_request:\n    paths:\n{all_paths}\n"
    assert validate(good) == []

    for removed in (
        "Directory.Build.props",
        "Directory.Packages.props",
        "**/Directory.*.targets",
        "**/*.props",
        "NuGet.config",
        "**/packages.lock.json",
    ):
        broken = good.replace(f"      - '{removed}'\n", "")
        if not validate(broken):
            raise AssertionError(f"self-test failed to reject missing {removed}")

    if not validate("on:\n  pull_request:\n    paths:\n      - 'src/**'\n"):
        raise AssertionError("self-test accepted incomplete build/restore coverage")


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
