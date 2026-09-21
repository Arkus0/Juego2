# WP-CITY-01 — Worker pre-review

WP: `WP-CITY-01 — Mobility, district graph + walk-time topology`  
Contract: `Docs/workpacks/CITY/WP-CITY-01.md`  
Baseline SHA: `7fe44840076eba05f1b67a7633cd33fc67b9023d`  
Active Worker: `ChatGPT GPT-5.6 Sol`  
Worker history: `ChatGPT GPT-5.6 Sol`  
Transfer SHA: `NONE`  
fail_cycle: **1**  
Governing protocol: `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` v1.7

This is Worker quality-gate evidence, not independent review.

Cycle 0 candidate `2fac578973cf3686d264b57c8ba7519b8ad507b2` received independent Reviewer **FAIL** in review `#5263730661`. The sole blocker was incomplete water-crossing mobility-profile semantics: `EW` was not assigned to X1–X7 and slower-pedestrian / bicycle behaviour across `EW` was underdetermined.

The branch was returned to Draft before repair. This cycle performs the causal repair only; it does not reopen CITY-00.

---

## 1. Predecessor / scope check

Accepted CITY-00 geography remains unchanged:

- Wedge, Ensanche bank and Orilla sur remain the only landmasses;
- X1..X7 remain the complete crossing set;
- X6/X7 remain mutually exclusive State-1/State-2 alternatives;
- there is no dry Wedge→Puerto edge and no Ensanche-bank↔Orilla-sur edge;
- X2/X3 remain the permanent Arroyo pedestrian floor;
- Plaza independence and the Wedge longitudinal route families are untouched.

No CITY-02 programme, CITY-03 retained-seed decision, Unity/navmesh/runtime, H0/H1 or ART authority is introduced.

Result: **PASS.**

---

## 2. Exact causal repair

`Docs/production/CITY_MOBILITY_TOPOLOGY.md` moved from v1.2 to v1.3 with only the affected contract surfaces changed:

1. `EW` now explicitly means water-crossing traversal/ferry motion and every X1..X7 row is assigned `EW`;
2. the crossing ledger now owns `Elev. + Access + Cost` together;
3. slower pedestrian uses ×1.35 for `EW` locomotion, with X6 wait added separately and never multiplied;
4. bicycle rides `EW + AR` crossings (X2/X4/X7), pushes X1, cannot use AF X3/X5, and receives no X6 carriage claim;
5. bicycle route comparison explicitly uses ledger base movement weights, without asserting a bicycle speed advantage;
6. CITY-04's represented-profile measurement question now includes EW validation.

No ordinary-pedestrian base cost or route-cost matrix value changed.

Result: **PASS.**

---

## 3. Crossing/profile audit

New derived evidence: `Docs/evidence/WP-CITY-01/PROFILE_CROSSING_AUDIT.md`.

The audit recomputes all seven slower-pedestrian crossing costs:

- X1 = 1.350;
- X2 = 0.945;
- X3 = 0.675;
- X4 = 1.215;
- X5 = 0.540;
- X6 = 1.350 + `W_ferry`;
- X7 = 1.350.

It also proves deterministic bicycle crossing behaviour for X1..X7 and recomputes the relevant Ensanche→Calle Mayor comparison:

- permanent X2 route to `W.SHOP`: 3.5 planning-weight minutes;
- X4 road-capable alternative while available: 7.3 planning-weight minutes.

Therefore the previous prose statement that the cyclist prefers X2 is now derivable from the authoritative rules.

Result: **PASS.**

---

## 4. X6 wait-composition challenge

The failed candidate left slower-pedestrian EW composition undefined. The repaired contract distinguishes motion from waiting:

- ordinary X6 = `1.0 + W_ferry`;
- slower-pedestrian X6 = `1.0 × 1.35 + W_ferry`.

At `W_ferry = 4`, slower X6 is 5.35 rather than 6.75, proving the wait term is not multiplied as locomotion.

X6 access remains `AX6`: pedestrian/porter/carryable load only. Bicycle, cart and handcart capacity are not added.

Result: **PASS.**

---

## 5. Bicycle / crossing-access challenge

The repaired rules derive directly from `EW + Access`:

| Crossing | Bicycle |
|---|---|
| X1 | push/dismount only |
| X2 | rideable |
| X3 | unavailable |
| X4 | rideable while available |
| X5 | unavailable |
| X6 | carriage not assumed |
| X7 | rideable in State 2 |

This does not change cart/service semantics: X2/X4/X7 remain the only full-cart crossings claimed by CITY-01.

Result: **PASS.**

---

## 6. Regression challenge

The repair diff was checked for unrelated semantic drift. It does not change:

- any node or crossing endpoint;
- any State-1/State-2 availability rule;
- any ordinary-pedestrian planning cost;
- the representative §7 route-cost matrix;
- Plaza removal or W17 restriction;
- X2 as the municipal closure lever;
- State-1 break-bulk / State-2 cart consequences;
- the CITY-03/CITY-04 bounded measurement boundary.

The affected scenario re-audit remains clean: pedestrians keep the same closure alternatives; bicycle X4 use is conditional on X4 availability; X6 stays non-bicycle/non-cart; X7 stays road-capable in State 2.

Result: **PASS.**

---

## 7. Residuals — explicitly not promoted to solved facts

Still downstream / empirical:

- actual slower-pedestrian multiplier after physical measurement;
- real bicycle comfort, clearance and dismount behaviour in realized geometry;
- exact metres, grades and stair counts;
- full-city end-to-end measurements outside the CITY-03 retained seed;
- runtime route choice / AI / live schedules;
- State 1→State 2 production timing.

The deterministic planning contract is now closed even though those physical/runtime facts remain intentionally open.

---

## 8. Worker verdict

The independent-review blocker has been repaired at its causal owner, the affected profile/scenario audit is clean, and no new in-claim blocker is known.

```text
WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FAIL_CYCLE: 1
WORKER_PRE_REVIEW_REVIEW_REPAIRED: #5263730661
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-CITY-01/WORKER_PRE_REVIEW.md
AFFECTED_AUDIT: Docs/evidence/WP-CITY-01/PROFILE_CROSSING_AUDIT.md
```

Next protocol step: refresh `HANDOFF.md`, read the exact PR HEAD, record it as the new Candidate/Frozen SHA in PR #73, mark Ready, and stop Worker writes pending a fresh independent Reviewer.
