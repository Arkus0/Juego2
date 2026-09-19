# WP-HK-05 Worker pre-review

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 7
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-05
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

Baseline: `dbd8121411079f22a01d5cb85345e180ff41f7e2`

Latest complete implementation/evidence observation before this report: exact SHA `a2196550c07caeac851e7f81d1edb9d4fe31ef60`, Actions `35460829594` — Release build 0 warnings / 0 errors, focused HK05 7/7 GREEN, full regression 106/106 GREEN, canonical receipt `Result: GREEN`.

This report is the final branch-content reconciliation. Its resulting exact SHA must receive a fresh canonical observation and exact-SHA freeze validation before handoff; no later Worker branch write is permitted after freeze.

## Contract and predecessor re-check

Re-read `WP-HK-05`, `AGENTS.md`, `WORKER_REVIEW_PROTOCOL.md` v1.6, `FOUNDATIONAL_PROOF_STANDARD.md` v1.3, the direct accepted HK02A contract/evidence, and the inherited HK01/HK02/HK03/HK04 guarantees materially consumed by HK05.

The `PREDECESSOR_CONTRACT_CHECK` in `WORKER_PLAN.md` remains accurate:

- direct predecessor HK02A reviewed SHA `f39a1994524c42213dafb63d440faaf9de7c040f`, independent PASS review `5256593405`, merge `ac7ce1180b462f093cf0ee02bbbd6f853938e27c`;
- HK05 consumes accepted route composition/discovery, finite canonical state/codec, inspection, and closed transactional commit authority;
- HK05 newly owns aggregate validator/diagnostic completeness, explicit current/proposed validation and validation-before-commit behavior;
- no concrete evidence falsified a predecessor guarantee. The CI authority failure instead proved that HK05 initially violated HK04's already accepted boundary, and the repair now reuses that boundary correctly.

## Complete baseline-to-candidate audit

The complete `dbd8121…` → candidate diff was inspected, not only the last repair. Product changes are confined to:

- `Arkus.Game.World`: explicit finite invariant catalog, candidate shape and aggregate invariant evaluation while preserving throw-first compatibility for legacy constructor/API boundaries;
- `Arkus.Game.Validation`: stable/versioned diagnostic/result model, validator inventory/reconciliation and deterministic aggregate conversion;
- `Arkus.Game.Authoring`: explicit validation service/contract and reuse of the same candidate validation before plan/dry-run/apply materialization;
- `Arkus.Harness.Runtime`: two canonical validation routes, bound through the existing HK04 non-committing attenuation facade;
- tests: independent 17-invariant universe, effective fixtures, required causal mutants, representative Potes probe, and extension of inherited HK03/HK04 conformance universes to the new routes;
- exact-SHA scripts, lockfile reconciliation and Worker evidence.

No Unity/engine validation, AI-generated repair prose, gameplay-specific invariant system, journal/replay, arbitrary filesystem mutation, new concurrency model, alternate commit authority or external validation framework was introduced.

## Strict falsification performed

### Completeness could self-shrink

Challenge: production catalog and production validator registry could omit the same invariant and still agree.

Result: fixed by a test-owned independent exact 17-ID universe. It must equal both `WorldInvariantCatalog.All` and the validator inventory. Separate effective fixtures require every one of those IDs to occur in real diagnostics. Removing one copied descriptor turns the inventory oracle RED.

### Registration could exist without effective behavior

Challenge: one descriptor per ID might still point to ineffective validator behavior.

Result: `EveryOwnedInvariantHasAnEffectiveDiagnosticFixture` observes the complete independent ID set from actual invalid candidates, so registration-only false green is rejected.

### Diagnostics could be unstable or not actionable

Challenge: aggregation could reorder with caller input or return ambiguous location/context.

Result: semantically equivalent reversed-input candidates produce identical ordered signatures; an injected reversed signature sequence is detected. All effective diagnostics require resource, `$/...` path, invariant ID, machine code and non-empty remediation context; an injected ambiguous diagnostic is detected.

### Explicit validation could disagree with apply

Challenge: `authoring.change.validate` could use a different semantic path or apply could skip validation.

Result: public explicit validation and public apply are independently dispatched for the same invalid request and their ordered diagnostic signatures must match. Synthetic success, missing validation context and a different invalid candidate each turn the comparison oracle RED. Revision/hash stay unchanged.

### A new public mutation route could escape validation

Challenge: testing only the named apply handler could miss another accepted canonical mutation definition.

Result: HK05 consumes HK01's accepted composed definition universe and executes the invalid request against every effective definition whose `SideEffect == CanonicalMutation`. No second hand-maintained mutation-route list is introduced.

### Expected invalid input could escape as an exception

Challenge: malformed or invalid authoring requests might throw instead of returning machine-readable results.

Result: malformed public proposed validation is rejected by the canonical schema/parser layers as structured `contract.invalid_request` or `world.change.invalid_request`; invalid complete candidates return aggregate diagnostics / `world.change.invalid_candidate`. An injected throwing callback is detected by the exception oracle.

### Read-only validation handler could accidentally carry commit authority

Challenge: the first implementation stored the authoritative `TransactionalWorldAuthoringSession` directly behind validation handlers.

Result: HK04's accepted `CapabilityRoute` authority guard correctly rejected that candidate in Actions `35460353246`. The repair does not weaken or special-case the inspector; validation now receives the already accepted `WorldMutationPlannerView` attenuation facade, while only canonical apply receives `ICanonicalWorldMutationCommitter`. Regression is GREEN.

### New routes could fall outside predecessor conformance universes

Challenge: adding validation routes changed the effective public surface while inherited HK03/HK04 tests still enumerated the old route set.

Result: full regression caught both omissions. HK04's effective non-mutation vector now executes both validation routes and proves canonical hash/revision unchanged. HK03's world-read discovery universe now includes `world.validation.current`. The predecessor assertions were expanded, not relaxed.

### Representative product shape could expose a missing semantic boundary

Challenge: abstract `node.*` fixtures might hide a granularity/reference problem relevant to the approved game target.

Result: the bounded Potes/Liébana probe models plaza + bar + shop + NPC + typed declared dependencies using only the existing generic model. Current validation succeeds; removing the referenced bar produces object and extension diagnostics; apply fails closed with unchanged state. No gameplay schema or Unity assumption is promoted into H0.

## Findings fixed before CLEAN

1. Test project lockfile did not include the new Authoring → Validation project dependency; locked restore caught it.
2. Initial focused tests referenced internal/nonexistent schema helpers instead of the public schema surface; tests now use `SemanticFingerprint()` and `RequiredProperties`.
3. Validation handlers directly carried the authoritative session, violating HK04 commit-authority attenuation; repaired by reusing `WorldMutationPlannerView` without weakening the guard.
4. The malformed-request test assumed service-layer `world.change.invalid_request` even when HK01 schema validation correctly rejects earlier as `contract.invalid_request`; the assertion now verifies the structured expected-error contract across the accepted layers.
5. The required v1.3 representative content-shape probe was made executable and documented rather than left implicit.
6. HK04's exhaustive non-mutation request-vector universe lacked the two new validation routes; both are now included and effectively executed against the live authoritative session.
7. HK03's exhaustive `world.*` read inventory lacked `world.validation.current`; it now includes the new public read and retains schema/read-only/determinism checks.

## Rejection sites, residual risk and proof budget

The HK05 diff's newly introduced expected public rejection sites are classified in `NEGATIVE_CONFORMANCE_MATRIX.md`; inherited HK01 schema and HK04 mutation errors are consumed rather than copied into a second taxonomy. No known HK05 expected-error path uses an escaping runtime exception as its public contract.

`RESIDUAL_RISK.md` names only out-of-claim risks: direct CLR programmer misuse, semantics hidden in opaque payload bytes, future gameplay/Unity invariants, total-size/performance budgets, future invariant growth and arbitrary trusted-infrastructure failure.

Proof machinery remains proportional: one validation component, two routes, two focused test files and thin evidence, while route/commit completeness continue to come from accepted predecessor mechanisms. The authority defect was repaired by reuse, not another generalized defensive framework.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.

## Handoff readiness

No known in-claim blocker remains. The exact pre-report candidate `a2196550c07caeac851e7f81d1edb9d4fe31ef60` is canonically GREEN. After this report commit, the resulting SHA must itself become GREEN, then be recorded unchanged as Candidate/Frozen SHA, with PR state `FROZEN_FOR_REVIEW`, `Branch frozen: YES` and the PR marked Ready for a fresh independent Reviewer.

This Worker does not issue the independent PASS/FAIL verdict.
