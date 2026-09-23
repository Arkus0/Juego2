#!/usr/bin/env python3
"""Regression guard for the simplified Worker preflight contract.

The normal hosted path is Arkus Main Safety on the exact PR SHA. A dedicated
Worker Candidate Preflight workflow remains available only as an explicit
workflow_dispatch fallback. Ordinary PR activity must not launch both.
"""
from pathlib import Path
import sys

SURFACES = {
    "local": Path("scripts/worker-preflight.sh"),
    "manual": Path(".github/workflows/worker-preflight.yml"),
    "main_safety": Path(".github/workflows/main-safety.yml"),
    "implement": Path(".agents/skills/implement-workpack/SKILL.md"),
    "repair": Path(".agents/skills/repair-workpack/SKILL.md"),
    "amendment": Path("Docs/engineering/PRODUCT_SHA_CLOSURE.md"),
}

LOCKED = "dotnet restore Juego2.sln --locked-mode"
BUILD = "dotnet build Juego2.sln --no-restore -c Release"
TEST = "dotnet test Juego2.sln --no-build --no-restore -c Release"


def validate(texts: dict[str, str]) -> list[str]:
    errors: list[str] = []
    local = texts["local"]
    manual = texts["manual"]
    safety = texts["main_safety"]

    for token in ("WORKER_PREFLIGHT_GREEN", "WORKER_PREFLIGHT_DELEGATION_REQUIRED", LOCKED):
        if token not in local:
            errors.append(f"local preflight missing {token}")

    # Main Safety is the normal hosted execution substrate.
    for token in ("pull_request:", LOCKED, BUILD, TEST):
        if token not in safety:
            errors.append(f"Main Safety missing {token}")

    # The dedicated runner must be explicit-only, never automatic PR fan-out.
    for token in ("workflow_dispatch:", "pr_number:", "candidate_sha:", LOCKED, BUILD, TEST, "WORKER_PREFLIGHT_DELEGATED_GREEN"):
        if token not in manual:
            errors.append(f"manual Worker preflight missing {token}")
    on_block = manual.split("permissions:", 1)[0]
    if "pull_request:" in on_block:
        errors.append("dedicated Worker preflight must not trigger automatically on pull_request")

    for name in ("implement", "repair", "amendment"):
        text = texts[name]
        if "Main Safety" not in text:
            errors.append(f"{name} does not name Main Safety as the normal hosted preflight")
        if "PRODUCT_SHA" not in text:
            errors.append(f"{name} does not preserve PRODUCT_SHA identity")

    return errors


def main() -> int:
    self_test = "--self-test" in sys.argv
    texts: dict[str, str] = {}
    for name, path in SURFACES.items():
        if not path.is_file():
            print(f"error: missing {path}", file=sys.stderr)
            return 2
        texts[name] = path.read_text(encoding="utf-8")

    errors = validate(texts)
    if errors:
        for error in errors:
            print(f"error: {error}", file=sys.stderr)
        return 1

    if self_test:
        broken = dict(texts)
        broken["manual"] = broken["manual"].replace("workflow_dispatch:", "pull_request:", 1)
        if not validate(broken):
            print("error: self-test failed to reject automatic dedicated preflight", file=sys.stderr)
            return 1
        print("WORKER_PREFLIGHT_CONTRACT_SELF_TEST_GREEN")
    else:
        print("WORKER_PREFLIGHT_CONTRACT_GREEN")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
