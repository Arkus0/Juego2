# WP-HK-00 Worker pseudo-plan

Status: IMPLEMENTED — stabilization/evidence/pre-review/freeze pending  
Current governing main before final integration: `591ee50ab1d36a14a26a06156aeccc20f086b0f7`  
Worker: ChatGPT Worker

## Goal

Build the smallest portable Arkus kernel boundary that satisfies HK00 without letting the candidate silently shrink the project/source universe being proved.

The closure target is practical and finite: prove Arkus-owned module/source/dependency/build invariants on the canonical path, not the impossibility of malicious behavior by the trusted toolchain or CI infrastructure.

## Explicit trusted base

HK00 trusts Git; the exact pinned .NET SDK/runtime; normal documented MSBuild/C# compiler/NuGet behavior under the canonical build path; and OS/CI infrastructure. Identity, pinning, dependency closure and effective observations remain checked, but HK00 does not recursively certify those implementations.

## Proof layers

- **Repository universe:** exact Git candidate tree; product C# and project existence/classification independent of mutable manifest scan roots.
- **Fixed Arkus contract:** reviewed module topology, TFMs, warnings policy, package allowance and engine boundary.
- **Canonical build:** exact solution membership, locked dependency graph and Linux restore/build/test.
- **Effective source proof:** evaluated/effective compiler sources plus PDB/source checks prove owned source actually compiles.
- **Defence in depth:** read-only candidate, import/build-surface closure, compiler extensions/references, normalized compiler args, byte stability and output integrity.

## Fixed module direction

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

Portable product libraries are `netstandard2.1`, C# 9 and warnings-as-errors. Product projects have no third-party package dependency and no Unity/DFU coupling. Tooling is fixed to SDK `8.0.425` / runtime `8.0.31` with SDK roll-forward disabled.

## Dependency reproducibility

Test dependencies use centrally pinned direct roots plus a committed `packages.lock.json` containing the complete resolved/transitive graph and `contentHash`. The proof observes a fresh isolated lock, compares it with the committed lock, then restores canonically in locked mode.

## Causal attack suite

37 existing RED→GREEN negative controls are retained. `TRUST_BOUNDARY_AUDIT.md` classifies them as direct HK00 obligations or defence in depth. No new family will be added merely for theoretical hostile implementation of Git/.NET/MSBuild/NuGet/CI.

## Completion state

1. Module/toolchain skeleton — DONE
2. Fixed canonical contract — DONE
3. Independent Git project/source universe — DONE
4. Canonical solution/build path — DONE
5. Effective source/compiler/assembly/PDB observation — DONE
6. Locked test dependency closure — DONE
7. Read-only candidate + observed evidence path — DONE
8. 37 causal self-attacks — IMPLEMENTED
9. Trust-boundary + proof-budget classification — DONE
10. Integrate governing proof-standard v1.2 / protocol v1.3 — IN PROGRESS
11. Evidence regeneration/reconciliation — PENDING real CI execution
12. Exact-SHA CI — PENDING; recent hosted runs failed before runner allocation
13. Mandatory Worker adversarial pre-review — PENDING complete evidence candidate
14. Freeze/Ready handoff — PENDING

## Closure discipline

A new finding blocks HK00 only if it falsifies a material acceptance criterion inside the declared trust boundary. A theoretical subversion of Git/toolchain/NuGet/runner itself is residual risk, not another hardening cycle. `PROOF_BUDGET_VERDICT` must remain `WITHIN_BUDGET` before freeze.

## Intended handoff

After evidence reconciliation and exact-SHA GREEN, the Worker performs Protocol v1.3 adversarial pre-review against real HK00 acceptance criteria. If CLEAN, the exact HEAD is frozen and handed to a fresh independent Reviewer. No HK00A/HK01 work begins in this Worker session.
