# WP-HK-04 foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: Docs/evidence/WP-HK-04/RESIDUAL_RISK.md
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Central claim

For the accepted finite HK02 canonical world model, HK04 provides one engine-neutral transactional mutation pipeline. `authoring.change.plan@1.0`, `authoring.change.dry-run@1.0` and `authoring.change.apply@1.0` share deterministic candidate construction/validation; only the canonical apply handler receives internal commit authority, and accepted commit is atomic against expected revision/hash with idempotency protection. The authoritative session exposes no public commit method. Every current public route not classified as `CanonicalMutation` is effectively invoked against the same live session and must leave canonical revision/hash unchanged. Structural object-graph inspection is defence in depth only and is not used as a completeness argument.

Latest implementation observation before evidence reconciliation:

- candidate SHA `914ab13f9fcb28f6d62974bd22eaf6dae1ccb6fe`
- GitHub Actions run `35450750854`
- Release build: 0 warnings / 0 errors
- HK04 transactional positives: 9/9 GREEN
- HK04 causal controls: 8/8 GREEN
- full regression: 87/87 GREEN
- canonical receipt: `Result: GREEN`

The final evidence/freeze commit changes documentation only. Canonical observation and frozen exact-SHA verification must rerun on the final HEAD.

## Proof obligations

| Proof obligation | Claim/trust-boundary scope | Completeness argument | Positive/evaluated evidence | Causal negative control | Result | Residual risk |
|---|---|---|---|---|---|---|
| deterministic proposed change set exists before commit | three HK04 routes over finite HK02 state | one `BuildPlan` constructs the complete candidate; independent semantic diff binds affected resources/fields/references; canonical hash + plan id bind result | plan/dry-run/apply plan-id/hash equality | omitted-effect mutant is rejected by `WorldMutationCoverage` | PASS | schema grammar remains partly semantic because accepted HK01 vocabulary has no discriminated union |
| dry-run uses same semantic path and persists nothing | plan/dry-run/apply | all phases use the same parser + `BuildPlan`; only internal apply reaches the final state replacement | predicted dry-run hash equals real accepted apply; non-mutation oracle invokes both plan and dry-run and observes unchanged state | `DryRunDivergenceMutantIsDetectedByPredictedCanonicalHash` | PASS | none inside current session boundary |
| apply is all-or-nothing | one in-process authoritative session | operations modify private working collections; a complete `WorldState` is constructed/validated before one locked reference replacement | create/update/remove/extension positives | later-invalid operation after an earlier effective operation leaves hash/inventory unchanged | PASS | durable/distributed commit outside HK04 |
| stale writers fail instead of overwriting | expected revision + canonical hash | anchor checked during planning and rechecked under commit lock immediately before replacement | sequential stale rejection + real two-writer race, exactly one commit | stale-overwrite control | PASS | future multi-process substrate must preserve equivalent CAS semantics |
| accepted retries are idempotent | authoritative session lifetime | receipt keyed by caller idempotency key stores request fingerprint + accepted plan; checks occur before planning and again under lock | exact retry replays without revision advance; different semantics under same key conflict | duplicate-retry control | PASS | receipts intentionally share in-memory state lifetime |
| change set covers every effective canonical change | current HK02 semantic state | `WorldMutationCoverage` diffs base/candidate independently of requested operation declarations | object/reference/extension positives | real type change with empty declaration turns oracle red | PASS | future HK02 semantic fields must extend coverage oracle |
| conditions are machine-readable | successful plans | result schema exposes `{code,path,satisfied}` arrays plus base/result revision/hash | positive assertions require satisfied conditions | stale/invalid candidates return structured failure instead of false success | PASS | richer gameplay conditions belong later |
| public command cannot hide canonical write outside mutation pipeline | current effective public route universe + public session API | session has no public `Apply`; external/scoped code can only plan/dry-run unless it crosses the internal trusted production binding. Independently, every current non-`CanonicalMutation` definition has a valid evaluated request vector; the vector set must equal that route set and each successful invocation must preserve live revision/hash | `EveryEffectiveNonMutationRouteIsEvaluatedAndCannotChangeCanonicalState`; 9 current non-mutation routes all execute successfully and preserve state | exact cycle-2 `List<TransactionalWorldAuthoringSession>` fixture remains ReadOnly and uses only public API; if public `Apply` reappears it invokes it and the real hash/revision assertions turn RED | PASS | hostile private reflection/toolchain subversion outside standard trust boundary |
| mutation dispatcher == discovered mutation == transactional surface | current composed canonical inventory and effective Runtime handlers | canonical mutation `SideEffect`, `CanonicalTransaction` policy, effective `ITransactionalMutationHandler` route identities and dispatcher bindings are mechanically compared; HK01 independently enumerates all public routes | `MutationSurfaceConformance` + HK01 route conformance | declaration/marker mismatch `HiddenMutationBypassMutantBreaksMutationSurfaceEquality` remains supplementary consistency control | PASS | none inside current effective surface |
| finite generic operation universe exercised | four operation kinds | grammar is exactly put/remove object + put/remove extension; no gameplay-specific commands | object and extension create/update/remove positives | final whole-state validator rejects invalid graph atomically | PASS | payload/resource budgets deferred to later guardrails |
| forbidden scope absent | baseline→candidate diff | product changes remain Authoring/Runtime/tests/scripts/evidence only | Worker baseline audit | Worker pre-review scope audit | PASS | none |

## Independent/effective universes

HK04 uses four complementary observables:

1. **Accepted state universe** — inherited HK02 `WorldState` and canonical content hash are the semantic state/result oracle.
2. **Effective public route universe** — inherited HK01 `RouteUniverse` independently reflects production public handlers and reconciles them with definition/dispatcher/discovery.
3. **Effective change universe** — `WorldMutationCoverage` independently diffs base versus candidate semantics and compares those effects with the plan.
4. **Effective non-mutation behaviour universe** — every current composed definition whose `SideEffect != CanonicalMutation` must have exactly one successful request vector and is invoked against a live authoritative session; revision/hash must be unchanged after every invocation. Missing/new non-mutation routes make the vector-set equality fail closed.

`MutationAuthorityInspector` is no longer an independent/effective universe. It is a narrow defence-in-depth publication guard for accidental direct leakage of internal `ICanonicalWorldMutationCommitter`. No proof claim depends on recursively walking arbitrary container/object shapes.

## Circuit-breaker reconciliation

- FAIL #1 (`28695c9d…`) proved the original hidden-mutation control circular because membership came from the `SideEffect` metadata under test.
- Repair cycle 1 introduced a structural authority walker, but FAIL #2 (`dc81b054…`) proved the walker could miss ordinary BCL/foreign-assembly indirection such as `List<TransactionalWorldAuthoringSession>` while public `Apply` still granted real write authority.
- Cycle 2 therefore changed the boundary instead of adding traversal cases: `TransactionalWorldAuthoringSession.Apply` is non-public and commit is an internal interface capability; structural traversal is demoted to defence in depth; the exact `List<>` shape remains executable; and an effective oracle now executes the whole current non-mutation public surface.

This removes the causal need to classify wrapper syntax. A public/scoped handler may possess or wrap the session without obtaining public commit power, while any current non-mutation production handler that nevertheless changes canonical state turns the evaluated oracle red.

## Proof-budget reconciliation

Cycle 2 simplifies rather than expands the failed proof strategy: the completeness claim drops recursive structural authority enumeration and replaces it with one finite evaluated behaviour check over the already accepted HK01 route universe. No third-party dependency or generalized proof framework is added. Existing atomicity/CAS/idempotency/change-coverage machinery was left intact because both independent reviews found it sound.

The retained support maps directly to explicit acceptance claims and the two observed false-green classes. `PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.

## Reconciliation verdict

All HK04 acceptance obligations are supported inside the stated finite trust boundary. No public session commit primitive remains; the exact ordinary-indirection class from Reviewer cycle 2 is retained without special-casing its container; every current non-mutation public route is effectively executed and state-neutral; mutation metadata/policy/transactional handlers/dispatcher remain mechanically equal; and no known in-boundary false-green defect class remains undetected.
