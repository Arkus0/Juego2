using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk07AExternalClientTests
    {
        [Fact]
        public void FreshExternalClientDiscoversAndExercisesAcceptedH0SurfaceEndToEnd()
        {
            using var source = new ExternalReferenceClient();
            var discovery = source.Invoke("system.describe", Empty());
            var capabilityNames = discovery.GetProperty("capabilities").EnumerateArray()
                .Select(value => value.GetProperty("name").GetString()!)
                .ToHashSet(StringComparer.Ordinal);
            var required = new[]
            {
                "world.summary",
                "world.object.get",
                "world.validation.current",
                "authoring.change.plan",
                "authoring.change.apply",
                "authoring.journal.read",
                "authoring.snapshot.export",
                "authoring.snapshot.import",
                "authoring.diff.compare",
                "authoring.journal.replay"
            };
            foreach (var capability in required) Assert.Contains(capability, capabilityNames);

            var baseSummary = source.Invoke("world.summary", Empty()).GetProperty("world");
            var validation = source.Invoke("world.validation.current", Empty());
            Assert.True(validation.GetProperty("valid").GetBoolean());
            var baseSnapshot = source.Invoke("authoring.snapshot.export", Empty());

            var operations = PotesOperations();
            var plan = source.Invoke(
                "authoring.change.plan",
                MutationRequest(baseSummary, "request.hk07a.potes.plan", operations));
            Assert.Equal("plan", plan.GetProperty("mode").GetString());
            Assert.False(plan.GetProperty("persisted").GetBoolean());

            var applied = source.Invoke(
                "authoring.change.apply",
                MutationRequest(baseSummary, "request.hk07a.potes.apply", operations));
            Assert.Equal("apply", applied.GetProperty("mode").GetString());
            Assert.True(applied.GetProperty("persisted").GetBoolean());

            var finalSummary = source.Invoke("world.summary", Empty()).GetProperty("world");
            var npc = source.Invoke(
                "world.object.get",
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["revision"] = finalSummary.GetProperty("revision").GetInt64(),
                    ["hash"] = finalSummary.GetProperty("hash").GetString(),
                    ["id"] = "npc.ana",
                    ["fields"] = new[] { "typeId", "containerId" }
                });
            Assert.Equal("npc.ana", npc.GetProperty("object").GetProperty("id").GetString());

            var journal = source.Invoke("authoring.journal.read", Empty());
            Assert.Equal(1, journal.GetProperty("entryCount").GetInt32());
            var finalSnapshot = source.Invoke("authoring.snapshot.export", Empty());
            var changed = source.Invoke(
                "authoring.diff.compare",
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["base"] = baseSnapshot,
                    ["target"] = finalSnapshot
                });
            Assert.False(changed.GetProperty("sameAuthorableState").GetBoolean());
            Assert.True(changed.GetProperty("changes").GetArrayLength() >= 4);

            using var target = new ExternalReferenceClient();
            var targetBootstrap = target.Invoke("world.summary", Empty()).GetProperty("world");
            target.Invoke(
                "authoring.snapshot.import",
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["idempotencyKey"] = "request.hk07a.base-import",
                    ["expectedRevision"] = targetBootstrap.GetProperty("revision").GetInt64(),
                    ["expectedHash"] = targetBootstrap.GetProperty("hash").GetString(),
                    ["snapshot"] = baseSnapshot
                });
            var replayBase = target.Invoke("world.summary", Empty()).GetProperty("world");
            var replayed = target.Invoke(
                "authoring.journal.replay",
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["expectedRevision"] = replayBase.GetProperty("revision").GetInt64(),
                    ["expectedHash"] = replayBase.GetProperty("hash").GetString(),
                    ["journal"] = journal
                });
            Assert.Equal(1, replayed.GetProperty("replayedEntries").GetInt32());

            var replayedSnapshot = target.Invoke("authoring.snapshot.export", Empty());
            var equivalent = target.Invoke(
                "authoring.diff.compare",
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["base"] = finalSnapshot,
                    ["target"] = replayedSnapshot
                });
            Assert.True(equivalent.GetProperty("sameAuthorableState").GetBoolean());
            Assert.Equal(0, equivalent.GetProperty("changes").GetArrayLength());
            Assert.Equal(
                finalSummary.GetProperty("hash").GetString(),
                replayed.GetProperty("targetCurrent").GetProperty("hash").GetString());
        }

        private static IReadOnlyList<object?> PotesOperations()
        {
            return new object?[]
            {
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "put-object",
                    ["id"] = "place.plaza",
                    ["typeId"] = "fixture.place",
                    ["references"] = Array.Empty<object?>()
                },
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "put-object",
                    ["id"] = "building.market",
                    ["typeId"] = "fixture.market",
                    ["containerId"] = "place.plaza",
                    ["references"] = Array.Empty<object?>()
                },
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "put-object",
                    ["id"] = "npc.ana",
                    ["typeId"] = "fixture.npc",
                    ["containerId"] = "place.plaza",
                    ["references"] = new object?[]
                    {
                        new Dictionary<string, object?>(StringComparer.Ordinal)
                        {
                            ["kind"] = "works.at",
                            ["targetId"] = "building.market"
                        }
                    }
                },
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "put-extension",
                    ["owner"] = "future.social",
                    ["schemaVersion"] = 1,
                    ["subjectId"] = "npc.ana",
                    ["payloadBase64"] = "ECAw",
                    ["dependencies"] = new object?[]
                    {
                        new Dictionary<string, object?>(StringComparer.Ordinal)
                        {
                            ["kind"] = "works.at",
                            ["targetId"] = "building.market"
                        }
                    }
                }
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

        private static IReadOnlyDictionary<string, object?> Empty() =>
            new Dictionary<string, object?>(StringComparer.Ordinal);

        private sealed class ExternalReferenceClient : IDisposable
        {
            private readonly Process _process;
            private readonly System.Threading.Tasks.Task<string> _error;
            private int _sequence;
            private bool _disposed;

            public ExternalReferenceClient()
            {
                _process = Hk07AProcessHarness.Start();
                _error = _process.StandardError.ReadToEndAsync();
            }

            public JsonElement Invoke(string capability, object arguments)
            {
                var requestId = "external." + (++_sequence).ToString(System.Globalization.CultureInfo.InvariantCulture);
                _process.StandardInput.WriteLine(Hk07AProcessHarness.Frame(requestId, capability, arguments));
                _process.StandardInput.Flush();
                var line = _process.StandardOutput.ReadLine();
                Assert.False(string.IsNullOrEmpty(line));
                using var document = JsonDocument.Parse(line!);
                var root = document.RootElement;
                Assert.Equal("arkus.reference.jsonl@1", root.GetProperty("protocol").GetString());
                var response = root.GetProperty("response");
                Assert.Equal(requestId, response.GetProperty("requestId").GetString());
                Assert.True(
                    response.GetProperty("status").GetString() == "success",
                    response.TryGetProperty("error", out var error)
                        ? error.GetProperty("machineCode").GetString()
                        : "missing response error");
                return response.GetProperty("result").Clone();
            }

            public void Dispose()
            {
                if (_disposed) return;
                _process.StandardInput.Close();
                Assert.True(_process.WaitForExit(30000), "External Arkus client host did not exit.");
                Assert.Equal(0, _process.ExitCode);
                Assert.Equal(string.Empty, _error.Result);
                _process.Dispose();
                _disposed = true;
            }
        }
    }
}
