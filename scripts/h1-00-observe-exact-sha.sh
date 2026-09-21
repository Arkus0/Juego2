#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"
cd "${ROOT}"

actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }
[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean before H1-00 observation" >&2; exit 2; }

test -f src/Arkus.EngineBridge/Arkus.EngineBridge.csproj
test -f src/Arkus.EngineBridge/ProjectionContract.cs

test -f src/Arkus.Harness.Mcp/packages.lock.json
DOTNET_NOLOGO=1 dotnet restore Juego2.sln --locked-mode -m:1 --disable-build-servers
DOTNET_NOLOGO=1 dotnet build Juego2.sln --configuration Release --no-restore -m:1 --disable-build-servers
DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
  --configuration Release --no-build --no-restore -m:1 --disable-build-servers \
  --filter 'FullyQualifiedName~H1EngineBridgeReferenceTests'
DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
  --configuration Release --no-build --no-restore -m:1 --disable-build-servers

if grep -R -E 'UnityEngine|UnityEditor' src/Arkus.EngineBridge --include='*.cs' --include='*.csproj'; then
  echo "H1-00 neutral assembly contains a Unity implementation type/reference" >&2
  exit 2
fi
if grep -E 'ProjectReference|PackageReference' src/Arkus.EngineBridge/Arkus.EngineBridge.csproj; then
  echo "H1-00 neutral assembly unexpectedly depends on another product/package project" >&2
  exit 2
fi

[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean after H1-00 observation" >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-H1-00
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/h1-00-observe-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: locked-restore=GREEN; release-build=GREEN; focused-reference-materializer=GREEN; neutral-dependency-boundary=GREEN; regression=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-H1-00
EOF
