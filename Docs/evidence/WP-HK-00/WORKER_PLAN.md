# WP-HK-00 Worker pseudo-plan

Status: IMPLEMENTED — evidence reconciliation and final exact-SHA freeze pending
Baseline: `a45985f02e64f0b44c3e1ed1156b7fa0bb47a567`
Worker: ChatGPT Worker

## Goal

Create the smallest portable Arkus kernel boundary that satisfies `WP-HK-00` without allowing the proof universe or the product guarantees to be weakened by the same configuration being proved.

This restart deliberately does not migrate PR #1/#2 wholesale. Previous implementation is reference material only. Components may be re-used only after they fit this plan and the new proof standard.

## Design rule zero

**Policy is not inventory.**

The candidate may use generated inventories/metadata to describe observed state, but the material guarantees below are not configurable by those inventories:

- canonical module set/direction for this WP;
- project class/build contract;
- portable target/language baseline;
- production package/raw-reference prohibition;
- engine/DFU exclusion;
- repository/project/source completeness boundary;
- warnings-as-errors requirement;
- proof/test/CI presence requirements.

Changing one of those guarantees requires changing reviewed contract/policy code or an ADR, not editing a data file that the proof then trusts.

## Independent universes

### Repository universe

Primary universe: tracked Git tree at the exact candidate SHA, cross-checked against the physical checkout before restore/build.

The proof enumerates repository C# project/source inputs independently of any kernel classification manifest. A tracked project/source cannot disappear by changing scan roots. A physical untracked C# input in the checkout is a failure for proof execution.

### Project and solution universes

All tracked `*.csproj` files are discovered repository-wide from Git and compared with the fixed HK00 contract classification. Extra, missing or relocated projects are observable failures.

`Juego2.sln` is checked independently with `dotnet sln ... list`; its C# project membership must exactly equal the fixed project universe. This prevents a project from remaining classified/proved in isolation while disappearing from the canonical restore/build/test entrypoint.

### Source universe

All tracked `*.cs` files are discovered repository-wide before build. For this candidate, tracked `.cs` means it must be owned by exactly one classified project.

Generated compiler inputs are a separate effective-build universe and never substitute for tracked product ownership.

### Effective build universe

For each classified project the proof inspects evaluated MSBuild inputs and the compiler-produced result. Static project XML is defence in depth only.

The proof compares:

- evaluated project references/properties/Compile items;
- evaluated `MSBuildAllProjects` import closure, rejecting repository-owned build extensions outside the fixed policy surface while allowing generated untracked `obj` imports;
- actual compiler source/reference/options/analyzer inputs;
- produced assembly references;
- portable PDB compiled-document set/checksums for repository sources;
- candidate tracked bytes before/after effective build execution.

## Fixed HK00 contract

Required direction:

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

Portable product libraries: `netstandard2.1`, C# 9, deterministic build, warnings-as-errors, no package dependencies, no raw binary references, no engine/DFU/game-content references.

Host/tests/proof tooling use exact .NET SDK `8.0.425` with `rollForward: disable`; GitHub Actions are pinned by immutable action SHA and the Linux runner family is pinned to `ubuntu-24.04`.

Product kernel never references proof/test/host infrastructure backwards.

## Oracle separation

No single oracle is sufficient.

1. **Git/checkout oracle** — establishes the repository universe independently.
2. **Fixed contract oracle** — owns the module/class/build policy in reviewed code, not mutable inventory data.
3. **Solution membership oracle** — independently proves the canonical solution contains exactly the fixed project universe.
4. **MSBuild evaluated oracle** — observes resolved properties/items/project references and effective import closure.
5. **Compiler/effective oracle** — observes actual source/reference/options/analyzer inputs consumed.
6. **Assembly/PDB oracle** — observes produced references and compiled repository-source checksums.
7. **Candidate-byte oracle** — detects tracked-file mutation during build/proof execution.
8. **CI presence oracle** — fails if any required proof/test/attack path is absent.

Where two oracles share implementation machinery, the proof matrix states the shared-failure risk and preserves independently reproducible commands where practical.

## Causal attack map before freeze

The candidate is not READY until material defect classes below have an injected RED→GREEN attack with a named intended oracle. Implemented attacks cover:

- extra project anywhere in repository;
- missing classified project;
- solution omitting a fixed project;
- unowned source anywhere in repository;
- source removed only at build time;
- cross-project source compile;
- source injected from outside repository;
- generated product source injection;
- dependency cycle/back-edge;
- required dependency made decorative in emitted IL;
- forbidden engine/raw reference;
- package dependency in production;
- repository/custom analyzer or source-generator;
- analyzer/source-generator injected late by a target;
- explicit custom build import;
- auto-imported repository `Directory.Build.targets` outside the fixed policy surface;
- tracked source mutated during build;
- real warning under warnings-as-errors;
- toolchain pin drift;
- proof project deletion;
- legacy self-shrinking manifest attempt;
- terminal NUL-delimited Git inventory entry omission;
- plus baseline GREEN reconstruction after each injected defect class.

Attack implementations target causal classes rather than enumerating endless MSBuild syntax variants.

## Generated-source rule

The previous candidate allowed generated files based largely on filename patterns. That was rejected.

HK00 product projects instead reject generated/untracked C# compiler inputs structurally. Official compiler analyzers/source-generators are allowed only from the exact selected SDK or `.NET packs` beneath the same `DOTNET_ROOT`; repo/NuGet/external compiler extensions are rejected.

## Reuse policy for PR #1/#2 work

Re-used after re-evaluation:

- module topology and minimal composition idea;
- MSBuild evaluated facts;
- PDB/assembly inspection techniques;
- compiler command-line inspection;
- negative-control harness mechanics;
- exact-SHA CI pattern.

Not re-used as authority:

- `kernel-manifest.json` as material policy;
- configurable scan roots/exclusion lists;
- filename-only generated-source trust;
- any proof statement whose universe comes only from the manifest/registry it validates;
- old READY/residual-risk conclusions.

## Implementation stages

1. Create pinned solution/module skeleton with no product semantics. — DONE
2. Encode fixed HK00 contract in proof/test policy code. — DONE
3. Build independent Git/checkout project+source universe. — DONE
4. Add solution membership + evaluated MSBuild/import-closure oracles. — DONE
5. Add effective compiler/assembly/PDB/candidate-byte oracles. — DONE
6. Add observation-derived inventories that are not policy authority. — DONE
7. Implement causal attack harness. — DONE; final suite contains 23 attacks across three attack entrypoints.
8. Produce proof matrix/residual-risk/verdict evidence. — PENDING final evidence commit.
9. Run exact-SHA CI and freeze only with zero unresolved/known-undetected classes. — PENDING.

## Worker STOP rules

Stop and redesign before freeze if:

- any material completeness set can be reduced by editing the same data it validates;
- a new bypass is another instance of an already-known causal class rather than a genuinely new class;
- an acceptance claim needs a growing syntax denylist instead of an evaluated/effective oracle;
- a residual risk can still falsify the central HK00 claim while CI remains green;
- proving generated-source provenance becomes more complex than forbidding it for product modules in HK00.

## Intended handoff

Reviewer should be able to reconstruct the complete claim from current GitHub state, run the independent universe checks without trusting Worker prose, and attack at least one omission class not listed here.
