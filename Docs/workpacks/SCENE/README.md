# SCENE track — Keeper-place production programme

Status: **PLANNED / NON-FOUNDATIONAL**  
Date: 2026-09-20  
Scope: product-space and scenario preproduction for streets, buildings, interiors and discovery layers

## Purpose

`CITY/` answers **what city exists and how its districts connect**. `SCENE/` answers **how an actual playable place is composed, entered, reused, discovered and later realized through Arkus + Unity**.

The track exists to close a production gap: a good urban topology does not tell us how to make a bar, house, warehouse, courtyard, alley, shop interior or layered secret space. Conversely, a scene grammar must not silently redesign the city.

Core rule:

> **The demo is not a disposable map and a scene is not just geometry. A retained place must have spatial structure, access, reuse, systemic affordances and an explicit depth budget.**

## Ownership boundary

| Track / layer | Owns | Does not own |
|---|---|---|
| `ART` | setting, visual language, asset/adaptation direction | city topology or scene semantics |
| `CITY` | scale, districts, river/crossings, mobility, location programme, seed boundary | detailed building/interior grammar |
| `SCENE` | streets/parcels, shells, interiors, access/transition structure, reusable place compositions, discovery depth | NPC cognition or canonical runtime authority |
| Living World / PA | actor behaviour, beliefs, relationships, schedules, activities and causal consequences | mesh/layout production |
| Arkus | discoverable authoring contracts, validation, composition references and safe mutation | art direction or authored product choices |
| Unity bridge | engine realization, imported assets, navigation/presentation binding | canonical product semantics |

`SCENE` specifies what information an authoring system must expose. It does **not** pre-decide whether the catalogue itself is canonical WorldState; that authority boundary remains a reviewed H1 decision.

## Place-depth model

SCENE uses a production-depth axis that is intentionally orthogonal to CITY's systemic-importance tiers.

- `S0 — scenic envelope`: visible only; no promise of access.
- `S1 — frontage/shell`: urban fabric with meaningful exterior identity; no normal interior promise.
- `S2 — shallow playable`: bounded interior/courtyard/roof/service space with limited systemic role.
- `S3 — systemic place`: persistent actors, activities, access rules, resources or consequences materially use it.
- `S4 — hero layered place`: bespoke multi-route or multi-layer space justified by repeated product value.

A CITY Tier-A location is not automatically S4. A quiet home may be S3; a visually important landmark may remain S1. This prevents 'reactive city' from becoming 'every building fully simulated'.

## Discovery layers

The city must feel deeper than its footprint without requiring Rockstar/Larian content volume. SCENE therefore distinguishes:

1. **public surface** — ordinary streets, shops, bars, plazas, waterfront;
2. **private/semi-private** — homes, back rooms, patios, staff areas, workshops;
3. **vertical/service** — roofs, stairs, upper floors, cellars, loading/service paths where justified;
4. **social/temporal** — a place becomes meaningful because of who uses it, when, and under what conditions;
5. **institutional/historical** — archives, ownership, municipal access, old uses, local history;
6. **extraordinary/cultural** — rare identity-bearing material such as martial-arts practice, kung-fu lineage, Hong Kong/Chinese cinema culture, unusual clubs or hidden expertise.

The extraordinary layer is a spice, not the whole city. The fictional Cantabrian town must remain credible if the player never discovers a martial thread.

## Secret model

A 'secret' is not synonymous with hidden room or collectible.

- **Authored secret:** a deliberately authored fact/place/object/history.
- **Systemic secret:** a discoverable situation produced by schedules, relationships, access, information or consequences.
- **Hybrid secret:** authored anchor whose discovery route/state changes through simulation.

Preferred pattern:

```text
one underlying fact/place
  -> several legitimate discovery routes
  -> knowledge/access changes what the player can do
  -> later state may differ because actors/world continued without the player
```

Examples of discovery routes include following an actor, being invited, overhearing testimony, obtaining access, reading a document, noticing a changed routine, entering from a service route or seeing a later consequence.

## Programme dependency

The planning chain is intentionally interleaved with CITY rather than duplicating it:

```text
CITY-00 PASS
   ↓
SCENE-00  Place constitution + depth/tier model
   ↓
CITY-01   Mobility/topology (consumes depth/access constraints)
   ↓
CITY-02   Systemic location programme
   ├──────────────┐
   ↓              ↓
SCENE-01       SCENE-02
streets/exterior  buildings/compositions
   └──────┬───────┘
          ↓
       SCENE-03  Interiors/access/transitions
          ↓
       SCENE-04  Discovery/secrets layering
          ↓
       CITY-03   Retained seed selection
          ↓
       SCENE-05  Exact retained-seed scenario spec
          ↓
       CITY-04   LOCAL macro blockout/traversal validation
          ↓
       SCENE-06  LOCAL realization + Arkus authoring proof
```

Parallelism is allowed only where stated by individual WPs; an upstream PASS cannot be fabricated by this diagram.

## Track-wide invariants

1. **Reuse generic structure; author the memorable.**
2. **Depth is selective.** Most buildings may remain S0/S1; deeper spaces require product justification.
3. **No secret checklist.** Every district does not need a cellar, roof and tunnel.
4. **Every core district needs at least one credible non-surface discovery vector**, physical or social/temporal.
5. **Citywide variety matters:** at least four discovery-layer families must be represented before the retained seed is frozen.
6. **Multiple discovery routes are preferred for important secrets**, but only where causal ownership makes them truthful.
7. **No omniscient reveal:** Living World knowledge/access rules remain authoritative once implemented.
8. **No catalogue authority invention:** SCENE defines discoverability requirements, not H1 ownership by fiat.
9. **No pre-GATE engine production:** SCENE-00..05 are documentation/planning only.
10. **Keeper-first:** any serious realized geometry must be designed to survive into the shipping town.

## Success condition

Before serious scene production we must be able to answer:

> Given a retained parcel/location and a product intent, what reviewed compositions, access rules, depth tier and discovery hooks should an agent be able to discover and propose, and how do we know the result belongs to the final city rather than a throwaway demo?
