using UnityEngine;

namespace Juego2.Foundation
{
    /// <summary>
    /// Declares a no-go volume for Juego2 scatter (structures, water, thresholds). Its colliders and its children's
    /// colliders keep scattered nature out, together with every generated traversable/edge realization.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class J2ScatterExclusion : MonoBehaviour
    {
    }
}
