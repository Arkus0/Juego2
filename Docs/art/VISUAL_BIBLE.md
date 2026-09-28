# Visual Bible — Juego2

Version: 0.2.0 — 2026-09-28  
Status: DRAFT — current art-direction authority must be read with accepted port-town product amendments and accepted `Docs/product/ASSET_REUSE_VISUAL_DIRECTION_AMENDMENT.md` (PR `#267`).  
Scope: art direction and asset selection only. This document does not alter H0/H1 authority or CITY topology.

> **Current-setting note.** The former fictional Potes/Liébana inland anchor is no longer the final product setting. It survives as historical/pilot art evidence only. The current final-product setting is the accepted **large fictional port town / villa portuaria in northern Spain**. Final town proper name remains undecided.

## 1. Style one-liner

A low-poly, stylized **northern-Spain port town**: dense, damp, lived-in streets; old-town, market, working-port, workshop and residential layers; restrained materials and readable human-scale silhouettes; the practical visual economy of a late-PS2 / early-PS3 game reinterpreted through a modern Unity pipeline.

The target is **not photorealism and not a geographically literal reconstruction**. Juego2 should look coherent and authored as one northern-Spain world while making aggressive, intelligent use of the admitted Quaternius/source library.

The Shenmue/Yakuza-era reference is primarily density, readability, lived-in composition and production economy rather than literal rendering emulation.

## 2. Yes / No

### Yes

- Flat or lightly gradient albedo and restrained materials compatible with the admitted low-poly source ecosystem.
- Chunky readable silhouettes; detail carried by shape, composition, props, signage and color more than texture resolution.
- Humidity / Atlantic-northern mood as a strong default: overcast-friendly light, damp greens, wet/dark surfaces and warmer localized interiors/nightlife.
- **Mixed wall language:** stone, render/plaster, masonry and timber can coexist by street/building role.
- **Wood is allowed.** Timber may be structural, decorative or contextual where it helps the scene: balconies, galleries, beams, shopfronts, workshops, port structures, service buildings, interiors, stairs, porches, sheds and mixed assemblies.
- Dense human-scale frontage rhythm, corners, thresholds, alleys, shops, bars, workshops and port/work edges.
- Reuse of admitted source assets as intact prefabs, adapted assets or donor components.
- Day/evening/night variants that materially change the social read without requiring a separate art pipeline.
- Shared humanoid retarget and reusable wardrobe/animation vocabulary.

### No

- Photorealism or a requirement that every asset be geographically literal in isolation.
- PBR micro-detail as a substitute for shape/composition quality.
- Generic sunny Mediterranean resort language as the dominant town read.
- A final keeper scene dominated by unmistakable fantasy/medieval/alpine signals: castles/fortifications, pervasive decorative half-timbering, town-wide thatch, exaggerated chalet massing, fantasy signage/weapons or equivalent genre markers.
- Blanket rules such as “wooden assets are forbidden”, “Medieval Village is unusable”, or “fantasy-labelled wardrobe is automatically rejected”.
- American-road / skyscraper / anonymous-major-city language as the dominant scale.
- Leisure-marina identity replacing the accepted working-port role.
- Palms, cacti or tropical vegetation as normal local language.
- Folkloric costume caricature or pseudo-realistic regional physiognomy requirements.
- Visible primitive greybox shells dressed with windows/roofs/props while presented as keeper architecture.

## 3. Palette and material intent

The earlier ART-00 palette remains useful as a **family of restrained northern values**, not a literal all-town material quota. Exact values may be tuned under the accepted URP/H2F baseline.

Useful retained anchors:

| Role | Hex |
|---|---|
| Light stone / plaster family | `#C9C3B6` |
| Mid stone / masonry family | `#A79F92` |
| Shadowed stone / quoins | `#7E7568` |
| Dark wet roof family | `#4A3730` |
| Timber family | `#5C4433` |
| Shutter / painted joinery green-blue | `#3E5A57` |
| Mid damp green | `#4E7A43` |
| Overcast high / low | `#B9C4C9` / `#93A3AC` |
| Port/river/sea-grey water family | `#3F5A5E` |
| Localized warm interior / nightlife accent | `#E8B26A` |

Avoid pure black/white albedo and uncontrolled saturation. District/street identity may legitimately shift material balance and accent frequency.

## 4. Scale / composition

Human-scale third-person readability is the primary visual judge. Streets, thresholds, facade depth, roof/base junctions and ground contact must read as intentional at ordinary player distance.

Final identity is expected to emerge from **composition**, not from requiring bespoke geography-specific meshes everywhere:

- frontage rhythm and irregular massing;
- material and palette treatment;
- wet northern lighting/atmosphere;
- signage, shopfronts and street furniture;
- port/work/commercial ground details;
- vegetation and terrain;
- hero landmarks and high-value bespoke derivatives;
- character styling and activity context;
- day/evening/night presentation.

## 5. Current setting anchor

Final product: a **large fictional port town / villa portuaria in northern Spain**, around the late 1990s / early 2000s in product feel. It is a substantial town/comarca hub, not an anonymous major city.

Current production-neighbourhood model:

- `N.CASCO` — dense old town; primary nightlife pole while retaining civic/family/day uses;
- `N.MERCADO` — main repeat-visit commercial/everyday zone;
- `N.MUELLE` — working port, including night work and a bounded port-social layer;
- `N.TALLERES` — industrial/work zone with secondary/alternative nightlife;
- `N.VIVIENDAS` — lower-intensity residential fabric.

The final proper name is undecided. Do not bake `Villa Bruma` or any other unreviewed name into keeper signage/assets/UI.

## 6. Reuse-first source policy

**Production strategy:** use the maximum practical amount of admitted source material before seeking replacement assets.

Quaternius/source packs are an **editable upstream parts library**, not only a set of final prefabs. Whole prefabs may be retained, adapted or dismantled into donor components while preserving provenance.

Use these dispositions:

- `DIRECT / A` — source item already works substantially as-is;
- `ADAPTABLE / B` — source remains the basis after bounded material/mesh/part/composition treatment;
- `DONOR / C` — whole prefab is unsuitable, but useful components are recombined into Juego2 derivatives/assemblies;
- `REJECT / D` — core silhouette/function/theme or adaptation cost remains incompatible;
- `BLOCKED_EXTERNAL` — a required role remains uncovered after reuse and reasonable derivative work.

For ART-01 compatibility: `KEEP_AS_IS = DIRECT`, `ADAPT = ADAPTABLE`, `DONOR_COMPONENTS = DONOR`, `REJECT_FOR_KEEPER = REJECT`.

**Pack labels are not visual oracles.** An asset originating in a medieval/fantasy pack may still be valuable if its final adapted/composed read belongs to Juego2. Conversely, a technically modern asset may still be rejected if it breaks scale or identity.

## 7. Wood / medieval-source handling

Wood is **not** a banned material and must not be used as an automatic rejection criterion.

A timber-heavy source asset can be:

- used directly where appropriate;
- mixed with stone/render/masonry;
- simplified or recolored;
- stripped of fantasy ornament;
- combined with modern shopfronts, gutters, utilities, signage, shutters, street furniture and port/commercial context;
- harvested for roof, beam, window, door, stair, balcony, chimney, trim or interior components.

The rejection boundary is the **final aggregate read**, not the upstream label or raw material percentage.

## 8. Adaptation-cost rule

Reuse is preferred, not mandatory at irrational cost.

Reject when the core silhouette/function fights the target, when unmistakable genre markers cannot be removed cheaply, or when adapting the asset would amount to rebuilding most of it and a cleaner admitted alternative/derived solution is cheaper.

Every material rejection should have a causal reason. “Not realistic enough for northern Spain” is insufficient on its own.

## 9. ART-01 recovery requirement

Before ART-01 final candidate/PASS, previously rejected/deprioritized source material must receive the bounded recovery pass defined by `ASSET_REUSE_VISUAL_DIRECTION_AMENDMENT.md`, especially items rejected primarily for visible wood, medieval/fantasy pack origin, weak direct regional specificity, or correctable material/palette mismatch.

ART-01 does **not** restart. Existing valid structural, dimensional, paintover, provenance and H2F sequencing work remains useful.

## 10. Character / wardrobe reuse

Wardrobe follows the same reuse-first rule. Fantasy-labelled clothing is not automatically unusable. Useful tops, bottoms, footwear, belts, bags, hats, coats or other components may be simplified/recombined/recolored if the final civilian silhouette works.

Armour, weapons and unmistakable fantasy ornament remain valid rejection/removal candidates. Final character presentation must fit role, period feel and scene context.

## 11. Acquisition rule

Do not purchase/adopt a new pack merely because useful owned material was filtered by an over-strict realism interpretation.

External acquisition becomes justified when a meaningful role remains blocked after DIRECT reuse, ADAPTABLE treatment, DONOR/component reuse and reasonable Juego2 derivative creation, subject to existing dependency/IP and owner-cost policy.

## 12. Final visual oracle

The decisive question is:

> **Does the composed third-person result read coherently as Juego2's northern-Spain port town?**

not:

> **Was every individual source asset originally authored for this exact geography?**

A scene may use a high proportion of Quaternius/source content and still PASS if massing, joins, materials, lighting, dressing, signage, vegetation, characters and contextual motifs create one coherent world. A scene may use geographically plausible assets and still FAIL if it reads as dressed greybox, asset showroom or incoherent kitbash.
