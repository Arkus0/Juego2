# CITY Amendment 01 — SCENE boundary + revised downstream sequencing

Status: **PROCESS / PLANNING AMENDMENT — applies only after merge**  
Date: 2026-09-20  
Does not amend: `WP-CITY-00` candidate or acceptance criteria

## Reason

The original CITY track correctly separated macro spatial planning from H0, but `WP-CITY-02` and `WP-CITY-03` were carrying too much implicit responsibility for building/interior production. A separate `SCENE/` track now owns micro-spatial composition, access/interiors and discovery layering.

This amendment deliberately does **not** repair or reinterpret the active `WP-CITY-00` candidate. CITY-00 must pass its own frozen contract independently.

## Ownership change

- CITY keeps: city constitution, district/crossing graph, travel topology, systemic location programme, retained-seed boundary and macro blockout.
- SCENE owns: street/parcel grammar, building compositions, interiors/access, depth tiers, secrets/discovery layering and exact keeper-scene production specification.

## Revised dependency chain

After CITY-00 PASS:

```text
CITY-00
  ↓
SCENE-00
  ↓
CITY-01
  ↓
CITY-02
  ├─────────────┐
  ↓             ↓
SCENE-01     SCENE-02
  └──────┬──────┘
         ↓
      SCENE-03
         ↓
      SCENE-04
         ↓
      CITY-03
         ↓
      SCENE-05
         ↓
      CITY-04   (LOCAL, plus GATE/bridge prerequisites)
         ↓
      SCENE-06  (LOCAL)
```

## Effects on frozen CITY plans

This amendment supplements rather than silently edits their text:

- `WP-CITY-01` additionally consumes SCENE-00 place/access vocabulary and must reserve plausible micro-route/access opportunities without designing interiors.
- `WP-CITY-02` continues to classify systemic locations, but detailed building/interior depth is expressed by mapping each location to SCENE S0–S4 instead of inventing a second interior taxonomy.
- `WP-CITY-03` MUST NOT start until SCENE-04 PASS. Seed comparison now includes depth/discovery feasibility and reusable-composition cost, not only macro urban topology.
- `WP-CITY-04` MUST NOT start until SCENE-05 PASS in addition to its existing GATE/bridge prerequisites.

## Non-effects

- No H0/H1 guarantee changes.
- No Unity scene or asset import is authorized.
- ART remains owner of setting/visual direction.
- Living World PAs remain owner of actor knowledge/behaviour/causal semantics.
- The active CITY-00 repair is neither weakened nor broadened.
