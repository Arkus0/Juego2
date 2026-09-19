# WP-HK-06C — Deterministic journal replay + end-to-end audit consistency

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-06B`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Prove that accepted authored changes can be reproduced from an accepted base plus machine-readable journal evidence, yielding the same final canonical state/hash with explicit replay and compatibility semantics.

## Acceptance

- Journal replay from an accepted canonical base reconstructs the same final authored-world canonical hash as the original accepted mutation sequence.
- Replay consumes HK06A provenance/journal entries and HK06B canonical state/snapshot semantics rather than defining a second mutation or state model.
- Replay order is explicit and deterministic; reordered or missing entries cannot silently claim equivalence.
- Replay version/tool/protocol compatibility policy is explicit; unsupported entries fail with stable machine-readable diagnostics rather than guessed interpretation.
- Replay failure policy is explicit and cannot report successful completion when the resulting persisted state/hash disagrees with the recorded expected result.
- Replayed mutations still respect accepted canonical validation and transaction semantics; replay is not a privileged bypass around HK04/HK05.
- Replaying a sequence does not fabricate success provenance for steps that fail to persist, and audit output can relate replayed results to the original journal evidence without confusing runtime observation with authored history.
- A clean-process reference test starts from an accepted base/snapshot, replays a nontrivial sequence, and proves final semantic state/hash identity.
- Semantic diff between original and replayed final authored states is empty under HK06B semantics.
- Replay/journal evidence is headlessly consumable by tests and future tooling; no source-code knowledge is required to interpret success/failure.

## Required negative-conformance tests

RED→GREEN for: reordered journal entries, missing/altered entry data, wrong expected before/after hash, unsupported replay version, replay path bypassing canonical validation/transaction semantics, false success after partial/failing replay, and final-hash divergence despite apparently successful replay.

## Forbidden scope

Deterministic gameplay simulation, gameplay clock/scheduler implementation, Unity replay, network replication, GUI history browser, cloud persistence, Git history as the replay source of truth.

## DoD

From an accepted base plus accepted journal evidence, a clean H0 process can deterministically reconstruct the original final authored state/hash and prove audit consistency end to end; independent PASS.
