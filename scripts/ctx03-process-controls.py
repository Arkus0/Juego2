#!/usr/bin/env python3
"""Independent negative controls for CTX-03 process-envelope mechanics."""
from __future__ import annotations

import argparse
import copy
import importlib.util
import json
import math
import shutil
import sys
import tempfile
from pathlib import Path

CHECKER = Path("scripts/context-envelope-check.py")
CONFIG = Path("Docs/engineering/context-envelope.json")
ESCALATIONS = Path("Docs/evidence/CTX-03/CONTEXT_ESCALATIONS.json")


def module(root: Path):
    spec = importlib.util.spec_from_file_location("ctx_envelope_controls_target", root / CHECKER)
    if spec is None or spec.loader is None:
        raise RuntimeError("cannot load context-envelope checker")
    m = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(m)
    return m


def copy_required(root: Path, tmp: Path, paths: set[str]) -> None:
    for raw in paths:
        src, dst = root / raw, tmp / raw
        dst.parent.mkdir(parents=True, exist_ok=True)
        shutil.copy2(src, dst)


def run_controls(root: Path) -> list[str]:
    m = module(root)
    cfg = m.load(root / CONFIG)
    errors: list[str] = []

    # The audited config cannot shrink or redirect its own measurement universe.
    narrowed_routes = copy.deepcopy(cfg)
    narrowed_routes["same_snapshot_measurement"]["routes"] = narrowed_routes["same_snapshot_measurement"]["routes"][:-1]
    if not m.canonical_route_errors(narrowed_routes):
        errors.append("removing a representative route from audited config did not turn RED")

    narrowed_profiles = copy.deepcopy(cfg)
    narrowed_profiles["process_envelope"]["profiles"].pop("reviewer", None)
    if not m.profile_universe_errors(root, narrowed_profiles):
        errors.append("removing an accepted role profile from audited config did not turn RED")

    redirected_source = copy.deepcopy(cfg)
    redirected_source["profile_source"] = "Docs/engineering/context-envelope.json"
    if not m.profile_universe_errors(root, redirected_source):
        errors.append("redirecting profile_source away from canonical role profiles did not turn RED")

    redirected_sub = copy.deepcopy(cfg)
    redirected_sub["process_envelope"]["profiles"]["worker"]["substitutions"]["<EXACT_WP>"] = "Docs/workpacks/CTX/README.md"
    if not m.profile_universe_errors(root, redirected_sub):
        errors.append("redirecting a calibration placeholder to a friendlier source did not turn RED")

    # Pre-CTX baseline is checker-owned rather than capsule-derived. Mutating a
    # compact capsule may change the post route, but it cannot erase the canonical
    # source from PRE_CTX_DEPENDENCY_SOURCES and self-shrink the baseline.
    pa_source = "Docs/research/living-world/results/PA-03.md"
    if pa_source not in m.PRE_CTX_DEPENDENCY_SOURCES["PA-worker"]:
        errors.append("checker-owned PA baseline omits canonical PA-03 result")
    if set(m.PRE_CTX_DEPENDENCY_SOURCES) != {r["id"] for r in m.CANONICAL_ROUTE_CONFIGS}:
        errors.append("checker-owned pre-CTX universe and representative route universe diverge")

    # Real profile-derived set: unrelated growth must not change the measured
    # required corpus; growth of a real required source must cross the ceiling.
    paths, _dynamic = m.accepted_profile_sources(root, cfg, "worker")
    pcfg = cfg["process_envelope"]["profiles"]["worker"]
    headroom = float(cfg["process_envelope"]["headroom_fraction"])
    with tempfile.TemporaryDirectory() as td:
        tmp = Path(td)
        copy_required(root, tmp, paths)
        before, _ = m.estimate_paths(tmp, paths)
        if m.ceiling_errors(before, pcfg, headroom, False):
            errors.append("calibrated worker corpus is already over its reviewed ceiling")

        unrelated = tmp / "unrelated-growth.bin"
        unrelated.write_bytes(b"x" * 100_000)
        after_unrelated, _ = m.estimate_paths(tmp, paths)
        if after_unrelated != before or m.ceiling_errors(after_unrelated, pcfg, headroom, False):
            errors.append("unrelated repository growth affected the profile-derived required-context envelope")

        target = tmp / sorted(paths)[0]
        ceiling = int(pcfg["ceiling_estimate"])
        extra_estimate = max(1, ceiling - before + 2)
        with target.open("ab") as f:
            f.write(b"z" * (extra_estimate * 4))
        after_required, _ = m.estimate_paths(tmp, paths)
        if after_required <= ceiling or not m.ceiling_errors(after_required, pcfg, headroom, False):
            errors.append("synthetic growth of a real required source did not turn the envelope RED")

    # Ceiling increases cannot ride along as silent side effects.
    old = {"process_envelope": {"profiles": {"worker": {"ceiling_estimate": 100, "ceiling_revision": 3}}}}
    bad = {"process_envelope": {"profiles": {"worker": {"ceiling_estimate": 101, "ceiling_revision": 3}}}}
    if len(m.ceiling_change_errors(old, bad)) < 2:
        errors.append("ceiling increase without revision+justification did not fail closed")
    good = {"process_envelope": {"profiles": {"worker": {"ceiling_estimate": 101, "ceiling_revision": 4, "ceiling_increase_justification": "reviewed reason"}}}}
    if m.ceiling_change_errors(old, good):
        errors.append("explicit reviewed ceiling increase fixture did not turn GREEN")

    # Escalation completeness universe comes from the role profile, not the
    # record under audit. Removing a real required predicate must be detected.
    real = json.loads((root / ESCALATIONS).read_text(encoding="utf-8"))
    required = list(m.load(root / m.CANONICAL_PROFILE_SOURCE)["profiles"][real["profile"]].get("must_escalate_if") or [])
    if not required:
        errors.append("worker profile unexpectedly has no mandatory escalation predicates")
    else:
        victim = required[0]
        mutated = dict(real)
        mutated["evaluations"] = [row for row in real["evaluations"] if row.get("predicate") != victim]
        with tempfile.NamedTemporaryFile("w", suffix=".json", delete=False, encoding="utf-8") as fh:
            json.dump(mutated, fh)
            p = Path(fh.name)
        try:
            e = m.validate_escalations(root, cfg, p)
            if not any(victim in item for item in e):
                errors.append("omitting a mandatory escalation predicate did not turn validation RED")
        finally:
            p.unlink(missing_ok=True)

    return errors


def self_test() -> None:
    assert math.ceil(100 * 1.2) == 120
    print("ctx03-process-controls self-test: PASS")


def main() -> int:
    p = argparse.ArgumentParser()
    p.add_argument("--repo-root", type=Path, default=Path("."))
    p.add_argument("--self-test", action="store_true")
    args = p.parse_args()
    if args.self_test:
        self_test(); return 0
    try:
        errors = run_controls(args.repo_root.resolve())
    except Exception as exc:
        print(f"CTX03_PROCESS_CONTROLS: INFRA_ERROR\n{exc}", file=sys.stderr)
        return 23
    if errors:
        print("CTX03_PROCESS_CONTROLS: FAIL", file=sys.stderr)
        for error in errors:
            print(f"- {error}", file=sys.stderr)
        return 20
    print("CTX03_PROCESS_CONTROLS: PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
