# DW-04 strict Worker pre-review — provisional blocking audit

WORKER_PRE_REVIEW: NOT_READY
FOUNDATIONAL_PROOF_VERDICT: NOT_READY
TRUST_BOUNDARY: accepted Git/source blobs and accepted DW providers; actual OpenRouter/DeepSeek request-response identity must be externally auditable
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Causal challenges already closed before calibration

- **Adaptive corpus selection:** RED via independently enumerated source-anchored twelve-task catalogue, immutable pre-result partition and deterministic first-three-per-domain acceptance selection.
- **Calibration/acceptance contamination:** calibration IDs are disjoint from the eight-task acceptance pool; only the four `C-*` tasks can run during CTX-only calibration; no calibration task may replace an acceptance task.
- **Question/oracle mismatch:** PA-02 calibration wording was corrected before execution and the amendment remains in Git history; expected facts/blocker/verdict did not change.
- **Model answer as oracle:** calibration and acceptance source oracles predate all provider calls and are derived from pinned accepted CITY/PA bytes.
- **Response vocabulary leaking expected answer:** task-specific blocker/verdict/evidence allowlists were removed before provider execution. Calibration tasks share one union formatting vocabulary; the external oracle remains hidden from the adapter.
- **Provider/model drift:** frozen route is OpenRouter -> exact `deepseek/deepseek-v4.1-flash` -> DeepSeek serving provider only, with fallback disabled and required-parameter routing enabled. Temperature, tool policy, reasoning-control posture, output budget and timeout are frozen.
- **Provider simulation:** active adapter performs a real authenticated OpenRouter HTTP request. The superseded direct-OpenAI adapter is removed. Campaign execution requires `OPENROUTER_API_KEY` and a durable GitHub campaign identity claimed before the first call.
- **Provider adapter substitution:** acceptance executor freezes and hashes the actual adapter script path, not merely the Python interpreter, and provider requests include the frozen response contract.
- **2/3 or majority-vote false PASS:** decision controls make any CTX-pass/DW-fail a FAIL; shared instability remains INCONCLUSIVE; PASS requires all 36 designated acceptance answers correct.
- **Context-cost vanity win:** deterministic audit recalculates injected UTF-8 source bytes; provider token accounting is supplemental. Structural source completeness is independently required, so a smaller broken route cannot PASS.
- **CTX strawman inflation:** contexts may use only task anchors; full unrelated documents are forbidden. Calibration CITY contexts include the table header needed to interpret A–D/S/Interior plus the exact row, rather than a semantically meaningless isolated row.
- **DW self-confirmation:** acceptance structural audit replays the real DW-02/03 typed query executable and checks source provenance/fallback against independent source literals.
- **Second favorable campaign:** calibration campaign is exact-PR+SHA gated and creates a durable GitHub check-run start marker before provider calls; a second campaign on that exact SHA is fail-closed. Acceptance requires an analogous unique durable campaign identity tied to its freeze.
- **Frozen request shape drift:** scorer now verifies model/configuration, task prompt, separate response contract, system prompt, slot, context and matched-run identity.

## Deliberately not claimed yet

No calibration provider call has occurred and no acceptance task has executed. Therefore none of the following is yet evidence-backed:

- eight-run CTX calibration readiness;
- final six-task `TASK_SELECTION_FREEZE`;
- post-freeze CTX/DW `CONTEXT_ASSEMBLY`;
- 18 matched pairs / 36 acceptance executions;
- semantic preservation result;
- >=30% median injected-source-byte reduction;
- final campaign uniqueness/inventory;
- final causal-negative matrix and circuit-breaker audit.

The current external precondition is a GitHub Actions repository secret named `OPENROUTER_API_KEY`. It must be added outside the proof artifacts. The campaign marker must remain absent until canonical observation is GREEN on the exact candidate that will execute calibration.

No actual model response has been simulated or substituted. Missing execution evidence prevents CLEAN pre-review, frozen candidate metadata and REVIEW_READY. Keep PR #150 Draft and keep DW-05 blocked.
