using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Projection
{
    /// <summary>
    /// The separately versioned execution envelope for capabilities already admitted by
    /// arkus.unity-host-policy@1. It does not alter the H0 envelope and cannot be selected by a
    /// transport for a non-H1 capability.
    /// </summary>
    public static class H1UnityOperationCeilings
    {
        public const string SchemaId = "arkus.h1-unity-operation-ceilings@1";
        public const int MaximumExecutionMilliseconds = 120000;
        public const int MaximumLedgerEntries = 256;
        public const int MaximumPayloadBytes = 256 * 1024;

        public static IReadOnlyDictionary<string, object?> ToData()
        {
            return new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = SchemaId,
                ["maximumExecutionMilliseconds"] = MaximumExecutionMilliseconds,
                ["maximumLedgerEntries"] = MaximumLedgerEntries,
                ["maximumPayloadBytes"] = MaximumPayloadBytes,
                ["h0EnvelopeUnchanged"] = H0ResourceEnvelope.SchemaId
            });
        }
    }
}
