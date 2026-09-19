# WP-HK-01 Worker adversarial pre-review

Baseline SHA: `9db7a1ffffa02c02af7889de29960de1f19d2755`  
Repair implementation observation SHA: `38783856d89a9ec7cff376086ae8702b0bb72501`

Observation run: GitHub Actions `35434057309`

## Scope reread

Re-read before freeze:

- `Docs/workpacks/HK/WP-HK-01.md`
- `Docs/engineering/PRODUCT_ARCHITECTURE.md`
- `Docs/engineering/WORKER_REVIEW_PROTOCOL.md`
- `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`
- `Docs/engineering/EXECUTION_RECEIPT_PROTOCOL.md`
- independent FAIL on rejected candidate `5a39252487328e74b8cd94e17b31f7b68af9e048`
- complete baseline-to-candidate diff, runtime/protocol code, positive tests, negative controls and HK01 evidence.

The candidate stays inside HK01: canonical contract/composition/discovery/dispatch conformance only. It adds no world semantics, gameplay commands, concrete Unity/DFU API, MCP host or production network server.

## Falsification performed

The Worker challenged: canonical ownership, schema/result/error completeness, scoped-provider admission, namespace/scope/identity conflicts, handler binding truth, runtime version/request fail-closed behavior, portable data, discovery completeness, independent-universe shrinkage, projection semantic drift, version compatibility, collision/ambiguity in comparison encodings, shared-generator self-confirmation and the exact-SHA validation route.

The complete diff was checked rather than only the latest commits. No third-party dependency or project reference was added. Protocol remains transport/engine neutral; Runtime depends on Protocol and owns composition/dispatch implementation rather than reversing that dependency.

## Material findings repaired during adversarial work

1. Removed an unnecessary Protocol→Runtime internal-access coupling and exposed only portable identity rules from Protocol.
2. Bound registered route identity to independently declared handler route metadata so a handler cannot be registered under a false version/identity.
3. Removed provider-ID filtering from the independent route universe; unknown providers can no longer erase themselves from completeness proof.
4. Extended the positive completeness proof to independently discover every current production project/output under `src` and to prove an accepted synthetic base + scoped surface end to end.
5. Added fail-closed scoped `scope` ownership in addition to provider/namespace/capability identity conflict checks.
6. Added recursive portable-data validation so permissive schema nodes cannot carry CLR/engine implementation objects through the canonical boundary.
7. Tightened compatibility so semantic changes at an unchanged contract version are breaking rather than silently additive.
8. Added direct `system.describe` execution over the accepted synthetic composed scoped inventory, proving discovery rather than only inspecting the projection object.
9. Removed serialized fingerprints from every correctness decision after independent review demonstrated a valid delimiter-bearing collision. Capability/schema compatibility and canonical error-schema validation now use exact structural equality; diagnostic fingerprints use unambiguous length/cardinality framing.
10. Replaced projection-object self-comparison with evaluation of the portable artifact actually returned by canonical `system.describe`. Its expected-data oracle independently traverses the canonical model and does not call production `ToData()` projectors, so omitted or remapped optional fields cannot define their own proof expectation.

These repairs target causal acceptance gaps. No known in-claim blocker remains.

## Validation state

Canonical observation on repair implementation head `38783856d89a9ec7cff376086ae8702b0bb72501`:

- locked restore: GREEN
- Release build: GREEN, 0 warnings / 0 errors
- canonical positive tests: 9/9 GREEN
- causal negative controls: 22/22 GREEN
- total regression: 32/32 GREEN
- candidate clean before/after: YES

The final evidence-bearing HEAD must still pass `scripts/hk01-verify-exact-sha.sh` before review. That verifier reruns canonical observation and requires the foundational/evidence/pre-review markers committed here.

## Proof budget

The proof architecture converged after fixing the self-shrinking provider filter and the independently observed semantic/projection false-green. The repair adds one reusable structural comparer, unambiguous diagnostic framing and one finite independent artifact oracle covering the complete existing public shape. Those additions map directly to HK01's compatibility and generated-artifact criteria; no syntax denylist, transport implementation or trusted-infrastructure hardening was added.

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 10
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-01/WORKER_PRE_REVIEW.md
PROOF_BUDGET_VERDICT: WITHIN_BUDGET
