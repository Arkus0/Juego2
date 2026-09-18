using System.Collections.Generic;

namespace Arkus.Harness.Protocol
{
    /// <summary>
    /// Boundary marker for the harness protocol module.
    /// </summary>
    /// <remarks>
    /// WP-HK-00 fixes module identity and dependency direction only. Protocol
    /// envelopes, capability discovery and schemas are owned by WP-HK-01 and are
    /// deliberately absent here: this type exists so the module boundary is
    /// exercised by real cross-assembly references instead of being a claim in a
    /// project file.
    /// </remarks>
    public static class ProtocolModule
    {
        /// <summary>Canonical module name.</summary>
        public static string Name => "Arkus.Harness.Protocol";

        /// <summary>
        /// Module names effectively composed into this module, including itself,
        /// ordinal-sorted.
        /// </summary>
        public static IReadOnlyList<string> ComposedModules => new List<string> { Name };
    }
}
