using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace Arkus.Harness.Protocol
{
    /// <summary>
    /// Exact structural semantic equality for canonical contract objects.
    /// Correctness decisions must use this comparer rather than serialized fingerprints.
    /// </summary>
    internal static class CanonicalSemanticEquality
    {
        internal static bool DefinitionsEqual(CapabilityDefinition left, CapabilityDefinition right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            return left.Key.Equals(right.Key) &&
                ProviderEquals(left.Provider, right.Provider) &&
                SchemaDocumentsEqual(left.RequestSchema, right.RequestSchema) &&
                SchemaDocumentsEqual(left.SuccessSchema, right.SuccessSchema) &&
                SchemaDocumentsEqual(left.ErrorSchema, right.ErrorSchema) &&
                left.SideEffect == right.SideEffect &&
                left.Determinism == right.Determinism &&
                SequenceEquals(left.Preconditions, right.Preconditions) &&
                SequenceEquals(left.Postconditions, right.Postconditions) &&
                ConcurrencyEquals(left.Concurrency, right.Concurrency) &&
                IdempotencyEquals(left.Idempotency, right.Idempotency) &&
                BatchingEquals(left.Batching, right.Batching) &&
                RepairEquals(left.Repair, right.Repair) &&
                PolicyEquals(left.Policy, right.Policy) &&
                CostEquals(left.Cost, right.Cost);
        }

        internal static bool SchemaDocumentsEqual(JsonSchemaDocument? left, JsonSchemaDocument? right)
        {
            if (left == null || right == null)
            {
                return left == null && right == null;
            }

            return string.Equals(left.Dialect, right.Dialect, StringComparison.Ordinal) &&
                SchemaNodesEqual(left.Root, right.Root);
        }

        internal static bool SchemaNodesEqual(SchemaNode left, SchemaNode right)
        {
            if (left.ValueType != right.ValueType ||
                left.AdditionalPropertiesAllowed != right.AdditionalPropertiesAllowed ||
                !string.Equals(left.Format, right.Format, StringComparison.Ordinal) ||
                !string.Equals(left.LogicalReferenceNamespace, right.LogicalReferenceNamespace, StringComparison.Ordinal) ||
                !SetEquals(left.RequiredProperties, right.RequiredProperties) ||
                !SetEquals(left.AllowedStringValues, right.AllowedStringValues) ||
                left.Properties.Count != right.Properties.Count)
            {
                return false;
            }

            foreach (var pair in left.Properties)
            {
                if (!right.Properties.TryGetValue(pair.Key, out var rightProperty) ||
                    !SchemaNodesEqual(pair.Value, rightProperty))
                {
                    return false;
                }
            }

            if (left.Items == null || right.Items == null)
            {
                return left.Items == null && right.Items == null;
            }

            return SchemaNodesEqual(left.Items, right.Items);
        }

        private static bool ProviderEquals(ProviderMetadata left, ProviderMetadata right)
        {
            return string.Equals(left.ProviderId, right.ProviderId, StringComparison.Ordinal) &&
                left.Kind == right.Kind &&
                string.Equals(left.Scope, right.Scope, StringComparison.Ordinal) &&
                string.Equals(left.CapabilityNamespace, right.CapabilityNamespace, StringComparison.Ordinal);
        }

        private static bool SequenceEquals(IReadOnlyList<string> left, IReadOnlyList<string> right)
        {
            if (left.Count != right.Count)
            {
                return false;
            }

            for (var index = 0; index < left.Count; index++)
            {
                if (!string.Equals(left[index], right[index], StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool SetEquals(IReadOnlyList<string> left, IReadOnlyList<string> right)
        {
            return new HashSet<string>(left, StringComparer.Ordinal).SetEquals(right);
        }

        private static bool ConcurrencyEquals(ConcurrencySemantics? left, ConcurrencySemantics? right)
        {
            if (left == null || right == null)
            {
                return left == null && right == null;
            }

            return left.Class == right.Class && string.Equals(left.VersionTokenName, right.VersionTokenName, StringComparison.Ordinal);
        }

        private static bool IdempotencyEquals(IdempotencySemantics? left, IdempotencySemantics? right)
        {
            if (left == null || right == null)
            {
                return left == null && right == null;
            }

            return left.Class == right.Class && string.Equals(left.KeyField, right.KeyField, StringComparison.Ordinal);
        }

        private static bool BatchingEquals(BatchingSemantics? left, BatchingSemantics? right)
        {
            if (left == null || right == null)
            {
                return left == null && right == null;
            }

            return left.Class == right.Class && left.MaximumItems == right.MaximumItems;
        }

        private static bool RepairEquals(RepairSemantics? left, RepairSemantics? right)
        {
            if (left == null || right == null)
            {
                return left == null && right == null;
            }

            return left.Retryable == right.Retryable && left.ExposesRepairHint == right.ExposesRepairHint;
        }

        private static bool PolicyEquals(PolicySemantics? left, PolicySemantics? right)
        {
            if (left == null || right == null)
            {
                return left == null && right == null;
            }

            return left.Privilege == right.Privilege &&
                left.TransactionRequirement == right.TransactionRequirement &&
                left.ProvenanceRequirement == right.ProvenanceRequirement;
        }

        private static bool CostEquals(CostSemantics? left, CostSemantics? right)
        {
            if (left == null || right == null)
            {
                return left == null && right == null;
            }

            return left.RelativeWeight == right.RelativeWeight && string.Equals(left.Note, right.Note, StringComparison.Ordinal);
        }
    }

    /// <summary>
    /// Independent expected-data oracle for the public discovery projection. It deliberately
    /// does not call CapabilityDefinition.ToData, JsonSchemaDocument.ToData or SchemaNode.ToData,
    /// so a defect in the production projector cannot redefine its own conformance expectation.
    /// </summary>
    internal static class CanonicalProjectionOracleData
    {
        internal static IReadOnlyDictionary<string, object?> FromDefinition(CapabilityDefinition definition)
        {
            var data = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["name"] = definition.Key.Name,
                ["version"] = definition.Key.Version.ToString(),
                ["provider"] = ReadOnlyMap(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["id"] = definition.Provider.ProviderId,
                    ["kind"] = ProviderToken(definition.Provider.Kind),
                    ["scope"] = definition.Provider.Scope,
                    ["namespace"] = definition.Provider.CapabilityNamespace
                }),
                ["requestSchema"] = SchemaDocumentData(definition.RequestSchema),
                ["successSchema"] = SchemaDocumentData(definition.SuccessSchema),
                ["errorSchema"] = SchemaDocumentData(definition.ErrorSchema),
                ["sideEffect"] = SideEffectToken(definition.SideEffect),
                ["determinism"] = DeterminismToken(definition.Determinism),
                ["preconditions"] = StringListData(definition.Preconditions),
                ["postconditions"] = StringListData(definition.Postconditions),
                ["concurrency"] = definition.Concurrency == null ? null : ReadOnlyMap(
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["class"] = ConcurrencyToken(definition.Concurrency.Class),
                        ["versionToken"] = definition.Concurrency.VersionTokenName
                    }),
                ["idempotency"] = definition.Idempotency == null ? null : ReadOnlyMap(
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["class"] = IdempotencyToken(definition.Idempotency.Class),
                        ["keyField"] = definition.Idempotency.KeyField
                    }),
                ["batching"] = definition.Batching == null ? null : ReadOnlyMap(
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["class"] = BatchingToken(definition.Batching.Class),
                        ["maximumItems"] = definition.Batching.MaximumItems
                    }),
                ["repair"] = definition.Repair == null ? null : ReadOnlyMap(
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["retryable"] = definition.Repair.Retryable,
                        ["exposesRepairHint"] = definition.Repair.ExposesRepairHint
                    }),
                ["policy"] = definition.Policy == null ? null : ReadOnlyMap(
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["privilege"] = PrivilegeToken(definition.Policy.Privilege),
                        ["transaction"] = TransactionToken(definition.Policy.TransactionRequirement),
                        ["provenance"] = ProvenanceToken(definition.Policy.ProvenanceRequirement)
                    })
            };

            if (definition.Cost != null)
            {
                data["cost"] = ReadOnlyMap(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["relativeWeight"] = definition.Cost.RelativeWeight,
                    ["note"] = definition.Cost.Note
                });
            }

            return ReadOnlyMap(data);
        }

        private static object? SchemaDocumentData(JsonSchemaDocument? schema)
        {
            if (schema == null)
            {
                return null;
            }

            var data = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["$schema"] = schema.Dialect
            };
            foreach (var pair in SchemaNodeData(schema.Root))
            {
                data[pair.Key] = pair.Value;
            }

            return ReadOnlyMap(data);
        }

        private static IReadOnlyDictionary<string, object?> SchemaNodeData(SchemaNode node)
        {
            var data = new Dictionary<string, object?>(StringComparer.Ordinal);
            if (node.ValueType != SchemaValueType.Any)
            {
                data["type"] = SchemaTypeToken(node.ValueType);
            }

            if (node.Properties.Count != 0)
            {
                var properties = new Dictionary<string, object?>(StringComparer.Ordinal);
                foreach (var pair in node.Properties)
                {
                    properties[pair.Key] = SchemaNodeData(pair.Value);
                }

                data["properties"] = ReadOnlyMap(properties);
            }

            if (node.ValueType == SchemaValueType.Object)
            {
                data["required"] = StringListData(node.RequiredProperties);
                data["additionalProperties"] = node.AdditionalPropertiesAllowed;
            }

            if (node.Items != null)
            {
                data["items"] = SchemaNodeData(node.Items);
            }

            if (node.AllowedStringValues.Count != 0)
            {
                data["enum"] = StringListData(node.AllowedStringValues);
            }

            if (node.Format != null)
            {
                data["format"] = node.Format;
            }

            if (node.LogicalReferenceNamespace != null)
            {
                data["x-arkus-reference-namespace"] = node.LogicalReferenceNamespace;
            }

            return ReadOnlyMap(data);
        }

        private static IReadOnlyList<object?> StringListData(IReadOnlyList<string> values)
        {
            var data = new List<object?>();
            foreach (var value in values)
            {
                data.Add(value);
            }

            return data.AsReadOnly();
        }

        private static IReadOnlyDictionary<string, object?> ReadOnlyMap(Dictionary<string, object?> data)
        {
            return new ReadOnlyDictionary<string, object?>(data);
        }

        private static string ProviderToken(ProviderKind value)
        {
            return value switch
            {
                ProviderKind.Unknown => "unknown",
                ProviderKind.Base => "base",
                ProviderKind.Scoped => "scoped",
                ProviderKind.TransportProjection => "transportprojection",
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            };
        }

        private static string SideEffectToken(SideEffectClass value)
        {
            return value switch
            {
                SideEffectClass.Unknown => "unknown",
                SideEffectClass.None => "none",
                SideEffectClass.ReadOnly => "readonly",
                SideEffectClass.CanonicalMutation => "canonicalmutation",
                SideEffectClass.ExternalReversible => "externalreversible",
                SideEffectClass.ExternalIrreversible => "externalirreversible",
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            };
        }

        private static string DeterminismToken(DeterminismClass value)
        {
            return value switch
            {
                DeterminismClass.Unknown => "unknown",
                DeterminismClass.Deterministic => "deterministic",
                DeterminismClass.DeterministicGivenSeed => "deterministicgivenseed",
                DeterminismClass.EnvironmentDependent => "environmentdependent",
                DeterminismClass.NonDeterministic => "nondeterministic",
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            };
        }

        private static string ConcurrencyToken(ConcurrencyClass value)
        {
            return value switch
            {
                ConcurrencyClass.Unknown => "unknown",
                ConcurrencyClass.ParallelSafe => "parallelsafe",
                ConcurrencyClass.Serialized => "serialized",
                ConcurrencyClass.OptimisticVersioned => "optimisticversioned",
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            };
        }

        private static string IdempotencyToken(IdempotencyClass value)
        {
            return value switch
            {
                IdempotencyClass.Unknown => "unknown",
                IdempotencyClass.Idempotent => "idempotent",
                IdempotencyClass.IdempotentWithKey => "idempotentwithkey",
                IdempotencyClass.NonIdempotent => "nonidempotent",
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            };
        }

        private static string BatchingToken(BatchingClass value)
        {
            return value switch
            {
                BatchingClass.Unknown => "unknown",
                BatchingClass.Unsupported => "unsupported",
                BatchingClass.Supported => "supported",
                BatchingClass.Required => "required",
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            };
        }

        private static string PrivilegeToken(PrivilegeClass value)
        {
            return value switch
            {
                PrivilegeClass.Unknown => "unknown",
                PrivilegeClass.PublicRead => "publicread",
                PrivilegeClass.Authoring => "authoring",
                PrivilegeClass.Elevated => "elevated",
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            };
        }

        private static string TransactionToken(TransactionRequirement value)
        {
            return value switch
            {
                TransactionRequirement.Unknown => "unknown",
                TransactionRequirement.None => "none",
                TransactionRequirement.ReadOnlyEnvelope => "readonlyenvelope",
                TransactionRequirement.CanonicalTransaction => "canonicaltransaction",
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            };
        }

        private static string ProvenanceToken(ProvenanceRequirement value)
        {
            return value switch
            {
                ProvenanceRequirement.Unknown => "unknown",
                ProvenanceRequirement.Minimal => "minimal",
                ProvenanceRequirement.Required => "required",
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            };
        }

        private static string SchemaTypeToken(SchemaValueType value)
        {
            return value switch
            {
                SchemaValueType.Object => "object",
                SchemaValueType.String => "string",
                SchemaValueType.Integer => "integer",
                SchemaValueType.Number => "number",
                SchemaValueType.Boolean => "boolean",
                SchemaValueType.Array => "array",
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            };
        }
    }

    /// <summary>
    /// Collision-free framing for diagnostic fingerprints. Each value is length-prefixed and
    /// collections declare their cardinality, so delimiter-bearing canonical data stays distinct.
    /// </summary>
    internal sealed class SemanticFingerprintWriter
    {
        private readonly StringBuilder _builder = new StringBuilder();

        internal void WriteString(string? value)
        {
            if (value == null)
            {
                _builder.Append("-1:");
                return;
            }

            _builder.Append(value.Length.ToString(CultureInfo.InvariantCulture));
            _builder.Append(':');
            _builder.Append(value);
        }

        internal void WriteInt(int value)
        {
            WriteString(value.ToString(CultureInfo.InvariantCulture));
        }

        internal void WriteNullableInt(int? value)
        {
            WriteString(value?.ToString(CultureInfo.InvariantCulture));
        }

        internal void WriteBool(bool value)
        {
            WriteString(value ? "1" : "0");
        }

        internal void WriteSequence(IReadOnlyList<string> values)
        {
            WriteInt(values.Count);
            foreach (var value in values)
            {
                WriteString(value);
            }
        }

        internal void WriteSet(IReadOnlyList<string> values)
        {
            var unique = new HashSet<string>(values, StringComparer.Ordinal);
            var sorted = new List<string>(unique);
            sorted.Sort(StringComparer.Ordinal);
            WriteSequence(sorted);
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }

    internal static class CanonicalDataEquality
    {
        internal static bool AreEqual(object? left, object? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left == null || right == null)
            {
                return false;
            }

            if (left is IReadOnlyDictionary<string, object?> leftMap &&
                right is IReadOnlyDictionary<string, object?> rightMap)
            {
                if (leftMap.Count != rightMap.Count)
                {
                    return false;
                }

                foreach (var pair in leftMap)
                {
                    if (!rightMap.TryGetValue(pair.Key, out var rightValue) || !AreEqual(pair.Value, rightValue))
                    {
                        return false;
                    }
                }

                return true;
            }

            if (left is IReadOnlyList<object?> leftList && right is IReadOnlyList<object?> rightList)
            {
                if (leftList.Count != rightList.Count)
                {
                    return false;
                }

                for (var index = 0; index < leftList.Count; index++)
                {
                    if (!AreEqual(leftList[index], rightList[index]))
                    {
                        return false;
                    }
                }

                return true;
            }

            return left.GetType() == right.GetType() && left.Equals(right);
        }
    }
}
