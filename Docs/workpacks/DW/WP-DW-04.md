# WP-DW-04 — Structured-context quality and token trial

Status: PLANNED / NOT_STARTED
Class: FOUNDATIONAL
EXECUTION_REQUIREMENT: REMOTE_OK
Depends on: `WP-DW-03` PASS + merge + DocSync; `WP-CTX-03` PASS + merge + DocSync
Blocks: `WP-DW-05`

## Objective and central claim

Demonstrate on a frozen representative task suite that DW structured retrieval plus source-open-on-demand can materially reduce supplied context relative to the accepted CTX baseline without losing required facts, causal blockers or verdict correctness.

## WHY_THIS_BOUNDARY

A faithful typed corpus does not prove that using it improves agent work. Context efficiency is a different claim with a dangerous vanity metric: smaller prompts can look successful while silently dropping the fact that matters. This WP therefore freezes quality oracles before measuring savings and fails on any demonstrated correctness regression.

## Inherited guarantees

Consumes accepted CTX-03 process/context baseline, DW-02 CITY queries, DW-03 PA lossless typed corpus and all repository review obligations. CTX remains owner of process policy.

## New guarantees owned

- a frozen comparison protocol and representative CITY+PA task suite;
- deterministic expected-fact/expected-verdict oracles for each fixture wherever the task admits them;
- a baseline route using accepted CTX context/navigation rules;
- a DW route using structured retrieval plus exact source fallback;
- reproducible context-cost measurement including injected source bytes and, where the execution surface reports it reliably, input-token usage;
- measured quality/context results with no universal extrapolation beyond the fixture universe;
- an explicit fail-closed rule: quality loss cannot be traded for token savings.

## Explicitly not re-proved

All CTX implementation correctness, model intelligence in general, universal token savings, PA research truth, CITY design quality or arbitrary future task performance.

## Allowed scope

Trial harness/scripts, frozen task fixtures, expected-fact/verdict manifests, context assembly, source-open-on-demand flow, measurement/evidence and narrowly necessary retrieval adapters.

## Forbidden scope

Changing task oracles after seeing results, cherry-picking only successful runs, weakening CTX baseline, claiming a universal percentage, using model prose as sole correctness oracle, modifying PA/CITY truth or modeling review process state inside DW.

## Architecture / authority boundary

DW retrieval selects context; it does not change source authority. When a compact record is insufficient, the route must open the exact accepted source. A retrieval miss cannot be hidden by a plausible final answer. Deterministic expected facts/verdicts and source provenance are the acceptance oracle; agent outputs are execution evidence.

## Acceptance criteria

- freeze at least six representative tasks before the final comparison, spanning both CITY and PA and including Worker-like retrieval/composition plus Reviewer-like contradiction/omission detection;
- every task has an independently reviewed expected fact set and, where meaningful, expected verdict/blocker set derived from accepted historical/source truth before route execution;
- baseline uses the accepted CTX-03 route rather than an intentionally bloated legacy strawman;
- DW route begins from structured queries and opens accepted sources only when required by the task/provenance policy;
- both routes recover 100% of required expected facts and expected causal verdicts/blockers on the deterministic fixture suite; any missing required item is FAIL;
- DW route introduces no new false blocker/fact that changes the expected verdict;
- median injected source-context bytes are at least 30% lower for the DW route across the frozen suite; provider-reported input tokens are recorded when stable/available but do not replace the deterministic byte metric;
- per-task context accounting records which source fragments/documents were injected/opened so savings are auditable;
- repeated route assembly over the same accepted anchors returns equal selected record/source sets before model execution;
- trial results clearly distinguish deterministic fixture correctness from stochastic model-run observations.

## Deterministic proof / evidence

Use previously resolved accepted CITY/PA cases so hidden truth is known without inventing new semantic review. The fixture manifest freezes task, authority anchors, expected facts, expected blockers/verdict and baseline/DW retrieval rules. Run deterministic context assembly first; then run the required agent executions if available, recording model/configuration and provider usage separately from the deterministic oracle.

## Causal negative-conformance classes

- one required expected fact is removed from the DW retrieval output while context gets smaller: trial must FAIL despite savings;
- one compact record loses provenance so exact source fallback cannot open the authority;
- one negative/rejected PA disposition is filtered as irrelevant and expected verdict changes;
- baseline is artificially inflated with unrelated documents and measurement guard detects protocol violation;
- expected fact/verdict manifest is edited after route results are observed;
- token/byte measurement excludes source fallback opened by the DW route;
- nondeterministic retrieval returns different source sets for the same accepted anchors.

## Content-shape probe

Required. Minimum six frozen tasks with both CITY and PA represented and both retrieval/composition and omission/contradiction review pressure. The suite must include at least one known cross-document CITY inheritance case and one PA disposition/fixture preservation case.

## Dependency / IP implications

No new external content dependency is expected. If a provider/tokenizer is used for supplemental token accounting, exact tool/version/configuration is recorded and byte accounting remains available as a vendor-neutral primary measure.

## Residual risks

The fixture suite cannot prove all future agents/tasks improve, and a 30% median reduction is not a universal promise. Model variance remains descriptive evidence around deterministic context correctness.

## Exact predecessor reopen condition

Reopen DW-03/DW-02 only if the trial exposes a source-projection/retrieval defect in their owned domain. Reopen CTX-03 only if effective evidence contradicts its accepted baseline guarantee rather than merely showing DW is more efficient.

## PASS consequence / next dependency

PASS permits `WP-DW-05`. It proves bounded project value for structured retrieval; it does not authorize replacing normal source review or deleting accepted documents.
