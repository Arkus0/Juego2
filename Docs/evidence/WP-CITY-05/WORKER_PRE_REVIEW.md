# WP-CITY-05 — Strict Worker pre-review

WP: `WP-CITY-05 — Streets, parcels + reusable building families`  
Contract: `Docs/workpacks/CITY/WP-CITY-05.md`  
Baseline: `9a8ff542d0a0ce70f9a6d5fa3baea2f8a5691cbb`  
Pre-review actor: same Worker (`ChatGPT GPT-5.6 Sol`) in strict challenge mode  
Class: NON-FOUNDATIONAL / REMOTE  
Repair cycle: **1** after independent review `#5266587982`

```text
WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 3
PROOF_BUDGET_VERDICT: N/A — non-foundational WP
```

This is Worker readiness evidence only. It is not independent PASS and does not constrain the fresh Reviewer's search.

## 1. Dependency / baseline recheck

At strict repair pre-review time current `main` remains exactly `9a8ff542d0a0ce70f9a6d5fa3baea2f8a5691cbb`, the CITY-02 DocSync merge baseline from which this branch started. The branch is ahead of that baseline and zero behind. No accepted predecessor changed during CITY-05 repair.

Direct predecessor remains:

- CITY-02 accepted candidate `ddcb22d9a2dfcf68054db2b762fc7ff7aed68ae8`;
- independent PASS review `#5266113192`;
- implementation PR `#81`, merge `1bdb7b6e914692493b17a9d2215d881dfe326cb0`;
- DocSync/current baseline `9a8ff542d0a0ce70f9a6d5fa3baea2f8a5691cbb`.

Inherited CITY-00/01 geography/access, CITY-02 programme/Q6 bounds and ART identity are consumed rather than re-proved.

## 2. Independent FAIL reconstructed before repair

Independent review `#5266587982` reviewed exact candidate `6e8179828d14f009239463d732fba63ed8d8e446` and returned FAIL for one causal family:

- CITY-02 requires `loc.calle.everyday_shop = public + service`, but CITY-05 allowed `public + optional service`;
- CITY-02 requires a semi-private layer for `loc.casco.bar`, but CITY-05 allowed a third support described as `semi-private/vertical/court candidate`, which could be read as satisfied by a merely vertical/court socket;
- the owner was therefore internally inconsistent with its own rule that programme-required access relations must be preserved.

The review explicitly did **not** reject capacity/host fit and did **not** use the known generic WP-CITY CI resolver failure as a product blocker.

Repair target was therefore restricted to functional-POI access binding and its regression evidence.

## 3. Complete baseline → repair candidate diff reviewed

Before final handoff metadata, the branch changes only CITY-05 production/evidence surfaces:

- `Docs/production/CITY_ENVIRONMENT_GRAMMAR.md`;
- `Docs/production/CITY_ENVIRONMENT_DISCOVERY_REQUIREMENTS.json`;
- `Docs/evidence/WP-CITY-05/WORKER_PLAN.md`;
- `Docs/evidence/WP-CITY-05/CAPACITY_FIT_CHECK.md`;
- `Docs/evidence/WP-CITY-05/GRAMMAR_AUDIT.md`;
- `Docs/evidence/WP-CITY-05/ACCESS_RELATION_CONFORMANCE.md`;
- this pre-review;
- final handoff metadata.

No code, Unity project, prefab, imported asset, H0/H1 contract, accepted CITY-00/01/02 owner or ART owner is modified.

## 4. Repair invariant

The semantic owner now defines one fail-closed functional-POI rule:

```text
programme_required_roles = normalize(CITY-02 Default access posture)
bound_required_roles     = roles required after functional-POI binding
PASS only if programme_required_roles ⊆ bound_required_roles
```

The normalized access-role vocabulary is limited to:

```text
public
service
private
semi-private
```

Spatial/use qualifiers such as `vertical`, `court`, `rear`, `staff`, `storage`, `records`, `work`, `rooms`, `waiting` and `frontage` describe how a role is realized. They cannot replace the role.

A generic family may keep an anchor optional before binding. If the CITY-02 programme row requires that role, binding promotes the anchor to required.

## 5. 23/23 access-role challenge

`Docs/evidence/WP-CITY-05/ACCESS_RELATION_CONFORMANCE.md` checks every accepted A/B row.

Result:

```text
A_B_ROWS_CHECKED: 23
A_B_ROWS_CONFORMANT: 23
ACCESS_RELATION_CONFORMANCE: PASS
```

Key repaired examples:

- `loc.calle.everyday_shop`: `{public, service}` -> `{public, service}`;
- `loc.casco.bar`: `{public, service, semi-private}` -> `{public, service, semi-private}`;
- `loc.puerto.worker_social`: `{public, service, semi-private}` -> `{public, service, semi-private}`.

Important non-overcorrections:

- `loc.calle.pharmacy` remains `{public, private}`; service is not invented as a CITY-02 requirement;
- `loc.ribera.service_yard` remains `{service}` with public conditional, so restricted service access is not publicised.

## 6. Causal access negative controls

### NC-ACCESS-01 — everyday shop loses service

```text
required = {public, service}
bound    = {public}
```

Subset test fails -> **FAIL**.

### NC-ACCESS-02 — bar substitutes form for role

```text
required  = {public, service, semi-private}
bound     = {public, service}
form_tags = {vertical, court}
```

`semi-private` is absent -> **FAIL**.

### NC-ACCESS-03 — pharmacy accidentally gains required service

CITY-02 says `public + private staff/storage`; normalized requirement stays `{public, private}`. This guards against rewriting the predecessor in the opposite direction.

### NC-ACCESS-04 — service yard becomes ordinary public

CITY-02 says `service-first, conditional public edge`; required set stays `{service}`. Public remains conditional and `AS` is not promoted.

## 7. Existing Worker-found defects remain repaired

### Finding 1 — access-width false compatibility

Initial street families allowed `AR` on ranges whose low end was appropriate only for AP/AF/service conditions.

Repair `b645f26518b29a7bbebad0129f8136f7e0fd0313` gives explicit road-capable minima/ranges and keeps historic/stepped lanes from satisfying inherited `AR`.

### Finding 2 — inherited-cap pass could hide impossible CITY-05 host

The first capacity evidence proved T/S/M/L but not that the same composition fit its selected parcel/site dimensions and coverage.

Repairs `e386b35136515ac4ad2490195fa7a58b4adba25b` and `21e063cf0946a3ed8228d20c1f0c39eb7d952688` bind all 23 A/B rows to host/site witnesses and parcel-coverage checks.

### Finding 3 — independent-review access-role weakening

Repairs:

- `2125a7f286f9b1b01db6b2b0a0cd06a99edb0c0e` — semantic owner: monotonic POI binding + exact 23-row role sets;
- `f43809e4e61f0421d227368080dd093fc80024c2` — discovery projection: required/optional status after binding + role provenance;
- `29755aef0b07e1094f760c548e5f3cbbc11c5b2a` — 23/23 access conformance + negative controls;
- `779432e29a37043221125442c5b84da1bd4556ad` — grammar audit reconciled with the independent FAIL and repair.

## 8. Capacity recomputation / footprint challenge

The access repair changes the required **role** of anchors already supported by the selected family/site vocabulary. It does not change:

- CITY-02 PE class;
- shell/support/open/apron witness numbers;
- parcel dimensions or parcel coverage;
- per-part caps;
- hard ordinary/quiet reserve;
- 15% circulation/uncommitted margin.

Dense host-valid witness totals remain:

- Wedge 3,730 m²;
- Ensanche 900 m²;
- Barrio Alto 500 m²;
- Puerto 2,700 m²;
- Entrada 2,150 m²;
- dense witness total 9,980 m².

These are existence witnesses only and do not replace CITY-02's conservative 57,300 m² charge or any T/S/M/L/cap/reserve boundary.

A later realized geometry that cannot fit a newly-required role inside the current host is a genuine falsification; it must reopen the site/capacity claim rather than drop the role.

## 9. Workpack acceptance challenge

### Varied composition without bespoke-per-parcel production

PASS. Eight street families, seven junction families, nine parcel/open-site families and nine building families provide reviewed bounded variation; one-offs have promotion rules.

### Shared constrained grammar across city characters

PASS. Casco, commercial/civic, slope residential, Ensanche, Ribera, Puerto and rural/arrival conditions use combinations of shared families rather than independent kits.

### Entrance/service relations

PASS after repair. All 23 A/B rows have explicit functional bindings whose required access-role sets are no weaker than CITY-02. Generic optional family sockets can be promoted by the binding but cannot weaken inherited roles.

### Constrained choice for weaker agents

PASS. The selection sequence now requires exact access-role binding before proposal. The JSON projection exposes functional-POI programme ID, programme-required roles, bound-required roles, required/optional anchor status and provenance.

### No implementation/canonical leakage

PASS. No asset import, prefab implementation, Unity scene, catalogue authority, canonical WorldState contract or public Arkus capability is created.

### CITY-06 Definition of Done

PASS. CITY-06 receives reviewed street/parcel/shell families plus exact public/service/private/semi-private role obligations for every A/B POI, while room/discovery topology remains downstream.

## 10. Mobility/topology challenge

No street or junction family creates connectivity. Explicit checks remain:

- no Ensanche↔Orilla-sur edge;
- no dry Wedge→Puerto continuation;
- X1–X7 remain the only crossing identities;
- X6/X7 landing composition represents a state-valid alternative rather than simultaneous geometry;
- `AS` remains service/back-only;
- place-internal courts/arcades/landings are not city-graph shortcuts;
- BUS arrival does not imply bus-compatible continuation into pedestrian fabric.

The access repair does not change CITY-01 route classes.

## 11. Quiet/ordinary/art challenge

The candidate retains substantial cheap fabric and rejects using it as A/B overflow:

- W07/Vega quiet paseo;
- W15 lavadero/ravine context;
- upper ordinary residential fabric;
- ordinary Puerto C/S1 work frontage;
- D/S0 scenic mass.

Puerto remains in the inland stone/dark-tile language and low work-yard grammar. No ART reopen condition is triggered.

## 12. Scope challenge

Searched for early ownership of:

- exact retained seed;
- Unity realization/measurement;
- prefab/import/catalogue authority;
- detailed interior rooms/secret routes/discovery topology;
- runtime schedules, beliefs, dialogue, incidents, permissions or save-state;
- canonical Arkus schema/transport.

None is claimed. The JSON remains a requirements projection, not a canonical contract or Unity schema.

## 13. Residual classification

Recorded downstream rather than hardened here:

- exact parcel polygons and citywide placement;
- realized width/slope/turning/retaining geometry;
- exact CITY-03 seed;
- detailed CITY-06 interiors/discovery;
- actual asset/module/prefab inventory and adoption;
- implemented Arkus discovery capabilities;
- keeper realization and runtime semantics.

Concrete later evidence that a realized composition cannot meet host/access/capacity constraints is a named falsification/reopen condition.

## 14. Verdict

`WORKER_PRE_REVIEW: CLEAN`

Three material defect classes are now accounted for: two found and repaired by the Worker before first freeze, and the independent-review access-role weakening found on exact candidate `6e8179828d14f009239463d732fba63ed8d8e446` and repaired in fail cycle 1.

No known in-claim blocker remains. The next branch mutation is the revised Worker handoff metadata. After that commit the Worker must read exact HEAD, record it as Candidate/Frozen SHA in PR #83, mark the PR Ready and stop writing for a fresh independent re-review.