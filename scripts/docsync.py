#!/usr/bin/env python3
"""Mechanical DocSync identity stamper/checker.

This tool deliberately does not decide acceptance or write semantic DocSync prose.
It only copies/verifies an already-authorized acceptance identity across the WP
contract and its post-PASS DocSync file, then checks the derived accepted-state
index does not contradict that accepted WP.
"""
from __future__ import annotations

import argparse
import json
from pathlib import Path
import re
import subprocess
import sys
from dataclasses import dataclass

ROOT = Path(__file__).resolve().parents[1]
INDEX = ROOT / "Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json"
SHA_RE = r"[0-9a-f]{40}"


class DocSyncError(RuntimeError):
    pass


@dataclass(frozen=True)
class Identity:
    candidate: str
    review: str
    pr: str
    merge: str
    validation_run: str


def _one(pattern: str, text: str, label: str, flags: int = 0) -> str:
    values = re.findall(pattern, text, flags)
    if len(values) != 1:
        raise DocSyncError(f"{label}: expected exactly one match, got {len(values)}")
    value = values[0]
    if isinstance(value, tuple):
        value = next((part for part in value if part), "")
    return str(value)


def parse_docsync(path: Path) -> tuple[str, Identity]:
    text = path.read_text(encoding="utf-8")
    wp = _one(r"^#\s+(WP-[A-Z0-9-]+)\b", text, f"{path}: WP heading", re.M)
    if not re.search(r"^DOCSYNC_STATUS:\s*\*\*DOCSYNC_COMPLETE\*\*\s*$", text, re.M):
        raise DocSyncError(f"{path}: DOCSYNC_STATUS is not DOCSYNC_COMPLETE")
    if not re.search(r"^- Independent Reviewer verdict:\s*\*\*PASS\*\*\s*$", text, re.M):
        raise DocSyncError(f"{path}: accepted result does not record independent PASS")
    ident = Identity(
        candidate=_one(rf"^- Frozen candidate SHA:\s*`({SHA_RE})`\s*$", text, f"{path}: candidate", re.M),
        review=_one(r"^- Review:\s*`#(\d+)`\s*$", text, f"{path}: review", re.M),
        pr=_one(r"^- PR:\s*`#(\d+)`\s*$", text, f"{path}: PR", re.M),
        merge=_one(rf"^- Merge commit:\s*`({SHA_RE})`\s*$", text, f"{path}: merge", re.M),
        validation_run=_one(r"^- Final[^\n]*validation[^\n]*?(?:Actions run\s*)?`(\d+)`\s*$", text, f"{path}: validation run", re.M | re.I),
    )
    return wp, ident


def locate_workpack(wp: str) -> Path:
    matches = list((ROOT / "Docs/workpacks").glob(f"**/{wp}.md"))
    if len(matches) != 1:
        raise DocSyncError(f"{wp}: expected exactly one workpack contract, got {len(matches)}")
    return matches[0]


def parse_workpack(path: Path) -> tuple[Identity, str]:
    text = path.read_text(encoding="utf-8")
    if not re.search(r"^Status:\s*COMPLETE\s*/\s*ACCEPTED\s*$", text, re.M):
        raise DocSyncError(f"{path}: Status is not COMPLETE / ACCEPTED")
    line = _one(r"^(Acceptance:[^\n]+)$", text, f"{path}: Acceptance line", re.M)
    ident = Identity(
        candidate=_one(rf"frozen candidate\s+`({SHA_RE})`", line, f"{path}: candidate", re.I),
        review=_one(r"independent PASS review\s+`#(\d+)`", line, f"{path}: review", re.I),
        pr=_one(r"PR\s+`#(\d+)`", line, f"{path}: PR", re.I),
        merge=_one(rf"merge\s+`({SHA_RE})`", line, f"{path}: merge", re.I),
        validation_run=_one(r"validation(?: Actions)?\s+`(\d+)`", line, f"{path}: validation run", re.I),
    )
    docsync = _one(r"post-PASS DocSync\s+`([^`]+)`", line, f"{path}: DocSync path", re.I)
    return ident, docsync


def track_for(wp: str) -> str | None:
    if wp.startswith("WP-HK-"):
        return "H0"
    for prefix in ("H1", "CITY", "PA", "CTX", "DW"):
        if wp.startswith(f"WP-{prefix}-"):
            return prefix
    return None


def check_index(wp: str) -> None:
    track = track_for(wp)
    if track is None or track == "H0":
        return
    raw = json.loads(INDEX.read_text(encoding="utf-8"))
    if raw.get("authority") != "DERIVED_NAVIGATION_ONLY":
        raise DocSyncError("accepted-state index lost DERIVED_NAVIGATION_ONLY authority marker")
    entry = (raw.get("tracks") or {}).get(track)
    if not isinstance(entry, dict):
        raise DocSyncError(f"accepted-state index has no {track} entry")
    accepted = entry.get("accepted_workpacks_hint") or []
    if wp not in accepted:
        raise DocSyncError(f"accepted-state index does not include accepted {wp}")
    if entry.get("next_contract_hint") == wp:
        raise DocSyncError(f"accepted-state index still names accepted {wp} as next contract")


def check_pair(docsync_path: Path) -> None:
    wp, doc_ident = parse_docsync(docsync_path)
    workpack = locate_workpack(wp)
    wp_ident, linked_docsync = parse_workpack(workpack)
    expected_rel = docsync_path.relative_to(ROOT).as_posix()
    if linked_docsync != expected_rel:
        raise DocSyncError(f"{wp}: workpack links {linked_docsync!r}, expected {expected_rel!r}")
    if wp_ident != doc_ident:
        raise DocSyncError(f"{wp}: identity divergence workpack={wp_ident} docsync={doc_ident}")
    check_index(wp)
    print(f"DOCSYNC_IDENTITY_GREEN wp={wp} pr={doc_ident.pr} candidate={doc_ident.candidate}")


def acceptance_line(identity: Identity, docsync_rel: str) -> str:
    return (
        f"Acceptance: frozen candidate `{identity.candidate}`; independent PASS review `#{identity.review}`; "
        f"PR `#{identity.pr}`; merge `{identity.merge}`; final fully GREEN exact-SHA validation Actions "
        f"`{identity.validation_run}`; post-PASS DocSync `{docsync_rel}`."
    )


def stamp(workpack: Path, docsync: Path, identity: Identity) -> None:
    if not workpack.is_file() or not docsync.is_file():
        raise DocSyncError("stamp requires existing workpack and DocSync files; semantic DocSync prose is never synthesized")
    wp_text = workpack.read_text(encoding="utf-8")
    wp_text, status_count = re.subn(r"^Status:\s*[^\n]+$", "Status: COMPLETE / ACCEPTED", wp_text, count=1, flags=re.M)
    if status_count != 1:
        raise DocSyncError(f"{workpack}: expected one Status line")
    rel = docsync.relative_to(ROOT).as_posix()
    rendered = acceptance_line(identity, rel)
    if re.search(r"^Acceptance:[^\n]+$", wp_text, re.M):
        wp_text = re.sub(r"^Acceptance:[^\n]+$", rendered, wp_text, count=1, flags=re.M)
    else:
        blocks = re.search(r"^Blocks:[^\n]*$", wp_text, re.M)
        if blocks is None:
            raise DocSyncError(f"{workpack}: cannot place Acceptance line; missing Blocks line")
        insert_at = blocks.end()
        wp_text = wp_text[:insert_at] + "\n\n" + rendered + wp_text[insert_at:]
    workpack.write_text(wp_text, encoding="utf-8")

    ds_text = docsync.read_text(encoding="utf-8")
    if not re.search(r"^## Accepted result\s*$", ds_text, re.M):
        raise DocSyncError(f"{docsync}: missing ## Accepted result section")
    block = (
        "## Accepted result\n\n"
        f"- Frozen candidate SHA: `{identity.candidate}`\n"
        "- Independent Reviewer verdict: **PASS**\n"
        f"- Review: `#{identity.review}`\n"
        f"- PR: `#{identity.pr}`\n"
        f"- Merge commit: `{identity.merge}`\n"
        f"- Final fully GREEN exact-SHA validation: Actions run `{identity.validation_run}`\n"
    )
    ds_text, count = re.subn(
        r"^## Accepted result\s*$.*?(?=^##\s+|\Z)", block + "\n", ds_text, count=1, flags=re.M | re.S
    )
    if count != 1:
        raise DocSyncError(f"{docsync}: could not replace Accepted result block")
    docsync.write_text(ds_text, encoding="utf-8")
    print(f"DOCSYNC_IDENTITY_STAMPED wp={workpack.stem} pr={identity.pr}")


def changed_docsync(base: str, head: str) -> list[Path]:
    cmd = ["git", "diff", "--name-only", f"{base}...{head}", "--", "Docs/evidence"]
    result = subprocess.run(cmd, cwd=ROOT, text=True, capture_output=True)
    if result.returncode != 0:
        raise DocSyncError(result.stderr.strip() or "git diff failed")
    paths: list[Path] = []
    for name in result.stdout.splitlines():
        if name.endswith("/DOCSYNC.md"):
            path = ROOT / name
            if path.is_file():
                paths.append(path)
    return paths


def self_test() -> None:
    good = Identity("a" * 40, "123", "42", "b" * 40, "999")
    line = acceptance_line(good, "Docs/evidence/WP-DW-99/DOCSYNC.md")
    for needle in (good.candidate, "#123", "#42", good.merge, "`999`", "WP-DW-99/DOCSYNC.md"):
        if needle not in line:
            raise DocSyncError(f"self-test render omission: {needle}")
    if track_for("WP-DW-03") != "DW" or track_for("WP-HK-GATE") != "H0" or track_for("X") is not None:
        raise DocSyncError("self-test track resolver failed")
    print("DOCSYNC_TOOL_SELF_TEST_GREEN")


def main() -> int:
    parser = argparse.ArgumentParser()
    sub = parser.add_subparsers(dest="command", required=True)
    sub.add_parser("self-test")

    check = sub.add_parser("check")
    check.add_argument("--docsync", required=True)

    changed = sub.add_parser("check-changed")
    changed.add_argument("--base", required=True)
    changed.add_argument("--head", required=True)

    stamp_p = sub.add_parser("stamp")
    stamp_p.add_argument("--workpack", required=True)
    stamp_p.add_argument("--docsync", required=True)
    stamp_p.add_argument("--candidate", required=True)
    stamp_p.add_argument("--review", required=True)
    stamp_p.add_argument("--pr", required=True)
    stamp_p.add_argument("--merge", required=True)
    stamp_p.add_argument("--validation-run", required=True)

    args = parser.parse_args()
    try:
        if args.command == "self-test":
            self_test()
        elif args.command == "check":
            check_pair((ROOT / args.docsync).resolve())
        elif args.command == "check-changed":
            paths = changed_docsync(args.base, args.head)
            if not paths:
                print("DOCSYNC_CHANGED_CHECK_GREEN changed_docsync=0")
            for path in paths:
                check_pair(path)
        else:
            ident = Identity(args.candidate.lower(), str(args.review).lstrip("#"), str(args.pr).lstrip("#"), args.merge.lower(), str(args.validation_run))
            if not re.fullmatch(SHA_RE, ident.candidate) or not re.fullmatch(SHA_RE, ident.merge):
                raise DocSyncError("candidate and merge must be exact 40-hex SHAs")
            if not all(re.fullmatch(r"\d+", value) for value in (ident.review, ident.pr, ident.validation_run)):
                raise DocSyncError("review, PR and validation run must be numeric identifiers")
            stamp((ROOT / args.workpack).resolve(), (ROOT / args.docsync).resolve(), ident)
        return 0
    except (DocSyncError, OSError, ValueError, json.JSONDecodeError) as exc:
        print(f"DOCSYNC_ERROR: {exc}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
