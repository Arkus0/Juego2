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
- The **journal/provenance entry schema and its version identifier are owned by HK06A** and are discoverable through the canonical contract. Later HK06C may define replay compatibility across accepted versions, but does not redefine the journal format/schema.
- The journal composes with the accepted HK04 transaction boundary and HK05 validation boundary; those guarantees remain consumed, not re-proved, unless concrete contradictory evidence appears.

## Required authored/live boundary oracle

HK06A does not implement a gameplay clock, scheduler, AI runtime or deterministic simulation engine, so the authored/live negative claim must not depend on an invented fake gameplay subsystem.

The bounded content-shape proof uses a **test-owned runtime-observation surrogate** representing a scheduled H2 NPC observation:

- the surrogate is explicitly outside canonical `WorldState`, the authored mutation planner/session and journal-append authority;
- it is stamped with the authored base revision/hash it observes;
- its transient observation fields can advance across synthetic ticks/steps without invoking an authored mutation;
- the oracle captures canonical authored state/hash/revision and journal contents before and after those transient changes and requires them to remain unchanged while the surrogate itself changes;
- the proof establishes the storage/authority boundary only. It does **not** claim deterministic gameplay simulation or validate future NPC movement semantics.

A surrogate implemented by sending authored no-op mutations is invalid evidence because it would exercise the wrong authority path.

## Required negative-conformance tests

RED→GREEN for: missing journal entry after an accepted mutation, entry emitted for rejected/dry-run/read-only work, wrong before/after revision or hash, wrong affected-resource set, provenance claiming a mutation that did not persist, runtime-observation surrogate activity advancing authored revision/hash/history, and a runtime-observation path acquiring journal/commit authority.

## Forbidden scope

Semantic diff, snapshot export/import, journal replay, Git integration as source of truth, Unity serialization, GUI history browser, cloud persistence, gameplay clock/scheduler implementation, runtime AI or deterministic simulation engine.

## DoD

A nontrivial sequence of accepted micro-world edits produces a deterministic, discoverable and versioned provenance journal whose entries agree exactly with persisted authored state, while a bounded runtime-observation surrogate can change independently without authored revision/hash/history churn; independent PASS.
