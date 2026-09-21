# WP-PA-04 — Adopt/revalidate Knowledge & Belief findings

Status: **FROZEN PLAN / NOT_STARTED**  
Class: **RESEARCH / NON-FOUNDATIONAL**  
Execution: **REMOTE_HARVEST**  
Depends on: `WP-PA-03` PASS + merge + DocSync  
Blocks: `WP-PA-05` only

## Objective

Adopt the donor PA-04 epistemic boundary into Juego2: actors act on what they can legitimately know/believe, not on omniscient canonical truth.

## Required inputs

- accepted PA-01..03 Juego2 findings;
- PA roadmap, harvest and current amendments;
- donor frozen `PA-04_PLAN_KNOWLEDGE.md` + `PA-04_KNOWLEDGE.md`;
- donor PASS on `672dcfa46dc1212d43f5302b4937ddd345bf249a` / PR #41.

## Work

- preserve the `canonical truth != actor belief` boundary;
- revalidate legitimate acquisition via perception, communication and accessed public sources;
- cover certainty/partial/stale/false belief, source/provenance, secrets, concealment and deception at product-semantic level;
- ensure player and NPC actions can create information asymmetry without privileged quest flags;
- define query/decision boundaries that distinguish actor-accessible knowledge from engine/debug truth;
- preserve compatibility with PA-05 transfer and PA-06 memory without duplicating either owner.

No final belief database/schema or numerical confidence formula is owned here.

## Deliverables

- canonical Juego2 PA-04 finding document;
- epistemic authority diagram;
- acquisition/provenance rules;
- two-actor same-world/different-belief counterfactual;
- hidden-truth leakage negative fixture;
- future H4 consumer notes and deferred proof.

## Acceptance

Two actors in the same canonical world state can hold different justified beliefs and therefore make different explainable decisions; hidden engine truth cannot make an actor smarter without a valid acquisition path.

Negative control: changing only privileged/debug truth metadata that an actor cannot access must not alter that actor's decision input.

## Deferred proof

Runtime representation, dialogue integration, persistence, confidence tuning, perception implementation and player-facing UI belong to later phases.

## Definition of Done

Juego2 has an independently reviewed epistemic contract at research level. Reviewer PASS names `WP-PA-05` next.
