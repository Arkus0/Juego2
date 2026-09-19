using Arkus.Game.Core;
using Arkus.Game.World;
using Arkus.Harness.Protocol;

namespace Arkus.Game.Validation
{
    /// <summary>Boundary marker that causally exercises the required validation edges.</summary>
    public static class ValidationModule
    {
        public static string Name => "Arkus.Game.Validation";
        public static string Composition => CoreModule.Name + ":" + WorldModule.Name + ":" + ProtocolModule.Name;
    }
}
