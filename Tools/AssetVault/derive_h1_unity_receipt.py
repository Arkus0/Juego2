#!/usr/bin/env python3
"""Derive a closed public receipt from ephemeral Unity NUnit XML."""

from __future__ import annotations

import argparse
import json
import re
import xml.etree.ElementTree as ET
from pathlib import Path

SHA40_RE = re.compile(r"[0-9a-f]{40}")
EXPECTED_TEST_COUNT = 12
EXPECTED_UNITY_VERSION = "6000.3.24f1"
EXPECTED_PLATFORM = "EditMode"
REQUIRED_PROBE = (
    "Arkus.H1.Editor.Tests.H1AssetCloudTests."
    "PrivateVaultMount_RecreatesAcceptedH104UnityIdentities"
)
VERSION_PROBE = "Arkus.H1.Editor.Tests.H1BaselineTests.EffectiveEditorMatchesPinnedPatch"


def die(message: str) -> "NoReturn":
    raise SystemExit(f"H1_ASSET_UNITY_RECEIPT_RED: {message}")


def int_attr(root: ET.Element, name: str, expected: int) -> int:
    raw = root.attrib.get(name)
    try:
        value = int(raw) if raw is not None else None
    except ValueError:
        value = None
    if value != expected:
        die(f"Unity summary {name} mismatch: observed={raw!r} expected={expected}")
    return expected


def require_exact_pass(test_cases: list[ET.Element], fullname: str, label: str) -> None:
    matches = [node for node in test_cases if node.attrib.get("fullname") == fullname]
    if len(matches) != 1:
        die(f"expected exactly one {label}, found {len(matches)}")
    if matches[0].attrib.get("result") != "Passed":
        die(f"{label} did not pass")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--xml", type=Path, required=True)
    parser.add_argument("--candidate-sha", required=True)
    parser.add_argument("--vault-sha", required=True)
    parser.add_argument("--receipt", type=Path, required=True)
    args = parser.parse_args()

    if SHA40_RE.fullmatch(args.candidate_sha) is None:
        die("candidate SHA must be exact lowercase 40-hex")
    if SHA40_RE.fullmatch(args.vault_sha) is None:
        die("vault SHA must be exact lowercase 40-hex")

    try:
        root = ET.parse(args.xml).getroot()
    except FileNotFoundError:
        die(f"Unity XML missing: {args.xml}")
    except ET.ParseError as exc:
        die(f"invalid Unity XML: {exc}")

    if root.tag != "test-run":
        die(f"unexpected Unity XML root: {root.tag}")
    if root.attrib.get("result") != "Passed":
        die("Unity suite result is not Passed")

    int_attr(root, "testcasecount", EXPECTED_TEST_COUNT)
    int_attr(root, "total", EXPECTED_TEST_COUNT)
    int_attr(root, "passed", EXPECTED_TEST_COUNT)
    int_attr(root, "failed", 0)
    int_attr(root, "skipped", 0)
    int_attr(root, "inconclusive", 0)

    test_cases = list(root.iter("test-case"))
    if len(test_cases) != EXPECTED_TEST_COUNT:
        die(f"expected {EXPECTED_TEST_COUNT} concrete test cases, found {len(test_cases)}")
    if any(node.attrib.get("result") != "Passed" for node in test_cases):
        die("not every concrete Unity test case is Passed")

    require_exact_pass(test_cases, REQUIRED_PROBE, "H1 asset-cloud identity probe")
    require_exact_pass(test_cases, VERSION_PROBE, "effective Unity-version probe")

    platform_values = {
        node.attrib.get("value")
        for node in root.iter("property")
        if node.attrib.get("name") == "platform"
    }
    if EXPECTED_PLATFORM not in platform_values:
        die(f"Unity platform evidence missing expected {EXPECTED_PLATFORM}")

    receipt = {
        "schemaId": "arkus.h1-asset-cloud-unity-receipt@1",
        "candidateSha": args.candidate_sha,
        "assetVaultSha": args.vault_sha,
        "unityVersion": EXPECTED_UNITY_VERSION,
        "testPlatform": EXPECTED_PLATFORM,
        "testCount": EXPECTED_TEST_COUNT,
        "passed": EXPECTED_TEST_COUNT,
        "failed": 0,
        "skipped": 0,
        "requiredProbe": "H1_04_NATIVE_IDENTITY",
        "requiredProbeResult": "Passed",
        "suiteResult": "Passed",
    }
    args.receipt.parent.mkdir(parents=True, exist_ok=True)
    args.receipt.write_text(json.dumps(receipt, indent=2, sort_keys=True) + "\n", encoding="utf-8")
    print("H1_ASSET_CLOUD_UNITY_RECEIPT_GREEN probe=H1_04_NATIVE_IDENTITY result=Passed tests=12/12")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
