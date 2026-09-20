# WP-HK-10 residual-risk reconciliation

Source universe: `Docs/engineering/RESIDUAL_LEDGER.md` v1.9. This file does not invent a smaller universe: every ledger entry is classified below.

## Classification rule

- `HK10-COVERED` means the residual is inside HK10's declared closure boundary and is exercised by HK10 evidence/causal controls.
- `CLOSED-BY` preserves an independently recorded accepted closure and does not reopen it.
- `OUT-BOUNDARY` means satisfying the residual would require a semantic, durability, scaling, security, engine, lifecycle or product-SLO claim that H0 explicitly does not make. HK-GATE must name these residual classes rather than treating them as hidden green.
- `DEFERRED` preserves a named later owner already present in the independent ledger.

## Trusted base

| ID | HK10 classification | Rationale |
|---|---|---|
| R-TB-01 | OUT-BOUNDARY | Git implementation correctness remains in the foundational trusted base. Exact SHA and clean-tree observations are checked, not Git reimplemented. |
| R-TB-02 | OUT-BOUNDARY | Pinned SDK/MSBuild/compiler documented behaviour remains trusted base. |
| R-TB-03 | OUT-BOUNDARY | NuGet implementation/service correctness behind locked restore remains trusted base. |
| R-TB-04 | OUT-BOUNDARY | Runner OS/hypervisor/Actions compromise remains trusted base. |
| R-TB-05 | OUT-BOUNDARY | SHA-256/BCL correctness and collision resistance remain trusted base. |

## Named-owner / already-closed ledger entries

| ID | HK10 classification | Evidence/boundary |
|---|---|---|
| R-00A-02 | CLOSED-BY | HK07B accepted dependency/adoption audit; ledger cites PASS #5259854121. |
| R-00A-05 | DEFERRED | H1 engine abstractions/Unity capability definitions. |
| R-01-07 | DEFERRED | Future reviewed external plugin-loader workpack. |
| R-01-08 | CLOSED-BY | HK07B accepted MCP/JSONL parity; ledger cites PASS #5259854121. |
| R-01-09 | DEFERRED | H1 Unity/editor scoped-provider instantiation. |
| R-02A-04 | CLOSED-BY | HK09B accepted payload/dependency/world limits; PASS #5261068513. |
| R-04-04 | CLOSED-BY | HK09B accepted request/payload/extension/world/session envelope; PASS #5261068513. |
| R-05-05 | DEFERRED | H1 engine/editor validation. |
| R-05-06 | DEFERRED | Shipping latency/throughput/resource SLO evidence; HK08B/HK09B explicitly do not claim product SLOs. |
| R-06A-02 | CLOSED-BY | HK06B accepted new-local-lineage rebase semantics; PASS #5258130916. |
| R-06A-03 | CLOSED-BY | HK06C deterministic replay/compatibility; PASS #5259530509. |
| R-06A-04 | CLOSED-BY | HK06B semantic diff; PASS #5258130916. |
| R-06A-05 | HK10-COVERED | `Hk10EnduranceCompatibilityTests.BoundedLongAuthoringSessionExercisesInspectValidateMutateJournalSnapshotAndRestartRecovery` executes 512 canonical transactions, periodic inspect/validate/journal/snapshot, process/managed-memory telemetry and fresh-session snapshot recovery inside the accepted 10,000-transaction/1,024-import HK09B ceilings. The v1 corpus also freezes those ceilings and a seeded defect changing the session ceiling must RED. |
| R-06A-07 | CLOSED-BY | HK07B accepted transport/storage semantic parity; PASS #5259854121. |
| R-06B-02 | CLOSED-BY | HK06C replay/compatibility; PASS #5259530509. |
| R-06B-03 | CLOSED-BY | HK07B local JSONL/MCP snapshot parity; PASS #5259854121. |
| R-07A-01 | CLOSED-BY | HK09B accepted cooperative dispatch/publication resource envelope; PASS #5261068513. |
| R-07A-02 | CLOSED-BY | HK09A removed caller-selected production file authority and enforced host admission; PASS #5260496498. |
| R-07A-03 | CLOSED-BY | HK08B stale recovery/interaction benchmark; PASS #5260337340. |

## Open ledger entries — HK10 classification

| ID | HK10 classification | Rationale / in-boundary counterpart |
|---|---|---|
| R-00-05 | OUT-BOUNDARY | Supply-chain/vendor compromise that preserves expected identity is outside repository-local H0. Pinned identity/locked restore remain checked. |
| R-00-06 | OUT-BOUNDARY | Semantic game-content code is product/H1 content growth, not generic harness closure. |
| R-01-01 | OUT-BOUNDARY | Expanding the canonical schema vocabulary is a new public contract feature. HK10 instead causally attacks the accepted service-level validation/contract guards. |
| R-01-03 | OUT-BOUNDARY | Cross-provider/shared logical-reference vocabulary is new semantic modelling. |
| R-01-05 | OUT-BOUNDARY | Cross-major migration guidance is contract-lifecycle work beyond Protocol v1 compatibility. |
| R-02-01 | OUT-BOUNDARY | Persisted-world migration beyond the accepted fail-closed current format is future lifecycle semantics. |
| R-02-04 | OUT-BOUNDARY | Rich display names/localization are content/UI semantics. |
| R-02-05 | OUT-BOUNDARY | Cross-world/external asset references require a new reviewed reference model. |
| R-02A-01 | OUT-BOUNDARY | Generic interpretation of undeclared identities hidden in opaque bytes is impossible without changing the opaque-extension contract. Declared identity/reference surfaces remain validated. |
| R-02A-02 | OUT-BOUNDARY | Per-resource CAS/locking and automatic merge are explicitly absent product concurrency semantics. HK10 does stress deterministic whole-world stale writers, anchored recovery and explicit replan without claiming merge. |
| R-03-01 | OUT-BOUNDARY | Sublinear/indexed or world-size-independent CPU complexity is post-GATE scale work. HK10 preserves deterministic bounded-output query behaviour inside finite H0 limits. |
| R-03-02 | OUT-BOUNDARY | Cursor authentication/MAC is an authorization/security boundary H0 does not claim. Accepted cursor integrity/determinism remains under regression. |
| R-03-03 | OUT-BOUNDARY | Multi-command snapshot leases would be a new capability/consistency model. Existing stale anchors fail closed and are stressed by HK10. |
| R-04-01 | OUT-BOUNDARY | WAL/fsync, process-kill and power-loss durability are explicitly outside `process-local-checkpoint`. HK10 covers owned in-process publication interruption plus explicit fresh-session snapshot recovery only. |
| R-04-02 | OUT-BOUNDARY | Multi-process/distributed writers and multi-agent coordination are beyond local H0. |
| R-04-05 | OUT-BOUNDARY | Asymptotic very-large-world performance is post-GATE scale evidence, not an H0 claim. |
| R-05-01 | OUT-BOUNDARY | HK05 deliberately defines iterative diagnostics under ambiguous identity. Adding a public partial-report marker would change accepted semantics; HK10 retains accepted independent-diagnostic regressions and a validation defect control. |
| R-06B-04 | OUT-BOUNDARY | Gameplay/runtime transforms, clocks, physics, animation, AI and simulation are outside authored `WorldState`. |
| R-06B-05 | OUT-BOUNDARY | Merging/reconciling independent histories is a new lineage/merge contract. H0 rejects hidden splicing; HK10 does not add automatic merge. |
| R-06B-06 | OUT-BOUNDARY | Streaming/chunking and arbitrary large-history/world throughput are beyond finite HK09B bounds. HK10 supplies bounded representative endurance within those bounds. |
| R-06C-01 | OUT-BOUNDARY | Durable lost-response replay receipts would add persistence/idempotency semantics not accepted by HK06C. HK10 proves replay equivalence for accepted replay semantics. |
| R-06C-02 | OUT-BOUNDARY | Future journal/snapshot versions and migration paths are future lifecycle work; HK10 freezes the accepted Protocol v1 compatibility corpus. |
| R-07A-04 | OUT-BOUNDARY | Authentication, remote tenancy, encryption/network service security and hostile-OS isolation are explicitly absent from local H0. |
| R-07B-01 | OUT-BOUNDARY | MCP bounded surrogate handle stability across inventory changes is intentionally discovery-scoped; canonical identity remains stable. Changing this would be transport lifecycle semantics. |
| R-07B-02 | OUT-BOUNDARY | Future MCP SDK/protocol upgrades require their own dependency/license/conformance review. |
| R-07B-03 | OUT-BOUNDARY | Certification of every vendor/client product is outside the accepted standards-compatible local stdio claim. |
| R-08A-01 | OUT-BOUNDARY | Removing/deprecating/changing complete `authoring.journal.read@1.0` is an explicit compatibility-lifecycle decision. HK10 measures a bounded representative long session rather than silently versioning v1. |
| R-08B-01 | OUT-BOUNDARY | Unknown future material-resource vocabularies require a reviewed semantic extension; current recovery preserves them visibly rather than fabricating an inspection contract. |
| R-09B-01 | OUT-BOUNDARY | Hard scheduler preemption and exact process memory/output ceilings are explicitly outside HK09B. HK10 measures process/session growth and keeps the accepted cooperative publication checks under regression/causal attack. |

## Reconciliation result

- Every ledger entry is classified.
- The only independently declared residual newly owned by HK10 is `R-06A-05`, and HK10 supplies executable bounded-session evidence for it.
- Accepted predecessor closures are consumed, not reopened without causal evidence.
- Remaining open items require claims H0 explicitly does not make; they are named here for HK-GATE rather than hidden behind a green test suite.
- No `UNCLASSIFIED` ledger entry remains in this HK10 reconciliation.

RESIDUAL_LEDGER_RECONCILIATION: COMPLETE
UNCLASSIFIED_RESIDUALS: 0
