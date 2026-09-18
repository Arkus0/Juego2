#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
CONFIGURATION="${CONFIGURATION:-Release}"
WORK="${ROOT}/artifacts/self-attacks"
PRISTINE="${WORK}/pristine"
SANDBOX="${WORK}/sandbox"
LOGDIR="${WORK}/logs"
RESULTS="${ROOT}/Docs/evidence/WP-HK-00/self-attacks/results.md"
TOOL_PROJECT="${ROOT}/tools/Arkus.HK00.Proof/Arkus.HK00.Proof.csproj"
TOOL_DLL="${ROOT}/tools/Arkus.HK00.Proof/bin/${CONFIGURATION}/net8.0/Arkus.HK00.Proof.dll"

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export MSBUILDDISABLENODEREUSE=1

fail() { echo "SELF-ATTACK FAILURE: $*" >&2; exit 1; }

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
  git -C "${SANDBOX}" config user.name "HK00 Self Attack"
  git -C "${SANDBOX}" config user.email "hk00@example.invalid"
  git -C "${SANDBOX}" add -A
  git -C "${SANDBOX}" commit -qm pristine
  local dirty
  dirty="$(git -C "${SANDBOX}" status --porcelain --untracked-files=all)"
  [[ -z "${dirty}" ]] || { printf '%s\n' "${dirty}" >&2; fail "fresh sandbox is not clean"; }
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

prepare_effective() {
  (cd "${SANDBOX}" && dotnet restore Juego2.sln >/dev/null)
}

run_guard() {
  local phase="$1" log="$2"
  if [[ "${phase}" == "effective" || "${phase}" == "all" ]]; then
    prepare_effective
  fi
  (cd "${SANDBOX}" && dotnet "${TOOL_DLL}" --root "${SANDBOX}" --configuration "${CONFIGURATION}" --phase "${phase}") >"${log}" 2>&1
}

expect_green() {
  local phase="$1" log="$2"
  if ! run_guard "${phase}" "${log}"; then
    cat "${log}" >&2
    fail "expected GREEN for ${phase}"
  fi
}

expect_red() {
  local phase="$1" check="$2" log="$3"
  set +e
  run_guard "${phase}" "${log}"
  local code=$?
  set -e
  if [[ ${code} -ne 1 ]]; then
    cat "${log}" >&2
    fail "${check}: expected proof exit 1, got ${code}"
  fi
  if ! grep -Fq "${check}" "${log}"; then
    cat "${log}" >&2
    fail "${check}: intended oracle did not fire"
  fi
}

revert_and_green() {
  local phase="$1" name="$2"
  # Revert by destroying the mutated checkout and reconstructing it from the
  # immutable pristine snapshot captured before any attack. This avoids hidden
  # state from package/build tooling becoming part of the revert proof.
  fresh
  expect_green "${phase}" "${LOGDIR}/${name}-green.log"
}

record() {
  printf '| `%s` | `%s` | RED on injection → pristine reconstruction → GREEN |\n' "$1" "$2" >>"${RESULTS}"
  echo "PASS ${1}"
}

run_attack() {
  local name="$1" phase="$2" check="$3" mutation="$4"
  fresh
  "${mutation}"
  expect_red "${phase}" "${check}" "${LOGDIR}/${name}-red.log"
  rm -rf "${WORK}/external"
  revert_and_green "${phase}" "${name}"
  record "${name}" "${check}"
}

m_extra_project() {
  mkdir -p "${SANDBOX}/outside/Rogue"
  printf '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net8.0</TargetFramework></PropertyGroup></Project>\n' >"${SANDBOX}/outside/Rogue/Rogue.csproj"
  echo 'namespace Rogue { public static class Marker { } }' >"${SANDBOX}/outside/Rogue/Marker.cs"
  git -C "${SANDBOX}" add outside
}

m_missing_project() {
  git -C "${SANDBOX}" rm -q src/Arkus.Harness.Cli/Arkus.Harness.Cli.csproj
}

m_unowned_source() {
  mkdir -p "${SANDBOX}/Docs/rogue"
  echo 'namespace RogueDocs { public static class Marker { } }' >"${SANDBOX}/Docs/rogue/Marker.cs"
  git -C "${SANDBOX}" add Docs/rogue/Marker.cs
}

m_relax_policy() {
  sed -i 's/<TreatWarningsAsErrors>true/<TreatWarningsAsErrors>false/' "${SANDBOX}/Directory.Build.props"
}

m_cycle() {
  insert_xml "${SANDBOX}/src/Arkus.Game.Core/Arkus.Game.Core.csproj" '  <ItemGroup><ProjectReference Include="../Arkus.Game.World/Arkus.Game.World.csproj" /></ItemGroup>'
}

m_cross_compile() {
  insert_xml "${SANDBOX}/src/Arkus.Game.World/Arkus.Game.World.csproj" '  <ItemGroup><Compile Include="../Arkus.Game.Core/CoreModule.cs" /></ItemGroup>'
}

m_package() {
  insert_xml "${SANDBOX}/src/Arkus.Game.Core/Arkus.Game.Core.csproj" '  <ItemGroup><PackageReference Include="xunit" /></ItemGroup>'
}

m_engine_ref() {
  insert_xml "${SANDBOX}/src/Arkus.Game.Core/Arkus.Game.Core.csproj" '  <ItemGroup><Reference Include="UnityEngine" /></ItemGroup>'
}

m_drop_effective() {
  echo 'namespace Arkus.Game.World { public static class Detached { public const int Value = 1; } }' >"${SANDBOX}/src/Arkus.Game.World/Detached.cs"
  git -C "${SANDBOX}" add src/Arkus.Game.World/Detached.cs
  insert_xml "${SANDBOX}/src/Arkus.Game.World/Arkus.Game.World.csproj" '  <Target Name="DropDetached" BeforeTargets="CoreCompile"><ItemGroup><Compile Remove="Detached.cs" /></ItemGroup></Target>'
}

m_generated() {
  insert_xml "${SANDBOX}/src/Arkus.Game.World/Arkus.Game.World.csproj" '  <Target Name="InjectGenerated" BeforeTargets="CoreCompile"><WriteLinesToFile File="$(IntermediateOutputPath)Fake.AssemblyInfo.cs" Overwrite="true" Lines="namespace Arkus.Game.World { public static class Smuggled { } }" /><ItemGroup><Compile Include="$(IntermediateOutputPath)Fake.AssemblyInfo.cs" /></ItemGroup></Target>'
}

m_external() {
  local external="${WORK}/external/Injected.cs"
  mkdir -p "$(dirname "${external}")"
  echo 'namespace ExternalInjected { public static class Marker { } }' >"${external}"
  insert_xml "${SANDBOX}/src/Arkus.Game.Core/Arkus.Game.Core.csproj" "  <Target Name=\"InjectExternal\" BeforeTargets=\"CoreCompile\"><ItemGroup><Compile Include=\"${external}\" /></ItemGroup></Target>"
}

m_toolchain() {
  sed -i 's/"latestPatch"/"major"/' "${SANDBOX}/global.json"
}

m_proof_deleted() {
  rm "${SANDBOX}/tools/Arkus.HK00.Proof/Arkus.HK00.Proof.csproj"
}

m_legacy_manifest() {
  echo '{"repository":{"sourceScanRoots":["src"]}}' >"${SANDBOX}/kernel-manifest.json"
  mkdir -p "${SANDBOX}/outside/Hidden"
  printf '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net8.0</TargetFramework></PropertyGroup></Project>\n' >"${SANDBOX}/outside/Hidden/Hidden.csproj"
  echo 'namespace Hidden { public static class Marker { } }' >"${SANDBOX}/outside/Hidden/Marker.cs"
  git -C "${SANDBOX}" add kernel-manifest.json outside
}

warning_attack() {
  local name="real-warning-is-error"
  fresh
  echo 'namespace Arkus.Game.Core { public static class Sloppy { public static void Run() { var unused = 1; } } }' >"${SANDBOX}/src/Arkus.Game.Core/Sloppy.cs"
  git -C "${SANDBOX}" add src/Arkus.Game.Core/Sloppy.cs
  (cd "${SANDBOX}" && dotnet restore src/Arkus.Game.Core/Arkus.Game.Core.csproj >/dev/null)
  set +e
  (cd "${SANDBOX}" && dotnet build src/Arkus.Game.Core/Arkus.Game.Core.csproj -c Release --no-restore -t:Rebuild) >"${LOGDIR}/${name}-red.log" 2>&1
  local code=$?
  set -e
  [[ ${code} -ne 0 ]] || fail "${name}: warning build stayed green"
  grep -Fq CS0219 "${LOGDIR}/${name}-red.log" || fail "${name}: intended warning not observed"
  revert_and_green static "${name}"
  record "${name}" CS0219
}

main() {
  rm -rf "${WORK}"
  mkdir -p "${WORK}" "${LOGDIR}" "$(dirname "${RESULTS}")"
  dotnet restore "${TOOL_PROJECT}" >/dev/null
  dotnet build "${TOOL_PROJECT}" -c "${CONFIGURATION}" --no-restore -v minimal >/dev/null
  [[ -f "${TOOL_DLL}" ]] || fail "proof DLL missing"

  copy_clean_tree "${ROOT}" "${PRISTINE}"
  cat >"${RESULTS}" <<'MD'
# WP-HK-00 causal self-attacks

| Attack | Intended oracle/check | Result |
|---|---|---|
MD

  fresh; expect_green repository "${LOGDIR}/baseline-repository.log"
  fresh; expect_green static "${LOGDIR}/baseline-static.log"
  fresh; expect_green effective "${LOGDIR}/baseline-effective.log"

  run_attack extra-project-anywhere repository HK00-PROJECT-UNEXPECTED m_extra_project
  run_attack missing-fixed-project repository HK00-PROJECT-MISSING m_missing_project
  run_attack unowned-source-anywhere repository HK00-SOURCE-UNOWNED m_unowned_source
  run_attack fixed-policy-relaxed-in-build static HK00-PROJECT-PROPERTY m_relax_policy
  run_attack dependency-cycle-backedge static HK00-GRAPH-UNDECLARED-EDGE m_cycle
  grep -Fq HK00-GRAPH-CYCLE "${LOGDIR}/dependency-cycle-backedge-red.log" || fail "cycle oracle did not also fire"
  run_attack cross-project-source-compile static HK00-SOURCE-FOREIGN-COMPILE m_cross_compile
  run_attack production-package-injected static HK00-PACKAGE-FORBIDDEN m_package
  run_attack engine-raw-reference static HK00-ENGINE-REFERENCE m_engine_ref
  run_attack source-dropped-at-build-time effective HK00-SOURCE-NOT-COMPILED-EFFECTIVE m_drop_effective
  run_attack generated-product-source-injected effective HK00-COMPILER-SOURCE-UNTRACKED m_generated
  run_attack external-source-injected-at-build-time effective HK00-COMPILER-SOURCE-FOREIGN m_external
  warning_attack
  run_attack toolchain-pin-relaxed repository HK00-TOOLCHAIN-PIN m_toolchain
  run_attack proof-project-deleted repository HK00-TRACKED-MISSING m_proof_deleted
  run_attack legacy-manifest-cannot-shrink-universe repository HK00-PROJECT-UNEXPECTED m_legacy_manifest

  echo "all 15 causal self-attacks passed"
}

main "$@"
