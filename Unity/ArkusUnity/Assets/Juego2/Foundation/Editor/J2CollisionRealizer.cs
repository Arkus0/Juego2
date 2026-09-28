using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Juego2.Foundation;
using UnityEditor;
using UnityEngine;

namespace Juego2.Foundation.Editor
{
    /// <summary>
    /// Physical collision for presentation that carries none. An H1-managed projection holds only H1's allowlisted
    /// components (Transform, MeshRenderer, Animator, canonical link), and the H1 source slice imports without
    /// colliders, so projected canonical architecture cannot stop a character (found by WP-H2F-03's integrated
    /// fixture). The proxies are generated realization in the owner's sidecar: derived from the source renderers'
    /// shared meshes and world transforms, never written into the source scene and never read back as canonical state.
    /// <see cref="J2GeneratedRealization.inputDigest"/> identifies the source geometry, so a rematerialized projection
    /// that moved is detected (<see cref="IsStale"/>) and re-realized by its owner, never hand-repaired. Scatter treats
    /// the proxies as no-go, like every other non-scatter realization.
    /// </summary>
    public static class J2CollisionRealizer
    {
        public const string Generator = "juego2.collision-proxy";
        public const int GeneratorVersion = 1;

        public static GameObject Realize(IEnumerable<Transform> sources, Transform host, string name)
        {
            if (sources == null || host == null) throw new ArgumentNullException(sources == null ? nameof(sources) : nameof(host));
            var list = sources.Where(s => s != null).ToList();
            var rows = Rows(list).ToList();
            if (rows.Count == 0) throw new InvalidOperationException("J2_COLLISION_NO_SOURCE_MESH " + name);
            var rootName = "J2_Collision_" + name;
            var existing = host.Find(rootName);
            if (existing != null) UnityEngine.Object.DestroyImmediate(existing.gameObject);

            var root = new GameObject(rootName);
            root.transform.SetParent(host, false);
            var hostScale = root.transform.lossyScale;
            foreach (var (path, filter) in rows)
            {
                var proxy = new GameObject(path.Replace('/', '|'));
                proxy.transform.SetParent(root.transform, false);
                var source = filter.transform;
                proxy.transform.SetPositionAndRotation(source.position, source.rotation);
                var s = source.lossyScale;
                proxy.transform.localScale = new Vector3(s.x / hostScale.x, s.y / hostScale.y, s.z / hostScale.z);
                proxy.AddComponent<MeshCollider>().sharedMesh = filter.sharedMesh;
            }

            var marker = root.AddComponent<J2GeneratedRealization>();
            marker.generator = Generator;
            marker.generatorVersion = GeneratorVersion;
            marker.inputDigest = InputDigest(list);
            marker.outputDigest = OutputDigest(root);
            return root;
        }

        /// <summary>True when the sources no longer have the geometry the proxy root was realized from.</summary>
        public static bool IsStale(GameObject root, IEnumerable<Transform> sources)
        {
            var marker = root != null ? root.GetComponent<J2GeneratedRealization>() : null;
            return marker == null || marker.generator != Generator || marker.inputDigest != InputDigest(sources.Where(s => s != null).ToList());
        }

        public static string InputDigest(IEnumerable<Transform> sources)
        {
            var s = new StringBuilder(Generator).Append('/').Append(GeneratorVersion).Append('\n');
            foreach (var (path, filter) in Rows(sources))
                s.Append(path).Append('|').Append(MeshIdentity(filter.sharedMesh)).Append('|').Append(Matrix(filter.transform.localToWorldMatrix)).Append('\n');
            return J2LinearRealizer.Sha(s.ToString());
        }

        public static string OutputDigest(GameObject root)
        {
            var s = new StringBuilder();
            foreach (var collider in root.GetComponentsInChildren<MeshCollider>(true).OrderBy(c => c.name, StringComparer.Ordinal))
                s.Append(collider.name).Append('|').Append(MeshIdentity(collider.sharedMesh)).Append('|').Append(Matrix(collider.transform.localToWorldMatrix)).Append('\n');
            return J2LinearRealizer.Sha(s.ToString());
        }

        /// <summary>Every static mesh under the sources, keyed by its path below its source root (sorted, deterministic).</summary>
        static IEnumerable<(string path, MeshFilter filter)> Rows(IEnumerable<Transform> sources)
        {
            var rows = new List<(string, MeshFilter)>();
            foreach (var source in sources)
                foreach (var filter in source.GetComponentsInChildren<MeshFilter>(true))
                {
                    if (filter.sharedMesh == null || filter.GetComponent<MeshRenderer>() == null) continue;
                    rows.Add((Path(source, filter.transform), filter));
                }
            return rows.OrderBy(r => r.Item1, StringComparer.Ordinal);
        }

        static string Path(Transform root, Transform t)
        {
            var parts = new List<string>();
            for (var cursor = t; cursor != null && cursor != root.parent; cursor = cursor.parent) parts.Add(cursor.name);
            parts.Reverse();
            return string.Join("/", parts);
        }

        static string MeshIdentity(Mesh mesh)
        {
            if (mesh == null) return "null";
            var path = AssetDatabase.GetAssetPath(mesh);
            var guid = string.IsNullOrEmpty(path) ? "" : AssetDatabase.AssetPathToGUID(path);
            return guid + ":" + mesh.name + ":" + mesh.vertexCount + ":" + F(mesh.bounds.size.x) + "," + F(mesh.bounds.size.y) + "," + F(mesh.bounds.size.z);
        }

        static string Matrix(Matrix4x4 m)
        {
            var s = new StringBuilder();
            for (int i = 0; i < 16; i++) s.Append(F(m[i])).Append(',');
            return s.ToString();
        }

        static string F(float v) => Math.Round(v, 4).ToString("0.####", CultureInfo.InvariantCulture);
    }
}
