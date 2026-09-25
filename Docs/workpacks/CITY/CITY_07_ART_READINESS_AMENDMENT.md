# CITY-07 art-kit + assembly-readiness amendment

Status: **PROPOSED / NOT ACCEPTED**  
Class: DOCS_ONLY / PRODUCT-SCOPE CLARIFICATION  
Scope: `WP-CITY-07` only  
Causal owner amended: `WP-CITY-07` prerequisite/readiness boundary  
Related owner: `WP-ART-01`  
Origin: owner review of the disposable Astra scene on 2026-09-25 + non-canonical conclusions in PR `#228`

## Purpose

Close two newly observed failure modes before CITY-07 keeper work begins:

1. **vocabulary failure** — too few keeper-capable pieces, causing primitive fallback;
2. **assembly failure** — pieces exist, but the authoring process has no explicit layered construction model, so buildings/streets become boxes or stacked planes with assets attached.

The accepted CITY-07 game-space amendment already says the keeper must not be a literal greybox transcription. Effective Astra exploration demonstrated a stronger version of the same problem: a scene can satisfy a superficial "real assets are present" condition while still being visibly built from primitive boxes/planes with windows, roofs, signs or materials attached on top.

That is not a keeper game-space. It is a proxy scene with decorative assets.

This amendment therefore adds an explicit **art-kit + assembly-readiness prerequisite** and a fail-closed visible-proxy rule to CITY-07.

## New prerequisite

`WP-CITY-07` may start keeper realization only after all are true:

1. `WP-CITY-04` is accepted;
2. `WP-H1-GATE` is accepted;
3. `WP-ART-01` is accepted for the representative retained-chain vocabulary and its layered assembly grammar.

ART-01 may execute in parallel with the remaining H1 infrastructure chain once its own predecessors are satisfied. This amendment does not make ART-01 a prerequisite for H1-GATE.

The assembly companion expected from ART-01 is:

`Docs/workpacks/ART/ART_ENVIRONMENT_ASSEMBLY_GRAMMAR.md`

## Why this belongs before CITY-07

H1 proves source/catalogue/projection/materialization/reconciliation capability. It deliberately does not prove production-art breadth, Cantabrian conversion or environment construction semantics.

CITY-07 owns keeper spatial composition. It should not also have to discover from scratch whether the available source kit can produce believable retained architecture or how walls/openings/roofs/thresholds/ground should connect.

Without this prerequisite CITY-07 could be forced into bad outcomes:

- retain visually incompatible Quaternius shapes because they are the only real assets admitted;
- manufacture visible primitive shells and decorate them with a few admitted assets;
- stack road/kerb/ground planes that are each individually plausible but collectively form an incoherent traversable surface;
- place roof/window/door assets without a host/support/meeting model and call the result assembled.

ART-01 closes the vocabulary and first-slice assembly gap first.

## Keeper-visible fallback states

CITY-07 must consume the ART-01 readiness states:

- `KEEPER_READY` — accepted for retained visible realization;
- `PROXY_VISUAL` — allowed only as explicitly temporary/non-keeper evidence;
- `COVERAGE_BLOCKED` — required visual/content/connection capability is missing and keeper realization for that element cannot claim PASS.

A `PROXY_VISUAL` or `COVERAGE_BLOCKED` element may exist in a working scene, but it cannot be hidden inside the CITY-07 acceptance claim.

## New CITY-07 assembly obligation

For the representative keeper chain, CITY-07 must show that accepted assets/compositions form the **actual architecture and environment**, not merely surface decoration on surviving blockout primitives.

At least one representative building and one representative street segment must have a compact, reviewable assembly plan derived from the ART-01 grammar.

### Representative building plan

At minimum record the applicable sequence:

1. site / terrain datum;
2. footprint + principal access level;
3. base/plinth/foundation or retaining response;
4. primary/secondary massing and storey relationships;
5. exterior wall/facade host surfaces and corner/end conditions;
6. openings + inserted doors/windows or accepted integrated facade modules;
7. roof plan + eaves/ridge/termination + roof-to-wall meeting;
8. public/service/private threshold realization required by accepted CITY semantics;
9. building-to-ground / building-to-street connection;
10. dressing/vegetation only after host structure and transitions are coherent.

CITY-07 does not need a BIM model or structural-engineering calculation. The plan only needs to be explicit enough to falsify `decorated cuboid` assembly.

### Representative street plan

At minimum record:

1. terrain/support datum;
2. principal traversable platform/surface;
3. accepted width/grade/elevation class;
4. kerb/shoulder/pavement/drainage/readable edge treatment where applicable;
5. retaining/garden/building contacts;
6. player-facing threshold transitions;
7. collision/traversable-surface ownership;
8. props/vegetation after the surface system is coherent.

A keeper street cannot PASS as a stack of accidental overlapping planes/cubes whose conflicts are merely hidden by materials.

## Connection evidence

The representative chain must evidence applicable host/support/meeting relations such as:

- facade `HOSTS` opening;
- door/window `FILLS` opening;
- roof/eave `CAPS` or `MEETS` the supported mass;
- porch/prop/sign `SUPPORTED_BY` or `ATTACHED_TO` a valid host;
- street `TRANSITIONS_TO` threshold/step/landing;
- wall/kerb/retaining edge `MEETS` the traversable/ground surface coherently;
- vegetation/props remain `CLEAR_OF` required route/access spaces.

This evidence may be compact and does not require all relations to become H0 canonical semantics in CITY-07.

## Third-person review additions

The review must explicitly inspect:

- building massing/silhouette;
- facade/opening depth and threshold treatment;
- roof-to-wall integration;
- building-to-ground and street-edge transitions;
- corner/junction composition and local setbacks/retaining/steps where applicable;
- whether streets/public ground are one coherent retained surface system rather than hidden overlapping blockout layers;
- vegetation/props as supporting composition rather than isolated scatter;
- human-scale reading with at least one player/NPC-scale proxy.

CITY-07 still owns spatial composition; ART-01 ensures the vocabulary and first retained assembly grammar are sufficient and visually admissible.

## Use of PR #228 CITY ideas

PR `#228` contains useful non-canonical CITY-07 concept work plus proposed P1/P8/P9 amendments.

This amendment does **not** adopt those proposals automatically. If they have been independently reviewed, merged and DocSynced before CITY-07 executes, CITY-07 consumes the accepted subset and its corresponding measurements. If they are still proposed, CITY-07 must not realize them as canonical truth.

The concept's recommended representative chain is compatible with the existing accepted seed and may be used without adopting P1/P8/P9:

`Orilla sur → Puente Viejo → S02 bridgehead → W12 → Casco square → casco micro-route B → Bar F01`

The concept's broader lessons are also consumed as design lenses where they do not alter accepted semantics:

- increase readable route learning and local stage variety rather than leaving the seed as a flat planning diagram;
- prefer meaningful frontage/threshold density over empty acreage;
- use local quiet/busy contrast rather than uniform saturation;
- use landmarks, framed views, small squares, lanes and discovery surfaces when their causal owner has accepted them;
- judge the result from the player camera, not only from top-down planning views.

If P8/P9 later become authoritative, their route-loop and place-density obligations extend the asset-rich demo rather than being approximated through unaccepted geometry.

## Acceptance addition

CITY-07 cannot PASS unless the representative keeper chain satisfies all of:

1. the existing semantic/physical game-space obligations;
2. the ART-01 keeper-visible composition boundary;
3. the layered assembly obligation for at least one building and one street segment.

Specifically, PASS requires that no materially visible keeper building or street assembly relies on crude primitive proxy geometry **because the production kit or connection grammar lacks the required architectural/environment role**, unless that geometry is intentionally authored retained geometry and itself meets the reviewed visual criteria.

A simple cuboid is not banned categorically. It only passes when it is a deliberate retained mass whose openings, corners, roof, threshold, ground contact and visual read satisfy the same keeper criteria. A primitive used merely as an unresolved host for decorative assets does not pass.

## Negative gates

FAIL if:

- the scene is presented as keeper-ready while a visible building is still fundamentally a greybox cuboid/plane with windows, doors, roof or facade detail attached on top;
- a window/door/sign is effectively a sticker or intersection because no host/opening/attachment relation is resolved;
- a roof floats, clips or behaves as a decorative cap without a coherent supported meeting condition;
- a building emerges from an undifferentiated ground plane with no deliberate threshold/base/ground-contact treatment where the player can inspect it;
- a retained street remains an accidental stack of duplicate/coplanar traversable surfaces or collision layers;
- a missing art-kit or connection role is hidden with materials, decals, props or dressing instead of being surfaced as `COVERAGE_BLOCKED`;
- `PROXY_VISUAL` content is counted as proof that the keeper environment is finished;
- CITY-07 silently uses Alpine/fantasy source shapes vetoed by the visual bible because they are convenient;
- CITY-07 reopens H1 bridge semantics to compensate for an art/content/assembly gap;
- or PR `#228` P1/P8/P9 proposal content is treated as accepted before independent review + merge + DocSync.

## Result

The relevant readiness sequence becomes:

```text
CITY-04 PASS                    H1 bridge chain -> H1-GATE PASS
    ↓                                      ↓
ART-01 kit + assembly readiness PASS       ↓
    └──────────────────────┬───────────────┘
                           ↓
CITY-07 keeper game-space realization
```

This prevents the first retained slice from becoming raw Quaternius Medieval Village, a dressed greybox, or a pile of individually plausible assets with no coherent construction logic, while preserving the ownership boundaries: ART owns visual/content vocabulary and first-slice assembly grammar; H1 owns the bridge; CITY owns the keeper spatial composition.