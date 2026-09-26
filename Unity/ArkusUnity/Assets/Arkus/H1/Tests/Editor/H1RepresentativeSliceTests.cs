using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Arkus.H1.Projection;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Arkus.H1.Editor.Tests
{
    /// <summary>
    /// WP-H1-11 effective Unity stages over the representative real-source slice. Each stage runs in its own Editor
    /// process. Import facts are compared with expectations recomputed from the adopted source bytes
    /// (Docs/evidence/WP-H1-11/REPRESENTATIVE_SLICE.json), never with values read back from Unity. The canonical plan is
    /// passed to the accepted worker entry points byte-for-byte; parity is decided by the .NET product verifier.
    /// </summary>
    public sealed class H1RepresentativeSliceTests
    {
        private const string SourceRoot = "Assets/Arkus/H1/SourceSlice";
        private const string ManagedScenes = "Assets/Arkus/H1/ManagedScenes";
        private const string ManagedPrefabs = "Assets/Arkus/H1/ManagedPrefabs";
        private const string ManifestPath = ManagedScenes + "/current.json";
        private const string SceneId = "arkus.h1-05.scene.potes";
        private const string ErrorShader = "Hidden/InternalErrorShader";
        private const string RemovedAsset = SourceRoot + "/Prop_Crate.fbx";
        private const string RemovedNode = "crate.stack";
        private const string ReplacedAsset = SourceRoot + "/Prop_Wagon.fbx";
        private const string ReplacedNode = "wagon.street";
        private const int CaptureWidth = 960;
        private const int CaptureHeight = 540;

        [Test]
        public void Stage0_EffectiveImportAndCatalogueMatchTheAdoptedSource()
        {
            Assert.That(AssetDatabase.IsValidFolder(SourceRoot), Is.True, "accepted H1 SourceSlice must be mounted");
            var manifest = ReadSliceManifest();
            var mapping = ReadMapping();
            var failures = new List<string>();

            // The accepted H1-04 inventory over the mounted slice must equal the committed catalogue snapshot.
            var captured = H1CatalogueInventory.Capture();
            File.WriteAllText(Path.Combine(ProofDirectory(), "effective-inventory.json"), JsonUtility.ToJson(captured, true));
            var committed = JsonUtility.FromJson<EffectiveInventory>(File.ReadAllText(Path.Combine(RepositoryRoot(), "Docs", "evidence", "WP-H1-04", "EFFECTIVE_INVENTORY.json")));
            if (JsonUtility.ToJson(captured) != JsonUtility.ToJson(committed)) failures.Add("effective catalogue inventory differs from the committed snapshot");
            var report = new ImportObservation { schemaId = "arkus.h1-11-import-observation@1" };
            var tolerance = manifest.importRecipe.boundsToleranceMetres;
            var sharedByName = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);

            foreach (var item in manifest.items)
            {
                var observed = new ImportedItem { assetPath = item.assetPath };
                report.items.Add(observed);
                observed.guid = AssetDatabase.AssetPathToGUID(item.assetPath);
                if (observed.guid != item.unityGuid) failures.Add(item.assetPath + ": GUID " + observed.guid + " != " + item.unityGuid);
                var root = AssetDatabase.LoadMainAssetAtPath(item.assetPath) as GameObject;
                if (root == null) { failures.Add(item.assetPath + ": no imported model prefab"); continue; }

                // Nested hierarchy: Unity's prefab tree must equal the FBX node tree under the declared root-collapse rule.
                var expectedPaths = ExpectedUnityPaths(item.fbx.nodes);
                observed.transformPaths = RelativePaths(root.transform, root.transform).ToArray();
                if (!expectedPaths.Keys.OrderBy(value => value, StringComparer.Ordinal).SequenceEqual(observed.transformPaths, StringComparer.Ordinal))
                    failures.Add(item.assetPath + ": hierarchy " + string.Join(",", observed.transformPaths.Take(8)) + " != " + string.Join(",", expectedPaths.Keys.Take(8)));

                // Pivot/scale/axis: the model root is an identity pivot and every mesh keeps its source-relative bounds.
                if (item.fbx.upAxis == 1)
                {
                    if (root.transform.localPosition != Vector3.zero || root.transform.localScale != Vector3.one ||
                        Quaternion.Angle(root.transform.localRotation, Quaternion.identity) > 0.01f)
                        failures.Add(item.assetPath + ": imported root is not an identity pivot");
                }
                var scale = item.fbx.unitScaleFactor / 100f;
                foreach (var expected in item.fbx.meshes)
                {
                    var unityPath = expectedPaths.FirstOrDefault(pair => pair.Value == expected.node).Key;
                    var holder = unityPath == null ? null : Find(root.transform, unityPath);
                    if (holder == null) { failures.Add(item.assetPath + ": mesh node " + expected.node + " missing"); continue; }
                    var renderer = holder.GetComponent<Renderer>();
                    var mesh = holder.GetComponent<SkinnedMeshRenderer>() != null
                        ? holder.GetComponent<SkinnedMeshRenderer>().sharedMesh
                        : holder.GetComponent<MeshFilter>() == null ? null : holder.GetComponent<MeshFilter>().sharedMesh;
                    var meshRow = new ImportedMesh { node = expected.node };
                    observed.meshes.Add(meshRow);
                    if (mesh == null || renderer == null) { failures.Add(item.assetPath + ": " + expected.node + " has no mesh/renderer"); continue; }
                    meshRow.meshName = mesh.name;
                    meshRow.min = new[] { mesh.bounds.min.x, mesh.bounds.min.y, mesh.bounds.min.z };
                    meshRow.max = new[] { mesh.bounds.max.x, mesh.bounds.max.y, mesh.bounds.max.z };
                    var leaf = expected.node.Substring(expected.node.LastIndexOf('/') + 1);
                    if (mesh.name != leaf) failures.Add(item.assetPath + ": mesh name " + mesh.name + " != FBX node " + leaf);
                    var expectedMin = new[] { -expected.boundsSource.max[0] * scale, expected.boundsSource.min[1] * scale, expected.boundsSource.min[2] * scale };
                    var expectedMax = new[] { -expected.boundsSource.min[0] * scale, expected.boundsSource.max[1] * scale, expected.boundsSource.max[2] * scale };
                    for (var axis = 0; axis < 3; axis++)
                    {
                        if (Math.Abs(meshRow.min[axis] - expectedMin[axis]) > tolerance || Math.Abs(meshRow.max[axis] - expectedMax[axis]) > tolerance)
                        {
                            failures.Add(item.assetPath + ": bounds " + Vec(meshRow.min) + ".." + Vec(meshRow.max) + " != " + Vec(expectedMin) + ".." + Vec(expectedMax));
                            break;
                        }
                    }

                    // Material slots: every FBX material is bound, compatible and in source order.
                    var slots = renderer.sharedMaterials;
                    meshRow.materials = slots.Select(material => material == null ? "<missing>" : AssetDatabase.GetAssetPath(material) + "|" + material.shader.name).ToArray();
                    if (slots.Length != expected.materials.Length) failures.Add(item.assetPath + ": material slots " + slots.Length + " != " + expected.materials.Length);
                    for (var slot = 0; slot < Math.Min(slots.Length, expected.materials.Length); slot++)
                    {
                        var material = slots[slot];
                        if (material == null || material.shader == null || material.shader.name == ErrorShader)
                        {
                            failures.Add(item.assetPath + ": slot " + slot + " (" + expected.materials[slot] + ") missing or incompatible");
                            continue;
                        }
                        if (!material.name.EndsWith(expected.materials[slot], StringComparison.Ordinal))
                            failures.Add(item.assetPath + ": slot " + slot + " material " + material.name + " != " + expected.materials[slot]);
                        if (item.provenance == "distribution-entry")
                        {
                            if (!sharedByName.TryGetValue(expected.materials[slot], out var paths)) sharedByName[expected.materials[slot]] = paths = new HashSet<string>(StringComparer.Ordinal);
                            paths.Add(AssetDatabase.GetAssetPath(material));
                        }
                    }

                    if (renderer is SkinnedMeshRenderer skinned)
                    {
                        meshRow.bones = skinned.bones.Length;
                        if (skinned.bones.Length < 20 || skinned.bones.Any(bone => bone == null) || skinned.rootBone == null ||
                            skinned.bones.Any(bone => !bone.IsChildOf(root.transform)))
                            failures.Add(item.assetPath + ": humanoid rig bones are missing or outside the model");
                        if (mesh.bindposes.Length != skinned.bones.Length) failures.Add(item.assetPath + ": bind poses do not match bones");
                    }
                }
            }

            // Shared materials: the upstream import settings bind one kit material per source material name.
            foreach (var pair in sharedByName.OrderBy(value => value.Key, StringComparer.Ordinal))
            {
                report.sharedMaterials.Add(pair.Key + "=" + string.Join("|", pair.Value.OrderBy(value => value, StringComparer.Ordinal)));
                if (pair.Value.Count != 1) failures.Add("material " + pair.Key + " is not shared: " + string.Join(",", pair.Value));
            }
            if (!sharedByName.Values.Any(paths => paths.Count == 1)) failures.Add("no shared kit material was observed");

            // Humanoid clip references resolve from logical identity to the exact native clip.
            foreach (var clip in manifest.clips)
            {
                var row = mapping.entries.SingleOrDefault(entry => entry.logicalId == clip.logicalId);
                var resolved = row == null ? null : Resolve<AnimationClip>(row.nativeGuid, row.localFileId);
                report.clips.Add(clip.role + "=" + (resolved == null ? "<missing>" : resolved.name + "|" + resolved.length.ToString("R", CultureInfo.InvariantCulture)));
                if (resolved == null || resolved.length <= 0f) failures.Add("clip " + clip.role + " does not resolve to a non-empty AnimationClip");
            }

            report.failures = failures.ToArray();
            File.WriteAllText(Path.Combine(ProofDirectory(), "import-observation.json"), JsonUtility.ToJson(report, true));
            Assert.That(failures, Is.Empty, string.Join("\n", failures));
        }

        [Test]
        public void StageA_MaterializeSaveReloadAndInspectTheStreetCorner()
        {
            Assert.That(AssetDatabase.IsValidFolder(SourceRoot), Is.True, "accepted H1 SourceSlice must be mounted");
            DeleteGenerated();
            var rawPlan = ReadRawPlan("plan.json", out var plan);
            var sourceBefore = SourceHashes();
            var materialized = Execute("materialize", rawPlan, out var raw);
            Assert.That(materialized.errorCode, Is.Empty, raw);
            Assert.That(materialized.observation.active, Is.True);
            Assert.That(materialized.observation.inputDigest, Is.EqualTo(plan.inputDigest));

            var observed = Reconcile();
            RequireCleanObservation(observed, plan);
            // observe reopens the published generation from disk: this is the save/reload inspection.
            WriteObservation(rawPlan, "baseline-observation.json");
            var inspection = InspectRealizedSlice(plan);
            File.WriteAllText(Path.Combine(ProofDirectory(), "baseline-inspection.json"), JsonUtility.ToJson(inspection, true));
            Assert.That(SourceHashes(), Is.EqualTo(sourceBefore), "the bridge modified purchased upstream source bytes");
            File.WriteAllText(Path.Combine(ProofDirectory(), "baseline.json"), JsonUtility.ToJson(new StageEvidence
            {
                schemaId = "arkus.h1-11-baseline@1",
                inputDigest = observed.observation.inputDigest,
                canonicalHash = observed.observation.canonicalHash,
                catalogueFingerprint = observed.observation.catalogueFingerprint,
                graphDigest = observed.observation.graphDigest,
                realizationDigest = observed.observation.realizationDigest
            }));
        }

        [Test]
        public void StageA2_SupplementaryRenderedCaptureIsNotTheParityOracle()
        {
            var rawPlan = ReadRawPlan("plan.json", out _);
            var observed = Execute("observe", rawPlan, out var raw);
            Assert.That(observed.errorCode, Is.Empty, raw);
            Assert.That(observed.observation.active, Is.True, "the capture renders the published generation only");
            var managed = SceneManager.GetActiveScene();
            var evidence = new CaptureEvidence
            {
                schemaId = "arkus.h1-11-supplementary-capture@1",
                graphicsDevice = SystemInfo.graphicsDeviceType.ToString(),
                width = CaptureWidth,
                height = CaptureHeight,
                graphDigest = observed.observation.graphDigest
            };
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
            {
                evidence.state = "graphics-unavailable";
                File.WriteAllText(Path.Combine(ProofDirectory(), "capture.json"), JsonUtility.ToJson(evidence, true));
                Assert.Ignore("No graphics device in this Editor process; the supplementary capture needs a graphics-capable run.");
            }

            var renderers = managed.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<Renderer>(true)).ToArray();
            Assert.That(renderers, Is.Not.Empty);
            var bounds = renderers[0].bounds;
            foreach (var renderer in renderers) bounds.Encapsulate(renderer.bounds);
            var temporary = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            RenderTexture target = null;
            Texture2D pixels = null;
            try
            {
                // Camera and light live only in an unsaved scratch scene, never in the managed generation.
                SceneManager.SetActiveScene(temporary);
                var cameraObject = new GameObject("h1-11-capture-camera");
                SceneManager.MoveGameObjectToScene(cameraObject, temporary);
                var camera = cameraObject.AddComponent<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0.55f, 0.70f, 0.85f, 1f);
                camera.fieldOfView = 40f;
                camera.nearClipPlane = 0.05f;
                camera.farClipPlane = 500f;
                var distance = bounds.extents.magnitude / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * 1.05f;
                camera.transform.position = bounds.center + new Vector3(-0.55f, 0.5f, 1f).normalized * distance;
                camera.transform.LookAt(bounds.center);
                var lightObject = new GameObject("h1-11-capture-light");
                SceneManager.MoveGameObjectToScene(lightObject, temporary);
                var light = lightObject.AddComponent<Light>();
                light.type = LightType.Directional;
                light.intensity = 1.2f;
                lightObject.transform.rotation = Quaternion.Euler(50f, 150f, 0f);

                target = new RenderTexture(CaptureWidth, CaptureHeight, 24, RenderTextureFormat.ARGB32);
                camera.targetTexture = target;
                camera.Render();
                RenderTexture.active = target;
                pixels = new Texture2D(CaptureWidth, CaptureHeight, TextureFormat.RGB24, false);
                pixels.ReadPixels(new Rect(0, 0, CaptureWidth, CaptureHeight), 0, 0);
                pixels.Apply();
                RenderTexture.active = null;
                camera.targetTexture = null;

                var background = camera.backgroundColor;
                var data = pixels.GetPixels32();
                var covered = data.Count(pixel => Math.Abs(pixel.r / 255f - background.r) + Math.Abs(pixel.g / 255f - background.g) + Math.Abs(pixel.b / 255f - background.b) > 0.06f);
                evidence.coveredPixelFraction = covered / (float)data.Length;
                evidence.distinctColours = data.Select(pixel => (pixel.r >> 3) << 10 | (pixel.g >> 3) << 5 | (pixel.b >> 3)).Distinct().Count();
                var encoded = ImageConversion.EncodeToJPG(pixels, 88);
                evidence.jpegSha256 = Sha(encoded);
                evidence.state = "rendered";
                File.WriteAllBytes(Path.Combine(ProofDirectory(), "capture.jpg"), encoded);
            }
            finally
            {
                RenderTexture.active = null;
                if (target != null) UnityEngine.Object.DestroyImmediate(target);
                if (pixels != null) UnityEngine.Object.DestroyImmediate(pixels);
                SceneManager.SetActiveScene(managed);
                EditorSceneManager.CloseScene(temporary, true);
            }
            File.WriteAllText(Path.Combine(ProofDirectory(), "capture.json"), JsonUtility.ToJson(evidence, true));
            Assert.That(managed.isDirty, Is.False, "the capture must not mutate the managed generation");
            Assert.That(evidence.coveredPixelFraction, Is.GreaterThan(0.05f), "the rendered capture shows no geometry");
            Assert.That(evidence.distinctColours, Is.GreaterThan(24), "the rendered capture is flat");
        }

        [Test]
        public void StageB_CleanGeneratedOutputRebuildOfTheRealSliceInFreshEditor()
        {
            Assert.That(AssetDatabase.IsValidFolder(SourceRoot), Is.True, "accepted H1 SourceSlice must be mounted");
            AssetDatabase.Refresh();
            Assert.That(AssetDatabase.IsValidFolder(ManagedScenes), Is.False, "workflow must remove all generated scenes before rebuild");
            Assert.That(AssetDatabase.IsValidFolder(ManagedPrefabs), Is.False, "workflow must remove all generated prefabs before rebuild");

            var rawPlan = ReadRawPlan("restored-plan.json", out var plan);
            var baseline = JsonUtility.FromJson<StageEvidence>(File.ReadAllText(Path.Combine(ProofDirectory(), "baseline.json")));
            Assert.That(plan.inputDigest, Is.EqualTo(baseline.inputDigest));
            var sourceBefore = SourceHashes();

            var rebuiltRaw = H1SceneProjection.ExecuteCleanRebuild(Request("clean-rebuild", rawPlan));
            var rebuilt = JsonUtility.FromJson<ProjectionReply>(rebuiltRaw);
            Assert.That(rebuilt, Is.Not.Null, rebuiltRaw);
            Assert.That(rebuilt.errorCode, Is.Empty, rebuiltRaw);
            Assert.That(rebuilt.observation.active, Is.True, rebuiltRaw);

            var observed = Reconcile();
            RequireCleanObservation(observed, plan);
            Assert.That(observed.observation.graphDigest, Is.EqualTo(baseline.graphDigest));
            WriteObservation(rawPlan, "rebuilt-observation.json");
            var inspection = InspectRealizedSlice(plan);
            File.WriteAllText(Path.Combine(ProofDirectory(), "rebuilt-inspection.json"), JsonUtility.ToJson(inspection, true));
            Assert.That(SourceHashes(), Is.EqualTo(sourceBefore), "the rebuild modified purchased upstream source bytes");
        }

        [Test]
        public void StageC_RemovedOrReplacedSelectedAssetYieldsNamedDiagnosticsNotSubstitution()
        {
            Assert.That(File.Exists(ProjectPath(ManifestPath)), Is.True, "a published generation is required");
            var rawPlan = ReadRawPlan("restored-plan.json", out _);
            var baseline = Reconcile();
            Assert.That(baseline.observation.diagnostics, Is.Empty);
            var published = File.ReadAllText(ProjectPath(ManifestPath));
            var removedBytes = File.ReadAllBytes(ProjectPath(RemovedAsset));
            var removedMeta = File.ReadAllBytes(ProjectPath(RemovedAsset) + ".meta");
            var replacedBytes = File.ReadAllBytes(ProjectPath(ReplacedAsset));
            var evidence = new NegativeEvidence { schemaId = "arkus.h1-11-negative-evidence@1" };
            try
            {
                // 1. One selected real asset disappears from the mounted source.
                File.Delete(ProjectPath(RemovedAsset));
                File.Delete(ProjectPath(RemovedAsset) + ".meta");
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                File.WriteAllText(Path.Combine(ProofDirectory(), "negative-removed-inventory.json"), JsonUtility.ToJson(H1CatalogueInventory.Capture(), true));
                var removed = Reconcile();
                evidence.removedDiagnostics = Rows(removed);
                Assert.That(removed.observation.diagnostics.Any(row => row.subject == RemovedNode), Is.True, string.Join(",", evidence.removedDiagnostics));
                var removedAttempt = Execute("materialize", rawPlan, out var removedRaw);
                evidence.removedMaterializeError = removedAttempt.errorCode;
                Assert.That(removedAttempt.errorCode, Is.Not.Empty, removedRaw);
                Assert.That(File.ReadAllText(ProjectPath(ManifestPath)), Is.EqualTo(published), "a failed materialization replaced the published generation");

                File.WriteAllBytes(ProjectPath(RemovedAsset), removedBytes);
                File.WriteAllBytes(ProjectPath(RemovedAsset) + ".meta", removedMeta);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

                // 2. Another selected real asset is silently swapped for different upstream bytes under the same identity.
                File.WriteAllBytes(ProjectPath(ReplacedAsset), removedBytes);
                AssetDatabase.ImportAsset(ReplacedAsset, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                File.WriteAllText(Path.Combine(ProofDirectory(), "negative-replaced-inventory.json"), JsonUtility.ToJson(H1CatalogueInventory.Capture(), true));
                var replaced = Reconcile();
                evidence.replacedDiagnostics = Rows(replaced);
                Assert.That(replaced.observation.diagnostics.Any(row => row.subject == ReplacedNode), Is.True, string.Join(",", evidence.replacedDiagnostics));
                var replacedAttempt = Execute("materialize", rawPlan, out var replacedRaw);
                evidence.replacedMaterializeError = replacedAttempt.errorCode;
                Assert.That(replacedAttempt.errorCode, Is.Not.Empty, replacedRaw);
                Assert.That(File.ReadAllText(ProjectPath(ManifestPath)), Is.EqualTo(published), "a failed materialization replaced the published generation");
            }
            finally
            {
                File.WriteAllBytes(ProjectPath(RemovedAsset), removedBytes);
                File.WriteAllBytes(ProjectPath(RemovedAsset) + ".meta", removedMeta);
                File.WriteAllBytes(ProjectPath(ReplacedAsset), replacedBytes);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                AssetDatabase.ImportAsset(ReplacedAsset, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
            }

            // Restoring the exact accepted bytes restores a clean observation of the same published generation.
            var recovered = Reconcile();
            evidence.recoveredDiagnostics = Rows(recovered);
            Assert.That(recovered.observation.diagnostics, Is.Empty, string.Join(",", evidence.recoveredDiagnostics));
            Assert.That(recovered.observation.graphDigest, Is.EqualTo(baseline.observation.graphDigest));
            Assert.That(recovered.observation.realizationDigest, Is.EqualTo(baseline.observation.realizationDigest));
            File.WriteAllText(Path.Combine(ProofDirectory(), "negative.json"), JsonUtility.ToJson(evidence, true));
        }

        // ------------------------------------------------------------------------------------------ inspection

        private static SliceInspection InspectRealizedSlice(ProjectionPlan plan)
        {
            var scene = SceneManager.GetActiveScene();
            var roots = scene.GetRootGameObjects();
            Assert.That(roots.Length, Is.EqualTo(1));
            var root = roots[0];
            var byId = root.GetComponentsInChildren<H1ManagedMarker>(true).Where(marker => marker.role == "object")
                .ToDictionary(marker => marker.canonicalObjectId, marker => marker.gameObject, StringComparer.Ordinal);
            Assert.That(byId.Keys.OrderBy(value => value, StringComparer.Ordinal),
                Is.EqualTo(plan.nodes.Select(node => node.objectId).OrderBy(value => value, StringComparer.Ordinal)));
            var inspection = new SliceInspection { schemaId = "arkus.h1-11-realized-inspection@1" };
            var overrides = new Dictionary<string, HashSet<Material>>(StringComparer.Ordinal);

            foreach (var node in plan.nodes.OrderBy(value => value.objectId, StringComparer.Ordinal))
            {
                var instance = byId[node.objectId];
                var expectedParent = node.parentObjectId.Length == 0 ? root : byId[node.parentObjectId];
                Assert.That(instance.transform.parent, Is.SameAs(expectedParent.transform), node.objectId + " lost its canonical parent");
                var owned = OwnedPaths(instance.transform);
                var row = new InspectedNode { objectId = node.objectId, sourceKind = node.sourceKind, ownedTransforms = owned.Count };
                inspection.nodes.Add(row);

                var renderersOverridden = node.components.Any(component => component.kind == "renderer");
                if (node.sourceKind == "prefab")
                {
                    var source = AssetDatabase.LoadAssetAtPath<GameObject>(node.sourcePath);
                    Assert.That(source, Is.Not.Null, node.sourcePath);
                    Assert.That(AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromOriginalSource(instance)), Is.EqualTo(node.sourcePath));
                    var sourcePaths = RelativePaths(source.transform, source.transform);
                    Assert.That(owned.Keys.OrderBy(value => value, StringComparer.Ordinal), Is.EqualTo(sourcePaths),
                        node.objectId + ": realized hierarchy differs from its source prefab");
                    foreach (var sourceRenderer in source.GetComponentsInChildren<Renderer>(true))
                    {
                        var path = RelativePath(source.transform, sourceRenderer.transform);
                        var realized = owned[path].GetComponent<Renderer>();
                        Assert.That(realized, Is.Not.Null, node.objectId + ": renderer lost at " + path);
                        row.materialSlots += realized.sharedMaterials.Length;
                        RequireCompatible(node.objectId, realized);
                        if (!renderersOverridden)
                            Assert.That(realized.sharedMaterials, Is.EqualTo(sourceRenderer.sharedMaterials), node.objectId + ": source material binding changed");
                        if (realized is SkinnedMeshRenderer skinned)
                        {
                            row.bones = skinned.bones.Length;
                            Assert.That(skinned.bones.All(bone => bone != null && bone.IsChildOf(instance.transform)), Is.True, node.objectId + ": rig bones missing");
                            Assert.That(skinned.sharedMesh, Is.SameAs(((SkinnedMeshRenderer)sourceRenderer).sharedMesh));
                        }
                    }
                }
                else
                {
                    var filter = instance.GetComponent<MeshFilter>();
                    Assert.That(filter != null && filter.sharedMesh != null, Is.True, node.objectId + ": mesh reference missing");
                    Assert.That(Identity(filter.sharedMesh), Is.EqualTo(node.sourceGuid + "|" + node.sourceLocalFileId));
                    RequireCompatible(node.objectId, instance.GetComponent<Renderer>());
                    row.materialSlots = instance.GetComponent<Renderer>().sharedMaterials.Length;
                }

                foreach (var component in node.components)
                {
                    if (component.kind == "renderer")
                    {
                        var renderer = owned.Values.Select(transform => transform.GetComponent<MeshRenderer>()).Single(value => value != null);
                        Assert.That(Identity(renderer.sharedMaterial), Is.EqualTo(component.referenceGuid + "|" + component.referenceLocalFileId), node.objectId);
                        if (!overrides.TryGetValue(component.referenceLogicalId, out var set)) overrides[component.referenceLogicalId] = set = new HashSet<Material>();
                        set.Add(renderer.sharedMaterial);
                        row.renderer = component.referenceLogicalId;
                    }
                    else if (component.kind == "animator")
                    {
                        var ownership = instance.GetComponent<H1ComponentOwnershipMarker>();
                        Assert.That(instance.GetComponent<Animator>(), Is.Not.Null, node.objectId + ": animator missing after reload");
                        Assert.That(ownership != null && ownership.animatorClip && ownership.animatorClipReference is AnimationClip, Is.True, node.objectId + ": clip reference missing after reload");
                        Assert.That(Identity(ownership.animatorClipReference), Is.EqualTo(component.referenceGuid + "|" + component.referenceLocalFileId), node.objectId);
                        row.clip = component.referenceLogicalId;
                    }
                }
            }

            foreach (var pair in overrides)
            {
                Assert.That(pair.Value.Count, Is.EqualTo(1), "shared catalogue material " + pair.Key + " resolved to more than one object");
                inspection.sharedOverrides.Add(pair.Key);
            }
            return inspection;
        }

        private static void RequireCompatible(string objectId, Renderer renderer)
        {
            Assert.That(renderer, Is.Not.Null, objectId + ": renderer missing");
            Assert.That(renderer.sharedMaterials.Length, Is.GreaterThan(0), objectId + ": no material slot");
            foreach (var material in renderer.sharedMaterials)
                Assert.That(material != null && material.shader != null && material.shader.name != ErrorShader, Is.True,
                    objectId + ": a material slot is missing or incompatible");
        }

        /// <summary>Transforms owned by one canonical node: its subtree minus nested canonical nodes.</summary>
        private static Dictionary<string, Transform> OwnedPaths(Transform owner)
        {
            var result = new Dictionary<string, Transform>(StringComparer.Ordinal) { [""] = owner };
            var pending = new Stack<Transform>();
            foreach (Transform child in owner) pending.Push(child);
            while (pending.Count > 0)
            {
                var current = pending.Pop();
                if (current.GetComponent<H1ManagedMarker>() != null) continue;
                result.Add(RelativePath(owner, current), current);
                foreach (Transform child in current) pending.Push(child);
            }
            return result;
        }

        private static List<string> RelativePaths(Transform owner, Transform current)
        {
            var paths = new List<string> { RelativePath(owner, current) };
            foreach (Transform child in current) paths.AddRange(RelativePaths(owner, child));
            return paths.OrderBy(value => value, StringComparer.Ordinal).ToList();
        }

        private static string RelativePath(Transform owner, Transform target)
        {
            var names = new List<string>();
            for (var current = target; current != owner; current = current.parent) names.Add(current.name);
            names.Reverse();
            return string.Join("/", names);
        }

        private static Transform Find(Transform root, string relativePath) => relativePath.Length == 0 ? root : root.Find(relativePath);

        /// <summary>Declared root-collapse rule: a single top-level FBX node becomes the prefab root.</summary>
        private static Dictionary<string, string> ExpectedUnityPaths(SliceNode[] nodes)
        {
            var result = new Dictionary<string, string>(StringComparer.Ordinal);
            var top = nodes.Where(node => !node.path.Contains("/")).ToArray();
            if (top.Length == 1)
            {
                var prefix = top[0].path + "/";
                foreach (var node in nodes)
                    result[node.path == top[0].path ? "" : node.path.Substring(prefix.Length)] = node.path;
            }
            else
            {
                result[""] = "";
                foreach (var node in nodes) result[node.path] = node.path;
            }
            return result;
        }

        private static T Resolve<T>(string guid, string localFileId) where T : UnityEngine.Object
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path)) return null;
            foreach (var candidate in AssetDatabase.LoadAllAssetsAtPath(path))
                if (candidate is T typed && Identity(candidate) == guid + "|" + localFileId) return typed;
            return null;
        }

        private static string Identity(UnityEngine.Object asset)
        {
            if (asset == null || !AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string guid, out long fileId)) return "<unresolved>";
            return guid + "|" + fileId.ToString(CultureInfo.InvariantCulture);
        }

        private static string[] SourceHashes()
        {
            var root = ProjectPath(SourceRoot);
            return Directory.GetFiles(root).Where(path => !path.EndsWith(".meta", StringComparison.Ordinal))
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(path => Path.GetFileName(path) + "=" + Sha(File.ReadAllBytes(path))).ToArray();
        }

        // --------------------------------------------------------------------------------------- worker I/O

        private static string Request(string mode, string rawPlan) =>
            "{\"schemaId\":\"arkus.h1-projection-worker-request@1\",\"mode\":\"" + mode + "\",\"sceneLogicalId\":\"" + SceneId + "\",\"plan\":" + rawPlan + "}";

        private static ProjectionReply Execute(string mode, string rawPlan, out string raw)
        {
            raw = H1SceneProjection.Execute(Request(mode, rawPlan));
            var reply = JsonUtility.FromJson<ProjectionReply>(raw);
            Assert.That(reply, Is.Not.Null, raw);
            return reply;
        }

        private static void WriteObservation(string rawPlan, string name)
        {
            var observed = Execute("observe", rawPlan, out var raw);
            Assert.That(observed.errorCode, Is.Empty, raw);
            Assert.That(observed.observation.active, Is.True, raw);
            File.WriteAllText(Path.Combine(ProofDirectory(), name), raw);
        }

        private static ReconciliationReply Reconcile()
        {
            var raw = H1SceneProjection.ExecuteReconciliation(JsonUtility.ToJson(new ReconciliationRequest
            {
                schemaId = "arkus.h1-projection-reconciliation-worker-request@1",
                sceneLogicalId = SceneId
            }));
            var reply = JsonUtility.FromJson<ReconciliationReply>(raw);
            Assert.That(reply, Is.Not.Null, raw);
            Assert.That(reply.errorCode, Is.Empty, raw);
            Assert.That(reply.observation, Is.Not.Null, raw);
            return reply;
        }

        private static void RequireCleanObservation(ReconciliationReply reply, ProjectionPlan plan)
        {
            var observation = reply.observation;
            Assert.That(observation.active, Is.True);
            Assert.That(observation.inputDigest, Is.EqualTo(plan.inputDigest));
            Assert.That(observation.canonicalHash, Is.EqualTo(plan.canonicalHash));
            Assert.That(observation.catalogueFingerprint, Is.EqualTo(plan.catalogueFingerprint));
            Assert.That(observation.unmanagedPaths, Is.Empty);
            Assert.That(observation.diagnostics, Is.Empty, string.Join(",", Rows(reply)));
            Assert.That(observation.graphDigest, Is.EqualTo(observation.manifestGraphDigest));
            Assert.That(observation.realizationDigest, Is.EqualTo(observation.manifestRealizationDigest));
        }

        private static string[] Rows(ReconciliationReply reply) =>
            (reply.observation.diagnostics ?? new ReconciliationDiagnostic[0]).Select(row => row.code + "@" + row.subject).ToArray();

        private static string ReadRawPlanText(string name)
        {
            var path = Path.Combine(ProofDirectory(), name);
            Assert.That(File.Exists(path), Is.True, "H1-11 proof plan is missing: " + path);
            return File.ReadAllText(path);
        }

        private static string ReadRawPlan(string name, out ProjectionPlan plan)
        {
            var raw = ReadRawPlanText(name);
            plan = JsonUtility.FromJson<ProjectionPlan>(raw);
            Assert.That(plan, Is.Not.Null);
            Assert.That(plan.schemaId, Is.EqualTo("arkus.h1-managed-scene-plan@1"));
            Assert.That(plan.sceneLogicalId, Is.EqualTo(SceneId));
            Assert.That(plan.nodes, Is.Not.Null.And.Not.Empty);
            return raw;
        }

        private static SliceManifest ReadSliceManifest()
        {
            var manifest = JsonUtility.FromJson<SliceManifest>(File.ReadAllText(Path.Combine(RepositoryRoot(), "Docs", "evidence", "WP-H1-11", "REPRESENTATIVE_SLICE.json")));
            Assert.That(manifest, Is.Not.Null);
            Assert.That(manifest.schemaId, Is.EqualTo("arkus.h1-11-representative-slice@1"));
            Assert.That(manifest.items, Is.Not.Null.And.Not.Empty);
            return manifest;
        }

        private static CatalogueMapping ReadMapping() =>
            JsonUtility.FromJson<CatalogueMapping>(File.ReadAllText(ProjectPath("Assets/Arkus/H1/CatalogueMapping.json")));

        private static string ProofDirectory()
        {
            var directory = Path.Combine(ProjectRoot(), "H1-11-RealAssetSlice");
            Directory.CreateDirectory(directory);
            return directory;
        }

        private static string ProjectRoot() => Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        private static string RepositoryRoot() => Path.GetFullPath(Path.Combine(ProjectRoot(), "..", ".."));
        private static string ProjectPath(string assetPath) => Path.Combine(ProjectRoot(), assetPath.Replace('/', Path.DirectorySeparatorChar));
        private static string Vec(float[] value) => "(" + string.Join(",", value.Select(part => part.ToString("0.####", CultureInfo.InvariantCulture))) + ")";
        private static string Sha(byte[] bytes)
        {
            using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();
        }

        private static void DeleteGenerated()
        {
            if (AssetDatabase.IsValidFolder(ManagedScenes)) AssetDatabase.DeleteAsset(ManagedScenes);
            if (AssetDatabase.IsValidFolder(ManagedPrefabs)) AssetDatabase.DeleteAsset(ManagedPrefabs);
            AssetDatabase.Refresh();
        }

        // ------------------------------------------------------------------------------------------- DTOs

        [Serializable] private sealed class SliceManifest { public string schemaId; public SliceRecipe importRecipe; public SliceItem[] items; public SliceClip[] clips; }
        [Serializable] private sealed class SliceRecipe { public float boundsToleranceMetres; }
        [Serializable] private sealed class SliceItem
        {
            public string provenance; public string sourceId; public string assetPath; public string unityGuid; public string contentSha256;
            public SliceFbx fbx;
        }
        [Serializable] private sealed class SliceFbx { public float unitScaleFactor; public int upAxis; public SliceNode[] nodes; public SliceMesh[] meshes; }
        [Serializable] private sealed class SliceNode { public string path; public string type; }
        [Serializable] private sealed class SliceMesh { public string node; public SliceBounds boundsSource; public string[] materials; }
        [Serializable] private sealed class SliceBounds { public float[] min; public float[] max; }
        [Serializable] private sealed class SliceClip { public string role; public string logicalId; }
        [Serializable] private sealed class CatalogueMapping { public MappingRow[] entries; }
        [Serializable] private sealed class MappingRow { public string logicalId; public string kind; public string nativeGuid; public string localFileId; }

        [Serializable] private sealed class ImportObservation
        {
            public string schemaId; public List<ImportedItem> items = new List<ImportedItem>();
            public List<string> sharedMaterials = new List<string>(); public List<string> clips = new List<string>(); public string[] failures;
        }
        [Serializable] private sealed class ImportedItem { public string assetPath; public string guid; public string[] transformPaths; public List<ImportedMesh> meshes = new List<ImportedMesh>(); }
        [Serializable] private sealed class ImportedMesh { public string node; public string meshName; public float[] min; public float[] max; public string[] materials; public int bones; }
        [Serializable] private sealed class SliceInspection { public string schemaId; public List<InspectedNode> nodes = new List<InspectedNode>(); public List<string> sharedOverrides = new List<string>(); }
        [Serializable] private sealed class InspectedNode
        {
            public string objectId; public string sourceKind; public int ownedTransforms; public int materialSlots; public int bones; public string renderer; public string clip;
        }
        [Serializable] private sealed class CaptureEvidence
        {
            public string schemaId; public string state; public string graphicsDevice; public int width; public int height; public string graphDigest;
            public float coveredPixelFraction; public int distinctColours; public string jpegSha256;
        }
        [Serializable] private sealed class NegativeEvidence
        {
            public string schemaId; public string[] removedDiagnostics; public string removedMaterializeError; public string[] replacedDiagnostics;
            public string replacedMaterializeError; public string[] recoveredDiagnostics;
        }
        [Serializable] private sealed class StageEvidence
        {
            public string schemaId; public string inputDigest; public string canonicalHash; public string catalogueFingerprint; public string graphDigest; public string realizationDigest;
        }
        [Serializable] private sealed class ProjectionPlan
        {
            public string schemaId; public string sceneLogicalId; public string inputDigest; public string canonicalHash; public string catalogueFingerprint;
            public ProjectionNode[] nodes;
        }
        [Serializable] private sealed class ProjectionNode
        {
            public string objectId; public string parentObjectId; public string sourceKind; public string sourceLogicalId; public string sourcePath;
            public string sourceGuid; public string sourceLocalFileId; public ProjectionComponent[] components;
        }
        [Serializable] private sealed class ProjectionComponent
        {
            public string schemaId; public string kind; public string referenceLogicalId; public string referenceGuid; public string referenceLocalFileId;
        }
        [Serializable] private sealed class ProjectionReply { public string errorCode; public ProjectionObservation observation; }
        [Serializable] private sealed class ProjectionObservation
        {
            public bool active; public string inputDigest; public string canonicalHash; public string catalogueFingerprint; public string graphDigest;
        }
        [Serializable] private sealed class ReconciliationRequest { public string schemaId; public string sceneLogicalId; }
        [Serializable] private sealed class ReconciliationReply { public string errorCode; public ReconciliationObservation observation; }
        [Serializable] private sealed class ReconciliationObservation
        {
            public bool active; public string inputDigest; public string canonicalHash; public string catalogueFingerprint;
            public string manifestGraphDigest; public string manifestRealizationDigest; public string graphDigest; public string realizationDigest;
            public string[] unmanagedPaths; public ReconciliationDiagnostic[] diagnostics;
        }
        [Serializable] private sealed class ReconciliationDiagnostic { public string code; public string subject; }
    }
}
