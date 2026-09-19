using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Runtime
{
    public sealed class ComposedContract
    {
        private readonly IReadOnlyDictionary<CapabilityKey, CapabilityDefinition> _definitionsByKey;
        private readonly IReadOnlyDictionary<CapabilityKey, CapabilityRoute> _routesByKey;

        internal ComposedContract(
            IDictionary<CapabilityKey, CapabilityDefinition> definitions,
            IDictionary<CapabilityKey, CapabilityRoute> routes)
        {
            var definitionCopy = new Dictionary<CapabilityKey, CapabilityDefinition>(definitions);
            var routeCopy = new Dictionary<CapabilityKey, CapabilityRoute>(routes);
            _definitionsByKey = new ReadOnlyDictionary<CapabilityKey, CapabilityDefinition>(definitionCopy);
            _routesByKey = new ReadOnlyDictionary<CapabilityKey, CapabilityRoute>(routeCopy);

            var definitionList = new List<CapabilityDefinition>(definitionCopy.Values);
            definitionList.Sort(CompareDefinitions);
            Definitions = definitionList.AsReadOnly();
            Projection = new CanonicalContractProjection(Definitions);
        }

        public IReadOnlyList<CapabilityDefinition> Definitions { get; }
        public CanonicalContractProjection Projection { get; }

        internal IEnumerable<CapabilityKey> RouteKeys => _routesByKey.Keys;

        public CapabilityInvocationResult Dispatch(
            string capabilityName,
            ContractVersionRange acceptedVersions,
            IReadOnlyDictionary<string, object?> request)
        {
            if (capabilityName == null)
            {
                throw new ArgumentNullException(nameof(capabilityName));
            }

            if (acceptedVersions == null)
            {
                throw new ArgumentNullException(nameof(acceptedVersions));
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var available = new List<CapabilityDefinition>();
            foreach (var definition in Definitions)
            {
                if (string.Equals(definition.Key.Name, capabilityName, StringComparison.Ordinal))
                {
                    available.Add(definition);
                }
            }

            if (available.Count == 0)
            {
                return Failure(
                    "contract.unknown_capability",
                    "Unknown canonical capability.",
                    "$.capability",
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["capability"] = capabilityName
                    },
                    false,
                    "Call system.describe and select a discovered capability identity.");
            }

            CapabilityDefinition? selected = null;
            foreach (var definition in available)
            {
                if (acceptedVersions.Accepts(definition.Key.Version) &&
                    (selected == null || definition.Key.Version.CompareTo(selected.Key.Version) > 0))
                {
                    selected = definition;
                }
            }

            if (selected == null)
            {
                var versions = new List<object?>();
                foreach (var definition in available)
                {
                    versions.Add(definition.Key.Version.ToString());
                }

                return Failure(
                    "contract.unsupported_version",
                    "No discovered contract version satisfies the requested version range.",
                    "$.version",
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["requested"] = acceptedVersions.ToString(),
                        ["available"] = versions.AsReadOnly()
                    },
                    false,
                    "Negotiate one of the versions returned by system.describe.");
            }

            if (selected.RequestSchema == null)
            {
                return InternalFailure("contract.missing_request_schema", "Accepted canonical definition has no request schema.");
            }

            var portableRequestIssues = PortableData.Validate(request);
            if (portableRequestIssues.Count != 0)
            {
                var first = portableRequestIssues[0];
                return Failure(
                    "contract.invalid_request",
                    first.Message,
                    first.Path,
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["schemaCode"] = first.Code,
                        ["capability"] = selected.Key.ToString()
                    },
                    false,
                    "Use only JSON-compatible portable canonical request data.");
            }

            var requestIssues = selected.RequestSchema.ValidateValue(request);
            if (requestIssues.Count != 0)
            {
                var first = requestIssues[0];
                return Failure(
                    "contract.invalid_request",
                    first.Message,
                    first.Path,
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["schemaCode"] = first.Code,
                        ["capability"] = selected.Key.ToString()
                    },
                    false,
                    "Repair the request so it conforms to the discovered requestSchema.");
            }

            if (!_routesByKey.TryGetValue(selected.Key, out var route))
            {
                return InternalFailure("contract.route_missing", "Accepted canonical capability has no implementation binding.");
            }

            var result = route.Handler.Invoke(new CapabilityInvocationContext(this, selected), request);
            if (result.Success)
            {
                if (result.Data == null || selected.SuccessSchema == null)
                {
                    return InternalFailure("contract.invalid_result", "Capability returned success without a schema-valid result payload.");
                }

                if (PortableData.Validate(result.Data).Count != 0)
                {
                    return InternalFailure("contract.invalid_result", "Capability success payload contained non-portable canonical data.");
                }

                var resultIssues = selected.SuccessSchema.ValidateValue(result.Data);
                if (resultIssues.Count != 0)
                {
                    return InternalFailure("contract.invalid_result", "Capability success payload violated its canonical success schema.");
                }

                return result;
            }

            if (result.Error == null || selected.ErrorSchema == null)
            {
                return InternalFailure("contract.invalid_error", "Capability returned failure without a canonical structured error.");
            }

            var errorData = result.Error.ToData();
            if (PortableData.Validate(errorData).Count != 0)
            {
                return InternalFailure("contract.invalid_error", "Capability error payload contained non-portable canonical data.");
            }

            var errorIssues = selected.ErrorSchema.ValidateValue(errorData);
            if (errorIssues.Count != 0)
            {
                return InternalFailure("contract.invalid_error", "Capability error payload violated the canonical structured error schema.");
            }

            return result;
        }

        private static CapabilityInvocationResult InternalFailure(string code, string message)
        {
            return Failure(
                code,
                message,
                "$",
                new Dictionary<string, object?>(StringComparer.Ordinal),
                false,
                "This is a canonical runtime invariant failure; do not retry unchanged.");
        }

        private static CapabilityInvocationResult Failure(
            string code,
            string message,
            string path,
            IReadOnlyDictionary<string, object?> context,
            bool retryable,
            string repairHint)
        {
            return CapabilityInvocationResult.Failed(new StructuredError(
                code,
                message,
                path,
                context,
                retryable,
                repairHint));
        }

        private static int CompareDefinitions(CapabilityDefinition left, CapabilityDefinition right)
        {
            var name = string.Compare(left.Key.Name, right.Key.Name, StringComparison.Ordinal);
            return name != 0 ? name : left.Key.Version.CompareTo(right.Key.Version);
        }
    }

    [PublicCapabilityRoute("arkus.base", "system.describe", "1.0")]
    public sealed class SystemDescribeHandler : ICanonicalCapabilityHandler
    {
        public CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return CapabilityInvocationResult.Succeeded(context.Contract.Projection.ToData());
        }
    }

    public static class BaseContract
    {
        public static CanonicalProviderContribution CreateContribution()
        {
            var provider = new ProviderMetadata("arkus.base", ProviderKind.Base, "base", "system");
            var definition = new CapabilityDefinition(
                new CapabilityKey("system.describe", new ContractVersion(1, 0)),
                provider,
                CanonicalContractSchemas.EmptyObject(),
                CanonicalContractSchemas.SystemDescribeSuccess(),
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.ReadOnly,
                DeterminismClass.Deterministic,
                System.Array.Empty<string>(),
                new[] { "returns-the-complete-composed-canonical-inventory" },
                new ConcurrencySemantics(ConcurrencyClass.ParallelSafe),
                new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Unsupported),
                new RepairSemantics(false, true),
                new PolicySemantics(
                    PrivilegeClass.PublicRead,
                    TransactionRequirement.ReadOnlyEnvelope,
                    ProvenanceRequirement.Required),
                new CostSemantics(1, "single canonical inventory projection"));

            return new CanonicalProviderContribution(
                new ProviderDescriptor("arkus.base", ProviderKind.Base, "base", new[] { "system" }),
                new[] { definition },
                new[] { CapabilityRoute.FromHandler(new SystemDescribeHandler()) });
        }

        public static ComposedContract Compose()
        {
            var result = ContractComposer.Compose(CreateContribution());
            if (!result.Success || result.Contract == null)
            {
                throw new InvalidOperationException("The built-in base canonical contract must compose successfully.");
            }

            return result.Contract;
        }
    }
}
