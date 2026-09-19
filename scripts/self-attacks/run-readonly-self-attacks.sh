#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
SHARD="${1:-all}"
DRIVER_ROOT="${ROOT}/artifacts/self-attacks-driver/${SHARD}/candidate"
OBSERVED_ROOT="${ROOT}/artifacts/observed/self-attacks"
LOG_DEST="${ROOT}/artifacts/self-attacks/logs/${SHARD}"

case "${SHARD}" in
  all|core|closure|test-surface|external-authority|reference-authority|terminal-inventory) ;;
  *) echo "SELF-ATTACK DRIVER FAIL: unknown shard '${SHARD}'." >&2; exit 2 ;;
esac

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export MSBUILDDISABLENODEREUSE=1

assert_candidate_clean() {
  git -C "${ROOT}" diff --exit-code -- . >/dev/null || {
    echo "SELF-ATTACK DRIVER FAIL: tracked candidate bytes changed." >&2
    git -C "${ROOT}" diff -- . >&2 || true
    exit 2
  }
  local status
  status="$(git -C "${ROOT}" status --porcelain --untracked-files=all)"
  [[ -z "${status}" ]] || {
    echo "SELF-ATTACK DRIVER FAIL: candidate acquired non-ignored state:" >&2
    printf '%s\n' "${status}" >&2
    exit 2
  }
}

assert_candidate_clean
rm -rf "${ROOT}/artifacts/self-attacks-driver/${SHARD}" "${LOG_DEST}"
mkdir -p "${DRIVER_ROOT}" "${OBSERVED_ROOT}" "${LOG_DEST}"

tar -C "${ROOT}" \
  --exclude=.git --exclude=artifacts --exclude='*/bin' --exclude='*/obj' --exclude='*/TestResults' \
  -cf - . | tar -C "${DRIVER_ROOT}" -xf -

git -C "${DRIVER_ROOT}" init -q
git -C "${DRIVER_ROOT}" config user.name "HK00 Readonly Attack Driver"
git -C "${DRIVER_ROOT}" config user.email "hk00@example.invalid"
git -C "${DRIVER_ROOT}" add -A
git -C "${DRIVER_ROOT}" commit -qm candidate

(
  cd "${DRIVER_ROOT}"
  bash scripts/self-attacks/run-all-self-attacks.sh "${SHARD}"
)

SOURCE_RESULTS="${DRIVER_ROOT}/Docs/evidence/WP-HK-00/self-attacks/results.md"
[[ -f "${SOURCE_RESULTS}" ]] || {
  echo "SELF-ATTACK DRIVER FAIL: attack summary was not produced." >&2
  exit 2
}

if [[ "${SHARD}" == "all" ]]; then
  OBSERVED_RESULTS="${OBSERVED_ROOT}/results.md"
else
  OBSERVED_RESULTS="${OBSERVED_ROOT}/${SHARD}.md"
fi
cp "${SOURCE_RESULTS}" "${OBSERVED_RESULTS}"

if [[ -d "${DRIVER_ROOT}/artifacts/self-attacks/logs" ]]; then
  cp -a "${DRIVER_ROOT}/artifacts/self-attacks/logs/." "${LOG_DEST}/"
fi

assert_candidate_clean
echo "HK00 self-attack shard '${SHARD}' GREEN in disposable candidate copy; original candidate remained read-only."
