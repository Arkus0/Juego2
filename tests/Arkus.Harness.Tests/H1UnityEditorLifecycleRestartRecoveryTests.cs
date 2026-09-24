using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using Arkus.H1.UnityHost;
using Arkus.Harness.H1LeaseCrashFixture;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    [Collection(H1UnityEditorLifecycleProjectLeaseCollection.Name)]
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
        public void Inherited_project_lease_survives_host_crash_and_blocks_recovery_until_worker_exits()
        {
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var invocationId = "h1u-host-crash-" + Guid.NewGuid().ToString("N");
            var proofDirectory = Path.Combine(profile.ProjectRoot, "Library", "Arkus", "H1Lifecycle", "crash-proof", invocationId);
            var readyPath = Path.Combine(proofDirectory, "worker.ready");
            var releasePath = Path.Combine(proofDirectory, "worker.release");
            var ledgerPath = LedgerPath(profile, invocationId);
            Process? host = null;
            try
            {
                Directory.CreateDirectory(proofDirectory);
                host = Process.Start(CreateCrashHostStartInfo(profile, invocationId, readyPath, releasePath));
                Assert.NotNull(host);
                WaitForFile(readyPath, host!);
                Assert.True(host!.WaitForExit(10000), "The crash-host fixture did not terminate after the worker inherited the real operation lease.");
                Assert.NotEqual(0, host.ExitCode);

                var rawLedger = new FileH1UnityInvocationLedger(profile);
                var recoveringLedger = new RestartRecoveringH1UnityInvocationLedger(
                    rawLedger,
                    new FileH1UnityProjectLease(profile));

                Assert.True(recoveringLedger.TryRead(invocationId, out var whileWorkerAlive));
                Assert.NotNull(whileWorkerAlive);
                Assert.Equal(H1UnityInvocationStatus.Running, whileWorkerAlive!.Status);
                Assert.Equal("running", whileWorkerAlive.OutcomeCode);

                using (var blockedAdmission = new FileH1UnityProjectLease(profile).TryAcquire())
                {
                    Assert.Null(blockedAdmission);
                }

                using (var restartedHost = ProductionH1UnityHost.Create())
                {
                    var busy = restartedHost.InvokeAsync(
                        new NeutralProjectionRequest(
                            "restart-busy-" + Guid.NewGuid().ToString("N"),
                            ProjectProfileInspectExecutor.Key.Name,
                            new ContractVersionRange(1, 0, 0),
                            new Dictionary<string, object?>(StringComparer.Ordinal),
                            1000),
                        CancellationToken.None).GetAwaiter().GetResult();
                    Assert.False(busy.Success);
                    Assert.NotNull(busy.Error);
                    Assert.Equal(H1UnityEditorExecutionCoordinator.BusyCode, busy.Error!.MachineCode);
                }

                File.WriteAllText(releasePath, "release");
                using (var reacquired = WaitForProjectAdmission(profile))
                {
                    Assert.NotNull(reacquired);
                }

                Assert.True(recoveringLedger.TryRead(invocationId, out var recovered));
                Assert.NotNull(recovered);
                Assert.Equal(H1UnityInvocationStatus.Indeterminate, recovered!.Status);
                Assert.Equal(H1UnityEditorExecutionCoordinator.IndeterminateCode, recovered.OutcomeCode);

                using var postRecoveryAdmission = new FileH1UnityProjectLease(profile).TryAcquire();
                Assert.NotNull(postRecoveryAdmission);
            }
            finally
            {
                if (!File.Exists(releasePath))
                {
                    Directory.CreateDirectory(proofDirectory);
                    File.WriteAllText(releasePath, "cleanup");
                }
                TryKillFixtureWorker(readyPath);
                host?.Dispose();
                File.Delete(ledgerPath);
                if (Directory.Exists(proofDirectory)) Directory.Delete(proofDirectory, true);
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

        private static ProcessStartInfo CreateCrashHostStartInfo(H1UnityLaunchProfile profile, string invocationId, string readyPath, string releasePath)
        {
            var info = new ProcessStartInfo
            {
                FileName = "dotnet",
                WorkingDirectory = profile.RepositoryRoot,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            info.ArgumentList.Add(typeof(FixtureMarker).Assembly.Location);
            info.ArgumentList.Add("host");
            info.ArgumentList.Add(invocationId);
            info.ArgumentList.Add(readyPath);
            info.ArgumentList.Add(releasePath);
            return info;
        }

        private static void WaitForFile(string path, Process host)
        {
            var deadline = DateTime.UtcNow.AddSeconds(15);
            while (!File.Exists(path))
            {
                if (host.HasExited) throw new Xunit.Sdk.XunitException("Crash-host fixture exited before its child inherited the worker lease. Exit=" + host.ExitCode.ToString(CultureInfo.InvariantCulture));
                if (DateTime.UtcNow >= deadline) throw new Xunit.Sdk.XunitException("Timed out waiting for the controlled worker process to become live.");
                Thread.Sleep(20);
            }
        }

        private static IDisposable WaitForProjectAdmission(H1UnityLaunchProfile profile)
        {
            var deadline = DateTime.UtcNow.AddSeconds(15);
            while (DateTime.UtcNow < deadline)
            {
                var lease = new FileH1UnityProjectLease(profile).TryAcquire();
                if (lease != null) return lease;
                Thread.Sleep(20);
            }
            throw new Xunit.Sdk.XunitException("Worker operation lease remained held after the controlled worker was released.");
        }

        private static void TryKillFixtureWorker(string readyPath)
        {
            try
            {
                if (!File.Exists(readyPath)) return;
                if (!int.TryParse(File.ReadAllText(readyPath), NumberStyles.Integer, CultureInfo.InvariantCulture, out var pid)) return;
                using var worker = Process.GetProcessById(pid);
                if (worker.HasExited) return;
                worker.Kill(true);
                worker.WaitForExit(5000);
            }
            catch
            {
                // Cleanup only. Lifecycle truth in the test is established by the inherited file lease.
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
