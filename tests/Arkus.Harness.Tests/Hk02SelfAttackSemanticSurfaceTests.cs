using System;
using System.Reflection;
using Arkus.Game.World;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk02SelfAttackTestsSemanticSurface
    {
        [Fact]
        public void SemanticInputAndPropertyUniverseCannotGrowWithoutUpdatingCanonicalProof()
        {
            AssertSurface(
                typeof(WorldState),
                new[] { "id", "revision", "objects", "extensions", "schemaVersion" },
                new[] { "Extensions", "Id", "Objects", "Revision", "SchemaVersion" });

            AssertSurface(
                typeof(WorldObject),
                new[] { "id", "typeId", "containerId", "references" },
                new[] { "ContainerId", "Id", "References", "TypeId" });

            AssertSurface(
                typeof(WorldReference),
                new[] { "kind", "targetId" },
                new[] { "Kind", "TargetId" });

            AssertSurface(
                typeof(WorldExtensionData),
                new[] { "owner", "schemaVersion", "payload", "subjectId", "dependencies" },
                new[] { "Dependencies", "Identity", "Owner", "PayloadLength", "SchemaVersion", "SubjectId" });
        }

        private static void AssertSurface(Type type, string[] expectedConstructorParameters, string[] expectedProperties)
        {
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            Assert.Single(constructors);

            var parameters = constructors[0].GetParameters();
            var actualParameterNames = new string[parameters.Length];
            for (var index = 0; index < parameters.Length; index++)
            {
                actualParameterNames[index] = parameters[index].Name ?? string.Empty;
            }

            Assert.Equal(expectedConstructorParameters, actualParameterNames);

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var actualPropertyNames = new string[properties.Length];
            for (var index = 0; index < properties.Length; index++)
            {
                actualPropertyNames[index] = properties[index].Name;
            }

            Array.Sort(actualPropertyNames, StringComparer.Ordinal);
            Assert.Equal(expectedProperties, actualPropertyNames);
        }
    }
}
