#!/usr/bin/env python3
"""Fail-closed scanner for the exact retained WP-H1-ASSET-CLOUD evidence surface."""

from __future__ import annotations

import argparse
import base64
import json
import xml.etree.ElementTree as ET
from pathlib import Path
from xml.sax.saxutils import escape as xml_escape

MAX_FILE_BYTES = 5 * 1024 * 1024
RECEIPT = Path("H1_ASSET_CLOUD_RECEIPT.json")
UNITY_RESULT = Path("unity/editmode-results.xml")
EXPECTED_FILES = {RECEIPT, UNITY_RESULT}
PRIVATE_CHUNK_BYTES = 96
PRIVATE_SAMPLE_COUNT = 5


def die(message: str) -> "NoReturn":
    raise SystemExit(f"H1_ASSET_EVIDENCE_RED: {message}")


def private_payloads(vault_root: Path):
    sensitive_roots = (
        vault_root / "h1" / "source-slice",
        vault_root / "h1" / "distributions",
    )
    found = 0
    for sensitive_root in sensitive_roots:
        if not sensitive_root.is_dir():
            die(f"private sensitive root missing: {sensitive_root.relative_to(vault_root)}")
        for path in sorted(p for p in sensitive_root.rglob("*") if p.is_file()):
            if path.suffix.lower() == ".meta":
                continue
            payload = path.read_bytes()
            if not payload:
                die(f"private sensitive payload is empty: {path.relative_to(vault_root)}")
            found += 1
            yield path.relative_to(vault_root), payload
    if found == 0:
        die("no private sensitive payloads found")


def sample_chunks(payload: bytes):
    if len(payload) <= PRIVATE_CHUNK_BYTES:
        yield payload
        return

    max_start = len(payload) - PRIVATE_CHUNK_BYTES
    offsets = {
        0,
        max_start,
        *(round(max_start * i / (PRIVATE_SAMPLE_COUNT - 1)) for i in range(PRIVATE_SAMPLE_COUNT)),
    }
    for offset in sorted(max(0, min(max_start, int(value))) for value in offsets):
        offset -= offset % 3
        yield payload[offset : offset + PRIVATE_CHUNK_BYTES]


def private_markers(vault_root: Path):
    seen = set()
    for source, payload in private_payloads(vault_root):
        for chunk in sample_chunks(payload):
            variants = {
                "raw": chunk,
                "base64": base64.b64encode(chunk),
                "hex": chunk.hex().encode("ascii"),
            }
            try:
                text = chunk.decode("utf-8")
            except UnicodeDecodeError:
                text = None
            if text is not None:
                escaped = xml_escape(text).encode("utf-8")
                if escaped != chunk:
                    variants["xml-escaped"] = escaped

            for encoding, marker in variants.items():
                if len(marker) < 32:
                    continue
                key = (encoding, marker)
                if key in seen:
                    continue
                seen.add(key)
                yield source, encoding, marker


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("root", type=Path)
    parser.add_argument("--private-root", type=Path, required=True)
    args = parser.parse_args()

    root = args.root.resolve()
    vault_root = args.private_root.resolve()
    if not root.is_dir():
        die(f"evidence root missing: {root}")
    if not vault_root.is_dir():
        die(f"private root missing: {vault_root}")

    files = sorted(path for path in root.rglob("*") if path.is_file())
    rels = {path.relative_to(root) for path in files}
    if rels != EXPECTED_FILES:
        observed = sorted(map(str, rels))
        expected = sorted(map(str, EXPECTED_FILES))
        die(f"public evidence file set mismatch: observed={observed} expected={expected}")

    markers = list(private_markers(vault_root))
    for path in files:
        rel = path.relative_to(root)
        blob = path.read_bytes()
        if len(blob) > MAX_FILE_BYTES:
            die(f"oversized public evidence file: {rel}")

        if rel == RECEIPT:
            try:
                payload = json.loads(blob.decode("utf-8"))
            except Exception as exc:
                die(f"invalid public receipt {rel}: {exc}")
            if payload.get("schemaId") != "arkus.h1-asset-cloud-receipt@1":
                die(f"unexpected receipt schema: {rel}")
        elif rel == UNITY_RESULT:
            try:
                ET.fromstring(blob)
            except Exception as exc:
                die(f"invalid Unity XML evidence {rel}: {exc}")

        for source, encoding, marker in markers:
            if marker in blob:
                die(
                    f"private source content retained in {rel} "
                    f"from {source} encoding={encoding}"
                )

    print(f"H1_ASSET_EVIDENCE_GREEN files={len(files)} private_markers={len(markers)}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
