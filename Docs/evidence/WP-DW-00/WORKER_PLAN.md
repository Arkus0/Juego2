# WP-DW-00 — Worker plan

Baseline SHA: `504e6b272e7bfed08b5a6466d94416718aadd62b`
Canonical branch: `wp-dw-00-authority-projection-v2`
Execution requirement: `REMOTE_OK`
Class: `FOUNDATIONAL`

## PREDECESSOR_CONTRACT_CHECK

Accepted direct dependency: `WP-HK-GATE`.

Accepted identity independently confirmed from `Docs/workpacks/HK/WP-HK-GATE.md`:

- reviewed candidate: `0fa3d4fb039a3d0049cea0f3eed1c83128ec7dcd`;
- independent review: `#5261636151` PASS;
- merge SHA: `048d2e449d5ff81e8bcc35bc15664ea4ceec18ca`.

Accepted-contract capsule used for initial reconstruction: `Docs/engineering/context-capsules/WP-HK-GATE.json` (`boundary_summary`). Authoritative escalations performed because DW-00 directly consumes the public H0 seam: exact `WP-HK-GATE`, current `WorldState`/`WorldObject`/`WorldReference`/`WorldExtensionData`, canonical world-state codec, and public inspection abstractions/service were read directly. Binding DW architecture and `FOUNDATIONAL_PROOF_STANDARD.md` v1.5 were also read directly.

Inherited guarantees relevant to DW-00:

- H0 is accepted as the engine-neutral authoring foundation;
- canonical H0 identity/reference/world-state/inspection/validation semantics are already accepted and are consumed as-is;
- the kernel remains engine/domain neutral and public transports/adapters do not own semantic truth;
- accepted H0 provenance/replay/diff/query foundations are not reopened merely because a second consumer is added.

Guarantees newly owned by DW-00:

- a generic versioned Design World fact/relation projection envelope outside H0;
- source-authority provenance sufficient to fail closed on missing, ambiguous or stale anchors;
- deterministic normalized projection and equal rebuild from identical authority inputs + projection version;
- independent expected-universe vs authority-reader separation for completeness-sensitive proof;
- generic mapping onto accepted H0 `WorldState` objects/references/extensions without reverse H0 dependency or CITY/PA semantics in H0;
- one neutral independently enumerable fixture plus one bounded approved Juego2 content-shape probe.

Inherited guarantees intentionally consumed rather than re-proved: H0 world-state validity, canonical serialization determinism, public inspection semantics and accepted engine-neutral kernel boundaries. DW-00 tests only the seam it uses plus the absence of a new reverse dependency.

Concrete predecessor reopen condition: only executable evidence that an essential generic typed/provenance/rebuild requirement cannot be represented through the accepted H0 public world-state/inspection surface without changing an accepted H0 semantic guarantee. Convenience, naming preference or CITY-specific semantics are insufficient.

## Claim and trust boundary

Claim: selected accepted design facts can be represented as typed, provenance-bearing, rebuildable derived state through public H0 world-state surfaces while source documents remain authority and H0 stays domain neutral.

Trusted base follows `FOUNDATIONAL_PROOF_STANDARD.md`: exact Git checkout, pinned .NET/MSBuild/NuGet toolchain, normal filesystem/runner behavior and SHA-256 implementation. Source-document semantics themselves are accepted upstream; DW-00 proves projection mechanics/authority preservation, not CITY semantic truth.

## Implementation plan

1. Add `Arkus.DesignWorld` as a downstream consumer assembly depending on `Arkus.Game.World`, never the reverse.
2. Define typed primitive fields, typed relations, stable provenance anchors, projection version, independent authority universe and authority reader contracts.
3. Implement an anchored-text reference reader that resolves an exact source anchor uniquely and binds both source-byte and anchor digests.
4. Build derived `WorldState` using generic `dw.fact` objects plus Design World extensions; relationship types remain opaque data to H0.
5. Normalize/sort projection state deterministically, hash it, support deterministic rebuild and bounded fact-level diff.
6. Validate exact universe coverage, expected projection version, provenance freshness/uniqueness, normalized digest and H0 surface parity.
7. Add neutral fixture tests and causal negatives for omission, stale provenance, ambiguous provenance, stale rule-version reuse, material source change and hidden-derived-state rebuild.
8. Add a bounded accepted `CITY_LOCATION_PROGRAMME.md` shape probe that checks only representability/identity/granularity/provenance and public inspect/query behavior; no CITY invariant truth is claimed.
9. Register a canonical exact-SHA observation entrypoint for `WP-DW-00` and run focused + full regression validation.
10. Persist proof matrix, content-shape findings and residual-risk audit before exact-HEAD strict pre-review.

## Scope guard

No H0 semantic/type changes, no CITY/PA-specific kernel/public generic contract, no CITY invariant ownership, no PA corpus work, no Unity work, no workflow/process modeling, no LLM extraction as authority, no gameplay/art/product packaging.

## Canonical-history note

An earlier Draft PR `#125` was intentionally superseded before any freeze/review because implementation bytes were written before this mandatory predecessor check was persisted. No acceptance evidence from that Draft is treated as canonical. This branch restarts from the same live baseline with the predecessor check as the first candidate commit.
