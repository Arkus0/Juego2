# WP-HK-05 Worker pre-review

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 10
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-05
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

Baseline: `dbd8121411079f22a01d5cb85345e180ff41f7e2`
Previous failed frozen candidates:
- cycle 1: `e0c865efd2127ca53d9e25064215e09a4579acd4` — review `5257015987`;
- cycle 2: `8c8d8c1a66b4995bbc9f3f933ba1791e49d69e97` — review `5257239932`.
Repair cycle: `2`

Architecture-reconciled observation SHA: `8c95d4e966ab7ee3d9f162161ad5627229edcdf8`.
Actions run `35464667287`: candidate observation GREEN. The canonical `hk05-observe-exact-sha.sh` gate binds the exact checkout and requires locked restore, Release build, all `Hk05*` focused tests, full regression, validation-diagnostics contract, causal negative controls, and clean-before/clean-after state; success means all of those gates completed GREEN.

This CLEAN report is the final evidence reconciliation write. The resulting exact branch SHA must receive one fresh canonical observation unchanged before freeze; no earlier green is reused as the final candidate receipt.

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

`Hk05AmbiguousIdentityRegressionTests` protects both sides of the policy:

- connected ambiguity: the existing `node.b -> node.a` case traverses duplicated `node.a`; reversing the two `node.a` definitions must not expose first-wins-dependent containment output;
- disconnected independence: two divergent `dup.x` entries coexist with unique `cycle.a -> cycle.b -> cycle.a`; reversing only the duplicate entries must preserve the complete semantic result and both unique cycle diagnostics must remain visible.

The second test turns repair-cycle-1 global suppression red, while the first prevents simply removing ambiguity deferral. Complete signatures include diagnostic count, severity, invariant ID, machine code, resource, path, message and sorted remediation context.

Extension ambiguity remains source-local and is covered by the existing divergent-dependency inversion test. The index-addressability oracle remains unchanged.

## Strict effective-path challenge

The complete ambiguity boundary was re-audited, not only the Reviewer fixture:

- `ContainerResolves`, reference-target resolution and extension target/subject resolution are presence checks; they do not select an object representative and therefore do not require world-wide deferral merely because the target ID is duplicated.
- source-local object/extension diagnostics are already deferred whenever their resource identity is ambiguous.
- `ContainmentAcyclic` is the only current owned invariant whose raw result can depend on walking through object representatives. The new dependency replay checks every parent ID before selecting an object for that step.
- if a unique origin reaches a duplicated parent ID, the raw cycle result is deferred regardless of which duplicate the inherited evaluator happened to choose.
- if a cycle closes before any ambiguous ID is encountered, its dependency chain is unique and the diagnostic remains safe/actionable.
- an origin whose own ID is duplicated is filtered before replay, so the replay never chooses between duplicate origins.

This makes duplicate-order inversion semantically stable without suppressing independent components.

## Predecessor ownership re-check

The direct accepted predecessor remains HK02A reviewed SHA `f39a1994524c42213dafb63d440faaf9de7c040f`, PASS review `5256593405`, merge `ac7ce1180b462f093cf0ee02bbbd6f853938e27c`; current `main` remains `dbd8121411079f22a01d5cb85345e180ff41f7e2`.

No evidence from either HK05 FAIL invalidates inherited HK02/HK04 guarantees. HK05 owns public aggregate diagnostic semantics; HK02 owns the finite invariant model/evaluator and HK04 owns commit authority/transactionality. Repairing the output dependency boundary in `Arkus.Game.Validation` avoids duplicate predecessor proof or semantic drift.

## Complete repair-cycle-2 diff audit

Compared with failed frozen SHA `8c8d8c1a66b4995bbc9f3f933ba1791e49d69e97`, repair cycle 2 changes only:

- `src/Arkus.Game.Validation/WorldValidation.cs` — replace global containment suppression with per-diagnostic dependency-local replay;
- `tests/Arkus.Harness.Tests/Hk05AmbiguousIdentityRegressionTests.cs` — add the disjoint unique-cycle regression while retaining the connected ambiguity and extension/actionability controls;
- `WORKER_PLAN.md`, `PROOF_MATRIX.md`, `NEGATIVE_CONFORMANCE_MATRIX.md`, `RESIDUAL_RISK.md`, and this report — circuit-breaker re-audit and evidence reconciliation.

No HK02/HK04 implementation, mutation route/authority code, gameplay schema, Unity path, AI natural-language repair generation, journal/replay implementation, external validator framework, second semantic registry or generalized provenance/index subsystem was added.

## Findings fixed

The prior nine fixed findings remain closed for their superseded cycles. Repair cycle 2 adds:

10. **Global ambiguity suppression hid independent containment violations.** Repaired by replacing world-wide `ContainmentAcyclic` deferral with a per-diagnostic dependency traversal and adding the connected/disconnected causal regression pair.

No further in-claim blocker was found in the strict pre-review after that repair.

## Proof budget

This cycle was required by the explicit foundational circuit breaker. It adds one bounded dependency check to the existing ambiguity policy and one regression method. It does not add graph provenance, per-validator addressing, a second validator, a new registry, or any HK02/HK04 defensive re-proof. The change restores a stated HK05 product behavior — aggregate reporting of independent violations — so it is acceptance progress rather than proof-machinery growth.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.

## Handoff readiness

`WORKER_PRE_REVIEW: CLEAN` applies to the repair-cycle-2 content and proof claims above. The evidence-only commit produced by this report must now pass the canonical candidate observation unchanged. If that exact SHA is GREEN, it may be recorded as Candidate/Frozen SHA, the branch may be frozen and marked Ready, and Automation V2 may run exact-SHA freeze validation for a fresh independent Reviewer.

This Worker does not issue the independent PASS/FAIL verdict.
