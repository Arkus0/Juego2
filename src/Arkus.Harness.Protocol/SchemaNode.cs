using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Arkus.Harness.Protocol
{
    public enum SchemaValueType
    {
        Any = 0,
        Object = 1,
        String = 2,
        Integer = 3,
        Number = 4,
        Boolean = 5,
        Array = 6
    }

    public sealed class SchemaValidationIssue
    {
        public SchemaValidationIssue(string code, string path, string message)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public string Code { get; }
        public string Path { get; }
        public string Message { get; }
    }

    public sealed class SchemaNode
    {
        private static readonly IReadOnlyDictionary<string, SchemaNode> EmptyProperties =
            new ReadOnlyDictionary<string, SchemaNode>(new Dictionary<string, SchemaNode>(StringComparer.Ordinal));

        private SchemaNode(
            SchemaValueType valueType,
            IReadOnlyDictionary<string, SchemaNode>? properties,
            IEnumerable<string>? requiredProperties,
            bool additionalPropertiesAllowed,
            SchemaNode? items,
            IEnumerable<string>? allowedStringValues,
            string? format,
            string? logicalReferenceNamespace)
        {
            ValueType = valueType;
            Properties = CopyProperties(properties);
            RequiredProperties = CopyStrings(requiredProperties);
            AdditionalPropertiesAllowed = additionalPropertiesAllowed;
            Items = items;
            AllowedStringValues = CopyStrings(allowedStringValues);
            Format = format;
            LogicalReferenceNamespace = logicalReferenceNamespace;
        }

        public SchemaValueType ValueType { get; }
        public IReadOnlyDictionary<string, SchemaNode> Properties { get; }
        public IReadOnlyList<string> RequiredProperties { get; }
        public bool AdditionalPropertiesAllowed { get; }
        public SchemaNode? Items { get; }
        public IReadOnlyList<string> AllowedStringValues { get; }
        public string? Format { get; }
        public string? LogicalReferenceNamespace { get; }

        public static SchemaNode Any()
        {
            return new SchemaNode(SchemaValueType.Any, null, null, true, null, null, null, null);
        }

        public static SchemaNode Object(
            IReadOnlyDictionary<string, SchemaNode>? properties = null,
            IEnumerable<string>? requiredProperties = null,
            bool additionalPropertiesAllowed = false)
        {
            return new SchemaNode(
                SchemaValueType.Object,
                properties,
                requiredProperties,
                additionalPropertiesAllowed,
                null,
                null,
                null,
                null);
        }

        public static SchemaNode String(
            IEnumerable<string>? allowedValues = null,
            string? format = null,
            string? logicalReferenceNamespace = null)
        {
            return new SchemaNode(
                SchemaValueType.String,
                null,
                null,
                false,
                null,
                allowedValues,
                format,
                logicalReferenceNamespace);
        }

        public static SchemaNode Integer()
        {
            return new SchemaNode(SchemaValueType.Integer, null, null, false, null, null, null, null);
        }

        public static SchemaNode Number()
        {
            return new SchemaNode(SchemaValueType.Number, null, null, false, null, null, null, null);
        }

        public static SchemaNode Boolean()
        {
            return new SchemaNode(SchemaValueType.Boolean, null, null, false, null, null, null, null);
        }

        public static SchemaNode Array(SchemaNode items)
        {
            return new SchemaNode(
                SchemaValueType.Array,
                null,
                null,
                false,
                items ?? throw new ArgumentNullException(nameof(items)),
                null,
                null,
                null);
        }

        public IReadOnlyDictionary<string, object?> ToData()
        {
            var data = new Dictionary<string, object?>(StringComparer.Ordinal);
            if (ValueType != SchemaValueType.Any)
            {
                data["type"] = JsonTypeName(ValueType);
            }

            if (Properties.Count != 0)
            {
                var properties = new Dictionary<string, object?>(StringComparer.Ordinal);
                foreach (var pair in Properties)
                {
                    properties[pair.Key] = pair.Value.ToData();
                }

                data["properties"] = properties;
            }

            if (ValueType == SchemaValueType.Object)
            {
                var required = new List<object?>();
                foreach (var property in RequiredProperties)
                {
                    required.Add(property);
                }

                data["required"] = required.AsReadOnly();
                data["additionalProperties"] = AdditionalPropertiesAllowed;
            }

            if (Items != null)
            {
                data["items"] = Items.ToData();
            }

            if (AllowedStringValues.Count != 0)
            {
                var values = new List<object?>();
                foreach (var value in AllowedStringValues)
                {
                    values.Add(value);
                }

                data["enum"] = values.AsReadOnly();
            }

            if (Format != null)
            {
                data["format"] = Format;
            }

            if (LogicalReferenceNamespace != null)
            {
                data["x-arkus-reference-namespace"] = LogicalReferenceNamespace;
            }

            return new ReadOnlyDictionary<string, object?>(data);
        }

        internal void AppendFingerprint(SemanticFingerprintWriter writer)
        {
            writer.WriteInt((int)ValueType);
            writer.WriteBool(AdditionalPropertiesAllowed);
            writer.WriteString(Format);
            writer.WriteString(LogicalReferenceNamespace);
            writer.WriteSet(RequiredProperties);
            writer.WriteSet(AllowedStringValues);

            var propertyNames = new List<string>(Properties.Keys);
            propertyNames.Sort(StringComparer.Ordinal);
            writer.WriteInt(propertyNames.Count);
            foreach (var propertyName in propertyNames)
            {
                writer.WriteString(propertyName);
                Properties[propertyName].AppendFingerprint(writer);
            }

            writer.WriteBool(Items != null);
            if (Items != null)
            {
                Items.AppendFingerprint(writer);
            }
        }

        internal void ValidateDefinition(string path, IList<SchemaValidationIssue> issues)
        {
            if (ValueType != SchemaValueType.Object && (Properties.Count != 0 || RequiredProperties.Count != 0))
            {
                issues.Add(new SchemaValidationIssue(
                    "schema.properties_on_non_object",
                    path,
                    "Only object schemas may declare properties or required members."));
            }

            if (ValueType == SchemaValueType.Array && Items == null)
            {
                issues.Add(new SchemaValidationIssue(
                    "schema.array_missing_items",
                    path,
                    "Array schemas must declare an item schema."));
            }
            else if (ValueType != SchemaValueType.Array && Items != null)
            {
                issues.Add(new SchemaValidationIssue(
                    "schema.items_on_non_array",
                    path,
                    "Only array schemas may declare items."));
            }

            if (AllowedStringValues.Count != 0 && ValueType != SchemaValueType.String)
            {
                issues.Add(new SchemaValidationIssue(
                    "schema.enum_on_non_string",
                    path,
                    "The canonical subset supports enum values only on string schemas."));
            }

            if (Format != null && !IsPortableFormat(Format))
            {
                issues.Add(new SchemaValidationIssue(
                    "schema.non_portable_format",
                    path + "/format",
                    "Schema format is outside the portable Arkus canonical format set."));
            }

            if (string.Equals(Format, "arkus-logical-reference", StringComparison.Ordinal))
            {
                if (ValueType != SchemaValueType.String || string.IsNullOrWhiteSpace(LogicalReferenceNamespace))
                {
                    issues.Add(new SchemaValidationIssue(
                        "schema.invalid_logical_reference",
                        path,
                        "Logical references must be strings with a non-empty Arkus logical namespace."));
                }
            }
            else if (LogicalReferenceNamespace != null)
            {
                issues.Add(new SchemaValidationIssue(
                    "schema.reference_namespace_without_reference_format",
                    path,
                    "A logical reference namespace requires the arkus-logical-reference format."));
            }

            if (ValueType == SchemaValueType.Object)
            {
                var seenRequired = new HashSet<string>(StringComparer.Ordinal);
                foreach (var required in RequiredProperties)
                {
                    if (!seenRequired.Add(required))
                    {
                        issues.Add(new SchemaValidationIssue(
                            "schema.duplicate_required",
                            path + "/required",
                            "Required property names may not be duplicated."));
                    }

                    if (!Properties.ContainsKey(required))
                    {
                        issues.Add(new SchemaValidationIssue(
                            "schema.required_property_missing_definition",
                            path + "/required",
                            "Required property '" + required + "' has no schema definition."));
                    }
                }

                foreach (var pair in Properties)
                {
                    pair.Value.ValidateDefinition(path + "/properties/" + pair.Key, issues);
                }
            }

            if (Items != null)
            {
                Items.ValidateDefinition(path + "/items", issues);
            }
        }

        internal void ValidateValue(object? value, string path, IList<SchemaValidationIssue> issues)
        {
            switch (ValueType)
            {
                case SchemaValueType.Any:
                    return;
                case SchemaValueType.Object:
                    ValidateObject(value, path, issues);
                    return;
                case SchemaValueType.String:
                    ValidateString(value, path, issues);
                    return;
                case SchemaValueType.Integer:
                    if (!IsInteger(value))
                    {
                        AddTypeMismatch(path, "integer", issues);
                    }
                    return;
                case SchemaValueType.Number:
                    if (!IsNumber(value))
                    {
                        AddTypeMismatch(path, "number", issues);
                    }
                    return;
                case SchemaValueType.Boolean:
                    if (!(value is bool))
                    {
                        AddTypeMismatch(path, "boolean", issues);
                    }
                    return;
                case SchemaValueType.Array:
                    ValidateArray(value, path, issues);
                    return;
                default:
                    issues.Add(new SchemaValidationIssue("schema.unknown_type", path, "Unknown schema value type."));
                    return;
            }
        }

        private void ValidateObject(object? value, string path, IList<SchemaValidationIssue> issues)
        {
            if (!(value is IReadOnlyDictionary<string, object?> map))
            {
                AddTypeMismatch(path, "object", issues);
                return;
            }

            foreach (var required in RequiredProperties)
            {
                if (!map.ContainsKey(required))
                {
                    issues.Add(new SchemaValidationIssue("schema.required", path + "/" + required, "Required property is missing."));
                }
            }

            foreach (var pair in map)
            {
                if (Properties.TryGetValue(pair.Key, out var schema))
                {
                    schema.ValidateValue(pair.Value, path + "/" + pair.Key, issues);
                }
                else if (!AdditionalPropertiesAllowed)
                {
                    issues.Add(new SchemaValidationIssue(
                        "schema.additional_property",
                        path + "/" + pair.Key,
                        "Additional property is not allowed by the canonical schema."));
                }
            }
        }

        private void ValidateString(object? value, string path, IList<SchemaValidationIssue> issues)
        {
            if (!(value is string text))
            {
                AddTypeMismatch(path, "string", issues);
                return;
            }

            if (AllowedStringValues.Count != 0)
            {
                var found = false;
                foreach (var allowed in AllowedStringValues)
                {
                    if (string.Equals(allowed, text, StringComparison.Ordinal))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    issues.Add(new SchemaValidationIssue(
                        "schema.enum",
                        path,
                        "String value is outside the allowed canonical enum set."));
                }
            }

            if (string.Equals(Format, "arkus-logical-reference", StringComparison.Ordinal) && text.Length == 0)
            {
                issues.Add(new SchemaValidationIssue(
                    "schema.logical_reference_empty",
                    path,
                    "Logical reference values may not be empty."));
            }
        }

        private void ValidateArray(object? value, string path, IList<SchemaValidationIssue> issues)
        {
            if (!(value is IReadOnlyList<object?> list))
            {
                AddTypeMismatch(path, "array", issues);
                return;
            }

            if (Items == null)
            {
                issues.Add(new SchemaValidationIssue("schema.array_missing_items", path, "Array schema has no item definition."));
                return;
            }

            for (var index = 0; index < list.Count; index++)
            {
                Items.ValidateValue(list[index], path + "/" + index.ToString(CultureInfo.InvariantCulture), issues);
            }
        }

        private static bool IsInteger(object? value)
        {
            return value is sbyte || value is byte || value is short || value is ushort ||
                value is int || value is uint || value is long || value is ulong;
        }

        private static bool IsNumber(object? value)
        {
            return IsInteger(value) || value is float || value is double || value is decimal;
        }

        private static bool IsPortableFormat(string format)
        {
            return string.Equals(format, "uuid", StringComparison.Ordinal) ||
                string.Equals(format, "date-time", StringComparison.Ordinal) ||
                string.Equals(format, "uri", StringComparison.Ordinal) ||
                string.Equals(format, "arkus-logical-reference", StringComparison.Ordinal);
        }

        private static void AddTypeMismatch(string path, string expected, IList<SchemaValidationIssue> issues)
        {
            issues.Add(new SchemaValidationIssue(
                "schema.type",
                path,
                "Value does not match expected type '" + expected + "'."));
        }

        private static string JsonTypeName(SchemaValueType valueType)
        {
            switch (valueType)
            {
                case SchemaValueType.Object:
                    return "object";
                case SchemaValueType.String:
                    return "string";
                case SchemaValueType.Integer:
                    return "integer";
                case SchemaValueType.Number:
                    return "number";
                case SchemaValueType.Boolean:
                    return "boolean";
                case SchemaValueType.Array:
                    return "array";
                default:
                    throw new InvalidOperationException("Any schemas intentionally omit the JSON Schema type keyword.");
            }
        }

        private static IReadOnlyDictionary<string, SchemaNode> CopyProperties(
            IReadOnlyDictionary<string, SchemaNode>? properties)
        {
            if (properties == null || properties.Count == 0)
            {
                return EmptyProperties;
            }

            var copy = new Dictionary<string, SchemaNode>(StringComparer.Ordinal);
            foreach (var pair in properties)
            {
                if (string.IsNullOrWhiteSpace(pair.Key))
                {
                    throw new ArgumentException("Schema property names may not be empty.", nameof(properties));
                }

                copy.Add(
                    pair.Key,
                    pair.Value ?? throw new ArgumentException("Schema property values may not be null.", nameof(properties)));
            }

            return new ReadOnlyDictionary<string, SchemaNode>(copy);
        }

        private static IReadOnlyList<string> CopyStrings(IEnumerable<string>? values)
        {
            if (values == null)
            {
                return System.Array.Empty<string>();
            }

            var copy = new List<string>();
            foreach (var value in values)
            {
                copy.Add(value ?? throw new ArgumentException("String collections may not contain null values.", nameof(values)));
            }

            return copy.AsReadOnly();
        }
    }
}
