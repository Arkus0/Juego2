# DW-04 pre-calibration amendment 05 — owner-authorized full calibration restart on Luna

Status: **OWNER-AUTHORIZED PRE-ACCEPTANCE CALIBRATION RESTART / NO ACCEPTANCE EXECUTION**

Effective pre-calibration universe remains `9cbed950a3469897cf286c9a90624c240701528f` and calibration/source oracles remain unchanged.

Durable owner authorization: PR #150 conversation comment `5792478254`.

## Trigger and preserved evidence

The DeepSeek calibration instrument is closed after two exact-SHA campaigns:

1. run `35840654094` on `7e03984e9ddeb0a31a64c99e35c394ed166d774d` retained one `RUN_INVALID_PRE_ANSWER` for `C-CITY-01/R1` attempt 1 with no provider request identity or scorable answer;
2. run `35842779697` on `9ac367a3a6dbdd5ad2e5d6fd219f28e3d7afa125` produced two scorable `C-CITY-01` answers that match the frozen oracle, then `C-PA-01/R1` attempt 1 and its sole objective-invalid replacement both ended `RUN_INVALID` with no scorable structured answer.

The second run is durably summarized in `CALIBRATION_DEEPSEEK_CLOSED.json`; GitHub Actions artifact `10742271293` has ZIP SHA-256 `7a83e642ea5e86a92e0e274b90e3f80320bf8b842dbd942e56e750db060513cb`. No DeepSeek result may count toward the restarted calibration.

## Authorization and classification

`PRECALIBRATION_FREEZE` classified provider/model/version/configuration, non-semantic output formatting, execution budget and bounded objective transport policy as calibratable before acceptance. The owner has explicitly authorized one complete pre-acceptance calibration restart after the instrument exhausted the bounded objective-invalid replacement on `C-PA-01/R1` without producing a scorable answer.

This authorization is not represented as an independent Reviewer PASS. The completed DW-04 candidate remains subject to final independent review.

## Restarted protocol

The restarted calibration uses:

- gateway: OpenRouter;
- exact model/version route: `openai/gpt-5.6-luna-20260709`;
- serving provider order: `openai` only;
- provider fallback: disabled;
- required-parameter routing: enabled;
- strict JSON-schema `response_format` unchanged;
- temperature omitted because the selected exact Luna route does not declare temperature as a supported model parameter;
- no tools and no explicit reasoning override;
- output budget and timeout unchanged;
- all eight CTX-only calibration slots rerun from scratch;
- each restarted slot receives at most one objective-invalid replacement under the same fail-closed rule;
- no successful or failed DeepSeek slot is carried into the Luna readiness result.

Current OpenRouter metadata confirms GPT-5.6 Luna supports structured outputs/`response_format`, `seed`, `max_tokens`, and provider pinning; provider routing uses the `openai` provider slug with fallback disabled.

## Immutable boundary

This amendment does **not** change:

- accepted CITY/PA source bytes or authority anchors;
- eligible task identities or calibration/acceptance partition;
- calibration semantic questions or frozen calibration oracles;
- acceptance-pool identities or deterministic six-task selection rule;
- CTX baseline semantics or DW retrieval semantics;
- acceptance source oracles or scorer semantics;
- 30% median injected-source-byte reduction threshold;
- 100% designated-run correctness requirement;
- acceptance pair count/order policy.

No acceptance task has executed and no `TASK_SELECTION_FREEZE` exists. Acceptance remains blocked until one complete restarted Luna calibration is `READY` on all eight designated slots.
