# Immersive object interaction scope

Status: **OWNER_DIRECTION — canonical when merged**
Date: 2026-09-28
Related: `PORT_TOWN_SCALE_AMENDMENT.md`, `GC2_ARKUS_RUNTIME_SPLIT.md`, `WP-H2-02`, `WP-GC2-04`.

## Goal

Juego2 should support an **immersive-sim-light** relationship with the physical world: the player can affect selected objects in legible ways, and meaningful consequences can survive. This is not a requirement that every mug, chair, stone or decoration become a fully persistent simulation object.

The target feeling is: **the world is touchable, useful and reactive**, without turning object simulation into a universal infrastructure project.

## Three object classes

### 1. WORLD PROP — local physical affordance

Examples: doors, shutters, chairs, crates, switches, movable clutter, a bottle that can be nudged, a drawer that can open.

- Unity/GC2 owns immediate interaction, physics, animation and presentation.
- No inventory representation is required merely because the object can be touched or moved.
- Local state may reset when the area reloads unless a later consequence actually matters.
- Prefer reusable interaction verbs and affordance tags over bespoke code per prop.

### 2. PORTABLE ITEM — player can take/use/drop/transfer

Examples: photograph, key, tool, food/drink, parcel, evidence object, collectible, sellable/stealable item.

- Evaluate **Game Creator Inventory 2** as the preferred execution/authoring accelerator for this class when `WP-GC2-04` reaches its dependency decision.
- Inventory Item definitions describe reusable item kinds; Runtime Items represent concrete instances with per-instance values.
- The portable loop should support, where useful: pick up → carry/store → inspect/use/show/equip → transfer/give/sell/steal → drop/place back into the world.
- Not every portable prop needs unique persistent identity. Stackable/commodity items can remain generic.

### 3. CONSEQUENTIAL OBJECT — world must remember this instance or outcome

Examples: murder evidence moved from a scene, a unique key stolen from an NPC, a tool left somewhere that changes access, a package delivered to the wrong person, an object broken or removed and later noticed.

- GC2/Inventory may execute and present the interaction.
- Arkus/Juego2 owns the durable semantic consequence and stable identity when the particular instance matters later.
- GC2 Inventory IDs, runtime GUIDs or save-slot identifiers do **not** become canonical Juego2 identity.
- On reload/revisit, presentation is derived from authoritative state: present here, carried by actor, consumed/broken, transferred, unavailable, etc.

## Design rule

**Physical manipulability, portability and persistent systemic meaning are separate costs.**

An object can be physically interactive without being inventory-backed. An Inventory Runtime Item can exist without Arkus knowing a unique identity. Arkus only gains an object-level durable fact when later gameplay needs the world to remember that object or result.

This separation is the guardrail that lets the game feel materially interactive without requiring thousands of fully simulated persistent objects.

## Inventory 2 adoption boundary

`WP-GC2-04` owns the first formal Inventory 2 dependency decision because its evidence/clue loop is the earliest keeper gameplay that clearly benefits from portable persistent items. If adopted, evaluate exact version/license/provisioning and H1/H2F lifecycle compatibility before keeper use.

Inventory adoption should prove a reusable portable-object pattern, not an RPG backpack mandate. The UI may be minimal or contextual if that better serves the adventure. Merchants, crafting, equipment grids and other Inventory capabilities remain optional until a concrete game feature needs them.

## First proofs

- `WP-H2-02`: one keeper world object demonstrates a legible physical interaction pattern, but does not need Inventory 2.
- `WP-GC2-04`: one evidence/portable object proves acquire → carry/use/show → drop/transfer if relevant → changed lead → reload/revisit, and decides Inventory 2.
- Later living-block work should include selected physical props and at least one object whose changed state affects an NPC, access route or investigation outcome.

## Non-goals

- no requirement that every decorative prop be pickup-able;
- no requirement that every moved object persist forever;
- no universal rigidbody simulation outside the active area;
- no item-by-item bespoke Arkus schema;
- no requirement to expose a traditional RPG inventory screen if contextual interaction is better;
- no speculative crafting/economy system before gameplay demands it.
