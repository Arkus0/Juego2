# CTX-03 — Measurement and decisions

Status: **CANDIDATE EVIDENCE / PROCESS_ONLY**

## Exact calibration anchor

The repair-cycle circuit-breaker audit used the successful exact-SHA report from candidate:

```text
80fe3c3c5ae03ebf169bf52b4648aca17fb254ef
```

GitHub Actions:

- `CTX Process Envelope` run `35707134612` / job `106678674050`: **SUCCESS**;
- `Arkus Candidate Validation` run `35707134798`: **SUCCESS**;
- `Context Capsule Validation` run `35707134622`: **SUCCESS**.

Artifact `10684209601` persisted the file-level route/profile estimates used to calibrate the expanded envelope. The ultimate candidate must rerun the same causal suite on its exact final SHA before freeze; this calibration anchor is not itself the final candidate.

## Measurement method

Estimator: `ceil(UTF-8 bytes / 4)` per unique source on one checked-out candidate snapshot. Declared provider/tokenizer uncertainty: `20%` independently on pre and post estimates. A saving is labelled material only when:

```text
pre - post > pre_uncertainty + post_uncertainty
```

The route universe, canonical profile source, calibration substitutions, fixed conditional-source universe and pre-CTX representative predecessor inventory are checker-owned. The compact artifact/config under measurement therefore cannot narrow its own baseline or choose a friendlier route.

Dynamic mandatory inputs such as live GitHub state, exact PR diff and candidate-specific exact dependency/evidence paths are held outside static corpus totals rather than omitted. Sources that remain binding in both designs are held constant.

## Representative routes

| Route family | Pre estimate | Post minimum | Minimum delta | Material? | Post escalated | Escalated delta | Interpretation |
|---|---:|---:|---:|---|---:|---:|---|
| H1 Worker/Reviewer | 35,104 | 29,896 | -5,208 / -14.8359% | **NO** | 37,028 | +1,924 / +5.4809% | Minimum route is smaller, but not beyond combined uncertainty. Foundational/H1 local overlay remains binding; material escalation may exceed baseline. |
| CITY Worker/Reviewer | 27,922 | 35,241 | +7,319 / +26.2123% | **NO SAVING** | 36,228 | +8,306 / +29.7472% | Quality-first result: exact `CITY_PRODUCT_SEED.md` is non-compressible and ROADMAP remains required for unresolved H1-08 cross-track eligibility. |
| PA Worker/Reviewer | 38,389 | 22,855 | -15,534 / -40.4647% | **YES** | 43,067 | +4,678 / +12.1858% | CTX-02 capsule-chain navigation materially reduces cumulative starting context. Material semantic escalation intentionally restores all authoritative PA sources even when larger than pre-CTX. |

No route is called materially improved merely because its raw estimate is lower.

## Base profile calibration

The base layer retains 20% headroom above canonical `initial_reads`:

| Profile | Baseline | Ceiling |
|---|---:|---:|
| worker | 19,028 | 22,834 |
| repair_worker | 19,028 | 22,834 |
| reviewer | 19,028 | 22,834 |
| planner_gate | 19,955 | 23,946 |
| docsync | 8,594 | 10,313 |
| h1_local_executor | 6,311 | 7,574 |

This bounds only the unconditional base pack.

## Conditional-profile calibration

The final circuit-breaker adds a checker-owned fixed-conditional superset for **every** canonical profile. This is intentionally broader than the six representative routes so a fixed `conditional_reads` source in repair/planning/DocSync cannot grow outside every ceiling.

Exact per-file estimates from run `35707134612` used by these unions include:

- `FOUNDATIONAL_PROOF_STANDARD.md`: `4,048`;
- `H1_REMOTE_LOCAL_EXECUTION.md`: `4,357`;
- `Docs/ROADMAP.md`: `6,509`;
- `CONTEXT_CAPSULE_V1.md`: `4,698`;
- capsule `index.json`: `494`.

| Profile | Conditional-union baseline | Ceiling | Fixed conditional superset beyond base |
|---|---:|---:|---|
| worker | 39,134 | 46,961 | Foundation, H1 local protocol, ROADMAP, capsule protocol + index |
| repair_worker | 39,134 | 46,961 | same checker-owned superset; candidate-specific FAIL/evidence stays dynamic |
| reviewer | 39,134 | 46,961 | same checker-owned superset; candidate diff/exact durable evidence stays dynamic |
| planner_gate | 24,003 | 28,804 | Foundation; ROADMAP is already in base |
| docsync | 20,295 | 24,354 | ROADMAP, capsule protocol + index |
| h1_local_executor | 6,311 | 7,574 | no extra fixed conditional reads |

`context-envelope-check.py` also extracts explicit fixed `Docs/...md|json` paths from the canonical profile's `conditional_reads`. A newly introduced explicit fixed path that is absent from the checker-owned superset turns RED pending intentional oracle/calibration review. Conversely, removing/narrowing profile prose cannot shrink the checker-owned set.

`ctx03-process-controls.py` attempts to grow **every fixed conditional source for every applicable profile** across its conditional-profile ceiling. This includes the requested H1 foundational/local and CITY/ROADMAP cases plus repair Worker, planner and DocSync variants.

## Concrete route-effective calibration

The route layer remains separately necessary because concrete routes include capsule payloads, non-compressible sources and authoritative escalation beyond the global fixed protocols.

| Route family | Minimum baseline | Minimum ceiling | Escalated baseline | Escalated ceiling | Route-forced examples |
|---|---:|---:|---:|---:|---|
| H1 Worker/Reviewer | 29,896 | 35,876 | 37,028 | 44,434 | Foundation, H1 local protocol, accepted predecessor capsule/navigation and authoritative predecessor sources on escalation |
| CITY Worker/Reviewer | 35,241 | 42,290 | 36,228 | 43,474 | ROADMAP, non-compressible CITY seed/capsule path and authoritative predecessor source on escalation |
| PA Worker/Reviewer | 22,855 | 27,426 | 43,067 | 51,681 | cumulative PA capsules/mandatory reads and PA authoritative result sources on material escalation |

The formula for all three layers remains `ceil(baseline * 1.20)`. Future ceiling increases require a revision increment plus explicit justification.

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

CTX-03 demonstrates a material saving where it claims one (cumulative PA), preserves required high-cost context where the claim needs it (CITY), refuses to overstate an uncertainty-bounded reduction (H1), and bounds future growth at three levels: unconditional base, every fixed conditional profile superset, and concrete route-effective minimum/escalation. The audited config cannot silently narrow any of those checker-owned universes.
