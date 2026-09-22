# CTX-03 — Measurement and decisions

Status: **CANDIDATE EVIDENCE / PROCESS_ONLY**

## Measurement method

Estimator: `ceil(UTF-8 bytes / 4)` per unique repository source on one checked-out candidate snapshot. Declared provider/tokenizer uncertainty: `20%` independently on pre and post estimates. A saving is called material only when:

```text
pre - post > pre_uncertainty + post_uncertainty
```

The representative route universe, profile source, calibration substitutions, fixed conditional-source universe and pre-CTX representative predecessor inventory are checker-owned. The compact config cannot shrink its own measurement universe.

The six H1/CITY/PA routes are calibration and quality-replay cases only. They are **not** the complete future repository-context universe.

## Representative results

| Route family | Pre | Post minimum | Minimum result | Post escalated | Interpretation |
|---|---:|---:|---|---:|---|
| H1 Worker/Reviewer | 35,104 | 29,896 | -14.8359%, **not material** under combined uncertainty | 37,028 | foundational/H1-local context remains binding |
| CITY Worker/Reviewer | 27,922 | 35,241 | no saving | 36,228 | non-compressible CITY seed + unresolved cross-track state remain mandatory |
| PA Worker/Reviewer | 38,389 | 22,855 | -40.4647%, **material** | 43,067 | capsule navigation saves startup cost; material escalation restores authoritative sources even when larger |

CTX-03 therefore claims a material reduction only for cumulative PA. It does not tune the universe until every route looks cheaper.

## Layer 1 — base profile budgets

| Profile | Baseline | Ceiling |
|---|---:|---:|
| worker | 19,028 | 22,834 |
| repair_worker | 19,028 | 22,834 |
| reviewer | 19,028 | 22,834 |
| planner_gate | 19,955 | 23,946 |
| docsync | 8,594 | 10,313 |
| h1_local_executor | 6,311 | 7,574 |

These cover only fixed base `initial_reads`.

## Layer 2 — fixed conditional-profile budgets

| Profile | Conditional union | Ceiling |
|---|---:|---:|
| worker | 39,134 | 46,961 |
| repair_worker | 39,134 | 46,961 |
| reviewer | 39,134 | 46,961 |
| planner_gate | 24,003 | 28,804 |
| docsync | 20,295 | 24,354 |
| h1_local_executor | 6,311 | 7,574 |

The checker-owned fixed set covers Foundation, H1 remote/local protocol, ROADMAP and capsule protocol/index wherever applicable. A new explicit fixed repository source absent from the reviewed oracle turns RED; deleting profile prose cannot shrink the checker-owned set.

## Layer 3 — representative route-effective budgets

| Route family | Minimum baseline / ceiling | Escalated baseline / ceiling |
|---|---:|---:|
| H1 Worker/Reviewer | 29,896 / 35,876 | 37,028 / 44,434 |
| CITY Worker/Reviewer | 35,241 / 42,290 | 36,228 / 43,474 |
| PA Worker/Reviewer | 22,855 / 27,426 | 43,067 / 51,681 |

These cover concrete capsule payloads, non-compressible sources and authoritative escalation for the six calibration routes.

## Layer 4 — dynamic repository envelope

Route-dependent repository context is **not** exempt from budgeting.

Reviewed policy v1:

```text
per_source_ceiling_estimate = 32768
aggregate_route_ceiling_estimate = 131072
policy_revision = 1
```

The dynamic checker independently discovers every current/future workpack contract under `Docs/workpacks/**/WP-*.md` and, for a concrete route, accounts for repository-backed exact contracts, direct dependencies, explicit contract-mandatory repository inputs, repository Worker evidence when applicable, anchored manifests and manifest-named repository files.

A new placeholder class fails closed pending explicit reviewed classification. A future role using an existing reviewed exact-contract slot inherits the same resolver. A future WP outside H1/CITY/PA therefore cannot grow without entering a reviewed ceiling merely because it was absent from the six calibration routes.

Only genuinely external payloads remain outside repository corpus estimates, such as live GitHub metadata, API response bytes, a current review stored only on GitHub, a generated complete PR diff, or an external handoff anchor. If that logical input is persisted in the repository and becomes mandatory context, it is repository-backed and budgeted.

Ceiling increases require an explicit policy revision increment and non-empty justification.

## Cumulative PA decision

**KEEP CTX-02 CAPSULE CHAIN; DO NOT CREATE A SECOND PA RESULT REGISTRY.** The PA minimum route demonstrates a material saving; accepted CTX-02 already owns completion-side discovery, disposition preservation and fail-closed reconstruction; CTX-03 quality replay keeps authoritative PA results reachable on escalation. A second compact registry would add another derived universe without measured need.

## Result

CTX-03 bounds mandatory repository-context growth at **four** levels: base, fixed conditional, representative route-effective and general dynamic repository context. External payloads are separated explicitly rather than used as a blanket “dynamic input” exemption. Quality-first routes may stay large or grow when authoritative context is materially required.
