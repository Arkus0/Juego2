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
import re
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
RESTORE_COMMAND_RE = re.compile(r"\bdotnet\s+restore\b")


def _non_comment_restore_lines(text: str) -> list[tuple[int, str, str]]:
    """Return every non-comment line that mentions a dotnet restore command."""
    found: list[tuple[int, str, str]] = []
    for index, raw in enumerate(text.splitlines()):
        stripped = raw.strip()
        if not stripped or stripped.startswith("#"):
            continue
        if RESTORE_COMMAND_RE.search(stripped):
            found.append((index, raw, stripped))
    return found


def _shell_context_depths(lines: list[str]) -> list[int]:
    """Approximate shell control/function nesting for direct-command contract checks.

    Batch A deliberately requires restore to remain a direct standalone command. We do not try to
    interpret arbitrary Bash; instead we reject a restore that sits inside the common constructs
    that can make textual command presence non-executing or conditional.
    """
    depths: list[int] = []
    stack: list[str] = []

    for raw in lines:
        stripped = raw.strip()
        depths.append(len(stack))
        if not stripped or stripped.startswith("#"):
            continue

        if re.match(r"^(fi|done|esac)\b", stripped):
            if stack:
                stack.pop()
            continue
        if stripped in {"}", ")"}:
            if stack:
                stack.pop()
            continue

        if re.match(
            r"^(?:function\s+[A-Za-z_][A-Za-z0-9_]*|[A-Za-z_][A-Za-z0-9_]*\s*\(\))\s*\{",
            stripped,
        ):
            stack.append("function")
        elif re.match(r"^if\b", stripped):
            stack.append("if")
        elif re.match(r"^(?:for|while|until|select)\b", stripped):
            stack.append("loop")
        elif re.match(r"^case\b", stripped):
            stack.append("case")
        elif stripped == "{" or stripped.endswith("$(") or stripped == "(":
            stack.append("group")

    return depths


def _validate_local_locked_restore(text: str) -> list[str]:
    lines = text.splitlines()
    mentions = _non_comment_restore_lines(text)
    errors: list[str] = []

    direct = [
        (index, raw, stripped)
        for index, raw, stripped in mentions
        if stripped == LOCKED_RESTORE
    ]
    if len(direct) != 1:
        errors.append("local preflight must contain exactly one canonical locked restore command")
    elif _shell_context_depths(lines)[direct[0][0]] != 0 or direct[0][1] != LOCKED_RESTORE:
        errors.append("local preflight locked restore must be a direct unconditional top-level command")

    unexpected = [stripped for _, _, stripped in mentions if stripped != LOCKED_RESTORE]
    if unexpected:
        errors.append(f"local preflight contains non-canonical restore command/content: {unexpected[0]!r}")
    return errors


def _validate_workflow_locked_restore(text: str, surface: str) -> list[str]:
    """Require a direct one-line Actions `run:` step for locked restore.

    Multi-line run blocks are intentionally rejected for this critical command: a shell function,
    conditional, or other dead branch could otherwise preserve the text without executing restore.
    The dedicated step must also be unconditional and fail-closed.
    """
    lines = text.splitlines()
    mentions = _non_comment_restore_lines(text)
    expected = f"run: {LOCKED_RESTORE}"
    direct = [(index, raw, stripped) for index, raw, stripped in mentions if stripped == expected]
    errors: list[str] = []

    if len(direct) != 1:
        errors.append(f"{surface} must contain exactly one direct locked-restore run step")
    unexpected = [stripped for _, _, stripped in mentions if stripped != expected]
    if unexpected:
        errors.append(f"{surface} contains non-canonical restore command/content: {unexpected[0]!r}")

    if len(direct) != 1:
        return errors

    target_index, target_raw, _ = direct[0]
    target_indent = len(target_raw) - len(target_raw.lstrip(" "))

    step_start = None
    step_indent = None
    for index in range(target_index - 1, -1, -1):
        raw = lines[index]
        stripped = raw.strip()
        if not stripped or stripped.startswith("#"):
            continue
        indent = len(raw) - len(raw.lstrip(" "))
        if indent < target_indent and stripped.startswith("- "):
            step_start = index
            step_indent = indent
            break
        if indent < target_indent - 2:
            break

    if step_start is None or step_indent is None:
        errors.append(f"{surface} locked restore is not inside a normal workflow step")
        return errors

    step_end = len(lines)
    for index in range(step_start + 1, len(lines)):
        raw = lines[index]
        stripped = raw.strip()
        if not stripped or stripped.startswith("#"):
            continue
        indent = len(raw) - len(raw.lstrip(" "))
        if indent < step_indent or (indent == step_indent and stripped.startswith("- ")):
            step_end = index
            break

    property_indent = step_indent + 2
    for raw in lines[step_start + 1 : step_end]:
        stripped = raw.strip()
        if not stripped or stripped.startswith("#"):
            continue
        indent = len(raw) - len(raw.lstrip(" "))
        if indent != property_indent:
            continue
        if stripped.startswith("if:"):
            errors.append(f"{surface} locked-restore step must not be conditional")
        if re.match(r"^continue-on-error\s*:\s*true\s*$", stripped, re.IGNORECASE):
            errors.append(f"{surface} locked-restore step must fail closed")

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
    errors.extend(_validate_local_locked_restore(local))

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
    errors.extend(_validate_workflow_locked_restore(workflow, "delegated workflow"))
    errors.extend(_validate_workflow_locked_restore(main_safety, "Main Safety"))

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
jobs:
  exact-candidate:
    steps:
      - name: Restore dependencies (locked)
        run: dotnet restore Juego2.sln --locked-mode
      - name: Build
        run: dotnet build Juego2.sln --no-restore -c Release
      - name: Test
        run: dotnet test Juego2.sln --no-build --no-restore -c Release
Simulate chat Worker without resolvable exact SDK
SIMULATED_CHAT_WORKER_DELEGATION_GREEN
gh api \"repos/${PREFLIGHT_REPOSITORY}/pulls/${PREFLIGHT_PR}\"
WORKER_PREFLIGHT_DELEGATED_GREEN
Workflow run ID:
""",
        "main_safety": """jobs:
  build-test:
    steps:
      - name: Restore dependencies (locked)
        run: dotnet restore Juego2.sln --locked-mode
""",
        "implement": "WORKER_PREFLIGHT_GREEN WORKER_PREFLIGHT_DELEGATED_GREEN grandfathered",
        "repair": "WORKER_PREFLIGHT_GREEN WORKER_PREFLIGHT_DELEGATED_GREEN grandfathered",
        "protocol": "WORKER_PREFLIGHT_GREEN WORKER_PREFLIGHT_DELEGATED_GREEN grandfathered",
        "plan": "WORKER_PREFLIGHT_GREEN WORKER_PREFLIGHT_DELEGATED_GREEN grandfathered",
    }
    assert validate_texts(base) == [], validate_texts(base)

    mutations = [
        ("local", "WORKER_PREFLIGHT_DELEGATION_REQUIRED", ""),
        ("local", LOCKED_RESTORE, "dotnet restore Juego2.sln"),
        ("workflow", "ref: ${{ github.event.pull_request.head.sha }}", "ref: main"),
        ("workflow", f"run: {LOCKED_RESTORE}", "run: dotnet restore Juego2.sln"),
        ("main_safety", f"run: {LOCKED_RESTORE}", "run: dotnet restore Juego2.sln"),
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

    local_decoys = (
        f"""locked_restore() {{
  {LOCKED_RESTORE}
}}
dotnet restore ./Juego2.sln
""",
        f"""if false; then
  {LOCKED_RESTORE}
fi
""",
        f"""echo '{LOCKED_RESTORE}'
dotnet restore ./Juego2.sln
""",
        f"""{LOCKED_RESTORE}
dotnet restore ./Juego2.sln
""",
    )
    for decoy in local_decoys:
        broken = dict(base)
        broken["local"] = broken["local"].replace(LOCKED_RESTORE, decoy.rstrip())
        if not validate_texts(broken):
            raise AssertionError("self-test accepted non-executing/non-canonical local restore decoy")

    for name in ("workflow", "main_safety"):
        workflow_decoys = (
            f"""run: |
          locked_restore() {{
            {LOCKED_RESTORE}
          }}
          dotnet restore ./Juego2.sln""",
            f"""run: |
          if false; then
            {LOCKED_RESTORE}
          fi""",
            f"""run: echo '{LOCKED_RESTORE}'""",
        )
        for decoy in workflow_decoys:
            broken = dict(base)
            broken[name] = broken[name].replace(f"run: {LOCKED_RESTORE}", decoy)
            if not validate_texts(broken):
                raise AssertionError(f"self-test accepted non-executing restore decoy in {name}")

        for extra in (
            "        if: github.event_name == 'push'\n",
            "        continue-on-error: true\n",
        ):
            broken = dict(base)
            broken[name] = broken[name].replace(
                f"        run: {LOCKED_RESTORE}\n",
                f"{extra}        run: {LOCKED_RESTORE}\n",
            )
            if not validate_texts(broken):
                raise AssertionError(f"self-test accepted conditional/non-blocking restore in {name}")

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
