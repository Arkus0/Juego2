#!/usr/bin/env python3
"""Structural audit for the derived accepted-state navigation index.

The index is never acceptance authority. This checker only proves that its hints
do not contradict repository contracts they point at. It intentionally does not
infer acceptance from Status lines and does not require the index freshness SHA
to equal the current branch while unrelated PROCESS_ONLY work is in flight.
"""
from __future__ import annotations

import json
from pathlib import Path
import re
import sys

ROOT = Path(__file__).resolve().parents[1]
INDEX = ROOT / "Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json"


def locate_wp(wp: str) -> Path:
    matches = list((ROOT / "Docs/workpacks").glob(f"**/{wp}.md"))
    if len(matches) != 1:
        raise ValueError(f"{wp}: expected exactly one workpack contract, got {len(matches)}")
    return matches[0]


def status(path: Path) -> str:
    text = path.read_text(encoding="utf-8")
    matches = re.findall(r"^Status:\s*(.+?)\s*$", text, re.M | re.I)
    if len(matches) != 1:
        raise ValueError(f"{path}: expected exactly one Status line, got {len(matches)}")
    return matches[0].strip().upper()


def accepted_status(value: str) -> bool:
    return "COMPLETE" in value and "ACCEPTED" in value


def audit() -> None:
    raw = json.loads(INDEX.read_text(encoding="utf-8"))
    errors: list[str] = []
    if raw.get("schema") != "arkus.accepted-state-index@1":
        errors.append("unexpected accepted-state index schema")
    if raw.get("authority") != "DERIVED_NAVIGATION_ONLY":
        errors.append("accepted-state index must remain DERIVED_NAVIGATION_ONLY")
    if raw.get("projection_phase") != "DOCSYNC_PERSISTED":
        errors.append("accepted-state index projection_phase must be DOCSYNC_PERSISTED")

    tracks = raw.get("tracks")
    if not isinstance(tracks, dict) or not tracks:
        errors.append("tracks must be a non-empty object")
        tracks = {}

    for track, entry in tracks.items():
        if not isinstance(entry, dict):
            errors.append(f"{track}: entry is not an object")
            continue
        accepted = entry.get("accepted_workpacks_hint")
        if not isinstance(accepted, list) or not all(isinstance(x, str) for x in accepted):
            errors.append(f"{track}: accepted_workpacks_hint must be a string list")
            continue
        if len(accepted) != len(set(accepted)):
            errors.append(f"{track}: accepted_workpacks_hint contains duplicates")
        for wp in accepted:
            try:
                path = locate_wp(wp)
                got = status(path)
                if not accepted_status(got):
                    errors.append(f"{track}: derived accepted hint {wp} contradicts contract Status {got!r}")
            except (ValueError, OSError) as exc:
                errors.append(f"{track}: {exc}")

        next_wp = entry.get("next_contract_hint")
        if next_wp is not None:
            if not isinstance(next_wp, str) or not next_wp:
                errors.append(f"{track}: next_contract_hint must be null or non-empty string")
            else:
                if next_wp in accepted:
                    errors.append(f"{track}: next_contract_hint {next_wp} is already in accepted hints")
                try:
                    next_path = locate_wp(next_wp)
                    if accepted_status(status(next_path)):
                        errors.append(f"{track}: next_contract_hint {next_wp} already has COMPLETE / ACCEPTED status")
                except (ValueError, OSError) as exc:
                    errors.append(f"{track}: next contract: {exc}")

        sources = entry.get("sources")
        if not isinstance(sources, list) or not all(isinstance(x, str) for x in sources):
            errors.append(f"{track}: sources must be a string list")
        else:
            if len(sources) != len(set(sources)):
                errors.append(f"{track}: sources contains duplicates")
            for source in sources:
                if not (ROOT / source).is_file():
                    errors.append(f"{track}: source path does not exist: {source}")

    cross = raw.get("cross_track_contract_hints")
    if not isinstance(cross, list):
        errors.append("cross_track_contract_hints must be a list")
    else:
        for index, hint in enumerate(cross):
            if not isinstance(hint, dict):
                errors.append(f"cross-track hint {index} is not an object")
                continue
            source = hint.get("source")
            if not isinstance(source, str) or not (ROOT / source).is_file():
                errors.append(f"cross-track hint {index} has missing source: {source!r}")
            requires = hint.get("requires")
            if not isinstance(requires, list) or not requires:
                errors.append(f"cross-track hint {index} has no requirements")

    if errors:
        for error in errors:
            print(f"ACCEPTED_STATE_ERROR: {error}", file=sys.stderr)
        raise SystemExit(1)
    print(f"ACCEPTED_STATE_STRUCTURAL_GREEN tracks={len(tracks)}")


def self_test() -> None:
    if not accepted_status("COMPLETE / ACCEPTED"):
        raise AssertionError("canonical accepted status was rejected")
    if accepted_status("PLANNED / NOT_STARTED"):
        raise AssertionError("planned status was accepted")
    print("ACCEPTED_STATE_AUDIT_SELF_TEST_GREEN")


def main() -> int:
    if len(sys.argv) != 2 or sys.argv[1] not in {"audit", "self-test"}:
        print("usage: accepted-state-audit.py audit|self-test", file=sys.stderr)
        return 2
    if sys.argv[1] == "self-test":
        self_test()
    else:
        audit()
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
