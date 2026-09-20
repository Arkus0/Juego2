#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"
cd "${ROOT}"

actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }
[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean before HK09B verification" >&2; exit 2; }

bash scripts/hk09b-observe-exact-sha.sh "${actual}"

grep -Fxq 'FOUNDATIONAL_PROOF_VERDICT: READY' Docs/evidence/WP-HK-09B/PROOF_MATRIX.md
grep -Fxq 'UNRESOLVED_PROOF_OBLIGATIONS: 0' Docs/evidence/WP-HK-09B/PROOF_MATRIX.md
grep -Fxq 'KNOWN_UNDETECTED_DEFECT_CLASSES: 0' Docs/evidence/WP-HK-09B/PROOF_MATRIX.md
grep -Fxq 'PROOF_BUDGET_VERDICT: WITHIN_BUDGET' Docs/evidence/WP-HK-09B/PROOF_MATRIX.md
grep -Fxq 'WORKER_PRE_REVIEW: CLEAN' Docs/evidence/WP-HK-09B/WORKER_PRE_REVIEW.md
grep -Eq '^WORKER_PRE_REVIEW_FINDINGS_FIXED: [0-9]+$' Docs/evidence/WP-HK-09B/WORKER_PRE_REVIEW.md
grep -Fxq 'NEGATIVE_CONFORMANCE_VERDICT: PASS' Docs/evidence/WP-HK-09B/NEGATIVE_CONFORMANCE_MATRIX.md
grep -Fxq 'RESOURCE_ENVELOPE_VERDICT: PASS' Docs/evidence/WP-HK-09B/RESOURCE_ENVELOPE.md
grep -Fxq 'PERSISTENCE_INTERRUPTION_VERDICT: PASS' Docs/evidence/WP-HK-09B/PERSISTENCE_INTERRUPTION.md
grep -Fxq 'CONTENT_SHAPE_PROBE_VERDICT: PASS' Docs/evidence/WP-HK-09B/CONTENT_SHAPE_PROBE.md
grep -Fxq 'APPROVED_PRODUCT_SOURCE: Docs/art/VISUAL_BIBLE.md' Docs/evidence/WP-HK-09B/CONTENT_SHAPE_PROBE.md
grep -Fxq 'UNRESOLVED_IN_SCOPE_FINDINGS: 0' Docs/evidence/WP-HK-09B/CONTENT_SHAPE_PROBE.md
test -f Docs/evidence/WP-HK-09B/RESIDUAL_RISK.md

[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean after HK09B verification" >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-HK-09B
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/hk09b-verify-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: locked-restore=GREEN; release-build=GREEN; resource-envelope=GREEN; persistence-interruption=GREEN; transport-conformance=GREEN; representative-content-shape-probe=GREEN; regression=GREEN; foundational-proof=GREEN; evidence-reconciliation=GREEN; worker-pre-review=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-HK-09B
EOF
