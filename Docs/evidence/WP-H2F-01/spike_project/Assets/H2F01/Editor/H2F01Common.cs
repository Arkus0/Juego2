using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace Juego2.H2F01.Editor
{
    /// <summary>Shared helpers for the disposable, non-keeper H2F-01 spike workspace.</summary>
    public static class H2F01Common
    {
        public const string ArtScene = "Assets/Arkus/ART/Art01Benchmark.unity";

        /// <summary>Workspace root = parent of Unity/Spike. Outputs never go to the repository directly.</summary>
        public static string Root => Path.GetFullPath(Path.Combine(Application.dataPath, "../../.."));

        public static string Out(string relative)
        {
            var path = Path.Combine(Root, "out", relative);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            return path;
        }

        public static string Sha(byte[] bytes)
        {
            using (var sha = SHA256.Create())
                return string.Concat(sha.ComputeHash(bytes).Select(b => b.ToString("x2")));
        }

        public static void WriteJson(string relative, string json)
        {
            File.WriteAllText(Out(relative), json + "\n", new UTF8Encoding(false));
        }

        public static Camera CameraAt(string name, Vector3 position, Vector3 target, float fov = 60)
        {
            var go = new GameObject(name);
            var camera = go.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.fieldOfView = fov;
            camera.nearClipPlane = 0.05f;
            camera.farClipPlane = 180;
            go.transform.position = position;
            go.transform.LookAt(target);
            return camera;
        }

        /// <summary>Renders one frame through whichever pipeline is active.</summary>
        public static void Image(Camera camera, string relative, int width = 1280, int height = 720)
        {
            var rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32) { antiAliasing = 4 };
            rt.Create();
            if (GraphicsSettings.currentRenderPipeline != null)
            {
                var request = new RenderPipeline.StandardRequest { destination = rt };
                if (RenderPipeline.SupportsRenderRequest(camera, request))
                    RenderPipeline.SubmitRenderRequest(camera, request);
                else
                    throw new Exception("H2F01_RENDER_REQUEST_UNSUPPORTED " + camera.name);
            }
            else
            {
                camera.targetTexture = rt;
                camera.Render();
                camera.targetTexture = null;
            }
            var old = RenderTexture.active;
            RenderTexture.active = rt;
            var frame = new Texture2D(width, height, TextureFormat.RGB24, false);
            frame.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            frame.Apply();
            RenderTexture.active = old;
            File.WriteAllBytes(Out(relative), frame.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(frame);
            rt.Release();
            UnityEngine.Object.DestroyImmediate(rt);
        }

        /// <summary>Counts every material/shader actually bound to a renderer in the open scenes.</summary>
        public static ShaderAudit AuditShaders()
        {
            var audit = new ShaderAudit { pipeline = GraphicsSettings.currentRenderPipeline == null ? "built-in" : GraphicsSettings.currentRenderPipeline.GetType().Name };
            var seen = new HashSet<Material>();
            var byShader = new SortedDictionary<string, int>();
            foreach (var renderer in UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    audit.bindings++;
                    if (material == null) { audit.nullBindings++; audit.failures.Add("null material on " + renderer.name); continue; }
                    if (!seen.Add(material)) continue;
                    var shader = material.shader;
                    var name = shader == null ? "<null>" : shader.name;
                    byShader[name] = byShader.TryGetValue(name, out var n) ? n + 1 : 1;
                    bool urp = audit.pipeline != "built-in";
                    bool error = shader == null || !shader.isSupported || name.StartsWith("Hidden/InternalErrorShader")
                                 || name.Contains("FallbackError") || name.Contains("MaterialError")
                                 || (urp && (name == "Standard" || name.StartsWith("Legacy Shaders/") || name.StartsWith("Mobile/")));
                    if (error) audit.failures.Add($"{material.name}: {name}");
                }
            }
            audit.materials = seen.Count;
            audit.shaders = byShader.Select(kv => kv.Key + "=" + kv.Value).ToArray();
            audit.green = audit.failures.Count == 0;
            return audit;
        }

        [Serializable]
        public class ShaderAudit
        {
            public string pipeline;
            public int bindings, nullBindings, materials;
            public string[] shaders;
            public List<string> failures = new List<string>();
            public bool green;
        }

        public static void OpenArtScene() => EditorSceneManager.OpenScene(ArtScene, OpenSceneMode.Single);

        public static void Log(string marker) => Debug.Log(marker);
    }
}
