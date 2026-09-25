# H2 — Keeper World + Visual Identity

Status: **PROPOSED PLAN / NOT STARTED**  
Class: PRODUCT PHASE  
Execution default: `PRODUCT_CHECKPOINT`  
Plan date: 2026-09-25

## 1. Phase claim

H2 answers one product question:

> **Can Arkus create, maintain and locally improve a small keeper third-person Juego2 zone that already expresses the intended final visual language, and can that zone be convincingly occupied by visually representative characters with only minimal movement and dialogue?**

H2 is not the Living World milestone. It deliberately finishes the **body** of the game before H3+ gives that body deeper behaviour.

At H2 exit, a player should be able to walk the representative keeper slice and say: **this is what Juego2 looks and feels like spatially**. Later phases may add content and improve assets inside that language; they should not still be discovering whether the game is a Quaternius asset pack, a greybox with decorations, or a coherent fictional Liébana game world.

## 2. Binding predecessors / inputs

H2 consumes without reopening:

- accepted H0 canonical authoring/validation/replay/public-surface guarantees;
- accepted H1 Unity materialization/observation/reconciliation boundary and `WP-H1-GATE`;
- accepted CITY geography/programme/seed and the final `WP-CITY-07` keeper slice;
- accepted ART-00 visual direction;
- accepted ART-01 production kit, assembly grammar, dimensional profile and `KEEPER_READY` boundary;
- accepted PR #229 ART/CITY/H2 planning consequences: no dressed-greybox false PASS, shared metric/elevation truth, bounded fast visual iteration and third-person evidence;
- accepted DW/CTX evidence where it improves public discovery/context, without turning fresh external-repository packaging into an H2 visual/product blocker.

`PROXY_VISUAL` or `COVERAGE_BLOCKED` may be truthful execution results, but they do not satisfy a required H2 keeper-readiness claim.

## 3. Representative retained benchmark

The default H2 benchmark is the retained human-scale chain:

`Orilla/Puente Viejo -> bridgehead/S02 -> W12/Casco -> Bar F01 exterior -> Bar F01 public interior`

The final fixture may use the exact accepted CITY-07 realization of that chain. It must contain enough exterior, elevation change, street edge, thresholds and interior transition to expose composition and scale failures.

H2 does not require the whole town.

## 4. H2 exit quality bar

H2-GATE may PASS only when all of the following are true:

1. **Keeper world** — the representative zone is retained shipping structure, not a disposable demo layout.
2. **Visual-language lock** — architecture, ground/street treatment, nature, material read, lighting/atmosphere, props, character proportions/wardrobe and density form one approved Juego2 visual language at third-person scale.
3. **No dressed greybox** — required environment roles/connections are `KEEPER_READY`; anonymous boxes/planes cannot be hidden by windows, roofs, signs, materials, vegetation or props.
4. **Playable shell** — third-person movement/camera, collision/navigation and a bounded interaction path work through the keeper slice, including the Bar transition.
5. **Character presentation** — at least six human-scale character presentations are available in the benchmark with approved scale, clothing/silhouette variation and basic animation presentation.
6. **Minimal NPC behaviour only** — benchmark characters can idle, walk/move between bounded destinations and answer one minimal interaction/dialogue/bark path. Nothing stronger is implied.
7. **Occupied-space proof** — the slice is inspected with non-empty representative occupancy so street/plaza/interior scale is not validated only while empty.
8. **AI-native world authoring** — a fresh capable author can use accepted Arkus public surfaces to inspect the bounded world, discover legal composition/semantic options, make one localized semantic improvement, materialize/reconcile it and prove unrelated accepted content was not silently regenerated or damaged.
9. **Reuse/economics proof** — the former CITY-08 claim is discharged here: at least one reviewed composition/relation is reused or varied with observably lower authoring effort than first assembly while preserving constraints.
10. **Truthful failure** — unavailable required coverage, invalid connections or unsupported edits fail visibly instead of being improvised into final-looking success.

A hero screenshot alone cannot satisfy H2-GATE.

## 5. Explicit H2 non-claims

H2 does **not** own:

- persistent NPC identity as a gameplay guarantee;
- clock or full daily schedules;
- jobs/work simulation;
- needs/goals;
- relationships or social graph runtime;
- memory, beliefs or rumours;
- autonomous events/causal chains;
- investigation simulation;
- governance runtime;
- save-game Living World continuity;
- combat;
- town-wide population scaling;
- final quantity of shippable NPCs;
- fresh external-repository packaging/version distribution of Arkus.

Those remain later consumer problems unless a reviewed amendment proves a smaller prerequisite is unavoidable.

## 6. H2 workpack DAG

```text
accepted H2 plan
      + WP-H1-GATE + ART-01 ----------------------> H2-01 world authoring
      |
      + WP-H1-GATE + CITY-07 --------------------> H2-02 playable shell
                                                      ↓
                                                   H2-03 character presentation
                                                      ↓
                                                   ART-02 visual closure
                                                      ↓
H2-01 -----------------------------------------------+
                                                     ↓
                                                  H2-GATE
```

`H2-01` may proceed in parallel with late CITY keeper work only to the extent its own fixtures do not pretend CITY-07 is already accepted. Its final acceptance must consume the accepted keeper constraints it claims to support.

## 7. Workpacks

### `WP-H2-01 — Semantic world authoring + localized edit loop`

Prove typed world concepts above raw transforms: route/street, site, building/shell, facade/host/opening, threshold/portal, interior, landmark/edge, activity/wait node where justified, shared metric/elevation relations and approved composition identity.

A fresh author must be able to inspect a bounded brief, discover legal assets/compositions, create or alter a local plan, materialize it through H1, inspect the result and make one bounded semantic correction without full-scene regeneration.

This is the authoring capability claim, not visual polish.

### `WP-H2-02 — Keeper playable shell`

On the accepted CITY-07 slice, provide the retained third-person player shell: movement, camera, collision/traversal, basic navigation support and bounded interaction/transition sufficient to walk the representative chain and enter/use the Bar public interior.

This WP does not implement Living World semantics.

### `WP-H2-03 — Character presentation + population proxy`

Create the human-scale population layer needed to judge the final environment and character direction.

Required benchmark minimum:

- six visible character presentations in the retained slice;
- clothing appropriate to role/context; no raw/unpresentable humanoids;
- approved scale and silhouette variation;
- idle + walk locomotion presentation;
- bounded point-to-point/nav movement sufficient for occupancy testing;
- one minimal interaction/dialogue/bark path;
- at least two bounded occupancy conditions (for example quiet vs busier) to judge density/camera/traversal.

These are presentation/proxy actors, not H3 persistent people.

### `WP-ART-02 — First-playable visual closure`

ART retains authority over the final representative visual language. ART-02 consumes the keeper environment and H2 character/playable shell and closes visible gaps in architecture/materials/nature, lighting/atmosphere, props, wardrobe, animation presentation and overall composition.

ART-02 uses `PRODUCT_CHECKPOINT`; owner visual approval is part of its acceptance oracle.

### `WP-H2-GATE — Keeper visual + AI world-authoring benchmark`

Compose H2-01/02/03 + ART-02 on the retained CITY-07 benchmark.

Required fresh-author trial:

```text
inspect accepted world + visual/assembly constraints
-> discover legal semantic/composition options
-> make one bounded local improvement
-> plan/validate/apply through accepted Arkus authority
-> materialize in Unity
-> inspect/play from third-person scale with representative occupants
-> correct one observed local issue if needed
-> reconcile/rematerialize
-> prove unaffected accepted content and keeper constraints remain intact
```

The gate also absorbs the useful claim of superseded `WP-CITY-08`: discoverability, constraint-preserving reuse and observable authoring-effort reduction are proven once here, not in a duplicate CITY gate.

## 8. PASS-before-work requirement

Every H2 implementation WP must carry a prewritten `PASS only if all are true` section before its Worker starts. The Reviewer does not invent a stronger product claim after implementation.

H2 uses `Docs/workpacks/PRODUCT_EXECUTION_POLICY.md`.

## 9. H3 handoff

H3 begins only after H2 visual/product closure and turns presentation actors into persistent people.

Expected H3 ownership starts with:

- persistent actor identity;
- clock/time model;
- POIs/smart objects/activity anchors;
- schedules/routine resolver;
- readable daily movement through the H2 spatial semantics;
- animation/runtime integration required for those routines.

A useful H3 benchmark remains roughly six persistent NPCs living a readable day for understandable reasons. H4 then owns the deeper Living World: knowledge/beliefs, relationships, events, memory, structured outcomes and runtime/save continuity.

## 10. External product portability disposition

The accepted H2 future-planning signal asked whether H2 must prove Arkus versioned/packaged for a fresh external repository.

Decision for this plan: **not an H2-GATE blocker**.

H2 consumes accepted DW/CTX public/context evidence but prioritizes proving Juego2 world authoring and visual/product closure. Fresh external-repository packaging/versioning may become a parallel/post-H2 productization track (`H2X` or successor) if still commercially useful. Deferring it does not permit hidden/private authoring surfaces inside H2; the H2 fresh-author trial still uses accepted public Arkus surfaces.

## 11. What H2 success looks like

A short capture of the retained slice should no longer need the explanation “ignore the cubes/assets/proxies; imagine the final game.” The environment, characters, proportions, lighting and density should already communicate the intended game. What is missing after H2 should primarily be **behavioural depth and content breadth**, not the answer to what Juego2 looks like.
