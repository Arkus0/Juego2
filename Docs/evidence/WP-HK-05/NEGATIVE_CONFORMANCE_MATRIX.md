# WP-HK-05 negative-conformance matrix

These controls are bounded to HK05's claim: complete canonical world invariant diagnostics, explicit current/proposed validation, and validation before every accepted public canonical mutation route. They do not re-prove HK04 commit-authority completeness or arbitrary runtime infrastructure.

Reviewer repair cycles 1 and 2 expose one causal class: **aggregate validation under ambiguous identity**. The policy is deterministic, dependency-local deferral: duplicate object/extension identity is reported at an index-addressable source entry, while a secondary diagnostic is deferred only when its own source or the specific graph traversal that produced it requires choosing an ambiguous identity representative. Unrelated uniquely identified components remain independently reportable in the same validation pass.

| Required defect class | Controlled defect / mutant | Oracle expected RED | Reverted/effective GREEN evidence |
|---|---|---|---|
| validation skipped on a public mutation path | feed an invalid proposal to every composed capability whose canonical `SideEffect == CanonicalMutation`; separately feed the comparison oracle a result without `world.change.invalid_candidate` + embedded validation | `MutationValidationIssues` returns `mutation-validation-skipped` | every actual canonical-mutation definition returns the same invalid-candidate diagnostic set as explicit validation |
| missing invariant registration | remove the first descriptor from a copied `ValidatorInventory` | `FindInventoryIssues` returns `missing-validator:<id>` | accepted inventory has exact set equality against the independent test-owned 17-ID universe and zero inventory issues |
| exception escaping as public expected error | injected callback throws `InvalidOperationException` | `EscapesException` returns true | malformed public proposed-validation request is rejected as structured `contract.invalid_request` or `world.change.invalid_request`; no exception escapes |
| nondeterministic diagnostic order under ambiguous identity | two `node.a` entries carry different secondary container defects; reverse only the duplicate entries while `node.b -> node.a` makes first-wins traversal observably divergent | full semantic diagnostic sequence (count, severity, invariant ID, machine code, resource/path, message and sorted remediation context) differs under the old behavior | duplicate identity remains one stable index-addressable diagnostic; only ambiguity-dependent secondaries/traversals are deferred; reversed candidates are semantically identical |
| over-broad ambiguity deferral hides independent violations | coexist two divergent `dup.x` entries with a disjoint unique-ID `cycle.a <-> cycle.b`; reverse only the duplicate entries | repair-cycle-1 global cycle suppression returns no `ContainmentAcyclic` diagnostics, violating aggregate multi-violation semantics | both unique-cycle diagnostics remain visible and identical under duplicate-order inversion while duplicate-sourced secondaries remain deferred |
| ambiguous path/context | inject a duplicate-ID diagnostic with non-empty identity-based resource/path and non-empty generic context but no source indices | strengthened `ActionabilityIssues` returns `duplicate-context-missing-source-indices` and `duplicate-location-not-index-addressable` | duplicate object diagnostic identifies `world.object@index:<n>` + `$/objects/<n>/id`, carries `firstIndex`, `duplicateIndex`, and `duplicateId` |
| duplicate extension identity with divergent secondaries | two extensions share the same owner/schema/subject identity but have different dangling dependencies; reverse their order | identity-based secondary locations/context would differ while naming the same ambiguous extension resource | one index-addressable duplicate-extension diagnostic remains; dependent extension diagnostics are deferred; full semantic result is order-invariant and carries `duplicateIdentity` + source indices |
| mutation accepts state rejected by explicit validation | feed comparison oracle a synthetic successful mutation result for an explicitly invalid proposal | `MutationValidationIssues` returns `mutation-accepted-invalid-state` | actual apply rejects with `world.change.invalid_candidate`, preserves revision/hash and embeds the same ordered validation diagnostics |
| validate/apply semantic disagreement | compare apply result against diagnostics from a different invalid candidate | `MutationValidationIssues` returns `validate-apply-disagreement` | explicit proposed validation and real apply produce identical diagnostic signatures |
| handler accidentally carries canonical commit authority | original HK05 validation bindings held the authoritative session directly | HK04 `CapabilityRoute` authority guard rejected route construction in Actions run `35460353246` | validation handlers now receive the already accepted `WorldMutationPlannerView` attenuation facade; HK04 apply remains the only route receiving internal committer authority |

## Ambiguous-identity policy boundary

The policy intentionally distinguishes three cases:

- **identity defect itself:** always emitted and made directly repairable through stable collection indices plus the duplicated semantic identity;
- **secondary defect whose source identity is ambiguous:** deferred until identity is unique, because emitting it would name an entry the client cannot uniquely select;
- **graph-derived defect from a unique source:** deferred only when that specific dependency traversal reaches an ambiguous identity. A graph violation whose complete dependency chain is uniquely identified remains reportable even when an unrelated duplicate exists elsewhere.

For containment, HK05 therefore does not treat `HasAnyDuplicateObjectId` as a world-wide suppression switch. It replays the reported source's containment chain only far enough to determine whether exposing that raw cycle result would require choosing a duplicate representative. The raw HK02 invariant evaluator is left intact as an inherited predecessor component; the HK05 aggregate layer owns public diagnostic safety/determinism. Presence-style checks that do not choose representatives remain unaffected. The extension equivalent remains source-local and does not require a second mechanism.

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

All required causal defect classes have an executable oracle that detects the injected false behavior. The ambiguity regression now proves both sides of the same semantic boundary: an ambiguity-dependent containment result is deferred, while a disjoint uniquely identified cycle remains aggregated. The actionability oracle still rejects merely non-empty duplicate diagnostics without concrete source indices. No unrelated defense or predecessor re-proof was added.
