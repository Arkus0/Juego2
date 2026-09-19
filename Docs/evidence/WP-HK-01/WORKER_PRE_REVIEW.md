# WP-HK-01 Worker adversarial pre-review

Baseline SHA: `9db7a1ffffa02c02af7889de29960de1f19d2755`  
Repair-cycle-2 implementation observation SHA: `cfcb9ab25977a1f58cc85554c5150d98574e01e3`  
Observation run: GitHub Actions `35435242336`

## Scope reread

Re-read before evidence reconciliation/freeze:

- `Docs/workpacks/HK/WP-HK-01.md`
- `Docs/engineering/PRODUCT_ARCHITECTURE.md`
- `Docs/engineering/WORKER_REVIEW_PROTOCOL.md`
- `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`
- `Docs/engineering/EXECUTION_RECEIPT_PROTOCOL.md`
- both independent FAIL histories, including rejected candidate `c0a097c570b91f2141c447f9c3751d6322c1fc10`
- complete baseline `9db7a1f...` → candidate `cfcb9ab...` file set, runtime/protocol code, positive tests, causal controls and HK01 evidence.

The candidate remains inside HK01: canonical contract/composition/discovery/dispatch conformance only. It adds no world semantics, gameplay commands, concrete Unity/DFU API, MCP host or production network server.

## Circuit-breaker re-audit

The second FAIL was treated as architectural. The Worker re-audited two complete causal boundaries rather than adding example strings/cases:

1. **Portability / logical references.** A legal schema format can no longer carry arbitrary CLR/assembly identity metadata. Logical-reference namespaces occupy canonical `ref.<provider-id>.<domain>` space; schema definition validation rejects non-canonical namespaces and implementation-choice enums, while composition recursively verifies provider ownership in request/success/error schemas. Runtime portable-data validation still prevents actual CLR/engine objects from crossing even permissive `Any` nodes.
2. **Evolution / negotiation.** Compatibility is now a recursive acceptance-preservation relation over the supported request-schema model, not an “optional-property means additive” heuristic. Open-object typed-property additions (including nested variants), enum narrowing, required-member additions, closing objects and other narrowing transformations fail conservative compatibility. Composition evaluates adjacent same-major versions and refuses a breaking chain before negotiation can exist.

Major versions remain explicit breaking boundaries. Conservative false-breaking outcomes are allowed; a known false-compatible same-major edge is not.

## Falsification performed

The Worker challenged canonical ownership, schema/result/error completeness, provider/scope/namespace conflicts, logical-reference portability and ownership, arbitrary implementation-object passage, handler binding truth, version chain ordering, nested/open-object request narrowing, discovery completeness, independent-universe shrinkage, projection drift, structural comparison ambiguity and exact-SHA validation.

The full baseline-to-candidate set was inspected rather than only the last repair. No third-party dependency or project reference was introduced. Protocol remains engine/transport neutral and Runtime depends on Protocol, not vice versa.

## Material findings repaired across Worker/review history

1. Removed unnecessary Protocol→Runtime internal-access coupling; exposed portable identity rules from Protocol.
2. Bound registered route identity to independently declared handler metadata.
3. Removed provider-ID filtering from the independent route universe.
4. Extended completeness proof to independently discover current production projects/outputs and synthetic scoped surfaces.
5. Added fail-closed scoped `scope` ownership alongside namespace/capability identity checks.
6. Added recursive portable-data validation so `Any` cannot carry CLR/engine objects.
7. Made unchanged-version semantic changes breaking.
8. Added direct `system.describe` execution over accepted scoped inventory.
9. Removed serialized fingerprints from correctness decisions after collision was demonstrated; structural equality owns correctness.
10. Bound conformance to the actual portable `system.describe` artifact with an independent expected-data oracle.
11. Replaced permissive logical-reference annotation strings with provider-owned canonical `ref.<provider-id>.<domain>` identities; reject assembly-qualified/noncanonical namespaces, foreign-provider namespaces and logical-reference enums.
12. Replaced optional-property compatibility heuristic with recursive request-acceptance preservation and made composer enforce every adjacent same-major edge before contract construction/negotiation.
13. During this pre-review, detected that the seven new repair controls initially passed only in full regression because their test class did not match the canonical causal-control filter. Renamed the class under `Hk01SelfAttackTests*`; exact-SHA Actions now proves 29/29 causal controls rather than the stale 22/22.

These findings map directly to HK01 acceptance or observed false-green history. No known in-claim blocker remains.

## Validation state

Canonical observation on repair implementation head `cfcb9ab25977a1f58cc85554c5150d98574e01e3`:

- locked restore: GREEN
- Release build: GREEN, 0 warnings / 0 errors
- canonical positive tests: 9/9 GREEN
- causal negative controls: 29/29 GREEN
- total regression: 39/39 GREEN
- candidate clean before/after: YES
- Actions run: `35435242336`

The evidence-bearing candidate created by this reconciliation must still pass `scripts/hk01-verify-exact-sha.sh` before freeze. That verifier reruns canonical observation and requires the foundational/evidence/pre-review markers committed here.

## Residual-risk / proof-budget check

No arbitrary engine-word denylist was added: such a list would be brittle proof growth and could not infer semantic intent. Instead the mechanical claim is bounded to canonical data shape/identity, provider ownership, and reviewed contribution admission. Cross-provider reference vocabularies, richer schema keywords and concrete engine semantics remain explicit later boundaries.

Repair cycle 2 materially changes product admission behavior (portable reference ownership + compatibility-governed composition) and adds causal controls directly for the observed false-green classes. The only proof-only addition was fixing the causal-test filter membership discovered by pre-review. The proof remains simpler than the behavior it protects and has converged.

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 13
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-01/WORKER_PRE_REVIEW.md
PROOF_BUDGET_VERDICT: WITHIN_BUDGET
