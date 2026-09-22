# CTX-03 — Measurement and decisions

Status: **CANDIDATE EVIDENCE / PROCESS_ONLY**

## Exact measurement anchor

The hardened same-snapshot causal suite originally calibrated on exact candidate:

```text
64437ca5fcbc124660c8c47328ef281dd0fd8aa0
```

GitHub Actions:

- `CTX Process Envelope` run `35703773432` / job `106667698653`: **SUCCESS**;
- `Arkus Candidate Validation` run `35703773486`: **SUCCESS**;
- `Context Capsule Validation` run `35703773518`: **SUCCESS**.

The envelope job persisted the file-level report, quality replay, independent process controls, DocSync/history control and mechanical classification as artifact `10683043030`. Repair cycle 1 preserves the measured route source sets but strengthens the budget from base `initial_reads` alone to both base-profile and route-effective minimum/escalated sets. The ultimate candidate must rerun the same causal suite on its exact final SHA before freeze.

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

## Base profile process-envelope calibration

The base-profile layer retains 20% headroom above the exact post-CTX `initial_reads` calibration:

| Profile | Baseline | Ceiling |
|---|---:|---:|
| worker | 19,028 | 22,834 |
| repair_worker | 19,028 | 22,834 |
| reviewer | 19,028 | 22,834 |
| planner_gate | 19,955 | 23,946 |
| docsync | 8,594 | 10,313 |
| h1_local_executor | 6,311 | 7,574 |

This layer is no longer described as the complete mandatory-context envelope. It bounds only the unconditional base pack.

## Route-effective mandatory-context calibration

Repair cycle 1 adds a second budget layer over the already checker-owned concrete routes. It budgets both the minimum effective mandatory set and the authoritative escalated set, so repository-owned conditionals cannot grow outside the envelope simply because they are absent from `initial_reads`.

| Route family | Minimum baseline | Minimum ceiling | Escalated baseline | Escalated ceiling | Route-forced examples |
|---|---:|---:|---:|---:|---|
| H1 Worker/Reviewer | 29,896 | 35,876 | 37,028 | 44,434 | `FOUNDATIONAL_PROOF_STANDARD.md`, `H1_REMOTE_LOCAL_EXECUTION.md`, accepted predecessor capsule/navigation and authoritative predecessor sources on escalation |
| CITY Worker/Reviewer | 35,241 | 42,290 | 36,228 | 43,474 | `Docs/ROADMAP.md`, non-compressible CITY seed/capsule path and authoritative predecessor source on escalation |
| PA Worker/Reviewer | 22,855 | 27,426 | 43,067 | 51,681 | cumulative PA capsules/mandatory reads and PA authoritative result sources on material escalation |

The formula remains `ceil(baseline * 1.20)`. The checker owns the route universe and an independent effective-required-source oracle. `ctx03-process-controls.py` grows real conditional sources across the applicable route ceilings: H1 foundational, H1 local, CITY/ROADMAP and PA authoritative escalation. Those mutations must turn RED. Unrelated repository growth remains neutral to the derived base set.

Future ceiling increases in either base-profile or route-effective layers require a revision increment plus explicit justification.

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

CTX-03 demonstrates a material saving where it claims one (cumulative PA), preserves required high-cost context where the claim needs it (CITY), refuses to overstate an uncertainty-bounded reduction (H1), and now bounds growth of both the unconditional base pack and checker-owned route-effective mandatory context. The pre/post and effective budget universes are checker-owned, so neither compact capsules nor measured config can self-shrink the proof boundary.
