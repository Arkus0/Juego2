using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1UnityHostPolicyTests
    {
        private const string ResourceNamespace = "ref.arkus.unity-host.asset";

        [Fact]
        public async Task DirectGenericCompositionIsRejectedByH0AndAdmittedOnlyThroughH1Policy()
        {
            UnityHostFixtureHandler.InvocationCount = 0;
            var contract = Compose(HostContribution(InspectDefinition(), TouchDefinition()));

            Assert.Throws<InvalidOperationException>(() => new NeutralProjectionService(contract));
            Assert.Equal(0, UnityHostFixtureHandler.InvocationCount);

            using var projection = H1UnityHostCapabilityPolicy.CreateProjection(
                contract,
                UnityProjectWorkspaceAuthority.ForArkusUnityProject(),
                Grants());

            Assert.Contains(projection.Capabilities, item => item.Key.Name == "unity.host.inspect");
            Assert.Contains(projection.Capabilities, item => item.Key.Name == "unity.host.touch");

            var read = await projection.InvokeAsync(Request("read", "unity.host.inspect", "asset.potes.market-stall"));
            var effect = await projection.InvokeAsync(Request("touch", "unity.host.touch", "asset.potes.market-stall", "apply"));

            Assert.True(read.Success);
            Assert.True(effect.Success);
            Assert.Equal(2, UnityHostFixtureHandler.InvocationCount);
        }

        [Fact]
        public void MissingAndOrphanGrantsCannotMintUnityAuthority()
        {
            var contract = Compose(HostContribution(InspectDefinition()));
            var workspace = UnityProjectWorkspaceAuthority.ForArkusUnityProject();

            var missing = H1UnityHostCapabilityPolicy.Validate(contract, workspace, Array.Empty<UnityHostCapabilityGrant>());
            Assert.Contains(missing, issue => issue.Code == H1UnityHostCapabilityPolicy.MissingGrantCode);

            var orphan = new UnityHostCapabilityGrant(
                new CapabilityKey("unity.host.absent", new ContractVersion(1, 0)),
                UnityProjectWorkspaceAuthority.ManagedAssetsRootId,
                ResourceNamespace,
                UnityHostResourceClass.ManagedAsset,
                UnityHostTimeClass.BoundedRead);
            var withOrphan = H1UnityHostCapabilityPolicy.Validate(
                contract,
                workspace,
                new[] { Grant("unity.host.inspect", UnityHostTimeClass.BoundedRead), orphan });
            Assert.Contains(withOrphan, issue => issue.Code == H1UnityHostCapabilityPolicy.OrphanGrantCode);
        }

        [Fact]
        public void LyingEffectOrResourceMetadataIsRejectedBeforeHandlerInvocation()
        {
            UnityHostFixtureHandler.InvocationCount = 0;
            var irreversible = Copy(
                TouchDefinition(),
                sideEffect: SideEffectClass.ExternalIrreversible);
            var contract = Compose(HostContribution(irreversible));

            var issues = H1UnityHostCapabilityPolicy.Validate(
                contract,
                UnityProjectWorkspaceAuthority.ForArkusUnityProject(),
                new[] { Grant("unity.host.touch", UnityHostTimeClass.BoundedEditorEffect) });

            Assert.Contains(issues, issue => issue.Code == H1UnityHostCapabilityPolicy.MetadataMismatchCode);
            Assert.Throws<InvalidOperationException>(() => H1UnityHostCapabilityPolicy.CreateProjection(
                contract,
                UnityProjectWorkspaceAuthority.ForArkusUnityProject(),
                new[] { Grant("unity.host.touch", UnityHostTimeClass.BoundedEditorEffect) }));
            Assert.Equal(0, UnityHostFixtureHandler.InvocationCount);

            var wrongRoot = new UnityHostCapabilityGrant(
                new CapabilityKey("unity.host.touch", new ContractVersion(1, 0)),
                "caller.selected.root",
                ResourceNamespace,
                UnityHostResourceClass.ManagedAsset,
                UnityHostTimeClass.BoundedEditorEffect);
            var rootIssues = H1UnityHostCapabilityPolicy.Validate(
                Compose(HostContribution(TouchDefinition())),
                UnityProjectWorkspaceAuthority.ForArkusUnityProject(),
                new[] { wrongRoot });
            Assert.Contains(rootIssues, issue => issue.Code == H1UnityHostCapabilityPolicy.ResourceBoundaryCode);
        }

        [Fact]
        public void HostRequestSchemaRejectsAmbientAuthorityStructurallyNotByAPathWordList()
        {
            var unbounded = new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["resource"] = LogicalResource(),
                    ["opaqueSelector"] = SchemaNode.String()
                },
                new[] { "resource", "opaqueSelector" }));
            var contract = Compose(HostContribution(Copy(InspectDefinition(), requestSchema: unbounded)));

            var issues = H1UnityHostCapabilityPolicy.Validate(
                contract,
                UnityProjectWorkspaceAuthority.ForArkusUnityProject(),
                new[] { Grant("unity.host.inspect", UnityHostTimeClass.BoundedRead) });

            Assert.Contains(issues, issue => issue.Code == H1UnityHostCapabilityPolicy.AmbientAuthorityCode);

            var endpoint = new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["resource"] = LogicalResource(),
                    ["destination"] = SchemaNode.String(format: "uri")
                },
                new[] { "resource", "destination" }));
            var endpointIssues = H1UnityHostCapabilityPolicy.Validate(
                Compose(HostContribution(Copy(InspectDefinition(), requestSchema: endpoint))),
                UnityProjectWorkspaceAuthority.ForArkusUnityProject(),
                new[] { Grant("unity.host.inspect", UnityHostTimeClass.BoundedRead) });
            Assert.Contains(endpointIssues, issue => issue.Code == H1UnityHostCapabilityPolicy.AmbientAuthorityCode);
        }

        [Fact]
        public async Task ContentShapeProbeKeepsOrdinaryH0AndPotesShapedUnityAdmissionUsable()
        {
            FixtureEngineHandler.InvocationCount = 0;
            var composition = ContractComposer.Compose(
                BaseContract.CreateContribution(),
                new[] { Hk01TestFixtures.FixtureProvider(), HostContribution(InspectDefinition(), TouchDefinition()) });
            Assert.True(composition.Success, string.Join("; ", composition.Issues.Select(issue => issue.Code + ":" + issue.Message)));

            using var projection = H1UnityHostCapabilityPolicy.CreateProjection(
                composition.Contract!,
                UnityProjectWorkspaceAuthority.ForArkusUnityProject(),
                Grants());

            var ordinary = await projection.InvokeAsync(new NeutralProjectionRequest(
                "ordinary",
                "engine.observe",
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                Hk01TestFixtures.EmptyRequest()));
            var potesRead = await projection.InvokeAsync(Request(
                "potes-read",
                "unity.host.inspect",
                "asset.potes.market-stall"));
            var potesEffect = await projection.InvokeAsync(Request(
                "potes-effect",
                "unity.host.touch",
                "asset.potes.market-stall",
                "apply"));

            Assert.True(ordinary.Success);
            Assert.True(potesRead.Success);
            Assert.True(potesEffect.Success);
            Assert.Equal(1, FixtureEngineHandler.InvocationCount);
        }

        private static ComposedContract Compose(CanonicalProviderContribution host)
        {
            var composition = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { host });
            Assert.True(composition.Success, string.Join("; ", composition.Issues.Select(issue => issue.Code + ":" + issue.Message)));
            return composition.Contract!;
        }

        private static CanonicalProviderContribution HostContribution(params CapabilityDefinition[] definitions)
        {
            var routes = new List<CapabilityRoute>();
            foreach (var definition in definitions)
            {
                routes.Add(CapabilityRoute.FromHandler(
                    definition.Key.Name == "unity.host.touch"
                        ? (ICanonicalCapabilityHandler)new UnityHostTouchFixtureHandler()
                        : new UnityHostFixtureHandler()));
            }

            return new CanonicalProviderContribution(
                new ProviderDescriptor(
                    H1UnityHostCapabilityPolicy.HostProviderId,
                    ProviderKind.Scoped,
                    H1UnityHostCapabilityPolicy.HostScope,
                    new[] { H1UnityHostCapabilityPolicy.HostNamespace }),
                definitions,
                routes);
        }

        private static CapabilityDefinition InspectDefinition() => Definition(
            "unity.host.inspect",
            SideEffectClass.ReadOnly,
            new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["resource"] = LogicalResource()
                },
                new[] { "resource" })));

        private static CapabilityDefinition TouchDefinition() => Definition(
            "unity.host.touch",
            SideEffectClass.ExternalReversible,
            new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["resource"] = LogicalResource(),
                    ["mode"] = SchemaNode.String(new[] { "apply" })
                },
                new[] { "resource", "mode" })));

        private static CapabilityDefinition Definition(
            string name,
            SideEffectClass sideEffect,
            JsonSchemaDocument requestSchema)
        {
            return new CapabilityDefinition(
                new CapabilityKey(name, new ContractVersion(1, 0)),
                new ProviderMetadata(
                    H1UnityHostCapabilityPolicy.HostProviderId,
                    ProviderKind.Scoped,
                    H1UnityHostCapabilityPolicy.HostScope,
                    H1UnityHostCapabilityPolicy.HostNamespace),
                requestSchema,
                new JsonSchemaDocument(SchemaNode.Object(
                    new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                    {
                        ["value"] = SchemaNode.String(new[] { "ok" })
                    },
                    new[] { "value" })),
                CanonicalContractSchemas.StructuredError(),
                sideEffect,
                DeterminismClass.EnvironmentDependent,
                new[] { "unity-project-bootstrapped", "resource-in-reviewed-root" },
                new[] { "host-effect-bounded" },
                new ConcurrencySemantics(ConcurrencyClass.Serialized),
                new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Unsupported),
                new RepairSemantics(true, true),
                new PolicySemantics(
                    PrivilegeClass.Authoring,
                    TransactionRequirement.None,
                    ProvenanceRequirement.Required),
                new CostSemantics(1, "bounded H1 Editor host probe"));
        }

        private static CapabilityDefinition Copy(
            CapabilityDefinition source,
            SideEffectClass? sideEffect = null,
            JsonSchemaDocument? requestSchema = null)
        {
            return new CapabilityDefinition(
                source.Key,
                source.Provider,
                requestSchema ?? source.RequestSchema,
                source.SuccessSchema,
                source.ErrorSchema,
                sideEffect ?? source.SideEffect,
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

        private static SchemaNode LogicalResource() => SchemaNode.String(
            format: "arkus-logical-reference",
            logicalReferenceNamespace: ResourceNamespace);

        private static IReadOnlyList<UnityHostCapabilityGrant> Grants() => new[]
        {
            Grant("unity.host.inspect", UnityHostTimeClass.BoundedRead),
            Grant("unity.host.touch", UnityHostTimeClass.BoundedEditorEffect)
        };

        private static UnityHostCapabilityGrant Grant(string name, UnityHostTimeClass timeClass) =>
            new UnityHostCapabilityGrant(
                new CapabilityKey(name, new ContractVersion(1, 0)),
                UnityProjectWorkspaceAuthority.ManagedAssetsRootId,
                ResourceNamespace,
                UnityHostResourceClass.ManagedAsset,
                timeClass);

        private static NeutralProjectionRequest Request(
            string id,
            string capability,
            string resource,
            string? mode = null)
        {
            var arguments = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["resource"] = resource
            };
            if (mode != null) arguments["mode"] = mode;
            return new NeutralProjectionRequest(
                id,
                capability,
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                arguments);
        }
    }

    [PublicCapabilityRoute("arkus.unity-host", "unity.host.inspect", "1.0")]
    public sealed class UnityHostFixtureHandler : ICanonicalCapabilityHandler
    {
        public static int InvocationCount { get; set; }

        public CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            InvocationCount++;
            return CapabilityInvocationResult.Succeeded(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["value"] = "ok"
            });
        }
    }

    [PublicCapabilityRoute("arkus.unity-host", "unity.host.touch", "1.0")]
    public sealed class UnityHostTouchFixtureHandler : ICanonicalCapabilityHandler
    {
        public CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            UnityHostFixtureHandler.InvocationCount++;
            return CapabilityInvocationResult.Succeeded(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["value"] = "ok"
            });
        }
    }
}
