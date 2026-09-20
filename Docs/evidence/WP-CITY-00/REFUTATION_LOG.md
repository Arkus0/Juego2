# WP-CITY-00 — Refutation log

WP: `WP-CITY-00`
Purpose: select a preferred constitution only after materially different alternatives have been
attacked. Original charges were written before defences and before selection.

Cycle-4 note: independent review later found a planar defect in the **implementation of selected B**.
That defect is recorded in the post-selection appendix; it does not pretend to have been one of the
preference-forming charges.

---

## Charges

### Against A — Dos Orillas

**A-1 — Density is split at exactly the wrong scale.** At the corrected 0.30–0.45 km² band, A
produces two half-towns; CITY-02 either duplicates systemic anchors or leaves one bank thin.

**A-2 — The funnel persists through the early production window.** Until a second crossing exists,
every cross-river trip uses Puente Viejo; a road bridge is the most expensive piece to defer.

**A-3 — No representative cheap seed exists.** One-bank seed omits too much; a straddling seed must
build both bridgeheads and a crossing.

**A-4 — No cheap frequent crossing.** Governance is powerful but blunt unless a light crossing is
added early.

### Against B — Cuña de Confluencia

**B-1 — Port risks becoming a mission appendix.** A downstream work district fails if freight/story
are the only reasons to go there; ordinary movement must reach it structurally.

**B-2 — The terrace can silently become the only through route.** Paseo and high lanes are costlier to
build; without an invariant, production pressure can collapse B into a single-spine town.

**B-3 — Three levels and two watercourses are hardest to read/build.** CITY-04 can still falsify the
verticality/readability assumptions.

**B-4 — Navigability premise was invented in the first draft.** CITY cannot require an ART/setting
fact that ART never granted.

**B-5 — Tip concentrates expensive fabric.** Casco, plaza, historic bridge, mirador and port-facing
crossing head all load the hardest geometry into the retained start area.

### Against C — Ribera Larga

**C-1 — River is scenery by the WP movement test.** It shapes land use strongly but ordinary movement
rarely crosses it.

**C-2 — One axis collapses follow/search branching.** Direction remains obvious even when route
character varies.

**C-3 — Governance loses small crossing levers.** Terminal crossings make decisions coarse.

**C-4 — It sits nearest the "four streets plus scenery" negative gate.** Not a literal failure, but
the smallest drift reaches it.

---

## Defences

### For A

A is the strongest literal river-boundary city. A cheap pasarela can answer the early funnel and
provide a granular governance lever. It cannot answer the representative-seed problem at the selected
small scale.

### For B

**B-1 is answered structurally.** Puerto + Entrada share the Orilla-sur downstream bank. The valley
road and south-bank camino give it ordinary dry traffic, while X6/X7 gives a direct core crossing.
At least two non-port services at the Entrada/Puerto junction are a permanent obligation (CSI-04).
The port therefore does not need an impossible dry Wedge continuation to be ordinary.

**B-2 is answered by a design invariant.** Low, middle and high route families remain designed; the
low family reaches the port through the named landing-side crossing and may be weather-interrupted in
State 1 without disappearing from the design.

**B-3 is partly inherited setting cost.** Verticality already belongs to the setting/blueprint; the
amount of level change remains a CITY-04 measurement risk.

**B-4 is answered by removing the premise.** Timber rafting only at high water, áridos, ferry use and
road break-bulk require no long-distance navigable corridor. CSI-05 forbids reintroducing one.

**B-5 is real but inherited by the retained-first strategy.** The blueprint already wanted the casco /
plaza / historic bridge area first.

### For C

C is cheapest, densest and easiest to phase. That cost argument is real and is preserved as a standing
constraint. It still does not satisfy the selected reading of the river/movement criterion as strongly
as A or B.

---

## Verdicts

### A — rejected, conditionally

A survives as a coherent city but loses on **A-1 + A-3 together** at the selected scale. Above roughly
0.7 km² dense fabric, reopen the decision because A's density objection weakens materially.

### C — rejected

C loses on the WP's movement reading of the river criterion and on the investigation/governance value
of a single dominant axis. Its production-cost discipline is carried into B's scale envelope.

### B — selected, with obligations

B was selected because its main charges can be converted into explicit structure rather than intent:

1. **B-1 → CSI-04:** Entrada/bus/road remains adjacent to Puerto and ordinary non-port services live
   there.
2. **B-2 → CSI-02:** three designed longitudinal route families remain.
3. **B-4 → CSI-05:** landing needs no unauthorized long-distance navigability premise.

B-3/B-5 remain declared risks rather than hidden assumptions.

---

## Post-selection review findings

These did not participate in the original preference ordering. They are recorded because they changed
the selected constitution after independent review.

### FAIL 1 — unauthorized landing premise

Candidate `b9473f0…`: the landing was conditional on a setting fact CITY did not own. Repaired by
removing the premise and correcting CITY→ART authority.

### FAIL 2 — barca added without reconciling topology

Candidate `6091584…`: adding the second Río crossing left stale closure consequences and graphs.
Repaired by naming two states and reconciling the crossing edge everywhere.

### FAIL 3 — false 2→1→0 summary

Candidate `daefc6a…`: prose denied the single-crossing state directly shown by the table. Repaired by
separating design invariants from availability and counting explicitly.

### FAIL 4 — impossible planar embedding

Candidate `d2d5f7a…`: the authoritative matrix placed the Puerto downstream of the confluence while
also treating it as a dry continuation of the Wedge. A wedge between two channels that meet at its
point ends at that point; the claimed dry Cuesta/sirga continuation was impossible.

**Causal repair:** `PLANAR_EMBEDDING.md` fixes the landmasses before route semantics. Puerto + Entrada
are now on the Orilla-sur downstream bank. X6/X7 crosses from the Wedge-tip landing head to Puerto;
X1 + camino sur is the other core↔port route. `CONNECTIVITY_MATRIX.md` is regenerated from that bank
choice.

**Effect on selection:** B survives. The repair changes port-bank placement and route semantics but
not the scale argument, landing rationale, two-state model or the reason A/C lost. It also strengthens
the intended territorial role of the Río because port access now visibly crosses it.

---

## What B does not get for free

B still does not beat C on build cost or A on raw barrier simplicity. Those losses remain live
constraints. If later measurement or programme invalidates B's scale/readability economics, reopening
the topology is preferable to stretching the constitution.