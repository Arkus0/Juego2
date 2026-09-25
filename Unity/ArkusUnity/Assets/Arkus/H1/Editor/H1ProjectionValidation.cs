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

    // H1-08 owns only composition, stable Unity diagnostic identity and one genuinely new finite-
    // transform check. The actions registered here delegate causal validity to H1-04/05/06/07.
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
            Descriptor("unity.scene.managed-marker", PostMaterialization, 120),
            Descriptor("unity.scene.prefab-link", PostMaterialization, 130),
            Descriptor("unity.scene.component", PostMaterialization, 140),
            Descriptor("unity.scene.effective-observation", PostMaterialization, 150),
            Descriptor("unity.scene.plan-observation", PostMaterialization, 160)
        };

        private static readonly IReadOnlyDictionary<string, H1ValidationInvariantDescriptor> RegisteredById =
            Registered.ToDictionary(value => value.invariantId, value => value, StringComparer.Ordinal);

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
                .ThenBy(value => value.LogicalAsset, StringComparer.Ordinal)
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
                    diagnostics.Add(new H1ValidationDiagnostic
                    {
                        code = error.Message,
                        severity = "error",
                        invariantId = check.InvariantId,
                        phase = phase,
                        canonicalResource = check.CanonicalResource,
                        logicalAsset = check.LogicalAsset,
                        managedPath = check.ManagedPath,
                        context = check.Context
                    });
                }
            }

            diagnostics.Sort(CompareDiagnostics);
            return Result(phase, scope, inputDigest,
                executed.OrderBy(value => OrderOf(value, phaseInventory)).ThenBy(value => value, StringComparer.Ordinal).ToArray(),
                diagnostics.ToArray());
        }

        // Single expected-invalidity boundary for work that must happen before/around the registered
        // checks (for example reading the active manifest). Only canonical projection.* invalidity is
        // converted; unexpected editor/runtime exceptions remain visible to the outer editor-failure path.
        internal static H1ValidationResult GuardExpectedInvalidity(
            string phase,
            string scope,
            string inputDigest,
            string invariantId,
            string canonicalResource,
            string logicalAsset,
            string managedPath,
            string context,
            Func<H1ValidationResult> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (!RegisteredById.TryGetValue(invariantId ?? string.Empty, out var descriptor) || descriptor.phase != phase)
                throw new InvalidOperationException("H1 validation boundary is not registered for phase: " + invariantId);

            try
            {
                return action();
            }
            catch (InvalidDataException error) when (IsProjectionCode(error.Message))
            {
                return Result(phase, scope, inputDigest,
                    new[] { invariantId },
                    new[]
                    {
                        new H1ValidationDiagnostic
                        {
                            code = error.Message,
                            severity = "error",
                            invariantId = invariantId,
                            phase = phase,
                            canonicalResource = canonicalResource ?? string.Empty,
                            logicalAsset = logicalAsset ?? string.Empty,
                            managedPath = managedPath ?? string.Empty,
                            context = context ?? string.Empty
                        }
                    });
            }
        }

        internal static void ValidateFiniteTransforms(string scenePath)
        {
            if (string.IsNullOrEmpty(scenePath)) throw new InvalidDataException("projection.active-scene-missing");

            var fault = Path.Combine(H1Bootstrap.ProjectRoot(), "Library", "Arkus", "H1Projection", "inject-non-finite-transform");
            var injectNonFiniteSample = File.Exists(fault);
            var injected = false;

            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            foreach (var root in scene.GetRootGameObjects())
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
            {
                var position = transform.localPosition;
                if (injectNonFiniteSample && !injected)
                {
                    position = new Vector3(float.NaN, position.y, position.z);
                    injected = true;
                }
                ValidateFiniteTransformValues(position, transform.localScale, transform.localRotation);
            }
        }

        internal static void ValidateFiniteTransformValues(Vector3 position, Vector3 scale, Quaternion rotation)
        {
            if (!Finite(position) || !Finite(scale) || !Finite(rotation))
                throw new InvalidDataException("projection.non-finite-transform");
        }

        private static H1ValidationResult Result(
            string phase,
            string scope,
            string inputDigest,
            string[] executed,
            H1ValidationDiagnostic[] diagnostics)
        {
            var orderedDiagnostics = diagnostics ?? new H1ValidationDiagnostic[0];
            Array.Sort(orderedDiagnostics, CompareDiagnostics);
            return new H1ValidationResult
            {
                schemaId = ResultSchema,
                inventorySchemaId = InventorySchema,
                phase = phase,
                scope = scope ?? string.Empty,
                inputDigest = inputDigest ?? string.Empty,
                valid = orderedDiagnostics.Length == 0,
                executedInvariantIds = executed ?? new string[0],
                diagnostics = orderedDiagnostics
            };
        }

        private static bool Finite(Vector3 value) => Finite(value.x) && Finite(value.y) && Finite(value.z);
        private static bool Finite(Quaternion value) => Finite(value.x) && Finite(value.y) && Finite(value.z) && Finite(value.w);
        private static bool Finite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);

        private static bool IsProjectionCode(string value) =>
            !string.IsNullOrEmpty(value) && value.StartsWith("projection.", StringComparison.Ordinal);

        private static int CompareDiagnostics(H1ValidationDiagnostic left, H1ValidationDiagnostic right)
        {
            var byOrder = OrderOf(left.invariantId, RegisteredById).CompareTo(OrderOf(right.invariantId, RegisteredById));
            if (byOrder != 0) return byOrder;
            var byCode = StringComparer.Ordinal.Compare(left.code, right.code);
            if (byCode != 0) return byCode;
            var byResource = StringComparer.Ordinal.Compare(left.canonicalResource, right.canonicalResource);
            if (byResource != 0) return byResource;
            var byAsset = StringComparer.Ordinal.Compare(left.logicalAsset, right.logicalAsset);
            if (byAsset != 0) return byAsset;
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
