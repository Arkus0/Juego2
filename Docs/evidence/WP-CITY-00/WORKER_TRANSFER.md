# WP-CITY-00 — Worker transfer record

WP: `WP-CITY-00 — Keeper City spatial constitution + scale envelope`
Contract: `Docs/workpacks/CITY/WP-CITY-00.md`
Baseline SHA: `290f92e9c21f1e454e0d6924b29f4d778b9875f8`
Outgoing Worker: `Claude Code — session 01KC1S5dCRLuMq6qeht4n34L`
Receiving Worker: `ChatGPT GPT-5.6 Sol — transfer Worker`
Transfer SHA: `d2d5f7a7277d8949264d62e6ac2ecbbb171aae9b`
fail_cycle at transfer: **4**

> **HISTORICAL TRANSFER RECORD — no longer the current-state authority.**
> The receiving Worker accepted ownership on PR #65 while it was Draft and unfrozen. Current Worker
> state is recorded in the PR body and, after the repair pre-review, `HANDOFF.md`. This file preserves
> what was transferred and must not be read as saying the Worker is still PAUSED or FAIL 4 is still
> unrepaired.

---

## 1. State that was transferred

At handoff:

- PR #65 was **Draft**;
- outgoing Worker had stopped;
- no freeze was active;
- prior Worker pre-review was invalidated by transfer;
- last reviewed candidate was `d2d5f7a7277d8949264d62e6ac2ecbbb171aae9b`;
- independent verdict on that SHA was **FAIL**;
- all failed candidates remained reachable by SHA;
- `fail_cycle` was **4** and was not reset.

The branch later received a documentation-only transfer commit after the failed candidate; that does
not change the binding `Transfer SHA` above or erase review history.

---

## 2. Four independent FAILs transferred

### FAIL 1 — `b9473f04da72462619e9561c097083afdc6c436d`

The selected landing depended on an ART/setting premise CITY itself admitted was invented: a loaded
long-distance navigability claim. The same defect reversed CITY→ART authority.

**Carried repair:** landing now rests on timber rafting only at high water, áridos, ferry use and road
break-bulk inside the already-granted small-working-landing direction. CITY consumes ART; it does not
bind it.

### FAIL 2 — `6091584313488bcfb1840132f22adcc9b1b5de4f`

Adding La barca as a second Río crossing left stale graphs and closure consequences that still acted as
if Puente Viejo were the sole access.

**Carried repair:** two named connectivity states, ferry/bridge present in graph and table, closure
consequences reconciled.

### FAIL 3 — `daefc6a1e4959c1cffacd662f5578a4c13a642d3`

A summary claimed the south bank was never on one crossing although the row above showed exactly that,
and misdescribed the double-failure state. CSI-06 carried the error.

**Carried repair:** explicit State-1 2 → 1 → 0 availability ladder; connectivity invariants bind
design rather than weather-dependent availability.

### FAIL 4 — `d2d5f7a7277d8949264d62e6ac2ecbbb171aae9b`

The circuit-breaker matrix itself was physically wrong. It said:

- Río from NE + Arroyo from N meet at the south tip of the Wedge;
- city occupies the Wedge between them;
- Puerto lies downstream of that confluence;
- yet Puerto was dry-continuous with the Wedge via Cuesta and sirga.

A Wedge between two channels ends where those channels join. Its interior cannot pass through the
confluence and reappear downstream as dry land.

**Minimum correction boundary transferred:** choose a real downstream bank for Puerto, name physical
Casco/Ribera/Entrada connections, enumerate every crossing, then regenerate port approaches,
L1/L1′, route counts, seams and affected invariants.

---

## 3. FAIL 4 repair adopted by receiving Worker

The receiving Worker chose the minimal planar correction documented in `PLANAR_EMBEDDING.md`:

- Wedge ends at the confluence;
- Ensanche remains outside the Arroyo;
- Orilla sur continues onto the south/east bank of the joined river;
- **Puerto + Entrada occupy that Orilla-sur downstream bank**;
- X1 Puente Viejo remains the historic core↔Orilla-sur crossing;
- X6 La barca, replaced by X7 Puente del Muelle, connects the Wedge-tip landing head to the **upstream
  Puerto bridgehead**;
- the port district extends downstream from that bridgehead;
- X1 + camino sur is the second core↔Puerto route.

Consequences:

- no hidden dry Wedge→Puerto edge remains;
- the crossing set stays seven IDs across the constitution's life / six per state;
- the 2 → 1 → 0 State-1 Río availability ladder survives;
- L1/L1′ are planar;
- seam 5 moves to port-bank growth on Orilla sur;
- seam 6 remains the X6→X7 state transition;
- Topology B survives as a corrected topology rather than as the impossible prior embedding.

Current semantics are owned by `CONNECTIVITY_MATRIX.md`; this transfer file does not supersede them.

---

## 4. Work deliberately not redone

The transfer explicitly preserved, unless the planar repair falsified them:

- the three materially different topologies and their pre-selection refutation;
- the scale correction: ~0.30–0.45 km² dense fabric and ~0.03–0.06 km² retained seed;
- the landing's setting-safe reason for existing;
- the two connectivity states and 2 → 1 → 0 ladder;
- design-vs-availability wording discipline.

The bank correction did **not** falsify those results. Comparison/refutation surfaces were reconciled
only where the port-bank change affected their wording.

---

## 5. Protocol continuity

- transfer did not reset `fail_cycle`;
- no prior FAIL was erased;
- the outgoing Worker remains in Worker history and cannot become the independent Reviewer of its own
  candidates;
- the receiving Worker must run a new strict pre-review after all mutations;
- no independent review is valid until the PR body records an exact new Frozen candidate SHA and the
  PR is Ready;
- CITY remains non-foundational and PROCESS_ONLY; synthetic CI is not semantic proof.

`WORKER_PLAN.md` is the receiving Worker's active plan and predecessor-contract check.