# ART-01 STRUCTURAL checkpoint — renderer-independent

2026-09-26. Worker: Claude (received by owner-directed transfer from Codex at `290c487`). Unity `6000.3.24f1`, `Unity/ArkusUnity`, **render pipeline unchanged (built-in)**.

This is checkpoint 2 of `ART_01_ASSEMBLY_GRAMMAR_AMENDMENT.md` ("massing checkpoint"), made renderer-independent at the owner's direction. It freezes geometry, kit identity, dimensional/host/connection metadata and the building/street assembly plans. It does **not** freeze materials, lighting, atmosphere, dressed captures or the final visual judgement of the candidate. Those are produced under URP after `WP-H1-GATE` (see `WORKER_PLAN.md`).

## What "renderer-independent" means here

`Art01StructuralAudit` puts temporary audit-layer `MeshCollider`s on every `MeshFilter` and measures effective geometry with rays and overlaps. It never reads a material, shader, light, fog value or pixel. Two things change when the pipeline changes: the neutral captures below (rendered with one grey material) and `UNITY_BENCHMARK_AUDIT.json`'s material check. Both are re-run under URP. The geometry verdicts are not re-run.

## Evidence

| Artifact | SHA-256 | Meaning |
|---|---|---|
| `UNITY_STRUCTURAL_AUDIT.json` | `c36d31b51eb6e3e579014a5bcfa6d889b9e8c0f4adb8f6e41283b20a047d9584` | **GREEN: 2009 checks, 0 failures**; binds manifest `105e2fef…` and structural digest `a7f8534f…` |
| `STRUCTURAL_SCENE_DIGEST.json` | `815976c488bf32314c78de0413d65c32b3bf8acaf65ac6b4843e5ca9fbf77231` | Canonical 360-piece record (id, kit, role, state, collision, pose, scale, meshes). Digest `a7f8534f33a46a189c5afb726125b01b6ab0eeba5907cbc3ba4aaafb8e30601e`. A full rebuild reproduced the same digest; scene YAML fileIDs are not deterministic, this digest is. |
| `KIT_COMPOSITION_MANIFEST.json` | `105e2feff2d9fbe01269b7889722fc2aee837b2148c3d1dc17145fa271620992` | Schema `@2`: 55 pieces (45 KEEPER_READY), 4 assemblies, `unityAssets`, `formPrimitive`, numeric dimensions, declared exceptions, benchmark route segments |
| `UNITY_STRUCTURAL_AUDIT_NEGATIVE_W12_2p5.json` | `1b82596f27e3b746e9344d93c9e3304dedc05ac59dbb512560d13b006ba8fd10` | Defect-injection run: W12 temporarily narrowed to 2.5 m. **Only** `accepted_route_width_class` fails (2.500 vs accepted 2.800); every other check, including route clearance, stays green. Builder and assets were restored afterwards and the audit re-ran GREEN on digest `a7f8534f…`. |
| `UNITY_STRUCTURAL_AUDIT_BASELINE_290c487.json` | `3506538c17f20ba118504fb75305c63898eb15ab3aa10e665947199287a72793` | Same audit run on the geometry produced by Codex's committed builder at `290c487`: **569/1449 failures** (353 were missing kit IDs) |
| `UNITY_BENCHMARK_AUDIT.json` | regenerated | `@2`, positions read from tagged pieces (no hard-coded coordinates); GREEN; `renderPipeline: built-in` |
| `structural_checkpoint/neutral_*.png` | below | 11 neutral-material human-scale views of digest `a7f8534f…` |

Reproduction (from the repo root, with the owner's read-only `C:\Juego2-Assets`):

```text
python Tools/art01_import_sources.py import && python Tools/art01_import_sources.py verify
python Tools/art01_build_manifest.py
Unity -batchmode -quit -projectPath Unity/ArkusUnity -executeMethod Juego2.ART.Editor.Art01BenchmarkBuilder.Build
Unity -batchmode -quit -projectPath Unity/ArkusUnity -executeMethod Juego2.ART.Editor.Art01StructuralAudit.Run
Unity -batchmode -quit -projectPath Unity/ArkusUnity -executeMethod Juego2.ART.Editor.Art01BenchmarkAudit.Run
Unity -batchmode -quit -projectPath Unity/ArkusUnity -executeMethod Juego2.ART.Editor.Art01BenchmarkBuilder.CaptureNeutral
```

All exited 0 without `-nographics`. With `-nographics`, the Build method sometimes segfaults after `Batchmode quit successfully invoked`, during editor shutdown without network. By then the scene is already saved, and the subsequent audit re-opens and verifies it. Evidence runs use the graphics path.

## Audit scope (what 2009 checks cover)

- **Identity/provenance (890):** every piece's `kitId` resolves to a KEEPER_READY manifest entry or assembly. Every source-model instance matches its kit's `sourceLockDest`/`unityAssets`. SOURCE_ONLY sources (roofs) are consumed only through a named derivative. Every generated mesh is listed by its kit entry. Every BOX-form piece is declared BOX, is not a facade/roof/building, and has one dimension ≤ 0.60 m (the no-decorated-cuboid guard). Every renderer and collider is owned by a kit piece.
- **Openings/inserts (287):** plain hosts are closed. Opening hosts have one real rectangular through-opening. Inserts are centred on a real opening. No insert projects past a facade end, and no two hosts' inserts clash.
- **Dimensions (321):** measured against `ART_01_DIMENSIONAL_PROFILE.md`, with explicit exceptions only where a source insert forces them.
- **Envelope (112):** horizontal probes over every facade (≈23–33k per side) and diagonal corner probes. They detect eave slits, jamb voids, module gaps and re-entrant corner notches. A leak is only counted inside the envelope (below the roof-skin underside).
- **Roof (16):** underside meets the outer wall-top line, eave projection is in band, gables are flush with the wall face, pitch is within the declared band.
- **Contact/attachment (368):** base of every ground-storey host on plinth/threshold/floor. Upper storeys stacked flush. Box feet embedded. Kerb feet embedded. Dressing/nature/human pivots on a real support. Attached pieces touch their host. Sign readable from its mounted side. Dressing/nature clear of structure.
- **Street/surface (10) and public threshold (5):** one traversable collider; street support on every route sample; no stacked traversable surfaces; route width clear 0.20–2.10 m; **measured traversable width equals the predecessor-accepted CITY-04 width class** — X1 5.5, W12 2.8, casco.micro.B 2.4, read from `City04Layout.json`, never from the mesh or builder (−0.01/+0.02 m); no coplanar overlapping top surfaces (34,496 samples); visible water; modelled drainage channel.

## Measured result (selected)

| Measure | Profile band | Measured |
|---|---:|---:|
| Floor-to-floor (all storeys; F01 social ground storey) | 2.8–3.4 / 3.0–3.6 | 3.123 |
| Wall depth at openings/returns (140 hosts) | 0.25–0.50 | 0.406 |
| Stone window through-opening / sill | 0.65–1.40 × 0.80–1.55 / 0.70–1.10 | 1.20 × 1.275 / 1.05 |
| Render window opening | declared source-insert exception ≤1.62 × ≤1.60 | 1.60 × 1.575 |
| Door clear width (public + 3 private) | 0.85–1.10 / 0.75–1.00; declared exception ≤1.16 | 1.14 |
| Door clear height | 1.95–2.20 | 2.16 |
| Private door reveal | 0.10–0.40 | 0.182 |
| Opening separation | ≥ 0.25 | 0.75–2.40 |
| Eave projection from wall face | 0.25–0.65 | 0.436–0.455 |
| Roof pitch (declared low-moderate band) | 28–36° | 34.0–34.3° |
| Roof underside gap at wall top / gable offset | ≤ 0.05 | 0.000 / 0.000 |
| Plinth visible above street/ground | 0.15–0.60 | 0.165–0.350 |
| Public approach risers | 0.12–0.18 each | 0.123, 0.123 |
| Landing depth (riser → door face) | 0.90–1.50 | 0.94 |
| Landing → public floor continuity | ≤ 0.02 | 0.000 |
| Public room clear height | 2.4–3.0 | 2.923 |
| Porch clear head height | 2.1–2.8 | 2.655 |
| Kerb rise above road edge / visible width | 0.08–0.16 / 0.12–0.30 | 0.119–0.120 / 0.205–0.220 |
| Low retaining/garden wall visible height | 0.45–1.20 | 0.62–0.70 |
| Accepted route width (X1 / W12 / micro-route B, from CITY-04) | 5.5 / 2.8 / 2.4 (−0.01/+0.02) | 5.500 / 2.800 / 2.400 |
| Clothed human reference | 1.70–1.85 | 1.814 |
| Dressing/nature pivot support gap | −0.12…+0.02 | −0.10…0.00 |

## Defects found at `290c487` and resolved here

Each line below is a measured baseline failure. Each one is resolved in the current audit.

1. **Accepted route width narrowed.** W12/S02 facades, plinths and kerbs intruded up to 0.34 m into the 2.8 m lane. Houses now sit so their plinth outer face meets the kerb outer edge (|x| = 6.80). Kerbs are outside the traversable edge.
2. **Floating/void base.** Door modules had no plinth under their 2 m width (20/20 unsupported samples; you could see under the house at the door). The public threshold left jamb voids. The plinth is now a continuous ring from −0.15 to the storey datum: the private door sill is the plinth top, and the public gap equals the landing width.
3. **Ground contact.** Plinths, kerbs, garden walls and post feet floated up to 0.09 m over an irregular bank; the bench floated 0.063 m. The scenic ground is now a flat −0.05 datum within 1.5 m of every retained feature, blended to valley relief beyond. Foundations are embedded, and nature/props are placed on the measured ground height.
4. **Roof/wall slit and gables.** The eave underside was 0.2 m above the wall top at the gable ends, and gables sat 0.17 m behind the wall face. The roof is now placed by its measured underside at the outer wall-top line. Gables are fitted to the measured underside and flush with the wall. Eave plates touch wall and roof.
5. **Corner notches.** The upper render storey stood proud of the stone storey (0.13 m jetty) and left 0.22 m notches; the stone storey left a 0.10 m notch above each 3.02 m quoin. Render hosts now share the source wall faces, and a measured `corner_closure` fills the WALL_OUT notch.
6. **Shutter overhang/clash.** Open leaves (±1.24 m) overhung facade ends by 0.24 m and clashed with neighbouring frames. Open leaves are now admitted only with plain neighbours on both sides. Constrained windows use the closed source shutters (now admitted) or, on the bar ground floor, none.
7. **Dead or false elements.** The drain boxes were buried under the road (0/8 visible samples): the channel is now modelled in the street mesh (0.038 m deep, second submesh). The river water was fully covered by the bank: now 1626/1680 samples are visible. The "bridgehead" had no bridge: it is now an arch span (7.9 m, rise 1.5 m) with flush spandrels, parapets meeting the road edge, and a capped river embankment.
8. **Duplicate traversal.** The arch deck would have stacked a second collider under the X1 road. The street is the sole traversable owner (0/1215 stacked samples).
9. **Public door.** The open leaf swung outward into the landing and blocked the 0.56 m body (it reached 0.9 m in front of the facade). It now swings inward against the jamb, and the body passes step, landing and doorway.
10. **Threshold numbers.** The landing was 0.86 m deep, and the road ended beside the step with a 0.8 m strip of bare bank. micro-route B now terminates on the F01 plinth face. The step (0.1775) and landing (0.30) give two 0.123 m risers and a 0.94 m landing. F01 is shifted +1 m locally so its public door is centred on the lane (a local specimen choice, not a CITY coordinate).
11. **Sign and lamp.** The board floated 0.23 m off the facade and both letterings were mirrored. It is now one flush board embedded behind the measured stone surface, with lettering readable from the street, on the plain module. The lamp is attached to the measured stone pier beside the door.
12. **Interior intersections.** The table ran through the counter, the chair sat inside the counter, and the counter carcass floated 0.05 m. Furniture is re-laid and supported.
13. **Misplaced dressing.** The S02 bench was inside the S02 house, and the W12 fern penetrated the W12 plinth. Both are moved to their stated hosts.
14. **Locale-dependent identity.** Wall logical IDs embedded the machine decimal separator (`…0,30` on this machine). IDs now use invariant formatting.
15. **Manifest drift.** The committed manifest did not match its own generator. It is regenerated, and the audit binds its SHA.

## Declared exceptions (not silent tolerances)

- `render_host.window_wide_flat.v1`: opening up to 1.62 × 1.60 m, because the Quaternius `Window_Wide_Flat1` insert is 1.61 × 1.58 m. The host is sized to the insert rather than stretching it.
- `doorframe_flat_brick`: clear width up to 1.16 m (measured 1.14) for public and private doors. The first slice has one source frame family; its height (2.16) is within band.
- Roof families: pitch band 28–36° declared on both `roof.lowpitch.*` entries (measured 34°).

## Neutral captures (digest `a7f8534f…`)

| View | SHA-256 | Structural read |
|---|---|---|
| `neutral_puente_s02.png` | `197325a18499e16a7d797f049cb33c7b9f95ef2f53f30667d32932437c82f226` | X1 5.5 m deck between parapets narrowing to W12 2.8 m; houses set back behind kerbs |
| `neutral_x1_arch_embankment.png` | `b211f07d960e0bab644e70a748d1a9e1ecbab2f19306f407cc6c2d28382a6703` | arch span, spandrel, parapet and capped embankment over visible water |
| `neutral_w12_casco.png` | `6ba217cfc59285ed3cb41ca7a1c612be1ef6f5ff51dfab9c8f7e7bdf9ec9db1d` | compression between set-back facades with plinths and kerbs; reveal toward F01 |
| `neutral_w12_private_door.png` | `58de0436227631a2b67888dfd17370e2545d85ed18875f00ec6f455fb3ee60c4` | framed private door with 0.18 m reveal on the plinth sill; closed shutters on the corner window |
| `neutral_w12_casco_joint.png` | `455968de2074dbacc1d8934fd19aec58c28da0c2ad1fd5d83123ad0b695856fc` | capped retaining edge meeting the W12 right house wall |
| `neutral_w12_casco_edge.png` | `77ac117d7bab5f0af07b16658ce601ff8abfd81cb04533db633cd5c6b6e49047` | retaining edge continuing the kerb line toward the Casco reveal |
| `neutral_roof_eave_corner.png` | `7572893923f4882fef91d613dfa6c513e6eb3c45b3e0d04b927ef35cb9d41a25` | closed eave line, timber plate, roof verge over flush gable |
| `neutral_f01_exterior.png` | `17b671cd5493af6eaa7afa725bfb1a606df0b07e4b96acf7d67e3f43f6de1892` | door centred on lane; porch posts outside kerbs; flush sign; open/closed shutters without clash |
| `neutral_f01_threshold.png` | `65e1ea59c71552538432218ac1537a2c038e5eb0cab38c00c20fba5e1b5c559a` | step, landing and open doorway into the public floor |
| `neutral_f01_threshold_side.png` | `cdfb29de4762d81490eb9b4ef566ff8eb062f7d4f5b345255598a3925bda6af1` | step -> landing -> floor; plinth ends at landing; post feet embedded |
| `neutral_f01_interior_scale.png` | `746d91ab9723bc6e3e5363b5b9e24c338f941d94fbd78d728c08b4799e9c2ec1` | public room, counter and furniture at human scale |

## Boundary

- No change to `Unity/ArkusUnity` ProjectSettings, Packages or render pipeline. All mutations are under `Assets/Arkus/ART`, ART evidence and ART tools, so there is no interference with H1-11/H1-GATE.
- No CITY route/place/access/elevation semantic changed. Specimen stations, F01's local +1 m offset, the 6.0 m Casco pocket and the bridgehead sample are ART composition specimens. CITY-07 places the vocabulary against the accepted seed.
- PR #233 was not modified or used as geometry.

## Pre-review hardening (review #5325775001)

The pre-evaluation of `f60ed87` accepted the STRUCTURAL checkpoint. It noted that route clearance was derived from the road mesh itself, so an accidental narrowing could stay "clear". `accepted_route_width_class` closes that gap: it compares widths measured by transverse rays on the sole traversable collider against the accepted CITY-04 widths, with the specimen segment map declared in the manifest benchmark assembly. The 6.0 m Casco reveal pocket is declared as an ART specimen and is not checked against CITY. The negative run above demonstrates that the check causally detects a narrowed W12 that every other oracle would miss.
