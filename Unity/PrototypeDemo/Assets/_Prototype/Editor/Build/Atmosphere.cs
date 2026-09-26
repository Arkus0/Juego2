using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Proto.Build
{
    /// Overcast Cantabrian light: diffuse key, trilight ambient, blue-grey distance fog, soft post.
    public static class Atmosphere
    {
        public static readonly Color Horizon = new Color(0.70f, 0.75f, 0.77f);
        public static readonly Color FogColor = new Color(0.64f, 0.69f, 0.71f);

        public static Light Build(Transform root)
        {
            var sky = new Material(Shader.Find("Proto/OvercastSky")) { name = "OvercastSky" };
            // overcast: the sky is brighter than any hazy mountain in front of it
            sky.SetColor("_Top", new Color(0.72f, 0.76f, 0.79f));
            sky.SetColor("_Horizon", new Color(0.80f, 0.83f, 0.84f));
            sky.SetColor("_CloudDark", new Color(0.62f, 0.66f, 0.70f));
            sky.SetColor("_CloudLight", new Color(0.88f, 0.9f, 0.91f));
            AssetDatabase.CreateAsset(sky, MatLib.GenDir + "/OvercastSky.mat");
            RenderSettings.skybox = sky;

            var sunGo = new GameObject("Sun (overcast key)");
            sunGo.transform.SetParent(root, false);
            var sun = sunGo.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = new Color(0.93f, 0.94f, 0.96f);
            sun.intensity = 1.05f;
            sun.shadows = LightShadows.Soft;
            sun.shadowStrength = 0.62f;
            sun.shadowBias = 0.04f;
            sun.shadowNormalBias = 0.35f;
            sunGo.transform.rotation = Quaternion.Euler(52f, -38f, 0f);
            RenderSettings.sun = sun;

            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.62f, 0.68f, 0.72f) * 1.05f;
            RenderSettings.ambientEquatorColor = new Color(0.52f, 0.56f, 0.56f);
            RenderSettings.ambientGroundColor = new Color(0.27f, 0.28f, 0.24f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = FogColor;
            RenderSettings.fogDensity = 0.0031f;
            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox;
            RenderSettings.reflectionIntensity = 0.8f;

            // post-processing
            var volGo = new GameObject("PostProcess");
            volGo.transform.SetParent(root, false);
            var vol = volGo.AddComponent<Volume>();
            vol.isGlobal = true;
            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(profile, MatLib.GenDir + "/PostProfile.asset");
            var tone = profile.Add<Tonemapping>(true); tone.mode.Override(TonemappingMode.ACES);
            var ca = profile.Add<ColorAdjustments>(true);
            ca.postExposure.Override(0.45f); ca.contrast.Override(10f); ca.saturation.Override(-14f);
            var wb = profile.Add<WhiteBalance>(true); wb.temperature.Override(-4f); wb.tint.Override(2f);
            var bloom = profile.Add<Bloom>(true); bloom.threshold.Override(1.05f); bloom.intensity.Override(0.35f); bloom.scatter.Override(0.6f);
            var vig = profile.Add<Vignette>(true); vig.intensity.Override(0.22f); vig.smoothness.Override(0.45f);
            var smh = profile.Add<ShadowsMidtonesHighlights>(true);
            smh.shadows.Override(new Vector4(0.96f, 1.0f, 1.04f, 0f));
            smh.highlights.Override(new Vector4(1.02f, 1.0f, 0.98f, 0f));
            EditorUtility.SetDirty(profile);
            vol.sharedProfile = profile;
            return sun;
        }

        /// Baked reflection probe over the town so water and wet stone reflect sky and façades.
        public static ReflectionProbe Probe(Transform root, Vector3 pos, Vector3 size, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(root, false);
            go.transform.position = pos;
            var rp = go.AddComponent<ReflectionProbe>();
            rp.mode = ReflectionProbeMode.Baked;
            rp.size = size;
            rp.resolution = 256;
            rp.boxProjection = false;
            rp.intensity = 1f;
            rp.hdr = true;
            return rp;
        }
    }
}
