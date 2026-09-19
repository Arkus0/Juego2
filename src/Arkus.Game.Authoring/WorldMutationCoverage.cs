using System;
using System.Collections.Generic;
using Arkus.Game.World;

namespace Arkus.Game.Authoring
{
    /// <summary>
    /// Independent semantic-effect oracle used by the mutation planner to ensure its returned
    /// change set cannot silently omit an effective authorable-state change.
    /// </summary>
    public static class WorldMutationCoverage
    {
        public static IReadOnlyList<string> FindMismatches(
            WorldState before,
            WorldState after,
            IReadOnlyList<WorldMutationChange> changes)
        {
            if (before == null) throw new ArgumentNullException(nameof(before));
            if (after == null) throw new ArgumentNullException(nameof(after));
            if (changes == null) throw new ArgumentNullException(nameof(changes));

            var expected = ExpectedEffects(before, after);
            var declared = DeclaredEffects(changes);
            var mismatches = new List<string>();

            foreach (var effect in expected)
            {
                if (!declared.Contains(effect))
                {
                    mismatches.Add("missing:" + effect);
                }
            }

            foreach (var effect in declared)
            {
                if (!expected.Contains(effect))
                {
                    mismatches.Add("extra:" + effect);
                }
            }

            mismatches.Sort(StringComparer.Ordinal);
            return mismatches.AsReadOnly();
        }

        private static HashSet<string> ExpectedEffects(WorldState before, WorldState after)
        {
            var effects = new HashSet<string>(StringComparer.Ordinal);
            var beforeObjects = ObjectsById(before);
            var afterObjects = ObjectsById(after);
            var objectIds = new SortedSet<string>(beforeObjects.Keys, StringComparer.Ordinal);
            objectIds.UnionWith(afterObjects.Keys);

            foreach (var id in objectIds)
            {
                beforeObjects.TryGetValue(id, out var oldValue);
                afterObjects.TryGetValue(id, out var newValue);
                var resource = "world.object:" + id;
                if (oldValue == null || newValue == null)
                {
                    effects.Add(Field(resource, "existence"));
                }

                if (oldValue == null || newValue == null || oldValue.TypeId != newValue.TypeId)
                {
                    effects.Add(Field(resource, "typeId"));
                }

                if (oldValue == null || newValue == null || oldValue.ContainerId != newValue.ContainerId)
                {
                    effects.Add(Field(resource, "containerId"));
                }

                var oldReferences = ReferenceSet(oldValue);
                var newReferences = ReferenceSet(newValue);
                foreach (var reference in newReferences)
                {
                    if (!oldReferences.Contains(reference))
                    {
                        effects.Add(Reference(resource, true, reference));
                    }
                }

                foreach (var reference in oldReferences)
                {
                    if (!newReferences.Contains(reference))
                    {
                        effects.Add(Reference(resource, false, reference));
                    }
                }

                if (!oldReferences.SetEquals(newReferences))
                {
                    effects.Add(Field(resource, "references"));
                }
            }

            var beforeExtensions = ExtensionsByKey(before);
            var afterExtensions = ExtensionsByKey(after);
            var extensionKeys = new SortedSet<string>(beforeExtensions.Keys, StringComparer.Ordinal);
            extensionKeys.UnionWith(afterExtensions.Keys);
            foreach (var key in extensionKeys)
            {
                beforeExtensions.TryGetValue(key, out var oldValue);
                afterExtensions.TryGetValue(key, out var newValue);
                var resource = "world.extension:" + key;
                if (oldValue == null || newValue == null)
                {
                    effects.Add(Field(resource, "existence"));
                }

                if (oldValue == null || newValue == null || !PayloadEquals(oldValue, newValue))
                {
                    effects.Add(Field(resource, "payload"));
                }
            }

            return effects;
        }

        private static HashSet<string> DeclaredEffects(IReadOnlyList<WorldMutationChange> changes)
        {
            var effects = new HashSet<string>(StringComparer.Ordinal);
            for (var index = 0; index < changes.Count; index++)
            {
                var change = changes[index] ?? throw new ArgumentException("Change list cannot contain null entries.", nameof(changes));
                foreach (var field in change.Fields)
                {
                    effects.Add(Field(change.Resource, field));
                }

                foreach (var reference in change.ReferencesAdded)
                {
                    effects.Add(Reference(change.Resource, true, reference));
                }

                foreach (var reference in change.ReferencesRemoved)
                {
                    effects.Add(Reference(change.Resource, false, reference));
                }
            }

            return effects;
        }

        private static Dictionary<string, WorldObject> ObjectsById(WorldState state)
        {
            var values = new Dictionary<string, WorldObject>(StringComparer.Ordinal);
            for (var index = 0; index < state.Objects.Count; index++)
            {
                values.Add(state.Objects[index].Id.Value, state.Objects[index]);
            }

            return values;
        }

        private static Dictionary<string, WorldExtensionData> ExtensionsByKey(WorldState state)
        {
            var values = new Dictionary<string, WorldExtensionData>(StringComparer.Ordinal);
            for (var index = 0; index < state.Extensions.Count; index++)
            {
                var value = state.Extensions[index];
                values.Add(ExtensionKey(value.Owner, value.SchemaVersion), value);
            }

            return values;
        }

        private static HashSet<string> ReferenceSet(WorldObject? value)
        {
            var references = new HashSet<string>(StringComparer.Ordinal);
            if (value == null)
            {
                return references;
            }

            for (var index = 0; index < value.References.Count; index++)
            {
                var current = value.References[index];
                references.Add(current.Kind.Value + "->" + current.TargetId.Value);
            }

            return references;
        }

        private static bool PayloadEquals(WorldExtensionData left, WorldExtensionData right)
        {
            var leftBytes = left.GetPayloadCopy();
            var rightBytes = right.GetPayloadCopy();
            if (leftBytes.Length != rightBytes.Length)
            {
                return false;
            }

            for (var index = 0; index < leftBytes.Length; index++)
            {
                if (leftBytes[index] != rightBytes[index])
                {
                    return false;
                }
            }

            return true;
        }

        private static string Field(string resource, string field) => resource + "|field|" + field;

        private static string Reference(string resource, bool added, string reference)
        {
            return resource + (added ? "|reference+|" : "|reference-|" ) + reference;
        }

        internal static string ExtensionKey(string owner, int schemaVersion)
        {
            return owner + "@" + schemaVersion.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
