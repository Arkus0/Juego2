# WP-CITY-06 — Worker → independent Reviewer handoff

WP: `WP-CITY-06 — Interiors + layered discovery`  
Contract: `Docs/workpacks/CITY/WP-CITY-06.md`  
PR: **#90**  
Baseline SHA: `632a63c089f844313e567046b3df541e435d75d4`  
Active Worker: `ChatGPT GPT-5.6 Sol`  
Worker history: `ChatGPT GPT-5.6 Sol`  
Transfer SHA: `NONE`  
fail_cycle: **0**

This file is the Worker's **final tracked branch mutation before freeze**. The exact 40-character commit containing this handoff is read from PR #90 immediately afterward and recorded in PR metadata as both `Candidate HEAD SHA` and `Frozen candidate SHA`. PR metadata is the authoritative exact-SHA freeze record.

## Worker state at handoff

```text
Predecessor contract check: Docs/evidence/WP-CITY-06/WORKER_PLAN.md
Worker pre-review: CLEAN
Worker pre-review findings fixed: 2
Worker pre-review evidence: Docs/evidence/WP-CITY-06/WORKER_PRE_REVIEW.md
Interior coverage: Docs/evidence/WP-CITY-06/INTERIOR_COVERAGE_AUDIT.md
Depth/shell compatibility: Docs/evidence/WP-CITY-06/DEPTH_SHELL_COMPATIBILITY_AUDIT.md
Discovery causality: Docs/evidence/WP-CITY-06/DISCOVERY_CAUSALITY_AUDIT.md
Semantic owner: Docs/production/CITY_INTERIORS_DISCOVERY.md v0.1
Branch frozen after this commit: YES
Worker verdict: IN_REVIEW after PR metadata freeze
Reviewer verdict: PENDING fresh independent review
```

## Direct predecessor reconstructed

Accepted `WP-CITY-05`:

- candidate `10d1528b0354b16a614fb10a3933a25b32f15f28`;
- independent PASS review `#5267776704`;
- PR #83 merge `47909a72eb6d38332f62e9c01426c8cd40e1863b`;
- DocSync PR #87 merge `cff6d4d0d40786dd1c002a8cc46e768b478ee3cc`;
- residual reconciliation PR #88 / baseline `632a63c089f844313e567046b3df541e435d75d4`.

CITY-06 consumes CITY-05's reviewed shells, building/site families and **fail-closed required access roles**. It does not re-prove or weaken them.

The critical inherited invariant remains:

```text
programme_required_roles ⊆ bound_required_roles
```

where the role vocabulary is:

```text
public
service
private
semi-private
```

`vertical`, `court`, `rear`, `staff`, `storage`, `records`, `work`, `rooms`, `landing` and similar terms are form/use qualifiers, not substitute roles.

## What the CITY-06 candidate freezes

`Docs/production/CITY_INTERIORS_DISCOVERY.md` provides one reviewed planning grammar for selective interior depth and layered discovery:

- explicit `I0`, `I1`, `I2`, `I3` allocation semantics;
- eight reusable interior topology/use families;
- one bar-only hero overlay rather than bespoke hero treatment everywhere;
- exact interior realization of inherited public/service/private/semi-private role sets;
- complete allocation of every inherited `I1..I3` place;
- explicit treatment of every CITY-02 `S2..S4` place, including deep exterior `I0` places;
- exterior-led second layers so depth does not require an enclosed room;
- authored/systemic/hybrid discovery taxonomy;
- route statuses `AUTHORED_SPATIAL_NOW`, `FUTURE_OWNER_CONDITIONAL`, `NOT_A_ROUTE`;
- district/location-family discovery-layer matrix;
- six multi-route-ready example opportunity families;
- strict causal ownership for follow/invitation/information/schedule/governance/return-after-change motifs;
- optional rare martial/kung-fu/Hong-Kong/Chinese-cinema strand that is local, missable and non-load-bearing;
- negative-content rules against secret inflation, omniscience, access laundering and theme-park drift;
- later authoring/discovery information requirements without defining a canonical Arkus schema;
- explicit CITY-03 candidate-comparison fields and selected-seed minimum depth/discovery mix.

No exact seed is selected here.

## Coverage results

### Interior backlog

Accepted CITY-02 backlog:

```text
I3: 1
I2: 6
I1: 7
TOTAL: 14
```

Candidate result:

```text
I1_I3_PLACES_EXPECTED: 14
I1_I3_PLACES_ALLOCATED: 14
INTERIOR_COVERAGE: PASS_FOR_WORKER_PRE_REVIEW
```

Only `loc.casco.bar` receives the I3 hero overlay.

### Full S2–S4 coverage

Accepted CITY-02 depth universe:

```text
S4: 1
S3: 7
S2: 10
TOTAL S2-S4: 18
```

Candidate result:

```text
S2_S4_PLACES_EXPECTED: 18
S2_S4_PLACES_COVERED: 18
S2_S4_COVERAGE: PASS_FOR_WORKER_PRE_REVIEW
```

The four S2/S3 places that remain `I0` are intentionally exterior-led:

- `loc.casco.shared_court`;
- `loc.plaza.market`;
- `loc.barrio.lavadero`;
- `loc.ensanche.shared_garden`.

They are not promoted to rooms merely to satisfy a depth count.

### CITY-05 shell compatibility

Every `I1..I3` allocation is mapped back to its accepted CITY-05 building/shell posture:

```text
I1_I3_SHELL_ROWS_EXPECTED: 14
I1_I3_SHELL_ROWS_COMPATIBLE: 14
SHELL_COMPATIBILITY: PASS_AT_PLANNING_TOPOLOGY_LEVEL
```

This is not metric/Unity proof. If realized geometry later cannot preserve required depth/access inside the reviewed shell/site envelope, that is a predecessor reopen/falsification signal, not permission to drop a role or invent an extra floor.

## Interior-depth rules Reviewer should attack

### I0

No enterable enclosed interior promised. Asset/prefab rooms do not promote it.

### I1

One primary meaningful enclosed zone plus zero/one minor support pocket. Multiple required roles may use distinct thresholds/screening without becoming multiple full rooms.

Key false-green control:

> `loc.puerto.work_hub` has `{public, service, private}` but remains I1. Three roles do **not** imply three rooms.

### I2

Two to four meaningful zones, multiple thresholds and at least one secondary access/use layer. No basement, roof, loop or secret-room quota.

### I3

`loc.casco.bar` only. Reusable `if.social_house` base plus `hero.bar_layered`; public/service/semi-private all remain required. Hero depth may use accepted vertical/court/rear/equivalent layering but cannot invent shell mass merely to look heroic.

## Reusable family result

Candidate families:

- `if.retail_shallow`;
- `if.work_support_shallow`;
- `if.shop_work_deep`;
- `if.civic_office`;
- `if.residential_cluster`;
- `if.workshop_deep`;
- `if.social_house`;
- `if.lodging_common`;
- `hero.bar_layered` — overlay only, bar only.

Reviewer should challenge whether any family is secretly narrative-specific, whether a shallow family permits depth inflation, and whether the bar overlay can stay inside accepted shell variants without silently demanding a new floor.

## Discovery truth boundary

Candidate distinguishes:

### `AUTHORED_SPATIAL_NOW`

A discovery route grounded in accepted spatial/access/object substrate that CITY-06 may honestly specify now.

### `FUTURE_OWNER_CONDITIONAL`

A plausible future route whose truth requires a later owner. The candidate names owner categories such as PA-01/02 daily-life/agency, PA-03 social graph, PA-04/05 knowledge/rumour, PA-06 memory/consequence, PA-07 material dependencies, PA-09 player agency, PA-11 investigation and PA-12 governance.

These are **future causal owners**, not runtime proof.

### `NOT_A_ROUTE`

Duplicate presentation, proximity magic, hidden engine truth or a plausible story without a causal owner.

A `MULTI_ROUTE_READY` opportunity is preproduction readiness only. It is not proof that later systemic routes exist.

## Multiple-route examples

Reviewer should independently challenge these six examples:

- `disc.bar.secondary_layer`;
- `disc.ayuntamiento.records_boundary`;
- `disc.workshop.material_trace`;
- `disc.residence.shared_private`;
- `disc.fonda.outsider_context`;
- `disc.puerto.concession_state`.

Each has at least one authored spatial substrate plus separate future-conditional causal routes. None may publicise service/private access merely to increase route count.

## Requested discovery motifs

The WP-requested motifs are bounded as follows:

- follow actor → future daily-life/agency runtime;
- invitation → future relationship/access semantics;
- key/access → access thresholds may exist, but CITY-06 does **not** mandate keys or an inventory system;
- overheard information → future knowledge/rumour/investigation/dialogue semantics;
- document → authored document/notice surface may exist; specific truth/knowledge effect remains later-owned;
- schedule change → future daily-life/consequence owner;
- municipal consequence → future governance owner;
- return-after-change → future memory/material/player-agency/governance owner depending on cause.

An unowned motif is not counted as a discovery route.

## Major-district second layers

All nine major district families have a grounded second-layer mechanism without requiring underground/roof content:

1. Casco Viejo — bar service/semi-private + shared court/stair;
2. Plaza/Ayuntamiento — civic public→staff/private + institutional surfaces;
3. Calle Mayor — bakery work/service + shallow service thresholds;
4. Barrio Alto — residence shared/private + lavadero;
5. Ensanche — neighbourhood service + shared garden;
6. Ribera/Talleres — workshop/material + paseo observation;
7. Puerto — social/service + work/material + landing context;
8. Entrada/Carretera — fonda private/service + arrival/logistics;
9. La Vega — supply/material + quiet/huerta context.

Reviewer should verify the candidate has not achieved this by turning every quiet place into a clue/event dispenser.

## Rare martial/cinema strand

The candidate deliberately makes this **optional**:

- a seed with zero such content is fully valid;
- if present, at most one primary opportunity in the selected seed;
- it attaches to an otherwise coherent ordinary place;
- it cannot explain city governance, economy, crossings, every family or every hidden fact;
- missing it cannot break an ordinary route/place/scenario;
- final cultural/narrative claims need a later explicit owner.

Reviewer should FAIL any reading that turns this accent into the town's universal ontology.

## Worker-found defects fixed before freeze

Strict Worker pre-review found **2** evidence-boundary defects:

1. S2–S4 coverage existed semantically but was not exhaustively proven as an 18-row universe;
2. shell/access compatibility was explicit for roles but did not yet map all 14 interiors back to accepted CITY-05 shell/building families.

Both were repaired in `DEPTH_SHELL_COMPATIBILITY_AUDIT.md` before freeze. No upstream product semantic owner was changed.

## Causal negative controls

Fresh Reviewer should reproduce at least these:

1. bar `{public, service}` + `vertical/court` but no `semi-private` → **FAIL**;
2. everyday shop makes service optional → **FAIL**;
3. service yard becomes ordinary-public shortcut → **FAIL**;
4. I0 prefab interior auto-opens → **FAIL**;
5. I1 work hub gets one room per access role → **FAIL depth inflation**;
6. Puerto worker social receives bar hero overlay → **FAIL hero inflation**;
7. every I2 gets one hidden room as quality proof → **FAIL secret quota**;
8. same hidden boolean presented twice counts as two discovery routes → **FAIL duplicate causality**;
9. “a neighbour tells you” with no knowledge/social owner → **FAIL unowned systemic route**;
10. PA plan/research acceptance is treated as implemented runtime → **FAIL future-owner confusion**;
11. engine provenance/occupancy truth becomes direct NPC knowledge → **FAIL omniscience**;
12. quiet Vega paseo must emit clues/events → **FAIL quiet-content contamination**;
13. zero martial/cinema content makes a seed invalid → **FAIL optional-strand rule**;
14. one hidden martial society explains civic/port/residential systems → **FAIL universal explanation**;
15. bar hero overlay silently adds an unsupported floor → **FAIL shell-growth rule**;
16. Vega supply node moves to dense urban shell for easier role layout → **FAIL exterior-grammar drift**.

## CITY-03 handoff boundary

CITY-03 receives comparison requirements, not a winner:

- which I1–I3 places each candidate contains;
- reusable interior family/overlay needs;
- exact inherited access roles;
- exterior-led I0/S2–S3 depth;
- authored vs future-conditional discovery opportunity routes;
- quiet/ordinary no-secret places;
- optional extraordinary-strand posture (`0` valid);
- concrete predecessor reopen signals.

The selected seed must contain enough reviewed depth/discovery variety, but CITY-06 does not draw its boundary or invent its exact scenario pack.

## Scope boundary

Candidate does **not**:

- change CITY-00 geography/crossings;
- change CITY-01 route/access topology;
- change CITY-02 programme, capacity or depth classifications;
- change CITY-05 exterior shell/building grammar;
- select CITY-03 seed;
- build CITY-04/07 Unity geometry;
- import assets or define canonical catalogue/native IDs;
- implement a discovery API/schema;
- author quests, named narrative facts or cinematics;
- define runtime schedules, opening hours, invitations, key/inventory mechanics, beliefs, dialogue, relationships, decisions, incidents, material simulation, governance, memory/consequence or saves;
- claim empirical proof for future systemic/hybrid routes.

## Concurrent-main state at Worker pre-review

While this branch was Draft, `main` advanced to `3688b7b9a27355b0fda160c20e57a385e40c6814` through accepted `WP-PA-01`. Baseline→that main state changes only PA-01 evidence/result files and no CITY dependency or candidate path. PR #90 remained mergeable.

PA-01 research acceptance does not instantiate runtime schedule/follow semantics, so CITY-06's conditional route classification remains valid. No semantic rebase was required.

Fresh Reviewer should still reconstruct current `main` at review start and verify no later concurrent change has altered a binding CITY input.

## Known residuals

- exact seed boundary and selected subset;
- exact room dimensions/clearances and metric shell fit;
- realized Unity geometry/collision/navmesh/traversal/accessibility;
- asset/prefab availability;
- final canonical authoring/discovery schema and native locators;
- runtime Living World/gameplay semantics;
- final narrative/lore/quest content;
- empirical proof of systemic/hybrid routes;
- CITY-07 keeper realization;
- CITY-08 public authoring/reuse proof.

A later concrete contradiction is a reopen/falsification signal at its causal owner, not permission to silently weaken this candidate.

## Fresh Reviewer challenge points

1. Independently reconstruct all 18 CITY-02 S2–S4 places; do not trust the Worker's universe.
2. Independently reconstruct the 14 I1–I3 backlog and all inherited CITY-05 required-role sets.
3. Cross-check every interior allocation against the actual accepted CITY-05 shell/building family mapping.
4. Attack I1 for disguised multi-room depth and I2 for disguised hero treatment.
5. Verify only `loc.casco.bar` receives I3/hero semantics.
6. Search for any I0 interior promotion hidden in prose or examples.
7. Challenge internal circulation for implicit public graph shortcuts or AS/publicisation.
8. For each claimed multi-route opportunity, identify the distinct causal truth source and later owner; reject duplicate presentation.
9. Verify PA workpack references are ownership labels, not runtime claims.
10. Challenge every requested motif (follow, invitation, key/access, overheard, document, schedule, municipal consequence, return-after-change) for stolen runtime semantics.
11. Verify every major district's second layer can remain coherent when future systemic routes do not exist yet.
12. Attack the rare martial/cinema strand for hidden load-bearing product assumptions or setting drift.
13. Verify CITY-03 receives comparison constraints without a seed being chosen here.
14. Inspect the complete baseline→candidate diff independently and do not treat Worker evidence as an independent verdict.

## Worker stop condition

After PR #90 is updated with the exact SHA of this commit and marked Ready, the Worker stops writing. Any independent Reviewer FAIL requires a fresh repair Worker cycle, a new pre-review and a new frozen SHA.