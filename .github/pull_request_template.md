## Workpack

WP: `WP-...`
Contract: `Docs/workpacks/...`
Baseline SHA: `<40-char>`

## Worker state

Active Worker: `@...`
Worker state: `ACTIVE | PAUSED | FROZEN_FOR_REVIEW`
Worker history: `...`
Transfer SHA: `NONE`
Candidate HEAD SHA: `<40-char>`
Worker pre-review: `NOT_RUN | NOT_READY | CLEAN`
Worker pre-review evidence: `NONE`
Frozen candidate SHA: `NONE`
Branch frozen: `NO`
Worker verdict: `IN_PROGRESS | IN_REVIEW`
fail_cycle: `0`

## Reviewer state

Reviewer verdict: `PENDING | PASS | FAIL | BLOCKED`
Reviewed candidate SHA: `NONE`
Reviewer evidence: `NONE`

## Evidence

Evidence: `Validation/...`
Required exact-SHA validation: `PENDING | GREEN | RED | BLOCKED`
Known limitations/skips: `NONE`

## Process

Mode: `WORKPACK | PROCESS_ONLY`

> Draft means Worker may write and Automation V2 may run observation checks. Before Ready: stop all writers, bind exact HEAD as Candidate/Frozen SHA, record `Worker pre-review: CLEAN`, set `Worker state: FROZEN_FOR_REVIEW`, `Branch frozen: YES`, and `Worker verdict: IN_REVIEW`. Ready triggers frozen exact-SHA validation; only a green exact-SHA handoff may proceed to an independent Reviewer. A canonical exact-SHA PASS may be auto-merged; merged workpacks require DocSync before the next WP.
