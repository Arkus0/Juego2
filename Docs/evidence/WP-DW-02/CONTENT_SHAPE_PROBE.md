# WP-DW-02 — CITY production-query content-shape probe

## Accepted source and complete universe

DW-02 adopts exactly one source family for its frozen production-query suite: `Docs/production/CITY_LOCATION_PROGRAMME.md` §4 `District × location programme` from accepted CITY-02. The current accepted table contains **37** rows: **25** `loc.*` declared place/POI subjects and **12** `fam.*` ordinary/scenic programme families. These are observations of current accepted bytes, not constants used by production completeness oracles.

The production provider enumerates every §4 data row before generic projection. The repository proof separately parses the same accepted source table through an independent test-side oracle that shares neither the production parser nor its projected registry. Exact subject identities/tuples are compared, so a production parser/query filter cannot silently shrink both the denominator and the proof obligation.

## Reviewed projection manifest

`CityProductionProjectionManifest` adopts only the fields required by frozen query suite v1:

- `Programme ID` — stable identity and explicit `loc.*` / `fam.*` family;
- `District/family` — grouping/distribution;
- `Place / family` — readable result label;
- `A–D` — systemic-importance distribution;
- `S` — spatial-production-depth distribution;
- `Interior` — I0..I3 distribution and I2+ query;
- `Default access posture` — narrow four-role access-demand query.

Every projected result carries the exact accepted CITY-02 row anchor, source path, source digest and anchor digest through the inherited DW-00 provenance model.

## Current source-shape observations

Independent source-side derivation over current accepted bytes observes:

- systemic importance: A=8, B=15, C=11, D=3;
- spatial-production depth: S0=2, S1=17, S2=10, S3=7, S4=1;
- interior priority: I0=23, I1=7, I2=6, I3=1;
- I2+ higher-depth interior query: 7 subjects;
- complete programme universe: 37 subjects, not merely the 23 A/B subjects used by DW-01.

The code/tests do not use those totals as authority; they derive identities and buckets from current accepted source bytes on every run.

## Frozen real production queries

The suite frozen before implementation verdict (`QUERY_SUITE_V1.md`) proves:

1. every `loc.*` subject grouped deterministically by source-owned district;
2. exact identity sets for each selected source-owned access role;
3. every I2/I3 subject, ordered I3 then I2 and stable identity;
4. complete content-shape distributions over all 37 current rows, including A–D × S0–S4 RD-5 cross-tab cells;
5. stable normalized machine-readable output with source provenance tokens;
6. explicit `cost-model=UNMODELED` with no invented numeric cost/effort default.

## Causal controls

- deleting one projected `loc.*` fact while retaining the complete independent source universe produces `city.query_subject_missing`;
- deleting the entire projected `fam.*` class likewise produces `city.query_subject_missing`, proving the content-shape denominator cannot silently collapse to only places;
- changing accepted `loc.casco.bar` from S4 to S3 in a source copy rebuilds the projection and moves exactly one A/S bucket contribution rather than leaving stale cached classification;
- values preserved with a wrong provenance source path produce `city.query_provenance_invalid`;
- changed authority bytes with equal table semantics produce inherited `dw.provenance_stale`;
- reversing required-id/definition enumeration preserves generic projection digest, normalized representation and normalized query snapshot;
- absent cost inputs remain the non-numeric `UNMODELED` state rather than zero.

## Boundary findings

| Finding | Classification | Consequence |
|---|---|---|
| Complete CITY-02 programme content fits generic DW facts plus typed fields and exact-row provenance. | in-scope evidence | No DW-00/H0 semantic change required. |
| Full programme query denominator includes C/D and ordinary/scenic families, not only DW-01 A/B/interior subsets. | in-scope evidence | DW-02 is materially broader than the invariant slice without pretending to be a parcel/Unity census. |
| CITY-02 RD-5 can be computed directly from accepted A–D/S/I attributes. | in-scope evidence | Useful content-shape planning is available before realized geometry. |
| RD-1..RD-4 depend on traversal/runtime/realized geometry. | named residual/out of boundary | Not fabricated by DW-02; remain with CITY-04/later owners. |
| Production cost/asset coverage is absent from accepted source inputs. | explicit non-invention | Report exposes `UNMODELED`, not a numeric proxy. |
| CITY-05/06 invariants remain separately accepted under DW-01. | predecessor boundary | DW-02 does not duplicate or weaken them. |

Current-WP blocker findings: **none**.  
Concrete predecessor reopen conditions triggered: **none**.
