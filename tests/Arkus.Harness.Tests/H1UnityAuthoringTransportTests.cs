using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Arkus.EngineBridge.UnityAuthoring;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1UnityAuthoringTransportTests
    {
        private const string CanonicalKeyMetaKey = "dev.arkus/canonicalKey";

        [Fact]
        public void UnityScopedCapabilitiesAndPotesCompileAreEquivalentAcrossJsonlAndMcp()
        {
            using var reference = new ReferenceClient();
            using var mcp = new McpClient();

            var discovery = Equivalent(reference, mcp, "system.describe", Empty());
            var capabilities = Result(discovery.Reference).GetProperty("capabilities").EnumerateArray().ToArray();
            Assert.Single(capabilities.Where(value => value.GetProperty("name").GetString() == UnityAuthoringProvider.CompileName));
            Assert.Single(capabilities.Where(value => value.GetProperty("name").GetString() == UnityAuthoringProvider.DecodeName));
            Assert.Single(capabilities.Where(value => value.GetProperty("name").GetString() == UnityAuthoringProvider.InspectName));

            var compile = Equivalent(reference, mcp, UnityAuthoringProvider.CompileName, PotesCompileRequest());
            var result = Result(compile.Reference);
            Assert.Equal(UnityBindingProducer.ResultSchemaId, result.GetProperty("schemaId").GetString());
            Assert.Equal(1, result.GetProperty("canonicalDependencies").GetArrayLength());
            Assert.Equal(4, result.GetProperty("catalogueDependencies").GetArrayLength());
            Assert.Equal(
                "market.potes-root",
                result.GetProperty("canonicalDependencies")[0].GetProperty("targetId").GetString());
        }

        [Fact]
        public void ProductionHostsApplyTheCompiledDocumentMutationAcrossJsonlAndMcp()
        {
            // WP-H1-01 reopen 1: both public production hosts admit the Unity binding document codec, so a client can
            // author the binding without transcribing the opaque payload.
            using var reference = new ReferenceClient();
            using var mcp = new McpClient();
            var compile = Equivalent(reference, mcp, UnityAuthoringProvider.CompileName, PotesCompileRequest());
            var documentMutation = JsonSerializer.Deserialize<Dictionary<string, object?>>(Result(compile.Reference).GetProperty("documentMutation").GetRawText());
            var summary = Result(Equivalent(reference, mcp, "world.summary", Empty()).Reference).GetProperty("world");
            var request = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["idempotencyKey"] = "request.h1-01.transport-document",
                ["expectedRevision"] = summary.GetProperty("revision").GetInt64(),
                ["expectedHash"] = summary.GetProperty("hash").GetString(),
                ["operations"] = new object?[]
                {
                    new Dictionary<string, object?>(StringComparer.Ordinal) { ["kind"] = "put-object", ["id"] = "building.potes-facade", ["typeId"] = "fixture.facade" },
                    new Dictionary<string, object?>(StringComparer.Ordinal) { ["kind"] = "put-object", ["id"] = "market.potes-root", ["typeId"] = "fixture.market-root" },
                    documentMutation
                }
            };
            var applied = Equivalent(reference, mcp, "authoring.change.apply", request);
            Assert.Equal("success", applied.Reference.GetProperty("status").GetString());
            var after = Result(Equivalent(reference, mcp, "world.summary", Empty()).Reference);
            Assert.Equal(1, after.GetProperty("extensionCount").GetInt32());
        }

        private static (JsonElement Reference, JsonElement Mcp) Equivalent(
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
                "H1-01 transport drift: " + string.Join(",", issues) +
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

        private static JsonElement Result(JsonElement outcome)
        {
            Assert.Equal("success", outcome.GetProperty("status").GetString());
            return outcome.GetProperty("result");
        }

        private static IReadOnlyDictionary<string, object?> PotesCompileRequest()
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["subjectId"] = "building.potes-facade",
                ["binding"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["schemaId"] = UnityBindingProducer.BindingSchemaId,
                    ["targetSceneId"] = "scene.potes-market",
                    ["source"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["kind"] = "prefab",
                        ["logicalId"] = "prefab.potes-facade"
                    },
                    ["transform"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["coordinateConvention"] = UnityBindingProducer.CoordinateConvention,
                        ["positionMm"] = Vector(1250, 0, -375),
                        ["rotationMilliDegrees"] = Vector(0, 90000, 0),
                        ["scalePpm"] = Vector(1000000, 1000000, 1000000)
                    },
                    ["components"] = new object?[]
                    {
                        new Dictionary<string, object?>(StringComparer.Ordinal)
                        {
                            ["kind"] = "renderer",
                            ["materialId"] = "material.potes-stone"
                        },
                        new Dictionary<string, object?>(StringComparer.Ordinal)
                        {
                            ["kind"] = "canonical-link",
                            ["relation"] = "attached-to",
                            ["targetObjectId"] = "market.potes-root"
                        },
                        new Dictionary<string, object?>(StringComparer.Ordinal)
                        {
                            ["kind"] = "animator",
                            ["clipId"] = "animation.potes-shutter"
                        }
                    }
                }
            };
        }

        private static IReadOnlyDictionary<string, object?> Vector(long x, long y, long z)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["x"] = x,
                ["y"] = y,
                ["z"] = z
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
                var requestId = "h1-01.reference." + (++_sequence).ToString(System.Globalization.CultureInfo.InvariantCulture);
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
                var major = int.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
                var minor = int.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
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
                            ["major"] = major,
                            ["minimumMinor"] = minor,
                            ["maximumMinor"] = minor
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
                            ["name"] = "arkus.h1-01.conformance",
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
