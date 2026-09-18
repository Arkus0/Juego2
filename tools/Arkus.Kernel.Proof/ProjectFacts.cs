using System;
using System.Collections.Generic;

namespace Arkus.Kernel.Proof
{
    /// <summary>
    /// The evaluated MSBuild facts of one project.
    /// </summary>
    /// <remarks>
    /// These are evaluated values, not project-file text: a property inherited
    /// from <c>Directory.Build.props</c>, overridden locally, or injected by the
    /// SDK is seen here exactly as the build sees it.
    /// </remarks>
    public sealed class ProjectFacts
    {
        /// <summary>Creates the fact set.</summary>
        /// <param name="name">Project name.</param>
        /// <param name="projectPath">Absolute project file path.</param>
        /// <param name="properties">Evaluated properties.</param>
        /// <param name="items">Evaluated item full paths or identities, by item type.</param>
        /// <param name="packageReferences">Evaluated package references with their version metadata.</param>
        public ProjectFacts(
            string name,
            string projectPath,
            IReadOnlyDictionary<string, string> properties,
            IReadOnlyDictionary<string, IReadOnlyList<string>> items,
            IReadOnlyList<PackageReferenceFact> packageReferences)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ProjectPath = projectPath ?? throw new ArgumentNullException(nameof(projectPath));
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
            Items = items ?? throw new ArgumentNullException(nameof(items));
            PackageReferences = packageReferences ?? throw new ArgumentNullException(nameof(packageReferences));
        }

        /// <summary>Project name.</summary>
        public string Name { get; }

        /// <summary>Absolute project file path.</summary>
        public string ProjectPath { get; }

        /// <summary>Evaluated properties.</summary>
        public IReadOnlyDictionary<string, string> Properties { get; }

        /// <summary>Evaluated items by item type.</summary>
        public IReadOnlyDictionary<string, IReadOnlyList<string>> Items { get; }

        /// <summary>Evaluated package references.</summary>
        public IReadOnlyList<PackageReferenceFact> PackageReferences { get; }

        /// <summary>Reads an evaluated property.</summary>
        /// <param name="name">Property name.</param>
        /// <returns>The evaluated value, or an empty string.</returns>
        public string Property(string name)
        {
            return Properties.TryGetValue(name, out var value) ? value : string.Empty;
        }

        /// <summary>Reads an evaluated item list.</summary>
        /// <param name="itemType">Item type.</param>
        /// <returns>The item values, or an empty list.</returns>
        public IReadOnlyList<string> Item(string itemType)
        {
            return Items.TryGetValue(itemType, out var values) ? values : Array.Empty<string>();
        }
    }

    /// <summary>One evaluated package reference.</summary>
    public sealed class PackageReferenceFact
    {
        /// <summary>Creates the fact.</summary>
        /// <param name="id">Package identity.</param>
        /// <param name="version">Version metadata declared on the reference itself.</param>
        public PackageReferenceFact(string id, string version)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Version = version ?? string.Empty;
        }

        /// <summary>Package identity.</summary>
        public string Id { get; }

        /// <summary>Version metadata declared on the reference itself.</summary>
        public string Version { get; }
    }
}
