# WP-HK-00 external-component boundary

Audit date: 2026-09-18

External components transfer implementation/tooling work; they do not define Arkus product semantics or the HK00 completeness universe.

| Component | Exact identity in candidate | License | HK00 role | Arkus guarantees outside component scope / replacement boundary |
|---|---|---|---|---|
| .NET SDK | `8.0.425`, `global.json` with `rollForward: disable` | MIT | MSBuild, C# compiler, build/test host and proof execution | Arkus owns fixed project/source/dependency policy and proof obligations. Toolchain replacement requires fixed-contract change plus conformance/self-attacks. |
| Microsoft.NET.Test.Sdk | `17.14.1` | MIT | Test discovery/execution integration only | Test-only; no product assembly or Arkus semantic authority. Replaceable behind the test-project boundary. |
| xunit | `2.9.3` | Apache-2.0 | Test assertions/execution only | Test-only; no product dependency. This v2 line is legacy upstream, so it is explicitly non-authoritative and must be re-evaluated before commercial release rather than becoming platform infrastructure. |
| xunit.runner.visualstudio | `2.8.2` | Apache-2.0 | Test discovery adapter | Test-only with `PrivateAssets=all`; replaceable without product-kernel changes. |
| actions/checkout | `11d5960a326750d5838078e36cf38b85af677262` | MIT | Exact candidate checkout in CI | CI plumbing only; Arkus independently verifies the candidate SHA. |
| actions/setup-dotnet | `67a3573c9a986a3f9c594539f4ab511d57bb3ce9` | MIT | Installs/activates pinned .NET toolchain | CI plumbing only; Arkus independently asserts `dotnet --version == 8.0.425`. |
| actions/upload-artifact | `ea165f8d65b6e75b540449e92b4886f43607fa02` | MIT | Publishes proof/self-attack logs | Evidence transport only; pass/fail is determined before artifact upload. |
| GitHub-hosted runner | `ubuntu-24.04` runner family | External service/image | Linux CI execution environment | Not semantic authority. Exact SDK/actions/package pins and clean-checkout proof reduce image drift. |
| NuGet.org | sole package source in `NuGet.config` | External service | Supplies pinned test-only packages | Product projects allow no packages. Package source does not define Arkus semantics; versions are centrally pinned. |

## Commercial/product boundary

No third-party NuGet package is referenced by a product kernel project in HK00. NuGet dependencies are confined to `Arkus.Harness.Tests`; GitHub Actions are CI infrastructure. No external component above can silently become the canonical Arkus model, protocol, world semantics or engine abstraction.
