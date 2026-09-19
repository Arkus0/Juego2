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

## 4–5. Scale / characters

Streets 4–6 m; plaza ~25×20 m; grid 1 m. Identity = archetype + outfit + silhouette (valley worker, bar server, neighbour, shopkeeper, teen+bike, builder, dog walker, outsider).

## 6–7. Packs and adaptation

Quaternius free tier; modular masonry + nature without palms; Survival crates; Ships only for later river craft. Dark wet tile, mid stone + damp course, overcast hills horizon, market props — reject thatch, chalet, castle, tropical, US downtown blocks.

## 8. H2 hero target

Plaza + 1–2 streets + bar/shop interior; 6 NPCs; no sea/quay in v1; ≤120 meshes; ≤6 atlases; ≤~20 anim clips.

## 9–12. Triage, licensing, animation

Triage dry-run: `Docs/evidence/WP-ART-00/TRIAGE_DRY_RUN.md`. License: UNVERIFIED-FOR-ADOPTION until human pack record. Animation: civilian allowlist only (walk/jog/idle/sit/interact).
