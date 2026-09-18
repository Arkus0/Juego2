# External Automation Setup Checklist

Repository contracts are versioned; the following external/account configuration cannot be copied as Git files and must be activated separately.

## GitHub

- [ ] Enable GitHub Actions for Juego2 if not already enabled.
- [ ] Make `Worker Handoff / validate-handoff` a required check for protected implementation branches/PRs when branch protection is configured.

## Telegram

- [ ] Add Juego2 Actions secret `TELEGRAM_BOT_TOKEN`.
- [ ] Add Juego2 Actions secret `TELEGRAM_CHAT_ID`.
- [ ] Run the manual `Telegram Notify` workflow test.
- [ ] Confirm delivery without exposing either secret in logs/comments.

## ChatGPT Work / mobile

Instantiate and acceptance-test the versioned contracts in `WORK_MOBILE_AUTOMATION.md`:

- [ ] Juego2 Review Ready
- [ ] Juego2 Review Fail Repair
- [ ] Juego2 Review Pass Finalize
- [ ] Juego2 Post Merge DocSync
- [ ] Juego2 Recovery Reconciler

Do not label the backend `PRODUCTION_READY` before the interruption/idempotency acceptance cases pass.

## Codex Desktop

- [ ] Configure repo access.
- [ ] Verify minimal entry reconstructs GitHub and routes to the correct WP.
- [ ] Verify separate Worker/Reviewer contexts and exact-SHA handoff.
- [ ] Mark local backend `PRODUCTION_READY` only after acceptance test.

## Claude Code

- [ ] Configure repo access.
- [ ] Verify minimal entry and role leases.
- [ ] Verify provider interruption leaves GitHub pending state intact.
- [ ] Mark cloud backend `PRODUCTION_READY` only after acceptance test.

Secret values are never stored in this checklist or any repository file.
