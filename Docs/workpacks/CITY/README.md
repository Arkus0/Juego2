# CITY track — Keeper City production-space programme

Status: **ACTIVE / NON-FOUNDATIONAL**  
Version: 2.0 — 2026-09-20  
Repository: `Arkus0/Juego2`

Accepted progress: `WP-CITY-00` is **COMPLETE**. It passed independent review on frozen candidate `f5c684461b525841487158d873a9b135008df3ab` (review `#5261672961`) and merged via PR `#65` as `69bbba2603e67cb233a3e1cc36a4173bed41cfc4`.

## Purpose

Design one coherent keeper-city pipeline from accepted macro geography to streets, buildings, interiors, layered discovery and the first retained playable district.

CITY intentionally remains **one operational track**. Macro urbanism, place grammar and scenario production are distinct responsibilities, but splitting them into interleaved CITY/SCENE programmes added coordination cost without creating a real authority boundary.

Core rule:

> **The demo is the first finished piece of the final city, and every CITY workpack reduces uncertainty needed to build that piece once.**

## Accepted predecessor

`WP-CITY-00` owns the accepted spatial constitution: landmasses, river/arroyos, district relationships, port placement, crossing strategy, scale envelope and expansion logic. Downstream WPs consume `Docs/production/CITY_SPATIAL_CONSTITUTION.md` and its reviewed evidence; they do not silently redraw that city.

## Execution chain

```text
CITY-00 ✅  Spatial constitution + scale envelope
   ↓
CITY-01    Place grammar + spatial depth tiers
   ↓
CITY-02    Mobility + access topology
   ↓
CITY-03    Systemic locations + reactive-density programme
   ↓
CITY-04    Streets, parcels + reusable building families
   ↓
CITY-05    Interiors + discovery layers
   ↓
CITY-06    Retained product seed + exact scenario specification
   ↓
CITY-07    LOCAL Unity greybox + traversal validation
   ↓
CITY-08    LOCAL keeper realization + Arkus authoring proof
```

`CITY-01..06` are REMOTE planning work and may run before `WP-HK-GATE` because they create documents/contracts only. `CITY-07` and `CITY-08` are dormant until GATE has passed and their relevant Unity/bridge/catalogue prerequisites are accepted.

## Two orthogonal classifications

CITY uses two separate axes so production scope does not get confused with systemic importance.

**Spatial production depth (CITY-01):**
- `S0` scenic envelope / inaccessible context;
- `S1` authored shell or façade only;
- `S2` shallow playable space with bounded interaction;
- `S3` deep playable place with multiple authored spaces/thresholds/anchors;
- `S4` hero layered place with multiple meaningful access/discovery opportunities.

**Systemic importance (CITY-03):**
- `A` primary systemic anchor;
- `B` supporting systemic/playable location;
- `C` ambient urban fabric;
- `D` scenic context.

The axes are deliberately independent. A visually impressive building may be `S1/C`; an ordinary home may be `S3/A` if persistent actors and consequences depend on it.

## Layered-city rule

The city must have more depth than its street plan without pretending every building is a dungeon. Later CITY work may use, where appropriate:

- public surface;
- private/semi-private space;
- service/back-of-house routes;
- vertical space;
- social/temporal discovery;
- institutional/historical discovery;
- rare extraordinary/cultural strands.

Important discoveries may be authored, systemic or hybrid. Martial arts/kung-fu/Hong Kong or Chinese-cinema material may form a rare identity-bearing strand, but the town must remain coherent if a player never discovers it.

## Track-wide invariants

1. **Retained-first:** serious geometry begins only where the urban structure is expected to survive into the game.
2. **One accepted geography:** downstream planning cannot contradict CITY-00 landmass/crossing truth.
3. **Reactive density over acreage:** empty expansion is worse than a smaller city with meaningful places.
4. **Function before decoration:** districts and places exist because people live, work, move, socialise, govern, exchange or discover there.
5. **Quiet is content:** not every street or place produces incidents.
6. **Depth is selective:** not every façade opens; not every interior is systemic; not every secret is a hidden room.
7. **Reuse compounds:** streets/buildings/compositions should become cheaper to author as reviewed families accumulate.
8. **Player absence matters:** locations/routes must make sense when actors use them off-screen.
9. **Discovery has truthful causes:** information/access is not granted merely because the player approached a marker.
10. **No pre-GATE production:** planning cannot smuggle Unity construction, asset adoption or runtime semantics ahead of accepted gates.
11. **Bridge neutrality:** CITY consumes accepted Arkus/Unity capabilities; it does not invent bridge semantics to make a scene plan pass.

## Current next workpack

`WP-CITY-01 — Place grammar + spatial depth tiers`.
