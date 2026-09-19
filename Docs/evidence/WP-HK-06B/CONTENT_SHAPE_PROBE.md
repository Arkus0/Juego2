# WP-HK-06B representative content-shape probe

## Approved product source

This bounded probe uses `Docs/art/SETTING.md`: the approved H2 hero slice is a fictional Potes/Liébana valley town with a plaza, streets, bar/shop and a social core. HK06B does not promote transforms, schedules, simulation, Unity scene state or gameplay AI into the canonical authored schema.

## Bounded scenario

`Hk06BPortabilityTests.PotesContentShapeRoundTripsAuthoredStateWhileRuntimeSurrogateRemainsTransient` models only enough content to exercise semantic diff and snapshot portability:

- authored world `world.potes` at revision 7;
- `place.plaza`, `building.bar`, `building.shop` and `npc.ana` identities;
- containment under the plaza and `npc.ana` reference `works.at -> building.bar`;
- one opaque `future.social@1@global` authored extension;
- one accepted authored shop edit through `authoring.change.apply@1.0`;
- a bounded test-owned runtime-step surrogate that changes outside `WorldState`;
- export of the resulting snapshot followed by import into a clean authored-state holder.

## Candidate surfaces exercised

1. Export the initial Potes-shaped authored world through `authoring.snapshot.export@1.0`.
2. Advance only the test-owned transient runtime step twice and export again; canonical authored bytes remain identical.
3. Apply one canonical shop edit and export the new snapshot.
4. Compare before/after with `authoring.diff.compare@1.0`; the only changed resource is `world.object:building.shop`.
5. Import the resulting snapshot into a clean holder through `authoring.snapshot.import@1.0`.
6. Require the imported canonical hash to equal the source hash and the local HK06A journal to remain empty.

## Findings

- **Representability:** PASS. The accepted generic world/object/reference/extension model represents the bounded plaza/building/NPC slice without a new gameplay schema.
- **Identity/granularity:** PASS. Diff identifies the shop by accepted canonical object identity rather than serializer line position.
- **Semantic diff:** PASS. One authored shop change produces exactly one resource change; transient runtime activity produces none.
- **Snapshot portability:** PASS. The nontrivial authored world reconstructs to the identical canonical hash in a clean holder.
- **Authored/live boundary:** PASS. Runtime-only surrogate state does not enter the snapshot payload.
- **History semantics:** PASS. Snapshot import starts a new local lineage at the imported anchor and does not fabricate source mutation history.

## Classification

- In-scope blockers: none after Worker pre-review repair.
- Predecessor reopen conditions: none observed; accepted HK02/HK02A state/identity, HK04 mutation authority, HK05 validation and HK06A journal truthfulness remain applicable.
- Named future/residual decisions: replay/compatibility belongs to HK06C; transport framing belongs to HK07A; durable persistence and gameplay/runtime schemas remain later work.
- Out-of-boundary observations: transforms, schedules, clock semantics, movement/physics/animation, Unity scene serialization, visual fidelity and gameplay AI.

This probe is representative only. Semantic completeness is established separately by the independent test-owned projections and causal negative-conformance controls.
