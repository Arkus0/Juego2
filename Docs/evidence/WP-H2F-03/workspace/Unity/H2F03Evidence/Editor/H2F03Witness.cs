using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.VisualScripting;
using Juego2.Arkus;
using Juego2.Foundation;
using Juego2.Foundation.Editor;
using Juego2.Gc2Adapter;
using Juego2.Gc2Adapter.Editor;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

namespace Juego2.H2F03.Evidence
{
    /// <summary>
    /// Read-only lifecycle witnesses for the H2F-02 matrix families that live in Unity state (scene/project/runtime
    /// configuration). File-level families (lock, ProjectSettings bytes, receipts) are hashed by the lifecycle driver.
    /// Every record carries a digest; the driver compares before/after per the family's declared lifecycle class.
    /// Checks that regenerate content in memory (scatter/collision determinism) run last and are never saved.
    /// </summary>
    public static class H2F03Witness
    {
        [Serializable]
        public sealed class Record
        {
            public string family;
            public bool present;
            public string digest;
            public string[] facts;
        }

        [Serializable]
        sealed class Report
        {
            public string schema = "juego2.h2f03.unity-witness@1";
            public string label, unityVersion, sidecarScene, managedScene, managedGeneration;
            public string[] errors;
            public Record[] records;
        }

        public static void Capture()
        {
            var label = H2F03Io.Arg("-h2f03-label") ?? "unlabelled";
            var report = new Report { label = label, unityVersion = Application.unityVersion, sidecarScene = H2F03Fixture.ScenePath };
            var records = new List<Record>();
            var errors = new List<string>();
            Scene sidecar = default, managed = default;
            try
            {
                sidecar = EditorSceneManager.OpenScene(H2F03Fixture.ScenePath, OpenSceneMode.Single);
                managed = H2F03Fixture.OpenManaged(out report.managedScene, out report.managedGeneration);
                SceneManager.SetActiveScene(sidecar);
            }
            catch (Exception e) { errors.Add("open: " + e.Message); }

            void Add(string family, bool present, params string[] facts) =>
                records.Add(new Record { family = family, present = present, facts = facts, digest = H2F03Io.Sha(family + "\n" + string.Join("\n", facts)) });
            void Try(string family, Action action)
            {
                try { action(); }
                catch (Exception e) { errors.Add(family + ": " + e.GetType().Name + ": " + e.Message); Add(family, false, "ERROR " + e.Message); }
            }

            Try("urp.pipeline_assets", () =>
            {
                var findings = J2FoundationBaseline.Verify();
                var pipeline = GraphicsSettings.defaultRenderPipeline;
                var urp = pipeline as UniversalRenderPipelineAsset;
                Add("urp.pipeline_assets", urp != null,
                    "default=" + AssetDatabase.GetAssetPath(pipeline),
                    "quality=" + string.Join(",", QualitySettings.names.Select((n, i) => n + ":" + AssetDatabase.GetAssetPath(QualitySettings.GetRenderPipelineAssetAt(i)))),
                    "features=" + string.Join(",", Features(urp)),
                    "baselineFindings=" + string.Join(",", findings));
                Add("gi.apv", findings.Any(f => f.StartsWith("J2_APV")), "apvFindings=" + string.Join(",", findings.Where(f => f.StartsWith("J2_APV"))));
            });
            Try("settings.project", () => Add("settings.project", true, "colorSpace=" + PlayerSettings.colorSpace,
                "renderingLayer1=" + RenderingLayerMask.RenderingLayerToName(1)));
            Try("urp.scene_look", () =>
            {
                var sun = RenderSettings.sun;
                var volume = sidecar.GetRootGameObjects().Select(g => g.GetComponent<Volume>()).FirstOrDefault(v => v != null);
                Add("urp.scene_look", sun != null && volume != null,
                    $"ambient={RenderSettings.ambientMode} {C(RenderSettings.ambientSkyColor)} {C(RenderSettings.ambientEquatorColor)} {C(RenderSettings.ambientGroundColor)}",
                    $"fog={RenderSettings.fog} {RenderSettings.fogMode} {RenderSettings.fogDensity:0.#####} {C(RenderSettings.fogColor)}",
                    "skybox=" + AssetDatabase.GetAssetPath(RenderSettings.skybox),
                    sun == null ? "sun=none" : $"sun={sun.name} {C(sun.color)} {sun.intensity:0.###} {sun.shadows} {V(sun.transform.eulerAngles)}",
                    "volume=" + (volume != null ? volume.name + " global=" + volume.isGlobal + " profile=" + AssetDatabase.GetAssetPath(volume.sharedProfile) : "none"));
                Add("urp.look_preset", AssetDatabase.LoadAssetAtPath<J2LookPreset>(J2FoundationBaseline.LookPreset) != null,
                    EditorJsonUtility.ToJson(AssetDatabase.LoadAssetAtPath<J2LookPreset>(J2FoundationBaseline.LookPreset)));
            });
            Try("gi.bakes", () => Add("gi.bakes", Lightmapping.lightingDataAsset != null || LightmapSettings.lightmaps.Length > 0,
                "lightingData=" + (Lightmapping.lightingDataAsset != null ? AssetDatabase.GetAssetPath(Lightmapping.lightingDataAsset) : "none"),
                "lightmaps=" + LightmapSettings.lightmaps.Length));
            Try("shaders.project", () => Add("shaders.project", true, new[] { "Juego2/StylizedWater", "Juego2/InteriorWindow" }
                .Select(n => { var s = Shader.Find(n); return n + "=" + (s == null ? "missing" : J2ShaderAudit.Classify(s) + " supported=" + s.isSupported); }).ToArray()));
            Try("materials.retained", () =>
            {
                var audit = J2ShaderAudit.Audit(J2ShaderAudit.SceneMaterials(sidecar).Concat(J2ShaderAudit.MaterialAssets(H2F03Fixture.Root, "Assets/Juego2")));
                Add("materials.retained", true, new[] { "green=" + audit.green, "failures=" + string.Join(",", audit.failures) }
                    .Concat(audit.rows.Select(r => r.material + "|" + r.shader + "|" + r.status).Distinct().OrderBy(s => s, StringComparer.Ordinal)).ToArray());
            });
            Try("materials.importer_embedded", () =>
            {
                var audit = J2ShaderAudit.Audit(J2ShaderAudit.SceneMaterials(managed));
                Add("materials.importer_embedded", true, new[] { "green=" + audit.green, "failures=" + string.Join(",", audit.failures) }
                    .Concat(audit.rows.Select(r => r.material + "|" + r.shader + "|" + r.status).Distinct().OrderBy(s => s, StringComparer.Ordinal)).ToArray());
            });
            Try("h1.managed_projection", () => Add("h1.managed_projection", managed.IsValid(),
                managed.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true))
                    .Select(t => Path(t) + "|" + V(t.position) + "|" + V(t.eulerAngles) + "|" + V(t.lossyScale) + "|" +
                                 string.Join(",", t.GetComponents<Component>().Select(c => c == null ? "MISSING" : c.GetType().Name)))
                    .OrderBy(s => s, StringComparer.Ordinal).ToArray()));
            Try("splines.containers", () =>
            {
                var containers = sidecar.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<SplineContainer>(true)).ToList();
                Add("splines.containers", containers.Count > 0, containers.Select(c => c.name + "|" + string.Join(";", c.Spline.Knots.Select(k => V(k.Position)))).OrderBy(s => s).ToArray());
                var instantiate = sidecar.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<SplineInstantiate>(true)).Count();
                Add("splines.instantiate", instantiate > 0, "splineInstantiate=" + instantiate);
            });
            Try("linear.realization", () =>
            {
                var roots = Generated(sidecar).Where(g => g.generator == J2LinearRealizer.Generator || g.generator == J2JunctionRealizer.Generator).ToList();
                Add("linear.realization", roots.Count > 0, roots.Select(g => $"{Path(g.transform)}|{g.generator}@{g.generatorVersion}|in={g.inputDigest}|out={g.outputDigest}|" +
                    (g.generator == J2LinearRealizer.Generator ? "recomputedOut=" + J2LinearRealizer.OutputDigest(g.gameObject) : "")).OrderBy(s => s).ToArray());
            });
            Try("worldbuilding.profiles", () => Add("worldbuilding.profiles", true, new[] { H2F03Fixture.StreetProfile, H2F03Fixture.LaneProfile, H2F03Fixture.ScatterProfile }
                .Select(p => p + "=" + H2F03Io.Sha(EditorJsonUtility.ToJson(AssetDatabase.LoadMainAssetAtPath(p)))).ToArray()));
            Try("terrain", () =>
            {
                var terrain = sidecar.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Terrain>(true)).FirstOrDefault();
                var data = terrain != null ? terrain.terrainData : null;
                var heights = data != null ? data.GetHeights(0, 0, data.heightmapResolution, data.heightmapResolution) : new float[0, 0];
                var hs = new StringBuilder();
                foreach (var v in heights) hs.Append(v.ToString("0.#####")).Append(',');
                Add("terrain", data != null, "data=" + AssetDatabase.GetAssetPath(data), "heights=" + H2F03Io.Sha(hs.ToString()),
                    "layers=" + (data != null ? string.Join(",", data.terrainLayers.Select(l => AssetDatabase.GetAssetPath(l))) : ""),
                    "material=" + (terrain != null && terrain.materialTemplate != null ? terrain.materialTemplate.shader.name : "none"),
                    "collider=" + (terrain != null && terrain.GetComponent<TerrainCollider>() != null));
                Add("terrain.vegetation", data != null && (data.treeInstanceCount > 0 || data.detailPrototypes.Length > 0),
                    "trees=" + (data?.treeInstanceCount ?? 0), "treePrototypes=" + (data?.treePrototypes.Length ?? 0), "details=" + (data?.detailPrototypes.Length ?? 0));
            });
            Try("navmesh.surface", () =>
            {
                var surfaces = sidecar.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<NavMeshSurface>(true)).ToList();
                var modifiers = sidecar.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<NavMeshModifier>(true))
                    .Select(m => $"{Path(m.transform)}|override={m.overrideArea}|area={m.area}|ignore={m.ignoreFromBuild}").OrderBy(s => s);
                Add("navmesh.surface", surfaces.Count > 0, surfaces.Select(s => $"{s.name}|agent={s.agentTypeID}|collect={s.collectObjects}|geometry={s.useGeometry}|layers={s.layerMask.value}|data={AssetDatabase.GetAssetPath(s.navMeshData)}")
                    .Concat(modifiers).ToArray());
                var tri = NavMesh.CalculateTriangulation();
                var t = new StringBuilder();
                foreach (var v in tri.vertices) t.Append(V(v)).Append(';');
                t.Append('|').Append(string.Join(",", tri.indices)).Append('|').Append(string.Join(",", tri.areas));
                var path = new NavMeshPath();
                NavMesh.CalculatePath(new Vector3(-1.2f, 0.1f, -14f), new Vector3(13f, 1.1f, 8.3f), NavMesh.AllAreas, path);
                Add("navmesh.data", tri.vertices.Length > 0, "asset=" + surfaces.Select(s => AssetDatabase.GetAssetPath(s.navMeshData)).FirstOrDefault(),
                    "triangles=" + tri.indices.Length / 3, "triangulation=" + H2F03Io.Sha(t.ToString()), "streetToLanePath=" + path.status);
                var agent = NavMesh.GetSettingsByIndex(0);
                Add("navmesh.agent", true, $"radius={agent.agentRadius:0.###} height={agent.agentHeight:0.###} climb={agent.agentClimb:0.###} slope={agent.agentSlope:0.#}");
            });
            Try("import.humanoid", () =>
            {
                var findings = J2ImportConventions.Verify(new[] { H2F03Fixture.Citizen }, new string[0]);
                var avatar = AssetDatabase.LoadAllAssetsAtPath(H2F03Fixture.Citizen).OfType<Avatar>().FirstOrDefault();
                Add("import.humanoid", avatar != null, "findings=" + string.Join(",", findings), "avatarValid=" + (avatar != null && avatar.isValid && avatar.isHuman),
                    "hips=" + ((ModelImporter)AssetImporter.GetAtPath(H2F03Fixture.Citizen)).humanDescription.human.FirstOrDefault(h => h.humanName == "Hips").boneName);
                var ual = J2ImportConventions.Verify(new string[0], H2F03Fixture.UalLibraries, H2F03Fixture.UalRootMotionLibraries);
                Add("import.ual", true, new[] { "findings=" + ual.Count }.Concat(ual.OrderBy(f => f)).ToArray());
            });
            Try("anim.controllers", () =>
            {
                var c = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(H2F03Fixture.Controller);
                Add("anim.controllers", c != null, "controller=" + H2F03Fixture.Controller, "clips=" + string.Join(",", c.animationClips.Select(a => a.name + "@" + AssetDatabase.GetAssetPath(a))),
                    "ikPass=" + c.layers[0].iKPass);
            });
            Try("rigging", () =>
            {
                var contact = Character(sidecar, H2F03Fixture.ContactKey);
                var ik = contact != null ? contact.GetComponentInChildren<TwoBoneIKConstraint>(true) : null;
                var builder = contact != null ? contact.GetComponentInChildren<RigBuilder>(true) : null;
                Add("rigging", ik != null && builder != null, ik == null ? new[] { "missing" } : new[]
                {
                    $"root={ik.data.root?.name} mid={ik.data.mid?.name} tip={ik.data.tip?.name}",
                    $"target={ik.data.target?.name}@{V(ik.data.target.localPosition)} hint={ik.data.hint?.name}@{V(ik.data.hint.localPosition)}",
                    $"weights pos={ik.data.targetPositionWeight} rot={ik.data.targetRotationWeight} hint={ik.data.hintWeight}",
                    "layers=" + string.Join(",", builder.layers.Select(l => l.name + ":" + l.active)),
                    "prefab=" + H2F03Fixture.ContactPrefab,
                });
            });
            Try("input.asset", () =>
            {
                var input = AssetDatabase.LoadAssetAtPath<InputActionAsset>(J2FoundationBaseline.InputAsset);
                var rows = input.actionMaps.SelectMany(m => m.actions.Select(a => m.name + "/" + a.name + "=" + string.Join(",", a.bindings.Select(b => b.path)))).ToArray();
                Add("input.asset", input != null, rows);
                Add("gc2.S10", input != null, rows);
            });
            Try("presets.character_camera", () => Add("presets.character_camera", true,
                new[] { J2FoundationBaseline.PlayerPreset, J2FoundationBaseline.NpcPreset, J2FoundationBaseline.CameraPreset, H2F03Fixture.PlayerPreset, H2F03Fixture.WalkerPreset, H2F03Fixture.ContactPreset }
                    .Select(p => p + "=" + H2F03Io.Sha(EditorJsonUtility.ToJson(AssetDatabase.LoadMainAssetAtPath(p)))).ToArray()));
            Try("gc2.S3", () =>
            {
                var rows = new List<string>();
                foreach (var key in new[] { H2F03Fixture.PlayerKey, H2F03Fixture.WalkerKey, H2F03Fixture.ContactKey })
                {
                    var c = Character(sidecar, key);
                    rows.Add(key + "=" + (c == null ? "missing" : Serialized(c) + " preset=" + AssetDatabase.GetAssetPath(c.GetComponent<J2PresetRealization>()?.preset)));
                }
                Add("gc2.S3", rows.All(r => !r.EndsWith("=missing")), rows.ToArray());
                Add("gc2.S4", true, new[] { H2F03Fixture.PlayerKey, H2F03Fixture.WalkerKey, H2F03Fixture.ContactKey }.Select(key =>
                {
                    var anim = Character(sidecar, key)?.GetComponentInChildren<Animator>(true);
                    return key + "|model=" + (anim != null ? anim.name : "none") + "|avatar=" + (anim != null && anim.avatar != null && anim.avatar.isHuman) +
                           "|controller=" + (anim != null && anim.runtimeAnimatorController != null ? anim.runtimeAnimatorController.name : "none") +
                           "|bones=" + (anim != null ? anim.GetComponentsInChildren<Transform>(true).Length : 0);
                }).ToArray());
                var runtimeOnly = sidecar.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Character>(true))
                    .Select(c => c.name + "|characterController=" + (c.GetComponent<CharacterController>() != null) + "|navMeshAgent=" + (c.GetComponent<NavMeshAgent>() != null)).ToArray();
                Add("gc2.S5", runtimeOnly.Any(r => r.Contains("=True")), runtimeOnly);
            });
            Try("gc2.S6", () =>
            {
                var triggers = sidecar.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Trigger>(true)).Select(t => t.name + "=" + Serialized(t));
                var hotspots = sidecar.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Hotspot>(true)).Select(h => h.name + "=" + Serialized(h));
                var lint = J2Gc2Lint.CheckScene(sidecar).Select(f => f.ToString()).ToArray();
                Add("gc2.S6", triggers.Any(), triggers.Concat(hotspots).Concat(new[] { "lint=" + string.Join(",", lint) }).ToArray());
                Add("gc2.lint", lint.Length > 0, lint);
            });
            Try("gc2.S7", () => Add("gc2.S7", AssetDatabase.FindAssets("t:MaterialSoundsAsset", new[] { "Assets/H2F03Fixture", "Assets/Juego2", "Assets/Arkus" }).Length > 0,
                "materialSoundsAssets=" + AssetDatabase.FindAssets("t:MaterialSoundsAsset", new[] { "Assets/H2F03Fixture", "Assets/Juego2", "Assets/Arkus" }).Length));
            Try("gc2.S15", () =>
            {
                var shot = sidecar.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<ShotCamera>(true)).FirstOrDefault();
                var main = sidecar.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<MainCamera>(true)).FirstOrDefault();
                Add("gc2.S15", shot != null && main != null, "shot=" + (shot != null ? Serialized(shot) : "none"), "main=" + (main != null ? main.name + " tag=" + main.tag : "none"));
            });
            Try("gc2.not_admitted", () =>
            {
                var gc2Components = sidecar.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Component>(true))
                    .Where(c => c != null && c.GetType().FullName.StartsWith("GameCreator.", StringComparison.Ordinal)).Select(c => c.GetType().Name).Distinct().OrderBy(n => n).ToArray();
                var saves = J2Gc2Lint.CheckSaveImplementations().Select(f => f.ToString()).ToArray();
                Add("gc2.S11", gc2Components.Any(n => n.Contains("Variables")), "gc2Components=" + string.Join(",", gc2Components));
                Add("gc2.S12", saves.Length > 0, "saveLint=" + string.Join(",", saves));
                Add("gc2.S14", gc2Components.Contains("Remember"), "remember=" + gc2Components.Contains("Remember"));
                Add("gc2.scene_instructions", J2Gc2Lint.CheckScene(sidecar).Any(f => f.Code.Contains("SCENE")), "sceneInstructionLint=" + J2Gc2Lint.CheckScene(sidecar).Count(f => f.Code.Contains("SCENE")));
                var gc2Controllers = sidecar.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Animator>(true))
                    .Where(a => a.runtimeAnimatorController != null && AssetDatabase.GetAssetPath(a.runtimeAnimatorController).StartsWith("Assets/Plugins/GameCreator")).Select(a => a.name).ToArray();
                Add("gc2.S9", gc2Controllers.Length > 0, "gc2LocomotionControllersInUse=" + string.Join(",", gc2Controllers));
                var singletons = sidecar.GetRootGameObjects().Where(g => g.name.Contains("Manager") && g.GetComponents<Component>().Any(c => c != null && c.GetType().FullName.StartsWith("GameCreator."))).Select(g => g.name).ToArray();
                Add("gc2.S13", singletons.Length > 0, "serializedSingletons=" + string.Join(",", singletons));
            });
            Try("gc2.adapter_code", () =>
            {
                var editor = CompilationPipeline.GetAssemblies(AssembliesType.Editor).Select(a => a.name).Where(n => n.StartsWith("Juego2.Gc2Adapter") || n.StartsWith("GameCreator.")).OrderBy(n => n).ToArray();
                Add("gc2.adapter_code", editor.Contains("Juego2.Gc2Adapter"), "assemblies=" + string.Join(",", editor),
                    "interactRelay=" + (typeof(J2Gc2InteractInput).Assembly.GetName().Name), "gc2Version=" + J2Gc2Provisioning.State(out var f) + " " + string.Join(",", f));
            });
            Try("cinemachine", () => Add("cinemachine", AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name.StartsWith("Unity.Cinemachine")),
                "cinemachineAssemblies=" + string.Join(",", AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetName().Name).Where(n => n.StartsWith("Unity.Cinemachine")))));
            Try("fixture.marker", () => Add("fixture.marker", sidecar.GetRootGameObjects().Any(g => g.name == H2F03Fixture.Marker), "root=" + H2F03Fixture.Marker, "scene=" + H2F03Fixture.ScenePath));

            // --- checks that regenerate in memory (never saved), last
            Try("projection.collision", () =>
            {
                var root = Generated(sidecar).FirstOrDefault(g => g.generator == J2CollisionRealizer.Generator);
                var sources = managed.GetRootGameObjects().Select(g => g.transform).ToList();
                Add("projection.collision", root != null, "in=" + root?.inputDigest, "out=" + root?.outputDigest,
                    "currentProjectionIn=" + J2CollisionRealizer.InputDigest(sources), "stale=" + (root == null || J2CollisionRealizer.IsStale(root.gameObject, sources)),
                    "recomputedOut=" + (root != null ? J2CollisionRealizer.OutputDigest(root.gameObject) : ""));
            });
            Try("scatter.realization", () =>
            {
                var root = Generated(sidecar).FirstOrDefault(g => g.generator == J2ScatterRealizer.Generator);
                string stored = root?.outputDigest, input = root?.inputDigest;
                int count = root != null ? root.transform.childCount : 0;
                var host = root != null ? root.transform.parent : null;
                var regenerated = host != null ? J2ScatterRealizer.Realize(host, new Bounds(new Vector3(0, 5, 0), new Vector3(60, 14, 60)),
                    AssetDatabase.LoadAssetAtPath<J2ScatterProfile>(H2F03Fixture.ScatterProfile), H2F03Fixture.ScatterSeed).GetComponent<J2GeneratedRealization>() : null;
                Add("scatter.realization", root != null, "in=" + input, "out=" + stored, "instances=" + count,
                    "regeneratedOut=" + regenerated?.outputDigest, "regeneratedEqual=" + (regenerated != null && regenerated.outputDigest == stored));
            });

            report.records = records.ToArray();
            report.errors = errors.ToArray();
            H2F03Io.WriteJson($"witness_unity_{label}.json", report);
            Debug.Log($"H2F03_WITNESS label={label} records={records.Count} errors={errors.Count}");
            H2F03Io.Exit(errors.Count == 0 ? 0 : 3);
        }

        // ------------------------------------------------------------------ documented restoration steps (GENERATED state)

        /// <summary>Re-realizes the collision proxies from the current H1 projection (H2F-03 row <c>projection.collision</c>).</summary>
        public static void RealizeCollision()
        {
            var sidecar = EditorSceneManager.OpenScene(H2F03Fixture.ScenePath, OpenSceneMode.Single);
            var managed = H2F03Fixture.OpenManaged(out _, out _);
            SceneManager.SetActiveScene(sidecar);
            var host = sidecar.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).First(t => t.name == "J2_CanonicalCollision");
            var root = J2CollisionRealizer.Realize(managed.GetRootGameObjects().Select(g => g.transform), host, "canonical_slice").GetComponent<J2GeneratedRealization>();
            EditorSceneManager.SaveScene(sidecar);
            EditorSceneManager.CloseScene(managed, true);
            H2F03Io.WriteText("restore_collision.json", JsonUtility.ToJson(new Record { family = "projection.collision", present = true, facts = new[] { "in=" + root.inputDigest, "out=" + root.outputDigest } }, true) + "\n");
            Debug.Log($"H2F03_COLLISION_REALIZED in={root.inputDigest}");
            H2F03Io.Exit(0);
        }

        /// <summary>Re-bakes the NavMeshData (H2F-02 row <c>navmesh.data</c>: disposable, rebaked by the scene owner).</summary>
        public static void BakeNavMesh()
        {
            var sidecar = EditorSceneManager.OpenScene(H2F03Fixture.ScenePath, OpenSceneMode.Single);
            var surface = sidecar.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<NavMeshSurface>(true)).First();
            surface.BuildNavMesh();
            AssetDatabase.DeleteAsset(H2F03Fixture.NavMeshAsset);
            AssetDatabase.CreateAsset(surface.navMeshData, H2F03Fixture.NavMeshAsset);
            EditorSceneManager.MarkSceneDirty(sidecar);
            EditorSceneManager.SaveScene(sidecar);
            AssetDatabase.SaveAssets();
            Debug.Log($"H2F03_NAVMESH_BAKED triangles={NavMesh.CalculateTriangulation().indices.Length / 3}");
            H2F03Io.Exit(0);
        }

        // ------------------------------------------------------------------ helpers

        static IEnumerable<J2GeneratedRealization> Generated(Scene scene) =>
            scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<J2GeneratedRealization>(true));

        static Character Character(Scene scene, string key) =>
            scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<ArkusEntityBinding>(true)).Where(b => b.entityKey == key)
                .Select(b => b.GetComponent<Character>()).FirstOrDefault(c => c != null);

        static IEnumerable<string> Features(UniversalRenderPipelineAsset urp)
        {
            if (urp == null) yield break;
            foreach (var data in urp.rendererDataList.ToArray())
            {
                if (data == null) continue;
                foreach (var f in data.rendererFeatures) if (f != null) yield return data.name + ":" + f.GetType().Name + (f.isActive ? "" : "(off)");
            }
        }

        /// <summary>Serialized state as the Inspector sees it (managed references by type, objects by asset/scene path).</summary>
        public static string Serialized(UnityEngine.Object o)
        {
            var s = new StringBuilder();
            var it = new SerializedObject(o).GetIterator();
            while (it.Next(true))
            {
                if (it.propertyPath == "m_ObjectHideFlags" || it.propertyPath.StartsWith("m_GameObject") || it.propertyPath.StartsWith("m_EditorClassIdentifier")) continue;
                string v;
                switch (it.propertyType)
                {
                    case SerializedPropertyType.ObjectReference:
                        var r = it.objectReferenceValue;
                        v = r == null ? "null" : AssetDatabase.Contains(r) ? AssetDatabase.GetAssetPath(r) + ":" + r.name : "scene:" + r.name;
                        break;
                    case SerializedPropertyType.ManagedReference: v = it.managedReferenceFullTypename; break;
                    case SerializedPropertyType.Float: v = it.floatValue.ToString("0.#####"); break;
                    case SerializedPropertyType.Integer: v = it.longValue.ToString(); break;
                    case SerializedPropertyType.Boolean: v = it.boolValue.ToString(); break;
                    case SerializedPropertyType.String: v = it.stringValue; break;
                    case SerializedPropertyType.Enum: v = it.enumValueIndex.ToString(); break;
                    default: continue;
                }
                s.Append(it.propertyPath).Append('=').Append(v).Append('\n');
            }
            return H2F03Io.Sha(s.ToString());
        }

        static string Path(Transform t)
        {
            var parts = new List<string>();
            for (var c = t; c != null; c = c.parent) parts.Add(c.name);
            parts.Reverse();
            return string.Join("/", parts);
        }

        static string V(Vector3 v) => $"{v.x:0.###},{v.y:0.###},{v.z:0.###}";
        static string C(Color c) => $"{c.r:0.###},{c.g:0.###},{c.b:0.###},{c.a:0.###}";
    }
}
