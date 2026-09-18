#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
CONFIGURATION="${CONFIGURATION:-Release}"
WORK="${ROOT}/artifacts/self-attacks"
PRISTINE="${WORK}/closure-pristine"
SANDBOX="${WORK}/closure-sandbox"
LOGDIR="${WORK}/logs"
RESULTS="${ROOT}/Docs/evidence/WP-HK-00/self-attacks/results.md"
TOOL_DLL="${ROOT}/tools/Arkus.HK00.Proof/bin/${CONFIGURATION}/net8.0/Arkus.HK00.Proof.dll"

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
}

rm -rf "${PRISTINE}" "${SANDBOX}"
copy_clean_tree "${ROOT}" "${PRISTINE}"

fresh
expect_green repository "${LOGDIR}/closure-baseline-repository.log"
expect_green static "${LOGDIR}/closure-baseline-static.log"

fresh
(cd "${SANDBOX}" && dotnet sln Juego2.sln remove src/Arkus.Harness.Cli/Arkus.Harness.Cli.csproj >/dev/null)
expect_red repository HK00-SOLUTION-MISSING-PROJECT "${LOGDIR}/solution-omits-fixed-project-red.log"
fresh
expect_green repository "${LOGDIR}/solution-omits-fixed-project-green.log"
record solution-omits-fixed-project HK00-SOLUTION-MISSING-PROJECT

fresh
cat >"${SANDBOX}/Directory.Build.targets" <<'XML'
<Project>
  <Target Name="HiddenRepositoryBuildExtension" BeforeTargets="BeforeBuild" />
</Project>
XML
git -C "${SANDBOX}" add Directory.Build.targets
expect_red static HK00-MSBUILD-IMPORT-UNTRUSTED "${LOGDIR}/auto-directory-build-target-red.log"
fresh
expect_green static "${LOGDIR}/auto-directory-build-target-green.log"
record auto-directory-build-target HK00-MSBUILD-IMPORT-UNTRUSTED

echo "all 2 closure attacks passed"
