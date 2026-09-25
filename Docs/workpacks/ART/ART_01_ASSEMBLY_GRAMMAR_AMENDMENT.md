# ART-01 layered assembly-grammar amendment

Status: **PROPOSED / NOT ACCEPTED**  
Class: DOCS_ONLY / PRODUCT-SCOPE CLARIFICATION  
Scope: `WP-ART-01` only  
Causal owner amended: `WP-ART-01` production-kit readiness claim  
Companion contracts:

- `Docs/workpacks/ART/ART_ENVIRONMENT_ASSEMBLY_GRAMMAR.md`
- `Docs/workpacks/ART/ART_01_DIMENSIONAL_PROFILE.md`

## Purpose

Clarify that `WP-ART-01` does not PASS merely by cataloguing/adapting enough individual environment assets.

The disposable Astra scene exposed two independent causes of dressed-greybox output:

1. insufficient keeper-capable asset vocabulary; and
2. insufficient construction semantics describing how pieces are hosted, supported, cut, capped, transitioned and connected.

The follow-up design review exposed two more prerequisites for a reusable construction vocabulary:

3. a shared metric profile so independently authored pieces actually fit; and
4. a cheap visual target before asset adaptation begins, so the kit is not produced against a vague aesthetic brief.

ART-01 owns all four for the first retained chain.

## Binding clarification

Where `WP-ART-01` says the kit must include composition metadata and avoid visible primitive fallback, interpret that requirement through `ART_ENVIRONMENT_ASSEMBLY_GRAMMAR.md` and `ART_01_DIMENSIONAL_PROFILE.md`.

For the representative benchmark, ART-01 must provide enough asset coverage **and enough connection/assembly/metric coverage** to realize at least:

- one coherent building exterior/threshold assembly;
- one coherent street/ground/edge assembly;
- their building-to-street connection;
- bounded props/nature added only after the host surfaces and transitions are resolved.

A catalogue with many individual parts is not sufficient if the author has no truthful way to resolve corners, openings, roof termination, thresholds, ground contact or street edges. Likewise, a valid relation graph is not sufficient if each module uses an incompatible scale convention.

## Required evidence addition

Add to ART-01 deliverables:

- one compact representative **building assembly plan**;
- one compact representative **street assembly plan**;
- connection/host metadata sufficient to distinguish intentional assembly from mesh intersection;
- dimensional metadata for every structural/hosted benchmark piece, consuming `ART_01_DIMENSIONAL_PROFILE.md` or an explicitly recorded reviewed exception;
- at least one neutral-material or equivalent massing inspection where texture/material polish cannot hide unresolved structure;
- an agent/fresh-author attempt that consumes those plans/metadata rather than only a natural-language aesthetic prompt.

The building plan must cover applicable layers:

`site/datum -> footprint/base -> massing/storeys -> facade hosts -> openings/inserts -> roof/top closure -> thresholds -> ground/street contact -> dressing/nature`.

The street plan must cover applicable layers:

`terrain/support -> traversable surface -> edge/kerb/shoulder/drainage -> retaining/building contacts -> thresholds -> collision ownership -> dressing/nature`.

## Pre-production visual target

Before spending production effort adapting or creating the retained kit, ART-01 must freeze a **cheap visual-target sheet** over the accepted CITY-04 seed.

Use **2–3 annotated paintovers / overpaints** from human-scale views of the representative chain. The recommended views are:

1. Puente Viejo / S02 approach toward the retained district;
2. W12 climb / Casco compression-reveal view;
3. Bar F01 exterior + threshold at ordinary third-person scale.

The paintovers are not geometry authority and do not adopt PR `#228` P1/P8/P9. They are a bounded art-direction target showing the intended:

- massing/silhouette rhythm;
- wall/roof/material family read;
- facade depth and opening character;
- building-to-ground/street treatment;
- vegetation density and wet-valley mood;
- prop/dressing density;
- visual hierarchy and human-scale read.

Each view must explicitly call out what is **structural intent** versus **illustrative dressing** so a later Worker does not treat a painted detail as an unreviewed CITY semantic commitment.

A material visual departure from the target during ART-01 is allowed, but the reason must be recorded: source/kit limitation, better measured composition, CITY constraint, performance/technical need, or owner/reviewer decision. The target is a cheap convergence tool, not a pixel-match oracle.

## Fast blockout / assembly iteration protocol

ART-01 and its local visual benchmark must support **rapid bounded iteration**. Do not require an independent Worker/Reviewer lifecycle for every small level-art adjustment.

Inside the already accepted CITY envelope and the ART-01 sandbox, the active Worker may iterate freely on non-canonical local realization details such as:

- module choice and substitution;
- opening spacing within the selected building family;
- roof/eave composition;
- threshold/base treatment;
- prop/vegetation density;
- material variants;
- local massing or setback choices already authorized by the consuming CITY contract.

The Worker retains only compact evidence at meaningful checkpoints rather than every click. Minimum retained checkpoints:

1. **target checkpoint** — elevation inputs available, dimensional profile selected, paintover target frozen;
2. **massing checkpoint** — neutral-material building/street assembly readable without dressing;
3. **candidate checkpoint** — normal materials/dressing, third-person inspection, fresh-author/Astra smoke test.

A change that would alter CITY topology, a programmed site, access role, hard boundary, route/elevation class or other predecessor semantic is **not** a local iteration. It is routed immediately to the causal owner and must not be smuggled into the sandbox.

Independent review is performed on the frozen ART-01 candidate and its retained checkpoints, not on every exploratory micro-iteration. This keeps environment design fast without weakening the acceptance boundary.

## Acceptance addition

ART-01 cannot PASS if any required retained relation is only achieved by accidental overlap/intersection while the scene is reported `KEEPER_READY`.

It also cannot PASS if the benchmark depends on undocumented per-piece scale guesses or if the art kit was produced without the pre-production visual target above.

Examples of FAIL:

- a window is visually attached to a box but no opening/recess/integrated facade module exists;
- a roof floats or clips into the shell with no coherent eave/top meeting;
- a building has no deliberate ground-contact/threshold condition;
- a street is a stack of overlapping traversable planes/cubes rather than one coherent surface system;
- a prop or vegetation layer hides a missing junction;
- the agent uses a primitive shell as a universal fallback because the kit does not encode the required assembly role;
- door/storey/roof modules only fit through ad-hoc non-uniform scaling with no recorded retained rule;
- the final benchmark drifts materially from the paintover target with no recorded reason.

## H1 / CITY / H2 boundary

This amendment does not widen H1 catalogue semantics automatically. ART-01 may keep assembly and dimensional metadata in an ART-owned reviewed manifest when H1 deliberately does not own those fields.

CITY-07 consumes the grammar and metric profile to build the keeper; it still owns lawful spatial composition, the district elevation profile, and preservation of accepted CITY physical/semantic conclusions.

Future reviewed H2 planning decides which assembly relations and metric/elevation concepts become first-class Arkus AI-world-authoring concepts. It may not claim successful world authoring if it can only place assets around anonymous boxes.

## Result

ART-01 proves a **construction vocabulary**, not merely an asset vocabulary. The first retained slice therefore has a reviewed path from a cheap visual target and shared dimensions through terrain/site intent to coherent building/street assemblies before CITY-07 is allowed to treat the result as keeper-ready.