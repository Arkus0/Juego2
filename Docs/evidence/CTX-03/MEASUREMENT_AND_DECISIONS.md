# CTX-03 — Measurement and decisions

Status: **CANDIDATE EVIDENCE / PROCESS_ONLY**

## Exact measurement anchor

The first fully calibrated causal suite ran on exact candidate:

```text
0be2a5cdf97047e11561d71944919c9d27850bc3
```

GitHub Actions:

- `CTX Process Envelope` run `35701788029` / job `106661279778`: **SUCCESS**;
- `Arkus Candidate Validation` run `35701787993`: **SUCCESS**;
- `Context Capsule Validation` run `35701787989`: **SUCCESS**.

The envelope job persisted the exact file-level report plus quality replay, process controls, DocSync/history control and mechanical classification as artifact `10681838369` (`ctx-process-envelope-0be2a5cdf97047e11561d71944919c9d27850bc3`). Final Worker pre-review must rerun the same suite on the final evidence-bearing candidate SHA; this anchor records the measurement decision, not the eventual freeze SHA.

## Measurement method

Estimator: `ceil(UTF-8 bytes / 4)` per unique source on one checked-out candidate snapshot. Declared provider/tokenizer uncertainty: `20%` independently on pre and post estimates. A saving is labelled material only when:

```text
pre - post > pre_uncertainty + post_uncertainty
```

Dynamic mandatory inputs such as live GitHub state and exact PR diff are held outside both static route totals rather than removed from one side. Sources that remain binding in both designs are held constant.

## Representative routes

| Route family | Pre estimate | Post minimum | Minimum delta | Material? | Post escalated | Escalated delta | Interpretation |
|---|---:|---:|---:|---|---:|---:|---|
| H1 Worker/Reviewer | 37,781 | 29,896 | -7,885 / -20.8703% | **NO** | 37,028 | -753 / -1.9931% | Useful reduction, but it does not clear conservative combined uncertainty. Foundational/H1 local overlay stays binding. |
| CITY Worker/Reviewer | 30,599 | 37,918 | +7,319 / +23.9191% | **NO SAVING** | 38,905 | +8,306 / +27.1447% | Correct quality-first result: exact `CITY_PRODUCT_SEED.md` is non-compressible and ROADMAP is still required for the unresolved H1-08 cross-track gate. |
| PA Worker/Reviewer | 41,066 | 22,855 | -18,211 / -44.3457% | **YES** | 43,067 | +2,001 / +4.8726% | CTX-02 capsule-chain navigation materially reduces cumulative starting context. When semantics require all authoritative PA results, escalation intentionally gives the full sources back. |

No route is called materially improved merely because its raw estimate is lower.

## Profile process-envelope calibration

The reviewed candidate config records 20% headroom above the exact effective role/profile baseline:

| Profile | Baseline | Ceiling |
|---|---:|---:|
| worker | 19,028 | 22,834 |
| repair_worker | 19,028 | 22,834 |
| reviewer | 19,028 | 22,834 |
| planner_gate | 22,632 | 27,159 |
| docsync | 8,594 | 10,313 |
| h1_local_executor | 6,311 | 7,574 |

The required read sets are derived from `context-bootstrap-profiles.json`; `context-envelope.json` does not duplicate their file lists. `ctx03-process-controls.py` proves unrelated repository growth does not affect those derived sets, while growth of a real required source across its ceiling turns RED. Ceiling increases require a revision increment plus explicit justification.

## Cumulative PA decision

Decision: **KEEP CTX-02 CAPSULE CHAIN; DO NOT CREATE A SECOND PA RESULT REGISTRY.**

Reason:

1. the actual cumulative PA-01..03 minimum route produces a material `44.3457%` estimated reduction under the conservative uncertainty rule;
2. CTX-02 already owns completion-side discovery, exact status/disposition preservation, authoritative source fingerprints and fail-closed reconstruction;
3. CTX-03 quality replay independently verifies the material PA-03 authoritative result is reachable when escalation is required;
4. the fully escalated route is slightly larger than pre-CTX, which is acceptable because escalation exists to preserve source fidelity rather than guarantee savings;
5. adding another compact PA registry would create a second derived universe without a measured need.

## CITY decision

No extra compression is introduced for CITY-04. The route becoming larger is not a quality regression: CTX-02 explicitly makes `CITY_PRODUCT_SEED.md` non-compressible, and current CITY-04 eligibility still has an H1-08 cross-track question. Optimizing those reads away would violate CTX-03's quality invariant.

## H1 decision

The minimum H1 route is smaller by `20.8703%`, but that number does not exceed the deliberately conservative combined uncertainty. CTX-03 therefore records **no demonstrated material H1 saving** rather than weakening the estimator or dropping the foundational/local overlay to make the result look better.

## Result

CTX-03 demonstrates a material saving where it claims one (cumulative PA), preserves required high-cost context where the claim needs it (CITY), and refuses to overstate a plausible but uncertainty-bounded reduction (H1). This is the intended quality-first closure criterion.
