# Owner visual judgement record

Date: 2026-09-27. The WP requires owner-visible visual judgement for spikes whose discriminator is appearance or feel (S01, S03, S04, S05, S06, S07).

## What the owner saw

The six labelled comparison sheets in `captures/owner/` were delivered to the owner in the Worker session:

- `S01_render.png`: Built-in vs URP A/B/C, evening variant, broken route B;
- `S03_water.png`: flat vs project water;
- `S04_nature.png`: deterministic scatter vs Terrain trees;
- `S05_window.png`: V1/V2/V3 non-enterable window plus the real F01 sequence;
- `S06_control_camera.png`: minimal vs GC2 play frames along the route;
- `S07_humanoid.png`: the ART citizen with UAL1/UAL2 civilian clips.

## Owner response (verbatim, Spanish)

> "Yo no veo mucha diferencia. Hasta que no esté a los mandos con city07 no te sabría decir. Dejo en tus manos que explores las fotos y decidas que se ve mejor."

In other words: the owner inspected the sheets, saw no decisive visual difference, **delegated the visual choice to the Worker**, and deferred feel validation to hands-on play in CITY-07.

## How the delegation is applied

- The visual choices below are **reversible defaults** selected by the Worker. They are not claims of final art direction. ART-01 Phase B (effective candidate on the H2F-GATE baseline) and CITY-07 hands-on play remain the owner/art-direction checkpoints that can overturn them without a new foundation dependency.

| Spike | Worker's visual choice | Reason read from the captures |
|---|---|---|
| S01 | URP route C: ART palette plus Source normal maps (route A as fallback) | Keeps the wet, desaturated palette. Normals add stone and cobble relief. Route B is off-palette and broken on derived meshes. |
| S03 | Project-owned water shader | The ART flat water is a light off-palette blue. The shader reads as a dark wet river with bank foam. |
| S04 | Deterministic prefab scatter at ART scale bands | Bounded density and ART proportions. The unconstrained first pass was visually noisy. |
| S05 | Interior-mapping material on non-enterable windows (V3), flat glass (V1) acceptable | Depth cue at single-material cost. V2 costs geometry per window and showed seams. |
| S06 | Native composition (minimal) as the baseline. GC2 is technically fine, and its feel is arguably smoother. | This row is **not** a visual-only decision. See the authority reasoning in `DISPOSITION_MATRIX.csv`. The camera interior framing of the minimal route is a tuning residual. |
| S07 | UAL1 baseline + UAL2 fillers on the explicitly mapped citizen | Poses read correctly at human scale once `Hips=pelvis`. |

- **GC2 Core:** the owner routed the Core-wide question to a separate owner-proposed WP-H2F-01A (PR #255, stacked on the sequencing amendment). H2F-01 therefore records only the S06 role result and a non-structural disposition for Core (see `DISPOSITION_MATRIX.csv`); it neither adopts nor structurally rejects Core.
