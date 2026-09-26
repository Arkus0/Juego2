# WP-PA-B3 — H3/H4 PRE-INTEGRATION HANDOFF INDEX

Status: `WORKER CANDIDATE EVIDENCE`  
Batch: `WP-PA-B3`  
Date: 2026-09-26

## 0. Purpose and boundary

This is the compact implementation handoff required by `WP-PA-B3`.

It maps the **accepted PA-01..12 research results plus candidate PA-13** to likely H3/H4 implementation consumers while preserving the distinction:

```text
RESEARCH FINDING / SEMANTIC REQUIREMENT
        !=
RUNTIME ARCHITECTURE
        !=
DEFERRED EMPIRICAL PROOF
```

This index does **not**:

- replace PA-14 integration review;
- invent final H3/H4 runtime topology, APIs, schemas, component model or scheduler;
- move Living World implementation into H2;
- validate tuning or performance constants;
- claim that every semantic item listed must ship in the first H3/H4 slice simultaneously.

Canonical authority remains each PA result under `Docs/research/living-world/results/PA-XX.md` and its accepted review/evidence. This file is navigation/handoff only.

## 1. Phase boundary

### H2

H2 characters remain visual/presentation proxies: keeper-scale identity/readability, idle/walk or bounded movement, minimal dialogue/bark and scene composition. Those behaviours may reveal content/authoring needs but are **not** evidence that PA Living World semantics or PA-13 scalability/control gates exist.

### H3

Likely first implementation layer for:

- persistent actor identity;
- time/routine context;
- POIs/smart opportunities;
- schedule/routine execution and interruption minimum;
- visible travel/place participation;
- bounded local service/activity opportunities and capacity;
- first deterministic/headless product fixtures around these seams;
- first local-query instrumentation and, only if introduced, first FULL/ABSTRACT seam.

### H4

Likely primary Living World causal implementation layer for:

- autonomous actor choice;
- relationships;
- beliefs/knowledge and information flow;
- selected memory;
- shared work/material/activity outcomes;
- origin-neutral player/NPC world consequences;
- autonomous causal chains;
- legitimate investigation traces;
- institutional/governance condition effects;
- simulation control, persistence/reconciliation and structured diagnostics.

These are **consumer expectations from research**, not a frozen architectural decomposition.

## 2. PA finding → implementation consumer matrix

| PA | Canonical result | Semantic requirement to carry forward | Likely first consumer | Deferred empirical/runtime proof |
|---|---|---|---|---|
| PA-01 | `results/PA-01.md` | expected semantic routine distinct from actual state; fallible opportunity; causal travel; interruption cleanup + current-context re-evaluation; no waypoint screenplay | **H3** | navigation quality, tuned travel estimates, interruption feel, save/load, authoring cost, FULL/ABSTRACT performance |
| PA-02 | `results/PA-02.md` | actor-owned problem/goal/action/target; scope-first bounded discovery; receiver-owned optional response; commitment/replan/failure; explainable trace | **H4**, with H3 preparing bounded opportunity sources | chooser algorithm, cadence, candidate caps, population scale, target hardware |
| PA-03 | `results/PA-03.md` | directed trust/affinity/fear kept semantically distinct from structural ties/obligations; no universal relationship score; action-specific consumption | **H4** | numeric representation/tuning, persistence/indexing, content dimensions beyond minimum |
| PA-04 | `results/PA-04.md` | canonical truth != actor belief; `UNKNOWN` fail-closed; explicit acquisition/revision; false/stale/partial belief; no truth/debug leakage | **H4** | perception geometry, belief storage/index, revision tuning, UI/query API, scale |
| PA-05 | `results/PA-05.md` | deliberate sender→receiver assertion transfer through real opportunity; receiver owns interpretation; no auto-retell; actor-visible provenance separated from privileged lineage | **H4** | communication opportunity implementation, propagation budgets, persistence/compaction, population performance |
| PA-06 | `results/PA-06.md` | actor-access-gated selected memory; causal-time selection; finite per-actor semantic capacity; deterministic pressure; bounded reason witness; declared decision consumer | **H4** | capacity/tuning, compaction/index algorithm, save footprint, long-horizon salience quality |
| PA-07 | `results/PA-07.md` | human-facing service availability/degradation; finite capacity; coarse named dependencies only where behaviour-visible; explicit wait/fail/substitute; no hidden macroeconomy | **H3** for place/service opportunity, **H4** for shared causal consequences | exact quantities/throughput/wages, replacement policy, crowd/content scale, FULL/ABSTRACT cost |
| PA-08 | `results/PA-08.md` | thin shared activity opportunity/session/outcome seam; NPC participation without player; real place/time/capacity; explicit interruption; minigame owns local play only | **H3** for opportunities/session entry, **H4** for structured aftermath | actual minigame fun, controls/UI, animation, tactics, duration/frequency tuning |
| PA-09 | `results/PA-09.md` | player as first-class semantic world-action initiator; player/NPC equivalent immediate outcomes enter compatible owners; embodied actions beyond dialogue; consequences continue after departure | **H4** after H3 interaction/place substrate | final action repertoire/controls, animation/physics, crime/combat rules, persistence, long-horizon transformation |
| PA-10 | `results/PA-10.md` | bounded causal-chain lineage/continuation/termination after legitimate cause; actor-owned decisions; bounded recruitment; explicit story collision policy; no storyteller puppetry | **H4** | pacing, chain concurrency, scheduler/data structure, save representation, authoring volume, player comprehension |
| PA-11 | `results/PA-11.md` | causal truth, actor knowledge, player-earned evidence and player conclusion stay separate; selected reconstructible events use legitimate independent channels; bounded trace retention | **H4** semantic trace production; later authored investigation consumer | notebook/UI, inspection UX, ambiguity tolerance, exact retention lifetime, authoring burden |
| PA-12 | `results/PA-12.md` | governance changes explicit institutional access/time/capacity/allocation/service conditions; actors react independently; macro decision must become third-person aftermath; repeal does not erase history | **H4** owner composition; later governance/content layer | mayoral UI, policy catalogue, legal/enforcement scope, tuning, trajectory readability/fun |
| PA-13 | `results/PA-13.md` | execution control around existing owners; separate budget dimensions; anti-thrashing; truthful degradation; SV-1 bounded local work; SV-2 bounded reconciliation; stability without rubber-town over-damping | **H3** first bounded-query/fidelity proofs where applicable; **H4** mandatory Living World control owner | all numeric budgets, population/density limits, CPU/memory/save targets, hardware thresholds, cadence/tuning |

## 3. H3 semantic handoff — minimum research obligations when implemented

This section does not expand H3 scope; it identifies research constraints on features **if H3 implements them**.

### H3-A — persistent actor identity and ordinary routine

Consume PA-01:

- routine represents expected intent, not executable choreography;
- actual state can diverge because of real place/capacity/material conditions;
- interruption releases transient claims and re-evaluates current context;
- visible schedule boundaries cannot be implemented as unexplained teleport.

### H3-B — POI/service/activity opportunity sources

Consume PA-01/07/08 and prepare PA-02:

- places expose semantic opportunities/capacity rather than selecting actor actions;
- unavailable/degraded service is a real state, not secretly selectable as normal;
- activity opportunity is separate from minigame-local rules;
- capacity/reservation exists only where contention matters and has coherent release.

### H3-C — bounded local discovery / SV-1 first proof

If H3 exposes local/place/service/activity queries, it owns the first executable SV-1 fixture for those query classes:

- fixed relevant source;
- `>=10,000` irrelevant distant actor decoys;
- unchanged semantic result;
- no global enumeration to rediscover local candidates;
- deterministic cap/partition before expensive work;
- dense-local stress cannot become unbounded all-pairs work.

This is a semantic/work-shape test. H3 does **not** need final shipping candidate counts.

### H3-D — FULL/ABSTRACT seam only if H3 introduces it

If H3 adds the first fidelity transition, it must carry PA-01 and PA-13 continuity:

- identity/context/current commitment remains coherent;
- re-entry re-evaluates current state rather than replaying stale execution;
- elapsed off-screen time alone cannot force generic omitted-tick replay when material summarized change is fixed;
- inability to represent required state is explicit.

If H3 does not introduce such a seam, SV-2 remains mandatory for H4 instead of being fake-passed.

### H3-E — instrumentation to preserve later proof reachability

Where relevant, H3 should expose structured counters/diagnostics for:

- query source/scope;
- relevant enumeration;
- candidate/eligibility counts;
- cap hits;
- reservation/claim owner and release reason;
- fidelity state/reconciliation outcome if present.

No final diagnostics UI is required by this handoff.

## 4. H4 semantic handoff — Living World core

H4 is the likely first phase where the PA spine composes as a causal world. Its future contract should remain testable against these owner-safe seams.

### H4-A — actor choice

Consume PA-02/03/04/06:

```text
actor-accessible state
+ typed relationship inputs
+ bounded selected memory inputs
+ current routine/commitment/opportunity
 -> bounded actor-owned choice
 -> explainable commitment/outcome
```

Negative: no quest/director/control layer chooses the actor action and later attributes it to PA-02.

### H4-B — information

Consume PA-04/05:

```text
canonical truth
 != actor belief
 != information transfer
 != privileged causal lineage
```

Negative: overload, tracing, debugging or optimization cannot leak canonical/lineage truth into actor decisions.

### H4-C — shared world consequences

Consume PA-07/08/09:

- material/service/activity state has normal owners;
- equivalent player/NPC actions enter compatible immediate owner paths;
- player departure does not end already-created causal state;
- minigames/actions do not directly write unrelated relationship/belief/memory state.

### H4-D — autonomous event continuation

Consume PA-10:

- chain begins after a legitimate cause;
- recruitment is bounded by real context/owner relation;
- actors retain action/response authority;
- chain has terminal/stable paths;
- authored collisions select explicit block/defer/substitute/replan without causal erasure.

### H4-E — legibility

Consume PA-11:

- real events produce legitimate potential trace surfaces;
- testimony derives from actor knowledge/memory, not canonical truth;
- repeated reports from one provenance do not become independent evidence;
- reconstructibility is bounded and may honestly end in uncertainty.

### H4-F — governance integration

Consume PA-12 only where governance implementation enters scope:

- policy changes explicit shared conditions;
- citizen response remains actor-owned;
- macro decisions produce inspectable third-person consequences;
- repeal changes future condition, not prior history.

### H4-G — simulation control

Consume PA-13:

- explicit budget dimensions for initiative/reconsideration, candidate scope, propagation, persistence, fidelity/reconciliation, aggregate overload and reservations/fairness;
- anti-thrashing with inspectable commitment/invalidation;
- truth-preserving overload/degradation/kill-switch;
- SV-1 and SV-2 executable negatives;
- deterministic normalized semantic replay/diff;
- no silent budget drops;
- paired accidental-stability vs persistent destructive and constructive transformation fixtures.

## 5. PA-13 future proof ledger

| Requirement | Semantic status after B3 | First executable obligation | What remains empirical |
|---|---|---|---|
| anti-thrashing | required | H4; H3 only if it implements autonomous reconsideration | exact hysteresis/cooldown/cadence |
| SV-1 irrelevant-population independence | required/falsifiable | H3 local query surfaces; H4 actor/event surfaces | final candidate caps, throughput, population/density target |
| dense-local bound | required/falsifiable | H3/H4 where local interactions are generated | selected partition/cap algorithm + tuned values |
| SV-2 bounded catch-up | required/falsifiable | H3 if fidelity seam exists, otherwise H4 | reconciliation thresholds/cadence/performance |
| FULL/ABSTRACT semantic continuity | required | H4, plus any earlier H3 seam | spatial/temporal approximation policy tuning |
| bounded propagation | required | H4 | chain concurrency/depth/frequency constants |
| bounded persistence | required | H4 | byte/item budgets, compaction thresholds, save targets |
| truthful degradation/kill-switch | required | H4 | activation thresholds, exact degradation ordering under real content |
| deterministic semantic replay | required | H3 owned subset + H4 composition | tooling cost/performance and platform variance |
| accidental stability | required | H4 scenario | severity/recovery tuning and long-horizon fun |
| deliberate destructive transformation | required | H4/later integration | supported action breadth and campaign balance |
| constructive transformation | required | H4/later integration | content breadth and positive-equilibrium tuning |
| shipping performance budgets | **not established** | later representative profiling/gate | all numeric CPU/memory/save/hardware thresholds |

## 6. H2 non-claim ledger

The following H2 observations remain useful but **cannot close** PA requirements:

| H2 proxy observation | Useful for | Does NOT prove |
|---|---|---|
| six characters read well at scene scale | visual density/scale/content composition | persistent actors, PA-02 agency, population scale |
| idle/walk/bounded movement works | presentation/nav/animation integration | PA-01 full routine semantics, anti-thrashing, FULL/ABSTRACT |
| minimal dialogue/bark works | presentation/readability | PA-04/05 belief/information flow |
| scene remains performant | H2 visual target only | Living World CPU/save budgets |
| authored character placement looks coherent | art/layout | autonomous opportunity discovery or SV-1 |

A future document may cite H2 as visual/content input, never as Living World runtime proof.

## 7. Deferred architecture choices

The PA corpus intentionally does not require H3/H4 to choose now between:

- utility AI / GOAP / behaviour trees / rule systems / another bounded chooser;
- ECS / GameObject / hybrid runtime representation;
- one scheduler topology;
- one spatial index;
- one persistence database/schema;
- exact event-bus/command/API shape;
- exact FULL/ABSTRACT representation;
- exact thread/job model;
- exact salience/scoring formula.

Any choice is acceptable only if the semantic fixtures remain reachable and inspectable.

## 8. Implementation-planning rule

When H3/H4 workpacks are written, each imported PA requirement should be classified explicitly as one of:

```text
IMPLEMENT_NOW
PREPARE_SEAM_ONLY
EXECUTABLE_NEGATIVE_GATE
DEFER_EMPIRICAL_TUNING
OUT_OF_SCOPE_FOR_THIS_PHASE
```

`OUT_OF_SCOPE_FOR_THIS_PHASE` does not delete the requirement; it identifies the later owner.

This prevents two opposite failures:

- importing the whole research programme into one enormous H3/H4 workpack;
- losing a PA negative gate because it was never linked to an implementation consumer.

## 9. PA-14 boundary

PA-14 remains deferred until **both** conditions hold:

1. PA-B3 is independently accepted and DocSynced;
2. H2-GATE is accepted.

PA-14 must review integrated product semantics against actual H2 boundary knowledge. This handoff index is an input to PA-14, not a substitute for it.

## 10. Handoff conclusion

The implementation handoff is intentionally asymmetric:

- **H3 establishes the persistent embodied substrate and first bounded local-query/fidelity proofs where those seams exist.**
- **H4 is the primary consumer of deeper Living World causality and PA-13 control/reconciliation semantics.**
- **later profiling establishes numbers.**

That order preserves research findings without pretending H2 proxies or synthetic spikes already validate the final runtime.

`H3_H4_IMPLEMENTATION_HANDOFF: PASS_CANDIDATE`
