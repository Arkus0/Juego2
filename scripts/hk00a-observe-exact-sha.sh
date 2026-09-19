#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"

cd "${ROOT}"
actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }

[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean before observation" >&2; exit 2; }

bash scripts/hk00a-architecture-check.sh
bash scripts/self-attacks/run-hk00a-architecture-attacks.sh

[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean after observation" >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-HK-00A
Candidate SHA: ${actual}
Executor role: WORKER
Execution substrate: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
OS: $(uname -srm)
Canonical command: scripts/hk00a-observe-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: architecture-consistency=GREEN; causal-negative-controls=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-HK-00A; Docs/engineering/PRODUCT_ARCHITECTURE.md; Docs/engineering/DEPENDENCY_IP_POLICY.md; Docs/engineering/EXTERNAL_HARNESS_ADOPTION_AUDIT.md
EOF
