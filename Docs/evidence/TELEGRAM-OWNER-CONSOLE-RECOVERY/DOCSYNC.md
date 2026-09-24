# Telegram owner console recovery — DocSync

Status: DOCSYNC_COMPLETE
Date: 2026-09-24
Class: PROCESS_ONLY / DOCUMENTATION_ONLY

## Accepted identity

- Canonical process PR: `#186`
- Accepted candidate: `63f95e991bdbf2b9e8435403603dd1501389fd9d`
- Final independent Reviewer PASS: `#5306057069`
- Merge: `08445b768b4b7bd0337ac4ace2d9fe1159fc35df`

## Validation

Exact-SHA checks on the accepted candidate were GREEN:

- Telegram Owner Console Validation run `36014931144`
- Arkus Main Safety run `36014931121`

The transport/recovery suite includes causal coverage for same-PR interrupted Worker recovery, bounded salvage, short-quota resume, duplicate durable-side-effect suppression, canonical `--adopt` handoff, and fresh `origin/main` admission.

## Review history

Review `#5305903327` identified two causal blockers in the initial recovery candidate: post-failure short-quota classification could infer quota exhaustion merely from a nearly exhausted short window, and the remote admission shim could decide new/resume against a stale local `origin/main` ref.

The accepted candidate closes both blockers. Post-session automatic short retry now requires explicit short-limit reached evidence rather than low remaining percentage alone; non-quota failures therefore remain fail-closed even with the short window at 98% used. Remote admission now executes `git fetch origin main` before the new/resume decision, with an integration falsification that proves a stale remote-tracking ref is refreshed before admission.

The final independent review `#5306057069` accepted those repairs and did not reopen the previously accepted same-PR/local-ahead recovery, non-authoritative salvage, duplicate suppression or canonical adoption behavior.

## Persisted result

`Docs/engineering/TELEGRAM_OWNER_CONSOLE.md` now records the accepted durability amendment: early canonical PR ownership, exact same-PR recovery, preservation of local-ahead/dirty work, bounded external salvage, coherent commit/push checkpoints, causal short-quota retry, protected general-quota stop, duplicate-side-effect suppression, fresh-main admission and canonical lifecycle handoff.

No H1, PA, CITY, DW or CTX workpack state changes. No next-WP advancement is caused by this PROCESS_ONLY DocSync.

DOCSYNC_COMPLETE
Next WP: unchanged by this process-only amendment.
