# WP-HK-02 residual risk

Implementation observation SHA: `58ca3eab4090c211126425be1e3950f503f48000`

## Known bounded residuals

1. **World schema v1 only.** HK02 defines the first persisted canonical form and intentionally does not implement arbitrary outer-schema migration. Unsupported outer versions fail closed. Later migration work must preserve the v1 contract or introduce an explicitly reviewed version bridge.
2. **Forward compatibility is extension-envelope compatibility, not unknown-structure guessing.** Unknown owner/version extension payloads are opaque bytes and survive exactly; unknown top-level record kinds do not. This avoids silently inventing semantics for data HK02 cannot understand.
3. **Hash security relies on SHA-256/BCL correctness.** Cryptographic implementation and collision resistance are part of the trusted base, not recursively proven here.
4. **Identity syntax is intentionally narrow.** Stable identifiers accept lowercase ASCII letters, digits, `.`, `_`, `-`. Richer display names/localization are not identity and can be layered later without changing these stable keys.
5. **References are state-local in HK02.** Cross-world/external asset references are not part of this WP. Introducing them requires a reviewed identity/integrity policy rather than weakening current same-state resolution.
6. **Transactions, concurrency, validation policy, provenance/replay and engine realization are downstream.** HK02 only establishes the deterministic state substrate they consume; it does not pre-implement HK03+ semantics.

## Dependency / portability result

No external package, Unity/DFU type, transport host, gameplay system or asset is introduced. The product implementation remains in `Arkus.Game.World` targeting the existing `netstandard2.1` boundary.

## Proof-budget conclusion

The proof is bounded to the finite current state model and the six mandatory defect classes plus three direct integrity/format variants. It does not duplicate HK00/HK01 completeness, add generic serializer infrastructure, or build speculative gameplay/migration machinery.

KNOWN_UNDETECTED_DEFECT_CLASSES: 0
UNRESOLVED_PROOF_OBLIGATIONS: 0
PROOF_BUDGET_VERDICT: WITHIN_BUDGET
