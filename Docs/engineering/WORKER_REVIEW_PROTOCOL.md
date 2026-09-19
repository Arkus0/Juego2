# Worker → Reviewer Protocol

Version: 1.7 — 2026-09-19

## Purpose

GitHub is the complete handoff surface between Worker, independent Reviewer, finalization and DocSync. Private chat memory is never required to reconstruct state.

Juego2 / Arkus Harness is a game-development and software-verification project. Review activity is limited to repository-owned game-authoring code, fixtures, tests, CI and documentation. Historical terms such as `self-attack`, `attack fixture`, `bypass` or `adversarial review` mean ordinary negative/conformance testing in this repository; new work uses the neutral terminology defined in `AGENTS.md`. This terminology clarification does not weaken any acceptance or proof obligation.

## Core bias

A Worker tries to satisfy the contract. Before freeze, that same Worker must also perform a strict pre-review of the candidate and try to disprove its own acceptance assumptions. An independent Reviewer then strictly challenges the frozen candidate from a fresh context.

Worker pre-review is a quality gate, not an independent review. `WORKER_PRE_REVIEW: CLEAN` never means `PASS`, never satisfies the Reviewer obligation and never permits the Reviewer to trust Worker conclusions.

Strict review does **not** mean unbounded review. Worker and Reviewer must challenge the WP's actual acceptance claims inside the trust boundary established by `FOUNDATIONAL_PROOF_STANDARD.md`; they do not earn quality by inventing arbitrary out-of-contract behavior of explicitly trusted infrastructure.

Accepted predecessor guarantees are compositional. A downstream WP is expected to consume binding guarantees already accepted by its dependencies rather than defensively re-proving them. Concrete evidence may reopen an inherited causal boundary; theoretical possibility or a desire for redundant proof may not.

## Adoption boundary

Version 1.7 applies to Worker/review cycles that start after the commit containing this version reaches `main`. It does not retroactively bind an already-started cycle and does not bind the `PROCESS_ONLY` PR that adopts it.

The v1.7 change adds one documentation-only DocSync step: reconciling the residual ledger. It adds no Worker or Reviewer duty, no acceptance criterion and no proof obligation.

The v1.6 change added the bounded representative content-shape probe to the pre-review duties for applicable foundational WPs. It retains the v1.5 predecessor-contract reconstruction and v1.4 post-PASS continuity rules, and does not weaken implementation acceptance, proof obligations, Worker/Reviewer independence or FAIL repair rules.

## Predecessor contract inheritance

Before implementation begins, the Worker must reconstruct the accepted contract inherited from each direct dependency. At minimum it reads the dependency WP, completion metadata/exact reviewed SHA, independent PASS evidence, relevant proof matrix/residual-risk evidence when present, and binding architecture/invariant documents made authoritative by that dependency. Transitive predecessors are followed only when their invariants are materially relied on by the direct dependency or current WP; this is not a requirement to reread the entire project history.

The Worker records a concise `PREDECESSOR_CONTRACT_CHECK` in Worker plan/evidence containing:

- accepted predecessor/dependency and reviewed/merge SHA(s);
- inherited guarantees relevant to the current WP;
- guarantees newly owned by the current WP;
- inherited guarantees intentionally consumed rather than re-proved;
- the concrete condition that would justify reopening an inherited guarantee.

This check is a reasoning aid and auditable handoff, not a new semantic registry. Upstream accepted evidence remains authoritative.

The independent Reviewer reconstructs the same split independently. Before issuing FAIL for an apparent omission, the Reviewer must determine whether the omitted guarantee is already binding from an accepted predecessor. If it is, the finding is a current-WP blocker only when concrete evidence shows that the inherited guarantee does not apply to the effective path or that the predecessor claim itself was false. Demanding duplicate proof of an accepted predecessor claim is overdefense and counts against the proof budget.

## State machine

```text
DRAFT + ACTIVE
  -> Worker performs PREDECESSOR_CONTRACT_CHECK before implementation
  -> Worker may write
  -> Worker must complete strict pre-review before freeze
  -> any in-claim pre-review finding is repaired while still Draft + ACTIVE
  -> out-of-boundary residual risks are recorded, not automatically hardened
WORKER_PRE_REVIEW: CLEAN
  -> candidate may proceed to freeze
READY + FROZEN_FOR_REVIEW
  -> no Worker writes
  -> fresh independent Reviewer owns next action
FAIL
  -> same WP returns to Draft for a fresh repair Worker
  -> pre-review must be rerun on the repaired candidate before the next freeze
PASS
  -> verdict is fixed to exact reviewed SHA
  -> same session may switch to FINALIZATION/DOCSYNC mode
  -> exact-SHA merge preflight + merge
  -> documentation-only DocSync
  -> DOCSYNC_COMPLETE + dependency-valid Next WP
```

## Worker rules

1. One active Worker per WP candidate and one canonical open implementation PR per active WP unless an explicit transfer/repair migration is being completed.
2. Start from current `main`; record baseline SHA.
3. Before implementation, complete and persist the mandatory `PREDECESSOR_CONTRACT_CHECK` for direct accepted dependencies and relevant inherited invariants.
4. Open/keep PR Draft while implementation can change.
5. Respect exact WP Allowed/Forbidden scope and dependencies.
6. Produce reproducible evidence before review.
7. For foundational WPs, satisfy `FOUNDATIONAL_PROOF_STANDARD.md` including explicit trust boundary and proof-budget verdict before freeze.
8. Before freeze, perform the mandatory Worker pre-review defined below against the complete candidate, contract, inherited guarantees, evidence and proof boundary.
9. If pre-review finds an in-claim defect, remain Draft + ACTIVE, repair the causal defect boundary, rerun affected validation/evidence and repeat pre-review. Do not freeze a knowingly defective candidate.
10. If pre-review finds only a risk that requires arbitrary out-of-contract behavior of infrastructure explicitly inside the trusted base, duplicate re-proof of an accepted predecessor guarantee, or a non-canonical unsupported path, record it as residual risk unless the WP explicitly owns that guarantee. Do not automatically open another hardening cycle.
11. A candidate may freeze only with a valid predecessor contract check, `WORKER_PRE_REVIEW: CLEAN`, `PROOF_BUDGET_VERDICT: WITHIN_BUDGET` where applicable, and no known blocking defect.
12. Before Ready, stop every writer, read the exact 40-char HEAD and record it as `Frozen candidate SHA`.
13. Mark `Worker state: FROZEN_FOR_REVIEW` and `Branch frozen: YES`.
14. After Ready, do not modify implementation until Reviewer verdict.
15. Worker never acts as the independent Reviewer of its own candidate.

A superseded implementation PR must be explicitly marked/closed so GitHub does not present two active ownership surfaces for the same WP.

## Mandatory Worker pre-review

The pre-review is a strict quality gate performed by the Worker while the PR is still Draft. Its purpose is to catch defects that should not consume an independent Reviewer cycle while staying bounded to the contract actually being delivered.

The Worker must temporarily switch from implementation reasoning to independent-challenge reasoning and inspect the candidate as if trying to issue a Reviewer FAIL. At minimum it must:

- re-read the exact WP acceptance criteria, DoD, allowed/forbidden scope and every binding engineering/proof document;
- verify the `PREDECESSOR_CONTRACT_CHECK` still matches current accepted dependency evidence and distinguish inherited guarantees from guarantees actually owned by this WP;
- identify the explicit claim and trust boundary before inventing negative scenarios;
- inspect the complete baseline→candidate diff rather than only the last repair;
- verify tests/CI/evidence actually prove the contract rather than merely exercising representative happy paths;
- for a foundational WP that defines or changes authorable-state or public-contract semantics, inspect the required bounded representative content-shape probe and verify that its findings are explicitly classified without treating the probe as a completeness oracle;
- inspect negative/error behaviour, boundary conditions, fail-closed behaviour and handoff/freeze requirements;
- search for missing objects, paths, variants or effective behaviour that could sit outside an asserted completeness/proof universe **inside the declared claim**;
- distinguish a local/trivial defect from a causal architectural/proof-boundary defect and repair at the correct level;
- for foundational WPs, actively exercise material causal negative-conformance tests against completeness, independent-oracle assumptions and false-green paths that remain inside `FOUNDATIONAL_PROOF_STANDARD.md`;
- record any material in-claim class discovered and the regression/negative-conformance test that now protects it;
- classify accepted predecessor guarantees as consumed unless concrete evidence shows they are inapplicable/false; do not add duplicate proof merely for defence-in-depth;
- classify out-of-boundary findings as residual risk rather than silently expanding the claim;
- perform the proof-budget check and stop for re-audit if proof/support machinery is expanding without corresponding acceptance/product progress;
- verify no known blocker remains before declaring the pre-review clean.

The pre-review must stay cheaper than the independent review: it does not need to duplicate every Reviewer action or produce a second full review report. It must, however, be substantive enough that an obvious contract breach cannot knowingly be handed off.

The pre-review is not required to discover a novel defect in order to be valid. `CLEAN` may mean that serious independent challenge found no in-boundary blocker.

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

The independent Reviewer must not treat the pre-review report or predecessor check as an authoritative checklist or limit its search to Worker-highlighted risks. Reviewer independence exists specifically to discover what the Worker did not see **within the contract being reviewed**.

## Required PR handoff fields

```text
WP: <id>
Contract: <path/revision>
Baseline SHA: <40-char>
Active Worker: <actor>
Worker state: ACTIVE | PAUSED | FROZEN_FOR_REVIEW
Worker history: <actors>
Transfer SHA: <40-char or NONE>
Predecessor contract check: <path/link or NONE>
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

If dependency state or accepted predecessor evidence changed after the recorded predecessor check, the receiving Worker must refresh that check before further implementation or freeze.

Concurrent writers on the same WP candidate are not permitted. If exclusive ownership is lost, stop, isolate to one canonical branch/PR and record the transfer rather than allowing branches to race.

## Reviewer rules

Reviewer must:

- be independent of Worker implementation for the frozen candidate;
- reconstruct contract and repository state from GitHub;
- verify PR HEAD == Frozen candidate SHA at review start;
- for candidates governed by v1.2+ pre-review rules, verify the handoff records `Worker pre-review: CLEAN`, while treating that only as Worker readiness evidence;
- for candidates governed by v1.5+, independently reconstruct direct accepted predecessor guarantees and verify a predecessor contract check was recorded before implementation;
- inspect complete baseline→candidate diff, tests, CI and evidence;
- challenge claims rather than trust Worker prose, Worker pre-review conclusions or the Worker's predecessor classification;
- for foundational WPs, independently search for omission/false-green classes and challenge completeness **inside the WP claim and declared trust boundary**, including risks not highlighted by the Worker;
- before issuing FAIL for an apparent missing proof/coverage surface, check whether an accepted predecessor already owns that guarantee; if so, require concrete evidence of inapplicability or predecessor falsehood rather than duplicate proof;
- distinguish an in-claim material defect from a risk that requires arbitrary out-of-contract behavior of the declared trusted base, re-proving an accepted upstream claim, or using an unsupported path;
- not require discovery of a novel defect as a condition of a valid review;
- not FAIL solely because an explicitly trusted infrastructure component could theoretically behave outside its documented contract, or because an accepted predecessor guarantee is not redundantly re-proven, unless the current WP acceptance criteria place that component/guarantee inside the claim or factual evidence invalidates the inherited guarantee;
- never repair implementation;
- emit `PASS | FAIL | BLOCKED | READY_FOR_LOCAL_VALIDATION` naming the exact reviewed SHA.

A green Worker test suite and a clean Worker pre-review are necessary, never sufficient. Reviewer independence means independent judgment, not an obligation to expand the product's proof claim.

## FAIL

A FAIL must identify the violated criterion, evidence, expected behavior and minimal correction boundary. The same WP remains unresolved. Repair creates a new candidate SHA and requires a fresh Worker pre-review followed by a fresh independent review.

If the alleged defect falls on a boundary accepted by a predecessor, the FAIL must additionally identify why that binding predecessor guarantee does not cover the effective case or provide concrete evidence that the predecessor claim was false. Without that bridge, the issue is not a blocker for the downstream WP.

A Reviewer that emitted FAIL stops as Reviewer. It must not repair implementation in the same context; start a fresh repair Worker.

A local/trivial defect may be repaired locally by that fresh Worker. A finding that invalidates the proof boundary, architecture or completeness argument **inside the accepted claim** must be repaired at that causal boundary rather than by special-casing the reported example.

A finding outside the claim/trust boundary should normally be recorded as residual risk or proposed as a future hardening WP, not converted into an implicit expansion of the current WP.

## PASS / merge preflight / finalization

Merge is valid only when:

- the predecessor contract check required by the governing protocol exists and matches accepted dependency state;
- the frozen handoff recorded `Worker pre-review: CLEAN` when required by the governing protocol;
- independent PASS names the exact Frozen candidate SHA;
- required CI/evidence for that SHA is green/complete;
- foundational proof budget is within budget when applicable;
- no later implementation mutation exists;
- no blocking in-claim finding remains.

Once the Reviewer persists a valid PASS, the verdict is immutable for that candidate unless new factual evidence proves the prerequisites were false. The same session may then leave Reviewer mode and enter **FINALIZATION/DOCSYNC** mode. This is a one-way transition: it does not permit further implementation edits or repair work.

Finalization should immediately:

1. verify PR HEAD, Frozen candidate SHA and Reviewed candidate SHA still agree;
2. merge that exact SHA, or recognize an Automation V2 exact-SHA auto-merge as equivalent;
3. reconstruct current `main` after merge;
4. perform documentation-only DocSync for the accepted result;
5. resolve the next dependency-valid WP from current accepted state;
6. emit the durable `DOCSYNC_COMPLETE` marker on the merged implementation PR.

If merge is blocked by a code/implementation issue, if the SHA moved, or if DocSync reveals that the accepted implementation itself must change, stop. Do not patch implementation during finalization; reopen the proper Worker → Reviewer cycle.

Documentation-only post-merge reconciliation does not redefine the reviewed implementation SHA.

## Foundational circuit breaker

If two independent Reviewer FAILs expose the same foundational defect class, stop local patching and re-audit the foundation before another downstream repair loop.

A single FAIL proving that the universe used by an in-claim completeness assertion can self-shrink, omit material objects by construction, or circularly define its own proof set is already an architectural finding. Re-audit that proof boundary immediately before another implementation cycle.

Independently, if two consecutive repair/pre-review cycles materially expand proof/support machinery without corresponding progress in product behaviour, an explicit acceptance gap or a realistic in-boundary false-green class, stop and re-audit the claim/trust boundary under the proof-budget rule. Do not continue merely to accumulate negative scenarios.

Repeated attempts to re-prove an accepted predecessor guarantee without concrete contradictory evidence are themselves a proof-budget warning: stop the duplicate-hardening loop and restore the predecessor/current-WP ownership split.

## DocSync

DocSync is the documentation-only completion phase of a successful PASS/finalization flow, not a mandatory separate reasoning session.

After implementation merge, reconcile only affected surfaces: ROADMAP, WP state, architecture/ADR where changed, evidence/final verdict, `Docs/engineering/RESIDUAL_LEDGER.md`, and compact handoff. Then emit `DOCSYNC_COMPLETE` with the dependency-valid `Next WP`. Only after that marker may the next WP be selected.

Reconciling the residual ledger means appending the residuals this workpack declared and updating any entry it closed, citing the accepted evidence. It is a transcription step: DocSync records residuals, it does not classify contested ones and does not decide whether a residual falls inside the boundary.
