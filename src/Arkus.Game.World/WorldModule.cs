using Arkus.Game.Core;

namespace Arkus.Game.World
{
    /// <summary>Boundary marker that causally exercises the required Core edge.</summary>
    public static class WorldModule
    {
        public static string Name => "Arkus.Game.World";
        public static string CoreBoundary => CoreModule.Name;
    }
}
