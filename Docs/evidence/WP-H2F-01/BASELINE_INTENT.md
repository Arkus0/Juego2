# Recommended baseline intent for WP-H2F-02

This is **selection**, not adoption. H2F-02 owns exact admission, migration of the real project and H1 lifecycle classification. Versions are the ones exercised on Unity 6000.3.24f1 (`results/spike_packages_lock.json`); H2F-02 re-verifies them at admission.

## Selected stack

| Role | Selection | Exercised version / form |
|---|---|---|
| Render pipeline | URP (+ Shader Graph as its dependency) | `com.unity.render-pipelines.universal` 17.3.0 (core), shadergraph 17.3.0 |
| Materials | Project-owned URP/Lit materials in the ART palette. Source normal maps are optional where UVs are compatible (route C). Source Shader Graph materials are allowed only on unmodified Source modules. | S01 |
| Lighting / look | Native: realtime soft key light (shadow distance 70 m, 2 cascades), trilight ambient, exp² fog, project-generated overcast panoramic sky, global Volume (neutral tonemapping, colour/white balance), SSAO renderer feature, reflection probes, Decal renderer feature | S01 |
| Water | Project-owned URP water shader (depth tint, shoreline foam, flow ripples) | S03 (`spike_project/Assets/H2F01/Shaders/H2F01StylizedWater.shader`) |
| Non-enterable windows | Project-owned interior-mapping material; never on enterable openings | S05 (`H2F01InteriorWindow.shader`) |
| Linear features | Unity Splines centreline host + project-owned profile extrusion + junction apron/fillet generator + `SplineInstantiate` for repeated modules | `com.unity.splines` 2.9.1 |
| Terrain | Unity Terrain for scenic periphery/banks only; street network, junctions and edges stay authored/derived meshes | built-in module |
| Nature placement | Project-owned deterministic seeded scatter of ART-admitted prefabs with no-go masks (traversable, structure, river) and ART scale bands | S04 |
| Navigation | AI Navigation NavMeshSurface over the sole traversable colliders, project agent (radius 0.28, climb 0.30, slope 40°), `NavMeshModifier` Not Walkable on scenic surfaces | `com.unity.ai.navigation` 2.0.15 |
| Player control | CharacterController + Input System action map + project-owned locomotion/camera-root controller | `com.unity.inputsystem` 1.20.0 |
| Player camera | Cinemachine 3 CinemachineCamera + ThirdPersonFollow with obstacle avoidance | `com.unity.cinemachine` 3.1.7 |
| Characters | Quaternius Universal Base Characters derivatives (ART) as Humanoid with an **explicit** shared avatar mapping | S07 |
| Animation | UAL1 (baseline) + UAL2 (named gap fillers). In-place clips driven by controller/agent speed by default; root motion optional per clip. | S07 |
| Contact IK | Animation Rigging TwoBoneIK, targets resolved against the animated pose | `com.unity.animation.rigging` 1.4.1 |
| DCC | Blender for ART derivatives | Blender 5.2 on this workstation |
| Authoring surface | Juego2-owned Astra presets/wrappers over the above (spline/profile, scatter, material, agent, camera presets) | H2F-02 implements, H2F-03 S08 proves |

Not selected for the foundation (see `DISPOSITION_MATRIX.csv`):

- HDRP, weather/water/road/landscape paid suites, Terrain Tools, ProBuilder, Mixamo, Starter Assets (not spiked), Unity AI stack as authority.
- Game Creator 2 Core (owned; deferred to an explicit H3 decision).

## Requirements H2F-02 must carry (from spike findings)

1. **Material migration path.** URP 17.3.0's `Converters.RunInBatchMode` throws on 6000.3.24f1. Use the public `MaterialUpgrader` API or, preferably, generate URP materials explicitly.
2. **ART material coupling.** ART-01's `Art01Materials.Get/Remap` hard-binds `Shader.Find("Standard")` and silently reverts converted materials on disk. It must become pipeline-aware before any ART tool runs on the URP project. This is an ART-01 Phase B / H2F-02 boundary item.
3. **Pre-switch FBX materials.** Embedded materials of FBX imported before the pipeline switch stay `Standard`. Remap or reimport them. The material audit must also enumerate **Terrain tree/detail prototypes**, which renderer-based audits miss.
4. **Project settings that must be explicit:**
   - `activeInputHandler` (Both during the ART transition, Input System afterwards);
   - URP depth texture on (water);
   - decal rendering layers (decals bleed onto walls otherwise);
   - project NavMesh agent (radius 0.28 / climb 0.30 / slope 40);
   - a manifest pinned after project creation (new projects get template packages injected).
5. **Humanoid rig rule.** Every Base Characters derivative is imported with the explicit mapping `Hips=pelvis, Spine=spine_01, Chest=spine_02, UpperChest=spine_03`. Auto-mapping selects the ground `root`. UAL `_Loop` clips import with `loopTime`.
6. **Interaction anchors.** Seat anchors are ~0.05 m for Source chairs. IK contact targets are resolved against the animated pose and stay within ~0.5 m arm reach.
7. **Terrain prototypes.** They need root-level baked URP derivatives (mesh on the root, import rotation baked). Terrain detail meshes did not render in the spike and are unresolved.
8. **Junctions.** Splines provides no junction mesh. The project-owned generator must build aprons from real cross-sections. The dense surface-continuity check (0 holes / 0 steps) is the regression oracle; it caught an axis-aligned apron defect.
9. **External package import rule.** Never import a `.unitypackage` that contains `Packages/manifest.json`, `packages-lock.json` or other project-global state without inspection, a diff and an explicit merge policy. `spike_project/import_unitypackage.py` is the proven Assets-only route.

## H1 lifecycle classification hints for H2F-02

These are observations, not classifications; H2F-02 decides.

- Spline containers and knots are serialized scene state.
- `SplineInstantiate` output is **transient** (`HideAndDontSave` root, regenerated) unless baked.
- Profile/junction/scatter outputs are generated from inputs and seeds, and are deterministic: the scatter digest was identical across runs.
- TerrainData holds tree instances and detail maps as an asset.
- NavMesh data must be saved as an asset to survive play mode.
- The URP asset, renderer data and Volume profiles are project assets.
- GC2 (if ever adopted) writes `Resources/Settings/core.*.asset` at editor load.
