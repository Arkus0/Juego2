# WP-HK-00 Worker pseudo-plan

Status: IMPLEMENTED — final exact-SHA validation and freeze pending  
Baseline: `a45985f02e64f0b44c3e1ed1156b7fa0bb47a567`  
Worker: ChatGPT Worker

## Goal and design rule

Build the smallest portable Arkus kernel boundary that satisfies HK00 without allowing the proof universe or product guarantees to be weakened by the same configuration being proved.

**Policy is not inventory.** Material guarantees are fixed in reviewed proof code; mechanically generated inventories describe observed state but cannot redefine project scope, scan roots, dependency policy, TFMs, warnings policy, engine exclusions or proof presence.

## Independent universes

- **Repository:** exact tracked Git tree plus physical checkout checks; all tracked `.csproj` and `.cs` discovered repository-wide.
- **Fixed contract:** separate reviewed project/module/build policy in `Arkus.HK00.Proof`.
- **Solution:** `dotnet sln Juego2.sln list` must exactly equal the fixed project universe.
- **MSBuild:** evaluated properties/items/project references plus `MSBuildAllProjects` import closure.
- **Compiler:** actual C# source/reference/options/analyzer inputs.
- **Outputs:** emitted assembly references and portable-PDB source documents/checksums.
- **Candidate bytes:** tracked files hashed before/after effective build so targets cannot mutate the candidate invisibly.

## Fixed HK00 direction

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

Portable product libraries are `netstandard2.1`, C# 9, deterministic and warnings-as-errors. Product projects allow no packages/raw binary references and no Unity/DFU dependencies. Required dependency markers are non-const so required graph edges are causally present in emitted IL.

Tooling is fixed to .NET SDK `8.0.425`, `rollForward: disable`; CI action code is pinned by commit SHA and runner family is `ubuntu-24.04`.

Generated/untracked C# product compiler inputs are structurally rejected. Effective analyzers/source-generators are allowed only from the selected SDK or `.NET packs` under the same `DOTNET_ROOT`; repo/NuGet/external compiler extensions are rejected.

## Causal attack suite

23 RED→GREEN attacks cover:

- extra/missing projects and solution omission;
- unowned/cross-owned/dropped/external/generated sources;
- dependency cycle/back-edge and decorative required edge;
- production package and engine/raw reference injection;
- static and late analyzer/source-generator injection;
- explicit and auto-imported repository build extensions;
- tracked source mutation during build;
- real warning under warnings-as-errors;
- toolchain drift and proof-tool deletion;
- legacy self-shrinking manifest attempt;
- terminal NUL-delimited Git inventory omission.

Every attack requires its intended oracle ID to fire, reconstructs pristine state and then requires GREEN.

## Reuse policy from abandoned attempts

Re-used only after re-evaluation: module topology, MSBuild facts, compiler/PDB/assembly inspection techniques, negative-control mechanics and exact-SHA CI pattern.

Explicitly not retained as authority: `kernel-manifest.json`, configurable scan roots/exclusion lists, filename-only generated-source trust, or old READY/risk conclusions.

## Completion state

1. Module/toolchain skeleton — DONE
2. Fixed contract — DONE
3. Independent Git universe — DONE
4. Solution + MSBuild/import closure — DONE
5. Compiler/assembly/PDB/candidate-byte oracles — DONE
6. Mechanical inventories — DONE
7. 23 causal self-attacks — DONE
8. Proof matrix/residual-risk/dependency/verdict evidence — DONE
9. Final exact-SHA CI + freeze — PENDING final run

## Worker STOP rules

Stop/redesign before freeze if any material set can self-shrink; a known causal class needs another syntax patch instead of an effective oracle; a negative control does not turn red for its intended cause; or residual risk can still falsify the central HK00 claim while CI remains green.

## Intended handoff

A fresh Reviewer must reconstruct the claim from GitHub, distrust Worker prose, verify PR HEAD equals the frozen SHA, and attack at least one omission class not highlighted by Worker.
