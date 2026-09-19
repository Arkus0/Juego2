using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

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

    /// <summary>
    /// Small engine-neutral JSON Schema 2020-12 model used by the canonical Arkus contract.
    /// It intentionally models only the semantic subset Arkus currently owns; transports may
    /// project it but may not extend canonical meaning independently.
    /// </summary>
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
                data["type"] = GetJsonTypeName(ValueType);
            }

            if (Properties.Count > 0)
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

                data["required"] = required;
                data["additionalProperties"] = AdditionalPropertiesAllowed;
            }

            if (Items != null)
            {
                data["items"] = Items.ToData();
            }

            if (AllowedStringValues.Count > 0)
            {
                var values = new List<object?>();
                foreach (var value in AllowedStringValues)
                {
                    values.Add(value);
                }

                data["enum"] = values;
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

        internal void AppendFingerprint(StringBuilder builder)
        {
            builder.Append('(');
            builder.Append((int)ValueType);
            builder.Append('|');
            builder.Append(AdditionalPropertiesAllowed ? '1' : '0');
            builder.Append('|');
            builder.Append(Format ?? string.Empty);
            builder.Append('|');
            builder.Append(LogicalReferenceNamespace ?? string.Empty);

            var required = new List<string>(RequiredProperties);
            required.Sort(StringComparer.Ordinal);
            builder.Append("|R:");
            foreach (var value in required)
            {
                builder.Append(value);
                builder.Append(';');
            }

            var enumValues = new List<string>(AllowedStringValues);
            enumValues.Sort(StringComparer.Ordinal);
            builder.Append("|E:");
            foreach (var value in enumValues)
            {
                builder.Append(value);
                builder.Append(';');
            }

            var keys = new List<string>(Properties.Keys);
            keys.Sort(StringComparer.Ordinal);
            builder.Append("|P:");
            foreach (var key in keys)
            {
                builder.Append(key);
                builder.Append('=');
                Properties[key].AppendFingerprint(builder);
                builder.Append(';');
            }

            if (Items != null)
            {
                builder.Append("|I:");
                Items.AppendFingerprint(builder);
            }

            builder.Append(')');
        }

        internal void ValidateWellFormed(string path, IList<SchemaValidationIssue> issues)
        {
            if (ValueType != SchemaValueType.Object && (Properties.Count > 0 || RequiredProperties.Count > 0))
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

            if (ValueType != SchemaValueType.Array && Items != null)
            {
                issues.Add(new SchemaValidationIssue(
                    "schema.items_on_non_array",
                    path,
                    "Only array schemas may declare items."));
            }

            if (AllowedStringValues.Count > 0 && ValueType != SchemaValueType.String)
            {
                issues.Add(new SchemaValidationIssue(
                    "schema.enum_on_non_string",
                    path,
                    "The current canonical subset supports enum values only on strings."));
            }

            if (Format != null && !IsAllowedPortableFormat(Format))
            {
                issues.Add(new SchemaValidationIssue(
                    "schema.non_portable_format",
                    path + "/format",
                    "Schema format is not part of the portable canonical format set."));
            }

            if (string.Equals(Format, "arkus-logical-reference", StringComparison.Ordinal))
            {
                if (ValueType != SchemaValueType.String || string.IsNullOrWhiteSpace(LogicalReferenceNamespace))
                {
                    issues.Add(new SchemaValidationIssue(
                        "schema.invalid_logical_reference",
                        path,
                        "Arkus logical references must be strings with a non-empty logical reference namespace."));
                }
            }
            else if (LogicalReferenceNamespace != null)
            {
                issues.Add(new SchemaValidationIssue(
                    "schema.reference_namespace_without_reference_format",
                    path,
                    "A logical reference namespace is valid only for arkus-logical-reference strings."));
            }

            if (ValueType == SchemaValueType.Object)
            {
                foreach (var required in RequiredProperties)
                {
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
                    pair.Value.ValidateWellFormed(path + "/properties/" + pair.Key, issues);
                }
            }

            if (Items != null)
            {
                Items.ValidateWellFormed(path + "/items", issues);
            }
        }

        internal void ValidateValue(object? value, string path, IList<SchemaValidationIssue> issues)
        {
            if (ValueType == SchemaValueType.Any)
            {
                return;
            }

            switch (ValueType)
            {
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
                    issues.Add(new SchemaValidationIssue(
                        "schema.unknown_type",
                        path,
                        "Unknown schema value type."));
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
                    issues.Add(new SchemaValidationIssue(
                        "schema.required",
                        path + "/" + required,
                        "Required property is missing."));
                }
            }

            foreach (var pair in map)
            {
                if (Properties.TryGetValue(pair.Key, out var propertySchema))
                {
                    propertySchema.ValidateValue(pair.Value, path + "/" + pair.Key, issues);
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

            if (AllowedStringValues.Count > 0)
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
                issues.Add(new SchemaValidationIssue(
                    "schema.array_missing_items",
                    path,
                    "Array schema has no item definition."));
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

        private static void AddTypeMismatch(string path, string expected, IList<SchemaValidationIssue> issues)
        {
            issues.Add(new SchemaValidationIssue(
                "schema.type",
                path,
                "Value does not match expected type '" + expected + "'."));
        }

        private static bool IsAllowedPortableFormat(string format)
        {
            return string.Equals(format, "uuid", StringComparison.Ordinal) ||
                string.Equals(format, "date-time", StringComparison.Ordinal) ||
                string.Equals(format, "uri", StringComparison.Ordinal) ||
                string.Equals(format, "arkus-logical-reference", StringComparison.Ordinal);
        }

        private static string GetJsonTypeName(SchemaValueType valueType)
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

                copy.Add(pair.Key, pair.Value ?? throw new ArgumentException("Schema property values may not be null.", nameof(properties)));
            }

            return new ReadOnlyDictionary<string, SchemaNode>(copy);
        }

        private static IReadOnlyList<string> CopyStrings(IEnumerable<string>? values)
        {
            if (values == null)
            {
                return Array.Empty<string>();
            }

            var copy = new List<string>();
            foreach (var value in values)
            {
                if (value == null)
                {
                    throw new ArgumentException("String collections may not contain null values.", nameof(values));
                }

                copy.Add(value);
            }

            return copy.AsReadOnly();
        }
    }

    public sealed class JsonSchemaDocument
    {
        public const string Draft202012 = "https://json-schema.org/draft/2020-12/schema";

        public JsonSchemaDocument(SchemaNode root)
        {
            Root = root ?? throw new ArgumentNullException(nameof(root));
        }

        public string Dialect => Draft202012;
        public SchemaNode Root { get; }

        public IReadOnlyList<SchemaValidationIssue> ValidateDefinition()
        {
            var issues = new List<SchemaValidationIssue>();
            Root.ValidateWellFormed("$", issues);
            return issues.AsReadOnly();
        }

        public IReadOnlyList<SchemaValidationIssue> ValidateValue(object? value)
        {
            var issues = new List<SchemaValidationIssue>();
            Root.ValidateValue(value, "$", issues);
            return issues.AsReadOnly();
        }

        public IReadOnlyDictionary<string, object?> ToData()
        {
            var data = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["$schema"] = Dialect
            };

            foreach (var pair in Root.ToData())
            {
                data[pair.Key] = pair.Value;
            }

            return new ReadOnlyDictionary<string, object?>(data);
        }

        public string SemanticFingerprint()
        {
            var builder = new StringBuilder();
            builder.Append(Dialect);
            builder.Append('|');
            Root.AppendFingerprint(builder);
            return builder.ToString();
        }
    }
}
