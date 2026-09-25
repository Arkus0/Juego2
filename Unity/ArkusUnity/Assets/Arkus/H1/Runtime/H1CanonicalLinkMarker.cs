using UnityEngine;

namespace Arkus.H1.Projection
{
    [DisallowMultipleComponent]
    public sealed class H1CanonicalLinkMarker : MonoBehaviour
    {
        public const string SchemaId = "arkus.h1.component.canonical-link@1";
        public string schemaId = SchemaId;
        public string relation = "";
        public string targetObjectId = "";
        public GameObject target;
    }
}
