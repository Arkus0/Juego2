using Juego2.Foundation;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Juego2.Foundation.Editor
{
    /// <summary>
    /// Applies the foundation look preset to a scene: sun, trilight ambient, exp² fog, overcast sky and one global
    /// Volume. A baseline for fixtures and new scenes; final scene lighting belongs to ART/CITY.
    /// </summary>
    public static class J2FoundationLook
    {
        public const string VolumeName = "J2_GlobalVolume";

        public static void ApplyToActiveScene() => ApplyTo(SceneManager.GetActiveScene(),
            AssetDatabase.LoadAssetAtPath<J2LookPreset>(J2FoundationBaseline.LookPreset));

        public static void ApplyTo(Scene scene, J2LookPreset preset)
        {
            if (preset == null) throw new System.InvalidOperationException("J2_LOOK_PRESET_MISSING");
            var previous = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(scene);
            RenderSettings.skybox = preset.skybox as Material;
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = preset.ambientSky;
            RenderSettings.ambientEquatorColor = preset.ambientEquator;
            RenderSettings.ambientGroundColor = preset.ambientGround;
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = preset.fogDensity;
            RenderSettings.fogColor = preset.fogColor;

            Light sun = RenderSettings.sun;
            if (sun == null)
                foreach (var root in scene.GetRootGameObjects())
                    if ((sun = root.GetComponentInChildren<Light>()) != null && sun.type == LightType.Directional) break;
            if (sun == null || sun.type != LightType.Directional)
            {
                sun = new GameObject("J2_Sun").AddComponent<Light>();
                sun.type = LightType.Directional;
                SceneManager.MoveGameObjectToScene(sun.gameObject, scene);
            }
            sun.color = preset.sunColor;
            sun.intensity = preset.sunIntensity;
            sun.shadows = LightShadows.Soft;
            sun.transform.rotation = Quaternion.Euler(preset.sunEuler);
            RenderSettings.sun = sun;

            Volume volume = null;
            foreach (var root in scene.GetRootGameObjects())
                if (root.name == VolumeName) volume = root.GetComponent<Volume>();
            if (volume == null)
            {
                volume = new GameObject(VolumeName).AddComponent<Volume>();
                SceneManager.MoveGameObjectToScene(volume.gameObject, scene);
            }
            volume.isGlobal = true;
            volume.sharedProfile = preset.volumeProfile as VolumeProfile;
            foreach (var root in scene.GetRootGameObjects())
                foreach (var cam in root.GetComponentsInChildren<Camera>(true))
                    cam.GetUniversalAdditionalCameraData().renderPostProcessing = true;
            SceneManager.SetActiveScene(previous);
        }

        /// <summary>Project-owned high-overcast equirect gradient with soft cloud banding (no external HDRI; H2F-01 S01).</summary>
        public static Texture2D GenerateOvercastSky()
        {
            const int w = 512, h = 256;
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            var zenith = new Color(0.60f, 0.64f, 0.67f);
            var horizon = new Color(0.78f, 0.80f, 0.80f);
            var ground = new Color(0.42f, 0.45f, 0.44f);
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    float v = y / (float)(h - 1);
                    Color c;
                    if (v < 0.5f) c = Color.Lerp(ground, horizon, Mathf.Pow(v / 0.5f, 3f));
                    else
                    {
                        float t = (v - 0.5f) / 0.5f;
                        c = Color.Lerp(horizon, zenith, Mathf.Sqrt(t));
                        float cloud = Mathf.PerlinNoise(x * 0.012f, y * 0.05f) * 0.6f + Mathf.PerlinNoise(x * 0.04f, y * 0.12f) * 0.4f;
                        c = Color.Lerp(c, new Color(0.84f, 0.85f, 0.85f), Mathf.Clamp01(cloud - 0.45f) * 0.9f * (1 - t * 0.5f));
                    }
                    tex.SetPixel(x, y, c);
                }
            tex.Apply();
            return tex;
        }
    }
}
