# WP-HK-08B negative conformance matrix

Every row below is an in-scope causal attack or false-green class. A negative control must fail because the owned HK08B guarantee is violated, not merely because unrelated validation happens to fail first.

| Defect class | Causal control / effective evidence | Required red signal | Status |
|---|---|---|---|
| opaque stale conflict forces whole-world reconstruction | same-lineage stale plan asserts `arkus.world-conflict-recovery@1`, exact expected/current anchors, `same-lineage-replan`, changed-resource union and bounded inspection descriptors | missing recovery context/descriptor fails `SameLineageStaleConflictReturnsCompleteAffectedOnlyRecoveryAndExplicitRetrySucceeds` | GREEN |
| recovery differs between mutation routes | one stale base is dispatched through public `plan`, `dry-run` and `apply`; recovery maps must be identical | any route missing/drifting recovery fails `PlanDryRunAndApplyExposeTheSameRecoveryTruthForOneStaleBase` | GREEN |
| wrong expected/current anchor | same-lineage test compares expected pair to original state and current world/schema/revision/hash to authoritative session | wrong anchor assertion | GREEN |
| material changed resource omitted | two independent commits change `node.peer` and `node.child`; recovery must return the exact sorted union | expected exact two-resource list mismatch | GREEN |
| removal disappears from delta | add then remove `node.extra` after stale base | resource omission or non-`absent` descriptor fails `ChangedResourceRemovedSinceExpectedBaseIsReturnedExplicitlyAsAbsent` | GREEN |
| revision-only ancestry guess crosses rebase | snapshot import deliberately produces same revision but different hash and fresh empty local journal | any trustworthy delta / `same-lineage-replan` fails `SnapshotRebaseWithSameRevisionButDifferentHashNeverFabricatesAncestryOrDelta` | GREEN |
| gapped/unavailable history falsely claims delta | provenance attack corrupts first journal sequence while underlying authored state remains coherent | must return `bounded-reinspection-required`, `required-history-unavailable`, empty delta | GREEN |
| recovery retry bypasses ordinary transaction/validation/provenance | affected-only recovery test retries only via public plan/dry-run/apply and checks revision + journal entry count | alternate commit or missing provenance breaks inherited HK04/HK05/HK06A assertions | GREEN |
| plan/dry-run obtain write authority through recovery helper | Runtime mutation-surface conformance traverses handler object graph; recovery source for read-only handlers is the accepted attenuated planner view | handler construction/conformance turns red if committer leaks back into plan/dry-run | GREEN |
| independent HK05 diagnostic hidden | synthetic authoritative set `{a,b,c}` compared with presentation omitting `b` | `independent-diagnostic-hidden` | GREEN |
| benchmark measures private path | benchmark client launches external JSONL process; shape oracle injects `publicTransportOnly=false` | `private-surface` | GREEN |
| representative benchmark flow omitted | benchmark executes all five named flows; synthetic omission flag | `representative-flow-omitted` | GREEN |
| required representative content-shape probe omitted or detached from its executable scenario | exact-SHA verifier requires the probe file, approved `VISUAL_BIBLE` source marker, PASS/zero-unresolved markers, executable test identifier, test file and matching test method | deleting the evidence/test or changing any required reconciliation marker makes `hk08b-verify-exact-sha.sh` red | GREEN |
| conflict recovery reloads whole world | recovery-phase capability log forbids `world.summary`, object query and extension query | `full-world-reload` / direct capability assertion | GREEN |
| affected-resource inspection omitted | representative stale conflict has one changed resource and exactly one descriptor-driven inspection; synthetic 2 vs 1 case | `affected-resource-omitted` | GREEN |
| coherent edit degenerates to mutation chatter | two-resource modify is one `authoring.change.apply`; synthetic count 2 | `per-resource-mutation-chatter` | GREEN |
| request-count regression | frozen evidence-derived maximum 12; attack supplies 13 | `request-budget-regression` | GREEN |
| serialized-response regression | frozen maximum 15,064 bytes; attack supplies 15,065 | `response-budget-regression` | GREEN |
| elapsed pathological regression | frozen maximum 1,480 ms; attack supplies 1,481 | `elapsed-budget-regression` | GREEN |
| JSONL/MCP recovery semantic drift | two external processes execute the same stale recovery and neutral outcomes are compared after removing request correlation only | injected changed-resource drift -> `semantic-drift` | GREEN |

## Boundary notes

The matrix deliberately does not add attacks for per-resource locks, automatic merge, distributed coordination, multi-agent scheduling or hostile-client authorization because HK08B forbids/defers those mechanisms. Their absence is not a false green inside this WP.

The ancestry/history controls are intentionally stricter than the benchmark. A benchmark improvement cannot make a non-ancestor case recoverable, cannot excuse a gapped journal and cannot remove a material affected resource.
