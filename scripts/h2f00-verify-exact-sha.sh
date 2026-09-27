#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT}"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"
ACTUAL_SHA="$(git rev-parse HEAD)"
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo 'Expected an exact candidate SHA' >&2; exit 2; }
[[ "${ACTUAL_SHA}" == "${EXPECTED_SHA}" ]] || { echo 'Candidate SHA mismatch' >&2; exit 2; }
# Candidate Validation writes its own context and receipt files before this step.
# Ignore only those untracked workflow artifacts; tracked changes still fail closed.
clean_candidate() {
  [[ -z "$(git status --porcelain --untracked-files=all | grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true)" ]]
}
clean_candidate || { echo 'Dirty candidate before research verification' >&2; exit 2; }

python3 - <<'PY'
import csv
import re
import subprocess
from pathlib import Path

root = Path('Docs/evidence/WP-H2F-00')
required = {'README.md', 'PREDECESSOR_CONTRACT_CHECK.md', 'RESEARCH_SOURCES.md',
            'CAPABILITY_SURVEY.md', 'CANDIDATE_REGISTER.csv', 'CANDIDATE_REGISTER_ADDENDUM.csv',
            'GAPS_ACQUISITION_AND_SPIKES.md'}
assert all((root / name).is_file() for name in required), 'Missing research evidence'

# This WP may publish research and its own verifier, but cannot change Unity or assets.
changed = set(subprocess.check_output(
    ['git', 'diff', '--name-only', '2e10beb11830b10d0f12015d24f86cfa19c56b28', 'HEAD'],
    text=True).splitlines())
allowed = {str(root / name) for name in required} | {
    'scripts/arkus-verify-exact-sha-base.sh', 'scripts/h2f00-verify-exact-sha.sh'}
assert required <= {Path(path).name for path in changed}, 'Incomplete evidence diff'
assert changed <= allowed, f'Unexpected project mutation: {changed - allowed}'

survey = (root / 'CAPABILITY_SURVEY.md').read_text()
survey_categories = {int(n) for n in re.findall(r'^\|\s*(\d{2})\s*\|', survey, re.M)}
assert survey_categories == set(range(1, 28)), 'Mandatory capability matrix incomplete'

sources = (root / 'RESEARCH_SOURCES.md').read_text()
source_ids = set(re.findall(r'^\|\s*([RUV]\d+)\s*\|', sources, re.M))
assert source_ids, 'Source ledger empty'

registers = [root / 'CANDIDATE_REGISTER.csv', root / 'CANDIDATE_REGISTER_ADDENDUM.csv']
rows = []
fieldnames = None
for register in registers:
    with register.open(newline='') as handle:
        reader = csv.DictReader(handle)
        assert reader.fieldnames is not None and len(reader.fieldnames) >= 18, f'Register fields missing: {register.name}'
        if fieldnames is None:
            fieldnames = reader.fieldnames
        else:
            assert reader.fieldnames == fieldnames, 'Register schema mismatch'
        rows.extend(list(reader))

required_fields = {'candidate', 'capability_categories', 'classification',
                   'license_or_EULA_observation', 'Unity_6_3_URP_observation',
                   'preliminary_disposition', 'rationale', 'source_ids'}
assert fieldnames is not None and required_fields <= set(fieldnames), 'Required register columns missing'
assert rows and all(row and all(value and value.strip() for value in row.values()) for row in rows), 'Empty candidate field'
covered = set()
for row in rows:
    covered.update(int(n) for n in row['capability_categories'].split('|'))
    assert set(row['source_ids'].split('|')) <= source_ids, f"Unknown source for {row['candidate']}"
    assert row['preliminary_disposition'] in {
        'ADOPT_NOW', 'AVAILABLE_ASSET', 'SPIKE_REQUIRED', 'DEFER', 'REDUNDANT', 'REJECT', 'NO_ADOPT'
    }, f"Invalid disposition for {row['candidate']}"
    if row['classification'].lower().startswith('paid'):
        assert row['preliminary_disposition'] == 'DEFER', 'Paid option requires owner decision'
        assert row['spike_required'] == 'no', 'Paid option cannot be a mandatory spike'
assert covered == set(range(1, 28)), 'Register capability coverage incomplete'

# Review-repair completeness: exact missing official Unity candidates are now explicit.
by_name = {row['candidate']: row for row in rows}
required_repair_candidates = {
    'Unity Starter Assets - ThirdPerson / Character Controllers': '11',
    'Unity Assistant (com.unity.ai.assistant)': '25',
    'Unity AI Gateway': '25',
    'Unity MCP Server': '25',
}
for name, category in required_repair_candidates.items():
    assert name in by_name, f'Missing review-repair candidate: {name}'
    assert category in by_name[name]['capability_categories'].split('|'), f'Wrong category for review-repair candidate: {name}'
assert by_name['Unity Starter Assets - ThirdPerson / Character Controllers']['spike_required'] == 'S06'
assert by_name['Unity MCP Server']['spike_required'] == 'S08'

gaps = (root / 'GAPS_ACQUISITION_AND_SPIKES.md').read_text()
assert all(re.search(rf'\bG{n}\b', gaps) for n in range(1, 8)), 'Gap map incomplete'
assert all(re.search(rf'\bS{n:02}\b', gaps) for n in range(1, 9)), 'Bounded spike handoff incomplete'
assert 'owner approves any purchase' in gaps, 'Purchase decision must remain with owner'
assert 'Starter Assets' in gaps and 'Unity MCP Server' in gaps, 'Review-repair handoff missing'
print(f'H2F-00 structural evidence: {len(rows)} candidates; 27 capabilities; {len(source_ids)} sources; G1-G7; S01-S08; review-repair candidates present; research-only diff GREEN')
PY

if [[ -n "${PR_BODY:-}" ]]; then
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Candidate HEAD SHA:[[:space:]]*\`?${ACTUAL_SHA}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Frozen candidate SHA:[[:space:]]*\`?${ACTUAL_SHA}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Worker pre-review:[[:space:]]*CLEAN[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Branch frozen:[[:space:]]*YES[[:space:]]*$'
fi
clean_candidate || { echo 'Dirty candidate after research verification' >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-H2F-00
Candidate SHA: ${ACTUAL_SHA}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/h2f00-verify-exact-sha.sh ${ACTUAL_SHA}
Candidate clean before: YES
Candidate clean after: YES
Required gates: research-evidence-presence=GREEN; 27-category-coverage=GREEN; candidate-source-integrity=GREEN; review-repair-completeness=GREEN; paid-owner-constraint=GREEN; bounded-handoff=GREEN; research-only-diff=GREEN; frozen-metadata=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-H2F-00
EOF
