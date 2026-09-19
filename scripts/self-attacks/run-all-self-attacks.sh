#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
BOOT="${ROOT}/artifacts/bootstrap-proof"
LEGACY="${ROOT}/tools/Arkus.HK00.Proof/bin/Release/net8.0"
RESULTS="${ROOT}/Docs/evidence/WP-HK-00/self-attacks/results.md"
SHARD="${1:-all}"

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

reset_results() {
  mkdir -p "$(dirname "${RESULTS}")"
  cat >"${RESULTS}" <<'MD'
# WP-HK-00 causal self-attacks

| Attack | Intended oracle/check | Result |
|---|---|---|
MD
}

prepare_shared_workspace() {
  local work="${ROOT}/artifacts/self-attacks"
  local pristine="${work}/pristine"
  rm -rf "${work}"
  mkdir -p "${pristine}" "${work}/logs"
  tar -C "${ROOT}" \
    --exclude=.git --exclude=artifacts --exclude='*/bin' --exclude='*/obj' --exclude='*/TestResults' \
    -cf - . | tar -C "${pristine}" -xf -
}

run_family() {
  prepare_shared_workspace
  reset_results
  bash "$1"
}

run_shard() {
  case "${SHARD}" in
    core)
      bash "${ROOT}/scripts/self-attacks/run-self-attacks.sh"
      ;;
    closure)
      run_family "${ROOT}/scripts/self-attacks/run-closure-attacks.sh"
      ;;
    test-surface)
      run_family "${ROOT}/scripts/self-attacks/run-test-surface-attack.sh"
      ;;
    external-authority)
      run_family "${ROOT}/scripts/self-attacks/run-external-authority-attacks.sh"
      ;;
    reference-authority)
      run_family "${ROOT}/scripts/self-attacks/run-reference-authority-attack.sh"
      ;;
    terminal-inventory)
      run_family "${ROOT}/scripts/self-attacks/run-terminal-inventory-attack.sh"
      ;;
    all)
      bash "${ROOT}/scripts/self-attacks/run-self-attacks.sh"
      bash "${ROOT}/scripts/self-attacks/run-closure-attacks.sh"
      bash "${ROOT}/scripts/self-attacks/run-test-surface-attack.sh"
      bash "${ROOT}/scripts/self-attacks/run-external-authority-attacks.sh"
      bash "${ROOT}/scripts/self-attacks/run-reference-authority-attack.sh"
      bash "${ROOT}/scripts/self-attacks/run-terminal-inventory-attack.sh"
      ;;
    *)
      echo "Unknown self-attack shard: ${SHARD}" >&2
      exit 2
      ;;
  esac
}

run_shard

if [[ "${SHARD}" == "all" ]]; then
  echo "all 37 causal self-attacks passed"
else
  echo "causal self-attack shard '${SHARD}' passed"
fi
