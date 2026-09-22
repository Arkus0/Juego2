#!/usr/bin/env python3
"""Fail-closed envelope for repository-backed dynamic mandatory context."""
from __future__ import annotations

import argparse
import json
import math
import re
import subprocess
import sys
import tempfile
from pathlib import Path

CONFIG = Path("Docs/engineering/context-envelope.json")
PROFILES = Path("Docs/engineering/context-bootstrap-profiles.json")
PLACEHOLDER_RE = re.compile(r"<([A-Z0-9_]+)>")
REPO_PATH_RE = re.compile(r"(?<![A-Za-z0-9_./-])((?:Docs|scripts|Assets|Packages|ProjectSettings|\.github)/[A-Za-z0-9_./+@-]+\.[A-Za-z0-9_-]+)")
DEPENDENCY_LINE_RE = re.compile(r"^Depends on:\s*(.+)$", re.MULTILINE | re.IGNORECASE)
WP_ID_RE = re.compile(r"\b(WP-[A-Z0-9]+(?:-[A-Z0-9]+)+)\b")

EXTERNAL = "external"
REPOSITORY = "repository"
REPOSITORY_OR_EXTERNAL = "repository_or_external"
DERIVED_DEPENDENCY_SET = "derived_dependency_set"
DERIVED_MANIFEST_SET = "derived_manifest_set"

DYNAMIC_SLOT_CLASSIFICATION = {
    "<EXACT_WP>": REPOSITORY,
    "<CANONICAL_PR_LATEST_FAIL>": EXTERNAL,
    "<ORIGINAL_WORKER_EVIDENCE>": REPOSITORY_OR_EXTERNAL,
    "<LIVE_CANONICAL_PR_AND_COMPLETE_DIFF>": EXTERNAL,
    "<DIRECT_PREDECESSOR_ACCEPTED_EVIDENCE_OR_VALIDATED_CAPSULE_NAVIGATION>": DERIVED_DEPENDENCY_SET,
    "<EXACT_MILESTONE_OR_GATE_CONTRACT>": REPOSITORY,
    "<RELEVANT_TRACK_README_OR_CONSTITUENT_WPS>": REPOSITORY,
    "<ACCEPTED_PR_REVIEW_MERGE>": EXTERNAL,
    "<EXACT_ACCEPTED_WP>": REPOSITORY,
    "<DURABLE_EXTERNAL_HANDOFF_ANCHOR>": EXTERNAL,
    "<ANCHORED_LOCAL_EXECUTION_MANIFEST>": REPOSITORY,
    "<EXACT_H1_WP>": REPOSITORY,
    "<MANIFEST_NAMED_FILES_AND_SCRIPTS>": DERIVED_MANIFEST_SET,
}


def load(path: Path):
    return json.loads(path.read_text(encoding="utf-8"))


def estimate_file(path: Path) -> int:
    return math.ceil(len(path.read_bytes()) / 4)


def profile_placeholders(profile: dict) -> set[str]:
    found: set[str] = set()
    for item in profile.get("initial_reads") or []:
        if isinstance(item, str):
            found.update(f"<{name}>" for name in PLACEHOLDER_RE.findall(item))
    return found


def slot_universe_errors(root: Path) -> list[str]:
    profiles = load(root / PROFILES).get("profiles", {})
    observed: set[str] = set()
    for profile in profiles.values():
        observed |= profile_placeholders(profile)
    known = set(DYNAMIC_SLOT_CLASSIFICATION)
    errors: list[str] = []
    unknown = sorted(observed - known)
    if unknown:
        errors.append(f"unreviewed dynamic placeholders require oracle classification: {unknown}")
    stale = sorted(known - observed)
    if stale:
        errors.append(f"checker-owned dynamic slot classification no longer exists in canonical profiles: {stale}")
    return errors


def _as_paths(value) -> list[str]:
    if isinstance(value, str): return [value]
    if isinstance(value, list) and all(isinstance(v, str) for v in value): return list(value)
    raise ValueError("repository dynamic binding must be a path string or list of path strings")


def _is_external_binding(value) -> bool:
    return isinstance(value, str) and value.startswith("external:") and len(value) > len("external:")


def extract_repo_paths(text: str) -> set[str]:
    return set(REPO_PATH_RE.findall(text))


def find_exact_contract_binding(profile_name: str, bindings: dict) -> str | None:
    preferred = {
        "worker": "<EXACT_WP>", "repair_worker": "<EXACT_WP>", "reviewer": "<EXACT_WP>",
        "planner_gate": "<EXACT_MILESTONE_OR_GATE_CONTRACT>", "docsync": "<EXACT_ACCEPTED_WP>",
        "h1_local_executor": "<EXACT_H1_WP>",
    }.get(profile_name)
    value = bindings.get(preferred) if preferred else None
    return value if isinstance(value, str) and not _is_external_binding(value) else None


def resolve_dependency_sources(root: Path, exact_contract: str | None) -> set[str]:
    if not exact_contract: return set()
    path = root / exact_contract
    if not path.is_file(): raise FileNotFoundError(f"exact contract missing: {exact_contract}")
    text = path.read_text(encoding="utf-8")
    sources = extract_repo_paths(text)
    dependency_ids: set[str] = set()
    for line in DEPENDENCY_LINE_RE.findall(text): dependency_ids |= set(WP_ID_RE.findall(line))
    for dep in dependency_ids:
        matches = list((root / "Docs/workpacks").glob(f"**/{dep}.md"))
        if len(matches) != 1: raise ValueError(f"dependency {dep} resolves to {len(matches)} workpack contracts")
        sources.add(matches[0].relative_to(root).as_posix())
        for evidence_root in (root / "Docs/evidence" / dep, root / "Docs/evidence" / dep.removeprefix("WP-")):
            if evidence_root.is_dir():
                for p in evidence_root.rglob("*"):
                    if p.is_file(): sources.add(p.relative_to(root).as_posix())
    return sources


def resolve_manifest_sources(root: Path, bindings: dict) -> set[str]:
    raw = bindings.get("<ANCHORED_LOCAL_EXECUTION_MANIFEST>")
    if not isinstance(raw, str) or _is_external_binding(raw):
        raise ValueError("manifest-derived slot requires repository <ANCHORED_LOCAL_EXECUTION_MANIFEST>")
    path = root / raw
    if not path.is_file(): raise FileNotFoundError(f"anchored local manifest missing: {raw}")
    sources = extract_repo_paths(path.read_text(encoding="utf-8"))
    sources.discard(raw)
    if not sources: raise ValueError("local execution manifest names no repository files/scripts")
    return sources


def dynamic_policy(cfg: dict) -> dict:
    policy = cfg.get("process_envelope", {}).get("dynamic_repository_envelope")
    if not isinstance(policy, dict): raise ValueError("process_envelope.dynamic_repository_envelope missing")
    required = {"per_source_ceiling_estimate", "aggregate_route_ceiling_estimate", "policy_revision", "rationale"}
    missing = sorted(required - set(policy))
    if missing: raise ValueError(f"dynamic repository envelope missing fields: {missing}")
    for key in ("per_source_ceiling_estimate", "aggregate_route_ceiling_estimate", "policy_revision"):
        if not isinstance(policy[key], int) or policy[key] <= 0: raise ValueError(f"dynamic repository envelope {key} must be positive integer")
    if policy["aggregate_route_ceiling_estimate"] < policy["per_source_ceiling_estimate"]:
        raise ValueError("aggregate dynamic ceiling cannot be lower than per-source ceiling")
    if not str(policy.get("rationale") or "").strip(): raise ValueError("dynamic repository envelope rationale missing")
    return policy


def resolve_route(root: Path, profile_name: str, bindings: dict) -> tuple[set[str], list[str]]:
    profiles = load(root / PROFILES).get("profiles", {})
    if profile_name not in profiles: raise ValueError(f"unknown profile {profile_name!r}")
    placeholders = profile_placeholders(profiles[profile_name])
    errors: list[str] = []
    sources: set[str] = set()
    for slot in sorted(placeholders):
        kind = DYNAMIC_SLOT_CLASSIFICATION.get(slot)
        if kind is None:
            errors.append(f"{profile_name}: unreviewed dynamic slot {slot}")
        elif kind == EXTERNAL:
            if not _is_external_binding(bindings.get(slot)): errors.append(f"{profile_name}: external dynamic slot {slot} lacks explicit external binding")
        elif kind == REPOSITORY:
            if slot not in bindings:
                errors.append(f"{profile_name}: repository dynamic slot {slot} is unresolved")
            else:
                try: sources.update(_as_paths(bindings[slot]))
                except ValueError as exc: errors.append(f"{profile_name}: {slot}: {exc}")
        elif kind == REPOSITORY_OR_EXTERNAL:
            value = bindings.get(slot)
            if value is None: errors.append(f"{profile_name}: hybrid dynamic slot {slot} is unresolved")
            elif not _is_external_binding(value):
                try: sources.update(_as_paths(value))
                except ValueError as exc: errors.append(f"{profile_name}: {slot}: {exc}")
        elif kind == DERIVED_DEPENDENCY_SET:
            try: sources.update(resolve_dependency_sources(root, find_exact_contract_binding(profile_name, bindings)))
            except Exception as exc: errors.append(f"{profile_name}: dependency-derived slot failed closed: {exc}")
        elif kind == DERIVED_MANIFEST_SET:
            try: sources.update(resolve_manifest_sources(root, bindings))
            except Exception as exc: errors.append(f"{profile_name}: manifest-derived slot failed closed: {exc}")

    exact = find_exact_contract_binding(profile_name, bindings)
    if exact:
        p = root / exact
        if p.is_file():
            sources.add(exact)
            sources.update(extract_repo_paths(p.read_text(encoding="utf-8")))
        else: errors.append(f"{profile_name}: exact route contract missing: {exact}")
    manifest = bindings.get("<ANCHORED_LOCAL_EXECUTION_MANIFEST>")
    if isinstance(manifest, str) and not _is_external_binding(manifest): sources.add(manifest)

    clean: set[str] = set()
    for raw in sources:
        if raw.startswith("/") or ".." in Path(raw).parts:
            errors.append(f"{profile_name}: invalid repository source path {raw!r}")
            continue
        path = root / raw
        if not path.is_file(): errors.append(f"{profile_name}: mandatory repository source missing: {raw}")
        else: clean.add(raw)
    return clean, errors


def audit_resolved_route(root: Path, cfg: dict, profile_name: str, bindings: dict) -> dict:
    policy = dynamic_policy(cfg)
    sources, errors = resolve_route(root, profile_name, bindings)
    rows, total = [], 0
    per_source = int(policy["per_source_ceiling_estimate"])
    aggregate = int(policy["aggregate_route_ceiling_estimate"])
    for raw in sorted(sources):
        est = estimate_file(root / raw); total += est; rows.append({"path":raw,"estimate":est})
        if est > per_source: errors.append(f"{profile_name}: dynamic source {raw} estimate {est} exceeds per-source ceiling {per_source}")
    if total > aggregate: errors.append(f"{profile_name}: dynamic repository aggregate {total} exceeds route ceiling {aggregate}")
    return {"profile":profile_name,"sources":rows,"aggregate_estimate":total,"per_source_ceiling_estimate":per_source,"aggregate_route_ceiling_estimate":aggregate,"errors":errors}


def git_json_at(ref: str, path: str):
    p = subprocess.run(["git","show",f"{ref}:{path}"], capture_output=True, text=True)
    if p.returncode != 0: return None
    return json.loads(p.stdout)


def policy_change_errors(old_cfg: dict | None, new_cfg: dict) -> list[str]:
    if old_cfg is None: return []
    old = old_cfg.get("process_envelope", {}).get("dynamic_repository_envelope")
    new = new_cfg.get("process_envelope", {}).get("dynamic_repository_envelope")
    if not isinstance(old, dict) or not isinstance(new, dict): return []
    increased = any(isinstance(old.get(k), int) and isinstance(new.get(k), int) and new[k] > old[k] for k in ("per_source_ceiling_estimate","aggregate_route_ceiling_estimate"))
    if not increased: return []
    errors = []
    if not isinstance(new.get("policy_revision"), int) or new["policy_revision"] <= int(old.get("policy_revision", 0)): errors.append("dynamic repository ceiling increase requires policy_revision increment")
    if not str(new.get("ceiling_increase_justification") or "").strip(): errors.append("dynamic repository ceiling increase requires explicit justification")
    return errors


def audit_policy(root: Path, cfg: dict, base_ref: str | None) -> list[str]:
    errors = slot_universe_errors(root)
    try: dynamic_policy(cfg)
    except Exception as exc: errors.append(str(exc))
    old = git_json_at(base_ref, str(CONFIG)) if base_ref else None
    errors += policy_change_errors(old, cfg)
    return errors


def _write(root: Path, rel: str, text: str) -> None:
    p = root / rel; p.parent.mkdir(parents=True, exist_ok=True); p.write_text(text, encoding="utf-8")


def _profiles_fixture() -> dict:
    return {"profiles":{"worker":{"initial_reads":["<EXACT_WP>"]},"repair_worker":{"initial_reads":["<EXACT_WP>","<CANONICAL_PR_LATEST_FAIL>","<ORIGINAL_WORKER_EVIDENCE>"]},"reviewer":{"initial_reads":["<EXACT_WP>","<LIVE_CANONICAL_PR_AND_COMPLETE_DIFF>","<DIRECT_PREDECESSOR_ACCEPTED_EVIDENCE_OR_VALIDATED_CAPSULE_NAVIGATION>"]},"planner_gate":{"initial_reads":["<EXACT_MILESTONE_OR_GATE_CONTRACT>","<RELEVANT_TRACK_README_OR_CONSTITUENT_WPS>"]},"docsync":{"initial_reads":["<ACCEPTED_PR_REVIEW_MERGE>","<EXACT_ACCEPTED_WP>"]},"h1_local_executor":{"initial_reads":["<DURABLE_EXTERNAL_HANDOFF_ANCHOR>","<ANCHORED_LOCAL_EXECUTION_MANIFEST>","<EXACT_H1_WP>","<MANIFEST_NAMED_FILES_AND_SCRIPTS>"]}}}


def self_test() -> None:
    with tempfile.TemporaryDirectory() as td:
        root = Path(td); _write(root, str(PROFILES), json.dumps(_profiles_fixture()))
        cfg = {"process_envelope":{"dynamic_repository_envelope":{"per_source_ceiling_estimate":40,"aggregate_route_ceiling_estimate":120,"policy_revision":1,"rationale":"synthetic reviewed policy"}}}
        assert slot_universe_errors(root) == []
        future_wp = "Docs/workpacks/FUTURE/WP-FUTURE-77.md"; proof = "Docs/evidence/FUTURE-77/proof.md"
        _write(root, future_wp, f"# future\nMandatory repository source: {proof}\n"); _write(root, proof, "proof\n")
        route = audit_resolved_route(root, cfg, "worker", {"<EXACT_WP>":future_wp}); paths = {r["path"] for r in route["sources"]}
        assert route["errors"] == [] and future_wp in paths and proof in paths
        _write(root, future_wp, "x"*200 + f"\nMandatory repository source: {proof}\n")
        assert any("per-source ceiling" in e for e in audit_resolved_route(root,cfg,"worker",{"<EXACT_WP>":future_wp})["errors"])
    print("ctx03-dynamic-context self-test: PASS")


def main() -> int:
    p = argparse.ArgumentParser(); p.add_argument("--repo-root", type=Path, default=Path(".")); p.add_argument("--config", type=Path, default=CONFIG); p.add_argument("--base-ref"); p.add_argument("--audit-policy", action="store_true"); p.add_argument("--profile"); p.add_argument("--bindings-json", type=Path); p.add_argument("--self-test", action="store_true"); args = p.parse_args()
    if args.self_test: self_test(); return 0
    root = args.repo_root.resolve()
    try:
        cfg = load(root / args.config); errors = audit_policy(root, cfg, args.base_ref) if args.audit_policy else []; report = None
        if args.profile:
            if not args.bindings_json: raise ValueError("--profile requires --bindings-json")
            report = audit_resolved_route(root, cfg, args.profile, load(root / args.bindings_json)); errors.extend(report["errors"])
        if not args.audit_policy and not args.profile: raise ValueError("choose --audit-policy and/or --profile")
    except Exception as exc:
        print(f"CTX03_DYNAMIC_CONTEXT: INFRA_ERROR\n{exc}", file=sys.stderr); return 23
    if report is not None: print(json.dumps(report, indent=2, sort_keys=True))
    if errors:
        print("CTX03_DYNAMIC_CONTEXT: FAIL", file=sys.stderr)
        for e in errors: print(f"- {e}", file=sys.stderr)
        return 20
    print("CTX03_DYNAMIC_CONTEXT: PASS"); return 0

if __name__ == "__main__": raise SystemExit(main())
