#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
CONFIGURATION="${CONFIGURATION:-Release}"
TOOL_PROJECT="${ROOT}/tools/Arkus.HK00.Proof/Arkus.HK00.Proof.csproj"
TOOL_DLL="${ROOT}/tools/Arkus.HK00.Proof/bin/${CONFIGURATION}/net8.0/Arkus.HK00.Proof.dll"
INVENTORY_DIR="${ROOT}/Docs/evidence/WP-HK-00/inventory"
REPORT_PATH="${ROOT}/artifacts/proof/report.json"

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export MSBUILDDISABLENODEREUSE=1

cd "${ROOT}"

step() { printf '\n=== %s ===\n' "$1"; }

step "build fixed-contract proof oracle"
dotnet restore "${TOOL_PROJECT}"
dotnet build "${TOOL_PROJECT}" --configuration "${CONFIGURATION}" --no-restore -v minimal

test -f "${TOOL_DLL}" || { echo "FAIL CLOSED: proof oracle missing at ${TOOL_DLL}" >&2; exit 2; }

step "independent repository/project/source universe"
dotnet "${TOOL_DLL}" --root "${ROOT}" --configuration "${CONFIGURATION}" --phase repository

step "restore exact solution"
dotnet restore Juego2.sln

step "evaluated MSBuild/static contract"
dotnet "${TOOL_DLL}" --root "${ROOT}" --configuration "${CONFIGURATION}" --phase static

step "build complete solution"
dotnet build Juego2.sln --configuration "${CONFIGURATION}" --no-restore -v minimal

step "effective compiler + assembly + PDB proof"
mkdir -p "${ROOT}/artifacts/proof"
dotnet "${TOOL_DLL}" \
  --root "${ROOT}" \
  --configuration "${CONFIGURATION}" \
  --phase all \
  --inventory "${INVENTORY_DIR}" \
  --report "${REPORT_PATH}"

step "tests"
dotnet test Juego2.sln --configuration "${CONFIGURATION}" --no-build --no-restore -v minimal

step "proof green"
echo "WP-HK-00 proof pipeline GREEN"
