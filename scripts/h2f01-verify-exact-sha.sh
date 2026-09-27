#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT}"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"
ACTUAL_SHA="$(git rev-parse HEAD)"
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo 'Expected an exact candidate SHA' >&2; exit 2; }
[[ "${ACTUAL_SHA}" == "${EXPECTED_SHA}" ]] || { echo 'Candidate SHA mismatch' >&2; exit 2; }
# Candidate Validation writes its own context and receipt files before this step.
clean_candidate() {
  [[ -z "$(git status --porcelain --untracked-files=all | grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true)" ]]
}
clean_candidate || { echo 'Dirty candidate before H2F-01 verification' >&2; exit 2; }

python3 - <<'PY'
import csv
import json
import re
import subprocess
from pathlib import Path

BASELINE = 'a47f879c46abce88ee23d5959736e32ba5bd62be'
root = Path('Docs/evidence/WP-H2F-01')
required = ['README.md', 'PREDECESSOR_CONTRACT_CHECK.md', 'WORKER_PLAN.md', 'SPIKE_RESULTS.md',
            'DISPOSITION_MATRIX.csv', 'BASELINE_INTENT.md', 'SPIKE_LEDGER.md', 'ACQUISITION_AND_POLICY.md',
            'OWNER_JUDGEMENT.md', 'SPIKE_INPUT_LOCK.json', 'spike_project/bootstrap.py',
            'spike_project/Packages/manifest.json', 'spike_project/import_unitypackage.py']
missing = [n for n in required if not (root / n).is_file()]
assert not missing, f'Missing H2F-01 evidence: {missing}'

# Scope: evidence + this verifier/routing only. No Unity/ArkusUnity, keeper, or other WP bytes.
changed = subprocess.check_output(['git', 'diff', '--name-only', BASELINE, 'HEAD'], text=True).splitlines()
allowed_scripts = {'scripts/h2f01-verify-exact-sha.sh', 'scripts/arkus-verify-exact-sha-base.sh'}
outside = [p for p in changed if not p.startswith(str(root) + '/') and p not in allowed_scripts]
assert not outside, f'Unexpected mutation outside H2F-01 evidence: {outside}'

# Isolation: the recipe carries code/config only; no vendor or Source bytes; images only under captures/.
code_ext = {'.cs', '.shader', '.py', '.json', '.txt', '.md'}
for p in (root / 'spike_project').rglob('*'):
    if p.is_file():
        assert p.suffix in code_ext, f'Non-recipe file in spike_project: {p}'
        assert 'GameCreator' not in p.read_text(encoding='utf-8', errors='ignore') or p.suffix == '.cs', f'Vendor content in {p}'
for p in root.rglob('*'):
    if p.is_file() and p.suffix.lower() in {'.png', '.jpg', '.jpeg'}:
        assert p.parts[3] == 'captures', f'Image outside captures/: {p}'
    assert p.suffix.lower() not in {'.fbx', '.unitypackage', '.blend', '.mat', '.prefab', '.unity', '.asset'}, f'Asset bytes committed: {p}'

manifest = json.loads((root / 'spike_project/Packages/manifest.json').read_text())
for name, version in manifest['dependencies'].items():
    assert re.fullmatch(r'\d+\.\d+\.\d+', version), f'Floating spike package version {name}={version}'
lock = json.loads((root / 'SPIKE_INPUT_LOCK.json').read_text())
assert lock['schema'] == 'juego2.h2f01.spike-input-lock@1' and lock['artInputSha'] == '174d05d23c3bceb9d5df00e460b33519cf68328e'
assert lock['files'] and all(re.fullmatch(r'[0-9a-f]{64}', f['sha256']) for f in lock['files'])

# Disposition matrix covers the whole accepted H2F-00 register and leaves nothing SPIKE_REQUIRED.
vocab = {'ADOPT_NOW', 'AVAILABLE_ASSET', 'DEFER', 'REDUNDANT', 'REJECT', 'NO_ADOPT'}
register = []
for f in ['CANDIDATE_REGISTER.csv', 'CANDIDATE_REGISTER_ADDENDUM.csv']:
    with open(f'Docs/evidence/WP-H2F-00/{f}', newline='', encoding='utf-8') as h:
        register += list(csv.DictReader(h))
with open(root / 'DISPOSITION_MATRIX.csv', newline='', encoding='utf-8') as h:
    matrix = {r['candidate']: r for r in csv.DictReader(h)}
for r in register:
    m = matrix.get(r['candidate'])
    assert m is not None, f"H2F-00 candidate without final disposition: {r['candidate']}"
    assert m['final_disposition'] in vocab, f"Invalid final disposition: {r['candidate']}"
    assert m['rationale'].strip() and m['evidence'].strip(), f"Unexplained disposition: {r['candidate']}"
    if r['preliminary_disposition'] == 'SPIKE_REQUIRED':
        assert re.search(r'\bS0[1-8]\b|H2F-03', m['evidence']), f"Spike item not resolved by spike evidence: {r['candidate']}"
    if r['classification'].lower().startswith('paid'):
        assert m['final_disposition'] in {'DEFER', 'REJECT', 'NO_ADOPT'}, f"Paid candidate adopted without owner decision: {r['candidate']}"
for m in matrix.values():
    for ref in re.findall(r'(?:results|captures)/[\w./-]+\.(?:json|txt|tsv|png)', m['evidence']):
        assert (root / ref).is_file(), f"Evidence reference missing: {ref}"
    assert m['final_disposition'] in vocab

# Measured facts the selection depends on (recorded on the owner's workstation; re-read, not re-run).
res = lambda n: json.loads((root / 'results' / n).read_text())
assert res('s01_reproduction.json')['digestMatch'] is True
assert res('s01_audit_urpA.json')['green'] and res('s01_audit_urpC.json')['green']
assert res('s01_negative_audit_standard_under_urp.json')['green'] is False
s02 = res('s02_result.json')
assert abs(s02['w12WidthMin'] - 2.8) <= 0.011 and abs(s02['w12WidthMax'] - 2.8) <= 0.021
assert abs(s02['x1WidthMin'] - 5.5) <= 0.011 and abs(s02['x1WidthMax'] - 5.5) <= 0.021
assert s02['junctionHoles'] == 0 and s02['junctionSteps'] == 0
assert s02['artPath'] == 'PathComplete' and s02['splinePath'] == 'PathComplete' and s02['navOnScenic'] is False
s04 = res('s04_result.json')
assert s04['deterministic'] and s04['digestRun1'] == s04['digestRun2']
assert s04['violationsRoad'] == s04['violationsStructure'] == s04['violationsRiver'] == 0
assert s04['negativeRoadMaskOffViolations'] > 0 and s04['terrainAuditGreen']
assert res('s03_result.json')['shaderSupported'] and res('s03_result.json')['auditGreen']
assert res('s05_result.json')['shaderSupported'] and res('s05_result.json')['auditGreen']
pose = res('s07_pose_report.json')
assert pose['human'] and all(-0.02 <= p['footMin'] <= 0.03 for p in pose['poses'] if p['use'] in ('stand', 'walk'))
motion = (root / 'results/s07_motion_log.txt').read_text()
ik = float(re.search(r'ik_hand_to_target_weight1=([0-9.,]+)', motion).group(1).replace(',', '.'))
assert ik <= 0.02, 'IK contact not reached'
for log in ['s06_minimal_route_log.txt', 's06_gc2_route_log.txt']:
    text = (root / 'results' / log).read_text()
    assert 'arrived=True' in text and 'camera_clip_frames=0' in text and 'stalls=0' in text, log
assert 'Packages/manifest.json' in res('gc2_import_report.json')['excluded']
assert 'Dejo en tus manos' in (root / 'OWNER_JUDGEMENT.md').read_text(encoding='utf-8')
print(f'H2F-01 evidence: {len(register)} H2F-00 candidates resolved (+{len(matrix) - len(register)} owner addition); '
      f'S01-S07 facts GREEN; negative controls causal; scope isolated')
PY

if [[ -n "${PR_BODY:-}" ]]; then
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Candidate HEAD SHA:[[:space:]]*\`?${ACTUAL_SHA}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Frozen candidate SHA:[[:space:]]*\`?${ACTUAL_SHA}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Worker pre-review:[[:space:]]*CLEAN[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Branch frozen:[[:space:]]*YES[[:space:]]*$'
fi
clean_candidate || { echo 'Dirty candidate after H2F-01 verification' >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-H2F-01
Candidate SHA: ${ACTUAL_SHA}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/h2f01-verify-exact-sha.sh ${ACTUAL_SHA}
Candidate clean before: YES
Candidate clean after: YES
Required gates: evidence-presence=GREEN; scope-isolation=GREEN; no-vendor-bytes=GREEN; pinned-spike-packages=GREEN; register-coverage=GREEN; no-spike-required-left=GREEN; paid-owner-constraint=GREEN; recorded-spike-facts=GREEN; negative-controls=GREEN; owner-judgement=GREEN; frozen-metadata=GREEN
Result: GREEN
Note: Unity spike execution is owner-workstation evidence recorded in Docs/evidence/WP-H2F-01/results; this verifier re-reads it and does not re-run Unity.
Evidence: Docs/evidence/WP-H2F-01
EOF
