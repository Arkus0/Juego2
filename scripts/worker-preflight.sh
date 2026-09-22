#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${repo_root}"

if [[ ! -f global.json ]]; then
  echo "error: global.json not found" >&2
  exit 2
fi

required_sdk="$(python3 - <<'PY'
import json
from pathlib import Path
print(json.loads(Path('global.json').read_text())['sdk']['version'])
PY
)"

if ! command -v dotnet >/dev/null 2>&1; then
  cat >&2 <<EOF
error: dotnet is not available.
Required SDK: ${required_sdk}
Install/allow the SDK declared by global.json in this Worker environment before handing off a candidate.
EOF
  exit 3
fi

actual_sdk="$(dotnet --version)"
if [[ "${actual_sdk}" != "${required_sdk}" ]]; then
  cat >&2 <<EOF
error: wrong .NET SDK.
Required: ${required_sdk}
Actual:   ${actual_sdk}
global.json uses rollForward=disable; install the exact SDK in this Worker environment.
EOF
  exit 4
fi

echo "[preflight] SDK ${actual_sdk}"
echo "[preflight] restore --locked-mode"
dotnet restore Juego2.sln --locked-mode

echo "[preflight] build Release --no-restore"
dotnet build Juego2.sln -c Release --no-restore

echo "[preflight] test Release --no-build --no-restore"
dotnet test Juego2.sln -c Release --no-build --no-restore --logger "console;verbosity=minimal"

echo "[preflight] Python syntax checks (no bytecode writes)"
python3 - <<'PY'
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
  exit 5
fi

echo "WORKER_PREFLIGHT_GREEN"
