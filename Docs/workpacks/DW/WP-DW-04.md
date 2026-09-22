# WP-DW-04 — Structured-context quality and token trial

Status: PLANNED / NOT_STARTED
Class: FOUNDATIONAL
EXECUTION_REQUIREMENT: REMOTE_OK
Depends on: `WP-DW-03` PASS + merge + DocSync; `WP-CTX-03` PASS + merge + DocSync
Blocks: `WP-DW-05`

## Objective and central claim

Demonstrate on a representative acceptance task universe selected under a result-independent frozen partition that DW structured retrieval plus source-open-on-demand can materially reduce supplied context relative to the accepted CTX baseline while preserving actual Worker-like/Reviewer-like agent task correctness: required facts, causal blockers and final verdicts must be recovered by paired executions under the same declared model/configuration.

DW-04 may use a disjoint CTX-only calibration partition to validate the execution instrument before the acceptance freeze. Calibration tasks can never become acceptance tasks, cannot replace acceptance failures and cannot be used to tune DW retrieval against acceptance truth.

## WHY_THIS_BOUNDARY

A faithful typed corpus does not prove that using it improves or preserves agent work. Context efficiency is a different claim with a dangerous vanity metric: a smaller context package can contain the right facts while the agent still fails to notice or use them. This WP therefore separates two necessary proof layers: deterministic context-package/fallback correctness, and mandatory paired agent executions scored by pre-frozen deterministic expected facts/blockers/verdicts. Neither layer may substitute for the other, and quality loss cannot be traded for token savings.

Because model/provider variance can otherwise make a fail-closed 36-execution acceptance trial inconclusive for reasons unrelated to DW, the execution instrument is calibrated on a disjoint partition first. The acceptance tasks remain unseen by route-specific execution until their immutable `TASK_SELECTION_FREEZE` exists.

## Inherited guarantees

Consumes accepted CTX-03 process/context baseline, DW-02 CITY queries, DW-03 PA lossless typed corpus and all repository review obligations. CTX remains owner of process policy.

## New guarantees owned

- an independently reviewable eligible-task universe with a deterministic result-independent partition into a calibration set and an untouched acceptance pool, frozen before any DW-04 route-specific execution;
- calibration tasks are permanently excluded from acceptance and may not be promoted, swapped in or counted toward acceptance after their results are observed;
- a CTX-only calibration phase that may validate provider/model stability, execution budget, machine-readable answer schema and harness/scorer operability without exposing DW retrieval to acceptance tasks;
- an independently reviewable deterministic acceptance selection/stratification rule, or exact reviewed acceptance task manifest, whose resulting six tasks are frozen after calibration but before any acceptance-route execution;
- a durable chronology proving the pre-calibration partition, allowed calibration dimensions and acceptance-selection rule existed before calibration results, and proving the final task prompts, authority anchors, expected fact/blocker/verdict sets, scorer, model/configuration and matched-run policy existed before any acceptance CTX/DW result;
- a frozen representative CITY+PA acceptance task suite selected from the untouched acceptance pool/rule;
- deterministic expected-fact/expected-blocker/expected-verdict oracles for each acceptance fixture wherever the task admits them;
- a baseline route using accepted CTX context/navigation rules;
- a DW route using structured retrieval plus exact source fallback;
- mandatory paired actual agent executions for every frozen acceptance task under the same declared model, model version, system/task prompt, decoding configuration, tool permissions, execution budget and **pre-frozen matched-run policy**; the only intended treatment difference is the CTX-vs-DW context/retrieval route;
- a frozen machine-readable agent answer schema whose scored surface is canonical facts, blockers, verdict and evidence identifiers rather than free-form prose wording;
- deterministic scoring of agent outputs against the pre-frozen expected facts/blockers/verdicts, with model prose never acting as its own oracle;
- a predeclared `INCONCLUSIVE` disposition/restart policy that forbids selective semantic reruns or task replacement after results;
- reproducible context-cost measurement including injected source bytes and, where the execution surface reports it reliably, input-token usage;
- measured quality/context results with no universal extrapolation beyond the frozen fixture universe;
- an explicit fail-closed rule: any demonstrated agent-quality regression or structural context omission fails regardless of token savings.

## Explicitly not re-proved

All CTX implementation correctness, model intelligence in general, universal token savings, PA research truth, CITY design quality or arbitrary future task performance.

## Allowed scope

Trial harness/scripts, pre-calibration universe/partition artifact, CTX-only calibration fixtures, pre-tuning acceptance-selection freeze artifact, frozen acceptance task fixtures, expected-fact/blocker/verdict manifests, deterministic scorer, machine-readable response schema, paired agent execution harness, context assembly, source-open-on-demand flow, measurement/evidence and narrowly necessary retrieval adapters.

## Forbidden scope

Selecting the acceptance tasks because they succeeded during calibration; allowing any calibration task to enter the acceptance suite; executing either CTX or DW against an acceptance task before `TASK_SELECTION_FREEZE`; using DW-route calibration to tune retrieval against acceptance truth; changing acceptance task identities, semantic oracles or scorer semantics after observing acceptance results; cherry-picking only successful tasks or runs; adaptively adding/replacing semantic runs after observing outcomes; using a different model/configuration/tool budget between CTX and DW routes; skipping one side of a required pair; weakening CTX baseline; claiming a universal percentage; scoring semantically correct canonical output as wrong because of free-form wording; using model prose as sole correctness oracle; modifying PA/CITY truth; or modeling review process state inside DW.

## Architecture / authority boundary

DW retrieval selects context; it does not change source authority. When a compact record is insufficient, the route must open the exact accepted source. A retrieval miss cannot be hidden by a plausible final answer. The deterministic context-assembly oracle proves that the supplied material/fallback path is structurally complete; the paired agent layer separately proves that an actual Worker-like/Reviewer-like execution still derives the required answer/verdict from that route. Both are scored against pre-frozen external expected facts/blockers/verdicts.

Calibration validates the execution instrument, not DW quality. The acceptance corpus remains untouched until the final freeze, and the acceptance scorer consumes canonical structured claims rather than prose style.

## Pre-calibration partition and calibration policy

Before any DW-04 route-specific execution, persist a `PRECALIBRATION_FREEZE` that contains:

- the complete eligible historical task universe or independently reviewed exact universe;
- a deterministic result-independent partition rule producing a calibration set and an untouched acceptance pool;
- exact calibration task identities and predeclared calibration run count/order;
- the deterministic rule that will later select the six acceptance tasks from the untouched acceptance pool;
- authority anchors and oracle-construction rules for both partitions;
- the dimensions calibration is allowed to change before acceptance freeze: provider/model/configuration choice, non-semantic prompt/output formatting, execution budget, machine-readable response schema and objective infrastructure-retry policy;
- the dimensions calibration may not change: accepted source truth, acceptance selection rule, task semantic meaning, CTX baseline semantics, DW retrieval semantics, context-reduction threshold or the 100% correctness rule;
- the calibration readiness criterion and proof budget.

Calibration is **CTX-only** and uses only the calibration partition. It may be repeated only within the predeclared calibration proof budget. Before an acceptance freeze can be issued, the chosen execution protocol must satisfy the predeclared CTX calibration readiness criterion on all designated calibration runs. Calibration results may not be used to rank, discard or replace acceptance-pool tasks, because no acceptance task has yet been executed.

After calibration closes, persist `TASK_SELECTION_FREEZE`. From that point the exact acceptance suite and acceptance protocol are immutable unless the entire acceptance trial is invalidated under the restart rule below.

## Frozen machine-readable answer schema

`TASK_SELECTION_FREEZE` must define one canonical machine-readable answer schema for every acceptance execution. At minimum the scored surface contains:

- `facts[]`: canonical expected/observed fact identifiers and values needed by the task;
- `blockers[]`: canonical causal blocker identifiers, including an empty set when none are expected;
- `verdict`: one canonical verdict token from the task manifest;
- `evidence[]`: canonical authority/evidence identifiers required by the manifest.

The deterministic scorer evaluates the structured surface, not writing style. Optional prose, if the execution surface requires it, cannot rescue a missing structured item and cannot cause failure merely because wording differs. Any verdict-changing contradictory structured fact/blocker remains a failure. The schema and normalization rules are frozen before acceptance execution.

## Pre-frozen matched-run policy and decision rule

DW-04 is a bounded fail-closed comparison, not a claim of statistical superiority. The acceptance policy is fixed in `TASK_SELECTION_FREEZE` before any acceptance CTX/DW result observation:

- the acceptance manifest contains **exactly six tasks** unless a reviewed pre-acceptance plan amendment changes that number before `TASK_SELECTION_FREEZE`;
- each task receives **three matched CTX-vs-DW run pairs** (`R1..R3`), for 18 pairs / 36 agent executions across the six-task suite;
- when a provider exposes stable seeds, three seeds are frozen in `TASK_SELECTION_FREEZE` and the same seed is used on both sides of each matched pair; when stable seeds are unavailable, three run identities/order slots are frozen and both routes use the same declared model/configuration/tool/execution budget;
- route order within each matched pair is frozen before execution and may be interleaved to reduce temporal/provider drift; it may not be changed after observing an outcome;
- every individual run is scored against the same pre-frozen required facts/blockers/verdicts. No majority vote, best-of-three, averaging or post-hoc run exclusion can turn an oracle miss into a PASS;
- a matched pair is `PAIR_PASS` only when **both** CTX and DW runs satisfy all required expected facts/blockers/verdicts and neither introduces a verdict-changing false fact/blocker;
- any pair where CTX satisfies the oracle and DW misses a required item, emits a wrong required verdict, or introduces a verdict-changing false fact/blocker is a demonstrated DW regression and makes the WP `FAIL`;
- if no demonstrated DW regression exists but one or more designated scorable runs fail because CTX itself is unstable, or both routes fail in a way that prevents the preservation claim from being established, the WP is `INCONCLUSIVE`, not PASS;
- objective provider/transport/tool/harness failure that produces no scorable structured answer is `RUN_INVALID`, not a semantic miss. A replacement is allowed only if `TASK_SELECTION_FREEZE` already contains a bounded objective invalid-run retry rule; semantic oracle misses can never trigger that retry rule;
- `PASS` requires all designated scorable runs for all six tasks to be `PAIR_PASS`, structural context correctness to remain GREEN, and the context-reduction criterion below to be met;
- `INCONCLUSIVE` does not authorize `WP-DW-05` and cannot be converted to PASS by adding selective runs;
- the acceptance execution budget above is the default `PROOF_BUDGET`. Any increase in task count or semantic run count requires reviewed amendment **before** acceptance execution and becomes part of a new immutable freeze.

## Predeclared `INCONCLUSIVE` disposition and restart rule

An acceptance `INCONCLUSIVE` has only the following allowed dispositions:

1. **STOP:** preserve the trial evidence and keep `WP-DW-05` blocked.
2. **OBJECTIVE_INVALID-RUN COMPLETION:** complete only replacements explicitly permitted by the already-frozen `RUN_INVALID` retry rule. This route is unavailable for any scorable oracle miss.
3. **ONE REVIEWED FULL RESTART:** an independent review may authorize one complete restart only after the observed cause is classified and a revised execution protocol first demonstrates its correction on the calibration partition. The restart must preserve the same six acceptance task identities, accepted authority anchors, semantic expected facts/blockers/verdicts, scorer semantics, 30% context-reduction threshold and 100% correctness rule. Only operational dimensions already declared calibratable in `PRECALIBRATION_FREEZE` may change. All 36 acceptance executions are rerun from scratch under one new freeze; no successful prior pair is carried forward and no failed task is replaced.

A second semantic/baseline `INCONCLUSIVE` after that full restart blocks further adaptive attempts under the existing DW-04 plan. Any additional attempt requires a new plan-level amendment reviewed before further route execution. This prevents repeated restarts from becoming a statistical search for a favorable outcome.

A task identity may be replaced only if an independent reviewer proves the task/oracle itself contradicts accepted source truth or is mechanically unscorable for a reason independent of the observed model outcome. Such a replacement requires a plan amendment and deterministic reselection from the untouched original acceptance pool; it is not an `INCONCLUSIVE` convenience path.

## Acceptance criteria

- before any route-specific execution, freeze the eligible task universe, deterministic calibration/acceptance partition rule, exact calibration tasks, untouched acceptance pool, acceptance selection rule, oracle-construction rules, allowed calibration dimensions and calibration readiness criterion;
- calibration tasks and acceptance-pool tasks are disjoint, and calibration tasks can never count toward or replace acceptance tasks;
- CTX-only calibration closes under its predeclared readiness criterion before `TASK_SELECTION_FREEZE` is issued;
- after calibration, select exactly six representative acceptance tasks from the untouched acceptance pool using the predeclared rule unless a reviewed pre-acceptance amendment changes the count, spanning both CITY and PA and including Worker-like retrieval/composition plus Reviewer-like contradiction/omission detection;
- every acceptance task has a pre-frozen expected fact set and, where meaningful, expected blocker/verdict set derived from accepted historical/source truth before either acceptance route is executed;
- the machine-readable answer schema, normalization rules, deterministic scorer, exact model/configuration, run identities/seeds, route order and objective invalid-run retry rule are frozen before acceptance execution;
- the baseline uses the accepted CTX-03 route rather than an intentionally bloated legacy strawman;
- the DW route begins from structured queries and opens accepted sources only when required by the task/provenance policy;
- deterministic route assembly for both routes recovers 100% of required expected facts/source-fallback reachability before any model execution; any missing required item is FAIL;
- every frozen acceptance task receives the three predesignated paired actual CTX and DW agent executions; no task may PASS on context-package completeness alone;
- each CTX/DW pair uses the same declared model/version, system/task prompt, decoding settings, tool permissions, execution budget and pre-frozen run identity/seed policy. When a provider offers a stable seed, it must be equal across the pair; when it does not, the limitation is recorded and the same predeclared matched-run policy is used for both routes;
- the deterministic scorer evaluates each agent's structured output against the pre-frozen expected facts/blockers/verdicts. `PASS` requires all designated CTX and DW runs to achieve 100% of required expected facts and causal expected blockers/verdicts; any CTX-pass/DW-fail matched pair is `FAIL`, while unresolved shared/baseline instability is `INCONCLUSIVE` under the pre-frozen rule above;
- semantically equivalent structured answers cannot fail because of prose wording or ordering differences covered by the frozen normalization rules;
- DW agent outputs introduce no new false blocker/fact that changes the expected verdict;
- median injected source-context bytes are at least 30% lower for the DW route across the frozen acceptance suite; provider-reported input tokens are recorded when stable/available but do not replace the deterministic byte metric;
- per-task context accounting records which source fragments/documents were injected/opened so savings are auditable;
- repeated route assembly over the same accepted anchors returns equal selected record/source sets before model execution;
- trial results report calibration evidence, structural context correctness, actual paired-agent correctness and context-cost measurements separately so one cannot mask failure in another;
- any `INCONCLUSIVE` disposition follows the predeclared restart rule above rather than a result-specific ad hoc decision.

## Deterministic proof / evidence

Use previously resolved accepted CITY/PA cases so hidden truth is known without inventing new semantic review.

First persist `PRECALIBRATION_FREEZE` with the eligible universe, result-independent calibration/acceptance partition, exact calibration tasks/run budget, untouched acceptance pool, deterministic acceptance-selection rule, authority anchors, oracle-construction rules, calibratable/non-calibratable dimensions and readiness criterion. Execute CTX-only calibration and freeze the chosen execution protocol only after that criterion is met.

Then persist `TASK_SELECTION_FREEZE` containing the exact six acceptance tasks selected mechanically from the untouched pool, authority anchors, task prompts, expected facts/blockers/verdicts, deterministic scoring rubric, canonical structured answer schema and normalization rules, three matched run identities/seeds and route order per task, objective invalid-run retry rule, decision rule above, `INCONCLUSIVE` disposition/restart rule, `PROOF_BUDGET`, and exact model/configuration constraints. No acceptance CTX or DW task may execute before this artifact exists.

Run deterministic context assembly first and score its selected source/record/fallback set against the frozen expected material. Then execute the mandatory paired agents for every acceptance task under the same declared model/configuration and score their structured outputs mechanically against the frozen expected facts/blockers/verdicts. Model prose is execution evidence, not the oracle; the oracle is the independently frozen expected/scoring manifest. Provider usage and byte/token accounting are recorded separately from correctness scoring.

## Causal negative-conformance classes

- a calibration task is promoted into the acceptance suite after its CTX result is known: protocol must FAIL;
- an acceptance-pool task is executed during calibration and later retained as acceptance: chronology/protocol must FAIL;
- calibration results are used to replace/rank acceptance tasks rather than applying the predeclared selection rule: protocol must FAIL;
- one required expected fact is removed from the DW retrieval output while context gets smaller: structural trial and downstream paired-agent scoring must FAIL despite savings;
- one compact record loses provenance so exact source fallback cannot open the authority;
- one negative/rejected PA disposition is filtered as irrelevant and expected verdict changes;
- baseline is artificially inflated with unrelated documents and measurement guard detects protocol violation;
- exact acceptance task manifest, expected fact/blocker/verdict set, scorer semantics, run count, run identity/seed or route order is changed after acceptance results begin without invalidating/restarting the entire trial under the allowed rule;
- one required CTX or DW agent execution is skipped while deterministic context assembly remains GREEN: WP must FAIL or become `INCONCLUSIVE` only when the predeclared objective `RUN_INVALID` rule applies;
- CTX and DW paired executions use different model/version, decoding settings, tool permissions, execution budget or run-count policy: protocol guard must FAIL;
- one designated pair is CTX-pass/DW-fail while the other two pairs pass: the WP must still FAIL; a 2/3 majority cannot hide the demonstrated regression;
- baseline/shared model instability causes a designated scorable oracle miss without a demonstrated DW regression: the result must be `INCONCLUSIVE`, not silently rerun, excluded or averaged into PASS;
- an `INCONCLUSIVE` semantic miss triggers selective reruns or replacement of only the failed task: protocol must FAIL;
- a full restart carries forward successful old pairs instead of rerunning all 36 under one new freeze: protocol must FAIL;
- a calibration/acceptance answer with semantically correct canonical fields is rejected only because free-form prose differs: scorer conformance must FAIL;
- scorer omits one pre-frozen expected blocker/verdict and would otherwise mark an incorrect agent answer GREEN: scorer conformance must FAIL;
- token/byte measurement excludes source fallback opened by the DW route;
- nondeterministic retrieval returns different source sets for the same accepted anchors.

## Content-shape probe

Required. Freeze a disjoint calibration/acceptance partition before any DW-04 execution. Calibration must include both CITY and PA pressure sufficient to exercise the execution instrument but its tasks can never enter acceptance. The acceptance suite contains exactly six tasks selected from the untouched predeclared pool/rule unless a reviewed pre-acceptance amendment changes the count, with both CITY and PA represented and both retrieval/composition and omission/contradiction review pressure. The acceptance suite must include at least one known cross-document CITY inheritance case and one PA disposition/fixture preservation case. Every selected acceptance task participates in all three mandatory paired CTX-vs-DW agent runs and deterministic scoring protocol.

## Dependency / IP implications

No new external content dependency is expected. If a provider/tokenizer is used for calibration, agent execution or supplemental token accounting, exact model/provider/version/configuration is recorded. Byte accounting remains the vendor-neutral primary context-cost measure; deterministic expected/scoring manifests remain the correctness oracle.

## Residual risks

The fixture suite cannot prove all future agents/tasks improve, and a 30% median reduction is not a universal promise. Three matched pairs per task are a bounded fail-closed conformance check, not a statistically powered estimate of model performance. Calibration reduces avoidable execution-instrument instability but cannot eliminate model variance and cannot guarantee that the frozen acceptance suite will PASS. Unresolved baseline/shared instability therefore still yields `INCONCLUSIVE`; the predeclared restart rule prevents that limitation from turning into adaptive rerun/cherry-picking pressure. The claim remains bounded to the frozen acceptance suite, model/configuration and run policy actually executed. Structural context completeness and actual agent performance are reported separately.

## Exact predecessor reopen condition

Reopen DW-03/DW-02 only if the trial exposes a source-projection/retrieval defect in their owned domain. Reopen CTX-03 only if effective evidence contradicts its accepted baseline guarantee rather than merely showing DW is more efficient. A bad task-selection/scoring/execution protocol is DW-04-owned and cannot be routed to predecessors.

## PASS consequence / next dependency

PASS permits `WP-DW-05`. `INCONCLUSIVE` does not. PASS proves bounded project value for structured retrieval under the exact frozen acceptance task universe, matched model/configuration and scoring protocol: context reduction plus preserved actual agent task/review correctness. Calibration evidence proves only execution-instrument readiness and cannot substitute for acceptance. PASS does not authorize replacing normal source review, deleting accepted documents or generalizing the measured result to other models/tasks.
