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
clean_candidate || { echo 'Dirty candidate before H2F-02 verification' >&2; exit 2; }

python3 - <<'PY'
import csv
import hashlib
import json
import re
import subprocess
from pathlib import Path

BASELINE = 'e5322c842de072ae120ae19fe5cf4280e3569806'
GC2_SHA = '1e4f3ba0560eadb944b3cf3ca63f7095d95b47c55a94ee655f28a15952f3380b'
EV = Path('Docs/evidence/WP-H2F-02')
RES = EV / 'results'
U = Path('Unity/ArkusUnity')

def git(*a):
    return subprocess.check_output(['git', *a], text=True)

required = ['README.md', 'PREDECESSOR_CONTRACT_CHECK.md', 'WORKER_PLAN.md', 'ADOPTION_RECORDS.md', 'URP_BASELINE.md',
            'IMPORT_CONVENTIONS.md', 'ADAPTER_BOUNDARY.md', 'PROVISIONING.md', 'GC2_CORE_ADOPTION.md', 'H1_COMPATIBILITY.md',
            'H1_LIFECYCLE_MATRIX.csv', 'RESIDUALS.md', 'OWNER_JUDGEMENT.md', 'workspace/workspace.py',
            'tools/h1_catalogue_fingerprint.py', 'results/summary.json', 'results/representative.json',
            'results/player_build.json', 'results/c2_gc2_lint.json', 'results/a1_clean_absent_report.json',
            'results/b1_gc2_report.json', 'results/b0_gc2_provisioning_receipt.json', 'results/real_project_material_migration.json']
missing = [n for n in required if not (EV / n).is_file()]
assert not missing, f'Missing H2F-02 evidence: {missing}'

# --- scope: only the declared write set changed since the baseline
changed = git('diff', '--name-only', BASELINE, 'HEAD').splitlines()
allowed_prefix = ('Unity/ArkusUnity/Assets/Juego2/', 'Unity/ArkusUnity/Packages/', 'Unity/ArkusUnity/ProjectSettings/',
                  'Unity/ArkusUnity/Assets/Arkus/CITY/Materials/', 'Docs/evidence/WP-H2F-02/', 'scripts/h2f02-')
allowed_exact = {'Unity/ArkusUnity/Assets/Juego2.meta', '.gitignore', 'scripts/arkus-verify-exact-sha-base.sh',
                 'Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1Baseline.cs', 'Unity/ArkusUnity/Assets/Arkus/H1/Tests/Editor/H1BaselineTests.cs',
                 'Unity/ArkusUnity/Assets/Arkus/CITY/Editor/City04GreyboxBuilder.cs',
                 'Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json', 'tests/Arkus.Harness.Tests/H1CatalogueTests.cs'}
outside = [p for p in changed if p not in allowed_exact and not p.startswith(allowed_prefix)]
assert not outside, f'Mutation outside the H2F-02 write set: {outside}'
tests_diff = git('diff', BASELINE, 'HEAD', '--', 'tests/Arkus.Harness.Tests/H1CatalogueTests.cs')
assert len([l for l in tests_diff.splitlines() if l.startswith(('+', '-')) and not l.startswith(('+++', '---'))]) == 2, 'H1CatalogueTests: only the pinned fingerprint may change'

# --- licensed/vendor bytes never committed
tracked = git('ls-files').splitlines()
assert not [p for p in tracked if '/Plugins/GameCreator' in p or p.endswith('.unitypackage') or p.endswith('csc.rsp')], 'GC2 vendor bytes or csc.rsp tracked'
for p in tracked:
    if p.endswith('.cs') and (p.startswith('Unity/') or p.startswith(str(EV))):
        assert not re.search(r'^\s*namespace\s+GameCreator\b', Path(p).read_text(encoding='utf-8', errors='ignore'), re.M), f'Vendor source in {p}'
ignore = Path('.gitignore').read_text(encoding='utf-8')
for rule in ['Unity/ArkusUnity/Assets/Plugins/GameCreator/', 'Unity/ArkusUnity/Assets/csc.rsp']:
    assert rule in ignore, f'.gitignore lacks {rule}'
for p in EV.rglob('*'):
    if p.is_file() and p.suffix.lower() in {'.png', '.jpg'}:
        assert 'captures' in p.parts, f'image outside captures/: {p}'
    assert p.suffix.lower() not in {'.fbx', '.unitypackage', '.dll', '.exe'}, f'binary vendor/build bytes in evidence: {p}'

# --- exact package set (independent of the Unity-side constants)
direct = {'com.unity.ai.navigation': '2.0.15', 'com.unity.animation.rigging': '1.4.1', 'com.unity.collections': '2.6.8',
          'com.unity.inputsystem': '1.20.0', 'com.unity.mathematics': '1.3.3', 'com.unity.render-pipelines.universal': '17.3.0',
          'com.unity.shadergraph': '17.3.0', 'com.unity.splines': '2.9.1', 'com.unity.test-framework': '1.6.0', 'com.unity.ugui': '2.0.0'}
for m in ['ai', 'animation', 'audio', 'imageconversion', 'imgui', 'jsonserialize', 'particlesystem', 'physics', 'physics2d',
          'screencapture', 'terrain', 'terrainphysics', 'ui', 'uielements']:
    direct[f'com.unity.modules.{m}'] = '1.0.0'
manifest = json.loads((U / 'Packages/manifest.json').read_text(encoding='utf-8'))['dependencies']
assert manifest == direct, f'manifest drift: {sorted(set(manifest.items()) ^ set(direct.items()))}'
lock = json.loads((U / 'Packages/packages-lock.json').read_text(encoding='utf-8'))['dependencies']
for name, version in direct.items():
    assert lock[name]['version'] == version and lock[name]['depth'] == 0, f'lock {name}'
for name in ['com.unity.cinemachine', 'com.unity.probuilder', 'com.unity.terrain-tools', 'com.unity.render-pipelines.high-definition',
             'com.unity.timeline', 'com.unity.addressables', 'com.unity.recorder']:
    assert name not in lock, f'not admitted but resolved: {name}'

# --- project settings of the adopted baseline
def text(rel): return (U / rel).read_text(encoding='utf-8')
def meta_guid(rel): return re.search(r'^guid: ([0-9a-f]{32})', text(rel + '.meta'), re.M).group(1)
assert 'm_EditorVersion: 6000.3.24f1' in text('ProjectSettings/ProjectVersion.txt')
ps = text('ProjectSettings/ProjectSettings.asset')
assert re.search(r'^\s*m_ActiveColorSpace: 1$', ps, re.M) and re.search(r'^\s*activeInputHandler: 2$', ps, re.M), 'colour space / input handler'
high, low = meta_guid('Assets/Juego2/Foundation/Rendering/J2_URP_High.asset'), meta_guid('Assets/Juego2/Foundation/Rendering/J2_URP_Low.asset')
gs = text('ProjectSettings/GraphicsSettings.asset')
assert re.search(r'm_CustomRenderPipeline: \{fileID: 11400000, guid: ' + high, gs), 'default pipeline is not J2_URP_High'
assert 'm_LightsUseLinearIntensity: 1' in gs and 'm_LightsUseColorTemperature: 1' in gs, 'URP light units not committed'
qs = text('ProjectSettings/QualitySettings.asset')
names = re.findall(r'^\s*- serializedVersion: \d+\n\s*name: (\w+)', qs, re.M)
assert names == ['Low', 'High'], f'quality levels {names}'
assert qs.count('customRenderPipeline: {fileID: 11400000, guid: ' + low) == 1 and qs.count('customRenderPipeline: {fileID: 11400000, guid: ' + high) == 1
nav = text('ProjectSettings/NavMeshAreas.asset')
for key, value in [('agentRadius', '0.28'), ('agentHeight', '1.8'), ('agentSlope', '40'), ('agentClimb', '0.3')]:
    assert re.search(rf'^\s*{key}: {re.escape(value)}$', nav, re.M), f'NavMesh agent {key}'
assert re.search(r'm_RenderingLayers:\n\s*- Default\n\s*- DecalReceiver', text('ProjectSettings/TagManager.asset')), 'DecalReceiver layer'
for asset, dist, cascades in [('J2_URP_High.asset', '70', '2'), ('J2_URP_Low.asset', '40', '1')]:
    a = text('Assets/Juego2/Foundation/Rendering/' + asset)
    for key, value in [('m_RequireDepthTexture', '1'), ('m_ShadowDistance', dist), ('m_ShadowCascadeCount', cascades),
                       ('m_SoftShadowsSupported', '1'), ('m_LightProbeSystem', '0'), ('m_SupportsLightLayers', '1')]:
        assert re.search(rf'^\s*{key}: {value}$', a, re.M), f'{asset} {key}'
rh = text('Assets/Juego2/Foundation/Rendering/J2_Renderer_High.asset'); rl = text('Assets/Juego2/Foundation/Rendering/J2_Renderer_Low.asset')
assert 'ScreenSpaceAmbientOcclusion' in rh and 'ScreenSpaceAmbientOcclusion' not in rl, 'SSAO tier policy'
for r in (rh, rl):
    assert 'DecalRendererFeature' in r and re.search(r'^\s*decalLayers: 1$', r, re.M), 'decal layers'

# --- structural GC2 / Arkus boundary
def asmdef(rel): return json.loads(text(rel))
assert asmdef('Assets/Juego2/Arkus/Juego2.Arkus.asmdef')['references'] == [], 'Juego2.Arkus must reference nothing'
adapter = asmdef('Assets/Juego2/Gc2Adapter/Runtime/Juego2.Gc2Adapter.asmdef')
assert sorted(adapter['references']) == ['GameCreator.Runtime.Core', 'Juego2.Arkus'] and adapter['defineConstraints'] == ['JUEGO2_GC2_CORE']
strip = re.compile(r'@"(?:""|[^"])*"|"(?:\\.|[^"\\])*"|//[^\n]*|/\*.*?\*/', re.S)
for p in (U / 'Assets').rglob('*.asmdef'):
    t = p.read_text(encoding='utf-8')
    assert '"GUID:' not in t, f'GUID asmdef reference: {p}'
    if re.search(r'"GameCreator\.[^"]+"', t):
        assert 'Assets/Juego2/Gc2Adapter' in p.as_posix() and '"JUEGO2_GC2_CORE"' in t, f'ungated/outside GameCreator reference: {p}'
for root in ['Assets/Arkus', 'Assets/Juego2/Arkus', 'Assets/Juego2/Foundation']:
    for p in (U / root).rglob('*.cs'):
        assert not re.search(r'\bGameCreator\b', strip.sub(' ', p.read_text(encoding='utf-8'))), f'GameCreator named in {p}'
adapters = text('Assets/Juego2/Gc2Adapter/Runtime/ArkusGc2Adapters.cs')
for t in ['ConditionArkusFact', 'InstructionArkusRequestTransition', 'EventOnArkusFactChanged', 'GetStringArkusFact', 'GetStringArkusLiteral', 'GetGameObjectArkusEntity']:
    assert re.search(rf'\[MovedFrom\(false, "Juego2\.H2F01A\.Gc2Adapter"[^\]]*\]\s*public class {t}\b', adapters), f'frozen seam type {t}'
assert 'ArkusProbeAuthority' not in adapters and 'ArkusFacts.' in adapters, 'adapter must go through the Juego2 authority seam'
assert not (U / 'Assets/Juego2/Gc2Adapter/Runtime/ArkusGc2Save.cs').exists(), 'save host is H6 DEFER_EVALUATION'
lint = text('Assets/Juego2/Gc2Adapter/Editor/J2Gc2Lint.cs')
for code in ['J2_GC2_L2_REMEMBER', 'J2_GC2_L2_SCENE_INSTRUCTION', 'J2_GC2_L3_VARIABLE_NAMED_AS_FACT', 'J2_GC2_L4_ADAPTER_ON_UNBOUND_OBJECT',
             'J2_GC2_L5_UNGATED_MUTATION', 'J2_GC2_L6_', 'J2_GC2_L7_JUEGO2_SAVE_THROUGH_GC2']:
    assert code in lint, f'lint {code}'
city = text('Assets/Arkus/CITY/Editor/City04GreyboxBuilder.cs')
assert 'Universal Render Pipeline/Lit' in city
for mat in (U / 'Assets/Arkus/CITY/Materials').glob('*.mat'):
    assert 'guid: 0000000000000000f000000000000000' not in mat.read_text(encoding='utf-8'), f'built-in Standard left: {mat}'

# --- H1 catalogue re-baseline: one row, fingerprint recomputed independently
import importlib.util
spec = importlib.util.spec_from_file_location('fp', EV / 'tools/h1_catalogue_fingerprint.py')
before = json.loads(git('show', f'{BASELINE}:Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json'))
after = json.loads(Path('Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json').read_text(encoding='utf-8'))
diff_rows = [(a, b) for a, b in zip(before['rows'], after['rows']) if a != b]
assert len(before['rows']) == len(after['rows']) and len(diff_rows) == 1, 'EFFECTIVE_INVENTORY must differ in exactly one row'
a, b = diff_rows[0]
assert a['path'] == b['path'] == 'Assets/Arkus/H1/SourceSlice/MI_Plaster.mat' and {k: v for k, v in a.items() if k != 'dependencies'} == {k: v for k, v in b.items() if k != 'dependencies'}
assert a['dependencies'] == [] and b['dependencies'] == ['Packages/com.unity.render-pipelines.universal/Editor/AssetVersion.cs']
fpmod = importlib.util.module_from_spec(spec)
spec.loader.exec_module(fpmod)
ns = {'fp': fpmod.fp}
mapping = Path('Unity/ArkusUnity/Assets/Arkus/H1/CatalogueMapping.json').read_text(encoding='utf-8')
assert ns['fp'](json.dumps(before), mapping) == 'e1e92d9878a4fea1ac9fb876d10b17a870db7ac68a669dd97ecf49b036f8c3e1', 'fingerprint replica does not reproduce the accepted value'
pinned = re.search(r'Assert\.Equal\("([0-9a-f]{64})", snapshot\.Fingerprint\)', Path('tests/Arkus.Harness.Tests/H1CatalogueTests.cs').read_text(encoding='utf-8')).group(1)
assert ns['fp'](json.dumps(after), mapping) == pinned, 'pinned catalogue fingerprint does not match the re-baselined snapshot'
h1 = text('Assets/Arkus/H1/Editor/H1Baseline.cs')
assert 'RenderPipelineAssetPath = "Assets/Juego2/Foundation/Rendering/J2_URP_High.asset"' in h1 and 'render-pipeline-mismatch' in h1

# --- lifecycle matrix completeness
classes = {'H1_MANAGED_PROJECTION', 'RETAINED_PROJECT_CONFIGURATION', 'RETAINED_REALIZATION', 'GENERATED_TRANSIENT', 'EXTERNAL_MANUAL_INPUT', 'NOT_ADMITTED'}
with open(EV / 'H1_LIFECYCLE_MATRIX.csv', newline='', encoding='utf-8') as h:
    rows = list(csv.DictReader(h))
cols = ['family_id', 'family', 'introduced_by', 'lifecycle_class', 'host', 'source_of_truth', 'materialize', 'observe_drift', 'reconcile', 'rematerialize_clean_rebuild', 'witness']
assert rows and list(rows[0].keys()) == cols
ids = [r['family_id'] for r in rows]
assert len(ids) == len(set(ids)), 'duplicate family'
needed = [f'gc2.S{i}' for i in range(1, 16)] + ['gc2.define', 'gc2.adapter_code', 'gc2.scene_instructions', 'pkg.manifest_lock', 'settings.project',
          'urp.pipeline_assets', 'urp.look_preset', 'urp.scene_look', 'gi.bakes', 'gi.apv', 'shaders.project', 'materials.retained',
          'h1.catalogue_snapshot', 'h1.managed_projection', 'splines.containers', 'splines.instantiate', 'linear.realization', 'scatter.realization',
          'terrain', 'terrain.vegetation', 'navmesh.surface', 'navmesh.data', 'navmesh.agent', 'import.humanoid', 'import.ual', 'rigging',
          'input.asset', 'presets.character_camera', 'cinemachine']
assert not [f for f in needed if f not in ids], f'lifecycle families missing: {[f for f in needed if f not in ids]}'
for r in rows:
    assert r['lifecycle_class'] in classes, r['family_id']
    assert all(r[c].strip() for c in cols), f"empty cell in {r['family_id']}"
    for ref in re.findall(r'results/[\w./-]+\.(?:json|xml)', r['witness']):
        assert (EV / ref).is_file(), f"witness missing: {ref}"

# --- workspace evidence bound to the exact candidate Unity bytes
s = json.loads((RES / 'summary.json').read_text(encoding='utf-8'))
assert s['unityTree'] == git('rev-parse', 'HEAD:Unity/ArkusUnity').strip(), 'evidence ran on different Unity bytes'
subprocess.check_call(['git', 'merge-base', '--is-ancestor', s['candidateSha'], 'HEAD'])
assert not git('diff', '--name-only', s['candidateSha'], 'HEAD', '--', 'Unity/ArkusUnity', 'scripts/h2f02-provision.py', 'scripts/h2f02-unity.py',
               'Docs/evidence/WP-H2F-02/workspace', '.gitignore').strip(), 'evidence-affecting bytes changed after the evidence run'
for k in ['a_clean_absent_verify_exit', 'b_gc2_provision_exit', 'b_gc2_verify_exit', 'c_representative_exit', 'c_lint_exit', 'c_player_build_exit']:
    assert s[k] == 0, k
for k in ['a_clean_absent_tests', 'b_gc2_tests']:
    assert s[k]['result'] == 'Passed' and s[k]['failed'] == '0' and int(s[k]['total']) > 20, k
for k in ['a_tree', 'b_tree', 'c_tree']:
    assert s[k]['changed'] == [] and s[k]['files'] > 250, f'{k}: committed bytes changed by import/provisioning/evidence'
a1 = json.loads((RES / 'a1_clean_absent_report.json').read_text()); b1 = json.loads((RES / 'b1_gc2_report.json').read_text())
assert a1['gc2Core'] == 'ABSENT' and a1['findings'] == [] and not [x for x in a1['assemblies'] if x.startswith('GameCreator') or x.startswith('Juego2.Gc2')]
assert a1['activePipelineAsset'].endswith('J2_URP_High.asset') and a1['colorSpace'] == 'Linear'
assert b1['gc2Core'] == 'PROVISIONED_CORE_2.19.61' and b1['findings'] == []
assert sorted(x for x in b1['assemblies'] if x.startswith('GameCreator')) == ['GameCreator.Editor.Core', 'GameCreator.Runtime.Core', 'GameCreator.Tests.Core']
receipt = json.loads((RES / 'b0_gc2_provisioning_receipt.json').read_text())
assert receipt['packageSha256'] == GC2_SHA and receipt['version'] == '2.19.61' and receipt['excluded'] == ['Packages/manifest.json']
r = json.loads((RES / 'representative.json').read_text(encoding='utf-8'))
assert r['errors'] == [] and r['lintFindings'] == [] and r['identityFields'] == [], 'representative errors/lint/identity'
assert r['citizenAvatarValid'] and r['playerIsGc2Player'] and r['npcInputNone']
assert r['bindings'] == ['j2.char.player', 'j2.npc.evidence_a']
assert r['junctionSamples'] > 5000 and r['junctionHoles'] == 0 and r['junctionSteps'] == 0
assert r['scatterInstances'] > 50 and r['scatterInExclusion'] == 0
for audit in ['routeAudit', 'contentAudit', 'worldbuildingAudit']:
    assert r[audit]['green'] and r[audit]['failures'] == [] and r[audit]['pipeline'] != 'builtin', audit
assert all(f.startswith('J2_IMPORT_LOOP_FLAG:Assets/Arkus/ART/External/UAL/UAL1.fbx#') for f in r['importFindings']), 'import findings outside the recorded ART-01 rebase item'
assert all('Hips=pelvis' in m for m in r['humanoidMapping'])
inputs = r['enabledInputBindings']
assert all(('InputValueVector2None' in i) or ('J2_Input/' in i) for i in inputs) and any('J2_Input/Move' in i for i in inputs)
for cap in r['captures']:
    assert (EV / 'captures/owner' / Path(cap).name).is_file(), f'capture not kept: {cap}'
pb = json.loads((RES / 'player_build.json').read_text())
assert pb['result'] == 'Succeeded' and pb['totalErrors'] == 0
assert 'GameCreator.Runtime.Core.dll' in pb['playerAssemblies'] and 'Juego2.Gc2Adapter.dll' in pb['playerAssemblies']
assert not [a for a in pb['playerAssemblies'] if 'Editor' in a or 'Tests' in a], 'editor/test assembly shipped'
assert json.loads((RES / 'c2_gc2_lint.json').read_text())['findings'] == []
mig = json.loads((RES / 'real_project_material_migration.json').read_text())
assert mig['audit']['green'] and mig['audit']['failures'] == []
owner = (EV / 'OWNER_JUDGEMENT.md').read_text(encoding='utf-8')
assert re.search(r'^OWNER_VISUAL_INSPECTION: (ACCEPTED|ACCEPTED_WITH_NOTES)\s*$', owner, re.M), 'owner visual inspection not recorded'
print(f'H2F-02 evidence: scope, vendor exclusion, exact packages/settings, GC2 boundary, H1 catalogue re-baseline, '
      f'{len(rows)} lifecycle families, workspace evidence bound to tree {s["unityTree"][:12]}: GREEN')
PY

if [[ -n "${PR_BODY:-}" ]]; then
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Candidate HEAD SHA:[[:space:]]*\`?${ACTUAL_SHA}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Frozen candidate SHA:[[:space:]]*\`?${ACTUAL_SHA}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Worker pre-review:[[:space:]]*CLEAN[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Branch frozen:[[:space:]]*YES[[:space:]]*$'
fi
clean_candidate || { echo 'Dirty candidate after H2F-02 verification' >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-H2F-02
Candidate SHA: ${ACTUAL_SHA}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/h2f02-verify-exact-sha.sh ${ACTUAL_SHA}
Candidate clean before: YES
Candidate clean after: YES
Required gates: evidence-presence=GREEN; scope-isolation=GREEN; no-vendor-bytes=GREEN; exact-packages=GREEN; urp-project-settings=GREEN; gc2-structural-boundary=GREEN; frozen-seam-names=GREEN; city04-urp=GREEN; h1-catalogue-rebaseline=GREEN; lifecycle-matrix=GREEN; evidence-bound-to-unity-tree=GREEN; clean-restoration=GREEN; gc2-provisioned=GREEN; representative-content=GREEN; player-build=GREEN; owner-inspection=GREEN; frozen-metadata=GREEN
Result: GREEN
Note: Unity/GC2 execution is owner-workstation evidence recorded in Docs/evidence/WP-H2F-02/results; this verifier re-reads it and binds it to the exact Unity tree; it does not run Unity or need licensed GC2 bytes. Hosted H1 suites are separate GitHub Actions evidence.
Evidence: Docs/evidence/WP-H2F-02
EOF
