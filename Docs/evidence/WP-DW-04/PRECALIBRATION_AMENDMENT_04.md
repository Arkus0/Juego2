# DW-04 pre-calibration amendment 04 — retain invalid attempt and correct serving route

Status: **CALIBRATION INFRASTRUCTURE CORRECTION / NO SEMANTIC RESULT OBSERVED**

Effective pre-calibration universe remains `9cbed950a3469897cf286c9a90624c240701528f`.

## Trigger

Exact-SHA calibration campaign `35840654094` on candidate `7e03984e9ddeb0a31a64c99e35c394ed166d774d` passed credential validation, canonical observation, build/tests and the durable campaign claim. Its first designated slot (`C-CITY-01`, run 1, CTX) then terminated as `RUN_INVALID` before any scorable structured answer existed.

The complete machine record is retained in `CALIBRATION_INVALID_ATTEMPT_01.json`. The campaign artifact contained `runs=[]`, one invalid attempt, no provider request identity and no semantic score. Nothing from this attempt may be used to rank, replace or modify a task or oracle.

## Classification and correction

The active protocol pinned OpenRouter routing to the DeepSeek serving provider while simultaneously requiring strict `response_format` structured output and `require_parameters=true`. Current OpenRouter routing metadata exposes DeepSeek V4.1 Flash through multiple providers and reports structured-output support at the model route, while the endpoint-specific DeepSeek route used by the failed configuration does not provide the required `response_format` capability. This is an execution/provider compatibility defect, not a semantic task result.

Provider/model/configuration and objective infrastructure retry policy are explicitly calibratable dimensions in `PRECALIBRATION_FREEZE`. Before any scorable calibration answer, the serving route is therefore corrected to:

- gateway: OpenRouter — unchanged;
- model: exact `deepseek/deepseek-v4.1-flash` — unchanged;
- serving provider: **DeepInfra**;
- provider fallback: disabled;
- required-parameter routing: enabled;
- temperature, tools, response schema, output budget and timeout: unchanged;
- semantic questions, CTX contexts, source truth and calibration oracles: unchanged.

The first call on the corrected route is the **single permitted replacement** for the retained `C-CITY-01/R1` invalid attempt. That slot may not receive another replacement if the replacement itself becomes invalid. Other designated calibration slots retain their own original one-invalid-replacement allowance.

## Unchanged proof boundary

This amendment does not change eligible task identities, calibration/acceptance partition, acceptance selection rule, any accepted CITY/PA bytes, any expected fact/blocker/verdict/evidence oracle, CTX/DW semantics, the 30% context-reduction threshold, 100% correctness, or acceptance pair count/order policy.

No acceptance task has executed. No `TASK_SELECTION_FREEZE` exists. The invalid attempt is preserved rather than erased, and a fresh exact-SHA canonical GREEN is required before arming the corrected campaign.
