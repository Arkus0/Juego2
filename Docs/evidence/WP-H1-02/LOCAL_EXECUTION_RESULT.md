LOCAL_EXECUTION_RESULT: PASS

WP: `WP-H1-02`
LOCAL_ROUND: `4`
Repository: `Arkus0/Juego2`
Canonical PR: `#152`
Canonical branch: `work/wp-h1-02`

## Exact SHA chain

- EXECUTION_BASE_SHA: `e9e022f871ba6b7da95f663956b07d0519b99b2d`
- MANIFEST_COMMIT_SHA: `27602941ebc98dd2befa4dea7b2354242459d049`
- PRODUCT_RESULT_SHA: `ed7f0d00f4a6d38daee51c6661af50ff2a423cd4`
- Manifest-only direct child: verified
- Product-result direct child: verified
- Remote branch reached PRODUCT_RESULT_SHA before this summary was written: verified

The later EVIDENCE_COMMIT_SHA is intentionally anchored outside this file after
the result-summary-only commit exists.

## Environment fingerprint

- OS: Microsoft Windows 11 Pro `10.0.26200` (build `26200`)
- Unity executable: `C:\Program Files\Unity\Hub\Editor\6000.3.24f1\Editor\Unity.exe`
- Unity file version: `6000.3.24.5143451`
- Unity product version: `6000.3.24f1_4e7b9b5b6244`
- Unity executable SHA-256: `80CABDC2AA9A3DC8385410D14031F234F9437472157F82A1671CA73153E71EA0`
- Effective Unity version: `6000.3.24f1`
- Serialization mode: `ForceText`
- External version control: `Visible Meta Files`
- Render pipeline: `builtin`

## Commands and effective results

1. `python scripts/h1-02-static-check.py --mode remote-prep --self-test`
   returned 0. The ordinary check was GREEN and the `editor-pin`,
   `package-pin`, `h0-unity-reference`, `unity-cache-ignore`,
   `unity-exact-process-wait` and `unity-output-postcondition` controls all
   turned RED for their intended injected defects.
2. `scripts/h1-02-unity.ps1 -Action configure` returned 0, printed
   `H1-02 Unity exit: 0` and `H1_02_UNITY_configure_GREEN`, and produced the
   first effective inventory plus the resolved package lock and project
   settings.
3. `scripts/h1-02-unity.ps1 -Action editmode` returned 0, printed
   `H1-02 Unity exit: 0` and `H1_02_UNITY_editmode_GREEN`, and produced a
   non-empty NUnit result containing 5 tests, 5 passed and 0 failed.
4. The effective package cache contained exactly one
   `com.unity.test-framework` `1.6.0`; its package/legal byte observations are
   recorded in `PACKAGE_LEGAL_OBSERVATION.md`.
5. A Git archive of MANIFEST_COMMIT_SHA was expanded outside the repository,
   overlaid only with the retained first-run `packages-lock.json` and
   `ProjectSettings`, and configured without caches. Its inventory exactly
   matched the first import for `unityVersion`, `serializationMode`,
   `externalVersionControl`, `renderPipeline`, `packages` and `assemblies`.
6. `scripts/h1-02-unity.ps1 -Action batch` returned 0, printed
   `H1-02 Unity exit: 0` and `H1_02_UNITY_batch_GREEN`, and its inventory
   exactly matched those same six first-import fields.
7. `python scripts/h1-02-static-check.py --mode final --self-test` returned 0,
   with the final check GREEN and every declared negative-conformance control
   RED for its intended defect.

Effective inventory: 4 packages and 5 assemblies. Resolved packages:

- `com.unity.ext.nunit` `2.0.5` (`BuiltIn`)
- `com.unity.modules.imgui` `1.0.0` (`BuiltIn`)
- `com.unity.modules.jsonserialize` `1.0.0` (`BuiltIn`)
- `com.unity.test-framework` `1.6.0` (`BuiltIn`)

## Complete pre-product changed-file inventory

- `Docs/evidence/WP-H1-02/PACKAGE_LEGAL_OBSERVATION.md`
- `Docs/evidence/WP-H1-02/editmode-results.xml`
- `Docs/evidence/WP-H1-02/effective-inventory.json`
- `Docs/evidence/WP-H1-02/second-import-inventory.json`
- `Unity/ArkusUnity/Packages/packages-lock.json`
- `Unity/ArkusUnity/ProjectSettings/AudioManager.asset`
- `Unity/ArkusUnity/ProjectSettings/ClusterInputManager.asset`
- `Unity/ArkusUnity/ProjectSettings/DynamicsManager.asset`
- `Unity/ArkusUnity/ProjectSettings/EditorBuildSettings.asset`
- `Unity/ArkusUnity/ProjectSettings/EditorSettings.asset`
- `Unity/ArkusUnity/ProjectSettings/GraphicsSettings.asset`
- `Unity/ArkusUnity/ProjectSettings/InputManager.asset`
- `Unity/ArkusUnity/ProjectSettings/MemorySettings.asset`
- `Unity/ArkusUnity/ProjectSettings/MultiplayerManager.asset`
- `Unity/ArkusUnity/ProjectSettings/NavMeshAreas.asset`
- `Unity/ArkusUnity/ProjectSettings/Physics2DSettings.asset`
- `Unity/ArkusUnity/ProjectSettings/PresetManager.asset`
- `Unity/ArkusUnity/ProjectSettings/ProjectSettings.asset`
- `Unity/ArkusUnity/ProjectSettings/QualitySettings.asset`
- `Unity/ArkusUnity/ProjectSettings/TagManager.asset`
- `Unity/ArkusUnity/ProjectSettings/TimeManager.asset`
- `Unity/ArkusUnity/ProjectSettings/UnityConnectSettings.asset`
- `Unity/ArkusUnity/ProjectSettings/VFXManager.asset`
- `Unity/ArkusUnity/ProjectSettings/VersionControlSettings.asset`

All 24 paths were committed together at PRODUCT_RESULT_SHA. The complete
visible mutation set was a subset of the manifest allowlist. No Unity cache,
temporary editor output, `UserSettings`, manifest change, script change or
other repository path entered the product-result commit.

## Logs and evidence

- `artifacts/h1-02/round4-configure.log` (ignored), SHA-256
  `2264957A92F092B754797BB5C9FC9298B776F0CCBCDC29152DEDB9E67B50EC0A`
- `artifacts/h1-02/round4-editmode.log` (ignored), SHA-256
  `4A34A07C2727DF8C835F3505A899DFF29A946D8DE8C1A161FE1EDBDD75098B4B`
- temporary clean-second-import log, SHA-256
  `E0B0DD54981A4BAF86D916A137EB39FDC76967C54C2CF977F4FC69115250D690`
- `artifacts/h1-02/round4-batch.log` (ignored), SHA-256
  `C537E4F0C3C34012F82545792A2476AA73CA898A1AC96B3D03082697F2D077CD`
- retained evidence: `effective-inventory.json`,
  `second-import-inventory.json`, `editmode-results.xml`,
  `PACKAGE_LEGAL_OBSERVATION.md`, package lock and ProjectSettings.

The local execution made no architecture, product-semantic, package-strategy,
proof-threshold or scope decision. It executed only the externally anchored
round-4 contract and its predeclared mutation allowlist.
