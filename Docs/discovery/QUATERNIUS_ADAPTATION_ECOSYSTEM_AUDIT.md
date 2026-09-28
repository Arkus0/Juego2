# Quaternius adaptation ecosystem audit

Status: **DISCOVERY SNAPSHOT / NON-CANONICAL**  
Date: 2026-09-28  
Scope: external tooling, reference pipelines and production techniques that may reduce custom work in Juego2's Quaternius-based art lanes.  
Authority rule: this document is research input only. It does **not** adopt a dependency, buy a tool, change H2F, or override any accepted ART/Product contract.

## Executive summary

The external ecosystem materially reduces the risk of Juego2's planned Quaternius production factories.

The strongest finding is not a single magic converter. It is that most difficult stages already have public or commercial precedents:

- Quaternius Universal Base Characters, Modular Character Outfits and Universal Animation Library share a reusable humanoid character/animation ecosystem;
- reproducible Blender/Python pipelines exist for proportion editing, body-topology clothing, garment fitting, weight transfer and export;
- Unity-specific Quaternius utilities already automate Universal Animation Library import settings and Medieval Village collision-prefab construction;
- Blender tools exist for batch material editing, rule-based/procedural building composition and Blender-to-Unity prefab/export setup;
- public building-grammar projects demonstrate useful semantic roles such as facade/window/door/shop/awning/balcony/shutter/chimney/gutter/roof/signboard/loading-dock rather than relying on source asset names.

The recommended project response is therefore **not** to invent one monolithic proprietary asset factory up front. The ART owners should evaluate these techniques/tools in bounded spikes, retain Juego2-owned composition/quality/provenance contracts, and implement only the missing glue.

Current confidence by lane:

| Lane | Audit result |
|---|---|
| Character base / rig | HIGH confidence reuse path exists |
| Wardrobe adaptation | HIGH confidence; multiple concrete precedents |
| Animation sharing / intake | HIGH confidence |
| Quaternius→Unity import helpers | MEDIUM-HIGH confidence |
| Batch material normalization | HIGH confidence tooling exists |
| Environment donor/component assembly | MEDIUM-HIGH confidence; several candidate mechanisms |
| Full Juego2 environment factory | NOT found as an off-the-shelf solution; still project-owned |
| Northern-Spain visual identity | intentionally project-owned; no external tool is authority |

## Binding boundaries

All actual adoption remains subject to:

- `Docs/engineering/DEPENDENCY_IP_POLICY.md`;
- accepted source/provenance contracts;
- exact adopted version/commit and license re-verification;
- the existing ART reuse order (`DIRECT -> ADAPTABLE -> DONOR -> CREATE_DERIVED -> REJECT/BLOCKED_EXTERNAL`);
- H2F's accepted Unity/renderer/toolchain baseline.

A research README or marketplace page is insufficient dependency-adoption evidence. Copyleft Blender tools should normally remain isolated external authoring tools unless an explicit reviewed exception says otherwise. External generators are mechanisms, never semantic authority for Juego2 visual identity, canonical asset identity, provenance or keeper acceptance.

## Important license/provenance note: Quaternius

Quaternius pack pages observed during this audit still describe several packs as CC0, while the current Quaternius site also publishes **Quaternius Asset License (QAL) v1.0**, last updated 2026-08-28. QAL permits use/modification in commercial products but forbids redistributing the assets themselves as standalone assets; its Section 7 states future license changes do not retroactively alter assets already obtained under an earlier version.

Therefore this audit does **not** rewrite the license truth of source packs already admitted by Juego2. For every actual source adoption/derivative, preserve the exact acquisition record and license/package evidence already required by project provenance policy.

Observed current license page:
- https://quaternius.com/license.html

## 1. Quaternius-native production advantages

### Universal Base Characters

Observed official/source claims:

- six base models across Superhero, Regular and Teen proportions, male/female;
- optimized animation-friendly topology;
- Humanoid rig for retargeting;
- 20 hairstyles;
- Source tier includes rigged `.blend` files and engine projects;
- compatible with Universal Animation Library.

Sources:
- https://quaternius.com/packs/universalbasecharacters.html
- https://quaternius.itch.io/universal-base-characters

Juego2 relevance: **HIGH**. The shared character basis makes a variant factory more credible than creating ordinary NPCs as unrelated bespoke meshes.

### Modular Character Outfits - Fantasy

Observed official/source claims:

- modular parts rather than only indivisible characters;
- compatible with Universal Base Characters;
- Humanoid rig / retargeting support;
- multiple texture/color variants;
- Universal Animation Library compatibility.

Sources:
- https://quaternius.com/packs/modularcharacteroutfitsfantasy.html
- https://quaternius.itch.io/modular-character-outfits-fantasy

Juego2 relevance: **HIGH as ADAPTABLE/DONOR input**, not as a requirement to preserve fantasy styling. This aligns directly with the accepted asset-reuse amendment.

### Universal Animation Library

Observed official/source claims:

- 120+ animations in UAL1 Source;
- universal humanoid rig;
- Unity/Godot/Unreal-ready exports;
- root-motion and no-root-motion variants;
- Source `.blend` availability.

Sources:
- https://quaternius.com/packs/universalanimationlibrary.html
- https://quaternius.itch.io/universal-animation-library

Juego2 relevance: **HIGH** for `WP-ART-ANIM-01`; exact owned/admitted UAL editions remain provenance-controlled.

### Medieval Village MegaKit

Observed official/source claims:

- 300+ modular pieces;
- grid-oriented walls/floors/stairs/roofs/openings;
- walls with exterior + interior treatment;
- Source project implemented in Unity URP;
- custom wear shaders and optimized collisions in Source.

Sources:
- https://quaternius.com/packs/medievalvillagemegakit.html
- https://quaternius.itch.io/medieval-village-megakit

Juego2 relevance: **VERY HIGH** for donor/component reuse. Its modularity strongly supports the accepted ART-01 recovery direction; source pack origin is not final visual identity.

### Downtown City MegaKit

Observed official/source claims:

- 300+ modular building/street pieces;
- mix-and-match building construction;
- Source fake-window-interior shader;
- vertex-color-controlled wear;
- fake normal bevels;
- custom simple collisions;
- example buildings.

Source:
- https://quaternius.itch.io/downtown-city-megakit

Juego2 relevance: **VERY HIGH candidate** for later port-town/urban breadth if/when the exact source is owned and admitted. Do not treat this audit as acquisition authority.

## 2. Character / wardrobe pipeline findings

### Chilyer Studios — `blender-character-pipeline`

Source: https://github.com/ChilyerStudiosLLC/blender-character-pipeline  
Observed license: MIT (`LICENSE` in repository).  
Disposition: **ADAPT / REFERENCE — HIGH VALUE**.

The repository demonstrates a reproducible Blender/Python workflow using a Quaternius Universal Base Character. Relevant techniques include:

- shared base-mesh import;
- lattice/sculpt proportion editing;
- silhouette gate before detail work;
- body-topology costume generation by duplicating/culling/pushing body surfaces;
- solidify/material stages;
- verification using a real animation before export;
- scripted glTF export.

It is a workflow demonstration, not a Juego2-ready dependency. Its value is that the core production shape already exists and can inform `WP-ART-CHAR-01` without forcing a bespoke invention-first path.

### Tinqs clothing pipeline

Source: https://git.tinqs.com/tinqs/animation/src/branch/main/clothing/README.md  
License status for repository code: **NOT VERIFIED BY THIS AUDIT**.  
Disposition: **REFERENCE_ONLY — VERY HIGH CONCEPTUAL VALUE** until exact code license is resolved.

Observed architecture is a deterministic/headless Blender garment lane with staged checkpoints. It addresses:

`census -> prepare -> fit -> reduce -> bake -> skin -> export`

and documents techniques such as:

- body-shell fitting/clearance;
- selective shrinkwrap behaviour;
- per-part geometry budgets;
- weight transfer to a shared Quaternius skeleton;
- skirt/hem handling;
- config-driven processing;
- stage-local `.blend` checkpoints and QA renders.

This is especially strong evidence that ordinary clothing adaptation can be industrialized. It is **not** permission to copy code while its license remains unresolved.

### Avelune — Quaternius character composition

Source: https://github.com/benjamincanac/avelune  
Observed repository license: MIT.  
Disposition: **REFERENCE / POSSIBLE ADAPT — HIGH VALUE**.

The project documents composition of Quaternius characters from outfit + trimmed Universal Base Character head and a shared animation set from UAL1/UAL2. Its design intentionally avoids duplicating animation data per character by binding a shared skeleton-only animation asset to compatible character rigs.

Juego2 should treat the technique as a reference for variant/population architecture, not copy web-runtime assumptions into Unity blindly.

### `Animate-Rigged-Humanoid-No-Blender`

Source: https://github.com/NafisRayan/Animate-Rigged-Humanoid-No-Blender  
Disposition: **REFERENCE / WATCH**.

It demonstrates a glTF-transform based path for merging Quaternius characters with UAL animation data without Blender. It is primarily web/Three.js oriented, so it is not a recommended Unity dependency. Its value is confirming that the shared skeleton can be exploited programmatically and that animation packaging is not intrinsically tied to manual retargeting.

## 3. Quaternius→Unity tooling findings

### `QuaterniusUnityUtils`

Source: https://github.com/firstkindgamer/QuaterniusUnityUtils  
Observed license: Unlicense/public-domain dedication in repository `LICENSE`.  
Disposition: **ADAPT / SPIKE — HIGH VALUE**.

Observed features:

- Universal Animation Library importer automation (axis conversion, animation type, root-motion node and loop settings);
- non-destructive collision-prefab construction from Quaternius model/collision naming;
- Medieval Village MegaKit explicitly listed as supported.

This is a strong candidate to inspect before writing equivalent editor tooling. It must still be revalidated against Juego2's exact Unity 6.3/H2F baseline before adoption.

### `game_export`

Source: https://github.com/codec-xyz/game_export  
Observed repository license: MIT.  
Disposition: **REFERENCE / SPIKE — MEDIUM-HIGH VALUE**.

Observed capabilities:

- Blender collections exported as FBX + Unity prefabs;
- Unity static settings;
- material remapping to existing Unity `.mat` assets;
- collider configuration;
- collection-instance handling.

Risk: it directly generates Unity-side asset/prefab metadata/YAML conventions. Treat it as a technique/tool candidate that must be proven against the accepted Unity version rather than assuming old Unity serialization remains safe.

## 4. Environment factory findings

### Auto-Building (commercial Blender add-on)

Source: https://superhivemarket.com/products/auto-building  
Observed marketplace license: GPL; standalone tier observed at USD 20.  
Disposition: **SPIKE CANDIDATE — VERY HIGH POTENTIAL VALUE, NO PURCHASE AUTHORITY**.

Observed capabilities relevant to Juego2:

- custom user-provided building-part collections;
- doors/windows/panels/other parts distributed over a base mesh;
- boolean openings;
- deformation of parts to facade shape;
- edge-driven repeated details / ledges;
- roof tiles/parapets;
- roof/facade scatter;
- support pillars/foundations;
- modular roof (beta) and basic interiors.

This maps unusually well to `DONOR_COMPONENTS`. A Quaternius window/door/balcony/trim library could be consumed as custom parts rather than requiring the add-on's presets.

Current caveat observed 2026-09-28: the product page warns it is **not properly working with Blender 5.2** and announces a future update. Do not purchase/adopt until `WP-ART-ENV-02` knows its Blender baseline and a bounded spike demonstrates real time/quality savings.

### Geo-Buildings

Source: https://superhivemarket.com/products/geobuildings  
Observed marketplace price: USD 11; marketplace describes Creative Commons licensing.  
Disposition: **WATCH / ALTERNATIVE SPIKE**.

Geometry-Nodes building generator with custom asset collections/materials and manual placement control. It is worth comparing against Auto-Building before any purchase because it may cover enough of the same authoring problem more cheaply. Exact purchased-version terms must be reverified before adoption.

### `osm_building_grammar`

Source: https://github.com/p-schulz/osm_building_grammar  
Observed repository license: Apache-2.0.  
Disposition: **REFERENCE / ADAPTABLE ARCHITECTURE — HIGH VALUE**.

The strongest relevance is its semantic decomposition rather than OSM itself. The grammar uses explicit building roles including facades, windows, frames, sills, balconies, signboards, awnings, garage/loading elements, shutters, roofs, dormers, chimneys, gutters and other reusable detail classes. It also demonstrates deterministic style/config data, metadata retention, instancing and batching.

Juego2 should consider this **a design reference for semantic donor metadata**. It does not imply that OSM or its generated visual style should become authoritative.

### Material Batch Tools

Source: https://github.com/theanine3D/mat_batch_tools  
Observed repository license: GPL-3.0.  
Disposition: **EXTERNAL TOOL CANDIDATE / SPIKE — MEDIUM-HIGH VALUE**.

Observed capabilities include batch modification of material nodes, template-like node unification and repetitive UV/bake preparation. This is relevant to systematic Quaternius palette/material normalization.

Because it is GPL, default project policy should prefer keeping it as an external Blender authoring tool rather than embedding/copying it into shipped Juego2 code unless explicitly reviewed.

## 5. Proposed factory architecture derived from the audit

This is a **discovery model**, not yet a canonical implementation:

```text
1. INGEST
   admitted Quaternius Source / owner-provided lawful sources
        |
2. SEMANTIC CATALOG
   role + dimensions + materials + connection data + provenance
   DIRECT / ADAPTABLE / DONOR / derived
        |
3. ADAPTATION
   palette/material normalization
   bounded geometry edits
   remove incompatible genre cues
   donor extraction / derived components
        |
4. ASSEMBLY
   Juego2-owned assembly grammar
   optional proven helpers/generators
        |
5. VALIDATION
   dimensional/connection checks
   clipping/rig checks
   third-person scale
   KEEPER_READY / PROXY_VISUAL / COVERAGE_BLOCKED
        |
6. UNITY HANDOFF
   accepted H2F import/material/prefab/collider path
   provenance preserved
```

No external tool may collapse stages 2, 4 or 5 into an opaque black box. Juego2 must retain semantic catalog/grammar and final keeper judgement.

## 6. Recommended bounded spikes

These are candidates for their owning WPs, not work authorized by this audit.

### Environment spike

Use one admitted Medieval Village family:

1. select roughly 8–15 real donor components;
2. classify semantic roles (`wall`, `opening`, `window`, `door`, `balcony`, `roof`, `trim`, etc.);
3. compose a materially new urban facade/building using either native/manual assembly or one candidate procedural helper;
4. normalize materials/palette;
5. hand off through the H2F-accepted Unity path;
6. capture at third-person scale;
7. compare operations/time/errors with the existing manual path.

Success is not "generator ran". Success is lower repeat-work **and** a keeper-quality composed result.

### Character spike

1. one admitted Universal Base Character;
2. one recovered/adaptable/donor wardrobe family;
3. one scripted/tool-assisted transformation inspired by audited pipelines;
4. one real UAL motion/pose validation;
5. clipping/weight/scale/rig evidence;
6. repeat on a second variant without a bespoke new script.

### Unity import spike

Before writing custom tooling, test whether the relevant ideas from `QuaterniusUnityUtils` cover UAL import/collision-prefab setup under the exact H2F Unity baseline. If the utility is not compatible, preserve the learned rules and implement only the missing minimal equivalent.

## 7. What this audit changes — and does not change

It materially changes the **starting assumption** of future ART industrialization work:

> Do not begin by inventing a new pipeline. Begin by checking Quaternius-native capabilities, this audited ecosystem, and bounded adaptation of existing tooling/techniques.

It does **not**:

- stop or restart `WP-H2F-02`;
- install Blender add-ons or Unity packages;
- buy Auto-Building, Geo-Buildings or new Quaternius packs;
- admit external source bytes;
- make any GitHub/marketplace project canonical;
- weaken provenance or `DEPENDENCY_IP_POLICY`;
- change the northern-Spain port-town visual target;
- replace human/owner visual acceptance.

See `Docs/discovery/QUATERNIUS_WP_IMPACT_MAP.md` for the exact workpack mapping and `Docs/discovery/QUATERNIUS_TOOLING_CATALOG.md` for candidate-by-candidate disposition.
