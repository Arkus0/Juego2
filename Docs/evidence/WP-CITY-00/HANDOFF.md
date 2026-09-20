# WP-CITY-00 — Worker handoff

WP: `WP-CITY-00 — Keeper City spatial constitution + scale envelope`
Contract: `Docs/workpacks/CITY/WP-CITY-00.md`
Baseline SHA: `290f92e9c21f1e454e0d6924b29f4d778b9875f8`
Branch: `claude/city-urban-topologies-ehn6qf`
Worker history: `Claude Code — session 01KC1S5dCRLuMq6qeht4n34L → ChatGPT GPT-5.6 Sol`
Transfer SHA: `d2d5f7a7277d8949264d62e6ac2ecbbb171aae9b`
fail_cycle: **5**

The exact candidate/freeze status is recorded in PR #65 after this file is committed. Evidence files
cannot safely name their own final commit SHA.

`WORKER_TRANSFER.md` remains the historical transfer record, not the current-state authority.

---

## 1. What review cycle 5 found

Independent review #5261646488 reviewed candidate
`bc9f1c850b2d9eedc9f3bb4393bb62a11ffcde35` and confirmed that the cycle-4 physical repair was real:

- Wedge ends at the confluence;
- Puerto/Entrada are on Orilla sur;
- X1 and X6/X7 are the only Wedge↔Orilla-sur crossing families;
- the connectivity matrix is internally consistent on that point.

The remaining blocker was narrower but still contractual: the **selected semantic ASCII graph** drew
the Ensanche branch down into a horizontal line continuing directly to `PUERTO FLUVIAL`. The selected
option dossier had the same visual implication.

Because those drawings are semantic graphs, that line implied a dry Ensanche-bank↔Orilla-sur edge
which the authoritative matrix explicitly does not define.

---

## 2. Cycle-5 repair

No topology or scale redesign was performed.

The two semantic graphs were redrawn around explicit landmass groups:

- `[ENSANCHE BANK]` contains Ensanche;
- `[WEDGE]` contains the historic/civic/commercial core, Ribera, Barrio Alto and Vega;
- `[ORILLA SUR]` contains Puerto, Entrada, camino sur and the outward road.

The graph rule is now explicit:

> **Only labeled connectors are graph edges.** Whitespace, alignment and grouping have no connectivity
> semantics.

Allowed inter-landmass edges remain exactly those in `CONNECTIVITY_MATRIX.md`:

- X2–X5: Ensanche bank ↔ Wedge over Arroyo;
- X1: Wedge/casco ↔ Orilla sur over Río;
- X6 State 1 / X7 State 2: Wedge-tip landing head ↔ upstream Puerto edge over joined Río.

There is **no Ensanche-bank ↔ Orilla-sur edge**. Any Ensanche→Puerto route must first cross the Arroyo
into the Wedge and then cross the Río using X1 or X6/X7.

---

## 3. What did not change

Cycle 5 does not reopen:

- selection of Topology B;
- cycle-4 planar embedding;
- Puerto/Entrada bank choice;
- seven crossing IDs / six coexisting per state;
- State-1 Río availability 2 → 1 → 0;
- four Arroyo crossings;
- L1/L1′ territorial loops;
- port approach families;
- six expansion seams;
- 0.30–0.45 km² dense-fabric scale envelope;
- 0.03–0.06 km² retained-seed band;
- CITY→ART authority direction;
- the landing rationale independent of long-distance navigability.

`CONNECTIVITY_MATRIX.md`, `PLANAR_EMBEDDING.md`, scale evidence and Production Blueprint therefore did
not need semantic changes.

---

## 4. Correct connectivity at a glance

### Landmasses

- **Ensanche bank** — across Arroyo from Wedge.
- **Wedge** — between Río and Arroyo; ends at confluence.
- **Orilla sur** — across Río; continues downstream and contains Puerto/Entrada.

### Crossings

- X1 Puente Viejo — Wedge ↔ Orilla sur;
- X2 Puente del Mercado — Wedge ↔ Ensanche;
- X3 Pasarela del Lavadero — Wedge ↔ upper Ensanche;
- X4 Puente de la Vega — Wedge ↔ north Ensanche;
- X5 Pasos/vado — Wedge ↔ Ensanche;
- X6 La barca — State 1 only, Wedge tip ↔ upstream Puerto edge;
- X7 Puente del Muelle — State 2 only, same endpoints as X6.

### Ensanche ↔ Puerto

There is no direct edge. Representative route families are:

- X2/X3/X4/X5 → Wedge → X6/X7 → Puerto when the landing crossing is available;
- X2/X3/X4/X5 → Wedge → X1 → camino sur → Puerto when routing through the historic bridge.

This is exactly the matrix semantics that the previous ASCII rendering failed to show.

---

## 5. Files changed in cycle 5

Semantic repair:

- `Docs/production/CITY_SPATIAL_CONSTITUTION.md` — version 1.2; selected semantic graph redrawn and
  explicit no-edge statement added;
- `Docs/evidence/WP-CITY-00/TOPOLOGY_B_CUNA_CONFLUENCIA.md` — selected-option graph redrawn with the
  same landmass/edge semantics and repair history extended to cycle 5.

Process/evidence refresh:

- `Docs/evidence/WP-CITY-00/WORKER_PRE_REVIEW.md` — fresh cycle-5 pre-review, prior CLEAN superseded;
- `Docs/evidence/WP-CITY-00/HANDOFF.md` — this handoff.

No code, test, Unity, asset, H0, ART-source or scale-calculation file was changed in this repair.

---

## 6. Prior independent review history

1. `b9473f04…` — unauthorized navigability premise / CITY→ART inversion.
2. `60915843…` — ferry added without reconciling graph/closure semantics.
3. `daefc6a1…` — false single-crossing availability absolute.
4. `d2d5f7a…` — physically impossible dry continuation of the Wedge past the confluence.
5. `bc9f1c85…` — semantic graph visually reintroduced a forbidden Ensanche→Puerto edge.

All five remain part of the record. `fail_cycle` is **5**; no repair resets it.

---

## 7. Validation / proof boundary

CITY is non-foundational. `PROCESS_ONLY` validation can produce a synthetic green receipt but cannot
prove spatial semantics.

The meaningful Worker evidence for this cycle is:

- reproduction of review #5261646488's graph contradiction;
- exact comparison against `CONNECTIVITY_MATRIX.md` and `PLANAR_EMBEDDING.md`;
- explicit landmass partition in both semantic graphs;
- state/count and route regression audit;
- fresh `WORKER_PRE_REVIEW.md` marked CLEAN only after those checks.

The independent Reviewer should still reconstruct the exact frozen SHA and challenge the drawings
against the matrix rather than trusting this handoff.

---

## 8. Downstream risks deliberately left open

Unchanged:

- measured walk times and effective speed belong to CITY-04;
- verticality/readability and route-cost detail belong to CITY-01/CITY-04;
- CITY-02 must place ordinary non-port services at Entrada/Puerto;
- CITY-03 owns the exact retained seed boundary;
- State 1→2 timing remains unassigned;
- Topology A must reopen if the dense city grows far beyond the selected small-city envelope.

None of those creates a hidden direct Ensanche↔Puerto connection.

---

## 9. Reviewer boundary

This session is the **Worker** and must not independently review its own candidate.

After this commit, PR #65 must record the exact new `Candidate HEAD SHA` / `Frozen candidate SHA`,
`Worker pre-review: CLEAN`, `Branch frozen: YES`, `fail_cycle: 5`, and return to Ready for a **fresh
independent Reviewer**.

No CITY-01 work has begun.