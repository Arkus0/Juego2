#!/usr/bin/env python3
from __future__ import annotations

import argparse
import json
import re
import sys
from pathlib import Path

POLICY = Path("Docs/engineering/context-capsule-coverage.json")
INDEX = Path("Docs/engineering/context-capsules/index.json")
WORKPACK_ROOT = Path("Docs/workpacks")
ALLOWED = {"required", "optional", "none"}
FALLBACK = "RECONSTRUCT_FROM_AUTHORITATIVE_SOURCES"
DIR_TRACK = {"HK": "H0"}

class DiscoverabilityError(ValueError):
    pass

def load(path: Path):
    return json.loads(path.read_text(encoding="utf-8"))

def track_for_dir(name: str) -> str:
    return DIR_TRACK.get(name, name)

def policy_audit(root: Path) -> dict:
    p = load(root / POLICY)
    if p.get("schema") != "arkus.context-capsule-coverage@1":
        raise DiscoverabilityError("unexpected coverage policy schema")
    if p.get("fallback_without_usable_capsule") != FALLBACK:
        raise DiscoverabilityError("safe authoritative fallback changed")
    tracks = p.get("tracks")
    if not isinstance(tracks, dict):
        raise DiscoverabilityError("tracks policy must be an object")
    discovered = []
    for child in sorted((root / WORKPACK_ROOT).iterdir()):
        if not child.is_dir():
            continue
        track = track_for_dir(child.name)
        discovered.append(track)
        spec = tracks.get(track)
        if not isinstance(spec, dict):
            raise DiscoverabilityError(f"unclassified workpack track: {track}")
        if spec.get("coverage") not in ALLOWED:
            raise DiscoverabilityError(f"{track}: coverage must be required|optional|none")
        if spec.get("workpack_directory") != child.name:
            raise DiscoverabilityError(f"{track}: workpack_directory mismatch")
    return {"policy": p, "discovered_tracks": discovered}

def navigation_decision(policy: dict, track: str, capsule_present: bool, capsule_valid: bool) -> dict:
    spec = (policy.get("tracks") or {}).get(track)
    if not isinstance(spec, dict) or spec.get("coverage") not in ALLOWED:
        raise DiscoverabilityError(f"unclassified workpack track: {track}")
    coverage = spec["coverage"]
    usable = capsule_present and capsule_valid and coverage != "none"
    if usable:
        return {"mode": "CAPSULE_NAVIGATION", "coverage_gap": False}
    return {
        "mode": FALLBACK,
        "coverage_gap": coverage == "required" and not capsule_present,
    }

def cue_texts(capsule: dict) -> list[str]:
    out: list[str] = []
    for field in ("exported_guarantees", "exclusions_nonclaims"):
        for row in capsule.get(field, []) or []:
            if isinstance(row, dict):
                for key in ("statement", "source_pointer"):
                    if isinstance(row.get(key), str): out.append(row[key])
    for field in ("reopen_conditions", "escalate_if"):
        for value in capsule.get(field, []) or []:
            if isinstance(value, str): out.append(value)
    for row in capsule.get("mandatory_source_reads", []) or []:
        if isinstance(row, dict):
            if isinstance(row.get("path"), str): out.append(row["path"])
            if isinstance(row.get("reason"), str): out.append(row["reason"])
    return [x.lower() for x in out]

def aliases(source: dict) -> set[str]:
    path = str(source.get("path") or "")
    name = Path(path).name
    stem = Path(path).stem
    kind = str(source.get("kind") or "")
    vals = {path.lower(), name.lower(), stem.lower()}
    if kind:
        cleaned = kind.lower().replace("accepted_", "").replace("canonical_", "").replace("_", " ")
        vals.add(cleaned)
    vals.discard("")
    return vals

def source_discoverable(capsule: dict, source: dict) -> tuple[bool, str]:
    path = str(source.get("path") or "")
    identity = capsule.get("identity_source") or {}
    if path and path == identity.get("path"):
        return True, "accepted-identity-source"
    for row in capsule.get("mandatory_source_reads", []) or []:
        if isinstance(row, dict) and row.get("path") == path:
            return True, "mandatory-source-read"
    cues = cue_texts(capsule)
    for alias in aliases(source):
        if len(alias) >= 4 and any(alias in cue for cue in cues):
            return True, f"observable-cue:{alias}"
    return False, "reachable-without-observable-opening-cue"

def capsule_report(capsule: dict) -> dict:
    rows, errors = [], []
    for source in capsule.get("authoritative_sources", []) or []:
        if not isinstance(source, dict):
            continue
        ok, reason = source_discoverable(capsule, source)
        path = source.get("path")
        rows.append({"path": path, "reachable": True, "discoverable": ok, "reason": reason})
        if not ok:
            errors.append(f"{capsule.get('capsule_id')}: reachable but not causally discoverable: {path}")
    return {"capsule_id": capsule.get("capsule_id"), "sources": rows, "errors": errors}

def audit(root: Path) -> dict:
    pa = policy_audit(root)
    policy = pa["policy"]
    index = load(root / INDEX)
    entries = index.get("entries") or []
    reports, errors = [], []
    indexed_by_track: dict[str, set[str]] = {}
    for entry in entries:
        capsule = load(root / entry["path"])
        track = capsule.get("track")
        if track not in policy["tracks"]:
            errors.append(f"indexed capsule uses unclassified track: {track}")
            continue
        indexed_by_track.setdefault(track, set()).add(capsule.get("capsule_id"))
        report = capsule_report(capsule)
        reports.append(report)
        errors.extend(report["errors"])
    # Required coverage is process-fail-closed, while runtime still falls back.
    for track, spec in policy["tracks"].items():
        if spec["coverage"] != "required":
            continue
        directory = root / WORKPACK_ROOT / spec["workpack_directory"]
        for wp in directory.glob("WP-*.md"):
            text = wp.read_text(encoding="utf-8")
            if re.search(r"(?mi)^Status:\s*\**COMPLETE\**\s*$", text):
                if wp.stem not in indexed_by_track.get(track, set()):
                    errors.append(f"{track}: required capsule missing for COMPLETE {wp.stem}; Reviewer fallback remains {FALLBACK}")
    return {"tracks": pa["discovered_tracks"], "capsules": reports, "errors": errors}

def self_test() -> None:
    policy = {"tracks": {"DW": {"coverage":"optional"}, "PA":{"coverage":"required"}, "CTX":{"coverage":"none"}}}
    assert navigation_decision(policy, "DW", False, False)["mode"] == FALLBACK
    assert navigation_decision(policy, "CTX", False, False)["mode"] == FALLBACK
    req = navigation_decision(policy, "PA", False, False)
    assert req["mode"] == FALLBACK and req["coverage_gap"] is True
    try: navigation_decision(policy, "NEW", False, False)
    except DiscoverabilityError: pass
    else: raise AssertionError("unclassified track stayed GREEN")
    cap = {
        "capsule_id":"WP-X-01",
        "identity_source":{"path":"Docs/workpacks/X/WP-X-01.md"},
        "authoritative_sources":[{"path":"Docs/evidence/WP-X-01/HIDDEN_BLOCKER.md","kind":"hidden_blocker"}],
        "exported_guarantees":[{"statement":"compact claim","source_pointer":"WP-X-01 / summary"}],
        "exclusions_nonclaims":[], "reopen_conditions":[], "escalate_if":[], "mandatory_source_reads":[]
    }
    assert capsule_report(cap)["errors"], "circular discoverability stayed GREEN"
    cap["escalate_if"] = ["If hidden blocker evidence becomes material, open HIDDEN_BLOCKER.md."]
    assert not capsule_report(cap)["errors"], "observable cue did not restore discoverability"
    print("CONTEXT_DISCOVERABILITY_SELF_TEST_GREEN")

def main() -> int:
    ap = argparse.ArgumentParser(); ap.add_argument("command", choices=["self-test","audit"]); ap.add_argument("--repo-root", default=".")
    args = ap.parse_args(); root = Path(args.repo_root).resolve()
    try:
        if args.command == "self-test": self_test(); return 0
        result = audit(root); print(json.dumps(result, indent=2, sort_keys=True))
        if result["errors"]: raise DiscoverabilityError("; ".join(result["errors"]))
        print("CONTEXT_DISCOVERABILITY_AUDIT_GREEN"); return 0
    except (OSError, json.JSONDecodeError, DiscoverabilityError, AssertionError) as exc:
        print(f"CONTEXT_DISCOVERABILITY_ERROR: {exc}", file=sys.stderr); return 1
if __name__ == "__main__": raise SystemExit(main())
