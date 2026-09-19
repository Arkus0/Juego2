# WP-HK-05 foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: Docs/evidence/WP-HK-05/RESIDUAL_RISK.md
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Central claim

Inside the accepted finite engine-neutral `WorldState` model, every currently owned canonical world invariant is independently enumerable and registered exactly once; effective validation aggregates stable deterministic repairable diagnostics; current and proposed state validation are discoverable/versioned public capabilities; and every accepted public canonical mutation route rejects a proposal that explicit validation rejects before canonical state can change. Expected invalid public requests/states are represented as structured results rather than runtime exceptions.

HK05 consumes, rather than re-proves, the accepted HK01 public-route universe and HK04 closed commit/transaction boundary.

## Proof obligations

| Proof obligation | Claim/trust-boundary scope | Completeness argument | Positive/effective evidence | Causal negative control | Result | Residual risk |
|---|---|---|---|---|---|---|
| every owned micro-world invariant covered | current canonical `WorldStateCandidate` invariant universe | test-owned finite 17-ID universe is independent of both catalog and validator registry; all three sets must be exactly equal | one effective invalid fixture triggers each invariant; `evaluatedInvariantCount` uses accepted registry | remove one copied descriptor → `missing-validator:<id>` | PASS | future invariant growth requires proof update |
| structural, identity and reference validity aggregate rather than throw-first | all 17 current invariant evaluators | evaluator registry runs every descriptor and collects violations before deterministic sort | multi-invalid town candidate yields several independent diagnostics | null/duplicate/dangling/cycle/extension fixtures independently exercise each class | PASS | programmer misuse outside candidate/public dispatcher is not an expected-error claim |
| stable repairable diagnostic shape | effective validator output | every emitted violation maps through one `ToDiagnostic` boundary carrying machine code, severity, resource, path, invariant identity, message, remediation context | diagnostic-shape oracle on multi-violation result | injected empty resource/path/context diagnostic is rejected by oracle | PASS | natural-language repair generation is forbidden scope |
| deterministic diagnostic ordering | effective result, independent of caller collection order | final sort is over stable semantic fields; equivalent candidates with reversed object order must serialize diagnostic signatures identically | `InvalidTownCandidate(false/true)` signature equality | reversed-signature mutant fails ordering oracle | PASS | no cross-version ordering compatibility beyond v1 result contract claimed |
| current/proposed validation is explicit, read-only, discoverable and versioned | accepted HK01 composed H0 contract | canonical definitions/routes expose exactly the two validation capabilities with public schema documents; state hash/revision observed before/after | current valid result, proposed invalid result, equal schema fingerprint, required fields | malformed request follows structured schema/parser rejection rather than exception | PASS | unavailable service remains a structured inherited runtime condition |
| validation runs before every public canonical commit | all accepted composed definitions with `SideEffectClass.CanonicalMutation` | route universe is inherited from accepted HK01; HK05 enumerates every canonical-mutation definition from the composed contract and applies one explicit-invalid proposal to each | actual apply returns `world.change.invalid_candidate`, embeds same diagnostics, revision/hash unchanged | synthetic accepted-invalid result → `mutation-accepted-invalid-state`; missing embedded validation → `mutation-validation-skipped` | PASS | concrete evidence of an HK01/HK04 omitted route would reopen predecessor boundary |
| explicit validation and apply agree | proposed validation vs canonical apply for identical parsed request/snapshot | both execute `BuildCandidate` / `WorldValidationEngine.ValidateCandidate`; comparison oracle checks ordered diagnostic signatures | same invalid mutation gives identical explicit/apply validation set | different invalid candidate fed to comparison oracle → `validate-apply-disagreement` | PASS | concurrent update after planning remains inherited HK04 concern |
| runtime exceptions are not expected public invalid-request contract | dispatcher-visible malformed/invalid inputs owned by accepted schema/parser/validator layers | public dispatch schema validation and authoring parser return `CapabilityInvocationResult`; candidate disagreement maps to structured internal validation error | malformed proposed request produces `contract.invalid_request` or `world.change.invalid_request`; invalid state returns validation result | injected throwing callback is detected by `EscapesException` | PASS | null/programmer misuse of direct CLR APIs is outside public dispatcher contract |
| new validation routes do not leak commit authority | validation handler object graph under HK04 accepted guard | validation is routed through the already accepted `WorldMutationPlannerView` attenuation facade; only apply receives internal committer | canonical composition now succeeds and conformance/regression covers routes | original direct-session validation binding was rejected by HK04 guard in Actions `35460353246` | PASS | no new authority-inspector exception or traversal was added |
| representative Juego2 shape fits | bounded approved Potes/Liébana plaza + bar/shop + NPC slice | one content-shaped test exercises construct → inspect → validate-current → validate-proposed → failed apply without new gameplay schema | `Hk05ContentShapeProbeTests` and `CONTENT_SHAPE_PROBE.md` | removing referenced bar yields object + extension diagnostics and no commit | PASS | gameplay/Unity/content budgets are named future scope |
| rejection sites classified | newly introduced HK05 public control-flow plus inherited expected-error sites used by it | complete HK05 diff audit classifies schema rejection, inherited parser/mutation failures, invalid candidate, materialization disagreement and unavailable service | `NEGATIVE_CONFORMANCE_MATRIX.md` | escaped-exception and skipped-validation controls would expose unstructured new expected-error behavior | PASS | inherited HK04 error taxonomy is consumed, not duplicated |
| forbidden scope absent | baseline-to-candidate diff | changes confined to engine-neutral world validation, authoring/runtime bindings, tests, exact-SHA scripts and evidence | complete Worker diff audit | no Unity, NL repair generation, gameplay invariants, journal/replay or engine-specific validation added | PASS | none |

## Independent/effective universes

1. **Invariant universe** — the test-owned `ExpectedOwnedInvariantIds` list is independent of `WorldInvariantCatalog.All` and `WorldValidationEngine.ValidatorInventory`; exact three-way set equality prevents either production list from silently shrinking the proof obligation.
2. **Effective invariant behavior** — `InvariantFixtures()` causes every expected invariant ID to appear in real validator output, so registration alone is insufficient.
3. **Public mutation universe** — accepted HK01 composition owns route completeness. HK05 consumes the effective composed definitions and checks every definition classified `CanonicalMutation`, rather than maintaining a second mutation route list.
4. **Validation/apply semantic oracle** — explicit proposed validation and apply are independently invoked through public dispatch and their effective diagnostic signatures are compared.
5. **Representative content shape** — the Potes probe is deliberately representative only; it is not used to prove invariant completeness.

## Rejection-site ownership split

HK05 newly owns invalid complete-candidate diagnostics and the materialization-disagreement fail-closed mapping. HK01 owns schema rejection and HK04 owns mutation-envelope/concurrency/idempotency/planner structured errors. Those predecessor guarantees are consumed; the HK05 audit verifies that its new paths neither introduce an unclassified expected public rejection nor convert inherited structured failures into escaping exceptions.

## Proof-budget verdict

The proof uses one focused HK05 test class, one bounded content-shape test and existing contract/authority surfaces. The only architectural defect found during Worker execution — read-only validation handlers directly carrying the authoritative session — was fixed by reusing HK04's existing attenuation facade. No new authority exception, registry framework, external validator package or duplicate predecessor proof system was added.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.

The evidence files are documentation on the same candidate branch; the exact documentation-reconciled SHA still requires canonical observation and exact-SHA freeze validation before handoff.
