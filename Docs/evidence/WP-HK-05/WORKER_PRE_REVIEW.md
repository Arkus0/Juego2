# WP-HK-05 Worker pre-review

WORKER_PRE_REVIEW: NOT_READY
WORKER_PRE_REVIEW_FINDINGS_FIXED: 10
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-05
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

Baseline: `dbd8121411079f22a01d5cb85345e180ff41f7e2`
Previous failed frozen candidates:
- cycle 1: `e0c865efd2127ca53d9e25064215e09a4579acd4` — review `5257015987`;
- cycle 2: `8c8d8c1a66b4995bbc9f3f933ba1791e49d69e97` — review `5257239932`.
Repair cycle: `2`

This report is deliberately `NOT_READY` while the repaired documentation-reconciled candidate has not yet completed canonical observation and strict Worker pre-review. Any earlier CLEAN applied only to a superseded candidate.

## Circuit-breaker architecture re-audit

Both independent FAILs are the same HK05-owned foundational class: **aggregate validation under ambiguous identity**. Under `FOUNDATIONAL_PROOF_STANDARD.md` and `WORKER_REVIEW_PROTOCOL.md`, repair cycle 2 therefore re-audited the semantic boundary before making another code change.

The defect was not another missing fixture. Repair cycle 1 correctly recognized that a raw containment result can depend on which duplicate object becomes the evaluator's representative, but implemented the uncertainty as a global world mode: any duplicate object ID suppressed every `ContainmentAcyclic` diagnostic. That hid a completely separate unique-ID cycle and violated HK05's deterministic multi-violation aggregation claim.

The corrected architecture keeps inherited HK02 evaluation untouched and makes HK05's public ambiguity policy dependency-local:

1. duplicate identity itself is always materialized at an index-addressable source entry;
2. a secondary diagnostic whose source resource is ambiguous is deferred;
3. for a containment-cycle diagnostic sourced from a unique object, HK05 traces that source's own containment chain;
4. reaching a duplicated ID means exposing the raw cycle would require choosing a representative, so that diagnostic is deferred;
5. a chain that closes using only unique IDs is independent and remains reportable even when an unrelated duplicate exists elsewhere.

This is the causal boundary: **does this diagnostic depend on ambiguous identity?**, not **does any ambiguity exist anywhere?**

## Causal regression pair

`Hk05AmbiguousIdentityRegressionTests` now protects both sides of the policy:

- connected ambiguity: the existing `node.b -> node.a` case traverses duplicated `node.a`; reversing the two `node.a` definitions must not expose first-wins-dependent containment output;
- disconnected independence: two divergent `dup.x` entries coexist with unique `cycle.a -> cycle.b -> cycle.a`; reversing only the duplicate entries must preserve the complete semantic result and both unique cycle diagnostics must remain visible.

The second test turns the repair-cycle-1 global suppression red, while the first prevents simply removing ambiguity deferral. Complete signatures still include diagnostic count, severity, invariant ID, machine code, resource, path, message and sorted remediation context.

Extension ambiguity remains source-local and is covered by the existing divergent-dependency inversion test. The index-addressability oracle remains unchanged.

## Predecessor ownership re-check

The direct accepted predecessor remains HK02A reviewed SHA `f39a1994524c42213dafb63d440faaf9de7c040f`, PASS review `5256593405`, merge `ac7ce1180b462f093cf0ee02bbbd6f853938e27c`; current `main` remains `dbd8121411079f22a01d5cb85345e180ff41f7e2`.

No evidence from either HK05 FAIL invalidates inherited HK02/HK04 guarantees. HK05 owns public aggregate diagnostic semantics; HK02 owns the finite invariant model/evaluator and HK04 owns commit authority/transactionality. Repairing the output dependency boundary in `Arkus.Game.Validation` avoids duplicate predecessor proof or semantic drift.

## Findings fixed so far

The prior nine fixed findings remain closed for their superseded cycles. Repair cycle 2 adds:

10. **Global ambiguity suppression hid independent containment violations.** Repaired by replacing world-wide `ContainmentAcyclic` deferral with a per-diagnostic dependency traversal and adding the connected/disconnected causal regression pair.

## Proof budget

This cycle was required by the explicit foundational circuit breaker. It adds one bounded dependency check to the existing ambiguity policy and one regression method. It does not add graph provenance, per-validator addressing, a second validator, a new registry, or any HK02/HK04 defensive re-proof. The change restores a stated HK05 product behavior — aggregate reporting of independent violations — so it is acceptance progress rather than proof-machinery growth.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.

## Remaining before CLEAN

- canonical observation on the final documentation-reconciled SHA;
- verify Release build, focused `Hk05*` suite, full regression, validation-diagnostics contract and causal negative controls are GREEN;
- inspect the complete baseline-to-candidate diff again, including this repair, for in-claim omissions and forbidden scope;
- record exact observation evidence, then freeze the unchanged exact SHA and run exact-SHA freeze validation.

This Worker does not issue the independent PASS/FAIL verdict.
