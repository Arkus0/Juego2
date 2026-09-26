using System;
using System.Collections.Generic;
using Arkus.Harness.Protocol;

namespace Arkus.EngineBridge.UnityAuthoring
{
    /// <summary>
    /// WP-H1-01 reopen 1: typed-document codec for the <c>arkus.unity-binding</c> extension (WP-HK-04 reopen 1). The
    /// document is the same structured binding that <c>unity.binding.compile</c> accepts; the codec uses the compiler's own
    /// normalization and encoding, so a document-authored binding is byte-identical to the compiled payload.
    /// </summary>
    public sealed class UnityBindingDocumentCodec : IExtensionDocumentCodec
    {
        private const string BindingPathPrefix = "$.binding";
        private const string RepairHint =
            "Repair the binding document. unity.binding.compile validates the same document and returns a ready documentMutation.";

        public string Owner => UnityBindingProducer.ExtensionOwner;
        public int SchemaVersion => UnityBindingProducer.ExtensionSchemaVersion;

        public ExtensionDocumentEncoding Encode(string? subjectId, IReadOnlyDictionary<string, object?> document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (subjectId == null)
            {
                return ExtensionDocumentEncoding.Rejected(
                    "unity.binding.missing-subject",
                    "$",
                    "A Unity binding extension must be attached to the canonical object it realizes.",
                    "Set subjectId on the put-extension operation to that object's id.");
            }

            try
            {
                UnityBindingProducer.CanonicalizeForExtension(document, out var payload, out var dependencies);
                var derived = new List<IReadOnlyDictionary<string, object?>>();
                foreach (var dependency in dependencies) derived.Add((IReadOnlyDictionary<string, object?>)dependency!);
                return ExtensionDocumentEncoding.Encoded(payload, derived);
            }
            catch (UnityBindingException exception)
            {
                return ExtensionDocumentEncoding.Rejected(exception.MachineCode, DocumentPath(exception.Path), exception.Message, RepairHint);
            }
        }

        // Compiler paths are rooted at the request's "binding" property; codec paths are rooted at the document.
        private static string DocumentPath(string path) =>
            path.StartsWith(BindingPathPrefix, StringComparison.Ordinal) ? "$" + path.Substring(BindingPathPrefix.Length) : "$";
    }
}
