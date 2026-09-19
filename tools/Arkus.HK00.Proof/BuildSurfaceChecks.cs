using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Arkus.HK00.Proof
{
    internal static class BuildSurfaceChecks
    {
        private static readonly HashSet<string> AllowedPolicyFiles = new HashSet<string>(StringComparer.Ordinal)
        {
            "Directory.Build.props",
            "Directory.Packages.props",
        };

        private static readonly HashSet<string> DirectoryBuildProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "LangVersion",
            "Nullable",
            "ImplicitUsings",
            "TreatWarningsAsErrors",
            "EnableNETAnalyzers",
            "AnalysisLevel",
            "DisableTransitiveProjectReferences",
            "Deterministic",
            "DebugType",
            "DebugSymbols",
            "EmbedAllSources",
            "GenerateAssemblyInfo",
            "GenerateTargetFrameworkAttribute",
            "GenerateDocumentationFile",
        };

        private static readonly HashSet<string> DirectoryPackageProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "ManagePackageVersionsCentrally",
            "CentralPackageTransitivePinningEnabled",
        };

        private static readonly HashSet<string> ProjectProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "OutputType",
            "TargetFramework",
            "AssemblyName",
            "RootNamespace",
            "IsPackable",
            "IsTestProject",
        };

        private static readonly HashSet<string> AllowedPackageMetadata = new HashSet<string>(StringComparer.Ordinal)
        {
            "PrivateAssets",
            "IncludeAssets",
        };

        public static int RunRepository(string root)
        {
            var findings = 0;
            var tracked = ReadTrackedFiles(root);

            foreach (var path in tracked)
            {
                if (path.EndsWith(".rsp", StringComparison.OrdinalIgnoreCase))
                {
                    findings += Report(
                        "HK00-BUILD-RESPONSE-FILE",
                        "repository",
                        path,
                        "Repository-owned response files are forbidden; canonical MSBuild invocations use -noAutoResponse.");
                }

                if (IsSolutionEntrypoint(path)
                    && !string.Equals(path, FixedContract.CanonicalSolution, StringComparison.Ordinal))
                {
                    findings += Report(
                        "HK00-ALT-SOLUTION-ENTRYPOINT",
                        "repository",
                        path,
                        "HK00 permits exactly one repository-owned solution entrypoint.");
                }

                var full = Path.Combine(root, path);
                if (!File.Exists(full))
                {
                    continue;
                }

                XDocument? document = TryLoadXml(full);
                if (document?.Root is null
                    || !string.Equals(document.Root.Name.LocalName, "Project", StringComparison.Ordinal))
                {
                    continue;
                }

                var fixedProject = FixedContract.ByPath(path);
                if (fixedProject is null && !AllowedPolicyFiles.Contains(path))
                {
                    findings += Report(
                        "HK00-MSBUILD-FILE-UNCLASSIFIED",
                        "repository",
                        path,
                        "Tracked MSBuild Project XML exists outside the fixed project/build-policy surface.");
                    continue;
                }

                findings += fixedProject is not null
                    ? CheckProjectDocument(path, document)
                    : CheckPolicyDocument(path, document);
            }

            return findings;
        }

        private static int CheckProjectDocument(string path, XDocument document)
        {
            var findings = 0;
            var root = document.Root!;
            findings += RequireOnlyAttributes(path, root, new[] { "Sdk" });
            var sdk = root.Attribute("Sdk")?.Value ?? string.Empty;
            if (!string.Equals(sdk, "Microsoft.NET.Sdk", StringComparison.Ordinal))
            {
                findings += Report(
                    "HK00-PROJECT-SDK",
                    "repository",
                    path,
                    "Canonical projects must use exactly Microsoft.NET.Sdk.");
            }

            foreach (var element in root.Elements())
            {
                if (string.Equals(element.Name.LocalName, "PropertyGroup", StringComparison.Ordinal))
                {
                    findings += CheckPropertyGroup(path, element, ProjectProperties);
                }
                else if (string.Equals(element.Name.LocalName, "ItemGroup", StringComparison.Ordinal))
                {
                    findings += CheckProjectItemGroup(path, element);
                }
                else
                {
                    findings += ReportShape(path, element, "Only PropertyGroup and ItemGroup are permitted in canonical project files.");
                }
            }

            return findings;
        }

        private static int CheckPolicyDocument(string path, XDocument document)
        {
            var findings = RequireOnlyAttributes(path, document.Root!, Array.Empty<string>());
            if (string.Equals(path, "Directory.Build.props", StringComparison.Ordinal))
            {
                foreach (var element in document.Root!.Elements())
                {
                    if (!string.Equals(element.Name.LocalName, "PropertyGroup", StringComparison.Ordinal))
                    {
                        findings += ReportShape(path, element, "Directory.Build.props is a closed property-only surface.");
                        continue;
                    }
                    findings += CheckPropertyGroup(path, element, DirectoryBuildProperties);
                }
                return findings;
            }

            foreach (var element in document.Root!.Elements())
            {
                if (string.Equals(element.Name.LocalName, "PropertyGroup", StringComparison.Ordinal))
                {
                    findings += CheckPropertyGroup(path, element, DirectoryPackageProperties);
                }
                else if (string.Equals(element.Name.LocalName, "ItemGroup", StringComparison.Ordinal))
                {
                    findings += CheckPackageVersionGroup(path, element);
                }
                else
                {
                    findings += ReportShape(path, element, "Directory.Packages.props contains only fixed package-policy groups.");
                }
            }

            return findings;
        }

        private static int CheckPropertyGroup(string path, XElement group, HashSet<string> allowed)
        {
            var findings = RequireOnlyAttributes(path, group, Array.Empty<string>());
            foreach (var property in group.Elements())
            {
                if (!allowed.Contains(property.Name.LocalName))
                {
                    findings += ReportShape(path, property, "Property is outside the fixed HK00 build surface.");
                }
                findings += RequireOnlyAttributes(path, property, Array.Empty<string>());
                if (property.Elements().Any())
                {
                    findings += ReportShape(path, property, "Build properties may not contain nested XML.");
                }
            }
            return findings;
        }

        private static int CheckProjectItemGroup(string path, XElement group)
        {
            var findings = RequireOnlyAttributes(path, group, Array.Empty<string>());
            foreach (var item in group.Elements())
            {
                if (string.Equals(item.Name.LocalName, "ProjectReference", StringComparison.Ordinal))
                {
                    findings += RequireOnlyAttributes(path, item, new[] { "Include" });
                    if (item.Elements().Any())
                    {
                        findings += ReportShape(path, item, "ProjectReference metadata is not part of the HK00 surface.");
                    }
                }
                else if (string.Equals(item.Name.LocalName, "PackageReference", StringComparison.Ordinal))
                {
                    findings += RequireOnlyAttributes(path, item, new[] { "Include" });
                    foreach (var metadata in item.Elements())
                    {
                        if (!AllowedPackageMetadata.Contains(metadata.Name.LocalName))
                        {
                            findings += ReportShape(path, metadata, "PackageReference metadata is outside the fixed HK00 surface.");
                        }
                        findings += RequireOnlyAttributes(path, metadata, Array.Empty<string>());
                        if (metadata.Elements().Any())
                        {
                            findings += ReportShape(path, metadata, "PackageReference metadata may not contain nested XML.");
                        }
                    }
                }
                else
                {
                    findings += ReportShape(
                        path,
                        item,
                        "Only ProjectReference and PackageReference items are permitted; compiler/build input items must not create a second channel.");
                }
            }
            return findings;
        }

        private static int CheckPackageVersionGroup(string path, XElement group)
        {
            var findings = RequireOnlyAttributes(path, group, Array.Empty<string>());
            foreach (var item in group.Elements())
            {
                if (!string.Equals(item.Name.LocalName, "PackageVersion", StringComparison.Ordinal))
                {
                    findings += ReportShape(path, item, "Only PackageVersion items are permitted in Directory.Packages.props.");
                    continue;
                }

                findings += RequireOnlyAttributes(path, item, new[] { "Include", "Version" });
                if (item.Elements().Any())
                {
                    findings += ReportShape(path, item, "PackageVersion may not contain nested XML.");
                }

                var id = item.Attribute("Include")?.Value ?? string.Empty;
                var version = item.Attribute("Version")?.Value ?? string.Empty;
                if (!FixedContract.PackageVersions.TryGetValue(id, out var expected)
                    || !string.Equals(expected, version, StringComparison.Ordinal))
                {
                    findings += Report(
                        "HK00-PACKAGE-POLICY",
                        "repository",
                        path + ":" + id,
                        "Package version entry is not part of the fixed package policy.");
                }
            }
            return findings;
        }

        private static int RequireOnlyAttributes(string path, XElement element, IReadOnlyCollection<string> allowedNames)
        {
            var findings = 0;
            foreach (var attribute in element.Attributes())
            {
                if (!allowedNames.Contains(attribute.Name.LocalName))
                {
                    findings += ReportShape(path, element, "Attribute '" + attribute.Name.LocalName + "' is outside the closed HK00 build surface.");
                }
            }
            return findings;
        }

        private static XDocument? TryLoadXml(string path)
        {
            try
            {
                var text = File.ReadAllText(path);
                var trimmed = text.TrimStart('\uFEFF', ' ', '\t', '\r', '\n');
                if (!trimmed.StartsWith("<", StringComparison.Ordinal))
                {
                    return null;
                }
                return XDocument.Parse(text, LoadOptions.PreserveWhitespace);
            }
            catch
            {
                return null;
            }
        }

        private static SortedSet<string> ReadTrackedFiles(string root)
        {
            var tracked = new SortedSet<string>(StringComparer.Ordinal);
            var result = ProcessExec.Run("git", new[] { "ls-files", "-z" }, root);
            foreach (var raw in ProcessExec.SplitNull(result.Stdout))
            {
                tracked.Add(raw.Replace('\\', '/'));
            }
            return tracked;
        }

        private static bool IsSolutionEntrypoint(string path)
        {
            return path.EndsWith(".sln", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".slnx", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".slnf", StringComparison.OrdinalIgnoreCase);
        }

        private static int ReportShape(string path, XElement element, string message)
        {
            return Report(
                "HK00-BUILD-XML-SHAPE",
                "repository",
                path + ":" + element.Name.LocalName,
                message);
        }

        private static int Report(string id, string phase, string subject, string message)
        {
            Console.Error.WriteLine(id + " [" + phase + "] " + subject + ": " + message);
            return 1;
        }
    }
}
