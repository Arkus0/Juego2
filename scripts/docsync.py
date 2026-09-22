#!/usr/bin/env python3
"""Deterministic core of post-PASS DocSync.

The command persists facts that are mechanical from an already accepted
transition. It deliberately does NOT synthesize semantic accepted claims or
rewrite free-form track/root prose: those remain reconciliation surfaces, not an
oracle owned by this script.
"""
from __future__ import annotations

import argparse
from datetime import date
import json
from pathlib import Path
import re
import sys

ROOT = Path(__file__).resolve().parents[1]
INDEX = ROOT / "Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json"
SHA_RE = re.compile(r"^[0-9a-f]{40}$")
WP_RE = re.compile(r"^WP-([A-Z0-9]+)-([A-Z0-9-]+)$")
STATUS_RE = re.compile(r"^Status:\s*(.+)$", re.M)
BLOCKS_RE = re.compile(r"^Blocks:\s*(.+)$", re.M)
ACCEPTANCE_RE = re.compile(r"^Acceptance:\s*.*$", re.M)
WP_TOKEN_RE = re.compile(r"WP-[A-Z0-9]+-[A-Z0-9-]+")


def fail(message: str) -> "NoReturn":
    raise SystemExit(f"error: {message}")


def wp_path(wp: str) -> Path:
    matches = list((ROOT / "Docs/workpacks").glob(f"**/{wp}.md"))
    if len(matches) != 1:
        fail(f"expected exactly one authoritative workpack file for {wp}; found {len(matches)}")
    return matches[0]


def parse_track(wp: str) -> str:
    match = WP_RE.fullmatch(wp)
    if not match:
        fail(f"invalid workpack id: {wp}")
    return match.group(1)


def first_blocked_wp(text: str) -> str | None:
    match = BLOCKS_RE.search(text)
    if not match:
        return None
    tokens = WP_TOKEN_RE.findall(match.group(1))
    return tokens[0] if len(tokens) == 1 else None


def validate_sha(name: str, value: str) -> str:
    value = value.lower()
    if not SHA_RE.fullmatch(value):
        fail(f"{name} must be a 40-hex SHA")
    return value


def acceptance_line(candidate: str, review: str, pr: int, merge: str, validation_run: str, evidence_rel: str) -> str:
    return (
        f"Acceptance: frozen candidate `{candidate}`; independent PASS review `#{review}`; "
        f"PR `#{pr}`; merge `{merge}`; final frozen exact-SHA validation Actions `{validation_run}` GREEN; "
        f"post-PASS DocSync `{evidence_rel}`."
    )


def update_wp_document(path: Path, line: str) -> None:
    text = path.read_text(encoding="utf-8")
    statuses = STATUS_RE.findall(text)
    if len(statuses) != 1:
        fail(f"{path}: expected exactly one Status line")
    text = STATUS_RE.sub("Status: COMPLETE / ACCEPTED", text, count=1)
    if ACCEPTANCE_RE.search(text):
        text = ACCEPTANCE_RE.sub(line, text, count=1)
    else:
        blocks = BLOCKS_RE.search(text)
        if not blocks:
            fail(f"{path}: missing Blocks line; refusing implicit insertion point")
        insert_at = blocks.end()
        text = text[:insert_at] + "\n\n" + line + text[insert_at:]
    path.write_text(text, encoding="utf-8")


def render_evidence(*, wp: str, when: str, candidate: str, review: str, pr: int, merge: str,
                    validation_run: str, source_main: str, next_wp: str | None) -> str:
    nxt = next_wp or "NONE"
    return f"""# {wp} — Post-PASS DocSync

DOCSYNC_STATUS: **DOCSYNC_COMPLETE**  
MODE: **PROCESS_ONLY / DOCUMENTATION_ONLY**  
DATE: {when}

## Accepted identity

- Frozen candidate SHA: `{candidate}`
- Independent Reviewer verdict: **PASS**
- Review: `#{review}`
- PR: `#{pr}`
- Merge commit: `{merge}`
- Final frozen exact-SHA validation: Actions run `{validation_run}` GREEN
- Source main reconstructed before DocSync persistence: `{source_main}`

## Mechanical actions

1. Marked `{wp}` COMPLETE / ACCEPTED with the exact accepted identity above.
2. Refreshed the derived accepted-state index from source main `{source_main}`.
3. Recorded the direct blocked workpack as `{nxt}` for navigation only; dependency validity still comes from authoritative workpack contracts and live accepted state.

## Boundary

This generated core records accepted identity and derived navigation only. It does not invent or restate the semantic claim proved by the accepted implementation, alter implementation bytes, waive cross-track prerequisites, or make the accepted-state index authoritative over the workpack contracts/evidence.

## Remaining reconciliation

Track/root README prose is reconciled only where its effective current-state meaning changed. The command intentionally does not synthesize that prose. Run `docsync.py check --wp {wp}` after the complete documentation reconciliation.

`DOCSYNC_COMPLETE`
"""


def update_index(*, wp: str, track: str, source_main: str, when: str, evidence_rel: str,
                 wp_rel: str, next_wp: str | None) -> None:
    raw = json.loads(INDEX.read_text(encoding="utf-8"))
    if raw.get("authority") != "DERIVED_NAVIGATION_ONLY":
        fail("accepted-state index authority changed; refusing to write")
    tracks = raw.get("tracks")
    if not isinstance(tracks, dict) or track not in tracks:
        fail(f"accepted-state index has no existing track entry for {track}")
    entry = tracks[track]
    accepted = entry.get("accepted_workpacks_hint")
    if not isinstance(accepted, list):
        fail(f"track {track}: accepted_workpacks_hint is not a list")
    if wp not in accepted:
        accepted.append(wp)
    entry["accepted_state_hint"] = f"THROUGH_{wp}"
    entry["next_contract_hint"] = next_wp
    sources = entry.get("sources")
    if not isinstance(sources, list):
        fail(f"track {track}: sources is not a list")
    for source in (wp_rel, evidence_rel):
        if source not in sources:
            sources.append(source)
    if next_wp:
        next_rel = wp_path(next_wp).relative_to(ROOT).as_posix()
        if next_rel not in sources:
            sources.append(next_rel)
    raw["projection_phase"] = "DOCSYNC_PERSISTED"
    raw["generated_from_main_sha"] = source_main
    raw["generated_on"] = when
    INDEX.write_text(json.dumps(raw, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")


def parse_evidence_identity(path: Path) -> dict[str, str]:
    text = path.read_text(encoding="utf-8")
    patterns = {
        "candidate": r"^- Frozen candidate SHA: `([0-9a-f]{40})`$",
        "review": r"^- Review: `#([^`]+)`$",
        "pr": r"^- PR: `#(\d+)`$",
        "merge": r"^- Merge commit: `([0-9a-f]{40})`$",
        "run": r"^- Final frozen exact-SHA validation: Actions run `([^`]+)` GREEN$",
        "source_main": r"^- Source main reconstructed before DocSync persistence: `([0-9a-f]{40})`$",
    }
    out: dict[str, str] = {}
    for key, pattern in patterns.items():
        found = re.findall(pattern, text, re.M)
        if len(found) != 1:
            fail(f"{path}: expected exactly one generated {key} field")
        out[key] = found[0]
    return out


def check(wp: str) -> None:
    track = parse_track(wp)
    path = wp_path(wp)
    text = path.read_text(encoding="utf-8")
    status = STATUS_RE.findall(text)
    if status != ["COMPLETE / ACCEPTED"]:
        fail(f"{wp}: Status is not exactly COMPLETE / ACCEPTED")
    evidence = ROOT / "Docs/evidence" / wp / "DOCSYNC.md"
    if not evidence.is_file():
        fail(f"{wp}: missing {evidence.relative_to(ROOT)}")
    identity = parse_evidence_identity(evidence)
    line = acceptance_line(identity["candidate"], identity["review"], int(identity["pr"]), identity["merge"], identity["run"], evidence.relative_to(ROOT).as_posix())
    if text.count(line) != 1:
        fail(f"{wp}: workpack Acceptance line does not match generated DocSync identity")
    raw = json.loads(INDEX.read_text(encoding="utf-8"))
    entry = raw.get("tracks", {}).get(track)
    if not isinstance(entry, dict) or wp not in entry.get("accepted_workpacks_hint", []):
        fail(f"{wp}: accepted-state index omits accepted workpack")
    if raw.get("generated_from_main_sha") != identity["source_main"]:
        fail(f"{wp}: index/evidence source-main identity mismatch")
    next_wp = first_blocked_wp(text)
    if entry.get("next_contract_hint") != next_wp:
        fail(f"{wp}: next_contract_hint does not equal direct Blocks contract ({next_wp})")
    print(f"DOCSYNC_CORE_GREEN wp={wp} next={next_wp or 'NONE'}")


def prepare(args: argparse.Namespace) -> None:
    wp = args.wp.upper()
    track = parse_track(wp)
    path = wp_path(wp)
    text = path.read_text(encoding="utf-8")
    next_wp = first_blocked_wp(text)
    if next_wp:
        next_text = wp_path(next_wp).read_text(encoding="utf-8")
        if wp not in next_text:
            fail(f"{next_wp}: direct successor contract does not mention predecessor {wp}; refusing navigation inference")
    candidate = validate_sha("candidate", args.candidate)
    merge = validate_sha("merge", args.merge)
    source_main = validate_sha("source-main", args.source_main)
    if not str(args.review).strip():
        fail("review is required")
    if not str(args.validation_run).strip():
        fail("validation-run is required")
    when = args.date or date.today().isoformat()
    evidence = ROOT / "Docs/evidence" / wp / "DOCSYNC.md"
    evidence.parent.mkdir(parents=True, exist_ok=True)
    evidence_rel = evidence.relative_to(ROOT).as_posix()
    wp_rel = path.relative_to(ROOT).as_posix()
    line = acceptance_line(candidate, str(args.review), args.pr, merge, str(args.validation_run), evidence_rel)
    update_wp_document(path, line)
    evidence.write_text(render_evidence(wp=wp, when=when, candidate=candidate, review=str(args.review), pr=args.pr, merge=merge, validation_run=str(args.validation_run), source_main=source_main, next_wp=next_wp), encoding="utf-8")
    update_index(wp=wp, track=track, source_main=source_main, when=when, evidence_rel=evidence_rel, wp_rel=wp_rel, next_wp=next_wp)
    print(f"DOCSYNC_CORE_PREPARED wp={wp} next={next_wp or 'NONE'}")
    print("Manual semantic reconciliation still required only for track/root README text whose effective current-state meaning changed.")


def main() -> int:
    parser = argparse.ArgumentParser()
    sub = parser.add_subparsers(dest="command", required=True)
    p_prepare = sub.add_parser("prepare")
    p_prepare.add_argument("--wp", required=True)
    p_prepare.add_argument("--candidate", required=True)
    p_prepare.add_argument("--review", required=True)
    p_prepare.add_argument("--pr", type=int, required=True)
    p_prepare.add_argument("--merge", required=True)
    p_prepare.add_argument("--validation-run", required=True)
    p_prepare.add_argument("--source-main", required=True)
    p_prepare.add_argument("--date")
    p_check = sub.add_parser("check")
    p_check.add_argument("--wp", required=True)
    args = parser.parse_args()
    if args.command == "prepare":
        prepare(args)
    else:
        check(args.wp.upper())
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
