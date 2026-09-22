#!/usr/bin/env python3
"""Repository-wide dynamic mandatory-context discovery for CTX-03."""
from __future__ import annotations

import argparse
import importlib.util
import re
import sys
from pathlib import Path

TARGET = Path("scripts/ctx03-dynamic-context-check.py")
CONFIG = Path("Docs/engineering/context-envelope.json")
MANDATORY_CUES = ("mandatory", "required input", "required source", "required evidence", "must read", "must open", "requires:", "depends on:")
MANDATORY_HEADINGS = ("required inputs", "required evidence", "dependencies", "dependency", "mandatory context", "mandatory sources")
HEADING_RE = re.compile(r"^(#{1,6})\s+(.+?)\s*$")


def module(root: Path):
    spec = importlib.util.spec_from_file_location("ctx03_dynamic_policy_target", root / TARGET)
    if spec is None or spec.loader is None:
        raise RuntimeError("cannot load dynamic context checker")
    m = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(m)
    return m


def mandatory_existing_refs(m, root: Path, text: str) -> set[str]:
    found: set[str] = set()
    mandatory_level: int | None = None
    for line in text.splitlines():
        heading = HEADING_RE.match(line)
        if heading:
            level = len(heading.group(1))
            title = heading.group(2).strip().lower()
            if mandatory_level is not None and level <= mandatory_level:
                mandatory_level = None
            if any(key in title for key in MANDATORY_HEADINGS):
                mandatory_level = level
        lower = line.lower()
        if mandatory_level is None and not any(cue in lower for cue in MANDATORY_CUES):
            continue
        for raw in m.extract_repo_paths(line):
            if (root / raw).is_file():
                found.add(raw)
    return found


def dependency_sources(m, root: Path, text: str) -> set[str]:
    sources: set[str] = set()
    dependency_ids: set[str] = set()
    for line in m.DEPENDENCY_LINE_RE.findall(text):
        dependency_ids |= set(m.WP_ID_RE.findall(line))
    for dep in dependency_ids:
        dep_contract = m.resolve_wp_contract(root, dep)
        sources.add(dep_contract)
        for evidence_root in (root / "Docs/evidence" / dep, root / "Docs/evidence" / dep.removeprefix("WP-")):
            if not evidence_root.is_dir():
                continue
            for name in ("VERDICT.md", "PROOF_MATRIX.md", "RESIDUAL_RISK.md", "INVARIANTS.md", "RESULT.md", "DOCSYNC.md"):
                p = evidence_root / name
                if p.is_file():
                    sources.add(p.relative_to(root).as_posix())
    return sources


def discovered_routes(m, root: Path) -> list[tuple[str, set[str]]]:
    routes: list[tuple[str, set[str]]] = []
    for wp in sorted((root / "Docs/workpacks").glob("**/WP-*.md")):
        if not wp.is_file():
            continue
        rel = wp.relative_to(root).as_posix()
        text = wp.read_text(encoding="utf-8")
        sources = {rel} | mandatory_existing_refs(m, root, text) | dependency_sources(m, root, text)
        routes.append((f"workpack:{rel}", sources))

    evidence = root / "Docs/evidence"
    if evidence.is_dir():
        for manifest in evidence.rglob("*"):
            if not manifest.is_file() or manifest.suffix.lower() not in {".json", ".md", ".yml", ".yaml"}:
                continue
            if not any(marker in manifest.name.lower() for marker in m.MANIFEST_NAME_MARKERS):
                continue
            try:
                text = manifest.read_text(encoding="utf-8")
            except UnicodeDecodeError:
                continue
            refs = {raw for raw in m.extract_repo_paths(text) if (root / raw).is_file()}
            if refs:
                rel = manifest.relative_to(root).as_posix()
                routes.append((f"manifest:{rel}", {rel} | refs))
    return routes


def audit(root: Path, base_ref: str | None) -> list[str]:
    m = module(root)
    cfg = m.load(root / CONFIG)
    errors = m.slot_universe_errors(root)
    policy = m.dynamic_policy(cfg)
    old = m.git_json_at(base_ref, str(CONFIG)) if base_ref else None
    errors += m.policy_change_errors(old, cfg)
    routes = discovered_routes(m, root)
    if not routes:
        return errors + ["repository-wide dynamic discovery found no workpack routes"]
    for label, sources in routes:
        path_errors: list[str] = []
        clean = m._validate_repo_paths(root, label, sources, path_errors)
        errors += path_errors
        _rows, budget_errors = m.budget_sources(root, policy, label, clean)
        errors += budget_errors
    return errors


def self_test() -> None:
    assert "mandatory" in MANDATORY_CUES
    print("ctx03-dynamic-universe self-test: PASS")


def main() -> int:
    p = argparse.ArgumentParser()
    p.add_argument("--repo-root", type=Path, default=Path("."))
    p.add_argument("--base-ref")
    p.add_argument("--self-test", action="store_true")
    args = p.parse_args()
    if args.self_test:
        self_test(); return 0
    try:
        errors = audit(args.repo_root.resolve(), args.base_ref)
    except Exception as exc:
        print(f"CTX03_DYNAMIC_UNIVERSE: INFRA_ERROR\n{exc}", file=sys.stderr)
        return 23
    if errors:
        print("CTX03_DYNAMIC_UNIVERSE: FAIL", file=sys.stderr)
        for error in errors:
            print(f"- {error}", file=sys.stderr)
        return 20
    print("CTX03_DYNAMIC_UNIVERSE: PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
