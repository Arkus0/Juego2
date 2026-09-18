using System;
using System.Collections.Generic;
using Arkus.Game.Core;
using Arkus.Game.World;
using Arkus.Harness.Protocol;

namespace Arkus.Game.Validation
{
    /// <summary>
    /// Boundary marker for the validation module.
    /// </summary>
    /// <remarks>
    /// Invariants and structured repairable diagnostics are owned by WP-HK-05.
    /// </remarks>
    public static class ValidationModule
    {
        /// <summary>Canonical module name.</summary>
        public static string Name => "Arkus.Game.Validation";

        /// <summary>
        /// Module names effectively composed into this module, including itself,
        /// ordinal-sorted.
        /// </summary>
        public static IReadOnlyList<string> ComposedModules
        {
            get
            {
                var modules = new SortedSet<string>(StringComparer.Ordinal) { Name };
                modules.UnionWith(CoreModule.ComposedModules);
                modules.UnionWith(WorldModule.ComposedModules);
                modules.UnionWith(ProtocolModule.ComposedModules);
                return new List<string>(modules);
            }
        }
    }
}
