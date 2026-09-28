using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Proto.Build
{
    /// Winding: Quad(a,b,c,d) faces Cross(b-a, d-a) (Unity front = clockwise).
    /// Accumulates procedural geometry with world-space UVs (continuous across pieces) in several submeshes.
    public class MeshKit
    {
        readonly List<Vector3> v = new List<Vector3>();
        readonly List<Vector3> n = new List<Vector3>();
        readonly List<Vector2> uv = new List<Vector2>();
        readonly List<List<int>> subs = new List<List<int>>();
        readonly List<Material> mats = new List<Material>();
        readonly Dictionary<Material, int> matIndex = new Dictionary<Material, int>();
        public float Tile = 2f;

        public int VertexCount => v.Count;

        int Sub(Material m)
        {
            if (!matIndex.TryGetValue(m, out int i)) { i = subs.Count; subs.Add(new List<int>()); mats.Add(m); matIndex[m] = i; }
            return i;
        }

        Vector2 UV(Vector3 p, Vector3 nn, float tile)
        {
            if (Mathf.Abs(nn.y) > 0.72f) return new Vector2(p.x, p.z) / tile;
            var t = Vector3.Cross(Vector3.up, nn).normalized;
            return new Vector2(Vector3.Dot(p, t), p.y) / tile;
        }

        public void Quad(Material m, Vector3 a, Vector3 b, Vector3 c, Vector3 d, float tile = -1)
        {
            if (tile <= 0) tile = Tile;
            var nn = Vector3.Cross(b - a, d - a).normalized;
            if (nn.sqrMagnitude < 0.5f) nn = Vector3.Cross(c - b, a - b).normalized;
            int s = Sub(m), i = v.Count;
            foreach (var p in new[] { a, b, c, d }) { v.Add(p); n.Add(nn); uv.Add(UV(p, nn, tile)); }
            subs[s].AddRange(new[] { i, i + 1, i + 3, i + 1, i + 2, i + 3 });
        }

        /// Quad with explicit UVs (signs, posters). Face normal = Cross(b-a, d-a).
        public void QuadUV(Material m, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector2 ua, Vector2 ub, Vector2 uc, Vector2 ud)
        {
            var nn = Vector3.Cross(b - a, d - a).normalized;
            int s = Sub(m), i = v.Count;
            v.Add(a); v.Add(b); v.Add(c); v.Add(d);
            for (int k = 0; k < 4; k++) n.Add(nn);
            uv.Add(ua); uv.Add(ub); uv.Add(uc); uv.Add(ud);
            subs[s].AddRange(new[] { i, i + 1, i + 3, i + 1, i + 2, i + 3 });
        }

        public void Tri(Material m, Vector3 a, Vector3 b, Vector3 c, float tile = -1)
        {
            if (tile <= 0) tile = Tile;
            var nn = Vector3.Cross(b - a, c - a).normalized;
            int s = Sub(m), i = v.Count;
            foreach (var p in new[] { a, b, c }) { v.Add(p); n.Add(nn); uv.Add(UV(p, nn, tile)); }
            subs[s].AddRange(new[] { i, i + 1, i + 2 });
        }

        /// Oriented box: centre, right/up/forward half-extent vectors.
        public void Box(Material m, Vector3 c, Vector3 r, Vector3 u, Vector3 f, bool bottom = false, float tile = -1)
        {
            if (Vector3.Dot(Vector3.Cross(u, f), r) < 0) r = -r;      // keep a proper (non-mirrored) basis
            Vector3 P(int x, int y, int z) => c + r * x + u * y + f * z;
            Quad(m, P(-1, 1, -1), P(-1, 1, 1), P(1, 1, 1), P(1, 1, -1), tile);     // top (+u)
            Quad(m, P(-1, -1, -1), P(-1, 1, -1), P(1, 1, -1), P(1, -1, -1), tile);  // front (-f)
            Quad(m, P(1, -1, 1), P(1, 1, 1), P(-1, 1, 1), P(-1, -1, 1), tile);      // back (+f)
            Quad(m, P(-1, -1, 1), P(-1, 1, 1), P(-1, 1, -1), P(-1, -1, -1), tile);  // left (-r)
            Quad(m, P(1, -1, -1), P(1, 1, -1), P(1, 1, 1), P(1, -1, 1), tile);      // right (+r)
            if (bottom) Quad(m, P(-1, -1, -1), P(1, -1, -1), P(1, -1, 1), P(-1, -1, 1), tile);
        }

        /// Axis-aligned-in-plan box from a base segment a->b (plan), width w to the left side, y0..y1.
        public void Slab(Material m, Vector2 a, Vector2 b, float w, float y0, float y1, float tile = -1)
        {
            var d = (b - a); float L = d.magnitude; if (L < 1e-4f) return;
            d /= L; var nn = Geo.Perp(d);
            var c2 = (a + b) * 0.5f + nn * (w * 0.5f);
            Box(m, new Vector3(c2.x, (y0 + y1) * 0.5f, c2.y), new Vector3(d.x, 0, d.y) * (L * 0.5f), Vector3.up * ((y1 - y0) * 0.5f), new Vector3(nn.x, 0, nn.y) * (w * 0.5f), false, tile);
        }

        public GameObject Build(string name, Transform parent, bool collider, bool castShadows = true, bool isStatic = true)
        {
            var mesh = new Mesh { name = name, indexFormat = v.Count > 65000 ? IndexFormat.UInt32 : IndexFormat.UInt16 };
            mesh.SetVertices(v);
            mesh.SetNormals(n);
            mesh.SetUVs(0, uv);
            mesh.subMeshCount = subs.Count;
            for (int i = 0; i < subs.Count; i++) mesh.SetTriangles(subs[i], i);
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            var mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterials = mats.ToArray();
            mr.shadowCastingMode = castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
            if (collider) go.AddComponent<MeshCollider>().sharedMesh = mesh;
            go.isStatic = isStatic;
            Meshes.Add(mesh);
            return go;
        }

        /// Every generated mesh, so the scene builder can persist them as assets.
        public static readonly List<Mesh> Meshes = new List<Mesh>();
    }
}
