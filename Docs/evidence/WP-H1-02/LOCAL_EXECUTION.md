# WP-H1-02 — LOCAL_EXECUTION

WP_ID: `WP-H1-02`
LOCAL_ROUND: `1`
REPOSITORY: `Arkus0/Juego2`
CANONICAL_PR: `#152`
CANONICAL_BRANCH: `work/wp-h1-02`
EXECUTION_BASE_SHA: `11ca31563b170cdae140a6e672234801596caa45`
MANIFEST_COMMIT_ANCHOR: PR `#152` comment beginning `H1_LOCAL_HANDOFF_V1 WP=WP-H1-02 ROUND=1`
RESULT_FILE: `Docs/evidence/WP-H1-02/LOCAL_EXECUTION_RESULT.md`

## Prompt for Codex local

Copy/paste only this prompt into Codex on the Windows/Unity workstation:

> Ejecuta únicamente el LOCAL_EXECUTION de WP-H1-02. Lee `Docs/evidence/WP-H1-02/LOCAL_EXECUTION.md` en la rama canónica `work/wp-h1-02` y sigue ese contrato literalmente. No rediseñes ni repares. Si falta Unity 6000.3.24f1, usa únicamente la instalación headless exacta predeclarada en el manifest; no uses mi Unity 6000.6.0f1 como sustituto. Al terminar, publica los commits/anchors exigidos por el manifest y STOP.

If Codex was launched by opening this file directly, the sentence above is only a convenience: **this complete file is the authoritative local execution contract**.

## Role boundary

Execute this manifest literally as the delegated H1 local executor. Do not redesign, repair, change package strategy, expand scope, alter this manifest, run Worker pre-review, freeze, review, merge, DocSync or begin another WP. Any required decision not encoded here returns `REMOTE_DECISION_REQUIRED`.

## Required environment

- Windows workstation.
- Authenticated `git` + `gh` access to `Arkus0/Juego2`.
- Exact Unity Editor required by H1-02: **`6000.3.24f1`**, changeset **`4e7b9b5b6244`**.
- The already-installed `6000.6.0f1` is **not** an acceptable substitute for this WP.
- Retained Unity project: `Unity/ArkusUnity`.
- Expected direct test package: `com.unity.test-framework` version `1.6.0`.
- Render pipeline baseline: Built-in (`GraphicsSettings.currentRenderPipeline == null`).
- PowerShell and Python 3 must be available.

### Exact editor resolution / installation

Resolve the editor mechanically in this order:

1. If non-empty `UNITY_EDITOR_PATH` points to an existing executable whose path/version is exact `6000.3.24f1`, use it.
2. Otherwise use `C:\Program Files\Unity\Hub\Editor\6000.3.24f1\Editor\Unity.exe` if it exists.
3. Otherwise, if `C:\Program Files\Unity Hub\Unity Hub.exe` exists, run this predeclared environment-only installation command:

```powershell
& 'C:\Program Files\Unity Hub\Unity Hub.exe' -- --headless install --version 6000.3.24f1 --changeset 4e7b9b5b6244
```

No platform modules are required for this EditMode/toolchain baseline. After the command, require the exact editor executable at `C:\Program Files\Unity\Hub\Editor\6000.3.24f1\Editor\Unity.exe` (or an exact Hub-reported equivalent install path).

If exact `6000.3.24f1` still cannot be resolved or the headless installation/license state requires an interactive decision, report `ENVIRONMENT_BLOCKED`; do not change the project pin and do not use 6000.6.

## Preconditions

Before executing Unity actions:

1. Verify `git remote get-url origin` identifies `Arkus0/Juego2` and `gh auth status` succeeds.
2. Read PR `#152` and locate the durable external handoff comment beginning `H1_LOCAL_HANDOFF_V1 WP=WP-H1-02 ROUND=1`.
3. Obtain its exact `MANIFEST_COMMIT_SHA`, fetch `work/wp-h1-02`, and verify remote canonical branch HEAD equals that SHA.
4. Checkout that exact SHA and verify local `HEAD == MANIFEST_COMMIT_SHA`.
5. Verify `git rev-parse HEAD^ == 11ca31563b170cdae140a6e672234801596caa45`.
6. Verify `git diff --name-only HEAD^ HEAD` outputs exactly `Docs/evidence/WP-H1-02/LOCAL_EXECUTION.md`.
7. Verify the working tree is clean before execution.
8. Verify `Unity/ArkusUnity/ProjectSettings/ProjectVersion.txt` declares `6000.3.24f1 (4e7b9b5b6244)`.
9. Read `Docs/workpacks/H1/WP-H1-02.md`, this manifest, `scripts/h1-02-unity.ps1`, and `scripts/h1-02-static-check.py`. Do not reconstruct unrelated architecture/history.

Any identity/SHA/manifest-only mismatch is `REMOTE_DECISION_REQUIRED`.

## Allowed mutation paths

Before the result-summary commit, local execution may create/change **only**:

- `Unity/ArkusUnity/Packages/packages-lock.json`
- `Unity/ArkusUnity/ProjectSettings/**`
- `Docs/evidence/WP-H1-02/effective-inventory.json`
- `Docs/evidence/WP-H1-02/second-import-inventory.json`
- `Docs/evidence/WP-H1-02/editmode-results.xml`
- `Docs/evidence/WP-H1-02/PACKAGE_LEGAL_OBSERVATION.md`

The separate result-summary commit may add only:

- `Docs/evidence/WP-H1-02/LOCAL_EXECUTION_RESULT.md`

Ignored/generated local-only paths such as `Unity/ArkusUnity/Library/**`, `Temp/**`, `Logs/**`, `Obj/**`, `UserSettings/**` and repo `artifacts/**` may exist locally but must never be force-added or committed.

## Forbidden mutations

Everything not listed above is forbidden, including:

- this `LOCAL_EXECUTION.md` manifest;
- `Unity/ArkusUnity/Packages/manifest.json`;
- `Unity/ArkusUnity/Assets/**` and all repository-authored C#/asmdef/meta files;
- `.gitignore`, scripts, workflows, workpack/architecture/process docs;
- H0 `src/**`, `tests/**`, solution or package/build configuration;
- bridge public capabilities, scenes, gameplay, materialization or third-party art.

If Unity or a command proposes any non-ignored mutation outside the allowlist, STOP before staging it and return `REMOTE_DECISION_REQUIRED` with the complete changed-file inventory.

## Exact actions

From repository root set:

```powershell
$repo = (git rev-parse --show-toplevel)
$unity = $env:UNITY_EDITOR_PATH
if ([string]::IsNullOrWhiteSpace($unity) -or -not (Test-Path -LiteralPath $unity -PathType Leaf)) {
    $unity = 'C:\Program Files\Unity\Hub\Editor\6000.3.24f1\Editor\Unity.exe'
}
if (-not (Test-Path -LiteralPath $unity -PathType Leaf)) {
    throw 'ENVIRONMENT_BLOCKED: exact Unity 6000.3.24f1 executable is unavailable after the predeclared install step.'
}
```

Record for the final result file:

```powershell
(Get-Item $unity).FullName
(Get-Item $unity).VersionInfo | Format-List FileVersion,ProductVersion
Get-FileHash -Algorithm SHA256 $unity
```

### A. Repository-side proof on the anchored local start

Run:

```powershell
python scripts/h1-02-static-check.py --mode remote-prep --self-test
```

Require `H1_02_STATIC_CHECK_GREEN mode=remote-prep`, all declared injected defects RED, and `H1_02_STATIC_NEGATIVE_CONTROLS_GREEN`.

Any failure is `CANDIDATE_DEFECT`; do not repair locally.

### B. First clean import/configuration and effective inventory

Ensure these project-local caches are absent before first import: `Library`, `Temp`, `Logs`, `Obj`, `UserSettings`.

Run:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/h1-02-unity.ps1 `
  -Action configure `
  -UnityEditorPath $unity `
  -OutputPath (Join-Path $repo 'Docs/evidence/WP-H1-02/effective-inventory.json') `
  -LogPath (Join-Path $repo 'artifacts/h1-02/configure.log')
```

PASS observations required:

- exit 0;
- effective inventory exists and reports `unityVersion = 6000.3.24f1`;
- `serializationMode = ForceText`;
- `externalVersionControl = Visible Meta Files`;
- `renderPipeline = builtin`;
- `Unity/ArkusUnity/Packages/packages-lock.json` exists;
- effective package inventory contains `com.unity.test-framework` exactly `1.6.0`;
- no prompt or discretionary decision was required.

Unexpected package resolution, compile errors, prompts, or non-allowlisted generated repository files are `REMOTE_DECISION_REQUIRED` or `CANDIDATE_DEFECT` as appropriate; do not repair locally.

### C. EditMode batch proof

Run:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/h1-02-unity.ps1 `
  -Action editmode `
  -UnityEditorPath $unity `
  -OutputPath (Join-Path $repo 'Docs/evidence/WP-H1-02/editmode-results.xml') `
  -LogPath (Join-Path $repo 'artifacts/h1-02/editmode.log')
```

Require exit 0, at least one test and zero failures. A compile/import/test failure is `CANDIDATE_DEFECT`; preserve evidence and stop without repair.

### D. Effective package legal/notices observation

Inspect only the resolved local package cache produced by action B. Locate exactly one resolved package whose `package.json` has:

- `name == com.unity.test-framework`;
- `version == 1.6.0`.

Write `Docs/evidence/WP-H1-02/PACKAGE_LEGAL_OBSERVATION.md` containing:

- first line exactly `PACKAGE_LEGAL_OBSERVATION: COMPLETE`;
- resolved package name/version;
- package cache directory basename;
- `package.json` SHA-256;
- literal `license` value if present;
- literal `licensesUrl` value if present;
- names and SHA-256 values of package-root files matching `LICENSE*`, `NOTICE*` or `Third Party*` (case-insensitive).

Do not interpret or rewrite license terms. If the exact package cannot be found, return `CANDIDATE_DEFECT`. If neither license metadata nor any license/notices file exists, return `REMOTE_DECISION_REQUIRED`.

### E. Clean second import

Create a temporary directory outside the repository. Export the anchored `MANIFEST_COMMIT_SHA`, expand it, then overlay **only** the first-run retained project outputs before importing:

```powershell
$tempRoot = Join-Path ([System.IO.Path]::GetTempPath()) ('arkus-h1-02-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force -Path $tempRoot | Out-Null
$archive = Join-Path $tempRoot 'repo.zip'
git archive --format=zip --output=$archive $manifestSha
$copy = Join-Path $tempRoot 'repo'
Expand-Archive -LiteralPath $archive -DestinationPath $copy

Copy-Item -LiteralPath (Join-Path $repo 'Unity/ArkusUnity/Packages/packages-lock.json') `
  -Destination (Join-Path $copy 'Unity/ArkusUnity/Packages/packages-lock.json') -Force
Copy-Item -Path (Join-Path $repo 'Unity/ArkusUnity/ProjectSettings/*') `
  -Destination (Join-Path $copy 'Unity/ArkusUnity/ProjectSettings') -Recurse -Force

powershell -NoProfile -ExecutionPolicy Bypass -File (Join-Path $copy 'scripts/h1-02-unity.ps1') `
  -Action configure `
  -UnityEditorPath $unity `
  -OutputPath (Join-Path $copy 'second-import-inventory.json') `
  -LogPath (Join-Path $copy 'second-import.log')
Copy-Item -LiteralPath (Join-Path $copy 'second-import-inventory.json') `
  -Destination (Join-Path $repo 'Docs/evidence/WP-H1-02/second-import-inventory.json') -Force
```

Do **not** copy `Library`, `Temp`, `Logs`, `Obj`, `UserSettings` or other caches into the temporary project.

Compare `effective-inventory.json` and `second-import-inventory.json` structurally for exact equality of these fields:

- `unityVersion`;
- `serializationMode`;
- `externalVersionControl`;
- `renderPipeline`;
- `packages`;
- `assemblies`.

Ignore `manifestSha256` and `packagesLockSha256` for this parity assertion. Any listed-field difference is `CANDIDATE_DEFECT`; do not normalize it away.

### F. Canonical non-interactive Arkus batch entry

Run:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/h1-02-unity.ps1 `
  -Action batch `
  -UnityEditorPath $unity `
  -OutputPath (Join-Path $repo 'artifacts/h1-02/batch-inventory.json') `
  -LogPath (Join-Path $repo 'artifacts/h1-02/batch.log')
```

Require exit 0. Compare its inventory to `effective-inventory.json` for the same six effective fields from action E; require equality.

### G. Final mechanical checks and mutation audit

Run:

```powershell
python scripts/h1-02-static-check.py --mode final --self-test
```

Require GREEN. Then collect the complete `git status --porcelain --untracked-files=all` inventory. Every visible changed/untracked repository path must match the allowlist above. Verify no tracked Unity cache/build/editor-local paths exist.

## Stop classifications

Use exactly one when the round cannot PASS:

- `EXPECTED_NEGATIVE_CONTROL` only for the deliberately injected checker self-tests;
- `CANDIDATE_DEFECT` when effective behavior falsifies a predeclared H1-02 claim;
- `ENVIRONMENT_BLOCKED` when exact Unity/editor/license/tool availability prevents execution;
- `REMOTE_DECISION_REQUIRED` when execution reveals a package/configuration/generated-file/design decision or would require a mutation outside the allowlist.

Do not repair locally after a stop condition.

## Commit protocol on PASS

1. Capture the complete changed-file inventory and verify every entry is allowlisted.
2. Stage all allowlisted project/evidence outputs from actions B-G **except** `LOCAL_EXECUTION_RESULT.md`.
3. Commit them in one commit with message `h1-02: record Unity 6.3 local baseline evidence` and record this exact SHA as `PRODUCT_RESULT_SHA`. If there are genuinely no repository mutations, `PRODUCT_RESULT_SHA = MANIFEST_COMMIT_SHA`.
4. Push to `work/wp-h1-02` and verify remote branch HEAD equals `PRODUCT_RESULT_SHA` **before** writing the summary.
5. Create `Docs/evidence/WP-H1-02/LOCAL_EXECUTION_RESULT.md` containing at minimum:
   - first line exactly `LOCAL_EXECUTION_RESULT: PASS`;
   - `WP: WP-H1-02`, `LOCAL_ROUND: 1`;
   - exact `EXECUTION_BASE_SHA: 11ca31563b170cdae140a6e672234801596caa45`;
   - exact externally anchored `MANIFEST_COMMIT_SHA`;
   - exact `PRODUCT_RESULT_SHA`;
   - repository/branch/PR identity;
   - Windows version;
   - Unity executable absolute path, Windows file/product version and SHA-256;
   - effective `Application.unityVersion`;
   - package/platform fingerprints and resolved package count;
   - each action/command and exit state;
   - EditMode test/failure counts;
   - first/second/batch inventory parity result;
   - package legal/notices observation path/result;
   - complete visible changed-file inventory before product commit;
   - complete paths committed in the product/output commit;
   - explicit mutation-allowlist compliance and statement that no forbidden mutation was committed;
   - evidence paths and local ignored-log paths;
   - explicit statement that no architecture/product decision was made locally.
6. Commit **only** `LOCAL_EXECUTION_RESULT.md` next with message `h1-02: record local execution result`. Record that exact direct-child SHA as `EVIDENCE_COMMIT_SHA`.
7. Push it and verify remote `work/wp-h1-02` HEAD equals `EVIDENCE_COMMIT_SHA`.
8. Only after that verification add a durable PR #152 conversation comment beginning exactly:

```text
H1_LOCAL_RESULT_V1 WP=WP-H1-02 ROUND=1
```

and include `EXECUTION_BASE_SHA`, `MANIFEST_COMMIT_SHA`, `PRODUCT_RESULT_SHA`, `EVIDENCE_COMMIT_SHA`, result, result path, branch and `REMOTE_HEAD_VERIFIED: YES`.
9. STOP and return control to the remote Worker. Do not perform Worker pre-review or freeze.
