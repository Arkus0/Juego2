# PROCESS_ONLY autopilot recovery policy

Status: accepted and effective. Independent PASS was recorded for candidate `bdb6e97df570913d08041ec79ac76ed6652cce53` on PR #217, then merged to `main` as `9068ac97cdddf0e09c9e8fca847b2f06851cde3c`. This changes local process liveness, not any product WP, Reviewer verdict, exact-SHA proof, or H1-06 implementation.

The work plane may stop while the Telegram owner console stays alive. The console stores a supervisor-authenticated, derived `campaign.json` beside its local decision files and reconstructs the current PR/HEAD from GitHub when it restarts. A surviving controller lock or child process prevents a second work plane. A pending owner request keeps its campaign and request identity across a restart; an attested, abandoned, completed, other-campaign, or superseded request is excluded from `/status` and advertisement. The supervisor secret remains only in the owner console.

| Situation | Policy | Work-plane action | Control-plane action |
| --- | --- | --- | --- |
| Derived PR body/lifecycle/campaign state is stale but one exact GitHub state and durable transition determine the frontier | RECOVERABLE | Re-read, reconcile the derived state, continue the same PR/campaign; no new `fail_cycle` | Keep commands and valid decisions available |
| Exact-SHA Reviewer FAIL was already adopted or `REPAIR_REQUIRED` already exists | RECOVERABLE | Reuse the durable ledger/bot transition and its recorded cycle; write only missing derived side effects | Continue the existing campaign |
| A role, GitHub query, network operation, or state wait fails transiently | RETRYABLE | Retry at most three times, re-entering GitHub reconstruction each time; then `PAUSED_RECOVERABLE` | Remain online; `/resume` retries from durable state |
| Process exits between campaign persistence, role completion and local state update | RECOVERABLE / RETRYABLE | Check the controller lock, PR and log; resume the same campaign only when no work plane survives | Reannounce still-valid decisions |
| Conflicting authoritative verdicts, moved/ambiguous SHA, two incompatible PR owners, invalid authority, or a foundational circuit breaker | HARD_BLOCKER | Stop WP advancement without inventing a verdict or changing product proof | Remain online; `/status`, `/help`, `/resume` and `/abandon` work |

`/resume` on an unresolved HARD_BLOCKER reports that the same SHA is still blocked. It does not waive the blocker. An owner can abandon the stopped campaign with `/abandon`; while a role is running, that command requests a stop at the next safe boundary. A restart does not turn a HARD_BLOCKER into permission to run another Reviewer or repair.

The H1-06 circuit-breaker case is an offline process regression only. It asserts that the WP remains stopped while the console can show, reannounce and accept a valid pending decision, answer `/status`, and later accept `/resume`, `/abandon` and another command. It does not change the accepted H1-06 proof or reopen merged PR #195.

The exact Reviewer identity, `ARKUS_INTENT_V1` GitHub PR-review authority, owner authority, one canonical PR, idempotent `fail_cycle`, no duplicate reasoning roles after durable completion, and accepted product gates remain binding. A missing or contradictory authoritative source still fails closed.
