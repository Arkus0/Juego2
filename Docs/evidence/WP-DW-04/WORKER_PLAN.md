# DW-04 trial chronology and causal boundary

Status: `DRAFT / ACTIVE / PRECALIBRATION`. The acceptance claim has **not** been established. `WP-DW-05` stays blocked.

## Published pre-result sequence

1. Baseline `main@82369fdb69e33e3492b41c4aaa7f1ae9aaed18af`; direct accepted dependencies and owned-vs-inherited obligations are recorded in `PREDECESSOR_CONTRACT_CHECK.md`.
2. Initial eligible universe and CTX-only calibration policy published in `091f4e9e813736c7e503e44d88a100b4d5ac1d6d` with zero route executions.
3. Pre-execution source-locator/reserve-overlap correction published in `ad570738613c992963668be5abd89bfdeb26b7d6`, documented in `PRECALIBRATION_AMENDMENT_01.md`.
4. Pre-execution PA-02 question/oracle alignment published in `9cbed950a3469897cf286c9a90624c240701528f`, documented in `PRECALIBRATION_AMENDMENT_02.md`. This is the **sole effective pre-calibration commit**. Earlier commits remain visible as superseded history. No model outcome informed either correction.
5. Calibration and acceptance expected answers are reconstructed from pinned accepted CITY/PA source rows and fixture passages in separate source-oracle files. They cannot be inferred from either treatment route or model response. Their Git chronology predates all DW-04 model execution.
6. The continuation Worker replaced the parallel validation path with repository-owned exact-SHA observation/verification. Normal `Arkus Candidate Validation`, `Worker Candidate Preflight`, `Arkus Main Safety` and CTX process checks remain the validation substrate.
7. The real model campaign is a deliberately triggered GitHub Actions executor, not an automatic validator. It requires an exact PR+SHA marker, re-proves canonical observation, then claims a durable one-shot campaign identity before the first provider call.

No calibration provider call and no acceptance model call has occurred. No `TASK_SELECTION_FREEZE` exists. No acceptance result, context saving or PASS/FAIL quality observation exists.

## Frozen calibration route before the first call

- Effective universe: `9cbed950a3469897cf286c9a90624c240701528f`.
- Provider gateway: OpenRouter using repository secret `OPENROUTER_API_KEY`.
- Exact model slug: `deepseek/deepseek-v4.1-flash`; do not use a `latest` alias.
- Provider route: DeepSeek only, `allow_fallbacks=false`, `require_parameters=true`, so matched calls cannot silently move to another serving provider.
- Decoding/execution: `temperature=0`, no tools, no extra reasoning control, fixed output budget/timeout.
- Machine-readable output: strict JSON-schema surface for `facts`, `blockers`, `verdict`, `evidence`.
- Response-contract vocabulary is deliberately shared across calibration tasks for blockers/verdicts/evidence; task-specific allowed sets may not reveal the expected semantic answer.
- CTX calibration context uses accepted bounded-inspection semantics. Full unrelated documents are forbidden baseline inflation; fragments remain exact substrings of pinned accepted sources.
- Exactly eight designated CTX calls: two per frozen calibration task. No DW route and no acceptance task executes during calibration.
- Objective provider/transport failure may use only the already-declared bounded retry. A scorable semantic miss cannot trigger a replacement or extension.

A prior infrastructure probe on candidate `5aba31be0ad362ea2926da93d353fee3d2155779` / Actions run `35837616972` stopped because `OPENAI_API_KEY` was absent. That run occurred **before campaign claim and before provider calls**, consumed zero slots, and is not semantic evidence. The final route no longer uses OpenAI credentials or the Luna model.

## After CTX readiness and before the first acceptance call

- Reconcile the complete unfiltered calibration evidence into `CALIBRATION_RESULTS.json` and commit it. Its result commit must descend from the effective pre-calibration/oracle/protocol lineage.
- Select the six acceptance identities mechanically from the untouched acceptance pool. Commit `TASK_SELECTION_FREEZE.json` with exact prompts, copied source-oracles, required context literals, CTX/DW assembly recipes, scorer hash, the same frozen provider/model configuration, matched run identities/order, objective invalid-run rule, 30% median byte threshold, 100% correctness and the predeclared `INCONCLUSIVE`/restart rule. No acceptance request may occur before this commit.
- Only after that freeze, assemble and commit `CONTEXT_ASSEMBLY.json`. CTX fragments implement accepted bounded CTX navigation; DW fragments are real deterministic DW-02/03 query output plus exact accepted-source fallback when required.
- Publish exact freeze/assembly identities before any acceptance request. The acceptance campaign must have one durable GitHub Actions identity and preserve all 36 designated model executions without semantic cherry-picking.

## Acceptance and disposition

The frozen run order is R1 CTX→DW, R2 DW→CTX, R3 CTX→DW per task: 18 matched pairs / 36 actual model executions. The execution harness sends identical non-context settings within each pair and preserves every designated response. It may not inspect semantic scores between calls to decide whether to continue, replace or rerun a scorable slot.

`scripts/dw04-trial.py audit` reopens the Git-anchored selection and assembly, checks ancestry/content identity, replays actual DW query output, verifies every context/request identity and scores all 18 pairs against the source-frozen oracles. Any CTX-pass/DW-fail or structural omission is `FAIL`; shared/baseline scorable instability is `INCONCLUSIVE`; `PASS` requires all 36 designated answers correct plus at least 30% median injected-source-byte reduction for DW. Provider-reported token usage is supplemental; deterministic injected-source bytes remain the acceptance metric.

After execution, reconcile the complete transcript, campaign receipt and deterministic result into a new exact candidate. Final verification is read-only on that candidate and must recompute the same disposition. Campaign receipts are chronology/inventory evidence only, never semantic oracles.

## Current boundary

`FOUNDATIONAL_PROOF_VERDICT: NOT_READY` until real calibration and acceptance evidence exist. The remaining external precondition is a configured GitHub Actions secret named `OPENROUTER_API_KEY`; adding the secret does not itself execute or modify the proof. After it exists, the exact-SHA campaign marker can authorize the single frozen eight-call calibration run.

No DW-00..03, CTX, hardening or accepted CITY/PA truth is changed. No DocSync or merge is authorized before independent PASS.
