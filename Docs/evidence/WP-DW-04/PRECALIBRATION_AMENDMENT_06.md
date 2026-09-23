# DW-04 pre-calibration amendment 06 — final canonical-schema calibration generation

Status: **OWNER-AUTHORIZED PLAN-LEVEL CALIBRATION BUDGET AMENDMENT / FINAL GENERATION**

Effective pre-calibration task universe, accepted source truth and all semantic oracles remain unchanged from `9cbed950a3469897cf286c9a90624c240701528f` / `88c05db465a3abb9196c1ba8c9f60c42d5506453`.

Durable owner authorization: PR #150 conversation comment `5792657967`.

## Trigger

Owner-authorized Luna calibration generation 1 executed all eight designated CTX slots on run `35844576874` with zero provider/transport invalid attempts. Provider routing and strict structured JSON were stable, but the calibration instrument still allowed unrestricted free-form fact values and left generic verdict-token semantics under-specified.

The resulting evidence is retained in `CALIBRATION_LUNA_GEN1_CLOSED.json`. It is `NOT_READY` under the exact pre-frozen scorer and cannot contribute any successful slot to this final generation.

The failure pattern is instrument-facing rather than a source-truth contradiction:

- both `C-CITY-01` runs were exact PASS;
- both `C-PA-01` runs recovered the correct blocker, evidence and REJECT disposition but rendered canonical fact tokens as explanatory prose;
- both `C-CITY-02` runs recovered A/S1/I0 material and the blocker but rendered some canonical values as prose and emitted REPORT rather than the intended review disposition;
- both `C-PA-02` runs recovered the asymmetry control/blocker/evidence but rendered canonical fact tokens as free-form text and emitted REPORT rather than the intended review disposition.

DW-04 explicitly makes non-semantic prompt/output formatting and canonical answer schema formatting calibratable before acceptance, and explicitly forbids treating semantically correct structured content as wrong merely because of free-form wording. Calibration exists to validate the execution instrument before `TASK_SELECTION_FREEZE`.

## Owner-authorized proof-budget amendment

The original `PRECALIBRATION_FREEZE` proof budget allowed exactly eight designated calls plus bounded objective-invalid replacements. The owner now authorizes one plan-level extension of exactly **eight additional fresh CTX-only calls** to validate the corrected canonical schema. This is an explicit process deviation/amendment, not a hidden retry and not an independent Reviewer PASS.

No further calibration generation is authorized under the current DW-04 plan after this one.

## Final generation correction

Model/provider remain unchanged from Luna generation 1:

- OpenRouter gateway;
- exact model/version `openai/gpt-5.6-luna-20260709`;
- `openai` serving provider only;
- fallback disabled;
- required-parameter routing enabled;
- no tools;
- no explicit reasoning override;
- output budget and timeout unchanged.

Only the pre-acceptance execution instrument is tightened:

- canonical fact fields use task-independent field-domain constraints rather than arbitrary strings:
  - `importance`: `A|B|C|D`;
  - `spatial_depth`: `S0|S1|S2|S3|S4`;
  - `interior`: `I0|I1|I2|I3`;
  - yes/no relation/trigger fields: `YES|NO`;
  - requirement fields: `REQUIRED|NOT_REQUIRED`;
  - fixture identifiers: a shared union vocabulary covering frozen fixture/disposition identifiers rather than a task-specific expected answer;
- canonical facts must be tokens only, without explanatory suffixes;
- generic verdict semantics are defined consistently for all tasks: `REJECT` when the model emits a causal blocker invalidating the reviewed proposal/assumption, `REPORT` when it emits no blocker;
- blocker/evidence vocabularies remain shared unions and do not reveal which item is expected for a task.

## Immutable boundary

This amendment does not change task identity, semantic question, accepted source bytes, authority anchors, expected fact values, blocker identities, evidence identities, expected verdicts, acceptance selection rule, CTX baseline meaning, DW retrieval meaning, scorer equality semantics, the 30% reduction threshold, 100% acceptance correctness, or acceptance pair/run count.

All eight final-generation slots start from fresh attempt 1. Prior DeepSeek/Luna results are evidence only and cannot satisfy readiness. Each final-generation slot retains at most one objective provider/transport invalid replacement. Semantic misses are not retryable.

If this final generation is not READY on all eight slots, DW-04 stops under the current plan. If it is READY, `TASK_SELECTION_FREEZE` is created immediately before any acceptance-route call and the 36 acceptance executions proceed without an intermediate Reviewer round, as authorized by the owner. Final independent review remains mandatory for the completed candidate.
