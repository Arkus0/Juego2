# WP-CITY-05 — Exterior grammar capacity fit check

Semantic owner: `Docs/production/CITY_ENVIRONMENT_GRAMMAR.md`  
Inherited capacity owner: `Docs/evidence/WP-CITY-02/CAPACITY_SANITY.md`  
Purpose: test whether the CITY-05 street/parcel/building grammar can realize every accepted CITY-02 A/B place without exceeding its inherited `T/S/M/L` programme-envelope ceiling or consuming the hard ordinary/quiet reserve.

This evidence does **not** redraw the city or establish exact parcel polygons. It constructs one plausible bounded exterior composition per A/B place using CITY-05 families and checks the exclusive-envelope arithmetic. Final realized geometry remains falsifiable downstream.

## 1. Accounting rule

For this check, each A/B place is decomposed into at most three **exclusive** components:

1. `shell/support` — building/support-structure footprint required by that place;
2. `dedicated open` — place-owned yard/court/garden/market occupation/work apron;
3. `threshold apron` — immediate place-specific approach/working threshold outside the shared route polygon.

Excluded from every total:

- shared public street/path route polygon;
- junction clearances that serve the route rather than one place;
- water;
- scenic envelope;
- ordinary neighbouring C fabric;
- the hard ordinary/quiet reserve.

Therefore this check cannot make itself pass by counting the same square metres as both place envelope and circulation/reserve.

Inherited class ceilings are unchanged:

| Class | Accepted upper bound |
|---|---:|
| T | 600 m² |
| S | 1,200 m² |
| M | 2,500 m² |
| L | 5,000 m² |

The figures below are **bounded planning maxima for one legal composition pattern**, not predictions that the final geometry consumes exactly that area.

## 2. Per-place fit

| Part | Place | Class | Shell/support | Dedicated open | Threshold apron | CITY-05 bounded total | Class ceiling | Result |
|---|---|---:|---:|---:|---:|---:|---:|---|
| Wedge | `loc.casco.bar` | M | 500 | 450 | 250 | **1,200** | 2,500 | PASS |
| Wedge | `loc.casco.bridgehead` | T | 0 | 200 | 150 | **350** | 600 | PASS |
| Wedge | `loc.plaza.ayuntamiento` | M | 800 | 350 | 250 | **1,400** | 2,500 | PASS |
| Wedge | `loc.plaza.market` | L | 300 | 2,200 | 500 | **3,000** | 5,000 | PASS |
| Wedge | `loc.calle.bakery` | M | 600 | 250 | 150 | **1,000** | 2,500 | PASS |
| Wedge | `loc.calle.pharmacy` | S | 350 | 100 | 50 | **500** | 1,200 | PASS |
| Wedge | `loc.calle.everyday_shop` | S | 450 | 100 | 100 | **650** | 1,200 | PASS |
| Wedge | `loc.ribera.workshop` | L | 1,000 | 1,800 | 400 | **3,200** | 5,000 | PASS |
| Wedge | `loc.ribera.service_yard` | L | 400 | 2,100 | 300 | **2,800** | 5,000 | PASS |
| Wedge | `loc.ribera.paseo_edge` | T | 0 | 150 | 150 | **300** | 600 | PASS |
| Ensanche | `loc.ensanche.neighbourhood_anchor` | M | 500 | 800 | 300 | **1,600** | 2,500 | PASS |
| Ensanche | `loc.ensanche.shared_garden` | M | 200 | 1,300 | 300 | **1,800** | 2,500 | PASS |
| Barrio Alto | `loc.barrio.residence_cluster` | L | 1,600 | 1,000 | 400 | **3,000** | 5,000 | PASS |
| Barrio Alto | `loc.barrio.lavadero` | M | 150 | 850 | 200 | **1,200** | 2,500 | PASS |
| Puerto | `loc.puerto.work_hub` | L | 1,200 | 1,900 | 400 | **3,500** | 5,000 | PASS |
| Puerto | `loc.puerto.landing` | T | 0 | 250 | 200 | **450** | 600 | PASS |
| Puerto | `loc.puerto.worker_social` | M | 600 | 500 | 200 | **1,300** | 2,500 | PASS |
| Puerto | `loc.puerto.warehouse_yard` | L | 1,500 | 2,300 | 400 | **4,200** | 5,000 | PASS |
| Entrada | `loc.entrada.arrival` | T | 100 | 300 | 150 | **550** | 600 | PASS |
| Entrada | `loc.entrada.fonda` | M | 900 | 600 | 200 | **1,700** | 2,500 | PASS |
| Entrada | `loc.entrada.depot_forecourt` | M | 500 | 1,500 | 300 | **2,300** | 2,500 | PASS — 200 m² class slack |
| Vega | `loc.vega.supply_node` | L | 800 | 2,000 | 400 | **3,200** | 5,000 | PASS — outside dense subtotal |
| Vega | `loc.vega.quiet_paseo` | T | 0 | 150 | 150 | **300** | 600 | PASS — outside dense subtotal |

Every place has a concrete grammar path in `CITY_ENVIRONMENT_GRAMMAR.md` matching the kind of area charged here: shell/yard/open site/threshold. No fit relies on interior-room count or A/B handle count.

## 3. Dense-part reconciliation

CITY-02 accepted per-part caps/reserves remain authoritative. CITY-05's bounded compositions use less than the conservative class-upper charges accepted in CITY-02; **the unused difference is not permission to consume reserve**.

| Part | Accepted area | CITY-02 A/B cap | Hard ordinary/quiet reserve | 15% circulation/uncommitted | CITY-02 conservative upper charge | CITY-05 bounded composition total | Headroom to cap after CITY-05 bounded total | Result |
|---|---:|---:|---:|---:|---:|---:|---:|---|
| Wedge core | 150,000 | 37,500 | 90,000 | 22,500 | 26,100 | **14,400** | 23,100 | PASS |
| Ensanche | 90,000 | 13,500 | 63,000 | 13,500 | 5,000 | **3,400** | 10,100 | PASS |
| Barrio Alto | 60,000 | 12,000 | 39,000 | 9,000 | 7,500 | **4,200** | 7,800 | PASS |
| Puerto | 45,000 | 15,750 | 22,500 | 6,750 | 13,100 | **9,450** | 6,300 | PASS |
| Entrada | 20,000 | 6,000 | 11,000 | 3,000 | 5,600 | **4,550** | 1,450 | PASS |
| **Dense total** | **365,000** | **84,750 aggregate** | **225,500** | **54,750** | **57,300** | **36,000** | — | **PASS** |

The decisive acceptance boundary remains the inherited **class/cap/reserve** contract, not the lower CITY-05 planning totals. A later geometry realization cannot cite the 36,000 m² number to exceed an individual class ceiling.

## 4. Circulation separation

CITY-05 street families deliberately separate route polygons from place-owned aprons:

- `st.plaza_market_edge`: always-clear public route band is not part of `loc.plaza.market`'s 3,000 m² exclusive envelope;
- `st.port_work_edge`: clear public/service route is not part of warehouse/work-hub yards;
- `st.riverside_promenade`: shared walking route is not part of paseo pause thresholds;
- `st.service_edge`: the service route itself is shared circulation; loading/work yard behind it is exclusive place area;
- junction turning/landing clearances remain circulation/uncommitted area unless a bounded waiting/threshold site is explicitly charged to the place.

If a future site plan requires route/circulation to consume more than the inherited 15% part margin after actual shapes/retaining are known, CITY-05's grammar does not authorize borrowing the hard reserve. That is a downstream fit failure requiring reviewed correction/reopen.

## 5. Hard-reserve protection checks

The following are non-borrowable in this fit:

- upstream `W07` / Vega quiet paseo as compensating development land;
- `W15` ravine/lavadero quiet context;
- upper Barrio/Ensanche ordinary residential fabric around A/B anchors;
- ordinary Puerto `C/S1` work sheds/frontage;
- scenic slopes/roofline.

The bounded compositions above fit without assigning any of those categories as overflow area.

## 6. Causal negative controls

### NC-1 — Entrada depot exceeds M

Current CITY-05 pattern for `loc.entrada.depot_forecourt` is 2,300 m², leaving only 200 m² before the accepted M ceiling.

If actual composition requires **2,600 m²**, CITY-05 may not call it “close enough.” Class truthfulness fails. Reclassifying the place to L invokes CITY-02's accepted conservative upper charge of 5,000 m², so Entrada becomes:

`600 + 2,500 + 5,000 = 8,100 m²`

against a 6,000 m² A/B cap = **FAIL / REOPEN CITY-02 Q6**.

This is intentionally the same causal negative witness accepted by the CITY-02 Reviewer; CITY-05 preserves it rather than weakening it with lower local estimates.

### NC-2 — market consumes route band

If market stalls/occupation are allowed to use the `st.plaza_market_edge` always-clear route and the same area is still counted as circulation, the fit is **FAIL** for shared-route double counting and public-graph obstruction even if total square metres remain below L.

### NC-3 — service yard uses `W17` as public entrance

`loc.ribera.service_yard` may use a service anchor associated with accepted `AS` where appropriate. If its only ordinary public access is achieved by treating `W17` as public, the composition is **FAIL** on CITY-01 access inheritance. Extra area does not cure the access defect.

### NC-4 — workshop borrows quiet paseo reserve

If `loc.ribera.workshop` or another L place requires expansion into the protected quiet paseo/ordinary reserve to fit its yard, the composition is **FAIL** even if district raw area could absorb it.

## 7. Verdict

**PASS at CITY-05 planning-grammar level.**

The reviewed grammar has at least one bounded exterior composition path for all 23 A/B places within each inherited T/S/M/L ceiling. Dense-part bounded compositions total 36,000 m² and remain under every accepted part cap without using hard ordinary/quiet reserve or double-counting shared circulation.

What remains unproved on purpose:

- exact parcel polygons and exact site placement;
- realized street/turning/retaining geometry;
- actual Unity footprint/frontage dimensions;
- any future geometry that exceeds the bounded planning patterns.

Those remain falsifiable. A later breach triggers the explicit reopen rules rather than invalidating the causal separation in this check.