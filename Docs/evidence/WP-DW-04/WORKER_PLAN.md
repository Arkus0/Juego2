# DW-04 trial chronology and causal boundary

Status: `DRAFT / ACTIVE / PRECALIBRATION`. The acceptance claim has **not** been established. `WP-DW-05` stays blocked.

## Published pre-result sequence

1. Baseline `main@82369fdb69e33e3492b41c4aaa7f1ae9aaed18af`; direct accepted dependencies and owned-vs-inherited obligations are recorded in `PREDECESSOR_CONTRACT_CHECK.md`.
2. Initial eligible universe and CTX-only calibration policy published in `091f4e9e813736c7e503e44d88a100b4d5ac1d6d` with zero route executions.
3. Pre-execution source-locator/reserve-overlap correction published in `ad570738613c992963668be5abd89bfdeb26b7d6`, documented in `PRECALIBRATION_AMENDMENT_01.md`. This is the **sole effective pre-calibration commit**. Initial commit remains visible as superseded history. No outcomes informed the correction.
4. Calibration and acceptance expected answers are reconstructed from pinned accepted CITY/PA source rows and fixture passages in separate source-oracle files. They cannot be inferred from either treatment route or model response. Their Git chronology predates all DW-04 model execution.
5. The continuation Worker replaced the parallel instrument workflow with the repository-owned exact-SHA path: `scripts/arkus-observe-exact-sha.sh` dispatches `WP-DW-04` to `scripts/dw04-observe-exact-sha.sh`, and frozen verification dispatches to `scripts/dw04-verify-exact-sha.sh`. Normal `Arkus Candidate Validation` / `Worker Candidate Preflight` remain the validation substrate. Observation proves instrumentation only and deliberately does **not** emit a foundational `EXECUTION_RECEIPT_V1`; only the frozen verifier may emit final GREEN after the full campaign exists.

No acceptance model call has occurred. No `TASK_SELECTION_FREEZE` exists. No acceptance result, context saving or PASS/FAIL quality observation exists.

## Before the first CTX calibration call

- Recheck the complete frozen task universe and each authority anchor against its pinned accepted blob. The four `C-*` identities and eight `A-*` identities are disjoint; the exact first-three-per-domain rule yields three CITY plus three PA acceptance tasks. Reserve `A-*-04` cannot substitute for a selected task because of outcomes.
- Use the accepted CTX-03 bounded-inspection/escalation semantics, not a vanity-minimum excerpt. A CITY row that relies on a table schema carries the relevant header/interpretation; a PA fixture carries the complete bounded causal block needed to answer the frozen question. A full unrelated document is forbidden baseline inflation. Every supplied source fragment remains an exact substring of the pinned accepted source.
- Freeze a concrete provider/model/configuration, machine-readable response schema, CTX context assembly, eight CTX-only calibration slots and bounded objective invalid-call policy before the first calibration request. No DW route and no acceptance question may execute during calibration.
- Execute exactly eight designated CTX calibration calls and preserve raw provider request IDs, effective requests/responses, timing and injected source bytes. All eight must satisfy the pre-frozen calibration oracle under one protocol. A semantic miss cannot trigger selective extension.

## After CTX readiness and before the first acceptance call

- Reconcile the unfiltered calibration evidence into `CALIBRATION_RESULTS.json` and commit it. Its result commit must descend from the pre-calibration/oracle lineage.
- Select the six acceptance identities mechanically. Commit `TASK_SELECTION_FREEZE.json` with exact prompts, copied source-oracles, required context literals, CTX/DW assembly recipes, scorer hash, exact provider/model/configuration, three matched run identities or documented seed unavailability, 36 slot order, objective invalid-run rule, 30% median byte threshold, 100% correctness and the predeclared `INCONCLUSIVE`/full-restart rule. No acceptance request may occur before this commit.
- Only after that freeze, assemble and commit `CONTEXT_ASSEMBLY.json`. CTX fragments must implement the accepted bounded CTX route and DW fragments must be real deterministic DW-02/03 query output plus exact accepted-source fallback where required. `scripts/dw04-trial.py` replays typed queries twice and verifies the required source literals before model execution.
- Publish the exact freeze/assembly identities on GitHub before any acceptance request. The acceptance campaign is one explicit GitHub Actions `workflow_dispatch`, identified by freeze commit. The campaign records the durable Actions run ID and all 36 unique provider request IDs. Frozen verification queries live GitHub and requires exactly one acceptance campaign for that freeze; a second favorable campaign is therefore RED rather than selectable evidence.

## Acceptance and disposition

The frozen run order is R1 CTX→DW, R2 DW→CTX, R3 CTX→DW per task: 18 matched pairs / 36 actual model executions. The execution harness must send identical non-context settings within each pair and write every designated response to an append-only transcript. It may not inspect semantic scores between calls to decide whether to continue, replace or rerun a scorable slot.

`scripts/dw04-trial.py audit` reopens the Git-anchored selection and assembly, checks ancestry/content identity, replays actual DW query output, verifies every context/request identity and scores all 18 pairs against the source-frozen oracles. Any CTX-pass/DW-fail or structural omission is `FAIL`; shared/baseline scorable instability is `INCONCLUSIVE`; `PASS` requires all 36 designated answers correct plus at least 30% median injected-source-byte reduction for DW. Provider-reported input-token usage is supplemental, not a replacement for deterministic bytes.

After execution, reconcile the complete unfiltered transcript, `CAMPAIGN_RECEIPT.json` and deterministic `TRIAL_RESULT.json` into a new candidate. Final verification is read-only on that exact candidate and re-runs the audit; committed result and recomputed result must be equal. The campaign receipt is chronology/inventory evidence only, never a semantic oracle.

## Current boundary

`FOUNDATIONAL_PROOF_VERDICT: NOT_READY` until real calibration and acceptance evidence exist. The remaining proof obligations are: semantically sufficient frozen CTX calibration contexts; eight real CTX calibration calls; final six-task/protocol freeze; deterministic post-freeze CTX/DW assembly; one durable 36-call acceptance campaign with unique provider IDs; deterministic quality/context audit; causal negative controls/final circuit-breaker; exact-SHA evidence reconciliation and strict Worker pre-review.

The repository-owned GitHub Actions substrate now supplies the pinned .NET SDK and canonical exact-SHA execution, so lack of local SDK is no longer treated as a proof limitation. A real authenticated model provider remains mandatory for the calibration/acceptance calls; simulated responses are never acceptance evidence.

No DW-00..03, CTX, hardening or accepted CITY/PA truth is changed. No DocSync or merge is authorized before independent PASS.