# Keeper City — Systemic location, spatial-depth and interior programme

Version: 1.0 — 2026-09-21  
Workpack: `WP-CITY-02 — Systemic locations + spatial-depth/interior programme`  
Class: **PRODUCT / PLACE & INTERIOR PREPRODUCTION — NON-FOUNDATIONAL**

Status: **CITY-02 candidate semantic owner.** This document owns the Keeper City's programmed place families, provisional `A..D` systemic importance, `S0..S4` spatial-production depth, A/B use profiles, enclosed-interior priorities and reactive-density measurement vocabulary. It consumes the accepted CITY-00 geography and CITY-01 movement/access graph without changing either.

It does **not** compose parcels/building shells (`CITY-05`), design detailed interiors/discovery (`CITY-06`), choose the retained seed (`CITY-03`), construct Unity geometry (`CITY-04`), or author runtime schedules, beliefs, dialogue, relationships, decisions or incidents.

Planning IDs in this document are CITY programme handles only. They are not frozen runtime object IDs, parcel IDs or canonical Arkus IDs.

---

## 1. Binding inputs and non-negotiable boundaries

CITY-02 consumes:

- `Docs/production/CITY_SPATIAL_CONSTITUTION.md` — Wedge / Ensanche bank / Orilla-sur geography, district families, crossing truth, scale envelope, quiet-fabric invariants and physical municipal addresses;
- `Docs/production/CITY_MOBILITY_TOPOLOGY.md` — authoritative movement/access graph, route character, access/elevation classes, planning travel costs and closure/alternate-route semantics;
- `Docs/art/SETTING.md` and `Docs/art/VISUAL_BIBLE.md` — fictional Liébana/Potes valley identity and visual exclusions within ART authority;
- `Docs/production/PRODUCTION_BLUEPRINT.md` only as non-binding production input where it agrees with the accepted owners above.

Hard rules for this programme:

1. No new water crossing or dry Wedge→Puerto / Ensanche→Orilla-sur connection is created.
2. No programme row may silently turn an `AF`, `AH`, `AS` or `AX6` route into ordinary road-capable access.
3. CITY-01 planning costs remain hypotheses; CITY-02 never relabels them as measured travel.
4. Quiet routes remain useful without becoming incident funnels.
5. A location may matter systemically without being spatially deep, and may be spatially deep without being systemically important.
6. A place's ability to matter while the player is absent is a **spatial/programme requirement**, not a claim that runtime simulation already exists.

---

## 2. Place/access vocabulary

This is the complete CITY-02 vocabulary handed to CITY-05/06. Later work may add implementation detail but should not redefine these words casually.

- **Place** — a bounded planning unit with a coherent use/access identity. A place may later span one parcel, several shells or an exterior space; it is not automatically one runtime object.
- **Frontage** — the public- or route-facing edge through which a place reads and is ordinarily approached. A place may have more than one frontage.
- **Shell** — the enclosing exterior/building volume promised by the programme. Shell composition belongs to CITY-05.
- **Threshold** — a meaningful transition between access conditions or spatial layers: street→shop, public room→staff area, lane→shared court, quay→office, etc. Detailed threshold design belongs to CITY-06.
- **Public** — ordinary access may be expected when the later runtime says the place is available; this is not an opening-hours definition.
- **Semi-private** — access is socially/contextually conditional rather than universally public. The later causal owner decides invitations, permissions and temporal rules.
- **Private** — not ordinary public circulation. CITY-02 may require that the space exists without specifying how or whether the player gains access.
- **Service access** — back-of-house/material access tied to an accepted CITY-01 route/access class. It never promotes a restricted `AS` path into an ordinary public route.
- **Vertical relation** — a programmed upstairs/downstairs/terrace/stair relationship whose detailed geometry remains downstream.
- **Interior anchor** — a spatially meaningful interior use point (counter, desk, workbench, table, storage bay, etc.) that later composition can support. It is not a smart-object/runtime contract.
- **Scenic envelope** — visual context outside the promised playable fabric. Scenic envelope can be important to composition but does not count as playable area or reactive-density success.

### 2.1 Enclosed-interior priority

Enclosed-interior priority is separate from `S0..S4`. A deep exterior/shared court can be `S3` with no enclosed interior; a shallow office may be `S2` with a small enclosed room.

- `I0 NONE` — no enterable enclosed interior promised by CITY-02.
- `I1 SHALLOW` — one bounded enterable room/zone or equivalent shallow access.
- `I2 DEEP` — multiple meaningful enclosed zones/thresholds are required.
- `I3 HERO` — layered enclosed/exterior relation with multiple meaningful access/discovery opportunities; exact layout belongs to CITY-06.

---

## 3. Two orthogonal classifications

### 3.1 Systemic importance — `A..D`

- `A` — **primary systemic anchor**: repeated-use location expected to carry more than one important product function or consequence pathway.
- `B` — **supporting systemic/playable location**: meaningful recurring or scenario-supporting place that enriches routes/district use without carrying the city's main causal load.
- `C` — **ambient urban fabric**: playable/legible fabric whose main job is ordinary city coherence, traversal, residence/work texture or spacing.
- `D` — **scenic context**: composition/identity context not promised as systemic playable content.

### 3.2 Spatial production depth — `S0..S4`

- `S0` — scenic envelope / inaccessible context.
- `S1` — authored shell, façade or exterior threshold only.
- `S2` — shallow playable place with bounded interaction/spatial use.
- `S3` — deep playable place with multiple authored spaces, thresholds or anchors.
- `S4` — hero layered place with multiple meaningful access/discovery opportunities.

### 3.3 Independence test

Neither axis is derived from the other. This programme deliberately contains:

- `A/S1`: the arrival/bus threshold — highly systemic, spatially shallow;
- `A/S2`: the Puerto work/concession hub and Ensanche neighbourhood anchor;
- `A/S3`: Ayuntamiento, workshop and persistent-residence cluster;
- `A/S4`: the casco bar/social house;
- `B/S1`, `B/S2` and `B/S3` examples across multiple districts;
- `C/S3`: an ordinary casco passage/shared court — spatially deep but not a primary system anchor;
- substantial `C/S1` ordinary building/frontage fabric and `D/S0` scenic envelope.

If a later table can predict `S` solely from `A..D`, or vice versa, it has collapsed the contract and must be corrected.

---

## 4. District × location programme

`Mobility anchor` names accepted CITY-01 planning nodes/route families. It does not create a new edge. `Revisit reason` is a place-level reason; it does not claim a runtime schedule or quest.

| Programme ID | District/family | Place / family | Mobility anchor | Domain | A–D | S | Interior | Default access posture | Non-visual revisit reason |
|---|---|---|---|---|---|---|---|---|---|
| `loc.casco.bar` | Casco Viejo | bar / social house | `W.CASCO` | food, drink, social, information | **A** | **S4** | **I3** | public + service + semi-private layers | service/social use, meeting/witness potential, repeat interior use |
| `loc.casco.bridgehead` | Casco Viejo | Puente Viejo head + mirador edge | `W.X1`, `W.CASCO` | crossing, leisure, witness | **B** | **S1** | I0 | public | territorial crossing choice, observation/meeting point |
| `loc.casco.shared_court` | Casco Viejo | passage + shared court / stair relation | `W.CASCO` / lower lanes | residential fabric, quiet semi-private | **C** | **S3** | I0 | public passage + semi-private court | route texture, ordinary neighbour use, second-layer spatial depth |
| `fam.casco.houses` | Casco Viejo | ordinary old-quarter residences/frontages | `W.CASCO`, lower lanes | residential | **C** | **S1** | I0 by default | private thresholds | home-town coherence; later selected exceptions may deepen only by explicit downstream choice |
| `loc.plaza.ayuntamiento` | Plaza y Ayuntamiento | town hall / civic office | `W.PLAZA` | civic, governance, authority | **A** | **S3** | **I2** | public + staff/service + private records layer | municipal decisions, permissions/notices, institutional witness |
| `loc.plaza.market` | Plaza y Ayuntamiento | market terrace / fountain / stall pitches | `W.PLAZA` | market, social, leisure | **B** | **S2** | I0 | public | periodic exchange/social use, physical activity/event home without UI portal |
| `fam.plaza.arcades` | Plaza y Ayuntamiento | benches / arcades / ordinary frontages | `W.PLAZA` | social rest, ambient commerce | **C** | **S1** | I0 | public/frontage | ordinary waiting/rest, crowd spacing, non-event city life |
| `loc.calle.bakery` | Calle Mayor | bakery + service threshold | `W.SHOP` / middle route | food, retail, work | **B** | **S3** | **I2** | public + service + private work layer | repeat daily-life service, production/work visibility |
| `loc.calle.pharmacy` | Calle Mayor | pharmacy / health-service frontage | `W.SHOP` / middle route | health, retail/service | **B** | **S2** | **I1** | public + private staff/storage | grounded health/service destination, witness/contact potential |
| `loc.calle.everyday_shop` | Calle Mayor | general everyday shop/service | `W.SHOP`, `W.X2` direction | retail/service | **B** | **S2** | **I1** | public + service | repeated practical destination from Ensanche/core routes |
| `fam.calle.mixed_frontages` | Calle Mayor | mixed-use house/shop shells | `W02–W04` family | retail/residential fabric | **C** | **S1** | I0 by default | frontage + private upper thresholds | ordinary commercial continuity; not every shopfront opens |
| `fam.calle.landmark_shell` | Calle Mayor | visually strong non-playable shell/significant façade | middle route | composition/context | **D** | **S1** | I0 | inaccessible shell | orientation/identity only; explicitly not systemic content |
| `loc.barrio.residence_cluster` | Barrio Alto | persistent-residence cluster / shared landing | `W.BARRIO` | residential, social baseline | **A** | **S3** | **I2** | private + semi-private + public threshold | home/work-return anchor, relationship/witness context, ordinary absence matters |
| `loc.barrio.lavadero` | Barrio Alto | lavadero / shared patio edge | `W.X3`, `W15` quiet ravine route | domestic work, social, quiet | **B** | **S2** | I0 | public/semi-private edge | recurring domestic/social use without spectacle; alternative crossing context |
| `loc.barrio.viewpoint` | Barrio Alto | bench/viewpoint/upper pause | high quiet route | leisure, quiet | **C** | **S1** | I0 | public | rest/orientation and low-intensity social opportunity |
| `fam.barrio.homes` | Barrio Alto | ordinary stepped residences | `W09–W10` high route | residential | **C** | **S1** | I0 by default | private threshold | schedule/home fabric without promise that every door opens |
| `loc.ensanche.neighbourhood_anchor` | Ensanche | corner shop + small square relation | `E.HOME`, `E.X2` | retail, social, leisure | **A** | **S2** | **I1** | public + service | second residential centre; repeated practical/social use independent of old core |
| `loc.ensanche.shared_garden` | Ensanche | shared garden/court / neighbour threshold | `E.HOME`, `E.X3` direction | residential, quiet semi-private | **B** | **S2** | I0 | semi-private + public edge | neighbour use, low-intensity encounter, reason to traverse upper bank |
| `fam.ensanche.homes` | Ensanche | ordinary homes with gardens | `E01–E04` family | residential | **C** | **S1** | I0 by default | private threshold | distinct newer residential fabric; selective future deepening only |
| `loc.ribera.workshop` | Ribera y Talleres | workshop / repair workplace | `W.RIBERA` | work, workshop, service | **A** | **S3** | **I2** | public/customer threshold + service + private work layer | work obligations, material change, repair/service destination |
| `loc.ribera.service_yard` | Ribera y Talleres | storage / delivery yard | `W.RIBERA`, service relation toward `W17` | work, logistics | **B** | **S2** | I0/I1 | service-first, conditional public edge | goods movement, delivery/collection, changing material state |
| `loc.ribera.paseo_edge` | Ribera y Talleres | river walk / towpath work overlook | low `W07–W08` family | river activity, leisure, quiet | **B** | **S1** | I0 | public | alternate route, observation, work/water relation without forced incident |
| `fam.ribera.sheds` | Ribera y Talleres | ordinary sheds / retaining/work frontages | low/work route | work fabric | **C** | **S1** | I0 by default | service/frontage | ordinary work edge; visual/material continuity |
| `loc.puerto.work_hub` | Puerto Fluvial | quay + weighbridge + concession-office cluster | `O.QUAY`, `O.PUERTO_UP` | port work, logistics, governance | **A** | **S2** | **I1** | public work frontage + service + staff room | ordinary work, goods/concession consequences, repeat material destination |
| `loc.puerto.landing` | Puerto Fluvial | X6/X7 landing / upstream port threshold | `O.PUERTO_UP` | crossing, arrival, territorial access | **B** | **S1** | I0 | public subject to inherited crossing availability | real crossing choice/state consequence, meeting/observation point |
| `loc.puerto.worker_social` | Puerto Fluvial | carter/raft-crew social corner / modest fonda-room relation | `O.QUAY` / port road | food, drink, social, work | **B** | **S3** | **I2** | public + service + semi-private | keeps port socially ordinary rather than mission-only; shift-change/social use |
| `loc.puerto.warehouse_yard` | Puerto Fluvial | warehouse + timber/drying yard relation | `O.QUAY` | logistics, work | **B** | **S2** | **I1** | service-first + bounded public frontage | goods/material state, loading activity, physical work/activity home |
| `fam.puerto.sheds` | Puerto Fluvial | ordinary sheds / working frontage | `O02` family | work fabric | **C** | **S1** | I0 by default | service/frontage | port scale/ordinary labour without turning every shed into content |
| `loc.entrada.arrival` | Entrada y Carretera | bus stop / arrival threshold | `O.ENTRADA` | arrival, visitor edge, information | **A** | **S1** | I0 | public | outsiders enter/leave here; strong information/meeting function with shallow spatial depth |
| `loc.entrada.fonda` | Entrada y Carretera | visitor/carter lodging + common room | `O.ENTRADA` / Puerto road relation | lodging, food, outsider social | **B** | **S3** | **I2** | public + private rooms + service | visitor/outsider reason to remain at edge; second ordinary non-port service |
| `loc.entrada.depot_forecourt` | Entrada y Carretera | depot/service forecourt | `O.ENTRADA` | logistics, service, arrival | **B** | **S1** | I0 | service + public waiting edge | deliveries/road arrivals, ordinary non-river service at Puerto/Entrada |
| `fam.entrada.road_edge` | Entrada y Carretera | ordinary road-edge sheds/fields/walls | `O.ENTRADA` | edge fabric | **C** | **S1** | I0 | mixed frontage | transition and expansion seam without urban spectacle |
| `loc.vega.supply_node` | La Vega | huerta/mill/market-supply working node | `W.VEGA`, `W.X4` direction | rural work, food supply | **A** | **S2** | **I1** | public work edge + private/service portions | market supply, seasonal work/material role, rural-to-town repeat relation |
| `loc.vega.quiet_paseo` | La Vega | upstream rest / river-edge quiet place | `W.VEGA`, `W07` quiet route | leisure, river, quiet | **B** | **S1** | I0 | public | purposeful low-intensity destination and alternate route, no incident obligation |
| `fam.vega.huertas` | La Vega | ordinary huertas / walls / rural work plots | `W.VEGA`, `W14` family | rural work fabric | **C** | **S1** | I0 by default | private/service edges + public path | seasonal supply context and believable edge life |
| `fam.scenic.slopes` | citywide scenic envelope | distant green slopes / rock silhouettes | outside playable graph | scenic/context | **D** | **S0** | I0 | inaccessible | valley identity only; never counted as playable/reactive content |
| `fam.scenic.roofline` | citywide composition | inaccessible roof/backdrop mass beyond playable promise | district roofline | scenic/context | **D** | **S0** | I0 | inaccessible | density/silhouette without false door/interior promise |

### 4.1 Required-domain coverage

The ledger covers all CITY-02 required domains:

- civic/governance/authority — Ayuntamiento; Puerto concession/weighbridge;
- food/drink/social — casco bar, port worker social corner, Entrada fonda;
- retail/service — Calle Mayor bakery/pharmacy/everyday shop; Ensanche neighbourhood anchor;
- work/workshop/logistics — Ribera workshop/yard, Puerto work hub/warehouse, Entrada depot, Vega supply node;
- port/waterfront — landing, quay/work hub, warehouse yard, paseo;
- residential — Barrio Alto, Ensanche, Casco ordinary/shared fabric;
- leisure/minigame physical homes — bar table/counter zone, market stall pitches/terrace, Ensanche square, workshop/yard work bay and Puerto loading yard are physical candidate homes; CITY-02 freezes **places**, not minigame mechanics;
- health/safety/authority — pharmacy plus Ayuntamiento/municipal authority surface; no hospital/police-complex scope is invented;
- rural/river activity — Vega supply/huertas, Ribera paseo, Puerto work edge;
- quiet/private/semi-private — Barrio lavadero, shared court/garden, upper/viewpoint and Vega quiet route, private residence thresholds;
- arrival/visitor edge — bus arrival, fonda, depot/forecourt.

No domain requires a UI-only portal or an unowned route.

---

## 5. A/B systemic-use profiles

Time bands below are **demand windows** (`early`, `day`, `late/day-evening`, `arrival/shift-change`, `seasonal/contextual`), not authored schedules or opening hours. Role families are illustrative spatial consumers, not frozen NPC behaviour.

`Change potential` means a later accepted system could alter visible access/material/use state here. It does not assign that runtime authority to CITY.

| ID | Plausible users / time bands | Activity / material role | Information / witness potential | Governance / access hook | Interior need | Change potential | Still functions when player absent because… |
|---|---|---|---|---|---|---|---|
| `loc.casco.bar` | staff, locals, visitors; day→late | food/drink/service + social tables | dense conversation/witness surface | public threshold; staff/service and semi-private layers | I3 hero layering | occupancy/use, access, furnishings or social context may change later | it is a service/social node actors can plausibly use regardless of player presence |
| `loc.casco.bridgehead` | pedestrians, residents, visitors; all ordinary travel bands | crossing approach + pause/view | sees arrivals/departures and Río crossing changes | inherits X1 `AH`; no cart-freight invention | I0 | crossing availability/context can alter flow | it remains a territorial route/witness point |
| `loc.plaza.ayuntamiento` | clerk/authority roles, residents, traders; day/contextual | civic administration, notices, permissions | institutional witness/records surface | civic public→staff/private thresholds | I2 | public access, notices, decisions, queue/use state | town governance has a physical address even off-camera |
| `loc.plaza.market` | vendors, shoppers, passers-by; market/context windows | exchange, stalls, fountain/rest | broad public witness surface | public terrace; event occupation must not destroy graph permanently | I0 | stall occupation/market setup/closure footprint | it remains an exchange/social place rather than an event trigger only |
| `loc.calle.bakery` | baker/service roles, residents; early→day | food production + retail | routine witness/contact surface | public frontage + service/private work threshold | I2 | stock/workroom/use state later | production/service rationale exists without player |
| `loc.calle.pharmacy` | residents, service staff; day/contextual | health/service retail | discreet contact/witness potential | public counter + private staff/storage | I1 | availability/stock/access later | practical service destination remains useful |
| `loc.calle.everyday_shop` | residents from core/Ensanche; day | everyday goods/service | ordinary neighbourhood information surface | public + service | I1 | stock/frontage/access later | recurring practical destination anchors route use |
| `loc.barrio.residence_cluster` | resident roles, neighbours; early/late + contextual | home/private life + shared threshold | neighbour/absence/witness context | private + semi-private access layers | I2 | occupancy/access/material traces later | a home is meaningful even when the player is elsewhere; absence/return can matter to later systems |
| `loc.barrio.lavadero` | residents, walkers; day/contextual | domestic work + pause/social edge | quiet local witness potential | public/semi-private edge on X3/ravine context | I0 | use/material/weather context later | ordinary domestic function survives without incidents |
| `loc.ensanche.neighbourhood_anchor` | residents, visitors crossing X2; day→early evening | practical retail + small-square social use | local witness/meeting surface | public + service; tied to existing Ensanche routes | I1 | access/stock/square occupation later | gives Ensanche its own recurring centre instead of making every trip cross to old core |
| `loc.ensanche.shared_garden` | neighbours/residents; quiet day/evening | shared domestic/leisure space | low-intensity neighbour witness | semi-private + public edge | I0 | access/use state later | ordinary residential life has a spatial home |
| `loc.ribera.workshop` | worker/customer/service roles; work bands | repair/craft + deliveries | workplace witness/material provenance | public/customer + service/private work layers | I2 | work target, materials, access later | work obligations/material changes continue to make sense off-camera |
| `loc.ribera.service_yard` | workers/delivery roles; work/arrival bands | storage, delivery, collection | material/witness surface | respects service access; `AS` is not ordinary-public proof | I0/I1 | goods/material occupation | logistics exists as ordinary town function |
| `loc.ribera.paseo_edge` | workers, walkers, residents; broad/quiet bands | route + river/work observation | sees work edge and movement without guaranteed event | public low route | I0 | temporary work/water context later | alternate quiet route remains useful without player-centric incidents |
| `loc.puerto.work_hub` | port workers, traders, municipal/concession roles; work/seasonal | weighing, quay work, concession/admin | goods/provenance + institutional witness | public work edge + staff/service room | I1 | goods, access/capacity/concession state later | port is an ordinary work system, not a mission backdrop |
| `loc.puerto.landing` | pedestrians/porters/crossing users; arrival/contextual | territorial crossing and waiting edge | strong arrival/departure witness | inherits X6/X7 availability exactly | I0 | crossing state changes flow | crossing demand exists independently of player |
| `loc.puerto.worker_social` | crews, carters, visitors; shift-change/late | food/drink/rest/social | port-specific information/witness surface | public + service + semi-private | I2 | use/access/context later | gives Puerto ordinary social life and repeated reason to remain |
| `loc.puerto.warehouse_yard` | workers/delivery/trader roles; work bands | storage/loading/material handling | material provenance/witness | service-first bounded public frontage | I1 | stock/yard occupation/access later | goods chain remains spatially coherent off-camera |
| `loc.entrada.arrival` | outsiders, residents meeting arrivals; arrival bands | bus/road arrival, waiting, orientation | first-contact/information surface | `O.ENTRADA`; BUS stops here and does not imply bus streets | I0 | arrival/departure context later | visitors have a real entry/exit address |
| `loc.entrada.fonda` | visitors, carters, local staff; arrival→late | lodging, food, common room | outsider/local information crossover | public common room + private rooms/service | I2 | occupancy/access later | visitor edge supports ordinary stays, not only cutscenes |
| `loc.entrada.depot_forecourt` | delivery/service/arrival roles; work bands | road service, waiting, transfer | road-arrival witness | service + public waiting edge | I0 | vehicle/goods presence later (without promising vehicle simulation) | second ordinary non-port service makes edge plausible |
| `loc.vega.supply_node` | farm/market-supply roles, residents; early/day/seasonal | food/material supply, mill/huerta work | rural-to-market provenance/witness | public work edge + private/service portions | I1 | harvest/material/access context later | supply role explains why Vega matters when player is elsewhere |
| `loc.vega.quiet_paseo` | walkers/workers/residents; broad quiet bands | rest, route choice, river/rural observation | low-density witness only | public Q route | I0 | minimal; quiet use is allowed to stay ordinary | it is valuable specifically as non-spectacle space |

### 5.1 Spatial requirements later Living World/PA may consume

CITY-02 offers the following spatial affordances without owning their semantics:

- stable home, work, service, social, civic, arrival and supply destinations;
- public/semi-private/private/service threshold distinctions;
- places where presence, absence or route choice could be observable;
- physical surfaces for information/witness opportunities;
- material-state-capable workplaces/yards;
- multiple districts with reasons for recurring use;
- quiet places where nothing exceptional needs to happen.

Living World/PA remains owner of actual schedules, beliefs, dialogue, relationship logic, decision policy, incident generation and off-screen simulation.

---

## 6. Interior-priority ledger

This is the explicit backlog handed forward. It promises **need**, not layout.

### I3 HERO

- `loc.casco.bar` — the only CITY-02 hero interior commitment. It must support public social space, service/back relation, at least one additional meaningful layer and multiple truthful access/discovery opportunities to be designed by CITY-06.

### I2 DEEP

- `loc.plaza.ayuntamiento`
- `loc.calle.bakery`
- `loc.barrio.residence_cluster`
- `loc.ribera.workshop`
- `loc.puerto.worker_social`
- `loc.entrada.fonda`

These require multiple meaningful zones/thresholds but are not automatically hero spaces.

### I1 SHALLOW

- `loc.calle.pharmacy`
- `loc.calle.everyday_shop`
- `loc.ensanche.neighbourhood_anchor`
- `loc.ribera.service_yard` (may remain exterior-led; one shallow enclosed support room maximum is sufficient)
- `loc.puerto.work_hub`
- `loc.puerto.warehouse_yard`
- `loc.vega.supply_node`

### I0 NONE by CITY-02

All remaining rows, including bridgeheads, market terrace, shared courts/gardens, paseo/quiet places, ordinary `C` fabric and scenic `D` envelope. A downstream WP may not silently turn every I0 façade into promised playable interior merely because an asset provides one.

### 6.1 Openable/playable versus façade-only promise

- `I1–I3` is an explicit enterable/interior need subject to later shell/interior design.
- `I0` means CITY-02 promises no enclosed interior. Some I0 places are still playable exterior spaces (`S2/S3`).
- `C/S1` ordinary fabric is façade/shell-first by default.
- `D/S0` scenic context is not playable.

This distinction is intentional production scope, not missing content.

---

## 7. Quiet / ordinary fabric declaration

Reactive density is not universal incident density.

Protected low-intensity commitments:

1. `W07` upstream paseo / `loc.vega.quiet_paseo` remains useful as a route/rest place with **no incident-generation requirement**.
2. `W15` ravine/lavadero context remains quiet; `loc.barrio.lavadero` may support ordinary domestic/social use without becoming a clue/event dispenser.
3. Upper Barrio/Ensanche residential fabric retains substantial `C/S1` frontage and private thresholds.
4. Ordinary port sheds/yards are not all missions, secrets or enterable interiors; `fam.puerto.sheds` remains `C/S1`.
5. Scenic slopes/roofline remain `D/S0` and never count toward destination/reactive-density totals.

CITY-05 parcelisation must preserve these categories rather than promoting unprogrammed buildings to A/B by default.

---

## 8. Reactive-density measurement proposal

These are **measurement definitions**, not fabricated current measurements. CITY-03/04/later playtest owners may select the subset their realized geometry can honestly evaluate. No metric authorizes CITY-04 to build beyond its accepted seed.

### RD-1 — Meaningful destinations per traversal (`MDT`)

For a sampled public traversal, count **distinct A/B programmed place thresholds actually reachable from that traversal** and divide by traversed minutes.

- Count a place once even if it has multiple frontages.
- Do not count props, `C/D` fabric, scenic envelope or UI markers.
- Report `A`, `B` and total separately.
- Recommended display unit: destinations per 5 minutes of traversal.

Purpose: reveal dead stretches or overstuffed corridors without demanding every doorway be interactive.

### RD-2 — Repeated-use ratio (`RUR`)

During playtest/runtime later, for A/B places available in the tested slice:

`RUR = A/B places used meaningfully in >=2 distinct observed contexts / A/B places observed`

A distinct context is a different place role (service, work, social, civic, arrival, material/logistics, quiet leisure, information/witness), not the same interaction repeated twice.

Pre-runtime planning proxy: mark how many A/B profiles above contain at least two independent role families. The proxy is not gameplay proof.

Purpose: prevent expensive one-scene locations that never justify return.

### RD-3 — Route-choice density (`RCD`)

For a sampled public traversal, count accepted public decision points where at least two materially different route continuations remain legal and lead toward different programmed destinations; divide by traversed minutes.

- Restricted `AS` service routes do not count in the ordinary-public sample.
- A low-water/seasonal edge counts only in a scenario where CITY-01 says it is available.
- Plaza removal must still use inherited public alternates rather than a hidden service route.

Purpose: tie place density to real movement choice rather than clustering all content at one hub.

### RD-4 — Quiet-space balance (`QSB`)

For each sampled traversal, report:

- minutes on CITY-01 `Q`-tagged route segments;
- minutes through `C/D`-dominant frontage with no required A/B threshold;
- total minutes.

No single universal percentage is frozen before geometry exists. The programme requirement is qualitative but falsifiable: at least one representative route sample must retain a purposeful low-intensity stretch, and adding A/B places must not force `QSB` to zero everywhere.

Purpose: protect ordinary pacing and observational space.

### RD-5 — Depth distribution (`DD`)

Produce a cross-tab of programmed/realized places by `A..D × S0..S4`, and separately by `I0..I3`.

Automatic review warnings:

- all `A` locations share one S-depth;
- all `B` locations share one S-depth;
- any rule promotes `A→S4`, `B→S3`, `C→S1`, `D→S0` mechanically;
- `C/D` or `S0/S1` disappear from the realized scope;
- interior priority is inferred solely from A–D rather than the ledger.

Purpose: detect classification collapse and silent scope inflation.

### 8.1 Recommended representative samples

Subject to later geometry/seed ownership, useful samples include:

- Ensanche origin → X2 → Calle Mayor service/Plaza edge;
- Barrio Alto → Ribera using the high/low lateral relation;
- Vega → market/commercial approach with a quiet-route comparison;
- Casco → Puerto using X6/X7 versus X1/camino-sur scenario where realized;
- Entrada → Puerto work/social edge;
- one deliberate quiet sample along upstream paseo/ravine fabric.

A sample is `NOT_IN_SEED` rather than “measured” if the required geometry is absent, consistent with CITY-01.

---

## 9. District revisitation and anti-monopoly check

Every substantial district has at least one non-visual reason to revisit:

| District | Revisit drivers |
|---|---|
| Casco Viejo | bar/social house; X1 bridgehead; shared-court spatial layer |
| Plaza/Ayuntamiento | civic office; market terrace |
| Calle Mayor | bakery, pharmacy, everyday shop |
| Barrio Alto | residence cluster; lavadero/quiet social place |
| Ensanche | neighbourhood shop/square; shared garden; crossing choice |
| Ribera/Talleres | workshop, service yard, low paseo/work observation |
| Puerto Fluvial | work/concession hub, crossing landing, warehouse, worker social place |
| Entrada/Carretera | arrival threshold, fonda, depot/service forecourt |
| La Vega | supply node, quiet paseo, huerta fabric |

Primary `A` anchors are deliberately distributed across Casco, civic core, Barrio Alto, Ensanche, Ribera, Puerto, Entrada and Vega. Casco + Plaza therefore do not monopolise systemic importance.

Puerto explicitly has **ordinary work, logistics and social hooks**; it is not a mission-only or harbour-hero district.

---

## 10. Access / closure compatibility notes

CITY-02 does not change CITY-01, but location programming must remain useful under its important scenarios:

- **X2 closure:** Ensanche retains a neighbourhood anchor on its own bank, so closure does not make the district contentless; pedestrians may use accepted X3/X4 alternatives per CITY-01.
- **X6 suspension / State 1:** Puerto work/social/arrival places remain on Orilla sur and still make sense when Wedge traffic detours through X1 + camino sur. No dry shortcut is implied.
- **X7 closure / State 2:** pedestrian access can fall back to X1; cart-freight interruption remains a real consequence and may affect Puerto material use later. CITY-02 does not invent a replacement freight path.
- **Plaza event/closure:** Calle Mayor, Barrio, Ribera and other place families remain distributed across the accepted alternate-route graph; the programme does not make Plaza the sole systemic hub.
- **Quiet routes:** A/B placement along/near quiet routes creates destinations but does not require continuous event density between them.

---

## 11. Scope envelope for CITY-05 and CITY-06

### CITY-05 receives

- the complete programme IDs/families above;
- required frontage/access relations and mobility anchors;
- A–D/S0–S4 classifications;
- interior priorities as shell/access obligations, **not** detailed room layouts;
- explicit ordinary/scenic families that should remain cheap/reusable;
- no permission to reinterpret CITY-01 access classes or create new routes.

### CITY-06 later receives

- the `I1–I3` backlog;
- the `S2–S4` locations needing selective deeper spatial treatment;
- the public/semi-private/private/service relations;
- the requirement that `loc.casco.bar` is the only CITY-02 hero-layer commitment;
- examples of spatial surfaces where later authored/systemic/hybrid discovery could truthfully attach, without pre-accepting runtime semantics.

CITY-06 remains responsible for actual interior/discovery patterns. CITY-02 does not predesign secret rooms, keys, invitations, overheard-dialogue logic or actor schedules.

---

## 12. C/D and S0/S1 production-scope declaration

The master ledger is a **programme of meaningful place families**, not a census of every future parcel. The accepted city-scale estimate includes far more buildings/frontages than the A/B entries above, and CITY-02 explicitly intends most future fabric to stay cheap unless a later reviewed need promotes it.

Therefore:

- substantial ordinary building count may remain `C/S1` façade/shell fabric;
- scenic valley/roofline context remains `D/S0`;
- some spatially deep ordinary places may be `C/S2–S3` without becoming systemic anchors;
- `I0` is the default for unprogrammed ordinary buildings;
- no downstream asset/prefab convenience may be used as a reason to make every door open;
- promotion from `C/D` or `I0` to deeper/systemic scope requires an explicit reviewed downstream reason and must stay within that WP's authority.

This is intentional scope control and preserves the workpack's negative gate against “reactive city = everything interactive.”

---

## 13. Residuals and downstream ownership

CITY-02 intentionally leaves open:

- exact parcel/site placement, frontage dimensions, building shells and reusable families — `CITY-05`;
- detailed interior layouts, threshold geometry, discovery layers and secret/access routes — `CITY-06`;
- exact retained-seed selection/boundary and which subset of this programme enters first — `CITY-03`;
- physical blockout, measured traversal and realized reactive-density samples inside the selected seed — `CITY-04` where within its accepted scope;
- full-city measurements outside the seed — preserved from CITY-01 as unmeasured until sufficient realized geometry has an accepted owner;
- runtime schedules, opening hours, beliefs, dialogue, relationships, decisions, incident generation and off-screen simulation — Living World/runtime owners;
- final IDs, parcel counts, NPC bindings, narrative/backstories and minigame mechanics — later explicit owners;
- State 1→State 2 production timing — remains outside CITY-02.

---

## 14. CITY-02 contract closure

This programme provides CITY-05/CITY-06 with one reviewed semantic owner for which places matter and how much spatial/interior promise they carry:

- all required place domains have physical homes;
- every substantial district has non-visual revisit reasons;
- old quarter/civic centre do not monopolise `A` anchors;
- Puerto contains ordinary work/logistics/social use;
- protected quiet/ordinary fabric remains explicit content;
- openable interiors are distinct from façade-only promises;
- `A..D` and `S0..S4` contain direct counterexamples to any one-axis mapping;
- activities/minigame candidates have physical place sockets rather than UI-only portals, without inventing mechanics;
- reactive density has reproducible later measurement definitions that do not require every prop or door to be interactive;
- substantial `C/D` and `S0/S1` scope is an explicit production decision.

No CITY-00 geography, CITY-01 mobility, Unity/runtime implementation, ART style authority or Living World semantics are changed by this document.
