#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"

cd "${ROOT}"
actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }

[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean before validation" >&2; exit 2; }

bash scripts/hk00a-architecture-check.sh
bash scripts/self-attacks/run-hk00a-architecture-attacks.sh

test -f Docs/evidence/WP-HK-00A/PROOF_MATRIX.md
test -f Docs/evidence/WP-HK-00A/SELF_ATTACKS.md
test -f Docs/evidence/WP-HK-00A/RESIDUAL_RISK.md
test -f Docs/evidence/WP-HK-00A/WORKER_PRE_REVIEW.md

grep -Fq 'FOUNDATIONAL_PROOF_VERDICT: READY' Docs/evidence/WP-HK-00A/PROOF_MATRIX.md
grep -Fq 'UNRESOLVED_PROOF_OBLIGATIONS: 0' Docs/evidence/WP-HK-00A/PROOF_MATRIX.md
grep -Fq 'KNOWN_UNDETECTED_DEFECT_CLASSES: 0' Docs/evidence/WP-HK-00A/PROOF_MATRIX.md
grep -Fq 'PROOF_BUDGET_VERDICT: WITHIN_BUDGET' Docs/evidence/WP-HK-00A/PROOF_MATRIX.md
grep -Fq 'WORKER_PRE_REVIEW: CLEAN' Docs/evidence/WP-HK-00A/WORKER_PRE_REVIEW.md

[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean after validation" >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-HK-00A
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution substrate: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
OS: $(uname -srm)
Canonical command: scripts/hk00a-verify-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: architecture-consistency=GREEN; causal-negative-controls=GREEN; foundational-evidence=GREEN; worker-pre-review=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-HK-00A
EOF
