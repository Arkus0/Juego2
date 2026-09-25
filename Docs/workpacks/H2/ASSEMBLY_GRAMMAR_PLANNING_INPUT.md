# H2 planning input — layered environment assembly for AI-native world authoring

Status: **NON-BINDING H2 PLANNING INPUT**  
Date recorded: 2026-09-25  
Consumes: `Docs/workpacks/H2/WORLD_AUTHORING_PLANNING_INPUT.md`, `WP-ART-01`, `ART_ENVIRONMENT_ASSEMBLY_GRAMMAR.md`, accepted CITY keeper constraints

## Purpose

Preserve a product requirement discovered during the first Astra environment experiment: H2 must not equate **asset placement** with **world construction**.

A system can know the semantic intent (`bar`, `street`, `house`), know the available Quaternius/Juego2 assets, and still materialize a decorated greybox. Therefore future H2 planning must explicitly decide how Arkus represents and edits the layered physical assembly of retained environment elements.

This note does not freeze the final H2 workpack split or claim the current H1 bridge already owns these semantics.

## Required planning question

The future H2 planner must answer:

> What minimum typed assembly model lets an AI author turn a semantic place/building/street intent into coherent retained Unity geometry without collapsing the problem into anonymous boxes plus decorative assets?

A truthful answer may reuse ART-01 manifests and CITY composition rules. It must not silently assume that catalogue IDs alone solve the problem.

## Candidate assembly vocabulary

The planner should evaluate at least these layers for building/world authoring:

1. `site_datum` — terrain/support surface, local elevation/slope, frontage orientation;
2. `footprint` — retained building/site footprint and access level;
3. `base` — plinth/foundation/retaining/ground-contact treatment;
4. `massing` — primary/secondary volumes and storey/eaves relationships;
5. `facade_surface` — typed wall/exterior host surface with corner/end conditions;
6. `opening` — hosted cut/recess for a door/window or accepted integrated facade module;
7. `insert` — door/window/shutter assembly filling an opening;
8. `roof_surface` — roof/eave/ridge/termination and supported top meeting;
9. `threshold_transition` — street/ground -> entrance/interior or step/landing relation;
10. `street_surface` — traversable lane/square/road surface and its support;
11. `edge_transition` — kerb/shoulder/drainage/retaining/building contact;
12. `attachment` — sign/lamp/porch/prop hosted by a valid surface/support;
13. `nature_weathering` — vegetation/wear attached to plausible ground/contact zones.

The planner may compress or rename these concepts if a simpler model proves sufficient. It must preserve the ability to falsify dressed-greybox output.

## Candidate connection vocabulary

Future H2 authoring should evaluate whether relations such as these need first-class representation or a reviewed derived projection:

- `SUPPORTED_BY`;
- `HOSTS` / `CUTS`;
- `FILLS`;
- `MEETS`;
- `CAPS`;
- `TRANSITIONS_TO`;
- `ATTACHED_TO`;
- `ALIGNS_TO`;
- `CLEAR_OF`;
- `OCCLUDES` for bounded reveal/composition edits.

The important product guarantee is not the exact names. It is that the authoring system can distinguish a coherent connection from two meshes merely intersecting in Unity.

## Authoring-loop implication

The existing H2 hypothesis:

`high-level spatial intent -> typed world structure -> approved-asset composition -> Unity materialization -> spatial observation -> localized semantic edit -> revalidation/parity`

should be interpreted more strongly as:

```text
high-level spatial intent
  -> CITY/semantic layout
  -> layered assembly plan
  -> approved asset + connection resolution
  -> Unity materialization
  -> structural + third-person observation
  -> localized layer-aware edit
  -> affected-connection revalidation
  -> parity / truthful result state
```

The assembly plan can be compact and generated. It does not need to resemble professional architectural drawings.

## Local semantic editing implication

H2 should test that useful edits target the appropriate assembly layer instead of regenerating unrelated content.

Examples:

- `recess this entrance` -> facade/opening/threshold/massing layers;
- `make the roof less chalet-like` -> roof/massing/art-family choice, not street topology;
- `narrow this lane` -> street surface/edge assembly plus affected clearance/threshold checks;
- `add a partial reveal to the civic landmark` -> lawful occluding mass/edge adjustments plus sightline revalidation;
- `add moss near this retaining wall` -> nature/weathering layer only;
- `replace this Alpine facade` -> facade/roof/art composition without changing the canonical place identity.

If the author can only regenerate the whole scene to make these changes, H2 has not yet proved the intended localized world-authoring capability.

## Failure states

Future H2 planning must preserve the ART-01 distinction:

- `KEEPER_READY` — success state for a required retained world-authoring benchmark;
- `PROXY_VISUAL` — truthful temporary/non-keeper state;
- `COVERAGE_BLOCKED` — truthful inability to satisfy required retained coverage.

A world-authoring agent must be allowed to say `COVERAGE_BLOCKED` when the kit lacks a required opening/corner/roof/threshold/ground-contact solution, and may expose `PROXY_VISUAL` during iteration. Those outcomes are valuable honest evidence and are preferable to fabricated success.

They are **not alternative PASS states** for a required H2 gate benchmark. A required benchmark that ends `PROXY_VISUAL` or `COVERAGE_BLOCKED` remains blocked/failed until it reaches `KEEPER_READY` or the reviewed requirement itself changes.

The agent must never convert either non-pass state into apparent success by constructing a cuboid and decorating it.

## Representative H2 proof

The future reviewed H2 plan should include at least one bounded test where a fresh AI author:

1. receives a semantic brief for a real Juego2 place;
2. consumes the approved kit and assembly metadata;
3. produces a building assembly plan plus adjacent street/ground assembly;
4. materializes them in Unity;
5. is inspected in neutral/massing form and normal third-person presentation;
6. receives one localized structural instruction;
7. edits the relevant layers without silently regenerating unaffected accepted content;
8. revalidates affected connections/clearances/sightlines;
9. reaches `KEEPER_READY` for the required benchmark.

If step 9 truthfully returns `PROXY_VISUAL` or `COVERAGE_BLOCKED`, the system has demonstrated honest failure handling but has **not** passed the required world-authoring proof.

The existing CITY-07 representative chain is a natural candidate fixture once its prerequisite state is authoritative.

If PR `#228` P1/P8/P9 proposals are later accepted, they provide useful additional stress cases: landmark visibility, narrow lane/junction assembly, and a denser set of player-facing thresholds. This note does not adopt them.

## H2 gate implication

H2-GATE should not pass solely because an AI can place prefabs, assign materials and produce a visually plausible screenshot.

For any **required** H2 world-authoring benchmark, gate success requires `KEEPER_READY`. `PROXY_VISUAL` and `COVERAGE_BLOCKED` remain legitimate truthful outputs for diagnostics, iteration and negative evidence, but they block that benchmark from satisfying H2-GATE.

The future planner should therefore use a gate condition at least as strong as:

> A fresh author can create and locally revise a bounded third-person environment whose buildings, streets and thresholds are assembled through explicit support/host/meeting relations from approved assets; no required retained element is an anonymous primitive decorated to hide missing construction semantics; every required benchmark element reaches `KEEPER_READY`; and any unresolved coverage is surfaced explicitly **and blocks the gate rather than being counted as success**.

## Boundary

- ART owns visual fit, asset triage/derivatives and first-slice assembly grammar.
- CITY owns spatial constitution, routes, sites, access, keeper geometry and physical/game-space validation.
- H1 owns Unity bridge/catalogue/materialization/reconciliation capability.
- H2 should own the generalized AI-native typed world/assembly authoring loop if the reviewed plan confirms it is technically justified.

Do not reopen H1 simply because H2 needs richer authoring semantics. Do reopen a causal H1 owner if effective evidence shows an accepted bridge guarantee is actually false.