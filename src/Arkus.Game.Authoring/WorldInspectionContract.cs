using System;
using System.Collections.Generic;
using Arkus.Harness.Protocol;

namespace Arkus.Game.Authoring
{
    /// <summary>Canonical schema/definition source for HK03 world reads.</summary>
    public static class WorldInspectionContract
    {
        public const string SummaryName = "world.summary";
        public const string ObjectGetName = "world.object.get";
        public const string ObjectQueryName = "world.object.query";
        public const string ReferenceQueryName = "world.reference.query";
        public const string ExtensionQueryName = "world.extension.query";
        public const string ExtensionReadName = "world.extension.read";

        private static readonly ContractVersion Version = new ContractVersion(1, 0);

        public static IReadOnlyList<CapabilityDefinition> CreateDefinitions()
        {
            return new List<CapabilityDefinition>
            {
                Define(SummaryName, CanonicalContractSchemas.EmptyObject(), SummarySuccessSchema(), 1),
                Define(ObjectGetName, ObjectGetRequestSchema(), ObjectGetSuccessSchema(), 2),
                Define(ObjectQueryName, ObjectQueryRequestSchema(), ObjectQuerySuccessSchema(), 3),
                Define(ReferenceQueryName, ReferenceQueryRequestSchema(), ReferenceQuerySuccessSchema(), 3),
                Define(ExtensionQueryName, ExtensionQueryRequestSchema(), ExtensionQuerySuccessSchema(), 2),
                Define(ExtensionReadName, ExtensionReadRequestSchema(), ExtensionReadSuccessSchema(), 3)
            }.AsReadOnly();
        }

        private static CapabilityDefinition Define(
            string name,
            JsonSchemaDocument request,
            JsonSchemaDocument success,
            int cost)
        {
            return new CapabilityDefinition(
                new CapabilityKey(name, Version),
                new ProviderMetadata("arkus.base", ProviderKind.Base, "base", "world"),
                request,
                success,
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.ReadOnly,
                DeterminismClass.Deterministic,
                Array.Empty<string>(),
                new[] { "response-binds-world-revision-and-content-hash", "read-is-side-effect-free" },
                new ConcurrencySemantics(ConcurrencyClass.ParallelSafe),
                new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Unsupported),
                new RepairSemantics(true, true),
                new PolicySemantics(
                    PrivilegeClass.PublicRead,
                    TransactionRequirement.ReadOnlyEnvelope,
                    ProvenanceRequirement.Required),
                new CostSemantics(cost, "bounded deterministic canonical world inspection"));
        }

        private static JsonSchemaDocument ObjectGetRequestSchema()
        {
            var properties = AnchorProperties();
            properties["id"] = SchemaNode.String();
            properties["fields"] = ObjectFieldsSchema();
            return ObjectSchema(properties, new[] { "revision", "hash", "id" });
        }

        private static JsonSchemaDocument ObjectQueryRequestSchema()
        {
            var properties = AnchorProperties();
            properties["filter"] = SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["ids"] = SchemaNode.Array(SchemaNode.String()),
                    ["typeIds"] = SchemaNode.Array(SchemaNode.String()),
                    ["containerIds"] = SchemaNode.Array(SchemaNode.String()),
                    ["referenceKinds"] = SchemaNode.Array(SchemaNode.String()),
                    ["targetIds"] = SchemaNode.Array(SchemaNode.String())
                });
            properties["fields"] = ObjectFieldsSchema();
            properties["limit"] = SchemaNode.Integer();
            properties["cursor"] = SchemaNode.String();
            return ObjectSchema(properties, new[] { "revision", "hash" });
        }

        private static JsonSchemaDocument ReferenceQueryRequestSchema()
        {
            var properties = AnchorProperties();
            properties["filter"] = SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["sourceIds"] = SchemaNode.Array(SchemaNode.String()),
                    ["kinds"] = SchemaNode.Array(SchemaNode.String()),
                    ["targetIds"] = SchemaNode.Array(SchemaNode.String())
                });
            properties["limit"] = SchemaNode.Integer();
            properties["cursor"] = SchemaNode.String();
            return ObjectSchema(properties, new[] { "revision", "hash" });
        }

        private static JsonSchemaDocument ExtensionQueryRequestSchema()
        {
            var properties = AnchorProperties();
            properties["filter"] = SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["owners"] = SchemaNode.Array(SchemaNode.String()),
                    ["schemaVersions"] = SchemaNode.Array(SchemaNode.Integer()),
                    ["scopes"] = SchemaNode.Array(SchemaNode.String(new[] { "global", "object" })),
                    ["subjectIds"] = SchemaNode.Array(SchemaNode.String())
                });
            properties["limit"] = SchemaNode.Integer();
            properties["cursor"] = SchemaNode.String();
            return ObjectSchema(properties, new[] { "revision", "hash" });
        }

        private static JsonSchemaDocument ExtensionReadRequestSchema()
        {
            var properties = AnchorProperties();
            properties["owner"] = SchemaNode.String();
            properties["schemaVersion"] = SchemaNode.Integer();
            properties["subjectId"] = SchemaNode.String();
            properties["offset"] = SchemaNode.Integer();
            properties["limit"] = SchemaNode.Integer();
            properties["dependencyOffset"] = SchemaNode.Integer();
            properties["dependencyLimit"] = SchemaNode.Integer();
            return ObjectSchema(properties, new[] { "revision", "hash", "owner", "schemaVersion" });
        }

        private static JsonSchemaDocument SummarySuccessSchema()
        {
            var properties = WithWorldMetadata(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
            {
                ["objectCount"] = SchemaNode.Integer(),
                ["referenceCount"] = SchemaNode.Integer(),
                ["extensionCount"] = SchemaNode.Integer()
            });
            return ObjectSchema(properties, new[] { "world", "objectCount", "referenceCount", "extensionCount" });
        }

        private static JsonSchemaDocument ObjectGetSuccessSchema()
        {
            var properties = WithWorldMetadata(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
            {
                ["object"] = ObjectResultSchema()
            });
            return ObjectSchema(properties, new[] { "world", "object" });
        }

        private static JsonSchemaDocument ObjectQuerySuccessSchema()
        {
            var properties = WithWorldMetadata(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
            {
                ["items"] = SchemaNode.Array(ObjectResultSchema()),
                ["nextCursor"] = SchemaNode.String()
            });
            return ObjectSchema(properties, new[] { "world", "items" });
        }

        private static JsonSchemaDocument ReferenceQuerySuccessSchema()
        {
            var row = SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["sourceId"] = SchemaNode.String(),
                    ["kind"] = SchemaNode.String(),
                    ["targetId"] = SchemaNode.String()
                },
                new[] { "sourceId", "kind", "targetId" });
            var properties = WithWorldMetadata(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
            {
                ["items"] = SchemaNode.Array(row),
                ["nextCursor"] = SchemaNode.String()
            });
            return ObjectSchema(properties, new[] { "world", "items" });
        }

        private static JsonSchemaDocument ExtensionQuerySuccessSchema()
        {
            var descriptor = SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["owner"] = SchemaNode.String(),
                    ["schemaVersion"] = SchemaNode.Integer(),
                    ["scope"] = SchemaNode.String(new[] { "global", "object" }),
                    ["subjectId"] = SchemaNode.String(),
                    ["payloadLength"] = SchemaNode.Integer(),
                    ["dependencyCount"] = SchemaNode.Integer()
                },
                new[] { "owner", "schemaVersion", "scope", "payloadLength", "dependencyCount" });
            var properties = WithWorldMetadata(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
            {
                ["items"] = SchemaNode.Array(descriptor),
                ["nextCursor"] = SchemaNode.String()
            });
            return ObjectSchema(properties, new[] { "world", "items" });
        }

        private static JsonSchemaDocument ExtensionReadSuccessSchema()
        {
            var dependency = SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["kind"] = SchemaNode.String(),
                    ["targetId"] = SchemaNode.String()
                },
                new[] { "kind", "targetId" });
            var properties = WithWorldMetadata(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
            {
                ["owner"] = SchemaNode.String(),
                ["schemaVersion"] = SchemaNode.Integer(),
                ["scope"] = SchemaNode.String(new[] { "global", "object" }),
                ["subjectId"] = SchemaNode.String(),
                ["payloadLength"] = SchemaNode.Integer(),
                ["offset"] = SchemaNode.Integer(),
                ["payloadBase64"] = SchemaNode.String(),
                ["nextOffset"] = SchemaNode.Integer(),
                ["dependencyCount"] = SchemaNode.Integer(),
                ["dependencyOffset"] = SchemaNode.Integer(),
                ["dependencies"] = SchemaNode.Array(dependency),
                ["nextDependencyOffset"] = SchemaNode.Integer()
            });
            return ObjectSchema(
                properties,
                new[]
                {
                    "world", "owner", "schemaVersion", "scope", "payloadLength", "offset", "payloadBase64",
                    "dependencyCount", "dependencyOffset", "dependencies"
                });
        }

        private static SchemaNode ObjectResultSchema()
        {
            return SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["id"] = SchemaNode.String(),
                    ["typeId"] = SchemaNode.String(),
                    ["containerId"] = SchemaNode.String()
                },
                new[] { "id" });
        }

        private static SchemaNode ObjectFieldsSchema()
        {
            return SchemaNode.Array(SchemaNode.String(new[] { "typeId", "containerId" }));
        }

        private static Dictionary<string, SchemaNode> AnchorProperties()
        {
            return new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
            {
                ["revision"] = SchemaNode.Integer(),
                ["hash"] = SchemaNode.String()
            };
        }

        private static Dictionary<string, SchemaNode> WithWorldMetadata(Dictionary<string, SchemaNode> properties)
        {
            properties["world"] = SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["worldId"] = SchemaNode.String(),
                    ["schemaVersion"] = SchemaNode.Integer(),
                    ["revision"] = SchemaNode.Integer(),
                    ["hash"] = SchemaNode.String()
                },
                new[] { "worldId", "schemaVersion", "revision", "hash" });
            return properties;
        }

        private static JsonSchemaDocument ObjectSchema(
            IReadOnlyDictionary<string, SchemaNode> properties,
            IEnumerable<string> required)
        {
            return new JsonSchemaDocument(SchemaNode.Object(properties, required));
        }
    }
}
