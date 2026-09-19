# WP-HK-01 Worker plan

Status: READY FOR FREEZE — repair cycle 1

Baseline main: `9db7a1ffffa02c02af7889de29960de1f19d2755`  
Worker: ChatGPT / GPT-5.6 Sol (initial + fresh repair cycle 1)

## Goal

Implement the canonical Arkus capability contract, base + scoped composition boundary, machine-readable discovery and canonical dispatch/schema validation required by `WP-HK-01`, without introducing world semantics, transport framing, Unity/engine APIs or gameplay commands.

## Claim and trust boundary

HK01 claims completeness over the Arkus public capability surface admitted through the canonical runtime boundary. Git/checkout semantics, the pinned .NET toolchain/runtime, normal reflection semantics for the candidate assemblies and the CI runner are trusted infrastructure under `FOUNDATIONAL_PROOF_STANDARD.md`.

The proof universe will not be sourced only from discovery/registration metadata. The conformance suite will independently enumerate concrete public capability route implementations/bindings in the tested assemblies/provider fixtures and compare that effective surface against the composed canonical inventory, discovery projection and request/success/error schema surface.

## Implementation shape

- `Arkus.Harness.Protocol` owns portable capability identity/version, JSON-Schema-shaped contract documents, structured errors, side-effect/determinism/concurrency/batching/repair/policy/provider metadata, compatibility rules and discovery projection.
- `Arkus.Harness.Runtime` owns canonical composition, accepted scoped-provider bindings and fail-closed dispatch/version/schema validation.
- Base H0 exposes only `system.describe` in HK01; later WPs add real world/authoring capabilities through the same boundary.
- Synthetic scoped providers exercise future engine-scoped contribution without importing an engine type system.
- Projection/conformance code consumes only the composed canonical inventory; no adapter-owned registry is introduced.
- Semantic equality is structural. Serialized fingerprints are diagnostic only and use unambiguous framing.
- Conformance dispatches canonical `system.describe` and checks its emitted portable artifact with an independent expected-data oracle that does not call the production projectors.

## Proof strategy

1. Positive contract/composition/discovery/runtime tests.
2. Independent/effective route enumeration versus canonical inventory.
3. Structural projection semantic-equivalence checks over the artifact actually emitted by `system.describe`, including an independent oracle for projector omissions/remapping.
4. Required RED→GREEN causal self-attacks for omission, schema, version, unknown route, implementation-detail leakage, transport leakage, projection omission, self-shrinking discovery and scoped-provider rejection classes.
5. Exact-SHA observation/verification through HK01-specific canonical scripts routed by `scripts/arkus-*.sh`.
6. Evidence matrix, self-attack record, residual-risk audit and mandatory Worker pre-review before freeze.

`PROOF_BUDGET_VERDICT` is initially expected to remain `WITHIN_BUDGET`: proof code will target only explicit HK01 acceptance and required causal attacks.
