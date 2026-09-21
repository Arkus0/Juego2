# WP-PA-02 — Adopt/revalidate NPC Agency findings

Status: **FROZEN PLAN / NOT_STARTED**  
Class: **RESEARCH / NON-FOUNDATIONAL**  
Execution: **REMOTE_HARVEST**  
Depends on: `WP-PA-01` PASS + merge + DocSync  
Blocks: `WP-PA-03` only

## Objective

Adopt the useful independently reviewed donor PA-02 findings into Juego2 while preserving implementation freedom and the current playable-causal-city thesis.

## Required inputs

- accepted Juego2 PA-01 finding;
- PA roadmap + legacy harvest + cross-cutting amendments;
- donor `PA-02_NPC_AGENCY.md`;
- donor PASS on `dfe8a2b10831774f846274143c58dc41227d6231` / review `5225572191`.

## Work

Revalidate, without assuming a universal algorithm:

- NPCs as valid causal initiators;
- bounded candidate/action/target discovery;
- goals/pressures/opportunity/current commitment as decision inputs;
- explicit initiator/receiver ownership for social actions;
- cooldown/inertia/commitment and failure/replanning guardrails;
- off-screen agency preserving causal meaning;
- deterministic/explainable decision traces;
- separation between PersistentActor-grade agency and cheaper ambient presence.

Explicitly test whether current player-first causal requirements expose gaps in a donor model designed mainly around NPC autonomy.

Do not select Utility AI, GOAP, rules, planners or LLM control as universal authority unless new evidence makes such a choice unavoidable; that would require a separately reviewable later architecture decision.

## Deliverables

- canonical Juego2 PA-02 finding document;
- mechanism disposition matrix;
- bounded discovery / anti-global-scan requirements;
- actor→actor positive scenario and no-player-trigger negative/control scenario;
- future H3/H4/H7 consumer notes;
- deferred empirical proof list.

## Acceptance

The finding must support a deterministic bounded scenario where an NPC initiates a meaningful action for an explainable reason and affects another actor/world state without a privileged player trigger or global population scan.

Negative control: adding irrelevant distant actors must not alter candidate discovery cost/result solely because a global scan exists.

## Deferred proof

Actual decision runtime, CPU budgets, animation/action execution, persistence and off-screen fidelity belong to later implementation phases.

## Definition of Done

Juego2 has a reviewed agency requirement set without importing donor BehaviourResolver architecture. Reviewer PASS names `WP-PA-03` next.
