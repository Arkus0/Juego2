using UnityEngine;

namespace Arkus.H1.Projection
{
    [DisallowMultipleComponent]
    public sealed class H1ComponentOwnershipMarker : MonoBehaviour
    {
        public const string SchemaId = "arkus.h1.component-ownership@1";
        public string schemaId = SchemaId;
        public bool rendererMaterial;
        public string rendererRelativePath = "";
        public bool animatorClip;
        public string animatorRelativePath = "";
    }
}
