#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT}"
CANDIDATE="$(git rev-parse HEAD)"
[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "HK GATE negative conformance requires a clean candidate" >&2; exit 2; }

TMP_ROOT="$(mktemp -d)"
WORKTREE="${TMP_ROOT}/mutant"
LOG="${TMP_ROOT}/gate-step-omission.log"
cleanup() {
  git worktree remove --force "${WORKTREE}" >/dev/null 2>&1 || true
  rm -rf "${TMP_ROOT}"
}
trap cleanup EXIT

git worktree add --detach "${WORKTREE}" "${CANDIDATE}" >/dev/null
cd "${WORKTREE}"
DOTNET_NOLOGO=1 dotnet restore Juego2.sln --locked-mode -m:1 --disable-build-servers >/dev/null

python3 - <<'PY'
from pathlib import Path
path = Path("tests/Arkus.Harness.Tests/HkGateReadinessTests.cs")
text = path.read_text(encoding="utf-8")
needle = '            "14-headless-full-validation"\n'
if text.count(needle) != 1:
    raise SystemExit("HK GATE seeded-defect target missing or ambiguous")
path.write_text(text.replace(needle, '', 1), encoding="utf-8")
PY

set +e
DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
  --configuration Release --no-restore -m:1 --disable-build-servers \
  --filter 'FullyQualifiedName~HkGateReadinessTests.GateStepUniverseIsExplicitAndCannotSilentlyOmitARequiredScenarioStage' \
  >"${LOG}" 2>&1
status=$?
set -e

if grep -Eq 'error (CS|MSB|NU)[0-9]+' "${LOG}"; then
  echo "HK GATE negative control: infrastructure/build failure instead of causal test RED" >&2
  cat "${LOG}" >&2
  exit 2
fi
if [[ ${status} -eq 0 ]] || ! grep -Fq 'Failed!' "${LOG}"; then
  echo "HK GATE negative control: FALSE GREEN for omitted mandatory scenario stage" >&2
  cat "${LOG}" >&2
  exit 1
fi
if ! grep -Fq 'Expected: 14' "${LOG}"; then
  echo "HK GATE negative control: test failed for the wrong reason" >&2
  cat "${LOG}" >&2
  exit 2
fi

echo 'HK_GATE_NEGATIVE_RED mandatory-scenario-stage-omission'
cd "${ROOT}"
[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "HK GATE candidate changed during disposable negative control" >&2; exit 2; }
echo 'HK_GATE_NEGATIVE_CONFORMANCE GREEN red_controls=1'
