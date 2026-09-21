# WP-CITY-05 — CITY-02 access-relation conformance

Semantic owner: `Docs/production/CITY_ENVIRONMENT_GRAMMAR.md`  
Inherited owner: `Docs/production/CITY_LOCATION_PROGRAMME.md`  
Purpose: prove that CITY-05 functional-POI bindings preserve every required CITY-02 external access role for all 23 A/B places instead of weakening programme obligations through generic-family optionality.

This evidence addresses independent review `#5266587982` on candidate `6e8179828d14f009239463d732fba63ed8d8e446`.

## 1. Fail-closed rule

CITY-02 remains authority for each programme row's `Default access posture`. CITY-05 normalizes that posture into required access-role tokens drawn only from:

```text
public
service
private
semi-private
```

Spatial/use words such as `customer`, `staff`, `storage`, `records`, `work`, `rooms`, `waiting`, `frontage`, `shared landing`, `vertical`, `court` and `rear` describe the form or use of an anchor. They do not replace the access role.

Normalization rules used here:

- `public`, `public/customer`, public work/frontage/waiting/arrival/approach -> `public`;
- `service`, service/material, rear/service -> `service`;
- private records/work/rooms/staff-storage -> `private` where CITY-02 names a private layer;
- shared/private-social layers named semi-private -> `semi-private`;
- `conditional public edge` remains conditional and is not promoted to required;
- slash/composite wording preserves every explicitly required role rather than choosing one branch.

A CITY-05 functional binding passes only when:

```text
programme_required_roles ⊆ bound_composition_required_roles
```

A generic family is allowed to advertise anchors as optional before binding. The functional-POI binding must promote any such anchor to required when its CITY-02 programme row requires that role.

## 2. 23/23 A/B conformance ledger

| # | CITY-02 place | Accepted default access posture | Programme-required roles | CITY-05 bound required roles | Result |
|---:|---|---|---|---|---|
| 1 | `loc.casco.bar` | public + service + semi-private layers | `{public, service, semi-private}` | `{public, service, semi-private}` | PASS |
| 2 | `loc.casco.bridgehead` | public | `{public}` | `{public}` | PASS |
| 3 | `loc.plaza.ayuntamiento` | public + staff/service + private records layer | `{public, service, private}` | `{public, service, private}` | PASS |
| 4 | `loc.plaza.market` | public | `{public}` | `{public}` | PASS |
| 5 | `loc.calle.bakery` | public + service + private work layer | `{public, service, private}` | `{public, service, private}` | PASS |
| 6 | `loc.calle.pharmacy` | public + private staff/storage | `{public, private}` | `{public, private}` | PASS |
| 7 | `loc.calle.everyday_shop` | public + service | `{public, service}` | `{public, service}` | PASS |
| 8 | `loc.barrio.residence_cluster` | private + semi-private + public threshold | `{private, semi-private, public}` | `{private, semi-private, public}` | PASS |
| 9 | `loc.barrio.lavadero` | public/semi-private edge | `{public, semi-private}` | `{public, semi-private}` | PASS |
| 10 | `loc.ensanche.neighbourhood_anchor` | public + service | `{public, service}` | `{public, service}` | PASS |
| 11 | `loc.ensanche.shared_garden` | semi-private + public edge | `{semi-private, public}` | `{semi-private, public}` | PASS |
| 12 | `loc.ribera.workshop` | public/customer threshold + service + private work layer | `{public, service, private}` | `{public, service, private}` | PASS |
| 13 | `loc.ribera.service_yard` | service-first, conditional public edge | `{service}` | `{service}` | PASS |
| 14 | `loc.ribera.paseo_edge` | public | `{public}` | `{public}` | PASS |
| 15 | `loc.puerto.work_hub` | public work frontage + service + staff room | `{public, service, private}` | `{public, service, private}` | PASS |
| 16 | `loc.puerto.landing` | public subject to inherited crossing availability | `{public}` | `{public}` | PASS |
| 17 | `loc.puerto.worker_social` | public + service + semi-private | `{public, service, semi-private}` | `{public, service, semi-private}` | PASS |
| 18 | `loc.puerto.warehouse_yard` | service-first + bounded public frontage | `{service, public}` | `{service, public}` | PASS |
| 19 | `loc.entrada.arrival` | public | `{public}` | `{public}` | PASS |
| 20 | `loc.entrada.fonda` | public + private rooms + service | `{public, private, service}` | `{public, private, service}` | PASS |
| 21 | `loc.entrada.depot_forecourt` | service + public waiting edge | `{service, public}` | `{service, public}` | PASS |
| 22 | `loc.vega.supply_node` | public work edge + private/service portions | `{public, private, service}` | `{public, private, service}` | PASS |
| 23 | `loc.vega.quiet_paseo` | public | `{public}` | `{public}` | PASS |

**Result: 23/23 PASS.** No A/B functional binding has a weaker required-role set than its accepted CITY-02 programme row.

## 3. Reviewer blocker repairs

### `loc.calle.everyday_shop`

Before repair, CITY-05 §9 said `public + optional service`, while CITY-02 requires `public + service`. That allowed a generic `bf.shop_service` composition without service to appear locally valid.

After repair:

```text
programme_required = {public, service}
bound_required     = {public, service}
```

The generic family may still expose service as optional for unbound/other uses, but binding this programme ID promotes service to required.

### `loc.casco.bar`

Before repair, CITY-05 allowed `public + service + extra semi-private/vertical/court candidate`. A merely vertical or court-tagged socket could therefore be read as satisfying the third requirement despite CITY-02 requiring a semi-private layer.

After repair:

```text
programme_required = {public, service, semi-private}
bound_required     = {public, service, semi-private}
```

`vertical`, `court` or `rear` may describe the spatial form of that semi-private anchor, but the anchor must still be tagged `semi-private`.

The same rule applies to `loc.puerto.worker_social`.

## 4. Causal negative controls

These controls deliberately remove or mis-tag one inherited role while keeping the rest of the composition plausible.

### NC-ACCESS-01 — everyday shop loses service

```text
programme_required = {public, service}
bound_required     = {public}
```

`programme_required ⊆ bound_required` is false -> **FAIL**.

This is the exact reviewer counterexample. Generic-family optionality cannot rescue it.

### NC-ACCESS-02 — casco bar substitutes form for role

```text
programme_required = {public, service, semi-private}
bound_required     = {public, service}
form_tags          = {vertical, court}
```

The required set is still missing `semi-private` -> **FAIL**.

A vertical/court candidate counts only when the corresponding anchor is also explicitly role-tagged `semi-private`.

### NC-ACCESS-03 — pharmacy is accidentally over-normalized to service

CITY-02 says `public + private staff/storage`, not `public + service`. The normalization remains:

```text
programme_required = {public, private}
```

Adding `service` is permitted only as an extra compatible anchor; it is **not** inherited as mandatory. This control prevents the repair from rewriting CITY-02 in the opposite direction.

### NC-ACCESS-04 — service yard is accidentally publicised

CITY-02 says `service-first, conditional public edge`.

```text
programme_required = {service}
```

CITY-05 therefore requires service while leaving public conditional. The repair does not turn the service yard or an `AS` relation into ordinary public routing.

## 5. Capacity / footprint boundary

This repair changes required **roles on already-selected access anchors**, not the accepted CITY-02 T/S/M/L class, the CITY-05 host/site witness dimensions, shell/support/open/apron accounting or district caps.

The current host families already permit the relevant relations:

- commercial/mixed-use families can expose rear/service relations for the everyday shop;
- `bf.bar_social` already exposes public + service and supports a semi-private/vertical/court socket;
- civic/workshop/warehouse/residential families already expose the other role types represented in the 23-row mapping.

The repair therefore does not require a larger host witness or a different CITY-02 programme-envelope class. `Docs/evidence/WP-CITY-05/CAPACITY_FIT_CHECK.md` remains the capacity authority for this candidate. If a later realized geometry demonstrates that a required role cannot fit within the current witness, that is a genuine falsification and the relevant capacity/site claim must reopen rather than dropping the access role.

## 6. Downstream handoff guarantee

CITY-06 must receive both:

1. the selected family/site/shell constraints; and
2. the exact required access-role set for the bound CITY-02 place.

Discovery/authoring surfaces must expose required-vs-optional status **after** functional-POI binding and preserve provenance back to the CITY-02 programme row. A downstream author cannot infer that a generic optional family socket means an inherited programme role is optional.

## 7. Verdict

```text
A_B_ROWS_CHECKED: 23
A_B_ROWS_CONFORMANT: 23
ACCESS_RELATION_CONFORMANCE: PASS
NEGATIVE_CONTROLS: PASS
CITY_02_REOPEN_REQUIRED: NO
CAPACITY_RECOMPUTATION_REQUIRED: NO
```

The reviewer blocker is repaired causally at the functional-POI binding boundary rather than by making every generic family maximally strict.