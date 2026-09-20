using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading;

[assembly: InternalsVisibleTo("Arkus.Game.Authoring")]
[assembly: InternalsVisibleTo("Arkus.Harness.Runtime")]
[assembly: InternalsVisibleTo("Arkus.Harness.Projection")]
[assembly: InternalsVisibleTo("Arkus.Harness.Tests")]

namespace Arkus.Harness.Protocol
{
    /// <summary>
    /// The measured H0 resource envelope. These are product limits shared by the canonical
    /// authoring/runtime boundary and every conforming transport; they are not adapter defaults.
    /// </summary>
    public static class H0ResourceEnvelope
    {
        public const string SchemaId = "arkus.h0-resource-envelope@1";
        public const int MaximumTransportFrameBytes = 2 * 1024 * 1024;
        public const int MaximumCanonicalRequestBytes = 1792 * 1024;
        public const int MaximumPortableDepth = 32;
        public const int MaximumBatchOperations = 96;
        public const int MaximumBatchPayloadBytes = 512 * 1024;
        public const int MaximumPageSize = 100;
        public const int MaximumRelationsPerResource = 256;
        public const int MaximumExtensionPayloadBytes = 256 * 1024;
        public const int MaximumCanonicalWorldBytes = 1024 * 1024;
        public const int MaximumWorldResources = 10000;
        public const int MaximumSessionTransactions = 10000;
        public const int MaximumSnapshotImportReceipts = 1024;
        public const int MaximumExecutionMilliseconds = 5000;

        public static IReadOnlyDictionary<string, object?> ToData()
        {
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = SchemaId,
                ["request"] = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["maximumTransportFrameBytes"] = MaximumTransportFrameBytes,
                    ["maximumCanonicalArgumentBytes"] = MaximumCanonicalRequestBytes,
                    ["maximumPortableDepth"] = MaximumPortableDepth,
                    ["maximumExecutionMilliseconds"] = MaximumExecutionMilliseconds
                }),
                ["mutation"] = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["maximumOperations"] = MaximumBatchOperations,
                    ["maximumDecodedPayloadBytes"] = MaximumBatchPayloadBytes,
                    ["maximumRelationsPerResource"] = MaximumRelationsPerResource,
                    ["maximumSessionTransactions"] = MaximumSessionTransactions
                }),
                ["query"] = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["maximumPageSize"] = MaximumPageSize
                }),
                ["world"] = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["maximumCanonicalBytes"] = MaximumCanonicalWorldBytes,
                    ["maximumResources"] = MaximumWorldResources,
                    ["maximumExtensionPayloadBytes"] = MaximumExtensionPayloadBytes
                }),
                ["snapshot"] = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["maximumDecodedStateBytes"] = MaximumCanonicalWorldBytes,
                    ["maximumImportReceipts"] = MaximumSnapshotImportReceipts
                }),
                ["persistence"] = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["durabilityLevel"] = "process-local-checkpoint",
                    ["publicationBoundary"] = "validate-stage-aggregate-publish",
                    ["powerLossDurabilityClaimed"] = false
                })
            });
        }

        private static IReadOnlyDictionary<string, object?> ReadOnly(
            IDictionary<string, object?> source)
        {
            return new ReadOnlyDictionary<string, object?>(
                new Dictionary<string, object?>(source, StringComparer.Ordinal));
        }
    }

    public static class H0ResourceDiagnostics
    {
        public static StructuredError Exceeded(
            string machineCode,
            string message,
            string path,
            string dimension,
            long limit,
            long observed,
            string repairHint)
        {
            return new StructuredError(
                machineCode,
                message,
                path,
                new ReadOnlyDictionary<string, object?>(
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["resourceEnvelope"] = H0ResourceEnvelope.SchemaId,
                        ["dimension"] = dimension,
                        ["limit"] = limit,
                        ["observed"] = observed
                    }),
                false,
                repairHint);
        }

        public static StructuredError ExecutionExceeded(string boundary)
        {
            return Exceeded(
                "resource.execution_budget_exceeded",
                "The operation exhausted the H0 execution budget before authoritative publication.",
                "$",
                "executionMilliseconds",
                H0ResourceEnvelope.MaximumExecutionMilliseconds,
                H0ResourceEnvelope.MaximumExecutionMilliseconds + 1L,
                "Reduce the bounded request shape and retry; no canonical publication occurred at " + boundary + ".");
        }

        public static StructuredError ExecutionCancelled(string boundary)
        {
            return new StructuredError(
                "resource.execution_cancelled",
                "The operation was cancelled before authoritative publication.",
                "$",
                new ReadOnlyDictionary<string, object?>(
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["resourceEnvelope"] = H0ResourceEnvelope.SchemaId,
                        ["boundary"] = boundary
                    }),
                true,
                "Retry with a live request scope; no canonical publication occurred at " + boundary + ".");
        }

        public static StructuredError PublicationInterrupted(string boundary)
        {
            return new StructuredError(
                "resource.persistence_interrupted",
                "Persistence was interrupted after staging and before authoritative aggregate publication.",
                "$",
                new ReadOnlyDictionary<string, object?>(
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["resourceEnvelope"] = H0ResourceEnvelope.SchemaId,
                        ["boundary"] = boundary
                    }),
                true,
                "Retry the same idempotent request; the previous canonical aggregate and evidence remain authoritative.");
        }
    }

    /// <summary>
    /// One cooperative execution budget. Mutation/rebase/replay authorities check it at their
    /// accepted aggregate publication boundary. A committed publication always wins over a later
    /// cancellation so the public outcome cannot falsely claim that authoritative state was absent.
    /// </summary>
    public sealed class InvocationResourceBudget
    {
        private readonly CancellationToken _cancellationToken;
        private readonly long _deadlineTimestamp;
        private readonly Func<long> _timestamp;
        private readonly Func<string, bool>? _publicationPermit;
        private readonly bool _authoritativePublication;
        private bool _publicationCommitted;

        private InvocationResourceBudget(
            CancellationToken cancellationToken,
            long deadlineTimestamp,
            Func<long> timestamp,
            Func<string, bool>? publicationPermit,
            bool authoritativePublication)
        {
            _cancellationToken = cancellationToken;
            _deadlineTimestamp = deadlineTimestamp;
            _timestamp = timestamp ?? throw new ArgumentNullException(nameof(timestamp));
            _publicationPermit = publicationPermit;
            _authoritativePublication = authoritativePublication;
        }

        public bool PublicationCommitted => _publicationCommitted;

        public static InvocationResourceBudget Start(CancellationToken cancellationToken = default)
        {
            var now = Environment.TickCount64;
            return new InvocationResourceBudget(
                cancellationToken,
                checked(now + H0ResourceEnvelope.MaximumExecutionMilliseconds),
                () => Environment.TickCount64,
                null,
                true);
        }

        internal static InvocationResourceBudget Unlimited()
        {
            return new InvocationResourceBudget(
                CancellationToken.None,
                long.MaxValue,
                () => 0L,
                null,
                true);
        }

        internal static InvocationResourceBudget InterruptedAtPublication(string boundary)
        {
            if (string.IsNullOrWhiteSpace(boundary)) throw new ArgumentException("Publication boundary is required.", nameof(boundary));
            return new InvocationResourceBudget(
                CancellationToken.None,
                long.MaxValue,
                () => 0L,
                candidate => !string.Equals(candidate, boundary, StringComparison.Ordinal),
                true);
        }

        internal static InvocationResourceBudget ExpiredForTesting()
        {
            return new InvocationResourceBudget(
                CancellationToken.None,
                0L,
                () => 1L,
                null,
                true);
        }

        internal InvocationResourceBudget ForStaging()
        {
            return new InvocationResourceBudget(
                _cancellationToken,
                _deadlineTimestamp,
                _timestamp,
                null,
                false);
        }

        public bool TryContinue(string boundary, out StructuredError? error)
        {
            if (_publicationCommitted)
            {
                error = null;
                return true;
            }

            if (_cancellationToken.IsCancellationRequested)
            {
                error = H0ResourceDiagnostics.ExecutionCancelled(boundary);
                return false;
            }

            if (_timestamp() > _deadlineTimestamp)
            {
                error = H0ResourceDiagnostics.ExecutionExceeded(boundary);
                return false;
            }

            error = null;
            return true;
        }

        public bool TryBeginPublication(string boundary, out StructuredError? error)
        {
            if (string.IsNullOrWhiteSpace(boundary)) throw new ArgumentException("Publication boundary is required.", nameof(boundary));
            if (!TryContinue(boundary, out error)) return false;
            if (_publicationPermit != null && !_publicationPermit(boundary))
            {
                error = H0ResourceDiagnostics.PublicationInterrupted(boundary);
                return false;
            }

            return true;
        }

        public void MarkPublicationCommitted()
        {
            if (_authoritativePublication) _publicationCommitted = true;
        }
    }
}
