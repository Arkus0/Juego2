# CTX-03 — Control matrix

Status: **CANDIDATE EVIDENCE / PROCESS_ONLY**

| CTX-03 acceptance/control | Source of truth / universe | Control / evidence | Result boundary |
|---|---|---|---|
| same-snapshot pre/post measurement | checker-owned representative routes + checker-owned pre-CTX direct-predecessor inventories + exact candidate bytes | `context-envelope-check.py --audit` | reproducible file-level pre/min/escalated estimates; config/capsules cannot self-shrink baseline |
| canonical profile/calibration universe | checker-owned `Docs/engineering/context-bootstrap-profiles.json` + checker-owned calibration substitutions | `profile_universe_errors` + `ctx03-process-controls.py` redirection mutations | alternate profile source, dropped profile or redirected placeholder -> RED |
| claimed material saving exceeds uncertainty | estimator rule in config/protocol | route report | only PA minimum route is labelled material; H1/CITY are not overstated |
| cumulative PA composition | accepted CTX-02 PA capsule family + checker-owned canonical PA baseline | measurement + `ctx03-quality-replay.py` | keep CTX-02 chain; no second PA registry |
| mandatory escalation completeness | canonical profile-owned `must_escalate_if` | `CONTEXT_ESCALATIONS.json` + `ctx03-process-controls.py` omission mutation | omitted required predicate -> RED |
| stale/compact source cannot override authority | CTX-01/02 accepted authority rules | profile escalation + exact source reconstruction | compact contradiction deepens/fails closed |
| PA material reference cannot disappear silently | real capsule source selectors + canonical PA-03 result | quality replay integration mutation | source removal -> RED |
| CITY non-compressible detail remains reachable | `WP-CITY-03` capsule mandatory read | quality replay integration mutation | seed removal -> RED |
| base process-envelope derived, not duplicated | canonical accepted role profiles | checker derives `initial_reads` | config cannot narrow file/profile universe |
| effective mandatory context bounded | checker-owned H1/CITY/PA routes + checker-owned effective required-source oracle | route minimum+escalated budgets in `context-envelope.json` | route-forced conditional/capsule/escalated source growth over ceiling -> RED |
| H1 foundational conditional cannot escape budget | `H1-worker` checker-owned route | grow real `FOUNDATIONAL_PROOF_STANDARD.md` across route ceiling | RED |
| H1 local conditional cannot escape budget | `H1-reviewer` checker-owned route | grow real `H1_REMOTE_LOCAL_EXECUTION.md` across route ceiling | RED |
| CITY cross-track conditional cannot escape budget | CITY checker-owned routes | grow real `Docs/ROADMAP.md` across route ceiling | RED |
| materially escalated PA source cannot escape budget | cumulative PA checker-owned routes | grow real `PA-03.md` across escalated route ceiling | RED |
| unrelated repository growth does not false-red | base derived profile set | add unrelated 100KB fixture | remains GREEN |
| ceiling growth explicit | base-ref previous config | checker + control fixture | profile or route increase requires revision + justification |
| historical FAIL corpus re-derived/classified | live GitHub review/comment evidence + accepted repo evidence | `HISTORICAL_CLASSIFICATION.json` | each family has mechanical/semantic/mixed + ADOPT/DEFER/REJECT |
| no semantic-gate substitution | independent Reviewer authority | classification explicitly rejects generic semantic-equivalence automation | semantic review remains mandatory |
| exact pre-review does not mutate candidate | final committed HEAD + complete Worker pre-review + durable GitHub issue comment | `derive-worker-review-metadata.py --pre-review-evidence <comment URL>` | repository-local final CLEAN pointer rejected; metadata points to post-byte exact-SHA record |
| unregistered red verifier cannot count WP FAIL | reviewed verifier registry | `mechanical-verifier-classifier.py` self-test | overall INFRA_ERROR, `wp_failed_mechanically=false`, review blocked |
| causal registered failure distinguishable | registered structured outcome + registry validation | classifier self-test | registered causal FAIL -> WP mechanical FAIL; unstructured FAIL-capable registry invalid |
| infra crash not mistaken for WP defect | structured-result requirement | classifier self-test | missing/unclassified result -> INFRA_ERROR |
| N/A not synthetic green proof | registry/outcome vocabulary | classifier self-test | NOT_APPLICABLE neutral |
| CLEAN alone cannot mean review-ready | terminal state contract | `review-ready-closure.py` | Draft/ACTIVE or missing gates -> REVIEW_BLOCKED |
| missing real REVIEW_READY marker cannot false-green | Automation V2 durable marker contract | closure negative control | all prerequisite gates GREEN + no marker -> REVIEW_BLOCKED |
| same-SHA closure retry is reachable | existing durable REVIEW_READY + Candidate Validation workflow completion | closure exact negative→repair→GREEN self-test + workflow contract control | initial freeze RED blocks; later same-SHA GREEN reuses marker and closes |
| post-marker race closed | live PR HEAD after marker | closure moved-HEAD control + automatic closure workflow | moved HEAD -> REVIEW_BLOCKED/no CLOSED marker |
| REVIEW_READY_CLOSED no manual ceremony | bot marker event or later Candidate Validation completion | `.github/workflows/review-ready-closure.yml` | automatic idempotent durable projection only |
| CTX-01 premature/incomplete handoff reproduced | review `#5273364796` + exact candidate lineage | closure handoff-check fixture | incomplete terminal condition -> RED |
| CTX-02 handoff family reproduced | PR #113 durable handoff/marker history | closure handoff-check fixture | missing real predecessor/handoff condition -> RED |
| DocSync compact current state updated once | `ACCEPTED_STATE_INDEX.json` derived CTX row | `ctx03-docsync-history-check.py` | exactly one CTX current-state row, next CTX-03 agreement |
| no contradictory normal bootstrap history | role profiles + CTX/root index/DocSync | DocSync/history checker | history in normal `initial_reads` or contradictory next action -> RED |
| historical closure reconstructible after ROADMAP trim | `Docs/history/CTX_PROCESS_HISTORY.md`, `Docs/history/ROADMAP_ACCEPTED_CLOSURES.md` + exact accepted evidence | DocSync/history checker pointers | representative accepted PR/review/SHA pointers retained; verbose closure not duplicated in current ROADMAP |
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
