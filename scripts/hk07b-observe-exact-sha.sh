#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"
cd "${ROOT}"

actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }
[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean before HK07B observation" >&2; exit 2; }

lock="src/Arkus.Harness.Mcp/packages.lock.json"
if [[ ! -f "${lock}" ]]; then
  echo "HK07B_LOCK_BOOTSTRAP_REQUIRED"
  DOTNET_NOLOGO=1 dotnet restore src/Arkus.Harness.Mcp/Arkus.Harness.Mcp.csproj -p:RestorePackagesWithLockFile=true -m:1 --disable-build-servers
  echo "HK07B_LOCKFILE_BEGIN"
  cat "${lock}"
  echo "HK07B_LOCKFILE_END"
  echo "Generated MCP lockfile must be committed before canonical observation can pass." >&2
  exit 2
fi

DOTNET_NOLOGO=1 dotnet restore Juego2.sln --locked-mode -m:1 --disable-build-servers
DOTNET_NOLOGO=1 dotnet restore src/Arkus.Harness.Mcp/Arkus.Harness.Mcp.csproj --locked-mode -m:1 --disable-build-servers
DOTNET_NOLOGO=1 dotnet build Juego2.sln --configuration Release --no-restore -m:1 --disable-build-servers
DOTNET_NOLOGO=1 dotnet build src/Arkus.Harness.Mcp/Arkus.Harness.Mcp.csproj --configuration Release --no-restore -m:1 --disable-build-servers
DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
  --configuration Release --no-build --no-restore -m:1 --disable-build-servers \
  --filter 'FullyQualifiedName~Hk07B'
DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
  --configuration Release --no-build --no-restore -m:1 --disable-build-servers

[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean after HK07B observation" >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-HK-07B
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/hk07b-observe-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: locked-restore=GREEN; release-build=GREEN; mcp-sdk-projection=GREEN; canonical-discovery=GREEN; cross-transport=GREEN; synthetic-provider=GREEN; cancellation-timeout=GREEN; dependency-boundary=GREEN; causal-negative-controls=GREEN; regression=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-HK-07B
EOF
