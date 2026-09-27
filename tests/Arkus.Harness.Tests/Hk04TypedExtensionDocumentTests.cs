using System;
using System.Collections.Generic;
using System.Text;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    /// <summary>
    /// WP-HK-04 reopen 1 (typed extension documents), triggered by the WP-H1-GATE fresh-agent trials: a client may send
    /// a structured document that the extension owner's registered codec canonicalizes inside the transaction, instead
    /// of transcribing an opaque payload. The document path must be indistinguishable from the payloadBase64 path in
    /// state, hash, request fingerprint and journal.
    /// </summary>
    public sealed class Hk04TypedExtensionDocumentTests
    {
        private const string Owner = "probe.document";

        [Fact]
        public void DocumentAuthoredExtensionIsIdenticalToThePayloadAuthoredExtension()
        {
            var initial = new WorldState(new WorldId("world.hk04.documents"), 0, Array.Empty<WorldObject>());
            var documentSession = new PortableWorldAuthoringSession(initial, Codecs());
            var payloadSession = new PortableWorldAuthoringSession(initial);
            var documentContract = Compose(documentSession);
            var payloadContract = Compose(payloadSession);

            var documentRequest = Request(initial, "request.hk04-document", Subject("subject.a"), Subject("subject.b"),
                DocumentOperation("subject.a", Map(("value", "alpha"), ("target", "subject.b"))));
            var payloadRequest = Request(initial, "request.hk04-document", Subject("subject.a"), Subject("subject.b"),
                PayloadOperation("subject.a", ProbeCodec.Payload("alpha"), ("refers-to", "subject.b")));

            var documentPlan = Hk04TransactionalMutationTests.Success(documentContract, WorldMutationContract.PlanName, documentRequest);
            var payloadPlan = Hk04TransactionalMutationTests.Success(payloadContract, WorldMutationContract.PlanName, payloadRequest);
            Assert.Equal(Json(payloadPlan.Data), Json(documentPlan.Data));

            Hk04TransactionalMutationTests.Success(documentContract, WorldMutationContract.ApplyName, documentRequest);
            Hk04TransactionalMutationTests.Success(payloadContract, WorldMutationContract.ApplyName, payloadRequest);
            Assert.Equal(
                CanonicalWorldStateCodec.ComputeContentHash(payloadSession.Current),
                CanonicalWorldStateCodec.ComputeContentHash(documentSession.Current));
            Assert.Equal(Json(Hk06AProvenanceJournalTests.ReadJournal(payloadContract).Data), Json(Hk06AProvenanceJournalTests.ReadJournal(documentContract).Data));

            // Same request fingerprint: re-sending the payload form under the same key is an idempotent replay, not a conflict.
            var replay = Hk04TransactionalMutationTests.Success(documentContract, WorldMutationContract.ApplyName, payloadRequest);
            Assert.True((bool)replay.Data!["replayed"]!);
        }

        [Fact]
        public void DocumentDiagnosticsAreStructuredAndLeaveStateUnchanged()
        {
            var initial = new WorldState(new WorldId("world.hk04.documents"), 0, Array.Empty<WorldObject>());
            var session = new PortableWorldAuthoringSession(initial, Codecs());
            var contract = Compose(session);
            var before = CanonicalWorldStateCodec.ComputeContentHash(session.Current);

            StructuredError Reject(IReadOnlyDictionary<string, object?> extension, string code, string path)
            {
                var result = contract.Dispatch(WorldMutationContract.PlanName, Hk04TransactionalMutationTests.ExactVersion(),
                    Request(initial, "request.hk04-document-error", Subject("subject.a"), extension));
                Assert.False(result.Success);
                Assert.Equal(code, result.Error!.MachineCode);
                Assert.Equal(path, result.Error.Path);
                Assert.False(result.Error.Retryable);
                Assert.Empty(PortableData.Validate(result.Error.ToData()));
                Assert.Empty(CanonicalContractSchemas.StructuredError().ValidateValue(result.Error.ToData()));
                return result.Error;
            }

            var unsupported = Reject(new Dictionary<string, object?>(DocumentOperation("subject.a", Map(("value", "alpha"))), StringComparer.Ordinal)
            {
                ["owner"] = "probe.other"
            }, "world.change.extension_document_unsupported", "$.operations[1].document");
            Assert.Equal(new object?[] { Owner + "@1" }, (IReadOnlyList<object?>)unsupported.Context["supportedDocumentOwners"]!);

            var rejected = Reject(DocumentOperation("subject.a", Map(("target", "subject.b"))),
                "world.change.invalid_extension_document", "$.operations[1].document.value");
            Assert.Equal("probe.missing-value", rejected.Context["extensionMachineCode"]);
            Assert.Equal("$.value", rejected.Context["extensionPath"]);
            Assert.Equal("Add a string value.", rejected.RepairHint);

            var both = new Dictionary<string, object?>(DocumentOperation("subject.a", Map(("value", "alpha"))), StringComparer.Ordinal)
            {
                ["payloadBase64"] = Convert.ToBase64String(ProbeCodec.Payload("alpha"))
            };
            Assert.Equal(new object?[] { "payloadBase64", "document" },
                (IReadOnlyList<object?>)Reject(both, "world.change.invalid_request", "$.operations[1]").Context["oneOfFields"]!);
            var neither = new Dictionary<string, object?>(StringComparer.Ordinal) { ["kind"] = "put-extension", ["owner"] = Owner, ["schemaVersion"] = 1L };
            Reject(neither, "world.change.invalid_request", "$.operations[1]");

            var suppliedDependencies = new Dictionary<string, object?>(DocumentOperation("subject.a", Map(("value", "alpha"))), StringComparer.Ordinal)
            {
                ["dependencies"] = new List<object?>().AsReadOnly()
            };
            Reject(suppliedDependencies, "world.change.invalid_request", "$.operations[1].dependencies");

            var notObject = new Dictionary<string, object?>(DocumentOperation("subject.a", Map(("value", "alpha"))), StringComparer.Ordinal)
            {
                ["document"] = "alpha"
            };
            // A non-object document is refused by the published request schema before the kernel parser.
            var notObjectResult = contract.Dispatch(WorldMutationContract.PlanName, Hk04TransactionalMutationTests.ExactVersion(),
                Request(initial, "request.hk04-document-not-object", Subject("subject.a"), notObject));
            Assert.Equal("contract.invalid_request", notObjectResult.Error!.MachineCode);

            // A defective codec (throws, or derives an invalid dependency) is contained as a structured codec fault.
            var faulty = new PortableWorldAuthoringSession(initial, ExtensionDocumentCodecs.Create(new IExtensionDocumentCodec[] { new ThrowingCodec(), new BadDependencyCodec() }));
            var faultyContract = Compose(faulty);
            foreach (var owner in new[] { ThrowingCodec.OwnerId, BadDependencyCodec.OwnerId })
            {
                var extension = new Dictionary<string, object?>(DocumentOperation("subject.a", Map(("value", "alpha"))), StringComparer.Ordinal) { ["owner"] = owner };
                var result = faultyContract.Dispatch(WorldMutationContract.PlanName, Hk04TransactionalMutationTests.ExactVersion(),
                    Request(initial, "request.hk04-codec-fault", Subject("subject.a"), extension));
                Assert.Equal("world.change.invalid_extension_document", result.Error!.MachineCode);
                Assert.Equal("extension.codec-fault", result.Error.Context["extensionMachineCode"]);
            }

            Assert.Equal(before, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
        }

        [Fact]
        public void CodecsSurviveSnapshotImportAndAreDiscoverableInTheRequestSchema()
        {
            var initial = new WorldState(new WorldId("world.hk04.documents"), 0, Array.Empty<WorldObject>());
            var session = new PortableWorldAuthoringSession(initial, Codecs());
            var contract = Compose(session);
            var snapshot = Hk04TransactionalMutationTests.Success(contract, WorldPortabilityContract.ExportName, Hk01TestFixtures.EmptyRequest()).Data!;
            Hk04TransactionalMutationTests.Success(contract, WorldPortabilityContract.ImportName, new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["idempotencyKey"] = "request.hk04-import",
                ["expectedRevision"] = session.Current.Revision,
                ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(session.Current),
                ["snapshot"] = snapshot
            });

            // The imported (new-local-lineage) inner session still admits documents.
            var current = session.Current;
            Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.ApplyName,
                Request(current, "request.hk04-after-import", Subject("subject.a"), DocumentOperation("subject.a", Map(("value", "beta")))));
            Assert.Equal(1, session.Current.Revision - current.Revision);

            var definition = FindDefinition(contract, WorldMutationContract.PlanName);
            var operation = definition.RequestSchema!.Root.Properties["operations"].Items!;
            Assert.True(operation.Properties.ContainsKey("document"));
            Assert.Equal(SchemaValueType.Object, operation.Properties["document"].ValueType);
        }

        internal static ExtensionDocumentCodecs Codecs() => ExtensionDocumentCodecs.Create(new IExtensionDocumentCodec[] { new ProbeCodec() });

        private static ComposedContract Compose(PortableWorldAuthoringSession session) =>
            CanonicalWorldContract.Compose(new WorldInspectionService(session), session);

        private static CapabilityDefinition FindDefinition(ComposedContract contract, string name)
        {
            foreach (var definition in contract.Definitions)
                if (string.Equals(definition.Key.Name, name, StringComparison.Ordinal)) return definition;
            throw new InvalidOperationException("Missing definition " + name);
        }

        private static Dictionary<string, object?> Request(WorldState state, string key, params IReadOnlyDictionary<string, object?>[] operations) =>
            Hk04TransactionalMutationTests.Request(state, key, operations);

        private static IReadOnlyDictionary<string, object?> Subject(string id) =>
            new Dictionary<string, object?>(StringComparer.Ordinal) { ["kind"] = "put-object", ["id"] = id, ["typeId"] = "probe.subject" };

        private static IReadOnlyDictionary<string, object?> DocumentOperation(string subjectId, IReadOnlyDictionary<string, object?> document) =>
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = "put-extension", ["owner"] = Owner, ["schemaVersion"] = 1L, ["subjectId"] = subjectId, ["document"] = document
            };

        private static IReadOnlyDictionary<string, object?> PayloadOperation(string subjectId, byte[] payload, params (string Kind, string Target)[] dependencies)
        {
            var list = new List<object?>();
            foreach (var (kind, target) in dependencies)
                list.Add(new Dictionary<string, object?>(StringComparer.Ordinal) { ["kind"] = kind, ["targetId"] = target });
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = "put-extension", ["owner"] = Owner, ["schemaVersion"] = 1L, ["subjectId"] = subjectId,
                ["dependencies"] = list.AsReadOnly(), ["payloadBase64"] = Convert.ToBase64String(payload)
            };
        }

        private static IReadOnlyDictionary<string, object?> Map(params (string Key, object? Value)[] values)
        {
            var map = new Dictionary<string, object?>(StringComparer.Ordinal);
            foreach (var (key, value) in values) map[key] = value;
            return map;
        }

        private static string Json(object? value) => System.Text.Json.JsonSerializer.Serialize(value);

        private sealed class ProbeCodec : IExtensionDocumentCodec
        {
            public string Owner => Hk04TypedExtensionDocumentTests.Owner;
            public int SchemaVersion => 1;

            public static byte[] Payload(string value) => Encoding.UTF8.GetBytes("PD1:" + value);

            public ExtensionDocumentEncoding Encode(string? subjectId, IReadOnlyDictionary<string, object?> document)
            {
                if (!(document.TryGetValue("value", out var raw) && raw is string value))
                    return ExtensionDocumentEncoding.Rejected("probe.missing-value", "$.value", "value is required.", "Add a string value.");
                var dependencies = new List<IReadOnlyDictionary<string, object?>>();
                if (document.TryGetValue("target", out var target) && target is string targetId)
                    dependencies.Add(new Dictionary<string, object?>(StringComparer.Ordinal) { ["kind"] = "refers-to", ["targetId"] = targetId });
                return ExtensionDocumentEncoding.Encoded(Payload(value), dependencies);
            }
        }

        private sealed class ThrowingCodec : IExtensionDocumentCodec
        {
            public const string OwnerId = "probe.throwing";
            public string Owner => OwnerId;
            public int SchemaVersion => 1;
            public ExtensionDocumentEncoding Encode(string? subjectId, IReadOnlyDictionary<string, object?> document) =>
                throw new InvalidOperationException("injected codec defect");
        }

        private sealed class BadDependencyCodec : IExtensionDocumentCodec
        {
            public const string OwnerId = "probe.bad-dependency";
            public string Owner => OwnerId;
            public int SchemaVersion => 1;
            public ExtensionDocumentEncoding Encode(string? subjectId, IReadOnlyDictionary<string, object?> document) =>
                ExtensionDocumentEncoding.Encoded(new byte[] { 1 }, new[]
                {
                    (IReadOnlyDictionary<string, object?>)new Dictionary<string, object?>(StringComparer.Ordinal) { ["kind"] = "not a token", ["targetId"] = "x" }
                });
        }
    }
}
