using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Arkus.Game.Authoring;
using Arkus.Harness.Protocol;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk09BTransportConformanceTests
    {
        private const string CanonicalKeyMetaKey = "dev.arkus/canonicalKey";

        [Fact]
        public void ResourceEnvelopeAndLimitDiagnosticsAreEquivalentAcrossReferenceJsonlAndMcp()
        {
            using var reference = new ReferenceClient();
            using var mcp = new McpClient();

            var envelope = Equivalent(reference, mcp, "system.resource-envelope.describe", Empty());
            Assert.Equal(
                H0ResourceEnvelope.SchemaId,
                envelope.GetProperty("result").GetProperty("schemaId").GetString());

            var oversized = Equivalent(
                reference,
                mcp,
                "system.describe",
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["padding"] = new string('x', H0ResourceEnvelope.MaximumCanonicalRequestBytes + 1)
                });
            AssertResourceError(oversized, "resource.request_bytes_exceeded", "canonicalArgumentBytes");

            var page = Equivalent(
                reference,
                mcp,
                WorldInspectionContract.ObjectQueryName,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["limit"] = H0ResourceEnvelope.MaximumPageSize + 1
                });
            AssertResourceError(page, "resource.page_size_exceeded", "pageItems");

            var deep = Equivalent(
                reference,
                mcp,
                "system.describe",
                DeepArguments(H0ResourceEnvelope.MaximumPortableDepth + 2));
            AssertResourceError(deep, "resource.nesting_depth_exceeded", "portableDepth");
        }

        [Fact]
        public void TransportDriftOracleTurnsRedForDifferentResourceDimension()
        {
            using var left = JsonDocument.Parse(
                "{\"status\":\"error\",\"failureKind\":\"resource\",\"error\":{\"code\":\"resource.page_size_exceeded\",\"context\":{\"dimension\":\"pageItems\"}}}");
            using var same = JsonDocument.Parse(
                "{\"status\":\"error\",\"failureKind\":\"resource\",\"error\":{\"code\":\"resource.page_size_exceeded\",\"context\":{\"dimension\":\"pageItems\"}}}");
            Assert.Empty(NeutralSemanticIssues(left.RootElement, same.RootElement));

            var drifted = (JsonObject)JsonNode.Parse(same.RootElement.GetRawText())!;
            ((JsonObject)((JsonObject)drifted["error"]!)["context"]!)["dimension"] = "batchOperations";
            using var right = JsonDocument.Parse(drifted.ToJsonString());
            Assert.Contains("semantic-drift", NeutralSemanticIssues(left.RootElement, right.RootElement));
        }

        private static JsonElement Equivalent(
            IClient reference,
            IClient mcp,
            string capability,
            object arguments,
            string version = "1.0")
        {
            var left = reference.Invoke(capability, arguments, version);
            var right = mcp.Invoke(capability, arguments, version);
            var issues = NeutralSemanticIssues(left, right);
            Assert.True(
                issues.Count == 0,
                "HK09B transport drift: " + string.Join(",", issues) +
                " JSONL=" + left.GetRawText() + " MCP=" + right.GetRawText());
            return left;
        }

        private static IReadOnlyList<string> NeutralSemanticIssues(JsonElement reference, JsonElement mcp)
        {
            var referenceNode = (JsonObject)JsonNode.Parse(reference.GetRawText())!;
            var mcpNode = (JsonObject)JsonNode.Parse(mcp.GetRawText())!;
            referenceNode.Remove("requestId");
            mcpNode.Remove("requestId");
            return JsonNode.DeepEquals(referenceNode, mcpNode)
                ? Array.Empty<string>()
                : new[] { "semantic-drift" };
        }

        private static void AssertResourceError(JsonElement response, string code, string dimension)
        {
            Assert.Equal("error", response.GetProperty("status").GetString());
            Assert.Equal("resource", response.GetProperty("failureKind").GetString());
            var error = response.GetProperty("error");
            Assert.Equal(code, error.GetProperty("code").GetString());
            Assert.Equal(H0ResourceEnvelope.SchemaId, error.GetProperty("context").GetProperty("resourceEnvelope").GetString());
            Assert.Equal(dimension, error.GetProperty("context").GetProperty("dimension").GetString());
        }

        private static IReadOnlyDictionary<string, object?> DeepArguments(int levels)
        {
            IReadOnlyDictionary<string, object?> current = Empty();
            for (var index = 0; index < levels; index++)
            {
                current = new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["nested"] = current
                };
            }
            return current;
        }

        private static IReadOnlyDictionary<string, object?> Empty() =>
            new Dictionary<string, object?>(StringComparer.Ordinal);

        private interface IClient : IDisposable
        {
            JsonElement Invoke(string capability, object arguments, string version);
        }

        private sealed class ReferenceClient : IClient
        {
            private readonly Process _process;
            private readonly System.Threading.Tasks.Task<string> _error;
            private int _sequence;

            public ReferenceClient()
            {
                _process = Hk07AProcessHarness.Start();
                _error = _process.StandardError.ReadToEndAsync();
            }

            public JsonElement Invoke(string capability, object arguments, string version)
            {
                var requestId = "hk09b.reference." + (++_sequence).ToString(System.Globalization.CultureInfo.InvariantCulture);
                _process.StandardInput.WriteLine(Frame(requestId, capability, arguments, version));
                _process.StandardInput.Flush();
                var line = _process.StandardOutput.ReadLine();
                Assert.False(string.IsNullOrWhiteSpace(line), "Reference JSONL process ended before response.");
                using var document = JsonDocument.Parse(line!);
                return document.RootElement.GetProperty("response").Clone();
            }

            private static string Frame(string requestId, string capability, object arguments, string version)
            {
                var parts = version.Split('.');
                Assert.Equal(2, parts.Length);
                return JsonSerializer.Serialize(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["protocol"] = "arkus.reference.jsonl@1",
                    ["request"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["projectionVersion"] = "arkus.neutral-projection@1",
                        ["requestId"] = requestId,
                        ["capability"] = capability,
                        ["acceptedVersions"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                        {
                            ["major"] = int.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture),
                            ["minimumMinor"] = int.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture),
                            ["maximumMinor"] = int.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture)
                        },
                        ["arguments"] = arguments
                    }
                });
            }

            public void Dispose()
            {
                _process.StandardInput.Close();
                Assert.True(_process.WaitForExit(30000), "Reference JSONL process did not exit.");
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
                            ["name"] = "arkus.hk09b.conformance",
                            ["version"] = "1.0.0"
                        }
                    });
                Assert.Equal("2025-11-25", initialized.GetProperty("result").GetProperty("protocolVersion").GetString());
                Notify("notifications/initialized", Empty());
                _toolNames = new Dictionary<string, string>(StringComparer.Ordinal);
                foreach (var tool in Request("tools/list", Empty()).GetProperty("result").GetProperty("tools").EnumerateArray())
                {
                    var key = tool.GetProperty("_meta").GetProperty(CanonicalKeyMetaKey).GetString()!;
                    Assert.True(_toolNames.TryAdd(key, tool.GetProperty("name").GetString()!), "Duplicate MCP canonical key " + key);
                }
            }

            public JsonElement Invoke(string capability, object arguments, string version)
            {
                var key = capability + "@" + version;
                Assert.True(_toolNames.TryGetValue(key, out var toolName), "MCP discovery omitted " + key);
                var response = Request(
                    "tools/call",
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["name"] = toolName,
                        ["arguments"] = arguments
                    });
                Assert.False(response.TryGetProperty("error", out var rpcError),
                    rpcError.ValueKind == JsonValueKind.Undefined ? "MCP JSON-RPC error" : rpcError.GetRawText());
                var result = response.GetProperty("result");
                Assert.True(result.TryGetProperty("structuredContent", out var structured), "MCP omitted structuredContent.");
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
                    Assert.False(string.IsNullOrWhiteSpace(line), "MCP process ended before response for " + method + ".");
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
                Assert.True(_process.WaitForExit(30000), "MCP process did not exit.");
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
                return Process.Start(start) ?? throw new InvalidOperationException("Failed to start Arkus MCP server.");
            }
        }
    }
}
