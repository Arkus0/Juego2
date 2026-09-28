using System.IO;
using System.Linq;
using System.Text;
using Juego2.Foundation.Editor;
using UnityEditor;
using UnityEngine;

namespace Juego2.Gc2Adapter.Editor
{
    /// <summary>Batch entry: the 01A GC2 lints over every scene/prefab under the Juego2/Arkus content roots.</summary>
    public static class J2Gc2Batch
    {
        public static readonly string[] ContentRoots = { "Assets/Juego2", "Assets/Arkus" };

        public static void LintProject()
        {
            var roots = ContentRoots.Concat(J2FoundationBatch.ExtraRoots()).Where(AssetDatabase.IsValidFolder).ToArray();
            var findings = J2Gc2Lint.CheckProjectContent(roots).Select(f => f.ToString()).ToArray();
            var output = J2FoundationBatch.OutputPath();
            if (output != null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(output));
                File.WriteAllText(output, "{\n  \"roots\": [" + string.Join(", ", roots.Select(r => "\"" + r + "\"")) + "],\n  \"findings\": [" +
                    string.Join(", ", findings.Select(f => "\"" + f.Replace("\\", "/").Replace("\"", "'") + "\"")) + "]\n}\n", new UTF8Encoding(false));
            }
            Debug.Log($"J2_GC2_LINT roots={string.Join(",", roots)} findings={findings.Length} {string.Join(" ", findings)}");
            if (findings.Length > 0 && Application.isBatchMode) EditorApplication.Exit(3);
        }
    }
}
