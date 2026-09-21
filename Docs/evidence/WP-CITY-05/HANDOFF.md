# WP-CITY-05 — Worker → independent Reviewer handoff

WP: `WP-CITY-05 — Streets, parcels + reusable building families`  
Contract: `Docs/workpacks/CITY/WP-CITY-05.md`  
PR: **#83**  
Baseline SHA: `9a8ff542d0a0ce70f9a6d5fa3baea2f8a5691cbb`  
Active Worker: `ChatGPT GPT-5.6 Sol`  
Worker history: `ChatGPT GPT-5.6 Sol`  
Transfer SHA: `NONE`  
fail_cycle: **1**

This file is the Worker's final branch mutation before the repaired freeze. The exact 40-character commit containing this handoff is read from PR #83 immediately afterward and recorded in PR metadata as both `Candidate HEAD SHA` and `Frozen candidate SHA`. PR metadata is the authoritative exact-SHA freeze record.

## Worker state at repaired handoff

```text
Predecessor contract check: Docs/evidence/WP-CITY-05/WORKER_PLAN.md
Worker pre-review: CLEAN
Worker pre-review findings fixed: 3
Worker pre-review evidence: Docs/evidence/WP-CITY-05/WORKER_PRE_REVIEW.md
Grammar audit: Docs/evidence/WP-CITY-05/GRAMMAR_AUDIT.md
Capacity/host fit: Docs/evidence/WP-CITY-05/CAPACITY_FIT_CHECK.md
Access conformance: Docs/evidence/WP-CITY-05/ACCESS_RELATION_CONFORMANCE.md
Semantic owner: Docs/production/CITY_ENVIRONMENT_GRAMMAR.md v0.2
Machine-readable requirements projection: Docs/production/CITY_ENVIRONMENT_DISCOVERY_REQUIREMENTS.json v0.2
Branch frozen after this commit: YES
Worker verdict: IN_REVIEW after PR metadata freeze
Reviewer verdict: PENDING fresh re-review
```

## Independent FAIL being repaired

Independent review `#5266587982` reviewed exact candidate:

`6e8179828d14f009239463d732fba63ed8d8e446`

and returned **FAIL** because CITY-05 could weaken access relations already required by CITY-02.

Two concrete counterexamples were accepted as valid:

1. `loc.calle.everyday_shop` — CITY-02 requires `public + service`; CITY-05 had `public + optional service`.
2. `loc.casco.bar` — CITY-02 requires a semi-private layer; CITY-05 allowed an extra `semi-private/vertical/court candidate`, which could be interpreted as satisfied by a merely vertical/court socket.

The reviewer did **not** reject the capacity witnesses and did **not** use the generic WP-CITY verifier failure as a product blocker.

## Causal repair

CITY-05 now treats functional-POI binding as monotonic/fail-closed.

For every A/B place:

```text
programme_required_roles = normalize(CITY-02 Default access posture)
bound_required_roles     = roles required after functional-POI binding
PASS only if programme_required_roles ⊆ bound_required_roles
```

Required role vocabulary:

```text
public
service
private
semi-private
```

`vertical`, `court`, `rear`, `staff`, `storage`, `records`, `work`, `rooms`, `waiting` and `frontage` are form/use qualifiers. They may describe how a role is realized but cannot substitute for the role.

Generic family optionality remains useful before binding. Binding promotes an anchor to required whenever the accepted CITY-02 row requires that role.

## Repair commits

- `2125a7f286f9b1b01db6b2b0a0cd06a99edb0c0e` — semantic owner: fail-closed functional-POI binding, exact 23-row required-role sets, role-vs-form distinction, downstream validation/negative gates;
- `f43809e4e61f0421d227368080dd093fc80024c2` — discovery projection: functional POI kind, required-vs-optional anchor status after binding, role provenance and binding invariant;
- `29755aef0b07e1094f760c548e5f3cbbc11c5b2a` — new 23/23 access-relation conformance evidence + four causal negative controls;
- `779432e29a37043221125442c5b84da1bd4556ad` — grammar audit reconciled with the independent FAIL and repair;
- `6de0177ea7d90314632102864c8597b06a00dce6` — strict Worker pre-review re-run after repair.

## 23/23 access-role result

`ACCESS_RELATION_CONFORMANCE.md` checks every accepted A/B programme row.

```text
A_B_ROWS_CHECKED: 23
A_B_ROWS_CONFORMANT: 23
ACCESS_RELATION_CONFORMANCE: PASS
```

Key rows:

- `loc.calle.everyday_shop` -> required `{public, service}`;
- `loc.casco.bar` -> required `{public, service, semi-private}`;
- `loc.puerto.worker_social` -> required `{public, service, semi-private}`;
- `loc.calle.pharmacy` remains `{public, private}` rather than being incorrectly strengthened to service;
- `loc.ribera.service_yard` remains `{service}` with public conditional, preserving restricted-service semantics.

## Access negative controls

Fresh Reviewer should reproduce these directly:

1. bind `loc.calle.everyday_shop` as `{public}` -> **FAIL**, inherited `service` missing;
2. bind `loc.casco.bar` as `{public, service}` plus `{vertical, court}` form tags -> **FAIL**, inherited `semi-private` missing;
3. normalize pharmacy as requiring service -> **FAIL predecessor fidelity**; CITY-02 requires `{public, private}` only;
4. make the service yard ordinary public because a family exposes a public socket -> **FAIL**; CITY-02 keeps public conditional and `AS` restricted.

## What CITY-05 candidate freezes

The repaired candidate gives later CITY work one reviewed exterior-production grammar:

- 8 street-segment families and 7 junction families that realize accepted CITY-01 edges without creating graph connectivity;
- 9 parcel/open-site families with frontage/depth, coverage, party-wall, setback, slope/retaining, rear/service and no-build/view constraints;
- 9 reusable building families covering ordinary houses, mixed-use, bar/social, shops/services, workshops, warehouse/port, civic, residential multi and rural/peripheral conditions;
- composition ladder `module -> assembly -> shell -> reusable building -> functional POI -> street segment`;
- explicit A–D/S0–S4/I0–I3 exterior/shell obligations without deriving one axis from another;
- one reviewed exterior composition pattern for every 23/23 CITY-02 A/B place;
- exact fail-closed public/service/private/semi-private required-role set for every A/B POI after binding;
- ordinary C/S1 and scenic D/S0 fabric mappings so cheap fabric remains an output rather than empty development reserve;
- shared district grammar: old/commercial/residential/work/port/rural identity comes from constrained combinations, not unrelated kits;
- later machine-readable discovery information requirements, explicitly non-canonical and non-Unity-authoritative;
- reviewed one-off -> variant/family promotion rules that prevent bespoke-everywhere drift.

## Existing Worker-found repairs retained

### 1. `AR` width ambiguity

Initial street families allowed `AR` on some shared width bands whose low end was appropriate only for AP/AF/service conditions. Repair `b645f26518b29a7bbebad0129f8136f7e0fd0313` makes road-capable minima explicit.

### 2. Capacity witness could pass predecessor cap but fail its own host

The first fit evidence checked T/S/M/L but did not bind the same composition to CITY-05 parcel dimensions/coverage. Repairs `e386b35136515ac4ad2490195fa7a58b4adba25b` and `21e063cf0946a3ed8228d20c1f0c39eb7d952688` require every A/B witness to pass its CITY-05 host dimensions, host coverage posture and inherited PE ceiling simultaneously.

### 3. Access-role weakening

Independent review exposed the first frozen candidate's false-green family. Fail cycle 1 repairs it at the POI-binding boundary rather than by making every generic family globally strict.

## Capacity / host-fit result

All 23 A/B rows retain host-valid witnesses. Dense witness totals remain:

| Part | CITY-05 host-valid witness | Inherited CITY-02 cap |
|---|---:|---:|
| Wedge | 3,730 m² | 37,500 m² |
| Ensanche | 900 m² | 13,500 m² |
| Barrio Alto | 500 m² | 12,000 m² |
| Puerto | 2,700 m² | 15,750 m² |
| Entrada | 2,150 m² | 6,000 m² |
| **Dense total** | **9,980 m²** | — |

The access repair changes required anchor roles, not shell/support/open/apron accounting or PE classes. These lower witnesses do **not** weaken CITY-02's accepted conservative 57,300 m² charge, T/S/M/L ceilings, caps, hard reserve or 15% circulation/uncommitted margin.

If later realized geometry shows that a newly-required role cannot fit inside a current host witness, that is a falsification/reopen condition; the role may not be dropped to save the fit.

## Existing capacity / topology negative controls

Fresh Reviewer should also retain these earlier controls:

1. 1,500 m² exclusive workshop on one `pc.workshop_court` -> FAIL host fit even though L=5,000 passes;
2. 500 m² shell on a 20×40 `pc.rural_edge` -> FAIL 62.5% coverage against 45% maximum;
3. `loc.entrada.depot_forecourt` beyond M -> L restores CITY-02's 5,000 m² charge and Entrada becomes 8,100 > 6,000 -> REOPEN Q6;
4. market occupation consuming/double-counting the always-clear route -> FAIL;
5. W17/AS used as ordinary public service-yard access -> FAIL;
6. W07/W15/upper-residential/ordinary-Puerto reserve borrowed by A/B -> FAIL.

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

1. Reconstruct CITY-02 `Default access posture` directly and independently recompute all 23 normalized required-role sets; do not trust the Worker's ledger.
2. Verify `programme_required_roles ⊆ bound_required_roles` for every A/B row and try to reproduce both reviewer counterexamples.
3. Attack role-vs-form ambiguity: a `vertical`, `court`, `rear`, `staff` or `storage` qualifier must not satisfy a missing public/service/private/semi-private role by itself.
4. Verify the repair does not over-strengthen CITY-02, especially pharmacy and service yard.
5. Reconstruct CITY-02 PASS/Q6 and confirm lower CITY-05 witness totals still do not replace conservative predecessor ceilings/caps/reserves.
6. Recompute all 23 host witnesses against the actual parcel frontage/depth and coverage bands.
7. Challenge open-site witnesses for hidden unboundedness; each claimed fit should name a bounded site.
8. Cross-check every `AR/AP/AF/AS` street-family compatibility against accepted CITY-01.
9. Look for implicit connectivity through rear lanes, service courts, landings or X6/X7 compositions.
10. Verify the JSON remains a non-canonical requirements projection rather than a second catalogue/schema owner.
11. Inspect the complete baseline→candidate diff independently and challenge the promotion rule for bespoke-everywhere escape hatches.

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

After PR #83 is updated with the exact SHA of this commit and marked Ready, the Worker stops writing. Any independent Reviewer FAIL requires a new repair cycle and a new pre-review/freeze.