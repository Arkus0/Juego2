#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"
cd "${ROOT}"

actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }
[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean before HK10 verification" >&2; exit 2; }

bash scripts/hk10-observe-exact-sha.sh "${actual}"

grep -Fxq 'CAUSAL_OWNER: WP-HK-01' Docs/evidence/WP-HK-01/DISPATCH_FAILURE_AMENDMENT.md
grep -Fxq 'OWNER_PROOF: Hk01DispatchFailureContractTests' Docs/evidence/WP-HK-01/DISPATCH_FAILURE_AMENDMENT.md
grep -Fxq 'POST_PUBLICATION_MACHINE_CODE: contract.handler_failure_after_publication' Docs/evidence/WP-HK-01/DISPATCH_FAILURE_AMENDMENT.md
grep -Fxq 'PRE_PUBLICATION_MACHINE_CODE: contract.handler_failure' Docs/evidence/WP-HK-01/DISPATCH_FAILURE_AMENDMENT.md
grep -Fxq 'RAW_EXCEPTION_MESSAGE_PUBLIC: NO' Docs/evidence/WP-HK-01/DISPATCH_FAILURE_AMENDMENT.md
grep -Fxq 'FOUNDATIONAL_PROOF_VERDICT: READY' Docs/evidence/WP-HK-10/PROOF_MATRIX.md
grep -Fxq 'UNRESOLVED_PROOF_OBLIGATIONS: 0' Docs/evidence/WP-HK-10/PROOF_MATRIX.md
grep -Fxq 'KNOWN_UNDETECTED_DEFECT_CLASSES: 0' Docs/evidence/WP-HK-10/PROOF_MATRIX.md
grep -Fxq 'PROOF_BUDGET_VERDICT: WITHIN_BUDGET' Docs/evidence/WP-HK-10/PROOF_MATRIX.md
grep -Fxq 'WORKER_PRE_REVIEW: CLEAN' Docs/evidence/WP-HK-10/WORKER_PRE_REVIEW.md
grep -Eq '^WORKER_PRE_REVIEW_FINDINGS_FIXED: [0-9]+$' Docs/evidence/WP-HK-10/WORKER_PRE_REVIEW.md
grep -Fxq 'NEGATIVE_CONTROL_UNIVERSE: 12' Docs/evidence/WP-HK-10/NEGATIVE_CONFORMANCE_MATRIX.md
grep -Fxq 'KNOWN_UNRESOLVED_FALSE_GREEN_CONTROLS: 0' Docs/evidence/WP-HK-10/NEGATIVE_CONFORMANCE_MATRIX.md
grep -Fxq 'RESIDUAL_LEDGER_RECONCILIATION: COMPLETE' Docs/evidence/WP-HK-10/RESIDUAL_RISK.md
grep -Fxq 'UNCLASSIFIED_RESIDUALS: 0' Docs/evidence/WP-HK-10/RESIDUAL_RISK.md
grep -Fxq 'COMPATIBILITY_CORPUS: COMPLETE' Docs/evidence/WP-HK-10/COMPATIBILITY_CORPUS.md
grep -Fxq 'CONTENT_SHAPE_PROBE_VERDICT: PASS' Docs/evidence/WP-HK-10/CONTENT_SHAPE_PROBE.md
grep -Fxq 'APPROVED_PRODUCT_SOURCE: Docs/art/VISUAL_BIBLE.md' Docs/evidence/WP-HK-10/CONTENT_SHAPE_PROBE.md
grep -Fxq 'UNRESOLVED_IN_SCOPE_FINDINGS: 0' Docs/evidence/WP-HK-10/CONTENT_SHAPE_PROBE.md
grep -Fxq 'ENDURANCE_VERDICT: PASS' Docs/evidence/WP-HK-10/ENDURANCE.md

[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean after HK10 verification" >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-HK-10
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/hk10-verify-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: locked-restore=GREEN; release-build=GREEN; hk01-dispatch-failure-amendment=GREEN; hk10-property-robustness=GREEN; hk10-endurance-compatibility=GREEN; representative-content-shape-probe=GREEN; causal-negative-controls=GREEN; regression=GREEN; foundational-proof=GREEN; residual-reconciliation=GREEN; worker-pre-review=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-HK-01/DISPATCH_FAILURE_AMENDMENT.md; Docs/evidence/WP-HK-10
EOF
