# WP-CITY-03 — Strict Worker pre-review

WP: `WP-CITY-03 — Retained product seed + exact scenario specification`  
Contract: `Docs/workpacks/CITY/WP-CITY-03.md`  
Baseline SHA: `1b503cfabeca340c42055aaf31df90d72ca28e68`  
PR: `#94`  
Pre-review semantic candidate HEAD before this report: `2a77f15a59e23b86df55180e7701bb06111b594d`  
Worker: `ChatGPT GPT-5.6 Sol`  
fail_cycle: **0**

`WORKER_PRE_REVIEW: CLEAN`  
`WORKER_PRE_REVIEW_FINDINGS_FIXED: 3`  
`WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-CITY-03/WORKER_PRE_REVIEW.md`

This is Worker quality-gate evidence only. It is not an independent PASS and does not authorize merge.

## 1. Surfaces reviewed

The strict challenge re-read and cross-checked:

- `Docs/workpacks/CITY/WP-CITY-03.md` and CITY execution spine;
- accepted `CITY_SPATIAL_CONSTITUTION.md` (CITY-00);
- accepted `CITY_MOBILITY_TOPOLOGY.md` (CITY-01);
- accepted `CITY_LOCATION_PROGRAMME.md` (CITY-02);
- accepted `CITY_ENVIRONMENT_GRAMMAR.md` (CITY-05);
- accepted `CITY_INTERIORS_DISCOVERY.md` (CITY-06 direct predecessor);
- `Docs/workpacks/CITY/WP-CITY-04.md` as downstream local consumer;
- ART authority boundary and non-binding production blueprint only where compatible;
- `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` v1.7;
- complete baseline→semantic-candidate diff before this report:
  - `Docs/production/CITY_PRODUCT_SEED.md`;
  - `Docs/evidence/WP-CITY-03/WORKER_PLAN.md`;
  - `Docs/evidence/WP-CITY-03/SEED_COMPARISON.md`;
  - `Docs/evidence/WP-CITY-03/BOUNDARY_AND_HANDOFF_AUDIT.md`.

No code, Unity, asset, runtime, bridge, PA implementation or accepted predecessor file is changed.

## 2. Predecessor-contract check

`WORKER_PLAN.md` reconstructs the direct accepted CITY-06 dependency:

- accepted candidate `c73cbb19f8b152bd4b97ce3154eb18a78deaec3c`;
- independent PASS review `#5268249668`;
- implementation merge PR #90 / `acb84ec9ba7aa94e962bf7b35d7e53c0549ba758`;
- DocSync PR #93 / Worker baseline `1b503cfabeca340c42055aaf31df90d72ca28e68`.

It correctly consumes rather than re-proves CITY-00 geography/seed band, CITY-01 edge/access semantics, CITY-02 place/depth/capacity, CITY-05 exterior/access-role conformance and CITY-06 interior/discovery grammar. Concrete reopen conditions are named for each causal owner.

`PREDECESSOR_CONTRACT_CHECK: VALID`

## 3. Findings discovered and repaired before freeze

### Finding 1 — unsupported “net land” precision risked becoming a second area oracle

The first draft reported the exact hard-envelope shoelace area **and** an approximate net land/bridge area after unspecified water/no-build subtraction.

That second number was unnecessary and not mechanically reproducible because CITY-03 had not frozen metric water-mask polygons. More importantly, CITY-00 delegates one `0.03–0.06 km²` retained-seed band; inventing a second net-area acceptance interpretation would create false precision.

Repair:

- removed the approximate net-area claim from the semantic owner and comparison;
- made the hard planning envelope the explicit band metric;
- retained water/no-build masks only as non-traversable semantic exclusions;
- audit recomputes exact envelope area `44,817.5 m² = 0.0448175 km²`.

Disposition: **FIXED at CITY-03 measurement-definition boundary; no predecessor changed.**

### Finding 2 — frontage names without bounded placement regions left too much city design to CITY-04

The first draft froze eight frontage identities but not their exact bounded placement regions. A local Worker would still have had to decide where the bar, civic shell, shop and ordinary frontage mass belonged inside the seed, which weakens the DoD requirement that CITY-04 build rather than reselect layout.

Repair:

- added exact seed-local polygons for F01–F08 and open/exterior sites S01–S03;
- bound each slot to its accepted CITY-05 family/parcel posture and facing/access relation;
- mechanically checked all 11 regions lie inside the hard seed polygon;
- checked F01–F08 planning dimensions remain inside the inherited parcel-family bands;
- stated that CITY-04 may shape legal geometry **inside** a region but may not move functional slots between streets or exchange identities.

Disposition: **FIXED at CITY-03 exact-handoff boundary.**

### Finding 3 — home/work/social coverage was too implicit

The initial scenario pack had a home-threshold → practical shop → bar route. That demonstrated ordinary life but did not explicitly state how it satisfied the WP's requested `NPC home/work/social trip` coverage.

Repair:

- SCN-03 now explicitly uses F04 closed home threshold → F03 everyday-shop work/service threshold → F01 bar social entrance;
- it remains a spatial proxy only: no schedule, job assignment, ownership, opening hours or NPC runtime is asserted;
- F04 remains I0/closed and the proxy starts/ends at the threshold rather than inventing a home interior.

Disposition: **FIXED at scenario-specification boundary; PA/runtime ownership unchanged.**

## 4. Contract acceptance challenge

### A. Three genuine alternatives before selection

`SEED_COMPARISON.md` compares:

1. compact Casco/civic seed (~0.038 km²);
2. selected civic-commercial seed (0.0448175 km²);
3. work-edge seed (~0.0547 km²).

They differ materially in I-depth, ordinary commercial/work content, family yield, scenario breadth, seam position and local cost. The comparison is not three cosmetic boundary nudges around the same content.

Result: **CLEAN**.

### B. Exact band and boundary

Hard polygon area recomputes to `0.0448175 km²`, inside accepted `0.03–0.06 km²`.

Point-in-polygon checks:

- selected anchors: 8/8 inside;
- frontage/open-site regions: 11/11 fully inside.

No unsupported net-area oracle remains.

Result: **CLEAN**.

### C. Geography and crossing truth

Selected playable crossings are exactly:

- X1 across Río;
- X5 across Arroyo, still low-water-only.

W.LANDING is only a future X6/X7 socket. No Puerto traversal is built and no dry confluence continuation exists. There is no Ensanche↔Orilla-sur edge, no eighth crossing and no water-mask shortcut.

Result: **CLEAN**.

### D. Route choice without graph mutation

The seed carries accepted W04/W05/W06/W12/W13 plus X1/X5. For the required follow/search local choice, `casco.micro.A/B` split/reconnect **inside W.CASCO** and are explicitly excluded from CITY-01 route-redundancy claims.

The semi-private shared court cannot be used as an alternate. Blocking micro.A falls back only to public micro.B. X5 closure creates no invented substitute crossing.

This is detailed node realization owned by the seed/blockout boundary, not a new city-graph edge.

Result: **CLEAN**.

### E. Place/depth mix and hero selectivity

Hard seed contains:

- A/B/C systemic classes;
- S1/S2/S3/S4;
- I0/I1/I2/I3;
- bar I3 only;
- ayuntamiento I2;
- everyday shop I1;
- exterior-led market/shared court/bridgehead;
- ordinary closed C/S1 fabric.

D/S0 is correctly left as soft scenic envelope rather than forced into playable area.

Result: **CLEAN**.

### F. CITY-05 access-role conformance

Selected bindings preserve:

- bar `{public,service,semi-private}`;
- ayuntamiento `{public,service,private}`;
- everyday shop `{public,service}`;
- market/bridgehead public;
- shared court public passage + semi-private court.

No `vertical`, `court`, `rear`, `staff` or other form qualifier substitutes for a role. I1 shallowness does not delete shop service.

Result: **CLEAN**.

### G. CITY-06 minimum depth/discovery mix

Witnesses:

- deep interior: bar I3 and ayuntamiento I2;
- shallow/second layer: shop I1 plus market/shared court;
- ordinary/quiet no-secret content: W13/S03 and ordinary closed frontages;
- multi-route-ready discovery: bar or ayuntamiento has one authored-spatial route plus named future-owner routes;
- no extra I3;
- extraordinary/cultural strand = 0, explicitly neutral.

Future routes remain `FUTURE_OWNER_CONDITIONAL`, never runtime proof.

Result: **CLEAN**.

### H. Required scenario pack

Coverage audit maps all contract motifs:

- quiet morning → SCN-01;
- market/commercial → SCN-02;
- home/work/social trip → SCN-03;
- materially different ordinary trip → SCN-04;
- follow/search choice → SCN-05;
- bar/social spatial support → SCN-06;
- bridge traversal → SCN-07;
- material delivery/work consequence → SCN-08;
- municipal access/service consequence → SCN-09;
- low-stakes perturbation → SCN-10;
- leave/return changed state → SCN-11;
- truthful multi-route-ready discovery → SCN-12;
- blocked route/alternate → SCN-13.

Every runtime-dependent case is a spatial proxy/readiness setup and names the later causal boundary rather than simulating it by declaration.

Result: **CLEAN**.

### I. At least two materially different everyday actor trips

SCN-03 and SCN-04 differ materially:

- SCN-03 begins at a residential threshold and uses commercial/service/social places;
- SCN-04 begins across the Río, depends on X1/AH crossing, uses old-bridge approach and ends at civic/market space.

They are not one path with a different label.

Result: **CLEAN**.

### J. Expansion seams

Five named seams are frozen: commercial/Vega, Ensanche, direct Puerto landing, Orilla-sur/Entrada via X1, and upper/Barrio future continuity.

None requires moving W.PLAZA, W.CASCO, X1, accepted core edges or F01–F03.

Result: **CLEAN**.

### K. CITY-04 measurement handoff

Only fully represented accepted edges are assigned direct measurement targets. Planning sums recompute correctly:

- X1 + W12 + W05 = 3.75 min;
- W04 + W05 + W06 = 4.50 min;
- W13 + X5 = 1.40 min when X5 is available.

Absent full-city routes are explicitly forbidden from being relabelled as measured through extrapolation.

CITY-04 also receives physical/access/sightline checks and owner-tagged failure routing.

Result: **CLEAN**.

### L. Retained versus temporary boundary

The candidate distinguishes retained topology/site/access/depth/seam decisions from temporary primitives, materials, props, proxy actors, diagnostics, soft-envelope meshes and pre-acceptance nav/collision.

Unity existence cannot turn a placeholder into keeper authority.

Result: **CLEAN**.

### M. CITY-08 later authoring slice

`trial.city08.civic_commercial_corner` is bounded inside the retained seed and deliberately non-hero: W04/plaza edge + F03 functional shop + F07 ordinary closed frontage + S01 open-site market edge.

It tests reusable authoring distinctions rather than hiding proof inside bespoke bar content. CITY-08 still owns actual proof.

Result: **CLEAN**.

## 5. Negative/error challenge

The candidate/evidence explicitly rejects:

- dropping X1;
- making X5 permanent;
- dry W.LANDING→Puerto continuation;
- publicising S03 shared court;
- deleting F03 service role;
- opening F04–F08 I0 fabric for convenience;
- counting future PA semantics as implemented;
- moving F01/F02/F03 in local blockout;
- adding another I3 hero;
- treating soft-envelope paths as playable;
- measuring absent full-city geometry as if present.

No negative control requires arbitrary behaviour outside the declared claim.

## 6. Residuals / deliberately deferred falsification

Not blockers for this REMOTE WP:

- target coordinates are planning geometry; actual grade, width, bank profile, sightline and travel time remain CITY-04 empirical questions;
- Casco micro.A/B topology is frozen as a public split/rejoin inside one node, while exact bends/width realization remains greybox geometry — demanding final surveyed polylines here would pre-empt the local falsification owner rather than close a semantic gap;
- final asset/module choice and exact shell footprint inside each bounded site region remain later production/Unity work;
- future discovery/material/governance/memory routes remain conditional until their runtime owners exist.

These residuals are outside the CITY-03 planning claim and already have downstream owners.

## 7. Proof-budget / scope check

CITY-03 is non-foundational. No foundational proof budget is required. Support evidence consists of one predecessor/claim plan, one comparison, one bounded mechanical handoff audit and this pre-review. No duplicate proof framework or runtime test harness was introduced.

`PROOF_BUDGET_VERDICT: NOT_APPLICABLE_NON_FOUNDATIONAL`

## 8. Final Worker pre-review verdict

`WORKER_PRE_REVIEW: CLEAN`  
`WORKER_PRE_REVIEW_FINDINGS_FIXED: 3`  
`KNOWN_IN_CLAIM_BLOCKERS: 0`

The semantic candidate is ready for final handoff metadata and exact-SHA freeze. Independent Reviewer must reconstruct and challenge the frozen candidate rather than trust this report.