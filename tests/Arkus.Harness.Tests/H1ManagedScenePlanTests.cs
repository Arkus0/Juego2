using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkus.EngineBridge.UnityAuthoring;
using Arkus.Game.World;
using Arkus.H1.UnityHost;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1ManagedScenePlanTests
    {
        [Fact]
        public void Potes_hierarchy_uses_canonical_containment_normalized_transforms_and_allowlisted_components()
        {
            var catalogue = Catalogue();
            var baseline = Fixture(1, true);
            var first = H1ManagedScenePlan.Build(baseline, catalogue);
            var repeat = H1ManagedScenePlan.Build(baseline, catalogue);
            Assert.Equal(first.InputDigest, repeat.InputDigest);
            Assert.Equal(4, first.Nodes.Length);
            var bar = first.Nodes.Single(node => node.ObjectId == "bar.potes");
            Assert.Equal("market.potes", bar.ParentObjectId);
            Assert.Equal(1250, bar.PositionMm.X);
            Assert.Equal(90000, bar.RotationMilliDegrees.Y);
            Assert.Equal(1250000, bar.ScalePpm.X);
            Assert.Equal(H1ComponentSchemas.Required, bar.Components.Select(component => component.SchemaId).OrderBy(value => value, StringComparer.Ordinal));
            Assert.Equal("plaza.potes", bar.Components.Single(component => component.SchemaId == H1ComponentSchemas.CanonicalLink).TargetObjectId);
            Assert.Equal("quaternius.medieval.material.wall-plaster-window-wide-flat-mi-plaster", bar.Components.Single(component => component.SchemaId == H1ComponentSchemas.MeshRenderer).ReferenceLogicalId);
            Assert.Equal("quaternius.ual1.animation-clip.armature-a-tpose", bar.Components.Single(component => component.SchemaId == H1ComponentSchemas.Animator).ReferenceLogicalId);
            Assert.Equal(CanonicalWorldStateCodec.ComputeContentHash(baseline), first.CanonicalHash);

            var deleted = H1ManagedScenePlan.Build(Fixture(2, false), catalogue);
            Assert.Equal(3, deleted.Nodes.Length);
            Assert.NotEqual(first.InputDigest, deleted.InputDigest);
        }

        [Fact]
        public void Component_reference_remap_fails_closed_without_rewriting_canonical_identity()
        {
            var root = H1UnityLaunchProfile.ForCurrentHost().RepositoryRoot;
            var effective = File.ReadAllText(Path.Combine(root, "Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json"));
            var mapping = File.ReadAllText(Path.Combine(root, H1CatalogueSnapshot.MappingRelativePath));
            var adoption = File.ReadAllText(Path.Combine(root, H1CatalogueSnapshot.AdoptionRelativePath));
            const string original = "quaternius.ual1.animation-clip.armature-a-tpose";
            const string remapped = "quaternius.ual1.animation-clip.armature-a-tpose-remapped";
            Assert.Equal(1, mapping.Split(original, StringSplitOptions.None).Length - 1);
            var world = Fixture(7, true);
            var accepted = H1ManagedScenePlan.Build(world, H1CatalogueSnapshot.Build(effective, mapping, adoption));
            Assert.Equal(CanonicalWorldStateCodec.ComputeContentHash(world), accepted.CanonicalHash);
            var rebound = H1CatalogueSnapshot.Build(effective, mapping.Replace(original, remapped, StringComparison.Ordinal), adoption);
            Assert.Equal("projection.reference-missing", Assert.Throws<H1ProjectionException>(() => H1ManagedScenePlan.Build(world, rebound)).Code);
        }

        [Fact]
        public void Source_parent_and_canonical_component_failures_are_stable()
        {
            var catalogue = Catalogue();
            var unbound = new WorldState(new WorldId("world.potes"), 0,
                new[] { new WorldObject(new WorldObjectId("plaza.potes"), new WorldTypeId("fixture.plaza")), new WorldObject(new WorldObjectId("market.potes"), new WorldTypeId("fixture.market"), new WorldObjectId("plaza.potes")) },
                new[] { Binding("market.potes", 0) });
            Assert.Equal("projection.parent-unbound", Assert.Throws<H1ProjectionException>(() => H1ManagedScenePlan.Build(unbound, catalogue)).Code);

            var missing = new WorldState(new WorldId("world.potes"), 0,
                new[] { new WorldObject(new WorldObjectId("plaza.potes"), new WorldTypeId("fixture.plaza")) }, new[] { Binding("plaza.potes", 0, "prefab.absent") });
            Assert.Equal("projection.source-missing", Assert.Throws<H1ProjectionException>(() => H1ManagedScenePlan.Build(missing, catalogue)).Code);

            var wrongType = new WorldState(new WorldId("world.potes"), 0,
                new[] { new WorldObject(new WorldObjectId("plaza.potes"), new WorldTypeId("fixture.plaza")) }, new[] { Binding("plaza.potes", 0, "quaternius.medieval.asset.wall-plaster-window-wide-flat") });
            Assert.Equal("projection.source-wrong-type", Assert.Throws<H1ProjectionException>(() => H1ManagedScenePlan.Build(wrongType, catalogue)).Code);

            var renderer = new WorldState(new WorldId("world.potes"), 0,
                new[] { new WorldObject(new WorldObjectId("plaza.potes"), new WorldTypeId("fixture.plaza")) }, new[] { Binding("plaza.potes", 0, renderer: true) });
            Assert.Contains(H1ManagedScenePlan.Build(renderer, catalogue).Nodes[0].Components, component => component.SchemaId == H1ComponentSchemas.MeshRenderer);

            var targetNotProjected = new WorldState(new WorldId("world.potes"), 0,
                new[]
                {
                    new WorldObject(new WorldObjectId("plaza.potes"), new WorldTypeId("fixture.plaza")),
                    new WorldObject(new WorldObjectId("ghost.potes"), new WorldTypeId("fixture.target"))
                },
                new[] { Binding("plaza.potes", 0, link: "ghost.potes") });
            Assert.Equal("projection.canonical-target-unbound", Assert.Throws<H1ProjectionException>(() => H1ManagedScenePlan.Build(targetNotProjected, catalogue)).Code);
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
                Binding("bar.potes", 1250, "quaternius.medieval.prefab.wall-plaster-window-wide-flat", "plaza.potes", true, true)
            };
            if (includeWorkshop)
            {
                objects.Add(new WorldObject(new WorldObjectId("workshop.potes"), new WorldTypeId("fixture.workshop"), new WorldObjectId("market.potes")));
                extensions.Add(Binding("workshop.potes", -800));
            }
            return new WorldState(new WorldId("world.potes"), revision, objects, extensions);
        }

        private static WorldExtensionData Binding(string subject, long x, string sourceId = "quaternius.medieval.prefab.wall-plaster-window-wide-flat", string? link = null, bool renderer = false, bool animator = false)
        {
            var components = new List<object?>();
            var references = new List<WorldReference>();
            if (link != null)
            {
                components.Add(new Dictionary<string, object?> { ["kind"] = "canonical-link", ["relation"] = "faces", ["targetObjectId"] = link });
                references.Add(new WorldReference(new WorldReferenceKind("faces"), new WorldObjectId(link)));
            }
            if (renderer) components.Add(new Dictionary<string, object?> { ["kind"] = "renderer", ["materialId"] = "quaternius.medieval.material.wall-plaster-window-wide-flat-mi-plaster" });
            if (animator) components.Add(new Dictionary<string, object?> { ["kind"] = "animator", ["clipId"] = "quaternius.ual1.animation-clip.armature-a-tpose" });
            var binding = new Dictionary<string, object?>
            {
                ["schemaId"] = UnityBindingProducer.BindingSchemaId,
                ["targetSceneId"] = H1ManagedScenePlan.SceneId,
                ["source"] = new Dictionary<string, object?> { ["kind"] = "prefab", ["logicalId"] = sourceId },
                ["transform"] = new Dictionary<string, object?>
                {
                    ["coordinateConvention"] = UnityBindingProducer.CoordinateConvention,
                    ["positionMm"] = Vector(x, 0, 0), ["rotationMilliDegrees"] = Vector(0, link == null ? 0 : 90000, 0),
                    ["scalePpm"] = Vector(link == null ? 1000000 : 1250000, 1000000, 1000000)
                },
                ["components"] = components
            };
            var compiled = UnityBindingProducer.Compile(new Dictionary<string, object?> { ["subjectId"] = subject, ["binding"] = binding });
            return new WorldExtensionData(UnityBindingProducer.ExtensionOwner, UnityBindingProducer.ExtensionSchemaVersion,
                Convert.FromBase64String((string)compiled["payloadBase64"]!), new WorldObjectId(subject), references);
        }

        private static Dictionary<string, object?> Vector(long x, long y, long z) => new Dictionary<string, object?> { ["x"] = x, ["y"] = y, ["z"] = z };

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
