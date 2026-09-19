using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Arkus.Game.World
{
    public static class CanonicalWorldStateCodec
    {
        private const string Header = "ARKUS_WORLD_STATE_V1";
        private static readonly UTF8Encoding StrictUtf8 = new UTF8Encoding(false, true);

        public static byte[] Serialize(WorldState state)
        {
            if (state is null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            WorldStateValidator.ValidateOrThrow(state);

            var builder = new StringBuilder();
            builder.Append(Header).Append('\n');
            builder.Append("schema\t").Append(state.SchemaVersion.ToString(CultureInfo.InvariantCulture)).Append('\n');
            builder.Append("world\t").Append(EncodeText(state.Id.Value)).Append('\n');
            builder.Append("revision\t").Append(state.Revision.ToString(CultureInfo.InvariantCulture)).Append('\n');

            var objects = new List<WorldObject>(state.Objects);
            objects.Sort(CompareObjects);
            for (var index = 0; index < objects.Count; index++)
            {
                var current = objects[index];
                builder.Append("object\t")
                    .Append(EncodeText(current.Id.Value)).Append('\t')
                    .Append(EncodeText(current.TypeId.Value)).Append('\t');

                if (current.ContainerId.HasValue)
                {
                    builder.Append(EncodeText(current.ContainerId.Value.Value));
                }
                else
                {
                    builder.Append('-');
                }

                builder.Append('\n');
            }

            var references = new List<ReferenceRecord>();
            for (var objectIndex = 0; objectIndex < state.Objects.Count; objectIndex++)
            {
                var source = state.Objects[objectIndex];
                for (var referenceIndex = 0; referenceIndex < source.References.Count; referenceIndex++)
                {
                    references.Add(new ReferenceRecord(source.Id, source.References[referenceIndex]));
                }
            }

            references.Sort(CompareReferences);
            for (var index = 0; index < references.Count; index++)
            {
                var current = references[index];
                builder.Append("reference\t")
                    .Append(EncodeText(current.SourceId.Value)).Append('\t')
                    .Append(EncodeText(current.Reference.Kind.Value)).Append('\t')
                    .Append(EncodeText(current.Reference.TargetId.Value)).Append('\n');
            }

            var extensions = new List<WorldExtensionData>(state.Extensions);
            extensions.Sort(CompareExtensions);
            for (var index = 0; index < extensions.Count; index++)
            {
                var current = extensions[index];
                builder.Append("extension\t")
                    .Append(EncodeText(current.Owner)).Append('\t')
                    .Append(current.SchemaVersion.ToString(CultureInfo.InvariantCulture)).Append('\t')
                    .Append(Convert.ToBase64String(current.PayloadBytes)).Append('\n');
            }

            builder.Append("end\n");
            return StrictUtf8.GetBytes(builder.ToString());
        }

        public static WorldState Deserialize(byte[] canonicalBytes)
        {
            if (canonicalBytes is null)
            {
                throw new ArgumentNullException(nameof(canonicalBytes));
            }

            string text;
            try
            {
                text = StrictUtf8.GetString(canonicalBytes);
            }
            catch (DecoderFallbackException exception)
            {
                throw new WorldStateException(
                    "world.invalid_utf8",
                    "Canonical world state must be valid UTF-8: " + exception.Message,
                    "$");
            }

            if (!text.EndsWith("\n", StringComparison.Ordinal))
            {
                throw Error("world.noncanonical_payload", "Canonical payload must end with a single LF-delimited record.", "$");
            }

            var lines = text.Split('\n');
            if (lines.Length < 6 || lines[lines.Length - 1].Length != 0 || !StringComparer.Ordinal.Equals(lines[0], Header))
            {
                throw Error("world.invalid_format", "Canonical world-state header or record framing is invalid.", "$");
            }

            int? schemaVersion = null;
            WorldId? worldId = null;
            long? revision = null;
            var parsedObjects = new List<ParsedObject>();
            var parsedReferences = new List<ParsedReference>();
            var extensions = new List<WorldExtensionData>();
            var sawEnd = false;

            for (var lineIndex = 1; lineIndex < lines.Length - 1; lineIndex++)
            {
                var line = lines[lineIndex];
                var fields = line.Split('\t');
                if (fields.Length == 0 || fields[0].Length == 0)
                {
                    throw Error("world.invalid_format", "Empty structural records are not allowed.", "$");
                }

                if (sawEnd)
                {
                    throw Error("world.invalid_format", "No records may follow the end marker.", "$");
                }

                switch (fields[0])
                {
                    case "schema":
                        RequireFieldCount(fields, 2, "$/schemaVersion");
                        if (schemaVersion.HasValue)
                        {
                            throw Error("world.invalid_format", "Schema record appears more than once.", "$/schemaVersion");
                        }

                        schemaVersion = ParseInt(fields[1], "$/schemaVersion");
                        if (schemaVersion.Value != WorldState.CurrentSchemaVersion)
                        {
                            throw Error("world.unsupported_schema", "Unsupported world schema version.", "$/schemaVersion");
                        }

                        break;

                    case "world":
                        RequireFieldCount(fields, 2, "$/id");
                        if (worldId.HasValue)
                        {
                            throw Error("world.invalid_format", "World identity record appears more than once.", "$/id");
                        }

                        worldId = new WorldId(DecodeText(fields[1], "$/id"));
                        break;

                    case "revision":
                        RequireFieldCount(fields, 2, "$/revision");
                        if (revision.HasValue)
                        {
                            throw Error("world.invalid_format", "Revision record appears more than once.", "$/revision");
                        }

                        revision = ParseLong(fields[1], "$/revision");
                        break;

                    case "object":
                        RequireFieldCount(fields, 4, "$/objects");
                        var objectId = new WorldObjectId(DecodeText(fields[1], "$/objects/id"));
                        var typeId = new WorldTypeId(DecodeText(fields[2], "$/objects/type"));
                        WorldObjectId? containerId = null;
                        if (!StringComparer.Ordinal.Equals(fields[3], "-"))
                        {
                            containerId = new WorldObjectId(DecodeText(fields[3], "$/objects/container"));
                        }

                        parsedObjects.Add(new ParsedObject(objectId, typeId, containerId));
                        break;

                    case "reference":
                        RequireFieldCount(fields, 4, "$/references");
                        parsedReferences.Add(new ParsedReference(
                            new WorldObjectId(DecodeText(fields[1], "$/references/source")),
                            new WorldReference(
                                new WorldReferenceKind(DecodeText(fields[2], "$/references/kind")),
                                new WorldObjectId(DecodeText(fields[3], "$/references/target")))));
                        break;

                    case "extension":
                        RequireFieldCount(fields, 4, "$/extensions");
                        extensions.Add(new WorldExtensionData(
                            DecodeText(fields[1], "$/extensions/owner"),
                            ParseInt(fields[2], "$/extensions/schemaVersion"),
                            DecodeBytes(fields[3], "$/extensions/payload")));
                        break;

                    case "end":
                        RequireFieldCount(fields, 1, "$");
                        sawEnd = true;
                        break;

                    default:
                        throw Error(
                            "world.unknown_record",
                            "Unknown structural record kind '" + fields[0] + "'.",
                            "$");
                }
            }

            if (!sawEnd || !schemaVersion.HasValue || !worldId.HasValue || !revision.HasValue)
            {
                throw Error("world.invalid_format", "Required canonical records are missing.", "$");
            }

            var knownIds = new HashSet<WorldObjectId>();
            for (var index = 0; index < parsedObjects.Count; index++)
            {
                knownIds.Add(parsedObjects[index].Id);
            }

            var referencesBySource = new Dictionary<WorldObjectId, List<WorldReference>>();
            for (var index = 0; index < parsedReferences.Count; index++)
            {
                var current = parsedReferences[index];
                if (!knownIds.Contains(current.SourceId))
                {
                    throw Error(
                        "world.dangling_reference",
                        "Reference source must resolve inside the same world state.",
                        "$/references/source");
                }

                if (!referencesBySource.TryGetValue(current.SourceId, out var list))
                {
                    list = new List<WorldReference>();
                    referencesBySource.Add(current.SourceId, list);
                }

                list.Add(current.Reference);
            }

            var objects = new List<WorldObject>();
            for (var index = 0; index < parsedObjects.Count; index++)
            {
                var parsed = parsedObjects[index];
                referencesBySource.TryGetValue(parsed.Id, out var referencesForObject);
                objects.Add(new WorldObject(
                    parsed.Id,
                    parsed.TypeId,
                    parsed.ContainerId,
                    referencesForObject ?? (IEnumerable<WorldReference>)Array.Empty<WorldReference>()));
            }

            var state = new WorldState(worldId.Value, revision.Value, objects, extensions, schemaVersion.Value);
            var reserialized = Serialize(state);
            if (!ByteArraysEqual(canonicalBytes, reserialized))
            {
                throw Error(
                    "world.noncanonical_payload",
                    "Payload is semantically readable but is not in the unique canonical byte form.",
                    "$");
            }

            return state;
        }

        public static string ComputeContentHash(WorldState state)
        {
            var bytes = Serialize(state);
            byte[] digest;
            using (var sha256 = SHA256.Create())
            {
                digest = sha256.ComputeHash(bytes);
            }

            var characters = new char[digest.Length * 2];
            const string alphabet = "0123456789abcdef";
            for (var index = 0; index < digest.Length; index++)
            {
                characters[index * 2] = alphabet[digest[index] >> 4];
                characters[index * 2 + 1] = alphabet[digest[index] & 0x0f];
            }

            return new string(characters);
        }

        private static int CompareObjects(WorldObject left, WorldObject right)
        {
            return left.Id.CompareTo(right.Id);
        }

        private static int CompareReferences(ReferenceRecord left, ReferenceRecord right)
        {
            var comparison = left.SourceId.CompareTo(right.SourceId);
            if (comparison != 0)
            {
                return comparison;
            }

            comparison = left.Reference.Kind.CompareTo(right.Reference.Kind);
            if (comparison != 0)
            {
                return comparison;
            }

            return left.Reference.TargetId.CompareTo(right.Reference.TargetId);
        }

        private static int CompareExtensions(WorldExtensionData left, WorldExtensionData right)
        {
            var comparison = StringComparer.Ordinal.Compare(left.Owner, right.Owner);
            if (comparison != 0)
            {
                return comparison;
            }

            return left.SchemaVersion.CompareTo(right.SchemaVersion);
        }

        private static string EncodeText(string value)
        {
            return Convert.ToBase64String(StrictUtf8.GetBytes(value));
        }

        private static string DecodeText(string encoded, string path)
        {
            try
            {
                return StrictUtf8.GetString(Convert.FromBase64String(encoded));
            }
            catch (FormatException)
            {
                throw Error("world.invalid_format", "Text field is not valid Base64.", path);
            }
            catch (DecoderFallbackException)
            {
                throw Error("world.invalid_utf8", "Decoded text field is not valid UTF-8.", path);
            }
        }

        private static byte[] DecodeBytes(string encoded, string path)
        {
            try
            {
                return Convert.FromBase64String(encoded);
            }
            catch (FormatException)
            {
                throw Error("world.invalid_format", "Binary extension payload is not valid Base64.", path);
            }
        }

        private static int ParseInt(string value, string path)
        {
            if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var parsed) || parsed <= 0)
            {
                throw Error("world.invalid_format", "Expected a positive invariant integer.", path);
            }

            return parsed;
        }

        private static long ParseLong(string value, string path)
        {
            if (!long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var parsed) || parsed < 0)
            {
                throw Error("world.invalid_revision", "Expected a non-negative invariant revision.", path);
            }

            return parsed;
        }

        private static void RequireFieldCount(string[] fields, int expected, string path)
        {
            if (fields.Length != expected)
            {
                throw Error("world.invalid_format", "Structural record has the wrong number of fields.", path);
            }
        }

        private static bool ByteArraysEqual(byte[] left, byte[] right)
        {
            if (left.Length != right.Length)
            {
                return false;
            }

            for (var index = 0; index < left.Length; index++)
            {
                if (left[index] != right[index])
                {
                    return false;
                }
            }

            return true;
        }

        private static WorldStateException Error(string code, string message, string path)
        {
            return new WorldStateException(code, message, path);
        }

        private readonly struct ReferenceRecord
        {
            public ReferenceRecord(WorldObjectId sourceId, WorldReference reference)
            {
                SourceId = sourceId;
                Reference = reference;
            }

            public WorldObjectId SourceId { get; }

            public WorldReference Reference { get; }
        }

        private sealed class ParsedObject
        {
            public ParsedObject(WorldObjectId id, WorldTypeId typeId, WorldObjectId? containerId)
            {
                Id = id;
                TypeId = typeId;
                ContainerId = containerId;
            }

            public WorldObjectId Id { get; }

            public WorldTypeId TypeId { get; }

            public WorldObjectId? ContainerId { get; }
        }

        private sealed class ParsedReference
        {
            public ParsedReference(WorldObjectId sourceId, WorldReference reference)
            {
                SourceId = sourceId;
                Reference = reference;
            }

            public WorldObjectId SourceId { get; }

            public WorldReference Reference { get; }
        }
    }
}
