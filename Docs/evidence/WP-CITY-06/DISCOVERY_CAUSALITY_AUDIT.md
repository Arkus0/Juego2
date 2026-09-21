# WP-CITY-06 — Discovery causality audit

Candidate semantic owner: `Docs/production/CITY_INTERIORS_DISCOVERY.md`  
Baseline: `main@632a63c089f844313e567046b3df541e435d75d4`  
Status: Worker evidence; not an independent review.

## 1. Audit question

Does CITY-06 distinguish spatially authored discovery substrate from future systemic semantics strongly enough that it cannot obtain a false PASS by describing plausible stories as already-truthful routes?

The WP contract requires authored/systemic/hybrid taxonomy, truthful multiple-route examples, real future causal owners and secret-content restraint. It explicitly forbids claiming systemic discovery without a real future owner.

## 2. Route-status invariant

The candidate defines three statuses:

- `AUTHORED_SPATIAL_NOW` — spatial truth CITY-06 can specify from accepted CITY/ART inputs;
- `FUTURE_OWNER_CONDITIONAL` — plausible route only if the named future semantic owner later implements/validates its required state;
- `NOT_A_ROUTE` — duplicate presentation, omniscient engine truth, proximity magic or unowned story plausibility.

A conditional route is not runtime proof and cannot silently become authored simply because its future behaviour is easy to imagine.

## 3. Future-owner coverage

The candidate maps required systemic motifs to owner categories already present in the accepted PA plan:

| Discovery cause | Future owner category | Status in CITY-06 |
|---|---|---|
| actor routine / presence / absence | PA-01 + later H3+ runtime | conditional |
| actor autonomous choice/interruption | PA-02 | conditional |
| relationship/invitation context | PA-03 + later access owner | conditional |
| knowledge/belief | PA-04 | conditional |
| socially propagated information | PA-05 | conditional |
| return-after-consequence / remembered state | PA-06 | conditional |
| work/material dependency | PA-07 | conditional |
| player-caused shared-state change | PA-09 | conditional |
| investigation / evidence legibility | PA-11 | conditional |
| municipal/governance consequence | PA-12 | conditional |

CITY-06 does not assign runtime APIs, tuning or implementation to those WPs.

## 4. Contract-requested route motifs

The workpack asks for examples involving follow actor, invitation, key/access, overheard information, document, schedule change, municipal consequence and return-after-change.

Candidate treatment:

- **follow actor** — conditional on PA-01/02 + later runtime;
- **invitation** — conditional on PA-03 + later access semantics;
- **key/access** — CITY-06 reserves access-role thresholds only; it does not require literal keys or inventory mechanics;
- **overheard information** — conditional on PA-04/05/11 + later dialogue/audio runtime;
- **document** — an authored document/notice surface may exist now; the specific information/knowledge consequence is later-owned;
- **schedule change** — conditional on PA-01/06 and later runtime;
- **municipal consequence** — conditional on PA-12;
- **return-after-change** — conditional on PA-06/07/09/12 depending on cause.

Every requested motif therefore has a bounded status and no unowned “magic route” is required for acceptance.

## 5. Example-opportunity audit

### `disc.bar.secondary_layer`

- authored spatial route: role-bearing public/service/semi-private topology is legible;
- follow actor: PA-01/02 conditional;
- invitation: PA-03 conditional;
- overheard/social information: PA-04/05/11 conditional.

Independence: each route has a different causal source. Service access is not publicised.

### `disc.ayuntamiento.records_boundary`

- authored spatial route: public civic layer versus private/staff boundary;
- governance route: PA-12 conditional;
- return-after-change route: PA-06/09/12 conditional.

Independence: policy/access consequence differs from static spatial/document substrate.

### `disc.workshop.material_trace`

- authored spatial/material reading: workbench/service topology;
- work/material state: PA-07 conditional;
- return after player/material change: PA-06/09 conditional;
- explanation/legibility: PA-04/11 conditional.

Independence: state change and knowledge of its cause are separate causal sources.

### `disc.residence.shared_private`

- authored hierarchy: public→semi-private→private relation;
- routine/follow: PA-01/02 conditional;
- invitation: PA-03 conditional;
- knowledge/rumour of presence/absence: PA-04/05 conditional.

No omniscient occupancy flag is exposed.

### `disc.fonda.outsider_context`

- authored arrival/lodging layout;
- schedule/arrival state: PA-01 conditional;
- information context: PA-04/05/11 conditional;
- return-after-consequence: PA-06/09 conditional.

The authored substrate remains coherent with no active systemic route.

### `disc.puerto.concession_state`

- authored public/service/private work-institutional surfaces;
- material state: PA-07 conditional;
- governance state: PA-12 conditional;
- knowledge/investigation: PA-04/11 conditional.

The port remains ordinary work/logistics even if every conditional route is absent.

## 6. Multi-route false-green controls

### NC-C-01 — duplicate clue presentation

Mutant: the same hidden boolean produces both a note icon and an NPC barks icon; count them as two routes.

Expected: **FAIL / NOT_A_ROUTE duplication**. One hidden source with two presentations is one route at most, and direct hidden-state exposure is invalid unless later semantics make it legible.

### NC-C-02 — plausible story with no owner

Mutant: “a neighbour eventually tells the player” is counted as a route with no knowledge/social/dialogue owner.

Expected: **FAIL**. Story plausibility is not causal ownership.

### NC-C-03 — future owner treated as implemented

Mutant: PA-12 appears in the accepted plan, therefore a municipal-access-change route is marked `AUTHORED_SPATIAL_NOW`.

Expected: **FAIL**. The plan names a future owner; it does not instantiate runtime governance.

### NC-C-04 — access laundering for route diversity

Mutant: bar service entrance becomes ordinary public access so the discovery has two spatial routes.

Expected: **FAIL**. Route-count goals cannot weaken CITY-05 access-role semantics.

### NC-C-05 — omniscient actor knowledge

Mutant: an NPC can tell the player which object changed because the engine records canonical provenance, even though the NPC never learned it.

Expected: **FAIL**. Engine truth is not actor knowledge.

### NC-C-06 — hidden-room count as discovery quality

Mutant: a seed with five hidden rooms outranks a seed with one hybrid multi-route opportunity by rule.

Expected: **FAIL**. Hidden-room quantity is not a quality metric.

## 7. Quiet-content controls

### NC-Q-01 — quiet paseo clue dispenser

Mutant: every traversal of `loc.vega.quiet_paseo` must spawn a clue or event to satisfy second-layer coverage.

Expected: **FAIL**. Quiet observation/rest is valid content and needs no incident.

### NC-Q-02 — ordinary shed promotion

Mutant: every Puerto shed gets a secret interior because the district needs “discovery density”.

Expected: **FAIL**. `fam.puerto.sheds` remains ordinary C/S1/I0 fabric by default.

## 8. Rare martial/cinema strand controls

### NC-X-01 — mandatory strand

Mutant: seed candidates without martial/cinema content lose acceptance eligibility.

Expected: **FAIL**. Zero opportunities is explicitly valid/neutral.

### NC-X-02 — universal explanation

Mutant: civic authority, port labour and residential secrets all trace back to one hidden kung-fu society.

Expected: **FAIL**. The strand is optional/local/non-load-bearing and may not explain the town's institutions or systemic fabric.

### NC-X-03 — district theme replacement

Mutant: Casco/Puerto/Ensanche each receive separate Hong-Kong-cinema secret kits.

Expected: **FAIL**. This would overwhelm shared grounded setting/production grammar.

## 9. Seed-handoff causality check

CITY-03 is required to report selected discovery opportunities with explicit route statuses. This prevents a seed comparison from inflating readiness by mixing:

- current authored spatial truth;
- future PA/runtime possibilities;
- unsupported narrative ideas.

Selected-seed minimum requires at least one opportunity with one authored spatial route and one genuinely different plausible future route where a named owner exists. That is a **preproduction readiness** criterion only; CITY-04 is not asked to prove the future systemic route in Unity.

## 10. Worker conclusion

`DISCOVERY_CAUSALITY_AUDIT: PASS_FOR_WORKER_PRE_REVIEW`

The candidate has explicit false-green barriers for duplicate routes, unowned story causes, future-owner-as-runtime confusion, omniscient knowledge, access laundering, secret inflation and martial-theme overreach.

This remains Worker evidence. Independent review must challenge the exact frozen candidate and may find additional causal gaps.