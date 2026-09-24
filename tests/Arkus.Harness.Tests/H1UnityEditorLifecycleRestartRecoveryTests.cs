using System;
using System.Collections.Generic;
using System.IO;
using Arkus.H1.UnityHost;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1UnityEditorLifecycleRestartRecoveryTests
    {
        [Fact]
        public void Restarted_status_query_reclassifies_orphaned_running_as_indeterminate()
        {
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var invocationId = "h1u-restart-running-" + Guid.NewGuid().ToString("N");
            var path = LedgerPath(profile, invocationId);
            try
            {
                var firstHostLedger = new FileH1UnityInvocationLedger(profile);
                firstHostLedger.Record(new H1UnityInvocationRecord(
                    invocationId,
                    ProjectProfileInspectExecutor.Key.ToString(),
                    H1UnityInvocationStatus.Running,
                    "running"));

                var restartedRawLedger = new FileH1UnityInvocationLedger(profile);
                var restartedLedger = new RestartRecoveringH1UnityInvocationLedger(
                    restartedRawLedger,
                    new FileH1UnityProjectLease(profile));
                var handler = new OperationStatusHandler(restartedLedger);

                var outcome = handler.Invoke(null!, Request(invocationId));

                Assert.True(outcome.Success);
                Assert.Equal("indeterminate", outcome.Data!["status"]);
                Assert.Equal(H1UnityEditorExecutionCoordinator.IndeterminateCode, outcome.Data["outcomeCode"]);
                Assert.True(restartedRawLedger.TryRead(invocationId, out var persisted));
                Assert.NotNull(persisted);
                Assert.Equal(H1UnityInvocationStatus.Indeterminate, persisted!.Status);
                Assert.Equal(H1UnityEditorExecutionCoordinator.IndeterminateCode, persisted.OutcomeCode);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void Live_project_lease_prevents_running_record_from_being_reclassified()
        {
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var invocationId = "h1u-live-running-" + Guid.NewGuid().ToString("N");
            var path = LedgerPath(profile, invocationId);
            try
            {
                var rawLedger = new FileH1UnityInvocationLedger(profile);
                rawLedger.Record(new H1UnityInvocationRecord(
                    invocationId,
                    ProjectProfileInspectExecutor.Key.ToString(),
                    H1UnityInvocationStatus.Running,
                    "running"));

                var liveLeaseOwner = new FileH1UnityProjectLease(profile);
                var recoveringLedger = new RestartRecoveringH1UnityInvocationLedger(
                    rawLedger,
                    new FileH1UnityProjectLease(profile));
                var handler = new OperationStatusHandler(recoveringLedger);

                using (var held = liveLeaseOwner.TryAcquire())
                {
                    Assert.NotNull(held);
                    var whileLive = handler.Invoke(null!, Request(invocationId));
                    Assert.True(whileLive.Success);
                    Assert.Equal("running", whileLive.Data!["status"]);
                    Assert.Equal("running", whileLive.Data["outcomeCode"]);
                }

                var afterOwnerLoss = handler.Invoke(null!, Request(invocationId));
                Assert.True(afterOwnerLoss.Success);
                Assert.Equal("indeterminate", afterOwnerLoss.Data!["status"]);
                Assert.Equal(H1UnityEditorExecutionCoordinator.IndeterminateCode, afterOwnerLoss.Data["outcomeCode"]);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void Final_lifecycle_state_is_never_rewritten_by_restart_recovery()
        {
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var invocationId = "h1u-restart-final-" + Guid.NewGuid().ToString("N");
            var path = LedgerPath(profile, invocationId);
            try
            {
                var rawLedger = new FileH1UnityInvocationLedger(profile);
                rawLedger.Record(new H1UnityInvocationRecord(
                    invocationId,
                    ProjectProfileInspectExecutor.Key.ToString(),
                    H1UnityInvocationStatus.Completed,
                    "success"));
                var recoveringLedger = new RestartRecoveringH1UnityInvocationLedger(
                    rawLedger,
                    new FileH1UnityProjectLease(profile));

                Assert.True(recoveringLedger.TryRead(invocationId, out var recovered));
                Assert.NotNull(recovered);
                Assert.Equal(H1UnityInvocationStatus.Completed, recovered!.Status);
                Assert.Equal("success", recovered.OutcomeCode);
            }
            finally
            {
                File.Delete(path);
            }
        }

        private static IReadOnlyDictionary<string, object?> Request(string invocationId) =>
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["invocationId"] = invocationId
            };

        private static string LedgerPath(H1UnityLaunchProfile profile, string invocationId) =>
            Path.Combine(profile.ProjectRoot, "Library", "Arkus", "H1Lifecycle", "ledger", invocationId + ".ledger");
    }
}
