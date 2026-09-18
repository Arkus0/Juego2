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

- [ ] Juego2 Review Ready — configure **GPT-5.6 Sol · Extra High** for H0 Reviewer.
- [ ] Juego2 Review Fail Repair — configure **GPT-5.6 Sol · Extra High** for H0 Repair Worker.
- [ ] Juego2 Review Pass Finalize.
- [ ] Juego2 Post Merge DocSync.
- [ ] Juego2 Recovery Reconciler.
- [ ] Verify any manually/automatically launched H0 Worker also uses **GPT-5.6 Sol · Extra High**.
- [ ] Verify unavailable Sol Extra High causes `HUMAN_ACTION_REQUIRED: REQUIRED_MODEL_UNAVAILABLE`, never silent fallback.

Do not label the backend `PRODUCTION_READY` before the interruption/idempotency acceptance cases pass.

## Codex Desktop

- [ ] Configure repo access and mark the repository trusted so project `.codex/config.toml` is loaded.
- [ ] Verify the effective project model is `gpt-5.6-sol`.
- [ ] Verify `model_reasoning_effort = "xhigh"` for H0 Worker and Reviewer sessions.
- [ ] Verify minimal entry reconstructs GitHub and routes to the correct WP.
- [ ] Verify separate Worker/Reviewer contexts and exact-SHA handoff.
- [ ] Verify an accidental model/effort downgrade is detected before H0 implementation/review.
- [ ] Mark local backend `PRODUCTION_READY` only after acceptance test.

## Claude Code

- [ ] Configure repo access.
- [ ] Configure Claude's own strongest foundational Worker/Reviewer profile independently; do not map Sol/xhigh names literally.
- [ ] Verify minimal entry and role leases.
- [ ] Verify provider interruption leaves GitHub pending state intact.
- [ ] Mark cloud backend `PRODUCTION_READY` only after acceptance test.

Secret values are never stored in this checklist or any repository file.
