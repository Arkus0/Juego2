#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
CONFIGURATION="${CONFIGURATION:-Release}"
WORK="${ROOT}/artifacts/self-attacks"
PRISTINE="${WORK}/closure-pristine"
SANDBOX="${WORK}/closure-sandbox"
LOGDIR="${WORK}/logs"
RESULTS="${ROOT}/Docs/evidence/WP-HK-00/self-attacks/results.md"
TOOL_DLL="${ROOT}/artifacts/bootstrap-proof/Arkus.HK00.Proof.dll"

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export MSBUILDDISABLENODEREUSE=1

fail() { echo "CLOSURE ATTACK FAILURE: $*" >&2; exit 1; }

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
  git -C "${SANDBOX}" config user.name "HK00 Closure Attack"
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

run_guard() {
  local phase="$1" log="$2"
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
  [[ ${code} -eq 1 ]] || { cat "${log}" >&2; fail "${check}: expected proof exit 1, got ${code}"; }
  grep -Fq "${check}" "${log}" || { cat "${log}" >&2; fail "${check}: intended oracle did not fire"; }
}

record() {
  printf '| `%s` | `%s` | RED on injection → pristine reconstruction → GREEN |\n' "$1" "$2" >>"${RESULTS}"
  echo "PASS $1"
}

revert_green() {
  local phase="$1" name="$2"
  fresh
  expect_green "${phase}" "${LOGDIR}/${name}-green.log"
}

prepare_built() {
  (cd "${SANDBOX}" && dotnet msbuild Juego2.sln -nologo -noAutoResponse -t:Restore -p:Configuration="${CONFIGURATION}" >/dev/null)
  (cd "${SANDBOX}" && dotnet msbuild Juego2.sln -nologo -noAutoResponse -t:Rebuild -p:Configuration="${CONFIGURATION}" >/dev/null)
}

rm -rf "${PRISTINE}" "${SANDBOX}"
copy_clean_tree "${ROOT}" "${PRISTINE}"
[[ -f "${TOOL_DLL}" ]] || bash "${ROOT}/scripts/build-proof-oracle.sh" >/dev/null
mkdir -p "${LOGDIR}"

fresh
expect_green repository "${LOGDIR}/closure-baseline-repository.log"
expect_green static "${LOGDIR}/closure-baseline-static.log"

fresh
(cd "${SANDBOX}" && dotnet sln Juego2.sln remove src/Arkus.Harness.Cli/Arkus.Harness.Cli.csproj >/dev/null)
expect_red repository HK00-SOLUTION-MISSING-PROJECT "${LOGDIR}/solution-omits-fixed-project-red.log"
revert_green repository solution-omits-fixed-project
record solution-omits-fixed-project HK00-SOLUTION-MISSING-PROJECT

fresh
cat >"${SANDBOX}/Directory.Build.targets" <<'XML'
<Project><Target Name="HiddenRepositoryBuildExtension" BeforeTargets="BeforeBuild" /></Project>
XML
git -C "${SANDBOX}" add Directory.Build.targets
expect_red repository HK00-MSBUILD-FILE-UNCLASSIFIED "${LOGDIR}/auto-directory-build-target-red.log"
revert_green repository auto-directory-build-target
record auto-directory-build-target HK00-MSBUILD-FILE-UNCLASSIFIED

fresh
mkdir -p "${SANDBOX}/outside/Rogue"
cat >"${SANDBOX}/outside/Rogue/Rogue.fsproj" <<'XML'
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net8.0</TargetFramework></PropertyGroup></Project>
XML
git -C "${SANDBOX}" add outside/Rogue/Rogue.fsproj
(cd "${SANDBOX}" && dotnet sln Juego2.sln add outside/Rogue/Rogue.fsproj >/dev/null)
expect_red repository HK00-SOLUTION-UNEXPECTED-PROJECT "${LOGDIR}/solution-noncsharp-participant-red.log"
revert_green repository solution-noncsharp-participant
record solution-noncsharp-participant HK00-SOLUTION-UNEXPECTED-PROJECT

fresh
mkdir -p "${SANDBOX}/outside"
echo '<Project />' >"${SANDBOX}/outside/Hidden.xyz"
git -C "${SANDBOX}" add outside/Hidden.xyz
expect_red repository HK00-MSBUILD-FILE-UNCLASSIFIED "${LOGDIR}/unclassified-msbuild-project-red.log"
revert_green repository unclassified-msbuild-project
record unclassified-msbuild-project HK00-MSBUILD-FILE-UNCLASSIFIED

fresh
insert_xml "${SANDBOX}/tools/Arkus.HK00.Proof/Arkus.HK00.Proof.csproj" \
  '  <Target Name="ReplaceProofCompile" BeforeTargets="CoreCompile"><ItemGroup><Compile Remove="Contract.cs" /></ItemGroup></Target>'
expect_red repository HK00-BUILD-XML-SHAPE "${LOGDIR}/proof-project-inline-target-red.log"
revert_green repository proof-project-inline-target
record proof-project-inline-target HK00-BUILD-XML-SHAPE

fresh
echo '-p:TreatWarningsAsErrors=false' >"${SANDBOX}/Directory.Build.rsp"
git -C "${SANDBOX}" add Directory.Build.rsp
expect_red repository HK00-BUILD-RESPONSE-FILE "${LOGDIR}/repository-response-file-red.log"
revert_green repository repository-response-file
record repository-response-file HK00-BUILD-RESPONSE-FILE

fresh
cp "${SANDBOX}/Juego2.sln" "${SANDBOX}/Alternate.sln"
git -C "${SANDBOX}" add Alternate.sln
expect_red repository HK00-ALT-SOLUTION-ENTRYPOINT "${LOGDIR}/alternate-solution-entrypoint-red.log"
revert_green repository alternate-solution-entrypoint
record alternate-solution-entrypoint HK00-ALT-SOLUTION-ENTRYPOINT

fresh
insert_xml "${SANDBOX}/src/Arkus.Game.Core/Arkus.Game.Core.csproj" \
  '  <ItemGroup><AddModules Include="rogue.netmodule" /></ItemGroup>'
expect_red repository HK00-BUILD-XML-SHAPE "${LOGDIR}/unclassified-compiler-input-item-red.log"
revert_green repository unclassified-compiler-input-item
record unclassified-compiler-input-item HK00-BUILD-XML-SHAPE

fresh
echo 'late compiler metadata' >"${SANDBOX}/Rogue.additional"
git -C "${SANDBOX}" add Rogue.additional
insert_xml "${SANDBOX}/src/Arkus.Game.Core/Arkus.Game.Core.csproj" \
  '  <Target Name="LateAdditionalFile" BeforeTargets="CoreCompile"><ItemGroup><AdditionalFiles Include="$(MSBuildProjectDirectory)/../../Rogue.additional" /></ItemGroup></Target>'
prepare_built
expect_red effective HK00-COMPILER-INPUT-UNCLASSIFIED "${LOGDIR}/late-compiler-input-channel-red.log"
fresh
prepare_built
expect_green effective "${LOGDIR}/late-compiler-input-channel-green.log"
record late-compiler-input-channel HK00-COMPILER-INPUT-UNCLASSIFIED

fresh
insert_xml "${SANDBOX}/tools/Arkus.HK00.Proof/Arkus.HK00.Proof.csproj" \
  '  <Target Name="InjectProofGeneratedSource" BeforeTargets="CoreCompile"><WriteLinesToFile File="$(IntermediateOutputPath)InjectedProof.cs" Overwrite="true" Lines="namespace Arkus.HK00.Proof { internal static class InjectedProofSource { } }" /><ItemGroup><Compile Include="$(IntermediateOutputPath)InjectedProof.cs" /></ItemGroup></Target>'
prepare_built
expect_red effective HK00-COMPILER-SOURCE-UNTRACKED "${LOGDIR}/proof-effective-generated-source-red.log"
fresh
prepare_built
expect_green effective "${LOGDIR}/proof-effective-generated-source-green.log"
record proof-effective-generated-source HK00-COMPILER-SOURCE-UNTRACKED

fresh
mkdir -p "${SANDBOX}/tools/Arkus.HK00.Proof/nested"
echo 'this is deliberately invalid C#' >"${SANDBOX}/tools/Arkus.HK00.Proof/nested/Break.cs"
git -C "${SANDBOX}" add tools/Arkus.HK00.Proof/nested/Break.cs
set +e
(cd "${SANDBOX}" && bash scripts/build-proof-oracle.sh) >"${LOGDIR}/bootstrap-recursive-proof-source-red.log" 2>&1
code=$?
set -e
[[ ${code} -ne 0 ]] || fail "bootstrap-recursive-proof-source: nested tracked proof source escaped direct csc bootstrap"
fresh
(cd "${SANDBOX}" && bash scripts/build-proof-oracle.sh) >"${LOGDIR}/bootstrap-recursive-proof-source-green.log" 2>&1 || {
  cat "${LOGDIR}/bootstrap-recursive-proof-source-green.log" >&2
  fail "bootstrap-recursive-proof-source: pristine bootstrap did not return GREEN"
}
record bootstrap-recursive-proof-source DIRECT-CSC-BOOTSTRAP

fresh
prepare_built
MAL="${WORK}/malicious-output"
rm -rf "${MAL}"
mkdir -p "${MAL}"
cat >"${MAL}/Malicious.csproj" <<'XML'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.1</TargetFramework>
    <AssemblyName>Arkus.Game.Core</AssemblyName>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
    <GenerateTargetFrameworkAttribute>false</GenerateTargetFrameworkAttribute>
    <DebugType>portable</DebugType>
  </PropertyGroup>
</Project>
XML
echo 'namespace Arkus.Game.Core { public static class Replaced { public static int Value => 7; } }' >"${MAL}/Replaced.cs"
dotnet build "${MAL}/Malicious.csproj" -c Release -noAutoResponse -v quiet >/dev/null
cp "${MAL}/bin/Release/netstandard2.1/Arkus.Game.Core.dll" \
   "${SANDBOX}/src/Arkus.Game.Core/bin/Release/netstandard2.1/Arkus.Game.Core.dll"
expect_red output HK00-PDB-PE-MISMATCH "${LOGDIR}/postcompile-output-substitution-red.log"
fresh
prepare_built
expect_green output "${LOGDIR}/postcompile-output-substitution-green.log"
record postcompile-output-substitution HK00-PDB-PE-MISMATCH

echo "all 12 closure/build-bootstrap attacks passed"
