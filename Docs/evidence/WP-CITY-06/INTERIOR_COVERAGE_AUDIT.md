# WP-CITY-06 — Interior coverage audit

Candidate semantic owner: `Docs/production/CITY_INTERIORS_DISCOVERY.md`  
Baseline: `main@632a63c089f844313e567046b3df541e435d75d4`  
Status: Worker evidence; not an independent review.

## 1. Audit question

Does the CITY-06 candidate allocate every inherited CITY-02 `I1..I3` commitment to a bounded reusable interior topology while preserving accepted CITY-05 access roles and avoiding silent promotion of `I0`, `I1` or hero depth?

Accepted backlog cardinality:

- I3: **1**;
- I2: **6**;
- I1: **7**;
- total I1–I3: **14**.

The remaining CITY-02 rows stay I0 unless a later explicit owner reopens/promotes them.

## 2. Exact backlog conformance

| Place | Inherited I | Inherited required roles | CITY-06 family / overlay | Depth check | Role check | Result |
|---|---:|---|---|---|---|---|
| `loc.casco.bar` | I3 | public, service, semi-private | `if.social_house` + `hero.bar_layered` | only I3 hero; bounded 4–6 zones/layers | all 3 preserved | PASS |
| `loc.plaza.ayuntamiento` | I2 | public, service, private | `if.civic_office` | 2–4 meaningful zones; no hero requirement | all 3 preserved | PASS |
| `loc.calle.bakery` | I2 | public, service, private | `if.shop_work_deep` | 2–4 zones; work/service depth | all 3 preserved | PASS |
| `loc.barrio.residence_cluster` | I2 | public, private, semi-private | `if.residential_cluster` | 2–4 zones; shared/private layer | all 3 preserved | PASS |
| `loc.ribera.workshop` | I2 | public, service, private | `if.workshop_deep` | 2–4 zones; no hero promotion | all 3 preserved | PASS |
| `loc.puerto.worker_social` | I2 | public, service, semi-private | `if.social_house` | reusable I2 base; no hero overlay | all 3 preserved | PASS |
| `loc.entrada.fonda` | I2 | public, private, service | `if.lodging_common` | 2–4 zones; common/private/service | all 3 preserved | PASS |
| `loc.calle.pharmacy` | I1 | public, private | `if.retail_shallow` | 1 primary zone + private pocket | both preserved | PASS |
| `loc.calle.everyday_shop` | I1 | public, service | `if.retail_shallow` | 1 primary zone; service threshold/pocket | both preserved | PASS |
| `loc.ensanche.neighbourhood_anchor` | I1 | public, service | `if.retail_shallow` | 1 primary zone; service threshold/pocket | both preserved | PASS |
| `loc.ribera.service_yard` | I1 | service | `if.work_support_shallow` | exterior-led; 1 room maximum | service preserved | PASS |
| `loc.puerto.work_hub` | I1 | public, service, private | `if.work_support_shallow` | one compact zone; roles via thresholds/screening | all 3 preserved | PASS |
| `loc.puerto.warehouse_yard` | I1 | public, service | `if.work_support_shallow` | one shallow room/support office | both preserved | PASS |
| `loc.vega.supply_node` | I1 | public, private, service | `if.work_support_shallow` | one compact work/supply room | all 3 preserved | PASS |

Coverage: **14 / 14** inherited I1–I3 rows allocated.

## 3. Reuse audit

The candidate defines eight reusable interior families plus one hero overlay:

- `if.retail_shallow` — 3 direct programme uses;
- `if.work_support_shallow` — 4 direct programme uses;
- `if.shop_work_deep` — 1 current use, promotable only by reviewed topology need;
- `if.civic_office` — 1 current use;
- `if.residential_cluster` — 1 current use;
- `if.workshop_deep` — 1 current use;
- `if.social_house` — 2 direct uses, including the bar's reusable base;
- `if.lodging_common` — 1 current use;
- `hero.bar_layered` — 1 one-off overlay, explicitly non-family/universal.

The catalogue therefore does not achieve reuse by collapsing semantically different access patterns into one generic room, and it does not require a bespoke family for every place.

## 4. `I0` protection audit

The semantic owner explicitly preserves I0 as no enclosed-interior promise and lists exterior-led second layers instead of promoting rooms for:

- Casco shared court;
- Plaza market;
- Barrio lavadero;
- Ensanche shared garden;
- Ribera paseo edge;
- Puerto landing;
- Entrada arrival/depot edge;
- Vega quiet paseo/huerta edge.

No I0 programme row appears in the I1–I3 allocation table.

## 5. Orthogonality audit

The candidate preserves direct counterexamples to any collapsed depth rule:

- `loc.casco.shared_court`: S3 / I0 — spatially deep exterior, no enclosed interior;
- `loc.calle.pharmacy`: S2 / I1 — shallow interior without deep spatial classification;
- `loc.ribera.workshop`: S3 / I2 — deep reusable workplace, not hero;
- `loc.casco.bar`: S4 / I3 — sole hero commitment;
- `loc.entrada.arrival`: A / S1 / I0 — high systemic importance with no interior.

No rule derives I-depth from A–D or S-depth.

## 6. Required access-role negative controls

### NC-I-01 — bar qualifier substitution

Mutant: replace the bar's required `semi-private` role with a `vertical` qualifier.

Expected: **FAIL**. `vertical` describes form and cannot satisfy `semi-private`.

Protected by: semantic owner §§1, 5, 15.

### NC-I-02 — everyday-shop optional service

Mutant: make the everyday shop service threshold optional because `if.retail_shallow` can exist without it generically.

Expected: **FAIL**. Functional-POI binding requires `{public, service}`.

Protected by: semantic owner §§1, 5.

### NC-I-03 — service-yard publicisation

Mutant: turn `loc.ribera.service_yard` into an ordinary public through-room so it is easier to traverse.

Expected: **FAIL**. Its inherited required role is service; public edge remains conditional and internal circulation cannot publicise restricted/service topology.

Protected by: semantic owner §§5.2, 15.

## 7. Depth-inflation negative controls

### NC-D-01 — asset-room promotion

Mutant: an I0 mixed frontage uses a prefab with a furnished interior, so the interior becomes playable by default.

Expected: **FAIL**. Asset geometry is not authority for CITY-02 interior promise.

### NC-D-02 — I1 becomes mini-I2

Mutant: `loc.puerto.work_hub` receives separate public office, staff room and private records room because it has three access roles.

Expected: **FAIL** unless CITY-02 is explicitly reopened. Role multiplicity is not room multiplicity; I1 owns one meaningful enclosed zone.

### NC-D-03 — I2 hero promotion

Mutant: `loc.puerto.worker_social` gets the `hero.bar_layered` overlay because it shares `if.social_house`.

Expected: **FAIL**. Only `loc.casco.bar` is I3.

### NC-D-04 — secret-room quality metric

Mutant: every I2 must add one hidden room to count as “deep”.

Expected: **FAIL**. I2 depth is meaningful zone/threshold topology, not a hidden-room quota.

## 8. District second-layer coverage

The candidate's district matrix gives all nine major district families at least one grounded second-layer mechanism:

1. Casco — bar layering + shared court/stair;
2. Plaza/Ayuntamiento — civic staff/private + records/notice surface;
3. Calle Mayor — bakery work/service + shallow service shops;
4. Barrio Alto — residence shared/private + lavadero;
5. Ensanche — service shop + shared garden;
6. Ribera/Talleres — workshop/service/material + paseo observation;
7. Puerto — social/service + work/material + landing state;
8. Entrada/Carretera — fonda private/service + arrival/logistics;
9. La Vega — supply/material + quiet/huerta context.

No district is required to have a basement, roof route or extraordinary secret.

## 9. Capacity/reopen posture

This audit does not re-prove CITY-02 Q6 or CITY-05 host-fit evidence. It checks that CITY-06 does not intentionally expand those claims.

Reopen signal remains mandatory if realized/intermediate topology shows that:

- a required interior cannot fit the accepted shell/support posture;
- preserving required access roles needs an exterior-family contract change;
- the accepted programme-envelope ceiling would be exceeded;
- hard ordinary/quiet reserve would be consumed.

No such contradiction is identified by the planning topology in this candidate.

## 10. Worker conclusion

`INTERIOR_COVERAGE_AUDIT: PASS_FOR_WORKER_PRE_REVIEW`

This is Worker evidence only. A fresh independent Reviewer must independently reconstruct the backlog, access-role inheritance and depth boundaries.