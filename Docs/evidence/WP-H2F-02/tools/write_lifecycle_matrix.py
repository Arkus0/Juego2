"""Writes Docs/evidence/WP-H2F-02/H1_LIFECYCLE_MATRIX.csv (one row per material selected-foundation state family).

Kept as a script so the matrix is reviewable as data; the exact-SHA verifier checks the CSV itself.
"""

import csv
from pathlib import Path

OUT = Path(__file__).resolve().parents[1] / "H1_LIFECYCLE_MATRIX.csv"
H = ["family_id", "family", "introduced_by", "lifecycle_class", "host", "source_of_truth", "materialize",
     "observe_drift", "reconcile", "rematerialize_clean_rebuild", "witness"]
NOTH1 = ("Not an H1 authoring unit (ADR-H1-003: packages and ProjectSettings are bootstrap authority) and outside "
         "the H1 catalogue universe, so never reported as H1 drift")
SIDECAR = ("owner retained sidecar scene or prefab outside Assets/Arkus/H1/Managed*; never an H1-managed scene node "
           "(H1 allowlists only Transform, MeshRenderer, Animator and the canonical link, so this component inside a "
           "managed scene would be surfaced as unmanaged drift, by design)")
ROWS = []


def row(*values):
    assert len(values) == len(H), values[0]
    ROWS.append(dict(zip(H, values)))


row("pkg.manifest_lock", "Packages/manifest.json + packages-lock.json (URP, Shader Graph, Input System, Splines, AI Navigation, Animation Rigging, uGUI, Collections, Mathematics, built-in modules)",
    "H2F-02", "RETAINED_PROJECT_CONFIGURATION", "Unity/ArkusUnity/Packages (committed)", "committed manifest + lock; J2PackageBaseline.DirectPackages",
    "Unity Package Manager resolves the pinned set on open", NOTH1 + "; J2FoundationBaseline.Verify fails with J2_PACKAGE_* / J2_LOCK_*",
    "never from Unity; a change is an H2F amendment", "clean import resolves the identical lock", "results/a1_clean_absent_report.json")
row("settings.project", "ProjectSettings of the baseline: GraphicsSettings (default pipeline, URP global settings), QualitySettings Low/High, TagManager rendering layer DecalReceiver, Linear colour space, activeInputHandler=Both, NavMesh agent, URP debug input axes, ShaderGraphSettings",
    "H2F-02", "RETAINED_PROJECT_CONFIGURATION", "Unity/ArkusUnity/ProjectSettings (committed)", "J2FoundationBaseline.Apply (idempotent) + the committed files",
    "present on open", NOTH1 + "; Verify codes J2_QUALITY_*, J2_COLOR_SPACE_NOT_LINEAR, J2_INPUT_HANDLER, J2_NAVMESH_AGENT, J2_RENDERING_LAYER_NAME",
    "never from Unity; rerun Apply after an H2F amendment", "survives; clean import leaves every committed byte unchanged", "results/summary.json")
row("urp.pipeline_assets", "J2_URP_High/Low, J2_Renderer_High (SSAO + Decals with decal layers) / J2_Renderer_Low (Decals), J2_URPGlobalSettings, J2_DefaultVolumeProfile",
    "H2F-02", "RETAINED_PROJECT_CONFIGURATION", "Assets/Juego2/Foundation/Rendering (committed)", "J2FoundationBaseline.Apply updates in place (stable GUIDs)",
    "GraphicsSettings and QualitySettings reference them", NOTH1 + "; Verify J2_PIPELINE_*, J2_SSAO_POLICY, J2_DECAL_LAYERS_OFF, J2_DEPTH_TEXTURE_OFF, J2_APV_ENABLED; H1Bootstrap requires the exact asset path",
    "never from Unity", "survives; the hosted H1 suites run on it", "results/b1_gc2_report.json")
row("urp.look_preset", "J2_LookPreset, J2_LookVolumeProfile (neutral tonemapping, colour adjustments, white balance), J2_Sky_Overcast + generated J2_OvercastSky.png",
    "H2F-02", "RETAINED_REALIZATION", "Assets/Juego2/Foundation/Rendering (committed preset assets)", "the preset assets",
    "J2FoundationLook.ApplyTo(scene, preset)", "outside the H1 catalogue universe; Verify J2_LOOK_PRESET",
    "never from Unity; final lighting is ART/CITY tuning", "survives (committed)", "captures/owner/route_f01_exterior.png")
row("urp.scene_look", "Per-scene RenderSettings (trilight ambient, exp2 fog, skybox), J2_Sun light and J2_GlobalVolume object written by J2FoundationLook",
    "H2F-02", "RETAINED_REALIZATION", "the owner lighting/look sidecar scene, loaded as the active scene with H1-managed scenes additive; " + SIDECAR,
    "J2_LookPreset + owner tuning", "J2FoundationLook.ApplyTo on the sidecar",
    "not observed by H1 while in the sidecar; a Volume or Light placed in a managed scene is unmanaged drift", "never into canonical state",
    "survives in the sidecar scene; re-applicable from the preset", "results/representative.json")
row("gi.bakes", "Lightmaps, LightingDataAsset, reflection-probe and light-probe bakes (none in the baseline, which is realtime key light + trilight ambient + realtime probes)",
    "H2F-02 policy", "GENERATED_TRANSIENT", "Library or generated assets beside the owning sidecar scene", "the sidecar scene lighting and bake settings",
    "Unity lighting bake run by the scene owner", "never canonical; a stale bake is rebaked", "never reconciled", "disposable; rebaked from the sidecar",
    "URP_BASELINE.md")
row("gi.apv", "Adaptive Probe Volumes", "H2F-02 policy", "NOT_ADMITTED", "none (both URP assets use LegacyLightProbes)", "J2FoundationBaseline (m_LightProbeSystem)",
    "n/a", "Verify J2_APV_ENABLED fails closed", "n/a", "n/a", "results/b1_gc2_report.json")
row("shaders.project", "Juego2/StylizedWater and Juego2/InteriorWindow shaders (promoted from the H2F-01 S03/S05 spikes)", "H2F-02", "RETAINED_PROJECT_CONFIGURATION",
    "Assets/Juego2/Foundation/Shaders (committed)", "the shader sources", "compiled by Unity; owner materials reference them",
    "J2ShaderAudit requires URP_OK; EditMode J2RenderingTests.ProjectShaders_AreSupportedUrpShaders", "never", "survives",
    "captures/owner/worldbuilding_water_window.png")
row("materials.retained", "Retained URP material assets: Juego2 foundation materials and the CITY-04 greybox materials migrated Standard -> URP/Lit with colour preserved",
    "H2F-02", "RETAINED_REALIZATION", "Assets/Juego2/** and Assets/Arkus/CITY/Materials (committed)", "committed .mat bytes", "referenced by owner scenes",
    "J2ShaderAudit fails on BUILTIN_ONLY / MISSING_SHADER / UNSUPPORTED (J2RenderingTests.RetainedProjectMaterials_AreAllUrp)", "never from Unity",
    "survives; clean-import audit green", "results/real_project_material_migration.json")
row("materials.importer_embedded", "Materials embedded in FBX/OBJ imports (model sub-assets); models imported before the switch keep built-in materials until reimported",
    "H2F-02", "GENERATED_TRANSIENT", "Library import artefacts", "model bytes + importer settings (ImportViaMaterialDescription)",
    "reimport under URP (J2MaterialMigration reimports models whose embedded materials are not URP)", "J2ShaderAudit over model sub-assets", "never",
    "a clean Library import produces URP/Lit", "results/representative.json")
row("h1.catalogue_snapshot", "H1 committed catalogue snapshot Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json and its pinned fingerprint in tests/Arkus.Harness.Tests/H1CatalogueTests.cs",
    "H1-04; re-baselined by H2F-02 (one MI_Plaster dependency on the URP AssetVersion.cs script)", "H1_MANAGED_PROJECTION",
    "H1 bridge-owned catalogue files (committed)", "the reviewed effective inventory", "consumed by H1 plan and materialize",
    "H1 ValidateCatalogueSnapshot fails closed with projection.catalogue-snapshot-stale on any difference",
    "only by an explicit reviewed re-baseline (WP-H1-11 precedent)", "hosted H1-07..11 re-run green on the re-baselined snapshot", "H1_COMPATIBILITY.md")
row("h1.managed_projection", "H1 managed scenes, prefabs and markers under Assets/Arkus/H1/Managed* and their generations", "H1 (unchanged)", "H1_MANAGED_PROJECTION",
    "Assets/Arkus/H1/Managed* (git-ignored generations)", "canonical Arkus state + binding intent", "accepted H1 materialize", "accepted H1 observe and drift",
    "accepted H1 reconcile (proposal -> H0 apply)", "accepted H1 rematerialize and clean rebuild", "H1_COMPATIBILITY.md")
row("splines.containers", "Unity Splines SplineContainer components and knots (road, lane and path centrelines)", "H2F-02 (Splines 2.9.1)", "RETAINED_REALIZATION", SIDECAR,
    "CITY/ART-authored realization input; canonical road intent stays with Arkus/CITY", "authored in the sidecar",
    "the inputDigest of every realization changes when a knot moves (J2LinearRealizer.InputDigest)", "never into canonical state; CITY/ART edit, then re-realize",
    "survives in the sidecar", "J2WorldbuildingTests.LinearProfile_IsDeterministicAndDetectsStaleInputs")
row("splines.instantiate", "SplineInstantiate generated instances", "Splines 2.9.1", "GENERATED_TRANSIENT",
    "HideAndDontSave root regenerated by SplineInstantiate (H2F-01 finding)", "the SplineInstantiate component settings", "UpdateInstances",
    "not serialized, not observed", "never", "regenerated; foundation tools do not rely on it (bake into a realization before keeper use)",
    "Docs/evidence/WP-H2F-01/BASELINE_INTENT.md")
row("linear.realization", "J2LinearRealizer / J2JunctionRealizer output: traversable surface (sole collider), kerbs, retaining walls, apron and fillets under a J2GeneratedRealization root",
    "H2F-02", "RETAINED_REALIZATION", "generated subtree stored in the owner sidecar scene next to its spline; " + SIDECAR,
    "spline + J2LinearProfile + generator id and version", "J2LinearRealizer.Realize / J2JunctionRealizer.Realize",
    "marker inputDigest/outputDigest detect stale inputs and hand edits; surface continuity oracle (0 holes, 0 steps)", "never; change the spline or profile and re-realize",
    "deterministic: identical output digest on regeneration; survives in the sidecar", "results/representative.json")
row("worldbuilding.profiles", "J2LinearProfile and J2ScatterProfile preset assets", "H2F-02", "RETAINED_REALIZATION", "owner preset assets (CITY/ART/fixture)",
    "the preset asset", "consumed by the realizers", "profile JSON is part of every inputDigest", "never", "survives", "ADAPTER_BOUNDARY.md")
row("scatter.realization", "J2ScatterRealizer placements of admitted prefabs (seeded; excludes generated roads/edges and J2ScatterExclusion volumes)", "H2F-02",
    "RETAINED_REALIZATION", "generated subtree in the owner sidecar scene; " + SIDECAR, "area + J2ScatterProfile + seed + no-go colliders", "J2ScatterRealizer.Realize",
    "output digest identical for the same seed, different for another seed", "never", "regenerated identically from the seed",
    "J2WorldbuildingTests.Scatter_IsSeedDeterministicAndKeepsOutOfNoGoAreas")
row("terrain", "Unity Terrain component, TerrainData asset and TerrainLayers for scenic ground only", "H2F-02 (built-in terrain module)", "RETAINED_REALIZATION",
    "owner sidecar scene + committed TerrainData asset", "the TerrainData asset", "authored by the scene owner",
    "J2ShaderAudit covers terrain material, tree and detail prototypes", "never", "survives (committed asset)",
    "J2RenderingTests.ShaderAudit_ReportsATerrainPrototypeThatRendererAuditsMiss")
row("terrain.vegetation", "Terrain tree instances and detail maps", "H2F-01 decision", "NOT_ADMITTED", "none: vegetation is the scatter role", "n/a", "n/a",
    "a tree or detail prototype is still audited if someone adds one", "n/a", "n/a", "Docs/evidence/WP-H2F-01/BASELINE_INTENT.md")
row("navmesh.surface", "AI Navigation NavMeshSurface and NavMeshModifier (Not Walkable on scenic surfaces)", "H2F-02 (AI Navigation 2.0.15)", "RETAINED_REALIZATION", SIDECAR,
    "the traversable realization colliders + component settings", "authored in the sidecar", "not observed by H1 in the sidecar", "never", "survives in the sidecar",
    "ADAPTER_BOUNDARY.md")
row("navmesh.data", "Baked NavMeshData", "AI Navigation 2.0.15", "GENERATED_TRANSIENT",
    "asset saved beside the sidecar scene (an asset is needed to survive play mode; H2F-01 finding)", "traversable colliders + project agent",
    "NavMeshSurface.BuildNavMesh", "stale data is rebaked", "never", "disposable; rebaked", "Docs/evidence/WP-H2F-01/SPIKE_RESULTS.md")
row("navmesh.agent", "Project NavMesh agent: radius 0.28, height 1.8, climb 0.30, slope 40", "H2F-02", "RETAINED_PROJECT_CONFIGURATION",
    "ProjectSettings/NavMeshAreas.asset (committed)", "J2FoundationBaseline", "present on open", "Verify J2_NAVMESH_AGENT", "never", "survives",
    "results/b1_gc2_report.json")
row("import.humanoid", "Humanoid avatar with the explicit mapping Hips=pelvis, Spine=spine_01, Chest=spine_02, UpperChest=spine_03 on Base Characters derivatives",
    "H2F-02 (J2ImportConventions)", "RETAINED_REALIZATION", "model importer settings in the owner's derivative .meta (committed by ART) or regenerated for restored vault copies",
    "J2ImportConventions.BaseCharacterMapping", "J2ImportConventions.ApplyHumanoid", "J2ImportConventions.Verify: J2_IMPORT_HIPS_NOT_PELVIS, J2_IMPORT_AVATAR_INVALID",
    "never", "reapplied deterministically after restoration", "results/representative.json")
row("import.ual", "UAL1/UAL2 clip libraries: Humanoid, same mapping, loopTime exactly on *_Loop clips, root rotation/height/XZ baked into the pose (in-place) with keepOriginalOrientation off; _RM libraries keep XZ/rotation as root motion",
    "H2F-02 (J2ImportConventions)", "RETAINED_REALIZATION", "importer settings of the clip-library copies",
    "J2ImportConventions (UalClipLibrary / UalRootMotionLibrary)", "J2ImportConventions.ApplyHumanoid(path, family)",
    "J2_IMPORT_LOOP_FLAG, J2_IMPORT_ROOT_NOT_IN_PLACE, J2_IMPORT_ROOT_MOTION_BAKED, J2_IMPORT_ROOT_HEIGHT_OR_ORIENTATION per clip", "never",
    "reapplied after restoration; ART-01 converges its own UAL1 loop rule at rebase", "results/representative.json")
row("anim.controllers", "UAL locomotion AnimatorController (1D blend on Speed, layer 0 IK pass) and other owner controllers", "H2F-02 recipe", "RETAINED_REALIZATION",
    "owner asset (ART/fixture); references restored clips by deterministic GUID", "the controller asset", "authored by the owner", "outside the H1 catalogue universe",
    "never", "survives; clips resolve after lawful restoration", "Docs/evidence/WP-H2F-01A/PUBLIC_AUTHORING_SURFACE.md")
row("rigging", "Animation Rigging Rig / TwoBoneIK constraints for contact IK on presentation prefabs", "H2F-02 (Animation Rigging 1.4.1)", "RETAINED_REALIZATION",
    "character presentation prefab of the owner", "the prefab", "authored on the prefab", "not an H1 component schema; must stay off H1-managed nodes", "never",
    "survives in the prefab", "Docs/evidence/WP-H2F-01/SPIKE_RESULTS.md")
row("input.asset", "J2_Input.inputactions: Player map Move, Look, Zoom, Interact", "H2F-02", "RETAINED_PROJECT_CONFIGURATION", "Assets/Juego2/Foundation/Input (committed)",
    "the action asset", "J2InputOwner enables it at runtime", "Verify J2_INPUT_ASSET; lint L6 requires every GC2 input to reference it", "never", "survives",
    "results/representative.json")
row("presets.character_camera", "J2_Player, J2_Npc_Civilian and J2_PlayerCamera presets (S06 amendment values)", "H2F-02", "RETAINED_REALIZATION",
    "Assets/Juego2/Foundation/Presets (committed)", "the preset assets", "J2Gc2Presets materializes GC2 objects from them",
    "Verify J2_PLAYER_PRESET, J2_NPC_PRESET, J2_CAMERA_PRESET", "never", "survives", "results/b2_gc2_tests.xml")
row("cinemachine", "Cinemachine 3 player rig", "H2F-01 selection displaced by the 01A S06 amendment", "NOT_ADMITTED", "none (package not installed)", "n/a", "n/a",
    "J2PackageBaseline J2_PACKAGE_NOT_ADMITTED:com.unity.cinemachine", "n/a", "n/a", "GC2_CORE_ADOPTION.md")
row("gc2.S1", "GC2 Core 2.19.61 vendor bytes (code, Mannequin, Skeleton, locomotion assets, gizmos)", "H2F-01A handoff", "EXTERNAL_MANUAL_INPUT",
    "Assets/Plugins/GameCreator (git-ignored)", "owner Asset Store package sha256 1e4f3ba0...2f3380b, 28,236,878 bytes",
    "scripts/h2f02-provision.py gc2 (hash-gated, Assets-only)", "J2Gc2Provisioning.State: version, receipt and Core-only assemblies (J2_GC2_*)", "never",
    "re-provisioned from the lawful local copy; when absent the adapter assemblies are excluded and the project still compiles",
    "results/b0_gc2_provisioning_receipt.json")
row("gc2.define", "Assets/csc.rsp (-define:JUEGO2_GC2_CORE) and the provisioning receipt", "H2F-02", "GENERATED_TRANSIENT",
    "git-ignored files written by the provisioning script", "scripts/h2f02-provision.py", "written by gc2, removed by remove",
    "status reports ABSENT / PROVISIONED / INVALID", "never", "regenerated by provisioning", "results/b1_gc2_report.json")
row("gc2.S2", "core.general / core.variables / core.updates / core.welcome settings assets", "H2F-01A handoff", "GENERATED_TRANSIENT",
    "Assets/Plugins/GameCreator/Data/Resources/Settings (git-ignored with the vendor root), generated at the first windowed editor load",
    "GC2 defaults; Juego2 does not override core.general because nothing Juego2 persists through GC2 before H6", "GC2 editor callbacks",
    "not project state; lint L7 forbids Juego2 persistence through GC2", "never", "regenerated", "Docs/evidence/WP-H2F-01A/CORE_STATE_INVENTORY.md")
row("gc2.S3", "GC2 Character serialized configuration (kernel units, input unit, motion, driver, footsteps, ragdoll, IK) for player and NPCs", "H2F-01A handoff + S06",
    "RETAINED_REALIZATION", "presentation object carrying J2PresetRealization in the " + SIDECAR,
    "J2CharacterPreset (J2_Player / J2_Npc_Civilian) + content model/controller", "J2Gc2Presets.MaterializeCharacter",
    "lints L4-L6 and the identity audit; never read back as canonical", "never; edit the preset and rematerialize",
    "rematerialized from the preset; survives in the sidecar", "results/c2_gc2_lint.json")
row("gc2.S4", "Character model instance created by ChangeModel (a copy, not a prefab link)", "H2F-01A handoff", "GENERATED_TRANSIENT",
    "child of the GC2 Character in the sidecar", "the preset model prefab", "J2Gc2Presets (removes the previous animator object first)", "n/a", "never",
    "rematerialized from the preset", "Unity/ArkusUnity/Assets/Juego2/Gc2Adapter/Editor/J2Gc2Presets.cs")
row("gc2.S5", "Components GC2 adds at play: hidden CharacterController, NavMeshAgent, InteractionTracker, kinematic Rigidbody, animator proxy, ragdoll bones",
    "H2F-01A handoff", "GENERATED_TRANSIENT", "runtime only", "GC2 kernel at play start", "created at play", "never serialized; runtime state is outside H1",
    "never", "recreated every play session", "Docs/evidence/WP-H2F-01A/CORE_STATE_INVENTORY.md")
row("gc2.S6", "Trigger / Hotspot / Actions / Conditions serialized lists including Juego2 adapter entries", "H2F-01A handoff", "RETAINED_REALIZATION",
    "authored local-execution configuration on presentation objects in the " + SIDECAR, "the authored lists; semantic effects only through Arkus requests",
    "authored through the GC2 public surface", "lints L2 (scene load, Remember), L4 (adapter on bound object), L5 (stock mutation only after a stopping Arkus request)",
    "never; Arkus decides, GC2 presents", "survives in the sidecar; adapter type names frozen, with [MovedFrom] from the 01A probe names",
    "results/b2_gc2_tests.xml")
row("gc2.S7", "MaterialSoundsAsset (texture key _BaseMap -> footstep clips)", "H2F-01A handoff", "RETAINED_REALIZATION",
    "Juego2 project asset created by the first owner that adopts footsteps (H2F-03 / GC2-00)", "the asset", "authored via the recorded serialized path",
    "outside the H1 catalogue universe", "never", "survives", "Docs/evidence/WP-H2F-01A/PUBLIC_AUTHORING_SURFACE.md")
row("gc2.S8", "GC2 Skeleton.asset and Human@Action_StandFace{Up,Down} recovery clips referenced by ragdoll configuration", "H2F-01A handoff", "EXTERNAL_MANUAL_INPUT",
    "provisioned with S1", "owner Core package", "provisioned with S1", "covered by the S1 checks", "never", "re-provisioned with S1",
    "Docs/evidence/WP-H2F-01A/CORE_STATE_INVENTORY.md")
row("gc2.S9", "GC2 locomotion state assets and controllers", "H2F-01A handoff", "NOT_ADMITTED", "vendor bytes only; Juego2 characters use the UAL controller",
    "n/a", "n/a", "n/a", "n/a", "n/a", "Docs/evidence/WP-H2F-01A/CORE_CAPABILITY_MATRIX.csv")
row("gc2.S10", "Juego2 input action asset read by the GC2 player unit and camera shot", "H2F-01A handoff", "RETAINED_PROJECT_CONFIGURATION",
    "same as input.asset", "J2_Input.inputactions", "J2InputOwner", "lint L6", "never", "survives", "results/representative.json")
row("gc2.S11", "GC2 Variables (local/global, name/list)", "H2F-01A handoff", "NOT_ADMITTED",
    "not used by the foundation; a later owner may only use one as local presentation state in the sidecar, never as a fact", "n/a", "n/a",
    "lint L3 rejects any Variable name in the Juego2 key namespace (GC2 normalizes j2.x:y to j2-x-y)", "n/a", "n/a", "results/b2_gc2_tests.xml")
row("gc2.S12", "GC2 save data (slots, volumes, scenes, global variables, IGameSave payloads)", "H2F-01A handoff", "NOT_ADMITTED",
    "none before H6 (save.host_storage DEFER_EVALUATION)", "n/a", "n/a", "lint L7 rejects any Juego2 IGameSave or TDataStorage", "n/a", "n/a",
    "results/b2_gc2_tests.xml")
row("gc2.S13", "GC2 runtime singletons (managers, SaveLoadManager, variable managers, Console) and spatial hashes", "H2F-01A handoff", "GENERATED_TRANSIENT",
    "DontDestroyOnLoad objects at play", "GC2 RuntimeInitializeOnLoadMethod", "created at play", "runtime only, outside H1", "never", "recreated every play session",
    "Docs/evidence/WP-H2F-01A/CORE_STATE_INVENTORY.md")
row("gc2.S14", "Remember / Memories / Tokens", "H2F-01A handoff", "NOT_ADMITTED", "none (REJECT_CAPABILITY)", "n/a", "n/a", "lint L2 J2_GC2_L2_REMEMBER", "n/a",
    "n/a", "results/b2_gc2_tests.xml")
row("gc2.S15", "GC2 MainCamera + ShotCamera Third Person configuration (radius 3, align 0.5 / 1.0, Look/Zoom on J2_Input)", "S06 amendment", "RETAINED_REALIZATION",
    "player camera object carrying J2PresetRealization in the " + SIDECAR, "J2CameraPreset J2_PlayerCamera", "J2Gc2Presets.MaterializePlayerCamera",
    "lint L6 on orbit and zoom inputs; identity audit empty", "never", "rematerialized from the preset; survives in the sidecar", "results/representative.json")
row("gc2.adapter_code", "Juego2.Gc2Adapter (+ .Editor, .Tests.Editor) assemblies: adapter seam, presets materializer, lints", "H2F-02", "RETAINED_PROJECT_CONFIGURATION",
    "Assets/Juego2/Gc2Adapter (committed; compiled only when provisioning defines JUEGO2_GC2_CORE)", "Juego2 source", "compiled by Unity",
    "J2AssemblyBoundary: only these assemblies may reference GameCreator and they must be gated", "never", "compiles with Core; excluded without it",
    "results/player_build.json")
row("gc2.scene_instructions", "GC2 scene load/unload instructions and GC2-driven scene reload", "H2F-01A handoff", "NOT_ADMITTED",
    "none (REJECT_CAPABILITY): scenes are loaded by H1 or the owner", "n/a", "n/a", "lint L2 J2_GC2_L2_SCENE_INSTRUCTION", "n/a", "n/a", "results/b2_gc2_tests.xml")

with OUT.open("w", newline="", encoding="utf-8") as handle:
    writer = csv.DictWriter(handle, fieldnames=H, lineterminator="\n")
    writer.writeheader()
    writer.writerows(ROWS)
print(f"H2F02_LIFECYCLE_MATRIX rows={len(ROWS)}")
