# PROCESS_ONLY owner-console / autopilot amendment

Status: BINDING PROCESS_ONLY amendment after merge to `main`.
Date: 2026-09-24

This amendment records the accepted process semantics from PR #194. It is documentation-only and changes no product workpack contract, product evidence, H1-05 implementation, architecture claim, or accepted product state.

Where older text in `LOCAL_WP_AUTOPILOT.md` or `TELEGRAM_OWNER_CONSOLE.md` conflicts with the behaviors below, this amendment governs only those process/admission/routing behaviors. All unaffected authority, trust-boundary, Reviewer-independence, proof and exact-SHA rules remain unchanged.

## Accepted identity

- Canonical process PR: `#194`
- Accepted candidate: `8ab77a2223ff4e48044dd4beb103257d8acedc4d`
- Independent Reviewer review: `#5308883522`
- Autopilot review ID: `4f95c89d5ce74c1abe3ee870431d52db`
- Reviewer verdict: `PASS`
- Merge commit: `9a6e89089dc683871c06111ec8eb2ae87aecc04a`
- Exact-candidate checks: `Arkus PROCESS_ONLY Hotfix Validation`, `Telegram Owner Console Validation`, `Arkus Candidate Validation`, and `Arkus Main Safety` GREEN on the accepted candidate.

## Telegram pending-decision universe

`/status` and decision advertisement use the same pending-validity universe for the active campaign.

A decision is eligible only when it belongs to the current `campaign_id`, is structurally valid, has the expected request digest, is neither completed nor abandoned, and has not already been answered for the current campaign. Stale prior-campaign JSON, malformed/tampered requests, completed/abandoned requests and old response files do not count as pending and are not advertised.

## State-aware `/work` and `/run`

The owner console supports two explicit modes:

- `/work <WP>` adopts the canonical existing PR/state and may launch only fresh Worker or Repair roles. It stops at the first lifecycle boundary that requires Reviewer, appeal-Reviewer, fail-audit, protocol-fix, DocSync, or any other non-Worker/Repair reasoning role. It never substitutes Luna/audit work for Worker-only intent. When the candidate reaches a context-valid `REVIEW_READY`, `/work` stops and reports the exact candidate SHA.
- `/run <WP>` adopts the canonical existing PR/state and continues the complete accepted lifecycle from that state. It may therefore start Reviewer, audit, repair, merge and DocSync roles when the canonical lifecycle requires them.

Both modes reuse the one canonical PR. An existing ACTIVE/IN_PROGRESS, REPAIR_REQUIRED, REVIEW_READY, PASS/merged or otherwise recoverable canonical state is adopted rather than bootstrapping a second PR. Normal exact-checkout, clean-worktree, one-controller and fail-closed admission rules still apply.

A Repair under `/work` is permitted only when the canonical non-Worker prerequisites for that repair are already durable. In particular, a fresh REPAIR_REQUIRED state whose lifecycle still requires the first `fail-audit` stops at a `FAIL_AUDIT` boundary instead of launching that audit through `/work`.

## Reviewer verdict adoption

GitHub is the source of truth for Reviewer verdicts. The local controller may request durable adoption of an already-published exact-SHA Reviewer `PASS` or `FAIL` even when the PR body still says `Reviewer verdict: PENDING`.

Adoption is fail-closed and requires all of the following:

- open, non-draft canonical PR;
- current PR HEAD equals Candidate/Frozen SHA;
- coherent frozen Worker handoff fields;
- one valid 32-character `Autopilot review ID`;
- one authoritative owner-authored structured Reviewer verdict for that ID and exact SHA;
- no contradictory duplicate verdict for the same review ID;
- a preceding bot-authored `REVIEW_READY` that matches the current canonical validation context, including key, target SHA, validation-context digest, effective WP, PROCESS_ONLY classification and non-foundational classification;
- no wrong-SHA verdict entering the same current REVIEW_READY cycle.

A minimal, stale-context, wrong-SHA, ambiguous, contradictory or non-owner structured verdict is not adoptable.

The local controller does not mint acceptance state. It requests a GitHub `repository_dispatch`; `github-actions[bot]` re-reads GitHub and performs the durable transition.

### Adopted FAIL

An exact-SHA adopted `FAIL` writes a durable `REVIEW_VERDICT_ADOPTED` ledger first, records the target `fail_cycle`, reconciles the PR body to that recorded cycle, and materializes bot-authority `REPAIR_REQUIRED`. Re-running the same adoption for the same Reviewer ID is idempotent: it reconciles missing side effects to the ledgered target cycle and must not increment the failure cycle twice.

### Adopted PASS

An exact-SHA adopted `PASS` writes the durable adoption ledger. The existing canonical lifecycle then applies its normal exact-SHA PASS/merge rules; adoption does not weaken merge preflight or Reviewer independence.

## Scope and routing consequence

This amendment is PROCESS_ONLY. It repairs owner-console/autopilot liveness and state adoption only. It does not reopen H1-05 or any product workpack, and it does not advance the project WP sequence. Any next WP remains whatever the current accepted dependency-valid `DOCSYNC_COMPLETE`/roadmap state says independently of this maintenance amendment.
