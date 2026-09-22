# WP-PA-13 — Simulation Control, Failure Modes & Budgets research

Status: **FROZEN PLAN / NOT_STARTED**  
Class: **RESEARCH / NON-FOUNDATIONAL**  
Execution: **REMOTE_RESEARCH**  
Depends on: `WP-PA-12` PASS + merge + DocSync  
Blocks: `WP-PA-14` only

## Objective

Compose the failure findings accumulated across PA-01..12 into explicit anti-chaos, fidelity and cost requirements without turning simulation control into a hidden system that overrides subsystem semantics or protects the starting town from deliberate player action.

## Required inputs

- all accepted PA-01..12 findings;
- donor `PA-11_PLAN_SIMULATION_CONTROL.md` remapped to canonical PA-13;
- both Juego2 cross-cutting amendments, especially intentional transformation;
- provisional structural scalability evidence in `Docs/evidence/PA-13/SCALABILITY_VIABILITY_SPIKE.md`; this evidence may motivate falsifiable requirements but may not be presented as a shipping-runtime benchmark or final numeric budget.

## Failure classes to attack

- autonomy spam and oscillation/thrashing;
- identical agents / diversity collapse;
- omniscient decisions;
- rumor/event cascades and infinite propagation;
- important consequences always off-screen or illegible;
- deadlock/contention and impossible routines;
- teleport/fallback that lies about world state;
- save/history explosion;
- CPU/global-scan explosion;
- candidate/interactor explosion caused by unrelated global population or unbounded local density;
- ABSTRACT→FULL catch-up whose work grows with every omitted micro-tick rather than bounded material state/change;
- opaque decision scoring;
- FULL/ABSTRACT divergence;
- accidental anarchy;
- the opposite failure: over-damping where sustained intentional action cannot materially transform the town.

## Structural scalability invariants

PA-13 must preserve two explicit viability invariants. They are architectural requirements, not final numeric tuning constants and do not create a separate active workpack.

### SV-1 — Irrelevant-population independence / bounded local work

For any action/query class claimed to be local, known-actor, role-, place- or opportunity-scoped, unrelated population growth may not make the decision path enumerate the global persistent population first.

A future executable negative control must be able to hold the relevant source constant, add at least 10,000 irrelevant distant actors, and require that:

- the semantic decision/result is unchanged;
- relevant enumeration and expensive comparison remain unchanged or under the same declared deterministic cap;
- unrelated actors outside the authorized source/scope are not inspected merely to rediscover the relevant set;
- local crowd density cannot create an unbounded all-pairs candidate surface: spatial/indexed discovery still needs an explicit deterministic local cap, partition, reservation surface or equivalent bound before expensive interaction scoring;
- cap hits/degradation are inspectable and truthful rather than silently order-dependent.

A spatial index alone is not sufficient if one cell/neighbourhood can accumulate unbounded candidates and every pair is still compared.

### SV-2 — Bounded ABSTRACT→FULL reconciliation

Cheaper ABSTRACT execution is allowed only if promotion/re-entry to FULL does not require replaying every omitted simulation micro-tick for every actor.

A future executable negative control must increase off-screen elapsed time by orders of magnitude while holding the amount of materially relevant summarized change constant, and require that reconciliation work remains bounded by the material change/reconciliation budget rather than proportional to all missed micro-steps.

Allowed future techniques include summarized state transitions, scheduled/material events, compact causal deltas, bounded catch-up windows and explicit degradation/failure. PA-13 does not select the implementation. It does require truthfulness: promotion may not invent state, silently erase material consequences or claim semantic continuity that the cheaper representation cannot reconcile.

If a domain genuinely requires elapsed-time-dependent work, it must expose and bound that cost explicitly rather than hiding an unbounded tick replay behind `ABSTRACT`.

## Deliverables

- canonical PA-13 dossier;
- budget dimensions for rate/scope/persistence/fidelity/cost without invented final constants;
- cooldown/inertia/salience/event-cap/degradation strategy recommendations;
- FULL/ABSTRACT semantic-continuity requirements;
- explicit adoption/reconciliation of SV-1 and SV-2, including which later runtime owner must make each executable;
- clumsy-vs-deliberate-chaos paired fixture;
- constructive-transformation fixture;
- kill-switch/degradation rules that preserve truthfulness;
- explicit list of values that remain empirical until H3/H4/H7 runtime evidence exists.

## Acceptance

The composed design can explain how ordinary noise tends toward recoverable normality **and** how sustained coherent causes can overcome those stabilizers and produce materially different state.

It must also preserve SV-1 and SV-2 as falsifiable requirements: adding irrelevant population cannot turn a bounded decision into a hidden global/all-pairs scan, and increasing off-screen elapsed micro-time cannot by itself force unbounded ABSTRACT→FULL replay when material summarized change is held constant.

Negative gates: simulation control may not directly rewrite beliefs/relationships/goals to force a designer-approved equilibrium; a spatial/index structure may not be presented as proof of bounded work without a bound on the candidate surface it yields; ABSTRACT mode may not defer an unbounded micro-tick debt to later FULL promotion; and no guessed numeric budget may be presented as validated.

## Deferred proof

Actual CPU/save budgets, population limits, update cadence, fidelity thresholds and tuning require instrumented runtime evidence. SV-1/SV-2 can be structurally falsified before those final budgets exist; their shipping thresholds remain empirical until representative H3/H4/H7 runtime and hardware evidence is available.

## Definition of Done

PA-13 closes the remote research spine with explicit failure/cost constraints and carries SV-1/SV-2 forward into the appropriate runtime acceptance fixtures. Reviewer PASS must identify PA-14 as next but mark its H2-boundary prerequisite if unsatisfied.
