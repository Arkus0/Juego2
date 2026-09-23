# WP-DW-05 — Foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: Accepted generic DW fact/relation/provenance/projection seams plus the reached H0 kernel dependency chain `Arkus.Game.World -> Arkus.Game.Core`; the neutral probe fixture, predecessor-residual reconciliation and bounded H2 planning input are inside claim. Substantive CITY/PA truth, arbitrary future domains, runtime/Unity realization, external packaging and normal documented .NET/Git/CI primitives remain outside or trusted as declared below.
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

`READY` here means the Worker has implemented executable coverage for every declared DW-05 proof class and has no known uncovered in-claim defect class. It is not an independent Reviewer PASS.

## Acceptance-criterion matrix

| Proof obligation | Claim / trust-boundary scope | Completeness argument | Positive evidence | Negative control / defect injection | Result | Residual risk |
|---|---|---|---|---|---|---|
| Generic DW/H0 contracts and reached kernel dependencies require no CITY/PA vocabulary or behavior | Generic DW public contract/projection files and effective H0 project-reference chain through `Arkus.Game.World -> Arkus.Game.Core` | The reached dependency chain is derived from project references; Game.Core is checked to have no further project reference. Audited source/project files are finite and baseline-pinned. | `GenericDwAndH0DependencyClosureContainsNoCityOrPaLeakageAndInjectedLeakageReds`; `DesignWorldH0DependencyClosureIncludesGameCoreAndContainsNoCityOrPaLeakage` | Inject synthetic `CityKernelRule` and transitive `PaKernelDependency`; both must RED | GREEN | Future dependencies require the same reached-closure audit; arbitrary future graph universality is not claimed. |
| Neutral third-shape can project, query, resolve provenance and rebuild using accepted generic semantics only | Observatory/calibration fixture and existing generic DW/H0 seams; probe-local schema/query/oracle glue is allowed | Independent 3-fact universe uses every accepted `DesignValue` kind, two relation kinds, public projected facts, generic validator, H0 world projection and diff/rebuild without production seam changes. | `NeutralThirdShapeProjectsQueriesValidatesAndRebuildsThroughAcceptedGenericSeams`; `NeutralAuthorityFixtureIsRealSourceOpenableAndProjectsThroughGenericPath` | Baseline blob/diff guards reject generic/H0 seam edits made to fit the probe | GREEN | One neutral shape does not prove arbitrary-domain universality or external packaging. |
| A causal invalid neutral mutation REDs through the same generic validation/provenance path | Neutral authority/provenance and structural relation boundary | Independent required universe stays fixed while authority bytes/targets are corrupted, so the defect cannot disappear from the oracle universe. | Positive neutral build/validate path is GREEN | Missing authority requires `dw.provenance_missing`; stale authority requires `dw.provenance_stale`; target outside universe requires `dw.relation_target_outside_universe` | GREEN | Domain-required-but-structurally-valid relation omission remains consumer-oracle-owned and is preserved separately as generic GREEN + consumer RED. |
| Every DW-00..04 limitation/residual is enumerated and assigned exactly one classification/owner | Accepted predecessor workpacks, architecture candidates, DW-00..03 explicit `RESIDUAL_RISK.md`, and DW-04 accepted bounded-result evidence | Inventory v2 anchors exact source text; an independent extractor exact-matches every explicit accepted DW-00..03 residual section and explicitly handles DW-04's lack of a residual file. Manifest reconciliation requires one entry per inventoried id. | `AcceptedDw00ThroughDw03ResidualSectionsExactlyMatchIndependentInventoryAnchors`; `PredecessorResidualManifestReconcilesIndependentEvidenceInventory` | Drop one manifest entry or shrink/misclassify inventory; omission and unsupported generic-kernel classification must RED | GREEN | New accepted predecessor evidence after this frozen baseline would require a new reviewed inventory version. |
| DW-05 silently changes no accepted H0/generic semantics | Baseline accepted generic DW files plus reached H0 kernel files | Canonical observer diffs exact baseline→candidate bytes; independent tests pin Git blob identities | `AcceptedGenericDwAndH0SeamBytesRemainAtBaselineBlobIdentities`; `scripts/dw05-observe-exact-sha.sh` | Any mutation to the guarded generic/H0 files makes canonical observation RED | GREEN | Probe/test/evidence/dispatcher glue is new but does not alter production semantics. |
| H2 input states exercised capabilities and remaining limitations without upgrading planning ideas into contracts | `H2_BOUNDARY_INPUT_V1.{json,md}` + limitation manifest | Report names CITY, PA, DW-04 and neutral-probe exercised slices; remaining limits are grouped explicitly and machine-classified | `H2InputKeepsUntestedDownstreamOpportunitiesOutOfAcceptedCapabilities` + report/manifest | Promote named downstream candidate into accepted capabilities or flip generalization flags; test must RED | GREEN | H2 must independently disposition this planning input; it is not binding scope. |
| Named downstream candidates retain owner/dependency prerequisites | Design↔Unity drift, art briefs, catalogue coverage, replay QA, narrative knowledge | Report enumerates all five required candidate ids as optional opportunities with prerequisite/owner text | H2 markdown + JSON optional-opportunity set | Removing a required opportunity from the JSON set makes the H2 input test RED | GREEN | Actual implementation/proof remains with H1/H2/ART/narrative owners. |
| No arbitrary-domain universality, external-repository consumability or general-DW-adoption claim is emitted | Worker result + H2 planning report | Explicit false/NO claims are machine-readable and repeated in human evidence | `claims.* = false` in H2 JSON; `GENERAL_DW_ADOPTION_AUTHORIZED: NO` and related report markers | Flip universality/external-consumption flag or promote corresponding capability; H2 test REDs | GREEN | Three bounded shapes still do not prove universality. |

## Detailed causal proof classes

| Claim / defect class | Independent/effective oracle | Positive / negative proof |
|---|---|---|
| Neutral non-CITY/non-PA shape is representable | frozen independent fact universe: `observatory.north`, `detector.spectro`, `calibration.lamp-a` | `NeutralThirdShapeProjectsQueriesValidatesAndRebuildsThroughAcceptedGenericSeams` plus source-open fixture proof |
| Public generic queryability | expected remote-observatory and calibration-target sets are test-owned, not projection-enumerated | same positive test |
| Provenance resolves to a real frozen authority fixture | repository-owned `neutral-observatory.txt`, independent authority id/path and exact anchors | `NeutralAuthorityFixtureIsRealSourceOpenableAndProjectsThroughGenericPath` |
| Clean rebuild is deterministic | independent second reader/build + digest/normalized/diff equality | neutral positive test |
| Missing authority/provenance fails closed | required universe remains fixed while authority line is removed | `MissingNeutralAuthorityEdgeFailsClosedThroughGenericProvenancePath` requires `dw.provenance_missing` |
| Stale authority fails closed | original projection is validated against independently changed source bytes | `StaleNeutralAuthorityBytesTurnExistingProjectionRed` requires `dw.provenance_stale` |
| Structurally invalid relation target fails | independent universe excludes `calibration.ghost` | `RelationTargetOutsideIndependentNeutralUniverseFailsGenericProjection` requires `dw.relation_target_outside_universe` |
| Domain semantic necessity is not silently attributed to H0 | independent neutral semantic oracle requires `calibrated-by` while generic validator remains structural | `RequiredNeutralRelationOmissionIsDetectedByProbeOracleWithoutPretendingH0OwnsDomainSemantics` records generic GREEN + consumer RED |
| Shared generic/H0 dependency closure is CITY/PA-neutral | generic DW files plus reached H0 chain `Arkus.Game.World -> Arkus.Game.Core`; Game.Core is independently checked to have no further `ProjectReference` | leakage tests plus injected CITY/PA negatives |
| DW-05 did not silently patch accepted generic/H0 semantics | baseline Git blob identities + baseline→candidate canonical diff over generic DW, Game.World and reached Game.Core | blob-identity test plus canonical observer |
| Accepted predecessor residual sections are completely inventoried | exact-section extraction from accepted DW-00..03 residual evidence; explicit DW-04 bounded-evidence check | `Dw05ResidualInventoryCompletenessTests` |
| Every inventoried predecessor residual is reconciled | source/anchor inventory verified against source bytes; every limitation id resolves to exactly one manifest entry | residual reconciliation tests |
| Domain-only need cannot be promoted to kernel flaw without causal seam evidence | classification rule requires public-seam failure evidence for generic deficiency | misclassification negative test |
| H2 report cannot silently promote named downstream opportunities | exact required opportunity ids + false generalization flags | H2 input negative test |

## Trust boundary / proof budget

Trusted rather than recursively re-proved:

- accepted H0 world-state behavior outside the generic surfaces actually consumed by the projector;
- substantive truth of accepted CITY and PA sources;
- DW-04's accepted frozen experiment result;
- .NET/xUnit/Git hashing implementations and repository CI substrate.

The proof attacks the material causal boundaries owned by DW-05: neutral representability, generic provenance/rebuild, reached kernel/domain leakage, silent H0 mutation, predecessor-residual completeness/classification and H2 overclaim. It does not expand into arbitrary compiler/toolchain corruption or future unclaimed domains.

The representative-product-content probe rule in `FOUNDATIONAL_PROOF_STANDARD.md` is not triggered by DW-05 because this workpack neither defines nor changes authorable-state/public-contract semantics; its contract instead explicitly requires the neutral synthetic third-shape falsification probe. The accepted generic/H0 seam is byte-guarded against semantic change.

## Strict pre-review repair retained in the proof surface

The Worker pre-review found that the first residual inventory could self-confirm completeness because it was anchored mainly to predecessor workpacks/architecture and did not enumerate every explicit accepted `RESIDUAL_RISK.md` item. That proof defect was repaired before freeze by inventory v2 plus a mechanically independent exact-section extractor. This was a repairable evidence defect, not an architectural falsification result; the repaired candidate must still receive a fresh exact-SHA preflight and independent review.

## Falsification disposition

No causal negative was weakened to obtain GREEN. In particular, the semantic-relation omission control is preserved as evidence that semantic necessity remains consumer-owned; DW-05 does not retrofit domain semantics into the generic validator.
