# WP-DW-02 — CITY production queries and content-shape projection

Status: COMPLETE / ACCEPTED
Class: FOUNDATIONAL
EXECUTION_REQUIREMENT: REMOTE_OK
Depends on: `WP-DW-01` PASS + merge + DocSync
Blocks: `WP-DW-03`

Acceptance: frozen candidate `65cfc1fcc4cce4df368e966297db52b3b5d06093`; independent PASS review `#5283919380`; PR `#139`; merge `1fe0b178668a7698e22de57f49f8100eaae0455a`; final fully GREEN exact-SHA validation Actions `35785494959`; post-PASS DocSync `Docs/evidence/WP-DW-02/DOCSYNC.md`.

## Objective and central claim

Expand the proven CITY slice only far enough that the Design World becomes useful for real project work: complete deterministic queries over accepted CITY programme/design data and explicit content-shape measurements that can inform art/content planning without inventing cost estimates.

## WHY_THIS_BOUNDARY

A validator that catches one invariant proves technical viability but not project value. Production query completeness, provenance and derived measurements can be wrong while validation remains correct, so they deserve a separate review boundary before the more semantically dense PA corpus is attempted.

## Inherited guarantees

Consumes DW-00 generic projection/provenance/rebuild semantics, DW-01 CITY domain isolation and accepted CITY source authority.

## New guarantees owned

- a reviewed CITY projection manifest covering the exact source families needed for declared production queries;
- deterministic query results for POI/district/access/interior-or-spatial-depth/spatial-demand and other explicitly adopted families;
- independently checked query completeness against source-owned universes;
- content-shape report v1 using only explicit accepted attributes/proxies;
- machine-readable outputs suitable as downstream brief/planning inputs while retaining source provenance;
- no implicit conversion from content-shape count to hours/euros/asset effort unless a future reviewed model supplies those inputs.

## Explicitly not re-proved

CITY design quality, exact production cost, art asset availability, Unity realization, player experience, PA research or H0 correctness outside the consumed seam.

## Allowed scope

CITY projection expansion, deterministic query/report surfaces, source-manifest/completeness oracles, focused tests and evidence.

## Forbidden scope

Full text summarization as authority, invented production estimates, generated final art briefs as a product claim, Unity inspection, changing CITY source truth, new H0 semantics or subjective design scoring.

## Architecture / authority boundary

Queries operate over derived DW state, but every result must remain traceable to accepted source truth. Completeness is proven against independent/effective source manifests, not by counting whatever the projection currently contains.

## Acceptance criteria

- the reviewed projection manifest declares exact accepted source families/fields and why each is needed;
- at minimum the system can deterministically answer: all declared POIs by district; all subjects requiring a selected access role; all accepted I2+/equivalent higher-depth interiors; and spatial-demand/content-shape distributions for the accepted projected universe;
- query results include stable projected identities and provenance sufficient to open the accepted source fact;
- independently derived source-side expected sets/counts equal DW results for the reference query suite;
- content-shape report v1 exposes explicit counts/buckets/proxies and labels unknown/unmodeled cost rather than manufacturing estimates;
- identical source anchors/schema version produce equal normalized report/query output after clean rebuild;
- omitted projected source record cannot silently reduce query totals and stay GREEN;
- a changed accepted classification moves the subject between the appropriate query buckets on rebuild;
- no query/report requires CITY-specific code inside H0 kernel/public generic semantics.

## Deterministic proof / evidence

Freeze a representative query suite before implementation verdict. For each query, compute/record an independent expected result from accepted sources or a separately implemented source-side oracle, compare IDs/counts, rebuild the projection and repeat. Capture normalized content-shape output and provenance samples.

## Causal negative-conformance classes

- one accepted projected POI/source record omitted while the source manifest remains complete;
- one district/access/depth/demand classification mutated so expected-set membership changes;
- one result loses source provenance while values remain correct;
- a content-shape bucket ignores an accepted source family and undercounts;
- query order depends on source enumeration rather than normalized deterministic ordering;
- report converts an absent cost input into a fabricated zero/default estimate.

## Content-shape probe

Required. Use the complete accepted CITY universe for every report dimension claimed complete. Queries may return subsets, but their candidate universe must not self-shrink.

## Dependency / IP implications

Repository-owned design data only. No external art asset is adopted by this WP.

## Residual risks

The report describes design-implied content shape, not actual Quaternius coverage, asset-production effort or Unity state. Those require later H1/H2/ART owners.

## Exact predecessor reopen condition

Reopen DW-01 only if a production query exposes a false CITY mapping/invariant assumption. Reopen DW-00 only for a generic projection/provenance/rebuild contradiction. New desirable query families are not reopen evidence by themselves.

## PASS consequence / next dependency

PASS permits `WP-DW-03` once PA-01..05 are accepted locally. It also establishes a real reusable CITY consumer, but does not authorize design↔Unity drift or generated production briefs as accepted product tooling.
