# WP-HK-00 — Causal self-attacks

Binding rule: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`, "Self-attack rule".

## Method

`scripts/self-attacks/run-self-attacks.sh` is the executable form of this
document. It is re-runnable, and it is the same script CI runs, so this evidence
cannot rot into a description of something that used to be true.

For every attack the harness:

1. copies the repository into a sandbox and proves the **pristine** sandbox
   builds and proves green — an attack against an already-red tree would prove
   nothing;
2. injects exactly one defect;
3. builds, when the defect is one that must survive to the compiler, and requires
   the build to succeed or fail as declared, with the declared error code where a
   compiler diagnostic is the oracle;
4. runs the proof tool and requires **the named check identifiers** to fire — and,
   where the point of the attack is that one layer is blind to it, requires the
   named check to *stay quiet*;
5. reverts by restoring the pristine tree, and proves the revert is exact with
   `diff -r` rather than assuming it;
6. rebuilds and requires the sandbox to prove green again.

Two details matter for whether this is evidence at all:

- **Assertions are on check identifiers, not exit codes.** A compile failure for
  an unrelated reason cannot be mistaken for a guard working, which is exactly
  what the standard forbids.
- **The revert restores timestamps too.** An earlier version of the harness kept
  original file times, which let MSBuild treat a tampered output as up to date
  and skip the rebuild; the "green again" step then inspected the *attacked*
  binaries. That was caught by the `hidden-generated-source` attack failing its
  green step, and fixed by extracting with `tar -m`. It is recorded here because
  a self-attack harness that quietly stops being causal is the most dangerous
  failure mode this workpack has.

## Required classes

The workpack requires RED→GREEN for six defect classes. All six are covered by
fifteen attacks, and four further attacks close classes found while building the
proof:

| Required class | Attack | Checks required to fire |
|---|---|---|
| Forbidden engine dependency | `forbidden-engine-dependency`, `forbidden-engine-package` | `HK00-ENGINE-DEP-STATIC`, `HK00-ENGINE-DEP-EFFECTIVE`, `HK00-PACKAGE-FORBIDDEN` |
| Dependency cycle / back-edge | `dependency-cycle`, `layering-back-edge` | `HK00-GRAPH-CYCLE`, `HK00-GRAPH-UNDECLARED-EDGE`, `HK00-REF-UNDECLARED-EFFECTIVE` |
| Duplicate product source ownership | `duplicate-source-ownership`, `duplicate-source-type-conflict` | `HK00-SOURCE-DUPLICATE-OWNERSHIP`, `HK00-SOURCE-FOREIGN-COMPILE-ITEM`, `HK00-SOURCE-FOREIGN-COMPILED-EFFECTIVE` |
| Unclassified production source | `unclassified-production-source` | `HK00-SOURCE-UNCLASSIFIED` |
| Source excluded from effective compilation | `source-excluded-from-compilation`, `source-excluded-by-build-target` | `HK00-SOURCE-NOT-COMPILED-STATIC`, `HK00-SOURCE-NOT-COMPILED-EFFECTIVE` |
| Toolchain drift | `toolchain-property-drift`, `toolchain-pin-loosened`, `toolchain-option-mutated-at-build-time`, `warnings-as-errors-neutralised`, `warning-suppression-added`, `effective-warning-as-error` | `HK00-TOOLCHAIN-PROPERTY`, `HK00-TOOLCHAIN-SDK-PIN`, `HK00-TOOLCHAIN-SUPPRESSION`, `HK00-COMPILER-OPTION`, plus a real CS0219 build failure |

Additional classes: `hidden-generated-source` (product code smuggled in as a
build-generated file), `package-pin-bypassed` (inline version instead of a
central pin), `required-project-deleted` and `proof-tool-deleted` (the proof
machinery itself going missing).

## The two attacks that justify the architecture

Most guards could have been written as project-file inspection. Two attacks show
why that would have been a false claim:

- **`source-excluded-by-build-target`** removes an owned source file from the
  compilation *inside an MSBuild target*, after evaluation. The evaluated
  `Compile` item list still contains it, so the static check is required to stay
  quiet — and it does. Only the portable-PDB document table and the real compiler
  source list see the file missing.
- **`toolchain-option-mutated-at-build-time`** switches warnings-as-errors off
  inside a target. The evaluated `TreatWarningsAsErrors` still says `true`, so
  the static property check is required to stay quiet — and it does. Only
  `/warnaserror-` on the actual compiler command line reveals it.

This is the concrete meaning of the standard's effective-behaviour rule in this
workpack: the static phase is defence in depth, and there is recorded proof that
it is not sufficient on its own.

## Results

Machine-generated table: `self-attacks/results.md`.
Per-attack logs (injection, build output, check identifiers, revert diff, green
re-run): `self-attacks/<attack>.log`. Paths in the logs are normalised to
`<REPO>` and `<SANDBOX>` so the evidence is readable from any checkout.
