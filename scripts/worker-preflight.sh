#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${repo_root}"

if [[ ! -f global.json ]]; then
  echo "error: global.json not found" >&2
  exit 2
fi

candidate_sha="$(git rev-parse HEAD)"
if [[ ! "${candidate_sha}" =~ ^[0-9a-fA-F]{40}$ ]]; then
  echo "error: current HEAD is not an exact 40-character commit SHA" >&2
  exit 3
fi

if [[ -n "$(git status --porcelain --untracked-files=all)" ]]; then
  git status --short
  echo "error: Worker preflight must start from a clean exact candidate HEAD" >&2
  exit 4
fi

echo "[preflight] candidate ${candidate_sha}"

if [[ -f scripts/validate-main-safety-trigger.py ]]; then
  echo "[preflight] Main Safety PR coverage guard"
  PYTHONDONTWRITEBYTECODE=1 python3 scripts/validate-main-safety-trigger.py --self-test
  PYTHONDONTWRITEBYTECODE=1 python3 scripts/validate-main-safety-trigger.py
fi

if [[ -f scripts/validate-worker-preflight-context.py ]]; then
  echo "[preflight] delegated-context negative controls"
  PYTHONDONTWRITEBYTECODE=1 python3 scripts/validate-worker-preflight-context.py --self-test
fi

required_sdk="$(python3 - <<'PY'
import json
from pathlib import Path
print(json.loads(Path('global.json').read_text(encoding='utf-8'))['sdk']['version'])
PY
)"

if ! command -v dotnet >/dev/null 2>&1; then
  cat >&2 <<EOF
Worker environment cannot execute the local preflight because dotnet is unavailable.
Required SDK: ${required_sdk}
Candidate SHA: ${candidate_sha}
WORKER_PREFLIGHT_DELEGATION_REQUIRED
Use a GREEN 'Worker Candidate Preflight' pull_request run whose durable receipt names the canonical PR and this exact SHA. Do not treat this local capability miss as NOT_READY by itself.
EOF
  exit 10
fi

actual_sdk="$(dotnet --version)"
if [[ "${actual_sdk}" != "${required_sdk}" ]]; then
  cat >&2 <<EOF
Worker environment cannot execute the local preflight with the exact SDK selected by global.json.
Required SDK: ${required_sdk}
Actual SDK:   ${actual_sdk}
Candidate SHA: ${candidate_sha}
WORKER_PREFLIGHT_DELEGATION_REQUIRED
Use a GREEN 'Worker Candidate Preflight' pull_request run whose durable receipt names the canonical PR and this exact SHA. Do not reuse a run from another PR or candidate.
EOF
  exit 10
fi

echo "[preflight] SDK ${actual_sdk}"
# This repository does not currently commit packages.lock.json files, so --locked-mode
# would fail rather than make restore more deterministic. Adopt locked restore only together
# with reviewed lockfiles for the full solution.
echo "[preflight] restore"
dotnet restore Juego2.sln

echo "[preflight] build Release --no-restore"
dotnet build Juego2.sln -c Release --no-restore

echo "[preflight] test Release --no-build --no-restore"
dotnet test Juego2.sln -c Release --no-build --no-restore --logger "console;verbosity=minimal"

echo "[preflight] Python syntax checks (no bytecode writes)"
PYTHONDONTWRITEBYTECODE=1 python3 - <<'PY'
from pathlib import Path
for path in sorted(Path('scripts').glob('*.py')):
    compile(path.read_text(encoding='utf-8'), str(path), 'exec')
PY

if [[ -f scripts/validate-worker-handoff.py ]]; then
  echo "[preflight] handoff linter self-test"
  PYTHONDONTWRITEBYTECODE=1 python3 scripts/validate-worker-handoff.py --self-test
fi

if [[ -f scripts/validation-context.py ]]; then
  echo "[preflight] validation-context self-test"
  PYTHONDONTWRITEBYTECODE=1 python3 scripts/validation-context.py self-test
fi

echo "[preflight] clean-tree check"
if [[ -n "$(git status --porcelain --untracked-files=all)" ]]; then
  git status --short
  echo "error: preflight left or found an unclean working tree" >&2
  exit 7
fi

if [[ "$(git rev-parse HEAD)" != "${candidate_sha}" ]]; then
  echo "error: candidate HEAD changed during preflight" >&2
  exit 8
fi

printf 'Candidate SHA: %s\n' "${candidate_sha}"
echo "WORKER_PREFLIGHT_GREEN"
