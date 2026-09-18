#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
CONFIGURATION="${CONFIGURATION:-Release}"
TOOL_DLL="${ROOT}/artifacts/bootstrap-proof/Arkus.HK00.Proof.dll"
INVENTORY_DIR="${ROOT}/Docs/evidence/WP-HK-00/inventory"
REPORT_PATH="${ROOT}/artifacts/proof/report.json"

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export MSBUILDDISABLENODEREUSE=1

cd "${ROOT}"

step() { printf '\n=== %s ===\n' "$1"; }

step "bootstrap fixed-contract proof oracle directly from tracked sources"
bash scripts/build-proof-oracle.sh
test -f "${TOOL_DLL}" || { echo "FAIL CLOSED: bootstrapped proof oracle missing at ${TOOL_DLL}" >&2; exit 2; }

step "independent repository/project/source/build-surface universe"
dotnet "${TOOL_DLL}" --root "${ROOT}" --configuration "${CONFIGURATION}" --phase repository

step "restore exact solution with automatic response files disabled"
dotnet msbuild Juego2.sln \
  -nologo \
  -noAutoResponse \
  -t:Restore \
  -p:Configuration="${CONFIGURATION}"

step "evaluated MSBuild/static contract"
dotnet "${TOOL_DLL}" --root "${ROOT}" --configuration "${CONFIGURATION}" --phase static

step "build complete solution with automatic response files disabled"
dotnet msbuild Juego2.sln \
  -nologo \
  -noAutoResponse \
  -t:Rebuild \
  -p:Configuration="${CONFIGURATION}"

step "effective compiler + assembly + PDB proof"
mkdir -p "${ROOT}/artifacts/proof"
dotnet "${TOOL_DLL}" \
  --root "${ROOT}" \
  --configuration "${CONFIGURATION}" \
  --phase all \
  --inventory "${INVENTORY_DIR}" \
  --report "${REPORT_PATH}"

step "tests with automatic response files disabled and no rebuild"
dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
  --configuration "${CONFIGURATION}" \
  --no-build \
  --no-restore \
  -noAutoResponse \
  -v minimal

step "post-test candidate/output integrity"
dotnet "${TOOL_DLL}" --root "${ROOT}" --configuration "${CONFIGURATION}" --phase repository
dotnet "${TOOL_DLL}" --root "${ROOT}" --configuration "${CONFIGURATION}" --phase output

step "proof green"
echo "WP-HK-00 proof pipeline GREEN"
