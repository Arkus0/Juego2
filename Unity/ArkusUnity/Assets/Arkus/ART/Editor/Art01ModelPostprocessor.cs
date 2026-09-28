using UnityEditor;

namespace Juego2.ART.Editor
{
    /// <summary>Normalizes only ART's ignored selected Source copies; never the purchased vault or H1 slice.</summary>
    public sealed class Art01ModelPostprocessor : AssetPostprocessor
    {
        const string Root = "Assets/Arkus/ART/External/";
        const string Character = Root + "Characters/Regular_Male_FullBody.fbx";
        const string DerivedHuman = "Assets/Arkus/ART/Derived/Characters/Townsfolk_Forastero.fbx";
        const string Ual = Root + "UAL/UAL1.fbx";

        public override uint GetVersion() => 2;

        void OnPreprocessModel()
        {
            if (!assetPath.StartsWith(Root) && assetPath != DerivedHuman) return;
            var importer = (ModelImporter)assetImporter;
            if (assetPath == Character || assetPath == DerivedHuman)
            {
                importer.animationType = ModelImporterAnimationType.Human;
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                importer.importAnimation = false;
                return;
            }
            if (assetPath == Ual)
            {
                importer.animationType = ModelImporterAnimationType.Human;
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                importer.importAnimation = true;
                return;
            }
            if (assetPath.Contains("/Medieval/Models/") || assetPath.Contains("/Nature/Models/") || assetPath.Contains("/Props/Models/"))
            {
                // Measured raw FBX vertices are centimetres. Props also contain a root x100;
                // the importer reduction and prefab root transform are both retained.
                importer.globalScale = 0.01f;
                importer.useFileScale = false;
                importer.importAnimation = false;
                importer.animationType = ModelImporterAnimationType.None;
            }
        }

        void OnPreprocessAnimation()
        {
            if (assetPath != Ual) return;
            var importer = (ModelImporter)assetImporter;
            var clips = importer.defaultClipAnimations;
            foreach (var clip in clips)
            {
                var lower = clip.name.ToLowerInvariant();
                clip.loopTime = lower.Contains("idle") || lower.Contains("walk") || lower.Contains("jog");
                clip.lockRootRotation = true;
                clip.lockRootHeightY = true;
                clip.lockRootPositionXZ = true;
                // #233 exposed a 180-degree body/root discrepancy on moving humanoids.
                clip.keepOriginalOrientation = false;
                clip.keepOriginalPositionY = true;
                clip.keepOriginalPositionXZ = true;
            }
            importer.clipAnimations = clips;
        }
    }
}
