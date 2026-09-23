#!/usr/bin/env python3
"""Causal controls for REVIEW_READY -> CLOSED -> PASS integration."""
from __future__ import annotations

from pathlib import Path
import sys

ROOT = Path(__file__).resolve().parents[1]
CLOSURE = ROOT / ".github/workflows/review-ready-closure.yml"
TRANSITIONS = ROOT / ".github/workflows/state-transitions.yml"
REUSE = ROOT / ".github/workflows/receipt-freeze-reuse.yml"
TELEGRAM = ROOT / ".github/workflows/telegram-notify.yml"
RAW_SUPPRESSION = "if marker_state == 'REVIEW_READY':\n                      sys.exit(0)"
CLOSED_PROMOTION = "state = 'REVIEW_READY' if marker_state == 'REVIEW_READY_CLOSED' else marker_state"


def closure_errors(text: str) -> list[str]:
    required = {
        "shared binding selection": "review-ready-binding.py select",
        "bound validation external id": 'external_id="validation-context:${PR}:${TARGET_SHA}:${BINDING_DIGEST}:${VALIDATION_RUN_ID}"',
        "closed key from shared binding": 'CLOSED_KEY: ${{ steps.marker.outputs.review_ready_closed_key }}',
        "closed marker binding field": "Review Binding Digest: ${BINDING_DIGEST}",
        "single terminal dispatch": '--arg state "REVIEW_READY_CLOSED"',
    }
    return [label for label, needle in required.items() if needle not in text]


def transition_errors(text: str) -> list[str]:
    required = {
        "shared binding selection": "review-ready-binding.py select",
        "closed key consumption": "review_ready_closed_key",
        "closed marker required": "No matching REVIEW_READY_CLOSED marker predating Reviewer verdict",
        "closed binding field checked": "Review Binding Digest",
        "bound validation external id": 'external_id="validation-context:${PR}:${reviewed_sha}:${binding_digest}:${validation_run_id}"',
        "pass merge bound": 'key="pass-merge:${PR}:${reviewed_sha}:${binding_digest}"',
    }
    errors = [label for label, needle in required.items() if needle not in text]
    if 'event_type:"arkus_notification",client_payload:{state:$state' in text and '--arg state "REVIEW_READY"' in text:
        errors.append("raw REVIEW_READY repository dispatch remains in state transitions")
    return errors


def reuse_errors(text: str) -> list[str]:
    required = {
        "reuse marker digest": "Reuse Context Digest: ${REUSE_CONTEXT_DIGEST}",
        "reuse key digest": 'key="review-ready:${PR}:${SHA}:${REUSE_CONTEXT_DIGEST}"',
        "no-dispatch boundary": "REVIEW_READY is deliberately not dispatched to Telegram",
    }
    errors = [label for label, needle in required.items() if needle not in text]
    if 'repos/${GITHUB_REPOSITORY}/dispatches' in text:
        errors.append("receipt reuse still repository-dispatches raw REVIEW_READY")
    return errors


def telegram_errors(text: str) -> list[str]:
    errors: list[str] = []
    # Telegram has two transports that can carry automation markers:
    # repository_dispatch and issue_comment. Both must suppress raw REVIEW_READY
    # and promote CLOSED. Requiring two occurrences prevents one path from
    # accidentally satisfying the other's control.
    if text.count(RAW_SUPPRESSION) != 2:
        errors.append(f"expected raw REVIEW_READY suppression in both Telegram paths, got {text.count(RAW_SUPPRESSION)}")
    if text.count(CLOSED_PROMOTION) != 2:
        errors.append(f"expected CLOSED promotion in both Telegram paths, got {text.count(CLOSED_PROMOTION)}")
    return errors


def mutate_must_red(text: str, old: str, new: str, checker, label: str) -> None:
    if old not in text:
        raise AssertionError(f"mutation anchor missing: {label}")
    mutated = text.replace(old, new, 1)
    if not checker(mutated):
        raise AssertionError(f"mutation stayed GREEN: {label}")


def main() -> int:
    try:
        closure = CLOSURE.read_text(encoding="utf-8")
        transitions = TRANSITIONS.read_text(encoding="utf-8")
        reuse = REUSE.read_text(encoding="utf-8")
        telegram = TELEGRAM.read_text(encoding="utf-8")
        errors = closure_errors(closure) + transition_errors(transitions) + reuse_errors(reuse) + telegram_errors(telegram)
        if errors:
            for error in errors:
                print(f"REVIEW_READY_INTEGRATION_ERROR: {error}", file=sys.stderr)
            return 1

        mutate_must_red(
            closure,
            'external_id="validation-context:${PR}:${TARGET_SHA}:${BINDING_DIGEST}:${VALIDATION_RUN_ID}"',
            'external_id="validation-context:${PR}:${TARGET_SHA}:${CONTEXT_DIGEST}:${VALIDATION_RUN_ID}"',
            closure_errors,
            "closure drops reuse binding",
        )
        mutate_must_red(
            transitions,
            'external_id="validation-context:${PR}:${reviewed_sha}:${binding_digest}:${validation_run_id}"',
            'external_id="validation-context:${PR}:${reviewed_sha}:${digest}:${validation_run_id}"',
            transition_errors,
            "PASS transition drops reuse binding",
        )
        mutate_must_red(
            transitions,
            "No matching REVIEW_READY_CLOSED marker predating Reviewer verdict",
            "No matching REVIEW_READY marker predating Reviewer verdict",
            transition_errors,
            "PASS no longer requires terminal closure",
        )
        raw_dispatch = reuse + '\nrepos/${GITHUB_REPOSITORY}/dispatches\n'
        if not reuse_errors(raw_dispatch):
            raise AssertionError("raw receipt-reuse dispatch stayed GREEN")
        mutate_must_red(
            telegram,
            RAW_SUPPRESSION,
            "if marker_state == 'NEVER':\n                      sys.exit(0)",
            telegram_errors,
            "Telegram re-enables raw REVIEW_READY on one transport",
        )
        print("REVIEW_READY_INTEGRATION_CONTROLS_GREEN")
        return 0
    except (OSError, AssertionError) as exc:
        print(f"REVIEW_READY_INTEGRATION_ERROR: {exc}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
