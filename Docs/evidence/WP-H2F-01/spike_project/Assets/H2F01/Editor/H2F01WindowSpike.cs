using System;
using System.Collections.Generic;
using System.Linq;
using Juego2.ART;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static Juego2.H2F01.Editor.H2F01Common;

namespace Juego2.H2F01.Editor
{
    /// <summary>
    /// S05 window/interior spike on the pinned ART-01 benchmark. A non-enterable W12 window is shown three ways
    /// (flat glass as built, shallow authored recess, project-owned interior-mapping shader) and the real
    /// enterable Bar F01 exterior -> threshold -> interior sequence is captured under the same URP baseline.
    /// </summary>
    public static class H2F01WindowSpike
    {
        [Serializable] class Result { public string windowPiece; public Vector3 glassCentre, outward, right; public Vector2 glassSize; public string shader; public bool shaderSupported; public bool auditGreen; public List<string> notes = new List<string>(); }

        public static void Run()
        {
            var r = new Result();
            OpenArtScene();
            H2F01RenderSpike.BaselineLook();
            H2F01RenderSpike.PoseHumans(H2F01RenderSpike.UalClip("Idle_Loop"));
            // target: a non-enterable house window whose glass is visible (shutters open or absent), on the lane facade
            var houses = new Dictionary<string, Vector3> { { "S02_stone_house", new Vector3(-6.8f, 0, -19) }, { "W12_left_house", new Vector3(-6.8f, 0, -5) }, { "W12_right_house", new Vector3(6.8f, 0, -1) } };
            var pieces = UnityEngine.Object.FindObjectsByType<Art01Piece>(FindObjectsSortMode.None).Where(p => p.logicalId != null).ToList();
            var candidates = new List<(Art01Piece piece, Renderer glass, Vector3 house)>();
            foreach (var w in pieces.Where(p => p.logicalId.EndsWith(".window")))
            {
                var houseId = houses.Keys.FirstOrDefault(h => w.logicalId.Contains(h));
                if (houseId == null) continue;
                var baseId = w.logicalId.Substring(0, w.logicalId.Length - ".window".Length);
                var shutters = pieces.FirstOrDefault(p => p.logicalId == baseId + ".shutters");
                bool closed = shutters != null && shutters.GetComponentsInChildren<MeshFilter>().Any(mf => mf.sharedMesh != null && mf.sharedMesh.name.Contains("Closed"));
                var g = w.GetComponentsInChildren<Renderer>().FirstOrDefault(x => x.sharedMaterials.Any(m => m != null && m.name == "Glass"));
                if (closed || g == null) continue;
                candidates.Add((w, g, houses[houseId]));
            }
            r.notes.Add($"visible-glass lane windows: {candidates.Count}");
            if (candidates.Count == 0) throw new Exception("H2F01_S05_VISIBLE_WINDOW_NOT_FOUND");
            // facade normal from the house footprint (yaw +-90: depth 10 along x, width 6 along z); lane facade preferred
            Vector3 Normal(Vector3 glassCentre, Vector3 hc)
            {
                var d = glassCentre - hc;
                return Mathf.Abs(d.x) / 5f >= Mathf.Abs(d.z) / 3f ? new Vector3(Mathf.Sign(d.x), 0, 0) : new Vector3(0, 0, Mathf.Sign(d.z));
            }
            var chosen = candidates.OrderByDescending(x => Mathf.Abs(Normal(x.glass.bounds.center, x.house).x) > 0.5f && Mathf.Sign(Normal(x.glass.bounds.center, x.house).x) == -Mathf.Sign(x.house.x))
                .ThenBy(x => x.glass.bounds.center.y).First();
            var piece = chosen.piece; var glass = chosen.glass; var house = chosen.house;
            r.windowPiece = piece.logicalId;
            var gb = glass.bounds;
            var outward = Normal(gb.center, house);
            r.notes.Add($"facade normal {outward} (lane facade={(Mathf.Abs(outward.x) > 0.5f)})");
            var right = Vector3.Cross(Vector3.up, outward).normalized;
            r.glassCentre = gb.center; r.outward = outward; r.right = right;
            r.glassSize = new Vector2(Mathf.Abs(Vector3.Dot(gb.size, right)), gb.size.y);
            // street-side human-scale views of that window
            // inspection cameras in front of the facade at human eye height on whatever ground is there
            Vector3 Stand(float dist, float side)
            {
                var p = gb.center + outward * dist + right * side;
                return new Vector3(p.x, RoadY(p.x, p.z) + 1.65f, p.z);
            }
            var eye = Stand(4.5f, 0.3f);
            var oblique = Stand(3.5f, 3.0f);
            void Shots(string variant)
            {
                Shot($"s05/{variant}_window_front.png", eye, gb.center);
                Shot($"s05/{variant}_window_oblique.png", oblique, gb.center);
            }
            Shots("V1_flat_glass");
            // V2: shallow authored recess (real geometry): dim room shell 1.0 m deep + curtain, glass made translucent
            var recess = new GameObject("S05_V2_authored_recess_NOT_KEEPER");
            var shell = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.DestroyImmediate(shell.GetComponent<Collider>());
            shell.transform.SetParent(recess.transform);
            shell.transform.position = gb.center - outward * 0.45f; // spans 0..0.9 m behind the glass plane
            shell.transform.rotation = Quaternion.LookRotation(outward);
            shell.transform.localScale = new Vector3(r.glassSize.x - 0.02f, r.glassSize.y - 0.02f, 0.9f); // exactly the opening: never pokes through the wall
            var shellMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            shellMat.SetColor("_BaseColor", new Color(0.33f, 0.28f, 0.23f)); shellMat.SetFloat("_Cull", 1); // front faces culled: see into the shell, never a lid behind the glass
            shell.GetComponent<Renderer>().sharedMaterial = shellMat;
            var curtain = GameObject.CreatePrimitive(PrimitiveType.Quad);
            UnityEngine.Object.DestroyImmediate(curtain.GetComponent<Collider>());
            curtain.transform.SetParent(recess.transform);
            curtain.transform.position = gb.center - outward * 0.12f + Vector3.up * r.glassSize.y * 0.3f + right * r.glassSize.x * 0.3f;
            curtain.transform.rotation = Quaternion.LookRotation(-outward);
            curtain.transform.localScale = new Vector3(r.glassSize.x * 0.4f, r.glassSize.y * 0.45f, 1);
            var curtainMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            curtainMat.SetColor("_BaseColor", new Color(0.42f, 0.2f, 0.16f)); curtainMat.SetFloat("_Cull", 0);
            curtain.GetComponent<Renderer>().sharedMaterial = curtainMat;
            var glassMats = glass.sharedMaterials;
            var translucent = new Material(glassMats.First(m => m.name == "Glass")) { name = "Glass_translucent_spike" };
            translucent.SetFloat("_Surface", 1); translucent.SetFloat("_Blend", 0);
            translucent.SetOverrideTag("RenderType", "Transparent");
            translucent.renderQueue = 3000;
            translucent.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            translucent.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            translucent.SetFloat("_ZWrite", 0);
            translucent.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            var c = translucent.GetColor("_BaseColor"); c.a = 0.3f; translucent.SetColor("_BaseColor", c);
            glass.sharedMaterials = glassMats.Select(m => m != null && m.name == "Glass" ? translucent : m).ToArray();
            Shots("V2_authored_recess");
            recess.SetActive(false);
            // V3: interior-mapping shader on the glass itself (fake room, non-enterable only)
            var interior = new Material(AssetDatabase.LoadAssetAtPath<Shader>("Assets/H2F01/Shaders/H2F01InteriorWindow.shader"));
            r.shader = interior.shader.name; r.shaderSupported = interior.shader.isSupported;
            interior.SetVector("_RoomOrigin", gb.center);
            interior.SetVector("_RoomRight", right);
            interior.SetVector("_RoomIn", -outward);
            glass.sharedMaterials = glassMats.Select(m => m != null && m.name == "Glass" ? interior : m).ToArray();
            Shots("V3_interior_mapping");
            r.auditGreen = AuditShaders().green;
            glass.sharedMaterials = glassMats;
            // Real enterable Bar F01: exterior -> threshold -> interior (not a fake)
            float barY = RoadY(0, 22);
            Shot("s05/F01_1_exterior.png", new Vector3(0, barY + 1.65f, 14), new Vector3(0, barY + 2.4f, 26));
            Shot("s05/F01_2_threshold.png", new Vector3(0, barY + 1.65f, 22.6f), new Vector3(0, barY + 1.3f, 29));
            Shot("s05/F01_3_interior.png", new Vector3(0.6f, barY + 1.9f, 26.2f), new Vector3(-2.5f, barY + 1.2f, 31.5f));
            Shot("s05/F01_4_interior_back_to_door.png", new Vector3(-1.5f, barY + 1.7f, 31f), new Vector3(0, barY + 1.3f, 24f));
            AnimationMode.StopAnimationMode();
            WriteJson("s05/result.json", JsonUtility.ToJson(r, true));
            Log($"H2F01_S05_DONE window={r.windowPiece} glass={r.glassSize} shader={r.shaderSupported} audit={r.auditGreen}");
        }

        static float RoadY(float x, float z) => Physics.Raycast(new Vector3(x, 40, z), Vector3.down, out var h, 80) ? h.point.y : 0;

        static void Shot(string path, Vector3 pos, Vector3 target)
        {
            var cam = CameraAt("s05_cam", pos, target, 55);
            cam.GetUniversalAdditionalCameraData().renderPostProcessing = true;
            Image(cam, path);
            UnityEngine.Object.DestroyImmediate(cam.gameObject);
        }
    }
}
