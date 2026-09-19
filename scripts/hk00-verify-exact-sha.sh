#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"

cd "${ROOT}"
actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }

git diff --exit-code -- . >/dev/null
[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean before validation" >&2; exit 2; }

[[ "$(dotnet --version)" == "8.0.425" ]] || { echo "Expected .NET SDK 8.0.425" >&2; exit 2; }
dotnet --list-runtimes | grep -Fq 'Microsoft.NETCore.App 8.0.31 ' || { echo "Expected Microsoft.NETCore.App 8.0.31" >&2; exit 2; }

bash scripts/proof.sh

test -d artifacts/observed/inventory
diff -ru Docs/evidence/WP-HK-00/inventory artifacts/observed/inventory

bash scripts/self-attacks/run-readonly-self-attacks.sh

test -f artifacts/observed/self-attacks/results.md
diff -u Docs/evidence/WP-HK-00/self-attacks/results.md artifacts/observed/self-attacks/results.md

git diff --exit-code -- . >/dev/null
[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean after validation" >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-HK-00
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution substrate: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
OS: $(uname -srm)
Toolchain: dotnet $(dotnet --version); Microsoft.NETCore.App 8.0.31
Canonical command: scripts/hk00-verify-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: foundational-proof=GREEN; committed-inventory-drift=GREEN; causal-negative-controls=GREEN; committed-attack-summary-drift=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-HK-00; artifacts/observed
EOF
