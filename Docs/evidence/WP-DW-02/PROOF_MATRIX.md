# WP-DW-02 — foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: DW-02 owns deterministic production queries and content-shape report v1 over the complete accepted CITY-02 §4 programme ledger, including an explicit reviewed field manifest mechanically bound to the accepted table header schema, independent source-side completeness checks, exact-row provenance, rebuild determinism, classification movement, a standalone machine-readable report artifact with per-subject provenance and explicit unmodeled-cost handling. CITY design quality, CITY-05/06 semantics beyond inherited DW-01 guarantees, realized geometry, Unity state, art coverage and production-cost modelling are outside the claim.
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

`READY` describes repository-backed proof obligations. Final exact-HEAD execution, Worker pre-review/freeze and independent Reviewer acceptance remain lifecycle gates and do not redefine the semantic proof boundary.

## Independent universe/oracles

- **Source schema:** `CityProductionProjectionManifest.ProgrammeHeaders` pins the complete ordered 10-column CITY-02 §4 table header; production refuses to parse if any header is renamed or reordered, so positional reads cannot silently reinterpret changed source bytes.
- **Subject universe:** every data row under accepted CITY-02 §4 `District × location programme`; production code enumerates all rows before projection, while `Dw02CityProductionQueryTests` separately parses source bytes without consuming the production parser/manifest/queries and compares exact subject identities/tuples.
- **POI universe:** independent source-side identity prefix `loc.*` over that complete table, with expected `(district,id)` set compared to Q1.
- **Access-role universe:** independent source-side narrow normalization over every table row, with exact IDs compared per role.
- **Higher-depth universe:** independent source-side I-prefix parsing over every row, with exact `(I3/I2,id)` membership compared to Q3.
- **Report universe:** independent complete per-subject `(district,A-D,S0-S4,I0-I3,roles,kind)` tuples aggregated test-side and compared to production report buckets, including explicit zero cross-tab cells.
- **Report provenance oracle:** the standalone content-shape artifact must enumerate exactly one provenance record per counted subject; subject IDs, exact CITY-02 path, source digest, row anchor and anchor digest are serialized before aggregate buckets.
- **Provenance oracle:** result construction rejects non-CITY-02 source paths; inherited generic validator detects changed source bytes; positive proof compares exact source-row anchor for every current fact.
- **Determinism oracle:** clean rebuild + reverse definition/universe enumeration must preserve projection digest/normal form and the canonical machine-readable query/report snapshot including report provenance.

## Proof-obligation matrix

| Proof obligation | Claim/trust-boundary scope | Completeness argument | Positive evidence | Negative control / defect injection | Result | Residual risk |
|---|---|---|---|---|---|---|
| Reviewed manifest covers every source field consumed by frozen suite and is bound to accepted schema | DW-02 projection boundary | manifest pins the seven adopted fields and the complete ordered 10-column source header; parser validates exact ordinal equality before reading positional cells | `ManifestDeclaresExactReviewedSourceColumnsForFrozenSuite`; `ManifestPinsCompleteAcceptedProgrammeHeaderSequence` | `RenamingConsumedReviewedHeaderFailsClosedBeforeProjection`; `ReorderingConsumedReviewedHeadersFailsClosedBeforeProjection` → `city.query_source_shape_invalid` | GREEN by repository proof; exact-SHA execution required | Intentional source schema changes require reviewed manifest/parser update rather than heuristic recovery. |
| Every accepted CITY-02 programme row is projected | query/report completeness | expected identities derive from all source rows independently of projected facts | independent oracle equals `AllSubjects` exact IDs | omit one `loc.*` projected fact while full expected IDs remain → `city.query_subject_missing` | GREEN by causal test | Git/filesystem source read is trusted base. |
| Ordinary/scenic families cannot disappear from report denominator | content-shape completeness | complete universe includes both `loc.*` and `fam.*` | current source proof includes both kinds | drop entire `fam.*` projected class → deterministic missing-subject RED | GREEN by causal test | A future new identity family intentionally fails closed pending manifest review. |
| Q1 returns all declared POIs by district | production query | independent `(district,id)` set selected directly from source IDs | full accepted-source comparison | generic subject omission control invalidates query service before smaller Q1 can be accepted | GREEN | Parcel/site placement is not claimed. |
| Q2 returns all subjects requiring selected access role | production query | independent source parser normalizes all rows under accepted four-role rule | exact set comparison for all four roles | subject omission remains visible against full source universe | GREEN | Runtime availability/permissions remain out of scope. |
| Q3 returns all I2+ higher-depth interiors | production query | independent source I-depth parser over complete ledger | exact ordered I3/I2 set comparison | source classification mutation rebuilds and changes result/report | GREEN | Detailed interior layout remains CITY-06. |
| Q4 report matches complete accepted content shape | report completeness | independent per-row tuples aggregate A-D/S/I/district/role/kind; 20 A-D×S cells are explicit | exact bucket-map comparison | drop `fam.*` class → RED; S4→S3 source mutation moves exact A/S counts | GREEN | RD-1..4 await realized traversal/runtime evidence. |
| Q4 standalone artifact remains auditable back to every counted source fact | report provenance | artifact provenance cardinality must equal report `TotalSubjects` and serializes every sorted projected identity before aggregates | `ContentShapeArtifactCarriesProvenanceForEveryCountedSubject` asserts path/anchor/source digest/anchor digest for each contributor | artifact constructor rejects provenance/report cardinality mismatch as `city.report_provenance_universe_mismatch` | GREEN | Provenance identifies accepted design facts, not downstream Unity realizations. |
| Query results preserve actionable source provenance | provenance | service rejects wrong source path; generic projection owns exact digest/anchor | every current fact anchor equals independent source row | equal values + wrong source path → `city.query_provenance_invalid`; changed bytes → `dw.provenance_stale` | GREEN | Hash/Git/filesystem primitives are trusted base. |
| Equal source/schema rebuilds are deterministic | deterministic output | generic normalization plus service/artifact ordinal ordering | repeated and reverse-enumeration builds equal | reverse input enumeration must preserve projection and canonical machine-output byte equality | GREEN | Cross-runtime out-of-contract behavior remains trusted infrastructure. |
| Missing cost inputs are not converted into estimates | non-invention | report schema has only explicit `UNMODELED` cost state and source-derived counts | normalized output asserts marker | test rejects numeric-zero/default cost keys | GREEN | Future cost model needs separate reviewed authority/model. |
| H0 remains generic/downstream | architecture boundary | code exists only in `Arkus.DesignWorld`; exact-SHA observer scans H0 and project refs | generic `dw.fact` WorldState asserted | reverse dependency/query vocabulary in H0 makes observer RED | IMPLEMENTED; final exact-SHA execution required | Generic H0 correctness outside seam is inherited. |
| Canonical exact-SHA route exists | lifecycle evidence | dedicated observer/verifier registered in canonical dispatch | scripts require focused tests + full regression + evidence/freeze metadata | missing dispatch/evidence/dirty SHA/frozen metadata returns nonzero | IMPLEMENTED; final exact-SHA execution required | CI runner is trusted base. |

## Review repair

Independent review `#5283775986` identified the original false-green where production and the source-side oracle both consumed positional columns while only `Programme ID` plus width were checked. The repair makes the complete accepted header schema executable production authority and adds explicit rename/reorder REDs; DW-01/DW-00 guarantees are unchanged.

## Freeze gate

Canonical frozen verification: `scripts/arkus-verify-exact-sha.sh <candidate-sha>` with PR metadata supplied by workflow. Repository evidence does not substitute for the resulting exact-SHA execution receipt.
