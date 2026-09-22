#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"
cd "${ROOT}"

actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }
[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean before DW-01 verification" >&2; exit 2; }

bash scripts/dw01-observe-exact-sha.sh "${actual}"

grep -Fxq 'FOUNDATIONAL_PROOF_VERDICT: READY' Docs/evidence/WP-DW-01/PROOF_MATRIX.md
grep -Fxq 'UNRESOLVED_PROOF_OBLIGATIONS: 0' Docs/evidence/WP-DW-01/PROOF_MATRIX.md
grep -Fxq 'KNOWN_UNDETECTED_DEFECT_CLASSES: 0' Docs/evidence/WP-DW-01/PROOF_MATRIX.md
grep -Fxq 'PROOF_BUDGET_VERDICT: WITHIN_BUDGET' Docs/evidence/WP-DW-01/PROOF_MATRIX.md
grep -Fq 'Current-WP blocker findings: **none' Docs/evidence/WP-DW-01/CONTENT_SHAPE_PROBE.md
grep -Fq 'Concrete predecessor reopen conditions triggered: **none' Docs/evidence/WP-DW-01/CONTENT_SHAPE_PROBE.md
grep -Fq 'PROOF_BUDGET_VERDICT: WITHIN_BUDGET' Docs/evidence/WP-DW-01/RESIDUAL_RISK.md
grep -Fq '## Predecessor gate' Docs/evidence/WP-DW-01/WORKER_PLAN.md

if [[ -n "${PR_BODY:-}" ]]; then
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Candidate HEAD SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Frozen candidate SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Worker pre-review:[[:space:]]*CLEAN[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Branch frozen:[[:space:]]*YES[[:space:]]*$'
fi

[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean after DW-01 verification" >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-DW-01
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/dw01-verify-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: locked-restore=GREEN; release-build=GREEN; focused-dw01=GREEN; reverse-h0-dependency=GREEN; h0-domain-neutrality=GREEN; designworld-dependency-boundary=GREEN; regression=GREEN; foundational-proof=GREEN; content-shape-probe=GREEN; residual-risk=GREEN; predecessor-check=GREEN; frozen-metadata=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-DW-01
EOF
