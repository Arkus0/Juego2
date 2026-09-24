using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;

namespace Arkus.H1.UnityHost
{
    public sealed class H1UnityLaunchProfile
    {
        public const string ProfileId = "arkus.h1-unity-launch-profile@1";
        public const string InvocationSchemaId = "arkus.h1-unity-invocation@1";
        public const string ResultSchemaId = "arkus.h1-unity-result@1";
        public const string FixedEntryPoint = "Arkus.H1.Editor.H1EditorWorker.Run";
        public const string EditorVersion = "6000.3.24f1";
        public const string EditorRevision = "4e7b9b5b6244";

        private H1UnityLaunchProfile(string repositoryRoot, string executablePath, string platform)
        {
            RepositoryRoot = repositoryRoot;
            ProjectRoot = Path.GetFullPath(Path.Combine(repositoryRoot, UnityProjectWorkspaceAuthority.ProjectRoot));
            ExecutablePath = executablePath;
            Platform = platform;
        }

        public string Id => ProfileId;
        public string RepositoryRoot { get; }
        public string ProjectRoot { get; }
        public string ExecutablePath { get; }
        public string Platform { get; }
        public string ProjectIdentity => UnityProjectWorkspaceAuthority.ProjectIdentity;
        public string EntryPoint => FixedEntryPoint;
        public string EffectiveEditorVersion => EditorVersion;
        public string EffectiveEditorRevision => EditorRevision;
        public string OperationCeilingsId => H1UnityOperationCeilings.SchemaId;

        public static H1UnityLaunchProfile ForCurrentHost()
        {
            var repositoryRoot = FindRepositoryRoot();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new H1UnityLaunchProfile(repositoryRoot, @"C:\Program Files\Unity\Hub\Editor\6000.3.24f1\Editor\Unity.exe", "windows-x64");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return new H1UnityLaunchProfile(repositoryRoot, "/Applications/Unity/Hub/Editor/6000.3.24f1/Unity.app/Contents/MacOS/Unity", "macos");
            return new H1UnityLaunchProfile(repositoryRoot, "/opt/unity/Editor/Unity", "linux-x64");
        }

        public IReadOnlyDictionary<string, object?> ToPublicData()
        {
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["profileId"] = Id,
                ["projectIdentity"] = ProjectIdentity,
                ["entryPoint"] = EntryPoint,
                ["platform"] = Platform,
                ["editorVersion"] = EffectiveEditorVersion,
                ["editorRevision"] = EffectiveEditorRevision,
                ["operationCeilingsId"] = OperationCeilingsId,
                ["operationCeilings"] = H1UnityOperationCeilings.ToData()
            });
        }

        private static string FindRepositoryRoot()
        {
            var probes = new[] { Environment.CurrentDirectory, AppContext.BaseDirectory };
            foreach (var probe in probes)
            {
                DirectoryInfo? current = new DirectoryInfo(Path.GetFullPath(probe));
                while (current != null)
                {
                    if (File.Exists(Path.Combine(current.FullName, "Juego2.sln"))) return current.FullName;
                    current = current.Parent;
                }
            }
            throw new InvalidOperationException("Unable to locate the fixed Juego2 repository root for the H1 Unity launch profile.");
        }

        private static IReadOnlyDictionary<string, object?> ReadOnly(IDictionary<string, object?> source) =>
            new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(source, StringComparer.Ordinal));
    }

    public sealed class H1UnityInvocationEnvelope
    {
        public H1UnityInvocationEnvelope(string invocationId, string capability, string executorId, string payload, H1UnityLaunchProfile profile)
        {
            InvocationId = Require(invocationId, nameof(invocationId));
            Capability = Require(capability, nameof(capability));
            ExecutorId = Require(executorId, nameof(executorId));
            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            ProfileId = profile.Id;
            ProjectIdentity = profile.ProjectIdentity;
            EditorVersion = profile.EffectiveEditorVersion;
            EditorRevision = profile.EffectiveEditorRevision;
            EntryPoint = profile.EntryPoint;
        }

        public string SchemaId => H1UnityLaunchProfile.InvocationSchemaId;
        public string InvocationId { get; }
        public string Capability { get; }
        public string ExecutorId { get; }
        public string Payload { get; }
        public string ProfileId { get; }
        public string ProjectIdentity { get; }
        public string EditorVersion { get; }
        public string EditorRevision { get; }
        public string EntryPoint { get; }

        private static string Require(string value, string name) => string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("A non-empty value is required.", name) : value;
    }

    public sealed class H1UnityResultEnvelope
    {
        public H1UnityResultEnvelope(string invocationId, string capability, string executorId, string profileId, string projectIdentity, string editorVersion, string editorRevision, bool mainThread, string payload)
        {
            InvocationId = invocationId ?? throw new ArgumentNullException(nameof(invocationId));
            Capability = capability ?? throw new ArgumentNullException(nameof(capability));
            ExecutorId = executorId ?? throw new ArgumentNullException(nameof(executorId));
            ProfileId = profileId ?? throw new ArgumentNullException(nameof(profileId));
            ProjectIdentity = projectIdentity ?? throw new ArgumentNullException(nameof(projectIdentity));
            EditorVersion = editorVersion ?? throw new ArgumentNullException(nameof(editorVersion));
            EditorRevision = editorRevision ?? throw new ArgumentNullException(nameof(editorRevision));
            MainThread = mainThread;
            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
        }

        public string SchemaId => H1UnityLaunchProfile.ResultSchemaId;
        public string InvocationId { get; }
        public string Capability { get; }
        public string ExecutorId { get; }
        public string ProfileId { get; }
        public string ProjectIdentity { get; }
        public string EditorVersion { get; }
        public string EditorRevision { get; }
        public bool MainThread { get; }
        public string Payload { get; }
    }

    public enum H1UnityWorkerLaunchKind
    {
        Completed = 0,
        NonZeroExit = 1,
        TimedOut = 2,
        Cancelled = 3,
        MissingResult = 4,
        CorruptResult = 5,
        StartFailure = 6
    }

    public sealed class H1UnityWorkerLaunchResult
    {
        private H1UnityWorkerLaunchResult(H1UnityWorkerLaunchKind kind, H1UnityResultEnvelope? result, int? exitCode, bool terminationConfirmed)
        {
            Kind = kind;
            Result = result;
            ExitCode = exitCode;
            TerminationConfirmed = terminationConfirmed;
        }

        public H1UnityWorkerLaunchKind Kind { get; }
        public H1UnityResultEnvelope? Result { get; }
        public int? ExitCode { get; }
        public bool TerminationConfirmed { get; }
        public static H1UnityWorkerLaunchResult Completed(H1UnityResultEnvelope result) => new H1UnityWorkerLaunchResult(H1UnityWorkerLaunchKind.Completed, result ?? throw new ArgumentNullException(nameof(result)), 0, true);
        public static H1UnityWorkerLaunchResult Failure(H1UnityWorkerLaunchKind kind, int? exitCode = null, bool terminationConfirmed = true) => kind == H1UnityWorkerLaunchKind.Completed ? throw new ArgumentException("Use Completed for successful launch results.", nameof(kind)) : new H1UnityWorkerLaunchResult(kind, null, exitCode, terminationConfirmed);
    }

    public interface IH1UnityEditorWorkerLauncher
    {
        H1UnityWorkerLaunchResult Launch(H1UnityInvocationEnvelope invocation, H1UnityLaunchProfile profile, InvocationResourceBudget executionBudget);
    }

    public interface IH1UnityCapabilityExecutor
    {
        CapabilityKey Capability { get; }
        string ExecutorId { get; }
        string EncodeRequest(IReadOnlyDictionary<string, object?> request);
        CapabilityInvocationResult DecodeResult(H1UnityResultEnvelope result);
    }

    public interface IH1UnityProjectLease
    {
        IDisposable? TryAcquire();
    }

    public enum H1UnityInvocationStatus
    {
        Running = 0,
        Completed = 1,
        Failed = 2,
        Interrupted = 3,
        Indeterminate = 4
    }

    public sealed class H1UnityInvocationRecord
    {
        public H1UnityInvocationRecord(string invocationId, string capability, H1UnityInvocationStatus status, string outcomeCode)
        {
            InvocationId = invocationId ?? throw new ArgumentNullException(nameof(invocationId));
            Capability = capability ?? throw new ArgumentNullException(nameof(capability));
            Status = status;
            OutcomeCode = outcomeCode ?? throw new ArgumentNullException(nameof(outcomeCode));
        }

        public string InvocationId { get; }
        public string Capability { get; }
        public H1UnityInvocationStatus Status { get; }
        public string OutcomeCode { get; }
    }

    public interface IH1UnityInvocationLedger
    {
        void Record(H1UnityInvocationRecord record);
        bool TryRead(string invocationId, out H1UnityInvocationRecord? record);
    }

    public sealed class H1UnityEditorExecutionCoordinator
    {
        public const string BusyCode = "unity.lifecycle.busy-project";
        public const string CrashCode = "unity.lifecycle.editor-crash";
        public const string TimeoutCode = "unity.lifecycle.timeout";
        public const string InterruptedCode = "unity.lifecycle.interrupted";
        public const string IndeterminateCode = "unity.lifecycle.indeterminate";
        public const string MissingResultCode = "unity.lifecycle.missing-result";
        public const string CorruptResultCode = "unity.lifecycle.corrupt-result";
        public const string WrongIdentityCode = "unity.lifecycle.wrong-result-identity";
        public const string OffMainThreadCode = "unity.lifecycle.off-main-thread";
        public const string BootstrapIncompleteCode = "unity.lifecycle.bootstrap-incomplete";

        private readonly H1UnityLaunchProfile _profile;
        private readonly IH1UnityEditorWorkerLauncher _launcher;
        private readonly IH1UnityProjectLease _lease;
        private readonly IH1UnityInvocationLedger _ledger;
        private readonly IReadOnlyDictionary<CapabilityKey, IH1UnityCapabilityExecutor> _executors;
        private bool _bound;

        public H1UnityEditorExecutionCoordinator(H1UnityLaunchProfile profile, IH1UnityEditorWorkerLauncher launcher, IH1UnityProjectLease lease, IH1UnityInvocationLedger ledger, IEnumerable<IH1UnityCapabilityExecutor> executors)
        {
            _profile = profile ?? throw new ArgumentNullException(nameof(profile));
            _launcher = launcher ?? throw new ArgumentNullException(nameof(launcher));
            _lease = lease ?? throw new ArgumentNullException(nameof(lease));
            _ledger = ledger ?? throw new ArgumentNullException(nameof(ledger));
            if (executors == null) throw new ArgumentNullException(nameof(executors));
            var snapshot = new Dictionary<CapabilityKey, IH1UnityCapabilityExecutor>();
            foreach (var executor in executors)
            {
                if (executor == null) throw new ArgumentException("Worker executors may not contain null entries.", nameof(executors));
                if (!snapshot.TryAdd(executor.Capability, executor)) throw new ArgumentException("Each Editor-bound capability must have exactly one typed worker executor.", nameof(executors));
            }
            _executors = new ReadOnlyDictionary<CapabilityKey, IH1UnityCapabilityExecutor>(snapshot);
        }

        public H1UnityLaunchProfile Profile => _profile;
        public IH1UnityInvocationLedger Ledger => _ledger;

        public void Bind(NeutralProjectionService admittedProjection)
        {
            if (admittedProjection == null) throw new ArgumentNullException(nameof(admittedProjection));
            var admitted = new HashSet<CapabilityKey>();
            foreach (var definition in admittedProjection.Capabilities)
            {
                if (string.Equals(definition.Provider.ProviderId, H1UnityHostCapabilityPolicy.HostProviderId, StringComparison.Ordinal)) admitted.Add(definition.Key);
            }
            foreach (var capability in admitted)
            {
                if (!_executors.ContainsKey(capability)) throw new InvalidOperationException(BootstrapIncompleteCode + ": missing worker executor for " + capability + ".");
            }
            foreach (var capability in _executors.Keys)
            {
                if (!admitted.Contains(capability)) throw new InvalidOperationException(BootstrapIncompleteCode + ": orphan worker executor for " + capability + ".");
            }
            if (admitted.Count == 0) throw new InvalidOperationException(BootstrapIncompleteCode + ": admitted composition has no Editor-bound capability.");
            _bound = true;
        }

        public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (!_bound) return Failure(BootstrapIncompleteCode, "The H1 Unity lifecycle is not bound to its admitted composition.", false);
            if (!_executors.TryGetValue(context.Definition.Key, out var executor)) return Failure(BootstrapIncompleteCode, "No typed Unity worker executor is paired with the composed handler.", false);
            if (!context.ResourceBudget.TryContinue("h1-unity-prelaunch", out var prelaunchError)) return CapabilityInvocationResult.Failed(prelaunchError!);
            var lease = _lease.TryAcquire();
            if (lease == null) return Failure(BusyCode, "The reviewed Unity project already has an active Editor operation.", true);
            using (lease)
            {
                if (!context.ResourceBudget.TryContinue("h1-unity-after-lease", out prelaunchError)) return CapabilityInvocationResult.Failed(prelaunchError!);
                string payload;
                try { payload = executor.EncodeRequest(request); }
                catch (Exception) { return Failure(CorruptResultCode, "The typed worker request encoder rejected the canonical request.", false); }
                if (System.Text.Encoding.UTF8.GetByteCount(payload) > H1UnityOperationCeilings.MaximumPayloadBytes) return Failure("unity.lifecycle.payload-too-large", "The internal worker payload exceeded the fixed H1 ceiling.", false);

                var invocationId = CreateInvocationId();
                var envelope = new H1UnityInvocationEnvelope(invocationId, context.Definition.Key.ToString(), executor.ExecutorId, payload, _profile);
                _ledger.Record(new H1UnityInvocationRecord(invocationId, envelope.Capability, H1UnityInvocationStatus.Running, "running"));
                H1UnityWorkerLaunchResult launch;
                try { launch = _launcher.Launch(envelope, _profile, context.ResourceBudget); }
                catch (Exception)
                {
                    _ledger.Record(new H1UnityInvocationRecord(invocationId, envelope.Capability, H1UnityInvocationStatus.Indeterminate, IndeterminateCode));
                    return Failure(IndeterminateCode, "The Unity worker launcher failed without a trustworthy result.", true, invocationId);
                }
                if (launch.Kind != H1UnityWorkerLaunchKind.Completed) return MapLaunchFailure(envelope, launch);
                var result = launch.Result;
                if (result == null) return RecordAndFail(envelope, H1UnityInvocationStatus.Indeterminate, MissingResultCode, "The Unity worker exited without a result envelope.", true);
                if (!Matches(envelope, result)) return RecordAndFail(envelope, H1UnityInvocationStatus.Indeterminate, WrongIdentityCode, "The Unity result does not match the invocation/profile identity.", false);
                if (!result.MainThread) return RecordAndFail(envelope, H1UnityInvocationStatus.Failed, OffMainThreadCode, "The Unity worker did not execute the Editor-bound plan on the Editor main thread.", false);
                CapabilityInvocationResult decoded;
                try { decoded = executor.DecodeResult(result); }
                catch (Exception) { return RecordAndFail(envelope, H1UnityInvocationStatus.Indeterminate, CorruptResultCode, "The typed worker result decoder rejected the result envelope.", false); }
                _ledger.Record(new H1UnityInvocationRecord(invocationId, envelope.Capability, decoded.Success ? H1UnityInvocationStatus.Completed : H1UnityInvocationStatus.Failed, decoded.Success ? "success" : decoded.Error?.MachineCode ?? "structured-error"));
                return decoded;
            }
        }

        private CapabilityInvocationResult MapLaunchFailure(H1UnityInvocationEnvelope envelope, H1UnityWorkerLaunchResult launch)
        {
            switch (launch.Kind)
            {
                case H1UnityWorkerLaunchKind.NonZeroExit:
                case H1UnityWorkerLaunchKind.StartFailure:
                    return RecordAndFail(envelope, H1UnityInvocationStatus.Indeterminate, CrashCode, "The Unity Editor worker failed before a trustworthy result was accepted.", true);
                case H1UnityWorkerLaunchKind.TimedOut:
                    return RecordAndFail(envelope, launch.TerminationConfirmed ? H1UnityInvocationStatus.Interrupted : H1UnityInvocationStatus.Indeterminate, launch.TerminationConfirmed ? TimeoutCode : IndeterminateCode, launch.TerminationConfirmed ? "The Unity Editor worker exceeded its fixed deadline and was terminated." : "The Unity Editor worker exceeded its deadline and termination could not be proven.", true);
                case H1UnityWorkerLaunchKind.Cancelled:
                    return RecordAndFail(envelope, launch.TerminationConfirmed ? H1UnityInvocationStatus.Interrupted : H1UnityInvocationStatus.Indeterminate, launch.TerminationConfirmed ? InterruptedCode : IndeterminateCode, launch.TerminationConfirmed ? "The launched Unity Editor worker was interrupted by cancellation." : "Cancellation occurred after launch and worker termination could not be proven.", true);
                case H1UnityWorkerLaunchKind.MissingResult:
                    return RecordAndFail(envelope, H1UnityInvocationStatus.Indeterminate, MissingResultCode, "The Unity Editor worker produced no result envelope.", true);
                case H1UnityWorkerLaunchKind.CorruptResult:
                    return RecordAndFail(envelope, H1UnityInvocationStatus.Indeterminate, CorruptResultCode, "The Unity Editor worker produced an invalid result envelope.", false);
                default:
                    return RecordAndFail(envelope, H1UnityInvocationStatus.Indeterminate, IndeterminateCode, "The Unity Editor worker outcome is indeterminate.", true);
            }
        }

        private CapabilityInvocationResult RecordAndFail(H1UnityInvocationEnvelope envelope, H1UnityInvocationStatus status, string code, string message, bool retryable)
        {
            _ledger.Record(new H1UnityInvocationRecord(envelope.InvocationId, envelope.Capability, status, code));
            return Failure(code, message, retryable, envelope.InvocationId);
        }

        private bool Matches(H1UnityInvocationEnvelope invocation, H1UnityResultEnvelope result) =>
            string.Equals(result.InvocationId, invocation.InvocationId, StringComparison.Ordinal) &&
            string.Equals(result.Capability, invocation.Capability, StringComparison.Ordinal) &&
            string.Equals(result.ExecutorId, invocation.ExecutorId, StringComparison.Ordinal) &&
            string.Equals(result.ProfileId, _profile.Id, StringComparison.Ordinal) &&
            string.Equals(result.ProjectIdentity, _profile.ProjectIdentity, StringComparison.Ordinal) &&
            string.Equals(result.EditorVersion, _profile.EffectiveEditorVersion, StringComparison.Ordinal) &&
            string.Equals(result.EditorRevision, _profile.EffectiveEditorRevision, StringComparison.Ordinal);

        private static CapabilityInvocationResult Failure(string code, string message, bool retryable, string? invocationId = null)
        {
            var details = new Dictionary<string, object?>(StringComparer.Ordinal);
            if (invocationId != null) details["invocationId"] = invocationId;
            return CapabilityInvocationResult.Failed(new StructuredError(code, message, "$", new ReadOnlyDictionary<string, object?>(details), retryable, retryable ? "Query unity.lifecycle.operation-status with the invocation identity before retrying." : "Repair the host/worker contract mismatch before retrying."));
        }

        private static string CreateInvocationId() => "h1u-" + DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture) + "-" + Guid.NewGuid().ToString("N");
    }

    public sealed class ProjectProfileInspectExecutor : IH1UnityCapabilityExecutor
    {
        public static readonly CapabilityKey Key = new CapabilityKey("unity.host.project-profile.inspect", new ContractVersion(1, 0));
        public const string WorkerExecutorId = "arkus.h1.worker.project-profile.inspect@1";
        public CapabilityKey Capability => Key;
        public string ExecutorId => WorkerExecutorId;
        public string EncodeRequest(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.Count != 0) throw new InvalidOperationException("Project/profile inspection accepts no public authority selectors.");
            return "inspect";
        }
        public CapabilityInvocationResult DecodeResult(H1UnityResultEnvelope result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (!string.Equals(result.Payload, "inspect", StringComparison.Ordinal)) throw new FormatException("Unexpected inspection payload.");
            var publicProfile = H1UnityLaunchProfile.ForCurrentHost().ToPublicData();
            return CapabilityInvocationResult.Succeeded(ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = "arkus.h1-unity-project-profile-inspection@1",
                ["invocationId"] = result.InvocationId,
                ["profileId"] = result.ProfileId,
                ["projectIdentity"] = result.ProjectIdentity,
                ["entryPoint"] = publicProfile["entryPoint"],
                ["platform"] = publicProfile["platform"],
                ["editorVersion"] = result.EditorVersion,
                ["editorRevision"] = result.EditorRevision,
                ["operationCeilingsId"] = publicProfile["operationCeilingsId"],
                ["operationCeilings"] = publicProfile["operationCeilings"],
                ["mainThread"] = result.MainThread,
                ["executorId"] = result.ExecutorId
            }));
        }
        private static IReadOnlyDictionary<string, object?> ReadOnly(IDictionary<string, object?> source) => new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(source, StringComparer.Ordinal));
    }

    public sealed class HierarchyProbeExecutor : IH1UnityCapabilityExecutor
    {
        public static readonly CapabilityKey Key = new CapabilityKey("unity.host.hierarchy-probe.inspect", new ContractVersion(1, 0));
        public const string WorkerExecutorId = "arkus.h1.worker.hierarchy-probe.inspect@1";
        public const string Payload = "hierarchy-probe:v1|root=diagnostic-root|child=diagnostic-child|component=Transform|active=true";
        public CapabilityKey Capability => Key;
        public string ExecutorId => WorkerExecutorId;
        public string EncodeRequest(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.Count != 0) throw new InvalidOperationException("Hierarchy probe accepts no public authority selectors.");
            return Payload;
        }
        public CapabilityInvocationResult DecodeResult(H1UnityResultEnvelope result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (!string.Equals(result.Payload, Payload, StringComparison.Ordinal)) throw new FormatException("Hierarchy probe payload did not round-trip exactly.");
            var hierarchy = new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["rootId"] = "diagnostic-root",
                ["childId"] = "diagnostic-child",
                ["component"] = "Transform",
                ["active"] = true
            });
            return CapabilityInvocationResult.Succeeded(new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = "arkus.h1-unity-hierarchy-probe@1",
                ["invocationId"] = result.InvocationId,
                ["hierarchy"] = hierarchy,
                ["mainThread"] = result.MainThread
            }));
        }
    }

    [PublicCapabilityRoute(H1UnityHostCapabilityPolicy.HostProviderId, "unity.host.project-profile.inspect", "1.0")]
    public sealed class ProjectProfileInspectHandler : ICanonicalCapabilityHandler
    {
        private readonly H1UnityEditorExecutionCoordinator _coordinator;
        public ProjectProfileInspectHandler(H1UnityEditorExecutionCoordinator coordinator) { _coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator)); }
        public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request) => _coordinator.Invoke(context, request);
    }

    [PublicCapabilityRoute(H1UnityHostCapabilityPolicy.HostProviderId, "unity.host.hierarchy-probe.inspect", "1.0")]
    public sealed class HierarchyProbeHandler : ICanonicalCapabilityHandler
    {
        private readonly H1UnityEditorExecutionCoordinator _coordinator;
        public HierarchyProbeHandler(H1UnityEditorExecutionCoordinator coordinator) { _coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator)); }
        public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request) => _coordinator.Invoke(context, request);
    }

    [PublicCapabilityRoute("arkus.unity-lifecycle", "unity.lifecycle.operation-status", "1.0")]
    public sealed class OperationStatusHandler : ICanonicalCapabilityHandler
    {
        private readonly IH1UnityInvocationLedger _ledger;
        public OperationStatusHandler(IH1UnityInvocationLedger ledger) { _ledger = ledger ?? throw new ArgumentNullException(nameof(ledger)); }
        public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (!request.TryGetValue("invocationId", out var raw) || !(raw is string invocationId) || !_ledger.TryRead(invocationId, out var record) || record == null)
                return CapabilityInvocationResult.Failed(new StructuredError("unity.lifecycle.unknown-invocation", "No bounded lifecycle record exists for the supplied invocation identity.", "$.invocationId", new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)), false, "Use an invocation identity returned by a structured H1 lifecycle outcome."));
            return CapabilityInvocationResult.Succeeded(new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = "arkus.h1-unity-operation-status@1",
                ["invocationId"] = record.InvocationId,
                ["capability"] = record.Capability,
                ["status"] = StatusToken(record.Status),
                ["outcomeCode"] = record.OutcomeCode
            }));
        }
        private static string StatusToken(H1UnityInvocationStatus status)
        {
            switch (status)
            {
                case H1UnityInvocationStatus.Running: return "running";
                case H1UnityInvocationStatus.Completed: return "completed";
                case H1UnityInvocationStatus.Failed: return "failed";
                case H1UnityInvocationStatus.Interrupted: return "interrupted";
                case H1UnityInvocationStatus.Indeterminate: return "indeterminate";
                default: throw new ArgumentOutOfRangeException(nameof(status));
            }
        }
    }

    public static class H1UnityLifecycleContract
    {
        public static CanonicalProviderContribution CreateEditorHostContribution(H1UnityEditorExecutionCoordinator coordinator, bool includeCatalogue = false, bool includeProjection = false)
        {
            if (coordinator == null) throw new ArgumentNullException(nameof(coordinator));
            var profile = Definition(ProjectProfileInspectExecutor.Key, ProjectProfileSuccessSchema(), "project/profile inspection");
            var hierarchy = Definition(HierarchyProbeExecutor.Key, HierarchyProbeSuccessSchema(), "hierarchy-shaped diagnostic probe");
            return new CanonicalProviderContribution(
                new ProviderDescriptor(H1UnityHostCapabilityPolicy.HostProviderId, ProviderKind.Scoped, H1UnityHostCapabilityPolicy.HostScope, new[] { H1UnityHostCapabilityPolicy.HostNamespace }),
                new[] { profile, hierarchy }
                    .Concat(includeCatalogue ? H1CatalogueContract.Definitions() : Array.Empty<CapabilityDefinition>())
                    .Concat(includeProjection ? H1ProjectionContract.EditorDefinitions() : Array.Empty<CapabilityDefinition>()).ToArray(),
                new[]
                {
                    CapabilityRoute.FromHandler(new ProjectProfileInspectHandler(coordinator)),
                    CapabilityRoute.FromHandler(new HierarchyProbeHandler(coordinator))
                }.Concat(includeCatalogue ? H1CatalogueContract.Routes(coordinator) : Array.Empty<CapabilityRoute>())
                 .Concat(includeProjection ? H1ProjectionContract.EditorRoutes(coordinator) : Array.Empty<CapabilityRoute>()).ToArray());
        }

        public static IReadOnlyList<UnityHostCapabilityGrant> CreateGrants(bool includeCatalogue = false, bool includeProjection = false)
        {
            return new List<UnityHostCapabilityGrant>
            {
                Grant(ProjectProfileInspectExecutor.Key),
                Grant(HierarchyProbeExecutor.Key)
            }.Concat(includeCatalogue ? H1CatalogueContract.Grants() : Array.Empty<UnityHostCapabilityGrant>())
             .Concat(includeProjection ? H1ProjectionContract.Grants() : Array.Empty<UnityHostCapabilityGrant>()).ToList().AsReadOnly();
        }

        public static CanonicalProviderContribution CreateLifecycleStatusContribution(IH1UnityInvocationLedger ledger)
        {
            if (ledger == null) throw new ArgumentNullException(nameof(ledger));
            var key = new CapabilityKey("unity.lifecycle.operation-status", new ContractVersion(1, 0));
            var definition = new CapabilityDefinition(
                key,
                new ProviderMetadata("arkus.unity-lifecycle", ProviderKind.Scoped, "unity-lifecycle", "unity.lifecycle"),
                new JsonSchemaDocument(SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal) { ["invocationId"] = SchemaNode.String() }, new[] { "invocationId" })),
                OperationStatusSuccessSchema(),
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.ReadOnly,
                DeterminismClass.EnvironmentDependent,
                Array.Empty<string>(),
                new[] { "bounded-ledger-status-returned" },
                new ConcurrencySemantics(ConcurrencyClass.ParallelSafe),
                new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Unsupported),
                new RepairSemantics(false, true),
                new PolicySemantics(PrivilegeClass.PublicRead, TransactionRequirement.ReadOnlyEnvelope, ProvenanceRequirement.Required),
                new CostSemantics(1, "bounded project-local lifecycle ledger lookup"));
            return new CanonicalProviderContribution(
                new ProviderDescriptor("arkus.unity-lifecycle", ProviderKind.Scoped, "unity-lifecycle", new[] { "unity.lifecycle" }),
                new[] { definition },
                new[] { CapabilityRoute.FromHandler(new OperationStatusHandler(ledger)) });
        }

        private static CapabilityDefinition Definition(CapabilityKey key, JsonSchemaDocument successSchema, string cost)
        {
            return new CapabilityDefinition(
                key,
                new ProviderMetadata(H1UnityHostCapabilityPolicy.HostProviderId, ProviderKind.Scoped, H1UnityHostCapabilityPolicy.HostScope, H1UnityHostCapabilityPolicy.HostNamespace),
                CanonicalContractSchemas.EmptyObject(),
                successSchema,
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.ReadOnly,
                DeterminismClass.EnvironmentDependent,
                new[] { "h1-unity-profile-bound", "project-operation-lease-available" },
                new[] { "unity-main-thread-observation-returned" },
                new ConcurrencySemantics(ConcurrencyClass.Serialized),
                new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Unsupported),
                new RepairSemantics(true, true),
                new PolicySemantics(PrivilegeClass.Authoring, TransactionRequirement.None, ProvenanceRequirement.Required),
                new CostSemantics(1, "one short-lived Unity Editor worker: " + cost));
        }

        private static UnityHostCapabilityGrant Grant(CapabilityKey key) => new UnityHostCapabilityGrant(key, UnityProjectWorkspaceAuthority.ProjectSettingsRootId, "ref.arkus.unity-host.project-metadata", UnityHostResourceClass.ProjectMetadata, UnityHostTimeClass.BoundedRead);

        private static JsonSchemaDocument ProjectProfileSuccessSchema() => new JsonSchemaDocument(SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
        {
            ["schemaId"] = SchemaNode.String(new[] { "arkus.h1-unity-project-profile-inspection@1" }),
            ["invocationId"] = SchemaNode.String(),
            ["profileId"] = SchemaNode.String(new[] { H1UnityLaunchProfile.ProfileId }),
            ["projectIdentity"] = SchemaNode.String(new[] { UnityProjectWorkspaceAuthority.ProjectIdentity }),
            ["entryPoint"] = SchemaNode.String(new[] { H1UnityLaunchProfile.FixedEntryPoint }),
            ["platform"] = SchemaNode.String(new[] { "windows-x64", "macos", "linux-x64" }),
            ["editorVersion"] = SchemaNode.String(new[] { H1UnityLaunchProfile.EditorVersion }),
            ["editorRevision"] = SchemaNode.String(new[] { H1UnityLaunchProfile.EditorRevision }),
            ["operationCeilingsId"] = SchemaNode.String(new[] { H1UnityOperationCeilings.SchemaId }),
            ["operationCeilings"] = OperationCeilingsSchema(),
            ["mainThread"] = SchemaNode.Boolean(),
            ["executorId"] = SchemaNode.String(new[] { ProjectProfileInspectExecutor.WorkerExecutorId })
        }, new[] { "schemaId", "invocationId", "profileId", "projectIdentity", "entryPoint", "platform", "editorVersion", "editorRevision", "operationCeilingsId", "operationCeilings", "mainThread", "executorId" }));

        private static JsonSchemaDocument HierarchyProbeSuccessSchema() => new JsonSchemaDocument(SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
        {
            ["schemaId"] = SchemaNode.String(new[] { "arkus.h1-unity-hierarchy-probe@1" }),
            ["invocationId"] = SchemaNode.String(),
            ["hierarchy"] = SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
            {
                ["rootId"] = SchemaNode.String(new[] { "diagnostic-root" }),
                ["childId"] = SchemaNode.String(new[] { "diagnostic-child" }),
                ["component"] = SchemaNode.String(new[] { "Transform" }),
                ["active"] = SchemaNode.Boolean()
            }, new[] { "rootId", "childId", "component", "active" }),
            ["mainThread"] = SchemaNode.Boolean()
        }, new[] { "schemaId", "invocationId", "hierarchy", "mainThread" }));

        private static SchemaNode OperationCeilingsSchema() => SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
        {
            ["schemaId"] = SchemaNode.String(new[] { H1UnityOperationCeilings.SchemaId }),
            ["maximumExecutionMilliseconds"] = SchemaNode.Integer(),
            ["maximumLedgerEntries"] = SchemaNode.Integer(),
            ["maximumPayloadBytes"] = SchemaNode.Integer(),
            ["h0EnvelopeUnchanged"] = SchemaNode.String(new[] { H0ResourceEnvelope.SchemaId })
        }, new[] { "schemaId", "maximumExecutionMilliseconds", "maximumLedgerEntries", "maximumPayloadBytes", "h0EnvelopeUnchanged" });

        private static JsonSchemaDocument OperationStatusSuccessSchema() => new JsonSchemaDocument(SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
        {
            ["schemaId"] = SchemaNode.String(new[] { "arkus.h1-unity-operation-status@1" }),
            ["invocationId"] = SchemaNode.String(),
            ["capability"] = SchemaNode.String(),
            ["status"] = SchemaNode.String(new[] { "running", "completed", "failed", "interrupted", "indeterminate" }),
            ["outcomeCode"] = SchemaNode.String()
        }, new[] { "schemaId", "invocationId", "capability", "status", "outcomeCode" }));
    }
}
