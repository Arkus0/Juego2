# WP-H0S-00 — Early Scalability Viability Spike

Status: **PLANNED / NOT_STARTED**  
Class: **SCALE / FALSIFICATION / NON-FOUNDATIONAL**  
Execution: **REMOTE_OK** for the synthetic structural phase; later runtime replay may be LOCAL/HYBRID if the owning runtime requires it  
Depends on: `WP-HK-GATE` PASS + merge + DocSync  
Blocks: nothing by default; may create a blocker only if it falsifies a required product assumption

## Why this workpack exists

H0 proved bounded correctness, transport neutrality, deterministic recovery and finite resource envelopes. It did not prove that the future living-world runtime can scale to a useful population.

Waiting for late H3/H4 tuning to discover an architectural complexity bomb would be unnecessary risk. Conversely, inventing FPS/CPU/memory thresholds before a representative runtime, cadence and hardware exist would produce false confidence.

H0S-00 therefore performs an **early structural falsification spike**. Its job is not to optimize code or declare shipping budgets. Its job is to detect whether the planned semantics force work to grow with irrelevant population, uncontrolled local density, or accumulated off-screen micro-time.

This workpack records two explicit proposals/invariants that future runtime work must either preserve or consciously replace with stronger reviewed evidence.

## Proposal / invariant SV-1 — Irrelevant-population independence and bounded candidate surfaces

A bounded/local decision must do work proportional to its authorized relevant source/candidate surface, not to the total number of persistent actors that merely exist.

The invariant has two parts:

1. **Global decoy independence.** Holding a decision's relevant source constant while adding at least 10,000 irrelevant distant actors must not turn the path into `AllActors -> filter local`. Relevant enumeration/scoring should remain unchanged or under the same declared deterministic cap.
2. **Density safety.** An index or spatial partition is not sufficient proof if arbitrarily many actors can occupy the queried locality and all pairs are then considered. Before expensive interaction scoring, the candidate surface must have a deterministic bound, partition, reservation/interest rule, scheduler, sampling rule with semantic justification, or an equivalent mechanism whose cap-hit behavior is inspectable.

### Falsification cases

Run synthetic scale sweeps at minimum across 100, 1,000 and 10,000 actors; include 100,000 where the harness is cheap enough.

Required cases:

- fixed relevant set + increasing irrelevant global decoys;
- roughly constant local density while total population grows;
- fixed physical/local domain while density grows;
- dense social graph / interaction burst;
- any candidate-provider design proposed by PA/H3/H4 that claims bounded discovery.

Measure structural work before wall-clock performance:

- actors/records inspected;
- candidate edges/pairs generated;
- candidates surviving cheap eligibility;
- candidates entering expensive comparison/scoring;
- cap/partition hits and degradation path;
- observed growth curve/exponent across the sweep.

### Structural PASS shape

- unrelated global population does not materially increase work for the tested bounded query class;
- constant-density growth is compatible with approximately linear aggregate work rather than accidental all-pairs growth;
- fixed-domain density growth cannot silently produce an unbounded all-pairs surface;
- any cap/partition/degradation is deterministic, visible and semantically defensible.

### Structural FAIL shape

- global enumeration is required to recover a local set;
- adding irrelevant decoys materially increases expensive decision work;
- an ostensibly indexed/local path becomes effectively quadratic as local density rises;
- correctness depends on silently dropping candidates by incidental container/order behavior.

A FAIL is architectural evidence. Do not paper over it with a faster collection or a larger machine before reviewing the semantic source of the candidate explosion.

## Proposal / invariant SV-2 — Bounded ABSTRACT→FULL reconciliation

ABSTRACT simulation must reduce work rather than merely defer an ever-growing debt of omitted micro-ticks until an actor/place becomes FULL again.

Holding the amount of **material summarized change** constant, increasing off-screen elapsed time by orders of magnitude must not force reconciliation work proportional to every skipped micro-update.

### Falsification cases

For representative actor/place state, compare at least:

- short off-screen interval;
- medium interval;
- long interval several orders of magnitude larger;
- same elapsed intervals with zero material events;
- same elapsed intervals with a bounded number of material scheduled events;
- burst case near event/history caps;
- FULL -> ABSTRACT -> FULL round-trip with deterministic semantic comparison.

Measure:

- reconciliation operations;
- material events/deltas consumed;
- history/state bytes that must be read;
- allocations/temporary state where measurable;
- maximum queue/catch-up batch;
- whether work scales with material change count, elapsed time, or both;
- divergence detected between FULL and ABSTRACT semantics.

### Structural PASS shape

- no-event or constant-material-change promotion remains bounded as elapsed micro-time grows;
- elapsed-time-dependent domains use summarized/scheduled transitions or another explicit bounded representation rather than replaying every omitted tick;
- material consequences remain truthful and inspectable;
- when the reconciliation budget cannot preserve required fidelity, the system reports explicit degradation/failure instead of silently fabricating equivalence.

### Structural FAIL shape

- promotion replays every skipped tick/action for every actor;
- long absence creates catch-up work proportional to elapsed micro-time even when material state is unchanged;
- ABSTRACT discards consequences that FULL would require and then silently invents/restores state on promotion;
- queues/history can grow without an explicit bound or compaction/degradation policy.

A FAIL means the FULL/ABSTRACT seam must be redesigned before it becomes a population-scale dependency.

## Relationship to PA-13 and later runtime budgets

`WP-PA-13` owns the semantic simulation-control/failure-mode synthesis and must carry SV-1/SV-2 forward as falsifiable requirements. H0S-00 supplies early structural evidence and reusable fixture shapes; it does **not** own final living-world algorithms.

Final CPU, memory, save-size, population, cadence and frame/tick budgets remain empirical. They should be frozen only once a representative H3/H4/H7 runtime and target hardware exist. At that point the same fixture families become instrumented CI/performance gates with reviewed numeric thresholds.

## Required evidence

H0S-00 must leave a compact machine-readable or tabular result containing, per case:

- seed/configuration;
- population and local-density assumptions;
- relevant-set size;
- inspected/generated/scored counts;
- catch-up interval and material-event count where applicable;
- measured growth relationship;
- PASS/FAIL against SV-1/SV-2;
- limitations explaining what the synthetic model does **not** prove.

Fixed deterministic seeds are required for review/replay. Exploratory fresh seeds may supplement them; any discovered failure seed should be persisted as a deterministic regression case.

## Negative controls

- `NC-SV1-DECOY`: add >=10,000 irrelevant distant actors while keeping the authorized relevant source identical.
- `NC-SV1-DENSITY`: hold the locality/domain fixed while increasing resident actors enough to expose pairwise explosion.
- `NC-SV2-ELAPSED`: multiply off-screen elapsed micro-time while holding material summarized change constant.
- `NC-SV2-NOEVENT`: promote after a very long ABSTRACT interval with zero material events; work may not be proportional to skipped ticks merely because time passed.

These controls must attack causal work, not just labels or telemetry counters.

## Acceptance

PASS only if the spike can falsify both invariant families and the tested architecture/model shows a plausible bounded path without inventing shipping constants.

A result may be `FAIL / ARCHITECTURE_REVIEW_REQUIRED`; that is a successful spike outcome if it catches a real complexity bomb early. Do not weaken the invariant to force GREEN.

## Definition of Done

- SV-1 and SV-2 are exercised by deterministic synthetic fixtures;
- growth evidence is recorded across increasing scale/density/elapsed-time cases;
- limitations are explicit;
- any failure is routed to the causal owner before a dependent runtime architecture freezes;
- PA-13 and later H3/H4/H7 performance work can consume the fixtures without treating synthetic timings as shipping budgets.
