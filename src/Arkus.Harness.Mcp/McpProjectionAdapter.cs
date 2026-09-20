using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace Arkus.Harness.Mcp
{
    /// <summary>
    /// MCP framing over the accepted neutral projection. This type never owns canonical capability identity
    /// or dispatch; every advertised tool is rebuilt from NeutralProjectionService.Capabilities and every
    /// call enters the same NeutralProjectionService.InvokeAsync authority used by sibling transports.
    /// </summary>
    public sealed class McpProjectionAdapter
    {
        public const string TransportId = "arkus.mcp.stdio@1";
        public const string NeutralProjectionVersion = "arkus.neutral-projection@1";
        public const string CanonicalDefinitionMetaKey = "dev.arkus/canonicalDefinition";
        public const string CanonicalKeyMetaKey = "dev.arkus/canonicalKey";
        public const string NeutralProjectionMetaKey = "dev.arkus/neutralProjection";
        public const string TimeoutMetaKey = "dev.arkus/timeoutMilliseconds";

        private readonly NeutralProjectionService _projection;

        public McpProjectionAdapter(NeutralProjectionService projection)
        {
            _projection = projection ?? throw new ArgumentNullException(nameof(projection));
        }

        public IReadOnlyList<McpProjectedCapability> DescribeCapabilities()
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
            var projected = new List<McpProjectedCapability>();
            var definitions = _projection.Capabilities
                .OrderBy(value => value.Key.Name, StringComparer.Ordinal)
                .ThenBy(value => value.Key.Version)
                .ToArray();

            for (var canonicalOrdinal = 0; canonicalOrdinal < definitions.Length; canonicalOrdinal++)
            {
                var definition = definitions[canonicalOrdinal];
                var toolName = McpToolNameCodec.Encode(definition.Key, canonicalOrdinal);
                if (!names.Add(toolName))
                {
                    throw new InvalidOperationException("MCP tool-name projection collision for canonical capability '" + definition.Key + "'.");
                }

                projected.Add(new McpProjectedCapability(toolName, definition));
            }

            return projected;
        }

        public McpServerOptions CreateServerOptions()
        {
            var handlers = new McpServerHandlers
            {
                ListToolsHandler = ListToolsAsync,
                CallToolHandler = CallToolAsync
            };

            return new McpServerOptions
            {
                ServerInfo = new Implementation
                {
                    Name = "arkus.harness.mcp",
                    Title = "Arkus Harness MCP projection",
                    Version = "1.0.0",
                    Description = "MCP stdio projection of the canonical Arkus neutral projection."
                },
                ProtocolVersion = "2025-11-25",
                Capabilities = new ServerCapabilities
                {
                    Tools = new ToolsCapability { ListChanged = false }
                },
                Handlers = handlers
            };
        }

        private ValueTask<ListToolsResult> ListToolsAsync(
            RequestContext<ListToolsRequestParams> request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var tools = DescribeCapabilities().Select(ToTool).ToArray();
            return new ValueTask<ListToolsResult>(new ListToolsResult { Tools = tools });
        }

        private async ValueTask<CallToolResult> CallToolAsync(
            RequestContext<CallToolRequestParams> request,
            CancellationToken cancellationToken)
        {
            var parameters = request.Params ?? throw new McpException("MCP tools/call requires parameters.");
            var arguments = new Dictionary<string, object?>(StringComparer.Ordinal);
            if (parameters.Arguments != null)
            {
                foreach (var pair in parameters.Arguments)
                {
                    arguments.Add(pair.Key, ToPortableValue(pair.Value));
                }
            }

            var timeoutMilliseconds = ReadTimeoutMilliseconds(parameters.Meta);
            var jsonRpcId = request.JsonRpcRequest.Id.ToString();
            var outcome = await InvokeProjectedAsync(
                parameters.Name,
                arguments,
                "mcp:" + jsonRpcId,
                timeoutMilliseconds,
                cancellationToken).ConfigureAwait(false);
            var outcomeData = outcome.ToData();
            var structured = JsonSerializer.SerializeToElement(outcomeData);
            var text = JsonSerializer.Serialize(outcomeData);
            return new CallToolResult
            {
                IsError = !outcome.Success,
                StructuredContent = structured,
                Content = new ContentBlock[]
                {
                    new TextContentBlock { Text = text }
                }
            };
        }

        internal Task<NeutralProjectionOutcome> InvokeProjectedAsync(
            string toolName,
            IReadOnlyDictionary<string, object?> arguments,
            string requestId,
            int? timeoutMilliseconds,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(toolName)) throw new ArgumentException("MCP tool name is required.", nameof(toolName));
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));
            if (string.IsNullOrWhiteSpace(requestId)) throw new ArgumentException("Neutral request ID is required.", nameof(requestId));

            var descriptor = DescribeCapabilities()
                .FirstOrDefault(value => string.Equals(value.ToolName, toolName, StringComparison.Ordinal));
            if (descriptor == null)
            {
                throw new McpException("Unknown MCP tool name. Rediscover tools from this server before invocation.");
            }

            var neutralRequest = new NeutralProjectionRequest(
                requestId,
                descriptor.Definition.Key.Name,
                ContractVersionRange.Exact(descriptor.Definition.Key.Version),
                arguments,
                timeoutMilliseconds);
            return _projection.InvokeAsync(neutralRequest, cancellationToken);
        }

        private static Tool ToTool(McpProjectedCapability projected)
        {
            var definition = projected.Definition;
            var inputSchema = definition.RequestSchema == null
                ? JsonSerializer.SerializeToElement(new Dictionary<string, object?>(StringComparer.Ordinal))
                : JsonSerializer.SerializeToElement(definition.RequestSchema.ToData());
            return new Tool
            {
                Name = projected.ToolName,
                Title = definition.Key.ToString(),
                Description = "Canonical Arkus capability " + definition.Key + " projected through " + NeutralProjectionVersion + ".",
                InputSchema = inputSchema,
                Meta = new JsonObject
                {
                    [CanonicalKeyMetaKey] = definition.Key.ToString(),
                    [NeutralProjectionMetaKey] = NeutralProjectionVersion,
                    [CanonicalDefinitionMetaKey] = JsonSerializer.SerializeToNode(definition.ToData())
                }
            };
        }

        private static int? ReadTimeoutMilliseconds(JsonObject? meta)
        {
            if (meta == null || !meta.TryGetPropertyValue(TimeoutMetaKey, out var node) || node == null)
            {
                return null;
            }

            if (node is not JsonValue value || !value.TryGetValue<int>(out var timeoutMilliseconds) || timeoutMilliseconds < 0)
            {
                throw new McpException(TimeoutMetaKey + " must be a non-negative 32-bit integer when present.");
            }

            return timeoutMilliseconds;
        }

        internal static object? ToPortableValue(JsonElement value)
        {
            switch (value.ValueKind)
            {
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return null;
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.String:
                    return value.GetString();
                case JsonValueKind.Number:
                    if (value.TryGetInt64(out var integer)) return integer;
                    if (value.TryGetDecimal(out var decimalValue)) return decimalValue;
                    return value.GetDouble();
                case JsonValueKind.Array:
                    return value.EnumerateArray().Select(ToPortableValue).ToArray();
                case JsonValueKind.Object:
                    var result = new Dictionary<string, object?>(StringComparer.Ordinal);
                    foreach (var property in value.EnumerateObject())
                    {
                        result.Add(property.Name, ToPortableValue(property.Value));
                    }
                    return result;
                default:
                    throw new InvalidOperationException("Unsupported JSON value kind: " + value.ValueKind.ToString());
            }
        }
    }

    public sealed class McpProjectedCapability
    {
        public McpProjectedCapability(string toolName, CapabilityDefinition definition)
        {
            ToolName = toolName ?? throw new ArgumentNullException(nameof(toolName));
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        }

        public string ToolName { get; }
        public CapabilityDefinition Definition { get; }
    }

    /// <summary>
    /// MCP-safe transport naming. Canonical keys that fit the MCP framing limit retain the reversible byte escape.
    /// Over-limit keys receive a bounded adapter-local surrogate. The canonical ordinal supplies collision separation
    /// inside the deterministically sorted composed inventory; the SHA-256 fingerprint is diagnostic only. Exact
    /// canonical identity remains in tool metadata and is used for neutral dispatch.
    /// </summary>
    public static class McpToolNameCodec
    {
        public const int MaxToolNameLength = 128;
        private const string LongNamePrefix = "arkus-long-";

        public static string Encode(CapabilityKey key)
        {
            return Encode(key, 0);
        }

        internal static string Encode(CapabilityKey key, int canonicalOrdinal)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (canonicalOrdinal < 0) throw new ArgumentOutOfRangeException(nameof(canonicalOrdinal));

            var reversible = EncodeReversible(key);
            if (reversible.Length <= MaxToolNameLength)
            {
                return reversible;
            }

            var canonicalKey = key.ToString();
            var digest = SHA256.HashData(Encoding.UTF8.GetBytes(canonicalKey));
            var fingerprint = Convert.ToHexString(digest).ToLowerInvariant();
            var surrogate = LongNamePrefix
                + canonicalOrdinal.ToString("D10", CultureInfo.InvariantCulture)
                + "-"
                + fingerprint;

            if (surrogate.Length == 0 || surrogate.Length > MaxToolNameLength)
            {
                throw new InvalidOperationException("Internal MCP long-name allocator produced an invalid tool name for canonical capability '" + key + "'.");
            }

            return surrogate;
        }

        private static string EncodeReversible(CapabilityKey key)
        {
            var source = key.Name + "@" + key.Version.ToString();
            var bytes = Encoding.UTF8.GetBytes(source);
            var builder = new StringBuilder(bytes.Length);
            foreach (var value in bytes)
            {
                if ((value >= (byte)'A' && value <= (byte)'Z') ||
                    (value >= (byte)'a' && value <= (byte)'z') ||
                    (value >= (byte)'0' && value <= (byte)'9') ||
                    value == (byte)'.' || value == (byte)'-')
                {
                    builder.Append((char)value);
                }
                else
                {
                    builder.Append('_');
                    builder.Append(value.ToString("X2", CultureInfo.InvariantCulture));
                }
            }

            return builder.ToString();
        }
    }
}
