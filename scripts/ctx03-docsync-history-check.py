#!/usr/bin/env python3
"""CTX-03 DocSync/history separation and derived-current-state validator."""
from __future__ import annotations

import argparse
import json
import re
import sys
import tempfile
from pathlib import Path

STATE = Path("Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json")
CTX_DIR = Path("Docs/workpacks/CTX")
CTX_README = CTX_DIR / "README.md"
ROOT_WP = Path("Docs/workpacks/README.md")
ROADMAP = Path("Docs/ROADMAP.md")
PROFILES = Path("Docs/engineering/context-bootstrap-profiles.json")
HISTORY = Path("Docs/history/CTX_PROCESS_HISTORY.md")
ROADMAP_HISTORY = Path("Docs/history/ROADMAP_ACCEPTED_CLOSURES.md")
CLASSIFICATION = Path("Docs/evidence/CTX-03/HISTORICAL_CLASSIFICATION.json")

WP_FILE_RE = re.compile(r"^WP-CTX-(\d+)\.md$")
STATUS_RE = re.compile(r"^Status:\s*\*\*(.+?)\*\*\s*$", re.MULTILINE)
REVIEW_RE = re.compile(r"independent PASS:\s*(?:review\s*)?`?#(\d+)`?", re.IGNORECASE)
PR_RE = re.compile(r"implementation PR:\s*`?#(\d+)`?", re.IGNORECASE)
NEXT_RE = re.compile(r"(?i)next[^\n]{0,100}CTX[^\n]{0,100}`?(WP-CTX-\d+)`?")


def load(root: Path, path: Path):
    return json.loads((root / path).read_text(encoding="utf-8"))


def contract_status(text: str) -> str:
    m = STATUS_RE.search(text)
    if not m:
        raise ValueError("CTX contract missing Status")
    return m.group(1).strip().upper()


def discover_ctx_authority(root: Path) -> tuple[list[str], str | None, list[str]]:
    """Derive accepted prefix + next contract without consulting derived current-state projections."""
    errors: list[str] = []
    contracts: list[tuple[int, str, str]] = []
    for path in (root / CTX_DIR).glob("WP-CTX-*.md"):
        m = WP_FILE_RE.match(path.name)
        if not m:
            continue
        wp = f"WP-CTX-{m.group(1)}"
        contracts.append((int(m.group(1)), wp, contract_status(path.read_text(encoding="utf-8"))))
    contracts.sort(key=lambda row: row[0])

    if not contracts:
        return [], None, ["no numeric CTX workpack contracts discovered"]
    numbers = [n for n, _wp, _status in contracts]
    if len(numbers) != len(set(numbers)):
        errors.append("duplicate numeric CTX workpack contract discovered")

    accepted: list[str] = []
    next_wp: str | None = None
    seen_open = False
    for _n, wp, status in contracts:
        is_complete = status == "COMPLETE"
        if is_complete and seen_open:
            errors.append(f"accepted CTX contract {wp} appears after an unaccepted predecessor")
        if not is_complete:
            seen_open = True
            if next_wp is None:
                next_wp = wp
            continue

        accepted.append(wp)
        suffix = wp.removeprefix("WP-CTX-")
        closure = root / f"Docs/evidence/CTX-{suffix}/DOCSYNC.md"
        if not closure.is_file():
            errors.append(f"accepted CTX contract {wp} has no required DocSync closure")
            continue
        ctext = closure.read_text(encoding="utf-8")
        if wp not in ctext:
            errors.append(f"DocSync closure for {wp} does not identify the accepted WP")
        if "DOCSYNC_COMPLETE" not in ctext and "DOCSYNC_PERSISTED" not in ctext:
            errors.append(f"DocSync closure for {wp} lacks persisted/complete status")
        if not REVIEW_RE.search(ctext):
            errors.append(f"DocSync closure for {wp} lacks independent PASS provenance")
        if not PR_RE.search(ctext):
            errors.append(f"DocSync closure for {wp} lacks implementation PR provenance")
    return accepted, next_wp, errors


def projected_next_mentions(text: str) -> set[str]:
    return set(NEXT_RE.findall(text))


def current_state_errors(root: Path) -> tuple[list[str], list[str], str | None]:
    accepted, next_wp, errors = discover_ctx_authority(root)
    state = load(root, STATE)
    ctx = state.get("tracks", {}).get("CTX") or {}
    if state.get("authority") != "DERIVED_NAVIGATION_ONLY":
        errors.append("accepted-state index authority drifted")
    if list(state.get("tracks", {})).count("CTX") != 1:
        errors.append("accepted-state index contains multiple CTX current-state rows")
    actual_accepted = ctx.get("accepted_workpacks_hint")
    if actual_accepted != accepted:
        errors.append(f"derived CTX accepted list mismatch: expected={accepted!r} actual={actual_accepted!r}")
    if ctx.get("next_contract_hint") != next_wp:
        errors.append(f"derived CTX next contract mismatch: expected={next_wp!r} actual={ctx.get('next_contract_hint')!r}")

    for label, path in (("CTX README", CTX_README), ("root workpack index", ROOT_WP)):
        if not (root / path).is_file():
            errors.append(f"{label} missing")
            continue
        mentions = projected_next_mentions((root / path).read_text(encoding="utf-8"))
        if next_wp is not None:
            wrong = sorted(m for m in mentions if m != next_wp)
            if wrong:
                errors.append(f"{label} contradicts derived next CTX contract {next_wp}: {wrong}")
        else:
            stale = sorted(m for m in mentions if m in accepted)
            if stale:
                errors.append(f"{label} still projects accepted CTX work as next: {stale}")
    return errors, accepted, next_wp


def check(root: Path) -> list[str]:
    errors, _accepted, _next = current_state_errors(root)
    profiles = load(root, PROFILES).get("profiles", {})
    for name, profile in profiles.items():
        reads = profile.get("initial_reads") if isinstance(profile, dict) else None
        if not isinstance(reads, list):
            errors.append(f"normal profile {name} initial_reads missing/not a list")
            continue
        for raw in reads:
            if isinstance(raw, str) and raw.startswith("Docs/history/"):
                errors.append(f"history leaked into normal {name} initial_reads: {raw}")

    if not (root / HISTORY).is_file():
        errors.append("CTX process history file missing")
    else:
        history = (root / HISTORY).read_text(encoding="utf-8")
        for pointer in ("#5273364796", "#5274094804", "#5274937744", "107694d3850a478849bffd9510dc030910fc8aa3"):
            if pointer not in history:
                errors.append(f"CTX history reconstruction pointer missing: {pointer}")

    road = (root / ROADMAP).read_text(encoding="utf-8")
    if "Docs/history/ROADMAP_ACCEPTED_CLOSURES.md" not in road:
        errors.append("current ROADMAP does not point to separated accepted closure history")
    if not (root / ROADMAP_HISTORY).is_file():
        errors.append("ROADMAP accepted-closure history file missing")
    else:
        history = (root / ROADMAP_HISTORY).read_text(encoding="utf-8")
        for pointer in (
            "#5257350871", "#5260337340", "#5261636151",
            "23a9fd4373a803187cd9391b1459cd48975177f6",
            "0fa3d4fb039a3d0049cea0f3eed1c83128ec7dcd",
            "048d2e449d5ff81e8bcc35bc15664ea4ceec18ca",
        ):
            if pointer not in history:
                errors.append(f"ROADMAP history reconstruction pointer missing: {pointer}")
        if "review `#5257350871`" in road or "review `#5261636151`" in road:
            errors.append("verbose accepted H0 closure chronology still duplicated in current ROADMAP")

    classification = load(root, CLASSIFICATION)
    cases = classification.get("cases") or []
    if not cases or not any(c.get("decision") == "ADOPT" for c in cases if isinstance(c, dict)):
        errors.append("historical classification has no adopted mechanical family")
    if not any(c.get("decision") == "REJECT" for c in cases if isinstance(c, dict)):
        errors.append("historical classification never rejects inappropriate mechanization")
    return errors


def _write(root: Path, rel: str, text: str) -> None:
    p = root / rel
    p.parent.mkdir(parents=True, exist_ok=True)
    p.write_text(text, encoding="utf-8")


def _fixture_state(accepted: list[str], next_wp: str | None) -> str:
    return json.dumps({"authority": "DERIVED_NAVIGATION_ONLY", "tracks": {"CTX": {"accepted_workpacks_hint": accepted, "next_contract_hint": next_wp}}})


def _closure(wp: str, review: int, pr: int) -> str:
    return f"WP: `{wp}`\nindependent PASS: review `#{review}`\nimplementation PR: `#{pr}`\nDOCSYNC_COMPLETE\n"


def self_test() -> None:
    with tempfile.TemporaryDirectory() as td:
        root = Path(td)
        _write(root, "Docs/workpacks/CTX/WP-CTX-01.md", "Status: **COMPLETE**\n")
        _write(root, "Docs/workpacks/CTX/WP-CTX-02.md", "Status: **FROZEN PLAN / NOT_STARTED**\n")
        _write(root, "Docs/evidence/CTX-01/DOCSYNC.md", _closure("WP-CTX-01", 1, 2))
        _write(root, str(STATE), _fixture_state(["WP-CTX-01"], "WP-CTX-02"))
        _write(root, str(CTX_README), "The next CTX action is `WP-CTX-02`.\n")
        _write(root, str(ROOT_WP), "next dependency-valid CTX action is `WP-CTX-02`.\n")
        e, accepted, nxt = current_state_errors(root)
        assert e == [] and accepted == ["WP-CTX-01"] and nxt == "WP-CTX-02"

        _write(root, "Docs/workpacks/CTX/WP-CTX-02.md", "Status: **COMPLETE**\n")
        _write(root, "Docs/evidence/CTX-02/DOCSYNC.md", _closure("WP-CTX-02", 3, 4))
        _write(root, str(STATE), _fixture_state(["WP-CTX-01", "WP-CTX-02"], None))
        _write(root, str(CTX_README), "CTX complete.\n")
        _write(root, str(ROOT_WP), "CTX complete.\n")
        e, accepted, nxt = current_state_errors(root)
        assert e == [] and accepted == ["WP-CTX-01", "WP-CTX-02"] and nxt is None

        # Numeric ordering must not regress when the track grows into two digits.
        _write(root, "Docs/workpacks/CTX/WP-CTX-10.md", "Status: **NOT_STARTED**\n")
        _write(root, "Docs/workpacks/CTX/WP-CTX-03.md", "Status: **NOT_STARTED**\n")
        _write(root, str(STATE), _fixture_state(["WP-CTX-01", "WP-CTX-02"], "WP-CTX-03"))
        _write(root, str(CTX_README), "The next CTX action is `WP-CTX-03`.\n")
        _write(root, str(ROOT_WP), "next dependency-valid CTX action is `WP-CTX-03`.\n")
        e, _accepted, nxt = current_state_errors(root)
        assert e == [] and nxt == "WP-CTX-03"
    print("ctx03-docsync-history self-test: PASS")


def main() -> int:
    p = argparse.ArgumentParser()
    p.add_argument("--repo-root", type=Path, default=Path("."))
    p.add_argument("--self-test", action="store_true")
    args = p.parse_args()
    if args.self_test:
        self_test(); return 0
    try:
        errors = check(args.repo_root.resolve())
    except Exception as exc:
        print(f"CTX03_DOCSYNC_HISTORY: INFRA_ERROR\n{exc}", file=sys.stderr)
        return 23
    if errors:
        print("CTX03_DOCSYNC_HISTORY: FAIL", file=sys.stderr)
        for error in errors:
            print(f"- {error}", file=sys.stderr)
        return 20
    print("CTX03_DOCSYNC_HISTORY: PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
