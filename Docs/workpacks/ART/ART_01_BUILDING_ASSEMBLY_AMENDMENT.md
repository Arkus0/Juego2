# ART-01 layered building-assembly amendment

Status: **PROPOSED / NOT ACCEPTED**  
Class: DOCS_ONLY / PRODUCT-COMPOSITION CLARIFICATION  
Scope: `WP-ART-01` + downstream CITY-07/H2 authoring consumption  
Origin: owner diagnosis from the 2026-09-25 Astra disposable scene

## Purpose

The visible Astra failure is not explained only by insufficient asset count. It also exposes a missing **architectural assembly model**.

A catalogue that says only “wall”, “window”, “door”, “roof”, “prop” still permits a generator to create one primitive cuboid and attach those pieces to its exterior. That may satisfy inventory use while producing exactly the observed failure: a greybox with assets pasted on.

For keeper-capable environment authoring, a building must be represented as a set of **layered plans and connected surfaces**, not merely as a transform containing child prefabs.

## Core model — `BuildingAssembly`

ART-01 must define a bounded, tool-agnostic building-assembly representation sufficient for the retained benchmark. The implementation may use ordinary Unity objects/prefabs behind the scenes, but authoring/review must be able to reason about the following layers explicitly.

### A. Site / ground plan

Defines how the building meets the world:

- footprint on the accepted parcel/site;
- local ground levels and slope relation;
- foundation/plinth/base condition;
- retaining condition where required;
- street/kerb/step/landing transition;
- drainage/edge seam where visually material;
- public/service/private approach sides inherited from CITY.

A building cannot be `KEEPER_READY` if it simply intersects an undifferentiated terrain/ground plane with no reviewed meeting condition where that seam is visible to the player.

### B. Mass / storey plan

Defines the actual building volume rather than treating a single cube as the default shell:

- primary footprint mass;
- number and height of visible storeys;
- secondary volumes/recesses/projections where the building type needs them;
- facade planes and their orientation;
- party/end/corner conditions;
- interior/exterior wall thickness policy appropriate to the stylized target.

The representation may be simple, but the mass must be intentional. “One box because no better primitive was available” is not an architectural decision.

### C. Floor / ceiling plan

For any visible or enterable level required by the benchmark:

- floor elevation;
- slab/floor surface extent;
- ceiling/soffit relation where visible;
- stair/landing relation when present;
- portal alignment between exterior threshold and interior floor.

This prevents doors, porches or interior floors from being placed independently and merely made to look approximately aligned.

### D. Facade / opening plan

Each street-facing or otherwise player-visible facade has a bounded bay/opening description:

- solid wall spans;
- door/window/opening locations and dimensions;
- recess/depth/thickness treatment;
- lintel/sill/frame role where required;
- shutter/balcony/sign attachment zones;
- public/service/private threshold role inherited from CITY;
- forbidden overlap with corners, floors, roof/eaves and adjacent openings.

Windows and doors are therefore **openings in a surface assembly**, not decals or prefabs that may be attached to an arbitrary plane with no corresponding wall logic.

### E. Roof / cover plan

Defines the cover independently from the wall box:

- roof footprint/coverage;
- roof type and slope family;
- ridge/hip/valley/end condition where used;
- eave projection and underside/soffit expectation;
- roof-to-wall support/contact;
- treatment of secondary masses/porches;
- gutter/downpipe role when used by the visual target.

A roof asset placed above a cube without a valid roof-to-wall meeting condition is not keeper-ready.

### F. Surface/material plan

Defines material treatment on **named surfaces/regions**, not on the whole object indiscriminately:

- wall base/plinth versus upper wall;
- stone/render/timber regions;
- roof material region;
- opening/frame/shutter regions;
- ground/threshold/step regions;
- material tiling/scale bounds sufficient to avoid obvious giant or mismatched tiling.

This layer cannot be used to conceal bad geometry. Material correctness never converts an invalid assembly into a valid one.

### G. Dressing / attachment plan

Props are attached only after the structural assembly exists:

- sign mount surfaces and facing direction;
- lamps, drainpipes, shutters, awnings or planters;
- furniture/boxes/barrels/firewood around thresholds;
- vegetation contact zones;
- local clearance and non-blocking expectations.

A sign must know its front/back orientation; a prop cannot be used to hide an unresolved facade/ground/roof seam.

## Connection graph

The assembly must expose reviewable **connections/joins** between layers. At minimum the benchmark needs enough information to validate:

- `ground ↔ foundation/base`;
- `foundation/base ↔ wall/mass`;
- `wall ↔ wall` at corners/ends;
- `wall ↔ floor` for visible/enterable levels;
- `opening ↔ wall`;
- `threshold ↔ exterior approach`;
- `threshold ↔ interior floor`;
- `wall/mass ↔ roof/eave`;
- `secondary volume ↔ primary mass`;
- `prop ↔ host surface`.

A connection can be implemented by a prefab, socket, dimensions/constraints, generated mesh or other reviewed mechanism. The contract is semantic/visual, not a requirement for one technical representation.

## Surface ownership / attachment rule

Keeper-visible child assets may not float in world space merely because their transforms visually line up in one view.

Where an element is semantically attached to a building or street surface, its authoring record must identify the host surface/assembly role or an equivalent connection witness. Examples:

- window belongs to facade bay `north.02`;
- sign mounts to facade surface `south.public_front`;
- roof covers masses `main + porch`;
- step bridges `street_level -> public_threshold`;
- retaining wall resolves `site_grade -> building_base`.

This is the key difference between **assembled architecture** and **assets pasted onto cubes**.

## Bounded plan-first authoring test

For the ART-01 benchmark, the author/agent must create or consume the building in this order conceptually:

1. site/ground plan;
2. mass/storey plan;
3. floor/threshold plan;
4. facade/opening plan;
5. roof/cover plan;
6. surface/material plan;
7. dressing/attachment plan.

The tool does not need seven separate UI documents. It does need evidence that these concerns are represented distinctly enough that one can change, for example, the roof or facade openings without rebuilding the whole building as an opaque prefab or losing the host/connection relationships.

## Astra / agent requirement

Astra or any later AI author must be able to answer from the authored state, for the benchmark building:

- what is the building footprint and ground-contact condition;
- how many visible storeys/masses exist;
- which facade surface contains the public door;
- which openings belong to each facade;
- what roof covers each mass and how it meets the wall/eave;
- what threshold connects exterior route level to interior floor;
- which props are dressing versus structural composition;
- whether any required join/role is currently `COVERAGE_BLOCKED`.

If the author cannot answer these questions because the scene is only a hierarchy of anonymous transforms, the assembly contract has not been met.

## Acceptance additions for ART-01

ART-01 cannot PASS its representative building benchmark unless:

- the building has a reviewable site/ground, mass, facade/opening and roof plan;
- every visible door/window is bound to a facade/opening role rather than merely placed against a wall plane;
- roof coverage/contact is explicit and visually coherent;
- visible building-to-ground contact is resolved intentionally;
- the public threshold connects the accepted exterior approach to the visible/enterable interior floor coherently;
- props/dressing are attached after and to a valid structural assembly;
- a missing assembly role becomes `COVERAGE_BLOCKED` or `PROXY_VISUAL`, never silent keeper success.

## Negative gates

FAIL if:

- the building has no meaningful representation beneath “one box + children”;
- roof, windows, doors or signs can be moved independently with no host-surface or join invariant and still count as valid;
- material assignment is used as the only distinction between foundation, wall, roof and threshold;
- a public door has no coherent relation among facade opening, exterior landing/step and interior floor;
- roof and facade can visibly separate/interpenetrate while the object remains `KEEPER_READY`;
- ground, wall, roof and dressing are validated only as individual assets rather than as an assembled whole;
- or the benchmark can pass from a single hero screenshot while the connection graph is structurally incoherent from another view.

## Downstream consequence

CITY-07 consumes this assembly contract when realizing keeper buildings. H2 world-authoring planning must treat building assembly as a semantic authoring problem alongside street/plaza/world layout: high-level intent must be able to produce and later edit connected site/mass/facade/roof/threshold structures, not only place prefabs/transforms.

This amendment does not require full BIM, construction engineering, structural simulation or realistic architectural detailing. It defines the **minimum game-architecture decomposition** needed so a low-poly stylized building still reads as a building rather than a decorated blockout.