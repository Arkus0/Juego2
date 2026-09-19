# Visual Bible — Juego2

Version: 0.1.3 — 2026-09-19  
Status: DRAFT — seed under `WP-ART-00`; palette **APPROVED-DRAFT**; setting anchor **fictional Potes / Liébana** (see `Docs/art/SETTING.md`)  
Scope: art direction and asset selection only. **This document does not block, gate or modify H0.**

## 1. Style one-liner

A low-poly, stylized **Cantabrian mountain-valley market town** — a fictionalized **Potes in Liébana** — wet stone, dark tile, damp green slopes and rock silhouettes under a high overcast sky — built from a restricted Quaternius-compatible kit, readable at 15 metres, with the shape language of a late-PS2 / early-PS3 game reinterpreted rather than emulated.

It is **not** a coastal harbour town as the primary identity (Castro-style coast may inform secondary mood only).

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

## 3. Palette (APPROVED-DRAFT)

Hex values are **albedo targets in sRGB, before lighting**. Status: **APPROVED-DRAFT** (2026-09-19).

### Stone and walls

| Role | Hex | Notes |
|---|---|---|
| Light stone | `#C9C3B6` | Dressed ashlar |
| Mid stone | `#A79F92` | Default wall |
| Shadowed stone / quoins | `#7E7568` | Lintels, wet lower courses |
| Dirty render / limewash | `#E6E1D6` | Sparingly |

### Roof

| Role | Hex | Notes |
|---|---|---|
| Tile, base | `#6B4A3E` | Default |
| Tile, dry | `#8A5E4C` | Sheltered pitches |
| Tile, wet | `#4A3730` | Hero default after rain |

### Carpentry and metal

| Role | Hex | Notes |
|---|---|---|
| Dark timber | `#5C4433` | Doors, beams, balcony rails |
| Shutter green-blue | `#3E5A57` | Signature joinery |

### Vegetation and ground

| Role | Hex | Notes |
|---|---|---|
| Deep wet green | `#2F4A32` | Shadowed foliage |
| Mid meadow green | `#4E7A43` | Default grass / slope |
| Lit grass | `#7BA35A` | Highlights only |

### Sky and water

| Role | Hex | Notes |
|---|---|---|
| Overcast, high | `#B9C4C9` | Zenith |
| Overcast, low | `#93A3AC` | Horizon (may meet hills, not open ocean) |
| River / stream | `#3F5A5E` | Grey-green; same family as former "sea" swatch |

### Accents

**≤ 5%** of visible surface in any framing.

| Role | Hex | Notes |
|---|---|---|
| Hydrangea | `#6F79B8` | Balconies, gardens |
| Market / crate accent | `#C24B34` | Sparse warm accent (was buoy red; still OK for market goods) |
| Warm interior light | `#E8B26A` | Bar and shop windows |

Darkest neutral: `#2A2622`. No pure black / pure white albedo.

## 4. Scale and camera

Indicative for H2. **Not a gameplay contract.**

| Quantity | Value |
|---|---|
| World unit | 1 unit = 1 metre |
| Character height | 1.75 m |
| Door height | 2.05 m |
| Floor-to-floor | 3.0 m |
| Street width (facade to facade) | 4–6 m |
| Plaza | ~25 × 20 m |
| Modular grid | 1 m; facade modules 2 m / 4 m |

Camera, third person: vFOV 55–60°; pivot 1.6 m; distance 3.5–4.5 m; pitch −8° to −12°.

Readability at 15 m: material, archetype, interactable affordance.

## 5. Characters

Identity = **archetype + outfit + prop + silhouette**. Not facial "Lebaniego realism".

Hero-target archetypes (pick 6 of 8):

| Archetype | Read-at-distance cue |
|---|---|
| Valley worker / farmer | Boots, work coat, crate or tool |
| Bar server | Apron, tray |
| Older neighbour | Long coat, cardigan, trolley/bag |
| Shopkeeper / market stall | Work coat, doorway or stall |
| Teenager on a bike | Single bright accent, bike silhouette |
| Builder | Overall / dusty blue, tool |
| Dog walker | Lead + dog |
| Outsider | Too-clean clothes; reads wrong on purpose |

(Former coastal "fisherman" is demoted; use only if a river/angling beat appears later.)

## 6. Authorized Quaternius packs

Unchanged adoption rules; see pack table in v0.1.2 history. **Ships Pack** stays authorized only if a river craft beat exists — not for coastal harbour identity. Prefer Survival crates, furniture, modular masonry, nature without palms.

Full pack table: keep prior rows; **Ships Pack** note becomes: *optional small river craft only; no harbour identity*.

## 7. Adaptation rules: Quaternius → Liébana / Potes-fiction

| Element | Cantabria-valley rule | Reject if |
|---|---|---|
| **Roof** | Dark wet tile; moderate pitch; eaves | Thatch, chalet pitch |
| **Stone** | Mid stone + darker quoins/damp course | Castle-scale blocks |
| **Vegetation** | Damp green slopes, hedge, mixed Atlantic trees | Palms, tropical, dry scrub |
| **Clothing** | Muted work layers | Fantasy / beach defaults |
| **Sky** | Overcast; horizon meets **hills** | Default hard blue sea-horizon |
| **Water** | River/stream grey-green if present | Tropical sea, beach resort |
| **Props** | Market, bar, workshop, village street | US street kit; harbour as hero |

## 8. H2 hero target

- One plaza (~25 × 20 m) — market/social, not harbour square.
- One to two streets (~40 m) into the plaza.
- One interior (~8 × 6 m) — bar or shop.
- Six archetypes from §5 (valley-weighted).
- Shared humanoid + §12 allowlist.
- **No quay/sea** in hero v1. Optional river glimpse later without expanding material budget carelessly.
- ≤ 120 unique meshes; ≤ 6 material/atlas sets; ≤ ~20 animation clips.

## 9. Asset triage test

Same five steps as before. Additional mental check: **Does this read Potes/Liébana valley, or coastal postcard / fantasy keep?** If the latter, REJECT.

Dry-run: `Docs/evidence/WP-ART-00/TRIAGE_DRY_RUN.md`.

## 10. Licensing

Unchanged: QAL/CC0 research-level only; `DEPENDENCY_IP_POLICY.md` governs adoption.

## 11. Open questions

1. Ships Pack — only if river craft; else ignore.
2. River edge in H2? **Default: no** (same budget logic as old quay deferral).
3. Atlas vs vertex roof recolor — atlas first.
4. Free tier through hero — yes.
5. Palette APPROVED-DRAFT — still OK under valley read.
6. Clip names at H2 import.

## 12. Animation

Unchanged principles: civilian valley town, not action demo. Allowlist: walk/jog/idle/turn, sit, short interact; optional carry. No combat/guns/parkour/swim-as-default/death-in-plaza.

See v0.1.2 §12 for full table.
