#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${repo_root}"

usage() {
  cat <<'EOF'
Usage:
  scripts/mutation-preflight.sh <source-project.csproj> <test-project.csproj> <mutate-glob> [<mutate-glob> ...]

Example:
  scripts/mutation-preflight.sh \
    src/Arkus.DesignWorld/Arkus.DesignWorld.csproj \
    tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
    '**/City*.cs' '**/DesignWorldValidator.cs'

This runner is advisory: surviving mutants are evidence for Worker pre-review, not an automatic WP verdict.
It is executor-neutral and may run in any capable remote or local Worker checkout.
EOF
}

if [[ $# -lt 3 ]]; then
  usage >&2
  exit 2
fi

source_project="$1"
test_project="$2"
shift 2

[[ -f "${source_project}" ]] || { echo "error: source project not found: ${source_project}" >&2; exit 3; }
[[ -f "${test_project}" ]] || { echo "error: test project not found: ${test_project}" >&2; exit 3; }

if ! command -v dotnet >/dev/null 2>&1; then
  echo "error: dotnet is required; run scripts/worker-preflight.sh first" >&2
  exit 4
fi

required_sdk="$(python3 - <<'PY'
import json
from pathlib import Path
print(json.loads(Path('global.json').read_text())['sdk']['version'])
PY
)"
actual_sdk="$(dotnet --version)"
[[ "${actual_sdk}" == "${required_sdk}" ]] || {
  echo "error: required SDK ${required_sdk}; actual ${actual_sdk}" >&2
  exit 5
}

echo "[mutation] restoring pinned local tools"
dotnet tool restore

source_dir="$(cd "$(dirname "${source_project}")" && pwd)"
source_name="$(basename "${source_project}")"
test_abs="$(cd "$(dirname "${test_project}")" && pwd)/$(basename "${test_project}")"

args=(
  --project "${source_name}"
  --test-project "${test_abs}"
  --configuration Release
  --reporter progress
  --reporter json
  --output "${repo_root}/artifacts/stryker"
)
for pattern in "$@"; do
  args+=(--mutate "${pattern}")
done

cat <<EOF
[mutation] project: ${source_project}
[mutation] tests:   ${test_project}
[mutation] mode:    advisory; inspect every surviving mutant in the touched semantic surface
EOF

(
  cd "${source_dir}"
  dotnet stryker "${args[@]}"
)

echo "MUTATION_PREFLIGHT_COMPLETE"
