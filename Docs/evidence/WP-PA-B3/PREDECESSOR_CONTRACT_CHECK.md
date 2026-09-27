# WP-PA-B3 — PREDECESSOR_CONTRACT_CHECK

Worker state: `ACTIVE_WORKER`  
Baseline: `main@86ed180925ca683d5e0ff7e8e1c6e1fb59ec86e0`  
Execution class: `RESEARCH_BATCH`  
Date: 2026-09-26

## Dependency identity

`WP-PA-B3` is dependency-valid because `WP-PA-B2` was accepted through:

- canonical implementation PR `#240`;
- accepted candidate `6d05206884ac729e02cdcceb2edeecb43244b4bd`;
- independent PASS review `#5326015639`;
- implementation merge `c886ac2712bd81718607153f9cae9394959bcdf5`;
- binding post-acceptance `Docs/evidence/WP-PA-B2/DOCSYNC.md`;
- DocSync main commit `86ed180925ca683d5e0ff7e8e1c6e1fb59ec86e0`.

B2 acceptance makes PA-10, PA-11 and PA-12 authoritative inputs alongside already accepted PA-01..09. The PA README and `WP-PA-B3.md` both identify B3 as the current dependency-valid next PA workpack.

`DEPENDENCY_CHECK: PASS`

## Binding B3 contracts

Batch wrapper:

- `Docs/workpacks/PA/WP-PA-B3.md`.

Unit research specification:

- `Docs/workpacks/PA/WP-PA-13.md`.

Process packaging:

- `Docs/workpacks/PA/PA_BATCH_EXECUTION_AMENDMENT.md`.

Required B3 outputs are therefore:

1. canonical `Docs/research/living-world/results/PA-13.md`;
2. preservation of PA-13 positive/negative gates, including SV-1 and SV-2 as falsifiable future executable requirements;
3. a compact H3/H4 implementation handoff index traceable to PA results;
4. explicit separation between semantic requirement and deferred runtime/tuning proof;
5. no claim that H2 presentation proxies satisfy Living World requirements.

One independent batch review and one post-acceptance DocSync close B3. PA-14 remains separate and H2-GATE-bound.

## Required PA-13 inputs checked

### Accepted PA corpus

Canonical result artefacts exist for PA-01 through PA-12 under:

`Docs/research/living-world/results/PA-XX.md`

B3 consumes their accepted semantic guarantees rather than reopening them.

### Frozen donor preregistration

Read-only donor:

- repository: `Arkus0/Juego`;
- branch: `master`;
- file: `Docs/living-city-research/PA-11_PLAN_SIMULATION_CONTROL.md`;
- preregistration baseline named by donor: `c55b0486ba05347638a3c83ada1f6b0fdf618b2a`.

The donor attack surface includes:

- anti-thrashing;
- deterministic semantic replay;
- decoy/load locality;
- FULL↔ABSTRACT continuity;
- controlled overload;
- cascade caps;
- authored reservations;
- explainable budget outcomes;
- silent-drop, starvation, catch-up burst, nondeterministic ordering and degradation-truth failures.

B3 remaps these onto current Juego2 owners and does not import old M9/M10 runtime routing.

### Juego2 cross-cutting amendments

B3 consumes both Living World cross-cutting amendments. The decisive strengthening is intentional transformation:

- ordinary/noisy mistakes must not self-amplify naturally into campaign collapse;
- sustained coherent accepted causes may overcome stabilisers;
- constructive transformation is as legitimate as destructive transformation;
- recovery is causal and finite, not an unconditional reset;
- PA-13 budgets regulate work/fidelity/persistence, not outcome morality.

### Structural viability spike

`Docs/evidence/PA-13/SCALABILITY_VIABILITY_SPIKE.md` is present and explicitly classified as provisional planning evidence, not runtime/shipping benchmark.

The spike supports preserving two architecture guards:

- **SV-1:** irrelevant global population and unbounded dense-local all-pairs surfaces cannot determine the work shape of a bounded local/known/role/place/opportunity query;
- **SV-2:** ABSTRACT→FULL promotion cannot accumulate generic work proportional to every omitted micro-tick when materially relevant summarized change is held constant.

The spike does not authorize any final numeric budget.

## Accepted predecessor guarantees consumed

### PA-01 — routine

Stable expected intent remains separate from actual state. Current context is re-evaluated after interruption; off-screen fidelity may be cheaper only if causal meaning remains coherent.

### PA-02 — actor agency

Actors own meaningful choices. Discovery is scope-first and bounded before expensive comparison. Commitment/replan/failure needs stability and explainability. PA-13 can regulate cadence/work but cannot choose the actor's meaningful action.

### PA-03 — relationships

Typed directed state, structural ties and obligations remain separate semantic inputs. PA-13 cannot flatten them into one cheaper score.

### PA-04 — epistemic authority

Canonical truth is distinct from actor belief. Overload/degradation cannot substitute hidden truth for unknown/stale/false actor state.

### PA-05 — information flow

Communication is deliberate, opportunity-bound and receiver-owned. Propagation caps/technical lineage cannot become epistemic evidence or global broadcast.

### PA-06 — memory

Selected memory is finite and causally selected, with bounded pressure and active reason continuity. PA-13 may regulate persistence/compaction but cannot create an unbounded exception list or erase a still-required causal minimum.

### PA-07 — service/material state

Availability, capacity and named dependencies are real owner state. Degradation cannot silently fabricate full service or invisible replacement.

### PA-08 — activity integration

Sessions consume real opportunity/capacity and return bounded structured outcomes. PA-13 may regulate concurrent work, not activity result truth or participant choice.

### PA-09 — player causal agency

Player and NPC equivalent world actions use compatible immediate owners. PA-13 cannot protect the starting town by routing player transformation into a weaker causal universe.

### PA-10 — causal chains

Chains start after legitimate causes, recruit through bounded relevance, preserve actor decisions and explicitly terminate/reach stable state. PA-13 owns scale/concurrency/propagation budgets, not chain story content.

### PA-11 — traces/legibility

Selected reconstructible events require legitimate bounded evidence surfaces; debug lineage is not player knowledge. PA-13 retention pressure must respect declared reconstructibility windows or expose truthful loss/uncertainty.

### PA-12 — governance

Institutional rules change shared access/time/capacity/resource conditions; actors respond independently. Ordinary mistakes may recover, while sustained policy direction may transform the town. Repeal is not a time machine.

## B3 ownership and non-claims

B3 may decide research-level requirements for:

- budget dimensions;
- anti-thrashing/stability guarantees;
- scope/locality and bounded candidate work;
- propagation/concurrency pressure;
- finite persistence/trace/memory pressure;
- FULL/ABSTRACT semantic continuity and bounded reconciliation;
- truthful overload/degradation/kill-switch behaviour;
- deterministic semantic replay/diagnostics;
- paired accidental-stability vs deliberate destructive/constructive transformation;
- future executable proof ownership across H3/H4 and later profiling.

B3 does **not** implement or freeze:

- runtime scheduler or data structures;
- Unity components/APIs/schemas;
- final numeric CPU/memory/save/candidate/event budgets;
- final actor population/density targets;
- exact cooldown/inertia/salience constants;
- final FULL/ABSTRACT representation;
- final GameFlow architecture;
- H3/H4 implementation topology;
- H2 Living World behaviour;
- PA-14 integration verdict.

## Reopen condition

An accepted predecessor is reopened only if PA-13 demonstrates that a mandatory bounded/control requirement cannot preserve that predecessor's accepted semantics. Performance preference, a different scheduler/data structure, or desire for more background activity is not sufficient.

`PREDECESSOR_CONTRACT_CHECK: PASS`
