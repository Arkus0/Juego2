using UnityEngine;

namespace Arkus.H1.Projection
{
    [DisallowMultipleComponent]
    public sealed class H1ManagedMarker : MonoBehaviour
    {
        public const string SchemaId = "arkus.h1-managed-marker@1";
        public string schemaId = SchemaId;
        public string role = "";
        public string sceneLogicalId = "";
        public string generationId = "";
        public string canonicalObjectId = "";
        public string sourceLogicalId = "";
    }
}
