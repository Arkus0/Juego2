# WP-CITY-05 — Streets, parcels + reusable building families

Status: **COMPLETE**  
Class: PRODUCT / ENVIRONMENT PREPRODUCTION (NON-FOUNDATIONAL)  
Mode: **REMOTE**  
Depends on: `WP-CITY-02` PASS  
Blocks: `WP-CITY-06` only

Accepted candidate: `10d1528b0354b16a614fb10a3933a25b32f15f28`  
Independent review: **PASS**, review `#5267776704`  
Merged: PR `#83`, merge commit `47909a72eb6d38332f62e9c01426c8cd40e1863b` on 2026-09-21

## Objective

Define the reusable exterior production grammar that turns accepted geography, mobility and place requirements into streets and buildings an agent can later compose instead of inventing each façade from a blank plane.

This is a new post-CITY-00 responsibility and therefore uses an ID not bound by the accepted constitution.

## Work

1. Define street-segment families: ordinary street, narrow historic lane, steps, riverside/promenade, service edge, plaza/market edge, port/work edge, rural transition and relevant junction types.
2. Define parcel constraints: frontage, depth/bounds, access edges, party-wall/detached relations, slope/retaining conditions, no-build/view corridors and service/rear relations.
3. Define a compact set of reusable building families: ordinary house, mixed-use house/shop, bar/social venue, shop/service, workshop, warehouse/port building, civic/municipal, apartment/residential and selected rural/peripheral types.
4. Define composition ladder: `module -> assembly -> shell -> reusable building -> functional POI -> street segment`.
5. Map CITY-02 A–D/S-depth requirements to shell/access obligations without choosing runtime behaviour.
6. Define what later Arkus discovery must be able to expose for a reviewed composition: stable ID, dimensions/bounds, sockets, access anchors, tags/archetype, dependencies, variants, style/material constraints and compatibility/version metadata.
7. Define promotion rule for a reviewed one-off composition to become a reusable family/variant.

## Deliverables

- `Docs/production/CITY_ENVIRONMENT_GRAMMAR.md`
- street/parcel grammar;
- initial building-family catalogue **specification**;
- reusable-composition ladder;
- machine-readable discovery requirements for later bridge/authoring work.

## Acceptance

- The grammar can produce varied streets/buildings without one-off design for every parcel.
- Old quarter, commercial, residential, port/work and rural transition differ through constrained shared rules rather than unrelated kits.
- Building families expose meaningful entrance/service relations needed by CITY-02 and later CITY-06.
- A weaker model could choose among constrained reviewed compositions rather than “make a nice building”.
- No asset import, prefab implementation, Unity scene, catalogue authority or canonical contract is created.

## Definition of Done

`CITY-06` receives reviewed shell/parcel constraints and reusable families sufficient to design interiors/discovery without inventing every building from scratch.

## Negative gates

FAIL if the plan depends on unique bespoke buildings everywhere, treats a marketplace asset pack as semantic authority, or promises procedural generation without reviewed constraints.
