#!/usr/bin/env python3
"""Verify and optionally mount the private H1 asset baseline.

This tool never downloads assets itself. The caller must provide a checked-out
private vault. It binds that vault to the already accepted WP-H1-04 authority,
verifies exact source bytes and Unity GUID sidecars, and only then mounts the
accepted SourceSlice into the ignored Unity project path.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import re
import shutil
import sys
from pathlib import Path, PurePosixPath

GUID_RE = re.compile(r"(?m)^guid:\s*([0-9a-f]{32})\s*$")


def die(message: str) -> "NoReturn":
    raise SystemExit(f"H1_ASSET_VAULT_RED: {message}")


def load_json(path: Path) -> dict:
    try:
        return json.loads(path.read_text(encoding="utf-8"))
    except FileNotFoundError:
        die(f"missing required JSON: {path}")
    except json.JSONDecodeError as exc:
        die(f"invalid JSON {path}: {exc}")


def sha256(path: Path) -> str:
    h = hashlib.sha256()
    try:
        with path.open("rb") as stream:
            for chunk in iter(lambda: stream.read(1024 * 1024), b""):
                h.update(chunk)
    except FileNotFoundError:
        die(f"missing required file: {path}")
    return h.hexdigest()


def safe_relative(value: str) -> Path:
    posix = PurePosixPath(value)
    if posix.is_absolute() or ".." in posix.parts or not posix.parts:
        die(f"unsafe vault path: {value!r}")
    return Path(*posix.parts)


def parse_meta_guid(path: Path) -> str:
    try:
        text = path.read_text(encoding="utf-8")
    except FileNotFoundError:
        die(f"missing Unity meta sidecar: {path}")
    except UnicodeDecodeError:
        die(f"Unity meta sidecar is not UTF-8 text: {path}")
    match = GUID_RE.search(text)
    if not match:
        die(f"Unity meta sidecar has no canonical guid line: {path}")
    return match.group(1)


def expected_catalogue_guids(catalogue: dict) -> dict[tuple[str, str], set[str]]:
    result: dict[tuple[str, str], set[str]] = {}
    for entry in catalogue.get("entries", []):
        source_id = entry.get("sourceId")
        content_hash = entry.get("contentSha256")
        guid = entry.get("nativeGuid")
        if source_id and content_hash and guid:
            result.setdefault((source_id, content_hash), set()).add(guid)
    return result


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--public-root", type=Path, required=True)
    parser.add_argument("--vault-root", type=Path, required=True)
    parser.add_argument("--candidate-sha", required=True)
    parser.add_argument("--vault-sha", required=True)
    parser.add_argument("--mount", action="store_true")
    parser.add_argument("--receipt", type=Path)
    args = parser.parse_args()

    if not re.fullmatch(r"[0-9a-f]{40}", args.candidate_sha):
        die("candidate SHA must be exact lowercase 40-hex")
    if not re.fullmatch(r"[0-9a-f]{40}", args.vault_sha):
        die("vault SHA must be exact lowercase 40-hex")

    public_root = args.public_root.resolve()
    vault_root = args.vault_root.resolve()

    authority_path = public_root / "Docs/evidence/WP-H1-04/SOURCE_ADOPTION.json"
    catalogue_path = public_root / "Unity/ArkusUnity/Assets/Arkus/H1/CatalogueMapping.json"
    manifest_path = vault_root / "h1/H1_SOURCE_SLICE_MANIFEST.json"

    authority = load_json(authority_path)
    catalogue = load_json(catalogue_path)
    manifest = load_json(manifest_path)

    if authority.get("schemaId") != "arkus.h1-04-source-adoption@1":
        die("unexpected H1-04 source-adoption schema")
    if manifest.get("schemaId") != "arkus.h1-asset-cloud-vault@2":
        die("unexpected private vault manifest schema")

    public_sources = {item["sourceId"]: item for item in authority.get("sources", [])}
    vault_sources = {item["sourceId"]: item for item in manifest.get("sourceDistributions", [])}
    if set(vault_sources) != set(public_sources):
        die(
            "vault source IDs differ from accepted H1-04 source IDs: "
            f"vault={sorted(vault_sources)} public={sorted(public_sources)}"
        )

    verified_distributions = []
    for source_id in sorted(public_sources):
        accepted = public_sources[source_id]
        supplied = vault_sources[source_id]
        for key in ("distributionSha256", "licenseId"):
            if supplied.get(key) != accepted.get(key):
                die(f"{source_id} {key} differs from H1-04 authority")
        payload_rel = safe_relative(supplied.get("vaultPath", ""))
        payload = vault_root / payload_rel
        observed = sha256(payload)
        if observed != accepted["distributionSha256"]:
            die(
                f"distribution hash mismatch for {source_id}: "
                f"expected={accepted['distributionSha256']} observed={observed}"
            )
        verified_distributions.append(
            {"sourceId": source_id, "sha256": observed, "vaultPath": payload_rel.as_posix()}
        )

    accepted_slices = {
        (item["sourceId"], item["contentSha256"]): item
        for item in authority.get("slices", [])
    }
    catalogue_guids = expected_catalogue_guids(catalogue)

    files = manifest.get("files", [])
    if len(files) != 4:
        die(f"expected exactly four H1-04 compatibility-slice files, found {len(files)}")

    verified_files = []
    for item in files:
        file_rel = safe_relative(item.get("path", ""))
        meta_rel = safe_relative(item.get("metaPath", ""))
        source_id = item.get("sourceId")
        expected_hash = item.get("sha256")
        expected_guid = item.get("unityGuid")
        if not all((source_id, expected_hash, expected_guid)):
            die(f"incomplete file manifest entry: {item}")

        accepted_slice = accepted_slices.get((source_id, expected_hash))
        if accepted_slice is None:
            die(f"file {file_rel} is not part of accepted H1-04 SourceSlice")

        observed_hash = sha256(vault_root / file_rel)
        if observed_hash != expected_hash:
            die(
                f"content hash mismatch for {file_rel}: "
                f"expected={expected_hash} observed={observed_hash}"
            )

        observed_guid = parse_meta_guid(vault_root / meta_rel)
        if observed_guid != expected_guid:
            die(
                f"Unity GUID mismatch for {meta_rel}: "
                f"expected={expected_guid} observed={observed_guid}"
            )

        allowed_guids = catalogue_guids.get((source_id, expected_hash), set())
        if expected_guid not in allowed_guids:
            die(
                f"manifest GUID {expected_guid} for {file_rel} is not present in "
                "the accepted H1-04 catalogue mapping"
            )

        verified_files.append(
            {
                "sourceId": source_id,
                "name": file_rel.name,
                "sha256": observed_hash,
                "unityGuid": observed_guid,
            }
        )

    target = public_root / "Unity/ArkusUnity/Assets/Arkus/H1/SourceSlice"
    if args.mount:
        if target.exists() and any(target.iterdir()):
            die(f"refusing to mount over non-empty SourceSlice: {target}")
        target.mkdir(parents=True, exist_ok=True)
        for item in files:
            file_rel = safe_relative(item["path"])
            meta_rel = safe_relative(item["metaPath"])
            shutil.copy2(vault_root / file_rel, target / file_rel.name)
            shutil.copy2(vault_root / meta_rel, target / meta_rel.name)

    receipt = {
        "schemaId": "arkus.h1-asset-cloud-receipt@1",
        "candidateSha": args.candidate_sha,
        "assetVaultRepository": "Arkus0/Juego2-assets",
        "assetVaultSha": args.vault_sha,
        "authority": "WP-H1-04",
        "distributionMode": "private-vault-readonly-source",
        "verifiedDistributions": verified_distributions,
        "verifiedSourceSlice": verified_files,
        "mounted": bool(args.mount),
        "result": "GREEN",
    }

    rendered = json.dumps(receipt, indent=2, sort_keys=True) + "\n"
    if args.receipt:
        args.receipt.parent.mkdir(parents=True, exist_ok=True)
        args.receipt.write_text(rendered, encoding="utf-8")
    print(rendered, end="")
    print("H1_ASSET_VAULT_GREEN", file=sys.stderr)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
