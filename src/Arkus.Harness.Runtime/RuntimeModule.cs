using Arkus.Game.Authoring;
using Arkus.Game.Validation;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Runtime
{
    /// <summary>Boundary marker that causally exercises the required runtime edges.</summary>
    public static class RuntimeModule
    {
        public const string Name = "Arkus.Harness.Runtime";
        public static string Composition => ProtocolModule.Name + ":" + AuthoringModule.Name + ":" + ValidationModule.Name;
    }
}
