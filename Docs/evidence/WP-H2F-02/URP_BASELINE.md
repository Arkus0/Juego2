# URP project baseline

The canonical render baseline of `Unity/ArkusUnity`, applied by `J2FoundationBaseline.Apply()` and checked by `J2FoundationBaseline.Verify()` (batch: `Juego2.Foundation.Editor.J2FoundationBatch.Apply` / `.Verify`). The values come from the H2F-01 S01 selection. H2F-02 owns this **baseline**, not final scene lighting.

## Pipeline, tiers and renderer

| Setting | Value | Where |
|---|---|---|
| Default pipeline | `J2_URP_High` (URP 17.3.0) | `GraphicsSettings.defaultRenderPipeline` |
| Quality tiers | exactly `Low` → `J2_URP_Low`, `High` → `J2_URP_High`; default `High` on every platform | `QualitySettings` |
| Renderer | Forward (URP default), one renderer per tier | `J2_Renderer_High`, `J2_Renderer_Low` |
| HDR | on (both) | pipeline assets |
| MSAA | 4x High / off Low | pipeline assets |
| Depth texture | **on** (the water depth fade needs it; H2F-01 S03) | pipeline assets |
| Opaque texture | off | pipeline assets |
| Main light shadows | on, **soft**. High: **70 m**, **2 cascades**. Low: 40 m, 1 cascade. | pipeline assets |
| Rendering layers | on (`m_SupportsLightLayers`). TagManager layer 1 = `DecalReceiver`. | pipeline assets + TagManager |
| SSAO | renderer feature on High, absent on Low | renderer data |
| Decals | `DecalRendererFeature` on both renderers, **decal layers on** (a projector otherwise bleeds onto walls; H2F-01 S01) | renderer data |
| Colour space | **Linear** | PlayerSettings |
| URP global settings | `J2_URPGlobalSettings` + `J2_DefaultVolumeProfile`, moved from the Assets root into `Assets/Juego2/Foundation/Rendering` (GUIDs preserved) | Graphics settings map |

## Look, colour and Volume (`J2_LookPreset`)

- Sun: colour (0.83, 0.89, 0.93), intensity 0.9, rotation (43, −28, 0), soft shadows.
- Ambient: trilight (sky 0.62/0.67/0.70, equator 0.50/0.54/0.54, ground 0.30/0.32/0.30).
- Fog: exponential squared, density 0.012, colour (0.66, 0.70, 0.72).
- Sky: `Skybox/Panoramic` with the project-generated overcast equirect `J2_OvercastSky.png`. No external HDRI.
- Global Volume `J2_LookVolumeProfile`: Tonemapping **Neutral**; Color Adjustments post-exposure +0.25, contrast +8, saturation −12; White Balance temperature −6.
- Applied per scene by `J2FoundationLook.ApplyTo(scene, preset)`. It writes RenderSettings, the `J2_Sun` light and one `J2_GlobalVolume`, and turns on post-processing for cameras. This state lives in the owner's lighting sidecar scene (`H1_LIFECYCLE_MATRIX.csv` `urp.scene_look`).

## Policies

| Topic | Policy |
|---|---|
| GI / probes | Realtime key light + trilight ambient + realtime reflection probes are the baseline. Light Probe Groups are allowed. Baked lightmaps, LightingDataAsset and probe bakes are **generated** state that a scene owner may bake later (`gi.bakes`). |
| APV | **Off.** Both assets use `LegacyLightProbes`. Enabling APV is an H2F amendment. `Verify` fails with `J2_APV_ENABLED`. |
| Shadows | soft realtime main-light shadows as above. Additional lights per-pixel (URP default). |
| SSAO | High tier only. |
| Decals | URP decals. A projector must target `DecalReceiver`. Traversable realizations add that bit to their renderer (`J2LinearProfile.surfaceRenderingLayers`), so decals land on roads and never on walls. |
| Transparency / cutout / vegetation | URP/Lit Alpha Clipping for cutout foliage (ART materials, e.g. Leaves). Transparent URP/Lit for glass. Terrain trees/details are not used; vegetation is the scatter role. |
| Water | project shader `Juego2/StylizedWater` (depth tint, shoreline foam, flow ripples); needs the depth texture. |
| Non-enterable windows | project shader `Juego2/InteriorWindow` (interior mapping). Never on an enterable opening. |
| Materials | URP/Lit project materials in the ART palette (route C of H2F-01 S01). Source Shader Graph materials only on unmodified Source modules. |
| Shader Graph | 17.3.0 is admitted as URP's dependency. Project Shader Graphs target URP. |
| Scene/camera assumptions | One MainCamera-tagged URP camera with post-processing on (the GC2 `MainCamera` under S06), near clip 0.05, FOV 55. |

## Migration and unsupported-shader handling

- **Recipe:** `J2MaterialMigration.UpgradeBuiltinMaterials(roots)`.
  - It reimports every model whose embedded materials are not URP. Models imported before the switch keep Standard until reimported (H2F-01 finding).
  - It upgrades built-in-only `.mat` assets through the public `MaterialUpgrader` API. URP 17.3.0's `Converters.RunInBatchMode` throws on 6000.3.24f1 (H2F-01 S01), so it is never used.
  - Colours carry over (`_Color` → `_BaseColor`).
- **Audit:** `J2ShaderAudit` classifies every material a scene renders, including Terrain material/tree/detail prototypes (renderer-only audits miss them; H2F-01 finding), and every material asset under given roots:
  - `URP_OK` means the subshader has `RenderPipeline=UniversalPipeline`, or a pass has a URP LightMode, or every pass is untagged (URP draws those as `SRPDefaultUnlit`, e.g. TextMesh `GUI/Text Shader`), or the shader is a pipeline-agnostic builtin (`Skybox/`, `UI/`, `Sprites/`).
  - Anything else is `BUILTIN_ONLY`, `MISSING_SHADER` or `UNSUPPORTED`, and the audit is not green. Nothing is hidden. Declared fixtures are listed separately, never silently dropped.
- **Retained project content:** 15 CITY-04 greybox materials migrated to URP/Lit. `City04GreyboxBuilder` now creates URP/Lit materials under URP instead of `Standard`. Foundation + CITY audit: 22 materials, all `URP_OK` (`results/real_project_material_migration.json`).
- **H1 representative-slice materials** (`Assets/Arkus/H1/SourceSlice`, restored from the private vault, git-ignored) are H1 proof fixtures, not production content. H2F-02 does not mutate them. Under URP the Source `MI_Plaster.mat` resolves URP's `AssetVersion` script, which is the one catalogue change re-baselined in `H1_COMPATIBILITY.md`.

## Evidence

- Real project: `J2FoundationBatch.Verify` → 0 findings with Core provisioned; 33/33 Juego2 EditMode tests.
- Clean restoration from committed bytes only (evidence workspace, Core absent): 0 findings, URP active, foundation tests green, every committed byte unchanged.
- Representative content (ART-01 `174d05d2` + vault): route, content and worldbuilding audits. Captures under `captures/owner/` are for the owner's visual inspection.
