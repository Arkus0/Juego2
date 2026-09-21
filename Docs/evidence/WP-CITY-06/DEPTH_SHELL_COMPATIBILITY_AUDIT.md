# WP-CITY-06 — Depth + shell compatibility audit

Candidate semantic owner: `Docs/production/CITY_INTERIORS_DISCOVERY.md`  
Baseline: `main@632a63c089f844313e567046b3df541e435d75d4`  
Status: Worker evidence; not an independent review.

## 1. Why this audit exists

Strict Worker challenge found two evidence gaps before freeze:

1. the candidate allocated the full `I1..I3` backlog, but the WP contract asks for interior/access expectations for **all CITY-02 `S2..S4` locations**, including deep exterior `I0` places;
2. access-role conformance was explicit, but compatibility with the **accepted CITY-05 shell/building families** was still mostly implicit.

This audit closes those evidence gaps without changing predecessor semantics or selecting the CITY-03 seed.

## 2. Exact `S2..S4` universe

Reconstructing the accepted CITY-02 programme yields **18** `S2..S4` places:

### S4 — 1 / 1

- `loc.casco.bar` — S4 / I3.

### S3 — 7 / 7

- `loc.casco.shared_court` — S3 / I0;
- `loc.plaza.ayuntamiento` — S3 / I2;
- `loc.calle.bakery` — S3 / I2;
- `loc.barrio.residence_cluster` — S3 / I2;
- `loc.ribera.workshop` — S3 / I2;
- `loc.puerto.worker_social` — S3 / I2;
- `loc.entrada.fonda` — S3 / I2.

### S2 — 10 / 10

- `loc.plaza.market` — S2 / I0;
- `loc.calle.pharmacy` — S2 / I1;
- `loc.calle.everyday_shop` — S2 / I1;
- `loc.barrio.lavadero` — S2 / I0;
- `loc.ensanche.neighbourhood_anchor` — S2 / I1;
- `loc.ensanche.shared_garden` — S2 / I0;
- `loc.ribera.service_yard` — S2 / I1;
- `loc.puerto.work_hub` — S2 / I1;
- `loc.puerto.warehouse_yard` — S2 / I1;
- `loc.vega.supply_node` — S2 / I1.

Coverage result:

```text
S2_S4_PLACES_EXPECTED: 18
S2_S4_PLACES_COVERED: 18
S2_S4_COVERAGE: PASS
```

The 14 `I1..I3` rows are allocated in `INTERIOR_COVERAGE_AUDIT.md`. The remaining four `I0` rows are deliberately exterior-led second layers in the semantic owner:

- shared court — public passage → semi-private court/stair relation;
- market — public terrace → contextual occupiable market layer while circulation stays open;
- lavadero — quiet/public route edge → semi-private domestic-use reading;
- shared garden — public edge → semi-private neighbour garden/court.

No `I0` row is promoted to an enclosed interior to satisfy this coverage count.

## 3. Accepted CITY-05 shell compatibility

The table below binds each inherited `I1..I3` place to the **already accepted CITY-05 exterior family choice** and checks that CITY-06 adds interior topology behind existing access anchors rather than demanding a new exterior family, route, floor or parcel class.

| Place | Accepted CITY-05 shell/building posture | CITY-06 topology | Compatibility result |
|---|---|---|---|
| `loc.casco.bar` | `bf.bar_social` on `pc.historic_row` / `pc.commercial_row`; public + service + semi-private required | `if.social_house` + `hero.bar_layered` | PASS — uses required public/service/semi-private anchors; vertical/court/rear may be form only; hero layer can be an equivalent interior/exterior relation and does not require a new storey |
| `loc.plaza.ayuntamiento` | `bf.civic` + `pc.civic_frontage`; public + service + private | `if.civic_office` | PASS — public office, staff/service and private records/work zones consume the exact reserved civic anchors |
| `loc.calle.bakery` | `bf.mixed_use_house` or `bf.shop_service` on `pc.commercial_row`; public + service + private | `if.shop_work_deep` | PASS — sales/work/service-private topology fits the accepted I2 shell-support obligation without new external connectivity |
| `loc.barrio.residence_cluster` | `bf.residential_multi` on slope parcels; public + private + semi-private | `if.residential_cluster` | PASS — public threshold/shared landing/private relation is the interior realization of accepted cluster anchors |
| `loc.ribera.workshop` | `bf.workshop` + `pc.workshop_court`; public + service + private | `if.workshop_deep` | PASS — customer/work/service-private topology consumes the reviewed workshop/yard anchors |
| `loc.puerto.worker_social` | `bf.bar_social` / `bf.mixed_use_house`; public + service + semi-private | `if.social_house` | PASS — same reusable social topology without the bar-only hero overlay |
| `loc.entrada.fonda` | `bf.mixed_use_house` lodging variant; public + private + service | `if.lodging_common` | PASS — common/private/service relation uses the accepted mixed-use lodging shell support |
| `loc.calle.pharmacy` | `bf.shop_service` on `pc.commercial_row`; public + private | `if.retail_shallow` | PASS — one public room + screened private pocket stays I1; no service role is invented |
| `loc.calle.everyday_shop` | `bf.shop_service` / `bf.mixed_use_house`; public + service | `if.retail_shallow` | PASS — one public room + service threshold/pocket preserves the role repaired in CITY-05 |
| `loc.ensanche.neighbourhood_anchor` | `bf.mixed_use_house` / `bf.shop_service` + `pc.ensanche_garden` / open-site relation; public + service | `if.retail_shallow` | PASS — one shallow room plus service boundary does not consume the small-square route or create another exterior family |
| `loc.ribera.service_yard` | `pc.workshop_court` + `bf.warehouse_port` / `bf.workshop` support; service required, public conditional | `if.work_support_shallow` | PASS — one service support room maximum; no ordinary-public through route is created |
| `loc.puerto.work_hub` | `bf.warehouse_port` + optional small `bf.civic` office; public + service + private | `if.work_support_shallow` | PASS — one compact admin/work zone may expose distinct role boundaries without becoming an I2 office suite |
| `loc.puerto.warehouse_yard` | `bf.warehouse_port` + `pc.warehouse_yard`; public + service | `if.work_support_shallow` | PASS — shallow support room remains subordinate to the accepted yard/apron composition |
| `loc.vega.supply_node` | `bf.rural_peripheral` / `bf.workshop` + `pc.rural_edge`; public + private + service | `if.work_support_shallow` | PASS — one compact supply/work room uses existing gate/work/service/private boundaries; no denser urban shell is required |

Result:

```text
I1_I3_SHELL_ROWS_EXPECTED: 14
I1_I3_SHELL_ROWS_COMPATIBLE: 14
SHELL_COMPATIBILITY: PASS_AT_PLANNING_TOPOLOGY_LEVEL
```

## 4. What this compatibility result does **not** prove

This is preproduction topology compatibility, not metric/Unity proof. It does not prove:

- exact room dimensions, stair clearance or furniture fit;
- realized shell metric geometry;
- collision/navmesh/accessibility;
- asset/prefab availability;
- performance or lighting;
- that any future systemic discovery route works.

Those remain later-owned.

If realized geometry shows that preserving the reviewed interior depth and required roles cannot fit an accepted shell/site witness, the correct result is a **reopen/falsification signal** at CITY-05 and/or CITY-02. CITY-06 may not shrink a required role or borrow protected ordinary/quiet reserve to save the fit.

## 5. Shell-growth negative controls

### NC-S-01 — hero floor invention

Mutant: `hero.bar_layered` is interpreted as requiring an extra floor even when the selected accepted shell variant does not support it.

Expected: **FAIL**. Hero depth may use an accepted vertical/court/rear relation or equivalent layered topology; it cannot invent shell mass. If no accepted shell can realize the commitment, reopen the predecessor rather than adding a floor silently.

### NC-S-02 — shallow-role room multiplication

Mutant: `loc.puerto.work_hub` gets three independently navigable rooms solely because `{public, service, private}` contains three roles.

Expected: **FAIL**. The accepted I1 shell reserves one shallow interior volume; roles may be expressed through thresholds/screening/pockets without becoming three zones.

### NC-S-03 — rural shell urbanisation

Mutant: `loc.vega.supply_node` is moved into a new dense commercial shell because its three role boundaries are easier to lay out there.

Expected: **FAIL**. CITY-06 consumes the accepted rural/workshop shell posture; convenience cannot change district/exterior grammar.

### NC-S-04 — I0 room rescue

Mutant: the Casco shared court receives an enclosed room so every S3 place has an interior.

Expected: **FAIL**. S3/I0 is an intentional orthogonality counterexample; its second layer is exterior.

## 6. Worker finding disposition

The strict Worker challenge therefore found and repaired two **evidence-boundary** defects before freeze:

1. missing explicit 18/18 S2–S4 coverage proof;
2. missing explicit 14/14 mapping from interior topology back to accepted CITY-05 shell families.

No product semantic owner upstream was changed. The candidate's existing interior/discovery rules already supported both conclusions; this audit makes the acceptance bridge explicit and falsifiable.

`DEPTH_SHELL_COMPATIBILITY_AUDIT: PASS_FOR_WORKER_PRE_REVIEW`
