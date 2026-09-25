# WP-CITY-09 — Retained-seed play-design amendment package

Status: **CANDIDATE — independent review required**  
Class: PRODUCT / GAME-SPACE DESIGN (NON-FOUNDATIONAL)  
Mode: **REMOTE**  
Depends on: `WP-CITY-03` PASS  
Blocks: **nothing directly**. This candidate does not alter accepted CITY owners or the current CITY-04 contract by itself.

Inputs:
- `Docs/production/CITY_SPATIAL_CONSTITUTION.md`
- `Docs/production/CITY_MOBILITY_TOPOLOGY.md`
- `Docs/production/CITY_LOCATION_PROGRAMME.md`
- `Docs/production/CITY_ENVIRONMENT_GRAMMAR.md`
- `Docs/production/CITY_INTERIORS_DISCOVERY.md`
- `Docs/production/CITY_PRODUCT_SEED.md`
- `Docs/workpacks/CITY/WP-CITY-04.md`

Primary output:
- `Docs/production/CITY_PLAY_DESIGN_PROPOSAL.md`

Evidence:
- `Docs/evidence/WP-CITY-09/CONTRACT_AUDIT.md`

## Why this WP exists

The accepted CITY chain makes the retained seed geographically truthful, causally owned and physically falsifiable, but no accepted owner currently owns the player's experience of moving through that space: loop quality, orientation, route learning, staged spaces, composed views, comfort and the distinction between semantic route-cost hypotheses and measured player traversal time.

This WP rescues that design problem without silently rewriting already accepted owners.

## Authority boundary

This WP is an **amendment package**, not an amendment itself.

It may:
- diagnose player-experience gaps in the accepted seed;
- define play-design vocabulary and test hypotheses;
- propose owner-tagged changes to CITY-01/02/03/05/06 and CITY-04;
- state which proposed checks are valid on the current seed and which require a future accepted amendment first.

It may not:
- add an edge to the accepted CITY-01 graph;
- add a place/site to accepted CITY-02/03;
- change the exact CITY-03 hard envelope, water masks, crossing overlays or represented-edge set;
- change CITY-04 required runs or acceptance;
- restyle the game or modify ART authority;
- claim runtime NPC schedules, AI, event systems, gameplay or Living World semantics.

A downstream owner may later adopt all, some or none of this package through an explicit reviewed amendment. Until that happens, current accepted CITY documents remain source of truth.

## Work

1. Diagnose retained-seed player-experience gaps using only accepted CITY facts.
2. Define a bounded play-design vocabulary for:
   - loops and route learning;
   - dead-end/seam payoff;
   - landmark hierarchy and orientation;
   - player traversal measurements distinct from CITY-01 planning weights;
   - comfort/readability;
   - reusable situation/stage types;
   - composed views/postcards;
   - ordinary-fabric density and perceived continuation beyond the seed.
3. Preserve useful candidate ideas from the closed PR #198 as **proposals only**, including:
   - an L1 civic landmark candidate at F02;
   - a represented paseo/sirga segment plus an escaleras connector;
   - a covered-passage shortcut candidate;
   - seam payoffs;
   - a Bolera del Puente candidate on Orilla sur;
   - a Cuesta mirador;
   - a reusable stage map;
   - closed-block / continuous-street-wall villa-density hypotheses.
4. For every proposed topology/site change, name the current accepted owner that would have to change before the proposal becomes truth.
5. Separate **current-seed-valid** CITY-04 observations from **post-amendment-only** checks.
6. Explicitly preserve the current X5 truth: in the exact retained seed, closing X5 disconnects the represented `E.X5` receiving component; CITY-04 must not invent or walk an alternate route that is outside the seed.
7. Preserve CITY-01 route costs as planning weights / target hypotheses. Do not relabel them as NPC schedule time or measured player time.
8. Keep ART direction out of scope. The alt-Liébana / visual-register material from closed PR #198 is not part of this candidate.

## Deliverables

- `CITY_PLAY_DESIGN_PROPOSAL.md`, clearly non-canonical until adopted by causal owners.
- `CONTRACT_AUDIT.md`, proving the proposal does not require CITY-04 to validate impossible topology and does not redefine CITY-01 semantics.
- A complete owner-impact matrix for every proposed structural change.

## Acceptance

PASS only if all are true:

- No accepted CITY production document is modified by this candidate.
- No ART/SETTING/VISUAL_BIBLE file is modified by this candidate.
- Every new edge, site, place, access rule or depth implication is explicitly marked **PROPOSED** and mapped to its accepted causal owner.
- The proposal never requires current CITY-04 to traverse geometry outside the exact accepted CITY-03 seed.
- X5 closure on the current seed is treated as expected loss of the represented Ensanche receiving component, not as a detour test.
- CITY-01 route-cost minutes remain planning weights / target hypotheses, distinct from both runtime schedule semantics and future measured player traversal time.
- All numerical thresholds are hypotheses to falsify, not measured facts.
- The proposal preserves CITY-00 landmass/crossing truth and creates zero current crossings.
- The proposal does not create runtime NPC, schedule, event or gameplay semantics.
- The independent Reviewer can identify exactly what would need a later owner amendment before any proposal becomes canonical.

## Negative gates

FAIL if this candidate:

- silently changes an accepted CITY owner;
- makes CITY-04 validate an alternate path that does not exist inside the exact seed;
- treats `E.X5` as loop-connected after X5 closes under the current seed;
- renames CITY-01 planning weights into fiction-time or NPC-schedule truth;
- smuggles visual-direction changes from PR #198 into CITY authority;
- treats illustrative density, timing or sightline targets as measured evidence;
- creates player-only teleports or unowned water crossings;
- or requires gameplay/runtime systems to make a spatial proposal coherent.

## Definition of Done

An independent Reviewer can PASS or FAIL a self-contained amendment package without having to accept any new CITY topology, ART direction or CITY-04 gate by implication. A later owner may then choose whether to author a separate canonical amendment PR from the reviewed package.
