# WP-CITY-05 — Exterior grammar capacity fit check

Semantic owner: `Docs/production/CITY_ENVIRONMENT_GRAMMAR.md`  
Inherited capacity owner: `Docs/evidence/WP-CITY-02/CAPACITY_SANITY.md`  
Purpose: test whether the CITY-05 street/parcel/building grammar can realize every accepted CITY-02 A/B place without exceeding its inherited `T/S/M/L` programme-envelope ceiling, contradicting CITY-05's own parcel/site ranges or consuming the hard ordinary/quiet reserve.

This evidence does **not** draw final parcel polygons. It gives one bounded **host/site witness** per A/B place. Each witness must pass two independent tests:

1. the exclusive place composition fits inside a parcel/open-site range allowed by `CITY_ENVIRONMENT_GRAMMAR.md`;
2. the exclusive place total remains below the inherited CITY-02 `T/S/M/L` ceiling.

Final placement and realized geometry remain falsifiable downstream.

## 1. Accounting rule

Each A/B place is decomposed into three exclusive ground-envelope components:

1. `shell/support` — building/support-structure footprint required by that place;
2. `dedicated open` — place-owned yard/court/garden/market occupation/work apron;
3. `threshold apron` — immediate place-specific threshold/working apron outside the shared route polygon.

Excluded from every total:

- shared public street/path route polygon;
- junction clearances serving the route rather than one place;
- water;
- scenic envelope;
- ordinary neighbouring C fabric;
- hard ordinary/quiet reserve.

The witness dimensions below are **planning bounding witnesses**, not final rectangles. Irregular final sites may occupy less of the bounding box. A witness is valid only when its width/depth fall within the named CITY-05 parcel family, or when `pc.open_site` explicitly permits a bounded site-specific region.

Inherited CITY-02 class ceilings remain unchanged:

| Class | Accepted upper bound |
|---|---:|
| T | 600 m² |
| S | 1,200 m² |
| M | 2,500 m² |
| L | 5,000 m² |

## 2. Per-place grammar-fit witnesses

| Part | Place | PE | Host/site witness | Shell | Open | Apron | Exclusive total | PE ceiling | Result |
|---|---|---:|---|---:|---:|---:|---:|---:|---|
| Wedge | `loc.casco.bar` | M | `pc.commercial_row`, within 12×24 m host | 180 | 60 | 30 | **270** | 2,500 | PASS |
| Wedge | `loc.casco.bridgehead` | T | `pc.open_site`, bounded 20×10 m | 0 | 150 | 50 | **200** | 600 | PASS |
| Wedge | `loc.plaza.ayuntamiento` | M | `pc.civic_frontage`, within 24×30 m host | 450 | 150 | 70 | **670** | 2,500 | PASS |
| Wedge | `loc.plaza.market` | L | `pc.open_site`, bounded 25×24 m market/terrace order | 50 | 500 | 50 | **600** | 5,000 | PASS |
| Wedge | `loc.calle.bakery` | M | `pc.commercial_row`, within 12×24 m host | 180 | 60 | 30 | **270** | 2,500 | PASS |
| Wedge | `loc.calle.pharmacy` | S | `pc.commercial_row`, within 10×20 m host | 140 | 30 | 20 | **190** | 1,200 | PASS |
| Wedge | `loc.calle.everyday_shop` | S | `pc.commercial_row`, within 10×24 m host | 170 | 40 | 20 | **230** | 1,200 | PASS |
| Wedge | `loc.ribera.workshop` | L | `pc.workshop_court`, within 18×32 m host | 250 | 250 | 50 | **550** | 5,000 | PASS |
| Wedge | `loc.ribera.service_yard` | L | `pc.workshop_court`, within 18×32 m host | 100 | 400 | 50 | **550** | 5,000 | PASS |
| Wedge | `loc.ribera.paseo_edge` | T | `pc.open_site`, bounded 20×10 m | 0 | 150 | 50 | **200** | 600 | PASS |
| Ensanche | `loc.ensanche.neighbourhood_anchor` | M | `pc.ensanche_garden`, within 15×30 m host | 180 | 220 | 50 | **450** | 2,500 | PASS |
| Ensanche | `loc.ensanche.shared_garden` | M | `pc.open_site`, bounded 30×15 m | 0 | 400 | 50 | **450** | 2,500 | PASS |
| Barrio Alto | `loc.barrio.residence_cluster` | L | `pc.slope_residential`, within 10×20 m host; vertical/shared-landing capacity supplies the cluster role | 150 | 30 | 20 | **200** | 5,000 | PASS |
| Barrio Alto | `loc.barrio.lavadero` | M | `pc.open_site`, bounded 20×15 m | 50 | 200 | 50 | **300** | 2,500 | PASS |
| Puerto | `loc.puerto.work_hub` | L | `pc.warehouse_yard`, within 20×45 m host | 350 | 450 | 100 | **900** | 5,000 | PASS |
| Puerto | `loc.puerto.landing` | T | `pc.open_site`, bounded 25×10 m | 0 | 200 | 50 | **250** | 600 | PASS |
| Puerto | `loc.puerto.worker_social` | M | `pc.workshop_court`, within 15×30 m host | 250 | 150 | 50 | **450** | 2,500 | PASS |
| Puerto | `loc.puerto.warehouse_yard` | L | `pc.warehouse_yard`, within 25×44 m host | 350 | 650 | 100 | **1,100** | 5,000 | PASS |
| Entrada | `loc.entrada.arrival` | T | `pc.open_site`, bounded 20×15 m | 50 | 200 | 50 | **300** | 600 | PASS |
| Entrada | `loc.entrada.fonda` | M | `pc.rural_edge`, within 20×35 m host | 350 | 250 | 50 | **650** | 2,500 | PASS |
| Entrada | `loc.entrada.depot_forecourt` | M | `pc.warehouse_yard`, within 28×45 m host | 250 | 850 | 100 | **1,200** | 2,500 | PASS |
| Vega | `loc.vega.supply_node` | L | `pc.rural_edge`, within 20×35 m host | 250 | 400 | 50 | **700** | 5,000 | PASS — outside dense subtotal |
| Vega | `loc.vega.quiet_paseo` | T | `pc.open_site`, bounded 20×10 m | 0 | 150 | 50 | **200** | 600 | PASS — outside dense subtotal |

The witness is intentionally a ground-envelope test. I2/I3 interior depth can use multiple floors/zones without inflating the ground envelope, so interior-room count remains neither a capacity oracle nor an excuse to exceed a parcel/site range.

## 3. Cross-check against CITY-05 parcel ranges

Every non-open witness stays inside the semantic owner's current host range:

- `pc.commercial_row` ≤ 12 m frontage × 24 m depth;
- `pc.civic_frontage` ≤ 24 × 30 m;
- `pc.workshop_court` ≤ 18 × 32 m;
- `pc.ensanche_garden` ≤ 16 × 30 m;
- `pc.slope_residential` ≤ 10 × 20 m;
- `pc.warehouse_yard` ≤ 28 × 45 m;
- `pc.rural_edge` ≤ 20 × 40 m.

`pc.open_site` is site-specific by definition, so each witness states an explicit bounded region instead of relying on an unbounded “open space” category.

This check closes a false-green class: passing the inherited T/S/M/L ceiling is **not sufficient** if the claimed host parcel/site is too small for the same composition.

## 4. Dense-part reconciliation

| Part | Accepted area | CITY-02 A/B cap | Hard ordinary/quiet reserve | 15% circulation/uncommitted | CITY-02 conservative upper charge | CITY-05 host-valid witness total | Result |
|---|---:|---:|---:|---:|---:|---:|---|
| Wedge core | 150,000 | 37,500 | 90,000 | 22,500 | 26,100 | **3,730** | PASS |
| Ensanche | 90,000 | 13,500 | 63,000 | 13,500 | 5,000 | **900** | PASS |
| Barrio Alto | 60,000 | 12,000 | 39,000 | 9,000 | 7,500 | **500** | PASS |
| Puerto | 45,000 | 15,750 | 22,500 | 6,750 | 13,100 | **2,700** | PASS |
| Entrada | 20,000 | 6,000 | 11,000 | 3,000 | 5,600 | **2,150** | PASS |
| **Dense total** | **365,000** | **84,750 aggregate** | **225,500** | **54,750** | **57,300** | **9,980** | **PASS** |

The 9,980 m² witness total is **not** a replacement capacity budget. It proves that one legal CITY-05 grammar realization exists below the accepted conservative bounds. CITY-02's larger class-upper charges, per-part caps, hard reserve and circulation margin remain the fail-closed acceptance boundary for any later composition.

## 5. Circulation and reserve separation

- `st.plaza_market_edge`: always-clear public route is outside market exclusive envelope;
- `st.port_work_edge`: public/service route is outside warehouse/work-hub yard;
- `st.riverside_promenade`: shared route is outside pause threshold;
- `st.service_edge`: service route is shared circulation; parcel-owned loading/work space is exclusive;
- junction/turning/landing clearances stay circulation unless a bounded place-specific threshold is explicitly charged.

Non-borrowable categories remain W07/Vega quiet paseo, W15 lavadero/ravine context, upper Barrio/Ensanche ordinary residential fabric, ordinary Puerto C/S1 work frontage and D/S0 scenic mass.

## 6. Causal negative controls

### NC-1 — host-fit false green

Take `loc.ribera.workshop`. Its accepted CITY-02 L ceiling is 5,000 m², but the selected `pc.workshop_court` host is at most 18×32 = 576 m². A proposal claiming a 1,500 m² exclusive workshop composition on **one** `pc.workshop_court` therefore **FAILS CITY-05 host fit** even though 1,500 < 5,000. Passing the predecessor cap cannot rescue an internally impossible parcel witness.

### NC-2 — Entrada depot exceeds M

The current host-valid depot witness is 1,200 m². If later requirements cannot remain at or below the inherited M ceiling of 2,500 m², CITY-05 may not stretch/relabel M. Reclassifying to L invokes CITY-02's accepted 5,000 m² upper charge, so Entrada becomes:

`600 + 2,500 + 5,000 = 8,100 m²`

against a 6,000 m² A/B cap = **FAIL / REOPEN CITY-02 Q6**.

### NC-3 — market consumes route band

If market occupation uses the always-clear route and the same area remains counted as circulation, the proposal **FAILS** on shared-route double counting/public-graph obstruction even when below L.

### NC-4 — service yard uses W17 as public entrance

If `loc.ribera.service_yard` obtains ordinary public access by treating W17/`AS` as public, the proposal **FAILS** inherited CITY-01 access. Extra area cannot cure it.

### NC-5 — quiet-reserve borrowing

If workshop/yard expansion needs protected quiet/ordinary reserve, the proposal **FAILS** even when raw district area would otherwise fit.

## 7. Verdict

**PASS at CITY-05 planning-grammar level.**

All 23 A/B places now have a bounded exterior witness that simultaneously fits its selected CITY-05 parcel/open-site host and its inherited T/S/M/L ceiling. The dense host-valid witness total is 9,980 m², every dense part remains under its inherited cap, shared circulation is separated, and no hard ordinary/quiet reserve is used.

Exact parcel polygons, final site placement and realized Unity geometry remain downstream and falsifiable. Any later host-range/access/capacity breach triggers the explicit fail/reopen rules rather than being hidden by this planning evidence.