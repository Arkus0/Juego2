# WP-CITY-05 — Environment grammar audit

Semantic owner: `Docs/production/CITY_ENVIRONMENT_GRAMMAR.md`  
Machine-readable requirements projection: `Docs/production/CITY_ENVIRONMENT_DISCOVERY_REQUIREMENTS.json`  
Capacity evidence: `Docs/evidence/WP-CITY-05/CAPACITY_FIT_CHECK.md`

Purpose: audit the CITY-05 candidate against the workpack acceptance/DoD and inherited CITY/ART boundaries without turning this evidence file into a second semantic owner.

## 1. Candidate inventory

The semantic owner defines a compact shared grammar:

| Surface | Count | IDs / coverage |
|---|---:|---|
| street-segment families | 8 | ordinary road, historic lane, stepped lane, riverside promenade, service edge, plaza/market edge, port/work edge, rural transition |
| junction families | 7 | irregular fork, ordinary tee, bridgehead, X6/X7 landing gate, step landing, plaza gate, service court |
| parcel/open-site families | 9 | historic row, commercial row, slope residential, Ensanche garden, workshop court, warehouse yard, civic frontage, rural edge, open site |
| reusable building families | 9 | ordinary house, mixed-use house, bar/social, shop/service, workshop, warehouse/port, civic, residential multi, rural/peripheral |
| A/B exterior mappings | 23/23 | every accepted CITY-02 A/B programme row |
| ordinary/scenic mappings | 11 named CITY-02 families | C/S1 ordinary fabric + D/S0 scenic fabric remain explicit |

This is intentionally smaller than a building-by-building catalogue. Variation is produced through reviewed parcel relation, frontage bands, floor count, party-wall/end condition, roof/silhouette, setback, slope/retaining relation, sockets and ART-compatible material variants.

## 2. Workpack acceptance audit

### A1 — varied streets/buildings without bespoke design for every parcel

**PASS.**

The grammar composes normal fabric through the sequence:

`parcel/site family -> reusable building family -> reviewed shell variant -> bounded parameters -> sockets/material variant`.

A normal street run is expected to vary at least two reviewed dimensions such as frontage bucket, floor count, roof/end condition, setback or family variant. This is an authoring review constraint rather than procedural randomization.

One-off compositions remain allowed only for a real site/POI requirement that the existing grammar cannot represent. Promotion back into a variant/family has explicit reuse and compatibility conditions, so one-offs cannot silently become the normal production model.

### A2 — district differences use constrained shared rules, not unrelated kits

**PASS.**

All districts consume the same nine building families and shared street/parcel vocabulary. Identity changes through combinations:

- Casco — tight historic/stepped fabric, party walls, irregular frontage and bridge/Cuesta relations;
- Calle Mayor / commercial — ordinary 4–6 m road plus commercial rows and mixed/service frontages;
- Barrio Alto — stepped lanes, slope parcels, retaining and shared landings;
- Ensanche — flatter garden/setback parcels and lower frontage continuity;
- Ribera — riverside/service edges, workshop courts and yards;
- Puerto — public working edge, low warehouses/yards and modest social/civic variants using the same inland materials;
- Entrada/Vega — lower-density ordinary/rural-transition edges, fields/gardens/walls and peripheral shells.

There is no district-specific visual kit authority and no asset-pack category becomes semantic truth.

### A3 — meaningful entrance/service relations for CITY-02 and CITY-06

**PASS.**

Every A/B programme row has an exterior composition mapping. Building families distinguish public/customer, service/material, private/semi-private, vertical/shared-court and staff anchors where required. The mapping preserves the CITY-02 I-depth boundary:

- I0 does not promise an enterable interior;
- I1 reserves one bounded interior-link support only;
- I2 reserves shell support for multiple zones/threshold relations;
- I3 is limited to `loc.casco.bar` and requires public + service + at least one additional candidate layer.

Detailed room topology and discovery use remain CITY-06.

### A4 — weaker model can choose among constrained reviewed compositions

**PASS.**

The semantic owner defines an explicit selection sequence: inspect inherited route/district/site -> choose compatible street/junction -> inspect parcel/no-build constraints -> filter shell/building options -> apply CITY-02 A-D/S/I/PE obligations -> validate access/capacity -> propose.

The JSON projection names the later discovery information needed to implement that filtering: stable logical identity, bounds, sockets, access anchors, tags/archetype, dependencies, variants, style/material constraints, site compatibility, A-D/S/I compatibility, capacity posture, version and provenance.

The projection is explicitly non-canonical; it does not pre-empt H1/later capability/schema ownership.

### A5 — no asset import, prefab implementation, Unity scene, catalogue authority or canonical contract

**PASS.**

The candidate contains only production/evidence Markdown plus a non-canonical JSON requirements projection. It creates no Unity project bytes, prefab, imported dependency, runtime code, canonical WorldState shape or Arkus public capability. Native Unity locators are explicitly rejected as CITY family identity, and source assets/prefabs are implementation sources rather than semantic authority.

## 3. Definition-of-Done audit for CITY-06 handoff

CITY-06 receives all exterior prerequisites named by CITY-05:

- reviewed shell/building families;
- parcel/site constraint vocabulary;
- public/service/private/semi-private/vertical/court anchor vocabulary;
- one exterior composition path for every A/B place;
- I0–I3 shell-support obligations that stop before room layouts;
- no-build/view/rear relation vocabulary;
- promotion/reuse rules and discovery information requirements.

Therefore CITY-06 does not need to invent every building shell or parcel relation from scratch before designing selective interiors/discovery.

## 4. Inherited mobility/access audit

The grammar does not create graph edges. Key checks:

- historic/stepped lanes cannot satisfy inherited `AR`;
- `st.service_edge` does not publicise `AS`;
- road-capable realizations have an explicit clear-width floor: ordinary `AR` 4–6 m; riverside `AR` minimum 4.0 m; public/service `AR` service-edge minimum 4.0 m; plaza `AR` 4–5 m; rural-transition `AR` 4–4.5 m; port-work `AR` 4.5–6 m;
- X6/X7 landing family represents the state-valid crossing and never assumes simultaneous ferry+bridge geometry;
- courts/arcades/shared landings are place-internal spatial relations, not implicit city-graph shortcuts;
- public market/work occupation remains outside the always-clear route polygon.

### Worker finding fixed before pre-review

The first semantic-owner draft allowed `AR` in several street families whose shared width range had a lower bound of 2.5–3.5 m. Although the document said inherited access remained authoritative, that representation could let a downstream author choose the narrow end while claiming road-capable compatibility.

Repair commit: `b645f26518b29a7bbebad0129f8136f7e0fd0313`.

The families now encode access-specific bands/minima directly. This is classified as a material Worker-found in-claim ambiguity and is the first pre-freeze finding repaired.

## 5. Capacity / ordinary-fabric audit

`CAPACITY_FIT_CHECK.md` gives all 23 A/B places one bounded grammar composition under their inherited T/S/M/L ceiling. Recomputed dense totals are:

| Part | CITY-05 bounded A/B total | Inherited A/B cap | Status |
|---|---:|---:|---|
| Wedge | 14,400 m² | 37,500 m² | PASS |
| Ensanche | 3,400 m² | 13,500 m² | PASS |
| Barrio Alto | 4,200 m² | 12,000 m² | PASS |
| Puerto | 9,450 m² | 15,750 m² | PASS |
| Entrada | 4,550 m² | 6,000 m² | PASS |
| **Dense total** | **36,000 m²** | — | PASS |

The lower CITY-05 planning totals do **not** replace CITY-02 conservative upper charges. Individual T/S/M/L ceilings, per-part caps, hard reserve and circulation margin remain the fail-closed acceptance boundary.

Named quiet/ordinary fabric remains non-borrowable. The candidate expressly preserves W07/Vega quiet paseo, W15/lavadero context, upper residential ordinary fabric, ordinary Puerto work sheds/frontage and D/S0 scenic mass.

## 6. Causal negative controls

The evidence has explicit conditions that turn the result into FAIL rather than allowing convenient reinterpretation:

1. `loc.entrada.depot_forecourt` needs 2,600 m² or otherwise exceeds M -> class truthfulness fails; L reclassification invokes the inherited 5,000 m² upper charge and Entrada becomes 8,100 > 6,000 m² cap -> **REOPEN CITY-02 Q6**.
2. Market occupation consumes its always-clear route and the same area is still counted as circulation -> **FAIL** on route obstruction/double counting.
3. `loc.ribera.service_yard` obtains ordinary public access by treating W17/AS as public -> **FAIL** on inherited CITY-01 access.
4. Workshop/yard expansion borrows protected quiet/ordinary reserve -> **FAIL** even if raw district area would fit.
5. A discovered prefab has rooms but CITY-02 says I0 -> rooms do not create an interior promise; treating them as one would **FAIL** scope/depth inheritance.

## 7. Negative-gate audit

- unique bespoke buildings everywhere: **rejected** by compact family catalogue + promotion rule;
- marketplace asset pack as semantic authority: **rejected explicitly**;
- procedural generation without reviewed constraints: **rejected explicitly**;
- unrelated district kits: **rejected**;
- service/private shortcut creates public topology: **rejected**;
- port becomes hero harbour/coastal identity: **rejected**;
- scenic/quiet fabric becomes capacity overflow: **rejected**;
- CITY-06 interiors/discovery or Living World runtime semantics authored early: **rejected**.

## 8. Residuals, not current blockers

The candidate intentionally does not prove:

- final exact parcel polygons or citywide cadastral layout;
- realized Unity width/grade/turning/retaining geometry;
- exact retained-seed boundary;
- actual prefab/module inventory or adopted asset dependencies;
- detailed room/interior/discovery topology;
- runtime permissions/schedules/interactions;
- implemented Arkus discovery capability/schema.

These are named downstream ownership surfaces. Concrete future geometry that cannot satisfy this grammar or an inherited access/capacity guarantee is a reopen/falsification signal, not evidence the current planning WP should implement those downstream systems now.

## 9. Audit verdict before strict Worker pre-review

The candidate covers the CITY-05 acceptance and DoD surfaces after repairing the access-width ambiguity. No second semantic owner has been created: this file audits, while `CITY_ENVIRONMENT_GRAMMAR.md` owns the actual grammar.

Formal `WORKER_PRE_REVIEW` remains separate and must challenge the complete baseline→candidate diff before freeze.