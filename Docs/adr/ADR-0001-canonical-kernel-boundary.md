# ADR-0001 — Canonical portable kernel boundary

Status: ACCEPTED (WP-HK-00 candidate)
Date: 2026-09-18
Workpack: `WP-HK-00`
Supersedes: none

## Context

`Docs/ROADMAP.md` puts a harness kernel under everything else and makes Unity a
downstream consumer. WP-HK-00 has to create the smallest code and build boundary
the rest of the harness can grow on, and it has to be provable rather than
merely asserted: every later workpack inherits whatever this one gets wrong.

Three properties matter more than convenience here.

1. The kernel must stay consumable by Unity later without importing Unity now.
2. The dependency direction must be a fact about the build, not a diagram.
3. A future change that breaks either must fail CI loudly, not silently.

## Decisions

### 1. Module set and direction

The module direction required by the workpack is implemented exactly:

```text
Arkus.Harness.Protocol
Arkus.Game.Core
Arkus.Game.World        -> Core
Arkus.Game.Authoring    -> Core + World + Protocol
Arkus.Game.Validation   -> Core + World + Protocol
Arkus.Harness.Runtime   -> Protocol + Authoring + Validation
Arkus.Harness.Cli       -> Runtime
Arkus.Harness.Tests     -> accepted public/test surfaces
```

Three projects exist beyond that list and are deliberately **not** part of the
kernel: `Arkus.Kernel.Proof` and `Arkus.Kernel.Proof.Tool` (build/proof tooling)
and the test project. They are classified as `tooling` and `tests` in
`kernel-manifest.json`, no kernel project may depend on them, and the dependency
check enforces that.

### 2. Portable contract: `netstandard2.1` and C# 9

Portable kernel libraries target `netstandard2.1` with `LangVersion 9.0`, which
is the contract a Unity consumer can still honour. The headless host, the proof
tooling and the tests target pinned `net8.0`; none of them is consumed by Unity.

C# 9 is a real constraint and not a formality: the kernel already had to avoid a
C# 10 interpolation helper during this workpack, which is the kind of drift the
`LangVersion` check exists to catch.

### 3. Toolchain pinning

`global.json` pins `8.0.100` with `rollForward: latestPatch`, which fixes the
major, minor and feature band and allows only security/patch movement inside
`8.0.1xx`. Roll-forward policies looser than that are rejected in tooling code
(`SdkPinRules.IsNarrowEnough`), not in data, so loosening the pin means changing
code and tests rather than editing one file.

An exact-version pin (`rollForward: disable`) was rejected: it makes the
repository unbuildable on any machine that has a different patch of the same
band, which in practice leads to the pin being deleted rather than respected.
The exact SDK that produced any given run is recorded in the run report instead.

Applications additionally set `RollForward: LatestPatch` so a host never
silently runs on a newer major runtime than the one it was proven against.

### 4. Warnings, suppressions and packages

`TreatWarningsAsErrors` is on repository-wide. The only tolerated suppressions
are the SDK's own `1701;1702` defaults, recorded as an allow-list in the
manifest; anything else is a finding. Package versions are centrally managed
(`Directory.Packages.props`), production classes may not take NuGet
dependencies at all, and lock files with `--locked-mode` restore keep the
dependency set reproducible.

### 5. Declared direction is the compile-time surface

`DisableTransitiveProjectReferences` is `true` repository-wide. Without it, a
project can use any type from anywhere in its transitive closure and the
declared edges become decoration. With it, `Arkus.Harness.Runtime` physically
cannot see `Arkus.Game.Core`, so the layering is enforced by the compiler.

### 6. Build output shape that makes the proof possible

`UseArtifactsOutput`, `Deterministic`, `DebugType=portable` and
`EmbedAllSources` are set so every build leaves behind an inspectable record of
what the compiler consumed: the portable PDB document table lists every source
file with a SHA-256 checksum, including files with no executable code.

### 7. One declarative manifest, checked mechanically

`kernel-manifest.json` is the single source of truth for project classification,
ownership, allowed edges, forbidden engine names and per-class build contracts.
It is data, and it is never trusted on its own: `Arkus.Kernel.Proof` checks the
repository against it, and `KernelManifestTests` checks the manifest against the
module direction the workpack requires.

### 8. Three independent oracles instead of a syntax denylist

The proof does not grow a list of forbidden spellings. It asks three different
layers what actually happened:

| Phase | Source of truth | Answers |
|---|---|---|
| `static` | evaluated MSBuild properties and items | what the build resolves before any target runs |
| `effective` | built assembly metadata and portable PDB | which assemblies are really referenced, which sources were really compiled, and whether their content still matches disk |
| `compiler` | the real `csc` command line (`ProvideCommandLineArgs`) | which options, references and source files the compiler actually received |

The `static` phase alone is not a completeness argument, and the self-attacks
prove it: a source file dropped by a build target, and warnings-as-errors
switched off by a build target, are both invisible to evaluation and caught by
the other two phases.

A Roslyn-based analysis of the source tree was rejected. It would add a NuGet
dependency to tooling that is otherwise dependency-free, and it would re-derive
what the real compiler already reports.

### 9. Boundary placeholders instead of empty projects

Each module exposes a tiny `*Module` type whose `ComposedModules` property calls
into the modules it depends on. This is not gameplay semantics and not a
convenience for tests: it makes each declared edge a real cross-assembly
reference, which is what lets the effective oracle require that every declared
dependency is actually exercised. Protocol envelopes, world state, authoring and
validation semantics remain owned by WP-HK-01 and later.

## Consequences

- Adding a project means classifying it in the manifest; an unclassified project
  or an unowned production source file fails CI.
- Changing the module direction means changing the manifest *and*
  `KernelManifestTests`, which is deliberate friction.
- Moving a kernel project off `netstandard2.1`/C# 9, adding a suppression, or
  taking a NuGet dependency in production code all fail CI.
- The proof costs roughly a minute of CI for the pipeline plus a separate job for
  the self-attack suite; that cost is accepted for a foundational workpack.
- DFU and Unity remain out of the tree entirely. Their return requires its own
  ADR after `WP-HK-GATE`, as the roadmap states.
