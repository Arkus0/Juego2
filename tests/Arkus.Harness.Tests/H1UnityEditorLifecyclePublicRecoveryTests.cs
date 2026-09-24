using System;
using System.Collections.Generic;
using Arkus.H1.UnityHost;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1UnityEditorLifecyclePublicRecoveryTests
    {
        [Fact]
        public void Successful_profile_result_exposes_recoverable_invocation_and_observable_H1_ceilings_without_paths()
        {
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var executor = new ProjectProfileInspectExecutor();
            var invocationId = "h1u-public-recovery-proof";
            var envelope = new H1UnityResultEnvelope(
                invocationId,
                ProjectProfileInspectExecutor.Key.ToString(),
                executor.ExecutorId,
                profile.Id,
                profile.ProjectIdentity,
                profile.EffectiveEditorVersion,
                profile.EffectiveEditorRevision,
                true,
                "inspect");

            var decoded = executor.DecodeResult(envelope);

            Assert.True(decoded.Success);
            var result = decoded.Data!;
            Assert.Equal(invocationId, result["invocationId"]);
            Assert.Equal(H1UnityLaunchProfile.FixedEntryPoint, result["entryPoint"]);
            Assert.Equal(profile.Platform, result["platform"]);
            Assert.Equal(H1UnityOperationCeilings.SchemaId, result["operationCeilingsId"]);
            var ceilings = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(result["operationCeilings"]);
            Assert.Equal(H1UnityOperationCeilings.SchemaId, ceilings["schemaId"]);
            Assert.Equal(H1UnityOperationCeilings.MaximumExecutionMilliseconds, ceilings["maximumExecutionMilliseconds"]);
            Assert.Equal(H1UnityOperationCeilings.MaximumLedgerEntries, ceilings["maximumLedgerEntries"]);
            Assert.Equal(H1UnityOperationCeilings.MaximumPayloadBytes, ceilings["maximumPayloadBytes"]);
            Assert.Equal(H0ResourceEnvelope.SchemaId, ceilings["h0EnvelopeUnchanged"]);
            Assert.False(result.ContainsKey("projectRoot"));
            Assert.False(result.ContainsKey("executablePath"));
            Assert.False(result.ContainsKey("arguments"));

            var ledger = new MemoryLedger();
            ledger.Record(new H1UnityInvocationRecord(
                invocationId,
                ProjectProfileInspectExecutor.Key.ToString(),
                H1UnityInvocationStatus.Completed,
                "success"));
            var status = new OperationStatusHandler(ledger).Invoke(
                null!,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["invocationId"] = Assert.IsType<string>(result["invocationId"])
                });

            Assert.True(status.Success);
            Assert.Equal(invocationId, status.Data!["invocationId"]);
            Assert.Equal("completed", status.Data["status"]);
            Assert.Equal("success", status.Data["outcomeCode"]);
        }

        private sealed class MemoryLedger : IH1UnityInvocationLedger
        {
            private readonly Dictionary<string, H1UnityInvocationRecord> _records =
                new Dictionary<string, H1UnityInvocationRecord>(StringComparer.Ordinal);

            public void Record(H1UnityInvocationRecord record) => _records[record.InvocationId] = record;

            public bool TryRead(string invocationId, out H1UnityInvocationRecord? record)
            {
                if (_records.TryGetValue(invocationId, out var found))
                {
                    record = found;
                    return true;
                }
                record = null;
                return false;
            }
        }
    }
}
