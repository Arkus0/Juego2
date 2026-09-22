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
[[ -z "$(candidate_dirty_status)" ]] || { echo "Candidate is not clean before DW-03 verification" >&2; candidate_dirty_status >&2; exit 2; }

bash scripts/dw03-observe-exact-sha.sh "${actual}"

grep -Fxq 'FOUNDATIONAL_PROOF_VERDICT: READY' Docs/evidence/WP-DW-03/PROOF_MATRIX.md
grep -Fxq 'UNRESOLVED_PROOF_OBLIGATIONS: 0' Docs/evidence/WP-DW-03/PROOF_MATRIX.md
grep -Fxq 'KNOWN_UNDETECTED_DEFECT_CLASSES: 0' Docs/evidence/WP-DW-03/PROOF_MATRIX.md
grep -Fxq 'PROOF_BUDGET_VERDICT: WITHIN_BUDGET' Docs/evidence/WP-DW-03/PROOF_MATRIX.md
grep -Fq 'PREDECESSOR_CONTRACT_CHECK: PASS' Docs/evidence/WP-DW-03/PREDECESSOR_CONTRACT_CHECK.md
grep -Fq 'FROZEN_QUERY_SUITE: `dw03-pa-query-suite-v1`' Docs/evidence/WP-DW-03/QUERY_SUITE_V1.md
grep -Fq 'FINAL_CIRCUIT_BREAKER_AUDIT: CLEAN' Docs/evidence/WP-DW-03/FINAL_CIRCUIT_BREAKER_AUDIT.md
grep -Fq 'PROOF_BUDGET_VERDICT: WITHIN_BUDGET' Docs/evidence/WP-DW-03/RESIDUAL_RISK.md

if [[ -n "${PR_BODY:-}" ]]; then
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Candidate HEAD SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Frozen candidate SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Worker pre-review:[[:space:]]*CLEAN[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Branch frozen:[[:space:]]*YES[[:space:]]*$'
fi

[[ -z "$(candidate_dirty_status)" ]] || { echo "Candidate is not clean after DW-03 verification" >&2; candidate_dirty_status >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-DW-03
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/dw03-verify-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: locked-restore=GREEN; release-build=GREEN; focused-dw03=GREEN; reverse-h0-dependency=GREEN; h0-domain-neutrality=GREEN; designworld-dependency-boundary=GREEN; pa-authority-immutability=GREEN; regression=GREEN; foundational-proof=GREEN; predecessor-check=GREEN; frozen-query-suite=GREEN; final-circuit-breaker=GREEN; residual-risk=GREEN; frozen-metadata=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-DW-03
EOF
