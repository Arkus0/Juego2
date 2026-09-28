using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Juego2.Foundation.Editor
{
    /// <summary>
    /// Built-in -> URP material migration through the public MaterialUpgrader API. URP 17.3.0's batch converter
    /// (Converters.RunInBatchMode) throws on 6000.3.24f1 (H2F-01 S01), so it is not used. Only materials whose shader is
    /// built-in-only are touched; colours/textures carry over through the URP upgraders.
    /// </summary>
    public static class J2MaterialMigration
    {
        public static List<string> UpgradeBuiltinMaterials(params string[] roots)
        {
            var upgraders = MaterialUpgrader.FetchAllUpgradersForPipeline(typeof(UniversalRenderPipelineAsset));
            var changed = new List<string>();
            // Models imported before the pipeline switch keep built-in embedded materials until reimported (H2F-01).
            foreach (var path in AssetDatabase.FindAssets("t:Model", roots).Select(AssetDatabase.GUIDToAssetPath).Distinct())
                if (AssetDatabase.LoadAllAssetsAtPath(path).OfType<Material>().Any(m => J2ShaderAudit.Classify(m.shader) != "URP_OK"))
                {
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                    changed.Add(path + " REIMPORTED_UNDER_URP");
                }
            foreach (var path in AssetDatabase.FindAssets("t:Material", roots).Select(AssetDatabase.GUIDToAssetPath).Distinct())
            {
                if (!path.EndsWith(".mat", System.StringComparison.OrdinalIgnoreCase)) continue; // model sub-assets are importer output
                var material = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (material == null || J2ShaderAudit.Classify(material.shader) != "BUILTIN_ONLY") continue;
                string message = null;
                if (MaterialUpgrader.Upgrade(material, upgraders, MaterialUpgrader.UpgradeFlags.None, ref message))
                {
                    EditorUtility.SetDirty(material);
                    changed.Add(path + " -> " + material.shader.name);
                }
                else changed.Add(path + " NOT_UPGRADED " + message);
            }
            AssetDatabase.SaveAssets();
            return changed;
        }

        /// <summary>URP/Lit material for code that creates materials at runtime of an editor tool (e.g. greyboxes).</summary>
        public static Shader LitShader() => Shader.Find("Universal Render Pipeline/Lit");
    }
}
