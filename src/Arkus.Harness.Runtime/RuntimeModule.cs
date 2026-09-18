using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Game.Validation;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Runtime
{
    /// <summary>
    /// Boundary marker for the harness runtime composition root.
    /// </summary>
    /// <remarks>
    /// The runtime deliberately does not reference <c>Arkus.Game.Core</c> or
    /// <c>Arkus.Game.World</c> directly: it reaches world state only through the
    /// authoring and validation surfaces. Transitive project references are
    /// disabled repository-wide, so that restriction is enforced by the compiler
    /// rather than by convention. Transport and process boundary are owned by
    /// WP-HK-07.
    /// </remarks>
    public static class RuntimeModule
    {
        /// <summary>Canonical module name.</summary>
        public static string Name => "Arkus.Harness.Runtime";

        /// <summary>
        /// Module names effectively composed into this module, including itself,
        /// ordinal-sorted.
        /// </summary>
        public static IReadOnlyList<string> ComposedModules
        {
            get
            {
                var modules = new SortedSet<string>(StringComparer.Ordinal) { Name };
                modules.UnionWith(AuthoringModule.ComposedModules);
                modules.UnionWith(ValidationModule.ComposedModules);
                modules.UnionWith(ProtocolModule.ComposedModules);
                return new List<string>(modules);
            }
        }
    }
}
