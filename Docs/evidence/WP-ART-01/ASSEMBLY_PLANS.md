# ART-01 representative assembly plans (STRUCTURAL checkpoint)

These plans use the accepted **width classes**: X1 5.5 m, W12 2.8 m, casco micro-route B 2.4 m, and the accepted F01 planning frontage (8 m used, within the 8 × 15 m class).

The ART specimen's station coordinates are local test coordinates. They are not a replacement CITY layout, route grade, elevation decision or final keeper placement; CITY-07 must place and grade this vocabulary against the accepted seed.

Metric conventions:
- one Unity unit is one metre;
- massing snap is 0.25 m and contact trim 0.05 m, unless a measured Quaternius dimension forces a recorded value;
- every value below is enforced by `Art01StructuralAudit` (GREEN, digest `a7f8534f…`).

Kit families and assemblies are the `kitId`s in `KIT_COMPOSITION_MANIFEST.json` (`@2`). Materials named here are **family intent only** and provisional until the URP CANDIDATE checkpoint.

## Building: two-storey families (`house.two_storey_6x10.v1`, `house.bar_8x14.v1`)

| Layer | Retained assembly rule | Measured / enforced |
|---|---|---|
| Site/datum | Local ground datum −0.05 within 1.5 m of the footprint, blended to valley relief beyond. No world-height guess is exported. | Every plinth foot embedded; ground-contact checks GREEN |
| Footprint/base | Continuous plinth ring from −0.15 to the storey datum, from the wall inner face to 0.10 beyond the wall outer face. Private doors: the plinth top **is** the sill. Public threshold: the ring is interrupted exactly by the landing width. | Visible plinth 0.165–0.35 m; no unsupported host base |
| Storey datum | Private house 0.18 (one 0.17 m rise from the street edge); public room 0.30 (two 0.123 m risers). | `facade_base_supported` 70/70 |
| Massing/storeys | Two 3.122689 m storeys: stone source walls below, ART render hosts above. Render hosts share the source wall faces (outer +0.0924, inner −0.3141 from the module line), so storeys are flush. | `upper_storey_stacked_flush` 70/70; wall depth 0.406 |
| Corners | Stone storey: `Corner_Exterior_Brick` quoin, symmetric within 0.06 m. Every storey: `facade.corner_closure.v1` fills the 0.0924 × 0.0924 notch where two module lines meet. | 96 diagonal corner probes ≤ 0.07 m |
| Facade hosts | 2.00 m modules; plain hosts closed; opening hosts carry one real through-opening. A plain host never receives an insert. | 107 plain hosts closed; 33 single rectangular openings |
| Openings/inserts | Stone window: 1.20 × 1.275 opening, 1.05 sill, frame overlapping the stone reveal. Render window: 1.61 × 1.59 aperture sized to the `Window_Wide_Flat1` insert (declared exception). Door frame: 1.14 × 2.16 clear (declared exception). Private leaf recessed 0.10 m behind the frame (reveal 0.18). Public leaf swings **inward** against the jamb. | Inserts centred on real openings; within facade ends; no inter-host clash |
| Shutters | Open leaves (±1.24 m) only when both same-storey neighbours are plain and the module is not a facade end. Constrained windows get the closed source shutters, or none on the bar ground floor. | No overhang, no clash |
| Roof/top closure | Named low-pitch derivatives (`0.86/0.45/0.94` for 6×10, `0.91/0.45/0.96` for 8×14) placed so the **measured** underside meets the outer wall-top line. Pitch 34° (declared band 28–36°); eave projection 0.44–0.46 m. | Underside gap 0.000; 16 envelope probes GREEN |
| Gables | `gable.stone.v1` flush with the top-storey wall faces. Rise fitted to the measured roof underside (+0.03), checked to stay under the roof top surface. | Gable offset 0.000; no ridge/rake slit inside the envelope |
| Eave plate | Timber plate on the eave-side outer wall face; top meets the measured underside at its outer edge. | Attached |
| Public threshold (F01) | Landing 1.70 × 0.96 (from the wall face) at 0.30, continuing level into the public floor. Step 1.70 × 0.30 at 0.1775; both embedded into the street. Lean-to canopy (3.60 × 1.65, back edge embedded in the facade under the first floor) on two posts at ±1.65 m and 1.40 m setback, stone feet embedded. Fascia caps the edge. | Risers 0.123/0.123; landing 0.94; body clearance clear; porch head 2.655; posts touch canopy |
| Room context (F01) | Public floor at 0.30 and ceiling (clear 2.92). Counter on the right wall, table and chair on the left; all supported, no intersections. No schedules, shop semantics or NPC behavior. | Pivots/support GREEN |
| Attachments | Flush sign on the plain ground-storey module, back behind the measured stone surface, lettering readable from the street. Wall lantern on the measured pier beside the public door. | Attached; readable |
| Placement | Plinth outer face meets the kerb outer edge (houses at \|x\| = 6.80). F01's public door module is centred on micro-route B (local +1 m specimen offset). | Route clear-width check GREEN |

## Street: X1 → S02 → W12 → Casco/micro B → F01 specimen

| Layer | Retained assembly rule | Collision/connection evidence |
|---|---|---|
| Terrain/support | Non-collidable scenic ground: flat −0.05 datum beside the route, buildings, walls and river edge; valley relief beyond. Never a CITY heightfield. | `art01.site.*.scenic`, no collider |
| Bridgehead | `bridge.arch_span.v1`: 6.6 m wide deck body with one segmental arch (7.9 m span, 1.5 m rise, crown underside −0.70) and flush spandrels. `bridge.parapet_capped.v1` parapets: inner face on the road edge, ends meeting the S02 kerb start. `retaining.embankment.v1`: capped river embankment. Visible water at −2.2. The south end is a declared specimen cut. | Deck has **no** traversal collider; street is the sole owner |
| Traversable surface | One continuous crowned mesh (crown +0.045) for widths 5.5 → 2.8 → 6.0 local reveal pocket → 2.4. It terminates on the F01 plinth face. The 6.0 m pocket is an ART composition specimen, **not** an accepted CITY widening. | One `MeshCollider`; 0/1215 samples unsupported or stacked |
| Drainage | Right-side channel modelled **inside** the street mesh (0.20 m wide, 0.03 m deep, own submesh). | Visible; walkable |
| Edge/kerb | Continuous mitred kerb **outside** the traversable edge (0.20 wide, top 0.13, foot −0.15). It starts at the parapet ends and ends on the F01 plinth. | Rise 0.12, width 0.205–0.22 |
| Retaining/building contact | Capped low walls (body 0.42 × to 0.48, cap 0.53 × 0.09, foot −0.15). The W12/Casco edge continues the kerb outer line from the W12 right house wall; the F01 garden walls meet the frontage plinth line. | Visible height 0.62–0.70 |
| Threshold transition | Street → step → landing → public floor (see building table). The step and landing sit on the street surface; there is no coplanar duplicate ownership. | Stacked-surface check GREEN |
| Dressing/nature | Quaternius Nature/Props with centimetre normalization; Props keep root ×100 and X≈270°. Placed on the measured ground/floor height, clear of route and structure. | Pivots −0.10…0.00; `dressing_clear_of_structure` 21/21 |

## Authoring states and negative examples

Use `KEEPER_READY` only when every demanded host, support, cap, connection, dimension and source is present. `PROXY_VISUAL` marks a labelled test blockout; `COVERAGE_BLOCKED` marks a needed piece or connection that is absent. Neither diagnostic state can satisfy an ART-01 required benchmark row.

The structural audit now makes the following machine-detectable failures, not just prose:
- a big box standing in for a facade/roof/building;
- a floating roof or eave slit;
- a window on an uncut host;
- a mirrored or floating sign;
- a buried "drain";
- invisible scenic water;
- two collidable coplanar road slabs;
- a plinth gap under a door;
- a prop inside a building.

The ART tags are navigation aids, not proof by themselves: the audit measures geometry.

## Target reconciliation

The three frozen targets in `VISUAL_TARGETS.md` remain illustration, not CITY geometry.

**Structural relations now realized:**
- a bridge deck, parapet and bridgehead as one traversable system, with a visible arch and water edge (T1);
- a singular 2.8 m W12 surface with authored drainage and stone lower walls behind plinths (T2);
- an F01 separate plinth, thick hosted facade, true public opening with reveal, continuous threshold/landing into the public floor, and a supported canopy (T3).

**Deferred to URP CANDIDATE:** dark wet tile, restrained render tone, damp greens, overcast fog and the overall mood. The current built-in materials are provisional.

**Density:** the local specimen has less built density and landscape depth than the targets, because ART-01 only proves reusable first-chain vocabulary; CITY-07 owns district composition, ART-02 later polish.
