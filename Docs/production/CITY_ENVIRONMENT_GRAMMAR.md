# Keeper City — Exterior environment grammar

Version: 0.1 — 2026-09-21  
Workpack: `WP-CITY-05 — Streets, parcels + reusable building families`  
Class: **PRODUCT / ENVIRONMENT PREPRODUCTION — NON-FOUNDATIONAL**

Status: **CITY-05 candidate semantic owner.** This document owns the Keeper City's reusable exterior-production grammar: street/junction families, parcel constraints, building-family specification, composition ladder, CITY-02 shell/access mapping, later-discovery requirements and one-off promotion rules.

It consumes accepted CITY-00 geography, CITY-01 mobility/access, CITY-02 place/depth programme and ART direction. It does **not** redefine those owners. It does not select the CITY-03 seed, create Unity geometry, import assets/prefabs, define a canonical Arkus catalogue, design CITY-06 room/discovery topology, or author runtime schedules, interactions, beliefs, dialogue, incidents or save semantics.

Family IDs below are reviewed **CITY planning IDs**, not canonical runtime IDs, Unity asset IDs or proof that a matching prefab already exists.

---

## 1. Binding inputs and fail-closed boundary

CITY-05 consumes:

- `Docs/production/CITY_SPATIAL_CONSTITUTION.md` — landmasses, district identity, scale envelope, port character and quiet-fabric invariants;
- `Docs/production/CITY_MOBILITY_TOPOLOGY.md` — authoritative route graph, route-character/elevation/access vocabulary and crossing truth;
- `Docs/production/CITY_LOCATION_PROGRAMME.md` — programme IDs, A–D/S0–S4/I0–I3 obligations, access posture and ordinary/scenic scope;
- `Docs/evidence/WP-CITY-02/CAPACITY_SANITY.md` — accepted `T/S/M/L` programme-envelope ceilings, per-part A/B caps, hard ordinary/quiet reserve and separate 15% circulation/uncommitted margin;
- `Docs/art/SETTING.md` + `Docs/art/VISUAL_BIBLE.md` — fictional Potes/Liébana setting, material language, scale cues and vetoes.

Hard rules:

1. A street family can realize only an already accepted CITY-01 edge. **A family never creates graph connectivity.**
2. No parcel, rear lane, courtyard or service relation may create a hidden inter-landmass path, a dry Wedge→Puerto continuation or an Ensanche↔Orilla-sur connection.
3. `AS` can satisfy a service/back anchor only; it cannot be used as an ordinary public frontage merely because a building needs easier access.
4. A/B composition must remain inside its accepted CITY-02 `T/S/M/L` ceiling. If a required place cannot truthfully do so, Q6 reopens before CITY-05 can pass.
5. Shared public street/circulation area is charged to the inherited circulation/uncommitted margin, never double-counted as A/B-exclusive programme envelope or hard ordinary/quiet reserve.
6. Protected quiet fabric remains low-intensity. The grammar may give it useful edges/thresholds but may not turn it into overflow for A/B development.
7. `I0` does not become enterable because a discovered asset happens to contain rooms. `I1–I3` require shell/access affordances only; detailed interiors remain CITY-06.
8. All building families inherit the accepted Liébana valley art language. District identity comes from constrained variation of shared families, not independent visual kits.
9. This is an **authoring-time reviewed grammar**, not a promise of procedural generation.

---

## 2. Grammar model

A legal exterior composition is represented conceptually as:

```text
accepted route edge(s)
  + street / junction family
  + bounded parcel or open-site region
  + parcel constraints
  + reviewed shell/building family variant (optional for exterior-only sites)
  + CITY-02 functional-place binding (optional for ordinary C/D fabric)
```

Every stage narrows choices. A weaker authoring agent should select among compatible reviewed options rather than receive an empty surface and a style prompt.

### 2.1 Dimension semantics

All dimensions in this document are **planning bands**, not measured final geometry.

- frontage/depth values describe a parcel's usable bounding range; the polygon may be irregular;
- clear-route width describes unobstructed movement space, excluding dedicated private work yards and building threshold aprons;
- floor bands are shell-production constraints, not an interior-room specification;
- realized slope, clearance, turning and retaining geometry remain measurable/falsifiable later;
- if a planned band cannot satisfy an inherited access claim in realized geometry, the access claim or composition must reopen at its causal owner rather than be silently degraded.

---

## 3. Street-segment families

A segment family controls cross-section character and frontage relation. It does not own connectivity, route permissions or travel cost.

| ID | Family | Typical clear movement band | Compatible inherited access/elevation | Frontage / edge rule | Primary uses |
|---|---|---:|---|---|---|
| `st.ordinary_road` | ordinary town street | **4.0–6.0 m** | `AR` E0/E1; `AP` E0/E1 where no cart claim is required | mostly continuous frontage; door/shop thresholds stay outside clear route | Calle Mayor, ordinary Ensanche/Entrada road, ordinary connectors |
| `st.historic_lane` | narrow historic lane / cuesta | **2.5–4.0 m** | `AP`/`AF`, E0–E2; never used to satisfy an inherited `AR` claim | irregular flush frontages, occasional widening pocket, party-wall fabric common | Casco, old side lanes, Cuesta-like pedestrian links |
| `st.stepped_lane` | stepped residential lane | **1.8–3.0 m** | `AF`, normally E3; E2 stepped hybrid allowed only where CITY-01 already permits foot access | landings may host doors/benches but clear stair path is continuous | Barrio Alto / upper vertical connections |
| `st.riverside_promenade` | paseo / sirga | **2.5–4.0 m** | `AP` or accepted public `AR`, E0/E1; usually `Q`/`S` character | water/retaining side remains visually open; frontage is intermittent rather than retail-continuous | `W07`, low river walk, quiet/work overlook |
| `st.service_edge` | service/back edge | **3.5–5.0 m** | `AS` or already-accepted service-capable `AR`, E0/E1 | loading/service doors face edge; public entrance requires a separate public anchor unless the inherited edge itself is public | rear delivery, bounded work/service relation |
| `st.plaza_market_edge` | civic/market edge | **3.0–5.0 m always-clear route band** plus separately bounded occupiable apron | public `AP/AR`, E0/E1 | temporary/market occupation cannot consume the always-clear route; civic/shop thresholds open to edge | Plaza, market terrace, civic frontage |
| `st.port_work_edge` | public working frontage / port road | **4.5–6.0 m** clear route plus parcel-owned work apron | public/service-capable `AR`, E0/E1 | work apron belongs to parcel/site envelope; public route remains legible and unobstructed | `O02`, quay/workfront |
| `st.rural_transition` | huerta / road-edge transition | **2.5–4.5 m** depending inherited access | `AR/AP/AF` E0–E2 as inherited | walls, verges, gates and detached edges; no continuous urban frontage requirement | Vega, outer Ensanche/Entrada seam |

### 3.1 Street-family invariants

- `st.historic_lane` and `st.stepped_lane` may never be substituted for an `AR` edge to save space.
- `st.service_edge` does not publicise `AS`.
- `st.riverside_promenade` retains at least one visually open or low-built side where it borders the water/quiet edge; it is not a second Calle Mayor.
- `st.plaza_market_edge` separates **circulation** from **occupiable market apron** so market content cannot erase the accepted public graph.
- `st.port_work_edge` keeps the working frontage ordinary and legible; warehouse yards/work aprons are private/service space outside the route polygon.
- `st.rural_transition` preserves gaps, walls, garden/field edges and lower frontage density; it must not become suburban strip retail by default.

---

## 4. Junction families

A junction family shapes an already-authorized node. Its number and type of exits are derived from CITY-01, never invented by the family.

| ID | Purpose | Rules |
|---|---|---|
| `jn.irregular_fork` | old-quarter/upper-lane fork | 2–3 graph-authorized exits; preserve a readable primary continuation; no hidden through-passage via private court |
| `jn.ordinary_tee` | ordinary road T / offset T | all exits must be accepted edges; `AR` legs retain turning/clearance intent; frontage corner may chamfer/set back |
| `jn.bridgehead` | X1–X5 approach/head | water/no-build corridor remains explicit; public threshold can widen but cannot create a parallel crossing |
| `jn.landing_gate` | X6/X7 Wedge/Puerto landing head | crossing socket binds to **either** state-valid X6 or X7, never both as simultaneous geometry; no dry continuation across water |
| `jn.step_landing` | stair/steep-lane meeting | maintains an unambiguous foot route and landing; wheeled service is not inferred |
| `jn.plaza_gate` | street entering civic/market space | keeps through-route readable even when occupiable apron is active; route exits come from accepted graph |
| `jn.service_court` | public edge to rear/service relation | public and service anchors remain semantically distinct; service court cannot become a public connectivity shortcut |

A courtyard, arcade, passage or shared landing may be spatially traversable inside a place while still **not** becoming a city-graph edge. Later blockout/authoring must keep that distinction visible in metadata.

---

## 5. Parcel and open-site grammar

### 5.1 Required parcel fields

Every reviewed parcel/site proposal must expose:

- bounded polygon/region plus planning bounding dimensions;
- public frontage edge(s), if any;
- service/rear edge(s), if any;
- inherited route/access class associated with each external edge;
- allowed floor/height band;
- party-wall relation (`none`, `left`, `right`, `both`, or reviewed irregular adjacency);
- setback class (`flush`, `threshold`, `garden`, `work_apron`, `civic_open`);
- slope/retaining posture;
- no-build/view corridor intersections;
- permitted rear relation;
- compatible building families/variants;
- CITY-02 programme-envelope class where the site realizes an A/B place;
- whether any enclosed interior is promised (`I0..I3` consumed from CITY-02, never inferred from the parcel).

### 5.2 Parcel/site families

Ranges below are bounding bands, not a requirement for rectangles.

| ID | Frontage | Depth / site depth | Typical floors | Edge relation | Compatible character |
|---|---:|---:|---:|---|---|
| `pc.historic_row` | 5–9 m | 9–18 m | 2–3 | usually 1–2 party walls, flush/threshold setback | Casco / tight old fabric |
| `pc.commercial_row` | 6–12 m | 12–24 m | 2–3 | party wall optional; public front + optional rear/service | Calle Mayor / Plaza edge / mixed-use frontage |
| `pc.slope_residential` | 5–10 m | 10–20 m | 2–3 | stepped/split-level; retaining relation required where grade demands | Barrio Alto / steep old fabric |
| `pc.ensanche_garden` | 8–16 m | 16–30 m | 1–3 | detached/semi-detached; 1.5–5 m garden/threshold setback | Ensanche / quieter residential |
| `pc.workshop_court` | 8–18 m | 15–32 m | 1–2 | public/customer front optional; material/service edge required for work role | Ribera / peripheral service |
| `pc.warehouse_yard` | 12–28 m | 20–45 m | 1–2 | shell + dedicated yard/work apron; service-capable external edge required | Puerto / logistics / depot-like uses |
| `pc.civic_frontage` | 12–24 m | 15–30 m | 2–3 | clear public approach + distinct staff/service relation where programme requires | Ayuntamiento / institutional shell |
| `pc.rural_edge` | 8–20 m | 15–40 m | 1–2 | detached/irregular; wall/gate/garden/work-field relation | Vega / Entrada rural seam |
| `pc.open_site` | site-specific bounded edge | site-specific bounded region | 0–1 support structure | exterior-first; route polygon remains separately protected | bridgehead, market, garden, landing, arrival, quiet pause, forecourt |

### 5.3 Parcel coverage posture

These are **parcel-level** composition bands, not district-wide coverage percentages:

- `pc.historic_row`: roughly 60–90% built footprint of parcel;
- `pc.commercial_row`: 50–85%;
- `pc.slope_residential`: 40–80%;
- `pc.ensanche_garden`: 25–60%;
- `pc.workshop_court`: 20–60%;
- `pc.warehouse_yard`: 15–50%;
- `pc.civic_frontage`: 35–70%;
- `pc.rural_edge`: 15–45%;
- `pc.open_site`: 0–30% support structure by default.

The accepted district-scale coverage assumptions remain CITY-00 planning assumptions and are not replaced by these parcel ranges.

### 5.4 Rear/service relations

Allowed rear relation vocabulary:

- `rear.party_wall` — no usable rear edge;
- `rear.private_court` — private/semi-private court, never an automatic public shortcut;
- `rear.shared_residential` — shared landing/garden relation;
- `rear.service_lane` — only when an accepted `AS` or public/service-capable edge exists;
- `rear.work_yard` — workshop/warehouse material apron;
- `rear.river_work_edge` — working river-facing apron, never a new crossing;
- `rear.retaining` — uphill/downhill retaining wall/service strip;
- `rear.garden_field` — quieter residential/rural relation.

### 5.5 No-build / view-corridor vocabulary

Exact polygons are site data later; CITY-05 freezes the types that must be representable:

- `nb.water_clearance` — keeps watercourse/edge and bank treatment outside ordinary parcel fill;
- `nb.crossing_approach` — keeps X1–X7 approach/head legible and physically unblocked;
- `nb.route_clearance` — protects accepted public/service route polygon and junction turning/landing space;
- `nb.quiet_buffer` — prevents A/B spill from consuming named quiet/ordinary fabric;
- `nb.view_corridor` — protects selected valley/bridge/civic orientation view where a site requires it;
- `nb.retaining_access` — keeps required retaining/drainage/maintenance strip clear;
- `nb.scenic_silhouette` — prevents playable parcels from using scenic-envelope mass as hidden developable land.

A no-build corridor is a constraint, not automatically playable public space.

---

## 6. Reusable building families

All families share the accepted stone/dark-tile Liébana material language. They differ through parcel relation, frontage rhythm, setback, roof/silhouette parameter, service posture and reviewed variants — **not** through unrelated district asset kits.

### 6.1 Shared shell parameters

Every reusable family may vary only through reviewed fields such as:

- frontage-width bucket within compatible parcel range;
- 1/2/3-storey shell where family permits;
- flush / threshold / garden / work-apron setback;
- party-wall left/right/end/corner condition;
- roof ridge/orientation variant permitted by site silhouette;
- 1–3 façade-bay rhythm;
- door/shopfront/sign/balcony socket presence from a reviewed set;
- rear relation compatible with parcel and programme;
- slope/retaining variant;
- wet-stone/dark-tile/material selection inside ART allowlist.

A variant changes bounded shell composition. It does not author character identity, opening hours, ownership, inventory, dialogue or interaction rules.

### 6.2 Catalogue specification

| ID | Family | Core shell rule | Required/optional exterior anchors | Compatible parcels | Typical district uses |
|---|---|---|---|---|---|
| `bf.house` | ordinary house | compact 1–3-storey residential shell; variants `old_row`, `slope`, `ensanche_detached` | primary residential threshold; optional garden/court/balcony; service anchor only when site requires | historic, slope, ensanche, rural | Casco/Barrio/Ensanche ordinary C fabric |
| `bf.mixed_use_house` | mixed-use house/shop/lodging shell | street-level public/service bay + private/upper shell without fixing interior rooms | public frontage, private/upper threshold socket, optional rear service | commercial, historic, ensanche, rural | Calle Mayor mixed frontages, corner shop, fonda shell |
| `bf.bar_social` | bar/social venue | public social frontage plus distinct service/back relation; may support semi-private/upper/court anchor | public entrance **and** service anchor; optional semi-private/vertical/court socket | historic, commercial, work-edge | casco bar; port worker social variant |
| `bf.shop_service` | shop/service | compact public-facing service shell | public/customer entrance; optional staff/storage/service anchor | commercial, ensanche, rural | pharmacy, everyday shop, neighbourhood shop |
| `bf.workshop` | workshop/repair | work bay shell with customer threshold and material/service side | customer/public threshold where programme requires; service/material anchor required | workshop court, warehouse yard, rural | Ribera workshop, repair/supply variants |
| `bf.warehouse_port` | warehouse / port-work building | low robust shell intended to pair with yard/apron; no hero-harbour silhouette | service/material anchor required; bounded public/customer/office threshold optional | warehouse yard, workshop court | Puerto warehouse/work hub, depot support |
| `bf.civic` | civic/municipal | legible public-facing institutional shell within town material language | public civic entrance; staff/service/private-records access anchor as programme requires | civic frontage, commercial row | Ayuntamiento / municipal office variants |
| `bf.residential_multi` | apartment / residence cluster | 2–3-storey multi-household shell or small cluster around shared landing/court | primary residential threshold(s), shared landing/court anchor, optional service/private rear | slope, ensanche, commercial | Barrio residence cluster; selected newer residential |
| `bf.rural_peripheral` | rural/peripheral house/work shell | low-density 1–2-storey house, shed or small work building; irregular wall/garden/field relation | gate/threshold; optional service/work anchor | rural edge, workshop court | Vega supply context, Entrada edge, huerta fabric |

### 6.3 Explicit non-families

These are **not** new building families:

- market stalls/pitches — reviewed modular exterior assemblies on `pc.open_site`;
- bridgeheads/landings/paseo overlooks — open-site compositions;
- scenic D/S0 roofline/slopes — scenic-envelope assemblies, not enterable buildings;
- one-off landmark façade — a reviewed shell variant or composition; it does not automatically become a family;
- imported asset packs — implementation sources only; they do not define semantic families.

---

## 7. Composition ladder

CITY-05 freezes the following authoring ladder:

```text
module
  -> assembly
  -> shell
  -> reusable building
  -> functional POI
  -> street segment
```

### `module`
Small reusable construction element: wall bay, corner, roof piece, opening, stair flight, retaining piece, arcade bay, fence/wall/gate, market stall part. Modules have dimensions, sockets and style/material compatibility but no place semantics.

### `assembly`
Reviewed combination of modules that solves a repeated local problem: façade bay set, roof end, stair/retaining run, shopfront, balcony/galería set, port shed bay, market stall, garden wall/gate.

### `shell`
Exterior volume/site enclosure with stable bounds, frontage orientation, roof/silhouette constraints, external access sockets and compatible parcel relation. A shell does not imply an interior layout.

### `reusable building`
A reviewed shell + family/archetype + declared variants/dependencies/compatibility. It is discoverable and reusable but still has no CITY-02 functional identity by itself.

### `functional POI`
A reusable building or exterior open-site composition bound to one CITY-02 programme ID and its A–D/S/I/access obligations. This binding may require extra anchors but may not invent runtime behaviour.

### `street segment`
One accepted route edge/portion realized with a street family plus compatible parcel/open-site frontages and junction conditions. It is a composition container, not a new graph owner.

A later authoring system may materialize these concepts differently. The semantic order is the requirement: place semantics bind **after** a reviewed reusable shell/site exists, and street composition cannot retroactively create illegal connectivity.

---

## 8. CITY-02 classification → shell/access obligations

A–D, S-depth and I-depth remain orthogonal. CITY-05 maps them to **exterior/shell obligations only**.

### 8.1 Systemic importance

- `A`: explicit functional-POI binding, stable public/service access anchors as required by the programme, and a reviewed place-readable exterior composition. No automatic extra floor, area or interior depth.
- `B`: explicit functional-POI binding and the same truthful access relation, but no requirement for landmark treatment or larger shell.
- `C`: ordinary reusable fabric. Default is family/variant composition without functional-POI binding; promotion requires a later reviewed reason.
- `D`: scenic/context composition. No playable threshold promise unless another owner explicitly promotes it.

### 8.2 Spatial depth

- `S0`: scenic envelope only; no usable entrance/access socket is promised.
- `S1`: authored shell/façade/exterior threshold; may be a meaningful exterior A/B place but does not imply interior depth.
- `S2`: bounded playable exterior/shallow place; shell/site must expose at least one truthful functional access anchor and enough exterior space for its place role.
- `S3`: shell/site must support multiple meaningful thresholds/spatial layers (for example public + rear/service, public + shared court, public + vertical relation), while exact interior path/rooms remain CITY-06.
- `S4`: hero-layer shell must expose multiple truthful access/discovery **opportunities**. CITY-02 permits this only for `loc.casco.bar`; CITY-05 requires public + service plus at least one additional semi-private/vertical/court candidate anchor, without deciding how CITY-06 uses them.

### 8.3 Enclosed-interior priority

- `I0`: no enterable enclosed interior promised. Visual doors/windows may exist as façade modules but are tagged non-promised; discovered asset interiors do not override I0.
- `I1`: shell must reserve one bounded interior-link anchor/volume and any programme-required service relation; room layout remains undefined.
- `I2`: shell must reserve capacity for multiple interior zones/thresholds and expose the distinct public/private/service exterior relations named by CITY-02 where applicable.
- `I3`: layered shell support with public, service and additional semi-private/vertical/court access candidates; only the casco bar carries this CITY-02 commitment.

No rule maps `A→S4`, `B→S3` or `I` from systemic importance.

---

## 9. A/B place → exterior composition mapping

This table constrains family choice without assigning exact parcels or room layouts. `PE` is the inherited CITY-02 programme-envelope class.

| CITY-02 place | PE | Exterior composition pattern | Building/open-site family | Required external relation |
|---|---|---|---|---|
| `loc.casco.bar` | M | historic/commercial shell + bounded rear/court relation | `bf.bar_social` on `pc.historic_row`/`pc.commercial_row` | public + service + extra semi-private/vertical/court candidate |
| `loc.casco.bridgehead` | T | widened historic/open threshold at X1 approach | `pc.open_site` + `jn.bridgehead` | public approach only; no new crossing |
| `loc.plaza.ayuntamiento` | M | civic frontage on plaza edge | `bf.civic` + `pc.civic_frontage` | public civic + staff/service/private relation |
| `loc.plaza.market` | L | bounded occupiable apron/stall assemblies outside always-clear route | `pc.open_site` + `st.plaza_market_edge` | public; route remains open |
| `loc.calle.bakery` | M | mixed commercial/work shell | `bf.mixed_use_house` or `bf.shop_service` on `pc.commercial_row` | public shop + rear/service/work relation |
| `loc.calle.pharmacy` | S | compact service shell | `bf.shop_service` on `pc.commercial_row` | public + staff/storage relation |
| `loc.calle.everyday_shop` | S | compact mixed/service shell | `bf.shop_service` / `bf.mixed_use_house` | public + optional service |
| `loc.barrio.residence_cluster` | L | small stepped residential cluster/shared landing | `bf.residential_multi` on slope parcels | private + semi-private + public threshold; no public shortcut through private court |
| `loc.barrio.lavadero` | M | open/low-built shared domestic edge | `pc.open_site` + rural/step assemblies | public/semi-private edge; preserve quiet route |
| `loc.ensanche.neighbourhood_anchor` | M | corner mixed/service shell + small bounded square edge | `bf.mixed_use_house` / `bf.shop_service` + `pc.ensanche_garden`/open site | public + service; square does not erase route |
| `loc.ensanche.shared_garden` | M | shared garden/court with residential edge | `pc.open_site` + `bf.house`/`bf.residential_multi` edges | semi-private + public edge |
| `loc.ribera.workshop` | L | work shell + yard/court | `bf.workshop` + `pc.workshop_court` | customer/public + material/service |
| `loc.ribera.service_yard` | L | yard-first service composition + shallow support shell | `pc.workshop_court` + `bf.warehouse_port`/`bf.workshop` support | service-first; public edge only where CITY-02 allows |
| `loc.ribera.paseo_edge` | T | open overlook/work observation on riverside route | `pc.open_site` + `st.riverside_promenade` | public, low-intensity, water edge open |
| `loc.puerto.work_hub` | L | low municipal/work shells + bounded work apron | `bf.warehouse_port` + optional `bf.civic` small office | public work frontage + service/staff relation |
| `loc.puerto.landing` | T | landing/wait threshold at accepted X6/X7 head | `pc.open_site` + `jn.landing_gate` | public subject to inherited crossing state; no simultaneous X6+X7 assumption |
| `loc.puerto.worker_social` | M | modest social shell within working frontage | `bf.bar_social` / `bf.mixed_use_house` | public + service + semi-private candidate |
| `loc.puerto.warehouse_yard` | L | low warehouse shell + dedicated yard/apron | `bf.warehouse_port` + `pc.warehouse_yard` | service/material + bounded public frontage |
| `loc.entrada.arrival` | T | bus/arrival waiting threshold | `pc.open_site` on ordinary/rural-transition edge | public arrival; BUS does not continue into pedestrian streets |
| `loc.entrada.fonda` | M | mixed lodging/social shell + private/service rear | `bf.mixed_use_house` with reviewed lodging variant | public common threshold + private/service relation |
| `loc.entrada.depot_forecourt` | M | forecourt-first service site + small support shell | `pc.open_site`/`pc.warehouse_yard` + `bf.warehouse_port` support | service + public waiting edge; must remain within M ceiling |
| `loc.vega.supply_node` | L | rural work/supply shell + bounded work apron | `bf.rural_peripheral` / `bf.workshop` + `pc.rural_edge` | public work edge + private/service portion |
| `loc.vega.quiet_paseo` | T | small pause/rest threshold on quiet rural/river edge | `pc.open_site` + `st.rural_transition`/riverside | public, intentionally low intensity |

If a place requires a family/parcel combination outside this table, the proposal must explain why existing reviewed families cannot represent it before adding a one-off family.

---

## 10. Ordinary/scenic family mapping

CITY-02's cheap fabric is intentionally composed from the same shared grammar:

- `fam.casco.houses` → `bf.house.old_row` + `pc.historic_row`;
- `fam.plaza.arcades` → commercial/civic assemblies on `pc.commercial_row` / plaza edge;
- `fam.calle.mixed_frontages` → `bf.mixed_use_house` + `bf.house` commercial variants;
- `fam.calle.landmark_shell` → reviewed shell variant with **no playable/interior promise**;
- `fam.barrio.homes` → `bf.house.slope` + `pc.slope_residential`;
- `fam.ensanche.homes` → `bf.house.ensanche_detached` / `bf.residential_multi` on `pc.ensanche_garden`;
- `fam.ribera.sheds` → low `bf.workshop` / `bf.warehouse_port` ordinary variants;
- `fam.puerto.sheds` → low `bf.warehouse_port` ordinary variants with no automatic POI binding;
- `fam.entrada.road_edge` → `bf.rural_peripheral` / ordinary house/work shells;
- `fam.vega.huertas` → walls/gates + `bf.rural_peripheral` support shells;
- `fam.scenic.slopes` / `fam.scenic.roofline` → scenic assemblies outside playable parcel promises.

Substantial C/S1 and D/S0 fabric therefore remain first-class outputs of the grammar rather than unfilled space waiting to be promoted.

---

## 11. District composition profiles — shared kit, constrained difference

Districts select different **combinations** of the same street/parcel/building grammar.

| District | Street emphasis | Parcel emphasis | Building emphasis | What makes it distinct without a separate kit |
|---|---|---|---|---|
| Casco Viejo | historic lane, irregular fork, bridgehead | historic row, slope edge | house, bar/social, mixed use | tight frontage, party walls, irregular widths, Cuesta/bridge relation |
| Plaza / Ayuntamiento | plaza-market edge, ordinary approach | civic + commercial + open site | civic, mixed use, shop/service | occupiable civic apron + legible public thresholds |
| Calle Mayor | ordinary road, occasional historic side lane | commercial row | mixed use, shop/service, house | continuous everyday frontage, 6–12 m rhythm, rear service where legal |
| Barrio Alto | stepped/historic lane | slope residential | house, residential multi | retaining/split-level variants, shared landings, quieter frontage |
| Ensanche | ordinary road, quiet secondary | garden parcels + open shared site | house, residential multi, shop/service | setbacks/gardens, flatter regularity, lower frontage continuity |
| Ribera / Talleres | riverside + service edge | workshop court/open site | workshop, warehouse support | intermittent work frontage, yards and low paseo relation |
| Puerto Fluvial | port-work edge + landing gate | warehouse yard/open site | warehouse/port, workshop, modest social/civic | low sheds/yards + ordinary labour; same inland materials, no hero harbour kit |
| Entrada / Carretera | ordinary road + rural transition | open/warehouse/rural edge | mixed use, warehouse support, rural peripheral | low-density arrival/forecourt seam, no urban downtown block language |
| La Vega | rural transition + quiet riverside | rural/open site | rural peripheral, house/work support | walls/gates/huerta edges, low built coverage, quiet continuity |

### 11.1 Variation without bespoke-everywhere production

A normal frontage/building is produced by choosing:

1. one compatible parcel/site family;
2. one compatible building family;
3. one reviewed shell variant;
4. bounded frontage/floor/party-wall/roof/setback parameters;
5. permitted socket set;
6. ART-compatible material/style variant.

The result may look different while staying structurally reusable. Exact clone chains are not the default: a reviewed street composition should normally vary at least **two** of family variant, frontage-width bucket, floor count, roof/end condition or setback across a run of ordinary parcels. This is an authoring review rule, not a procedural-randomization algorithm.

---

## 12. Capacity accounting rule inherited from CITY-02

CITY-05 distinguishes three area categories:

1. **A/B-exclusive programme envelope** — shell footprint + dedicated yard/court/garden/market/work apron + immediate place-specific threshold apron;
2. **shared circulation/uncommitted** — public street/path route polygons, junction clearances, awkward-shape/retaining allowance;
3. **hard ordinary/quiet reserve** — ordinary C/private/low-intensity fabric that is unavailable as A/B overflow.

They may not be double-counted.

Accepted CITY-02 class ceilings remain:

- `T` ≤ **600 m²**;
- `S` ≤ **1,200 m²**;
- `M` ≤ **2,500 m²**;
- `L` ≤ **5,000 m²**.

CITY-05's evidence must show one plausible bounded composition for every A/B place below its inherited ceiling. A fit is not permission to spend the full ceiling. If realized or later-reviewed exterior composition needs more than the ceiling, the correct result is **REOPEN CITY-02 Q6**, not “borrow from ordinary frontage” or reclassify silently.

Special negative control retained from CITY-02: `loc.entrada.depot_forecourt` must remain truthfully M-sized. If it needs L, Entrada's inherited part cap fails under the accepted Q6 arithmetic.

---

## 13. Later Arkus discovery requirements

CITY-05 requires later bridge/authoring discovery to expose enough structured data that an agent can filter legal/reviewed options before proposing geometry.

For every discoverable environment composition unit (module, assembly, shell, reusable building, street family or parcel template), later discovery must expose where applicable:

| Field group | Required information |
|---|---|
| identity | stable logical ID, kind, human label, review status |
| version | item version/revision, compatibility version, deprecated/superseded marker |
| bounds | local dimensions/bounds and any min/max parametric band |
| sockets | socket ID/type, orientation/edge role, clearance requirement, compatible link types |
| access anchors | public/service/private/semi-private role; compatible CITY access classes; required clear approach |
| archetype/tags | building/street/parcel archetype plus district/use/style tags |
| dependencies | required modules/assemblies/material families/schema adapters or provider-owned dependencies |
| variants | reviewed variant IDs and allowed parameter ranges |
| style/material | ART family/allowlist, material constraints and explicit veto tags |
| parcel/site compatibility | compatible parcel families, party-wall/setback/slope/rear relations, no-build/view requirements |
| place/depth compatibility | permitted A–D/S/I postures without deriving one classification from another |
| capacity | compatible `T/S/M/L` classes where relevant plus exclusive-envelope accounting posture |
| provenance | semantic owner/review reference and source/provider identity |

### 13.1 Authority boundary

These are **requirements on later discovery**, not a new Arkus canonical contract.

Consistent with H1 architecture:

- logical catalogue identity may be project/bridge-owned and discoverable;
- native Unity GUID/local IDs/paths are implementation locators, not CITY family identity;
- source assets/prefabs may implement a family but do not become semantic authority for it;
- canonical `WorldState` identity and CITY planning-family identity are separate concepts;
- a later capability/schema owner may choose the transport/serialization shape while preserving the information above.

`Docs/production/CITY_ENVIRONMENT_DISCOVERY_REQUIREMENTS.json` is a non-canonical machine-readable requirements projection of this section. This Markdown remains semantic owner.

---

## 14. Agent selection / validation sequence

A reviewed authoring workflow should be able to execute this order:

```text
1. inspect accepted route edge + district + site constraints
2. choose compatible street/junction family
3. inspect parcel/open-site constraints and no-build corridors
4. filter reusable building/shell options by parcel + access + slope + style compatibility
5. if a CITY-02 place is being realized, apply its A-D/S/I/PE obligations
6. validate public/service anchors against inherited CITY-01 access
7. validate exclusive-envelope accounting against T/S/M/L ceiling and district cap/reserve boundary
8. propose composition
9. later plan/dry-run/materialize/inspect through the owning Arkus/engine contracts
```

A prompt such as “make a nice Potes house here” is insufficient authoring input. The system must first expose the constrained choice set.

---

## 15. One-off → variant/family promotion

One-off composition is allowed for a specific site/POI when the reviewed grammar cannot represent a real requirement without forcing a bad fit. It is not the default escape hatch.

### 15.1 Promote a one-off to a reviewed **variant** only when

1. it obeys an existing building/street/parcel family's structural contract;
2. its differences can be expressed through declared dimensions, sockets, dependencies, style constraints and compatibility metadata rather than hidden site-specific logic;
3. it is demonstrated to fit at least **two** compliant site contexts or one current site plus one independently valid hypothetical site within the same reviewed grammar;
4. its place-specific dressing/meaning can be removed without breaking the reusable shell;
5. it passes the same access/capacity/no-build checks as existing variants;
6. it receives a stable logical ID + version and review provenance.

### 15.2 Promote variants into a new **family** only when

- at least two reviewed variants share a structural contract that cannot be represented honestly as variants of an existing family;
- the new family closes a repeated production need rather than a single landmark exception;
- existing family compatibility matrices would become misleading if the variants stayed under an old family;
- the family adds no runtime/gameplay semantics and remains within CITY-05 exterior authority.

### 15.3 Never promote because

- an asset pack happens to contain a prefab category;
- one district “needs its own kit” for novelty;
- a one-off landmark is visually memorable;
- procedural generation would be easier with more categories;
- a downstream runtime mechanic wants a convenient semantic shortcut.

Promotion changes the reviewed authoring catalogue specification; it does not adopt an asset dependency or create canonical runtime authority.

---

## 16. Negative gates / rejection rules

A CITY-05 proposal is invalid if any of these occur:

- a street/junction family adds connectivity not present in CITY-01;
- an `AS` service edge is used as ordinary public routing;
- a required `AR` relation is squeezed into a lane/stair family that does not support it;
- a parcel or yard uses protected quiet/ordinary reserve as A/B overflow;
- an A/B composition exceeds its inherited T/S/M/L ceiling without reopening Q6;
- a market/plaza/work apron consumes the public route polygon;
- a private/shared court becomes a silent graph shortcut;
- `I0` ordinary fabric is promoted to open interior because an asset offers rooms;
- every district receives unrelated bespoke building kits instead of shared-family variation;
- the port turns into a maritime/harbour hero identity or changes material language;
- scenic D/S0 mass is counted as playable parcel/capacity;
- a discovered prefab/asset category is treated as semantic authority;
- a procedural generator is promised without reviewed composition constraints;
- CITY-06 interior/discovery topology or Living World runtime semantics are authored here.

---

## 17. What CITY-06 receives

CITY-06 receives, without having to invent exterior production rules anew:

- street/junction family and parcel/site vocabularies;
- reusable building families and reviewed shell variants;
- exterior public/service/private/semi-private/vertical/court anchor obligations;
- exact mapping from every A/B programme place to allowed exterior composition families;
- `I0..I3` shell-support rules that deliberately stop before room/discovery layout;
- no-build/view/rear relation vocabulary;
- later-discovery field requirements;
- the rule that one-off exceptional shells should remain exceptional unless they pass promotion criteria.

CITY-06 remains owner of reusable interior families, room/threshold topology, private/service/vertical discovery patterns, authored/systemic/hybrid discovery and secret-content restraint.

---

## 18. Residuals and downstream ownership

CITY-05 intentionally leaves open:

- exact parcel polygons/site placement and retained-seed boundary — CITY-03 selection + later realization;
- realized street lengths/grades/turning/clearances and Unity geometry — CITY-04 where inside its seed and later geometry owners elsewhere;
- detailed room layouts, interior families, hidden/service routes, discovery layers and secret classes — CITY-06;
- final asset/prefab/module inventory and imported dependency adoption — ART/H1/later production owners;
- Arkus public capability/schema implementation for discovery — owning H1/later authoring WPs;
- keeper realization — CITY-07 after H1-GATE;
- public authoring/reuse-cost proof — CITY-08;
- runtime schedules, access permissions over time, beliefs, dialogue, decisions, incidents, NPC bindings and save semantics — Living World/runtime owners;
- final canonical game IDs and Unity native locators.

A downstream finding that exterior geometry cannot meet a declared range or inherited access/capacity guarantee is a falsification signal, not permission to silently relax this grammar.