# WP-CITY-02 — Programme audit

Candidate semantic owner: `Docs/production/CITY_LOCATION_PROGRAMME.md`  
Audit role: bounded Worker evidence; **not** a second semantic owner.  
Status: DRAFT-ACTIVE audit before Worker pre-review.

## 1. Contract coverage

| WP-CITY-02 requirement | Evidence in semantic owner | Audit result |
|---|---|---|
| district × location programme | §4 master ledger | COVERED |
| provisional A–D + S0–S4 independently | §3 + §4 | COVERED |
| A/B use profiles | §5 profiles every A/B row | COVERED |
| interior priority | §2.1 + §6 | COVERED, subject to pre-review consistency check |
| reactive-density measurements | §8 RD-1..RD-5 | COVERED |
| quiet/ordinary fabric | §7 + C/D/S0/S1 rows | COVERED |
| Living World spatial handoff only | §5.1 + §13 | COVERED |
| explicit C/D + S0/S1 scope | §12 | COVERED |

## 2. Required domain coverage

The master ledger provides physical homes for every required domain without inventing a new CITY-01 route:

- civic/governance/authority — `loc.plaza.ayuntamiento`, `loc.puerto.work_hub`;
- food/drink/social — `loc.casco.bar`, `loc.puerto.worker_social`, `loc.entrada.fonda`;
- retail/service — bakery, pharmacy, everyday shop, Ensanche neighbourhood anchor;
- work/workshop/logistics — Ribera workshop/service yard, Puerto work/warehouse, Entrada depot, Vega supply;
- port/waterfront — Puerto landing/work hub/warehouse plus Ribera paseo relation;
- residential — Barrio Alto, Ensanche and Casco fabric;
- leisure / physical activity homes — bar table/counter, market terrace/stall pitch, Ensanche square, workshop/yard and Puerto loading yard;
- health/safety/authority where needed — pharmacy plus municipal authority surface in Ayuntamiento; no unrequired hospital/police-complex scope is invented;
- rural/river — Vega huerta/supply, quiet paseo, Ribera and Puerto work edge;
- quiet/private/semi-private — shared court/garden, lavadero, upper/quiet places and residence thresholds;
- arrival/visitor — bus arrival, fonda and depot forecourt.

Result: **COVERED**.

## 3. Classification census

The semantic owner contains **37 programme rows**. Counts are intentionally place/family counts, not future parcel/building counts.

### A–D totals

| Importance | Rows |
|---|---:|
| A | 8 |
| B | 15 |
| C | 11 |
| D | 3 |

### S-depth totals

| Depth | Rows |
|---|---:|
| S0 | 2 |
| S1 | 17 |
| S2 | 10 |
| S3 | 7 |
| S4 | 1 |

### A–D × S0–S4 cross-tab

| | S0 | S1 | S2 | S3 | S4 | Total |
|---|---:|---:|---:|---:|---:|---:|
| A | 0 | 1 | 3 | 3 | 1 | 8 |
| B | 0 | 5 | 7 | 3 | 0 | 15 |
| C | 0 | 10 | 0 | 1 | 0 | 11 |
| D | 2 | 1 | 0 | 0 | 0 | 3 |

Direct independence witnesses:

- `A/S1` — `loc.entrada.arrival`;
- `A/S2` — `loc.ensanche.neighbourhood_anchor`, `loc.puerto.work_hub`, `loc.vega.supply_node`;
- `A/S3` — Ayuntamiento, Barrio residence cluster, Ribera workshop;
- `A/S4` — casco bar;
- `B/S1`, `B/S2`, `B/S3` all exist;
- `C/S3` — ordinary `loc.casco.shared_court`;
- `D/S1` — landmark shell; `D/S0` — scenic envelope.

Result: **A–D is not a synonym or deterministic lookup for S-depth.**

## 4. Interior-priority census

The intended frozen interior backlog is:

| Priority | Rows |
|---|---:|
| I0 NONE | 23 |
| I1 SHALLOW | 7 |
| I2 DEEP | 6 |
| I3 HERO | 1 |

Only `loc.casco.bar` is I3/HERO. Deep interiors are selective, and most programmed rows promise no enclosed interior.

Pre-review consistency condition: every master-ledger and profile reference must resolve to one unambiguous `I0..I3` value. If any row retains a slash/range instead of one priority, the candidate is NOT_READY until repaired.

## 5. A/B profile completeness

The master ledger contains 23 A/B rows (8 A + 15 B). §5 contains one profile for each:

1. `loc.casco.bar`
2. `loc.casco.bridgehead`
3. `loc.plaza.ayuntamiento`
4. `loc.plaza.market`
5. `loc.calle.bakery`
6. `loc.calle.pharmacy`
7. `loc.calle.everyday_shop`
8. `loc.barrio.residence_cluster`
9. `loc.barrio.lavadero`
10. `loc.ensanche.neighbourhood_anchor`
11. `loc.ensanche.shared_garden`
12. `loc.ribera.workshop`
13. `loc.ribera.service_yard`
14. `loc.ribera.paseo_edge`
15. `loc.puerto.work_hub`
16. `loc.puerto.landing`
17. `loc.puerto.worker_social`
18. `loc.puerto.warehouse_yard`
19. `loc.entrada.arrival`
20. `loc.entrada.fonda`
21. `loc.entrada.depot_forecourt`
22. `loc.vega.supply_node`
23. `loc.vega.quiet_paseo`

Each profile carries plausible role/time demand, activity/material role, witness potential, governance/access hook, interior need, change potential and an absent-player spatial rationale. These are programme demands, not authored runtime schedules or simulation guarantees.

Result: **23/23 profile coverage**.

## 6. District revisitation / A-anchor distribution

Substantial CITY-00 district families all have non-visual revisit drivers:

- Casco — bar + bridgehead + spatial shared court;
- Plaza/Ayuntamiento — civic office + market;
- Calle Mayor — bakery + pharmacy + everyday shop;
- Barrio Alto — residence cluster + lavadero;
- Ensanche — neighbourhood anchor + shared garden;
- Ribera/Talleres — workshop + yard + paseo;
- Puerto — ordinary work hub + landing + warehouse + worker social place;
- Entrada — arrival + fonda + depot;
- Vega — supply node + quiet paseo.

A anchors by district: Casco, Plaza, Barrio Alto, Ensanche, Ribera, Puerto, Entrada and Vega each own one. Casco + Plaza account for **2/8**, so the old/civic core does not monopolise A importance.

Result: **COVERED**.

## 7. CITY-00 deferred questions closed by CITY-02

### Q6 — A/B programme within accepted scale without crowding quiet fabric

Closed at programme level: 23 A/B place/family commitments are distributed across nine district families while the ledger and §12 explicitly retain substantial C/D and S0/S1 fabric. CITY-02 does not claim exact parcel count or density measurement; CITY-05 must test fit during parcel/family grammar without silently promoting ordinary fabric.

### Q7 — two everyday non-port services at Entrada/Puerto

Closed by:

- `loc.entrada.fonda` — lodging/food/common-room service for visitors/carters;
- `loc.entrada.depot_forecourt` — road/depot service and waiting/transfer function independent of river work.

These sit beside, but are not dependent on, Puerto's river operations.

### Q8 — physical homes for scarce ordinary activities

Closed as place commitments, without inventing gameplay mechanics:

- bar table/counter zone — social/table activity home;
- Plaza market pitches — stall/setup/exchange activity home;
- workshop/yard — repair/craft/work activity home;
- Puerto warehouse/loading yard — loading/weighing/material activity home;
- Ensanche small square/shared garden — neighbourhood leisure/social home;
- Vega supply node — huerta/mill/supply work home.

No activity is represented solely as a UI portal.

## 8. CITY-01 compatibility audit

No location requires a new crossing or route. Anchors resolve only to accepted CITY-01 nodes/route families:

- Casco/Plaza/Calle — `W.CASCO`, `W.X1`, `W.PLAZA`, `W.SHOP`, `W.X2` and accepted middle/lower relations;
- Barrio — `W.BARRIO`, `W.X3`, high/ravine route family;
- Ensanche — `E.HOME`, `E.X2/E.X3` and existing bank graph;
- Ribera — `W.RIBERA`, low/service relation including the restricted nature of `W17`;
- Puerto — `O.PUERTO_UP`, `O.QUAY`, `O02` work-front relation;
- Entrada — `O.ENTRADA`, with BUS terminating there per CITY-01;
- Vega — `W.VEGA`, quiet/rural approaches and `W.X4` direction where available.

Scenario checks:

- X2 closure does not strand all meaningful Ensanche content because an A anchor exists on the Ensanche bank and pedestrian alternates remain inherited X3/X4 cases;
- X6 suspension does not erase Puerto purpose; Orilla-sur work/social/arrival places still exist and pedestrian access falls back through X1 + camino sur;
- X7 closure does not gain a fake cart route; freight interruption remains meaningful;
- Plaza removal does not erase all other A/B destinations or require `W17` as an ordinary-public shortcut;
- quiet Q routes retain low-intensity destinations without continuous A/B/event frontage.

Result: **compatible; zero new semantic movement edges**.

## 9. Negative-gate audit

| Negative gate / failure class | Candidate status |
|---|---|
| every door open | rejected by I0 default + explicit C/S1 fabric |
| every prop interactive | RD-1 excludes props/C/D; no prop interaction programme exists |
| every pedestrian persistent | no population identity/simulation contract authored |
| every location incident-generating | quiet commitments explicitly require no incident obligation |
| A–D collapsed into S0–S4 | cross-tab contains direct counterexamples |
| old/civic core owns all A anchors | false; A is distributed across eight district families |
| port is mission-only scenery | false; work/logistics/social/arrival roles are explicit |
| minigames are UI-only portals | physical candidate homes are named; mechanics remain unowned |
| Living World semantics pre-accepted | no schedule/belief/dialogue/decision semantics are frozen |
| CITY-01 route/access reinterpreted for convenience | no new edge or access-class promotion is declared |

## 10. Reactive-density audit

The proposal defines five separate measurable families:

- `MDT` — A/B destinations reachable per traversal time;
- `RUR` — repeated meaningful use across distinct contexts;
- `RCD` — public route-choice decision points per traversal time;
- `QSB` — low-intensity/quiet share;
- `DD` — classification/depth distribution and collapse warnings.

Each has a denominator or explicit reporting basis. Scenic/ambient props do not count as meaningful destinations. Restricted service routes do not inflate ordinary-public route choice. Absent geometry is `NOT_IN_SEED`, preserving CITY-01's measurement boundary.

Result: **MEASURABLE LATER WITHOUT “EVERYTHING INTERACTIVE”.**

## 11. Pre-review targets

Before freeze the Worker must challenge at least:

1. all 23 A/B rows have exactly one profile and one interior priority;
2. all 37 programme rows have a single A–D and single S-depth assignment;
3. no mobility anchor implies a new inter-landmass relation or publicises `W17`;
4. Q6/Q7/Q8 above are actually closed by the semantic owner, not only by this audit;
5. the A/S and C/S counterexamples survive any edits;
6. quiet places retain value without incident-generation claims;
7. all runtime-social wording remains requirement/affordance language rather than implementation semantics;
8. the complete baseline→candidate diff remains limited to CITY-02 production/evidence scope.
