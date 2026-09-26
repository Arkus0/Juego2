using System;
using System.IO;
using Arkus.EngineBridge.UnityAuthoring;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Projection;
using Arkus.Harness.Runtime;

namespace Arkus.H1.UnityHost
{
    /// <summary>
    /// H1-10 production composition. The Editor coordinator is first bound against the editor-only
    /// H1 surface; host-local checkpoint handlers are then added to the public projection. This keeps
    /// one public contract while preserving the accepted coordinator invariant that every capability
    /// it binds has a real Unity worker executor.
    /// </summary>
    public static class ProductionH1ProjectCheckpointHost
    {
        public static NeutralProjectionService Create()
        {
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var world = new PortableWorldAuthoringSession(new WorldState(
                new WorldId(ProductionHarnessHost.InitialWorldId), 0, Array.Empty<WorldObject>()),
                UnityAuthoringProvider.CreateDocumentCodecs());
            var worldReader = new ReadOnlyWorldStateView(world);
            var store = new H1ProjectCheckpointStore(profile);
            var projectLease = new FileH1UnityProjectLease(profile);
            var ledger = new RestartRecoveringH1UnityInvocationLedger(
                new FileH1UnityInvocationLedger(profile),
                projectLease);
            var coordinator = new H1UnityEditorExecutionCoordinator(
                profile,
                new FixedH1UnityEditorProcessLauncher(),
                projectLease,
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
                    new H1ProjectionReconciliationExecutor(H1ProjectionReconciliationExecutor.ImportProposalKey, worldReader, profile),
                    new H1ProjectionCleanRebuildExecutor(worldReader, profile)
                });

            // Bind only the capabilities backed by Unity worker executors. Host-local checkpoint
            // handlers are intentionally absent from this projection.
            var editorComposition = ContractComposer.Compose(
                CanonicalWorldContract.CreateContribution(new WorldInspectionService(world), world),
                new[]
                {
                    UnityAuthoringProvider.CreateContribution(),
                    H1ProjectionContract.PlanContribution(worldReader, profile),
                    H1ProjectCheckpointComposition.CreateEditorHostContribution(coordinator, worldReader, profile, store, includeCheckpointHandlers: false),
                    H1UnityLifecycleContract.CreateLifecycleStatusContribution(ledger)
                });
            if (!editorComposition.Success || editorComposition.Contract == null)
                throw CompositionFailure("editor", editorComposition);

            var editorProjection = H1UnityHostCapabilityPolicy.CreateProjection(
                editorComposition.Contract,
                UnityProjectWorkspaceAuthority.ForArkusUnityProject(),
                H1ProjectCheckpointComposition.CreateGrants(includeCheckpointHandlers: false));
            coordinator.Bind(editorProjection);

            var publicComposition = ContractComposer.Compose(
                CanonicalWorldContract.CreateContribution(new WorldInspectionService(world), world),
                new[]
                {
                    UnityAuthoringProvider.CreateContribution(),
                    H1ProjectionContract.PlanContribution(worldReader, profile),
                    H1ProjectCheckpointComposition.CreateEditorHostContribution(coordinator, worldReader, profile, store, includeCheckpointHandlers: true),
                    H1UnityLifecycleContract.CreateLifecycleStatusContribution(ledger)
                });
            if (!publicComposition.Success || publicComposition.Contract == null)
            {
                editorProjection.Dispose();
                throw CompositionFailure("public", publicComposition);
            }

            var publicProjection = H1UnityHostCapabilityPolicy.CreateProjection(
                publicComposition.Contract,
                UnityProjectWorkspaceAuthority.ForArkusUnityProject(),
                H1ProjectCheckpointComposition.CreateGrants(includeCheckpointHandlers: true));
            editorProjection.Dispose();
            return publicProjection;
        }

        private static InvalidOperationException CompositionFailure(string stage, ContractCompositionResult result)
        {
            var first = result.Issues.Count == 0 ? "unknown" : result.Issues[0].Code + " at " + result.Issues[0].Path;
            return new InvalidOperationException("H1-10 " + stage + " production contract failed composition: " + first + ".");
        }
    }
}
