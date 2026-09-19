# WP-HK-05 Worker pre-review

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 9
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-05
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

Baseline: `dbd8121411079f22a01d5cb85345e180ff41f7e2`
Previous failed frozen candidate: `e0c865efd2127ca53d9e25064215e09a4579acd4`
Repair cycle: `1`

Repair implementation + regression observation: exact SHA `06f749b2b8b700803b955dd394a08d7e5d58c3fe`, Actions `35461795461` — Release build 0 warnings / 0 errors, original focused HK05 contract suite 7/7 GREEN, full regression 109/109 GREEN, canonical receipt `Result: GREEN`.

That observation also exposed an evidence-gate gap: the focused command still named only `Hk05ValidationDiagnosticsTests`, so the three new ambiguous-identity tests were proven only by full regression. The Worker corrected the filter to `FullyQualifiedName~Hk05`, which includes the contract tests, the bounded content-shape probe and the ambiguity regression class. The exact documentation-reconciled SHA resulting from this report must receive a fresh canonical observation under that corrected gate and then a fresh exact-SHA freeze validation before handoff. No previous green is reusable as the frozen-candidate receipt.

## Contract and predecessor re-check

Re-read `WP-HK-05`, `AGENTS.md`, `WORKER_REVIEW_PROTOCOL.md` v1.6, `FOUNDATIONAL_PROOF_STANDARD.md` v1.3, the direct accepted HK02A contract/evidence, and the inherited HK01/HK02/HK03/HK04 guarantees materially consumed by HK05.

The predecessor split remains unchanged:

- direct predecessor HK02A reviewed SHA `f39a1994524c42213dafb63d440faaf9de7c040f`, independent PASS review `5256593405`, merge `ac7ce1180b462f093cf0ee02bbbd6f853938e27c`;
- HK05 consumes accepted route composition/discovery, finite canonical state/codec, inspection, duplicate-ID invalidity and closed transactional commit authority;
- HK05 owns aggregate validator/diagnostic completeness, actionability, deterministic multi-violation semantics, explicit current/proposed validation and validation-before-commit behavior;
- the Reviewer FAIL supplied no concrete evidence that HK02/HK04 guarantees were false, so those boundaries were not reopened.

## Repair-cycle causal finding

Reviewer FAIL `5257015987` on `e0c865…` proved one material HK05-owned class: **aggregate validation under ambiguous identity**.

The failure was not the literal `node.a` fixture. Duplicate object identity allowed secondary diagnostics to share identity-based resource/path while differing in remediation context, and containment traversal used a first-wins representative. Reversing duplicate entries could therefore change ordering and, more seriously, diagnostic set/count.

The adopted semantic policy is deterministic deferral:

1. detect duplicated object IDs and extension identities before public aggregate diagnostics are materialized;
2. always emit the duplicate-identity defect itself;
3. rewrite that defect to an index-addressable source location and preserve `firstIndex` / `duplicateIndex` plus the duplicated semantic identity;
4. defer secondary diagnostics whose source resource is ambiguous;
5. while any object identity is ambiguous, defer containment-cycle diagnostics globally because traversal through a duplicated target would otherwise require choosing a representative;
6. sort remaining diagnostics using remediation context as the final stable comparison field.

This fixes the semantic class without per-validator provenance/index infrastructure. A client receives a concrete entry to repair, fixes identity, and revalidates to reveal any deferred secondary errors.

## Causal regression and stronger oracle

`Hk05AmbiguousIdentityRegressionTests` covers both material variants of the same cause:

- **objects:** two `node.a` entries have different container defects while `node.b -> node.a` makes first-wins containment traversal observably divergent; reversing only the duplicate entries must preserve the complete semantic result;
- **extensions:** two extensions with the same owner/schema/subject identity have different dangling dependencies; reversing them must likewise preserve the complete semantic result.

The equality oracle includes diagnostic count and every emitted semantic field: severity, invariant ID, machine code, resource, path, message and sorted remediation context.

The ambiguity oracle is no longer “non-empty means actionable.” A synthetic duplicate diagnostic with non-empty resource/path/context is rejected unless it carries distinct source indices and its resource/path identify the concrete duplicate index. Effective object duplicates additionally carry `duplicateId`; effective extension duplicates carry `duplicateIdentity`.

## Complete repair diff audit

The exact failed-candidate → repair diff was inspected. Before this report it contains only:

- `src/Arkus.Game.Validation/WorldValidation.cs` — ambiguity classification, deterministic deferral, index-addressable duplicate materialization and remediation-context sorting;
- `tests/Arkus.Harness.Tests/Hk05AmbiguousIdentityRegressionTests.cs` — one causal regression class covering object + extension identity and the strengthened actionability oracle;
- `scripts/hk05-observe-exact-sha.sh` — one-line focused-filter broadening so every `Hk05*` test participates in the focused gate;
- `PROOF_MATRIX.md`, `NEGATIVE_CONFORMANCE_MATRIX.md`, `RESIDUAL_RISK.md` and this report — evidence reconciliation.

No HK02/HK04 implementation, route/commit authority, gameplay schema, Unity path, AI repair generation, journal/replay, external validator framework or generalized provenance/index subsystem was added.

## Effective-path check

Public current/proposed validation and mutation rejection materialize diagnostics through `WorldValidationEngine.ValidateCandidate`, which now owns the ambiguity policy before sorting/serialization. The raw `WorldStateValidator` remains the finite invariant evaluator; its constructor compatibility path is throw-first rather than the public aggregate-diagnostic contract, and duplicate identity is registered before identity-dependent secondary evaluators. No second public aggregate route was found that bypasses `WorldValidationEngine`.

The existing validate/apply agreement test still proves that explicit proposal validation and canonical apply use the same effective aggregate diagnostics. Existing HK03/HK04 conformance universes remain unchanged by this repair and full regression is GREEN on the repair implementation observation.

## Findings fixed before CLEAN

The original pre-review fixed seven findings recorded in the prior report. Repair cycle 1 adds two more:

8. Reviewer-discovered ambiguous-identity aggregation could be order-dependent and non-actionable; repaired at the aggregate semantic boundary with deterministic deferral, index-addressable duplicate diagnostics and object/extension causal regressions.
9. The canonical focused HK05 observation command still selected only the original contract test class, so the new repair regressions would have relied on the full suite alone; broadened to all `Hk05*` tests.

## Rejection sites, residual risk and proof budget

`NEGATIVE_CONFORMANCE_MATRIX.md` now records the duplicate-object causal mutant, the equivalent duplicate-extension variant and the stronger non-empty-but-ambiguous diagnostic mutant. `RESIDUAL_RISK.md` makes deferred secondary diagnostics an explicit semantic policy rather than hiding them as a risk: repair identity first, then revalidate.

Proof machinery remains proportional. The repair adds one small ambiguity classification and one causal regression class; it does not introduce per-validator addressing machinery or a provenance layer. Accepted predecessor completeness/authority mechanisms are consumed, not re-proved.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.

## Handoff readiness

No known in-claim blocker remains after the repair-cycle challenge. The implementation/regression SHA `06f749b…` is canonically GREEN with full regression 109/109. The resulting exact SHA containing the corrected focused gate plus reconciled evidence must now itself pass canonical observation, then be recorded unchanged as Candidate/Frozen SHA and pass exact-SHA freeze validation before the PR returns to Ready.

This Worker does not issue the independent PASS/FAIL verdict.
