#!/usr/bin/env python3
"""Causal controls for Batch C Reviewer claim wiring."""
from __future__ import annotations

from pathlib import Path
import sys

ROOT = Path(__file__).resolve().parents[1]
WORKFLOW = ROOT / ".github/workflows/reviewer-claim.yml"
SKILL = ROOT / ".agents/skills/validate-workpack/SKILL.md"
CONTRACT = ROOT / "Docs/engineering/REVIEW_CLAIM_V1.md"


def workflow_errors(text: str) -> list[str]:
    required = {
        "serialized per-PR concurrency": "group: arkus-review-claim-pr-${{ github.event.issue.number }}",
        "no cancellation race": "cancel-in-progress: false",
        "workflow-SHA policy checkout": "ref: ${{ github.workflow_sha }}",
        "policy self-test": "review-claim.py self-test",
        "live PR fetch": 'gh api "repos/${GITHUB_REPOSITORY}/pulls/${PR}" > pr.json',
        "stale-SHA rejection": '[[ "${head}" == "${target}" ]]',
        "ready-only claim": "[[ \"$(jq -r '.draft' pr.json)\" == \"false\" ]]",
        "claim decision": "review-claim.py decide",
        "release decision": "review-claim.py can-release",
        "grant marker": "State: REVIEW_CLAIMED",
        "deny marker": "State: REVIEW_CLAIM_DENIED",
        "release marker": 'state="REVIEW_CLAIM_RELEASED"',
        "90-minute lease": "date -u -d '+90 minutes'",
    }
    return [name for name, needle in required.items() if needle not in text]


def skill_errors(text: str) -> list[str]:
    required = {
        "contract adoption": "REVIEW_CLAIM_V1.md",
        "request before substantive review": "request the exact-SHA Reviewer lease before substantive review",
        "grant requirement": "REVIEW_CLAIMED",
        "duplicate stop": "BLOCKED: DUPLICATE_REVIEW_CLAIM",
        "Batch C circularity exception": "Batch C itself is the one-time adoption candidate",
        "release after verdict": "REVIEW_CLAIM_RELEASE_REQUEST",
    }
    return [name for name, needle in required.items() if needle not in text]


def contract_errors(text: str) -> list[str]:
    required = {
        "bot-only authority": "Only a `github-actions[bot]` marker is authoritative",
        "request does not choose id": "The Reviewer does not choose a Claim ID",
        "lease expiry recovery": "A granted lease lasts 90 minutes",
        "no semantic authority": "does not establish independence, PASS, FAIL",
        "prospective adoption": "Adoption is prospective",
    }
    return [name for name, needle in required.items() if needle not in text]


def assert_mutation_red(text: str, old: str, new: str, checker, label: str) -> None:
    if old not in text:
        raise AssertionError(f"mutation anchor missing: {label}")
    mutated = text.replace(old, new, 1)
    if not checker(mutated):
        raise AssertionError(f"causal mutation stayed GREEN: {label}")


def main() -> int:
    try:
        workflow = WORKFLOW.read_text(encoding="utf-8")
        skill = SKILL.read_text(encoding="utf-8")
        contract = CONTRACT.read_text(encoding="utf-8")
        errors = workflow_errors(workflow) + skill_errors(skill) + contract_errors(contract)
        if errors:
            for error in errors:
                print(f"REVIEW_CLAIM_WIRING_ERROR: {error}", file=sys.stderr)
            return 1

        assert_mutation_red(
            workflow,
            "group: arkus-review-claim-pr-${{ github.event.issue.number }}",
            "group: arkus-review-claim-${{ github.run_id }}",
            workflow_errors,
            "remove per-PR serialization",
        )
        assert_mutation_red(
            workflow,
            '[[ "${head}" == "${target}" ]]',
            '[[ -n "${target}" ]]',
            workflow_errors,
            "remove exact live-head binding",
        )
        assert_mutation_red(
            skill,
            "request the exact-SHA Reviewer lease before substantive review",
            "optionally request a Reviewer lease",
            skill_errors,
            "make claim optional",
        )
        assert_mutation_red(
            contract,
            "Only a `github-actions[bot]` marker is authoritative",
            "Any matching marker is authoritative",
            contract_errors,
            "allow forged claim marker",
        )
        print("BATCH_C_REVIEW_CLAIM_WIRING_GREEN")
        return 0
    except (OSError, AssertionError) as exc:
        print(f"REVIEW_CLAIM_WIRING_ERROR: {exc}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
