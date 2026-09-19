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
Frozen candidate SHA: `NONE`
Branch frozen: `NO`
Worker verdict: `IN_PROGRESS`
fail_cycle: `0`

## Reviewer state

Reviewer verdict: `PENDING`
Reviewed candidate SHA: `NONE`
Reviewer evidence: `NONE`

## Evidence

Evidence: `Validation/...`
Required exact-SHA validation: `PENDING`
Known limitations/skips: `NONE`

## Process

Mode: `WORKPACK`

> Draft means Worker may write. Before Ready: stop all writers, bind the exact HEAD as Frozen candidate SHA, set `Worker state: FROZEN_FOR_REVIEW`, `Branch frozen: YES`, and `Worker verdict: IN_REVIEW`. Ready freezes implementation until independent verdict. Role transitions are manual; no GitHub Actions or bootstrap is required.
