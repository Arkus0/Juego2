# WP-HK-09B persistence/interruption proof

PERSISTENCE_INTERRUPTION_VERDICT: PASS

## Accepted durability claim

HK09B proves only the persistence mechanism H0 actually owns: an in-process, process-local checkpoint/session aggregate with fail-closed publication. The machine-readable resource envelope states:

- `durabilityLevel=process-local-checkpoint`;
- `publicationBoundary=validate-stage-aggregate-publish`;
- `powerLossDurabilityClaimed=false`.

Accordingly, the proof attacks the real validate/stage/publish seam. It does not invent a WAL, fsync protocol, crash-recovery subsystem, distributed transaction, multi-process writer protocol or power-loss guarantee.

## Mutation publication

`TransactionalWorldAuthoringSession.Apply` parses, validates and plans first. Under the accepted HK04 commit lock it builds the next provenance entry, receipt dictionary and journal, then calls `TryBeginPublication("canonical-mutation")`. Only after that succeeds does one `AuthoringState` reference assignment publish candidate state + receipts + journal; the budget is then marked committed.

`ExpiredAndInterruptedMutationBudgetsPublishNeitherStateReceiptNorJournal` proves both an already-expired execution budget and a publication interruption leave revision/hash/journal at the old authoritative aggregate. A normal retry then commits once and the same idempotency key subsequently replays without an extra publication.

## Snapshot-import publication

HK09B removes the former seam where imported state could become authoritative before the idempotency receipt/rebase evidence was published. `PortableWorldAuthoringSession` now owns an immutable `PortableSessionState` containing:

- the transactional inner session/current world;
- the fresh local mutation lineage;
- snapshot-import receipts;
- receipt result containing truthful rebase evidence.

Import validates the request/snapshot and expected revision+hash, builds the staged inner session/result/receipt map, then calls `TryBeginPublication("snapshot-import")`. One `_session = nextSession` swap publishes all of those together.

`InvalidOversizedAndInterruptedImportCannotReplaceStateHistoryOrEvidence` starts from a target with accepted local history, then proves unsupported version, oversized state and forced publication interruption cannot replace the target anchor or accepted journal. A later successful retry publishes the new lineage atomically and a repeated identical import returns the stored evidence as an idempotent replay.

## Replay publication

Replay runs all source HK06A entries against a separate staged `TransactionalWorldAuthoringSession`, verifies every resulting anchor, reparses the staged journal and audits source/replayed entry identities. Only after complete replay/audit does it call `TryBeginPublication("journal-replay")`; one new `PortableSessionState` reference then publishes staged state + HK04 receipts + HK06A journal while retaining the current snapshot-import receipt set.

`InterruptedReplayCannotPublishStagedStateOrProvenance` denies that final publication after staging. The target remains at its original revision/hash with an empty local journal. A normal retry then publishes the complete replay result once.

## Execution/cancellation semantics

`InvocationResourceBudget` uses a monotonic `Stopwatch` deadline and cancellation token. Writers check the budget at the authoritative publication boundary; the dispatcher also checks before returning when publication has not committed. Once an authoritative publication is marked committed, a later cancellation cannot cause the public result to falsely claim that no commit happened.

This is cooperative bounded execution, not arbitrary thread/process preemption. Long staging work may run until the next check, but it cannot cross the owned publication boundary after the budget has expired.

## Trust boundary and residual

Trusted infrastructure is the normal documented .NET memory/lock/reference-assignment behavior and process execution selected by the repository's pinned toolchain. A process/OS/power failure outside that contract is not claimed recoverable by H0. If future persistence claims survive those modes, a later WP must introduce and prove the required storage architecture rather than retroactively reading such a promise into HK09B.

Implementation checkpoint `e370606e4278da3e08602a3167c1cb6513bea93d`: focused HK09B 9/9 and full regression 205/205 GREEN in Actions run `35521120979`.
