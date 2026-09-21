# WP-CITY-01 — Worker → independent Reviewer handoff

WP: `WP-CITY-01 — Mobility, district graph + walk-time topology`  
Contract: `Docs/workpacks/CITY/WP-CITY-01.md`  
PR: **#73**  
Baseline SHA: `7fe44840076eba05f1b67a7633cd33fc67b9023d`  
Active Worker: `ChatGPT GPT-5.6 Sol`  
Worker history: `ChatGPT GPT-5.6 Sol`  
Transfer SHA: `NONE`  
fail_cycle: **1**

Cycle 0 frozen candidate `2fac578973cf3686d264b57c8ba7519b8ad507b2` received independent Reviewer **FAIL** in review `#5263730661`. The branch was returned to Draft before mutation. This handoff supersedes the cycle-0 freeze.

This file is the Worker's final branch mutation before the cycle-1 freeze. The exact 40-character commit containing this handoff is read from PR #73 immediately after this commit and recorded in PR metadata as both `Candidate HEAD SHA` and `Frozen candidate SHA`. PR metadata is the authoritative exact-SHA freeze record.

## Worker state at handoff

```text
Predecessor contract check: Docs/evidence/WP-CITY-01/WORKER_PLAN.md
Worker pre-review: CLEAN
Worker pre-review evidence: Docs/evidence/WP-CITY-01/WORKER_PRE_REVIEW.md
Original graph/scenario audit: Docs/evidence/WP-CITY-01/GRAPH_AUDIT.md
Cycle-1 affected audit: Docs/evidence/WP-CITY-01/PROFILE_CROSSING_AUDIT.md
Semantic owner: Docs/production/CITY_MOBILITY_TOPOLOGY.md v1.3
Branch frozen after this commit: YES
Worker verdict: IN_REVIEW after PR metadata freeze
Reviewer verdict: PENDING
Reviewed candidate SHA: NONE
```

## Cycle-1 causal repair

The failed candidate left water-crossing profile semantics underdetermined. The repair is local:

- every X1..X7 crossing is explicitly `EW` in the authoritative ledger;
- slower pedestrian applies ×1.35 to EW **motion**;
- X6 wait remains a separate additive term and is never multiplied;
- bicycle rideability is derived from `EW + Access`:
  - X2/X4/X7 = rideable `AR` crossings;
  - X1 = push-only;
  - X3/X5 = inaccessible `AF` crossings;
  - X6 = bicycle carriage not assumed;
- bicycle comparative planning uses ledger base movement weights, with no invented bicycle speed advantage.

The affected audit recomputes X1..X7 slower-pedestrian costs and the X2-versus-X4 bicycle route comparison.

## Regression boundary

The repair does **not** change:

- CITY-00 landmasses or crossing endpoints;
- State-1/State-2 availability;
- ordinary-pedestrian base costs or representative route matrix;
- Plaza-removal proof;
- X2 municipal-closure ownership;
- X6/X7 freight consequences;
- CITY-03 seed ownership or CITY-04 bounded measurement scope;
- CITY-02, Unity/runtime, H0/H1 or ART semantics.

## Fresh Reviewer challenge points

1. Verify that all X1..X7 rows carry `EW` and no internal edge is accidentally reclassified.
2. Recompute slower-pedestrian X6 as `1.35 + W_ferry`, not `1.35 × (1 + W_ferry)`.
3. Derive bicycle results from `EW + Access` without relying on prose.
4. Recompute Ensanche→Calle Mayor: X2 route weight 3.5 versus available X4 road alternative 7.3.
5. Confirm the repair did not alter ordinary route-cost arithmetic, availability, topology or downstream measurement authority.
6. Independently re-check the original CITY-01 contract rather than assuming the cycle-0 non-blocked areas remain valid.

## Known residuals

- actual measured slower-pedestrian factors;
- realized bicycle comfort/clearance/dismount behaviour;
- final metric geometry and grades;
- full-city measurements outside the retained seed;
- runtime route-choice AI and schedules;
- State 1→State 2 production timing.

These remain downstream empirical/runtime questions, not missing planning semantics.

## Worker stop condition

After PR #73 is updated with the exact SHA of this commit and marked Ready, the Worker stops writing. Any fresh Reviewer FAIL requires another repair cycle and a new exact-SHA freeze.
