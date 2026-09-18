#!/usr/bin/env bash
#
# WP-HK-00 causal completeness attacks added after the independent review found
# that manifest-owned sourceScanRoots could shrink the universe being proved.
#
# These attacks target omission classes specifically: project/source outside the
# old roots, and source hidden below a recursively excluded directory name.
# Each attack must turn the intended guard red, then revert byte-exactly and
# prove green again.
#
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
CONFIGURATION="${CONFIGURATION:-Release}"
WORK="${COMPLETENESS_ATTACK_WORKDIR:-${ROOT}/artifacts/completeness-attacks}"
PRISTINE="${WORK}/pristine"
SANDBOX="${WORK}/sandbox"
EVIDENCE="${ROOT}/Docs/evidence/WP-HK-00/self-attacks"
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
    fail "sandbox is not byte-identical to pristine after revert"
  fi
}

run_attack() {
  local name="$1" description="$2" expected="$3"
  local logfile="${EVIDENCE}/${name}.log"

  copy_tree "${PRISTINE}" "${SANDBOX}"
  {
    printf '# self-attack: %s\n' "${name}"
    printf '# %s\n' "${description}"
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
    *)
      fail "unknown attack ${name}"
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
  printf 'PASS  %s\n' "${name}"
}

main() {
  if [[ ! -f "${PROOF_TOOL_DLL}" ]]; then
    fail "proof tool not built at ${PROOF_TOOL_DLL}; build it first"
  fi

  mkdir -p "${WORK}" "${EVIDENCE}"
  copy_tree "${ROOT}" "${PRISTINE}"
  copy_tree "${PRISTINE}" "${SANDBOX}"

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

  run_attack \
    project-outside-legacy-roots \
    "A C# project and source are added outside the former src/tests/tools scan roots." \
    "HK00-MANIFEST-PROJECT-UNDECLARED HK00-SOURCE-UNCLASSIFIED"

  run_attack \
    source-outside-legacy-roots \
    "A production-looking C# source is added under Docs, outside the former scan roots." \
    "HK00-SOURCE-UNCLASSIFIED"

  run_attack \
    nested-excluded-directory-source \
    "Owned source is hidden below a nested obj directory; recursive exclusion must not hide it." \
    "HK00-SOURCE-NOT-COMPILED-STATIC"

  printf 'all 3 completeness self-attacks passed\n'
}

main "$@"
