namespace Arkus.Kernel.Proof
{
    /// <summary>
    /// Stable identifiers for every mechanical check.
    /// </summary>
    /// <remarks>
    /// Self-attacks assert on these identifiers, not on exit codes or message
    /// text: a negative control only counts when the intended guard turns red for
    /// the intended reason.
    /// </remarks>
    public static class CheckIds
    {
        /// <summary>A file required by the manifest is absent.</summary>
        public const string PreflightFileMissing = "HK00-PREFLIGHT-FILE-MISSING";

        /// <summary>A project declared by the manifest is absent from disk.</summary>
        public const string PreflightProjectMissing = "HK00-PREFLIGHT-PROJECT-MISSING";

        /// <summary>A required test project is absent or not declared.</summary>
        public const string PreflightTestProjectMissing = "HK00-PREFLIGHT-TEST-PROJECT-MISSING";

        /// <summary>The proof tool project itself is absent or not declared.</summary>
        public const string PreflightProofToolMissing = "HK00-PREFLIGHT-PROOF-TOOL-MISSING";

        /// <summary>A project is not listed in the solution.</summary>
        public const string PreflightSolutionProjectMissing = "HK00-PREFLIGHT-SOLUTION-PROJECT-MISSING";

        /// <summary>A project exists on disk but is not classified by the manifest.</summary>
        public const string ManifestProjectUndeclared = "HK00-MANIFEST-PROJECT-UNDECLARED";

        /// <summary>A project references a class that the manifest does not define.</summary>
        public const string ManifestClassUnknown = "HK00-MANIFEST-CLASS-UNKNOWN";

        /// <summary>A declared dependency names a project the manifest does not define.</summary>
        public const string ManifestDependencyUnknown = "HK00-MANIFEST-DEPENDENCY-UNKNOWN";

        /// <summary>A production source file is owned by no project.</summary>
        public const string SourceUnclassified = "HK00-SOURCE-UNCLASSIFIED";

        /// <summary>A source file is compiled by more than one project.</summary>
        public const string SourceDuplicateOwnership = "HK00-SOURCE-DUPLICATE-OWNERSHIP";

        /// <summary>An owned source file is missing from evaluated compiler inputs.</summary>
        public const string SourceNotCompiledStatic = "HK00-SOURCE-NOT-COMPILED-STATIC";

        /// <summary>A project's evaluated compiler inputs include source it does not own.</summary>
        public const string SourceForeignCompileItem = "HK00-SOURCE-FOREIGN-COMPILE-ITEM";

        /// <summary>An owned source file is missing from what the compiler actually consumed.</summary>
        public const string SourceNotCompiledEffective = "HK00-SOURCE-NOT-COMPILED-EFFECTIVE";

        /// <summary>The compiler actually consumed source the project does not own.</summary>
        public const string SourceForeignCompiledEffective = "HK00-SOURCE-FOREIGN-COMPILED-EFFECTIVE";

        /// <summary>Compiled source content does not match the owned file on disk.</summary>
        public const string SourceContentMismatch = "HK00-SOURCE-CONTENT-MISMATCH";

        /// <summary>A project reference edge exists that the manifest does not declare.</summary>
        public const string GraphUndeclaredEdge = "HK00-GRAPH-UNDECLARED-EDGE";

        /// <summary>A declared dependency edge is absent from the project files.</summary>
        public const string GraphMissingEdge = "HK00-GRAPH-MISSING-EDGE";

        /// <summary>The dependency graph contains a cycle or back-edge.</summary>
        public const string GraphCycle = "HK00-GRAPH-CYCLE";

        /// <summary>A project reference points outside the classified project set.</summary>
        public const string GraphUnknownTarget = "HK00-GRAPH-UNKNOWN-TARGET";

        /// <summary>A project file declares a forbidden engine or game-content dependency.</summary>
        public const string EngineDependencyStatic = "HK00-ENGINE-DEP-STATIC";

        /// <summary>A built assembly actually references a forbidden engine assembly.</summary>
        public const string EngineDependencyEffective = "HK00-ENGINE-DEP-EFFECTIVE";

        /// <summary>A built assembly references a kernel assembly that is not a declared dependency.</summary>
        public const string ReferenceUndeclaredEffective = "HK00-REF-UNDECLARED-EFFECTIVE";

        /// <summary>A declared dependency is never actually used by the built assembly.</summary>
        public const string ReferenceUnexercised = "HK00-REF-UNEXERCISED";

        /// <summary>A production project declares a raw assembly reference.</summary>
        public const string ReferenceRawAssembly = "HK00-REF-RAW-ASSEMBLY";

        /// <summary>The SDK pin is missing, loosened, or not satisfied by the running SDK.</summary>
        public const string ToolchainSdkPin = "HK00-TOOLCHAIN-SDK-PIN";

        /// <summary>An evaluated build property does not match the class contract.</summary>
        public const string ToolchainProperty = "HK00-TOOLCHAIN-PROPERTY";

        /// <summary>A warning suppression outside the allowed set is in effect.</summary>
        public const string ToolchainSuppression = "HK00-TOOLCHAIN-SUPPRESSION";

        /// <summary>A built assembly's actual target framework does not match the contract.</summary>
        public const string ToolchainEffectiveTfm = "HK00-TOOLCHAIN-EFFECTIVE-TFM";

        /// <summary>A project class that forbids NuGet dependencies declares one.</summary>
        public const string PackageForbidden = "HK00-PACKAGE-FORBIDDEN";

        /// <summary>A package version is floating or declared outside central pinning.</summary>
        public const string PackageUnpinned = "HK00-PACKAGE-UNPINNED";

        /// <summary>A build output required by the effective oracles is absent.</summary>
        public const string OutputMissing = "HK00-OUTPUT-MISSING";

        /// <summary>The compiler was actually invoked with an option outside the class contract.</summary>
        public const string CompilerOption = "HK00-COMPILER-OPTION";

        /// <summary>The compiler was actually invoked with a suppression outside the allowed set.</summary>
        public const string CompilerSuppression = "HK00-COMPILER-SUPPRESSION";

        /// <summary>The compiler's actual source list does not match the owned source set.</summary>
        public const string CompilerSourceMismatch = "HK00-COMPILER-SOURCE-MISMATCH";

        /// <summary>The compiler was actually given source from outside the project's ownership.</summary>
        public const string CompilerSourceForeign = "HK00-COMPILER-SOURCE-FOREIGN";

        /// <summary>The compiler was actually given a kernel reference that is not a declared dependency.</summary>
        public const string CompilerReferenceUndeclared = "HK00-COMPILER-REFERENCE-UNDECLARED";

        /// <summary>The compiler was actually given a forbidden engine reference.</summary>
        public const string EngineDependencyCompiler = "HK00-ENGINE-DEP-COMPILER";

        /// <summary>A build-generated source file does not match any expected generated-source pattern.</summary>
        public const string SourceUnexpectedGenerated = "HK00-SOURCE-UNEXPECTED-GENERATED";
    }
}
