# Keeper City — Spatial Constitution and Scale Envelope

Version: 1.1 — 2026-09-20
Workpack: `Docs/workpacks/CITY/WP-CITY-00.md`
Class: **PRODUCT / SPATIAL PREPRODUCTION — NON-FOUNDATIONAL**

Status: this document answers **what city we are making**. It creates no H0 acceptance criterion,
reopens no accepted H0 guarantee, authorizes no Unity scene, asset import or runtime contract, and
does not modify the `WP-HK-GATE` precondition. It binds `CITY-01..04` and nothing else.

Evidence: `Docs/evidence/WP-CITY-00/`.
Connectivity owner: `Docs/evidence/WP-CITY-00/CONNECTIVITY_MATRIX.md`.
Planar-realizability proof: `Docs/evidence/WP-CITY-00/PLANAR_EMBEDDING.md`.

---

## 1. The city in one paragraph

A fictional Potes/Liébana valley city built around a **wedge between two waters**: a valley river
coming from the NE and an incised tributary from the N meet at the rocky southern tip of the old
quarter. The historic/civic/commercial core occupies that wedge and climbs it in three readable
levels; the Ensanche sits across the tributary; the working river edge follows the main river; and a
small Puerto Fluvial sits **downstream on the opposite, Orilla-sur bank of the joined river**, beside
the valley road. The wedge ends at the confluence. Reaching the port directly from the casco therefore
requires a named river crossing — La barca in State 1, Puente del Muelle in State 2 — rather than an
imaginary dry continuation through the confluence.

The city is roughly a quarter-hour end to end, dense on purpose, and organized by two different water
roles: **the Arroyo shapes daily movement; the Río shapes territorial movement.**

---

## 2. Selected constitution — "Cuña de Confluencia"

### 2.1 Geography and landmasses

| Element | Character |
|---|---|
| **Río** | main valley river from NE, ~18–25 m wide above confluence; joined reach widens/slows below it |
| **Arroyo** | tributary from N, ~4–10 m wide, incised; old-quarter houses can overhang it |
| **Wedge** | land between Río and Arroyo; Casco/Plaza/Calle Mayor/Ribera/Barrio Alto; **ends at confluence** |
| **Ensanche bank** | land outside Arroyo, west of the Wedge |
| **Orilla sur** | land outside Río above confluence and continuous with south/east bank of joined river below it |
| **Puerto / Entrada bank choice** | downstream on Orilla sur; dry-connected to valley road and camino sur, **not** dry-connected to Wedge |
| **Laderas** | scenic green slopes / rock masses beyond roofline |

This landmass statement is binding. If a later drawing shows the Wedge continuing dry past the
confluence, the drawing is wrong. The Puerto district begins at an upstream bridgehead opposite the
Wedge-tip landing head and extends downstream; X6/X7 crosses the channel at that bridgehead rather
than spanning the district's longitudinal extent.

### 2.2 Selected semantic district graph

Topological schematic, not a metric map:

```text
                          VEGA / HUERTAS (NE)             laderas / peñas [scenic]
                                |                                  |
        BARRIO ALTO -------- callejas altas -----------------------+
           |   \                 |                       \
       escaleras   lavadero      |                        mirador
           |           \         |                          |
   ENSANCHE == X2-X5 == CALLE MAYOR (terrace) ===== PLAZA / AYUNTAMIENTO
      |                      |           \                    |       \
      |                 RIBERA / TALLERES \              CASCO VIEJO  X1 PUENTE VIEJO
      |                      |              \                  *----------- ORILLA SUR
      |               PASEO / SIRGA --------\------ landing head            |
      |                                                 |                    | camino sur
      |                                           X6 barca / X7 bridge       |
      |                                                 |                    |
      |                                      ~~~~~ joined RÍO ~~~~~~~~~~~~~~~|
      |                                                 |                    |
      +-------------------------------------------  PUERTO FLUVIAL -----------+
                                                        |
                                                ENTRADA / BUS / CARRETERA
                                                        |
                                                   valley road
```

`*` is the confluence-tip Wedge vertex. Dry Wedge routes stop there. X6/X7 crosses from the landing
head to the upstream Puerto edge. X1 reaches Orilla sur separately at the historic bridge.

All place names are placeholders; identifiers are not frozen.

### 2.3 District families

| Family | Identity | Why it exists | Intensity |
|---|---|---|---|
| **Casco Viejo** | rocky Wedge tip, irregular lanes, houses over Arroyo | strongest identity, intimate social encounter | high |
| **Plaza y Ayuntamiento** | first terrace behind casco | civic core, market, town hall, visible municipal decisions | high |
| **Calle Mayor** | terrace spine NE | everyday retail/services/opening-hours traffic | high |
| **Barrio Alto** | stepped residential slope | schedules, vertical navigation, vantage | medium |
| **Ensanche** | flatter regular residential across Arroyo | second residential character; cheapest large expansion | medium |
| **Ribera y Talleres** | work edge between terrace and Río | workshops, storage, alternate low corridor | medium |
| **Puerto Fluvial** | downstream Orilla-sur yards, quay, ferry/bridge landing, sheds, weighbridge | seasonal river work, road break-bulk, crossing, concessions | medium |
| **Entrada y Carretera** | road junction / bus beside Puerto | arrivals, departures, outsider/freight traffic | low edge family |
| **La Vega** | huertas/paths upstream | rural edge, quiet, seasonal work, market supply | low edge family |

Seven substantial families plus two edge families.

### 2.4 Crossing strategy

Connectivity semantics are owned by `CONNECTIVITY_MATRIX.md`; this section mirrors the named design
edges only.

| ID | Crossing | Water | Connects | Availability |
|---|---|---|---|---|
| **X1** | Puente Viejo | Río | Wedge/casco ↔ Orilla sur | permanent; pedestrians + handcarts, not carts |
| **X2** | Puente del Mercado | Arroyo | Calle Mayor ↔ Ensanche | permanent; road-capable |
| **X3** | Pasarela del Lavadero | Arroyo | Barrio Alto ↔ upper Ensanche | permanent; foot only |
| **X4** | Puente de la Vega | Arroyo | north huertas ↔ north Ensanche | seasonal; flood-closable |
| **X5** | Pasos / vado | Arroyo | lower casco lanes ↔ Ensanche | low water only |
| **X6** | La barca | joined Río | confluence-tip landing head ↔ upstream Puerto edge on Orilla sur | **State 1 only**; hours/fare; high water suspends |
| **X7** | Puente del Muelle | joined Río | confluence-tip landing head ↔ upstream Puerto edge / port road | **State 2 only**; permanent, carts |

Seven crossing IDs exist across the constitution's life; six coexist in each state because X6 and X7
are mutually exclusive. Four are Arroyo crossings. Two Río crossings are designed in either state:
X1 + X6, then X1 + X7.

The Arroyo is crossed casually/often. The Río is crossed rarely/deliberately.

#### 2.4.1 Two connectivity states

- **State 1 — barca era:** X1 + X6 serve Orilla sur.
- **State 2 — Puente del Muelle era:** X7 replaces X6 one-for-one; X1 remains.

Availability is separate from design:

| Situation | Río crossings available | Consequence |
|---|---|---|
| State 1 normal | **2** — X1 + X6 | short historic crossing + landing ferry |
| State 1 high water | **1** — X1 | Puerto reached by X1 + camino sur |
| State 1 X1 closed | **1** — X6 | south bank reached through landing ferry |
| State 1 flood takes both | **0** | Wedge and Orilla sur disconnected |
| State 2 normal | **2** — X1 + X7 | both unconditional |
| State 2 X1 closed | **1** — X7 | movement detours to port bridge |
| State 2 X7 closed | **1** — X1 | people/handcarts cross; cart freight waits |

The already-reviewed 2 → 1 → 0 State-1 ladder is preserved. What changed in v1.1 is the physically
correct endpoint of X6/X7.

### 2.5 Working landing — why it exists and where it sits

The joined reach below the confluence is wider/slower and braids over gravel bars. That is used as a
**workable** river edge, not as a claim of long-distance navigability.

The landing sits on the Orilla-sur downstream bank because four ordinary uses coincide there with flat
ground and the valley road. Its upstream edge is the X6/X7 bridgehead; the working frontage then
extends downstream from that point.

1. **Madera:** timber arrives by road/cart, is assembled into rafts and leaves only during high-water
   windows; destination beyond the city is outside this document.
2. **Áridos:** gravel/sand/lime are worked from bars under concession and moved by cart.
3. **Crossing:** X6 ferry, then X7 bridge, ties the port directly to the core.
4. **Road break-bulk:** weighbridge, yards, sheds and fielato serve the valley road; they do not depend
   on riverborne long-distance freight.

Plus fishing and ordinary waterfront routine.

Scale is **consumed from** `Docs/art/SETTING.md` and `Docs/art/VISUAL_BIBLE.md`, not imposed on ART:

- working frontage roughly 120–150 m;
- working craft only: rafts, flat-bottomed boats, ferry;
- 3–6 sheds/warehouses, small repair shed, hand crane/derrick, weighbridge, timber/drying yard,
  municipal office and a small carter/raft-crew social corner;
- same inland stone/dark-tile material family as town;
- never the city's hero harbour identity.

Nothing here depends on a long-distance navigable trade corridor.

### 2.6 Port approaches and route structure

The port is not a dry extension of the Wedge. Its route families are:

1. **Entrada / valley road → Puerto:** dry on Orilla sur.
2. **X1 far end → camino sur → Puerto:** dry after the historic crossing.
3. **Casco / Cuesta → X6/X7 → upstream Puerto edge:** direct core route.
4. **Ribera / paseo → landing head → X6/X7 → upstream Puerto edge:** low route to the same named crossing.

The last two share X6/X7 and are not double-counted as crossings.

The workpack's anti-hub test is design-level: **remove the plaza and the designed city remains
connected.** Representative route families from `CONNECTIVITY_MATRIX.md` §6:

| Pair | Route using plaza | Designed plaza-free route | Availability note |
|---|---|---|---|
| Barrio Alto ↔ Ribera | stairs → plaza → Calle Mayor → down | callejas altas → east stairs → Ribera | unconditional |
| Casco ↔ Ensanche | plaza → X2 | lower lanes → X5 | **X5 is low-water only; in high water this pair routes through the plaza** |
| Ensanche ↔ Puerto | X2 → terrace/plaza → lower casco → X6/X7 (or X1) | X3 → Barrio Alto → high lanes → east stairs → Ribera → paseo → X6/X7; high-water fallback continues via lower casco → X1 → camino sur | exact cost belongs to CITY-01 |
| Vega ↔ Puerto | Calle Mayor/plaza → X1 → camino sur | paseo/Ribera → landing head → X6/X7; high-water fallback follows paseo/lower casco → X1 → camino sur | exact cost belongs to CITY-01 |
| Barrio Alto ↔ Ensanche | plaza → X2 | X3 | unconditional |

This constitution does **not** promise two plaza-free routes per pair or permanent availability of a
bypass. The named Casco ↔ Ensanche high-water state is deliberately plaza-dependent because X5 is its
direct plaza-free crossing and is then submerged.

### 2.7 Three longitudinal route families

| Level | Route | Design obligation |
|---|---|---|
| low | **Paseo / sirga** | continuous to confluence-tip landing head; Puerto extension is through X6/X7 |
| middle | **Calle Mayor** | continuous commercial terrace |
| high | **Callejas altas** | continuous stepped residential route |

The low route's State-1 port extension is weather-conditional because X6 can suspend. That is an
availability fact, not a design breach.

### 2.8 "Elsewhere in town"

The adopted scale keeps the port genuinely elsewhere: planning hypotheses remain ~8–9 minutes from
Puerto quay to a Calle Mayor shop and ~15–17 minutes from upper Vega to Puerto. These are targets for
CITY-01/CITY-04, not measurements.

A port worker can therefore spend a shift outside the player's cheaply verifiable radius without any
simulation trick.

### 2.9 Retained seed

The retained seed remains on the Wedge tip: casco lanes, plaza edge, Puente Viejo head, bar, one
Arroyo crossing, short river edge, and the first metres of the Cuesta to the **landing-side X6/X7
head**. The Puerto can be visible across/downstream without being built in the seed.

Band remains **0.03–0.06 km²**; exact boundary belongs to CITY-03.

---

## 3. Scale envelope

Derived in `Docs/evidence/WP-CITY-00/SCALE_ENVELOPE.md`.

| Quantity | Starting hypothesis | Adopted |
|---|---|---|
| dense urban playable fabric | 0.8–1.2 km² | **0.30–0.45 km²** |
| total playable envelope | 1.5–2.5 km² | **0.9–1.4 km²** |
| representative long walk | 12–18 min | **13–17 min** |
| substantial district families | 6–8 | **7** (+2 edge families) |
| retained seed | 0.10–0.15 km² | **0.03–0.06 km²** |
| scenic envelope | unstated | unbounded silhouette, never counted as playable |

Dimensional sketch:

| Part | Approx. extent | Approx. area |
|---|---|---|
| Wedge core | 600 m × 150→350 m | ≈0.15 km² |
| Ensanche | 350 × 250 m | ≈0.09 km² |
| Barrio Alto | 300 × 200 m | ≈0.06 km² |
| Puerto | 300 × 150 m | ≈0.045 km² |
| Entrada / road edge | — | ≈0.02 km² |
| **dense fabric** | — | **≈0.365 km²** |

At the planning coverage ratios in `SCALE_ENVELOPE.md`, this is roughly 830 buildings; ratios and
footprints are assumptions, not measurements.

### Walk-time hypotheses

Assumed effective pedestrian speed: **1.15 m/s**, to be measured later.

| Route | Path length | Hypothesised time |
|---|---:|---:|
| Plaza ↔ casco bar | ~120 m | 1.5–2 min |
| Ensanche home ↔ Plaza | ~300 m | 4–5 min |
| Barrio Alto ↔ Ribera workshop | ~400 m + descent | 6–7 min |
| Vega ↔ Plaza | ~450 m | 6–7 min |
| Puente Viejo far end ↔ Entrada/bus | ~500 m | 7–8 min |
| Puerto quay ↔ Calle Mayor shop | ~550 m | 8–9 min |
| Casco tip ↔ NE Calle Mayor | ~600 m | 8–10 min |
| **L1 State 1:** casco → X1 → camino sur → Puerto → X6 → landing head → Cuesta → casco | ~650–750 m + wait | 10–12 min + wait |
| **L1′ State 2:** same via X7 | ~650–750 m | 10–12 min |
| **Vega NE ↔ Puerto quay** | **~1.05–1.15 km** | **15–17 min** |

Shrink/expand conditions remain those in `SCALE_ENVELOPE.md`; moving the port to the correct bank does
not change the adopted area band.

Special case remains: if dense fabric rises above roughly **0.7 km²**, Topology A's rejection must be
reopened rather than stretching this constitution.

---

## 4. Playable fabric versus scenic envelope

| Class | Content | Promise |
|---|---|---|
| **Fabric** | casco, plaza, Calle Mayor, Barrio Alto, Ensanche, Ribera, Puerto | traversable/populated/systemically programmed |
| **Transitional** | Wedge paseo, Arroyo ravine, camino sur, Vega paths, quay/road approaches, slope terraces | traversable, deliberately low-intensity |
| **Scenic** | green slopes, rock masses, distant caseríos, upper valley, river beyond port bend | visible only; never counted as playable |

Orilla sur is a **landmass**, not a content class: it contains fabric at Puerto/Entrada and
transitional/rural content along camino sur.

---

## 5. Permanent spatial invariants

These bind CITY-01..04. Connectivity invariants bind **design** unless stated otherwise; availability
belongs to `CONNECTIVITY_MATRIX.md`.

- **CSI-01 — Two waters, two roles.** Arroyo = everyday crossing; Río = territorial crossing. Neither
  may become scenery-only.
- **CSI-02 — Three longitudinal route families.** Low paseo/sirga, middle Calle Mayor and high
  callejas remain designed end-to-end through the Wedge. The low route reaches Puerto through X6/X7;
  State-1 high-water interruption at X6 is availability, not a design breach.
- **CSI-03 — Plaza converges, it does not structurally connect.** By design, representative adjacent
  pairs have a plaza-free route in `CONNECTIVITY_MATRIX.md` §6. Availability may temporarily remove
  one: **Casco ↔ Ensanche routes through the plaza in high water because X5 is submerged.** What this
  invariant forbids is making the plaza the only designed connector in the base graph.
- **CSI-04 — Port sits on ordinary routes.** Entrada/bus/road junction remains adjacent to Puerto and
  at least two everyday non-port services live there.
- **CSI-05 — Landing keeps valley scale and needs no unauthorized premise.** Working craft only,
  ~120–150 m working frontage, no marina/sea-going read, no dependency on long-distance navigability.
- **CSI-06 — Two designed Río crossings per state.** X1 Puente Viejo is the one historic Río bridge
  inside the casco fabric. Landing crossing is X6 in State 1 and X7 in State 2; no third Río crossing
  exists in either state. Availability can degrade to 1 or 0 as matrix §4 records.
- **CSI-07 — Arroyo crossings are plural/differentiated.** Four designed: X2–X5; X2/X3 permanent.
  Availability belongs to matrix §5.
- **CSI-08 — Quiet is protected fabric.** Upstream paseo, Arroyo ravine/lavadero and upper Vega lanes
  stay low-intensity.
- **CSI-09 — Five spatial characters remain distinct.** Old quarter; civic/commercial; two residential
  characters; work/port edge; rural edge.
- **CSI-10 — Expansion without demolition.** Six seams exist and none requires moving Plaza, Puente
  Viejo, Calle Mayor or quay.
- **CSI-11 — Backdrop is not fabric.** Scenic envelope never counts toward promised playable area.
- **CSI-12 — Municipal levers have physical addresses.** Quay, crossings, market terrace, town hall,
  depot, fonda and Cuesta/landing head make access/schedule/capacity/cost/sponsorship/enforcement/
  commitment consequences walkable.

### Expansion seams

1. **NE along Río / Vega** → more Calle Mayor / huerta quarter.
2. **N up Arroyo** → upper huertas, mill, hamlet.
3. **Up-slope N/NE** → Barrio Alto growth, terraces, miradores.
4. **W across Arroyo** → Ensanche growth.
5. **Downstream on Orilla sur from Puerto / Entrada** → yards, depot, boatyard, road-edge growth.
6. **X6 → X7 landing-crossing transition** → State 2; may later support more south-bank fabric, but
   CITY-00 does not schedule that expansion.

Seam 5 is no longer described as a dry Wedge→Puerto expansion.

---

## 6. Rejected alternatives

Full evidence: `REFUTATION_LOG.md`, `COMPARISON_MATRIX.md`, topology dossiers.

### A — Dos Orillas

Rejected at the adopted scale because splitting ~0.40 km² into two half-towns dilutes reactive density
and makes a representative cheap retained seed difficult. Its raw barrier logic survives in CSI-06.
If dense fabric later rises above ~0.7 km², reopen A.

### C — Ribera Larga

Rejected because the river shapes land use more than ordinary movement, and its single axis weakens
follow/search branching and granular governance. Its cost discipline survives in the scale envelope.

### Why B survives cycle 4

The fourth review did not show that confluence-wedge structure is impossible; it showed that the
**old bank assignment for the port was impossible**. After assigning Puerto/Entrada to the real
Orilla-sur downstream bank, every inter-landmass edge is named, both Río crossings remain meaningful,
and the landing is more clearly territorial rather than decorative.

B remains selected because its charges are answered structurally: route continuity, ordinary port
routes, explicit bank/crossing logic and a retained seed that grows without demolition.

---

## 7. Reconciliation with Production Blueprint

`PRODUCTION_BLUEPRINT.md` v0.3 remains non-binding and points to this constitution for topology.
Its existing statements remain valid after the bank correction:

- `zone.puerto` stays downstream of confluence;
- `zone.entrada` stays adjacent to Puerto;
- the seed keeps a visible descent/seam toward the port, now understood as descent to the landing-side
  crossing head rather than a dry road through the confluence;
- actor examples and the 0.03–0.06 km² seed band remain valid.

No blueprint amendment is required for the bank correction because the blueprint never asserted the
invalid dry Wedge→Puerto edge.

---

## 8. Unresolved questions, with owners

| # | Question | Owner |
|---|---|---|
| Q1 | Visual/material language for timber-and-gravel landing, yards and ferry inside art allowlist; no answer may invalidate topology | ART direction |
| Q2 | Total level change / readability / followability | CITY-04 measured; CITY-01 bounds |
| Q3 | Measured walk times and effective pedestrian speed | CITY-04 |
| Q4 | Primary/secondary route graph, profiles and chokepoint ledger | CITY-01 |
| Q5 | Which Arroyo crossing is the practical municipal closure lever and detour cost | CITY-01 |
| Q6 | Tier A/B location programme and whether ~0.36 km² houses it without crowding quiet fabric | CITY-02 |
| Q7 | Two everyday non-port services at Entrada/Puerto | CITY-02 |
| Q8 | Physical homes for scarce ordinary activities | CITY-02 |
| Q9 | Exact retained-seed boundary inside 0.03–0.06 km² | CITY-03 |
| Q10 | Whether seed includes X5 pasos or X2 Puente del Mercado as its Arroyo crossing | CITY-03 |
| Q11 | Hero mesh/atlas budget compatibility with fabric | art review + CITY-04 |
| Q12 | L1/L1′ traffic character: ordinary route or rural detour, and how State 1/2 changes usage | CITY-01 |
| Q13 | When State 1 transitions to State 2 | unassigned until a later production WP owns it |

---

## 9. Deliberate non-goals

- no Unity scene, navmesh, asset import or runtime contract;
- no change to ROADMAP, HK workpacks, H0 guarantees or HK-GATE;
- no frozen identifiers/final geometry/building placement;
- no CITY-02 programme, CITY-03 seed boundary or CITY-01 measured mobility graph;
- no pre-acceptance of Living World PA findings;
- no 1:1 Potes map;
- no claim that scale/walk-time hypotheses are measured facts.