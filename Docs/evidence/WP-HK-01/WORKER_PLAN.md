# WP-HK-01 Worker plan

Status: READY FOR EVIDENCE-BEARING VALIDATION — repair cycle 2 circuit-breaker re-audit

Baseline main: `9db7a1ffffa02c02af7889de29960de1f19d2755`  
Worker: ChatGPT / GPT-5.6 Sol (initial + repair cycles 1 and 2)

## Goal

Implement the canonical Arkus capability contract, base + scoped composition boundary, machine-readable discovery and canonical dispatch/schema validation required by `WP-HK-01`, without introducing world semantics, transport framing, Unity/engine APIs or gameplay commands.

Repair cycle 2 re-audits the complete portability and version-evolution/negotiation boundary after the foundational circuit breaker. It does not treat the Reviewer examples as isolated patches.

## Claim and trust boundary

HK01 claims completeness over the Arkus public capability surface admitted through the canonical runtime boundary. Git/checkout semantics, the pinned .NET toolchain/runtime, normal reflection semantics for candidate assemblies and the CI runner are trusted infrastructure under `FOUNDATIONAL_PROOF_STANDARD.md`.

The proof universe is not sourced only from discovery/registration metadata. Conformance independently enumerates concrete public capability route implementations/bindings in the tested assemblies/provider fixtures and compares that effective surface against the composed canonical inventory, discovery projection and schemas.

Reviewed provider contributions may define engine-scoped domain semantics, but public schemas remain portable. Logical references are opaque Arkus canonical identities in `ref.<provider-id>.<domain>` space; actual engine/runtime objects, assembly-qualified identities and foreign-provider reference namespaces are not accepted canonical data.

## Implementation shape

- `Arkus.Harness.Protocol` owns portable capability identity/version, canonical schema documents, structured errors, semantic metadata, structural equality, compatibility and discovery projection.
- `Arkus.Harness.Runtime` owns composition, accepted scoped-provider bindings, logical-reference ownership admission and fail-closed dispatch/version/schema validation.
- Same-major request evolution is modeled as a conservative recursive acceptance-preservation relation. A higher minor may be negotiable only when every request accepted before remains accepted after.
- `ContractComposer` enforces every adjacent same-major compatibility edge before constructing `ComposedContract`; runtime highest-minor selection is therefore downstream of compatibility admission.
- Base H0 exposes only `system.describe`; synthetic scoped providers exercise the future engine/provider extension boundary without importing an engine type system.
- Projection/conformance consumes the composed canonical inventory; no adapter-owned registry is introduced.
- Semantic equality is structural. Serialized fingerprints are diagnostic only.
- Conformance dispatches `system.describe` and checks its emitted portable artifact with an independent expected-data oracle.

## Proof strategy

1. Positive contract/composition/discovery/runtime tests.
2. Independent/effective route enumeration versus canonical inventory.
3. Structural projection equivalence over the emitted `system.describe` artifact.
4. Causal self-attacks for omission, portability, provider/reference ownership, schema narrowing, version-chain admission, binding, transport leakage and proof-universe shrinkage.
5. Exact-SHA observation/verification through HK01 canonical scripts.
6. Evidence matrix, residual-risk audit and mandatory Worker pre-review before freeze.

Repair implementation observation `cfcb9ab25977a1f58cc85554c5150d98574e01e3` is GREEN in Actions run `35435242336`: 9/9 canonical positives, 29/29 causal controls, 39/39 regression, 0 build warnings/errors.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET
