using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Arkus.EngineBridge.UnityAuthoring;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;

namespace Arkus.H1.UnityHost
{
    public sealed class FileH1UnityInvocationLedger : IH1UnityInvocationLedger
    {
        private const string LedgerSchema = "arkus.h1-unity-ledger@1";
        private readonly string _directory;

        public FileH1UnityInvocationLedger(H1UnityLaunchProfile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            _directory = Path.Combine(profile.ProjectRoot, "Library", "Arkus", "H1Lifecycle", "ledger");
        }

        public void Record(H1UnityInvocationRecord record)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));
            if (!H1UnityEnvelopeCodec.IsSafeInvocationId(record.InvocationId)) throw new ArgumentException("Invocation identity is not a bounded host-generated identifier.", nameof(record));
            Directory.CreateDirectory(_directory);
            var path = Path.Combine(_directory, record.InvocationId + ".ledger");
            var lines = new[]
            {
                "schema=" + LedgerSchema,
                "invocation=" + H1UnityEnvelopeCodec.Encode(record.InvocationId),
                "capability=" + H1UnityEnvelopeCodec.Encode(record.Capability),
                "status=" + record.Status.ToString(),
                "outcome=" + H1UnityEnvelopeCodec.Encode(record.OutcomeCode)
            };
            AtomicWrite(path, lines);
            BoundLedger();
        }

        public bool TryRead(string invocationId, out H1UnityInvocationRecord? record)
        {
            record = null;
            if (!H1UnityEnvelopeCodec.IsSafeInvocationId(invocationId)) return false;
            var path = Path.Combine(_directory, invocationId + ".ledger");
            if (!File.Exists(path)) return false;
            try
            {
                var fields = H1UnityEnvelopeCodec.ParseLines(File.ReadAllLines(path));
                if (!fields.TryGetValue("schema", out var schema) || !string.Equals(schema, LedgerSchema, StringComparison.Ordinal)) return false;
                if (!fields.TryGetValue("invocation", out var encodedInvocation) || !string.Equals(H1UnityEnvelopeCodec.Decode(encodedInvocation), invocationId, StringComparison.Ordinal)) return false;
                if (!fields.TryGetValue("capability", out var encodedCapability)) return false;
                if (!fields.TryGetValue("status", out var statusText) || !Enum.TryParse(statusText, false, out H1UnityInvocationStatus status)) return false;
                if (!fields.TryGetValue("outcome", out var encodedOutcome)) return false;
                record = new H1UnityInvocationRecord(invocationId, H1UnityEnvelopeCodec.Decode(encodedCapability), status, H1UnityEnvelopeCodec.Decode(encodedOutcome));
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

        internal static void AtomicWrite(string path, IEnumerable<string> lines)
        {
            var directory = Path.GetDirectoryName(path) ?? throw new InvalidOperationException("Atomic write path has no parent directory.");
            Directory.CreateDirectory(directory);
            var temp = path + ".tmp-" + Guid.NewGuid().ToString("N");
            File.WriteAllLines(temp, lines, new UTF8Encoding(false));
            File.Move(temp, path, true);
        }
    }

    public sealed class FileH1UnityProjectLease : IH1UnityProjectLease
    {
        private const uint HandleFlagInherit = 0x00000001;
        private const int FGetFd = 1;
        private const int FSetFd = 2;
        private const int FdCloseOnExec = 1;
        private readonly string _path;

        public FileH1UnityProjectLease(H1UnityLaunchProfile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            _path = Path.Combine(profile.ProjectRoot, "Library", "Arkus", "H1Lifecycle", "project-operation.lease");
        }

        public IDisposable? TryAcquire()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path) ?? throw new InvalidOperationException("Lease path has no directory."));

            FileStream stream;
            try
            {
                // Keep the original FileShare.None contract intact: on Unix this is what makes the
                // runtime request an exclusive advisory lock, and on Windows it denies competing opens.
                stream = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return null;
            }

            try
            {
                // The already-exclusive OS handle, not a PID or timeout, is the ownership token.
                // Make that same handle survive Process.Start so the Unity child keeps exclusion if
                // its parent host dies. The final child close then releases the operation naturally.
                MakeHandleInheritable(stream);
                stream.SetLength(0);
                using (var writer = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
                {
                    writer.Write("arkus.h1-unity-project-lease@1\n");
                    writer.Write(Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture));
                    writer.Flush();
                }
                stream.Flush(true);
                return stream;
            }
            catch
            {
                stream.Dispose();
                throw;
            }
        }

        private static void MakeHandleInheritable(FileStream stream)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (!SetHandleInformation(stream.SafeFileHandle.DangerousGetHandle(), HandleFlagInherit, HandleFlagInherit))
                {
                    throw NativeIOException("Unable to make the H1 Unity project lease handle inheritable.");
                }
                return;
            }

            var rawHandle = stream.SafeFileHandle.DangerousGetHandle().ToInt64();
            if (rawHandle < 0 || rawHandle > int.MaxValue)
            {
                throw new IOException("The H1 Unity project lease file descriptor is outside the supported native range.");
            }

            var descriptor = (int)rawHandle;
            var flags = Fcntl(descriptor, FGetFd, 0);
            if (flags < 0)
            {
                throw NativeIOException("Unable to inspect the H1 Unity project lease file descriptor flags.");
            }
            if ((flags & FdCloseOnExec) == 0) return;
            if (Fcntl(descriptor, FSetFd, flags & ~FdCloseOnExec) < 0)
            {
                throw NativeIOException("Unable to make the H1 Unity project lease file descriptor inheritable.");
            }
        }

        private static IOException NativeIOException(string message) =>
            new IOException(message, new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()));

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetHandleInformation(IntPtr handle, uint mask, uint flags);

        [DllImport("libc", EntryPoint = "fcntl", SetLastError = true)]
        private static extern int Fcntl(int descriptor, int command, int argument);
    }

    public static class H1UnityEnvelopeCodec
    {
        private const long EnvelopeOverheadBytes = 64 * 1024;

        public static void WriteInvocation(string path, H1UnityInvocationEnvelope invocation)
        {
            if (invocation == null) throw new ArgumentNullException(nameof(invocation));
            FileH1UnityInvocationLedger.AtomicWrite(path, new[]
            {
                "schema=" + H1UnityLaunchProfile.InvocationSchemaId,
                "invocation=" + Encode(invocation.InvocationId),
                "capability=" + Encode(invocation.Capability),
                "executor=" + Encode(invocation.ExecutorId),
                "payload=" + Encode(invocation.Payload),
                "profile=" + Encode(invocation.ProfileId),
                "project=" + Encode(invocation.ProjectIdentity),
                "editorVersion=" + Encode(invocation.EditorVersion),
                "editorRevision=" + Encode(invocation.EditorRevision),
                "entryPoint=" + Encode(invocation.EntryPoint)
            });
        }

        public static bool TryReadResult(string path, out H1UnityResultEnvelope? result)
        {
            result = null;
            try
            {
                var file = new FileInfo(path);
                if (!file.Exists || file.Length <= 0 || file.Length > H1UnityOperationCeilings.MaximumPayloadBytes + EnvelopeOverheadBytes) return false;
                var fields = ParseLines(File.ReadAllLines(path));
                if (!fields.TryGetValue("schema", out var schema) || !string.Equals(schema, H1UnityLaunchProfile.ResultSchemaId, StringComparison.Ordinal)) return false;
                if (!TryDecode(fields, "invocation", out var invocationId) || !IsSafeInvocationId(invocationId)) return false;
                if (!TryDecode(fields, "capability", out var capability)) return false;
                if (!TryDecode(fields, "executor", out var executor)) return false;
                if (!TryDecode(fields, "profile", out var profile)) return false;
                if (!TryDecode(fields, "project", out var project)) return false;
                if (!TryDecode(fields, "editorVersion", out var editorVersion)) return false;
                if (!TryDecode(fields, "editorRevision", out var editorRevision)) return false;
                if (!fields.TryGetValue("mainThread", out var mainThreadText) || !bool.TryParse(mainThreadText, out var mainThread)) return false;
                if (!TryDecode(fields, "payload", out var payload)) return false;
                if (Encoding.UTF8.GetByteCount(payload) > H1UnityOperationCeilings.MaximumPayloadBytes) return false;
                result = new H1UnityResultEnvelope(invocationId, capability, executor, profile, project, editorVersion, editorRevision, mainThread, payload);
                return true;
            }
            catch (IOException) { return false; }
            catch (FormatException) { return false; }
            catch (UnauthorizedAccessException) { return false; }
        }

        internal static Dictionary<string, string> ParseLines(IEnumerable<string> lines)
        {
            var fields = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var line in lines)
            {
                var separator = line.IndexOf('=');
                if (separator <= 0) throw new FormatException("Envelope field is malformed.");
                if (!fields.TryAdd(line.Substring(0, separator), line.Substring(separator + 1))) throw new FormatException("Envelope contains a duplicate field.");
            }
            return fields;
        }

        internal static string Encode(string value) => Convert.ToBase64String(Encoding.UTF8.GetBytes(value ?? throw new ArgumentNullException(nameof(value))));
        internal static string Decode(string value) => Encoding.UTF8.GetString(Convert.FromBase64String(value ?? throw new ArgumentNullException(nameof(value))));

        internal static bool IsSafeInvocationId(string? value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > 96) return false;
            foreach (var c in value)
            {
                if (!(char.IsLetterOrDigit(c) || c == '-')) return false;
            }
            return true;
        }

        private static bool TryDecode(IReadOnlyDictionary<string, string> fields, string key, out string value)
        {
            value = string.Empty;
            if (!fields.TryGetValue(key, out var encoded)) return false;
            value = Decode(encoded);
            return true;
        }
    }

    public sealed class FixedH1UnityEditorProcessLauncher : IH1UnityEditorWorkerLauncher
    {
        private const int DiagnosticCaptureChars = 64 * 1024;

        public H1UnityWorkerLaunchResult Launch(H1UnityInvocationEnvelope invocation, H1UnityLaunchProfile profile, InvocationResourceBudget executionBudget)
        {
            if (invocation == null) throw new ArgumentNullException(nameof(invocation));
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            if (executionBudget == null) throw new ArgumentNullException(nameof(executionBudget));
            if (!H1UnityEnvelopeCodec.IsSafeInvocationId(invocation.InvocationId)) return H1UnityWorkerLaunchResult.Failure(H1UnityWorkerLaunchKind.CorruptResult);

            var operationDirectory = Path.Combine(profile.ProjectRoot, "Library", "Arkus", "H1Lifecycle", "operations", invocation.InvocationId);
            Directory.CreateDirectory(operationDirectory);
            var invocationPath = Path.Combine(operationDirectory, "invocation.env");
            var resultPath = Path.Combine(operationDirectory, "result.env");
            var unityLogPath = Path.Combine(operationDirectory, "unity.log");
            var diagnosticsPath = Path.Combine(operationDirectory, "process-diagnostics.log");
            H1UnityEnvelopeCodec.WriteInvocation(invocationPath, invocation);
            if (File.Exists(resultPath)) File.Delete(resultPath);
            using var process = new Process();
            process.StartInfo = CreateStartInfo(profile, invocationPath, resultPath, unityLogPath);
            var stdout = new BoundedTextCapture(DiagnosticCaptureChars);
            var stderr = new BoundedTextCapture(DiagnosticCaptureChars);
            process.OutputDataReceived += (_, args) => stdout.Append(args.Data);
            process.ErrorDataReceived += (_, args) => stderr.Append(args.Data);

            try
            {
                if (!process.Start()) return H1UnityWorkerLaunchResult.Failure(H1UnityWorkerLaunchKind.StartFailure, null, true);
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
            catch (Exception exception) when (exception is InvalidOperationException || exception is System.ComponentModel.Win32Exception || exception is IOException)
            {
                WriteDiagnostics(diagnosticsPath, stdout, stderr);
                return H1UnityWorkerLaunchResult.Failure(H1UnityWorkerLaunchKind.StartFailure, null, true);
            }

            while (!process.WaitForExit(100))
            {
                if (executionBudget.TryContinue("h1-unity-editor-worker", out var budgetError)) continue;
                var terminationConfirmed = Terminate(process);
                WriteDiagnostics(diagnosticsPath, stdout, stderr);
                return H1UnityWorkerLaunchResult.Failure(
                    string.Equals(budgetError?.MachineCode, "resource.execution_cancelled", StringComparison.Ordinal)
                        ? H1UnityWorkerLaunchKind.Cancelled
                        : H1UnityWorkerLaunchKind.TimedOut,
                    process.HasExited ? process.ExitCode : null,
                    terminationConfirmed);
            }

            process.WaitForExit();
            WriteDiagnostics(diagnosticsPath, stdout, stderr);
            if (process.ExitCode != 0) return H1UnityWorkerLaunchResult.Failure(H1UnityWorkerLaunchKind.NonZeroExit, process.ExitCode, true);
            if (!File.Exists(resultPath)) return H1UnityWorkerLaunchResult.Failure(H1UnityWorkerLaunchKind.MissingResult, 0, true);
            if (!H1UnityEnvelopeCodec.TryReadResult(resultPath, out var result) || result == null) return H1UnityWorkerLaunchResult.Failure(H1UnityWorkerLaunchKind.CorruptResult, 0, true);
            return H1UnityWorkerLaunchResult.Completed(result);
        }

        private static ProcessStartInfo CreateStartInfo(H1UnityLaunchProfile profile, string invocationPath, string resultPath, string unityLogPath)
        {
            var info = new ProcessStartInfo
            {
                FileName = profile.ExecutablePath,
                WorkingDirectory = profile.RepositoryRoot,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            info.ArgumentList.Add("-batchmode");
            info.ArgumentList.Add("-nographics");
            info.ArgumentList.Add("-quit");
            info.ArgumentList.Add("-projectPath");
            info.ArgumentList.Add(profile.ProjectRoot);
            info.ArgumentList.Add("-executeMethod");
            info.ArgumentList.Add(H1UnityLaunchProfile.FixedEntryPoint);
            info.ArgumentList.Add("-arkusInvocation");
            info.ArgumentList.Add(invocationPath);
            info.ArgumentList.Add("-arkusResult");
            info.ArgumentList.Add(resultPath);
            info.ArgumentList.Add("-logFile");
            info.ArgumentList.Add(unityLogPath);
            return info;
        }

        private static bool Terminate(Process process)
        {
            try
            {
                if (process.HasExited) return true;
                process.Kill(true);
                return process.WaitForExit(5000) && process.HasExited;
            }
            catch
            {
                return false;
            }
        }

        private static void WriteDiagnostics(string path, BoundedTextCapture stdout, BoundedTextCapture stderr)
        {
            try
            {
                FileH1UnityInvocationLedger.AtomicWrite(path, new[]
                {
                    "schema=arkus.h1-unity-process-diagnostics@1",
                    "stdout-b64=" + H1UnityEnvelopeCodec.Encode(stdout.ToString()),
                    "stderr-b64=" + H1UnityEnvelopeCodec.Encode(stderr.ToString())
                });
            }
            catch (IOException)
            {
                // Diagnostics are deliberately non-authoritative. Lifecycle truth comes from exit/result identity.
            }
        }

        private sealed class BoundedTextCapture
        {
            private readonly int _maximumCharacters;
            private readonly StringBuilder _builder = new StringBuilder();
            private readonly object _gate = new object();
            public BoundedTextCapture(int maximumCharacters) { _maximumCharacters = maximumCharacters; }
            public void Append(string? line)
            {
                if (line == null) return;
                lock (_gate)
                {
                    if (_builder.Length >= _maximumCharacters) return;
                    var remaining = _maximumCharacters - _builder.Length;
                    var take = Math.Min(remaining, line.Length);
                    _builder.Append(line, 0, take);
                    if (_builder.Length < _maximumCharacters) _builder.Append('\n');
                }
            }
            public override string ToString() { lock (_gate) return _builder.ToString(); }
        }
    }

    public static class ProductionH1UnityHost
    {
        public static NeutralProjectionService Create()
        {
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var world = new PortableWorldAuthoringSession(new WorldState(
                new WorldId(ProductionHarnessHost.InitialWorldId), 0, Array.Empty<WorldObject>()),
                UnityAuthoringProvider.CreateDocumentCodecs());
            var worldReader = new ReadOnlyWorldStateView(world);
            var projectLease = new FileH1UnityProjectLease(profile);
            var ledger = new RestartRecoveringH1UnityInvocationLedger(
                new FileH1UnityInvocationLedger(profile),
                projectLease);
            var coordinator = new H1UnityEditorExecutionCoordinator(
                profile,
                new FixedH1UnityEditorProcessLauncher(),
                projectLease,
                ledger,
                new IH1UnityCapabilityExecutor[]
                {
                    new ProjectProfileInspectExecutor(),
                    new HierarchyProbeExecutor(),
                    new H1CatalogueExecutor(H1CatalogueExecutor.QueryKey, H1CatalogueExecutor.QueryExecutorId, Path.Combine(profile.RepositoryRoot, H1CatalogueSnapshot.MappingRelativePath), Path.Combine(profile.RepositoryRoot, H1CatalogueSnapshot.AdoptionRelativePath)),
                    new H1CatalogueExecutor(H1CatalogueExecutor.GetKey, H1CatalogueExecutor.GetExecutorId, Path.Combine(profile.RepositoryRoot, H1CatalogueSnapshot.MappingRelativePath), Path.Combine(profile.RepositoryRoot, H1CatalogueSnapshot.AdoptionRelativePath)),
                    new H1CatalogueExecutor(H1CatalogueExecutor.ResolveKey, H1CatalogueExecutor.ResolveExecutorId, Path.Combine(profile.RepositoryRoot, H1CatalogueSnapshot.MappingRelativePath), Path.Combine(profile.RepositoryRoot, H1CatalogueSnapshot.AdoptionRelativePath)),
                    new H1ManagedSceneExecutor(H1ManagedSceneExecutor.MaterializeKey, H1ManagedSceneExecutor.MaterializeExecutorId, worldReader, profile),
                    new H1ManagedSceneExecutor(H1ManagedSceneExecutor.ObserveKey, H1ManagedSceneExecutor.ObserveExecutorId, worldReader, profile),
                    new H1ProjectionReconciliationExecutor(H1ProjectionReconciliationExecutor.DriftKey, worldReader, profile),
                    new H1ProjectionRematerializeExecutor(worldReader, profile),
                    new H1ProjectionReconciliationExecutor(H1ProjectionReconciliationExecutor.ImportProposalKey, worldReader, profile)
                });

            var composition = ContractComposer.Compose(
                CanonicalWorldContract.CreateContribution(new WorldInspectionService(world), world),
                new[]
                {
                    UnityAuthoringProvider.CreateContribution(),
                    H1ProjectionContract.PlanContribution(worldReader, profile),
                    H1ProjectionReconciliationComposition.CreateEditorHostContribution(coordinator),
                    H1UnityLifecycleContract.CreateLifecycleStatusContribution(ledger)
                });
            if (!composition.Success || composition.Contract == null)
            {
                var first = composition.Issues.Count == 0 ? "unknown" : composition.Issues[0].Code + " at " + composition.Issues[0].Path;
                throw new InvalidOperationException("H1 Unity production contract failed composition: " + first + ".");
            }

            var projection = H1UnityHostCapabilityPolicy.CreateProjection(
                composition.Contract,
                UnityProjectWorkspaceAuthority.ForArkusUnityProject(),
                H1ProjectionReconciliationComposition.CreateGrants());
            coordinator.Bind(projection);
            return projection;
        }
    }
}
