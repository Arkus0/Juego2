# Automation / Agentic Flow — Juego2

Version: 1.0 — 2026-09-18

This directory ports the useful operating system from `Arkus0/Juego` without inheriting its technical architecture.

## Deliberate biases retained

1. **GitHub is truth.** Provider sessions/tasks are disposable.
2. **Exact-SHA freeze.** Ready means implementation is frozen and reviewable.
3. **Adversarial independence.** Reviewer tries to falsify; it does not repair.
4. **Fail closed.** Missing required evidence/tooling is not PASS.
5. **Repair the same WP.** A FAIL does not authorize skipping ahead.
6. **Foundational circuit breaker.** Repeated same-class FAILs reopen architecture rather than grow patches.
7. **DocSync after merge.** Next work starts only after accepted state is synchronized.
8. **Low-noise operator channel.** Telegram reports only meaningful terminal/human events.
9. **Backend neutrality.** Work, Codex and Claude implement the same GitHub state machine.

## Canonical docs

- `AGENTIC_PROTOCOL_DURABILITY.md` — durable state/leases/recovery.
- `DEPENDENCY_ROUTING.md` — contractual DAG resolver.
- `WORK_MOBILE_AUTOMATION.md` — versioned Work/mobile trigger contracts.
- `CODEX_DESKTOP_AUTOMATION.md` — local backend policy.
- `CLAUDE_CODE_AUTOMATION.md` — Claude backend policy.
- `MODEL_POLICY.md` — capability-based role/model guidance.
- `TELEGRAM_NOTIFICATIONS.md` — notification semantics.

## Repository-native enforcement

- `.github/pull_request_template.md` — canonical state fields.
- `.github/workflows/worker-handoff.yml` — exact-SHA Ready guard.
- `.github/workflows/agentic-transition-markers.yml` — REVIEW/REPAIR/FINALIZATION/DOCSYNC durable markers.
- `.github/workflows/telegram-notify.yml` — Telegram transport.

## Skills / profiles

Skills: `.agents/skills/{plan-milestone,implement-workpack,validate-workpack,validate-milestone,update-handoff}`.

Profiles: `.opencode/agents/{architect,worker,reviewer}.md`.

## Current activation status

| Capability | Repository state | External setup |
|---|---|---|
| Worker/Reviewer protocol | Versioned | none |
| exact-SHA handoff guard | Versioned GitHub Action | GitHub Actions enabled |
| transition markers | Versioned GitHub Action | GitHub Actions enabled |
| Telegram transport | Versioned GitHub Action | requires `TELEGRAM_BOT_TOKEN` + `TELEGRAM_CHAT_ID` in Juego2 Actions secrets |
| Work/mobile role automation | Trigger/prompt contracts versioned | provider tasks must be instantiated and acceptance-tested |
| Codex Desktop automation | Policy versioned | local host adapter acceptance-test required |
| Claude Code automation | Policy versioned | cloud adapter acceptance-test required |

Do not call an external backend `PRODUCTION_READY` until it passes the acceptance criteria in `AGENTIC_PROTOCOL_DURABILITY.md`.

## What was intentionally NOT copied

DFU/Unity/M3/M4 assumptions, Shenmue reverse-engineering rules, legacy game paths, old headless implementation, project-specific validators and any secret values.
