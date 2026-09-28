#!/usr/bin/env python3
"""Fail-closed test tier runner for Arkus.Harness.Tests.

The manifest classifies discovered xUnit test classes into non-overlapping buckets.
A newly discovered test that does not map to exactly one declared class fails before
any filtered gate can report GREEN.
"""

from __future__ import annotations

import json
import subprocess
import sys
from collections import Counter
from pathlib import Path
from typing import Iterable

ROOT = Path(__file__).resolve().parents[1]
MANIFEST_PATH = ROOT / "tests" / "test-suite-classification.json"
EXPECTED_BUCKETS = {
    "MAIN_SAFETY_CORE",
    "FULL_HARNESS_DEEP",
    "TRACK_CTX_DW",
    "TRACK_H1",
}


class ClassificationError(RuntimeError):
    pass


def load_manifest() -> dict:
    data = json.loads(MANIFEST_PATH.read_text(encoding="utf-8"))
    if data.get("schema") != "arkus.test-suite-classification@1":
        raise ClassificationError("unexpected test-suite classification schema")
    project = data.get("project")
    namespace = data.get("namespace")
    buckets = data.get("buckets")
    if not isinstance(project, str) or not project.endswith(".csproj"):
        raise ClassificationError("manifest project must be a .csproj path")
    if not isinstance(namespace, str) or not namespace:
        raise ClassificationError("manifest namespace must be non-empty")
    if not isinstance(buckets, dict) or set(buckets) != EXPECTED_BUCKETS:
        raise ClassificationError(
            "manifest buckets must be exactly: " + ", ".join(sorted(EXPECTED_BUCKETS))
        )

    owner: dict[str, str] = {}
    for bucket, classes in buckets.items():
        if not isinstance(classes, list) or not classes:
            raise ClassificationError(f"bucket {bucket} must contain at least one class")
        for class_name in classes:
            if not isinstance(class_name, str) or not class_name:
                raise ClassificationError(f"bucket {bucket} contains an invalid class name")
            previous = owner.setdefault(class_name, bucket)
            if previous != bucket:
                raise ClassificationError(
                    f"test class {class_name} belongs to both {previous} and {bucket}"
                )
        if len(classes) != len(set(classes)):
            raise ClassificationError(f"bucket {bucket} contains duplicate class names")
    return data


def discover_tests(manifest: dict) -> list[str]:
    project = ROOT / manifest["project"]
    command = [
        "dotnet",
        "test",
        str(project),
        "--no-build",
        "--no-restore",
        "-c",
        "Release",
        "--list-tests",
        "--logger",
        "console;verbosity=minimal",
    ]
    result = subprocess.run(command, cwd=ROOT, text=True, capture_output=True)
    if result.returncode != 0:
        sys.stdout.write(result.stdout)
        sys.stderr.write(result.stderr)
        raise ClassificationError(
            f"dotnet test --list-tests failed with exit {result.returncode}"
        )

    prefix = manifest["namespace"] + "."
    tests = sorted(
        {
            line.strip()
            for line in result.stdout.splitlines()
            if line.strip().startswith(prefix)
        }
    )
    if not tests:
        sys.stdout.write(result.stdout)
        raise ClassificationError("test discovery returned no Arkus.Harness.Tests tests")
    return tests


def class_prefix(manifest: dict, class_name: str) -> str:
    return f"{manifest['namespace']}.{class_name}."


def classify_tests(manifest: dict, tests: Iterable[str]) -> tuple[dict[str, list[str]], set[str]]:
    buckets: dict[str, list[str]] = {name: [] for name in manifest["buckets"]}
    declared = [
        (bucket, class_name, class_prefix(manifest, class_name))
        for bucket, classes in manifest["buckets"].items()
        for class_name in classes
    ]
    seen_classes: set[str] = set()
    errors: list[str] = []

    for test in tests:
        matches = [
            (bucket, class_name)
            for bucket, class_name, prefix in declared
            if test.startswith(prefix)
        ]
        if len(matches) != 1:
            owners = ", ".join(f"{bucket}:{class_name}" for bucket, class_name in matches)
            errors.append(
                f"{test} -> expected exactly one classification, found {len(matches)}"
                + (f" ({owners})" if owners else "")
            )
            continue
        bucket, class_name = matches[0]
        buckets[bucket].append(test)
        seen_classes.add(class_name)

    declared_classes = {class_name for _, class_name, _ in declared}
    stale = sorted(declared_classes - seen_classes)
    if stale:
        errors.append("declared classes with no discovered tests: " + ", ".join(stale))
    if errors:
        raise ClassificationError("\n".join(errors))
    return buckets, seen_classes


def print_summary(buckets: dict[str, list[str]]) -> None:
    total = sum(len(tests) for tests in buckets.values())
    print(f"ARKUS_TEST_CLASSIFICATION total={total}")
    for bucket in sorted(buckets):
        print(f"ARKUS_TEST_BUCKET {bucket} tests={len(buckets[bucket])}")


def validate_live(manifest: dict) -> dict[str, list[str]]:
    tests = discover_tests(manifest)
    buckets, _ = classify_tests(manifest, tests)
    print_summary(buckets)
    return buckets


def run_bucket(manifest: dict, bucket: str) -> int:
    buckets = validate_live(manifest)
    project = str(ROOT / manifest["project"])
    command = [
        "dotnet",
        "test",
        project,
        "--no-build",
        "--no-restore",
        "-c",
        "Release",
        "--logger",
        "console;verbosity=minimal",
    ]

    if bucket != "ALL":
        if bucket not in manifest["buckets"]:
            raise ClassificationError(f"unknown test bucket: {bucket}")
        if not buckets[bucket]:
            raise ClassificationError(f"bucket {bucket} discovered zero tests")
        terms = [
            "FullyQualifiedName~" + class_prefix(manifest, class_name)
            for class_name in manifest["buckets"][bucket]
        ]
        command.extend(["--filter", "|".join(terms)])

    print("ARKUS_TEST_RUN bucket=" + bucket)
    return subprocess.run(command, cwd=ROOT).returncode


def self_test() -> None:
    manifest = load_manifest()
    namespace = manifest["namespace"]
    synthetic = [
        f"{namespace}.KernelCompositionTests.ComposesKernel",
        f"{namespace}.Hk01SelfAttackTests.RejectsAttack",
        f"{namespace}.Dw00DesignWorldProjectionTests.ProjectsWorld",
        f"{namespace}.H1CatalogueTests.BuildsCatalogue",
    ]
    buckets, _ = classify_tests_for_subset(manifest, synthetic)
    expected = {
        "MAIN_SAFETY_CORE": 1,
        "FULL_HARNESS_DEEP": 1,
        "TRACK_CTX_DW": 1,
        "TRACK_H1": 1,
    }
    actual = {bucket: len(tests) for bucket, tests in buckets.items()}
    if actual != expected:
        raise ClassificationError(f"self-test bucket mismatch: {actual!r}")
    print("ARKUS_TEST_CLASSIFICATION_SELF_TEST GREEN")


def classify_tests_for_subset(
    manifest: dict, tests: Iterable[str]
) -> tuple[dict[str, list[str]], set[str]]:
    """Classify a synthetic subset without enforcing live-manifest completeness."""
    buckets: dict[str, list[str]] = {name: [] for name in manifest["buckets"]}
    declared = [
        (bucket, class_name, class_prefix(manifest, class_name))
        for bucket, classes in manifest["buckets"].items()
        for class_name in classes
    ]
    seen_classes: set[str] = set()
    for test in tests:
        matches = [
            (bucket, class_name)
            for bucket, class_name, prefix in declared
            if test.startswith(prefix)
        ]
        if len(matches) != 1:
            raise ClassificationError(
                f"synthetic test {test} matched {len(matches)} classifications"
            )
        bucket, class_name = matches[0]
        buckets[bucket].append(test)
        seen_classes.add(class_name)
    return buckets, seen_classes


def main(argv: list[str]) -> int:
    try:
        if len(argv) < 2:
            raise ClassificationError("usage: test-suite.py self-test|validate|run <BUCKET|ALL>")
        command = argv[1]
        if command == "self-test":
            self_test()
            return 0
        manifest = load_manifest()
        if command == "validate":
            validate_live(manifest)
            return 0
        if command == "run" and len(argv) == 3:
            return run_bucket(manifest, argv[2])
        raise ClassificationError("usage: test-suite.py self-test|validate|run <BUCKET|ALL>")
    except (ClassificationError, json.JSONDecodeError, OSError) as exc:
        print(f"ARKUS_TEST_CLASSIFICATION_ERROR {exc}", file=sys.stderr)
        return 2


if __name__ == "__main__":
    raise SystemExit(main(sys.argv))
