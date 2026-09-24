# Keeper City — Play-design layer

Version: 0.1 — 2026-09-24
Status: **PROPOSED — input to `WP-CITY-09`; non-binding until that WP passes independent review.**
Class: PRODUCT / GAME-SPACE DESIGN (NON-FOUNDATIONAL)

This document does not modify any accepted CITY document. Where a proposal needs an accepted owner to change, it says which one and why. It authorizes no Unity work, gameplay, Living World semantics or asset adoption.

Figures:
- `Docs/production/figures/city_seed_play_layer.svg`: exact CITY-03 geometry + indicative proposals.
- `Docs/production/figures/city_seed_fabric.svg`: the same seed with illustrative villa fabric (closed blocks) and the non-playable envelope beyond.

---

## 0. Why this layer exists

Most of the game (≥60% of play time) happens in this town. The accepted CITY chain (`CITY-00..03, 05, 06`) makes the town **true**: coherent geography, honest access, truthful causality, selective depth, a small retained seed. That is necessary and should be kept.

It is not sufficient. No accepted owner is responsible for the town being **fun, comfortable and memorable to move through**. Game logic — loops, shortcuts, detours, landmarks, pacing, staged spaces, events — currently has no home, and `CITY_PRODUCT_SEED.md` §13 rightly says that "attractive shortcuts" cannot authorize drift *on their own*. This layer gives them an owner.

Guiding line:

> **The town must be true when nobody is looking and fun when someone is.**

Realism is the raw material; game logic is the form. Every proposal below remains usable by NPCs off-screen (no player-only teleports), preserving CITY invariant 9 ("player absence matters").

---

## 1. Diagnosis of the accepted seed (player's-eye)

Facts taken from `CITY_PRODUCT_SEED.md` and `CITY_MOBILITY_TOPOLOGY.md`:

| # | Finding | Evidence | Player consequence |
|---|---|---|---|
| D1 | **Hub-and-spokes.** All seven represented edges radiate from `W.CASCO` or chain from it. The only loop is the node-local `casco.micro.A/B` split. | seed §3.1, §3.3 | Travel is out-and-back. Every trip retraces its steps. |
| D2 | **Spokes end in seams.** Landing, X5/Ensanche stub, O.X1 stub and the W.SHOP commercial cut are all termini. | seed §2.8, §7 | Four dead ends with no declared payoff. |
| D3 | **No vertical landmark in the seed.** The only programmed landmark shell (`fam.calle.landmark_shell`) sits on Calle Mayor, outside the seed. §10.3 tests orientation but nothing supplies it. | seed §10.3; location programme `fam.calle.landmark_shell` row | Players orient by memorizing corners, not by looking up. |
| D4 | **The paseo/sirga is cut but not represented.** CITY-00 makes the low paseo "continuous to the confluence-tip landing head" (`W08`). The seed spans that riverside alignment but only represents the bridge approach. | constitution route-family table (`Paseo / sirga`), mobility `W08`, seed §2.9 | The most pleasant, most "postcard" route in the seed is missing, along with the loop it would close. |
| D5 | **Only fiction time exists.** Costs are realistic walking minutes (plaza↔bar 1.75 min; city end-to-end 15–17 min). | mobility §5.1, §7 | Correct for NPC schedules. Nothing sets targets for **player** traversal time or feel. |
| D6 | **Scenarios validate truth, not situations.** `SCN-01..13` prove access/topology. No space is designed as a stage for chases, duels, overlooks, conversations, festivals or martial practice. | seed §9 | Situations will have to be forced onto spaces that were never shaped for them. |

Strengths to keep: the two-waters geography ("the Arroyo shapes daily movement; the Río shapes territorial movement"), the conditional X5 ford (already a great dynamic route), the bar as a central S4/I3 hero, quiet vs. busy contrast, and small retained scope.

---

## 2. Pillars

1. **Cómoda (comfortable):** moving around is never a chore; no frustration from invisible walls, ambiguous doors or backtracking.
2. **Legible:** you always know roughly where you are without a map.
3. **Enredada (interconnected):** loops, shortcuts and detours make the town feel like a place you learn, not a set of corridors.
4. **Viva (alive):** the town changes by hour, day, weather and season, and those changes alter how you move through it.
5. **Memorable:** a handful of images and places the player will carry with them for years.

---

## 3. Design rules (testable at CITY-04)

Each rule carries a proposed CITY-04 check. Thresholds are **hypotheses to be calibrated in greybox**, not dogma.

### Movement structure

**PD-01 Loops over spokes.** Every public anchor sits on at least one loop that does not retrace a street. The seed has **≥3 city-scale public loops** (not node-local). *Check:* loop list + jog-time per loop; each loop ≤ ~90 s at player jog.

**PD-02 No dead end without a payoff.** Every terminus rewards the walk: a view, a bench, a stage, a fixed NPC post, an object, a discovery substrate. *Check:* dead-end audit table, one payoff per terminus.

**PD-03 Shortcuts are earned and truthful.** Shortcuts come from world logic: a gate you open from the inside, a wall you can drop from but not climb, a ford that exists only at low water (X5 already does this), a ferry with hours. They are real graph edges with access/state, usable by NPCs under the same rules. *Check:* each shortcut has an edge ID, state rule and NPC eligibility.

**PD-04 Designed detours (rodeos).** When a route closes (high water, repairs, a market, a festival), the detour is designed to pass something worth seeing and to take ≤ 2× the normal time in the seed. *Check:* for each closable edge, the named detour and its time.

### Orientation

**PD-05 Landmark hierarchy.**
- **L1 (whole town):** one vertical landmark visible from ≥ ~80% of public seed ground.
- **L2 (per district):** a signature per district: material, colour accent, sound, activity.
- **L3 (local):** fountain, tejo, bar sign, bench, stele. Things you say "turn at".
- **Far compass:** Picos peaks on one side of the skyline; **"downhill = river"** holds everywhere in the Wedge.
- **Sound compass:** river noise grows toward the Río; bells come from the L1 landmark.

*Check:* blind-spawn test: from 10 random public points, a tester points to the landmark and to the bar within 5 s.

**PD-06 Framed vistas.** Each spoke/loop has at least one street end or opening that frames the landmark, the confluence or the Picos. *Check:* vista list with camera positions.

**PD-07 Door grammar.** Enterable vs. not-enterable is readable at 15 m and consistent across the whole town. Proposed: enterable = open or ajar door + warm interior light (`#E8B26A`) + sign or object at the threshold; closed fabric never shows that combination. *Check:* at 15 m, testers classify 10 frontages without error.

### Pacing and comfort

**PD-08 Two clocks.** CITY-01 minutes stay as **fiction time** (NPC schedules, routines). Player feel is measured separately in **player time** at walk/jog. Proposed targets for the seed: bar (home base) → any seed anchor ≤ ~45 s jog; each loop ≤ ~90 s. Any time compression between the two clocks is a later H2/PA decision; this layer only asks that both are measured and named. *Check:* both clocks recorded per represented edge.

**PD-09 Rhythm.** Something worth noticing roughly every 25 m (≈5–8 s at jog). Alternate compression and release: narrow lane → plaza reveal; Cuesta → confluence reveal; bridge → town postcard. *Check:* "boring meter": the longest stretch of public route with nothing new to notice.

**PD-10 Comfort.** Natural boundaries only (water, walls, fences, closed gates; never invisible walls). Low steps and kerbs never block movement. Stairs read at a glance. No trap ledges. Seats and shelter at regular intervals. Town playable without a map overlay. *Check:* collision/boundary audit list.

### Situations and life

**PD-11 Staged spaces.** A small catalogue of reusable stage types, each with ≥2 approaches, a vantage, an exit, props at hand height and a lighting intent:

| Stage | Purpose | Spatial needs |
|---|---|---|
| **Duelo** | face-off, confrontation, restraint | narrow linear span, railing, audience on both ends |
| **Plaza** | market, festival, speech, crowd | open floor + edges + arcade + balcony vantage |
| **Persecución (chase)** | chase, escape | stairs, turns, props (laundry, crates, carts), one drop |
| **Seguimiento (tail)** | follow/search | branching lanes, covered passage, reconvergence |
| **Conversación** | private talk | semi-private nook, seating, sound shelter |
| **Observación** | watch, plan, overlook | elevated vantage with partial cover |
| **Práctica** | martial practice, bolos, training | open level ground, boundary, dawn light/fog |
| **Refugio (home base)** | return, rest, gossip | the bar: central, warm, many relations |

**PD-12 Film grammar for martial physicality.** Stages (not ordinary streets) offer choreography partners: level changes of 1–2 m, railings, narrow spans, stalls, ladders, carts, doorways. This gives the PA-09 graded-physicality research (restraint, de-escalation, sparring, intervention) spaces made for it. Destruction is not the default; readable props are.

**PD-13 Event layers (spatial support only).** Every event must change how the town is crossed or looked at, at least locally:
- **Daily:** dawn fog, bells, bar rushes, shop openings.
- **Weekly:** market day (Potes' real market is on Mondays) occupying S01, with its own detours.
- **Seasonal:** village fiesta; an orujo festival (Potes has a real one in November); snow on the Picos.
- **Hydro/weather:** high water closes X5 (already accepted), fog, rain.
- **Civic:** temporary closure of a route (repairs, procession) with a designed detour.

This layer reserves footprints, stage points and closure points. Schedules and runtime belong to PA/H2 owners.

### Memory

**PD-14 Postcards.** The seed guarantees at least five composed views. Each is a named camera position the art pass must protect:
1. From the far bank at the bolera: houses over the river, the bridge, the tower, the Picos. **The game's hero image.**
2. The old bridge in dawn fog.
3. Market day under the tower.
4. The X5 stepping stones by the lavadero at low water.
5. The confluence from the landing: two waters meeting, the ferry post, the Puerto hinted downstream.

**PD-15 Hometown attachment.** Recurrence builds affection: fixed NPC posts (the old man on the same bench), shop owners who greet you, visible change after your actions (SCN-11 substrate), seasons. Spaces reserve those posts.

### Town-ness

**PD-16 Villa, not aldea.** The town must read as a dense Cantabrian *villa* (market town with urban pretensions), not a scattered hamlet. The accepted seed names only 8 programmed frontages (F01–F08); the ordinary fabric around them (`fam.casco.houses`, `fam.plaza.arcades`, `fam.calle.mixed_frontages`) is committed but not quantified, and **that fabric is what makes it read as a town**. Rules:

- **Closed blocks (manzanas), not detached houses.** In casco, plaza and Calle Mayor, buildings share party walls and form continuous street walls; the inside of the block holds patios, huertos and corrals. Hypothesis: ≥85% of public frontage length in the core is built wall; gaps are deliberate (a passage, a court gate, a view).
- **Narrow lots, many doors.** Lot frontage 5.5–9.5 m, so a 60 m street shows 7–10 doors. The rhythm of doors, balconies and signs is what says "town". Illustrative fill of the seed at this grain gives **≈280 houses** in the playable envelope (~46% of dry land built). CITY-00's coarser estimate (≈140 at ~90 m² footprints) is the same fabric counted at a bigger lot size.
- **Height gradient.** 3–4 storeys around the plaza and Calle Mayor (with soportales and solanas), 2–3 at the edges, 1–2 on the far bank. Height varies ±1 storey every 2–3 lots so roofs step, never a flat cornice line.
- **Villa signifiers across the full city** (not all in the seed): church with belfry, tower house/town hall, arcaded plaza, covered market, casino or cultural circle, small cinema, music kiosk, pharmacy, bank branch, bus stop/station, school or instituto, Guardia Civil post, Ensanche blocks with glazed *galerías*. These are what separate a *villa* from an *aldea*; CITY-02 should check that its programme covers them.
- **The town continues past the edge.** The soft envelope (CITY-03 §8) must show continuous roofscape on every seam: Ensanche blocks across the Arroyo, Barrio Alto climbing the slope, Calle Mayor continuing, Orilla-sur/Puerto sheds downstream. The player should never see the town "end" from inside the seed.
- **Production unit = block, not house.** 280 houses are affordable because they are authored as a small set of reviewed façade-bay assemblies (CITY-05 composition ladder) instanced along block perimeters, not 280 unique buildings. Only F01–F08 and landmarks are bespoke.

*Check (CITY-04):* built-frontage ratio per represented street; door count per 50 m; storey histogram; skyline screenshot from bridge, plaza and landing showing no visible "end of town".

---

## 4. Application to the seed

Positions are indicative. See the figure. All proposals stay on existing dry land. **No new water crossing, landmass change or seed-envelope change**; CITY-00 is untouched.

### P1 — La Torre (L1 landmark) at F02

Make the civic anchor `loc.plaza.ayuntamiento` (F02, 18×22 m) a **tower house**: a stone tower of roughly 18–22 m with the town hall in it. Potes' own Torre del Infantado is the real reference. It is visible from the X1 bridge, the landing, the X5 ford and the commercial seam. The tower top is an I2 civic extension and the town's best vantage (Observación stage).

*Owner impact:* CITY-05 one-off landmark variant of `bf.civic` (one-off promotion rules §6.3/§13); CITY-06 I2 extension note. The place, access roles and depth class stay the same.

### P2 — Paseo del río + Escaleras del mercado (two loops)

- **Represent the seed portion of the accepted paseo/sirga (`W08` alignment)** along the Río bank, from `W.LANDING` past the X1 bridgehead toward the upstream seam. This is an accepted CITY-00/01 route that the seed cuts through without representing (D4). It also creates a real `W.RIBERA` seam.
- **Escaleras del mercado:** a public stepped connector (E3/AF) from the paseo up to the market/plaza lower edge.

Resulting loops:
- **L-A:** Casco → Cuesta (W06) → Landing → Paseo → Bridgehead → W12 → Casco.
- **L-B:** Casco → W12 → Bridgehead → Paseo → Escaleras → Plaza → W05 → Casco.

*Owner impact:* CITY-01 edge-ledger amendment (represent the W08 portion, add bridgehead and escaleras junctions, add the escaleras edge); CITY-03 amendment (walkway authorization inside `nb.rio_bank`; a walkway is not a building shell, but the seed rule currently forbids waterside walking without an owned route).

### P3 — Pasadizo + atajo de bajada (shortcuts)

- **Pasadizo:** a covered passage under a house (typical in Potes) from the W13 lower lane to the plaza's north edge. It creates **L-C** (Casco → W13 → Pasadizo → Plaza → W05 → Casco) and a second city-scale Casco↔Plaza route, so follow/search stops depending on the node-local micro-loop alone.
- **Atajo de bajada:** a one-way drop from the Cuesta to the paseo: a wall with a gate that is opened from below once. It is shortcut-by-world-logic (PD-03), usable by NPCs once open.

*Owner impact:* CITY-01 (new edge, new one-way/state access notion); CITY-05 (passage assembly on `bf.house` old-row).

### P4 — A payoff at every seam

| Terminus | Payoff |
|---|---|
| `W.LANDING` | confluence mirador, bench, ferry bell/post for future X6 (postcard 5) |
| X5 / `E.X5` | stepping stones + lavadero; the X5 availability state is itself the event (postcard 4) |
| `O.X1` | bolera + the town postcard (P5) |
| W.SHOP commercial cut | visible continuing street life (awnings, sound, people), never a blank end |
| Paseo upstream seam | the towpath continuing toward Ribera, with work noise |

### P5 — Bolera del Puente (Orilla sur)

A **corro de bolos** on the far bank next to the X1 landing (dry Orilla-sur land, roughly 24×8 m, outside the 3 m bank shoulder). It gives the far bank a reason to exist. It is the **Práctica** stage (bolos by day, forms in the dawn fog) and the place from which the game's hero postcard is seen.

Bolos are ordinary Cantabrian life, not a martial opportunity. If the owner later chooses to use it as the seed's single martial opportunity, that stays within `CITY_INTERIORS_DISCOVERY.md` §11.

*Owner impact:* CITY-02 new B/S2/I0 place; CITY-03 new open-site region `S04` on Orilla sur.

### P6 — Mirador de la Cuesta

A small widening at the top of the Cuesta overlooking the confluence and the paseo. It serves as an Observación stage and a rest point.

### P7 — Stage map

| Stage | Seed location |
|---|---|
| Duelo | X1 old bridge |
| Plaza | plaza + market S01, under the tower |
| Persecución | Cuesta (W06) + atajo de bajada |
| Seguimiento | Casco micro-loop + Pasadizo |
| Conversación | S03 shared court; bar terrace; landing bench |
| Observación | tower top; Mirador de la Cuesta |
| Práctica | Bolera del Puente |
| Refugio | the bar (F01), already central |

### Before / after (seed)

| Metric | Accepted seed | With P1–P7 |
|---|---|---|
| City-scale public loops | 0 (only node-local micro-loop) | 3 (L-A, L-B, L-C) |
| Dead ends without payoff | 4–5 | 0 |
| L1 landmark | none | Torre |
| Designed stages | 0 | 8 types placed |
| Ordinary fabric in the playable seed | unquantified (8 named slots) | ≈280 houses in closed blocks (illustrative), continuous street walls |
| Composed postcards | 0 | 5 |
| Casco↔Plaza public routes | 1 (W05) | 3 (W05, Pasadizo, Paseo+Escaleras) |
| New water crossings | — | **0** |

---

## 5. Proposed additions to CITY-04 human runs

Added on top of the existing nine required runs:

1. **Blind orientation:** 10 random public spawns; point to the tower and the bar within 5 s.
2. **Loop runs:** time L-A, L-B, L-C at walk and at jog (both clocks).
3. **Boring meter:** the longest public stretch with nothing new to notice.
4. **Dead-end audit:** every terminus and its payoff.
5. **Chase run:** Cuesta → atajo → paseo, with a proxy pursuer.
6. **First-visit run:** a tester who has never seen the map finds the bar, the plaza and the bridge without an overlay.
7. **Door grammar:** classify 10 frontages at 15 m.
8. **Postcard check:** the five camera positions exist and read as intended in greybox massing.
9. **Detour check:** close X5, then W05, then the market; walk the designed detours.

---

## 6. Non-goals

- No change to CITY-00 geography, landmasses or crossings.
- No NPC AI, schedules, dialogue, quest scripting or combat rules.
- No time-compression decision (flagged for H2/PA).
- No final art. The Bible §13 register and the casona montañesa variant are consumed, not produced here.
- Not a license for "secret in every building" (CITY-06 anti-inflation rules still apply).
