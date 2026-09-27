using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static Juego2.H2F01.Editor.H2F01Common;

namespace Juego2.H2F01.Editor
{
    /// <summary>Diagnostic: is a URP DecalProjector visible through the spike renderer and capture path?</summary>
    public static class H2F01DecalProbe
    {
        public static void Run()
        {
            OpenArtScene();
            var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>("Assets/H2F01/Settings/H2F01_Renderer.asset");
            var feature = renderer.rendererFeatures.OfType<DecalRendererFeature>().FirstOrDefault();
            var so = feature != null ? new SerializedObject(feature) : null;
            var technique = so?.FindProperty("m_Settings.technique");
            Log($"H2F01_DECAL_PROBE feature={(feature != null)} active={(feature != null && feature.isActive)} technique={(technique != null ? technique.enumDisplayNames[technique.enumValueIndex] : "?")}");
            var tex = new Texture2D(4, 4);
            tex.SetPixels(Enumerable.Repeat(new Color(1, 0, 0, 1), 16).ToArray());
            tex.Apply();
            var mat = new Material(AssetDatabase.LoadAssetAtPath<Material>("Packages/com.unity.render-pipelines.universal/Runtime/Materials/Decal.mat"));
            mat.SetTexture("Base_Map", tex);
            Log("H2F01_DECAL_PROBE shader=" + mat.shader.name + " supported=" + mat.shader.isSupported + " passes=" + mat.passCount);
            var decal = new GameObject("probe_decal").AddComponent<DecalProjector>();
            decal.material = mat;
            decal.size = new Vector3(3, 3, 3);
            Physics.Raycast(new Vector3(0, 30, -3), Vector3.down, out var hit, 60);
            decal.transform.position = hit.point + Vector3.up * 1f;
            decal.transform.rotation = Quaternion.Euler(90, 0, 0);
            var cam = CameraAt("probe_cam", hit.point + new Vector3(0, 6, -4), hit.point);
            cam.GetUniversalAdditionalCameraData().renderPostProcessing = true;
            Image(cam, "s01/decal_probe.png");
            Log("H2F01_DECAL_PROBE_CAPTURED hit=" + hit.point);
        }
    }
}
