# WP-ART-01 — Quaternius Source → Juego2 derivative asset pipeline

Status: **PLANNED / NOT_STARTED**  
Class: PRODUCT / CONTENT PIPELINE (NON-FOUNDATIONAL)  
Mode: **LOCAL_DCC + DOCUMENTED**  
Depends on: `WP-ART-00` seed direction  
Provides content prerequisite for: `WP-H1-04` and later H1/CITY/H2 production-content probes

## Objective

Establish Quaternius Source as Juego2's default editable upstream art library and define the reproducible derivative pipeline used to turn it into Juego2-specific production assets.

The intended model is:

```text
human-approved Quaternius Source pack(s)
        ↓
immutable upstream/source record
        ↓
Juego2 adaptation in DCC/source files
        ↓
Juego2 Derived assets
        ↓
Unity import/catalogue/materialization
```

Juego2 should reuse the maximum sensible amount of Quaternius Source rather than fabricate replacement assets from scratch. Missing or unsuitable content is created by modifying, combining, retargeting or extending the accepted source style where legally and technically appropriate.

This includes clothing/outfits, character variants, missing props/building pieces, material/color variants and animation additions/edits/retargeting. Characters used as production-intent Juego2 civilians must not rely on an unclothed base body as the visible final result.

## WHY_THIS_BOUNDARY

Art-source adaptation and Unity bridge correctness are independently rejectable claims.

- H1 owns whether Unity/Arkus can catalogue, materialize, inspect and rebuild assets truthfully.
- ART owns what production asset is selected or created, how it derives from Quaternius Source, its visual/style constraints and its source provenance.

Putting Blender/source-art creation into H1-04/06 would mix content production with foundational engine-bridge proof. Deferring all real adaptation until H1-11 would instead force H1-04..10 to prove against temporary substitute art that the project already intends to throw away.

ART-01 therefore runs in parallel before H1-04 and produces a small accepted source/derived baseline that H1 can consume without owning art creation.

## Upstream/source rule

- Quaternius Source is the **preferred editable upstream**, not an untouchable binary dependency.
- Exact packs/distributions used by Juego2 must be human-approved and recorded before use.
- Preserve an immutable provenance identity for every upstream source item used: pack/distribution, source URL, obtained version/date/key/hash where available, governing license/terms and original item identity.
- Do not overwrite the upstream original when creating Juego2-specific content. Create a derived asset with explicit lineage.
- Reuse existing Quaternius shapes/rigs/material language/atlases whenever reasonable before creating unrelated art from scratch.
- A new non-Quaternius art source requires an explicit reason and its own dependency/IP adoption decision; it must not silently become a parallel visual foundation.

## Juego2 Derived rule

A **Juego2 Derived asset** is an authored product asset created from one or more accepted upstream source assets and/or existing Juego2 derivatives.

Each retained derivative records:

- stable Juego2 asset identity;
- upstream source asset(s) and exact provenance;
- type of transformation (mesh edit, modular recomposition, material/atlas variation, rig/skin/outfit addition, animation retarget/edit/new keyframes, etc.);
- editable source-file location/identity;
- exported engine artifact(s);
- scale/pivot/rig/material assumptions;
- license/distribution boundary inherited from the source plus any new locally-authored contribution;
- replacement strategy.

Derived assets are first-class Juego2 production inputs but **not canonical Arkus world state**. They remain replaceable catalogue inputs to the Engine Bridge.

## Initial deliverable slice

ART-01 does not build the whole game art library. It proves the pipeline with the minimum slice needed before H1-04..H1-07 require real game-representative content:

1. at least one Quaternius Source architectural/facade or modular environment family appropriate to the approved Potes/Liébana style;
2. at least one material/atlas adaptation demonstrating approved stone/tile/palette treatment without breaking the source style;
3. one Quaternius humanoid source baseline;
4. at least one **clothed Juego2 civilian derivative** with outfit/silhouette appropriate to the Visual Bible;
5. at least one accepted civilian animation source and one demonstrated retarget/edit path on the shared humanoid rig;
6. one missing/modified prop or modular piece created by adapting/recombining the accepted source rather than replacing the visual language with an unrelated kit.

The exact assets are selected by the Worker from the human-approved Source packs; ART-01 does not require every future Juego2 asset to exist.

## Deliverables

- `Docs/art/QUATERNIUS_SOURCE_ADAPTATION.md` — source hierarchy, DCC/edit/export rules, visual/style constraints and no-nudist character rule;
- `Docs/art/ASSET_PROVENANCE_LEDGER.md` — machine/reviewer-readable upstream → derivative lineage for the initial slice;
- exact pack/source/license/adoption records satisfying `Docs/engineering/DEPENDENCY_IP_POLICY.md`;
- editable source + exported derivative examples for the initial slice, stored/distributed only in a way permitted by the exact adopted terms;
- evidence captures/metadata showing the initial derivatives correspond to their recorded source lineage;
- explicit handoff identifying which upstream and derived assets H1-04 may catalogue.

## Acceptance

- Quaternius Source is recorded as the preferred upstream for production-intent art, with exact adopted pack/source terms verified rather than inferred from a generic website claim;
- the source/original and Juego2-derived namespaces/storage boundaries are distinct;
- at least one real derivative round-trip is demonstrated from editable source to exported game asset while preserving lineage;
- the initial civilian visible result is clothed and compatible with the shared humanoid/animation direction;
- a material/environment adaptation remains recognizably within the Visual Bible rather than drifting into a new art style;
- a missing prop/module can be created by source adaptation with truthful provenance;
- later H1/CITY/H2 consumers can reference stable accepted source/derived asset identities without needing to know DCC internals;
- no original or modified Quaternius asset is publicly redistributed as a standalone asset pack contrary to its governing terms.

## Forbidden

- whole-game asset production;
- final wardrobe catalogue;
- final NPC cast;
- claiming every future object or animation already exists;
- using H1/Unity-generated prefab derivatives as a substitute for editable DCC art-source provenance;
- destructive edits that erase the upstream/original lineage;
- silently introducing unrelated art packs because a Quaternius item is missing;
- publishing source or modified Quaternius assets as standalone downloadable assets;
- H2 gameplay logic, CITY keeper realization or runtime animation behavior claims.

## Dependency / IP implications

At execution time the Worker must verify the exact license/terms governing each obtained Quaternius Source pack/version. The current public Quaternius Asset License permits use/copy/modification and incorporation of original or modified assets into commercial products while restricting standalone redistribution of the assets; exact adopted-pack evidence remains authoritative under Juego2's dependency policy.

If a specific pack is governed by different terms (for example an earlier CC0 release or platform-specific purchase terms), record those exact terms rather than assuming one global license.

## H1 handoff

`WP-H1-04` consumes the accepted ART-01 source/derived baseline as catalogue inputs. H1 may create bridge-managed Unity derivatives/variants for technical projection purposes, but it does not become the owner of clothing design, mesh editing, prop modeling or animation authoring.

`WP-H1-11` later expands the **conformance breadth** over a representative set of accepted upstream + Juego2 Derived assets. It is not the first time source art is used and is not the owner of the art-production pipeline.

## Future expansion

H2/CITY production can add further ART workpacks for wardrobe breadth, NPC variation, missing architecture/props, environment kits and animation coverage. They should extend this upstream→derived provenance model rather than invent a second content pipeline.
