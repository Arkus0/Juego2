using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1UnityEditorLifecycleTransportTests
    {
        private const string CanonicalKeyMetaKey = "dev.arkus/canonicalKey";

        [Fact]
        public void H1_04_catalogue_public_inventory_is_visible_in_jsonl_and_mcp()
        {
            using var reference = new ReferenceClient();
            using var mcp = new McpClient();
            var discovery = Equivalent(reference, mcp, "system.describe", Empty());
            var capabilities = Result(discovery.Reference).GetProperty("capabilities").EnumerateArray().ToArray();
            foreach (var name in new[] { "unity.host.catalogue.query", "unity.host.catalogue.get", "unity.host.catalogue.resolve" })
            {
                Assert.Single(capabilities.Where(value => value.GetProperty("name").GetString() == name));
                Assert.True(mcp.ContainsCanonicalKey(name + "@1.0"));
            }
        }

        [Fact]
        public void H1_reference_and_mcp_share_lifecycle_discovery_and_normalized_status_outcome()
        {
            using var reference = new ReferenceClient();
            using var mcp = new McpClient();

            var discovery = Equivalent(reference, mcp, "system.describe", Empty());
            var capabilities = Result(discovery.Reference).GetProperty("capabilities").EnumerateArray().ToArray();
            Assert.Single(capabilities.Where(value => value.GetProperty("name").GetString() == "unity.host.project-profile.inspect"));
            Assert.Single(capabilities.Where(value => value.GetProperty("name").GetString() == "unity.lifecycle.operation-status"));
            Assert.True(mcp.ContainsCanonicalKey("unity.host.project-profile.inspect@1.0"));
            Assert.True(mcp.ContainsCanonicalKey("unity.lifecycle.operation-status@1.0"));

            var invocationId = "h1u-missing-" + Guid.NewGuid().ToString("N");
            var status = Equivalent(
                reference,
                mcp,
                "unity.lifecycle.operation-status",
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["invocationId"] = invocationId
                });

            Assert.Equal("canonical", status.Reference.GetProperty("failureKind").GetString());
            Assert.Equal(
                "unity.lifecycle.unknown-invocation",
                status.Reference.GetProperty("error").GetProperty("machineCode").GetString());
        }

        private static (JsonElement Reference, JsonElement Mcp) Equivalent(
            IClient reference,
            IClient mcp,
            string capability,
            object arguments)
        {
            var left = reference.Invoke(capability, arguments);
            var right = mcp.Invoke(capability, arguments);
            var leftNode = (JsonObject)JsonNode.Parse(left.GetRawText())!;
            var rightNode = (JsonObject)JsonNode.Parse(right.GetRawText())!;
            leftNode.Remove("requestId");
            rightNode.Remove("requestId");
            Assert.True(
                JsonNode.DeepEquals(leftNode, rightNode),
                "H1-03A cross-transport semantic drift. JSONL=" + leftNode.ToJsonString() +
                " MCP=" + rightNode.ToJsonString());
            return (left, right);
        }

        private static JsonElement Result(JsonElement outcome)
        {
            Assert.Equal("success", outcome.GetProperty("status").GetString());
            return outcome.GetProperty("result");
        }

        private static IReadOnlyDictionary<string, object?> Empty() =>
            new Dictionary<string, object?>(StringComparer.Ordinal);

        private interface IClient : IDisposable
        {
            JsonElement Invoke(string capability, object arguments);
        }

        private sealed class ReferenceClient : IClient
        {
            private readonly Process _process;
            private readonly System.Threading.Tasks.Task<string> _error;
            private int _sequence;

            public ReferenceClient()
            {
                _process = Hk07AProcessHarness.Start(new[] { "--h1-unity" });
                _error = _process.StandardError.ReadToEndAsync();
            }

            public JsonElement Invoke(string capability, object arguments)
            {
                var requestId = "h1-03a.reference." + (++_sequence).ToString(System.Globalization.CultureInfo.InvariantCulture);
                _process.StandardInput.WriteLine(Hk07AProcessHarness.Frame(requestId, capability, arguments));
                _process.StandardInput.Flush();
                var line = _process.StandardOutput.ReadLine();
                Assert.False(string.IsNullOrWhiteSpace(line), "H1 reference process ended before response.");
                using var document = JsonDocument.Parse(line!);
                return document.RootElement.GetProperty("response").Clone();
            }

            public void Dispose()
            {
                _process.StandardInput.Close();
                Assert.True(_process.WaitForExit(30000), "H1 reference process did not exit.");
                Assert.Equal(0, _process.ExitCode);
                Assert.Equal(string.Empty, _error.Result);
                _process.Dispose();
            }
        }

        private sealed class McpClient : IClient
        {
            private readonly Process _process;
            private readonly System.Threading.Tasks.Task<string> _error;
            private readonly Dictionary<string, string> _toolNames;
            private int _sequence;

            public McpClient()
            {
                _process = StartProcess();
                _error = _process.StandardError.ReadToEndAsync();
                var initialized = Request(
                    "initialize",
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["protocolVersion"] = "2025-11-25",
                        ["capabilities"] = new Dictionary<string, object?>(StringComparer.Ordinal),
                        ["clientInfo"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                        {
                            ["name"] = "arkus.h1-03a.conformance",
                            ["version"] = "1.0.0"
                        }
                    });
                Assert.Equal("2025-11-25", initialized.GetProperty("result").GetProperty("protocolVersion").GetString());
                Notify("notifications/initialized", Empty());

                _toolNames = new Dictionary<string, string>(StringComparer.Ordinal);
                var tools = Request("tools/list", Empty()).GetProperty("result").GetProperty("tools");
                foreach (var tool in tools.EnumerateArray())
                {
                    var key = tool.GetProperty("_meta").GetProperty(CanonicalKeyMetaKey).GetString()!;
                    Assert.True(
                        _toolNames.TryAdd(key, tool.GetProperty("name").GetString()!),
                        "Duplicate H1 MCP canonical key " + key);
                }
            }

            public bool ContainsCanonicalKey(string key) => _toolNames.ContainsKey(key);

            public JsonElement Invoke(string capability, object arguments)
            {
                var key = capability + "@1.0";
                Assert.True(_toolNames.TryGetValue(key, out var toolName), "H1 MCP discovery omitted " + key);
                var response = Request(
                    "tools/call",
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["name"] = toolName,
                        ["arguments"] = arguments
                    });
                Assert.False(
                    response.TryGetProperty("error", out var rpcError),
                    rpcError.ValueKind == JsonValueKind.Undefined ? "H1 MCP JSON-RPC error" : rpcError.GetRawText());
                var result = response.GetProperty("result");
                Assert.True(result.TryGetProperty("structuredContent", out var structured), "H1 MCP omitted structuredContent.");
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
                    Assert.False(string.IsNullOrWhiteSpace(line), "H1 MCP process ended before response for " + method + ".");
                    using var document = JsonDocument.Parse(line!);
                    var root = document.RootElement;
                    if (root.TryGetProperty("id", out var responseId) &&
                        responseId.ValueKind == JsonValueKind.Number &&
                        responseId.GetInt32() == id)
                    {
                        return root.Clone();
                    }
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
                Assert.True(_process.WaitForExit(30000), "H1 MCP process did not exit.");
                Assert.Equal(0, _process.ExitCode);
                Assert.Equal(string.Empty, _error.Result);
                _process.Dispose();
            }

            private static Process StartProcess()
            {
                var root = Hk07AProcessHarness.FindRepositoryRoot();
                var dll = Path.Combine(root, "src", "Arkus.Harness.Mcp", "bin", "Release", "net8.0", "Arkus.Harness.Mcp.dll");
                if (!File.Exists(dll)) throw new InvalidOperationException("Release MCP assembly is missing: " + dll);
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
                start.ArgumentList.Add("--h1-unity");
                return Process.Start(start) ?? throw new InvalidOperationException("Failed to start H1 Arkus MCP server.");
            }
        }
    }
}
