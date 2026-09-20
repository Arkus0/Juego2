using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk07BMcpConformanceTests
    {
        private const string CanonicalDefinitionMetaKey = "dev.arkus/canonicalDefinition";
        private const string CanonicalKeyMetaKey = "dev.arkus/canonicalKey";
        private const string NeutralProjectionMetaKey = "dev.arkus/neutralProjection";
        private const string TimeoutMetaKey = "dev.arkus/timeoutMilliseconds";

        [Fact]
        public void McpDiscoveryMatchesIndependentCanonicalInventoryAndSchemas()
        {
            using var client = new McpWireClient();
            var tools = client.ListTools();
            var byCanonicalKey = ToolsByCanonicalKey(tools);

            var initial = new WorldState(
                new WorldId(ProductionHarnessHost.InitialWorldId),
                0,
                Array.Empty<WorldObject>());
            var session = new PortableWorldAuthoringSession(initial);
            var independentlyComposed = CanonicalWorldContract.Compose(
                new WorldInspectionService(session),
                session);

            Assert.Equal(independentlyComposed.Definitions.Count, byCanonicalKey.Count);
            foreach (var definition in independentlyComposed.Definitions)
            {
                var canonicalKey = definition.Key.ToString();
                Assert.True(byCanonicalKey.TryGetValue(canonicalKey, out var tool), "MCP omitted canonical capability " + canonicalKey);
                Assert.Equal("arkus.neutral-projection@1", tool.GetProperty("_meta").GetProperty(NeutralProjectionMetaKey).GetString());
                AssertJsonEquivalent(
                    JsonSerializer.SerializeToElement(definition.ToData()),
                    tool.GetProperty("_meta").GetProperty(CanonicalDefinitionMetaKey));

                var expectedInput = definition.RequestSchema == null
                    ? JsonSerializer.SerializeToElement(new Dictionary<string, object?>(StringComparer.Ordinal))
                    : JsonSerializer.SerializeToElement(definition.RequestSchema.ToData());
                AssertJsonEquivalent(expectedInput, tool.GetProperty("inputSchema"));
            }
        }

        [Fact]
        public void McpAndReferenceTransportRemainEquivalentAcrossAcceptedH0Flows()
        {
            using var reference = new ReferenceNeutralClient();
            using var mcp = new McpWireClient();

            var summary = InvokeEquivalent(reference, mcp, "world.summary", Empty());
            var validation = InvokeEquivalent(reference, mcp, "world.validation.current", Empty());
            Assert.True(Result(validation.Reference).GetProperty("valid").GetBoolean());
            var baseSnapshot = InvokeEquivalent(reference, mcp, "authoring.snapshot.export", Empty());

            var referenceMutation = MutationRequest(
                Result(summary.Reference).GetProperty("world"),
                "request.hk07b.potes.apply",
                PotesOperations());
            var mcpMutation = MutationRequest(
                Result(summary.Mcp).GetProperty("world"),
                "request.hk07b.potes.apply",
                PotesOperations());
            var applied = InvokeEquivalent(
                reference,
                mcp,
                "authoring.change.apply",
                referenceMutation,
                mcpMutation);
            Assert.True(Result(applied.Reference).GetProperty("persisted").GetBoolean());

            var finalSummary = InvokeEquivalent(reference, mcp, "world.summary", Empty());
            var referenceObjectRequest = ObjectRequest(Result(finalSummary.Reference).GetProperty("world"));
            var mcpObjectRequest = ObjectRequest(Result(finalSummary.Mcp).GetProperty("world"));
            var npc = InvokeEquivalent(reference, mcp, "world.object.get", referenceObjectRequest, mcpObjectRequest);
            Assert.Equal("npc.ana", Result(npc.Reference).GetProperty("object").GetProperty("id").GetString());

            var journal = InvokeEquivalent(reference, mcp, "authoring.journal.read", Empty());
            Assert.Equal(1, Result(journal.Reference).GetProperty("entryCount").GetInt32());
            var finalSnapshot = InvokeEquivalent(reference, mcp, "authoring.snapshot.export", Empty());
            var changed = InvokeEquivalent(
                reference,
                mcp,
                "authoring.diff.compare",
                DiffRequest(Result(baseSnapshot.Reference), Result(finalSnapshot.Reference)),
                DiffRequest(Result(baseSnapshot.Mcp), Result(finalSnapshot.Mcp)));
            Assert.False(Result(changed.Reference).GetProperty("sameAuthorableState").GetBoolean());

            using var referenceTarget = new ReferenceNeutralClient();
            using var mcpTarget = new McpWireClient();
            var targetStart = InvokeEquivalent(referenceTarget, mcpTarget, "world.summary", Empty());
            var referenceImport = SnapshotImportRequest(
                Result(targetStart.Reference).GetProperty("world"),
                Result(baseSnapshot.Reference));
            var mcpImport = SnapshotImportRequest(
                Result(targetStart.Mcp).GetProperty("world"),
                Result(baseSnapshot.Mcp));
            InvokeEquivalent(referenceTarget, mcpTarget, "authoring.snapshot.import", referenceImport, mcpImport);

            var replayBase = InvokeEquivalent(referenceTarget, mcpTarget, "world.summary", Empty());
            var replayed = InvokeEquivalent(
                referenceTarget,
                mcpTarget,
                "authoring.journal.replay",
                ReplayRequest(Result(replayBase.Reference).GetProperty("world"), Result(journal.Reference)),
                ReplayRequest(Result(replayBase.Mcp).GetProperty("world"), Result(journal.Mcp)));
            Assert.Equal(1, Result(replayed.Reference).GetProperty("replayedEntries").GetInt32());

            var replayedSnapshot = InvokeEquivalent(referenceTarget, mcpTarget, "authoring.snapshot.export", Empty());
            var equivalent = InvokeEquivalent(
                referenceTarget,
                mcpTarget,
                "authoring.diff.compare",
                DiffRequest(Result(finalSnapshot.Reference), Result(replayedSnapshot.Reference)),
                DiffRequest(Result(finalSnapshot.Mcp), Result(replayedSnapshot.Mcp)));
            Assert.True(Result(equivalent.Reference).GetProperty("sameAuthorableState").GetBoolean());
            Assert.Equal(0, Result(equivalent.Reference).GetProperty("changes").GetArrayLength());

            var invalid = InvokeEquivalent(reference, mcp, "world.object.get", Empty());
            Assert.Equal("canonical", invalid.Reference.GetProperty("failureKind").GetString());
            Assert.False(invalid.Mcp.GetProperty("status").GetString() == "success");

            var timeoutReference = reference.InvokeOutcome("world.summary", Empty(), 0);
            var timeoutMcp = mcp.InvokeOutcome("world.summary", Empty(), 0);
            AssertNeutralEquivalent(timeoutReference, timeoutMcp);
            Assert.Equal("timed-out", timeoutReference.GetProperty("failureKind").GetString());
            Assert.Equal("projection.timeout", timeoutReference.GetProperty("error").GetProperty("machineCode").GetString());
        }

        [Fact]
        public void SyntheticScopedProviderProjectsWithoutMcpRegistry()
        {
            var composition = ContractComposer.Compose(
                BaseContract.CreateContribution(),
                new[] { Hk01TestFixtures.FixtureProvider() });
            Assert.True(composition.Success);
            using var projection = new NeutralProjectionService(composition.Contract!);

            var root = Hk07AProcessHarness.FindRepositoryRoot();
            var assemblyPath = Path.Combine(
                root,
                "src",
                "Arkus.Harness.Mcp",
                "bin",
                "Release",
                "net8.0",
                "Arkus.Harness.Mcp.dll");
            Assert.True(File.Exists(assemblyPath), "Release MCP assembly is missing: " + assemblyPath);
            var assembly = Assembly.LoadFrom(assemblyPath);
            var adapterType = assembly.GetType("Arkus.Harness.Mcp.McpProjectionAdapter", throwOnError: true)!;
            var adapter = Activator.CreateInstance(adapterType, projection)!;
            var describe = adapterType.GetMethod("DescribeCapabilities", BindingFlags.Public | BindingFlags.Instance)!;
            var values = ((IEnumerable)describe.Invoke(adapter, null)!).Cast<object>().ToArray();
            var keys = values.Select(value =>
            {
                var definition = (CapabilityDefinition)value.GetType().GetProperty("Definition")!.GetValue(value)!;
                return definition.Key.ToString();
            }).ToArray();

            Assert.Contains("engine.observe@1.0", keys);
            Assert.Equal(composition.Contract!.Definitions.Count, keys.Length);
            Assert.Equal(keys.Length, keys.Distinct(StringComparer.Ordinal).Count());
        }

        [Fact]
        public void CausalNegativeControlsDetectInventoryAndSchemaDrift()
        {
            using var client = new McpWireClient();
            var tools = client.ListTools();
            var actualKeys = tools.EnumerateArray()
                .Select(value => value.GetProperty("_meta").GetProperty(CanonicalKeyMetaKey).GetString()!)
                .ToArray();
            using var projection = ProductionHarnessHost.Create();
            var expectedKeys = projection.Capabilities.Select(value => value.Key.ToString()).ToArray();

            Assert.Empty(InventoryIssues(expectedKeys, actualKeys));

            var missing = actualKeys.Skip(1).ToArray();
            Assert.Contains("missing:" + actualKeys[0], InventoryIssues(expectedKeys, missing));

            var extra = actualKeys.Concat(new[] { "adapter.only@1.0" }).ToArray();
            Assert.Contains("extra:adapter.only@1.0", InventoryIssues(expectedKeys, extra));

            var duplicate = actualKeys.Concat(new[] { actualKeys[0] }).ToArray();
            Assert.Contains("duplicate:" + actualKeys[0], InventoryIssues(expectedKeys, duplicate));

            var first = tools.EnumerateArray().First();
            var key = first.GetProperty("_meta").GetProperty(CanonicalKeyMetaKey).GetString()!;
            var expected = projection.Capabilities.Single(value => value.Key.ToString() == key);
            var actualDefinition = (JsonObject)JsonNode.Parse(
                first.GetProperty("_meta").GetProperty(CanonicalDefinitionMetaKey).GetRawText())!;
            Assert.True(JsonNode.DeepEquals(JsonSerializer.SerializeToNode(expected.ToData()), actualDefinition));
            actualDefinition["successSchema"] = new JsonObject { ["type"] = "string" };
            Assert.False(JsonNode.DeepEquals(JsonSerializer.SerializeToNode(expected.ToData()), actualDefinition));
        }

        [Fact]
        public void SdkAndMcpFramingRemainOutsideCanonicalLayers()
        {
            var root = Hk07AProcessHarness.FindRepositoryRoot();
            var canonicalProjects = new[]
            {
                "src/Arkus.Harness.Protocol/Arkus.Harness.Protocol.csproj",
                "src/Arkus.Harness.Runtime/Arkus.Harness.Runtime.csproj",
                "src/Arkus.Harness.Projection/Arkus.Harness.Projection.csproj"
            };
            foreach (var relative in canonicalProjects)
            {
                var text = File.ReadAllText(Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar)));
                Assert.DoesNotContain("ModelContextProtocol", text, StringComparison.Ordinal);
            }

            var mcpProject = File.ReadAllText(Path.Combine(root, "src", "Arkus.Harness.Mcp", "Arkus.Harness.Mcp.csproj"));
            Assert.Contains("ModelContextProtocol.Core", mcpProject, StringComparison.Ordinal);

            using var client = new McpWireClient();
            var summaryTool = ToolsByCanonicalKey(client.ListTools())["world.summary@1.0"];
            Assert.False(summaryTool.GetProperty("inputSchema").TryGetProperty(TimeoutMetaKey, out _));
            Assert.Equal("arkus.neutral-projection@1", summaryTool.GetProperty("_meta").GetProperty(NeutralProjectionMetaKey).GetString());

            var neutralSource = File.ReadAllText(Path.Combine(root, "src", "Arkus.Harness.Projection", "NeutralProjection.cs"));
            Assert.DoesNotContain("ModelContextProtocol", neutralSource, StringComparison.Ordinal);
            Assert.DoesNotContain(TimeoutMetaKey, neutralSource, StringComparison.Ordinal);
        }

        private static (JsonElement Reference, JsonElement Mcp) InvokeEquivalent(
            IProjectionWireClient reference,
            IProjectionWireClient mcp,
            string capability,
            object arguments)
        {
            return InvokeEquivalent(reference, mcp, capability, arguments, arguments);
        }

        private static (JsonElement Reference, JsonElement Mcp) InvokeEquivalent(
            IProjectionWireClient reference,
            IProjectionWireClient mcp,
            string capability,
            object referenceArguments,
            object mcpArguments)
        {
            var referenceOutcome = reference.InvokeOutcome(capability, referenceArguments);
            var mcpOutcome = mcp.InvokeOutcome(capability, mcpArguments);
            AssertNeutralEquivalent(referenceOutcome, mcpOutcome);
            return (referenceOutcome, mcpOutcome);
        }

        private static void AssertNeutralEquivalent(JsonElement reference, JsonElement mcp)
        {
            var referenceNode = (JsonObject)JsonNode.Parse(reference.GetRawText())!;
            var mcpNode = (JsonObject)JsonNode.Parse(mcp.GetRawText())!;
            referenceNode.Remove("requestId");
            mcpNode.Remove("requestId");
            Assert.True(
                JsonNode.DeepEquals(referenceNode, mcpNode),
                "Cross-transport semantic drift. Reference=" + referenceNode.ToJsonString() + " MCP=" + mcpNode.ToJsonString());
        }

        private static void AssertJsonEquivalent(JsonElement expected, JsonElement actual)
        {
            Assert.True(
                JsonNode.DeepEquals(JsonNode.Parse(expected.GetRawText()), JsonNode.Parse(actual.GetRawText())),
                "JSON semantic mismatch. Expected=" + expected.GetRawText() + " Actual=" + actual.GetRawText());
        }

        private static IReadOnlyList<string> InventoryIssues(IEnumerable<string> expected, IEnumerable<string> observed)
        {
            var expectedSet = new HashSet<string>(expected, StringComparer.Ordinal);
            var observedList = observed.ToArray();
            var observedSet = new HashSet<string>(observedList, StringComparer.Ordinal);
            var issues = expectedSet.Where(value => !observedSet.Contains(value)).Select(value => "missing:" + value)
                .Concat(observedSet.Where(value => !expectedSet.Contains(value)).Select(value => "extra:" + value))
                .Concat(observedList.GroupBy(value => value, StringComparer.Ordinal).Where(group => group.Count() > 1).Select(group => "duplicate:" + group.Key))
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToArray();
            return issues;
        }

        private static Dictionary<string, JsonElement> ToolsByCanonicalKey(JsonElement tools)
        {
            var result = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
            foreach (var tool in tools.EnumerateArray())
            {
                var key = tool.GetProperty("_meta").GetProperty(CanonicalKeyMetaKey).GetString();
                Assert.False(string.IsNullOrWhiteSpace(key));
                Assert.True(result.TryAdd(key!, tool.Clone()), "Duplicate MCP canonical key " + key);
            }
            return result;
        }

        private static JsonElement Result(JsonElement outcome)
        {
            Assert.Equal("success", outcome.GetProperty("status").GetString());
            return outcome.GetProperty("result");
        }

        private static IReadOnlyDictionary<string, object?> ObjectRequest(JsonElement world)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["revision"] = world.GetProperty("revision").GetInt64(),
                ["hash"] = world.GetProperty("hash").GetString(),
                ["id"] = "npc.ana",
                ["fields"] = new[] { "typeId", "containerId" }
            };
        }

        private static IReadOnlyDictionary<string, object?> SnapshotImportRequest(JsonElement world, JsonElement snapshot)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["idempotencyKey"] = "request.hk07b.base-import",
                ["expectedRevision"] = world.GetProperty("revision").GetInt64(),
                ["expectedHash"] = world.GetProperty("hash").GetString(),
                ["snapshot"] = snapshot
            };
        }

        private static IReadOnlyDictionary<string, object?> ReplayRequest(JsonElement world, JsonElement journal)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["expectedRevision"] = world.GetProperty("revision").GetInt64(),
                ["expectedHash"] = world.GetProperty("hash").GetString(),
                ["journal"] = journal
            };
        }

        private static IReadOnlyDictionary<string, object?> DiffRequest(JsonElement baseSnapshot, JsonElement targetSnapshot)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["base"] = baseSnapshot,
                ["target"] = targetSnapshot
            };
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

        private static IReadOnlyList<object?> PotesOperations()
        {
            return new object?[]
            {
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "put-object", ["id"] = "place.plaza", ["typeId"] = "fixture.place", ["references"] = Array.Empty<object?>()
                },
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "put-object", ["id"] = "building.market", ["typeId"] = "fixture.market", ["containerId"] = "place.plaza", ["references"] = Array.Empty<object?>()
                },
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "put-object", ["id"] = "npc.ana", ["typeId"] = "fixture.npc", ["containerId"] = "place.plaza",
                    ["references"] = new object?[]
                    {
                        new Dictionary<string, object?>(StringComparer.Ordinal) { ["kind"] = "works.at", ["targetId"] = "building.market" }
                    }
                },
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "put-extension", ["owner"] = "future.social", ["schemaVersion"] = 1, ["subjectId"] = "npc.ana", ["payloadBase64"] = "ECAw",
                    ["dependencies"] = new object?[]
                    {
                        new Dictionary<string, object?>(StringComparer.Ordinal) { ["kind"] = "works.at", ["targetId"] = "building.market" }
                    }
                }
            };
        }

        private static IReadOnlyDictionary<string, object?> Empty() =>
            new Dictionary<string, object?>(StringComparer.Ordinal);

        private interface IProjectionWireClient : IDisposable
        {
            JsonElement InvokeOutcome(string capability, object arguments, int? timeoutMilliseconds = null);
        }

        private sealed class ReferenceNeutralClient : IProjectionWireClient
        {
            private readonly Process _process;
            private readonly System.Threading.Tasks.Task<string> _error;
            private int _sequence;
            private bool _disposed;

            public ReferenceNeutralClient()
            {
                _process = Hk07AProcessHarness.Start();
                _error = _process.StandardError.ReadToEndAsync();
            }

            public JsonElement InvokeOutcome(string capability, object arguments, int? timeoutMilliseconds = null)
            {
                var requestId = "hk07b.reference." + (++_sequence).ToString(System.Globalization.CultureInfo.InvariantCulture);
                _process.StandardInput.WriteLine(Hk07AProcessHarness.Frame(requestId, capability, arguments, timeoutMilliseconds));
                _process.StandardInput.Flush();
                var line = _process.StandardOutput.ReadLine();
                Assert.False(string.IsNullOrWhiteSpace(line));
                using var document = JsonDocument.Parse(line!);
                return document.RootElement.GetProperty("response").Clone();
            }

            public void Dispose()
            {
                if (_disposed) return;
                _process.StandardInput.Close();
                Assert.True(_process.WaitForExit(30000), "Reference transport did not exit.");
                Assert.Equal(0, _process.ExitCode);
                Assert.Equal(string.Empty, _error.Result);
                _process.Dispose();
                _disposed = true;
            }
        }

        private sealed class McpWireClient : IProjectionWireClient
        {
            private readonly Process _process;
            private readonly System.Threading.Tasks.Task<string> _error;
            private readonly Dictionary<string, string> _toolNames;
            private int _sequence;
            private bool _disposed;

            public McpWireClient()
            {
                _process = StartMcpProcess();
                _error = _process.StandardError.ReadToEndAsync();
                var initialized = Request(
                    "initialize",
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["protocolVersion"] = "2025-11-25",
                        ["capabilities"] = new Dictionary<string, object?>(StringComparer.Ordinal),
                        ["clientInfo"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                        {
                            ["name"] = "arkus.hk07b.conformance",
                            ["version"] = "1.0.0"
                        }
                    });
                Assert.Equal("2025-11-25", initialized.GetProperty("result").GetProperty("protocolVersion").GetString());
                Notify("notifications/initialized", new Dictionary<string, object?>(StringComparer.Ordinal));
                var tools = ListTools();
                _toolNames = ToolsByCanonicalKey(tools).ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value.GetProperty("name").GetString()!,
                    StringComparer.Ordinal);
            }

            public JsonElement ListTools()
            {
                return Request("tools/list", new Dictionary<string, object?>(StringComparer.Ordinal))
                    .GetProperty("result").GetProperty("tools").Clone();
            }

            public JsonElement InvokeOutcome(string capability, object arguments, int? timeoutMilliseconds = null)
            {
                var canonicalKey = capability + "@1.0";
                Assert.True(_toolNames.TryGetValue(canonicalKey, out var toolName), "MCP discovery omitted " + canonicalKey);
                var parameters = new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["name"] = toolName,
                    ["arguments"] = arguments
                };
                if (timeoutMilliseconds.HasValue)
                {
                    parameters["_meta"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        [TimeoutMetaKey] = timeoutMilliseconds.Value
                    };
                }

                var response = Request("tools/call", parameters);
                Assert.False(response.TryGetProperty("error", out var rpcError), rpcError.ValueKind == JsonValueKind.Undefined ? "MCP JSON-RPC error" : rpcError.GetRawText());
                var result = response.GetProperty("result");
                Assert.True(result.TryGetProperty("structuredContent", out var structured), "MCP result omitted structuredContent: " + result.GetRawText());
                return structured.Clone();
            }

            private JsonElement Request(string method, object parameters)
            {
                var id = ++_sequence;
                var frame = JsonSerializer.Serialize(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["jsonrpc"] = "2.0",
                    ["id"] = id,
                    ["method"] = method,
                    ["params"] = parameters
                });
                _process.StandardInput.WriteLine(frame);
                _process.StandardInput.Flush();
                while (true)
                {
                    var line = _process.StandardOutput.ReadLine();
                    Assert.False(string.IsNullOrWhiteSpace(line), "MCP process ended before JSON-RPC response for " + method + ".");
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
                var frame = JsonSerializer.Serialize(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["jsonrpc"] = "2.0",
                    ["method"] = method,
                    ["params"] = parameters
                });
                _process.StandardInput.WriteLine(frame);
                _process.StandardInput.Flush();
            }

            public void Dispose()
            {
                if (_disposed) return;
                _process.StandardInput.Close();
                Assert.True(_process.WaitForExit(30000), "MCP stdio server did not exit.");
                Assert.Equal(0, _process.ExitCode);
                Assert.Equal(string.Empty, _error.Result);
                _process.Dispose();
                _disposed = true;
            }

            private static Process StartMcpProcess()
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
