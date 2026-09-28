using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameCreator.Runtime.Characters;
using Juego2.Arkus;
using Juego2.Foundation;
using Juego2.Foundation.Editor;
using Juego2.Gc2Adapter.Editor;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

namespace Juego2.H2F02.Evidence
{
    /// <summary>
    /// WP-H2F-02 evidence driver for the disposable workspace only (never part of the product project). It exercises
    /// the adopted foundation's public surfaces on representative real content: ART-01 PREFOUNDATION_INPUT
    /// (174d05d2) and owner-vault Quaternius sources, with GC2 Core 2.19.61 provisioned.
    /// </summary>
    public static class J2H2F02Evidence
    {
        const string ArtScene = "Assets/Arkus/ART/Art01Benchmark.unity";
        const string Citizen = "Assets/Arkus/ART/Derived/Characters/Townsfolk_Forastero.fbx";
        const string Ual1 = "Assets/Arkus/ART/External/UAL/UAL1.fbx";
        static readonly string[] UalLibraries = { "Assets/H2F01Inputs/Animation/UAL2.fbx", "Assets/H2F01Inputs/Animation/UAL1_RM.fbx", "Assets/H2F01Inputs/Animation/UAL2_RM.fbx" };
        const string Dir = "Assets/H2F02Evidence";
        static readonly string[] ContentRoots = { "Assets/Arkus/ART", "Assets/H2F01Inputs", "Assets/Juego2", "Assets/Arkus/CITY" };

        static readonly (string id, Vector3 pos, Vector3 target)[] Views =
        {
            ("puente_s02", new Vector3(0, 1.65f, -35), new Vector3(0, 2.1f, -16)),
            ("w12_casco", new Vector3(0, 1.65f, -10), new Vector3(0, 2.2f, 13)),
            ("roof_eave_corner", new Vector3(3.6f, 2.2f, -9.6f), new Vector3(1.8f, 6.6f, -4.0f)),
            ("facade_stone_close", new Vector3(1.2f, 1.6f, -8.5f), new Vector3(-2.4f, 1.8f, -6.5f)),
            ("f01_exterior", new Vector3(0, 1.65f, 12), new Vector3(0, 3.0f, 28)),
            ("f01_threshold", new Vector3(0, 1.65f, 22.4f), new Vector3(0, 1.3f, 29)),
        };

        static string Results => Path.GetFullPath(Path.Combine(Application.dataPath, "../../../results"));

        [Serializable]
        sealed class Report
        {
            public string candidateNote = "values produced by the WP-H2F-02 evidence workspace";
            public string unityVersion, pipeline, gc2Core;
            public string[] migrated, humanoidMapping, importFindings, lintFindings, identityFields, enabledInputBindings, bindings, errors, captures;
            public J2ShaderAudit.Report routeAudit, contentAudit, worldbuildingAudit;
            public int junctionSamples, junctionHoles, junctionSteps, scatterInstances, scatterInExclusion;
            public bool citizenAvatarValid, playerIsGc2Player, npcInputNone;
        }

        public static void Representative()
        {
            var r = new Report { unityVersion = Application.unityVersion, pipeline = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline?.name };
            var errors = new List<string>();
            var captures = new List<string>();
            r.gc2Core = J2Gc2Provisioning.State(out var gc2Findings);
            Directory.CreateDirectory(Path.Combine(Results, "captures"));
            J2FoundationBaseline.EnsureFolder(Dir);

            Step(errors, "migrate", () => r.migrated = J2MaterialMigration.UpgradeBuiltinMaterials(ContentRoots).ToArray());
            Step(errors, "humanoid", () =>
            {
                var mapping = new List<string> { "citizen: " + J2ImportConventions.ApplyHumanoid(Citizen, J2ImportConventions.SourceFamily.HumanoidBaseCharacter) };
                foreach (var ual in UalLibraries.Concat(new[] { Ual1 }))
                    mapping.Add(Path.GetFileName(ual) + ": " + J2ImportConventions.ApplyHumanoid(ual, J2ImportConventions.SourceFamily.UalClipLibrary));
                r.humanoidMapping = mapping.ToArray();
                r.importFindings = J2ImportConventions.Verify(new[] { Citizen }, UalLibraries.Concat(new[] { Ual1 })).ToArray();
                var avatar = AssetDatabase.LoadAllAssetsAtPath(Citizen).OfType<Avatar>().FirstOrDefault();
                r.citizenAvatarValid = avatar != null && avatar.isValid && avatar.isHuman;
            });

            // --- ART-01 structural route under the adopted URP baseline + foundation look
            Step(errors, "route", () =>
            {
                var scene = EditorSceneManager.OpenScene(ArtScene, OpenSceneMode.Single);
                J2FoundationLook.ApplyTo(scene, AssetDatabase.LoadAssetAtPath<J2LookPreset>(J2FoundationBaseline.LookPreset));
                foreach (var cam in UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsSortMode.None)) cam.enabled = false;
                foreach (var v in Views) captures.Add(Capture("route_" + v.id, v.pos, v.target, 55));
                r.routeAudit = J2ShaderAudit.Audit(J2ShaderAudit.SceneMaterials(scene));

                // S06 amendment presets materialized with the ART clothed citizen and the UAL locomotion controller
                var controller = UalLocomotion();
                var playerPreset = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<J2CharacterPreset>(J2FoundationBaseline.PlayerPreset));
                playerPreset.name = "J2_Player (evidence copy)";
                playerPreset.model = AssetDatabase.LoadAssetAtPath<GameObject>(Citizen);
                playerPreset.locomotionController = controller;
                var npcPreset = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<J2CharacterPreset>(J2FoundationBaseline.NpcPreset));
                npcPreset.name = "J2_Npc_Civilian (evidence copy)";
                npcPreset.model = playerPreset.model;
                npcPreset.locomotionController = controller;
                var start = Ground(new Vector3(0, 40, -8));
                var player = J2Gc2Presets.MaterializeCharacter(playerPreset, "j2.char.player", start);
                player.transform.rotation = Quaternion.LookRotation(Vector3.forward); // walking the route toward the bar
                var npc = J2Gc2Presets.MaterializeCharacter(npcPreset, "j2.npc.evidence_a", Ground(new Vector3(1.6f, 40, -5.5f)));
                npc.transform.rotation = Quaternion.Euler(0, 200, 0);
                J2Gc2Presets.MaterializePlayerCamera(AssetDatabase.LoadAssetAtPath<J2CameraPreset>(J2FoundationBaseline.CameraPreset));
                r.playerIsGc2Player = new SerializedObject(player).FindProperty("m_IsPlayer").boolValue;
                r.npcInputNone = new SerializedObject(npc).FindProperty("m_Kernel.m_Player.m_InputMove.m_Input").managedReferenceFullTypename.EndsWith("InputValueVector2None");
                r.lintFindings = J2Gc2Lint.CheckScene(scene).Select(f => f.ToString()).ToArray();
                r.identityFields = IdentityFields(scene);
                r.bindings = UnityEngine.Object.FindObjectsByType<ArkusEntityBinding>(FindObjectsSortMode.None).Select(b => b.entityKey).OrderBy(k => k).ToArray();
                r.enabledInputBindings = InputBindings(scene);
                PoseHumans(UalClip(Ual1, "Idle_Loop"));
                var body = player.transform.position;
                captures.Add(Capture("s06_third_person_player", body + new Vector3(0.6f, 1.1f, -3.2f), body + new Vector3(0, 0.5f, 2.0f), 55));
                captures.Add(Capture("s06_player_and_npc", body + new Vector3(-1.6f, 0.9f, 3.4f), body + new Vector3(0.8f, 0.3f, 1.2f), 55));
                AnimationMode.StopAnimationMode();
                EditorSceneManager.SaveScene(scene, Dir + "/RouteEvidence.unity", true);
            });

            Step(errors, "worldbuilding", () => Worldbuilding(r, captures));

            Step(errors, "content-audit", () => r.contentAudit = J2ShaderAudit.Audit(J2ShaderAudit.MaterialAssets(ContentRoots)));
            r.errors = errors.Concat(gc2Findings).ToArray();
            r.captures = captures.ToArray();
            File.WriteAllText(Path.Combine(Results, "representative.json"), JsonUtility.ToJson(r, true) + "\n", new UTF8Encoding(false));
            Debug.Log($"H2F02_REPRESENTATIVE errors={r.errors.Length} route_green={r.routeAudit?.green} content_green={r.contentAudit?.green} world_green={r.worldbuildingAudit?.green} lint={r.lintFindings?.Length} import={r.importFindings?.Length} holes={r.junctionHoles} steps={r.junctionSteps}");
            if (Application.isBatchMode) EditorApplication.Exit(r.errors.Length == 0 ? 0 : 3);
        }

        static void Worldbuilding(Report r, List<string> captures)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            J2FoundationLook.ApplyTo(scene, AssetDatabase.LoadAssetAtPath<J2LookPreset>(J2FoundationBaseline.LookPreset));
            Material Art(string id) => AssetDatabase.LoadAssetAtPath<Material>($"Assets/Arkus/ART/Materials/{id}.mat");
            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "ground";
            ground.transform.position = new Vector3(0, -0.5f, 0);
            ground.transform.localScale = new Vector3(60, 1, 60);
            ground.GetComponent<Renderer>().sharedMaterial = Art("GrassGround");

            J2LinearProfile Profile(string name, float width, J2LinearProfile.EdgeKind right)
            {
                var p = ScriptableObject.CreateInstance<J2LinearProfile>();
                p.name = name; p.width = width; p.rightEdge = right;
                p.surfaceMaterial = Art("Cobble"); p.edgeMaterial = Art("StoneTrim"); p.wallMaterial = Art("Stone");
                return p;
            }
            SplineContainer Spline(string name, params Vector3[] pts)
            {
                var c = new GameObject(name).AddComponent<SplineContainer>();
                c.Spline.Clear();
                foreach (var p in pts) c.Spline.Add(new BezierKnot((float3)p), TangentMode.AutoSmooth);
                return c;
            }
            var rot = Quaternion.Euler(0, 25, 0);
            var main = Spline("x1_street", rot * new Vector3(0, 0.02f, -16), rot * new Vector3(0, 0.3f, 0), rot * new Vector3(0, 0.6f, 16));
            var branch = Spline("w12_lane", rot * new Vector3(0, 0.3f, 0), rot * new Vector3(8, 0.9f, 0) , rot * new Vector3(16, 1.8f, 0.5f));
            var junction = J2JunctionRealizer.Realize(main, Profile("x1", 5.5f, J2LinearProfile.EdgeKind.Kerb), main.CalculateLength() / 2f,
                branch, Profile("w12", 2.8f, J2LinearProfile.EdgeKind.RetainingWall), 1.2f);
            var at = J2LinearRealizer.FrameAt(main, junction.mainDistance);
            Vector3 f = Vector3.ProjectOnPlane(at.forward, Vector3.up).normalized, n = at.right * junction.side;
            float open = 1.4f + 0.2f + 1.2f;
            var report = J2SurfaceContinuity.Sample(at.position - f * 10 - n * 2.75f, f, n, 20, 2.75f + junction.branchStart + 5, 0.1f, p =>
            {
                var d = Vector3.ProjectOnPlane(p - at.position, Vector3.up);
                float u = Vector3.Dot(d, f), v = Vector3.Dot(d, n);
                if (Mathf.Abs(u) > 9.5f) return false;
                if (v >= -2.7f && v <= 2.7f) return true;
                if (v > junction.branchStart + 0.06f) return Mathf.Abs(u) <= 1.34f && v < junction.branchStart + 4.5f;
                if (v < 2.75f || Mathf.Abs(u) > open - 0.06f) return false;
                foreach (int k in new[] { -1, 1 }) if ((new Vector2(u, v) - new Vector2(k * open, junction.branchStart)).magnitude < 1.46f) return false;
                return true;
            });
            r.junctionSamples = report.samples; r.junctionHoles = report.holes; r.junctionSteps = report.steps;

            var water = GameObject.CreatePrimitive(PrimitiveType.Cube);
            water.name = "pond_exclusion";
            water.transform.position = rot * new Vector3(-12, -0.05f, 8);
            water.transform.localScale = new Vector3(9, 0.1f, 7);
            water.AddComponent<J2ScatterExclusion>();
            water.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Juego2/StylizedWater")) { name = "J2_Water_evidence" };
            var window = GameObject.CreatePrimitive(PrimitiveType.Quad);
            window.name = "interior_window";
            window.transform.position = rot * new Vector3(-6, 1.6f, -6);
            window.transform.rotation = rot * Quaternion.Euler(0, 90, 0);
            window.transform.localScale = new Vector3(1.4f, 1.6f, 1);
            window.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Juego2/InteriorWindow")) { name = "J2_Window_evidence" };

            var scatter = ScriptableObject.CreateInstance<J2ScatterProfile>();
            scatter.name = "art_nature";
            scatter.entries = new[] { "Bush_Common", "Fern_1", "Rock_Medium_1", "Bush_Common_Flowers" }
                .Select(id => AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Arkus/ART/External/Nature/Models/{id}.fbx"))
                .Where(p => p != null).Select(p => new J2ScatterProfile.Entry { prefab = p, weight = 1, scaleBand = new Vector2(0.8f, 1.2f) }).ToArray();
            scatter.density = 0.45f; scatter.minSpacing = 1.4f; scatter.exclusionMargin = 0.6f;
            var nature = J2ScatterRealizer.Realize(new GameObject("nature").transform, new Bounds(Vector3.zero, new Vector3(40, 6, 40)), scatter, 20260928);
            r.scatterInstances = nature.transform.childCount;
            r.scatterInExclusion = nature.transform.Cast<Transform>().Count(t => water.GetComponent<Collider>().bounds.Contains(new Vector3(t.position.x, water.transform.position.y, t.position.z)));

            r.worldbuildingAudit = J2ShaderAudit.Audit(J2ShaderAudit.SceneMaterials(scene));
            captures.Add(Capture("worldbuilding_junction_oblique", at.position + n * 9 + f * -9 + Vector3.up * 7, at.position + n * 2, 55));
            captures.Add(Capture("worldbuilding_water_window", rot * new Vector3(-3, 2.2f, 2), rot * new Vector3(-10, 0.4f, 6), 60));
            EditorSceneManager.SaveScene(scene, Dir + "/WorldbuildingEvidence.unity", true);
        }

        /// <summary>A player build with GC2 Core + the Juego2 adapter: the runtime assemblies must compile without UNITY_EDITOR.</summary>
        public static void PlayerBuild()
        {
            J2FoundationBaseline.EnsureFolder(Dir);
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            J2Gc2Presets.MaterializeCharacter(AssetDatabase.LoadAssetAtPath<J2CharacterPreset>(J2FoundationBaseline.PlayerPreset), "j2.char.player", Vector3.zero);
            J2Gc2Presets.MaterializePlayerCamera(AssetDatabase.LoadAssetAtPath<J2CameraPreset>(J2FoundationBaseline.CameraPreset));
            new GameObject("adapter_probe").AddComponent<ArkusEntityBinding>().entityKey = "j2.door.build_probe";
            var path = Dir + "/BuildProbe.unity";
            EditorSceneManager.SaveScene(scene, path);
            var output = Path.GetFullPath(Path.Combine(Application.dataPath, "../../../build/Probe.exe"));
            var build = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { path }, locationPathName = output, target = BuildTarget.StandaloneWindows64, options = BuildOptions.None,
            });
            var managed = Path.Combine(Path.GetDirectoryName(output), "Probe_Data/Managed");
            var assemblies = Directory.Exists(managed) ? Directory.GetFiles(managed, "*.dll").Select(Path.GetFileName)
                .Where(n => n.StartsWith("GameCreator") || n.StartsWith("Juego2") || n.StartsWith("Arkus")).OrderBy(n => n).ToArray() : new string[0];
            var summary = build.summary;
            var json = "{\n  \"result\": \"" + summary.result + "\",\n  \"totalErrors\": " + summary.totalErrors + ",\n  \"totalWarnings\": " + summary.totalWarnings +
                       ",\n  \"platform\": \"" + summary.platform + "\",\n  \"playerAssemblies\": [" + string.Join(", ", assemblies.Select(a => "\"" + a + "\"")) + "]\n}\n";
            Directory.CreateDirectory(Results);
            File.WriteAllText(Path.Combine(Results, "player_build.json"), json, new UTF8Encoding(false));
            Debug.Log($"H2F02_PLAYER_BUILD result={summary.result} errors={summary.totalErrors} assemblies={string.Join(",", assemblies)}");
            if (Application.isBatchMode) EditorApplication.Exit(summary.result == BuildResult.Succeeded ? 0 : 3);
        }

        // ------------------------------------------------------------------ helpers

        static void Step(List<string> errors, string name, Action action)
        {
            try { action(); }
            catch (Exception e) { errors.Add(name + ": " + e.GetType().Name + ": " + e.Message); Debug.LogException(e); }
        }

        static Vector3 Ground(Vector3 from)
        {
            Physics.SyncTransforms();
            return Physics.Raycast(from, Vector3.down, out var hit, 80) ? hit.point : new Vector3(from.x, 0, from.z);
        }

        static AnimatorController UalLocomotion()
        {
            var path = Dir + "/J2_UAL_Locomotion.controller";
            AssetDatabase.DeleteAsset(path);
            var c = AnimatorController.CreateAnimatorControllerAtPath(path);
            c.AddParameter("Speed", AnimatorControllerParameterType.Float);
            c.CreateBlendTreeInController("Locomotion", out var tree, 0);
            tree.blendParameter = "Speed";
            tree.AddChild(UalClip(Ual1, "Idle_Loop"), 0f);
            tree.AddChild(UalClip(Ual1, "Walk_Loop"), 1f);
            var layers = c.layers;
            layers[0].iKPass = true;
            c.layers = layers;
            return c;
        }

        static AnimationClip UalClip(string fbx, string name) =>
            AssetDatabase.LoadAllAssetsAtPath(fbx).OfType<AnimationClip>().FirstOrDefault(c => c.name == name || c.name == "Armature|" + name)
            ?? throw new Exception("H2F02_CLIP_MISSING " + name);

        static void PoseHumans(AnimationClip clip)
        {
            if (!AnimationMode.InAnimationMode()) AnimationMode.StartAnimationMode();
            foreach (var animator in UnityEngine.Object.FindObjectsByType<Animator>(FindObjectsSortMode.None))
                if (animator.avatar != null && animator.avatar.isHuman) AnimationMode.SampleAnimationClip(animator.gameObject, clip, 0.4f);
        }

        static string[] IdentityFields(Scene scene)
        {
            var identity = new System.Text.RegularExpressions.Regex(@"(guid|uniqueid|saveid|^m_id$)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var hits = new List<string>();
            foreach (var root in scene.GetRootGameObjects())
                foreach (var c in root.GetComponentsInChildren<Component>(true))
                {
                    if (c == null || !c.GetType().FullName.StartsWith("GameCreator.", StringComparison.Ordinal)) continue;
                    var it = new SerializedObject(c).GetIterator();
                    while (it.Next(true))
                        if (identity.IsMatch(it.name) && (it.propertyType != SerializedPropertyType.String || !string.IsNullOrEmpty(it.stringValue)))
                            hits.Add(c.GetType().Name + "." + it.propertyPath);
                }
            return hits.ToArray();
        }

        static string[] InputBindings(Scene scene)
        {
            var rows = new List<string>();
            foreach (var root in scene.GetRootGameObjects())
                foreach (var c in root.GetComponentsInChildren<Component>(true))
                {
                    if (c == null || !c.GetType().FullName.StartsWith("GameCreator.", StringComparison.Ordinal)) continue;
                    var it = new SerializedObject(c).GetIterator();
                    while (it.Next(true))
                        if (it.propertyType == SerializedPropertyType.ManagedReference && it.managedReferenceFullTypename.Contains(".Input"))
                        {
                            var asset = new SerializedObject(c).FindProperty(it.propertyPath + ".m_Input.m_InputAsset")?.objectReferenceValue;
                            var action = new SerializedObject(c).FindProperty(it.propertyPath + ".m_Input.m_Action")?.stringValue;
                            rows.Add($"{c.name}.{it.propertyPath} = {it.managedReferenceFullTypename.Split(' ').Last()} {(asset != null ? asset.name + "/" + action : "")}");
                        }
                }
            return rows.ToArray();
        }

        static string Capture(string id, Vector3 position, Vector3 target, float fov)
        {
            var go = new GameObject("capture_" + id);
            var camera = go.AddComponent<Camera>();
            camera.fieldOfView = fov;
            camera.nearClipPlane = 0.05f;
            go.transform.position = position;
            go.transform.LookAt(target);
            camera.GetUniversalAdditionalCameraData().renderPostProcessing = true;
            const int w = 1280, h = 720;
            var rt = RenderTexture.GetTemporary(w, h, 24, RenderTextureFormat.ARGB32);
            camera.targetTexture = rt;
            camera.Render();
            RenderTexture.active = rt;
            var png = new Texture2D(w, h, TextureFormat.RGB24, false);
            png.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            png.Apply();
            var file = Path.Combine(Results, "captures", id + ".png");
            File.WriteAllBytes(file, png.EncodeToPNG());
            camera.targetTexture = null;
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
            UnityEngine.Object.DestroyImmediate(png);
            UnityEngine.Object.DestroyImmediate(go);
            return "captures/" + id + ".png";
        }
    }
}
