using Arkus.Game.Core;
using Arkus.Game.Validation;
using Arkus.Game.World;
using Arkus.Harness.Protocol;

namespace Arkus.Game.Authoring
{
    /// <summary>Boundary marker that causally exercises the required authoring edges.</summary>
    public static class AuthoringModule
    {
        public static string Name => "Arkus.Game.Authoring";
        public static string Composition =>
            CoreModule.Name + ":" + WorldModule.Name + ":" + ValidationModule.Name + ":" + ProtocolModule.Name;
    }
}
