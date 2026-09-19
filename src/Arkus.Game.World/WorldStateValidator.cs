using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Arkus.Game.World
{
    public sealed class WorldInvariantDefinition
    {
        public WorldInvariantDefinition(string invariantId, string machineCode, string category)
        {
            InvariantId = invariantId ?? throw new ArgumentNullException(nameof(invariantId));
            MachineCode = machineCode ?? throw new ArgumentNullException(nameof(machineCode));
            Category = category ?? throw new ArgumentNullException(nameof(category));
        }

        public string InvariantId { get; }
        public string MachineCode { get; }
        public string Category { get; }
    }

    /// <summary>
    /// Authoritative, validator-independent universe of invariants owned by the finite H0
    /// micro-world model. Validator registration is reconciled against this catalog by HK05.
    /// </summary>
    public static class WorldInvariantCatalog
    {
        public const string CatalogVersion = "arkus.world-invariants/v1";

        public static readonly WorldInvariantDefinition WorldIdInitialized =
            Define("arkus.world.identity.initialized/v1", "world.invalid_id", "structural");
        public static readonly WorldInvariantDefinition SchemaSupported =
            Define("arkus.world.schema.supported/v1", "world.unsupported_schema", "structural");
        public static readonly WorldInvariantDefinition RevisionNonNegative =
            Define("arkus.world.revision.nonnegative/v1", "world.invalid_revision", "structural");
        public static readonly WorldInvariantDefinition ObjectEntryPresent =
            Define("arkus.world.object.entry-present/v1", "world.invalid_object", "structural");
        public static readonly WorldInvariantDefinition ObjectIdentityUnique =
            Define("arkus.world.object.identity-unique/v1", "world.duplicate_id", "identity");
        public static readonly WorldInvariantDefinition ContainerResolves =
            Define("arkus.world.object.container-resolves/v1", "world.dangling_container", "reference");
        public static readonly WorldInvariantDefinition ContainerNotSelf =
            Define("arkus.world.object.container-not-self/v1", "world.containment_cycle", "reference");
        public static readonly WorldInvariantDefinition ReferenceEntryPresent =
            Define("arkus.world.object.reference-entry-present/v1", "world.invalid_reference", "structural");
        public static readonly WorldInvariantDefinition ReferenceUnique =
            Define("arkus.world.object.reference-unique/v1", "world.duplicate_reference", "identity");
        public static readonly WorldInvariantDefinition ReferenceTargetResolves =
            Define("arkus.world.object.reference-target-resolves/v1", "world.dangling_reference", "reference");
        public static readonly WorldInvariantDefinition ContainmentAcyclic =
            Define("arkus.world.object.containment-acyclic/v1", "world.containment_cycle", "reference");
        public static readonly WorldInvariantDefinition ExtensionEntryPresent =
            Define("arkus.world.extension.entry-present/v1", "world.invalid_extension", "structural");
        public static readonly WorldInvariantDefinition ExtensionIdentityUnique =
            Define("arkus.world.extension.identity-unique/v1", "world.duplicate_extension", "identity");
        public static readonly WorldInvariantDefinition ExtensionSubjectResolves =
            Define("arkus.world.extension.subject-resolves/v1", "world.dangling_extension_subject", "reference");
        public static readonly WorldInvariantDefinition ExtensionDependencyEntryPresent =
            Define("arkus.world.extension.dependency-entry-present/v1", "world.invalid_extension_dependency", "structural");
        public static readonly WorldInvariantDefinition ExtensionDependencyUnique =
            Define("arkus.world.extension.dependency-unique/v1", "world.duplicate_extension_dependency", "identity");
        public static readonly WorldInvariantDefinition ExtensionDependencyTargetResolves =
            Define("arkus.world.extension.dependency-target-resolves/v1", "world.dangling_extension_dependency", "reference");

        private static readonly IReadOnlyList<WorldInvariantDefinition> Definitions =
            new List<WorldInvariantDefinition>
            {
                WorldIdInitialized,
                SchemaSupported,
                RevisionNonNegative,
                ObjectEntryPresent,
                ObjectIdentityUnique,
                ContainerResolves,
                ContainerNotSelf,
                ReferenceEntryPresent,
                ReferenceUnique,
                ReferenceTargetResolves,
                ContainmentAcyclic,
                ExtensionEntryPresent,
                ExtensionIdentityUnique,
                ExtensionSubjectResolves,
                ExtensionDependencyEntryPresent,
                ExtensionDependencyUnique,
                ExtensionDependencyTargetResolves
            }.AsReadOnly();

        public static IReadOnlyList<WorldInvariantDefinition> All => Definitions;

        private static WorldInvariantDefinition Define(string invariantId, string machineCode, string category)
        {
            return new WorldInvariantDefinition(invariantId, machineCode, category);
        }
    }

    public sealed class WorldStateCandidate
    {
        private readonly IReadOnlyList<WorldObject?> _objects;
        private readonly IReadOnlyList<WorldExtensionData?> _extensions;

        public WorldStateCandidate(
            WorldId id,
            long revision,
            IEnumerable<WorldObject?> objects,
            IEnumerable<WorldExtensionData?>? extensions = null,
            int schemaVersion = WorldState.CurrentSchemaVersion)
        {
            if (objects is null) throw new ArgumentNullException(nameof(objects));

            Id = id;
            Revision = revision;
            SchemaVersion = schemaVersion;
            _objects = new List<WorldObject?>(objects).AsReadOnly();
            _extensions = new List<WorldExtensionData?>(extensions ?? Array.Empty<WorldExtensionData?>()).AsReadOnly();
        }

        public int SchemaVersion { get; }
        public long Revision { get; }
        public WorldId Id { get; }
        public IReadOnlyList<WorldObject?> Objects => _objects;
        public IReadOnlyList<WorldExtensionData?> Extensions => _extensions;

        public static WorldStateCandidate FromState(WorldState state)
        {
            if (state is null) throw new ArgumentNullException(nameof(state));
            return new WorldStateCandidate(state.Id, state.Revision, state.Objects, state.Extensions, state.SchemaVersion);
        }
    }

    public sealed class WorldStateViolation
    {
        public WorldStateViolation(
            WorldInvariantDefinition invariant,
            string resource,
            string path,
            string message,
            IReadOnlyDictionary<string, string>? remediationContext = null)
        {
            Invariant = invariant ?? throw new ArgumentNullException(nameof(invariant));
            Resource = resource ?? throw new ArgumentNullException(nameof(resource));
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            RemediationContext = remediationContext == null
                ? new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal))
                : new ReadOnlyDictionary<string, string>(
                    new Dictionary<string, string>(remediationContext, StringComparer.Ordinal));
        }

        public WorldInvariantDefinition Invariant { get; }
        public string Resource { get; }
        public string Path { get; }
        public string Message { get; }
        public IReadOnlyDictionary<string, string> RemediationContext { get; }
    }

    public static class WorldStateValidator
    {
        private delegate void Validator(WorldStateCandidate candidate, IList<WorldStateViolation> violations);

        private sealed class Registration
        {
            public Registration(WorldInvariantDefinition invariant, Validator validator)
            {
                Invariant = invariant;
                Validator = validator;
            }

            public WorldInvariantDefinition Invariant { get; }
            public Validator Validator { get; }
        }

        private static readonly IReadOnlyList<Registration> Registrations =
            new List<Registration>
            {
                Register(WorldInvariantCatalog.WorldIdInitialized, ValidateWorldId),
                Register(WorldInvariantCatalog.SchemaSupported, ValidateSchema),
                Register(WorldInvariantCatalog.RevisionNonNegative, ValidateRevision),
                Register(WorldInvariantCatalog.ObjectEntryPresent, ValidateObjectEntries),
                Register(WorldInvariantCatalog.ObjectIdentityUnique, ValidateObjectIdentity),
                Register(WorldInvariantCatalog.ContainerResolves, ValidateContainersResolve),
                Register(WorldInvariantCatalog.ContainerNotSelf, ValidateContainersNotSelf),
                Register(WorldInvariantCatalog.ReferenceEntryPresent, ValidateReferenceEntries),
                Register(WorldInvariantCatalog.ReferenceUnique, ValidateReferenceUniqueness),
                Register(WorldInvariantCatalog.ReferenceTargetResolves, ValidateReferenceTargets),
                Register(WorldInvariantCatalog.ContainmentAcyclic, ValidateContainmentAcyclic),
                Register(WorldInvariantCatalog.ExtensionEntryPresent, ValidateExtensionEntries),
                Register(WorldInvariantCatalog.ExtensionIdentityUnique, ValidateExtensionIdentity),
                Register(WorldInvariantCatalog.ExtensionSubjectResolves, ValidateExtensionSubjects),
                Register(WorldInvariantCatalog.ExtensionDependencyEntryPresent, ValidateExtensionDependencyEntries),
                Register(WorldInvariantCatalog.ExtensionDependencyUnique, ValidateExtensionDependencyUniqueness),
                Register(WorldInvariantCatalog.ExtensionDependencyTargetResolves, ValidateExtensionDependencyTargets)
            }.AsReadOnly();

        public static IReadOnlyList<WorldInvariantDefinition> RegisteredInvariants
        {
            get
            {
                var values = new List<WorldInvariantDefinition>();
                for (var index = 0; index < Registrations.Count; index++) values.Add(Registrations[index].Invariant);
                return values.AsReadOnly();
            }
        }

        public static IReadOnlyList<WorldStateViolation> Validate(WorldState state)
        {
            if (state is null) throw new ArgumentNullException(nameof(state));
            return ValidateCandidate(WorldStateCandidate.FromState(state));
        }

        public static IReadOnlyList<WorldStateViolation> ValidateCandidate(WorldStateCandidate candidate)
        {
            if (candidate is null) throw new ArgumentNullException(nameof(candidate));
            var violations = new List<WorldStateViolation>();
            for (var index = 0; index < Registrations.Count; index++) Registrations[index].Validator(candidate, violations);
            return violations.AsReadOnly();
        }

        public static void ValidateOrThrow(WorldState state)
        {
            var violations = Validate(state);
            if (violations.Count == 0) return;
            Throw(violations[0]);
        }

        internal static void ValidateCandidateOrThrow(WorldStateCandidate candidate)
        {
            var violations = ValidateCandidate(candidate);
            if (violations.Count == 0) return;
            Throw(violations[0]);
        }

        private static void Throw(WorldStateViolation violation)
        {
            throw new WorldStateException(violation.Invariant.MachineCode, violation.Message, violation.Path);
        }

        private static Registration Register(WorldInvariantDefinition invariant, Validator validator) =>
            new Registration(invariant, validator);

        private static void ValidateWorldId(WorldStateCandidate candidate, IList<WorldStateViolation> violations)
        {
            if (!string.IsNullOrEmpty(candidate.Id.Value)) return;
            Add(violations, WorldInvariantCatalog.WorldIdInitialized, "world", "$/id",
                "World ID must be initialized.", Context("action", "provide-world-id"));
        }

        private static void ValidateSchema(WorldStateCandidate candidate, IList<WorldStateViolation> violations)
        {
            if (candidate.SchemaVersion == WorldState.CurrentSchemaVersion) return;
            Add(violations, WorldInvariantCatalog.SchemaSupported, WorldResource(candidate), "$/schemaVersion",
                "Unsupported world schema version.", Context(
                    "expected", WorldState.CurrentSchemaVersion.ToString(CultureInfo.InvariantCulture),
                    "actual", candidate.SchemaVersion.ToString(CultureInfo.InvariantCulture)));
        }

        private static void ValidateRevision(WorldStateCandidate candidate, IList<WorldStateViolation> violations)
        {
            if (candidate.Revision >= 0) return;
            Add(violations, WorldInvariantCatalog.RevisionNonNegative, WorldResource(candidate), "$/revision",
                "World revision cannot be negative.", Context("minimum", "0"));
        }

        private static void ValidateObjectEntries(WorldStateCandidate candidate, IList<WorldStateViolation> violations)
        {
            for (var index = 0; index < candidate.Objects.Count; index++)
            {
                if (candidate.Objects[index] != null) continue;
                Add(violations, WorldInvariantCatalog.ObjectEntryPresent,
                    "world.object@index:" + Invariant(index), "$/objects/" + Invariant(index),
                    "World object entries cannot be null.", Context("action", "remove-or-replace-entry"));
            }
        }

        private static void ValidateObjectIdentity(WorldStateCandidate candidate, IList<WorldStateViolation> violations)
        {
            var firstIndex = new Dictionary<WorldObjectId, int>();
            for (var index = 0; index < candidate.Objects.Count; index++)
            {
                var current = candidate.Objects[index];
                if (current == null) continue;
                if (firstIndex.TryGetValue(current.Id, out var original))
                {
                    Add(violations, WorldInvariantCatalog.ObjectIdentityUnique, ObjectResource(current),
                        "$/objects/" + current.Id.Value,
                        "World object IDs must be unique.", Context(
                            "firstIndex", Invariant(original), "duplicateIndex", Invariant(index)));
                }
                else
                {
                    firstIndex.Add(current.Id, index);
                }
            }
        }

        private static void ValidateContainersResolve(WorldStateCandidate candidate, IList<WorldStateViolation> violations)
        {
            var ids = ObjectIds(candidate);
            ForEachObject(candidate, current =>
            {
                if (current.ContainerId.HasValue && !ids.Contains(current.ContainerId.Value))
                {
                    Add(violations, WorldInvariantCatalog.ContainerResolves, ObjectResource(current),
                        ObjectPath(current, "container"),
                        "Container IDs must resolve inside the same world state.",
                        Context("targetId", current.ContainerId.Value.Value));
                }
            });
        }

        private static void ValidateContainersNotSelf(WorldStateCandidate candidate, IList<WorldStateViolation> violations)
        {
            ForEachObject(candidate, current =>
            {
                if (current.ContainerId.HasValue && current.ContainerId.Value == current.Id)
                {
                    Add(violations, WorldInvariantCatalog.ContainerNotSelf, ObjectResource(current),
                        ObjectPath(current, "container"), "An object cannot contain itself.",
                        Context("targetId", current.Id.Value));
                }
            });
        }

        private static void ValidateReferenceEntries(WorldStateCandidate candidate, IList<WorldStateViolation> violations)
        {
            ForEachObject(candidate, current =>
            {
                for (var index = 0; index < current.References.Count; index++)
                {
                    if (current.References[index] != null) continue;
                    Add(violations, WorldInvariantCatalog.ReferenceEntryPresent, ObjectResource(current),
                        ObjectPath(current, "references/" + Invariant(index)),
                        "Reference entries cannot be null.", Context("action", "remove-or-replace-entry"));
                }
            });
        }

        private static void ValidateReferenceUniqueness(WorldStateCandidate candidate, IList<WorldStateViolation> violations)
        {
            ForEachObject(candidate, current =>
            {
                var seen = new HashSet<WorldReference>();
                for (var index = 0; index < current.References.Count; index++)
                {
                    var reference = current.References[index];
                    if (reference == null || seen.Add(reference)) continue;
                    Add(violations, WorldInvariantCatalog.ReferenceUnique, ObjectResource(current),
                        ObjectPath(current, "references/" + Invariant(index)),
                        "Duplicate reference edges are not semantic state.", Context(
                            "kind", reference.Kind.Value, "targetId", reference.TargetId.Value));
                }
            });
        }

        private static void ValidateReferenceTargets(WorldStateCandidate candidate, IList<WorldStateViolation> violations)
        {
            var ids = ObjectIds(candidate);
            ForEachObject(candidate, current =>
            {
                for (var index = 0; index < current.References.Count; index++)
                {
                    var reference = current.References[index];
                    if (reference == null || ids.Contains(reference.TargetId)) continue;
                    Add(violations, WorldInvariantCatalog.ReferenceTargetResolves, ObjectResource(current),
                        ObjectPath(current, "references/" + Invariant(index)),
                        "Reference targets must resolve inside the same world state.", Context(
                            "kind", reference.Kind.Value, "targetId", reference.TargetId.Value));
                }
            });
        }

        private static void ValidateContainmentAcyclic(WorldStateCandidate candidate, IList<WorldStateViolation> violations)
        {
            var byId = ObjectsById(candidate);
            ForEachObject(candidate, origin =>
            {
                var seen = new HashSet<WorldObjectId> { origin.Id };
                var current = origin;
                while (current.ContainerId.HasValue)
                {
                    var parentId = current.ContainerId.Value;
                    if (!byId.TryGetValue(parentId, out var parent)) break;
                    if (!seen.Add(parentId))
                    {
                        Add(violations, WorldInvariantCatalog.ContainmentAcyclic, ObjectResource(origin),
                            ObjectPath(origin, "container"),
                            "Containment edges must form an acyclic forest.", Context("cycleAt", parentId.Value));
                        break;
                    }
                    current = parent;
                }
            });
        }

        private static void ValidateExtensionEntries(WorldStateCandidate candidate, IList<WorldStateViolation> violations)
        {
            for (var index = 0; index < candidate.Extensions.Count; index++)
            {
                if (candidate.Extensions[index] != null) continue;
                Add(violations, WorldInvariantCatalog.ExtensionEntryPresent,
                    "world.extension@index:" + Invariant(index), "$/extensions/" + Invariant(index),
                    "Extension entries cannot be null.", Context("action", "remove-or-replace-entry"));
            }
        }

        private static void ValidateExtensionIdentity(WorldStateCandidate candidate, IList<WorldStateViolation> violations)
        {
            var firstIndex = new Dictionary<WorldExtensionIdentity, int>();
            for (var index = 0; index < candidate.Extensions.Count; index++)
            {
                var current = candidate.Extensions[index];
                if (current == null) continue;
                if (firstIndex.TryGetValue(current.Identity, out var original))
                {
                    Add(violations, WorldInvariantCatalog.ExtensionIdentityUnique, ExtensionResource(current),
                        ExtensionPath(current, null),
                        "Only one opaque extension payload may exist for an owner/schema-version/subject identity.",
                        Context("firstIndex", Invariant(original), "duplicateIndex", Invariant(index)));
                }
                else
                {
                    firstIndex.Add(current.Identity, index);
                }
            }
        }

        private static void ValidateExtensionSubjects(WorldStateCandidate candidate, IList<WorldStateViolation> violations)
        {
            var ids = ObjectIds(candidate);
            ForEachExtension(candidate, current =>
            {
                if (current.SubjectId.HasValue && !ids.Contains(current.SubjectId.Value))
                {
                    Add(violations, WorldInvariantCatalog.ExtensionSubjectResolves, ExtensionResource(current),
                        ExtensionPath(current, "subjectId"),
                        "Extension subjects must resolve inside the same world state.",
                        Context("targetId", current.SubjectId.Value.Value));
                }
            });
        }

        private static void ValidateExtensionDependencyEntries(WorldStateCandidate candidate, IList<WorldStateViolation> violations)
        {
            ForEachExtension(candidate, current =>
            {
                for (var index = 0; index < current.Dependencies.Count; index++)
                {
                    if (current.Dependencies[index] != null) continue;
                    Add(violations, WorldInvariantCatalog.ExtensionDependencyEntryPresent, ExtensionResource(current),
                        ExtensionPath(current, "dependencies/" + Invariant(index)),
                        "Extension dependency entries cannot be null.",
                        Context("action", "remove-or-replace-entry"));
                }
            });
        }

        private static void ValidateExtensionDependencyUniqueness(WorldStateCandidate candidate, IList<WorldStateViolation> violations)
        {
            ForEachExtension(candidate, current =>
            {
                var seen = new HashSet<WorldReference>();
                for (var index = 0; index < current.Dependencies.Count; index++)
                {
                    var dependency = current.Dependencies[index];
                    if (dependency == null || seen.Add(dependency)) continue;
                    Add(violations, WorldInvariantCatalog.ExtensionDependencyUnique, ExtensionResource(current),
                        ExtensionPath(current, "dependencies/" + Invariant(index)),
                        "Duplicate extension dependency edges are not semantic state.", Context(
                            "kind", dependency.Kind.Value, "targetId", dependency.TargetId.Value));
                }
            });
        }

        private static void ValidateExtensionDependencyTargets(WorldStateCandidate candidate, IList<WorldStateViolation> violations)
        {
            var ids = ObjectIds(candidate);
            ForEachExtension(candidate, current =>
            {
                for (var index = 0; index < current.Dependencies.Count; index++)
                {
                    var dependency = current.Dependencies[index];
                    if (dependency == null || ids.Contains(dependency.TargetId)) continue;
                    Add(violations, WorldInvariantCatalog.ExtensionDependencyTargetResolves, ExtensionResource(current),
                        ExtensionPath(current, "dependencies/" + Invariant(index)),
                        "Extension dependency targets must resolve inside the same world state.", Context(
                            "kind", dependency.Kind.Value, "targetId", dependency.TargetId.Value));
                }
            });
        }

        private static void Add(
            IList<WorldStateViolation> violations,
            WorldInvariantDefinition invariant,
            string resource,
            string path,
            string message,
            IReadOnlyDictionary<string, string> context)
        {
            violations.Add(new WorldStateViolation(invariant, resource, path, message, context));
        }

        private static HashSet<WorldObjectId> ObjectIds(WorldStateCandidate candidate)
        {
            var ids = new HashSet<WorldObjectId>();
            ForEachObject(candidate, value => ids.Add(value.Id));
            return ids;
        }

        private static Dictionary<WorldObjectId, WorldObject> ObjectsById(WorldStateCandidate candidate)
        {
            var values = new Dictionary<WorldObjectId, WorldObject>();
            ForEachObject(candidate, value =>
            {
                if (!values.ContainsKey(value.Id)) values.Add(value.Id, value);
            });
            return values;
        }

        private static void ForEachObject(WorldStateCandidate candidate, Action<WorldObject> action)
        {
            for (var index = 0; index < candidate.Objects.Count; index++)
            {
                var current = candidate.Objects[index];
                if (current != null) action(current);
            }
        }

        private static void ForEachExtension(WorldStateCandidate candidate, Action<WorldExtensionData> action)
        {
            for (var index = 0; index < candidate.Extensions.Count; index++)
            {
                var current = candidate.Extensions[index];
                if (current != null) action(current);
            }
        }

        private static IReadOnlyDictionary<string, string> Context(params string[] values)
        {
            var result = new Dictionary<string, string>(StringComparer.Ordinal);
            for (var index = 0; index < values.Length; index += 2) result.Add(values[index], values[index + 1]);
            return result;
        }

        private static string WorldResource(WorldStateCandidate candidate) =>
            string.IsNullOrEmpty(candidate.Id.Value) ? "world" : "world:" + candidate.Id.Value;
        private static string ObjectResource(WorldObject value) => "world.object:" + value.Id.Value;
        private static string ExtensionResource(WorldExtensionData value) => "world.extension:" + value.Identity.ResourceKey;
        private static string ObjectPath(WorldObject value, string suffix) => "$/objects/" + value.Id.Value + "/" + suffix;
        private static string ExtensionPath(WorldExtensionData value, string? suffix) =>
            "$/extensions/" + value.Identity.ResourceKey + (suffix == null ? string.Empty : "/" + suffix);
        private static string Invariant(int value) => value.ToString(CultureInfo.InvariantCulture);
    }
}
