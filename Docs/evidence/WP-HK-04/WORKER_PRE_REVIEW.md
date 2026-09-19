# WP-HK-04 Worker pre-review — repair cycle 2

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 2
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-04

Pre-review implementation HEAD: `914ab13f9fcb28f6d62974bd22eaf6dae1ccb6fe`

Baseline: `ecebd054821eeb388c3cf6ee4e389545d762b6d8`  
Latest canonical implementation observation: Actions run `35450750854` on exact SHA `914ab13f9fcb28f6d62974bd22eaf6dae1ccb6fe` — Release build 0 warnings / 0 errors, 9/9 transactional positives, 8/8 HK04 causal controls, 87/87 full regression, canonical receipt `Result: GREEN`.

This evidence reconciliation commit is documentation-only. Canonical observation must rerun on its resulting HEAD before freeze; frozen exact-SHA verification then reruns after Ready.

## Contract / predecessor re-check

Re-read `WP-HK-04`, `FOUNDATIONAL_PROOF_STANDARD.md`, `WORKER_REVIEW_PROTOCOL.md`, `PRODUCT_ARCHITECTURE.md`, and the accepted HK03 contract/evidence.

- HK03 reviewed candidate `8c20a380003c082fa9bd472d3233afa9654fb231` independently PASSed on PR #18 and merged as `d8b808450ee7863726d718a25c5756534113fcfd`.
- HK04 continues to consume HK01 canonical composition/independent route-universe, HK02 canonical state/hash and HK03 bounded deterministic read guarantees rather than re-proving them.
- `main` advanced from the original dependency-valid baseline only by a PROCESS_ONLY context/terminology commit (`1f1805559c269b19c99a4b80eee638314024f601`); no inherited semantic guarantee changed.
- Neither Reviewer FAIL provided concrete evidence that HK01/HK02/HK03 is false or inapplicable.

## Circuit-breaker audit

Two independent FAILs hit the same hidden-mutation proof class, so cycle 2 reaudited the architecture before implementation rather than adding another traversal rule.

FAIL #1 (`28695c9d…`) showed the first control was circular: mutation membership came from the `SideEffect` metadata being challenged. FAIL #2 (`dc81b054…`) showed the replacement `MutationAuthorityInspector` could miss the same real public session write authority behind ordinary `List<>`/array/foreign-assembly indirection.

The repair changes the causal boundary:

1. `TransactionalWorldAuthoringSession` implements internal `ICanonicalWorldMutationCommitter`; `Apply` is non-public. Public API exposes `Current`, `Plan` and `DryRun`, not commit.
2. `CanonicalWorldMutationAuthority.Bind` no longer wraps a public session method; Runtime receives the existing internal capability only when the Authoring service actually implements it.
3. `MutationSurfaceConformance` no longer claims a structural write-authority universe. It mechanically compares canonical mutation definitions, `CanonicalTransaction` policy, effective `ITransactionalMutationHandler` routes and dispatcher bindings.
4. `MutationAuthorityInspector` remains only defence in depth for direct internal-committer leakage. Its walker is not a completeness proof and was not expanded for collections/arrays/wrappers.
5. The exact ordinary-indirection class is retained: an external ReadOnly handler stores the real session inside `List<TransactionalWorldAuthoringSession>`, uses only public API and is allowed to publish. Its fixture dynamically invokes a public `Apply` if such a regression exists; that would perform a real commit and make revision/hash assertions RED. In the repaired candidate it can only plan and state remains unchanged.
6. A second effective control invokes every current public non-`CanonicalMutation` route with a valid request against one live authoritative session and requires revision/hash to remain unchanged after each. The request-vector key set must exactly equal the current non-mutation definition set, so a new route cannot be silently omitted from the oracle.

This is a smaller and more semantic proof boundary than repair cycle 1: capability closure + effective behaviour instead of recursive syntax/container enumeration.

## Complete baseline-to-candidate audit

The full HK04 baseline→candidate diff was re-inspected, not only the latest repair. Product scope remains:

- `Arkus.Game.Authoring`: engine-neutral mutation contract, planner/dry-run surface, internal canonical committer, transactional session and semantic change-coverage oracle;
- `Arkus.Harness.Runtime`: canonical world composition/bindings, one transactional apply handler, mutation-surface equality and a narrow publication defence;
- tests: transactional positives, extension/concurrency coverage and causal controls including the isolated external fixture;
- HK04 exact-SHA scripts/evidence plus the necessary HK01 route-count reconciliation for the expanded accepted base surface.

No Unity types, engine writes, filesystem authoring, gameplay-specific commands, undo UI, transport host or natural-language mutation evaluator are introduced. No production dependency/package expansion was found.

## Adversarial falsification

### Public session / ordinary indirection

Attempt: reproduce Reviewer #2 exactly by hiding the real authoritative session behind `List<>` in a ReadOnly external handler.

Result: the route publishes without any collection-specific walker rule, proving the repair does not special-case the syntax. The fixture can see no public commit method, invocation succeeds only through planning, and canonical revision/hash remain unchanged. If public `Apply` reappears, the same fixture invokes it and state changes before the assertions, making the control causally RED.

### Runtime friend authority

Attempt: exploit the fact that Runtime is the Authoring friend assembly, without relying on a direct committer field that `MutationAuthorityInspector` happens to see.

Result: completeness does not trust object layout. The current effective public non-mutation surface — `system.describe`, six HK03 reads, plan and dry-run — is executed with valid inputs over one live session. All nine complete successfully and preserve revision/hash. The request-vector set must equal the non-mutation definition set, so a new current route forces explicit evaluation. A current hidden friend-based write therefore cannot remain green merely because the structural walker misses its call/field shape.

### Mutation classification / transaction marker

Attempt: declare or bind a canonical mutation without the required transaction policy/handler, or expose a transactional handler that is not a canonical mutation.

Result: `MutationSurfaceConformance` compares canonical mutation definitions, `CanonicalTransaction` policy, effective transactional handler keys and dispatcher membership. HK01 independently checks complete public route/definition/discovery agreement. The retained declaration/marker mismatch control turns this equality red.

### Partial commit / invalid final state

Attempt: make an earlier operation visible when a later operation invalidates the candidate.

Result: operations apply only to private working collections; complete candidate validation precedes the single locked `_current` replacement. The later-failure control preserves hash/inventory.

### Dry-run fork

Attempt: make dry-run predict semantics that apply does not commit.

Result: plan/dry-run/apply share parser + `BuildPlan`; apply adds receipt/CAS/commit. Predicted result hash is compared with accepted canonical hash; divergent candidate control turns red. Plan and dry-run are also covered by the all-non-mutation state-neutral oracle.

### Lost update / race

Attempt: commit two writers from the same anchor.

Result: revision/hash are checked at plan and under the commit lock immediately before replacement. Real concurrent positive permits exactly one commit; stale control prevents overwrite.

### Idempotency

Attempt: advance revision twice on retry or reuse a key for changed semantics.

Result: fingerprinted receipts are checked before planning and under lock. Exact retry replays without revision advance; changed semantics conflict. Session-lifetime scope is explicit residual risk, not represented as durable protection.

### Unreported effect

Attempt: make the plan derive effects from requested declarations instead of actual candidate semantics.

Result: `WorldMutationCoverage` independently diffs base/candidate semantic state; a real type change with empty declared effects turns red.

### Self-shrinking proof universe

Attempt: add/remove a current public route so both implementation and proof silently ignore it.

Result: inherited HK01 route enumeration is independent of canonical definitions/discovery. For the new HK04 side-effect-free guarantee, the evaluated request-vector set must equal all composed non-mutation definitions; omission of a current non-mutation route fails closed before effect assertions.

## Findings fixed during cycle-2 pre-review

1. **Structural authority inspection was still being treated as the completeness argument after the second FAIL.** Fixed causally by removing public session commit authority, removing structural authority from `MutationSurfaceConformance` completeness, and explicitly demoting `MutationAuthorityInspector` to defence in depth.
2. **Closing public `Apply` alone left a proof gap around current friend-assembly Runtime handlers.** Fixed with `EveryEffectiveNonMutationRouteIsEvaluatedAndCannotChangeCanonicalState`, which executes the complete current non-mutation surface and checks real revision/hash effects; the exact request-vector set is itself completeness-checked.

No third material in-claim blocker was found.

## Proof budget / residual-risk verdict

The repair converges: it does not teach the walker about `List<>`, arrays, tuples, wrappers or another syntax form. Instead it removes the public capability that made those shapes dangerous and adds one finite effective oracle over a universe already accepted from HK01. Structural proof machinery has less authority than in cycle 1, not more.

Non-blocking residuals remain explicit: in-memory receipt lifetime, process-local CAS, current schema vocabulary limitations, and resource budgets beyond the operation-count cap. Private-reflection/toolchain subversion remains outside the foundational default trust boundary.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.

## Handoff readiness

No known in-claim blocker remains. `WORKER_PRE_REVIEW: CLEAN` is justified for the implementation observed at `914ab13f…`. After this documentation reconciliation, the resulting exact HEAD must receive GREEN canonical observation; only then may PR metadata record that same HEAD as `Candidate HEAD SHA` and `Frozen candidate SHA`, set `FROZEN_FOR_REVIEW`/`Branch frozen: YES`, and mark the PR Ready. This Worker does not issue the independent PASS/FAIL verdict.
