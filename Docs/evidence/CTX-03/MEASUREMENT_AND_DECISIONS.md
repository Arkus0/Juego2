# CTX-03 — Measurement and decisions

Status: **CANDIDATE EVIDENCE / PROCESS_ONLY**

## Exact measurement anchor

The hardened same-snapshot causal suite ran on exact candidate:

```text
64437ca5fcbc124660c8c47328ef281dd0fd8aa0
```

GitHub Actions:

- `CTX Process Envelope` run `35703773432` / job `106667698653`: **SUCCESS**;
- `Arkus Candidate Validation` run `35703773486`: **SUCCESS**;
- `Context Capsule Validation` run `35703773518`: **SUCCESS**.

The envelope job persisted the file-level report, quality replay, independent process controls, DocSync/history control and mechanical classification as artifact `10683043030`. Later candidate commits only reconcile/calibrate evidence and protocol text; the final Worker pre-review must still rerun the same causal suite on the ultimate evidence-bearing SHA before freeze.

## Measurement method

Estimator: `ceil(UTF-8 bytes / 4)` per unique source on one checked-out candidate snapshot. Declared provider/tokenizer uncertainty: `20%` independently on pre and post estimates. A saving is labelled material only when:

```text
pre - post > pre_uncertainty + post_uncertainty
```

The route universe, canonical profile source, calibration substitutions and pre-CTX representative predecessor inventory are checker-owned. The compact artifact/config under measurement therefore cannot narrow its own baseline or choose a friendlier route.

Dynamic mandatory inputs such as live GitHub state and exact PR diff are held outside both static route totals rather than removed from one side. Sources that remain binding in both designs are held constant.

## Representative routes

| Route family | Pre estimate | Post minimum | Minimum delta | Material? | Post escalated | Escalated delta | Interpretation |
|---|---:|---:|---:|---|---:|---:|---|
| H1 Worker/Reviewer | 35,104 | 29,896 | -5,208 / -14.8359% | **NO** | 37,028 | +1,924 / +5.4809% | Minimum route is smaller, but not beyond combined uncertainty. Foundational/H1 local overlay remains binding; material escalation may exceed baseline. |
| CITY Worker/Reviewer | 27,922 | 35,241 | +7,319 / +26.2123% | **NO SAVING** | 36,228 | +8,306 / +29.7472% | Quality-first result: exact `CITY_PRODUCT_SEED.md` is non-compressible and ROADMAP remains required for unresolved H1-08 cross-track eligibility. |
| PA Worker/Reviewer | 38,389 | 22,855 | -15,534 / -40.4647% | **YES** | 43,067 | +4,678 / +12.1858% | CTX-02 capsule-chain navigation materially reduces cumulative starting context. Material semantic escalation intentionally restores all authoritative PA sources even when that is larger than pre-CTX. |

No route is called materially improved merely because its raw estimate is lower.

## Profile process-envelope calibration

The final-shape candidate config records 20% headroom above the exact effective role/profile baseline:

| Profile | Baseline | Ceiling |
|---|---:|---:|
| worker | 19,028 | 22,834 |
| repair_worker | 19,028 | 22,834 |
| reviewer | 19,028 | 22,834 |
| planner_gate | 19,955 | 23,946 |
| docsync | 8,594 | 10,313 |
| h1_local_executor | 6,311 | 7,574 |

The planner baseline fell from its earlier calibration because ROADMAP v1.33 moved verbose accepted H0 closure chronology to non-bootstrap history while retaining live state/order/gates. That is a real structural context reduction rather than an enlarged budget.

The required read sets are derived from the checker-bound canonical `context-bootstrap-profiles.json`; `context-envelope.json` cannot redirect the profile source, drop a profile, remove a representative route or retarget calibration placeholders. `ctx03-process-controls.py` proves unrelated repository growth is neutral while growth of a real required source across its ceiling turns RED. Future ceiling increases require a revision increment plus explicit justification.

## Cumulative PA decision

Decision: **KEEP CTX-02 CAPSULE CHAIN; DO NOT CREATE A SECOND PA RESULT REGISTRY.**

Reason:

1. cumulative PA-01..03 minimum route produces a material `40.4647%` estimated reduction under the conservative uncertainty rule;
2. CTX-02 already owns completion-side discovery, exact status/disposition preservation, authoritative source fingerprints and fail-closed reconstruction;
3. CTX-03 quality replay independently verifies the material PA-03 authoritative result is reachable when escalation is required;
4. the fully escalated route is larger than pre-CTX, which is acceptable because escalation preserves source fidelity rather than guaranteeing savings;
5. adding another compact PA registry would create a second derived universe without measured need.

## CITY decision

No extra compression is introduced for CITY-04. The route becoming larger is not a quality regression: CTX-02 explicitly makes `CITY_PRODUCT_SEED.md` non-compressible, and current CITY-04 eligibility still has an H1-08 cross-track question. Optimizing those reads away would violate CTX-03's quality invariant.

## H1 decision

The minimum H1 route is smaller by `14.8359%`, but does not exceed the deliberately conservative combined uncertainty. CTX-03 records **no demonstrated material H1 saving** rather than weakening the estimator or dropping the foundational/local overlay.

## Result

CTX-03 demonstrates a material saving where it claims one (cumulative PA), preserves required high-cost context where the claim needs it (CITY), and refuses to overstate an uncertainty-bounded reduction (H1). The pre/post universe is checker-owned, so neither the compact capsules nor the measured config can self-shrink the baseline to manufacture that result.
