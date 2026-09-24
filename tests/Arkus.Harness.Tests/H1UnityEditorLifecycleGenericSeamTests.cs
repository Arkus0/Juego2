using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Arkus.EngineBridge.UnityAuthoring;
using Arkus.H1.UnityHost;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1UnityEditorLifecycleGenericSeamTests
    {
        private static readonly CapabilityKey SyntheticKey =
            new CapabilityKey("unity.host.synthetic.inspect", new ContractVersion(1, 0));

        [Fact]
        public async Task Synthetic_admitted_capability_traverses_generic_seam_without_adapter_edit_and_absent_or_ungranted_capability_cannot_launch()
        {
            var launcher = new FakeLauncher(invocation => H1UnityWorkerLaunchResult.Completed(Matching(invocation)));
            var ledger = new MemoryLedger();
            var coordinator = Coordinator(launcher, ledger, includeSynthetic: true);
            var composition = Compose(coordinator, ledger, includeSynthetic: true);
            Assert.True(composition.Success, composition.Issues.Count == 0 ? "unknown composition failure" : composition.Issues[0].Code);

            var grants = new List<UnityHostCapabilityGrant>(H1UnityLifecycleContract.CreateGrants())
            {
                SyntheticGrant()
            };
            using var projection = H1UnityHostCapabilityPolicy.CreateProjection(
                composition.Contract!,
                UnityProjectWorkspaceAuthority.ForArkusUnityProject(),
                grants);
            coordinator.Bind(projection);

            var synthetic = await projection.InvokeAsync(Request(SyntheticKey.Name));
            Assert.True(synthetic.Success);
            Assert.Equal("arkus.h1-unity-synthetic-probe@1", synthetic.Result!["schemaId"]);
            Assert.Equal("synthetic:v1", synthetic.Result["value"]);
            Assert.Equal(1, launcher.Count);

            var absent = await projection.InvokeAsync(Request("unity.host.adapter-only.inspect"));
            Assert.False(absent.Success);
            Assert.Equal("contract.unknown_capability", absent.Error!.MachineCode);
            Assert.Equal(1, launcher.Count);

            var missingGrant = Assert.Throws<InvalidOperationException>(() =>
                H1UnityHostCapabilityPolicy.CreateProjection(
                    composition.Contract!,
                    UnityProjectWorkspaceAuthority.ForArkusUnityProject(),
                    H1UnityLifecycleContract.CreateGrants()));
            Assert.Contains(H1UnityHostCapabilityPolicy.MissingGrantCode, missingGrant.Message, StringComparison.Ordinal);
            Assert.Equal(1, launcher.Count);
        }

        [Fact]
        public async Task Raw_launcher_exception_text_never_becomes_public_error_contract()
        {
            const string raw = "RAW_UNITY_PROCESS_SECRET_SHOULD_NOT_ESCAPE";
            var launcher = new FakeLauncher(_ => throw new InvalidOperationException(raw));
            var ledger = new MemoryLedger();
            var coordinator = Coordinator(launcher, ledger, includeSynthetic: false);
            var composition = Compose(coordinator, ledger, includeSynthetic: false);
            Assert.True(composition.Success, composition.Issues.Count == 0 ? "unknown composition failure" : composition.Issues[0].Code);
            using var projection = H1UnityHostCapabilityPolicy.CreateProjection(
                composition.Contract!,
                UnityProjectWorkspaceAuthority.ForArkusUnityProject(),
                H1UnityLifecycleContract.CreateGrants());
            coordinator.Bind(projection);

            var outcome = await projection.InvokeAsync(Request(ProjectProfileInspectExecutor.Key.Name));

            Assert.False(outcome.Success);
            Assert.Equal(H1UnityEditorExecutionCoordinator.IndeterminateCode, outcome.Error!.MachineCode);
            Assert.DoesNotContain(raw, outcome.Error.Message, StringComparison.Ordinal);
            Assert.DoesNotContain(raw, System.Text.Json.JsonSerializer.Serialize(outcome.ToData()), StringComparison.Ordinal);
        }

        private static H1UnityEditorExecutionCoordinator Coordinator(
            FakeLauncher launcher,
            IH1UnityInvocationLedger ledger,
            bool includeSynthetic)
        {
            var executors = new List<IH1UnityCapabilityExecutor>
            {
                new ProjectProfileInspectExecutor(),
                new HierarchyProbeExecutor()
            };
            if (includeSynthetic) executors.Add(new SyntheticExecutor());
            return new H1UnityEditorExecutionCoordinator(
                H1UnityLaunchProfile.ForCurrentHost(),
                launcher,
                new AvailableLease(),
                ledger,
                executors);
        }

        private static ContractCompositionResult Compose(
            H1UnityEditorExecutionCoordinator coordinator,
            IH1UnityInvocationLedger ledger,
            bool includeSynthetic)
        {
            var editor = H1UnityLifecycleContract.CreateEditorHostContribution(coordinator);
            CanonicalProviderContribution editorContribution = editor;
            if (includeSynthetic)
            {
                var definitions = new List<CapabilityDefinition>(editor.Definitions) { SyntheticDefinition() };
                var routes = new List<CapabilityRoute>(editor.Routes)
                {
                    CapabilityRoute.FromHandler(new SyntheticHandler(coordinator))
                };
                editorContribution = new CanonicalProviderContribution(editor.Descriptor, definitions, routes);
            }

            return ContractComposer.Compose(
                CanonicalWorldContract.CreateEmptyPortableSessionContribution(ProductionHarnessHost.InitialWorldId),
                new[]
                {
                    UnityAuthoringProvider.CreateContribution(),
                    editorContribution,
                    H1UnityLifecycleContract.CreateLifecycleStatusContribution(ledger)
                });
        }

        private static CapabilityDefinition SyntheticDefinition() =>
            new CapabilityDefinition(
                SyntheticKey,
                new ProviderMetadata(
                    H1UnityHostCapabilityPolicy.HostProviderId,
                    ProviderKind.Scoped,
                    H1UnityHostCapabilityPolicy.HostScope,
                    H1UnityHostCapabilityPolicy.HostNamespace),
                CanonicalContractSchemas.EmptyObject(),
                new JsonSchemaDocument(SchemaNode.Object(
                    new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                    {
                        ["schemaId"] = SchemaNode.String(new[] { "arkus.h1-unity-synthetic-probe@1" }),
                        ["value"] = SchemaNode.String(new[] { "synthetic:v1" })
                    },
                    new[] { "schemaId", "value" })),
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.ReadOnly,
                DeterminismClass.EnvironmentDependent,
                new[] { "h1-unity-profile-bound", "project-operation-lease-available" },
                new[] { "synthetic-worker-result-returned" },
                new ConcurrencySemantics(ConcurrencyClass.Serialized),
                new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Unsupported),
                new RepairSemantics(true, true),
                new PolicySemantics(PrivilegeClass.Authoring, TransactionRequirement.None, ProvenanceRequirement.Required),
                new CostSemantics(1, "test-scoped generic Unity lifecycle seam probe"));

        private static UnityHostCapabilityGrant SyntheticGrant() =>
            new UnityHostCapabilityGrant(
                SyntheticKey,
                UnityProjectWorkspaceAuthority.ProjectSettingsRootId,
                "ref.arkus.unity-host.synthetic-project-metadata",
                UnityHostResourceClass.ProjectMetadata,
                UnityHostTimeClass.BoundedRead);

        private static NeutralProjectionRequest Request(string capability) =>
            new NeutralProjectionRequest(
                "h1-03a-generic-" + Guid.NewGuid().ToString("N"),
                capability,
                new ContractVersionRange(1, 0, 0),
                new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)),
                1000);

        private static H1UnityResultEnvelope Matching(H1UnityInvocationEnvelope invocation) =>
            new H1UnityResultEnvelope(
                invocation.InvocationId,
                invocation.Capability,
                invocation.ExecutorId,
                invocation.ProfileId,
                invocation.ProjectIdentity,
                invocation.EditorVersion,
                invocation.EditorRevision,
                true,
                invocation.Payload);

        [PublicCapabilityRoute(H1UnityHostCapabilityPolicy.HostProviderId, "unity.host.synthetic.inspect", "1.0")]
        private sealed class SyntheticHandler : ICanonicalCapabilityHandler
        {
            private readonly H1UnityEditorExecutionCoordinator _coordinator;
            public SyntheticHandler(H1UnityEditorExecutionCoordinator coordinator) => _coordinator = coordinator;
            public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request) =>
                _coordinator.Invoke(context, request);
        }

        private sealed class SyntheticExecutor : IH1UnityCapabilityExecutor
        {
            public CapabilityKey Capability => SyntheticKey;
            public string ExecutorId => "arkus.h1.worker.synthetic.inspect@1";
            public string EncodeRequest(IReadOnlyDictionary<string, object?> request) =>
                request.Count == 0 ? "synthetic:v1" : throw new InvalidOperationException("Synthetic probe accepts no public selectors.");
            public CapabilityInvocationResult DecodeResult(H1UnityResultEnvelope result)
            {
                if (!string.Equals(result.Payload, "synthetic:v1", StringComparison.Ordinal))
                    throw new FormatException("Synthetic worker payload mismatch.");
                return CapabilityInvocationResult.Succeeded(
                    new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["schemaId"] = "arkus.h1-unity-synthetic-probe@1",
                        ["value"] = "synthetic:v1"
                    }));
            }
        }

        private sealed class FakeLauncher : IH1UnityEditorWorkerLauncher
        {
            private readonly Func<H1UnityInvocationEnvelope, H1UnityWorkerLaunchResult> _launch;
            public FakeLauncher(Func<H1UnityInvocationEnvelope, H1UnityWorkerLaunchResult> launch) => _launch = launch;
            public int Count { get; private set; }
            public H1UnityWorkerLaunchResult Launch(
                H1UnityInvocationEnvelope invocation,
                H1UnityLaunchProfile profile,
                InvocationResourceBudget executionBudget)
            {
                Count++;
                return _launch(invocation);
            }
        }

        private sealed class AvailableLease : IH1UnityProjectLease
        {
            public IDisposable? TryAcquire() => new Releaser();
        }

        private sealed class Releaser : IDisposable
        {
            public void Dispose() { }
        }

        private sealed class MemoryLedger : IH1UnityInvocationLedger
        {
            private readonly Dictionary<string, H1UnityInvocationRecord> _records =
                new Dictionary<string, H1UnityInvocationRecord>(StringComparer.Ordinal);
            public void Record(H1UnityInvocationRecord record) => _records[record.InvocationId] = record;
            public bool TryRead(string invocationId, out H1UnityInvocationRecord? record)
            {
                if (_records.TryGetValue(invocationId, out var found))
                {
                    record = found;
                    return true;
                }
                record = null;
                return false;
            }
        }
    }
}
