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
- mandatory paired actual agent executions for every frozen task under the same declared model, model version, system/task prompt, decoding configuration, tool permissions, execution budget and run-count policy; the only intended treatment difference is the CTX-vs-DW context/retrieval route;
- deterministic scoring of agent outputs against the pre-frozen expected facts/blockers/verdicts, with model prose never acting as its own oracle;
- reproducible context-cost measurement including injected source bytes and, where the execution surface reports it reliably, input-token usage;
- measured quality/context results with no universal extrapolation beyond the frozen fixture universe;
- an explicit fail-closed rule: any demonstrated agent-quality regression or structural context omission fails regardless of token savings.

## Explicitly not re-proved

All CTX implementation correctness, model intelligence in general, universal token savings, PA research truth, CITY design quality or arbitrary future task performance.

## Allowed scope

Trial harness/scripts, pre-tuning task-universe/selection-freeze artifact, frozen task fixtures, expected-fact/blocker/verdict manifests, deterministic scorer, paired agent execution harness, context assembly, source-open-on-demand flow, measurement/evidence and narrowly necessary retrieval adapters.

## Forbidden scope

Selecting or changing the eligible task universe/selection rule after route-specific tuning/results begin; changing task oracles/scoring after seeing results; cherry-picking only successful tasks or runs; using a different model/configuration/tool budget between CTX and DW routes; skipping one side of a required pair; weakening CTX baseline; claiming a universal percentage; using model prose as sole correctness oracle; modifying PA/CITY truth; or modeling review process state inside DW.

## Architecture / authority boundary

DW retrieval selects context; it does not change source authority. When a compact record is insufficient, the route must open the exact accepted source. A retrieval miss cannot be hidden by a plausible final answer. The deterministic context-assembly oracle proves that the supplied material/fallback path is structurally complete; the paired agent layer separately proves that an actual Worker-like/Reviewer-like execution still derives the required answer/verdict from that route. Both are scored against pre-frozen external expected facts/blockers/verdicts.

## Acceptance criteria

- before any route-specific tuning/result observation, freeze either (a) the complete eligible historical task universe plus deterministic selection/stratification rule or (b) the exact independently reviewed task manifest; the freeze must be anchored durably so chronology is reviewable;
- select at least six representative tasks from that predeclared universe/rule, spanning both CITY and PA and including Worker-like retrieval/composition plus Reviewer-like contradiction/omission detection;
- every task has a pre-frozen expected fact set and, where meaningful, expected blocker/verdict set derived from accepted historical/source truth before either route is executed;
- the baseline uses the accepted CTX-03 route rather than an intentionally bloated legacy strawman;
- the DW route begins from structured queries and opens accepted sources only when required by the task/provenance policy;
- deterministic route assembly for both routes recovers 100% of required expected facts/source-fallback reachability before any model execution; any missing required item is FAIL;
- every frozen task receives paired actual CTX and DW agent executions; no task may PASS on context-package completeness alone;
- each CTX/DW pair uses the same declared model/version, system/task prompt, decoding settings, tool permissions, execution budget and run-count policy. When a provider offers a stable seed, it must be equal across the pair; when it does not, the limitation is recorded and the same predeclared matched-run policy is used for both routes;
- the deterministic scorer evaluates each agent output against the pre-frozen expected facts/blockers/verdicts. Both routes must achieve 100% of required expected facts and causal expected blockers/verdicts on the designated acceptance runs; any missed required item or wrong required verdict is FAIL;
- DW agent outputs introduce no new false blocker/fact that changes the expected verdict;
- median injected source-context bytes are at least 30% lower for the DW route across the frozen suite; provider-reported input tokens are recorded when stable/available but do not replace the deterministic byte metric;
- per-task context accounting records which source fragments/documents were injected/opened so savings are auditable;
- repeated route assembly over the same accepted anchors returns equal selected record/source sets before model execution;
- trial results report structural context correctness, actual paired-agent correctness and context-cost measurements separately so one cannot mask failure in another.

## Deterministic proof / evidence

Use previously resolved accepted CITY/PA cases so hidden truth is known without inventing new semantic review. Before route-specific tuning, persist a `TASK_SELECTION_FREEZE` artifact containing the eligible-task universe or exact reviewed manifest, selection/stratification rule, authority anchors, task prompts, expected facts/blockers/verdicts, deterministic scoring rubric, matched-run policy and model/configuration constraints. The artifact is immutable for the acceptance comparison; changing it requires invalidating prior DW-04 tuning/results and restarting the trial from a new pre-tuning freeze.

Run deterministic context assembly first and score its selected source/record/fallback set against the frozen expected material. Then execute the mandatory paired agents for every task under the same declared model/configuration and score their outputs mechanically against the frozen expected facts/blockers/verdicts. Model prose is execution evidence, not the oracle; the oracle is the independently frozen expected/scoring manifest. Provider usage and byte/token accounting are recorded separately from correctness scoring.

## Causal negative-conformance classes

- one required expected fact is removed from the DW retrieval output while context gets smaller: structural trial and downstream paired-agent scoring must FAIL despite savings;
- one compact record loses provenance so exact source fallback cannot open the authority;
- one negative/rejected PA disposition is filtered as irrelevant and expected verdict changes;
- baseline is artificially inflated with unrelated documents and measurement guard detects protocol violation;
- eligible task universe, deterministic selection rule, exact task manifest, expected fact/blocker/verdict set or scoring rule is changed after route-specific tuning/results begin without invalidating/restarting the trial;
- one required CTX or DW agent execution is skipped while deterministic context assembly remains GREEN: WP must FAIL;
- CTX and DW paired executions use different model/version, decoding settings, tool permissions, execution budget or run-count policy: protocol guard must FAIL;
- scorer omits one pre-frozen expected blocker/verdict and would otherwise mark an incorrect agent answer GREEN: scorer conformance must FAIL;
- token/byte measurement excludes source fallback opened by the DW route;
- nondeterministic retrieval returns different source sets for the same accepted anchors.

## Content-shape probe

Required. Minimum six tasks selected from the pre-tuning frozen universe/rule, with both CITY and PA represented and both retrieval/composition and omission/contradiction review pressure. The suite must include at least one known cross-document CITY inheritance case and one PA disposition/fixture preservation case. Every selected task participates in the mandatory paired CTX-vs-DW agent execution and deterministic scoring protocol.

## Dependency / IP implications

No new external content dependency is expected. If a provider/tokenizer is used for agent execution or supplemental token accounting, exact model/provider/version/configuration is recorded. Byte accounting remains the vendor-neutral primary context-cost measure; deterministic expected/scoring manifests remain the correctness oracle.

## Residual risks

The fixture suite cannot prove all future agents/tasks improve, and a 30% median reduction is not a universal promise. Model variance remains a limitation even under matched configuration; the claim is therefore bounded to the frozen suite, model/configuration and run policy actually executed. Structural context completeness and actual agent performance are reported separately.

## Exact predecessor reopen condition

Reopen DW-03/DW-02 only if the trial exposes a source-projection/retrieval defect in their owned domain. Reopen CTX-03 only if effective evidence contradicts its accepted baseline guarantee rather than merely showing DW is more efficient. A bad task-selection/scoring/execution protocol is DW-04-owned and cannot be routed to predecessors.

## PASS consequence / next dependency

PASS permits `WP-DW-05`. It proves bounded project value for structured retrieval under the exact frozen task universe, matched model/configuration and scoring protocol: context reduction plus preserved actual agent task/review correctness. It does not authorize replacing normal source review, deleting accepted documents or generalizing the measured result to other models/tasks.
