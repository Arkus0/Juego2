# WP-CITY-05 — Worker → independent Reviewer handoff

WP: `WP-CITY-05 — Streets, parcels + reusable building families`  
Contract: `Docs/workpacks/CITY/WP-CITY-05.md`  
PR: **#83**  
Baseline SHA: `9a8ff542d0a0ce70f9a6d5fa3baea2f8a5691cbb`  
Active Worker: `ChatGPT GPT-5.6 Sol`  
Worker history: `ChatGPT GPT-5.6 Sol`  
Transfer SHA: `NONE`  
fail_cycle: **0**

This file is the Worker's final branch mutation before freeze. The exact 40-character commit containing this handoff is read from PR #83 immediately afterward and recorded in PR metadata as both `Candidate HEAD SHA` and `Frozen candidate SHA`. PR metadata is the authoritative exact-SHA freeze record.

## Worker state at handoff

```text
Predecessor contract check: Docs/evidence/WP-CITY-05/WORKER_PLAN.md
Worker pre-review: CLEAN
Worker pre-review findings fixed: 2
Worker pre-review evidence: Docs/evidence/WP-CITY-05/WORKER_PRE_REVIEW.md
Grammar audit: Docs/evidence/WP-CITY-05/GRAMMAR_AUDIT.md
Capacity/host fit: Docs/evidence/WP-CITY-05/CAPACITY_FIT_CHECK.md
Semantic owner: Docs/production/CITY_ENVIRONMENT_GRAMMAR.md v0.1
Machine-readable requirements projection: Docs/production/CITY_ENVIRONMENT_DISCOVERY_REQUIREMENTS.json
Branch frozen after this commit: YES
Worker verdict: IN_REVIEW after PR metadata freeze
Reviewer verdict: PENDING fresh review
```

## What CITY-05 candidate freezes

The candidate gives later CITY work one reviewed exterior-production grammar:

- 8 street-segment families and 7 junction families that realize accepted CITY-01 edges without creating graph connectivity;
- 9 parcel/open-site families with frontage/depth, coverage, party-wall, setback, slope/retaining, rear/service and no-build/view constraints;
- 9 reusable building families covering ordinary houses, mixed-use, bar/social, shops/services, workshops, warehouse/port, civic, residential multi and rural/peripheral conditions;
- the composition ladder `module -> assembly -> shell -> reusable building -> functional POI -> street segment`;
- explicit A–D/S0–S4/I0–I3 exterior/shell obligations without deriving one axis from another;
- one reviewed exterior composition pattern for every 23/23 CITY-02 A/B place;
- ordinary C/S1 and scenic D/S0 fabric mappings so cheap fabric remains an output rather than empty development reserve;
- a shared district grammar: old/commercial/residential/work/port/rural identity comes from constrained combinations, not unrelated kits;
- later machine-readable discovery information requirements, explicitly non-canonical and non-Unity-authoritative;
- reviewed one-off -> variant/family promotion rules that prevent bespoke-everywhere drift.

## Two Worker-found defects repaired before freeze

### 1. `AR` width ambiguity

Initial street families allowed `AR` on some shared width bands whose low end was appropriate only for AP/AF/service conditions. Repair `b645f26518b29a7bbebad0129f8136f7e0fd0313` makes road-capable minima explicit.

### 2. Capacity witness could pass predecessor cap but fail its own host

The first fit evidence checked T/S/M/L but did not bind the same composition to CITY-05 parcel dimensions/coverage. That could falsely pass a multi-thousand-square-metre workshop below L while naming a single 18×32 m workshop parcel.

Repairs `e386b35136515ac4ad2490195fa7a58b4adba25b` and `21e063cf0946a3ed8228d20c1f0c39eb7d952688` now require every A/B witness to pass its CITY-05 host dimensions, host coverage posture and inherited PE ceiling simultaneously.

## Capacity / host-fit result

All 23 A/B rows have host-valid witnesses. Dense witness totals:

| Part | CITY-05 host-valid witness | Inherited CITY-02 cap |
|---|---:|---:|
| Wedge | 3,730 m² | 37,500 m² |
| Ensanche | 900 m² | 13,500 m² |
| Barrio Alto | 500 m² | 12,000 m² |
| Puerto | 2,700 m² | 15,750 m² |
| Entrada | 2,150 m² | 6,000 m² |
| **Dense total** | **9,980 m²** | — |

These lower witnesses do **not** weaken CITY-02's accepted conservative 57,300 m² charge, T/S/M/L ceilings, caps, hard reserve or 15% circulation/uncommitted margin. They only prove at least one legal CITY-05 grammar composition exists inside those inherited bounds.

Named ordinary/quiet/scenic reserve is not borrowed.

## Causal negative controls

Fresh Reviewer should reproduce at least these failure modes independently:

1. put a 1,500 m² exclusive workshop on one `pc.workshop_court` -> should FAIL CITY-05 host fit even though L=5,000 passes;
2. put a 500 m² shell on a 20×40 `pc.rural_edge` -> should FAIL 62.5% coverage against 45% maximum;
3. force `loc.entrada.depot_forecourt` beyond M -> L reclassification restores CITY-02's 5,000 m² charge and Entrada becomes 8,100 > 6,000 -> REOPEN Q6;
4. let market occupation consume/double-count the always-clear route -> FAIL;
5. use W17/AS as ordinary public service-yard access -> FAIL;
6. borrow W07/W15/upper-residential/ordinary-Puerto reserve -> FAIL.

## Mobility / geography boundary

Candidate adds no edge or crossing and does not reclassify inherited access:

- no dry Wedge→Puerto;
- no Ensanche↔Orilla-sur link;
- X1–X7 only;
- X6/X7 landing composition represents the state-valid crossing, not simultaneous ferry+bridge geometry;
- AS remains restricted service/back;
- internal courts/arcades/shared landings are not city-graph shortcuts;
- BUS arrival does not imply BUS-compatible continuation into pedestrian streets.

## Scope boundary

Candidate does **not**:

- select CITY-03 retained seed;
- create CITY-04 Unity geometry or measured traversal;
- design CITY-06 rooms, secrets or discovery topology;
- import assets, implement prefabs or establish catalogue authority;
- change canonical WorldState or implement an Arkus public capability/schema;
- author Living World schedules, beliefs, dialogue, decisions, incidents, interactions or save semantics;
- advance keeper realization/CITY-07+.

## Fresh Reviewer challenge points

1. Reconstruct CITY-02 PASS/Q6 rather than trusting Worker summaries; verify lower CITY-05 witness totals do not silently replace conservative predecessor ceilings/caps/reserves.
2. Recompute all 23 host witnesses against the actual parcel frontage/depth and coverage bands in the semantic owner.
3. Challenge open-site witnesses for hidden unboundedness; each claimed fit should name a bounded site.
4. Cross-check every `AR/AP/AF/AS` street-family compatibility against accepted CITY-01 rather than accepting the Worker's width fix at face value.
5. Look for implicit connectivity through rear lanes, service courts, landings or X6/X7 compositions.
6. Verify all A/B shell/access mappings consume the accepted CITY-02 programme without inventing room/discovery/runtime semantics.
7. Verify shared building families genuinely cover old/commercial/residential/port/rural characters without district-specific kit authority or asset-pack taxonomy.
8. Verify the JSON is a requirements projection rather than a second canonical catalogue/schema owner.
9. Inspect the complete baseline→candidate diff independently and challenge the promotion rule for bespoke-everywhere escape hatches.

## Known residuals

- exact parcel polygons/citywide placement;
- realized metric widths/grades/turning/retaining geometry;
- CITY-03 retained-seed selection;
- CITY-06 interior/discovery design;
- actual module/prefab/asset inventory and dependency adoption;
- implemented Arkus discovery capability/schema;
- CITY-04/07/08 realization/proof surfaces;
- runtime Living World/gameplay semantics.

Concrete later evidence that a composition cannot satisfy the reviewed host/access/capacity constraints is a reopen/falsification signal, not permission to stretch the grammar silently.

## Worker stop condition

After PR #83 is updated with the exact SHA of this commit and marked Ready, the Worker stops writing. Any independent Reviewer FAIL requires a fresh repair Worker and a new pre-review/freeze cycle.