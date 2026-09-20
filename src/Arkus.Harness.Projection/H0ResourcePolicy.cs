using System;
using System.Collections.Generic;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Projection
{
    /// <summary>
    /// Transport-neutral H0 request admission. Adapters may reject oversized framing earlier, but
    /// every successfully framed request crosses this same semantic resource boundary.
    /// </summary>
    internal static class H0ResourcePolicy
    {
        private static readonly HashSet<string> MutationCapabilities = new HashSet<string>(StringComparer.Ordinal)
        {
            "authoring.change.plan",
            "authoring.change.dry-run",
            "authoring.change.apply"
        };

        private static readonly HashSet<string> PagedCapabilities = new HashSet<string>(StringComparer.Ordinal)
        {
            "world.object.query",
            "world.reference.query",
            "world.extension.query",
            "world.extension.read",
            "authoring.journal.read"
        };

        public static StructuredError? Validate(NeutralProjectionRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (!PortableData.TryMeasure(
                    request.Arguments,
                    H0ResourceEnvelope.MaximumPortableDepth,
                    H0ResourceEnvelope.MaximumCanonicalRequestBytes,
                    out var metrics,
                    out var issue))
            {
                if (issue != null && string.Equals(issue.Code, "portable.depth_limit", StringComparison.Ordinal))
                {
                    return H0ResourceDiagnostics.Exceeded(
                        "resource.nesting_depth_exceeded",
                        "The canonical request exceeds the H0 portable nesting limit.",
                        issue.Path,
                        "portableDepth",
                        H0ResourceEnvelope.MaximumPortableDepth,
                        metrics.MaximumDepth,
                        "Flatten the request to the depth advertised by system.resource-envelope.describe.");
                }

                if (issue != null && string.Equals(issue.Code, "portable.byte_limit", StringComparison.Ordinal))
                {
                    return H0ResourceDiagnostics.Exceeded(
                        "resource.request_bytes_exceeded",
                        "The canonical request exceeds the H0 argument-byte limit.",
                        issue.Path,
                        "canonicalArgumentBytes",
                        H0ResourceEnvelope.MaximumCanonicalRequestBytes,
                        metrics.Utf8JsonBytes,
                        "Reduce the request payload without splitting one coherent accepted transaction.");
                }

                return null;
            }

            if (MutationCapabilities.Contains(request.CapabilityName))
            {
                var mutation = ValidateMutation(request.Arguments);
                if (mutation != null) return mutation;
            }

            if (PagedCapabilities.Contains(request.CapabilityName) &&
                TryInteger(request.Arguments, "limit", out var pageSize) &&
                pageSize > H0ResourceEnvelope.MaximumPageSize)
            {
                return H0ResourceDiagnostics.Exceeded(
                    "resource.page_size_exceeded",
                    "The requested page exceeds the H0 page-size limit.",
                    "$.arguments.limit",
                    "pageItems",
                    H0ResourceEnvelope.MaximumPageSize,
                    pageSize,
                    "Request a page no larger than the advertised maximum and continue with the returned cursor.");
            }

            if (string.Equals(request.CapabilityName, "authoring.snapshot.import", StringComparison.Ordinal) &&
                TrySnapshotBytes(request.Arguments, "snapshot", out var snapshotBytes) &&
                snapshotBytes > H0ResourceEnvelope.MaximumCanonicalWorldBytes)
            {
                return SnapshotTooLarge("$.arguments.snapshot.authoredStateBase64", snapshotBytes);
            }

            if (string.Equals(request.CapabilityName, "authoring.diff.compare", StringComparison.Ordinal))
            {
                if (TrySnapshotBytes(request.Arguments, "base", out snapshotBytes) &&
                    snapshotBytes > H0ResourceEnvelope.MaximumCanonicalWorldBytes)
                    return SnapshotTooLarge("$.arguments.base.authoredStateBase64", snapshotBytes);
                if (TrySnapshotBytes(request.Arguments, "target", out snapshotBytes) &&
                    snapshotBytes > H0ResourceEnvelope.MaximumCanonicalWorldBytes)
                    return SnapshotTooLarge("$.arguments.target.authoredStateBase64", snapshotBytes);
            }

            return null;
        }

        private static StructuredError? ValidateMutation(IReadOnlyDictionary<string, object?> request)
        {
            if (!request.TryGetValue("operations", out var raw) || !(raw is IReadOnlyList<object?> operations))
                return null;
            if (operations.Count > H0ResourceEnvelope.MaximumBatchOperations)
            {
                return H0ResourceDiagnostics.Exceeded(
                    "resource.batch_operations_exceeded",
                    "The mutation batch exceeds the measured H0 atomic operation envelope.",
                    "$.arguments.operations",
                    "batchOperations",
                    H0ResourceEnvelope.MaximumBatchOperations,
                    operations.Count,
                    "Keep the coherent intent at or below the advertised 96-operation atomic envelope.");
            }

            long payloadBytes = 0;
            for (var index = 0; index < operations.Count; index++)
            {
                if (!(operations[index] is IReadOnlyDictionary<string, object?> operation)) continue;
                var relationField = operation.ContainsKey("dependencies") ? "dependencies" : "references";
                if (operation.TryGetValue(relationField, out var relationsRaw) &&
                    relationsRaw is IReadOnlyList<object?> relations &&
                    relations.Count > H0ResourceEnvelope.MaximumRelationsPerResource)
                {
                    return H0ResourceDiagnostics.Exceeded(
                        "resource.relation_count_exceeded",
                        "One authored resource exceeds the H0 relation-count limit.",
                        "$.arguments.operations[" + index + "]." + relationField,
                        "relationsPerResource",
                        H0ResourceEnvelope.MaximumRelationsPerResource,
                        relations.Count,
                        "Reduce the relations attached to this resource and retry the same coherent transaction.");
                }

                if (!operation.TryGetValue("payloadBase64", out var payloadRaw) || !(payloadRaw is string payload))
                    continue;
                var decodedBytes = EstimateDecodedBase64Bytes(payload);
                if (decodedBytes > H0ResourceEnvelope.MaximumExtensionPayloadBytes)
                {
                    return H0ResourceDiagnostics.Exceeded(
                        "resource.extension_payload_exceeded",
                        "One extension payload exceeds the H0 per-resource byte limit.",
                        "$.arguments.operations[" + index + "].payloadBase64",
                        "extensionPayloadBytes",
                        H0ResourceEnvelope.MaximumExtensionPayloadBytes,
                        decodedBytes,
                        "Reduce this extension payload while preserving the canonical resource boundary.");
                }

                payloadBytes = checked(payloadBytes + decodedBytes);
            }

            if (payloadBytes > H0ResourceEnvelope.MaximumBatchPayloadBytes)
            {
                return H0ResourceDiagnostics.Exceeded(
                    "resource.batch_payload_exceeded",
                    "The mutation batch exceeds the H0 decoded-payload byte limit.",
                    "$.arguments.operations",
                    "decodedBatchPayloadBytes",
                    H0ResourceEnvelope.MaximumBatchPayloadBytes,
                    payloadBytes,
                    "Reduce opaque payload volume without splitting the accepted coherent authoring intent.");
            }

            return null;
        }

        private static StructuredError SnapshotTooLarge(string path, long bytes)
        {
            return H0ResourceDiagnostics.Exceeded(
                "resource.snapshot_bytes_exceeded",
                "The snapshot exceeds the H0 decoded canonical-state byte limit.",
                path,
                "snapshotStateBytes",
                H0ResourceEnvelope.MaximumCanonicalWorldBytes,
                bytes,
                "Use a snapshot inside the advertised H0 world envelope; canonical state was not replaced.");
        }

        private static bool TrySnapshotBytes(
            IReadOnlyDictionary<string, object?> request,
            string field,
            out long bytes)
        {
            bytes = 0;
            return request.TryGetValue(field, out var raw) &&
                raw is IReadOnlyDictionary<string, object?> snapshot &&
                snapshot.TryGetValue("authoredStateBase64", out var payloadRaw) &&
                payloadRaw is string payload &&
                (bytes = EstimateDecodedBase64Bytes(payload)) >= 0;
        }

        private static long EstimateDecodedBase64Bytes(string value)
        {
            if (value == null || value.Length == 0) return 0;
            var padding = value.EndsWith("==", StringComparison.Ordinal)
                ? 2
                : value.EndsWith("=", StringComparison.Ordinal) ? 1 : 0;
            return checked((long)value.Length / 4L * 3L - padding);
        }

        private static bool TryInteger(
            IReadOnlyDictionary<string, object?> data,
            string key,
            out long value)
        {
            value = 0;
            if (!data.TryGetValue(key, out var raw)) return false;
            switch (raw)
            {
                case sbyte current: value = current; return true;
                case byte current: value = current; return true;
                case short current: value = current; return true;
                case ushort current: value = current; return true;
                case int current: value = current; return true;
                case uint current: value = current; return true;
                case long current: value = current; return true;
                case ulong current when current <= long.MaxValue: value = (long)current; return true;
                default: return false;
            }
        }
    }
}
