# WP-HK-10 Worker pre-review

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 4
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-10/WORKER_PRE_REVIEW.md

## Candidate challenged

- WP: `WP-HK-10 — Strict quality closure`.
- Baseline: `8651a7bd55180297c3621336e9e64a2e6a211aef` (accepted + DocSynced HK09B mainline).
- Branch: `wp/hk10-strict-quality-closure`.
- Direct predecessor: accepted `WP-HK-09B`; HK10 newly owns bounded-session endurance/growth closure, while accepted HK09B resource/publication semantics are consumed unless concrete tests contradict them.
- Latest code/test checkpoint challenged before evidence finalization: `04b77e0c792d21e2372255011c6a7c15daec7b11`.
- Exact-SHA observation: Actions run `35524945085` GREEN, including 11 focused HK10 tests, all 12 causal RED controls, and full regression `216/216`.
- This is Worker quality-gate evidence only. A fresh independent Reviewer is still required on the final frozen SHA.

## Scope and predecessor challenge

The complete baseline→candidate diff was reviewed against HK10's closure-only rule. The only production behavior change is the canonical dispatcher exception boundary: an unexpected handler/validator exception is converted into a structured public error, with a distinct after-publication code so the runtime does not falsely claim that an authoritative publication did not occur. No new capability family, authoring authority, shell/network/filesystem primitive, persistence subsystem or merge/concurrency model is introduced.

HK01–HK09B accepted guarantees remain predecessor evidence. HK10 attacks them only where its contract explicitly requires causal controls or where concrete execution exposed a false-green proof path. Residuals requiring per-resource merge/CAS, distributed writers, WAL/fsync/power-loss, hard resource preemption, remote authentication, shipping SLOs, arbitrary large-world streaming, future-version migration or H1 engine/gameplay semantics remain named outside H0 in `RESIDUAL_RISK.md`.

## Finding 1 — thrown-handler proof fixture polluted the public route universe

The first exception-boundary fixture declared a synthetic `[PublicCapabilityRoute]`. Focused HK10 tests were green, but full regression correctly rejected the extra effective route. The repair removed the synthetic public route and exercises accepted `world.summary@1.0` with a throwing state source. The independent route-universe oracle was preserved, not weakened.

## Finding 2 — validator-throw fixture depended on inaccessible Runtime internals

A first validator-failure test attempted direct access to an internal validation handler and internal contract constructor, producing a compile failure. The repair uses test-side reflection to instantiate the already accepted validation handler and composes it through public `ContractComposer`. Production visibility was not widened and the same structured-error oracle remains.

## Finding 3 — inspection causal control was falsely green

Actions run `35524792972` proved the defect-injection runner itself could fail closed: controls #1 and #2 went RED, but control #3 stopped as `FALSE GREEN`. The mutation reversed object-query output while the selected HK03 fixture used reversed input ordering that accidentally cancelled the defect.

The repair added `Hk10InspectionOrderingTests.ObjectQueryOrderMatchesIndependentOrdinalIdOracle`. Its expected IDs are derived independently from source state and ordinal sorting, not from the implementation result. Run `35524945085` then produced `HK10_NEGATIVE_RED inspection` and all twelve causal controls completed RED as intended before the clean candidate passed full regression.

This was a material proof-oracle finding and is now protected by regression.

## Finding 4 — freeze evidence needed explicit v1.3 content-probe and verifier closure

Strict pre-review re-read `FOUNDATIONAL_PROOF_STANDARD.md` v1.3 and noted that HK10 changes public dispatch failure semantics, so a representative content-shape probe must be explicit even though HK10 does not change authorable-state shape. The repair adds an explicit canonical-observation rerun of the accepted Potes hero slice from `Docs/art/VISUAL_BIBLE.md` and records classification in `CONTENT_SHAPE_PROBE.md` instead of inventing a second content model. The final exact-SHA verifier also binds proof, residual, compatibility, content-probe, endurance and pre-review evidence before declaring GREEN.

## Architecture and false-green challenge

The pre-review challenged:

- handler/validator exceptions leaking raw internal output or disappearing from the structured protocol;
- post-publication failure being mislabeled as a no-effect retry;
- canonical serialization/hash depending on input order;
- query repeatability passing while canonical result order is wrong;
- idempotency receipt lookup silently missing accepted receipts;
- validator aggregation suppressing owned violations;
- replay accepting a supplied current anchor not equal to the final chain result;
- JSONL v1 framing drifting without a version change;
- HK08A 96-operation shape drifting with its oracle;
- HK08B recovery expected/current anchors being misreported;
- HK09A elevated authority being admitted;
- HK09B publication interruption or session limits drifting undetected;
- proof controls failing because of build/tool/fixture errors rather than causal product tests;
- residual classification silently converting post-GATE/H1 product decisions into HK10 blockers.

All twelve material seeded controls now produce actual test RED rather than compiler/tool failure. The disposable mutation worktree returns to the exact candidate after every control.

## Endurance, compatibility and content reconciliation

Run `35524945085` exercised 512 canonical transactions and emitted `journalBytes=491028`, `workingSetBefore=91475968`, `workingSetAfter=178704384`, `managedBefore=2240376`, `managedAfter=7114160`. These are observations, not SLOs. The executable case also performs periodic inspect/validate/journal/snapshot operations and fresh-session snapshot recovery while staying below the accepted HK09B session/state ceilings.

The Protocol v1 corpus stores accepted limits and identities as literals independent of runtime constants. Same-major breaking evolution remains governed by the inherited canonical composer. The residual reconciliation classifies every v1.9 ledger entry with `UNCLASSIFIED_RESIDUALS: 0`; only `R-06A-05` is newly closed by HK10 bounded-session evidence.

Because public failure semantics changed, the explicit Potes content-shape probe is part of final observation. It checks current product-shaped representability across mutation, inspection, snapshot and rebase without promoting gameplay/H1 semantics.

## Validation reconciliation and handoff condition

Latest pre-finalization executable checkpoint:

- SHA `04b77e0c792d21e2372255011c6a7c15daec7b11`;
- Actions run `35524945085`: GREEN;
- focused HK10: 11/11 GREEN;
- causal controls: 12/12 RED as required, runner GREEN;
- full regression: 216/216 GREEN.

The evidence/verifier commit produced after this report must itself receive a clean Draft exact-SHA observation including the newly explicit content-shape gate. Only then may its exact HEAD be recorded as Candidate/Frozen SHA, PR metadata switch to `FROZEN_FOR_REVIEW`, and the PR become Ready. The Ready transition must then receive GREEN frozen exact-SHA verification. No Worker implementation/evidence write is permitted after that freeze.

No known in-boundary Worker blocker remains.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET
