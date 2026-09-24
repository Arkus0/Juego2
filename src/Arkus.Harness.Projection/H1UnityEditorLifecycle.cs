using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;

namespace Arkus.Harness.Projection
{
    public static class H1UnityOperationCeilings
    {
        public const string SchemaId = "arkus.h1-unity-operation-ceilings@1";
        public const int MaximumExecutionMilliseconds = 120000;
        public const int MaximumLedgerEntries = 256;
        public const int MaximumPayloadBytes = 256 * 1024;

        public static IReadOnlyDictionary<string, object?> ToData()
        {
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = SchemaId,
                ["maximumExecutionMilliseconds"] = MaximumExecutionMilliseconds,
                ["maximumLedgerEntries"] = MaximumLedgerEntries,
                ["maximumPayloadBytes"] = MaximumPayloadBytes,
                ["h0EnvelopeUnchanged"] = H0ResourceEnvelope.SchemaId
            });
        }

        private static IReadOnlyDictionary<string, object?> ReadOnly(IDictionary<string, object?> source) =>
            new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(source, StringComparer.Ordinal));
    }

    public sealed class H1UnityLaunchProfile
    {
        public const string ProfileId = "arkus.h1-unity-launch-profile@1";
        public const string InvocationSchemaId = "arkus.h1-unity-invocation@1";
        public const string ResultSchemaId = "arkus.h1-unity-result@1";
        public const string FixedEntryPoint = "Arkus.H1.Editor.H1EditorWorker.Run";
        public const string EditorVersion = "6000.3.24f1";
        public const string EditorRevision = "4e7b9b5b6244";

        private H1UnityLaunchProfile(string executablePath, string platform, string repositoryRoot, string projectRoot)
        {
            ExecutablePath = executablePath;
            Platform = platform;
            RepositoryRoot = repositoryRoot;
            ProjectRoot = projectRoot;
        }

        public string Id => ProfileId;
        public string ExecutablePath { get; }
        public string Platform { get; }
        public string RepositoryRoot { get; }
        public string ProjectRoot { get; }
        public string ProjectIdentity => UnityProjectWorkspaceAuthority.ProjectIdentity;
        public string EntryPoint => FixedEntryPoint;
        public string EffectiveEditorVersion => EditorVersion;
        public string EffectiveEditorRevision => EditorRevision;
        public string OperationCeilingsId => H1UnityOperationCeilings.SchemaId;

        public static H1UnityLaunchProfile ForCurrentHost()
        {
            var repositoryRoot = Path.GetFullPath(Environment.CurrentDirectory);
            var projectRoot = Path.GetFullPath(Path.Combine(repositoryRoot, UnityProjectWorkspaceAuthority.ProjectRoot));
            string executable;
            string platform;
            if (Path.DirectorySeparatorChar == '\\')
            {
                executable = @"C:\Program Files\Unity\Hub\Editor\6000.3.24f1\Editor\Unity.exe";
                platform = "windows-x64";
            }
            else if (Directory.Exists("/Applications"))
            {
                executable = "/Applications/Unity/Hub/Editor/6000.3.24f1/Unity.app/Contents/MacOS/Unity";
                platform = "macos";
            }
            else
            {
                executable = "/opt/unity/Editor/Unity";
                platform = "linux-x64";
            }
            return new H1UnityLaunchProfile(executable, platform, repositoryRoot, projectRoot);
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
                ["operationCeilingsId"] = OperationCeilingsId
            });
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
            ProfileId = profile?.Id ?? throw new ArgumentNullException(nameof(profile));
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

        private static string Require(string value, string name) =>
            string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("A non-empty value is required.", name) : value;
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

    public enum H1UnityWorkerLaunchKind { Completed = 0, NonZeroExit = 1, TimedOut = 2, Cancelled = 3, MissingResult = 4, CorruptResult = 5, StartFailure = 6 }

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
        public static H1UnityWorkerLaunchResult Failure(H1UnityWorkerLaunchKind kind, int? exitCode = null, bool terminationConfirmed = true) =>
            kind == H1UnityWorkerLaunchKind.Completed ? throw new ArgumentException("Use Completed for a successful worker result.", nameof(kind)) : new H1UnityWorkerLaunchResult(kind, null, exitCode, terminationConfirmed);
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

    public interface IH1UnityProjectLease { IDisposable? TryAcquire(); }

    public enum H1UnityInvocationStatus { Running = 0, Completed = 1, Failed = 2, Interrupted = 3, Indeterminate = 4 }

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

    public sealed class FileH1UnityInvocationLedger : IH1UnityInvocationLedger
    {
        private readonly string _directory;
        public FileH1UnityInvocationLedger(H1UnityLaunchProfile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            _directory = Path.Combine(profile.ProjectRoot, "Library", "Arkus", "H1Lifecycle", "ledger");
        }

        public void Record(H1UnityInvocationRecord record)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));
            if (!IsSafeInvocationId(record.InvocationId)) throw new ArgumentException("Invocation IDs must be bounded host-generated identifiers.", nameof(record));
            Directory.CreateDirectory(_directory);
            var path = Path.Combine(_directory, record.InvocationId + ".ledger");
            var temp = path + ".tmp-" + Guid.NewGuid().ToString("N");
            File.WriteAllLines(temp, new[]
            {
                "schema=arkus.h1-unity-ledger@1",
                "invocation=" + Encode(record.InvocationId),
                "capability=" + Encode(record.Capability),
                "status=" + record.Status.ToString(),
                "outcome=" + Encode(record.OutcomeCode)
            });
            if (File.Exists(path)) File.Delete(path);
            File.Move(temp, path);
            BoundLedger();
        }

        public bool TryRead(string invocationId, out H1UnityInvocationRecord? record)
        {
            record = null;
            if (!IsSafeInvocationId(invocationId)) return false;
            var path = Path.Combine(_directory, invocationId + ".ledger");
            if (!File.Exists(path)) return false;
            try
            {
                var fields = Parse(File.ReadAllLines(path));
                if (!fields.TryGetValue("schema", out var schema) || schema != "arkus.h1-unity-ledger@1") return false;
                if (!fields.TryGetValue("invocation", out var encodedInvocation) || Decode(encodedInvocation) != invocationId) return false;
                if (!fields.TryGetValue("capability", out var encodedCapability)) return false;
                if (!fields.TryGetValue("status", out var statusText) || !Enum.TryParse(statusText, out H1UnityInvocationStatus status)) return false;
                if (!fields.TryGetValue("outcome", out var encodedOutcome)) return false;
                record = new H1UnityInvocationRecord(invocationId, Decode(encodedCapability), status, Decode(encodedOutcome));
                return true;
            }
            catch (IOException) { return false; }
            catch (FormatException) { return false; }
        }

        private void BoundLedger()
        {
            var files = new DirectoryInfo(_directory).GetFiles("*.ledger");
            if (files.Length <= H1UnityOperationCeilings.MaximumLedgerEntries) return;
            Array.Sort(files, (left, right) => left.LastWriteTimeUtc.CompareTo(right.LastWriteTimeUtc));
            for (var index = 0; index < files.Length - H1UnityOperationCeilings.MaximumLedgerEntries; index++) files[index].Delete();
        }

        private static bool IsSafeInvocationId(string? value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > 96) return false;
            for (var index = 0; index < value.Length; index++)
            {
                var c = value[index];
                if (!(char.IsLetterOrDigit(c) || c == '-')) return false;
            }
            return true;
        }

        private static Dictionary<string, string> Parse(IEnumerable<string> lines)
        {
            var fields = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var line in lines)
            {
                var separator = line.IndexOf('=');
                if (separator <= 0) throw new FormatException("Invalid ledger field.");
                fields.Add(line.Substring(0, separator), line.Substring(separator + 1));
            }
            return fields;
        }
        private static string Encode(string value) => Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value));
        private static string Decode(string value) => System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value));
    }

    public sealed class FileH1UnityProjectLease : IH1UnityProjectLease
    {
        private readonly string _path;
        public FileH1UnityProjectLease(H1UnityLaunchProfile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            _path = Path.Combine(profile.ProjectRoot, "Library", "Arkus", "H1Lifecycle", "project-operation.lease");
        }
        public IDisposable? TryAcquire()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path) ?? throw new InvalidOperationException("Lease path has no directory."));
            try { return new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None); }
            catch (IOException) { return null; }
        }
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
        private H1AdmittedUnityContract? _admission;

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

        public void Bind(NeutralProjectionService projection)
        {
            if (projection == null) throw new ArgumentNullException(nameof(projection));
            var admission = projection.H1Admission ?? throw new InvalidOperationException("H1 Unity lifecycle requires an admitted H1 projection.");
            if (!string.Equals(admission.Workspace.Identity, _profile.ProjectIdentity, StringComparison.Ordinal) || !string.Equals(admission.Workspace.Root, UnityProjectWorkspaceAuthority.ProjectRoot, StringComparison.Ordinal))
                throw new InvalidOperationException(BootstrapIncompleteCode + ": launch profile disagrees with the admitted project workspace.");
            var admitted = new HashSet<CapabilityKey>();
            foreach (var definition in admission.Contract.Definitions)
                if (string.Equals(definition.Provider.ProviderId, H1UnityHostCapabilityPolicy.HostProviderId, StringComparison.Ordinal)) admitted.Add(definition.Key);
            foreach (var capability in admitted)
                if (!_executors.ContainsKey(capability)) throw new InvalidOperationException(BootstrapIncompleteCode + ": missing worker executor for " + capability + ".");
            foreach (var capability in _executors.Keys)
                if (!admitted.Contains(capability)) throw new InvalidOperationException(BootstrapIncompleteCode + ": orphan worker executor for " + capability + ".");
            _admission = admission;
        }

        public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (_admission == null) return Failure(BootstrapIncompleteCode, "The H1 Unity lifecycle has not been bound to the admitted composition.", false);
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
                if (System.Text.Encoding.UTF8.GetByteCount(payload) > H1UnityOperationCeilings.MaximumPayloadBytes) return Failure("unity.lifecycle.payload-too-large", "The internal worker payload exceeded the fixed H1 operation ceiling.", false);
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
                    return RecordAndFail(envelope, launch.TerminationConfirmed ? H1UnityInvocationStatus.Interrupted : H1UnityInvocationStatus.Indeterminate, launch.TerminationConfirmed ? TimeoutCode : IndeterminateCode, launch.TerminationConfirmed ? "The Unity Editor worker exceeded the fixed operation deadline and was terminated." : "The Unity Editor worker exceeded the deadline and termination could not be proven.", true);
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
            string.Equals(result.InvocationId, invocation.InvocationId, StringComparison.Ordinal) && string.Equals(result.Capability, invocation.Capability, StringComparison.Ordinal) && string.Equals(result.ExecutorId, invocation.ExecutorId, StringComparison.Ordinal) && string.Equals(result.ProfileId, _profile.Id, StringComparison.Ordinal) && string.Equals(result.ProjectIdentity, _profile.ProjectIdentity, StringComparison.Ordinal) && string.Equals(result.EditorVersion, _profile.EffectiveEditorVersion, StringComparison.Ordinal) && string.Equals(result.EditorRevision, _profile.EffectiveEditorRevision, StringComparison.Ordinal);

        private static CapabilityInvocationResult Failure(string code, string message, bool retryable, string? invocationId = null)
        {
            var details = new Dictionary<string, object?>(StringComparer.Ordinal);
            if (invocationId != null) details["invocationId"] = invocationId;
            return CapabilityInvocationResult.Failed(new StructuredError(code, message, "$", new ReadOnlyDictionary<string, object?>(details), retryable, retryable ? "Query unity.lifecycle.operation-status with the invocation identity before retrying." : "Repair the host/worker contract mismatch before retrying."));
        }

        private static string CreateInvocationId() => "h1u-" + DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture) + "-" + Guid.NewGuid().ToString("N");
    }

    public sealed class H1UnityProjectProfileInspectExecutor : IH1UnityCapabilityExecutor
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
            return CapabilityInvocationResult.Succeeded(new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = "arkus.h1-unity-project-profile-inspection@1",
                ["profileId"] = result.ProfileId,
                ["projectIdentity"] = result.ProjectIdentity,
                ["editorVersion"] = result.EditorVersion,
                ["editorRevision"] = result.EditorRevision,
                ["mainThread"] = result.MainThread,
                ["executorId"] = result.ExecutorId
            }));
        }
    }

    [PublicCapabilityRoute(H1UnityHostCapabilityPolicy.HostProviderId, "unity.host.project-profile.inspect", "1.0")]
    public sealed class H1UnityProjectProfileInspectHandler : ICanonicalCapabilityHandler
    {
        private readonly H1UnityEditorExecutionCoordinator _coordinator;
        public H1UnityProjectProfileInspectHandler(H1UnityEditorExecutionCoordinator coordinator) { _coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator)); }
        public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request) => _coordinator.Invoke(context, request);
    }

    [PublicCapabilityRoute("arkus.unity-lifecycle", "unity.lifecycle.operation-status", "1.0")]
    public sealed class H1UnityOperationStatusHandler : ICanonicalCapabilityHandler
    {
        private readonly IH1UnityInvocationLedger _ledger;
        public H1UnityOperationStatusHandler(IH1UnityInvocationLedger ledger) { _ledger = ledger ?? throw new ArgumentNullException(nameof(ledger)); }
        public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (!request.TryGetValue("invocationId", out var raw) || !(raw is string invocationId) || !_ledger.TryRead(invocationId, out var record) || record == null)
                return CapabilityInvocationResult.Failed(new StructuredError("unity.lifecycle.unknown-invocation", "No bounded lifecycle record exists for the supplied invocation identity.", "$.invocationId", new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)), false, "Use the invocation identity returned by a structured H1 lifecycle outcome."));
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
        public static CanonicalProviderContribution CreateEditorHostContribution(H1UnityEditorExecutionCoordinator coordinator)
        {
            if (coordinator == null) throw new ArgumentNullException(nameof(coordinator));
            var definition = new CapabilityDefinition(H1UnityProjectProfileInspectExecutor.Key, new ProviderMetadata(H1UnityHostCapabilityPolicy.HostProviderId, ProviderKind.Scoped, H1UnityHostCapabilityPolicy.HostScope, H1UnityHostCapabilityPolicy.HostNamespace), CanonicalContractSchemas.EmptyObject(), InspectionSuccessSchema(), CanonicalContractSchemas.StructuredError(), SideEffectClass.ReadOnly, DeterminismClass.EnvironmentDependent, new[] { "h1-unity-profile-bound", "project-operation-lease-available" }, new[] { "unity-main-thread-observation-returned" }, new ConcurrencySemantics(ConcurrencyClass.Serialized), new IdempotencySemantics(IdempotencyClass.Idempotent), new BatchingSemantics(BatchingClass.Unsupported), new RepairSemantics(true, true), new PolicySemantics(PrivilegeClass.Authoring, TransactionRequirement.None, ProvenanceRequirement.Required), new CostSemantics(1, "one short-lived Unity Editor inspection worker"));
            return new CanonicalProviderContribution(new ProviderDescriptor(H1UnityHostCapabilityPolicy.HostProviderId, ProviderKind.Scoped, H1UnityHostCapabilityPolicy.HostScope, new[] { H1UnityHostCapabilityPolicy.HostNamespace }), new[] { definition }, new[] { CapabilityRoute.FromHandler(new H1UnityProjectProfileInspectHandler(coordinator)) });
        }

        public static UnityHostCapabilityGrant CreateInspectionGrant() => new UnityHostCapabilityGrant(H1UnityProjectProfileInspectExecutor.Key, UnityProjectWorkspaceAuthority.ProjectSettingsRootId, "ref.arkus.unity-host.project-metadata", UnityHostResourceClass.ProjectMetadata, UnityHostTimeClass.BoundedRead);

        public static CanonicalProviderContribution CreateLifecycleStatusContribution(IH1UnityInvocationLedger ledger)
        {
            if (ledger == null) throw new ArgumentNullException(nameof(ledger));
            var key = new CapabilityKey("unity.lifecycle.operation-status", new ContractVersion(1, 0));
            var definition = new CapabilityDefinition(key, new ProviderMetadata("arkus.unity-lifecycle", ProviderKind.Scoped, "unity-lifecycle", "unity.lifecycle"), new JsonSchemaDocument(SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal) { ["invocationId"] = SchemaNode.String() }, new[] { "invocationId" })), OperationStatusSuccessSchema(), CanonicalContractSchemas.StructuredError(), SideEffectClass.ReadOnly, DeterminismClass.EnvironmentDependent, Array.Empty<string>(), new[] { "bounded-ledger-status-returned" }, new ConcurrencySemantics(ConcurrencyClass.ParallelSafe), new IdempotencySemantics(IdempotencyClass.Idempotent), new BatchingSemantics(BatchingClass.Unsupported), new RepairSemantics(false, true), new PolicySemantics(PrivilegeClass.PublicRead, TransactionRequirement.ReadOnlyEnvelope, ProvenanceRequirement.Required), new CostSemantics(1, "bounded project-local lifecycle ledger lookup"));
            return new CanonicalProviderContribution(new ProviderDescriptor("arkus.unity-lifecycle", ProviderKind.Scoped, "unity-lifecycle", new[] { "unity.lifecycle" }), new[] { definition }, new[] { CapabilityRoute.FromHandler(new H1UnityOperationStatusHandler(ledger)) });
        }

        private static JsonSchemaDocument InspectionSuccessSchema() => new JsonSchemaDocument(SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
        {
            ["schemaId"] = SchemaNode.String(new[] { "arkus.h1-unity-project-profile-inspection@1" }),
            ["profileId"] = SchemaNode.String(new[] { H1UnityLaunchProfile.ProfileId }),
            ["projectIdentity"] = SchemaNode.String(new[] { UnityProjectWorkspaceAuthority.ProjectIdentity }),
            ["editorVersion"] = SchemaNode.String(new[] { H1UnityLaunchProfile.EditorVersion }),
            ["editorRevision"] = SchemaNode.String(new[] { H1UnityLaunchProfile.EditorRevision }),
            ["mainThread"] = SchemaNode.Boolean(),
            ["executorId"] = SchemaNode.String(new[] { H1UnityProjectProfileInspectExecutor.WorkerExecutorId })
        }, new[] { "schemaId", "profileId", "projectIdentity", "editorVersion", "editorRevision", "mainThread", "executorId" }));

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
