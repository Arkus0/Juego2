#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"
cd "${ROOT}"

candidate_dirty_status() {
  git status --porcelain --untracked-files=all | \
    grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true
}

actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }
[[ -z "$(candidate_dirty_status)" ]] || { echo "Candidate is not clean before H1-02 observation" >&2; candidate_dirty_status >&2; exit 2; }

test -f Unity/ArkusUnity/ProjectSettings/ProjectVersion.txt
test -f Unity/ArkusUnity/Packages/manifest.json
test -f Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1Baseline.cs
test -f Unity/ArkusUnity/Assets/Arkus/H1/Tests/Editor/H1BaselineTests.cs
test -f scripts/h1-02-unity.ps1
test -f scripts/h1-02-static-check.py
test -f Docs/evidence/WP-H1-02/PREDECESSOR_CONTRACT_CHECK.md
test -f Docs/evidence/WP-H1-02/PROOF_PLAN.md
test -f Docs/evidence/WP-H1-02/DEPENDENCY_ADOPTION.md

python3 scripts/h1-02-static-check.py --mode remote-prep --self-test

DOTNET_NOLOGO=1 dotnet restore Juego2.sln --locked-mode -m:1 --disable-build-servers
DOTNET_NOLOGO=1 dotnet build Juego2.sln --configuration Release --no-restore -m:1 --disable-build-servers

result="READY_FOR_LOCAL_VALIDATION"
required="remote-static=GREEN; causal-negative-controls=GREEN; h0-locked-restore=GREEN; h0-release-build=GREEN; local-unity=PENDING"

if [[ -f Unity/ArkusUnity/Packages/packages-lock.json \
   && -f Docs/evidence/WP-H1-02/effective-inventory.json \
   && -f Docs/evidence/WP-H1-02/second-import-inventory.json \
   && -f Docs/evidence/WP-H1-02/editmode-results.xml \
   && -f Docs/evidence/WP-H1-02/PACKAGE_LEGAL_OBSERVATION.md \
   && -f Docs/evidence/WP-H1-02/LOCAL_RESULT.md ]]; then
  python3 scripts/h1-02-static-check.py --mode final --self-test
  python3 - <<'PY'
import json
from pathlib import Path

base = json.loads(Path('Docs/evidence/WP-H1-02/effective-inventory.json').read_text(encoding='utf-8'))
second = json.loads(Path('Docs/evidence/WP-H1-02/second-import-inventory.json').read_text(encoding='utf-8'))
for key in ('unityVersion', 'serializationMode', 'externalVersionControl', 'renderPipeline', 'packages', 'assemblies'):
    if base.get(key) != second.get(key):
        raise SystemExit(f'second-import-effective-inventory-mismatch:{key}')
print('H1_02_SECOND_IMPORT_PARITY_GREEN')
PY
  grep -Eq 'tests="[1-9][0-9]*"' Docs/evidence/WP-H1-02/editmode-results.xml
  grep -Eq 'failures="0"' Docs/evidence/WP-H1-02/editmode-results.xml
  grep -Fxq 'LOCAL_EXECUTION_RESULT: PASS' Docs/evidence/WP-H1-02/LOCAL_RESULT.md
  grep -Fxq 'PACKAGE_LEGAL_OBSERVATION: COMPLETE' Docs/evidence/WP-H1-02/PACKAGE_LEGAL_OBSERVATION.md
  result="GREEN"
  required="remote-static=GREEN; causal-negative-controls=GREEN; h0-locked-restore=GREEN; h0-release-build=GREEN; local-unity-evidence=GREEN; editmode=GREEN; effective-inventory=GREEN; clean-second-import=GREEN; dependency-legal-observation=GREEN"
fi

[[ -z "$(candidate_dirty_status)" ]] || { echo "Candidate is not clean after H1-02 observation" >&2; candidate_dirty_status >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-H1-02
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/h1-02-observe-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: ${required}
Result: ${result}
Evidence: Docs/evidence/WP-H1-02
EOF
