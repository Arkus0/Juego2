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
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using ModelContextProtocol.Server;

namespace Arkus.Harness.H109TransportFixture
{
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            if (args.Length != 1) return 64;
            if (args[0] == "--compose-seed") return ComposedProof.Seed();
            if (args[0] == "--compose-apply") return ComposedProof.Apply();
            if (args[0] == "--compose-verify") return ComposedProof.Verify();
            if (args[0] != "--jsonl" && args[0] != "--mcp") return 64;

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

    internal static class ComposedProof
    {
        private static readonly JsonSerializerOptions Json = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public static int Seed()
        {
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var catalogue = Catalogue(profile);
            var original = OriginalWorld(catalogue);
            var plan = H1ManagedScenePlan.Build(original, catalogue);
            Require(plan.Nodes.Length == 2, "seed must project exactly facade and workshop");
            var directory = DirectoryPath(profile);
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
            Directory.CreateDirectory(directory);
            Write(Path.Combine(directory, "initial-plan.json"), plan);
            Console.WriteLine("H1_09_COMPOSED_SEED_GREEN revision=" + plan.WorldRevision + " input=" + plan.InputDigest);
            return 0;
        }

        public static int Apply()
        {
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var directory = DirectoryPath(profile);
            var driftPath = Path.Combine(directory, "drift-worker-reply.json");
            Require(File.Exists(driftPath), "effective Unity drift reply is missing");
            var worker = JsonSerializer.Deserialize<H1ProjectionReconciliationWorkerReply>(File.ReadAllText(driftPath), Json);
            Require(worker != null && worker.ErrorCode.Length == 0 && worker.Observation != null, "effective Unity drift reply is not valid");

            var catalogue = Catalogue(profile);
            var original = OriginalWorld(catalogue);
            var expected = H1ManagedScenePlan.Build(original, catalogue);
            var report = H1ProjectionReconciliation.Compare(expected, worker!.Observation!);
            Require(!report.Parity && report.State == "engine-drift", "effective Unity edit did not produce supported engine drift");
            var facadeDrift = report.Items.SingleOrDefault(value => value.ObjectId == "facade" && value.Classification == H1ProjectionDriftClass.Changed);
            Require(facadeDrift != null && facadeDrift.Fields.Contains("transform", StringComparer.Ordinal), "facade transform drift is absent");

            var proposal = H1ProjectionReconciliation.CompileProposal(expected, worker.Observation!, report, catalogue);
            Require(proposal.Available && proposal.MutationRequest != null, "supported effective drift did not compile a public H0 proposal");
            Require(proposal.ObjectIds.SequenceEqual(new[] { "facade" }, StringComparer.Ordinal), "proposal scope is not exactly facade");

            var session = new PortableWorldAuthoringSession(original);
            var contract = CanonicalWorldContract.Compose(new WorldInspectionService(session), session);
            var concurrent = MutationRequest(original, "request.h1-09.composed.concurrent", new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = "put-object",
                ["id"] = "observer",
                ["typeId"] = "fixture.concurrent"
            });
            RequireSuccess(contract.Dispatch(WorldMutationContract.ApplyName, Exact(), concurrent), "concurrent canonical write");
            Require(session.Current.Revision == original.Revision + 1, "concurrent write did not advance the canonical revision");

            var stale = contract.Dispatch(WorldMutationContract.PlanName, Exact(), proposal.MutationRequest!);
            Require(!stale.Success && stale.Error != null &&
                (stale.Error.MachineCode == "world.change.stale_revision" || stale.Error.MachineCode == "world.change.stale_hash"),
                "stale H1 proposal did not fail through accepted H0 CAS");
            Require(stale.Error!.Context.TryGetValue("recovery", out var rawRecovery), "stale H0 result omitted HK08B recovery");
            var recovery = rawRecovery as IReadOnlyDictionary<string, object?>;
            Require(recovery != null && Equals(recovery["disposition"], WorldConflictRecoveryContract.SameLineageReplan),
                "stale H1 proposal did not expose same-lineage-replan");

            var recovered = new Dictionary<string, object?>(proposal.MutationRequest!, StringComparer.Ordinal)
            {
                ["expectedRevision"] = session.Current.Revision,
                ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(session.Current)
            };
            RequireSuccess(contract.Dispatch(WorldMutationContract.PlanName, Exact(), recovered), "recovered H0 plan");
            RequireSuccess(contract.Dispatch(WorldMutationContract.DryRunName, Exact(), recovered), "recovered H0 dry-run");
            var applied = contract.Dispatch(WorldMutationContract.ApplyName, Exact(), recovered);
            RequireSuccess(applied, "recovered H0 apply");
            Require(session.Current.Revision == original.Revision + 2, "accepted H0 apply did not advance exactly once");
            Require(session.Current.Objects.Any(value => value.Id.Value == "observer"), "recovery overwrote the concurrent canonical writer");

            var rebuilt = H1ManagedScenePlan.Build(session.Current, catalogue);
            var effectiveFacade = worker.Observation!.Nodes.Single(value => value.ObjectId == "facade");
            var rebuiltFacade = rebuilt.Nodes.Single(value => value.ObjectId == "facade");
            Require(rebuiltFacade.PositionMm.X == effectiveFacade.PositionMm.X &&
                    rebuiltFacade.PositionMm.Y == effectiveFacade.PositionMm.Y &&
                    rebuiltFacade.PositionMm.Z == effectiveFacade.PositionMm.Z,
                "accepted H0 state did not rebuild the effective Unity transform");
            Require(rebuilt.CanonicalHash == CanonicalWorldStateCodec.ComputeContentHash(session.Current), "rebuilt H1 plan lost canonical provenance");
            Write(Path.Combine(directory, "applied-plan.json"), rebuilt);
            Console.WriteLine("H1_09_COMPOSED_APPLY_GREEN stale=" + stale.Error.MachineCode + " revision=" + rebuilt.WorldRevision + " input=" + rebuilt.InputDigest);
            return 0;
        }

        public static int Verify()
        {
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var directory = DirectoryPath(profile);
            var planPath = Path.Combine(directory, "applied-plan.json");
            var finalPath = Path.Combine(directory, "final-worker-reply.json");
            Require(File.Exists(planPath) && File.Exists(finalPath), "rematerialized plan/final observation evidence is missing");
            var plan = JsonSerializer.Deserialize<H1ManagedScenePlan>(File.ReadAllText(planPath), Json);
            var worker = JsonSerializer.Deserialize<H1ProjectionReconciliationWorkerReply>(File.ReadAllText(finalPath), Json);
            Require(plan != null && worker != null && worker.ErrorCode.Length == 0 && worker.Observation != null, "final composed evidence is malformed");
            var parity = H1ProjectionReconciliation.Compare(plan!, worker!.Observation!);
            Require(parity.Parity && parity.State == "in-sync" && parity.Items.Length == 0,
                "canonical rematerialization did not converge to effective parity: " + parity.State + "/" + string.Join(",", parity.Items.Select(value => value.Classification + ":" + value.ObjectId)));
            Console.WriteLine("H1_09_COMPOSED_PARITY_GREEN revision=" + plan!.WorldRevision + " managed=" + parity.ManagedDigest);
            return 0;
        }

        private static WorldState OriginalWorld(H1CatalogueSnapshot catalogue)
        {
            var source = catalogue.Entries.First(value => value.Kind == "prefab");
            return new WorldState(
                new WorldId("world.h1-09.composed"),
                7,
                new[]
                {
                    new WorldObject(new WorldObjectId("facade"), new WorldTypeId("fixture.facade")),
                    new WorldObject(new WorldObjectId("workshop"), new WorldTypeId("fixture.workshop"))
                },
                new[]
                {
                    Binding("facade", source.LogicalId, 100, 0, 0),
                    Binding("workshop", source.LogicalId, 900, 0, 0)
                });
        }

        private static WorldExtensionData Binding(string subject, string sourceLogicalId, long x, long y, long z)
        {
            var binding = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = UnityBindingProducer.BindingSchemaId,
                ["targetSceneId"] = H1ManagedScenePlan.SceneId,
                ["source"] = new Dictionary<string, object?>(StringComparer.Ordinal) { ["kind"] = "prefab", ["logicalId"] = sourceLogicalId },
                ["transform"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["coordinateConvention"] = UnityBindingProducer.CoordinateConvention,
                    ["positionMm"] = Vector(x, y, z),
                    ["rotationMilliDegrees"] = Vector(0, 0, 0),
                    ["scalePpm"] = Vector(1000000, 1000000, 1000000)
                },
                ["components"] = Array.Empty<object?>()
            };
            var compiled = UnityBindingProducer.Compile(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["subjectId"] = subject,
                ["binding"] = binding
            });
            return new WorldExtensionData(
                UnityBindingProducer.ExtensionOwner,
                UnityBindingProducer.ExtensionSchemaVersion,
                Convert.FromBase64String((string)compiled["payloadBase64"]!),
                new WorldObjectId(subject),
                Array.Empty<WorldReference>());
        }

        private static IReadOnlyDictionary<string, object?> MutationRequest(WorldState state, string key, IReadOnlyDictionary<string, object?> operation)
        {
            return new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["idempotencyKey"] = key,
                ["expectedRevision"] = state.Revision,
                ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(state),
                ["operations"] = new object?[] { operation }
            });
        }

        private static Dictionary<string, object?> Vector(long x, long y, long z) => new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["x"] = x, ["y"] = y, ["z"] = z
        };

        private static ContractVersionRange Exact() => ContractVersionRange.Exact(new ContractVersion(1, 0));

        private static void RequireSuccess(CapabilityInvocationResult result, string phase)
        {
            Require(result.Success, phase + " failed: " + (result.Error == null ? "unknown" : result.Error.MachineCode));
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("H1-09 composed proof: " + message);
        }

        private static H1CatalogueSnapshot Catalogue(H1UnityLaunchProfile profile) => H1CatalogueSnapshot.Build(
            File.ReadAllText(Path.Combine(profile.RepositoryRoot, "Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json")),
            File.ReadAllText(Path.Combine(profile.RepositoryRoot, H1CatalogueSnapshot.MappingRelativePath)),
            File.ReadAllText(Path.Combine(profile.RepositoryRoot, H1CatalogueSnapshot.AdoptionRelativePath)));

        private static string DirectoryPath(H1UnityLaunchProfile profile) => Path.Combine(profile.ProjectRoot, "H1-09-Composition");
        private static void Write<T>(string path, T value) => File.WriteAllText(path, JsonSerializer.Serialize(value, Json));
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
