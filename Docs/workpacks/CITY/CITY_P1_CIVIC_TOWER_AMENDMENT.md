# CITY P1 — civic tower landmark at F02 (amendment proposal)

Status: **PROPOSED / NOT ACCEPTED**  
Class: DOCS_ONLY / PRODUCT-SCOPE AMENDMENT  
Causal owner amended: **CITY-05** (`CITY_ENVIRONMENT_GRAMMAR.md`) — one-off landmark composition on an existing reviewed family  
Secondary owners checked: CITY-03 (no change required), CITY-06 (no change required)  
Origin: `CITY_PLAY_DESIGN_PROPOSAL.md` P1 (WP-CITY-09); concept `Docs/production/concept/CITY_07_GAME_MAP_CONCEPT.md`  
Base: `main` at `2aefda4`

Acceptance requires independent review, merge and DocSync like any other CITY amendment. Until then, CITY-07 must not realize F02 with landmark height.

## 1. Problem

The retained seed has no whole-seed orientation cue (CITY-09 PD-05 L1). The CITY-04 owner feedback reported that the greybox was hard to orient in and did not read as a town. The accepted civic anchor `loc.plaza.ayuntamiento` already occupies F02 on the plaza, and it is the natural L1 candidate, as in the Torre del Infantado read of the Potes reference.

## 2. Amendment

Add to CITY-05 a reviewed **one-off landmark composition** for `loc.plaza.ayuntamiento`:

| Field | Value |
|---|---|
| Composition ID | `oo.civic_tower.f02` (one-off; **not** a new family, per CITY-05 §15.3) |
| Base family | `bf.civic` on `pc.civic_frontage` (unchanged) |
| Site | inside CITY-03 region `F02 = (148,66),(166,66),(166,88),(148,88)`; no change to the region |
| Tower footprint | planning square ~6 × 6 m at the plaza-facing corner, reference `(148.5,66.5),(154.5,66.5),(154.5,72.5),(148.5,72.5)` |
| Tower height | 24–32 m to eaves (≈ 2.5–3 × surrounding 3-storey fabric); exact value tuned in the CITY-07 demo |
| Roof / style | hipped dark-tile roof, stone shaft with quoins, small openings; **no crenellation, keep or castle language** (VISUAL_BIBLE §2 “No”) |
| Access roles | unchanged `{public, service, private}`; the tower shaft adds **no** role |
| Interior | unchanged `I2 / if.civic_office`; upper tower floors are façade only, with **no top access or vantage promise** |
| Discovery | unchanged `disc.ayuntamiento.records_boundary` |

## 3. Why no other owner changes

- **CITY-03:** footprint and massing remain inside the frozen F02 region. No site, seam or boundary moves. If a later realization needs the tower outside F02, that becomes a CITY-03 deviation.
- **CITY-06:** no interior depth, top access or new discovery is added. If a future proposal wants a climbable tower or a vantage interior, it reopens CITY-06 (and CITY-02 if it becomes a programmed place).
- **CITY-01 / CITY-02:** no edge, node or programme change.

## 4. Hypotheses for the CITY-07 asset-rich demo

Record, do not assume, landmark visibility from:

1. the X1 bridge crest;
2. `W.LANDING`;
3. `W.X5` / the ford approach;
4. `W.SHOP` / the W04 commercial seam;
5. if P8 is accepted, the Calleja Alta (`W22`) behind the tower.

Also record whether the tower dominates the plaza or crowds the market apron S01. Height may be tuned down inside the 24–32 m band without re-amendment.

## 5. Negative gates

FAIL if the realization:

- introduces a climbable tower, rooftop route or mirador without CITY-06/CITY-02 amendment;
- leaves the F02 region;
- promotes the one-off into a reusable family by implication;
- uses castle/keep language vetoed by ART;
- or treats the tower as reason to move W04/W05 or the plaza.
