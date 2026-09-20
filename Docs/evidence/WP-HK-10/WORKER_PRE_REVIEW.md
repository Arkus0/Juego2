# WP-HK-10 Worker pre-review

WORKER_PRE_REVIEW: NOT_READY
WORKER_PRE_REVIEW_FINDINGS_FIXED: 5
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-10/WORKER_PRE_REVIEW.md

## Candidate challenged

- WP: `WP-HK-10 — Strict quality closure`.
- Baseline: `8651a7bd55180297c3621336e9e64a2e6a211aef` (accepted + DocSynced HK09B mainline).
- Branch: `wp/hk10-strict-quality-closure`.
- Direct accepted predecessor: `WP-HK-09B`.
- First frozen candidate: `dabcbeb9cce071ed6fca29a8fa01030127f2ce98`.
- Independent Reviewer verdict on that candidate: FAIL, review `#5261225652`.
- Repair cycle: `1`; PR returned to Draft + ACTIVE and the previous `WORKER_PRE_REVIEW: CLEAN` is invalidated.
- Current repair boundary: amend HK01-owned canonical dispatch/error semantics, then make HK10 consume that amendment as closure evidence.

This file intentionally remains `NOT_READY` while the repaired exact candidate is still mutable. It may return to `CLEAN` only after the amended HK01 owner proof, HK10 closure tests, 12 causal controls, representative content probe and full regression are GREEN on one exact repair SHA and this pre-review is rerun against that complete diff.

## Findings 1–4 from the first Worker cycle

### Finding 1 — thrown-handler proof fixture polluted the public route universe

The first exception-boundary fixture declared a synthetic `[PublicCapabilityRoute]`. Focused HK10 tests were green, but full regression correctly rejected the extra effective route. The repair removed the synthetic public route and exercises accepted `world.summary@1.0` with a throwing state source. The independent HK01 route-universe oracle was preserved, not weakened.

### Finding 2 — validator-throw fixture depended on inaccessible Runtime internals

A first validator-failure test attempted direct access to an internal validation handler and internal contract constructor, producing a compile failure. The repair uses test-side reflection to instantiate the already accepted validation handler and composes it through public `ContractComposer`. Production visibility was not widened.

### Finding 3 — inspection causal control was falsely green

Actions run `35524792972` stopped as `FALSE GREEN` on control #3 because reversing object-query output was accidentally cancelled by the selected fixture's input ordering. `Hk10InspectionOrderingTests.ObjectQueryOrderMatchesIndependentOrdinalIdOracle` now derives expected IDs independently. Run `35524945085` subsequently proved all twelve controls RED for the intended reason and the clean candidate regression GREEN.

### Finding 4 — freeze evidence needed explicit v1.3 content-probe and verifier closure

The first cycle added an explicit rerun of the accepted Potes hero slice from `Docs/art/VISUAL_BIBLE.md` and bound content/proof/residual/endurance evidence into exact-SHA verification. The probe remains required for the repaired combined candidate because that candidate includes a foundational public-contract amendment, but the amendment's semantic owner is now HK01 rather than HK10.

## Finding 5 — independent review found closure-only ownership violation

Independent review #5261225652 correctly found that the first frozen candidate introduced a material public semantic decision inside HK10: `ComposedContract.Dispatch` converted escaping handler exceptions into `contract.handler_failure` / `contract.handler_failure_after_publication` with retry/publication/recovery semantics, while HK10's own contract says material product semantics must be reopened or amended at their causal owner.

The behavior itself was not rejected. The ownership was wrong.

Repair cycle 1 therefore:

- amends `Docs/workpacks/HK/WP-HK-01.md`, because HK01 owns canonical dispatch and the structured-error model;
- records the causal amendment in `Docs/evidence/WP-HK-01/DISPATCH_FAILURE_AMENDMENT.md`;
- adds owner-level `Hk01DispatchFailureContractTests` covering both before-publication and after-publication exceptions without adding a synthetic public route;
- runs that HK01 owner proof before all HK10 closure tests in `hk10-observe-exact-sha.sh`;
- keeps `Hk10ExceptionBoundaryTests` only as required handler/validator fault injection through accepted real routes;
- does not add a thirteenth HK10 mutation control or another semantic subsystem.

The amendment is not treated as historically accepted HK01 evidence. It must receive fresh independent PASS on the same exact repaired candidate before HK10 may consume it as binding semantics.

## Repair-cycle architecture challenge

Before declaring this repair clean, pre-review must verify all of the following on the complete baseline→repair diff:

- HK01, not HK10 prose/tests, is the authoritative source for the two handler-failure machine codes and their pre/post-publication retry semantics;
- the post-publication branch cannot falsely advertise “no effect” and directs the client to inspect the current canonical anchor before retry;
- raw exception message/stack/internal sentinel text does not leak into public error message/repair guidance;
- no extra public route/capability/version, mutation authority, transport behavior or persistence subsystem was introduced by the amendment;
- HK10 fault fixtures consume the HK01 behavior rather than redefining it;
- the existing 12-control negative-conformance universe remains causal and has not been inflated merely to re-prove HK01;
- inherited HK04–HK09B guarantees remain consumed unless concrete execution contradicts them;
- content-shape, endurance, compatibility and residual claims remain unchanged except for the explicit ownership correction;
- exact-SHA evidence is regenerated after the repair and not reused from frozen SHA `dabcbeb9...`.

## Historical first-cycle validation

Before the independent FAIL, run `35524945085` on checkpoint `04b77e0c792d21e2372255011c6a7c15daec7b11` was GREEN with focused HK10 tests, 12/12 causal RED controls and full regression `216/216`; the later first frozen candidate also passed exact-SHA verification. Those runs remain useful historical evidence for the unaffected closure machinery but cannot validate repair-cycle ownership or the new HK01 owner proof.

## Handoff condition

The repair remains mutable and must not be frozen yet. Required sequence:

1. obtain one clean Draft exact-SHA observation of the repaired code/evidence, including `Hk01DispatchFailureContractTests` first;
2. rerun this strict Worker pre-review against that exact repaired candidate;
3. if no blocker remains, change this marker to `WORKER_PRE_REVIEW: CLEAN` and bind the resulting evidence-only HEAD;
4. obtain a clean Draft exact-SHA observation for that evidence-finalized HEAD;
5. record exact Candidate/Frozen SHA, set `FROZEN_FOR_REVIEW` / `Branch frozen: YES`, mark Ready and require GREEN frozen exact-SHA verification;
6. stop all Worker writes and hand the new exact SHA to a fresh independent Reviewer.

No known additional semantic expansion is justified by the current FAIL. The sole repair target is causal ownership plus proof of the already chosen dispatcher behavior.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET
