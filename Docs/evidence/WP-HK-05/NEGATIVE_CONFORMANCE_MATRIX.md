# WP-HK-05 negative-conformance matrix

These controls are bounded to HK05's claim: complete canonical world invariant diagnostics, explicit current/proposed validation, and validation before every accepted public canonical mutation route. They do not re-prove HK04 commit-authority completeness or arbitrary runtime infrastructure.

| Required defect class | Controlled defect / mutant | Oracle expected RED | Reverted/effective GREEN evidence |
|---|---|---|---|
| validation skipped on a public mutation path | feed an invalid proposal to every composed capability whose canonical `SideEffect == CanonicalMutation`; separately feed the comparison oracle a result without `world.change.invalid_candidate` + embedded validation | `MutationValidationIssues` returns `mutation-validation-skipped` | every actual canonical-mutation definition returns the same invalid-candidate diagnostic set as explicit validation |
| missing invariant registration | remove the first descriptor from a copied `ValidatorInventory` | `FindInventoryIssues` returns `missing-validator:<id>` | accepted inventory has exact set equality against the independent test-owned 17-ID universe and zero inventory issues |
| exception escaping as public expected error | injected callback throws `InvalidOperationException` | `EscapesException` returns true | malformed public proposed-validation request is rejected as structured `contract.invalid_request` or `world.change.invalid_request`; no exception escapes |
| nondeterministic diagnostic order | reverse an observed diagnostic-signature sequence | sequence oracle reports inequality | two semantically identical invalid candidates with reversed input object ordering produce identical ordered signatures |
| ambiguous path/context | inject diagnostic with empty resource, path and remediation context | `DiagnosticShapeIssues` reports ambiguity/empty-context issues | every effective multi-violation diagnostic has non-empty resource, JSON-pointer-like `$/...` path, invariant ID, machine code and remediation context |
| mutation accepts state rejected by explicit validation | feed comparison oracle a synthetic successful mutation result for an explicitly invalid proposal | `MutationValidationIssues` returns `mutation-accepted-invalid-state` | actual apply rejects with `world.change.invalid_candidate`, preserves revision/hash and embeds the same ordered validation diagnostics |
| validate/apply semantic disagreement | compare apply result against diagnostics from a different invalid candidate | `MutationValidationIssues` returns `validate-apply-disagreement` | explicit proposed validation and real apply produce identical diagnostic signatures |
| handler accidentally carries canonical commit authority | original HK05 validation bindings held the authoritative session directly | HK04 `CapabilityRoute` authority guard rejected route construction in Actions run `35460353246` | validation handlers now receive the already accepted `WorldMutationPlannerView` attenuation facade; HK04 apply remains the only route receiving internal committer authority |

## Rejection-site classification audit

The complete HK05 diff was searched for newly introduced public failure/throw boundaries. Classification is:

- request-shape rejection before handler dispatch: canonical HK01 schema layer → structured `contract.invalid_request` (inherited contract guarantee);
- valid-schema but semantically malformed mutation envelope: inherited HK04 parser → structured `world.change.invalid_request`;
- stale revision/hash, exhausted revision, missing delete target, no-effect, planner coverage disagreement, idempotency conflict and concurrent update: inherited HK04 structured error sites, consumed rather than re-proved;
- invalid complete proposed world: HK05 → `world.change.invalid_candidate` with full `WorldValidationResult` in context;
- post-validation materialization disagreement: HK05 fail-closed invariant → `world.validation.internal_error` rather than an escaping expected-error exception;
- unbound authoring service: inherited unavailable-service structured error `world.state_unavailable`;
- argument-null programmer misuse inside in-process APIs remains a programming-contract exception and is outside the public expected-error claim.

No newly introduced HK05 expected rejection site is unclassified. No runtime exception is used as the public contract for an expected invalid authoring request.

## Result

All required causal defect classes have an executable oracle that detects the injected false behavior, and the effective candidate path is GREEN after each defect is absent. No extra case-specific production guard was added solely for the test mutants.
