using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Arkus.Game.World;
using Arkus.Harness.Protocol;

namespace Arkus.Game.Authoring
{
    public interface IWorldStateSource
    {
        WorldState Current { get; }
    }

    public sealed class FixedWorldStateSource : IWorldStateSource
    {
        public FixedWorldStateSource(WorldState state)
        {
            Current = state ?? throw new ArgumentNullException(nameof(state));
        }

        public WorldState Current { get; }
    }

    public interface IWorldInspectionService
    {
        CapabilityInvocationResult Summary(IReadOnlyDictionary<string, object?> request);
        CapabilityInvocationResult GetObject(IReadOnlyDictionary<string, object?> request);
        CapabilityInvocationResult QueryObjects(IReadOnlyDictionary<string, object?> request);
        CapabilityInvocationResult QueryReferences(IReadOnlyDictionary<string, object?> request);
        CapabilityInvocationResult QueryExtensions(IReadOnlyDictionary<string, object?> request);
        CapabilityInvocationResult ReadExtension(IReadOnlyDictionary<string, object?> request);
    }

    public sealed class UnavailableWorldInspectionService : IWorldInspectionService
    {
        public CapabilityInvocationResult Summary(IReadOnlyDictionary<string, object?> request) => Unavailable();
        public CapabilityInvocationResult GetObject(IReadOnlyDictionary<string, object?> request) => Unavailable();
        public CapabilityInvocationResult QueryObjects(IReadOnlyDictionary<string, object?> request) => Unavailable();
        public CapabilityInvocationResult QueryReferences(IReadOnlyDictionary<string, object?> request) => Unavailable();
        public CapabilityInvocationResult QueryExtensions(IReadOnlyDictionary<string, object?> request) => Unavailable();
        public CapabilityInvocationResult ReadExtension(IReadOnlyDictionary<string, object?> request) => Unavailable();

        private static CapabilityInvocationResult Unavailable()
        {
            return CapabilityInvocationResult.Failed(new StructuredError(
                "world.state_unavailable",
                "No canonical world state is bound to this runtime instance.",
                "$",
                new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)),
                true,
                "Bind a canonical world state source before invoking world inspection capabilities."));
        }
    }
}
