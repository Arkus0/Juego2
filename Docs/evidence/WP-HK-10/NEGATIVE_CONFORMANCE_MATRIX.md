# WP-HK-10 negative conformance matrix

Runner: `scripts/hk10-negative-conformance.sh`.

The runner creates a detached disposable worktree at the exact candidate SHA, performs locked restore, injects one controlled defect at a time, runs an independently owned focused oracle, requires an actual test `Failed!` rather than a compiler/tool failure, hard-resets the disposable worktree after each control, and finally proves the disposable tree clean. Missing mutation targets, build failures, false GREENs and missing proof infrastructure all fail closed.

| # | Layer | Controlled defect | Independent oracle | Required outcome |
|---:|---|---|---|---|
| 1 | Protocol discovery | Remove accepted inspection definitions while routes remain | HK03 inspection/composition tests | RED |
| 2 | Canonical state/hash | Replace canonical object sorting with order-dependent reversal | HK10 seeded canonical round-trip/order/hash property | RED |
| 3 | Inspection | Reverse canonical object-query ordering | HK10 independent ordinal-ID ordering oracle | RED |
| 4 | Transaction/idempotency | Force receipt lookup to miss the accepted idempotency key | HK04 transaction tests | RED |
| 5 | Validation | Suppress `WorldStateValidator` violations at aggregation | HK05 validation diagnostics | RED |
| 6 | Provenance/replay | Disable final supplied-current-anchor chain binding | HK06C replay tests | RED |
| 7 | Host framing | Drift frozen JSONL v1 frame limit from 1 MiB to 1 MiB + 1 | HK07A + HK09B transport tests | RED |
| 8 | HK08A batching | Drift accepted coherent transaction ceiling from 96 to 95 | HK08A interaction shape and/or v1 corpus | RED |
| 9 | HK08B stale recovery | Misanchor `recovery.expected` to current state | HK08B conflict-recovery tests | RED |
| 10 | HK09A containment | Deliberately admit elevated host privilege | HK09A host-capability containment tests | RED |
| 11 | HK09B persistence | Ignore publication-interruption permit | HK09B resource/persistence tests | RED |
| 12 | HK09B session/resource growth | Drift session transaction ceiling from 10,000 to 10,001 | HK10 Protocol v1 compatibility corpus | RED |

## Exact causal result

Draft exact-SHA observation `35524945085` on `04b77e0c792d21e2372255011c6a7c15daec7b11` produced all twelve required markers in order and ended with:

`HK10_NEGATIVE_CONFORMANCE GREEN red_controls=12`

The same run then passed the full suite: `216` passed, `0` failed, `0` skipped.

## Fault-injection closure not represented as product mutations

The executable HK10 tests additionally inject runtime faults without changing product semantics:

- cancelled mutation request before publication → `resource.execution_cancelled`, revision/hash/journal unchanged;
- thrown accepted inspection-handler dependency → defined `contract.handler_failure`, no internal sentinel leak;
- thrown validation service through the accepted validation handler → defined `contract.handler_failure`, no internal sentinel leak;
- accepted HK09B publication interruption tests → complete previous aggregate remains authoritative;
- bounded long-session fresh-session recovery → final accepted snapshot imports into a new process-local session with matching revision/hash.

## Proof-infrastructure defects found and fixed

The Worker history is retained because HK10 explicitly requires distinguishing product defects from fixture/tool failures.

1. At SHA `33573e243342a24cbb38ea656877bd1e4475873b`, then-existing focused HK10 tests passed but full regression rejected a synthetic `[PublicCapabilityRoute]` used only by the thrown-handler fixture as `conformance.extra-public-route:fixture.hk10.throw@1.0`. The repair removed the synthetic public route and exercises accepted `world.summary@1.0` instead. The independent HK01 route-universe oracle was not weakened.
2. A later validator-throw fixture tried to reference Runtime internals directly and failed compilation. It was rewritten to instantiate the already accepted validation handler by test-side reflection and compose it through the public `ContractComposer`; no production visibility was widened.
3. Run `35524792972` at SHA `5b1f60dbee29172a89206876f6fcd0f1dfe14719` caught a genuine false-green defect in negative-control #3: reversing query order still passed the selected HK03 fixture because that fixture's reversed input happened to cancel the mutation. The repair added `Hk10InspectionOrderingTests.ObjectQueryOrderMatchesIndependentOrdinalIdOracle`, which derives expected IDs independently using ordinal sort. The next exact-SHA run `35524945085` proved the same inspection mutation RED and all twelve controls GREEN as a runner.

These were proof-fixture/oracle defects, not accepted product defects. Each was fixed at the proof boundary rather than changing product behavior to fit the test.

## Reproduction

Every randomized/property-style input uses checked-in deterministic seeds. Failure messages include `seed=<value>` where the seed materially selects the case. The negative-conformance mutations themselves are deterministic exact-string substitutions and therefore reproduce from the exact candidate SHA.

NEGATIVE_CONTROL_UNIVERSE: 12
FALSE_GREEN_CONTROLS_FOUND_AND_FIXED: 1
KNOWN_UNRESOLVED_FALSE_GREEN_CONTROLS: 0
