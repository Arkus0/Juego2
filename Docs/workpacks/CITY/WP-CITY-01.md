# WP-CITY-01 — Mobility, district graph + walk-time topology

Status: **FROZEN PLAN / NOT_STARTED**  
Class: PRODUCT / SPATIAL PREPRODUCTION (NON-FOUNDATIONAL)  
Mode: **REMOTE**  
Depends on: `WP-CITY-00` PASS + DocSync  
Blocks: `WP-CITY-02` only

## Contract continuity

Accepted CITY-00 assigns exact route costs/walk-time work to `CITY-01`. This WP preserves that delegation. It may sharpen mobility evidence and access vocabulary, but it MUST NOT become a different owner.

## Objective

Turn the selected city constitution into a movement topology that supports daily life, following/searching, schedule pressure, route choice, service access, municipal disruption and believable travel without turning traversal into empty commute.

CITY-01 owns movement/access topology and measurable travel hypotheses. It does not choose which locations are systemic and does not design interiors.

## Work

1. Derive the district-to-district graph from accepted CITY-00 landmasses, crossings and the connectivity matrix.
2. Define primary, secondary, quiet and service-route families.
3. Define elevation classes, stairs/ramps/roads and relevant travel restrictions.
4. Produce target walk-time bands between representative anchors, including the CITY-00 routes whose exact costs were explicitly deferred here.
5. Model pedestrian, slower pedestrian, bicycle, service/delivery, arrival/bus, following and time-sensitive travel profiles.
6. Test port ↔ commercial/workshop logistics and rural edge ↔ market/civic travel.
7. Identify chokepoints governance/events may alter without deadlocking the city.
8. Identify service/back-route opportunities without laying out building interiors.

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
- one service/back-route opportunity differs from the obvious public route.

## Deliverables

- `Docs/production/CITY_MOBILITY_TOPOLOGY.md`
- one authoritative semantic route/access graph;
- representative walk-time matrix;
- mobility-profile assumptions;
- chokepoint/alternate-route ledger;
- explicit measurement questions later consumed by `CITY-04`.

## Acceptance

- Every edge is compatible with accepted CITY-00 connectivity truth.
- Core movement has meaningful loops and ≥2 nontrivial alternate-route cases.
- No plaza or bridge becomes a universal connector contrary to the accepted constitution.
- Follow/search traversal is not just a corridor.
- Port and rural edge participate in ordinary movement, not only story missions.
- A municipal closure can matter without making the city unusable.
- Walk-time targets are documented as hypotheses for `CITY-04`, not treated as measured fact.

## Definition of Done

`CITY-02` receives one reviewable movement/access model with explicit target costs and residual measurement questions; it does not need to reinterpret CITY-00 connectivity or invent routes ad hoc.

## Forbidden

Redrawing CITY-00 crossings/landmasses; Unity navmesh work; vehicle simulation; final road engineering; live schedules; interior layouts; fast-travel design; reopening H0.
