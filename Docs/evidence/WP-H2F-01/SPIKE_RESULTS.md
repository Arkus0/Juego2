# WP-H2F-01 spike results

All spikes ran on 2026-09-27 on the owner's workstation (Windows 11, NVIDIA RTX 3060, Unity 6000.3.24f1 `4e7b9b5b6244`). Each ran in a disposable, non-keeper workspace built from `spike_project/` (see `SPIKE_LEDGER.md`). The authoritative run is workspace **B**, which was rebuilt from scratch from the committed recipe. Workspace A (the first exploration) reproduced the same S01/S02/S07 measurements, and a third isolated copy was used only for the owner-supplied Game Creator 2 candidate.

Representative input for every spike: the ART-01 structural checkpoint `174d05d2`, rebuilt in the spike project. The spike reproduction check proved it is the same geometry (`results/s01_reproduction.json`): structural digest `a7f8534f…60501e` and 360 rows; the ART structural audit reported 2009 checks and 0 failures. No keeper CITY geometry was authored or modified.

## S01 — render pipeline

| Question | Result | Evidence |
|---|---|---|
| Does the representative ART/Quaternius content render under URP 17.3.0 without unsupported shaders or silent fallback? | **Yes**, for route A (ART palette → URP Lit) and route C (A plus Source normal maps). The shader audit is GREEN: 26 materials, 0 unsupported, 0 null bindings. | `results/s01_audit_urpA.json`, `s01_audit_urpC.json`, `captures/owner/S01_render.png` |
| Does the audit actually catch fallback? | **Yes.** Negative control: the pinned ART `.mat` files were restored while URP was active; the audit went RED (25 `Standard` materials) and the capture shows magenta. | `results/s01_negative_audit_standard_under_urp.json`, `captures/negative/s01_standard_materials_under_urp_magenta.png` |
| Can Unity's official converter be automated? | **Not through its documented batch API.** `Converters.RunInBatchMode(BuiltInToURP, …)` throws `MissingMethodException` on the abstract `Base2DMaterialUpgrader` (URP 17.3.0 on 6000.3.24f1). The public core `MaterialUpgrader.Upgrade` path with the URP upgrader set converted 25/25 materials. | `results/s01_converter.json` |
| Are the Source-native URP Shader Graph materials (route B) a usable baseline? | **No.** They compile and are supported, but on ART's derived meshes (road, bridge, render-host facades) they show atlas/wear-mask garbage and an off-palette orange timber. They are only valid on unmodified Source modules. | `captures/negative/s01_source_shadergraph_on_art_derived_meshes.png` |
| Was H1's `MI_Plaster` Shader Graph incompatibility real? | It is a Built-in problem only. Under URP 17.3 the Source `MI_Plaster` resolves to `Shader Graphs/M_Plaster`, supported, with all texture links intact through the original Source `.meta` GUIDs. | `results/s01_h1_slice_mi_plaster.json` |
| Is the native lighting suite sufficient for the fixed overcast day/evening? | **Yes at foundation level.** The native kit is: realtime soft directional light (shadows 70 m, 2 cascades), trilight ambient, exp² fog, a project-generated overcast panoramic sky, a Volume (neutral tonemapping, colour/white balance), SSAO, a realtime reflection probe, and URP decals. An evening variant (low warm key, darker ambient) keeps the F01 threshold readable. Baked GI/APV was not needed for this decision. | `captures/owner/S01_render.png` |
| Decals | They work (URP Decal renderer feature, `Shader Graphs/Decal`), but a projector bleeds onto vertical walls without decal rendering layers. | `captures/negative/s01_decal_probe_bleeds_onto_walls.png` |

## S02 — topography, linear features, navigation

A disposable copy of the W12 climb was built in a clean scene. It uses the ART centreline heights, a deliberate bend, an X1 (5.5 m) T-junction, a retaining wall, a Source fence and Terrain banks. The ART route itself was only read.

| Measure | Result |
|---|---|
| W12 traversable width (spline profile extrusion, measured by ray against the sole traversable collider) | 2.795 m at every sample (CITY-04 class 2.8, −0.005) |
| X1 width | 5.490 m (class 5.5, −0.010) |
| Junction surface continuity (1674 dense rays) | **0 holes, 0 steps** after the fix. The first attempt used an axis-aligned apron, which left **206 holes** at the bent mouth. The oracle caught it; the apron is now generated from W12's real first cross-section. |
| Terrain seam next to the kerb | 0.006 m |
| Fence along the curve (`SplineInstantiate`, Source `Prop_WoodenFence_Extension1`, 2.05 m module) | 14 instances. The instances live under a `HideAndDontSave` root and are **not serialized** in the scene. |
| NavMesh: ART route, bridge → Bar F01 interior | `PathComplete`, 59.0 m, through the public door |
| NavMesh: spline route | `PathComplete`, 37.7 m |
| Agent | The default Humanoid radius (0.5 m) cannot pass ART doors (1.00–1.16 m clear). A project agent of radius 0.28, climb 0.30, slope 40° passes. |
| Scenic surfaces | Terrain banks must be `NavMeshModifier` Not Walkable, otherwise the NavMesh floods them. |
| ProBuilder | A bounded threshold step is generated with `ShapeGenerator.GenerateStair`. Stripping to a plain mesh is possible only through the **internal** `StripProBuilderScripts.DoStrip` (no public API). |

Evidence: `results/s02_result.json`, `captures/owner/` (S02 is technical; its captures are in the workspace, `out/s02/`).

## S03 — river

| Question | Result |
|---|---|
| Native/free route on the real ART river (80 × 21.5 m, flow along X) | A project-owned URP water shader (`H2F01/StylizedWater`, ~100 lines HLSL, no textures, no plugin): depth tint, broken shoreline foam from the camera depth texture, flow-aligned procedural ripples, fresnel sky reflection. It is supported, the audit is GREEN, and the water surface changes over time (mean pixel delta 0.0056 between t = 0 and 1.5 s). |
| Requirement | The URP asset must enable the camera **depth texture**. |
| Comparison | ART's flat water converted to URP Lit is a light, off-palette blue plane. The shader route reads as dark, wet river with visible bank foam. |
| Paid Stylized Water 3 | Not needed; no critical gap. Not bought or imported. |

Evidence: `results/s03_result.json`, `captures/owner/S03_water.png`.

## S04 — nature

| Question | Result |
|---|---|
| Deterministic scatter of the admitted Quaternius Nature pieces on the real ART banks, with no-go masks for traversable surfaces, structures and the river | **2072 placements** (grass 968, fern 517, bush 264, flowering bush 161, rock 97, tree 65). Two runs give the same digest `5ebb6e06…5ba9f1`. **0 violations** of road, structure or river masks. The scale bands are copied from ART-01's reviewed placements (tree 0.62–0.76, fern 0.12–0.16, …). |
| Negative control | The same rule with the traversable mask disabled gives **9 road violations**, so the audit is causal. |
| First pass | Uniform weights and unit scale gave 6981 oversized pieces (`results/s04_first_pass_overdense_result.json`). ART's scale authority is required. |
| Native Terrain trees/details | Terrain prototypes render the prefab's **own** materials: the FBX-embedded pre-URP `Standard` materials render magenta. A renderer-based audit cannot see this, because terrain trees are not `Renderer` components. Prototypes also need the mesh on the prefab root; the FBX child's −90° X import rotation is ignored. With a root-baked URP derivative, trees render correctly. Terrain detail meshes (grass) **did not render** in the spike; left unresolved (see residuals). |

Evidence: `results/s04_result.json`, `captures/owner/S04_nature.png`, `captures/negative/s04_terrain_prototypes_fbx_standard_magenta.png`.

## S05 — architecture/window depth

| Question | Result |
|---|---|
| Real enterable threshold | Bar F01 exterior → threshold → interior renders under URP with a real opening, walkable NavMesh through the door (S02) and a real room. There is no fake playable entrance. |
| Non-enterable window (W12_left upper storey, open shutters, 1.61 × 1.58 m opening) | V1: flat glass (as built). V2: authored recess shell + curtain. V3: project-owned interior-mapping shader (`H2F01/InteriorWindow`, one material, no geometry). All supported, audit GREEN. |
| Observation | At street distance the variants differ subtly. V2 needs per-window geometry and showed seam artefacts until it was sized exactly to the opening. V3 gives depth at single-material cost but reads slightly pink (a palette-tuning residual for ART). First choice was invalid: a window behind closed shutters. |

Evidence: `results/s05_result.json`, `captures/owner/S05_window.png`.

## S06 — control and camera

The same scripted play run was used for each candidate. A virtual gamepad is fed through the real Input System, steering along NavMesh corners from the bridge (z −30) to the Bar F01 interior. Both candidates use the ART clothed citizen at the same 1.45 m/s civilian walk.

| Candidate | Arrived | Time | Camera clip frames | Head occluded frames | Jerk frames (>12°) | Stalls | Notes |
|---|---|---|---|---|---|---|---|
| Minimal project-owned: CharacterController + Input System action + Cinemachine 3 ThirdPersonFollow (obstacle avoidance r = 0.2) | yes | 40.4 s | 0 | 0 | 0 (max 10°, first-frame settle) | 0 | ~60-line controller. At the door the camera is pushed against the lintel and loses the body. Blank first frame before Cinemachine settles. |
| Game Creator 2 Core 2.19.61 (owner-supplied): GC2 Player + Main Camera + Third Person shot (radius 3, auto-align) | yes | 40.5 s | 0 | 25 (≈0.8 s at the door) | 0 (max 0°) | 0 | Very smooth, follows into the interior with good framing, full locomotion set retargets onto the citizen. The model swap needed a workaround: `ChangeModel` calls runtime-only `Destroy()`, which in edit mode leaves GC2's mannequin hiding the new model. |
| Unity Starter Assets ThirdPerson | — | — | — | — | — | — | **Not spiked: manual Asset Store acquisition not performed.** It packages the same component composition as the minimal route, which was spiked directly. |

Evidence: `results/s06_minimal_route_log.txt`, `results/s06_gc2_route_log.txt`, `captures/owner/S06_control_camera.png`.

GC2 Core installation facts are in `results/gc2_import_report.json`, `gc2_vendor_embedded_manifest.json` and `gc2_probe.txt`:

- The `.unitypackage` (sha256 `1e4f3ba0…2f3380b`) embeds a `Packages/manifest.json` that would **replace** the project manifest. It drops URP/Cinemachine/Splines and pins Input System 1.16.0 and AI Navigation 2.0.14.
- Extracting `Assets/**` only, GC2 Core compiles on the selected stack with **one** added built-in module (`com.unity.modules.physics2d`). The vendor pins were not needed.
- Core is one runtime assembly that includes Characters/Cameras **and** Variables (global/local), SaveLoad/Remember, VisualScripting Triggers/Actions/Hotspots and 19 managers.
- At editor load it writes `Assets/Plugins/GameCreator/Data/Resources/Settings/core.*.asset`.

## S07 — humanoid and motion

| Question | Result |
|---|---|
| Same-emitter clip coverage for civilian needs | UAL1 covers idle, talk, look-around, walk, formal walk, sit enter/idle/talk/exit, drink, interact, pick-up and the bar-counter set (`Counter_*`). UAL2 (owner-owned, CC0) adds fold-arms, rail-lean, carry-walk, step-up, yes/no and turn-180. **No named civilian clip is missing**, so Mixamo is not needed. The catalogue has 508 clips (`results/s07_clip_catalog.tsv`). |
| Retarget fit | Automatic Humanoid mapping on the **ART-derived clothed body** assigns `Hips = root` (ground-level bone). UAL1/UAL2 map `Hips = pelvis`. With an explicit shared mapping (`Hips=pelvis, Spine=spine_01, Chest=spine_02, UpperChest=spine_03`): standing hips 0.90 m, feet on ground within 0.6–1.1 cm, and the sitting pelvis over a real Source chair needs only a **0.048 m** seat anchor. Before the fix the computed anchor was 0.99 m (`results/s07_pose_report_before_hipsroot.json`). |
| Root motion (UAL1_RM `Walk_Loop`, play mode) | 0.99 m/s steady, heading drift **6.8°** from facing, vertical drift 0.04 m over 5 s. UAL "_Loop" clips must be imported with `loopTime` (otherwise root motion stops after one cycle). |
| In-place walk moved at the root-motion speed | Stance-foot slide p50 0.127 m/s at 0.975 m/s. This is indicative (heuristic stance detection). |
| Animation Rigging contact (TwoBoneIK, right hand to bar-counter height) | Hand-to-target 0.371 m → **0.000 m** at weight 1, when the target is placed from the *animated* shoulder. Targets placed from bind pose were out of reach (0.199 m residual). |
| ART material coupling | Calling ART-01's `Art01Materials.Remap/Get` after the URP conversion re-binds `Shader.Find("Standard")` and **persists** it (Skin/Coat/Shirt/… reverted on disk). |

Evidence: `results/s07_*`, `captures/owner/S07_humanoid.png`, `captures/negative/s07_art01materials_reverts_standard.png`.

## Workstation/tooling facts discovered (not candidate failures)

- Unity 6000.3.24f1 often crashes **after** `Batchmode quit successfully invoked`, with the method's work already saved. This was also recorded by ART-01; the bootstrap tolerates only that exact post-quit shape.
- Batch mode **without** `-quit` needs the `com.unity.editor.headless` entitlement, which this Personal license lacks. Play-mode spikes therefore run a windowed editor that the driver closes. This matters for H2F-03 automation.
- A new project created without `ProjectSettings` gets Unity's template packages injected into its manifest. The recipe ships `ProjectVersion.txt` to prevent that.
- New projects here default to `activeInputHandler = 0` (legacy only) even with the Input System package present.
