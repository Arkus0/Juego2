# Game Creator 2 Core — exact version and provisioning

## Identity

| Fact | Value | Source |
|---|---|---|
| Product | Game Creator 2 **Core** (Catsoft Works), owner-purchased Asset Store package | H2F-01 `ACQUISITION_AND_POLICY.md` (owner purchase 2026-09-27) |
| Package file | `Game Creator 2.unitypackage` in the owner's Asset Store cache (`%APPDATA%/Unity/Asset Store-5.x/Catsoft Works/Editor ExtensionsGame Toolkits/`) | this workstation |
| Package SHA-256 | `1e4f3ba0560eadb944b3cf3ca63f7095d95b47c55a94ee655f28a15952f3380b` (28,236,878 bytes) | `results/c01_gc2_import_report.json`; identical to H2F-01 `results/gc2_import_report.json` |
| Installed version | **2.19.61** (`Packages/Core/Editor/Version.txt`) | `results/c01_core_surface_windowed.json` |
| Editor | Unity 6000.3.24f1 | same |
| Assemblies installed | exactly `GameCreator.Runtime.Core`, `GameCreator.Editor.Core`, `GameCreator.Tests.Core` | same |
| License | Unity Asset Store EULA (per-seat, non-redistributable). The package carries no separate license file. | H2F-01 acquisition record |

Only **Core** is present. No separately licensed module (Inventory, Dialogue, Quests, Behavior, Perception, Melee, Shooter, Stats, …) was acquired, imported or probed. The verifier checks that the assembly set is exactly the three Core assemblies.

The package also embeds three nested installer packages under `Packages/Core/Examples/` (`GameCreator.Blockout`, `GameCreator.Characters`, `GameCreator.Examples`). They are **not** installed (they are not needed and are not admitted; see the matrix row `editor.hub_installs_updates`).

## Provisioning route (reproduced from an empty directory)

The probe workspace is rebuilt by `probe_project/bootstrap_01a.py`:

1. `base` runs the accepted H2F-01 recipe (`Docs/evidence/WP-H2F-01/spike_project/bootstrap.py init`) and the H2F-01 steps the probes need:
   - ART-01 structural reproduction (digest `a7f8534f…`, GREEN);
   - URP asset and renderer;
   - Route-A material conversion;
   - explicit Humanoid mapping;
   - `activeInputHandler = 2`.
2. `core` checks the package hash and size against the pinned values and refuses anything else. It then:
   - snapshots every `ProjectSettings/*` file plus `Packages/manifest.json`;
   - imports **`Assets/**` only** through the accepted H2F-01 `import_unitypackage.py` (the vendor `Packages/manifest.json` is excluded; 3,630 entries imported);
   - adds `com.unity.modules.physics2d` (the one built-in module Core needs, as H2F-01 found);
   - copies the Juego2-owned probe code.
3. `probes` runs C01 (batch, then windowed) and C02–C07 (windowed play). `collect` copies the curated outputs into `results/`.

Workspace **A** (`C:\Juego2-H2F01A-Core`) was used for the first exploration. The authoritative results come from workspace **B** (`C:\Juego2-H2F01A-Core-B`), which was rebuilt from an empty directory with the committed recipe. Both are outside the repository and disposable.

## Dependencies Core actually binds (as resolved in the probe lock)

`GameCreator.Runtime.Core` references (`results/c01_core_surface_windowed.json`, `assemblies[].references`):

| Reference | Satisfied by (probe lock `results/probe_packages_lock.json`) | Note for H2F-02 |
|---|---|---|
| `Unity.InputSystem` | `com.unity.inputsystem` 1.20.0 (selected stack) | Core does not need its vendor pin (1.16.0). |
| `Unity.Collections` | `com.unity.collections` 2.6.8, **transitive** (depth 2, via URP core) | Pin explicitly or assert in the lock. Today it is present only because URP pulls it in. |
| `Unity.Mathematics` | `com.unity.mathematics` 1.3.3, **transitive** (Splines/URP/Burst) | Same. |
| `Unity.TextMeshPro`, `UnityEngine.UI` | `com.unity.ugui` 2.0.0 (built-in, TMP included) | Already in the H2F-01 manifest. |
| `UnityEngine.Physics2DModule` | `com.unity.modules.physics2d` 1.0.0 | **Explicit addition required.** Without it Core does not compile (H2F-01). |
| `UnityEngine.AIModule`, `TerrainModule`, `TerrainPhysicsModule`, `AnimationModule`, `AudioModule`, `PhysicsModule`, `JSONSerializeModule`, `ImageConversionModule`, `TextRenderingModule`, `UIModule` | built-in modules already in the selected manifest | — |
| `UnityEditor.CoreModule` | editor-only code inside the runtime assembly behind `UNITY_EDITOR` guards | A player build was not exercised here; this is an H2F-02/H2F-03 build check. |

`GameCreator.Editor.Core` also references `UnityWebRequestModule` and `UnityWebRequestTextureModule`. These back the editor-only network features (Hub, Welcome images, version check) that are not admitted.

## Project-global state touched

The pre-import snapshot (`results/c01_project_state_before_gc2.json`) is compared twice: after the Assets-only import in batch mode (`results/c01_core_surface_batch.json`), and after the first windowed editor session (`results/c01_project_state_after_gc2_windowed.json`, `results/c01_core_surface_windowed.json`). Workspace B, the authoritative run:

- **Batch, after import and compile:** only `Packages/manifest.json` changed, which is the recipe's own `physics2d` addition. **Zero** GC2 settings assets exist yet (`generatedSettings: []`).
- **Windowed, first editor session:**
  - still only the manifest changed;
  - `ProjectSettings/PackageManagerSettings.asset` and `URPProjectSettings.asset` were **added**. These are standard files Unity writes on the first interactive session of a project;
  - GC2 created its four settings assets under `Assets/Plugins/GameCreator/Data/Resources/Settings/`: `core.general.asset`, `core.updates.asset`, `core.variables.asset` and `core.welcome.asset`. It creates them from deferred editor callbacks, so a single batch `-executeMethod` never creates them. `CORE_STATE_INVENTORY.md` classifies them.
- Workspace A (exploration) additionally showed `GraphicsSettings.asset`/`QualitySettings.asset` re-serialized after its windowed sessions. This is not attributed to GC2: workspace B did not reproduce it, and a static scan of the GC2 Editor/Runtime sources finds no write to `QualitySettings`, `GraphicsSettings`, `PlayerSettings`, `TagManager`, `EditorBuildSettings` or Package Manager settings.
- No tags, layers or scripting defines are added (`layers`, `tags`, `scriptingDefines` in the inventory).
