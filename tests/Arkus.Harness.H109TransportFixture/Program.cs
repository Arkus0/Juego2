using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Arkus.EngineBridge.UnityAuthoring;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.H1.UnityHost;
using Arkus.Harness.Cli;
using Arkus.Harness.Mcp;
using Arkus.Harness.Projection;
using Arkus.Harness.Runtime;
using ModelContextProtocol.Server;

namespace Arkus.Harness.H109TransportFixture
{
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            if (args.Length != 1 || (args[0] != "--jsonl" && args[0] != "--mcp")) return 64;
            using var projection = FixtureProjection.Create();
            if (args[0] == "--jsonl")
                return H1ReferenceTransportHost.RunWithProjection(Console.OpenStandardInput(), Console.OpenStandardOutput(), Console.Error, projection);

            var adapter = new McpProjectionAdapter(projection);
            var options = adapter.CreateServerOptions();
            await using var server = McpServer.Create(new StdioServerTransport(options), options);
            await server.RunAsync().ConfigureAwait(false);
            return 0;
        }
    }

    internal static class FixtureProjection
    {
        public static NeutralProjectionService Create()
        {
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var world = new PortableWorldAuthoringSession(new WorldState(
                new WorldId("world.h1-09.transport-fixture"), 0, Array.Empty<WorldObject>()));
            var worldReader = new ReadOnlyWorldStateView(world);
            var lease = new FixtureLease();
            var ledger = new FixtureLedger();
            var coordinator = new H1UnityEditorExecutionCoordinator(
                profile,
                new ValidReconciliationLauncher(worldReader, profile),
                lease,
                ledger,
                new IH1UnityCapabilityExecutor[]
                {
                    new ProjectProfileInspectExecutor(),
                    new HierarchyProbeExecutor(),
                    new H1CatalogueExecutor(H1CatalogueExecutor.QueryKey, H1CatalogueExecutor.QueryExecutorId, Path.Combine(profile.RepositoryRoot, H1CatalogueSnapshot.MappingRelativePath), Path.Combine(profile.RepositoryRoot, H1CatalogueSnapshot.AdoptionRelativePath)),
                    new H1CatalogueExecutor(H1CatalogueExecutor.GetKey, H1CatalogueExecutor.GetExecutorId, Path.Combine(profile.RepositoryRoot, H1CatalogueSnapshot.MappingRelativePath), Path.Combine(profile.RepositoryRoot, H1CatalogueSnapshot.AdoptionRelativePath)),
                    new H1CatalogueExecutor(H1CatalogueExecutor.ResolveKey, H1CatalogueExecutor.ResolveExecutorId, Path.Combine(profile.RepositoryRoot, H1CatalogueSnapshot.MappingRelativePath), Path.Combine(profile.RepositoryRoot, H1CatalogueSnapshot.AdoptionRelativePath)),
                    new H1ManagedSceneExecutor(H1ManagedSceneExecutor.MaterializeKey, H1ManagedSceneExecutor.MaterializeExecutorId, worldReader, profile),
                    new H1ManagedSceneExecutor(H1ManagedSceneExecutor.ObserveKey, H1ManagedSceneExecutor.ObserveExecutorId, worldReader, profile),
                    new H1ProjectionReconciliationExecutor(H1ProjectionReconciliationExecutor.DriftKey, worldReader, profile),
                    new H1ProjectionRematerializeExecutor(worldReader, profile),
                    new H1ProjectionReconciliationExecutor(H1ProjectionReconciliationExecutor.ImportProposalKey, worldReader, profile)
                });

            var composition = ContractComposer.Compose(
                CanonicalWorldContract.CreateContribution(new WorldInspectionService(world), world),
                new[]
                {
                    UnityAuthoringProvider.CreateContribution(),
                    H1ProjectionContract.PlanContribution(worldReader, profile),
                    H1ProjectionReconciliationComposition.CreateEditorHostContribution(coordinator),
                    H1UnityLifecycleContract.CreateLifecycleStatusContribution(ledger)
                });
            if (!composition.Success || composition.Contract == null)
                throw new InvalidOperationException("H1-09 fixture composition failed: " + string.Join(",", composition.Issues.Select(value => value.Code)));

            var projection = H1UnityHostCapabilityPolicy.CreateProjection(
                composition.Contract,
                UnityProjectWorkspaceAuthority.ForArkusUnityProject(),
                H1ProjectionReconciliationComposition.CreateGrants());
            coordinator.Bind(projection);
            return projection;
        }
    }

    internal sealed class ValidReconciliationLauncher : IH1UnityEditorWorkerLauncher
    {
        private static readonly JsonSerializerOptions Json = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        private readonly IWorldStateSource _world;
        private readonly H1UnityLaunchProfile _profile;

        public ValidReconciliationLauncher(IWorldStateSource world, H1UnityLaunchProfile profile)
        {
            _world = world;
            _profile = profile;
        }

        public H1UnityWorkerLaunchResult Launch(H1UnityInvocationEnvelope invocation, H1UnityLaunchProfile profile, InvocationResourceBudget executionBudget)
        {
            if (invocation.ExecutorId != H1ProjectionReconciliationExecutor.WorkerExecutorId)
                return H1UnityWorkerLaunchResult.Failure(H1UnityWorkerLaunchKind.CorruptResult, 0, true);

            var catalogue = H1CatalogueSnapshot.Build(
                File.ReadAllText(Path.Combine(_profile.RepositoryRoot, "Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json")),
                File.ReadAllText(Path.Combine(_profile.RepositoryRoot, H1CatalogueSnapshot.MappingRelativePath)),
                File.ReadAllText(Path.Combine(_profile.RepositoryRoot, H1CatalogueSnapshot.AdoptionRelativePath)));
            var plan = H1ManagedScenePlan.Build(_world.Current, catalogue);
            var nodes = Array.Empty<H1ObservedSceneNode>();
            var observation = new H1ProjectionReconciliationObservation
            {
                Active = true,
                GenerationId = new string('4', 32),
                InputDigest = plan.InputDigest,
                CanonicalHash = plan.CanonicalHash,
                CatalogueFingerprint = plan.CatalogueFingerprint,
                ManifestGraphDigest = "",
                ManifestRealizationDigest = "",
                GraphDigest = "",
                RealizationDigest = "",
                Nodes = nodes,
                UnmanagedPaths = Array.Empty<string>(),
                Diagnostics = Array.Empty<H1ProjectionObservationDiagnostic>(),
                ManagedDigest = H1ProjectionReconciliation.ManagedDigest(nodes)
            };
            var payload = JsonSerializer.Serialize(new H1ProjectionReconciliationWorkerReply
            {
                SchemaId = "arkus.h1-projection-reconciliation-worker-result@1",
                SceneLogicalId = H1ManagedScenePlan.SceneId,
                ErrorCode = "",
                Observation = observation
            }, Json);
            return H1UnityWorkerLaunchResult.Completed(new H1UnityResultEnvelope(
                invocation.InvocationId,
                invocation.Capability,
                invocation.ExecutorId,
                profile.Id,
                profile.ProjectIdentity,
                profile.EffectiveEditorVersion,
                profile.EffectiveEditorRevision,
                true,
                payload));
        }
    }

    internal sealed class FixtureLease : IH1UnityProjectLease
    {
        public IDisposable? TryAcquire() => new LeaseHandle();
        private sealed class LeaseHandle : IDisposable { public void Dispose() { } }
    }

    internal sealed class FixtureLedger : IH1UnityInvocationLedger
    {
        private readonly Dictionary<string, H1UnityInvocationRecord> _records = new Dictionary<string, H1UnityInvocationRecord>(StringComparer.Ordinal);
        public void Record(H1UnityInvocationRecord record) => _records[record.InvocationId] = record;
        public bool TryRead(string invocationId, out H1UnityInvocationRecord? record)
        {
            var found = _records.TryGetValue(invocationId, out var value);
            record = value;
            return found;
        }
    }
}
