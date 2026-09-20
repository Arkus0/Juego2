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
    public sealed class Hk08ACursorIntegrityTests
    {
        private static readonly ContractVersionRange ExactV1 =
            ContractVersionRange.Exact(new ContractVersion(1, 0));

        [Fact]
        public void AlteredCursorOffsetFailsClosedInsteadOfSkippingJournalEntries()
        {
            var initial = new WorldState(
                new WorldId("world.hk08a.cursor-integrity"),
                0,
                Array.Empty<WorldObject>());
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = CanonicalWorldContract.Compose(new WorldInspectionService(session), session);

            for (var index = 0; index < 3; index++) ApplyObject(contract, session, index);

            var anchor = session.Current;
            var first = contract.Dispatch(
                WorldProvenanceContract.ReadName,
                ExactV1,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["revision"] = anchor.Revision,
                    ["hash"] = CanonicalWorldStateCodec.ComputeContentHash(anchor),
                    ["limit"] = 1
                });
            Assert.True(first.Success);
            var cursor = Assert.IsType<string>(first.Data!["nextCursor"]);

            // Controlled in-boundary defect reproduction: preserve the public envelope checksum while
            // changing only the inner continuation offset. A merely Base64-encoded cursor would accept
            // this and skip one persisted journal entry; the HK08A public cursor must fail closed.
            var altered = TamperInnerOffsetWithoutUpdatingEnvelopeChecksum(cursor);
            var rejected = contract.Dispatch(
                WorldProvenanceContract.ReadName,
                ExactV1,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["limit"] = 1,
                    ["cursor"] = altered
                });
            Assert.False(rejected.Success);
            Assert.Equal("world.provenance.invalid_cursor", rejected.Error!.MachineCode);
            Assert.Equal(anchor.Revision, session.Current.Revision);

            // The unchanged cursor still resumes at exactly the next persisted entry.
            var continued = contract.Dispatch(
                WorldProvenanceContract.ReadName,
                ExactV1,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["limit"] = 1,
                    ["cursor"] = cursor
                });
            Assert.True(continued.Success);
            Assert.Equal(1, Convert.ToInt32(continued.Data!["pageOffset"]));
            var entries = (IReadOnlyList<object?>)continued.Data!["entries"]!;
            var entry = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(Assert.Single(entries));
            Assert.Equal(2L, Convert.ToInt64(entry["sequence"]));
        }

        private static string TamperInnerOffsetWithoutUpdatingEnvelopeChecksum(string publicCursor)
        {
            var envelopeText = Encoding.UTF8.GetString(Convert.FromBase64String(publicCursor));
            var envelopeParts = envelopeText.Split('\n');
            Assert.Equal(3, envelopeParts.Length);

            var innerText = Encoding.UTF8.GetString(Convert.FromBase64String(envelopeParts[1]));
            var innerParts = innerText.Split('\n');
            Assert.Equal(10, innerParts.Length);
            innerParts[9] = (int.Parse(innerParts[9], System.Globalization.CultureInfo.InvariantCulture) + 1)
                .ToString(System.Globalization.CultureInfo.InvariantCulture);

            envelopeParts[1] = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join("\n", innerParts)));
            // Intentionally retain envelopeParts[2], the checksum of the original cursor context.
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join("\n", envelopeParts)));
        }

        private static void ApplyObject(
            ComposedContract contract,
            TransactionalWorldAuthoringSession session,
            int index)
        {
            var current = session.Current;
            var result = contract.Dispatch(
                WorldMutationContract.ApplyName,
                ExactV1,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["idempotencyKey"] = "request.hk08a.cursor-integrity." + index,
                    ["expectedRevision"] = current.Revision,
                    ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(current),
                    ["operations"] = new object?[]
                    {
                        new Dictionary<string, object?>(StringComparer.Ordinal)
                        {
                            ["kind"] = "put-object",
                            ["id"] = "cursor.item." + index,
                            ["typeId"] = "fixture.hk08a.cursor-item",
                            ["references"] = Array.Empty<object?>()
                        }
                    }
                });
            Assert.True(result.Success);
        }
    }
}
