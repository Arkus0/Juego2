# WP-ART-ANIM-02 — Reusable animation vocabulary + runtime mapping

Status: **PROPOSED / NOT_STARTED**
Class: PRODUCT ART / PRODUCTION INDUSTRIALIZATION
Depends on: `WP-ART-ANIM-01` PASS + `WP-ART-CHAR-02` PASS + accepted H2F/GC2 Core foundation
Blocks: `WP-H2-03`, `WP-ART-03`
Binding decision: `Docs/product/VISUAL_PRODUCTION_FACTORY_AMENDMENT.md`

## Claim

Juego2 has a reusable, extendable animation vocabulary that can be consumed by ordinary characters, Dialogue presentation and later Behavior/gameplay WPs without repeatedly inventing animation plumbing or per-scene import conventions.

## Required work

Turn the accepted `ART-ANIM-01` sources into a reviewed in-project library and map the motions onto the admitted runtime presentation surfaces.

Where GC2 Core provides the accepted execution primitive, use its Gesture/State/Character surfaces rather than building parallel animation managers. Otherwise use the exact H2F-admitted equivalent and record the boundary.

At minimum prove useful coverage across:

- locomotion;
- conversation/acting;
- ambient/social loops;
- work/activity actions;
- object handling;
- reactions.

The library must distinguish one-shot gestures, persistent states/loops, locomotion/root-motion clips and actions that require a prop/contact setup.

## Reusable authoring surface

Provide semantic author-facing names/categories such as `gesture.nod`, `gesture.point`, `state.lean`, `activity.sweep`, `object.receive` or equivalent reviewed vocabulary. These names are presentation/authoring vocabulary, not Arkus canonical world facts.

Document how a future Worker adds one new animation:

`source/provenance -> import/retarget -> classify -> map -> preview/test -> admit`

Adding a normal animation must not require a new manager, event bus or bespoke state machine.

## Demonstration

Use factory-produced characters from `ART-CHAR-02` to show a compact presentation reel or playable fixture that exercises multiple clips from every required family. Include at least one sequence that combines locomotion -> conversational gesture/state -> object interaction or ambient action -> recovery to ordinary locomotion.

Prove that the same admitted gesture/state can be reused on more than one character without character-specific code.

## PASS-before-work acceptance contract

**Mandatory evidence:** library manifest/categories, provenance links, runtime mappings, third-person captures/recordings, multi-character reuse proof, one negative/unsupported mapping case, and the documented future-animation intake recipe.

**FAIL if:** the library is just a folder of clips with no authoring vocabulary; every NPC has a bespoke Animator/controller path; Dialogue/Behavior would need to know raw clip paths or implementation internals; retarget quality is visibly unacceptable; or adding one ordinary gesture requires architecture work.

**Allowed residuals:** scene-specific hero acting, facial animation breadth, combat/chase animations owned later, rare occupations and bespoke story beats.

## Non-claims

No Behavior 2 routine system, combat moveset, cinematic timeline authoring system or final NPC AI.
