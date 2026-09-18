using System;
using System.Collections.Generic;
using Arkus.Game.Core;

namespace Arkus.Game.World
{
    /// <summary>
    /// Boundary marker for the world-state module.
    /// </summary>
    /// <remarks>
    /// Composition is written out per module on purpose: with transitive project
    /// references disabled, a module can only compose what it declares as a
    /// direct dependency, so this code is itself a compile-time witness of the
    /// declared dependency direction.
    /// </remarks>
    public static class WorldModule
    {
        /// <summary>Canonical module name.</summary>
        public static string Name => "Arkus.Game.World";

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
                return new List<string>(modules);
            }
        }
    }
}
