using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Cli
{
    internal static class ReferenceTransportHost
    {
        internal const string ProtocolVersion = "arkus.reference.jsonl@1";
        internal const int MaximumFrameBytes = H0ResourceEnvelope.MaximumTransportFrameBytes;
        internal const int SuccessExitCode = 0;
        internal const int RequestFailureExitCode = 2;
        internal const int UsageFailureExitCode = 64;
        internal const int SoftwareFailureExitCode = 70;

        internal static int Run(string[] args, Stream input, Stream output, TextWriter diagnostics)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (output == null) throw new ArgumentNullException(nameof(output));
            if (diagnostics == null) throw new ArgumentNullException(nameof(diagnostics));

            var options = HostOptions.Parse(args);
            if (!options.Success)
            {
                diagnostics.WriteLine("arkus-host usage: " + options.Error);
                return UsageFailureExitCode;
            }

            Stream? ownedInput = null;
            try
            {
                if (options.InputPath != null)
                {
                    try
                    {
                        ownedInput = new FileStream(
                            options.InputPath,
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.Read);
                    }
                    catch (Exception exception) when (
                        exception is IOException || exception is UnauthorizedAccessException)
                    {
                        diagnostics.WriteLine("arkus-host input: " + exception.GetType().Name);
                        return UsageFailureExitCode;
                    }

                    input = ownedInput;
                }

                using (var cancellation = new CancellationTokenSource())
                using (var projection = ProductionHarnessHost.Create())
                {
                    if (options.Diagnostics)
                        diagnostics.WriteLine("arkus-host: ready mode=" + options.ModeToken);

                    ConsoleCancelEventHandler handler = (sender, eventArgs) =>
                    {
                        eventArgs.Cancel = true;
                        cancellation.Cancel();
                    };

                    Console.CancelKeyPress += handler;
                    try
                    {
                        return options.OneShot
                            ? RunOneShot(input, output, projection, cancellation.Token)
                            : RunStream(input, output, projection, cancellation.Token);
                    }
                    finally
                    {
                        Console.CancelKeyPress -= handler;
                    }
                }
            }
            finally
            {
                ownedInput?.Dispose();
            }
        }

        private static int RunStream(
            Stream input,
            Stream output,
            NeutralProjectionService projection,
            CancellationToken cancellationToken)
        {
            var reader = new BoundedJsonLineReader(input, MaximumFrameBytes);
            while (!cancellationToken.IsCancellationRequested)
            {
                var frame = reader.Read();
                if (frame.Kind == FrameReadKind.EndOfStream) return SuccessExitCode;
                WriteResponse(output, Execute(frame, projection, cancellationToken));
            }

            return SuccessExitCode;
        }

        private static int RunOneShot(
            Stream input,
            Stream output,
            NeutralProjectionService projection,
            CancellationToken cancellationToken)
        {
            var reader = new BoundedJsonLineReader(input, MaximumFrameBytes);
            var first = reader.Read();
            if (first.Kind == FrameReadKind.EndOfStream)
            {
                WriteResponse(output, TransportFailure(
                    null,
                    null,
                    "transport.truncated_frame",
                    "One-shot mode requires exactly one complete JSON request frame.",
                    "$",
                    false,
                    "Provide one UTF-8 JSON request followed by end of input."));
                return RequestFailureExitCode;
            }

            var second = reader.Read();
            if (second.Kind != FrameReadKind.EndOfStream)
            {
                WriteResponse(output, TransportFailure(
                    null,
                    null,
                    "transport.multiple_frames",
                    "One-shot mode accepts exactly one request frame and performs no dispatch when more are supplied.",
                    "$",
                    false,
                    "Use stream mode for multiple frames or provide one request only."));
                return RequestFailureExitCode;
            }

            var response = Execute(first, projection, cancellationToken);
            WriteResponse(output, response);
            return ResponseSucceeded(response) ? SuccessExitCode : RequestFailureExitCode;
        }

        private static IReadOnlyDictionary<string, object?> Execute(
            FrameReadResult frame,
            NeutralProjectionService projection,
            CancellationToken cancellationToken)
        {
            if (frame.Kind == FrameReadKind.Oversized)
            {
                return TransportFailure(
                    null,
                    null,
                    "resource.request_bytes_exceeded",
                    "The JSON Lines frame exceeded the H0 transport framing bound.",
                    "$",
                    false,
                    "Send one frame no larger than " + MaximumFrameBytes.ToString(CultureInfo.InvariantCulture) + " UTF-8 bytes.");
            }

            if (frame.Kind == FrameReadKind.InvalidUtf8)
            {
                return TransportFailure(
                    null,
                    null,
                    "transport.invalid_utf8",
                    "The frame is not valid strict UTF-8.",
                    "$",
                    false,
                    "Encode the complete request frame as UTF-8 without replacement bytes.");
            }

            if (!ReferenceFrameCodec.TryParseRequest(
                frame.Text ?? string.Empty,
                out var request,
                out var requestId,
                out var capability,
                out var parseError))
            {
                return TransportFailure(
                    requestId,
                    capability,
                    parseError!.MachineCode,
                    parseError.Message,
                    parseError.Path,
                    parseError.Retryable,
                    parseError.RepairHint ?? "Repair the request frame and retry.");
            }

            var outcome = projection.InvokeAsync(request!, cancellationToken).GetAwaiter().GetResult();
            return WrapResponse(outcome.ToData());
        }

        private static IReadOnlyDictionary<string, object?> TransportFailure(
            string? requestId,
            string? capability,
            string code,
            string message,
            string path,
            bool retryable,
            string repairHint)
        {
            var response = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["projectionVersion"] = NeutralProjectionRequest.ProjectionVersion,
                ["requestId"] = requestId,
                ["capability"] = capability,
                ["status"] = "error",
                ["failureKind"] = "transport",
                ["error"] = new StructuredError(
                    code,
                    message,
                    path,
                    new ReadOnlyDictionary<string, object?>(
                        new Dictionary<string, object?>(StringComparer.Ordinal)),
                    retryable,
                    repairHint).ToData()
            };
            return WrapResponse(new ReadOnlyDictionary<string, object?>(response));
        }

        private static IReadOnlyDictionary<string, object?> WrapResponse(
            IReadOnlyDictionary<string, object?> response)
        {
            return new ReadOnlyDictionary<string, object?>(
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["protocol"] = ProtocolVersion,
                    ["response"] = response
                });
        }

        private static bool ResponseSucceeded(IReadOnlyDictionary<string, object?> frame)
        {
            return frame.TryGetValue("response", out var responseValue) &&
                responseValue is IReadOnlyDictionary<string, object?> response &&
                response.TryGetValue("status", out var status) &&
                status is string text &&
                string.Equals(text, "success", StringComparison.Ordinal);
        }

        private static void WriteResponse(Stream output, IReadOnlyDictionary<string, object?> response)
        {
            var bytes = DeterministicJson.Serialize(response);
            output.Write(bytes, 0, bytes.Length);
            output.WriteByte((byte)'\n');
            output.Flush();
        }

        private sealed class HostOptions
        {
            private HostOptions(bool success, bool oneShot, string? inputPath, bool diagnostics, string? error)
            {
                Success = success;
                OneShot = oneShot;
                InputPath = inputPath;
                Diagnostics = diagnostics;
                Error = error;
            }

            public bool Success { get; }
            public bool OneShot { get; }
            public string? InputPath { get; }
            public bool Diagnostics { get; }
            public string? Error { get; }
            public string ModeToken => InputPath != null ? "file" : OneShot ? "once" : "stream";

            public static HostOptions Parse(IReadOnlyList<string> args)
            {
                var oneShot = false;
                var explicitStream = false;
                var diagnostics = false;
                string? inputPath = null;

                for (var index = 0; index < args.Count; index++)
                {
                    var current = args[index];
                    if (string.Equals(current, "--once", StringComparison.Ordinal))
                    {
                        if (oneShot) return Failure("--once may be specified only once");
                        oneShot = true;
                    }
                    else if (string.Equals(current, "--stream", StringComparison.Ordinal))
                    {
                        if (explicitStream) return Failure("--stream may be specified only once");
                        explicitStream = true;
                    }
                    else if (string.Equals(current, "--diagnostics", StringComparison.Ordinal))
                    {
                        if (diagnostics) return Failure("--diagnostics may be specified only once");
                        diagnostics = true;
                    }
                    else if (string.Equals(current, "--file", StringComparison.Ordinal))
                    {
                        if (inputPath != null || index + 1 >= args.Count)
                            return Failure("--file requires exactly one explicit path");
                        inputPath = args[++index];
                        if (string.IsNullOrWhiteSpace(inputPath))
                            return Failure("--file path may not be empty");
                    }
                    else
                    {
                        return Failure("unknown option '" + current + "'");
                    }
                }

                if (explicitStream && (oneShot || inputPath != null))
                    return Failure("--stream cannot be combined with --once or --file");
                if (inputPath != null && oneShot)
                    return Failure("--file is already one-shot and cannot be combined with --once");

                return new HostOptions(true, oneShot || inputPath != null, inputPath, diagnostics, null);
            }

            private static HostOptions Failure(string error) =>
                new HostOptions(false, false, null, false, error);
        }
    }

    internal enum FrameReadKind
    {
        EndOfStream = 0,
        Complete = 1,
        Oversized = 2,
        InvalidUtf8 = 3
    }

    internal sealed class FrameReadResult
    {
        public FrameReadResult(FrameReadKind kind, string? text)
        {
            Kind = kind;
            Text = text;
        }

        public FrameReadKind Kind { get; }
        public string? Text { get; }
    }

    internal sealed class BoundedJsonLineReader
    {
        private static readonly UTF8Encoding StrictUtf8 = new UTF8Encoding(false, true);
        private readonly Stream _input;
        private readonly int _maximumBytes;

        public BoundedJsonLineReader(Stream input, int maximumBytes)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
            if (maximumBytes <= 0) throw new ArgumentOutOfRangeException(nameof(maximumBytes));
            _maximumBytes = maximumBytes;
        }

        public FrameReadResult Read()
        {
            using (var buffer = new MemoryStream())
            {
                var oversized = false;
                var sawAny = false;
                while (true)
                {
                    var next = _input.ReadByte();
                    if (next < 0)
                    {
                        if (!sawAny) return new FrameReadResult(FrameReadKind.EndOfStream, null);
                        break;
                    }

                    sawAny = true;
                    if (next == '\n') break;
                    if (!oversized)
                    {
                        if (buffer.Length == _maximumBytes)
                        {
                            oversized = true;
                        }
                        else
                        {
                            buffer.WriteByte((byte)next);
                        }
                    }
                }

                if (oversized) return new FrameReadResult(FrameReadKind.Oversized, null);

                var bytes = buffer.ToArray();
                if (bytes.Length != 0 && bytes[bytes.Length - 1] == '\r')
                    Array.Resize(ref bytes, bytes.Length - 1);
                try
                {
                    return new FrameReadResult(FrameReadKind.Complete, StrictUtf8.GetString(bytes));
                }
                catch (DecoderFallbackException)
                {
                    return new FrameReadResult(FrameReadKind.InvalidUtf8, null);
                }
            }
        }
    }

    internal static class ReferenceFrameCodec
    {
        internal static bool TryParseRequest(
            string json,
            out NeutralProjectionRequest? request,
            out string? requestId,
            out string? capability,
            out StructuredError? error)
        {
            request = null;
            requestId = null;
            capability = null;
            error = null;

            JsonDocument document;
            try
            {
                document = JsonDocument.Parse(json, new JsonDocumentOptions
                {
                    AllowTrailingCommas = false,
                    CommentHandling = JsonCommentHandling.Disallow,
                    MaxDepth = H0ResourceEnvelope.MaximumPortableDepth + 4
                });
            }
            catch (JsonException)
            {
                var truncated = IsTruncatedJsonValue(json);
                error = Error(
                    truncated ? "transport.truncated_frame" : "transport.malformed_json",
                    truncated
                        ? "The frame ended before one complete JSON value was available."
                        : "The frame is not one strict JSON value.",
                    "$",
                    truncated
                        ? "Send the remainder of one complete JSON object in a single frame."
                        : "Send one complete JSON object per line with no comments or trailing commas.");
                return false;
            }

            using (document)
            {
                var root = document.RootElement;
                if (root.ValueKind != JsonValueKind.Object ||
                    !HasExactProperties(root, new[] { "protocol", "request" }, Array.Empty<string>()))
                {
                    error = Error(
                        "transport.invalid_frame",
                        "A request frame must contain exactly protocol and request.",
                        "$",
                        "Use the arkus.reference.jsonl@1 request frame documented for HK07A.");
                    return false;
                }

                if (!root.TryGetProperty("protocol", out var protocolElement) ||
                    protocolElement.ValueKind != JsonValueKind.String ||
                    !string.Equals(protocolElement.GetString(), ReferenceTransportHost.ProtocolVersion, StringComparison.Ordinal))
                {
                    error = Error(
                        "transport.unsupported_protocol",
                        "The reference transport protocol identifier is missing or unsupported.",
                        "$.protocol",
                        "Use protocol arkus.reference.jsonl@1.");
                    return false;
                }

                var requestElement = root.GetProperty("request");
                if (requestElement.ValueKind != JsonValueKind.Object)
                {
                    error = Error(
                        "transport.invalid_request_envelope",
                        "request must be an object.",
                        "$.request",
                        "Use the neutral request envelope documented for HK07A.");
                    return false;
                }

                TryReadString(requestElement, "requestId", out requestId);
                TryReadString(requestElement, "capability", out capability);

                if (!HasExactProperties(
                    requestElement,
                    new[] { "projectionVersion", "requestId", "capability", "acceptedVersions", "arguments" },
                    new[] { "timeoutMilliseconds" }))
                {
                    error = Error(
                        "transport.invalid_request_envelope",
                        "The neutral request envelope contains missing, duplicate or unknown fields.",
                        "$.request",
                        "Use only the fields declared by arkus.neutral-projection@1.");
                    return false;
                }

                if (!TryReadString(requestElement, "projectionVersion", out var projectionVersion) ||
                    !string.Equals(projectionVersion, NeutralProjectionRequest.ProjectionVersion, StringComparison.Ordinal))
                {
                    error = Error(
                        "projection.unsupported_version",
                        "The neutral projection version is missing or unsupported.",
                        "$.request.projectionVersion",
                        "Use arkus.neutral-projection@1.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(requestId) || string.IsNullOrWhiteSpace(capability))
                {
                    error = Error(
                        "projection.invalid_request",
                        "requestId and capability must be non-empty strings.",
                        "$.request",
                        "Provide a correlation ID and a capability discovered through system.describe.");
                    return false;
                }

                if (!TryReadVersionRange(requestElement.GetProperty("acceptedVersions"), out var versions))
                {
                    error = Error(
                        "projection.invalid_version_range",
                        "acceptedVersions must contain a non-negative major and ordered non-negative minor range.",
                        "$.request.acceptedVersions",
                        "Select a version range from system.describe.");
                    return false;
                }

                var argumentsElement = requestElement.GetProperty("arguments");
                if (argumentsElement.ValueKind != JsonValueKind.Object ||
                    !TryConvert(argumentsElement, out var argumentsValue) ||
                    !(argumentsValue is IReadOnlyDictionary<string, object?> arguments))
                {
                    error = Error(
                        "projection.invalid_arguments",
                        "arguments must be a portable JSON object with unique property names and finite numbers.",
                        "$.request.arguments",
                        "Conform arguments to the discovered canonical request schema.");
                    return false;
                }

                int? timeout = null;
                if (requestElement.TryGetProperty("timeoutMilliseconds", out var timeoutElement) &&
                    timeoutElement.ValueKind != JsonValueKind.Null)
                {
                    if (timeoutElement.ValueKind != JsonValueKind.Number ||
                        !timeoutElement.TryGetInt32(out var timeoutValue) ||
                        timeoutValue < 0)
                    {
                        error = Error(
                            "projection.invalid_timeout",
                            "timeoutMilliseconds must be null or a non-negative 32-bit integer.",
                            "$.request.timeoutMilliseconds",
                            "Omit the admission timeout or use a non-negative millisecond value.");
                        return false;
                    }

                    timeout = timeoutValue;
                }

                request = new NeutralProjectionRequest(
                    requestId!,
                    capability!,
                    versions!,
                    arguments,
                    timeout);
                return true;
            }
        }

        private static bool TryReadVersionRange(JsonElement element, out ContractVersionRange? range)
        {
            range = null;
            if (element.ValueKind != JsonValueKind.Object ||
                !HasExactProperties(element, new[] { "major", "minimumMinor", "maximumMinor" }, Array.Empty<string>()) ||
                !TryReadNonNegativeInt(element, "major", out var major) ||
                !TryReadNonNegativeInt(element, "minimumMinor", out var minimumMinor) ||
                !TryReadNonNegativeInt(element, "maximumMinor", out var maximumMinor) ||
                maximumMinor < minimumMinor)
            {
                return false;
            }

            range = new ContractVersionRange(major, minimumMinor, maximumMinor);
            return true;
        }

        private static bool IsTruncatedJsonValue(string json)
        {
            var reader = new Utf8JsonReader(
                Encoding.UTF8.GetBytes(json),
                false,
                new JsonReaderState(new JsonReaderOptions
                {
                    AllowTrailingCommas = false,
                    CommentHandling = JsonCommentHandling.Disallow,
                    MaxDepth = H0ResourceEnvelope.MaximumPortableDepth + 4
                }));
            try
            {
                if (!JsonDocument.TryParseValue(ref reader, out var value)) return true;
                value?.Dispose();
                return false;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        private static bool TryReadNonNegativeInt(JsonElement element, string name, out int value)
        {
            value = 0;
            return element.TryGetProperty(name, out var property) &&
                property.ValueKind == JsonValueKind.Number &&
                property.TryGetInt32(out value) &&
                value >= 0;
        }

        private static bool TryReadString(JsonElement element, string name, out string? value)
        {
            value = null;
            if (!element.TryGetProperty(name, out var property) || property.ValueKind != JsonValueKind.String)
                return false;
            value = property.GetString();
            return true;
        }

        private static bool HasExactProperties(
            JsonElement element,
            IEnumerable<string> required,
            IEnumerable<string> optional)
        {
            var requiredSet = new HashSet<string>(required, StringComparer.Ordinal);
            var allowed = new HashSet<string>(requiredSet, StringComparer.Ordinal);
            allowed.UnionWith(optional);
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var property in element.EnumerateObject())
            {
                if (!allowed.Contains(property.Name) || !seen.Add(property.Name)) return false;
            }

            return requiredSet.IsSubsetOf(seen);
        }

        private static bool TryConvert(JsonElement element, out object? value)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Null:
                    value = null;
                    return true;
                case JsonValueKind.String:
                    value = element.GetString();
                    return true;
                case JsonValueKind.True:
                    value = true;
                    return true;
                case JsonValueKind.False:
                    value = false;
                    return true;
                case JsonValueKind.Number:
                    if (element.TryGetInt64(out var integer))
                    {
                        value = integer;
                        return true;
                    }
                    if (element.TryGetDecimal(out var decimalValue))
                    {
                        value = decimalValue;
                        return true;
                    }
                    if (element.TryGetDouble(out var doubleValue) &&
                        !double.IsNaN(doubleValue) &&
                        !double.IsInfinity(doubleValue))
                    {
                        value = doubleValue;
                        return true;
                    }
                    value = null;
                    return false;
                case JsonValueKind.Array:
                    var items = new List<object?>();
                    foreach (var item in element.EnumerateArray())
                    {
                        if (!TryConvert(item, out var converted))
                        {
                            value = null;
                            return false;
                        }
                        items.Add(converted);
                    }
                    value = items.AsReadOnly();
                    return true;
                case JsonValueKind.Object:
                    var properties = new Dictionary<string, object?>(StringComparer.Ordinal);
                    foreach (var property in element.EnumerateObject())
                    {
                        if (properties.ContainsKey(property.Name) || !TryConvert(property.Value, out var converted))
                        {
                            value = null;
                            return false;
                        }
                        properties.Add(property.Name, converted);
                    }
                    value = new ReadOnlyDictionary<string, object?>(properties);
                    return true;
                default:
                    value = null;
                    return false;
            }
        }

        private static StructuredError Error(string code, string message, string path, string repairHint)
        {
            return new StructuredError(
                code,
                message,
                path,
                new ReadOnlyDictionary<string, object?>(
                    new Dictionary<string, object?>(StringComparer.Ordinal)),
                false,
                repairHint);
        }
    }

    internal static class DeterministicJson
    {
        internal static byte[] Serialize(object? value)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
                {
                    Indented = false,
                    SkipValidation = false
                }))
                {
                    WriteValue(writer, value);
                }
                return stream.ToArray();
            }
        }

        private static void WriteValue(Utf8JsonWriter writer, object? value)
        {
            if (value == null) writer.WriteNullValue();
            else if (value is string text) writer.WriteStringValue(text);
            else if (value is bool boolean) writer.WriteBooleanValue(boolean);
            else if (value is sbyte signedByte) writer.WriteNumberValue(signedByte);
            else if (value is byte unsignedByte) writer.WriteNumberValue(unsignedByte);
            else if (value is short signedShort) writer.WriteNumberValue(signedShort);
            else if (value is ushort unsignedShort) writer.WriteNumberValue(unsignedShort);
            else if (value is int signedInt) writer.WriteNumberValue(signedInt);
            else if (value is uint unsignedInt) writer.WriteNumberValue(unsignedInt);
            else if (value is long signedLong) writer.WriteNumberValue(signedLong);
            else if (value is ulong unsignedLong) writer.WriteNumberValue(unsignedLong);
            else if (value is decimal decimalValue) writer.WriteNumberValue(decimalValue);
            else if (value is double doubleValue) writer.WriteNumberValue(doubleValue);
            else if (value is float floatValue) writer.WriteNumberValue(floatValue);
            else if (value is IReadOnlyDictionary<string, object?> map)
            {
                writer.WriteStartObject();
                var keys = new List<string>(map.Keys);
                keys.Sort(StringComparer.Ordinal);
                foreach (var key in keys)
                {
                    writer.WritePropertyName(key);
                    WriteValue(writer, map[key]);
                }
                writer.WriteEndObject();
            }
            else if (value is IReadOnlyList<object?> list)
            {
                writer.WriteStartArray();
                for (var index = 0; index < list.Count; index++) WriteValue(writer, list[index]);
                writer.WriteEndArray();
            }
            else
            {
                throw new InvalidOperationException("Reference transport received non-portable projection data.");
            }
        }
    }
}
