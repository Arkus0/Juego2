# Juego2 Production Blueprint

Version: 0.1 — 2026-09-20

Status: **NON-BINDING proposal.** This document creates no acceptance criterion, reopens no accepted guarantee, alters no workpack contract and commits the roadmap to nothing. It does not authorize H1 or H2 work, proposes no `WP-HK-*` workpack, and does not modify the `WP-HK-GATE` precondition in `Docs/ROADMAP.md`.

## 0. Scope and standing

### 0.1 Why this exists

`Docs/ROADMAP.md:11` states: **"No serious game production begins before `WP-HK-GATE` passes."** That rule is correct and this document does not touch it.

The risk it leaves open is different. H0 is 11 of 19 workpacks complete and the next dependency-valid unit is `WP-HK-07A`. When GATE eventually passes, the entire production-side inventory is 116 lines of art direction (`Docs/art/VISUAL_BIBLE.md`, `Docs/art/SETTING.md`) plus the non-binding `Docs/engineering/CONTENT_SHAPE_BACKLOG.md`. `Docs/ROADMAP.md:123` says plainly: "Detailed H1 WPs not frozen yet."

Without preparation, the H0 → production transition would be improvised under feature pressure. That is the failure mode that cost the `Arkus0/Juego` reference archive its M1/M2 Shenmue track and its DFU-as-world-foundation bet. Preparing on paper is cheap; improvising after a gate is not.

This blueprint therefore does one thing: it recovers the useful planning that already exists, re-expresses it in Arkus's canonical vocabulary, and sizes the jump. It is input for a future roadmap author, not an instruction to anyone.

### 0.2 Standing relative to accepted contracts

| Rule | Where | Honoured here by |
|---|---|---|
| Harness-first; no production before GATE | `AGENTS.md:5`, `ROADMAP.md:11` | Nothing here starts, unblocks or reorders any `HK-*` work |
| `Arkus0/Juego` is a reference archive, never an authority | `AGENTS.md:56`, `CLAUDE.md` | Every recovered idea is cited, restated in Arkus terms, and marked as adapted — no architecture or code is imported |
| Engine types may not enter canonical contracts | `PRODUCT_ARCHITECTURE.md:88` | No `GameObject`, `Prefab`, `Scene` or `MonoBehaviour` appears in any proposed canonical shape |
| Asset/engine binding is `OUT` of the harness boundary | `CONTENT_SHAPE_BACKLOG.md` row 16 | Presentation binding is described as H1 bridge work, never as kernel work |
| Dependency/IP adoption is fail-closed | `DEPENDENCY_IP_POLICY.md` | Asset acquisition is described as a human decision requiring an exact-version record |

### 0.3 Sources

Juego2 (authoritative for current state): `Docs/ROADMAP.md` v1.19, `Docs/workpacks/HK/WP-HK-GATE.md`, `Docs/engineering/PRODUCT_ARCHITECTURE.md`, `Docs/engineering/CONTENT_SHAPE_BACKLOG.md`, `Docs/engineering/RESIDUAL_LEDGER.md`, `Docs/engineering/DEPENDENCY_IP_POLICY.md`, `Docs/art/VISUAL_BIBLE.md`, `Docs/art/SETTING.md`, `src/Arkus.Game.World/WorldState.cs`, `tests/Arkus.Harness.Tests/Hk05ContentShapeProbeTests.cs`.

`Arkus0/Juego` (reference archive only — paths below are relative to *that* repository, written `Juego/…` throughout this document to avoid confusion): `Juego/Docs/DEV_REFERENCE_TOWN.md`, `Juego/Docs/ASSET_FACTORY.md`, `Juego/Docs/PRIOR_ART_HARVEST.md`, `Juego/Docs/ROADMAP_TO_DFU_AND_QUATERNIUS_DEMOS.md`, `Juego/Docs/LIVING_WORLD_RUNTIME.md`, `Juego/Docs/GAMEFLOW_RUNTIME.md`, `Juego/Docs/GAME_FIRST_POLICY.md`, `Juego/Docs/Architecture/GAMEPLAY_SYSTEMS_ARCHITECTURE.md`, `Juego/Docs/Architecture/GAMEPLAY_DEPENDENCY_GRAPH.md`, `Juego/Docs/Architecture/GAMEPLAY_IMPLEMENTATION_ROADMAP.md`, `Juego/Docs/ROADMAP.md`, `Juego/Docs/workpacks/M12/PLAN.md`, `Juego/Docs/living-city-research/LCRT-00_CONSTITUTION.md`, `Juego/Docs/living-city-research/PA-01_NPC_DAILY_LIFE.md`, `Juego/Docs/living-city-research/PA-03_SOCIAL_GRAPH.md`, `Juego/Docs/combat-research/CCRT-00_CONSTITUTION.md`.

One calibration note about that archive: it is roughly **95% documentation**. Living world, narrative, GameFlow, cinematics, QTE, combat and the asset factory were specified in depth and never implemented. That is exactly why it is worth reading for planning and worth ignoring for architecture.

### 0.4 The two gates

Easy to misread, so stated once: `WP-HK-GATE.md:84` says a GATE PASS lets the roadmap **author** detailed Engine Bridge / Unity workpacks. `WP-HK-GATE.md:86` adds: "A PASS does **not** authorize gameplay implementation directly; Unity must first receive its own downstream parity/bridge gate." `ROADMAP.md:129` and `:158` place H2 content behind that second gate.

```text
WP-HK-GATE  ->  H1 engine bridge  ->  Unity parity/bridge gate  ->  H2 content onwards
```

Everything in sections 1–7 below sits to the right of the first arrow.

---

## 1. Town semantic plane

### 1.1 What this section is and is not

It is a **semantic** plan: which zones exist, what they contain, how they connect, roughly how big they are, and which system each zone is meant to stress. It is deliberately not geometry. No metre-by-metre layout, no street centrelines, no building footprints are designed here. Those belong to whoever builds the reference zone, after the Unity parity gate.

Sizing anchors come from `VISUAL_BIBLE.md` §4–5: **streets 4–6 m, plaza ~25×20 m, 1 m grid**. The traversal constraint is recovered from the reference archive (`Juego/Docs/DEV_REFERENCE_TOWN.md:93`): most test POIs should sit **30–90 seconds apart on foot**. That is a testing constraint, not a shipping one — it keeps a day of schedules observable without waiting in real time.

### 1.2 Identifier convention

The existing harness fixtures already seed a convention (`tests/Arkus.Harness.Tests/Hk05ContentShapeProbeTests.cs:15-31`): world `world.potes`, `place.plaza`, `building.bar`, `building.shop`, `npc.ana`, reference kind `works.at`. Extending it:

| Prefix | Meaning | Example |
|---|---|---|
| `world.` | World root | `world.potes` |
| `zone.` | Named zone / barrio | `zone.plaza` |
| `place.` | Open space or street segment | `place.calle_mayor_01` |
| `building.` | Building shell | `building.ayuntamiento` |
| `interior.` | Authorable interior space | `interior.bar` |
| `poi.` | Semantic point of interest (function, not geometry) | `poi.bar_terraza` |
| `npc.` | Persistent actor identity | `npc.antonio` |
| `prop.` | Placed prop | `prop.banco_04` |

Proposed reference kinds: `contains` (expressed as containment, not a reference), `works.at`, `lives.at`, `frequents`, `entrance.of`, `connects.to`, `serves.at`.

### 1.3 Zones

Five zones — a civic core plus a belt. 25 POIs and 6 authorable interiors.

| Zone | Character | POIs | Approx. extent |
|---|---|---|---|
| `zone.plaza` | Plaza Mayor and civic core; the social hotspot | `poi.plaza_mayor`, `poi.ayuntamiento`, `poi.iglesia`, `poi.bar_terraza`, `poi.fuente`, `poi.mercado`, `poi.bancos_plaza` | Plaza ~25×20 m; 4 building frontages |
| `zone.calle_mayor` | Commerce spine running off the plaza | `poi.tienda`, `poi.panaderia`, `poi.farmacia`, `poi.bar_puerta`, `poi.portal_residencial` | ~90 m long, 4–6 m wide |
| `zone.barrio_alto` | Stepped residential lanes above the plaza | `poi.casa_antonio`, `poi.casa_carmen`, `poi.casa_ana`, `poi.lavadero`, `poi.patio_alto` | ~60 m of lanes, 6–8 m rise |
| `zone.ribera` | Working edge toward the water; reserves the later river | `poi.taller`, `poi.almacen`, `poi.puente`, `poi.huerta` | ~70 m along the bank line |
| `zone.entrada` | Arrival and outward edge | `poi.parada_autobus`, `poi.carretera`, `poi.mirador`, `poi.camino_cementerio` | ~50 m of road + verge |

Total walkable footprint: roughly **250 × 180 m**. Building frontage 8–12 m, depth 8–10 m, 2–3 floors at 3–3.5 m. Interiors are one-room-scale, ~6 × 8 m with 3 m ceilings.

The river itself and the small landing stay **deferred**, exactly as `SETTING.md` places them ("Later"). `zone.ribera` reserves the slot so that adding water later is an extension, not a re-plan. `poi.camino_cementerio` is likewise a path stub, not a built zone — the cemetery is recovered from the archive's slice as a future beat, not as H1/H2 content.

### 1.4 Connections and routes

```text
                zone.barrio_alto
                       |
                   (steps)
                       |
zone.entrada --- zone.plaza --- zone.calle_mayor
                       |
                   (bajada)
                       |
                  zone.ribera
```

The plaza is the single hub: every other zone touches it and nothing else. That is deliberate. It keeps schedule convergence observable (everyone crosses one readable space), keeps traversal inside the 30–90 s window, and means one small zone carries the demo weight.

Indicative on-foot times from the plaza: tienda ~25 s, casa Antonio ~45 s, taller ~60 s, parada de autobús ~75 s, extremes ~90 s.

### 1.5 Interiors

Six authorable interiors, which is what the systems need rather than what dressing would want: `interior.bar`, `interior.tienda`, `interior.ayuntamiento_oficina`, `interior.casa_antonio`, `interior.taller`, and optionally `interior.iglesia`.

`VISUAL_BIBLE.md` §8 asks for exactly **one** bar/shop interior in the H2 hero. The three beyond that (`casa_antonio`, `ayuntamiento_oficina`, `taller`) exist because home, work and institution are three different schedule and knowledge contexts, and testing all three in one room hides bugs. They are cheap: an interior at this scale is a room shell plus a dozen props.

### 1.6 What each zone is for

| Zone | Systems it is designed to stress |
|---|---|
| `zone.plaza` | Containment depth; dense typed references; social hotspot behaviour; schedule convergence; ambient population; the "is this readable at 15 m" art test |
| `zone.calle_mayor` | Traversal and navigation; POI opening hours (interval data crossing boundaries); contextual interaction; smart-object slots and reservation contention |
| `zone.barrio_alto` | Home assignment; night and sleep schedule blocks; vertical navigation over steps; abstract off-screen simulation when the player is elsewhere |
| `zone.ribera` | Work POIs with capacity; reservation conflicts; extension versioning rehearsal for the later river; kit stress outside the hero street |
| `zone.entrada` | Arrival and departure flow; zone and interior transitions; save/load anchoring; seeding knowledge from outside the town |

### 1.7 Sizing against measured cost

`Docs/engineering/RESIDUAL_LEDGER.md:99-110` records an exploratory probe over a synthetic **town-shaped** world (containment tree of branching factor 8, one typed reference per 10 objects, one object-scoped extension per 5):

| Objects | Validate + serialize + hash | Canonical bytes |
|---|---|---|
| 1 000 | 17 ms | 92 KiB |
| 10 000 | 69 ms | 958 KiB |
| 25 000 | 192 ms | 2.4 MiB |
| 200 000 | 2 475 ms | 19.4 MiB |

Rough estimate for the plane above: ~30 place/street objects, ~28 building shells, 6 interiors at ~12 objects each, 24 POIs, 400–900 props and street furniture, 200–600 vegetation objects, 13 NPCs, plus one presentation-binding extension per placed object. That lands between **~1 200 objects (undressed)** and **~2 500 objects (dressed)** — a **17–40 ms** whole-world commit.

Two conclusions. First, this town does not make world partition (`CONTENT_SHAPE_BACKLOG.md` row 14) a live decision; `RESIDUAL_LEDGER.md:116` sets that threshold at "high tens of thousands". Second, the ledger is explicit that this measurement "is **not** evidence" — it is not SHA-bound and ran on shared CPU. It is used here to bound ambition, not to claim performance.

---

## 2. Asset strategy

### 2.1 Categories, coverage and gaps

| # | Category | Quaternius Source coverage | Gap to adapt or author |
|---|---|---|---|
| 1 | Ground and paving | Medieval Village (partial), Downtown (kerbs) | Worn Cantabrian stone paving; wet variants; damp kerbs |
| 2 | Masonry walls | Medieval Village — strong | Damp course; mid-stone recolor; strip medieval signage language |
| 3 | Roofs | Medieval Village — shape only | Wet dark tile at `#4A3730`; thatch is vetoed outright |
| 4 | Openings (doors, windows, shutters) | Medieval Village + Downtown | Shutter green-blue `#3E5A57`; Spanish proportions |
| 5 | **Balconies / galerías** | none | **Author.** The single strongest Potes silhouette signature |
| 6 | Street furniture | Downtown (modern), Medieval (rustic) | Recolor pass; Spanish signage and shopfront lettering |
| 7 | Market props | Survival (crates), Medieval (stalls) | Produce variants; market-day dressing |
| 8 | Interior kit | Medieval Village (300+ modules incl. interiors) | Contemporary rural bar and shop fittings |
| 9 | Vegetation | Stylized Nature (116 assets) | Hydrangeas (`#6F79B8`); Picos-adjacent rock masses; no palms |
| 10 | Characters | Universal Base Characters (6 bodies, 20 hairstyles) | Facial variety within the "no realistic regional physiognomy" rule |
| 11 | **Wardrobe** | none | **Author.** Already flagged as the known gap in the archive's asset plan |
| 12 | Animation | Universal Animation Library 1 + 2 (250+ clips) | Bicycle; paired interactions; acting and contextual gesture |
| 13 | Vehicles | Downtown (partial) | **Author** bus and bicycle; both are slice-critical |
| 14 | River craft | Ships pack | Deferred with the river itself |
| 15 | Cemetery elements | none | **Author** niches/wall tombs; must not read as fantasy graves |

Rows 5, 11, 13 and 15 are where identity actually lives. Everything else is raw material. The archive's production rule is worth keeping verbatim in spirit: **buy or reuse the generic; author the memorable.**

### 2.2 Acquisition

Per the decision recorded for this blueprint, the paid **Source** tier is assumed, at the prices observed in the archive on 2026-09-16 (re-verify at checkout):

| Pack | Observed | Role |
|---|---|---|
| Universal Base Characters | USD 19.99 | 6 base bodies, 20 hairstyles, humanoid rig |
| Universal Animation Library | USD 14.99 | 120+ clips, root-motion and in-place |
| Universal Animation Library 2 | USD 14.99 | 130+ clips |
| Medieval Village MegaKit | USD 14.99 | 300+ building and interior modules |
| Stylized Nature MegaKit | USD 14.99 | 116 vegetation and rock assets |
| *(optional)* Downtown City MegaKit | USD 14.99 | Modern street furniture and shopfronts only |

Core five ≈ **USD 79.95**; with Downtown ≈ **USD 94.94**; cap **100 EUR at checkout**, Downtown deferred if the cap is exceeded.

Three constraints that do not relax:

1. `DEPENDENCY_IP_POLICY.md` requires exact-version license reverification before adoption, plus a full dependency record. "A README statement seen during research is not sufficient adoption evidence."
2. `VISUAL_BIBLE.md` §9–12 currently reads **`UNVERIFIED-FOR-ADOPTION` until human pack record**. Acquisition is a human action.
3. `WP-ART-00.md` forbids binary assets in the repository, and `Docs/art/Refs/README.md` forbids pack ZIPs. Source archives stay outside the repo; only manifests, hashes, licenses, recipes and validated transformed outputs are versioned.

Worth stating precisely, because it narrows the disagreement: the archive's prior-art catalogue classifies the Quaternius packs as **CC0**. The paid itch.io tier does not buy different licence terms — it buys the **Source** distribution (`.blend` files, shaders, colliders, Unity/URP projects). For a project whose whole asset plan is transformation rather than direct use, source files are the difference between a recipe and a guess.

Two further CC0 seed libraries recorded in the archive are worth keeping in view, and neither costs anything: **Kenney** retro urban and road kits, and the **ALEX Modular PSX Building Asset Pack** (188 modular assets, explicitly PS1/PS2-era). The second is arguably closer to the visual bible's "late-PS2 / early-PS3 shape language reinterpreted" than the Medieval kit is, and deserves a place in the first triage pass rather than being assumed away.

**This contradicts the repository as written.** `VISUAL_BIBLE.md` §6–7 says "Quaternius free tier". That divergence is recorded in §10 below, not silently resolved here.

### 2.3 Representative asset slice for H1

The point of the H1 slice is to prove the Unity bridge against **real** assets rather than primitives — real atlases, real pivots, real rigs, real colliders. It should be as small as possible while still being real.

| Piece | Count | What it proves |
|---|---|---|
| Wall modules | 3 | Modularity, grid snapping, atlas sharing |
| Roof section | 1 | Material policy, the wet-tile treatment |
| Door | 1 | Semantic socket, interior transition anchor |
| Window + shutter | 1 | Accent colour discipline (≤5% surface) |
| Bench | 1 | Prop placement, smart-object slot anchor |
| Street lamp | 1 | Vertical prop, lighting assumption |
| Tree | 1 | Vegetation import, billboard/LOD policy |
| Rock mass | 1 | Silhouette read at distance |
| Base character | 1 | Humanoid rig, scale, retarget |
| Animation clips | 3 | idle / walk / sit — the civilian allowlist floor |

Roughly **14 source assets → ~10 transformed outputs**: enough for one street corner plus one NPC. Sufficient to exercise import, scale and pivot conventions, atlas and material policy, collider generation, navigation surface, humanoid retarget, and harness → engine reference resolution end to end.

Deliberately **not** in the H1 slice: wardrobe, vehicles, galerías, market dressing, interiors beyond a doorway. Those are H2 concerns and would turn a bridge proof into a content push.

---

## 3. Prefab and composition strategy

### 3.1 From modules to buildings

Six levels, each a consumer of the one below:

| Level | Unit | Example |
|---|---|---|
| L0 | Module — a single mesh piece, no semantics | wall bay, roof section, door leaf |
| L1 | Assembly — a small fixed grouping | ground-floor bay with door, window bay with shutters |
| L2 | Building shell — facade grammar applied | ground floor + 1–2 upper floors + roof |
| L3 | Building prefab — shell plus semantic sockets | door socket, window sockets, sign socket, interior link |
| L4 | POI prefab — building plus function | smart-object slots, capacity, opening rules |
| L5 | Segment — a street or block run | straight run, corner, stepped run, plaza edge |
| L6 | Zone — segments plus dressing | `zone.calle_mayor` |

### 3.2 Prefabs worth making reusable

`casa_base` in three variants; `casa_con_galeria`; `bar_esquina`; `tienda_planta_baja`; `ayuntamiento`; `iglesia`; `taller`; `puente`; market stall; bar terrace set; street segment (straight, corner, steps); plaza paving set.

Twelve or so families. With three variants each and rotation, that covers ~28 building shells without visible repetition at the readability distance the visual bible sets (15 m).

### 3.3 How Arkus should see them — the part that must not be copied

The reference archive routed prefab knowledge through a Unity-aware domain. Arkus cannot do that: `PRODUCT_ARCHITECTURE.md:88` forbids `GameObject`, `Scene`, `Prefab` and `MonoBehaviour` in canonical contracts, and `CONTENT_SHAPE_BACKLOG.md` row 16 marks asset and engine binding `OUT` of the harness boundary.

The proposal that respects both:

- **Arkus authors semantics only.** A building is a `WorldObject { Id, TypeId, ContainerId, References[] }` (`src/Arkus.Game.World/WorldState.cs:58`). Its composition, dressing and prefab identity live in an object-scoped `WorldExtensionData` payload that the kernel treats as opaque bytes.
- **The bridge owns resolution.** A scoped capability contributed by the Unity bridge resolves a presentation binding to a concrete prefab. Per `PRODUCT_ARCHITECTURE.md:56-66`, it registers through the Arkus-owned composer and appears in the single composed canonical inventory — never as a parallel registry.
- **Dependencies are derived, not hand-kept.** `ROADMAP.md:125` already commits to this: when a schema-aware producer understands references embedded in its payload, it must derive the typed dependency surface mechanically. An agent authoring `casa_base` should not also have to remember to list every asset it references.

Discovery and modification for Arkus then work the ordinary way, with no new mechanism: `system.describe` for the capability surface; `world.object.query` to find every `town.building` in a zone; `world.extension.read` to read a composition document; `authoring.change.plan` → `validate` → `apply` to change one; `authoring.diff.compare` to see what a dressing pass did.

### 3.4 Catalogue discovery

An agent asked to "add a shop to Calle Mayor" needs to find `tienda_planta_baja` without reading C#. The proposal is that the prefab catalogue is itself authored canonical content — catalogue entries as `WorldObject`s with typed references to their modules — so it is queryable through the same read surface as the town. No second catalogue format, no engine-side manifest that the harness cannot see.

---

## 4. NPC foundation

### 4.1 Roster

Thirteen persistent actors. The names Antonio, Manolo, Carmen and Paco are recovered from the reference archive, where they are already the shared vocabulary of the social-graph research; `npc.ana` already exists in the Juego2 harness fixtures. Archetypes in brackets are the eight from `VISUAL_BIBLE.md` §4–5.

| Id | Role [archetype] | Home | Work | Frequents |
|---|---|---|---|---|
| `npc.ana` | Bar server [bar server] | `poi.casa_ana` | `poi.bar_terraza` | `poi.plaza_mayor` |
| `npc.antonio` | Valley worker; the deep reference actor [valley worker] | `poi.casa_antonio` | `poi.taller` | `poi.bar_terraza` |
| `npc.manolo` | Bar owner; opinionated [neighbour] | above the bar | `poi.bar_terraza` | `poi.plaza_mayor` |
| `npc.carmen` | Shopkeeper; Manolo's sister [shopkeeper] | `poi.casa_carmen` | `poi.tienda` | `poi.mercado` |
| `npc.paco` | Builder [builder] | `zone.ribera` | rotating sites | `poi.bar_terraza` |
| `npc.rosa` | Neighbour, dog walker [dog walker] | `zone.barrio_alto` | — | `poi.plaza_mayor`, `poi.mirador` |
| `npc.teresa` | Panadera [valley worker] | above the bakery | `poi.panaderia` | `poi.mercado` |
| `npc.javier` | Teen with a bike [teen+bike] | `zone.barrio_alto` | — | `poi.plaza_mayor`, `poi.carretera` |
| `npc.lucia` | Municipal clerk [shopkeeper] | `zone.calle_mayor` | `poi.ayuntamiento` | `poi.fuente` |
| `npc.bernardo` | Outsider; arrives by bus [outsider] | — (no home in town) | — | `poi.parada_autobus`, `poi.bar_terraza` |
| `npc.pilar` | Elderly neighbour, plaza bench [neighbour] | `zone.barrio_alto` | — | `poi.bancos_plaza`, `poi.iglesia` |
| `npc.guardia` | Municipal officer [builder silhouette, distinct outfit] | outside town | `poi.ayuntamiento` | patrol route |
| `npc.tomas` | Farmhand from the outskirts; market days only [valley worker] | outside town | `poi.huerta` | `poi.mercado` |

Plus three ambient archetypes that are deliberately **not** actor identities and carry no persistent knowledge or relationships: `ambient.shopper`, `ambient.walker`, `ambient.worker`. Promotion from ambient to persistent must be explicit, never implicit — that rule is worth keeping from the archive.

### 4.2 Identity independent of presentation

The separation the archive got right, restated in Arkus terms:

```text
npc.antonio                        <- WorldObject, TypeId town.npc
  references                       <- lives.at, works.at, frequents
  extension arkus.npc-profile@1    <- opaque authored profile/schedule document
  extension <bridge>.presentation  <- H1-owned binding to a prefab
```

Swapping the presentation extension changes the model on screen. It changes no identifier, no reference, no profile, no schedule, no knowledge and no relationship. `npc.antonio` can be a grey capsule in H1, a transformed base character in H2 and final art later, and nothing downstream notices.

This costs nothing new: `CONTENT_SHAPE_BACKLOG.md` rows 1, 3 and 4 are already `COVERED`, and row 16 keeps the binding itself outside the kernel.

### 4.3 What each character is there to prove

| System | Characters | Why these |
|---|---|---|
| Schedules | Ana (baseline day), **Teresa** (pre-dawn block crossing midnight — backlog row 10), **Tomás** (weekly market cadence) | Three different interval shapes, not three copies of one |
| POI contention | Paco, Antonio, Tomás | Rotating and shared work slots force reservation conflicts |
| Dialogue | Carmen, Lucía | Shop counter and institutional desk are different gating contexts |
| Knowledge | **Manolo** (source) → **Pilar** (witness and relay) → **Antonio** (consumer) | A three-hop provenance chain is the minimum that makes provenance visible |
| Relationships | Antonio / Manolo / Carmen / Paco | Recovered from the archive's social-graph research: Carmen is Manolo's sister but does not share his opinions; Paco owes Carmen a favour that expires once spent. Both cases break naive affinity-only models |
| Living World | Ana, Antonio, Manolo, Rosa around the plaza/bar hotspot | Enough density for emergent overlap in one readable space |
| Abstract simulation | Tomás | Lives outside the full-simulation area by design |
| GameFlow authority | Bernardo (arrival), `npc.guardia` (event override) | Entering and interrupting are the two authority transitions worth proving early |
| Bicycle locomotion | Javier | The one non-pedestrian movement in the slice |

### 4.4 Deliberately absent

No backstories, no dialogue lines, no quest structure, no faction alignments, no relationship values. One line of role per character is enough to develop the town and exercise the systems. Writing narrative before the dialogue runtime is decided is listed as a rabbit hole in §9 — `CONTENT_SHAPE_BACKLOG.md` row 17 marks dialogue and narrative runtime content `OUT`, pending a later reviewed architecture decision.

---

## 5. System dependency map

### 5.1 The one adaptation that matters

The reference archive's dependency graph is good and mostly recoverable. But its `WorldState` was a **runtime** kernel holding a clock, flags, facts, beliefs and relationships, all ticking.

In Arkus, `WorldState` is **authored** canonical state, and `WP-HK-06A` drew a hard authored/live boundary that is now an accepted guarantee. Residual `R-06B-04` records it: gameplay and runtime state — transforms, clocks, schedules, physics, animation, AI, simulation — stays outside canonical `WorldState` and replay unless a later reviewed workpack promotes it.

Copying the archive's graph as-is would re-merge the two and break that guarantee. So every layer below carries the split explicitly:

| | Arkus authors | The runtime owns |
|---|---|---|
| Time | schedule definitions, opening hours | the ticking clock, current time |
| Actors | identity, home/work references, profile | position, current activity, animation state |
| Knowledge | fact definitions, authored starting beliefs | live beliefs, who currently knows what |
| Relationships | initial edges, relationship kinds | current values after play |
| World | the town, its objects, its structure | which door is open right now |

`R-06B-04` has **no owner named** in the ledger. That is the highest-value unowned question standing between here and production, and this document flags it rather than answering it.

### 5.2 Layers

Ordered so nothing depends forward. Backlog rows are tagged where a layer first needs an `OPEN` shape.

| L | Layer | Contents | Depends on | Backlog rows |
|---|---|---|---|---|
| L0 | Canonical kernel | contract, world state, inspection, transactions, validation, provenance, diff, replay | — | 1–5, 11, 12 `COVERED` |
| L1 | Host and protocol | headless host, reference transport, MCP projection, batching, recovery, containment, limits, endurance | L0 | — |
| L2 | Engine bridge | scoped capability composition; presentation binding; deterministic projection of canonical state into engine artifacts; parity evidence | GATE | 16 (`OUT`) |
| L3 | World composition | modular kit ingestion; prefab and catalogue contracts; spatial/transform authoring; navigation as a **derived artifact**, not authored state | L2 | 9 |
| L4 | Playable shell | player movement, camera, contextual interaction, zone and interior transitions | L3 | 18 (`OUT`) |
| L5 | Actors and presentation | actor identity, presentation binding, animation port, ambient population tier | L3, L4 | 3, 4 |
| L6 | Time and routine | clock; schedule intervals; POIs and smart objects; reservations; activity intents; behaviour resolver v0 | L5 | **10** |
| L7 | Narrative and flow | world flags and stages; condition evaluation; dialogue adapter; GameFlow authority; structured outcomes | L6 | **7**, 17 (`OUT`) |
| L8 | Drama | cinematics; QTE; combat | L7 | — |
| L9 | Social simulation | knowledge and beliefs; relationships; events and memory; autonomous agency; abstract off-screen simulation | L6, L7 | **8**, **13**, 6 |
| L10 | Integration | runtime save/load; the vertical slice | all | 14, 15 |

### 5.3 Edges worth stating because they are counter-intuitive

- **Navigation depends on world composition, not on actors.** A navigable surface is a property of built geometry. Building it after actors exist is the usual ordering mistake.
- **Schedules depend on POIs and time, not on dialogue.** An NPC can live a full day before it can say a word. The archive's spike track proved this ordering works.
- **Abstract off-screen simulation depends on activity intents and explicitly not on navigation or animation.** If it needs a path or a clip, it is not abstract. An abstract action must still produce the same kind of domain outcome as its full-simulation equivalent.
- **Combat depends on GameFlow authority, animation and actors** — never directly on narrative. Narrative consumes the outcome.
- **QTE depends on GameFlow and cinematics**, not on combat. It is an input-authority mechanism that combat happens to use.
- **Autonomous agency depends on knowledge, relationships, schedules and events together.** It is the last thing, not an early one. Building a general decision engine before one NPC lives one day is listed as a rabbit hole in §9.
- **Runtime save/load is a separate contract from authored persistence.** HK-06A/B/C solved journal, snapshot and replay for **authored** state. Live simulation state is a different problem with a different lifetime, and reusing the canonical mechanism for it would violate the boundary in §5.1.

### 5.4 A trap inside the archive itself

The reference archive contains **two gameplay architectures, written a day apart, that do not compose**:

- **A — the deterministic kernel** (`Juego/Docs/Architecture/GAMEPLAY_SYSTEMS_ARCHITECTURE.md`, `Juego/Docs/Architecture/GAMEPLAY_DEPENDENCY_GRAPH.md`, `Juego/Docs/Architecture/GAMEPLAY_IMPLEMENTATION_ROADMAP.md`). Clock, state stores, one condition language, a fixed simulation phase order, seeded randomness, decision logging, a dialogue adapter, near/mid/far simulation levels. Concrete: it carries schemas, phase orderings and binary test criteria.
- **B — the living-world charter** (`Juego/Docs/LIVING_WORLD_RUNTIME.md`, `Juego/Docs/GAMEFLOW_RUNTIME.md`, and the living-city and combat research tracks). Bounded autonomous agency, receiver-owned social decisions, belief provenance separate from engine lineage, agency budgets, presentation binding, GameFlow authority. Conceptually stronger and better argued; mostly not reduced to schemas.

They agree on fundamentals — truth versus belief, directed relationships, schedules as intent rather than paths, headless determinism, explainability, adapters at the edges. They disagree on where state lives and who decides.

Reading the archive without noticing this is how a reader ends up building half of each. The proposal here: **take A's ordering and schema discipline as the implementation substrate, take B's design laws as the charter, and take B's research deltas as the backlog** — and in both cases re-home the state according to the authored/live split in §5.1, since A's kernel assumed a runtime `WorldState` that Arkus does not have.

Two charters from B are worth recovering close to verbatim because they are cheap and they prevent expensive mistakes: the living-city laws (the city does not wait for the player; NPCs may be primary causes; actor-to-actor is first-class; no omniscient agents; schedule is baseline not destiny; agency must be explainable; bounded autonomy beats unlimited emergence) and the combat laws (combat returns state to the living world; feel requires instrumented play, not document confidence; budget choreography rather than brute-forcing animation count).

---

## 6. Recovered research: living world and combat

Two research tracks ran in the reference archive before the restart, each under its own constitution and review protocol. They are in very different states, and the difference matters more than the similarity.

### 6.1 What the archive actually holds

**Living city** — `Juego/Docs/living-city-research/`

| Unit | Lines | Status in the archive | Output |
|---|---|---|---|
| `LCRT-00_CONSTITUTION.md` | 177 | accepted | 15 laws, `LC-01`…`LC-15` |
| `PA-01` NPC daily life | 773 | `DELTAS_READY` | 11 findings, 8 deltas, a 24-hour reference routine |
| `PA-02` NPC agency | 1 068 | `DELTAS_READY — INDEPENDENT PASS; MERGED` | 8 deltas, social-action vocabulary, agency profiles, budget dimensions, a 16-entry failure register |
| `PA-03` Social graph | 1 020 | `DELTAS_READY — INDEPENDENT PASS; MERGED` | 11 findings, 10 deltas |
| `PA-04` Knowledge and belief | 849 | `DELTAS_READY — INDEPENDENT PASS; MERGED` | 10 findings, 10 deltas, 6 acceptance scenarios |
| `PA-05` Rumours | 1 096 | `DELTAS_READY — CANDIDATE; REVIEW REQUIRED` | 10 findings, 10 deltas, 8 acceptance scenarios |
| `PA-06`…`PA-12` | ~170 each | `NOT_STARTED` | preregistered plans only |

Roughly **4 800 lines of findings and 46 deltas**, three of the five worked tracks carrying an independent PASS.

**Combat** — `Juego/Docs/combat-research/`

| Unit | Lines | Status | Output |
|---|---|---|---|
| `CCRT-00_CONSTITUTION.md` | 240 | accepted | 20 laws, `CC-01`…`CC-20`, plus a crowd proof and a duel proof |
| `C-PA-01`…`C-PA-12` | ~170 each | **all `NOT_STARTED` — plan preregistered** | no findings, no deltas |

The asymmetry is worth stating plainly rather than averaging away: the NPC work was largely done and reviewed; the combat work is a charter plus twelve research plans that were never run.

### 6.2 Three layers, three different answers

**Layer 1 — the charters transfer whole.** `LC-01`…`LC-15` and `CC-01`…`CC-20` are game-design decisions, not architecture: the city does not wait for the player; NPCs may be primary causes; actor-to-actor is first-class; no omniscient agents; schedule is baseline, not destiny; agency must be explainable; bounded autonomy beats unlimited emergence. On the combat side: clean hits matter; mobs are flow and masters are openings; responsiveness outranks synchronization; the environment is part of the moveset; combat returns state to the living world; feel requires instrumented play, not document confidence. `AGENTS.md:57` permits reusing process and design lessons that are engine and game independent, and these are exactly that. Cost to carry: close to zero.

**Layer 2 — the findings transfer with translation.** Truth is not belief; relationship is not opinion; the receiver owns its own decision; relaying a rumour is a new decision rather than an automatic propagation; engine lineage and actor-accessible provenance are two different authorities. None of that depends on Unity, on the donor engine, or on any particular implementation. It describes what the simulation must be true of.

**Layer 3 — the `PROJECT_DELTAS` do not transfer as deltas.** Each delta is written as `Decision / Target / Change / Why / Acceptance / Dependencies / Remote class / Risk / Do not`. Three of those fields are archive-specific and one is actively dangerous here.

### 6.3 Why the deltas need rewriting, concretely

1. **`Target` names workpacks that do not exist.** The deltas point at `WP-M9-00`, `M9-02`, `M10-01`, `M10-04`, `Docs/LIVING_WORLD_RUNTIME.md`. Juego2 has no `M*` series and no living-world runtime document. The destination has to be re-derived, not translated.
2. **`Remote class` belongs to the archive's execution model.** `REMOTE-DONE` / `REMOTE-PREP` / `LOCAL-UNITY` classify work against the donor project's cloud/local split. Juego2 does not have that split and should not acquire it.
3. **Every delta assumes a runtime `WorldState`.** They add beliefs, relationships, reservations, goals and clocks to a ticking world state. Arkus's `WorldState` is *authored*; `WP-HK-06A` drew that boundary and it is an accepted guarantee, with `R-06B-04` recording that gameplay and runtime state stay outside canonical state and replay. A delta that says "add `ActorBelief` to `WorldState`" would, applied literally in Juego2, break an accepted guarantee. This is the same seam §5.1 runs along, and it is the single reason the deltas cannot be lifted.

What survives each delta is its `Change`, `Acceptance` and `Do not` — which is where the research value actually sits.

### 6.4 The independent PASSes do not travel

`PA-02`, `PA-03` and `PA-04` passed independent review, but under the archive's own proof standard and bound to the archive's SHAs. Juego2 has its own `FOUNDATIONAL_PROOF_STANDARD.md` and `EXECUTION_RECEIPT_PROTOCOL.md`, and `AGENTS.md:66` plus the roadmap's fourth stop rule both forbid building on a predecessor claim that was never accepted here.

So these enter Juego2 the same way the archive's design material already enters `CONTENT_SHAPE_BACKLOG.md` — as **cited, non-binding design input**. Citing a reviewed finding is not the same as inheriting its verdict, and this blueprint claims no verdict.

### 6.5 The highest-value salvage is the "Do not" lines

They read as rabbit-hole guards bought with research rather than with a postmortem, and they are the cheapest thing in the archive to carry across. A representative sample, quoted from the deltas:

- do not create one executable day-script per actor;
- do not model every prop as a smart object;
- do not hide a global enumeration of all persistent actors inside a "bounded" query provider;
- do not build generic GOAP, or re-evaluate every agent every tick;
- do not introduce an ambient tier as an agency profile, or a second AI architecture for narratively important characters;
- do not implement a receiver's answer as a conditional inside the initiator's action;
- do not let dialogue consume rumour flags instead of querying knowledge;
- do not treat a false claim as a mutation of truth;
- do not freeze agency priority rules before the agency research is integrated.

Several of these describe mistakes that are actively tempting when a system is first built, which is exactly when the archive is least likely to be re-read.

### 6.6 Where each track attaches

| Archive track | Attaches to | Note |
|---|---|---|
| `LCRT-00` laws | the whole of L6–L9 | Adopt as a charter early; it is cheap and it constrains later choices |
| `PA-01` daily life | L6 time and routine | Its 24-hour reference routine is a ready-made schedule fixture shape, including the perturbations it demands |
| `PA-02` agency | L9 autonomous agency | The failure register and budget dimensions are more valuable than the architecture it sketches |
| `PA-03` social graph | L9 relationships | Carries the sibling-is-not-trust and expiring-obligation cases already used in §4.3 |
| `PA-04` knowledge | L9 knowledge | Backlog row 13 is the modelling decision this track presumes |
| `PA-05` rumours | L9 events and memory | Least mature of the worked tracks; review was never completed |
| `CCRT-00` laws | L8 drama | The charter is usable now; the twelve plans are a backlog, not findings |
| `PA-06`…`PA-12`, `C-PA-01`…`C-PA-12` | later | Twenty-four preregistered research plans. Useful as a backlog with questions already framed, and as evidence of how much was deliberately left unanswered |

### 6.7 What this means for sequencing

Nothing here changes §7. The research is design input, and it lands in phases that are several gates away. The one thing worth doing early is Layer 1: adopting the two charters costs a reading and prevents the expensive mistakes, whereas re-deriving them after building the systems costs a rewrite.

The one thing worth *not* doing early is treating `PA-02` as an implementation plan. Its own conclusion, and this blueprint's §9.5, agree: a general agency engine before one NPC has lived one full day is the classic way to spend a phase and have nothing to show.

---

## 7. Suggested post-GATE phases

Seven phases, sized so each ends in something a person can watch. **No workpacks are drafted here** — that is the roadmap author's job, after GATE.

| Phase | Scope | Demo worth showing |
|---|---|---|
| **H1** | Engine bridge foundation; presentation binding; representative asset slice (§2.3) | A harness command changes canonical state; the engine reflects it deterministically; parity report; one street corner and one character standing in real assets |
| *(gate)* | **Unity parity / bridge gate** — per `WP-HK-GATE.md:86` | — |
| **H2** | World composition from the modular kit; player movement, camera, interaction shell; the reference zone | Walk the plaza, Calle Mayor and one interior in third person under overcast light |
| **H3** | Actors, presentation binding, time, schedules, POIs, smart objects | Six NPCs live a full day; time-lapse; a readable explanation of why each one is where it is |
| **H4** | Flags and stages, conditions, dialogue adapter, GameFlow authority, structured outcomes | A conversation gated on something the player knows, with a consequence that is still there later |
| **H5** | Cinematics, QTE, combat | One sequence that takes every authority — player, camera, actors — and returns all of them cleanly, including on skip and on failure |
| **H6** | Knowledge, relationships, events and memory, autonomous agency, abstract simulation | Before/after state diff across an event; one NPC behaves differently and can say why |
| **VS** | Vertical slice — integration only, no new systems | The full chain, end to end (§8) |

Running in parallel: the **H0S** post-GATE scale and concurrency track already defined in `ROADMAP.md:105-117`. It is explicitly not a prerequisite for starting H1, and the §1.7 sizing suggests this town will not trigger it.

Two notes on sizing. H2 and H3 are the phases where a project like this usually stalls, because they are the first that require content rather than contracts — budget accordingly. H5 is the most droppable: §8 gives a reduced slice that survives without it.

---

## 8. Vertical-slice target

### 8.1 The minimum that proves this is the game

Place: `zone.plaza` + `zone.calle_mayor` + `interior.bar`. A recognisably Liébana street — wet stone, dark tile, green slopes and rock on the skyline, overcast key light — built from real transformed assets, not primitives.

Cast: six NPCs in full simulation (Ana, Antonio, Manolo, Carmen, Pilar, Javier), the rest abstract.

Chain, observable start to finish:

```text
arrive at the plaza
  -> enter the bar, talk to Manolo
  -> learn one fact, with provenance
  -> a short dramatic beat
  -> outcome persists
  -> later: Carmen speaks or behaves differently
     because Manolo told her
  -> the harness can explain the whole chain
```

The last two steps are the ones that matter. Anything can show a conversation. What separates a game from a framework demo is that something said in a bar at midday changes what a different person does that evening, and that the causal chain is inspectable rather than scripted.

### 8.2 Reduced slice if drama is not ready

If H5 has not landed, drop the dramatic beat and keep:

```text
arrive -> dialogue -> knowledge transfer
       -> schedule or behaviour change
       -> different dialogue later
```

That alone demonstrates the game. The combat and QTE beat makes it a better demo, not a more convincing one. Saying so now is cheaper than discovering it under deadline.

### 8.3 Budget reconciliation

`VISUAL_BIBLE.md` §8 caps the H2 hero at **≤120 meshes, ≤6 atlases, ~20 animation clips, 6 NPCs**. The slice above stays at 6 full-simulation NPCs, but two zones plus a bar interior plus a bus and a bicycle will likely need **~160 meshes**.

That is a real divergence from an approved-draft budget, recorded in §10 rather than quietly exceeded. The cap may well be the right number; if so, the slice should shrink to one zone rather than the budget being stretched to fit.

---

## 9. Do not overengineer

### 9.1 Needed before H1 starts

All cheap, all documentation-only, all able to run in parallel with the remaining H0 work without touching it:

| Item | Why it is first |
|---|---|
| Decide backlog rows **7** (namespaced state), **8** (pair state), **10** (interval wrap-around) as *decisions*, not implementations | These are precisely the three shapes the town plan demands first, and `CONTENT_SHAPE_BACKLOG.md:59` notes no accepted probe has exercised rows 6–10 |
| Create the human Quaternius pack record required by `DEPENDENCY_IP_POLICY.md` | Nothing can be imported without it, and it is fail-closed |
| Reconcile the free-tier vs paid-Source divergence (§10) | An asset plan built on the wrong tier wastes the triage pass |
| Fill the six missing `Docs/art/Refs/` folders | `SOURCES_INDEX.md` declares eight; only `01_Town_Fabric` and `08_Anti_Refs` exist on disk. The two most useful absentees are `05_Characters_Outfits` and `07_Style_Targets` |
| Settle the town identifier convention (§1.2) | The HK-02A probe used `building.tienda` and `plaza.mayor`; HK-05 onward use `building.shop` and `place.plaza`. Cheap now, expensive after content exists |
| Decide camera and render pipeline | Neither is specified anywhere in the repository today. Both shape every asset decision |

### 9.2 Two observations for the roadmap author

Recorded as observations, explicitly **not** as proposed workpacks:

- **Canonical authored state is currently process-local.** Residual `R-04-01` records no durable WAL, recovery or cross-process persistence claim; snapshot export/import is the only portability mechanism. Authoring a ~2 500-object town across sessions implies durable canonical storage somewhere. This document names the implication; who owns it and when is not its call.
- **`R-06B-04` has no owner.** The authored/live boundary is exactly the seam the whole system map in §5 runs along. It is the single most valuable unowned question for the production jump.

Separately, and flagged only: `Docs/workpacks/README.md` still prints the pre-split chain (`… HK-07B → HK-08 → HK-09 → HK-10 → HK-GATE`), which no longer matches `ROADMAP.md` v1.19 after the HK08/HK09 split. Documentation drift, not a contract problem.

### 9.3 Needed during H1 and H2

Presentation-binding contract and asset lineage; spatial and transform authoring; prefab composition and catalogue contracts; navigation as a derived artifact; the interaction shell; zone and interior transitions; the ambient population tier.

### 9.4 Can wait

The river and the landing; the cemetery zone; combat; QTE; cinematics; weather events; localization (row 15); economy; crowd density tuning; the Creator GUI; valley-scale world partition (row 14); final art.

### 9.5 Tempting ideas that would start another rabbit hole

Each of these is attractive, defensible in isolation, and has a precedent worth remembering.

| Temptation | Why it is a trap | Precedent |
|---|---|---|
| Letting process machinery outgrow the product | Proof standards, review protocols and orchestration are load-bearing here and should stay. The failure mode is different: they keep growing after they have stopped reducing risk, and the game never starts | The archive's own `Juego/Docs/GAME_FIRST_POLICY.md` is a written self-correction saying exactly this had happened, and cancelled its own mandatory pre-milestone chain |
| Reverse-engineering a reference game's systems for fidelity | Unbounded scope, no shippable output, and the fidelity never transfers | The archive's M1/M2 track, closed by pivot |
| Adopting a whole external world foundation to "get a town for free" | The foundation's model becomes the ceiling, and removing it later costs more than building small | `AGENTS.md:58` already ruled DFU off the critical path |
| Building the Creator GUI before the contracts are used in anger | A GUI over unexercised contracts hardens the wrong shapes | The archive correctly deferred its Creator track behind the slice gate |
| A general Utility-AI or GOAP agency engine up front | You cannot tune a decision system against zero observed behaviour | The archive's own agency research warns against exactly this, by name |
| A full deterministic asset-transformation pipeline before ~10 assets have shipped end to end | Pipeline cost is paid before any evidence about what the pipeline needs to do | The archive's asset-factory plan gates production behind a bootstrap for this reason |
| Per-resource CAS, Merkle trees or multi-agent concurrency | `ROADMAP.md:113` is explicit: these are options only if evidence justifies them, "not the definition of the solution" | H0S exists precisely to measure first |
| Full event sourcing for world memory | Every event forever, to answer questions a snapshot plus selected events already answers | The archive concluded snapshots + relevant events suffice |
| Modelling the whole valley | The town core is ~2 500 objects; partition becomes live in the high tens of thousands | `RESIDUAL_LEDGER.md:116` |
| PBR micro-detail on flat atlas assets | Doubles asset cost and fights the style one-liner | `VISUAL_BIBLE.md` §2 "No" list |
| Writing narrative content now | Dialogue and narrative runtime is `OUT` pending a reviewed architecture decision; content written against no runtime gets rewritten | `CONTENT_SHAPE_BACKLOG.md` row 17 |

A useful test for anything not on this list: **does it make the next demo in §7 happen sooner?** If not, it can wait.

---

## 10. Open reconciliation items

Divergences this document surfaces but does not resolve. Each would be settled by a human decision or a future art-track workpack, never by this file.

| # | Item | Current repository text | This blueprint assumes | Suggested home |
|---|---|---|---|---|
| 1 | Asset tier | `VISUAL_BIBLE.md` §6–7: "Quaternius free tier" | Paid Source packs, ~USD 95, ≤100 EUR cap | A future `WP-ART-01` |
| 2 | Mesh budget | `VISUAL_BIBLE.md` §8: ≤120 meshes | ~160 meshes for the two-zone slice | Same, or shrink the slice |
| 3 | NPC count | `VISUAL_BIBLE.md` §8: 6 NPCs | 6 in full simulation, 13 persistent identities total | Compatible; worth stating explicitly |
| 4 | Town identifiers | HK-02A probe: `building.tienda`, `plaza.mayor`; HK-05+: `building.shop`, `place.plaza` | The later English convention | Settle before content exists |
| 5 | Camera and render pipeline | Not specified anywhere | Third person; pipeline undecided | H1 planning |
| 6 | Missing reference folders | `SOURCES_INDEX.md` declares 8; 2 exist | — | Art track |
| 7 | Durable canonical persistence | `R-04-01`: none, unowned | Assumed necessary for multi-session authoring | Roadmap author |
| 8 | Runtime/live state ownership | `R-06B-04`: no owner named | Assumed separate from authored state | Roadmap author |

---

## 11. What this document deliberately does not do

- It does not start, unblock, reorder or delay any `HK-*` workpack. The next dependency-valid workpack remains `WP-HK-07A`.
- It does not propose any new harness workpack.
- It does not modify `Docs/ROADMAP.md`, `Docs/art/VISUAL_BIBLE.md`, `Docs/art/SETTING.md` or any accepted contract or evidence.
- It does not import architecture, code or systems from `Arkus0/Juego`. Ideas are cited and restated; nothing is inherited.
- It does not claim acceptance, PASS, evidence or exact-SHA validation of anything.
- It does not design geometry, write narrative, or select final assets.

Its only claim is that the jump from H0 to production is now sized on paper, so that when `WP-HK-GATE` passes, the next decision is a choice between prepared options rather than an improvisation.
