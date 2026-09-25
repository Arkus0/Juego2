using System;
using System.Collections.Generic;
using System.Linq;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;

namespace Arkus.H1.UnityHost
{
    /// <summary>
    /// Small local container used while extending the single arkus.unity-host provider. H1-10 does
    /// not introduce another provider/scope; its definitions and routes are merged into the already
    /// reviewed H1 provider descriptor below.
    /// </summary>
    public sealed class ContractContribution
    {
        public ContractContribution(IEnumerable<CapabilityDefinition> definitions, IEnumerable<CapabilityRoute> routes)
        {
            Definitions = (definitions ?? throw new ArgumentNullException(nameof(definitions))).ToArray();
            Routes = (routes ?? throw new ArgumentNullException(nameof(routes))).ToArray();
        }

        public IReadOnlyList<CapabilityDefinition> Definitions { get; }
        public IReadOnlyList<CapabilityRoute> Routes { get; }
    }

    public static class H1ProjectCheckpointComposition
    {
        public static CanonicalProviderContribution CreateEditorHostContribution(
            H1UnityEditorExecutionCoordinator coordinator,
            IWorldStateSource world,
            H1UnityLaunchProfile profile,
            H1ProjectCheckpointStore store,
            bool includeCheckpointHandlers)
        {
            if (coordinator == null) throw new ArgumentNullException(nameof(coordinator));
            if (world == null) throw new ArgumentNullException(nameof(world));
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            if (store == null) throw new ArgumentNullException(nameof(store));

            var existing = H1ProjectionReconciliationComposition.CreateEditorHostContribution(coordinator);
            var clean = H1ProjectCheckpointContract.CreateCleanRebuildContribution(coordinator);
            var definitions = existing.Definitions.Concat(clean.Definitions);
            var routes = existing.Routes.Concat(clean.Routes);

            if (includeCheckpointHandlers)
            {
                var checkpoint = H1ProjectCheckpointContract.CreateProjectCheckpointContribution(world, profile, store);
                var restore = H1ProjectCheckpointRestoreContract.CreateContribution(profile, store);
                definitions = definitions.Concat(checkpoint.Definitions).Concat(restore.Definitions);
                routes = routes.Concat(checkpoint.Routes).Concat(restore.Routes);
            }

            return new CanonicalProviderContribution(existing.Descriptor, definitions.ToArray(), routes.ToArray());
        }

        public static IReadOnlyList<UnityHostCapabilityGrant> CreateGrants(bool includeCheckpointHandlers)
        {
            var grants = H1ProjectionReconciliationComposition.CreateGrants()
                .Concat(H1ProjectCheckpointContract.CleanRebuildGrants());
            if (includeCheckpointHandlers)
            {
                // The public checkpoint request is selected by the already-reviewed managed-scene
                // logical reference. The checkpoint root itself is fixed host metadata and is never
                // selected by request data.
                grants = grants.Concat(new[]
                {
                    new UnityHostCapabilityGrant(
                        H1ProjectCheckpointContract.CaptureKey,
                        UnityProjectWorkspaceAuthority.ProjectSettingsRootId,
                        H1ProjectionContract.ReferenceNamespace,
                        UnityHostResourceClass.ProjectMetadata,
                        UnityHostTimeClass.BoundedEditorEffect),
                    new UnityHostCapabilityGrant(
                        H1ProjectCheckpointContract.CurrentKey,
                        UnityProjectWorkspaceAuthority.ProjectSettingsRootId,
                        H1ProjectionContract.ReferenceNamespace,
                        UnityHostResourceClass.ProjectMetadata,
                        UnityHostTimeClass.BoundedRead)
                }).Concat(H1ProjectCheckpointRestoreContract.Grants());
            }
            return grants.ToArray();
        }
    }
}
