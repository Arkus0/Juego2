# PA-13 — Initial synthetic structural viability spike

Status: **PROVISIONAL PLANNING EVIDENCE / NOT A RUNTIME BENCHMARK**  
Date: 2026-09-22  
Purpose: record the first falsification exercise that motivated SV-1 and SV-2 before a representative H3/H4 living-world runtime exists.

## Scope and caveat

These measurements are structural synthetic models executed outside the Juego2 runtime. They do **not** establish CPU milliseconds, memory budgets, supported NPC counts, frame/tick cadence or shipping hardware limits.

They answer a narrower question: can two plausible implementation shapes exhibit fundamentally different growth behavior under the same population/elapsed-time scaling, and therefore are SV-1/SV-2 meaningful early architecture guards?

Yes.

## Experiment A — candidate surface under population/density growth

Deterministic setup:

- seeds: integers `0..9`;
- populations: `1,000`, `10,000`, `100,000`;
- actors placed uniformly in a square domain;
- unit spatial cells;
- synthetic interaction candidate count = unordered actor pairs occupying the same cell;
- **constant-density case:** domain area scales with population to target roughly four actors per unit cell;
- **fixed-domain case:** domain remains at the `N=1,000` baseline size while population grows;
- reported candidate counts are the mean over ten deterministic seeds;
- growth exponent is a log-log linear fit across the three population points.

| Population | Constant-density candidate pairs | Fixed-domain candidate pairs |
|---:|---:|---:|
| 1,000 | 1,955.3 | 1,955.3 |
| 10,000 | 20,027.4 | 196,494.1 |
| 100,000 | 199,766.1 | 19,619,465.0 |

Observed growth exponent:

- constant density: approximately **N^1.005**;
- fixed domain / increasing density: approximately **N^2.001**.

### Interpretation

Spatial partitioning can support approximately linear aggregate candidate growth when local density stays bounded. The same partition does **not** prevent near-quadratic work when local occupancy is allowed to grow without a bound and every co-located pair becomes a candidate.

Therefore the architecture requirement cannot be merely “use a spatial hash/index”. It needs a bound on the expensive candidate surface produced by that index. This is recorded as SV-1.

## Experiment B — irrelevant global decoys

Synthetic decision has exactly two relevant candidates. Compare a naive `AllActors -> filter relevant` path with a direct bounded-source lookup.

| Irrelevant decoys | Naive records inspected | Bounded-source records inspected |
|---:|---:|---:|
| 0 | 2 | 2 |
| 100 | 102 | 2 |
| 1,000 | 1,002 | 2 |
| 10,000 | 10,002 | 2 |
| 100,000 | 100,002 | 2 |

### Interpretation

The semantic result can remain identical while the work shape changes from global-population dependent to relevant-source dependent. This directly matches PA-02's anti-global-scan/irrelevant-decoy requirement and provides a fixture family for SV-1.

## Experiment C — off-screen catch-up debt

Synthetic promotion set: `9,900` ABSTRACT actors. Hold material summarized change constant at `50` events while increasing elapsed omitted micro-steps.

Two deliberately contrasting cost models:

- naive replay: `actors × omitted_micro_steps`;
- summarized reconciliation: one bounded actor-state reconciliation pass plus the fixed material events, modeled as `actors + events` for this structural comparison.

| Omitted micro-steps | Naive replay operations | Summarized-model operations | Naive / summarized |
|---:|---:|---:|---:|
| 60 | 594,000 | 9,950 | 59.7× |
| 3,600 | 35,640,000 | 9,950 | 3,581.9× |
| 86,400 | 855,360,000 | 9,950 | 85,965.8× |

### Interpretation

This is not evidence that the final runtime will use exactly `actors + events`; it is a falsification model showing why an ABSTRACT mode that simply accumulates omitted tick debt can lose its scaling benefit catastrophically. Holding material summarized change constant while elapsed time grows gives a clean negative control for the future seam. This is recorded as SV-2.

## Proposals recorded from the spike

### SV-1 — Irrelevant-population independence / bounded local candidate work

A local/bounded decision must not acquire work proportional to unrelated global population, and local density must not silently recreate an unbounded all-pairs interaction surface. Candidate generation must be bounded before expensive scoring/comparison, with deterministic and inspectable cap/degradation behavior.

### SV-2 — Bounded ABSTRACT→FULL reconciliation

Promotion from ABSTRACT to FULL must not require replaying every omitted micro-tick when materially relevant summarized change is held constant. Reconciliation should be bounded by summarized/material state change or by an explicit reviewed catch-up budget; inability to preserve required fidelity must degrade/fail truthfully.

## What this evidence does not prove

It does not prove:

- that 100,000 persistent actors are a Juego2 product requirement or supported target;
- that four actors per synthetic cell corresponds to any real city density;
- that same-cell pairing is the future interaction algorithm;
- that the summarized reconciliation model is the implementation;
- any final CPU, memory, save-size, frame-time or tick-rate threshold;
- FULL/ABSTRACT semantic equivalence for any real subsystem.

Those claims require instrumented representative runtime evidence. The value of this spike is earlier: it establishes causal negative controls capable of detecting two classes of complexity bomb before numeric shipping budgets are knowable.

## Handoff

- `Docs/workpacks/PA/WP-PA-13.md` owns the two viability invariants and must preserve them as falsifiable requirements.
- later H3/H4/H7 runtime owners should reuse the same fixture families with real instrumentation and reviewed numeric budgets.
- no additional active workpack is created by this evidence note.
