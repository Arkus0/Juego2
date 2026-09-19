using System;
using System.Collections.Generic;

namespace Arkus.Harness.Protocol
{
    public sealed class PortableDataIssue
    {
        public PortableDataIssue(string code, string path, string message)
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
    /// Validates that canonical runtime values are representable as transport-neutral JSON data.
    /// SchemaNode.Any and open object properties remain flexible, but never admit CLR/engine objects.
    /// </summary>
    public static class PortableData
    {
        public static IReadOnlyList<PortableDataIssue> Validate(object? value)
        {
            var issues = new List<PortableDataIssue>();
            ValidateValue(value, "$", issues);
            return issues.AsReadOnly();
        }

        private static void ValidateValue(object? value, string path, IList<PortableDataIssue> issues)
        {
            if (value == null || value is string || value is bool || IsInteger(value) || value is decimal)
            {
                return;
            }

            if (value is double doubleValue)
            {
                if (double.IsNaN(doubleValue) || double.IsInfinity(doubleValue))
                {
                    AddNonPortable(path, issues, "Canonical JSON numbers must be finite.");
                }

                return;
            }

            if (value is float floatValue)
            {
                if (float.IsNaN(floatValue) || float.IsInfinity(floatValue))
                {
                    AddNonPortable(path, issues, "Canonical JSON numbers must be finite.");
                }

                return;
            }

            if (value is IReadOnlyDictionary<string, object?> map)
            {
                foreach (var pair in map)
                {
                    ValidateValue(pair.Value, path + "/" + EscapePathToken(pair.Key), issues);
                }

                return;
            }

            if (value is IReadOnlyList<object?> list)
            {
                for (var index = 0; index < list.Count; index++)
                {
                    ValidateValue(list[index], path + "/" + index, issues);
                }

                return;
            }

            AddNonPortable(
                path,
                issues,
                "Canonical contract data may contain only JSON-compatible null, scalar, object and array values.");
        }

        private static bool IsInteger(object value)
        {
            return value is sbyte || value is byte || value is short || value is ushort ||
                value is int || value is uint || value is long || value is ulong;
        }

        private static string EscapePathToken(string value)
        {
            return value.Replace("~", "~0").Replace("/", "~1");
        }

        private static void AddNonPortable(string path, IList<PortableDataIssue> issues, string message)
        {
            issues.Add(new PortableDataIssue("portable.non_json_value", path, message));
        }
    }
}
