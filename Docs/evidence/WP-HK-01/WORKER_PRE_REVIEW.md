# WP-HK-01 Worker adversarial pre-review

Baseline SHA: `9db7a1ffffa02c02af7889de29960de1f19d2755`  
Implementation observation SHA: `6cf1e6c5cdd76946f57a92b9d56a6b77267fc1dd`  
Observation run: GitHub Actions `35432812488`

## Scope reread

Re-read before freeze:

- `Docs/workpacks/HK/WP-HK-01.md`
- `Docs/engineering/PRODUCT_ARCHITECTURE.md`
- `Docs/engineering/WORKER_REVIEW_PROTOCOL.md`
- `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`
- complete baseline-to-candidate diff, runtime/protocol code, positive tests, negative controls and HK01 evidence.

The candidate stays inside HK01: canonical contract/composition/discovery/dispatch conformance only. It adds no world semantics, gameplay commands, concrete Unity/DFU API, MCP host or production network server.

## Falsification performed

The Worker challenged: canonical ownership, schema/result/error completeness, scoped-provider admission, namespace/scope/identity conflicts, handler binding truth, runtime version/request fail-closed behavior, portable data, discovery completeness, independent-universe shrinkage, projection semantic drift, version compatibility and the exact-SHA validation route.

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

These repairs target causal acceptance gaps. No known in-claim blocker remains.

## Validation state

Canonical observation on implementation head `6cf1e6c5cdd76946f57a92b9d56a6b77267fc1dd`:

- locked restore: GREEN
- Release build: GREEN, 0 warnings / 0 errors
- canonical positive tests: 9/9 GREEN
- causal negative controls: 18/18 GREEN
- total regression: 28/28 GREEN
- candidate clean before/after: YES

The final evidence-bearing HEAD must still pass `scripts/hk01-verify-exact-sha.sh` before review. That verifier reruns canonical observation and requires the foundational/evidence/pre-review markers committed here.

## Proof budget

The proof architecture converged after fixing the self-shrinking provider filter. Subsequent additions correspond to explicit HK01 acceptance gaps, not hypothetical attacks on trusted infrastructure.

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 8
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-01/WORKER_PRE_REVIEW.md
PROOF_BUDGET_VERDICT: WITHIN_BUDGET
