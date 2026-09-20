# WP-HK-10 negative conformance matrix

Runner: `scripts/hk10-negative-conformance.sh`.

The runner creates a detached disposable worktree at the exact candidate SHA, performs locked restore, injects one controlled defect at a time, runs an independently owned focused oracle, requires an actual test `Failed!` rather than a compiler/tool failure, hard-resets the disposable worktree after each control, and finally proves the disposable tree clean. Missing mutation targets, build failures, false GREENs and missing proof infrastructure all fail closed.

| # | Layer | Controlled defect | Independent oracle | Required outcome |
|---:|---|---|---|---|
| 1 | Protocol discovery | Remove accepted inspection definitions while routes remain | HK03 inspection/composition tests | RED |
| 2 | Canonical state/hash | Replace canonical object sorting with order-dependent reversal | HK10 seeded canonical round-trip/order/hash property | RED |
| 3 | Inspection | Reverse canonical object-query ordering | HK03 inspection tests | RED |
| 4 | Transaction/idempotency | Force receipt lookup to miss the accepted idempotency key | HK04 transaction tests | RED |
| 5 | Validation | Suppress `WorldStateValidator` violations at aggregation | HK05 validation diagnostics | RED |
| 6 | Provenance/replay | Disable final supplied-current-anchor chain binding | HK06C replay tests | RED |
| 7 | Host framing | Drift frozen JSONL v1 frame limit from 1 MiB to 1 MiB + 1 | HK07A + HK09B transport tests | RED |
| 8 | HK08A batching | Drift accepted coherent transaction ceiling from 96 to 95 | HK08A interaction shape and/or v1 corpus | RED |
| 9 | HK08B stale recovery | Misanchor `recovery.expected` to current state | HK08B conflict-recovery tests | RED |
| 10 | HK09A containment | Deliberately admit elevated host privilege | HK09A host-capability containment tests | RED |
| 11 | HK09B persistence | Ignore publication-interruption permit | HK09B resource/persistence tests | RED |
| 12 | HK09B session/resource growth | Drift session transaction ceiling from 10,000 to 10,001 | HK10 Protocol v1 compatibility corpus | RED |

## Fault-injection closure not represented as product mutations

The executable HK10 tests additionally inject runtime faults without changing product semantics:

- cancelled mutation request before publication → `resource.execution_cancelled`, revision/hash/journal unchanged;
- thrown accepted inspection-handler dependency → defined `contract.handler_failure`, no internal sentinel leak;
- thrown validation service through the accepted validation handler → defined `contract.handler_failure`, no internal sentinel leak;
- accepted HK09B publication interruption tests → complete previous aggregate remains authoritative;
- bounded long-session fresh-session recovery → final accepted snapshot imports into a new process-local session with matching revision/hash.

## Harness-defect versus product-defect control

The first Draft observation of HK10 intentionally remains part of Worker history. At SHA `33573e243342a24cbb38ea656877bd1e4475873b`, all then-existing HK10 focused tests passed, but full regression failed because the first thrown-handler fixture itself declared a synthetic `[PublicCapabilityRoute]`. HK01's independent effective-route universe correctly rejected it as `conformance.extra-public-route:fixture.hk10.throw@1.0`.

The repair did **not** weaken that oracle. Commit `ac5e1557a723e9b0fe516846ccf61fcecc01bed9` removed the synthetic public fixture and exercises an already accepted `world.summary@1.0` route with a throwing state source. Subsequent full observation at `2e94ea4da426c2df6d39d578dcce8baba81a0413` is GREEN in Actions run `35524338370`. This is the required distinction between a proof-fixture defect and a harness defect.

## Reproduction

Every randomized/property-style input uses checked-in deterministic seeds. Failure messages include `seed=<value>` where the seed materially selects the case. The negative-conformance mutations themselves are deterministic exact-string substitutions and therefore reproduce from the exact candidate SHA.

Expected final runner marker:

`HK10_NEGATIVE_CONFORMANCE GREEN red_controls=12`

NEGATIVE_CONTROL_UNIVERSE: 12
KNOWN_FALSE_GREEN_CONTROLS: 0
