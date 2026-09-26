# ART-01 fresh-author composition smoke test

> **Status: HISTORICAL / PROVISIONAL (built-in renderer, geometry of `290c487`).** This run is retained as evidence of an earlier iteration. It does **not** satisfy the ART-01 smoke-test deliverable for the candidate:
>
> - the STRUCTURAL checkpoint then corrected the geometry it judged (see `STRUCTURAL_CHECKPOINT.md`: the baseline audit of that geometry had 569 failures, including elements this report classified `KEEPER_READY`);
> - the owner decided the production renderer is URP.
>
> The smoke test is re-executed by a fresh author against the STRUCTURAL kit/manifest under the final URP pipeline, before CANDIDATE. The captures it cites live in `captures/` at `290c487` and are superseded.

2026-09-26. Role: fresh environment author, not the kit creator or independent Reviewer. Scope: bounded ART specimen in `C:\Juego2-ART01` / `codex/wp-art-01`. No CITY/H0/H1 identity, gameplay, NPC system, PR #233, commit, push or review action was changed.

## Inputs consumed before editing

- `Docs/workpacks/ART/WP-ART-01.md` and its binding `ART_01_ASSEMBLY_GRAMMAR_AMENDMENT.md`, `ART_ENVIRONMENT_ASSEMBLY_GRAMMAR.md`, `ART_01_DIMENSIONAL_PROFILE.md`.
- `Docs/evidence/WP-ART-01/KIT_COMPOSITION_MANIFEST.json`, `ASSEMBLY_PLANS.md`, `VISUAL_TARGETS.md` and `DERIVATIVE_AND_MATERIAL_LEDGER.md`.
- The existing `Art01BenchmarkBuilder.cs`, effective Unity scene/audit, normal and neutral W12/Casco captures. I used the accepted specimen dimensions and admitted kit metadata, not PR #233 geometry or code.

## Independent authored revision

I added `W12CascoEdge()` to `Unity/ArkusUnity/Assets/Arkus/ART/Editor/Art01BenchmarkBuilder.cs`, called after the two W12 house assemblies. It adds two tagged instances of the already admitted `art01.derived.retaining.capped.v1` family: `art01.w12_casco.right.retaining` and `art01.w12_casco.right.coping`. This changes the right W12 house / Casco reveal junction in the effective scene; it is a low stone edge, not a box carrying facade decoration or a prop concealing a gap.

The wall spans local specimen `z=1.95..6.75`, from the right W12 house end (`z=2`) to just before the road's Casco widening begins (`z=7`). Its centre is `x=1.72`; body is `0.42 W × 0.56 H × 4.80 L`, from `y=-0.08` scenic-bank datum to `y=0.48`. The `0.53 W × 0.09 H` coping begins exactly at `y=0.48`. The body inner face `x=1.51` stays outside the W12 2.8 m road edge `x=1.40` and its kerb outer edge `x≈1.49`. At `z=1.95..2.00` it meets the stone plinth/corner of `W12_right_house`. The wall alone has a static local collider; it creates no second traversable road surface.

Connections: `SUPPORTED_BY` scenic bank; `MEETS` W12 right plinth and street shoulder; `CLEAR_OF` the 2.8 m route; coping `CAPS` retaining body. Material family is admitted ART stone/trim, with no new source asset. The specimen stations are local test coordinates and do not declare a CITY-07 route, grade or parcel.

I also added close normal/neutral captures of the wall-to-plinth junction, normal/neutral captures toward the Casco reveal, and one third-person edge capture. These expose the structure without relying on dressing to hide it.

## Required benchmark needs and state

| Need exercised in the retained-chain specimen | State after revision | Concrete basis |
|---|---|---|
| S02 → W12 → Casco/micro B street surface and 2.8 m W12 width | `KEEPER_READY` | Existing single crowned `art01.street.s02_w12_casco_microB` mesh/collider; Unity audit reports exactly one street `MeshCollider` and W12 support on that surface. |
| W12 building exteriors, corners, true hosted door/window openings and supported 6 × 10 m roof/gable family | `KEEPER_READY` | Existing `W12_left_house`/`W12_right_house` use admitted 2 m source/render hosts, source inserts in cut apertures, plinths, corner modules and named `lowpitch.6x10` roof/gable assembly. Original and close neutral captures show depth and corner massing. |
| W12 right building-to-bank/street junction and Casco compression/reveal edge | `KEEPER_READY` | New capped retaining body meets the right plinth at its exposed end; foot reaches bank datum, clear of sole route. Normal and neutral close captures show the contact; third-person edge capture shows human clearance. |
| F01 exterior, public opening, step/landing/interior transition | `KEEPER_READY` | Existing admitted 8 × 14 m family, cut public door, supported canopy, real step/landing/full-depth floor. Audit hits first step at `0.15 m`, landing and interior at `0.30 m`; F01 normal/neutral captures remain available. |
| Street kerb/drain and ground/building contact | `KEEPER_READY` | Existing named road-edge/drain system and F01 plinth/threshold; new W12 wall stays beyond the kerb without claiming traversal. |
| Bounded nature, props and clothed human scale | `KEEPER_READY` for this specimen | Existing locked Quaternius Nature/Props and ART clothed 1.8 m scale reference remain; audit counts three clothed references. The new edge uses neither plants nor props as a seam mask. |

No required benchmark need was classified `PROXY_VISUAL` or `COVERAGE_BLOCKED` in the rebuilt scene. These states are based on inspected geometry/captures and the effective audit, not merely the `Art01Piece` tags. This is author smoke-test evidence; an independent Reviewer still judges the product acceptance claim.

## Negative composition decisions from the manifest

1. `PROXY_VISUAL` diagnostic, **not placed in the final scene**: put `Window_Wide_Flat1` onto an uncut `Wall_UnevenBrick_Straight` or solid cuboid and decorate it with shutters. The manifest admits the window only in a matching 2 m host with a real opening; the apparent window would be a sticker. It cannot satisfy the W12 facade need.
2. `COVERAGE_BLOCKED` diagnostic, **not demanded or placed in the final scene**: require a perpendicular intersecting hip/valley roof joining the W12 6 × 10 m gable roof to a new wing. The admitted roof family is the named low-pitch gable assembly, with measured gable/eave support; the manifest supplies no hip/valley junction piece or connection rule. A fresh author must stop that additional demand rather than stretch a source roof, overlap two caps or hide the join with a chimney. It is outside this bounded benchmark and does not change its final states.

## Effective Unity evidence and limitation

- Unity `6000.3.24f1` builder: `ART01_BENCHMARK_BUILD_GREEN`, one road collision owner, 22 dressing instances. Rebuilt scene: `Unity/ArkusUnity/Assets/Arkus/ART/Art01Benchmark.unity`, SHA-256 `4DCA35832744A56B9CD264932535CD1B58F5AD01D8EE6DB8E84C1CDA72632110`.
- GPU-rendered captures: `captures/dressed_w12_casco_joint.png` (SHA-256 `A655B7C691E497FED5670A70DBBF2F4B23832F6DEB7922BDA56734488AC7FD64`), `captures/neutral_w12_casco_joint.png` (`758B919A8B679B07D811318C1BA2A6909737DC7C5D6CD62D170BA94062B027FC`), `captures/dressed_w12_casco_edge.png`, `captures/neutral_w12_casco_edge.png`, `captures/third_person_w12_casco_edge.png` (`3AA3B5515C5B71E46BB21364C6BA0471B4B99887AB085CB3C88B9FCA371F3315`). The original W12/Casco and F01 views were also recaptured.
- `ART01_DRESSED_CAPTURE_GREEN`, `ART01_NEUTRAL_CAPTURE_GREEN`, `ART01_THIRD_PERSON_CAPTURE_GREEN` (four viewpoints) and `ART01_BENCHMARK_AUDIT GREEN pieces=353 issues=0`; `UNITY_BENCHMARK_AUDIT.json` records one street collider, three clothed humans, correct support hits and no violations.
- The first capture attempt used Unity `-nographics`; Unity crashed in `Camera.Render`, so those invocations produced no valid new capture. I repeated them without `-nographics`, waited for process exits `0`, and checked the regenerated image timestamps and green markers. The new close views also expose a broad, pale scenic bank beyond the route. It is existing non-CITY scenic context, visually sparse compared with the concept paintover; this smoke test does not claim final town landscape density or CITY-07 terrain placement.

Touched by this smoke test: `Art01BenchmarkBuilder.cs`, regenerated `Art01Benchmark.unity`, Unity's `UNITY_BENCHMARK_AUDIT.json`, regenerated files under `captures/`, and this report. No external source, material, manifest, assembly plan, or other Worker-owned file was edited by this fresh author.

Local untracked `art01-fresh-*.log` diagnostics remain at the worktree root. A cleanup command was rejected by automatic command review, so I left those logs untouched rather than claim they were removed.
