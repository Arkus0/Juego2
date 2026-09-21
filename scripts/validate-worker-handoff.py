#!/usr/bin/env python3
"""Mechanical validator for the canonical Worker -> Reviewer PR handoff.

This intentionally validates only structure/coherence that CI can establish without
making semantic Reviewer judgments. Pure PROCESS_ONLY maintenance/DocSync PRs that
never enter the Worker lifecycle are ignored; PROCESS_ONLY workpacks that publish
Worker lifecycle fields are validated normally.
"""

from __future__ import annotations

import argparse
import os
from pathlib import Path
import re
import sys
import tempfile

SHA_RE = re.compile(r"^[0-9a-f]{40}$", re.I)
PROCESS_ONLY_RE = re.compile(
    r"^(?:Mode|WORKFLOW_MODE):\s*`?PROCESS_ONLY`?\s*$", re.I | re.M
)

REQUIRED_FIELDS = (
    "WP",
    "Contract",
    "Baseline SHA",
    "Active Worker",
    "Worker state",
    "Worker history",
    "Transfer SHA",
    "Predecessor contract check",
    "Candidate HEAD SHA",
    "Worker pre-review",
    "Worker pre-review evidence",
    "Frozen candidate SHA",
    "Branch frozen",
    "Worker verdict",
    "Reviewer verdict",
    "Reviewed candidate SHA",
    "Evidence",
    "fail_cycle",
)

LIFECYCLE_SENTINELS = (
    "WP",
    "Contract",
    "Worker state",
    "Candidate HEAD SHA",
    "Worker pre-review",
    "Frozen candidate SHA",
    "Branch frozen",
)


def field(body: str, name: str) -> str | None:
    match = re.search(
        rf"^{re.escape(name)}:\s*`?([^`\r\n]+?)`?\s*$", body, re.I | re.M
    )
    return match.group(1).strip() if match else None


def is_none(value: str | None) -> bool:
    return value is not None and value.strip().upper() == "NONE"


def local_pointer(value: str | None, root: Path) -> Path | None:
    """Resolve a repository-local pointer when possible.

    URLs are accepted as pointers but cannot be inspected by this local validator.
    A markdown anchor and an optional @revision suffix after a recognizable file
    path are ignored for existence/content checks.
    """
    if value is None or is_none(value):
        return None
    value = value.strip().strip("`")
    if re.match(r"^https?://", value, re.I):
        return None

    raw = value.split("#", 1)[0].strip()
    # Contract is documented as <path/revision>; tolerate "path@revision" while
    # preserving ordinary @ characters in directory names if they ever appear.
    match = re.match(r"^(.+?\.(?:md|json|txt|ya?ml|toml|xml))(?:@.+)?$", raw, re.I)
    if match:
        raw = match.group(1)
    return root / raw


def contract_has_dependency(contract_path: Path | None) -> bool:
    if contract_path is None or not contract_path.is_file():
        return False
    text = contract_path.read_text(encoding="utf-8")
    match = re.search(r"^Depends on:\s*(.+?)\s*$", text, re.I | re.M)
    if not match:
        return False
    value = match.group(1).strip().strip("`*_ ")
    return value.lower() not in {"none", "n/a", "na", "-", "nothing"}


def validate(body: str, head_sha: str, root: Path) -> list[str]:
    errors: list[str] = []
    process_only = bool(PROCESS_ONLY_RE.search(body))
    lifecycle = any(field(body, name) is not None for name in LIFECYCLE_SENTINELS)

    # Documentation-only/process-maintenance PRs may deliberately avoid the Worker
    # lifecycle. Once any lifecycle surface is published, however, partial handoff
    # is more dangerous than no handoff and must fail closed.
    if process_only and not lifecycle:
        return errors
    if not lifecycle:
        return [
            "Ready non-PROCESS_ONLY PR is missing the canonical Worker handoff; "
            "publish all Required PR handoff fields before review."
        ]

    values = {name: field(body, name) for name in REQUIRED_FIELDS}
    for name, value in values.items():
        if value is None or not value.strip():
            errors.append(f"Missing required handoff field: {name}")

    if errors:
        return errors

    head_sha = head_sha.lower()
    if not SHA_RE.fullmatch(head_sha):
        errors.append(f"HEAD SHA is not an exact 40-character SHA: {head_sha!r}")

    for name in ("Baseline SHA", "Candidate HEAD SHA", "Frozen candidate SHA"):
        value = values[name]
        assert value is not None
        if not SHA_RE.fullmatch(value):
            errors.append(f"{name} must be an exact 40-character SHA, got {value!r}")

    for name in ("Transfer SHA", "Reviewed candidate SHA"):
        value = values[name]
        assert value is not None
        if not is_none(value) and not SHA_RE.fullmatch(value):
            errors.append(f"{name} must be NONE or an exact 40-character SHA, got {value!r}")

    candidate = (values["Candidate HEAD SHA"] or "").lower()
    frozen = (values["Frozen candidate SHA"] or "").lower()
    if SHA_RE.fullmatch(head_sha) and candidate != head_sha:
        errors.append(f"Candidate HEAD SHA {candidate} != PR HEAD {head_sha}")
    if SHA_RE.fullmatch(head_sha) and frozen != head_sha:
        errors.append(f"Frozen candidate SHA {frozen} != PR HEAD {head_sha}")

    expected = {
        "Worker state": "FROZEN_FOR_REVIEW",
        "Worker pre-review": "CLEAN",
        "Branch frozen": "YES",
        "Worker verdict": "IN_REVIEW",
        "Reviewer verdict": "PENDING",
    }
    for name, required in expected.items():
        got = (values[name] or "").upper()
        if got != required:
            errors.append(f"{name} must be {required} at Ready/freeze, got {values[name]!r}")

    fail_cycle = values["fail_cycle"] or ""
    if not re.fullmatch(r"\d+", fail_cycle):
        errors.append(f"fail_cycle must be a non-negative integer, got {fail_cycle!r}")

    pointer_fields = (
        "Contract",
        "Predecessor contract check",
        "Worker pre-review evidence",
        "Evidence",
    )
    pointers: dict[str, Path | None] = {}
    for name in pointer_fields:
        value = values[name]
        if name != "Predecessor contract check" and is_none(value):
            errors.append(f"{name} may not be NONE at Ready/freeze")
            pointers[name] = None
            continue
        path = local_pointer(value, root)
        pointers[name] = path
        if path is not None and not path.exists():
            errors.append(f"{name} points to missing repository path: {path.relative_to(root)}")

    contract_path = pointers.get("Contract")
    predecessor_value = values["Predecessor contract check"]
    if contract_has_dependency(contract_path) and is_none(predecessor_value):
        errors.append(
            "Contract declares a dependency but Predecessor contract check is NONE"
        )

    pred_path = pointers.get("Predecessor contract check")
    if pred_path is not None and pred_path.is_file():
        pred_text = pred_path.read_text(encoding="utf-8")
        if "PREDECESSOR_CONTRACT_CHECK" not in pred_text.upper():
            errors.append(
                "Predecessor contract check evidence does not contain "
                "PREDECESSOR_CONTRACT_CHECK"
            )

    pre_path = pointers.get("Worker pre-review evidence")
    if pre_path is not None and pre_path.is_file():
        pre_text = pre_path.read_text(encoding="utf-8")
        if not re.search(r"WORKER_PRE_REVIEW:\s*CLEAN", pre_text, re.I):
            errors.append(
                "Worker pre-review evidence does not contain WORKER_PRE_REVIEW: CLEAN"
            )

    return errors


def make_body(head: str, contract: str, pred: str, pre: str, evidence: str) -> str:
    return f"""WP: WP-TEST-01
Contract: {contract}
Baseline SHA: {'1' * 40}
Active Worker: test-worker
Worker state: FROZEN_FOR_REVIEW
Worker history: test-worker
Transfer SHA: NONE
Predecessor contract check: {pred}
Candidate HEAD SHA: {head}
Worker pre-review: CLEAN
Worker pre-review evidence: {pre}
Frozen candidate SHA: {head}
Branch frozen: YES
Worker verdict: IN_REVIEW
Reviewer verdict: PENDING
Reviewed candidate SHA: NONE
Evidence: {evidence}
fail_cycle: 0
"""


def self_test() -> None:
    head = "a" * 40
    with tempfile.TemporaryDirectory() as td:
        root = Path(td)
        (root / "contract-none.md").write_text("Depends on: none\n", encoding="utf-8")
        (root / "contract-dep.md").write_text("Depends on: WP-X\n", encoding="utf-8")
        (root / "pred.md").write_text("# PREDECESSOR_CONTRACT_CHECK\n", encoding="utf-8")
        (root / "bad-pred.md").write_text("# predecessor notes\n", encoding="utf-8")
        (root / "pre.md").write_text("WORKER_PRE_REVIEW: CLEAN\n", encoding="utf-8")
        (root / "evidence.md").write_text("evidence\n", encoding="utf-8")

        valid = make_body(head, "contract-none.md", "NONE", "pre.md", "evidence.md")
        assert validate(valid, head, root) == []

        dep_valid = make_body(head, "contract-dep.md", "pred.md", "pre.md", "evidence.md")
        assert validate(dep_valid, head, root) == []

        missing_pred = make_body(head, "contract-dep.md", "NONE", "pre.md", "evidence.md")
        assert any("declares a dependency" in e for e in validate(missing_pred, head, root))

        bad_pred = make_body(head, "contract-dep.md", "bad-pred.md", "pre.md", "evidence.md")
        assert any("does not contain PREDECESSOR_CONTRACT_CHECK" in e for e in validate(bad_pred, head, root))

        missing_field = valid.replace("Worker history: test-worker\n", "")
        assert any("Worker history" in e for e in validate(missing_field, head, root))

        docsync = "WORKFLOW_MODE: PROCESS_ONLY\nDocumentation reconciliation only.\n"
        assert validate(docsync, head, root) == []

        partial_process = "WORKFLOW_MODE: PROCESS_ONLY\nWorker state: FROZEN_FOR_REVIEW\n"
        assert validate(partial_process, head, root)

    print("worker-handoff self-test: PASS")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--head-sha", default=os.environ.get("TARGET_SHA", ""))
    parser.add_argument("--repo-root", default=".")
    parser.add_argument("--body-env", default="PR_BODY")
    parser.add_argument("--self-test", action="store_true")
    args = parser.parse_args()

    if args.self_test:
        self_test()
        return 0

    body = os.environ.get(args.body_env, "")
    errors = validate(body, args.head_sha, Path(args.repo_root).resolve())
    if errors:
        print("Worker handoff lint: FAIL", file=sys.stderr)
        for error in errors:
            print(f"- {error}", file=sys.stderr)
        return 2

    print("Worker handoff lint: GREEN")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
