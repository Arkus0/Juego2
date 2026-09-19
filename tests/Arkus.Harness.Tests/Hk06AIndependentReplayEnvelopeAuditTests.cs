using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk06AIndependentReplayEnvelopeAuditTests
    {
        [Fact]
        public void IndependentAcceptedRequestOracleRejectsSchemaValidEnvelopeAndCanonicalIdentityCorruption()
        {
            var first = ExecuteRepresentativeAcceptedRequest();
            var second = ExecuteRepresentativeAcceptedRequest();

            Assert.Empty(Audit(first.Entry, first.AcceptedRequest, (string)second.Entry["entryId"]!));

            var wrongType = DeepCloneMap(first.Entry);
            Operation(Request(wrongType), 0)["typeId"] = "fixture.other";
            Assert.Contains("request-envelope-mismatch", Audit(wrongType, first.AcceptedRequest, (string)second.Entry["entryId"]!));

            var wrongPayload = DeepCloneMap(first.Entry);
            Operation(Request(wrongPayload), 1)["payloadBase64"] = Convert.ToBase64String(new byte[] { 0x44, 0x55, 0x67 });
            Assert.Contains("request-envelope-mismatch", Audit(wrongPayload, first.AcceptedRequest, (string)second.Entry["entryId"]!));

            var wrongCanonicalIdentity = DeepCloneMap(first.Entry);
            wrongCanonicalIdentity["entryId"] = new string('0', 64);
            Assert.Contains("entry-identity-mismatch", Audit(wrongCanonicalIdentity, first.AcceptedRequest, (string)second.Entry["entryId"]!));
        }

        private static Observation ExecuteRepresentativeAcceptedRequest()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);
            var acceptedRequest = Hk04TransactionalMutationTests.Request(
                initial,
                "request.hk06a.independent-envelope",
                Hk04TransactionalMutationTests.PutObject(
                    "node.extra",
                    "fixture.item",
                    "node.root",
                    Hk04TransactionalMutationTests.Reference("fixture.root", "node.root")),
                Hk06AProvenanceJournalTests.PutExtension(
                    "future.alpha",
                    2,
                    new byte[] { 0x44, 0x55, 0x66 },
                    "node.extra",
                    Hk04TransactionalMutationTests.Reference("fixture.root", "node.root")));

            Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.ApplyName, acceptedRequest);
            var entry = Assert.Single(Hk06AProvenanceJournalTests.Entries(
                Hk06AProvenanceJournalTests.ReadJournal(contract).Data!));
            return new Observation(acceptedRequest, entry);
        }

        private static IReadOnlyList<string> Audit(
            IReadOnlyDictionary<string, object?> entry,
            IReadOnlyDictionary<string, object?> independentlyAcceptedRequest,
            string independentlyExpectedEntryId)
        {
            var issues = new List<string>();
            if (!DeepEquals(
                    Hk04TransactionalMutationTests.Map(entry, "request"),
                    independentlyAcceptedRequest))
            {
                issues.Add("request-envelope-mismatch");
            }

            if (!Equals(entry["entryId"], independentlyExpectedEntryId))
            {
                issues.Add("entry-identity-mismatch");
            }

            return issues.AsReadOnly();
        }

        private static bool DeepEquals(object? left, object? right)
        {
            if (left is IReadOnlyDictionary<string, object?> leftMap &&
                right is IReadOnlyDictionary<string, object?> rightMap)
            {
                if (leftMap.Count != rightMap.Count) return false;
                foreach (var pair in leftMap)
                {
                    if (!rightMap.TryGetValue(pair.Key, out var other) || !DeepEquals(pair.Value, other))
                        return false;
                }

                return true;
            }

            if (left is IReadOnlyList<object?> leftList && right is IReadOnlyList<object?> rightList)
            {
                if (leftList.Count != rightList.Count) return false;
                for (var index = 0; index < leftList.Count; index++)
                {
                    if (!DeepEquals(leftList[index], rightList[index])) return false;
                }

                return true;
            }

            return Equals(left, right);
        }

        private static Dictionary<string, object?> Request(Dictionary<string, object?> entry)
        {
            return (Dictionary<string, object?>)entry["request"]!;
        }

        private static Dictionary<string, object?> Operation(Dictionary<string, object?> request, int index)
        {
            return (Dictionary<string, object?>)((List<object?>)request["operations"]!)[index]!;
        }

        private static Dictionary<string, object?> DeepCloneMap(IReadOnlyDictionary<string, object?> source)
        {
            var result = new Dictionary<string, object?>(StringComparer.Ordinal);
            foreach (var pair in source) result[pair.Key] = DeepClone(pair.Value);
            return result;
        }

        private static object? DeepClone(object? value)
        {
            if (value is IReadOnlyDictionary<string, object?> map) return DeepCloneMap(map);
            if (value is IReadOnlyList<object?> list)
            {
                var copy = new List<object?>();
                foreach (var item in list) copy.Add(DeepClone(item));
                return copy;
            }

            return value;
        }

        private sealed class Observation
        {
            public Observation(
                IReadOnlyDictionary<string, object?> acceptedRequest,
                IReadOnlyDictionary<string, object?> entry)
            {
                AcceptedRequest = acceptedRequest;
                Entry = entry;
            }

            public IReadOnlyDictionary<string, object?> AcceptedRequest { get; }
            public IReadOnlyDictionary<string, object?> Entry { get; }
        }
    }
}
