# WP-CITY-00 — Worker transfer

WP: WP-CITY-00 — Keeper City spatial constitution + scale envelope
Contract: `Docs/workpacks/CITY/WP-CITY-00.md`
Baseline SHA: `290f92e9c21f1e454e0d6924b29f4d778b9875f8`
Outgoing Worker: Claude Code — session 01KC1S5dCRLuMq6qeht4n34L
Transfer SHA: `d2d5f7a7277d8949264d62e6ac2ecbbb171aae9b`
fail_cycle: **4** — a transfer does not reset it and does not erase evidence

**This file is the authoritative statement of current state.** Where any other evidence document
disagrees with it, this one is correct and the other is stale.

---

## 1. Current state

| | |
|---|---|
| PR | [#65](https://github.com/Arkus0/Juego2/pull/65), **Draft** |
| Branch | `claude/city-urban-topologies-ehn6qf` |
| Branch HEAD | `d2d5f7a7277d8949264d62e6ac2ecbbb171aae9b` |
| Worker state | **PAUSED** — the outgoing Worker has stopped |
| Frozen candidate SHA | **NONE** |
| Worker pre-review | **NOT_RUN** — the transfer invalidates the previous `CLEAN` |
| Last reviewer verdict | **FAIL** on `d2d5f7a…`, review [#5261556774](https://github.com/Arkus0/Juego2/pull/65#pullrequestreview-5261556774) |
| `main` | untouched, still at the baseline |

### No freeze is active for review

Nothing on this branch is awaiting an independent Reviewer. Do **not** start one. The last frozen
candidate was reviewed and failed; the freeze is released and the PR is back in Draft.

### The failed candidate is preserved

`d2d5f7a…` is intact on the branch. Nothing was reverted, rebased or squashed. All 15 commits above
the baseline remain individually recoverable, and each of the four failed candidates is reachable by
its SHA (§2).

---

## 2. The four FAILs

Every review was independent, by the project owner, bound to an exact SHA.

### FAIL 1 — `b9473f04da72462619e9561c097083afdc6c436d` · review [#5261386416](https://github.com/Arkus0/Juego2/pull/65#pullrequestreview-5261386416)

**Defect.** The constitution justified the landing's placement and its road↔water transfer role on a
premise it itself called invented and unaccepted by ART — that the joined river carries loaded craft
downstream — then left it open while admitting its refusal would reopen the topology. A workpack whose
DoD is stability for CITY-01 cannot close over a condition capable of invalidating it. The same defect
inverted the CITY→ART authority direction in §2.5 and CSI-05.

**Repaired.** The landing was rewritten to rest on confluence hydrology plus four ordinary reasons —
timber rafted at high water, áridos worked off the bars, a roped ferry, break-bulk for the valley road
— all inside the small-landing/working-boats direction `Docs/art/SETTING.md` already grants. CSI-05
now reads the port scale as *consumed from* ART rather than binding on it. **No ART decision is
pending.** Not reopened by any later review.

### FAIL 2 — `6091584313488bcfb1840132f22adcc9b1b5de4f` · review [#5261418045](https://github.com/Arkus0/Juego2/pull/65#pullrequestreview-5261418045)

**Defect.** The repair added *La barca* as a second Puerto ↔ south-bank crossing but left both
crossing tables asserting that closing the Puente Viejo cuts the south side off, and neither semantic
graph drew the ferry. The district graph and the crossing strategy described two different cities.

**Repaired.** Two connectivity states named (§2.4.1); closure consequences derived from the
dimensional sketch; both graphs redrawn with the ferry and the south-bank return; both tables
restructured to separate Availability from closure consequence. Not reopened by any later review.

### FAIL 3 — `daefc6a1e4959c1cffacd662f5578a4c13a642d3` · review [#5261488049](https://github.com/Arkus0/Juego2/pull/65#pullrequestreview-5261488049)

**Defect.** A sentence directly beneath the crossing table claimed the south bank is never left on a
single crossing — when the barca-suspended row is exactly that — and counted the double-failure case
as one crossing when it is zero. CSI-06 carried the claim into the permanent contract.

**Repaired.** CSI-06 restated as a design obligation rather than a promise the weather can break;
§2.4.1 given an explicit crossing-count ladder (2 → 1 → 0); the dossier's paraphrase deleted; the
expansion-seam count corrected in four places plus a broken section reference. Not reopened by any
later review.

### FAIL 4 — `d2d5f7a7277d8949264d62e6ac2ecbbb171aae9b` · review [#5261556774](https://github.com/Arkus0/Juego2/pull/65#pullrequestreview-5261556774) — **NOT REPAIRED**

This one is different in kind. The three before it were surfaces disagreeing with each other. This is
the authoritative source being **wrong**.

**Defect — the planar embedding around the confluence is impossible.** The candidate asserts all of:

1. the Río arrives from the NE;
2. the Arroyo arrives from the N;
3. they meet at the southern tip of the wedge, where the casco stands;
4. the city occupies the wedge between the two channels;
5. the Puerto lies downstream of that confluence, to the SW;
6. and the Puerto is dry-land continuous with the wedge — `CONNECTIVITY_MATRIX.md` §1 says it is
   separated by "nothing — it is the wedge's downstream tip", while the graph draws
   Casco → Cuesta del Puerto → Puerto and Ribera → paseo/sirga → Puerto **with no water crossing**.

Those six cannot hold at once. A wedge between two channels that join at its point **ends at the
confluence**. Downstream there are only the two banks of the combined river, and neither is a dry
continuation of the wedge's interior. Reaching a port genuinely downstream of the confluence requires
crossing one of the two channels near the mouth, or placing the landing in a different relation to the
confluence.

**What it invalidates inside the candidate:** `CONNECTIVITY_MATRIX.md` §1 ("only two boundaries need
crossing"), §2 (the seven-crossing set claimed complete), §7 (three land approaches to the port), §8
(L1/L1′ using Puerto → Cuesta → casco as a land leg); `CITY_SPATIAL_CONSTITUTION.md` §2.2 and §2.6;
CSI-10 and seam 5 as drawn. It also falsifies the pre-review's claims that no promised route is
impossible and that the matrix holds the complete crossing set.

---

## 3. A conclusion of mine the fourth FAIL falsified

The circuit-breaker audit that produced `CONNECTIVITY_MATRIX.md` concluded, in §11 of that file and
in `WORKER_PRE_REVIEW.md` §4, that *"every finding is a documentation defect — Topology B stands and
the city is not redesigned."*

**That conclusion is wrong and must not be trusted.** The audit verified that the surfaces agreed
with each other and never verified the matrix against planar geometry. They agreed on something
false — the exact residual risk the same pre-review had named as the place to attack, and where the
Reviewer duly found this.

The lesson for the receiving Worker: *a single source of truth removes drift between surfaces; it
does nothing about the source being wrong.* Any new matrix needs a check against physical
realisability, not only against internal agreement.

---

## 4. Surfaces that constitute the selected topology

### Normative — these define the city

| File | Role |
|---|---|
| `Docs/production/CITY_SPATIAL_CONSTITUTION.md` | the deliverable: district graph (§2.2), district families (§2.3), crossing strategy (§2.4) and connectivity states (§2.4.1), the landing (§2.5), loop test (§2.6), scale envelope (§3), fabric vs scenic envelope (§4), twelve CSI invariants and six expansion seams (§5), rejected alternatives (§6), blueprint reconciliation (§7), twelve open questions (§8) |
| `Docs/evidence/WP-CITY-00/CONNECTIVITY_MATRIX.md` | single source for connectivity — **contains the impossible embedding; regenerate, do not patch** |

### Supporting evidence

| File | Role |
|---|---|
| `TOPOLOGY_B_CUNA_CONFLUENCIA.md` | dossier for the selected option |
| `TOPOLOGY_A_DOS_ORILLAS.md`, `TOPOLOGY_C_RIBERA_LARGA.md` | the two rejected options, in full |
| `REFUTATION_LOG.md` | charges, defences and verdicts for all three |
| `COMPARISON_MATRIX.md` | eleven-criterion matrix plus the six negative gates |
| `SCALE_ENVELOPE.md` | traversal arithmetic, building-count estimate, change conditions |
| `WORKER_PLAN.md` | predecessor contract check, claim boundary, and the rules the four cycles accumulated |
| `WORKER_PRE_REVIEW.md` | the pre-review that declared `d2d5f7a…` CLEAN — kept as the record of a pre-review that missed a blocker |
| `HANDOFF.md` | superseded by this file where they disagree |

### Reconciled downstream

`Docs/production/PRODUCTION_BLUEPRINT.md` v0.3 — topology, district families and scale band handed to
the constitution so `main` does not carry two contradictory towns. Its §1.2 diagram was replaced by a
pointer; if the embedding changes, its §1.3 zone table may need to follow.

---

## 5. Pending decisions

### 5.1 Blocking — the planar embedding (FAIL 4's minimum correction boundary)

Not a sentence to patch. Reopen the planar embedding of Topology B around the confluence and fix
explicitly:

1. which bank the port occupies downstream;
2. how it physically connects to Casco, Ribera and Entrada;
3. which channel, if any, each approach crosses;
4. whether the crossing set is still seven;

then **regenerate from that decision** the area/crossing matrix, the four port approaches, L1/L1′, the
route counts, and the affected seams and invariants.

The Reviewer judges that Topology B may survive a small adjustment — a crossing at the arroyo's mouth,
or a different landing placement — but says plainly that the city as drawn is not topologically
closed. The receiving Worker is **not** obliged to keep Topology B if the fix proves expensive;
`REFUTATION_LOG.md` records what A and C lost on, and A's rejection is explicitly coupled to the size
decision rather than to quality.

### 5.2 Carried, non-blocking

| # | Decision | Owner |
|---|---|---|
| Q1 | Visual and material language for the landing, its yards and the ferry, inside the kit allowlist. A consumption question: no answer invalidates the constitution | `WP-ART-00` |
| — | *When* the State 1 → State 2 transition happens. No workpack owns the timing | unassigned |
| — | Casco ↔ Ensanche routes through the plaza in high water. Deliberate content; CITY-01 should treat it as a recurring seasonal state, and CITY-02 should weigh it before siting something essential across the arroyo | CITY-01 / CITY-02 |
| Q2–Q12 | Verticality, measured walk times, route graph and travel profiles, closable crossing, Tier A/B programme, the two non-port services at the junction, activity homes, seed boundary, seed's arroyo crossing, mesh budget, south-bank loop traffic | CITY-01/02/03/04, art review |

---

## 6. What the receiving Worker should not redo

No review has challenged any of this, and redoing it would burn a cycle:

- the three materially different topologies and the refutation of all three before any preference;
- the falsification of the size hypotheses — 12–18 min of cross-city walking implies 0.30–0.45 km² of
  dense fabric, not 0.8–1.2 km²; the seed band 0.03–0.06 km², not 0.10–0.15;
- the landing's reason for existing, which needs no premise beyond the inherited setting (FAIL 1);
- the two connectivity states and the crossing-count ladder (FAILs 2 and 3);
- the design-versus-availability rule for invariants, and the rule that an evidence document must not
  restate an invariant in its own words.

Those rules are recorded in `WORKER_PLAN.md` and are worth carrying forward.

---

## 7. Protocol notes

- `Docs/engineering/WORKER_REVIEW_PROTOCOL.md`: a transfer requires the prior Worker stopped, the PR
  in Draft, and a `Transfer SHA` plus Worker-history update recorded. All three are done.
- The transfer invalidates the prior `WORKER_PRE_REVIEW: CLEAN`. The receiving Worker reruns it from
  scratch before any freeze.
- `fail_cycle` stays at 4 and no evidence is erased.
- The outgoing Worker acted as Worker on all four candidates and may not review any of them.
- `Docs/workpacks/CITY/WP-CITY-00.md`, `Docs/engineering/RESIDUAL_LEDGER.md`, ART and H0 are
  untouched. `WP-HK-GATE` remains the next dependency-valid H0 workpack, unaffected by this track.
