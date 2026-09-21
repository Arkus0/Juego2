# WP-PA-01 — Adopt/revalidate NPC Daily Life findings

Status: **COMPLETE**  
Class: **RESEARCH / NON-FOUNDATIONAL**  
Execution: **REMOTE_HARVEST**  
Depends on: accepted PA programme plan  
Blocks: `WP-PA-02` only

Accepted candidate: `87c4cbe81f8195770eb23ba7f2a5d5ca2a237715`  
Independent review: **PASS**, review `#5268013445`  
Merged: PR `#89`, merge commit `3688b7b9a27355b0fda160c20e57a385e40c6814` on 2026-09-21

## Objective

Turn the independently reviewed `Arkus0/Juego` PA-01 Daily Life dossier into a compact Juego2 research finding without repeating the expensive prior-art study and without importing donor runtime architecture.

## Required inputs

- `Docs/research/living-world/PA_ROADMAP.md`;
- `Docs/research/living-world/PA_LEGACY_HARVEST.md`;
- current cross-cutting Living World amendments;
- donor `Docs/living-city-research/PA-01_NPC_DAILY_LIFE.md`;
- donor PR #32 review history, including PASS on `42f08346fbf518eb19e1158ea8448926a1a4d86b`.

## Work

- audit the donor findings against current Juego2 product direction;
- preserve schedule-as-intent, activity/affordance separation, interruption/cleanup/re-evaluation, expected-vs-actual routine, bounded calendar overrides, capacity/contention and FULL/ABSTRACT causal continuity where evidence still supports them;
- strip old M9/M10 ownership and any preselected runtime representation;
- explicitly reconcile ordinary routine with player causal agency, activities and intentional world transformation;
- state which claims remain `ADOPT`, need `ADAPT`, are `LATER`, or must be `REJECT`;
- define at least one positive and one negative headless/product scenario for later consumers.

Broad new prior-art research is forbidden unless the audit identifies a concrete unsupported/conflicting claim.

## Deliverables

- canonical Juego2 `PA-01` finding document;
- donor→Juego2 provenance table;
- accepted/adapted/rejected finding matrix;
- future H3 consumer notes;
- deferred empirical proof list.

## Acceptance

PASS only if a normal NPC day can be specified as understandable intent + compatible activity/place + interruption/recovery behaviour without becoming a unique waypoint screenplay, while leaving pathfinding/runtime implementation unowned.

Negative control: changing only route realization must not silently rewrite the actor's authored schedule intent.

## Deferred proof

H3+ must later prove navigation, animation, timing, contention, save/runtime continuity and actual player readability. This WP claims none of those.

## Definition of Done

Juego2 owns a reviewed PA-01 product finding and no longer needs the donor repo to understand the intended daily-life semantics. Reviewer PASS must name `WP-PA-02` as next.
