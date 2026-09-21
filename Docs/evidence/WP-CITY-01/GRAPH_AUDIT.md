# WP-CITY-01 — Graph and scenario audit

WP: `WP-CITY-01 — Mobility, district graph + walk-time topology`  
Semantic owner: `Docs/production/CITY_MOBILITY_TOPOLOGY.md` v1.2  
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

No `W*`, `E*` or `O*` internal edge changes landmass. The §5.5 district-family table is a projection of these edges and explicitly owns no new relation.

### Crossing counts

- crossing IDs across constitution life: **7 — X1..X7**;
- State 1 crossings coexisting: **6 — X1..X6**;
- State 2 crossings coexisting: **6 — X1..X5 + X7**;
- Arroyo crossings: **4 — X2..X5**;
- Río crossings in either state: **2 — X1 + X6/X7**.

Result: **PASS.**

## 2. Public base-graph connectivity

The public graph excludes `W17` because that edge is `AS` restricted service/back access and is forbidden as an oracle for ordinary public connectivity. `W08`, `O01` and `O02` carry a `V` service-capable character tag but remain public because their access is `AR`; route-character tags do not determine permission.

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

Representative surviving paths, using the directional `W11` stair cost correctly:

- `W.CASCO → W.LANDING → W.RIBERA → W.BARRIO → W.CM_NE → W.X2` = `W06-W08-W11(up)-W10-W02` = **16.55 min**;
- `E.HOME → X3 → W.BARRIO → W.RIBERA → W.LANDING → X7 → O.QUAY` = **16.0 min** because `W11` is downhill in this direction;
- the reverse Puerto→Ensanche version of that stair route is **17.0 min** because `W11` is uphill;
- `W.VEGA → W.RIBERA → W.LANDING → X7 → O.QUAY` = **15.5 min**.

The bypass is intentionally expensive. The invariant is connectivity without Plaza, not parity with the primary route.

Result: **PASS — CSI-03 preserved without using the restricted service edge.**

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

State-1 X6 routes add the separately modelled ferry wait and therefore can exceed motion-time targets door-to-door without rewriting the CITY-00 scale claim. `W_ferry` is a scenario variable, not a timetable or live schedule.

These are **planning** values. The fact that the arithmetic reconciles CITY-00 does not make a route measured.

## 5. CITY-00 deferred-cost closure

### Ensanche home → Puerto quay

- State 2 primary: **12.5 min** via X2 / commercial core / X7.
- State 2 permanent plaza-free: **16.0 min** via X3 / Barrio / Ribera / X7.
- reverse Puerto quay → Ensanche home on that stair route: **17.0 min** because the stairs are uphill.
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

The X4 detour adds about 3.8 planning minutes relative to X2, consistent in order of magnitude with CITY-00's accepted “~200 m” upstream cart detour rather than making X4 a costless parallel bridge.

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

## 8. Service-route and port logistics audit

`W17` is the only explicitly restricted `AS` service/back edge added by CITY-01.

State-2 full-cart routes:

- Puerto quay → Ribera workshop: `O02-X7-W08` = **7.0 min**;
- Puerto quay → representative Calle Mayor shop: `O02-X7-W08-W17` = **11.0 min**.

State 1 has no full-cart route from Puerto to the Wedge because CITY-01 grants neither X1 nor X6 to cart freight. The required delivery/logistics scenario remains viable after break-bulk to porter/carryable-load scale:

- quay → Ribera via X6: `O02-X6-W08` = **7.0 min + W_ferry**;
- quay → shop direct-X6 porter route: `O02-X6-W06-W05-W04` = **9.0 min + W_ferry**;
- quay → shop X1 fallback porter route: `O02-O01-X1-W12-W05-W04` = **10.75 min**.

This uses the accepted port break-bulk character instead of inventing ferry cart capacity. X1's inherited handcart capacity is preserved but is not used as an oracle for unreviewed downstream handcart clearance.

`W17` is not used in the Plaza-removal proof, the ordinary pedestrian benchmark matrix or any crossing-count claim.

Result: **PASS — both port↔workshop and port↔commercial logistics are represented.**

## 9. Rural/valley arrival audit

The contract's “rural/valley arrival → market/civic core” case is covered from both ordinary edge types rather than treating “arrival” as only a rural walk:

- Vega → Plaza: **6.5 min**;
- State-2 bus/valley arrival at Entrada → Plaza: `O03-O02-X7-W06-W05` = **9.5 min** after the passenger leaves the bus;
- State-1 direct X6 version: **9.5 min + W_ferry**;
- State-1 X1 fallback: `O03-O02-O01-X1-W12-W05` = **11.25 min**.

The bus terminates at Entrada in every case. No bus route into the core is inferred from the pedestrian continuation.

Result: **PASS.**

## 10. Permanent quiet-route audit

The original candidate used conditional low-water X5 to distinguish quiet evening movement. Pre-review rejected that as too availability-dependent for the required scenario.

The repaired scenario uses the same endpoints in ordinary permanent topology:

- Vega → landing by market/commercial route `W01-W02-W03-W04-W05-W06`: **10.5 min**;
- Vega → landing by protected low paseo `W07-W08`: **11.0 min**.

The 0.5-min planning difference is small enough that a context preference can plausibly select quietness without requiring a contrived time penalty, while the paths differ materially in district character and exposure.

Result: **PASS — the quiet route no longer depends on X5 or low water.**

## 11. Following/search branch audit

Required route: `E.HOME → X3 → W.BARRIO → W.RIBERA → W.LANDING → X6/X7 → O.PUERTO_UP`.

It is not a single corridor:

- at `W.BARRIO`, public alternatives include `W09` toward Vega, `W10` toward Calle Mayor, `W11` toward Ribera and `W15` toward X3;
- at `W.RIBERA`, public alternatives include `W07` toward Vega, `W08` toward landing and `W11` toward Barrio;
- at `W.LANDING`, public alternatives include `W06` toward Casco, `W08` toward Ribera and the state-valid Río crossing.

`W17` is not available to the follower unless the followed actor has legitimate service access.

Result: **PASS — following/search encounters real route-choice points across Ensanche, Wedge and Orilla-sur movement.**

## 12. Profile-boundary audit

- ordinary/slower pedestrian access remains on the public graph unless explicit legitimate `AS` access exists;
- slower-ped multipliers are unambiguous: 1.35 on E0/E1, 1.50 on E2/E3, never multiplied together;
- bicycle riding is deterministic: `AR` + `AP E0/E1` rideable, `AP E2/E3` dismount, `AF` inaccessible, X1 push-only, X6 not assumed, X7 rideable;
- service/delivery cart uses only `AR/AS`; no X1/X3/X5/X6 full-cart crossing is invented;
- bus terminates at Entrada; no core vehicle route is invented;
- following/search receives public junction choices and cannot silently use `AS` service access;
- time-sensitive is only a cost-selection scenario profile and does not claim runtime NPC omniscience;
- State-1 ferry carriage is deliberately underclaimed: porter/carryable load only.

Result: **PASS — CITY-01 supplies spatial affordances without taking Living World or vehicle-simulation ownership.**

## 13. Meaningful loops and non-universal connectors

CITY-01 acceptance requires meaningful loops, not merely graph connectivity.

### Loop examples derived from the same ledger

- **L1/L1′ cross-river loop:** Casco → X1 → camino sur → Puerto-upstream edge → X6/X7 → landing → Casco = **8.25 min base motion** plus State-1 ferry wait.
- **Arroyo residential loop:** Ensanche home → X2 → Calle-Mayor-side upper link → Barrio → X3 → Ensanche home = about **8.6 min**, using two differentiated permanent Arroyo crossings.
- **Wedge middle/low loop:** Vega → commercial middle route → landing → Ribera → quiet low paseo → Vega = about **21.5 min**.
- **Wedge upper/middle loop:** Vega → Barrio → Calle Mayor NE → Vega = about **10.5 min**.

The loops serve different movement characters rather than duplicating one hub spoke.

### Universal-connector challenge

Under normal availability:

- remove X2: pedestrians remain connected through X3/X4; X2 is important but not universal;
- remove X3: X2 remains permanent; X3 is not universal;
- remove X1: State 1 uses X6 and State 2 uses X7 for pedestrian Orilla-sur access;
- remove X6 in State 1: X1 remains;
- remove X7 in State 2: X1 remains for pedestrians/handcarts while cart freight intentionally waits;
- remove Plaza: §3 proves the public graph still connected without `W17`.

X4/X5 are seasonal/conditional by design and are never credited as the permanent floor.

Result: **PASS — meaningful loops exist and no plaza/bridge becomes a universal connector contrary to CITY-00.**

## 14. Required scenario matrix check

| WP scenario | Covered by production document | Audit result |
|---|---|---|
| upper/residential home → workplace | Barrio Alto → Ribera, `W11` | PASS |
| old quarter → port/work edge without Plaza | Casco → landing → X6/X7 → quay | PASS |
| port delivery → commercial destination | State-2 cart; State-1 porter after break-bulk | PASS |
| rural/valley arrival → market/civic core | Vega and Entrada/bus variants | PASS |
| player follows NPC across ≥2 district boundaries | Ensanche → X3 → Barrio → Ribera → landing → X6/X7 → Puerto | PASS |
| closure forces plausible alternate | X2 closure through X4/X3 | PASS |
| late actor chooses faster but contextually different route | X6 wait threshold versus X1 | PASS |
| quiet evening route differs meaningfully from market-day flow | permanent Vega→landing paseo versus commercial route | PASS |
| service/back route differs from public route | W17 restricted rear/service route | PASS |

## 15. CITY-04 bounded-measurement audit

Pre-review compared CITY-01's first measurement handoff against the accepted `WP-CITY-03` and `WP-CITY-04` contracts and found a real scope error:

- CITY-03 owns the exact retained-seed boundary and exact CITY-04 measurement pack;
- CITY-04 is permitted to build **only** that bounded retained seed;
- the first CITY-01 candidate incorrectly told CITY-04 to measure every §7.1 full-city benchmark, including routes such as Vega↔Puerto that can be outside the seed.

That would have created either impossible evidence or pressure for CITY-04 to violate its own scope.

v1.2 repairs the boundary:

- CITY-04 measures full routes only when complete geometry lies in the CITY-03 seed;
- it may classify represented route/elevation/access pieces as `CALIBRATED_COMPONENT`;
- absent full routes are `NOT_IN_SEED`, never “measured by projection”;
- CITY-03 chooses the applicable measurement subset without changing CITY-01 topology;
- remaining full-city measurement is preserved as an explicit downstream ownership residual rather than silently expanding CITY-04.

Result: **PASS — measurement questions are consumable without requiring out-of-scope Unity geometry.**

## 16. Pre-review findings repaired before freeze

The complete-diff challenge found six material correctness/clarity issues. All were repaired before freeze:

1. **Directional stair arithmetic:** one Plaza-removal sample used `W11`'s 6.5-min downhill value while travelling uphill. Corrected to 7.5 and the path total from 15.55 to **16.55 min**; reverse-direction Ensanche/Puerto cost is now explicit.
2. **Bicycle ambiguity:** “selected AP” did not actually select a deterministic rule. §6 now freezes ride/dismount/inaccessible treatment by access/elevation class and distinguishes X1/X6/X7.
3. **Quiet-route availability:** the first scenario depended on low-water X5. Replaced with permanent Vega→landing commercial-versus-paseo alternatives at 10.5/11.0 min.
4. **Arrival/bus incompleteness:** bus terminated at Entrada but the required valley-arrival continuation was not costed. State-1/State-2 pedestrian continuations to Plaza are now explicit.
5. **Route-tag/access ambiguity:** `V` originally sounded restricted even on public `S/V + AR` edges such as W08. Route character and permission are now separated; only `AS` restricts service access.
6. **Impossible CITY-04 full-city measurement claim:** the first handoff ignored CITY-04's exact-seed scope. v1.2 now separates measured full routes, calibrated components and `NOT_IN_SEED` residuals.

## 17. Worker-audit verdict

The rerun graph audit finds:

- no contradiction with CITY-00 connectivity;
- no hidden water crossing;
- no universal Plaza/bridge dependency;
- meaningful loops;
- no directional-cost arithmetic error in asserted examples;
- deterministic mobility-profile assumptions;
- every required scenario represented;
- port/workshop and rural/market ordinary movement represented;
- a municipal closure with a bounded nontrivial cost;
- measurement questions that respect CITY-03/CITY-04 scope.

Residuals are deliberately not hardened into fake proof:

- final metres/grades/stair counts;
- actual slow-ped multiplier;
- bicycle ride/dismount comfort and clearance;
- greybox route readability/followability;
- actual X2/X6 closure penalties where represented;
- actual service-cart clearance on represented `AR/AS` edges;
- whether the 10.5/11.0-min commercial-versus-quiet choice remains near-parity after geometry;
- full-city end-to-end measurement outside the retained seed.

These remain measurement or ownership residuals, not measured facts.
