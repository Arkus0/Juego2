using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Arkus.Harness.Protocol
{
    public sealed class ContractVersion : IEquatable<ContractVersion>, IComparable<ContractVersion>
    {
        public ContractVersion(int major, int minor)
        {
            if (major < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(major));
            }

            if (minor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minor));
            }

            Major = major;
            Minor = minor;
        }

        public int Major { get; }
        public int Minor { get; }

        public static ContractVersion Parse(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var parts = value.Split('.');
            if (parts.Length != 2 ||
                !int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var major) ||
                !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var minor) ||
                major < 0 || minor < 0)
            {
                throw new FormatException("Contract versions must use non-negative '<major>.<minor>' syntax.");
            }

            return new ContractVersion(major, minor);
        }

        public int CompareTo(ContractVersion? other)
        {
            if (other == null)
            {
                return 1;
            }

            var major = Major.CompareTo(other.Major);
            return major != 0 ? major : Minor.CompareTo(other.Minor);
        }

        public bool Equals(ContractVersion? other)
        {
            return other != null && Major == other.Major && Minor == other.Minor;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ContractVersion);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Major * 397) ^ Minor;
            }
        }

        public override string ToString()
        {
            return Major.ToString(CultureInfo.InvariantCulture) + "." + Minor.ToString(CultureInfo.InvariantCulture);
        }
    }

    public sealed class ContractVersionRange
    {
        public ContractVersionRange(int major, int minimumMinor, int maximumMinor)
        {
            if (major < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(major));
            }

            if (minimumMinor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumMinor));
            }

            if (maximumMinor < minimumMinor)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumMinor));
            }

            Major = major;
            MinimumMinor = minimumMinor;
            MaximumMinor = maximumMinor;
        }

        public int Major { get; }
        public int MinimumMinor { get; }
        public int MaximumMinor { get; }

        public static ContractVersionRange Exact(ContractVersion version)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            return new ContractVersionRange(version.Major, version.Minor, version.Minor);
        }

        public bool Accepts(ContractVersion version)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            return version.Major == Major && version.Minor >= MinimumMinor && version.Minor <= MaximumMinor;
        }

        public override string ToString()
        {
            return Major.ToString(CultureInfo.InvariantCulture) + "." +
                MinimumMinor.ToString(CultureInfo.InvariantCulture) + "-" +
                Major.ToString(CultureInfo.InvariantCulture) + "." +
                MaximumMinor.ToString(CultureInfo.InvariantCulture);
        }
    }

    public enum ProviderKind
    {
        Unknown = 0,
        Base = 1,
        Scoped = 2,
        TransportProjection = 3
    }

    public enum SideEffectClass
    {
        Unknown = 0,
        None = 1,
        ReadOnly = 2,
        CanonicalMutation = 3,
        ExternalReversible = 4,
        ExternalIrreversible = 5
    }

    public enum DeterminismClass
    {
        Unknown = 0,
        Deterministic = 1,
        DeterministicGivenSeed = 2,
        EnvironmentDependent = 3,
        NonDeterministic = 4
    }

    public enum ConcurrencyClass
    {
        Unknown = 0,
        ParallelSafe = 1,
        Serialized = 2,
        OptimisticVersioned = 3
    }

    public enum IdempotencyClass
    {
        Unknown = 0,
        Idempotent = 1,
        IdempotentWithKey = 2,
        NonIdempotent = 3
    }

    public enum BatchingClass
    {
        Unknown = 0,
        Unsupported = 1,
        Supported = 2,
        Required = 3
    }

    public enum TransactionRequirement
    {
        Unknown = 0,
        None = 1,
        ReadOnlyEnvelope = 2,
        CanonicalTransaction = 3
    }

    public enum ProvenanceRequirement
    {
        Unknown = 0,
        Minimal = 1,
        Required = 2
    }

    public enum PrivilegeClass
    {
        Unknown = 0,
        PublicRead = 1,
        Authoring = 2,
        Elevated = 3
    }

    public sealed class ProviderMetadata
    {
        public ProviderMetadata(string providerId, ProviderKind kind, string scope, string capabilityNamespace)
        {
            ProviderId = providerId ?? throw new ArgumentNullException(nameof(providerId));
            Kind = kind;
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            CapabilityNamespace = capabilityNamespace ?? throw new ArgumentNullException(nameof(capabilityNamespace));
        }

        public string ProviderId { get; }
        public ProviderKind Kind { get; }
        public string Scope { get; }
        public string CapabilityNamespace { get; }
    }

    public sealed class ConcurrencySemantics
    {
        public ConcurrencySemantics(ConcurrencyClass concurrencyClass, string? versionTokenName = null)
        {
            Class = concurrencyClass;
            VersionTokenName = versionTokenName;
        }

        public ConcurrencyClass Class { get; }
        public string? VersionTokenName { get; }
    }

    public sealed class IdempotencySemantics
    {
        public IdempotencySemantics(IdempotencyClass idempotencyClass, string? keyField = null)
        {
            Class = idempotencyClass;
            KeyField = keyField;
        }

        public IdempotencyClass Class { get; }
        public string? KeyField { get; }
    }

    public sealed class BatchingSemantics
    {
        public BatchingSemantics(BatchingClass batchingClass, int? maximumItems = null)
        {
            if (maximumItems.HasValue && maximumItems.Value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumItems));
            }

            Class = batchingClass;
            MaximumItems = maximumItems;
        }

        public BatchingClass Class { get; }
        public int? MaximumItems { get; }
    }

    public sealed class RepairSemantics
    {
        public RepairSemantics(bool retryable, bool exposesRepairHint)
        {
            Retryable = retryable;
            ExposesRepairHint = exposesRepairHint;
        }

        public bool Retryable { get; }
        public bool ExposesRepairHint { get; }
    }

    public sealed class PolicySemantics
    {
        public PolicySemantics(
            PrivilegeClass privilege,
            TransactionRequirement transactionRequirement,
            ProvenanceRequirement provenanceRequirement)
        {
            Privilege = privilege;
            TransactionRequirement = transactionRequirement;
            ProvenanceRequirement = provenanceRequirement;
        }

        public PrivilegeClass Privilege { get; }
        public TransactionRequirement TransactionRequirement { get; }
        public ProvenanceRequirement ProvenanceRequirement { get; }
    }

    public sealed class CostSemantics
    {
        public CostSemantics(int relativeWeight, string? note = null)
        {
            if (relativeWeight < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(relativeWeight));
            }

            RelativeWeight = relativeWeight;
            Note = note;
        }

        public int RelativeWeight { get; }
        public string? Note { get; }
    }

    public sealed class CapabilityKey : IEquatable<CapabilityKey>
    {
        public CapabilityKey(string name, ContractVersion version)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }

        public string Name { get; }
        public ContractVersion Version { get; }

        public bool Equals(CapabilityKey? other)
        {
            return other != null &&
                string.Equals(Name, other.Name, StringComparison.Ordinal) &&
                Version.Equals(other.Version);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as CapabilityKey);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (StringComparer.Ordinal.GetHashCode(Name) * 397) ^ Version.GetHashCode();
            }
        }

        public override string ToString()
        {
            return Name + "@" + Version;
        }
    }

    public sealed class CapabilityDefinition
    {
        public CapabilityDefinition(
            CapabilityKey key,
            ProviderMetadata provider,
            JsonSchemaDocument? requestSchema,
            JsonSchemaDocument? successSchema,
            JsonSchemaDocument? errorSchema,
            SideEffectClass sideEffect,
            DeterminismClass determinism,
            IEnumerable<string>? preconditions,
            IEnumerable<string>? postconditions,
            ConcurrencySemantics? concurrency,
            IdempotencySemantics? idempotency,
            BatchingSemantics? batching,
            RepairSemantics? repair,
            PolicySemantics? policy,
            CostSemantics? cost = null)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            RequestSchema = requestSchema;
            SuccessSchema = successSchema;
            ErrorSchema = errorSchema;
            SideEffect = sideEffect;
            Determinism = determinism;
            Preconditions = CopyStrings(preconditions);
            Postconditions = CopyStrings(postconditions);
            Concurrency = concurrency;
            Idempotency = idempotency;
            Batching = batching;
            Repair = repair;
            Policy = policy;
            Cost = cost;
        }

        public CapabilityKey Key { get; }
        public ProviderMetadata Provider { get; }
        public JsonSchemaDocument? RequestSchema { get; }
        public JsonSchemaDocument? SuccessSchema { get; }
        public JsonSchemaDocument? ErrorSchema { get; }
        public SideEffectClass SideEffect { get; }
        public DeterminismClass Determinism { get; }
        public IReadOnlyList<string> Preconditions { get; }
        public IReadOnlyList<string> Postconditions { get; }
        public ConcurrencySemantics? Concurrency { get; }
        public IdempotencySemantics? Idempotency { get; }
        public BatchingSemantics? Batching { get; }
        public RepairSemantics? Repair { get; }
        public PolicySemantics? Policy { get; }
        public CostSemantics? Cost { get; }

        public IReadOnlyDictionary<string, object?> ToData()
        {
            var data = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["name"] = Key.Name,
                ["version"] = Key.Version.ToString(),
                ["provider"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["id"] = Provider.ProviderId,
                    ["kind"] = Token(Provider.Kind),
                    ["scope"] = Provider.Scope,
                    ["namespace"] = Provider.CapabilityNamespace
                },
                ["requestSchema"] = RequestSchema?.ToData(),
                ["successSchema"] = SuccessSchema?.ToData(),
                ["errorSchema"] = ErrorSchema?.ToData(),
                ["sideEffect"] = Token(SideEffect),
                ["determinism"] = Token(Determinism),
                ["preconditions"] = StringListToData(Preconditions),
                ["postconditions"] = StringListToData(Postconditions),
                ["concurrency"] = Concurrency == null ? null : new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["class"] = Token(Concurrency.Class),
                    ["versionToken"] = Concurrency.VersionTokenName
                },
                ["idempotency"] = Idempotency == null ? null : new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["class"] = Token(Idempotency.Class),
                    ["keyField"] = Idempotency.KeyField
                },
                ["batching"] = Batching == null ? null : new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["class"] = Token(Batching.Class),
                    ["maximumItems"] = Batching.MaximumItems
                },
                ["repair"] = Repair == null ? null : new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["retryable"] = Repair.Retryable,
                    ["exposesRepairHint"] = Repair.ExposesRepairHint
                },
                ["policy"] = Policy == null ? null : new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["privilege"] = Token(Policy.Privilege),
                    ["transaction"] = Token(Policy.TransactionRequirement),
                    ["provenance"] = Token(Policy.ProvenanceRequirement)
                }
            };

            if (Cost != null)
            {
                data["cost"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["relativeWeight"] = Cost.RelativeWeight,
                    ["note"] = Cost.Note
                };
            }

            return new ReadOnlyDictionary<string, object?>(data);
        }

        public string SemanticFingerprint()
        {
            return BuildSemanticFingerprint(true);
        }

        internal string SemanticFingerprintWithoutRequest()
        {
            return BuildSemanticFingerprint(false);
        }

        private string BuildSemanticFingerprint(bool includeRequest)
        {
            var writer = new SemanticFingerprintWriter();
            writer.WriteString("arkus-capability-fingerprint-v1");
            writer.WriteString(Key.Name);
            writer.WriteInt(Key.Version.Major);
            writer.WriteInt(Key.Version.Minor);
            writer.WriteString(Provider.ProviderId);
            writer.WriteInt((int)Provider.Kind);
            writer.WriteString(Provider.Scope);
            writer.WriteString(Provider.CapabilityNamespace);
            writer.WriteBool(includeRequest);
            if (includeRequest)
            {
                writer.WriteString(RequestSchema?.SemanticFingerprint());
            }

            writer.WriteString(SuccessSchema?.SemanticFingerprint());
            writer.WriteString(ErrorSchema?.SemanticFingerprint());
            writer.WriteInt((int)SideEffect);
            writer.WriteInt((int)Determinism);
            writer.WriteSequence(Preconditions);
            writer.WriteSequence(Postconditions);
            writer.WriteBool(Concurrency != null);
            if (Concurrency != null)
            {
                writer.WriteInt((int)Concurrency.Class);
                writer.WriteString(Concurrency.VersionTokenName);
            }

            writer.WriteBool(Idempotency != null);
            if (Idempotency != null)
            {
                writer.WriteInt((int)Idempotency.Class);
                writer.WriteString(Idempotency.KeyField);
            }

            writer.WriteBool(Batching != null);
            if (Batching != null)
            {
                writer.WriteInt((int)Batching.Class);
                writer.WriteNullableInt(Batching.MaximumItems);
            }

            writer.WriteBool(Repair != null);
            if (Repair != null)
            {
                writer.WriteBool(Repair.Retryable);
                writer.WriteBool(Repair.ExposesRepairHint);
            }

            writer.WriteBool(Policy != null);
            if (Policy != null)
            {
                writer.WriteInt((int)Policy.Privilege);
                writer.WriteInt((int)Policy.TransactionRequirement);
                writer.WriteInt((int)Policy.ProvenanceRequirement);
            }

            writer.WriteBool(Cost != null);
            if (Cost != null)
            {
                writer.WriteInt(Cost.RelativeWeight);
                writer.WriteString(Cost.Note);
            }

            return writer.ToString();
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
                copy.Add(value ?? throw new ArgumentException("Metadata string collections may not contain null values.", nameof(values)));
            }

            return copy.AsReadOnly();
        }

        private static IReadOnlyList<object?> StringListToData(IReadOnlyList<string> values)
        {
            var data = new List<object?>();
            foreach (var value in values)
            {
                data.Add(value);
            }

            return data.AsReadOnly();
        }

        private static string Token(object value)
        {
            return value.ToString()?.ToLowerInvariant() ?? string.Empty;
        }
    }

    public sealed class StructuredError
    {
        public StructuredError(
            string machineCode,
            string message,
            string path,
            IReadOnlyDictionary<string, object?>? context,
            bool retryable,
            string? repairHint)
        {
            MachineCode = machineCode ?? throw new ArgumentNullException(nameof(machineCode));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Context = context == null
                ? new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal))
                : new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(context, StringComparer.Ordinal));
            Retryable = retryable;
            RepairHint = repairHint;
        }

        public string MachineCode { get; }
        public string Message { get; }
        public string Path { get; }
        public IReadOnlyDictionary<string, object?> Context { get; }
        public bool Retryable { get; }
        public string? RepairHint { get; }

        public IReadOnlyDictionary<string, object?> ToData()
        {
            var data = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["machineCode"] = MachineCode,
                ["message"] = Message,
                ["path"] = Path,
                ["context"] = Context,
                ["retryable"] = Retryable
            };

            if (RepairHint != null)
            {
                data["repairHint"] = RepairHint;
            }

            return new ReadOnlyDictionary<string, object?>(data);
        }
    }

    public sealed class CapabilityInvocationResult
    {
        private CapabilityInvocationResult(bool success, IReadOnlyDictionary<string, object?>? data, StructuredError? error)
        {
            Success = success;
            Data = data;
            Error = error;
        }

        public bool Success { get; }
        public IReadOnlyDictionary<string, object?>? Data { get; }
        public StructuredError? Error { get; }

        public static CapabilityInvocationResult Succeeded(IReadOnlyDictionary<string, object?> data)
        {
            return new CapabilityInvocationResult(true, data ?? throw new ArgumentNullException(nameof(data)), null);
        }

        public static CapabilityInvocationResult Failed(StructuredError error)
        {
            return new CapabilityInvocationResult(false, null, error ?? throw new ArgumentNullException(nameof(error)));
        }
    }

    public static class CanonicalContractSchemas
    {
        public static JsonSchemaDocument EmptyObject()
        {
            return new JsonSchemaDocument(SchemaNode.Object());
        }

        public static JsonSchemaDocument StructuredError()
        {
            var properties = new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
            {
                ["machineCode"] = SchemaNode.String(),
                ["message"] = SchemaNode.String(),
                ["path"] = SchemaNode.String(),
                ["context"] = SchemaNode.Object(additionalPropertiesAllowed: true),
                ["retryable"] = SchemaNode.Boolean(),
                ["repairHint"] = SchemaNode.String()
            };

            return new JsonSchemaDocument(SchemaNode.Object(
                properties,
                new[] { "machineCode", "message", "path", "context", "retryable" }));
        }

        public static JsonSchemaDocument SystemDescribeSuccess()
        {
            var capabilityProperties = new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
            {
                ["name"] = SchemaNode.String(),
                ["version"] = SchemaNode.String(),
                ["provider"] = SchemaNode.Object(additionalPropertiesAllowed: true),
                ["requestSchema"] = SchemaNode.Any(),
                ["successSchema"] = SchemaNode.Any(),
                ["errorSchema"] = SchemaNode.Any(),
                ["sideEffect"] = SchemaNode.String(),
                ["determinism"] = SchemaNode.String(),
                ["preconditions"] = SchemaNode.Array(SchemaNode.String()),
                ["postconditions"] = SchemaNode.Array(SchemaNode.String()),
                ["concurrency"] = SchemaNode.Object(additionalPropertiesAllowed: true),
                ["idempotency"] = SchemaNode.Object(additionalPropertiesAllowed: true),
                ["batching"] = SchemaNode.Object(additionalPropertiesAllowed: true),
                ["repair"] = SchemaNode.Object(additionalPropertiesAllowed: true),
                ["policy"] = SchemaNode.Object(additionalPropertiesAllowed: true),
                ["cost"] = SchemaNode.Object(additionalPropertiesAllowed: true)
            };

            var capability = SchemaNode.Object(
                capabilityProperties,
                new[]
                {
                    "name", "version", "provider", "requestSchema", "successSchema", "errorSchema",
                    "sideEffect", "determinism", "preconditions", "postconditions", "concurrency",
                    "idempotency", "batching", "repair", "policy"
                });

            return new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["contractModelVersion"] = SchemaNode.String(),
                    ["capabilities"] = SchemaNode.Array(capability)
                },
                new[] { "contractModelVersion", "capabilities" }));
        }
    }

    public sealed class ContractValidationIssue
    {
        public ContractValidationIssue(string code, string path, string message)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public string Code { get; }
        public string Path { get; }
        public string Message { get; }
    }

    public static class CanonicalContractValidator
    {
        public static IReadOnlyList<ContractValidationIssue> Validate(CapabilityDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            var issues = new List<ContractValidationIssue>();
            ValidateIdentifier(definition.Key.Name, "$.name", issues);
            ValidateIdentifier(definition.Provider.ProviderId, "$.provider.id", issues);
            ValidateIdentifier(definition.Provider.Scope, "$.provider.scope", issues);
            ValidateIdentifier(definition.Provider.CapabilityNamespace, "$.provider.namespace", issues);

            if (definition.Provider.Kind == ProviderKind.Unknown || definition.Provider.Kind == ProviderKind.TransportProjection)
            {
                issues.Add(new ContractValidationIssue(
                    "contract.invalid_provider_kind",
                    "$.provider.kind",
                    "Canonical definitions must originate from base or accepted scoped semantic providers."));
            }

            if (!IsWithinNamespace(definition.Key.Name, definition.Provider.CapabilityNamespace))
            {
                issues.Add(new ContractValidationIssue(
                    "contract.namespace_mismatch",
                    "$.name",
                    "Capability identity is outside the provider-owned canonical namespace."));
            }

            ValidateSchema(definition.RequestSchema, "$.requestSchema", issues);
            ValidateSchema(definition.SuccessSchema, "$.successSchema", issues);
            ValidateSchema(definition.ErrorSchema, "$.errorSchema", issues);

            if (definition.ErrorSchema != null &&
                !CanonicalSemanticEquality.SchemaDocumentsEqual(
                    definition.ErrorSchema,
                    CanonicalContractSchemas.StructuredError()))
            {
                issues.Add(new ContractValidationIssue(
                    "contract.noncanonical_error_schema",
                    "$.errorSchema",
                    "All public capabilities use the stable canonical StructuredError schema."));
            }

            if (definition.SideEffect == SideEffectClass.Unknown)
            {
                issues.Add(new ContractValidationIssue("contract.missing_side_effect", "$.sideEffect", "Side-effect metadata is mandatory."));
            }

            if (definition.Determinism == DeterminismClass.Unknown)
            {
                issues.Add(new ContractValidationIssue("contract.missing_determinism", "$.determinism", "Determinism metadata is mandatory."));
            }

            if (definition.Concurrency == null || definition.Concurrency.Class == ConcurrencyClass.Unknown)
            {
                issues.Add(new ContractValidationIssue("contract.missing_concurrency", "$.concurrency", "Concurrency metadata is mandatory."));
            }

            if (definition.Idempotency == null || definition.Idempotency.Class == IdempotencyClass.Unknown)
            {
                issues.Add(new ContractValidationIssue("contract.missing_idempotency", "$.idempotency", "Idempotency metadata is mandatory."));
            }

            if (definition.Batching == null || definition.Batching.Class == BatchingClass.Unknown)
            {
                issues.Add(new ContractValidationIssue("contract.missing_batching", "$.batching", "Batching metadata is mandatory."));
            }

            if (definition.Repair == null)
            {
                issues.Add(new ContractValidationIssue("contract.missing_repair", "$.repair", "Retry/repair metadata is mandatory."));
            }

            if (definition.Policy == null ||
                definition.Policy.Privilege == PrivilegeClass.Unknown ||
                definition.Policy.TransactionRequirement == TransactionRequirement.Unknown ||
                definition.Policy.ProvenanceRequirement == ProvenanceRequirement.Unknown)
            {
                issues.Add(new ContractValidationIssue(
                    "contract.missing_policy",
                    "$.policy",
                    "Privilege, transaction and provenance metadata are mandatory."));
            }

            return issues.AsReadOnly();
        }

        private static void ValidateSchema(
            JsonSchemaDocument? schema,
            string path,
            IList<ContractValidationIssue> issues)
        {
            if (schema == null)
            {
                issues.Add(new ContractValidationIssue("contract.missing_schema", path, "Request, success and error schemas are mandatory."));
                return;
            }

            foreach (var schemaIssue in schema.ValidateDefinition())
            {
                issues.Add(new ContractValidationIssue(
                    "contract." + schemaIssue.Code,
                    path + schemaIssue.Path.Substring(1),
                    schemaIssue.Message));
            }
        }

        private static void ValidateIdentifier(string value, string path, IList<ContractValidationIssue> issues)
        {
            if (!IsCanonicalIdentifier(value))
            {
                issues.Add(new ContractValidationIssue(
                    "contract.invalid_identifier",
                    path,
                    "Identifiers must be lower-case dot-separated portable tokens using letters, digits and hyphens."));
            }
        }

        internal static bool IsWithinNamespace(string capabilityName, string capabilityNamespace)
        {
            return string.Equals(capabilityName, capabilityNamespace, StringComparison.Ordinal) ||
                capabilityName.StartsWith(capabilityNamespace + ".", StringComparison.Ordinal);
        }

        internal static bool IsCanonicalIdentifier(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value[0] == '.' || value[value.Length - 1] == '.')
            {
                return false;
            }

            var previousDot = false;
            foreach (var character in value)
            {
                if (character == '.')
                {
                    if (previousDot)
                    {
                        return false;
                    }

                    previousDot = true;
                    continue;
                }

                previousDot = false;
                if ((character < 'a' || character > 'z') &&
                    (character < '0' || character > '9') &&
                    character != '-')
                {
                    return false;
                }
            }

            return true;
        }
    }
}
