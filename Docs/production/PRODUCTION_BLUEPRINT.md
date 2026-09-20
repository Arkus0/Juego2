# Juego2 Production Blueprint

Version: 0.2 — 2026-09-20  
Status: **NON-BINDING, owner-reviewed production proposal.** This document creates no H0 acceptance criterion, reopens no accepted H0 guarantee, starts no gameplay work and does not modify the `WP-HK-GATE` precondition. It is planning input for the roadmap author after GATE.

## 0. Decisions captured by this revision

This revision turns the first draft from a good test-town plan into a **product-seed plan**. Five planning decisions guide everything below:

1. **The demo is the first piece of the game, not a disposable prototype.** The first serious town geometry should be extendable into the shipping town rather than replaced after proving systems.
2. **H1 should prove the Unity bridge with a small slice of real Quaternius-derived assets, not only primitives.** This deliberately differs from the current `ROADMAP.md`, which places Quaternius content in H2. Because this file is non-binding, it records the desired future H1/H2 boundary; the roadmap must be amended explicitly after GATE before implementation.
3. **Living World Core comes before combat.** A social consequence that changes another NPC's later behaviour is a more important early proof of Juego2 than a fight. Directed drama and combat build on the living town, not the reverse.
4. **Asset and prefab discovery must be machine-readable, but the catalogue is not declared canonical `WorldState` by fiat.** H1 must decide the reviewed catalogue authority and projection boundary. The likely shape is bridge-owned catalogue data projected through Arkus scoped capabilities, with canonical world objects referring to stable catalogue/composition IDs.
5. **Authored project state and live game/save state remain separate authorities.** Multi-session authoring needs durable project checkpoints; gameplay needs its own runtime/save contract. Neither is allowed to smuggle live simulation into the accepted HK06 authored journal/replay semantics.
6. **The player must be able to act on the town, not only read it.** The recovered research designs a town that runs without the player and channels through which the player finds out what it did. It never designs a player verb. A living world the player can only observe is a museum; §6.3 closes that gap, and it is cheap because the machinery already exists on the NPC side.
7. **Combat is scoped to short, lethal and rare, and gated behind one feel prototype.** Feel cannot be established on paper. §12 records the scope and the single experiment that should settle viability before further research is opened.

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

```text
                         LADERA / BARRIO ALTO
                       /        |          \
                viviendas -- callejas -- mirador
                    |           |             |
                 escaleras      |        camino exterior
                    |           |             |
             CASCO VIEJO ---- BAR/CALLEJON   |
                 |     \          |           |
                 |      \         |           |
             PUENTE VIEJO ---- PLAZA/MERCADO ---- CALLE COMERCIAL ---- ENTRADA / BUS
                 |                 |      \              |                  |
                 |                 |       \             |                  |
          RIBERA / TALLERES ------+---- PASEO FLUVIAL ---+----------- carretera exterior
                 |                                                        |
              huertas ---------------------------------------------- camino de valle
```

This topology has **loops**. The plaza is still important, but it is no longer the only possible route between every pair of places.

That matters to the game as much as to the art:

- Antonio can descend from Barrio Alto, cross the old bridge and reach the workshops without traversing the whole commercial street.
- Ana can take a short lane from her home to the bar.
- Pilar can choose the riverside walk rather than the market route.
- Javier's bicycle prefers the exterior/commercial route and avoids steep steps.
- Tomás can arrive from the valley/huertas directly toward the market.
- Bernardo arrives by bus and enters through the commercial edge.

NPC schedules therefore create believable traffic because the **town affords different routes**, not because a script funnels actors past the player.

## 1.3 Zones

| Zone | Product character | Core POIs | Purpose |
|---|---|---|---|
| `zone.cascoverde` / old-quarter equivalent | tight stone lanes, small level changes, irregular frontages | bar alley, old residences, small courtyard | strongest town identity; intimate social encounters |
| `zone.plaza_mercado` | civic/commercial meeting space near a crossing | market, fountain, benches, civic frontage | visible convergence without becoming the only hub |
| `zone.calle_comercial` | main everyday spine | shop, bakery, pharmacy, residential portals | interaction, opening hours, everyday pedestrian flow |
| `zone.barrio_alto` | stepped residential lanes on the slope | homes, wash place/patio, viewpoint | schedules, vertical navigation, off-screen transitions |
| `zone.ribera` | work edge along the water | workshop, storage, huerta access, river walk | work routines, alternate routes, later expansion |
| `zone.entrada` | road/bus arrival and outward connection | bus stop, road, path to later cemetery/valley | arrivals, departures, outsider traffic, expansion seam |

Names are placeholders; identifiers are not frozen by this document.

## 1.4 The first buildable product seed

The first serious H2 town build should be **small enough to finish and good enough to keep**:

- one side of `zone.plaza_mercado`;
- one old stone bridge;
- approximately 60–100 m of the commercial/old-quarter connection;
- the bar exterior and one bar interior;
- one ascending lane toward Barrio Alto;
- 6–8 authored building frontages;
- a short visible river/stream strip and riverside edge;
- distant green slopes / rock silhouettes.

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

# 6. Production seams that must be owned explicitly

The first draft correctly identified two gaps, and review since has surfaced a third. This section gives all three a planning direction without promoting them into H0 obligations.

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

## 6.3 Player agency

The first draft and this revision both describe a town that becomes socially causal. Neither describes what the **player** does to it. That is a third seam of the same kind: identified, unowned, and cheap to close now because nothing is built.

It is measurable rather than interpretive. A verb sweep across the ~4 800 lines of living-city research in the reference archive returns only epistemic or passive constructions — *can learn*, *can reconstruct*, *can predict*, *puede presenciar*, *puede conocer*, *no presencia*. Not one causal verb. The nearest is the constitutional "el player puede entrar en la cadena", which grants permission without naming an action.

Four findings are sharper than the count:

- **The exclusion is visible in the API.** `Juego/Docs/Architecture/GAMEPLAY_SYSTEMS_ARCHITECTURE.md:343-348` defines `Tell(from, to, fact, sincerity)` — bidirectional in shape and explicitly lie-capable — and then a *separate, receive-only* verb, `LearnPlayer(fact)`. If the player were a peer in `Tell`, `LearnPlayer` would not need to exist. The knowledge pipeline at `:326-337` runs `WorldFact → NPCBelief → PlayerKnowledge` with no upward arrow, and no transfer originating from the player exists anywhere in the archive.
- **The player exists as a causal origin only in order to be excluded.** `Juego/Docs/living-city-research/PA-02_NPC_AGENCY.md:362-384` defines `originKind: PLAYER | ACTOR | WORLD_SYSTEM | AUTHORED_STORY`, then rules that "only `originKind = ACTOR` can satisfy the actor-originated proof". Nothing downstream consumes `PLAYER`.
- **A drift nobody recorded.** `Juego/Docs/living-city-research/ROADMAP.md:410` asked *"¿Como hacemos que **ser alcalde** modifique vidas y flujos reales del pueblo…?"*. The frozen `PA-10_PLAN_GOVERNANCE.md:14` restates it as *"¿Cómo puede **una decisión municipal** cambiar horarios…?"*. The player was the subject of the question and is not the subject of the answer.
- **The archive's own review gate already caught the risk.** `Juego/Docs/living-city-research/PA-12_PLAN_INTEGRATION_REVIEW.md:74`, failure mode F12-10: *"Causal proof existe pero player no puede percibir/explotar nada."* The risk is written down. "Explotar" is never defined anywhere.

Four documents assert player-side causality as a premise — `ADR-013:10` ("el player debe poder presenciar, descubrir, **alterar** o ignorar esas cadenas"), `ADR-011:42` ("eventos del jugador pueden alterar organicamente conocimiento, relaciones y rutinas"), `LC-03`, and the archive README. Zero documents specify it.

### Three different things, not one mistake

| | What it is | What to do |
|---|---|---|
| A defensible exclusion | The knowledge and rumour tracks keep the player out of the epistemic model on purpose. NPCs must reason from information they can reach, and letting the player write beliefs directly is a real omniscience hazard, warded off as failure mode F09-9, *"Player knowledge = NPC belief"* | Keep it, and design around it |
| An unnoticed drift | The governance question lost its subject between the roadmap and the frozen plan | Recover the original question. The frozen plan is a rules-mutation contract that works identically whether the mayor is the player, an NPC or a die roll |
| A structural gap with no owner | Player-side causality asserted in four documents, specified in none | This is the part that needs new design |

### Adding verbs without breaking the epistemic boundary

The unsafe design is "the player writes a belief". F09-9 is right to forbid it.

The safe design is already specified, for NPCs: the player issues a `TELL`, which is an **assertion**, and the receiver decides whether to accept it through the same receiver-owned interpretation boundary the rumour track already requires (`PA-05-H02`, `D-05-02`). A false or partial claim is already modelled as an assertion payload that leaves canonical truth untouched (`D-05-08`), distortion already transforms an assertion edge rather than mutating truth (`PA-05-H06`), and deception already has to create epistemic state rather than merely deceptive text (`PA-04-H07`).

So lying is modelled, receiver autonomy is modelled, provenance is modelled. What is missing is only the player's legality as a sender. The player becomes a peer in the transfer contract, not an exception to it, and nothing the player says is automatically believed.

### Verb inventory by cost

| Tier | Verbs | Why it costs what it costs |
|---|---|---|
| **0 — already modelled** | `TELL` (player as a claim source), lie, `ASK`, `CONFRONT`, `REPORT`, `OFFER_HELP` / `REQUEST` | The machinery exists on the NPC side: belief with source categories, receiver-owned interpretation, false claims as payloads, an identified debtor/creditor obligation lifecycle. Only the player's legality as an actor is missing |
| **1 — small addition** | occupy or deny a POI; give or take an object; be witnessed | Reuses POI capacity and reservation, resource/ownership change, and witness metadata — which yields reputation without needing a reputation system |
| **2 — real work** | governance, economy, violence | Both governance and economy research remain unstarted; violence is the combat→living-world edge |

Tier 0 is the striking one: six verbs, and the cost is a design decision rather than an engineering project.

### The highest-leverage single verb

**The player as a node in the knowledge and rumour graph.** In an investigation game this is not one feature among several: it means investigating alters what is being investigated. Ask Carmen about Manolo and Carmen now knows you are asking, and may tell him. Say something false and it propagates with you as its source. It reuses the knowledge and rumour design wholesale, respects every epistemic boundary those tracks established, and converts the strongest asset in the recovered design from scenery into a system the player plays.

Planning constraints:

- the player is a peer in the transfer contract, never an exception to it;
- nothing the player asserts is automatically believed;
- player actions are themselves events with witnesses;
- do not build a player-only social subsystem — the archive rejects that in two places, and it is still right;
- tier 0 is a design decision, not a research track; do not defer it to one.


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
- one ascending residential lane.

Cast: six NPCs in full simulation; additional persistent actors may exist abstractly.

Core chain:

```text
arrive through the town edge
 -> cross / approach the old bridge and market area
 -> enter the bar
 -> ask Manolo about Antonio
 -> learn or witness a fact with source/provenance
 -> tell Carmen yourself, truthfully or not
 -> Carmen decides whether to believe you
 -> her later routine or dialogue changes because YOU told her
 -> Manolo learns that you were asking
 -> the player can notice the changed regularity
 -> the system can explain the chain and name the player as its source
```

That is already a convincing Juego2 slice without combat.

The sixth and eighth steps are what make it a game rather than a diorama. A world that rearranges itself while the player watches proves the simulation; a world that rearranges itself **because of something the player chose to say** proves the game. The receiver stays free to disbelieve, per §6.3, so this costs no omniscience — and the eighth step matters as much as the sixth, because acting on the town has to be observable *by the town*.

A later enhanced slice may insert a directed dramatic beat, QTE or fight. Those improve spectacle; they are not prerequisites for proving the living-town identity.

## 8.2 The player must be able to learn the town, and then use what they learned

Longer term, the town succeeds when routine knowledge becomes useful:

- where somebody tends to be at a given hour;
- which route or place they prefer;
- who they know/trust/fear/owe;
- who could plausibly know a fact;
- when an absence or changed routine is suspicious;
- how an event changed visible behaviour.

The vertical slice only needs one or two of these to be useful. The product can expand the same grammar rather than replace it.

Learning is half of it. The other half is that knowing these things must let the player *do* something — tell the right person, ask in front of the wrong one, be somewhere at the hour that matters. A regularity the player can only admire is a fact sheet.

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
- Do not open twelve combat research tracks before one prototype has answered whether the feel is reachable at all.
- Do not build a living world the player can only watch; a player verb that reuses an existing NPC-side mechanism is cheaper than the system it plugs into.
- Do not give the player a private social subsystem either; the same abstractions run in both directions.

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

# 12. Combat: scope, viability and the feel spike

The phase ladder already places combat at H6, after Living World Core. This section records why the scope is what it is, and the one experiment that should settle it.

## 12.1 Three questions wearing one coat

"Is this combat viable for five people without an animator" is unanswerable as asked, because it bundles three things with different answers.

| | Question | Answer |
|---|---|---|
| **a** | Moveset breadth — a large bespoke martial vocabulary | Not viable at this team size. Flatly |
| **b** | Kinesthetics — does a hit feel immediate and physical | Viable, and mostly *not* an animation problem |
| **c** | Music-action synchronisation | The cheapest of the three, and the most under-rated |

## 12.2 What the archive already concluded

Two recorded answers, neither of them opinion.

`Juego/Docs/ASSET_FACTORY.md:99-106` states the two Universal Animation Libraries "deberían cubrir una gran parte del trabajo rutinario", then lists what they do not cover — naming explicitly *"kung-fu/moves de personajes cuando el set genérico no alcance calidad"*. Its production rule: **"comprar/reutilizar lo genérico; autorar lo memorable"**.

`Docs/art/VISUAL_BIBLE.md` §8 budgets ~20 animation clips, §9–12 restricts animation to a civilian allowlist, and §2 puts combat animation on the No list. So the generic libraries are not enough for kung fu, and the current art direction does not budget for combat animation at all. Both were already written down.

## 12.3 Feel is not primarily an animation problem

This is the reframe that changes the estimate, and it is the archive's own preregistered hypothesis rather than a claim invented here. `Juego/Docs/combat-research/C-PA-01_PLAN_FEEL_IMPACT.md` hypothesis **H2**: *"La sensación física depende más de sincronía/contraste/latencia que de fidelidad gráfica."*

Feel lives in frame data, cancel windows, hit stop, camera and audio — programming and tuning, not animation. Mediocre clips with excellent timing feel good; excellent clips with bad timing do not.

The bar that plan sets is also lower than it sounds. Acceptance scenario **A2** asks only that a player reliably distinguish **four** contact categories — block, deflect, glancing hit, clean decisive hit — presented without damage numbers, and without every category using maximum intensity. Four states, not a moveset.

## 12.4 The charter is accidentally budget-friendly

Read together, "clean hits matter", "mobs are flow, masters are openings" and "boss difficulty comes from access, not durability" describe **short, lethal exchanges**. That needs far fewer animations than combo-heavy design, and puts the target closer to Sekiro or Sifu than to the combo game the ambition is usually compared against.

`C-PA-12`'s hypothesis **H3** makes the same move at the content level: *"Una vertical pequeña con pocos enemigos/props bien instrumentados es mejor gate que una gran demo content-heavy."*

## 12.5 The music goal is the cheap half

Moments where music and action coincide are, mechanically, adaptive audio: vertical layering, stingers on finishers, tempo-aware transitions, intensity tracking. Middleware work a musician can own nearly end to end, requiring no additional animation.

The expensive version — combat animation authored to land on a beat — is already forbidden by the charter's own ranking of responsiveness above synchronisation. The correct design is music reacting to combat, which is also the affordable one.

## 12.6 Accepted scope: short, lethal, rare

| | |
|---|---|
| Vocabulary | 1 stance; 4–6 attacks; 1 deflect; 1 dodge; 1 finisher; ~3 reaction sets |
| Encounters | 6–10 authored, each tuned individually |
| Frequency | Rare and consequential, never a routine traversal cost |

Rarity does real work here, not just budget saving. Combat is required to return state to the living town, and a fight the whole town talks about is meaningless if there is one every ten minutes. The failure mode that makes beloved experimental games clunky is usually frequency: combat performed constantly cannot be hand-tuned; eight authored encounters can.

## 12.7 Closing the animation gap

Three routes, chosen together, with no contracted animator:

- **markerless mocap from video** — enough fidelity for stylized low-poly, and it makes the team autonomous for iteration, which matters because timing is where feel lives;
- **martial-arts-specific packs** instead of generic libraries, accepting a worse fit against the base rig and a transform pass onto the presentation profile;
- **designing around the gap** — fewer moves, better transitions, cancellation and warping.

The honest note on the combination: **mocap supplies motion, not timing.** Capture gives raw clips; frame data, cancel windows, hit stop and reaction matching still have to be authored. Design-around is therefore load-bearing rather than a fallback, and the bulk of combat-feel work is programming.

## 12.8 The feel spike

**Run `C-PA-01` as a prototype rather than as a document.**

| | |
|---|---|
| Content | One enemy. Three attacks, one deflect, one dodge. Quaternius clips as they come |
| Work | Hit stop, camera, audio and input handling tuned to death. No new animation |
| Exit test | A2's four-state discrimination without damage numbers, plus a playtest |
| Output | A decision. Not code that ships |

The charter already rules that headless tests cannot establish feel, and that claims about responsiveness, readability, camera, hit stop, audio or haptics need instrumented play. No document can answer this question, including this one.

**The governance collision, stated rather than buried.** `AGENTS.md:5` forbids gameplay, Unity scene production and Quaternius integration before `WP-HK-GATE` passes. A combat feel spike is all three. This document does **not** authorize it and cannot: the decision belongs to the human, and running it would be a deliberate, time-boxed, throwaway exception whose output is a go/no-go rather than product code. The precedent for the shape is `WP-ART-00` — non-foundational, outside H0, gating nothing, producing direction rather than product.

## 12.9 Where the rabbit hole actually starts

The unbounded commitment is not combat. It is **twelve unstarted research tracks**, each with mandatory prior art, preregistered hypotheses and an exit criterion, ahead of a five-person team with no animator. That is the largest open-ended obligation anywhere in the recovered plan.

One prototype that answers the binding question is the opposite of a rabbit hole. It is the cheapest way to find out whether the other eleven are worth opening.

---

# 13. Open reconciliation items

| Item | Current state | Desired future decision |
|---|---|---|
| Quaternius timing | current roadmap puts import/select in H2 | explicitly move a tiny representative real-asset slice into H1 when post-GATE roadmap is authored |
| Quaternius tier | visual bible says free tier | decide free vs Source after exact dependency/license review |
| town layout | first draft was a radial test hub | use the product-seed river/bridge/loops topology in this revision |
| prefab catalogue authority | first draft proposed canonical `WorldObject` catalogue | review in H1; require discoverability without pre-deciding storage authority |
| phase order | first draft put cinematics/QTE/combat before social simulation | Living World Core first; drama then combat; deeper agency later |
| durable authored persistence | H0 has no production project store claim | use canonical snapshot/checkpoint-based project workspace first; escalate only with evidence |
| runtime/save state | deliberately outside canonical authored journal | define separate runtime/save authority when H3/H4 requires it |
| mesh budget | approved-draft hero cap may be tighter than keeper seed | preserve cap unless an art review explicitly changes it |
| camera/render pipeline | not yet frozen | decide before real H1 asset adoption |
| player agency | asserted in `ADR-013`, `ADR-011` and `LC-03`; specified nowhere | adopt a tier-0 verb set as a design decision; do not defer it to a research track |
| combat animation | visual bible puts combat animation on the No list and caps at ~20 clips | a feel spike and the accepted scope both need combat clips; revise with the asset-tier decision |
| pre-GATE feel spike | `AGENTS.md:5` forbids gameplay and Quaternius integration before GATE | human decision on a time-boxed throwaway exception; this document does not authorize it |

---

# 14. What this document deliberately does not do

- It does not modify `ROADMAP.md`, `WP-HK-GATE`, any H0 workpack, accepted proof or accepted guarantee.
- It does not authorize H1 before GATE.
- It does not freeze final geometry or final character writing.
- It does not make real Potes the shipping map; the town remains fictional.
- It does not decree a prefab-catalogue storage model.
- It does not merge authored project state with live gameplay state.
- It does not require combat for the first convincing game slice.
- It does not claim that any design research PASS from `Arkus0/Juego` transfers to Juego2.
- It does not authorize the combat feel spike in §12.8, which collides with `AGENTS.md:5` and needs an explicit human decision.
- It does not specify the player-verb contract; §6.3 argues the gap and its cost, and leaves the design to the owner.

The intended production philosophy is simple:

> **Do not build a demo and then build the game. Build a small, keeper-quality piece of the game, and let the demo prove that piece already works.**
