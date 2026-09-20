# WP-SCENE-03 — Interiors, access + transition model

Status: **FROZEN PLAN / NOT_STARTED**  
Class: PRODUCT / SCENARIO PREPRODUCTION (NON-FOUNDATIONAL)  
Mode: **REMOTE**  
Depends on: `WP-SCENE-01` PASS, `WP-SCENE-02` PASS

## Objective

Define how playable interiors and thresholds work as product spaces: which rooms exist, how actors/player enter, what stays abstract, and how an interior remains connected to the city's schedules, access rules and consequences.

## Work

1. Define interior archetypes and depth budgets for S2/S3/S4.
2. Define public, private, staff/service and conditional access states as product requirements without implementing permission runtime.
3. Define entrance/exit/vertical/service connection semantics.
4. Define interior-to-exterior continuity requirements for schedules, witnesses, delivery/service and later save/runtime systems.
5. Define when an interior may be separately loaded/presented without becoming a separate semantic universe.
6. Define occupancy/capacity and functional-area metadata requirements.

## Required examples

At minimum: bar, home, shop, workshop, warehouse/port facility, town-hall office/archive.

## Deliverables

- `Docs/production/SCENE_INTERIOR_MODEL.md`
- access/threshold vocabulary;
- interior archetype matrix;
- continuity and later-engine validation checklist.

## Acceptance

- Not every visible door implies an interior.
- S3 locations can be causally meaningful while using compact interiors.
- Staff/service/private spaces are representable without requiring stealth-game complexity.
- Actors can conceptually use a location while player is absent.
- Loaded/unloaded presentation cannot redefine ownership of world state.
