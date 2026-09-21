# WP-CITY-02 — Programme capacity sanity check

Semantic owner: `Docs/production/CITY_LOCATION_PROGRAMME.md`  
Purpose: answer the programme-level part of CITY-00 Q6 without pretending CITY-05 parcel fit or CITY-04 metric geometry has already happened.

This evidence is a **coarse programme-capacity bound, not a parcel plan**. It consumes the accepted CITY-00 area sketch and the CITY-02 A/B programme. It deliberately asks a falsifiable question before CITY-05: under conservative class ceilings for the spatial demand of each programmed A/B place, can the programme fit while preserving a hard ordinary/quiet reserve and design slack?

The answer below is independent of `A/B handle count` and independent of `I0..I3`. Neither count nor interior depth is used as a capacity oracle.

## 1. Accepted area envelope consumed

CITY-00's adopted dimensional sketch gives approximately:

| Part | Accepted approximate area |
|---|---:|
| Wedge core | 0.150 km² / 150,000 m² |
| Ensanche | 0.090 km² / 90,000 m² |
| Barrio Alto | 0.060 km² / 60,000 m² |
| Puerto | 0.045 km² / 45,000 m² |
| Entrada / road edge | 0.020 km² / 20,000 m² |
| **dense fabric total** | **0.365 km² / 365,000 m²** |

Vega/huertas belong to the wider playable envelope rather than that dense-fabric subtotal, so the two Vega A/B places are classified below but are not charged against 0.365 km².

## 2. Coarse spatial-demand classes

A CITY-02 **programme envelope** (`PE`) is a conservative upper planning allowance for the land/space a programmed A/B place may require **exclusively because that place exists**. It bundles, where applicable:

- building/shell footprint needed by the place;
- dedicated yard, court, garden, market occupation or work apron;
- immediate threshold/frontage apron that must remain usable by that place.

It excludes shared public streets/routes, river/water, scenic envelope and ordinary neighbouring fabric. A `PE` is not a parcel boundary, building dimension or placement instruction. CITY-05 may realize the same requirement with one parcel, several shells, shared edges or a smaller actual footprint.

The ceilings are intentionally broad enough to be conservative while still distinguishing a crossing head from a market/work yard:

| Class | Coarse demand range | Upper bound used for capacity | Intended family |
|---|---:|---:|---|
| `T` threshold / linear edge | 0–600 m² | **600 m²** | bridge/arrival/landing/paseo threshold with little exclusive land demand |
| `S` small service/node | 250–1,200 m² | **1,200 m²** | small shop/service room plus immediate frontage/support |
| `M` medium place | 700–2,500 m² | **2,500 m²** | bar/fonda/neighbourhood place, small cluster, lavadero/garden, bounded work/service place |
| `L` large place / work-open-space cluster | 1,800–5,000 m² | **5,000 m²** | market/civic-residential cluster, workshop/yard, warehouse/work hub or supply node |

These are **programme ceilings**. They are not claims that the final city uses these exact numbers. Their purpose is to make overload detectable now: if a downstream requirement cannot truthfully stay within its assigned class ceiling, CITY-02 capacity must be recomputed rather than silently borrowing quiet/ordinary fabric.

## 3. Per-place A/B demand assignment

All 23 A/B programme rows receive one demand class. The upper bound, not the midpoint, is charged.

| Accepted part | Programme ID | Class | Charged upper bound |
|---|---|---:|---:|
| Wedge core | `loc.casco.bar` | M | 2,500 m² |
| Wedge core | `loc.casco.bridgehead` | T | 600 m² |
| Wedge core | `loc.plaza.ayuntamiento` | M | 2,500 m² |
| Wedge core | `loc.plaza.market` | L | 5,000 m² |
| Wedge core | `loc.calle.bakery` | M | 2,500 m² |
| Wedge core | `loc.calle.pharmacy` | S | 1,200 m² |
| Wedge core | `loc.calle.everyday_shop` | S | 1,200 m² |
| Wedge core | `loc.ribera.workshop` | L | 5,000 m² |
| Wedge core | `loc.ribera.service_yard` | L | 5,000 m² |
| Wedge core | `loc.ribera.paseo_edge` | T | 600 m² |
| Ensanche | `loc.ensanche.neighbourhood_anchor` | M | 2,500 m² |
| Ensanche | `loc.ensanche.shared_garden` | M | 2,500 m² |
| Barrio Alto | `loc.barrio.residence_cluster` | L | 5,000 m² |
| Barrio Alto | `loc.barrio.lavadero` | M | 2,500 m² |
| Puerto | `loc.puerto.work_hub` | L | 5,000 m² |
| Puerto | `loc.puerto.landing` | T | 600 m² |
| Puerto | `loc.puerto.worker_social` | M | 2,500 m² |
| Puerto | `loc.puerto.warehouse_yard` | L | 5,000 m² |
| Entrada | `loc.entrada.arrival` | T | 600 m² |
| Entrada | `loc.entrada.fonda` | M | 2,500 m² |
| Entrada | `loc.entrada.depot_forecourt` | M | 2,500 m² |
| Vega | `loc.vega.supply_node` | L | 5,000 m² — outside dense subtotal |
| Vega | `loc.vega.quiet_paseo` | T | 600 m² — outside dense subtotal |

The class follows the programme's spatial role, not its systemic importance or interior depth. For example, `loc.entrada.arrival` is A/S1/I0 but only `T`, while the B/S2/I0 market is `L`. This prevents A–D, S-depth or I-depth from becoming a disguised capacity proxy.

## 4. District-part budgets and hard reserve

CITY-02 now reserves area explicitly before CITY-05 can parcel anything. Each accepted dense part is split into three programme-level buckets:

1. **A/B programme cap** — maximum area that the conservative A/B `PE` upper bounds may claim.
2. **Hard ordinary/quiet reserve** — minimum area that must remain outside A/B-exclusive envelopes for ordinary `C` fabric, private/low-intensity fabric and the district's protected non-spectacle identity.
3. **Uncommitted/circulation margin** — 15% held back for shared public movement, awkward shape, retaining/river edges and future composition. CITY-02 may not spend this margin to make the A/B arithmetic pass.

The reserve percentages are intentionally strongest in residential/quiet districts and lower in the working Puerto, while still protecting ordinary work fabric there.

| Accepted part | Area | A/B cap | Hard ordinary/quiet reserve | Uncommitted/circulation margin | Charged A/B PE upper sum | Result |
|---|---:|---:|---:|---:|---:|---|
| Wedge core | 150,000 m² | 25% = 37,500 m² | 60% = 90,000 m² | 15% = 22,500 m² | **26,100 m² (17.4%)** | PASS; 11,400 m² cap headroom |
| Ensanche | 90,000 m² | 15% = 13,500 m² | 70% = 63,000 m² | 15% = 13,500 m² | **5,000 m² (5.6%)** | PASS; 8,500 m² cap headroom |
| Barrio Alto | 60,000 m² | 20% = 12,000 m² | 65% = 39,000 m² | 15% = 9,000 m² | **7,500 m² (12.5%)** | PASS; 4,500 m² cap headroom |
| Puerto | 45,000 m² | 35% = 15,750 m² | 50% = 22,500 m² | 15% = 6,750 m² | **13,100 m² (29.1%)** | PASS; 2,650 m² cap headroom |
| Entrada | 20,000 m² | 30% = 6,000 m² | 55% = 11,000 m² | 15% = 3,000 m² | **5,600 m² (28.0%)** | PASS; 400 m² cap headroom |
| **Dense total** | **365,000 m²** | **84,750 m² aggregate caps** | **225,500 m² hard reserve** | **54,750 m² margin** | **57,300 m² (15.7%)** | **PASS** |

The citywide aggregate is secondary; **every part must pass individually**. Entrada is intentionally close to its programme cap. That is useful: this test can fail rather than always blessing the programme.

The hard reserve is not a promise that every reserved square metre is a park or quiet route. It is a non-A/B floor protecting ordinary residences/frontages, private thresholds, low-intensity edges, ordinary work sheds and district breathing room. In addition, the named CITY-00/CITY-02 quiet invariants remain non-borrowable:

- upstream `W07` / Vega quiet paseo cannot become compensating A/B development land;
- `W15` ravine/lavadero context remains low-intensity domestic/quiet fabric;
- upper Barrio and Ensanche retain ordinary residential fabric around their A/B anchors;
- Puerto must retain ordinary `C/S1` work sheds/frontage rather than converting the entire work edge into programmed destinations;
- scenic slopes/roofline remain `D/S0` and are never counted as reserve or capacity.

## 5. Explicit fail / reopen rules

Q6 is positive only while all of the following remain true:

1. **Part cap:** the sum of assigned A/B `PE` upper bounds in every accepted dense part is at or below that part's A/B cap.
2. **Reserve floor:** no siting/composition proposal consumes the hard ordinary/quiet reserve to make A/B places fit.
3. **Class truthfulness:** if CITY-05 discovers that a required place cannot plausibly fit within its assigned class ceiling, that place is reclassified and the whole part is recomputed before acceptance.
4. **Programme growth:** adding/promoting an A/B location requires rerunning this table; handle count alone is never sufficient.
5. **No quiet borrowing:** named protected quiet fabric cannot be used as overflow even if the district's raw area arithmetic would otherwise pass.
6. **No shared-route double counting:** shared streets/routes cannot be counted simultaneously as A/B-exclusive `PE` and hard reserve.

Concrete falsifiability witness: Entrada currently charges 5,600 m² against a 6,000 m² A/B cap. If `loc.entrada.depot_forecourt` proves to require `L` rather than `M`, its charge rises from 2,500 to 5,000 m²; Entrada becomes 8,100 m² / 40.5% and **fails CITY-02 Q6** until the programme is reduced/reclassified through review. CITY-05 is not authorized to hide that failure by shrinking the ordinary-edge reserve.

Likewise, Puerto has only 2,650 m² of cap headroom under conservative upper bounds. A materially larger warehouse/work-hub demand can therefore falsify the current programme before parcel detail is treated as accepted.

## 6. What this proves — and what it does not

This bound does prove a CITY-02-level statement that the previous `area / handle` ratio could not prove:

> Using conservative, role-based upper spatial-demand classes for every A/B place, the current dense-fabric programme charges at most 57,300 m² of the accepted 365,000 m² dense envelope; every accepted part remains under its own A/B programme cap while preserving an explicit hard ordinary/quiet reserve and an additional 15% uncommitted/circulation margin.

It does **not** claim:

- exact parcel fit or exact parcel boundaries;
- exact final footprints or frontage dimensions;
- that every upper class allowance will be used;
- realized frontage density or street geometry;
- measured traversal/reactive density;
- that CITY-05 cannot discover a class-ceiling breach or shape/access conflict.

Those are downstream checks. The distinction is causal: CITY-02 now proves coarse programme capacity under declared upper bounds; CITY-05 must prove actual composition without violating those bounds/reserves.

## 7. CITY-00 Q6 verdict

At the **programme** level, Q6 is answerable positively on the current programme:

> The accepted ~0.365 km² dense-fabric envelope can host the proposed A/B programme without consuming protected ordinary/quiet fabric under the declared conservative demand classes and district-part budgets. The claim automatically reopens if a class ceiling is exceeded, a district A/B cap is breached, or the hard reserve/quiet invariants would have to be borrowed.

This replaces the previous false-green `area / A/B handle` argument. The 21 dense-fabric handle count is descriptive only and carries no capacity inference.
