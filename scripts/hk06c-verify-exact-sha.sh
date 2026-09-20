#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"
cd "${ROOT}"

actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }
[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean before HK06C verification" >&2; exit 2; }

bash scripts/hk06c-observe-exact-sha.sh "${actual}"

grep -Fxq 'FOUNDATIONAL_PROOF_VERDICT: READY' Docs/evidence/WP-HK-06C/PROOF_MATRIX.md
grep -Fxq 'UNRESOLVED_PROOF_OBLIGATIONS: 0' Docs/evidence/WP-HK-06C/PROOF_MATRIX.md
grep -Fxq 'KNOWN_UNDETECTED_DEFECT_CLASSES: 0' Docs/evidence/WP-HK-06C/PROOF_MATRIX.md
grep -Fxq 'PROOF_BUDGET_VERDICT: WITHIN_BUDGET' Docs/evidence/WP-HK-06C/PROOF_MATRIX.md
grep -Fxq 'WORKER_PRE_REVIEW: CLEAN' Docs/evidence/WP-HK-06C/WORKER_PRE_REVIEW.md

[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean after HK06C verification" >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-HK-06C
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/hk06c-verify-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: locked-restore=GREEN; release-build=GREEN; replay-final-hash=GREEN; semantic-diff-identity=GREEN; journal-sequence-integrity=GREEN; replay-atomicity=GREEN; compatibility-policy=GREEN; causal-negative-controls=GREEN; regression=GREEN; foundational-proof=GREEN; evidence-reconciliation=GREEN; worker-pre-review=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-HK-06C
EOF
