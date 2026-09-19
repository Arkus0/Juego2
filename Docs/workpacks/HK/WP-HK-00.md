# WP-HK-00 — Canonical portable kernel boundary

Status: COMPLETE  
Class: FOUNDATIONAL  
Depends on: none  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

Completion:
- Implementation PR: `#8`
- Reviewed candidate SHA: `014345b44035c796aa8509f88dcec227c476fcc7`
- Independent Reviewer verdict: `PASS`
- Merge SHA: `80dd20cf09b31e1d2b050410d46619b06511878f`
- Completed: `2026-09-19`

## Objective

Create the smallest canonical code/build boundary on which the harness can safely grow, without Unity, DFU or gameplay coupling.

## Required module direction

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

Exact names may change only if the same ownership/dependency properties are preserved and documented.

## Acceptance

- Canonical product source has single ownership: no copied implementation and no linked/cherry-picked product files into alternate projects.
- Portable libraries target a Unity-compatible contract (`netstandard2.1`, C# 9 unless an ADR proves a better compatible baseline); host/tests may use pinned .NET 8.
- No UnityEngine, UnityEditor, DFU or game-content dependencies exist in the portable kernel.
- Dependency graph is mechanically checked and acyclic.
- SDK/toolchain is pinned; clean restore/build/test works headlessly on Linux CI.
- Warnings-as-errors for canonical kernel projects.
- CI fails closed if a required project/test/proof tool is absent.
- Source/project inventories are generated mechanically; unowned/unclassified production `.cs` count is zero.
- Effective compiler inputs for each canonical project are inspectable enough to prove owned source is what is actually compiled; static project checks are defence in depth, not the sole completeness claim.

## Required self-attacks

At minimum demonstrate RED→GREEN for: forbidden engine dependency, dependency-cycle/back-edge, duplicate product source ownership, unclassified production source, source excluded from effective compilation, and toolchain drift.

## Forbidden scope

Unity project/scenes/assets, Quaternius, DFU, gameplay systems, AI transport features, Creator GUI, automation orchestration.

## DoD

Exact-SHA CI green; proof matrix complete; zero unresolved/undetected classes; fresh independent Reviewer PASS.
