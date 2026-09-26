# WP-H1-11 — Effective evidence

Executor: GitHub-hosted Unity (accepted `WP-H1-UNITY-CI` substrate) with the pinned private asset vault (accepted `WP-H1-ASSET-CLOUD`), workflow `.github/workflows/h1-11-unity-validation.yml`. Every stage is a separate process; the only durable hand-off is the accepted H1-10 project checkpoint plus bounded proof files. Raw Unity/GameCI output and private source bytes are deleted before the job ends; the public tree must equal the candidate SHA afterwards.

The exact-SHA run identifiers for the frozen candidate are recorded in the Worker pre-review comment on the canonical PR (a commit cannot contain its own run IDs). This file records the stable facts the effective runs established.

## Stage chain

| Stage | Process | What it proves |
| --- | --- | --- |
| Resolve | Python, vault verifier | adopted distributions and the four H1-04 slices hash/GUID-verified; the 12 representative archive entries and their original sidecars verified against the manifest and mounted fail-closed; source-derived manifest facts recomputed and equal; mounted universe = admitted slices |
| 0 | Unity Editor | clean import equals the facts derived from the source bytes (per-item importer unit settings, handedness/unit-scale bounds, root-collapse hierarchy, material slot multiset, shared kit materials, rig bones/bind poses, clip resolution) and the effective catalogue inventory equals the committed snapshot |
| A | .NET | canonical street-corner world (16 bound objects, one H0 mutation) planned through the accepted planner; coverage of every selected item, clip, shared override and nested depth |
| A | Unity Editor | accepted materialize → postflight → publication; reconciliation clean; observe reopens the generation from disk; inspection of hierarchy/material/mesh/clip identities and facade/kit scale composition; upstream bytes untouched |
| A2 | Unity Editor | supplementary rendered capture of the published generation (not the parity oracle); SourceSlice and published generation byte-identical around the draw |
| A3 | .NET | observation decoded by the public observe executor; rig relationships present; checkpoint captured from the effective observation |
| — | shell | every generated scene/prefab deleted |
| B | .NET | fresh process restores canonical truth only through accepted H0 snapshot import (`new-local-lineage`, `sameAuthorableState`) |
| B | Unity Editor | clean generated-output rebuild in a fresh Editor; same graph digest; inspection repeated after rebuild |
| C | .NET | normalized reconstruction parity through `H1ReconstructionParity`; drift on a real humanoid/roof node is rejected |
| C | Unity Editor | removed crate / replaced wagon / rebound skinned material are named on the affected canonical node; materialize refuses without replacing the published generation; exact bytes restore a clean observation |
| D | .NET | the live catalogue names the removed/replaced logical ID; the public checkpoint restore blocks with `checkpoint.source-missing` / `checkpoint.source-rebound` |
| — | Python | mounted upstream bytes still equal the admitted hashes (also re-checked after every Editor process exits) |

## Established facts

First complete GREEN chain: H1-11 run `36230271886` (job `108372049047`) on `2207da31d7948c6117020eb13b99586cddbff7f6`. The frozen candidate re-runs the same chain; its run IDs and any changed digest are recorded in the Worker pre-review comment. Digests below are content-addressed and stable for identical inputs.

**Resolve / import (Stage 0)**

- Mounted universe: 16 admitted SourceSlice files (4 accepted H1-04 slices + 12 representative entries), each equal to its admitted hash before the flow, after every Editor process has exited, and at the end (`H1_11_MOUNTED_SOURCE_UNCHANGED_GREEN files=16` after Unity 0/A/A2/B/C and at the end).
- Effective catalogue inventory equals the committed snapshot: `committed=274 effective=274 equalRows=True`.
- Importer unit settings per item equal the sidecar-derived recipe: the 12 kit entries import with the upstream `useFileScale` conversion (file scale `0.01`, `globalScale 1`), the facade and UAL1 with the accepted H1-04 GUID-only defaults (no conversion). Every non-rig item's bounds oracle is discriminating (the alternative convention falls outside tolerance).
- Shared kit materials: one imported material per source material name across the kit entries (`MI_Brick`, `MI_Plaster`, `MI_RockTrim`, `MI_RoundTiles`, `MI_Vine`, `MI_WindowGlass`, `MI_WoodTrim`, `MI_WoodTrim_Wear`).
- Humanoid: `Mannequin` skinned mesh with 65 bones and matching bind poses. Clips resolve from logical IDs: idle `Idle_Loop` 2.5 s, walk `Walk_Loop` 1.333 s, sit `Sitting_Idle_Loop` 1.667 s; each animates 66 rig transforms, every one a node of the rig recomputed from the UAL1 FBX bytes and present in the imported rig.

**Canonical bind and plan (Process A)**

- World `world.h1-11.street-corner`, 16 bound nodes, 14 selected items, 3 clips; canonical hash `267695451f5173bf3daef6fcdaa68c6bc32b4c7c35fc726b83089afd7e00b33b`, plan input digest `b50aba2c9b4a16b88162f106ba07803c4c29a16bb62a68e29b3342e3626f9ef3`, catalogue fingerprint `e1e92d9878a4fea1ac9fb876d10b17a870db7ac68a669dd97ecf49b036f8c3e1`. Coverage oracle: no uncovered item, clip, shared override or nesting obligation.

**Materialize → save/reload → inspect (Unity A) and rebuild (Unity B)**

- Graph digest `762a8a0d371593a6843cd0adbdbfd718c2186163ded3db3e7a0a0881f539c051` for the baseline and for the clean rebuild in a fresh Editor.
- Inspection after reload, identical before and after the rebuild: every prefab node's realized subtree equals its source prefab; material slots per node (facade 3, wall.door 3, window/roof/chimney/door/wall.side 2, each civilian 2 skinned, others 1); the two asset nodes bind the shared catalogue material `quaternius.medieval.material.wall-plaster-window-wide-flat-mi-plaster`; each civilian owns 68 transforms, 65 bones, its exact clip and 66 animated transforms resolving under the realized rig.
- Scale composition: facade world width `2.0` m equals the kit wall width `2.0` m (accepted H1-04 facade in file units bound at 1 %, kit wall with the upstream unit conversion).

**Checkpoint, restore and parity (Processes A3/B/C)**

- Checkpoint `09614a63d302e0c6680550757ef6ce5f7b93e622c20d4c825b040902aa1d3fdd`, manifest SHA-256 `5c913813b11b133c9fa75336511a3482eb987d3fade10d1de4b496436b623f7a`, source fingerprint `6887464d78ca6f329c57313fa2b8c7d3c886ddeceacda65fa8d7604b83fb0019`. The observation it was captured from carries each civilian's skinned `Mannequin` mesh and both skinned material references.
- Fresh-process restore only through accepted H0 import reproduces the canonical hash and plan input digest.
- Normalized reconstruction parity through `H1ReconstructionParity`: reconstruction digest `ec5ab177ad073d655365b31a0a516445b13986758e75f22ebd559baac6f1d0ba` over 16 nodes, while the realization digests of baseline (`a225b0fa…`) and rebuild (`de2e574c…`) differ in the generation-specific locators that the accepted normalization excludes. The drift control on `civilian.walk`/`roof.house` is rejected with `checkpoint.restore-observation-drift`.

**Removed / replaced asset (Unity C, Process D)**

- Removed `Prop_Crate.fbx`: reconciliation names `crate.stack` (`projection.reconciliation-node-unreadable`, `projection.source-wrong-type`, see F9); materialize refuses with `projection.catalogue-snapshot-stale` and the published generation is unchanged; the live catalogue reports `catalogue.stale-mapping` naming `quaternius.medieval.asset.prop-crate`; public restore blocks with `checkpoint.source-missing`.
- Replaced `Prop_Wagon.fbx` bytes: reconciliation names `wagon.street` (`projection.source-rebound`); materialize refuses with `projection.catalogue-snapshot-stale`; the live catalogue names the wagon logical ID; public restore blocks with `checkpoint.source-rebound`.
- Skinned material rebound inside the published generation: `civilian.walk` reported (`projection.prefab-source-reference-missing`).
- After restoring the exact bytes, reconciliation is clean (no diagnostics) and the recovered inventory rebuilds the catalogue.

**Supplementary capture (Unity A2) — not the parity oracle**

- `SUPPLEMENTARY_CAPTURE.jpg`: 960×540, `OpenGLCore`, SHA-256 `a5d42476d1b74ffb1ceb7a5762669aef3353f621356f6a4dae7dbcd8e1b2c8d5`, 9.1 % covered pixels, 618 colour bins, graph digest `762a8a0d…` (the same generation). It is a derived render of CC0 content, not source bytes.
- The kit modules render untextured: no texture file of the kit is part of the admitted slice. The civilians show the bind pose because edit-mode capture does not evaluate the Animator. Neither is a claimed property.
- Drawing left 10 source materials dirty in the Editor (`FacadeImportedMaterial.mat` and nine importer-generated `Materials/*.mat`); the stage discarded that state and the whole SourceSlice and published-generation trees were byte-identical after the capture and after the Editor exited (F8).
