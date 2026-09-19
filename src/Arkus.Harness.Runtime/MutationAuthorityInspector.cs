using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Arkus.Game.Authoring;

namespace Arkus.Harness.Runtime
{
    /// <summary>
    /// Independent structural oracle for effective canonical write authority.
    ///
    /// It deliberately does not read CapabilityDefinition.SideEffect or policy metadata. A handler
    /// is authority-bearing when its executable object graph can directly hold the Authoring-owned
    /// commit capability or the concrete transactional session whose public Apply method can commit.
    /// The sealed WorldMutationPlannerView is the explicit attenuation boundary: its private wrapped
    /// service is not reachable through the handler's API without reflection/private-member subversion.
    /// </summary>
    internal static class MutationAuthorityInspector
    {
        public static bool TypeCarriesCanonicalWriteAuthority(Type handlerType)
        {
            if (handlerType == null) throw new ArgumentNullException(nameof(handlerType));
            return TypeCarriesCanonicalWriteAuthority(
                handlerType,
                handlerType.Assembly,
                new HashSet<Type>());
        }

        public static bool CarriesCanonicalWriteAuthority(ICanonicalCapabilityHandler handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            return ObjectCarriesCanonicalWriteAuthority(
                handler,
                handler.GetType().Assembly,
                new HashSet<object>(ReferenceEqualityComparer.Instance),
                true);
        }

        private static bool TypeCarriesCanonicalWriteAuthority(
            Type type,
            Assembly handlerAssembly,
            ISet<Type> visited)
        {
            if (type == typeof(WorldMutationPlannerView)) return false;
            if (IsDirectAuthorityType(type)) return true;
            if (!visited.Add(type)) return false;

            foreach (var field in EnumerateFields(type))
            {
                if (IsDirectAuthorityType(field.FieldType)) return true;
                if (field.FieldType == typeof(WorldMutationPlannerView)) continue;
                if (field.FieldType.Assembly == handlerAssembly &&
                    IsInspectableContainer(field.FieldType) &&
                    TypeCarriesCanonicalWriteAuthority(field.FieldType, handlerAssembly, visited))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ObjectCarriesCanonicalWriteAuthority(
            object value,
            Assembly handlerAssembly,
            ISet<object> visited,
            bool root)
        {
            if (value is WorldMutationPlannerView) return false;
            var type = value.GetType();
            if (IsDirectAuthorityType(type)) return true;
            if (!visited.Add(value)) return false;

            if (value is Delegate callback)
            {
                if (callback.Target != null &&
                    ObjectCarriesCanonicalWriteAuthority(callback.Target, handlerAssembly, visited, false))
                {
                    return true;
                }

                return false;
            }

            if (!root && type.Assembly != handlerAssembly) return false;

            foreach (var field in EnumerateFields(type))
            {
                if (IsDirectAuthorityType(field.FieldType)) return true;
                if (field.FieldType == typeof(WorldMutationPlannerView)) continue;

                object? nested;
                try
                {
                    nested = field.GetValue(field.IsStatic ? null : value);
                }
                catch (TargetException)
                {
                    continue;
                }

                if (nested == null) continue;
                if (nested is WorldMutationPlannerView) continue;
                if (IsDirectAuthorityType(nested.GetType())) return true;

                if ((nested.GetType().Assembly == handlerAssembly || nested is Delegate) &&
                    IsInspectableContainer(nested.GetType()) &&
                    ObjectCarriesCanonicalWriteAuthority(nested, handlerAssembly, visited, false))
                {
                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<FieldInfo> EnumerateFields(Type type)
        {
            for (var current = type; current != null && current != typeof(object); current = current.BaseType)
            {
                foreach (var field in current.GetFields(
                    BindingFlags.Instance |
                    BindingFlags.Static |
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.DeclaredOnly))
                {
                    yield return field;
                }
            }
        }

        private static bool IsDirectAuthorityType(Type type)
        {
            return typeof(ICanonicalWorldMutationCommitter).IsAssignableFrom(type) ||
                typeof(TransactionalWorldAuthoringSession).IsAssignableFrom(type);
        }

        private static bool IsInspectableContainer(Type type)
        {
            return !type.IsPrimitive &&
                !type.IsEnum &&
                type != typeof(string) &&
                type != typeof(decimal) &&
                type != typeof(DateTime) &&
                type != typeof(DateTimeOffset) &&
                type != typeof(TimeSpan);
        }

        private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
        {
            public static readonly ReferenceEqualityComparer Instance = new ReferenceEqualityComparer();

            public new bool Equals(object? left, object? right)
            {
                return ReferenceEquals(left, right);
            }

            public int GetHashCode(object value)
            {
                return RuntimeHelpers.GetHashCode(value);
            }
        }
    }
}
