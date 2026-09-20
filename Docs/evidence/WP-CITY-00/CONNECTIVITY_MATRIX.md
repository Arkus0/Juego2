# WP-CITY-00 — Authoritative connectivity matrix

WP: WP-CITY-00
Status: **single source of connectivity semantics for the selected constitution.**
Produced under the circuit-breaker rule in `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` after three
independent FAILs in one causal family.

Every other surface in this candidate — `CITY_SPATIAL_CONSTITUTION.md`, the topology dossier, the
CSI invariants, the comparison matrix, the refutation log, the handoff — **derives** from this file
and may not restate it in its own words. Where a surface needs a connectivity fact, it cites this
matrix. That rule exists because all three FAILs were one surface contradicting another.

## 0. Why this file exists rather than a fourth patch

| Cycle | Claim | Refuted by | Distance |
|---|---|---|---|
| 1 | the landing needs a navigability premise | the setting already licensed a landing needing none | same document |
| 2 | closing the Puente Viejo cuts the south side off | the barca, added in the same repair | same table |
| 3 | the south bank is never on a single crossing | the row directly above it | 8 lines |

Three patches, three new contradictions. The defect is not any of the three sentences; it is that
connectivity semantics lived in six places at once.

## 1. Areas

Connectivity is between **areas separated by water or by the slope**, not between district families.
District families are a content classification (`CITY_SPATIAL_CONSTITUTION.md` §2.3) and several sit
in the same area.

| Area | Contains | Separated from the wedge by |
|---|---|---|
| **Wedge** | Casco Viejo, Plaza, Calle Mayor, Ribera y Talleres, Barrio Alto, north huertas | — |
| **Ensanche** | Ensanche, north Ensanche | the arroyo |
| **Puerto** | Puerto Fluvial, Entrada y Carretera | nothing — it is the wedge's downstream tip |
| **Orilla sur** | camino sur, ermita, cementerio, valley road | the río |
| **Vega** | huertas and paths NE | nothing — continuous with the wedge |

Only two boundaries need crossing: **the arroyo** (Wedge ↔ Ensanche) and **the río**
(Wedge/Puerto ↔ Orilla sur).

## 2. Crossings — the complete set

| ID | Name | Water | Connects | Exists in | Availability |
|---|---|---|---|---|---|
| **X1** | Puente Viejo | río | Wedge (casco) ↔ Orilla sur | State 1 and 2 | permanent; pedestrian and handcart, not carts |
| **X2** | Puente del Mercado | arroyo | Wedge (Calle Mayor) ↔ Ensanche | State 1 and 2 | permanent; road-capable |
| **X3** | Pasarela del Lavadero | arroyo | Wedge (Barrio Alto) ↔ upper Ensanche | State 1 and 2 | permanent; foot only |
| **X4** | Puente de la Vega | arroyo | Wedge (north huertas) ↔ north Ensanche | State 1 and 2 | seasonal; flood-closable |
| **X5** | Pasos / vado | arroyo | Wedge (lower casco lanes) ↔ Ensanche | State 1 and 2 | low water only |
| **X6** | La barca | río | Puerto ↔ Orilla sur | **State 1 only** | hours and fare are a municipal decision; high water suspends it |
| **X7** | Puente del Muelle | río | Puerto ↔ Orilla sur (road) | **State 2 only** | permanent; carries carts |

Seven crossings across the constitution's life; **six co-exist in any one state** (X6 and X7 never
co-exist). Four cross the arroyo, three cross the río but only two at a time.

## 3. The two states

| State | Name | Río crossings | Transition |
|---|---|---|---|
| **State 1** | the barca era | X1 + X6 | from first build until the Puente del Muelle is built |
| **State 2** | the Puente del Muelle era | X1 + X7 | X7 replaces X6; expansion seam 6 |

The crossing set changes exactly once across production. No workpack yet owns *when* — see residual
in `WORKER_PRE_REVIEW.md`.

## 4. Río — crossings serving the Orilla sur

| Situation | Count | Which | Cost / character |
|---|---:|---|---|
| State 1, normal | **2** | X1, X6 | casco → ermita over X1, ≈80 m |
| State 1, high water | **1** | X1 | routine and seasonal; X6 suspended, south-bank traffic walks the length of town |
| State 1, X1 closed | **1** | X6 | casco → Cuesta → landing (≈150 m) → ferry → upstream on the camino sur (≈350 m): ≈500 m plus the wait, a detour of order 6× |
| **State 1, a flood takes both** | **0** | — | genuinely cut off; rare, bounded, and not contrived — one flood can suspend the ferry and threaten the old bridge the same day |
| State 2, normal | **2** | X1, X7 | both unconditional |
| State 2, X1 closed | **1** | X7 | a long detour, no condition attached |
| State 2, X7 closed | **1** | X1 | people still cross at the casco; **freight cannot cross at all**, since X1 takes handcarts and not carts, so the yards wait |

## 5. Arroyo — crossings serving the Ensanche

The ladder the previous cycles never built. The río was counted; the arroyo was not, and two
invariants depend on it.

| Situation | Count | Which |
|---|---:|---|
| low water, normal | **4** | X2, X3, X4, X5 |
| ordinary water | **3** | X2, X3, X4 — the pasos are under |
| flood | **2** | X2, X3 — the Vega bridge is closable and the pasos are gone |
| X2 closed, ordinary water | **2** | X3, X4; carts detour upstream to X4, ~200 m |

**Permanent floor: 2** (X2, X3). That is the number an invariant may promise; four is the number the
design provides.

## 6. Route counts and plaza dependence

The workpack's loop test is *remove the plaza and the city still connects*. Per adjacent pair, and
**excluding pairs of which the plaza is itself a member**:

| Pair | Route via the plaza | Plaza-free route | Plaza-free count | Conditional? |
|---|---|---|---:|---|
| Barrio Alto ↔ Ribera | escaleras → plaza → Calle Mayor → down | callejas altas → escaleras east → Ribera | 1 | no |
| Casco ↔ Ensanche | plaza → X2 | lower lanes → X5 | 1 | **yes — X5 is low water only** |
| Ensanche ↔ Puerto | X2 → plaza → Cuesta | quay road south from Entrada | 1 | no |
| Vega ↔ Puerto | Calle Mayor → plaza → Cuesta | paseo fluvial / camino de sirga the whole way | 1 | no |
| Barrio Alto ↔ Ensanche | plaza → X2 | X3 Pasarela del Lavadero | 1 | no |

Two facts the invariants must respect rather than overstate:

1. the plaza-free count is **one** per pair, not two;
2. **Casco ↔ Ensanche routes through the plaza in high water**, because its only plaza-free route is
   the pasos. That is a named seasonal condition, not a structural funnel — and it is content, not a
   defect. No redesign is warranted for it.

## 7. Port approaches

| Approach | From | Kind |
|---|---|---|
| camino de sirga / paseo fluvial | Ribera, along the water | land |
| carretera del muelle | Entrada and the valley road | land |
| Cuesta del Puerto | casco tip, steep | land |
| X6 (State 1) or X7 (State 2) | Orilla sur, across the río | water crossing / bridge |

**Three land approaches plus the río crossing = four.** The port is a destination, not a cul-de-sac,
in both states.

## 8. Loops

| ID | Loop | Exists in | Length |
|---|---|---|---|
| **L1** | casco → X1 → camino sur → X6 → Puerto → Cuesta → casco | **State 1 only** | ≈650–750 m + ferry wait |
| **L1′** | the same with X7 in place of X6 | **State 2 only** | ≈650–750 m, no wait |
| **L2** | any two arroyo crossings close a Wedge/Ensanche loop | both states | varies; 2–4 crossings available per §5 |
| **L3** | the three longitudinal routes plus their lateral links close loops along the wedge | both states | — |

L1 and L1′ are detours, not shortcuts: casco to landing over the Cuesta is ≈150 m directly.

## 9. Expansion seams

**Six.** This is the authoritative count; every surface citing it must say six.

| # | Direction | Note |
|---|---|---|
| 1 | NE along the río → vega, more Calle Mayor, a second huerta quarter | touches the seed |
| 2 | N up the arroyo → upper huertas, mill, hamlet | |
| 3 | Up-slope N/NE → Barrio Alto growth, terraces, miradores | |
| 4 | W across the arroyo → Ensanche growth | touches the seed; cheapest large residential expansion |
| 5 | SW downstream → port growth, yards, depot, boatyard | touches the seed via the Cuesta |
| 6 | S across the río at X7 | **the State 1 → State 2 transition.** It is also the only seam that would promote a *transitional* area (Orilla sur, `CITY_SPATIAL_CONSTITUTION.md` §4) into fabric — a CITY-02/CITY-03 decision, not one taken here |

## 10. How invariants must be phrased

The root of all three FAILs, stated once: **a connectivity invariant that promises an availability
count is refutable by weather.** Every CSI invariant touching connectivity therefore declares which
of two things it binds:

- **design** — what the constitution must provide. Not refutable by a closure or a flood.
- **availability** — how many crossings or routes are usable right now. Owned by §4, §5 and §6 of
  this matrix, never asserted in an invariant.

Applied: CSI-03, CSI-06 and CSI-07 all bind design. Their availability consequences live here.

## 11. What this audit did *not* find

The circuit breaker permits reopening the selected topology if the audit exposes a structural
contradiction. It does not.

Every contradiction found is a **documentation** defect — a surface restating connectivity in its own
words and drifting. The one finding with real spatial content, Casco ↔ Ensanche routing through the
plaza in high water, is a seasonal condition worth keeping rather than a funnel worth removing.

**Topology B stands. The city is not redesigned.**
