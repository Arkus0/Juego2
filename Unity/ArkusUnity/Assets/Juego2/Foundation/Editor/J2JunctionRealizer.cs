using System;
using System.Collections.Generic;
using System.Linq;
using Juego2.Foundation;
using UnityEngine;
using UnityEngine.Splines;

namespace Juego2.Foundation.Editor
{
    /// <summary>
    /// Tee junction between a main linear feature and a branch that leaves it on one side (H2F-01 S02: Splines
    /// provides no junction mesh). Everything is built in the main road's local frame at the junction, so it works for
    /// any heading/slope (the S02 spike caught an axis-aligned apron defect). It realizes: the main road with a kerb
    /// opening, the branch from its first real cross-section, quarter-circle kerb fillets and a traversable apron from
    /// the main edge to the branch cross-section. Crossroads and oblique branches are outside this bounded generator.
    /// </summary>
    public static class J2JunctionRealizer
    {
        public const string Generator = "juego2.tee-junction";
        public const int GeneratorVersion = 1;
        public const float MaxBranchSkewDegrees = 12f;

        public sealed class Result
        {
            public GameObject main, branch, junction;
            public float mainDistance, branchStart;
            public int side;
        }

        /// <summary>
        /// <paramref name="branch"/> must start on the main centreline at <paramref name="mainDistance"/> and leave
        /// roughly perpendicular (within <see cref="MaxBranchSkewDegrees"/>); otherwise J2_JUNCTION_NOT_TEE.
        /// </summary>
        public static Result Realize(SplineContainer main, J2LinearProfile mainProfile, float mainDistance,
            SplineContainer branch, J2LinearProfile branchProfile, float cornerRadius)
        {
            var at = J2LinearRealizer.FrameAt(main, mainDistance);
            var branchOrigin = J2LinearRealizer.FrameAt(branch, 0f);
            var planar = Vector3.ProjectOnPlane(branchOrigin.position - at.position, Vector3.up);
            if (planar.magnitude > 0.05f) throw new InvalidOperationException("J2_JUNCTION_BRANCH_NOT_ON_MAIN_CENTRELINE");
            float dot = Vector3.Dot(Vector3.ProjectOnPlane(branchOrigin.forward, Vector3.up).normalized, at.right);
            if (Mathf.Abs(dot) < Mathf.Cos(MaxBranchSkewDegrees * Mathf.Deg2Rad)) throw new InvalidOperationException("J2_JUNCTION_NOT_TEE");
            int side = dot > 0 ? 1 : -1;

            float kerb = mainProfile.leftEdge == J2LinearProfile.EdgeKind.None && mainProfile.rightEdge == J2LinearProfile.EdgeKind.None ? 0 : mainProfile.kerbWidth;
            float open = branchProfile.width / 2f + kerb + cornerRadius;
            float branchStart = mainProfile.width / 2f + kerb + cornerRadius;
            var opening = new List<Vector2> { new Vector2(mainDistance - open, mainDistance + open) };
            var mainOptions = new J2LinearRealizer.Options();
            if (side > 0) mainOptions.rightOpenings = opening; else mainOptions.leftOpenings = opening;
            var result = new Result
            {
                main = J2LinearRealizer.Realize(main, mainProfile, mainOptions),
                branch = J2LinearRealizer.Realize(branch, branchProfile, new J2LinearRealizer.Options { start = branchStart }),
                mainDistance = mainDistance,
                branchStart = branchStart,
                side = side,
            };

            var name = "J2_Junction_" + branch.name;
            var existing = main.transform.Find(name);
            if (existing != null) UnityEngine.Object.DestroyImmediate(existing.gameObject);
            var root = new GameObject(name);
            root.transform.SetParent(main.transform, false);
            result.junction = root;

            // Frame: F along the main road, N from the main centreline toward the branch.
            Vector3 f = at.forward, n = at.right * side;
            Vector3 edgeCentre = at.position + n * (mainProfile.width / 2f) + Vector3.up * mainProfile.crown[side > 0 ? 4 : 0];
            var b = J2LinearRealizer.FrameAt(branch, branchStart);
            Vector3 bLeft = b.position - b.right * branchProfile.width / 2f + Vector3.up * branchProfile.crown[0];
            Vector3 bRight = b.position + b.right * branchProfile.width / 2f + Vector3.up * branchProfile.crown[4];
            Vector3 e0 = edgeCentre - f * open, e1 = edgeCentre + f * open;
            // pair each main-edge corner with the branch-side point on the same side of the branch axis
            bool leftIsNegativeF = Vector3.Dot(bLeft - b.position, f) < 0;
            Vector3 b0 = leftIsNegativeF ? bLeft : bRight, b1 = leftIsNegativeF ? bRight : bLeft;
            var av = new List<Vector3> { e0, e1, b0, b1 };
            var at2 = UpFacing(av, new List<int> { 0, 2, 1, 1, 2, 3 });
            var apron = J2LinearRealizer.MeshObject(root.transform, "apron_traversable", av, J2LinearRealizer.WorldUv(av, mainProfile.uvScale), at2, mainProfile.surfaceMaterial, collider: true);
            apron.GetComponent<MeshRenderer>().renderingLayerMask = J2RenderingLayers.Default | mainProfile.surfaceRenderingLayers;

            if (kerb > 0)
            {
                var kv = new List<Vector3>(); var kt = new List<int>();
                foreach (int k in new[] { -1, 1 })
                {
                    Vector3 centre = edgeCentre + f * (k * open) + n * (kerb + cornerRadius);
                    const int segments = 8;
                    for (int s = 0; s < segments; s++)
                    {
                        float a0 = Mathf.PI / 2 * s / segments, a1 = Mathf.PI / 2 * (s + 1) / segments;
                        Vector3 Dir(float a) => -k * f * Mathf.Cos(a) - n * Mathf.Sin(a);
                        Fillet(kv, kt, centre, Dir(a0), Dir(a1), cornerRadius, kerb, mainProfile.kerbHeight, mainProfile.kerbEmbed);
                    }
                }
                J2LinearRealizer.MeshObject(root.transform, "kerb_fillets", kv, J2LinearRealizer.WorldUv(kv, mainProfile.uvScale), kt, mainProfile.edgeMaterial ?? mainProfile.surfaceMaterial, collider: false);
            }

            var marker = root.AddComponent<J2GeneratedRealization>();
            marker.generator = Generator;
            marker.generatorVersion = GeneratorVersion;
            marker.inputDigest = J2LinearRealizer.Sha(result.main.GetComponent<J2GeneratedRealization>().inputDigest + "|" +
                                                      result.branch.GetComponent<J2GeneratedRealization>().inputDigest + "|" + mainDistance + "|" + cornerRadius);
            marker.outputDigest = J2LinearRealizer.OutputDigest(root);
            return result;
        }

        static List<int> UpFacing(List<Vector3> v, List<int> t)
        {
            var normal = Vector3.Cross(v[t[1]] - v[t[0]], v[t[2]] - v[t[0]]);
            if (normal.y >= 0) return t;
            var flipped = new List<int>();
            for (int i = 0; i < t.Count; i += 3) flipped.AddRange(new[] { t[i], t[i + 2], t[i + 1] });
            return flipped;
        }

        static void Fillet(List<Vector3> v, List<int> t, Vector3 centre, Vector3 dirA, Vector3 dirB, float radius, float kerb, float height, float embed)
        {
            // road-facing inner edge at radius+kerb, outer edge at radius (the kerb band sits on the corner).
            // Faces: road-facing side (normal away from the corner centre), top (up), back side (toward the centre).
            (float r, float h)[] ring = { (radius + kerb, 0f), (radius + kerb, height), (radius, height), (radius, -embed) };
            var mid = (dirA + dirB).normalized;
            Vector3[] outward = { mid, Vector3.up, -mid };
            for (int k = 0; k < ring.Length - 1; k++)
            {
                // one quad per face with its own vertices, so flat faces keep crisp, well-defined normals
                int start = v.Count;
                v.Add(centre + dirA * ring[k].r + Vector3.up * ring[k].h);
                v.Add(centre + dirA * ring[k + 1].r + Vector3.up * ring[k + 1].h);
                v.Add(centre + dirB * ring[k].r + Vector3.up * ring[k].h);
                v.Add(centre + dirB * ring[k + 1].r + Vector3.up * ring[k + 1].h);
                var n = Vector3.Cross(v[start + 1] - v[start], v[start + 2] - v[start]);
                if (Vector3.Dot(n, outward[k]) >= 0) t.AddRange(new[] { start, start + 1, start + 2, start + 1, start + 3, start + 2 });
                else t.AddRange(new[] { start, start + 2, start + 1, start + 1, start + 2, start + 3 });
            }
        }
    }

    /// <summary>
    /// Dense traversable-surface oracle (H2F-01 S02): every sample inside the expected traversable region must hit a
    /// traversable collider, and neighbouring samples may not step more than <c>maxStep</c>. It found the S02 apron hole.
    /// </summary>
    public static class J2SurfaceContinuity
    {
        public struct Report
        {
            public int samples, holes, steps;
            public float maxStepFound;
        }

        public static Report Sample(Vector3 origin, Vector3 axisU, Vector3 axisV, float extentU, float extentV, float spacing,
            Func<Vector3, bool> expectedTraversable, float maxStep = 0.05f)
        {
            Physics.SyncTransforms();
            int nu = Mathf.CeilToInt(extentU / spacing), nv = Mathf.CeilToInt(extentV / spacing);
            var heights = new float?[nu + 1, nv + 1];
            var report = new Report();
            for (int i = 0; i <= nu; i++)
                for (int j = 0; j <= nv; j++)
                {
                    var p = origin + axisU * (i * spacing) + axisV * (j * spacing);
                    if (!expectedTraversable(p)) continue;
                    report.samples++;
                    var hits = Physics.RaycastAll(p + Vector3.up * 50f, Vector3.down, 100f)
                        .Where(h => h.collider.GetComponentInParent<J2GeneratedRealization>() != null && h.collider.name.Contains("traversable"))
                        .OrderBy(h => h.distance).ToArray();
                    if (hits.Length == 0) { report.holes++; continue; }
                    heights[i, j] = hits[0].point.y;
                }
            for (int i = 0; i <= nu; i++)
                for (int j = 0; j <= nv; j++)
                {
                    if (heights[i, j] is not float h) continue;
                    foreach (var (di, dj) in new[] { (1, 0), (0, 1) })
                    {
                        int a = i + di, b = j + dj;
                        if (a > nu || b > nv || heights[a, b] is not float g) continue;
                        float step = Mathf.Abs(h - g);
                        report.maxStepFound = Mathf.Max(report.maxStepFound, step);
                        if (step > maxStep) report.steps++;
                    }
                }
            return report;
        }
    }
}
