# Topology B — "Cuña de Confluencia" (confluence wedge)

WP: `WP-CITY-00`
Status: selected option after refutation, amended through repair cycle 5.

## Repair history

- **Cycle 1:** removed an unauthorized navigability premise and corrected CITY→ART authority.
- **Cycle 2:** reconciled the added barca with the graph and closure consequences.
- **Cycle 3:** removed a false single-crossing absolute and separated design from availability.
- **Cycle 4:** independent review #5261556774 proved the previous planar embedding impossible: a
  wedge between two channels cannot continue dry past their confluence. The landmasses are now fixed
  first in `PLANAR_EMBEDDING.md`, and connectivity is regenerated in `CONNECTIVITY_MATRIX.md`.
- **Cycle 5:** independent review #5261646488 found that the ASCII graph still visually connected the
  Ensanche bank to Puerto even though the matrix defines no such edge. The graph now renders the
  landmasses separately and treats only explicit labeled connectors as graph edges.

The selected topology survives. Cycle 5 changes no landmass, crossing, route count or scale claim; it
removes a contradictory rendering of the already-correct cycle-4 embedding.

## 1. Premise and bank choice

Two watercourses meet at the south tip of the settlement:

- **Río:** main valley river, arriving from the NE, ~18–25 m wide above the confluence;
- **Arroyo:** tributary from the N, 4–10 m wide, incised and crossed casually/often.

The historic city occupies the **Wedge between them** and the Wedge ends at the confluence. The
Ensanche lies outside the Arroyo. The Orilla sur lies outside the Río and continues downstream as the
south/east bank of the joined river.

**Puerto Fluvial + Entrada / Bus / Carretera occupy that Orilla-sur downstream bank.** They do not
continue the Wedge. The direct core-to-port route therefore crosses the joined river at the landing:
La barca in State 1, Puente del Muelle in State 2. The crossing lands at the **upstream edge of the
Puerto district**, which extends downstream from that bridgehead.

That is the topology's physical embedding. All route/crossing counts derive from
`CONNECTIVITY_MATRIX.md`.

## 2. Semantic graph

Only explicit labeled connectors below are graph edges. Whitespace, columns and landmass grouping do
not imply connectivity.

```text
[ENSANCHE BANK]                         [WEDGE]

ENSANCHE == X2-X5 / ARROYO == CALLE MAYOR =========== PLAZA / AYUNTAMIENTO
                                      |        \              |      \
                                RIBERA / TALLERES       CASCO VIEJO   \
                                      |                      |         \
                               PASEO / SIRGA ------ landing head       \
                                      |                  *              \
                            BARRIO ALTO / callejas altas   \              \
                                      |                     \              \
                               VEGA / HUERTAS (NE)           \              \
                                                             \              \
                                             X6 barca / X7 bridge    X1 PUENTE VIEJO
                                                   |                       |
                                                   v                       v

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ joined RÍO / water boundary ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

[ORILLA SUR]

                             upstream PUERTO edge ---- PUERTO FLUVIAL ---- camino sur ---- X1 far end
                                      |                      |                                  |
                                      +----------------------+                          ermita/cementerio
                                                             |
                                                    ENTRADA / BUS / CARRETERA
                                                             |
                                                   valley / downstream road
```

Topological meaning, not metric drawing:

- the Wedge stops at `landing head` / confluence tip;
- X1 reaches Orilla sur near the casco, then `camino sur` stays dry on that landmass to Puerto;
- X6/X7 reaches the upstream edge of Puerto on the same Orilla-sur landmass downstream;
- Entrada and the valley road are dry-continuous with Puerto;
- **no Ensanche-bank ↔ Orilla-sur edge exists.** Any Ensanche→Puerto route first crosses X2–X5 to
  the Wedge and then uses X1 or X6/X7 as defined by the matrix.

## 3. Longitudinal route families

Three designed route families still structure the Wedge:

| Level | Route | Character |
|---|---|---|
| low | **Paseo / sirga** | quiet riverside route to the confluence-tip landing head; reaches Puerto through X6/X7 |
| middle | **Calle Mayor** | commercial terrace spine |
| high | **Callejas altas** | stepped residential route through Barrio Alto |

The low family is continuous **by design**, not by weather guarantee: State-1 high water can suspend
X6, in which case Puerto is reached through X1 + camino sur instead. `CONNECTIVITY_MATRIX.md` owns
availability.

## 4. Required spatial ingredients

| # | Ingredient | Placement |
|---|---|---|
| 1 | old quarter / historic lanes | Casco Viejo on rocky Wedge tip above confluence |
| 2 | civic core / town hall / plaza-market | first terrace behind casco |
| 3 | commercial everyday spine | Calle Mayor along middle terrace NE |
| 4 | substantial residential beyond old quarter | Barrio Alto + Ensanche, two distinct characters |
| 5 | river corridor / waterfront paths | Wedge paseo/sirga, Arroyo ravine walk, south-bank camino |
| 6 | old bridge + future crossing strategy | X1 Puente Viejo; X2-X5 Arroyo crossings; X6 barca → X7 Puente del Muelle |
| 7 | fluvial-port / wharf-storage-work | downstream on Orilla-sur bank of joined river |
| 8 | workshops/service/peripheral edge | Ribera y Talleres on Wedge river edge |
| 9 | outward road/bus/valley connection | Entrada beside Puerto on Orilla sur; valley road continues outward |
| 10 | rural edge | Vega NE, Arroyo huertas N, south-bank ermita/cemetery road, upper slopes |
| 11 | expansion seams | six, owned by matrix §9 |
| 12 | intentionally quiet family | upstream paseo, ravine/lavadero, upper vega lanes |

## 5. Crossing strategy

Complete set and availability: `CONNECTIVITY_MATRIX.md` §§2–5.

Summary of design only:

- four Arroyo crossings connect Wedge ↔ Ensanche bank;
- X1 connects core Wedge ↔ Orilla sur;
- X6 in State 1, replaced by X7 in State 2, connects the confluence-tip landing head ↔ upstream Puerto
  edge on Orilla sur;
- each state therefore designs exactly two Río crossings, and the prior 2 → 1 → 0 State-1
  availability ladder remains intact.

There is **no** Puerto ↔ Orilla-sur crossing because Puerto is on Orilla sur. There is likewise **no
Ensanche-bank ↔ Orilla-sur crossing**. Any route between those landmasses necessarily traverses the
Wedge through one of X2–X5 and then a named Río crossing.

## 6. Port logic and approaches

The landing sits below the confluence where the joined water is wider/slower and braided over gravel
bars. Its existence still needs no long-distance navigability premise: timber rafting at high water,
áridos from bars, the ferry, road break-bulk, fishing and ordinary waterfront work are sufficient.

Approach families are now physically honest:

1. valley road / Entrada → Puerto, dry on Orilla sur;
2. Puente Viejo far end → camino sur → Puerto, dry after X1;
3. Casco / Cuesta → X6 or X7 → upstream Puerto edge;
4. Ribera / paseo → confluence-tip landing head → X6 or X7 → upstream Puerto edge.

The last two share the same named crossing and are not double-counted. The port is not a cul-de-sac:
it has two named dry approach directions on its own bank plus the direct core crossing.

## 7. Axis profile

**Mobility / loops.** Strong. Four cheap Arroyo crossings, three Wedge levels and the two Río
crossings per state create local and territorial route choices. L1/L1′ are explicitly planar in the
matrix.

**River / port logic.** Stronger after repair. The Río is now undeniably a territorial boundary:
Puerto is on the opposite landmass, and core access crosses it. The port's own bank also carries the
road and south-bank camino, so river logic does not turn it into a mission appendix.

**Reactive density.** Adequate→Strong. The high-intensity core remains contiguous on the Wedge.
Ensanche is a deliberate lower-intensity bank across the Arroyo; Puerto/Entrada form a compact working
edge across the Río rather than a second full town centre.

**Schedules / travel.** Strong. Short Arroyo crossings generate local routine; the port shift is a
real cross-river commute; Puente Viejo gives the rural trip; Barrio Alto supplies vertical routine.

**Investigation / following.** Strong. Targets can change level, bank and crossing choice. A port-bound
target can take X1 + camino sur or the landing-side crossing when available.

**Governance.** Strong and granular. Arroyo closures are small levers; ferry hours/fare and X7 repair
are territorial levers; quay concessions, market siting and road access all have walkable addresses.

**Expansion.** Six directions remain; seams 5–6 are corrected to the port bank / crossing transition.

**Production cost.** Medium. Four cheap Arroyo crossings; one later expensive Río bridge; a small
phaseable port; retained seed remains on the Wedge tip.

## 8. Dimensional sketch

| Part | Approx. extent | Approx. area |
|---|---|---|
| Wedge (casco + plaza + Calle Mayor + Ribera) | 600 m × 150→350 m | ≈0.15 km² |
| Ensanche | 350 × 250 m | ≈0.09 km² |
| Barrio Alto | 300 × 200 m | ≈0.06 km² |
| Puerto Fluvial | 300 × 150 m | ≈0.045 km² |
| Entrada / carretera | — | ≈0.02 km² |
| **dense fabric total** | — | **≈0.365 km²** |
| playable envelope | river corridor, vega, ravine, camino sur, roads, slopes | ≈1.0–1.2 km² |

Longest ordinary route remains a planning hypothesis of ≈1.05–1.15 km from upper Vega to Puerto.

## 9. Expansion seams

Owned by `CONNECTIVITY_MATRIX.md` §9:

1. NE along Río/Vega;
2. N up Arroyo;
3. up-slope N/NE;
4. W across Arroyo into Ensanche;
5. downstream **on Orilla sur** from Puerto/Entrada;
6. X6 → X7 landing-crossing transition, with any later south-bank urbanization left to CITY-02/03.

No seam requires the Wedge to continue through the confluence.

## 10. Retained seed

The Wedge tip remains the natural retained seed: casco lanes, plaza edge, Puente Viejo head, bar, one
Arroyo crossing, river edge, and the first metres of the Cuesta down to the **landing-side crossing
head**. It can visibly point to the future/remote Puerto across the joined river without building the
whole port.

This preserves the 0.03–0.06 km² seed and the blueprint's retained-first logic.

## 11. Known weaknesses / carried risks

- State-1 high water removes the direct landing ferry and makes the port a long detour through X1.
- The low longitudinal route therefore has conditional availability at its port end in State 1.
- Three levels plus two watercourses remain the hardest option to read and block out; CITY-04 owns
  measured validation.
- The landing still needs ordinary non-port services at Entrada/Puerto (CSI-04) so it does not become
  a work-only appendix.
- The timing of X6 → X7 remains unassigned.

None of those is a hidden planarity assumption.