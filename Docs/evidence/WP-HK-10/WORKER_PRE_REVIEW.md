# WP-HK-10 Worker pre-review

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 7
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-10/WORKER_PRE_REVIEW.md

## Candidate challenged

- WP: `WP-HK-10 — Strict quality closure`.
- Baseline: `8651a7bd55180297c3621336e9e64a2e6a211aef` (accepted + DocSynced HK09B mainline).
- Branch: `wp/hk10-strict-quality-closure`.
- Direct accepted predecessor: `WP-HK-09B`.
- First frozen candidate: `dabcbeb9cce071ed6fca29a8fa01030127f2ce98`.
- Independent Reviewer verdict on that candidate: FAIL, review `#5261225652`.
- Repair cycle: `1`.
- Repaired executable checkpoint challenged before evidence finalization: `c3995412a83f6d66cc036120d2ff7bc67c674670`.
- Draft exact-SHA observation: Actions run `35526773478` GREEN.
- Repair checkpoint result: HK01 dispatch-failure owner proof `2/2` GREEN; focused HK10 `11/11` GREEN; representative content probe GREEN; all 12 causal controls RED as required; full regression `218/218` GREEN; clean before/after.
- This is Worker quality-gate evidence only. Fresh independent Reviewer PASS is still required on the final frozen SHA.

## Scope and causal-ownership challenge

The complete baseline→repair diff was reviewed against HK10's closure-only rule and the independent FAIL. Closure discovered a real undefined canonical-dispatch branch, but the public semantics are no longer claimed as HK10-owned hardening.

`WP-HK-01` is explicitly reopened/amended as the causal owner because HK01 defines canonical dispatch and the structured-error model. The repaired contract owns:

- `contract.handler_failure` for an escaping handler exception before authoritative publication, non-retryable by default with `publicationCommitted=false`;
- `contract.handler_failure_after_publication` after authoritative publication, with `publicationCommitted=true` and recovery guidance that requires inspecting the current canonical anchor before retry;
- diagnostic capability/exception-type context without leaking raw exception message/stack/internal sentinel text.

`Hk01DispatchFailureContractTests` is the owner-level proof and exercises both branches. `Hk10ExceptionBoundaryTests` remains only because HK10 explicitly requires handler/validator fault injection through accepted paths; those fixtures consume the HK01-defined result instead of defining it.

No new capability family, authoring authority, shell/network/filesystem primitive, persistence subsystem, alternate registry or merge/concurrency model is introduced. The HK01 amendment remains candidate semantics until the exact frozen repair candidate receives fresh independent PASS.

## Findings 1–4 retained from the first Worker cycle

### Finding 1 — thrown-handler proof fixture polluted the public route universe

The first exception-boundary fixture declared a synthetic `[PublicCapabilityRoute]`. Focused HK10 tests were green, but full regression correctly rejected the extra effective route. The repair removed the synthetic public route and exercises accepted `world.summary@1.0` with a throwing state source. The independent HK01 route-universe oracle was preserved, not weakened.

### Finding 2 — validator-throw fixture depended on inaccessible Runtime internals

A first validator-failure test attempted direct access to an internal validation handler and internal contract constructor, producing a compile failure. The repair uses test-side reflection to instantiate the already accepted validation handler and composes it through public `ContractComposer`. Production visibility was not widened.

### Finding 3 — inspection causal control was falsely green

Actions run `35524792972` stopped as `FALSE GREEN` on control #3 because reversing object-query output was accidentally cancelled by the selected fixture's input ordering. `Hk10InspectionOrderingTests.ObjectQueryOrderMatchesIndependentOrdinalIdOracle` now derives expected IDs independently. Run `35524945085` subsequently proved all twelve controls RED for the intended reason and the clean first-cycle checkpoint regression GREEN.

### Finding 4 — freeze evidence needed explicit v1.3 content-probe and verifier closure

The first cycle added an explicit rerun of the accepted Potes hero slice from `Docs/art/VISUAL_BIBLE.md` and bound content/proof/residual/endurance evidence into exact-SHA verification. The probe remains required for the repaired combined candidate because that candidate includes a foundational public-contract amendment, but the amendment's semantic owner is HK01 rather than HK10.

## Finding 5 — independent review found closure-only ownership violation

Independent review #5261225652 correctly found that frozen SHA `dabcbeb9cce071ed6fca29a8fa01030127f2ce98` introduced a material public semantic decision inside HK10. `ComposedContract.Dispatch` converted escaping handler exceptions into structured pre/post-publication failures with retry/publication/recovery semantics, while HK10's contract requires material product semantics to be reopened or amended at their causal owner.

The behavior itself was not rejected; its ownership was. Repair cycle 1 therefore amends `Docs/workpacks/HK/WP-HK-01.md`, records `Docs/evidence/WP-HK-01/DISPATCH_FAILURE_AMENDMENT.md`, adds owner-level `Hk01DispatchFailureContractTests`, and runs that owner proof before HK10 closure tests. The existing 12-control HK10 mutation universe is intentionally not expanded merely to duplicate HK01's proof.

## Finding 6 — first HK01 owner fixture polluted HK01 route enumeration

Draft repair run `35526531736` on SHA `530a55e81ebe1346917240ad779ba216d2bf0746` showed the new HK01 owner tests themselves `2/2` GREEN, HK10 `11/11` GREEN and all twelve negative controls RED, but full regression failed `Hk01CanonicalContractTests.AcceptedSyntheticScopedSurfaceIsIndependentlyEnumerableAndConformant`.

Cause: two nested fixture classes statically implemented `ICanonicalCapabilityHandler`; HK01's independent `RouteUniverse` correctly treated those implementation types as binding-metadata candidates and reported `route-universe.binding-metadata-count`. The repair did **not** weaken route enumeration. The static handler fixture types were replaced with a `DispatchProxy`-generated implementation, leaving no extra statically enumerable handler implementation in the test assembly.

## Finding 7 — first DispatchProxy fixture was accidentally sealed

Draft repair run `35526700147` on SHA `1eec4e2f6fe66ede963425cb8b8bdde8b69502ea` failed immediately in the two HK01 owner tests because the `DispatchProxy` base fixture was declared `sealed`, which the runtime forbids for generated proxy derivation. This was a pure test-fixture construction error; production code and semantic assertions were untouched. Commit `c3995412a83f6d66cc036120d2ff7bc67c674670` makes the proxy base derivable.

Run `35526773478` then proved the intended result end-to-end: the two owner tests pass, the original HK01 route-universe conformance also passes in full regression, and no production visibility or registry rule was relaxed.

## Architecture and false-green challenge

The repaired pre-review challenged:

- HK01, not HK10 prose/tests, is the authoritative source for the two handler-failure machine codes and pre/post-publication retry semantics;
- post-publication failure cannot falsely advertise “no effect” and directs the client to inspect the current canonical anchor before retry;
- raw exception message/stack/internal sentinel text does not leak into public error message/repair guidance;
- HK01 owner fixtures cannot create a synthetic public route or static handler-binding obligation merely to prove the contract;
- HK10 handler/validator fixtures consume, rather than redefine, the HK01 behavior;
- canonical serialization/hash dependence on input order;
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

All twelve material seeded controls on repair checkpoint `c399541...` produce actual test RED rather than compiler/tool failure. The disposable mutation worktree returns to the exact candidate after every control.

## Endurance, compatibility and content reconciliation

Run `35526773478` exercised 512 canonical transactions and emitted `journalBytes=491028`, `workingSetBefore=91459584`, `workingSetAfter=121356288`, `maximumWorkingSet=121303040`, `managedBefore=2097904`, `managedAfter=7127384`, `maximumManaged=17093536`. These remain observations, not SLOs. Periodic inspect/validate/journal/snapshot operations and fresh-session snapshot recovery stay below accepted HK09B ceilings.

The Protocol v1 corpus stores accepted limits and identities as literals independent of runtime constants. Same-major breaking evolution remains governed by the canonical composer. Residual reconciliation remains complete with `UNCLASSIFIED_RESIDUALS: 0`.

The explicit Potes content-shape probe passes on the combined repaired candidate. It checks product-shaped representability across mutation, inspection, snapshot and rebase without promoting gameplay/H1 semantics; the separately owned HK01 amendment affects failure definition, not successful content representation.

## Validation reconciliation and handoff condition

Repair checkpoint execution:

- SHA `c3995412a83f6d66cc036120d2ff7bc67c674670`;
- Actions run `35526773478`: GREEN;
- HK01 owner proof: `2/2` GREEN;
- focused HK10: `11/11` GREEN;
- representative content probe: GREEN;
- causal controls: `12/12` RED as required, runner GREEN;
- full regression: `218/218` GREEN;
- exact checkout clean before and after: YES.

This pre-review evidence commit changes HEAD after the executable checkpoint. Therefore its exact evidence-finalized HEAD must receive one final Draft observation. Only after that GREEN may the exact HEAD be recorded as Candidate/Frozen SHA, metadata switch to `FROZEN_FOR_REVIEW`, and the PR become Ready. The Ready transition must then receive GREEN frozen exact-SHA verification. No Worker implementation/evidence write is permitted after freeze.

No known in-boundary Worker blocker remains.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET
