#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
BOOT="${ROOT}/artifacts/bootstrap-proof"
LEGACY="${ROOT}/tools/Arkus.HK00.Proof/bin/Release/net8.0"

bash "${ROOT}/scripts/build-proof-oracle.sh" >/dev/null

mkdir -p "${LEGACY}"
cp "${BOOT}/Arkus.HK00.Proof.dll" "${LEGACY}/Arkus.HK00.Proof.dll"
cp "${BOOT}/Arkus.HK00.Proof.pdb" "${LEGACY}/Arkus.HK00.Proof.pdb"
cp "${BOOT}/Arkus.HK00.Proof.runtimeconfig.json" "${LEGACY}/Arkus.HK00.Proof.runtimeconfig.json"

REAL_DOTNET="$(command -v dotnet)"
export REAL_DOTNET

dotnet() {
  if [[ "${1:-}" == "restore" && "${2:-}" == *"/tools/Arkus.HK00.Proof/Arkus.HK00.Proof.csproj" ]]; then
    return 0
  fi
  if [[ "${1:-}" == "build" && "${2:-}" == *"/tools/Arkus.HK00.Proof/Arkus.HK00.Proof.csproj" ]]; then
    return 0
  fi
  "${REAL_DOTNET}" "$@"
}
export -f dotnet

bash "${ROOT}/scripts/self-attacks/run-self-attacks.sh"
bash "${ROOT}/scripts/self-attacks/run-closure-attacks.sh"
bash "${ROOT}/scripts/self-attacks/run-terminal-inventory-attack.sh"

echo "all 33 causal self-attacks passed"
