using UnityEngine;

namespace Juego2.ART
{
    /// <summary>ART companion identity/assembly evidence; never canonical WorldState or H1 catalogue state.</summary>
    public sealed class Art01Piece : MonoBehaviour
    {
        public string logicalId;
        /// <summary>Reviewed KIT_COMPOSITION_MANIFEST family (piece or assembly) this instance realizes.</summary>
        public string kitId;
        public string assemblyRole;
        public string connection;
        public string presentationState = "KEEPER_READY";
        public string collisionRole = "none";
    }
}
