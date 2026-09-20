# WP-CITY-00 — Planar embedding proof

WP: `WP-CITY-00`
Repair cycle: 4 (transfer Worker)
Purpose: causal repair for independent review #5261556774 on `d2d5f7a7277d8949264d62e6ac2ecbbb171aae9b`.

This file answers one question before any connectivity prose is regenerated: **can the selected
confluence-wedge topology be embedded in the plane without an uncounted water crossing?**

The previous candidate could not. It treated a wedge between two channels that meet at their point as
if the wedge continued downstream past the confluence. It does not. This repair fixes the landmasses
first and derives every route from them.

## 1. Landmasses

Use the confluence as the reference vertex.

1. **Wedge.** Land between the Río approaching from the NE and the Arroyo approaching from the N.
   Casco Viejo occupies the rocky southern tip. The Wedge **ends at the confluence**.
2. **Ensanche bank.** Land west/outside the Arroyo. It is continuous with the Ensanche and upper
   west-bank huertas. It is not a continuation of the Wedge.
3. **Orilla sur.** Land east/outside the Río above the confluence and continuing onto the south/east
   bank of the joined river below it. Puente Viejo reaches this landmass from the Wedge.
4. **Puerto bank choice.** `Puerto Fluvial` and `Entrada / Bus / Carretera` sit on the **Orilla sur
   landmass downstream of the confluence**, beside the joined river. Therefore the port is genuinely
   downstream, genuinely on a bank of the joined river, and dry-connected to the valley road and the
   south-bank camino. It is **not** dry-connected to the Wedge.

The port district begins at a small **upstream bridgehead/landing edge opposite the Wedge-tip head**
and extends downstream from there. X6/X7 therefore spans the river channel at the top of the port; it
does not span the full ~300 m longitudinal extent of the port district.

That bank choice is the minimum structural change that preserves the landing rationale, the historic
bridge, the two-state crossing strategy and the 2 → 1 → 0 availability ladder.

## 2. Schematic

This is a topological schematic, not a metric map.

```text
                    north
                      ^
                      |
        ENSANCHE      |       WEDGE / VEGA
      (west of A)     |      (between A and R)
            \         |          /
             \  A     |         / R
              \       |        /
               \      |       /
                \     |      /
                 \    |     /
                  \   |    /
                   \  |   /
                    \ |  /
                     \| /
                 CONFLUENCE / CASCO TIP
                       *
                      / \
        joined río ->/   \<- joined río banks
                    /     \
   Ensanche-side   /       \   ORILLA SUR
   downstream bank          \  (continuous with the
                               land beyond Puente Viejo)
                                \
                                 \  PUERTO + ENTRADA
                                  \ [downstream bank]
```

`A` = Arroyo. `R` = Río above the confluence.

The important fact is the vertex at `*`: the interior of the Wedge reaches the confluence and stops.
No dry edge is allowed to pass through `*` and reappear downstream.

## 3. Crossings implied by this embedding

Every edge that moves between the three landmasses is explicit:

- **Wedge ↔ Ensanche bank:** X2 Puente del Mercado, X3 Pasarela del Lavadero, X4 Puente de la Vega,
  X5 Pasos/vado. All cross the Arroyo.
- **Wedge ↔ Orilla sur near the casco:** X1 Puente Viejo. It crosses the Río above/at the core edge.
- **Wedge tip ↔ upstream Puerto edge on Orilla sur:** X6 La barca in State 1, replaced by X7 Puente
  del Muelle in State 2. They cross the upper joined-river reach from the confluence-tip landing head
  to the port-bank bridgehead.

There is no hidden eighth crossing and no dry Wedge → Puerto edge.

## 4. Physical approaches after the correction

### To the port

1. **Valley-road approach:** Entrada / Bus / Carretera → Puerto. Dry, entirely on Orilla sur.
2. **South-bank approach:** Puente Viejo far end → camino sur → Puerto. X1 is the only water crossing
   on a trip that starts in the Wedge; after X1 the route stays on Orilla sur.
3. **Core crossing:** Casco / lower Cuesta → X6 (State 1) or X7 (State 2) → upstream Puerto edge. The
   Cuesta ends at the Wedge-side ferry/bridge head; it does **not** pretend to continue as dry land to
   the port.
4. **Ribera / paseo approach:** Ribera → paseo/sirga → casco-tip landing head → X6/X7 → upstream
   Puerto edge. The riverside path reaches the crossing head, not the opposite bank by magic.

The port therefore remains on ordinary routes without requiring three dry approaches.

### Between the core and the south bank

Two designed río crossings still exist in each state:

- X1 at the casco;
- X6 at the landing in State 1, replaced one-for-one by X7 in State 2.

This preserves the already-reviewed 2 → 1 → 0 availability ladder. The repair changes **where the
landing-side crossing lands**, not the number of designed south-bank crossings.

## 5. Loops

The corrected south-bank loop is planar:

- **L1, State 1:** Casco → X1 → camino sur → Puerto → X6 → confluence-tip landing head → Cuesta →
  Casco.
- **L1′, State 2:** the same loop with X7 replacing X6.

Every segment is either dry on one named landmass or one named crossing.

## 6. Expansion implications

The earlier seam 5 was wrong because it described Wedge → Puerto as a dry downstream expansion.
Corrected:

- seam 5 is **port-bank build-out** on Orilla sur, reached from the retained seed through the
  landing-side crossing head and from outside through Entrada;
- seam 6 is the **State 1 → State 2 crossing transition**, X7 replacing X6, and can later support
  further south-bank fabric if CITY-02/03 choose it.

No core demolition is introduced.

## 7. Planarity checks

The corrected embedding passes all five checks:

| Check | Result |
|---|---|
| Wedge terminates at the confluence | PASS |
| Every inter-landmass route names a water crossing | PASS |
| Puerto is downstream on a real bank of the joined river | PASS |
| X6/X7 crosses channel width to upstream port edge rather than spanning port length | PASS |
| Crossing set is complete under this embedding | PASS — seven across the constitution's life; six per state |

This file is evidence for physical realizability. `CONNECTIVITY_MATRIX.md` remains the owner of
connectivity semantics and must agree with this embedding.