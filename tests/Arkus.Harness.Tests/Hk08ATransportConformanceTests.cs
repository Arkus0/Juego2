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
    public sealed class Hk08ATransportConformanceTests
    {
        private const string CanonicalKeyMetaKey = "dev.arkus/canonicalKey";

        [Fact]
        public void Hk08AChangedInteractionPrimitivesRemainEquivalentAcrossReferenceJsonlAndMcp()
        {
            using var reference = new ReferenceClient();
            using var mcp = new McpClient();

            var start = Equivalent(reference, mcp, "world.summary", Empty());
            var referenceBatch = MutationRequest(Result(start.Reference).GetProperty("world"), "request.hk08a.transport.batch", BatchOperations(96));
            var mcpBatch = MutationRequest(Result(start.Mcp).GetProperty("world"), "request.hk08a.transport.batch", BatchOperations(96));
            var applied = Equivalent(reference, mcp, "authoring.change.apply", referenceBatch, mcpBatch);
            Assert.True(Result(applied.Reference).GetProperty("persisted").GetBoolean());
            Assert.Equal(96, Result(applied.Reference).GetProperty("plan").GetProperty("changes").GetArrayLength());

            var afterBatch = Equivalent(reference, mcp, "world.summary", Empty());
            var referenceSecond = MutationRequest(
                Result(afterBatch.Reference).GetProperty("world"),
                "request.hk08a.transport.second",
                BatchOperations(1, 96));
            var mcpSecond = MutationRequest(
                Result(afterBatch.Mcp).GetProperty("world"),
                "request.hk08a.transport.second",
                BatchOperations(1, 96));
            Equivalent(reference, mcp, "authoring.change.apply", referenceSecond, mcpSecond);

            var firstPage = Equivalent(
                reference,
                mcp,
                "authoring.journal.read",
                new Dictionary<string, object?>(StringComparer.Ordinal) { ["limit"] = 1 });
            var referencePage = Result(firstPage.Reference);
            var mcpPage = Result(firstPage.Mcp);
            Assert.Equal("arkus.authoring.journal-page@1", referencePage.GetProperty("schemaId").GetString());
            Assert.Equal(2, referencePage.GetProperty("entryCount").GetInt32());
            Assert.Equal(1, referencePage.GetProperty("entries").GetArrayLength());
            var referenceCursor = referencePage.GetProperty("nextCursor").GetString()!;
            var mcpCursor = mcpPage.GetProperty("nextCursor").GetString()!;

            var secondPage = Equivalent(
                reference,
                mcp,
                "authoring.journal.read",
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["limit"] = 1,
                    ["cursor"] = referenceCursor
                },
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["limit"] = 1,
                    ["cursor"] = mcpCursor
                });
            Assert.Equal(1, Result(secondPage.Reference).GetProperty("pageOffset").GetInt32());
            Assert.False(Result(secondPage.Reference).TryGetProperty("nextCursor", out _));

            var finalSummary = Equivalent(reference, mcp, "world.summary", Empty());
            var referenceCompact = CompactObjectRequest(Result(finalSummary.Reference).GetProperty("world"), "transport.item.000");
            var mcpCompact = CompactObjectRequest(Result(finalSummary.Mcp).GetProperty("world"), "transport.item.000");
            var compact = Equivalent(reference, mcp, "world.object.get", referenceCompact, mcpCompact);
            var compactObject = Result(compact.Reference).GetProperty("object");
            Assert.Equal("transport.item.000", compactObject.GetProperty("id").GetString());
            Assert.False(compactObject.TryGetProperty("typeId", out _));
            Assert.True(Result(compact.Reference).TryGetProperty("world", out _));
        }

        [Fact]
        public void Hk08ACostAndBatchDiscoveryMetadataAreIdenticalAcrossTransports()
        {
            using var reference = new ReferenceClient();
            using var mcp = new McpClient();

            var describe = Equivalent(reference, mcp, "system.describe", Empty());
            var referenceCapabilities = Result(describe.Reference).GetProperty("capabilities").EnumerateArray().ToArray();
            var apply = referenceCapabilities.Single(value => value.GetProperty("name").GetString() == "authoring.change.apply");
            var journal = referenceCapabilities.Single(value => value.GetProperty("name").GetString() == "authoring.journal.read");
            var summary = referenceCapabilities.Single(value => value.GetProperty("name").GetString() == "world.summary");

            Assert.Equal(96, apply.GetProperty("batching").GetProperty("maximumItems").GetInt32());
            Assert.Equal("canonicalmutation", apply.GetProperty("sideEffect").GetString());
            Assert.Equal("readonly", journal.GetProperty("sideEffect").GetString());
            Assert.True(summary.GetProperty("cost").GetProperty("relativeWeight").GetInt32() < apply.GetProperty("cost").GetProperty("relativeWeight").GetInt32());
        }

        private static (JsonElement Reference, JsonElement Mcp) Equivalent(
            IClient reference,
            IClient mcp,
            string capability,
            object arguments)
        {
            return Equivalent(reference, mcp, capability, arguments, arguments);
        }

        private static (JsonElement Reference, JsonElement Mcp) Equivalent(
            IClient reference,
            IClient mcp,
            string capability,
            object referenceArguments,
            object mcpArguments)
        {
            var left = reference.Invoke(capability, referenceArguments);
            var right = mcp.Invoke(capability, mcpArguments);
            var leftNode = (JsonObject)JsonNode.Parse(left.GetRawText())!;
            var rightNode = (JsonObject)JsonNode.Parse(right.GetRawText())!;
            leftNode.Remove("requestId");
            rightNode.Remove("requestId");
            Assert.True(
                JsonNode.DeepEquals(leftNode, rightNode),
                "HK08A transport drift. JSONL=" + leftNode.ToJsonString() + " MCP=" + rightNode.ToJsonString());
            return (left, right);
        }

        private static JsonElement Result(JsonElement outcome)
        {
            Assert.Equal("success", outcome.GetProperty("status").GetString());
            return outcome.GetProperty("result");
        }

        private static IReadOnlyList<object?> BatchOperations(int count, int start = 0)
        {
            var operations = new List<object?>();
            for (var index = start; index < start + count; index++)
            {
                operations.Add(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "put-object",
                    ["id"] = "transport.item." + index.ToString("D3", System.Globalization.CultureInfo.InvariantCulture),
                    ["typeId"] = "fixture.hk08a.transport-item",
                    ["references"] = Array.Empty<object?>()
                });
            }
            return operations.AsReadOnly();
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

        private static IReadOnlyDictionary<string, object?> CompactObjectRequest(JsonElement anchor, string id)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["revision"] = anchor.GetProperty("revision").GetInt64(),
                ["hash"] = anchor.GetProperty("hash").GetString(),
                ["id"] = id,
                ["fields"] = Array.Empty<object?>()
            };
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
                _process = Hk07AProcessHarness.Start();
                _error = _process.StandardError.ReadToEndAsync();
            }

            public JsonElement Invoke(string capability, object arguments)
            {
                var requestId = "hk08a.reference." + (++_sequence).ToString(System.Globalization.CultureInfo.InvariantCulture);
                _process.StandardInput.WriteLine(Hk07AProcessHarness.Frame(requestId, capability, arguments));
                _process.StandardInput.Flush();
                var line = _process.StandardOutput.ReadLine();
                Assert.False(string.IsNullOrWhiteSpace(line), "Reference JSONL process ended before response.");
                using var document = JsonDocument.Parse(line!);
                return document.RootElement.GetProperty("response").Clone();
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
                            ["name"] = "arkus.hk08a.conformance",
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

            public JsonElement Invoke(string capability, object arguments)
            {
                var key = capability + "@1.0";
                Assert.True(_toolNames.TryGetValue(key, out var toolName), "MCP discovery omitted " + key);
                var response = Request(
                    "tools/call",
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["name"] = toolName,
                        ["arguments"] = arguments
                    });
                Assert.False(response.TryGetProperty("error", out var rpcError), rpcError.ValueKind == JsonValueKind.Undefined ? "MCP JSON-RPC error" : rpcError.GetRawText());
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
                    if (root.TryGetProperty("id", out var responseId) && responseId.ValueKind == JsonValueKind.Number && responseId.GetInt32() == id)
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
