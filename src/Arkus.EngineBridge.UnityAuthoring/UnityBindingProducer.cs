using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Arkus.Harness.Protocol;

namespace Arkus.EngineBridge.UnityAuthoring
{
    public sealed class UnityBindingException : Exception
    {
        public UnityBindingException(string machineCode, string path, string message)
            : base(message)
        {
            MachineCode = machineCode ?? throw new ArgumentNullException(nameof(machineCode));
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public string MachineCode { get; }
        public string Path { get; }
    }

    public static class UnityBindingProducer
    {
        public const string BindingSchemaId = "arkus.unity-binding@1";
        public const string ResultSchemaId = "arkus.unity-binding-result@1";
        public const string ExtensionOwner = "arkus.unity-binding";
        public const int ExtensionSchemaVersion = 1;
        public const string CoordinateConvention = "unity-local-left-handed-y-up-z-forward";
        public const int MaximumComponents = 64;
        public const int MaximumStringBytes = 1024;

        private static readonly UTF8Encoding StrictUtf8 = new UTF8Encoding(false, true);
        private static readonly byte[] Magic = { 0x41, 0x55, 0x42, 0x31 }; // AUB1

        public static IReadOnlyDictionary<string, object?> Compile(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var subjectId = RequiredIdentifier(request, "subjectId", "$.subjectId");
            var binding = RequiredMap(request, "binding", "$.binding");
            var normalized = NormalizeBinding(binding);
            var canonical = DeriveCanonicalDependencies(normalized);
            var catalogue = DeriveCatalogueDependencies(normalized);

            if (request.TryGetValue("expectedCanonicalDependencies", out var expectedCanonicalRaw))
                ValidateCanonicalAssertion(expectedCanonicalRaw, canonical, "$.expectedCanonicalDependencies");
            if (request.TryGetValue("expectedCatalogueDependencies", out var expectedCatalogueRaw))
                ValidateCatalogueAssertion(expectedCatalogueRaw, catalogue, "$.expectedCatalogueDependencies");

            var payload = Encode(normalized);
            return Result(subjectId, normalized, canonical, catalogue, payload);
        }

        public static IReadOnlyDictionary<string, object?> Decode(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var payload = RequiredBase64(request, "payloadBase64", "$.payloadBase64");
            var normalized = DecodePayload(payload);
            var canonical = DeriveCanonicalDependencies(normalized);
            var catalogue = DeriveCatalogueDependencies(normalized);
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = ResultSchemaId,
                ["binding"] = normalized,
                ["canonicalDependencies"] = CanonicalData(canonical),
                ["catalogueDependencies"] = CatalogueData(catalogue),
                ["payloadBase64"] = Convert.ToBase64String(payload)
            });
        }

        public static IReadOnlyDictionary<string, object?> Inspect(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var subjectId = RequiredIdentifier(request, "subjectId", "$.subjectId");
            var payload = RequiredBase64(request, "payloadBase64", "$.payloadBase64");
            var normalized = DecodePayload(payload);
            var canonical = DeriveCanonicalDependencies(normalized);
            var catalogue = DeriveCatalogueDependencies(normalized);

            if (!request.TryGetValue("dependencies", out var supplied))
                throw Error("unity.binding.missing-dependency-assertion", "$.dependencies", "Inspection requires the extension dependency metadata so it can be checked against structured truth.");
            ValidateCanonicalAssertion(supplied, canonical, "$.dependencies");

            return Result(subjectId, normalized, canonical, catalogue, payload);
        }

        public static IReadOnlyDictionary<string, object?> NormalizeBinding(IReadOnlyDictionary<string, object?> binding)
        {
            if (binding == null) throw new ArgumentNullException(nameof(binding));
            RequireExactKeys(binding, "$.binding", "schemaId", "targetSceneId", "source", "transform", "components");
            var schemaId = RequiredString(binding, "schemaId", "$.binding.schemaId");
            if (!string.Equals(schemaId, BindingSchemaId, StringComparison.Ordinal))
                throw Error("unity.binding.unsupported-schema", "$.binding.schemaId", "The binding schemaId is not supported by this producer version.");

            var targetSceneId = RequiredLogicalId(binding, "targetSceneId", "$.binding.targetSceneId");
            var source = NormalizeSource(RequiredMap(binding, "source", "$.binding.source"));
            var transform = NormalizeTransform(RequiredMap(binding, "transform", "$.binding.transform"));
            var components = NormalizeComponents(RequiredList(binding, "components", "$.binding.components"));

            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = BindingSchemaId,
                ["targetSceneId"] = targetSceneId,
                ["source"] = source,
                ["transform"] = transform,
                ["components"] = components
            });
        }

        private static IReadOnlyDictionary<string, object?> NormalizeSource(IReadOnlyDictionary<string, object?> source)
        {
            RequireExactKeys(source, "$.binding.source", "kind", "logicalId");
            var kind = RequiredString(source, "kind", "$.binding.source.kind");
            if (!string.Equals(kind, "asset", StringComparison.Ordinal) && !string.Equals(kind, "prefab", StringComparison.Ordinal))
                throw Error("unity.binding.invalid-source-kind", "$.binding.source.kind", "Source kind must be 'asset' or 'prefab'.");
            var logicalId = RequiredLogicalId(source, "logicalId", "$.binding.source.logicalId");
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = kind,
                ["logicalId"] = logicalId
            });
        }

        private static IReadOnlyDictionary<string, object?> NormalizeTransform(IReadOnlyDictionary<string, object?> transform)
        {
            RequireExactKeys(transform, "$.binding.transform", "coordinateConvention", "positionMm", "rotationMilliDegrees", "scalePpm");
            var convention = RequiredString(transform, "coordinateConvention", "$.binding.transform.coordinateConvention");
            if (!string.Equals(convention, CoordinateConvention, StringComparison.Ordinal))
                throw Error("unity.binding.coordinate-convention", "$.binding.transform.coordinateConvention", "The initial Unity binding requires the explicit normalized local coordinate convention.");
            var position = NormalizeVector(RequiredMap(transform, "positionMm", "$.binding.transform.positionMm"), "$.binding.transform.positionMm", false);
            var rotation = NormalizeVector(RequiredMap(transform, "rotationMilliDegrees", "$.binding.transform.rotationMilliDegrees"), "$.binding.transform.rotationMilliDegrees", false);
            var scale = NormalizeVector(RequiredMap(transform, "scalePpm", "$.binding.transform.scalePpm"), "$.binding.transform.scalePpm", true);
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["coordinateConvention"] = CoordinateConvention,
                ["positionMm"] = position,
                ["rotationMilliDegrees"] = rotation,
                ["scalePpm"] = scale
            });
        }

        private static IReadOnlyDictionary<string, object?> NormalizeVector(IReadOnlyDictionary<string, object?> value, string path, bool positive)
        {
            RequireExactKeys(value, path, "x", "y", "z");
            var x = RequiredInt64(value, "x", path + ".x");
            var y = RequiredInt64(value, "y", path + ".y");
            var z = RequiredInt64(value, "z", path + ".z");
            if (positive && (x <= 0 || y <= 0 || z <= 0))
                throw Error("unity.binding.invalid-scale", path, "Normalized scale parts must be positive parts-per-million integers.");
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["x"] = x,
                ["y"] = y,
                ["z"] = z
            });
        }

        private static IReadOnlyList<object?> NormalizeComponents(IReadOnlyList<object?> input)
        {
            if (input.Count > MaximumComponents)
                throw Error("unity.binding.component-limit", "$.binding.components", "The initial binding vocabulary has a bounded component-document count.");

            var normalized = new List<ComponentEntry>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var singletonKinds = new HashSet<string>(StringComparer.Ordinal);
            for (var index = 0; index < input.Count; index++)
            {
                if (!(input[index] is IReadOnlyDictionary<string, object?> component))
                    throw Error("unity.binding.invalid-component", "$.binding.components[" + index + "]", "Each component document must be a portable object.");
                var path = "$.binding.components[" + index + "]";
                var kind = RequiredString(component, "kind", path + ".kind");
                IReadOnlyDictionary<string, object?> data;
                string sortKey;
                if (string.Equals(kind, "canonical-link", StringComparison.Ordinal))
                {
                    RequireExactKeys(component, path, "kind", "relation", "targetObjectId");
                    var relation = RequiredIdentifier(component, "relation", path + ".relation");
                    var target = RequiredIdentifier(component, "targetObjectId", path + ".targetObjectId");
                    data = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["kind"] = kind,
                        ["relation"] = relation,
                        ["targetObjectId"] = target
                    });
                    sortKey = kind + "|" + relation + "|" + target;
                }
                else if (string.Equals(kind, "renderer", StringComparison.Ordinal))
                {
                    RequireExactKeys(component, path, "kind", "materialId");
                    if (!singletonKinds.Add(kind))
                        throw Error("unity.binding.duplicate-component-kind", path + ".kind", "The initial binding allows at most one renderer document.");
                    var material = RequiredLogicalId(component, "materialId", path + ".materialId");
                    data = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["kind"] = kind,
                        ["materialId"] = material
                    });
                    sortKey = kind + "|" + material;
                }
                else if (string.Equals(kind, "animator", StringComparison.Ordinal))
                {
                    RequireExactKeys(component, path, "kind", "clipId");
                    if (!singletonKinds.Add(kind))
                        throw Error("unity.binding.duplicate-component-kind", path + ".kind", "The initial binding allows at most one animator document.");
                    var clip = RequiredLogicalId(component, "clipId", path + ".clipId");
                    data = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["kind"] = kind,
                        ["clipId"] = clip
                    });
                    sortKey = kind + "|" + clip;
                }
                else
                {
                    throw Error("unity.binding.component-not-allowlisted", path + ".kind", "Component document kind is outside the initial allowlist.");
                }

                if (!seen.Add(sortKey))
                    throw Error("unity.binding.duplicate-component", path, "Duplicate normalized component documents are rejected rather than silently coalesced.");
                normalized.Add(new ComponentEntry(sortKey, data));
            }

            normalized.Sort((left, right) => StringComparer.Ordinal.Compare(left.SortKey, right.SortKey));
            var result = new List<object?>();
            foreach (var entry in normalized) result.Add(entry.Data);
            return result.AsReadOnly();
        }

        private static IReadOnlyList<CanonicalDependency> DeriveCanonicalDependencies(IReadOnlyDictionary<string, object?> normalized)
        {
            var result = new Dictionary<string, CanonicalDependency>(StringComparer.Ordinal);
            foreach (var raw in RequiredList(normalized, "components", "$.binding.components"))
            {
                var component = (IReadOnlyDictionary<string, object?>)raw!;
                if (!string.Equals((string)component["kind"]!, "canonical-link", StringComparison.Ordinal)) continue;
                var dependency = new CanonicalDependency((string)component["relation"]!, (string)component["targetObjectId"]!);
                result[dependency.Key] = dependency;
            }
            var list = new List<CanonicalDependency>(result.Values);
            list.Sort((left, right) => StringComparer.Ordinal.Compare(left.Key, right.Key));
            return list.AsReadOnly();
        }

        private static IReadOnlyList<CatalogueDependency> DeriveCatalogueDependencies(IReadOnlyDictionary<string, object?> normalized)
        {
            var result = new Dictionary<string, CatalogueDependency>(StringComparer.Ordinal);
            AddCatalogue(result, "scene", (string)normalized["targetSceneId"]!);
            var source = (IReadOnlyDictionary<string, object?>)normalized["source"]!;
            AddCatalogue(result, (string)source["kind"]!, (string)source["logicalId"]!);
            foreach (var raw in RequiredList(normalized, "components", "$.binding.components"))
            {
                var component = (IReadOnlyDictionary<string, object?>)raw!;
                var kind = (string)component["kind"]!;
                if (string.Equals(kind, "renderer", StringComparison.Ordinal))
                    AddCatalogue(result, "material", (string)component["materialId"]!);
                else if (string.Equals(kind, "animator", StringComparison.Ordinal))
                    AddCatalogue(result, "animation-clip", (string)component["clipId"]!);
            }
            var list = new List<CatalogueDependency>(result.Values);
            list.Sort((left, right) => StringComparer.Ordinal.Compare(left.Key, right.Key));
            return list.AsReadOnly();
        }

        private static void AddCatalogue(IDictionary<string, CatalogueDependency> result, string kind, string logicalId)
        {
            var dependency = new CatalogueDependency(kind, logicalId);
            result[dependency.Key] = dependency;
        }

        private static void ValidateCanonicalAssertion(object? raw, IReadOnlyList<CanonicalDependency> derived, string path)
        {
            if (!(raw is IReadOnlyList<object?> list))
                throw Error("unity.binding.invalid-dependency-assertion", path, "Canonical dependency assertion must be an array.");
            var supplied = new List<CanonicalDependency>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            for (var index = 0; index < list.Count; index++)
            {
                if (!(list[index] is IReadOnlyDictionary<string, object?> map))
                    throw Error("unity.binding.invalid-dependency-assertion", path + "[" + index + "]", "Canonical dependency entries must be objects.");
                RequireExactKeys(map, path + "[" + index + "]", "kind", "targetId");
                var dependency = new CanonicalDependency(
                    RequiredIdentifier(map, "kind", path + "[" + index + "].kind"),
                    RequiredIdentifier(map, "targetId", path + "[" + index + "].targetId"));
                if (!seen.Add(dependency.Key))
                    throw Error("unity.binding.duplicate-dependency-assertion", path + "[" + index + "]", "Caller dependency assertions may not contain duplicates.");
                supplied.Add(dependency);
            }
            supplied.Sort((left, right) => StringComparer.Ordinal.Compare(left.Key, right.Key));
            if (!EqualCanonical(supplied, derived))
                throw Error("unity.binding.canonical-dependency-mismatch", path, "Caller canonical dependency metadata does not exactly equal mechanically derived structured references.");
        }

        private static void ValidateCatalogueAssertion(object? raw, IReadOnlyList<CatalogueDependency> derived, string path)
        {
            if (!(raw is IReadOnlyList<object?> list))
                throw Error("unity.binding.invalid-catalogue-assertion", path, "Catalogue dependency assertion must be an array.");
            var supplied = new List<CatalogueDependency>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            for (var index = 0; index < list.Count; index++)
            {
                if (!(list[index] is IReadOnlyDictionary<string, object?> map))
                    throw Error("unity.binding.invalid-catalogue-assertion", path + "[" + index + "]", "Catalogue dependency entries must be objects.");
                RequireExactKeys(map, path + "[" + index + "]", "kind", "logicalId");
                var dependency = new CatalogueDependency(
                    RequiredCatalogueKind(map, "kind", path + "[" + index + "].kind"),
                    RequiredLogicalId(map, "logicalId", path + "[" + index + "].logicalId"));
                if (!seen.Add(dependency.Key))
                    throw Error("unity.binding.duplicate-catalogue-assertion", path + "[" + index + "]", "Caller catalogue dependency assertions may not contain duplicates.");
                supplied.Add(dependency);
            }
            supplied.Sort((left, right) => StringComparer.Ordinal.Compare(left.Key, right.Key));
            if (!EqualCatalogue(supplied, derived))
                throw Error("unity.binding.catalogue-dependency-mismatch", path, "Caller catalogue dependency metadata does not exactly equal mechanically derived structured references.");
        }

        private static bool EqualCanonical(IReadOnlyList<CanonicalDependency> left, IReadOnlyList<CanonicalDependency> right)
        {
            if (left.Count != right.Count) return false;
            for (var index = 0; index < left.Count; index++) if (!string.Equals(left[index].Key, right[index].Key, StringComparison.Ordinal)) return false;
            return true;
        }

        private static bool EqualCatalogue(IReadOnlyList<CatalogueDependency> left, IReadOnlyList<CatalogueDependency> right)
        {
            if (left.Count != right.Count) return false;
            for (var index = 0; index < left.Count; index++) if (!string.Equals(left[index].Key, right[index].Key, StringComparison.Ordinal)) return false;
            return true;
        }

        private static IReadOnlyDictionary<string, object?> Result(
            string subjectId,
            IReadOnlyDictionary<string, object?> normalized,
            IReadOnlyList<CanonicalDependency> canonical,
            IReadOnlyList<CatalogueDependency> catalogue,
            byte[] payload)
        {
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = ResultSchemaId,
                ["binding"] = normalized,
                ["canonicalDependencies"] = CanonicalData(canonical),
                ["catalogueDependencies"] = CatalogueData(catalogue),
                ["payloadBase64"] = Convert.ToBase64String(payload),
                ["extensionMutation"] = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "put-extension",
                    ["owner"] = ExtensionOwner,
                    ["schemaVersion"] = ExtensionSchemaVersion,
                    ["subjectId"] = subjectId,
                    ["dependencies"] = CanonicalData(canonical),
                    ["payloadBase64"] = Convert.ToBase64String(payload)
                })
            });
        }

        private static IReadOnlyList<object?> CanonicalData(IReadOnlyList<CanonicalDependency> dependencies)
        {
            var result = new List<object?>();
            foreach (var dependency in dependencies)
            {
                result.Add(ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = dependency.Kind,
                    ["targetId"] = dependency.TargetId
                }));
            }
            return result.AsReadOnly();
        }

        private static IReadOnlyList<object?> CatalogueData(IReadOnlyList<CatalogueDependency> dependencies)
        {
            var result = new List<object?>();
            foreach (var dependency in dependencies)
            {
                result.Add(ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = dependency.Kind,
                    ["logicalId"] = dependency.LogicalId
                }));
            }
            return result.AsReadOnly();
        }

        private static byte[] Encode(IReadOnlyDictionary<string, object?> normalized)
        {
            using (var stream = new MemoryStream())
            {
                stream.Write(Magic, 0, Magic.Length);
                WriteString(stream, BindingSchemaId);
                WriteString(stream, (string)normalized["targetSceneId"]!);
                var source = (IReadOnlyDictionary<string, object?>)normalized["source"]!;
                WriteString(stream, (string)source["kind"]!);
                WriteString(stream, (string)source["logicalId"]!);
                var transform = (IReadOnlyDictionary<string, object?>)normalized["transform"]!;
                WriteString(stream, (string)transform["coordinateConvention"]!);
                WriteVector(stream, (IReadOnlyDictionary<string, object?>)transform["positionMm"]!);
                WriteVector(stream, (IReadOnlyDictionary<string, object?>)transform["rotationMilliDegrees"]!);
                WriteVector(stream, (IReadOnlyDictionary<string, object?>)transform["scalePpm"]!);
                var components = RequiredList(normalized, "components", "$.binding.components");
                WriteInt32(stream, components.Count);
                foreach (var raw in components)
                {
                    var component = (IReadOnlyDictionary<string, object?>)raw!;
                    var kind = (string)component["kind"]!;
                    WriteString(stream, kind);
                    if (string.Equals(kind, "canonical-link", StringComparison.Ordinal))
                    {
                        WriteString(stream, (string)component["relation"]!);
                        WriteString(stream, (string)component["targetObjectId"]!);
                    }
                    else if (string.Equals(kind, "renderer", StringComparison.Ordinal)) WriteString(stream, (string)component["materialId"]!);
                    else if (string.Equals(kind, "animator", StringComparison.Ordinal)) WriteString(stream, (string)component["clipId"]!);
                    else throw new InvalidOperationException("Normalizer admitted an unknown component kind.");
                }
                return stream.ToArray();
            }
        }

        private static IReadOnlyDictionary<string, object?> DecodePayload(byte[] payload)
        {
            try
            {
                using (var stream = new MemoryStream(payload, false))
                {
                    for (var index = 0; index < Magic.Length; index++) if (ReadByte(stream) != Magic[index])
                        throw Error("unity.binding.invalid-payload", "$.payloadBase64", "Payload magic/version does not identify the admitted Unity binding codec.");
                    var schema = ReadString(stream);
                    if (!string.Equals(schema, BindingSchemaId, StringComparison.Ordinal))
                        throw Error("unity.binding.unsupported-schema", "$.payloadBase64", "Encoded binding schema is not supported.");
                    var targetScene = ReadString(stream);
                    var sourceKind = ReadString(stream);
                    var sourceId = ReadString(stream);
                    var convention = ReadString(stream);
                    var position = ReadVector(stream);
                    var rotation = ReadVector(stream);
                    var scale = ReadVector(stream);
                    var count = ReadInt32(stream);
                    if (count < 0 || count > MaximumComponents)
                        throw Error("unity.binding.invalid-payload", "$.payloadBase64", "Encoded component count is outside the admitted bound.");
                    var components = new List<object?>();
                    for (var index = 0; index < count; index++)
                    {
                        var kind = ReadString(stream);
                        if (string.Equals(kind, "canonical-link", StringComparison.Ordinal))
                            components.Add(ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal) { ["kind"] = kind, ["relation"] = ReadString(stream), ["targetObjectId"] = ReadString(stream) }));
                        else if (string.Equals(kind, "renderer", StringComparison.Ordinal))
                            components.Add(ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal) { ["kind"] = kind, ["materialId"] = ReadString(stream) }));
                        else if (string.Equals(kind, "animator", StringComparison.Ordinal))
                            components.Add(ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal) { ["kind"] = kind, ["clipId"] = ReadString(stream) }));
                        else throw Error("unity.binding.invalid-payload", "$.payloadBase64", "Encoded component kind is outside the admitted allowlist.");
                    }
                    if (stream.Position != stream.Length)
                        throw Error("unity.binding.invalid-payload", "$.payloadBase64", "Encoded binding contains trailing bytes outside the versioned document.");
                    var data = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["schemaId"] = schema,
                        ["targetSceneId"] = targetScene,
                        ["source"] = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal) { ["kind"] = sourceKind, ["logicalId"] = sourceId }),
                        ["transform"] = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                        {
                            ["coordinateConvention"] = convention,
                            ["positionMm"] = position,
                            ["rotationMilliDegrees"] = rotation,
                            ["scalePpm"] = scale
                        }),
                        ["components"] = components.AsReadOnly()
                    });
                    return NormalizeBinding(data);
                }
            }
            catch (UnityBindingException) { throw; }
            catch (Exception exception) when (exception is EndOfStreamException || exception is DecoderFallbackException || exception is OverflowException || exception is IOException)
            {
                throw Error("unity.binding.invalid-payload", "$.payloadBase64", "Payload is truncated or malformed for the admitted Unity binding codec.");
            }
        }

        private static void WriteVector(Stream stream, IReadOnlyDictionary<string, object?> value)
        {
            WriteInt64(stream, Convert.ToInt64(value["x"], System.Globalization.CultureInfo.InvariantCulture));
            WriteInt64(stream, Convert.ToInt64(value["y"], System.Globalization.CultureInfo.InvariantCulture));
            WriteInt64(stream, Convert.ToInt64(value["z"], System.Globalization.CultureInfo.InvariantCulture));
        }

        private static IReadOnlyDictionary<string, object?> ReadVector(Stream stream)
        {
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["x"] = ReadInt64(stream), ["y"] = ReadInt64(stream), ["z"] = ReadInt64(stream)
            });
        }

        private static void WriteString(Stream stream, string value)
        {
            var bytes = StrictUtf8.GetBytes(value);
            if (bytes.Length > MaximumStringBytes) throw Error("unity.binding.string-limit", "$", "Binding string exceeds the admitted portable bound.");
            WriteInt32(stream, bytes.Length);
            stream.Write(bytes, 0, bytes.Length);
        }

        private static string ReadString(Stream stream)
        {
            var length = ReadInt32(stream);
            if (length < 0 || length > MaximumStringBytes) throw Error("unity.binding.invalid-payload", "$.payloadBase64", "Encoded string length is outside the admitted bound.");
            var bytes = new byte[length];
            var offset = 0;
            while (offset < bytes.Length)
            {
                var read = stream.Read(bytes, offset, bytes.Length - offset);
                if (read <= 0) throw new EndOfStreamException();
                offset += read;
            }
            return StrictUtf8.GetString(bytes);
        }

        private static void WriteInt32(Stream stream, int value)
        {
            unchecked
            {
                stream.WriteByte((byte)(value >> 24)); stream.WriteByte((byte)(value >> 16)); stream.WriteByte((byte)(value >> 8)); stream.WriteByte((byte)value);
            }
        }

        private static int ReadInt32(Stream stream)
        {
            unchecked
            {
                return (ReadByte(stream) << 24) | (ReadByte(stream) << 16) | (ReadByte(stream) << 8) | ReadByte(stream);
            }
        }

        private static void WriteInt64(Stream stream, long value)
        {
            unchecked
            {
                for (var shift = 56; shift >= 0; shift -= 8) stream.WriteByte((byte)(value >> shift));
            }
        }

        private static long ReadInt64(Stream stream)
        {
            unchecked
            {
                ulong value = 0;
                for (var index = 0; index < 8; index++) value = (value << 8) | (uint)ReadByte(stream);
                return (long)value;
            }
        }

        private static int ReadByte(Stream stream)
        {
            var value = stream.ReadByte();
            if (value < 0) throw new EndOfStreamException();
            return value;
        }

        private static byte[] RequiredBase64(IReadOnlyDictionary<string, object?> map, string key, string path)
        {
            var value = RequiredString(map, key, path);
            try { return Convert.FromBase64String(value); }
            catch (FormatException) { throw Error("unity.binding.invalid-payload", path, "Payload must be canonical Base64."); }
        }

        private static IReadOnlyDictionary<string, object?> RequiredMap(IReadOnlyDictionary<string, object?> map, string key, string path)
        {
            if (!map.TryGetValue(key, out var raw) || !(raw is IReadOnlyDictionary<string, object?> value))
                throw Error("unity.binding.invalid-document", path, "Required object field is missing or not an object.");
            return value;
        }

        private static IReadOnlyList<object?> RequiredList(IReadOnlyDictionary<string, object?> map, string key, string path)
        {
            if (!map.TryGetValue(key, out var raw) || !(raw is IReadOnlyList<object?> value))
                throw Error("unity.binding.invalid-document", path, "Required array field is missing or not an array.");
            return value;
        }

        private static string RequiredString(IReadOnlyDictionary<string, object?> map, string key, string path)
        {
            if (!map.TryGetValue(key, out var raw) || !(raw is string value) || string.IsNullOrWhiteSpace(value))
                throw Error("unity.binding.invalid-document", path, "Required non-empty string field is missing or invalid.");
            if (StrictUtf8.GetByteCount(value) > MaximumStringBytes) throw Error("unity.binding.string-limit", path, "Binding string exceeds the admitted portable bound.");
            return value;
        }

        private static string RequiredIdentifier(IReadOnlyDictionary<string, object?> map, string key, string path)
        {
            var value = RequiredString(map, key, path);
            if (!CanonicalIdentityRules.IsCanonicalIdentifier(value)) throw Error("unity.binding.invalid-identifier", path, "Value must use the canonical portable identifier syntax.");
            return value;
        }

        private static string RequiredLogicalId(IReadOnlyDictionary<string, object?> map, string key, string path) => RequiredIdentifier(map, key, path);

        private static string RequiredCatalogueKind(IReadOnlyDictionary<string, object?> map, string key, string path)
        {
            var value = RequiredString(map, key, path);
            switch (value)
            {
                case "scene": case "asset": case "prefab": case "material": case "animation-clip": return value;
                default: throw Error("unity.binding.invalid-catalogue-kind", path, "Catalogue dependency kind is outside the admitted typed vocabulary.");
            }
        }

        private static long RequiredInt64(IReadOnlyDictionary<string, object?> map, string key, string path)
        {
            if (!map.TryGetValue(key, out var raw) || raw == null) throw Error("unity.binding.invalid-document", path, "Required integer field is missing.");
            try
            {
                if (raw is sbyte || raw is byte || raw is short || raw is ushort || raw is int || raw is uint || raw is long)
                    return Convert.ToInt64(raw, System.Globalization.CultureInfo.InvariantCulture);
                if (raw is ulong unsigned && unsigned <= long.MaxValue) return (long)unsigned;
            }
            catch (Exception exception) when (exception is OverflowException || exception is InvalidCastException || exception is FormatException) { }
            throw Error("unity.binding.invalid-document", path, "Transform values must be signed 64-bit integers.");
        }

        private static void RequireExactKeys(IReadOnlyDictionary<string, object?> map, string path, params string[] expected)
        {
            var allowed = new HashSet<string>(expected, StringComparer.Ordinal);
            foreach (var key in map.Keys) if (!allowed.Contains(key))
                throw Error("unity.binding.unknown-field", path + "." + key, "Field is outside the versioned Unity binding vocabulary.");
            foreach (var key in expected) if (!map.ContainsKey(key))
                throw Error("unity.binding.invalid-document", path + "." + key, "Required versioned binding field is missing.");
        }

        private static UnityBindingException Error(string code, string path, string message) => new UnityBindingException(code, path, message);

        private static IReadOnlyDictionary<string, object?> ReadOnly(Dictionary<string, object?> value) => new ReadOnlyDictionary<string, object?>(value);

        private sealed class ComponentEntry
        {
            public ComponentEntry(string sortKey, IReadOnlyDictionary<string, object?> data) { SortKey = sortKey; Data = data; }
            public string SortKey { get; }
            public IReadOnlyDictionary<string, object?> Data { get; }
        }

        private sealed class CanonicalDependency
        {
            public CanonicalDependency(string kind, string targetId) { Kind = kind; TargetId = targetId; Key = kind + "|" + targetId; }
            public string Kind { get; }
            public string TargetId { get; }
            public string Key { get; }
        }

        private sealed class CatalogueDependency
        {
            public CatalogueDependency(string kind, string logicalId) { Kind = kind; LogicalId = logicalId; Key = kind + "|" + logicalId; }
            public string Kind { get; }
            public string LogicalId { get; }
            public string Key { get; }
        }
    }
}
