using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using Arkus.EngineBridge.UnityAuthoring;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.H1.UnityHost;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    /// <summary>
    /// WP-H1-11 representative street-corner/humanoid scenario authored only through accepted public surfaces:
    /// canonical <see cref="WorldState"/> objects, Unity binding extensions compiled by <see cref="UnityBindingProducer"/>
    /// and one ordinary H0 mutation. It is deliberately a generic street corner in the fixed managed test scene,
    /// not CITY geometry, a seed or gameplay.
    /// </summary>
    internal static class H1RepresentativeSliceScenario
    {
        public const string ManifestRelativePath = "Docs/evidence/WP-H1-11/REPRESENTATIVE_SLICE.json";
        public const string SharedOverrideMaterial = "quaternius.medieval.material.wall-plaster-window-wide-flat-mi-plaster";
        public const string CratePrefab = "quaternius.medieval.prefab.prop-crate";
        public const string CrateMesh = "quaternius.medieval.asset.prop-crate";

        private const string Humanoid = "quaternius.ual1.prefab.ual1";
        private const string IdleClip = "quaternius.ual1.animation-clip.armature-idle-loop";
        private const string WalkClip = "quaternius.ual1.animation-clip.armature-walk-loop";
        private const string SitClip = "quaternius.ual1.animation-clip.armature-sitting-idle-loop";

        /// <summary>Canonical scene graph: object, parent, source kind, source ID, position (mm), yaw (m°), scale (ppm) and components.</summary>
        public static IReadOnlyList<NodeSpec> Nodes { get; } = new[]
        {
            new NodeSpec("corner.street", null, "prefab", "quaternius.medieval.prefab.corner-exterior-wood", Vec(0, 0, 0)),
            // The accepted H1-04 facade sidecar imports FBX file units without conversion (REPRESENTATIVE_SLICE.json
            // import.unitConversion=false); its Unity binding states the 1% unit scale so it composes with the kit modules.
            new NodeSpec("facade.front", "corner.street", "prefab", "quaternius.medieval.prefab.wall-plaster-window-wide-flat", Vec(1000, 0, 0),
                scale: Vec(10000, 10000, 10000)),
            new NodeSpec("window.front", "corner.street", "prefab", "quaternius.medieval.prefab.window-wide-flat1", Vec(1000, 0, 0)),
            new NodeSpec("shutters.front", "window.front", "prefab", "quaternius.medieval.prefab.windowshutters-wide-flat-open", Vec(0, 0, 0)),
            new NodeSpec("vine.front", "corner.street", "prefab", "quaternius.medieval.prefab.prop-vine1", Vec(1600, 3000, 150)),
            new NodeSpec("wall.side", "corner.street", "prefab", "quaternius.medieval.prefab.wall-plaster-straight", Vec(0, 0, -1000), yaw: 90000),
            new NodeSpec("wall.door", "corner.street", "prefab", "quaternius.medieval.prefab.wall-plaster-door-flat", Vec(3000, 0, 0)),
            new NodeSpec("door.main", "wall.door", "prefab", "quaternius.medieval.prefab.door-1-flat", Vec(535, 0, 0)),
            new NodeSpec("roof.house", "corner.street", "prefab", "quaternius.medieval.prefab.roof-roundtiles-4x4", Vec(2000, 3120, -1500)),
            new NodeSpec("chimney.house", "roof.house", "prefab", "quaternius.medieval.prefab.prop-chimney", Vec(1200, 1500, 0)),
            new NodeSpec("fence.yard", "corner.street", "asset", "quaternius.medieval.asset.prop-woodenfence-single", Vec(5000, 0, -500),
                scale: Vec(1250000, 1000000, 1000000), materialId: SharedOverrideMaterial),
            new NodeSpec("crate.stack", "corner.street", "asset", CrateMesh, Vec(4200, 0, 800), materialId: SharedOverrideMaterial),
            new NodeSpec("civilian.sit", "crate.stack", "prefab", Humanoid, Vec(0, 1060, 0), yaw: 180000, clipId: SitClip),
            new NodeSpec("wagon.street", "corner.street", "prefab", "quaternius.medieval.prefab.prop-wagon", Vec(6500, 0, 2500), yaw: 30000),
            new NodeSpec("civilian.idle", "corner.street", "prefab", Humanoid, Vec(1500, 0, 1500), clipId: IdleClip, link: ("faces", "door.main")),
            new NodeSpec("civilian.walk", "corner.street", "prefab", Humanoid, Vec(3000, 0, 2500), yaw: 270000, clipId: WalkClip, link: ("walks-toward", "wagon.street"))
        };

        public static WorldState InitialState() => InitialState(Nodes);

        public static WorldState InitialState(IEnumerable<NodeSpec> nodes)
        {
            var specs = nodes.ToArray();
            return new WorldState(
                new WorldId("world.h1-11.street-corner"),
                3,
                specs.Select(node => node.Parent == null
                    ? new WorldObject(new WorldObjectId(node.ObjectId), new WorldTypeId("fixture." + node.ObjectId.Split('.')[0]))
                    : new WorldObject(new WorldObjectId(node.ObjectId), new WorldTypeId("fixture." + node.ObjectId.Split('.')[0]), new WorldObjectId(node.Parent))).ToArray(),
                specs.Select(Binding).ToArray());
        }

        /// <summary>The canonical session carries one ordinary accepted H0 mutation so the checkpoint journal is non-empty.</summary>
        public static PortableWorldAuthoringSession AuthoredSession()
        {
            var initial = InitialState();
            var session = new PortableWorldAuthoringSession(initial);
            var contract = CanonicalWorldContract.Compose(new WorldInspectionService(session), session);
            var applied = contract.Dispatch(
                WorldMutationContract.ApplyName,
                Exact(),
                ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["idempotencyKey"] = "h1-11.street-corner.observer",
                    ["expectedRevision"] = initial.Revision,
                    ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(initial),
                    ["operations"] = new object?[]
                    {
                        new Dictionary<string, object?>(StringComparer.Ordinal)
                        {
                            ["kind"] = "put-object",
                            ["id"] = "observer",
                            ["typeId"] = "fixture.observer"
                        }
                    }
                }));
            Assert.True(applied.Success, applied.Error?.MachineCode);
            return session;
        }

        public static WorldExtensionData Binding(NodeSpec node)
        {
            var components = new List<object?>();
            var references = new List<WorldReference>();
            if (node.Link.HasValue)
            {
                components.Add(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "canonical-link", ["relation"] = node.Link.Value.Relation, ["targetObjectId"] = node.Link.Value.Target
                });
                references.Add(new WorldReference(new WorldReferenceKind(node.Link.Value.Relation), new WorldObjectId(node.Link.Value.Target)));
            }
            if (node.MaterialId != null)
                components.Add(new Dictionary<string, object?>(StringComparer.Ordinal) { ["kind"] = "renderer", ["materialId"] = node.MaterialId });
            if (node.ClipId != null)
                components.Add(new Dictionary<string, object?>(StringComparer.Ordinal) { ["kind"] = "animator", ["clipId"] = node.ClipId });
            var compiled = UnityBindingProducer.Compile(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["subjectId"] = node.ObjectId,
                ["binding"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["schemaId"] = UnityBindingProducer.BindingSchemaId,
                    ["targetSceneId"] = H1ManagedScenePlan.SceneId,
                    ["source"] = new Dictionary<string, object?>(StringComparer.Ordinal) { ["kind"] = node.SourceKind, ["logicalId"] = node.SourceId },
                    ["transform"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["coordinateConvention"] = UnityBindingProducer.CoordinateConvention,
                        ["positionMm"] = Map(node.PositionMm),
                        ["rotationMilliDegrees"] = Map(Vec(0, node.Yaw, 0)),
                        ["scalePpm"] = Map(node.ScalePpm)
                    },
                    ["components"] = components
                }
            });
            return new WorldExtensionData(
                UnityBindingProducer.ExtensionOwner,
                UnityBindingProducer.ExtensionSchemaVersion,
                Convert.FromBase64String((string)compiled["payloadBase64"]!),
                new WorldObjectId(node.ObjectId),
                references);
        }

        public static H1CatalogueSnapshot CommittedCatalogue(string repositoryRoot) => H1CatalogueSnapshot.Build(
            File.ReadAllText(Path.Combine(repositoryRoot, "Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json")),
            File.ReadAllText(Path.Combine(repositoryRoot, H1CatalogueSnapshot.MappingRelativePath)),
            File.ReadAllText(Path.Combine(repositoryRoot, H1CatalogueSnapshot.AdoptionRelativePath)));

        public static Manifest ReadManifest(string repositoryRoot)
        {
            var json = File.ReadAllText(Path.Combine(repositoryRoot, ManifestRelativePath));
            var manifest = JsonSerializer.Deserialize<Manifest>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
                ?? throw new InvalidDataException("representative manifest is not JSON");
            Assert.Equal("arkus.h1-11-representative-slice@1", manifest.SchemaId);
            return manifest;
        }

        /// <summary>
        /// Completeness of the proof universe: the universe is enumerated from the representative manifest (which is itself
        /// bijective with the admitted adoption slices), never from the plan under test. Returns every uncovered obligation.
        /// </summary>
        public static IReadOnlyList<string> UncoveredObligations(Manifest manifest, H1ManagedScenePlan plan)
        {
            var sources = new HashSet<string>(plan.Nodes.Select(node => node.SourceLogicalId), StringComparer.Ordinal);
            var clips = new HashSet<string>(plan.Nodes.SelectMany(node => node.Components)
                .Where(component => component.Kind == "animator").Select(component => component.ReferenceLogicalId), StringComparer.Ordinal);
            var missing = new List<string>();
            foreach (var item in manifest.Items)
                if (!item.Catalogue.Values.Any(sources.Contains)) missing.Add("item:" + item.AssetPath);
            foreach (var clip in manifest.Clips)
                if (!clips.Contains(clip.LogicalId)) missing.Add("clip:" + clip.Role);
            var materials = plan.Nodes.SelectMany(node => node.Components).Where(component => component.Kind == "renderer")
                .GroupBy(component => component.ReferenceLogicalId, StringComparer.Ordinal);
            if (!materials.Any(group => group.Count() >= 2)) missing.Add("shared-material-override");
            if (MaximumDepth(plan) < 3) missing.Add("nested-canonical-hierarchy");
            return missing;
        }

        public static int MaximumDepth(H1ManagedScenePlan plan)
        {
            var parents = plan.Nodes.ToDictionary(node => node.ObjectId, node => node.ParentObjectId, StringComparer.Ordinal);
            var deepest = 0;
            foreach (var node in plan.Nodes)
            {
                var depth = 1;
                for (var parent = node.ParentObjectId; parent.Length != 0; parent = parents[parent]) depth++;
                deepest = Math.Max(deepest, depth);
            }
            return deepest;
        }

        public static ContractVersionRange Exact() => ContractVersionRange.Exact(new ContractVersion(1, 0));
        public static IReadOnlyDictionary<string, object?> ReadOnly(Dictionary<string, object?> value) => new ReadOnlyDictionary<string, object?>(value);

        private static long[] Vec(long x, long y, long z) => new[] { x, y, z };
        private static Dictionary<string, object?> Map(long[] value) => new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["x"] = value[0], ["y"] = value[1], ["z"] = value[2]
        };

        public sealed class NodeSpec
        {
            public NodeSpec(string objectId, string? parent, string sourceKind, string sourceId, long[] positionMm,
                long yaw = 0, long[]? scale = null, string? materialId = null, string? clipId = null, (string Relation, string Target)? link = null)
            {
                ObjectId = objectId; Parent = parent; SourceKind = sourceKind; SourceId = sourceId; PositionMm = positionMm;
                Yaw = yaw; ScalePpm = scale ?? Vec(1000000, 1000000, 1000000); MaterialId = materialId; ClipId = clipId; Link = link;
            }

            public string ObjectId { get; }
            public string? Parent { get; }
            public string SourceKind { get; }
            public string SourceId { get; }
            public long[] PositionMm { get; }
            public long Yaw { get; }
            public long[] ScalePpm { get; }
            public string? MaterialId { get; }
            public string? ClipId { get; }
            public (string Relation, string Target)? Link { get; }

            public NodeSpec WithSource(string sourceKind, string sourceId) =>
                new NodeSpec(ObjectId, Parent, sourceKind, sourceId, PositionMm, Yaw, ScalePpm, MaterialId, ClipId, Link);
        }

        public sealed class Manifest
        {
            public string SchemaId { get; set; } = "";
            public List<ManifestItem> Items { get; set; } = new List<ManifestItem>();
            public List<ManifestClip> Clips { get; set; } = new List<ManifestClip>();
        }

        public sealed class ManifestItem
        {
            public string Provenance { get; set; } = "";
            public string SourceId { get; set; } = "";
            public string AssetPath { get; set; } = "";
            public string ContentSha256 { get; set; } = "";
            public Dictionary<string, string> Catalogue { get; set; } = new Dictionary<string, string>(StringComparer.Ordinal);
        }

        public sealed class ManifestClip
        {
            public string Role { get; set; } = "";
            public string LogicalId { get; set; } = "";
        }
    }
}
