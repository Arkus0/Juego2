# WP-PA-03 — Adopt/revalidate Social Graph findings

Status: **COMPLETE**  
Class: **RESEARCH / NON-FOUNDATIONAL**  
Execution: **REMOTE_HARVEST**  
Depends on: `WP-PA-02` PASS + merge + DocSync  
Blocks: `WP-PA-04` only

Accepted candidate: `d216f32f0c82bf57c23a3c7ef0c4433cbcbdc927`  
Independent review: **PASS**, review `#5270421825`  
Merged: PR `#100`, merge commit `2f3862b601b0521a6a3d5a57afe54f182d037e97` on 2026-09-21

## Objective

Carry forward the donor PA-03 result that relationships must change behaviour, not just dialogue flavour, while simplifying it for Juego2's current product scope.

## Required inputs

- accepted PA-01/02 Juego2 findings;
- PA roadmap, harvest and amendments;
- donor `PA-03_SOCIAL_GRAPH.md`;
- donor PASS on `205aab2ae2cdd7cfd8b5fda987cbf3bbd6cba41c` / review `5226298241`.

## Work

- revalidate directed/asymmetric relationship semantics;
- classify useful relationship families such as household/family, affinity, trust, fear, authority/status, debts/favours/obligations, employment and rivalry without forcing them into one universal score vector;
- preserve the requirement that relationships alter action/target/opportunity selection;
- separate current relationship state from unbounded relationship biography;
- reconcile relationship changes caused by player actions, activities, rumours, memory and governance without letting PA-03 own those other systems;
- define minimum explanation requirements for a relationship-dependent decision.

## Deliverables

- canonical Juego2 PA-03 finding document;
- minimal relationship vocabulary recommendation with `ADOPT/ADAPT/LATER/REJECT` status;
- same-world/different-edge counterfactual fixture;
- ownership matrix for relationship vs memory/belief/rumour/work;
- future H4 consumer notes and deferred proof.

## Acceptance

Same external situation with a materially different directed relationship edge must be able to produce a different explainable choice or target without requiring bespoke quest logic.

Negative control: changing only A→B must not silently imply the same change in B→A.

## Deferred proof

Runtime storage, tuning, UI, network scale, persistence and final relationship schemas belong to future consumers.

## Definition of Done

Juego2 has reviewed relationship semantics strong enough to constrain H4 without freezing a runtime model. Reviewer PASS names `WP-PA-04` next.
