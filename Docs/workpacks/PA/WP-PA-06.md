# WP-PA-06 — Memory & Consequences research

Status: **COMPLETE**  
Class: **RESEARCH / NON-FOUNDATIONAL**  
Execution: **REMOTE_RESEARCH**  
Depends on: `WP-PA-05` PASS + merge + DocSync  
Blocks: `WP-PA-07` only

Accepted corrected candidate: `9ce17952e257587c0dcec5dc4b854170365545ca`  
Independent correction review: **PASS**, review `#5300025658`  
Correction PR: `#172`  
Correction merge: `fdfec568844bc0d006959c3cd207795544eca33d` on 2026-09-24  
Original research PR: `#170`, original candidate `092b3169b627da38ac66dde3f0c0856df4603423`, merge `fe0ec7d1b030b49c3ce699878a07798a4fea2cc7`; post-PASS owner audit found and #172 repaired the aggregate-boundedness and post-hoc-selection gaps before PA-07 consumption.

## Objective

Determine what an actor must remember for past events to alter later behaviour without turning every NPC into an infinite event log or duplicating truth, belief and relationship state.

## Required inputs

- accepted PA-01..05 Juego2 findings;
- current PA roadmap and cross-cutting amendments;
- donor frozen `PA-06_PLAN_MEMORY_CONSEQUENCES.md` as preregistration input, rebased away from old milestone/runtime ownership.

## Research questions

- what kinds of experience merit durable memory;
- salience/selection dimensions;
- decay, forgetting, summary and compaction;
- grudges, gratitude, habit/reputation effects without making memory the relationship owner;
- when memory should influence goals/choices;
- provenance/causal trace sufficient to explain a later consequence;
- player-originated events and off-screen events under the same selection rules;
- how recovery/normality interacts with persistent severe experiences;
- what must remain empirical for save/performance budgets.

## Deliverables

- canonical PA-06 research dossier;
- memory-vs-belief-vs-relationship ownership table;
- selected-memory model recommendation at semantic level;
- positive scenario where a past event changes a later autonomous decision;
- negative scenario proving forgotten/non-salient events do not accumulate forever;
- candidate H4/H7 fixtures;
- explicit deferred tuning/performance questions.

## Acceptance

A later autonomous decision can be causally changed by a selected past experience for an explainable reason without requiring complete biography replay.

Negative gates: memory cannot become a second truth store, automatically mirror every event, or directly overwrite relationship/belief state it does not own.

The accepted correction additionally closes two material classes before downstream consumption: the **total selected-memory surface is finitely bounded even for many distinct salient candidates**, and retention/selection at a checkpoint is **causal in time**, so later fixture needs or privileged future information cannot retroactively decide what was retained.

## Deferred proof

Exact retention windows, compaction algorithms, save footprint and runtime lookup cost require future implementation evidence.

## Definition of Done

PA-06 provides a bounded continuity model usable by later H4/H7 planning. Reviewer PASS names `WP-PA-07` next.
