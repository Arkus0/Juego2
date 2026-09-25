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
            var baseline = Fixture(1, includeWorkshop: true);
            var first = H1ManagedScenePlan.Build(baseline, catalogue);
            var repeat = H1ManagedScenePlan.Build(baseline, catalogue);
            Assert.Equal(first.InputDigest, repeat.InputDigest);
            Assert.Equal(4, first.Nodes.Length);
            Assert.Equal("plaza.potes", first.Nodes.Single(node => node.ObjectId == "market.potes").ParentObjectId);
            Assert.Equal("market.potes", first.Nodes.Single(node => node.ObjectId == "bar.potes").ParentObjectId);
            Assert.Equal("market.potes", first.Nodes.Single(node => node.ObjectId == "workshop.potes").ParentObjectId);
            var bar = first.Nodes.Single(node => node.ObjectId == "bar.potes");
            Assert.Equal(1250, bar.PositionMm.X);
            Assert.Equal(90000, bar.RotationMilliDegrees.Y);
            Assert.Equal(1250000, bar.ScalePpm.X);
            Assert.Equal("quaternius.medieval.prefab.wall-plaster-window-wide-flat", bar.SourceLogicalId);
            Assert.Equal(H1ComponentSchemas.Required, bar.Components.Select(component => component.SchemaId).OrderBy(value => value, StringComparer.Ordinal));
            Assert.Equal("plaza.potes", bar.Components.Single(component => component.SchemaId == H1ComponentSchemas.CanonicalLink).TargetObjectId);
            Assert.Equal("quaternius.medieval.material.wall-plaster-window-wide-flat-mi-plaster", bar.Components.Single(component => component.SchemaId == H1ComponentSchemas.MeshRenderer).ReferenceLogicalId);
            Assert.Equal("quaternius.ual1.animation-clip.armature-a-tpose", bar.Components.Single(component => component.SchemaId == H1ComponentSchemas.Animator).ReferenceLogicalId);

            var deleted = H1ManagedScenePlan.Build(Fixture(2, includeWorkshop: false), catalogue);
            Assert.Equal(3, deleted.Nodes.Length);
            Assert.NotEqual(first.InputDigest, deleted.InputDigest);
            Assert.Equal(CanonicalWorldStateCodec.ComputeContentHash(baseline), first.CanonicalHash);
        }

        [Fact]
        public void Catalogue_remap_changes_projection_evidence_without_changing_canonical_world_identity()
        {
            var root = H1UnityLaunchProfile.ForCurrentHost().RepositoryRoot;
            var effective = File.ReadAllText(Path.Combine(root, "Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json"));
            var mapping = File.ReadAllText(Path.Combine(root, H1CatalogueSnapshot.MappingRelativePath));
            var adoption = File.ReadAllText(Path.Combine(root, H1CatalogueSnapshot.AdoptionRelativePath));
            const string original = "quaternius.ual1.animation-clip.armature-a-tpose";
            const string remapped = "quaternius.ual1.animation-clip.armature-a-tpose-remapped";
            Assert.Equal(1, mapping.Split(original, StringSplitOptions.None).Length - 1);

            var acceptedCatalogue = H1CatalogueSnapshot.Build(effective, mapping, adoption);
            var remappedCatalogue = H1CatalogueSnapshot.Build(effective, mapping.Replace(original, remapped, StringComparison.Ordinal), adoption);
            var world = Fixture(7, includeWorkshop: true);
            var accepted = H1ManagedScenePlan.Build(world, acceptedCatalogue);
            Assert.Equal(CanonicalWorldStateCodec.ComputeContentHash(world), accepted.CanonicalHash);
            Assert.Equal(original, accepted.Nodes.Single(node => node.ObjectId == "bar.potes").Components.Single(component => component.SchemaId == H1ComponentSchemas.Animator).ReferenceLogicalId);
            Assert.Equal("projection.component-reference-missing", Assert.Throws<H1ProjectionException>(() => H1ManagedScenePlan.Build(world, remappedCatalogue)).Code);
        }

        [Fact]
        public void Missing_wrong_type_unbound_parent_and_component_references_fail_closed()
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

            var renderer = new WorldState(new WorldId("world.potes"), 0,
                new[] { new WorldObject(new WorldObjectId("plaza.potes"), new WorldTypeId("fixture.plaza")) },
                new[] { Binding("plaza.potes", 0, renderer: true) });
            var rendererPlan = H1ManagedScenePlan.Build(renderer, catalogue);
            Assert.Contains(rendererPlan.Nodes[0].Components, component => component.SchemaId == H1ComponentSchemas.MeshRenderer);

            var missingCanonicalTarget = new WorldState(new WorldId("world.potes"), 0,
                new[] { new WorldObject(new WorldObjectId("plaza.potes"), new WorldTypeId("fixture.plaza")) },
                new[] { Binding("plaza.potes", 0, link: "ghost.potes") });
            Assert.Equal("projection.canonical-target-missing", Assert.Throws<H1ProjectionException>(() => H1ManagedScenePlan.Build(missingCanonicalTarget, catalogue)).Code);
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
                Binding("bar.potes", 1250, "quaternius.medieval.prefab.wall-plaster-window-wide-flat", "plaza.potes", renderer: true, animator: true)
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
            if (renderer)
                components.Add(new Dictionary<string, object?> { ["kind"] = "renderer", ["materialId"] = "quaternius.medieval.material.wall-plaster-window-wide-flat-mi-plaster" });
            if (animator)
                components.Add(new Dictionary<string, object?> { ["kind"] = "animator", ["clipId"] = "quaternius.ual1.animation-clip.armature-a-tpose" });
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
            var compiledReferences = ((IReadOnlyList<object?>)compiled["dependencies"]!).Select(raw =>
            {
                var row = (IReadOnlyDictionary<string, object?>)raw!;
                return new WorldReference(new WorldReferenceKind((string)row["kind"]!), new WorldObjectId((string)row["targetId"]!));
            }).ToArray();
            return new WorldExtensionData(UnityBindingProducer.ExtensionOwner, UnityBindingProducer.ExtensionSchemaVersion,
                Convert.FromBase64String((string)compiled["payloadBase64"]!), new WorldObjectId(subject), compiledReferences);
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
