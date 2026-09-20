# WP-HK-08B Worker pre-review

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 4
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-08B/WORKER_PRE_REVIEW.md

## Candidate challenged

- WP: `WP-HK-08B`.
- Baseline: `dc8b6ee3d4ea62df70e28605268526c726de69d8`.
- Branch: `wp/hk-08b-conflict-recovery`.
- Direct predecessor: accepted + DocSynced `WP-HK-08A`.
- First measured benchmark candidate: `8b04a66079cd8a637f0abd8aa1eeb18cd90d9f99`.
- Benchmark observation: Actions `35501506939`, artifact `10602945775`.
- Implementation/test SHA after final Worker test-oracle repair: `9968d119648996531aa8823ab97786e9a8eb1855`.

This is Worker quality-gate evidence, not an independent Reviewer verdict. The eventual frozen SHA must still pass the exact-SHA verification job after evidence-only reconciliation.

## Scope/predecessor challenge

The pre-review treated HK04 mutation authority, HK05 diagnostic completeness, HK06A provenance truth, HK06B rebase lineage semantics and HK07/08A transport/interaction primitives as accepted predecessors. They were reopened only where the new HK08B effective path supplied evidence that their boundary mattered: specifically, the HK04 authority inspector caught the first recovery wiring because the read-only handlers retained the raw service object graph.

No automatic merge, per-resource CAS/locks, distributed coordination, autonomous scheduling, final quota layer or gameplay semantics were introduced.

## Finding 1 — recovery helper initially crossed the accepted HK04 attenuation boundary

Initial wiring constructed recovery from the raw `IWorldMutationService` and retained it beside `WorldMutationPlannerView` in plan/dry-run handlers. Even though recovery itself did not call apply, the accepted HK04 object-graph conformance correctly saw effective canonical write authority reachable from those handlers.

Repair:

- introduced narrow read-only `IWorldConflictRecoverySource` exposing only current snapshot and journal read;
- implemented that source on the already accepted `WorldMutationPlannerView` attenuation;
- plan/dry-run receive only that attenuated object graph;
- apply still receives the HK04 `ICanonicalWorldMutationCommitter` plus the same read-only recovery helper;
- did **not** weaken or special-case `MutationAuthorityInspector`.

The next Actions observation was GREEN, showing the recovery seam fits the predecessor authority boundary rather than bypassing it.

## Finding 2 — measured budgets were evidence but not yet enforceable regression gates

The first complete public-client benchmark correctly persisted 5 flows / 12 requests / 12,051 bytes / 185 ms and proved no recovery full-world reload, but a future regression could still have changed those numbers without failing the test.

Repair:

- froze request maximum at 12;
- froze response maximum at 15,064 bytes (25% bounded headroom from the measured 12,051);
- froze elapsed pathological-regression guard at 1,480 ms (8x measured 185 ms, explicitly not an SLO);
- the real benchmark now fails above any threshold;
- causal controls inject one unit over each maximum and require distinct request/response/elapsed regression codes;
- baseline and maximum values are emitted in benchmark evidence.

This satisfies the WP rule that budgets come from measurement first and become CI/review-controlled afterward.

## Finding 3 — public recovery route symmetry was implemented but under-proved

Strong causal coverage exercised stale `plan`, while the production binding attaches recovery to `plan`, `dry-run` and `apply`. Relying on common helper code would leave a route-binding omission as an avoidable reviewer inference.

Repair:

`PlanDryRunAndApplyExposeTheSameRecoveryTruthForOneStaleBase` now dispatches one real stale base independently through all three public mutation capabilities and requires:

- stale rejection on every route;
- `same-lineage-replan` on every route;
- exact changed-resource set on every route;
- identical structured recovery maps;
- no extra authored-state revision or provenance entry from the failed plan/dry-run/apply calls.

## Finding 4 — route-symmetry oracle initially risked testing object equality instead of semantic equality

The first form of the new route-symmetry test compared `IReadOnlyDictionary` values directly with `Assert.Equal`. That leaves overload/equality behavior as an unnecessary test-framework ambiguity and could produce a false red even when the public recovery payloads are semantically identical.

Repair:

- each recovery map is serialized deterministically through `System.Text.Json`;
- the test compares those complete serialized semantic signatures;
- no field is removed or normalized away;
- production behavior is unchanged.

This is a test-oracle hardening finding, not a new product semantic.

## False-green challenge

The reconciled candidate has been challenged for:

- revision-only or hash-only ancestry reasoning;
- accepting a rebase predecessor as current-lineage ancestor;
- tolerating journal sequence/anchor gaps;
- omitting a material changed resource or a resource deleted since the stale base;
- broad/full-world reconstruction during ordinary same-lineage recovery;
- leaking commit authority back into read-only mutation handlers;
- merging/retrying through a path outside normal plan/dry-run/apply;
- suppressing independent HK05 diagnostics for compact repair presentation;
- measuring a private/in-process path instead of the accepted public host;
- silently omitting one of the five representative flows;
- per-resource mutation chatter for a coherent multi-resource intent;
- request, response-volume or elapsed-time regression beyond frozen budget;
- JSONL/MCP semantic drift in recovery disposition/context;
- route-equivalence proof depending on reference/object equality rather than complete semantic payload equality.

The causal controls for these classes are enumerated in `NEGATIVE_CONFORMANCE_MATRIX.md`. No known in-boundary blocker remains.

## Benchmark evidence

The first post-truth-contract public-process observation on candidate `8b04a66079cd8a637f0abd8aa1eeb18cd90d9f99` produced:

- 5 representative flows;
- 12 public requests;
- 12,051 serialized response bytes;
- 185 ms measured harness interaction time;
- 1 changed resource and 1 bounded recovery inspection in the representative stale flow;
- 0 recovery full-world reloads;
- 1 request for the coherent multi-resource modify;
- focused HK08B 8/8 GREEN and full regression 181/181 GREEN.

Budget derivation and interpretation are recorded in `INTERACTION_BENCHMARK.md`.

## Final handoff condition

Evidence-only reconciliation after `9968d119648996531aa8823ab97786e9a8eb1855` must not alter production/test semantics. The final branch HEAD is eligible for freeze only when:

- canonical observation is GREEN on that exact HEAD;
- `scripts/hk08b-verify-exact-sha.sh` is GREEN on the exact frozen SHA;
- candidate is clean before/after;
- PR body `Frozen candidate SHA` equals HEAD;
- no Worker commits follow freeze.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET

No known in-boundary Worker blocker remains; independent review is still required.
