# WP-CITY-01 — Worker → independent Reviewer handoff

WP: `WP-CITY-01 — Mobility, district graph + walk-time topology`  
Contract: `Docs/workpacks/CITY/WP-CITY-01.md`  
PR: **#73**  
Baseline SHA: `7fe44840076eba05f1b67a7633cd33fc67b9023d`  
Active Worker: `ChatGPT GPT-5.6 Sol`  
Worker history: `ChatGPT GPT-5.6 Sol`  
Transfer SHA: `NONE`  
fail_cycle: **0**

This file is the Worker's final branch mutation before freeze. The exact 40-character commit containing this handoff is read from PR #73 immediately after this commit and recorded there as both `Candidate HEAD SHA` and `Frozen candidate SHA`. The PR metadata is the authoritative exact-SHA freeze record.

## Worker state at handoff

```text
Predecessor contract check: Docs/evidence/WP-CITY-01/WORKER_PLAN.md
Worker pre-review: CLEAN
Worker pre-review evidence: Docs/evidence/WP-CITY-01/WORKER_PRE_REVIEW.md
Graph/scenario audit: Docs/evidence/WP-CITY-01/GRAPH_AUDIT.md
Semantic owner: Docs/production/CITY_MOBILITY_TOPOLOGY.md
Branch frozen after this commit: YES
Worker verdict: IN_REVIEW after PR metadata freeze
Reviewer verdict: PENDING
Reviewed candidate SHA: NONE
```

## What the candidate owns

- one authoritative semantic route/access edge ledger derived inside accepted CITY-00 geography;
- primary/secondary/quiet/service-capable route character and restricted service access;
- elevation/access classes;
- deterministic spatial assumptions for ordinary/slower pedestrian, bicycle, service/delivery, porter, arrival/bus, following/search and time-sensitive profiles;
- planning route-cost bands, including CITY-00-deferred Ensanche→Puerto and Vega↔Puerto costs;
- meaningful loops and alternate routes;
- X2 as the practical municipal Arroyo closure lever;
- L1/L1′ State-1/State-2 traffic character;
- all mandatory scenario cases;
- a bounded CITY-03/CITY-04 measurement handoff that distinguishes measured route, calibrated component and route absent from seed.

## What the candidate deliberately does not own

- any CITY-00 landmass/crossing change;
- final metric geometry or measured full-city traversal;
- CITY-02 systemic location/interior programme;
- CITY-03 exact retained seed;
- Unity/navmesh/assets;
- runtime AI/schedules/vehicle simulation;
- full-city measurement ownership after the bounded CITY-04 seed when geometry is absent;
- H0/H1 or ART semantics.

## High-value Reviewer challenge points

The Worker pre-review already repaired six findings, but the Reviewer should reconstruct independently rather than trusting this list. Particularly worth challenging:

1. whether every inter-landmass relation really decomposes only through X1..X7;
2. whether Plaza removal remains connected without restricted W17;
3. whether W11 direction-sensitive arithmetic is consistent everywhere;
4. whether profile access accidentally creates cart/bicycle capacity across a forbidden crossing;
5. whether X2 closure has meaningful cost without deadlocking ordinary pedestrian movement;
6. whether required follow/search and quiet-vs-market scenarios are genuinely different routes rather than prose labels;
7. whether `V` route character versus `AS` access restriction is consistently applied;
8. whether port↔workshop/commercial and rural/valley-arrival routes remain ordinary movement rather than mission-only exceptions;
9. whether the measurement handoff respects CITY-03 exact-seed ownership and CITY-04's “build only the bounded seed” contract;
10. whether any planning cost is accidentally represented as a measurement.

## Known residuals

- final metres/grades/stair counts and physical clearances;
- real slower-ped/bicycle behaviour;
- realized readability/followability and closure penalties;
- service-cart clearance where geometry is eventually built;
- quiet/commercial route near-parity after greybox;
- full-city end-to-end measurement for routes outside the retained seed;
- State 1→State 2 timing.

These are residuals or later measurements, not claims of this WP.

## Concurrent-main note

The Worker started from then-current main `7fe44840076eba05f1b67a7633cd33fc67b9023d`. During execution, unrelated H1 planning DocSync advanced main to `b02f9dfdc0fae1e38f66d7a527584a2ea80a055f`. That change does not modify CITY-00, WP-CITY-01 or CITY production/evidence dependencies, and PR #73 remained mergeable at pre-review. The candidate was not semantically rebased merely to absorb unrelated H1 documentation.

## Worker stop condition

After PR #73 is updated with the exact SHA of this commit and marked Ready, this Worker stops writing. Any Reviewer FAIL requires a fresh repair cycle and new candidate SHA. This Worker does not act as the independent Reviewer of its own candidate.
