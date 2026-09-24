# WP-OPS-00 — Activate agentic operating flow

Status: PLANNED  
Class: PROCESS_ONLY  
Depends on: none

Historical note (2026-09-23): this pre-Automation-V2 activation sketch refers to old Work/mobile and marker names. It is **not** authorization to replace the currently accepted Automation V2 or to call those obsolete acceptance items complete. The optional prospective local session driver is specified separately in `Docs/engineering/LOCAL_WP_AUTOPILOT.md`; any future execution of OPS-00 first needs an explicit re-scope against current GitHub and protocol state.

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
