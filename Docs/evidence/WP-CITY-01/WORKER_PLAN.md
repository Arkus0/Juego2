# WP-CITY-01 — Worker plan

WP: `WP-CITY-01 — Mobility, district graph + walk-time topology`  
Contract: `Docs/workpacks/CITY/WP-CITY-01.md`  
Baseline SHA: `7fe44840076eba05f1b67a7633cd33fc67b9023d`  
Branch: `city/wp-city-01-mobility-topology`  
Active Worker: `ChatGPT GPT-5.6 Sol`  
Worker history: `ChatGPT GPT-5.6 Sol`  
Transfer SHA: `NONE`  
fail_cycle: **0**  
Class: PRODUCT / SPATIAL PREPRODUCTION (NON-FOUNDATIONAL)  
Mode: **REMOTE**

This workpack is governed by `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` v1.7. CITY is non-foundational: `FOUNDATIONAL_PROOF_STANDARD.md`, foundational self-attack machinery and foundational exact-SHA proof obligations do not apply. The normal Worker predecessor check, strict Worker pre-review, frozen handoff and fresh independent review do apply.

## 1. PREDECESSOR_CONTRACT_CHECK

### Direct dependency — accepted `WP-CITY-00`

Accepted candidate: `f5c684461b525841487158d873a9b135008df3ab`  
Independent PASS: review `#5261672961`  
Merge: PR `#65`, merge commit `69bbba2603e67cb233a3e1cc36a4173bed41cfc4`  
Post-PASS DocSync: PR `#69`  
Accepted CITY Programme v2: candidate `87a902584f2c46b2d256f6fef26829e9182e7605`, review `#5261734418`, PR `#70`, merge `058e2f4f7c5b018d60cce84e9c89bd07249dd36a`  
Programme DocSync / current baseline: `7fe44840076eba05f1b67a7633cd33fc67b9023d`

### Inherited guarantees consumed by CITY-01

CITY-01 consumes rather than redesigns:

- the Cuña de Confluencia landmass model: Wedge, Ensanche bank and Orilla sur;
- the rule that the Wedge ends at the confluence and Puerto/Entrada are on Orilla sur;
- the complete inter-landmass crossing set `X1..X7`, including State 1 `X1+X6` and State 2 `X1+X7`;
- no Ensanche-bank ↔ Orilla-sur edge and no dry Wedge ↔ Puerto continuation;
- Arroyo crossing availability and Río availability semantics from `CONNECTIVITY_MATRIX.md`;
- the three Wedge longitudinal families: low paseo/sirga, middle Calle Mayor and high callejas;
- plaza-independence by design, including the representative plaza-free routes accepted in CITY-00;
- all twelve permanent `CSI-*` invariants, including protected quiet fabric and municipal levers having physical addresses;
- the adopted scale envelope and the explicit status of all walk times and 1.15 m/s effective pedestrian speed as planning hypotheses, not measurements;
- CITY-00's unresolved ownership split: CITY-01 owns route graph/profiles/chokepoints and bounds verticality; CITY-04 later measures actual traversal.

The accepted CITY-00 connectivity and planarity claims are not re-proved as new guarantees here. CITY-01 performs a compatibility audit because every new route edge must sit inside those inherited boundaries.

### Guarantees newly owned by CITY-01

CITY-01 must freeze:

- one authoritative semantic route/access graph inside the accepted landmasses;
- primary, secondary, quiet and service-route families;
- elevation/access classes for roads, ramps, steep lanes and stairs;
- mobility-profile assumptions for ordinary pedestrian, slower pedestrian, bicycle, service/delivery, arrival/bus, following and time-sensitive travel;
- target route-cost / walk-time bands, including the costs explicitly deferred by CITY-00;
- a chokepoint and alternate-route ledger, including the practical municipal Arroyo closure lever;
- the required scenario matrix;
- explicit measurement questions handed to CITY-04.

CITY-01 does **not** choose systemic locations, interiors, exact retained-seed boundaries, final road engineering, live schedules, navmesh, fast travel or vehicle simulation.

### Concrete conditions that would justify reopening an inherited guarantee

Reopen CITY-00 only if CITY-01 discovers concrete evidence that satisfying its own contract requires one of the following:

1. an inter-landmass route not representable by `X1..X7`;
2. a dry Wedge→Puerto or Ensanche→Orilla-sur connection;
3. removal of the plaza disconnecting the designed base graph despite the inherited route families;
4. the accepted scale/walk-time envelope becoming internally impossible even as a planning target after the full route-cost model is made explicit.

A preference for a shorter route, a prettier graph or easier authoring is not sufficient to reopen CITY-00.

## 2. Claim / trust boundary

Claim: within the accepted CITY-00 geography, CITY-01 can provide one coherent movement/access topology with meaningful alternate routes, profile-specific access, explicit closure behaviour and falsifiable travel-time targets suitable for CITY-02 planning and CITY-04 measurement.

Trusted inputs:

- `Docs/production/CITY_SPATIAL_CONSTITUTION.md`;
- `Docs/evidence/WP-CITY-00/CONNECTIVITY_MATRIX.md`;
- `Docs/evidence/WP-CITY-00/PLANAR_EMBEDDING.md`;
- `Docs/evidence/WP-CITY-00/SCALE_ENVELOPE.md`;
- accepted CITY Programme v2 and the non-binding production blueprint only where they do not override CITY-00.

Outside claim: final metric geometry, measured traversal, crowd simulation, traffic simulation, Unity/navmesh behaviour, location-systemic importance, interior topology and runtime NPC schedule semantics.

## 3. Planned deliverables

Primary semantic owner:

- `Docs/production/CITY_MOBILITY_TOPOLOGY.md`

Evidence/process:

- `Docs/evidence/WP-CITY-01/WORKER_PLAN.md`;
- `Docs/evidence/WP-CITY-01/GRAPH_AUDIT.md`;
- `Docs/evidence/WP-CITY-01/WORKER_PRE_REVIEW.md`;
- `Docs/evidence/WP-CITY-01/HANDOFF.md`.

To avoid the multi-surface drift that caused CITY-00 review failures, the production document will own node IDs, edge IDs, route costs, profile access and chokepoint semantics. Evidence files may test or project those facts but will not redefine them.

## 4. Execution sequence

1. Freeze node vocabulary and an edge ledger derived from CITY-00 landmasses/crossings.
2. Assign route family, access profile and elevation class to each owned edge.
3. Assign planning travel-cost weights while preserving CITY-00's inherited representative bands.
4. Resolve CITY-00 Q4/Q5/Q12 explicitly: route graph/profiles/chokepoint owner, practical Arroyo closure lever, and L1/L1′ traffic character.
5. Run every mandatory scenario and at least two nontrivial closure/alternate-route cases against the same graph.
6. Produce CITY-04 measurement questions without turning planning values into measured facts.
7. Run a strict baseline→candidate Worker pre-review and repair any in-claim defect before freeze.
8. Freeze exact HEAD and hand off to a fresh independent Reviewer.

## 5. Scope guard

Allowed: CITY mobility/spatial-preproduction Markdown and evidence needed by `WP-CITY-01`.

Forbidden: changing CITY-00 accepted geography/crossing semantics; CITY-02 location programming; CITY-03 seed selection; Unity scenes/navmesh/assets; runtime code/tests; live schedules; vehicle simulation; H0/H1 contract changes; ART authority changes.

No implementation semantics outside CITY planning are authorized by this branch.
