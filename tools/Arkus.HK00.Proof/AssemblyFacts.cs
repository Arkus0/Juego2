using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;

namespace Arkus.HK00.Proof
{
    internal sealed class CompiledDocument
    {
        public CompiledDocument(string path, ImmutableArray<byte> hash, Guid hashAlgorithm)
        {
            Path = path;
            Hash = hash;
            HashAlgorithm = hashAlgorithm;
        }

        public string Path { get; }
        public ImmutableArray<byte> Hash { get; }
        public Guid HashAlgorithm { get; }
    }

    internal sealed class AssemblyFacts
    {
        public static readonly Guid Sha256Algorithm = new Guid("8829d00f-11b8-4213-878b-770e8597ac16");

        private AssemblyFacts(
            string assemblyName,
            IReadOnlyList<string> references,
            IReadOnlyList<CompiledDocument> documents,
            int assemblyFileCount,
            int manifestResourceCount,
            int nativeResourceSize,
            Guid? codeViewGuid,
            Guid? portablePdbGuid)
        {
            AssemblyName = assemblyName;
            AssemblyReferences = references;
            CompiledDocuments = documents;
            AssemblyFileCount = assemblyFileCount;
            ManifestResourceCount = manifestResourceCount;
            NativeResourceSize = nativeResourceSize;
            CodeViewGuid = codeViewGuid;
            PortablePdbGuid = portablePdbGuid;
        }

        public string AssemblyName { get; }
        public IReadOnlyList<string> AssemblyReferences { get; }
        public IReadOnlyList<CompiledDocument> CompiledDocuments { get; }
        public int AssemblyFileCount { get; }
        public int ManifestResourceCount { get; }
        public int NativeResourceSize { get; }
        public Guid? CodeViewGuid { get; }
        public Guid? PortablePdbGuid { get; }
        public bool PdbIdentityMatchesPe =>
            CodeViewGuid.HasValue
            && PortablePdbGuid.HasValue
            && CodeViewGuid.Value == PortablePdbGuid.Value;

        public static AssemblyFacts Read(string assemblyPath)
        {
            if (!File.Exists(assemblyPath))
            {
                throw new FileNotFoundException("Built assembly not found.", assemblyPath);
            }

            var pdbPath = Path.ChangeExtension(assemblyPath, ".pdb");
            if (!File.Exists(pdbPath))
            {
                throw new FileNotFoundException("Portable PDB not found.", pdbPath);
            }

            string name;
            var references = new List<string>();
            var assemblyFileCount = 0;
            var manifestResourceCount = 0;
            var nativeResourceSize = 0;
            Guid? codeViewGuid = null;

            using (var stream = File.OpenRead(assemblyPath))
            using (var pe = new PEReader(stream))
            {
                var reader = pe.GetMetadataReader();
                name = reader.GetString(reader.GetAssemblyDefinition().Name);
                foreach (var handle in reader.AssemblyReferences)
                {
                    references.Add(reader.GetString(reader.GetAssemblyReference(handle).Name));
                }

                assemblyFileCount = reader.AssemblyFiles.Count;
                manifestResourceCount = reader.ManifestResources.Count;
                nativeResourceSize = pe.PEHeaders.PEHeader?.ResourceTableDirectory.Size ?? 0;

                foreach (var entry in pe.ReadDebugDirectory())
                {
                    if (entry.Type != DebugDirectoryEntryType.CodeView)
                    {
                        continue;
                    }

                    codeViewGuid = pe.ReadCodeViewDebugDirectoryData(entry).Guid;
                    break;
                }
            }

            references.Sort(StringComparer.Ordinal);
            var documents = new List<CompiledDocument>();
            Guid? portablePdbGuid = null;
            using (var stream = File.OpenRead(pdbPath))
            using (var provider = MetadataReaderProvider.FromPortablePdbStream(stream))
            {
                var reader = provider.GetMetadataReader();
                if (reader.DebugMetadataHeader is not null)
                {
                    var id = reader.DebugMetadataHeader.Id;
                    if (id.Length >= 16)
                    {
                        portablePdbGuid = new Guid(id.Take(16).ToArray());
                    }
                }

                foreach (var handle in reader.Documents)
                {
                    var document = reader.GetDocument(handle);
                    documents.Add(new CompiledDocument(
                        reader.GetString(document.Name),
                        reader.GetBlobContent(document.Hash),
                        reader.GetGuid(document.HashAlgorithm)));
                }
            }

            documents.Sort((a, b) => string.CompareOrdinal(a.Path, b.Path));
            return new AssemblyFacts(
                name,
                references,
                documents,
                assemblyFileCount,
                manifestResourceCount,
                nativeResourceSize,
                codeViewGuid,
                portablePdbGuid);
        }

        public static byte[] Sha256OfFile(string path)
        {
            using var stream = File.OpenRead(path);
            using var sha = SHA256.Create();
            return sha.ComputeHash(stream);
        }

        public static bool HashEquals(ImmutableArray<byte> recorded, byte[] computed)
        {
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
    }
}
