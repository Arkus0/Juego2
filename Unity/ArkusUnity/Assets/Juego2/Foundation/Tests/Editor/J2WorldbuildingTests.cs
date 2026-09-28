using System;
using System.Linq;
using Juego2.Foundation.Editor;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Splines;

namespace Juego2.Foundation.Tests
{
    public sealed class J2WorldbuildingTests
    {
        const string TempDir = "Assets/Juego2/Foundation/Tests/_Temp";

        [SetUp]
        public void SetUp() => EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        [TearDown]
        public void TearDown()
        {
            if (AssetDatabase.IsValidFolder(TempDir)) AssetDatabase.DeleteAsset(TempDir);
        }

        static J2LinearProfile Profile(string name, float width)
        {
            var p = ScriptableObject.CreateInstance<J2LinearProfile>();
            p.name = name;
            p.width = width;
            return p;
        }

        static SplineContainer Spline(string name, params Vector3[] points)
        {
            var c = new GameObject(name).AddComponent<SplineContainer>();
            c.Spline.Clear();
            foreach (var p in points) c.Spline.Add(new BezierKnot((float3)p), TangentMode.AutoSmooth);
            return c;
        }

        [Test]
        public void LinearProfile_IsDeterministicAndDetectsStaleInputs()
        {
            var profile = Profile("lane", 2.8f);
            var spline = Spline("lane", new Vector3(0, 0, 0), new Vector3(1, 0.6f, 10), new Vector3(0, 1.4f, 20));
            var first = J2LinearRealizer.Realize(spline, profile).GetComponent<J2GeneratedRealization>();
            string input = first.inputDigest, output = first.outputDigest;
            var second = J2LinearRealizer.Realize(spline, profile).GetComponent<J2GeneratedRealization>();
            Assert.That(second.inputDigest, Is.EqualTo(input));
            Assert.That(second.outputDigest, Is.EqualTo(output));
            Assert.That(spline.transform.Cast<Transform>().Count(t => t.name.StartsWith("J2_Realized_")), Is.EqualTo(1), "re-realization replaces, never duplicates");
            Assert.That(second.GetComponentsInChildren<MeshCollider>().Select(c => c.name), Is.EqualTo(new[] { "surface_traversable" }), "kerbs carry no collider");
            var knot = spline.Spline[1];
            knot.Position += new float3(0, 0.2f, 0);
            spline.Spline[1] = knot;
            Assert.That(J2LinearRealizer.InputDigest(spline, profile, new J2LinearRealizer.Options()), Is.Not.EqualTo(input));
        }

        static J2JunctionRealizer.Result RotatedTee(float headingDeg, float slope)
        {
            var main = Profile("main", 5.5f);
            var branch = Profile("branch", 2.8f);
            var rot = Quaternion.Euler(0, headingDeg, 0);
            Vector3 P(float along, float lateral) => rot * new Vector3(lateral, along * slope, along);
            var mainSpline = Spline("main", P(-14, 0), P(0, 0), P(14, 0));
            var branchSpline = Spline("branch", P(0, 0), P(0, 8) + Vector3.up * 0.2f, P(0, 16) + Vector3.up * 0.6f);
            return J2JunctionRealizer.Realize(mainSpline, main, mainSpline.CalculateLength() / 2f, branchSpline, branch, 1.2f);
        }

        static J2SurfaceContinuity.Report Survey(J2JunctionRealizer.Result r)
        {
            var mainSpline = r.main.GetComponentInParent<SplineContainer>();
            var at = J2LinearRealizer.FrameAt(mainSpline, r.mainDistance);
            Vector3 f = Vector3.ProjectOnPlane(at.forward, Vector3.up).normalized, n = at.right * r.side;
            const float mainHalf = 5.5f / 2, branchHalf = 2.8f / 2, kerb = 0.2f, radius = 1.2f, eps = 0.06f;
            float open = branchHalf + kerb + radius;
            Vector3 origin = at.position - f * 10f - n * mainHalf;
            bool Expected(Vector3 p)
            {
                var d = Vector3.ProjectOnPlane(p - at.position, Vector3.up);
                float u = Vector3.Dot(d, f), v = Vector3.Dot(d, n);
                if (Mathf.Abs(u) > 9.5f) return false;
                if (v >= -mainHalf + eps && v <= mainHalf - eps) return true;                 // main road surface
                if (v > r.branchStart + eps) return Mathf.Abs(u) <= branchHalf - eps && v < r.branchStart + 6f; // branch
                if (v < mainHalf || Mathf.Abs(u) > open - eps) return false;                   // kerb line / beyond the opening
                foreach (int k in new[] { -1, 1 })                                               // outside the corner discs
                {
                    var c = new Vector2(k * open, r.branchStart);
                    if ((new Vector2(u, v) - c).magnitude < radius + kerb + eps) return false;
                }
                return true;                                                                      // junction apron
            }
            return J2SurfaceContinuity.Sample(origin, f, n, 20f, mainHalf * 2 + r.branchStart + 6f, 0.1f, Expected);
        }

        [TestCase(0f, 0f)]
        [TestCase(30f, 0.06f)]
        [TestCase(-117f, 0.04f)]
        public void TeeJunction_HasNoHolesOrStepsAtAnyHeading(float heading, float slope)
        {
            var result = RotatedTee(heading, slope);
            var report = Survey(result);
            Assert.That(report.samples, Is.GreaterThan(2000));
            Assert.That(report.holes, Is.Zero, $"holes {report.holes}/{report.samples}");
            Assert.That(report.steps, Is.Zero, $"max step {report.maxStepFound:F3}");
            Assert.That(result.junction.GetComponent<J2GeneratedRealization>().generator, Is.EqualTo(J2JunctionRealizer.Generator));
            // kerb fillets are single-sided with well-formed normals (a double-sided band averaged to degenerate, black normals)
            var fillets = result.junction.transform.Find("kerb_fillets").GetComponent<MeshFilter>().sharedMesh;
            Assert.That(fillets.normals.All(n => n.magnitude > 0.99f), Is.True, "degenerate fillet normals");
            Assert.That(fillets.normals.Count(n => n.y > 0.99f), Is.GreaterThan(0), "fillet top faces must face up");
            Assert.That(fillets.triangles.Length / 3, Is.EqualTo(2 * 8 * 3 * 2), "one quad per face, no duplicated back faces");
        }

        [Test]
        public void TeeJunction_OracleDetectsAMissingApron()
        {
            var result = RotatedTee(30f, 0.06f);
            UnityEngine.Object.DestroyImmediate(result.junction.transform.Find("apron_traversable").gameObject);
            Assert.That(Survey(result).holes, Is.GreaterThan(0), "negative control: the oracle must see the hole");
        }

        [Test]
        public void TeeJunction_RejectsAnObliqueBranch()
        {
            var main = Spline("main", new Vector3(0, 0, -10), new Vector3(0, 0, 10));
            var oblique = Spline("oblique", new Vector3(0, 0, 0), new Vector3(6, 0, 6));
            var ex = Assert.Throws<InvalidOperationException>(() =>
                J2JunctionRealizer.Realize(main, Profile("m", 5.5f), 10f, oblique, Profile("b", 2.8f), 1.2f));
            Assert.That(ex.Message, Is.EqualTo("J2_JUNCTION_NOT_TEE"));
        }

        static GameObject TempPrefab()
        {
            J2FoundationBaseline.EnsureFolder(TempDir);
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.transform.localScale = new Vector3(0.3f, 0.5f, 0.3f);
            UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, TempDir + "/Shrub.prefab");
            UnityEngine.Object.DestroyImmediate(go);
            return prefab;
        }

        static (Transform host, SplineContainer road, GameObject pond) ScatterScene()
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.transform.localScale = new Vector3(4, 1, 4); // 40 x 40 m
            var road = Spline("road", new Vector3(-20, 0.01f, 0), new Vector3(20, 0.01f, 0));
            J2LinearRealizer.Realize(road, Profile("road", 3f));
            var pond = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pond.transform.position = new Vector3(10, 0, 10);
            pond.transform.localScale = new Vector3(6, 0.2f, 6);
            pond.AddComponent<J2ScatterExclusion>();
            return (new GameObject("nature").transform, road, pond);
        }

        [Test]
        public void Scatter_IsSeedDeterministicAndKeepsOutOfNoGoAreas()
        {
            var (host, road, pond) = ScatterScene();
            var profile = ScriptableObject.CreateInstance<J2ScatterProfile>();
            profile.name = "shrubs";
            profile.entries = new[] { new J2ScatterProfile.Entry { prefab = TempPrefab(), weight = 1, scaleBand = new Vector2(0.8f, 1.2f) } };
            profile.density = 0.5f;
            profile.exclusionMargin = 0.6f;
            var area = new Bounds(Vector3.zero, new Vector3(36, 4, 36));
            var a = J2ScatterRealizer.Realize(host, area, profile, 7).GetComponent<J2GeneratedRealization>();
            string digest = a.outputDigest;
            int count = a.transform.childCount;
            Assert.That(count, Is.GreaterThan(50));
            foreach (Transform t in a.transform)
            {
                // the margin is measured from the traversable surface (1.5 m half-width) and must also clear the 0.2 m kerb
                Assert.That(Mathf.Abs(t.position.z), Is.GreaterThanOrEqualTo(1.5f + 0.6f - 0.02f), "inside the road margin: " + t.position);
                Assert.That(Mathf.Abs(t.position.x - 10) < 3 && Mathf.Abs(t.position.z - 10) < 3, Is.False, "inside the declared exclusion: " + t.position);
            }
            Assert.That(J2ScatterRealizer.Realize(host, area, profile, 7).GetComponent<J2GeneratedRealization>().outputDigest, Is.EqualTo(digest));
            Assert.That(J2ScatterRealizer.Realize(host, area, profile, 8).GetComponent<J2GeneratedRealization>().outputDigest, Is.Not.EqualTo(digest));
        }
    }
}
