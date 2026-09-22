# CTX-03 — Control matrix

Status: **CANDIDATE EVIDENCE / PROCESS_ONLY**

| CTX-03 acceptance/control | Source of truth / universe | Control / evidence | Result boundary |
|---|---|---|---|
| same-snapshot pre/post measurement | `context-envelope.json` routes + files from exact candidate snapshot | `context-envelope-check.py --audit` | reproducible file-level pre/min/escalated estimates; no cross-SHA comparison |
| claimed material saving exceeds uncertainty | estimator rule in config/protocol | route report | only PA minimum route is labelled material; H1/CITY are not overstated |
| cumulative PA composition | accepted CTX-02 PA capsule family + canonical PA results | measurement + `ctx03-quality-replay.py` | keep CTX-02 chain; no second PA registry |
| mandatory escalation completeness | profile-owned `must_escalate_if` | `CONTEXT_ESCALATIONS.json` + `ctx03-process-controls.py` omission mutation | omitted required predicate -> RED |
| stale/compact source cannot override authority | CTX-01/02 accepted authority rules | profile escalation + exact source reconstruction | compact contradiction deepens/fails closed |
| PA material reference cannot disappear silently | real capsule source selectors + canonical PA-03 result | quality replay integration mutation | source removal -> RED |
| CITY non-compressible detail remains reachable | `WP-CITY-03` capsule mandatory read | quality replay integration mutation | seed removal -> RED |
| process-envelope derived, not duplicated | accepted `context-bootstrap-profiles.json` | checker derives `initial_reads` | config cannot narrow file universe |
| required-context growth bounded | derived profile set + calibrated baseline/ceiling | `ctx03-process-controls.py` | real required-source growth over ceiling -> RED |
| unrelated repository growth does not false-red | same derived profile set | add unrelated 100KB fixture | remains GREEN |
| ceiling growth explicit | base-ref previous config | checker + control fixture | increase requires revision + justification |
| historical FAIL corpus re-derived/classified | live GitHub review/comment evidence + accepted repo evidence | `HISTORICAL_CLASSIFICATION.json` | each family has mechanical/semantic/mixed + ADOPT/DEFER/REJECT |
| no semantic-gate substitution | independent Reviewer authority | classification explicitly rejects generic semantic-equivalence automation | semantic review remains mandatory |
| derivable metadata before Reviewer | final HEAD + predecessor check + CLEAN evidence | `derive-worker-review-metadata.py` + existing handoff lint | missing prerequisites -> REVIEW_BLOCKED; generator cannot mint CLEAN/PASS |
| unregistered red verifier cannot count WP FAIL | reviewed verifier registry | `mechanical-verifier-classifier.py` self-test | overall INFRA_ERROR, `wp_failed_mechanically=false`, review blocked |
| causal registered failure distinguishable | verifier structured outcome | classifier self-test | registered causal FAIL -> WP mechanical FAIL |
| infra crash not mistaken for WP defect | structured-result requirement | classifier self-test | missing/unclassified result -> INFRA_ERROR |
| N/A not synthetic green proof | registry/outcome vocabulary | classifier self-test | NOT_APPLICABLE neutral |
| CLEAN alone cannot mean review-ready | terminal state contract | `review-ready-closure.py` | Draft/ACTIVE or missing gates -> REVIEW_BLOCKED |
| missing real REVIEW_READY marker cannot false-green | Automation V2 durable marker contract | closure negative control | all prerequisite gates GREEN + no marker -> REVIEW_BLOCKED |
| post-marker race closed | live PR HEAD after marker | closure moved-HEAD control + automatic closure workflow | moved HEAD -> REVIEW_BLOCKED/no CLOSED marker |
| REVIEW_READY_CLOSED no manual ceremony | issue-comment trigger on existing bot marker | `.github/workflows/review-ready-closure.yml` | automatic durable projection only |
| CTX-01 premature/incomplete handoff reproduced | review `#5273364796` + exact candidate lineage | closure handoff-check fixture | incomplete terminal condition -> RED |
| CTX-02 handoff family reproduced | PR #113 durable handoff/marker history | closure handoff-check fixture | missing real predecessor/handoff condition -> RED |
| DocSync compact current state updated once | `ACCEPTED_STATE_INDEX.json` derived CTX row | `ctx03-docsync-history-check.py` | exactly one CTX current-state row, next CTX-03 agreement |
| no contradictory normal bootstrap history | role profiles + CTX/root index/DocSync | DocSync/history checker | history in normal `initial_reads` or contradictory next action -> RED |
| historical closure reconstructible | `Docs/history/CTX_PROCESS_HISTORY.md` + exact accepted evidence | DocSync/history checker pointers | accepted PR/review/SHA pointers retained |
| future consumer reusable primitives | repository-generic scripts/config contracts | protocol boundary | no H2/H3 ownership decision in CTX-03 |

## Exact canonical validation surface for final candidate

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

The PR workflow runs the same CTX-03 causal surface and classifies its structured result. Accepted CTX-02 capsule validation remains independently required/unchanged whenever capsule/index state is affected; CTX-03 does not replace it.
