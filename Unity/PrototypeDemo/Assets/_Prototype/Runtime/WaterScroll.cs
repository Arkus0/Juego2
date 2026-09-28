using UnityEngine;

namespace Proto.Runtime
{
    /// Slow drift of the water normal map (two directions via main + detail offset).
    public class WaterScroll : MonoBehaviour
    {
        public Vector2 speed = new Vector2(0.012f, 0.004f);
        Material mat;

        void Start() => mat = GetComponent<Renderer>().material;

        void Update()
        {
            if (mat == null) return;
            mat.SetTextureOffset("_BaseMap", speed * Time.time);
            mat.SetTextureOffset("_BumpMap", speed * Time.time);
        }
    }
}
