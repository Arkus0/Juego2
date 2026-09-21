using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Arkus.EngineBridge
{
    public enum ProjectionDriftState
    {
        Absent = 0,
        InSync = 1,
        CanonicalAhead = 2,
        EngineDrift = 3,
        MissingDependency = 4,
        Ambiguous = 5,
        Failed = 6
    }

    public enum ReferenceFailurePoint
    {
        None = 0,
        BeforeValidation = 1,
        BeforePublication = 2
    }

    public sealed class CanonicalProjectionAnchor
    {
        public CanonicalProjectionAnchor(string worldId, long revision, string stateHash)
        {
            WorldId = Require(worldId, nameof(worldId));
            if (revision < 0) throw new ArgumentOutOfRangeException(nameof(revision));
            Revision = revision;
            StateHash = Require(stateHash, nameof(stateHash));
        }

        public string WorldId { get; }
        public long Revision { get; }
        public string StateHash { get; }

        internal string Normalized => StableEncoding.Fields(WorldId, Revision.ToString(System.Globalization.CultureInfo.InvariantCulture), StateHash);

        private static string Require(string value, string name) =>
            string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("A non-empty value is required.", name) : value;
    }

    public sealed class BridgeToolchainProfile
    {
        public BridgeToolchainProfile(string bridgeId, string bridgeVersion, string toolchainFingerprint, string platform)
        {
            BridgeId = Require(bridgeId, nameof(bridgeId));
            BridgeVersion = Require(bridgeVersion, nameof(bridgeVersion));
            ToolchainFingerprint = Require(toolchainFingerprint, nameof(toolchainFingerprint));
            Platform = Require(platform, nameof(platform));
        }

        public string BridgeId { get; }
        public string BridgeVersion { get; }
        public string ToolchainFingerprint { get; }
        public string Platform { get; }
        public string Fingerprint => StableEncoding.Hash(StableEncoding.Fields(BridgeId, BridgeVersion, ToolchainFingerprint, Platform));

        internal string Normalized => StableEncoding.Fields(BridgeId, BridgeVersion, ToolchainFingerprint, Platform);

        private static string Require(string value, string name) =>
            string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("A non-empty value is required.", name) : value;
    }

    public sealed class ProjectionInput
    {
        public const string ContractVersion = "arkus.engine-projection@1";
        private readonly byte[] _canonicalSnapshot;

        public ProjectionInput(
            CanonicalProjectionAnchor canonicalAnchor,
            string bindingVersion,
            string catalogueFingerprint,
            BridgeToolchainProfile profile,
            byte[] canonicalSnapshot)
        {
            CanonicalAnchor = canonicalAnchor ?? throw new ArgumentNullException(nameof(canonicalAnchor));
            BindingVersion = Require(bindingVersion, nameof(bindingVersion));
            CatalogueFingerprint = Require(catalogueFingerprint, nameof(catalogueFingerprint));
            Profile = profile ?? throw new ArgumentNullException(nameof(profile));
            if (canonicalSnapshot == null) throw new ArgumentNullException(nameof(canonicalSnapshot));
            _canonicalSnapshot = (byte[])canonicalSnapshot.Clone();
            CanonicalSnapshotDigest = StableEncoding.Hash(_canonicalSnapshot);
            InputDigest = StableEncoding.Hash(StableEncoding.Fields(
                ContractVersion,
                CanonicalAnchor.Normalized,
                BindingVersion,
                CatalogueFingerprint,
                Profile.Normalized,
                CanonicalSnapshotDigest));
        }

        public CanonicalProjectionAnchor CanonicalAnchor { get; }
        public string BindingVersion { get; }
        public string CatalogueFingerprint { get; }
        public BridgeToolchainProfile Profile { get; }
        public string CanonicalSnapshotDigest { get; }
        public string InputDigest { get; }
        public byte[] GetCanonicalSnapshotCopy() => (byte[])_canonicalSnapshot.Clone();

        private static string Require(string value, string name) =>
            string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("A non-empty value is required.", name) : value;
    }

    public sealed class ProjectionResource
    {
        public ProjectionResource(
            string resourceId,
            string? parentResourceId,
            string resourceKind,
            string contentDigest,
            IEnumerable<string>? logicalDependencies = null)
        {
            ResourceId = Require(resourceId, nameof(resourceId));
            ParentResourceId = string.IsNullOrWhiteSpace(parentResourceId) ? null : parentResourceId;
            ResourceKind = Require(resourceKind, nameof(resourceKind));
            ContentDigest = Require(contentDigest, nameof(contentDigest));
            LogicalDependencies = new ReadOnlyCollection<string>((logicalDependencies ?? Array.Empty<string>())
                .Select(value => Require(value, nameof(logicalDependencies)))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToArray());
        }

        public string ResourceId { get; }
        public string? ParentResourceId { get; }
        public string ResourceKind { get; }
        public string ContentDigest { get; }
        public IReadOnlyList<string> LogicalDependencies { get; }

        internal string Normalized => StableEncoding.Fields(
            ResourceId,
            ParentResourceId ?? string.Empty,
            ResourceKind,
            ContentDigest,
            StableEncoding.Sequence(LogicalDependencies));

        private static string Require(string value, string name) =>
            string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("A non-empty value is required.", name) : value;
    }

    public sealed class ProjectionPlan
    {
        internal ProjectionPlan(ProjectionInput input, IReadOnlyList<ProjectionResource> resources)
        {
            Input = input;
            Resources = resources;
            var resourceShape = StableEncoding.Sequence(resources.Select(value => value.Normalized));
            PlanDigest = StableEncoding.Hash(StableEncoding.Fields(ProjectionInput.ContractVersion, input.InputDigest, resourceShape));
            GenerationId = "gen-" + StableEncoding.Hash(StableEncoding.Fields(input.InputDigest, PlanDigest)).Substring(0, 24);
        }

        public ProjectionInput Input { get; }
        public IReadOnlyList<ProjectionResource> Resources { get; }
        public string PlanDigest { get; }
        public string GenerationId { get; }
    }

    public sealed class ProjectionObservation
    {
        internal ProjectionObservation(ProjectionDriftState state, IEnumerable<string> diagnostics, string effectiveDigest)
        {
            State = state;
            Diagnostics = new ReadOnlyCollection<string>(diagnostics.OrderBy(value => value, StringComparer.Ordinal).ToArray());
            EffectiveDigest = effectiveDigest;
            ObservationDigest = StableEncoding.Hash(StableEncoding.Fields(
                StateToken(state),
                StableEncoding.Sequence(Diagnostics),
                effectiveDigest));
        }

        public ProjectionDriftState State { get; }
        public IReadOnlyList<string> Diagnostics { get; }
        public string EffectiveDigest { get; }
        public string ObservationDigest { get; }

        public static string StateToken(ProjectionDriftState state)
        {
            switch (state)
            {
                case ProjectionDriftState.Absent: return "absent";
                case ProjectionDriftState.InSync: return "in-sync";
                case ProjectionDriftState.CanonicalAhead: return "canonical-ahead";
                case ProjectionDriftState.EngineDrift: return "engine-drift";
                case ProjectionDriftState.MissingDependency: return "missing-dependency";
                case ProjectionDriftState.Ambiguous: return "ambiguous";
                case ProjectionDriftState.Failed: return "failed";
                default: throw new ArgumentOutOfRangeException(nameof(state));
            }
        }
    }

    public sealed class ProjectionReceipt
    {
        internal ProjectionReceipt(ProjectionPlan plan, ProjectionObservation observation, bool published)
        {
            ContractVersion = ProjectionInput.ContractVersion;
            CanonicalAnchor = plan.Input.CanonicalAnchor.Normalized;
            CanonicalSnapshotDigest = plan.Input.CanonicalSnapshotDigest;
            BindingVersion = plan.Input.BindingVersion;
            CatalogueFingerprint = plan.Input.CatalogueFingerprint;
            BridgeProfileFingerprint = plan.Input.Profile.Fingerprint;
            InputDigest = plan.Input.InputDigest;
            PlanDigest = plan.PlanDigest;
            GenerationId = plan.GenerationId;
            ObservationDigest = observation.ObservationDigest;
            Published = published;
            ReceiptDigest = StableEncoding.Hash(StableEncoding.Fields(
                ContractVersion,
                CanonicalAnchor,
                CanonicalSnapshotDigest,
                BindingVersion,
                CatalogueFingerprint,
                BridgeProfileFingerprint,
                InputDigest,
                PlanDigest,
                GenerationId,
                ObservationDigest,
                published ? "published" : "not-published"));
        }

        public string ContractVersion { get; }
        public string CanonicalAnchor { get; }
        public string CanonicalSnapshotDigest { get; }
        public string BindingVersion { get; }
        public string CatalogueFingerprint { get; }
        public string BridgeProfileFingerprint { get; }
        public string InputDigest { get; }
        public string PlanDigest { get; }
        public string GenerationId { get; }
        public string ObservationDigest { get; }
        public bool Published { get; }
        public string ReceiptDigest { get; }
    }

    public sealed class MaterializationResult
    {
        internal MaterializationResult(ProjectionReceipt receipt, ProjectionObservation observation, bool semanticChange)
        {
            Receipt = receipt;
            Observation = observation;
            SemanticChange = semanticChange;
        }

        public ProjectionReceipt Receipt { get; }
        public ProjectionObservation Observation { get; }
        public bool SemanticChange { get; }
    }

    public static class ProjectionPlanner
    {
        public static ProjectionPlan Create(ProjectionInput input, IEnumerable<ProjectionResource> resources)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (resources == null) throw new ArgumentNullException(nameof(resources));

            var normalized = resources
                .Select(value => value ?? throw new ArgumentException("Resource inventory contains null.", nameof(resources)))
                .OrderBy(value => value.ResourceId, StringComparer.Ordinal)
                .ToArray();

            for (var index = 1; index < normalized.Length; index++)
            {
                if (StringComparer.Ordinal.Equals(normalized[index - 1].ResourceId, normalized[index].ResourceId))
                    throw new ArgumentException("Projection resource IDs must be unique.", nameof(resources));
            }

            var ids = new HashSet<string>(normalized.Select(value => value.ResourceId), StringComparer.Ordinal);
            foreach (var resource in normalized)
            {
                if (resource.ParentResourceId != null && !ids.Contains(resource.ParentResourceId))
                    throw new ArgumentException("Projection parent must exist in the same plan.", nameof(resources));
            }

            return new ProjectionPlan(input, new ReadOnlyCollection<ProjectionResource>(normalized));
        }
    }

    public sealed class ReferenceMaterializer
    {
        private ProjectionPlan? _activePlan;
        private ProjectionReceipt? _activeReceipt;
        private string? _lastFailedInputDigest;

        public string? ActiveGenerationId => _activePlan?.GenerationId;
        public ProjectionReceipt? ActiveReceipt => _activeReceipt;

        public MaterializationResult Materialize(
            ProjectionPlan plan,
            IEnumerable<string> availableLogicalDependencies,
            ReferenceFailurePoint failurePoint = ReferenceFailurePoint.None)
        {
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            var available = DependencySet(availableLogicalDependencies);

            if (_activePlan != null && StringComparer.Ordinal.Equals(_activePlan.GenerationId, plan.GenerationId))
            {
                var existing = Observe(plan, ToObserved(_activePlan.Resources), available);
                if (existing.State == ProjectionDriftState.InSync && _activeReceipt != null)
                    return new MaterializationResult(_activeReceipt, existing, false);
            }

            if (failurePoint == ReferenceFailurePoint.BeforeValidation)
                return Fail(plan, "reference.failure.before-validation");

            var staged = ToObserved(plan.Resources);
            var stagedObservation = Evaluate(plan, plan, staged, available, null);
            if (stagedObservation.State != ProjectionDriftState.InSync)
            {
                _lastFailedInputDigest = plan.Input.InputDigest;
                var rejected = new ProjectionReceipt(plan, stagedObservation, false);
                return new MaterializationResult(rejected, stagedObservation, false);
            }

            if (failurePoint == ReferenceFailurePoint.BeforePublication)
                return Fail(plan, "reference.failure.before-publication");

            var receipt = new ProjectionReceipt(plan, stagedObservation, true);
            _activePlan = plan;
            _activeReceipt = receipt;
            _lastFailedInputDigest = null;
            return new MaterializationResult(receipt, stagedObservation, true);
        }

        public ProjectionObservation Observe(
            ProjectionPlan expectedPlan,
            IEnumerable<ProjectionResource>? effectiveResources,
            IEnumerable<string> availableLogicalDependencies)
        {
            if (expectedPlan == null) throw new ArgumentNullException(nameof(expectedPlan));
            var effective = effectiveResources == null
                ? (_activePlan == null ? Array.Empty<ProjectionResource>() : ToObserved(_activePlan.Resources))
                : effectiveResources.ToArray();
            var available = DependencySet(availableLogicalDependencies);
            return Evaluate(expectedPlan, _activePlan, effective, available, _lastFailedInputDigest);
        }

        private MaterializationResult Fail(ProjectionPlan plan, string diagnostic)
        {
            _lastFailedInputDigest = plan.Input.InputDigest;
            var observation = new ProjectionObservation(
                ProjectionDriftState.Failed,
                new[] { diagnostic },
                EffectiveDigest(Array.Empty<ProjectionResource>()));
            return new MaterializationResult(new ProjectionReceipt(plan, observation, false), observation, false);
        }

        private static ProjectionObservation Evaluate(
            ProjectionPlan expected,
            ProjectionPlan? active,
            IEnumerable<ProjectionResource> effectiveResources,
            HashSet<string> availableDependencies,
            string? failedInputDigest)
        {
            var effective = effectiveResources.ToArray();
            var effectiveDigest = EffectiveDigest(effective);

            if (failedInputDigest != null && StringComparer.Ordinal.Equals(failedInputDigest, expected.Input.InputDigest))
                return new ProjectionObservation(ProjectionDriftState.Failed, new[] { "projection.failed" }, effectiveDigest);

            var missing = expected.Resources
                .SelectMany(value => value.LogicalDependencies)
                .Distinct(StringComparer.Ordinal)
                .Where(value => !availableDependencies.Contains(value))
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToArray();
            if (missing.Length > 0)
                return new ProjectionObservation(
                    ProjectionDriftState.MissingDependency,
                    missing.Select(value => "missing-dependency:" + value),
                    effectiveDigest);

            var duplicate = effective
                .GroupBy(value => value.ResourceId, StringComparer.Ordinal)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToArray();
            if (duplicate.Length > 0)
                return new ProjectionObservation(
                    ProjectionDriftState.Ambiguous,
                    duplicate.Select(value => "ambiguous-resource:" + value),
                    effectiveDigest);

            if (active == null)
                return new ProjectionObservation(ProjectionDriftState.Absent, Array.Empty<string>(), effectiveDigest);

            if (!StringComparer.Ordinal.Equals(active.Input.InputDigest, expected.Input.InputDigest) ||
                !StringComparer.Ordinal.Equals(active.PlanDigest, expected.PlanDigest))
                return new ProjectionObservation(
                    ProjectionDriftState.CanonicalAhead,
                    new[] { "active-generation-does-not-match-expected-input" },
                    effectiveDigest);

            var expectedShape = StableEncoding.Sequence(expected.Resources.Select(value => value.Normalized));
            var effectiveShape = StableEncoding.Sequence(effective
                .OrderBy(value => value.ResourceId, StringComparer.Ordinal)
                .Select(value => value.Normalized));
            if (!StringComparer.Ordinal.Equals(expectedShape, effectiveShape))
                return new ProjectionObservation(
                    ProjectionDriftState.EngineDrift,
                    new[] { "effective-managed-resources-differ" },
                    effectiveDigest);

            return new ProjectionObservation(ProjectionDriftState.InSync, Array.Empty<string>(), effectiveDigest);
        }

        private static ProjectionResource[] ToObserved(IEnumerable<ProjectionResource> resources) =>
            resources.Select(value => new ProjectionResource(
                value.ResourceId,
                value.ParentResourceId,
                value.ResourceKind,
                value.ContentDigest,
                value.LogicalDependencies)).ToArray();

        private static HashSet<string> DependencySet(IEnumerable<string> dependencies)
        {
            if (dependencies == null) throw new ArgumentNullException(nameof(dependencies));
            return new HashSet<string>(dependencies, StringComparer.Ordinal);
        }

        private static string EffectiveDigest(IEnumerable<ProjectionResource> resources) =>
            StableEncoding.Hash(StableEncoding.Sequence(resources
                .OrderBy(value => value.ResourceId, StringComparer.Ordinal)
                .Select(value => value.Normalized)));
    }

    internal static class StableEncoding
    {
        internal static string Fields(params string[] values) => Sequence(values);

        internal static string Sequence(IEnumerable<string> values)
        {
            var builder = new StringBuilder();
            foreach (var value in values)
            {
                var current = value ?? string.Empty;
                builder.Append(current.Length.ToString(System.Globalization.CultureInfo.InvariantCulture));
                builder.Append(':');
                builder.Append(current);
                builder.Append(';');
            }
            return builder.ToString();
        }

        internal static string Hash(string value) => Hash(Encoding.UTF8.GetBytes(value));

        internal static string Hash(byte[] value)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(value);
            var builder = new StringBuilder(bytes.Length * 2);
            foreach (var item in bytes)
                builder.Append(item.ToString("x2", System.Globalization.CultureInfo.InvariantCulture));
            return builder.ToString();
        }
    }
}
