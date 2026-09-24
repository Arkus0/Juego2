using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;

namespace Arkus.Harness.Projection
{
    public enum UnityHostResourceClass
    {
        Unknown = 0,
        ManagedAsset = 1,
        ProjectMetadata = 2
    }

    public enum UnityHostTimeClass
    {
        Unknown = 0,
        BoundedRead = 1,
        BoundedEditorEffect = 2
    }

    /// <summary>
    /// Project authority is bootstrapped by the host. Public protocol requests never choose these
    /// roots and only carry logical resource references inside the reviewed workspace.
    /// </summary>
    public sealed class UnityProjectWorkspaceAuthority
    {
        public const string ProjectIdentity = "arkus.unity-project@1:ArkusUnity";
        public const string ProjectRoot = "Unity/ArkusUnity";
        public const string ManagedAssetsRootId = "assets.arkus";
        public const string ProjectSettingsRootId = "project.settings";

        private readonly IReadOnlyDictionary<string, string> _managedRoots;

        private UnityProjectWorkspaceAuthority()
        {
            _managedRoots = new ReadOnlyDictionary<string, string>(
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    [ManagedAssetsRootId] = "Assets/Arkus",
                    [ProjectSettingsRootId] = "ProjectSettings"
                });
        }

        public string Identity => ProjectIdentity;
        public string Root => ProjectRoot;
        public IReadOnlyDictionary<string, string> ManagedRoots => _managedRoots;

        public static UnityProjectWorkspaceAuthority ForArkusUnityProject() =>
            new UnityProjectWorkspaceAuthority();

        public bool ContainsManagedRoot(string rootId) =>
            rootId != null && _managedRoots.ContainsKey(rootId);
    }

    /// <summary>
    /// Reviewed H1 metadata that binds one canonical capability to a project-local resource class
    /// and a bounded execution-time class. It does not contain a host path, process or endpoint.
    /// </summary>
    public sealed class UnityHostCapabilityGrant
    {
        public UnityHostCapabilityGrant(
            CapabilityKey capability,
            string managedRootId,
            string logicalReferenceNamespace,
            UnityHostResourceClass resourceClass,
            UnityHostTimeClass timeClass)
        {
            Capability = capability ?? throw new ArgumentNullException(nameof(capability));
            ManagedRootId = string.IsNullOrWhiteSpace(managedRootId)
                ? throw new ArgumentException("A reviewed managed-root identity is required.", nameof(managedRootId))
                : managedRootId;
            LogicalReferenceNamespace = string.IsNullOrWhiteSpace(logicalReferenceNamespace)
                ? throw new ArgumentException("A logical resource namespace is required.", nameof(logicalReferenceNamespace))
                : logicalReferenceNamespace;
            ResourceClass = resourceClass;
            TimeClass = timeClass;
        }

        public CapabilityKey Capability { get; }
        public string ManagedRootId { get; }
        public string LogicalReferenceNamespace { get; }
        public UnityHostResourceClass ResourceClass { get; }
        public UnityHostTimeClass TimeClass { get; }
    }

    public sealed class H1UnityHostPolicyIssue
    {
        public H1UnityHostPolicyIssue(string code, string capability, string message)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Capability = capability ?? throw new ArgumentNullException(nameof(capability));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public string Code { get; }
        public string Capability { get; }
        public string Message { get; }
    }

    /// <summary>
    /// H1-only host admission boundary. Non-Unity capabilities must still satisfy the accepted H0
    /// policy. Unity Editor capabilities use a separate truthful profile: read-only or externally
    /// reversible, Authoring privilege, no canonical transaction claim, required provenance,
    /// serialized execution, reviewed project roots and closed logical-resource schemas.
    /// </summary>
    public static class H1UnityHostCapabilityPolicy
    {
        public const string PolicyId = "arkus.unity-host-policy@1";
        public const string HostProviderId = "arkus.unity-host";
        public const string HostScope = "unity-host";
        public const string HostNamespace = "unity.host";

        public const string MissingGrantCode = "unity-host-policy.missing-grant";
        public const string OrphanGrantCode = "unity-host-policy.orphan-grant";
        public const string MetadataMismatchCode = "unity-host-policy.metadata-mismatch";
        public const string ResourceBoundaryCode = "unity-host-policy.resource-boundary";
        public const string AmbientAuthorityCode = "unity-host-policy.ambient-authority";
        public const string H0BoundaryCode = "unity-host-policy.h0-boundary";

        public static IReadOnlyList<H1UnityHostPolicyIssue> Validate(
            ComposedContract contract,
            UnityProjectWorkspaceAuthority workspace,
            IEnumerable<UnityHostCapabilityGrant> grants)
        {
            if (contract == null) throw new ArgumentNullException(nameof(contract));
            if (workspace == null) throw new ArgumentNullException(nameof(workspace));
            if (grants == null) throw new ArgumentNullException(nameof(grants));

            var issues = new List<H1UnityHostPolicyIssue>();
            var byCapability = new Dictionary<CapabilityKey, UnityHostCapabilityGrant>();
            foreach (var grant in grants)
            {
                if (grant == null) throw new ArgumentException("H1 grants may not contain null entries.", nameof(grants));
                if (!byCapability.TryAdd(grant.Capability, grant))
                {
                    issues.Add(new H1UnityHostPolicyIssue(
                        MetadataMismatchCode,
                        grant.Capability.ToString(),
                        "A Unity host capability may have exactly one reviewed H1 grant."));
                }
            }

            var consumed = new HashSet<CapabilityKey>();
            foreach (var definition in contract.Definitions)
            {
                if (string.Equals(definition.Provider.ProviderId, HostProviderId, StringComparison.Ordinal))
                {
                    ValidateUnityHostDefinition(definition, workspace, byCapability, consumed, issues);
                    continue;
                }

                foreach (var h0Issue in H0HostCapabilityPolicy.Validate(new[] { definition }))
                {
                    issues.Add(new H1UnityHostPolicyIssue(
                        H0BoundaryCode,
                        definition.Key.ToString(),
                        "Non-Unity capability must remain admissible under H0: " + h0Issue.Code + "."));
                }
            }

            foreach (var grant in byCapability.Values)
            {
                if (!consumed.Contains(grant.Capability))
                {
                    issues.Add(new H1UnityHostPolicyIssue(
                        OrphanGrantCode,
                        grant.Capability.ToString(),
                        "Reviewed H1 metadata may not mint authority absent from the composed canonical inventory."));
                }
            }

            return issues.AsReadOnly();
        }

        public static NeutralProjectionService CreateProjection(
            ComposedContract contract,
            UnityProjectWorkspaceAuthority workspace,
            IEnumerable<UnityHostCapabilityGrant> grants)
        {
            if (grants == null) throw new ArgumentNullException(nameof(grants));
            var grantSnapshot = SnapshotGrants(grants);
            var issues = Validate(contract, workspace, grantSnapshot);
            if (issues.Count != 0)
            {
                var first = issues[0];
                throw new InvalidOperationException(
                    "H1 Unity host capability policy rejected " + first.Capability +
                    " (" + first.Code + "): " + first.Message);
            }

            return new NeutralProjectionService(
                new H1AdmittedUnityContract(contract, workspace, grantSnapshot));
        }

        private static IReadOnlyList<UnityHostCapabilityGrant> SnapshotGrants(
            IEnumerable<UnityHostCapabilityGrant> grants)
        {
            var snapshot = new List<UnityHostCapabilityGrant>();
            foreach (var grant in grants)
            {
                snapshot.Add(grant ?? throw new ArgumentException(
                    "H1 grants may not contain null entries.", nameof(grants)));
            }

            return snapshot.AsReadOnly();
        }

        private static void ValidateUnityHostDefinition(
            CapabilityDefinition definition,
            UnityProjectWorkspaceAuthority workspace,
            IReadOnlyDictionary<CapabilityKey, UnityHostCapabilityGrant> grants,
            ISet<CapabilityKey> consumed,
            IList<H1UnityHostPolicyIssue> issues)
        {
            var identity = definition.Key.ToString();
            if (definition.Provider.Kind != ProviderKind.Scoped ||
                !string.Equals(definition.Provider.Scope, HostScope, StringComparison.Ordinal) ||
                !string.Equals(definition.Provider.CapabilityNamespace, HostNamespace, StringComparison.Ordinal))
            {
                issues.Add(new H1UnityHostPolicyIssue(
                    MetadataMismatchCode,
                    identity,
                    "Unity host capabilities must use the reviewed scoped provider identity and namespace."));
            }

            if (definition.SideEffect != SideEffectClass.ReadOnly &&
                definition.SideEffect != SideEffectClass.ExternalReversible)
            {
                issues.Add(new H1UnityHostPolicyIssue(
                    MetadataMismatchCode,
                    identity,
                    "H1 admits only truthful read-only or externally reversible Unity Editor effects."));
            }

            var policy = definition.Policy;
            if (policy == null ||
                policy.Privilege != PrivilegeClass.Authoring ||
                policy.TransactionRequirement != TransactionRequirement.None ||
                policy.ProvenanceRequirement != ProvenanceRequirement.Required)
            {
                issues.Add(new H1UnityHostPolicyIssue(
                    MetadataMismatchCode,
                    identity,
                    "Unity host effects require Authoring privilege, no canonical transaction claim and required provenance."));
            }

            if (definition.Determinism != DeterminismClass.EnvironmentDependent ||
                definition.Concurrency == null ||
                definition.Concurrency.Class != ConcurrencyClass.Serialized)
            {
                issues.Add(new H1UnityHostPolicyIssue(
                    MetadataMismatchCode,
                    identity,
                    "Editor-host capabilities must truthfully declare environment dependence and serialized execution."));
            }

            if (!grants.TryGetValue(definition.Key, out var grant))
            {
                issues.Add(new H1UnityHostPolicyIssue(
                    MissingGrantCode,
                    identity,
                    "The composed Unity host capability has no reviewed H1 resource/time grant."));
                return;
            }

            consumed.Add(definition.Key);
            if (!workspace.ContainsManagedRoot(grant.ManagedRootId) ||
                grant.ResourceClass == UnityHostResourceClass.Unknown ||
                grant.TimeClass == UnityHostTimeClass.Unknown)
            {
                issues.Add(new H1UnityHostPolicyIssue(
                    ResourceBoundaryCode,
                    identity,
                    "Unity authority must resolve to one bootstrapped reviewed project root and explicit resource/time classes."));
            }

            if ((definition.SideEffect == SideEffectClass.ReadOnly && grant.TimeClass != UnityHostTimeClass.BoundedRead) ||
                (definition.SideEffect == SideEffectClass.ExternalReversible && grant.TimeClass != UnityHostTimeClass.BoundedEditorEffect))
            {
                issues.Add(new H1UnityHostPolicyIssue(
                    MetadataMismatchCode,
                    identity,
                    "The declared side effect and bounded time class disagree."));
            }

            ValidateRequestSchema(definition, grant, issues);
        }

        private static void ValidateRequestSchema(
            CapabilityDefinition definition,
            UnityHostCapabilityGrant grant,
            IList<H1UnityHostPolicyIssue> issues)
        {
            var identity = definition.Key.ToString();
            if (definition.RequestSchema == null || definition.RequestSchema.Root.ValueType != SchemaValueType.Object)
            {
                issues.Add(new H1UnityHostPolicyIssue(
                    AmbientAuthorityCode,
                    identity,
                    "Unity host requests require a closed canonical object schema."));
                return;
            }

            var logicalReferenceFound = false;
            ValidateRequestNode(definition.RequestSchema.Root, grant, identity, "$", ref logicalReferenceFound, issues);
            if (grant.ResourceClass == UnityHostResourceClass.ManagedAsset && !logicalReferenceFound)
            {
                issues.Add(new H1UnityHostPolicyIssue(
                    ResourceBoundaryCode,
                    identity,
                    "Managed-asset authority must be selected by a reviewed Arkus logical resource reference."));
            }
        }

        private static void ValidateRequestNode(
            SchemaNode node,
            UnityHostCapabilityGrant grant,
            string identity,
            string path,
            ref bool logicalReferenceFound,
            IList<H1UnityHostPolicyIssue> issues)
        {
            if (node.ValueType == SchemaValueType.Any)
            {
                issues.Add(new H1UnityHostPolicyIssue(
                    AmbientAuthorityCode,
                    identity,
                    "Unbounded request data is forbidden at " + path + "."));
                return;
            }

            if (node.ValueType == SchemaValueType.Object)
            {
                if (node.AdditionalPropertiesAllowed)
                {
                    issues.Add(new H1UnityHostPolicyIssue(
                        AmbientAuthorityCode,
                        identity,
                        "Open-ended request objects are forbidden at " + path + "."));
                }

                foreach (var pair in node.Properties)
                    ValidateRequestNode(pair.Value, grant, identity, path + "/" + pair.Key, ref logicalReferenceFound, issues);
                return;
            }

            if (node.ValueType == SchemaValueType.Array)
            {
                if (node.Items != null)
                    ValidateRequestNode(node.Items, grant, identity, path + "/*", ref logicalReferenceFound, issues);
                return;
            }

            if (node.ValueType != SchemaValueType.String)
                return;

            if (string.Equals(node.Format, "arkus-logical-reference", StringComparison.Ordinal))
            {
                if (!string.Equals(node.LogicalReferenceNamespace, grant.LogicalReferenceNamespace, StringComparison.Ordinal))
                {
                    issues.Add(new H1UnityHostPolicyIssue(
                        ResourceBoundaryCode,
                        identity,
                        "Logical resource namespace is outside the reviewed H1 grant at " + path + "."));
                }
                else
                {
                    logicalReferenceFound = true;
                }
                return;
            }

            if (string.Equals(node.Format, "uri", StringComparison.Ordinal) || node.AllowedStringValues.Count == 0)
            {
                issues.Add(new H1UnityHostPolicyIssue(
                    AmbientAuthorityCode,
                    identity,
                    "Free-form strings and endpoints are forbidden in Unity host authority requests at " + path +
                    "; use a reviewed logical resource reference or bounded enum."));
            }
        }
    }

    /// <summary>
    /// Opaque result of one successful H1 admission. The exact bootstrapped workspace and reviewed
    /// per-capability grants travel with the composed contract so H1-03A can build the fixed
    /// project-bound invocation envelope from the same admission decision instead of reconstructing
    /// authority from transport data or a second registry.
    /// </summary>
    internal sealed class H1AdmittedUnityContract
    {
        private readonly IReadOnlyDictionary<CapabilityKey, UnityHostCapabilityGrant> _grants;

        internal H1AdmittedUnityContract(
            ComposedContract contract,
            UnityProjectWorkspaceAuthority workspace,
            IEnumerable<UnityHostCapabilityGrant> grants)
        {
            Contract = contract ?? throw new ArgumentNullException(nameof(contract));
            Workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
            if (grants == null) throw new ArgumentNullException(nameof(grants));

            var snapshot = new Dictionary<CapabilityKey, UnityHostCapabilityGrant>();
            foreach (var grant in grants)
            {
                if (grant == null)
                    throw new ArgumentException("H1 grants may not contain null entries.", nameof(grants));
                snapshot.Add(grant.Capability, grant);
            }

            _grants = new ReadOnlyDictionary<CapabilityKey, UnityHostCapabilityGrant>(snapshot);
        }

        internal ComposedContract Contract { get; }
        internal UnityProjectWorkspaceAuthority Workspace { get; }
        internal IReadOnlyDictionary<CapabilityKey, UnityHostCapabilityGrant> Grants => _grants;
    }
}
