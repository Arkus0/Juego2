# WP-HK-10 proof matrix

WP: `WP-HK-10 — Strict quality closure`
Boundary: accepted H0 harness semantics through HK09B; closure-only, no new product-semantic capability family.

| Acceptance obligation | Executable/evidence proof | Why the oracle is independent enough |
|---|---|---|
| Canonical serialization/hash property coverage | `Hk10PropertyRobustnessTests.SeededCanonicalSerializationHashAndQueriesAreDeterministicAndReproducible` | Generates independently seeded worlds, compares round-trip bytes/hash, reverses input order and repeats canonical query output. |
| Transaction atomicity | `SeededAtomicityIdempotencyAndStaleRecoveryPreserveIndependentStateAnchors` plus inherited HK04 regression | Rejected multi-op change must leave separately captured revision/hash unchanged; accepted path is checked independently. |
| Idempotency | Same seeded transaction test | Reuses the same request/idempotency key and requires `replayed=true`, unchanged accepted anchor. Causal receipt-lookup defect must RED HK04. |
| Query determinism/canonical order | Seeded repeated-query property plus `Hk10InspectionOrderingTests.ObjectQueryOrderMatchesIndependentOrdinalIdOracle` | Repeatability is checked separately from canonical order. Expected IDs are independently ordinal-sorted from source state; reversing implementation query order must RED this oracle. |
| Replay equivalence | `SeededReplayProducesEquivalentFinalStateAndProvenanceIdentity` | Source and fresh target sessions are compared by final canonical hash/revision and journal entry identity. Replay-chain defect must RED HK06C. |
| Malformed/truncated/unknown-version/unknown-command/schema-invalid robustness | `MalformedTruncatedUnknownVersionUnknownCommandAndSchemaInvalidInputsFailDefined` | Exercises public dispatch plus canonical-world decoder and asserts explicit machine codes and unchanged authoritative anchor. |
| No undefined public output for thrown handlers/validators | `Hk10ExceptionBoundaryTests` plus `ComposedContract.Dispatch` exception boundary | Uses already accepted `world.summary@1.0` and validation handler routes; internal sentinels must not leak and failure must be structured. |
| Persistence interruption | Inherited `Hk09BResourcePersistenceTests` plus causal publication-permit mutation | Accepted staged aggregate must remain authoritative when publication is interrupted; disabling the interrupt must RED. |
| Cancellation | `CancelledMutationFailsBeforePublicationAndKeepsStateAndJournalStable` | Pre-cancelled invocation must return `resource.execution_cancelled` with independently checked revision/hash/journal unchanged. |
| Restart/recovery | `BoundedLongAuthoringSessionExercisesInspectValidateMutateJournalSnapshotAndRestartRecovery` | Final snapshot from the long session is imported into a fresh session and must reproduce revision/hash. This is explicit process-local recovery, not WAL/power-loss durability. |
| Deterministic stale writers/conflicting writers | `SeededConflictingWritersRecoverByExplicitReplanWithoutAutomaticMergeClaim` | Twelve deterministic seeds exercise stale rejection, expected/current anchors, explicit plan/dry-run/apply retry and exactly two journal entries. |
| HK08B recovery remains anchored | Seeded transaction recovery test + inherited HK08B + causal `recovery.expected` misanchor | Misanchoring accepted recovery context must RED the inherited recovery suite. |
| Bounded representative long authoring session | 512-transaction endurance test + `ENDURANCE.md` | Periodic inspect/validate/journal/snapshot, process/managed-memory telemetry and restart import execute accepted primitives below HK09B limits rather than creating a new endurance subsystem/SLO. |
| Capability closure / no alternate privileged host path | Full inherited HK09A regression plus causal elevated-privilege admission defect | HK09A's independently enumerated admission policy must reject the seeded privilege bypass. No HK10 production host route is added. |
| Protocol v1 compatibility corpus and evolution | `Compatibility/protocol-v1.json`, corpus test and inherited composer compatibility regression | Accepted v1 identities/limits are literals outside runtime constants; same-major breaking evolution remains rejected by the canonical composer. |
| Reproducible property seeds | Checked-in seed arrays in HK10 tests; negative controls are exact deterministic substitutions | No time/random-device seed participates; material seeded assertions identify their case. |
| Proof infrastructure fails closed | `hk10-negative-conformance.sh` rejects missing mutation target, compiler/tool failures, false GREENs or dirty mutation worktrees | Run `35524792972` demonstrated this by stopping on an inspection false green; the repaired independent oracle then made the same mutation RED in `35524945085`. |
| Representative content-shape probe | Explicit rerun of accepted Potes hero slice + `CONTENT_SHAPE_PROBE.md` | Uses approved `VISUAL_BIBLE.md` product shape through current public dispatch without inventing a new content vocabulary. |
| Residual inventory reconciliation | `RESIDUAL_RISK.md` against independently maintained ledger v1.9 | Every ledger ID is classified; `UNCLASSIFIED_RESIDUALS: 0`. |
| Exact-SHA CI and regression | `hk10-observe-exact-sha.sh` during mutable Worker phase; `hk10-verify-exact-sha.sh` at freeze | Candidate SHA/clean tree are checked before and after canonical commands. |

## Causal defect universe

The required foundational layers are attacked by 12 deterministic RED controls in `NEGATIVE_CONFORMANCE_MATRIX.md`: protocol discovery, state/hash, inspection, transaction/idempotency, validation, provenance/replay, host framing, HK08A batching, HK08B stale recovery, HK09A capability containment and two HK09B resource/persistence controls (publication interruption and session-envelope drift).

Exact-SHA run `35524945085` demonstrates all twelve controls RED for the intended reason, then a clean full regression GREEN (`216/216`). This mutation universe is deliberately additive to, not a replacement for, inherited predecessor proofs.

## Boundary / non-claims

HK10 does not add or claim automatic merge, per-resource CAS, distributed writers, WAL/fsync or power-loss durability, hard scheduler/memory preemption, authentication/remote tenancy, production shipping SLOs, arbitrary large-world streaming, future-version migration, engine/editor persistence or H1 gameplay semantics. Those are explicitly reconciled in `RESIDUAL_RISK.md` rather than smuggled into closure.

## Proof budget

The post-implementation proof growth corresponds to observed in-boundary proof defects: a synthetic public-route fixture, an invalid internal-access fixture and one real causal false green in inspection ordering. The latter was repaired with one small independent ordering oracle rather than a new subsystem. The required content probe reuses an accepted product-shaped slice. No new product semantics were added to satisfy proof machinery. The proof remains larger than the runtime delta because HK10 is explicitly a quality-closure WP, but it has converged on the named acceptance universe.

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: exact Git checkout; pinned .NET/MSBuild/NuGet documented behaviour; documented runner process/thread/filesystem/OS behaviour; BCL/SHA-256 correctness and collision resistance
PROOF_BUDGET_VERDICT: WITHIN_BUDGET
