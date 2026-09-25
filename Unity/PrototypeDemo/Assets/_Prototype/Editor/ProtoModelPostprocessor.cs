using UnityEditor;
using UnityEngine;

namespace Proto.EditorTools
{
    /// Import settings for the copied Quaternius packs (deterministic, so a fresh copy imports the same way).
    public class ProtoModelPostprocessor : AssetPostprocessor
    {
        const string Root = "Assets/ThirdParty/Quaternius/";

        void OnPreprocessModel()
        {
            if (!assetPath.StartsWith(Root)) return;
            var importer = (ModelImporter)assetImporter;
            if (assetPath.Contains("/UAL/"))
            {
                importer.animationType = ModelImporterAnimationType.Human;
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                importer.importAnimation = true;
                importer.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
            }
            else if (assetPath.Contains("/Nature/") || assetPath.Contains("/Props/"))
            {
                importer.importAnimation = false;
                importer.animationType = ModelImporterAnimationType.None;
                importer.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
            }
            importer.isReadable = assetPath.Contains("/Nature/");
        }

        void OnPreprocessAnimation()
        {
            if (!assetPath.StartsWith(Root) || !assetPath.Contains("/UAL/")) return;
            var importer = (ModelImporter)assetImporter;
            var clips = importer.defaultClipAnimations;
            foreach (var clip in clips)
            {
                string n = clip.name.ToLowerInvariant();
                bool loop = n.Contains("idle") || n.Contains("walk") || n.Contains("jog") || n.Contains("run") || n.Contains("sprint");
                clip.loopTime = loop;
                clip.lockRootRotation = true;
                clip.lockRootHeightY = true;
                clip.lockRootPositionXZ = true;
                clip.keepOriginalOrientation = true;
                clip.keepOriginalPositionY = true;
                clip.keepOriginalPositionXZ = true;
            }
            importer.clipAnimations = clips;
        }
    }
}
