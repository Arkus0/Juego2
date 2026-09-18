# Juego2

Harness-first game project.

> **Prime directive:** no serious gameplay work starts until the AI authoring harness can create, inspect, modify, validate, replay, and test a representative micro-world deterministically and headlessly.

The project is intentionally rebuilt from first principles. `Arkus0/Juego` is reference material only; code or architecture is migrated into this repository only after explicit justification.

## Where to start

| If you want | Read |
|---|---|
| The milestone order and gates | `Docs/ROADMAP.md` |
| The rules an agent must follow here | `AGENTS.md` |
| What "proven" means for a foundational workpack | `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` |
| The current unit of work and its Definition of Done | `Docs/workpacks/HK/` |
| Why the kernel is shaped the way it is | `Docs/adr/ADR-0001-canonical-kernel-boundary.md` |

## Kernel layout

```text
src/Arkus.Harness.Protocol     portable kernel (netstandard2.1, C# 9)
src/Arkus.Game.Core            portable kernel
src/Arkus.Game.World           portable kernel  -> Core
src/Arkus.Game.Authoring       portable kernel  -> Core + World + Protocol
src/Arkus.Game.Validation      portable kernel  -> Core + World + Protocol
src/Arkus.Harness.Runtime      portable kernel  -> Protocol + Authoring + Validation
src/Arkus.Harness.Cli          headless host    -> Runtime            (net8.0)
tools/Arkus.Kernel.Proof       boundary proof library                  (net8.0)
tools/Arkus.Kernel.Proof.Tool  boundary proof CLI                      (net8.0)
tests/Arkus.Harness.Tests      test surface                            (net8.0)
```

`kernel-manifest.json` is the declarative source of truth for that layout: which
projects exist, which class each belongs to, which dependency edges are allowed,
and which build contract each class must satisfy. It is checked mechanically and
never trusted on its own.

## Build and prove

The SDK is pinned in `global.json` (`8.0.1xx`). From a clean checkout:

```bash
bash scripts/proof.sh                            # restore, build, prove the boundary, run the host, run tests, regenerate inventories
bash scripts/self-attacks/run-self-attacks.sh    # inject every known defect class, require the right guard to fire, revert, re-prove
```

`scripts/proof.sh` exits `1` on a finding and `2` when it could not complete a
check; both are failures. CI runs the same two commands on Linux.

Ordinary day-to-day commands still work:

```bash
dotnet build Juego2.sln -c Release
dotnet test Juego2.sln -c Release
```
