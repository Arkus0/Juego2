#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"
cd "${ROOT}"

actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }
[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean before H1-01 verification" >&2; exit 2; }

bash scripts/h1-01-observe-exact-sha.sh "${actual}"

grep -Fxq 'FOUNDATIONAL_PROOF_VERDICT: READY' Docs/evidence/WP-H1-01/PROOF_MATRIX.md
grep -Fxq 'UNRESOLVED_PROOF_OBLIGATIONS: 0' Docs/evidence/WP-H1-01/PROOF_MATRIX.md
grep -Fxq 'KNOWN_UNDETECTED_DEFECT_CLASSES: 0' Docs/evidence/WP-H1-01/PROOF_MATRIX.md
grep -Fxq 'PROOF_BUDGET_VERDICT: WITHIN_BUDGET' Docs/evidence/WP-H1-01/PROOF_MATRIX.md
grep -Fxq 'WORKER_PRE_REVIEW: CLEAN' Docs/evidence/WP-H1-01/WORKER_PRE_REVIEW.md
grep -Eq '^WORKER_PRE_REVIEW_FINDINGS_FIXED: [0-9]+$' Docs/evidence/WP-H1-01/WORKER_PRE_REVIEW.md
grep -Fxq 'Probe status: `COMPLETE`' Docs/evidence/WP-H1-01/CONTENT_SHAPE_PROBE.md
grep -Fxq 'RESIDUAL_RECONCILIATION: `COMPLETE`' Docs/evidence/WP-H1-01/RESIDUAL_RISK.md
grep -Fxq 'UNCLASSIFIED_RESIDUALS: `0`' Docs/evidence/WP-H1-01/RESIDUAL_RISK.md
grep -Fxq 'PREDECESSOR_REOPEN_TRIGGERED: `NO`' Docs/evidence/WP-H1-01/RESIDUAL_RISK.md

if [[ -n "${PR_BODY:-}" ]]; then
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Candidate HEAD SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Frozen candidate SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Worker pre-review:[[:space:]]*CLEAN[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Branch frozen:[[:space:]]*YES[[:space:]]*$'
fi

[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean after H1-01 verification" >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-H1-01
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/h1-01-verify-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: locked-restore=GREEN; release-build=GREEN; focused-unity-producer=GREEN; potes-content-probe=GREEN; hk02a-integration=GREEN; jsonl-mcp-delta=GREEN; portable-unity-boundary=GREEN; causal-negative-controls=GREEN; regression=GREEN; foundational-proof=GREEN; residual-reconciliation=GREEN; worker-pre-review=GREEN; frozen-metadata=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-H1-01
EOF
