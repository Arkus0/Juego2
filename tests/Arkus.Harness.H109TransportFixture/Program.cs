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
            if (args[0] == "--content-drift-verify") return ComposedProof.VerifyReferencedContentDrift();
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
            var driftRaw = File.ReadAllText(driftPath);
            var worker = JsonSerializer.Deserialize<H1ProjectionReconciliationWorkerReply>(driftRaw, Json);
            Require(worker != null && worker.ErrorCode.Length == 0 && worker.Observation != null, "effective Unity drift reply is not valid");

            var catalogue = Catalogue(profile);
            var original = OriginalWorld(catalogue);
            var publicProposal = InvokePublic(original, driftRaw, "unity.host.projection.import-proposal", "h1-09.composed.import-proposal");
            Require(Bool(publicProposal, "available"), "public import-proposal did not expose an importable supported Unity edit");
            var mutationRequest = AsDictionary(publicProposal["mutationRequest"], "public import-proposal mutationRequest");
            var objectIds = Strings(publicProposal["objectIds"]);
            Require(objectIds.SequenceEqual(new[] { "facade" }, StringComparer.Ordinal), "public proposal scope is not exactly facade");
            var publicDrift = AsDictionary(publicProposal["drift"], "public import-proposal drift");
            Require(Text(publicDrift, "state") == "engine-drift" && !Bool(publicDrift, "parity"),
                "public import-proposal did not carry the supported engine-drift report");
            Console.WriteLine("H1_09_PUBLIC_IMPORT_PROPOSAL_GREEN object=facade");

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

            var stale = contract.Dispatch(WorldMutationContract.PlanName, Exact(), mutationRequest);
            Require(!stale.Success && stale.Error != null &&
                (stale.Error.MachineCode == "world.change.stale_revision" || stale.Error.MachineCode == "world.change.stale_hash"),
                "stale H1 proposal did not fail through accepted H0 CAS");
            Require(stale.Error!.Context.TryGetValue("recovery", out var rawRecovery), "stale H0 result omitted HK08B recovery");
            var recovery = rawRecovery as IReadOnlyDictionary<string, object?>;
            Require(recovery != null && Equals(recovery["disposition"], WorldConflictRecoveryContract.SameLineageReplan),
                "stale H1 proposal did not expose same-lineage-replan");

            var recovered = new Dictionary<string, object?>(mutationRequest, StringComparer.Ordinal)
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
            var effectiveFacade = worker!.Observation!.Nodes.Single(value => value.ObjectId == "facade");
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

        public static int VerifyReferencedContentDrift()
        {
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var path = Path.Combine(DirectoryPath(profile), "content-drift-worker-reply.json");
            Require(File.Exists(path), "referenced-content drift worker reply is missing");
            var payload = File.ReadAllText(path);
            var catalogue = Catalogue(profile);
            var original = OriginalWorld(catalogue);

            var drift = InvokePublic(original, payload, "unity.host.projection.drift", "h1-09.content-drift.drift");
            Require(Text(drift, "state") == "ambiguous" && !Bool(drift, "parity"),
                "referenced-content drift reached false parity through the public drift capability");
            var codes = DiagnosticCodes(drift);
            Require(codes.Contains("projection.component-reference-content-drift", StringComparer.Ordinal),
                "public drift report omitted projection.component-reference-content-drift");
            Require(codes.Contains("projection.reconciliation-node-unreadable", StringComparer.Ordinal),
                "public drift report omitted projection.reconciliation-node-unreadable");

            var proposal = InvokePublic(original, payload, "unity.host.projection.import-proposal", "h1-09.content-drift.import-proposal");
            Require(!Bool(proposal, "available"), "referenced-content drift produced an importable public proposal");
            Require(proposal.TryGetValue("mutationRequest", out var request) && request == null,
                "referenced-content drift produced a public H0 mutation request");
            var proposalDrift = AsDictionary(proposal["drift"], "referenced-content public proposal drift");
            Require(Text(proposalDrift, "state") == "ambiguous" && !Bool(proposalDrift, "parity"),
                "public import-proposal lost referenced-content ambiguity");
            Console.WriteLine("H1_09_REFERENCED_CONTENT_DRIFT_GREEN diagnostic=projection.component-reference-content-drift proposal=blocked");
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

        internal static WorldState OriginalWorld(H1CatalogueSnapshot catalogue)
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

        private static IReadOnlyDictionary<string, object?> InvokePublic(WorldState world, string workerPayload, string capability, string requestId)
        {
            var session = new PortableWorldAuthoringSession(world);
            using var projection = FixtureProjection.Create(session, new FixedReconciliationLauncher(workerPayload));
            var outcome = projection.InvokeAsync(new NeutralProjectionRequest(
                requestId,
                capability,
                Exact(),
                new Dictionary<string, object?>(StringComparer.Ordinal) { ["sceneLogicalId"] = H1ManagedScenePlan.SceneId }))
                .GetAwaiter().GetResult();
            Require(outcome.Success && outcome.Result != null,
                "public " + capability + " failed: " + (outcome.Error == null ? "unknown" : outcome.Error.MachineCode));
            return outcome.Result!;
        }

        private static IReadOnlyDictionary<string, object?> AsDictionary(object? value, string subject)
        {
            var dictionary = value as IReadOnlyDictionary<string, object?>;
            Require(dictionary != null, subject + " is not a dictionary");
            return dictionary!;
        }

        private static string[] Strings(object? value)
        {
            if (value is IEnumerable<object?> objects)
                return objects.Select(item => Convert.ToString(item, System.Globalization.CultureInfo.InvariantCulture) ?? "").ToArray();
            if (value is IEnumerable<string> strings) return strings.ToArray();
            throw new InvalidOperationException("H1-09 composed proof: public string array has an unexpected shape");
        }

        private static string[] DiagnosticCodes(IReadOnlyDictionary<string, object?> report)
        {
            if (!report.TryGetValue("diagnostics", out var value) || !(value is IEnumerable<object?> rows))
                throw new InvalidOperationException("H1-09 composed proof: public diagnostics have an unexpected shape");
            return rows.Select(row => Text(AsDictionary(row, "public diagnostic"), "code")).ToArray();
        }

        private static string Text(IReadOnlyDictionary<string, object?> data, string key) =>
            data.TryGetValue(key, out var value) ? Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture) ?? "" : "";
        private static bool Bool(IReadOnlyDictionary<string, object?> data, string key) =>
            data.TryGetValue(key, out var value) && value != null && Convert.ToBoolean(value, System.Globalization.CultureInfo.InvariantCulture);

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

        internal static H1CatalogueSnapshot Catalogue(H1UnityLaunchProfile profile) => H1CatalogueSnapshot.Build(
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
            var catalogue = ComposedProof.Catalogue(profile);
            var world = new PortableWorldAuthoringSession(ComposedProof.OriginalWorld(catalogue));
            var worldReader = new ReadOnlyWorldStateView(world);
            return Create(profile, world, worldReader, new ValidReconciliationLauncher(worldReader, profile));
        }

        public static NeutralProjectionService Create(PortableWorldAuthoringSession world, IH1UnityEditorWorkerLauncher launcher)
        {
            if (world == null) throw new ArgumentNullException(nameof(world));
            if (launcher == null) throw new ArgumentNullException(nameof(launcher));
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var worldReader = new ReadOnlyWorldStateView(world);
            return Create(profile, world, worldReader, launcher);
        }

        private static NeutralProjectionService Create(
            H1UnityLaunchProfile profile,
            PortableWorldAuthoringSession world,
            ReadOnlyWorldStateView worldReader,
            IH1UnityEditorWorkerLauncher launcher)
        {
            var lease = new FixtureLease();
            var ledger = new FixtureLedger();
            var coordinator = new H1UnityEditorExecutionCoordinator(
                profile,
                launcher,
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

            var catalogue = ComposedProof.Catalogue(_profile);
            var plan = H1ManagedScenePlan.Build(_world.Current, catalogue);
            var nodes = plan.Nodes.Select(Observed).ToArray();
            var facadePlan = plan.Nodes.Single(value => value.ObjectId == "facade");
            var facade = nodes.Single(value => value.ObjectId == "facade");
            facade.PositionMm.X += 375;
            facade.ComponentRows = Rows(facadePlan, facade.PositionMm);
            facade.ComponentDigest = H1ManagedScenePlan.Sha(string.Join("\n", facade.ComponentRows));
            var observation = new H1ProjectionReconciliationObservation
            {
                Active = true,
                GenerationId = new string('4', 32),
                InputDigest = plan.InputDigest,
                CanonicalHash = plan.CanonicalHash,
                CatalogueFingerprint = plan.CatalogueFingerprint,
                ManifestGraphDigest = H1ManagedScenePlan.Sha("h1-09-valid-transport-graph"),
                ManifestRealizationDigest = H1ManagedScenePlan.Sha("h1-09-valid-transport-realization"),
                GraphDigest = H1ManagedScenePlan.Sha("h1-09-valid-transport-graph"),
                RealizationDigest = H1ManagedScenePlan.Sha("h1-09-valid-transport-realization"),
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
            return Completed(invocation, profile, payload);
        }

        private static H1ObservedSceneNode Observed(H1ManagedSceneNode node)
        {
            var rows = Rows(node, node.PositionMm);
            return new H1ObservedSceneNode
            {
                ObjectId = node.ObjectId,
                ParentObjectId = node.ParentObjectId,
                SourceLogicalId = node.SourceLogicalId,
                SourceKind = node.SourceKind,
                SourcePath = node.SourcePath,
                SourceGuid = node.SourceGuid,
                SourceLocalFileId = node.SourceLocalFileId,
                SourceContentSha256 = node.SourceContentSha256,
                RealizationKind = node.SourceKind == "prefab" ? "managed-prefab-variant" : "source-asset",
                RealizedPath = node.SourcePath,
                RealizedGuid = node.SourceGuid,
                RealizedLocalFileId = node.SourceLocalFileId,
                PrefabGenerationId = node.SourceKind == "prefab" ? new string('2', 32) : "",
                RelationshipDigest = H1ManagedScenePlan.Sha("relationships-" + node.ObjectId),
                Relationships = Array.Empty<H1ObservedPrefabRelationship>(),
                PositionMm = Copy(node.PositionMm),
                RotationMilliDegrees = Copy(node.RotationMilliDegrees),
                ScalePpm = Copy(node.ScalePpm),
                ComponentRows = rows,
                ComponentDigest = H1ManagedScenePlan.Sha(string.Join("\n", rows))
            };
        }

        private static string[] Rows(H1ManagedSceneNode node, H1ProjectionVector position) => new[]
        {
            H1ComponentSchemas.Transform + "|positionMm=" + Vec(position) + "|rotationMilliDegrees=" + VecNormalized(node.RotationMilliDegrees) + "|scalePpm=" + Vec(node.ScalePpm)
        };

        private static H1ProjectionVector Copy(H1ProjectionVector value) => new H1ProjectionVector { X = value.X, Y = value.Y, Z = value.Z };
        private static string Vec(H1ProjectionVector value) => value.X + "," + value.Y + "," + value.Z;
        private static string VecNormalized(H1ProjectionVector value) => Normalize(value.X) + "," + Normalize(value.Y) + "," + Normalize(value.Z);
        private static long Normalize(long value) { var result = value % 360000; return result < 0 ? result + 360000 : result; }

        private static H1UnityWorkerLaunchResult Completed(H1UnityInvocationEnvelope invocation, H1UnityLaunchProfile profile, string payload) =>
            H1UnityWorkerLaunchResult.Completed(new H1UnityResultEnvelope(
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

    internal sealed class FixedReconciliationLauncher : IH1UnityEditorWorkerLauncher
    {
        private readonly string _payload;
        public FixedReconciliationLauncher(string payload) => _payload = payload ?? throw new ArgumentNullException(nameof(payload));

        public H1UnityWorkerLaunchResult Launch(H1UnityInvocationEnvelope invocation, H1UnityLaunchProfile profile, InvocationResourceBudget executionBudget)
        {
            if (invocation.ExecutorId != H1ProjectionReconciliationExecutor.WorkerExecutorId)
                return H1UnityWorkerLaunchResult.Failure(H1UnityWorkerLaunchKind.CorruptResult, 0, true);
            return H1UnityWorkerLaunchResult.Completed(new H1UnityResultEnvelope(
                invocation.InvocationId,
                invocation.Capability,
                invocation.ExecutorId,
                profile.Id,
                profile.ProjectIdentity,
                profile.EffectiveEditorVersion,
                profile.EffectiveEditorRevision,
                true,
                _payload));
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
