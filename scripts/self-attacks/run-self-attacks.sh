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

copy_tree() {
  local from="$1" to="$2"
  rm -rf "${to}"
  mkdir -p "${to}"
  tar -C "${from}" \
    --exclude=.git --exclude=artifacts --exclude='*/bin' --exclude='*/obj' --exclude='*/TestResults' \
    -cf - . | tar -C "${to}" -xf -
}

init_git() {
  rm -rf "${SANDBOX}/.git"
  git -C "${SANDBOX}" init -q
  git -C "${SANDBOX}" config user.name "HK00 Self Attack"
  git -C "${SANDBOX}" config user.email "hk00@example.invalid"
  git -C "${SANDBOX}" add -A
  git -C "${SANDBOX}" commit -qm pristine
}

fresh() {
  copy_tree "${PRISTINE}" "${SANDBOX}"
  init_git
}

insert_before_project_end() {
  local file="$1" snippet="$2"
  python3 - "$file" "$snippet" <<'PY'
import sys
p,s=sys.argv[1],sys.argv[2]
t=open(p).read()
assert '</Project>' in t
open(p,'w').write(t.replace('</Project>',s+'\n</Project>',1))
PY
}

run_guard() {
  local phase="$1" log="$2"
  set +e
  (cd "${SANDBOX}" && dotnet "${TOOL_DLL}" --root "${SANDBOX}" --configuration "${CONFIGURATION}" --phase "${phase}") >"${log}" 2>&1
  local code=$?
  set -e
  return ${code}
}

prepare_effective() {
  (cd "${SANDBOX}" && dotnet restore Juego2.sln >/dev/null)
}

assert_green() {
  local phase="$1" log="$2"
  if [[ "${phase}" == "effective" || "${phase}" == "all" ]]; then prepare_effective; fi
  if ! run_guard "${phase}" "${log}"; then
    cat "${log}" >&2
    fail "expected GREEN for phase ${phase}"
  fi
}

assert_red() {
  local phase="$1" expected="$2" log="$3"
  if [[ "${phase}" == "effective" || "${phase}" == "all" ]]; then prepare_effective; fi
  set +e
  run_guard "${phase}" "${log}"
  local code=$?
  set -e
  if [[ ${code} -ne 1 ]]; then
    cat "${log}" >&2
    fail "expected proof finding exit=1, got ${code} for ${expected}"
  fi
  if ! grep -Fq "${expected}" "${log}"; then
    cat "${log}" >&2
    fail "expected check ${expected} did not fire"
  fi
}

prove_revert() {
  local phase="$1" name="$2"
  fresh
  if ! diff -r --exclude=.git --exclude=bin --exclude=obj --exclude=artifacts "${PRISTINE}" "${SANDBOX}" >/dev/null; then
    fail "${name}: fresh revert is not byte-identical"
  fi
  assert_green "${phase}" "${LOGDIR}/${name}-green.log"
}

record() {
  printf '| `%s` | `%s` | RED on injection → GREEN after clean revert |\n' "$1" "$2" >>"${RESULTS}"
  echo "PASS $1"
}

attack_extra_project() {
  local n="extra-project-anywhere"; fresh
  mkdir -p "${SANDBOX}/outside/Rogue"
  cat >"${SANDBOX}/outside/Rogue/Rogue.csproj" <<'XML'
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net8.0</TargetFramework></PropertyGroup></Project>
XML
  echo 'namespace Rogue { public static class Marker { } }' >"${SANDBOX}/outside/Rogue/Marker.cs"
  git -C "${SANDBOX}" add outside
  assert_red repository HK00-PROJECT-UNEXPECTED "${LOGDIR}/${n}-red.log"
  prove_revert repository "$n"; record "$n" HK00-PROJECT-UNEXPECTED
}

attack_missing_project() {
  local n="missing-fixed-project"; fresh
  git -C "${SANDBOX}" rm -q src/Arkus.Harness.Cli/Arkus.Harness.Cli.csproj
  assert_red repository HK00-PROJECT-MISSING "${LOGDIR}/${n}-red.log"
  prove_revert repository "$n"; record "$n" HK00-PROJECT-MISSING
}

attack_unowned_source() {
  local n="unowned-source-anywhere"; fresh
  mkdir -p "${SANDBOX}/Docs/rogue"
  echo 'namespace RogueDocs { public static class Marker { } }' >"${SANDBOX}/Docs/rogue/Marker.cs"
  git -C "${SANDBOX}" add Docs/rogue/Marker.cs
  assert_red repository HK00-SOURCE-UNOWNED "${LOGDIR}/${n}-red.log"
  prove_revert repository "$n"; record "$n" HK00-SOURCE-UNOWNED
}

attack_policy_relax() {
  local n="fixed-policy-relaxed-in-build"; fresh
  sed -i 's/<TreatWarningsAsErrors>true/<TreatWarningsAsErrors>false/' "${SANDBOX}/Directory.Build.props"
  assert_red static HK00-PROJECT-PROPERTY "${LOGDIR}/${n}-red.log"
  prove_revert static "$n"; record "$n" HK00-PROJECT-PROPERTY
}

attack_cycle() {
  local n="dependency-cycle-backedge"; fresh
  insert_before_project_end "${SANDBOX}/src/Arkus.Game.Core/Arkus.Game.Core.csproj" '  <ItemGroup><ProjectReference Include="../Arkus.Game.World/Arkus.Game.World.csproj" /></ItemGroup>'
  assert_red static HK00-GRAPH-UNDECLARED-EDGE "${LOGDIR}/${n}-red.log"
  grep -Fq HK00-GRAPH-CYCLE "${LOGDIR}/${n}-red.log" || fail "${n}: cycle oracle did not also fire"
  prove_revert static "$n"; record "$n" HK00-GRAPH-CYCLE
}

attack_duplicate_compile() {
  local n="cross-project-source-compile"; fresh
  insert_before_project_end "${SANDBOX}/src/Arkus.Game.World/Arkus.Game.World.csproj" '  <ItemGroup><Compile Include="../Arkus.Game.Core/CoreModule.cs" /></ItemGroup>'
  assert_red static HK00-SOURCE-FOREIGN-COMPILE "${LOGDIR}/${n}-red.log"
  prove_revert static "$n"; record "$n" HK00-SOURCE-FOREIGN-COMPILE
}

attack_package() {
  local n="production-package-injected"; fresh
  insert_before_project_end "${SANDBOX}/src/Arkus.Game.Core/Arkus.Game.Core.csproj" '  <ItemGroup><PackageReference Include="xunit" /></ItemGroup>'
  assert_red static HK00-PACKAGE-FORBIDDEN "${LOGDIR}/${n}-red.log"
  prove_revert static "$n"; record "$n" HK00-PACKAGE-FORBIDDEN
}

attack_engine_reference() {
  local n="engine-raw-reference"; fresh
  insert_before_project_end "${SANDBOX}/src/Arkus.Game.Core/Arkus.Game.Core.csproj" '  <ItemGroup><Reference Include="UnityEngine" /></ItemGroup>'
  assert_red static HK00-ENGINE-REFERENCE "${LOGDIR}/${n}-red.log"
  prove_revert static "$n"; record "$n" HK00-ENGINE-REFERENCE
}

attack_drop_effective_source() {
  local n="source-dropped-at-build-time"; fresh
  cat >"${SANDBOX}/src/Arkus.Game.World/Detached.cs" <<'CS'
namespace Arkus.Game.World { public static class Detached { public const int Value = 1; } }
CS
  git -C "${SANDBOX}" add src/Arkus.Game.World/Detached.cs
  insert_before_project_end "${SANDBOX}/src/Arkus.Game.World/Arkus.Game.World.csproj" '  <Target Name="DropDetached" BeforeTargets="CoreCompile"><ItemGroup><Compile Remove="Detached.cs" /></ItemGroup></Target>'
  assert_red effective HK00-SOURCE-NOT-COMPILED-EFFECTIVE "${LOGDIR}/${n}-red.log"
  prove_revert effective "$n"; record "$n" HK00-SOURCE-NOT-COMPILED-EFFECTIVE
}

attack_generated_source() {
  local n="generated-product-source-injected"; fresh
  insert_before_project_end "${SANDBOX}/src/Arkus.Game.World/Arkus.Game.World.csproj" '  <Target Name="InjectGenerated" BeforeTargets="CoreCompile"><WriteLinesToFile File="$(IntermediateOutputPath)Fake.AssemblyInfo.cs" Overwrite="true" Lines="namespace Arkus.Game.World { public static class Smuggled { } }" /><ItemGroup><Compile Include="$(IntermediateOutputPath)Fake.AssemblyInfo.cs" /></ItemGroup></Target>'
  assert_red effective HK00-COMPILER-SOURCE-UNTRACKED "${LOGDIR}/${n}-red.log"
  prove_revert effective "$n"; record "$n" HK00-COMPILER-SOURCE-UNTRACKED
}

attack_external_source() {
  local n="external-source-injected-at-build-time"; fresh
  local external="${WORK}/external/Injected.cs"
  mkdir -p "$(dirname "${external}")"
  echo 'namespace ExternalInjected { public static class Marker { } }' >"${external}"
  insert_before_project_end "${SANDBOX}/src/Arkus.Game.Core/Arkus.Game.Core.csproj" "  <Target Name=\"InjectExternal\" BeforeTargets=\"CoreCompile\"><ItemGroup><Compile Include=\"${external}\" /></ItemGroup></Target>"
  assert_red effective HK00-COMPILER-SOURCE-FOREIGN "${LOGDIR}/${n}-red.log"
  rm -rf "${WORK}/external"
  prove_revert effective "$n"; record "$n" HK00-COMPILER-SOURCE-FOREIGN
}

attack_warning_guard() {
  local n="real-warning-is-error"; fresh
  cat >"${SANDBOX}/src/Arkus.Game.Core/Sloppy.cs" <<'CS'
namespace Arkus.Game.Core { public static class Sloppy { public static void Run() { var unused = 1; } } }
CS
  git -C "${SANDBOX}" add src/Arkus.Game.Core/Sloppy.cs
  (cd "${SANDBOX}" && dotnet restore src/Arkus.Game.Core/Arkus.Game.Core.csproj >/dev/null)
  set +e
  (cd "${SANDBOX}" && dotnet build src/Arkus.Game.Core/Arkus.Game.Core.csproj -c Release --no-restore -t:Rebuild) >"${LOGDIR}/${n}-red.log" 2>&1
  local code=$?
  set -e
  [[ ${code} -ne 0 ]] || fail "${n}: warning build stayed green"
  grep -Fq 'CS0219' "${LOGDIR}/${n}-red.log" || fail "${n}: intended compiler warning not observed"
  prove_revert static "$n"; record "$n" CS0219
}

attack_toolchain() {
  local n="toolchain-pin-relaxed"; fresh
  sed -i 's/"latestPatch"/"major"/' "${SANDBOX}/global.json"
  assert_red repository HK00-TOOLCHAIN-PIN "${LOGDIR}/${n}-red.log"
  prove_revert repository "$n"; record "$n" HK00-TOOLCHAIN-PIN
}

attack_proof_deleted() {
  local n="proof-project-deleted"; fresh
  rm "${SANDBOX}/tools/Arkus.HK00.Proof/Arkus.HK00.Proof.csproj"
  assert_red repository HK00-TRACKED-MISSING "${LOGDIR}/${n}-red.log"
  prove_revert repository "$n"; record "$n" HK00-TRACKED-MISSING
}

attack_legacy_manifest_cannot_shrink() {
  local n="legacy-manifest-cannot-shrink-universe"; fresh
  echo '{"repository":{"sourceScanRoots":["src"]}}' >"${SANDBOX}/kernel-manifest.json"
  mkdir -p "${SANDBOX}/outside/Hidden"
  cat >"${SANDBOX}/outside/Hidden/Hidden.csproj" <<'XML'
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net8.0</TargetFramework></PropertyGroup></Project>
XML
  echo 'namespace Hidden { public static class Marker { } }' >"${SANDBOX}/outside/Hidden/Marker.cs"
  git -C "${SANDBOX}" add kernel-manifest.json outside
  assert_red repository HK00-PROJECT-UNEXPECTED "${LOGDIR}/${n}-red.log"
  prove_revert repository "$n"; record "$n" HK00-PROJECT-UNEXPECTED
}

main() {
  rm -rf "${WORK}"
  mkdir -p "${WORK}" "${LOGDIR}" "$(dirname "${RESULTS}")"

  dotnet restore "${TOOL_PROJECT}" >/dev/null
  dotnet build "${TOOL_PROJECT}" -c "${CONFIGURATION}" --no-restore -v minimal >/dev/null
  [[ -f "${TOOL_DLL}" ]] || fail "proof DLL missing"

  copy_tree "${ROOT}" "${PRISTINE}"
  cat >"${RESULTS}" <<'MD'
# WP-HK-00 causal self-attacks

| Attack | Intended oracle/check | Result |
|---|---|---|
MD

  fresh
  assert_green repository "${LOGDIR}/baseline-repository.log"
  assert_green static "${LOGDIR}/baseline-static.log"
  assert_green effective "${LOGDIR}/baseline-effective.log"

  attack_extra_project
  attack_missing_project
  attack_unowned_source
  attack_policy_relax
  attack_cycle
  attack_duplicate_compile
  attack_package
  attack_engine_reference
  attack_drop_effective_source
  attack_generated_source
  attack_external_source
  attack_warning_guard
  attack_toolchain
  attack_proof_deleted
  attack_legacy_manifest_cannot_shrink

  echo "all 15 causal self-attacks passed"
}

main "$@"
