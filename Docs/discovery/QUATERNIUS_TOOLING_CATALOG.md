# Quaternius tooling catalog

Status: **DISCOVERY / NON-CANONICAL**  
Date: 2026-09-28  
Purpose: quick triage surface for tooling/techniques discovered around Quaternius adaptation and Juego2 visual-production industrialization.

Legend:

- `USE` — already-native/source capability that should be consumed when admitted.
- `ADAPT` — strong candidate to modify/wrap after a bounded spike and exact-version IP review.
- `REFERENCE` — useful architecture/technique; do not assume code adoption.
- `SPIKE` — needs a bounded compatibility/value proof before adoption/purchase.
- `WATCH` — interesting but not currently necessary.
- `REJECT` — no current reason to carry forward.

No row is dependency-adoption authority. `Docs/engineering/DEPENDENCY_IP_POLICY.md` governs actual adoption.

| Candidate | Area | Observed capability | Observed license/status | Disposition | Value | Primary WP | Key risk / note |
|---|---|---|---|---|---|---|---|
| Quaternius Universal Base Characters | Characters | Shared Humanoid base, multiple proportions, hair, rigged Source `.blend`, UAL compatibility | Pack/acquisition provenance controls; current official pages must be reconciled with exact acquired license | USE when admitted | VERY_HIGH | ART-CHAR-01 | Preserve exact source/acquisition record |
| Quaternius Modular Character Outfits - Fantasy | Wardrobe | Modular outfit parts, color variants, shared Humanoid ecosystem | Exact acquired pack provenance controls | USE / ADAPT / DONOR | VERY_HIGH | ART-CHAR-01 | Remove fantasy cues; clipping/fit still needs proof |
| Quaternius UAL | Animation | Large reusable humanoid animation vocabulary, root-motion/no-root-motion variants | Exact acquired pack provenance controls | USE when admitted | VERY_HIGH | ART-ANIM-01 | Do not infer useful coverage from raw clip count |
| Quaternius Medieval Village MegaKit Source | Environment | 300+ modular parts, grid assembly, URP project, wear shaders, custom collisions | Exact acquired pack provenance controls | USE / ADAPT / DONOR | VERY_HIGH | ART-01 / ART-ENV-02 | Pack theme is not final scene identity |
| Quaternius Downtown City MegaKit Source | Environment | 300+ modular city/street parts, fake interiors, vertex-color wear, bevel normals, collisions | Acquisition not authorized by this audit | WATCH / possible future USE | HIGH | ART-ENV-02 / ART-URBAN-01 | Do not buy solely because it exists |
| ChilyerStudiosLLC/blender-character-pipeline | Character tooling | Reproducible base/proportion/body-topology clothing/rig-verify/export workflow | MIT verified in repo LICENSE | ADAPT / REFERENCE | HIGH | ART-CHAR-01 | Workflow demo, not drop-in Juego2 factory |
| Tinqs clothing pipeline | Clothing tooling | Headless staged garment census/fit/reduce/bake/skin/export on Quaternius skeleton | Code license NOT VERIFIED | REFERENCE_ONLY | VERY_HIGH conceptual | ART-CHAR-01 | Do not copy code until license is resolved |
| benjamincanac/avelune | Character composition | Quaternius outfit+head composition, shared 65-bone animation asset, runtime colorway | MIT repo | REFERENCE / possible ADAPT | HIGH | ART-CHAR-01 / ART-ANIM-01 | Web runtime assumptions differ from Unity |
| NafisRayan/Animate-Rigged-Humanoid-No-Blender | Animation packaging | glTF-based character + UAL merge without Blender/manual retarget | License not adopted/verified here | REFERENCE / WATCH | MEDIUM | ART-ANIM-01 | Three.js-oriented; packaging pattern only |
| firstkindgamer/QuaterniusUnityUtils | Unity import | UAL import settings + Quaternius collision-prefab creation; Medieval Village support | Unlicense verified | ADAPT / SPIKE | HIGH | ART-ANIM-01 / ART-ENV-02 | Must pass Unity 6.3/H2F lifecycle/compatibility |
| codec-xyz/game_export | Blender→Unity | FBX+prefab export, static settings, material remap, colliders, instances | MIT metadata verified | REFERENCE / SPIKE | MEDIUM_HIGH | ART-ENV-02 | Direct Unity YAML/meta generation may be version-fragile |
| Auto-Building | Building assembly | Custom modular collections, boolean openings, facade/edge scattering, roofs, foundations, basic interior | Marketplace: GPL; paid; $20 observed | SPIKE only | VERY_HIGH potential | ART-ENV-02 | Product page warns Blender 5.2 incompatibility; no purchase before benchmark |
| Geo-Buildings | Building assembly | Geometry Nodes building generator, custom assets/materials, manual placement controls | Marketplace says Creative Commons; $11 observed | WATCH / alternative SPIKE | MEDIUM_HIGH | ART-ENV-02 | Exact purchased-version terms/Blender compatibility required |
| p-schulz/osm_building_grammar | Semantic building grammar | Deterministic facade/window/door/balcony/shop/roof/etc. roles; metadata, instancing, batching | Apache-2.0 metadata verified | REFERENCE / ADAPT architecture | HIGH | ART-ENV-02 | OSM semantics/style are not Juego2 authority |
| theanine3D/mat_batch_tools | Materials | Batch node/material changes and bake/UV preparation | GPL-3.0 metadata verified | EXTERNAL TOOL SPIKE | MEDIUM_HIGH | ART-ENV-02 | Prefer isolated DCC tool boundary under IP policy |

## Candidate ordering for future spikes

1. `QuaterniusUnityUtils` — cheapest way to test whether known Quaternius import/collision work is already solved on the accepted Unity baseline.
2. Chilyer character pipeline techniques — low-risk reference for `ART-CHAR-01` because the code license is clear and the workflow maps directly to our source family.
3. Tinqs garment architecture — highest clothing-process insight, but reference-only until exact code licensing is known.
4. `osm_building_grammar` semantic-role model — use as a vocabulary/metadata reference before inventing donor-component taxonomy.
5. Material Batch Tools — test as an isolated Blender helper if material normalization becomes repetitive.
6. Auto-Building vs Geo-Buildings — only when `ART-ENV-02` has an actual repeated-building brief and can measure whether purchase/tool complexity saves material work.
7. `game_export` — inspect/benchmark only after the H2F import/prefab contract is frozen; do not bypass accepted Unity authority.

## Explicit non-decisions

This catalog does not:

- authorize buying Source packs not already owned/admitted;
- authorize buying Auto-Building or Geo-Buildings;
- select Blender as canonical over any already accepted DCC boundary;
- vendor GPL code into Juego2;
- adopt direct Unity YAML generation;
- establish a 65-bone rig as product-semantic authority;
- waive H2F lifecycle proof;
- supersede ART visual-quality or provenance gates.
