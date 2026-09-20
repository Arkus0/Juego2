using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

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
        private static readonly UTF8Encoding StrictUtf8 = new UTF8Encoding(false, true);

        public static IReadOnlyList<PortableDataIssue> Validate(object? value)
        {
            var issues = new List<PortableDataIssue>();
            ValidateValue(value, "$", issues);
            return issues.AsReadOnly();
        }

        public static bool TryMeasure(
            object? value,
            int maximumDepth,
            long maximumBytes,
            out PortableDataMetrics metrics,
            out PortableDataIssue? issue)
        {
            if (maximumDepth <= 0) throw new ArgumentOutOfRangeException(nameof(maximumDepth));
            if (maximumBytes <= 0) throw new ArgumentOutOfRangeException(nameof(maximumBytes));

            var state = new MeasurementState(maximumDepth, maximumBytes);
            if (!MeasureValue(value, "$", 1, state, out issue))
            {
                metrics = new PortableDataMetrics(state.Bytes, state.MaximumDepth, state.Nodes);
                return false;
            }

            metrics = new PortableDataMetrics(state.Bytes, state.MaximumDepth, state.Nodes);
            return true;
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

        private static bool MeasureValue(
            object? value,
            string path,
            int depth,
            MeasurementState state,
            out PortableDataIssue? issue)
        {
            state.MaximumDepth = Math.Max(state.MaximumDepth, depth);
            state.Nodes++;
            if (depth > state.DepthLimit)
            {
                issue = new PortableDataIssue(
                    "portable.depth_limit",
                    path,
                    "Portable canonical data exceeds the configured nesting depth.");
                return false;
            }

            long bytes;
            if (value == null) bytes = 4;
            else if (value is string text)
            {
                try
                {
                    bytes = JsonStringBytes(text);
                }
                catch (EncoderFallbackException)
                {
                    issue = new PortableDataIssue(
                        "portable.invalid_utf16",
                        path,
                        "Canonical strings must contain valid Unicode scalar data.");
                    return false;
                }
            }
            else if (value is bool boolean) bytes = boolean ? 4 : 5;
            else if (TryNumberText(value, out var number)) bytes = StrictUtf8.GetByteCount(number);
            else if (value is IReadOnlyDictionary<string, object?> map)
            {
                if (!AddBytes(state, 2, path, out issue)) return false;
                var index = 0;
                foreach (var pair in map)
                {
                    try
                    {
                        bytes = JsonStringBytes(pair.Key) + 1 + (index == 0 ? 0 : 1);
                    }
                    catch (EncoderFallbackException)
                    {
                        issue = new PortableDataIssue(
                            "portable.invalid_utf16",
                            path,
                            "Canonical object keys must contain valid Unicode scalar data.");
                        return false;
                    }

                    if (!AddBytes(state, bytes, path, out issue) ||
                        !MeasureValue(pair.Value, path + "/" + EscapePathToken(pair.Key), depth + 1, state, out issue))
                    {
                        return false;
                    }
                    index++;
                }

                issue = null;
                return true;
            }
            else if (value is IReadOnlyList<object?> list)
            {
                if (!AddBytes(state, 2, path, out issue)) return false;
                for (var index = 0; index < list.Count; index++)
                {
                    if ((index != 0 && !AddBytes(state, 1, path, out issue)) ||
                        !MeasureValue(list[index], path + "/" + index.ToString(CultureInfo.InvariantCulture), depth + 1, state, out issue))
                    {
                        return false;
                    }
                }

                issue = null;
                return true;
            }
            else
            {
                issue = new PortableDataIssue(
                    "portable.non_json_value",
                    path,
                    "Canonical contract data may contain only JSON-compatible null, scalar, object and array values.");
                return false;
            }

            return AddBytes(state, bytes, path, out issue);
        }

        private static bool AddBytes(
            MeasurementState state,
            long bytes,
            string path,
            out PortableDataIssue? issue)
        {
            state.Bytes = checked(state.Bytes + bytes);
            if (state.Bytes > state.ByteLimit)
            {
                issue = new PortableDataIssue(
                    "portable.byte_limit",
                    path,
                    "Portable canonical data exceeds the configured byte envelope.");
                return false;
            }

            issue = null;
            return true;
        }

        private static long JsonStringBytes(string value)
        {
            var bytes = 2L + StrictUtf8.GetByteCount(value);
            for (var index = 0; index < value.Length; index++)
            {
                var character = value[index];
                if (character == '"' || character == '\\') bytes += 1;
                else if (character <= 0x1f) bytes += 5;
            }
            return bytes;
        }

        private static bool TryNumberText(object value, out string text)
        {
            switch (value)
            {
                case sbyte current: text = current.ToString(CultureInfo.InvariantCulture); return true;
                case byte current: text = current.ToString(CultureInfo.InvariantCulture); return true;
                case short current: text = current.ToString(CultureInfo.InvariantCulture); return true;
                case ushort current: text = current.ToString(CultureInfo.InvariantCulture); return true;
                case int current: text = current.ToString(CultureInfo.InvariantCulture); return true;
                case uint current: text = current.ToString(CultureInfo.InvariantCulture); return true;
                case long current: text = current.ToString(CultureInfo.InvariantCulture); return true;
                case ulong current: text = current.ToString(CultureInfo.InvariantCulture); return true;
                case decimal current: text = current.ToString(CultureInfo.InvariantCulture); return true;
                case double current when !double.IsNaN(current) && !double.IsInfinity(current):
                    text = current.ToString("R", CultureInfo.InvariantCulture); return true;
                case float current when !float.IsNaN(current) && !float.IsInfinity(current):
                    text = current.ToString("R", CultureInfo.InvariantCulture); return true;
                default: text = string.Empty; return false;
            }
        }

        private static string EscapePathToken(string value)
        {
            return value.Replace("~", "~0").Replace("/", "~1");
        }

        private static void AddNonPortable(string path, IList<PortableDataIssue> issues, string message)
        {
            issues.Add(new PortableDataIssue("portable.non_json_value", path, message));
        }

        private sealed class MeasurementState
        {
            public MeasurementState(int depthLimit, long byteLimit)
            {
                DepthLimit = depthLimit;
                ByteLimit = byteLimit;
            }

            public int DepthLimit { get; }
            public long ByteLimit { get; }
            public long Bytes { get; set; }
            public int MaximumDepth { get; set; }
            public long Nodes { get; set; }
        }
    }

    public sealed class PortableDataMetrics
    {
        public PortableDataMetrics(long utf8JsonBytes, int maximumDepth, long nodeCount)
        {
            Utf8JsonBytes = utf8JsonBytes;
            MaximumDepth = maximumDepth;
            NodeCount = nodeCount;
        }

        public long Utf8JsonBytes { get; }
        public int MaximumDepth { get; }
        public long NodeCount { get; }
    }
}
