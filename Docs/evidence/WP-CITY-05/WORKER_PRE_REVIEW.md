# WP-CITY-05 — Strict Worker pre-review

WP: `WP-CITY-05 — Streets, parcels + reusable building families`  
Contract: `Docs/workpacks/CITY/WP-CITY-05.md`  
Baseline: `9a8ff542d0a0ce70f9a6d5fa3baea2f8a5691cbb`  
Pre-review actor: same Worker (`ChatGPT GPT-5.6 Sol`) in strict challenge mode  
Class: NON-FOUNDATIONAL / REMOTE

```text
WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 2
PROOF_BUDGET_VERDICT: N/A — non-foundational WP
```

This is Worker readiness evidence only. It is not independent PASS and does not constrain the fresh Reviewer's search.

## 1. Dependency / baseline recheck

At strict pre-review time current `main` remains exactly `9a8ff542d0a0ce70f9a6d5fa3baea2f8a5691cbb`, the CITY-02 DocSync merge baseline from which this branch started. No accepted predecessor changed after `PREDECESSOR_CONTRACT_CHECK`.

Direct predecessor remains:

- CITY-02 accepted candidate `ddcb22d9a2dfcf68054db2b762fc7ff7aed68ae8`;
- independent PASS review `#5266113192`;
- implementation PR `#81`, merge `1bdb7b6e914692493b17a9d2215d881dfe326cb0`;
- DocSync/current baseline `9a8ff542d0a0ce70f9a6d5fa3baea2f8a5691cbb`.

Inherited CITY-00/01 geography/access, CITY-02 programme/Q6 bounds and ART identity are consumed rather than re-proved.

## 2. Complete baseline → candidate diff reviewed

At the final semantic/evidence inspection before this report, the branch is nine commits ahead and zero behind baseline, changing only:

- `Docs/production/CITY_ENVIRONMENT_GRAMMAR.md`;
- `Docs/production/CITY_ENVIRONMENT_DISCOVERY_REQUIREMENTS.json`;
- `Docs/evidence/WP-CITY-05/WORKER_PLAN.md`;
- `Docs/evidence/WP-CITY-05/CAPACITY_FIT_CHECK.md`;
- `Docs/evidence/WP-CITY-05/GRAMMAR_AUDIT.md`.

No code, Unity project, prefab, imported asset, H0/H1 contract, accepted CITY-00/01/02 owner or ART owner is modified.

## 3. Exact workpack acceptance challenge

### Varied composition without bespoke-per-parcel production

PASS. Eight street families, seven junction families, nine parcel/open-site families and nine building families produce ordinary variation through reviewed parametric choices. One-offs have explicit promotion conditions rather than being the default.

### Shared constrained grammar across city characters

PASS. Casco, commercial/civic, slope residential, Ensanche, Ribera, Puerto and rural/arrival conditions select different combinations of the same shared families. No independent district kit or marketplace taxonomy becomes semantic authority.

### Entrance/service relations

PASS. All 23 A/B rows have explicit exterior host/family/access mappings. Public/service/private/semi-private/vertical/court relations are represented without designing CITY-06 room paths. I0 cannot become enterable merely because an asset contains rooms; only `loc.casco.bar` carries I3/S4 shell support.

### Constrained choice for weaker agents

PASS. The owner freezes a selection/validation sequence and the JSON projection enumerates later discovery fields required to filter legal choices before proposal. The JSON is syntactically valid and explicitly non-canonical.

### No implementation/canonical leakage

PASS. No asset import, prefab implementation, Unity scene, catalogue authority, canonical WorldState contract or public Arkus capability is created. H1's catalogue/native-locator authority boundary is preserved.

### CITY-06 Definition of Done

PASS. CITY-06 receives reviewed street/parcel/shell families, external access anchors, I-depth shell-support rules, no-build/rear vocabulary, A/B host mappings and reuse/discovery requirements without having to invent exterior building rules from scratch.

## 4. Material Worker findings fixed

### Finding 1 — access-width false compatibility

**Defect:** first draft allowed `AR` on several street families sharing ranges whose low end was 2.5–3.5 m. That could let a later composition claim road-capable compatibility at a width intended only for AP/AF/service conditions.

**Repair:** commit `b645f26518b29a7bbebad0129f8136f7e0fd0313` gives `AR` explicit clear-width floors/ranges: ordinary 4–6 m, riverside ≥4 m, service-edge public/service `AR` ≥4 m, plaza `AR` 4–5 m, rural-transition `AR` 4–4.5 m, port-work 4.5–6 m.

**Regression protection:** semantic-owner invariants reject substituting historic/stepped lanes for `AR`; `GRAMMAR_AUDIT.md` records the access-specific widths.

### Finding 2 — inherited-cap pass could hide impossible CITY-05 host

**Defect:** first capacity evidence proved each A/B total below T/S/M/L but did not require the same total to fit its selected CITY-05 parcel/site dimensions. A multi-thousand-square-metre workshop could therefore pass L=5,000 while naming a single `pc.workshop_court` whose maximum host is 18×32 m.

**Repair:** commits `e386b35136515ac4ad2490195fa7a58b4adba25b` and `21e063cf0946a3ed8228d20c1f0c39eb7d952688` replace those false-green totals with 23 explicit host/site witnesses. Each must fit both its named frontage/depth/open-site bound and the parcel-family coverage posture before the inherited T/S/M/L check is even considered.

**Regression protection:** `CAPACITY_FIT_CHECK.md` includes independent host-fit and coverage negative controls.

## 5. Capacity recomputation / false-green challenge

Recomputed host-valid dense witness totals:

- Wedge 3,730 m²;
- Ensanche 900 m²;
- Barrio Alto 500 m²;
- Puerto 2,700 m²;
- Entrada 2,150 m²;
- dense witness total 9,980 m².

These are **existence witnesses only** and do not replace the accepted CITY-02 conservative charge of 57,300 m² or any T/S/M/L ceiling/cap/reserve. The candidate states that explicitly.

Causal negative controls remain live:

1. 1,500 m² workshop on one 18×32 `pc.workshop_court` -> FAIL host fit despite L passing;
2. 500 m² shell on a 20×40 rural parcel -> FAIL 62.5% built coverage against 45% maximum;
3. depot no longer truthfully M -> L charge drives Entrada to 8,100 > 6,000 -> REOPEN CITY-02 Q6;
4. market consumes shared route/double-counts circulation -> FAIL;
5. service yard publicises W17/AS -> FAIL;
6. quiet/ordinary reserve borrowing -> FAIL.

## 6. Mobility/topology challenge

No street or junction family creates connectivity. Explicit checks:

- no Ensanche↔Orilla-sur edge;
- no dry Wedge→Puerto continuation;
- X1–X7 remain the only crossing identities;
- X6/X7 landing composition represents state-valid alternative rather than simultaneous geometry;
- `AS` remains service/back-only;
- place-internal courts/arcades/landings are not city-graph shortcuts;
- BUS arrival does not imply bus-compatible continuation into pedestrian fabric.

No contradictory evidence was found that requires reopening CITY-00/01.

## 7. Quiet/ordinary/art challenge

The candidate retains substantial cheap fabric and explicitly rejects using it as A/B overflow:

- W07/Vega quiet paseo;
- W15 lavadero/ravine context;
- upper ordinary residential fabric;
- ordinary Puerto C/S1 work frontage;
- D/S0 scenic mass.

Puerto uses the same inland stone/dark-tile language and low work-yard grammar; it does not become a coastal/maritime hero kit. No ART reopen condition is triggered.

## 8. Scope challenge

Searched for early ownership of:

- exact retained seed;
- Unity realization/measurement;
- prefab/import/catalogue authority;
- detailed interior rooms/secret routes/discovery topology;
- runtime schedules, beliefs, dialogue, incidents, permissions or save-state;
- canonical Arkus schema/transport.

None is claimed. The JSON explicitly says it is a requirements projection, not a canonical contract or Unity schema.

## 9. Residual classification

Recorded downstream rather than hardened here:

- exact parcel polygons and citywide placement;
- realized width/slope/turning/retaining geometry;
- exact CITY-03 seed;
- detailed CITY-06 interiors/discovery;
- actual asset/module/prefab inventory and adoption;
- implemented Arkus discovery capabilities;
- keeper realization and runtime semantics.

Concrete later evidence that a realized composition cannot meet host/access/capacity constraints is a named falsification/reopen condition.

## 10. Verdict

`WORKER_PRE_REVIEW: CLEAN`

Two material in-claim findings were discovered and repaired before freeze. After re-reading the exact WP acceptance/negative gates, predecessor ownership, full branch diff, capacity/host evidence, mobility boundaries, authority boundary and downstream residuals, no known in-claim blocker remains.

The next branch mutation is the final Worker handoff metadata. After that commit the Worker will read the exact HEAD, record it as Candidate/Frozen SHA in PR #83, mark the PR Ready and stop writing for fresh independent review.