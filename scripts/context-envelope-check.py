#!/usr/bin/env python3
"""CTX-03 context measurement, process-envelope and escalation validator."""
from __future__ import annotations

import argparse, json, math, subprocess, sys
from pathlib import Path

DEFAULT_CONFIG = Path("Docs/engineering/context-envelope.json")
CAPSULE_DIR = Path("Docs/engineering/context-capsules")
CAPSULE_INDEX = CAPSULE_DIR / "index.json"
CAPSULE_PROTOCOL = Path("Docs/engineering/CONTEXT_CAPSULE_V1.md")

# The representative measurement universe is checker-owned. The config repeats
# these values as reviewed assertions/documentation; it cannot choose a friendlier
# task/source universe after seeing the result.
CANONICAL_ROUTE_CONFIGS = [
    {"id": "H1-worker", "profile": "worker", "exact_wp": "Docs/workpacks/H1/WP-H1-02.md", "capsules": ["WP-HK-GATE"], "held_constant_sources": ["Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md", "Docs/engineering/H1_REMOTE_LOCAL_EXECUTION.md"]},
    {"id": "H1-reviewer", "profile": "reviewer", "exact_wp": "Docs/workpacks/H1/WP-H1-02.md", "capsules": ["WP-HK-GATE"], "held_constant_sources": ["Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md", "Docs/engineering/H1_REMOTE_LOCAL_EXECUTION.md"]},
    {"id": "CITY-worker", "profile": "worker", "exact_wp": "Docs/workpacks/CITY/WP-CITY-04.md", "capsules": ["WP-CITY-03"], "post_required_additions": ["Docs/ROADMAP.md"]},
    {"id": "CITY-reviewer", "profile": "reviewer", "exact_wp": "Docs/workpacks/CITY/WP-CITY-04.md", "capsules": ["WP-CITY-03"], "post_required_additions": ["Docs/ROADMAP.md"]},
    {"id": "PA-worker", "profile": "worker", "exact_wp": "Docs/workpacks/PA/WP-PA-04.md", "capsules": ["WP-PA-01", "WP-PA-02", "WP-PA-03"]},
    {"id": "PA-reviewer", "profile": "reviewer", "exact_wp": "Docs/workpacks/PA/WP-PA-04.md", "capsules": ["WP-PA-01", "WP-PA-02", "WP-PA-03"]},
]

# Pre-CTX direct-predecessor reconstruction is also checker-owned. Crucially it
# is NOT derived from the compact capsule being measured; otherwise deleting a
# compact source could shrink both baseline and post route and manufacture a
# false saving.
PRE_CTX_DEPENDENCY_SOURCES = {
    "H1-worker": {
        "Docs/workpacks/HK/WP-HK-GATE.md",
        "Docs/evidence/WP-HK-GATE/VERDICT.md",
        "Docs/evidence/WP-HK-GATE/PROOF_MATRIX.md",
        "Docs/evidence/WP-HK-GATE/RESIDUAL_RISK.md",
    },
    "H1-reviewer": {
        "Docs/workpacks/HK/WP-HK-GATE.md",
        "Docs/evidence/WP-HK-GATE/VERDICT.md",
        "Docs/evidence/WP-HK-GATE/PROOF_MATRIX.md",
        "Docs/evidence/WP-HK-GATE/RESIDUAL_RISK.md",
    },
    "CITY-worker": {
        "Docs/workpacks/CITY/WP-CITY-03.md",
        "Docs/production/CITY_PRODUCT_SEED.md",
    },
    "CITY-reviewer": {
        "Docs/workpacks/CITY/WP-CITY-03.md",
        "Docs/production/CITY_PRODUCT_SEED.md",
    },
    "PA-worker": {
        "Docs/workpacks/PA/WP-PA-01.md",
        "Docs/research/living-world/results/PA-01.md",
        "Docs/workpacks/PA/WP-PA-02.md",
        "Docs/research/living-world/results/PA-02.md",
        "Docs/workpacks/PA/WP-PA-03.md",
        "Docs/research/living-world/results/PA-03.md",
    },
    "PA-reviewer": {
        "Docs/workpacks/PA/WP-PA-01.md",
        "Docs/research/living-world/results/PA-01.md",
        "Docs/workpacks/PA/WP-PA-02.md",
        "Docs/research/living-world/results/PA-02.md",
        "Docs/workpacks/PA/WP-PA-03.md",
        "Docs/research/living-world/results/PA-03.md",
    },
}


def load(path: Path):
    return json.loads(path.read_text(encoding="utf-8"))


def estimate_paths(root: Path, paths: set[str]):
    total, rows = 0, []
    for raw in sorted(paths):
        path = root / raw
        if not path.is_file():
            raise FileNotFoundError(f"required source missing: {raw}")
        size = len(path.read_bytes())
        est = math.ceil(size / 4)
        total += est
        rows.append({"path": raw, "utf8_bytes": size, "estimate": est})
    return total, rows


def accepted_profile_sources(root: Path, cfg: dict, profile_name: str):
    profile_doc = load(root / cfg["profile_source"])
    profile = profile_doc["profiles"][profile_name]
    pcfg = cfg["process_envelope"]["profiles"][profile_name]
    subs = pcfg.get("substitutions") or {}
    concrete, dynamic = set(), []
    for item in profile.get("initial_reads") or []:
        resolved = subs.get(item, item)
        if isinstance(resolved, str) and resolved.startswith("<") and resolved.endswith(">"):
            dynamic.append(resolved)
        else:
            concrete.add(str(resolved))
    return concrete, dynamic


def source_path(row):
    if isinstance(row, str):
        return row
    if isinstance(row, dict) and isinstance(row.get("path"), str):
        return row["path"]
    return None


def capsule_sources(root: Path, capsule_id: str):
    cap_path = CAPSULE_DIR / f"{capsule_id}.json"
    cap = load(root / cap_path)
    authoritative, mandatory = set(), set()
    p = source_path(cap.get("identity_source"))
    if p:
        authoritative.add(p)
    for row in cap.get("authoritative_sources") or []:
        p = source_path(row)
        if p:
            authoritative.add(p)
    p = source_path(cap.get("disposition_source"))
    if p:
        authoritative.add(p)
    for row in cap.get("mandatory_source_reads") or []:
        p = source_path(row)
        if p:
            mandatory.add(p)
    return str(cap_path), authoritative, mandatory


def route_profile_sources(root: Path, cfg: dict, route: dict):
    profile = load(root / cfg["profile_source"])["profiles"][route["profile"]]
    concrete, dynamic = set(), []
    for item in profile.get("initial_reads") or []:
        if item in {"<EXACT_WP>", "<EXACT_H1_WP>"}:
            concrete.add(route["exact_wp"])
        elif isinstance(item, str) and item.startswith("<") and item.endswith(">"):
            dynamic.append(item)
        else:
            concrete.add(str(item))
    return concrete, dynamic


def material(pre: int, post: int, fraction: float):
    pre_u, post_u = math.ceil(pre * fraction), math.ceil(post * fraction)
    saving = pre - post
    return {
        "saving_estimate": saving,
        "saving_fraction": round(saving / pre, 6) if pre else 0,
        "pre_uncertainty": pre_u,
        "post_uncertainty": post_u,
        "material": saving > pre_u + post_u,
    }


def canonical_route_errors(cfg: dict) -> list[str]:
    actual = cfg.get("same_snapshot_measurement", {}).get("routes")
    if actual != CANONICAL_ROUTE_CONFIGS:
        return ["same_snapshot_measurement.routes must exactly equal checker-owned representative route universe"]
    return []


def profile_universe_errors(root: Path, cfg: dict) -> list[str]:
    profiles = load(root / cfg["profile_source"]).get("profiles", {})
    configured = cfg.get("process_envelope", {}).get("profiles", {})
    if set(profiles) != set(configured):
        missing = sorted(set(profiles) - set(configured))
        extra = sorted(set(configured) - set(profiles))
        return [f"process-envelope profile universe mismatch; missing={missing}, extra={extra}"]
    return []


def measure_route(root: Path, cfg: dict, route: dict):
    route_id = route["id"]
    if route_id not in PRE_CTX_DEPENDENCY_SOURCES:
        raise ValueError(f"route {route_id!r} has no checker-owned pre-CTX dependency universe")

    post, dynamic = route_profile_sources(root, cfg, route)
    auth, mandatory = set(), set()
    capsules = route.get("capsules") or []
    if capsules:
        post.update({str(CAPSULE_PROTOCOL), str(CAPSULE_INDEX)})
    for cid in capsules:
        cpath, cauth, cmandatory = capsule_sources(root, cid)
        post.add(cpath)
        auth |= cauth
        mandatory |= cmandatory
    post |= mandatory
    held = set(route.get("held_constant_sources") or [])
    post |= held
    post |= set(route.get("post_required_additions") or [])

    pre = {
        "AGENTS.md",
        "Docs/ROADMAP.md",
        route["exact_wp"],
        "Docs/engineering/WORKER_REVIEW_PROTOCOL.md",
    }
    pre |= set(PRE_CTX_DEPENDENCY_SOURCES[route_id]) | held

    pre_est, pre_rows = estimate_paths(root, pre)
    post_est, post_rows = estimate_paths(root, post)
    escalated = post | auth | set(route.get("post_escalated_additions") or [])
    esc_est, esc_rows = estimate_paths(root, escalated)
    frac = float(cfg["estimator"]["provider_token_uncertainty_fraction"])
    return {
        "id": route_id,
        "profile": route["profile"],
        "exact_wp": route["exact_wp"],
        "capsules": capsules,
        "pre_universe_owner": "checker-owned-v1",
        "dynamic_mandatory_sources_held_outside_delta": dynamic,
        "pre_ctx": {"estimate": pre_est, "files": pre_rows},
        "post_ctx_min": {"estimate": post_est, "files": post_rows, **material(pre_est, post_est, frac)},
        "post_ctx_escalated": {"estimate": esc_est, "files": esc_rows, **material(pre_est, esc_est, frac)},
    }


def ceiling_errors(current: int, pcfg: dict, headroom: float, allow_uncalibrated: bool):
    baseline, ceiling = pcfg.get("baseline_estimate"), pcfg.get("ceiling_estimate")
    if baseline is None or ceiling is None:
        return [] if allow_uncalibrated else ["uncalibrated baseline_estimate/ceiling_estimate"]
    if not isinstance(baseline, int) or baseline <= 0 or not isinstance(ceiling, int) or ceiling <= 0:
        return ["baseline_estimate and ceiling_estimate must be positive integers"]
    expected = math.ceil(baseline * (1 + headroom))
    errors = []
    if ceiling != expected:
        errors.append(f"ceiling {ceiling} != formula {expected}")
    if current > ceiling:
        errors.append(f"required estimate {current} exceeds ceiling {ceiling}")
    if not str(pcfg.get("rationale") or "").strip():
        errors.append("rationale missing")
    return errors


def git_json_at(ref: str, path: str):
    p = subprocess.run(["git", "show", f"{ref}:{path}"], capture_output=True, text=True)
    if p.returncode != 0:
        return None
    return json.loads(p.stdout)


def ceiling_change_errors(old: dict | None, new: dict):
    if old is None:
        return []
    errors = []
    op = old.get("process_envelope", {}).get("profiles", {})
    np = new.get("process_envelope", {}).get("profiles", {})
    for name, n in np.items():
        o = op.get(name)
        if not o:
            continue
        oc, nc = o.get("ceiling_estimate"), n.get("ceiling_estimate")
        if isinstance(oc, int) and isinstance(nc, int) and nc > oc:
            if not isinstance(n.get("ceiling_revision"), int) or n["ceiling_revision"] <= int(o.get("ceiling_revision", 0)):
                errors.append(f"{name}: ceiling increase requires ceiling_revision increment")
            if not str(n.get("ceiling_increase_justification") or "").strip():
                errors.append(f"{name}: ceiling increase requires explicit justification")
    return errors


def validate_escalations(root: Path, cfg: dict, path: Path):
    data = load(path)
    profiles = load(root / cfg["profile_source"])["profiles"]
    profile_name = data.get("profile")
    if profile_name not in profiles:
        return [f"unknown profile {profile_name!r}"]
    rows = data.get("evaluations")
    if not isinstance(rows, list) or not rows:
        return ["evaluations must be a non-empty list"]
    errors, seen = [], set()
    required_predicates = set(profiles[profile_name].get("must_escalate_if") or [])
    for i, row in enumerate(rows):
        if not isinstance(row, dict):
            errors.append(f"row {i}: not an object")
            continue
        pred = str(row.get("predicate") or "").strip()
        seen.add(pred)
        triggered = row.get("triggered")
        sources = row.get("authoritative_sources_opened") or []
        if not pred:
            errors.append(f"row {i}: predicate missing")
        if not isinstance(triggered, bool):
            errors.append(f"row {i}: triggered must be boolean")
        if triggered and not sources:
            errors.append(f"row {i}: triggered predicate opened no authoritative source")
        if not triggered and not str(row.get("non_material_reason") or "").strip():
            errors.append(f"row {i}: non-triggered predicate lacks non_material_reason")
        for raw in sources:
            if not (root / raw).is_file():
                errors.append(f"row {i}: authoritative source missing: {raw}")
    missing = required_predicates - seen
    if missing:
        errors.append("profile must_escalate_if predicates omitted: " + " | ".join(sorted(missing)))
    return errors


def audit(root: Path, cfg: dict, allow_uncalibrated: bool, base_ref: str | None, escalation: Path | None):
    errors, profiles = [], []
    route_errors = canonical_route_errors(cfg)
    universe_errors = profile_universe_errors(root, cfg)
    errors += route_errors + universe_errors

    headroom = float(cfg["process_envelope"]["headroom_fraction"])
    profile_doc = load(root / cfg["profile_source"])["profiles"]
    for name in profile_doc:  # accepted profile owns the universe, not config
        pcfg = cfg["process_envelope"]["profiles"].get(name)
        if pcfg is None:
            continue
        paths, dynamic = accepted_profile_sources(root, cfg, name)
        est, rows = estimate_paths(root, paths)
        perr = ceiling_errors(est, pcfg, headroom, allow_uncalibrated)
        errors += [f"{name}: {e}" for e in perr]
        profiles.append({
            "profile": name,
            "estimate": est,
            "files": rows,
            "dynamic_mandatory_sources_excluded_from_static_budget": dynamic,
            "baseline_estimate": pcfg.get("baseline_estimate"),
            "ceiling_estimate": pcfg.get("ceiling_estimate"),
            "errors": perr,
        })
    old = git_json_at(base_ref, str(DEFAULT_CONFIG)) if base_ref else None
    cerr = ceiling_change_errors(old, cfg)
    errors += cerr
    routes = [measure_route(root, cfg, r) for r in CANONICAL_ROUTE_CONFIGS]
    eerr = validate_escalations(root, cfg, escalation) if escalation else []
    errors += [f"CONTEXT_ESCALATIONS: {e}" for e in eerr]
    return {
        "schema": "arkus.context-envelope-report@1",
        "profiles": profiles,
        "routes": routes,
        "route_universe_errors": route_errors,
        "profile_universe_errors": universe_errors,
        "ceiling_change_errors": cerr,
        "escalation_errors": eerr,
        "errors": errors,
    }


def self_test():
    assert material(1000, 100, .2)["material"] is True
    assert material(1000, 700, .2)["material"] is False
    assert ceiling_errors(120, {"baseline_estimate": 100, "ceiling_estimate": 120, "rationale": "x"}, .2, False) == []
    assert ceiling_errors(121, {"baseline_estimate": 100, "ceiling_estimate": 120, "rationale": "x"}, .2, False)
    old = {"process_envelope": {"profiles": {"x": {"ceiling_estimate": 100, "ceiling_revision": 1}}}}
    new = {"process_envelope": {"profiles": {"x": {"ceiling_estimate": 101, "ceiling_revision": 1}}}}
    assert len(ceiling_change_errors(old, new)) == 2
    cfg = {"same_snapshot_measurement": {"routes": CANONICAL_ROUTE_CONFIGS}}
    assert canonical_route_errors(cfg) == []
    cfg["same_snapshot_measurement"]["routes"] = CANONICAL_ROUTE_CONFIGS[:-1]
    assert canonical_route_errors(cfg)
    assert set(PRE_CTX_DEPENDENCY_SOURCES) == {r["id"] for r in CANONICAL_ROUTE_CONFIGS}
    print("context-envelope self-test: PASS")


def main():
    p = argparse.ArgumentParser()
    p.add_argument("--repo-root", type=Path, default=Path("."))
    p.add_argument("--config", type=Path, default=DEFAULT_CONFIG)
    p.add_argument("--audit", action="store_true")
    p.add_argument("--allow-uncalibrated", action="store_true")
    p.add_argument("--base-ref")
    p.add_argument("--escalations", type=Path)
    p.add_argument("--output", type=Path)
    p.add_argument("--self-test", action="store_true")
    a = p.parse_args()
    if a.self_test:
        self_test()
        return 0
    try:
        root = a.repo_root.resolve()
        cfg = load(root / a.config)
        esc = (root / a.escalations) if a.escalations else None
        report = audit(root, cfg, a.allow_uncalibrated, a.base_ref, esc)
    except Exception as exc:
        print(f"CTX_PROCESS_ENVELOPE_OUTCOME: INFRA_ERROR\n{exc}", file=sys.stderr)
        return 23
    text = json.dumps(report, indent=2, sort_keys=True) + "\n"
    if a.output:
        (root / a.output).write_text(text, encoding="utf-8")
    print(text, end="")
    if report["errors"]:
        print("CTX_PROCESS_ENVELOPE_OUTCOME: FAIL", file=sys.stderr)
        return 20
    print("CTX_PROCESS_ENVELOPE_OUTCOME: PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
