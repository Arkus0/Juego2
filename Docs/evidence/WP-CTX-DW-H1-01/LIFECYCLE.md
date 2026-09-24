# WP-CTX-DW-H1-01 — H1 projection lifecycle

LIFECYCLE_VERDICT: READY

## Current accepted authority

- H1-04 accepted candidate: `8c6ffd61d17e832ed5b9f900e8c0f7d4e85bf5f5`
- Catalogue authority: `Unity/ArkusUnity/Assets/Arkus/H1/CatalogueMapping.json`
  - schema: `arkus.h1-catalogue-mapping@1`
  - Git blob: `922dbdffbe2f0a622acc2e153ca8b181427f6ce6`
- Source-adoption authority: `Docs/evidence/WP-H1-04/SOURCE_ADOPTION.json`
  - schema: `arkus.h1-04-source-adoption@1`
  - Git blob: `664e83e25269f345a248ce43410a28ed0a670750`

## Versioned adapter and projection identity

- Adapter: `ctx-dw-h1-01-adapter-v1`
- Projection schema: `ctx-dw-h1-01-v1`
- Lifecycle contract: `ctx-dw-h1-01-lifecycle-v1`
- Projection.Digest: `516f51930124e0e77e6f767af0c8074c8384723c809c0bd50571ff44d80b1858`
- ProjectionIdentity: `ctx-dw-h1-01-adapter-v1:516f51930124e0e77e6f767af0c8074c8384723c809c0bd50571ff44d80b1858`
- Current lifecycle identity:
  `ctx-dw-h1-01-lifecycle-v1|adapter=ctx-dw-h1-01-adapter-v1|projection=ctx-dw-h1-01-v1|h104=8c6ffd61d17e832ed5b9f900e8c0f7d4e85bf5f5|catalogue=922dbdffbe2f0a622acc2e153ca8b181427f6ce6|adoption=664e83e25269f345a248ce43410a28ed0a670750`

The concrete content identity above is the value produced by a deterministic rebuild from the two frozen accepted H1-04 authority blobs. `Projection.Digest` is already the SHA-256 of `DesignWorldProjection.NormalizedRepresentation`, so a second normalized-representation hash would duplicate the same binding rather than strengthen it. The exact-SHA verifier reconstructs the projection and requires both the computed digest and adapter-qualified identity to equal these published values.

## BUILD

`H1DesignWorldProvider` first rejects any authority bytes whose Git blob differs from the two accepted H1-04 blobs. The H1 adapter then parses only the versioned minimal H1 fields and relations, builds a generic `DesignWorldProjection`, and source-opens every fact to exactly one anchor inside one of those accepted authority documents.

The production H1 adapter is intentionally separate from the independent source-side oracle. The adapter may determine how H1 meaning is represented; it may not determine whether the authority universe is complete.

## VALIDATE

A projection is current only when both layers are GREEN:

1. generic DW validation: independent universe coverage, deterministic normalized projection, provenance resolution, projection version and generic relation/fact invariants;
2. `H1SourceAuthorityOracle`: independently reconstructs the H1-04 catalogue/source universe directly from the two frozen documents and compares exact identity, type, every declared field, relation multiset/cardinality/target and complete provenance.

The independent oracle does not consume production parser records, projection-derived counts or production projection identities as its expected universe.

## USE

CTX may admit this H1 projection as derived navigation only after a fresh BUILD + VALIDATE under the current lifecycle identity and only when the rebuilt projection identity is exactly `ctx-dw-h1-01-adapter-v1:516f51930124e0e77e6f767af0c8074c8384723c809c0bd50571ff44d80b1858`. Source-open authority remains H1-04; a returned DW fact is navigation context, not a product-authoritative replacement for the catalogue/source documents.

## STALE / CORRUPT

The projection is RED and must be rebuilt or bypassed when any of the following occurs:

- either accepted authority blob differs;
- H1 schema/project/distribution identity differs;
- projection schema/version differs;
- the rebuilt projection digest or `ProjectionIdentity` differs from the concrete values published above;
- an independently expected fact is absent or unexpected;
- any material projected field differs or is missing/extra;
- a relation is missing, extra or points at another target;
- provenance cannot source-open to the independently reconstructed authority anchor;
- generic DW validation reports stale/missing/ambiguous provenance or non-deterministic normalized state.

A source change is not silently accepted by rebuilding against new bytes: it first requires explicit H1 authority adoption and a new lifecycle identity/version as appropriate.

## Deterministic rebuild

The focused identity/lifecycle suite builds the same accepted authority twice with opposite adapter enumeration order and requires byte-identical normalized representation, projection digest and runtime projection identity. Both enumeration orders must produce the exact published identity `ctx-dw-h1-01-adapter-v1:516f51930124e0e77e6f767af0c8074c8384723c809c0bd50571ff44d80b1858`; the exact-SHA verifier also contains a causal tamper control proving that a unilateral different published expectation is RED.

## Optional-infrastructure boundary

`WP-H1-05` remains dependent on `WP-H1-04` and explicitly keeps canonical state authoritative. Missing/stale/corrupt H1 DW projection therefore disables the optimization/context path only; it cannot block or redefine H1 product work.
