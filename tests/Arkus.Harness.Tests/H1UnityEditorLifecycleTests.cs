using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Arkus.EngineBridge.UnityAuthoring;
using Arkus.H1.UnityHost;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1UnityEditorLifecycleTests
    {
        [Fact]
        public void Launch_profile_is_fixed_and_public_shape_does_not_expose_paths()
        {
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var data = profile.ToPublicData();

            Assert.Equal(H1UnityLaunchProfile.ProfileId, data["profileId"]);
            Assert.Equal(UnityProjectWorkspaceAuthority.ProjectIdentity, data["projectIdentity"]);
            Assert.Equal(H1UnityLaunchProfile.FixedEntryPoint, data["entryPoint"]);
            Assert.Equal(H1UnityLaunchProfile.EditorVersion, data["editorVersion"]);
            Assert.Equal(H1UnityLaunchProfile.EditorRevision, data["editorRevision"]);
            Assert.False(data.ContainsKey("executablePath"));
            Assert.False(data.ContainsKey("projectRoot"));
            Assert.False(data.ContainsKey("arguments"));
        }

        [Fact]
        public void H0_policy_still_rejects_raw_H1_editor_bound_definitions()
        {
            var launcher = new FakeLauncher((invocation, _) => Completed(invocation));
            var coordinator = Coordinator(launcher);
            var contribution = H1UnityLifecycleContract.CreateEditorHostContribution(coordinator);

            var issues = H0HostCapabilityPolicy.Validate(contribution.Definitions);

            Assert.NotEmpty(issues);
        }

        [Fact]
        public void Bootstrap_fails_when_an_admitted_editor_capability_has_no_executor()
        {
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var launcher = new FakeLauncher((invocation, _) => Completed(invocation));
            var ledger = new MemoryLedger();
            var coordinator = new H1UnityEditorExecutionCoordinator(
                profile,
                launcher,
                new AvailableLease(),
                ledger,
                new IH1UnityCapabilityExecutor[] { new ProjectProfileInspectExecutor() });
            using var projection = CreateProjection(coordinator, ledger);

            var exception = Assert.Throws<InvalidOperationException>(() => coordinator.Bind(projection));

            Assert.Contains("missing worker executor", exception.Message, StringComparison.Ordinal);
        }

        [Fact]
        public void Bootstrap_fails_when_an_executor_has_no_admitted_handler()
        {
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var launcher = new FakeLauncher((invocation, _) => Completed(invocation));
            var ledger = new MemoryLedger();
            var coordinator = new H1UnityEditorExecutionCoordinator(
                profile,
                launcher,
                new AvailableLease(),
                ledger,
                new IH1UnityCapabilityExecutor[]
                {
                    new ProjectProfileInspectExecutor(),
                    new HierarchyProbeExecutor(),
                    new OrphanExecutor()
                });
            using var projection = CreateProjection(coordinator, ledger);

            var exception = Assert.Throws<InvalidOperationException>(() => coordinator.Bind(projection));

            Assert.Contains("orphan worker executor", exception.Message, StringComparison.Ordinal);
        }

        [Fact]
        public async Task Two_read_only_editor_calls_use_fresh_invocations_and_return_equal_canonical_results()
        {
            var seen = new List<string>();
            var launcher = new FakeLauncher((invocation, _) =>
            {
                seen.Add(invocation.InvocationId);
                return Completed(invocation);
            });
            using var harness = CreateHarness(launcher);

            var first = await InvokeAsync(harness.Projection, ProjectProfileInspectExecutor.Key.Name);
            var second = await InvokeAsync(harness.Projection, ProjectProfileInspectExecutor.Key.Name);

            Assert.True(first.Success);
            Assert.True(second.Success);
            Assert.Equal(2, launcher.Count);
            Assert.Equal(2, seen.Count);
            Assert.NotEqual(seen[0], seen[1]);
            Assert.Equal(first.Result!["profileId"], second.Result!["profileId"]);
            Assert.Equal(first.Result["projectIdentity"], second.Result["projectIdentity"]);
            Assert.Equal(first.Result["editorVersion"], second.Result["editorVersion"]);
            Assert.Equal(true, first.Result["mainThread"]);
        }

        [Fact]
        public async Task Already_cancelled_request_never_launches_Unity()
        {
            var launcher = new FakeLauncher((invocation, _) => Completed(invocation));
            using var harness = CreateHarness(launcher);
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            var outcome = await InvokeAsync(harness.Projection, ProjectProfileInspectExecutor.Key.Name, cancellation.Token);

            Assert.False(outcome.Success);
            Assert.Equal(NeutralProjectionFailureKind.Cancelled, outcome.FailureKind);
            Assert.Equal(0, launcher.Count);
        }

        [Fact]
        public async Task Busy_project_is_structured_and_does_not_launch_worker()
        {
            var launcher = new FakeLauncher((invocation, _) => Completed(invocation));
            using var harness = CreateHarness(launcher, new BusyLease());

            var outcome = await InvokeAsync(harness.Projection, ProjectProfileInspectExecutor.Key.Name);

            Assert.False(outcome.Success);
            Assert.Equal(H1UnityEditorExecutionCoordinator.BusyCode, outcome.Error!.MachineCode);
            Assert.True(outcome.Error.Retryable);
            Assert.Equal(0, launcher.Count);
        }

        [Theory]
        [InlineData(H1UnityWorkerLaunchKind.NonZeroExit, true, H1UnityEditorExecutionCoordinator.CrashCode, "indeterminate")]
        [InlineData(H1UnityWorkerLaunchKind.MissingResult, true, H1UnityEditorExecutionCoordinator.MissingResultCode, "indeterminate")]
        [InlineData(H1UnityWorkerLaunchKind.CorruptResult, true, H1UnityEditorExecutionCoordinator.CorruptResultCode, "indeterminate")]
        [InlineData(H1UnityWorkerLaunchKind.TimedOut, true, H1UnityEditorExecutionCoordinator.TimeoutCode, "interrupted")]
        [InlineData(H1UnityWorkerLaunchKind.TimedOut, false, H1UnityEditorExecutionCoordinator.IndeterminateCode, "indeterminate")]
        [InlineData(H1UnityWorkerLaunchKind.Cancelled, true, H1UnityEditorExecutionCoordinator.InterruptedCode, "interrupted")]
        [InlineData(H1UnityWorkerLaunchKind.Cancelled, false, H1UnityEditorExecutionCoordinator.IndeterminateCode, "indeterminate")]
        public async Task Launch_failure_class_is_preserved_in_status_recovery(
            H1UnityWorkerLaunchKind kind,
            bool terminationConfirmed,
            string expectedCode,
            string expectedStatus)
        {
            var ledger = new MemoryLedger();
            var launcher = new FakeLauncher((_, __) => H1UnityWorkerLaunchResult.Failure(kind, kind == H1UnityWorkerLaunchKind.NonZeroExit ? 91 : null, terminationConfirmed));
            using var harness = CreateHarness(launcher, ledger: ledger);

            var outcome = await InvokeAsync(harness.Projection, ProjectProfileInspectExecutor.Key.Name);

            Assert.False(outcome.Success);
            Assert.Equal(expectedCode, outcome.Error!.MachineCode);
            Assert.True(outcome.Error.Context.TryGetValue("invocationId", out var rawInvocation));
            var invocationId = Assert.IsType<string>(rawInvocation);
            var status = await InvokeAsync(
                harness.Projection,
                "unity.lifecycle.operation-status",
                new Dictionary<string, object?>(StringComparer.Ordinal) { ["invocationId"] = invocationId });
            Assert.True(status.Success);
            Assert.Equal(expectedStatus, status.Result!["status"]);
            Assert.Equal(expectedCode, status.Result["outcomeCode"]);
        }

        [Fact]
        public async Task Wrong_result_identity_is_rejected_before_public_decode()
        {
            var launcher = new FakeLauncher((invocation, _) => H1UnityWorkerLaunchResult.Completed(new H1UnityResultEnvelope(
                invocation.InvocationId + "-wrong",
                invocation.Capability,
                invocation.ExecutorId,
                invocation.ProfileId,
                invocation.ProjectIdentity,
                invocation.EditorVersion,
                invocation.EditorRevision,
                true,
                invocation.Payload)));
            using var harness = CreateHarness(launcher);

            var outcome = await InvokeAsync(harness.Projection, ProjectProfileInspectExecutor.Key.Name);

            Assert.False(outcome.Success);
            Assert.Equal(H1UnityEditorExecutionCoordinator.WrongIdentityCode, outcome.Error!.MachineCode);
        }

        [Fact]
        public async Task Off_main_thread_result_is_rejected_even_when_identity_matches()
        {
            var launcher = new FakeLauncher((invocation, _) => H1UnityWorkerLaunchResult.Completed(Matching(invocation, false)));
            using var harness = CreateHarness(launcher);

            var outcome = await InvokeAsync(harness.Projection, HierarchyProbeExecutor.Key.Name);

            Assert.False(outcome.Success);
            Assert.Equal(H1UnityEditorExecutionCoordinator.OffMainThreadCode, outcome.Error!.MachineCode);
        }

        [Fact]
        public async Task Hierarchy_probe_returns_the_frozen_H1_00_shape_only_after_main_thread_proof()
        {
            var launcher = new FakeLauncher((invocation, _) => H1UnityWorkerLaunchResult.Completed(Matching(invocation, true)));
            using var harness = CreateHarness(launcher);

            var outcome = await InvokeAsync(harness.Projection, HierarchyProbeExecutor.Key.Name);

            Assert.True(outcome.Success);
            var hierarchy = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(outcome.Result!["hierarchy"]);
            Assert.Equal("diagnostic-root", hierarchy["rootId"]);
            Assert.Equal("diagnostic-child", hierarchy["childId"]);
            Assert.Equal("Transform", hierarchy["component"]);
            Assert.Equal(true, hierarchy["active"]);
            Assert.Equal(true, outcome.Result["mainThread"]);
        }

        [Fact]
        public async Task Project_local_ledger_survives_host_reconstruction_and_repeated_status_reads_are_stable()
        {
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var invocationId = "h1u-restart-proof-" + Guid.NewGuid().ToString("N");
            var firstLedger = new FileH1UnityInvocationLedger(profile);
            firstLedger.Record(new H1UnityInvocationRecord(invocationId, ProjectProfileInspectExecutor.Key.ToString(), H1UnityInvocationStatus.Indeterminate, H1UnityEditorExecutionCoordinator.IndeterminateCode));
            var secondLedger = new FileH1UnityInvocationLedger(profile);
            Assert.True(secondLedger.TryRead(invocationId, out var recovered));
            Assert.NotNull(recovered);
            Assert.Equal(H1UnityInvocationStatus.Indeterminate, recovered!.Status);

            var launcher = new FakeLauncher((invocation, _) => Completed(invocation));
            using var harness = CreateHarness(launcher, ledger: secondLedger);
            var arguments = new Dictionary<string, object?>(StringComparer.Ordinal) { ["invocationId"] = invocationId };
            var first = await InvokeAsync(harness.Projection, "unity.lifecycle.operation-status", arguments);
            var second = await InvokeAsync(harness.Projection, "unity.lifecycle.operation-status", arguments);

            Assert.True(first.Success);
            Assert.True(second.Success);
            Assert.Equal(first.Result!["status"], second.Result!["status"]);
            Assert.Equal(first.Result["outcomeCode"], second.Result["outcomeCode"]);
            Assert.Equal(0, launcher.Count);
        }

        private static Harness CreateHarness(
            FakeLauncher launcher,
            IH1UnityProjectLease? lease = null,
            IH1UnityInvocationLedger? ledger = null)
        {
            ledger ??= new MemoryLedger();
            var coordinator = new H1UnityEditorExecutionCoordinator(
                H1UnityLaunchProfile.ForCurrentHost(),
                launcher,
                lease ?? new AvailableLease(),
                ledger,
                new IH1UnityCapabilityExecutor[]
                {
                    new ProjectProfileInspectExecutor(),
                    new HierarchyProbeExecutor()
                });
            var projection = CreateProjection(coordinator, ledger);
            coordinator.Bind(projection);
            return new Harness(projection);
        }

        private static H1UnityEditorExecutionCoordinator Coordinator(FakeLauncher launcher)
        {
            return new H1UnityEditorExecutionCoordinator(
                H1UnityLaunchProfile.ForCurrentHost(),
                launcher,
                new AvailableLease(),
                new MemoryLedger(),
                new IH1UnityCapabilityExecutor[]
                {
                    new ProjectProfileInspectExecutor(),
                    new HierarchyProbeExecutor()
                });
        }

        private static NeutralProjectionService CreateProjection(
            H1UnityEditorExecutionCoordinator coordinator,
            IH1UnityInvocationLedger ledger)
        {
            var composition = ContractComposer.Compose(
                CanonicalWorldContract.CreateEmptyPortableSessionContribution(ProductionHarnessHost.InitialWorldId),
                new[]
                {
                    UnityAuthoringProvider.CreateContribution(),
                    H1UnityLifecycleContract.CreateEditorHostContribution(coordinator),
                    H1UnityLifecycleContract.CreateLifecycleStatusContribution(ledger)
                });
            Assert.True(composition.Success, composition.Issues.Count == 0 ? "unknown composition failure" : composition.Issues[0].Code);
            return H1UnityHostCapabilityPolicy.CreateProjection(
                composition.Contract!,
                UnityProjectWorkspaceAuthority.ForArkusUnityProject(),
                H1UnityLifecycleContract.CreateGrants());
        }

        private static Task<NeutralProjectionOutcome> InvokeAsync(
            NeutralProjectionService projection,
            string capability,
            CancellationToken cancellationToken = default)
        {
            return InvokeAsync(projection, capability, Empty(), cancellationToken);
        }

        private static Task<NeutralProjectionOutcome> InvokeAsync(
            NeutralProjectionService projection,
            string capability,
            IReadOnlyDictionary<string, object?> arguments,
            CancellationToken cancellationToken = default)
        {
            return projection.InvokeAsync(
                new NeutralProjectionRequest(
                    "test-" + Guid.NewGuid().ToString("N"),
                    capability,
                    new ContractVersionRange(1, 0, 0),
                    arguments,
                    1000),
                cancellationToken);
        }

        private static IReadOnlyDictionary<string, object?> Empty() =>
            new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal));

        private static H1UnityWorkerLaunchResult Completed(H1UnityInvocationEnvelope invocation) =>
            H1UnityWorkerLaunchResult.Completed(Matching(invocation, true));

        private static H1UnityResultEnvelope Matching(H1UnityInvocationEnvelope invocation, bool mainThread)
        {
            return new H1UnityResultEnvelope(
                invocation.InvocationId,
                invocation.Capability,
                invocation.ExecutorId,
                invocation.ProfileId,
                invocation.ProjectIdentity,
                invocation.EditorVersion,
                invocation.EditorRevision,
                mainThread,
                invocation.Payload);
        }

        private sealed class Harness : IDisposable
        {
            public Harness(NeutralProjectionService projection) { Projection = projection; }
            public NeutralProjectionService Projection { get; }
            public void Dispose() => Projection.Dispose();
        }

        private sealed class FakeLauncher : IH1UnityEditorWorkerLauncher
        {
            private readonly Func<H1UnityInvocationEnvelope, InvocationResourceBudget, H1UnityWorkerLaunchResult> _launch;
            public FakeLauncher(Func<H1UnityInvocationEnvelope, InvocationResourceBudget, H1UnityWorkerLaunchResult> launch) { _launch = launch; }
            public int Count { get; private set; }
            public H1UnityWorkerLaunchResult Launch(H1UnityInvocationEnvelope invocation, H1UnityLaunchProfile profile, InvocationResourceBudget executionBudget)
            {
                Count++;
                return _launch(invocation, executionBudget);
            }
        }

        private sealed class AvailableLease : IH1UnityProjectLease
        {
            public IDisposable? TryAcquire() => new Releaser();
        }

        private sealed class BusyLease : IH1UnityProjectLease
        {
            public IDisposable? TryAcquire() => null;
        }

        private sealed class Releaser : IDisposable
        {
            public void Dispose() { }
        }

        private sealed class MemoryLedger : IH1UnityInvocationLedger
        {
            private readonly Dictionary<string, H1UnityInvocationRecord> _records = new Dictionary<string, H1UnityInvocationRecord>(StringComparer.Ordinal);
            public void Record(H1UnityInvocationRecord record) { _records[record.InvocationId] = record; }
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

        private sealed class OrphanExecutor : IH1UnityCapabilityExecutor
        {
            public CapabilityKey Capability { get; } = new CapabilityKey("unity.host.orphan.inspect", new ContractVersion(1, 0));
            public string ExecutorId => "arkus.h1.worker.orphan@1";
            public string EncodeRequest(IReadOnlyDictionary<string, object?> request) => "orphan";
            public CapabilityInvocationResult DecodeResult(H1UnityResultEnvelope result) => throw new InvalidOperationException("Orphan executor must never be dispatched.");
        }
    }
}
