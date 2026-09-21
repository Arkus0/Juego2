# WP-CITY-06 — Worker plan

WP: `WP-CITY-06 — Interiors + layered discovery`  
Contract: `Docs/workpacks/CITY/WP-CITY-06.md`  
Baseline SHA: `632a63c089f844313e567046b3df541e435d75d4`  
Branch: `city/wp-city-06-interiors-discovery`  
Active Worker: `ChatGPT GPT-5.6 Sol`  
Worker history: `ChatGPT GPT-5.6 Sol`  
Transfer SHA: `NONE`  
fail_cycle: **0**  
Class: PRODUCT / ENVIRONMENT & DISCOVERY PREPRODUCTION (NON-FOUNDATIONAL)  
Mode: **REMOTE**

This workpack is governed by `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` v1.7. CITY-06 is non-foundational: foundational-proof machinery does not apply. The mandatory predecessor-contract check, Draft Worker state, strict Worker pre-review, exact frozen handoff and fresh independent review do apply.

## 1. PREDECESSOR_CONTRACT_CHECK

### Direct dependency — accepted `WP-CITY-05`

Accepted candidate: `10d1528b0354b16a614fb10a3933a25b32f15f28`  
Independent PASS: review `#5267776704`  
Implementation merge: PR `#83`, merge commit `47909a72eb6d38332f62e9c01426c8cd40e1863b`  
Post-PASS DocSync: PR `#87`, merge commit `cff6d4d0d40786dd1c002a8cc46e768b478ee3cc`  
Residual reconciliation: PR `#88`, merge commit / current baseline `632a63c089f844313e567046b3df541e435d75d4`  
Accepted semantic owner: `Docs/production/CITY_ENVIRONMENT_GRAMMAR.md` v0.2  
Non-canonical requirements projection: `Docs/production/CITY_ENVIRONMENT_DISCOVERY_REQUIREMENTS.json` v0.2

CITY-05 freezes the reusable exterior-production grammar consumed here: street/junction and parcel/site families, reusable shell/building families, composition ladder, A/B exterior composition mappings, `I0..I3` shell-support rules, no-build/rear/view vocabulary, exact fail-closed access-role sets and later authoring-discovery information requirements.

Most importantly, functional-POI binding is monotonic and fail-closed. Every inherited CITY-02 `public`, `service`, `private` and `semi-private` role required by a place is required on the bound exterior composition. `vertical`, `court`, `rear`, `staff`, `storage` and similar descriptors are form/use qualifiers only; CITY-06 may choose how a role is spatially realized, but may not substitute a qualifier for the role itself.

### Transitive binding inputs materially relied on by CITY-06

CITY-06 consumes, without redesigning:

- `Docs/production/CITY_LOCATION_PROGRAMME.md` — accepted CITY-02 programme IDs, `A..D`, `S0..S4`, `I0..I3`, A/B use profiles, interior-priority backlog, public/semi-private/private/service posture and quiet/ordinary scope;
- `Docs/production/CITY_MOBILITY_TOPOLOGY.md` — accepted CITY-01 movement/access graph and route/access semantics; interior paths may connect accepted thresholds but do not create city-graph edges;
- `Docs/production/CITY_SPATIAL_CONSTITUTION.md` — accepted CITY-00 geography, district identity, retained-seed scale band and quiet-fabric invariants;
- `Docs/art/SETTING.md` and `Docs/art/VISUAL_BIBLE.md` — accepted setting/visual authority and grounded Liébana-valley identity;
- `Docs/workpacks/PA/README.md` and its accepted PA programme plan only as an ownership map for future semantic owners. PA research/runtime behaviour is not treated as already implemented or accepted gameplay.

Accepted CITY-02 interior backlog consumed by this WP:

- `I3`: `loc.casco.bar` only;
- `I2`: `loc.plaza.ayuntamiento`, `loc.calle.bakery`, `loc.barrio.residence_cluster`, `loc.ribera.workshop`, `loc.puerto.worker_social`, `loc.entrada.fonda`;
- `I1`: `loc.calle.pharmacy`, `loc.calle.everyday_shop`, `loc.ensanche.neighbourhood_anchor`, `loc.ribera.service_yard`, `loc.puerto.work_hub`, `loc.puerto.warehouse_yard`, `loc.vega.supply_node`;
- all remaining rows remain `I0` unless a later explicitly reviewed owner promotes them.

### Inherited guarantees consumed rather than re-proved

CITY-06 consumes:

- the accepted macro geography and crossing truth from CITY-00;
- the accepted city movement/access graph from CITY-01;
- the 37-row place programme, 23 A/B place profiles, orthogonal A–D/S/I classifications and coarse capacity boundary from CITY-02;
- the explicit CITY-02 rule that `I0` is no enclosed-interior promise even when an asset happens to contain rooms;
- the CITY-02 rule that the casco bar is the only hero-layer `I3` commitment;
- the accepted CITY-05 exterior family/parcel compatibility and one plausible host/site witness for every A/B place;
- the accepted CITY-05 required access-role set for each A/B place;
- the accepted CITY-05 rule that detailed room/threshold/discovery topology belongs here, not upstream;
- the track-wide rule that quiet/ordinary fabric is content and must not become an incident/secret funnel;
- the accepted PA plan's ownership split: later Living World work owns schedules, beliefs/knowledge, relationships, dialogue, decisions, governance semantics, memory/consequences and causal runtime behaviour.

CITY-06 does not duplicate capacity arithmetic, re-derive exterior host fit or pretend planned PA ownership is runtime proof.

### Guarantees newly owned by CITY-06

CITY-06 must freeze one reviewed interior/discovery grammar that later seed selection and realization can consume without inventing content after the seed is chosen:

1. interior-depth allocation rules for `I1..I3` and selective `S2..S4` treatment while preserving `I0` scope control;
2. a compact reusable interior-family vocabulary plus a bounded hero-interior rule rather than bespoke layouts everywhere;
3. room/threshold/access topology patterns that preserve every inherited required exterior access role and keep public/semi-private/private/service meanings distinct;
4. district/location-family discovery-layer expectations so every major district can support meaningful second-layer reading without requiring hidden rooms, roofs or basements everywhere;
5. a precise authored/systemic/hybrid discovery taxonomy with truth/provenance requirements;
6. multiple-route examples where each route is either spatially authored now or explicitly conditional on a real future semantic owner; no systemic discovery may be claimed merely because a plausible story can be imagined;
7. negative-content rules preventing secret inflation, collectible-marker logic, omniscient clues and martial/cinema monoculture;
8. seed-selection constraints handed to CITY-03 so candidates are compared against the reviewed interior/discovery obligations instead of inventing them after boundary selection.

### Concrete conditions that justify reopening inherited guarantees

Reopen only with concrete contradiction:

1. **CITY-05 reopen:** a required access role or interior-support promise cannot be represented by any accepted shell/family/site composition without weakening a required role, changing an exterior family contract or inventing illegal connectivity;
2. **CITY-02 reopen:** an accepted I-depth or required spatial/use programme cannot fit truthfully inside its accepted programme-envelope ceiling, would require consuming hard ordinary/quiet reserve, or the only workable result materially changes the place programme;
3. **CITY-01 reopen:** a claimed interior/service relation requires a new city-graph edge, changes an accepted access class or publicises a restricted service route;
4. **CITY-00 reopen:** the accepted landmass/district/seed-scale constitution makes the required product depth impossible rather than merely inconvenient;
5. **ART reopen:** a genuinely required product function cannot be expressed within accepted setting/visual constraints, not merely because an attractive reference or asset kit conflicts with them.

No predecessor is reopened because a larger hidden room would be more dramatic, because a discovered prefab contains extra rooms, because a cinematic trope is attractive, or because a hypothetical future AI/NPC system might make a discovery route interesting.

## 2. Claim / trust boundary

Claim: within accepted CITY-00 geography, CITY-01 movement/access, CITY-02 place/depth/capacity obligations, CITY-05 shells/access roles and ART identity, CITY-06 can define selective reusable interior and discovery patterns that provide meaningful second layers, preserve scope and ordinary life, and hand CITY-03 reviewed comparison constraints without authoring runtime Living World semantics.

Trusted inputs:

- accepted CITY-00/01/02/05 semantic owners and exact accepted evidence named above;
- ART setting/visual bible within ART authority;
- accepted PA programme only for future ownership labels, never as proof that the later causal systems exist;
- current Worker/Reviewer process contracts.

Outside claim:

- exact retained-seed selection/boundary (`CITY-03`);
- final parcel placement, exact room dimensions or realized metric geometry;
- Unity construction, navmesh, collision, lighting or performance (`CITY-04/07` and H1/later owners);
- final asset/prefab/module inventory or canonical Arkus/Unity catalogue identity;
- runtime schedules/opening hours, invitations/permissions, keys/inventory mechanics, beliefs/knowledge, dialogue, relationships, decisions, incidents, governance, memory or save semantics;
- quest scripting, narrative backstories, collectible systems or minigame mechanics;
- proof that any future systemic/hybrid route actually works at runtime.

## 3. Planned deliverables

Primary semantic owner:

- `Docs/production/CITY_INTERIORS_DISCOVERY.md`

Evidence/process:

- `Docs/evidence/WP-CITY-06/WORKER_PLAN.md`;
- `Docs/evidence/WP-CITY-06/INTERIOR_COVERAGE_AUDIT.md`;
- `Docs/evidence/WP-CITY-06/DISCOVERY_CAUSALITY_AUDIT.md`;
- `Docs/evidence/WP-CITY-06/WORKER_PRE_REVIEW.md`;
- `Docs/evidence/WP-CITY-06/HANDOFF.md`.

`CITY_INTERIORS_DISCOVERY.md` will be the sole semantic owner for CITY-06 interior families, depth allocation, discovery-layer expectations, authored/systemic/hybrid taxonomy, negative-content rules and seed-selection constraints. Evidence audits may recompute coverage but will not redefine the product rules.

## 4. Execution sequence

1. Normalize inherited I1/I2/I3 obligations and required access roles into an interior-topology input ledger.
2. Define reusable interior families and threshold/zone patterns whose variants remain bounded by use/access rather than bespoke narrative identity.
3. Allocate the full I1–I3 backlog and selected exterior-led S2/S3 second-layer places without promoting I0 interiors.
4. Define authored/systemic/hybrid discovery with explicit truth/provenance and future-owner conditions.
5. Build a district/location discovery matrix that proves every major district has a meaningful second layer while preserving quiet/ordinary places and avoiding a universal-secret rule.
6. Define multiple-route examples for important discoveries; mark every route as `AUTHORED_SPATIAL_NOW` or `FUTURE_OWNER_CONDITIONAL` and name the future owner category when conditional.
7. Define the rare optional martial/cinema strand as non-load-bearing identity material with strict frequency, provenance and coherence constraints.
8. Produce exact seed-selection constraints for CITY-03: minimum mixes, hero selectivity, reusable-family yield, route diversity, expansion seams and discovery-opportunity conditions.
9. Run coverage and causality audits plus strict baseline→candidate Worker pre-review.
10. Repair any in-claim defect while Draft, then freeze exact HEAD and hand off to a fresh independent Reviewer.

## 5. Scope guard

Allowed: CITY-06 product-preproduction Markdown and bounded evidence required to establish interior/discovery coverage, reuse, causality boundaries and the CITY-03 handoff.

Forbidden: changing accepted CITY geography/routes/programme/exterior grammar; selecting the seed; Unity/runtime implementation; making PA research/runtime claims true by declaration; authoring quest/dialogue/NPC schedules; inventing keys/inventory mechanics as required gameplay; asset import/catalogue authority; expanding every I0 shell into an interior; or using martial/cinema material as the town's universal explanation.

If a required interior/discovery promise cannot fit inherited shell/access/capacity constraints, the Worker records the correct predecessor reopen signal instead of weakening the inherited contract.