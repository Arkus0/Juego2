using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Arkus.EngineBridge.UnityAuthoring;
using Arkus.Game.World;
using Arkus.H1.UnityHost;
using Arkus.Harness.Protocol;
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

        [Fact]
        public void Binding_failures_name_the_canonical_subject_and_the_typed_document_repair()
        {
            // WP-H1-05 reopen 1 (WP-H1-GATE trial 7): a payload that is valid Base64 but not a Unity binding, which is what
            // a mis-transcribed payloadBase64 looks like after the kernel accepted it. The public refusal must name the
            // failing subject and point to the typed document form instead of transcription.
            var catalogue = Catalogue();
            var garbled = new WorldState(new WorldId("world.potes"), 0,
                new[] { new WorldObject(new WorldObjectId("plaza.potes"), new WorldTypeId("fixture.plaza")) },
                new[]
                {
                    new WorldExtensionData(UnityBindingProducer.ExtensionOwner, UnityBindingProducer.ExtensionSchemaVersion,
                        Encoding.UTF8.GetBytes("not-a-unity-binding"), new WorldObjectId("plaza.potes"), new List<WorldReference>())
                });
            var invalid = Assert.Throws<H1ProjectionException>(() => H1ManagedScenePlan.Build(garbled, catalogue));
            Assert.Equal("projection.binding-invalid", invalid.Code);
            Assert.Equal("unity.binding.invalid-payload at $.payloadBase64", invalid.Message);
            Assert.Equal("plaza.potes", invalid.Context["subjectId"]);
            Assert.Equal("unity.binding.invalid-payload", invalid.Context["bindingCode"]);
            Assert.Equal("$.payloadBase64", invalid.Context["bindingPath"]);
            Assert.Contains("documentMutation", invalid.RepairHint, StringComparison.Ordinal);

            var missing = new WorldState(new WorldId("world.potes"), 0,
                new[] { new WorldObject(new WorldObjectId("plaza.potes"), new WorldTypeId("fixture.plaza")) }, new[] { Binding("plaza.potes", 0, "prefab.absent") });
            var source = Assert.Throws<H1ProjectionException>(() => H1ManagedScenePlan.Build(missing, catalogue));
            Assert.Equal("projection.source-missing", source.Code);
            Assert.Equal("plaza.potes", source.Context["subjectId"]);
            Assert.Equal("catalogue.missing-reference", source.Context["catalogueCode"]);
            Assert.Equal("prefab.absent", source.Context["logicalId"]);

            var unbound = new WorldState(new WorldId("world.potes"), 0,
                new[] { new WorldObject(new WorldObjectId("plaza.potes"), new WorldTypeId("fixture.plaza")), new WorldObject(new WorldObjectId("market.potes"), new WorldTypeId("fixture.market"), new WorldObjectId("plaza.potes")) },
                new[] { Binding("market.potes", 0) });
            var parent = Assert.Throws<H1ProjectionException>(() => H1ManagedScenePlan.Build(unbound, catalogue));
            Assert.Equal("projection.parent-unbound", parent.Code);
            Assert.Equal("market.potes", parent.Context["subjectId"]);
            Assert.Equal("plaza.potes", parent.Context["parentObjectId"]);

            // The public preflight error keeps the code and message, carries the facts and the code-specific hint, and is
            // a valid portable StructuredError; a failure without a specific hint keeps the accepted default hint.
            foreach (var exception in new[] { invalid, source, parent })
            {
                var error = H1ProjectionContract.PreflightError(exception);
                Assert.Equal(exception.Code, error.MachineCode);
                Assert.Equal(exception.Message, error.Message);
                Assert.Equal(exception.Context["subjectId"], error.Context["subjectId"]);
                Assert.Empty(PortableData.Validate(error.ToData()));
                Assert.Empty(CanonicalContractSchemas.StructuredError().ValidateValue(error.ToData()));
            }
            Assert.Equal(H1ManagedScenePlan.BindingInvalidRepairHint, H1ProjectionContract.PreflightError(invalid).RepairHint);
            Assert.Equal(H1ProjectionContract.DefaultPreflightRepairHint, H1ProjectionContract.PreflightError(source).RepairHint);
            Assert.Equal(H1ManagedScenePlan.ParentUnboundRepairHint, H1ProjectionContract.PreflightError(parent).RepairHint);
            Assert.Contains("containerId", H1ManagedScenePlan.ParentUnboundRepairHint, StringComparison.Ordinal);

            var targetNotProjected = new WorldState(new WorldId("world.potes"), 0,
                new[]
                {
                    new WorldObject(new WorldObjectId("plaza.potes"), new WorldTypeId("fixture.plaza")),
                    new WorldObject(new WorldObjectId("ghost.potes"), new WorldTypeId("fixture.target"))
                },
                new[] { Binding("plaza.potes", 0, link: "ghost.potes") });
            var target = Assert.Throws<H1ProjectionException>(() => H1ManagedScenePlan.Build(targetNotProjected, catalogue));
            Assert.Equal("projection.canonical-target-unbound", target.Code);
            Assert.Equal("plaza.potes", target.Context["subjectId"]);
            Assert.Equal("ghost.potes", target.Context["targetObjectId"]);
            Assert.Equal(H1ManagedScenePlan.CanonicalTargetUnboundRepairHint, H1ProjectionContract.PreflightError(target).RepairHint);
        }

        [Fact]
        public void Editor_refusals_carry_the_unity_diagnostic_subject_and_a_code_specific_repair()
        {
            // WP-H1-05 reopen 1 (WP-H1-GATE pre-merge probe): a renderer bound to a prefab that owns no MeshRenderer (a skinned
            // model) is refused by the Unity preflight. The public refusal must name the canonical subject and its source, not
            // only the scene, and say how to repair it. This is the Editor reply shape of H1SceneProjection.ValidationReply.
            var reply = Reply("projection.component-target-missing",
                "{\"code\":\"projection.component-target-missing\",\"severity\":\"error\",\"invariantId\":\"unity.plan.component\"," +
                "\"phase\":\"preflight\",\"canonicalResource\":\"slice-humanoid\",\"logicalAsset\":\"quaternius.ual1.prefab.ual1\"," +
                "\"managedPath\":\"Assets/Arkus/H1/ManagedScenes/slice-humanoid\",\"context\":\"repair component schema/field/reference\"}");
            var target = H1ManagedSceneExecutor.WorkerFailure(reply).Error!;
            Assert.Equal("projection.component-target-missing", target.MachineCode);
            Assert.Equal("Unity rejected the staged managed-scene projection.", target.Message);
            Assert.Equal("slice-humanoid", target.Context["subjectId"]);
            Assert.Equal("quaternius.ual1.prefab.ual1", target.Context["sourceLogicalId"]);
            Assert.Equal("unity.plan.component", target.Context["invariantId"]);
            Assert.Equal("preflight", target.Context["phase"]);
            Assert.Equal("repair component schema/field/reference", target.Context["validationContext"]);
            Assert.Equal(H1ProjectionContract.RendererTargetRepairHint, target.RepairHint);

            // A scene-scoped diagnostic names no subject; its code-specific hint still applies.
            var inactive = H1ManagedSceneExecutor.WorkerFailure(Reply("projection.active-scene-missing",
                "{\"code\":\"projection.active-scene-missing\",\"invariantId\":\"unity.scene.effective-observation\"," +
                "\"phase\":\"post-materialization\",\"canonicalResource\":\"arkus.h1-05.scene.potes\",\"logicalAsset\":\"\"," +
                "\"managedPath\":\"Assets/Arkus/H1/ManagedScenes/current.json\",\"context\":\"repair the active manifest/current managed generation\"}")).Error!;
            Assert.False(inactive.Context.ContainsKey("subjectId"));
            Assert.False(inactive.Context.ContainsKey("sourceLogicalId"));
            Assert.Equal(H1ProjectionContract.ActiveSceneMissingRepairHint, inactive.RepairHint);

            // A reply whose diagnostics do not carry its code (or carry none) keeps the accepted empty context and default hint.
            var bare = H1ManagedSceneExecutor.WorkerFailure(Reply("projection.editor-failure", "")).Error!;
            Assert.Empty(bare.Context);
            Assert.Equal(H1ProjectionContract.DefaultWorkerRepairHint, bare.RepairHint);
            Assert.Equal(H1ProjectionContract.RendererMaterialSlotRepairHint, H1ProjectionContract.WorkerRepairHint("projection.component-material-slot-cardinality"));
            Assert.Equal(H1ProjectionContract.RendererTargetRepairHint, H1ProjectionContract.WorkerRepairHint("projection.component-target-cardinality"));

            foreach (var error in new[] { target, inactive, bare })
            {
                Assert.Empty(PortableData.Validate(error.ToData()));
                Assert.Empty(CanonicalContractSchemas.StructuredError().ValidateValue(error.ToData()));
            }
        }

        private static H1ManagedSceneWorkerReply Reply(string errorCode, string diagnostic)
        {
            var payload = "{\"schemaId\":\"arkus.h1-projection-worker-result@1\",\"sceneLogicalId\":\"arkus.h1-05.scene.potes\"," +
                "\"expectedInputDigest\":\"\",\"errorCode\":\"" + errorCode + "\",\"observation\":{\"schemaId\":\"\",\"nodes\":[]}," +
                "\"validation\":{\"schemaId\":\"arkus.h1-unity-validation-result@1\",\"valid\":false,\"executedInvariantIds\":[]," +
                "\"diagnostics\":[" + diagnostic + "]}}";
            return JsonSerializer.Deserialize<H1ManagedSceneWorkerReply>(payload,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true })!;
        }

        [Fact]
        public void Component_adapter_source_has_no_generic_reflection_or_serialized_property_mutation_endpoint()
        {
            var root = H1UnityLaunchProfile.ForCurrentHost().RepositoryRoot;
            var source = File.ReadAllText(Path.Combine(root,
                "Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1ComponentProjection.cs"));

            Assert.DoesNotContain("new SerializedObject", source, StringComparison.Ordinal);
            Assert.DoesNotContain(".FindProperty(", source, StringComparison.Ordinal);
            Assert.DoesNotContain("GetProperty(", source, StringComparison.Ordinal);
            Assert.DoesNotContain("GetField(", source, StringComparison.Ordinal);
            Assert.DoesNotContain("SetValue(", source, StringComparison.Ordinal);
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
