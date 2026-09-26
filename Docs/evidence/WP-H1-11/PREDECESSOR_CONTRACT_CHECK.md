# WP-H1-11 — PREDECESSOR_CONTRACT_CHECK

Status: WORKER ACTIVE (recorded before implementation, baseline `caacfbf2706aa4c41c076dfe5e4f58a6c35ce7e0`)

## Accepted direct predecessor

`WP-H1-10 — Project checkpoint and clean rebuild` is accepted.

- Frozen candidate / PRODUCT_SHA: `bad48d1cf76cb5c8d7c3870b3dc88dc9b9a5c5e5`
- Canonical implementation PR: `#230`
- Independent PASS review: `#5325001446` (supersedes the same Reviewer's withdrawn FAIL `#5324996772`)
- Implementation merge: `aa31db7d3ca34f72709db19b22375166c6fd42dd`
- `DOCSYNC_COMPLETE` on PR #230 names `Next WP: WP-H1-11`; the only DocSync change was the process-only causal review circuit breaker (PR #235, merge `caacfbf2`), which does not touch H0/H1 product contracts.

The H1 remaining-work compression amendment is binding (`Docs/workpacks/H1/H1_REMAINING_COMPRESSION_AMENDMENT.md`, PR `#209`, PASS `#5315077514`). It makes H1-11 conformance-first with a `ZERO_CODE` target outside selected-source manifest/import-configuration evidence.

## Authoritative escalations used

No accepted-contract capsule covers H1. Because H1-11 composes every H1 bridge stage over new real content, the Worker read the exact sources rather than status metadata:

- `WP-H1-04` source adoption (`SOURCE_ADOPTION.{md,json}`), `CatalogueMapping.json`, committed `EFFECTIVE_INVENTORY.json`, the catalogue model (`tools/Arkus.H1.UnityHost/H1CatalogueModel.cs`) and the Unity inventory (`H1CatalogueInventory.cs`);
- `WP-H1-ASSET-CLOUD` contract, DocSync and the vault verifier (`Tools/AssetVault/verify_h1_asset_vault.py`), plus the pinned private vault `Arkus0/Juego2-assets@ae782c5f08cc4144a7ff0c3d4af67451d4d86bb6`;
- the H1-05/06/07/08 materializer and adapters (`H1SceneProjection*.cs`, `H1ComponentProjection.cs`), `H1ManagedScenePlan.cs`;
- the H1-10 checkpoint/environment/parity implementation, its process-staged effective proof (`H1ProjectCheckpointEffectiveProofTests`, `H1ProjectCheckpointReconstructionTests`, `.github/workflows/h1-10-unity-validation.yml`) and `Docs/evidence/WP-H1-10/{PROOF_MATRIX,RESIDUAL_RISK}.md`;
- the derived CTX-DW-H1 projection lifecycle (`Docs/evidence/WP-CTX-DW-H1-01/LIFECYCLE.md`, `src/Arkus.DesignWorld/H1CatalogueProjection.cs`), because it pins the exact H1-04 authority blobs that an admitted-subset extension necessarily changes.

## Inherited guarantees consumed (not re-proved)

1. H1-04: the Quaternius Medieval Village MegaKit Source (URP archive, SHA-256 `b9d757dd…c8b10`) and Universal Animation Library Source (`UAL1.fbx`, SHA-256 `0556d52f…70842`) adoption, CC0 licence identity, logical-ID/native-locator catalogue model, move/copy/delete/omission behaviour and the fixed effective-inventory universe (immediate files of `SourceSlice` + `CatalogueProof`).
2. H1-ASSET-CLOUD: the private vault holds both complete adopted distributions; hash/GUID-verified fail-closed mount into the ignored `SourceSlice`; no private source bytes in retained public evidence; public tree identical to the candidate after cleanup.
3. H1-05: managed-scene planning from canonical state, deterministic input digest, staged generation and atomic publication, effective observation.
4. H1-06: read-only source prefabs, derivative prefab variants with lineage and exact source-derived relationship multiset.
5. H1-07: the allowlisted Transform / MeshRenderer (single material slot) / Animator (clip reference) / canonical-link adapters and their fail-closed reference resolution.
6. H1-08: validator composition and pre/postflight; a failed postflight never publishes.
7. H1-09: complete managed-scope effective observation and drift oracle.
8. H1-10: versioned checkpoint over accepted H0 snapshot/journal with bridge/profile/catalogue/source/package/editor fingerprints, fresh-process restore through accepted H0 import only, clean generated-output rebuild and normalized reconstruction parity (`H1ReconstructionParity`), structured `checkpoint.source-missing` / `checkpoint.source-rebound` blockers.

## Guarantees newly owned by H1-11

Exactly the WP's new guarantees, no generic subsystem:

- a representative selected-item manifest that extends the accepted H1-04 admitted subset **inside the same adopted distributions** (no new source, version or licence), with every item bound to its distribution entry path, content SHA-256 and original Unity sidecar GUID;
- the reproducible import/adaptation recipe for those items (mount from the verified distribution payload, original upstream `.meta` import settings) and their logical catalogue mapping;
- explicit, independently derived pivot/scale/axis and hierarchy expectations for the selected real sources, checked against Unity's effective import;
- one end-to-end street-corner/humanoid scenario over the representative slice through the accepted public bridge: resolve/import → canonical bind → materialize → validate → save/reload → inspect → checkpoint → delete generated → fresh restore → rebuild → normalized parity;
- actionable missing/incompatible diagnostics when a selected asset is removed or replaced;
- one supplementary rendered capture (never the parity oracle).

## Predecessor guarantees intentionally consumed

Catalogue reconciliation, materialization/publication, prefab lineage, component adapters, validation, observation/drift, checkpoint/restore/parity and H0 import/compare are invoked through their accepted public entry points only. H1-11 adds no second catalogue, materializer, adapter, validator, checkpoint store, parity oracle or persistence format. The admitted-subset extension necessarily changes the H1-04 catalogue *data* (mapping, adoption slice list, committed effective inventory and the adoption pin that guards against silent widening); the catalogue *mechanism* is unchanged. The derived CTX-DW-H1 projection is refreshed only because its accepted lifecycle requires a new identity whenever the pinned H1-04 authority blobs change.

## Concrete reopen conditions

- H1-04 reopens only if the accepted source/catalogue identity cannot be reproduced from the pinned vault, or the catalogue model cannot represent a correctly admitted real item without a semantic change.
- H1-05/06/08 reopen only if a representative real prefab/mesh materializes, validates or publishes differently from its accepted contract for identical plan/source inputs (for example a lost source hierarchy or a published failed postflight).
- H1-07 reopens only if an allowlisted adapter mis-resolves or silently drops a real material/clip reference inside its accepted schema. The single-slot renderer schema is accepted scope, not a defect: multi-material real meshes keep their imported bindings through prefab sources.
- H1-10 reopens only if checkpoint/restore/rebuild parity is false for the representative slice under the accepted normalization.
- A missing production-art feature (for example content absent from the adopted distributions) is downstream H2/ART scope, not a predecessor reopen.

Theoretical possibility or a desire for duplicate proof is not a reopen condition.
