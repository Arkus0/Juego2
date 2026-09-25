using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Arkus.H1.Editor
{
    [Serializable]
    public sealed class H1ValidationInvariantDescriptor
    {
        public string invariantId;
        public string phase;
        public int order;
    }

    [Serializable]
    public sealed class H1ValidationDiagnostic
    {
        public string code;
        public string severity;
        public string invariantId;
        public string phase;
        public string canonicalResource;
        public string logicalAsset;
        public string managedPath;
        public string context;
    }

    [Serializable]
    public sealed class H1ValidationResult
    {
        public string schemaId;
        public string inventorySchemaId;
        public string phase;
        public string scope;
        public string inputDigest;
        public bool valid;
        public string[] executedInvariantIds;
        public H1ValidationDiagnostic[] diagnostics;
    }

    internal sealed class H1ValidationCheck
    {
        internal string InvariantId;
        internal string CanonicalResource;
        internal string LogicalAsset;
        internal string ManagedPath;
        internal string Context;
        internal Action Action;
    }

    internal sealed class H1ValidationFailureException : Exception
    {
        internal H1ValidationFailureException(H1ValidationResult result)
            : base(result != null && result.diagnostics != null && result.diagnostics.Length != 0
                ? result.diagnostics[0].code
                : "projection.validation-failed")
        {
            Result = result;
        }

        internal H1ValidationResult Result { get; }
    }

    // H1-08 owns composition and deterministic Unity diagnostics. Causal validity remains in
    // H1-04/05/06/07 validators; this registry only gives those checks stable phase/actionability.
    public static class H1ProjectionValidation
    {
        public const string ResultSchema = "arkus.h1-unity-validation-result@1";
        public const string InventorySchema = "arkus.h1-unity-validation-inventory@1";
        public const string Preflight = "preflight";
        public const string PostMaterialization = "post-materialization";

        private static readonly H1ValidationInvariantDescriptor[] Registered =
        {
            Descriptor("unity.catalogue.snapshot", Preflight, 10),
            Descriptor("unity.component.adapter-inventory", Preflight, 20),
            Descriptor("unity.plan.node-shape", Preflight, 30),
            Descriptor("unity.plan.source-binding", Preflight, 40),
            Descriptor("unity.plan.component", Preflight, 50),
            Descriptor("unity.plan.hierarchy", Preflight, 60),
            Descriptor("unity.scene.finite-transform", PostMaterialization, 110),
            Descriptor("unity.scene.effective-observation", PostMaterialization, 120),
            Descriptor("unity.scene.managed-marker", PostMaterialization, 121),
            Descriptor("unity.scene.prefab-link", PostMaterialization, 122),
            Descriptor("unity.scene.component", PostMaterialization, 123),
            Descriptor("unity.scene.plan-observation", PostMaterialization, 130)
        };

        public static H1ValidationInvariantDescriptor[] Inventory()
        {
            return Registered.Select(value => new H1ValidationInvariantDescriptor
            {
                invariantId = value.invariantId,
                phase = value.phase,
                order = value.order
            }).ToArray();
        }

        public static string InventoryJson()
        {
            return JsonUtility.ToJson(new InventoryEnvelope
            {
                schemaId = InventorySchema,
                invariants = Inventory()
            });
        }

        internal static H1ValidationCheck Check(
            string invariantId,
            string canonicalResource,
            string logicalAsset,
            string managedPath,
            string context,
            Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return new H1ValidationCheck
            {
                InvariantId = invariantId ?? string.Empty,
                CanonicalResource = canonicalResource ?? string.Empty,
                LogicalAsset = logicalAsset ?? string.Empty,
                ManagedPath = managedPath ?? string.Empty,
                Context = context ?? string.Empty,
                Action = action
            };
        }

        internal static H1ValidationResult Run(
            string phase,
            string scope,
            string inputDigest,
            IEnumerable<H1ValidationCheck> checks)
        {
            if (phase != Preflight && phase != PostMaterialization)
                throw new ArgumentException("Unknown H1 validation phase.", nameof(phase));
            if (checks == null) throw new ArgumentNullException(nameof(checks));

            var phaseInventory = Registered.Where(value => value.phase == phase)
                .ToDictionary(value => value.invariantId, value => value, StringComparer.Ordinal);
            var diagnostics = new List<H1ValidationDiagnostic>();
            var executed = new HashSet<string>(StringComparer.Ordinal);

            foreach (var check in checks
                .OrderBy(value => OrderOf(value.InvariantId, phaseInventory))
                .ThenBy(value => value.InvariantId, StringComparer.Ordinal)
                .ThenBy(value => value.CanonicalResource, StringComparer.Ordinal)
                .ThenBy(value => value.ManagedPath, StringComparer.Ordinal))
            {
                if (!phaseInventory.ContainsKey(check.InvariantId))
                    throw new InvalidOperationException("H1 validation check is not registered for phase: " + check.InvariantId);
                executed.Add(check.InvariantId);
                try
                {
                    check.Action();
                }
                catch (InvalidDataException error) when (IsProjectionCode(error.Message))
                {
                    var invariantId = MapInvariant(error.Message, check.InvariantId, phase);
                    diagnostics.Add(new H1ValidationDiagnostic
                    {
                        code = error.Message,
                        severity = "error",
                        invariantId = invariantId,
                        phase = phase,
                        canonicalResource = check.CanonicalResource,
                        logicalAsset = check.LogicalAsset,
                        managedPath = check.ManagedPath,
                        context = check.Context
                    });
                }
            }

            diagnostics.Sort(CompareDiagnostics);
            return new H1ValidationResult
            {
                schemaId = ResultSchema,
                inventorySchemaId = InventorySchema,
                phase = phase,
                scope = scope ?? string.Empty,
                inputDigest = inputDigest ?? string.Empty,
                valid = diagnostics.Count == 0,
                executedInvariantIds = executed.OrderBy(value => OrderOf(value, phaseInventory))
                    .ThenBy(value => value, StringComparer.Ordinal).ToArray(),
                diagnostics = diagnostics.ToArray()
            };
        }

        internal static void ValidateFiniteTransforms(string scenePath)
        {
            if (string.IsNullOrEmpty(scenePath)) throw new InvalidDataException("projection.active-scene-missing");
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            foreach (var root in scene.GetRootGameObjects())
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
            {
                if (!Finite(transform.localPosition) || !Finite(transform.localScale) || !Finite(transform.localRotation))
                    throw new InvalidDataException("projection.non-finite-transform");
            }
        }

        private static bool Finite(Vector3 value) => Finite(value.x) && Finite(value.y) && Finite(value.z);
        private static bool Finite(Quaternion value) => Finite(value.x) && Finite(value.y) && Finite(value.z) && Finite(value.w);
        private static bool Finite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);

        private static bool IsProjectionCode(string value) =>
            !string.IsNullOrEmpty(value) && value.StartsWith("projection.", StringComparison.Ordinal);

        private static string MapInvariant(string code, string fallback, string phase)
        {
            if (phase != PostMaterialization) return fallback;
            if (code.IndexOf("marker", StringComparison.Ordinal) >= 0 || code == "projection.root-count")
                return "unity.scene.managed-marker";
            if (code.IndexOf("prefab", StringComparison.Ordinal) >= 0 || code.IndexOf("lineage", StringComparison.Ordinal) >= 0)
                return "unity.scene.prefab-link";
            if (code.IndexOf("component", StringComparison.Ordinal) >= 0)
                return "unity.scene.component";
            return fallback;
        }

        private static int CompareDiagnostics(H1ValidationDiagnostic left, H1ValidationDiagnostic right)
        {
            var byOrder = OrderOf(left.invariantId, Registered.ToDictionary(value => value.invariantId, value => value, StringComparer.Ordinal))
                .CompareTo(OrderOf(right.invariantId, Registered.ToDictionary(value => value.invariantId, value => value, StringComparer.Ordinal)));
            if (byOrder != 0) return byOrder;
            var byCode = StringComparer.Ordinal.Compare(left.code, right.code);
            if (byCode != 0) return byCode;
            var byResource = StringComparer.Ordinal.Compare(left.canonicalResource, right.canonicalResource);
            if (byResource != 0) return byResource;
            var byPath = StringComparer.Ordinal.Compare(left.managedPath, right.managedPath);
            if (byPath != 0) return byPath;
            return StringComparer.Ordinal.Compare(left.context, right.context);
        }

        private static int OrderOf(string invariantId, IReadOnlyDictionary<string, H1ValidationInvariantDescriptor> inventory)
        {
            return invariantId != null && inventory.TryGetValue(invariantId, out var descriptor)
                ? descriptor.order
                : int.MaxValue;
        }

        private static H1ValidationInvariantDescriptor Descriptor(string id, string phase, int order) =>
            new H1ValidationInvariantDescriptor { invariantId = id, phase = phase, order = order };

        [Serializable]
        private sealed class InventoryEnvelope
        {
            public string schemaId;
            public H1ValidationInvariantDescriptor[] invariants;
        }
    }
}
