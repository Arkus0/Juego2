# WP-DW-03 — Foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
PROOF_BUDGET_VERDICT: WITHIN_BUDGET
TRUST_BOUNDARY: DW-03 owns the reviewed PA-01..05 structured projection vocabulary, independent source-side losslessness oracle, compact deterministic query surface, relation/provenance fidelity and production wiring. Accepted PA source documents remain authority. DW-04 owns context/agent efficiency; runtime/Unity/H0 domain semantics are not claimed.

`READY` describes repository-backed proof obligations. Final exact-HEAD Worker pre-review/freeze and canonical exact-SHA verification remain lifecycle gates and do not redefine the semantic proof boundary.

## Independent universe / oracle structure

- **Accepted-source universe:** six source authorities are pinned by independently reviewed Git blob identity: PA-01..05 canonical result files plus the PA-05 delegated fixture authority. Production cannot accept changed source bytes as the same universe.
- **Independent semantic oracle:** `PaSourceCorpusOracle` reconstructs expected objects, fields, exact relation multisets and provenance directly from accepted sources. It does not consume `PaProjectionManifest`, `PaProductionCorpusParser`, `PaRecordDefinition`, compact query output or projected counts.
- **Reviewed source schema:** structured tables/headings/key sets are explicit. Production fails closed on incompatible schema; focused rename/reorder controls demonstrate that positions cannot silently acquire new meaning.
- **Generic-vs-semantic separation:** synthetic mutated projections use a self-confirming generic authority/universe so inherited generic validation can remain GREEN while the DW-03 source oracle detects PA-specific loss.
- **Determinism oracle:** normal and reversed enumeration must produce equal projection digest/normalized representation and byte-identical compact index.
- **Wiring oracle:** a sentinel `IPaCorpusSemanticOracle` proves `PaDesignWorldProvider.BuildAndValidate()` actually invokes the semantic oracle before delivery.

## Proof-obligation matrix

| Claim | Independent universe / positive evidence | Negative causal control | Expected/result | Residual risk / reopen condition |
|---|---|---|---|---|
| Source loss cannot redefine its own universe | accepted source blobs are fixed independently from projection; normal build accepts exact bytes | rename one accepted PA-01 finding key while otherwise retaining source structure | production RED `pa.accepted_source_blob_mismatch`; source-side universe cannot shrink with the defect | intentional PA revision requires reviewed manifest/schema extension; PA itself reopens only under PA evidence rules |
| Consumed schema is causally bound | exact reviewed table/header/section shapes in manifest and parsers | rename a consumed PA-01 header; reorder consumed header fields | RED `pa.source_shape_invalid` before positional interpretation | intentional schema changes require reviewed adapter update |
| Every finding survives | source oracle reconstructs all adopted finding identities/content | omit exactly one PA-01 finding and remove references to it in a self-confirming projection | inherited generic validator GREEN; DW-03 oracle RED `pa.semantic_fact_missing` | none in declared v1 surface |
| Every evidence item survives | source oracle reconstructs all adopted evidence records | omit one PA-02 evidence record while retaining the finding corpus | generic GREEN; semantic RED `pa.semantic_fact_missing` | evidence remains navigation/provenance, not replacement authority |
| Disposition surface cannot disappear | each finding has a separately reconstructed exact disposition record | remove the complete PA-03 disposition class and its edges | generic GREEN; semantic RED on missing records | future disposition vocabulary extension requires schema review |
| Disposition meaning cannot be weakened behind stable IDs/counts | exact `material-text`, `disposition-text` and flags are source-derived | rewrite one rejected disposition to ADOPT while preserving record identity | generic GREEN; semantic RED `pa.semantic_content_mismatch` | keyword flags supplement, not replace, exact disposition text |
| Material negative/exception knowledge survives | failure-mode/hard-negative records are part of independent universe | omit one PA-05 hard negative/failure-mode record | generic GREEN; semantic RED `pa.semantic_fact_missing` | narrative outside declared structured surface is explicitly unmodeled/source-open |
| Fixture completeness survives | all declared PA fixtures, including PA-05 delegated fixtures, are independently enumerated | omit PA-05 `NC-02` while findings remain | generic GREEN; semantic RED `pa.semantic_fact_missing` | no runtime execution claim is made for research fixtures |
| Relation existence is lossless | oracle independently reconstructs exact relation multiset | remove one `has-disposition` while both endpoints remain | generic GREEN; semantic RED `pa.semantic_relation_missing` | relation navigation semantics are deliberately documented as coarse same-authority links |
| Relation names cannot drift | exact relation name+target pairs are compared | rename `has-disposition` only | generic GREEN; semantic RED `pa.semantic_relation_mismatch` | schema version bump required for intentional rename |
| Relation targets cannot self-confirm | expected target derives from source identity/disposition pairing | retarget `has-disposition` to a different valid disposition | generic GREEN; semantic RED `pa.semantic_relation_mismatch` | none in declared pairing rule |
| Cardinality cannot inflate invisibly | oracle compares relation multiset, not mere existence | add a second valid `has-disposition` target (not a generic duplicate edge) | generic GREEN; semantic RED `pa.semantic_relation_extra` | inherited H0 still catches exact duplicate references earlier; DW-03 proves the PA-specific extra-cardinality class |
| Equal values do not excuse forged provenance | oracle independently derives authority id, source path, full anchor, source digest and anchor digest | keep content/identity equal but forge authority provenance | generic self-confirming projection GREEN; semantic RED `pa.semantic_provenance_mismatch` | Git/hash/filesystem primitives remain trusted base |
| Projection is deterministic | repeated and reverse-enumeration builds use the same accepted sources | reverse definition/universe enumeration | equal projection digest, normalized representation and compact-index bytes | cross-runtime primitives are inherited infrastructure |
| Compact retrieval preserves declared material meaning | frozen Q1–Q8 traverse rejected/later dispositions, fixtures, evidence and five failure families with source-open records | build a smaller compact view by omitting one material failure record | reduced index is observably smaller; generic projection can remain GREEN; semantic oracle RED | DW-04, not DW-03, measures whether this improves agent context usage |
| Production cannot bypass the semantic oracle | provider owns `_oracle.Validate()` in delivery route | inject sentinel oracle whose only behavior is RED and record invocation | `BuildAndValidate()` calls sentinel and fails; deleting/bypassing invocation breaks wiring test | future provider refactor must preserve this causal route |
| Future PA output is not silently pre-authorized | independent source universe is exactly accepted PA-01..05 + reviewed PA-05 fixtures | add synthetic `pa06.finding.unreviewed` to otherwise valid self-confirming projection | generic GREEN; semantic RED `pa.semantic_fact_unexpected` | future accepted PA requires ordinary PASS/merge/DocSync and reviewed extension |
| Design World does not become PA authority | positive query records carry exact accepted-source provenance; sources are immutable input | authority/source/provenance corruption and unexpected records are rejected | GREEN only when projection remains a rebuildable derived view | reopen DW-00 only for a demonstrated generic provenance/rebuild limitation |
| H0 remains generic | implementation and queries live downstream in `Arkus.DesignWorld`; canonical observer executes inherited dependency/domain-neutrality guards | reverse H0 dependency/domain leakage would fail canonical gates | repository proof implemented; final exact-SHA lifecycle run required | reopen inherited boundary only on concrete contradiction, not convenience |

## Frozen query proof

`QUERY_SUITE_V1.md` was fixed before implementation verdict. The positive suite exercises PA-01 rejected/deferred meaning, PA-02 full fixture class, PA-03 compound dispositions, PA-04 privileged-metadata negative, PA-05 hidden-lineage regression, evidence traversal, five failure families and deterministic source-open compact indexing. It is intentionally not reducible to a flat list of findings.

## Reopen classification

- **DW-00:** only a demonstrated generic provenance/rebuild/normalization limitation that cannot be repaired in the PA provider/oracle.
- **DW-01/DW-02:** only a concrete contradiction in the inherited semantic-oracle/schema-binding guarantees on the effective path.
- **Accepted PA research:** only through normal PA evidence rules when the accepted semantics themselves are contradicted.
- **Otherwise:** representational/schema/query defects remain DW-03 defects.

No such predecessor contradiction was found during this proof cycle.