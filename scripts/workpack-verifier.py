#!/usr/bin/env python3
"""Checker-owned routing for exact-SHA workpack verifiers.

This module deliberately delegates proof to the existing per-WP scripts.  The
registry only owns routing, and its self-test proves exact equivalence with the
legacy shell dispatchers before any later migration is allowed to delete them.
"""
from __future__ import annotations

import argparse
import json
import os
from pathlib import Path
import re
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[1]
REGISTRY = ROOT / "Docs/engineering/workpack-verifiers.json"
LEGACY = {
    "verify": ROOT / "scripts/arkus-verify-exact-sha.sh",
    "observe": ROOT / "scripts/arkus-observe-exact-sha.sh",
}
SCHEMA = "arkus.workpack-verifiers@1"
WP_RE = re.compile(r"^WP-[A-Z0-9-]+$")
BODY_WP_RE = re.compile(r"^WP:\s*`?(WP-[A-Z0-9-]+)`?\s*$", re.M)


def load_registry() -> dict[str, dict[str, str]]:
    raw = json.loads(REGISTRY.read_text(encoding="utf-8"))
    if raw.get("schema") != SCHEMA:
        raise ValueError(f"unexpected registry schema: {raw.get('schema')!r}")
    entries = raw.get("entries")
    if not isinstance(entries, dict) or not entries:
        raise ValueError("registry entries must be a non-empty object")
    return entries


def legacy_map(path: Path) -> dict[str, str]:
    text = path.read_text(encoding="utf-8")
    # Parse only the case dispatcher, never the resolve_wp helper above it.
    marker = 'case "$(resolve_wp)" in'
    if marker not in text:
        raise ValueError(f"legacy dispatcher marker missing: {path}")
    case_text = text.split(marker, 1)[1]
    found: dict[str, str] = {}
    block_re = re.compile(
        r"^\s*(WP-[A-Z0-9-]+)\)\s*\n"
        r"(?P<body>.*?)(?=^\s*(?:WP-[A-Z0-9-]+|\*)\)|\Z)",
        re.M | re.S,
    )
    exec_re = re.compile(r"exec\s+bash\s+(scripts/[A-Za-z0-9_.\-/]+)\s+\"\$@\"")
    for match in block_re.finditer(case_text):
        wp = match.group(1)
        exec_match = exec_re.search(match.group("body"))
        if exec_match is None:
            raise ValueError(f"legacy dispatcher {path.name}: {wp} has no exact exec target")
        if wp in found:
            raise ValueError(f"legacy dispatcher {path.name}: duplicate {wp}")
        found[wp] = exec_match.group(1)
    if not found:
        raise ValueError(f"legacy dispatcher {path.name}: no routes parsed")
    return found


def resolve_wp(explicit: str | None, body: str | None) -> str:
    if explicit:
        wp = explicit.strip().upper()
        if not WP_RE.fullmatch(wp):
            raise ValueError(f"invalid workpack id: {explicit!r}")
        return wp
    matches = BODY_WP_RE.findall(body or "")
    unique = sorted(set(matches))
    if len(matches) != 1 or len(unique) != 1:
        raise ValueError("PR body must contain exactly one canonical `WP: WP-...` line when --wp/ARKUS_WP is absent")
    return unique[0]


def route(mode: str, wp: str, entries: dict[str, dict[str, str]]) -> str:
    entry = entries.get(wp)
    if not isinstance(entry, dict):
        raise KeyError(f"no {mode} route registered for {wp}")
    target = entry.get(mode)
    if not isinstance(target, str) or not target.startswith("scripts/"):
        raise KeyError(f"no {mode} route registered for {wp}")
    target_path = (ROOT / target).resolve()
    scripts_root = (ROOT / "scripts").resolve()
    if scripts_root not in target_path.parents:
        raise ValueError(f"route escapes scripts/: {target}")
    if not target_path.is_file():
        raise FileNotFoundError(f"registered route does not exist: {target}")
    return target


def self_test() -> None:
    entries = load_registry()
    errors: list[str] = []

    for wp, entry in entries.items():
        if not WP_RE.fullmatch(wp):
            errors.append(f"invalid registry workpack id: {wp}")
            continue
        if set(entry) != {"verify", "observe"}:
            errors.append(f"{wp}: expected exactly verify+observe keys, got {sorted(entry)}")
            continue
        for mode in ("verify", "observe"):
            try:
                route(mode, wp, entries)
            except Exception as exc:  # deterministic diagnostics for CI
                errors.append(f"{wp}/{mode}: {exc}")

    for mode, legacy_path in LEGACY.items():
        try:
            legacy = legacy_map(legacy_path)
        except Exception as exc:
            errors.append(f"{mode}: cannot parse legacy dispatcher: {exc}")
            continue
        registered = {wp: entry[mode] for wp, entry in entries.items() if mode in entry}
        if legacy != registered:
            missing = sorted(set(legacy) - set(registered))
            extra = sorted(set(registered) - set(legacy))
            changed = sorted(wp for wp in set(legacy) & set(registered) if legacy[wp] != registered[wp])
            errors.append(
                f"{mode}: registry/legacy divergence; missing={missing}, extra={extra}, changed={changed}"
            )

    # Causal controls for resolver ambiguity/normalization.
    controls = [
        (None, "WP: `WP-DW-01`\n", "WP-DW-01"),
        ("wp-dw-01", "WP: `WP-HK-00`\n", "WP-DW-01"),
    ]
    for explicit, body, expected in controls:
        if resolve_wp(explicit, body) != expected:
            errors.append(f"resolver control failed: explicit={explicit!r}")
    for bad_body in ("", "WP: WP-DW-01\nWP: WP-HK-00\n"):
        try:
            resolve_wp(None, bad_body)
        except ValueError:
            pass
        else:
            errors.append(f"ambiguous/missing resolver control stayed green: {bad_body!r}")

    if errors:
        for error in errors:
            print(f"ERROR: {error}", file=sys.stderr)
        raise SystemExit(1)
    print(f"WORKPACK_VERIFIER_REGISTRY_GREEN entries={len(entries)} legacy_equivalence=exact")


def main() -> int:
    parser = argparse.ArgumentParser()
    sub = parser.add_subparsers(dest="command", required=True)
    sub.add_parser("self-test")

    p_resolve = sub.add_parser("resolve")
    p_resolve.add_argument("--mode", choices=("verify", "observe"), required=True)
    p_resolve.add_argument("--wp")
    p_resolve.add_argument("--pr-body")

    p_exec = sub.add_parser("exec")
    p_exec.add_argument("--mode", choices=("verify", "observe"), required=True)
    p_exec.add_argument("--wp")
    p_exec.add_argument("--pr-body")
    p_exec.add_argument("args", nargs=argparse.REMAINDER)

    args = parser.parse_args()
    if args.command == "self-test":
        self_test()
        return 0

    entries = load_registry()
    explicit = args.wp or os.environ.get("ARKUS_WP")
    body = args.pr_body if args.pr_body is not None else os.environ.get("PR_BODY", "")
    wp = resolve_wp(explicit, body)
    target = route(args.mode, wp, entries)
    if args.command == "resolve":
        print(target)
        return 0

    forwarded = list(args.args)
    if forwarded and forwarded[0] == "--":
        forwarded = forwarded[1:]
    completed = subprocess.run(["bash", str(ROOT / target), *forwarded], cwd=ROOT)
    return completed.returncode


if __name__ == "__main__":
    raise SystemExit(main())
