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
clean_candidate || { echo 'Dirty candidate before H2F-01A verification' >&2; exit 2; }

python3 - <<'PY'
import csv
import json
import re
import subprocess
from pathlib import Path

BASELINE = '4ba6b2d42ecb75a9c9fe450b5efc02aaa62a0849'
GC2_SHA = '1e4f3ba0560eadb944b3cf3ca63f7095d95b47c55a94ee655f28a15952f3380b'
root = Path('Docs/evidence/WP-H2F-01A')
res = root / 'results'
required = ['README.md', 'PREDECESSOR_CONTRACT_CHECK.md', 'CORE_VERSION_AND_PROVISIONING.md', 'CORE_SURFACE_INVENTORY.md',
            'CORE_CAPABILITY_MATRIX.csv', 'CORE_STATE_INVENTORY.md', 'PROBE_RESULTS.md', 'ARKUS_GC2_AUTHORITY_BOUNDARY.md',
            'HORIZON_SAVINGS_MAP.md', 'H2F02_CORE_HANDOFF.md', 'PUBLIC_AUTHORING_SURFACE.md', 'RESIDUE_AND_LICENSE_LEDGER.md',
            'probe_project/bootstrap_01a.py']
missing = [n for n in required if not (root / n).is_file()]
assert not missing, f'Missing H2F-01A evidence: {missing}'

# Scope: evidence + this verifier + its routing line only. No Unity/ArkusUnity, keeper, workpack or other WP bytes.
changed = subprocess.check_output(['git', 'diff', '--name-only', BASELINE, 'HEAD'], text=True).splitlines()
allowed_scripts = {'scripts/h2f01a-verify-exact-sha.sh', 'scripts/arkus-verify-exact-sha-base.sh'}
outside = [p for p in changed if not p.startswith(root.as_posix() + '/') and p not in allowed_scripts]
assert not outside, f'Unexpected mutation outside H2F-01A evidence: {outside}'

# Licensed/vendor bytes stay out: recipe is code/config only, images only under captures/, no Unity/vendor asset bytes,
# and no vendored Game Creator source (a GameCreator namespace declaration would mean copied vendor code).
code_ext = {'.cs', '.py', '.json', '.asmdef', '.md', '.txt'}
for p in (root / 'probe_project').rglob('*'):
    if p.is_file():
        assert p.suffix in code_ext, f'Non-recipe file in probe_project: {p}'
for p in root.rglob('*'):
    if not p.is_file():
        continue
    suffix = p.suffix.lower()
    assert suffix not in {'.fbx', '.unitypackage', '.blend', '.mat', '.prefab', '.unity', '.asset', '.anim',
                          '.controller', '.dll', '.wav', '.inputactions', '.meta'}, f'Asset/vendor bytes committed: {p}'
    if suffix in {'.png', '.jpg', '.jpeg'}:
        assert 'captures' in p.parts, f'Image outside captures/: {p}'
    if suffix in {'.cs', '.py', '.md', '.txt', '.json', '.tsv', '.csv'}:
        assert not re.search(r'^\s*namespace\s+GameCreator\b', p.read_text(encoding='utf-8', errors='ignore'), re.M), f'Vendor source in {p}'

# Authority boundary is structural: the Arkus stand-in references nothing and never names GC2; the adapter references
# exactly GC2 Core runtime + Arkus; no probe or adapter writes private GC2 members.
arkus_asm = json.loads((root / 'probe_project/Assets/H2F01A/Arkus/Juego2.H2F01A.Arkus.asmdef').read_text())
assert arkus_asm['references'] == [], 'Arkus stand-in must reference no assembly'
for p in (root / 'probe_project/Assets/H2F01A/Arkus').glob('*.cs'):
    assert 'GameCreator' not in p.read_text(encoding='utf-8'), f'GC2 type inside the Arkus boundary: {p}'
adapter_asm = json.loads((root / 'probe_project/Assets/H2F01A/Gc2Adapter/Juego2.H2F01A.Gc2Adapter.asmdef').read_text())
assert sorted(adapter_asm['references']) == ['GameCreator.Runtime.Core', 'Juego2.H2F01A.Arkus'], 'Adapter references drifted'
for p in (root / 'probe_project/Assets/H2F01A').rglob('*.cs'):
    text = p.read_text(encoding='utf-8')
    assert '.SetValue(' not in text, f'Reflection write into a vendor member: {p}'
    if 'Gc2Adapter' in p.parts or p.name == 'H2F01ABuilder.cs':
        assert 'System.Reflection' not in text and 'BindingFlags' not in text, f'Adapter/builder uses reflection: {p}'

# C01: exact installed Core identity; only Core assemblies (no separately licensed module).
imp = json.loads((res / 'c01_gc2_import_report.json').read_text())
assert imp['sha256'] == GC2_SHA and 'Packages/manifest.json' in imp['excluded'], 'GC2 package identity/import rule'
surface = json.loads((res / 'c01_core_surface_windowed.json').read_text())
assert surface['version'] == '2.19.61' and surface['unityVersion'] == '6000.3.24f1'
assert sorted(a['name'] for a in surface['assemblies']) == ['GameCreator.Editor.Core', 'GameCreator.Runtime.Core', 'GameCreator.Tests.Core'], \
    'A non-Core GC2 assembly is present'
inventory_families = {f['family'] for f in surface['families']}
assert all(f['found'] for f in surface['families']), 'Inventory base type not found in the installed Core'
assert len(surface['generatedSettings']) >= 1

# Capability matrix: one disposition per row from the 01A vocabulary; every inventory family and every WP-named family
# is covered; anti-duplication rows cite real probe results.
vocab = {'USE_FOUNDATION_NOW', 'USE_LATER_DO_NOT_DUPLICATE', 'LOCAL_EXECUTION_ONLY', 'DEFER_EVALUATION',
         'REDUNDANT_WITH_SELECTED_STACK', 'REJECT_CAPABILITY'}
with open(root / 'CORE_CAPABILITY_MATRIX.csv', newline='', encoding='utf-8') as h:
    rows = list(csv.DictReader(h))
cols = {'family_id', 'family', 'c01_family', 'status', 'evidence', 'public_route', 'retained_state', 'horizon',
        'authority_risk', 'replacement_risk', 'disposition', 'rationale'}
assert rows and cols <= set(rows[0].keys()), f'Matrix columns: {sorted(cols - set(rows[0].keys()))}'
ids = [r['family_id'] for r in rows]
assert len(ids) == len(set(ids)), 'Duplicate matrix family_id'
covered = set()
for r in rows:
    for c in cols:
        assert r[c].strip(), f"Unresolved matrix cell {r['family_id']}.{c}"
    assert r['status'] in {'PRESENT', 'ABSENT'}, f"Status: {r['family_id']}"
    assert r['disposition'] in vocab, f"Disposition: {r['family_id']}={r['disposition']}"
    covered |= {x.strip() for x in r['c01_family'].split(';')}
    if r['disposition'] in {'USE_FOUNDATION_NOW', 'USE_LATER_DO_NOT_DUPLICATE', 'REDUNDANT_WITH_SELECTED_STACK'}:
        assert re.search(r'\bC0[2-7]\b', r['evidence']), f"Reuse/redundancy claim without probe evidence: {r['family_id']}"
    for ref in re.findall(r'results/[\w./-]+\.(?:json|txt|tsv)', r['evidence']):
        assert (root / ref).is_file(), f"Evidence reference missing: {ref}"
uncovered = inventory_families - covered
assert not uncovered, f'C01 inventory families without a matrix row: {sorted(uncovered)}'
wp_families = ['character.kernel', 'input.inputsystem_bridge', 'locomotion.facing', 'navigation.navmesh_driver',
               'camera.gc2_camera_shots', 'interaction.focus_interact', 'interaction.hotspot_spots', 'vs.stock_library',
               'vs.custom_extensions', 'variables', 'character.model_avatar_animation', 'gestures_states', 'ik.look_at',
               'footsteps.material_sounds', 'ragdoll.recovery', 'save.host_storage']
assert not [f for f in wp_families if f not in ids], f'WP-named family missing: {[f for f in wp_families if f not in ids]}'
licensed = re.compile(r'\b(Inventory|Dialogue|Quests|Behavior|Perception|Melee|Shooter|Stats)\b module', re.I)
assert not [r['family_id'] for r in rows if licensed.search(r['family']) and r['disposition'] != 'REJECT_CAPABILITY'], \
    'Separately licensed module treated as Core'

# Probe facts (owner-workstation Unity 6000.3.24f1 runs; re-read here, not re-run).
def result(s):
    return json.loads((res / f'{s}_result.json').read_text())
for s in ['c02', 'c02g', 'c03', 'c04', 'c05', 'c06', 'c07']:
    r = result(s)
    assert r['scenario'] == s and r.get('console_errors') == 0 and 'fatal' not in r, f'{s}: errors or fatal'
    audit = json.loads((res / f'{s}_shader_audit.json').read_text())
    assert audit['green'] and audit['pipeline'] != 'built-in', f'{s}: shader audit not green under URP'
c02, c02g = result('c02'), result('c02g')
for label, r in (('c02_cinemachine', c02), ('c02_gc2camera', c02g)):
    assert r[f'{label}_arrived'] is True and r[f'{label}_stalls'] == 0 and r[f'{label}_camera_clip_frames'] == 0, label
assert c02['player_is_gc2_player'] and not c02['npc_is_gc2_player'] and c02['npc_nav_success'] and c02['player_avatar_human_valid']
assert c02['gc2_identity_fields_in_scene'] == '' and c02['arkus_binding_keys_unique'] and c02['gc2_players'] == 1
assert c02['cinemachine_brains'] == 1 and c02['gc2_camera_components'] == 0
assert c02g['gc2_camera_components'] == 1 and c02g['gc2_shot_cameras'] == 1 and c02g['cinemachine_brains'] == 0
for r in (c02, c02g):
    # Juego2 is the only input owner: its actions are enabled and no loose GC2 device action is
    actions = r['enabled_input_actions'].split(',')
    assert {'J2_Input/Player/Move', 'J2_Input/Player/Look', 'J2_Input/Player/Zoom'} <= set(actions)
    assert not [a for a in actions if a.startswith('<loose>//')], f'GC2 device action enabled: {actions}'
    assert r['gc2_identity_fields_in_scene'] == '' and r['unity_player_input_components'] == 0
assert -0.02 <= c02['player_foot_bone_above_ground_m'] <= 0.03, 'GC2 body feet not on ground'
c03 = result('c03')
assert c03['door_after_first_press'] == 'open' and c03['door_presentation_logged_after_second'] == 1
assert c03['mug_after_player'] == 'j2.char.player' and c03['chair_after_npc'] == 'j2.npc.probe_a'
assert c03['mug_after_npc_attempt'] == 'j2.char.player' and 'UNBOUND_ENTITY' in c03['arkus_log'] and c03['hotspot_activations'] >= 1
c04 = result('c04')
assert c04['gesture_playing_mid'] and not c04['gesture_playing_after'] and c04['gesture_max_bone_excursion_m'] > 0.1
assert c04['state_hips_in_state_m'] < c04['state_hips_stand_m'] - 0.2 and abs(c04['state_hips_after_one_loop_m'] - c04['state_hips_in_state_m']) < 0.05
assert c04['lookat_angle_before_deg'] - c04['lookat_angle_with_target_deg'] > 20, 'Look-at had no effect'
steps = dict(kv.split('=') for kv in c04['footsteps_by_detector_surface'].split(','))
assert all(int(steps.get(k, 0)) > 0 for k in ['ual_fulcrum:Floor_Cobble', 'ual_fulcrum:Floor_WoodDark', 'gc2loco_curves:Floor_Cobble', 'gc2loco_curves:Floor_WoodDark'])
plays = dict(kv.split('=') for kv in c04['footstep_clip_plays'].split(','))
assert int(plays.get('j2_step_cobble', 0)) > 0 and int(plays.get('j2_step_wood', 0)) > 0
assert c04['ragdoll_is_ragdoll'] and not c04['recover_is_ragdoll'] and c04['recover_can_move_m'] > 1.0
c05 = result('c05')
assert not c05['condition_door_open_initially'] and c05['property_entity_lamp'] == 'J2_Lamp' and c05['property_entity_unknown'] == 'null'
assert c05['trigger_runs_after_rejected'] == 0 and c05['lamp_fact_after_open'] == 'on' and c05['lamp_intensity_after_open'] > 0
assert c05['lamp_requests_total'] == 1 and c05['presentation_lines_on'] == 1
c06 = result('c06')
assert c06['gc2_storage_backend'].endswith('Juego2WorkspaceStorage') and c06['stored_payload_is_juego2_schema']
assert c06['roundtrip_digest_match'] and c06['roundtrip_not_reset_baseline'] and c06['gc2_load_reloaded_scene']
assert c06['tampered_load_host_accepted'] is False and c06['tampered_load_host_reason'] == 'VERSION:99' and not c06['tampered_digest_equals_saved']
assert c06['playerprefs_has_gc2_key'] is False and c06['has_save_slot1_after_delete'] is False
c07 = result('c07')
assert c07['c07_route_arrived'] and c07['c07_route_stalls'] == 0 and c07['c07_route_camera_clip_frames'] == 0
for p in ('pre_', 'post_'):
    # amended S06 realization: one GC2 player body, one GC2 camera, no Cinemachine brain, Juego2 input only
    assert c07[p + 'gc2_players'] == 1 and c07[p + 'main_camera_tagged'] == 1 and c07[p + 'enabled_cameras'] == 1
    assert c07[p + 'gc2_camera_components'] == 1 and c07[p + 'cinemachine_brains'] == 0 and c07[p + 'unity_player_input_components'] == 0
    assert not [a for a in c07[p + 'enabled_input_actions'].split(',') if a.startswith('<loose>//')], 'GC2 device action enabled'
    assert c07[p + 'arkus_bindings'] == 7
    assert c07[p + 'gc2_identity_fields_in_scene'] == '' and c07[p + 'arkus_binding_keys_unique']
assert c07['chair_after_npc'] == 'j2.npc.probe_a' and c07['mug_after_player'] == 'j2.char.player' and c07['lamp_fact_after_door'] == 'on'
assert c07['npc_recovered'] and c07['save_payload_matches_arkus'] and c07['player_footstep_events'] > 0

# S06: exactly one outcome, and an amendment names what it displaces and the H2F-02 handoff.
probe_text = (root / 'PROBE_RESULTS.md').read_text(encoding='utf-8')
outcomes = re.findall(r'^S06_OUTCOME: (NO_AMENDMENT|AMENDMENT_PROPOSED)\s*$', probe_text, re.M)
assert len(outcomes) == 1, f'PROBE_RESULTS must state exactly one S06_OUTCOME, found {outcomes}'
amendment = root / 'S06_BASELINE_AMENDMENT.md'
assert (outcomes[0] == 'AMENDMENT_PROPOSED') == amendment.exists(), 'S06 outcome and amendment file disagree'
if amendment.exists():
    text = amendment.read_text(encoding='utf-8')
    for needle in ['Displaced H2F-01 realization', 'Measured benefit', 'Authority and lifecycle burden', 'H2F-02 handoff']:
        assert needle in text, f'Amendment lacks: {needle}'
print(f'H2F-01A evidence: {len(rows)} capability rows ({len(inventory_families)} C01 families covered); '
      f'C02-C07 facts GREEN; authority boundary structural; scope isolated; amendment={"YES" if amendment.exists() else "NO"}')
PY

if [[ -n "${PR_BODY:-}" ]]; then
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Candidate HEAD SHA:[[:space:]]*\`?${ACTUAL_SHA}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Frozen candidate SHA:[[:space:]]*\`?${ACTUAL_SHA}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Worker pre-review:[[:space:]]*CLEAN[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Branch frozen:[[:space:]]*YES[[:space:]]*$'
fi
clean_candidate || { echo 'Dirty candidate after H2F-01A verification' >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-H2F-01A
Candidate SHA: ${ACTUAL_SHA}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/h2f01a-verify-exact-sha.sh ${ACTUAL_SHA}
Candidate clean before: YES
Candidate clean after: YES
Required gates: evidence-presence=GREEN; scope-isolation=GREEN; no-vendor-bytes=GREEN; structural-authority-boundary=GREEN; core-identity=GREEN; core-only-assemblies=GREEN; matrix-vocabulary=GREEN; inventory-family-coverage=GREEN; wp-family-coverage=GREEN; probe-backed-reuse=GREEN; c02-c07-facts=GREEN; negative-controls=GREEN; s06-outcome=GREEN; frozen-metadata=GREEN
Result: GREEN
Note: Unity/GC2 probe execution is owner-workstation evidence recorded in Docs/evidence/WP-H2F-01A/results; this verifier re-reads it and does not re-run Unity or require licensed GC2 bytes.
Evidence: Docs/evidence/WP-H2F-01A
EOF
