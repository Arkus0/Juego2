# WP-HK-06A — Provenance journal + authored/live boundary

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-05`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Make every accepted authored-world mutation leave a trustworthy machine-readable provenance record, while fixing the boundary between canonical authored state and future live/runtime observations before snapshot/replay work builds on it.

## Acceptance

- Canonical revisions, hashes and mutation history own **authored world state** only.
- Live simulation state (clock ticks, transient NPC positions, animation/physics state and equivalent runtime observations) does not silently advance authoring revisions, hashes or mutation history.
- Runtime observations, when later exposed, must identify the canonical authored base revision/hash they were produced from through a separately named contract; this WP does not implement a simulation engine.
- Every successfully committed mutation records request identity, protocol/tool version, base revision/hash, resulting revision/hash and affected resources.
- Provenance is emitted only for persisted mutations; a failed, rejected, dry-run or read-only request does not create a successful mutation entry.
- Provenance cannot claim success when persisted state/hash disagrees with the recorded result.
- Journal ordering and entry identity are deterministic and machine-consumable.
- Journal/provenance APIs are versioned and discoverable through the canonical contract rather than a side registry.
- The journal composes with the accepted HK04 transaction boundary and HK05 validation boundary; it does not reopen or duplicate their proofs without concrete contradictory evidence.
- A bounded content-shape probe models a scheduled H2 NPC and demonstrates that ordinary world ticks/runtime observations cannot create authored mutation journal entries or CAS churn.

## Required negative-conformance tests

RED→GREEN for: missing journal entry after an accepted mutation, entry emitted for rejected/dry-run/read-only work, wrong before/after revision or hash, wrong affected-resource set, provenance claiming a mutation that did not persist, and runtime/tick activity advancing authored revision/history.

## Forbidden scope

Semantic diff, snapshot export/import, journal replay, Git integration as source of truth, Unity serialization, GUI history browser, cloud persistence, gameplay clock/scheduler implementation, runtime AI or deterministic simulation engine.

## DoD

A nontrivial sequence of accepted micro-world edits produces a deterministic, discoverable provenance journal whose entries agree exactly with persisted authored state, while runtime-like observations remain outside authored revision/history semantics; independent PASS.
