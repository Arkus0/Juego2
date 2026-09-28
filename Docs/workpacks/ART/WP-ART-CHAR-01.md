# WP-ART-CHAR-01 — Character/wardrobe production grammar

Status: **PROPOSED / NOT_STARTED**
Class: PRODUCT ART / PRODUCTION INDUSTRIALIZATION
Depends on: `WP-H2F-GATE` PASS + accepted character/source/provenance inputs from H1/H2F/ART
Blocks: `WP-ART-CHAR-02`
Binding decision: `Docs/product/VISUAL_PRODUCTION_FACTORY_AMENDMENT.md`
Binding reuse amendment: `Docs/product/ASSET_REUSE_VISUAL_DIRECTION_AMENDMENT.md`

## Claim

Juego2 has a reviewed, source-traceable character/wardrobe vocabulary and production recipe sufficient to create ordinary population characters repeatedly without treating each NPC as a bespoke art project.

## Required work

Audit the accepted Quaternius/source character inputs and any lawful owner-provided character/clothing sources actually admitted for production. Define the smallest reusable grammar needed for the large northern-Spain port-town population.

The audit follows the same reuse-first rule as environment art: pack labels do not decide keeper value. A fantasy-labelled outfit or clothing part may be `DIRECT`, `ADAPTABLE` or a `DONOR` component if its final civilian silhouette and context fit after bounded treatment. Remove/reject armour, weapons and unmistakable genre ornament where needed rather than rejecting an otherwise useful source family by label alone.

At minimum resolve:

- base body/rig/avatar families actually used;
- Humanoid/retarget compatibility and scale;
- tops, bottoms, outerwear/workwear, footwear, headwear and bounded accessories where the source supports them;
- hair/head presentation where available;
- palette/material variation rules that avoid obvious clone repetition;
- role-oriented outfit families such as port/work, shop/service, casual, older/conservative, youth and civic/professional where visually supportable;
- `DIRECT / ADAPTABLE / DONOR / REJECT` source dispositions and source-vs-derived identity;
- lawful derivative rules and component recombination lineage;
- mesh intersection, clipping, skin exposure, layer ordering and LOD/collider expectations where relevant;
- prefab/variant naming and the boundary between visual identity and later Arkus canonical actor identity.

Do not create gameplay jobs, personality, schedules or narrative tiers here. A visual role preset is not a systemic actor definition.

## Production recipe

Publish a reproducible recipe from admitted source to game-ready presentation:

`source body/rig -> reuse/adapt/donor decision -> wardrobe selection/derivation -> palette/accessories -> Humanoid validation -> material/import validation -> prefab/variant -> GC2/H2-ready presentation`

The recipe must name which stages are deterministic/tool-assisted and which require human visual judgement. If Blender or another local DCC helper is used, preserve a reconstructable source/derivative path rather than committing opaque one-off outputs without lineage.

## PASS-before-work acceptance contract

**Mandatory evidence:**

1. source/wardrobe coverage and gap matrix;
2. visual target references for ordinary population clothing in the final port-town setting;
3. production recipe and folder/identity conventions;
4. representative fit/scale/clipping inspection across the admitted base families;
5. at least one recovered/adapted or donor-use example from a source item that would have been rejected under a strict fantasy-pack-label reading, if such admitted source exists;
6. at least one rejected/blocked combination proving invalid assemblies fail visibly rather than silently shipping;
7. a bounded set of role-oriented outfit recipes sufficient for `ART-CHAR-02` to produce a varied batch;
8. residual ledger separating missing breadth from factory-blocking gaps.

**FAIL if:** the only production path is hand-editing every NPC independently; clothing is accepted because it technically attaches while visibly clipping/floating; source provenance is lost; outfit roles silently become gameplay/systemic truth; fantasy-pack origin alone is treated as a rejection oracle despite useful adaptable/donor components; or the vocabulary is so narrow that `ART-CHAR-02` cannot produce a visibly varied representative group without inventing a new pipeline.

**Allowed residuals:** hero-specific bespoke garments, final facial detail, all-town breadth, later special occupations and story-specific outfits.

## Non-claims

No final character factory throughput, animation-library breadth, NPC behaviour, persistent identity, dialogue system or population-scale claim. Those belong downstream.
