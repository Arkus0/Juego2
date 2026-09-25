using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Arkus.H1.Editor.Tests
{
    public sealed class H1ComponentProjectionTests
    {
        private const string SourceRoot = "Assets/Arkus/H1/SourceSlice";
        private const string ManagedScenes = "Assets/Arkus/H1/ManagedScenes";
        private const string ManagedPrefabs = "Assets/Arkus/H1/ManagedPrefabs";
        private const string SceneId = "arkus.h1-05.scene.potes";
        private const string TransformSchema = "arkus.h1.component.transform@1";
        private const string RendererSchema = "arkus.h1.component.mesh-renderer@1";
        private const string AnimatorSchema = "arkus.h1.component.animator@1";
        private const string LinkSchema = "arkus.h1.component.canonical-link@1";

        [SetUp]
        public void SetUp()
        {
            DeleteGenerated();
            Assert.That(AssetDatabase.IsValidFolder(SourceRoot), Is.True, "accepted H1 SourceSlice must be mounted");
        }

        [TearDown]
        public void TearDown() => DeleteGenerated();

        [Test]
        public void MultiComponentRoundTrip_SaveReloadObserveAndRebuild_IsDeterministic()
        {
            var fixture = BuildFixture("roundtrip");
            var first = Execute("materialize", fixture.plan);
            Assert.That(first.errorCode, Is.Empty);
            Assert.That(first.observation.active, Is.True);
            Assert.That(first.observation.inputDigest, Is.EqualTo(fixture.plan.inputDigest));
            Assert.That(first.observation.nodes.Length, Is.EqualTo(2));

            var actor = first.observation.nodes.Single(node => node.objectId == "actor.potes");
            Assert.That(actor.componentRows, Is.EqualTo(fixture.expectedActorRows));
            Assert.That(actor.componentDigest, Has.Length.EqualTo(64));
            var firstGeneration = first.observation.generationId;
            var firstRealization = first.observation.realizationDigest;

            var observed = Execute("observe", fixture.plan);
            Assert.That(observed.errorCode, Is.Empty);
            Assert.That(observed.observation.generationId, Is.EqualTo(firstGeneration));
            Assert.That(observed.observation.realizationDigest, Is.EqualTo(firstRealization));
            Assert.That(observed.observation.nodes.Single(node => node.objectId == "actor.potes").componentRows,
                Is.EqualTo(fixture.expectedActorRows));

            var rebuilt = Execute("materialize", fixture.plan);
            Assert.That(rebuilt.errorCode, Is.Empty);
            Assert.That(rebuilt.observation.generationId, Is.EqualTo(firstGeneration), "idempotent rebuild must reuse the active matching generation");
            Assert.That(rebuilt.observation.realizationDigest, Is.EqualTo(firstRealization));
            Assert.That(rebuilt.observation.nodes.Single(node => node.objectId == "actor.potes").componentRows,
                Is.EqualTo(fixture.expectedActorRows));
        }

        [Test]
        public void UnsupportedSchema_FailsBeforePublication_AndKeepsPreviousGenerationActive()
        {
            var fixture = BuildFixture("negative-baseline");
            var baseline = Execute("materialize", fixture.plan);
            Assert.That(baseline.errorCode, Is.Empty);
            var activeGeneration = baseline.observation.generationId;

            var bad = BuildFixture("negative-attempt");
            bad.plan.nodes[1].components = bad.plan.nodes[1].components.Concat(new[]
            {
                new ProjectionComponent { schemaId = "arkus.h1.component.unsupported@1", kind = "unsupported" }
            }).ToArray();
            bad.plan.inputDigest = HashText("negative-attempt-with-unsupported-schema");

            var rejected = Execute("materialize", bad.plan);
            Assert.That(rejected.errorCode, Is.EqualTo("projection.component-schema-unsupported"));
            Assert.That(rejected.observation.active, Is.True);
            Assert.That(rejected.observation.generationId, Is.EqualTo(activeGeneration));
            Assert.That(rejected.observation.inputDigest, Is.EqualTo(fixture.plan.inputDigest));
        }

        private static Fixture BuildFixture(string salt)
        {
            var wallPath = SourceRoot + "/Wall_Plaster_Window_Wide_Flat.fbx";
            var mesh = AssetDatabase.LoadAllAssetsAtPath(wallPath).OfType<Mesh>().FirstOrDefault();
            Assert.That(mesh, Is.Not.Null, "accepted facade FBX must expose a Mesh");
            var source = Identity(mesh, wallPath);

            var materialPath = SourceRoot + "/FacadeImportedMaterial.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            Assert.That(material, Is.Not.Null, "accepted facade material must import");
            var materialIdentity = Identity(material, materialPath);

            var animationPath = SourceRoot + "/UAL1.fbx";
            var clip = AssetDatabase.LoadAllAssetsAtPath(animationPath)
                .OfType<AnimationClip>()
                .Where(value => !value.name.StartsWith("__preview__", StringComparison.Ordinal))
                .OrderBy(value => value.name, StringComparer.Ordinal)
                .FirstOrDefault();
            Assert.That(clip, Is.Not.Null, "accepted UAL1 source must expose a non-preview clip");
            var clipIdentity = Identity(clip, animationPath);

            var transform = new ProjectionComponent { schemaId = TransformSchema, kind = "transform" };
            var renderer = new ProjectionComponent
            {
                schemaId = RendererSchema,
                kind = "renderer",
                referenceKind = "material",
                referenceLogicalId = "quaternius.medieval.material.facade-imported",
                referencePath = materialIdentity.path,
                referenceGuid = materialIdentity.guid,
                referenceLocalFileId = materialIdentity.fileId,
                referenceContentSha256 = materialIdentity.sha256
            };
            var animator = new ProjectionComponent
            {
                schemaId = AnimatorSchema,
                kind = "animator",
                referenceKind = "animation-clip",
                referenceLogicalId = "quaternius.ual1.animation-clip.probe",
                referencePath = clipIdentity.path,
                referenceGuid = clipIdentity.guid,
                referenceLocalFileId = clipIdentity.fileId,
                referenceContentSha256 = clipIdentity.sha256
            };
            var link = new ProjectionComponent
            {
                schemaId = LinkSchema,
                kind = "canonical-link",
                relation = "faces",
                targetObjectId = "target.potes"
            };

            var target = new ProjectionNode
            {
                objectId = "target.potes",
                parentObjectId = "",
                sourceKind = "asset",
                sourceLogicalId = "quaternius.medieval.asset.wall-plaster-window-wide-flat",
                sourcePath = source.path,
                sourceGuid = source.guid,
                sourceLocalFileId = source.fileId,
                sourceContentSha256 = source.sha256,
                positionMm = Vec(0, 0, 0),
                rotationMilliDegrees = Vec(0, 0, 0),
                scalePpm = Vec(1000000, 1000000, 1000000),
                components = new[] { transform }
            };
            var actor = new ProjectionNode
            {
                objectId = "actor.potes",
                parentObjectId = "",
                sourceKind = "asset",
                sourceLogicalId = "quaternius.medieval.asset.wall-plaster-window-wide-flat",
                sourcePath = source.path,
                sourceGuid = source.guid,
                sourceLocalFileId = source.fileId,
                sourceContentSha256 = source.sha256,
                positionMm = Vec(1250, 0, -500),
                rotationMilliDegrees = Vec(0, 90000, 0),
                scalePpm = Vec(1250000, 1000000, 1000000),
                components = new[] { transform, renderer, animator, link }
                    .OrderBy(value => value.schemaId, StringComparer.Ordinal).ToArray()
            };
            var plan = new ProjectionPlan
            {
                schemaId = "arkus.h1-managed-scene-plan@1",
                sceneLogicalId = SceneId,
                worldId = "world.potes.h1-07-probe",
                worldRevision = 1,
                canonicalHash = HashText("canonical-" + salt),
                catalogueFingerprint = HashText("catalogue-" + salt),
                inputDigest = HashText("input-" + salt),
                nodes = new[] { actor, target }.OrderBy(value => value.objectId, StringComparer.Ordinal).ToArray()
            };

            var expected = new[]
            {
                AnimatorSchema + "|applyRootMotion=false|updateMode=Normal|cullingMode=AlwaysAnimate|clip=" + AssetRow(clipIdentity),
                LinkSchema + "|relation=faces|target=target.potes",
                RendererSchema + "|enabled=true|material=" + AssetRow(materialIdentity),
                TransformSchema + "|positionMm=1250,0,-500|rotationMilliDegrees=0,90000,0|scalePpm=1250000,1000000,1000000"
            }.OrderBy(value => value, StringComparer.Ordinal).ToArray();
            return new Fixture { plan = plan, expectedActorRows = expected };
        }

        private static ProjectionReply Execute(string mode, ProjectionPlan plan)
        {
            var payload = JsonUtility.ToJson(new ProjectionRequest
            {
                schemaId = "arkus.h1-projection-worker-request@1",
                mode = mode,
                sceneLogicalId = SceneId,
                plan = plan
            });
            var raw = H1SceneProjection.Execute(payload);
            var reply = JsonUtility.FromJson<ProjectionReply>(raw);
            Assert.That(reply, Is.Not.Null, raw);
            Assert.That(reply.schemaId, Is.EqualTo("arkus.h1-projection-worker-result@1"), raw);
            return reply;
        }

        private static AssetIdentity Identity(UnityEngine.Object asset, string path)
        {
            Assert.That(AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string guid, out long fileId), Is.True, path);
            var full = Path.GetFullPath(Path.Combine(Application.dataPath, "..", path));
            return new AssetIdentity
            {
                path = path,
                guid = guid,
                fileId = fileId.ToString(System.Globalization.CultureInfo.InvariantCulture),
                sha256 = HashBytes(File.ReadAllBytes(full))
            };
        }

        private static string AssetRow(AssetIdentity value) => value.path + "|" + value.guid + "|" + value.fileId;
        private static ProjectionVector Vec(long x, long y, long z) => new ProjectionVector { x = x, y = y, z = z };
        private static string HashText(string text) => HashBytes(System.Text.Encoding.UTF8.GetBytes(text));
        private static string HashBytes(byte[] bytes)
        {
            using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();
        }

        private static void DeleteGenerated()
        {
            if (AssetDatabase.IsValidFolder(ManagedScenes)) AssetDatabase.DeleteAsset(ManagedScenes);
            if (AssetDatabase.IsValidFolder(ManagedPrefabs)) AssetDatabase.DeleteAsset(ManagedPrefabs);
            AssetDatabase.Refresh();
        }

        private sealed class Fixture { public ProjectionPlan plan; public string[] expectedActorRows; }
        private sealed class AssetIdentity { public string path; public string guid; public string fileId; public string sha256; }

        [Serializable] private sealed class ProjectionRequest { public string schemaId; public string mode; public string sceneLogicalId; public ProjectionPlan plan; }
        [Serializable] private sealed class ProjectionPlan
        {
            public string schemaId; public string sceneLogicalId; public string worldId; public long worldRevision;
            public string canonicalHash; public string catalogueFingerprint; public string inputDigest; public ProjectionNode[] nodes;
        }
        [Serializable] private sealed class ProjectionNode
        {
            public string objectId; public string parentObjectId; public string sourceKind; public string sourceLogicalId;
            public string sourcePath; public string sourceGuid; public string sourceLocalFileId; public string sourceContentSha256;
            public ProjectionVector positionMm; public ProjectionVector rotationMilliDegrees; public ProjectionVector scalePpm;
            public ProjectionComponent[] components;
        }
        [Serializable] private sealed class ProjectionComponent
        {
            public string schemaId; public string kind; public string relation; public string targetObjectId; public string referenceKind;
            public string referenceLogicalId; public string referencePath; public string referenceGuid; public string referenceLocalFileId; public string referenceContentSha256;
        }
        [Serializable] private sealed class ProjectionVector { public long x; public long y; public long z; }
        [Serializable] private sealed class ProjectionReply
        {
            public string schemaId; public string sceneLogicalId; public string expectedInputDigest; public string errorCode; public ProjectionObservation observation;
        }
        [Serializable] private sealed class ProjectionObservation
        {
            public string schemaId; public string sceneLogicalId; public bool active; public string generationId; public string inputDigest;
            public string canonicalHash; public string catalogueFingerprint; public string graphDigest; public string realizationDigest; public ProjectionObservedNode[] nodes;
        }
        [Serializable] private sealed class ProjectionObservedNode
        {
            public string objectId; public string componentDigest; public string[] componentRows;
        }
    }
}
