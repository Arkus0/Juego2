# Keeper City — Retained product seed + exact local-validation specification

Version: 0.1 — 2026-09-21  
Workpack: `WP-CITY-03 — Retained product seed + exact scenario specification`  
Class: **PRODUCT / KEEPER-SEED PREPRODUCTION — NON-FOUNDATIONAL**

Status: **CITY-03 candidate semantic owner.** This document selects the exact retained product seed and freezes the bounded semantic handoff to CITY-04. It consumes accepted CITY-00 geography, CITY-01 mobility, CITY-02 place/depth programme, CITY-05 exterior grammar and CITY-06 interior/discovery grammar without changing them.

It does **not** create Unity geometry, import assets, measure traversal, author runtime Living World/gameplay semantics, change accepted crossings/routes, promote ordinary I0 content, or prove future conditional discovery routes.

Seed comparison evidence: `Docs/evidence/WP-CITY-03/SEED_COMPARISON.md`.

---

## 1. Selection result

Selected seed: **`seed.confluence_civic_commercial`**.

The selected seed is the smallest retained slice that simultaneously carries:

- the accepted confluence-tip/Casco identity;
- one civic/market edge;
- the old bridge X1 as real traversable structure;
- one Arroyo crossing, X5, preserving its low-water-only status;
- the bar hero interior commitment;
- one deep civic interior;
- one shallow everyday commercial interior;
- ordinary closed residential/commercial frontages;
- a short river edge and the Cuesta continuation to the future X6/X7 landing socket;
- enough internal public branching and threshold variety for CITY-04 follow/search and service/private-readability tests;
- clean seams toward Calle Mayor/Vega, Ensanche, Puerto and Orilla-sur/Entrada without moving the retained core.

This is the **first keeper piece of the shipping city**, not a tutorial island, arena or disposable harness map.

---

## 2. Exact boundary semantics

### 2.1 Exactness rule

CITY-03 exactness means the following are frozen for downstream construction:

1. which accepted planning anchors/edge portions are inside or outside;
2. which crossings are fully represented;
3. which place IDs, frontage slots and exterior/interior families are committed;
4. which boundary cuts are expansion seams;
5. which water/no-build regions remain non-traversable;
6. the seed-local planning polygon used as CITY-04's initial blockout target.

The coordinates below are **planning geometry**, not measured survey truth. CITY-04 may discover that metric bends, grades, widths or anchor distances need revision, but it must record any material deviation against CITY-03 rather than silently choosing another seed or changing topology.

### 2.2 Seed-local planning frame

Use a local 2D planning frame in metres:

- origin `(0,0)` = accepted `W.LANDING` planning anchor;
- `+U` = generally upstream along the Wedge toward Casco → Plaza → Calle Mayor;
- `+V` = generally transverse from the Río-facing low side toward the Arroyo / Ensanche side.

This frame does **not** redefine cardinal geography. It is a construction handoff coordinate system only.

### 2.3 Hard outer polygon

The hard seed boundary is the closed polygon below, in order:

| Boundary vertex | U (m) | V (m) | Semantic role |
|---|---:|---:|---|
| `B01` | -20 | -60 | confluence/river-facing lower seam |
| `B02` | 70 | -80 | X1 far-head / Orilla-sur receiving-stub envelope |
| `B03` | 205 | -48 | lower civic/commercial edge |
| `B04` | 245 | 22 | Calle Mayor commercial expansion cut |
| `B05` | 220 | 95 | upper commercial/Casco fabric cut |
| `B06` | 150 | 145 | Arroyo-side / future upper-residential continuation cut |
| `B07` | 70 | 132 | X5 Ensanche-side receiving-stub envelope |
| `B08` | -20 | 55 | landing-side/confluence visual seam |

Shoelace area: **44,817.5 m² = 0.0448175 km²**.

This is inside CITY-00's accepted `0.03–0.06 km²` retained-seed band. Water/no-build voids remain inside the gross spatial envelope but are not traversable land. A conservative net retained land/bridge estimate also remains inside the band (≈0.0386 km²); exact bank/water surface area is a CITY-04 physical measurement, not a CITY-03 proof dependency.

### 2.4 Water and no-build masks

The outer polygon never makes water traversable by inclusion. Two semantic masks remain binding:

- `mask.rio` — the accepted Río/river separation around X1/confluence. **Only X1** is traversable across the river inside this seed. The W.LANDING edge is a seam toward future X6/X7; no dry Wedge→Puerto continuation exists.
- `mask.arroyo` — the accepted Arroyo separation at the lower Casco side. **Only X5** is traversable across the Arroyo inside this seed, and only when its inherited low-water availability condition is active.

CITY-04 may shape banks/retaining geometry but may not create an additional crossing, a hidden ford, a walkable water mesh or a private-court bridge.

### 2.5 Selected anchor targets

Seed-local target anchors:

| Accepted anchor | U (m) | V (m) | Inclusion |
|---|---:|---:|---|
| `W.LANDING` | 0 | 0 | inside; boundary-facing future X6/X7 socket |
| `W.X1` | 50 | -32 | inside |
| `O.X1` | 42 | -64 | inside as small receiving stub only |
| `W.CASCO` | 80 | 35 | inside |
| `W.X5` | 92 | 108 | inside |
| `E.X5` | 90 | 122 | inside as small receiving stub only |
| `W.PLAZA` | 150 | 58 | inside |
| `W.SHOP` | 195 | 70 | inside and boundary-near commercial seam |

These are target blockout anchors. They do not change the CITY-01 edge ledger or turn target distances into measured route lengths.

### 2.6 Explicitly outside

The following accepted anchors remain outside the hard playable seed:

- `W.X2`, `E.X2`, `E.HOME` and the Ensanche neighbourhood anchor;
- `W.CM_NE`, `W.VEGA`, `W.BARRIO`, `W.X3`, `W.X4`;
- `W.RIBERA` and the Ribera workshop/service-yard programme;
- `O.PUERTO_UP`, `O.QUAY`, `O.ENTRADA`;
- X2, X3, X4, X6 and X7 traversal geometry.

They may be suggested in the soft visual envelope where useful, but no outside place/route counts as playable or as current acceptance evidence.

---

## 3. Selected semantic route map

### 3.1 Accepted CITY-01 edges represented

CITY-04 must represent these accepted edge portions without changing access/elevation semantics:

| Edge | From ↔ To | Inherited posture | Seed realization posture |
|---|---|---|---|
| `W04` | `W.SHOP ↔ W.PLAZA` | P / E0 / AR / 0.5 min planning weight | full represented segment; ordinary/commercial street into plaza edge |
| `W05` | `W.PLAZA ↔ W.CASCO` | P/S / E1 / AP / 1.75 min | full represented historic/civic connector |
| `W06` | `W.CASCO ↔ W.LANDING` | S / E2 / AP / 2.25 min | full represented Cuesta/descent to landing seam |
| `W12` | `W.CASCO ↔ W.X1` | S / E0 / AP / 1.0 min | full represented old-bridge approach |
| `W13` | `W.CASCO ↔ W.X5` | Q / E1 / AF / 1.0 min | full represented quiet lower lane |
| `X1` | `W.X1 ↔ O.X1` | EW / AH / 1.0 min | full old stone bridge; pedestrians + handcart/light wheeled, no cart freight |
| `X5` | `W.X5 ↔ E.X5` | EW / AF / 0.4 min | full low-water foot crossing; availability conditional |

No other CITY-01 edge is represented as playable.

### 3.2 Street/junction family binding

| Seed surface | CITY-05 family posture |
|---|---|
| W04 commercial shoulder | `st.ordinary_road`, transitioning to `st.plaza_market_edge` at the civic edge; inherited AR clear-route intent preserved |
| W05 plaza→Casco | `st.historic_lane` + `jn.plaza_gate` at the plaza side |
| W06 Cuesta | `st.historic_lane`; no vehicle capability inferred from visual width |
| W12 old-bridge approach | `st.historic_lane` + `jn.bridgehead` |
| W13 lower quiet lane | `st.historic_lane` + X5 `jn.bridgehead`; remains AF/quiet |
| market edge | `st.plaza_market_edge`; occupiable apron never consumes the always-clear route |
| river-facing short edge | bounded `st.riverside_promenade` presentation where compatible with the represented accepted low-edge relation; no unowned graph edge is created |

### 3.3 Casco micro-branch for follow/search

Inside the `W.CASCO` planning node, CITY-04 must block out **two short public historic-lane traces** that branch and reconnect **within the same node** before reaching the accepted W05/W06/W12/W13 exits.

Purpose: make local follow/search choice perceptible at human scale.

Hard boundary:

- these are intra-node micro-layout traces, **not new CITY-01 graph edges**;
- they cannot be used to claim city-level route redundancy;
- `loc.casco.shared_court` may touch/read from one branch, but its semi-private court is **not** a public through-shortcut;
- closing one micro-branch for a CITY-04 blocked-route test may redirect through the other public branch, never through a service/private/semi-private space.

---

## 4. Selected places, parcels and frontages

### 4.1 A/B/C selected place set

| Place | A–D | S | I | Exterior binding | Seed status |
|---|---:|---:|---:|---|---|
| `loc.casco.bar` | A | S4 | I3 | `bf.bar_social` on `pc.historic_row`/`pc.commercial_row` | retained playable commitment |
| `loc.plaza.ayuntamiento` | A | S3 | I2 | `bf.civic` + `pc.civic_frontage` | retained playable commitment |
| `loc.calle.everyday_shop` | B | S2 | I1 | `bf.shop_service` or compatible `bf.mixed_use_house` on `pc.commercial_row` | retained playable commitment |
| `loc.plaza.market` | B | S2 | I0 | `pc.open_site` + `st.plaza_market_edge` | retained exterior commitment |
| `loc.casco.bridgehead` | B | S1 | I0 | `pc.open_site` + `jn.bridgehead` | retained exterior commitment |
| `loc.casco.shared_court` | C | S3 | I0 | historic fabric/shared-court relation | retained exterior second layer; court remains semi-private |
| selected `fam.casco.houses` | C | S1 | I0 default | `bf.house.old_row` + `pc.historic_row` | retained ordinary closed fabric |
| selected `fam.plaza.arcades` | C | S1 | I0 default | commercial/civic assemblies at plaza edge | retained ordinary fabric |
| selected `fam.calle.mixed_frontages` | C | S1 | I0 default | `bf.mixed_use_house` / ordinary house commercial variants | retained ordinary closed fabric |

No selected C/S1 shell becomes enterable merely because the future asset contains interior geometry.

### 4.2 Eight authored frontage slots

CITY-04 greybox should reserve exactly these frontage **slots**; mesh/module choice remains downstream.

| Slot | Binding | Playable depth promise |
|---|---|---|
| `F01` | `loc.casco.bar` / `bf.bar_social` | I3 hero topology placeholder |
| `F02` | `loc.plaza.ayuntamiento` / `bf.civic` | I2 civic topology placeholder |
| `F03` | `loc.calle.everyday_shop` / `bf.shop_service` | I1 shallow topology placeholder |
| `F04` | ordinary `fam.casco.houses` / `bf.house.old_row` | I0 closed threshold |
| `F05` | ordinary `fam.casco.houses` / `bf.house.old_row` | I0 closed threshold |
| `F06` | `fam.plaza.arcades` ordinary frontage | I0 closed/non-POI frontage |
| `F07` | `fam.calle.mixed_frontages` ordinary mixed-use shell | I0 closed/non-POI frontage |
| `F08` | ordinary old-quarter end/corner house | I0 closed threshold |

The market and bridgehead are open-site compositions and do not consume frontage slots.

---

## 5. Interior/depth/discovery handoff required by CITY-06

### 5.1 `loc.casco.bar`

```text
place_id: loc.casco.bar
interior_depth: I3
interior_family_or_NONE: if.social_house
hero_overlay_or_NONE: hero.bar_layered
required_access_roles: {public, service, semi-private}
selected_zone_topology_posture:
  public common/social
  + distinct service/back relation
  + distinct semi-private secondary layer
  + at least one additional meaningful spatial relation
selected_discovery_opportunity_ids:
  disc.bar.secondary_layer
route_statuses:
  AUTHORED_SPATIAL_NOW — legible public vs service/semi-private thresholds
  FUTURE_OWNER_CONDITIONAL — PA-01/PA-02 actor routine/context
  FUTURE_OWNER_CONDITIONAL — PA-03 relationship/invitation + later access owner
  FUTURE_OWNER_CONDITIONAL — PA-04/PA-05/PA-11 knowledge/rumour/investigation
retained_vs_temporary:
  access-role topology retained; greybox walls/props/materials temporary until later realization
```

No service door becomes ordinary-public to manufacture a second route.

### 5.2 `loc.plaza.ayuntamiento`

```text
place_id: loc.plaza.ayuntamiento
interior_depth: I2
interior_family_or_NONE: if.civic_office
hero_overlay_or_NONE: NONE
required_access_roles: {public, service, private}
selected_zone_topology_posture:
  public civic room
  + distinct staff/service relation
  + private records/work zone
selected_discovery_opportunity_ids:
  disc.ayuntamiento.records_boundary
route_statuses:
  AUTHORED_SPATIAL_NOW — public notice/document surface + legible private institutional layer
  FUTURE_OWNER_CONDITIONAL — PA-12 governance consequence
  FUTURE_OWNER_CONDITIONAL — PA-06/PA-09/PA-12 remembered/player-caused/municipal return-after-change
retained_vs_temporary:
  public/service/private hierarchy retained; document text and runtime policy state remain later-owned
```

### 5.3 `loc.calle.everyday_shop`

```text
place_id: loc.calle.everyday_shop
interior_depth: I1
interior_family_or_NONE: if.retail_shallow
hero_overlay_or_NONE: NONE
required_access_roles: {public, service}
selected_zone_topology_posture:
  one public/use room
  + explicit service threshold or minor support pocket
  + no independent second circulation loop
selected_discovery_opportunity_ids: NONE REQUIRED
route_statuses: NONE REQUIRED
retained_vs_temporary:
  shallow depth + service role retained; stock, opening hours, worker identity and material state remain later-owned
```

The shop is deliberately allowed to remain ordinary. CITY-03 does not turn every selected POI into discovery content.

### 5.4 Exterior-led I0 places

| Place | Interior | Required role | Selected second layer | Discovery/secret requirement |
|---|---|---|---|---|
| `loc.plaza.market` | I0 | `{public}` | ordinary public terrace → contextual market occupation while always-clear route remains open | none required |
| `loc.casco.bridgehead` | I0 | `{public}` | bridgehead/river separation/orientation | none required |
| `loc.casco.shared_court` | I0 | public passage + semi-private court relation | public lane → semi-private court/stair reading | **no secret/incident required** |

### 5.5 Extraordinary/cultural strand

Selected seed posture: **zero primary extraordinary/martial/cinema opportunities**.

This is intentionally neutral and satisfies CITY-06. Future reviewed content may use the reserved grammar, but no acceptance, route, institutional explanation or ordinary-life scenario depends on it.

---

## 6. Retained versus temporary declaration

### 6.1 Structural decisions intended to survive

Retained unless later evidence explicitly reopens CITY-03 or an upstream owner:

- selected seed identity and boundary inclusion/exclusion;
- X1 and X5 as the only crossings physically represented in the seed, with inherited semantics unchanged;
- accepted W04/W05/W06/W12/W13 route relations;
- W.LANDING as future X6/X7 seam, never a dry Puerto continuation;
- selected place IDs and their A/S/I/access obligations;
- frontage-slot roles F01–F08 and the distinction between playable POIs and ordinary closed fabric;
- public/service/private/semi-private topology for selected interiors;
- market always-clear circulation versus occupiable apron;
- quiet W13/shared-court contrast against busier plaza/commercial surfaces;
- expansion seam locations and owner directions described below.

### 6.2 Replaceable presentation / greybox implementation

Not keeper authority merely because CITY-04 creates it:

- primitive mesh dimensions before measurement acceptance;
- placeholder materials, colours, props, signage and lighting;
- proxy actor meshes/markers;
- temporary market stall blocks/crates;
- exact roof/door/window module choice before adopted assets exist;
- soft visual-envelope meshes/silhouettes;
- diagnostic labels/gizmos;
- temporary collision/navmesh settings used only to measure/falsify the paper plan.

Unity existence never promotes temporary presentation into retained semantic authority.

---

## 7. Expansion-seam ledger

Every seam preserves the keeper core. Expansion must extend from these cuts rather than move the selected X1/Casco/Plaza anchors.

| Seam | Direction / future content | Accepted continuation | Hard rule |
|---|---|---|---|
| `seam.commercial_ne` | Calle Mayor → X2 → wider commercial/Vega approach | continue outward from `W.SHOP` through future W03/W02/W.CM_NE fabric | do not relocate W04/W.PLAZA/shop frontage to make later street fit |
| `seam.ensanche_x5` | Ensanche residential bank | continue from `E.X5` toward accepted E04/E.HOME system when later boundary expands | X5 stays low-water-only; future permanent Ensanche routing must come from accepted crossings, not a strengthened X5 fiction |
| `seam.port_landing` | Puerto direct core relation | W.LANDING remains the Wedge-side future X6/X7 socket | no dry continuation; later implementation binds either state-valid X6 or X7 as owned by accepted city state |
| `seam.orilla_x1` | camino sur → Puerto/Entrada from the old bridge | continue outward from `O.X1` via future accepted O01 | X1 remains AH/no cart freight; do not widen semantics locally |
| `seam.upper_future` | later upper/Barrio continuity through accepted outer graph | visual slope/roof continuation only in this seed; eventual playable connection must enter through accepted upstream nodes/edges | no invented direct Casco→Barrio city edge |

Named direction coverage therefore exceeds the WP minimum four while preserving accepted topology.

---

## 8. Soft visual envelope

The hard seed should read as part of a larger valley city without making outside content playable.

Soft visual envelope may include:

- continuation of Calle Mayor roofline beyond `seam.commercial_ne`;
- visible Ensanche roofs/gardens across the Arroyo;
- upper/Barrio roof/slope silhouettes;
- Puerto/Orilla-sur hints downstream/across from W.LANDING, with no dry route;
- green slopes/rock silhouettes beyond the roofline;
- continuation of river/Arroyo surfaces outside the hard polygon.

Rules:

- no outside façade counts toward the eight retained frontage slots;
- no soft-envelope door/interior/path is enterable;
- no outside route time is measured as a full CITY-01 trip;
- no scenic mesh can bridge water or close an expansion seam semantically;
- soft-envelope replacement requires no CITY-03 reopen unless it changes a retained sightline/continuation hypothesis recorded for CITY-04.

---

## 9. CITY-04 exact scenario pack

All scenarios are **spatial validation setups**. Proxy actors, crates, occupancy markers or state labels do not claim that NPC AI, material simulation, governance, memory or discovery runtime already exists.

### `SCN-01 quiet ordinary morning`

Walk W13/X5-side lower Casco and the shared-court edge with market apron empty and minimal proxy occupation.

Measure/observe:

- quiet lane remains useful without incident/clue content;
- shared court reads semi-private rather than as public shortcut;
- X5 crossing availability is legible without making it permanent redundancy;
- quiet-to-busier-core transition is perceptible.

### `SCN-02 market/commercial flow`

Occupy only the bounded market apron with temporary stall/proxy geometry.

Validate:

- W04→plaza→W05 public circulation remains legible and unblocked;
- market occupation does not consume the always-clear `st.plaza_market_edge` route;
- F03 everyday shop reads as a repeated practical destination rather than spectacle.

### `SCN-03 ordinary trip A — domestic threshold → shop → bar`

Spatial proxy only:

`F04 ordinary closed home threshold → public Casco lane → W05/W04 relation → F03 shop public threshold → return toward F01 bar public entrance`.

Purpose: one mundane home/service/social pattern with mixed quiet/public thresholds. No schedule or resident ownership is implemented.

### `SCN-04 ordinary trip B — old bridge → civic/market`

`O.X1 → X1 → W.X1 → W12 → W.CASCO → W05 → W.PLAZA → F02 ayuntamiento public threshold / market edge`.

This trip differs materially from SCN-03 in landmass crossing, route character, civic destination and bridge dependency.

### `SCN-05 follow/search branch A/B`

Run a proxy actor twice inside `W.CASCO`:

- Route A uses public micro-branch A;
- Route B uses public micro-branch B;
- both reconnect inside W.CASCO before an accepted exit.

Validate that a follower must make a visible local choice without a map overlay. The semi-private shared court must **not** become route B and neither micro-branch counts as a new CITY-01 edge.

### `SCN-06 bar/social threshold stack`

At F01, validate blockout readability of:

- public common entrance;
- service/back threshold;
- semi-private secondary relation;
- one additional meaningful relation sufficient to preserve I3 posture.

No invitation, dialogue, schedule or social runtime is implemented.

### `SCN-07 river/bridge traversal`

Run:

`O.X1 → X1 → W.X1 → W12 → W.CASCO → W05 → W.PLAZA/market`.

Validate old-bridge geometry genuinely affects movement/orientation and cannot be bypassed across the river mask.

### `SCN-08 material delivery / service consequence proxy`

Move a temporary delivery proxy along legal public/service approach to F03 and place it at the **service** threshold/support pocket while preserving the public entrance.

Validate:

- `{public, service}` remain distinct and simultaneously realizable at I1;
- delivery geometry does not block the always-clear public route;
- shallow I1 does not inflate into a second deep room.

This is a spatial/material-state proxy only; PA-07/later runtime owns actual work/material dependency.

### `SCN-09 municipal access/routing/service change proxy`

At F02, change temporary access markers so a staff/service or private-records threshold reads unavailable while the public civic room remains reachable.

Validate the spatial hierarchy supports a future municipal access consequence without collapsing public/service/private into one door. No governance logic is implemented; future governance remains PA-12/later runtime-owned.

### `SCN-10 low-stakes player perturbation`

Place/remove a benign obstacle or stall cluster inside the **occupiable market apron**, never the always-clear route.

Validate the player can alter local presentation/use without destroying public circulation or forcing a new graph edge.

### `SCN-11 leave and return to changed local state proxy`

Observe F03 service/public arrangement, leave the immediate area, manually change a visible delivery/stock proxy, then return.

Validate that the authored spatial substrate can display a changed material state without requiring a different room/route. This is **not** save/memory proof; PA-06/PA-07/PA-09 and later runtime owners remain responsible for causal persistence.

### `SCN-12 multi-route-ready discovery substrate`

At F01 and/or F02:

1. verify the `AUTHORED_SPATIAL_NOW` substrate is legible through ordinary spatial observation;
2. annotate at least one separate threshold/surface where a named future conditional route could truthfully attach;
3. record the future owner label rather than scripting the route.

Required example:

- `disc.bar.secondary_layer`: authored threshold distinction + future PA-01/02 or PA-03 route; **or**
- `disc.ayuntamiento.records_boundary`: authored public/private institutional distinction + future PA-12 route.

Passing this scenario proves substrate readiness only, never runtime discovery.

### `SCN-13 changing/blocked route + alternate local path`

Temporarily block public Casco micro-branch A with a greybox barrier and use public micro-branch B.

Validate:

- alternate choice is perceptible;
- the semi-private shared court is not used as the fallback;
- no service/private threshold becomes a public bypass;
- city-level graph semantics are unchanged because both traces remain inside W.CASCO.

Separately toggle X5 presentation unavailable to confirm the seed does not invent a substitute stream crossing.

---

## 10. CITY-04 measurement pack

### 10.1 Segment hypotheses represented in full

CITY-04 should measure these represented planning weights for ordinary pedestrian traversal and record actual geometry/conditions:

| Segment | CITY-01 planning weight | CITY-04 duty |
|---|---:|---|
| W04 `W.SHOP ↔ W.PLAZA` | 0.5 min | measure represented segment |
| W05 `W.PLAZA ↔ W.CASCO` | 1.75 min | measure represented segment |
| W06 `W.CASCO ↔ W.LANDING` | 2.25 min | measure represented segment |
| W12 `W.CASCO ↔ W.X1` | 1.0 min | measure represented segment |
| W13 `W.CASCO ↔ W.X5` | 1.0 min | measure represented segment |
| X1 `W.X1 ↔ O.X1` | 1.0 min | measure bridge traversal and AH physical readability |
| X5 `W.X5 ↔ E.X5` | 0.4 min | measure only in available/low-water test state; preserve AF posture |

### 10.2 Seed route hypotheses

Useful concatenated planning expectations:

- `O.X1 → X1 → W.X1 → W.CASCO → W.PLAZA` = **3.75 min** planning sum;
- `W.SHOP → W.PLAZA → W.CASCO → W.LANDING` = **4.50 min** planning sum;
- `W.CASCO → W.X5 → E.X5` = **1.40 min** when X5 is available;
- `W.PLAZA → W.CASCO` remains the inherited **1.75 min** target relation.

CITY-04 must not relabel absent full-city routes as measured by extrapolating these segments.

### 10.3 Physical/access checks

Record at minimum:

- W04 realized clear width and AR plausibility;
- historic-lane clear widths for W05/W06/W12/W13;
- actual level change/grade posture, especially W06 E2 and W13 E1;
- X1 bridge width, approach separation and AH plausibility without cart-freight assumption;
- X5 foot-only readability and closure state;
- market apron versus always-clear route dimensions;
- F01/F02/F03 role-bearing threshold separations;
- shared-court public/semi-private boundary readability;
- no-build/water separation and absence of accidental extra crossings.

### 10.4 Sightline/orientation hypotheses

Test, do not assume:

1. from X1/O.X1 approach, the player should read that the old bridge enters the old-quarter core rather than a generic road grid;
2. from W.PLAZA, Casco continuation and commercial continuation should both read without forcing all movement through one central point;
3. from W.LANDING, Puerto continuation should read **across/downstream** as a future connection, never as dry Wedge ground;
4. from `seam.commercial_ne`, roof/street continuation should imply more town beyond the hard seed;
5. quiet W13/shared-court fabric should remain visually legible without becoming empty/dead space.

Material failure routes back to the relevant owner instead of being patched by moving macro anchors ad hoc.

---

## 11. CITY-08 representative Arkus authoring-proof slice

Reserve later trial slice **`trial.city08.civic_commercial_corner`** inside the retained seed:

- one bounded portion of W04 as it meets the plaza edge;
- frontage F03 (`loc.calle.everyday_shop`, I1) with required public + service anchors;
- frontage F07 ordinary closed mixed frontage;
- a bounded market-edge/open-site piece preserving the always-clear route.

Why this slice:

- it uses reusable, non-hero families rather than hiding authoring cost inside the bespoke bar;
- it requires street + parcel + functional POI + ordinary closed frontage + open-site composition;
- it demonstrates that one reviewed family can remain ordinary while another gains functional binding/access requirements;
- it is small enough for a public fresh-agent Arkus authoring proof but materially representative of retained city production.

CITY-08 owns the actual public authoring/reuse proof after its prerequisites. CITY-03 only reserves the retained slice.

---

## 12. Acceptance checks against WP-CITY-03

- **≥3 genuine alternatives:** yes; comparison evidence contains compact, civic-commercial and work-edge candidates.
- **0.03–0.06 km²:** selected hard polygon = 0.0448175 km² gross; conservative net interpretation also remains inside band.
- **route/place/depth variety:** X1 river crossing, X5 Arroyo crossing, AR/AP/AF/AH surfaces, busy/quiet contrast, A/B/C and S1/S2/S3/S4, I0/I1/I2/I3 all represented.
- **at least two everyday actor trips differ materially:** SCN-03 and SCN-04 differ by origins, access/crossing, route character and destinations.
- **≥4 expansion directions without moving keeper core:** commercial/Vega, Ensanche, Puerto landing, Orilla-sur/Entrada, plus upper future continuation.
- **hero selectivity:** only `loc.casco.bar` is I3; no new hero.
- **LOCAL Worker can build without choosing another city slice:** hard polygon, anchors, included edges/places/frontages, scenario list and measurements are enumerated.

---

## 13. Fail/reopen routing

CITY-04 must report, not silently improvise, if physical evidence shows:

- **CITY-03 deviation:** selected hard boundary cannot realize the declared retained subset or an expansion seam requires moving keeper core anchors;
- **CITY-01 issue:** accepted edge/access/elevation posture is physically implausible in represented geometry or route choice fails because the accepted mobility model, not local dressing, is wrong;
- **CITY-02 issue:** selected place/depth programme cannot fit or ordinary/quiet scope must be consumed to make A/B commitments work;
- **CITY-05 issue:** accepted parcel/building/access-role mapping cannot realize required thresholds/clearance;
- **CITY-06 issue:** accepted interior-depth/access/discovery substrate cannot fit the selected shell without weakening roles or inflating depth;
- **CITY-00 issue:** only if physical evidence actually contradicts accepted geography/seed-scale facts — stop and raise explicit predecessor amendment; CITY-04 cannot reinterpret CITY-00 locally.

No asset convenience, prefab interior, attractive shortcut or runtime idea authorizes an unreviewed semantic change.

---

## 14. CITY-03 closure claim

CITY-03 selects one bounded first keeper piece of the accepted city, fixes what is retained versus temporary, preserves all inherited crossing/access/depth/discovery semantics, leaves multiple clean expansion seams, and gives CITY-04 a concrete instruction:

> **Build exactly this bounded slice at greybox fidelity, measure the listed hypotheses, run the listed spatial scenarios, and report owner-tagged deviations. Do not select a different city and do not implement future Living World semantics to make the blockout look successful.**
