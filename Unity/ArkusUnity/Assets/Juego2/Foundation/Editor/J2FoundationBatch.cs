using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Juego2.Foundation.Editor
{
    /// <summary>Fixed batch entry points for the Juego2 foundation (bootstrap authority, not a public authoring capability).</summary>
    public static class J2FoundationBatch
    {
        public static void Apply()
        {
            J2FoundationBaseline.Apply();
            Verify();
        }

        /// <summary>Writes the effective baseline report to <c>-j2-output</c>; exits non-zero when any finding exists.</summary>
        public static void Verify()
        {
            var report = J2FoundationReport.Build();
            var output = Argument("-j2-output");
            if (output != null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(output));
                File.WriteAllText(output, JsonUtility.ToJson(report, true) + "\n", new UTF8Encoding(false));
            }
            Debug.Log($"J2_FOUNDATION_VERIFY findings={report.findings.Length} gc2={report.gc2Core} {string.Join(" ", report.findings)}");
            if (report.findings.Length > 0 && Application.isBatchMode) EditorApplication.Exit(3);
        }

        /// <summary>One-time URP migration of retained project materials (CITY-04 greybox), then a fail-closed audit report.</summary>
        public static void MigrateRetainedMaterials()
        {
            var changed = J2MaterialMigration.UpgradeBuiltinMaterials(RetainedMaterialRoots);
            var report = J2ShaderAudit.Audit(J2ShaderAudit.MaterialAssets(RetainedMaterialRoots));
            var output = Argument("-j2-output");
            if (output != null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(output));
                File.WriteAllText(output, JsonUtility.ToJson(new MigrationReport { upgraded = changed.ToArray(), audit = report }, true) + "\n", new UTF8Encoding(false));
            }
            Debug.Log($"J2_MATERIAL_MIGRATION upgraded={changed.Count} audit_green={report.green} failures={report.failures.Count}");
            if (!report.green && Application.isBatchMode) EditorApplication.Exit(3);
        }

        public static readonly string[] RetainedMaterialRoots = { "Assets/Juego2", "Assets/Arkus/CITY" };

        [Serializable]
        sealed class MigrationReport
        {
            public string[] upgraded;
            public J2ShaderAudit.Report audit;
        }

        internal static bool Flag(string name) => Array.IndexOf(Environment.GetCommandLineArgs(), name) >= 0;

        internal static string Argument(string name)
        {
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
                if (args[i] == name) return Path.GetFullPath(args[i + 1]);
            return null;
        }
    }
}
