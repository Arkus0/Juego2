using System;
using System.Collections.Generic;
using System.Linq;
using Arkus.EngineBridge.UnityAuthoring;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1UnityAuthoringProducerTests
    {
        [Fact]
        public void ScopedUnityProviderComposesAndProductionProjectionDiscoversIt()
        {
            var contribution = UnityAuthoringProvider.CreateContribution();
            var contract = CanonicalWorldContract.ComposeEmptyPortableSession(
                "world.h1-provider",
                new[] { contribution });

            Assert.Equal(3, contribution.Definitions.Count);
            Assert.All(contribution.Definitions, definition =>
            {
                Assert.Equal(UnityAuthoringProvider.ProviderId, definition.Provider.ProviderId);
                Assert.Equal(ProviderKind.Scoped, definition.Provider.Kind);
                Assert.Equal(UnityAuthoringProvider.Scope, definition.Provider.Scope);
                Assert.Equal(UnityAuthoringProvider.CapabilityNamespace, definition.Provider.CapabilityNamespace);
                Assert.Equal(UnityAuthoringProvider.ContractVersionText, definition.Key.Version.ToString());
                Assert.Equal(SideEffectClass.None, definition.SideEffect);
                Assert.Equal(DeterminismClass.Deterministic, definition.Determinism);
            });

            Assert.Contains(contract.Definitions, value => value.Key.Name == UnityAuthoringProvider.CompileName);
            Assert.Contains(contract.Definitions, value => value.Key.Name == UnityAuthoringProvider.DecodeName);
            Assert.Contains(contract.Definitions, value => value.Key.Name == UnityAuthoringProvider.InspectName);

            using var production = ProductionHarnessHost.Create();
            var productionKeys = production.Capabilities.Select(value => value.Key.Name).ToArray();
            Assert.Contains(UnityAuthoringProvider.CompileName, productionKeys);
            Assert.Contains(UnityAuthoringProvider.DecodeName, productionKeys);
            Assert.Contains(UnityAuthoringProvider.InspectName, productionKeys);

            var universe = RouteUniverse.Enumerate(new[]
            {
                typeof(CanonicalWorldContract).Assembly,
                typeof(UnityAuthoringProvider).Assembly
            });
            Assert.Empty(universe.Issues);
            var report = CanonicalContractConformance.Evaluate(contract, universe);
            Assert.True(report.IsConformant,
                "Scoped provider conformance failed: " + string.Join(", ", report.Issues.Select(value => value.Code + "@" + value.Subject)));
        }

        [Fact]
        public void PotesFacadeProbeDerivesEveryStructuredReferenceExactlyOnceAndRoundTrips()
        {
            var contract = Contract();
            var request = CompileRequest(PotesBinding(reverseComponents: false));
            var reversed = CompileRequest(PotesBinding(reverseComponents: true));

            var compiled = Success(contract, UnityAuthoringProvider.CompileName, request);
            var reordered = Success(contract, UnityAuthoringProvider.CompileName, reversed);

            // Independent test oracle: this expected reference set is derived from the authored fixture,
            // not from provider registries, codec output or dependency-derivation implementation.
            Assert.Equal(
                new[] { "attached-to|market.potes-root" },
                DependencyTokens(compiled, "canonicalDependencies", "kind", "targetId"));
            Assert.Equal(
                new[]
                {
                    "animation-clip|animation.potes-shutter",
                    "material|material.potes-stone",
                    "prefab|prefab.potes-facade",
                    "scene|scene.potes-market"
                },
                DependencyTokens(compiled, "catalogueDependencies", "kind", "logicalId"));

            Assert.Equal((string)compiled["payloadBase64"]!, (string)reordered["payloadBase64"]!);

            var extension = Map(compiled, "extensionMutation");
            Assert.Equal("put-extension", extension["kind"]);
            Assert.Equal(UnityBindingProducer.ExtensionOwner, extension["owner"]);
            Assert.Equal(UnityBindingProducer.ExtensionSchemaVersion, Convert.ToInt32(extension["schemaVersion"]));
            Assert.Equal("building.potes-facade", extension["subjectId"]);
            Assert.Equal(
                new[] { "attached-to|market.potes-root" },
                DependencyTokens(extension, "dependencies", "kind", "targetId"));

            var decoded = Success(contract, UnityAuthoringProvider.DecodeName,
                ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["payloadBase64"] = compiled["payloadBase64"]
                }));
            Assert.Equal((string)compiled["payloadBase64"]!, (string)decoded["payloadBase64"]!);
            Assert.Equal(
                DependencyTokens(compiled, "canonicalDependencies", "kind", "targetId"),
                DependencyTokens(decoded, "canonicalDependencies", "kind", "targetId"));
            Assert.Equal(
                DependencyTokens(compiled, "catalogueDependencies", "kind", "logicalId"),
                DependencyTokens(decoded, "catalogueDependencies", "kind", "logicalId"));

            var inspected = Success(contract, UnityAuthoringProvider.InspectName,
                ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["subjectId"] = "building.potes-facade",
                    ["dependencies"] = extension["dependencies"],
                    ["payloadBase64"] = compiled["payloadBase64"]
                }));
            Assert.Equal((string)compiled["payloadBase64"]!, (string)inspected["payloadBase64"]!);
            Assert.Equal(
                DependencyTokens(compiled, "catalogueDependencies", "kind", "logicalId"),
                DependencyTokens(inspected, "catalogueDependencies", "kind", "logicalId"));
        }

        [Fact]
        public void CallerMaintainedDependencyTruthFailsClosedOnOmissionContradictionAndDuplicates()
        {
            var contract = Contract();

            var missingCanonical = CompileRequest(PotesBinding(false));
            missingCanonical["expectedCanonicalDependencies"] = Array.Empty<object?>();
            AssertFailure(
                contract,
                missingCanonical,
                "unity.binding.canonical-dependency-mismatch");

            var wrongCatalogue = CompileRequest(PotesBinding(false));
            wrongCatalogue["expectedCatalogueDependencies"] = new List<object?>
            {
                CatalogueDependency("scene", "scene.potes-market"),
                CatalogueDependency("prefab", "prefab.potes-facade"),
                CatalogueDependency("material", "material.potes-wrong"),
                CatalogueDependency("animation-clip", "animation.potes-shutter")
            }.AsReadOnly();
            AssertFailure(
                contract,
                wrongCatalogue,
                "unity.binding.catalogue-dependency-mismatch");

            var duplicateCanonical = CompileRequest(PotesBinding(false));
            var duplicate = CanonicalDependency("attached-to", "market.potes-root");
            duplicateCanonical["expectedCanonicalDependencies"] = new List<object?> { duplicate, duplicate }.AsReadOnly();
            AssertFailure(
                contract,
                duplicateCanonical,
                "unity.binding.duplicate-dependency-assertion");

            var contradictory = CompileRequest(PotesBinding(false));
            contradictory["expectedCanonicalDependencies"] = new List<object?>
            {
                CanonicalDependency("inside", "market.potes-root")
            }.AsReadOnly();
            AssertFailure(
                contract,
                contradictory,
                "unity.binding.canonical-dependency-mismatch");
        }

        [Fact]
        public void CompiledExtensionFragmentUsesAcceptedH0PlanDryRunApplyPath()
        {
            var initial = new WorldState(
                new WorldId("world.potes-h1"),
                7,
                new[]
                {
                    new WorldObject(new WorldObjectId("building.potes-facade"), new WorldTypeId("fixture.facade")),
                    new WorldObject(new WorldObjectId("market.potes-root"), new WorldTypeId("fixture.market-root"))
                });
            var session = new PortableWorldAuthoringSession(initial);
            var contract = CanonicalWorldContract.Compose(
                new WorldInspectionService(session),
                session,
                new[] { UnityAuthoringProvider.CreateContribution() });

            var compiled = Success(contract, UnityAuthoringProvider.CompileName, CompileRequest(PotesBinding(false)));
            var operation = Map(compiled, "extensionMutation");
            var request = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["idempotencyKey"] = "request.h1-01.potes-binding",
                ["expectedRevision"] = initial.Revision,
                ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(initial),
                ["operations"] = new List<object?> { operation }.AsReadOnly()
            });

            var plan = contract.Dispatch(
                WorldMutationContract.PlanName,
                ExactVersion(),
                request);
            var dryRun = contract.Dispatch(
                WorldMutationContract.DryRunName,
                ExactVersion(),
                request);
            var apply = contract.Dispatch(
                WorldMutationContract.ApplyName,
                ExactVersion(),
                request);

            Assert.True(plan.Success, plan.Error?.MachineCode);
            Assert.True(dryRun.Success, dryRun.Error?.MachineCode);
            Assert.True(apply.Success, apply.Error?.MachineCode);
            Assert.Single(session.Current.Extensions);
            var stored = session.Current.Extensions[0];
            Assert.Equal(UnityBindingProducer.ExtensionOwner, stored.Owner);
            Assert.Equal(UnityBindingProducer.ExtensionSchemaVersion, stored.SchemaVersion);
            Assert.Equal(new WorldObjectId("building.potes-facade"), stored.SubjectId);
            Assert.Single(stored.Dependencies);
            Assert.Equal(new WorldObjectId("market.potes-root"), stored.Dependencies[0].TargetId);
        }

        [Fact]
        public void PortableProviderRejectsNativeLocatorFieldsAndReferencesNoUnityClrAssembly()
        {
            var references = typeof(UnityAuthoringProvider).Assembly.GetReferencedAssemblies()
                .Select(value => value.Name ?? string.Empty)
                .ToArray();
            Assert.DoesNotContain(references, value => value.StartsWith("UnityEngine", StringComparison.Ordinal));
            Assert.DoesNotContain(references, value => value.StartsWith("UnityEditor", StringComparison.Ordinal));

            var binding = new Dictionary<string, object?>(PotesBinding(false), StringComparer.Ordinal);
            var source = new Dictionary<string, object?>(Map(binding, "source"), StringComparer.Ordinal)
            {
                ["guid"] = "deadbeef",
                ["path"] = "Assets/Potes/Facade.prefab"
            };
            binding["source"] = ReadOnly(source);
            var exception = Assert.Throws<UnityBindingException>(() =>
                UnityBindingProducer.Compile(CompileRequest(ReadOnly(binding))));
            Assert.Equal("unity.binding.unknown-field", exception.MachineCode);
        }

        [Fact]
        public void SameVersionSemanticDriftChangesFingerprintAndDuplicateIdentityIsRejected()
        {
            var provider = UnityAuthoringProvider.CreateContribution();
            var original = provider.Definitions.Single(value => value.Key.Name == UnityAuthoringProvider.CompileName);
            var drifted = new CapabilityDefinition(
                original.Key,
                original.Provider,
                new JsonSchemaDocument(SchemaNode.Object(
                    new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                    {
                        ["subjectId"] = SchemaNode.String(),
                        ["binding"] = SchemaNode.Any(),
                        ["silentlyChangedMeaning"] = SchemaNode.Boolean()
                    },
                    new[] { "subjectId", "binding" })),
                original.SuccessSchema,
                original.ErrorSchema,
                original.SideEffect,
                original.Determinism,
                original.Preconditions,
                original.Postconditions,
                original.Concurrency,
                original.Idempotency,
                original.Batching,
                original.Repair,
                original.Policy,
                original.Cost);

            Assert.NotEqual(original.SemanticFingerprint(), drifted.SemanticFingerprint());

            var collision = new CanonicalProviderContribution(
                provider.Descriptor,
                provider.Definitions.Concat(new[] { drifted }),
                provider.Routes);
            var versionSource = new FixedWorldStateSource(new WorldState(
                new WorldId("world.h1-version"),
                0,
                Array.Empty<WorldObject>()));
            var baseContract = CanonicalWorldContract.CreateContribution(
                new WorldInspectionService(versionSource));
            var composition = ContractComposer.Compose(baseContract, new[] { collision });
            Assert.False(composition.Success);
            Assert.Contains(composition.Issues, value => value.Code == "composition.duplicate_capability");
        }

        private static ComposedContract Contract()
        {
            return CanonicalWorldContract.ComposeEmptyPortableSession(
                "world.h1-01",
                new[] { UnityAuthoringProvider.CreateContribution() });
        }

        private static IReadOnlyDictionary<string, object?> Success(
            ComposedContract contract,
            string capability,
            IReadOnlyDictionary<string, object?> request)
        {
            var result = contract.Dispatch(capability, ExactVersion(), request);
            Assert.True(result.Success, result.Error == null
                ? "Canonical dispatch failed without a structured error."
                : result.Error.MachineCode + ": " + result.Error.Message);
            return result.Data!;
        }

        private static void AssertFailure(
            ComposedContract contract,
            IReadOnlyDictionary<string, object?> request,
            string expectedCode)
        {
            var result = contract.Dispatch(UnityAuthoringProvider.CompileName, ExactVersion(), request);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(expectedCode, result.Error!.MachineCode);
        }

        private static ContractVersionRange ExactVersion() =>
            ContractVersionRange.Exact(new ContractVersion(1, 0));

        private static Dictionary<string, object?> CompileRequest(IReadOnlyDictionary<string, object?> binding)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["subjectId"] = "building.potes-facade",
                ["binding"] = binding
            };
        }

        private static IReadOnlyDictionary<string, object?> PotesBinding(bool reverseComponents)
        {
            var components = new List<object?>
            {
                ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "renderer",
                    ["materialId"] = "material.potes-stone"
                }),
                ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "canonical-link",
                    ["relation"] = "attached-to",
                    ["targetObjectId"] = "market.potes-root"
                }),
                ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "animator",
                    ["clipId"] = "animation.potes-shutter"
                })
            };
            if (reverseComponents) components.Reverse();

            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = UnityBindingProducer.BindingSchemaId,
                ["targetSceneId"] = "scene.potes-market",
                ["source"] = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "prefab",
                    ["logicalId"] = "prefab.potes-facade"
                }),
                ["transform"] = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["coordinateConvention"] = UnityBindingProducer.CoordinateConvention,
                    ["positionMm"] = Vector(1250, 0, -375),
                    ["rotationMilliDegrees"] = Vector(0, 90000, 0),
                    ["scalePpm"] = Vector(1000000, 1000000, 1000000)
                }),
                ["components"] = components.AsReadOnly()
            });
        }

        private static IReadOnlyDictionary<string, object?> Vector(long x, long y, long z)
        {
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["x"] = x,
                ["y"] = y,
                ["z"] = z
            });
        }

        private static IReadOnlyDictionary<string, object?> CanonicalDependency(string kind, string targetId)
        {
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = kind,
                ["targetId"] = targetId
            });
        }

        private static IReadOnlyDictionary<string, object?> CatalogueDependency(string kind, string logicalId)
        {
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = kind,
                ["logicalId"] = logicalId
            });
        }

        private static string[] DependencyTokens(
            IReadOnlyDictionary<string, object?> source,
            string field,
            string left,
            string right)
        {
            return List(source, field)
                .Select(raw =>
                {
                    var map = (IReadOnlyDictionary<string, object?>)raw!;
                    return (string)map[left]! + "|" + (string)map[right]!;
                })
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToArray();
        }

        private static IReadOnlyDictionary<string, object?> Map(IReadOnlyDictionary<string, object?> source, string key)
        {
            return (IReadOnlyDictionary<string, object?>)source[key]!;
        }

        private static IReadOnlyList<object?> List(IReadOnlyDictionary<string, object?> source, string key)
        {
            return (IReadOnlyList<object?>)source[key]!;
        }

        private static IReadOnlyDictionary<string, object?> ReadOnly(Dictionary<string, object?> value)
        {
            return new System.Collections.ObjectModel.ReadOnlyDictionary<string, object?>(value);
        }
    }
}
