using System.Linq;
using Juego2.Foundation.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Juego2.Foundation.Tests
{
    public sealed class J2RenderingTests
    {
        [Test]
        public void ShaderClassification_SeparatesUrpFromBuiltinOnlyAndMissing()
        {
            Assert.That(J2ShaderAudit.Classify(Shader.Find("Universal Render Pipeline/Lit")), Is.EqualTo("URP_OK"));
            Assert.That(J2ShaderAudit.Classify(Shader.Find("Standard")), Is.Not.EqualTo("URP_OK"));
            Assert.That(J2ShaderAudit.Classify(Shader.Find("Legacy Shaders/Diffuse")), Is.Not.EqualTo("URP_OK"));
            Assert.That(J2ShaderAudit.Classify(null), Is.EqualTo("MISSING_SHADER"));
            Assert.That(J2ShaderAudit.Classify(Shader.Find("Skybox/Panoramic")), Is.EqualTo("URP_OK"));
            Assert.That(J2ShaderAudit.Classify(Shader.Find("GUI/Text Shader")), Is.EqualTo("URP_OK"), "untagged unlit passes draw as SRPDefaultUnlit");
        }

        [TestCase("Juego2/StylizedWater")]
        [TestCase("Juego2/InteriorWindow")]
        public void ProjectShaders_AreSupportedUrpShaders(string name)
        {
            var shader = Shader.Find(name);
            Assert.That(shader, Is.Not.Null, name);
            Assert.That(shader.isSupported, Is.True);
            Assert.That(J2ShaderAudit.Classify(shader), Is.EqualTo("URP_OK"));
        }

        /// <summary>Retained project content (foundation + CITY-04 greybox) has no hidden built-in or missing shader.</summary>
        [Test]
        public void RetainedProjectMaterials_AreAllUrp()
        {
            var report = J2ShaderAudit.Audit(J2ShaderAudit.MaterialAssets("Assets/Juego2", "Assets/Arkus/CITY"));
            Assert.That(report.materials, Is.GreaterThan(10));
            Assert.That(report.failures, Is.Empty, string.Join("\n", report.failures));
        }

        [Test]
        public void ShaderAudit_ReportsATerrainPrototypeThatRendererAuditsMiss()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var tree = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tree.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Standard")) { name = "tree_builtin" };
            var data = new TerrainData { treePrototypes = new[] { new TreePrototype { prefab = tree } } };
            var terrain = Terrain.CreateTerrainGameObject(data).GetComponent<Terrain>();
            terrain.materialTemplate = new Material(Shader.Find("Universal Render Pipeline/Terrain/Lit"));
            tree.SetActive(false);
            tree.transform.SetParent(null);
            var report = J2ShaderAudit.Audit(J2ShaderAudit.SceneMaterials(EditorSceneManager.GetActiveScene()).Where(m => m.Item2.StartsWith("terrain")));
            Assert.That(report.failures.Any(f => f.Contains("tree_builtin") && f.Contains("terrain-tree")), Is.True, string.Join("\n", report.failures));
        }

        [Test]
        public void ImportConventions_FailClosedOnMissingModels()
        {
            var findings = J2ImportConventions.Verify(new[] { "Assets/Missing/Body.fbx" }, new[] { "Assets/Missing/UAL.fbx" });
            Assert.That(findings, Has.Member("J2_IMPORT_MODEL_MISSING:Assets/Missing/Body.fbx"));
            Assert.That(findings, Has.Member("J2_IMPORT_MODEL_MISSING:Assets/Missing/UAL.fbx"));
            Assert.That(J2ImportConventions.BaseCharacterMapping["Hips"], Is.EqualTo("pelvis"));
        }
    }
}
