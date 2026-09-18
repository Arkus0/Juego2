#!/usr/bin/env bash
#
# WP-HK-00 causal self-attacks.
#
# For every material defect class that could leave CI green while the boundary
# claim is false, this harness injects the defect into a sandbox copy of the
# repository, requires the intended guard to fire for the intended reason,
# reverts the defect, proves the revert is byte-exact against the pristine tree,
# and proves the sandbox is green again.
#
# A run only passes if every attack completed that whole cycle. Asserting on
# stable check identifiers (not on exit codes or message text) is what keeps an
# unrelated compile failure from being mistaken for evidence.
#
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
CONFIGURATION="${CONFIGURATION:-Release}"
WORK="${SELF_ATTACK_WORKDIR:-${ROOT}/artifacts/self-attacks}"
PRISTINE="${WORK}/pristine"
SANDBOX="${WORK}/sandbox"
EVIDENCE="${ROOT}/Docs/evidence/WP-HK-00/self-attacks"
PROOF_TOOL_DLL="${ROOT}/artifacts/bin/Arkus.Kernel.Proof.Tool/${CONFIGURATION,,}/Arkus.Kernel.Proof.Tool.dll"
RESULTS="${EVIDENCE}/results.md"

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
export MSBUILDDISABLENODEREUSE=1

ATTACKS=(
  forbidden-engine-dependency
  dependency-cycle
  layering-back-edge
  duplicate-source-ownership
  duplicate-source-type-conflict
  unclassified-production-source
  source-excluded-from-compilation
  source-excluded-by-build-target
  toolchain-property-drift
  toolchain-option-mutated-at-build-time
  hidden-generated-source
  toolchain-pin-loosened
  warnings-as-errors-neutralised
  warning-suppression-added
  effective-warning-as-error
  required-project-deleted
  proof-tool-deleted
  package-pin-bypassed
  forbidden-engine-package
)

declare -a TOUCHED=()
ATTACK_PHASE=""
ATTACK_EXPECT=""
ATTACK_EXPECT_ABSENT=""
ATTACK_BUILD=""
ATTACK_BUILD_EXPECT=""
ATTACK_BUILD_GREP=""
ATTACK_DESCRIPTION=""

log() {
  printf '%s\n' "$*"
}

fail() {
  printf 'SELF-ATTACK FAILURE: %s\n' "$*" >&2
  exit 1
}

copy_tree() {
  local from="$1" to="$2"
  rm -rf "${to}"
  mkdir -p "${to}"
  tar -C "${from}" --exclude=./.git --exclude=./artifacts -cf - . | tar -C "${to}" -xf -
}

sandbox_build() {
  local logfile="$1"
  set +e
  (cd "${SANDBOX}" && dotnet build Juego2.sln -c "${CONFIGURATION}" -v minimal) >>"${logfile}" 2>&1
  local code=$?
  set -e
  return ${code}
}

run_proof() {
  local logfile="$1"
  shift
  set +e
  dotnet "${PROOF_TOOL_DLL}" \
    --repository-root "${SANDBOX}" \
    --configuration "${CONFIGURATION}" \
    "$@" >>"${logfile}" 2>&1
  local code=$?
  set -e
  return ${code}
}

write_file() {
  local path="$1"
  shift
  mkdir -p "$(dirname "${path}")"
  cat >"${path}"
}

insert_before_closing_project() {
  local file="$1" snippet="$2"
  python3 - "${file}" "${snippet}" <<'PY'
import sys
path, snippet = sys.argv[1], sys.argv[2]
text = open(path).read()
marker = "</Project>"
assert marker in text, path
open(path, "w").write(text.replace(marker, snippet + "\n" + marker, 1))
PY
}

sanitize_log() {
  # Keep committed evidence free of machine-specific paths.
  python3 - "$1" "${SANDBOX}" "${ROOT}" <<'PY'
import sys
path, sandbox, root = sys.argv[1], sys.argv[2], sys.argv[3]
text = open(path).read().replace(sandbox, "<SANDBOX>").replace(root, "<REPO>")
open(path, "w").write(text)
PY
}

revert_attack() {
  # The revert is deliberately generic rather than a targeted undo of the paths
  # the attack listed: a defect can have side effects the attack did not name
  # (a restore rewriting lock files, for example), and an incomplete revert would
  # quietly weaken the "green again" half of the evidence.
  # -m (do not preserve mtimes) matters: restoring original timestamps would let
  # MSBuild consider a tampered output up to date and skip the rebuild, so the
  # "green again" run could silently inspect the attacked binaries.
  tar -C "${PRISTINE}" -cf - . | tar -C "${SANDBOX}" -xmf -

  local rel
  while IFS= read -r rel; do
    if [[ ! -e "${PRISTINE}/${rel}" ]]; then
      rm -f "${SANDBOX}/${rel}"
    fi
  done < <(cd "${SANDBOX}" && find . -path ./artifacts -prune -o -type f -print)

  while IFS= read -r rel; do
    if [[ "${rel}" != "." && ! -d "${PRISTINE}/${rel}" ]]; then
      rmdir "${SANDBOX}/${rel}" 2>/dev/null || true
    fi
  done < <(cd "${SANDBOX}" && find . -path ./artifacts -prune -o -type d -print | sort -r)
}

prove_revert_is_exact() {
  local logfile="$1"
  if ! diff -r --exclude=artifacts "${PRISTINE}" "${SANDBOX}" >>"${logfile}" 2>&1; then
    fail "sandbox is not byte-identical to the pristine tree after revert"
  fi
}

configure_attack() {
  TOUCHED=()
  ATTACK_PHASE="static"
  ATTACK_EXPECT=""
  ATTACK_EXPECT_ABSENT=""
  ATTACK_BUILD="no"
  ATTACK_BUILD_EXPECT="succeed"
  ATTACK_BUILD_GREP=""
  ATTACK_DESCRIPTION=""

  case "$1" in
    forbidden-engine-dependency)
      ATTACK_DESCRIPTION="A Unity engine assembly is referenced and used by a portable kernel module."
      ATTACK_PHASE="all"
      ATTACK_BUILD="yes"
      ATTACK_EXPECT="HK00-ENGINE-DEP-STATIC HK00-ENGINE-DEP-EFFECTIVE"
      TOUCHED=(
        "src/UnityEngine/UnityEngine.csproj"
        "src/UnityEngine/GameObject.cs"
        "src/Arkus.Game.World/Arkus.Game.World.csproj"
        "src/Arkus.Game.World/EngineBridge.cs"
      )
      ;;
    dependency-cycle)
      ATTACK_DESCRIPTION="Arkus.Game.Core is made to depend back on Arkus.Game.World."
      ATTACK_PHASE="static"
      ATTACK_BUILD="yes"
      ATTACK_BUILD_EXPECT="fail"
      ATTACK_BUILD_GREP="ycle"
      ATTACK_EXPECT="HK00-GRAPH-CYCLE HK00-GRAPH-UNDECLARED-EDGE"
      TOUCHED=("src/Arkus.Game.Core/Arkus.Game.Core.csproj")
      ;;
    layering-back-edge)
      ATTACK_DESCRIPTION="The runtime reaches past authoring/validation straight into core."
      ATTACK_PHASE="all"
      ATTACK_BUILD="yes"
      ATTACK_EXPECT="HK00-GRAPH-UNDECLARED-EDGE HK00-REF-UNDECLARED-EFFECTIVE"
      TOUCHED=(
        "src/Arkus.Harness.Runtime/Arkus.Harness.Runtime.csproj"
        "src/Arkus.Harness.Runtime/CoreBridge.cs"
      )
      ;;
    duplicate-source-ownership)
      ATTACK_DESCRIPTION="One product source file is compiled into two assemblies without breaking the build."
      ATTACK_PHASE="all"
      ATTACK_BUILD="yes"
      ATTACK_EXPECT="HK00-SOURCE-DUPLICATE-OWNERSHIP HK00-SOURCE-FOREIGN-COMPILE-ITEM HK00-SOURCE-FOREIGN-COMPILED-EFFECTIVE"
      TOUCHED=(
        "src/Arkus.Game.Core/SharedHelper.cs"
        "src/Arkus.Game.World/Arkus.Game.World.csproj"
      )
      ;;
    duplicate-source-type-conflict)
      ATTACK_DESCRIPTION="A product source file is linked into a second assembly whose type the graph already exports; the compiler must reject it too."
      ATTACK_PHASE="static"
      ATTACK_BUILD="yes"
      ATTACK_BUILD_EXPECT="fail"
      ATTACK_BUILD_GREP="error CS0433"
      ATTACK_EXPECT="HK00-SOURCE-DUPLICATE-OWNERSHIP HK00-SOURCE-FOREIGN-COMPILE-ITEM"
      TOUCHED=("src/Arkus.Game.Authoring/Arkus.Game.Authoring.csproj")
      ;;
    unclassified-production-source)
      ATTACK_DESCRIPTION="A production source file is added that no classified project owns."
      ATTACK_EXPECT="HK00-SOURCE-UNCLASSIFIED"
      TOUCHED=("src/Orphan/OrphanType.cs")
      ;;
    source-excluded-from-compilation)
      ATTACK_DESCRIPTION="An owned source file is removed from the compiler inputs in the project file."
      ATTACK_PHASE="all"
      ATTACK_BUILD="yes"
      ATTACK_EXPECT="HK00-SOURCE-NOT-COMPILED-STATIC HK00-SOURCE-NOT-COMPILED-EFFECTIVE"
      TOUCHED=(
        "src/Arkus.Game.World/Detached.cs"
        "src/Arkus.Game.World/Arkus.Game.World.csproj"
      )
      ;;
    source-excluded-by-build-target)
      ATTACK_DESCRIPTION="An owned source file is dropped during the build, after evaluation, so only the effective oracle can see it."
      ATTACK_PHASE="all"
      ATTACK_BUILD="yes"
      ATTACK_EXPECT="HK00-SOURCE-NOT-COMPILED-EFFECTIVE"
      ATTACK_EXPECT_ABSENT="HK00-SOURCE-NOT-COMPILED-STATIC"
      TOUCHED=(
        "src/Arkus.Game.World/Detached.cs"
        "src/Arkus.Game.World/Arkus.Game.World.csproj"
      )
      ;;
    toolchain-property-drift)
      ATTACK_DESCRIPTION="A kernel project drifts off the pinned language version."
      ATTACK_EXPECT="HK00-TOOLCHAIN-PROPERTY"
      TOUCHED=("src/Arkus.Game.Core/Arkus.Game.Core.csproj")
      ;;
    toolchain-option-mutated-at-build-time)
      ATTACK_DESCRIPTION="Warnings-as-errors is switched off during the build, after evaluation, so only the compiler command line can see it."
      ATTACK_PHASE="all"
      ATTACK_BUILD="yes"
      ATTACK_EXPECT="HK00-COMPILER-OPTION"
      ATTACK_EXPECT_ABSENT="HK00-TOOLCHAIN-PROPERTY"
      TOUCHED=("src/Arkus.Game.Core/Arkus.Game.Core.csproj")
      ;;
    hidden-generated-source)
      ATTACK_DESCRIPTION="Product code is smuggled in as a build-generated source file under the intermediate output directory."
      ATTACK_PHASE="all"
      ATTACK_BUILD="yes"
      ATTACK_EXPECT="HK00-SOURCE-UNEXPECTED-GENERATED"
      TOUCHED=("src/Arkus.Game.World/Arkus.Game.World.csproj")
      ;;
    toolchain-pin-loosened)
      ATTACK_DESCRIPTION="The SDK pin is loosened to roll forward across major versions."
      ATTACK_EXPECT="HK00-TOOLCHAIN-SDK-PIN"
      TOUCHED=("global.json")
      ;;
    warnings-as-errors-neutralised)
      ATTACK_DESCRIPTION="Warnings-as-errors is switched off for a kernel project."
      ATTACK_EXPECT="HK00-TOOLCHAIN-PROPERTY"
      TOUCHED=("src/Arkus.Game.Core/Arkus.Game.Core.csproj")
      ;;
    warning-suppression-added)
      ATTACK_DESCRIPTION="A compiler warning is suppressed outside the allow-list."
      ATTACK_EXPECT="HK00-TOOLCHAIN-SUPPRESSION"
      TOUCHED=("src/Arkus.Game.Core/Arkus.Game.Core.csproj")
      ;;
    effective-warning-as-error)
      ATTACK_DESCRIPTION="A real compiler warning is introduced; the build itself must reject it."
      ATTACK_PHASE="none"
      ATTACK_BUILD="yes"
      ATTACK_BUILD_EXPECT="fail"
      ATTACK_BUILD_GREP="error CS0219"
      TOUCHED=("src/Arkus.Game.Core/SloppyCode.cs")
      ;;
    required-project-deleted)
      ATTACK_DESCRIPTION="A required kernel project file is deleted."
      ATTACK_PHASE="preflight"
      ATTACK_EXPECT="HK00-PREFLIGHT-PROJECT-MISSING"
      TOUCHED=("src/Arkus.Harness.Cli/Arkus.Harness.Cli.csproj")
      ;;
    proof-tool-deleted)
      ATTACK_DESCRIPTION="The proof tool project is deleted."
      ATTACK_PHASE="preflight"
      ATTACK_EXPECT="HK00-PREFLIGHT-PROOF-TOOL-MISSING"
      TOUCHED=("tools/Arkus.Kernel.Proof.Tool/Arkus.Kernel.Proof.Tool.csproj")
      ;;
    package-pin-bypassed)
      ATTACK_DESCRIPTION="A package version is pinned inline instead of centrally."
      ATTACK_EXPECT="HK00-PACKAGE-UNPINNED"
      TOUCHED=("tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj")
      ;;
    forbidden-engine-package)
      ATTACK_DESCRIPTION="A portable kernel project takes a Unity NuGet dependency."
      ATTACK_EXPECT="HK00-ENGINE-DEP-STATIC HK00-PACKAGE-FORBIDDEN"
      TOUCHED=("src/Arkus.Game.Core/Arkus.Game.Core.csproj")
      ;;
    *)
      fail "unknown attack '$1'"
      ;;
  esac
}

apply_attack() {
  case "$1" in
    forbidden-engine-dependency)
      write_file "${SANDBOX}/src/UnityEngine/UnityEngine.csproj" <<'XML'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.1</TargetFramework>
    <AssemblyName>UnityEngine</AssemblyName>
    <RootNamespace>UnityEngine</RootNamespace>
  </PropertyGroup>
</Project>
XML
      write_file "${SANDBOX}/src/UnityEngine/GameObject.cs" <<'CS'
namespace UnityEngine
{
    public sealed class GameObject
    {
        public string Name => "stub";
    }
}
CS
      insert_before_closing_project \
        "${SANDBOX}/src/Arkus.Game.World/Arkus.Game.World.csproj" \
        '  <ItemGroup>
    <ProjectReference Include="../UnityEngine/UnityEngine.csproj" />
  </ItemGroup>'
      write_file "${SANDBOX}/src/Arkus.Game.World/EngineBridge.cs" <<'CS'
using UnityEngine;

namespace Arkus.Game.World
{
    public static class EngineBridge
    {
        public static string DescribeEngineObject()
        {
            return new GameObject().Name;
        }
    }
}
CS
      ;;
    dependency-cycle)
      insert_before_closing_project \
        "${SANDBOX}/src/Arkus.Game.Core/Arkus.Game.Core.csproj" \
        '  <ItemGroup>
    <ProjectReference Include="../Arkus.Game.World/Arkus.Game.World.csproj" />
  </ItemGroup>'
      ;;
    layering-back-edge)
      insert_before_closing_project \
        "${SANDBOX}/src/Arkus.Harness.Runtime/Arkus.Harness.Runtime.csproj" \
        '  <ItemGroup>
    <ProjectReference Include="../Arkus.Game.Core/Arkus.Game.Core.csproj" />
  </ItemGroup>'
      write_file "${SANDBOX}/src/Arkus.Harness.Runtime/CoreBridge.cs" <<'CS'
using Arkus.Game.Core;

namespace Arkus.Harness.Runtime
{
    public static class CoreBridge
    {
        public static string CoreName => CoreModule.Name;
    }
}
CS
      ;;
    duplicate-source-ownership)
      write_file "${SANDBOX}/src/Arkus.Game.Core/SharedHelper.cs" <<'CS'
namespace Arkus.Game.Core
{
    public static class SharedHelper
    {
        public static string Name => "shared";
    }
}
CS
      insert_before_closing_project \
        "${SANDBOX}/src/Arkus.Game.World/Arkus.Game.World.csproj" \
        '  <ItemGroup>
    <Compile Include="../Arkus.Game.Core/SharedHelper.cs" />
  </ItemGroup>'
      ;;
    duplicate-source-type-conflict)
      insert_before_closing_project \
        "${SANDBOX}/src/Arkus.Game.Authoring/Arkus.Game.Authoring.csproj" \
        '  <ItemGroup>
    <Compile Include="../Arkus.Game.Validation/ValidationModule.cs" />
  </ItemGroup>'
      ;;
    unclassified-production-source)
      write_file "${SANDBOX}/src/Orphan/OrphanType.cs" <<'CS'
namespace Orphan
{
    public static class OrphanType
    {
        public static string Name => "orphan";
    }
}
CS
      ;;
    source-excluded-from-compilation)
      write_file "${SANDBOX}/src/Arkus.Game.World/Detached.cs" <<'CS'
namespace Arkus.Game.World
{
    public static class Detached
    {
        public static string Name => "detached";
    }
}
CS
      insert_before_closing_project \
        "${SANDBOX}/src/Arkus.Game.World/Arkus.Game.World.csproj" \
        '  <ItemGroup>
    <Compile Remove="Detached.cs" />
  </ItemGroup>'
      ;;
    source-excluded-by-build-target)
      write_file "${SANDBOX}/src/Arkus.Game.World/Detached.cs" <<'CS'
namespace Arkus.Game.World
{
    public static class Detached
    {
        public static string Name => "detached";
    }
}
CS
      insert_before_closing_project \
        "${SANDBOX}/src/Arkus.Game.World/Arkus.Game.World.csproj" \
        '  <Target Name="DropSourceAfterEvaluation" BeforeTargets="CoreCompile">
    <ItemGroup>
      <Compile Remove="Detached.cs" />
    </ItemGroup>
  </Target>'
      ;;
    toolchain-property-drift)
      insert_before_closing_project \
        "${SANDBOX}/src/Arkus.Game.Core/Arkus.Game.Core.csproj" \
        '  <PropertyGroup>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>'
      ;;
    toolchain-option-mutated-at-build-time)
      insert_before_closing_project \
        "${SANDBOX}/src/Arkus.Game.Core/Arkus.Game.Core.csproj" \
        '  <Target Name="RelaxWarningsAfterEvaluation" BeforeTargets="CoreCompile">
    <PropertyGroup>
      <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
    </PropertyGroup>
  </Target>'
      ;;
    hidden-generated-source)
      insert_before_closing_project \
        "${SANDBOX}/src/Arkus.Game.World/Arkus.Game.World.csproj" \
        '  <Target Name="EmitHiddenSource" BeforeTargets="CoreCompile">
    <WriteLinesToFile File="$(IntermediateOutputPath)Hidden.g.cs" Overwrite="true" Lines="namespace Arkus.Game.World { public static class Hidden { } }" />
    <ItemGroup>
      <Compile Include="$(IntermediateOutputPath)Hidden.g.cs" />
    </ItemGroup>
  </Target>'
      ;;
    toolchain-pin-loosened)
      write_file "${SANDBOX}/global.json" <<'JSON'
{
  "sdk": {
    "version": "8.0.100",
    "rollForward": "latestMajor",
    "allowPrerelease": false
  }
}
JSON
      ;;
    warnings-as-errors-neutralised)
      insert_before_closing_project \
        "${SANDBOX}/src/Arkus.Game.Core/Arkus.Game.Core.csproj" \
        '  <PropertyGroup>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
  </PropertyGroup>'
      ;;
    warning-suppression-added)
      insert_before_closing_project \
        "${SANDBOX}/src/Arkus.Game.Core/Arkus.Game.Core.csproj" \
        '  <PropertyGroup>
    <NoWarn>$(NoWarn);CS0219</NoWarn>
  </PropertyGroup>'
      ;;
    effective-warning-as-error)
      write_file "${SANDBOX}/src/Arkus.Game.Core/SloppyCode.cs" <<'CS'
namespace Arkus.Game.Core
{
    public static class SloppyCode
    {
        public static string Name()
        {
            int unused = 42;
            return "sloppy";
        }
    }
}
CS
      ;;
    required-project-deleted)
      rm -f "${SANDBOX}/src/Arkus.Harness.Cli/Arkus.Harness.Cli.csproj"
      ;;
    proof-tool-deleted)
      rm -f "${SANDBOX}/tools/Arkus.Kernel.Proof.Tool/Arkus.Kernel.Proof.Tool.csproj"
      ;;
    package-pin-bypassed)
      python3 - "${SANDBOX}/tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj" <<'PY'
import sys
path = sys.argv[1]
text = open(path).read()
old = '<PackageReference Include="xunit" />'
assert old in text, path
open(path, "w").write(text.replace(old, '<PackageReference Include="xunit" Version="2.9.2" />', 1))
PY
      ;;
    forbidden-engine-package)
      insert_before_closing_project \
        "${SANDBOX}/src/Arkus.Game.Core/Arkus.Game.Core.csproj" \
        '  <ItemGroup>
    <PackageReference Include="UnityEngine.Modules" />
  </ItemGroup>'
      ;;
  esac
}

run_attack() {
  local name="$1"
  local logfile="${EVIDENCE}/${name}.log"
  mkdir -p "${EVIDENCE}"

  configure_attack "${name}"

  {
    printf '# self-attack: %s\n' "${name}"
    printf '# %s\n' "${ATTACK_DESCRIPTION}"
    printf '# configuration: %s\n' "${CONFIGURATION}"
    printf '# touched paths: %s\n' "${TOUCHED[*]:-none}"
    printf '\n--- 1. inject defect ---\n'
  } >"${logfile}"

  apply_attack "${name}"

  if [[ "${ATTACK_BUILD}" == "yes" ]]; then
    printf '\n--- 2. build with defect (expected to %s) ---\n' "${ATTACK_BUILD_EXPECT}" >>"${logfile}"
    if sandbox_build "${logfile}"; then
      if [[ "${ATTACK_BUILD_EXPECT}" == "fail" ]]; then
        fail "${name}: build succeeded but the defect had to break it"
      fi
    else
      if [[ "${ATTACK_BUILD_EXPECT}" != "fail" ]]; then
        fail "${name}: build failed for an unrelated reason; see ${logfile}"
      fi
    fi

    if [[ -n "${ATTACK_BUILD_GREP}" ]] && ! grep -q -- "${ATTACK_BUILD_GREP}" "${logfile}"; then
      fail "${name}: build output does not contain the expected reason '${ATTACK_BUILD_GREP}'"
    fi
  fi

  if [[ "${ATTACK_PHASE}" != "none" ]]; then
    printf '\n--- 3. guard must turn red for the intended reason ---\n' >>"${logfile}"
    local proof_args=(--phase "${ATTACK_PHASE}")
    local check
    for check in ${ATTACK_EXPECT}; do
      proof_args+=(--expect "${check}")
    done
    for check in ${ATTACK_EXPECT_ABSENT}; do
      proof_args+=(--expect-absent "${check}")
    done

    if ! run_proof "${logfile}" "${proof_args[@]}"; then
      fail "${name}: the intended check did not fire; see ${logfile}"
    fi
  fi

  printf '\n--- 4. revert defect ---\n' >>"${logfile}"
  revert_attack
  prove_revert_is_exact "${logfile}"

  printf '\n--- 5. rebuild and prove green again ---\n' >>"${logfile}"
  if ! sandbox_build "${logfile}"; then
    fail "${name}: the sandbox does not build again after revert"
  fi

  if ! run_proof "${logfile}" --phase all; then
    fail "${name}: the sandbox is not green again after revert; see ${logfile}"
  fi

  printf '\n--- result: RED on injection, GREEN after revert ---\n' >>"${logfile}"

  sanitize_log "${logfile}"

  log "PASS  ${name}"
}

main() {
  local selected=("$@")
  if [[ ${#selected[@]} -eq 0 ]]; then
    selected=("${ATTACKS[@]}")
  fi

  if [[ ! -f "${PROOF_TOOL_DLL}" ]]; then
    fail "proof tool not built at ${PROOF_TOOL_DLL}; run scripts/proof.sh first"
  fi

  mkdir -p "${WORK}" "${EVIDENCE}"
  log "sandbox root: ${WORK}"

  copy_tree "${ROOT}" "${PRISTINE}"
  copy_tree "${PRISTINE}" "${SANDBOX}"

  local baseline="${EVIDENCE}/baseline.log"
  printf '# baseline: pristine sandbox must build and prove green before any attack\n' >"${baseline}"
  if ! sandbox_build "${baseline}"; then
    fail "pristine sandbox does not build"
  fi
  if ! run_proof "${baseline}" --phase all; then
    fail "pristine sandbox is not green; attacks would prove nothing"
  fi
  sanitize_log "${baseline}"
  log "PASS  baseline (pristine sandbox is green)"

  local name
  for name in "${selected[@]}"; do
    run_attack "${name}"
  done

  {
    printf '# WP-HK-00 self-attack results\n\n'
    printf 'Generated by `scripts/self-attacks/run-self-attacks.sh`.\n\n'
    printf 'Every row was injected into a sandbox copy, proved red for the named check,\n'
    printf 'reverted to a byte-identical tree, rebuilt, and proved green again.\n\n'
    printf '| Attack | Defect | Phase | Checks required to fire | Checks required to stay quiet | Result |\n'
    printf '|---|---|---|---|---|---|\n'
    for name in "${selected[@]}"; do
      configure_attack "${name}"
      local expect="${ATTACK_EXPECT:-build must fail}"
      local quiet="${ATTACK_EXPECT_ABSENT:-—}"
      printf '| `%s` | %s | %s | `%s` | `%s` | RED then GREEN |\n' \
        "${name}" "${ATTACK_DESCRIPTION}" "${ATTACK_PHASE}" "${expect}" "${quiet}"
    done
  } >"${RESULTS}"

  log ""
  log "all ${#selected[@]} self-attacks passed; evidence in ${EVIDENCE}"
}

main "$@"
