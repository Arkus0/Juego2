# WP-CITY-00 — Worker pre-review after transfer / repair cycle 4

WP: `WP-CITY-00 — Keeper City spatial constitution + scale envelope`
Contract: `Docs/workpacks/CITY/WP-CITY-00.md`
Baseline SHA: `290f92e9c21f1e454e0d6924b29f4d778b9875f8`
Active Worker: `ChatGPT GPT-5.6 Sol — transfer Worker`
Worker history: `Claude Code — session 01KC1S5dCRLuMq6qeht4n34L → ChatGPT GPT-5.6 Sol`
Transfer SHA: `d2d5f7a7277d8949264d62e6ac2ecbbb171aae9b`
Governing protocol: `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` v1.7
fail_cycle: **4**

This is a Worker quality gate, **not** an independent review. Every earlier `CLEAN` is void. The old
circuit-breaker pre-review is preserved in git history and in `WORKER_TRANSFER.md` as a failed quality
gate: it declared a physically impossible candidate CLEAN.

This report is the final branch mutation after the semantic candidate and handoff were prepared. The
exact frozen SHA is therefore recorded in the PR body after this report commit; this file does not try
to name its own commit.

CITY is non-foundational. `Mode: PROCESS_ONLY` may generate a synthetic green receipt; that is not
semantic proof and is not used below.

---

## 1. Predecessor / authority check

Re-run from `WORKER_PLAN.md`:

- **WP-ART-00 seed / setting direction:** consumed as merged direction, not promoted to an H0-style
  proof. CITY keeps the small working landing inside the inherited valley setting and no longer needs
  an invented long-distance navigability premise.
- **Production Blueprint:** non-binding production proposal. v0.3 delegates topology, district
  families and scale to the constitution; no second topology remains authoritative there.
- **Living World PA material:** consumed only as non-binding spatial/product input. CITY-00 does not
  pre-accept a PA guarantee.
- **CITY-owned outputs:** topology, landmasses/crossings/port structure, scale envelope, playable vs
  scenic split, spatial invariants, rejected alternatives and downstream questions.
- **Outside claim:** measured travel/navmesh, CITY-02 programme, CITY-03 exact seed boundary, final
  geometry, asset feasibility, runtime/H0 contracts.

Result: **PASS** — no authority inversion or predecessor proof laundering found.

---

## 2. Complete diff / scope check

Baseline→candidate comparison contains **14 changed files**, all Markdown:

- 12 files under `Docs/evidence/WP-CITY-00/`;
- `Docs/production/CITY_SPATIAL_CONSTITUTION.md`;
- `Docs/production/PRODUCTION_BLUEPRINT.md`.

No `src/`, tests, Unity scene, asset, navmesh, tool/script, ROADMAP, H0 workpack, ART source, or
`RESIDUAL_LEDGER.md` mutation exists in the candidate.

`PRODUCTION_BLUEPRINT.md` changes only its non-binding topology reconciliation, zone/seed wording and
scale pointers; it keeps its non-binding status and delegates the selected city to the constitution.

Result: **PASS**.

---

## 3. Repair-cycle-4 causal proof — planarity first

The fourth independent FAIL was not a wording mismatch. It proved the authoritative connectivity
source itself physically impossible. The repair therefore uses this order:

1. fix landmasses in `PLANAR_EMBEDDING.md`;
2. enumerate every inter-landmass edge;
3. regenerate `CONNECTIVITY_MATRIX.md`;
4. regenerate constitution/dossier/comparison/refutation surfaces from that result.

### Landmass check

| Fact | Result |
|---|---|
| Río and Arroyo meet at the south point of the Wedge | PASS |
| Wedge terminates at that confluence | PASS |
| Ensanche remains outside Arroyo | PASS |
| Orilla sur exists outside Río and continues downstream as a joined-river bank | PASS |
| Puerto + Entrada occupy that real downstream Orilla-sur bank | PASS |
| no dry edge passes through the confluence and reappears downstream | PASS |

### Crossing check

Every change of landmass is a named crossing:

- Wedge↔Ensanche bank: **X2, X3, X4, X5** over Arroyo;
- Wedge↔Orilla sur near core: **X1** over Río;
- Wedge-tip landing head↔upstream Puerto edge: **X6** in State 1 or **X7** in State 2 over joined Río.

X6/X7 lands at the port's **upstream edge** and the district extends downstream from that bridgehead;
there is no implied 300 m bridge span and no hidden eighth crossing.

### Loop decomposition

- **L1 State 1:** Casco → X1 → camino sur → Puerto → X6 → landing head → Cuesta → Casco.
- **L1′ State 2:** same with X7.

Every segment is either dry on one named landmass or one named crossing.

Result: **PASS** — the specific geometric impossibility from FAIL 4 is removed rather than paraphrased.

---

## 4. Connectivity counts / state audit

Mechanical counts against the owning matrix:

| Quantity | Count / state | Check |
|---|---|---|
| crossing IDs over full constitution | **7** — X1..X7 | PASS |
| coexisting crossings in State 1 | **6** — X1..X6 | PASS |
| coexisting crossings in State 2 | **6** — X1..X5 + X7 | PASS |
| Arroyo crossing IDs | **4** — X2..X5 | PASS |
| permanent Arroyo crossings | **2** — X2, X3 | PASS |
| Río crossings in each state | **2** — X1+X6 or X1+X7 | PASS |
| State-1 Río availability ladder | **2 → 1 → 0** | PASS |
| State-2 single-closure floor | **1** | PASS |
| expansion seams | **6** | PASS |
| district families | **9** — 7 substantial + 2 edge | PASS |
| permanent CSI invariants | **12** | PASS |

The old `Puerto ↔ Orilla sur` crossing no longer exists because Puerto **is on Orilla sur**. X6/X7 is
instead the Wedge-tip↔Puerto crossing, which preserves the per-state count without inventing an edge.

Result: **PASS**.

---

## 5. Full-graph reachability audit — finding that the old pre-review missed

The outgoing circuit-breaker pre-review recorded as F15 that high water makes Casco↔Ensanche depend on
the plaza because X5 is submerged. The transfer notes carried that as a known seasonal condition.

That conclusion also fails a full-graph reachability check.

With X5 unavailable, the already-authored graph still contains this plaza-free path:

`Casco → Cuesta → landing head → paseo → Ribera → east stairs → Barrio Alto → X3 → Ensanche`.

No new street was added to rescue CSI-03; every segment was already used elsewhere in the candidate.
The old audit had checked the **local direct crossing** rather than graph reachability.

Repair:

- matrix §6 now distinguishes the short low-water X5 bypass from the long high-water X3 detour;
- constitution §2.6 mirrors it;
- CSI-03 binds structural plaza independence without promising that the shortest bypass is always
  available;
- handoff explicitly records that this inherited audit conclusion was corrected.

Result: **PASS** — remove-plaza connectivity is now a graph claim rather than a local-pair shortcut
claim.

---

## 6. Port / ordinary-movement audit

The port has four route families without double-counting crossings:

1. Entrada / valley road → Puerto, dry on Orilla sur;
2. X1 far end / camino sur → Puerto, dry after X1;
3. Casco / Cuesta → X6/X7 → upstream Puerto edge;
4. Ribera / paseo → landing head → X6/X7 → upstream Puerto edge.

Routes 3 and 4 share X6/X7 and are explicitly not counted as separate crossing IDs.

The port's reason for existing remains independent of long-distance navigability: high-water timber
rafting, áridos, ferry/bridge crossing, road break-bulk, fishing and ordinary waterfront work.
CSI-04 keeps Entrada/bus/road adjacent and requires at least two everyday non-port services there.

State-1 high water is intentionally costly: X6 suspends, leaving X1 + camino sur as the core↔port
route. That is a declared availability condition, not a hidden loss of connectivity.

Result: **PASS**.

---

## 7. Absolute / quantifier audit

Checked factual candidate absolutes and counts against their owning tables/graphs rather than against
nearby prose. Main assertions:

- **Wedge ends at confluence** — matches planar embedding;
- **no dry Wedge→Puerto edge** — matches all corrected route surfaces;
- **seven IDs / six per state** — matches X1..X7 table;
- **four Arroyo / two Río per state** — matches crossing table;
- **no third Río crossing per state** — follows same table;
- **2→1→0 State-1 ladder** — matches availability rows;
- **six seams** — six enumerated;
- **nine families** — nine rows;
- **twelve invariants** — CSI-01..CSI-12;
- **at least two expansion directions** — six named;
- **no Unity/runtime work** — complete diff contains docs only.

One wording overclaim found during this audit — "two independent dry approaches" to Puerto — was
reduced to the supportable claim "two named dry approach directions" in the matrix and selected-option
dossier. Graph-disjoint independence is not needed by the WP and was not proven.

Result: **PASS**.

---

## 8. Cross-surface reconciliation

| Surface | Check | Result |
|---|---|---|
| `PLANAR_EMBEDDING.md` ↔ matrix | same landmasses/endpoints | PASS |
| matrix ↔ constitution | X1..X7, two states, 2→1→0, port approaches, L1/L1′, seams | PASS |
| matrix ↔ Topology B dossier | bank choice, bridgehead, counts, no dry continuation | PASS |
| comparison/refutation ↔ corrected B | selection rationale preserved; bank-dependent reasoning updated | PASS |
| scale envelope ↔ corrected B | area/density/walk assumptions do not depend on impossible dry edge | PASS |
| blueprint ↔ constitution | blueprint delegates topology/scale and keeps compatible seed/zone language | PASS |
| transfer/handoff/process docs | transfer historical; current handoff points at repaired sources | PASS |

The earlier failure mode — surfaces agreeing with one another but all being wrong — is addressed by
checking them **against the planar embedding first**, not merely against each other.

Result: **PASS**.

---

## 9. WP acceptance walkthrough

Against `Docs/workpacks/CITY/WP-CITY-00.md`:

| Acceptance | Evidence / result |
|---|---|
| ≥3 genuine topology alternatives | A Dos Orillas, B Cuña de Confluencia, C Ribera Larga; **PASS** |
| loops; not one hub with spokes | three Wedge levels, four Arroyo crossings, Río loop L1/L1′, remove-plaza graph remains connected; **PASS** |
| river shapes movement | Río is a territorial boundary with two designed crossings/state; Arroyo is everyday crossing network; **PASS** |
| port has gameplay/material/social reason + setting scale | work/crossing/road-break-bulk rationale, 120–150 m working frontage hypothesis consumed from ART; **PASS** |
| spatial characters distinct | casco, civic/commercial, two residential characters, work/port edge, rural edge; **PASS** |
| ≥2 outward/expansion directions | six seams; **PASS** |
| actor can be elsewhere in town | longest ordinary route target ≈1.05–1.15 km / 15–17 min; hypothesis clearly marked for CITY-04; **PASS** |
| size justified by density/travel/content cost | `SCALE_ENVELOPE.md` falsifies 0.8–1.2 km² and adopts 0.30–0.45 km²; **PASS** |
| retained seed is real part of final constitution | 0.03–0.06 km² Wedge-tip seed, no demolition required; **PASS** |
| no Unity/asset/runtime contract | docs-only diff; **PASS** |

### Negative gates

- not four streets plus scenery — **clear**;
- no acreage vanity — **clear**;
- not every meaningful route through plaza — **clear**, including high-water full-graph detour;
- river/port are structural movement/work elements — **clear**;
- retained seed grows by seams without replacement — **clear**;
- no H1/H2 implementation authority pre-decided — **clear**.

### Definition of Done

CITY-01 can consume named landmasses, crossings, states, route families, loops, seams and a scale
hypothesis without inventing a different city.

Result: **PASS**.

---

## 10. Findings fixed before freeze

This transfer Worker found and repaired four issues before declaring CLEAN:

1. **Planarity repair required an explicit port bridgehead.** X6/X7 now lands at the upstream Puerto
   edge and the district extends downstream, removing an avoidable bridge-span ambiguity.
2. **The old F15 seasonal-plaza conclusion was false at full-graph scope.** Long X3 bypass now recorded
   instead of claiming mandatory plaza routing.
3. **"Independent dry approaches" overclaimed graph-disjointness.** Reduced to named dry approach
   directions.
4. **Process surfaces were stale after transfer.** `WORKER_TRANSFER.md` is now historical, and
   `HANDOFF.md` describes the receiving candidate rather than the failed circuit-breaker candidate.

None is an additional Reviewer FAIL; `fail_cycle` remains **4**.

---

## 11. Residual risks / downstream questions

Not blockers for CITY-00:

1. 1.15 m/s effective speed and all walk times are planning hypotheses, not measurements.
2. Verticality/readability/followability can still fail CITY-04 measurement.
3. ~830 buildings depends on declared coverage/footprint assumptions.
4. Topology A's rejection is coupled to the small-city scale; above roughly 0.7 km² reopen topology.
5. State-1 high water removes X6 and makes Puerto a long X1+camino-sur detour.
6. CITY-02 must place at least two ordinary non-port services at Entrada/Puerto.
7. CITY-03 owns exact seed boundary.
8. The timing of X6→X7 / State 1→2 remains unassigned.
9. Exact route costs, including the long high-water Casco↔Ensanche bypass, belong to CITY-01 and later
   CITY-04 measurement.
10. A future Reviewer should attack the **embedding and matrix contents**, not infer correctness from
    this report or cross-surface agreement.

---

## 12. Worker verdict

No known blocking defect remains inside WP-CITY-00's claim.

```text
WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED_THIS_TRANSFER: 4
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-CITY-00/WORKER_PRE_REVIEW.md
fail_cycle: 4
```

Next protocol step after committing this report: record the resulting exact 40-character HEAD as the
Frozen candidate SHA in PR #65, set the branch frozen, mark the PR Ready, and hand it to a **fresh
independent Reviewer**. This Worker must not review its own candidate.