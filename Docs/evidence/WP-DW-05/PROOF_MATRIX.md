# WP-DW-05 — Foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: READY  
UNRESOLVED_PROOF_OBLIGATIONS: 0  
KNOWN_UNDETECTED_DEFECT_CLASSES: 0  
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

`READY` here means the Worker has implemented executable coverage for every declared DW-05 proof class and has no known uncovered in-claim defect class. It is not an independent Reviewer PASS.

| Claim / defect class | Independent/effective oracle | Positive / negative proof |
|---|---|---|
| Neutral non-CITY/non-PA shape is representable | frozen independent fact universe: `observatory.north`, `detector.spectro`, `calibration.lamp-a` | `NeutralThirdShapeProjectsQueriesValidatesAndRebuildsThroughAcceptedGenericSeams` |
| Public generic queryability | expected remote-observatory and calibration-target sets are test-owned, not projection-enumerated | same positive test |
| Provenance resolves to the frozen authority | independent authority id/path and exact anchors | same positive test |
| Clean rebuild is deterministic | independent second reader/build + digest/normalized/diff equality | same positive test |
| Missing authority/provenance fails closed | required universe remains fixed while authority line is removed | `MissingNeutralAuthorityEdgeFailsClosedThroughGenericProvenancePath` requires `dw.provenance_missing` |
| Stale authority fails closed | original projection is validated against independently changed source bytes | `StaleNeutralAuthorityBytesTurnExistingProjectionRed` requires `dw.provenance_stale` |
| Structurally invalid relation target fails | independent universe excludes `calibration.ghost` | `RelationTargetOutsideIndependentNeutralUniverseFailsGenericProjection` requires `dw.relation_target_outside_universe` |
| Domain semantic necessity is not silently attributed to H0 | independent neutral semantic oracle requires `calibrated-by` while generic validator remains structural | `RequiredNeutralRelationOmissionIsDetectedByProbeOracleWithoutPretendingH0OwnsDomainSemantics` records generic GREEN + consumer RED |
| Shared generic/H0 dependency closure is CITY/PA-neutral | generic DW files plus reached H0 chain `Arkus.Game.World -> Arkus.Game.Core`; Game.Core is independently checked to have no further `ProjectReference` | `GenericDwAndH0DependencyClosureContainsNoCityOrPaLeakageAndInjectedLeakageReds` plus `DesignWorldH0DependencyClosureIncludesGameCoreAndContainsNoCityOrPaLeakage`; injected `CityKernelRule` and transitive `PaKernelDependency` must RED |
| DW-05 did not silently patch accepted generic/H0 semantics | baseline Git blob identities + baseline→candidate canonical diff over generic DW, Game.World and reached Game.Core | `AcceptedGenericDwAndH0SeamBytesRemainAtBaselineBlobIdentities` plus `scripts/dw05-observe-exact-sha.sh` |
| Every predecessor residual is reconciled | independent source/anchor inventory in `PREDECESSOR_RESIDUAL_INVENTORY.json` | `PredecessorResidualManifestReconcilesIndependentEvidenceInventory` |
| Residual omission fails | independent inventory remains complete while manifest clone drops one id | `ResidualReconciliationRedsForOmissionAndUnsupportedKernelClassification` |
| Domain-only need cannot be promoted to kernel flaw without causal seam evidence | classification rule requires public-seam failure evidence for generic deficiency | same negative test |
| H2 report cannot silently promote named downstream opportunities | exact required opportunity ids + false generalization flags | `H2InputKeepsUntestedDownstreamOpportunitiesOutOfAcceptedCapabilities` |

## Trust boundary / proof budget

Trusted rather than recursively re-proved:

- accepted H0 world-state behavior outside the generic surfaces actually consumed by the projector;
- substantive truth of accepted CITY and PA sources;
- DW-04's accepted frozen experiment result;
- .NET/xUnit/Git hashing implementations and repository CI substrate.

The proof attacks the material causal boundaries owned by DW-05: neutral representability, generic provenance/rebuild, reached kernel/domain leakage, silent H0 mutation, residual completeness/classification and H2 overclaim. It does not expand into arbitrary compiler/toolchain corruption or future unclaimed domains.

## Falsification disposition

No causal negative was weakened to obtain GREEN. In particular, the semantic-relation omission control is preserved as evidence that semantic necessity remains consumer-owned; DW-05 does not retrofit domain semantics into the generic validator.
