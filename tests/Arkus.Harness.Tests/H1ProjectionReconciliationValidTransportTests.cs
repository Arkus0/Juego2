using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Arkus.H1.UnityHost;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1ProjectionReconciliationValidTransportTests
    {
        private const string CanonicalKeyMetaKey = "dev.arkus/canonicalKey";

        [Fact]
        public void ValidReconciliationResultIsEquivalentAcrossReferenceJsonlAndMcp()
        {
            using var reference = new ReferenceClient();
            using var mcp = new McpClient();
            var arguments = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["sceneLogicalId"] = H1ManagedScenePlan.SceneId
            };

            var left = reference.Invoke("unity.host.projection.drift", arguments);
            var right = mcp.Invoke("unity.host.projection.drift", arguments);
            var leftNode = (JsonObject)JsonNode.Parse(left.GetRawText())!;
            var rightNode = (JsonObject)JsonNode.Parse(right.GetRawText())!;
            leftNode.Remove("requestId");
            rightNode.Remove("requestId");
            Assert.True(JsonNode.DeepEquals(leftNode, rightNode),
                "valid H1-09 reconciliation diverged by transport. JSONL=" + leftNode.ToJsonString() + " MCP=" + rightNode.ToJsonString());

            Assert.Equal("success", left.GetProperty("status").GetString());
            var result = left.GetProperty("result");
            Assert.Equal("arkus.h1-projection-drift-report@1", result.GetProperty("schemaId").GetString());
            Assert.Equal("in-sync", result.GetProperty("state").GetString());
            Assert.True(result.GetProperty("parity").GetBoolean());
            Assert.Empty(result.GetProperty("items").EnumerateArray());
        }

        private interface IClient : IDisposable
        {
            JsonElement Invoke(string capability, object arguments);
        }

        private sealed class ReferenceClient : IClient
        {
            private readonly Process _process = Start("--jsonl");
            private readonly System.Threading.Tasks.Task<string> _error;
            private int _sequence;

            public ReferenceClient() => _error = _process.StandardError.ReadToEndAsync();

            public JsonElement Invoke(string capability, object arguments)
            {
                var requestId = "h1-09.valid.reference." + (++_sequence).ToString(System.Globalization.CultureInfo.InvariantCulture);
                _process.StandardInput.WriteLine(Hk07AProcessHarness.Frame(requestId, capability, arguments));
                _process.StandardInput.Flush();
                var line = _process.StandardOutput.ReadLine();
                Assert.False(string.IsNullOrWhiteSpace(line), "valid H1-09 JSONL fixture ended before response");
                using var document = JsonDocument.Parse(line!);
                return document.RootElement.GetProperty("response").Clone();
            }

            public void Dispose()
            {
                _process.StandardInput.Close();
                Assert.True(_process.WaitForExit(30000), "valid H1-09 JSONL fixture did not exit");
                Assert.Equal(0, _process.ExitCode);
                Assert.Equal(string.Empty, _error.Result);
                _process.Dispose();
            }
        }

        private sealed class McpClient : IClient
        {
            private readonly Process _process = Start("--mcp");
            private readonly System.Threading.Tasks.Task<string> _error;
            private readonly Dictionary<string, string> _toolNames = new Dictionary<string, string>(StringComparer.Ordinal);
            private int _sequence;

            public McpClient()
            {
                _error = _process.StandardError.ReadToEndAsync();
                var initialized = Request("initialize", new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["protocolVersion"] = "2025-11-25",
                    ["capabilities"] = new Dictionary<string, object?>(StringComparer.Ordinal),
                    ["clientInfo"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["name"] = "arkus.h1-09.valid-conformance",
                        ["version"] = "1.0.0"
                    }
                });
                Assert.Equal("2025-11-25", initialized.GetProperty("result").GetProperty("protocolVersion").GetString());
                Notify("notifications/initialized", new Dictionary<string, object?>(StringComparer.Ordinal));
                foreach (var tool in Request("tools/list", new Dictionary<string, object?>(StringComparer.Ordinal)).GetProperty("result").GetProperty("tools").EnumerateArray())
                {
                    var key = tool.GetProperty("_meta").GetProperty(CanonicalKeyMetaKey).GetString()!;
                    _toolNames.Add(key, tool.GetProperty("name").GetString()!);
                }
            }

            public JsonElement Invoke(string capability, object arguments)
            {
                Assert.True(_toolNames.TryGetValue(capability + "@1.0", out var toolName));
                var response = Request("tools/call", new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["name"] = toolName,
                    ["arguments"] = arguments
                });
                Assert.False(response.TryGetProperty("error", out var rpcError),
                    rpcError.ValueKind == JsonValueKind.Undefined ? "valid H1-09 MCP JSON-RPC error" : rpcError.GetRawText());
                var result = response.GetProperty("result");
                Assert.True(result.TryGetProperty("structuredContent", out var structured));
                return structured.Clone();
            }

            private JsonElement Request(string method, object parameters)
            {
                var id = ++_sequence;
                _process.StandardInput.WriteLine(JsonSerializer.Serialize(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["jsonrpc"] = "2.0",
                    ["id"] = id,
                    ["method"] = method,
                    ["params"] = parameters
                }));
                _process.StandardInput.Flush();
                while (true)
                {
                    var line = _process.StandardOutput.ReadLine();
                    Assert.False(string.IsNullOrWhiteSpace(line), "valid H1-09 MCP fixture ended before response for " + method);
                    using var document = JsonDocument.Parse(line!);
                    var root = document.RootElement;
                    if (root.TryGetProperty("id", out var responseId) && responseId.ValueKind == JsonValueKind.Number && responseId.GetInt32() == id)
                        return root.Clone();
                }
            }

            private void Notify(string method, object parameters)
            {
                _process.StandardInput.WriteLine(JsonSerializer.Serialize(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["jsonrpc"] = "2.0",
                    ["method"] = method,
                    ["params"] = parameters
                }));
                _process.StandardInput.Flush();
            }

            public void Dispose()
            {
                _process.StandardInput.Close();
                Assert.True(_process.WaitForExit(30000), "valid H1-09 MCP fixture did not exit");
                Assert.Equal(0, _process.ExitCode);
                Assert.Equal(string.Empty, _error.Result);
                _process.Dispose();
            }
        }

        private static Process Start(string mode)
        {
            var root = Hk07AProcessHarness.FindRepositoryRoot();
            var local = Path.Combine(AppContext.BaseDirectory, "Arkus.Harness.H109TransportFixture.dll");
            var dll = File.Exists(local)
                ? local
                : Path.Combine(root, "tests", "Arkus.Harness.H109TransportFixture", "bin", "Release", "net8.0", "Arkus.Harness.H109TransportFixture.dll");
            if (!File.Exists(dll)) throw new InvalidOperationException("H1-09 valid transport fixture is missing: " + dll);
            var host = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH");
            if (string.IsNullOrWhiteSpace(host)) host = "dotnet";
            var start = new ProcessStartInfo(host)
            {
                WorkingDirectory = root,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardInputEncoding = new UTF8Encoding(false),
                StandardOutputEncoding = new UTF8Encoding(false),
                StandardErrorEncoding = new UTF8Encoding(false),
                CreateNoWindow = true
            };
            start.ArgumentList.Add(dll);
            start.ArgumentList.Add(mode);
            return Process.Start(start) ?? throw new InvalidOperationException("Failed to start H1-09 valid transport fixture");
        }
    }
}
