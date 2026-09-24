using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkus.EngineBridge.UnityAuthoring;
using Arkus.Game.World;
using Arkus.H1.UnityHost;
using Arkus.Harness.Projection;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1ManagedScenePlanTests
    {
        [Fact]
        public void Potes_hierarchy_uses_canonical_containment_and_normalized_binding_transforms()
        {
            var catalogue = Catalogue();
            var baseline = Fixture(1, includeWorkshop: true);
            var first = H1ManagedScenePlan.Build(baseline, catalogue);
            var repeat = H1ManagedScenePlan.Build(baseline, catalogue);
            Assert.Equal(first.InputDigest, repeat.InputDigest);
            Assert.Equal(4, first.Nodes.Length);
            Assert.Equal("plaza.potes", first.Nodes.Single(node => node.ObjectId == "market.potes").ParentObjectId);
            Assert.Equal("market.potes", first.Nodes.Single(node => node.ObjectId == "bar.potes").ParentObjectId);
            Assert.Equal("market.potes", first.Nodes.Single(node => node.ObjectId == "workshop.potes").ParentObjectId);
            Assert.Equal(1250, first.Nodes.Single(node => node.ObjectId == "bar.potes").PositionMm.X);
            Assert.Equal(90000, first.Nodes.Single(node => node.ObjectId == "bar.potes").RotationMilliDegrees.Y);
            Assert.Equal(1250000, first.Nodes.Single(node => node.ObjectId == "bar.potes").ScalePpm.X);
            Assert.Equal("quaternius.medieval.prefab.wall-plaster-window-wide-flat", first.Nodes.Single(node => node.ObjectId == "bar.potes").SourceLogicalId);

            var deleted = H1ManagedScenePlan.Build(Fixture(2, includeWorkshop: false), catalogue);
            Assert.Equal(3, deleted.Nodes.Length);
            Assert.NotEqual(first.InputDigest, deleted.InputDigest);
            Assert.Equal(CanonicalWorldStateCodec.ComputeContentHash(baseline), first.CanonicalHash);
        }

        [Fact]
        public void Missing_wrong_type_and_unbound_parent_fail_with_stable_projection_diagnostics()
        {
            var catalogue = Catalogue();
            var unbound = new WorldState(new WorldId("world.potes"), 0,
                new[]
                {
                    new WorldObject(new WorldObjectId("plaza.potes"), new WorldTypeId("fixture.plaza")),
                    new WorldObject(new WorldObjectId("market.potes"), new WorldTypeId("fixture.market"), new WorldObjectId("plaza.potes"))
                }, new[] { Binding("market.potes", 0) });
            Assert.Equal("projection.parent-unbound", Assert.Throws<H1ProjectionException>(() => H1ManagedScenePlan.Build(unbound, catalogue)).Code);

            var missing = new WorldState(new WorldId("world.potes"), 0,
                new[] { new WorldObject(new WorldObjectId("plaza.potes"), new WorldTypeId("fixture.plaza")) },
                new[] { Binding("plaza.potes", 0, "prefab.absent") });
            Assert.Equal("projection.source-missing", Assert.Throws<H1ProjectionException>(() => H1ManagedScenePlan.Build(missing, catalogue)).Code);

            var wrongType = new WorldState(new WorldId("world.potes"), 0,
                new[] { new WorldObject(new WorldObjectId("plaza.potes"), new WorldTypeId("fixture.plaza")) },
                new[] { Binding("plaza.potes", 0, "quaternius.medieval.asset.wall-plaster-window-wide-flat") });
            Assert.Equal("projection.source-wrong-type", Assert.Throws<H1ProjectionException>(() => H1ManagedScenePlan.Build(wrongType, catalogue)).Code);

            var unsupported = new WorldState(new WorldId("world.potes"), 0,
                new[] { new WorldObject(new WorldObjectId("plaza.potes"), new WorldTypeId("fixture.plaza")) },
                new[] { Binding("plaza.potes", 0, renderer: true) });
            Assert.Equal("projection.component-not-owned", Assert.Throws<H1ProjectionException>(() => H1ManagedScenePlan.Build(unsupported, catalogue)).Code);
        }

        private static WorldState Fixture(long revision, bool includeWorkshop)
        {
            var objects = new List<WorldObject>
            {
                new WorldObject(new WorldObjectId("plaza.potes"), new WorldTypeId("fixture.plaza")),
                new WorldObject(new WorldObjectId("market.potes"), new WorldTypeId("fixture.market"), new WorldObjectId("plaza.potes")),
                new WorldObject(new WorldObjectId("bar.potes"), new WorldTypeId("fixture.bar"), new WorldObjectId("market.potes"))
            };
            var extensions = new List<WorldExtensionData>
            {
                Binding("plaza.potes", 0), Binding("market.potes", 500),
                Binding("bar.potes", 1250, "quaternius.medieval.prefab.wall-plaster-window-wide-flat", "plaza.potes")
            };
            if (includeWorkshop)
            {
                objects.Add(new WorldObject(new WorldObjectId("workshop.potes"), new WorldTypeId("fixture.workshop"), new WorldObjectId("market.potes")));
                extensions.Add(Binding("workshop.potes", -800));
            }
            return new WorldState(new WorldId("world.potes"), revision, objects, extensions);
        }

        private static WorldExtensionData Binding(string subject, long x, string sourceId = "quaternius.medieval.prefab.wall-plaster-window-wide-flat", string? link = null, bool renderer = false)
        {
            var components = new List<object?>();
            var references = new List<WorldReference>();
            if (link != null)
            {
                components.Add(new Dictionary<string, object?>
                {
                    ["kind"] = "canonical-link", ["relation"] = "faces", ["targetObjectId"] = link
                });
                references.Add(new WorldReference(new WorldReferenceKind("faces"), new WorldObjectId(link)));
            }
            if (renderer)
                components.Add(new Dictionary<string, object?>
                {
                    ["kind"] = "renderer", ["materialId"] = "quaternius.medieval.material.wall-plaster-window-wide-flat-mi-plaster"
                });
            var binding = new Dictionary<string, object?>
            {
                ["schemaId"] = UnityBindingProducer.BindingSchemaId,
                ["targetSceneId"] = H1ManagedScenePlan.SceneId,
                ["source"] = new Dictionary<string, object?> { ["kind"] = "prefab", ["logicalId"] = sourceId },
                ["transform"] = new Dictionary<string, object?>
                {
                    ["coordinateConvention"] = UnityBindingProducer.CoordinateConvention,
                    ["positionMm"] = Vector(x, 0, 0),
                    ["rotationMilliDegrees"] = Vector(0, link == null ? 0 : 90000, 0),
                    ["scalePpm"] = Vector(link == null ? 1000000 : 1250000, 1000000, 1000000)
                },
                ["components"] = components
            };
            var compiled = UnityBindingProducer.Compile(new Dictionary<string, object?>
            {
                ["subjectId"] = subject, ["binding"] = binding
            });
            return new WorldExtensionData(UnityBindingProducer.ExtensionOwner, UnityBindingProducer.ExtensionSchemaVersion,
                Convert.FromBase64String((string)compiled["payloadBase64"]!), new WorldObjectId(subject), references);
        }

        private static Dictionary<string, object?> Vector(long x, long y, long z) => new Dictionary<string, object?>
        {
            ["x"] = x, ["y"] = y, ["z"] = z
        };

        private static H1CatalogueSnapshot Catalogue()
        {
            var root = H1UnityLaunchProfile.ForCurrentHost().RepositoryRoot;
            return H1CatalogueSnapshot.Build(
                File.ReadAllText(Path.Combine(root, "Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json")),
                File.ReadAllText(Path.Combine(root, H1CatalogueSnapshot.MappingRelativePath)),
                File.ReadAllText(Path.Combine(root, H1CatalogueSnapshot.AdoptionRelativePath)));
        }
    }
}
