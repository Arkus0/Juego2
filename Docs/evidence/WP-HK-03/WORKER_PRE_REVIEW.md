# WP-HK-03 Worker pre-review

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 2
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-03

Pre-review input HEAD: `97d571b32c53aa1e6333d6e02e35826df7058dce`  
Baseline: `39b0660193af53575aab26d3e5ff567a48ea86f3`  
Latest product-behaviour observation before proof-doc commits: `05e25995d5c0ad91954bded22ab65e7c49097ea2`, Actions `35438474195` — 0 warnings / 0 errors, 7/7 positive HK03 tests, 7/7 causal controls, 70/70 regression.

This report is the final Worker-authored repository file. Its own commit changes documentation only; the canonical observation and frozen exact-SHA verification must both rerun on the final HEAD before handoff.

## Contract and predecessor re-check

Re-read `WP-HK-03`, `WORKER_REVIEW_PROTOCOL.md` v1.5 and `FOUNDATIONAL_PROOF_STANDARD.md` v1.2. Current `main` remains `39b0660193af53575aab26d3e5ff567a48ea86f3`; no accepted dependency state changed during the Worker cycle.

`WORKER_PLAN.md` still matches the accepted direct predecessor:

- HK02 reviewed SHA `f23fba9ab81566e682237183cf92618bbce504bd`;
- independent PASS on PR #17;
- merge SHA `1605f922b74d4f0b71bba7896c311a914ffed3f7`;
- HK03 consumes HK02 canonical state/identity and HK01 canonical composition/discovery guarantees rather than duplicating them;
- no concrete contradictory evidence was found that would reopen either accepted predecessor boundary.

## Complete baseline-to-candidate audit

The complete baseline diff was inspected, not only the last repair. Product changes are limited to:

- Authoring-owned inspection abstractions, canonical HK03 definitions and pure read semantics;
- Runtime-owned canonical bindings and the composed `system + world` base contribution;
- exact-SHA HK03 observation/verification routing;
- HK03 positive/causal tests plus the existing HK01 effective-route conformance test updated to evaluate the expanded canonical base contribution;
- HK03 evidence.

No `.csproj` dependency change was introduced. `Arkus.Harness.Runtime` still references Protocol + Authoring + Validation and does not directly reference `Arkus.Game.World`. No mutation/apply path, Unity/engine bridge, gameplay system, transport host or natural-language evaluator appears in the candidate.

## Falsification checks

### Completeness / self-shrinking universe

Attempt: remove a semantic read mapping while allowing the proof universe to shrink with it.

Result: blocked by two independent/effective oracles. The current HK02 public state-bearing property surface is reflected and compared with an explicit expected inventory independent of HK03 definitions. Effective concrete public handlers are independently reflected by HK01 `RouteUniverse` and compared against canonical definitions/dispatch/discovery. Full micro-world semantics are reconstructed only through public reads and compared through the accepted canonical HK02 hash.

No current semantic field or effective world route is unclassified.

### Bounded-output false green

Attempt: hide an unbounded collection under a paginated top-level result.

Result: this exposed a real defect during the Worker cycle: object rows initially nested the complete `References` collection, allowing a single object to bypass page bounds. The product architecture was corrected by removing relationship collections from object output and making `world.reference.query` the sole complete relationship channel. The retained 130-edge causal control proves 100 + 30 deterministic pages while object lookup stays fixed-size.

### Canonical-route false green

Attempt: add public handlers without canonical definition/discovery coverage.

Result: the first real HK03 CI pass turned red because the existing HK01 independent route universe saw all six `world.*` handlers as extras relative to the old base-only composition. The repair preserved that independent universe and changed the effective canonical base composition used by the proof to include the world definitions/routes. No scan/reflection root was narrowed and no handler was ignored.

### Selector / error boundary

Expression-like stable-token input is rejected; page/chunk bounds fail closed; missing object, stale revision and stale cursor are structured errors. Generic malformed request shapes remain structured HK01 contract errors. The read language contains only fixed schema fields/enums/identifier arrays/integers/cursors and no executable expression mechanism.

### Determinism / stale state

Canonical object and relationship ordering is explicit and independent of HK02 input order. Cursor context includes command, normalized selector/projection/limit fingerprint, revision/hash and offset. Repeated reads against an unchanged state return the same ordering/cursor; source advancement rejects old revision/cursor contexts rather than mixing state.

### Side effects

Inspection service only reads a captured canonical `WorldState`; repeated calls leave the canonical HK02 content hash unchanged. Definitions declare read-only/deterministic/idempotent semantics. No write API was added.

## Material findings fixed before freeze

1. **Unbounded nested relationship response** — causal product defect. Fixed by separating fixed-size object projection from paginated relationship reconstruction; protected by `UnboundedNestedRelationshipMutantIsSplitIntoBoundedReferencePages`.
2. **Expanded effective route universe not reconciled with the old base-only HK01 conformance fixture** — proof/integration false-green boundary. Fixed without weakening `RouteUniverse`; protected by `EveryEffectiveWorldReadIsDiscoveredAndMissingSchemaMutantTurnsRed` and the updated HK01 production route-universe regression.

No third material in-claim blocker was found in the final adversarial pass.

## Residual-risk / proof-budget classification

Recorded non-blocking residuals are bounded-output-vs-scan-cost, cursor non-authentication, cross-call snapshot lifetime, generic schema vocabulary lacking numeric min/max facets, and intentional future-model incompleteness. None falsifies the current HK03 claim inside its trust boundary.

Proof machinery remains proportional: no third-party proof framework, no duplicate HK02/HK01 re-proof, seven causal controls for six mandatory classes plus the observed unbounded nested-edge variant, and reuse of the existing independent route oracle. There were no two consecutive proof-only expansion cycles.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET` remains justified.

## Handoff readiness

No known blocker remains. Freeze is permitted only if the post-report canonical observation is GREEN on the report commit, PR HEAD is then read as an exact 40-character SHA, no Writer changes occur, the PR handoff records `FROZEN_FOR_REVIEW`, and the Ready-triggered exact-SHA verification is GREEN for that same SHA.
