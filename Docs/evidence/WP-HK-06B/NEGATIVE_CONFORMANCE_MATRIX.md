# WP-HK-06B negative-conformance matrix

These controls are bounded to HK06B: semantic authored-resource diff, versioned snapshot integrity/import, and truthful snapshot/history/live-state boundaries. Accepted predecessor guarantees are consumed rather than re-proved.

| Required defect class | Controlled defect / mutant | Oracle expected RED | Effective GREEN evidence |
|---|---|---|---|
| hidden authored-state change absent from semantic diff | independently mutate every current object field class, extension payload/dependency class, object existence, and extension existence; separately remove an expected extension resource from the oracle input | missing resource/field causes the focused test or `SemanticCoverageIssues` to report `missing:<resource>` | effective diff matches test-owned resource projection; extension add/remove are exact-set checked |
| semantic diff invents a resource | add `world.object:invented` to observed-resource mutant | independent coverage oracle reports `extra:world.object:invented` | effective add/remove extension resource sets exactly equal independent projection |
| serializer/input ordering creates false semantic change | construct the same MicroWorld with reversed object/extension/reference input order | non-empty diff would fail | canonical hash is unchanged and semantic diff is empty |
| altered snapshot data accepted as same artifact | change declared anchor hash or mutate embedded canonical bytes | import must fail with `world.snapshot.anchor_mismatch` or `world.snapshot.invalid_state` | untouched export parses and round-trips to the same canonical hash |
| unsupported snapshot version accepted | set `snapshotVersion = 2` | `world.snapshot.unsupported_version` | exported version 1 succeeds |
| invalid snapshot partially replaces state | run every rejected import against a live target | target revision/hash/journal change would fail the test | all rejected imports leave revision/hash/journal unchanged |
| runtime-only data leaks into canonical snapshot | set `runtimeObservationsIncluded = true`; separately advance a test-owned runtime surrogate | boundary violation, or changed authored snapshot bytes, is RED | exports carry `runtimeObservationsIncluded=false`; runtime surrogate steps do not change authored bytes |
| snapshot import fabricates/retains mutation history | import a snapshot exported after a source mutation into a clean target | non-zero retained/imported journal count or inherited source entry is RED | target local journal is empty; its base/current anchors equal the imported state; first later mutation becomes local entry 1 |
| snapshot import violates canonical mutation idempotency contract | retry exact accepted import; reuse key with changed request semantics | duplicate state/history effect or acceptance of conflicting semantics is RED | exact retry returns `replayed=true` without a second effect; conflicting key returns `world.snapshot.idempotency_conflict` |
| public contract omits portability routes/schemas | canonical route/discovery regression enumerates effective surface | definition/route/projection mismatch turns inherited HK01 conformance RED | compare/export/import are discoverable at `1.0`, schema-valid and route-complete |
| read-only portability route gains write authority | evaluate all effective non-mutation routes against canonical hash/revision | any diff/export state mutation fails inherited HK04 behavioural oracle | diff/export execute through attenuated read-only view with unchanged state |

## Pre-review repair note

The Worker pre-review found that the original focused suite covered object add/remove and extension field changes but did not explicitly cover **extension add/remove**, leaving a material accepted resource class without a causal omission control. `Hk06BNegativeConformanceTests` closes that class with a distinct extension identity, exact independent resource-set comparison, an omission mutant and a false-extra mutant.

The first attempted fixture accidentally reused the existing `future.beta@1@global` identity and correctly failed HK02A validation before reaching the diff. That incidental failure is not counted as negative-conformance evidence. The repaired fixture uses `future.gamma@3@global` and the intended causal controls pass.
