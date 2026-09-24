# WP-H1-06 physical-local observation

This is the evidence-reconciliation observation, not the final frozen-SHA receipt. The complete canonical command ran from a clean checkout at `f0b874d64cd2e4ef7d79a52d1a118ac23e3840c9` on the owner workstation (Windows x64), using Unity `6000.3.24f1 (4e7b9b5b6244)` and .NET `8.0.425`:

```powershell
scripts/h1-06-local-evidence.ps1 -AssetsRoot <owner-configured> -ExpectedSha f0b874d64cd2e4ef7d79a52d1a118ac23e3840c9
```

Result: **GREEN**. The command checked the H1-04 approved Source pins before and after, effective Unity catalogue inventory, the content-shape probe, locked Release build, three focused H1-06 plan tests, public prefab conformance, source immutability, missing/wrong-type/rebound diagnostics, same-input idempotence, and derivative deletion/rebuild convergence. The candidate was clean before and after.

The public conformance output reported four realized nodes; source logical ID `quaternius.medieval.prefab.wall-plaster-window-wide-flat`, native GUID `a914dbae2609f0107a8bce353d33727c`, local file ID `-927199367670048503`, and source SHA-256 `45825c565b9d1027036ce7fc922f7e7a7d69bc05e459d886eb738bf1eafc92b8`. The initial and rebuilt graph digest was `df9889057dec9096232a73dbd19bbb66783799e731a730080044b9b81df71c69`; the initial realization digest was `a98b2e0da87666e42809efb9eee0add8e1d1ab67337bad807b7bc51d3d127d63`. Normalized relationship profiles matched after rebuild, and canonical world hash/journal stayed unchanged.

The source archive is an external read-only input. Generated `ManagedPrefabs/`, `ManagedScenes/` and Unity `Library/` output remain ignored editor-local state. This result file commits the observed facts. The complete command must be rerun on the later evidence-bearing candidate SHA, with its exact receipt persisted in PR #195 before freeze.
