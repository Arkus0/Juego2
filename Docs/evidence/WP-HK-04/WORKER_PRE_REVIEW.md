# WP-HK-04 Worker pre-review

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 3
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-04

Pre-review input HEAD: `388d85b7f6ae45203b3d89aaa86a0d9886c5a31f`  
Baseline: `ecebd054821eeb388c3cf6ee4e389545d762b6d8`  
Latest canonical implementation observation before evidence commits: `1922fcb7e43729326b9860fb27b13e90d8a2b552`, Actions `35439895690` — 0 warnings / 0 errors, 9/9 HK04 transactional positives, 6/6 mandatory causal controls, 85/85 full regression, receipt `Result: GREEN`.

This report is the final Worker-authored repository evidence file. Its own commit changes documentation only. Canonical observation and frozen exact-SHA verification must rerun on the final HEAD before independent Reviewer handoff.

## Contract and predecessor re-check

Re-read `WP-HK-04` and the foundational proof obligations. `main` remains exactly `ecebd054821eeb388c3cf6ee4e389545d762b6d8`; the accepted predecessor did not advance during this Worker cycle.

`WORKER_PLAN.md` remains aligned with the dependency boundary:

- HK03 reviewed/frozen candidate `8c20a380003c082fa9bd472d3233afa9654fb231` independently PASSed on PR #18;
- HK03 implementation merge `d8b808450ee7863726d718a25c5756534113fcfd`, then DocSync/handoff baseline `ecebd054821eeb388c3cf6ee4e389545d762b6d8`;
- HK04 consumes the accepted HK02 canonical state/hash and HK01/HK03 canonical composition/read guarantees rather than re-proving them;
- no concrete evidence was found that reopens an inherited guarantee.

## Complete baseline-to-candidate audit

The full baseline diff was inspected. Product changes are limited to:

- `Arkus.Game.Authoring`: public engine-neutral mutation service boundary, canonical HK04 capability definitions, one sealed transactional authoring session and independent change-coverage oracle;
- `Arkus.Harness.Runtime`: three canonical handler bindings, one transactional-handler marker/conformance oracle, and extension of the existing canonical world composition to include the authoring namespace;
- tests: HK04 positive/concurrency/extension coverage plus six causal self-attacks; existing HK01 route-universe count reconciled with the expanded canonical base inventory; HK03 read assertion scoped explicitly to the accepted inspection command set;
- scripts: HK04 exact-SHA observation/verification plus routing through the existing canonical wrappers;
- HK04 evidence.

No `.csproj` dependency changed. Runtime still receives world semantics through Authoring and does not gain a direct `Arkus.Game.World` reference. No Unity/engine write, arbitrary filesystem edit, gameplay-specific authoring command, undo UI, transport host or natural-language mutation evaluator appears in the candidate.

## Adversarial falsification checks

### Partial state exposure / mutable alias

Attempt: make an early operation visible before a later operation fails, or mutate accepted state through an alias.

Result: the session captures an immutable accepted `WorldState`, applies operations only to private working dictionaries, constructs and validates a complete new `WorldState`, and has one private authoritative replacement `_current = plan.CandidateState` inside the successful apply critical section. `Current` is read-only and HK02 object/reference state is immutable; opaque extension bytes are exposed only by copy. The retained later-failure self-attack proves the canonical hash and object inventory remain unchanged.

### Dry-run/apply fork

Attempt: give dry-run a separate semantic implementation that can disagree with commit.

Result: plan, dry-run and new-request apply all call the same parser and `BuildPlan`. Apply adds only receipt checks and the final revision/hash compare-and-swap. Dry-run's predicted canonical result hash equals the state actually committed from the same base; an intentionally divergent candidate turns the oracle red.

### Concurrency / lost update

Attempt: let two writers accepted from one anchor both commit.

Result: revision/hash is checked while planning and rechecked under the same commit lock against the captured base immediately before replacement. The actual parallel two-writer positive allows exactly one success; the loser is stale at planning or concurrent at commit. The stale-overwrite causal control separately proves accepted state cannot be replaced by an old anchor.

### Idempotency race / duplicate apply

Attempt: replay an accepted request after revision advances, race two identical keys, or reuse a key for different semantics.

Result: accepted receipts are checked before stale-anchor validation and again inside the commit lock. Exact request fingerprint replay returns the stored plan without another revision advance; different semantics under the same key fail with `world.change.idempotency_conflict`. Receipt lifetime is explicitly bounded to the same in-memory lifetime as authoritative state in `RESIDUAL_RISK.md` rather than being misrepresented as durable replay protection.

### Plan omits real effects

Attempt: derive the change set only from requested operations so cancelling/implicit/final-state effects can be omitted.

Result: `WorldMutationCoverage` independently diffs accepted base versus validated candidate object/extension semantics, including existence, type, container, reference-edge and opaque-payload effects, then compares that universe to the returned plan. A real type change with an empty declared change list turns the oracle red. Net-zero operation sequences are rejected as `world.change.no_effect`.

### Hidden mutation route / second registry

Attempt: add an effective public canonical mutation without the transactional handler boundary, or create a parallel mutation registry.

Result: mutation definitions remain in the existing HK01 canonical contribution/composer. `MutationSurfaceConformance` independently selects `CanonicalMutation` definitions, reflects effective `ITransactionalMutationHandler` routes, and requires equality plus dispatcher presence. Existing HK01 `RouteUniverse` still independently reconciles all effective production handlers against canonical definitions/discovery. The marker contains no definitions or dispatch logic and is not a semantic registry. The hidden-bypass mutant turns the equality oracle red.

### Discovery/schema boundary

Attempt: hide operational rules in implementation-only types or accidentally redefine the accepted HK03 `world.*` read namespace.

Result: public mutation identity is `authoring.change.*@1.0`, provider namespace `authoring`, within the same base inventory. Discovery exposes request/success/error schemas, operation-kind enum, affected-resource/reference schema, condition schema, optimistic/idempotency/batching/policy metadata. The accepted canonical schema subset lacks discriminated unions and numeric min/max facets, so operation-specific combinations, stable-token syntax, Base64 canonicality and 1..64 count are deterministic semantic validation with structured failures; the 64 maximum is also advertised in canonical batching metadata. Reopening HK01 solely to add `oneOf`/min-max was rejected as disproportionate and recorded as bounded residual risk.

## Material findings fixed before freeze

1. **HK04 absent from canonical observation/verification routing** — the first CI attempt failed before compilation because the established Arkus wrapper knew only HK00..HK03. Fixed by adding dedicated HK04 observe/verify entrypoints and both router cases, without weakening exact-SHA or cleanliness checks.
2. **Initial mutation identity used the inherited `world.*` namespace** — corrected to `authoring.change.*` so HK03's accepted `world.*` surface remains purely inspection and authoring has a distinct semantic namespace inside the same HK01 inventory.
3. **Initial positive suite emphasized object operations and sequential stale writes** — expanded before freeze to exercise put/remove opaque extensions and a real concurrent two-writer race, closing the finite operation-universe/concurrency evidence gap without adding new product machinery.

No fourth material in-claim blocker was found in the final pass.

## Residual-risk / proof-budget classification

Non-blocking residuals are explicit: session-lifetime rather than durable idempotency, process-local rather than distributed CAS, the current canonical schema vocabulary's lack of discriminated-union/min-max facets, and payload/performance budgets beyond the 64-operation cap. None falsifies the current in-memory engine-neutral HK04 claim.

Proof support remains proportional: one semantic-diff coverage oracle, one small mutation-surface reconciliation oracle reusing the accepted HK01 route universe, exactly the six mandatory causal classes, two narrowly targeted positive additions, and one established-style exact-SHA wrapper pair. No third-party framework or general transaction abstraction was introduced.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET` remains justified.

## Handoff readiness

No known blocker remains. Freeze is permitted only after the post-report canonical observation is GREEN on this report commit, PR HEAD is read as an exact 40-character SHA, no Worker commits occur after that SHA is recorded, PR metadata is updated to the same frozen SHA and `FROZEN_FOR_REVIEW`, the PR is marked ready, and the ready-triggered exact-SHA verification is GREEN for that same SHA. This Worker does not issue the independent PASS/FAIL verdict.
