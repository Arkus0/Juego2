# WP-CITY-02 — Mobility + access topology

Status: **FROZEN PLAN / NOT_STARTED**  
Class: PRODUCT / SPATIAL PREPRODUCTION (NON-FOUNDATIONAL)  
Mode: **REMOTE**  
Depends on: `WP-CITY-01` PASS  
Blocks: `WP-CITY-03` only

## Objective

Turn the accepted CITY-00 constitution into a movement topology that supports daily life, following/searching, schedule pressure, route choice, service access and municipal disruption without turning traversal into empty commute.

CITY-02 owns movement/access logic, not building or interior design.

## Work

1. Derive the district-to-district route graph from accepted CITY-00 landmasses/crossings.
2. Define primary, secondary, quiet and service-route families.
3. Define elevation classes, stairs/ramps/roads and relevant travel restrictions.
4. Reserve plausible rear/service/vertical access opportunities using CITY-01 vocabulary without laying out interiors.
5. Produce target walk-time bands between representative anchors.
6. Test pedestrian, slower pedestrian, bicycle, service/delivery, arrival/bus, following and time-sensitive travel profiles.
7. Test port ↔ commercial/workshop logistics and rural edge ↔ market/civic travel.
8. Identify chokepoints governance/events may alter without deadlocking the city.

## Required scenario matrix

At minimum:

- home in upper/residential district → workplace;
- old quarter → port/work edge without mandatory plaza traversal;
- port delivery → commercial destination;
- rural/valley arrival → market/civic core;
- player follows an NPC across ≥2 district boundaries;
- closure forces a plausible alternate route;
- late actor chooses a faster but contextually different route;
- quiet evening route differs meaningfully from market-day flow;
- at least one service/back-route opportunity differs from the obvious public route.

## Deliverables

- `Docs/production/CITY_MOBILITY_TOPOLOGY.md`
- semantic route/access graph;
- representative walk-time matrix;
- mobility-profile assumptions;
- chokepoint/alternate-route ledger;
- later local measurements required from CITY-07.

## Acceptance

- Every edge is compatible with CITY-00 connectivity truth.
- Core movement has meaningful loops and ≥2 nontrivial alternate-route cases.
- Follow/search gameplay is not just corridor traversal.
- Service/private access opportunities exist without becoming universal shortcuts.
- A municipal closure can matter without making the city unusable.
- Walk-time targets remain hypotheses until local validation.

## Forbidden

Redrawing CITY-00 crossings/landmasses; Unity navmesh; final road engineering; live schedules; interior layouts; fast-travel design.
