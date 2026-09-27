using UnityEditor;

namespace Juego2.H2F01.Editor
{
    /// <summary>Import rules for the spike's extra pinned inputs only (never ART's or the vault's bytes).</summary>
    public sealed class H2F01ModelPostprocessor : AssetPostprocessor
    {
        const string Models = "Assets/H2F01Inputs/QuaterniusURP/Models/";
        const string Anim = "Assets/H2F01Inputs/Animation/";

        public override uint GetVersion() => 4;

        void OnPreprocessModel()
        {
            var importer = (ModelImporter)assetImporter;
            if (assetPath.StartsWith(Models))
            {
                // Same measured centimetre convention ART-01 applies to Medieval Source models.
                importer.globalScale = 0.01f;
                importer.useFileScale = false;
                importer.importAnimation = false;
                importer.animationType = ModelImporterAnimationType.None;
            }
            else if (assetPath.StartsWith(Anim))
            {
                importer.animationType = ModelImporterAnimationType.Human;
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                importer.importAnimation = true;
            }
        }

        // UAL "_Loop" clips must loop (ART-01 applies the same rule to its UAL1 copy); root motion is kept baked.
        void OnPreprocessAnimation()
        {
            if (!assetPath.StartsWith(Anim)) return;
            var importer = (ModelImporter)assetImporter;
            var clips = importer.defaultClipAnimations;
            foreach (var clip in clips)
                clip.loopTime = clip.name.EndsWith("_Loop");
            importer.clipAnimations = clips;
        }
    }
}
