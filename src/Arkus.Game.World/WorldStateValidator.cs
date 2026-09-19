using System;
using System.Collections.Generic;

namespace Arkus.Game.World
{
    public static class WorldStateValidator
    {
        public static void ValidateOrThrow(WorldState state)
        {
            if (state is null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (state.SchemaVersion != WorldState.CurrentSchemaVersion)
            {
                throw Error(
                    "world.unsupported_schema",
                    "Unsupported world schema version.",
                    "$/schemaVersion");
            }

            if (state.Revision < 0)
            {
                throw Error("world.invalid_revision", "World revision cannot be negative.", "$/revision");
            }

            var byId = new Dictionary<WorldObjectId, WorldObject>();
            for (var index = 0; index < state.Objects.Count; index++)
            {
                var current = state.Objects[index] ?? throw Error(
                    "world.invalid_object",
                    "World object entries cannot be null.",
                    "$/objects/" + index.ToString(System.Globalization.CultureInfo.InvariantCulture));

                if (!byId.TryAdd(current.Id, current))
                {
                    throw Error(
                        "world.duplicate_id",
                        "World object IDs must be unique.",
                        "$/objects/" + current.Id.Value);
                }
            }

            for (var index = 0; index < state.Objects.Count; index++)
            {
                var current = state.Objects[index];
                if (current.ContainerId.HasValue)
                {
                    var containerId = current.ContainerId.Value;
                    if (!byId.ContainsKey(containerId))
                    {
                        throw Error(
                            "world.dangling_container",
                            "Container IDs must resolve inside the same world state.",
                            "$/objects/" + current.Id.Value + "/container");
                    }

                    if (containerId == current.Id)
                    {
                        throw Error(
                            "world.containment_cycle",
                            "An object cannot contain itself.",
                            "$/objects/" + current.Id.Value + "/container");
                    }
                }

                var uniqueReferences = new HashSet<WorldReference>();
                for (var referenceIndex = 0; referenceIndex < current.References.Count; referenceIndex++)
                {
                    var reference = current.References[referenceIndex] ?? throw Error(
                        "world.invalid_reference",
                        "Reference entries cannot be null.",
                        "$/objects/" + current.Id.Value + "/references/" + referenceIndex.ToString(System.Globalization.CultureInfo.InvariantCulture));

                    if (!uniqueReferences.Add(reference))
                    {
                        throw Error(
                            "world.duplicate_reference",
                            "Duplicate reference edges are not semantic state.",
                            "$/objects/" + current.Id.Value + "/references");
                    }

                    if (!byId.ContainsKey(reference.TargetId))
                    {
                        throw Error(
                            "world.dangling_reference",
                            "Reference targets must resolve inside the same world state.",
                            "$/objects/" + current.Id.Value + "/references/" + referenceIndex.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                }
            }

            ValidateContainmentAcyclic(state, byId);
            ValidateExtensions(state, byId);
        }

        private static void ValidateContainmentAcyclic(
            WorldState state,
            IReadOnlyDictionary<WorldObjectId, WorldObject> byId)
        {
            for (var index = 0; index < state.Objects.Count; index++)
            {
                var origin = state.Objects[index];
                var seen = new HashSet<WorldObjectId>();
                var current = origin;
                seen.Add(origin.Id);

                while (current.ContainerId.HasValue)
                {
                    var parentId = current.ContainerId.Value;
                    if (!seen.Add(parentId))
                    {
                        throw Error(
                            "world.containment_cycle",
                            "Containment edges must form an acyclic forest.",
                            "$/objects/" + origin.Id.Value + "/container");
                    }

                    current = byId[parentId];
                }
            }
        }

        private static void ValidateExtensions(
            WorldState state,
            IReadOnlyDictionary<WorldObjectId, WorldObject> byId)
        {
            var identities = new HashSet<WorldExtensionIdentity>();
            for (var index = 0; index < state.Extensions.Count; index++)
            {
                var extension = state.Extensions[index] ?? throw Error(
                    "world.invalid_extension",
                    "Extension entries cannot be null.",
                    "$/extensions/" + index.ToString(System.Globalization.CultureInfo.InvariantCulture));

                var identity = extension.Identity;
                if (!identities.Add(identity))
                {
                    throw Error(
                        "world.duplicate_extension",
                        "Only one opaque extension payload may exist for an owner/schema-version/subject identity.",
                        "$/extensions/" + identity.ResourceKey);
                }

                if (extension.SubjectId.HasValue && !byId.ContainsKey(extension.SubjectId.Value))
                {
                    throw Error(
                        "world.dangling_extension_subject",
                        "Extension subjects must resolve inside the same world state.",
                        "$/extensions/" + identity.ResourceKey + "/subjectId");
                }

                var uniqueDependencies = new HashSet<WorldReference>();
                for (var dependencyIndex = 0; dependencyIndex < extension.Dependencies.Count; dependencyIndex++)
                {
                    var dependency = extension.Dependencies[dependencyIndex] ?? throw Error(
                        "world.invalid_extension_dependency",
                        "Extension dependency entries cannot be null.",
                        "$/extensions/" + identity.ResourceKey + "/dependencies/" +
                        dependencyIndex.ToString(System.Globalization.CultureInfo.InvariantCulture));

                    if (!uniqueDependencies.Add(dependency))
                    {
                        throw Error(
                            "world.duplicate_extension_dependency",
                            "Duplicate extension dependency edges are not semantic state.",
                            "$/extensions/" + identity.ResourceKey + "/dependencies");
                    }

                    if (!byId.ContainsKey(dependency.TargetId))
                    {
                        throw Error(
                            "world.dangling_extension_dependency",
                            "Extension dependency targets must resolve inside the same world state.",
                            "$/extensions/" + identity.ResourceKey + "/dependencies/" +
                            dependencyIndex.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                }
            }
        }

        private static WorldStateException Error(string code, string message, string path)
        {
            return new WorldStateException(code, message, path);
        }
    }
}
