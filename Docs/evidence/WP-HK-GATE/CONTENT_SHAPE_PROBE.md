# WP-HK-GATE representative content-shape probe

CONTENT_SHAPE_PROBE_VERDICT: PASS
APPROVED_PRODUCT_SOURCE: Docs/art/VISUAL_BIBLE.md v0.1.3
EXECUTABLE_PROBE: HkGateReadinessTests.PublicReferenceClientCompletesDeterministicGateScenarioWithoutPrivateProductCalls
UNRESOLVED_IN_SCOPE_FINDINGS: 0

## Source and bounded slice

`Docs/art/VISUAL_BIBLE.md` v0.1.3 describes the current Juego2 target as a fictional Potes/Liébana valley market town, with plaza/market grain, bar/shop life, workshops and modular reuse. `WP-HK-08B` already used that document as its approved bounded product source; GATE reuses the same approved content shape rather than inventing a new abstract fixture.

The deterministic GATE client creates, through the public reference transport only:

- `gate.place.plaza` — a generic canonical place/root identity;
- `gate.building.market` — a separately addressable market resource contained by the plaza;
- `gate.npc.ana` — a separately addressable NPC-shaped resource contained by the plaza;
- a typed `works.at` cross-reference from the NPC to the market;
- an opaque `future.gate.social` extension attached to the NPC, with an explicit dependency on the market.

These names and `fixture.gate.*` type IDs are test vocabulary only. They do not add plaza, market, NPC, transform, art, gameplay, Unity or simulation semantics to H0 canonical contracts.

## Boundaries exercised

The public scenario proves the bounded slice can be:

1. discovered from public capability/schema metadata;
2. planned, dry-run and atomically authored as one coherent multi-resource request;
3. reconstructed through `world.object.query`, `world.object.get`, `world.reference.query`, `world.extension.query` and `world.extension.read`;
4. validated through `world.validation.current`;
5. rejected with structured diagnostics when intentionally invalid and then repaired through the same public mutation route;
6. recovered after a same-lineage stale write using HK08B affected-resource descriptors without a full-world reload;
7. extended by a representative 96-operation coherent HK08A batch inside the accepted HK09B envelope;
8. exported, journaled, semantically diffed and replayed in a fresh process to the identical final canonical hash.

## Findings

- **Representability — GREEN.** The approved market-town slice fits H0's generic object/containment/reference/opaque-extension grammar.
- **Identity/granularity — GREEN.** Plaza, market and NPC-shaped resources remain independently addressable without introducing game-engine object identity.
- **Inspection — GREEN.** Objects, relations and opaque extension state can be reconstructed through public bounded reads; the extension is not hidden behind implementation knowledge.
- **Validation/mutation — GREEN.** The slice traverses ordinary plan/dry-run/apply and ordinary validation; invalid state does not commit.
- **Recovery — GREEN.** Same-lineage stale recovery remains affected-resource bounded and preserves the original coherent intent as one ordinary retry.
- **Diff/provenance/replay — GREEN.** Accepted mutations remain explainable by journal + semantic diff and replay to the same hash after restart.
- **Engine bridge — CLEAN.** No Unity/editor/transform/asset semantics are required by this H0 content shape.

## Classification

No in-scope blocker or concrete predecessor reopen condition was found by this probe. Exact Unity realization, transforms, assets, schedules, simulation and gameplay remain downstream Engine Bridge / gameplay work. Per-resource locking, automatic merge and multi-agent coordination remain outside the GATE claim unless future measured evidence requires them.
