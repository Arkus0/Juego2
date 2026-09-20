using System;
using System.Collections.Generic;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Runtime
{
    /// <summary>
    /// Production H0 host-capability boundary. H0 is intentionally local and engine-neutral:
    /// canonical reads/authoring/rebase/replay are admitted, while generic external host effects
    /// and elevated host privileges require a later reviewed policy owned by a future milestone.
    /// </summary>
    public static class H0HostCapabilityPolicy
    {
        public const string ExternalEffectCode = "host-policy.external-effect-forbidden";
        public const string ElevatedPrivilegeCode = "host-policy.elevated-privilege-forbidden";
        public const string MetadataMismatchCode = "host-policy.metadata-mismatch";

        public static ComposedContract Enforce(ComposedContract contract)
        {
            if (contract == null) throw new ArgumentNullException(nameof(contract));

            var issues = Validate(contract.Definitions);
            if (issues.Count != 0)
            {
                var first = issues[0];
                throw new InvalidOperationException(
                    "H0 host capability policy rejected " + first.Capability +
                    " (" + first.Code + "): " + first.Message);
            }

            return contract;
        }

        public static IReadOnlyList<H0HostCapabilityPolicyIssue> Validate(
            IEnumerable<CapabilityDefinition> definitions)
        {
            if (definitions == null) throw new ArgumentNullException(nameof(definitions));

            var issues = new List<H0HostCapabilityPolicyIssue>();
            foreach (var definition in definitions)
            {
                if (definition == null)
                {
                    issues.Add(new H0HostCapabilityPolicyIssue(
                        MetadataMismatchCode,
                        "<null>",
                        "The effective H0 inventory may not contain a null capability definition."));
                    continue;
                }

                if (definition.SideEffect == SideEffectClass.ExternalReversible ||
                    definition.SideEffect == SideEffectClass.ExternalIrreversible)
                {
                    issues.Add(new H0HostCapabilityPolicyIssue(
                        ExternalEffectCode,
                        definition.Key.ToString(),
                        "H0 exposes no generic external host-side-effect authority."));
                }

                var policy = definition.Policy;
                if (policy == null)
                {
                    issues.Add(new H0HostCapabilityPolicyIssue(
                        MetadataMismatchCode,
                        definition.Key.ToString(),
                        "An effective H0 capability must declare canonical policy semantics."));
                    continue;
                }

                if (policy.Privilege == PrivilegeClass.Elevated)
                {
                    issues.Add(new H0HostCapabilityPolicyIssue(
                        ElevatedPrivilegeCode,
                        definition.Key.ToString(),
                        "H0 exposes no elevated host privilege through its canonical inventory."));
                }

                var expectedTransaction = ExpectedTransaction(definition.SideEffect);
                if (expectedTransaction == TransactionRequirement.Unknown ||
                    policy.TransactionRequirement != expectedTransaction)
                {
                    issues.Add(new H0HostCapabilityPolicyIssue(
                        MetadataMismatchCode,
                        definition.Key.ToString(),
                        "Declared side effects and transaction policy do not describe the same effective authority."));
                }
            }

            return issues.AsReadOnly();
        }

        private static TransactionRequirement ExpectedTransaction(SideEffectClass sideEffect)
        {
            switch (sideEffect)
            {
                case SideEffectClass.None:
                case SideEffectClass.ReadOnly:
                    return TransactionRequirement.ReadOnlyEnvelope;
                case SideEffectClass.CanonicalMutation:
                    return TransactionRequirement.CanonicalTransaction;
                case SideEffectClass.CanonicalRebase:
                    return TransactionRequirement.CanonicalRebase;
                case SideEffectClass.CanonicalReplay:
                    return TransactionRequirement.CanonicalReplay;
                default:
                    return TransactionRequirement.Unknown;
            }
        }
    }

    public sealed class H0HostCapabilityPolicyIssue
    {
        public H0HostCapabilityPolicyIssue(string code, string capability, string message)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Capability = capability ?? throw new ArgumentNullException(nameof(capability));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public string Code { get; }
        public string Capability { get; }
        public string Message { get; }
    }
}
