# H1 non-interference after merging `main` (WP-H1-GATE PASS)

2026-09-27. After `WP-H1-GATE` (merge `b730ba0`) and its DocSync were on `main`, the ART-01 branch merged `origin/main` at `2e10beb1`. Against `main`, the branch now differs **only** under `Unity/ArkusUnity/Assets/Arkus/ART/**`, `Docs/evidence/WP-ART-01/**`, `Tools/art01_*` and `.gitignore`. Packages, ProjectSettings and render pipeline were identical to `main` at the time of this check.

## Method

The same Unity `6000.3.24f1` EditMode suite (`Arkus.H1.Baseline.Tests.Editor`, 43 cases) was run locally on two checkouts:

| Checkout | ART present | Result file |
|---|---|---|
| `codex/wp-art-01` after the merge | yes, including the 51 locked ART source copies | `editmode_with_art.xml` |
| detached `origin/main` `2e10beb1` | no | `editmode_main_2e10beb1.xml` |

Both used the accepted H1-04 SourceSlice, mounted from the owner's read-only `C:\Juego2-Assets` by a faithful Python port of `scripts/h1-04-import-source.ps1`. That script needs PowerShell 7's `ZipFile`, which is absent on this workstation; the port applies the same hash assertions and writes the same bytes and metas.

## Result

- Both checkouts: 43 cases, 16 passed, 27 failed.
- **Differing test cases: 0.**

The 27 local failures are identical without ART. They are staged proofs that CI runs through GameCI plus .NET hand-offs: missing H1-09/H1-10/H1-11 proof plans, published generations, and orchestrated SourceSlice material imports. They are not something this local suite can reproduce.

Conclusion: at `main` `2e10beb1`, the presence of ART-01 assets, scripts and scene changes no H1 EditMode outcome. The comparison is repeated after the URP integration (see `URP_INTEGRATION.md`).
