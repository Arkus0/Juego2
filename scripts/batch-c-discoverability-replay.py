#!/usr/bin/env python3
from __future__ import annotations

import importlib.util
import json
import shutil
import sys
import tempfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]


def load_module(name: str, path: Path):
    spec = importlib.util.spec_from_file_location(name, path)
    if spec is None or spec.loader is None:
        raise RuntimeError(f"cannot import {path}")
    mod = importlib.util.module_from_spec(spec); spec.loader.exec_module(mod); return mod

DISC = load_module("discoverability", ROOT / "scripts/context-discoverability-check.py")
QUALITY = load_module("ctx03_quality", ROOT / "scripts/ctx03-quality-replay.py")

CASES = (
    ("H1-reviewer", "post_ctx_escalated", "WP-HK-GATE", "Docs/evidence/WP-HK-GATE/PROOF_MATRIX.md"),
    ("CITY-reviewer", "post_ctx_min", "WP-CITY-03", "Docs/production/CITY_PRODUCT_SEED.md"),
    ("PA-reviewer", "post_ctx_escalated", "WP-PA-03", "Docs/research/living-world/results/PA-03.md"),
)


def source_row(capsule: dict, path: str) -> dict:
    for row in capsule.get("authoritative_sources", []) or []:
        if isinstance(row, dict) and row.get("path") == path:
            return row
    return {"path": path, "kind": "mandatory_source_read"}


def evaluate(root: Path) -> list[str]:
    errors: list[str] = []
    report = QUALITY.real_report(root)
    routes = {row["id"]: row for row in report.get("routes", [])}
    index = json.loads((root / DISC.INDEX).read_text(encoding="utf-8"))
    by_id = {row["capsule_id"]: row["path"] for row in index.get("entries", [])}
    for route_id, mode, capsule_id, source in CASES:
        route = routes.get(route_id) or {}
        reachable = source in QUALITY.source_set(route, mode) if route else False
        if not reachable:
            errors.append(f"{route_id}: material source is not reachable: {source}")
            continue
        capsule = json.loads((root / by_id[capsule_id]).read_text(encoding="utf-8"))
        discoverable, reason = DISC.source_discoverable(capsule, source_row(capsule, source))
        if not discoverable:
            errors.append(f"{route_id}: source reachable but not causally discoverable: {source} ({reason})")
    return errors


def circular_negative(root: Path) -> list[str]:
    failures: list[str] = []
    with tempfile.TemporaryDirectory() as td:
        tmp = Path(td) / "repo"
        shutil.copytree(root, tmp, ignore=shutil.ignore_patterns(".git", "bin", "obj", ".vs"))
        path = tmp / "Docs/engineering/context-capsules/WP-HK-GATE.json"
        capsule = json.loads(path.read_text(encoding="utf-8"))
        # Keep every authoritative source technically reachable. Remove only the
        # observable compact cue that tells a fresh Reviewer to open proof/residual/verdict.
        capsule["escalate_if"] = [
            "Escalate when material ambiguity remains.",
            "Escalate when live accepted identity is contradicted."
        ]
        path.write_text(json.dumps(capsule, indent=2) + "\n", encoding="utf-8")
        errors = evaluate(tmp)
        target = "Docs/evidence/WP-HK-GATE/PROOF_MATRIX.md"
        if not any("reachable but not causally discoverable" in e and target in e for e in errors):
            failures.append("circular discoverability negative stayed GREEN")
    return failures


def fallback_controls(root: Path) -> list[str]:
    policy = DISC.policy_audit(root)["policy"]
    failures: list[str] = []
    for track in ("DW", "H2"):
        got = DISC.navigation_decision(policy, track, False, False)
        if got["mode"] != DISC.FALLBACK or got["coverage_gap"]:
            failures.append(f"{track}: optional no-capsule path did not preserve authoritative fallback")
    none = DISC.navigation_decision(policy, "CTX", False, False)
    if none["mode"] != DISC.FALLBACK:
        failures.append("CTX none-coverage path did not preserve authoritative fallback")
    return failures


def main() -> int:
    errors = []
    try:
        DISC.self_test()
        errors.extend(evaluate(ROOT))
        errors.extend(circular_negative(ROOT))
        errors.extend(fallback_controls(ROOT))
    except Exception as exc:
        print(f"BATCH_C_DISCOVERABILITY_INFRA_ERROR: {exc}", file=sys.stderr); return 2
    if errors:
        for error in errors: print(f"BATCH_C_DISCOVERABILITY_ERROR: {error}", file=sys.stderr)
        return 1
    print("BATCH_C_DISCOVERABILITY_REPLAY_GREEN")
    return 0

if __name__ == "__main__": raise SystemExit(main())
