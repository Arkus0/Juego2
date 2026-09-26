# ART-01 derivative, source and material ledger

`KIT_COMPOSITION_MANIFEST.json` (schema `@2`) is the machine-readable ART companion for every bounded source model, admitted derivative and building/benchmark assembly. `Tools/art01_build_manifest.py` regenerates it from `SOURCE_LOCK.json` and the normalized Unity audit.

For each entry it records:
- stable logical ID, source file hash/license;
- effective bounds and numeric `dimensionsM`;
- root scale and rotation, pivot/facing;
- hosts, repeat/snap, connections;
- `formPrimitive` (`BOX` for intentional linear/slab pieces), `unityAssets`;
- declared `dimensionalExceptions`, collision role, incompatibilities and readiness.

Every tagged benchmark instance carries an `Art01Piece.kitId` resolving to exactly one entry. `Art01StructuralAudit` enforces this closure and the provenance fields.

These are ART composition fields. They do not redefine H1 catalogue IDs, Unity GUIDs as canonical identity, or CITY place/route semantics.

## Exact source and provenance

- **Pins.** The H1-04 accepted Medieval Village Source/Unity URP archive hash remains pinned in `SOURCE_LOCK.json`. The locally selected Nature, Props, Base Characters and UAL1 files and the exact CC0 license files are separately pinned there. `SOURCE_ADOPTION.md` records the adoption decision.
- **Verification.** `Tools/art01_import_sources.py verify` fails on changed archive/file/license bytes.
- **Import.** `import` copies **only** 51 selected files into the ignored `Assets/Arkus/ART/External`, keeps deterministic Unity meta GUIDs, and never edits the owner source vault.
- **What is committed.** The repository contains ART-owned derivatives and metadata, not a copy of the Quaternius packs.

## Admitted derivatives and assemblies

| ART identity/family | Lineage and concrete derivative | What it resolves |
|---|---|---|
| `render_host.{straight,window_wide_flat}.v1` | Extruded 2 m render hosts sharing the stone source wall faces (outer +0.0924, inner −0.3141). Mesh by `RenderHostMesh`. | Smooth Cantabrian render without alpine timber; real window voids; storeys flush with the stone storey |
| `facade.corner_closure.v1` (BOX) | 0.0924 × storey × 0.0924, derived from the measured wall face offset | Closes the notch where two module lines meet; no corner gap |
| `roof.lowpitch.{6x10,8x14}.v1` | Selected RoundTiles Source FBX, named per-family scale, placed by its **measured** underside on the outer wall-top line | Avoids the steep chalet source roof and any floating cap/eave slit; pitch 34° in the declared 28–36° band. Original roof models stay `SOURCE_ONLY`. |
| `gable.stone.v1` | Triangular prism with the wall's own faces; rise fitted to the measured roof underside | Flush closed gable, no rake slit |
| `eave.timber_plate.v1` (BOX) | Timber plate on the eave-side wall face | Readable eave line touching wall and roof underside |
| `plinth.segment.v1` (BOX) | Continuous embedded ring, inner wall face → 0.10 beyond outer face | Ground contact; private door sill; public gap only at the landing |
| `threshold.public.v1` (BOX) | Step + landing, embedded in the street | Two 0.123 m risers, 0.94 m landing, level with the public floor |
| `threshold.lean_to.v1` + `threshold.porch_frame.v1` | Pitched canopy mesh (back edge in the facade) + posts, stone feet and fascia | Physically supported entrance canopy |
| `interior.public_room_shell.v1`, `interior.counter.v1` (BOX) | Floor/ceiling and counter carcass | Human-scale public room context only |
| `street.crowned.v1` | One crowned street mesh with an integrated drainage-channel submesh | Single traversal owner, visible drainage |
| `street.edge_drain.v1` | Continuous mitred kerb meshes outside the traversable edge, embedded foot | Readable edge without joint gaps or route narrowing |
| `retaining.capped.v1` (BOX) | Low stone wall + cap, embedded | W12/Casco edge and F01 garden walls |
| `bridge.arch_span.v1` | Deck body with one segmental arch and flush spandrels | Bridgehead reads as a bridge over water, not a walled road on flat ground |
| `bridge.parapet_capped.v1`, `retaining.embankment.v1` (BOX) | Parapets on the deck edge; capped river embankment | Parapet/road/kerb meeting; coherent water-edge retaining condition |
| `site.scenic_bank.v1`, `site.river_water.v1` | Flat-datum-near-features scenic ground; visible river plane | Grounded buildings/edges; visible water |
| `sign.bar.v1` (BOX + TextMesh) | Flush wall board, one readable face | No floating board, no mirrored lettering. The generic word is a specimen label. |
| `clothed_scale.forastero.v1` | CC0 Quaternius `Regular_Male_FullBody`, `Hair_Buzzed`, UAL1 civilian idle; bounded Blender adaptation in `Tools/art01_make_scale_reference.py` | Clothed 1.81 m human reference; no NPC system |
| `assembly.house.two_storey_6x10.v1`, `assembly.house.bar_8x14.v1`, `assembly.benchmark.retained_chain.v1` | Layered assembly rules in the manifest `assemblies` section and `ASSEMBLY_PLANS.md` | Machine-readable building/benchmark composition |

`render_host.door_flat.v1` was listed at `290c487` but never generated or exercised (no render-storey door exists), so it is removed rather than claimed.

`WindowShutters_Wide_Flat_Closed` is now an admitted source insert. It is used where open leaves would overhang a facade end or clash with a neighbouring opening.

## Importer decisions

The effective Unity audit before normalization exposed:
- wall meshes at ~200 × 312 × 40 cm;
- roof 8×14 at ~995 × 678 × 1570 cm;
- Nature trees at ~431 × 726 × 457 cm;
- a Props Barrel visually ~69 × 89 × 69 **metres** when its root ×100 was naïvely retained.

The scoped ART postprocessor applies `globalScale=0.01` to Medieval/Nature/Props and leaves the Props prefab root ×100 intact. Props preserve their imported X≈270° root rotation; wrapper yaw supplies street facing. These are renderer-independent and final for the STRUCTURAL checkpoint.

## Materials — PROVISIONAL (built-in), replaced under URP

`Art01Materials.cs` currently maps each selected source material by name to explicit **built-in Standard** materials; unknown names fail closed. The active H1 Unity project is built-in, so these are valid for iteration and neutral inspection.

Per the owner's decision, URP is the production renderer. These materials, the overcast built-in light rig and the `provisional_builtin/` captures are **not frozen**. After `WP-H1-GATE`, ART-01 adopts URP for the production benchmark and replaces this path with a reproducible URP remap:
- pinned package version;
- versioned pipeline/renderer assets;
- explicit cutout/transparent handling;
- no silent Standard fallback and no magenta.

Family intent for the URP pass (from the Visual Bible): grey stone, restrained lime-grey render, dark timber, muted green shutters, darkened wet tile, cool wet cobble, desaturated ground.

The raw `Grass.png` is a foliage texture, not a ground-surface texture.

## Rejected and source-only pieces

- **Rejected as direct keeper defaults:** alpine half-timber/plaster-grid and chalet/steep roof forms, even when Quaternius supplies them.
- **Source-only/rejected:** `Wall_UnevenBrick_Window_Thin_Round` for this rectangular retained opening.
- **`SOURCE_ONLY`:** the raw Base Character/UAL mannequin. It cannot be presented as a final player-facing outfit.
- **Measured but not claimed as benchmark construction:** balcony, chimney, round-rock floor tile and short grass.
- **Not ingested:** props or plants outside the locked 51-file subset.
