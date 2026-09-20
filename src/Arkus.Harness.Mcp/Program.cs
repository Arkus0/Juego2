using System;
using System.Threading.Tasks;
using Arkus.Harness.Projection;
using ModelContextProtocol.Server;

namespace Arkus.Harness.Mcp
{
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            if (args.Length != 0)
            {
                Console.Error.WriteLine("Arkus MCP projection accepts no command-line arguments; use MCP over stdin/stdout.");
                return 64;
            }

            var projection = ProductionHarnessHost.Create();
            var adapter = new McpProjectionAdapter(projection);
            var options = adapter.CreateServerOptions();
            await using var server = McpServer.Create(new StdioServerTransport(options), options);
            await server.RunAsync().ConfigureAwait(false);
            return 0;
        }
    }
}
