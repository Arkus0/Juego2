#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
CONFIGURATION="${CONFIGURATION:-Release}"
WORK="${ROOT}/artifacts/self-attacks"
PRISTINE="${WORK}/reference-pristine"
SANDBOX="${WORK}/reference-sandbox"
LOGDIR="${WORK}/logs"
RESULTS="${ROOT}/Docs/evidence/WP-HK-00/self-attacks/results.md"
TOOL_DLL="${ROOT}/artifacts/bootstrap-proof/Arkus.HK00.Proof.dll"

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export MSBUILDDISABLENODEREUSE=1

fail() { echo "REFERENCE AUTHORITY ATTACK FAILURE: $*" >&2; exit 1; }

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
  git -C "${SANDBOX}" config user.name "HK00 Reference Authority Attack"
  git -C "${SANDBOX}" config user.email "hk00@example.invalid"
  git -C "${SANDBOX}" add -A
  git -C "${SANDBOX}" commit -qm pristine
}

insert_xml() {
  local file="$1" snippet="$2"
  python3 - "$file" "$snippet" <<'PY'
import sys
p,s=sys.argv[1],sys.argv[2]
t=open(p).read()
assert '</Project>' in t
open(p,'w').write(t.replace('</Project>',s+'\n</Project>',1))
PY
}

prepare_built() {
  (cd "${SANDBOX}" && dotnet msbuild Juego2.sln -nologo -noAutoResponse -t:Restore -p:Configuration="${CONFIGURATION}" >/dev/null)
  (cd "${SANDBOX}" && dotnet msbuild Juego2.sln -nologo -noAutoResponse -t:Rebuild -p:Configuration="${CONFIGURATION}" >/dev/null)
}

run_effective() {
  local log="$1"
  (cd "${SANDBOX}" && dotnet "${TOOL_DLL}" --root "${SANDBOX}" --configuration "${CONFIGURATION}" --phase effective) >"${log}" 2>&1
}

rm -rf "${PRISTINE}" "${SANDBOX}"
copy_clean_tree "${ROOT}" "${PRISTINE}"
mkdir -p "${LOGDIR}"

fresh
prepare_built
cp "${SANDBOX}/src/Arkus.Game.Core/bin/Release/netstandard2.1/Arkus.Game.Core.dll" "${SANDBOX}/RogueReference.dll"
insert_xml "${SANDBOX}/tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj" \
  '  <ItemGroup><Reference Include="RogueReference"><HintPath>../../RogueReference.dll</HintPath><Private>false</Private></Reference></ItemGroup>'
prepare_built
set +e
run_effective "${LOGDIR}/rogue-compiler-reference-red.log"
code=$?
set -e
[[ ${code} -eq 1 ]] || { cat "${LOGDIR}/rogue-compiler-reference-red.log" >&2; fail "expected proof exit 1, got ${code}"; }
grep -Fq HK00-COMPILER-REFERENCE-UNTRUSTED "${LOGDIR}/rogue-compiler-reference-red.log" || {
  cat "${LOGDIR}/rogue-compiler-reference-red.log" >&2
  fail "rogue compiler reference escaped reference-authority oracle"
}

fresh
prepare_built
run_effective "${LOGDIR}/rogue-compiler-reference-green.log" || {
  cat "${LOGDIR}/rogue-compiler-reference-green.log" >&2
  fail "pristine reconstruction did not return GREEN"
}

printf '| `%s` | `%s` | RED on compiler reference outside packs/dependency outputs/locked test packages → pristine reconstruction → GREEN |\n' \
  'rogue-compiler-reference' 'HK00-COMPILER-REFERENCE-UNTRUSTED' >>"${RESULTS}"

echo "PASS rogue-compiler-reference"
