# WP-HK-04 foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: Docs/evidence/WP-HK-04/RESIDUAL_RISK.md
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Central claim

For the accepted finite HK02 canonical world model, HK04 provides one engine-neutral mutation pipeline in which `authoring.change.plan@1.0`, `authoring.change.dry-run@1.0` and `authoring.change.apply@1.0` share deterministic candidate construction/validation; only apply may replace authoritative state, and then only atomically against the explicit expected revision/hash with idempotency protection. Planning authority is attenuated from commit authority, and effective canonical mutation definitions, transactional handlers, structurally authority-bearing handlers and dispatcher bindings are mechanically reconciled.

Pre-freeze implementation observation:

- observed repair SHA `7da26b0e45847aba5f63d0a079001ce550460c4d`
- GitHub Actions run `35443591856`
- Release build: 0 warnings / 0 errors
- HK04 transactional positives: 9/9 GREEN
- HK04 causal controls: 7/7 GREEN (six required classes; hidden-bypass has real-authority and declaration-mismatch controls)
- full regression: 86/86 GREEN
- exact observation receipt: `Result: GREEN`

The eventual frozen candidate is re-executed by `scripts/hk04-verify-exact-sha.sh`; exact-SHA frozen verification is PR metadata and cannot be embedded into the same immutable candidate without changing its SHA.

## Proof obligations

| Proof obligation | Claim/trust-boundary scope | Completeness argument | Positive evidence | Negative control / attack | Result | Residual risk |
|---|---|---|---|---|---|---|
| deterministic proposed change set exists before commit | three HK04 routes over finite HK02 state | one `BuildPlan` constructs complete candidate, independent semantic diff produces resources/fields/reference edges, canonical hash + plan id bind result | plan/dry-run/apply plan-id/hash equality; affected-resource/reference positive | omitted-effect mutant is rejected by `WorldMutationCoverage` oracle | PASS | conditional operation grammar partly semantic because accepted schema vocabulary has no discriminated union |
| dry-run uses same semantic path and persists nothing | `plan`/`dry-run` versus `apply` before final commit section | all phases call the same parser + `BuildPlan`; only `Apply` contains `_current = plan.CandidateState` | unchanged pre/post dry-run canonical hash; real apply equals predicted result hash | `DryRunDivergenceMutantIsDetectedByPredictedCanonicalHash` | PASS | none inside current session boundary |
| apply is all-or-nothing | one in-process authoritative session | every operation is applied to private working collections; complete `WorldState` validation occurs before the one locked state replacement | successful micro-world create/modify and extension transaction positives | later-invalid operation after an earlier effective operation leaves hash/inventory unchanged | PASS | distributed/durable commit is outside HK04 |
| stale writers fail rather than overwrite | explicit revision + canonical HK02 content hash | planning validates anchor; commit rechecks captured base revision/hash under lock immediately before replacement | sequential stale writer rejection + actual concurrent two-writer test, exactly one commit | stale-overwrite causal control | PASS | future multi-process substrate must preserve equivalent CAS semantics |
| accepted retries are idempotent | authoritative session lifetime | receipt keyed by caller idempotency key stores request fingerprint + accepted plan; receipt check precedes stale-anchor rejection and is repeated in commit lock | exact retry replays same plan without revision advance; key reuse for different semantics conflicts | duplicate-retry causal control | PASS | receipts share in-memory lifetime with current state; durable bridges must couple durable receipt/state |
| change sets identify every effective resource/field/reference | accepted HK02 objects/extensions | independent `WorldMutationCoverage` diffs base/candidate semantics rather than trusting operation declarations; planner refuses mismatch | object field/reference positive; extension create/update/remove positive | empty declaration over real `typeId` change turns oracle red | PASS | future HK02 semantic fields must extend oracle and turn proof red until mapped |
| conditions are machine-readable | successful plans | response schema exposes arrays of `{code,path,satisfied}`; current revision/hash and candidate-state/change-coverage invariants are emitted | positive assertions require all returned conditions satisfied | stale/invalid candidate paths fail structurally instead of emitting false success | PASS | richer per-command domain conditions belong to later gameplay authoring |
| public canonical mutation cannot bypass transactional handler surface | effective Runtime/attack-fixture assemblies, route publication and composed contract | public planning handlers receive only a sealed planner view; the apply handler alone receives the internal Authoring-owned commit capability. Independently of `SideEffect`, structural inspection enumerates handler types carrying that capability or the concrete authoritative session; route construction also evaluates the actual handler object graph and rejects authority without the transactional marker. Authority-bearing routes, declared mutations, transactional handlers and dispatcher keys must agree | production `MutationSurfaceConformance` + HK01 full route-universe conformance are GREEN; normal planner/apply routes publish successfully | isolated `engine.observe@1.0` remains declared `ReadOnly`, directly calls the real session `Apply`, demonstrably changes canonical hash/revision when invoked outside publication, is rejected by `CapabilityRoute`, and appears as an `effective-write-authority` extra in the independent assembly oracle | PASS | trusted .NET type/object reflection and the current finite Authoring write-capability types; arbitrary private-reflection/toolchain subversion is outside the foundational trusted boundary |
| mutation inventory remains one HK01 canonical truth | production composition/discovery | mutation definitions/routes are appended through `CanonicalWorldContract` and existing HK01 composer; authority inspection adds no definitions, schemas or dispatch registry | discovery + dispatcher + route-universe conformance; HK01 count regression updated 7→10 base routes | real hidden mutator plus the retained declaration/marker mismatch and existing HK01 route oracle | PASS | unavailable runtime binding returns structured state-unavailable rather than shrinking discovery |
| finite micro-world operation universe is exercised | four generic operation kinds only | grammar contains exactly put/remove object and put/remove extension; no gameplay-specific commands | object create/update/remove paths plus extension put/remove transaction | final whole-state validator rejects dangling/cyclic results atomically | PASS | extension payload byte budgets deferred to later guardrail WP |
| forbidden scope absent | baseline→candidate diff | product changes confined to Authoring neutral mutation semantics, Runtime bindings/conformance, tests, scripts and evidence | changed-file/pre-review audit | scope audit in Worker prereview | PASS | none |

## Independent/effective universes

HK04 uses four non-self-shrinking observables:

1. **Accepted state universe** — HK02 `WorldState` remains the semantic state boundary and its canonical content hash is the result oracle.
2. **Effective public route universe** — inherited HK01 `RouteUniverse` reflects production handlers independently of definition/discovery and now reconciles ten base routes.
3. **Effective change universe** — `WorldMutationCoverage` independently diffs base vs candidate object/extension semantics and compares that set with the plan's declared affected fields/references.
4. **Effective write-authority universe** — `MutationAuthorityInspector` derives authority from executable handler structure and actual publication-time object graphs carrying the internal committer or concrete authoritative session. It never reads capability `SideEffect` or transaction policy metadata. The isolated attack assembly keeps `engine.observe` declared `ReadOnly` while its handler really commits state, so contractual relabelling cannot remove the defect from this universe.

The transactional marker and write-authority inspector are enforcement/conformance facts rather than semantic registries; neither owns definitions, schemas or dispatch. Equality with canonical mutation definitions catches both declared mutations missing transaction handling and authority-bearing handlers misdeclared as read-only, while HK01 route conformance prevents undiscovered effective public handlers.

## Proof-budget reconciliation

HK04 adds no third-party dependency or generalized transaction/proof framework. Proof support is limited to one small independent semantic-diff oracle, one mutation-surface reconciliation oracle layered on the already accepted HK01 route universe, the six mandatory defect classes, one supplementary declaration-mismatch control, two additional positives for extension operations/concurrent writers, and one exact-SHA observe/verify wrapper following the established HK sequence.

Repair cycle 1 responds to the independent FAIL's concrete false-green class with a product boundary change, not a nominal seventh check: planning and commit authority are separated; commit authority becomes an internal friend capability; route publication fails closed on a non-transactional authority-bearing object graph; and conformance gains a metadata-independent authority universe. The isolated test assembly exists only to inject the exact real bypass. This closes an explicit HK04 acceptance gap, remains materially smaller than the transactional behavior protected, and is within proof budget.

## Reconciliation verdict

All HK04 acceptance obligations are PASS inside the stated finite trust boundary after repair cycle 1. The hidden-bypass control now performs a real authoritative mutation while its route remains declared `ReadOnly`, and two independent enforcement points turn red: publication rejects the handler and the assembly authority universe reports it. All other mandatory causal classes remain GREEN, all four operation kinds are exercised, the actual concurrent-writer control allows exactly one commit, and no known in-boundary false-green class remains undetected.
