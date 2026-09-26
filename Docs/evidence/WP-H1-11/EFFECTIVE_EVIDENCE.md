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
| A2 | Unity Editor | supplementary rendered capture of the published generation (not the parity oracle) |
| A3 | .NET | observation decoded by the public observe executor; rig relationships present; checkpoint captured from the effective observation |
| — | shell | every generated scene/prefab deleted |
| B | .NET | fresh process restores canonical truth only through accepted H0 snapshot import (`new-local-lineage`, `sameAuthorableState`) |
| B | Unity Editor | clean generated-output rebuild in a fresh Editor; same graph digest; inspection repeated after rebuild |
| C | .NET | normalized reconstruction parity through `H1ReconstructionParity`; drift on a real humanoid/roof node is rejected |
| C | Unity Editor | removed crate / replaced wagon / rebound skinned material are named on the affected canonical node; materialize refuses without replacing the published generation; exact bytes restore a clean observation |
| D | .NET | the live catalogue names the removed/replaced logical ID; the public checkpoint restore blocks with `checkpoint.source-missing` / `checkpoint.source-rebound` |
| — | Python | mounted upstream bytes still equal the admitted hashes |

## Established facts

Filled from the effective runs below the freeze; see the PR pre-review comment for the exact-SHA run IDs.
