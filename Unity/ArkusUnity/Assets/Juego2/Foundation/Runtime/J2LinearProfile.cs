using UnityEngine;

namespace Juego2.Foundation
{
    /// <summary>
    /// Cross-section of a linear feature realized along a Unity Splines centreline (road, lane, path). The traversable
    /// surface is the only collider; kerbs and retaining walls are edges outside it (H2F-01 S02).
    /// </summary>
    [CreateAssetMenu(menuName = "Juego2/Foundation/Linear Profile", fileName = "J2_LinearProfile")]
    public sealed class J2LinearProfile : ScriptableObject
    {
        [Min(0.5f)] public float width = 2.8f;
        [Tooltip("Lateral crown height at -w/2, -w/4, 0, +w/4, +w/2.")]
        public float[] crown = { 0f, 0.02f, 0.03f, 0.02f, 0f };
        [Min(0.05f)] public float sampleStep = 0.5f;
        [Min(0.1f)] public float uvScale = 1.5f;

        public EdgeKind leftEdge = EdgeKind.Kerb;
        public EdgeKind rightEdge = EdgeKind.Kerb;
        [Min(0f)] public float kerbWidth = 0.2f;
        [Min(0f)] public float kerbHeight = 0.12f;
        [Min(0f)] public float kerbEmbed = 0.15f;
        [Min(0.05f)] public float wallThickness = 0.4f;
        [Min(0f)] public float wallParapet = 0.65f;
        [Min(0f)] public float wallDrop = 2.2f;

        public Material surfaceMaterial;
        public Material edgeMaterial;
        public Material wallMaterial;
        [Tooltip("Rendering layer mask bits added to the traversable surface so URP decals project onto it only.")]
        public uint surfaceRenderingLayers = J2RenderingLayers.DecalReceiver;

        public enum EdgeKind
        {
            None,
            Kerb,
            RetainingWall,
        }
    }
}
