using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Rendering;

namespace Arkus.H1.Editor
{
    public static class H1Bootstrap
    {
        internal const string ExpectedUnityVersion = "6000.3.24f1";
        internal const string ProjectIdentity = "arkus-h1-unity";
        internal const string RenderPipelineBaseline = "builtin";

        public static void ConfigureBaseline()
        {
            RequirePinnedEditor();

            EditorSettings.serializationMode = SerializationMode.ForceText;
            EditorSettings.externalVersionControl = "Visible Meta Files";

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            RequireEffectiveBaseline();
            H1Inventory.WriteFromCommandLine();
        }

        internal static void RequirePinnedEditor()
        {
            if (!string.Equals(Application.unityVersion, ExpectedUnityVersion, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"unity-version-mismatch: expected {ExpectedUnityVersion}, effective {Application.unityVersion}");
            }
        }

        internal static void RequireEffectiveBaseline()
        {
            if (EditorSettings.serializationMode != SerializationMode.ForceText)
            {
                throw new InvalidOperationException(
                    $"serialization-mode-mismatch: expected ForceText, effective {EditorSettings.serializationMode}");
            }

            if (!string.Equals(
                    EditorSettings.externalVersionControl,
                    "Visible Meta Files",
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "meta-policy-mismatch: expected Visible Meta Files, effective " +
                    EditorSettings.externalVersionControl);
            }

            if (GraphicsSettings.currentRenderPipeline != null)
            {
                throw new InvalidOperationException(
                    "render-pipeline-mismatch: expected built-in render pipeline");
            }

            var lockPath = Path.Combine(ProjectRoot(), "Packages", "packages-lock.json");
            if (!File.Exists(lockPath))
            {
                throw new InvalidOperationException("packages-lock-missing: " + lockPath);
            }
        }

        internal static string ProjectRoot()
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        }
    }

    public static class H1Batch
    {
        // This is the fixed H1-02 batch entry identity. H1-03/H1-03A may bind it
        // into a launch profile; it is not a public capability or caller-chosen route.
        public static void Run()
        {
            H1Bootstrap.RequirePinnedEditor();
            H1Bootstrap.RequireEffectiveBaseline();
            H1Inventory.WriteFromCommandLine();
        }
    }

    internal static class H1Inventory
    {
        private const string OutputArgument = "-arkus-h1-output";

        public static void WriteFromCommandLine()
        {
            var outputPath = ReadRequiredArgument(OutputArgument);
            var document = Build();
            var parent = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(parent))
            {
                Directory.CreateDirectory(parent);
            }

            File.WriteAllText(
                outputPath,
                JsonUtility.ToJson(document, true) + Environment.NewLine,
                new UTF8Encoding(false));

            Debug.Log("ARKUS_H1_INVENTORY_WRITTEN " + outputPath);
        }

        private static InventoryDocument Build()
        {
            var projectRoot = H1Bootstrap.ProjectRoot();
            var manifestPath = Path.Combine(projectRoot, "Packages", "manifest.json");
            var lockPath = Path.Combine(projectRoot, "Packages", "packages-lock.json");

            var packages = ReadPackages();
            var assemblies = new List<AssemblyRow>();
            assemblies.AddRange(ReadAssemblies(AssembliesType.Player, "player", projectRoot));
            assemblies.AddRange(ReadAssemblies(AssembliesType.Editor, "editor", projectRoot));

            return new InventoryDocument
            {
                schema = "arkus.h1-02-effective-inventory@1",
                projectIdentity = H1Bootstrap.ProjectIdentity,
                unityVersion = Application.unityVersion,
                serializationMode = EditorSettings.serializationMode.ToString(),
                externalVersionControl = EditorSettings.externalVersionControl,
                renderPipeline = GraphicsSettings.currentRenderPipeline == null
                    ? H1Bootstrap.RenderPipelineBaseline
                    : GraphicsSettings.currentRenderPipeline.GetType().FullName,
                manifestSha256 = Sha256(manifestPath),
                packagesLockSha256 = Sha256(lockPath),
                packages = packages,
                assemblies = assemblies
                    .OrderBy(x => x.kind, StringComparer.Ordinal)
                    .ThenBy(x => x.name, StringComparer.Ordinal)
                    .ToArray()
            };
        }

        private static PackageRow[] ReadPackages()
        {
            ListRequest request = Client.List(true, true);
            var deadline = DateTime.UtcNow.AddSeconds(60);
            while (!request.IsCompleted)
            {
                if (DateTime.UtcNow >= deadline)
                {
                    throw new TimeoutException("package-list-timeout");
                }

                Thread.Sleep(25);
            }

            if (request.Status != StatusCode.Success)
            {
                var message = request.Error == null ? "unknown" : request.Error.message;
                throw new InvalidOperationException("package-list-failed: " + message);
            }

            return request.Result
                .Select(x => new PackageRow
                {
                    name = x.name,
                    version = x.version,
                    source = x.source.ToString()
                })
                .OrderBy(x => x.name, StringComparer.Ordinal)
                .ToArray();
        }

        private static IEnumerable<AssemblyRow> ReadAssemblies(
            AssembliesType type,
            string kind,
            string projectRoot)
        {
            return CompilationPipeline.GetAssemblies(type)
                .Select(x => new AssemblyRow
                {
                    kind = kind,
                    name = x.name,
                    sourceFiles = x.sourceFiles
                        .Select(path => ToProjectRelative(path, projectRoot))
                        .OrderBy(path => path, StringComparer.Ordinal)
                        .ToArray()
                });
        }

        private static string ToProjectRelative(string path, string projectRoot)
        {
            var fullPath = Path.GetFullPath(path);
            var normalizedRoot = projectRoot.TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;

            if (fullPath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
            {
                return fullPath
                    .Substring(normalizedRoot.Length)
                    .Replace(Path.DirectorySeparatorChar, '/')
                    .Replace(Path.AltDirectorySeparatorChar, '/');
            }

            return Path.GetFileName(fullPath);
        }

        private static string Sha256(string path)
        {
            using (var stream = File.OpenRead(path))
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(stream);
                var builder = new StringBuilder(bytes.Length * 2);
                foreach (var value in bytes)
                {
                    builder.Append(value.ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private static string ReadRequiredArgument(string name)
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length - 1; i++)
            {
                if (string.Equals(args[i], name, StringComparison.Ordinal))
                {
                    return Path.GetFullPath(args[i + 1]);
                }
            }

            throw new InvalidOperationException("missing-required-argument: " + name);
        }
    }

    [Serializable]
    internal sealed class InventoryDocument
    {
        public string schema;
        public string projectIdentity;
        public string unityVersion;
        public string serializationMode;
        public string externalVersionControl;
        public string renderPipeline;
        public string manifestSha256;
        public string packagesLockSha256;
        public PackageRow[] packages;
        public AssemblyRow[] assemblies;
    }

    [Serializable]
    internal sealed class PackageRow
    {
        public string name;
        public string version;
        public string source;
    }

    [Serializable]
    internal sealed class AssemblyRow
    {
        public string kind;
        public string name;
        public string[] sourceFiles;
    }
}
