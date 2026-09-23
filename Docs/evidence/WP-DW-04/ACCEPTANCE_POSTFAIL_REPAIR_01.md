# DW-04 post-FAIL causal repair 01

Authorization: PR #150 comment `5794585006`.

Campaign 01 (`35853858468`, artifact `10746802865`, exact candidate `196012fb809bca68448ef0ad6d468115acb36ac5`) remains immutable terminal FAIL evidence. Its 36 answers are not rescored, carried forward, selectively rerun, or counted toward this repair generation.

## Causal classes repaired

1. **PA disposition canonicalization.** The DW retrieval adapter previously exposed `MaterialText` such as `REJECT as requirement` as one free-form value. It now separates the uppercase canonical `Disposition` token from optional `Qualification`, preserving accepted provenance without making explanatory text masquerade as the canonical answer.
2. **Canonical field domains.** Acceptance fields that previously used unconstrained `token` now use result-independent structural domains: `access_code`, `pa_id`, and `disposition`. This prevents semantically-correct source identifiers from being replaced by prose while not encoding the expected value.
3. **Evidence scoring.** Required frozen evidence remains mandatory; extra evidence may only be emitted from the already-frozen global authoritative evidence vocabulary and no longer fails solely because it is additional. Facts, blockers and verdict remain exact. This avoids the Campaign-01 A-PA-01 evidence-cardinality artifact without giving the model task-specific expected evidence.
4. **Phase-local audit.** READY calibration requests are validated against their own frozen calibration `run_policy`; acceptance requests remain validated against the acceptance policy. Effective provider/model/configuration equality is still required across phases.

## Preserved proof surface

Unchanged: six acceptance task identities, accepted source truth, semantic task questions, source oracles and expected facts/blockers/verdicts, CTX route semantics, Luna/OpenAI effective provider/model configuration, R1/R2/R3 order, 18 pairs / 36 calls, 100% correctness rule, 30% median injected-source-byte threshold, no-replacement acceptance policy, and fail-closed CTX-pass/DW-fail decision rule.

The existing calibration READY evidence is reused only as pre-acceptance instrument evidence; no prior acceptance answer is reused. A fresh acceptance generation, if armed, must execute all 36 slots from scratch under the new freeze.
