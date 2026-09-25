# CITY-07 game-map concept — Puente Viejo / Casco

Status: **CONCEPT / NON-CANONICAL**  
Class: DOCS_ONLY / DESIGN EXPLORATION  
Date: 2026-09-25  
Scope: input for the future `WP-CITY-07` Worker and for owner decisions on amendments. It is not a WP deliverable and does not change any CITY, H1, ART or PA contract.

Interactive map: [`city07_game_map.html`](city07_game_map.html) (open in a browser; pan/zoom, layers, click for details).  
Snapshots: [`city07_map_overview.png`](city07_map_overview.png), [`city07_map_casco.png`](city07_map_casco.png).

## 1. Why this exists

The CITY-04 greybox (PR #221) proved the accepted seed is coherent, but the owner found it read as a flat planning diagram: cubes on a meadow, hard to orient in, and too small/sparse to judge as a game space (`OWNER_GREYBOX_FEEDBACK.md`, deviation D02). `CITY_07_GAME_SPACE_REALIZATION_AMENDMENT.md` already asks CITY-07 to turn the validated seed into an authored third-person game space.

This concept shows what that game space could look like, what each part would need, and which owner must approve it before Unity work.

## 2. Source of truth used

Taken literally from `CITY_PRODUCT_SEED.md` §2–4 (seed-local U/V metres):

- hard outer polygon `B01–B08`, `mask.rio`, `mask.arroyo`, 3 m / 2 m no-build shoulders;
- `x1.crossing`, `x5.crossing` and the four receiving stubs;
- anchors `W.LANDING … W.SHOP`;
- represented edges `W04, W05, W06, W12, W13, X1, X5` and node-local `casco.micro.A/B`;
- frontage slots `F01–F08`, open sites `S01–S03` with their place IDs, A–D / S / I and access roles.

Everything else in the map is presentation or a tagged proposal.

## 3. What was added, and who must approve it

| Layer | Content | Authority |
|---|---|---|
| Ordinary fabric | Closed `I0` houses only where they front a street (one row on lanes, with gaps); only F01 bar, F02 ayuntamiento and F03 shop are enterable | CITY-07 realization freedom; impact-ledger entry; no door inflation (CITY-06 §12) |
| Walled huertas | Stone-walled gardens filling block interiors instead of more houses | CITY-07 presentation; private, not traversable |
| Street furniture | Fountains (plaza, Casco square), lamps, benches, hydrangea pots, laundry lines, barrels at the bar, cart/crates at the shop service side, firewood, well, bolos, moored boats | CITY-07 props; no semantics, no blocking of always-clear routes or bank shoulders |
| **P1** tower at F02 | Vertical civic landmark (Torre del Infantado read, hipped dark-tile roof, no crenellation per VISUAL_BIBLE) | CITY-09 P1 → **CITY-05** amendment (CITY-03 if massing changes) |
| **P4 / P6** | Landing and O.X1 seam payoffs; Cuesta mirador | CITY-09 → **CITY-03** |
| **P5** | Bolera on Orilla sur | CITY-09 → CITY-02 + CITY-03; **not recommended yet** |
| **P8** (new here) | Lanes `P8a` Tintes, `P8b` Horno, `P8c` Calleja Alta, `P8d` Pasadizo del Arco, `P8e` Ensanche bajo; dirt paths `P8f` Huertas, `P8g` Arroyo; four small squares (Horno, Tinte, Era alta, Mirador del Arroyo) | New graph edges and cycles → **CITY-01 + CITY-03** amendment before any realization |
| Discovery points | 11 points, see §5 | Colour-coded by owner in the map |

Measured on the exact seed geometry: without P8, only ~33 % of the Wedge's dry land lies within 15 m of a public route and ~49 % within 25 m. P8 is what turns those interior blocks from "seen but never walked" into playable space. Everything under the fog (Ensanche, Barrio Alto, Calle Mayor, Ribera, Puerto, Vega) is soft visual envelope only.

## 4. Game possibilities

These are spatial affordances. The runtime systems that would use them (routines, NPC agency, knowledge, rumours, consequences, investigation, governance) belong to PA owners; nothing here claims they exist. Stage names follow CITY-09 PD-11.

### 4.1 Orientation and route learning

- **Three-scale landmark hierarchy (PD-05):** L1 the tower (P1), visible from the bridge crest, the ford, the commercial street and the Calleja Alta; L2 the river / Puente Viejo and the plaza; L3 local cues such as the hornacina, fountains, the laundry lines and the rendija.
- **Real loops with P8:** Casco → W05 → Plaza → W04 → Tintes → Horno → W12 → Casco, and W13 → Pasadizo → W05. Players learn the town by choosing, not by out-and-back. Without P8 the only branching is the node-local A/B micro-loop.
- **Seam payoffs:** every edge of the seed ends in a view or cue (confluence, Orilla sur, Calle Mayor continuation, Ensanche across the ford) rather than an invisible wall.

### 4.2 Traversal rhythm

The representative chain (§6) alternates open → compressed → open: bridge span, bridgehead pocket, narrow W12 climb, Casco square, bar frontage. P8 adds a second rhythm: stone lane → dirt path between walls → small square with tree and bench. The longest stretch without a new cue drops sharply once the squares and discovery points exist (CITY-09 PD-09 measurement, to be taken in the demo).

### 4.3 Reusable stage shapes (spatial only)

| Stage type | Where | Why it works |
|---|---|---|
| Home base | Bar F01 + Casco square | Several approaches (W12, W13, W06, A/B), terrace, public/service/semi-private layers |
| Plaza / crowd | Plaza + market S01 | Open floor, arcade edge F06, several exits, always-clear band separate from stalls |
| Follow / search | Casco A/B + Pasadizo + Horno | Branching and reconvergence with occluding corners |
| Chase | Cuesta W06 (grade, landings) → W12 → Tintes loop | Level change, turns, alternate lines of sight |
| Confrontation | Puente Viejo span | Readable linear span, two approaches, observers on both banks |
| Conversation | Bench at Mirador del Arroyo, Rincón del Tinte, landing | Semi-sheltered edges and nooks |
| Observation | Bridge crest, Calleja Alta behind the tower, Mirador del Arroyo | Elevated or protected vantage over routes |
| Practice / play | La Era alta (bolos) | Bounded open ground away from traffic |
| Waiting / state | Ford X5 and Mirador del Arroyo | X5 only exists at low water; the flood marks and the bench make that state legible |

### 4.4 Quiet / busy contrast

Busy: plaza, market, commercial street, bar terrace. Quiet: W13, huertas paths, Era alta, Mirador del Arroyo. The contrast is spatial (width, paving, enclosure, furniture density) so it holds even before NPCs exist.

### 4.5 Discovery

Discovery is spatial truth, not collectibles (CITY-06 §8, §12). The player learns that a building has another layer, that a wall remembers an older use, that the river rises, and later systems can attach meaning to those surfaces.

### 4.6 Future hooks (owner-conditional)

- Low/high water changes X5 and the Mirador del Arroyo reading (crossing availability already owned by CITY-01/03).
- Work/material state at F03 service pocket and market S01 (PA-07).
- Institutional access at the Ayuntamiento records boundary (PA-12).
- Barca X6 at the landing when State 1 is realized (CITY-00/01 already name it).

## 5. Discovery points

| ID | Name | Status | What the player perceives |
|---|---|---|---|
| `disc.bar.secondary_layer` | Patio de atrás del bar | Accepted (CITY-06) | Through the service-alley gap: back patio and stair; the public room is not the whole building |
| `disc.ayuntamiento.records_boundary` | Tablón del Ayuntamiento | Accepted (CITY-06) | Public notices and a grilled door that signals a private archive |
| `sec.court_stair` | Escalera del patio S03 | Accepted relation (CITY-03 S03) | Exterior stair of the shared court seen from W13; reads semi-private |
| `sec.riada` | Marcas de riada | CITY-07 surface | Carved flood heights on the bridge abutment; explains why X5 is low-water only |
| `sec.puerta_cegada` | Puerta cegada | CITY-07 surface | Walled-up arch with different stones on W12; older use, lore owned later |
| `sec.hornacina` | Hornacina | CITY-07 composition | Candle niche at the Casco square; L3 cue for the A/B loop |
| `sec.rendija` | Rendija al río | CITY-07 composition | Narrow gap between W12 houses framing the river; a view, not a path |
| `sec.barca` | Barca varada | CITY-07 prop | Old boat at the landing hinting the future X6 barca |
| `sec.pozo` | Pozo de la huerta | Needs P8f | Well and fig tree behind a garden grille; view only, garden stays private |
| `sec.mirador_arroyo` | Mirador del Arroyo | Needs P8g | Bench over the Arroyo facing the ford and the Ensanche |
| `sec.era` | La Era alta | Needs P8c | Threshing floor with chestnut and bolos behind the Ayuntamiento; CITY-02 too if it becomes a programmed place |

No building receives a hidden room and no route is laundered through service/private space.

## 6. Recommended CITY-07 representative chain

`O.X1 → X1 → W12 (S02 bridgehead) → W.CASCO square → casco.micro.B → F01 bar (public / service / semi-private)`

1. Orilla sur: the Casco framed across the river, tower above (if P1).
2. Bridge crest: water below, narrow AH span, Casco ahead.
3. S02 bridgehead: open pause; W12 climb and Cuesta start visible.
4. W12 compression: 3–4 m lane between closed façades (rendija and puerta cegada here).
5. Casco square: A/B choice, hornacina, fountain.
6. Bar frontage: terrace, warm light, separate service door.
7. Inside: public room → semi-private layer (`disc.bar.secondary_layer`).

Rationale: ends in the only hero interior (I3); richest lawful level change, reveal and framing; covers CITY-03 SCN-04/05/06/07; avoids the CITY-08 reserved slice (`W04 + F03 + F07 + S01`) so that later reuse proof stays honest. This chain needs **no** amendment.

## 7. Suggested owner actions

1. **P1 → CITY-05**: accept the civic tower as L1 landmark.
2. **P8 → CITY-01 + CITY-03**: accept the lanes, dirt paths and small squares (or a subset) as represented edges before CITY-07, so the demo is not half unreachable blocks.
3. **P4 / P6 → CITY-03**: seam payoffs and Cuesta mirador.
4. Keep P2 / P3 / P5 closed until the asset-rich demo has been played.
5. Resolve PR #224 (demo-stage human validation) so CITY-07 knows whether it owns the timing campaign.

## 8. Limits

- Planning geometry only; no measured widths, grades, sightlines or times.
- Filler placement is procedural presentation, not a parcel plan.
- The session that drew this concept should not act as independent Reviewer of a CITY-07 candidate built from it.
