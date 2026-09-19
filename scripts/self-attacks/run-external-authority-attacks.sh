#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
CONFIGURATION="${CONFIGURATION:-Release}"
WORK="${ROOT}/artifacts/self-attacks"
PRISTINE="${WORK}/external-authority-pristine"
SANDBOX="${WORK}/external-authority-sandbox"
LOGDIR="${WORK}/logs"
RESULTS="${ROOT}/Docs/evidence/WP-HK-00/self-attacks/results.md"
TOOL_DLL="${ROOT}/artifacts/bootstrap-proof/Arkus.HK00.Proof.dll"

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export MSBUILDDISABLENODEREUSE=1

fail() { echo "EXTERNAL-AUTHORITY ATTACK FAILURE: $*" >&2; exit 1; }

copy_clean_tree() {
  local from="$1" to="$2"
  rm -rf "${to}"
  mkdir -p "${to}"
  tar -C "${from}" \
    --exclude=.git --exclude=artifacts --exclude='*/bin' --exclude='*/obj' --exclude='*/TestResults' \
    -cf - . | tar -C "${to}" -xf -
}

fresh() {
  copy_clean_tree "${PRISTINE}" "${SANDBOX}"
  git -C "${SANDBOX}" init -q
  git -C "${SANDBOX}" config user.name "HK00 External Authority Attack"
  git -C "${SANDBOX}" config user.email "hk00@example.invalid"
  git -C "${SANDBOX}" add -A
  git -C "${SANDBOX}" commit -qm pristine
  rm -rf "${SANDBOX}/artifacts/nuget-packages"
  mkdir -p "${SANDBOX}/artifacts/nuget-packages"
}

locked_restore() {
  (cd "${SANDBOX}" && NUGET_PACKAGES="${SANDBOX}/artifacts/nuget-packages" \
    dotnet msbuild Juego2.sln -nologo -noAutoResponse -t:Restore \
      -p:Configuration="${CONFIGURATION}" -p:RestoreLockedMode=true)
}

run_static() {
  local log="$1"
  (cd "${SANDBOX}" && NUGET_PACKAGES="${SANDBOX}/artifacts/nuget-packages" \
    dotnet "${TOOL_DLL}" --root "${SANDBOX}" --configuration "${CONFIGURATION}" --phase static) >"${log}" 2>&1
}

record() {
  printf '| `%s` | `%s` | RED on injection → pristine reconstruction → GREEN |\n' "$1" "$2" >>"${RESULTS}"
  echo "PASS $1"
}

rm -rf "${PRISTINE}" "${SANDBOX}" "${WORK}/external-authority"
copy_clean_tree "${ROOT}" "${PRISTINE}"
mkdir -p "${LOGDIR}"

# Class 1: the committed transitive package graph cannot drift while locked restore stays green.
fresh
python3 - "${SANDBOX}/tests/Arkus.Harness.Tests/packages.lock.json" <<'PY'
import json,sys
p=sys.argv[1]
d=json.load(open(p))
node=d['dependencies']['net8.0']['Newtonsoft.Json']
node['resolved']='13.0.2'
open(p,'w').write(json.dumps(d,indent=2)+'\n')
PY
set +e
locked_restore >"${LOGDIR}/dependency-lock-drift-red.log" 2>&1
code=$?
set -e
[[ ${code} -ne 0 ]] || { cat "${LOGDIR}/dependency-lock-drift-red.log" >&2; fail "mutated package lock restored successfully in locked mode"; }

fresh
locked_restore >"${LOGDIR}/dependency-lock-drift-green.log" 2>&1 || {
  cat "${LOGDIR}/dependency-lock-drift-green.log" >&2
  fail "pristine locked restore did not return GREEN"
}
record dependency-lock-drift NUGET_LOCKED_RESTORE

# Class 2: generated MSBuild glue cannot smuggle arbitrary external build logic.
fresh
locked_restore >/dev/null
ROGUE="${WORK}/external-authority/Rogue.targets"
mkdir -p "$(dirname "${ROGUE}")"
printf '<Project><PropertyGroup><RogueImported>true</RogueImported></PropertyGroup></Project>\n' >"${ROGUE}"
GEN="${SANDBOX}/tests/Arkus.Harness.Tests/obj/Arkus.Harness.Tests.csproj.nuget.g.props"
[[ -f "${GEN}" ]] || fail "NuGet generated props not found after restore"
python3 - "${GEN}" "${ROGUE}" <<'PY'
import sys
p,rogue=sys.argv[1],sys.argv[2]
t=open(p).read()
assert '</Project>' in t
imp='  <Import Project="'+rogue.replace('&','&amp;').replace('"','&quot;')+'" />\n'
open(p,'w').write(t.replace('</Project>',imp+'</Project>',1))
PY
set +e
run_static "${LOGDIR}/external-build-import-red.log"
code=$?
set -e
[[ ${code} -eq 1 ]] || { cat "${LOGDIR}/external-build-import-red.log" >&2; fail "expected proof exit 1, got ${code}"; }
grep -Fq HK00-MSBUILD-IMPORT-EXTERNAL-UNTRUSTED "${LOGDIR}/external-build-import-red.log" || {
  cat "${LOGDIR}/external-build-import-red.log" >&2
  fail "arbitrary external MSBuild import escaped closure oracle"
}

fresh
locked_restore >/dev/null
run_static "${LOGDIR}/external-build-import-green.log" || {
  cat "${LOGDIR}/external-build-import-green.log" >&2
  fail "pristine static proof did not return GREEN"
}
record external-build-import HK00-MSBUILD-IMPORT-EXTERNAL-UNTRUSTED

echo "PASS external authority closure attacks"
