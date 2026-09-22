# CTX-03 — False-red and causal control audit

Status: **CANDIDATE EVIDENCE / PROCESS_ONLY**

## Goal

Move deterministic protocol defects left without reducing false-green detection power or semantic Reviewer authority. Mechanical PASS never substitutes for source reconstruction or independent review; unknown/infra red also cannot be silently ignored.

## Mechanical outcome contract

| Outcome | Meaning | Reviewer may start? |
|---|---|---:|
| `PASS` | registered deterministic condition satisfied | yes, if all other terminal predicates close |
| `FAIL` | registered deterministic verifier causally proves its owned repository condition false | no; Worker repairs first |
| `REVIEW_BLOCKED` | lifecycle/handoff/freeze state incomplete or incoherent | no |
| `NOT_APPLICABLE` | verifier is neutral for this candidate/mode | neutral |
| `INFRA_ERROR` | runner/API/tool/unregistered result cannot prove a candidate defect | no; triage/rerun |

`mechanical-verifier-registry.json` is process classification only. It cannot claim semantic authority.

## Exact-candidate pre-review ordering

The final sequence is non-self-referential:

1. finish and push every repository/evidence byte while Draft + ACTIVE;
2. stop writers and read exact HEAD;
3. run the complete Worker pre-review on that HEAD and the complete baseline→candidate diff;
4. only if clean, persist `WORKER_PRE_REVIEW: CLEAN` as a durable GitHub PR issue comment bound to that exact SHA;
5. derive/freeze metadata without mutating repository bytes.

`derive-worker-review-metadata.py` rejects repository-local final CLEAN pointers. `validate-worker-handoff.py` resolves the external comment and proves same repository, same PR and exact candidate SHA. Any later repository/evidence mutation invalidates CLEAN.

## Four-layer mandatory-context envelope

CTX-03 now uses four non-substitutable layers:

1. **base profile** — fixed canonical `initial_reads`;
2. **fixed conditional profile** — base plus checker-owned fixed repository conditionals for every canonical profile;
3. **representative route-effective** — H1/CITY/PA minimum/escalated quality/calibration routes;
4. **dynamic repository envelope** — exact route-dependent repository context for arbitrary current/future WPs and roles.

The fourth layer closes B2. `ctx03-dynamic-context-check.py` owns reviewed dynamic slot classification, independently discovers all workpack contracts, counts exact contracts/direct dependencies/contract-mandatory repository inputs, handles repository-vs-external repair evidence, and derives manifest-named local-executor files. Every resolved repository source is bounded by a per-source ceiling and every resolved dynamic route by an aggregate ceiling. New placeholder classes fail closed pending explicit review; policy ceiling increases require revision + justification.

Genuinely external live GitHub/API/diff payloads may remain outside repository corpus budgets. Repository-backed inputs do not escape merely because their identity is dynamic.

`ctx03-dynamic-slot-controls.py` independently scans both `initial_reads` and `conditional_reads`, so a future placeholder cannot hide in a conditional surface that the current profiles did not previously use.

The global resolver deliberately does **not** promote every repository path mentioned in prose into mandatory context. An intermediate implementation false-red on future output paths, donor-only references and historical evidence mentions. Mandatory context is derived from routing/contract semantics; incidental/output/history prose does not acquire semantic authority.

## Causal controls

| Claim | Negative challenge | Expected |
|---|---|---|
| base ceiling | grow real unconditional source | RED |
| fixed conditional completeness | add new fixed repository path outside checker-owned set | RED |
| fixed conditional growth | grow every checker-owned fixed source for every profile | RED |
| representative routes | grow H1/CITY/PA route-forced source | RED |
| future arbitrary WP | invent non-H1/CITY/PA exact WP | automatically counted |
| future exact WP growth | grow invented WP past per-source ceiling | RED |
| new mandatory repo evidence | add contract-required source | automatically counted |
| caller omission | omit source from binding while contract still requires it | still counted |
| manifest-named repo input | omit caller-maintained named-file list | manifest still supplies source |
| future role | reuse reviewed exact-WP slot under new role | same dynamic resolver applies |
| new dynamic slot | add unknown placeholder in initial/conditional read surface | RED pending oracle review |
| unrelated repository growth | grow non-mandatory file | GREEN |
| structured escalation omission | remove field / `[]` / `{}` | RED |
| current-state projection omission | remove/empty derived CTX index row | RED while authoritative discovery remains intact |
| PA/CITY source reachability | remove material capsule selector/read | quality replay RED |
| external CLEAN identity | nonexistent/wrong PR/wrong SHA comment | handoff RED |
| same-SHA lifecycle retry | marker exists + gate RED → same SHA gate repaired | blocked → closure reachable |
| post-marker HEAD move | mutate HEAD after marker | blocked |

## Retry-safe REVIEW_READY closure

`.github/workflows/review-ready-closure.yml` wakes both from the original bot `REVIEW_READY` comment and from a later `Arkus Candidate Validation` completion. Therefore an existing durable marker can be reused after same-SHA metadata/gate repair. Closure is idempotent by PR+SHA and remains lifecycle evidence only.

## Exact final validation surface

After the **last repository/evidence mutation**, rerun on the exact HEAD:

```text
python3 scripts/context-envelope-check.py --self-test
python3 scripts/ctx03-dynamic-context-check.py --self-test
python3 scripts/ctx03-dynamic-slot-controls.py --self-test
python3 scripts/mechanical-verifier-classifier.py --self-test
python3 scripts/review-ready-closure.py --self-test
python3 scripts/derive-worker-review-metadata.py --self-test
python3 scripts/validate-worker-handoff.py --self-test
python3 scripts/ctx03-quality-replay.py --self-test
python3 scripts/ctx03-process-controls.py --self-test
python3 scripts/ctx03-docsync-history-check.py --self-test
python3 scripts/context-envelope-check.py --audit --base-ref <BASE_SHA> --escalations Docs/evidence/CTX-03/CONTEXT_ESCALATIONS.json
python3 scripts/ctx03-dynamic-context-check.py --audit-policy --base-ref <BASE_SHA>
python3 scripts/ctx03-dynamic-slot-controls.py
python3 scripts/ctx03-quality-replay.py --negative-controls
python3 scripts/ctx03-process-controls.py
python3 scripts/ctx03-docsync-history-check.py
python3 scripts/ctx03-final-circuit-breaker.py
```

Accepted CTX-02 capsule validation remains separately binding. No historical GREEN substitutes for the final exact-SHA rerun or fresh independent semantic review.
