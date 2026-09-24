using System;
using System.Threading.Tasks;
using Arkus.H1.UnityHost;
using Arkus.Harness.Projection;
using ModelContextProtocol.Server;

namespace Arkus.Harness.Mcp
{
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            NeutralProjectionService projection;
            if (args.Length == 0)
            {
                projection = ProductionHarnessHost.Create();
            }
            else if (args.Length == 1 && string.Equals(args[0], "--h1-unity", StringComparison.Ordinal))
            {
                projection = ProductionH1UnityHost.Create();
            }
            else
            {
                Console.Error.WriteLine("Arkus MCP projection accepts either no arguments or the fixed --h1-unity profile selector; use MCP over stdin/stdout.");
                return 64;
            }

            using (projection)
            {
                var adapter = new McpProjectionAdapter(projection);
                var options = adapter.CreateServerOptions();
                await using var server = McpServer.Create(new StdioServerTransport(options), options);
                await server.RunAsync().ConfigureAwait(false);
                return 0;
            }
        }
    }
}
