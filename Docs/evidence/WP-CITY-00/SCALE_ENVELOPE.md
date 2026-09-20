# WP-CITY-00 — Scale envelope and the falsification of the starting hypotheses

WP: WP-CITY-00
`Docs/workpacks/CITY/WP-CITY-00.md`: *"The Worker MUST treat these as falsifiable planning
hypotheses, not numbers to rationalise after the fact."*

This document tests them. The result is that the starting set is not merely debatable — **it is
internally inconsistent**, and two of its four numbers have to move.

## 1. Walking-speed assumption (declared, not measured)

All arithmetic below uses an assumed **effective** pedestrian speed of **1.15 m/s**.

| Component | Assumption | Basis |
|---|---|---|
| level ground, unobstructed | ~1.35 m/s | ordinary adult walking |
| old-quarter lanes, uneven paving, corners | ~1.1 m/s | irregular surface and sightline turns |
| stepped lanes / escaleras | ~0.6–0.8 m/s horizontal equivalent | stair ascent dominates |
| stopping, waiting, giving way | small continuous penalty | market, doorways, narrow passing |

**1.15 m/s is a planning hypothesis for CITY-01 to use and CITY-04 to measure. It is not a measured
fact, and nothing in this workpack may be cited as if it were.** If CITY-04 measures materially
differently, the areas below move with it; the method stays.

## 2. The four starting hypotheses, tested

### H1 — "a representative cross-city walk may feel right around 12–18 minutes"

12–18 min at 1.15 m/s = **830–1240 m of path**. A real walking route in irregular fabric is roughly
1.25–1.4× the straight-line distance, so that is a straight-line extent of **600–900 m**.

A compact town of straight-line extent 700 m across, with a depth-to-length ratio of about 0.5,
encloses roughly 700 × 350 m ≈ **0.25 km²**; a generous reading with a longer axis and deeper fabric
reaches **≈0.45 km²**.

**H1 is retained.** It is the hypothesis that survives, and it is the one that constrains the others.

### H2 — "final dense urban fabric may land around 0.8–1.2 km²"

Run the same arithmetic in reverse. A dense fabric of 1.0 km² in any reasonably compact shape has a
longest internal straight line of roughly 1.2–1.4 km. At a route factor of 1.3 that is **1.6–1.8 km
of path**, i.e. **23–27 minutes** at 1.15 m/s — and more with slope.

**H2 is falsified against H1.** 0.8–1.2 km² of dense fabric and a 12–18 minute cross-city walk
cannot both be true. One of them had to go, and the travel-time hypothesis is the one tied to felt
experience, while the area hypothesis is tied to nothing but ambition.

Two independent checks agree with dropping the area.

#### Content cost — how many buildings an area actually implies

Building count, not mesh count, is the cost that scales with area. The mesh cap in
`Docs/art/VISUAL_BIBLE.md` §8 counts **distinct meshes**, and a modular kit is precisely the
mechanism for instancing many buildings from few meshes — so the cap is not the binding constraint
here. What scales is the number of authored **building compositions, parcels and frontage
decisions**: `PRODUCTION_BLUEPRINT.md` §1.6 requires every parcel to carry its own constraints, and
§3.1's composition ladder has to be instanced and varied once per building.

Estimated from the selected constitution's own district breakdown, using declared coverage ratios
and average footprints:

| District | Area | Building coverage | Avg. footprint | ≈ buildings |
|---|---:|---:|---:|---:|
| Casco Viejo | 0.035 km² | 40% | 90 m² | ~155 |
| Plaza / civic | 0.020 km² | 35% | 120 m² | ~58 |
| Calle Mayor | 0.050 km² | 40% | 110 m² | ~180 |
| Ribera / Talleres | 0.045 km² | 20% | 180 m² | ~50 |
| Ensanche | 0.090 km² | 25% | 120 m² | ~190 |
| Barrio Alto | 0.060 km² | 25% | 90 m² | ~165 |
| Puerto | 0.045 km² | 12% | 250 m² (sheds) | ~22 |
| Entrada | 0.020 km² | 10% | 200 m² | ~10 |
| **Total at ≈0.365 km²** | | | | **≈830** |

Across the adopted 0.30–0.45 km² band that is roughly **700–1,050 buildings**. Applying the same
ratios to the 0.8–1.2 km² hypothesis gives roughly **1,800–2,800 buildings**.

The coverage ratios and footprints above are planning assumptions stated so they can be argued with,
not measured values.

#### Reactive density — the ratio that actually decides it

Track invariant 4 is *"empty expansion is worse than a smaller city with meaningful locations"*, and
the WP's negative gate rejects chasing acreage without a reactive-density argument. The concrete form
of that argument is a ratio: `WP-CITY-02` will programme some number of Tier A/B systemic locations,
and every other building is Tier C ambient frontage.

If CITY-02 lands on the order of 40–60 Tier A/B locations — a plausible number for a cast of about a
dozen persistent actors plus an ambient tier — then:

- at ≈830 buildings, roughly **5–7%** of the fabric participates in life. That reads as a town where
  meaningful places are common enough to stumble into.
- at ≈2,300 buildings, roughly **2%** does. The same content, spread over three times the walking,
  reads as a city that is 98% façade.

Reaching 5% at 1.0 km² would instead require CITY-02 to programme on the order of 115–140 systemic
locations, which is a different and much larger product than this project has planned.

That is the whole argument for the smaller city, and it is a ratio argument rather than a taste one.

**Replacement: dense urban playable fabric ≈ 0.30–0.45 km².**

### H3 — "total playable envelope with river/roads/rural edge may land around 1.5–2.5 km²"

The envelope is a multiple of the fabric, not an independent number. River corridor, towpath, vega,
ravine walk, camino sur, approach roads and slope paths are cheap per square metre but they are still
promised as traversable. A multiplier of 2.5–3.5× on the corrected fabric gives **0.9–1.4 km²**.

**H3 is revised proportionally, not falsified.** It was consistent with H2; it inherits H2's error.

**Replacement: total playable envelope ≈ 0.9–1.4 km²**, plus an explicitly non-traversable scenic
envelope of unbounded apparent extent (slopes, peñas, distant caseríos) that may never be counted
toward promised playable districts.

### H4 — "the first retained seed may be around 0.10–0.15 km²"

Against a corrected core of ≈0.36 km², a 0.10–0.15 km² seed is **28–42% of the whole city** as a
first buildable piece. It is also **3–5× larger** than the seed the project has already described:
`PRODUCTION_BLUEPRINT.md` §1.4 lists one plaza side, one bridge, 60–100 m of street connection, a bar
exterior and interior, one ascending lane, 6–8 frontages and a river strip. That list, laid out at
the blueprint's own scale anchors (§1.5: streets 4–6 m, plaza ~25×20 m, frontages 6–12 m), occupies
roughly 200 × 180 m ≈ **0.035 km²**.

**H4 is falsified twice over** — against the corrected core, and against the project's own existing
seed description, which it contradicts by a factor of three to five.

**Replacement: first retained seed ≈ 0.03–0.06 km²**, with the exact boundary owned by `WP-CITY-03`.

### H5 — "6–8 district families may be enough"

The selected constitution has **nine** named families, but they are not equal: seven are substantial
(casco, plaza/civic, Calle Mayor, Barrio Alto, Ensanche, Ribera/Talleres, Puerto) and two are
low-area, low-content edge families (Entrada/Carretera, Vega).

**H5 is retained with a clarification**: 6–8 *substantial* families is right; edge families are not
free but they are not district-scale content either.

## 3. Adopted scale envelope

| Quantity | Starting hypothesis | Adopted | Status |
|---|---|---|---|
| dense urban playable fabric | 0.8–1.2 km² | **0.30–0.45 km²** | falsified and replaced |
| total playable envelope | 1.5–2.5 km² | **0.9–1.4 km²** | revised proportionally |
| representative long cross-city walk | 12–18 min | **13–17 min** | retained; it is the binding constraint |
| substantial district families | 6–8 | **7** (+2 edge families) | retained with clarification |
| first retained seed | 0.10–0.15 km² | **0.03–0.06 km²** | falsified and replaced |
| scenic, non-traversable envelope | not stated | unbounded silhouette, **never counted as playable** | added |

## 4. Walk-time hypothesis table for the selected constitution

Route lengths are measured off the selected semantic graph's dimensional sketch; times are
hypotheses at 1.15 m/s including a slope penalty where the route climbs or descends.

| Route | Path length | Hypothesised time | Purpose it serves |
|---|---:|---:|---|
| Plaza ↔ bar in the casco | ~120 m | 1.5–2 min | blueprint's 30–90 s observability band inside the seed |
| Ensanche home ↔ Plaza (via Puente del Mercado) | ~300 m | 4–5 min | ordinary cross-arroyo commute |
| Barrio Alto home ↔ Ribera workshop | ~400 m + descent | 6–7 min | vertical daily commute |
| Vega ↔ Plaza/market | ~450 m | 6–7 min | rural edge → civic core |
| Puente Viejo ↔ Entrada/bus | ~500 m | 7–8 min | arrival/departure trip |
| Puerto quay ↔ Calle Mayor shop | ~550 m | 8–9 min | port participates in ordinary commerce |
| Casco tip ↔ NE end of Calle Mayor | ~600 m | 8–10 min | the commercial spine end to end |
| **Vega NE ↔ Puerto quay (longest ordinary route)** | **~1.05–1.15 km** | **15–17 min** | the representative cross-city walk |

These are **targets for CITY-01 to reason with and CITY-04 to measure**, never measured results.

## 5. Explicit conditions that would shrink or expand this envelope

The envelope is not permanent. It changes when one of these fires — and only then.

**Would shrink it:**

1. CITY-02 cannot programme enough Tier A/B systemic locations to make ≈0.36 km² feel inhabited
   without inventing content the project does not want.
2. CITY-04 measures effective walking speed materially below 1.15 m/s, so the same area costs more
   minutes than the travel hypothesis allows.
3. Authored frontage cost proves higher than the kit assumes, so the fabric can only be filled by
   repetition that reads as procedural.
4. The hero mesh/atlas budget survives an art review unchanged and the fabric cannot reach its
   intended silhouette inside it (`PRODUCTION_BLUEPRINT.md` §8.3 offers exactly this choice: shrink
   the visible extent, or revise the budget with evidence).

**Would expand it:**

5. CITY-04 measures the selected constitution as cramped — districts read as adjacent rather than
   distinct, or actors cannot plausibly be "elsewhere in town".
6. CITY-02 produces materially more systemic locations than ≈0.36 km² can house without crowding the
   quiet fabric that invariant CSI-08 protects.
7. A later accepted decision raises the dense band above roughly **0.7 km²**. This one is special:
   at that point the refutation of Topology A no longer holds (`REFUTATION_LOG.md`, verdict A), and
   the correct response is to **reopen the constitution**, not to stretch the selected one.

**Would not change it:** a wish for a bigger city, a comparison with another game's acreage, or a
desire to use more of an asset kit. The WP's acceptance criterion is explicit that size is justified
through density, travel and content cost, not through comparison.
