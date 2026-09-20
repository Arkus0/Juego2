# CITY track — Keeper City spatial programme

Status: **PLANNED / NON-FOUNDATIONAL**  
Date: 2026-09-20  
Repository: `Arkus0/Juego2`  
Scope: product-space preproduction for the fictional Potes/Liébana keeper city

## Purpose

Design the spatial skeleton of the shipping city early enough that the first playable/demo district can remain part of the final game.

The CITY track does **not** build a disposable test town and does not compete with the Harness Kernel. It converts the current Production Blueprint into independently reviewable spatial decisions before expensive Unity geometry exists.

Core rule:

> **The demo must be the first finished piece of the final city, not a temporary map that teaches us what to build later.**

## Product target

The desired place is a compact but credible small regional city/town: substantially more than four streets, far smaller than an open-world metropolis, dense enough that most traversed space has social/material/gameplay meaning, and large enough that actors can disappear into another district and continue life outside the player's view.

Permanent spatial ingredients to test and preserve:

- river as a genuine movement/land-use boundary;
- old bridge plus later additional crossings where justified;
- historic old quarter with irregular lanes and verticality;
- civic/commercial core and town hall;
- residential areas with more than one social character;
- workshops/service/peripheral urban edge;
- **river port / fluvial-port district** with wharf/storage/work/logistics identity;
- roads and valley routes that connect outward;
- rural edge, paths, slopes, huertas/green transition;
- quiet spaces as well as busy spaces;
- expansion seams that do not require rebuilding the first district.

The river port is not required to imply a giant shipping harbour. Its scale must fit the fictional valley city. Its value is systemic: arrivals, goods, work, schedules, visitors, storage, fishing/waterfront activity and municipal decisions can intersect there.

## Scale hypothesis — to test, not silently freeze

`WP-CITY-00` starts from this **working hypothesis** and must challenge it before adoption:

- roughly **0.8–1.2 km² of dense urban playable fabric** as a plausible final-core band;
- roughly **1.5–2.5 km² total playable envelope** if river, roads, slopes and rural edge are included;
- approximately **12–18 minutes on foot** for a representative long cross-city route before shortcuts/fast travel;
- **6–8 recognisable districts/zone families** rather than one undifferentiated town;
- first retained product seed roughly **0.10–0.15 km²** only if traversal/density tests support it.

These numbers are planning hypotheses, not production commitments or acceptance criteria. CITY-00 may shrink or reshape them if causal density, content cost or travel quality argues otherwise.

## Dependency story

```text
CITY-00  Spatial constitution + scale envelope
   ↓
CITY-01  Mobility, district graph + walk-time topology
   ↓
CITY-02  Systemic locations/interiors + reactive-density programme
   ↓
CITY-03  Retained product seed + expansion seams
   ↓
CITY-04  LOCAL Unity greybox/blockout validation
```

`CITY-00..03` are REMOTE planning work and may run before GATE because they produce documents only. `CITY-04` is dormant until GATE + relevant Unity bridge readiness.

## Relationship to other programmes

- `WP-ART-00` owns visual/setting direction; CITY consumes it and must not silently restyle the game.
- `Docs/production/PRODUCTION_BLUEPRINT.md` is the main prior product-seed input; CITY may sharpen or explicitly propose amendments to it.
- Living World `PA-01..PA-14` define behavioural/system needs; CITY provides spatial affordances and test situations but does not pre-accept PA findings.
- H0/H1 canonical/bridge architecture remains authoritative for implementation boundaries.
- The old `Arkus0/Juego` repository is reference only.

## Track-wide invariants

1. **Retained-first:** serious geometry starts only where we expect to keep the urban structure.
2. **Loops over funnels:** the plaza may be important but must not be the mandatory route for every journey.
3. **Function before decoration:** each core district exists because people live, work, move, socialise, govern or exchange there.
4. **Reactive density over acreage:** empty expansion is worse than a smaller city with meaningful locations.
5. **Quiet is content too:** ordinary residential/rural/river spaces are required so the town does not feel like permanent procedural theatre.
6. **Macro ↔ micro:** municipal choices must be able to alter routes/access/capacity/services in spaces the player later experiences on foot.
7. **Player absence matters:** actors need routes and destinations that plausibly continue without the player.
8. **Expansion without demolition:** the seed must expose clean seams toward port, residential, commercial/peripheral and rural growth.
9. **No fake scale:** inaccessible backdrop may sell silhouette, but it must not substitute for promised playable districts.
10. **No pre-GATE production:** planning cannot be used to sneak scene construction or asset adoption past `WP-HK-GATE`.

## Completion condition

The CITY planning programme is ready for local blockout only when CITY-00..03 have independent PASS verdicts and together answer:

> What city are we building, how is it connected, what meaningful places must it contain, and exactly which first piece can we build once and keep?
