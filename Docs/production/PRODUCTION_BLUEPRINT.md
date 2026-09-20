# Juego2 Production Blueprint

Version: 0.3 — 2026-09-20  
Status: **NON-BINDING, owner-reviewed production proposal.** This document creates no H0 acceptance criterion, reopens no accepted H0 guarantee, starts no gameplay work and does not modify the `WP-HK-GATE` precondition. It is planning input for the roadmap author after GATE.

Revision 0.3 reconciles this document with `Docs/production/CITY_SPATIAL_CONSTITUTION.md`, produced by `WP-CITY-00`. Where the two disagreed about the shape of the town, the constitution now owns the topology, the district families and the scale band, and this document keeps the production reasoning built on top of them. Nothing else in this revision changes, and this remains non-binding planning input.

## 0. Decisions captured by this revision

This revision turns the first draft from a good test-town plan into a **product-seed plan**. Five planning decisions guide everything below:

1. **The demo is the first piece of the game, not a disposable prototype.** The first serious town geometry should be extendable into the shipping town rather than replaced after proving systems.
2. **H1 should prove the Unity bridge with a small slice of real Quaternius-derived assets, not only primitives.** This deliberately differs from the current `ROADMAP.md`, which places Quaternius content in H2. Because this file is non-binding, it records the desired future H1/H2 boundary; the roadmap must be amended explicitly after GATE before implementation.
3. **Living World Core comes before combat.** A social consequence that changes another NPC's later behaviour is a more important early proof of Juego2 than a fight. Directed drama and combat build on the living town, not the reverse.
4. **Asset and prefab discovery must be machine-readable, but the catalogue is not declared canonical `WorldState` by fiat.** H1 must decide the reviewed catalogue authority and projection boundary. The likely shape is bridge-owned catalogue data projected through Arkus scoped capabilities, with canonical world objects referring to stable catalogue/composition IDs.
5. **Authored project state and live game/save state remain separate authorities.** Multi-session authoring needs durable project checkpoints; gameplay needs its own runtime/save contract. Neither is allowed to smuggle live simulation into the accepted HK06 authored journal/replay semantics.

Nothing above changes H0. The next dependency-valid H0 work remains `WP-HK-07A`.

---

# 1. Town plan — a product seed, not a test diagram

## 1.1 Design target

The setting remains a **fictional Potes / Liébana valley market town**, not a 1:1 reconstruction of real Potes. The useful morphological lesson from Potes is not a specific street map; it is that water, bridges, narrow historic streets, slope, compact neighbourhoods and a commercial centre produce the town's spatial identity.

Public tourism references describe Potes as a town where the Deva and Quiviesa and smaller watercourses condition the disposition of neighbourhoods along the banks, with numerous bridges, narrow historic streets and houses perched over the Quiviesa. Those are **reference principles**, not content to copy.

The town therefore uses four permanent spatial ideas:

- a river/stream corridor that actually shapes movement;
- at least two crossings eventually, with **one old bridge in the first product seed**;
- an irregular old quarter and stepped residential lanes rather than a radial test layout;
- a commercial/civic centre that is important because routes naturally meet there, not because every route is forced through one debugging plaza.

The first product seed should already contain enough of those ideas that a screenshot reads as our game.

## 1.2 Semantic topology

The town's topology is no longer defined here. It is owned by
`Docs/production/CITY_SPATIAL_CONSTITUTION.md` §2.2, which selected a **confluence-wedge** city after
comparing three materially different alternatives.

What the earlier draft of this section got right survives unchanged:

> The town needs **loops**. The plaza is important, but it must never be the only possible route
> between every pair of places.

What it lacked was a river port, a second residential character, and any reason the river had to be
crossed. The selected constitution supplies all three: two watercourses with different roles — a
tributary stream crossed casually and often, a valley river crossed rarely and deliberately — and a
working fluvial landing downstream of the confluence.

The actor examples that motivated the original diagram all still hold, now on the new graph:

- Antonio descends from the Barrio Alto and reaches the workshops along the Ribera without traversing the whole commercial street.
- Ana takes a short lane from her home to the bar in the casco.
- Pilar chooses the riverside paseo rather than the market terrace.
- Javier's bicycle prefers the Calle Mayor and the Puente del Mercado, avoiding the stepped lanes.
- Tomás arrives from the huertas along the vega path and reaches the market directly.
- Bernardo arrives by bus at the Entrada — which now sits beside the port, so outsiders enter through the working edge rather than at an abstract road edge.

NPC schedules therefore create believable traffic because the **town affords different routes**, not because a script funnels actors past the player.

## 1.3 Zones

| Zone | Product character | Core POIs | Purpose |
|---|---|---|---|
| `zone.casco` / old-quarter equivalent | tight stone lanes on the confluence tip, level changes, houses over the stream | bar alley, old residences, small courtyard, mirador | strongest town identity; intimate social encounters |
| `zone.plaza_mercado` | civic/commercial meeting space on the first terrace | market, fountain, benches, town hall frontage | visible convergence without becoming the only hub |
| `zone.calle_comercial` | main everyday spine along the terrace | shop, bakery, pharmacy, residential portals | interaction, opening hours, everyday pedestrian flow |
| `zone.barrio_alto` | stepped residential lanes on the slope | homes, wash place/patio, viewpoint | schedules, vertical navigation, off-screen transitions |
| `zone.ensanche` | flat, regular, newer residential across the stream | homes with gardens, corner shop, small square | a second residential social character; cheapest large expansion |
| `zone.ribera` | work edge between the terrace and the river | workshop, storage, river walk, towpath | work routines, alternate low route |
| `zone.puerto` | fluvial landing downstream of the confluence | quay, warehouses, weighbridge, boatyard, fonda | arrivals, goods, work shifts, outsiders, municipal concessions |
| `zone.entrada` | road/bus arrival, adjacent to the port | bus stop, road junction, depot | arrivals, departures, outsider traffic, expansion seam |
| `zone.vega` | huertas, paths and terraces upstream | huerta plots, mill, paths to ermita/cemetery | rural edge, quiet, seasonal work, market supply |

Names are placeholders; identifiers are not frozen by this document. District identity and intensity
are owned by `CITY_SPATIAL_CONSTITUTION.md` §2.3; the location and interior programme belongs to
`WP-CITY-02`.

## 1.4 The first buildable product seed

The first serious H2 town build should be **small enough to finish and good enough to keep**:

- one side of `zone.plaza_mercado`;
- one old stone bridge;
- approximately 60–100 m of the commercial/old-quarter connection;
- the bar exterior and one bar interior;
- one ascending lane toward Barrio Alto;
- one stream crossing, so route choice exists inside the seed itself;
- 6–8 authored building frontages;
- a short visible river/stream strip and riverside edge;
- the first metres of the descent toward the port, as a visible seam rather than a built district;
- distant green slopes / rock silhouettes.

Laid out at the scale anchors in §1.5 this occupies roughly **0.03–0.06 km²**. That band replaces the
0.10–0.15 km² starting hypothesis, which `WP-CITY-00` falsified against both the corrected city size
and this very element list (see `Docs/evidence/WP-CITY-00/SCALE_ENVELOPE.md`). The exact seed
boundary is owned by `WP-CITY-03`, not by this document.

The water can initially be presentation-simple. H2 does **not** need a river simulation, swimming, hydrology or boats. The river is included because it is structural town identity and route geometry.

When this area is good, later phases **extend it at its seams**. They do not rebuild it as a different town.

## 1.5 Approximate scale

Use the current visual-bible anchors as starting values, not final geometry:

- ordinary streets: ~4–6 m;
- narrow old-quarter lanes may be tighter where navigation/readability still works;
- plaza/market space: roughly the current ~25×20 m order of magnitude, but irregular edges are preferred to a perfect rectangle;
- ordinary frontages: ~6–12 m;
- 2–3 floors, typically ~3–3.5 m per floor;
- most early daily-life POIs should remain roughly 30–90 seconds apart on foot for observability.

The 30–90 second rule is a development convenience, **not** a requirement that every route be short or pass through the plaza.

District-scale figures are not listed here. `CITY_SPATIAL_CONSTITUTION.md` §3 owns the city's dense-fabric band, playable envelope, walk-time hypotheses and the explicit conditions that would shrink or expand them.

## 1.6 Urban grammar for AI authoring

A capable LLM should not receive an empty plane and the instruction "place a nice house." The town needs machine-readable **urban constraints**.

A parcel/building site should be able to expose, where relevant:

- parcel polygon or bounded placement region;
- required street/frontage edge;
- approximate frontage and depth range;
- allowed floor/height range;
- party-wall / detached-edge rules;
- required public entrance side;
- roof-orientation / silhouette constraints where authored;
- permitted rear relation: patio, lane, river edge, neighbouring wall;
- semantic sockets: door, sign, shopfront, interior link, balcony, service entrance;
- collision/navigation clearance;
- important view corridor or no-build region;
- style/kit allowlist and vetoes.

A street or path should expose, where relevant:

- width and slope class;
- pedestrian/bicycle/vehicle affordances;
- surface family;
- frontage alignment rules;
- junction/connectivity identity;
- furniture/vegetation bands;
- stairs/ramp constraints;
- sightline or landmark intent.

A zone should expose:

- intended urban character;
- density range;
- allowed building archetypes;
- landmark/POI obligations;
- route connections that must remain open;
- dressing budget and visual vetoes.

The desired agent workflow becomes:

```text
inspect zone + parcel constraints
    -> discover available modules/compositions
    -> propose coherent composition
    -> plan/dry-run
    -> validate geometry/semantic dependencies
    -> apply atomically
    -> inspect realized engine result
    -> adjust if necessary
```

This is the mechanism by which the harness can make a weaker model useful at environment construction: the model still chooses; Arkus and the bridge make the legal/design space explicit and reject incoherent states.

## 1.7 Product-first rule

No H1/H2 demo requirement should deliberately force a layout decision that we already expect to delete for the shipping town. Temporary presentation is fine; temporary **urban structure** should be avoided once H2 begins.

---

# 2. Asset strategy

## 2.1 H1 uses real assets on purpose

The Unity bridge should not earn its meaningful parity claim only against cubes and synthetic fixtures. Real assets exercise failure classes that primitives hide:

- pivots and local origins;
- nested hierarchies;
- imported materials and atlases;
- scale conventions;
- colliders;
- prefab/module references;
- humanoid rigs and retargeting;
- animation clips;
- engine-side asset identity and missing references.

Therefore the desired future H1 boundary includes a **representative real-asset slice**. This is a planning decision; because the current roadmap still puts Quaternius adoption in H2, the roadmap author must make that boundary change explicitly after GATE.

## 2.2 Representative H1 slice

Small, real, deliberately boring in scope:

| Piece | Approx. count | What it proves |
|---|---:|---|
| masonry wall modules | 3 | modular placement, scale, shared material |
| roof section | 1 | material/style treatment, pivot |
| door | 1 | opening/socket and transition anchor |
| window/shutter module | 1 | façade assembly and accent material |
| bench/table/chair class | 1–2 | prop placement and later affordance anchor |
| street lamp | 1 | vertical prop / lighting assumptions |
| tree | 1 | vegetation import |
| rock mass | 1 | large scenic mesh / silhouette |
| base humanoid | 1 | rig, scale, presentation binding |
| idle/walk/sit clips | 3 | civilian retarget minimum |

Target: roughly **10–15 source pieces**, enough for one real street corner and one character.

H1 is **not** the point to build wardrobe, vehicles, a full bar interior, market dressing, dozens of houses or a whole asset pipeline.

## 2.3 Production kit categories

The long-lived kit should cover at least:

- ground/paving and kerbs;
- masonry walls and corners;
- dark-tile roofs;
- doors/windows/shutters;
- balconies / galerías;
- stairs and retaining edges;
- street furniture and signage sockets;
- market props;
- bar/shop/home interior basics;
- vegetation and Picos-adjacent rock masses;
- base humanoids, civilian wardrobe and hairstyles;
- civilian animation;
- bicycle/bus presentation later.

The likely identity gaps remain the same as the first draft: balconies/galerías, convincing contemporary rural wardrobe, selected vehicles and some locally distinctive props are more likely to need custom/adapted work than generic walls, rocks or crates.

## 2.4 Quaternius and licensing

The working preference is to use Quaternius Source distributions when the source files materially improve adaptation, subject to the project's dependency/IP policy.

Before any pack becomes an adopted project dependency:

- a human verifies the exact current license/version/source record;
- the pack receives the required dependency record;
- source archives remain outside the repository as required by the art/dependency policy;
- only permitted manifests, hashes, recipes and project outputs are versioned.

The current visual bible says "Quaternius free tier" while this blueprint prefers paid Source access where useful. That is an **explicit open reconciliation item**, not something this document silently changes.

## 2.5 Art-production rule

**Reuse the generic; author/adapt the memorable.**

A generic wall, tree or chair does not need to prove originality. The things that make the fictional Liébana town recognisable — silhouette, galleries/balconies, wet dark roofs, river edges, street proportions, wardrobe, signage and selected hero props — deserve the project's art effort.

---

# 3. Composition, prefabs and catalogue discovery

## 3.1 Reusable composition ladder

| Level | Meaning | Example |
|---|---|---|
| L0 | module | wall bay, roof piece, door |
| L1 | assembly | door bay, shuttered window bay |
| L2 | shell | façade grammar + floors + roof |
| L3 | reusable building composition | house/bar/shop shell with semantic sockets |
| L4 | functional POI composition | bar/shop/workplace with capacities/affordances |
| L5 | street segment | straight/corner/steps/bridge edge |
| L6 | authored zone | segments + buildings + dressing + constraints |

Early reusable families should include a few houses, a gallery house, bar, ground-floor shop, workshop, bridge edge, market stall/terrace grouping and street segments. The exact list follows real asset triage rather than being frozen on paper.

## 3.2 What the LLM must be able to discover

An agent asked to "add a bakery on this parcel" needs to discover without reading C#:

- available modules and reusable compositions;
- stable composition/asset IDs;
- tags/archetypes;
- physical dimensions and sockets relevant to placement;
- compatibility/material/style metadata needed to choose correctly;
- licensing/adoption status where it affects legal use;
- dependency relationships;
- whether an item is an engine asset, transformed project asset or project-authored composition.

This discovery surface is a **product requirement**.

## 3.3 What is deliberately not decided yet

The first draft proposed putting the prefab catalogue itself into canonical `WorldState` as `WorldObject`s. This revision withdraws that proposed implementation.

There is a real requirement — one discoverable source of catalogue truth visible through Arkus — but there are several possible correct ownership models. H1 must review them against accepted architecture.

The current preferred direction is:

```text
Unity/project asset catalogue
        owned by bridge/project integration
                    ↓
transport-neutral Arkus scoped capability
                    ↓
agent discovers stable catalogue/composition IDs
                    ↓
canonical authored world refers to selected IDs + authored overrides
```

The important guarantees are:

- no hidden adapter-only registry;
- no hand-maintained second semantic command universe;
- changing the installed asset library must not arbitrarily rewrite the canonical town hash when no authored town choice changed;
- a canonical world reference to an unavailable asset/composition must fail visibly and diagnostically;
- composition references understood by a schema-aware producer derive their typed dependency surface mechanically where the accepted contract requires it.

Whether catalogue entries themselves ever become canonical authored objects is therefore a **reviewed H1 decision**, not a premise of this blueprint.

## 3.4 Promote good compositions instead of rebuilding them

When an agent constructs a coherent house/bar/shop from lower-level modules and that composition passes human/automated review, the project should be able to promote it to a reusable composition with:

- stable ID;
- source/module lineage;
- parameter/variant surface;
- validated sockets and bounds;
- style/category tags;
- compatibility/version metadata.

That is how early expensive agent work turns into later cheap model work. A weaker model should often be instantiating and adapting reviewed compositions rather than designing every façade from first principles.

---

# 4. NPC foundation

## 4.1 Persistent identity is independent of the model

The useful idea from `Arkus0/Juego` survives unchanged at the design level:

```text
npc.antonio
  authored identity/profile
  home/work/frequents references
  authored schedule definition
  authored initial social/knowledge setup
  presentation binding -> replaceable character asset
```

Antonio can be a placeholder humanoid, a transformed Quaternius base character and later bespoke art without changing his ID, authored relationships, home, work or narrative role.

Live position, current activity, current emotion/relationship values and current beliefs during a running save do **not** automatically become canonical authored `WorldState`; see §6.

## 4.2 Initial persistent cast

Thirteen identities are enough to plan the town without pretending to write the whole population now:

| ID | Working role | Home/work anchor | Primary system value |
|---|---|---|---|
| `npc.ana` | bar server | home nearby / bar | baseline schedule + service interaction |
| `npc.antonio` | valley worker | Barrio Alto / workshop | deep reference actor |
| `npc.manolo` | bar owner | above/near bar / bar | source of social information |
| `npc.carmen` | shopkeeper | residential / shop | relationship + later behaviour change |
| `npc.paco` | builder | town / rotating work | changing work target / obligations |
| `npc.rosa` | neighbour + dog walker | Barrio Alto | alternate walking routes |
| `npc.teresa` | baker | above bakery / bakery | schedule crossing midnight/pre-dawn |
| `npc.javier` | teen + bicycle | Barrio Alto | non-pedestrian route affordance |
| `npc.lucia` | municipal clerk | town / civic office | institutional interaction |
| `npc.bernardo` | outsider | outside / none | bus arrival / outsider knowledge |
| `npc.pilar` | older neighbour | Barrio Alto | witness/relay/bench routine |
| `npc.guardia` | municipal officer | outside/town / civic patrol | event override / authority interruption |
| `npc.tomas` | farmhand/market visitor | outside / huerta/market | abstract/off-screen simulation |

Plus a small ambient tier (`ambient.shopper`, `ambient.walker`, `ambient.worker`) that does not acquire persistent social identity unless explicitly promoted.

These are **development roles**, not final character writing. No detailed backstories, dialogue scripts or relationship numbers are frozen here.

## 4.3 The cast should use the town, not decorate it

The new topology lets NPCs prove different spatial behaviours:

- Antonio uses slope + old bridge + work edge;
- Ana has a short home-to-bar commute and service schedule;
- Pilar can choose plaza or riverside walk;
- Javier has a bicycle-compatible path distinct from stair routes;
- Tomás can remain outside full simulation and enter through huerta/market edge;
- Bernardo enters from the bus edge rather than spawning inside the social hotspot.

This prevents the common failure where every NPC is technically scheduled but all routines look like variations of "walk to the same square."

## 4.4 Early causal social fixture

The most valuable pre-combat fixture is intentionally simple:

```text
Manolo possesses / communicates fact F
    -> Pilar witnesses or receives F with provenance
    -> Pilar may relay F to Antonio
    -> Antonio's later eligible behaviour changes
    -> Carmen later reacts differently because of a related event/belief/relationship state
    -> the runtime can explain the causal chain
```

The exact story content is not important yet. The architecture should make the chain possible without scripting each downstream reaction as a bespoke quest trigger.

---

# 5. System dependency map

## 5.1 Preserve the authored/live boundary

`WP-HK-06A` accepted a hard distinction between authored canonical history and runtime observation. Production planning must not erase it.

| Concern | Authored/project definition | Live runtime/save |
|---|---|---|
| town | objects, zones, parcel constraints, POIs | currently loaded/visible realization |
| time | schedule/opening-hour definitions | current clock/date |
| actor | identity, role, home/work, schedule profile | position, current activity, temporary state |
| knowledge | facts and authored initial beliefs/rules | who currently believes what and why |
| relationships | relation kinds + authored initial edges | current relationship values/obligations after play |
| doors/props | authored existence and setup | currently open/broken/occupied state |
| story | authored conditions/outcome definitions | current stage/consequences in this save |

The runtime may expose inspection/testing capabilities through Arkus, but that does **not** imply the live save becomes the authoring journal.

## 5.2 Product layers

| Layer | Purpose | Depends on |
|---|---|---|
| L0 | accepted H0 canonical kernel | — |
| L1 | H0 host/protocol/efficiency/closure | L0 |
| L2 | Unity engine bridge + real representative assets + presentation/catalogue projection | GATE |
| L3 | product-seed world composition, spatial constraints, navigation, player/camera/interaction shell | L2 + Unity parity gate |
| L4 | actors, animation, time, schedules, POIs/smart objects, routine resolver | L3 |
| L5 | **Living World Core:** dialogue conditions, knowledge/beliefs, relationships, events, minimal memory, structured outcomes and behaviour changes | L4 |
| L6 | Directed Drama: full GameFlow authority, cinematics and QTE | L5 |
| L7 | Combat | L6 + actors/animation + living-world outcome bridge |
| L8 | deeper autonomous agency, richer social simulation, abstract/off-screen continuity and population scaling | L5, optionally L7 consequences |
| L9 | integrated vertical slice and runtime save/load closure | preceding required layers |

Key ordering rule: **L5 precedes combat.** The town should become socially causal before combat becomes a production dependency.

## 5.3 Why Living World moves earlier

The reference archive contains much more completed/reviewed research on NPC daily life, agency, social graph, knowledge and rumours than on combat. The combat track largely remained a design constitution plus preregistered research plans.

More importantly, Juego2's distinctive product promise is better demonstrated early by:

```text
something happens / is said
 -> somebody else learns it
 -> a relationship/belief changes
 -> a later routine/dialogue/action changes
 -> the player can observe and understand why
```

Combat can then return outcomes into that already-working world rather than becoming a large isolated system waiting for social consequences later.

## 5.4 Navigation and simulation boundaries

- Navigation derives from built geometry; do not wait for actor AI to define the walkable world.
- Schedules describe intentions and destinations, not hand-authored waypoint scripts.
- Abstract/off-screen simulation preserves semantic activity/outcome state without requiring NavMesh/Animator frame-by-frame execution.
- Dialogue consumes structured conditions/beliefs/relationships; text is not authority.
- Combat emits structured outcomes; it does not own a duplicate social state model.
- Autonomous agency comes **after** one NPC can live a stable day and after knowledge/relationship/event primitives exist.

---

# 6. Two production seams that must be owned explicitly

The first draft correctly identified two gaps. This revision gives them a planning direction without promoting them into H0 obligations.

## 6.1 Multi-session authored project persistence

Current H0 semantics provide canonical state identity, snapshots, import/rebase, provenance and replay; they do not claim a production project database or durable WAL.

For H1/H2, the simplest product architecture should be preferred first:

```text
canonical authored session
      -> atomic project checkpoint / canonical snapshot
      -> project workspace storage
      -> next session loads through accepted import/rebase mechanism
```

Planning constraints:

- storage must preserve the accepted canonical artifact rather than invent a second world format;
- a failed checkpoint must not produce a false authoritative project version;
- project workspace metadata (asset catalogue location, editor preferences, caches) is not automatically part of canonical world state;
- do not add a complex WAL/event-store architecture unless real multi-session recovery evidence requires it.

This is **post-GATE project infrastructure**, not a reason to reopen HK04/HK06 now.

## 6.2 Runtime/save-state authority

Gameplay needs a separate versioned runtime state/save contract. It should contain only what a running game needs to resume/continue, for example current time, actor live state, current beliefs/relationships/events/stages and other simulation values that have actually become product requirements.

Planning constraints:

- runtime save state is not the HK06 authored journal;
- authoring a schedule definition and playing through that schedule are different authorities;
- runtime state may be inspected/simulated/tested headlessly where useful;
- gameplay outcomes may be persisted in a save without rewriting the authored project definition;
- a future "promote runtime result into authored content" feature, if ever useful, must be an explicit authoring operation rather than an implicit side effect.

The exact runtime schema is intentionally deferred until L4/L5 has real systems to save.

---

# 7. Suggested post-GATE phase ladder

These are **planning phases, not authored workpacks**.

| Phase | Scope | Demo worth showing |
|---|---|---|
| **H1** | Unity bridge foundation, presentation binding, catalogue projection decision, representative **real asset** slice | Arkus changes authored state; Unity realizes it deterministically; one real street corner + one real humanoid/animation set |
| **Unity parity gate** | prove bridge semantics and real-asset boundary | no gameplay expansion until this passes |
| **H2** | product-seed town composition, urban constraints, navigation, player movement, third-person camera, interaction, one bar interior | walk a **keeper** plaza/bridge/street/bar area that will remain in the final town |
| **H3** | actors, animation, clock, schedules, POIs/smart objects, routine resolver | six NPCs live a readable day and use different routes/places for understandable reasons |
| **H4** | **Living World Core:** dialogue conditions, beliefs/knowledge, relationships, events, memory minimum, structured outcomes, runtime/save minimum | Manolo/Pilar/Antonio/Carmen causal chain changes later behaviour and the system can explain why |
| **H5** | GameFlow authority, cinematics, directed sequences, QTE | one skippable/failable dramatic sequence acquires and returns player/camera/actor authority cleanly |
| **H6** | combat prototype/runtime, returning outcomes into Living World | one instrumented encounter affects witnesses/relationships/events rather than living in a separate dimension |
| **H7** | deeper actor-to-actor agency, abstract simulation, ambient population scaling | player absent: actor-originated causal chain continues, remains bounded and explainable |
| **VS** | integration only; no new foundational systems | full product-seed vertical slice |

H0S scale/concurrency work can run in parallel after GATE exactly as the current roadmap already allows; it should not delay H1 by default.

---

# 8. Vertical-slice target

## 8.1 Minimum product slice

Place:

- product-seed plaza/market edge;
- old bridge + visible river strip;
- 60–100 m old-quarter/commercial connection;
- bar interior;
- one ascending residential lane;
- one stream crossing, so the "follows an NPC through a route choice" scenario has a real branch.

This is the §1.4 seed, not a second area. Where the two lists differ, §1.4 is the one that counts.

Cast: six NPCs in full simulation; additional persistent actors may exist abstractly.

Core chain:

```text
arrive through the town edge
 -> cross / approach the old bridge and market area
 -> enter the bar
 -> speak with Manolo
 -> learn or witness a fact with source/provenance
 -> leave and continue normal town activity
 -> information/event reaches another actor through accepted social rules
 -> Carmen later speaks or behaves differently
 -> the player can notice the changed regularity
 -> the system can explain the causal chain
```

That is already a convincing Juego2 slice without combat.

A later enhanced slice may insert a directed dramatic beat, QTE or fight. Those improve spectacle; they are not prerequisites for proving the living-town identity.

## 8.2 The player must be able to learn the town

Longer term, the town succeeds when routine knowledge becomes useful:

- where somebody tends to be at a given hour;
- which route or place they prefer;
- who they know/trust/fear/owe;
- who could plausibly know a fact;
- when an absence or changed routine is suspicious;
- how an event changed visible behaviour.

The vertical slice only needs one or two of these to be useful. The product can expand the same grammar rather than replace it.

## 8.3 Visual budget

The current visual bible's hero budget (≤120 meshes, ≤6 atlases, ~20 animation clips, six NPCs) should remain a **constraint to test**, not an invisible promise to violate.

If the keeper seed genuinely cannot achieve the intended silhouette and route structure inside 120 meshes, the future art/roadmap owner should either:

- shrink the first visible extent while preserving the topology; or
- explicitly revise the budget with evidence.

Do not quietly inflate the budget because a more ambitious sketch was written here.

---

# 9. What should be decided before H1, without starting H1 early

These are cheap planning decisions or human prerequisites that may be prepared while H0 finishes, but they do not authorize implementation:

1. Exact Quaternius pack/version/license record and whether Source tier is adopted.
2. Camera target and Unity render-pipeline choice.
3. Stable naming convention for town IDs before content proliferates.
4. The initial asset slice and adaptation recipes.
5. Which open content shapes are actually required by H3/H4 (especially ordered/wrapping intervals, pair-state/relationship representation, namespaced story/runtime state) — decide only when their consumer is clear.
6. H1 catalogue authority/projection boundary, explicitly **without assuming catalogue = `WorldState`**.
7. Project-workspace checkpoint ownership for multi-session authoring.
8. Runtime/save-state authority boundary for H4+, preserving HK06 authored/live separation.
9. A better visual/reference board for buildings, characters/wardrobe, river/bridge edges and late-PS2/early-PS3 style target.

None of these should mutate accepted H0 semantics merely to make future planning feel complete.

---

# 10. Rabbit-hole guards

The project has already paid for several lessons. Keep them visible:

- Do not expand proof/process machinery after it stops reducing meaningful product risk.
- Do not reverse-engineer another game's implementation to obtain fidelity that can be specified behaviourally.
- Do not adopt another engine/world foundation merely to get a town quickly.
- Do not build the Creator GUI before the contracts have survived real game authoring.
- Do not build generic GOAP/Utility AI before one actor can live one stable day.
- Do not build a giant asset-transformation factory before roughly ten real assets have shipped end-to-end through the bridge.
- Do not model every prop as a smart object.
- Do not create a bespoke executable day script for every NPC.
- Do not let dialogue text become knowledge/state authority.
- Do not assume rumours propagate automatically; receiving/relaying information is behaviour with provenance.
- Do not add per-resource CAS, Merkle trees, locks or multi-agent orchestration before H0S measurements justify them.
- Do not model the whole valley before the keeper town core is fun and useful.
- Do not make combat a prerequisite for proving the living town.
- Do not write large amounts of final narrative before the dialogue/Living World runtime has a stable authoring shape.

A useful question for any proposed subsystem before VS: **does it make the next visible demo materially better or unlock a required causal dependency?** If neither, it probably waits.

---

# 11. Recovered design principles worth carrying forward

The old `Arkus0/Juego` repository remains reference material only, but several design principles are valuable independent of its abandoned architecture.

## 11.1 Living city

- The city does not wait for the player.
- Persistent NPCs may be primary causes of events.
- Actor→Actor interaction is first-class; the player is not required for every chain.
- Actors act from beliefs/perception/relationships, not omniscient world truth.
- Schedule is baseline, not destiny.
- Relevant autonomous decisions should be explainable.
- Meaningful actions produce structured consequences.
- Relationships affect decisions, not only dialogue filters.
- Bounded autonomy is better than unlimited invisible emergence.
- Ambient population and persistent actors have different budgets/identity requirements.
- An LLM may author or explain but is not runtime simulation authority.
- Causal simulation should be testable headlessly even when presentation requires Unity.

## 11.2 Combat

The combat charter remains useful product direction, but its implementation research is far less mature than the Living World research. Preserve the principles without pulling combat forward in the schedule:

- clean hits matter;
- difficulty comes from access/openings/timing/space, not ordinary-human damage sponges;
- defence can preserve/steal initiative;
- crowds need active but readable pressure;
- target transitions and environment use are part of the choreography;
- camera serves play readability first;
- responsiveness outranks cinematic synchronization;
- impact presentation derives from gameplay truth;
- combat consequences return to the living town;
- feel claims require instrumented play, not only headless proof;
- animation count is not a substitute for a coherent combat grammar.

Nothing in this section carries an old repository PASS into Juego2. It is design input only.

---

# 12. Open reconciliation items

| Item | Current state | Desired future decision |
|---|---|---|
| Quaternius timing | current roadmap puts import/select in H2 | explicitly move a tiny representative real-asset slice into H1 when post-GATE roadmap is authored |
| Quaternius tier | visual bible says free tier | decide free vs Source after exact dependency/license review |
| town layout | first draft was a radial test hub; revision 0.2 proposed a single-river loop diagram | **resolved** — `CITY_SPATIAL_CONSTITUTION.md` owns the topology, district families and crossings |
| river port plausibility | the setting permits a small river landing with working boats | confirm with `WP-ART-00` the fictional premise that the joined river below the confluence carries loaded shallow craft; if refused, reopen the selected topology |
| city scale band | 0.8–1.2 km² dense fabric and a 0.10–0.15 km² first seed were working hypotheses | **corrected** to ≈0.30–0.45 km² and ≈0.03–0.06 km² on traversal, density and content-cost grounds; reopen the constitution rather than stretch it if the band ever rises above ~0.7 km² |
| prefab catalogue authority | first draft proposed canonical `WorldObject` catalogue | review in H1; require discoverability without pre-deciding storage authority |
| phase order | first draft put cinematics/QTE/combat before social simulation | Living World Core first; drama then combat; deeper agency later |
| durable authored persistence | H0 has no production project store claim | use canonical snapshot/checkpoint-based project workspace first; escalate only with evidence |
| runtime/save state | deliberately outside canonical authored journal | define separate runtime/save authority when H3/H4 requires it |
| mesh budget | approved-draft hero cap may be tighter than keeper seed | preserve cap unless an art review explicitly changes it |
| camera/render pipeline | not yet frozen | decide before real H1 asset adoption |

---

# 13. What this document deliberately does not do

- It does not modify `ROADMAP.md`, `WP-HK-GATE`, any H0 workpack, accepted proof or accepted guarantee.
- It does not authorize H1 before GATE.
- It does not freeze final geometry or final character writing.
- It does not make real Potes the shipping map; the town remains fictional.
- It does not decree a prefab-catalogue storage model.
- It does not merge authored project state with live gameplay state.
- It does not require combat for the first convincing game slice.
- It does not claim that any design research PASS from `Arkus0/Juego` transfers to Juego2.

The intended production philosophy is simple:

> **Do not build a demo and then build the game. Build a small, keeper-quality piece of the game, and let the demo prove that piece already works.**
