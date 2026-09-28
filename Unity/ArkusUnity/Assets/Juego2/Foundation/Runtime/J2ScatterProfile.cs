using System;
using UnityEngine;

namespace Juego2.Foundation
{
    /// <summary>
    /// Deterministic seeded placement of admitted prefabs with no-go exclusions (traversable, structure, water) and
    /// per-prefab scale bands (H2F-01 S04). Terrain trees/details are deliberately not used.
    /// </summary>
    [CreateAssetMenu(menuName = "Juego2/Foundation/Scatter Profile", fileName = "J2_ScatterProfile")]
    public sealed class J2ScatterProfile : ScriptableObject
    {
        public Entry[] entries = Array.Empty<Entry>();
        [Min(0.1f)] public float minSpacing = 1.2f;
        [Min(0f)] public float density = 0.35f;
        [Tooltip("Physics layers whose colliders are no-go (traversable surfaces, structures, water volumes).")]
        public LayerMask exclusionLayers = ~0;
        [Min(0f)] public float exclusionMargin = 0.6f;
        [Tooltip("Physics layers a placement must land on (ground/terrain).")]
        public LayerMask groundLayers = ~0;
        [Range(0f, 60f)] public float maxGroundSlope = 30f;

        [Serializable]
        public struct Entry
        {
            public GameObject prefab;
            [Min(0f)] public float weight;
            public Vector2 scaleBand;
            public bool alignToGround;
        }
    }
}
