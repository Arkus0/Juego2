#!/usr/bin/env python3
"""Regression guard for Batch A Worker-preflight execution semantics.

The contract has two valid executors: local exact-SDK execution and a repository-owned
pull-request workflow used when the Worker environment lacks that SDK. This checker prevents
future edits from silently making chat/cloud Workers NOT_READY again, weakening the PR+SHA
identity of delegated evidence, or drifting preflight/Main Safety away from locked restore.
"""

from __future__ import annotations

import argparse
from pathlib import Path
import sys

SURFACES = {
    "local": Path("scripts/worker-preflight.sh"),
    "workflow": Path(".github/workflows/worker-preflight.yml"),
    "main_safety": Path(".github/workflows/main-safety.yml"),
    "implement": Path(".agents/skills/implement-workpack/SKILL.md"),
    "repair": Path(".agents/skills/repair-workpack/SKILL.md"),
    "protocol": Path("Docs/engineering/WORKER_REVIEW_PROTOCOL.md"),
    "plan": Path("Docs/engineering/OPERATIONAL_HARDENING_BATCHES.md"),
}

LOCKED_RESTORE = "dotnet restore Juego2.sln --locked-mode"
RESTORE_LITERAL = "dotnet restore Juego2.sln"


def _effective_lines(text: str) -> list[str]:
    """Return non-comment lines, normalizing one-line GitHub Actions `run:` commands."""
    effective: list[str] = []
    for raw in text.splitlines():
        stripped = raw.strip()
        if not stripped or stripped.startswith("#"):
            continue
        if stripped.startswith("run: "):
            stripped = stripped[len("run: ") :].strip()
        effective.append(stripped)
    return effective


def _validate_locked_restore(text: str, surface: str) -> list[str]:
    """Require canonical locked restore and reject executable/textual restore decoys.

    The Batch A surfaces intentionally keep restore as a direct standalone command. A comment,
    echo, wrapper, unlocked restore, or second non-canonical restore must not be able to preserve
    the literal and satisfy the regression guard.
    """
    mentions = [line for line in _effective_lines(text) if RESTORE_LITERAL in line]
    errors: list[str] = []
    if LOCKED_RESTORE not in mentions:
        errors.append(f"{surface} must execute canonical locked restore")
    unexpected = [line for line in mentions if line != LOCKED_RESTORE]
    if unexpected:
        errors.append(f"{surface} contains non-canonical restore content: {unexpected[0]!r}")
    return errors


def validate_texts(texts: dict[str, str]) -> list[str]:
    errors: list[str] = []

    local = texts.get("local", "")
    workflow = texts.get("workflow", "")
    main_safety = texts.get("main_safety", "")
    implement = texts.get("implement", "")
    repair = texts.get("repair", "")
    protocol = texts.get("protocol", "")
    plan = texts.get("plan", "")

    local_required = [
        "WORKER_PREFLIGHT_DELEGATION_REQUIRED",
        "WORKER_PREFLIGHT_GREEN",
        "dotnet build Juego2.sln",
        "dotnet test Juego2.sln",
        "validate-worker-preflight-context.py --self-test",
    ]
    for token in local_required:
        if token not in local:
            errors.append(f"local preflight missing {token!r}")
    errors.extend(_validate_locked_restore(local, "local preflight"))

    workflow_required = [
        "pull_request:",
        "worker-preflight-pr-${{ github.event.pull_request.number }}",
        "ref: ${{ github.event.pull_request.head.sha }}",
        "global-json-file: global.json",
        "validate-worker-preflight-context.py",
        "dotnet build Juego2.sln --no-restore -c Release",
        "dotnet test Juego2.sln --no-build --no-restore -c Release",
        "Simulate chat Worker without resolvable exact SDK",
        "SIMULATED_CHAT_WORKER_DELEGATION_GREEN",
        "gh api \"repos/${PREFLIGHT_REPOSITORY}/pulls/${PREFLIGHT_PR}\"",
        "WORKER_PREFLIGHT_DELEGATED_GREEN",
        "Workflow run ID:",
    ]
    for token in workflow_required:
        if token not in workflow:
            errors.append(f"delegated workflow missing {token!r}")
    errors.extend(_validate_locked_restore(workflow, "delegated workflow"))
    errors.extend(_validate_locked_restore(main_safety, "Main Safety"))

    for name, text in (
        ("implement", implement),
        ("repair", repair),
        ("protocol", protocol),
        ("plan", plan),
    ):
        if "WORKER_PREFLIGHT_DELEGATED_GREEN" not in text:
            errors.append(f"{name} surface does not admit delegated GREEN receipt")
        if "WORKER_PREFLIGHT_GREEN" not in text:
            errors.append(f"{name} surface does not preserve local GREEN path")

    if "grandfather" not in implement.lower():
        errors.append("implement surface lost grandfathering")
    if "grandfather" not in repair.lower():
        errors.append("repair surface lost grandfathering")
    if "grandfather" not in protocol.lower():
        errors.append("protocol lost grandfathering")
    if "grandfather" not in plan.lower():
        errors.append("plan lost grandfathering")

    forbidden = [
        "GitHub Actions run is not a substitute for this Worker-environment gate",
        "Missing exact SDK/tooling is `NOT_READY`; GitHub Actions may not substitute",
    ]
    for phrase in forbidden:
        for name, text in texts.items():
            if phrase in text:
                errors.append(f"{name} reintroduces local-only preflight prohibition")

    return errors


def self_test() -> None:
    base = {
        "local": """WORKER_PREFLIGHT_DELEGATION_REQUIRED
WORKER_PREFLIGHT_GREEN
dotnet restore Juego2.sln --locked-mode
dotnet build Juego2.sln
dotnet test Juego2.sln
validate-worker-preflight-context.py --self-test
""",
        "workflow": """pull_request:
worker-preflight-pr-${{ github.event.pull_request.number }}
ref: ${{ github.event.pull_request.head.sha }}
global-json-file: global.json
validate-worker-preflight-context.py
dotnet restore Juego2.sln --locked-mode
dotnet build Juego2.sln --no-restore -c Release
dotnet test Juego2.sln --no-build --no-restore -c Release
Simulate chat Worker without resolvable exact SDK
SIMULATED_CHAT_WORKER_DELEGATION_GREEN
gh api \"repos/${PREFLIGHT_REPOSITORY}/pulls/${PREFLIGHT_PR}\"
WORKER_PREFLIGHT_DELEGATED_GREEN
Workflow run ID:
""",
        "main_safety": "run: dotnet restore Juego2.sln --locked-mode\n",
        "implement": "WORKER_PREFLIGHT_GREEN WORKER_PREFLIGHT_DELEGATED_GREEN grandfathered",
        "repair": "WORKER_PREFLIGHT_GREEN WORKER_PREFLIGHT_DELEGATED_GREEN grandfathered",
        "protocol": "WORKER_PREFLIGHT_GREEN WORKER_PREFLIGHT_DELEGATED_GREEN grandfathered",
        "plan": "WORKER_PREFLIGHT_GREEN WORKER_PREFLIGHT_DELEGATED_GREEN grandfathered",
    }
    assert validate_texts(base) == []

    mutations = [
        ("local", "WORKER_PREFLIGHT_DELEGATION_REQUIRED", ""),
        ("local", LOCKED_RESTORE, RESTORE_LITERAL),
        ("workflow", "ref: ${{ github.event.pull_request.head.sha }}", "ref: main"),
        ("workflow", LOCKED_RESTORE, RESTORE_LITERAL),
        ("main_safety", LOCKED_RESTORE, RESTORE_LITERAL),
        ("workflow", "SIMULATED_CHAT_WORKER_DELEGATION_GREEN", ""),
        ("workflow", "gh api \"repos/${PREFLIGHT_REPOSITORY}/pulls/${PREFLIGHT_PR}\"", ""),
        ("protocol", "WORKER_PREFLIGHT_DELEGATED_GREEN", ""),
        ("implement", "grandfathered", ""),
    ]
    for name, old, new in mutations:
        broken = dict(base)
        broken[name] = broken[name].replace(old, new)
        if not validate_texts(broken):
            raise AssertionError(f"self-test failed to reject mutation {name}:{old}")

    for name in ("local", "workflow", "main_safety"):
        decoys = (
            f"# {LOCKED_RESTORE}\n{RESTORE_LITERAL}",
            f"echo '{LOCKED_RESTORE}'\n{RESTORE_LITERAL}",
            f"{LOCKED_RESTORE}\n{RESTORE_LITERAL}",
            f"bash -c '{RESTORE_LITERAL}'\n{LOCKED_RESTORE}",
        )
        for decoy in decoys:
            broken = dict(base)
            broken[name] = broken[name].replace(LOCKED_RESTORE, decoy)
            if not validate_texts(broken):
                raise AssertionError(f"self-test accepted non-canonical restore decoy in {name}")

    broken = dict(base)
    broken["protocol"] += " GitHub Actions run is not a substitute for this Worker-environment gate"
    if not validate_texts(broken):
        raise AssertionError("self-test failed to reject local-only protocol regression")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--self-test", action="store_true")
    args = parser.parse_args()

    if args.self_test:
        self_test()
        print("WORKER_PREFLIGHT_CONTRACT_SELF_TEST_GREEN")
        return 0

    texts: dict[str, str] = {}
    missing: list[str] = []
    for name, path in SURFACES.items():
        if not path.is_file():
            missing.append(str(path))
        else:
            texts[name] = path.read_text(encoding="utf-8")
    if missing:
        print("error: missing preflight contract surfaces: " + ", ".join(missing), file=sys.stderr)
        return 2

    errors = validate_texts(texts)
    if errors:
        for error in errors:
            print(f"error: {error}", file=sys.stderr)
        return 1

    print("WORKER_PREFLIGHT_CONTRACT_GREEN")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
