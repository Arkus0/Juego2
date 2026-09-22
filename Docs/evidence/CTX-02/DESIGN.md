# CTX-02 Design / Coverage Note

Baseline: `f7b4f1e8247dfc927203dca6754b71aaa53938f3`

## Chosen boundary

CTX-02 adds a non-authoritative accepted-contract capsule layer with contract-scoped freshness. It does not reuse CTX-01's global-main freshness because an unrelated main commit must not invalidate an immutable accepted predecessor.

The mechanical oracle is split:

- accepted identity comes from the independent `COMPLETE` workpack metadata and may additionally be checked against live accepted state;
- source integrity comes from recomputed Git blob SHA over repository bytes;
- PA disposition completeness comes from parsing the authoritative result table, not from the capsule/index's own inventory;
- accepted PA-chain coverage is discovered from canonical result files whose matching workpack is `COMPLETE`.

## Representative coverage

- H1: `WP-HK-GATE` → `WP-H1-02`, boundary capsule only; no bulk H0 migration.
- CITY: `WP-CITY-03` → `WP-CITY-04`, boundary capsule only. `CITY_PRODUCT_SEED.md` is mandatory/non-compressible.
- PA: accepted result capsules for `WP-PA-01`, `WP-PA-02`, `WP-PA-03`. Future accepted PA results are added during DocSync.

## PA compression rule

Disposition state is preserved exactly as source data, including compound/multi-state values (`ADOPT ... / LATER ...`) and exclusions (`REJECT`, `REJECT baseline`, `REJECT as authority`). The capsule is not a binary summary.

The PA-03 inherited exclusion `default global/N-hop social traversal to discover targets = REJECT` is deliberately protected because it carries PA-02's bounded-discovery guarantee.

## CITY non-compression rule

The capsule does not contain the playable boundary geometry, streets, parcels, scenarios, seams or measurement pack. Any CITY-04 construction question escalates to the exact `CITY_PRODUCT_SEED.md`.

## No authority promotion

A VALID result means only `VALID_NAVIGATION_ONLY`. It does not mean predecessor PASS is re-proved, source semantics are copied into the capsule, or Reviewer independent judgment is satisfied.
