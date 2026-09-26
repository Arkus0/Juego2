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
[[ -z "$(git status --porcelain --untracked-files=all | grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true)" ]] || { echo "Candidate is not clean before HK05 observation" >&2; exit 2; }

DOTNET_NOLOGO=1 dotnet restore Juego2.sln --locked-mode
DOTNET_NOLOGO=1 dotnet build Juego2.sln --configuration Release --no-restore
DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
  --configuration Release --no-build --no-restore \
  --filter 'FullyQualifiedName~Hk05'
DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
  --configuration Release --no-build --no-restore

[[ -z "$(git status --porcelain --untracked-files=all | grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true)" ]] || { echo "Candidate is not clean after HK05 observation" >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-HK-05
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/hk05-observe-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: locked-restore=GREEN; release-build=GREEN; validation-diagnostics-contract=GREEN; causal-negative-controls=GREEN; regression=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-HK-05
EOF
