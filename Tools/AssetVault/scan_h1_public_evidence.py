#!/usr/bin/env python3
"""Fail-closed allowlist scanner for public WP-H1-ASSET-CLOUD evidence."""

from __future__ import annotations

import argparse
import json
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

MAX_FILE_BYTES = 5 * 1024 * 1024
RECEIPT = Path("H1_ASSET_CLOUD_RECEIPT.json")
UNITY_DIR = Path("unity")


def die(message: str) -> "NoReturn":
    raise SystemExit(f"H1_ASSET_EVIDENCE_RED: {message}")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("root", type=Path)
    args = parser.parse_args()

    root = args.root.resolve()
    if not root.is_dir():
        die(f"evidence root missing: {root}")

    files = sorted(path for path in root.rglob("*") if path.is_file())
    if not files:
        die("no public evidence files found")

    for path in files:
        rel = path.relative_to(root)
        if path.stat().st_size > MAX_FILE_BYTES:
            die(f"oversized public evidence file: {rel}")

        if rel == RECEIPT:
            try:
                payload = json.loads(path.read_text(encoding="utf-8"))
            except Exception as exc:
                die(f"invalid public receipt {rel}: {exc}")
            if payload.get("schemaId") != "arkus.h1-asset-cloud-receipt@1":
                die(f"unexpected receipt schema: {rel}")
            continue

        if len(rel.parts) >= 2 and rel.parts[0] == UNITY_DIR.name and path.suffix.lower() == ".xml":
            try:
                ET.parse(path)
            except Exception as exc:
                die(f"invalid Unity XML evidence {rel}: {exc}")
            continue

        die(f"non-allowlisted public evidence path: {rel}")

    print(f"H1_ASSET_EVIDENCE_GREEN files={len(files)}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
