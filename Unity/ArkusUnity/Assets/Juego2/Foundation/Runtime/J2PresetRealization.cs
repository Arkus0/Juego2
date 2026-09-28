using UnityEngine;

namespace Juego2.Foundation
{
    /// <summary>
    /// Marks a presentation object materialized from a Juego2 preset (character body, player camera). Its GC2 and
    /// Unity configuration is a projection of the preset: it is rematerialized from the preset, never read back as
    /// canonical state (H2F-02 lifecycle matrix, families S3/S4/S15).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class J2PresetRealization : MonoBehaviour
    {
        public ScriptableObject preset;
        public string presetKind;
    }
}
