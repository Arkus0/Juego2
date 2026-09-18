# Worker → Reviewer Protocol

Version: 1.2 — 2026-09-18

## Purpose

GitHub is the complete handoff surface between Worker, independent Reviewer, finalization and DocSync. Private chat memory is never required to reconstruct state.

## Core bias

A Worker tries to satisfy the contract. Before freeze, that same Worker must also adversarially pre-review the candidate and try to falsify its own work. An independent Reviewer then tries to falsify the frozen candidate from a fresh context.

Worker pre-review is a quality gate, not an independent review. `WORKER_PRE_REVIEW: CLEAN` never means `PASS`, never satisfies the Reviewer obligation and never permits the Reviewer to trust Worker conclusions.

## Adoption boundary

This v1.2 pre-review gate applies to implementation candidates frozen after the commit containing this protocol version reaches `main`.

A candidate already validly frozen before that adoption point remains reviewable under the handoff/protocol that governed its freeze; the process change alone does not invalidate its exact SHA. If that candidate receives a Reviewer FAIL, returns to Draft, changes implementation/evidence, transfers to a materially changed candidate, or is otherwise re-frozen after adoption, the next freeze must satisfy v1.2 including `WORKER_PRE_REVIEW: CLEAN`.

This transition prevents process hardening from manufacturing a false implementation defect in an already-frozen candidate while ensuring every subsequent candidate receives the new gate.

## State machine

```text
DRAFT + ACTIVE
  -> Worker may write
  -> Worker must complete adversarial pre-review before freeze
  -> any pre-review finding is repaired while still Draft + ACTIVE
WORKER_PRE_REVIEW: CLEAN
  -> candidate may proceed to freeze
READY + FROZEN_FOR_REVIEW
  -> no Worker writes
  -> independent Reviewer owns next action
FAIL
  -> same WP returns to Draft for repair
  -> pre-review must be rerun on the repaired candidate before the next freeze
PASS
  -> exact reviewed SHA may proceed to finalization/merge
MERGE
  -> DocSync before selecting next WP
```

## Worker rules

1. One active Worker per WP candidate and one canonical open implementation PR per active WP unless an explicit transfer/repair migration is being completed.
2. Start from current `main`; record baseline SHA.
3. Open/keep PR Draft while implementation can change.
4. Respect exact WP Allowed/Forbidden scope and dependencies.
5. Produce reproducible evidence before review.
6. For foundational WPs, satisfy `FOUNDATIONAL_PROOF_STANDARD.md` before freeze.
7. Before freeze, perform the mandatory Worker pre-review defined below against the complete candidate, contract, evidence and proof boundary.
8. If pre-review finds any defect, remain Draft + ACTIVE, repair the causal defect boundary, rerun affected validation/evidence and repeat pre-review. Do not freeze a knowingly defective candidate.
9. A candidate may freeze only with `WORKER_PRE_REVIEW: CLEAN` and no known blocking defect.
10. Before Ready, stop every writer, read the exact 40-char HEAD and record it as `Frozen candidate SHA`.
11. Mark `Worker state: FROZEN_FOR_REVIEW` and `Branch frozen: YES`.
12. After Ready, do not modify implementation until Reviewer verdict.
13. Worker never acts as the independent Reviewer of its own candidate.

A superseded implementation PR must be explicitly marked/closed so GitHub does not present two active ownership surfaces for the same WP.

## Mandatory Worker pre-review

The pre-review is an adversarial quality gate performed by the Worker while the PR is still Draft. Its purpose is to catch defects that should not consume an independent Reviewer cycle.

The Worker must temporarily switch from implementation reasoning to falsification reasoning and inspect the candidate as if trying to issue a Reviewer FAIL. At minimum it must:

- re-read the exact WP acceptance criteria, DoD, allowed/forbidden scope and every binding engineering/proof document;
- inspect the complete baseline→candidate diff rather than only the last repair;
- verify tests/CI/evidence actually prove the contract rather than merely exercising representative happy paths;
- inspect negative/error behaviour, boundary conditions, fail-closed behaviour and handoff/freeze requirements;
- search for missing objects, paths, variants or effective behaviour that could sit outside an asserted completeness/proof universe;
- distinguish a local/trivial defect from a causal architectural/proof-boundary defect and repair at the correct level;
- for foundational WPs, actively attempt material causal self-attacks against completeness, independent-oracle assumptions and false-green paths, consistent with `FOUNDATIONAL_PROOF_STANDARD.md`;
- record any material class discovered and the regression/self-attack that now protects it;
- verify no known blocker remains before declaring the pre-review clean.

The pre-review must stay cheaper than the independent review: it does not need to duplicate every Reviewer action or produce a second full review report. It must, however, be substantive enough that an obvious contract breach cannot knowingly be handed off.

A valid clean result is recorded as:

```text
WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: <integer>
WORKER_PRE_REVIEW_EVIDENCE: <path/link>
```

If the Worker cannot establish cleanliness:

```text
WORKER_PRE_REVIEW: NOT_READY
```

and the candidate must remain Draft. Missing tooling, skipped mandatory proof or an unresolved material doubt is `NOT_READY`, not `CLEAN`.

The independent Reviewer must not treat the pre-review report as an authoritative checklist or limit its search to Worker-highlighted risks. Reviewer independence exists specifically to discover what the Worker did not see.

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
Worker pre-review: NOT_RUN | NOT_READY | CLEAN
Worker pre-review evidence: <path/link or NONE>
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

If repair is migrated to a new PR, the previous PR must be closed/superseded and the new PR must preserve Worker history, transfer SHA, reviewed SHA and `fail_cycle`; migration is not a clean restart.

A transfer invalidates any prior `WORKER_PRE_REVIEW: CLEAN` unless the receiving Worker proves that the candidate bytes, evidence and proof claims are unchanged. Any implementation/evidence mutation after a clean pre-review requires the pre-review to be rerun before freeze.

## Reviewer rules

Reviewer must:

- be independent of Worker implementation for the frozen candidate;
- reconstruct contract and repository state from GitHub;
- verify PR HEAD == Frozen candidate SHA at review start;
- for candidates governed by v1.2+, verify the handoff records `Worker pre-review: CLEAN`, while treating that only as Worker readiness evidence;
- for candidates grandfathered by the adoption boundary, do not fail them solely because the v1.2 pre-review fields did not yet exist;
- inspect complete baseline→candidate diff, tests, CI and evidence;
- challenge claims rather than trust Worker prose or Worker pre-review conclusions;
- for foundational WPs, independently search for omission classes and attack completeness, including risks not highlighted by the Worker;
- never repair implementation;
- emit `PASS | FAIL | BLOCKED | READY_FOR_LOCAL_VALIDATION` naming the exact reviewed SHA.

For candidates governed by v1.2+, a green Worker test suite and a clean Worker pre-review are necessary, never sufficient.

## FAIL

A FAIL must identify the violated criterion, evidence, expected behavior and minimal correction boundary. The same WP remains unresolved. Repair creates a new candidate SHA and requires a fresh Worker pre-review followed by a fresh independent review.

A local/trivial defect may be repaired locally. A finding that invalidates the proof boundary, architecture or completeness argument must be repaired at that causal boundary rather than by special-casing the reported example.

## PASS / merge preflight

Merge is valid only when:

- for candidates governed by v1.2+, the frozen handoff recorded `Worker pre-review: CLEAN` for the exact candidate lineage;
- a grandfathered pre-v1.2 candidate satisfies the handoff rules that governed its original valid freeze;
- independent PASS names the exact Frozen candidate SHA;
- required CI/evidence for that SHA is green/complete;
- no later implementation mutation exists;
- no blocking finding remains.

If documentation-only review finalization is used, it must not redefine the reviewed implementation SHA.

## Foundational circuit breaker

If two independent Reviewer FAILs expose the same foundational defect class, stop local patching and re-audit the foundation before another downstream repair loop.

A single FAIL proving that the universe used by a completeness claim can self-shrink, omit material objects by construction, or circularly define its own proof set is already an architectural finding. Re-audit that proof boundary immediately before another implementation cycle.

The Worker pre-review should catch these classes before handoff where possible, but finding one during pre-review does not satisfy or replace the later independent Reviewer attack duty.

## DocSync

After implementation merge, reconcile ROADMAP, WP state, architecture/ADR, evidence and handoff. Only then may the next WP be selected.
