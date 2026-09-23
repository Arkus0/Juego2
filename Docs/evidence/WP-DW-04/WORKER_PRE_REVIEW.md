# DW-04 strict Worker pre-review — provisional blocking audit

WORKER_PRE_REVIEW: NOT_READY
FOUNDATIONAL_PROOF_VERDICT: NOT_READY
TRUST_BOUNDARY: accepted Git/source blobs and accepted DW providers; actual provider adapter/request/response identity must be externally auditable
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Causal challenges already applied

- A corpus selected from DW query results could silently shrink: RED via independently enumerated source-anchored twelve-task catalogue, immutable pre-result commit and exact six-task selection.
- Calibration could leak acceptance tasks or transfer a successful calibration task: disjoint IDs/anchors, CTX-only calibration policy, eight designated slots and ancestry checks; **effective provider calls are not yet present**, so this is preparatory rather than complete proof.
- Expected claims could be copied from the model or adjusted after outcomes: calibration and acceptance source oracles were written before model calls; scorer consumes their exact pre-result Git objects. Their correctness still needs independent source review.
- A 2/3 apparent win or shared baseline instability could be mislabeled PASS: pure decision controls turn a single CTX-pass/DW-fail into FAIL and shared misses into INCONCLUSIVE. No majority or selective semantic rerun path is implemented.
- Context bytes could omit source fallback or compare unequal configuration: acceptance audit recalculates every fragment's UTF-8 bytes, compares frozen contexts/requests/seed-or-slot identities, and replays typed queries. The concrete contexts and actual provider requests have yet to exist.
- CTX could be inflated to win savings: only accepted task-anchor paths are permitted and a largely unrelated full document is rejected. A fresh reviewer must still challenge whether each concrete CTX excerpt follows the accepted bounded-read rule. The current simple CITY tasks may produce little or no genuine saving.
- A fabricated provider adapter could emit expected answers without model work: the required concrete provider adapter, raw provider IDs/time, exact executable/dependencies and external run evidence are still missing. This is a blocking in-claim class until real execution and source review establish it.
- A route could be run on acceptance questions while drafting the selection freeze: the freeze now contains only source/query recipes; `CONTEXT_ASSEMBLY.json` must be committed as a descendant of `TASK_SELECTION_FREEZE` and before model calls. It cannot update those recipes or expected material after seeing answers.

The current code checks scorer semantics and catalogue invariants locally; the dedicated hosted instrument workflow must still compile the C# query adapter. No actual model run was performed or simulated as an acceptance observation. Missing proof prevents a clean pre-review, an exact-SHA Worker preflight closure and a REVIEW_READY marker. Keep PR Draft.
