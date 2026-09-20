using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class HkGateReadinessTests
    {
        private static readonly string[] RequiredCapabilityKeys =
        {
            "system.describe@1.0", "world.summary@1.0", "world.object.get@1.0",
            "world.object.query@1.0", "world.reference.query@1.0", "world.extension.query@1.0",
            "world.extension.read@1.0", "world.validation.current@1.0",
            "authoring.change.plan@1.0", "authoring.change.dry-run@1.0", "authoring.change.apply@1.0",
            "authoring.journal.read@1.0", "authoring.journal.read@2.0",
            "authoring.snapshot.export@1.0", "authoring.snapshot.import@1.0",
            "authoring.diff.compare@1.0", "authoring.journal.replay@1.0"
        };

        private static readonly string[] GateStepUniverse =
        {
            "01-discovery-and-schemas",
            "02-create-representative-micro-world",
            "03-bounded-inspection-and-query",
            "04-plan-dry-run-atomic-apply",
            "05-invalid-diagnostics-repair",
            "06-stale-recovery-bounded-replan",
            "07-hk08a-coherent-batch",
            "08-export-snapshot-and-provenance",
            "09-clean-restart-replay",
            "10-semantic-diff-and-provenance-chain",
            "11-reference-mcp-semantic-surface",
            "12-host-authority-and-resource-envelope",
            "13-bounded-endurance",
            "14-headless-full-validation"
        };

        [Fact]
        public void PublicReferenceClientCompletesDeterministicGateScenarioWithoutPrivateProductCalls()
        {
            using var source = new ReferenceGateClient("gate.source");

            var discovery = Success(source.Invoke("system.describe", Empty()));
            var capabilityKeys = new HashSet<string>(StringComparer.Ordinal);
            foreach (var capability in discovery.GetProperty("capabilities").EnumerateArray())
            {
                var key = capability.GetProperty("name").GetString() + "@" + capability.GetProperty("version").GetString();
                Assert.True(capabilityKeys.Add(key), "Duplicate public discovery identity " + key);
                Assert.Equal(JsonValueKind.Object, capability.GetProperty("requestSchema").ValueKind);
                Assert.Equal(JsonValueKind.Object, capability.GetProperty("successSchema").ValueKind);
                Assert.Equal(JsonValueKind.Object, capability.GetProperty("errorSchema").ValueKind);
            }
            foreach (var key in RequiredCapabilityKeys) Assert.Contains(key, capabilityKeys);

            var initialSummary = Success(source.Invoke("world.summary", Empty())).GetProperty("world");
            var initialSnapshot = Success(source.Invoke("authoring.snapshot.export", Empty()));

            var representativeOperations = RepresentativePotesOperations();
            var representativeRequest = MutationRequest(initialSummary, "request.hkgate.representative", representativeOperations);
            var planned = Success(source.Invoke("authoring.change.plan", representativeRequest));
            Assert.Equal(4, planned.GetProperty("plan").GetProperty("changes").GetArrayLength());
            Assert.False(planned.GetProperty("persisted").GetBoolean());
            var dryRun = Success(source.Invoke("authoring.change.dry-run", representativeRequest));
            Assert.Equal(4, dryRun.GetProperty("plan").GetProperty("changes").GetArrayLength());
            Assert.False(dryRun.GetProperty("persisted").GetBoolean());
            var applied = Success(source.Invoke("authoring.change.apply", representativeRequest));
            Assert.True(applied.GetProperty("persisted").GetBoolean());
            var currentAnchor = applied.GetProperty("plan").GetProperty("result");

            var query = Success(source.Invoke("world.object.query", new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["revision"] = currentAnchor.GetProperty("revision").GetInt64(),
                ["hash"] = currentAnchor.GetProperty("hash").GetString(),
                ["limit"] = 20,
                ["filter"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["typeIds"] = new[] { "fixture.gate.place", "fixture.gate.market", "fixture.gate.npc" }
                },
                ["fields"] = new[] { "typeId", "containerId" }
            }));
            Assert.Equal(3, query.GetProperty("items").GetArrayLength());
            Assert.False(query.TryGetProperty("nextCursor", out _));

            var npc = Success(source.Invoke("world.object.get", new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["revision"] = currentAnchor.GetProperty("revision").GetInt64(),
                ["hash"] = currentAnchor.GetProperty("hash").GetString(),
                ["id"] = "gate.npc.ana",
                ["fields"] = new[] { "typeId", "containerId" }
            }));
            Assert.Equal("gate.npc.ana", npc.GetProperty("object").GetProperty("id").GetString());
            Assert.Equal("gate.place.plaza", npc.GetProperty("object").GetProperty("containerId").GetString());

            var references = Success(source.Invoke("world.reference.query", new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["revision"] = currentAnchor.GetProperty("revision").GetInt64(),
                ["hash"] = currentAnchor.GetProperty("hash").GetString(),
                ["limit"] = 20,
                ["filter"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["sourceIds"] = new[] { "gate.npc.ana" },
                    ["kinds"] = new[] { "works.at" },
                    ["targetIds"] = new[] { "gate.building.market" }
                }
            }));
            Assert.Equal(1, references.GetProperty("items").GetArrayLength());
            var reference = references.GetProperty("items")[0];
            Assert.Equal("gate.npc.ana", reference.GetProperty("sourceId").GetString());
            Assert.Equal("works.at", reference.GetProperty("kind").GetString());
            Assert.Equal("gate.building.market", reference.GetProperty("targetId").GetString());

            var extensions = Success(source.Invoke("world.extension.query", new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["revision"] = currentAnchor.GetProperty("revision").GetInt64(),
                ["hash"] = currentAnchor.GetProperty("hash").GetString(),
                ["limit"] = 20
            }));
            Assert.Equal(1, extensions.GetProperty("items").GetArrayLength());
            var extensionDescriptor = extensions.GetProperty("items")[0];
            Assert.Equal("future.gate.social", extensionDescriptor.GetProperty("owner").GetString());
            Assert.Equal(1, extensionDescriptor.GetProperty("schemaVersion").GetInt32());
            Assert.Equal("gate.npc.ana", extensionDescriptor.GetProperty("subjectId").GetString());

            var extension = Success(source.Invoke("world.extension.read", new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["revision"] = currentAnchor.GetProperty("revision").GetInt64(),
                ["hash"] = currentAnchor.GetProperty("hash").GetString(),
                ["owner"] = "future.gate.social",
                ["schemaVersion"] = 1,
                ["subjectId"] = "gate.npc.ana"
            }));
            Assert.Equal("ECAw", extension.GetProperty("payloadBase64").GetString());
            Assert.Equal(1, extension.GetProperty("dependencyCount").GetInt32());
            Assert.Equal("works.at", extension.GetProperty("dependencies")[0].GetProperty("kind").GetString());
            Assert.Equal("gate.building.market", extension.GetProperty("dependencies")[0].GetProperty("targetId").GetString());

            var validation = Success(source.Invoke("world.validation.current", Empty()));
            Assert.True(validation.GetProperty("valid").GetBoolean());
            Assert.Equal(0, validation.GetProperty("diagnosticCount").GetInt32());

            var invalid = source.Invoke("authoring.change.apply", MutationRequest(
                currentAnchor,
                "request.hkgate.invalid",
                new object?[] { PutObject("gate.invalid.child", "fixture.gate.invalid", "gate.invalid.missing-parent") }));
            Assert.Equal("error", invalid.GetProperty("status").GetString());
            Assert.Equal("world.change.invalid_candidate", invalid.GetProperty("error").GetProperty("machineCode").GetString());
            var invalidValidation = invalid.GetProperty("error").GetProperty("context").GetProperty("validation");
            var invalidDiagnosticCount = invalidValidation.GetProperty("diagnosticCount").GetInt32();
            Assert.True(invalidDiagnosticCount > 0);
            Assert.Equal(invalidDiagnosticCount, invalidValidation.GetProperty("diagnostics").GetArrayLength());

            var afterInvalid = Success(source.Invoke("world.summary", Empty())).GetProperty("world");
            Assert.Equal(currentAnchor.GetProperty("revision").GetInt64(), afterInvalid.GetProperty("revision").GetInt64());
            Assert.Equal(currentAnchor.GetProperty("hash").GetString(), afterInvalid.GetProperty("hash").GetString());

            var repaired = Success(source.Invoke("authoring.change.apply", MutationRequest(
                currentAnchor,
                "request.hkgate.repair",
                new object?[]
                {
                    PutObject("gate.invalid.missing-parent", "fixture.gate.repair-root"),
                    PutObject("gate.invalid.child", "fixture.gate.invalid", "gate.invalid.missing-parent")
                })));
            var recoveryBase = repaired.GetProperty("plan").GetProperty("result");

            var writer = Success(source.Invoke("authoring.change.apply", MutationRequest(
                recoveryBase,
                "request.hkgate.concurrent-writer",
                new object?[] { PutObject("gate.concurrent.writer", "fixture.gate.writer") })));
            var writerAnchor = writer.GetProperty("plan").GetProperty("result");

            var recoveryRequestOffset = source.RequestCount;
            var stale = source.Invoke("authoring.change.plan", MutationRequest(
                recoveryBase,
                "request.hkgate.concurrent-client",
                new object?[] { PutObject("gate.concurrent.client", "fixture.gate.client") }));
            Assert.Equal("error", stale.GetProperty("status").GetString());
            Assert.Equal("world.change.stale_revision", stale.GetProperty("error").GetProperty("machineCode").GetString());
            var recovery = stale.GetProperty("error").GetProperty("context").GetProperty("recovery");
            Assert.Equal("arkus.world-conflict-recovery@1", recovery.GetProperty("schemaId").GetString());
            Assert.Equal("same-lineage-replan", recovery.GetProperty("disposition").GetString());
            Assert.Equal(writerAnchor.GetProperty("hash").GetString(), recovery.GetProperty("current").GetProperty("hash").GetString());

            var recoveryInspectionRequests = 0;
            foreach (var descriptor in recovery.GetProperty("currentResources").EnumerateArray())
            {
                if (!descriptor.TryGetProperty("inspection", out var inspection)) continue;
                Success(source.Invoke(
                    inspection.GetProperty("name").GetString()!,
                    inspection.GetProperty("request").Clone(),
                    inspection.GetProperty("version").GetString()!));
                recoveryInspectionRequests++;
            }
            Assert.True(recoveryInspectionRequests > 0);

            var recoveredRequest = MutationRequest(
                recovery.GetProperty("current"),
                "request.hkgate.concurrent-client",
                new object?[] { PutObject("gate.concurrent.client", "fixture.gate.client") });
            Success(source.Invoke("authoring.change.plan", recoveredRequest));
            Success(source.Invoke("authoring.change.dry-run", recoveredRequest));
            var recovered = Success(source.Invoke("authoring.change.apply", recoveredRequest));
            currentAnchor = recovered.GetProperty("plan").GetProperty("result");

            var recoveryCapabilities = source.CapabilitiesFrom(recoveryRequestOffset);
            Assert.DoesNotContain("world.summary", recoveryCapabilities);
            Assert.DoesNotContain("world.object.query", recoveryCapabilities);
            Assert.DoesNotContain("world.extension.query", recoveryCapabilities);

            var batchOperations = RepresentativeNinetySixOperationIntent();
            Assert.Equal(96, batchOperations.Count);
            var batchRequest = MutationRequest(currentAnchor, "request.hkgate.coherent-batch", batchOperations);
            var batchPlan = Success(source.Invoke("authoring.change.plan", batchRequest));
            Assert.Equal(96, batchPlan.GetProperty("plan").GetProperty("changes").GetArrayLength());
            Assert.False(Success(source.Invoke("authoring.change.dry-run", batchRequest)).GetProperty("persisted").GetBoolean());
            var batchApplied = Success(source.Invoke("authoring.change.apply", batchRequest));
            Assert.True(batchApplied.GetProperty("persisted").GetBoolean());
            Assert.Equal(96, batchApplied.GetProperty("plan").GetProperty("changes").GetArrayLength());

            var finalSummary = Success(source.Invoke("world.summary", Empty())).GetProperty("world");
            var finalSnapshot = Success(source.Invoke("authoring.snapshot.export", Empty()));
            var journal = Success(source.Invoke("authoring.journal.read", Empty()));
            var journalEntryCount = journal.GetProperty("entryCount").GetInt32();
            Assert.Equal(5, journalEntryCount);
            Assert.Equal(journalEntryCount, journal.GetProperty("entries").GetArrayLength());

            var pagedJournalEntries = 0;
            var pageCount = 0;
            string? cursor = null;
            do
            {
                var pageRequest = new Dictionary<string, object?>(StringComparer.Ordinal) { ["limit"] = 2 };
                if (cursor != null) pageRequest["cursor"] = cursor;
                var page = Success(source.Invoke("authoring.journal.read", pageRequest, "2.0"));
                pagedJournalEntries += page.GetProperty("entries").GetArrayLength();
                pageCount++;
                cursor = page.TryGetProperty("nextCursor", out var next) ? next.GetString() : null;
            }
            while (cursor != null);
            Assert.Equal(journalEntryCount, pagedJournalEntries);
            Assert.Equal(3, pageCount);

            var changed = Success(source.Invoke("authoring.diff.compare", new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["base"] = initialSnapshot,
                ["target"] = finalSnapshot
            }));
            Assert.False(changed.GetProperty("sameAuthorableState").GetBoolean());
            Assert.True(changed.GetProperty("changes").GetArrayLength() >= 100);

            using var restarted = new ReferenceGateClient("gate.restart");
            var restartBootstrap = Success(restarted.Invoke("world.summary", Empty())).GetProperty("world");
            Success(restarted.Invoke("authoring.snapshot.import", new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["idempotencyKey"] = "request.hkgate.restart-base",
                ["expectedRevision"] = restartBootstrap.GetProperty("revision").GetInt64(),
                ["expectedHash"] = restartBootstrap.GetProperty("hash").GetString(),
                ["snapshot"] = initialSnapshot
            }));
            var replayBase = Success(restarted.Invoke("world.summary", Empty())).GetProperty("world");
            var replayed = Success(restarted.Invoke("authoring.journal.replay", new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["expectedRevision"] = replayBase.GetProperty("revision").GetInt64(),
                ["expectedHash"] = replayBase.GetProperty("hash").GetString(),
                ["journal"] = journal
            }));
            Assert.Equal(journalEntryCount, replayed.GetProperty("replayedEntries").GetInt32());
            Assert.Equal(finalSummary.GetProperty("hash").GetString(), replayed.GetProperty("targetCurrent").GetProperty("hash").GetString());

            var replayedSnapshot = Success(restarted.Invoke("authoring.snapshot.export", Empty()));
            var equivalent = Success(restarted.Invoke("authoring.diff.compare", new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["base"] = finalSnapshot,
                ["target"] = replayedSnapshot
            }));
            Assert.True(equivalent.GetProperty("sameAuthorableState").GetBoolean());
            Assert.Equal(0, equivalent.GetProperty("changes").GetArrayLength());

            EmitTranscript(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = "arkus.hk-gate.reference-scenario@1",
                ["client"] = "reference-jsonl-clean-process",
                ["publicTransportOnly"] = true,
                ["sourceReadRequired"] = false,
                ["gateStepUniverse"] = GateStepUniverse,
                ["discoveredCapabilityCount"] = capabilityKeys.Count,
                ["requiredCapabilityKeys"] = RequiredCapabilityKeys,
                ["representativeOperationCount"] = representativeOperations.Count,
                ["invalidDiagnosticCount"] = invalidDiagnosticCount,
                ["recoveryChangedResourceCount"] = recovery.GetProperty("changedResources").GetArrayLength(),
                ["recoveryInspectionRequestCount"] = recoveryInspectionRequests,
                ["recoveryFullWorldReloadCount"] = 0,
                ["coherentBatchOperationCount"] = batchOperations.Count,
                ["journalEntryCount"] = journalEntryCount,
                ["journalPageCount"] = pageCount,
                ["initialRevision"] = initialSummary.GetProperty("revision").GetInt64(),
                ["initialHash"] = initialSummary.GetProperty("hash").GetString(),
                ["staleExpectedRevision"] = recovery.GetProperty("expected").GetProperty("revision").GetInt64(),
                ["staleCurrentRevision"] = recovery.GetProperty("current").GetProperty("revision").GetInt64(),
                ["finalRevision"] = finalSummary.GetProperty("revision").GetInt64(),
                ["finalHash"] = finalSummary.GetProperty("hash").GetString(),
                ["replayedFinalHash"] = replayed.GetProperty("targetCurrent").GetProperty("hash").GetString(),
                ["semanticDiffChangeCount"] = changed.GetProperty("changes").GetArrayLength(),
                ["sourceRequestCount"] = source.RequestCount,
                ["sourceSerializedResponseBytes"] = source.SerializedResponseBytes,
                ["restartRequestCount"] = restarted.RequestCount
            });
        }

        [Fact]
        public void GateStepUniverseIsExplicitAndCannotSilentlyOmitARequiredScenarioStage()
        {
            Assert.Equal(14, GateStepUniverse.Length);
            Assert.Equal(14, GateStepUniverse.Distinct(StringComparer.Ordinal).Count());
            Assert.Equal("01-discovery-and-schemas", GateStepUniverse[0]);
            Assert.Equal("14-headless-full-validation", GateStepUniverse[13]);
        }

        private static JsonElement Success(JsonElement response)
        {
            var status = response.GetProperty("status").GetString();
            var detail = response.TryGetProperty("error", out var error) && error.TryGetProperty("machineCode", out var machineCode)
                ? machineCode.GetString()
                : "missing response error";
            Assert.True(string.Equals("success", status, StringComparison.Ordinal), detail);
            return response.GetProperty("result").Clone();
        }

        private static IReadOnlyList<object?> RepresentativePotesOperations()
        {
            return new object?[]
            {
                PutObject("gate.place.plaza", "fixture.gate.place"),
                PutObject("gate.building.market", "fixture.gate.market", "gate.place.plaza"),
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "put-object", ["id"] = "gate.npc.ana", ["typeId"] = "fixture.gate.npc",
                    ["containerId"] = "gate.place.plaza",
                    ["references"] = new object?[]
                    {
                        new Dictionary<string, object?>(StringComparer.Ordinal)
                        {
                            ["kind"] = "works.at", ["targetId"] = "gate.building.market"
                        }
                    }
                },
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "put-extension", ["owner"] = "future.gate.social", ["schemaVersion"] = 1,
                    ["subjectId"] = "gate.npc.ana", ["payloadBase64"] = "ECAw",
                    ["dependencies"] = new object?[]
                    {
                        new Dictionary<string, object?>(StringComparer.Ordinal)
                        {
                            ["kind"] = "works.at", ["targetId"] = "gate.building.market"
                        }
                    }
                }
            };
        }

        private static IReadOnlyList<object?> RepresentativeNinetySixOperationIntent()
        {
            var operations = new List<object?>();
            for (var index = 0; index < 72; index++)
            {
                operations.Add(PutObject(
                    "gate.micro-block.item-" + index.ToString("D2", CultureInfo.InvariantCulture),
                    "fixture.gate.micro-block-item"));
            }
            for (var index = 0; index < 24; index++)
            {
                operations.Add(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "put-extension",
                    ["owner"] = "fixture.gate.visual-metadata",
                    ["schemaVersion"] = 1,
                    ["subjectId"] = "gate.micro-block.item-" + index.ToString("D2", CultureInfo.InvariantCulture),
                    ["payloadBase64"] = Convert.ToBase64String(new byte[] { (byte)index }),
                    ["dependencies"] = Array.Empty<object?>()
                });
            }
            return operations.AsReadOnly();
        }

        private static IReadOnlyDictionary<string, object?> PutObject(string id, string typeId, string? containerId = null)
        {
            var operation = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = "put-object", ["id"] = id, ["typeId"] = typeId, ["references"] = Array.Empty<object?>()
            };
            if (containerId != null) operation["containerId"] = containerId;
            return operation;
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

        private static IReadOnlyDictionary<string, object?> Empty() => new Dictionary<string, object?>(StringComparer.Ordinal);

        private static void EmitTranscript(IReadOnlyDictionary<string, object?> transcript)
        {
            var json = JsonSerializer.Serialize(transcript, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine("HK_GATE_REFERENCE_SCENARIO=" + json.Replace(Environment.NewLine, " ", StringComparison.Ordinal));
            var output = Environment.GetEnvironmentVariable("ARKUS_HK_GATE_TRANSCRIPT_OUTPUT");
            if (string.IsNullOrWhiteSpace(output)) return;
            var directory = Path.GetDirectoryName(output);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText(output, json + Environment.NewLine, new UTF8Encoding(false));
        }

        private sealed class ReferenceGateClient : IDisposable
        {
            private readonly Process _process;
            private readonly System.Threading.Tasks.Task<string> _error;
            private readonly List<string> _capabilities = new List<string>();
            private readonly string _requestPrefix;
            private bool _disposed;
            private int _sequence;

            public ReferenceGateClient(string requestPrefix)
            {
                _requestPrefix = requestPrefix;
                _process = Hk07AProcessHarness.Start();
                _error = _process.StandardError.ReadToEndAsync();
            }

            public int RequestCount => _sequence;
            public long SerializedResponseBytes { get; private set; }

            public IReadOnlyList<string> CapabilitiesFrom(int requestOffset)
            {
                if (requestOffset < 0 || requestOffset > _capabilities.Count) throw new ArgumentOutOfRangeException(nameof(requestOffset));
                return _capabilities.Skip(requestOffset).ToArray();
            }

            public JsonElement Invoke(string capability, object arguments, string version = "1.0")
            {
                var requestId = _requestPrefix + "." + (++_sequence).ToString(CultureInfo.InvariantCulture);
                _capabilities.Add(capability);
                _process.StandardInput.WriteLine(Frame(requestId, capability, arguments, version));
                _process.StandardInput.Flush();
                var line = _process.StandardOutput.ReadLine();
                Assert.False(string.IsNullOrWhiteSpace(line), "Reference gate process ended before response.");
                SerializedResponseBytes += Encoding.UTF8.GetByteCount(line!);
                using var document = JsonDocument.Parse(line!);
                var root = document.RootElement;
                Assert.Equal("arkus.reference.jsonl@1", root.GetProperty("protocol").GetString());
                var response = root.GetProperty("response");
                Assert.Equal(requestId, response.GetProperty("requestId").GetString());
                return response.Clone();
            }

            private static string Frame(string requestId, string capability, object arguments, string version)
            {
                var parts = version.Split('.');
                Assert.Equal(2, parts.Length);
                var major = int.Parse(parts[0], CultureInfo.InvariantCulture);
                var minor = int.Parse(parts[1], CultureInfo.InvariantCulture);
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
                            ["major"] = major, ["minimumMinor"] = minor, ["maximumMinor"] = minor
                        },
                        ["arguments"] = arguments
                    }
                });
            }

            public void Dispose()
            {
                if (_disposed) return;
                _process.StandardInput.Close();
                Assert.True(_process.WaitForExit(30000), "Reference gate process did not exit.");
                Assert.Equal(0, _process.ExitCode);
                Assert.Equal(string.Empty, _error.Result);
                _process.Dispose();
                _disposed = true;
            }
        }
    }
}
