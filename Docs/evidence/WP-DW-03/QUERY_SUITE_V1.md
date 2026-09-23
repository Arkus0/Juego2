# WP-DW-03 — Frozen query suite v1

Suite ID: `dw03-pa-query-suite-v1`
FROZEN_QUERY_SUITE: `dw03-pa-query-suite-v1`
Frozen before implementation verdict. Expected membership is derived independently from the pinned accepted PA authorities, never from projected/query output.

## Q1 — Daily-life rejected/deferred meaning survives compact projection

Query PA-01 findings by disposition flags `contains-reject=true` and `contains-later=true`.

Required source identities include the accepted DL negative/deferred rows (including `DL-11`, `DL-12`, `DL-13`, `DL-14` according to their exact compound dispositions). Each returned finding must expose complete material row text, exact disposition text through `has-disposition`, and source-open provenance.

Purpose: a compact representation cannot keep only adopted recommendations and silently discard negative/rejected/later semantics.

## Q2 — Agency fixture surface is queryable as a complete class

Query PA-02 `pa-fixture` records.

Expected fixture keys from accepted §9: `P1`, `NC-01`, `NC-02`, `NC-03`, `NC-04`, `NC-05`, `NC-06`, `NC-07`.

Purpose: prove positive actor-owned initiative and negative controls coexist in one typed corpus rather than selecting only the easy positive example.

## Q3 — Relationship dispositions remain semantically distinct

Query PA-03 findings by `contains-reject`, `contains-later`, `contains-adapt`, and `contains-adopt` separately. Expected source keys derive from every row of the accepted §4 relationship vocabulary table; mixed rows may appear in more than one flag query because their exact disposition is preserved rather than collapsed.

Purpose: cover a no-explicit-source-ID table and prove stable non-positional identity plus non-lossy compound status.

## Q4 — Epistemic privileged-metadata negative remains reachable

Query PA-04 fixture key `NC-02` and retrieve its complete material section. Also query PA-04 rejected disposition rows.

Purpose: preserve the central `canonical truth != actor belief` negative boundary and its pure privileged/debug-metadata isolation fixture.

## Q5 — Hidden-lineage regression remains linked from PA-05 findings

Query fixture key `NC-02` in the PA-05 delegated fixture authority and then traverse `same-authority-fixture` from PA-05 findings.

Expected fixture title: `NC-02 — HIDDEN_LINEAGE_EPISTEMIC_INVARIANCE`. Every PA-05 H01..H10 finding is structurally linked to the accepted PA-05 fixture surface; the relation is navigation lineage only, not an assertion that NC-02 proves each finding individually.

Purpose: ensure the donor FAIL→repair regression cannot disappear from the compact corpus while findings remain.

## Q6 — Evidence provenance is traversable without becoming authority

Resolve a PA result's evidence records from the accepted provenance-audit surface and traverse a finding's `same-authority-evidence` relation to those records. The query result must expose the accepted evidence material text plus its own source provenance, while the root/finding still points to the canonical PA result as semantic authority.

Purpose: evidence is typed and findable but Design World/evidence does not replace PA authority.

## Q7 — Cross-PA failure family view

Query `failure-family` across all five accepted PAs and require non-empty typed findings/fixtures/failure-mode or invariant records for each of:

- `daily-life`
- `npc-agency`
- `social-graph`
- `knowledge-belief`
- `rumour-flow`

Purpose: demonstrate useful multi-class corpus access rather than a flat finding list.

## Q8 — Source-open compact index

Build the canonical compact index under deliberately reversed input enumeration and normal enumeration.

Required equality: byte-identical compact index and projection digest. Every material fact contributes identity, type, PA, failure family, content digest, exact provenance path/anchor digest and exact typed relation targets. Removing one material fact or relation must change the index and fail semantic validation rather than yielding an apparently valid smaller summary.

## Selection discipline

This suite is intentionally cross-PA and cross-entity-type. It was frozen from the accepted source surfaces before the final implementation verdict. It is not allowed to shrink after observing which cases are easiest to project.
