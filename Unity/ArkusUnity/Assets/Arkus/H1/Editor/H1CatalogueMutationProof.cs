using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Arkus.H1.Editor
{
    // Runs only in a disposable project copy. The approved external Source root is never written.
    public static class H1CatalogueMutationProof
    {
        private const string Original = H1CatalogueInventory.SourceRoot + "/Wall_Plaster_Window_Wide_Flat.fbx";
        private const string Moved = H1CatalogueInventory.SourceRoot + "/Wall_Plaster_Window_Moved.fbx";
        private const string Copy = H1CatalogueInventory.SourceRoot + "/Wall_Plaster_Window_Copy.fbx";

        public static void Run()
        {
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, "-arkus-h1-output");
            if (index < 0 || index + 1 >= args.Length) throw new InvalidDataException("catalogue.proof-output-missing");
            var output = Path.GetFullPath(args[index + 1]);
            var first = H1CatalogueInventory.Capture();
            var originalPrefab = Prefab(first, Original);
            var projectRoot = H1Bootstrap.ProjectRoot();
            var originalBytes = File.ReadAllBytes(Path.Combine(projectRoot, Original));
            var originalMeta = File.ReadAllBytes(Path.Combine(projectRoot, Original + ".meta"));

            Require(string.IsNullOrEmpty(AssetDatabase.MoveAsset(Original, Moved)), "catalogue.move-failed");
            var afterMove = H1CatalogueInventory.Capture();
            var movedPrefab = Prefab(afterMove, Moved);
            Require(movedPrefab.nativeGuid == originalPrefab.nativeGuid && movedPrefab.localFileId == originalPrefab.localFileId,
                "catalogue.move-changed-native-identity");
            Require(afterMove.rows.Count(row => row.path == Original) == 0, "catalogue.old-path-remained");
            Require(string.IsNullOrEmpty(AssetDatabase.MoveAsset(Moved, Original)), "catalogue.restore-move-failed");

            Require(AssetDatabase.CopyAsset(Original, Copy), "catalogue.copy-failed");
            var afterCopy = H1CatalogueInventory.Capture();
            var copyPrefab = Prefab(afterCopy, Copy);
            Require(copyPrefab.nativeGuid != originalPrefab.nativeGuid && copyPrefab.localFileId == originalPrefab.localFileId,
                "catalogue.copy-aliased-source");
            Require(AssetDatabase.DeleteAsset(Copy), "catalogue.delete-copy-failed");

            Require(AssetDatabase.DeleteAsset(Original), "catalogue.delete-source-failed");
            var afterDelete = H1CatalogueInventory.Capture();
            Require(afterDelete.rows.Count(row => row.nativeGuid == originalPrefab.nativeGuid) == 0,
                "catalogue.deleted-source-remained-visible");
            File.WriteAllBytes(Path.Combine(projectRoot, Original), originalBytes);
            File.WriteAllBytes(Path.Combine(projectRoot, Original + ".meta"), originalMeta);
            AssetDatabase.ImportAsset(Original, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            var afterReimport = H1CatalogueInventory.Capture();
            var restoredPrefab = Prefab(afterReimport, Original);
            Require(restoredPrefab.nativeGuid == originalPrefab.nativeGuid && restoredPrefab.localFileId == originalPrefab.localFileId,
                "catalogue.reimport-changed-native-identity");
            Require(Fingerprint(first) == Fingerprint(afterReimport), "catalogue.reimport-changed-effective-inventory");

            var summary = new MutationSummary
            {
                schemaId = "arkus.h1-04-unity-mutation-proof@1",
                editorVersion = Application.unityVersion,
                projectIdentity = first.projectIdentity,
                baselineCount = first.rows.Length,
                movedCount = afterMove.rows.Length,
                copiedCount = afterCopy.rows.Length,
                deletedCount = afterDelete.rows.Length,
                restoredCount = afterReimport.rows.Length,
                originalGuid = originalPrefab.nativeGuid,
                movedGuid = movedPrefab.nativeGuid,
                copiedGuid = copyPrefab.nativeGuid,
                restoredGuid = restoredPrefab.nativeGuid,
                baselineInventorySha256 = Fingerprint(first),
                restoredInventorySha256 = Fingerprint(afterReimport),
                result = "GREEN"
            };
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            File.WriteAllText(output, JsonUtility.ToJson(summary, true), new UTF8Encoding(false));
        }

        private static EffectiveCatalogueRow Prefab(EffectiveInventory inventory, string path)
        {
            var matches = inventory.rows.Where(row => row.kind == "prefab" && row.path == path).ToArray();
            if (matches.Length != 1) throw new InvalidDataException("catalogue.prefab-identity-ambiguous:" + path);
            return matches[0];
        }

        private static string Fingerprint(EffectiveInventory inventory)
        {
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(JsonUtility.ToJson(inventory)))).Replace("-", "").ToLowerInvariant();
        }

        private static void Require(bool value, string code)
        {
            if (!value) throw new InvalidDataException(code);
        }

        [Serializable]
        private sealed class MutationSummary
        {
            public string schemaId;
            public string editorVersion;
            public string projectIdentity;
            public int baselineCount;
            public int movedCount;
            public int copiedCount;
            public int deletedCount;
            public int restoredCount;
            public string originalGuid;
            public string movedGuid;
            public string copiedGuid;
            public string restoredGuid;
            public string baselineInventorySha256;
            public string restoredInventorySha256;
            public string result;
        }
    }
}
