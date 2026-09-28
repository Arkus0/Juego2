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
    /// Deterministic seeded scatter of admitted prefabs (H2F-01 S04). A placement must land on ground within the
    /// slope limit and keep <see cref="J2ScatterProfile.exclusionMargin"/> from every no-go collider: any generated
    /// traversable/edge realization, and anything carrying <see cref="J2ScatterExclusion"/> (structures, water).
    /// Same area + profile + seed + no-go set => the same placements (output digest).
    /// </summary>
    public static class J2ScatterRealizer
    {
        public const string Generator = "juego2.seeded-scatter";
        public const int GeneratorVersion = 1;

        public static GameObject Realize(Transform host, Bounds area, J2ScatterProfile profile, int seed)
        {
            if (host == null || profile == null) throw new ArgumentNullException(host == null ? nameof(host) : nameof(profile));
            var entries = profile.entries.Where(e => e.prefab != null && e.weight > 0).ToArray();
            if (entries.Length == 0) throw new InvalidOperationException("J2_SCATTER_PROFILE_EMPTY");
            var name = $"J2_Scatter_{profile.name}_{seed}";
            var existing = host.Find(name);
            if (existing != null) UnityEngine.Object.DestroyImmediate(existing.gameObject);
            Physics.SyncTransforms();

            var root = new GameObject(name);
            root.transform.SetParent(host, false);
            var rng = new System.Random(seed);
            float cell = profile.minSpacing;
            int nx = Mathf.Max(1, Mathf.FloorToInt(area.size.x / cell)), nz = Mathf.Max(1, Mathf.FloorToInt(area.size.z / cell));
            float total = entries.Sum(e => e.weight);
            var placed = new List<Vector3>();
            var digest = new StringBuilder(Generator).Append('/').Append(GeneratorVersion).Append('\n');
            for (int ix = 0; ix < nx; ix++)
                for (int iz = 0; iz < nz; iz++)
                {
                    // one jittered candidate per cell, consumed in fixed order so the sequence is seed-deterministic
                    double keep = rng.NextDouble(), jx = rng.NextDouble(), jz = rng.NextDouble(), pick = rng.NextDouble() * total,
                        yaw = rng.NextDouble(), scale = rng.NextDouble();
                    if (keep > profile.density) continue;
                    var candidate = new Vector3(area.min.x + (ix + (float)jx) * cell, area.max.y + 1f, area.min.z + (iz + (float)jz) * cell);
                    if (!Ground(candidate, area, profile, out var hit)) continue;
                    if (NoGo(hit.point, profile.exclusionMargin)) continue;
                    if (placed.Any(p => (p - hit.point).sqrMagnitude < profile.minSpacing * profile.minSpacing)) continue;
                    var entry = entries.Last();
                    float acc = 0;
                    foreach (var e in entries) { acc += e.weight; if (pick <= acc) { entry = e; break; } }
                    var instance = (GameObject)PrefabUtility.InstantiatePrefab(entry.prefab, root.transform);
                    float s = Mathf.Lerp(entry.scaleBand.x <= 0 ? 1 : entry.scaleBand.x, entry.scaleBand.y <= 0 ? 1 : entry.scaleBand.y, (float)scale);
                    var rotation = Quaternion.Euler(0, (float)yaw * 360f, 0);
                    if (entry.alignToGround) rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * rotation;
                    instance.transform.SetPositionAndRotation(hit.point, rotation * entry.prefab.transform.localRotation);
                    instance.transform.localScale = Vector3.Scale(entry.prefab.transform.localScale, Vector3.one * s);
                    placed.Add(hit.point);
                    digest.Append(entry.prefab.name).Append('@').Append(F(hit.point.x)).Append(',').Append(F(hit.point.y)).Append(',')
                          .Append(F(hit.point.z)).Append(':').Append(F((float)yaw)).Append(':').Append(F(s)).Append('\n');
                }

            var marker = root.AddComponent<J2GeneratedRealization>();
            marker.generator = Generator;
            marker.generatorVersion = GeneratorVersion;
            marker.inputDigest = J2LinearRealizer.Sha(EditorJsonUtility.ToJson(profile) + "|" + seed + "|" + area.center.ToString("F4") + area.size.ToString("F4"));
            marker.outputDigest = J2LinearRealizer.Sha(digest.ToString());
            return root;
        }

        static bool Ground(Vector3 from, Bounds area, J2ScatterProfile profile, out RaycastHit hit)
        {
            var hits = Physics.RaycastAll(from, Vector3.down, area.size.y + 2f, profile.groundLayers)
                .Where(h => !(h.collider.GetComponentInParent<J2GeneratedRealization>() is J2GeneratedRealization g && g.generator == Generator))
                .OrderBy(h => h.distance).ToArray();
            hit = hits.FirstOrDefault();
            if (hits.Length == 0) return false;
            if (IsNoGo(hit.collider)) return false; // landed on a road, structure or water
            return Vector3.Angle(hit.normal, Vector3.up) <= profile.maxGroundSlope;
        }

        /// <summary>The margin is measured from no-go colliders at ground level; it must also clear kerbs, which carry no collider.</summary>
        static bool NoGo(Vector3 point, float margin) =>
            Physics.OverlapSphere(point, Mathf.Max(0.01f, margin)).Any(IsNoGo);

        public static bool IsNoGo(Collider c) =>
            c.GetComponentInParent<J2ScatterExclusion>() != null ||
            c.GetComponentInParent<J2GeneratedRealization>() is J2GeneratedRealization g && g.generator != Generator;

        static string F(float v) => Math.Round(v, 4).ToString("0.####", CultureInfo.InvariantCulture);
    }
}
