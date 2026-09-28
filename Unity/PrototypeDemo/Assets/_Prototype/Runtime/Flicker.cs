using UnityEngine;

namespace Proto.Runtime
{
    /// Fire light: intensity and a little position jitter driven by noise.
    [RequireComponent(typeof(Light))]
    public class Flicker : MonoBehaviour
    {
        public float amount = 0.35f, speed = 7f;
        Light l; float baseI, seed; Vector3 basePos;

        void Start() { l = GetComponent<Light>(); baseI = l.intensity; basePos = transform.localPosition; seed = Random.value * 100f; }

        void Update()
        {
            float t = Time.time * speed + seed;
            float n = Mathf.PerlinNoise(t, seed) * 0.7f + Mathf.PerlinNoise(t * 2.3f, seed + 7) * 0.3f;
            l.intensity = baseI * (1f - amount + 2f * amount * n);
            transform.localPosition = basePos + new Vector3(Mathf.PerlinNoise(t * 0.7f, 3) - 0.5f, 0, Mathf.PerlinNoise(5, t * 0.7f) - 0.5f) * 0.06f;
        }
    }
}
