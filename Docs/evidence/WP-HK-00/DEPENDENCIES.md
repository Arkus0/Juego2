# WP-HK-00 external-component and commercial boundary

Audit date: 2026-09-18

External components transfer implementation/tooling work; they do not define Arkus product semantics or the HK00 completeness universe. Product kernel projects have **zero third-party NuGet PackageReference**.

## Toolchain / CI

| Component | Exact identity | License / upstream | HK00 role | Replacement / authority boundary |
|---|---|---|---|---|
| .NET SDK | `8.0.425`; `global.json` rollForward `disable` | Microsoft .NET, MIT | MSBuild, Roslyn `csc`, build/test host, proof execution | Arkus owns project/source/dependency policy and proof obligations. Toolchain replacement requires fixed-contract change plus conformance/self-attacks. |
| .NET runtime/ref pack | `Microsoft.NETCore.App` / `Microsoft.NETCore.App.Ref` `8.0.31` | Microsoft .NET, MIT | Executes proof/tests; framework compiler refs | Runtime/ref pack are pinned external toolchain inputs, not Arkus semantic authority. |
| actions/checkout | `11d5960a326750d5838078e36cf38b85af677262` | GitHub, MIT | Exact SHA checkout | CI plumbing only; proof separately checks Git HEAD/tree. |
| actions/setup-dotnet | `67a3573c9a986a3f9c594539f4ab511d57bb3ce9` | GitHub, MIT | Activates pinned SDK | CI plumbing only; runtime proof asserts exact SDK/runtime. |
| actions/upload-artifact | `ea165f8d65b6e75b540449e92b4886f43607fa02` | GitHub, MIT | Evidence transport | Cannot determine PASS; used after proof/attack execution. |
| GitHub-hosted runner | `ubuntu-24.04` runner family | External service/image | Linux CI | Not semantic authority. Current allocation outage is fail-closed: no freeze without executed exact-SHA jobs. |

## Locked test-only NuGet closure

The authoritative identity is `tests/Arkus.Harness.Tests/packages.lock.json`: every resolved package carries an exact version and NuGet `contentHash`; a fresh isolated restore is byte-compared to that lock and canonical restore then runs in locked mode.

| Package | Exact version | Direct/transitive | License family / upstream | Role / boundary |
|---|---:|---|---|---|
| Microsoft.NET.Test.Sdk | 17.14.1 | Direct | Microsoft Test Platform, MIT | Test SDK integration; injects one exact locked `Microsoft.NET.Test.Sdk.Program.cs` source that is explicitly verified in compiler/PDB evidence. |
| Microsoft.CodeCoverage | 17.14.1 | Transitive | Microsoft Test Platform, MIT | Test-platform support only; not product dependency. |
| Microsoft.TestPlatform.TestHost | 17.14.1 | Transitive | Microsoft Test Platform, MIT | Test host only. |
| Microsoft.TestPlatform.ObjectModel | 17.14.1 | Transitive | Microsoft Test Platform, MIT | Test-platform object model only. |
| Newtonsoft.Json | 13.0.3 | Transitive | Newtonsoft.Json, MIT | TestHost transitive implementation dependency only. |
| System.Reflection.Metadata | 8.0.0 | Transitive | dotnet/runtime, MIT | Test-platform transitive dependency. |
| System.Collections.Immutable | 8.0.0 | Transitive | dotnet/runtime, MIT | Test-platform transitive dependency. |
| xunit.core | 2.9.3 | Direct | xUnit.net, Apache-2.0 | Test framework core only; v2 is legacy/deprecated upstream and must not become Arkus infrastructure authority. |
| xunit.assert | 2.9.3 | Direct | xUnit.net, Apache-2.0 | Assertions only. |
| xunit.runner.visualstudio | 2.8.2 | Direct | xUnit.net, Apache-2.0 | Test discovery adapter; `PrivateAssets=all`. |
| xunit.abstractions | 2.0.3 | Transitive | xUnit.net, Apache-2.0 | xUnit v2 transitive test support only. |
| xunit.extensibility.core | 2.9.3 | Transitive | xUnit.net, Apache-2.0 | xUnit v2 transitive test support only. |
| xunit.extensibility.execution | 2.9.3 | Transitive | xUnit.net, Apache-2.0 | xUnit v2 transitive test support only. |

Project entries recorded in the lock are Arkus project references, not third-party packages.

## External-authority constraints

- Product projects permit no NuGet packages.
- The full test package graph is a committed lock with content hashes; direct package declarations alone are not treated as sufficient proof.
- Locked packages are **not** blanket compiler-extension authority: analyzers/source generators remain restricted to selected SDK/reference packs.
- Test MSBuild imports and ordinary test assembly references may originate only from exact package id/version paths present in the committed lock.
- Framework compiler references must come from selected `.NET` packs; Arkus references must be exact outputs of declared project dependencies.
- No component in this file may redefine canonical Arkus contracts, world semantics, engine abstraction or completeness universe.

## Commercial note

The current dependency set is permissively licensed and confined to build/test/CI infrastructure. xUnit v2 is intentionally treated as replaceable legacy test tooling rather than a product pillar. Before distributable commercial packaging, the wider product process still owes SBOM/notices generation under the repository dependency/IP policy; HK00 establishes the dependency boundary and exact closure, not the final release SBOM.
