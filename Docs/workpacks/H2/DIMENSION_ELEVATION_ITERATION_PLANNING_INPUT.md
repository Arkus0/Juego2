# H2 planning input — shared metrics, elevation frame and rapid world iteration

Status: **NON-BINDING H2 PLANNING INPUT**  
Date recorded: 2026-09-25  
Consumes: `ASSEMBLY_GRAMMAR_PLANNING_INPUT.md`, `ART_01_DIMENSIONAL_PROFILE.md`, `CITY_07_ELEVATION_PROFILE_AMENDMENT.md`

## Purpose

Preserve three product requirements exposed while correcting the first Astra environment test:

1. connected assemblies need a **shared metric profile**, not per-asset visual guessing;
2. a district needs a **shared elevation frame**, not individually coherent buildings floating on unrelated local datums;
3. AI-native world authoring must support **fast bounded iteration** rather than requiring whole-scene regeneration or a full review cycle for every local adjustment.

This note does not freeze H2 implementation or authority. It tells the future reviewed H2 planner what must be explicitly dispositioned.

## Shared metric implication

Future H2 authoring should be able to consume a reviewed dimensional profile for the active environment kit, including at least:

- world-unit convention;
- modular/fine snap where appropriate;
- storey/eave bands;
- door/window/opening dimensions;
- threshold/step/kerb bands;
- roof span/pitch/eave compatibility;
- support/contact planes and pivots;
- human-scale inspection proxy.

The agent may choose reviewed one-off exceptions, but it must not non-uniformly stretch pieces until they look approximately right and then lose the compatibility decision.

## Shared elevation-frame implication

A local object transform must not become an independent source of vertical truth.

For a bounded authored zone H2 should evaluate a typed vertical frame that can represent:

- district/local datum;
- route longitudinal profile and grade breakpoints;
- bridge/landing/plaza/casco or equivalent anchor elevations;
- building threshold and interior-floor relation to adjacent public ground;
- retaining/step/landing transitions;
- visible expansion-seam vertical continuation.

An instruction such as `raise this doorway 10 cm` must therefore be evaluated against its host facade, threshold, adjacent route/landing and interior floor rather than moving one mesh in isolation.

## Fast local iteration implication

The AI world-authoring loop should optimize for **small affected edits**.

Examples:

- tune a roof/eave family without rebuilding the street;
- move an opening within its host facade and revalidate only affected wall/threshold relations;
- smooth a lawful local grade and revalidate affected clearances/thresholds;
- adjust retaining/kerb treatment without regenerating unrelated buildings;
- revise prop/vegetation density without touching structural layers.

Exploratory iterations may happen many times inside a bounded sandbox. Durable evidence/review should be retained at meaningful checkpoints rather than for every micro-edit.

The future H2 plan should distinguish:

- **exploratory local edit** — reversible, inside accepted authority, cheap;
- **candidate checkpoint** — materialized/observed state retained for comparison/evidence;
- **semantic amendment required** — edit would cross CITY/ART/H2 authority and must route to the causal owner.

## Candidate H2 proof addition

In addition to the layered assembly proof, future H2 planning should consider requiring a fresh AI author to:

1. consume a shared metric profile and district elevation frame;
2. build/materialize a bounded third-person environment;
3. perform at least two local edits in different layers (for example threshold/elevation and facade/roof);
4. preserve unaffected content;
5. revalidate only affected dimensions/connections/clearances/sightlines;
6. retain compact before/after checkpoints;
7. produce a truthful final `KEEPER_READY`, `PROXY_VISUAL` or `COVERAGE_BLOCKED` result.

A system that can only regenerate the whole zone, or that loses the shared metric/elevation relationships after a local edit, has not proved the intended world-authoring capability.

## Boundary

- ART owns the first-slice dimensional/visual kit contract.
- CITY owns the keeper district's accepted spatial/elevation truth and physical validation.
- H1 remains the Unity bridge/materialization/reconciliation substrate.
- H2 may generalize these concepts into AI-native world authoring only through its future reviewed plan.

This planning input must not be used to retroactively widen H1 or to bypass CITY amendments.