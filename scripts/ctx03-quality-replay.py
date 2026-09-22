#!/usr/bin/env python3
"""CTX-03 routing-quality replay over the real context-envelope generator.

This proves source reachability only. It does not decide whether a model notices a
semantic defect and never substitutes for independent Reviewer judgment.
"""
from __future__ import annotations

import argparse
import importlib.util
import json
import shutil
import sys
import tempfile
from pathlib import Path

CHECKER = Path("scripts/context-envelope-check.py")
CONFIG = Path("Docs/engineering/context-envelope.json")

CASES = (
    ("H1-worker", "post_ctx_escalated", "Docs/evidence/WP-HK-GATE/PROOF_MATRIX.md"),
    ("H1-reviewer", "post_ctx_escalated", "Docs/evidence/WP-HK-GATE/PROOF_MATRIX.md"),
    ("CITY-worker", "post_ctx_min", "Docs/production/CITY_PRODUCT_SEED.md"),
    ("CITY-reviewer", "post_ctx_min", "Docs/production/CITY_PRODUCT_SEED.md"),
    ("PA-worker", "post_ctx_escalated", "Docs/research/living-world/results/PA-03.md"),
    ("PA-reviewer", "post_ctx_escalated", "Docs/research/living-world/results/PA-03.md"),
)


def load_checker(root: Path):
    spec = importlib.util.spec_from_file_location("ctx_envelope", root / CHECKER)
    if spec is None or spec.loader is None:
        raise RuntimeError("cannot load context-envelope checker")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


def source_set(route: dict, mode: str) -> set[str]:
    return {row["path"] for row in route[mode]["files"]}


def evaluate(report: dict) -> dict:
    by_id = {row["id"]: row for row in report.get("routes", [])}
    rows, errors = [], []
    for route_id, mode, source in CASES:
        route = by_id.get(route_id)
        present = bool(route) and source in source_set(route, mode)
        rows.append({"route": route_id, "mode": mode, "required_authoritative_source": source, "reachable": present})
        if not present:
            errors.append(f"{route_id}: {source} not reachable through {mode}")
    return {"schema": "arkus.ctx03-quality-replay@1", "cases": rows, "errors": errors}


def real_report(root: Path) -> dict:
    checker = load_checker(root)
    cfg = checker.load(root / CONFIG)
    return checker.audit(root, cfg, False, None, root / "Docs/evidence/CTX-03/CONTEXT_ESCALATIONS.json")


def remove_path_rows(rows, path: str):
    if not isinstance(rows, list):
        return rows
    return [row for row in rows if not (isinstance(row, dict) and row.get("path") == path)]


def integration_negative_controls(root: Path) -> list[str]:
    """Mutate the real compact routing inputs; the replay must turn RED."""
    failures: list[str] = []
    with tempfile.TemporaryDirectory() as td:
        tmp = Path(td) / "repo"
        shutil.copytree(root, tmp, ignore=shutil.ignore_patterns(".git", "bin", "obj", ".vs"))

        # Cumulative PA: remove the real PA-03 result from every capsule-derived
        # source selector used by the measurement route. The compact route must
        # no longer be able to claim source reachability.
        pa_path = tmp / "Docs/engineering/context-capsules/WP-PA-03.json"
        pa = json.loads(pa_path.read_text(encoding="utf-8"))
        target = "Docs/research/living-world/results/PA-03.md"
        pa["authoritative_sources"] = remove_path_rows(pa.get("authoritative_sources"), target)
        if isinstance(pa.get("disposition_source"), dict) and pa["disposition_source"].get("path") == target:
            pa.pop("disposition_source")
        pa_path.write_text(json.dumps(pa, indent=2) + "\n", encoding="utf-8")
        pa_eval = evaluate(real_report(tmp))
        if not any("PA-worker" in e and target in e for e in pa_eval["errors"]):
            failures.append("PA material-source removal did not turn quality replay RED")

    with tempfile.TemporaryDirectory() as td:
        tmp = Path(td) / "repo"
        shutil.copytree(root, tmp, ignore=shutil.ignore_patterns(".git", "bin", "obj", ".vs"))
        city_path = tmp / "Docs/engineering/context-capsules/WP-CITY-03.json"
        city = json.loads(city_path.read_text(encoding="utf-8"))
        target = "Docs/production/CITY_PRODUCT_SEED.md"
        city["mandatory_source_reads"] = remove_path_rows(city.get("mandatory_source_reads"), target)
        city_path.write_text(json.dumps(city, indent=2) + "\n", encoding="utf-8")
        city_eval = evaluate(real_report(tmp))
        if not any("CITY-worker" in e and target in e for e in city_eval["errors"]):
            failures.append("CITY non-compressible source removal did not turn quality replay RED")

    return failures


def self_test() -> None:
    route = lambda rid, source, mode: {"id": rid, mode: {"files": [{"path": source}]}, "post_ctx_min": {"files": []}, "post_ctx_escalated": {"files": []}}
    routes = []
    for rid, mode, source in CASES:
        r = {"id": rid, "post_ctx_min": {"files": []}, "post_ctx_escalated": {"files": []}}
        r[mode]["files"].append({"path": source})
        routes.append(r)
    assert evaluate({"routes": routes})["errors"] == []
    routes[0][CASES[0][1]]["files"] = []
    assert evaluate({"routes": routes})["errors"]
    print("ctx03-quality-replay self-test: PASS")


def main() -> int:
    p = argparse.ArgumentParser()
    p.add_argument("--repo-root", type=Path, default=Path("."))
    p.add_argument("--output", type=Path)
    p.add_argument("--self-test", action="store_true")
    p.add_argument("--negative-controls", action="store_true")
    args = p.parse_args()
    if args.self_test:
        self_test(); return 0
    root = args.repo_root.resolve()
    try:
        result = evaluate(real_report(root))
        if args.negative_controls:
            result["negative_control_errors"] = integration_negative_controls(root)
            if result["negative_control_errors"]:
                result["errors"].extend(result["negative_control_errors"])
    except Exception as exc:
        print(f"CTX03_QUALITY_REPLAY: INFRA_ERROR\n{exc}", file=sys.stderr)
        return 23
    text = json.dumps(result, indent=2, sort_keys=True) + "\n"
    if args.output:
        (root / args.output).write_text(text, encoding="utf-8")
    print(text, end="")
    if result["errors"]:
        print("CTX03_QUALITY_REPLAY: FAIL", file=sys.stderr)
        return 20
    print("CTX03_QUALITY_REPLAY: PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
