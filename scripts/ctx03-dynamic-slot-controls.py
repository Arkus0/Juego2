#!/usr/bin/env python3
"""Independent future-extension control for dynamic placeholders in role profiles.

The production dynamic checker owns slot classification. This control independently
scans every canonical profile read surface so a new dynamic placeholder cannot hide
inside conditional_reads merely because it was not added to initial_reads.
"""
from __future__ import annotations

import importlib.util
import json
import sys
from pathlib import Path

TARGET = Path("scripts/ctx03-dynamic-context-check.py")
PROFILES = Path("Docs/engineering/context-bootstrap-profiles.json")


def load_target(root: Path):
    spec = importlib.util.spec_from_file_location("ctx03_dynamic_slot_target", root / TARGET)
    if spec is None or spec.loader is None:
        raise RuntimeError("cannot load dynamic context checker")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


def values(profile: dict):
    for item in profile.get("initial_reads") or []:
        if isinstance(item, str):
            yield item
    conditionals = profile.get("conditional_reads") or {}
    if isinstance(conditionals, dict):
        for item in conditionals.values():
            if isinstance(item, str):
                yield item


def audit(root: Path) -> list[str]:
    target = load_target(root)
    profiles = json.loads((root / PROFILES).read_text(encoding="utf-8")).get("profiles", {})
    observed: set[str] = set()
    for profile in profiles.values():
        if not isinstance(profile, dict):
            continue
        for text in values(profile):
            observed.update(f"<{name}>" for name in target.PLACEHOLDER_RE.findall(text))
    known = set(target.DYNAMIC_SLOT_CLASSIFICATION)
    unknown = sorted(observed - known)
    return [f"unreviewed dynamic placeholders in canonical profile read surfaces require explicit classification: {unknown}"] if unknown else []


def self_test() -> None:
    class T:
        pass
    assert list(values({"initial_reads": ["<A>"], "conditional_reads": {"x": "<B>"}})) == ["<A>", "<B>"]
    print("ctx03-dynamic-slot-controls self-test: PASS")


def main() -> int:
    root = Path(".").resolve()
    if "--self-test" in sys.argv:
        self_test()
        return 0
    try:
        errors = audit(root)
    except Exception as exc:
        print(f"CTX03_DYNAMIC_SLOT_CONTROLS: INFRA_ERROR\n{exc}", file=sys.stderr)
        return 23
    if errors:
        print("CTX03_DYNAMIC_SLOT_CONTROLS: FAIL", file=sys.stderr)
        for error in errors:
            print(f"- {error}", file=sys.stderr)
        return 20
    print("CTX03_DYNAMIC_SLOT_CONTROLS: PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
