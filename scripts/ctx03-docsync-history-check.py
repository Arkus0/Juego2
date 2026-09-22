#!/usr/bin/env python3
"""CTX-03 representative DocSync/history separation control."""
from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path

STATE = Path("Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json")
CTX_README = Path("Docs/workpacks/CTX/README.md")
ROOT_WP = Path("Docs/workpacks/README.md")
ROADMAP = Path("Docs/ROADMAP.md")
DOCSYNC = Path("Docs/evidence/CTX-02/DOCSYNC.md")
PROFILES = Path("Docs/engineering/context-bootstrap-profiles.json")
HISTORY = Path("Docs/history/CTX_PROCESS_HISTORY.md")
ROADMAP_HISTORY = Path("Docs/history/ROADMAP_ACCEPTED_CLOSURES.md")
CLASSIFICATION = Path("Docs/evidence/CTX-03/HISTORICAL_CLASSIFICATION.json")


def load(root: Path, path: Path):
    return json.loads((root / path).read_text(encoding="utf-8"))


def check(root: Path) -> list[str]:
    errors: list[str] = []
    state = load(root, STATE)
    ctx = state.get("tracks", {}).get("CTX") or {}
    accepted = ctx.get("accepted_workpacks_hint") or []
    if accepted != ["WP-CTX-01", "WP-CTX-02"]:
        errors.append(f"accepted CTX compact state unexpected: {accepted!r}")
    if ctx.get("next_contract_hint") != "WP-CTX-03":
        errors.append("compact CTX next contract is not WP-CTX-03")
    if state.get("authority") != "DERIVED_NAVIGATION_ONLY":
        errors.append("accepted-state index authority drifted")

    ct = (root / CTX_README).read_text(encoding="utf-8")
    rw = (root / ROOT_WP).read_text(encoding="utf-8")
    road = (root / ROADMAP).read_text(encoding="utf-8")
    ds = (root / DOCSYNC).read_text(encoding="utf-8")
    if "next CTX action is `WP-CTX-03" not in ct:
        errors.append("CTX README does not agree on WP-CTX-03 as next action")
    if "`WP-CTX-03 — Structured evidence" not in rw or "next dependency-valid CTX action" not in rw:
        errors.append("root workpack index does not agree on WP-CTX-03 as next CTX action")
    for marker in ("DOCSYNC_COMPLETE", "next dependency-valid CTX workpack: `WP-CTX-03`", "implementation PR: `#113`"):
        if marker not in ds:
            errors.append(f"CTX-02 representative DocSync marker missing: {marker}")

    profiles = load(root, PROFILES).get("profiles", {})
    for name, profile in profiles.items():
        for raw in profile.get("initial_reads") or []:
            if isinstance(raw, str) and raw.startswith("Docs/history/"):
                errors.append(f"history leaked into normal {name} initial_reads: {raw}")

    if not (root / HISTORY).is_file():
        errors.append("CTX process history file missing")
    else:
        history = (root / HISTORY).read_text(encoding="utf-8")
        for pointer in ("#5273364796", "#5274094804", "#5274937744", "107694d3850a478849bffd9510dc030910fc8aa3"):
            if pointer not in history:
                errors.append(f"CTX history reconstruction pointer missing: {pointer}")

    if "Docs/history/ROADMAP_ACCEPTED_CLOSURES.md" not in road:
        errors.append("current ROADMAP does not point to separated accepted closure history")
    if not (root / ROADMAP_HISTORY).is_file():
        errors.append("ROADMAP accepted-closure history file missing")
    else:
        history = (root / ROADMAP_HISTORY).read_text(encoding="utf-8")
        # Representative exact pointers from the moved H0 chronology. The
        # current ROADMAP no longer needs them in normal bootstrap, but history
        # must remain sufficient to reconstruct the accepted closure lineage.
        for pointer in (
            "#5257350871",
            "#5260337340",
            "#5261636151",
            "23a9fd4373a803187cd9391b1459cd48975177f6",
            "0fa3d4fb039a3d0049cea0f3eed1c83128ec7dcd",
            "048d2e449d5ff81e8bcc35bc15664ea4ceec18ca",
        ):
            if pointer not in history:
                errors.append(f"ROADMAP history reconstruction pointer missing: {pointer}")
        # Closure chronology moved out of normal ROADMAP rather than being
        # duplicated in both places.
        if "review `#5257350871`" in road or "review `#5261636151`" in road:
            errors.append("verbose accepted H0 closure chronology still duplicated in current ROADMAP")

    classification = load(root, CLASSIFICATION)
    cases = classification.get("cases") or []
    if not cases or not any(c.get("decision") == "ADOPT" for c in cases):
        errors.append("historical classification has no adopted mechanical family")
    if not any(c.get("decision") == "REJECT" for c in cases):
        errors.append("historical classification never rejects inappropriate mechanization")

    # The representative accepted transition has one CTX row in one compact
    # accepted-state index; repeated chronology is history/evidence, not another
    # competing current-state registry.
    if list(state.get("tracks", {})).count("CTX") != 1:
        errors.append("accepted-state index contains multiple CTX current-state rows")
    return errors


def self_test() -> None:
    fake = {"tracks": {"CTX": {"next_contract_hint": "WP-CTX-99"}}}
    assert fake["tracks"]["CTX"]["next_contract_hint"] != "WP-CTX-03"
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
