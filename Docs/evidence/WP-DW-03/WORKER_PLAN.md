# WP-DW-03 — Worker plan

Date: 2026-09-22
Baseline SHA: `e7d15a3a6ddcba51848b738fc6f33b69cf1fd8d1`
Branch: `work/wp-dw-03`
Contract: `Docs/workpacks/DW/WP-DW-03.md`
Class: FOUNDATIONAL / REMOTE_OK

## Claim under proof

The accepted PA-01..05 semantic corpus can be projected as typed findings, dispositions, evidence, fixtures, invariants/failure modes and material relations; queried compactly without losing accepted negative/deferred meaning; and traced back to accepted PA authority without making Design World authoritative.

## Authority audit

Semantic authority adopted by the projection:

1. `Docs/research/living-world/results/PA-01.md`
2. `Docs/research/living-world/results/PA-02.md`
3. `Docs/research/living-world/results/PA-03.md`
4. `Docs/research/living-world/results/PA-04.md`
5. `Docs/research/living-world/results/PA-05.md`
6. `Docs/evidence/WP-PA-05/TRANSFER_AND_FIXTURES.md` only for the exact mandatory PA-05 fixture surface explicitly delegated to it by the accepted PA-05 result.

Acceptance/process evidence (PA workpacks, reviews, DocSync, handoffs) establishes that these results are accepted but is not projected as semantic truth. Donor repository material is provenance referenced by accepted PA results, not direct DW-03 authority.

Each adopted source is pinned to the accepted Git blob identity in the reviewed manifest. A source-byte change, including deletion of a source item that would otherwise shrink a dynamically parsed universe, must fail closed until the manifest is intentionally reviewed and updated.

## Reviewed structured surfaces

DW-03 will project complete selected structured semantic surfaces rather than cherry-pick convenient rows:

- finding/disposition surfaces: PA-01 donor→Juego2 disposition matrix, PA-02 mechanism disposition matrix, PA-03 minimal relationship vocabulary matrix, PA-04 Juego2 disposition matrix, and PA-05 H01–H10 findings with their declared decisions;
- evidence: exact accepted provenance-audit entries carried by each canonical PA result;
- invariants/failure modes: the explicitly structured invariant/requirement/failure-mode surfaces named in the source manifest;
- fixtures: all explicitly identified P/CF/NC fixtures in the manifest-owned fixture sections, with PA-05 fixture bodies sourced from its accepted delegated fixture file;
- dispositions preserve the complete source text, not a lossy enum that would collapse `ADOPT`, `ADAPT`, `LATER`, `REJECT`, baseline, conditional or mixed forms.

Narrative exposition, future consumer notes, deferred runtime proof, reviewer/process metadata and duplicate restatements are deliberately outside the typed row model. They remain reachable through per-record source provenance and are not claimed to be replaced by the projection.

## Implementation shape

- `PaProjectionManifest`: executable reviewed source/schema contract, accepted blob pins, adopted section/header signatures, entity types, exact consumed fields/relations and exclusions.
- `PaDesignWorldProvider`: fail-closed production parser/adapter over accepted source text; builds a generic `DesignWorldProjection`; validates generic DW guarantees; then invokes the PA semantic oracle before returning a corpus.
- `PaSourceCorpusOracle`: production semantic oracle that independently reparses the accepted authority bytes without consuming the production manifest/parser/intermediate projection definitions. It compares identity, material fields, relation names/cardinality/targets and exact provenance.
- `PaCorpusQueries`: deterministic compact typed queries by PA/failure family/disposition/fixture/evidence relation, preserving source-open pointers and full disposition/material text.
- internal injection seams only for defect-injection/wiring proof; public production construction always uses the real oracle.

The relation vocabulary will distinguish structural containment from semantic/navigation lineage. A finding owns exactly one `has-disposition` relation. `same-authority-evidence` and `same-authority-fixture` mean only that the target is an accepted evidence/fixture record under the same PA authority; they do not claim that every fixture proves every finding. This bounded relation is intentional so compact consumers can traverse finding→evidence/fixture without inventing causal research support.

## Independent oracle discipline

The source oracle will not consume `PaProjectionManifest`, production parsed rows, `AnchoredFactDefinition` collections, projected IDs/counts or `PaCorpusQueries`. It has its own exact source selectors/schema expectations and reconstructs expected typed objects directly from pinned authority text. The projected corpus therefore cannot define its own expected universe.

Defect-injection tests will also build internally self-consistent corrupted projections/readers so the inherited generic validator can remain GREEN while the PA semantic oracle is RED. This is required for relation/content/provenance mutations where producer+generic validation could otherwise self-confirm.

## Frozen query suite before implementation verdict

The suite will be persisted before freeze and cover multiple PAs and types: accepted/adapted/deferred/rejected dispositions; a daily-life finding + fixture traversal; agency negative controls; PA-03 relationship semantics; PA-04 epistemic negatives; PA-05 hidden-lineage fixture; evidence-source traversal; and negative/deferred records that a lossy compact summary would be tempted to omit. Expected result identities come from an independent source-side test oracle, not from query output.

## Required causal negatives

At minimum the proof will isolate: one finding omitted; one evidence item omitted; one disposition removed/changed; one negative/failure record removed; one fixture omitted; relation removed; relation renamed; wrong relation target; duplicate/cardinality extra; equal values with forged provenance; reverse enumeration determinism; compressed output with one material fact removed. Source-header/schema rename/reorder and accepted-source blob mutation will also fail closed. A wiring test will fail if `BuildAndValidate` stops invoking the semantic oracle.

## Proof boundary / budget

Trusted: Git checkout/file bytes supplied to the provider, .NET string/UTF-8/hash primitives, accepted generic DW-00 projection machinery, accepted PA source semantics. In claim: PA adapter/schema/oracle/query behavior and repository-owned tests/scripts/evidence. Out of claim: runtime H3/H4/H7 implementation, PA-06+, Unity, context/agent optimisation (DW-04), new PA research, subjective scoring and H0 semantic changes.

The plan intentionally reuses accepted DW generic machinery rather than rebuilding provenance/normalization. New proof machinery is limited to PA-specific source interpretation, losslessness and compact-query claims.

See `PREDECESSOR_CONTRACT_CHECK.md` for inherited/reopen classification.