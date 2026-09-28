# WP-CITY-URBAN-00 — planning evaluation

Status: DRAFT Worker evidence. Source: `Docs/production/CITY_PORT_TOWN_TRANSITION.md` on this candidate branch. This is a planning consistency and third-person route review, **not** measured port geometry or a Unity playtest.

## Scope and method

1. Compare the new graph and site programme with the exact accepted inland sources listed in `PREDECESSOR_CONTRACT_CHECK.md`. Check that no inland river crossing, route weight, F/S parcel or place ID is reused as port-town authority.
2. Enumerate the proposed B0 public edges from the document and run a stdlib Python adjacency/reachability calculation. Use the proposed node coordinates only for straight-chord lower bounds; no chord is claimed to be a buildable street or measured route.
3. Walk the route mentally at third-person camera height, node by node, asking what the player can see or choose. Name the exact Unity questions left for URBAN-01.

## Symbolic consistency result

The check used nodes `L(30,105), A(75,100), S(115,105), E(160,100), P(160,40), Q(105,35), O(105,65), V(55,65)` and exactly the ten `B01..B10` endpoint pairs in §4. Python 3 with `collections.deque` checked every consecutive route pair against that ledger, all nodes north of the illustrative shore `v=0`, reachability, and the closure cases.

```text
direct: L-A-S-E-P; chord lower bound 190.9 m
quiet: L-V-O-E-P; chord lower bound 222.4 m
chase: E-P-Q-O-E; chord lower bound 210.4 m
detour_B04_closed: E-O-Q-P; chord lower bound 150.4 m
PLANNING_GRAPH_CONSISTENT: 8 dry nodes, 10 public edges, 2 stated cycles, closure truth checked
```

The first draft incorrectly said B04 closure disconnected P. The edge ledger itself showed E–O–Q–P remained open; §4 and §7 were corrected while Draft. The accepted planning claim is now: B04 closure removes the direct approach but a longer **pedestrian** detour remains through B07/B06/B05; B04+B06 closure disconnects P from the B0 public graph. This is a causal closure-truth correction, not a new edge.

The straight-chord numbers are lower bounds from the illustrative coordinates. Curved routes, obstacles, vertical travel, walking speed, pursuit and camera lag remain unmeasured. They are not CITY-01 planning weights, NPC schedule times or player traversal measurements.

## Third-person route review at planning fidelity

| Camera/path station | Required read/choice | Present in plan | Physical question for URBAN-01 |
|---|---|---|---|
| L → A | Lodging exit leads to everyday activity court and commercial strip | B01 and B10 provide two public departures; A is an activity patch beside an always-clear route | At player height, can the player see the court and understand the public route without UI markers? |
| A → S → E | Shop public door is identifiable; service/back door is distinct; port clue becomes directional | S is a destination on B02/B03, no through-shop edge; E is the port reveal turn | Does frontage/signage distinguish enterable shop from C/I0 closed doors at ordinary play distance? |
| E → P / E → O | Direct port descent versus upper observation approach | B04 and B07 are separate public edges | Are both branches visible and comfortably turnable with third-person camera and actual slopes? |
| P → Q → O | Waterfront observation and ascent without water/yard incursion | B05 public rail edge; B06 ascent; service yard excluded | Are water and controlled-yard limits unmistakable; is Q–O physically safe and readable? |
| O → E / O → V | Pursuit rejoin and quieter return choice | B07/B08 connect to E/V; no teleport or private-court path | Can player track a person without occlusion ambiguity, and does the chase loop have workable pace? |
| E/O → A/L return | Return to same shop/lodging surfaces after later state change | Public graph reconnects through S/A or V/L | Does a later reload/revisit expose the changed surface without changing spatial identity? Gameplay owns state. |

**Verdict:** planning route/access structure is internally coherent and exposes the WP's mandatory first-block affordances. It is **not** physically approved. The four critical B0 camera moments are E's branch/reveal, Q's water boundary, Q–O's ascent and S/P's public-versus-service thresholds. URBAN-01 must measure and inspect those on a keeper candidate. If physical proof cannot realize the specified cycle, shore separation or roles, it must report the causal CITY owner amendment rather than silently editing the plan.

## Source and authority check

- Accepted CITY-00/01 inland Wedge, X1..X7 and W/E/O IDs: absent from the new `U*`/`B*` edge ledgers; no inland crossing becomes coast.
- CITY-03 exact polygon, water masks, F01..F08/S01..S03: retained as inland seed only. B0 coordinates are explicitly illustrative; a new hard seed is assigned to CITY-03 ownership.
- CITY-04: accepted technical scene handoff, no human spatial PASS and no coastal proof.
- CITY-09: reviewed proposal vocabulary only; no P1–P7 adoption.
- Product scale: large port town with five *candidate* neighbourhoods; only Mercado and a Muelle seam are first-block inputs.
- Residency: graphical work limited to active B0 and needed seam; off-screen state remains abstract. No universal streaming or whole-town graphical-NPC requirement.
- Required future proof and allowed residuals are explicit in §§8–9 of the transition document.

## Residual risk and classification

| Finding | Classification | Owner |
|---|---|---|
| Illustrative shore/route chords may not yield comfortable or legal physical geometry | **Named future measurement**, not a planning PASS claim | CITY-00/01/03 urban addenda + CITY-URBAN-01 |
| Shop/lodging thresholds or public waterfront may be visually confusing with final assets | **Named future visual/physical decision** | CITY-05/06 urban addenda, ART-URBAN-01, CITY-URBAN-01 |
| NPC/chase/return could fail in gameplay despite spatial support | **Out of this WP boundary** | GC2/Arkus gameplay WPs and GC2-SLICE |
| Later neighbourhood parceling, final NPC totals and activities beyond B0 | **Allowed residual** | Later district/content owners |
