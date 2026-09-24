# WP-H1-06 physical-local observation

This is the evidence-reconciliation observation, not the final frozen-SHA receipt. The complete canonical command ran from a clean checkout at `f0b874d64cd2e4ef7d79a52d1a118ac23e3840c9` on the owner workstation (Windows x64), using Unity `6000.3.24f1 (4e7b9b5b6244)` and .NET `8.0.425`:

```powershell
scripts/h1-06-local-evidence.ps1 -AssetsRoot <owner-configured> -ExpectedSha f0b874d64cd2e4ef7d79a52d1a118ac23e3840c9
```

Result: **GREEN**. The command checked the H1-04 approved Source pins before and after, effective Unity catalogue inventory, the content-shape probe, locked Release build, three focused H1-06 plan tests, public prefab conformance, source immutability, missing/wrong-type/rebound diagnostics, same-input idempotence, and derivative deletion/rebuild convergence. The candidate was clean before and after.

The public conformance output reported four realized nodes; source logical ID `quaternius.medieval.prefab.wall-plaster-window-wide-flat`, native GUID `a914dbae2609f0107a8bce353d33727c`, local file ID `-927199367670048503`, and source SHA-256 `45825c565b9d1027036ce7fc922f7e7a7d69bc05e459d886eb738bf1eafc92b8`. The initial and rebuilt graph digest was `df9889057dec9096232a73dbd19bbb66783799e731a730080044b9b81df71c69`; the initial realization digest was `a98b2e0da87666e42809efb9eee0add8e1d1ab67337bad807b7bc51d3d127d63`. Normalized relationship profiles matched after rebuild, and canonical world hash/journal stayed unchanged.

The source archive is an external read-only input. Generated `ManagedPrefabs/`, `ManagedScenes/` and Unity `Library/` output remain ignored editor-local state. This result file commits the observed facts. The complete command must be rerun on the later evidence-bearing candidate SHA, with its exact receipt persisted in PR #195 before freeze.

## Nested-relationship repair and complete implementation observation

Strict Worker challenge of the first observation found that the accepted positive slice has no nested prefab, so it could not by itself establish the explicit nested relationship claim. The Worker added evaluated source-versus-derivative relationship comparison and a harness-only Unity fixture. The fixture creates a nested source prefab, saves a managed variant, reloads the scene and confirms the nested link; replacing that nested child with a flattened child returns `projection.prefab-nested-lineage-missing`. This fixture stays under an ignored managed proof root and deletes itself after execution. It does not alter H1-04 Source or catalogue authority.

The complete canonical command ran GREEN on clean implementation SHA `1404da8b312b86046a036fde0f5fc179b466f7d0` using the same physical editor, .NET SDK, approved Source root and platform. The focused .NET suite passed 3/3; the public conformance reported four Quaternius nodes, stable missing/wrong-type/rebound diagnostics, idempotence, unchanged source and canonical journal/hash, and matching normalized relationships after delete/rebuild. The nested fixture reported `arkus.h1-06-nested-prefab-conformance@1`, result `GREEN`, positive `nested-prefab-link-preserved-after-save-reload`, negative `projection.prefab-nested-lineage-missing`. The graph digest remained `df9889057dec9096232a73dbd19bbb66783799e731a730080044b9b81df71c69`; the realization digest for this run was `7a7792f3766bdf61b63b54e7099ee011c60d6507eb2012072d8647987b76ebe6`. Native generated GUIDs may differ across runs; stable source/relationship identity is the compared claim.

This observation is still pre-evidence SHA. The final evidence-bearing SHA requires one complete read-only rerun and a durable exact-SHA receipt before pre-review/freeze.

The later strict Worker challenge also made the nested fixture invoke the actual `H1SceneProjection.ObserveRealization` path on the saved/reloaded variant and require a normalized `nested-prefab` row. Its focused Unity run returned `productPath: ObserveRealization:nested-prefab-row` and retained the flattened-child negative result. The final exact-SHA command includes this stronger fixture gate.
