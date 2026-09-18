#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
CONFIGURATION="${CONFIGURATION:-Release}"
WORK="${ROOT}/artifacts/self-attacks"
PRISTINE="${WORK}/pristine"
SANDBOX="${WORK}/terminal-inventory-sandbox"
LOGDIR="${WORK}/logs"
RESULTS="${ROOT}/Docs/evidence/WP-HK-00/self-attacks/results.md"
TOOL_DLL="${ROOT}/tools/Arkus.HK00.Proof/bin/${CONFIGURATION}/net8.0/Arkus.HK00.Proof.dll"

fail() { echo "TERMINAL-INVENTORY ATTACK FAILURE: $*" >&2; exit 1; }

fresh() {
  rm -rf "${SANDBOX}"
  mkdir -p "${SANDBOX}"
  tar -C "${PRISTINE}" --exclude=.git --exclude=artifacts --exclude='*/bin' --exclude='*/obj' --exclude='*/TestResults' -cf - . | tar -C "${SANDBOX}" -xf -
  git -C "${SANDBOX}" init -q
  git -C "${SANDBOX}" config user.name "HK00 Terminal Inventory Attack"
  git -C "${SANDBOX}" config user.email "hk00@example.invalid"
  git -C "${SANDBOX}" add -A
  git -C "${SANDBOX}" commit -qm pristine
}

fresh
mkdir -p "${SANDBOX}/zzzzzz"
printf '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net8.0</TargetFramework></PropertyGroup></Project>\n' >"${SANDBOX}/zzzzzz/Terminal.csproj"
git -C "${SANDBOX}" add zzzzzz/Terminal.csproj

set +e
(cd "${SANDBOX}" && dotnet "${TOOL_DLL}" --root "${SANDBOX}" --configuration "${CONFIGURATION}" --phase repository) >"${LOGDIR}/terminal-nul-inventory-entry-red.log" 2>&1
code=$?
set -e
[[ ${code} -eq 1 ]] || { cat "${LOGDIR}/terminal-nul-inventory-entry-red.log" >&2; fail "expected proof exit 1, got ${code}"; }
grep -Fq HK00-PROJECT-UNEXPECTED "${LOGDIR}/terminal-nul-inventory-entry-red.log" || { cat "${LOGDIR}/terminal-nul-inventory-entry-red.log" >&2; fail "terminal project escaped project universe"; }

fresh
(cd "${SANDBOX}" && dotnet "${TOOL_DLL}" --root "${SANDBOX}" --configuration "${CONFIGURATION}" --phase repository) >"${LOGDIR}/terminal-nul-inventory-entry-green.log" 2>&1 || { cat "${LOGDIR}/terminal-nul-inventory-entry-green.log" >&2; fail "pristine reconstruction did not return GREEN"; }

printf '| `%s` | `%s` | RED on terminal NUL-delimited entry → pristine reconstruction → GREEN |\n' \
  'terminal-nul-inventory-entry' 'HK00-PROJECT-UNEXPECTED' >>"${RESULTS}"

echo "PASS terminal-nul-inventory-entry"
