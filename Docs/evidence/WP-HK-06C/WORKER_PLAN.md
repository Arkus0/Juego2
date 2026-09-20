# WP-HK-06C Worker plan

Baseline SHA: `c6534fa42f6bdd18a5bff4c3fe23f3868af993e8`
Branch: `wp/hk-06c-deterministic-replay`
Worker state: ACTIVE

## Objective

Implement HK06C only: deterministic replay of the accepted HK06A authored-mutation journal against an accepted canonical base, with explicit replay compatibility and end-to-end audit consistency under HK06B snapshot/diff semantics. Do not redefine journal, snapshot, mutation or authored-state formats and do not add gameplay simulation, transport framing or Unity replay.

## PREDECESSOR_CONTRACT_CHECK

Direct accepted predecessor: `WP-HK-06B`.

- Reviewed candidate: `2b05e982c96e7ece08cca999075c183e75eee2fd`.
- Independent review: PASS (`#5258130916`).
- Exact-SHA validation: Actions `35473614054` GREEN; artifact `10594111979`.
- Implementation merge SHA: `28e2d0aadf63fe322eac636955e9223dc9249328`.
- Post-PASS DocSync SHA: `8488a361144abed9499ff76779df57c7ca42f025`.
- Current Worker baseline: `c6534fa42f6bdd18a5bff4c3fe23f3868af993e8` (later process-only H0 CAS/batching boundary update; it leaves HK06C replay scope unchanged).

Transitive accepted predecessor relevant to replay evidence: `WP-HK-06A`.

- Reviewed candidate: `4ed9a925791ae14b0b5c0d92625161504021542e`.
- Independent review: PASS (`#5257682288`).
- Merge SHA: `8089a52e8a7bbdde46e97705df6906c0d38d593a`.

Inherited guarantees consumed rather than re-proved:

1. HK06A owns `arkus.authoring.journal@1` and `arkus.authoring.journal-entry@1`. Each newly persisted ordinary canonical mutation appends one deterministic entry whose normalized accepted request is independently bound to request identity, base/result anchors and affected resources; failed/non-persisting work does not fabricate successful history.
2. HK04/HK05, as composed into HK06A, own ordinary canonical mutation execution: expected revision/hash reconciliation, keyed idempotency, complete candidate validation and atomic state/receipt/journal publication. Replay must invoke that accepted authority rather than reproduce mutation execution privately.
3. HK06B owns `arkus.authoring.snapshot@1`, canonical snapshot validation/import and semantic diff. A successful import is `CanonicalRebase`, establishes the imported state as a new local lineage root and deliberately leaves the HK06A mutation journal empty; it emits separate rebase evidence and is not an HK06A mutation.
4. Consequently an accepted replay base may be a state reconstructed from an HK06B snapshot, but snapshot import itself is never synthesized into the mutation journal. A journal produced after such a rebase begins at that imported anchor and replay consumes only the subsequent HK06A entries.
5. HK06A authored/live separation and HK06B snapshot/runtime boundary remain binding and are not re-proved here.
6. The post-HK06B process update deliberately keeps whole-world revision/hash CAS through H0 and defers richer stale-plan recovery/batching ergonomics to HK08. HK06C therefore does not introduce merge, per-resource locking or multi-agent semantics.

HK06C newly owns:

1. Replay interpretation and compatibility policy for accepted journal/snapshot versions without redefining their schemas.
2. Fail-closed validation of replay evidence as a sequence: accepted top-level version identifiers, exact contiguous sequence/order, anchor continuity, complete entry integrity and declared journal current anchor.
3. Deterministic execution of each accepted journal request through the existing canonical mutation authority, with per-step comparison against the persisted result anchor recorded by the source entry.
4. Replay publication/failure semantics that never report successful completion after an incompatible, malformed, missing/reordered, partially failing or divergent sequence.
5. Machine-readable audit output relating the replayed local entries/result to the original journal evidence while keeping replay orchestration distinct from runtime observations and from snapshot rebase history.
6. End-to-end clean-process proof: accepted base snapshot -> source mutation sequence -> journal -> clean imported base -> replay -> identical final canonical hash and empty HK06B semantic diff.

Predecessor reopen trigger: concrete effective evidence that an accepted HK06A entry can be schema/integrity-valid while not identifying the mutation actually persisted; that accepted HK04/HK05 canonical mutation execution can persist invalid/non-atomic authored state; or that an HK06B snapshot/import accepted as v1 reconstructs a different canonical base/hash or fabricates mutation history. No such evidence is present; replay-specific malformed/tampered inputs are HK06C-owned compatibility/integrity failures, not predecessor reopenings.

## Architecture decision

Replay is an orchestration over accepted canonical mutations, not a second mutation engine and not a snapshot rebase. The replay request supplies the accepted journal artifact and requires the bound target's current authored anchor to equal the journal base. Each source entry's normalized accepted request is executed through `ICanonicalWorldMutationCommitter`; the observed persisted anchor must equal that entry's recorded result before the next step is eligible.

To prevent a later replay failure from leaving a publicly advanced target while still preserving HK04/HK05 semantics, the portable authoring session stages the complete sequence in a fresh transactional session rooted at the current accepted base. Every staged step therefore executes through the existing canonical mutation authority and produces the normal HK06A local journal. Only after the whole sequence and final expected anchor agree does the outer session publish the staged aggregate under its existing gate, conditioned on the target still matching the captured base. A failure publishes no replayed state/history.

Replay compatibility is separately discoverable machine-readable policy. Initially only the accepted pair `journal@1` + `journal-entry@1` is executable; HK06B `snapshot@1` is supported as a way to establish the replay base, not embedded or reinterpreted by replay. Unknown format/version combinations fail closed with stable compatibility diagnostics and an explicit disposition. Future migrations require a separately reviewed path rather than implicit coercion.

## Expected implementation surfaces

- `src/Arkus.Game.Authoring/WorldReplay*.cs`: replay contract, accepted-journal parser/integrity checks, compatibility policy and atomic replay staging/publication integration.
- `src/Arkus.Harness.Runtime/WorldReplayBindings.cs` + canonical composition: discoverable compatibility/replay routes backed by the accepted authoring session.
- `tests/Arkus.Harness.Tests/Hk06C*.cs`: end-to-end identity/diff proof plus required causal negative-conformance tests.
- `scripts/hk06c-*` and `Docs/evidence/WP-HK-06C/*`: exact-SHA observation, proof matrix, residual risk, content-shape probe and Worker pre-review.

## Proof plan

Positive/effective evidence:

- nontrivial multi-entry source sequence replays from the same accepted base to exactly the same final revision/hash;
- replay-generated local journal is the normal HK06A journal and audit output binds source entry IDs to replay result anchors;
- source final snapshot versus replay final snapshot has an empty HK06B semantic diff;
- base established via accepted HK06B snapshot import works without importing source history;
- public discovery exposes replay plus compatibility schemas, with the policy consumable without source knowledge.

Causal negative-conformance classes:

- reordered entry sequence;
- missing entry / sequence gap;
- altered normalized request, identity, base or result data;
- wrong journal/base/final expected anchor;
- unsupported journal/entry/snapshot compatibility combination;
- execution path not using the canonical mutation authority;
- a later replay step that fails canonical validation/transaction semantics;
- false completion after any failed step;
- apparently successful step whose persisted result hash diverges from recorded evidence.

Independent/evaluated oracle:

Tests independently walk the public journal dictionaries, assert chain continuity and compare source/replayed snapshots through the accepted HK06B public semantic-diff capability. For the two material late-failure classes, tests independently recompute the accepted HK06A request/entry identity framing to forge evidence that passes replay's integrity parser but then (a) asks the canonical mutation authority to persist a semantically invalid later state or (b) records a false later result hash. Those controls force effective staged execution before failure and prove that neither canonical-validation bypass nor incremental/false-success publication can remain green.

## Proof/trust boundary

Trusted base remains the accepted foundational standard: pinned .NET/MSBuild/NuGet behavior, Git checkout/object semantics, normal single-process memory/locking and SHA-256 used according to contract. HK06C does not claim durable journal storage/recovery, cross-process concurrent agents, transport framing, gameplay replay/simulation, Unity replay, cloud persistence, journal authenticity/signatures or history merging across snapshot rebases.

Initial `PROOF_BUDGET_VERDICT`: `WITHIN_BUDGET`. Product machinery remains one replay parser/compatibility policy plus one staged canonical-execution path. Proof additions map directly to the WP-required negative classes and the required Potes/Liébana content-shape probe; no predecessor correctness proof is duplicated.