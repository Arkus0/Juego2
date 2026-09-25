#!/usr/bin/env python3
"""Fail-closed scanner for the exact retained WP-H1-ASSET-CLOUD evidence surface."""

from __future__ import annotations

import argparse
import base64
import hashlib
import json
import re
import xml.etree.ElementTree as ET
from pathlib import Path

MAX_FILE_BYTES = 5 * 1024 * 1024
RECEIPT = Path("H1_ASSET_CLOUD_RECEIPT.json")
UNITY_RESULT = Path("unity/editmode-results.xml")
EXPECTED_FILES = {RECEIPT, UNITY_RESULT}
PRIVATE_BLOCK_BYTES = 96
MIN_PROVABLE_LEAK_BYTES = (PRIVATE_BLOCK_BYTES * 2) - 1
BASE64_TOKEN = re.compile(rb"[A-Za-z0-9+/]{256,}={0,2}")
URLSAFE_BASE64_TOKEN = re.compile(rb"[A-Za-z0-9_-]{256,}={0,2}")
HEX_TOKEN = re.compile(rb"[0-9A-Fa-f]{384,}")


def die(message: str) -> "NoReturn":
    raise SystemExit(f"H1_ASSET_EVIDENCE_RED: {message}")


def sensitive_paths(vault_root: Path):
    roots = (
        vault_root / "h1" / "source-slice",
        vault_root / "h1" / "distributions",
    )
    found = 0
    for root in roots:
        if not root.is_dir():
            die(f"private sensitive root missing: {root.relative_to(vault_root)}")
        for path in sorted(p for p in root.rglob("*") if p.is_file()):
            if path.suffix.lower() == ".meta":
                continue
            if path.stat().st_size == 0:
                die(f"private sensitive payload is empty: {path.relative_to(vault_root)}")
            found += 1
            yield path
    if found == 0:
        die("no private sensitive payloads found")


def block_digest(block: bytes) -> bytes:
    return hashlib.blake2b(block, digest_size=16).digest()


def build_private_oracle(vault_root: Path):
    fingerprints = {}
    short_payloads = []
    payload_count = 0
    block_count = 0

    for path in sensitive_paths(vault_root):
        payload_count += 1
        source = str(path.relative_to(vault_root))
        size = path.stat().st_size
        if size < PRIVATE_BLOCK_BYTES:
            payload = path.read_bytes()
            short_payloads.append((source, payload))
            continue

        with path.open("rb") as handle:
            while True:
                block = handle.read(PRIVATE_BLOCK_BYTES)
                if len(block) < PRIVATE_BLOCK_BYTES:
                    break
                fingerprints.setdefault(block_digest(block), source)
                block_count += 1

        if size % PRIVATE_BLOCK_BYTES:
            with path.open("rb") as handle:
                handle.seek(-PRIVATE_BLOCK_BYTES, 2)
                tail = handle.read(PRIVATE_BLOCK_BYTES)
            fingerprints.setdefault(block_digest(tail), source)
            block_count += 1

    if not fingerprints and not short_payloads:
        die("private content oracle is empty")

    return fingerprints, short_payloads, payload_count, block_count


def detect_private_content(blob: bytes, fingerprints, short_payloads):
    for source, payload in short_payloads:
        if payload in blob:
            return source

    if len(blob) < PRIVATE_BLOCK_BYTES:
        return None

    limit = len(blob) - PRIVATE_BLOCK_BYTES + 1
    for offset in range(limit):
        block = blob[offset : offset + PRIVATE_BLOCK_BYTES]
        source = fingerprints.get(block_digest(block))
        if source is not None:
            return source
    return None


def iter_json_strings(value):
    if isinstance(value, str):
        yield value
    elif isinstance(value, dict):
        for key, item in value.items():
            yield str(key)
            yield from iter_json_strings(item)
    elif isinstance(value, list):
        for item in value:
            yield from iter_json_strings(item)


def iter_xml_strings(root: ET.Element):
    for node in root.iter():
        for value in node.attrib.values():
            yield value
        if node.text:
            yield node.text
        if node.tail:
            yield node.tail


def decoded_candidates(text_blob: bytes):
    seen = set()

    for match in BASE64_TOKEN.finditer(text_blob):
        token = match.group(0)
        try:
            decoded = base64.b64decode(token, validate=True)
        except Exception:
            continue
        if len(decoded) >= MIN_PROVABLE_LEAK_BYTES and decoded not in seen:
            seen.add(decoded)
            yield "base64", decoded

    for match in URLSAFE_BASE64_TOKEN.finditer(text_blob):
        token = match.group(0)
        try:
            padding = b"=" * ((4 - (len(token) % 4)) % 4)
            decoded = base64.urlsafe_b64decode(token + padding)
        except Exception:
            continue
        if len(decoded) >= MIN_PROVABLE_LEAK_BYTES and decoded not in seen:
            seen.add(decoded)
            yield "urlsafe-base64", decoded

    for match in HEX_TOKEN.finditer(text_blob):
        token = match.group(0)
        if len(token) % 2:
            continue
        try:
            decoded = bytes.fromhex(token.decode("ascii"))
        except Exception:
            continue
        if len(decoded) >= MIN_PROVABLE_LEAK_BYTES and decoded not in seen:
            seen.add(decoded)
            yield "hex", decoded


def prove_no_private_content(rel: Path, raw: bytes, logical_strings, fingerprints, short_payloads):
    source = detect_private_content(raw, fingerprints, short_payloads)
    if source is not None:
        die(f"private source content retained in {rel} from {source} encoding=raw")

    for text in logical_strings:
        logical = text.encode("utf-8", errors="strict")
        source = detect_private_content(logical, fingerprints, short_payloads)
        if source is not None:
            die(f"private source content retained in {rel} from {source} encoding=logical-text")
        for encoding, decoded in decoded_candidates(logical):
            source = detect_private_content(decoded, fingerprints, short_payloads)
            if source is not None:
                die(f"private source content retained in {rel} from {source} encoding={encoding}")


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

    fingerprints, short_payloads, payload_count, block_count = build_private_oracle(vault_root)

    for path in files:
        rel = path.relative_to(root)
        raw = path.read_bytes()
        if len(raw) > MAX_FILE_BYTES:
            die(f"oversized public evidence file: {rel}")

        if rel == RECEIPT:
            try:
                payload = json.loads(raw.decode("utf-8"))
            except Exception as exc:
                die(f"invalid public receipt {rel}: {exc}")
            if payload.get("schemaId") != "arkus.h1-asset-cloud-receipt@1":
                die(f"unexpected receipt schema: {rel}")
            logical_strings = list(iter_json_strings(payload))
        elif rel == UNITY_RESULT:
            try:
                xml_root = ET.fromstring(raw)
            except Exception as exc:
                die(f"invalid Unity XML evidence {rel}: {exc}")
            logical_strings = list(iter_xml_strings(xml_root))
        else:
            die(f"unexpected retained evidence path: {rel}")

        prove_no_private_content(
            rel,
            raw,
            logical_strings,
            fingerprints,
            short_payloads,
        )

    print(
        "H1_ASSET_EVIDENCE_GREEN "
        f"files={len(files)} private_payloads={payload_count} private_blocks={block_count} "
        f"min_provable_contiguous_leak_bytes={MIN_PROVABLE_LEAK_BYTES}"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
