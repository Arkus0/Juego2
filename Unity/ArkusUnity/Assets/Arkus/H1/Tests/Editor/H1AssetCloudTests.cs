using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Arkus.H1.Editor.Tests
{
    public sealed class H1AssetCloudTests
    {
        private const string Root = "Assets/Arkus/H1/SourceSlice";

        [Test]
        public void PrivateVaultMount_RecreatesAcceptedH104UnityIdentities()
        {
            Assert.That(AssetDatabase.IsValidFolder(Root), Is.True, "H1 SourceSlice was not mounted");

            AssertGuid("Wall_Plaster_Window_Wide_Flat.fbx", "a914dbae2609f0107a8bce353d33727c");
            AssertGuid("MI_Plaster.mat", "75fb52ef5e0f0ad40a28c36d06ecd99c");
            AssertGuid("FacadeImportedMaterial.mat", "963743a1121475e92497a201e301bf48");
            AssertGuid("UAL1.fbx", "06d37381cd6d9bece36de7794e2fc74a");

            var wallAssets = AssetDatabase.LoadAllAssetsAtPath(Root + "/Wall_Plaster_Window_Wide_Flat.fbx");
            Assert.That(wallAssets.OfType<Mesh>().Any(), Is.True, "accepted wall FBX did not import a Mesh");
            Assert.That(wallAssets.OfType<GameObject>().Any(), Is.True, "accepted wall FBX did not import a GameObject/prefab root");

            var plaster = AssetDatabase.LoadAssetAtPath<Material>(Root + "/MI_Plaster.mat");
            Assert.That(plaster, Is.Not.Null, "accepted material did not import as Material");

            var facadeImported = AssetDatabase.LoadAssetAtPath<Material>(Root + "/FacadeImportedMaterial.mat");
            Assert.That(facadeImported, Is.Not.Null, "accepted source-derived facade material did not import as Material");

            var animationAssets = AssetDatabase.LoadAllAssetsAtPath(Root + "/UAL1.fbx");
            Assert.That(animationAssets.OfType<AnimationClip>().Any(clip => !clip.name.StartsWith("__preview__")), Is.True,
                "accepted UAL1 source did not expose animation clips");
        }

        private static void AssertGuid(string fileName, string expected)
        {
            var path = Root + "/" + fileName;
            Assert.That(AssetDatabase.LoadMainAssetAtPath(path), Is.Not.Null, path + " did not import");
            Assert.That(AssetDatabase.AssetPathToGUID(path), Is.EqualTo(expected), path + " GUID differs from H1-04 authority");
        }
    }
}
