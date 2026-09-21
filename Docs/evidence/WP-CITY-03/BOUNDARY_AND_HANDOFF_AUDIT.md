# WP-CITY-03 — Boundary + handoff audit

Baseline: `main@1b503cfabeca340c42055aaf31df90d72ca28e68`  
Audited semantic owner: `Docs/production/CITY_PRODUCT_SEED.md` v0.3  
Repair cycle: **fail_cycle 1** after independent review #5268624526  
Purpose: recompute the selected seed's mechanical handoff properties without redefining product semantics.

## 1. Reviewer blocker addressed

The failed candidate `68dafdc555be0732f31200aa95d8533c4fad2142` froze an exact outer polygon but left `mask.rio` / `mask.arroyo` as semantic exclusions and deferred bank geometry to CITY-04. That made the actual land/water boundary non-deterministic.

The repaired semantic owner now freezes:

- exact Río polygon;
- exact Arroyo polygon;
- 3 m Río and 2 m Arroyo derived bank/no-build shoulders;
- exact X1/X5 crossing polygons;
- exact Wedge/Orilla/Ensanche receiving/approach stubs;
- explicit set algebra for permanent versus low-water playable space.

CITY-04 retains empirical falsification authority over width/grade/bend/bank treatment, but no longer selects the first bank limits.

## 2. Hard-envelope arithmetic

Outer polygon, in order:

```text
B01 (-20,-60)
B02 (70,-80)
B03 (205,-48)
B04 (245,22)
B05 (220,95)
B06 (150,145)
B07 (70,132)
B08 (-20,55)
```

Shoelace recomputation:

```text
area = 44,817.5 m²
     = 0.0448175 km²
```

Result: **PASS** against inherited CITY-00 retained-seed band `0.03–0.06 km²`.

The band remains defined on the hard planning envelope. Exact water subtraction is a construction-boundary diagnostic, not a new upstream acceptance metric.

## 3. Exact water-mask arithmetic

### Río

Authoritative polygon:

```text
(-20,-8),(0,-10),(20,-24),(40,-42),(60,-46),(95,-45),(140,-42),(185,-40),(205,-38),
(205,-48),(185,-52),(140,-57),(95,-61),(60,-62),(40,-58),(20,-43),(0,-30),(-20,-24)
```

Recomputed area: **3,505.0 m²**.

### Arroyo

Authoritative polygon:

```text
(-20,-8),(0,10),(20,28),(40,48),(60,68),(78,90),(88,111),(108,132),(120,140.125),
(100,136.875),(99,130),(90,118),(82,106),(70,86),(50,60),(30,40),(8,20),(-20,4)
```

Recomputed area: **713.6875 m²**.

`(120,140.125)` and `(100,136.875)` lie on outer edge `B07→B06`; the mask cannot be skirted through a dry upstream sliver. Both water polygons meet at `(-20,-8)` on the outer boundary, so the Wedge terminates at the confluence rather than continuing dry toward Puerto.

Combined water area: **4,218.6875 m²**.

Dry-land diagnostic area:

```text
44,817.5 - 4,218.6875 = 40,598.8125 m²
```

Again, this number is diagnostic only.

## 4. Landmass-component check

Subtracting the exact water polygons from the outer envelope yields exactly **three** connected dry components:

| Component | Area | Required selected anchors |
|---|---:|---|
| Wedge | 31,704.125 m² | W.LANDING, W.X1, W.CASCO, W.X5, W.PLAZA, W.SHOP |
| Orilla-sur | 3,200.0 m² | O.X1 |
| Ensanche receiving component | 5,694.6875 m² | E.X5 |

Result: **PASS** against CITY-00 landmass semantics.

Negative controls:

- if `mask.rio` is removed, Wedge and Orilla-sur become dry-connected → FAIL;
- if `mask.arroyo` is removed, Wedge and Ensanche become dry-connected → FAIL;
- if the two masks do not meet at the confluence cut, the Wedge may leak dry past the tip → FAIL.

## 5. Crossing-overlay check

### X1

```text
x1.crossing = (53,-33),(45,-65),(39,-63),(47,-31)
stub.x1.wedge  = (42,-40),(58,-40),(58,-22),(42,-22)
stub.x1.orilla = (34,-69),(50,-72),(52,-62),(37,-60)
```

- W.X1 `(50,-32)` lies in the Wedge-side approach.
- O.X1 `(42,-64)` lies in the Orilla-side receiving stub.
- X1 crosses `mask.rio` and reconnects exactly Wedge↔Orilla-sur.
- With X1 applied but X5 unavailable, there are **two** playable connected components: `{Wedge+Orilla-sur}` and `{Ensanche}`.

Result: **PASS**.

### X5

```text
x5.crossing = (90.5,107.8),(88.5,121.8),(91.5,122.2),(93.5,108.2)
stub.x5.wedge    = (84,99),(99,99),(96,108),(87,108)
stub.x5.ensanche = (86,119),(93,124),(94,130),(84,128)
```

- W.X5 `(92,108)` lies in Wedge land/approach.
- E.X5 `(90,122)` lies in Ensanche land/receiving stub.
- X5 crosses `mask.arroyo`.
- When X5 is available, adding its crossing overlay connects the remaining Ensanche component to the Wedge.
- When X5 is unavailable, its water intersection is non-traversable and no alternate crossing appears.

Result: **PASS** with inherited low-water-only availability preserved.

## 6. Bank/no-build derivation

The semantic owner defines:

```text
nb.rio_bank = ((buffer(mask.rio, 3.0 m) ∩ hard_outer) - mask.rio) - exempt.rio_crossing
nb.arroyo_bank = ((buffer(mask.arroyo, 2.0 m) ∩ hard_outer) - mask.arroyo) - exempt.arroyo_crossing
```

Named exemptions contain only their crossing corridor + two associated approach/receiving stubs.

Audit results:

- no frontage/open-site region is allowed to use an unnamed bank-clearance exemption;
- bank shoulders create no graph edge;
- only X1 pierces Río water/clearance permanently;
- only X5 pierces Arroyo water/clearance conditionally;
- W.LANDING remains dry Wedge land adjacent to the confluence and does not become X6/X7 geometry.

Result: **PASS**.

## 7. Anchor containment

Point-in-polygon / water-exclusion recomputation:

| Anchor | Target | Outer | Dry component / role |
|---|---:|---|---|
| `W.LANDING` | `(0,0)` | inside | Wedge |
| `W.X1` | `(50,-32)` | inside | Wedge/X1 approach |
| `O.X1` | `(42,-64)` | inside | Orilla-sur/X1 stub |
| `W.CASCO` | `(80,35)` | inside | Wedge |
| `W.X5` | `(92,108)` | inside | Wedge/X5 approach |
| `E.X5` | `(90,122)` | inside | Ensanche/X5 stub |
| `W.PLAZA` | `(150,58)` | inside | Wedge |
| `W.SHOP` | `(195,70)` | inside | Wedge |

Result: **8/8 selected anchors inside the correct land component**.

## 8. Site-region containment + bank-clearance check

All vertices of F01–F08 and S01–S03 were checked against:

1. hard outer polygon;
2. `mask.rio`;
3. `mask.arroyo`;
4. effective `nb.rio_bank`;
5. effective `nb.arroyo_bank`.

Result: **11/11 fully inside hard outer; 11/11 outside water; 11/11 outside effective bank no-build.**

S02 is the exact `stub.x1.wedge`, so its proximity to Río is covered solely by the named X1 exemption; it is not a generic bank-clearance exception.

F01–F08 family-band check remains:

| Slot | Approx. bounding size | Inherited family band | Result |
|---|---:|---|---|
| F01 bar | 8×15 m | `pc.historic_row` 5–9 × 9–18 | PASS |
| F02 ayuntamiento | 18×22 m | `pc.civic_frontage` 12–24 × 15–30 | PASS |
| F03 everyday shop | 10×18 m | `pc.commercial_row` 6–12 × 12–24 | PASS |
| F04 old-row home | 7×14 m | `pc.historic_row` | PASS |
| F05 old-row home | 7×12 m | `pc.historic_row` | PASS |
| F06 plaza-edge frontage | 12×16 m | `pc.commercial_row` | PASS |
| F07 mixed frontage | 8×16 m | `pc.commercial_row` | PASS |
| F08 old-row house | 7×13 m | `pc.historic_row` | PASS |

## 9. Crossing / graph conformance

Playable inter-landmass crossings represented by CITY-03:

- Río: **X1 only**.
- Arroyo: **X5 only**, preserving low-water-only availability.

Explicit non-playable future crossing socket:

- `W.LANDING` may expose future X6/X7 relation but neither X6 nor X7 traversal geometry exists.

Negative graph checks:

- no dry Wedge→Puerto edge: PASS;
- no Ensanche→Orilla-sur edge: PASS;
- no eighth crossing: PASS;
- X5 not counted as permanent redundancy: PASS;
- shared court not counted as public graph edge: PASS;
- Casco micro.A/B remain within one W.CASCO node and are not used as CITY-01 proof: PASS.

## 10. Selected place/depth/interior and access coverage

Selected hard seed still contains:

- I3: `loc.casco.bar` only;
- I2: `loc.plaza.ayuntamiento`;
- I1: `loc.calle.everyday_shop`;
- I0 second layers: market, bridgehead, shared court;
- ordinary C/S1 closed frontages.

Required CITY-05 roles remain:

- bar `{public, service, semi-private}`;
- ayuntamiento `{public, service, private}`;
- everyday shop `{public, service}`;
- market/bridgehead public;
- shared court public passage + semi-private court.

No geometry repair changes those bindings.

## 11. Route/time arithmetic

Only fully represented accepted CITY-01 edges are listed for CITY-04 measurement.

```text
O.X1 → X1 → W.X1 → W.CASCO → W.PLAZA
= X1 1.0 + W12 1.0 + W05 1.75
= 3.75 min

W.SHOP → W.PLAZA → W.CASCO → W.LANDING
= W04 0.5 + W05 1.75 + W06 2.25
= 4.50 min

W.CASCO → W.X5 → E.X5
= W13 1.0 + X5 0.4
= 1.40 min when X5 is available
```

Arithmetic: **PASS**.

## 12. Scenario coverage audit

WP-CITY-03 minimum future scenario coverage remains mapped as before:

| Contract coverage | Scenario witness |
|---|---|
| quiet ordinary morning | SCN-01 |
| market/commercial flow | SCN-02 |
| NPC home/work/social trip | SCN-03 spatial proxy |
| second materially different ordinary trip | SCN-04 |
| follow/search route choice | SCN-05 |
| bar/social activity spatial support | SCN-06 |
| river/bridge traversal | SCN-07, now explicitly uses `x1.crossing` |
| material delivery/work consequence | SCN-08 proxy |
| municipal access/routing/service change | SCN-09 proxy |
| low-stakes player perturbation | SCN-10 |
| leave/return to changed local state | SCN-11 proxy |
| discovery with >1 truthful route when later systems exist | SCN-12 |
| changing/blocked route + alternate | SCN-13, including X5 unavailable water cut |

Every runtime-dependent scenario remains explicitly proxy/readiness-only.

## 13. Expansion-seam audit

Named seams remain:

1. `seam.commercial_ne` — Calle Mayor / X2 / Vega direction;
2. `seam.ensanche_x5` — from exact Ensanche X5 receiving stub;
3. `seam.port_landing` — future direct X6/X7 Puerto relation from W.LANDING;
4. `seam.orilla_x1` — from exact Orilla X1 receiving stub toward Puerto/Entrada;
5. `seam.upper_future` — later upper/Barrio continuity through accepted graph.

The exact water masks meet/cut the hard boundary without opening any extra dry seam. None requires moving retained W.PLAZA, W.CASCO, X1, W04/W05/W06/W12/W13 or F01–F03.

Result: **PASS**.

## 14. CITY-04 execution sufficiency

A LOCAL Worker now receives, before greybox judgment:

- exact hard outer polygon;
- exact Río/Arroyo polygons;
- exact derived bank/no-build shoulders;
- exact X1/X5 crossing overlays and receiving/approach stubs;
- selected anchor targets in their correct landmasses;
- exact accepted edges/crossings represented;
- bounded F01–F08/S01–S03 site regions;
- selected A/B/C places and I0–I3 obligations;
- preserved access-role sets;
- retained-vs-temporary declaration;
- five expansion seams;
- thirteen spatial scenarios;
- segment/time hypotheses and physical/access/sightline measurements;
- causal owner routing if physical evidence falsifies the plan.

CITY-04 may test and report that a bank width/profile/grade/bend is physically poor, but it does not have to decide where land ends and water begins in order to start.

## 15. Negative controls

1. Remove X1 → inherited seed/bridge scenario fails.
2. Make X5 permanent → inherited crossing semantics fail.
3. Remove/shift Río so Wedge touches Orilla → CITY-00 landmass truth fails.
4. Remove/shift Arroyo so Wedge touches Ensanche around the seed cut → CITY-00 landmass truth fails.
5. Separate the two masks at confluence to leave a dry tip continuation → CITY-00 confluence truth fails.
6. Connect W.LANDING dry to Puerto → CITY-00 geography fails.
7. Add an unnamed bank-clearance exemption → CITY-05/site-data boundary fails.
8. Route through S03 semi-private court → access laundering / false graph edge.
9. Remove F03 service role → CITY-05 role binding fails.
10. Open F04/F05/F06/F07/F08 interiors → I0/open-door inflation.
11. Count PA route as implemented → CITY-06 causal boundary fails.
12. Move F01/F02/F03 to another street during greybox → CITY-04 becomes city redesign.
13. Promote another hero interior → CITY-02/06 fails.
14. Use a scenic/soft-envelope path as playable seam → hard/soft boundary fails.

## 16. Audit verdict

**BOUNDARY_AND_HANDOFF_AUDIT: CLEAN**

The independent review blocker is repaired causally: the hard playable boundary is now reproducible from exact local geometry before CITY-04 begins. No predecessor reopen signal is present. Metric plausibility remains deliberately falsifiable by CITY-04.