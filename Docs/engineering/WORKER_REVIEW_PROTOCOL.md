# Worker → Reviewer Protocol

Version: 1.3 — 2026-09-18

## Purpose

GitHub is the complete handoff surface between Worker, independent Reviewer, finalization and DocSync. Private chat memory is never required to reconstruct state.

## Core bias

A Worker tries to satisfy the contract. Before freeze, that same Worker must also adversarially pre-review the candidate and try to falsify its own work. An independent Reviewer then tries to falsify the frozen candidate from a fresh context.

Worker pre-review is a quality gate, not an independent review. `WORKER_PRE_REVIEW: CLEAN` never means `PASS`, never satisfies the Reviewer obligation and never permits the Reviewer to trust Worker conclusions.

Adversarial does **not** mean unbounded. Worker and Reviewer must attack the WP's actual acceptance claims inside the trust boundary established by `FOUNDATIONAL_PROOF_STANDARD.md`; they do not earn quality by inventing arbitrary subversions of explicitly trusted infrastructure.

## Adoption boundary

Version 1.3 applies to implementation candidates frozen after the commit containing this protocol version reaches `main`.

A candidate already validly frozen before that adoption point remains reviewable under the handoff/protocol that governed its freeze. An ACTIVE/Draft candidate that has not frozen yet must reconcile its pre-review and foundational proof claim with v1.3 before freeze. This process change alone does not create an implementation defect in an already-frozen candidate.

## State machine

```text
DRAFT + ACTIVE
  -> Worker may write
  -> Worker must complete adversarial pre-review before freeze
  -> any in-claim pre-review finding is repaired while still Draft + ACTIVE
  -> out-of-boundary residual risks are recorded, not automatically hardened
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
6. For foundational WPs, satisfy `FOUNDATIONAL_PROOF_STANDARD.md` including explicit trust boundary and proof-budget verdict before freeze.
7. Before freeze, perform the mandatory Worker pre-review defined below against the complete candidate, contract, evidence and proof boundary.
8. If pre-review finds an in-claim defect, remain Draft + ACTIVE, repair the causal defect boundary, rerun affected validation/evidence and repeat pre-review. Do not freeze a knowingly defective candidate.
9. If pre-review finds only a risk that requires arbitrary subversion of infrastructure explicitly inside the trusted base or a non-canonical unsupported path, record it as residual risk unless the WP explicitly owns that guarantee. Do not automatically open another hardening cycle.
10. A candidate may freeze only with `WORKER_PRE_REVIEW: CLEAN`, `PROOF_BUDGET_VERDICT: WITHIN_BUDGET` where applicable, and no known blocking defect.
11. Before Ready, stop every writer, read the exact 40-char HEAD and record it as `Frozen candidate SHA`.
12. Mark `Worker state: FROZEN_FOR_REVIEW` and `Branch frozen: YES`.
13. After Ready, do not modify implementation until Reviewer verdict.
14. Worker never acts as the independent Reviewer of its own candidate.

A superseded implementation PR must be explicitly marked/closed so GitHub does not present two active ownership surfaces for the same WP.

## Mandatory Worker pre-review

The pre-review is an adversarial quality gate performed by the Worker while the PR is still Draft. Its purpose is to catch defects that should not consume an independent Reviewer cycle while staying bounded to the contract actually being delivered.

The Worker must temporarily switch from implementation reasoning to falsification reasoning and inspect the candidate as if trying to issue a Reviewer FAIL. At minimum it must:

- re-read the exact WP acceptance criteria, DoD, allowed/forbidden scope and every binding engineering/proof document;
- identify the explicit claim and trust boundary before inventing attacks;
- inspect the complete baseline→candidate diff rather than only the last repair;
- verify tests/CI/evidence actually prove the contract rather than merely exercising representative happy paths;
- inspect negative/error behaviour, boundary conditions, fail-closed behaviour and handoff/freeze requirements;
- search for missing objects, paths, variants or effective behaviour that could sit outside an asserted completeness/proof universe **inside the declared claim**;
- distinguish a local/trivial defect from a causal architectural/proof-boundary defect and repair at the correct level;
- for foundational WPs, actively attempt material causal self-attacks against completeness, independent-oracle assumptions and false-green paths that remain inside `FOUNDATIONAL_PROOF_STANDARD.md`;
- record any material in-claim class discovered and the regression/self-attack that now protects it;
- classify out-of-boundary findings as residual risk rather than silently expanding the claim;
- perform the proof-budget check and stop for re-audit if proof/support machinery is expanding without corresponding acceptance/product progress;
- verify no known blocker remains before declaring the pre-review clean.

The pre-review must stay cheaper than the independent review: it does not need to duplicate every Reviewer action or produce a second full review report. It must, however, be substantive enough that an obvious contract breach cannot knowingly be handed off.

The pre-review is not required to discover a novel defect in order to be valid. `CLEAN` may mean that serious falsification attempts found no in-boundary blocker.

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

and the candidate must remain Draft. Missing tooling, skipped mandatory proof or an unresolved material doubt inside the claim is `NOT_READY`, not `CLEAN`.

The independent Reviewer must not treat the pre-review report as an authoritative checklist or limit its search to Worker-highlighted risks. Reviewer independence exists specifically to discover what the Worker did not see **within the contract being reviewed**.

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

Concurrent writers on the same WP candidate are not permitted. If exclusive ownership is lost, stop, isolate to one canonical branch/PR and record the transfer rather than allowing branches to race.

## Reviewer rules

Reviewer must:

- be independent of Worker implementation for the frozen candidate;
- reconstruct contract and repository state from GitHub;
- verify PR HEAD == Frozen candidate SHA at review start;
- for candidates governed by v1.2+ pre-review rules, verify the handoff records `Worker pre-review: CLEAN`, while treating that only as Worker readiness evidence;
- inspect complete baseline→candidate diff, tests, CI and evidence;
- challenge claims rather than trust Worker prose or Worker pre-review conclusions;
- for foundational WPs, independently search for omission/false-green classes and attack completeness **inside the WP claim and declared trust boundary**, including risks not highlighted by the Worker;
- distinguish an in-claim material defect from a risk that requires compromising the declared trusted base or using an unsupported path;
- not require discovery of a novel defect as a condition of a valid review;
- not FAIL solely because an explicitly trusted infrastructure component could theoretically be subverted, unless the WP acceptance criteria place that component inside the claim;
- never repair implementation;
- emit `PASS | FAIL | BLOCKED | READY_FOR_LOCAL_VALIDATION` naming the exact reviewed SHA.

A green Worker test suite and a clean Worker pre-review are necessary, never sufficient. Reviewer independence means independent judgment, not an obligation to expand the product's security/proof claim.

## FAIL

A FAIL must identify the violated criterion, evidence, expected behavior and minimal correction boundary. The same WP remains unresolved. Repair creates a new candidate SHA and requires a fresh Worker pre-review followed by a fresh independent review.

A local/trivial defect may be repaired locally. A finding that invalidates the proof boundary, architecture or completeness argument **inside the accepted claim** must be repaired at that causal boundary rather than by special-casing the reported example.

A finding outside the claim/trust boundary should normally be recorded as residual risk or proposed as a future hardening WP, not converted into an implicit expansion of the current WP.

## PASS / merge preflight

Merge is valid only when:

- the frozen handoff recorded `Worker pre-review: CLEAN` when required by the governing protocol;
- independent PASS names the exact Frozen candidate SHA;
- required CI/evidence for that SHA is green/complete;
- foundational proof budget is within budget when applicable;
- no later implementation mutation exists;
- no blocking in-claim finding remains.

If documentation-only review finalization is used, it must not redefine the reviewed implementation SHA.

## Foundational circuit breaker

If two independent Reviewer FAILs expose the same foundational defect class, stop local patching and re-audit the foundation before another downstream repair loop.

A single FAIL proving that the universe used by an in-claim completeness assertion can self-shrink, omit material objects by construction, or circularly define its own proof set is already an architectural finding. Re-audit that proof boundary immediately before another implementation cycle.

Independently, if two consecutive repair/pre-review cycles materially expand proof/support machinery without corresponding progress in product behaviour, an explicit acceptance gap or a realistic in-boundary false-green class, stop and re-audit the claim/trust boundary under the proof-budget rule. Do not continue merely to accumulate attacks.

## DocSync

After implementation merge, reconcile ROADMAP, WP state, architecture/ADR, evidence and handoff. Only then may the next WP be selected.
