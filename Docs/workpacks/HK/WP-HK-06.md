# WP-HK-06 — Provenance, diff, snapshot + replay

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-05`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Make every accepted harness change auditable and reproducible from machine-readable evidence.

## Acceptance

- Every committed mutation records request identity, protocol/tool version, base revision/hash, resulting revision/hash and affected resources.
- Semantic diff reports added/removed/changed authorable state without relying on raw serializer text diff.
- Canonical snapshot export/import is versioned and round-trips to the same semantic state/hash.
- Journal replay from an accepted base reconstructs the same final canonical hash.
- Replay order, failure policy and version compatibility are explicit.
- Provenance cannot claim success if persisted state/hash disagrees with the recorded result.
- Read-only requests do not pollute mutation history.
- Journal/snapshot evidence can be consumed headlessly by tests and future tooling.

## Required self-attacks

RED→GREEN for: missing journal entry, wrong before/after hash, reordered replay, tampered snapshot, hidden state change absent from semantic diff, and provenance claiming a mutation that did not persist.

## Forbidden scope

Git integration as source of truth, Unity serialization, GUI history browser, cloud persistence.

## DoD

A nontrivial sequence of micro-world edits can be exported, replayed in a clean process and proven hash-identical with auditable provenance; independent PASS.
