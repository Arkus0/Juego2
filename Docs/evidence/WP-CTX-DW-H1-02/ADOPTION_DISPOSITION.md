# WP-CTX-DW-H1-02 — selective-adoption disposition

ADOPTION_DISPOSITION: READY_FOR_EXACT_SHA_VALIDATION

Projection under evaluation: `ctx-dw-h1-01-adapter-v1:516f51930124e0e77e6f767af0c8074c8384723c809c0bd50571ff44d80b1858`.

## Result

The accepted H1 projection is useful, but only selectively. Its real represented surface is H1-04 catalogue/source identity, fields, `declared-in` / `adopted-from-source` relationships, source slices and the H1-04-declared component-schema catalogue entries. It does **not** represent effective scene membership/publication, H1-06's effective prefab relationship multiset, H1-07 effective adapters, or component field round-trip behavior.

Therefore H1-07+ must route claim-by-claim rather than assigning one broad H1 default.

| Claim shape | Disposition | Why | Product/source authority preserved |
|---|---|---|---|
| H1-05 catalogue/source navigation | `USE` | accepted projection directly represents catalogue entry + source adoption with source-open provenance | H1-05 proof matrix and H1-04 catalogue remain mandatory |
| H1-05 effective scene/publication | `NOT_MATERIAL` | effective scene membership and active-generation publication are not projected capabilities | H1-05 effective observation/manifest oracle remains authoritative |
| H1-06 source/prefab identity navigation | `USE` | catalogue prefab identity and adopted source relation are represented | H1-06 proof matrix + H1-04 catalogue remain mandatory |
| H1-06 effective source-derived relationship equality | `OPTIONAL` | catalogue identity can shorten navigation, but the effective relationship multiset is not in DW | exact H1-06 source-vs-realized multiset oracle remains authoritative |
| H1-07 component-schema inventory navigation | `USE` | H1-04 already declares the Animator, MeshRenderer and Transform `component-schema` entries and H1-01 projects them source-open | H1-07 must still independently prove schema↔effective-adapter completeness |
| H1-07 supported field/reference round-trip | `OPTIONAL` | schema identity is projected; effective adapter + materialize/save/reload/inspect semantics are not | H1-07 product evidence remains mandatory |
| H1-07 unsupported/effective behavior | `NOT_MATERIAL` | the projection contains no effective-adapter or field-runtime truth | H1-07 fail-closed adapter/field oracle owns the claim |
| H1-GATE mandatory fresh public-client trial | `NOT_MATERIAL` | Juego2-private CTX/DW pre-seeding would contaminate the public discovery trial | public launch profile + discovery/schemas only |

## Navigation effect

The exact-SHA verifier rebuilds the accepted H1-01 projection and serializes one representative Quaternius prefab query plus one representative component-schema query. For every admitted `USE` route, each selective source-open payload must be smaller than the full accepted H1-04 catalogue authority document. The verifier publishes the exact byte counts in its proof summary/receipt; the comparison is a bounded navigation observation only and is **not** evidence of product correctness or model quality.

No claim is made that the full normalized projection is smaller than the source corpus, or that every agent/task benefits. The useful operation here is selective retrieval of a source-open fact from a current projection.

## Fail-closed routing

`USE` is legal only while the H1-01 lifecycle is current for the exact accepted projection identity. A projection-schema staleness probe must turn a baseline `USE` route into `OPTIONAL`/source-first rather than silently continuing to admit DW.

The proof also requires:

- undeclared product capabilities cannot be injected into the projection observation to manufacture adoption;
- adding `H1` labels or suggestive free text cannot change classification;
- a broad `WP-H1-* => USE` classifier is observably wrong on product-only claims;
- a route that drops one mandatory source/evidence read is detected;
- partial coverage cannot close H1-06 relationship equality or H1-07 component behavior;
- H1-GATE public-client isolation forces private CTX/DW abstention.

## H1-07+ default guidance

Default to `USE` only for exact current catalogue/source/schema-inventory navigation that the accepted projection actually represents. Default to `OPTIONAL` for mixed claims where the projection can locate an identity but the deciding truth lives in effective Unity/product evidence. Default to `NOT_MATERIAL` where no represented capability contributes to the decision, and always for the mandatory fresh H1-GATE public-client trial.

A missing, stale or weak projection never blocks H1. CTX/source authority remains the fallback, and any Worker/Reviewer may source-open more deeply whenever needed.

## Residuals

This disposition does not prove future H1-08+ projection usefulness before those claims exist, does not authorize projection-schema expansion, and does not pre-author a future H2 portability boundary. Future claims should inherit the classification rule, not the current case labels.
