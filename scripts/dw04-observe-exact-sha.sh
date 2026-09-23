#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"
BASELINE_SHA="82369fdb69e33e3492b41c4aaa7f1ae9aaed18af"
PRECALIBRATION_COMMIT="ad570738613c992963668be5abd89bfdeb26b7d6"
cd "${ROOT}"

candidate_dirty_status() {
  git status --porcelain --untracked-files=all | \
    grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true
}

actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }
git cat-file -e "${BASELINE_SHA}^{commit}" || { echo "DW-04 baseline commit is unavailable" >&2; exit 2; }
git cat-file -e "${PRECALIBRATION_COMMIT}^{commit}" || { echo "DW-04 pre-calibration freeze commit is unavailable" >&2; exit 2; }
git merge-base --is-ancestor "${BASELINE_SHA}" "${actual}" || { echo "DW-04 candidate does not descend from accepted baseline" >&2; exit 2; }
git merge-base --is-ancestor "${PRECALIBRATION_COMMIT}" "${actual}" || { echo "DW-04 candidate lost the effective pre-calibration freeze" >&2; exit 2; }
[[ -z "$(candidate_dirty_status)" ]] || { echo "Candidate is not clean before DW-04 observation" >&2; candidate_dirty_status >&2; exit 2; }

for path in \
  Docs/evidence/WP-DW-04/PRECALIBRATION_FREEZE.json \
  Docs/evidence/WP-DW-04/CALIBRATION_ORACLES.json \
  Docs/evidence/WP-DW-04/ACCEPTANCE_SOURCE_ORACLES.json \
  Docs/evidence/WP-DW-04/PREDECESSOR_CONTRACT_CHECK.md \
  Docs/evidence/WP-DW-04/WORKER_PLAN.md \
  scripts/dw04-trial.py \
  tools/Arkus.Dw04.Retrieval/Arkus.Dw04.Retrieval.csproj \
  tools/Arkus.Dw04.Retrieval/Program.cs; do
  test -f "${path}"
done

# DW-04 consumes accepted authority and predecessors. It may add trial tooling,
# but it may not mutate accepted CITY/PA truth or DW-00..03 production semantics.
git diff --exit-code "${BASELINE_SHA}" "${actual}" -- \
  Docs/production/CITY_LOCATION_PROGRAMME.md \
  Docs/production/CITY_MOBILITY_TOPOLOGY.md \
  Docs/research/living-world/results/PA-01.md \
  Docs/research/living-world/results/PA-02.md \
  Docs/research/living-world/results/PA-03.md \
  Docs/research/living-world/results/PA-04.md \
  Docs/research/living-world/results/PA-05.md \
  Docs/evidence/WP-PA-05/TRANSFER_AND_FIXTURES.md \
  src/Arkus.DesignWorld \
  tests/Arkus.Harness.Tests/Dw00* \
  tests/Arkus.Harness.Tests/Dw01* \
  tests/Arkus.Harness.Tests/Dw02* \
  tests/Arkus.Harness.Tests/Dw03*

PYTHONDONTWRITEBYTECODE=1 python3 scripts/dw04-trial.py precheck --pre-commit "${PRECALIBRATION_COMMIT}"
PYTHONDONTWRITEBYTECODE=1 python3 scripts/dw04-trial.py selftest
PYTHONDONTWRITEBYTECODE=1 python3 - <<'PY'
import json
from pathlib import Path
root = Path('.')
for name in ('CALIBRATION_ORACLES.json', 'ACCEPTANCE_SOURCE_ORACLES.json'):
    data = json.loads((root / 'Docs/evidence/WP-DW-04' / name).read_text(encoding='utf-8'))
    assert data['precalibration_commit'] == 'ad570738613c992963668be5abd89bfdeb26b7d6', name
    for task in data['tasks'].values():
        for item in task.get('required_context', []):
            source = (root / item['source_path']).read_text(encoding='utf-8')
            assert item['literal'] in source, (name, item)
for path in sorted((root / 'scripts').glob('dw04-*.py')):
    compile(path.read_text(encoding='utf-8'), str(path), 'exec')
print('DW-04 frozen universe/oracles/scorer: GREEN')
PY

DOTNET_NOLOGO=1 dotnet restore Juego2.sln --locked-mode -m:1 --disable-build-servers
DOTNET_NOLOGO=1 dotnet build Juego2.sln --configuration Release --no-restore -m:1 --disable-build-servers
DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
  --configuration Release --no-build --no-restore -m:1 --disable-build-servers
DOTNET_NOLOGO=1 dotnet build tools/Arkus.Dw04.Retrieval/Arkus.Dw04.Retrieval.csproj \
  --configuration Release -m:1 --disable-build-servers

PYTHONDONTWRITEBYTECODE=1 python3 - <<'PY'
import json, subprocess
from pathlib import Path
root = str(Path('.').resolve())
cases = [
    (['city', 'loc.casco.shared_court'], 'Docs/production/CITY_LOCATION_PROGRAMME.md', 'SourceRow'),
    (['city', 'loc.puerto.landing'], 'Docs/production/CITY_LOCATION_PROGRAMME.md', 'SourceRow'),
    (['pa-finding', 'pa01', 'DL-11'], 'Docs/research/living-world/results/PA-01.md', 'MaterialText'),
    (['pa-fixture', 'pa04', 'NC-02'], 'Docs/research/living-world/results/PA-04.md', 'MaterialText'),
    (['pa-fixture', 'pa05', 'NC-02'], 'Docs/evidence/WP-PA-05/TRANSFER_AND_FIXTURES.md', 'MaterialText'),
]
for query, expected_path, material_field in cases:
    command = ['dotnet', 'run', '--project', 'tools/Arkus.Dw04.Retrieval', '-c', 'Release', '--no-build', '--', root, *query]
    first = subprocess.check_output(command).decode('utf-8').strip()
    second = subprocess.check_output(command).decode('utf-8').strip()
    assert first == second, query
    result = json.loads(first)
    assert result['Provenance']['SourcePath'] == expected_path, query
    assert result[material_field], query
print('DW-04 accepted typed-query/source-open replay: GREEN')
PY

[[ -z "$(candidate_dirty_status)" ]] || { echo "Candidate is not clean after DW-04 observation" >&2; candidate_dirty_status >&2; exit 2; }

cat <<EOF
DW04_OBSERVATION_V1
WP: WP-DW-04
Candidate SHA: ${actual}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/dw04-observe-exact-sha.sh ${actual}
Instrument/universe/oracles: GREEN
Accepted-authority immutability: GREEN
Locked restore/build/regression: GREEN
Typed retrieval determinism/source replay: GREEN
Foundational paired-agent campaign: NOT_EVALUATED_BY_OBSERVATION
Result: OBSERVATION_GREEN
EOF
