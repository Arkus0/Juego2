# WP-CITY-02 — Worker → independent Reviewer handoff

WP: `WP-CITY-02 — Systemic locations + spatial-depth/interior programme`  
Contract: `Docs/workpacks/CITY/WP-CITY-02.md`  
PR: **#81**  
Baseline SHA: `0b8f23fbce227bbbc710c8410dff575fcb9fcf12`  
Active Worker: `ChatGPT GPT-5.6 Sol`  
Worker history: `ChatGPT GPT-5.6 Sol`  
Transfer SHA: `NONE`  
fail_cycle: **0**

This file is the Worker's final branch mutation before the cycle-0 freeze. The exact 40-character commit containing this handoff is read from PR #81 immediately after this commit and recorded in PR metadata as both `Candidate HEAD SHA` and `Frozen candidate SHA`. PR metadata is the authoritative exact-SHA freeze record.

## Worker state at handoff

```text
Predecessor contract check: Docs/evidence/WP-CITY-02/WORKER_PLAN.md
Worker pre-review: CLEAN
Worker pre-review findings fixed: 1
Worker pre-review evidence: Docs/evidence/WP-CITY-02/WORKER_PRE_REVIEW.md
Programme audit: Docs/evidence/WP-CITY-02/PROGRAMME_AUDIT.md
Capacity sanity: Docs/evidence/WP-CITY-02/CAPACITY_SANITY.md
Semantic owner: Docs/production/CITY_LOCATION_PROGRAMME.md v1.0
Branch frozen after this commit: YES
Worker verdict: IN_REVIEW after PR metadata freeze
Reviewer verdict: PENDING
Reviewed candidate SHA: NONE
```

## What CITY-02 freezes

The candidate gives downstream CITY work one semantic owner for:

- place/access vocabulary: place, frontage, shell, threshold, public/semi-private/private, service access, vertical relation, interior anchor and scenic envelope;
- a 37-row district × location/family programme covering every required product domain;
- independent A–D systemic importance and S0–S4 spatial-production depth;
- 23 A/B profiles with users/time-demand bands, activity/material role, witness potential, governance/access hook, interior need, change potential and player-absence function;
- an unambiguous interior backlog: I0=23, I1=7, I2=6, I3=1;
- one hero interior commitment only: `loc.casco.bar`;
- explicit quiet/ordinary/scenic fabric commitments;
- reactive-density families `MDT`, `RUR`, `RCD`, `QSB` and `DD` for later measurement;
- CITY-00 Q6/Q7/Q8 programme-level closure without claiming exact parcel geometry.

## Independence / scope highlights

A–D and S0–S4 are intentionally orthogonal. Direct witnesses include:

- `loc.entrada.arrival` = A/S1;
- `loc.ensanche.neighbourhood_anchor` = A/S2;
- `loc.ribera.workshop` = A/S3;
- `loc.casco.bar` = A/S4;
- B examples at S1/S2/S3;
- `loc.casco.shared_court` = C/S3;
- ordinary C/S1 and scenic D/S0 fabric.

The old/civic core does not monopolise A: Casco + Plaza own 2/8 A anchors. Puerto has ordinary work, logistics and social roles and no S4 hero place.

## Pre-review repair

The Worker found one material ambiguity before freeze: `loc.ribera.service_yard` was written as `I0/I1` in the master ledger/profile while the interior ledger assigned I1. Commit `e6fe45d94701d4b2cec0cada6f6996287fac3874` repaired both authoritative references to **I1**. The programme audit was then reconciled.

No known second in-claim blocker remains.

## Capacity / quiet-fabric boundary

`CAPACITY_SANITY.md` is deliberately bounded. It uses the accepted CITY-00 dimensional sketch only to detect an obvious programme-pressure contradiction:

- 21 A/B handles in the ~0.365 km² dense-fabric subtotal;
- crude average ~17,380 m² of dense fabric per A/B handle;
- 2 additional A/B handles in Vega outside that subtotal;
- only one I3/S4 hero commitment.

This is **not** parcel fit or realized density. CITY-05 still owns exact shell/parcel fit and can falsify a particular siting. CITY-04 later measures only geometry inside its accepted seed.

## CITY-01 compatibility boundary

The candidate adds no route/crossing and does not reclassify access:

- X2 closure preserves meaningful Ensanche content plus inherited alternatives;
- X6 suspension uses X1 + camino-sur fallback rather than a dry Wedge→Puerto shortcut;
- X7 closure keeps cart-freight interruption real;
- Plaza closure does not require restricted `W17` as public routing;
- quiet Q routes remain low-intensity and do not become incident funnels.

## Fresh Reviewer challenge points

1. Independently verify every required CITY-02 domain has a real physical place home and no domain is satisfied by UI-only abstraction.
2. Recompute the 37-row A–D/S cross-tab and try to derive one axis from the other; a deterministic mapping would be a blocker.
3. Check all 23 A/B rows have exactly one complete profile and one unambiguous I0–I3 commitment.
4. Challenge whether the location programme remains feasible against CITY-00 scale/quiet invariants without trusting `CAPACITY_SANITY.md` as a parcel proof.
5. Trace every mobility anchor against accepted CITY-01 and look specifically for accidental publicisation of `W17`, new crossings, dry Wedge→Puerto or fake State-1/State-2 freight routes.
6. Challenge reactive-density metrics for false-green counting of props, decorative fabric, restricted routes or absent geometry.
7. Check that player-absence/witness/change language stays a spatial requirement and does not pre-accept Living World schedules, beliefs, dialogue, decisions or off-screen simulation.
8. Confirm C/D, S0/S1, I0 and quiet ordinary fabric are intentional substantial scope rather than omissions hidden by the A/B programme.
9. Inspect the complete baseline→candidate diff independently; do not rely on Worker conclusions.

## Concurrent-main note

During the Worker cycle, `main` advanced to `d20f7452ac689399b66f0601e8d0d8faa86d01a5` through accepted WP-H1-01 DocSync. The baseline→new-main delta affects H1 evidence/index surfaces only; it does not change CITY-00, CITY-01, CITY-02, ART or Production Blueprint inputs consumed here. The Worker's predecessor check therefore remains valid.

## Known residuals

- exact parcel/site placement, frontage dimensions and reusable shells — CITY-05;
- detailed interior/discovery topology — CITY-06;
- exact retained seed — CITY-03;
- Unity blockout and measured traversal/reactive density inside the accepted seed — CITY-04;
- full-city physical measurement outside the seed until sufficient realized geometry has an accepted owner;
- runtime schedules, opening hours, beliefs, dialogue, relationships, decisions, incidents and off-screen simulation;
- final runtime IDs, NPC bindings, narrative/backstories and minigame mechanics;
- State 1→State 2 production timing.

## Worker stop condition

After PR #81 is updated with the exact SHA of this commit and marked Ready, the Worker stops writing. Any fresh independent Reviewer FAIL requires a new repair cycle and a new exact-SHA freeze.
