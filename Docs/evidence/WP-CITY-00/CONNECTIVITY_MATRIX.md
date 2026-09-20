# WP-CITY-00 — Authoritative connectivity matrix

WP: `WP-CITY-00`
Status: **single source of connectivity semantics for the selected constitution.**
Repair cycle: 4, after independent review #5261556774 proved the prior source itself was spatially
impossible.

Every other surface in this candidate derives crossing, route, loop and seam claims from this file.
Physical realizability is checked separately in `PLANAR_EMBEDDING.md`; this matrix may not contradict
that embedding.

## 0. Repair-cycle-4 causal correction

The previous matrix treated the Puerto as a dry continuation of a wedge that ends at the confluence.
That was impossible. The corrected bank choice is explicit:

- the **Wedge** lies between Río and Arroyo and **ends at the confluence**;
- the **Ensanche bank** lies outside the Arroyo;
- the **Orilla sur** lies outside the Río and continues onto the south/east bank of the joined river;
- **Puerto + Entrada occupy that Orilla-sur landmass downstream of the confluence**;
- no dry Wedge → Puerto edge exists;
- X6/X7 are the landing-side río crossing from the confluence-tip head to the **upstream edge of the
  Puerto district**; the port extends downstream from that bridgehead, so the crossing spans the
  channel, not the district's 300 m length.

`PLANAR_EMBEDDING.md` is the bounded proof that every inter-landmass edge below names a crossing.

## 1. Areas / landmasses

Connectivity is between areas separated by water or slope, not between district-family labels.

| Area | Contains | Relation to Wedge |
|---|---|---|
| **Wedge** | Casco Viejo, Plaza, Calle Mayor, Ribera y Talleres, Barrio Alto, north huertas | source landmass; terminates at confluence |
| **Ensanche bank** | Ensanche, north Ensanche, west-bank huertas | across the Arroyo |
| **Orilla sur** | camino sur, ermita, cementerio, valley road, **Puerto Fluvial, Entrada / Bus / Carretera** | across the Río; continues downstream as one bank of the joined river |
| **Vega** | upper huertas and paths NE inside/along the Wedge | dry-continuous with Wedge |

Two water boundaries matter: **Arroyo** (Wedge ↔ Ensanche bank) and **Río / joined river**
(Wedge ↔ Orilla sur). The Puerto is not a fourth landmass and is not dry-continuous with the Wedge.

## 2. Crossings — complete set

| ID | Name | Water | Connects | Exists in | Availability |
|---|---|---|---|---|---|
| **X1** | Puente Viejo | Río | Wedge (casco) ↔ Orilla sur | State 1 and 2 | permanent; pedestrian and handcart, not carts |
| **X2** | Puente del Mercado | Arroyo | Wedge (Calle Mayor) ↔ Ensanche bank | State 1 and 2 | permanent; road-capable |
| **X3** | Pasarela del Lavadero | Arroyo | Wedge (Barrio Alto) ↔ upper Ensanche | State 1 and 2 | permanent; foot only |
| **X4** | Puente de la Vega | Arroyo | Wedge (north huertas) ↔ north Ensanche | State 1 and 2 | seasonal; flood-closable |
| **X5** | Pasos / vado | Arroyo | Wedge (lower casco lanes) ↔ Ensanche | State 1 and 2 | low water only |
| **X6** | La barca | joined Río | Wedge confluence-tip head ↔ upstream Puerto edge on Orilla sur | **State 1 only** | hours and fare are municipal; high water suspends it |
| **X7** | Puente del Muelle | joined Río | Wedge confluence-tip head ↔ upstream Puerto edge / port road on Orilla sur | **State 2 only** | permanent; carries carts |

Seven crossings across the constitution's life; **six coexist in either state** because X6 and X7 are
mutually exclusive. Four cross the Arroyo. Three IDs cross the Río across the constitution's life,
but only two Río crossings exist in a state: X1 + X6, then X1 + X7.

No other water crossing is required by the planar embedding.

## 3. The two connectivity states

| State | Name | Río crossings | Transition |
|---|---|---|---|
| **State 1** | barca era | X1 + X6 | first build until Puente del Muelle |
| **State 2** | Puente del Muelle era | X1 + X7 | X7 replaces X6 one-for-one |

No workpack yet owns *when* the transition occurs. CITY-01 may model both; CITY-02/03 may later own
production consequences if explicitly assigned.

## 4. Río availability — crossings serving Orilla sur / Puerto bank

| Situation | Count | Which | Consequence |
|---|---:|---|---|
| State 1, normal | **2** | X1, X6 | core has a short historic crossing and a landing-side ferry |
| State 1, high water | **1** | X1 | X6 suspended; core reaches Puerto by X1 then camino sur |
| State 1, X1 closed | **1** | X6 | south bank and Puerto remain reachable through the landing-side ferry; long rural trips detour to the tip |
| **State 1, flood takes both** | **0** | — | Orilla sur is cut off from the Wedge; rare and bounded |
| State 2, normal | **2** | X1, X7 | both unconditional |
| State 2, X1 closed | **1** | X7 | all core↔south-bank movement detours to the port bridge |
| State 2, X7 closed | **1** | X1 | people/handcarts cross at X1; cart freight cannot cross until X7 reopens |

The 2 → 1 → 0 ladder from the prior repaired candidate is preserved. What changed is the correct
landing-side endpoint: X6/X7 connect **the Wedge tip to Puerto on Orilla sur**, not Puerto to an
Orilla-sur landmass that the old matrix simultaneously treated as separate from Puerto.

## 5. Arroyo availability — crossings serving Ensanche

| Situation | Count | Which |
|---|---:|---|
| low water, normal | **4** | X2, X3, X4, X5 |
| ordinary water | **3** | X2, X3, X4 |
| flood | **2** | X2, X3 |
| X2 closed, ordinary water | **2** | X3, X4; carts detour upstream to X4, ~200 m |

**Permanent floor: 2** (X2, X3). Four is design provision; two is the flood floor.

## 6. Route counts and plaza dependence

The WP test is: **remove the plaza node and the designed city remains connected.** Availability can
remove a short bypass and impose a long detour without invalidating that design claim.

Representative pairs, excluding pairs containing the plaza itself:

| Pair | Route using plaza | Plaza-free designed route | Availability note |
|---|---|---|---|
| Barrio Alto ↔ Ribera | escaleras → plaza → Calle Mayor → down | callejas altas → east stairs → Ribera | unconditional |
| Casco ↔ Ensanche | plaza → X2 | low water: lower lanes → X5; high water: Cuesta → landing head → paseo → Ribera → east stairs → Barrio Alto → X3 | X5 disappears in high water, so the plaza-free trip becomes a long detour rather than vanishing |
| Ensanche ↔ Puerto | X2 → Calle Mayor → plaza → lower casco → X6/X7 (or X1 in high water) | X3 → Barrio Alto → callejas altas → east stairs → Ribera → paseo → X6/X7; if X6 is suspended, paseo/lower-casco route → X1 → camino sur | designed plaza-free route exists; exact cost is CITY-01 work |
| Vega ↔ Puerto | Calle Mayor → plaza → X1 → camino sur | paseo/Ribera → confluence-tip head → X6/X7; if X6 is suspended, paseo/lower-casco route → X1 → camino sur | designed plaza-free route exists; exact cost is CITY-01 work |
| Barrio Alto ↔ Ensanche | plaza → X2 | X3 Pasarela del Lavadero | unconditional |

The matrix does **not** claim two plaza-free routes per pair or that the shortest bypass is always
available. In particular, high water removes Casco ↔ Ensanche's direct X5 bypass; the surviving
plaza-free route is intentionally much longer.

## 7. Port approaches

The port is on Orilla sur downstream. It is **not** dry-connected to the Wedge.

| Route family | From | Water crossing on that trip? |
|---|---|---|
| **valley-road approach** | Entrada / Bus / Carretera → Puerto | none; same landmass |
| **south-bank approach** | X1 far end / ermita / camino sur → Puerto | X1 only if trip began in Wedge |
| **core landing approach** | Casco / lower Cuesta → X6 or X7 → upstream Puerto edge | X6/X7 |
| **Ribera / paseo approach** | Ribera → paseo/sirga → confluence-tip head → X6/X7 → upstream Puerto edge | X6/X7 |

The last two are distinct route families to the same landing-side crossing. They are **not** counted
as separate crossings. The port has two named dry approach directions on its own bank plus the direct
core crossing and therefore is not a cul-de-sac.

`Cuesta del Puerto` ends at the Wedge-side landing head. `Paseo/sirga` reaches the same head. Neither
continues as imaginary dry ground through the confluence. X6/X7 lands at the port's upstream edge;
the working frontage and yards extend downstream from there.

## 8. Loops

| ID | Loop | Exists in | Character |
|---|---|---|---|
| **L1** | casco → X1 → camino sur → Puerto → X6 → confluence-tip head → Cuesta → casco | **State 1 only** | ≈650–750 m + ferry wait |
| **L1′** | same with X7 replacing X6 | **State 2 only** | ≈650–750 m, no wait |
| **L2** | any two available Arroyo crossings close a Wedge/Ensanche loop | both states | 2–4 crossings available by §5 |
| **L3** | low/middle/high Wedge corridors plus lateral links close internal loops | both states | low corridor reaches the port through X6/X7, not by dry continuation |

L1/L1′ are now planar by construction: every dry segment stays on one named landmass and every
change of landmass is X1 plus X6/X7.

## 9. Expansion seams

**Six** named seams remain, but seam 5 and seam 6 are corrected to match the bank choice.

| # | Direction | Note |
|---|---|---|
| 1 | NE along Río / Vega → more Calle Mayor, huerta quarter | touches retained wedge fabric |
| 2 | N up Arroyo → upper huertas, mill, hamlet | |
| 3 | up-slope N/NE → Barrio Alto growth, terraces, miradores | |
| 4 | W across Arroyo → Ensanche growth | cheapest large residential expansion |
| 5 | downstream on **Orilla sur** from Puerto / Entrada → yards, depot, boatyard, road-edge growth | port-bank build-out; no dry Wedge continuation implied |
| 6 | landing crossing transition X6 → X7 | State 1 → State 2; may later support more south-bank fabric, but CITY-00 does not schedule or programme it |

None requires moving Plaza, Puente Viejo, Calle Mayor or the quay.

## 10. How connectivity invariants are phrased

A connectivity invariant binds **design**, not momentary availability, unless explicitly stated.
Weather/closure availability lives in §§4–5.

Applied:

- CSI-02 binds three designed longitudinal route families; its low route reaches the port through the
  landing-side crossing and may be unavailable at X6 during State-1 high water.
- CSI-03 binds plaza independence by design. Seasonal water can replace a short plaza-free route with
  a much longer one, but the base graph still remains connected with the plaza node removed.
- CSI-06 binds two designed Río crossings per state: X1 plus X6/X7.
- CSI-07 binds four designed Arroyo crossings, with two permanent.

## 11. Physical-realizability audit

This matrix was regenerated **after** the bank choice, not before it.

| Claim | Check |
|---|---|
| Wedge ends at confluence | PASS — no edge crosses the confluence vertex as dry land |
| Puerto downstream of confluence | PASS — on Orilla-sur bank of joined river |
| X6/X7 span | PASS — channel crossing to upstream Puerto edge; port extends downstream from bridgehead |
| Casco/Ribera → Puerto physical | PASS — routes reach X6/X7 head, then cross water |
| Entrada → Puerto physical | PASS — same Orilla-sur landmass |
| Every inter-landmass edge counted | PASS — X1..X7 complete under `PLANAR_EMBEDDING.md` |
| L1/L1′ planar | PASS — each landmass change is a named crossing |
| seam 5 planar | PASS — lives on port bank, not through the Wedge tip |

**Repair-cycle-4 verdict:** Topology B survives, but with a real spatial correction: the Puerto moves
from an impossible implied continuation of the Wedge to the Orilla-sur downstream bank. That changes
approach semantics, route tables and two expansion seams; it does not change the selected scale,
landing rationale, two-state model or south-bank availability ladder.