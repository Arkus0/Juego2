# CITY Programme v2 — Planning rationale

Status: **PROCESS_ONLY / CANDIDATE — REVIEW CYCLE 2**  
Date: 2026-09-20

## Why this revision exists

After `WP-CITY-00` passed, a planning review found a genuine responsibility gap around streets, parcels, buildings, interiors and layered discovery. A first response proposed a separate `SCENE/` track; that split was rejected before adoption because it would have interleaved two operational programmes around one keeper-city product boundary.

The first CITY Programme v2 candidate then made a different mistake: it reused the already-referenced identifiers `CITY-01..04` for different causal owners. Independent review `#5261712211` correctly failed candidate `96532be9f3214705d8c57b9278b68813817c823a` because accepted `CITY_SPATIAL_CONSTITUTION.md` already binds those IDs.

This revision fixes that contract error without editing or reinterpreting `WP-CITY-00`.

## Stable-ID rule

`CITY-01..04` are stable contract handles because accepted CITY-00 and the Production Blueprint already delegate concrete responsibilities to them. Therefore this programme preserves:

| Stable ID | Preserved owner |
|---|---|
| `CITY-01` | mobility, route graph, walk-time hypotheses/costs |
| `CITY-02` | systemic locations, playable/interior programme, reactive density |
| `CITY-03` | exact retained-seed selection and boundary |
| `CITY-04` | LOCAL retained-seed greybox/blockout and physical validation |

The new planning responsibilities are placed in previously unbound IDs. No compatibility alias, remap or amendment to CITY-00 is used.

## Execution spine

The operational sequence is intentionally **not numeric** because preserving accepted identifiers is more important than making the labels look sequential:

`CITY-00 -> CITY-01 -> CITY-02 -> CITY-05 -> CITY-06 -> CITY-03 -> CITY-04 -> CITY-07 -> CITY-08`.

This remains one linear track with one next workpack at every step.

Modes:

- REMOTE: `CITY-01`, `CITY-02`, `CITY-05`, `CITY-06`, `CITY-03`;
- LOCAL / gated: `CITY-04`, `CITY-07`, `CITY-08`.

## Causal boundaries

- `CITY-01` owns movement/access topology and measurable travel hypotheses, not location importance or interiors.
- `CITY-02` owns which places matter, their A–D systemic importance, their S0–S4 production-depth promise, interior priority and reactive-density programme; it does not compose building shells.
- `CITY-05` owns reusable street/parcel/building-family grammar, not detailed interiors, secrets or bridge/catalogue authority.
- `CITY-06` owns detailed interior/access/discovery substrate, not NPC beliefs, schedules, dialogue or runtime decision semantics.
- `CITY-03` owns exact keeper-seed choice, hard boundary and scenario specification, not local implementation.
- `CITY-04` owns Unity greybox/traversal falsification and measurements, not keeper polish or new city design.
- `CITY-07` owns keeper realization of the validated seed using accepted assets/compositions, not Arkus contract invention.
- `CITY-08` owns the bounded Arkus public-authoring proof and reuse-cost closure, consuming accepted bridge/catalogue/authoring authority.

## Classification rule

Two orthogonal classifications are retained:

- `S0..S4` = **spatial production depth**;
- `A..D` = **systemic importance**.

They may be recorded in the same location programme because both classify places, but they MUST remain independent dimensions. A visually impressive shell may be `S1/C`; an ordinary home may be `S3/A`.

## Why the non-numeric order is acceptable

The alternative would be worse in one of three ways: reinterpret accepted IDs, amend an already-PASSed predecessor merely to make numbering pretty, or fuse causally independent responsibilities into oversized WPs. This plan accepts a small naming irregularity and makes the execution spine explicit in the two programme indexes instead.

No future Worker may infer execution order from numeric sorting. The `Depends on`/`Blocks` fields and this spine are authoritative.

## Lessons from CITY-00

CITY-00 required multiple review cycles because derived connectivity lived across several surfaces and eventually exposed a planar-embedding error. Downstream WPs should prefer one declared owner representation per semantic family and derive secondary diagrams/tables from it instead of allowing independent copies to drift.

CITY-00 also demonstrated that accepted workpack IDs can become contractual references. Once that happens, planning revisions may sharpen an owner's scope but must not silently assign the same ID to another responsibility.

## Process boundary

This programme revision is non-foundational and PROCESS_ONLY. It changes no accepted CITY-00 fact, connectivity edge, scale decision, ART authority, H0/H1 guarantee, Unity implementation, asset dependency or Living World semantic owner.
