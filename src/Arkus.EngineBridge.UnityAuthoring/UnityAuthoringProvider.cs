using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;

namespace Arkus.EngineBridge.UnityAuthoring
{
    public static class UnityAuthoringProvider
    {
        public const string ProviderId = "arkus.unity-authoring";
        public const string Scope = "unity-authoring";
        public const string CapabilityNamespace = "unity.binding";
        public const string CatalogueReferenceNamespace = "ref.arkus.unity-authoring.catalogue";
        public const string CompileName = "unity.binding.compile";
        public const string DecodeName = "unity.binding.decode";
        public const string InspectName = "unity.binding.inspect";
        public const string ContractVersionText = "1.0";

        private static readonly ContractVersion Version = ContractVersion.Parse(ContractVersionText);

        public static CanonicalProviderContribution CreateContribution()
        {
            var provider = new ProviderMetadata(ProviderId, ProviderKind.Scoped, Scope, CapabilityNamespace);
            var definitions = new[]
            {
                Define(
                    CompileName,
                    CompileRequestSchema(),
                    CompileSuccessSchema(),
                    new[] { "structured-binding-document-valid" },
                    new[] { "canonical-and-catalogue-dependencies-derived-from-structured-truth", "no-canonical-state-persisted" },
                    2,
                    "portable Unity binding normalization and dependency derivation"),
                Define(
                    DecodeName,
                    DecodeRequestSchema(),
                    DecodeSuccessSchema(),
                    new[] { "versioned-binding-payload-valid" },
                    new[] { "normalized-binding-and-derived-dependencies-reconstructed", "no-canonical-state-persisted" },
                    1,
                    "portable Unity binding decode"),
                Define(
                    InspectName,
                    InspectRequestSchema(),
                    CompileSuccessSchema(),
                    new[] { "versioned-binding-payload-valid", "extension-dependencies-supplied" },
                    new[] { "structured-binding-and-hk02a-dependencies-cross-checked", "no-canonical-state-persisted" },
                    2,
                    "portable Unity binding inspection and dependency reconciliation")
            };

            return new CanonicalProviderContribution(
                new ProviderDescriptor(
                    ProviderId,
                    ProviderKind.Scoped,
                    Scope,
                    new[] { CapabilityNamespace }),
                definitions,
                new[]
                {
                    CapabilityRoute.FromHandler(new CompileHandler()),
                    CapabilityRoute.FromHandler(new DecodeHandler()),
                    CapabilityRoute.FromHandler(new InspectHandler())
                });
        }

        private static CapabilityDefinition Define(
            string name,
            JsonSchemaDocument request,
            JsonSchemaDocument success,
            IReadOnlyList<string> preconditions,
            IReadOnlyList<string> postconditions,
            int relativeCost,
            string note)
        {
            return new CapabilityDefinition(
                new CapabilityKey(name, Version),
                new ProviderMetadata(ProviderId, ProviderKind.Scoped, Scope, CapabilityNamespace),
                request,
                success,
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.None,
                DeterminismClass.Deterministic,
                preconditions,
                postconditions,
                new ConcurrencySemantics(ConcurrencyClass.ParallelSafe),
                new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Unsupported),
                new RepairSemantics(false, true),
                new PolicySemantics(
                    PrivilegeClass.Authoring,
                    TransactionRequirement.ReadOnlyEnvelope,
                    ProvenanceRequirement.Minimal),
                new CostSemantics(relativeCost, note));
        }

        private static JsonSchemaDocument CompileRequestSchema()
        {
            return new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["subjectId"] = SchemaNode.String(),
                    ["binding"] = BindingSchema(),
                    ["expectedCanonicalDependencies"] = SchemaNode.Array(CanonicalDependencySchema()),
                    ["expectedCatalogueDependencies"] = SchemaNode.Array(CatalogueDependencySchema())
                },
                new[] { "subjectId", "binding" }));
        }

        private static JsonSchemaDocument DecodeRequestSchema()
        {
            return new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["payloadBase64"] = SchemaNode.String()
                },
                new[] { "payloadBase64" }));
        }

        private static JsonSchemaDocument InspectRequestSchema()
        {
            return new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["subjectId"] = SchemaNode.String(),
                    ["dependencies"] = SchemaNode.Array(CanonicalDependencySchema()),
                    ["payloadBase64"] = SchemaNode.String()
                },
                new[] { "subjectId", "dependencies", "payloadBase64" }));
        }

        private static JsonSchemaDocument CompileSuccessSchema()
        {
            return new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["schemaId"] = SchemaNode.String(new[] { UnityBindingProducer.ResultSchemaId }),
                    ["binding"] = BindingSchema(),
                    ["canonicalDependencies"] = SchemaNode.Array(CanonicalDependencySchema()),
                    ["catalogueDependencies"] = SchemaNode.Array(CatalogueDependencySchema()),
                    ["payloadBase64"] = SchemaNode.String(),
                    ["extensionMutation"] = ExtensionMutationSchema()
                },
                new[]
                {
                    "schemaId", "binding", "canonicalDependencies", "catalogueDependencies",
                    "payloadBase64", "extensionMutation"
                }));
        }

        private static JsonSchemaDocument DecodeSuccessSchema()
        {
            return new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["schemaId"] = SchemaNode.String(new[] { UnityBindingProducer.ResultSchemaId }),
                    ["binding"] = BindingSchema(),
                    ["canonicalDependencies"] = SchemaNode.Array(CanonicalDependencySchema()),
                    ["catalogueDependencies"] = SchemaNode.Array(CatalogueDependencySchema()),
                    ["payloadBase64"] = SchemaNode.String()
                },
                new[] { "schemaId", "binding", "canonicalDependencies", "catalogueDependencies", "payloadBase64" }));
        }

        private static SchemaNode BindingSchema()
        {
            return SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["schemaId"] = SchemaNode.String(new[] { UnityBindingProducer.BindingSchemaId }),
                    ["targetSceneId"] = CatalogueIdSchema(),
                    ["source"] = SchemaNode.Object(
                        new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                        {
                            ["kind"] = SchemaNode.String(new[] { "asset", "prefab" }),
                            ["logicalId"] = CatalogueIdSchema()
                        },
                        new[] { "kind", "logicalId" }),
                    ["transform"] = TransformSchema(),
                    ["components"] = SchemaNode.Array(ComponentSchema())
                },
                new[] { "schemaId", "targetSceneId", "source", "transform", "components" });
        }

        private static SchemaNode TransformSchema()
        {
            return SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["coordinateConvention"] = SchemaNode.String(new[] { UnityBindingProducer.CoordinateConvention }),
                    ["positionMm"] = VectorSchema(),
                    ["rotationMilliDegrees"] = VectorSchema(),
                    ["scalePpm"] = VectorSchema()
                },
                new[] { "coordinateConvention", "positionMm", "rotationMilliDegrees", "scalePpm" });
        }

        private static SchemaNode VectorSchema()
        {
            return SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["x"] = SchemaNode.Integer(),
                    ["y"] = SchemaNode.Integer(),
                    ["z"] = SchemaNode.Integer()
                },
                new[] { "x", "y", "z" });
        }

        // SchemaNode intentionally has no conditional/oneOf construct. The public schema exposes the
        // finite admitted field universe while the versioned producer performs the stronger per-kind
        // semantic validation and rejects every unknown/invalid combination before emitting payload.
        private static SchemaNode ComponentSchema()
        {
            return SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["kind"] = SchemaNode.String(new[] { "canonical-link", "renderer", "animator" }),
                    ["relation"] = SchemaNode.String(),
                    ["targetObjectId"] = SchemaNode.String(),
                    ["materialId"] = CatalogueIdSchema(),
                    ["clipId"] = CatalogueIdSchema()
                },
                new[] { "kind" });
        }

        private static SchemaNode CanonicalDependencySchema()
        {
            return SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["kind"] = SchemaNode.String(),
                    ["targetId"] = SchemaNode.String()
                },
                new[] { "kind", "targetId" });
        }

        private static SchemaNode CatalogueDependencySchema()
        {
            return SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["kind"] = SchemaNode.String(new[] { "scene", "asset", "prefab", "material", "animation-clip" }),
                    ["logicalId"] = CatalogueIdSchema()
                },
                new[] { "kind", "logicalId" });
        }

        private static SchemaNode ExtensionMutationSchema()
        {
            return SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["kind"] = SchemaNode.String(new[] { "put-extension" }),
                    ["owner"] = SchemaNode.String(new[] { UnityBindingProducer.ExtensionOwner }),
                    ["schemaVersion"] = SchemaNode.Integer(),
                    ["subjectId"] = SchemaNode.String(),
                    ["dependencies"] = SchemaNode.Array(CanonicalDependencySchema()),
                    ["payloadBase64"] = SchemaNode.String()
                },
                new[] { "kind", "owner", "schemaVersion", "subjectId", "dependencies", "payloadBase64" });
        }

        private static SchemaNode CatalogueIdSchema()
        {
            return SchemaNode.String(
                format: "arkus-logical-reference",
                logicalReferenceNamespace: CatalogueReferenceNamespace);
        }

        private static CapabilityInvocationResult Invoke(
            Func<IReadOnlyDictionary<string, object?>, IReadOnlyDictionary<string, object?>> action,
            IReadOnlyDictionary<string, object?> request)
        {
            try
            {
                return CapabilityInvocationResult.Succeeded(action(request));
            }
            catch (UnityBindingException exception)
            {
                return CapabilityInvocationResult.Failed(new StructuredError(
                    exception.MachineCode,
                    exception.Message,
                    exception.Path,
                    EmptyContext(),
                    false,
                    "Repair the structured Unity binding document or dependency assertion and retry."));
            }
        }

        private static IReadOnlyDictionary<string, object?> EmptyContext()
        {
            return new ReadOnlyDictionary<string, object?>(
                new Dictionary<string, object?>(StringComparer.Ordinal));
        }

        [PublicCapabilityRoute(ProviderId, CompileName, ContractVersionText)]
        private sealed class CompileHandler : ICanonicalCapabilityHandler
        {
            public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request)
            {
                if (context == null) throw new ArgumentNullException(nameof(context));
                if (request == null) throw new ArgumentNullException(nameof(request));
                return UnityAuthoringProvider.Invoke(UnityBindingProducer.Compile, request);
            }
        }

        [PublicCapabilityRoute(ProviderId, DecodeName, ContractVersionText)]
        private sealed class DecodeHandler : ICanonicalCapabilityHandler
        {
            public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request)
            {
                if (context == null) throw new ArgumentNullException(nameof(context));
                if (request == null) throw new ArgumentNullException(nameof(request));
                return UnityAuthoringProvider.Invoke(UnityBindingProducer.Decode, request);
            }
        }

        [PublicCapabilityRoute(ProviderId, InspectName, ContractVersionText)]
        private sealed class InspectHandler : ICanonicalCapabilityHandler
        {
            public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request)
            {
                if (context == null) throw new ArgumentNullException(nameof(context));
                if (request == null) throw new ArgumentNullException(nameof(request));
                return UnityAuthoringProvider.Invoke(UnityBindingProducer.Inspect, request);
            }
        }
    }
}
