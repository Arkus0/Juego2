# WP-CITY-05 — Worker plan

WP: `WP-CITY-05 — Streets, parcels + reusable building families`  
Contract: `Docs/workpacks/CITY/WP-CITY-05.md`  
Baseline SHA: `9a8ff542d0a0ce70f9a6d5fa3baea2f8a5691cbb`  
Branch: `city/wp-city-05-environment-grammar`  
Active Worker: `ChatGPT GPT-5.6 Sol`  
Worker history: `ChatGPT GPT-5.6 Sol`  
Transfer SHA: `NONE`  
fail_cycle: **0**  
Class: PRODUCT / ENVIRONMENT PREPRODUCTION (NON-FOUNDATIONAL)  
Mode: **REMOTE**

This workpack is governed by `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` v1.7. CITY-05 is non-foundational: foundational-proof machinery does not apply. The mandatory predecessor-contract check, Draft Worker state, strict Worker pre-review, exact frozen handoff and fresh independent review do apply.

## 1. PREDECESSOR_CONTRACT_CHECK

### Direct dependency — accepted `WP-CITY-02`

Accepted candidate: `ddcb22d9a2dfcf68054db2b762fc7ff7aed68ae8`  
Independent PASS: review `#5266113192`  
Implementation merge: PR `#81`, merge commit `1bdb7b6e914692493b17a9d2215d881dfe326cb0`  
Post-PASS DocSync: PR `#82`, merge commit / current baseline `9a8ff542d0a0ce70f9a6d5fa3baea2f8a5691cbb`  
Accepted semantic owner: `Docs/production/CITY_LOCATION_PROGRAMME.md` v1.0  
Accepted capacity evidence: `Docs/evidence/WP-CITY-02/CAPACITY_SANITY.md`

The independent PASS explicitly accepted the repaired Q6 boundary: CITY-02 owns coarse programme capacity through role-based `T/S/M/L` programme-envelope ceilings, per-part A/B caps, hard ordinary/quiet reserves and an additional 15% uncommitted/circulation margin. CITY-05 owns actual street/parcel/shell composition and must reopen Q6 if real composition shows a class ceiling is untruthful or a reserve/cap would be consumed.

### Transitive binding inputs materially relied on by CITY-05

CITY-05 consumes, without redesigning:

- `Docs/production/CITY_SPATIAL_CONSTITUTION.md` — accepted CITY-00 geography, district identities, landmasses, port scale/character, scale envelope and quiet-fabric invariants;
- `Docs/production/CITY_MOBILITY_TOPOLOGY.md` — accepted CITY-01 route graph, route-character tags, elevation/access classes, crossing truth and closure semantics;
- `Docs/art/SETTING.md` + `Docs/art/VISUAL_BIBLE.md` — fictional Potes/Liébana visual identity, material language, scale cues and explicit exclusions;
- `Docs/production/CITY_LOCATION_PROGRAMME.md` — accepted programme IDs, A–D/S0–S4/I0–I3 obligations, frontage/access posture, ordinary/scenic scope and downstream ownership;
- `Docs/evidence/WP-CITY-02/CAPACITY_SANITY.md` — accepted T/S/M/L programme-envelope ceilings and district-part reserve/cap arithmetic.

### Inherited guarantees consumed by CITY-05

CITY-05 consumes rather than re-proves:

- Wedge, Ensanche bank and Orilla-sur geography; the Wedge ends at the confluence;
- Puerto/Entrada are on Orilla sur; every inter-landmass edge is one of `X1..X7`; there is no dry Wedge→Puerto continuation or Ensanche↔Orilla-sur crossing;
- the authoritative route/access graph and its `P/S/Q/V`, `E0..E3/EW`, `AR/AP/AF/AH/AS/AX6/BUS` meanings;
- route-cost numbers are planning hypotheses, not measured traversal;
- protected quiet fabric remains meaningful and low intensity rather than becoming an incident/content funnel;
- the accepted district identities and visual/material exclusions;
- the 37-row CITY-02 programme, 23 A/B place profiles and explicit C/D + S0/S1 ordinary/scenic scope;
- A–D systemic importance, S0–S4 spatial depth and I0–I3 interior priority are separate dimensions;
- CITY-02's coarse-capacity result is conditional, fail-closed and not permission to spend the ordinary/quiet reserve.

CITY-05 performs compatibility and fit checks against these guarantees. It does not attempt to re-establish the predecessor graph, programme or Q6 arithmetic as if they were newly owned claims.

### Guarantees newly owned by CITY-05

CITY-05 must freeze one reviewed exterior-production grammar that gives later work constrained composition choices rather than blank-slate design:

1. reusable street-segment and junction families covering ordinary, historic/narrow, stepped, riverside, service, plaza/market, port/work and rural-transition conditions;
2. parcel/site constraints for frontage, depth/bounds, access, party-wall/detached relations, slope/retaining, no-build/view corridors and rear/service relations;
3. a compact reusable building-family catalogue covering ordinary houses, mixed-use house/shop, bar/social, shop/service, workshop, warehouse/port, civic/municipal, apartment/residential and selected rural/peripheral types;
4. one composition ladder: `module -> assembly -> shell -> reusable building -> functional POI -> street segment`;
5. an explicit mapping from accepted CITY-02 programme/access/interior obligations to shell/access requirements without authoring runtime behaviour or CITY-06 room topology;
6. machine-readable discovery requirements for stable ID, bounds, sockets, access anchors, tags/archetype, dependencies, variants, style/material constraints and compatibility/version metadata;
7. a reviewed promotion rule for turning a one-off composition into a reusable family/variant;
8. a fit/capacity reconciliation showing that the proposed grammar can realize each A/B programme role inside its accepted `T/S/M/L` ceiling without consuming protected ordinary/quiet reserve.

### Concrete conditions that would justify reopening inherited guarantees

Reopen the relevant predecessor only with concrete evidence that satisfying CITY-05's own contract requires one of the following:

1. **CITY-02 Q6 reopen:** an A/B place cannot truthfully be composed inside its accepted T/S/M/L ceiling, a dense-part A/B cap would be breached, a hard ordinary/quiet reserve would need to be consumed, or shared-route/circulation area would need to be double-counted as exclusive programme envelope;
2. **CITY-01 reopen:** a required ordinary/service access relation cannot be realized without changing the accepted route/access class, creating a new edge/crossing or silently publicising an `AS` route;
3. **CITY-00 reopen:** the accepted landmass/district/scale geometry makes the required exterior grammar impossible without contradicting the confluence/port topology or named quiet invariants;
4. **ART reopen:** the required product roles genuinely cannot be expressed within the accepted visual/material identity rather than merely being inconvenient for a chosen kit.

A preference for larger hero buildings, easier kit assembly, more open interiors, wider roads, denser spectacle or asset-pack convenience is not sufficient to reopen accepted predecessors.

## 2. Claim / trust boundary

Claim: within accepted CITY-00 geography, CITY-01 access topology, CITY-02 programme/capacity bounds and ART direction, CITY-05 can define a compact reusable exterior grammar that covers every required district/place family, preserves ordinary/quiet fabric, and gives later CITY-06/Arkus authoring constrained reviewed choices rather than bespoke building invention.

Trusted inputs:

- accepted CITY-00/01/02 semantic owners and evidence named above;
- ART setting/visual bible within ART authority;
- `Docs/workpacks/CITY/README.md` and the accepted CITY Programme v2;
- current engineering process contracts for Worker/Reviewer evidence only.

Outside claim:

- final exact parcel map or citywide cadastral census;
- exact retained-seed selection (`CITY-03`);
- Unity/metric blockout, navmesh, measured traversal or realized geometry (`CITY-04`);
- detailed room layouts, secret routes, keys, discovery mechanics or threshold choreography (`CITY-06`);
- prefab/asset import, catalogue authority, Unity scenes or Arkus implementation;
- runtime schedules, beliefs, dialogue, decisions, incidents, interactions or off-screen simulation;
- final canonical runtime IDs or a canonical authoring contract.

## 3. Planned deliverables

Primary semantic owner:

- `Docs/production/CITY_ENVIRONMENT_GRAMMAR.md`

Evidence/process:

- `Docs/evidence/WP-CITY-05/WORKER_PLAN.md`;
- `Docs/evidence/WP-CITY-05/GRAMMAR_AUDIT.md`;
- `Docs/evidence/WP-CITY-05/CAPACITY_FIT_CHECK.md`;
- `Docs/evidence/WP-CITY-05/WORKER_PRE_REVIEW.md`;
- `Docs/evidence/WP-CITY-05/HANDOFF.md`.

`CITY_ENVIRONMENT_GRAMMAR.md` will be the sole semantic owner for CITY-05 family IDs, composition rules, parcel classes, shell/access obligations, discovery fields and promotion semantics. Evidence files may audit those facts but will not redefine them.

## 4. Execution sequence

1. Define a bounded street/junction vocabulary tied to accepted CITY-01 route/access classes without changing them.
2. Define parcel archetypes and constraint fields, including shared-street versus exclusive-envelope accounting.
3. Define reusable module/assembly/shell/building-family catalogue with district-compatible variants instead of disconnected district kits.
4. Map every CITY-02 A/B role to allowed shell/access/family patterns and verify I0–I3 obligations are represented as exterior/access requirements only.
5. Perform class-by-class and district-part capacity fit checks against accepted T/S/M/L ceilings, caps, hard reserves and 15% circulation margin.
6. Define machine-readable discovery requirements and a one-off→variant/family promotion rule without creating an Arkus canonical contract.
7. Audit visual identity, quiet-fabric protection, port ordinary-work character, route/access compatibility, reuse coverage and anti-bespoke acceptance.
8. Run strict baseline→candidate Worker pre-review and repair any in-claim defect while Draft.
9. Freeze exact HEAD, record PR metadata and hand off to a fresh independent Reviewer.

## 5. Scope guard

Allowed: CITY-05 environment-preproduction Markdown and bounded evidence required to establish the grammar/fits claimed by this WP.

Forbidden: changing accepted geography/routes/programme; inventing new A/B places to rescue a family catalogue; consuming quiet/ordinary reserve; detailed CITY-06 interiors/discovery; CITY-03 seed selection; CITY-04 Unity geometry/measurement; asset import/prefabs/catalogue authority; runtime/gameplay semantics; H0/H1 contract changes; ART restyling.

If a required composition cannot fit inside an inherited capacity/access boundary, the Worker records the predecessor reopen condition instead of patching around it.