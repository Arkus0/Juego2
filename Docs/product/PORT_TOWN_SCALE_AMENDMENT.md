# Large port town scale and district-residency amendment

Status: **ACCEPTED product amendment**
Date: 2026-09-28
Accepted: PR `#262`, candidate `489312831ca0e337de58762c067ca60e5c20c874`, Reviewer PASS `#5333508771`, merge `65a2a727371833806f019af0a7af1767b3724534`.
Amends: [`URBAN_EXPANSION_DECISION.md`](URBAN_EXPANSION_DECISION.md), [`NPC_DEPTH_TIERS.md`](../design/NPC_DEPTH_TIERS.md) and the post-H2F-01A urban route.

Current product authority (upon acceptance of [the identity/nightlife amendment](PORT_TOWN_IDENTITY_NIGHTLIFE_AMENDMENT.md), PR `#266`): the historical `nightlife/industrial` planning option below is superseded as a description of the town's primary nightlife. `Villa Bruma` is retired and is not a canonical/final town name; the final proper name remains undecided. The current five-zone distribution is `N.CASCO` **nightlife-primary** and `N.TALLERES` **nightlife-secondary**. Where this record conflicts with the later amendment, that amendment governs current product work; the earlier wording remains historical evidence.

## Product scale decision

The final setting is a **large fictional port town / villa portuaria in northern Spain**, around the late 1990s or early 2000s. It should feel socially like a substantial town or comarca hub rather than an anonymous major city: repeated faces, family and business connections, rumours, local reputations and reasons for the same people to encounter each other across days.

The existing four-or-five-district structure remains useful and binding as a production/world-organization model. In fiction these can read as **barrios, zonas or neighbourhoods**, not necessarily formal municipal districts. Port/lonja, old town, market/commercial, residential and nightlife/industrial remain planning options until CITY fixes the exact geography. This amendment changes the diegetic scale/identity, not the compact-density ambition, gameplay breadth or roadmap shape.

`URBAN`, `CITY-URBAN` and `DISTRICT-*` identifiers are retained to avoid churn. In current product language, “district” means a bounded production/neighbourhood zone unless a later CITY decision explicitly gives it another administrative meaning.

## NPC routine decision

**Narrative depth and daily routine are separate axes.** An NPC does not need Tier-A memory, authored relationships or story state merely to have a believable daily schedule.

Initial whole-town planning orientation:

- approximately **80–120 visible/interchangeable person identities** remains a useful first total range;
- approximately **10–15 Tier A** characters carry the deepest persistent story/systemic state;
- approximately **20–40 Tier B** characters are recurring interactive/reactive people;
- target **roughly 60–100 routine-bearing NPCs across the whole town** if production proves affordable; many Tier C people may therefore have lightweight but real schedules;
- these are authoring targets, not simultaneous-render, performance or shipping gates.

A “routine-bearing” NPC may have time/day → district/POI/activity transitions, recurring work/leisure/home anchors and bounded exceptions. That does **not** imply bespoke dialogue trees, relationship graphs, memories or unique animations for every person.

## District residency and off-screen rule

The town is not required to render or run full GameObject AI for every NPC at once.

1. The **active district/neighbourhood** (plus only the seam/buffer actually needed for continuity) owns graphical realization: bodies, Animator, local GC2 behavior, NavMesh/pathing, look-at, interaction and immediate reactions.
2. NPCs outside the active residency set remain **abstract**. Arkus/Juego2 retains the small amount of canonical information needed to answer where they should be and what durable facts changed: identity, current schedule segment, district/POI anchor, significant deviations and consequential state.
3. Off-screen routines should advance by clock/schedule transitions or coarse events, not by hidden frame-by-frame pathfinding. When a district becomes active, Unity/GC2 materializes the NPC from current canonical schedule/state.
4. Cross-district travel may be represented as departure/transit/arrival state while off-screen. If the player is present and follows the actor, the relevant seam can remain graphically realized instead of teleporting through a visible boundary.
5. A district unload must not erase an authored consequence. On revisit, durable Arkus state and current schedule determine presentation.

This is a **scaling rule, not authorization for a new universal streaming framework**. `GC2-05/06` and `DISTRICT-01` should prove the minimum residency/materialization behavior demanded by actual gameplay. Only measured needs may justify broader infrastructure.

## Feasibility rule

The likely bottleneck for 60–100 scheduled townspeople is **authoring and QA**, not storing their schedules or keeping all of them graphically alive. Reuse schedule templates, occupations, POI grammars and generated variants where appropriate, while preserving stable identity for named people. `GC2-06` measures one living block/district context; `DISTRICT-01` measures active population, transition and authoring cost before later neighbourhoods scale up.

Do not convert the 60–100 routine target into a requirement that 60–100 characters be simultaneously loaded, uniquely modeled or deeply systemic.

## What does not change

- **GC2 ejecuta. Arkus recuerda y conecta.**
- H2F-02 → H2F-03 → H2F-GATE remains the immediate foundation route.
- ART-01, retained inland pilot work, H2 keeper proof and the later `CITY-URBAN` / `ART-URBAN` / `GC2` chain remain useful.
- The first slice still targets 20–30 minutes of daily life, investigation, activity, chase, confrontation/fight and a persistent changed return visit.
- The game may still grow to four or five compact neighbourhood/district zones; this amendment does not demand all of them before the slice.
