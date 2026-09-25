# H2 future planning input — AI-native world authoring + population proxies

Status: **NON-BINDING H2 PLANNING INPUT**

Date recorded: 2026-09-25

## Purpose

Preserve the intended product direction discovered before formal H2 planning: H2 should explicitly evaluate whether Arkus should become the AI-native **world-authoring layer above Unity**, rather than merely implementing a conventional vertical slice or a prefab-placement demo.

This note does **not** freeze H2 workpacks, exact gate ownership, final scope, NPC counts, art scope or implementation mechanics. Those remain owned by the future reviewed H2 planning work.

## Planning hypothesis

H2 should evaluate and, if technically justified by the accepted H0/H1/CITY state, prove a bounded authoring loop of the form:

`high-level spatial intent -> typed world structure -> approved-asset composition -> Unity materialization -> spatial observation -> localized semantic edit -> revalidation/parity`

The desired product boundary is not "Arkus replaces Unity". Unity remains the runtime/rendering/physics/editor substrate. Arkus should own the machine-readable world intent, semantic authoring operations, observable state, controlled evolution and verification of the authored world.

A successful H2 should therefore be materially stronger than "an agent can place prefabs in Unity". It should test whether an AI agent can create and evolve a small real game space through stable Arkus concepts while preserving H0/H1 truthfulness and bounded CITY constraints.

## Capabilities the future H2 planner must evaluate

The reviewed H2 plan should explicitly disposition at least these capabilities:

1. **Semantic world vocabulary** — concepts such as plaza, street, building/shell, facade, access, landmark, edge, transition, interior/portal, route and activity node are represented as authoring concepts rather than only anonymous transforms.
2. **Intent-to-layout authoring** — an agent can turn a bounded spatial brief into a typed layout suitable for a third-person game.
3. **Asset-aware composition** — the layout can be realized using approved Quaternius/Juego2 assets while retaining source/provenance identity and the H1 asset/materialization guarantees.
4. **Unity materialization** — the result is a real Unity scene/slice, not only an abstract plan or disconnected JSON artifact.
5. **Spatial observation** — Arkus can inspect enough of the realized result to reason about scale, traversal, landmarks, sightlines, access and other H2-owned spatial claims.
6. **Localized semantic editing** — instructions such as "narrow this street", "open a partial reveal toward the landmark" or "turn this corner into a small commercial node" can alter the intended local area without silently regenerating or corrupting unrelated accepted content.
7. **Playable evaluation** — the realized zone is assessed from human/third-person scale, not only from top-down/editor geometry.

The planner must decide which of these belong to individual H2 workpacks versus the final H2 gate and must reject any capability that cannot be supported truthfully by the accepted predecessor architecture.

## Population-proxy requirement

H2 must not validate world authoring only on an empty scene.

The future H2 plan should include a **bounded population-proxy layer** whose purpose is to validate the authored environment at human scale and representative occupancy before persistent NPC/living-world semantics exist.

The proxy population should be sufficient to answer questions such as:

- does a plaza or street feel absurdly large once human-sized characters occupy it;
- do doors, facades, pavements, stairs and street widths read correctly beside people;
- how many visible bodies are required for a representative space to feel alive rather than empty;
- do small groups block circulation or create camera/navigation problems;
- are observation/following distances plausible in third person;
- does a nominally dense layout still feel sparse because its population capacity is wrong;
- can different spatial roles support meaningfully different occupancy densities.

The future planner should choose exact counts from measured evidence rather than freeze an arbitrary global NPC target now. It should, however, require at least one non-empty representative density and preferably compare multiple bounded densities so that H2-GATE cannot pass solely because an empty environment looks correct.

Population proxies may use a player-scale avatar plus simple NPC stand/walk/patrol behaviors and approved visual variants where available. They are validation instruments, not H3 NPCs.

## Boundary with later NPC / living-world work

H2 population proxies must **not** silently pull the persistent NPC or living-world problem into H2.

Unless the future reviewed H2 plan explicitly proves a stronger need, H2 proxies do not own:

- persistent personal identity;
- social relationships;
- memory or beliefs;
- jobs and complete daily schedules;
- narrative state;
- long-lived needs or goals;
- systemic reactions/consequences;
- full living-world causality.

Those later systems should consume the spatial semantics established by H2 rather than hard-code themselves to raw coordinates. A future persistent NPC should be able to reason in terms such as `HOME -> STREET -> SHOP -> BAR`, public/service access, route roles, waiting points and activity nodes where the accepted architecture supports them.

A useful ownership shorthand for future planning is:

- **H2:** can Arkus build and evolve the place, and prove that the place works when occupied by representative human-scale proxies?
- **Later NPC/living-world milestone:** can those proxies become persistent people with routines, relationships, memory and consequences?

## Candidate H2 gate question

The future reviewed plan should consider a gate claim at least as strong as:

> Can a fresh AI author, through the accepted Arkus public surfaces, take a bounded Juego2 spatial brief, build a keeper-capable third-person zone in Unity from approved assets, populate it with representative human-scale proxies, inspect its scale/density/traversal, make a localized semantic improvement, rematerialize/reconcile it, and prove that the resulting world remains truthful, reproducible and coherent without silently damaging unaffected accepted content?

A representative brief may be a compact fictional Cantabrian/Liébana zone such as a plaza + active street + secondary route + bar/local + transition toward another district or natural edge. The final planner owns the exact fixture and must consume the accepted CITY/visual/asset constraints that exist at planning time rather than invent a parallel spatial authority.

## Product direction this preserves

If accepted by future H2 planning, the milestone sequence would become conceptually:

- H0: canonical world state / authoring and verification kernel;
- H1: Unity projection, materialization, observation and reconciliation boundary;
- H2: AI-native world authoring over that boundary;
- later milestone(s): persistent NPC/routine authoring and living-world causality.

This is a product-direction signal, not a renaming mandate. The future H2 planner may choose different workpack/milestone names if the causal ownership is clearer.

## Explicit non-claims

This note does **not** assert that:

- H2 is already formally planned;
- H2 may start before its accepted H1 prerequisites are satisfied;
- Arkus is a replacement for Unity;
- H2 must generate an entire town or arbitrary open world;
- the product is a one-click procedural city generator;
- Quaternius is final art;
- exact NPC counts or town-wide population targets are already known;
- H2 owns persistent NPC simulation or the final living-world system;
- CITY constraints may be bypassed in the name of AI creativity;
- current H1, CITY, PA, CTX or DW scope changes because this planning input exists.

The concrete H2 contract still requires a dedicated reviewed planning WP after predecessor state is known.
