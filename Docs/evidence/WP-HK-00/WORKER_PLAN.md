# WP-HK-00 Worker pseudo-plan

Status: ACTIVE PLAN — implementation not yet frozen
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

The proof must enumerate repository C# project/source inputs independently of any kernel classification manifest. A tracked project/source cannot disappear by changing scan roots. A physical untracked C# input in the checkout is a failure for proof execution.

### Project universe

Discover all tracked `*.csproj` files repository-wide from the independent repository universe. Compare the resulting set with the fixed HK00 contract classification. Extra, missing or relocated projects are observable failures.

### Source universe

Discover all tracked `*.cs` files repository-wide before build. Every source must have exactly one allowed ownership disposition:

- owned by exactly one classified project; or
- explicitly non-product documentation/example material only if the contract defines such a category independently.

For this candidate, prefer the simpler rule: tracked `.cs` means it must be owned by a classified project.

Generated compiler inputs are a separate effective-build universe and must never substitute for tracked product ownership.

### Effective build universe

For each classified project inspect evaluated MSBuild inputs and the compiler-produced result. Static project XML is defence in depth only.

At minimum compare:

- evaluated project references/properties/Compile items;
- actual compiler source/reference/options where obtainable;
- produced assembly references/TFM;
- portable PDB compiled-document set/checksums for repository sources.

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

Host/tests/proof tooling may use pinned .NET 8 only where the WP permits it. Product kernel never references proof/test/host infrastructure backwards.

## Oracle separation

No single oracle is sufficient.

1. **Git/checkout oracle** — establishes the repository universe independently.
2. **Fixed contract oracle** — owns the module/class/build policy in reviewed code/tests, not mutable inventory data.
3. **MSBuild evaluated oracle** — observes what projects/properties/items resolve to.
4. **Compiler/effective oracle** — observes actual source/reference/options consumed.
5. **Assembly/PDB oracle** — observes produced references, TFM and compiled repository-source checksums.
6. **CI presence oracle** — fails if any required proof/test path is missing.

Where two oracles share implementation machinery, the proof matrix must state the shared-failure risk and preserve an independently reproducible command where practical.

## Causal attack map before freeze

The candidate is not READY until each material class below has an injected RED→GREEN attack with a named intended oracle:

| Defect class | Intended independent/effective detection |
|---|---|
| extra project anywhere in repository | Git/project universe vs fixed contract |
| missing classified project | fixed contract vs Git/project universe |
| unowned source anywhere in repository | Git/source universe vs ownership map |
| source removed only at build time | PDB/compiler effective source oracle |
| source linked/compiled by two projects | effective compiler/PDB ownership |
| source injected from outside repository | compiler/PDB path boundary |
| generated source contains unexpected product code | generated-input policy must be structural, not filename-only |
| dependency cycle/back-edge | evaluated ProjectReference graph + assembly refs |
| undeclared effective kernel dependency | assembly/compiler reference oracle |
| forbidden Unity/DFU dependency | fixed policy + evaluated/compiler/assembly references |
| package/raw assembly dependency in production | evaluated project + compiler reference oracle |
| warnings-as-errors/property drift | evaluated properties + compiler options + real warning negative control |
| toolchain pin drift | fixed pin policy vs `global.json` and running SDK |
| proof/test/CI tool deletion | independent required-path preflight |
| proof universe made configurable/shrinkable | schema/code test rejects configurable boundary knobs |
| policy relaxed in inventory/config | fixed contract oracle disagrees and fails |

Attack implementations should target these causal classes, not enumerate endless MSBuild syntax variants.

## Generated-source rule

The previous candidate allowed generated files based largely on filename patterns. That is insufficient: a target can emit arbitrary product code using an innocuous generated filename.

The new candidate must either:

1. prove exact allowed generated-source origins/content classes structurally from the SDK/effective build; or
2. make product projects reject unexpected generated compiler sources entirely for HK00, except a minimal fixed SDK-owned set whose provenance is independently checked.

Prefer option 2 unless evidence shows it is impractical.

## Reuse policy for PR #1/#2 work

Potentially reusable after re-evaluation:

- module topology and minimal composition idea;
- MSBuild evaluated facts;
- PDB/assembly inspection techniques;
- compiler command-line inspection;
- negative-control harness mechanics;
- exact-SHA CI pattern.

Do **not** reuse as authority:

- `kernel-manifest.json` as the source of material policy;
- configurable scan roots/exclusion lists;
- filename-only generated-source trust;
- any proof statement whose universe comes only from the manifest/registry it is validating;
- old READY/residual-risk conclusions.

## Implementation stages

1. Create pinned solution/module skeleton with no product semantics.
2. Encode fixed HK00 contract in proof/test policy code.
3. Build independent Git/checkout project+source universe.
4. Add evaluated MSBuild oracle.
5. Add effective compiler/assembly/PDB oracle.
6. Add independent inventories derived from observation, not used as policy authority.
7. Implement attack harness from the causal map.
8. Produce proof matrix/residual risk and independently reproducible commands.
9. Run exact-SHA CI, regenerate evidence, freeze only with zero unresolved/known-undetected classes.

## Worker STOP rules

Stop and redesign before freeze if:

- any material completeness set can be reduced by editing the same data it validates;
- a new bypass is another instance of an already-known causal class rather than a genuinely new class;
- an acceptance claim needs a growing syntax denylist instead of an evaluated/effective oracle;
- a residual risk can still falsify the central HK00 claim while CI remains green;
- proving generated-source provenance becomes more complex than simply forbidding it for product modules in HK00.

## Intended handoff

Reviewer should be able to reconstruct the complete claim from current GitHub state, run the independent universe checks without trusting Worker prose, and attack at least one omission class not listed here.
