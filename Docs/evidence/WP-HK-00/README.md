# WP-HK-00 evidence pack

Workpack: `Docs/workpacks/HK/WP-HK-00.md`
Standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`
Decision record: `Docs/adr/ADR-0001-canonical-kernel-boundary.md`

| File | What it is |
|---|---|
| `VERDICT.md` | The foundational proof verdict and its counts. Read this first. |
| `PROOF_MATRIX.md` | One row per acceptance criterion: completeness argument, positive evidence, negative control, result, residual risk. |
| `SELF_ATTACKS.md` | How the negative controls work and why two of them justify the whole architecture. |
| `RESIDUAL_RISK.md` | Known limitations, each with the argument that bounds it. |
| `inventory/projects.json` | Every classified project, its class, contract and declared edges. Regenerated and diffed in CI. |
| `inventory/sources.json` | Every source file with owner, class, SHA-256 and whether it was statically and effectively compiled. |
| `inventory/graph.json` | Declared edges, project-reference edges and effective assembly-reference edges. |
| `self-attacks/results.md` | Machine-generated table of every injected defect and the checks it had to trip. |
| `self-attacks/*.log` | Per-attack record: injection, build, check identifiers, revert diff, green re-run. |

## Reproducing this from a clean checkout

```bash
bash scripts/proof.sh                            # ~1 min: preflight, static, effective, compiler, host smoke test, tests, inventory
bash scripts/self-attacks/run-self-attacks.sh    # ~20 min: every negative control, red then green
git diff --exit-code -- Docs/evidence/WP-HK-00/inventory   # inventories must not drift
```

`scripts/proof.sh` exits non-zero on any finding (exit 1) and on any run it could
not complete (exit 2). CI runs exactly these commands.

## Checking this without trusting the proof tool

A Reviewer can re-derive the central facts directly from the SDK:

```bash
# evaluated build contract of one kernel project
dotnet msbuild src/Arkus.Game.World/Arkus.Game.World.csproj \
  -getProperty:TargetFramework -getProperty:LangVersion -getProperty:TreatWarningsAsErrors -getItem:Compile

# what the compiler was actually given
dotnet build src/Arkus.Game.World/Arkus.Game.World.csproj -c Release \
  -t:Rebuild -p:ProvideCommandLineArgs=true -getItem:CscCommandLineArgs

# what the built assembly actually references, and which sources it was built from
#   -> artifacts/bin/<project>/release/<project>.dll  (AssemblyRef table)
#   -> artifacts/bin/<project>/release/<project>.pdb  (document table, SHA-256 per file)
```
