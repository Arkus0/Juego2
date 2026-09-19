# WP-HK-04 Worker plan

Baseline SHA: `ecebd054821eeb388c3cf6ee4e389545d762b6d8`  
Worker: `ChatGPT / GPT-5.6 Sol (repair cycle 1)`

State: `ACTIVE`

## PREDECESSOR_CONTRACT_CHECK

Accepted direct predecessor: `WP-HK-03 — Inspection + query surface`.

- Reviewed candidate SHA: `8c20a380003c082fa9bd472d3233afa9654fb231`
- Independent Reviewer: `PASS` on PR #18.
- Implementation merge SHA: `d8b808450ee7863726d718a25c5756534113fcfd`
- DocSync/main baseline making HK04 dependency-valid: `ecebd054821eeb388c3cf6ee4e389545d762b6d8`
- HK03 proof: `READY`, unresolved obligations `0`, known undetected defect classes `0`, proof budget `WITHIN_BUDGET`.

### Inherited guarantees consumed

1. HK02 owns the current finite canonical `WorldState` model, referential validation, immutable-by-copy semantics and deterministic canonical SHA-256 identity.
2. HK01 owns the single canonical capability definition/composition/discovery path and structured schema validation.
3. HK03 owns complete bounded deterministic reads over current canonical state, explicit revision/hash anchors and side-effect-free inspection.
4. HK03's independent effective public-route universe remains the completeness oracle for production routes; HK04 extends it rather than creating another registry.

These guarantees are not re-proved unless concrete evidence shows one is false or inapplicable to an effective HK04 path.

### Guarantees newly owned by HK04

- deterministic proposed plans/change sets before persistence;
- one semantic planning/validation path shared by plan, dry-run and apply;
- dry-run without persistence;
- atomic whole-state commit after complete candidate validation;
- optimistic expected revision + hash checks;
- explicit idempotency-key replay/conflict semantics;
- machine-readable affected resources/fields/references and evaluated conditions;
- no public mutation handler outside the canonical transactional pipeline;
- mechanical equality of effective mutation routes, discovered mutation definitions and transactional handler surface.

### Reopen conditions

Reopen inherited HK01/HK02/HK03 only if concrete evidence shows, for example, state required by mutation is outside the accepted state model, a public handler escapes canonical composition, or committed state cannot be described by accepted read/hash semantics.

## Implementation boundary

`Arkus.Game.Authoring` owns engine-neutral mutation planning, validation and the transactional authoring session over `Arkus.Game.World`. `Arkus.Harness.Runtime` owns canonical bindings/composition only. Existing inspection reads the same session through `IWorldStateSource`. No Unity, filesystem writes, gameplay-specific authoring, undo UI, transport host or natural-language mutation is added.

## Planned surface

The accepted HK03 read namespace remains `world.*`. HK04 adds a distinct authoring namespace while entering the same HK01 canonical inventory:

- `authoring.change.plan@1.0`: validate and return deterministic plan + predicted result revision/hash, no persistence.
- `authoring.change.dry-run@1.0`: same semantic planner/validator, explicit non-persisted outcome.
- `authoring.change.apply@1.0`: same planner for new requests, then atomic compare-and-swap commit with idempotency.

The envelope carries `idempotencyKey`, `expectedRevision`, `expectedHash` and ordered generic operations: `put-object`, `remove-object`, `put-extension`, `remove-extension`. Object replacement carries type/container/references as one canonical object value; no gameplay semantics are introduced.

Planning copies one immutable base state into private working collections, applies every operation, and constructs one validated `WorldState` at `baseRevision + 1`. No session state changes during planning. Apply commits only the completed candidate inside one critical section if revision/hash still match. Exact accepted retries replay the stored receipt; the same key with different request semantics fails closed.

The production session exposes no public state replacement API: `_current` replacement remains private inside successful apply. The public `IWorldMutationService` surface now exposes planning/dry-run only. Runtime gives those handlers a sealed attenuated planner view; only the transactional apply handler receives the internal Authoring-owned `ICanonicalWorldMutationCommitter` capability through the explicit Runtime friend boundary.

Repair cycle 1 additionally treats the concrete `TransactionalWorldAuthoringSession` as effective write authority because its public `Apply` method remains callable for direct host/test use. `CapabilityRoute` rejects a non-transactional handler whose effective object graph carries that session or the internal commit capability. `MutationSurfaceConformance` independently reflects authority-bearing handler structure without consulting `SideEffect`, so a route cannot disappear from both sides merely by remaining declared `ReadOnly`.

## Foundational proof approach

The finite claim universe is the three HK04 public routes, four generic operation kinds, accepted HK02 state and the transactional authoring session reachable through those routes.

Independent/evaluated oracles:

1. independently enumerate effective production route handlers, transactional markers and structurally authority-bearing handlers, then reconcile all three with discovered canonical-mutation definitions and dispatcher keys;
2. compare dry-run predicted canonical result hash with the state actually committed by apply from the same base;
3. hash state before a multi-operation request whose later operation is invalid and prove no partial change persists;
4. independently diff base vs candidate state and require every effective resource/field/reference change to appear in the returned change set.

Required causal controls cover partial apply, stale writer, duplicate retry, a real `ReadOnly` public handler that commits canonical state outside transaction handling, dry-run/apply divergence, and an effective state change omitted from the plan. The original declaration/marker mismatch remains as a supplementary metadata control; it is no longer the evidence for the hidden-mutation-bypass claim.
