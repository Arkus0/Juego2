# WP-CITY-05 — Environment grammar audit

Semantic owner: `Docs/production/CITY_ENVIRONMENT_GRAMMAR.md`  
Machine-readable requirements projection: `Docs/production/CITY_ENVIRONMENT_DISCOVERY_REQUIREMENTS.json`  
Capacity evidence: `Docs/evidence/WP-CITY-05/CAPACITY_FIT_CHECK.md`

Purpose: audit the CITY-05 candidate against the workpack acceptance/DoD and inherited CITY/ART boundaries without turning this evidence file into a second semantic owner.

## 1. Candidate inventory

| Surface | Count | Coverage |
|---|---:|---|
| street-segment families | 8 | ordinary, historic, stepped, riverside, service, plaza/market, port/work, rural transition |
| junction families | 7 | irregular fork, ordinary tee, bridgehead, X6/X7 landing gate, step landing, plaza gate, service court |
| parcel/open-site families | 9 | historic, commercial, slope, Ensanche garden, workshop court, warehouse yard, civic, rural, open site |
| reusable building families | 9 | house, mixed-use, bar/social, shop/service, workshop, warehouse/port, civic, residential multi, rural/peripheral |
| A/B exterior mappings | 23/23 | every accepted CITY-02 A/B programme row |
| ordinary/scenic mappings | 11 named CITY-02 families | C/S1 ordinary fabric + D/S0 scenic fabric remain explicit |

The catalogue is intentionally smaller than a building-by-building inventory. Variation comes from reviewed parcel relation, frontage bands, floor count, party-wall/end condition, roof/silhouette, setback, slope/retaining relation, sockets and ART-compatible material variants.

## 2. Workpack acceptance audit

### A1 — varied streets/buildings without bespoke design for every parcel

**PASS.** Normal fabric follows `parcel/site -> building family -> shell variant -> bounded parameters -> sockets/material variant`. One-off compositions require a real unmet site/POI need and have explicit promotion criteria; they are not the default production model.

### A2 — old/commercial/residential/port/rural areas differ through shared rules

**PASS.** District identity is a constrained combination of the same families:

- Casco: historic/stepped lanes, tight frontages, party walls and bridge/Cuesta relations;
- Calle Mayor/Plaza: ordinary/public routes, commercial/civic rows and occupiable apron separated from circulation;
- Barrio Alto: stepped lanes, slope parcels, retaining and shared landings;
- Ensanche: flatter garden/setback parcels and lower frontage continuity;
- Ribera: riverside/service edges, workshop courts and ordinary work sheds;
- Puerto: public working edge, low warehouse yards and modest social/civic shells in the same inland material language;
- Entrada/Vega: low-density rural-transition edges, walls/gardens/fields and peripheral shells.

No district-specific asset kit becomes semantic authority.

### A3 — meaningful entrance/service relationships for CITY-02/CITY-06

**PASS.** Every A/B place has an exterior composition mapping. Families expose public/customer, service/material, private/semi-private, vertical/shared-court and staff anchors where required. I-depth stays inherited and exterior-only here:

- I0: no enterable enclosed interior promised;
- I1: one bounded interior-link support;
- I2: shell support for multiple zones/threshold relations;
- I3: only `loc.casco.bar`, with public + service + an additional semi-private/vertical/court candidate.

CITY-06 still owns room topology and discovery use.

### A4 — weaker model can choose constrained reviewed options

**PASS.** The owner defines a selection order from accepted route/district/site constraints through compatible street, parcel, shell, access and capacity checks before proposal. The JSON projection requires later machine-readable identity, bounds, sockets, access anchors, tags, dependencies, variants, style/material constraints, site compatibility, A-D/S/I compatibility, capacity posture, version and provenance.

The JSON is explicitly a **non-canonical requirements projection**. It does not define H1/later capability transport or catalogue authority.

### A5 — no implementation/canonical-scope leak

**PASS.** The candidate contains only CITY production/evidence documentation and one non-canonical JSON requirements projection. It imports no asset, creates no prefab/Unity scene/runtime code, changes no WorldState shape and implements no Arkus public capability. Native Unity locators remain implementation locators, not CITY family identity.

## 3. CITY-06 handoff / Definition of Done

CITY-06 receives reviewed exterior prerequisites rather than a blank slate:

- street/junction and parcel/site vocabularies;
- reusable building/shell families;
- public/service/private/semi-private/vertical/court anchor obligations;
- one valid exterior host path for every A/B place;
- I0–I3 shell-support rules stopping before room layout;
- no-build/view/rear relation vocabulary;
- promotion/reuse and later-discovery requirements.

That is sufficient for CITY-06 to design selective interiors/discovery without inventing exterior shells and access relations anew.

## 4. Inherited mobility/access audit

The grammar creates no graph edges. Checks:

- historic/stepped lanes cannot satisfy inherited `AR`;
- `st.service_edge` does not publicise `AS`;
- road-capable realization has explicit clear-width floors: ordinary `AR` 4–6 m; riverside `AR` ≥4.0 m; service-edge public/service `AR` ≥4.0 m; plaza `AR` 4–5 m; rural-transition `AR` 4–4.5 m; port-work `AR` 4.5–6 m;
- X6/X7 landing represents the state-valid crossing rather than simultaneous ferry+bridge geometry;
- courts/arcades/shared landings remain place-internal relations, not city-graph shortcuts;
- market/work occupation remains outside the always-clear public route.

### Finding 1 repaired — access-width ambiguity

Initial street-family ranges allowed `AR` on families whose shared lower bound was 2.5–3.5 m. Even with inherited access described as authoritative, a downstream author could have selected the narrow end while still claiming `AR` compatibility.

Repair: `b645f26518b29a7bbebad0129f8136f7e0fd0313` encodes access-specific minima directly.

## 5. Capacity / host-fit audit

All 23 A/B rows have a host/site witness that now passes **three** checks simultaneously:

1. exclusive shell + dedicated open + threshold apron fits the named CITY-05 parcel/open-site host;
2. shell coverage fits the parcel family's reviewed coverage posture where applicable;
3. total remains below inherited T/S/M/L.

Dense host-valid witness totals are:

| Part | CITY-05 host-valid witness total | Inherited A/B cap | Status |
|---|---:|---:|---|
| Wedge | 3,730 m² | 37,500 m² | PASS |
| Ensanche | 900 m² | 13,500 m² | PASS |
| Barrio Alto | 500 m² | 12,000 m² | PASS |
| Puerto | 2,700 m² | 15,750 m² | PASS |
| Entrada | 2,150 m² | 6,000 m² | PASS |
| **Dense total** | **9,980 m²** | — | PASS |

The 9,980 m² total is only an existence witness, **not** a replacement budget. CITY-02's conservative upper charges, individual T/S/M/L ceilings, part caps, hard ordinary/quiet reserve and circulation margin remain authoritative.

### Finding 2 repaired — predecessor-cap pass could hide an impossible host

The first `CAPACITY_FIT_CHECK.md` proved totals below T/S/M/L but did not bind those totals back to the selected CITY-05 parcel/site ranges. For example, a multi-thousand-square-metre “workshop” could pass the inherited L ceiling while naming a single `pc.workshop_court` whose own maximum host is 18×32 m.

That was a CITY-05 false-green class, not a CITY-02 defect.

Repairs: `e386b35136515ac4ad2490195fa7a58b4adba25b` binds every A/B to a host witness; `21e063cf0946a3ed8228d20c1f0c39eb7d952688` additionally checks the host family's built-coverage posture and fixes the service-yard/fonda witnesses accordingly.

## 6. Ordinary/quiet/scenic protection

Named non-borrowable fabric remains explicit:

- W07/Vega quiet paseo;
- W15 lavadero/ravine context;
- upper Barrio/Ensanche ordinary residential fabric;
- ordinary Puerto C/S1 work sheds/frontage;
- D/S0 scenic slopes/roofline.

Shared circulation is separately accounted and cannot be counted as both A/B-exclusive area and route margin.

## 7. Causal negative controls

1. A 1,500 m² `loc.ribera.workshop` on one `pc.workshop_court` **fails CITY-05 host fit** despite being below L=5,000 m².
2. A 500 m² shell on a 20×40 `pc.rural_edge` **fails CITY-05 coverage posture** (62.5% > 45%) even if its PE class passes.
3. If `loc.entrada.depot_forecourt` cannot remain within M=2,500 m², reclassification to L invokes CITY-02's 5,000 m² charge and Entrada becomes 8,100 > 6,000 m² -> **REOPEN CITY-02 Q6**.
4. Market occupation consuming the always-clear route while the same area is counted as circulation -> **FAIL** double counting/graph obstruction.
5. `loc.ribera.service_yard` using W17/AS as ordinary public access -> **FAIL** inherited CITY-01 access.
6. Workshop/yard borrowing protected quiet/ordinary reserve -> **FAIL** even if raw area fits.
7. A prefab offering rooms where CITY-02 says I0 does not create an interior promise; treating it as one -> **FAIL** scope/depth inheritance.

## 8. Negative-gate audit

- unique bespoke buildings everywhere: rejected by compact family catalogue + promotion rule;
- marketplace asset pack as semantic authority: rejected;
- procedural generation without reviewed composition constraints: rejected;
- unrelated district kits: rejected;
- service/private shortcut becoming public topology: rejected;
- port becoming maritime/coastal hero identity: rejected;
- scenic/quiet fabric becoming capacity overflow: rejected;
- CITY-06 interior/discovery or Living World runtime semantics authored early: rejected.

## 9. Residuals, not current blockers

Intentionally downstream:

- final parcel polygons/citywide cadastral layout;
- realized Unity widths, grades, turning and retaining geometry;
- exact CITY-03 seed boundary;
- actual prefab/module inventory and asset adoption;
- detailed interiors/discovery;
- runtime permissions/schedules/interactions;
- implemented Arkus discovery capability/schema.

Future realized geometry that cannot satisfy this grammar or an inherited access/capacity guarantee is a falsification/reopen signal, not permission to silently relax the reviewed constraints.

## 10. Audit verdict before formal Worker pre-review

After the two Worker-found repairs above, the candidate covers CITY-05 acceptance and DoD surfaces while preserving predecessor ownership. This file remains evidence only; `CITY_ENVIRONMENT_GRAMMAR.md` is the single semantic owner.

Formal `WORKER_PRE_REVIEW` must still inspect the complete baseline→candidate diff and current dependency state before freeze.