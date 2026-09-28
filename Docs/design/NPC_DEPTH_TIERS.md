# NPC depth tiers — planning and authoring rule

Status: **ACCEPTED product direction** via PR `#257`, Reviewer PASS `#5333313032`, merge `6bf2d6e74215be51e73d85f659a2a734752ea34c`, with current scale/routine interpretation amended by [`PORT_TOWN_SCALE_AMENDMENT.md`](../product/PORT_TOWN_SCALE_AMENDMENT.md). Counts are orientation for the final large port town, not H2, slice, performance or recruitment gates.

| Tier | Typical final planning range | Gameplay obligation | Persistent cost |
|---|---:|---|---|
| A — MAIN / SYSTEMIC | ~10–15 named leads | Multi-state character, authored history, relevant relationships/knowledge/memory, readable routine and consequences across scenes/days | Stable Arkus identity and reviewed durable facts; save/revisit validation |
| B — INTERACTIVE / REACTIVE | ~20–40 named/recurring characters | Shop, work, investigation, contextual dialogue and useful local routine; selective memories where a later encounter needs them | Arkus only for named consequential facts; otherwise local presentation |
| C — POPULATION / AMBIENT | Remainder of an initial ~80–120 visible people | Believable occupancy, motion and immediate response with reused variants; many may also carry a lightweight daily schedule | Usually only stable identity/schedule anchor when needed plus GC2/Unity local presentation; no permanent bespoke history required |

## Routine depth is a separate axis

A daily routine is **not** equivalent to Tier-A systemic depth. The product may target roughly **60–100 routine-bearing NPCs across the whole town** while keeping only ~10–15 at Tier A. A routine-bearing B/C NPC can use reusable schedule templates such as home → job/POI → leisure → home, with day/time variations and bounded authored exceptions, without receiving bespoke memories, relationships, quests or animation sets.

The ranges overlap intentionally: the 80–120 total includes A and B. Visible at once, routine-bearing identities, unique models, deeply systemic actors and runtime-active GameObjects are different budgets.

## District residency

Only the current district/neighbourhood and the continuity seam actually needed by the player must run graphical NPC realization: GameObjects, Animator, GC2 local behavior, NavMesh/pathing, look-at and immediate interaction. Off-screen people advance through a coarse schedule/state representation rather than hidden frame-by-frame movement. Arkus/Juego2 retains stable identity, current schedule segment or POI/district anchor, consequential deviations and durable facts; Unity/GC2 materializes the appropriate presentation when the district activates.

An NPC moving between districts can be represented as departure/transit/arrival while off-screen. If the player follows through a visible seam, the relevant actors remain realized for continuity. Unloading a district must not erase an authored consequence or make a recurring NPC lose their canonical identity.

`GC2-06` measures a small quiet/busy living area and the cost of materializing its active people. `DISTRICT-01` measures active-population, transition and authoring cost before later districts scale up. Neither WP should create a universal streaming framework without a demonstrated gameplay need.

Promote a C/B NPC's **persistent depth** only when a concrete authored encounter requires remembering, cross-system causality or richer identity. The promoted actor receives the bounded Arkus fact model required by that gameplay. A principal NPC can still use GC2 Behavior/Dialogue/Melee for immediate actions. The tier specifies depth of **meaning**, not whether the character is allowed to have a routine or which Unity component moves them.

Example budget per active block: one A witness, several B shop/work/investigation people and enough C pedestrians/locals to read as inhabited. Across the full town, many of those C identities may follow lightweight schedules and appear in different POIs at different times. Record only the authored reactions and schedule deviations the player can meaningfully notice later; avoid storing every glance as history. Character design, clothing, animation and density remain subject to ART and player-scale inspection.
