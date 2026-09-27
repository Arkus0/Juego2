# WP-PA-B1 — Protocol reconciliation

Date: 2026-09-26

This evidence records lifecycle-only corrections discovered while freezing the research batch for independent review. No PA-07/08/09 research conclusion, fixture, oracle, ownership rule, or deferred boundary is changed here.

## Ready attempt findings

1. The first Ready attempt rejected the handoff because `Reviewed candidate SHA` was absent.
2. The same attempt treated the research batch as product/runtime work because the PR omitted the canonical `WORKFLOW_MODE: PROCESS_ONLY` authority.
3. After adding that authority, validation correctly detected that the legacy `Mode: WORKPACK` line created two simultaneous process-mode authorities. The legacy line is therefore removed from the canonical PR body.

## Final protocol shape

The canonical handoff must contain exactly one process-mode authority:

`WORKFLOW_MODE: PROCESS_ONLY`

It must also retain:

- `Reviewed candidate SHA: NONE` before independent review;
- the exact candidate/frozen SHA matching PR HEAD;
- `Worker state: FROZEN_FOR_REVIEW`;
- `Branch frozen: YES`;
- `Worker verdict: IN_REVIEW`;
- `Reviewer verdict: PENDING`.

These are lifecycle corrections only. The strict Worker pre-review remains CLEAN with zero unresolved material findings. A fresh exact-SHA validation cycle is required on the commit containing this evidence before Reviewer handoff.
