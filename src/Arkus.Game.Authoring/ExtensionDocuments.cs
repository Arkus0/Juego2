using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Arkus.Game.Authoring
{
    /// <summary>
    /// Engine-neutral port that lets an extension owner accept a structured JSON document inside an ordinary
    /// <c>put-extension</c> operation instead of a caller-transcribed opaque payload. The codec is a pure,
    /// deterministic function of the document and subject: the authoring kernel stores exactly the canonical bytes and
    /// dependencies it returns, so a document-authored extension is identical in state, hash, request fingerprint and
    /// journal to the same extension authored with <c>payloadBase64</c>. H0 never interprets the payload itself.
    /// </summary>
    public interface IExtensionDocumentCodec
    {
        /// <summary>Stable-token extension owner, for example <c>arkus.unity-binding</c>.</summary>
        string Owner { get; }

        /// <summary>Positive extension schema version this codec encodes.</summary>
        int SchemaVersion { get; }

        /// <summary>
        /// Canonicalizes one document. Must not throw for invalid input; it returns a rejection whose message and hint
        /// are public-safe and whose path is relative to the document root (<c>$</c>).
        /// </summary>
        ExtensionDocumentEncoding Encode(string? subjectId, IReadOnlyDictionary<string, object?> document);
    }

    public sealed class ExtensionDocumentEncoding
    {
        private ExtensionDocumentEncoding(
            byte[]? payload,
            IReadOnlyList<IReadOnlyDictionary<string, object?>>? dependencies,
            string? machineCode,
            string? documentPath,
            string? message,
            string? repairHint)
        {
            Payload = payload;
            Dependencies = dependencies ?? Array.Empty<IReadOnlyDictionary<string, object?>>();
            MachineCode = machineCode;
            DocumentPath = documentPath;
            Message = message;
            RepairHint = repairHint;
        }

        public bool Success => Payload != null;
        public byte[]? Payload { get; }

        /// <summary>Derived canonical dependencies as <c>{kind, targetId}</c> objects; H0 validates them like caller input.</summary>
        public IReadOnlyList<IReadOnlyDictionary<string, object?>> Dependencies { get; }

        public string? MachineCode { get; }
        public string? DocumentPath { get; }
        public string? Message { get; }
        public string? RepairHint { get; }

        public static ExtensionDocumentEncoding Encoded(byte[] payload, IReadOnlyList<IReadOnlyDictionary<string, object?>> dependencies)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (dependencies == null) throw new ArgumentNullException(nameof(dependencies));
            return new ExtensionDocumentEncoding((byte[])payload.Clone(), new List<IReadOnlyDictionary<string, object?>>(dependencies).AsReadOnly(), null, null, null, null);
        }

        public static ExtensionDocumentEncoding Rejected(string machineCode, string documentPath, string message, string repairHint)
        {
            if (string.IsNullOrEmpty(machineCode)) throw new ArgumentException("A rejection needs a machine code.", nameof(machineCode));
            if (documentPath == null || !documentPath.StartsWith("$", StringComparison.Ordinal))
                throw new ArgumentException("A rejection path is relative to the document root and starts with '$'.", nameof(documentPath));
            return new ExtensionDocumentEncoding(null, null, machineCode, documentPath,
                message ?? throw new ArgumentNullException(nameof(message)),
                repairHint ?? throw new ArgumentNullException(nameof(repairHint)));
        }
    }

    /// <summary>Immutable (owner, schemaVersion) → codec registry supplied to an authoring session at composition.</summary>
    public sealed class ExtensionDocumentCodecs
    {
        private readonly IReadOnlyDictionary<string, IExtensionDocumentCodec> _byKey;

        private ExtensionDocumentCodecs(IReadOnlyDictionary<string, IExtensionDocumentCodec> byKey, IReadOnlyList<string> supported)
        {
            _byKey = byKey;
            Supported = supported;
        }

        public static ExtensionDocumentCodecs Empty { get; } = new ExtensionDocumentCodecs(
            new ReadOnlyDictionary<string, IExtensionDocumentCodec>(new Dictionary<string, IExtensionDocumentCodec>(StringComparer.Ordinal)),
            Array.Empty<string>());

        /// <summary>Sorted <c>owner@schemaVersion</c> keys, for diagnostics and discovery.</summary>
        public IReadOnlyList<string> Supported { get; }

        public static ExtensionDocumentCodecs Create(IEnumerable<IExtensionDocumentCodec> codecs)
        {
            if (codecs == null) throw new ArgumentNullException(nameof(codecs));
            var byKey = new Dictionary<string, IExtensionDocumentCodec>(StringComparer.Ordinal);
            foreach (var codec in codecs)
            {
                if (codec == null) throw new ArgumentException("Codec entries may not be null.", nameof(codecs));
                if (string.IsNullOrEmpty(codec.Owner) || codec.SchemaVersion <= 0)
                    throw new ArgumentException("A codec needs a non-empty owner and a positive schema version.", nameof(codecs));
                var key = Key(codec.Owner, codec.SchemaVersion);
                if (byKey.ContainsKey(key)) throw new ArgumentException("Duplicate extension document codec: " + key, nameof(codecs));
                byKey.Add(key, codec);
            }

            var supported = new List<string>(byKey.Keys);
            supported.Sort(StringComparer.Ordinal);
            return new ExtensionDocumentCodecs(new ReadOnlyDictionary<string, IExtensionDocumentCodec>(byKey), supported.AsReadOnly());
        }

        public bool TryGet(string owner, int schemaVersion, out IExtensionDocumentCodec codec)
        {
            return _byKey.TryGetValue(Key(owner, schemaVersion), out codec!);
        }

        private static string Key(string owner, int schemaVersion) => owner + "@" + schemaVersion.ToString(CultureInfo.InvariantCulture);
    }
}
