#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
CONFIGURATION="${CONFIGURATION:-Release}"
TOOL_DLL="${ROOT}/artifacts/bootstrap-proof/Arkus.HK00.Proof.dll"
OBSERVED_ROOT="${ROOT}/artifacts/observed"
INVENTORY_DIR="${OBSERVED_ROOT}/inventory"
REPORT_PATH="${OBSERVED_ROOT}/proof/report.json"

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export MSBUILDDISABLENODEREUSE=1

cd "${ROOT}"

step() { printf '\n=== %s ===\n' "$1"; }

assert_candidate_clean() {
  git diff --exit-code -- . >/dev/null || {
    echo "FAIL CLOSED: tracked candidate bytes changed during proof." >&2
    git diff -- . >&2 || true
    exit 2
  }
  local status
  status="$(git status --porcelain --untracked-files=all)"
  [[ -z "${status}" ]] || {
    echo "FAIL CLOSED: candidate checkout acquired non-ignored untracked/staged state during proof:" >&2
    printf '%s\n' "${status}" >&2
    exit 2
  }
}

step "assert immutable candidate input"
assert_candidate_clean
rm -rf "${OBSERVED_ROOT}/inventory" "${OBSERVED_ROOT}/proof"
mkdir -p "${INVENTORY_DIR}" "$(dirname "${REPORT_PATH}")"

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

step "assert candidate remained immutable"
assert_candidate_clean

step "proof green"
echo "WP-HK-00 proof pipeline GREEN (candidate read-only; evidence emitted under artifacts/observed)"
