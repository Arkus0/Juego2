# WP-CTX-DW-H1-01 — Proof matrix

PROOF_MATRIX_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0

## Scope boundary

This proof matrix covers only the new H1-04 → DW derived projection/lifecycle. H1-04 Unity catalogue truth, accepted Quaternius source/licensing and native locator correctness are inherited from accepted PR #185. Generic DW/H0 contracts are inherited from the accepted DesignWorld kernel and are exercised here only where the H1 adapter consumes them.

## Requirements → evidence

| Obligation | Production evidence | Causal / falsification evidence | Verdict |
| --- | --- | --- | --- |
| Use real accepted H1-04 authority, not a fixture-owned universe | `H1ProjectionManifest` pins H1-04 candidate plus exact catalogue/source-adoption paths, schemas and Git blobs; `H1DesignWorldProvider` verifies blobs before parsing | `MaterialAuthorityDriftIsStaleBeforeProjectionCanSelfConfirm` mutates accepted authority and requires `h1.accepted_source_blob_mismatch` before a new projection can self-confirm | GREEN |
| Independent H1 source universe | `H1SourceAuthorityOracle.OracleBuilder` independently scans `entries`, `sources` and `slices` from frozen authority; it does not call the production H1 parser | `OmittedCatalogueFactIsRedEvenWhenGenericProjectionSelfConfirms` removes a real catalogue fact, deliberately gives generic DW a self-confirming universe/reader, proves generic GREEN and independent H1 oracle RED | GREEN |
| Minimal, versioned H1→DW adapter | `ctx-dw-h1-01-adapter-v1`, projection `ctx-dw-h1-01-v1`, adopted H1 entity/relation types in `H1ProjectionManifest` | `ProjectionSchemaChangeMarksCachedProjectionStale` validates cached v1 projection against v2 and requires `dw.projection_version_stale` | GREEN |
| H1 vocabulary does not pollute generic H0/DW kernel | all H1 parsing/oracle semantics live in `H1CatalogueProjection.cs` and `H1SourceAuthorityOracle.cs`; generic kernel is unchanged | `H1VocabularyDoesNotLeakIntoGenericDesignWorldKernel` rejects H1/Quaternius vocabulary in `DesignWorldContracts.cs` / `DesignWorldProjection.cs`; exact-SHA verifier repeats the static gate | GREEN |
| Complete accepted catalogue/source coverage | production adapter projects catalogue root, every accepted catalogue entry (including the three `component-schema` records with intentionally empty builtin GUID/hash), source-adoption root, sources and source slices | first Main Safety run #532 went RED because production silently omitted the three empty-field `component-schema` records while the independent oracle enumerated them; repair made production cover them rather than weakening the oracle | GREEN |
| Exact material field preservation | independent oracle compares every expected field and rejects missing, extra or changed fields | `MaterialFieldWeakeningIsRedWithIdentityPreserved` preserves identity/provenance but weakens `adoption-status`; generic DW is GREEN and H1 oracle requires `h1.semantic_content_mismatch` | GREEN |
| Exact relation names/cardinality/targets | H1 oracle compares sorted relation multisets, preserving duplicate/cardinality semantics and exact targets | `MissingSourceRelationIsRedWithBothEndpointsPresent` removes only `adopted-from-source` while both endpoints remain; generic DW is GREEN and oracle requires `h1.semantic_relation_missing` | GREEN |
| Source-open provenance is material | `H1MultiSourceAuthorityReader` requires one unique accepted-source anchor and stores authority ID/path/source digest/anchor digest; oracle independently reconstructs and compares the same provenance tuple | `EqualValuesWithForgedProvenanceAreRed` keeps identical values and anchor text but forges authority identity; generic self-confirmation remains GREEN while H1 oracle requires `h1.semantic_provenance_mismatch` | GREEN |
| Deterministic rebuild | generic projector canonicalizes facts/relations; adapter version/source blobs are frozen | `ReversingAdapterEnumerationRebuildsByteIdenticalProjection` plus `ExactAcceptedAuthorityRebuildMatchesPublishedIdentity` require identical normalized representation/digest/identity under opposite enumeration order | GREEN |
| Stale source/schema/version is fail-closed | exact Git blob freeze, accepted schema/project/distribution checks, generic projection version validation | source-byte mutation and projection-version mutation both go RED; exact-SHA verifier rechecks committed source blobs independently with `git hash-object` | GREEN |
| Concrete current projection identity is published for CTX USE | `LIFECYCLE.md` durably publishes `Projection.Digest=516f51930124e0e77e6f767af0c8074c8384723c809c0bd50571ff44d80b1858` and `ProjectionIdentity=ctx-dw-h1-01-adapter-v1:516f51930124e0e77e6f767af0c8074c8384723c809c0bd50571ff44d80b1858`, alongside adapter/schema/lifecycle and accepted authority bindings | identity test rebuilds real authority forward/reverse and compares against durable evidence; exact-SHA verifier independently extracts durable values, recomputes actual values, rejects mismatch, and proves a unilateral tampered expected identity goes RED | GREEN |
| DW remains optional, never H1 authority | no H1 workpack dependency is changed; H1-05 still depends on H1-04 and canonical source | `H105RemainsDependentOnAuthoritativeH104NotThisDerivedProjection`; exact-SHA verifier independently greps the same boundary | GREEN |
| Canonical freeze-time verification exists | `scripts/ctx-dw-h1-01-verify-exact-sha.sh` validates checkout identity/cleanliness, exact H1-04 blobs, version constants, locked restore, Release build, focused lifecycle/identity tests, concrete published identity binding, generic-kernel boundary, H1-05 fallback and durable proof markers | `scripts/arkus-verify-exact-sha.sh` routes `WP-CTX-DW-H1-01` to that verifier for Candidate Validation | GREEN |

## Independent-oracle incident evidence

The initial implementation was not accepted merely because its own tests compiled. Main Safety #532 compiled successfully but five focused tests failed. The independent oracle had enumerated three real `component-schema` entries that the production regex silently skipped because their accepted `nativeGuid` and `contentSha256` values are intentionally empty for `unity-builtin`.

The repair changed the production parser to include those valid empty values and changed the oracle only to distinguish a missing key from a present-empty accepted key. The oracle's independent expected universe and exact-comparison rules were not weakened. Main Safety #534 then passed on repaired SHA `a97758eccf8abeddc501a7705208a5447a5a316b`.

## Concrete-identity incident evidence

Reviewer #5307063383 found that the accepted lifecycle inputs were frozen but the actual content identity was still represented symbolically. An intermediate exact-authority probe on SHA `99852f1428512d9e344fc8fbd7f2c83368cb85b3` rebuilt the real projection under Candidate Validation #2001 and produced digest `516f51930124e0e77e6f767af0c8074c8384723c809c0bd50571ff44d80b1858`; reverse enumeration produced the same adapter-qualified identity. This repair publishes that mechanically derived value and makes the canonical exact-SHA verifier compare every future rebuild against it rather than trusting the evidence text.

## Final freeze gate

The final candidate is not considered complete by this document alone. At freeze, `scripts/ctx-dw-h1-01-verify-exact-sha.sh <PRODUCT_SHA>` must execute through `scripts/arkus-verify-exact-sha.sh` on the exact clean checkout and Candidate Validation must be GREEN for that exact SHA.
