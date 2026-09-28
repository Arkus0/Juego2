# WP-ART-ANIM-01 — Animation source audit + retarget/coverage baseline

Status: **PROPOSED / NOT_STARTED**
Class: PRODUCT ART / PRODUCTION INDUSTRIALIZATION
Depends on: `WP-H2F-GATE` PASS + accepted UAL/character animation inputs
Blocks: `WP-ART-ANIM-02`
Binding decision: `Docs/product/VISUAL_PRODUCTION_FACTORY_AMENDMENT.md`

## Claim

Juego2 has a lawful, traceable and technically reliable animation-source baseline with enough mapped functional coverage to build a broad reusable motion vocabulary instead of acquiring or creating clips reactively one scene at a time.

## Functional coverage families

Audit and disposition animation sources against at least these families where the game needs them:

- locomotion: idle, walk, run, starts/stops/turns and useful traversal variants;
- conversation/acting: nod, deny, point, shrug, think, anger, nervousness, laugh, beckon, look-away and related reusable gestures;
- ambient/social: sit/stand, lean, smoke, drink, read/watch/wait and similar town-life loops;
- work/activity: sweep/clean, carry, handle counter/tools/crates and representative job loops where source coverage exists;
- object interaction: pick up, receive, give, inspect, place/drop and useful hand-object actions;
- reactions: surprise, recoil, fear, anger, cover/flee initiation and bounded non-combat reactions.

These are coverage families, not a requirement that every named example has a bespoke clip. Equivalent reusable motions may satisfy a need when visually credible.

## Required work

For every admitted source/clip family record:

- source, license/provenance and exact identity;
- Humanoid/retarget compatibility;
- root-motion/in-place disposition;
- loop, transition and import settings;
- known hand/foot/prop-contact limitations;
- whether the clip is `KEEP`, `ADAPT`, `DERIVE/CLEAN`, `RECORD/CREATE`, `REJECT` or `GAP`;
- intended high-level semantic tags for downstream use without making clip names canonical gameplay semantics.

Run representative retarget tests on the actual accepted character bases. Reject a technically importable clip if visible deformation, scale, foot sliding or contact makes it unusable for the intended presentation.

## Breadth target

A planning orientation of roughly 40–60 genuinely useful clips/variants across the coverage families is encouraged if existing lawful sources support it. This is **not a numerical PASS quota**. A smaller coherent library may pass if it covers the required visual vocabulary; a larger raw dump does not pass if clips are redundant, broken or unmapped.

## Acquisition/create plan

For every material gap, choose a bounded plan: reuse/retarget, derive/clean, record/mocap from owner/team, obtain lawful free source, purchase only if the time/quality gain is material, or defer because no near-term scene needs it. Do not create a shopping list detached from concrete coverage.

## PASS-before-work acceptance contract

**Mandatory evidence:** coverage matrix, provenance ledger, representative retarget captures/tests, import/root-motion conventions, rejected/bad-retarget negative evidence and explicit gap plan.

**FAIL if:** downstream WPs would still need to rediscover sources/import conventions; raw clip count substitutes for usable coverage; licensing/provenance is unknown; retarget defects are accepted because Unity reports a valid Humanoid avatar; or every new dialogue/activity is expected to trigger a new animation-pipeline decision.

## Non-claims

No final runtime Gesture/State mapping, complete animation library, cinematic choreography, combat moveset or scene-specific acting polish. Those belong to `ART-ANIM-02` and later gameplay/action owners.
