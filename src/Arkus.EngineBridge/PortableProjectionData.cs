using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Arkus.EngineBridge
{
    /// <summary>
    /// Stable portable projections of the engine-neutral H1 contract. These records intentionally
    /// contain only strings, integers, booleans, null, dictionaries and ordered lists; engine/native
    /// objects and canonical mutable stores never cross this boundary.
    /// </summary>
    public static class ProjectionPortableData
    {
        public static IReadOnlyDictionary<string, object?> Anchor(CanonicalProjectionAnchor anchor)
        {
            if (anchor == null) throw new ArgumentNullException(nameof(anchor));
            return Map(
                ("worldId", anchor.WorldId),
                ("revision", anchor.Revision),
                ("stateHash", anchor.StateHash));
        }

        public static IReadOnlyDictionary<string, object?> Profile(BridgeToolchainProfile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            return Map(
                ("bridgeId", profile.BridgeId),
                ("bridgeVersion", profile.BridgeVersion),
                ("toolchainFingerprint", profile.ToolchainFingerprint),
                ("platform", profile.Platform),
                ("profileFingerprint", profile.Fingerprint));
        }

        public static IReadOnlyDictionary<string, object?> Input(ProjectionInput input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            return Map(
                ("contractVersion", ProjectionInput.ContractVersion),
                ("canonicalAnchor", Anchor(input.CanonicalAnchor)),
                ("bindingVersion", input.BindingVersion),
                ("catalogueFingerprint", input.CatalogueFingerprint),
                ("bridgeProfile", Profile(input.Profile)),
                ("canonicalSnapshotDigest", input.CanonicalSnapshotDigest),
                ("inputDigest", input.InputDigest));
        }

        public static IReadOnlyDictionary<string, object?> Resource(ProjectionResource resource)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            return Map(
                ("resourceId", resource.ResourceId),
                ("parentResourceId", resource.ParentResourceId),
                ("resourceKind", resource.ResourceKind),
                ("contentDigest", resource.ContentDigest),
                ("logicalDependencies", List(resource.LogicalDependencies.Cast<object?>())));
        }

        public static IReadOnlyDictionary<string, object?> Plan(ProjectionPlan plan)
        {
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            return Map(
                ("contractVersion", ProjectionInput.ContractVersion),
                ("input", Input(plan.Input)),
                ("resources", List(plan.Resources.Select(value => (object?)Resource(value)))),
                ("planDigest", plan.PlanDigest),
                ("generationId", plan.GenerationId));
        }

        public static IReadOnlyDictionary<string, object?> Observation(ProjectionObservation observation)
        {
            if (observation == null) throw new ArgumentNullException(nameof(observation));
            return Map(
                ("state", ProjectionObservation.StateToken(observation.State)),
                ("diagnostics", List(observation.Diagnostics.Cast<object?>())),
                ("effectiveDigest", observation.EffectiveDigest),
                ("observationDigest", observation.ObservationDigest));
        }

        /// <summary>
        /// Portable bridge receipt. Diagnostics are copied from the exact normalized observation
        /// whose digest is anchored by the receipt; mismatched pairs fail closed.
        /// </summary>
        public static IReadOnlyDictionary<string, object?> Receipt(
            ProjectionReceipt receipt,
            ProjectionObservation observation)
        {
            if (receipt == null) throw new ArgumentNullException(nameof(receipt));
            if (observation == null) throw new ArgumentNullException(nameof(observation));
            if (!StringComparer.Ordinal.Equals(receipt.ObservationDigest, observation.ObservationDigest))
                throw new ArgumentException("Receipt and observation digests do not match.", nameof(observation));

            return Map(
                ("contractVersion", receipt.ContractVersion),
                ("canonicalAnchor", receipt.CanonicalAnchor),
                ("canonicalSnapshotDigest", receipt.CanonicalSnapshotDigest),
                ("bindingVersion", receipt.BindingVersion),
                ("catalogueFingerprint", receipt.CatalogueFingerprint),
                ("bridgeProfileFingerprint", receipt.BridgeProfileFingerprint),
                ("inputDigest", receipt.InputDigest),
                ("planDigest", receipt.PlanDigest),
                ("generationId", receipt.GenerationId),
                ("observationDigest", receipt.ObservationDigest),
                ("diagnostics", List(observation.Diagnostics.Cast<object?>())),
                ("publicationStatus", receipt.Published ? "published" : "not-published"),
                ("receiptDigest", receipt.ReceiptDigest));
        }

        public static IReadOnlyDictionary<string, object?> Materialization(MaterializationResult result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            return Map(
                ("receipt", Receipt(result.Receipt, result.Observation)),
                ("observation", Observation(result.Observation)),
                ("semanticChange", result.SemanticChange));
        }

        private static IReadOnlyDictionary<string, object?> Map(params (string Key, object? Value)[] entries)
        {
            var data = new Dictionary<string, object?>(StringComparer.Ordinal);
            foreach (var entry in entries)
                data.Add(entry.Key, entry.Value);
            return new ReadOnlyDictionary<string, object?>(data);
        }

        private static IReadOnlyList<object?> List(IEnumerable<object?> values) =>
            new ReadOnlyCollection<object?>(values.ToArray());
    }
}
