# WP-DW-04 — Structured-context quality and token trial

Status: PLANNED / NOT_STARTED
Class: FOUNDATIONAL
EXECUTION_REQUIREMENT: REMOTE_OK
Depends on: `WP-DW-03` PASS + merge + DocSync; `WP-CTX-03` PASS + merge + DocSync
Blocks: `WP-DW-05`

## Objective and central claim

Demonstrate on a representative task universe and selection rule frozen **before any route-specific tuning or result observation** that DW structured retrieval plus source-open-on-demand can materially reduce supplied context relative to the accepted CTX baseline while preserving actual Worker-like/Reviewer-like agent task correctness: required facts, causal blockers and final verdicts must be recovered by paired executions under the same declared model/configuration.

## WHY_THIS_BOUNDARY

A faithful typed corpus does not prove that using it improves or preserves agent work. Context efficiency is a different claim with a dangerous vanity metric: a smaller context package can contain the right facts while the agent still fails to notice or use them. This WP therefore separates two necessary proof layers: deterministic context-package/fallback correctness, and mandatory paired agent executions scored by pre-frozen deterministic expected facts/blockers/verdicts. Neither layer may substitute for the other, and quality loss cannot be traded for token savings.

## Inherited guarantees

Consumes accepted CTX-03 process/context baseline, DW-02 CITY queries, DW-03 PA lossless typed corpus and all repository review obligations. CTX remains owner of process policy.

## New guarantees owned

- an independently reviewable eligible-task universe plus deterministic selection/stratification rule, or an exact reviewed task manifest, frozen before any DW-04 route-specific tuning/result observation;
- a durable selection-freeze anchor proving chronology: task universe/selection rule, task prompts, authority anchors, expected fact/blocker/verdict sets and scoring rules exist before comparison-route tuning/results;
- a frozen representative CITY+PA task suite selected from that predeclared universe/rule;
- deterministic expected-fact/expected-blocker/expected-verdict oracles for each fixture wherever the task admits them;
- a baseline route using accepted CTX context/navigation rules;
- a DW route using structured retrieval plus exact source fallback;
- mandatory paired actual agent executions for every frozen task under the same declared model, model version, system/task prompt, decoding configuration, tool permissions, execution budget and **pre-frozen matched-run policy**; the only intended treatment difference is the CTX-vs-DW context/retrieval route;
- deterministic scoring of agent outputs against the pre-frozen expected facts/blockers/verdicts, with model prose never acting as its own oracle;
- reproducible context-cost measurement including injected source bytes and, where the execution surface reports it reliably, input-token usage;
- measured quality/context results with no universal extrapolation beyond the frozen fixture universe;
- an explicit fail-closed rule: any demonstrated agent-quality regression or structural context omission fails regardless of token savings.

## Explicitly not re-proved

All CTX implementation correctness, model intelligence in general, universal token savings, PA research truth, CITY design quality or arbitrary future task performance.

## Allowed scope

Trial harness/scripts, pre-tuning task-universe/selection-freeze artifact, frozen task fixtures, expected-fact/blocker/verdict manifests, deterministic scorer, paired agent execution harness, context assembly, source-open-on-demand flow, measurement/evidence and narrowly necessary retrieval adapters.

## Forbidden scope

Selecting or changing the eligible task universe/selection rule after route-specific tuning/results begin; changing task oracles/scoring after seeing results; cherry-picking only successful tasks or runs; adaptively adding/replacing runs after observing outcomes; using a different model/configuration/tool budget between CTX and DW routes; skipping one side of a required pair; weakening CTX baseline; claiming a universal percentage; using model prose as sole correctness oracle; modifying PA/CITY truth; or modeling review process state inside DW.

## Architecture / authority boundary

DW retrieval selects context; it does not change source authority. When a compact record is insufficient, the route must open the exact accepted source. A retrieval miss cannot be hidden by a plausible final answer. The deterministic context-assembly oracle proves that the supplied material/fallback path is structurally complete; the paired agent layer separately proves that an actual Worker-like/Reviewer-like execution still derives the required answer/verdict from that route. Both are scored against pre-frozen external expected facts/blockers/verdicts.

## Pre-frozen matched-run policy and decision rule

DW-04 is a bounded fail-closed comparison, not a claim of statistical superiority. The acceptance policy is fixed before route-specific tuning or result observation:

- the acceptance manifest contains **exactly six tasks** unless a reviewed pre-tuning plan amendment changes that number;
- each task receives **three matched CTX-vs-DW run pairs** (`R1..R3`), for 18 pairs / 36 agent executions across the six-task suite;
- when a provider exposes stable seeds, three seeds are frozen in `TASK_SELECTION_FREEZE` and the same seed is used on both sides of each matched pair; when stable seeds are unavailable, three run identities/order slots are frozen and both routes use the same declared model/configuration/tool/execution budget;
- route order within each matched pair is frozen before execution and may be interleaved to reduce temporal/provider drift; it may not be changed after observing an outcome;
- every individual run is scored against the same pre-frozen required facts/blockers/verdicts. No majority vote, best-of-three, averaging or post-hoc run exclusion can turn an oracle miss into a PASS;
- a matched pair is `PAIR_PASS` only when **both** CTX and DW runs satisfy all required expected facts/blockers/verdicts and neither introduces a verdict-changing false fact/blocker;
- any pair where CTX satisfies the oracle and DW misses a required item, emits a wrong required verdict, or introduces a verdict-changing false fact/blocker is a demonstrated DW regression and makes the WP `FAIL`;
- if no demonstrated DW regression exists but one or more designated runs fail because CTX itself is unstable, or both routes fail in a way that prevents the preservation claim from being established, the WP is `INCONCLUSIVE`, not PASS;
- `PASS` requires all designated runs for all six tasks to be `PAIR_PASS`, structural context correctness to remain GREEN, and the context-reduction criterion below to be met;
- `INCONCLUSIVE` does not authorize `WP-DW-05`. It requires an explicit reviewed disposition or a new pre-tuning freeze; simply adding more runs until results look favorable is forbidden;
- the acceptance execution budget above is the default `PROOF_BUDGET`. Any increase in task count or run count must be justified and reviewed **before** route-specific tuning/results and becomes part of a new immutable freeze.

## Acceptance criteria

- before any route-specific tuning/result observation, freeze either (a) the complete eligible historical task universe plus deterministic selection/stratification rule or (b) the exact independently reviewed task manifest; the freeze must be anchored durably so chronology is reviewable;
- select exactly six representative tasks from that predeclared universe/rule unless a reviewed pre-tuning amendment changes the count, spanning both CITY and PA and including Worker-like retrieval/composition plus Reviewer-like contradiction/omission detection;
- every task has a pre-frozen expected fact set and, where meaningful, expected blocker/verdict set derived from accepted historical/source truth before either route is executed;
- the baseline uses the accepted CTX-03 route rather than an intentionally bloated legacy strawman;
- the DW route begins from structured queries and opens accepted sources only when required by the task/provenance policy;
- deterministic route assembly for both routes recovers 100% of required expected facts/source-fallback reachability before any model execution; any missing required item is FAIL;
- every frozen task receives the three predesignated paired actual CTX and DW agent executions; no task may PASS on context-package completeness alone;
- each CTX/DW pair uses the same declared model/version, system/task prompt, decoding settings, tool permissions, execution budget and pre-frozen run identity/seed policy. When a provider offers a stable seed, it must be equal across the pair; when it does not, the limitation is recorded and the same predeclared matched-run policy is used for both routes;
- the deterministic scorer evaluates each agent output against the pre-frozen expected facts/blockers/verdicts. `PASS` requires all designated CTX and DW runs to achieve 100% of required expected facts and causal expected blockers/verdicts; any CTX-pass/DW-fail matched pair is `FAIL`, while unresolved shared/baseline instability is `INCONCLUSIVE` under the pre-frozen rule above;
- DW agent outputs introduce no new false blocker/fact that changes the expected verdict;
- median injected source-context bytes are at least 30% lower for the DW route across the frozen suite; provider-reported input tokens are recorded when stable/available but do not replace the deterministic byte metric;
- per-task context accounting records which source fragments/documents were injected/opened so savings are auditable;
- repeated route assembly over the same accepted anchors returns equal selected record/source sets before model execution;
- trial results report structural context correctness, actual paired-agent correctness and context-cost measurements separately so one cannot mask failure in another.

## Deterministic proof / evidence

Use previously resolved accepted CITY/PA cases so hidden truth is known without inventing new semantic review. Before route-specific tuning, persist a `TASK_SELECTION_FREEZE` artifact containing the eligible-task universe or exact reviewed manifest, selection/stratification rule, authority anchors, task prompts, expected facts/blockers/verdicts, deterministic scoring rubric, the exact six-task acceptance manifest, three matched run identities/seeds and route order per task, decision rule above, `PROOF_BUDGET`, and model/configuration constraints. The artifact is immutable for the acceptance comparison; changing it requires invalidating prior DW-04 tuning/results and restarting the trial from a new pre-tuning freeze.

Run deterministic context assembly first and score its selected source/record/fallback set against the frozen expected material. Then execute the mandatory paired agents for every task under the same declared model/configuration and score their outputs mechanically against the frozen expected facts/blockers/verdicts. Model prose is execution evidence, not the oracle; the oracle is the independently frozen expected/scoring manifest. Provider usage and byte/token accounting are recorded separately from correctness scoring.

## Causal negative-conformance classes

- one required expected fact is removed from the DW retrieval output while context gets smaller: structural trial and downstream paired-agent scoring must FAIL despite savings;
- one compact record loses provenance so exact source fallback cannot open the authority;
- one negative/rejected PA disposition is filtered as irrelevant and expected verdict changes;
- baseline is artificially inflated with unrelated documents and measurement guard detects protocol violation;
- eligible task universe, deterministic selection rule, exact task manifest, expected fact/blocker/verdict set, scoring rule, run count, run identity/seed or route order is changed after route-specific tuning/results begin without invalidating/restarting the trial;
- one required CTX or DW agent execution is skipped while deterministic context assembly remains GREEN: WP must FAIL;
- CTX and DW paired executions use different model/version, decoding settings, tool permissions, execution budget or run-count policy: protocol guard must FAIL;
- one designated pair is CTX-pass/DW-fail while the other two pairs pass: the WP must still FAIL; a 2/3 majority cannot hide the demonstrated regression;
- baseline/shared model instability causes a designated oracle miss without a demonstrated DW regression: the result must be `INCONCLUSIVE`, not silently rerun, excluded or averaged into PASS;
- scorer omits one pre-frozen expected blocker/verdict and would otherwise mark an incorrect agent answer GREEN: scorer conformance must FAIL;
- token/byte measurement excludes source fallback opened by the DW route;
- nondeterministic retrieval returns different source sets for the same accepted anchors.

## Content-shape probe

Required. Exactly six tasks selected from the pre-tuning frozen universe/rule unless a reviewed pre-tuning amendment changes the count, with both CITY and PA represented and both retrieval/composition and omission/contradiction review pressure. The suite must include at least one known cross-document CITY inheritance case and one PA disposition/fixture preservation case. Every selected task participates in all three mandatory paired CTX-vs-DW agent runs and deterministic scoring protocol.

## Dependency / IP implications

No new external content dependency is expected. If a provider/tokenizer is used for agent execution or supplemental token accounting, exact model/provider/version/configuration is recorded. Byte accounting remains the vendor-neutral primary context-cost measure; deterministic expected/scoring manifests remain the correctness oracle.

## Residual risks

The fixture suite cannot prove all future agents/tasks improve, and a 30% median reduction is not a universal promise. Three matched pairs per task are a bounded fail-closed conformance check, not a statistically powered estimate of model performance. Model variance remains a limitation even under matched configuration; unresolved baseline/shared instability therefore yields `INCONCLUSIVE` rather than permitting adaptive reruns or a weak PASS. The claim remains bounded to the frozen suite, model/configuration and run policy actually executed. Structural context completeness and actual agent performance are reported separately.

## Exact predecessor reopen condition

Reopen DW-03/DW-02 only if the trial exposes a source-projection/retrieval defect in their owned domain. Reopen CTX-03 only if effective evidence contradicts its accepted baseline guarantee rather than merely showing DW is more efficient. A bad task-selection/scoring/execution protocol is DW-04-owned and cannot be routed to predecessors.

## PASS consequence / next dependency

PASS permits `WP-DW-05`. `INCONCLUSIVE` does not. PASS proves bounded project value for structured retrieval under the exact frozen task universe, matched model/configuration and scoring protocol: context reduction plus preserved actual agent task/review correctness. It does not authorize replacing normal source review, deleting accepted documents or generalizing the measured result to other models/tasks.
