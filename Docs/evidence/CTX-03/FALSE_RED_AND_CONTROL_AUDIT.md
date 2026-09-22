# CTX-03 — False-red and causal control audit

Status: **CANDIDATE EVIDENCE / PROCESS_ONLY**

## Goal

Move deterministic protocol defects left **without reducing false-green detection power or semantic Reviewer authority**. A red check must not be promoted to “the WP is wrong” unless a reviewed deterministic verifier causally owns that claim, but unknown/infra red state also must not be silently ignored to start review.

Repair cycle 1 circuit-breaks the three classes raised by independent review `#5275757245`: exact-candidate pre-review ordering, effective mandatory-context growth, and retry-safe same-SHA terminal closure. The Worker's own pre-review then widened the context-growth repair once more after discovering that repair/planner/DocSync fixed conditionals were not yet inside the six H1/CITY/PA route budgets.

## Mechanical outcome contract

| Outcome | Meaning | WP defective? | May independent semantic review start? |
|---|---|---:|---:|
| `PASS` | registered deterministic condition satisfied | no | yes, subject to other terminal predicates |
| `FAIL` | registered deterministic verifier causally proves its owned candidate condition false | **yes, mechanically** | no; Worker repairs first |
| `REVIEW_BLOCKED` | lifecycle/handoff/freeze metadata is incomplete, pending, stale or incoherent | no semantic defect asserted | no |
| `NOT_APPLICABLE` | verifier does not apply to this candidate/mode | no | neutral |
| `INFRA_ERROR` | runner/API/tool/unregistered-red/unclassified failure gives no causal candidate defect proof | **no** | no until triaged/rerun |

## Registry boundary

`Docs/engineering/mechanical-verifier-registry.json` is the reviewed mapping from GitHub check names to deterministic ownership/failure class. An unregistered red cannot count as WP FAIL; it remains operationally blocking as `INFRA_ERROR`. A registered FAIL-capable verifier must emit a structured outcome and cannot claim semantic Reviewer authority.

## Exact-candidate Worker pre-review ordering

The failed candidate `1421f1690f1bd20b578ba8f70ee8ee5deb90b67a` exposed a sequencing defect: the complete Worker pre-review covered parent `460f997c8e70720356f285583977530cc2175c76`, then a repository evidence file claiming final cleanliness was committed afterward. Exact-SHA CI on the child did not retroactively make the complete Worker pre-review cover the child's bytes.

Repair cycle 1 removes that self-invalidating pattern:

1. all implementation and repository/evidence bytes are finalized and pushed while Draft + ACTIVE;
2. writers stop and the exact resulting HEAD is read;
3. the complete Worker pre-review inspects that exact HEAD and full baseline→candidate diff;
4. only if clean, a durable GitHub PR issue comment records the clean marker, exact Candidate SHA, findings count and evidence pointers;
5. `derive-worker-review-metadata.py` accepts that durable issue-comment URL and rejects a repository-local final-clean pointer;
6. Ready/freeze metadata may then change without changing candidate bytes.

Any later repository/evidence mutation changes HEAD and invalidates the clean result.

## Effective mandatory-context envelope

The original candidate bounded only `initial_reads`. The first repair added concrete H1/CITY/PA route-effective budgets. Circuit-breaker pre-review then found a broader variant: canonical `repair_worker`, `planner_gate` and `docsync` also contain fixed conditional reads that can become mandatory but were not necessarily activated by those six routes.

The final repair therefore uses **three layers**:

- **base profile** — canonical `initial_reads`;
- **conditional profile** — base plus checker-owned fixed conditional superset for every canonical profile;
- **route-effective** — concrete H1/CITY/PA minimum and escalated sets, including capsule payloads, non-compressible material and authoritative predecessor/result escalation.

`CANONICAL_FIXED_CONDITIONAL_SOURCES` is independent of the profile/config under audit. The checker additionally extracts explicit fixed `Docs/...md|json` paths from canonical `conditional_reads`: a newly introduced fixed path not present in the checker-owned set turns RED pending intentional oracle/calibration review. Removing/narrowing profile prose does not shrink the checker-owned set.

The fixed conditional universe covers:

- Worker / repair Worker / Reviewer: foundational proof, H1 remote/local protocol, ROADMAP, capsule protocol + index;
- planner/gate: foundational proof (ROADMAP already base);
- DocSync: ROADMAP, capsule protocol + index;
- H1 local executor: no additional fixed conditional reads.

Candidate-specific exact dependency/evidence sources remain mandatory dynamic inputs outside the static corpus budget; that exclusion does not apply to any known fixed protocol/global-state source.

`ctx03-process-controls.py` grows **every checker-owned fixed conditional source for every applicable profile** across the corresponding conditional-profile ceiling. It also retains the concrete H1 foundational/local, CITY/ROADMAP and cumulative PA escalation route controls. Unrelated 100KB growth remains GREEN. Ceiling increases in base, conditional-profile or route layers require revision increment plus explicit justification.

This closes the defect class rather than the three named examples.

## Retry-safe REVIEW_READY_CLOSED

The failed workflow woke only on creation of a new Automation V2 `REVIEW_READY` comment. Because Automation V2 deduplicates that marker by PR+SHA, a first closure attempt blocked by red/pending freeze state could become permanently unreachable after a same-SHA metadata/gate correction.

After repair, `.github/workflows/review-ready-closure.yml` wakes from either the original bot `issue_comment` or completion of `Arkus Candidate Validation`. The workflow-run path resolves the exact PR/candidate SHA, finds an already-existing durable `REVIEW_READY` marker for that same SHA, and reruns the canonical handoff/freeze/final-live-HEAD closure. Persistence remains idempotent by `review-ready-closed:<PR>:<SHA>`.

`scripts/review-ready-closure.py --self-test` reproduces the exact negative→repair→GREEN case with the same head and same marker. No semantic authority is added; CLOSED is lifecycle evidence only.

## Causal controls

| Claim | Independent/derived universe | Causal negative control | Required result |
|---|---|---|---|
| base role context ceiling | canonical `initial_reads` | grow real unconditional source | `FAIL` |
| fixed conditional completeness | checker-owned fixed set + extracted explicit fixed profile paths | add new explicit fixed path absent from checker set / drop conditional budget | `FAIL` |
| all fixed conditional growth bounded | every canonical profile fixed superset | grow every real fixed source across its applicable ceiling | `FAIL` |
| concrete H1 foundational/local | checker-owned H1 routes | grow Foundation/H1-local source | `FAIL` |
| concrete CITY cross-track | checker-owned CITY routes | grow ROADMAP | `FAIL` |
| effective PA escalation | checker-owned cumulative PA routes | grow canonical PA-03 result | `FAIL` |
| unrelated repo growth should not false-red | derived base set | add 100KB unrelated file | GREEN |
| ceiling increase reviewability | prior config from base ref | increase any layer without revision + justification | `FAIL` |
| escalation completeness | canonical profile `must_escalate_if` | delete one required predicate | `FAIL` |
| PA material-source reachability | production route + real capsule selectors | remove canonical PA-03 result selector | quality replay RED |
| CITY non-compressible source | production route + real CITY capsule mandatory read | remove `CITY_PRODUCT_SEED.md` | quality replay RED |
| exact pre-review ordering | final Git HEAD vs external clean record | repository-local final-clean pointer rejected | parent review cannot false-bind child bytes |
| terminal REVIEW_READY transaction | live-state-shaped closure oracle | remove marker / move HEAD / break gate | `REVIEW_BLOCKED` |
| same-SHA retry | existing marker + later Candidate Validation completion | freeze RED -> same-SHA freeze GREEN | blocked -> PASS |

The quality replay proves routing/discoverability, not model semantic competence. Independent Reviewer reasoning remains mandatory.

## Historical corpus classification

`HISTORICAL_CLASSIFICATION.json` remains a classification of durable historical handoff/failure families. Generic natural-language semantic equivalence is explicitly rejected as a deterministic gate. The current independent FAIL is not reclassified as infrastructure noise: all three reviewed blockers are real current-WP defects repaired at their causal boundary.

## Validation rule for the repaired candidate

Earlier GREEN runs are calibration/history only. After the **last repository/evidence mutation**, the Worker must rerun the exact canonical CTX-03 surface and then perform the complete Worker pre-review against that same HEAD before creating the external clean record.

Required surface:

```text
python3 scripts/context-envelope-check.py --self-test
python3 scripts/mechanical-verifier-classifier.py --self-test
python3 scripts/review-ready-closure.py --self-test
python3 scripts/derive-worker-review-metadata.py --self-test
python3 scripts/ctx03-quality-replay.py --self-test
python3 scripts/ctx03-process-controls.py --self-test
python3 scripts/ctx03-docsync-history-check.py --self-test
python3 scripts/context-envelope-check.py --audit --base-ref <BASE_SHA> --escalations Docs/evidence/CTX-03/CONTEXT_ESCALATIONS.json
python3 scripts/ctx03-quality-replay.py --negative-controls
python3 scripts/ctx03-process-controls.py
python3 scripts/ctx03-docsync-history-check.py
```

No historical green in this document substitutes for the final exact-SHA rerun or fresh independent semantic review.
