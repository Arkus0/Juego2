#!/usr/bin/env bash
#
# WP-HK-00 causal attacks for repository/proof-boundary completeness.
#
# The original Reviewer failure exposed a self-shrinkable proof universe. These
# attacks cover that omission class and equivalent ways to hide projects/sources
# from a Linux proof: arbitrary paths, nested excluded-name directories,
# extension casing, excluded root-level artifacts, symlinks, gitlinks, sparse
# worktrees and untracked files.
#
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
CONFIGURATION="${CONFIGURATION:-Release}"
WORK="${COMPLETENESS_ATTACK_WORKDIR:-${ROOT}/artifacts/completeness-attacks}"
PRISTINE="${WORK}/pristine"
SANDBOX="${WORK}/sandbox"
EVIDENCE="${ROOT}/Docs/evidence/WP-HK-00/self-attacks"
RESULTS="${EVIDENCE}/completeness-results.md"
PROOF_TOOL_DLL="${ROOT}/artifacts/bin/Arkus.Kernel.Proof.Tool/${CONFIGURATION,,}/Arkus.Kernel.Proof.Tool.dll"

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
export MSBUILDDISABLENODEREUSE=1

fail() {
  printf 'COMPLETENESS SELF-ATTACK FAILURE: %s\n' "$*" >&2
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

sanitize_log() {
  python3 - "$1" "${SANDBOX}" "${ROOT}" <<'PY'
import sys
path, sandbox, root = sys.argv[1], sys.argv[2], sys.argv[3]
text = open(path).read().replace(sandbox, "<SANDBOX>").replace(root, "<REPO>")
open(path, "w").write(text)
PY
}

revert_attack() {
  tar -C "${PRISTINE}" -cf - . | tar -C "${SANDBOX}" -xmf -

  while IFS= read -r rel; do
    if [[ ! -e "${PRISTINE}/${rel}" ]]; then
      rm -f "${SANDBOX}/${rel}"
    fi
  done < <(cd "${SANDBOX}" && find . -path ./artifacts -prune -o -path ./.git -prune -o -type f -print)

  while IFS= read -r rel; do
    if [[ "${rel}" != "." && ! -d "${PRISTINE}/${rel}" ]]; then
      rmdir "${SANDBOX}/${rel}" 2>/dev/null || true
    fi
  done < <(cd "${SANDBOX}" && find . -path ./artifacts -prune -o -path ./.git -prune -o -type d -print | sort -r)
}

prove_revert_is_exact() {
  local logfile="$1"
  if ! diff -r --exclude=artifacts --exclude=.git "${PRISTINE}" "${SANDBOX}" >>"${logfile}" 2>&1; then
    fail "sandbox is not byte-identical to pristine after revert"
  fi
}

record_pass() {
  local name="$1" oracle="$2"
  printf '| `%s` | %s | PASS |\n' "${name}" "${oracle}" >>"${RESULTS}"
  printf 'PASS  %s\n' "${name}"
}

run_proof_attack() {
  local name="$1" description="$2" expected="$3"
  local logfile="${EVIDENCE}/${name}.log"

  copy_tree "${PRISTINE}" "${SANDBOX}"
  {
    printf '# self-attack: %s\n' "${name}"
    printf '# %s\n' "${description}"
    printf '# oracle: C# kernel proof\n'
    printf '# configuration: %s\n' "${CONFIGURATION}"
    printf '\n--- 1. inject defect ---\n'
  } >"${logfile}"

  case "${name}" in
    project-outside-legacy-roots)
      mkdir -p "${SANDBOX}/outside-old-roots/Rogue"
      cat >"${SANDBOX}/outside-old-roots/Rogue/Rogue.csproj" <<'XML'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>
XML
      cat >"${SANDBOX}/outside-old-roots/Rogue/Rogue.cs" <<'CS'
namespace Rogue
{
    public static class Marker { }
}
CS
      ;;
    source-outside-legacy-roots)
      mkdir -p "${SANDBOX}/Docs/rogue-source"
      cat >"${SANDBOX}/Docs/rogue-source/Orphan.cs" <<'CS'
namespace RogueDocs
{
    public static class Orphan { }
}
CS
      ;;
    nested-excluded-directory-source)
      mkdir -p "${SANDBOX}/src/Arkus.Game.Core/obj"
      cat >"${SANDBOX}/src/Arkus.Game.Core/obj/Hidden.cs" <<'CS'
namespace Arkus.Game.Core
{
    public static class HiddenUnderObj { }
}
CS
      ;;
    uppercase-source-extension)
      mkdir -p "${SANDBOX}/Docs/rogue-source"
      cat >"${SANDBOX}/Docs/rogue-source/Orphan.CS" <<'CS'
namespace RogueCase
{
    public static class Orphan { }
}
CS
      ;;
    uppercase-project-extension)
      mkdir -p "${SANDBOX}/outside-old-roots/CaseProject"
      cat >"${SANDBOX}/outside-old-roots/CaseProject/CaseProject.CSPROJ" <<'XML'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>
XML
      ;;
    *)
      fail "unknown proof attack ${name}"
      ;;
  esac

  printf '\n--- 2. guard must turn red for the intended reason ---\n' >>"${logfile}"
  local args=(--phase static)
  local check
  for check in ${expected}; do
    args+=(--expect "${check}")
  done
  if ! run_proof "${logfile}" "${args[@]}"; then
    fail "${name}: intended completeness guard did not fire; see ${logfile}"
  fi

  printf '\n--- 3. revert defect byte-exactly ---\n' >>"${logfile}"
  revert_attack
  prove_revert_is_exact "${logfile}"

  printf '\n--- 4. rebuild and prove green again ---\n' >>"${logfile}"
  if ! sandbox_build "${logfile}"; then
    fail "${name}: sandbox does not build after revert"
  fi
  if ! run_proof "${logfile}" --phase all; then
    fail "${name}: sandbox does not prove green after revert"
  fi

  printf '\n--- result: RED on injection, GREEN after revert ---\n' >>"${logfile}"
  sanitize_log "${logfile}"
  record_pass "${name}" "C# proof"
}

init_boundary_git() {
  rm -rf "${SANDBOX}/.git"
  git -C "${SANDBOX}" init -q
  git -C "${SANDBOX}" config user.name "HK00 Self Attack"
  git -C "${SANDBOX}" config user.email "hk00-self-attack@example.invalid"
  git -C "${SANDBOX}" add -A
}

run_boundary_oracle() {
  local logfile="$1"
  set +e
  (cd "${SANDBOX}" && bash scripts/proof-repository-boundary.sh) >>"${logfile}" 2>&1
  local code=$?
  set -e
  return ${code}
}

run_boundary_attack() {
  local name="$1" description="$2" expected="$3"
  local logfile="${EVIDENCE}/${name}.log"

  copy_tree "${PRISTINE}" "${SANDBOX}"
  init_boundary_git
  {
    printf '# self-attack: %s\n' "${name}"
    printf '# %s\n' "${description}"
    printf '# oracle: independent Git/checkout boundary oracle\n'
    printf '\n--- 1. pristine oracle must be green ---\n'
  } >"${logfile}"

  if ! run_boundary_oracle "${logfile}"; then
    fail "${name}: pristine boundary oracle is not green"
  fi

  printf '\n--- 2. inject defect ---\n' >>"${logfile}"
  case "${name}" in
    tracked-source-under-artifacts)
      mkdir -p "${SANDBOX}/artifacts"
      printf 'namespace RogueArtifacts { public static class Smuggled { } }\n' >"${SANDBOX}/artifacts/Smuggled.cs"
      git -C "${SANDBOX}" add -f artifacts/Smuggled.cs
      ;;
    tracked-symlink)
      ln -s "src/Arkus.Game.Core/CoreModule.cs" "${SANDBOX}/LinkedCore.cs"
      git -C "${SANDBOX}" add LinkedCore.cs
      ;;
    tracked-gitlink)
      tree="$(git -C "${SANDBOX}" write-tree)"
      commit="$(printf 'baseline\n' | git -C "${SANDBOX}" commit-tree "${tree}")"
      git -C "${SANDBOX}" update-index --add --cacheinfo "160000,${commit},ExternalKernel"
      ;;
    tracked-source-missing)
      rm "${SANDBOX}/tools/Arkus.Kernel.Proof/RepositoryPaths.cs"
      ;;
    untracked-source-in-checkout)
      mkdir -p "${SANDBOX}/loose"
      printf 'namespace Loose { public static class Untracked { } }\n' >"${SANDBOX}/loose/Untracked.cs"
      ;;
    boundary-uppercase-source)
      printf 'namespace RogueCase { public static class Upper { } }\n' >"${SANDBOX}/Upper.CS"
      git -C "${SANDBOX}" add Upper.CS
      ;;
    *)
      fail "unknown boundary attack ${name}"
      ;;
  esac

  printf '\n--- 3. oracle must turn red for the intended reason ---\n' >>"${logfile}"
  set +e
  (cd "${SANDBOX}" && bash scripts/proof-repository-boundary.sh) >>"${logfile}" 2>&1
  local code=$?
  set -e
  if [[ ${code} -eq 0 ]]; then
    fail "${name}: boundary oracle stayed green"
  fi
  if ! grep -Fq "${expected}" "${logfile}"; then
    fail "${name}: expected ${expected} was not reported"
  fi

  printf '\n--- 4. revert defect byte-exactly and re-prove green ---\n' >>"${logfile}"
  rm -rf "${SANDBOX}/.git"
  revert_attack
  prove_revert_is_exact "${logfile}"
  init_boundary_git
  if ! run_boundary_oracle "${logfile}"; then
    fail "${name}: boundary oracle is not green after revert"
  fi

  printf '\n--- result: RED on injection, GREEN after revert ---\n' >>"${logfile}"
  sanitize_log "${logfile}"
  record_pass "${name}" "independent boundary oracle"
}

main() {
  if [[ ! -f "${PROOF_TOOL_DLL}" ]]; then
    fail "proof tool not built at ${PROOF_TOOL_DLL}; build it first"
  fi

  mkdir -p "${WORK}" "${EVIDENCE}"
  copy_tree "${ROOT}" "${PRISTINE}"
  copy_tree "${PRISTINE}" "${SANDBOX}"

  cat >"${RESULTS}" <<'MD'
# WP-HK-00 repository-completeness causal attacks

| Attack | Oracle | Result |
|---|---|---|
MD

  local baseline="${EVIDENCE}/completeness-baseline.log"
  printf '# completeness baseline: pristine sandbox must build and prove green\n' >"${baseline}"
  if ! sandbox_build "${baseline}"; then
    fail "pristine completeness sandbox does not build"
  fi
  if ! run_proof "${baseline}" --phase all; then
    fail "pristine completeness sandbox is not green"
  fi
  sanitize_log "${baseline}"
  printf 'PASS  completeness baseline\n'

  run_proof_attack \
    project-outside-legacy-roots \
    "A C# project and source are added outside the former src/tests/tools scan roots." \
    "HK00-MANIFEST-PROJECT-UNDECLARED HK00-SOURCE-UNCLASSIFIED"

  run_proof_attack \
    source-outside-legacy-roots \
    "A C# source is added under Docs, outside the former scan roots." \
    "HK00-SOURCE-UNCLASSIFIED"

  run_proof_attack \
    nested-excluded-directory-source \
    "Owned source is hidden below a nested obj directory; recursive exclusion must not hide it." \
    "HK00-SOURCE-NOT-COMPILED-STATIC"

  run_proof_attack \
    uppercase-source-extension \
    "A source uses .CS casing that Linux globbing could otherwise omit." \
    "HK00-SOURCE-UNCLASSIFIED"

  run_proof_attack \
    uppercase-project-extension \
    "A project uses .CSPROJ casing that Linux globbing could otherwise omit." \
    "HK00-MANIFEST-PROJECT-UNDECLARED"

  run_boundary_attack \
    tracked-source-under-artifacts \
    "A tracked C# file is placed under the root artifacts workspace excluded from the C# inventory." \
    "HK00-BOUNDARY-TRACKED-ARTIFACT"

  run_boundary_attack \
    tracked-symlink \
    "A tracked source-like path is a symlink rather than owned bytes in the candidate commit." \
    "HK00-BOUNDARY-SYMLINK"

  run_boundary_attack \
    tracked-gitlink \
    "A tracked gitlink/submodule represents an external tree that the checkout proof would not own." \
    "HK00-BOUNDARY-GITLINK"

  run_boundary_attack \
    tracked-source-missing \
    "A tracked C# source is absent from the physical checkout, as in a sparse/incomplete worktree." \
    "HK00-BOUNDARY-TRACKED-SOURCE-MISSING"

  run_boundary_attack \
    untracked-source-in-checkout \
    "A physical C# source exists in the checkout but is absent from the candidate commit." \
    "HK00-BOUNDARY-UNTRACKED-SOURCE"

  run_boundary_attack \
    boundary-uppercase-source \
    "A tracked source uses non-canonical extension casing; the independent oracle must reject it too." \
    "HK00-BOUNDARY-NONCANONICAL-CASE"

  printf 'all 11 repository-completeness self-attacks passed\n'
}

main "$@"
