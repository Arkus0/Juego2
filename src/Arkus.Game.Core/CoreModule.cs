using System.Collections.Generic;

namespace Arkus.Game.Core
{
    /// <summary>
    /// Boundary marker for the engine-agnostic core module.
    /// </summary>
    /// <remarks>
    /// WP-HK-00 fixes module identity and dependency direction only. Canonical
    /// world state, identity and deterministic serialization are owned by
    /// WP-HK-02.
    /// </remarks>
    public static class CoreModule
    {
        /// <summary>Canonical module name.</summary>
        public static string Name => "Arkus.Game.Core";

        /// <summary>
        /// Module names effectively composed into this module, including itself,
        /// ordinal-sorted.
        /// </summary>
        public static IReadOnlyList<string> ComposedModules => new List<string> { Name };
    }
}
