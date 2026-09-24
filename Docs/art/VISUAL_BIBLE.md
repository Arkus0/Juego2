# Visual Bible — Juego2

Version: 0.2.0 — 2026-09-24  
Status: DRAFT — seed under `WP-ART-00`; palette **APPROVED-DRAFT**; setting anchor **fictional Potes / Liébana** (see `Docs/art/SETTING.md`); §13 alt-Liébana register **PROPOSED — pending owner approval**  
Scope: art direction and asset selection only. **This document does not block, gate or modify H0.**

## 1. Style one-liner

A low-poly, stylized **Cantabrian mountain-valley market town** — a fictionalized **Potes in Liébana** — wet stone, dark tile, damp green slopes and rock silhouettes under a high overcast sky — built from a restricted Quaternius-compatible kit, readable at 15 metres, with the shape language of a late-PS2 / early-PS3 game reinterpreted rather than emulated.

It is **not** a coastal harbour town as the primary identity (Castro-style coast may inform secondary mood only).

Tone line (v0.2): **"Lo raro es de aquí."** An *alt Liébana* where the uncanny, the esoteric and the martial live inside ordinary Cantabrian daily life. The strangeness is grown from the valley's own material (Beato, stelae, shepherds' staffs, bolos, orujo stills, fog) and only reaches the East through people and objects — never through architecture or a theme-park skin. See §13.

Three looks this excludes outright: photorealism; the Dreamcast/Shenmue graphical style (lived-in *feeling* kept, rendering not); and the generic sunny Mediterranean village (whitewash, flat blue sky, terracotta warmth).

## 2. Yes / No

### Yes

- Flat or lightly gradient albedo, atlas-driven, consistent with Quaternius packs.
- Chunky readable silhouettes; detail from shape and color, not texture resolution.
- Humidity as dominant mood: darker values, lower contrast, green bias in shadow.
- Stone as primary wall; dark tile as primary roof.
- Overcast, diffuse key light as default; sun is an event.
- **Valley town life:** market grain, bar terrace, workshops, laundry, worn kerbs; optional river/bridge edge later.
- Green slopes / rock masses beyond the roofscape (Picos-adjacent read, not alpine postcard).
- Modular reuse of a small kit.
- Shared humanoid retarget; civilian animation allowlist (§12).
- **Alt-Liébana register (§13):** strangeness at object/person scale, sourced from local material; fog at dawn as the signal that something is happening; density that grows with depth.

### No

- Photorealism or "top-tier realistic PS3".
- PBR micro-detail layered onto flat atlas assets.
- Dreamcast/Shenmue as *graphical* target.
- Alpine half-timbering; steep chalet roofs.
- Mediterranean whitewash-and-cobalt; terracotta warmth as default.
- Fantasy: thatch, castles, palisades, ruins-as-fantasy, medieval signage, weapons.
- American road language; downtown blocks; skyscrapers.
- **Harbour / open sea / fishing-port identity as the hero read** (wrong valley).
- Palms, cacti, tropical foliage, bulk autumn red.
- Realistic regional facial physiognomy; folkloric costume jokes.
- Combat / gun / parkour animations as village default (§12).
- **Imported-Orient skin:** pagodas, torii, dragon ornament, lanterns strung over streets, hanzi signage on public façades, red-lacquer architecture, gi/kimono/changshan as everyday wear (§13).
- Mystical VFX as ambient default: glowing runes, auras, particle magic on props.

## 3. Palette (APPROVED-DRAFT)

Hex values are **albedo targets in sRGB, before lighting**. Status: **APPROVED-DRAFT** (2026-09-19).

| Role | Hex |
|---|---|
| Light stone | `#C9C3B6` |
| Mid stone | `#A79F92` |
| Shadowed stone / quoins | `#7E7568` |
| Tile wet (hero default) | `#4A3730` |
| Dark timber | `#5C4433` |
| Shutter green-blue | `#3E5A57` |
| Mid meadow green | `#4E7A43` |
| Overcast high / low | `#B9C4C9` / `#93A3AC` |
| River / stream | `#3F5A5E` |
| Hydrangea / market accent / interior warm | `#6F79B8` / `#C24B34` / `#E8B26A` |

Accents ≤5% surface. No pure black/white albedo.

**Proposed v0.2 addition — NOT yet approved:** one reserved accent for the extraordinary only (§13):

| Role | Hex | Budget |
|---|---|---|
| Beato orpiment / *lo raro* | `#D4A437` | ≤2% surface; never on ordinary fabric; counts inside the ≤5% accent budget |

Rationale: the existing market/hydrangea/warm accents (`#C24B34`, `#6F79B8`, `#E8B26A`) already rhyme with the flat colour bands of the Beato de Liébana miniatures; the orpiment yellow closes that family and is reserved so the player learns to read it as "something here is not ordinary".

## 4–5. Scale / characters

Streets 4–6 m; plaza ~25×20 m; grid 1 m. Identity = archetype + outfit + silhouette (valley worker, bar server, neighbour, shopkeeper, teen+bike, builder, dog walker, outsider).

Alt-register characters (§13.5) are **the same archetypes**, never a separate costume class: a practitioner is first a shopkeeper, shepherd, distiller or retiree who reads as ordinary at 15 m.

## 6–7. Quaternius Source baseline and Cantabrian adaptation

**Production strategy:** the first playable demo should use the maximum practical amount of an exact human-approved **Quaternius Source** distribution directly. Do not delay a working demo merely to replace usable source assets with bespoke art.

Quaternius Source is the **editable upstream art baseline**, not the final visual authority and not an immutable black box. Its exact purchased/adopted distribution, source identity and terms are pinned at the first Unity asset-adoption point (`WP-H1-04`) under `DEPENDENCY_IP_POLICY.md`.

The intended progression is:

```text
Quaternius Source upstream
        ↓
first playable/demo uses maximum viable direct reuse
        ↓
Cantabrian adaptation where the game actually needs it
        ↓
Juego2 Derived assets
        ↓
keeper/demo art increasingly replaces or augments upstream pieces
```

Adaptation is demand-driven rather than a prerequisite to prove the bridge. Examples include dark wet tile, stone/material changes, shutters, signs, market/bar/workshop props, Cantabrian architectural variants, character clothing/outfit silhouettes, vegetation selection, missing objects and missing civilian animations. New or modified assets should remain stylistically coherent with the accepted Quaternius base and preserve upstream/derivative provenance.

Characters in any player-facing demo must be appropriately clothed for their role. If the usable Quaternius humanoid source is unclothed or lacks a required outfit, a minimal coherent Juego2 derivative outfit is created before that character becomes visible in the demo; this does not require finishing the whole wardrobe system first.

Likewise, reuse the available Quaternius animation set first. Create, adapt or retarget additional clips only when an actual gameplay/demo need is not covered by the accepted source set.

Architectural/nature selection still rejects palms, thatch, chalet/castle language, tropical content and other vetoed shapes. Ships remain optional for later river craft rather than a primary town identity.

## 8. H2 hero target

Plaza + 1–2 streets + bar/shop interior; 6 NPCs; no sea/quay in v1; ≤120 meshes; ≤6 atlases; ≤~20 anim clips.

The accepted CITY seed (`Docs/production/CITY_PRODUCT_SEED.md`) includes a short Río edge and the X1 old bridge, so the river swatch (`#3F5A5E`) and stone bridge language are in scope for the first slice; still no sea/quay.

The first playable may contain visibly unmodified Quaternius Source assets. The target is then to replace or adapt only what is necessary to make the bounded demo read unmistakably as Juego2 / Cantabria while preserving reuse where the source already fits.

## 9–12. Triage, licensing, animation

Triage dry-run: `Docs/evidence/WP-ART-00/TRIAGE_DRY_RUN.md`. License/source terms: **UNVERIFIED-FOR-ADOPTION until the exact human-approved Quaternius Source distribution is recorded at the adoption gate**. Animation: civilian allowlist only (walk/jog/idle/sit/interact), extended only by concrete demo/gameplay need.

## 13. Alt-Liébana register — "lo raro es de aquí" (PROPOSED v0.2)

Status: **PROPOSED — pending owner approval.** Direction only; no asset import, no Unity work, no gameplay or narrative facts.

### 13.1 Premise

The town is first a believable Liébana market town. The *alt* layer is a register on top of it: the mysterious, the esoteric and the martial appear **inside** everyday Cantabrian life — a shop, a bar, a meadow at dawn, a shepherd's hut — never as a separate themed world.

Two rules carry the whole register:

1. **The strangeness is grown from local material.** The valley already has an apocalypse manuscript, sun-disc stones, a pole technique, a ritual game, a distillation craft, sacred trees and caves. These carry the esoteric read.
2. **The East arrives through people and objects, not buildings.** A returned emigrant, a VHS shelf, a scroll in a kitchen, a way of standing. Never a façade, a roofline or a street skin.

### 13.2 Authority boundary with CITY-06

This section owns only **visual register** (shape, colour, light, prop and outfit language). It does not create places, routes, discoveries or narrative facts.

- **Martial / kung-fu / Hong-Kong / Chinese-cinema content opportunities** remain governed by `Docs/production/CITY_INTERIORS_DISCOVERY.md` §11: optional, local, non-explanatory, at most one primary opportunity per seed, and the town must stay coherent if it is missed. The accepted seed currently declares **zero** (`CITY_PRODUCT_SEED.md` §5.5).
- The **atmospheric register** below (fog, stelae forms, Beato colour, hanging herbs, a tejo by the church) is ambient art grain. It may appear across the town at low density without constituting a martial "opportunity", provided it asserts no discovery, secret or function.
- If the product intent is for the martial/esoteric strand to become the game's premise rather than an accent, CITY-06 §11 must be reopened by its own owner. ART cannot widen that cap by restyling.

### 13.3 Local source palette → alt read

| Local source (real Liébana / Cantabria) | Alt-register read | Visual use |
|---|---|---|
| **Beato de Liébana** (Commentary on the Apocalypse, written in the valley) | the esoteric iconography of the game | flat colour bands, stylised eyes/stars/beasts; murals, painted beams, a page in a drawer |
| **Estelas cántabras** (discoidal sun-swirl stelae) | the local mandala | circular motif for a garden gate, a coat of arms, a mark cut in rock, a well cover |
| **Palo / salto pasiego** (shepherds' pole technique) | native staff art | the practitioner's weapon is a shepherd's pole, not a bō |
| **Corro de bolos** (bolo palma ground) | the dojo | a ritual space of stance and precision already present in every village |
| **Té del puerto / té de roca** (Picos mountain herb) | the tea ceremony | bundles drying, a kettle, a kitchen ritual |
| **Alquitara de orujo** (Potes distillation) | alchemy | copper still, patience, transformation; a workshop that reads slightly liturgical |
| **Tejo** (sacred yew beside churches) | the master's tree | one old yew as a landmark silhouette |
| **Cuevas** (painted caves of Cantabria) | the sanctuary | hand prints, ochre, a place for retreat — later content only |
| **Faja** (traditional sash) | the belt | sash colour can mark rank; otherwise ordinary clothing |
| **Indianos** and their houses | the route for foreignness | an emigrant who returned from Hong Kong or the Philippines instead of Cuba |
| **Cantabrian gardens** (hydrangea, camellia — East Asian origin, locally ubiquitous) | the botanical bridge | already on-palette (`#6F79B8`); camellia and ginkgo replace any palm on an indiano plot |

### 13.4 Visual rules

1. **Depth gradient.** Strangeness density rises with depth: plaza < interior < back lane / shared court < monte < cave. Consistent with the CITY-06 layered-discovery model; no layer is required to contain anything.
2. **Frame budget.** In public exterior space, at most **one** alt-register element per typical player frame, sized to an object or a person. Interiors and remote places may carry more.
3. **No architecture skin.** No Eastern roof, gate, colour or signage on public façades. The single allowed exception is a reviewed one-off on an *indiano* plot (e.g. a circular opening in a garden wall, camellias, one ginkgo), subject to the CITY-05 one-off promotion rules.
4. **Reserved colour.** `#D4A437` (§3 proposal) marks the extraordinary only. Ordinary fabric never uses it.
5. **Fog is the signal.** Overcast is default and sun is an event (§2). Low morning fog in meadows and lanes is the register's light: dawn practice, a figure on the slope, a stele half-visible. Fog is lighting/atmosphere, not a gameplay claim.
6. **No ambient magic VFX.** No glow, aura or particle mysticism as default presentation. If later content needs a supernatural beat, its owner designs it explicitly.
7. **Coherent if missed.** Remove every alt element and the town must still read as a complete, ordinary Liébana town.

### 13.5 Characters and outfits

- Practitioners are ordinary archetypes first: the grocer who does slow forms in the meadow at dawn, the retired shepherd with the pole, the distiller, the bar owner with the VHS shelf, the returned indiano.
- Wardrobe: work clothes, waistcoat, beret, albarcas, 90s tracksuit, work coat. The **faja** is the only martial marker. No gi, kimono or changshan as everyday wear.
- Any East Asian character is a full local person with ordinary life, work and dignity; never set dressing, caricature or a mystical function. Realistic regional physiognomy and folkloric costume jokes remain vetoed (§2).
- Animation: Quaternius martial/combat clips may be used only as **practice** in context (forms, sparring at the corro, a demonstration), consistent with §12 and the PA-09 graded-physicality research; never as village default.

### 13.6 Prop list (first candidates, all derivative/demand-driven)

Estela (disc stele), old tejo, wooden training post made from a stump and farm tools, corro de bolos with bolos and boundary, alquitara still, hanging herb bundles, VHS/poster shelf in a social interior, Beato-style mural fragment or painted beam, cuévano (basket), shepherd's pole.

Each remains subject to §6–7 demand-driven adaptation: nothing is produced before a retained place needs it.

### 13.7 Architecture anchor for Quaternius adaptation

The first high-value derivative is the **casona montañesa** read, kitbashed from the Medieval Village source: two projecting stone fire walls (*cortafuegos*), a timber **solana** balcony between them, ground-floor arcade (*soportales*) where the parcel allows, low-pitch dark-tile roof, carved stone coat of arms. Remove half-timbering; lower roof pitch (chalet veto). This maps onto the existing CITY-05 families (`bf.house`, `bf.mixed_use_house`, arcade assemblies) as a reviewed variant, not a new family. The alt accent on it is at most one detail — a coat of arms with an odd emblem.

### 13.8 Adaptation order (for future `WP-ART-01`)

1. Atlas palette remap of adopted Quaternius Source to §3 swatches, with a source→derivative provenance record.
2. Casona montañesa variant set (§13.7).
3. Minimal clothed archetypes with faja variants (§13.5).
4. Alt prop kit (§13.6), only for retained places that need it.
5. Fog/overcast lighting profile; flat shading, vertex AO, no PBR micro-detail.

A local Blender look-probe (one casona, one practitioner, one stele, fog) is recommended as owner-side reference before `WP-ART-01` is authored; it is not repository evidence.

## 14. Change log

| Version | Date | Change |
|---|---|---|
| 0.1.4 | 2026-09-21 | Quaternius Source baseline and demand-driven adaptation rules |
| 0.2.0 | 2026-09-24 | Tone line; §13 alt-Liébana register (PROPOSED); reserved accent `#D4A437` (PROPOSED); Imported-Orient and ambient-magic vetoes; river in first-slice scope per CITY seed |
