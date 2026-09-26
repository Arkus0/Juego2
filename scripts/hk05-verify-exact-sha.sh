#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"
cd "${ROOT}"
# Candidate Validation writes its own receipt files (VALIDATION_CONTEXT.json, validation.log) before invoking this
# verifier; they are tolerated exactly as in the newer exact-SHA verifiers (for example h1-04-verify-exact-sha.sh).

actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }
[[ -z "$(git status --porcelain --untracked-files=all | grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true)" ]] || { echo "Candidate is not clean before HK05 verification" >&2; exit 2; }

bash scripts/hk05-observe-exact-sha.sh "${actual}"

grep -Fxq 'FOUNDATIONAL_PROOF_VERDICT: READY' Docs/evidence/WP-HK-05/PROOF_MATRIX.md
grep -Fxq 'UNRESOLVED_PROOF_OBLIGATIONS: 0' Docs/evidence/WP-HK-05/PROOF_MATRIX.md
grep -Fxq 'KNOWN_UNDETECTED_DEFECT_CLASSES: 0' Docs/evidence/WP-HK-05/PROOF_MATRIX.md
grep -Fxq 'PROOF_BUDGET_VERDICT: WITHIN_BUDGET' Docs/evidence/WP-HK-05/PROOF_MATRIX.md
grep -Fxq 'WORKER_PRE_REVIEW: CLEAN' Docs/evidence/WP-HK-05/WORKER_PRE_REVIEW.md

[[ -z "$(git status --porcelain --untracked-files=all | grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true)" ]] || { echo "Candidate is not clean after HK05 verification" >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-HK-05
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/hk05-verify-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: locked-restore=GREEN; release-build=GREEN; validation-diagnostics-contract=GREEN; causal-negative-controls=GREEN; regression=GREEN; foundational-proof=GREEN; evidence-reconciliation=GREEN; worker-pre-review=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-HK-05
EOF
