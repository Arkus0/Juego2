using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Runtime
{
    internal static class WorldPortabilityBindings
    {
        public static IReadOnlyList<CapabilityRoute> CreateRoutes(IWorldMutationService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            var reader = (IWorldPortabilityService)new WorldMutationPlannerView(service);
            var importer = IdempotentWorldSnapshotAuthority.Bind(service);
            return new List<CapabilityRoute>
            {
                CapabilityRoute.FromHandler(new WorldSemanticDiffHandler(reader)),
                CapabilityRoute.FromHandler(new WorldSnapshotExportHandler(reader)),
                CapabilityRoute.FromHandler(new WorldSnapshotImportHandler(importer))
            }.AsReadOnly();
        }
    }

    [PublicCapabilityRoute("arkus.base", WorldPortabilityContract.CompareName, "1.0")]
    internal sealed class WorldSemanticDiffHandler : ICanonicalCapabilityHandler
    {
        private readonly IWorldPortabilityService _service;

        public WorldSemanticDiffHandler(IWorldPortabilityService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (request == null) throw new ArgumentNullException(nameof(request));
            return _service.CompareSnapshots(request);
        }
    }

    [PublicCapabilityRoute("arkus.base", WorldPortabilityContract.ExportName, "1.0")]
    internal sealed class WorldSnapshotExportHandler : ICanonicalCapabilityHandler
    {
        private readonly IWorldPortabilityService _service;

        public WorldSnapshotExportHandler(IWorldPortabilityService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (request == null) throw new ArgumentNullException(nameof(request));
            return _service.ExportSnapshot(request);
        }
    }

    [PublicCapabilityRoute("arkus.base", WorldPortabilityContract.ImportName, "1.0")]
    internal sealed class WorldSnapshotImportHandler : ITransactionalMutationHandler
    {
        private readonly ICanonicalWorldSnapshotImporter _importer;

        public WorldSnapshotImportHandler(ICanonicalWorldSnapshotImporter importer)
        {
            _importer = importer ?? throw new ArgumentNullException(nameof(importer));
        }

        public CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (request == null) throw new ArgumentNullException(nameof(request));
            return _importer.ImportSnapshot(request);
        }
    }
}
