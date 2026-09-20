# WP-CITY-00 — Worker handoff

WP: `WP-CITY-00 — Keeper City spatial constitution + scale envelope`
Contract: `Docs/workpacks/CITY/WP-CITY-00.md`
Baseline SHA: `290f92e9c21f1e454e0d6924b29f4d778b9875f8`
Branch: `claude/city-urban-topologies-ehn6qf`
Worker history: `Claude Code — session 01KC1S5dCRLuMq6qeht4n34L → ChatGPT GPT-5.6 Sol`
Transfer SHA: `d2d5f7a7277d8949264d62e6ac2ecbbb171aae9b`
fail_cycle: **4**

The exact candidate/freeze status is recorded in the PR body after this file and the final Worker
pre-review are committed. Evidence files cannot safely name their own final commit SHA.

`WORKER_TRANSFER.md` is the historical transfer record. It is not the current-state authority.

---

## 1. What changed in repair cycle 4

Independent review #5261556774 proved that candidate
`d2d5f7a7277d8949264d62e6ac2ecbbb171aae9b` described an impossible planar embedding: the selected
city occupied a Wedge between two channels that meet at its southern point, but the same candidate let
that Wedge continue dry through the confluence to a downstream Puerto.

The repair starts from geometry rather than prose:

- the **Wedge ends at the confluence**;
- Ensanche remains outside the Arroyo;
- **Orilla sur** continues onto the south/east bank of the joined river;
- **Puerto Fluvial + Entrada / Bus / Carretera sit on that Orilla-sur downstream bank**;
- X1 Puente Viejo remains the historic Wedge↔Orilla-sur crossing;
- X6 La barca (State 1), replaced by X7 Puente del Muelle (State 2), crosses from the Wedge-tip landing
  head to the **upstream edge of Puerto**;
- the Puerto district extends downstream from that bridgehead, so X6/X7 spans the channel rather than
  the district's full longitudinal length;
- X1 + camino sur is the other core↔Puerto route.

`PLANAR_EMBEDDING.md` is the bounded physical-realizability proof.
`CONNECTIVITY_MATRIX.md` owns connectivity semantics regenerated from it.

This is a **real spatial correction**. It is not another documentation-only patch.

---

## 2. What survives unchanged

The fourth FAIL did not falsify:

- the three materially different alternatives A/B/C and the reason B was selected;
- the 0.30–0.45 km² dense-fabric scale envelope;
- the 0.03–0.06 km² retained-seed band;
- the landing rationale based on timber rafting at high water, áridos, ferry work and road break-bulk;
- the two connectivity states;
- the State-1 Río availability ladder **2 → 1 → 0**;
- the CITY→ART authority direction;
- the rule that design provision and momentary availability are separate claims.

Comparison/refutation surfaces were updated only where the corrected bank assignment changes their
reasoning.

---

## 3. Corrected connectivity at a glance

### Crossings

Seven IDs exist across the constitution's life; six coexist in either state:

- X1 Puente Viejo — Río, Wedge↔Orilla sur;
- X2 Puente del Mercado — Arroyo;
- X3 Pasarela del Lavadero — Arroyo;
- X4 Puente de la Vega — Arroyo;
- X5 Pasos/vado — Arroyo;
- X6 La barca — joined Río, State 1 only, Wedge-tip landing head↔upstream Puerto edge;
- X7 Puente del Muelle — joined Río, State 2 only, same endpoints as X6.

Thus: four Arroyo crossings; two Río crossings per state; no hidden eighth crossing.

### Port approaches

1. Entrada / valley road → Puerto, dry on Orilla sur;
2. X1 far end → camino sur → Puerto, dry after the historic crossing;
3. Casco / Cuesta → X6/X7 → upstream Puerto edge;
4. Ribera / paseo → landing head → X6/X7 → upstream Puerto edge.

The last two are route families sharing the same crossing, not extra crossing IDs.

### Loops

- **L1, State 1:** Casco → X1 → camino sur → Puerto → X6 → landing head → Cuesta → Casco.
- **L1′, State 2:** same, X7 replacing X6.

Every landmass change in both loops is now a named crossing.

### Expansion seams

Six remain. Seam 5 is now downstream **on Orilla sur from Puerto/Entrada**. Seam 6 is the X6→X7
transition. No seam asks the Wedge to continue through the confluence.

---

## 4. One inherited audit conclusion was also corrected

The outgoing circuit-breaker pre-review had concluded that, in high water, Casco↔Ensanche must route
through the plaza because X5 is submerged.

The transfer repair rechecked the **full graph**, not only the local crossing pair. That absolute is
also false: a longer plaza-free route already exists through the same edges the candidate otherwise
uses:

`Casco → Cuesta → landing head → paseo → Ribera → east stairs → Barrio Alto → X3 → Ensanche`.

High water therefore removes the **short** direct X5 bypass and imposes a long detour; it does not make
the plaza a mandatory connector. `CONNECTIVITY_MATRIX.md` §6 and CSI-03 now say exactly that.

This is not a new street invented to save the invariant. Every segment was already present in the
selected graph; the earlier audit simply stopped its reachability analysis too early.

---

## 5. Where to read the candidate

| File | Role |
|---|---|
| `Docs/production/CITY_SPATIAL_CONSTITUTION.md` | reviewable product constitution and scale envelope |
| `Docs/evidence/WP-CITY-00/PLANAR_EMBEDDING.md` | landmasses + physical-realizability proof for FAIL 4 |
| `Docs/evidence/WP-CITY-00/CONNECTIVITY_MATRIX.md` | authoritative crossings, states, availability, routes, loops and seams |
| `TOPOLOGY_B_CUNA_CONFLUENCIA.md` | selected-option dossier after cycle-4 correction |
| `TOPOLOGY_A_DOS_ORILLAS.md`, `TOPOLOGY_C_RIBERA_LARGA.md` | rejected alternatives |
| `COMPARISON_MATRIX.md` | required multi-axis comparison + negative gates |
| `REFUTATION_LOG.md` | charges, defences, original selection and post-selection FAIL history |
| `SCALE_ENVELOPE.md` | travel/density/content-cost arithmetic and change conditions |
| `WORKER_PLAN.md` | current predecessor check, claim boundary and repair rules |
| `WORKER_PRE_REVIEW.md` | final Worker quality gate for this transferred candidate |
| `WORKER_TRANSFER.md` | historical state handed from the outgoing Worker |

`PRODUCTION_BLUEPRINT.md` v0.3 remains non-binding and delegates topology/scale to the constitution.
Its retained-seed and actor-route reasoning remains compatible with the corrected bank choice.

---

## 6. Four prior independent FAILs remain part of the record

1. `b9473f04…` — unauthorized navigability premise / CITY→ART inversion.
2. `60915843…` — ferry added without reconciling graph/closure semantics.
3. `daefc6a1…` — false availability absolute despite 2→1→0 table.
4. `d2d5f7a…` — authoritative matrix itself physically impossible at the confluence.

`fail_cycle` remains **4**. No transfer or repair resets it, and none of those candidates is presented
as accepted.

---

## 7. Validation / proof boundary

CITY is non-foundational. No canonical `WP-HK-*` exact-SHA validation entrypoint applies here.
`Mode: PROCESS_ONLY` can therefore produce a synthetic green receipt; that receipt is **not semantic
proof** of this spatial constitution.

The meaningful Worker evidence is the WP acceptance walkthrough, complete diff/scope check,
planar-realizability audit, connectivity counts and strict Worker pre-review.

The independent Reviewer must reconstruct the candidate and challenge the source data itself rather
than trusting this handoff or the Worker's matrix conclusions.

---

## 8. Product risks deliberately left to downstream owners

- walk-time values and 1.15 m/s effective speed remain hypotheses for CITY-04 measurement;
- verticality/readability remains a CITY-01/CITY-04 risk;
- the 830-building estimate depends on planning coverage/footprint assumptions;
- Topology A's rejection remains coupled to the small-city scale and must reopen above roughly 0.7 km²;
- State-1 high water removes X6 and makes Puerto a long X1+camino-sur detour;
- CITY-02 still has to place at least two ordinary non-port services at Entrada/Puerto;
- exact seed boundary belongs to CITY-03;
- timing of State 1→2 remains unassigned.

Those are residual/product questions, not hidden claims of this WP.

---

## 9. Reviewer boundary

This session is the receiving **Worker**. It must not act as independent Reviewer of the candidate it
prepared.

Once the PR body records `Worker pre-review: CLEAN`, exact `Frozen candidate SHA`, `Branch frozen:
YES`, and the PR is Ready, the next action is a **fresh independent Reviewer** on that exact SHA.

No CITY-01 work has begun.