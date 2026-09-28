using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Juego2.Arkus
{
    /// <summary>
    /// Bridge locator from a Juego2 semantic entity key to the scene object that presents it this session.
    /// The key is the identity. The GameObject, its instance ID, GUIDs and any Game Creator component on it
    /// are realization only (H2F-01A authority rule 2). This is the only identity a GC2-driven object carries.
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public sealed class ArkusEntityBinding : MonoBehaviour
    {
        /// <summary>Juego2 entity keys: lowercase dotted segments starting with <c>j2.</c>, e.g. <c>j2.char.player</c>.</summary>
        public static readonly Regex KeyGrammar = new Regex(@"^j2(\.[a-z0-9_]+)+$", RegexOptions.CultureInvariant);

        public string entityKey;
        public string kind;

        static readonly Dictionary<string, ArkusEntityBinding> Bound = new Dictionary<string, ArkusEntityBinding>(StringComparer.Ordinal);

        public static bool IsValidKey(string key) => key != null && KeyGrammar.IsMatch(key);

        public static GameObject Resolve(string key) =>
            key != null && Bound.TryGetValue(key, out var binding) && binding != null ? binding.gameObject : null;

        /// <summary>The Juego2 key of the nearest bound ancestor, or null when the object is unbound.</summary>
        public static string KeyOf(GameObject go)
        {
            if (go == null) return null;
            var binding = go.GetComponentInParent<ArkusEntityBinding>(true);
            return binding != null && IsValidKey(binding.entityKey) ? binding.entityKey : null;
        }

        public static int Count => Bound.Count;

        void OnEnable()
        {
            if (IsValidKey(entityKey)) Bound[entityKey] = this;
        }

        void OnDisable()
        {
            if (entityKey != null && Bound.TryGetValue(entityKey, out var b) && b == this) Bound.Remove(entityKey);
        }
    }
}
