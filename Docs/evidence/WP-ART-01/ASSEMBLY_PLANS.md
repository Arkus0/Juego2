# ART-01 representative assembly plans

These plans use the accepted X1 5.5 m, W12 2.8 m and casco micro-route B 2.4 m **width classes** and the accepted F01 8 × 15 m planning frontage. The ART specimen's station coordinates are local test coordinates, not a replacement CITY layout, route grade, elevation decision or final keeper placement. CITY-07 must place and grade this vocabulary against the accepted seed. One Unity unit is one metre; massing snap is 0.25 m and contact trim is 0.05 m unless an audited Quaternius pivot forces a recorded correction.

## Building: F01 exterior/public threshold specimen

| Layer | Retained assembly rule | Instance/metric proof |
|---|---|---|
| Site/datum | Ground-floor support is local `y=0`; no world-height assumption is exported. | `Art01Benchmark.unity`, F01 specimen local pivot. |
| Footprint/base | 8 × 14 m within the accepted F01 planning class; 0.30 m stone plinth with a **real doorway interruption**. | `art01.plinth.F01*`; facade modules start at `y=0.30`. |
| Massing/storeys | Two 3.122689 m audited Quaternius module storeys. Lower stone and upper lime render; gable-to-approach silhouette. | `art01.assembly.F01_bar_exterior_threshold`. |
| Facade hosts | Repeat source 2.00 m stone hosts, or ART-owned 2.00 × 3.122689 × 0.44 m render hosts with actual extruded door/window apertures. A plain host never receives a pasted insert. | `art01.derived.render_host.*`; source wall bounds in `KIT_COMPOSITION_MANIFEST.json`. |
| Openings/inserts | Window inserts, shutters, and stone entry frame share host pivot/facing. Render-host window aperture is 1.61 × 1.59 m, sill 0.94 m. This is a reviewed **source-insert exception** to the 1.40 m typical ART opening band. Door uses 1.61 × 2.39 m rough opening; source leaf's 1.14 × 2.09 m visible size remains within the door scale target. | `Wall()` and `RenderHostMesh()`; neutral captures reveal wall depth and open doorway. |
| Roof/top closure | Named low-pitch derivative from measured RoundTiles 8×14 source uses scale `(0.91, 0.45, 0.96)` **only for this family**. Eave `y` derives from `0.30 + 2 × 3.122689`; gable fills the measured rise and penetrates 0.25 m into roof underside. Timber eave meets wall. | `art01.derived.roof.lowpitch.8x14.v1`; `art01.derived.gable.stone.v1`. Source steep roof is `SOURCE_ONLY`. |
| Public threshold | Porch roof is a pitched 3.60 × 1.65 m authored mesh supported by two 2.65 m posts, with feet and fascia. Street ends at the first step at local station 23.84; 0.15 m step, 0.30 m landing/floor, open leaf and interior floor meet at the doorway. | `art01.f01.*threshold*`, `*porch*`, `*roof.threshold*`; road/threshold collider ownership is disjoint. |
| Room context | Full-depth floor, ceiling at first-floor datum, table, stool, chair and a slatted counter with mug establish scale and public use; no schedules, shop semantics or NPC behavior. | `dressed_f01_interior_scale.png`. |
| Contact/dressing | Plinth meets scenic ground; portal and kerb terminate at the step; low capped garden wall, restrained wet-valley vegetation, lantern, barrel, crate and two-sided sign follow hosts rather than hide a seam. | Dressed and neutral F01 captures. |

For the 6 × 10 m S02/W12 houses, the same 2 m wall/3.122689 m storey rule applies. The paired source roof derivative is `(0.86, 0.45, 0.94)` and has a separately measured gable. Window distribution is deliberately asymmetric; upper corner quoins are omitted where smooth render itself closes the junction. This is a reusable family demonstration, not a universal urban generator. No Bar/Casco special from #233 was copied into the house grammar.

## Street: X1/S02 → W12 → Casco/micro B → F01 specimen

| Layer | Retained assembly rule | Collision/connection evidence |
|---|---|---|
| Terrain/support | ART-owned irregular **scenic** bank meshes flank the route and remain below its crown at the contact; no independent traversal surface or CITY heightfield claim. | `art01.site.*.scenic`, no collider. |
| Traversable surface | One continuous crowned cobble mesh for width-class transitions 5.5 → 2.8 → 6.0 local reveal pocket → 2.4 m. The 6.0 m pocket is an ART composition specimen and is **not** an accepted CITY widening. | `art01.street.s02_w12_casco_microB`, one `MeshCollider`. |
| Surface treatment | Quaternius round-rock albedo is remapped to restrained wet grey; UV repeats in metres. It is a real road mesh, not a thin plane layered over a second traversable slab. | `art01.derived.street.crowned.v1`, `Art01Materials.Cobble`. |
| Edge/kerb/drainage | 0.18 × 0.13 m stone kerb follows each measured station; the right 0.12 m dark groove is visual drainage beyond the sole road collider. | `art01.kerb.*`, `art01.drain.*`. |
| Retaining/building contact | Parapet/bank sample at the S02 side, capped low garden wall at F01, stone plinth at facades. No wall or plant is used as an unexplained seam mask. | `art01.bridgehead.*`, `art01.f01.garden.*`. |
| Threshold transition | Road collider ends where F01's first step begins. Step and landing provide the sole next support, then meet the full-depth interior floor at the doorway plane. The final CITY access/elevation check remains CITY-07's job. | No duplicate coplanar road/landing ownership. |
| Dressing/nature | Quaternius Nature and Props are explicitly imported at centimetre normalization; Props retain root ×100 and X≈270° orientation. Roots/feet are positioned on bank/floor and outside clear path. | Material/source checks plus third-person captures. |

## Authoring states and negative examples

Use `KEEPER_READY` only when every demanded host, support, cap, connection, dimension and source is present. Use `PROXY_VISUAL` for a labeled test blockout and `COVERAGE_BLOCKED` if the needed piece/connection is absent. Neither diagnostic state can satisfy an ART-01 required benchmark row. In particular, a big solid cube carrying a window/frame/roof, a floating roof, a painted window on an uncut facade, a sign with a mirrored back, a decorative grass strip hiding a road gap, or two collidable coplanar road slabs is not keeper-ready. The scene's ART tags are navigation aids, not proof by themselves; inspect geometry and captures.

## Target reconciliation

The three frozen targets in `VISUAL_TARGETS.md` remain illustration, not CITY geometry. The benchmark carries the stone/render split, darkened low-pitch tile, deep shuttered openings, a supported entry canopy, explicit plinth, wet grey cobble, local retaining/kerb/drain, overcast light and clothed human. Its local specimen has less built density and landscape depth than the targets because ART-01 only proves reusable first-chain vocabulary; CITY-07 owns the actual district composition and ART-02 later polish. The current source tree reads somewhat pale under overcast lighting; that is a bounded art-direction residual, not a missing required structural connection.
