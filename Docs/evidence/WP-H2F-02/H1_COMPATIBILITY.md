# H1 compatibility of the adopted baseline

H2F-02 changes the Unity toolchain under the accepted H1 bridge. It does not reopen H1 authority. Below are every H1-facing byte it changed, why, and the effective proof.

## 1. H1 bytes changed (complete list)

| File | Change | Why |
|---|---|---|
| `Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1Baseline.cs` | `RequireEffectiveBaseline` now requires the exact adopted URP asset (`Assets/Juego2/Foundation/Rendering/J2_URP_High.asset`, type `UniversalRenderPipelineAsset`) instead of built-in. The inventory keeps recording the effective pipeline type. The asmdef still references nothing. | H1-02 froze "built-in" as the baseline. H2F-02 is the only WP allowed to turn the URP selection into project truth, and H1 must still fail closed on any other pipeline. |
| `Unity/ArkusUnity/Assets/Arkus/H1/Tests/Editor/H1BaselineTests.cs` | `BuiltInRenderPipelineIsTheRetainedBaseline` → `Juego2UrpIsTheRetainedBaseline` | same |
| `Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json` | row `Assets/Arkus/H1/SourceSlice/MI_Plaster.mat`: `dependencies` `[]` → `["Packages/com.unity.render-pipelines.universal/Editor/AssetVersion.cs"]`. The other 273 rows are byte-identical; content hash, GUID, dimensions and `compatible` are unchanged. | This committed snapshot is H1's live catalogue contract. The owner-vault Source material (Medieval **URP** archive) already carries URP's `AssetVersion` sub-object. Under built-in that script reference did not resolve; with URP installed it does. H1 failed closed exactly as designed (`projection.catalogue-snapshot-stale`, hosted H1-07..11 on `d770bf20`; the H1-11 diagnostics listed this single row as the only difference). |
| `tests/Arkus.Harness.Tests/H1CatalogueTests.cs` | pinned catalogue fingerprint `e1e92d98…c3e1` → `ee13be7c…48ce` | The fingerprint covers dependencies. It was derived with `tools/h1_catalogue_fingerprint.py`, a replica of `H1CatalogueModel.FingerprintOf` that reproduces the old pinned value exactly on the pre-H2F-02 snapshot. Main Safety (the .NET suite) is GREEN on it. WP-H1-11 is the precedent: a WP that changes the catalogue re-baselines the snapshot and the fingerprint together. |

Nothing else under `Assets/Arkus/H1`, `tools/`, `src/`, `tests/` or H1 evidence changed. The H1 managed roots, CatalogueMapping, the H1 code paths and the source-adoption records are untouched.

## 2. Effective proof

| Check | Result |
|---|---|
| Hosted H1-07 / H1-08 / H1-09 / H1-10 (private vault mounted, GameCI Unity 6000.3.24f1, Core absent) | GREEN on `1b91d43d` and `b6bfb507` after the re-baseline (runs listed in the PR). They failed on `d770bf20` before it, which is how the stale snapshot was found. |
| Hosted H1-11 representative real-source slice | every stage passed on `b6bfb507`: Stage0 import vs committed catalogue, A materialize/save/reload, A2 supplementary capture, B clean generated-output rebuild in a fresh editor, C removed/replaced asset diagnostics; parity and negative evidence GREEN. The cleanup's clean-tree assertion failed there because URP rewrote seven project files on its first render. The baseline commits that initialized state from `2a2a9112` on (see `URP_BASELINE.md`), and the final candidate's run is recorded in the PR. The supplementary rendered capture differs from the committed built-in capture, which is expected; the workflow states the capture is never the parity oracle. |
| Main Safety (.NET restore/build/test) | GREEN on every pushed SHA, including the re-pinned catalogue fingerprint |
| Local comparison (this workstation, no vault mount, no host-prepared plans) | `main` and this branch give the same 18 pass / 25 environment-only failures; the only difference is the renamed baseline test, which passes (`results/h1_local_suite_comparison.json`) |

## 3. Lifecycle boundary handed to H2F-03

- Selected-tool state lives in **retained sidecars** outside `Assets/Arkus/H1/Managed*`. This covers GC2 Characters/cameras/Triggers, Volumes/lights, spline containers, generated realizations, NavMeshSurface, Terrain and rigging.
- H1-managed scenes and prefabs keep only H1-allowlisted components (Transform, MeshRenderer, Animator, canonical link). A selected-tool component authored inside a managed scene is unmanaged drift, which H1 reports by design.
- The lighting/look sidecar is the active scene; H1-managed scenes load additively.
- Per-family expectations for materialize, observe, reconcile and rematerialize/clean rebuild are in `H1_LIFECYCLE_MATRIX.csv`. H2F-03 owns the composed lifecycle proof over that matrix with the full stack present.
