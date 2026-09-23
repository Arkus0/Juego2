#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"
cd "${ROOT}"

actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }

bash scripts/dwgate-observe-exact-sha.sh "${actual}"

grep -Fxq 'FOUNDATIONAL_PROOF_VERDICT: READY' Docs/evidence/WP-DW-GATE/PROOF_MATRIX.md
grep -Fxq 'UNRESOLVED_PROOF_OBLIGATIONS: 0' Docs/evidence/WP-DW-GATE/PROOF_MATRIX.md
grep -Fxq 'KNOWN_UNDETECTED_DEFECT_CLASSES: 0' Docs/evidence/WP-DW-GATE/PROOF_MATRIX.md
grep -Fxq 'PREDECESSOR_RESIDUAL_RECONCILIATION: COMPLETE' Docs/evidence/WP-DW-GATE/PROOF_MATRIX.md
grep -Fxq 'H2_HANDOFF_RECONCILIATION: COMPLETE' Docs/evidence/WP-DW-GATE/PROOF_MATRIX.md

python3 scripts/dwgate-evidence-audit.py

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-DW-GATE
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-remote-shell}
Canonical command: scripts/dwgate-verify-exact-sha.sh ${actual}
Required gates: deterministic-composed-observation=GREEN; predecessor-residual-reconciliation=GREEN; h2-handoff=GREEN; foundational-proof=GREEN
FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
Result: GREEN
Evidence: Docs/evidence/WP-DW-GATE
EOF
