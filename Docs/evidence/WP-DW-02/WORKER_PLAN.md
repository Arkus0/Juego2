# WP-DW-02 — Worker plan

Baseline: `main@c6a70f016e95f1f9605a505ffede10c7a5d078a5`
Workpack: `WP-DW-02 — CITY production queries and content-shape projection`
Execution mode: REMOTE_OK

## Predecessor gate

`WP-DW-01` is accepted, merged and DocSynced on the baseline. DW-02 consumes the accepted generic DW-00 projection/provenance/rebuild boundary plus DW-01 CITY domain isolation and invariant slice. No current evidence requires reopening DW-00, DW-01 or H0.

## Declared production-query boundary

DW-02 projects the complete accepted CITY-02 `District × location programme` ledger as its production-query universe. The source table, not the projection/query implementation, independently defines the subject universe. Every row is projected, including `loc.*` place subjects and `fam.*` ordinary/scenic families.

The exact adopted fields are: programme identity, district/family, place/family label, A–D importance, S0–S4 spatial-production depth, I0–I3 enclosed-interior priority and normalized source-owned access roles from default access posture. These fields are sufficient for the frozen query suite and RD-5 content-shape measurements without inventing geometry, effort or cost.

CITY-05/CITY-06 remain consumed by DW-01 for their accepted invariant guarantees; DW-02 does not duplicate those authorities merely to make the query surface look broader.

## Independent completeness strategy

Production code derives its required fact universe from every row of the accepted CITY-02 table before building the generic DW projection. Repository proof additionally uses a separately implemented source-side test oracle that parses the frozen table independently of the production parser/query service, so a production-parser omission cannot redefine the expected set.

The proof compares stable identities and buckets, not only totals. A projected omission, classification drift, provenance loss or source-enumeration ordering difference must therefore remain observable.

## Frozen query suite

The representative query suite is frozen separately in `Docs/evidence/WP-DW-02/QUERY_SUITE_V1.md` before implementation verdict/evidence. Implementation may add internal helpers but may not narrow that suite or its source-owned universe after seeing results.

## Content-shape report v1

The report will expose explicit source-owned counts/buckets only:

- programme subjects and declared `loc.*` POIs by district;
- A–D distribution;
- S0–S4 distribution;
- I0–I3 distribution;
- A–D × S0–S4 cross-tab required by CITY-02 RD-5;
- normalized access-role demand counts over the complete programme universe.

No hours, euros, asset counts, staffing, production-effort multiplier or fabricated default cost is emitted. Cost state is an explicit non-numeric `UNMODELED` marker.

## RED-first proof plan

1. Remove one projected source record while keeping the independently source-derived expected set complete; completeness oracle must turn RED for that exact subject.
2. Change one accepted S-depth classification in source, rebuild, and require the subject and report count to move between the exact old/new buckets.
3. Preserve values but break/replace result provenance; query validation must turn RED.
4. Reverse source/definition enumeration and rebuild; normalized queries/report must remain byte-equal.
5. Present a query/report projection missing one complete source family/bucket contributor; independent identity/bucket comparison must turn RED rather than accepting a smaller denominator.
6. Verify absent production-cost inputs remain `UNMODELED` and no numeric zero/default estimate appears.

Restore accepted bytes and require the full reference suite GREEN.

## H0 / authority boundary

All CITY-specific parser/query/report code remains in `Arkus.DesignWorld`, downstream of `Arkus.Game.World`. H0 receives no CITY query type, field registry, manifest or semantic rule. CITY source truth remains Markdown authority with exact-row provenance; the DW state is rebuildable derived state only.

## Known unknowns / explicit non-inventions

- `loc.*` is adopted only as the source-visible declared-place identity family; `fam.*` remains separately visible in the complete report universe.
- District strings are preserved exactly after Markdown emphasis/backtick cleanup; no new district ontology or aliasing is invented.
- Access-role normalization remains the accepted four-role vocabulary `{public, service, private, semi-private}` and does not promote `conditional public`.
- RD-1..RD-4 require realized traversal/runtime observations and therefore remain outside this report; DW-02 adopts only source-computable RD-5 shape plus explicit planning distributions.
- Actual art coverage, prefab availability, geometry, production cost and Unity realization remain downstream owners.