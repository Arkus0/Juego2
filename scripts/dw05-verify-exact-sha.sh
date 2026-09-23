#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"
cd "${ROOT}"

candidate_dirty_status() {
  git status --porcelain --untracked-files=all | \
    grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true
}

actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }
[[ -z "$(candidate_dirty_status)" ]] || { echo "Candidate is not clean before DW-05 verification" >&2; candidate_dirty_status >&2; exit 2; }

bash scripts/dw05-observe-exact-sha.sh "${actual}"

grep -Fxq 'FOUNDATIONAL_PROOF_VERDICT: READY' Docs/evidence/WP-DW-05/PROOF_MATRIX.md
grep -Fxq 'UNRESOLVED_PROOF_OBLIGATIONS: 0' Docs/evidence/WP-DW-05/PROOF_MATRIX.md
grep -Fxq 'KNOWN_UNDETECTED_DEFECT_CLASSES: 0' Docs/evidence/WP-DW-05/PROOF_MATRIX.md
grep -Fxq 'PROOF_BUDGET_VERDICT: WITHIN_BUDGET' Docs/evidence/WP-DW-05/PROOF_MATRIX.md
grep -Fq 'PREDECESSOR_CONTRACT_CHECK: PASS' Docs/evidence/WP-DW-05/PREDECESSOR_CONTRACT_CHECK.md
grep -Fq 'GENERIC_BOUNDARY_CONCLUSION: SURVIVES_BOUNDED_STRESS' Docs/evidence/WP-DW-05/GENERIC_BOUNDARY_AUDIT.md
grep -Fq 'MATERIAL_GENERIC_CONTRADICTIONS: 0' Docs/evidence/WP-DW-05/GENERIC_BOUNDARY_AUDIT.md
grep -Fq 'PROOF_BUDGET_VERDICT: WITHIN_BUDGET' Docs/evidence/WP-DW-05/RESIDUAL_RISK.md
grep -Fq 'GENERAL_DW_ADOPTION_AUTHORIZED: NO' Docs/evidence/WP-DW-05/H2_BOUNDARY_INPUT_V1.md

if [[ -n "${PR_BODY:-}" ]]; then
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Candidate HEAD SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Frozen candidate SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Worker pre-review:[[:space:]]*CLEAN[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Branch frozen:[[:space:]]*YES[[:space:]]*$'
fi

[[ -z "$(candidate_dirty_status)" ]] || { echo "Candidate is not clean after DW-05 verification" >&2; candidate_dirty_status >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-DW-05
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/dw05-verify-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: observation=GREEN; foundational-proof=GREEN; predecessor-check=GREEN; generic-boundary-audit=GREEN; no-material-generic-contradiction=GREEN; residual-risk=GREEN; h2-nonadoption-boundary=GREEN; frozen-metadata=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-DW-05
EOF
