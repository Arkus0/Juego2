# WP-DW-02 — Frozen representative production-query suite v1

FROZEN_QUERY_SUITE: `dw02-city-query-suite-v1`
Authority universe: every data row in `Docs/production/CITY_LOCATION_PROGRAMME.md` §4 `District × location programme`.
Freeze baseline: `main@c6a70f016e95f1f9605a505ffede10c7a5d078a5`.

This suite is frozen before implementation verdict. Expected identities/counts are derived from accepted source bytes by an independent source-side oracle; current observations are evidence, not hard-coded authority.

## Q1 — declared POIs by district

Return every projected CITY-02 programme subject whose stable identity starts with `loc.`, grouped/ordered by the source-owned `District/family` value and then stable identity. Each result must expose fact identity plus exact accepted-source provenance sufficient to open the source row.

Completeness oracle: independently parse every §4 row, select `loc.*`, compare the complete `(district, id)` set to query results.

## Q2 — subjects requiring a selected access role

For each supported role in `{public, service, private, semi-private}`, return every complete-universe programme subject whose accepted `Default access posture` requires that role under the accepted narrow normalization. Order by stable identity and retain provenance.

`conditional public` is removed before role extraction and therefore never manufactures a required `public` role by itself.

Completeness oracle: independent source parser applies the same source-owned token rule directly to §4 bytes and compares exact subject identities per role.

## Q3 — higher-depth interiors

Return every complete-universe programme subject with accepted enclosed-interior priority `I2` or `I3`, ordered first by depth descending (`I3`, then `I2`) and then stable identity, with provenance.

Completeness oracle: independently parse §4 `Interior`, normalize only the literal I0..I3 prefix, and compare exact `(depth, id)` membership.

## Q4 — complete content-shape report v1

Over every §4 row, report deterministic distributions for:

- district/family;
- A–D systemic importance;
- S0–S4 spatial-production depth;
- I0–I3 interior priority;
- A–D × S0–S4 cross-tab (CITY-02 RD-5 planning shape);
- the four normalized required access-role counts;
- total programme subjects;
- total `loc.*` declared POIs;
- total `fam.*` programme families.

All bucket keys with source-defined vocabulary are emitted explicitly, including zero-valued A–D/S/I cross-tab cells, so ordering or absent cells cannot make normalized output ambiguous.

Completeness oracle: independently parse all §4 rows and compare complete per-subject tuples plus aggregate buckets.

## Q5 — normalized machine-readable snapshot

Serialize Q1–Q4 to a stable normalized text representation with ordinal key/identity ordering and exact provenance tokens. A clean rebuild over identical accepted source bytes/schema version must produce byte-equal output even when definition enumeration is reversed.

## Cost boundary

The report exposes `cost-model=UNMODELED`. It contains no numeric hours/euros/assets/staffing/effort estimate and no absent input is converted to zero. A future reviewed model must own any such conversion.
