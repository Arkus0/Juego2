using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using Juego2.Arkus;
using Juego2.Foundation;
using Juego2.Foundation.Editor;
using Juego2.Gc2Adapter;
using Juego2.Gc2Adapter.Editor;
using Unity.AI.Navigation;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

namespace Juego2.H2F03.Evidence
{
    /// <summary>
    /// Builds the WP-H2F-03 <c>H2F_INTEGRATION_FIXTURE / NON_KEEPER</c> retained-realization sidecar around the canonical
    /// slice that the public H1 host has already materialized. Evidence code for the disposable workspace only. It uses the
    /// adopted Juego2 surfaces (look preset, linear/junction/scatter/collision realizers, water/window shaders, GC2
    /// presets, the Arkus adapter seam) and never writes into the H1-managed scene.
    /// </summary>
    public static class H2F03Fixture
    {
        public const string Root = "Assets/H2F03Fixture";
        public const string ScenePath = Root + "/H2F03_IntegrationFixture_NONKEEPER.unity";
        public const string Marker = "H2F_INTEGRATION_FIXTURE__NON_KEEPER";
        public const string NavMeshAsset = Root + "/H2F03_NavMesh.asset";
        public const string TerrainAsset = Root + "/H2F03_Terrain.asset";
        public const string ManagedManifest = "Assets/Arkus/H1/ManagedScenes/current.json";

        public const string Citizen = "Assets/Arkus/ART/Derived/Characters/Townsfolk_Forastero.fbx";
        public const string Ual1 = "Assets/Arkus/ART/External/UAL/UAL1.fbx";
        public static readonly string[] UalLibraries = { Ual1, "Assets/H2F01Inputs/Animation/UAL2.fbx" };
        public static readonly string[] UalRootMotionLibraries = { "Assets/H2F01Inputs/Animation/UAL1_RM.fbx", "Assets/H2F01Inputs/Animation/UAL2_RM.fbx" };
        public static readonly string[] ContentRoots = { "Assets/Arkus/ART", "Assets/H2F01Inputs" };

        public const string PlayerKey = "j2.char.player";
        public const string WalkerKey = "j2.npc.fixture_walker";
        public const string ContactKey = "j2.npc.fixture_contact";
        public const string DoorKey = "j2.door.fixture_house";
        public const string LampKey = "j2.lamp.fixture_house";
        public const int ScatterSeed = 20260928;

        static string A(string name) => Root + "/" + name;
        public static string CitizenPrefab => A("Characters/J2_Citizen.prefab");
        public static string ContactPrefab => A("Characters/J2_Citizen_ContactRig.prefab");
        public static string Controller => A("Characters/J2_UAL_Locomotion.controller");
        public static string PlayerPreset => A("Presets/J2_Player_Fixture.asset");
        public static string WalkerPreset => A("Presets/J2_Npc_Walker_Fixture.asset");
        public static string ContactPreset => A("Presets/J2_Npc_Contact_Fixture.asset");
        public static string StreetProfile => A("Profiles/J2_Street_Fixture.asset");
        public static string LaneProfile => A("Profiles/J2_Lane_Fixture.asset");
        public static string ScatterProfile => A("Profiles/J2_Nature_Fixture.asset");

        // ------------------------------------------------------------------ 1. content preparation (before H1 materialize)

        [Serializable]
        sealed class PrepareReport
        {
            public string schema = "juego2.h2f03.prepare-content@1";
            public string[] migrated, humanoidMapping, importFindings, created, errors;
            public bool citizenAvatarValid;
            public J2ShaderAudit.Report contentAudit;
        }

        public static void PrepareContent()
        {
            var r = new PrepareReport();
            var errors = new List<string>();
            var created = new List<string>();
            foreach (var d in new[] { Root, A("Characters"), A("Presets"), A("Profiles"), A("Materials"), A("Nature"), A("Textures") })
                J2FoundationBaseline.EnsureFolder(d);

            Step(errors, "migrate", () => r.migrated = J2MaterialMigration.UpgradeBuiltinMaterials(ContentRoots).ToArray());
            Step(errors, "humanoid", () =>
            {
                var mapping = new List<string> { "citizen: " + J2ImportConventions.ApplyHumanoid(Citizen, J2ImportConventions.SourceFamily.HumanoidBaseCharacter) };
                foreach (var ual in UalLibraries)
                    mapping.Add(Path.GetFileName(ual) + ": " + J2ImportConventions.ApplyHumanoid(ual, J2ImportConventions.SourceFamily.UalClipLibrary));
                foreach (var ual in UalRootMotionLibraries)
                    mapping.Add(Path.GetFileName(ual) + ": " + J2ImportConventions.ApplyHumanoid(ual, J2ImportConventions.SourceFamily.UalRootMotionLibrary));
                r.humanoidMapping = mapping.ToArray();
                r.importFindings = J2ImportConventions.Verify(new[] { Citizen }, UalLibraries, UalRootMotionLibraries).ToArray();
                var avatar = AssetDatabase.LoadAllAssetsAtPath(Citizen).OfType<Avatar>().FirstOrDefault();
                r.citizenAvatarValid = avatar != null && avatar.isValid && avatar.isHuman;
            });
            Step(errors, "characters", () =>
            {
                var citizen = ArtPalettePrefab(Citizen, CitizenPrefab);
                created.Add(CitizenPrefab);
                created.Add(ContactRigPrefab(citizen));
                created.Add(UalLocomotion());
            });
            Step(errors, "presets", () =>
            {
                var model = AssetDatabase.LoadAssetAtPath<GameObject>(CitizenPrefab);
                var contactModel = AssetDatabase.LoadAssetAtPath<GameObject>(ContactPrefab);
                var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(Controller);
                created.Add(PresetCopy(J2FoundationBaseline.PlayerPreset, PlayerPreset, model, controller));
                created.Add(PresetCopy(J2FoundationBaseline.NpcPreset, WalkerPreset, model, controller));
                created.Add(PresetCopy(J2FoundationBaseline.NpcPreset, ContactPreset, contactModel, controller));
            });
            Step(errors, "materials", () => created.AddRange(Materials()));
            Step(errors, "profiles", () => created.AddRange(Profiles()));
            Step(errors, "content-audit", () => r.contentAudit = J2ShaderAudit.Audit(J2ShaderAudit.MaterialAssets(Root)));
            AssetDatabase.SaveAssets();
            r.created = created.ToArray();
            r.errors = errors.ToArray();
            H2F03Io.WriteJson("prepare_content.json", r);
            Debug.Log($"H2F03_PREPARE_CONTENT errors={r.errors.Length} migrated={r.migrated?.Length} import_findings={r.importFindings?.Length} avatar={r.citizenAvatarValid} audit_green={r.contentAudit?.green}");
            H2F03Io.Exit(r.errors.Length == 0 ? 0 : 3);
        }

        static string PresetCopy(string source, string path, GameObject model, RuntimeAnimatorController controller)
        {
            AssetDatabase.DeleteAsset(path);
            var copy = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<J2CharacterPreset>(source));
            copy.model = model;
            copy.locomotionController = controller;
            AssetDatabase.CreateAsset(copy, path);
            return path;
        }

        static IEnumerable<string> Materials()
        {
            Material Save(Material m, string name)
            {
                var path = A("Materials/" + name + ".mat");
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.CreateAsset(m, path);
                return m;
            }
            Save(new Material(Shader.Find("Juego2/StylizedWater")), "J2_Water_Fixture");
            Save(new Material(Shader.Find("Juego2/InteriorWindow")), "J2_InteriorWindow_Fixture");
            var terrain = Save(new Material(Shader.Find("Universal Render Pipeline/Terrain/Lit")), "J2_Terrain_Fixture");
            var decal = new Material(Shader.Find("Shader Graphs/Decal"));
            decal.SetTexture("Base_Map", DecalTexture());
            Save(decal, "J2_Decal_WetGrime_Fixture");
            var counter = new Material(J2MaterialMigration.LitShader()) { color = new Color(0.42f, 0.30f, 0.20f) };
            Save(counter, "J2_Counter_Fixture");
            var layer = new TerrainLayer
            {
                diffuseTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Arkus/ART/External/Nature/Textures/Grass.png"),
                tileSize = new Vector2(3, 3),
            };
            var layerPath = A("Materials/J2_TerrainLayer_Grass.terrainlayer");
            AssetDatabase.DeleteAsset(layerPath);
            AssetDatabase.CreateAsset(layer, layerPath);
            return new[] { "Materials/*", layerPath };
        }

        /// <summary>Deterministic wet-grime stain (seeded value noise), so no external decal texture is needed.</summary>
        static Texture2D DecalTexture()
        {
            var path = A("Textures/J2_WetGrime.png");
            const int n = 256;
            var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    float u = x / (float)n - 0.5f, v = y / (float)n - 0.5f;
                    float r = Mathf.Sqrt(u * u * 1.6f + v * v) * 2f;
                    float noise = Mathf.PerlinNoise(x * 0.035f + 11.3f, y * 0.035f + 7.1f) * 0.6f + Mathf.PerlinNoise(x * 0.11f + 3.7f, y * 0.11f + 1.9f) * 0.4f;
                    float a = Mathf.Clamp01((1.05f - r) * 1.8f) * Mathf.SmoothStep(0.25f, 0.75f, noise);
                    tex.SetPixel(x, y, new Color(0.10f, 0.09f, 0.08f, a * 0.85f));
                }
            File.WriteAllBytes(Path.GetFullPath(path), tex.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(path);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        static IEnumerable<string> Profiles()
        {
            Material Art(string id) => AssetDatabase.LoadAssetAtPath<Material>($"Assets/Arkus/ART/Materials/{id}.mat") ?? throw new Exception("H2F03_ART_MATERIAL_MISSING " + id);
            J2LinearProfile Linear(string path, float width, J2LinearProfile.EdgeKind left, J2LinearProfile.EdgeKind right)
            {
                AssetDatabase.DeleteAsset(path);
                var p = ScriptableObject.CreateInstance<J2LinearProfile>();
                p.width = width; p.leftEdge = left; p.rightEdge = right;
                p.surfaceMaterial = Art("Cobble"); p.edgeMaterial = Art("StoneTrim"); p.wallMaterial = Art("Stone");
                AssetDatabase.CreateAsset(p, path);
                return p;
            }
            Linear(StreetProfile, 5f, J2LinearProfile.EdgeKind.Kerb, J2LinearProfile.EdgeKind.Kerb);
            Linear(LaneProfile, 2.8f, J2LinearProfile.EdgeKind.Kerb, J2LinearProfile.EdgeKind.RetainingWall);

            AssetDatabase.DeleteAsset(ScatterProfile);
            var scatter = ScriptableObject.CreateInstance<J2ScatterProfile>();
            scatter.entries = new[] { ("CommonTree_1", 0.3f, true), ("Bush_Common", 1f, false), ("Rock_Medium_1", 0.6f, true), ("Bush_Common_Flowers", 0.8f, false) }
                .Select(e => new J2ScatterProfile.Entry
                {
                    prefab = NaturePrefab(e.Item1, e.Item3), weight = e.Item2, scaleBand = new Vector2(0.8f, 1.2f),
                }).ToArray();
            scatter.density = 0.14f; scatter.minSpacing = 1.6f; scatter.exclusionMargin = 0.6f; scatter.maxGroundSlope = 30f;
            AssetDatabase.CreateAsset(scatter, ScatterProfile);
            return new[] { StreetProfile, LaneProfile, ScatterProfile };
        }

        /// <summary>ART-palette nature prefab; trees and rocks get a simple collider so the player cannot walk through them.</summary>
        static GameObject NaturePrefab(string model, bool collide)
        {
            var path = A($"Nature/{model}.prefab");
            var prefab = ArtPalettePrefab($"Assets/Arkus/ART/External/Nature/Models/{model}.fbx", path);
            if (!collide) return prefab;
            var root = PrefabUtility.LoadPrefabContents(path);
            var b = new Bounds(root.transform.position, Vector3.zero);
            foreach (var rr in root.GetComponentsInChildren<Renderer>()) b.Encapsulate(rr.bounds);
            var capsule = root.AddComponent<CapsuleCollider>();
            capsule.center = root.transform.InverseTransformPoint(new Vector3(b.center.x, b.min.y + Mathf.Min(1.2f, b.size.y) / 2f, b.center.z));
            capsule.height = Mathf.Min(1.2f, b.size.y);
            capsule.radius = model.StartsWith("Rock") ? Mathf.Min(b.extents.x, b.extents.z) * 0.8f : 0.25f;
            PrefabUtility.SaveAsPrefabAsset(root, path);
            PrefabUtility.UnloadPrefabContents(root);
            return AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }

        /// <summary>
        /// The ART citizen with an Animation Rigging right-hand contact (TwoBoneIK). The target and hint are part of the
        /// presentation prefab (H2F-02 matrix family <c>rigging</c>), so a GC2 ChangeModel copy carries them.
        /// </summary>
        static string ContactRigPrefab(GameObject citizen)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(citizen);
            PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, UnityEditor.InteractionMode.AutomatedAction);
            var animator = instance.GetComponentInChildren<Animator>();
            if (animator == null || !animator.isHuman) throw new Exception("H2F03_CONTACT_NOT_HUMANOID");
            var rig = new GameObject("J2_ContactRig").AddComponent<Rig>();
            rig.transform.SetParent(animator.transform, false);
            var ik = new GameObject("RightHandContact").AddComponent<TwoBoneIKConstraint>();
            ik.transform.SetParent(rig.transform, false);
            var target = new GameObject("IK_RightHand_Target").transform;
            target.SetParent(animator.transform, false);
            target.localPosition = ContactTargetLocal;
            target.localRotation = Quaternion.Euler(0, 0, -90);
            var hint = new GameObject("IK_RightHand_Hint").transform;
            hint.SetParent(animator.transform, false);
            hint.localPosition = new Vector3(0.55f, 1.15f, -0.25f);
            var data = ik.data;
            data.root = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            data.mid = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            data.tip = animator.GetBoneTransform(HumanBodyBones.RightHand);
            data.target = target;
            data.hint = hint;
            data.targetPositionWeight = 1f;
            data.targetRotationWeight = 0f;
            data.hintWeight = 0.6f;
            ik.data = data;
            if (data.root == null || data.mid == null || data.tip == null) throw new Exception("H2F03_CONTACT_BONES_MISSING");
            var builder = animator.gameObject.AddComponent<RigBuilder>();
            builder.layers.Add(new RigLayer(rig, true));
            PrefabUtility.SaveAsPrefabAsset(instance, ContactPrefab);
            UnityEngine.Object.DestroyImmediate(instance);
            return ContactPrefab;
        }

        /// <summary>Right-hand contact target in the model's local frame (feet origin): ~1 m high, in front and to the right.</summary>
        public static readonly Vector3 ContactTargetLocal = new Vector3(0.24f, 1.0f, 0.34f);

        static string UalLocomotion()
        {
            AssetDatabase.DeleteAsset(Controller);
            var c = AnimatorController.CreateAnimatorControllerAtPath(Controller);
            c.AddParameter("Speed", AnimatorControllerParameterType.Float);
            c.CreateBlendTreeInController("Locomotion", out var tree, 0);
            tree.blendParameter = "Speed";
            tree.AddChild(UalClip(Ual1, "Idle_Loop"), 0f);
            tree.AddChild(UalClip(Ual1, "Walk_Loop"), 1f);
            var layers = c.layers;
            layers[0].iKPass = true;
            c.layers = layers;
            return Controller;
        }

        public static AnimationClip UalClip(string fbx, string name) =>
            AssetDatabase.LoadAllAssetsAtPath(fbx).OfType<AnimationClip>().FirstOrDefault(c => c.name == name || c.name == "Armature|" + name)
            ?? throw new Exception("H2F03_CLIP_MISSING " + name);

        /// <summary>
        /// H2F-01 route C: Source models render with the project-owned URP ART palette (the same name mapping H2F-02's
        /// evidence used). The migrated ART material assets are loaded directly.
        /// </summary>
        public static GameObject ArtPalettePrefab(string model, string path)
        {
            J2FoundationBaseline.EnsureFolder(Path.GetDirectoryName(path).Replace('\\', '/'));
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(model) ?? throw new Exception("H2F03_MODEL_MISSING " + model));
            foreach (var renderer in instance.GetComponentsInChildren<Renderer>(true))
                renderer.sharedMaterials = renderer.sharedMaterials.Select(m => AssetDatabase.LoadAssetAtPath<Material>($"Assets/Arkus/ART/Materials/{Palette(m.name)}.mat")
                    ?? throw new Exception("H2F03_PALETTE_MATERIAL_MISSING " + m.name)).ToArray();
            var prefab = PrefabUtility.SaveAsPrefabAsset(instance, path);
            UnityEngine.Object.DestroyImmediate(instance);
            return prefab;
        }

        static string Palette(string source)
        {
            if (source.Contains("Bark")) return "Bark";
            if (source.Contains("Leaves")) return "Leaves";
            if (source.Contains("Flowers")) return "Flowers";
            if (source == "Grass") return "GrassGround";
            if (source.Contains("Rocks")) return "Rock";
            if (source.Contains("Regular_Male") || source.Contains("Skin_Regular_Male_Light")) return "Skin";
            if (source.Contains("Eye")) return "Eye";
            if (source.Contains("Hair")) return "Hair";
            if (source.Contains("Cloth_D8D2C4")) return "Shirt";
            if (source.Contains("Cloth_2E4A4E")) return "Coat";
            if (source.Contains("Cloth_3A3530")) return "Trousers";
            if (source.Contains("Cloth_4A3222")) return "Shoes";
            throw new Exception("H2F03_SOURCE_MATERIAL_UNMAPPED " + source);
        }

        // ------------------------------------------------------------------ 2. the sidecar (after the first H1 materialize)

        [Serializable]
        sealed class BuildReport
        {
            public string schema = "juego2.h2f03.fixture-build@1";
            public string scene, managedScene, managedGeneration;
            public string[] managedRoots, gc2Lint, errors, authoringLog, identityFields, notes;
            public J2ShaderAudit.Report sceneAudit;
            public int junctionSamples, junctionHoles, junctionSteps, scatterInstances, scatterInExclusion, navTriangles;
            public string navPathStatus, collisionInputDigest, collisionOutputDigest;
            public bool managedHasColliders;
            public float contactTargetToCounterTop;
        }

        public static void BuildSidecar()
        {
            var r = new BuildReport { scene = ScenePath };
            var errors = new List<string>();
            var notes = new List<string>();
            J2Gc2Presets.AuthoringLog.Clear();
            var sidecar = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(sidecar, ScenePath);
            var managed = OpenManaged(out r.managedScene, out r.managedGeneration);
            SceneManager.SetActiveScene(sidecar);
            r.managedRoots = managed.GetRootGameObjects().Select(g => g.name).OrderBy(n => n).ToArray();
            r.managedHasColliders = managed.GetRootGameObjects().Any(g => g.GetComponentsInChildren<Collider>(true).Length > 0);

            var fixture = new GameObject(Marker);
            new GameObject("README: compatibility fixture only; no ART-01/CITY-07 keeper claim").transform.SetParent(fixture.transform, false);
            Transform Group(string name) { var g = new GameObject(name).transform; g.SetParent(fixture.transform, false); return g; }

            Step(errors, "look", () => J2FoundationLook.ApplyTo(sidecar, AssetDatabase.LoadAssetAtPath<J2LookPreset>(J2FoundationBaseline.LookPreset)));

            SplineContainer street = null, lane = null;
            Step(errors, "linear", () =>
            {
                var world = Group("Worldbuilding");
                street = Spline(world, "fixture_street", new Vector3(0, 0.02f, -24), new Vector3(0, 0.02f, 0), new Vector3(0, 0.02f, 24));
                lane = Spline(world, "fixture_lane", new Vector3(0, 0.02f, 8), new Vector3(7, 0.45f, 8), new Vector3(15, 1.15f, 8.4f));
                var junction = J2JunctionRealizer.Realize(street, AssetDatabase.LoadAssetAtPath<J2LinearProfile>(StreetProfile), 32f,
                    lane, AssetDatabase.LoadAssetAtPath<J2LinearProfile>(LaneProfile), 1.2f);
                var at = J2LinearRealizer.FrameAt(street, junction.mainDistance);
                Vector3 f = Vector3.ProjectOnPlane(at.forward, Vector3.up).normalized, n = at.right * junction.side;
                float open = 1.4f + 0.2f + 1.2f;
                var report = J2SurfaceContinuity.Sample(at.position - f * 10 - n * 2.5f, f, n, 20, 2.5f + junction.branchStart + 5, 0.1f, p =>
                {
                    var d = Vector3.ProjectOnPlane(p - at.position, Vector3.up);
                    float u = Vector3.Dot(d, f), v = Vector3.Dot(d, n);
                    if (Mathf.Abs(u) > 9.5f) return false;
                    if (v >= -2.45f && v <= 2.45f) return true;
                    if (v > junction.branchStart + 0.06f) return Mathf.Abs(u) <= 1.34f && v < junction.branchStart + 4.5f;
                    if (v < 2.5f || Mathf.Abs(u) > open - 0.06f) return false;
                    foreach (int k in new[] { -1, 1 }) if ((new Vector2(u, v) - new Vector2(k * open, junction.branchStart)).magnitude < 1.46f) return false;
                    return true;
                });
                r.junctionSamples = report.samples; r.junctionHoles = report.holes; r.junctionSteps = report.steps;
            });

            Step(errors, "terrain", () => TerrainGround(Group("Terrain"), lane));

            Transform collisionHost = null;
            Step(errors, "collision", () =>
            {
                collisionHost = Group("J2_CanonicalCollision");
                var modifier = collisionHost.gameObject.AddComponent<NavMeshModifier>();
                modifier.overrideArea = true;
                modifier.area = 1; // Not Walkable: proxies are obstacles, never walkable surfaces
                var proxy = J2CollisionRealizer.Realize(managed.GetRootGameObjects().Select(g => g.transform), collisionHost, "canonical_slice");
                var marker = proxy.GetComponent<J2GeneratedRealization>();
                r.collisionInputDigest = marker.inputDigest;
                r.collisionOutputDigest = marker.outputDigest;
            });

            var house = Bounds(managed, "house.corner");
            var door = Bounds(managed, "house.door");
            var window = Bounds(managed, "house.window");
            notes.Add($"house bounds {house}; door {door}; window {window}");

            Step(errors, "water", () =>
            {
                var group = Group("Water");
                var water = GameObject.CreatePrimitive(PrimitiveType.Plane);
                water.name = "pond_water";
                water.transform.SetParent(group, false);
                water.transform.position = new Vector3(12f, -0.25f, -3.5f);
                water.transform.localScale = new Vector3(0.95f, 1, 1.15f);
                UnityEngine.Object.DestroyImmediate(water.GetComponent<Collider>());
                water.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(A("Materials/J2_Water_Fixture.mat"));
                Exclusion(group, "pond_exclusion", new Vector3(12f, -0.4f, -3.5f), new Vector3(11f, 1.8f, 13f));
            });

            Step(errors, "window", () =>
            {
                var group = Group("InteriorWindow");
                var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.name = "interior_window_fixture";
                quad.transform.SetParent(group, false);
                UnityEngine.Object.DestroyImmediate(quad.GetComponent<Collider>());
                // The canonical facade faces +X; the quad sits just inside the opening and faces the street.
                quad.transform.position = new Vector3(window.min.x - 0.02f, window.center.y, window.center.z);
                quad.transform.rotation = Quaternion.Euler(0, -90, 0);
                quad.transform.localScale = new Vector3(window.size.z * 0.92f, window.size.y * 0.92f, 1);
                quad.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(A("Materials/J2_InteriorWindow_Fixture.mat"));
            });

            Step(errors, "decal", () =>
            {
                var go = new GameObject("decal_wet_grime_threshold");
                go.transform.SetParent(Group("Decals"), false);
                // Straddles the street edge, the threshold and the canonical facade base; decal layers must keep it on the street.
                go.transform.SetPositionAndRotation(new Vector3((house.max.x - 0.3f) / 2f, 0.6f, door.center.z + 0.4f), Quaternion.Euler(90, 0, 0));
                var projector = go.AddComponent<DecalProjector>();
                projector.material = AssetDatabase.LoadAssetAtPath<Material>(A("Materials/J2_Decal_WetGrime_Fixture.mat"));
                projector.size = new Vector3(Mathf.Abs(house.max.x) + 0.6f, 2.4f, 1.4f);
                projector.renderingLayerMask = J2RenderingLayers.DecalReceiver;
            });

            Step(errors, "exclusions", () =>
            {
                var group = Group("ScatterExclusions");
                Exclusion(group, "threshold_exclusion", new Vector3(house.max.x + 0.4f, 0.6f, house.center.z), new Vector3(1.6f, 1.6f, house.size.z + 3f));
                Exclusion(group, "street_start_exclusion", new Vector3(0, 0.5f, -22f), new Vector3(8f, 1.5f, 5f));
            });

            // characters before the NavMesh bake, so the market counter carves it
            Character player = null, walker = null, contact = null;
            Step(errors, "characters", () =>
            {
                var group = Group("Characters");
                player = Place(group, J2Gc2Presets.MaterializeCharacter(Load<J2CharacterPreset>(PlayerPreset), PlayerKey, new Vector3(0.6f, 0.03f, -20f)), 0);
                J2Gc2Presets.MaterializePlayerCamera(AssetDatabase.LoadAssetAtPath<J2CameraPreset>(J2FoundationBaseline.CameraPreset)).transform.SetParent(group, true);
                walker = Place(group, J2Gc2Presets.MaterializeCharacter(Load<J2CharacterPreset>(WalkerPreset), WalkerKey, new Vector3(-1.2f, 0.03f, -14f)), 0);
                contact = Place(group, J2Gc2Presets.MaterializeCharacter(Load<J2CharacterPreset>(ContactPreset), ContactKey, new Vector3(1.3f, 0.03f, -4f)), 90);
                // the contact surface: a sidecar market counter whose top meets the IK target
                var target = contact.GetComponentInChildren<TwoBoneIKConstraint>(true).data.target.position;
                var counter = GameObject.CreatePrimitive(PrimitiveType.Cube);
                counter.name = "market_counter_fixture";
                counter.transform.SetParent(group, false);
                float top = target.y - 0.03f;
                counter.transform.position = new Vector3(target.x + 0.28f, 0.03f + (top - 0.03f) / 2f, target.z);
                counter.transform.localScale = new Vector3(0.6f, top - 0.03f, 1.0f);
                counter.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(A("Materials/J2_Counter_Fixture.mat"));
                r.contactTargetToCounterTop = target.y - (counter.transform.position.y + counter.transform.localScale.y / 2f);
            });

            Step(errors, "navmesh", () =>
            {
                var nav = new GameObject("J2_Navigation");
                nav.transform.SetParent(fixture.transform, false);
                var surface = nav.AddComponent<NavMeshSurface>();
                surface.collectObjects = CollectObjects.All;
                surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
                surface.agentTypeID = 0;
                surface.BuildNavMesh();
                AssetDatabase.DeleteAsset(NavMeshAsset);
                AssetDatabase.CreateAsset(surface.navMeshData, NavMeshAsset);
                var tri = NavMesh.CalculateTriangulation();
                r.navTriangles = tri.indices.Length / 3;
                var path = new NavMeshPath();
                NavMesh.CalculatePath(new Vector3(-1.2f, 0.1f, -14f), new Vector3(13f, 1.1f, 8.3f), NavMesh.AllAreas, path);
                r.navPathStatus = path.status.ToString();
            });

            Step(errors, "scatter", () =>
            {
                var nature = J2ScatterRealizer.Realize(Group("Nature"), new Bounds(new Vector3(0, 5, 0), new Vector3(60, 14, 60)), Load<J2ScatterProfile>(ScatterProfile), ScatterSeed);
                r.scatterInstances = nature.transform.childCount;
                var exclusions = UnityEngine.Object.FindObjectsByType<J2ScatterExclusion>(FindObjectsSortMode.None).SelectMany(e => e.GetComponentsInChildren<Collider>()).ToList();
                r.scatterInExclusion = nature.transform.Cast<Transform>().Count(t => exclusions.Any(c => c.bounds.Contains(t.position)));
            });

            Step(errors, "interaction", () =>
            {
                var group = Group("Interaction");
                var lamp = new GameObject(LampKey);
                lamp.transform.SetParent(group, false);
                lamp.transform.position = new Vector3(door.max.x + 0.35f, 2.75f, door.center.z + 0.95f);
                var light = lamp.AddComponent<Light>();
                light.type = LightType.Point; light.intensity = 0f; light.range = 7f; light.color = new Color(1f, 0.78f, 0.52f); light.shadows = LightShadows.None;
                Bind(lamp, LampKey, "lamp");

                var hook = new GameObject(DoorKey);
                hook.transform.SetParent(group, false);
                hook.transform.position = new Vector3(door.max.x + 0.55f, 0f, door.center.z);
                var box = hook.AddComponent<BoxCollider>();
                box.isTrigger = true; box.center = new Vector3(0, 1f, 0); box.size = new Vector3(1.0f, 2.0f, 1.3f);
                hook.AddComponent<NavMeshModifier>().ignoreFromBuild = true;
                Bind(hook, DoorKey, "door");
                var trigger = hook.AddComponent<Trigger>();
                Trigger.Reconfigure(trigger, new EventCharacterOnInteract(), new InstructionList(
                    new InstructionArkusRequestTransition(GetGameObjectSelf.Create(), GetGameObjectTarget.Create(), "door.open", true),
                    new InstructionLightChangeIntensity()));
                var so = new SerializedObject(trigger);
                var light1 = so.FindProperty("m_Instructions.m_Instructions.Array.data[1].m_Light.m_Property") ?? throw new Exception("H2F03_GC2_PATH_MISSING light");
                light1.managedReferenceValue = new GetGameObjectArkusEntity(LampKey);
                so.ApplyModifiedPropertiesWithoutUndo();
                var hotspot = hook.AddComponent<Hotspot>();
                var hs = new SerializedObject(hotspot);
                var radius = hs.FindProperty("m_Radius.m_Property") ?? throw new Exception("H2F03_GC2_PATH_MISSING hotspot radius");
                radius.managedReferenceValue = new GetDecimalDecimal(2.5f);
                hs.ApplyModifiedPropertiesWithoutUndo();

                var authority = new GameObject("H2F03_FixtureAuthority (evidence stand-in for WP-GC2-00)").AddComponent<H2F03FixtureAuthority>();
                authority.transform.SetParent(group, false);
                authority.doorKey = DoorKey;

                var driver = new GameObject("H2F03_Traversal (idle unless scripted)").AddComponent<H2F03Traversal>();
                driver.transform.SetParent(fixture.transform, false);
                driver.authority = authority;
                driver.playerKey = PlayerKey; driver.walkerKey = WalkerKey; driver.contactKey = ContactKey; driver.lampKey = LampKey;
                driver.route = Route(door, window);
                driver.npcLoop = new[] { new Vector3(-1.2f, 0, -14f), new Vector3(-1.2f, 0, 5.5f), new Vector3(6.5f, 0.4f, 8.1f), new Vector3(-1.2f, 0, 3f) };
            });

            Step(errors, "audit", () =>
            {
                r.sceneAudit = J2ShaderAudit.Audit(J2ShaderAudit.SceneMaterials(sidecar).Concat(J2ShaderAudit.SceneMaterials(managed)));
                r.gc2Lint = J2Gc2Lint.CheckScene(sidecar).Select(f => f.ToString()).ToArray();
            });
            r.authoringLog = J2Gc2Presets.AuthoringLog.ToArray();
            r.errors = errors.ToArray();
            r.notes = notes.ToArray();
            EditorSceneManager.SaveScene(sidecar, ScenePath);
            EditorSceneManager.CloseScene(managed, true); // never saved: the managed projection belongs to H1
            AssetDatabase.SaveAssets();
            H2F03Io.WriteJson("fixture_build.json", r);
            Debug.Log($"H2F03_FIXTURE_BUILT errors={r.errors.Length} lint={r.gc2Lint?.Length} audit_green={r.sceneAudit?.green} holes={r.junctionHoles} steps={r.junctionSteps} scatter={r.scatterInstances} in_exclusion={r.scatterInExclusion} nav={r.navPathStatus} managed_colliders={r.managedHasColliders}");
            H2F03Io.Exit(r.errors.Length == 0 ? 0 : 3);
        }

        public static H2F03Traversal.Waypoint[] Route(Bounds door, Bounds window)
        {
            H2F03Traversal.Waypoint W(string name, float x, float z, string action = "", float yaw = 0, float hold = 0) =>
                new H2F03Traversal.Waypoint { name = name, position = new Vector3(x, 0, z), action = action, lookYaw = yaw, hold = hold };
            return new[]
            {
                W("street_south", 0.6f, -12f),
                W("street_house", 0.6f, door.center.z + 0.6f),
                W("door_threshold", door.max.x + 0.95f, door.center.z, "interact", 0, 0.5f),
                W("window_view", -1.4f, window.center.z, "look", 270f, 2f),
                W("street_north", 0.6f, 5.5f),
                W("junction", 0.9f, 8.0f),
                W("lane_mid", 7f, 8.1f),
                W("lane_top", 13f, 8.3f, "look", 180f, 2f),
                W("lane_back", 5f, 8.1f),
                W("street_end", 0.6f, -6f, "", 0, 1f),
            };
        }

        // ------------------------------------------------------------------ helpers

        public static Scene OpenManaged(out string path, out string generation)
        {
            var manifest = File.ReadAllText(Path.Combine(H2F03Io.ProjectRoot, ManagedManifest));
            var m = JsonUtility.FromJson<ManagedManifestJson>(manifest);
            path = m.scenePath;
            generation = m.generationId;
            return EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
        }

        [Serializable]
        sealed class ManagedManifestJson
        {
            public string scenePath, generationId;
        }

        public static Bounds Bounds(Scene scene, string rootName)
        {
            var go = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).FirstOrDefault(t => t.name == rootName)
                     ?? throw new Exception("H2F03_MANAGED_NODE_MISSING " + rootName);
            var renderers = go.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0) throw new Exception("H2F03_MANAGED_NODE_HAS_NO_RENDERER " + rootName);
            var b = renderers[0].bounds;
            foreach (var r in renderers) b.Encapsulate(r.bounds);
            return b;
        }

        static SplineContainer Spline(Transform parent, string name, params Vector3[] pts)
        {
            var c = new GameObject(name).AddComponent<SplineContainer>();
            c.transform.SetParent(parent, false);
            c.Spline.Clear();
            foreach (var p in pts) c.Spline.Add(new BezierKnot((float3)p), TangentMode.AutoSmooth);
            return c;
        }

        static void Exclusion(Transform parent, string name, Vector3 center, Vector3 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = center;
            var box = go.AddComponent<BoxCollider>();
            box.size = size;
            box.isTrigger = true;
            go.AddComponent<J2ScatterExclusion>();
            go.AddComponent<NavMeshModifier>().ignoreFromBuild = true;
        }

        static void Bind(GameObject go, string key, string kind)
        {
            var b = go.AddComponent<ArkusEntityBinding>();
            b.entityKey = key;
            b.kind = kind;
        }

        static Character Place(Transform parent, Character c, float yaw)
        {
            c.transform.SetParent(parent, true);
            c.transform.rotation = Quaternion.Euler(0, yaw, 0);
            return c;
        }

        static T Load<T>(string path) where T : UnityEngine.Object => AssetDatabase.LoadAssetAtPath<T>(path) ?? throw new Exception("H2F03_ASSET_MISSING " + path);

        /// <summary>Scenic ground only (H2F-01): a pond basin, and ground that rises with the lane on its kerb side.</summary>
        static void TerrainGround(Transform parent, SplineContainer lane)
        {
            const int res = 129;
            const float size = 64f, height = 8f, baseY = -2f;
            var laneHeight = new List<Vector2>();
            for (int i = 0; i <= 200; i++)
            {
                var p = (Vector3)lane.EvaluatePosition(i / 200f);
                laneHeight.Add(new Vector2(p.x, p.y));
            }
            float LaneY(float x) => x <= laneHeight[0].x ? laneHeight[0].y : x >= laneHeight[laneHeight.Count - 1].x ? laneHeight[laneHeight.Count - 1].y
                : laneHeight.Zip(laneHeight.Skip(1), (a, b) => (a, b)).Where(s => x >= s.a.x && x <= s.b.x).Select(s => Mathf.Lerp(s.a.y, s.b.y, (x - s.a.x) / Mathf.Max(1e-4f, s.b.x - s.a.x))).First();
            float H(float x, float z)
            {
                float y = 0f;
                if (Mathf.Abs(x) < 3.1f) y = -0.06f; // under the street
                if (x > 2.6f && z > 6.2f)
                {
                    float lx = Mathf.Min(x, 15.5f);
                    float target = z < 9.8f ? LaneY(lx) - 0.14f : LaneY(lx) - 0.03f; // under the lane / its kerb side
                    y = Mathf.Lerp(y, target, Mathf.Clamp01((z - 6.2f) / 0.8f));
                }
                // pond basin with 1.5 m banks
                float dx = Mathf.Max(0, Mathf.Abs(x - 12f) - 4.2f), dz = Mathf.Max(0, Mathf.Abs(z + 3.5f) - 5.2f);
                float bank = Mathf.Sqrt(dx * dx + dz * dz);
                if (bank < 1.5f) y = Mathf.Min(y, Mathf.Lerp(-1.2f, 0f, bank / 1.5f));
                // scenic rise behind the house and at the far edges
                if (x < -12f) y += Mathf.SmoothStep(0, 2.2f, (-12f - x) / 16f);
                return y;
            }
            var data = new TerrainData { heightmapResolution = res };
            data.size = new Vector3(size, height, size);
            var h = new float[res, res];
            for (int iz = 0; iz < res; iz++)
                for (int ix = 0; ix < res; ix++)
                {
                    float x = -size / 2 + ix * size / (res - 1), z = -size / 2 + iz * size / (res - 1);
                    h[iz, ix] = Mathf.Clamp01((H(x, z) - baseY) / height);
                }
            data.SetHeights(0, 0, h);
            data.terrainLayers = new[] { AssetDatabase.LoadAssetAtPath<TerrainLayer>(A("Materials/J2_TerrainLayer_Grass.terrainlayer")) };
            AssetDatabase.DeleteAsset(TerrainAsset);
            AssetDatabase.CreateAsset(data, TerrainAsset);
            var go = Terrain.CreateTerrainGameObject(data);
            go.name = "scenic_terrain";
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(-size / 2, baseY, -size / 2);
            var terrain = go.GetComponent<Terrain>();
            terrain.materialTemplate = AssetDatabase.LoadAssetAtPath<Material>(A("Materials/J2_Terrain_Fixture.mat"));
            terrain.drawTreesAndFoliage = false;
            var modifier = go.AddComponent<NavMeshModifier>();
            modifier.overrideArea = true;
            modifier.area = 1; // Not Walkable: scenic
        }

        static void Step(List<string> errors, string name, Action action)
        {
            try { action(); }
            catch (Exception e) { errors.Add(name + ": " + e.GetType().Name + ": " + e.Message); Debug.LogException(e); }
        }
    }
}
