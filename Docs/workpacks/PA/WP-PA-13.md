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
- both Juego2 cross-cutting amendments, especially intentional transformation.

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
- opaque decision scoring;
- FULL/ABSTRACT divergence;
- accidental anarchy;
- the opposite failure: over-damping where sustained intentional action cannot materially transform the town.

## Deliverables

- canonical PA-13 dossier;
- budget dimensions for rate/scope/persistence/fidelity/cost without invented final constants;
- cooldown/inertia/salience/event-cap/degradation strategy recommendations;
- FULL/ABSTRACT semantic-continuity requirements;
- clumsy-vs-deliberate-chaos paired fixture;
- constructive-transformation fixture;
- kill-switch/degradation rules that preserve truthfulness;
- explicit list of values that remain empirical until H3/H4/H7 runtime evidence exists.

## Acceptance

The composed design can explain how ordinary noise tends toward recoverable normality **and** how sustained coherent causes can overcome those stabilizers and produce materially different state.

Negative gates: simulation control may not directly rewrite beliefs/relationships/goals to force a designer-approved equilibrium, and no guessed numeric budget may be presented as validated.

## Deferred proof

Actual CPU/save budgets, population limits, update cadence, fidelity thresholds and tuning require instrumented runtime evidence.

## Definition of Done

PA-13 closes the remote research spine with explicit failure/cost constraints. Reviewer PASS must identify PA-14 as next but mark its H2-boundary prerequisite if unsatisfied.
