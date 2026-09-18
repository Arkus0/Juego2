using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;

namespace Arkus.Kernel.Proof
{
    /// <summary>One source document the compiler actually consumed.</summary>
    public sealed class CompiledDocument
    {
        /// <summary>Creates the document fact.</summary>
        /// <param name="path">Absolute path as recorded by the compiler.</param>
        /// <param name="hash">Recorded content hash.</param>
        /// <param name="hashAlgorithm">Hash algorithm GUID recorded by the compiler.</param>
        public CompiledDocument(string path, ImmutableArray<byte> hash, Guid hashAlgorithm)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Hash = hash;
            HashAlgorithm = hashAlgorithm;
        }

        /// <summary>Absolute path as recorded by the compiler.</summary>
        public string Path { get; }

        /// <summary>Recorded content hash.</summary>
        public ImmutableArray<byte> Hash { get; }

        /// <summary>Hash algorithm GUID recorded by the compiler.</summary>
        public Guid HashAlgorithm { get; }
    }

    /// <summary>
    /// What a built assembly actually contains.
    /// </summary>
    /// <remarks>
    /// This is the effective-behaviour oracle for WP-HK-00: assembly references
    /// and compiled documents are read out of the produced binary and its portable
    /// PDB, so a claim about dependencies or compiled source cannot be satisfied by
    /// project-file text alone.
    /// </remarks>
    public sealed class AssemblyFacts
    {
        /// <summary>GUID Roslyn records for SHA-256 source checksums.</summary>
        public static readonly Guid Sha256Algorithm = new Guid("8829d00f-11b8-4213-878b-770e8597ac16");

        private AssemblyFacts(
            string assemblyName,
            string targetFramework,
            IReadOnlyList<string> assemblyReferences,
            IReadOnlyList<CompiledDocument> compiledDocuments)
        {
            AssemblyName = assemblyName;
            TargetFramework = targetFramework;
            AssemblyReferences = assemblyReferences;
            CompiledDocuments = compiledDocuments;
        }

        /// <summary>Assembly simple name.</summary>
        public string AssemblyName { get; }

        /// <summary>Value of the emitted <c>TargetFrameworkAttribute</c>.</summary>
        public string TargetFramework { get; }

        /// <summary>Simple names of every assembly reference in the metadata table.</summary>
        public IReadOnlyList<string> AssemblyReferences { get; }

        /// <summary>Every source document recorded in the portable PDB.</summary>
        public IReadOnlyList<CompiledDocument> CompiledDocuments { get; }

        /// <summary>Reads an assembly and its portable PDB.</summary>
        /// <param name="assemblyPath">Absolute path of the built assembly.</param>
        /// <returns>The effective facts.</returns>
        public static AssemblyFacts Read(string assemblyPath)
        {
            if (assemblyPath is null)
            {
                throw new ArgumentNullException(nameof(assemblyPath));
            }

            if (!File.Exists(assemblyPath))
            {
                throw new ProofToolException($"Built assembly not found: {assemblyPath}");
            }

            var pdbPath = Path.ChangeExtension(assemblyPath, ".pdb");
            if (!File.Exists(pdbPath))
            {
                throw new ProofToolException(
                    $"Portable PDB not found next to '{assemblyPath}'. The effective-compilation oracle cannot run.");
            }

            string assemblyName;
            var targetFramework = string.Empty;
            var references = new List<string>();

            try
            {
                using var assemblyStream = File.OpenRead(assemblyPath);
                using var peReader = new PEReader(assemblyStream);
                var reader = peReader.GetMetadataReader();

                assemblyName = reader.GetString(reader.GetAssemblyDefinition().Name);

                foreach (var handle in reader.AssemblyReferences)
                {
                    references.Add(reader.GetString(reader.GetAssemblyReference(handle).Name));
                }

                targetFramework = ReadTargetFramework(reader);
            }
            catch (BadImageFormatException ex)
            {
                throw new ProofToolException($"Built assembly is not readable metadata: {assemblyPath}", ex);
            }

            references.Sort(StringComparer.Ordinal);

            var documents = new List<CompiledDocument>();
            try
            {
                using var pdbStream = File.OpenRead(pdbPath);
                using var provider = MetadataReaderProvider.FromPortablePdbStream(pdbStream);
                var pdbReader = provider.GetMetadataReader();

                foreach (var handle in pdbReader.Documents)
                {
                    var document = pdbReader.GetDocument(handle);
                    documents.Add(
                        new CompiledDocument(
                            pdbReader.GetString(document.Name),
                            pdbReader.GetBlobContent(document.Hash),
                            pdbReader.GetGuid(document.HashAlgorithm)));
                }
            }
            catch (BadImageFormatException ex)
            {
                throw new ProofToolException($"Portable PDB is not readable: {pdbPath}", ex);
            }

            documents.Sort((a, b) => string.CompareOrdinal(a.Path, b.Path));

            return new AssemblyFacts(assemblyName, targetFramework, references, documents);
        }

        /// <summary>Computes the SHA-256 checksum of a file exactly as the compiler records it.</summary>
        /// <param name="path">Absolute file path.</param>
        /// <returns>The checksum bytes.</returns>
        public static byte[] Sha256OfFile(string path)
        {
            using var stream = File.OpenRead(path);
            using var sha = SHA256.Create();
            return sha.ComputeHash(stream);
        }

        /// <summary>Compares a recorded hash with freshly computed bytes.</summary>
        /// <param name="recorded">Hash recorded in the PDB.</param>
        /// <param name="computed">Freshly computed hash.</param>
        /// <returns><c>true</c> when identical.</returns>
        public static bool HashEquals(ImmutableArray<byte> recorded, byte[] computed)
        {
            if (computed is null)
            {
                throw new ArgumentNullException(nameof(computed));
            }

            if (recorded.IsDefaultOrEmpty || recorded.Length != computed.Length)
            {
                return false;
            }

            for (var i = 0; i < computed.Length; i++)
            {
                if (recorded[i] != computed[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static string ReadTargetFramework(MetadataReader reader)
        {
            foreach (var handle in reader.CustomAttributes)
            {
                var attribute = reader.GetCustomAttribute(handle);
                if (attribute.Constructor.Kind != HandleKind.MemberReference)
                {
                    continue;
                }

                var constructor = reader.GetMemberReference((MemberReferenceHandle)attribute.Constructor);
                if (constructor.Parent.Kind != HandleKind.TypeReference)
                {
                    continue;
                }

                var typeReference = reader.GetTypeReference((TypeReferenceHandle)constructor.Parent);
                if (!string.Equals(
                        reader.GetString(typeReference.Name),
                        "TargetFrameworkAttribute",
                        StringComparison.Ordinal))
                {
                    continue;
                }

                var value = attribute.DecodeValue(StringAttributeTypeProvider.Instance);
                if (value.FixedArguments.Length > 0 && value.FixedArguments[0].Value is string moniker)
                {
                    return moniker;
                }
            }

            return string.Empty;
        }

        private sealed class StringAttributeTypeProvider : ICustomAttributeTypeProvider<string>
        {
            public static readonly StringAttributeTypeProvider Instance = new StringAttributeTypeProvider();

            public string GetPrimitiveType(PrimitiveTypeCode typeCode) => typeCode.ToString();

            public string GetSystemType() => "System.Type";

            public string GetSZArrayType(string elementType) => elementType + "[]";

            public string GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
                => "definition";

            public string GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
                => "reference";

            public string GetTypeFromSerializedName(string name) => name;

            public PrimitiveTypeCode GetUnderlyingEnumType(string type) => PrimitiveTypeCode.Int32;

            public bool IsSystemType(string type) => string.Equals(type, "System.Type", StringComparison.Ordinal);
        }
    }
}
