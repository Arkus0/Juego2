# WP-H1-02 — LOCAL_EXECUTION

WP_ID: `WP-H1-02`
LOCAL_ROUND: `4`
REPOSITORY: `Arkus0/Juego2`
CANONICAL_PR: `#152`
CANONICAL_BRANCH: `work/wp-h1-02`
EXECUTION_BASE_SHA: `e9e022f871ba6b7da95f663956b07d0519b99b2d`
MANIFEST_COMMIT_ANCHOR: PR `#152` comment beginning `H1_LOCAL_HANDOFF_V1 WP=WP-H1-02 ROUND=4`
RESULT_FILE: evidence directory + `LOCAL_EXECUTION_RESULT.md`

## Prompt for Codex local

> Ejecuta únicamente el LOCAL_EXECUTION de WP-H1-02, LOCAL_ROUND 4. Lee `Docs/evidence/WP-H1-02/LOCAL_EXECUTION.md` en la rama canónica `work/wp-h1-02` y sigue ese contrato literalmente. No rediseñes ni repares. Las reparaciones Worker del wrapper Windows y EditMode ya están incluidas; debes validarlas ejecutando Unity real. Publica los commits/anchors exigidos y STOP.

This complete file is the authoritative local execution contract.

## Why round 4 exists

Round 1 discovered asynchronous GUI invocation, and Round 2 discovered that
`Start-Process -Wait` waited on the spawned process tree. Round 3 proved the
exact-process wait repair, then exposed a separate EditMode false green: Unity
returned 0 after receiving both `-runTests` and `-quit`, but produced no test
result file. The receiving Worker repaired the canonical wrapper before this
round:

- `scripts/h1-02-unity.ps1` launches Unity with `Start-Process -PassThru`, calls `WaitForExit()` on that exact returned process object, refreshes it and consumes its actual `ExitCode`;
- EditMode no longer supplies `-quit`; Unity Test Framework owns termination after the test run;
- every action now fails closed when its declared output is missing or empty;
- `scripts/h1-02-static-check.py` rejects `Start-Process -Wait`, guards the exact-process wait/exit shape and output postcondition, and includes causal negative controls for both.

Do not reinterpret or repair these changes locally. Round 4 exists to exercise
them with the real Editor and then continue the already-decided H1-02 proof.

## Role boundary

You are the delegated **local executor only**. Do not redesign, repair, change package strategy, expand scope, alter this manifest, run Worker pre-review, freeze, review, merge, DocSync or begin another WP. Any required product/configuration decision not encoded here returns `REMOTE_DECISION_REQUIRED`.

## Required environment

- Windows workstation with authenticated `git` and `gh` access to `Arkus0/Juego2`.
- Exact Unity Editor **`6000.3.24f1`**, changeset **`4e7b9b5b6244`**.
- Do not substitute the installed `6000.6.0f1`.
- Project: `Unity\ArkusUnity`.
- Direct test package intent: `com.unity.test-framework` version `1.6.0`.
- Render pipeline baseline: Built-in.
- PowerShell and Python 3 available.

Resolve Unity in this order only:

1. exact executable from non-empty `UNITY_EDITOR_PATH`;
2. `C:\Program Files\Unity\Hub\Editor\6000.3.24f1\Editor\Unity.exe`;
3. if absent and Unity Hub exists, the already-decided environment-only install command:

```powershell
& 'C:\Program Files\Unity Hub\Unity Hub.exe' -- --headless install --version 6000.3.24f1 --changeset 4e7b9b5b6244
```

No platform modules are required. If exact 6000.3.24f1 still cannot run non-interactively, return `ENVIRONMENT_BLOCKED`; do not change the project pin.

## SHA-chain preflight and deterministic cleanup of stopped-round dirt

Rounds 1 through 3 intentionally left **no local-result commits**, but the stopped local checkout may still contain uncommitted generated outputs/caches. Cleaning those bytes back to the anchored round-4 state is explicitly authorized and is not a repair.

1. Verify `git remote get-url origin` identifies `Arkus0/Juego2` and `gh auth status` succeeds.
2. Read PR #152 and locate the durable comment beginning `H1_LOCAL_HANDOFF_V1 WP=WP-H1-02 ROUND=4`; call its exact `MANIFEST_COMMIT_SHA` value `$manifestSha`.
3. `git fetch origin work/wp-h1-02` and require `git rev-parse origin/work/wp-h1-02 == $manifestSha`.
4. Checkout/reset the local repository exactly to `$manifestSha`:

```powershell
git reset --hard $manifestSha
git clean -fd -- Docs/evidence/WP-H1-02 Unity/ArkusUnity/Packages Unity/ArkusUnity/ProjectSettings
```

5. Delete only ignored Unity/editor-local caches from the retained project if present: `Library`, `Temp`, `Logs`, `Obj`, `UserSettings`.
6. Require `git status --porcelain --untracked-files=all` to be empty.
7. Require `git rev-parse HEAD^ == e9e022f871ba6b7da95f663956b07d0519b99b2d`.
8. Require `git diff --name-only HEAD^ HEAD` to output exactly `Docs/evidence/WP-H1-02/LOCAL_EXECUTION.md`.
9. Verify the pinned `ProjectVersion.txt` says `6000.3.24f1 (4e7b9b5b6244)`.
10. Read `Docs/workpacks/H1/WP-H1-02.md`, this manifest, `scripts/h1-02-unity.ps1` and `scripts/h1-02-static-check.py`. Do not reconstruct unrelated history.

Any SHA/identity/manifest-only mismatch is `REMOTE_DECISION_REQUIRED`.

## Evidence variables

From repository root define these variables so future generated evidence is not confused with pre-existing mandatory read inputs:

```powershell
$repo = (git rev-parse --show-toplevel)
$evidenceDir = Join-Path $repo 'Docs/evidence/WP-H1-02'
$effective = Join-Path $evidenceDir 'effective-inventory.json'
$second = Join-Path $evidenceDir 'second-import-inventory.json'
$editmode = Join-Path $evidenceDir 'editmode-results.xml'
$legal = Join-Path $evidenceDir 'PACKAGE_LEGAL_OBSERVATION.md'
$result = Join-Path $evidenceDir 'LOCAL_EXECUTION_RESULT.md'
$unity = $env:UNITY_EDITOR_PATH
if ([string]::IsNullOrWhiteSpace($unity) -or -not (Test-Path -LiteralPath $unity -PathType Leaf)) {
    $unity = 'C:\Program Files\Unity\Hub\Editor\6000.3.24f1\Editor\Unity.exe'
}
if (-not (Test-Path -LiteralPath $unity -PathType Leaf)) {
    throw 'ENVIRONMENT_BLOCKED: exact Unity 6000.3.24f1 executable unavailable.'
}
```

Record the Unity executable absolute path, Windows file/product version and SHA-256 for the final result.

## Allowed repository mutations

Before the result-summary commit, only these may change:

- `Unity\ArkusUnity\Packages\packages-lock.json`
- `Unity\ArkusUnity\ProjectSettings\**`
- under `$evidenceDir`: `effective-inventory.json`, `second-import-inventory.json`, `editmode-results.xml`, `PACKAGE_LEGAL_OBSERVATION.md`

The separate result-summary commit may add only `$result`.

Ignored `Library`, `Temp`, `Logs`, `Obj`, `UserSettings` and repo `artifacts` are local-only and must never be force-added.

Everything else is forbidden, including this manifest, package intent, Assets, scripts, workflows, H0 code/tests/configuration, architecture/process docs, bridge capabilities, scenes/gameplay/materialization and third-party art.

## A. Remote-owned checker on the anchored local start

Run:

```powershell
python scripts/h1-02-static-check.py --mode remote-prep --self-test
```

Require GREEN plus every declared defect-injection control RED, including `unity-exact-process-wait` and `unity-output-postcondition`. Any failure is `CANDIDATE_DEFECT`; do not repair locally.

## B. First clean configure/import through the repaired wrapper

With project caches absent, run:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/h1-02-unity.ps1 `
  -Action configure -UnityEditorPath $unity -OutputPath $effective `
  -LogPath (Join-Path $repo 'artifacts/h1-02/round4-configure.log')
```

This is the direct causal re-test of both observed wrapper failures, especially
the round-2 process-tree wait that remained blocked after Unity exited. Require:

- wrapper waits until Unity exits;
- wrapper prints `H1-02 Unity exit: 0` and `H1_02_UNITY_configure_GREEN`;
- effective inventory reports Unity `6000.3.24f1`, ForceText, Visible Meta Files and built-in pipeline;
- generated package lock exists;
- effective inventory contains Test Framework exactly `1.6.0`.

If Unity itself exits nonzero, or the repaired wrapper still fails to wait/capture its real code, return `CANDIDATE_DEFECT` without local repair.

## C. EditMode suite

Run:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/h1-02-unity.ps1 `
  -Action editmode -UnityEditorPath $unity -OutputPath $editmode `
  -LogPath (Join-Path $repo 'artifacts/h1-02/round4-editmode.log')
```

Require wrapper exit 0, at least one test, zero failures. Preserve evidence and stop `CANDIDATE_DEFECT` on compile/import/test failure.

## D. Effective package legal/notices observation

Inspect only the resolved local package cache produced by B. Locate exactly one `package.json` with name `com.unity.test-framework` and version `1.6.0`.

Write `$legal` with first line exactly `PACKAGE_LEGAL_OBSERVATION: COMPLETE`, then record resolved package name/version, cache directory basename, package.json SHA-256, literal license/licensesUrl values if present, and names+SHA-256 of package-root LICENSE/NOTICE/Third Party files if present.

Do not interpret terms. Missing exact package is `CANDIDATE_DEFECT`; absence of both license metadata and license/notices files is `REMOTE_DECISION_REQUIRED`.

## E. Clean second import

Create a temporary directory outside the repository, export `$manifestSha`, expand it, then overlay **only** the first-run retained package lock and ProjectSettings from the canonical repository. Do not copy caches.

Run the copied `scripts/h1-02-unity.ps1` with `-Action configure`, exact `$unity`, output to a temporary `second-import-inventory.json`, and a temporary log. Copy that inventory to `$second`.

Compare `$effective` and `$second` for exact equality of:

- `unityVersion`
- `serializationMode`
- `externalVersionControl`
- `renderPipeline`
- `packages`
- `assemblies`

Ignore only the two hash fields. Any listed-field difference is `CANDIDATE_DEFECT`.

## F. Fixed Arkus batch entry

Run:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/h1-02-unity.ps1 `
  -Action batch -UnityEditorPath $unity `
  -OutputPath (Join-Path $repo 'artifacts/h1-02/round4-batch-inventory.json') `
  -LogPath (Join-Path $repo 'artifacts/h1-02/round4-batch.log')
```

Require exit 0 and equality with `$effective` for the same six effective fields from E.

## G. Final mechanical check and mutation audit

Run:

```powershell
python scripts/h1-02-static-check.py --mode final --self-test
```

Require GREEN. Then capture complete `git status --porcelain --untracked-files=all`. Every visible changed/untracked repository path must be inside the allowlist above. No tracked Unity cache/build/editor-local paths may exist.

## Stop classifications

Use exactly one if the round cannot PASS:

- `EXPECTED_NEGATIVE_CONTROL` only for deliberate checker self-test fixtures;
- `CANDIDATE_DEFECT` when effective behavior falsifies a predeclared claim;
- `ENVIRONMENT_BLOCKED` when the exact editor/license/tool environment cannot execute;
- `REMOTE_DECISION_REQUIRED` when an unexpected package/config/generated-file/design choice or non-allowlisted mutation is required.

On any stop classification: do not repair; publish a durable PR #152 comment beginning `H1_LOCAL_STOP_V1 WP=WP-H1-02 ROUND=4` with classification, SHA chain, stopped action and observations, then STOP. Do not commit partial candidate outputs.

## PASS commit protocol

1. Verify the complete visible changed-file set is allowlisted.
2. Stage allowlisted project/evidence outputs from B-G **except** `$result` and commit them once with message `h1-02: record Unity 6.3 local baseline evidence round 4`. This exact commit is `PRODUCT_RESULT_SHA`; if there are genuinely no repository outputs it equals `MANIFEST_COMMIT_SHA`.
3. Push and verify remote `work/wp-h1-02` HEAD equals `PRODUCT_RESULT_SHA` before writing the summary.
4. Write `$result` with first line exactly `LOCAL_EXECUTION_RESULT: PASS` and include: WP, `LOCAL_ROUND: 4`, exact execution base, externally anchored manifest SHA, product-result SHA, repo/branch/PR, Windows version, Unity executable fingerprint, effective Unity version, package count/identity, commands+exit states, EditMode counts, first/second/batch parity, legal observation result, complete pre-product changed-file inventory, committed paths, mutation-allowlist compliance, evidence/log locations, and explicit statement that no local product/architecture decision was made.
5. Commit **only** `$result` next with message `h1-02: record local execution result round 4`. This exact direct child is `EVIDENCE_COMMIT_SHA`.
6. Push and verify remote branch HEAD equals `EVIDENCE_COMMIT_SHA`.
7. Add a durable PR #152 conversation comment beginning exactly `H1_LOCAL_RESULT_V1 WP=WP-H1-02 ROUND=4` and include execution base, manifest SHA, product-result SHA, evidence SHA, PASS, result filename, branch and `REMOTE_HEAD_VERIFIED: YES`.
8. STOP. Do not perform Worker pre-review or freeze.
