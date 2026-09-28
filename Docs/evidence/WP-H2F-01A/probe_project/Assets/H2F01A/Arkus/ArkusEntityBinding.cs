using System.Collections.Generic;
using UnityEngine;

namespace Juego2.H2F01A.Arkus
{
    /// <summary>
    /// Bridge locator from a Juego2 entity key to its current Unity realization. The key is the identity; the
    /// GameObject (and any GC2 component on it) is only where that entity is presented this session.
    /// </summary>
    public sealed class ArkusEntityBinding : MonoBehaviour
    {
        public string entityKey;
        public string kind;

        static readonly Dictionary<string, ArkusEntityBinding> Bound = new Dictionary<string, ArkusEntityBinding>();

        public static GameObject Resolve(string key) =>
            key != null && Bound.TryGetValue(key, out var binding) && binding != null ? binding.gameObject : null;

        public static string KeyOf(GameObject go)
        {
            if (go == null) return null;
            var binding = go.GetComponentInParent<ArkusEntityBinding>();
            return binding != null ? binding.entityKey : null;
        }

        public static int Count => Bound.Count;

        void OnEnable()
        {
            if (!string.IsNullOrEmpty(entityKey)) Bound[entityKey] = this;
        }

        void OnDisable()
        {
            if (!string.IsNullOrEmpty(entityKey) && Bound.TryGetValue(entityKey, out var b) && b == this) Bound.Remove(entityKey);
        }
    }
}
