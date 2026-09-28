using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.Rendering;

namespace Juego2.Foundation.Editor
{
    /// <summary>Effective foundation inventory + findings, the machine-readable evidence of the adopted baseline.</summary>
    [Serializable]
    public sealed class J2FoundationReport
    {
        public string schema = "juego2.h2f02.foundation-report@1";
        public string unityVersion;
        public string activePipeline;
        public string activePipelineAsset;
        public string[] qualityLevels;
        public string colorSpace;
        public string gc2Core;
        public string[] packages;
        public string[] assemblies;
        public string[] findings;

        public static J2FoundationReport Build()
        {
            var findings = J2FoundationBaseline.Verify();
            var gc2 = J2Gc2Provisioning.State(out var gc2Findings);
            findings.AddRange(gc2Findings);
            if (J2FoundationBatch.Flag("-j2-require-gc2") && gc2 != J2Gc2Provisioning.Provisioned)
                findings.Add("J2_GC2_CORE_NOT_PROVISIONED:" + gc2);
            var locked = J2PackageBaseline.ReadLock(File.ReadAllText(Path.Combine(J2PackageBaseline.ProjectRoot, "Packages/packages-lock.json")));
            return new J2FoundationReport
            {
                unityVersion = Application.unityVersion,
                activePipeline = GraphicsSettings.currentRenderPipeline == null ? "builtin" : GraphicsSettings.currentRenderPipeline.GetType().FullName,
                activePipelineAsset = GraphicsSettings.currentRenderPipeline == null ? "" : AssetDatabase.GetAssetPath(GraphicsSettings.currentRenderPipeline),
                qualityLevels = QualitySettings.names,
                colorSpace = PlayerSettings.colorSpace.ToString(),
                gc2Core = gc2,
                packages = locked.OrderBy(p => p.Key, StringComparer.Ordinal).Select(p => $"{p.Key}@{p.Value.version}/{p.Value.depth}").ToArray(),
                assemblies = CompilationPipeline.GetAssemblies(AssembliesType.Editor).Select(a => a.name)
                    .Where(n => n.StartsWith("Juego2.", StringComparison.Ordinal) || n.StartsWith("Arkus.", StringComparison.Ordinal) || n.StartsWith("GameCreator.", StringComparison.Ordinal))
                    .OrderBy(n => n, StringComparer.Ordinal).ToArray(),
                findings = findings.ToArray(),
            };
        }
    }

    /// <summary>
    /// GC2 Core is an owner-licensed, non-redistributable dependency restored outside Git by
    /// <c>scripts/h2f02-provision.py gc2</c>. This reads its effective state without referencing GC2 types, so the
    /// check also runs where Core is lawfully absent (hosted CI).
    /// </summary>
    public static class J2Gc2Provisioning
    {
        public const string Version = "2.19.61";
        public const string PackageSha256 = "1e4f3ba0560eadb944b3cf3ca63f7095d95b47c55a94ee655f28a15952f3380b";
        public const string Root = "Assets/Plugins/GameCreator";
        public const string VersionFile = Root + "/Packages/Core/Editor/Version.txt";
        public const string Receipt = Root + "/.juego2-provisioning.json";
        public const string Absent = "ABSENT";
        public const string Provisioned = "PROVISIONED_CORE_" + Version;
        public static readonly string[] AdmittedAssemblies = { "GameCreator.Editor.Core", "GameCreator.Runtime.Core", "GameCreator.Tests.Core" };

        public static string State(out List<string> findings)
        {
            findings = new List<string>();
            var project = J2PackageBaseline.ProjectRoot;
            if (!Directory.Exists(Path.Combine(project, Root))) return Absent;
            var versionPath = Path.Combine(project, VersionFile);
            var version = File.Exists(versionPath) ? File.ReadAllText(versionPath).Trim() : "<missing>";
            if (version != Version) findings.Add("J2_GC2_VERSION_MISMATCH:" + version);
            var receiptPath = Path.Combine(project, Receipt);
            if (!File.Exists(receiptPath) || !File.ReadAllText(receiptPath).Contains(PackageSha256)) findings.Add("J2_GC2_PROVISIONING_RECEIPT_MISSING");
            foreach (var asm in CompilationPipeline.GetAssemblies(AssembliesType.Editor).Select(a => a.name).Where(n => n.StartsWith("GameCreator.", StringComparison.Ordinal)))
                if (!AdmittedAssemblies.Contains(asm)) findings.Add("J2_GC2_MODULE_NOT_ADMITTED:" + asm);
            return findings.Count == 0 ? Provisioned : "INVALID";
        }
    }
}
