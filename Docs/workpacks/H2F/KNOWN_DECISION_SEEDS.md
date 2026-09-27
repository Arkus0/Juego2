# H2F known decision seeds

Status: **NON-BINDING INPUT TO H2F-00**  
Date: 2026-09-27

These are hypotheses already identified before the exhaustive H2F-00 survey. They are not adopted dependencies and may be overturned by current evidence.

## Strong initial hypotheses

- `URP`: likely `ADOPT_NOW` because H2 visual closure needs a deliberate retained render baseline rather than Built-in/default-looking source presentation.
- `Cinemachine 3`: likely `ADOPT_NOW` for third-person camera and later cinematic framing.
- `Input System`: likely `ADOPT_NOW` for the retained player-control baseline.
- `Unity Splines`: likely `ADOPT_NOW` or the native baseline to beat for roads, paths, walls/fences/rails and repeated linear realization.
- `AI Navigation`: likely `ADOPT_NOW` for local path execution; Arkus remains authority over where/why actors move.
- `Terrain Tools`: likely useful for retained elevation/nature workflow; exact role must be compared against actual CITY-07 geometry needs.
- `Animation Rigging`: likely useful for bounded IK/contact correction, not a reason to build a custom animation framework.
- `Timeline`: likely `DEFER` or low-cost adoption around H2/H3 depending on whether retained H2 interaction/capture work benefits from it.
- `ProBuilder`: likely bounded editor utility rather than primary keeper construction language.

## Free/content hypotheses to verify

- Quaternius Source content and project-side capabilities should be inspected first because they may already cover shaders, windows/interiors, nature or animation needs that would otherwise cause redundant dependencies.
- Free/basic humanoid animation packs and Mixamo may be useful gap fillers after same-source Quaternius animation fit is tested.
- Free vegetation/scatter tooling may have high ROI if it is Unity 6.3/URP compatible, deterministic enough for authored exclusions and replaceable.
- Fake-interior/window-depth tooling may save substantial interior authoring if it fits the stylized target and does not conflict with real playable interiors.
- CC0 libraries such as Poly Haven/ambientCG-type sources may be valuable for sky/HDRI, stone/soil/wetness/detail textures and decal inputs while preserving the Quaternius/Juego2 stylized shape language.
- Ivy/moss tools are optional visual accelerators only if current compatibility and runtime/editor cost are acceptable.

## Likely deferrals unless evidence changes them

- DOTS/ECS as a general rewrite;
- ML-Agents;
- heavy quest frameworks;
- FMOD or other audio middleware before audio requirements justify the integration;
- complex Addressables/streaming architecture before measured scene/world scale requires it;
- commercial behaviour-tree frameworks when bounded H2 movement can be solved more simply;
- Gaia/large world generators that would compete with CITY/Arkus authored semantics;
- Odin/FinalIK or similar paid productivity/runtime tools without a demonstrated gap in the selected native baseline.

H2F-00 must still survey the capability categories independently. This document prevents the survey from forgetting useful prior hypotheses; it does not permit confirmation bias or adoption without the H2F evidence chain.
