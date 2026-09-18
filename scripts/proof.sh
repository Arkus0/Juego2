#!/usr/bin/env bash
#
# Canonical WP-HK-00 proof pipeline.
#
# The order matters. The proof tool is built and the preflight runs before the
# solution build, so a deleted project, test project or proof tool is reported as
# a failure instead of quietly shrinking what gets verified. Static checks come
# from evaluated MSBuild facts; effective checks read the produced assemblies and
# portable PDBs after the build.
#
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
CONFIGURATION="${CONFIGURATION:-Release}"
PROOF_TOOL_PROJECT="tools/Arkus.Kernel.Proof.Tool/Arkus.Kernel.Proof.Tool.csproj"
PROOF_TOOL_DLL="${ROOT}/artifacts/bin/Arkus.Kernel.Proof.Tool/${CONFIGURATION,,}/Arkus.Kernel.Proof.Tool.dll"
INVENTORY_DIR="${ROOT}/Docs/evidence/WP-HK-00/inventory"
REPORT_PATH="${ROOT}/artifacts/proof/report.json"

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
export MSBUILDDISABLENODEREUSE=1

cd "${ROOT}"

step() {
  printf '\n=== %s ===\n' "$1"
}

step "toolchain"
dotnet --version

step "restore (locked mode)"
dotnet restore Juego2.sln --locked-mode

step "build proof tool"
dotnet build "${PROOF_TOOL_PROJECT}" --no-restore -c "${CONFIGURATION}" -v minimal

if [[ ! -f "${PROOF_TOOL_DLL}" ]]; then
  echo "FAIL CLOSED: proof tool was not produced at ${PROOF_TOOL_DLL}" >&2
  exit 2
fi

step "preflight"
dotnet "${PROOF_TOOL_DLL}" --repository-root "${ROOT}" --configuration "${CONFIGURATION}" --phase preflight

step "build solution"
dotnet build Juego2.sln --no-restore -c "${CONFIGURATION}" -v minimal

step "proof (static + effective) and inventory"
dotnet "${PROOF_TOOL_DLL}" \
  --repository-root "${ROOT}" \
  --configuration "${CONFIGURATION}" \
  --phase all \
  --report "${REPORT_PATH}" \
  --inventory "${INVENTORY_DIR}"

step "headless host smoke test"
HOST_DLL="${ROOT}/artifacts/bin/Arkus.Harness.Cli/${CONFIGURATION,,}/Arkus.Harness.Cli.dll"
if [[ ! -f "${HOST_DLL}" ]]; then
  echo "FAIL CLOSED: headless host was not produced at ${HOST_DLL}" >&2
  exit 2
fi

EXPECTED_MODULES=$'Arkus.Game.Authoring\nArkus.Game.Core\nArkus.Game.Validation\nArkus.Game.World\nArkus.Harness.Protocol\nArkus.Harness.Runtime'
ACTUAL_MODULES="$(dotnet "${HOST_DLL}")"
if [[ "${ACTUAL_MODULES}" != "${EXPECTED_MODULES}" ]]; then
  echo "FAIL: headless host printed an unexpected kernel composition" >&2
  printf 'expected:\n%s\nactual:\n%s\n' "${EXPECTED_MODULES}" "${ACTUAL_MODULES}" >&2
  exit 1
fi
echo "host composed: ${ACTUAL_MODULES//$'\n'/, }"

step "tests"
dotnet test Juego2.sln --no-build --no-restore -c "${CONFIGURATION}" -v minimal

step "done"
echo "proof pipeline completed for configuration ${CONFIGURATION}"
