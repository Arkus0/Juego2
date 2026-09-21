# WP-CITY-01 — Graph and scenario audit

WP: `WP-CITY-01 — Mobility, district graph + walk-time topology`  
Semantic owner: `Docs/production/CITY_MOBILITY_TOPOLOGY.md`  
Baseline: `7fe44840076eba05f1b67a7633cd33fc67b9023d`

This file is **evidence only**. It does not define graph edges, costs or access. The authoritative semantics live in `CITY_MOBILITY_TOPOLOGY.md` §5. The audit projects that ledger and checks it against accepted CITY-00 facts and the CITY-01 acceptance criteria.

## 1. CITY-00 compatibility

### Inter-landmass edge inventory

| Relation | Allowed by CITY-00 | CITY-01 edges | Result |
|---|---|---|---|
| Wedge ↔ Ensanche bank | X2, X3, X4, X5 | exactly X2, X3, X4, X5 | PASS |
| Wedge ↔ Orilla sur | X1 plus X6 or X7 by state | exactly X1, X6, X7 with X6/X7 mutually exclusive | PASS |
| Ensanche bank ↔ Orilla sur | none | none | PASS |
| dry Wedge ↔ Puerto | none | none | PASS |

No `W*`, `E*` or `O*` internal edge changes landmass.

### Crossing counts

- crossing IDs across constitution life: **7 — X1..X7**;
- State 1 crossings coexisting: **6 — X1..X6**;
- State 2 crossings coexisting: **6 — X1..X5 + X7**;
- Arroyo crossings: **4 — X2..X5**;
- Río crossings in either state: **2 — X1 + X6/X7**.

Result: **PASS.**

## 2. Public base-graph connectivity

The public graph excludes `W17` because that edge is `AS` service/back access and is forbidden as an oracle for ordinary public connectivity.

The graph remains connected in all six design/availability combinations below:

| Río state | Arroyo availability | Expected crossings | Public graph connected? |
|---|---|---|---|
| State 1 | low water | X1-X6 | YES |
| State 1 | ordinary | X1-X4 + X6 | YES |
| State 1 | flood floor | X1-X3 + X6 | YES |
| State 2 | low water | X1-X5 + X7 | YES |
| State 2 | ordinary | X1-X4 + X7 | YES |
| State 2 | flood floor | X1-X3 + X7 | YES |

The rare CITY-00 case where both State-1 Río crossings are unavailable intentionally disconnects Orilla sur; CITY-01 does not erase that inherited 2→1→0 availability ladder.

## 3. Plaza-removal test

Test surface: State 2, ordinary Arroyo water, public edges only, `W.PLAZA` removed, `W17` excluded.

Result: **graph remains connected**.

Representative surviving paths:

- `W.CASCO → W.LANDING → W.RIBERA → W.BARRIO → W.CM_NE → W.X2` = `W06-W08-W11-W10-W02`, about **15.55 min**;
- `E.HOME → X3 → W.BARRIO → W.RIBERA → W.LANDING → X7 → O.QUAY` = **16.0 min**;
- `W.VEGA → W.RIBERA → W.LANDING → X7 → O.QUAY` = **15.5 min**.

The bypass is intentionally expensive. The invariant is connectivity without Plaza, not parity with the primary route.

Result: **PASS — CSI-03 preserved without using the service edge.**

## 4. Inherited walk-time reconciliation

All values below are sums of §5 edge weights.

| Benchmark | Edge sum | Target from CITY-00 | Result |
|---|---:|---:|---|
| Plaza → Casco | `W05 = 1.75` | 1.5–2 | PASS |
| Ensanche home → Plaza | `E01+X2+W03+W04 = 4.0` | 4–5 | PASS |
| Barrio Alto → Ribera | `W11 = 6.5` downhill | 6–7 | PASS |
| Vega → Plaza | `W01+W02+W03+W04 = 6.5` | 6–7 | PASS |
| X1 far head → Entrada | `O01+O02+O03 = 7.5` | 7–8 | PASS |
| Puerto quay → Calle Mayor shop | `O02+X7+W06+W05+W04 = 9.0` | 8–9 | PASS |
| landing/casco tip → NE Calle Mayor | `W06+W05+W04+W03+W02 = 8.0` | 8–10 | PASS |
| Vega → Puerto quay, primary/middle | `W01+W02+W03+W04+W05+W06+X7+O02 = 15.0` | 15–17 | PASS |
| Vega → Puerto quay, plaza-free low | `W07+W08+X7+O02 = 15.5` | 15–17 | PASS |

State-1 X6 routes add the separately modelled ferry wait and therefore can exceed motion-time targets door-to-door without rewriting the CITY-00 scale claim.

## 5. CITY-00 deferred-cost closure

### Ensanche home → Puerto quay

- State 2 primary: **12.5 min** via X2 / commercial core / X7.
- State 2 permanent plaza-free: **16.0 min** via X3 / Barrio / Ribera / X7.
- low-water X5 shortcut: about **10–11 min**, explicitly excluded from permanent redundancy.
- State 1 high-water fastest ordinary route after X6 suspension: **14.25 min** via X2 / core / X1 / camino sur.
- State 1 high-water plaza-free fallback: about **22–23 min**, intentionally expensive rather than replaced by an invented shortcut.

### Vega → Puerto quay

- State 2 primary: **15.0 min**;
- State 2 plaza-free low: **15.5 min**;
- State 1 X6-suspended primary fallback via X1: **16.75 min**.

Result: **PASS — the two costs explicitly deferred by CITY-00 are now bounded and do not require a new crossing.**

## 6. X2 municipal-closure audit

Baseline `E.HOME → W.PLAZA` through X2: **4.0 min**.

With X2 closed:

- ordinary-water path through X4: **7.8 min**;
- if flood also removes X4, permanent X3 path: **9.0 min**;
- service/cart traffic can use X4 while it exists;
- if X2 and X4 are both unavailable, cart crossing of the Arroyo pauses but the pedestrian city remains connected through X3.

This is a material but bounded consequence. X2 is therefore a legitimate municipal lever rather than a cosmetic toggle or a city-wide deadlock.

Result: **PASS.**

## 7. Río alternate-route audit

### State 1 — ferry tradeoff

`W.CASCO → O.QUAY`:

- X6 direct: **6.75 min + `W_ferry`**;
- X1 + camino sur: **8.5 min**.

Threshold: if expected ferry wait exceeds about **1.75 min**, the longer-distance X1 route becomes the faster expected-time route.

When X6 is suspended:

- Casco → quay remains **8.5 min** through X1;
- Ensanche home → quay remains **14.25 min** through X1;
- Vega → quay remains **16.75 min** through X1.

### State 2 — X7 closure

- pedestrians remain connected to Puerto through X1;
- cart/service freight has **no** Río crossing while X7 is closed, matching accepted CITY-00 semantics.

Result: **PASS.**

## 8. Service-route separation

`W17` is the only explicitly restricted service/back edge added by CITY-01.

State-2 cart route from Puerto quay to the representative Calle Mayor shop:

`O02 → X7 → W08 → W17` = **11.0 min** planning cost.

State 1 has no cart route from Puerto to the Wedge because CITY-01 grants neither X1 nor X6 to cart freight. This forces the accepted Puerto break-bulk logic to matter: goods must transfer to handcart/porter scale rather than acquiring an unreviewed ferry capacity.

`W17` is not used in the Plaza-removal proof, the ordinary pedestrian benchmark matrix or any crossing-count claim.

Result: **PASS.**

## 9. Required scenario matrix check

| WP scenario | Covered by production document | Audit result |
|---|---|---|
| upper/residential home → workplace | Barrio Alto → Ribera, `W11` | PASS |
| old quarter → port/work edge without Plaza | Casco → landing → X6/X7 → quay | PASS |
| port delivery → commercial destination | State-2 cart service route; State-1 break-bulk boundary | PASS |
| rural/valley arrival → market/civic core | Vega → Calle Mayor → Plaza | PASS |
| follow NPC across ≥2 district boundaries | Ensanche → X3 → Barrio → Ribera → landing → X7 → Puerto | PASS |
| closure forces plausible alternate | X2 closure through X4/X3 | PASS |
| late actor chooses faster but different route | X6 wait threshold versus X1 | PASS |
| quiet evening route differs from market-day flow | conditional X5/lower-lane route versus X2/Plaza flow | PASS |
| service/back route differs from public route | W17 rear/service route | PASS |

## 10. Profile-boundary audit

- ordinary/slower pedestrian access remains on public graph;
- bicycle does not magically ride stairs; E2/E3/AF require dismount or exclusion;
- bus terminates at Entrada; no core vehicle route is invented;
- following/search receives public junction choices and cannot silently use `AS` service access;
- time-sensitive is only a cost-selection scenario profile and does not claim runtime NPC omniscience;
- State-1 ferry carriage is deliberately underclaimed: porter/carryable load only.

Result: **PASS — CITY-01 supplies spatial affordances without taking Living World or vehicle-simulation ownership.**

## 11. Acceptance verdict for Worker pre-review input

The graph audit finds no contradiction with CITY-00 connectivity, no hidden water crossing, no universal Plaza dependency, and no scenario that requires route invention outside the semantic ledger.

Residuals are measurement questions, not silently hardened assumptions:

- final metres/grades/stair counts;
- actual slow-ped multiplier;
- bicycle roll/dismount behaviour;
- greybox route readability/followability;
- actual X2/X6 closure penalties;
- actual service-cart clearance.

These are handed to CITY-04 rather than misrepresented as measured evidence.
