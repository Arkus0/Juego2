using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Arkus.H1.Editor.Tests
{
    public sealed class H1BaselineTests
    {
        [Test]
        public void EffectiveEditorMatchesPinnedPatch()
        {
            Assert.That(Application.unityVersion, Is.EqualTo("6000.3.24f1"));
        }

        [Test]
        public void ForceTextAndVisibleMetaPolicyAreEffective()
        {
            Assert.That(EditorSettings.serializationMode, Is.EqualTo(SerializationMode.ForceText));
            Assert.That(EditorSettings.externalVersionControl, Is.EqualTo("Visible Meta Files"));
        }

        [Test]
        public void BuiltInRenderPipelineIsTheRetainedBaseline()
        {
            Assert.That(GraphicsSettings.currentRenderPipeline, Is.Null);
        }

        [Test]
        public void PackagesLockExists()
        {
            var projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            Assert.That(
                File.Exists(Path.Combine(projectRoot, "Packages", "packages-lock.json")),
                Is.True,
                "packages-lock.json must be generated and retained before review.");
        }

        [Test]
        public void RepositoryOwnedUnityCodeHasVisibleMetaFiles()
        {
            var governedExtensions = new[] { ".cs", ".asmdef" };
            var missing = Directory.EnumerateFiles(Application.dataPath, "*", SearchOption.AllDirectories)
                .Where(path => governedExtensions.Contains(
                    Path.GetExtension(path),
                    StringComparer.OrdinalIgnoreCase))
                .Where(path => !File.Exists(path + ".meta"))
                .Select(path => path.Substring(Application.dataPath.Length).TrimStart('\\', '/'))
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();

            Assert.That(missing, Is.Empty, "Missing visible meta files: " + string.Join(", ", missing));
        }
    }
}
