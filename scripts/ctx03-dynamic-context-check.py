#!/usr/bin/env python3
"""Canonical CTX-03 effective mandatory-read discovery and repository envelope.

There is one production discovery path: discover_effective_mandatory_read_set().
It scans every reviewed profile read surface, resolves reviewed dynamic slot classes,
and independently derives repository-backed route context from repository authority.
Callers may identify a route, but cannot make the repository universe smaller by
omitting derived bindings. Genuinely external live/API/diff inputs are classified
explicitly and remain outside the repository corpus budget.
"""
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
CAPSULE_DIR = Path("Docs/engineering/context-capsules")
CAPSULE_INDEX = CAPSULE_DIR / "index.json"
CAPSULE_PROTOCOL = Path("Docs/engineering/CONTEXT_CAPSULE_V1.md")

PLACEHOLDER_RE = re.compile(r"<([A-Z0-9_]+)>")
REPO_PATH_RE = re.compile(r"(?<![A-Za-z0-9_./-])((?:Docs|scripts|Assets|Packages|ProjectSettings|\.github)/[A-Za-z0-9_./+@-]+\.[A-Za-z0-9_-]+)")
DEPENDENCY_LINE_RE = re.compile(r"^Depends on:\s*(.+)$", re.MULTILINE | re.IGNORECASE)
WP_ID_RE = re.compile(r"\b(WP-[A-Z0-9]+(?:-[A-Z0-9]+)+)\b")
SECTION_RE = re.compile(r"^##\s+(.+?)\s*$")

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

EXACT_CONTRACT_SLOTS = (
    "<EXACT_WP>",
    "<EXACT_H1_WP>",
    "<EXACT_ACCEPTED_WP>",
    "<EXACT_MILESTONE_OR_GATE_CONTRACT>",
)

READ_SURFACE_CLASSIFICATION = {
    "initial_reads": "repository_or_dynamic",
    "conditional_reads": "repository_or_dynamic_conditional",
    "live_state": "external_descriptor_only",
}

CONDITIONAL_READ_CLASSIFICATION = {
    "foundational_claim": {"fixed"},
    "h1_02_through_gate": {"fixed"},
    "cross_track_or_unclosed_order_gate": {"fixed"},
    "accepted_contract_capsules": {"fixed", "dependency_navigation"},
    "direct_dependencies": {"dependency_navigation"},
    "predecessor_touched_or_changed": {"dependency_navigation"},
    "h1_local_evidence": {"fixed", "route_evidence"},
    "claimed_inherited_guarantee": {"dependency_navigation"},
    "foundational_plan_or_gate": {"fixed"},
    "architecture_binding": {"contract_bindings"},
    "constituent_proof": {"dependency_navigation"},
    "next_wp_or_cross_track_not_closed": {"fixed", "dependency_navigation"},
    "affected_track": {"track_context"},
    "protocol_changed": {"contract_bindings"},
    "accepted_pa_result_after_ctx02": {"fixed", "route_evidence"},
}

MANDATORY_SECTION_MARKERS = ("required input", "required source", "binding input")
MANDATORY_LINE_MARKERS = (
    "mandatory repository source",
    "required repository source",
    "dependency evidence",
    "binding proof standard",
    "execution overlay",
)
EXTERNAL_CONTEXT_MARKERS = (
    "donor ", "donor`", "external repo", "external repository",
    "upstream repo", "upstream repository", "arkus0/juego`", "arkus0/juego ",
)
ACCEPTED_EVIDENCE_NAME_MARKERS = (
    "verdict", "pass", "proof", "result", "residual", "invariant",
    "completion", "accepted", "docsync",
)
MANIFEST_NAME_MARKERS = ("manifest", "local_execution", "local-execution")


def load(path: Path):
    return json.loads(path.read_text(encoding="utf-8"))


def estimate_file(path: Path) -> int:
    return math.ceil(len(path.read_bytes()) / 4)


def _is_external_binding(value) -> bool:
    return isinstance(value, str) and value.startswith("external:") and len(value) > len("external:")


def _as_paths(value) -> list[str]:
    if isinstance(value, str):
        return [value]
    if isinstance(value, list) and value and all(isinstance(v, str) for v in value):
        return list(value)
    raise ValueError("repository dynamic binding must be a non-empty path string/list")


def extract_repo_paths(text: str) -> set[str]:
    return set(REPO_PATH_RE.findall(text))


def _line_is_external_context(line: str) -> bool:
    lower = line.lower()
    return any(marker in lower for marker in EXTERNAL_CONTEXT_MARKERS)


def mandatory_contract_paths(text: str) -> set[str]:
    sources: set[str] = set()
    current_section = ""
    for raw_line in text.splitlines():
        match = SECTION_RE.match(raw_line.strip())
        if match:
            current_section = match.group(1).strip().lower()
            continue
        line = raw_line.strip()
        if not line:
            continue
        lower = line.lower()
        section_mandatory = any(marker in current_section for marker in MANDATORY_SECTION_MARKERS)
        line_mandatory = any(marker in lower for marker in MANDATORY_LINE_MARKERS)
        if (section_mandatory or line_mandatory) and not _line_is_external_context(line):
            sources.update(extract_repo_paths(line))
    return sources


def resolve_wp_contract(root: Path, wp_id: str) -> str:
    matches = list((root / "Docs/workpacks").glob(f"**/{wp_id}.md"))
    if len(matches) != 1:
        raise ValueError(f"dependency {wp_id} resolves to {len(matches)} workpack contracts")
    return matches[0].relative_to(root).as_posix()


def direct_dependency_contracts(root: Path, text: str) -> set[str]:
    result: set[str] = set()
    for line in DEPENDENCY_LINE_RE.findall(text):
        for dep in WP_ID_RE.findall(line):
            result.add(resolve_wp_contract(root, dep))
    return result


def source_path(row):
    if isinstance(row, str):
        return row
    if isinstance(row, dict) and isinstance(row.get("path"), str):
        return row["path"]
    return None


def _wp_id_from_contract(rel: str) -> str | None:
    name = Path(rel).name
    return name[:-3] if name.startswith("WP-") and name.endswith(".md") else None


def _evidence_dirs(root: Path, wp_id: str) -> list[Path]:
    candidates = [root / "Docs/evidence" / wp_id]
    if wp_id.startswith("WP-"):
        candidates.append(root / "Docs/evidence" / wp_id[3:])
    return [p for p in candidates if p.is_dir()]


def accepted_evidence_sources(root: Path, wp_id: str) -> set[str]:
    sources: set[str] = set()
    for directory in _evidence_dirs(root, wp_id):
        for path in directory.rglob("*"):
            if not path.is_file():
                continue
            lower = path.name.lower()
            if any(marker in lower for marker in ACCEPTED_EVIDENCE_NAME_MARKERS):
                sources.add(path.relative_to(root).as_posix())
    return sources


def capsule_navigation_sources(root: Path, wp_id: str) -> set[str]:
    if not (root / CAPSULE_INDEX).is_file():
        return set()
    index = load(root / CAPSULE_INDEX)
    entries = index.get("entries") or []
    for entry in entries:
        if isinstance(entry, dict) and entry.get("capsule_id") == wp_id and isinstance(entry.get("path"), str):
            cap_rel = entry["path"]
            cap_path = root / cap_rel
            if not cap_path.is_file():
                raise FileNotFoundError(f"indexed capsule missing for {wp_id}: {cap_rel}")
            sources = {str(CAPSULE_PROTOCOL), str(CAPSULE_INDEX), cap_rel}
            cap = load(cap_path)
            for key in ("identity_source", "disposition_source"):
                p = source_path(cap.get(key))
                if p:
                    sources.add(p)
            for key in ("authoritative_sources", "mandatory_source_reads"):
                for row in cap.get(key) or []:
                    p = source_path(row)
                    if p:
                        sources.add(p)
            return sources
    return set()


def dependency_navigation_sources(root: Path, exact_contract: str | None) -> tuple[set[str], list[str]]:
    if not exact_contract:
        return set(), ["dependency navigation has no exact route contract"]
    exact_path = root / exact_contract
    if not exact_path.is_file():
        return set(), [f"exact route contract missing: {exact_contract}"]
    errors: list[str] = []
    result: set[str] = set()
    exact_text = exact_path.read_text(encoding="utf-8")
    for dep_rel in sorted(direct_dependency_contracts(root, exact_text)):
        result.add(dep_rel)
        dep_path = root / dep_rel
        dep_text = dep_path.read_text(encoding="utf-8")
        result |= mandatory_contract_paths(dep_text)
        wp_id = _wp_id_from_contract(dep_rel)
        if not wp_id:
            errors.append(f"cannot derive dependency identity from {dep_rel}")
            continue
        try:
            capsule = capsule_navigation_sources(root, wp_id)
        except Exception as exc:
            errors.append(str(exc))
            capsule = set()
        if capsule:
            result |= capsule
        else:
            evidence = accepted_evidence_sources(root, wp_id)
            if evidence:
                result |= evidence
            else:
                errors.append(f"direct dependency {wp_id} has neither validated capsule navigation nor repository accepted-evidence surface")
    return result, errors


def route_evidence_sources(root: Path, exact_contract: str | None) -> set[str]:
    if not exact_contract:
        return set()
    wp_id = _wp_id_from_contract(exact_contract)
    if not wp_id:
        return set()
    sources = accepted_evidence_sources(root, wp_id)
    try:
        sources |= capsule_navigation_sources(root, wp_id)
    except Exception:
        pass
    return sources


def track_context_sources(root: Path, exact_contract: str | None) -> set[str]:
    if not exact_contract:
        return set()
    p = Path(exact_contract)
    candidate = p.parent / "README.md"
    return {candidate.as_posix()} if (root / candidate).is_file() else set()


def resolve_manifest_sources(root: Path, bindings: dict) -> tuple[set[str], list[str]]:
    raw = bindings.get("<ANCHORED_LOCAL_EXECUTION_MANIFEST>")
    if not isinstance(raw, str) or _is_external_binding(raw):
        return set(), ["manifest-derived slot requires repository <ANCHORED_LOCAL_EXECUTION_MANIFEST>"]
    path = root / raw
    if not path.is_file():
        return set(), [f"anchored local manifest missing: {raw}"]
    sources = extract_repo_paths(path.read_text(encoding="utf-8"))
    sources.discard(raw)
    if not sources:
        return set(), ["local execution manifest names no repository files/scripts"]
    return sources, []


def _profile_read_strings(profile: dict):
    for item in profile.get("initial_reads") or []:
        if isinstance(item, str):
            yield "initial_reads", None, item
    conditionals = profile.get("conditional_reads") or {}
    if isinstance(conditionals, dict):
        for key, value in conditionals.items():
            if isinstance(value, str):
                yield "conditional_reads", str(key), value
    for item in profile.get("live_state") or []:
        if isinstance(item, str):
            yield "live_state", None, item


def profile_read_surface_errors(profile_name: str, profile: dict) -> list[str]:
    errors: list[str] = []
    for key in profile:
        readish = key.endswith("_reads") or key.startswith("read_") or key in {"live_state"}
        if readish and key not in READ_SURFACE_CLASSIFICATION:
            errors.append(f"{profile_name}: unreviewed read surface {key!r}; classify before use")
    if not isinstance(profile.get("initial_reads", []), list):
        errors.append(f"{profile_name}: initial_reads must be a list")
    if not isinstance(profile.get("conditional_reads", {}), dict):
        errors.append(f"{profile_name}: conditional_reads must be an object")
    if not isinstance(profile.get("live_state", []), list):
        errors.append(f"{profile_name}: live_state must be a list")
    for surface, _key, text in _profile_read_strings(profile):
        if surface == "live_state" and (PLACEHOLDER_RE.search(text) or extract_repo_paths(text)):
            errors.append(f"{profile_name}: live_state contains repository/dynamic read semantics pending surface classification: {text}")
    for key in (profile.get("conditional_reads") or {}):
        if key not in CONDITIONAL_READ_CLASSIFICATION:
            errors.append(f"{profile_name}: unreviewed conditional read class {key!r}")
    return errors


def profile_placeholders(profile: dict) -> set[str]:
    found: set[str] = set()
    for _surface, _key, text in _profile_read_strings(profile):
        found.update(f"<{name}>" for name in PLACEHOLDER_RE.findall(text))
    return found


def slot_universe_errors(root: Path) -> list[str]:
    profiles = load(root / PROFILES).get("profiles", {})
    if not isinstance(profiles, dict) or not profiles:
        return ["canonical profile universe is missing/empty"]
    observed: set[str] = set()
    errors: list[str] = []
    for name, profile in profiles.items():
        if not isinstance(profile, dict):
            errors.append(f"{name}: profile must be an object")
            continue
        errors += profile_read_surface_errors(name, profile)
        observed |= profile_placeholders(profile)
    known = set(DYNAMIC_SLOT_CLASSIFICATION)
    unknown = sorted(observed - known)
    stale = sorted(known - observed)
    if unknown:
        errors.append(f"unreviewed dynamic placeholders require oracle classification: {unknown}")
    if stale:
        errors.append(f"checker-owned dynamic slot classification no longer exists in canonical profiles: {stale}")
    return errors


def find_exact_contract_binding(profile_name: str, bindings: dict) -> str | None:
    candidates: list[str] = []
    for slot in EXACT_CONTRACT_SLOTS:
        value = bindings.get(slot)
        if isinstance(value, str) and not _is_external_binding(value):
            candidates.append(value)
    unique = sorted(set(candidates))
    if len(unique) > 1:
        raise ValueError(f"{profile_name}: ambiguous exact-contract bindings: {unique}")
    return unique[0] if unique else None


def _add_bound_repository_value(sources: set[str], errors: list[str], label: str, value) -> None:
    try:
        sources.update(_as_paths(value))
    except ValueError as exc:
        errors.append(f"{label}: {exc}")


def discover_effective_mandatory_read_set(root: Path, profile_name: str, bindings: dict) -> tuple[set[str], list[str], set[str]]:
    profiles = load(root / PROFILES).get("profiles", {})
    if profile_name not in profiles:
        raise ValueError(f"unknown profile {profile_name!r}")
    profile = profiles[profile_name]
    if not isinstance(profile, dict):
        raise ValueError(f"profile {profile_name!r} is not an object")

    errors = profile_read_surface_errors(profile_name, profile)
    sources: set[str] = set()
    external_slots: set[str] = set()
    try:
        exact = find_exact_contract_binding(profile_name, bindings)
    except Exception as exc:
        exact = None
        errors.append(str(exc))

    for surface, condition, text in _profile_read_strings(profile):
        if surface == "live_state":
            continue
        sources |= extract_repo_paths(text)
        if surface == "initial_reads" and not PLACEHOLDER_RE.search(text):
            candidate = root / text
            if candidate.is_file():
                sources.add(text)
        if surface == "conditional_reads" and condition in CONDITIONAL_READ_CLASSIFICATION:
            semantics = CONDITIONAL_READ_CLASSIFICATION[condition]
            if "dependency_navigation" in semantics:
                derived, derr = dependency_navigation_sources(root, exact)
                sources |= derived
                errors += [f"{profile_name}/{condition}: {e}" for e in derr]
            if "route_evidence" in semantics:
                sources |= route_evidence_sources(root, exact)
            if "contract_bindings" in semantics and exact and (root / exact).is_file():
                sources |= mandatory_contract_paths((root / exact).read_text(encoding="utf-8"))
            if "track_context" in semantics:
                sources |= track_context_sources(root, exact)

    placeholders = profile_placeholders(profile)
    for slot in sorted(placeholders):
        kind = DYNAMIC_SLOT_CLASSIFICATION.get(slot)
        if kind is None:
            errors.append(f"{profile_name}: unreviewed dynamic slot {slot}")
            continue
        value = bindings.get(slot)
        if kind == EXTERNAL:
            external_slots.add(slot)
            if not _is_external_binding(value):
                errors.append(f"{profile_name}: external dynamic slot {slot} lacks explicit external binding")
        elif kind == REPOSITORY:
            if slot in EXACT_CONTRACT_SLOTS:
                if not exact:
                    errors.append(f"{profile_name}: route identity slot {slot} is unresolved")
                else:
                    sources.add(exact)
            elif slot == "<RELEVANT_TRACK_README_OR_CONSTITUENT_WPS>":
                derived = track_context_sources(root, exact)
                dep, derr = dependency_navigation_sources(root, exact)
                sources |= derived | dep
                errors += [f"{profile_name}/{slot}: {e}" for e in derr]
                if value is not None and not _is_external_binding(value):
                    _add_bound_repository_value(sources, errors, f"{profile_name}: {slot}", value)
                if not derived and value is None:
                    errors.append(f"{profile_name}: {slot} cannot be derived from route authority")
            elif slot == "<ANCHORED_LOCAL_EXECUTION_MANIFEST>":
                if value is None:
                    errors.append(f"{profile_name}: repository dynamic slot {slot} is unresolved")
                elif _is_external_binding(value):
                    errors.append(f"{profile_name}: repository dynamic slot {slot} cannot be external")
                else:
                    _add_bound_repository_value(sources, errors, f"{profile_name}: {slot}", value)
            else:
                if value is None:
                    errors.append(f"{profile_name}: repository dynamic slot {slot} is unresolved")
                elif _is_external_binding(value):
                    errors.append(f"{profile_name}: repository dynamic slot {slot} cannot be external")
                else:
                    _add_bound_repository_value(sources, errors, f"{profile_name}: {slot}", value)
        elif kind == REPOSITORY_OR_EXTERNAL:
            if value is None:
                derived = route_evidence_sources(root, exact)
                if derived:
                    sources |= derived
                else:
                    errors.append(f"{profile_name}: hybrid dynamic slot {slot} is unresolved and no repository evidence can be derived")
            elif _is_external_binding(value):
                external_slots.add(slot)
            else:
                _add_bound_repository_value(sources, errors, f"{profile_name}: {slot}", value)
                sources |= route_evidence_sources(root, exact)
        elif kind == DERIVED_DEPENDENCY_SET:
            derived, derr = dependency_navigation_sources(root, exact)
            sources |= derived
            errors += [f"{profile_name}: dependency-derived slot failed closed: {e}" for e in derr]
            if value is not None and not _is_external_binding(value):
                _add_bound_repository_value(sources, errors, f"{profile_name}: {slot}", value)
        elif kind == DERIVED_MANIFEST_SET:
            derived, derr = resolve_manifest_sources(root, bindings)
            sources |= derived
            errors += [f"{profile_name}: manifest-derived slot failed closed: {e}" for e in derr]

    if exact:
        p = root / exact
        if p.is_file():
            text = p.read_text(encoding="utf-8")
            sources.add(exact)
            sources |= mandatory_contract_paths(text)
            sources |= direct_dependency_contracts(root, text)
        else:
            errors.append(f"{profile_name}: exact route contract missing: {exact}")

    manifest = bindings.get("<ANCHORED_LOCAL_EXECUTION_MANIFEST>")
    if isinstance(manifest, str) and not _is_external_binding(manifest):
        sources.add(manifest)

    clean = _validate_repo_paths(root, profile_name, sources, errors)
    return clean, errors, external_slots


def _validate_repo_paths(root: Path, label: str, sources: set[str], errors: list[str]) -> set[str]:
    clean: set[str] = set()
    for raw in sorted(sources):
        if raw.startswith("/") or ".." in Path(raw).parts:
            errors.append(f"{label}: invalid repository source path {raw!r}")
            continue
        path = root / raw
        if not path.is_file():
            errors.append(f"{label}: mandatory repository source missing: {raw}")
        else:
            clean.add(raw)
    return clean


def dynamic_policy(cfg: dict) -> dict:
    policy = cfg.get("process_envelope", {}).get("dynamic_repository_envelope")
    if not isinstance(policy, dict):
        raise ValueError("process_envelope.dynamic_repository_envelope missing")
    required = {"per_source_ceiling_estimate", "aggregate_route_ceiling_estimate", "policy_revision", "rationale"}
    missing = sorted(required - set(policy))
    if missing:
        raise ValueError(f"dynamic repository envelope missing fields: {missing}")
    for key in ("per_source_ceiling_estimate", "aggregate_route_ceiling_estimate", "policy_revision"):
        if not isinstance(policy[key], int) or policy[key] <= 0:
            raise ValueError(f"dynamic repository envelope {key} must be positive integer")
    if policy["aggregate_route_ceiling_estimate"] < policy["per_source_ceiling_estimate"]:
        raise ValueError("aggregate dynamic ceiling cannot be lower than per-source ceiling")
    if not str(policy.get("rationale") or "").strip():
        raise ValueError("dynamic repository envelope rationale missing")
    return policy


def budget_sources(root: Path, policy: dict, label: str, sources: set[str]) -> tuple[list[dict], list[str]]:
    rows: list[dict] = []
    errors: list[str] = []
    total = 0
    per_source = int(policy["per_source_ceiling_estimate"])
    aggregate = int(policy["aggregate_route_ceiling_estimate"])
    for raw in sorted(sources):
        est = estimate_file(root / raw)
        total += est
        rows.append({"path": raw, "estimate": est})
        if est > per_source:
            errors.append(f"{label}: dynamic source {raw} estimate {est} exceeds per-source ceiling {per_source}")
    if total > aggregate:
        errors.append(f"{label}: dynamic repository aggregate {total} exceeds route ceiling {aggregate}")
    return rows, errors


def audit_resolved_route(root: Path, cfg: dict, profile_name: str, bindings: dict) -> dict:
    policy = dynamic_policy(cfg)
    sources, errors, external = discover_effective_mandatory_read_set(root, profile_name, bindings)
    rows, budget_errors = budget_sources(root, policy, profile_name, sources)
    errors.extend(budget_errors)
    return {
        "profile": profile_name,
        "sources": rows,
        "external_slots": sorted(external),
        "aggregate_estimate": sum(r["estimate"] for r in rows),
        "per_source_ceiling_estimate": policy["per_source_ceiling_estimate"],
        "aggregate_route_ceiling_estimate": policy["aggregate_route_ceiling_estimate"],
        "errors": errors,
    }


def discover_workpack_routes(root: Path) -> list[tuple[str, set[str]]]:
    routes: list[tuple[str, set[str]]] = []
    for path in sorted((root / "Docs/workpacks").glob("**/WP-*.md")):
        if not path.is_file():
            continue
        rel = path.relative_to(root).as_posix()
        text = path.read_text(encoding="utf-8")
        sources = {rel} | mandatory_contract_paths(text)
        for dep_rel in sorted(direct_dependency_contracts(root, text)):
            sources.add(dep_rel)
            dep_text = (root / dep_rel).read_text(encoding="utf-8")
            sources |= mandatory_contract_paths(dep_text)
            wp_id = _wp_id_from_contract(dep_rel)
            if wp_id:
                try:
                    capsule = capsule_navigation_sources(root, wp_id)
                except Exception:
                    capsule = set()
                sources |= capsule or accepted_evidence_sources(root, wp_id)
        routes.append((rel, sources))
    return routes


def discover_manifest_routes(root: Path) -> list[tuple[str, set[str]]]:
    routes: list[tuple[str, set[str]]] = []
    evidence_root = root / "Docs/evidence"
    if not evidence_root.is_dir():
        return routes
    for path in evidence_root.rglob("*"):
        if not path.is_file() or path.suffix.lower() not in {".json", ".md", ".txt", ".yml", ".yaml"}:
            continue
        if not any(marker in path.name.lower() for marker in MANIFEST_NAME_MARKERS):
            continue
        try:
            text = path.read_text(encoding="utf-8")
        except UnicodeDecodeError:
            continue
        named = extract_repo_paths(text)
        if named:
            rel = path.relative_to(root).as_posix()
            routes.append((f"manifest:{rel}", {rel} | named))
    return routes


def audit_repository_dynamic_universe(root: Path, cfg: dict) -> list[str]:
    policy = dynamic_policy(cfg)
    errors: list[str] = []
    routes = discover_workpack_routes(root) + discover_manifest_routes(root)
    if not routes:
        return ["dynamic repository discovery found no workpack/manifest routes"]
    for label, sources in routes:
        clean = _validate_repo_paths(root, label, sources, errors)
        _rows, berr = budget_sources(root, policy, label, clean)
        errors.extend(berr)
    return errors


def git_json_at(ref: str, path: str):
    p = subprocess.run(["git", "show", f"{ref}:{path}"], capture_output=True, text=True)
    if p.returncode != 0:
        return None
    return json.loads(p.stdout)


def policy_change_errors(old_cfg: dict | None, new_cfg: dict) -> list[str]:
    if old_cfg is None:
        return []
    old = old_cfg.get("process_envelope", {}).get("dynamic_repository_envelope")
    new = new_cfg.get("process_envelope", {}).get("dynamic_repository_envelope")
    if not isinstance(old, dict) or not isinstance(new, dict):
        return []
    increased = any(
        isinstance(old.get(k), int) and isinstance(new.get(k), int) and new[k] > old[k]
        for k in ("per_source_ceiling_estimate", "aggregate_route_ceiling_estimate")
    )
    if not increased:
        return []
    errors: list[str] = []
    if not isinstance(new.get("policy_revision"), int) or new["policy_revision"] <= int(old.get("policy_revision", 0)):
        errors.append("dynamic repository ceiling increase requires policy_revision increment")
    if not str(new.get("ceiling_increase_justification") or "").strip():
        errors.append("dynamic repository ceiling increase requires explicit justification")
    return errors


def audit_policy(root: Path, cfg: dict, base_ref: str | None) -> list[str]:
    errors = slot_universe_errors(root)
    try:
        dynamic_policy(cfg)
        errors += audit_repository_dynamic_universe(root, cfg)
    except Exception as exc:
        errors.append(str(exc))
    old = git_json_at(base_ref, str(CONFIG)) if base_ref else None
    errors += policy_change_errors(old, cfg)
    return errors


def _write(root: Path, rel: str, text: str) -> None:
    p = root / rel
    p.parent.mkdir(parents=True, exist_ok=True)
    p.write_text(text, encoding="utf-8")


def _profiles_fixture() -> dict:
    return {"profiles": {
        "worker": {
            "initial_reads": ["AGENTS.md", "<EXACT_WP>"],
            "live_state": ["current main SHA"],
            "conditional_reads": {"direct_dependencies": "accepted predecessor sources"},
        },
        "repair_worker": {"initial_reads": ["<EXACT_WP>", "<CANONICAL_PR_LATEST_FAIL>", "<ORIGINAL_WORKER_EVIDENCE>"], "conditional_reads": {}, "live_state": []},
        "reviewer": {"initial_reads": ["<EXACT_WP>", "<LIVE_CANONICAL_PR_AND_COMPLETE_DIFF>", "<DIRECT_PREDECESSOR_ACCEPTED_EVIDENCE_OR_VALIDATED_CAPSULE_NAVIGATION>"], "conditional_reads": {}, "live_state": []},
        "planner_gate": {"initial_reads": ["<EXACT_MILESTONE_OR_GATE_CONTRACT>", "<RELEVANT_TRACK_README_OR_CONSTITUENT_WPS>"], "conditional_reads": {}, "live_state": []},
        "docsync": {"initial_reads": ["<ACCEPTED_PR_REVIEW_MERGE>", "<EXACT_ACCEPTED_WP>"], "conditional_reads": {}, "live_state": []},
        "h1_local_executor": {"initial_reads": ["<DURABLE_EXTERNAL_HANDOFF_ANCHOR>", "<ANCHORED_LOCAL_EXECUTION_MANIFEST>", "<EXACT_H1_WP>", "<MANIFEST_NAMED_FILES_AND_SCRIPTS>"], "conditional_reads": {}, "live_state": []},
    }}


def self_test() -> None:
    with tempfile.TemporaryDirectory() as td:
        root = Path(td)
        _write(root, str(PROFILES), json.dumps(_profiles_fixture()))
        cfg = {"process_envelope": {"dynamic_repository_envelope": {
            "per_source_ceiling_estimate": 40,
            "aggregate_route_ceiling_estimate": 120,
            "policy_revision": 1,
            "rationale": "synthetic reviewed policy",
        }}}
        _write(root, "AGENTS.md", "agents\n")
        dep = "Docs/workpacks/FUTURE/WP-FUTURE-01.md"
        wp = "Docs/workpacks/FUTURE/WP-FUTURE-77.md"
        proof = "Docs/evidence/FUTURE-77/proof.md"
        _write(root, dep, "# dep\n")
        _write(root, "Docs/evidence/FUTURE-01/VERDICT.md", "PASS\n")
        _write(root, proof, "proof\n")
        _write(root, wp, f"# future\nDepends on: WP-FUTURE-01\nMandatory repository source: {proof}\n")
        route = audit_resolved_route(root, cfg, "worker", {"<EXACT_WP>": wp})
        paths = {r["path"] for r in route["sources"]}
        assert route["errors"] == [] and {wp, dep, proof, "Docs/evidence/FUTURE-01/VERDICT.md"} <= paths
        data = _profiles_fixture()
        data["profiles"]["future_role"] = {"initial_reads": ["AGENTS.md"], "conditional_reads": {"direct_dependencies": "<EXACT_WP>"}, "live_state": []}
        _write(root, str(PROFILES), json.dumps(data))
        route = audit_resolved_route(root, cfg, "future_role", {"<EXACT_WP>": wp})
        assert route["errors"] == [] and wp in {r["path"] for r in route["sources"]}
    print("ctx03-dynamic-context self-test: PASS")


def main() -> int:
    p = argparse.ArgumentParser()
    p.add_argument("--repo-root", type=Path, default=Path("."))
    p.add_argument("--config", type=Path, default=CONFIG)
    p.add_argument("--base-ref")
    p.add_argument("--audit-policy", action="store_true")
    p.add_argument("--profile")
    p.add_argument("--bindings-json", type=Path)
    p.add_argument("--self-test", action="store_true")
    args = p.parse_args()
    if args.self_test:
        self_test()
        return 0
    root = args.repo_root.resolve()
    try:
        cfg = load(root / args.config)
        errors = audit_policy(root, cfg, args.base_ref) if args.audit_policy else []
        report = None
        if args.profile:
            if not args.bindings_json:
                raise ValueError("--profile requires --bindings-json")
            report = audit_resolved_route(root, cfg, args.profile, load(root / args.bindings_json))
            errors.extend(report["errors"])
        if not args.audit_policy and not args.profile:
            raise ValueError("choose --audit-policy and/or --profile")
    except Exception as exc:
        print(f"CTX03_DYNAMIC_CONTEXT: INFRA_ERROR\n{exc}", file=sys.stderr)
        return 23
    if report is not None:
        print(json.dumps(report, indent=2, sort_keys=True))
    if errors:
        print("CTX03_DYNAMIC_CONTEXT: FAIL", file=sys.stderr)
        for error in errors:
            print(f"- {error}", file=sys.stderr)
        return 20
    print("CTX03_DYNAMIC_CONTEXT: PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
