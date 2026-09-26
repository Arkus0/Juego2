# CITY P8 — interior lanes, paths and small squares for the retained seed (amendment proposal)

Status: **PROPOSED / NOT ACCEPTED**  
Class: DOCS_ONLY / PRODUCT-SCOPE AMENDMENT  
Causal owners amended: **CITY-01** (`CITY_MOBILITY_TOPOLOGY.md` edge ledger + nodes) and **CITY-03** (`CITY_PRODUCT_SEED.md` represented edges, geometry and open sites)  
Secondary owner checked: CITY-05 (existing families suffice)  
Origin: concept `Docs/production/concept/CITY_07_GAME_MAP_CONCEPT.md`; relates to CITY-09 PD-01/PD-03 but adopts **none** of P2/P3 as written  
Base: `main` at `2aefda4`

Acceptance requires independent review, merge and DocSync. Until then CITY-07 must not build or count any of these routes.

## 1. Problem

The accepted seed represents only W04, W05, W06, W12, W13, X1, X5 and the node-local Casco micro-loop. Measured on the exact CITY-03 geometry, only ~33 % of the Wedge's dry land lies within 15 m of a public route and ~49 % within 25 m. Two large blocks (Plaza ↔ Río and north of W13) can only be seen, never walked. Route choice outside the Casco micro-loop is out-and-back. The CITY-04 owner feedback (D02) flagged the seed as small and sparse.

## 2. New planning nodes

Planning anchors in the CITY-03 seed frame (U/V metres). They are junction nodes, not places.

| Node | U | V | Meaning |
|---|---:|---:|---|
| `W.J05A` | 104 | 41 | junction on former W05, just above the Casco |
| `W.J05B` | 121 | 48.5 | junction on former W05, below the Plaza |
| `W.J12` | 68 | 8 | junction on former W12 |
| `W.J13` | 88 | 79 | junction on former W13 |
| `W.HORNO` | 110 | 22 | small square / fork (Plazuela del Horno) |
| `W.J8A` | 155 | 8 | fork on the Tintes lane |
| `W.J8C` | 128 | 106 | fork on the Calleja Alta |
| `E.LOW` | 12 | 64 | Ensanche-component lane end, seam-facing |

`W.PLAZA` gains two exits at the plaza edge (east corner ≈ `(168,57)` and `(168,65)`), realized through `jn.plaza_gate`.

## 3. Edge subdivision (no semantic change)

Existing edges are split at the new junctions. Endpoints, character, elevation and access are unchanged, and the weights sum exactly to the accepted totals.

| Accepted edge | Becomes | Weights (min) |
|---|---|---|
| `W05` W.PLAZA ↔ W.CASCO · P/S · E1 · AP · 1.75 | `W05a` W.CASCO↔W.J05A · `W05b` W.J05A↔W.J05B · `W05c` W.J05B↔W.PLAZA | 0.25 + 0.55 + 0.95 = 1.75 |
| `W12` W.CASCO ↔ W.X1 · S · E0 · AP · 1.0 | `W12a` W.CASCO↔W.J12 · `W12b` W.J12↔W.X1 | 0.40 + 0.60 = 1.0 |
| `W13` W.CASCO ↔ W.X5 · Q · E1 · AF · 1.0 | `W13a` W.CASCO↔W.J13 · `W13b` W.J13↔W.X5 | 0.60 + 0.40 = 1.0 |

Split ratios follow drawn centreline length in the concept map. CITY-07 demo timing may recalibrate them.

## 4. New edges

Planning weights use ~75 m/min for AP lanes and ~65 m/min for AF dirt paths, rounded to 0.05. They are hypotheses for the CITY-07 demo campaign, not measurements.

| Edge | From ↔ To | Name | Char | Elev. | Access | Width band / family | Centreline (U,V) | Length | Weight |
|---|---|---|---|---|---|---|---|---:|---:|
| `W18` | W.J05A ↔ W.HORNO | Calleja de los Tintes (1) | S | E1 | AP | 2.5–4.0 `st.historic_lane` | (104,41),(108,38),(110,22) | 21 m | 0.30 |
| `W19` | W.HORNO ↔ W.J8A | Calleja de los Tintes (2) | S | E0 | AP | 2.5–4.0 `st.historic_lane` | (110,22),(128,6),(155,8) | 51 m | 0.70 |
| `W20` | W.J8A ↔ W.PLAZA | Calleja de los Tintes (3) | S | E1 | AP | 2.5–4.0 `st.historic_lane` + `jn.plaza_gate` | (155,8),(172,28),(172,45),(168,57) | 56 m | 0.75 |
| `W21` | W.J12 ↔ W.HORNO | Travesía del Horno | S | E0 | AP | 2.5–4.0 `st.historic_lane` | (68,8),(88,12),(110,22) | 45 m | 0.60 |
| `W22` | W.PLAZA ↔ W.J8C | Calleja Alta (1) | Q | E1 | AP | 2.5–3.0 `st.historic_lane` (passes F02/F07 gap) | (168,65),(168,93),(160,110),(150,114),(128,106) | 81 m | 1.10 |
| `W23` | W.J8C ↔ W.J13 | Calleja Alta (2) | Q | E1 | AP | 2.5–4.0 `st.historic_lane` | (128,106),(110,96),(96,84),(88,79) | 49 m | 0.65 |
| `W24` | W.J13 ↔ W.J05B | Pasadizo del Arco | S | E0 | AP | 2.5–3.0 `st.historic_lane` | (88,79),(104,72),(116,62),(121,48.5) | 48 m | 0.65 |
| `W25` | W.HORNO ↔ W.J8A | Senda de las Huertas | Q | E0 | AF | 1.5–2.0 dirt path between garden walls | (110,22),(125,28),(140,24),(155,8) | 54 m | 0.80 |
| `W26` | W.J8C ↔ W.X5 | Senda del Arroyo | Q | E1 | AF | 1.5–2.0 dirt path | (128,106),(114,116),(100,108),(92,108) | 41 m | 0.65 |
| `E06` | E.X5 ↔ E.LOW | Camino del Ensanche bajo | Q | E0 | AP | 3.0–4.0 `st.ordinary_road` (lower band) | (90,122),(84,132),(60,112),(35,88),(12,64) | 111 m | 1.50 |

`W25` intentionally parallels `W19` between the same nodes with a different character (quiet AF path versus secondary AP lane). It is a genuine route choice, not a duplicate.

**Geometry validation** (seed-frame polygons from CITY-03, lane buffered by half its drawn width, shapely): every new edge lies inside the hard polygon, outside `mask.rio` / `mask.arroyo` and outside the effective 3 m / 2 m bank shoulders, and does not intersect F01–F08, S01 or S03. `W26` reaches W.X5 through `stub.x5.wedge` only. `E06` stays within the Ensanche component and starts inside `stub.x5.ensanche`.

## 5. New open sites (CITY-03)

| ID | Planning region | Binding | Relation |
|---|---|---|---|
| `S04` Rincón del Tinte | ≈ 6 m radius pocket around (171,27) on W20 | `pc.open_site` | public pause; see P9 for tintorería use |
| `S05` Plazuela del Horno | ≈ 6.5 m radius pocket around (114,19) at W.HORNO | `pc.open_site` + `jn.irregular_fork` | public small square |
| `S06` La Era alta | ≈ 9 m radius dirt ground around (150,113) on W22 | `pc.open_site` | public quiet play ground (bolos may later be promoted by CITY-02) |
| `S07` Mirador del Arroyo | ≈ 4 m radius widening around (115,118) on W26 | `pc.open_site` | public bench/overlook, outside the 2 m Arroyo shoulder |

## 6. Resulting graph properties

- **New genuine cycles inside the seed**, for example:
  - `W.CASCO → W05a → W18 → W21 → W12a → W.CASCO`;
  - `W.CASCO → W05 → W.PLAZA → W22 → W23 → W13a → W.CASCO`;
  - `W.J13 → W24 → W05b/W05a → W.CASCO → W13a`;
  - `W.HORNO → W19 / W25 → W.J8A`.
- **Plaza-independent Casco ↔ Plaza alternates:** `W18 → W19 → W20`, and `W13a → W23 → W22`.
- **Second route to W.X5:** `W23 → W26`, besides `W13b`.
- **X5 closure truth is unchanged:** closing X5 still disconnects E.X5 and now also `E06 / E.LOW`. That is correct. No new Arroyo or Río crossing exists.
- The CITY-08 reserved slice (`W04 + F03 + F07 + S01`) is untouched: no edge attaches to W04.

## 7. Invariants preserved

No change to CITY-00 geography or crossings, the hard polygon, water masks, bank shoulders, the X1/X5 overlays, anchors of the accepted nodes, F01–F08 / S01–S03, place IDs, access roles or seams. No service or private space becomes a public shortcut: `W24` passes behind F06 on public land and never through S03.

## 8. Required downstream evidence

If accepted, CITY-07's asset-rich demo campaign (`CITY_GREYBOX_DEMO_VALIDATION_AMENDMENT.md`) adds:

- timing of each new edge;
- one follow/search run using `W18/W21` or `W24`;
- a closure test (block `W05b`, use `W24`/`W18`);
- quiet/busy reading of `W22/W23/W25/W26`;
- a check that `W22`'s 4 m gap between F02 and F07 remains comfortable.

## 9. Negative gates

FAIL if a realization adds any lane not listed here, widens a lane beyond its family band to fake a plaza, lets a dirt path become service access, crosses a bank shoulder, or treats this amendment as adoption of CITY-09 P2/P3 (paseo, stairs, covered passage, one-way drop).
