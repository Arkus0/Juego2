# WP-OPS-00 — Activate agentic operating flow

Status: PLANNED  
Class: PROCESS_ONLY  
Depends on: none

## Objective

Activate and acceptance-test the repository-independent operating flow already versioned in Juego2: GitHub handoff enforcement, Work/mobile transition automation, Codex/Claude adapters and low-noise Telegram notifications.

This WP is process infrastructure only. It does not satisfy or block any H0 technical acceptance criterion, but it should complete before relying on unattended automation for HK work.

## Acceptance

- GitHub Actions workflows are syntactically valid and execute on representative harmless events.
- `Worker Handoff` goes RED for an invalid Ready freeze and GREEN for an exact-SHA valid freeze.
- Transition markers are idempotent for REVIEW_PENDING, REPAIR_PENDING, FINALIZATION_PENDING and DOCSYNC_PENDING.
- Telegram repository secrets are configured and manual transport test delivers once without exposing credentials.
- Work/mobile tasks from `WORK_MOBILE_AUTOMATION.md` are instantiated and pass harmless Ready→Review, FAIL→Repair, PASS→Finalize, merge→DocSync and recovery tests.
- Codex Desktop and Claude Code are each labelled only `SETUP`, `VALIDATING` or `PRODUCTION_READY` according to actual acceptance evidence.
- Provider interruption is demonstrated not to become contractual FAIL or close an unresolved PR.
- Documentation records exact external setup state without secret values.

## Forbidden scope

Harness implementation, Unity, gameplay, DFU, changing H0 contracts except process-link fixes.

## DoD

Activation evidence persisted; no secret values committed; every backend status reflects tested reality.
