#!/usr/bin/env python3
"""Validate the closed retained WP-H1-ASSET-CLOUD evidence surface."""

from __future__ import annotations

import argparse
import json
import re
from pathlib import Path

RECEIPT = Path("H1_ASSET_CLOUD_RECEIPT.json")
UNITY_RECEIPT = Path("H1_ASSET_CLOUD_UNITY_RECEIPT.json")
EXPECTED_FILES = {RECEIPT, UNITY_RECEIPT}
MAX_RECEIPT_BYTES = 16 * 1024
SHA40_RE = re.compile(r"[0-9a-f]{40}")
SHA256_RE = re.compile(r"[0-9a-f]{64}")
GUID_RE = re.compile(r"[0-9a-f]{32}")

EXPECTED_DISTRIBUTIONS = [
    {
        "sha256": "b9d757dd2608a5cee4d9ee1e8183f6cb4cad9d27480841a905180def9c7d8b10",
        "sourceId": "quaternius-medieval-source",
        "vaultPath": "h1/distributions/quaternius-medieval-source.payload",
    },
    {
        "sha256": "0556d52f6bce01c0982b3548ee3cdfa1b8270977507001f62cbdfcc405570842",
        "sourceId": "quaternius-ual1-source",
        "vaultPath": "h1/distributions/quaternius-ual1-source.payload",
    },
]
EXPECTED_SOURCE_SLICE = [
    {
        "name": "Wall_Plaster_Window_Wide_Flat.fbx",
        "sha256": "45825c565b9d1027036ce7fc922f7e7a7d69bc05e459d886eb738bf1eafc92b8",
        "sourceId": "quaternius-medieval-source",
        "unityGuid": "a914dbae2609f0107a8bce353d33727c",
    },
    {
        "name": "MI_Plaster.mat",
        "sha256": "3fcfc0359d1460009858533461893c626e25ea52364ae4e85f4a6bf84fc3ce69",
        "sourceId": "quaternius-medieval-source",
        "unityGuid": "75fb52ef5e0f0ad40a28c36d06ecd99c",
    },
    {
        "name": "FacadeImportedMaterial.mat",
        "sha256": "e585297c2271fc83378e2eebe7f72ff9f8b66c2fc1b8a1eeb1c7abdfac5d2e29",
        "sourceId": "quaternius-medieval-source",
        "unityGuid": "963743a1121475e92497a201e301bf48",
    },
    {
        "name": "UAL1.fbx",
        "sha256": "0556d52f6bce01c0982b3548ee3cdfa1b8270977507001f62cbdfcc405570842",
        "sourceId": "quaternius-ual1-source",
        "unityGuid": "06d37381cd6d9bece36de7794e2fc74a",
    },
]
EXPECTED_UPLOAD_PATHS = [
    "artifacts/h1-asset-cloud-public/H1_ASSET_CLOUD_RECEIPT.json",
    "artifacts/h1-asset-cloud-public/H1_ASSET_CLOUD_UNITY_RECEIPT.json",
]
UPLOAD_STEP = "- name: Upload retained public-safe exact-SHA evidence only"


def die(message: str) -> "NoReturn":
    raise SystemExit(f"H1_ASSET_EVIDENCE_RED: {message}")


def require_exact_keys(payload: dict, expected: set[str], label: str) -> None:
    observed = set(payload)
    if observed != expected:
        die(f"{label} keys mismatch: observed={sorted(observed)} expected={sorted(expected)}")


def require_sha40(value, label: str, expected: str) -> None:
    if not isinstance(value, str) or SHA40_RE.fullmatch(value) is None:
        die(f"{label} must be exact lowercase 40-hex")
    if value != expected:
        die(f"{label} mismatch: observed={value} expected={expected}")


def require_int(value, label: str, expected: int) -> None:
    if isinstance(value, bool) or not isinstance(value, int):
        die(f"{label} must be an integer")
    if value != expected:
        die(f"{label} mismatch: observed={value} expected={expected}")


def load_receipt(path: Path) -> dict:
    raw = path.read_bytes()
    if len(raw) > MAX_RECEIPT_BYTES:
        die(f"oversized retained receipt: {path.name}")
    try:
        payload = json.loads(raw.decode("utf-8"))
    except Exception as exc:
        die(f"invalid retained JSON {path.name}: {exc}")
    if not isinstance(payload, dict):
        die(f"retained receipt must be a JSON object: {path.name}")
    return payload


def validate_asset_receipt(payload: dict, candidate_sha: str, vault_sha: str) -> None:
    require_exact_keys(
        payload,
        {
            "schemaId",
            "candidateSha",
            "assetVaultRepository",
            "assetVaultSha",
            "authority",
            "distributionMode",
            "verifiedDistributions",
            "verifiedSourceSlice",
            "mounted",
            "result",
        },
        RECEIPT.name,
    )
    if payload["schemaId"] != "arkus.h1-asset-cloud-receipt@1":
        die("unexpected asset receipt schemaId")
    require_sha40(payload["candidateSha"], "asset receipt candidateSha", candidate_sha)
    require_sha40(payload["assetVaultSha"], "asset receipt assetVaultSha", vault_sha)
    if payload["assetVaultRepository"] != "Arkus0/Juego2-assets":
        die("asset receipt repository mismatch")
    if payload["authority"] != "WP-H1-04":
        die("asset receipt authority must be WP-H1-04")
    if payload["distributionMode"] != "private-vault-readonly-source":
        die("asset receipt distributionMode mismatch")
    if payload["mounted"] is not False:
        die("retained asset receipt mounted must be false")
    if payload["result"] != "GREEN":
        die("asset receipt result must be GREEN")

    distributions = payload["verifiedDistributions"]
    if distributions != EXPECTED_DISTRIBUTIONS:
        die("asset receipt distribution identities mismatch")
    for item in distributions:
        require_exact_keys(item, {"sha256", "sourceId", "vaultPath"}, "verifiedDistributions entry")
        if SHA256_RE.fullmatch(item["sha256"]) is None:
            die("asset receipt contains a non-SHA256 distribution identifier")

    source_slice = payload["verifiedSourceSlice"]
    if source_slice != EXPECTED_SOURCE_SLICE:
        die("asset receipt SourceSlice identities mismatch")
    for item in source_slice:
        require_exact_keys(item, {"name", "sha256", "sourceId", "unityGuid"}, "verifiedSourceSlice entry")
        if SHA256_RE.fullmatch(item["sha256"]) is None or GUID_RE.fullmatch(item["unityGuid"]) is None:
            die("asset receipt SourceSlice identifier has invalid shape")


def validate_unity_receipt(payload: dict, candidate_sha: str, vault_sha: str) -> None:
    require_exact_keys(
        payload,
        {
            "schemaId",
            "candidateSha",
            "assetVaultSha",
            "unityVersion",
            "testPlatform",
            "testCount",
            "passed",
            "failed",
            "skipped",
            "requiredProbe",
            "requiredProbeResult",
            "suiteResult",
        },
        UNITY_RECEIPT.name,
    )
    if payload["schemaId"] != "arkus.h1-asset-cloud-unity-receipt@1":
        die("unexpected Unity receipt schemaId")
    require_sha40(payload["candidateSha"], "Unity receipt candidateSha", candidate_sha)
    require_sha40(payload["assetVaultSha"], "Unity receipt assetVaultSha", vault_sha)
    if payload["unityVersion"] != "6000.3.24f1":
        die("Unity receipt unityVersion mismatch")
    if payload["testPlatform"] != "EditMode":
        die("Unity receipt testPlatform mismatch")
    require_int(payload["testCount"], "Unity receipt testCount", 12)
    require_int(payload["passed"], "Unity receipt passed", 12)
    require_int(payload["failed"], "Unity receipt failed", 0)
    require_int(payload["skipped"], "Unity receipt skipped", 0)
    if payload["requiredProbe"] != "H1_04_NATIVE_IDENTITY":
        die("Unity receipt requiredProbe mismatch")
    if payload["requiredProbeResult"] != "Passed":
        die("Unity receipt requiredProbeResult must be Passed")
    if payload["suiteResult"] != "Passed":
        die("Unity receipt suiteResult must be Passed")


def validate_uploader_paths(workflow: Path) -> None:
    try:
        lines = workflow.read_text(encoding="utf-8").splitlines()
    except FileNotFoundError:
        die(f"workflow missing: {workflow}")
    markers = [i for i, line in enumerate(lines) if line.strip() == UPLOAD_STEP]
    if len(markers) != 1:
        die(f"expected exactly one retained-evidence upload step, found {len(markers)}")
    start = markers[0]
    path_indexes = [
        i for i in range(start + 1, len(lines)) if lines[i].strip() == "path: |"
    ]
    if len(path_indexes) != 1:
        die("retained-evidence upload step must contain exactly one path block")
    i = path_indexes[0] + 1
    observed = []
    while i < len(lines) and lines[i].startswith("            "):
        if lines[i].strip():
            observed.append(lines[i].strip())
        i += 1
    if observed != EXPECTED_UPLOAD_PATHS:
        die(f"uploader paths mismatch: observed={observed} expected={EXPECTED_UPLOAD_PATHS}")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("root", type=Path)
    parser.add_argument("--candidate-sha", required=True)
    parser.add_argument("--vault-sha", required=True)
    parser.add_argument("--workflow", type=Path, required=True)
    args = parser.parse_args()

    if SHA40_RE.fullmatch(args.candidate_sha) is None:
        die("expected candidate SHA must be exact lowercase 40-hex")
    if SHA40_RE.fullmatch(args.vault_sha) is None:
        die("expected vault SHA must be exact lowercase 40-hex")

    root = args.root.resolve()
    if not root.is_dir():
        die(f"evidence root missing: {root}")
    files = sorted(path for path in root.rglob("*") if path.is_file())
    rels = {path.relative_to(root) for path in files}
    if rels != EXPECTED_FILES:
        die(
            "public evidence file set mismatch: "
            f"observed={sorted(map(str, rels))} expected={sorted(map(str, EXPECTED_FILES))}"
        )

    validate_asset_receipt(load_receipt(root / RECEIPT), args.candidate_sha, args.vault_sha)
    validate_unity_receipt(load_receipt(root / UNITY_RECEIPT), args.candidate_sha, args.vault_sha)
    validate_uploader_paths(args.workflow)

    print("H1_ASSET_EVIDENCE_GREEN files=2 schemas=closed uploader_paths=2 arbitrary_text_channels=0")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
