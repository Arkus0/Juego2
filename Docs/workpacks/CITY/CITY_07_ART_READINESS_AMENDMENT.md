# CITY-07 art-kit readiness amendment

Status: **PROPOSED / NOT ACCEPTED**  
Class: DOCS_ONLY / PRODUCT-SCOPE CLARIFICATION  
Scope: `WP-CITY-07` only  
Causal owner amended: `WP-CITY-07` prerequisite/readiness boundary  
Related owner: `WP-ART-01`  
Origin: owner review of the disposable Astra scene on 2026-09-25 + non-canonical conclusions in PR `#228`

## Purpose

Close a newly observed failure mode before CITY-07 keeper work begins.

The accepted CITY-07 game-space amendment already says the keeper must not be a literal greybox transcription. Effective Astra exploration then demonstrated a stronger version of the same problem: a scene can satisfy a superficial "real assets are present" condition while still being visibly built from primitive boxes/planes with windows, roofs, signs or materials attached on top.

That is not a keeper game-space. It is a proxy scene with decorative assets.

This amendment therefore adds an explicit **art-kit readiness prerequisite** and a fail-closed visible-proxy rule to CITY-07.

## New prerequisite

`WP-CITY-07` may start keeper realization only after all are true:

1. `WP-CITY-04` is accepted;
2. `WP-H1-GATE` is accepted;
3. `WP-ART-01` is accepted for the representative retained-chain vocabulary.

ART-01 may execute in parallel with the remaining H1 infrastructure chain once its own predecessors are satisfied. This amendment does not make ART-01 a prerequisite for H1-GATE.

## Why this belongs before CITY-07

H1 proves source/catalogue/projection/materialization/reconciliation capability. It deliberately does not prove production-art breadth or Cantabrian conversion.

CITY-07 owns keeper spatial composition. It should not also have to discover from scratch whether the available source kit can produce believable retained architecture. If the required vocabulary is absent, CITY-07 would otherwise be forced into one of two bad outcomes:

- retain visually incompatible Quaternius shapes because they are the only real assets admitted; or
- manufacture visible primitive shells and decorate them with a few admitted assets, recreating the greybox under a different surface treatment.

ART-01 closes that product/content gap first.

## Keeper-visible fallback states

CITY-07 must consume the ART-01 readiness states:

- `KEEPER_READY` — accepted for retained visible realization;
- `PROXY_VISUAL` — allowed only as explicitly temporary/non-keeper evidence;
- `COVERAGE_BLOCKED` — required visual/content capability is missing and keeper realization for that element cannot claim PASS.

A `PROXY_VISUAL` or `COVERAGE_BLOCKED` element may exist in a working scene, but it cannot be hidden inside the CITY-07 acceptance claim.

## New CITY-07 visual-composition obligation

For the representative keeper chain, CITY-07 must show that accepted assets/compositions form the **actual architecture and environment**, not merely surface decoration on surviving blockout primitives.

The third-person review must explicitly inspect:

- building massing/silhouette;
- facade/opening depth and threshold treatment;
- roof-to-wall integration;
- building-to-ground and street-edge transitions;
- corner/junction composition and local setbacks/retaining/steps where applicable;
- vegetation/props as supporting composition rather than isolated scatter;
- human-scale reading with at least one player/NPC-scale proxy.

CITY-07 still owns spatial composition; ART-01 merely ensures the vocabulary is sufficient and visually admissible.

## Use of PR #228 CITY ideas

PR `#228` contains useful non-canonical CITY-07 concept work plus proposed P1/P8/P9 amendments.

This amendment does **not** adopt those proposals automatically. If they have been independently reviewed, merged and DocSynced before CITY-07 executes, CITY-07 consumes the accepted subset and its corresponding measurements. If they are still proposed, CITY-07 must not realize them as canonical truth.

The concept's recommended representative chain is compatible with the existing accepted seed and may be used without adopting P1/P8/P9:

`Orilla sur → Puente Viejo → S02 bridgehead → W12 → Casco square → casco micro-route B → Bar F01`

If P8/P9 later become authoritative, their route-loop and place-density obligations extend the asset-rich demo rather than being approximated through unaccepted geometry.

## Acceptance addition

CITY-07 cannot PASS unless the representative keeper chain satisfies both:

1. the existing semantic/physical game-space obligations; and
2. the ART-01 keeper-visible composition boundary.

Specifically, PASS requires that no materially visible keeper building or street assembly relies on crude primitive proxy geometry **because the production kit lacks the required architectural/environment role**, unless that geometry is intentionally authored retained geometry and itself meets the reviewed visual criteria.

## Negative gates

FAIL if:

- the scene is presented as keeper-ready while a visible building is still fundamentally a greybox cuboid/plane with windows, doors, roof or facade detail attached on top;
- a missing art-kit role is hidden with materials, decals or dressing instead of being surfaced as `COVERAGE_BLOCKED`;
- `PROXY_VISUAL` content is counted as proof that the keeper environment is finished;
- CITY-07 silently uses Alpine/fantasy source shapes vetoed by the visual bible because they are convenient;
- CITY-07 reopens H1 bridge semantics to compensate for an art/content gap;
- or PR `#228` P1/P8/P9 proposal content is treated as accepted before independent review + merge + DocSync.

## Result

The relevant readiness sequence becomes:

```text
CITY-04 PASS                    H1 bridge chain → H1-GATE PASS
    ↓                                      ↓
ART-01 production-kit readiness PASS      ↓
    └──────────────────────┬───────────────┘
                           ↓
CITY-07 keeper game-space realization
```

This prevents the first retained slice from becoming either raw Quaternius Medieval Village or a dressed greybox, while preserving the existing ownership boundaries: ART owns visual/content vocabulary; H1 owns the bridge; CITY owns the keeper spatial composition.