# WP-H1-06 — Post-acceptance DocSync

DOCSYNC_STATUS: **DOCSYNC_COMPLETE**  
MODE: **PROCESS_ONLY / DOCUMENTATION_ONLY**  
DATE: 2026-09-25

## Accepted result

- Accepted candidate SHA: `96de260021fb28ff2cc7da8d2ef488be419568b3`
- Canonical implementation PR: `#195`
- Final independent PASS review: `#5313437809`
- Autopilot review ID: `9f730f253ad94451bbff4f08180ad6fe`
- Implementation merge: `6d04581bc3916b376bedc0797258097cfb22c225`
- Physical-local Unity receipt: PR comment `#5826765811`, GREEN on the exact accepted SHA using Unity `6000.3.24f1 (4e7b9b5b6244)` and .NET `8.0.425`; candidate clean before/after.
- Arkus Main Safety: run `36094529078` GREEN on the exact accepted SHA.
- Arkus Candidate Validation: run `36094850815` GREEN on the exact accepted SHA.
- Worker preflight: GREEN with `402/402` solution tests.

## Accepted claim

H1-06 establishes truthful source-asset/prefab realization over the accepted H1-04 Quaternius Source and H1-05 managed-scene publication boundary without making generated Unity state canonical authority.

The accepted implementation preserves managed prefab variant/source identity and exact lineage, keeps bridge derivatives confined to deterministic managed generation roots, and observes source-derived prefab relationships with one normalized identity rule. The relationship oracle is exact multiset equality over nested-prefab, mesh, material and animation-reference rows, preserving multiplicity and rejecting both missing/under-count and extra/over-count states with the same mechanism. Validation and relationship digest use the same `RelationshipKey` semantics.

The repair cycle also proved that descendant H1-managed canonical scene nodes are evaluated under their own prefab realizations rather than incorrectly counted as parent-source relationships, while unmarked prefab internals remain inspectable. A stale existing derivative with an extra material relationship is rejected for reuse, rebuilt from the unchanged source, staged and published as a fresh valid generation. Non-null Unity references without stable asset identity/path fail closed as `projection.prefab-reference-unresolved` rather than disappearing from observation.

Source bytes remain read-only, missing/wrong-type/rebound logical asset mappings fail closed with stable diagnostics, same-input materialization is semantically idempotent, deletion/rebuild converges to the same normalized relationship profile, and prefab realization does not mutate canonical world state or journal.

## Foundational review history

Two earlier independent FAILs remain useful causal history rather than accepted state:

1. candidate `53801ae7f9990502dd5881c73717613a0fc7abe2` lost multiplicity for duplicate equivalent nested-prefab instances (`source=2`, realized=1);
2. candidate `5a771e8f28bf67e3fa235a88fd2ffd2d4520f5d4` rejected under-count but still accepted leftover extra relationships (`source multiset ⊂ realized multiset`).

The accepted candidate closes both as the two inequality directions of one exact multiset-equality claim. The circuit breaker therefore resulted in a simpler oracle, not relation-kind-specific guards.

## DocSync actions

1. Mark `WP-H1-06` COMPLETE / ACCEPTED on the exact candidate, review and merge identities above.
2. Persist `DOCSYNC_COMPLETE` and the exact physical-local/hosted validation identities without rewriting historical failed observations as GREEN.
3. Advance the H1 execution spine to `WP-H1-07 — Allowlisted component projection` as the next dependency-valid workpack.
4. Preserve H1-04 catalogue/source authority, H1-05 publication authority, H0 canonical authority and the separation between prefab relationship fidelity and H1-07 component-property semantics.
5. Preserve art production, Cantabrian adaptation, broader source adoption, CITY/H2/gameplay and arbitrary component reflection outside H1-06.

## Boundary

This DocSync is documentation/current-state reconciliation only. It does not modify runtime/editor implementation, rerun Unity, adopt new source assets, reopen H1-04/H1-05, or pre-authorize H1-07 implementation work.

## Next action

Next default H1 workpack: `WP-H1-07 — Allowlisted component projection`.

It is dependency-valid from accepted H1-06 but remains `NOT_STARTED` until explicitly started by the human/automation controller.

`DOCSYNC_COMPLETE`
