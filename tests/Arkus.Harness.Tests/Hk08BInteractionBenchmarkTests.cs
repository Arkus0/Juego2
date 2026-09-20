using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk08BInteractionBenchmarkTests
    {
        // Frozen from the first green public-client observation on GitHub Actions run 35501506939
        // (candidate 8b04a66079cd8a637f0abd8aa1eeb18cd90d9f99), after recovery truth was fixed.
        // Requests are structural and receive no growth allowance. Serialized bytes receive 25%
        // headroom for legitimate additive diagnostic/recovery payload growth. Elapsed time is a
        // coarse regression guard with 8x runner-noise headroom; it is not a latency SLO.
        private const int BaselineRequestCount = 12;
        private const long BaselineSerializedResponseBytes = 12051;
        private const long BaselineElapsedMilliseconds = 185;
        private const int MaximumRequestCount = BaselineRequestCount;
        private const long MaximumSerializedResponseBytes = 15064;
        private const long MaximumElapsedMilliseconds = BaselineElapsedMilliseconds * 8;

        [Fact]
        public void RepresentativePublicClientFlowMeasuresInteractionCostWithoutWholeWorldConflictReload()
        {
            using var client = new InstrumentedReferenceClient();
            var stopwatch = Stopwatch.StartNew();

            // 1. Create through one coherent canonical mutation.
            var initialSummary = Success(client.Invoke("world.summary", Empty()));
            var initialAnchor = initialSummary.GetProperty("world");
            var created = Success(client.Invoke(
                "authoring.change.apply",
                MutationRequest(
                    initialAnchor,
                    "request.hk08b.benchmark.create",
                    new object?[]
                    {
                        PutObject("bench.root", "fixture.hk08b.root"),
                        PutObject("bench.child", "fixture.hk08b.child", "bench.root")
                    })));
            var currentAnchor = created.GetProperty("plan").GetProperty("result");

            // 2. Bounded compact inspection consumes the accepted HK08A projection rather than a
            // broad query/full-world materialization.
            var compact = Success(client.Invoke(
                "world.object.get",
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["revision"] = currentAnchor.GetProperty("revision").GetInt64(),
                    ["hash"] = currentAnchor.GetProperty("hash").GetString(),
                    ["id"] = "bench.child",
                    ["fields"] = Array.Empty<object?>()
                }));
            Assert.Equal("bench.child", compact.GetProperty("object").GetProperty("id").GetString());
            Assert.False(compact.GetProperty("object").TryGetProperty("typeId", out _));

            // 3. Modify two material resources in one public request/one HK04 transaction.
            var modified = Success(client.Invoke(
                "authoring.change.apply",
                MutationRequest(
                    currentAnchor,
                    "request.hk08b.benchmark.modify",
                    new object?[]
                    {
                        PutObject("bench.root", "fixture.hk08b.root.modified"),
                        PutObject("bench.child", "fixture.hk08b.child.modified", "bench.root")
                    })));
            currentAnchor = modified.GetProperty("plan").GetProperty("result");
            Assert.Equal(2, modified.GetProperty("plan").GetProperty("changes").GetArrayLength());

            // 4. Invalid intent returns the complete HK05 diagnostic artifact. Repair uses that
            // unchanged anchor and remains one coherent two-resource transaction.
            var invalid = client.Invoke(
                "authoring.change.apply",
                MutationRequest(
                    currentAnchor,
                    "request.hk08b.benchmark.invalid",
                    new object?[] { RemoveObject("bench.root") }));
            Assert.Equal("error", invalid.GetProperty("status").GetString());
            Assert.Equal("world.change.invalid_candidate", invalid.GetProperty("error").GetProperty("machineCode").GetString());
            var validation = invalid.GetProperty("error").GetProperty("context").GetProperty("validation");
            var diagnostics = validation.GetProperty("diagnostics");
            var diagnosticCount = validation.GetProperty("diagnosticCount").GetInt32();
            Assert.True(diagnosticCount > 0);
            Assert.Equal(diagnosticCount, diagnostics.GetArrayLength());

            var repaired = Success(client.Invoke(
                "authoring.change.apply",
                MutationRequest(
                    currentAnchor,
                    "request.hk08b.benchmark.repair",
                    new object?[]
                    {
                        RemoveObject("bench.child"),
                        RemoveObject("bench.root")
                    })));
            var recoveryBase = repaired.GetProperty("plan").GetProperty("result");

            // 5. Create a real same-lineage conflict, then consume only the recovery descriptors.
            var writer = Success(client.Invoke(
                "authoring.change.apply",
                MutationRequest(
                    recoveryBase,
                    "request.hk08b.benchmark.writer",
                    new object?[] { PutObject("bench.conflict", "fixture.hk08b.writer") })));
            var writerAnchor = writer.GetProperty("plan").GetProperty("result");

            var recoveryPhaseStart = client.RequestCount;
            var stale = client.Invoke(
                "authoring.change.plan",
                MutationRequest(
                    recoveryBase,
                    "request.hk08b.benchmark.stale-client",
                    new object?[] { PutObject("bench.client", "fixture.hk08b.client") }));
            Assert.Equal("error", stale.GetProperty("status").GetString());
            Assert.Equal("world.change.stale_revision", stale.GetProperty("error").GetProperty("machineCode").GetString());
            var recovery = stale.GetProperty("error").GetProperty("context").GetProperty("recovery");
            Assert.Equal("arkus.world-conflict-recovery@1", recovery.GetProperty("schemaId").GetString());
            Assert.Equal("same-lineage-replan", recovery.GetProperty("disposition").GetString());
            Assert.Equal(writerAnchor.GetProperty("revision").GetInt64(), recovery.GetProperty("current").GetProperty("revision").GetInt64());
            Assert.Equal(writerAnchor.GetProperty("hash").GetString(), recovery.GetProperty("current").GetProperty("hash").GetString());
            Assert.Equal(1, recovery.GetProperty("changedResources").GetArrayLength());

            var recoveryInspectionRequests = 0;
            foreach (var descriptor in recovery.GetProperty("currentResources").EnumerateArray())
            {
                Assert.Equal("present", descriptor.GetProperty("presence").GetString());
                var inspection = descriptor.GetProperty("inspection");
                var capability = inspection.GetProperty("name").GetString()!;
                Assert.Equal("1.0", inspection.GetProperty("version").GetString());
                Success(client.Invoke(capability, inspection.GetProperty("request").Clone()));
                recoveryInspectionRequests++;
            }
            Assert.Equal(1, recoveryInspectionRequests);

            var recoveryAnchor = recovery.GetProperty("current");
            var recoveredRequest = MutationRequest(
                recoveryAnchor,
                "request.hk08b.benchmark.stale-client",
                new object?[] { PutObject("bench.client", "fixture.hk08b.client") });
            Success(client.Invoke("authoring.change.plan", recoveredRequest));
            Success(client.Invoke("authoring.change.dry-run", recoveredRequest));
            var recovered = Success(client.Invoke("authoring.change.apply", recoveredRequest));
            Assert.True(recovered.GetProperty("persisted").GetBoolean());

            stopwatch.Stop();
            var elapsedMilliseconds = (long)Math.Ceiling(stopwatch.Elapsed.TotalMilliseconds);

            var recoveryCapabilities = client.CapabilitiesFrom(recoveryPhaseStart);
            Assert.DoesNotContain("world.summary", recoveryCapabilities);
            Assert.DoesNotContain("world.object.query", recoveryCapabilities);
            Assert.DoesNotContain("world.extension.query", recoveryCapabilities);
            Assert.Equal(BaselineRequestCount, client.RequestCount);
            Assert.Empty(BudgetIssues(client.RequestCount, client.SerializedResponseBytes, elapsedMilliseconds));

            var metrics = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = "arkus.hk08b.interaction-benchmark@1",
                ["transport"] = "reference-jsonl",
                ["flowCount"] = 5,
                ["requestCount"] = client.RequestCount,
                ["serializedResponseBytes"] = client.SerializedResponseBytes,
                ["elapsedMilliseconds"] = elapsedMilliseconds,
                ["invalidDiagnosticCount"] = diagnosticCount,
                ["recoveryChangedResourceCount"] = recovery.GetProperty("changedResources").GetArrayLength(),
                ["recoveryInspectionRequestCount"] = recoveryInspectionRequests,
                ["recoveryFullWorldReloadCount"] = 0,
                ["multiResourceModifyRequestCount"] = 1,
                ["publicTransportOnly"] = true,
                ["baselineRequestCount"] = BaselineRequestCount,
                ["baselineSerializedResponseBytes"] = BaselineSerializedResponseBytes,
                ["baselineElapsedMilliseconds"] = BaselineElapsedMilliseconds,
                ["maximumRequestCount"] = MaximumRequestCount,
                ["maximumSerializedResponseBytes"] = MaximumSerializedResponseBytes,
                ["maximumElapsedMilliseconds"] = MaximumElapsedMilliseconds
            };
            EmitMetrics(metrics);
        }

        [Fact]
        public void BenchmarkAndDiagnosticCompletenessOraclesTurnRedForForbiddenShortcuts()
        {
            var complete = new[] { "diag-a", "diag-b", "diag-c" };
            Assert.Empty(DiagnosticCompletenessIssues(complete, complete));
            Assert.Contains("independent-diagnostic-hidden", DiagnosticCompletenessIssues(complete, new[] { "diag-a", "diag-c" }));

            Assert.Empty(BenchmarkShapeIssues(
                publicTransportOnly: true,
                includesAllRepresentativeFlows: true,
                recoveryFullWorldReloadCount: 0,
                conceptualChangedResourceCount: 2,
                recoveryInspectionRequestCount: 2,
                multiResourceModifyRequestCount: 1));
            Assert.Contains("private-surface", BenchmarkShapeIssues(false, true, 0, 2, 2, 1));
            Assert.Contains("representative-flow-omitted", BenchmarkShapeIssues(true, false, 0, 2, 2, 1));
            Assert.Contains("full-world-reload", BenchmarkShapeIssues(true, true, 1, 2, 2, 1));
            Assert.Contains("affected-resource-omitted", BenchmarkShapeIssues(true, true, 0, 2, 1, 1));
            Assert.Contains("per-resource-mutation-chatter", BenchmarkShapeIssues(true, true, 0, 2, 2, 2));

            Assert.Empty(BudgetIssues(
                BaselineRequestCount,
                BaselineSerializedResponseBytes,
                BaselineElapsedMilliseconds));
            Assert.Contains("request-budget-regression", BudgetIssues(
                MaximumRequestCount + 1,
                BaselineSerializedResponseBytes,
                BaselineElapsedMilliseconds));
            Assert.Contains("response-budget-regression", BudgetIssues(
                BaselineRequestCount,
                MaximumSerializedResponseBytes + 1,
                BaselineElapsedMilliseconds));
            Assert.Contains("elapsed-budget-regression", BudgetIssues(
                BaselineRequestCount,
                BaselineSerializedResponseBytes,
                MaximumElapsedMilliseconds + 1));
        }

        private static IReadOnlyList<string> DiagnosticCompletenessIssues(
            IReadOnlyList<string> authoritative,
            IReadOnlyList<string> presented)
        {
            var expected = new HashSet<string>(authoritative, StringComparer.Ordinal);
            var actual = new HashSet<string>(presented, StringComparer.Ordinal);
            return expected.SetEquals(actual)
                ? Array.Empty<string>()
                : new[] { "independent-diagnostic-hidden" };
        }

        private static IReadOnlyList<string> BenchmarkShapeIssues(
            bool publicTransportOnly,
            bool includesAllRepresentativeFlows,
            int recoveryFullWorldReloadCount,
            int conceptualChangedResourceCount,
            int recoveryInspectionRequestCount,
            int multiResourceModifyRequestCount)
        {
            var issues = new List<string>();
            if (!publicTransportOnly) issues.Add("private-surface");
            if (!includesAllRepresentativeFlows) issues.Add("representative-flow-omitted");
            if (recoveryFullWorldReloadCount != 0) issues.Add("full-world-reload");
            if (recoveryInspectionRequestCount < conceptualChangedResourceCount) issues.Add("affected-resource-omitted");
            if (multiResourceModifyRequestCount > 1) issues.Add("per-resource-mutation-chatter");
            return issues.AsReadOnly();
        }

        private static IReadOnlyList<string> BudgetIssues(
            int requestCount,
            long serializedResponseBytes,
            long elapsedMilliseconds)
        {
            var issues = new List<string>();
            if (requestCount > MaximumRequestCount) issues.Add("request-budget-regression");
            if (serializedResponseBytes > MaximumSerializedResponseBytes) issues.Add("response-budget-regression");
            if (elapsedMilliseconds > MaximumElapsedMilliseconds) issues.Add("elapsed-budget-regression");
            return issues.AsReadOnly();
        }

        private static JsonElement Success(JsonElement response)
        {
            Assert.Equal("success", response.GetProperty("status").GetString());
            return response.GetProperty("result");
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

        private static IReadOnlyDictionary<string, object?> PutObject(
            string id,
            string typeId,
            string? containerId = null)
        {
            var operation = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = "put-object",
                ["id"] = id,
                ["typeId"] = typeId,
                ["references"] = Array.Empty<object?>()
            };
            if (containerId != null) operation["containerId"] = containerId;
            return operation;
        }

        private static IReadOnlyDictionary<string, object?> RemoveObject(string id)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = "remove-object",
                ["id"] = id
            };
        }

        private static IReadOnlyDictionary<string, object?> Empty() =>
            new Dictionary<string, object?>(StringComparer.Ordinal);

        private static void EmitMetrics(IReadOnlyDictionary<string, object?> metrics)
        {
            var json = JsonSerializer.Serialize(metrics, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine("HK08B_BENCHMARK=" + json.Replace(Environment.NewLine, " ", StringComparison.Ordinal));
            var output = Environment.GetEnvironmentVariable("ARKUS_HK08B_BENCHMARK_OUTPUT");
            if (string.IsNullOrWhiteSpace(output)) return;
            var directory = Path.GetDirectoryName(output);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText(output, json + Environment.NewLine, new UTF8Encoding(false));
        }

        private sealed class InstrumentedReferenceClient : IDisposable
        {
            private readonly Process _process;
            private readonly System.Threading.Tasks.Task<string> _error;
            private readonly List<string> _capabilities = new List<string>();
            private int _sequence;

            public InstrumentedReferenceClient()
            {
                _process = Hk07AProcessHarness.Start();
                _error = _process.StandardError.ReadToEndAsync();
            }

            public int RequestCount => _sequence;
            public long SerializedResponseBytes { get; private set; }

            public IReadOnlyList<string> CapabilitiesFrom(int requestOffset)
            {
                if (requestOffset < 0 || requestOffset > _capabilities.Count)
                    throw new ArgumentOutOfRangeException(nameof(requestOffset));
                var values = new List<string>();
                for (var index = requestOffset; index < _capabilities.Count; index++) values.Add(_capabilities[index]);
                return values.AsReadOnly();
            }

            public JsonElement Invoke(string capability, object arguments)
            {
                var requestId = "hk08b.benchmark." + (++_sequence).ToString(CultureInfo.InvariantCulture);
                _capabilities.Add(capability);
                _process.StandardInput.WriteLine(Hk07AProcessHarness.Frame(requestId, capability, arguments));
                _process.StandardInput.Flush();
                var line = _process.StandardOutput.ReadLine();
                Assert.False(string.IsNullOrWhiteSpace(line), "Reference process ended before benchmark response.");
                using var document = JsonDocument.Parse(line!);
                var response = document.RootElement.GetProperty("response").Clone();
                SerializedResponseBytes += Encoding.UTF8.GetByteCount(response.GetRawText());
                return response;
            }

            public void Dispose()
            {
                _process.StandardInput.Close();
                Assert.True(_process.WaitForExit(30000), "Reference benchmark process did not exit.");
                Assert.Equal(0, _process.ExitCode);
                Assert.Equal(string.Empty, _error.Result);
                _process.Dispose();
            }
        }
    }
}
