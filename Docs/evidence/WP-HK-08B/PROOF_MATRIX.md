# WP-HK-08B foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: Docs/evidence/WP-HK-08B/RESIDUAL_RISK.md
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Central claim

HK08B makes ordinary stale whole-world CAS failures recoverable without changing H0 concurrency semantics. A recovery delta is emitted only when the request's exact revision+hash base is positively proven inside the complete current local HK06A lineage and every required transition is contiguous. The delta is the deterministic union of material affected resources since that base and includes bounded current-resource descriptors. If ancestry/history cannot be proven, recovery returns `bounded-reinspection-required` with no fabricated delta. Retry remains ordinary public plan/dry-run/apply and therefore remains under accepted HK04/HK05/HK06A authority.

The representative external-process client proves the ordinary same-lineage path does not require full-world reconstruction and remains inside evidence-derived request/response/time regression budgets. JSONL and MCP project the same recovery meaning. The required v1.3 content-shape probe additionally drives this recovery contract through a bounded approved Juego2 market-town slice rather than relying only on abstract fixture vocabulary.

## Proof obligations

| Proof obligation | Completeness argument | Positive/effective evidence | Causal negative control | Result |
|---|---|---|---|---|
| stale errors carry stable structured recovery | recovery decorates only stale plan/dry-run/apply results and preserves original machine error fields while adding canonical context | same-lineage test checks schema, expected/current anchors, disposition and history proof | opaque/missing recovery causes direct assertions to fail | PASS |
| all three public mutation routes expose one recovery truth | bindings use the same read-only recovery source/decorator; route test dispatches one stale base through plan/dry-run/apply | `PlanDryRunAndApplyExposeTheSameRecoveryTruthForOneStaleBase` requires complete serialized recovery payload equality | any route omission/drift turns the test red | PASS |
| ancestry requires exact base and complete local history | recovery validates journal base/current against captured authored anchor, sequence continuity, anchor continuity, identity/schema continuity and +1 revisions | two real mutations from the expected journal base produce `history-complete` with two transitions | gapped-sequence service must fail closed as `required-history-unavailable` | PASS |
| rebase/non-ancestor does not fabricate precision | HK06B's new lineage has a new empty local journal; revision equality alone is insufficient | same-revision/different-hash snapshot-rebase case returns bounded fallback and empty delta | a revision-only ancestry algorithm would make the test red | PASS |
| changed-resource delta is complete and deterministic | delta is sorted union of every HK06A `affectedResources` after the proven base | independent changes to `node.peer` and `node.child` require exact two-resource list | omission of either resource fails exact list comparison | PASS |
| removal remains actionable | resource union is history-derived even when current resource no longer exists | add-then-remove case returns `world.object:node.extra` with `presence=absent` | dropping absent current resources fails test | PASS |
| ordinary recovery avoids whole-world reload | current descriptors are generated only for affected material resources and benchmark logs capabilities used after stale rejection | representative conflict performs one descriptor-driven `world.object.get`; broad summary/query calls are forbidden in recovery phase | full-world reload and affected-resource omission oracles turn red | PASS |
| approved product-shaped content remains representable at the recovery boundary | the v1.3 probe maps only approved visual-bible market/bar/workshop relationships onto existing generic object identity, makes a coherent two-resource intent stale through two real same-lineage commits, then requires exact changed-resource identities, descriptor-only reinspection and one normal retry | `CONTENT_SHAPE_PROBE.md` + `RepresentativeMarketMicroBlockStaleEditRecoversByInspectingOnlyChangedResources` cover representability, identity/granularity, inspect, mutate, validate, delta/history and provenance seams without adding game schemas | exact-SHA verifier turns red if the required probe/source/test reconciliation disappears; executable assertions turn red if product-shaped identities or recovery semantics drift | PASS |
| recovery cannot become a second mutation authority | plan/dry-run retain the accepted `WorldMutationPlannerView`; recovery source exposes only current snapshot + journal read; apply still holds HK04 committer | inherited mutation-surface conformance remains GREEN; retry changes revision/journal only through public apply | leaking committer into plan/dry-run is detected by the HK04 object-graph authority guard | PASS |
| retry preserves validation/atomicity/provenance | HK08B never commits or merges; successful retry is a new normal request against returned current anchor | plan -> dry-run -> apply succeeds and journal count advances exactly once for retry | alternate/bypass path would violate inherited HK04/HK05/HK06A regressions | PASS |
| HK05 diagnostics remain complete | HK08B does not replace the authoritative validation artifact; benchmark consumes its complete `diagnostics` collection | invalid flow asserts `diagnosticCount == diagnostics.Length` before repair | independent diagnostic omission oracle returns `independent-diagnostic-hidden` | PASS |
| representative client surface is public and complete | benchmark launches Release JSONL host and executes all five required flows | create/compact inspect/multi-resource modify/invalid-repair/conflict-recovery in one measured client | private-surface and flow-omission controls | PASS |
| coherent edit remains non-chatty | representative two-resource modify is one `authoring.change.apply` | `multiResourceModifyRequestCount=1`; product-shaped probe also retries two resources as one request | synthetic count 2 -> `per-resource-mutation-chatter` | PASS |
| interaction budgets are evidence-derived and executable | truth contract predates measurement; first green external-process observation fixes the baseline and constants enforce bounded headroom | run 35501506939: 12 requests, 12,051 bytes, 185 ms; limits 12 / 15,064 / 1,480 | +1 beyond each limit produces request/response/elapsed budget regression | PASS |
| JSONL/MCP recovery semantics are equivalent | both accepted hosts run as separate Release processes over the same canonical contract; comparison removes request correlation only | stale recovery outcome compared end-to-end | injected changed-resource drift -> `semantic-drift` | PASS |
| HK08B does not pull deferred concurrency/resource scope forward | implementation is recovery decoration + evidence/benchmark only | no merge engine, per-resource CAS/lock, scheduler, distributed transaction or final quota layer is introduced | any such semantic surface would be visible outside this matrix/forbidden scope | PASS |

## Independent/effective universes

1. **Lineage universe:** real HK06A journal transitions are checked independently of stale-error metadata; the rebase case uses equal revision but different hash specifically to kill revision-only reasoning.
2. **History-integrity universe:** `GappedHistoryMutationService` corrupts provenance sequence while leaving underlying authored state coherent, proving recovery validates the proof chain rather than trusting final state alone.
3. **Resource universe:** exact journal-derived affected-resource keys drive the expected set; current resource presence is checked against the immutable captured `WorldState`, including deletion.
4. **Mutation-authority universe:** inherited HK04 conformance recursively inspects effective handler object graphs. The HK08B read helper had to remain behind the accepted attenuation boundary.
5. **Transport universe:** reference JSONL and MCP execute as independent external processes and compare neutral response semantics, not shared in-process objects.
6. **Benchmark universe:** the measurement client itself is public-process based; shape oracles separately reject private measurement, omitted flow, full-world reload, missing affected inspection and mutation chatter.
7. **Budget universe:** request/byte/time limits are executable code reviewed with the WP. Threshold increases cannot happen as an unreviewed runtime configuration change.
8. **Route-equivalence universe:** recovery maps from plan/dry-run/apply are compared through complete deterministic JSON signatures, avoiding dependence on reference equality or collection-specific test-framework overloads.
9. **Product-shape universe:** `Docs/art/VISUAL_BIBLE.md` v0.1.3 is independent of HK08B's abstract fixtures. The bounded market/plaza/bar/workshop slice is mapped through the candidate generic surface, and `CONTENT_SHAPE_PROBE.md` classifies representability, identity/granularity and owned boundary findings without promoting product examples into H0 schemas.

## Worker convergence before freeze

The recovery truth contract was persisted before benchmark budgets. During implementation/convergence the Worker corrected four material proof/quality issues before the first handoff:

1. the first recovery integration let the plan/dry-run handler object graph retain the raw mutation service; HK04 correctly rejected that as apparent write authority. Recovery was moved behind the accepted `WorldMutationPlannerView` attenuation rather than weakening the inspector;
2. the first green benchmark measured cost but did not yet make the recorded values executable regression gates. The first green observation (`8b04a66079cd8a637f0abd8aa1eeb18cd90d9f99`, Actions `35501506939`) was used to freeze explicit constants and causal +1 red controls;
3. strong recovery evidence centered on `plan` even though the implementation decorates plan/dry-run/apply. A route-symmetry test was added to require identical recovery truth on all three public mutation paths without state/provenance change;
4. the first route-symmetry assertion risked exercising dictionary/object equality semantics rather than payload semantics. Candidate `9968d119648996531aa8823ab97786e9a8eb1855` changed the oracle to compare complete deterministic JSON signatures, without changing production behavior.

The first independent review then found one proof-boundary omission rather than a recovery-architecture defect:

5. the candidate had no required v1.3 representative content-shape probe, and the exact-SHA verifier could still declare foundational proof GREEN without one. The repair adds an executable approved-product-shaped stale-recovery scenario, preserves its analysis in `CONTENT_SHAPE_PROBE.md`, reconciles this proof/pre-review evidence, and makes the exact-SHA gate require the probe/source/test linkage. Production recovery/CAS/journal/transports are unchanged.

Incidental compile/fixture syntax mistakes encountered before these gates were corrected before candidate freeze and are not counted as proof discoveries.

## First measured benchmark evidence

Candidate `8b04a66079cd8a637f0abd8aa1eeb18cd90d9f99` passed Actions run `35501506939` before budget constants were added:

- Release build: 0 warnings / 0 errors;
- focused HK08B: 8/8 GREEN;
- full regression: 181/181 GREEN;
- benchmark: 5 flows, 12 requests, 12,051 serialized response bytes, 185 ms;
- full-world recovery reloads: 0;
- public transport only: true;
- candidate clean before/after: YES;
- artifact: `10602945775`.

The repaired candidate must additionally pass the exact-SHA verifier containing the executable budgets, route-symmetry, required representative content-shape and evidence gates before handoff.

## Proof-budget verdict

The proof stays bounded to the new recovery seam and representative interaction path. The reviewer repair adds one product-shaped scenario and one gate linkage; it does not introduce another recovery mechanism, generic content model or speculative concurrency framework. The negative matrix covers each material owned false-green class, and residual deferred mechanisms are explicit rather than silently absorbed into HK08B.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET
