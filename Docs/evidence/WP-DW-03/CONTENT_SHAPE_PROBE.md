# WP-DW-03 — Content-shape probe

CONTENT_SHAPE_PROBE_VERDICT: PASS
DECLARED_SCHEMA: `dw03-pa-corpus-manifest-v1`
FROZEN_QUERY_SUITE: `dw03-pa-query-suite-v1`

## Claim boundary

DW-03 does not claim that every paragraph in PA-01..05 becomes a first-class Design World entity. It claims losslessness for the complete, reviewed structured universe declared in `SOURCE_MANIFEST_V1.md`, while keeping every projected record source-open and keeping the accepted PA documents as authority.

The declared v1 universe is therefore neither a hand-picked positive subset nor a replacement research corpus. It deliberately contains positive findings, rejected/deferred meanings, negative gates, failure modes, evidence/provenance rows, counterfactual/negative fixtures, dispositions and the relations required by the frozen query suite.

## Adopted semantic surfaces

| PA | Complete adopted surface for DW-03 v1 |
|---|---|
| PA-01 | one corpus root; every §1 provenance row; all 14 §3 DL findings and their explicit dispositions; all 10 §5 invariants; all §6 P1–P4 and NC-01–NC-04 fixtures |
| PA-02 | one corpus root; every §1 provenance row; all 15 §3 AG findings and dispositions; all 6 §4 bounded-discovery requirements; §9 P1 plus NC-01–NC-07 fixtures |
| PA-03 | one corpus root; every §1 provenance row; all 20 §4 relationship/mechanism rows and dispositions; §8 CF-01–CF-03 plus NC-01–NC-05; every row of §13 failure modes |
| PA-04 | one corpus root; every §1 provenance row; all 18 §11 mechanism rows and dispositions; §12 CF-01–CF-03; §13 NC-01–NC-06; every row of §17 failure-mode audit |
| PA-05 | one corpus root; all 9 accepted §1 donor-provenance bullets; PA-05-H01..H10 and each exact `Decision`; all 14 §7 hard negative gates; complete delegated fixture set CF-01, NC-01, CF-02, NC-02, CF-03, CF-04, CF-05 from `Docs/evidence/WP-PA-05/TRANSFER_AND_FIXTURES.md` |

The source bytes are pinned independently by accepted Git blob SHA. A source deletion/rename/rewrite cannot silently reduce this universe: production rejects unreviewed source bytes and the source-side oracle separately reconstructs the same accepted authority set without consuming the production registry or projected counts.

## Adopted entity and relation shape

Entity classes are `pa-corpus-root`, `pa-finding`, `pa-disposition`, `pa-evidence`, `pa-fixture`, `pa-invariant` and `pa-failure-mode`.

For every PA family, the root contains every typed child through the corresponding `contains-*` edge and every child has exactly one `declared-in` edge back to the root. Every finding has exactly one `has-disposition` target. Findings additionally expose `same-authority-evidence` and `same-authority-fixture` navigation to all adopted evidence/fixture records for that accepted PA authority.

Those navigation links are intentionally coarse. They mean “same accepted authority surface is available for compact traversal”; they do not claim that each fixture individually proves each finding. Provenance on every record remains the route back to the exact accepted source material.

## Material fields

The model preserves stable PA identity, record class, failure family, source key, complete adopted material text, explicit disposition text and disposition flags (`ADOPT`, `ADAPT`, `LATER`, `REJECT`, `baseline`) where applicable. A compact query result therefore cannot preserve only an ID/count while silently weakening accepted meaning.

## Deliberately unmodeled in v1

The following are explicitly outside the structured DW-03 claim and remain available through the original accepted PA sources rather than being silently summarized:

- narrative/explanatory prose outside the reviewed structured surfaces above;
- donor runtime architecture, schemas, APIs, tuning and old milestone routing;
- external donor documents as authority in Juego2;
- Worker/Reviewer/process state;
- CTX policy and agent-context/token optimisation, which belong to CTX/DW-04;
- Unity/runtime implementation or H0 domain-specific semantics;
- future PA-06+ work, which requires ordinary PA acceptance plus an explicit reviewed manifest/schema extension before projection.

## Anti-cherry-pick result

The frozen query suite crosses all five PA families and multiple entity classes. It requires rejected/later meanings, negative fixtures, hard failure gates, evidence navigation, cross-family failure views and deterministic compact output. Causal controls remove or corrupt a finding, evidence item, whole disposition surface, material disposition, negative/failure item, fixture, relation, provenance and compacted material fact while keeping the generic projection self-consistent where useful.

Result: the declared structured corpus is complete for the DW-03 v1 claim, exclusions are explicit and source-open, and no smaller self-defined projected universe is accepted as equivalent.