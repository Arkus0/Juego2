using UnityEngine;

namespace Juego2.Foundation
{
    /// <summary>
    /// Marks a subtree produced by a Juego2 realization tool (linear profile, junction, scatter). The subtree is
    /// generated realization: it is rebuilt from its declared inputs (spline, profile, seed) and is never authoring
    /// truth. <see cref="inputDigest"/> identifies the inputs, <see cref="outputDigest"/> the geometry produced, so a
    /// stale or hand-edited output is detectable (H2F-02 lifecycle matrix).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class J2GeneratedRealization : MonoBehaviour
    {
        public string generator;
        public int generatorVersion;
        public string inputDigest;
        public string outputDigest;
    }
}
