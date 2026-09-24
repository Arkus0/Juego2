using System;
using Arkus.Game.World;

namespace Arkus.Game.Authoring
{
    /// <summary>Read-only projection of the current canonical state for engine bridges.</summary>
    public sealed class ReadOnlyWorldStateView : IWorldStateSource
    {
        private readonly IWorldStateSource _source;
        public ReadOnlyWorldStateView(IWorldStateSource source) => _source = source ?? throw new ArgumentNullException(nameof(source));
        public WorldState Current => _source.Current;
    }
}
