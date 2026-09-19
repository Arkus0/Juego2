# WP-HK-06 — Provenance, diff, snapshot + replay

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-05`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Make every accepted harness change auditable and reproducible from machine-readable evidence.

## Acceptance

- Canonical snapshots, hashes, revisions and mutation journals own **authored world state**. Live simulation state (clock ticks, transient NPC positions, animation/physics state and equivalent runtime observations) does not silently advance authoring revisions or participate in authoring CAS.
- Runtime observations, when later exposed, identify the canonical authored base revision/hash they were produced from and use a separately named contract; deterministic scenario simulation is not falsely claimed by authored-journal replay.
- Every committed mutation records request identity, protocol/tool version, base revision/hash, resulting revision/hash and affected resources.
- Semantic diff reports added/removed/changed authorable state without relying on raw serializer text diff.
- Canonical snapshot export/import is versioned and round-trips to the same semantic state/hash.
- Journal replay from an accepted base reconstructs the same final canonical hash.
- Replay order, failure policy and version compatibility are explicit.
- Provenance cannot claim success if persisted state/hash disagrees with the recorded result.
- Read-only requests do not pollute mutation history.
- Journal/snapshot evidence can be consumed headlessly by tests and future tooling.
- A bounded content-shape probe models a scheduled H2 NPC against this authored/live boundary and demonstrates that ordinary world ticks cannot create authoring journal entries or CAS churn.

## Required negative-conformance tests

RED→GREEN for: missing journal entry, wrong before/after hash, reordered replay, altered snapshot data, hidden state change absent from semantic diff, and provenance claiming a mutation that did not persist.

## Forbidden scope

Git integration as source of truth, Unity serialization, GUI history browser, cloud persistence, gameplay clock/scheduler implementation, runtime AI or deterministic simulation engine.

## DoD

A nontrivial sequence of micro-world edits can be exported, replayed in a clean process and proven hash-identical with auditable provenance; independent PASS.
