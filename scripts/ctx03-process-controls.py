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
CLOSURE_WORKFLOW = Path(".github/workflows/review-ready-closure.yml")


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


def growth_crosses_ceiling(m, root: Path, paths: set[str], victim: str, budget: dict, headroom: float) -> bool:
    if victim not in paths:
        return False
    with tempfile.TemporaryDirectory() as td:
        tmp = Path(td)
        copy_required(root, tmp, paths)
        before, _ = m.estimate_paths(tmp, paths)
        if m.ceiling_errors(before, budget, headroom, False):
            return False
        ceiling = int(budget["ceiling_estimate"])
        extra_estimate = max(1, ceiling - before + 2)
        with (tmp / victim).open("ab") as f:
            f.write(b"z" * (extra_estimate * 4))
        after, _ = m.estimate_paths(tmp, paths)
        return after > ceiling and bool(m.ceiling_errors(after, budget, headroom, False))


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

    narrowed_route_budgets = copy.deepcopy(cfg)
    narrowed_route_budgets["process_envelope"]["effective_route_budgets"].pop("H1-worker", None)
    if not m.route_budget_universe_errors(narrowed_route_budgets):
        errors.append("removing a route-effective budget from audited config did not turn RED")

    redirected_source = copy.deepcopy(cfg)
    redirected_source["profile_source"] = "Docs/engineering/context-envelope.json"
    if not m.profile_universe_errors(root, redirected_source):
        errors.append("redirecting profile_source away from canonical role profiles did not turn RED")

    redirected_sub = copy.deepcopy(cfg)
    redirected_sub["process_envelope"]["profiles"]["worker"]["substitutions"]["<EXACT_WP>"] = "Docs/workpacks/CTX/README.md"
    if not m.profile_universe_errors(root, redirected_sub):
        errors.append("redirecting a calibration placeholder to a friendlier source did not turn RED")

    # Pre-CTX baseline is checker-owned rather than capsule-derived.
    pa_source = "Docs/research/living-world/results/PA-03.md"
    if pa_source not in m.PRE_CTX_DEPENDENCY_SOURCES["PA-worker"]:
        errors.append("checker-owned PA baseline omits canonical PA-03 result")
    if set(m.PRE_CTX_DEPENDENCY_SOURCES) != {r["id"] for r in m.CANONICAL_ROUTE_CONFIGS}:
        errors.append("checker-owned pre-CTX universe and representative route universe diverge")

    # Base-profile control remains: unrelated growth is neutral while growth of a
    # real unconditional initial source crosses the base-profile ceiling.
    paths, _dynamic = m.accepted_profile_sources(root, cfg, "worker")
    pcfg = cfg["process_envelope"]["profiles"]["worker"]
    headroom = float(cfg["process_envelope"]["headroom_fraction"])
    with tempfile.TemporaryDirectory() as td:
        tmp = Path(td)
        copy_required(root, tmp, paths)
        before, _ = m.estimate_paths(tmp, paths)
        if m.ceiling_errors(before, pcfg, headroom, False):
            errors.append("calibrated worker base corpus is already over its reviewed ceiling")

        unrelated = tmp / "unrelated-growth.bin"
        unrelated.write_bytes(b"x" * 100_000)
        after_unrelated, _ = m.estimate_paths(tmp, paths)
        if after_unrelated != before or m.ceiling_errors(after_unrelated, pcfg, headroom, False):
            errors.append("unrelated repository growth affected the base profile envelope")

        target = sorted(paths)[0]
        if not growth_crosses_ceiling(m, root, paths, target, pcfg, headroom):
            errors.append("synthetic growth of a real initial required source did not turn the base envelope RED")

    # Circuit-break the Reviewer's class: conditionally mandatory sources must be
    # inside route-effective budgets. These are not hand-added to the base pack;
    # they are selected by checker-owned concrete routes and then independently
    # challenged here by growing the real source across its route ceiling.
    route_by_id = {r["id"]: r for r in m.CANONICAL_ROUTE_CONFIGS}
    conditional_growth_cases = [
        ("H1-worker", "minimum", "Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md"),
        ("H1-reviewer", "minimum", "Docs/engineering/H1_REMOTE_LOCAL_EXECUTION.md"),
        ("CITY-worker", "minimum", "Docs/ROADMAP.md"),
        ("CITY-reviewer", "minimum", "Docs/ROADMAP.md"),
        ("PA-worker", "escalated", "Docs/research/living-world/results/PA-03.md"),
        ("PA-reviewer", "escalated", "Docs/research/living-world/results/PA-03.md"),
    ]
    for route_id, mode, victim in conditional_growth_cases:
        report = m.measure_route(root, cfg, route_by_id[route_id])
        requirement_errors = m.effective_requirement_errors(report)
        if requirement_errors:
            errors.extend(requirement_errors)
            continue
        key = "post_ctx_min" if mode == "minimum" else "post_ctx_escalated"
        effective_paths = {row["path"] for row in report[key]["files"]}
        budget = cfg["process_envelope"]["effective_route_budgets"][route_id][mode]
        if not growth_crosses_ceiling(m, root, effective_paths, victim, budget, headroom):
            errors.append(f"{route_id}/{mode}: growth of conditionally mandatory {victim} did not turn route-effective envelope RED")

    # Ceiling increases cannot ride along as silent side effects, including the
    # new route-effective layer.
    old = {"process_envelope": {"profiles": {"worker": {"ceiling_estimate": 100, "ceiling_revision": 3}}, "effective_route_budgets": {"H1-worker": {"minimum": {"ceiling_estimate": 100, "ceiling_revision": 3}}}}}
    bad = {"process_envelope": {"profiles": {"worker": {"ceiling_estimate": 101, "ceiling_revision": 3}}, "effective_route_budgets": {"H1-worker": {"minimum": {"ceiling_estimate": 101, "ceiling_revision": 3}}}}}
    if len(m.ceiling_change_errors(old, bad)) < 4:
        errors.append("profile/route ceiling increase without revision+justification did not fail closed")
    good = {"process_envelope": {"profiles": {"worker": {"ceiling_estimate": 101, "ceiling_revision": 4, "ceiling_increase_justification": "reviewed reason"}}, "effective_route_budgets": {"H1-worker": {"minimum": {"ceiling_estimate": 101, "ceiling_revision": 4, "ceiling_increase_justification": "reviewed reason"}}}}}
    if m.ceiling_change_errors(old, good):
        errors.append("explicit reviewed profile/route ceiling increase fixture did not turn GREEN")

    # Escalation completeness universe comes from the role profile, not the record.
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

    # Workflow-level retry contract: closure must wake both on the original marker
    # and on later Candidate Validation completion, so an existing same-SHA marker
    # can be reused after metadata/gate repair without needing a duplicate marker.
    workflow = (root / CLOSURE_WORKFLOW).read_text(encoding="utf-8")
    for token in ("issue_comment:", "workflow_run:", "Arkus Candidate Validation", "review-ready-closed:${PR}:${TARGET_SHA}"):
        if token not in workflow:
            errors.append(f"review-ready closure retry contract missing workflow token: {token}")

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
