# DW-04 trial chronology and causal boundary

Status: `DRAFT / ACTIVE / BLOCKED_BEFORE_CALIBRATION`. The acceptance claim has **not** been established. `WP-DW-05` stays blocked.

## Published pre-result sequence

1. Baseline `main@82369fdb69e33e3492b41c4aaa7f1ae9aaed18af`; direct accepted dependencies and owned-vs-inherited obligations are recorded in `PREDECESSOR_CONTRACT_CHECK.md`.
2. Initial eligible universe and CTX-only calibration policy published in `091f4e9e813736c7e503e44d88a100b4d5ac1d6d` with zero route executions.
3. Pre-execution source-locator/reserve-overlap correction published in `ad570738613c992963668be5abd89bfdeb26b7d6`, documented in `PRECALIBRATION_AMENDMENT_01.md`. This is the **sole effective pre-calibration commit**. Initial commit remains visible as superseded history. No outcomes informed the correction.
4. Calibration and acceptance expected answers are reconstructed manually from pinned accepted CITY/PA source rows and fixture passages, in separate source oracle files. They cannot be inferred from either treatment route or model response. Their commit must precede calibration executions; immutable copies and ancestry are checked by `scripts/dw04-trial.py`.

No model call has occurred. No `TASK_SELECTION_FREEZE` exists. No acceptance result, context saving or PASS/FAIL quality observation exists.

## Before the first CTX calibration call

- Review the complete task universe and independently check each anchor against its pinned accepted source blob. The four `C-*` identities and the eight `A-*` identities are disjoint; the exact first-three-per-domain rule yields three CITY plus three PA acceptance tasks. Reserve `A-*-04` cannot be substituted after outcomes.
- Freeze calibration oracles before calibration calls; choose and record a concrete model/provider adapter, exact executable/dependency/configuration, task/system prompt, response schema, CTX assembly, eight fixed CTX-only run slots and objective invalid-call policy. No DW route or acceptance question may execute in calibration.
- Derive CTX baseline excerpts with the accepted Context Bootstrap §2A bounded-inspection/escalation rule, including source headers or nearby context only when needed for correct interpretation. A full unrelated document is not a legitimate baseline inflation. Every excerpt is checked against the pinned accepted blob.
- Execute exactly eight CTX calibration calls and preserve all raw provider request IDs, requests, responses and context bytes. Every designated call must satisfy its pre-frozen oracle under one chosen protocol. A semantic miss cannot be hidden by a selective calibration extension.

## After CTX readiness and before the first acceptance call

- Commit `CALIBRATION_RESULTS.json` with the complete, unfiltered CTX evidence. The result commit must descend from the pre-calibration and oracle commits.
- Select the six identities mechanically. Commit `TASK_SELECTION_FREEZE.json` with the six exact task prompts, source-oracle copies, required context literals, **CTX/DW assembly recipes** (path, selector/query arguments, provenance/source-open policy), scorer hash, exact model/version/decoding/thinking/tool/run policy, provider adapter binary/dependency identity, three matched seeds or documented stable-seed unavailability, 36 slot order, objective invalid-run rule, 30% byte threshold, 100% correctness and restart policy. Do not run either acceptance route before this commit. Any source-oracle or scorer change after observed acceptance outcomes invalidates the campaign.
- Only **after** that freeze, assemble the exact route contexts and commit `CONTEXT_ASSEMBLY.json` as a descendant before any model call. Replay DW queries twice through `tools/Arkus.Dw04.Retrieval` using the accepted DW-02/03 providers and pinned source bytes. Structural requirement failure stops execution before the model. CTX source excerpts and any DW fallback source text must be literal accepted-source substrings. Model context and all opened source bytes are accounted for; there is no free unmeasured fallback. The post-freeze assembly cannot change the pre-frozen source/query recipe or expected material.
- Publish the exact freeze commit durably on GitHub before any acceptance request. Retain the external publication timestamp and independently observable provider call IDs/time. Git commit object ancestry plus raw run timestamps are necessary, but a local self-asserted timestamp alone is insufficient chronology proof.

## Acceptance and disposition

The frozen run order is R1 CTX→DW, R2 DW→CTX, R3 CTX→DW per task, for 18 pairs/36 executions. `scripts/dw04-execute.py` creates an exclusive append-only local JSONL transcript and sends the identical non-context settings to a real fixed provider adapter. It never scores semantic results between calls. A partial/invalid campaign remains preserved; any objective retry or full restart must follow the previously frozen bounded policy and retain old evidence. Simulated provider output is never acceptance proof.

`scripts/dw04-trial.py audit` reopens the pre-run selection and assembly commits, checks ancestry/content identity, replays actual DW query output, checks source fragments and all 36 request settings/identities/context bytes, scores each run independently and reports all 18 pairs. Any CTX-pass/DW-fail or structural miss is FAIL. Baseline/shared scorable instability is INCONCLUSIVE. PASS needs all 36 scorable answers correct and median DW injected source bytes at least 30% below median CTX. Provider token usage is supplemental and must be recorded when reported. An exclusive local output file prevents overwriting one transcript but cannot by itself prove that a second favorable campaign was never started under a different filename; an externally durable unique campaign-start receipt and provider-side request-ID inventory must close that class before execution.

## Scope and current limitation

This environment has no `dotnet` 8.0.425 and exposes no authenticated model provider, endpoint or API credential. Its GitHub connector can publish repository commits, and hosted CI can compile/test the trial adapter, but neither is a substitute for eight real CTX calibration calls followed by 36 real paired acceptance calls. Thus `FOUNDATIONAL_PROOF_VERDICT: NOT_READY`, `UNRESOLVED_PROOF_OBLIGATIONS: 6` (calibration, final freeze, structural replay, real runs, external campaign uniqueness, independent source/context-cost audit), `KNOWN_UNDETECTED_DEFECT_CLASSES: 3` (provider-adapter authenticity, CTX baseline fairness until concrete contexts are frozen, and repeated campaigns under alternate transcript paths), `PROOF_BUDGET_VERDICT: WITHIN_BUDGET` for the preparatory tooling. Worker pre-review cannot be CLEAN and the PR cannot be Ready/FROZEN while these obligations remain.

No DW-00..03, CTX or accepted CITY/PA truth is changed. No DocSync or merge is authorized by this preparatory state.
