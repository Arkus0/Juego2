#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"
cd "${ROOT}"

actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }
[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean before HK GATE observation" >&2; exit 2; }

EVIDENCE_DIR="artifacts/observed/WP-HK-GATE"
mkdir -p "${EVIDENCE_DIR}"
export ARKUS_HK_GATE_TRANSCRIPT_OUTPUT="${ROOT}/${EVIDENCE_DIR}/reference-scenario.json"
export ARKUS_HK08B_BENCHMARK_OUTPUT="${ROOT}/${EVIDENCE_DIR}/hk08b-interaction-benchmark.json"

test -f tests/Arkus.Harness.Tests/HkGateReadinessTests.cs
test -f scripts/hkgate-negative-conformance.sh
test -f scripts/hkgate-proof-infrastructure-check.sh
test -f Docs/evidence/WP-HK-GATE/WORKER_PLAN.md

# GATE-owned proof infrastructure is checked independently of the declarative
# step list. In particular, stage 14 must be wired as a real unfiltered test
# execution in this runner; G1 mutates that execution rather than its label.
bash scripts/hkgate-proof-infrastructure-check.sh

DOTNET_NOLOGO=1 dotnet restore Juego2.sln --locked-mode -m:1 --disable-build-servers
DOTNET_NOLOGO=1 dotnet build Juego2.sln --configuration Release --no-restore -m:1 --disable-build-servers

DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
  --configuration Release --no-build --no-restore -m:1 --disable-build-servers \
  --logger 'console;verbosity=detailed' \
  --filter 'FullyQualifiedName~HkGateReadinessTests'

# Cross-transport proof consumes the accepted transport oracles, including HK08A batch/compact/v2 paging,
# HK08B stale recovery and HK09B resource-envelope equivalence.
DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
  --configuration Release --no-build --no-restore -m:1 --disable-build-servers \
  --logger 'console;verbosity=detailed' \
  --filter 'FullyQualifiedName~Hk07BMcpConformanceTests|FullyQualifiedName~Hk08ATransportConformanceTests|FullyQualifiedName~Hk08BTransportConformanceTests|FullyQualifiedName~Hk09BTransportConformanceTests'

# Accepted interaction benchmark remains the budget oracle rather than inflating the whole gate transcript into a latency SLO.
DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
  --configuration Release --no-build --no-restore -m:1 --disable-build-servers \
  --logger 'console;verbosity=detailed' \
  --filter 'FullyQualifiedName~Hk08BInteractionBenchmarkTests.RepresentativePublicClientFlowMeasuresInteractionCostWithoutWholeWorldConflictReload'

# Host authority and resource/persistence boundaries are transport-neutral accepted obligations that GATE composes.
DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
  --configuration Release --no-build --no-restore -m:1 --disable-build-servers \
  --logger 'console;verbosity=detailed' \
  --filter 'FullyQualifiedName~Hk09A|FullyQualifiedName~Hk09B'

# Re-run the accepted bounded H0 endurance/compatibility surface and persist its console evidence separately.
set +e
DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
  --configuration Release --no-build --no-restore -m:1 --disable-build-servers \
  --logger 'console;verbosity=detailed' \
  --filter 'FullyQualifiedName~Hk10EnduranceCompatibilityTests' \
  2>&1 | tee "${EVIDENCE_DIR}/hk10-endurance.log"
endurance_status=${PIPESTATUS[0]}
set -e
[[ ${endurance_status} -eq 0 ]] || exit "${endurance_status}"

bash scripts/hkgate-negative-conformance.sh
bash scripts/hk10-negative-conformance.sh

# Mandatory headless full validation surface.
unset ARKUS_HK_GATE_TRANSCRIPT_OUTPUT
unset ARKUS_HK08B_BENCHMARK_OUTPUT
DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
  --configuration Release --no-build --no-restore -m:1 --disable-build-servers

[[ -f "${EVIDENCE_DIR}/reference-scenario.json" ]]
[[ -f "${EVIDENCE_DIR}/hk08b-interaction-benchmark.json" ]]
grep -Fq 'HK10_ENDURANCE transactions=512' "${EVIDENCE_DIR}/hk10-endurance.log"

[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean after HK GATE observation" >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-HK-GATE
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/hkgate-observe-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: proof-infrastructure-wiring=GREEN; locked-restore=GREEN; release-build=GREEN; public-reference-scenario=GREEN; reference-mcp-conformance=GREEN; hk08b-interaction-budget=GREEN; hk09a-authority-boundary=GREEN; hk09b-resource-persistence=GREEN; hk10-endurance=GREEN; gate-negative-control=GREEN; inherited-causal-negative-controls=GREEN; headless-regression=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-HK-GATE; artifacts/observed/WP-HK-GATE
EOF
