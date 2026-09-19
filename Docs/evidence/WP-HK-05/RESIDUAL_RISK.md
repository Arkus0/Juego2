# WP-HK-05 trust boundary and residual risk

## Claim boundary

HK05 claims that, for the current finite engine-neutral canonical `WorldState`/`WorldStateCandidate` model and the accepted public H0 contract:

- every currently owned canonical world invariant is represented in an independently checkable finite universe and has one registered validator descriptor;
- effective validation aggregates stable deterministic structured diagnostics;
- current state and proposed mutation results can be validated explicitly through discoverable/versioned capabilities;
- every accepted public capability classified as `CanonicalMutation` rejects a proposal that explicit validation rejects, before canonical state changes;
- expected invalid-request/invalid-state behavior is a structured public result rather than an escaping runtime exception.

When object or extension identity is duplicated, HK05 reports the identity defect at a concrete collection index and defers a secondary diagnostic only when that diagnostic's own source identity or specific graph dependency chain requires choosing one ambiguous representative. Independent uniquely identified components remain visible in the same validation pass. This dependency-local ambiguity policy is part of the claimed deterministic validation semantics, not an unreported residual.

## Trusted / inherited base

Per `FOUNDATIONAL_PROOF_STANDARD.md`, normal pinned .NET/MSBuild/NuGet, Git checkout/object semantics, filesystem/runner and cryptographic primitives are trusted infrastructure.

Accepted predecessor guarantees are consumed:

- HK01: canonical capability composition, schema validation, discovery/dispatcher reconciliation and independently enumerable public route universe;
- HK02: canonical finite world identity/model/codec/hash and base invariant ownership;
- HK03: bounded side-effect-free inspection semantics;
- HK04: closed canonical commit authority, transactionality, CAS/idempotency, public mutation-surface reconciliation and semantic plan/change coverage;
- HK02A: object-scoped extension identity/dependency semantics and referential propagation.

Concrete evidence that an accepted predecessor route or commit path is missing from its accepted universe would reopen that predecessor. Neither Reviewer FAIL `5257015987` on `e0c865…` nor FAIL `5257239932` on `8c8d8c…` provided such evidence; both exposed HK05-owned aggregation semantics after a candidate was already invalid for duplicate identity.

## In-boundary residuals

None known after repair-cycle-2 architecture re-audit and pre-review.

Three previously observed HK05 defects are closed at their causal boundaries:

- the validation-route authority leak was repaired by reusing HK04's attenuation facade rather than weakening/re-proving HK04;
- ambiguous identity now produces index-addressable duplicate diagnostics and defers only source/traversal-dependent secondaries;
- world-wide cycle suppression was removed: a disjoint unique-ID containment cycle remains aggregated even when another component contains a duplicate ID.

Deferred secondary diagnostics are intentionally not claimed to be visible in the same validation pass when their own resource or dependency traversal is ambiguous. The contract is iterative only for those affected diagnostics: repair the concrete identity ambiguity and revalidate. Unrelated independently actionable violations must not be hidden behind that repair step.

## Non-blocking residuals outside HK05

- **Constructor/programmer misuse:** direct in-process calls with null arguments may still throw ordinary argument exceptions. HK05 claims structured errors for expected public capability requests, not for arbitrary misuse of internal/public CLR APIs outside the dispatcher contract.
- **Opaque payload semantics:** validators cannot infer references or gameplay meaning hidden inside extension payload bytes. HK02A requires declared dependencies; undeclared payload meaning is outside current canonical invariant ownership.
- **Gameplay/content invariants:** schedules, economy, AI behavior, quest logic, transforms, navmesh and final gameplay rules are not owned by the current micro-world model and are explicitly forbidden/future scope.
- **Unity/engine validation:** engine object existence, scene serialization, prefab/component rules and editor/runtime constraints are outside HK05.
- **Whole-world size/performance budgets:** deterministic finite validation is claimed; global content-size or latency budgets are not yet product guarantees.
- **Future invariant growth:** adding a new canonical world invariant requires extending the independent test-owned universe/catalog/registration/fixture evidence. The current proof does not claim future source changes are automatically complete.
- **Arbitrary trusted-infrastructure failure:** compiler/runtime/CI behavior outside documented contracts is not re-proved.

## Proof-budget assessment

HK05 adds one validation component, two public validation routes, focused validation/content-shape tests, one compact ambiguous-identity regression class and thin evidence. It reuses accepted HK01 route enumeration and HK04 attenuation/commit closure.

Repair cycle 2 does not introduce generalized provenance or a second graph engine. It replaces one over-broad boolean policy with a bounded dependency-local check and adds one regression that directly demonstrates the Reviewer-observed false negative. That is product/acceptance progress rather than support-machinery growth.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.
