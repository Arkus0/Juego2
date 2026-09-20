using System;
using Arkus.Game.World;
using Arkus.Harness.Protocol;

namespace Arkus.Game.Authoring
{
    internal static class WorldResourceLimits
    {
        public static StructuredError? ValidateState(WorldState state, string path, out int canonicalBytes)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            canonicalBytes = 0;
            var resources = checked(state.Objects.Count + state.Extensions.Count);
            if (resources > H0ResourceEnvelope.MaximumWorldResources)
            {
                return H0ResourceDiagnostics.Exceeded(
                    "resource.world_resources_exceeded",
                    "The authored world exceeds the H0 resource-count limit.",
                    path,
                    "worldResources",
                    H0ResourceEnvelope.MaximumWorldResources,
                    resources,
                    "Reduce the authored resource count before retrying this canonical operation.");
            }

            for (var index = 0; index < state.Objects.Count; index++)
            {
                var count = state.Objects[index].References.Count;
                if (count > H0ResourceEnvelope.MaximumRelationsPerResource)
                {
                    return RelationError(path + "/objects/" + index + "/references", count);
                }
            }

            for (var index = 0; index < state.Extensions.Count; index++)
            {
                var extension = state.Extensions[index];
                if (extension.Dependencies.Count > H0ResourceEnvelope.MaximumRelationsPerResource)
                {
                    return RelationError(path + "/extensions/" + index + "/dependencies", extension.Dependencies.Count);
                }

                if (extension.PayloadLength > H0ResourceEnvelope.MaximumExtensionPayloadBytes)
                {
                    return H0ResourceDiagnostics.Exceeded(
                        "resource.extension_payload_exceeded",
                        "One extension payload exceeds the H0 per-resource byte limit.",
                        path + "/extensions/" + index + "/payload",
                        "extensionPayloadBytes",
                        H0ResourceEnvelope.MaximumExtensionPayloadBytes,
                        extension.PayloadLength,
                        "Reduce this extension payload before retrying the canonical operation.");
                }
            }

            canonicalBytes = CanonicalWorldStateCodec.Serialize(state).Length;
            if (canonicalBytes > H0ResourceEnvelope.MaximumCanonicalWorldBytes)
            {
                return H0ResourceDiagnostics.Exceeded(
                    "resource.world_bytes_exceeded",
                    "The authored world exceeds the H0 canonical-state byte limit.",
                    path,
                    "canonicalWorldBytes",
                    H0ResourceEnvelope.MaximumCanonicalWorldBytes,
                    canonicalBytes,
                    "Reduce authored state inside the H0 world envelope before retrying.");
            }

            return null;
        }

        public static StructuredError? ValidateEncodedSnapshotSize(string base64, string path)
        {
            if (base64 == null) throw new ArgumentNullException(nameof(base64));
            var maximumEncodedLength = checked(((H0ResourceEnvelope.MaximumCanonicalWorldBytes + 2) / 3) * 4);
            if (base64.Length <= maximumEncodedLength) return null;
            var estimatedBytes = (long)base64.Length / 4L * 3L;
            return H0ResourceDiagnostics.Exceeded(
                "resource.snapshot_bytes_exceeded",
                "The snapshot exceeds the H0 decoded canonical-state byte limit.",
                path,
                "snapshotStateBytes",
                H0ResourceEnvelope.MaximumCanonicalWorldBytes,
                estimatedBytes,
                "Use a snapshot inside the advertised H0 world envelope; canonical state was not replaced.");
        }

        private static StructuredError RelationError(string path, int observed)
        {
            return H0ResourceDiagnostics.Exceeded(
                "resource.relation_count_exceeded",
                "One authored resource exceeds the H0 relation-count limit.",
                path,
                "relationsPerResource",
                H0ResourceEnvelope.MaximumRelationsPerResource,
                observed,
                "Reduce the relations attached to this resource and retry.");
        }
    }
}
