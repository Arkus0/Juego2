# Keeper City — Retained product seed + exact local-validation specification

Version: 0.2 — 2026-09-21  
Workpack: `WP-CITY-03 — Retained product seed + exact scenario specification`  
Class: **PRODUCT / KEEPER-SEED PREPRODUCTION — NON-FOUNDATIONAL**

Status: **CITY-03 candidate semantic owner.** This document selects the exact retained product seed and freezes the bounded semantic handoff to CITY-04. It consumes accepted CITY-00 geography, CITY-01 mobility, CITY-02 place/depth programme, CITY-05 exterior grammar and CITY-06 interior/discovery grammar without changing them.

It does **not** create Unity geometry, import assets, measure traversal, author runtime Living World/gameplay semantics, change accepted crossings/routes, promote ordinary I0 content, or prove future conditional discovery routes.

Seed comparison evidence: `Docs/evidence/WP-CITY-03/SEED_COMPARISON.md`.

---

## 1. Selection result

Selected seed: **`seed.confluence_civic_commercial`**.

The selected seed is the first keeper piece of the shipping city, not a tutorial island or disposable harness. It carries:

- accepted confluence-tip/Casco identity;
- one civic/market edge;
- old bridge X1 as real traversable structure;
- one Arroyo crossing, X5, preserving its low-water-only status;
- the bar hero commitment;
- one deep civic interior;
- one shallow everyday commercial interior;
- ordinary closed residential/commercial frontages;
- a short river edge and the Cuesta continuation to the future X6/X7 landing socket;
- bounded public branching for follow/search validation;
- clean seams toward Calle Mayor/Vega, Ensanche, Puerto and Orilla-sur/Entrada.

No upstream obligation was weakened to make this candidate win.

---

## 2. Exact boundary semantics

### 2.1 Exactness rule

CITY-03 freezes:

1. which accepted planning anchors/edge portions are inside or outside;
2. which crossings are physically represented;
3. which place IDs, site regions, frontage slots and exterior/interior families are committed;
4. which boundary cuts are expansion seams;
5. which water/no-build regions remain non-traversable;
6. the seed-local planning polygon used as CITY-04's initial blockout target.

Coordinates are **planning geometry**, not measured survey truth. CITY-04 may falsify metric bends, grades, widths or distances, but must report material deviation rather than silently choosing another seed or topology.

### 2.2 Seed-local planning frame

Use a local 2D planning frame in metres:

- origin `(0,0)` = accepted `W.LANDING` planning anchor;
- `+U` = generally upstream along the Wedge toward Casco → Plaza → Calle Mayor;
- `+V` = generally transverse from the Río-facing low side toward the Arroyo / Ensanche side.

This frame is only a construction handoff coordinate system; it does not redefine cardinal geography.

### 2.3 Hard outer polygon

The hard seed boundary is the closed polygon below, in order:

| Vertex | U | V | Semantic role |
|---|---:|---:|---|
| `B01` | -20 | -60 | confluence/river-facing lower seam |
| `B02` | 70 | -80 | X1 far-head / Orilla-sur receiving-stub envelope |
| `B03` | 205 | -48 | lower civic/commercial edge |
| `B04` | 245 | 22 | Calle Mayor commercial expansion cut |
| `B05` | 220 | 95 | upper commercial/Casco fabric cut |
| `B06` | 150 | 145 | Arroyo-side / future upper-residential continuation cut |
| `B07` | 70 | 132 | X5 Ensanche-side receiving-stub envelope |
| `B08` | -20 | 55 | landing-side/confluence visual seam |

Shoelace area of the **hard planning envelope**: **44,817.5 m² = 0.0448175 km²**.

That is the CITY-03 band check and is inside CITY-00's accepted `0.03–0.06 km²` retained-seed band. Internal water/no-build masks remain non-traversable but are part of the bounded spatial envelope; CITY-03 does not invent a second, more precise “net land” acceptance metric that CITY-00 never defined.

### 2.4 Water and no-build masks

The outer polygon never makes water traversable by inclusion:

- `mask.rio` — accepted Río separation around X1/confluence. **Only X1** is traversable across the Río inside this seed. `W.LANDING` is a seam toward future X6/X7; no dry Wedge→Puerto continuation exists.
- `mask.arroyo` — accepted Arroyo separation at lower Casco. **Only X5** is traversable across the Arroyo inside this seed, and only when its inherited low-water availability condition is active.

CITY-04 may shape banks/retaining geometry but may not create an extra crossing, hidden ford, walkable water mesh or private-court bridge.

### 2.5 Selected anchor targets

| Accepted anchor | U | V | Inclusion |
|---|---:|---:|---|
| `W.LANDING` | 0 | 0 | inside; boundary-facing future X6/X7 socket |
| `W.X1` | 50 | -32 | inside |
| `O.X1` | 42 | -64 | inside as small receiving stub only |
| `W.CASCO` | 80 | 35 | inside |
| `W.X5` | 92 | 108 | inside |
| `E.X5` | 90 | 122 | inside as small receiving stub only |
| `W.PLAZA` | 150 | 58 | inside |
| `W.SHOP` | 195 | 70 | inside and boundary-near commercial seam |

These target anchors do not alter the CITY-01 edge ledger or turn target distances into measurements.

### 2.6 Explicitly outside

Outside the hard playable seed:

- `W.X2`, `E.X2`, `E.HOME` and the Ensanche neighbourhood anchor;
- `W.CM_NE`, `W.VEGA`, `W.BARRIO`, `W.X3`, `W.X4`;
- `W.RIBERA` and Ribera workshop/service-yard programme;
- `O.PUERTO_UP`, `O.QUAY`, `O.ENTRADA`;
- X2, X3, X4, X6 and X7 traversal geometry.

Outside content may be suggested in the soft visual envelope but never counts as playable or current acceptance evidence.

---

## 3. Selected semantic route map

### 3.1 Accepted CITY-01 edges represented

| Edge | From ↔ To | Inherited posture | Seed posture |
|---|---|---|---|
| `W04` | `W.SHOP ↔ W.PLAZA` | P / E0 / AR / 0.5 min | full represented ordinary/commercial segment |
| `W05` | `W.PLAZA ↔ W.CASCO` | P/S / E1 / AP / 1.75 min | full represented historic/civic connector |
| `W06` | `W.CASCO ↔ W.LANDING` | S / E2 / AP / 2.25 min | full Cuesta/descent to landing seam |
| `W12` | `W.CASCO ↔ W.X1` | S / E0 / AP / 1.0 min | full old-bridge approach |
| `W13` | `W.CASCO ↔ W.X5` | Q / E1 / AF / 1.0 min | full quiet lower lane |
| `X1` | `W.X1 ↔ O.X1` | EW / AH / 1.0 min | full old stone bridge; no cart freight |
| `X5` | `W.X5 ↔ E.X5` | EW / AF / 0.4 min | full low-water foot crossing; availability conditional |

No other CITY-01 edge is represented as playable.

### 3.2 Street/junction binding

| Seed surface | CITY-05 posture |
|---|---|
| W04 commercial shoulder | `st.ordinary_road`, transitioning to `st.plaza_market_edge`; inherited AR clearance intent preserved |
| W05 plaza→Casco | `st.historic_lane` + `jn.plaza_gate` at plaza side |
| W06 Cuesta | `st.historic_lane`; no vehicle capability inferred |
| W12 old-bridge approach | `st.historic_lane` + `jn.bridgehead` |
| W13 lower quiet lane | `st.historic_lane` + X5 `jn.bridgehead`; AF/quiet preserved |
| market edge | `st.plaza_market_edge`; occupiable apron never consumes always-clear route |
| short river-facing edge | bounded riverside presentation only where compatible; it creates no unowned graph edge |

### 3.3 Bounded Casco micro-loop for follow/search

Inside the `W.CASCO` planning node, CITY-04 must block out two short public historic-lane traces, `casco.micro.A` and `casco.micro.B`, which split and reconnect **inside the same CITY-01 node** before reaching accepted exits W05/W06/W12/W13.

Rules:

- both traces are node-local realization detail, not new CITY-01 edges;
- they cannot prove city-level route redundancy;
- `loc.casco.shared_court` may be visible/touch one trace but its semi-private court is not a public through-route;
- closing micro.A may redirect to micro.B, never through service/private/semi-private space.

This is the seed's small local route loop. It provides a genuine follow/search choice without modifying the accepted city graph.

---

## 4. Selected places, site regions and frontages

### 4.1 Selected place/depth set

| Place | A–D | S | I | Exterior binding | Seed status |
|---|---:|---:|---:|---|---|
| `loc.casco.bar` | A | S4 | I3 | `bf.bar_social` on `pc.historic_row`/`pc.commercial_row` | retained playable |
| `loc.plaza.ayuntamiento` | A | S3 | I2 | `bf.civic` + `pc.civic_frontage` | retained playable |
| `loc.calle.everyday_shop` | B | S2 | I1 | `bf.shop_service` or compatible `bf.mixed_use_house` on `pc.commercial_row` | retained playable |
| `loc.plaza.market` | B | S2 | I0 | `pc.open_site` + `st.plaza_market_edge` | retained exterior |
| `loc.casco.bridgehead` | B | S1 | I0 | `pc.open_site` + `jn.bridgehead` | retained exterior |
| `loc.casco.shared_court` | C | S3 | I0 | historic fabric/shared-court relation | retained exterior second layer |
| selected `fam.casco.houses` | C | S1 | I0 default | `bf.house` old-row variant + `pc.historic_row` | retained ordinary closed fabric |
| selected `fam.plaza.arcades` | C | S1 | I0 default | commercial/civic assemblies at plaza edge | retained ordinary fabric |
| selected `fam.calle.mixed_frontages` | C | S1 | I0 default | `bf.mixed_use_house` / ordinary commercial variant | retained ordinary closed fabric |

Hard playable classification coverage is A/B/C and S1/S2/S3/S4. D/S0 scenic context is deliberately kept in the **soft visual envelope**, not promoted into playable seed area. This preserves the A–D/S independence rather than forcing scenic context to become a hard playable parcel.

### 4.2 Exact bounded frontage/site regions

The polygons below are CITY-03 **placement regions**, in seed-local metres. CITY-04 may shape a legal parcel/shell inside each region but may not move the functional slot to another street or exchange slot identities. Every final route-facing threshold must remain outside the inherited clear-route polygon and preserve CITY-05 frontage/service rules.

| ID | Region polygon `(U,V)` | Binding / planning size | Required facing/relation |
|---|---|---|---|
| `F01` | `(88,45),(96,45),(96,60),(88,60)` | bar, ~8×15 m, historic/commercial | public face to Casco public trace; distinct service + semi-private relations retained |
| `F02` | `(148,66),(166,66),(166,88),(148,88)` | ayuntamiento, ~18×22 m civic | public face to plaza; service/staff and private records relations distinct |
| `F03` | `(188,76),(198,76),(198,94),(188,94)` | everyday shop, ~10×18 m commercial | public face to W04/commercial shoulder; distinct service threshold/pocket |
| `F04` | `(62,40),(69,40),(69,54),(62,54)` | ordinary old-row home, ~7×14 m | private/closed residential threshold; no interior promise |
| `F05` | `(98,24),(105,24),(105,36),(98,36)` | ordinary old-row home, ~7×12 m | private/closed threshold; no public shortcut |
| `F06` | `(126,62),(138,62),(138,78),(126,78)` | ordinary plaza-edge frontage, ~12×16 m | plaza/arcade read; I0 closed/non-POI |
| `F07` | `(170,74),(178,74),(178,90),(170,90)` | ordinary mixed frontage, ~8×16 m | W04 commercial continuity; I0 closed/non-POI |
| `F08` | `(50,3),(57,3),(57,16),(50,16)` | old-quarter end/corner house, ~7×13 m | closed threshold framing Cuesta/landing approach |
| `S01` | `(132,35),(150,40),(148,52),(130,47)` | market occupiable open-site region | public apron only; always-clear route remains separate |
| `S02` | `(42,-40),(58,-40),(58,-22),(42,-22)` | X1 Wedge bridgehead open-site region | public bridgehead; no parallel crossing |
| `S03` | `(70,55),(83,55),(83,68),(70,68)` | shared-court region | public passage edge + semi-private court; never public through-shortcut |

All listed region vertices lie inside the hard outer polygon. The regions are deliberately bounded tightly enough that CITY-04 chooses **geometry**, not a different parcel/location plan.

### 4.3 Eight authored frontage slots

`F01–F08` are the eight retained frontage slots. Only F01–F03 carry enclosed playable depth. F04–F08 remain I0/closed unless a future explicitly reviewed owner promotes them. `S01–S03` are open/exterior site regions and do not consume frontage count.

---

## 5. Interior/depth/discovery handoff

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
selected_discovery_opportunity_ids: {disc.bar.secondary_layer}
route_statuses:
  AUTHORED_SPATIAL_NOW — legible public vs service/semi-private thresholds
  FUTURE_OWNER_CONDITIONAL — PA-01/PA-02 actor routine/context
  FUTURE_OWNER_CONDITIONAL — PA-03 relationship/invitation + later access owner
  FUTURE_OWNER_CONDITIONAL — PA-04/PA-05/PA-11 knowledge/rumour/investigation
retained_vs_temporary:
  access-role topology retained; greybox walls/props/materials temporary
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
selected_discovery_opportunity_ids: {disc.ayuntamiento.records_boundary}
route_statuses:
  AUTHORED_SPATIAL_NOW — public notice/document surface + legible private institutional layer
  FUTURE_OWNER_CONDITIONAL — PA-12 governance consequence
  FUTURE_OWNER_CONDITIONAL — PA-06/PA-09/PA-12 return-after-change context
retained_vs_temporary:
  access hierarchy retained; document text and runtime policy state later-owned
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
  shallow depth + service role retained; stock/opening hours/worker/material state later-owned
```

The shop may remain ordinary; CITY-03 does not turn every selected POI into discovery content.

### 5.4 Exterior-led I0 places

| Place | Required role/posture | Selected second layer | Discovery/secret requirement |
|---|---|---|---|
| `loc.plaza.market` | public | ordinary terrace → contextual market occupation while route remains open | none |
| `loc.casco.bridgehead` | public | bridgehead/river separation/orientation | none |
| `loc.casco.shared_court` | public passage + semi-private court | lane → semi-private court/stair read | **no secret/incident required** |

### 5.5 Extraordinary/cultural strand

Selected seed posture: **zero primary extraordinary/martial/cinema opportunities**. Zero is neutral under CITY-06. No acceptance, route or ordinary-life scenario depends on that optional strand.

---

## 6. Retained versus temporary declaration

### Retained structural decisions

Retained unless later evidence explicitly reopens the correct owner:

- selected seed identity and inclusion/exclusion boundary;
- X1 and X5 as the only crossings physically represented, with inherited semantics unchanged;
- W04/W05/W06/W12/W13 route relations;
- `W.LANDING` as future X6/X7 seam, never dry Puerto continuation;
- selected place IDs, site regions F01–F08/S01–S03 and their A/S/I/access obligations;
- public/service/private/semi-private topology for selected interiors;
- market always-clear circulation versus occupiable apron;
- quiet W13/shared-court contrast against busier plaza/commercial surfaces;
- expansion seams below.

### Replaceable presentation / greybox implementation

Not keeper authority merely because Unity creates it:

- primitive mesh dimensions before measured acceptance;
- placeholder materials, props, signs, lighting and proxy actors;
- temporary market stall/crate geometry;
- exact adopted roof/door/window asset choice;
- soft visual-envelope meshes;
- diagnostics/gizmos;
- temporary collision/navmesh settings used only for measurement.

Unity existence never promotes temporary presentation into semantic authority.

---

## 7. Expansion-seam ledger

| Seam | Future content | Accepted continuation | Hard rule |
|---|---|---|---|
| `seam.commercial_ne` | Calle Mayor → X2 → wider commercial/Vega approach | outward from `W.SHOP` through future W03/W02/W.CM_NE | do not relocate W04/W.PLAZA/F03 to make later street fit |
| `seam.ensanche_x5` | Ensanche bank | from `E.X5` toward future E04/E.HOME system | X5 stays low-water-only; permanent routing must use accepted crossings |
| `seam.port_landing` | direct core→Puerto relation | `W.LANDING` remains future X6/X7 Wedge socket | no dry continuation; later state binds valid X6 **or** X7 |
| `seam.orilla_x1` | camino sur → Puerto/Entrada | outward from `O.X1` through future O01 | X1 remains AH/no cart freight |
| `seam.upper_future` | later upper/Barrio continuity | visual upper continuation now; future playable joins only through accepted upstream graph | no invented direct Casco→Barrio city edge |

Five named seam directions exceed the required four without moving retained core streets/bridges/anchors.

---

## 8. Soft visual envelope

May include non-playable continuation of Calle Mayor roofline, Ensanche roofs/gardens across Arroyo, upper/Barrio roof/slope silhouettes, Puerto/Orilla-sur hints across/downstream from W.LANDING, green slopes/rock silhouettes and water outside the hard polygon.

Rules:

- outside façades do not count toward F01–F08;
- outside doors/interiors/paths are not enterable;
- no outside route time becomes a measured full CITY-01 trip;
- no scenic mesh bridges water or closes a seam semantically;
- D/S0 scenic context stays soft/non-playable.

---

## 9. CITY-04 exact scenario pack

All scenarios are **spatial validation setups**. Proxy actors, crates, occupancy markers or state labels do not claim NPC AI, material simulation, governance, memory or discovery runtime.

### `SCN-01 quiet ordinary morning`

Walk W13/X5-side lower Casco and S03 shared-court edge with market apron empty. Validate quiet fabric remains useful without incident/clue content, S03 reads semi-private rather than shortcut, X5 reads conditional, and quiet→busy transition is perceptible.

### `SCN-02 market/commercial flow`

Occupy only S01 with temporary stall/proxy geometry. W04→plaza→W05 circulation must remain legible/unblocked and F03 should read as an everyday service destination rather than spectacle.

### `SCN-03 ordinary home → work/service → social trip`

Spatial proxy route:

`F04 closed home threshold → public Casco trace → F03 everyday-shop public/service workplace threshold → F01 bar public entrance`.

This satisfies the home/work/social trip **spatial** pattern without claiming a schedule, job assignment or opening-hours runtime. F04 remains I0 and closed; the actor proxy begins/ends at the threshold only.

### `SCN-04 materially different ordinary trip — old bridge → civic/market`

`O.X1 → X1 → W.X1 → W12 → W.CASCO → W05 → W.PLAZA → F02/S01`.

It differs from SCN-03 by landmass crossing, route character, destination and bridge dependency.

### `SCN-05 follow/search route A/B`

Run a proxy actor through `casco.micro.A` then `casco.micro.B`. Both reconnect within W.CASCO. A follower should perceive a local choice without map overlay. S03 must not become route B.

### `SCN-06 bar/social threshold stack`

At F01, validate public common entrance, distinct service/back threshold, semi-private secondary relation and one additional meaningful relation sufficient to preserve I3 posture. No invitation/dialogue/social runtime is implemented.

### `SCN-07 river/bridge traversal`

Run `O.X1 → X1 → W.X1 → W12 → W.CASCO → W05 → W.PLAZA/S01`. X1 must genuinely affect movement/orientation and cannot be bypassed across `mask.rio`.

### `SCN-08 material delivery/work consequence proxy`

Move a temporary delivery proxy legally to F03's **service** threshold/support pocket, then alter a visible stock/delivery proxy while keeping the public entrance distinct. Validate `{public,service}` coexist at I1, delivery does not block public route and I1 does not inflate to I2. Actual work/material causality remains PA-07/later-runtime owned.

### `SCN-09 municipal access/service consequence proxy`

At F02, change temporary access markers so staff/service or private-records access reads unavailable while the public civic room remains reachable. Validate future municipal consequences can use the spatial hierarchy without collapsing roles. No governance logic is implemented; PA-12/later runtime owns it.

### `SCN-10 low-stakes player perturbation`

Place/remove a benign obstacle or stall cluster **inside S01 occupiable apron**, never the always-clear route. Local use may change without destroying circulation or creating a new graph edge.

### `SCN-11 leave and return to changed local state proxy`

Observe F03, leave the immediate area, manually change the delivery/stock proxy, return, and verify the authored substrate can display the changed state without a different room/route. This is not save/memory proof.

### `SCN-12 multi-route-ready discovery substrate`

At F01 or F02, verify one `AUTHORED_SPATIAL_NOW` substrate and annotate a separate future conditional route with its named owner. Required examples are `disc.bar.secondary_layer` or `disc.ayuntamiento.records_boundary`. Passing proves substrate readiness only.

### `SCN-13 changing/blocked route + alternate local path`

Block `casco.micro.A` temporarily and use `casco.micro.B`. The shared court and service/private thresholds must not become fallback. Separately toggle X5 unavailable and confirm no substitute Arroyo crossing is invented.

---

## 10. CITY-04 measurement pack

### 10.1 Full represented segment hypotheses

| Segment | CITY-01 planning weight | CITY-04 duty |
|---|---:|---|
| W04 | 0.5 min | measure represented segment |
| W05 | 1.75 min | measure represented segment |
| W06 | 2.25 min | measure represented segment |
| W12 | 1.0 min | measure represented segment |
| W13 | 1.0 min | measure represented segment |
| X1 | 1.0 min | measure bridge traversal and AH physical readability |
| X5 | 0.4 min | measure only in available/low-water state; preserve AF |

Useful concatenated planning expectations:

- `O.X1 → X1 → W.X1 → W.CASCO → W.PLAZA` = **3.75 min**;
- `W.SHOP → W.PLAZA → W.CASCO → W.LANDING` = **4.50 min**;
- `W.CASCO → W.X5 → E.X5` = **1.40 min** when X5 is available.

Absent full-city routes must not be relabelled as measured by extrapolation.

### 10.2 Physical/access checks

Record at minimum:

- W04 realized clear width and AR plausibility;
- historic-lane clear widths on W05/W06/W12/W13;
- actual level change/grade posture, especially W06 E2 and W13 E1;
- X1 bridge width/approach and AH plausibility without cart-freight assumption;
- X5 foot-only readability and closure state;
- S01 apron versus always-clear route dimensions;
- F01/F02/F03 role-bearing threshold separation;
- S03 public/semi-private boundary readability;
- absence of accidental extra crossings;
- whether all F01–F08/S01–S03 regions fit their accepted family/route constraints without moving the selected anchors.

### 10.3 Sightline/orientation hypotheses

Test, do not assume:

1. X1/O.X1 approach reads as entrance to old-quarter core rather than generic road grid;
2. from W.PLAZA, Casco and commercial continuations both read without making plaza the only connector;
3. from W.LANDING, Puerto reads across/downstream as a future connection, never dry Wedge ground;
4. `seam.commercial_ne` implies more town beyond the hard seed;
5. quiet W13/S03 remains legible without reading as dead space.

---

## 11. CITY-08 representative Arkus authoring-proof slice

Reserve later trial slice **`trial.city08.civic_commercial_corner`**:

- bounded W04/plaza-edge portion;
- F03 everyday shop (I1, public + service);
- F07 ordinary closed mixed frontage;
- S01 market-edge/open-site piece preserving the always-clear route.

It is intentionally non-hero: street + bounded parcel + functional POI + ordinary closed frontage + open-site composition demonstrate reuse more honestly than the bespoke bar. CITY-08 owns the actual proof after its prerequisites.

---

## 12. Acceptance checks

- **≥3 genuine alternatives:** yes; compact, civic-commercial and work-edge candidates compared before selection.
- **0.03–0.06 km²:** hard planning envelope = 0.0448175 km².
- **route/place/depth variety:** X1/X5, AR/AP/AF/AH, busy/quiet, A/B/C hard-playable plus D scenic envelope, S0–S4 across hard+soft representation, I0/I1/I2/I3.
- **two materially different everyday trips:** SCN-03 and SCN-04.
- **≥4 expansion directions:** five named seams.
- **hero selectivity:** bar only.
- **LOCAL build without choosing another slice:** polygon, anchors, route edges, site regions, place/depth/access bindings, scenarios and measurements are enumerated.

---

## 13. Fail/reopen routing

CITY-04 must report rather than improvise if physical evidence shows:

- **CITY-03:** selected hard boundary/site regions cannot realize the declared subset or a seam requires moving keeper core anchors;
- **CITY-01:** accepted edge/access/elevation posture is physically implausible or route-choice failure comes from the accepted mobility model;
- **CITY-02:** selected place/depth programme cannot fit or ordinary/quiet scope must be consumed for A/B commitments;
- **CITY-05:** accepted parcel/building/access-role mapping cannot realize required thresholds/clearance;
- **CITY-06:** accepted interior-depth/access/discovery substrate cannot fit without weakening roles or inflating depth;
- **CITY-00:** only if physical evidence actually contradicts accepted geography/seed-scale facts — stop and raise explicit predecessor amendment.

Asset convenience, prefab interiors, attractive shortcuts or runtime ideas do not authorize semantic drift.

---

## 14. CITY-03 closure claim

CITY-03 selects one bounded first keeper piece of the accepted city, freezes exact site-placement regions and retained/temporary distinctions, preserves inherited crossing/access/depth/discovery semantics, leaves clean expansion seams, and gives CITY-04 one instruction:

> **Build exactly this bounded slice at greybox fidelity, measure the listed hypotheses, run the listed spatial scenarios, and report owner-tagged deviations. Do not select a different city and do not implement future Living World semantics to make the blockout look successful.**
