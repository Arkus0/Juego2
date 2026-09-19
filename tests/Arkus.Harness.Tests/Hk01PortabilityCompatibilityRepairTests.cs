using System;
using System.Collections.Generic;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk01SelfAttackTestsPortabilityCompatibility
    {
        [Fact]
        public void AllowedLogicalReferenceFormatCannotCarryClrOrEngineTypeNamespace()
        {
            var source = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 0), false);
            var request = new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["target"] = SchemaNode.String(
                        format: "arkus-logical-reference",
                        logicalReferenceNamespace: "UnityEngine.GameObject, UnityEngine.CoreModule")
                }));
            var invalid = Hk01TestFixtures.CopyWithRequest(source, request);

            var validation = CanonicalContractValidator.Validate(invalid);
            Assert.Contains(
                validation,
                issue => issue.Code == "contract.schema.non_portable_logical_reference_namespace");

            var scoped = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.engine", ProviderKind.Scoped, "engine", new[] { "engine" }),
                new[] { invalid },
                new[] { CapabilityRoute.FromHandler(new FixtureEngineHandler()) });
            var composition = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { scoped });

            Assert.False(composition.Success);
        }

        [Fact]
        public void LogicalReferenceNamespaceMustBelongToContributingProvider()
        {
            var source = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 0), false);
            var request = new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["target"] = SchemaNode.String(
                        format: "arkus-logical-reference",
                        logicalReferenceNamespace: "ref.other.provider.entity")
                }));
            var invalid = Hk01TestFixtures.CopyWithRequest(source, request);

            Assert.Empty(CanonicalContractValidator.Validate(invalid));

            var scoped = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.engine", ProviderKind.Scoped, "engine", new[] { "engine" }),
                new[] { invalid },
                new[] { CapabilityRoute.FromHandler(new FixtureEngineHandler()) });
            var composition = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { scoped });

            Assert.False(composition.Success);
            Assert.Contains(
                composition.Issues,
                issue => issue.Code == "composition.logical_reference_namespace_mismatch");
        }

        [Fact]
        public void LogicalReferenceSchemaCannotEncodeImplementationChoicesAsEnumValues()
        {
            var source = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 0), false);
            var request = new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["target"] = SchemaNode.String(
                        new[] { "UnityEngine.GameObject, UnityEngine.CoreModule" },
                        "arkus-logical-reference",
                        "ref.fixture.engine.entity")
                }));
            var invalid = Hk01TestFixtures.CopyWithRequest(source, request);

            var validation = CanonicalContractValidator.Validate(invalid);
            Assert.Contains(
                validation,
                issue => issue.Code == "contract.schema.logical_reference_enum_forbidden");
        }

        [Fact]
        public void TypingAPropertyOnPreviouslyOpenObjectIsBreaking()
        {
            var source = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 0), false);
            var previous = WithVersionAndRequest(
                source,
                new ContractVersion(1, 0),
                new JsonSchemaDocument(SchemaNode.Object(additionalPropertiesAllowed: true)));
            var next = WithVersionAndRequest(
                source,
                new ContractVersion(1, 1),
                new JsonSchemaDocument(SchemaNode.Object(
                    new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                    {
                        ["target"] = SchemaNode.String()
                    },
                    additionalPropertiesAllowed: true)));

            Assert.Equal(CompatibilityKind.Breaking, ContractCompatibility.Compare(previous, next).Kind);
        }

        [Fact]
        public void CompatibilityRelationRecursesThroughNestedOpenObjects()
        {
            var source = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 0), false);
            var previous = WithVersionAndRequest(
                source,
                new ContractVersion(1, 0),
                new JsonSchemaDocument(SchemaNode.Object(
                    new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                    {
                        ["options"] = SchemaNode.Object(additionalPropertiesAllowed: true)
                    })));
            var next = WithVersionAndRequest(
                source,
                new ContractVersion(1, 1),
                new JsonSchemaDocument(SchemaNode.Object(
                    new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                    {
                        ["options"] = SchemaNode.Object(
                            new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                            {
                                ["target"] = SchemaNode.String()
                            },
                            additionalPropertiesAllowed: true)
                    })));

            Assert.Equal(CompatibilityKind.Breaking, ContractCompatibility.Compare(previous, next).Kind);
        }

        [Fact]
        public void OpenObjectMayAddExplicitAnyPropertyWithoutNarrowingAcceptance()
        {
            var source = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 0), false);
            var previous = WithVersionAndRequest(
                source,
                new ContractVersion(1, 0),
                new JsonSchemaDocument(SchemaNode.Object(additionalPropertiesAllowed: true)));
            var next = WithVersionAndRequest(
                source,
                new ContractVersion(1, 1),
                new JsonSchemaDocument(SchemaNode.Object(
                    new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                    {
                        ["target"] = SchemaNode.Any()
                    },
                    additionalPropertiesAllowed: true)));

            Assert.Equal(CompatibilityKind.Additive, ContractCompatibility.Compare(previous, next).Kind);
        }

        [Fact]
        public void ComposerRejectsBreakingSameMajorVersionChainBeforeNegotiation()
        {
            var source = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 0), false);
            var previous = WithVersionAndRequest(
                source,
                new ContractVersion(1, 0),
                new JsonSchemaDocument(SchemaNode.Object(additionalPropertiesAllowed: true)));
            var next = WithVersionAndRequest(
                source,
                new ContractVersion(1, 1),
                new JsonSchemaDocument(SchemaNode.Object(
                    new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                    {
                        ["target"] = SchemaNode.String()
                    },
                    additionalPropertiesAllowed: true)));
            var scoped = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.engine", ProviderKind.Scoped, "engine", new[] { "engine" }),
                new[] { previous, next },
                new[]
                {
                    CapabilityRoute.FromHandler(new FixtureEngineHandler()),
                    CapabilityRoute.FromHandler(new FixtureEngineV11Handler())
                });

            var composition = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { scoped });

            Assert.False(composition.Success);
            Assert.Null(composition.Contract);
            Assert.Contains(
                composition.Issues,
                issue => issue.Code == "composition.breaking_same_major_version");
        }

        private static CapabilityDefinition WithVersionAndRequest(
            CapabilityDefinition source,
            ContractVersion version,
            JsonSchemaDocument request)
        {
            return new CapabilityDefinition(
                new CapabilityKey(source.Key.Name, version),
                source.Provider,
                request,
                source.SuccessSchema,
                source.ErrorSchema,
                source.SideEffect,
                source.Determinism,
                source.Preconditions,
                source.Postconditions,
                source.Concurrency,
                source.Idempotency,
                source.Batching,
                source.Repair,
                source.Policy,
                source.Cost);
        }
    }
}
