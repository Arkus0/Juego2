using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;

namespace Arkus.Harness.Projection
{
    /// <summary>
    /// Transport-neutral request envelope. Framing adapters map their own correlation and deadline
    /// representation into this type; canonical arguments remain the exact portable data consumed by
    /// the accepted composed dispatcher.
    /// </summary>
    public sealed class NeutralProjectionRequest
    {
        public const string ProjectionVersion = "arkus.neutral-projection@1";

        public NeutralProjectionRequest(
            string requestId,
            string capabilityName,
            ContractVersionRange acceptedVersions,
            IReadOnlyDictionary<string, object?> arguments,
            int? timeoutMilliseconds = null)
        {
            if (string.IsNullOrWhiteSpace(requestId))
                throw new ArgumentException("A non-empty request correlation ID is required.", nameof(requestId));
            if (string.IsNullOrWhiteSpace(capabilityName))
                throw new ArgumentException("A non-empty canonical capability name is required.", nameof(capabilityName));
            if (timeoutMilliseconds.HasValue && timeoutMilliseconds.Value < 0)
                throw new ArgumentOutOfRangeException(nameof(timeoutMilliseconds));

            RequestId = requestId;
            CapabilityName = capabilityName;
            AcceptedVersions = acceptedVersions ?? throw new ArgumentNullException(nameof(acceptedVersions));
            Arguments = arguments == null
                ? throw new ArgumentNullException(nameof(arguments))
                : new ReadOnlyDictionary<string, object?>(
                    new Dictionary<string, object?>(arguments, StringComparer.Ordinal));
            TimeoutMilliseconds = timeoutMilliseconds;
        }

        public string RequestId { get; }
        public string CapabilityName { get; }
        public ContractVersionRange AcceptedVersions { get; }
        public IReadOnlyDictionary<string, object?> Arguments { get; }
        public int? TimeoutMilliseconds { get; }

        public IReadOnlyDictionary<string, object?> ToData()
        {
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["projectionVersion"] = ProjectionVersion,
                ["requestId"] = RequestId,
                ["capability"] = CapabilityName,
                ["acceptedVersions"] = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["major"] = AcceptedVersions.Major,
                    ["minimumMinor"] = AcceptedVersions.MinimumMinor,
                    ["maximumMinor"] = AcceptedVersions.MaximumMinor
                }),
                ["arguments"] = Arguments,
                ["timeoutMilliseconds"] = TimeoutMilliseconds
            });
        }

        internal static IReadOnlyDictionary<string, object?> ReadOnly(Dictionary<string, object?> source) =>
            new ReadOnlyDictionary<string, object?>(source);
    }

    public enum NeutralProjectionFailureKind
    {
        None = 0,
        Canonical = 1,
        Cancelled = 2,
        TimedOut = 3
    }

    /// <summary>
    /// One transport-neutral outcome. Canonical failures preserve the accepted canonical error
    /// byte-for-byte as portable data; admission cancellation/timeout use stable projection errors.
    /// </summary>
    public sealed class NeutralProjectionOutcome
    {
        private NeutralProjectionOutcome(
            string requestId,
            string capabilityName,
            IReadOnlyDictionary<string, object?>? result,
            StructuredError? error,
            NeutralProjectionFailureKind failureKind)
        {
            RequestId = requestId ?? throw new ArgumentNullException(nameof(requestId));
            CapabilityName = capabilityName ?? throw new ArgumentNullException(nameof(capabilityName));
            Result = result;
            Error = error;
            FailureKind = failureKind;
        }

        public string RequestId { get; }
        public string CapabilityName { get; }
        public bool Success => Result != null && Error == null && FailureKind == NeutralProjectionFailureKind.None;
        public IReadOnlyDictionary<string, object?>? Result { get; }
        public StructuredError? Error { get; }
        public NeutralProjectionFailureKind FailureKind { get; }

        public static NeutralProjectionOutcome FromCanonical(
            NeutralProjectionRequest request,
            CapabilityInvocationResult result)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (result == null) throw new ArgumentNullException(nameof(result));

            return result.Success
                ? new NeutralProjectionOutcome(
                    request.RequestId,
                    request.CapabilityName,
                    result.Data ?? throw new InvalidOperationException("Canonical success had no result."),
                    null,
                    NeutralProjectionFailureKind.None)
                : new NeutralProjectionOutcome(
                    request.RequestId,
                    request.CapabilityName,
                    null,
                    result.Error ?? throw new InvalidOperationException("Canonical failure had no structured error."),
                    NeutralProjectionFailureKind.Canonical);
        }

        internal static NeutralProjectionOutcome Cancelled(NeutralProjectionRequest request)
        {
            return Failure(
                request,
                NeutralProjectionFailureKind.Cancelled,
                "projection.cancelled",
                "The request was cancelled before canonical dispatch began.",
                true,
                "Retry with a live cancellation scope; canonical state was not invoked by this request.");
        }

        internal static NeutralProjectionOutcome TimedOut(NeutralProjectionRequest request)
        {
            return Failure(
                request,
                NeutralProjectionFailureKind.TimedOut,
                "projection.timeout",
                "The request deadline expired before canonical dispatch began.",
                true,
                "Retry with a sufficient admission timeout; canonical state was not invoked by this request.");
        }

        public IReadOnlyDictionary<string, object?> ToData()
        {
            var data = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["projectionVersion"] = NeutralProjectionRequest.ProjectionVersion,
                ["requestId"] = RequestId,
                ["capability"] = CapabilityName,
                ["status"] = Success ? "success" : "error"
            };

            if (Success)
            {
                data["result"] = Result;
            }
            else
            {
                data["failureKind"] = FailureToken(FailureKind);
                data["error"] = Error?.ToData();
            }

            return NeutralProjectionRequest.ReadOnly(data);
        }

        private static NeutralProjectionOutcome Failure(
            NeutralProjectionRequest request,
            NeutralProjectionFailureKind kind,
            string code,
            string message,
            bool retryable,
            string repairHint)
        {
            return new NeutralProjectionOutcome(
                request.RequestId,
                request.CapabilityName,
                null,
                new StructuredError(
                    code,
                    message,
                    "$",
                    new ReadOnlyDictionary<string, object?>(
                        new Dictionary<string, object?>(StringComparer.Ordinal)),
                    retryable,
                    repairHint),
                kind);
        }

        private static string FailureToken(NeutralProjectionFailureKind kind)
        {
            switch (kind)
            {
                case NeutralProjectionFailureKind.Canonical: return "canonical";
                case NeutralProjectionFailureKind.Cancelled: return "cancelled";
                case NeutralProjectionFailureKind.TimedOut: return "timed-out";
                default: throw new InvalidOperationException("Success has no failure token.");
            }
        }
    }

    /// <summary>
    /// Generic projection of one composed canonical runtime. The composed inventory remains the only
    /// capability/schema authority. The gate bounds cancellation and timeout to admission; once the
    /// synchronous canonical dispatcher starts, its truthful success/error outcome always wins.
    /// </summary>
    public sealed class NeutralProjectionService : IDisposable
    {
        private readonly ComposedContract _contract;
        private readonly SemaphoreSlim _dispatchGate = new SemaphoreSlim(1, 1);
        private bool _disposed;

        public NeutralProjectionService(ComposedContract contract)
        {
            _contract = contract ?? throw new ArgumentNullException(nameof(contract));
        }

        public IReadOnlyList<CapabilityDefinition> Capabilities => _contract.Definitions;

        public async Task<NeutralProjectionOutcome> InvokeAsync(
            NeutralProjectionRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            ThrowIfDisposed();

            if (cancellationToken.IsCancellationRequested)
                return NeutralProjectionOutcome.Cancelled(request);
            if (request.TimeoutMilliseconds == 0)
                return NeutralProjectionOutcome.TimedOut(request);

            bool entered;
            try
            {
                entered = request.TimeoutMilliseconds.HasValue
                    ? await _dispatchGate.WaitAsync(request.TimeoutMilliseconds.Value, cancellationToken).ConfigureAwait(false)
                    : await WaitWithoutTimeout(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return NeutralProjectionOutcome.Cancelled(request);
            }

            if (!entered)
                return NeutralProjectionOutcome.TimedOut(request);

            try
            {
                if (cancellationToken.IsCancellationRequested)
                    return NeutralProjectionOutcome.Cancelled(request);

                var result = _contract.Dispatch(
                    request.CapabilityName,
                    request.AcceptedVersions,
                    request.Arguments);
                return NeutralProjectionOutcome.FromCanonical(request, result);
            }
            finally
            {
                _dispatchGate.Release();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _dispatchGate.Dispose();
            _disposed = true;
        }

        private async Task<bool> WaitWithoutTimeout(CancellationToken cancellationToken)
        {
            await _dispatchGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            return true;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(NeutralProjectionService));
        }
    }

    public sealed class ProjectionCompletenessIssue
    {
        public ProjectionCompletenessIssue(string code, string identity, string message)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public string Code { get; }
        public string Identity { get; }
        public string Message { get; }
    }

    public static class NeutralProjectionCompleteness
    {
        public static IReadOnlyList<ProjectionCompletenessIssue> Compare(
            IEnumerable<CapabilityDefinition> canonicalInventory,
            IEnumerable<CapabilityKey> projectedCapabilities)
        {
            if (canonicalInventory == null) throw new ArgumentNullException(nameof(canonicalInventory));
            if (projectedCapabilities == null) throw new ArgumentNullException(nameof(projectedCapabilities));

            var canonical = new HashSet<CapabilityKey>();
            foreach (var definition in canonicalInventory)
                canonical.Add((definition ?? throw new ArgumentException("Canonical inventory contains null.", nameof(canonicalInventory))).Key);

            var projected = new HashSet<CapabilityKey>();
            var issues = new List<ProjectionCompletenessIssue>();
            foreach (var key in projectedCapabilities)
            {
                if (key == null)
                    throw new ArgumentException("Projected capability inventory contains null.", nameof(projectedCapabilities));
                if (!projected.Add(key))
                    issues.Add(new ProjectionCompletenessIssue(
                        "projection.duplicate_capability",
                        key.ToString(),
                        "Projection emitted the same canonical capability more than once."));
            }

            foreach (var key in canonical)
            {
                if (!projected.Contains(key))
                    issues.Add(new ProjectionCompletenessIssue(
                        "projection.omitted_capability",
                        key.ToString(),
                        "Projection omitted a capability from the composed canonical inventory."));
            }

            foreach (var key in projected)
            {
                if (!canonical.Contains(key))
                    issues.Add(new ProjectionCompletenessIssue(
                        "projection.extra_capability",
                        key.ToString(),
                        "Projection invented a capability outside the composed canonical inventory."));
            }

            issues.Sort((left, right) =>
            {
                var identity = StringComparer.Ordinal.Compare(left.Identity, right.Identity);
                return identity != 0 ? identity : StringComparer.Ordinal.Compare(left.Code, right.Code);
            });
            return issues.AsReadOnly();
        }

        public static IReadOnlyList<CapabilityKey> KeysFrom(NeutralProjectionService projection)
        {
            if (projection == null) throw new ArgumentNullException(nameof(projection));
            var keys = new List<CapabilityKey>();
            foreach (var definition in projection.Capabilities) keys.Add(definition.Key);
            return keys.AsReadOnly();
        }
    }

    /// <summary>The single production local host composition used by the executable adapter.</summary>
    public static class ProductionHarnessHost
    {
        public const string InitialWorldId = "world.arkus.session";

        public static NeutralProjectionService Create()
        {
            var contract = CanonicalWorldContract.ComposeEmptyPortableSession(InitialWorldId);
            return new NeutralProjectionService(contract);
        }
    }
}
