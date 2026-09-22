#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"
cd "${ROOT}"

candidate_dirty_status() {
  git status --porcelain --untracked-files=all | \
    grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true
}

actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }
[[ -z "$(candidate_dirty_status)" ]] || { echo "Candidate is not clean before DW-02 observation" >&2; candidate_dirty_status >&2; exit 2; }

test -f src/Arkus.DesignWorld/Arkus.DesignWorld.csproj
test -f src/Arkus.DesignWorld/DesignWorldContracts.cs
test -f src/Arkus.DesignWorld/DesignWorldProjection.cs
test -f src/Arkus.DesignWorld/CityDesignWorldProvider.cs
test -f src/Arkus.DesignWorld/CityProductionQueries.cs
test -f tests/Arkus.Harness.Tests/Dw02CityProductionQueryTests.cs
test -f Docs/evidence/WP-DW-02/WORKER_PLAN.md
test -f Docs/evidence/WP-DW-02/QUERY_SUITE_V1.md

test -f src/Arkus.Harness.Mcp/packages.lock.json
DOTNET_NOLOGO=1 dotnet restore Juego2.sln --locked-mode -m:1 --disable-build-servers
DOTNET_NOLOGO=1 dotnet build Juego2.sln --configuration Release --no-restore -m:1 --disable-build-servers
DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
  --configuration Release --no-build --no-restore -m:1 --disable-build-servers \
  --filter 'FullyQualifiedName~Dw02'

# DW-02 remains a downstream CITY consumer. No production-query vocabulary or
# rule may leak back into accepted H0 assemblies.
if grep -R -E 'Arkus\.DesignWorld|CityProduction(Query|Projection)|CityContentShape|city\.query_|dw02-city' \
  src/Arkus.Game.Core src/Arkus.Game.World src/Arkus.Game.Authoring \
  src/Arkus.Game.Validation src/Arkus.Harness.Protocol src/Arkus.Harness.Runtime \
  --include='*.cs' --include='*.csproj'; then
  echo "DW-02 introduced a reverse H0/domain dependency on the Design World CITY query consumer" >&2
  exit 2
fi

if grep -R -E '<ProjectReference' src/Arkus.DesignWorld/Arkus.DesignWorld.csproj | \
  grep -v -F '../Arkus.Game.World/Arkus.Game.World.csproj'; then
  echo "DW-02 added an unexpected project dependency to Arkus.DesignWorld" >&2
  exit 2
fi

grep -Fq '../Arkus.Game.World/Arkus.Game.World.csproj' src/Arkus.DesignWorld/Arkus.DesignWorld.csproj

DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
  --configuration Release --no-build --no-restore -m:1 --disable-build-servers

[[ -z "$(candidate_dirty_status)" ]] || { echo "Candidate is not clean after DW-02 observation" >&2; candidate_dirty_status >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-DW-02
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/dw02-observe-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: locked-restore=GREEN; release-build=GREEN; focused-dw02=GREEN; reverse-h0-dependency=GREEN; h0-domain-neutrality=GREEN; designworld-dependency-boundary=GREEN; regression=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-DW-02
EOF
