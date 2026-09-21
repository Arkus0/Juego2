# Keeper City — Mobility topology, access graph and travel-cost hypotheses

Version: 1.0 — 2026-09-21  
Workpack: `WP-CITY-01 — Mobility, district graph + walk-time topology`  
Class: **PRODUCT / SPATIAL PREPRODUCTION — NON-FOUNDATIONAL**

Status: **CITY-01 candidate semantic owner.** This document owns the movement/access graph, planning route costs, access/elevation vocabulary, mobility-profile assumptions and closure/alternate-route semantics created by CITY-01. It does not alter the accepted CITY-00 geography.

Accepted predecessor: `Docs/production/CITY_SPATIAL_CONSTITUTION.md` plus `Docs/evidence/WP-CITY-00/CONNECTIVITY_MATRIX.md`. If this document appears to create a dry Wedge→Puerto edge, an Ensanche↔Orilla-sur edge, or any inter-landmass edge other than `X1..X7`, this document is wrong.

All times below are **planning weights / target hypotheses**, not measurements. `CITY-04` must measure them in the Unity greybox.

---

## 1. What CITY-01 freezes — and what it does not

CITY-01 freezes enough movement structure that CITY-02 can place meaningful locations without inventing routes ad hoc:

- one semantic route/access graph;
- primary, secondary, quiet and service/back route families;
- road/ramp/steep-lane/stair access classes;
- planning route-cost weights;
- mobility-profile assumptions;
- chokepoints, closure consequences and alternate routes;
- measurement questions for CITY-04.

CITY-01 does **not** freeze final metric geometry, navmesh, street engineering, final slope percentages, systemic-location importance, interior layouts, live NPC schedules, vehicle simulation, fast travel or the retained-seed boundary.

The graph is a planning contract: a later greybox may move bends, lengths and grades while preserving accepted connectivity and explaining any material cost change.

---

## 2. Inherited topology that this graph may not reinterpret

The following are consumed from CITY-00:

1. **Landmasses:** Wedge, Ensanche bank and Orilla sur. The Wedge ends at the confluence.
2. **Puerto:** Puerto + Entrada live on Orilla sur downstream of the confluence.
3. **Arroyo crossings:** `X2..X5`, with `X2/X3` permanent, `X4` seasonal/flood-closable and `X5` low-water only.
4. **Río crossings:** `X1` plus `X6` in State 1; `X1` plus `X7` in State 2.
5. **No hidden crossing:** there is no Ensanche-bank ↔ Orilla-sur edge and no dry Wedge ↔ Puerto continuation.
6. **Three Wedge longitudinal families:** low paseo/sirga, middle Calle Mayor and high callejas/upper route.
7. **Plaza independence:** the Plaza may converge routes but cannot be the structural connector.
8. **Quiet fabric:** upper Vega, upstream paseo and the ravine/lavadero route remain low-intensity.
9. **Scale hypotheses:** effective pedestrian speed starts at 1.15 m/s; representative long ordinary travel remains targeted at roughly 13–17 min; all such values remain unmeasured until CITY-04.

This document adds internal same-landmass edges. It adds **zero** new water crossings.

---

## 3. Semantic graph rule

### 3.1 The edge ledger is authoritative

The tables in §5 are the **only semantic definition of graph edges**. Node alignment, prose order and any later diagram are non-semantic projections. An edge exists only if an edge ID appears in §5.

This rule deliberately prevents a visual line or convenient drawing from creating an unreviewed connection.

### 3.2 Node vocabulary

Node IDs are planning anchors, not final object IDs.

| Node | Landmass | Meaning |
|---|---|---|
| `W.VEGA` | Wedge | upper Vega / NE rural-edge anchor |
| `W.CM_NE` | Wedge | NE end of Calle Mayor |
| `W.X2` | Wedge | Wedge head of Puente del Mercado |
| `W.SHOP` | Wedge | representative Calle Mayor shop frontage |
| `W.PLAZA` | Wedge | Plaza / Ayuntamiento market terrace |
| `W.CASCO` | Wedge | representative Casco Viejo/bar anchor |
| `W.LANDING` | Wedge | confluence-tip landing head for X6/X7 |
| `W.RIBERA` | Wedge | representative Ribera/Talleres workplace |
| `W.BARRIO` | Wedge | representative Barrio Alto home / upper-lane anchor |
| `W.X1` | Wedge | Wedge head of Puente Viejo |
| `W.X3` | Wedge | Lavadero / Barrio Alto head of X3 |
| `W.X4` | Wedge | Vega head of X4 |
| `W.X5` | Wedge | lower-casco head of X5 |
| `E.HOME` | Ensanche bank | representative Ensanche home |
| `E.X2..E.X5` | Ensanche bank | Ensanche-side heads of X2..X5 |
| `O.X1` | Orilla sur | far head of Puente Viejo |
| `O.PUERTO_UP` | Orilla sur | upstream Puerto edge / X6-X7 bridgehead |
| `O.QUAY` | Orilla sur | representative quay / working-frontage anchor |
| `O.ENTRADA` | Orilla sur | Entrada / bus / valley-road anchor |

`W.LANDING` and `O.PUERTO_UP` are on different landmasses. Only `X6` or `X7` connects them.

---

## 4. Route, elevation and access vocabulary

### 4.1 Route character

- `P` — **primary:** ordinary civic/commercial or road movement; busiest default path.
- `S` — **secondary:** legitimate everyday connector with lower flow or more contextual use.
- `Q` — **quiet:** protected low-intensity path; ordinary movement is allowed but the route is not designed as an incident funnel.
- `V` — **service/back:** goods/service route or rear access. It may be traversable when context permits, but it is **excluded from the public-route proofs** unless explicitly stated.

### 4.2 Elevation class

These are planning classes, not final grade percentages:

- `E0` — near-level terrace/road/bridge approach;
- `E1` — ordinary slope or ramped connector;
- `E2` — steep lane where bicycles may need to dismount and carts are not assumed;
- `E3` — stairs/stepped route; pedestrian only for ordinary routing;
- `EW` — water crossing or ferry service.

CITY-04 owns measured grade, total level change and actual traversal cost.

### 4.3 Access class

- `AR` — road-capable: pedestrian + slower pedestrian + bicycle + service/cart where route width later validates it.
- `AP` — public pedestrian route; bicycle may be allowed only where the profile table says it can roll or dismount.
- `AF` — foot-only public route; no wheeled-service claim.
- `AH` — pedestrian + handcart/light wheeled use, but **not cart freight**; used by X1 per CITY-00.
- `AS` — service/back route; authorized service/delivery plus context-permitted pedestrians; excluded from ordinary public shortest-path claims.
- `AX6` — State-1 ferry: pedestrian/porter/carryable load only in CITY-01. CITY-01 deliberately makes **no** claim that X6 carries carts, handcarts or bicycles.

---

## 5. Authoritative edge ledger

### 5.1 Wedge internal edges

`Normal-ped cost` is the routing weight in minutes for an ordinary pedestrian in planning conditions. Values are deliberately rounded planning weights; they are not measured seconds.

| Edge | From ↔ To | Family | Char | Elev. | Access | Normal-ped cost | Notes |
|---|---|---|---|---|---|---:|---|
| `W01` | `W.VEGA ↔ W.CM_NE` | middle approach | Q/S | E1 | AP | 2.5 | rural edge joins commercial terrace |
| `W02` | `W.CM_NE ↔ W.X2` | middle | P | E0 | AR | 1.8 | Calle Mayor |
| `W03` | `W.X2 ↔ W.SHOP` | middle | P | E0 | AR | 1.7 | Calle Mayor |
| `W04` | `W.SHOP ↔ W.PLAZA` | middle | P | E0 | AR | 0.5 | shop sits near plaza edge |
| `W05` | `W.PLAZA ↔ W.CASCO` | middle/south | P/S | E1 | AP | 1.75 | inherited 1.5–2 min plaza↔bar band |
| `W06` | `W.CASCO ↔ W.LANDING` | Cuesta | S | E2 | AP | 2.25 | ends at Wedge-side landing head |
| `W07` | `W.VEGA ↔ W.RIBERA` | low | Q | E1 | AP | 8.5 | upstream paseo/sirga; intentionally quiet |
| `W08` | `W.RIBERA ↔ W.LANDING` | low | S/V | E1 | AR | 2.5 | towpath/work-edge approach |
| `W09` | `W.VEGA ↔ W.BARRIO` | high | Q | E2 | AP | 5.5 | upper lanes / residential route |
| `W10` | `W.BARRIO ↔ W.CM_NE` | upper lateral | S/Q | E2 | AP | 2.5 | lets upper routes meet Calle Mayor without Plaza |
| `W11` | `W.BARRIO ↔ W.RIBERA` | high→low lateral | S | E3 | AF | 6.5 down / 7.5 up | east stairs; inherited Barrio Alto→Ribera target |
| `W12` | `W.CASCO ↔ W.X1` | old-bridge approach | S | E0 | AP | 1.0 | no Plaza needed |
| `W13` | `W.CASCO ↔ W.X5` | lower lanes | Q | E1 | AF | 1.0 | X5 remains opportunistic only |
| `W14` | `W.VEGA ↔ W.X4` | rural approach | Q/S | E1 | AR | 2.0 | X4 approach |
| `W15` | `W.BARRIO ↔ W.X3` | ravine/lavadero | Q | E1 | AF | 0.5 | protected quiet fabric |
| `W16` | `W.X4 ↔ W.CM_NE` | rural-road connector | S | E1 | AR | 1.0 | road detour when X2 closes |
| `W17` | `W.RIBERA ↔ W.SHOP` | service/back | V | E1 | AS | 4.0 | rear/service access; **not** used to prove public plaza-independence |

The three designed Wedge longitudinal choices therefore remain:

- **middle:** `W01→W02→W03→W04→W05→W06`;
- **low:** `W07→W08`;
- **high/upper:** `W09→W11→W08`, with `W10` as the upper-to-middle lateral connector.

They are not intended to have equal cost. Their value is differentiated route character, slope, quietness and access.

### 5.2 Ensanche-bank internal edges

| Edge | From ↔ To | Char | Elev. | Access | Normal-ped cost | Notes |
|---|---|---|---|---|---:|---|
| `E01` | `E.HOME ↔ E.X2` | P | E0 | AR | 1.1 | ordinary road route |
| `E02` | `E.HOME ↔ E.X3` | Q | E1 | AP | 1.5 | upper pedestrian choice |
| `E03` | `E.X3 ↔ E.X4` | Q/S | E1 | AP | 2.0 | upper bank continuity |
| `E04` | `E.HOME ↔ E.X5` | Q | E1 | AF | 2.0 | low-water shortcut approach |
| `E05` | `E.X2 ↔ E.X4` | S | E0/E1 | AR | 0.8 | road-capable upstream detour link |

### 5.3 Crossings — the complete inter-landmass edge set

| Edge | From ↔ To | State/availability | Access | Cost | Binding source |
|---|---|---|---|---:|---|
| `X1` | `W.X1 ↔ O.X1` | State 1+2 permanent | AH | 1.0 | Puente Viejo; no cart freight |
| `X2` | `W.X2 ↔ E.X2` | State 1+2 permanent | AR | 0.7 | Puente del Mercado; road-capable |
| `X3` | `W.X3 ↔ E.X3` | State 1+2 permanent | AF | 0.5 | Pasarela del Lavadero; foot only |
| `X4` | `W.X4 ↔ E.X4` | seasonal; flood-closable | AR | 0.9 | Puente de la Vega; cart detour when available |
| `X5` | `W.X5 ↔ E.X5` | low water only | AF | 0.4 | pasos/vado; never counted as permanent redundancy |
| `X6` | `W.LANDING ↔ O.PUERTO_UP` | **State 1 only**; hours/fare; high water suspends | AX6 | 1.0 + wait | La barca |
| `X7` | `W.LANDING ↔ O.PUERTO_UP` | **State 2 only**; permanent | AR | 1.0 | Puente del Muelle; carries carts |

There is no eighth crossing. `X6` and `X7` are mutually exclusive.

For X6, planning wait is modelled separately as `W_ferry ∈ [0,4] min` during operating hours. That is a scenario variable, not a promise of a four-minute real queue. When high water suspends the ferry, X6 is unavailable rather than assigned an infinite wait.

### 5.4 Orilla-sur internal edges

| Edge | From ↔ To | Char | Elev. | Access | Normal-ped cost | Notes |
|---|---|---|---|---|---:|---|
| `O01` | `O.X1 ↔ O.PUERTO_UP` | S/V | E0/E1 | AR | 3.0 | camino sur on one landmass |
| `O02` | `O.PUERTO_UP ↔ O.QUAY` | P/V | E0 | AR | 3.5 | working frontage / port road |
| `O03` | `O.QUAY ↔ O.ENTRADA` | P | E0 | AR + BUS | 1.0 | valley-road / bus approach |

No water is crossed by `O01..O03`.

---

## 6. Mobility profiles

These are **spatial routing assumptions**, not runtime AI or vehicle simulation contracts.

| Profile | Spatial rule |
|---|---|
| ordinary pedestrian | uses `AR/AP/AF/AH`, X6 when available, and `AS` only when access context explicitly permits it |
| slower pedestrian | same connectivity; start with ordinary route cost ×1.35, and ×1.50 on E2/E3 segments; CITY-04 measures whether this is fair |
| bicycle | rolls on `AR` and selected `AP` E0/E1 edges; E2/E3 and `AF` imply dismount/walk rather than a magical bicycle route; X6 carriage is **not assumed** |
| service / delivery cart | uses `AR/AS`; may use X2/X4 and State-2 X7; cannot use X1/X3/X5; CITY-01 does not grant cart carriage on X6 |
| porter / carried delivery | follows pedestrian topology and may use X6 when available |
| arrival / bus | bus movement terminates at `O.ENTRADA`; onward travel is another profile. No bus route through the core is created here |
| following / search | uses public pedestrian edges; `AS` is excluded unless the followed actor has legitimate access. Route design must expose real junction choices rather than hidden teleports |
| time-sensitive | selects among currently available routes using expected travel cost, including X6 wait/closure/context. This is a scenario-evaluation profile, not a claim about NPC decision logic |

The graph intentionally creates profile disagreement: the shortest pedestrian line may be unusable by a cart; a cyclist may prefer Calle Mayor over stairs; a late pedestrian may choose a longer-distance bridge route to avoid ferry wait.

---

## 7. Representative walk-time / route-cost matrix

### 7.1 Inherited CITY-00 targets reconciled

| Pair | CITY-01 route | Planning cost | CITY-00 target | Result |
|---|---|---:|---:|---|
| Plaza ↔ casco bar | `W05` | **1.75 min** | 1.5–2 | inside band |
| Ensanche home ↔ Plaza | `E01-X2-W03-W04` | **4.0 min** | 4–5 | inside band |
| Barrio Alto → Ribera workshop | `W11` downhill | **6.5 min** | 6–7 | inside band; reverse uphill target 7.5 |
| Vega ↔ Plaza/market | `W01-W02-W03-W04` | **6.5 min** | 6–7 | inside band |
| Puente Viejo far end ↔ Entrada/bus | `O01-O02-O03` | **7.5 min** | 7–8 | inside band |
| Puerto quay ↔ Calle Mayor shop | `O02-X7-W06-W05-W04` | **9.0 min** | 8–9 | top of band in State 2 |
| casco tip / landing ↔ NE Calle Mayor | `W06-W05-W04-W03-W02` | **8.0 min** | 8–10 | inside band |
| Vega NE ↔ Puerto quay | middle route + X7 | **15.0 min** | 15–17 | inside band |
| Vega NE ↔ Puerto quay | plaza-free low route `W07-W08-X7-O02` | **15.5 min** | 15–17 | inside band |

The State-1 equivalent of an X7 route substitutes X6 at the same base crossing cost and then adds `W_ferry`. Therefore CITY-00's walking bands remain **motion/route targets**; door-to-door State-1 ferry trips may legitimately exceed them while waiting.

### 7.2 Costs explicitly resolved by CITY-01

**Ensanche home ↔ Puerto quay** was deferred by CITY-00:

- State 2 ordinary public route via X2 / Calle Mayor / Plaza / landing / X7: **12.5 min**.
- State 2 permanent plaza-free route via X3 / Barrio Alto / east stairs / Ribera / landing / X7: **16.0 min**.
- Low-water X5 route is an opportunistic shortcut at roughly **10–11 min** and is never counted as permanent redundancy.
- State 1 direct-ferry versions add `W_ferry`.
- State 1 high-water fastest ordinary route uses X2 + core + X1 + camino sur: **≈14.25 min**.
- State 1 high-water **plaza-free** fallback via X3 → Barrio → Ribera → landing → Casco → X1 → camino sur is deliberately long: **≈22–23 min**. This is the cost consequence already implied by CITY-00 rather than a hidden new shortcut.

**Vega ↔ Puerto quay**:

- State 2 middle/public: **15.0 min**.
- State 2 low/plaza-free: **15.5 min**.
- State 1 direct ferry: the same base costs + `W_ferry`.
- State 1 X6-suspended route via market/core/X1/camino sur: **≈16.75 min**.
- plaza-free X1 fallback stays much longer and is not treated as the ordinary default.

### 7.3 L1 / L1′ traffic character — CITY-00 Q12 resolved

`L1/L1′ = Casco → X1 → camino sur → O.PUERTO_UP → X6/X7 → landing → Casco`.

Base motion cost is **≈8.25 min** before any State-1 ferry wait. It is a **secondary circumferential loop**, not the city's primary commute spine.

- **State 1 / X6:** fare, hours and variable wait make L1 deliberate and somewhat unreliable. It is useful for port workers, errands, following/search, walking loops and municipal events, but it does not replace the primary core routes.
- **State 2 / X7:** removing ferry uncertainty makes L1′ a more ordinary cross-river circulation loop and enables cart movement over the landing crossing. It still remains secondary because Calle Mayor, X2 and the internal Wedge routes carry the everyday core.

CITY-01 does not decide *when* State 1 becomes State 2.

---

## 8. Required scenario matrix

| Scenario | Route / decision | Result |
|---|---|---|
| upper/residential home → workplace | `W.BARRIO → W.RIBERA` via `W11` | 6.5 min downhill; different uphill cost preserves slope meaning |
| old quarter → port/work edge without Plaza | `W.CASCO → W.LANDING → X6/X7 → O.QUAY` | works; no Plaza edge used |
| port delivery → commercial destination | State 2 cart: `O.QUAY → O.PUERTO_UP → X7 → W.LANDING → W.RIBERA → W17 → W.SHOP` | service path bypasses Plaza/customer lanes; State 1 requires break-bulk to porter/handcart because CITY-01 grants no cart crossing over X6/X1 |
| rural/valley edge → market/civic core | `W.VEGA → W.CM_NE → W.X2 → W.SHOP → W.PLAZA` | 6.5 min target |
| follow NPC across ≥2 district boundaries | `E.HOME → X3 → Barrio Alto → Ribera → landing → X7 → Puerto` | crosses Ensanche→Wedge→Orilla sur and exposes multiple decision points |
| closure forces plausible alternate | close X2: X4 is ordinary-water road detour; X3 is permanent pedestrian fallback | route remains usable; closure matters rather than becoming cosmetic |
| late actor chooses faster but contextually different route | State 1 Casco→quay: X6 route = **6.75 + W_ferry**; X1/camino = **8.5** | when expected ferry wait exceeds ≈1.75 min, X1 becomes faster despite longer distance |
| quiet evening route differs from market-day flow | low water: Ensanche→Casco may use X5/lower lanes instead of X2→Plaza | quiet shortcut is conditional and never used as permanent proof; at ordinary water evening route reverts to permanent choices |
| service/back route differs from obvious public route | `W17` Ribera↔shop rear/service connector | supports deliveries without turning the back route into a universal public shortcut |

The following/search scenario deliberately spans more than one boundary and more than one route family. It is not a corridor test.

---

## 9. Chokepoint and alternate-route ledger

### C1 — X2 Puente del Mercado: practical municipal Arroyo closure lever

**Decision:** X2 is the primary municipal closure lever requested by CITY-00 Q5.

Why X2 rather than X3/X4/X5:

- it is permanent and road-capable;
- it sits on ordinary Ensanche↔commercial movement;
- closing it changes both pedestrian and goods routing;
- it has differentiated alternates instead of disconnecting the pedestrian city.

Consequences:

- ordinary-water pedestrians can use X4; the permanent foot fallback is X3;
- carts/service detour upstream via X4 while X4 is available;
- if flood removes X4 while X2 is closed, cart crossing of the Arroyo pauses, but pedestrians remain connected through X3;
- X5 may shorten a low-water trip but is never credited as permanent redundancy.

Planning costs from `E.HOME` to Plaza:

- X2 normal: **4.0 min**;
- X2 closed, X4 available: **≈7.8 min**;
- permanent X3 fallback: **≈9.0 min**.

A municipal closure therefore matters without turning the city into a disconnected hub-and-spokes layout.

### C2 — X6 ferry hours/fare/high water

X6 is a territorial mobility lever. A closure/suspension redirects Wedge↔Puerto pedestrian movement through X1 + camino sur. For Casco→quay the base choice is:

- X6 direct: **6.75 + W_ferry**;
- X1/camino: **8.5 min**.

That produces a real expected-time decision instead of treating the ferry as decorative scenery.

### C3 — X1 Puente Viejo closure

Pedestrians keep Orilla-sur access through X6 (State 1) or X7 (State 2). In State 1, losing X1 also removes the only CITY-01-approved handcart Río crossing; port loads that still cross X6 do so as porter/carryable-load movement. No new ferry capacity is invented to hide that consequence.

### C4 — X7 closure in State 2

Pedestrians/handcarts fall back to X1. **Cart freight across the Río waits** until X7 reopens, exactly as CITY-00 already requires. The pedestrian city remains connected; freight availability is a real consequence.

### C5 — Plaza/event footprint closure

Removing `W.PLAZA` from the public graph does not disconnect it. The upper route `W.CM_NE ↔ W.BARRIO`, east stairs `W11`, low route `W08` and Cuesta `W06` provide a long but physical bypass. `W17` is not needed for this proof and therefore cannot silently turn a service route into the anti-hub oracle.

### C6 — X4/X5 weather availability

X4 and X5 are useful variability, not foundations:

- X4 provides road redundancy while available;
- X5 provides a low-water pedestrian shortcut;
- permanent pedestrian cross-Arroyo floor remains X2 + X3.

---

## 10. Ordinary movement by district role

- **Casco / Plaza / Calle Mayor:** dense primary pedestrian movement, but not a universal connector.
- **Barrio Alto:** slower high-route movement, real stair penalty, strong following/search choices.
- **Ensanche:** ordinary residential origin with both road and foot crossing choices.
- **Ribera/Talleres:** secondary work corridor plus the service/back connection to Calle Mayor.
- **Puerto/Entrada:** ordinary work, freight and arrival movement; not mission-only scenery.
- **Vega:** quiet rural origin that still has a credible 6–7 min market trip and 15–17 min cross-city trip.

This satisfies CITY-00's “elsewhere in town” intention without increasing city acreage: some ordinary trips are short, while the longest representative trips are long enough for an actor to be genuinely elsewhere.

---

## 11. CITY-04 measurement handoff

CITY-04 must measure/falsify, not merely reproduce, the planning graph.

At minimum record:

1. actual route length and traversal time for every §7.1 inherited benchmark;
2. effective speed on E0/E1 lanes versus E2/E3 steep/stair segments;
3. total level change and direction-sensitive time on `W11` Barrio Alto↔Ribera;
4. whether `W06` Cuesta and the landing approach remain readable/followable without making the river crossing visually trivial;
5. actual X2-closure penalty through X4 and X3;
6. actual State-2 Vega↔Puerto and Ensanche↔Puerto times;
7. whether an X1 route can plausibly beat X6 once ferry wait is introduced, without scripting the answer;
8. bicycle roll/dismount points and whether the Calle Mayor preference remains meaningful;
9. service-cart geometry on the State-2 `O02→X7→W08→W17` delivery route;
10. arrival transition from `O.ENTRADA` into pedestrian movement;
11. followability over `Ensanche → X3 → Barrio → Ribera → landing → X7 → Puerto`, including decision points and sightline loss;
12. the Plaza-node removal test using only public edges, with `W17` excluded.

### Material falsification conditions

CITY-04 should reopen the relevant planning boundary rather than rationalise the result if:

- the representative ordinary long walk cannot land inside the inherited **13–17 min** envelope without distorting geography;
- a required alternate route is physically unrealizable at the accepted landmass/crossing layout;
- X2 closure either becomes nearly costless or functionally disconnects normal pedestrian movement;
- the Barrio Alto↔Ribera vertical route is so slow/opaque that following/search through it is not credible;
- service delivery requires turning a foot-only crossing into a road crossing;
- a claimed public Plaza bypass only works by using `W17` or another restricted service edge.

Small segment-level differences are expected and should update measured costs in CITY-04 rather than being treated as CITY-01 failure.

---

## 12. Residuals and downstream ownership

CITY-01 leaves the following intentionally open:

- exact metres, slope percentages and stair counts — `CITY-04` measurement;
- exact location programme / A–D systemic importance / S0–S4 spatial depth — `CITY-02`;
- final building/service-door placement — later CITY work;
- exact retained seed and which Arroyo crossing enters it — `CITY-03`;
- State 1→State 2 production timing — still unassigned here;
- live schedule choice, beliefs, route-planning AI and social consequence — Living World/runtime owners;
- bus service simulation and vehicle handling — outside CITY-01.

CITY-02 may place locations against these anchors and route families. It may not create a new crossing or silently reinterpret a route as public/road-capable merely to make a location programme convenient.

---

## 13. CITY-01 contract closure

The accepted geography now has a reviewable movement model with:

- one explicit semantic edge ledger;
- no hidden inter-landmass edges;
- differentiated primary/secondary/quiet/service routes;
- profile-specific access and verticality;
- preserved inherited walk-time bands;
- explicit costs for the CITY-00-deferred Ensanche↔Puerto and Vega↔Puerto routes;
- two nontrivial alternate-route families (Arroyo closure and Río crossing substitution) plus Plaza removal;
- a meaningful municipal closure lever;
- a State-1 ferry wait tradeoff and a State-2 cart-routing consequence;
- concrete measurement questions for CITY-04.

All costs remain hypotheses until measured locally.
