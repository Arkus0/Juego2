# WP-CITY-04 — Streets, parcels + reusable building families

Status: **FROZEN PLAN / NOT_STARTED**  
Class: PRODUCT / ENVIRONMENT PREPRODUCTION (NON-FOUNDATIONAL)  
Mode: **REMOTE**  
Depends on: `WP-CITY-03` PASS  
Blocks: `WP-CITY-05` only

## Objective

Define the reusable exterior production grammar that turns CITY-00..03 decisions into streets and buildings an agent can later compose instead of inventing each façade from a blank plane.

## Work

1. Define street-segment families: ordinary street, narrow historic lane, steps, riverside/promenade, service edge, plaza/market edge, port/work edge, rural transition and relevant junction types.
2. Define parcel constraints: frontage, depth/bounds, access edges, party-wall/detached relations, slope/retaining conditions, no-build/view corridors and service/rear relations.
3. Define a compact set of reusable building families: ordinary house, mixed-use house/shop, bar/social venue, shop/service, workshop, warehouse/port building, civic/municipal, apartment/residential and selected rural/peripheral types.
4. Define composition ladder:
   `module -> assembly -> shell -> reusable building -> functional POI -> street segment`.
5. Define what Arkus must eventually be able to discover for a composition: stable ID, dimensions/bounds, sockets, access anchors, tags/archetype, dependencies, variants, style/material constraints and compatibility/version metadata.
6. Define promotion rule for a reviewed one-off composition to become a reusable family/variant.

## Deliverables

- `Docs/production/CITY_ENVIRONMENT_GRAMMAR.md`
- street/parcel grammar;
- initial building-family catalogue specification;
- reusable-composition ladder;
- machine-readable discovery requirements for later bridge/authoring work.

## Acceptance

- The grammar can produce varied streets/buildings without one-off design for every parcel.
- Old quarter, commercial, residential, port/work and rural transition can differ using shared rules rather than unrelated kits.
- Building families expose meaningful entrances/service relations needed by CITY-02/03.
- A weaker model could choose among constrained reviewed compositions rather than “make a nice building”.
- No asset import, prefab implementation, Unity scene, catalogue authority or canonical contract is created.

## Negative gates

FAIL if the plan depends on unique bespoke buildings everywhere, treats a marketplace asset pack as semantic authority, or promises procedural generation without reviewed constraints.
