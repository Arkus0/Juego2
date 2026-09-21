# WP-CITY-02 — Programme capacity sanity check

Semantic owner: `Docs/production/CITY_LOCATION_PROGRAMME.md`  
Purpose: answer the programme-level part of CITY-00 Q6 without pretending CITY-05 parcel fit or CITY-04 geometry has already happened.

This evidence is a **pressure check, not a metric layout**. It consumes the accepted CITY-00 area sketch and the CITY-02 A/B programme. It does not assign parcel footprints, building dimensions or measured density.

## 1. Accepted area envelope consumed

CITY-00's adopted dimensional sketch gives approximately:

| Part | Accepted approximate area |
|---|---:|
| Wedge core | 0.150 km² |
| Ensanche | 0.090 km² |
| Barrio Alto | 0.060 km² |
| Puerto | 0.045 km² |
| Entrada / road edge | 0.020 km² |
| **dense fabric total** | **0.365 km²** |

Vega/huertas belong to the wider playable envelope rather than that dense-fabric subtotal, so the two Vega A/B places are not charged against 0.365 km² here.

## 2. A/B programme pressure by accepted part

CITY-02's master ledger contains 23 A/B rows. Twenty-one sit in the dense-fabric parts above and two sit in Vega.

| Accepted part | CITY-02 district families included | A/B programme rows | Area | Crude area per A/B handle |
|---|---|---:|---:|---:|
| Wedge core | Casco + Plaza/Ayuntamiento + Calle Mayor + Ribera/Talleres | 10 | 0.150 km² | ~15,000 m² |
| Ensanche | Ensanche | 2 | 0.090 km² | ~45,000 m² |
| Barrio Alto | Barrio Alto | 2 | 0.060 km² | ~30,000 m² |
| Puerto | Puerto Fluvial | 4 | 0.045 km² | ~11,250 m² |
| Entrada | Entrada/Carretera | 3 | 0.020 km² | ~6,670 m² |
| **Dense total** | — | **21** | **0.365 km²** | **~17,380 m²** |
| Vega | La Vega | 2 | outside dense subtotal | not computed as dense-fabric pressure |

The last column is deliberately only a pressure indicator. A programme handle is not a parcel and can be an exterior threshold, square, crossing head, yard, work cluster or shallow service place. It must not be read as a required footprint.

## 3. Why this does not imply crowding

The programme does not ask for 21 hero buildings inside 0.365 km²:

- only **one** location is `I3 HERO` / `S4` — `loc.casco.bar`;
- six are `I2 DEEP`;
- seven are `I1 SHALLOW`;
- the remaining programme rows are `I0`, including many exterior/threshold/quiet places and all ordinary/scenic families;
- CITY-02 explicitly leaves the majority of future parcel/building count available for `C/D` and `S0/S1` fabric rather than turning the accepted ~830-building planning estimate into hundreds of POIs;
- even the most concentrated accepted part in this coarse check, Entrada, carries only three A/B handles across ~20,000 m² and two of those are spatially shallow/exterior-led (`arrival`, `depot_forecourt`).

Therefore the programme produces no obvious area-pressure contradiction with CITY-00. Exact siting and footprint fit remain correctly owned by CITY-05.

## 4. Quiet-fabric pressure check

The A/B count is not distributed by filling every route with destinations:

- upstream `W07` / Vega quiet paseo remains a single low-intensity B destination on a protected quiet route, with no incident-generation obligation;
- `W15` lavadero/ravine remains B/S2 but explicitly ordinary/domestic rather than an event funnel;
- upper Barrio and Ensanche retain ordinary `C/S1` residential families around their A/B anchors;
- Ribera/Puerto retain `C/S1` sheds/work frontage rather than promoting every work building;
- scenic slopes/roofline remain `D/S0` and do not count as reactive destinations.

A later parcel plan that consumes all quiet frontage or converts ordinary families into A/B content would contradict CITY-02 rather than prove this capacity check wrong.

## 5. CITY-00 Q6 verdict

At the **programme** level, Q6 is answerable positively:

> The accepted ~0.365 km² dense-fabric envelope can host the proposed A/B programme without an evident crowding contradiction, because the programme distributes only 21 dense-fabric A/B handles across the five accepted dense parts, keeps depth/interior commitments selective, and explicitly reserves substantial ordinary/quiet fabric.

What is **not** claimed:

- exact parcel fit;
- exact square metres consumed by each location;
- realized frontage density;
- measured traversal/reactive density;
- a guarantee that CITY-05 cannot falsify a particular siting choice.

If CITY-05 later demonstrates that the required shell/access constraints cannot fit while preserving quiet fabric, that is concrete evidence to revisit the programme/siting boundary. CITY-02 does not pre-empt that empirical/design check with fake precision.
