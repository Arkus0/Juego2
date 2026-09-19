using System;
using System.Collections.Generic;
using System.Reflection;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk06BRebaseContractTests
    {
        [Fact]
        public void SnapshotImportIsCanonicalRebaseWithRequiredTruthfulEvidence()
        {
            var source = new PortableWorldAuthoringSession(Hk02TestFixtures.MicroWorld());
            var sourceContract = Compose(source);
            var snapshot = Hk04TransactionalMutationTests.Success(
                sourceContract,
                WorldPortabilityContract.ExportName,
                Hk01TestFixtures.EmptyRequest()).Data!;

            var targetInitial = new WorldState(new WorldId("world.rebase-target"), 0, Array.Empty<WorldObject>());
            var target = new PortableWorldAuthoringSession(targetInitial);
            var targetContract = Compose(target);
            var definition = FindDefinition(targetContract, WorldPortabilityContract.ImportName);

            Assert.Equal(SideEffectClass.CanonicalRebase, definition.SideEffect);
            Assert.NotEqual(SideEffectClass.CanonicalMutation, definition.SideEffect);
            Assert.NotNull(definition.Policy);
            Assert.Equal(TransactionRequirement.CanonicalRebase, definition.Policy!.TransactionRequirement);
            Assert.Equal(ProvenanceRequirement.Required, definition.Policy.ProvenanceRequirement);

            var idempotencyKey = "request.hk06b.rebase-evidence";
            var request = ImportRequest(target.Current, snapshot, idempotencyKey);
            var result = Hk04TransactionalMutationTests.Success(
                targetContract,
                WorldPortabilityContract.ImportName,
                request);
            var resultData = result.Data!;

            Assert.Empty(definition.SuccessSchema!.ValidateValue(resultData));
            Assert.Equal(0, JournalCount(targetContract));

            var evidence = Hk04TransactionalMutationTests.Map(resultData, "rebaseEvidence");
            Assert.Equal(WorldPortabilityContract.RebaseEvidenceSchemaId, evidence["schemaId"]);
            Assert.Equal(idempotencyKey, evidence["idempotencyKey"]);
            Assert.Equal(WorldPortabilityContract.SnapshotSchemaId, evidence["snapshotSchemaId"]);
            Assert.Equal(snapshot["snapshotVersion"], evidence["snapshotVersion"]);
            Assert.Equal(snapshot["anchor"], evidence["snapshotAnchor"]);
            Assert.Equal(resultData["previous"], evidence["previous"]);
            Assert.Equal(resultData["current"], evidence["current"]);
            Assert.Equal("new-local-lineage", evidence["lineageDisposition"]);
            Assert.Equal("new-local-lineage-empty", evidence["mutationJournalDisposition"]);
            Assert.False(string.IsNullOrWhiteSpace((string)evidence["requestFingerprint"]!));

            var missingEvidence = new Dictionary<string, object?>(resultData, StringComparer.Ordinal);
            missingEvidence.Remove("rebaseEvidence");
            Assert.NotEmpty(definition.SuccessSchema.ValidateValue(missingEvidence));

            var retry = Hk04TransactionalMutationTests.Success(
                targetContract,
                WorldPortabilityContract.ImportName,
                request);
            Assert.True((bool)retry.Data!["replayed"]!);
            Assert.Equal(evidence, retry.Data["rebaseEvidence"]);
            Assert.Equal(0, JournalCount(targetContract));
        }

        [Fact]
        public void RebaseAuthorityCannotMasqueradeAsHk04MutationAuthority()
        {
            var session = new PortableWorldAuthoringSession(Hk02TestFixtures.MicroWorld());
            var contract = Compose(session);
            var mutation = MutationSurfaceConformance.Evaluate(
                contract,
                new[] { typeof(SystemDescribeHandler).Assembly });
            Assert.True(mutation.IsConformant, FormatMutation(mutation.Issues));

            var import = FindDefinition(contract, WorldPortabilityContract.ImportName);
            Assert.Equal(SideEffectClass.CanonicalRebase, import.SideEffect);
            Assert.Equal(TransactionRequirement.CanonicalRebase, import.Policy!.TransactionRequirement);

            var handler = typeof(SystemDescribeHandler).Assembly.GetType(
                "Arkus.Harness.Runtime.WorldSnapshotImportHandler",
                throwOnError: true)!;
            Assert.False(typeof(ITransactionalMutationHandler).IsAssignableFrom(handler));
            Assert.Contains(handler.GetInterfaces(), value =>
                string.Equals(value.Name, "ICanonicalRebaseHandler", StringComparison.Ordinal));

            var apply = FindDefinition(contract, WorldMutationContract.ApplyName);
            Assert.Equal(SideEffectClass.CanonicalMutation, apply.SideEffect);
            Assert.Equal(TransactionRequirement.CanonicalTransaction, apply.Policy!.TransactionRequirement);
        }

        private static ComposedContract Compose(PortableWorldAuthoringSession session)
        {
            return CanonicalWorldContract.Compose(new WorldInspectionService(session), session);
        }

        private static CapabilityDefinition FindDefinition(ComposedContract contract, string name)
        {
            foreach (var definition in contract.Definitions)
            {
                if (string.Equals(definition.Key.Name, name, StringComparison.Ordinal)) return definition;
            }

            throw new InvalidOperationException("Missing capability definition: " + name);
        }

        private static IReadOnlyDictionary<string, object?> ImportRequest(
            WorldState current,
            IReadOnlyDictionary<string, object?> snapshot,
            string idempotencyKey)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["idempotencyKey"] = idempotencyKey,
                ["expectedRevision"] = current.Revision,
                ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(current),
                ["snapshot"] = snapshot
            };
        }

        private static int JournalCount(ComposedContract contract)
        {
            return Convert.ToInt32(Hk06AProvenanceJournalTests.ReadJournal(contract).Data!["entryCount"]);
        }

        private static string FormatMutation(IReadOnlyList<MutationSurfaceIssue> issues)
        {
            var value = string.Empty;
            foreach (var issue in issues) value += issue.Code + ":" + issue.Subject + ";";
            return value;
        }
    }
}
