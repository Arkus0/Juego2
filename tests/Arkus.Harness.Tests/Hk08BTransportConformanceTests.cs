using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Arkus.Game.Authoring;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk08BTransportConformanceTests
    {
        private const string CanonicalKeyMetaKey = "dev.arkus/canonicalKey";

        [Fact]
        public void StaleRecoveryMeaningIsIdenticalAcrossReferenceJsonlAndMcp()
        {
            using var reference = new ReferenceClient();
            using var mcp = new McpClient();

            var start = Equivalent(reference, mcp, "world.summary", Empty());
            var referenceAnchor = start.Reference.GetProperty("result").GetProperty("world");
            var mcpAnchor = start.Mcp.GetProperty("result").GetProperty("world");

            var writerOperation = new object?[]
            {
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "put-object",
                    ["id"] = "transport.conflict",
                    ["typeId"] = "fixture.hk08b.writer",
                    ["references"] = Array.Empty<object?>()
                }
            };
            Equivalent(
                reference,
                mcp,
                "authoring.change.apply",
                MutationRequest(referenceAnchor, "request.hk08b.transport.writer", writerOperation),
                MutationRequest(mcpAnchor, "request.hk08b.transport.writer", writerOperation));

            var staleOperation = new object?[]
            {
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "put-object",
                    ["id"] = "transport.client-intent",
                    ["typeId"] = "fixture.hk08b.client",
                    ["references"] = Array.Empty<object?>()
                }
            };
            var stale = Equivalent(
                reference,
                mcp,
                "authoring.change.plan",
                MutationRequest(referenceAnchor, "request.hk08b.transport.stale", staleOperation),
                MutationRequest(mcpAnchor, "request.hk08b.transport.stale", staleOperation));

            var raw = stale.Reference.GetRawText();
            Assert.Contains("world.change.stale_revision", raw, StringComparison.Ordinal);
            Assert.Contains(WorldConflictRecoveryContract.SchemaId, raw, StringComparison.Ordinal);
            Assert.Contains(WorldConflictRecoveryContract.SameLineageReplan, raw, StringComparison.Ordinal);
            Assert.Contains("world.object:transport.conflict", raw, StringComparison.Ordinal);
        }

        [Fact]
        public void CrossTransportRecoveryOracleTurnsRedWhenRecoveryContextDrifts()
        {
            using var leftDocument = JsonDocument.Parse(
                "{\"requestId\":\"left\",\"status\":\"error\",\"error\":{\"context\":{\"recovery\":{\"schemaId\":\"arkus.world-conflict-recovery@1\",\"disposition\":\"same-lineage-replan\",\"changedResources\":[\"world.object:a\"]}}}}}");
            using var rightDocument = JsonDocument.Parse(
                "{\"requestId\":\"right\",\"status\":\"error\",\"error\":{\"context\":{\"recovery\":{\"schemaId\":\"arkus.world-conflict-recovery@1\",\"disposition\":\"same-lineage-replan\",\"changedResources\":[\"world.object:a\"]}}}}}");
            Assert.Empty(NeutralSemanticIssues(leftDocument.RootElement, rightDocument.RootElement));

            var drifted = (JsonObject)JsonNode.Parse(rightDocument.RootElement.GetRawText())!;
            var error = (JsonObject)drifted["error"]!;
            var context = (JsonObject)error["context"]!;
            var recovery = (JsonObject)context["recovery"]!;
            recovery["changedResources"] = new JsonArray();
            using var driftedDocument = JsonDocument.Parse(drifted.ToJsonString());
            Assert.Contains("semantic-drift", NeutralSemanticIssues(leftDocument.RootElement, driftedDocument.RootElement));
        }

        private static (JsonElement Reference, JsonElement Mcp) Equivalent(
            IClient reference,
            IClient mcp,
            string capability,
            object arguments,
            string version = "1.0")
        {
            return Equivalent(reference, mcp, capability, arguments, arguments, version);
        }

        private static (JsonElement Reference, JsonElement Mcp) Equivalent(
            IClient reference,
            IClient mcp,
            string capability,
            object referenceArguments,
            object mcpArguments,
            string version = "1.0")
        {
            var left = reference.Invoke(capability, referenceArguments, version);
            var right = mcp.Invoke(capability, mcpArguments, version);
            var issues = NeutralSemanticIssues(left, right);
            Assert.True(
                issues.Count == 0,
                "HK08B transport drift: " + string.Join(",", issues) +
                " JSONL=" + left.GetRawText() + " MCP=" + right.GetRawText());
            return (left, right);
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

        private static IReadOnlyDictionary<string, object?> MutationRequest(
            JsonElement anchor,
            string idempotencyKey,
            IReadOnlyList<object?> operations)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["idempotencyKey"] = idempotencyKey,
                ["expectedRevision"] = anchor.GetProperty("revision").GetInt64(),
                ["expectedHash"] = anchor.GetProperty("hash").GetString(),
                ["operations"] = operations
            };
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
                var requestId = "hk08b.reference." + (++_sequence).ToString(System.Globalization.CultureInfo.InvariantCulture);
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
                            ["name"] = "arkus.hk08b.conformance",
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
