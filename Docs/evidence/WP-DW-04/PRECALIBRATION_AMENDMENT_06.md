# DW-04 pre-acceptance calibration amendment 06 — canonical answer schema repair

Status: **OWNER-AUTHORIZED PLAN-LEVEL CALIBRATION INSTRUMENT REPAIR / NO ACCEPTANCE EXECUTION**

Effective pre-calibration universe remains `9cbed950a3469897cf286c9a90624c240701528f`. Calibration and acceptance task identities, source truth, semantic questions, frozen oracles, selection rule, CTX/DW semantics, 30% reduction threshold and 100% correctness requirement remain unchanged.

## Durable owner authorization

The repository owner explicitly authorized completing DW-04 here without an intermediate Reviewer round and authorized additional **pre-acceptance calibration-only** iterations needed to repair already-declared calibratable instrument dimensions, provided that:

- every prior campaign/result remains preserved and auditable;
- no successful prior answer is carried forward into a later calibration generation;
- acceptance tasks remain completely unexecuted until calibration becomes READY;
- no task identity, authority bytes, semantic question, expected fact/blocker/verdict/evidence oracle, acceptance selection rule, CTX/DW meaning, 30% threshold or 100% correctness rule changes;
- calibration changes are limited to provider/model/configuration, execution budget, nonsemantic prompt/output formatting, machine-readable schema and objective invalid-run policy;
- semantic misses may not be selectively retried within a generation.

This authorization is a transparent owner plan-level amendment, not an independent Reviewer PASS. The final complete DW-04 candidate remains subject to independent review.

## Generation 1 Luna result

Workflow run `35844576874` on candidate `8d610ec008c5d4676dbbf39cf90f92cf9ea8df7c` completed all eight designated CTX calls successfully at the transport/provider/schema layer:

- exact model `openai/gpt-5.6-luna-20260709`;
- resolved provider `OpenAI` on all eight calls;
- zero `RUN_INVALID` attempts;
- all eight produced parseable structured answers;
- calibration readiness was `NOT_READY` under the unchanged oracle.

The complete result is retained as `CALIBRATION_LUNA_GEN1_RESULTS.json` and contributes no favorable slot to the next generation.

## Causal diagnosis

The generation exposed an instrument-format defect rather than provider instability. The response schema constrained only fact keys to strings, while the scorer required canonical tokens. This allowed semantically correct content to become false REDs, for example explanatory `S1 (shallow)` instead of canonical `S1`, prose forms of `NO`/`YES`, and descriptive fixture text instead of the canonical fixture identifier. The output contract also allowed `REPORT` and `REJECT` without defining their general decision semantics, while the oracle scored them exactly.

The correction is general and answer-independent:

- fact fields receive canonical **type vocabularies**, not task-specific expected values;
- importance uses the full `A..D` universe;
- spatial depth uses the full `S0..S4` universe;
- interior uses the full `I0..I3` universe;
- boolean fields use `YES|NO`;
- requirement fields use `REQUIRED|NOT_REQUIRED`;
- fixture values are identifier-shaped tokens, not descriptive prose;
- verdict semantics are declared generically: `REJECT` when the proposition/assumption/promise under review is contradicted by supplied authority or a causal blocker is established against it; `REPORT` for neutral recovery/reporting when no proposition is rejected.

No expected answer is supplied to the adapter and no oracle is changed.

## Generation 2 rule

Generation 2 reruns all eight CTX calibration slots from scratch using the same exact Luna model/provider route. No generation-1 answer counts. Within generation 2 a slot may receive at most one replacement for an objective provider/transport invalid with no scorable answer; semantic misses are not selectively rerun. Acceptance remains blocked until all eight generation-2 slots satisfy the unchanged pre-frozen oracle.
