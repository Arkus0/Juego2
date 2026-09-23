#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"
BASELINE_SHA="18d4aa33be526c65a53796aa305a56fe8df15d12"
cd "${ROOT}"

candidate_dirty_status() {
  git status --porcelain --untracked-files=all | \
    grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true
}

actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }
git cat-file -e "${BASELINE_SHA}^{commit}" || { echo "DW-05 baseline commit is unavailable" >&2; exit 2; }
git merge-base --is-ancestor "${BASELINE_SHA}" "${actual}" || { echo "DW-05 candidate does not descend from accepted baseline" >&2; exit 2; }
[[ -z "$(candidate_dirty_status)" ]] || { echo "Candidate is not clean before DW-05 observation" >&2; candidate_dirty_status >&2; exit 2; }

for path in \
  Docs/evidence/WP-DW-05/PREDECESSOR_CONTRACT_CHECK.md \
  Docs/evidence/WP-DW-05/WORKER_PLAN.md \
  Docs/evidence/WP-DW-05/PREDECESSOR_RESIDUAL_INVENTORY.json \
  Docs/evidence/WP-DW-05/LIMITATIONS_MANIFEST.json \
  Docs/evidence/WP-DW-05/H2_BOUNDARY_INPUT_V1.json \
  Docs/evidence/WP-DW-05/fixtures/neutral-observatory.txt \
  tests/Arkus.Harness.Tests/Dw05GenericBoundaryStressTests.cs \
  tests/Arkus.Harness.Tests/Dw05TransitiveKernelLeakageTests.cs \
  tests/Arkus.Harness.Tests/Dw05NeutralFixtureAuthorityTests.cs \
  tests/Arkus.Harness.Tests/Dw05ResidualInventoryCompletenessTests.cs; do
  test -f "${path}"
done

# DW-05 may add probe/test/evidence/validation glue, but it may not silently
# repair the accepted generic DW seam or H0/kernel semantics to make the probe fit.
git diff --exit-code "${BASELINE_SHA}" "${actual}" -- \
  src/Arkus.DesignWorld/Arkus.DesignWorld.csproj \
  src/Arkus.DesignWorld/DesignWorldContracts.cs \
  src/Arkus.DesignWorld/DesignWorldProjection.cs \
  src/Arkus.Game.World \
  src/Arkus.Game.Core

# Accepted predecessor/source semantics are inputs to this stress test, not a
# surface the Worker may rewrite to manufacture closure.
git diff --exit-code "${BASELINE_SHA}" "${actual}" -- \
  Docs/workpacks/DW/WP-DW-00.md \
  Docs/workpacks/DW/WP-DW-01.md \
  Docs/workpacks/DW/WP-DW-02.md \
  Docs/workpacks/DW/WP-DW-03.md \
  Docs/workpacks/DW/WP-DW-04.md \
  Docs/engineering/DW_DESIGN_WORLD_ARCHITECTURE.md \
  Docs/production/CITY_LOCATION_PROGRAMME.md \
  Docs/production/CITY_MOBILITY_TOPOLOGY.md \
  Docs/research/living-world/results/PA-01.md \
  Docs/research/living-world/results/PA-02.md \
  Docs/research/living-world/results/PA-03.md \
  Docs/research/living-world/results/PA-04.md \
  Docs/research/living-world/results/PA-05.md

test -f src/Arkus.Harness.Mcp/packages.lock.json
DOTNET_NOLOGO=1 dotnet restore Juego2.sln --locked-mode -m:1 --disable-build-servers
DOTNET_NOLOGO=1 dotnet build Juego2.sln --configuration Release --no-restore -m:1 --disable-build-servers
DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
  --configuration Release --no-build --no-restore -m:1 --disable-build-servers \
  --filter 'FullyQualifiedName~Dw05'
DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
  --configuration Release --no-build --no-restore -m:1 --disable-build-servers

[[ -z "$(candidate_dirty_status)" ]] || { echo "Candidate is not clean after DW-05 observation" >&2; candidate_dirty_status >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-DW-05
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/dw05-observe-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: accepted-generic-dw-h0-seam-immutability=GREEN; transitive-h0-kernel-immutability=GREEN; real-neutral-authority-fixture=GREEN; residual-inventory-completeness-proof-present=GREEN; predecessor-source-immutability=GREEN; locked-restore=GREEN; release-build=GREEN; focused-dw05=GREEN; regression=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-DW-05
EOF
