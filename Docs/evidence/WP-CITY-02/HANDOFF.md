# WP-CITY-02 — Worker → independent Reviewer handoff

WP: `WP-CITY-02 — Systemic locations + spatial-depth/interior programme`  
Contract: `Docs/workpacks/CITY/WP-CITY-02.md`  
PR: **#81**  
Baseline SHA: `0b8f23fbce227bbbc710c8410dff575fcb9fcf12`  
Active Worker: `ChatGPT GPT-5.6 Sol`  
Worker history: `ChatGPT GPT-5.6 Sol`  
Transfer SHA: `NONE`  
fail_cycle: **1**

This file is the Worker's final branch mutation before the cycle-1 refreeze. The exact 40-character commit containing this handoff is read from PR #81 immediately after this commit and recorded in PR metadata as both `Candidate HEAD SHA` and `Frozen candidate SHA`. PR metadata is the authoritative exact-SHA freeze record.

## Worker state at handoff

```text
Predecessor contract check: Docs/evidence/WP-CITY-02/WORKER_PLAN.md
Worker pre-review: CLEAN
Worker pre-review findings fixed: 1
Independent-review blockers fixed: 1
Independent FAIL repaired: review #5265810937
Worker pre-review evidence: Docs/evidence/WP-CITY-02/WORKER_PRE_REVIEW.md
Programme audit: Docs/evidence/WP-CITY-02/PROGRAMME_AUDIT.md
Capacity sanity: Docs/evidence/WP-CITY-02/CAPACITY_SANITY.md
Semantic owner: Docs/production/CITY_LOCATION_PROGRAMME.md v1.0
Branch frozen after this commit: YES
Worker verdict: IN_REVIEW after PR metadata refreeze
Reviewer verdict: PENDING fresh review
Previous reviewed candidate: 5ae61e0863bf1480e616275377306f5df940aeb5 — FAIL
```

## Cycle-1 blocker repaired

Independent review #5265810937 correctly rejected the cycle-0 Q6 capacity proof. The previous argument inferred feasibility from `21 A/B handles / 0.365 km²` plus selective I-depth, even though a programme handle can be a threshold, square, yard or building and I-depth does not measure land/frontage/open-space demand.

That false-green oracle has been removed.

Cycle 1 introduces a bounded, falsifiable **programme-capacity proof** in `CAPACITY_SANITY.md`:

- every A/B place receives one role-based `T/S/M/L` programme-envelope class;
- the **upper** class bound is charged, not an average or midpoint;
- the envelope includes shell/footprint demand plus dedicated yard/open-space/threshold apron where applicable;
- exact parcel boundaries, exact dimensions and site composition remain CITY-05;
- each dense part has an explicit A/B cap, hard ordinary/quiet reserve and separate 15% uncommitted/circulation margin;
- named quiet fabric cannot be borrowed as overflow;
- class-ceiling breach, programme growth, part-cap breach or reserve borrowing forces Q6 recomputation/review.

Dense-part upper-bound result:

| Part | Charged A/B upper demand | A/B cap | Hard reserve | Status |
|---|---:|---:|---:|---|
| Wedge core | 26,100 / 150,000 m² | 25% | 60% | PASS |
| Ensanche | 5,000 / 90,000 m² | 15% | 70% | PASS |
| Barrio Alto | 7,500 / 60,000 m² | 20% | 65% | PASS |
| Puerto | 13,100 / 45,000 m² | 35% | 50% | PASS |
| Entrada | 5,600 / 20,000 m² | 30% | 55% | PASS — only 400 m² cap headroom |
| **Dense total** | **57,300 / 365,000 m²** | — | **225,500 m² aggregate hard reserve** | **PASS** |

Concrete negative control: if `loc.entrada.depot_forecourt` proves to require `L` rather than current `M`, Entrada rises to 8,100 m² / 40.5% and **Q6 fails**. CITY-05 may not hide that failure by consuming the ordinary-edge reserve.

Repair commits before this final handoff:

- `9661006d7f617dad287c7ea383763748dcb2ab6f` — replace count-per-handle capacity oracle with falsifiable spatial-demand budgets;
- `1f4cad98fe105f0f13a9ace591a3bec962909687` — reconcile programme audit/Q6;
- `228d6419606c11465085083442a5b268f2775770` — rerun strict Worker pre-review for cycle 1.

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

Cycle 1 does not change the 37-row programme or its A–D/S/I assignments. It repairs the evidentiary/acceptance layer for Q6.

## Independence / scope highlights

A–D and S0–S4 remain intentionally orthogonal. Direct witnesses include:

- `loc.entrada.arrival` = A/S1;
- `loc.ensanche.neighbourhood_anchor` = A/S2;
- `loc.ribera.workshop` = A/S3;
- `loc.casco.bar` = A/S4;
- B examples at S1/S2/S3;
- `loc.casco.shared_court` = C/S3;
- ordinary C/S1 and scenic D/S0 fabric.

The capacity classes are also deliberately independent of those axes: A/S1 `loc.entrada.arrival` is `T`, while B/S2/I0 `loc.plaza.market` is `L`.

The old/civic core does not monopolise A: Casco + Plaza own 2/8 A anchors. Puerto has ordinary work, logistics and social roles and no S4 hero place.

## Earlier Worker pre-review repair

Before cycle-0 freeze the Worker found one material ambiguity: `loc.ribera.service_yard` was written as `I0/I1` in the master ledger/profile while the interior ledger assigned I1. Commit `e6fe45d94701d4b2cec0cada6f6996287fac3874` repaired both authoritative references to **I1**.

Cycle 1 adds no new place-programme semantic repair beyond the Q6 capacity proof.

## CITY-01 compatibility boundary

The candidate adds no route/crossing and does not reclassify access:

- X2 closure preserves meaningful Ensanche content plus inherited alternatives;
- X6 suspension uses X1 + camino-sur fallback rather than a dry Wedge→Puerto shortcut;
- X7 closure keeps cart-freight interruption real;
- Plaza closure does not require restricted `W17` as public routing;
- quiet Q routes remain low-intensity and do not become incident funnels.

## Fresh Reviewer challenge points

1. Reproduce the cycle-0 blocker first: confirm the old handle-density argument is gone and is not smuggled back through I-depth.
2. Independently audit the `T/S/M/L` assignment and arithmetic in `CAPACITY_SANITY.md`; challenge whether the upper bounds are conservative enough for the stated programme roles without becoming parcel design.
3. Verify each dense part, not merely the aggregate, satisfies its A/B cap and hard reserve; specifically test Entrada and Puerto because they have the least headroom.
4. Run the explicit negative control: promote `loc.entrada.depot_forecourt` M→L and confirm Q6 becomes FAIL rather than still passing.
5. Confirm named quiet fabric (`W07`, `W15`, upper residential fabric, ordinary Puerto work frontage) cannot be borrowed to rescue a failed part.
6. Independently verify every required CITY-02 domain has a real physical place home and no domain is satisfied by UI-only abstraction.
7. Recompute the 37-row A–D/S cross-tab and try to derive one axis from the other; a deterministic mapping would be a blocker.
8. Check all 23 A/B rows have exactly one complete profile and one unambiguous I0–I3 commitment.
9. Trace every mobility anchor against accepted CITY-01 and look specifically for accidental publicisation of `W17`, new crossings, dry Wedge→Puerto or fake State-1/State-2 freight routes.
10. Inspect the complete baseline→candidate diff independently; do not rely on Worker conclusions.

## Concurrent-main note

At cycle-1 refreeze PR #81 still reports `main` base `d20f7452ac689399b66f0601e8d0d8faa86d01a5`. The earlier baseline→base movement affected accepted H1 evidence/index surfaces only; it did not change CITY-00, CITY-01, CITY-02, ART or Production Blueprint inputs consumed here.

## Known residuals

- exact parcel/site placement, exact footprint/frontage dimensions and reusable shells — CITY-05;
- a CITY-05 discovery that any place exceeds its capacity-class ceiling — requires Q6 recomputation/review, not quiet-reserve borrowing;
- detailed interior/discovery topology — CITY-06;
- exact retained seed — CITY-03;
- Unity blockout and measured traversal/reactive density inside the accepted seed — CITY-04;
- full-city physical measurement outside the seed until sufficient realized geometry has an accepted owner;
- runtime schedules, opening hours, beliefs, dialogue, relationships, decisions, incidents and off-screen simulation;
- final runtime IDs, NPC bindings, narrative/backstories and minigame mechanics;
- State 1→State 2 production timing.

## Worker stop condition

After PR #81 is updated with the exact SHA of this commit and marked Ready, the Worker stops writing. Any fresh independent Reviewer FAIL requires a new repair cycle and a new exact-SHA freeze.
