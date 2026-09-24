using UnityEngine;

namespace Arkus.H1.Projection
{
    [DisallowMultipleComponent]
    public sealed class H1ManagedPrefabLineage : MonoBehaviour
    {
        public const string SchemaId = "arkus.h1-managed-prefab-lineage@1";
        public string schemaId = SchemaId;
        public string prefabGenerationId = "";
        public string sourceLogicalId = "";
        public string sourcePath = "";
        public string sourceGuid = "";
        public string sourceLocalFileId = "";
        public string sourceContentSha256 = "";
    }
}
