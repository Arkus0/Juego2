# WP-CITY-09 — Play-design layer for the retained seed

Status: **PROPOSED — not yet planned for execution; requires owner approval and independent review**  
Class: PRODUCT / GAME-SPACE DESIGN (NON-FOUNDATIONAL)  
Mode: **REMOTE**  
Depends on: `WP-CITY-03` PASS  
Blocks: `WP-CITY-04` (proposed: CITY-04 should consume this layer before its greybox starts)  
Input: `Docs/production/CITY_PLAY_DESIGN.md` (PROPOSED), `Docs/art/VISUAL_BIBLE.md` v0.2

## Why a new ID

`CITY-00` fixed the meanings of `CITY-01..04`, and `CITY-05..08` are taken. Play design is a distinct responsibility that no accepted CITY WP owns, so it takes the next free stable ID instead of repurposing one.

## Objective

Give the Keeper City an explicit owner for **player experience of space**: loops, earned shortcuts, designed detours, orientation/landmarks, pacing, comfort, staged spaces, event footprints and composed views. The town becomes fun and memorable to move through while staying truthful to the accepted CITY geography and causal rules.

## Timing

CITY-04 is dormant until `WP-H1-08` PASS. This WP is remote planning and can complete in that window, so the greybox validates a town designed for play rather than discovering the gap after construction.

## Work

1. Adopt play-design rules PD-01..PD-16 (or a reviewed revision) as CITY-owned acceptance vocabulary.
2. Produce explicit, owner-tagged amendment deltas for the seed proposals P1–P7:
   - `CITY-01`: represent the seed portion of the accepted `W08` paseo/sirga alignment; add the escaleras, pasadizo and one-way drop edges with access/state rules and NPC eligibility;
   - `CITY-02`: add Bolera del Puente as a B/S2/I0 place; check the full-city programme covers the PD-16 villa signifiers;
   - `CITY-03`: walkway authorization inside `nb.rio_bank` for the paseo only; new open-site region `S04` on Orilla sur; F02 landmark posture; the five postcard camera positions; quantify ordinary fabric (closed blocks, built-frontage ratio, lot grain, storey gradient) and soft-envelope roofscape on every seam;
   - `CITY-05`: tower-house one-off variant of `bf.civic`; covered-passage assembly;
   - `CITY-06`: tower-top I2 extension note; stage/discovery compatibility check.
3. Verify that no delta changes CITY-00 geography, landmasses, crossings or the seed envelope.
4. Extend the CITY-04 human-run list and acceptance with the §5 play checks (orientation, loops, boring meter, dead ends, chase, first visit, door grammar, postcards, detours).
5. Record the two-clock rule (fiction time vs. player time) and flag time compression to its future H2/PA owner without deciding it.

## Deliverables

- Accepted `Docs/production/CITY_PLAY_DESIGN.md` (status promoted from PROPOSED);
- amendment deltas applied to the affected CITY production docs, each citing this WP;
- updated `WP-CITY-04` required runs/acceptance;
- updated figures `Docs/production/figures/city_seed_play_layer.svg` and `city_seed_fabric.svg`.

## Acceptance

- The seed has ≥3 city-scale public loops, and none is node-local only.
- Every terminus has a named payoff.
- One L1 landmark with declared sightlines from the bridge, landing, ford and commercial seam.
- Every stage type in PD-11 is placed or explicitly deferred with a reason.
- Five postcard positions are named.
- Seed fabric reads as a villa: closed blocks, continuous street walls in the core, declared lot grain and storey gradient, and no visible end of town from inside the seed.
- Every shortcut is an owned graph edge that NPCs can use under the same rules.
- Zero new water crossings, landmass changes or envelope changes.
- CITY-06 §11 restraint and anti-inflation rules are preserved.

## Negative gates

FAIL if a proposal:

- creates a player-only route or teleport;
- invents a crossing or dry connection that CITY-00 forbids;
- turns every street into a stage;
- reads as a scattered hamlet (detached houses, gaps in the core street walls);
- replaces Liébana identity with a theme skin;
- claims NPC, schedule or event runtime;
- or treats hypothesis thresholds as measured facts.

## Forbidden

Unity work, asset import, gameplay implementation, Living World semantics, H1 bridge semantics, final art.
