using System;
using Arkus.Harness.Runtime;

namespace Arkus.Harness.Cli
{
    /// <summary>
    /// Headless host entry point.
    /// </summary>
    /// <remarks>
    /// WP-HK-00 only proves that a headless host can be built and run on Linux CI
    /// over the canonical kernel. The production transport, command surface and
    /// closed process boundary are owned by WP-HK-07.
    /// </remarks>
    public static class Program
    {
        /// <summary>Prints the effectively composed kernel modules.</summary>
        /// <returns>Process exit code.</returns>
        public static int Main()
        {
            foreach (var module in RuntimeModule.ComposedModules)
            {
                Console.Out.WriteLine(module);
            }

            return 0;
        }
    }
}
