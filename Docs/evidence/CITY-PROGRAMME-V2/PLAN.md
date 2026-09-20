# CITY Programme v2 — Planning rationale

Status: **PROCESS_ONLY / CANDIDATE**  
Date: 2026-09-20

## Why this revision exists

After `WP-CITY-00` passed, a planning review found a genuine responsibility gap around streets, parcels, buildings, interiors and layered discovery. The first response created a separate proposed `SCENE/` track. Before merge, that split was rejected as unnecessary operational complexity: CITY and SCENE would have alternated dependencies while sharing the same keeper-city product boundary.

The v2 programme keeps the useful responsibility distinctions but collapses them into one linear `CITY/` execution track.

## Decision

Do **not** create an operational SCENE track.

Use one sequence:

`CITY-00 -> CITY-01 -> CITY-02 -> CITY-03 -> CITY-04 -> CITY-05 -> CITY-06 -> CITY-07 -> CITY-08`.

The former draft meanings of CITY-01..04 are superseded by the v2 files in this candidate. `WP-CITY-00` is not amended.

## Causal boundaries

- CITY-01 owns vocabulary/depth, not actual location selection.
- CITY-02 owns routes/access, not interior layout.
- CITY-03 owns which places matter, not how their shells are composed.
- CITY-04 owns reusable exterior/building grammar, not detailed interiors/secrets.
- CITY-05 owns interior/discovery substrate, not NPC belief/dialogue/runtime semantics.
- CITY-06 owns exact keeper-seed selection/specification, not local implementation.
- CITY-07 owns local greybox/traversal falsification, not keeper polish.
- CITY-08 owns keeper realization + bounded Arkus authoring proof, consuming accepted bridge/catalogue authority.

## Lessons from CITY-00

CITY-00 required multiple review cycles because derived connectivity lived across several surfaces and eventually exposed a planar-embedding error. Downstream WPs should therefore prefer one declared owner representation per semantic family and derive secondary diagrams/tables from it instead of allowing independent copies to drift.

## Process boundary

This programme revision is non-foundational and PROCESS_ONLY. It changes no accepted CITY-00 fact, H0/H1 guarantee, Unity implementation, asset dependency or Living World semantic owner.
