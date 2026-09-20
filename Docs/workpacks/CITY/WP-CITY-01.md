# WP-CITY-01 — Mobility, district graph + walk-time topology

Status: **FROZEN PLAN / NOT_STARTED**  
Class: PRODUCT / SPATIAL PREPRODUCTION (NON-FOUNDATIONAL)  
Mode: **REMOTE**  
Depends on: `WP-CITY-00` PASS  
Blocks: `WP-CITY-02` only

## Objective

Turn the selected city constitution into a movement topology that supports daily life, following/searching, schedule pressure, route choice, municipal disruption and believable travel without turning traversal into empty commute.

## Work

1. Define district-to-district graph with primary and secondary routes.
2. Define crossing strategy: bridges/river crossings and what each one connects.
3. Define elevation classes, stairs/ramps/roads and route restrictions where useful.
4. Produce target walk-time bands between representative anchors.
5. Model routes used by different travel profiles:
   - ordinary pedestrian;
   - older/slower pedestrian where relevant;
   - bicycle;
   - service/delivery route;
   - bus/arrival edge;
   - player following another actor;
   - player making a time-sensitive trip.
6. Test port ↔ commercial/workshop logistics and rural edge ↔ market/civic travel.
7. Identify chokepoints that governance/events may alter without making the whole city unusable.
8. Identify quiet/low-traffic alternatives and high-traffic convergence routes.

## Required scenario matrix

At minimum test:

- home in upper/residential district → workplace;
- old quarter → port/work edge without mandatory plaza traversal;
- port delivery → commercial destination;
- rural/valley arrival → market/civic core;
- player follows an NPC across ≥2 district boundaries;
- street/crossing closure forces a plausible alternate route;
- late actor chooses a faster but contextually different route;
- quiet evening route differs meaningfully from market-day flow.

## Deliverables

- `Docs/production/CITY_MOBILITY_TOPOLOGY.md`
- semantic route graph;
- representative walk-time matrix;
- mobility-profile assumptions;
- chokepoint/alternate-route ledger;
- CITY-04 measurements that must later be validated in actual Unity blockout.

## Acceptance

- Core district graph has meaningful loops and ≥2 nontrivial alternate-route cases.
- No single plaza/bridge is the universal mandatory connector unless explicitly justified as temporary early-seed state.
- At least one long traversal creates real “elsewhere in town” separation without becoming empty filler.
- Port and rural edge participate in ordinary movement, not only story missions.
- A municipal closure/access decision can change movement without deadlocking the city.
- Follow/search gameplay has enough branching to be gameplay, not a corridor.
- Walk-time targets are documented as hypotheses for local validation, not treated as measured fact.

## Forbidden

Unity navmesh work; vehicle simulation; final road engineering; fast-travel design; live schedule implementation; reopening H0.
