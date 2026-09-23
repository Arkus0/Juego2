# DW-04 strict Worker pre-review — provisional blocking audit

WORKER_PRE_REVIEW: NOT_READY
FOUNDATIONAL_PROOF_VERDICT: NOT_READY
TRUST_BOUNDARY: accepted Git/source blobs and accepted DW providers; actual OpenRouter request-response identity must be externally auditable
PROOF_BUDGET_VERDICT: OWNER_AMENDED_PRE_ACCEPTANCE

## Causal challenges already closed before restarted calibration

- **Adaptive corpus selection:** RED via independently enumerated source-anchored twelve-task catalogue, immutable pre-result partition and deterministic first-three-per-domain acceptance selection.
- **Calibration/acceptance contamination:** calibration IDs are disjoint from the eight-task acceptance pool; only the four `C-*` tasks can run during CTX-only calibration; no calibration task may replace an acceptance task.
- **Question/oracle mismatch:** PA-02 calibration wording was corrected before provider execution and the amendment remains in Git history; expected facts/blocker/verdict did not change.
- **Model answer as oracle:** calibration and acceptance source oracles predate all provider calls and are derived from pinned accepted CITY/PA bytes.
- **Response vocabulary leaking expected answer:** calibration tasks share one union blocker/verdict/evidence formatting vocabulary; the external oracle is unavailable to the adapter.
- **DeepSeek evidence erasure:** run `35840654094` is retained as `CALIBRATION_INVALID_ATTEMPT_01.json`; run `35842779697` is retained as `CALIBRATION_DEEPSEEK_CLOSED.json` and GitHub artifact `10742271293`. The latter contains two scorable correct C-CITY-01 answers followed by two objective invalid C-PA-01 attempts. None can satisfy a Luna restart slot.
- **Adaptive third retry:** DeepSeek is closed rather than granting C-PA-01 a third attempt. Owner authorization in PR #150 comment `5792478254` permits one complete calibration restart from eight fresh slots under a new frozen calibratable provider/model protocol.
- **Provider/model drift:** restarted route is exact `openai/gpt-5.6-luna-20260709` through OpenRouter, pinned to the `openai` serving provider with fallback disabled and `require_parameters=true`. Unsupported temperature control is omitted; tools and explicit reasoning overrides remain absent; schema, budget and timeout remain frozen.
- **Provider simulation:** active Luna adapter performs a real authenticated OpenRouter HTTP request and records provider request id, resolved model/provider, usage and raw structured response. Campaign execution requires `OPENROUTER_API_KEY` and a durable GitHub campaign identity claimed before provider calls.
- **Provider adapter substitution:** acceptance executor freezes and hashes the actual adapter script path, not merely the Python interpreter, and provider requests include the frozen response contract.
- **2/3 or majority-vote false PASS:** decision controls make any CTX-pass/DW-fail a FAIL; shared instability remains INCONCLUSIVE; PASS requires all 36 designated acceptance answers correct.
- **Context-cost vanity win:** deterministic audit recalculates injected UTF-8 source bytes; provider token accounting is supplemental. Structural source completeness is independently required, so a smaller broken route cannot PASS.
- **CTX strawman inflation:** contexts may use only task anchors; full unrelated documents are forbidden. Calibration CITY contexts include the table header needed to interpret A–D/S/Interior plus the exact row.
- **DW self-confirmation:** acceptance structural audit replays the real DW-02/03 typed query executable and checks source provenance/fallback against independent source literals.
- **Second favorable campaign on one candidate:** each campaign is exact-PR+SHA gated and creates a durable GitHub check-run start marker before provider calls; a second campaign on that exact SHA is fail-closed.
- **Frozen request shape drift:** scorer verifies model/configuration, task prompt, separate response contract, system prompt, slot, context and matched-run identity.

## Deliberately not claimed yet

DeepSeek calibration did not reach readiness and is closed. No acceptance task has executed. The owner-authorized Luna restart has not yet executed. Therefore none of the following is yet evidence-backed:

- eight-slot Luna CTX calibration readiness;
- final six-task `TASK_SELECTION_FREEZE`;
- post-freeze CTX/DW `CONTEXT_ASSEMBLY`;
- 18 matched pairs / 36 acceptance executions;
- semantic preservation result;
- >=30% median injected-source-byte reduction;
- final acceptance campaign uniqueness/inventory;
- final causal-negative matrix and circuit-breaker audit.

The repository secret `OPENROUTER_API_KEY` is configured. The Luna campaign marker must remain absent until canonical exact-SHA observation is GREEN for the restart protocol candidate.

Missing restarted calibration and acceptance evidence prevents CLEAN pre-review, frozen candidate metadata and REVIEW_READY. Keep PR #150 Draft and keep DW-05 blocked.
