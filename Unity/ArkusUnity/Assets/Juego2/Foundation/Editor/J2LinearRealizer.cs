using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Juego2.Foundation;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace Juego2.Foundation.Editor
{
    /// <summary>
    /// Project-owned linear-feature realization over a Unity Splines centreline (H2F-01 S02): a crowned traversable
    /// surface that is the only collider, kerbs or retaining walls outside it, openings for junctions. Output is
    /// generated realization under a <see cref="J2GeneratedRealization"/> root: deterministic for the same spline,
    /// profile and options, and detectably stale when an input changes. CITY/ART own where and what; this owns how.
    /// </summary>
    public static class J2LinearRealizer
    {
        public const string Generator = "juego2.linear-profile";
        public const int GeneratorVersion = 1;

        public sealed class Options
        {
            public float start;
            public float end = -1f;
            public List<Vector2> leftOpenings = new List<Vector2>();
            public List<Vector2> rightOpenings = new List<Vector2>();
        }

        public struct Frame
        {
            public Vector3 position, forward, right;
        }

        public static Frame FrameAt(SplineContainer container, float distance)
        {
            var length = container.CalculateLength();
            float t = Mathf.Clamp01(length <= 0 ? 0 : distance / length);
            var position = (Vector3)container.EvaluatePosition(t);
            var forward = ((Vector3)container.EvaluateTangent(t)).normalized;
            var right = Vector3.Cross(Vector3.up, forward).normalized;
            return new Frame { position = position, forward = forward, right = right };
        }

        public static GameObject Realize(SplineContainer container, J2LinearProfile profile, Options options = null)
        {
            if (container == null || profile == null) throw new ArgumentNullException(container == null ? nameof(container) : nameof(profile));
            if (profile.crown == null || profile.crown.Length != 5) throw new ArgumentException("J2_PROFILE_CROWN_NEEDS_5_SAMPLES");
            options ??= new Options();
            var name = "J2_Realized_" + profile.name;
            var existing = container.transform.Find(name);
            if (existing != null) UnityEngine.Object.DestroyImmediate(existing.gameObject);
            var root = new GameObject(name);
            root.transform.SetParent(container.transform, false);

            float length = container.CalculateLength();
            float d0 = Mathf.Clamp(options.start, 0, length), d1 = options.end < 0 ? length : Mathf.Clamp(options.end, d0, length);
            float half = profile.width / 2f;
            float[] lateral = { -half, -half / 2f, 0f, half / 2f, half };

            var sv = new List<Vector3>(); var suv = new List<Vector2>(); var st = new List<int>();
            var ev = new List<Vector3>(); var et = new List<int>();
            var wv = new List<Vector3>(); var wt = new List<int>();
            int rows = Mathf.Max(1, Mathf.CeilToInt((d1 - d0) / profile.sampleStep));
            Frame previous = default;
            float previousDistance = d0;
            for (int r = 0; r <= rows; r++)
            {
                float d = Mathf.Min(d0 + r * profile.sampleStep, d1);
                var f = FrameAt(container, d);
                for (int k = 0; k < lateral.Length; k++)
                {
                    sv.Add(f.position + f.right * lateral[k] + Vector3.up * profile.crown[k]);
                    suv.Add(new Vector2(lateral[k] / profile.uvScale, d / profile.uvScale));
                }
                if (r > 0)
                {
                    for (int k = 0; k < lateral.Length - 1; k++)
                    {
                        int a = (r - 1) * lateral.Length + k, b = a + 1, e = a + lateral.Length, g = e + 1;
                        st.AddRange(new[] { a, e, b, b, e, g });
                    }
                    float mid = (previousDistance + d) / 2f;
                    Edge(profile.leftEdge, -1, options.leftOpenings, mid, previous, f, profile, ev, et, wv, wt);
                    Edge(profile.rightEdge, 1, options.rightOpenings, mid, previous, f, profile, ev, et, wv, wt);
                }
                previous = f;
                previousDistance = d;
            }

            var surface = MeshObject(root.transform, "surface_traversable", sv, suv, st, profile.surfaceMaterial, collider: true);
            surface.GetComponent<MeshRenderer>().renderingLayerMask = J2RenderingLayers.Default | profile.surfaceRenderingLayers;
            if (et.Count > 0) MeshObject(root.transform, "edge_kerbs", ev, WorldUv(ev, profile.uvScale), et, profile.edgeMaterial ?? profile.surfaceMaterial, collider: false);
            if (wt.Count > 0) MeshObject(root.transform, "edge_retaining_walls", wv, WorldUv(wv, profile.uvScale), wt, profile.wallMaterial ?? profile.edgeMaterial, collider: true);

            var marker = root.AddComponent<J2GeneratedRealization>();
            marker.generator = Generator;
            marker.generatorVersion = GeneratorVersion;
            marker.inputDigest = InputDigest(container, profile, options);
            marker.outputDigest = OutputDigest(root);
            return root;
        }

        static void Edge(J2LinearProfile.EdgeKind kind, int side, List<Vector2> openings, float mid, Frame a, Frame b, J2LinearProfile p,
            List<Vector3> ev, List<int> et, List<Vector3> wv, List<int> wt)
        {
            if (kind == J2LinearProfile.EdgeKind.None || (openings != null && openings.Any(o => mid >= o.x && mid <= o.y))) return;
            float half = p.width / 2f;
            if (kind == J2LinearProfile.EdgeKind.Kerb)
            {
                // profile (lateral from the traversable edge, height): inner foot, inner top, outer top, embedded outer foot
                (float l, float h)[] prof = { (0, 0), (0, p.kerbHeight), (p.kerbWidth, p.kerbHeight), (p.kerbWidth, -p.kerbEmbed) };
                Band(ev, et, a, b, side, half, prof);
            }
            else
            {
                float inner = half + p.kerbWidth;
                (float l, float h)[] prof = { (0, p.wallParapet), (p.wallThickness, p.wallParapet), (p.wallThickness, -p.wallDrop) };
                Band(wv, wt, a, b, side, inner, prof);
            }
        }

        static void Band(List<Vector3> v, List<int> t, Frame a, Frame b, int side, float offset, (float l, float h)[] prof)
        {
            int start = v.Count;
            foreach (var f in new[] { a, b })
                foreach (var (l, h) in prof)
                    v.Add(f.position + f.right * side * (offset + l) + Vector3.up * h);
            for (int k = 0; k < prof.Length - 1; k++)
            {
                int i0 = start + k, i1 = i0 + 1, j0 = start + prof.Length + k, j1 = j0 + 1;
                if (side > 0) t.AddRange(new[] { i0, i1, j0, i1, j1, j0 });
                else t.AddRange(new[] { i0, j0, i1, i1, j0, j1 });
            }
        }

        internal static List<Vector2> WorldUv(List<Vector3> v, float scale) =>
            v.Select(p => new Vector2((p.x + p.z) / scale, p.y / scale)).ToList();

        /// <summary>A mesh object under <paramref name="parent"/> built from world-space vertices (independent of the host transform).</summary>
        internal static GameObject MeshObject(Transform parent, string name, List<Vector3> v, List<Vector2> uv, List<int> t, Material m, bool collider)
        {
            var toLocal = parent.worldToLocalMatrix;
            var mesh = new Mesh { name = name, indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
            mesh.SetVertices(v.Select(p => (Vector3)toLocal.MultiplyPoint3x4(p)).ToList());
            if (uv != null) mesh.SetUVs(0, uv);
            mesh.SetTriangles(t, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            go.AddComponent<MeshRenderer>().sharedMaterial = m;
            if (collider) go.AddComponent<MeshCollider>().sharedMesh = mesh;
            return go;
        }

        public static string InputDigest(SplineContainer container, J2LinearProfile p, Options o)
        {
            var s = new StringBuilder(Generator).Append('/').Append(GeneratorVersion).Append('\n');
            var m = container.transform.localToWorldMatrix;
            for (int i = 0; i < 16; i++) s.Append(F(m[i])).Append(',');
            s.Append('\n');
            foreach (var spline in container.Splines)
                foreach (var knot in spline.Knots)
                    s.Append(V(knot.Position)).Append(';').Append(V(knot.TangentIn)).Append(';').Append(V(knot.TangentOut)).Append(';')
                     .Append(F(knot.Rotation.value.x)).Append(',').Append(F(knot.Rotation.value.y)).Append(',')
                     .Append(F(knot.Rotation.value.z)).Append(',').Append(F(knot.Rotation.value.w)).Append('\n');
            s.Append(EditorJsonUtility.ToJson(p)).Append('\n');
            s.Append(F(o.start)).Append(',').Append(F(o.end)).Append('|');
            foreach (var v in o.leftOpenings) s.Append(F(v.x)).Append('-').Append(F(v.y)).Append(',');
            s.Append('|');
            foreach (var v in o.rightOpenings) s.Append(F(v.x)).Append('-').Append(F(v.y)).Append(',');
            return Sha(s.ToString());
        }

        /// <summary>Digest of every generated vertex/index in the subtree (the realized geometry).</summary>
        public static string OutputDigest(GameObject root)
        {
            var s = new StringBuilder();
            foreach (var filter in root.GetComponentsInChildren<MeshFilter>(true).OrderBy(f => f.name, StringComparer.Ordinal))
            {
                var mesh = filter.sharedMesh;
                s.Append(filter.name).Append(':').Append(mesh.vertexCount).Append('\n');
                foreach (var v in mesh.vertices) s.Append(F(v.x)).Append(',').Append(F(v.y)).Append(',').Append(F(v.z)).Append(';');
                s.Append('\n').Append(string.Join(",", mesh.triangles)).Append('\n');
            }
            return Sha(s.ToString());
        }

        static string V(float3 v) => F(v.x) + "," + F(v.y) + "," + F(v.z);

        static string F(float v) => Math.Round(v, 4).ToString("0.####", CultureInfo.InvariantCulture);

        internal static string Sha(string text)
        {
            using (var sha = SHA256.Create())
                return string.Concat(sha.ComputeHash(Encoding.UTF8.GetBytes(text)).Select(b => b.ToString("x2")));
        }
    }
}
