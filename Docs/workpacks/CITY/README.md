# CITY track — Keeper City production-space programme

Status: **ACTIVE / NON-FOUNDATIONAL**  
Version: 2.6 — 2026-09-21
Repository: `Arkus0/Juego2`

Accepted progress: `WP-CITY-00` is **COMPLETE**. It passed independent review on frozen candidate `f5c684461b525841487158d873a9b135008df3ab` (review `#5261672961`) and merged via PR `#65` as `69bbba2603e67cb233a3e1cc36a4173bed41cfc4`.

CITY Programme v2 is **ACCEPTED**. Candidate `87a902584f2c46b2d256f6fef26829e9182e7605` passed independent review `#5261734418` in PR `#70` and merged as `058e2f4f7c5b018d60cce84e9c89bd07249dd36a`.

`WP-CITY-01` is **COMPLETE**. Candidate `3444555a983415645dcc2897b748d6c4f6294f19` passed independent review `#5263809993` in PR `#73` and merged as `a9ff655e5bf2319690d919b88bd57389a32483f3`.

`WP-CITY-02` is **COMPLETE**. Candidate `ddcb22d9a2dfcf68054db2b762fc7ff7aed68ae8` passed independent review `#5266113192` in PR `#81` and merged as `1bdb7b6e914692493b17a9d2215d881dfe326cb0`.

`WP-CITY-05` is **COMPLETE**. Candidate `10d1528b0354b16a614fb10a3933a25b32f15f28` passed independent review `#5267776704` in PR `#83` and merged as `47909a72eb6d38332f62e9c01426c8cd40e1863b`.

`WP-CITY-06` is **COMPLETE**. Candidate `c73cbb19f8b152bd4b97ce3154eb18a78deaec3c` passed independent review `#5268249668` in PR `#90` and merged as `acb84ec9ba7aa94e962bf7b35d7e53c0549ba758`. The next executable CITY workpack is `WP-CITY-03`.

## Purpose

Design one coherent keeper-city pipeline from accepted macro geography to movement, meaningful places, reusable environment grammar, layered interiors/discovery and the first retained playable district.

CITY intentionally remains **one operational track**. Splitting CITY/SCENE created coordination overhead without a true authority boundary; merging every responsibility into one giant WP would create the opposite problem. The programme therefore keeps causal boundaries while using one explicit spine.

Core rule:

> **The demo is the first finished piece of the final city, and every CITY workpack reduces uncertainty needed to build that piece once.**

## Accepted predecessor and stable IDs

`WP-CITY-00` owns the accepted spatial constitution: landmasses, river/arroyos, district relationships, port placement, crossing strategy, scale envelope and expansion logic. Downstream WPs consume `Docs/production/CITY_SPATIAL_CONSTITUTION.md` and its reviewed evidence; they do not silently redraw that city.

Accepted `WP-CITY-01` now owns the movement/access topology, route families, planning route costs, mobility-profile assumptions, chokepoints/alternate routes and CITY-04 measurement questions in `Docs/production/CITY_MOBILITY_TOPOLOGY.md`. Downstream CITY work consumes that model; it does not invent routes ad hoc or promote planning weights into measured facts.

Accepted `WP-CITY-02` now owns the reviewed place programme in `Docs/production/CITY_LOCATION_PROGRAMME.md`: A–D systemic importance, S0–S4 spatial depth, A/B use profiles, interior-priority backlog, reactive-density measurement vocabulary and the coarse programme-capacity boundary for CITY-00 Q6. Downstream work must preserve the explicit ordinary/quiet reserve and reopen Q6 if a programmed place exceeds its accepted spatial-demand ceiling.

Accepted `WP-CITY-05` now owns the reusable exterior-production grammar in `Docs/production/CITY_ENVIRONMENT_GRAMMAR.md`: street/junction families, parcel/site constraints, reusable building families, composition ladder, fail-closed CITY-02 functional-POI access binding, later-discovery information requirements and one-off promotion rules. `Docs/production/CITY_ENVIRONMENT_DISCOVERY_REQUIREMENTS.json` remains a non-canonical requirements projection. Downstream CITY work consumes those reviewed shells/access roles; it does not weaken them or turn spatial-form qualifiers into substitute access roles.

Accepted `WP-CITY-06` now owns the reviewed interior/discovery planning grammar in `Docs/production/CITY_INTERIORS_DISCOVERY.md`: bounded I0–I3 interior realization, reusable interior families, inherited access-role preservation, district/location-family second-layer expectations, authored/systemic/hybrid discovery causality, secret-content restraint and the comparison constraints handed to CITY-03. Future PA/H3+ owner names in that grammar are conditional ownership labels, not runtime proof.

CITY-00 also already names `CITY-01..04`. Those identifiers are therefore stable contracts, not free numbering slots:

- `CITY-01` remains mobility / route graph / walk-time topology;
- `CITY-02` remains systemic locations + playable/interior/reactive-density programme;
- `CITY-03` remains exact retained product seed;
- `CITY-04` remains LOCAL Unity blockout/traversal validation.

## Execution chain

The execution order is deliberately not numeric so those accepted IDs keep their meanings:

```text
CITY-00 ✅  Spatial constitution + scale envelope
   ↓
CITY-01 ✅  Mobility, district graph + walk-time topology
   ↓
CITY-02 ✅  Systemic locations + spatial-depth/interior programme
   ↓
CITY-05 ✅  Streets, parcels + reusable building families
   ↓
CITY-06 ✅  Interiors + layered discovery
   ↓
CITY-03    Retained product seed + exact scenario specification
   ↓
CITY-04    LOCAL Unity greybox + traversal validation
   ↓
CITY-07    LOCAL keeper realization
   ↓
CITY-08    LOCAL Arkus authoring proof + reuse closure
```

`CITY-01`, `CITY-02`, `CITY-05`, `CITY-06` and `CITY-03` are REMOTE planning work. `CITY-04`, `CITY-07` and `CITY-08` are gated LOCAL work. Do **not** infer sequence by sorting filenames; the graph above and each WP's dependency fields are authoritative.

H1 supplies engine prerequisites without taking CITY ownership. Under the proposed H1 plan, `CITY-04` may start after its own chain reaches CITY-03 and `WP-H1-08` has accepted managed scenes/assets/components plus Unity diagnostics. It is then a bounded greybox/falsification sidecar, not H2/gameplay authorization and not hidden H1 proof. `CITY-07` remains blocked until `WP-H1-GATE` because keeper realization consumes the accepted bridge and real-asset boundary. `CITY-08` remains a later CITY-owned authoring-efficiency/reuse proof; the H1 Gate's smaller fresh-agent readiness trial does not pre-accept it.

## Two orthogonal classifications

CITY uses two separate axes so production scope does not get confused with systemic importance. Both are frozen in `CITY-02`, where locations are programmed, but they remain independent dimensions.

**Spatial production depth:**
- `S0` scenic envelope / inaccessible context;
- `S1` authored shell or façade only;
- `S2` shallow playable space with bounded interaction;
- `S3` deep playable place with multiple authored spaces/thresholds/anchors;
- `S4` hero layered place with multiple meaningful access/discovery opportunities.

**Systemic importance:**
- `A` primary systemic anchor;
- `B` supporting systemic/playable location;
- `C` ambient urban fabric;
- `D` scenic context.

A visually impressive building may be `S1/C`; an ordinary home may be `S3/A` if persistent actors and consequences depend on it.

## Layered-city rule

The city must have more depth than its street plan without pretending every building is a dungeon. Later CITY work may use, selectively:

- public surface;
- private/semi-private space;
- service/back-of-house routes;
- vertical space;
- social/temporal discovery;
- institutional/historical discovery;
- rare extraordinary/cultural strands.

Important discoveries may be authored, systemic or hybrid. Martial arts/kung-fu/Hong Kong or Chinese-cinema material may form a rare identity-bearing strand, but the town must remain coherent if a player never discovers it.

## Relationship to other programmes

- `WP-ART-00` / ART own visual and setting direction; CITY consumes them and cannot silently restyle the game.
- `Docs/production/PRODUCTION_BLUEPRINT.md` remains non-binding production input. Its `CITY-02` location/interior and `CITY-03` exact-seed delegations remain compatible with this track.
- Living World / PA work owns beliefs, schedules, dialogue, decisions and runtime social semantics. CITY supplies spatial affordances and test situations only.
- H0/H1 and later accepted bridge/catalogue/authoring contracts remain authoritative for implementation surfaces. Unity is a realization/inspection target, not the semantic source of truth.

## Track-wide invariants

1. **Retained-first:** serious geometry begins only where the urban structure is expected to survive into the game.
2. **One accepted geography:** downstream planning cannot contradict CITY-00 landmass/crossing truth.
3. **Stable accepted IDs:** a workpack identifier referenced by an accepted predecessor is not repurposed casually.
4. **Reactive density over acreage:** empty expansion is worse than a smaller city with meaningful places.
5. **Function before decoration:** districts and places exist because people live, work, move, socialise, govern, exchange or discover there.
6. **Quiet is content:** not every street or place produces incidents.
7. **Depth is selective:** not every façade opens; not every interior is systemic; not every secret is a hidden room.
8. **Reuse compounds:** streets/buildings/compositions should become cheaper to author as reviewed families accumulate.
9. **Player absence matters:** locations/routes must make sense when actors use them off-screen.
10. **Discovery has truthful causes:** information/access is not granted merely because the player approached a marker.
11. **No pre-gate production:** REMOTE planning cannot smuggle Unity construction, asset adoption or runtime semantics ahead of the exact gates named by each LOCAL WP; CITY-04's H1-08 greybox exception does not authorize keeper realization or gameplay.
12. **Bridge neutrality:** CITY consumes accepted Arkus/Unity capabilities; it does not invent bridge semantics to make a scene plan pass.

## Current next workpack

`WP-CITY-03 — Retained product seed + exact scenario specification`.
