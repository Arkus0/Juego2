# WP-HK-06C — Deterministic journal replay + end-to-end audit consistency

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-06B`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Prove that accepted authored changes can be reproduced from an accepted base plus machine-readable journal evidence, yielding the same final canonical state/hash with explicit replay and compatibility semantics.

## Acceptance

- Journal replay from an accepted canonical base reconstructs the same final authored-world canonical hash as the original accepted mutation sequence.
- Replay consumes the accepted HK06A journal/provenance schema and HK06B canonical state/snapshot/diff semantics rather than defining a second mutation, journal or state model.
- Replay order is explicit and deterministic; reordered or missing entries cannot silently claim equivalence.
- **HK06C owns replay compatibility behaviour only**: for each accepted HK06A journal version (and any HK06B snapshot version used as a replay base), the policy states whether replay is supported, requires an explicit migration path, or fails closed with stable machine-readable diagnostics. HK06C does not redefine those artifact formats.
- Replay failure policy is explicit and cannot report successful completion when the replay result disagrees with the expected persisted state/hash recorded by the accepted journal evidence.
- Replayed mutations still respect accepted canonical validation and transaction semantics; replay is not a privileged bypass around HK04/HK05.
- Replaying a sequence does not fabricate success provenance for steps that fail to persist, and audit output can relate replayed results to the original journal evidence without confusing runtime observation with authored history.
- A clean-process reference test starts from an accepted base/snapshot, replays a nontrivial sequence, and proves final semantic state/hash identity.
- Semantic diff between original and replayed final authored states is empty under HK06B semantics.
- Replay/journal evidence is headlessly consumable by tests and future tooling; no source-code knowledge is required to interpret success/failure.
- **Accepted HK06A journal truthfulness/authored-live boundary and HK06B snapshot/diff semantics remain consumed, not re-proved.** HK06C may challenge tampered or incompatible replay inputs, but it must not reopen accepted predecessor guarantees merely to accumulate proof volume unless concrete contradictory evidence appears.

## Required negative-conformance tests

RED→GREEN for: reordered journal entries, missing/altered entry data, wrong expected before/after hash supplied to replay, unsupported replay-version combination, replay path bypassing canonical validation/transaction semantics, false success after partial/failing replay, and final-hash divergence despite apparently successful replay.

## Forbidden scope

Redefining journal/snapshot artifact schemas, deterministic gameplay simulation, gameplay clock/scheduler implementation, Unity replay, network replication, GUI history browser, cloud persistence, Git history as the replay source of truth.

## DoD

From an accepted base plus accepted journal evidence, a clean H0 process can deterministically reconstruct the original final authored state/hash and prove audit consistency end to end under an explicit replay-compatibility policy; independent PASS.
