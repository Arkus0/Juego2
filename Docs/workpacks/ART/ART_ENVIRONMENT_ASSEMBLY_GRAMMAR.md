# ART environment assembly grammar — layered construction contract

Status: **PROPOSED / ART-01 COMPANION**  
Class: DOCS_ONLY / PRODUCT-COMPOSITION CONTRACT  
Owner: `WP-ART-01` for the first retained environment kit  
Consumers: `WP-CITY-07` keeper realization; future reviewed H2 world-authoring plan  
Origin: owner review of the disposable Astra environment test on 2026-09-25

## Purpose

Close the gap between **having assets** and **building architecture/environment coherently from those assets**.

The observed failure mode is not explained by catalogue breadth alone. A system can know that walls, windows, roofs and props exist and still build a primitive box, attach those pieces to its faces and report apparent success. That produces a dressed greybox rather than a keeper environment.

This grammar therefore defines a bounded construction model for the first retained Juego2 environment. It is deliberately smaller than BIM or a general architectural CAD system. Its purpose is to make the physical assembly logic explicit enough that a human or AI author cannot honestly skip from `building intent` to `decorated cuboid` without declaring proxy mode.

## Core rule

A retained environment element is authored as an **assembly of hosted layers and explicit connections**, not as an anonymous primitive plus decoration.

For buildings, the expected conceptual order is:

```text
site / terrain datum
  -> footprint + ground contact
  -> foundation / plinth / retaining response
  -> storey + massing volumes
  -> exterior wall/facade surfaces
  -> openings cut into hosts
  -> doors/windows/shutters inserted into openings
  -> roof surfaces + eaves/ridge/termination
  -> thresholds / stairs / landings / drainage
  -> structural/support attachments
  -> dressing / signage / props
  -> vegetation / weathering at valid contacts
```

For streets and public ground:

```text
terrain datum
  -> formation / traversable platform
  -> road / lane / square surface
  -> kerb / shoulder / drainage / retaining edge
  -> pavement / threshold transitions where present
  -> building and wall contacts
  -> street furniture
  -> vegetation / wear / clutter at valid edges
```

The layers may collapse when the accepted stylized asset already embodies several of them in one prefab. The semantic relationship must still be understandable: a prefab that contains wall + opening + frame is a valid composed unit; a window mesh simply intersecting a box is not equivalent.

## 1. Site and terrain datum

Every keeper composition begins from a declared local datum:

- terrain/ground surface used for placement;
- local elevation and slope;
- public/private/service side where relevant;
- parcel/frontage orientation;
- retaining requirement if the building or street cuts a slope;
- no-build/water/route constraints inherited from CITY.

A building may not be considered grounded merely because its transform Y approximately matches the terrain. Ground contact must resolve the visible base condition.

## 2. Footprint, foundation and plinth

The building plan declares:

- footprint polygon or bounded retained envelope;
- principal access level;
- base/plinth/foundation treatment;
- relation to slope and retaining wall if required;
- any deliberate step, porch slab or threshold landing.

This layer prevents buildings from visually emerging from one undifferentiated ground plane.

For Juego2 this is a stylized visual/assembly contract, not a structural-engineering simulation. No real load calculation is implied.

## 3. Storeys and massing volumes

Before facade dressing, the composition declares:

- principal volume(s);
- storey/eaves heights;
- secondary projections/recesses where used;
- frontage line and setbacks;
- corner condition;
- party-wall or exposed-side condition where relevant.

A single cuboid may be valid for a genuinely simple building, but only if its silhouette, openings, roof and ground-contact treatment satisfy the same reviewed keeper criteria. `simple` is not permission for `undetailed greybox presented as final`.

## 4. Facade and wall surfaces

Exterior faces become typed host surfaces, not arbitrary primitive faces.

Each relevant surface records:

- exterior orientation;
- material/family intent;
- base/eave limits;
- corner/end meeting condition;
- whether openings are allowed;
- whether the face is public frontage, service side, party wall or scenic/back face.

A visible keeper facade must resolve end/corner conditions and may not depend on coplanar decorative planes to hide an unfinished host.

## 5. Openings and inserted elements

Doors and windows are hosted by explicit openings or by an accepted facade module that already models that relationship.

The retained representation must make clear:

- host wall/facade;
- opening bounds;
- wall thickness or accepted stylized recess depth;
- inserted door/window/shutter frame;
- sill/lintel/jamb or accepted equivalent where visually required;
- threshold relation to ground/interior.

Negative rule: `window asset intersects or floats on a flat wall/box face` is not sufficient evidence of a keeper opening.

## 6. Roof plan and roof-to-wall connection

Roofing is not a decorative cap.

The assembly plan declares:

- roof type/family;
- supported wall/volume perimeter;
- ridge/hip/valley/termination as applicable;
- eave/overhang condition;
- roof-to-wall meeting;
- drainage/gutter/downpipe only where selected by the visual kit.

A single integrated roof prefab is valid if it truthfully covers those roles. A roof mesh floating above or clipping through a primitive body without a reviewed meeting condition fails.

## 7. Thresholds and vertical transitions

Every player-facing entrance or traversable level change must resolve the connection between surfaces:

- street/ground -> threshold;
- threshold -> interior floor;
- stair/step -> landing;
- retaining edge -> adjacent traversable surface;
- ramp/slope -> route surface.

This grammar does not redefine CITY access roles. It makes the physical realization of accepted access readable and coherent.

## 8. Street / ground / edge assembly

Keeper streets and squares are not stacks of independent coplanar slabs.

For each retained segment, the composition records the applicable assembly from:

- terrain/support surface;
- principal traversable surface;
- road/lane/square material family;
- shoulder/kerb/pavement if present;
- drainage/readable edge;
- retaining or garden wall where present;
- building threshold connections;
- vegetation/prop edge zones.

Overlapping surfaces are permitted only when intentional and non-contradictory. Duplicate traversable planes, hidden z-fighting layers or duplicate collision are not an acceptable retained assembly.

## 9. Attachment and dressing layer

Props may enrich architecture only after their host relation is valid.

Examples:

- sign `ATTACHED_TO` facade/beam with explicit facing;
- lamp `ATTACHED_TO` wall/post;
- bench `SUPPORTED_BY` ground/terrace;
- barrel/crate `SUPPORTED_BY` ground/porch;
- planter `SUPPORTED_BY` sill/ground;
- laundry line `SPANS` two supports;
- downpipe `DRAINS_FROM` roof/eave and `TERMINATES_AT` ground/drain zone.

Dressing cannot be used to conceal an unresolved structural/host connection.

## 10. Vegetation and weathering layer

Vegetation and damp/wear are contextual, not scatter-only.

At minimum distinguish:

- rooted ground vegetation;
- wall-edge / retaining-edge vegetation;
- planter vegetation;
- moss/damp treatment attached to plausible surface/contact zones;
- trees/shrubs with clearance from buildings/routes;
- scenic-only distant vegetation.

Negative rule: tree trunks/rocks/props penetrating unrelated geometry without an intentional authored relation are defects, not stylization.

## Connection vocabulary

The first retained kit should support, directly or through a reviewed companion manifest, a small explicit connection vocabulary:

- `SUPPORTED_BY` — element rests on/depends on a host surface or structure;
- `MEETS` — two retained surfaces terminate coherently at a junction;
- `HOSTS` / `CUTS` — facade hosts an opening; opening cuts the host;
- `FILLS` — door/window assembly fills an opening;
- `CAPS` — roof/eave/termination closes the top of a wall/volume;
- `ALIGNS_TO` — frontage/edge aligns to a reviewed reference;
- `TRANSITIONS_TO` — traversable surface changes to another at a threshold/step/ramp;
- `ATTACHED_TO` — non-structural element attaches to a host;
- `DRAINS_TO` — optional visual drainage relation;
- `CLEAR_OF` — required clearance from route, opening, water shoulder or another object;
- `OCCLUDES` — optional composition relation used when an authored reveal depends on a mass.

This is not a mandate to encode every relation in H0 `WorldState` immediately. ART-01 owns the reviewed environment-composition truth; future H2 planning decides which relations become first-class Arkus world-authoring concepts. H1 catalogue semantics are not silently widened by this document.

## Minimum per-piece composition metadata

Where applicable, each keeper-capable piece records:

- logical asset ID and provenance;
- assembly role(s);
- dimensions / pivot / orientation assumptions;
- allowed hosts;
- allowed/required connection types;
- repetition axis / tiling rule;
- supported material/family variants;
- collision/traversal role;
- whether it is `STRUCTURAL_COMPOSITION`, `HOSTED_INSERT`, `DRESSING`, `NATURE`, or `PROXY_ONLY`;
- known incompatible neighbors/combinations;
- keeper readiness state.

## Building assembly plan

A representative keeper building must have a reviewable plan containing at least:

1. site/datum;
2. footprint + access level;
3. base/plinth/retaining response;
4. primary/secondary massing;
5. facade host surfaces;
6. openings + inserts;
7. roof plan + top termination;
8. public/service/private threshold realization required by accepted CITY semantics;
9. ground/street connection;
10. dressing/nature only after the above are resolved.

The plan can be compact and data-driven. It need not be an architectural drawing set.

## Street assembly plan

A representative keeper street segment must have a reviewable plan containing at least:

1. terrain/support datum;
2. traversable platform/surface;
3. width/grade inherited from CITY constraints;
4. edge/kerb/shoulder/drainage treatment as applicable;
5. retaining/garden/building contacts;
6. threshold transitions;
7. collision/traversable-surface ownership;
8. props/vegetation after the surface system is coherent.

## Validity rules

A `KEEPER_READY` composition fails if any applicable rule is violated:

- an opening has no host surface;
- an inserted door/window has no opening or accepted integrated facade module;
- a roof has no coherent supported perimeter/meeting condition;
- a visible exterior wall terminates without a reviewed corner/end condition where one is required;
- a player-facing threshold does not connect its adjacent surfaces coherently;
- a retained building lacks deliberate ground contact;
- two retained traversable surfaces overlap accidentally or both claim the same collision path;
- a prop is used to conceal a structural seam or missing host relationship;
- a one-sided sign is presented backwards/mirrored from a valid player approach;
- vegetation/props penetrate unrelated retained geometry without deliberate justification;
- the author skipped required layers by creating a primitive shell and decorating it while reporting keeper success.

## Authoring sequence and local edits

Astra or any future authoring agent should plan from coarse support to fine dressing:

1. validate site and constraints;
2. create/choose support and footprint;
3. solve massing and level relationships;
4. solve hosted surfaces and junctions;
5. solve openings and thresholds;
6. solve roof/top closure;
7. solve street/building/terrain contacts;
8. validate collision/traversal/support relations;
9. add dressing and vegetation;
10. inspect at third-person human scale.

Localized edits should target the smallest relevant layer. For example:

- `make the entrance more recessed` edits facade/threshold/massing, not the whole town;
- `narrow this street` edits the street surface/edge assembly and then revalidates affected thresholds/clearances;
- `add a partial reveal toward the tower` edits occluding masses/edge placement inside authorized CITY bounds;
- `darken the roof` is a material change and must not rebuild the footprint.

This sequencing is a planning target for H2 world authoring, not a claim that the current H1 bridge already implements every semantic edit.

## Proxy and coverage states

This grammar consumes ART-01 states:

- `KEEPER_READY` — required layers/connections are resolved for the claimed retained result;
- `PROXY_VISUAL` — deliberately incomplete assembly, visibly/evidentially marked;
- `COVERAGE_BLOCKED` — required piece/connection/role cannot yet be built truthfully.

A missing connection family is a coverage gap. It is not permission to fake the relation with intersecting primitives.

## Benchmark use

The ART-01/CITY-07 representative chain must include at least one building and one street segment whose assembly plans are inspected against this grammar.

A neutral-material diagnostic is encouraged for the building so silhouette, openings, roof meeting and ground contact can be judged without texture polish masking the structure.

## Boundary with PR #228

PR `#228` provides useful CITY concept input: denser route loops (P8), additional places (P9), a civic landmark (P1), human-scale stage shapes and a recommended retained traversal chain. Those proposals remain non-canonical until their causal CITY owners accept them.

This assembly grammar is compatible with either the currently accepted seed or any later accepted subset of P1/P8/P9. It does not itself adopt routes, places or landmark massing.

## Definition of Done for the first slice

The first retained environment vocabulary is not considered ready merely because every requested object has an asset ID. It is ready when a fresh author can produce the representative building/street composition with explicit support, hosted-surface, opening, roof, threshold and ground/street relations — or truthfully return `COVERAGE_BLOCKED` — without disguising missing construction logic as a decorated primitive.