# Worker → Reviewer Protocol

Version: 1.0 — 2026-09-18

## Purpose

GitHub is the complete handoff surface between Worker, independent Reviewer, finalization and DocSync. Private chat memory is never required to reconstruct state.

## Core bias

A Worker tries to satisfy the contract. A Reviewer tries to falsify the candidate. They must not be the same context for the same frozen candidate.

## State machine

```text
DRAFT + ACTIVE
  -> Worker may write
READY + FROZEN_FOR_REVIEW
  -> no Worker writes
  -> independent Reviewer owns next action
FAIL
  -> same WP returns to Draft for repair
PASS
  -> exact reviewed SHA may proceed to finalization/merge
MERGE
  -> DocSync before selecting next WP
```

## Worker rules

1. One active Worker per WP candidate.
2. Start from current `main`; record baseline SHA.
3. Open/keep PR Draft while implementation can change.
4. Respect exact WP Allowed/Forbidden scope and dependencies.
5. Produce reproducible evidence before review.
6. For foundational WPs, satisfy `FOUNDATIONAL_PROOF_STANDARD.md` before freeze.
7. Before Ready, stop every writer, read the exact 40-char HEAD and record it as `Frozen candidate SHA`.
8. Mark `Worker state: FROZEN_FOR_REVIEW` and `Branch frozen: YES`.
9. After Ready, do not modify implementation until Reviewer verdict.
10. Worker never self-reviews.

## Required PR handoff fields

```text
WP: <id>
Contract: <path/revision>
Baseline SHA: <40-char>
Active Worker: <actor>
Worker state: ACTIVE | PAUSED | FROZEN_FOR_REVIEW
Worker history: <actors>
Transfer SHA: <40-char or NONE>
Candidate HEAD SHA: <40-char>
Frozen candidate SHA: <40-char or NONE>
Branch frozen: YES | NO
Worker verdict: IN_PROGRESS | IN_REVIEW
Reviewer verdict: PENDING | PASS | FAIL | BLOCKED
Reviewed candidate SHA: <40-char or NONE>
Evidence: <path/link>
fail_cycle: <integer>
```

## Transfer

A second Worker may continue the same WP only after the prior Worker stops, the PR is Draft, and a `Transfer SHA` + Worker history update is recorded. Transfer does not reset `fail_cycle` or erase evidence.

## Reviewer rules

Reviewer must:

- be independent of Worker implementation for the frozen candidate;
- reconstruct contract and repository state from GitHub;
- verify PR HEAD == Frozen candidate SHA at review start;
- inspect complete baseline→candidate diff, tests, CI and evidence;
- challenge claims rather than trust Worker prose;
- for foundational WPs, independently search for omission classes and attack completeness;
- never repair implementation;
- emit `PASS | FAIL | BLOCKED | READY_FOR_LOCAL_VALIDATION` naming the exact reviewed SHA.

A green Worker test suite is necessary, never sufficient.

## FAIL

A FAIL must identify the violated criterion, evidence, expected behavior and minimal correction boundary. The same WP remains unresolved. Repair creates a new candidate SHA and requires a fresh independent review.

## PASS / merge preflight

Merge is valid only when:

- independent PASS names the exact Frozen candidate SHA;
- required CI/evidence for that SHA is green/complete;
- no later implementation mutation exists;
- no blocking finding remains.

If documentation-only review finalization is used, it must not redefine the reviewed implementation SHA.

## Foundational circuit breaker

If two independent Reviewer FAILs expose the same foundational defect class, stop local patching and re-audit the foundation before another downstream repair loop.

## DocSync

After implementation merge, reconcile ROADMAP, WP state, architecture/ADR, evidence and handoff. Only then may the next WP be selected.
