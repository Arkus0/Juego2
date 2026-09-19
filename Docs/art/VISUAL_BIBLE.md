# Visual Bible — Juego2

Version: 0.1 — 2026-09-19  
Status: DRAFT — seed draft, pending human approval under `WP-ART-00`  
Scope: art direction and asset selection only. **This document does not block, gate or modify H0.** No `HK-*` acceptance criterion depends on it.

## 1. Style one-liner

A low-poly, stylized Atlantic-coastal Cantabrian village — wet stone, dark tile, saturated damp green under a high overcast sky — built from a restricted Quaternius-compatible kit, readable at 15 metres, with the shape language of a late-PS2 / early-PS3 game reinterpreted rather than emulated.

Three looks this excludes outright: photorealism; the Dreamcast/Shenmue graphical style (its lived-in-world *feeling* is kept, its rendering is not); and the generic sunny Mediterranean village (whitewash, flat blue sky, terracotta warmth).

## 2. Yes / No

### Yes

- Flat or lightly gradient albedo, atlas-driven, consistent with how Quaternius packs are authored.
- Chunky readable silhouettes; detail carried by shape and color, not by texture resolution.
- Humidity as the dominant mood: darker value range, lower contrast, green bias in shadow.
- Stone as the primary wall material; dark tile as the primary roof material.
- Overcast, diffuse key light as the default weather; sun is an event, not the norm.
- A village that looks inhabited: laundry, crates, a bar terrace, fishing tackle, worn kerbs.
- Modular reuse: the same six wall modules recolored beat sixty unique ones.

### No

- Photorealism, or "top-tier realistic PS3" as a target.
- PBR detail maps, roughness/metallic authoring or normal-mapped micro-detail layered onto flat atlas assets.
- Dreamcast/Shenmue as a *graphical* reference, or any hardware-nostalgia rendering trick (dithering, fixed low-res buffer, PS1 vertex jitter, affine texture warping).
- Alpine or Germanic half-timbering; steep chalet roofs.
- Mediterranean whitewash-and-cobalt; terracotta warm-ochre ground.
- Fantasy: thatched roofs, castles, palisades, ruins-as-fantasy, medieval signage, weapons.
- American road language: wide multi-lane asphalt, yellow center lines, stop signs, fire hydrants.
- Skyscrapers, glass curtain walls, downtown blocks.
- Palms, cacti, tropical foliage, or a mass of autumn red.
- Realistic "Cantabrian" facial physiognomy for characters (see §5).
- Regional caricature: no costume-folklore villagers, no stereotype props used as a joke.

## 3. Palette (draft)

Hex values are **albedo targets in sRGB, before lighting**. Lit results will read darker and cooler under the default overcast key. This palette is a draft pending human approval.

### Stone and walls

| Role | Hex | Notes |
|---|---|---|
| Light stone | `#C9C3B6` | Dressed ashlar catching sky light |
| Mid stone | `#A79F92` | Default wall value |
| Shadowed stone / quoins | `#7E7568` | Corner stones, lintels, wet lower courses |
| Dirty render / limewash | `#E6E1D6` | Used sparingly — never a whole facade run |

### Roof

| Role | Hex | Notes |
|---|---|---|
| Tile, base | `#6B4A3E` | Default roof value |
| Tile, dry | `#8A5E4C` | Sunlit or sheltered pitches only |
| Tile, wet | `#4A3730` | Dominant after rain; safest default for hero shots |

### Carpentry and metal

| Role | Hex | Notes |
|---|---|---|
| Dark timber | `#5C4433` | Doors, beams, balcony rails |
| Shutter green-blue | `#3E5A57` | The signature joinery color; keep it consistent village-wide |

### Vegetation and ground

| Role | Hex | Notes |
|---|---|---|
| Deep wet green | `#2F4A32` | Shadowed foliage, hedge interiors |
| Mid meadow green | `#4E7A43` | Default grass |
| Lit grass | `#7BA35A` | Highlights only — never a full field |

### Sky and water

| Role | Hex | Notes |
|---|---|---|
| Overcast, high | `#B9C4C9` | Zenith |
| Overcast, low | `#93A3AC` | Horizon |
| Cantabrian sea | `#3F5A5E` | Grey-green, never tropical blue |

### Accents

Accents are bounded: **no more than 5% of visible surface in any framing.**

| Role | Hex | Notes |
|---|---|---|
| Hydrangea | `#6F79B8` | Balconies, front gardens |
| Buoy / net red | `#C24B34` | Harbour and fishing props |
| Warm interior light | `#E8B26A` | Bar and shop windows; the only warm value at street level |

Darkest neutral permitted: `#2A2622`. Pure black and pure white are not used as albedo.

## 4. Scale and camera

Indicative for the H2 hero target. **Not a gameplay contract** — camera and movement tuning belong to H2, not here.

| Quantity | Value |
|---|---|
| World unit | 1 unit = 1 metre |
| Character height | 1.75 m |
| Door height | 2.05 m |
| Floor-to-floor | 3.0 m |
| Street width (facade to facade) | 4–6 m |
| Plaza | ~25 × 20 m |
| Modular grid | 1 m, with facade modules at 2 m and 4 m |

Camera, third person: vertical FOV 55–60°; pivot at 1.6 m; distance 3.5–4.5 m; default pitch −8° to −12°.

Readability rule: a building's material, a character's archetype and an interactable's affordance must all be identifiable at 15 m at this FOV. If an asset only reads up close, it is background dressing, not a hero asset.

## 5. Characters

Identity comes from **archetype + outfit + prop + silhouette**. It never comes from facial detail, and it never attempts realistic regional physiognomy — that goal is out of scope, technically unsupported by the kit, and creatively unnecessary.

Rules:

- Build NPCs from the modular character packs (§6) by swapping parts and recoloring, not by sculpting new heads.
- Faces stay at pack fidelity. No unique facial likenesses, no real-person references.
- Two NPCs sharing a base mesh must still be distinguishable at 15 m by outfit value and one silhouette-breaking prop.
- Outfit colors draw from §3; work clothing uses the muted stone/timber ranges, with at most one accent per character.
- No folkloric costume, no dialect-as-visual-joke, no character whose design exists to mark them as rural or provincial.

Hero-target archetypes (6 of these 8):

| Archetype | Read-at-distance cue |
|---|---|
| Fisherman | Oilskin value block, boots, coiled rope or crate |
| Bar server | Apron, tray, short sleeves in cold weather |
| Older neighbour | Long coat, headscarf or cardigan, shopping trolley |
| Shopkeeper | Overall or work coat, standing in a doorway |
| Teenager on a bike | Bright single accent, bicycle silhouette |
| Builder | Hi-vis or dusty blue overall, hard hat, tool |
| Dog walker | Lead and dog extend the silhouette laterally |
| Outsider | Clean, unweathered clothing; reads wrong on purpose |

## 6. Authorized Quaternius packs

**Verification status legend** — `URL-CONFIRMED`: pack page confirmed to exist, contents summarized from search results, **not** read first-hand; `HYPOTHESIS`: believed to exist, name unconfirmed; `REJECTED`: not to be used.

A `URL-CONFIRMED` row is not an adoption approval. See §10.

| Pack | Status | Authorized for | Forbidden from this pack |
|---|---|---|---|
| Ultimate Modular Men Pack | URL-CONFIRMED | NPC bases, modular part swapping, animation set | Fantasy and sci-fi variants; using a character unaltered as a named NPC |
| Ultimate Modular Women Pack | URL-CONFIRMED | NPC bases, modular part swapping | Same as above |
| Universal Base Characters | URL-CONFIRMED | Base mesh when a custom outfit is needed | Shipping a base character with no outfit pass |
| Medieval Village Pack | URL-CONFIRMED | Stone wall modules, doors, windows, small props | Thatched roofs; half-timbering; castle, keep or tower pieces; wooden palisade; medieval signage |
| Medieval Village MegaKit | URL-CONFIRMED | Modular masonry, kerbs, stairs, fences | Same vetoes as above, plus whole prefabricated village blocks used unedited |
| Modular Medieval Building Pack | URL-CONFIRMED | Facade modules at 2 m / 4 m | Half-timbered variants; any piece whose roof pitch exceeds the tile rule in §7 |
| Ultimate Buildings Pack | URL-CONFIRMED | Contemporary infill blocks; atlas recoloring | Glass curtain walls; anything above three storeys |
| Modular Streets Pack | URL-CONFIRMED | Carriageway, pavement, kerb, drain | Multi-lane highway pieces; American lane markings; traffic signals |
| Downtown City MegaKit | URL-CONFIRMED | **Loose props only**: bench, streetlight, bollard, litter bin, drain cover | Whole blocks; brownstone or NYC/Boston facades; any building mass |
| Ultimate Nature Pack | URL-CONFIRMED | Trees, rocks, grass, hedges | Palms, cacti, tropical species; conifer-only massing; autumn-red variants in bulk |
| Ultimate Stylized Nature Pack | URL-CONFIRMED | Higher-fidelity vegetation for foreground | Mixing its normal-mapped look into flat-atlas hero facades |
| Ultimate House Interior Pack | URL-CONFIRMED | Bar and shop interior: doors, windows, kitchen, fittings | Suburban American set dressing that reads non-European |
| Ultimate Furniture Pack | URL-CONFIRMED | Tables, chairs, shelving, counter | Modern luxury or designer furniture |
| Ultimate Modular Ruins Pack | URL-CONFIRMED | Optional ruined chapel or boundary wall | Fantasy ruins; classical columns; anything monumental |
| Survival Pack | URL-CONFIRMED | Crates, barrels, rope, sacks | Weapons; camping and wilderness gear |
| Ships / small boats pack | **HYPOTHESIS** | Moored boats for the quay, if the quay is in scope | Galleons, pirate ships, cruise or military vessels. Confirm the exact pack name at quaternius.com before relying on it |
| Modular Character Outfits – Fantasy | **REJECTED** | — | Entire pack: fantasy outfits are out of style |

Cross-pack rule: assets from different packs may share a scene only when their shading style matches after recoloring. A normal-mapped asset next to a flat-atlas asset on the same facade is a rejection, not a compromise.

## 7. Adaptation rules: Quaternius → Cantabria

| Element | Quaternius default | Cantabria rule | Mechanism | Reject if |
|---|---|---|---|---|
| **Roof** | Bright or orange tile, sometimes thatch or shingle; varied pitch | Dark tile, wet-biased (`#4A3730`–`#6B4A3E`); moderate pitch; deep eaves overhanging the facade | Recolor atlas; swap the mesh where pitch is chalet-steep | Thatch, shingle, chalet pitch, or a roof with no eave overhang |
| **Stone** | Clean light blocks, uniform value, sometimes fantasy-scaled | Stone is the default wall; visible dressed quoins and lintels against a rougher wall field; lower courses darker (damp wicking) | Recolor to `#A79F92` field with `#7E7568` quoins; vertex-darken the bottom ~0.5 m | Uniform untouched value across a whole facade, or block scale that reads as castle masonry |
| **Vegetation** | Bright saturated green, dry-climate massing, mixed exotic species | Dense damp green, low value, oak/eucalyptus/hedge massing; hydrangea as the one flowering accent; moss at wall bases | Recolor to the §3 green ramp; delete non-Atlantic species; scatter moss decals or vertex tint | Palms, cacti, tropical forms, or a green brighter than `#7BA35A` covering more than a highlight |
| **Clothing** | Saturated primaries, fantasy or generic-modern cuts | Muted work clothing from the stone/timber ranges; at most one accent per character; weather-appropriate layers | Recolor material slots; swap part meshes across the modular packs | More than one accent per character, bare arms as a village default, or a fantasy silhouette |
| **Sky** | Clear blue gradient or stylized sunset | High overcast as the default: `#B9C4C9` zenith to `#93A3AC` horizon, diffuse key, soft shadows, no hard sun disc | Replace skybox/gradient; lower directional intensity; raise ambient | A clear blue sky or a hard-shadow sunny setup used as the default state |
| **Water / quay** | Tropical blue-cyan, flat plane | Grey-green (`#3F5A5E`), choppy, opaque rather than transparent-tropical | Recolor material; reduce transparency and specular | Turquoise, visible sandy seabed, or a calm mirror surface |
| **Props / street life** | Generic or American street furniture | European village scale: narrow pavement, short bollards, small bins, bar terrace, fishing tackle near water | Select loose props only; rescale to the 1 m grid | American-coded props (hydrants, mailboxes, yellow school signage) or props at city rather than village scale |

## 8. H2 hero target

The validation slice. Building it proves the bible; nothing here authorizes starting it before the Unity parity gate.

- One plaza, ~25 × 20 m.
- One to two streets, ~40 m each, connecting to the plaza.
- One simple interior, ~8 × 6 m — a bar or a shop, enterable.
- Six archetype NPCs from §5.
- Optional, pending §11: a short quay with two moored boats.

Budget: **≤ 120 unique meshes** and **≤ 6 material/atlas sets** across the whole slice. Exceeding either budget means the slice is being built from unique assets rather than from a kit, which is the failure this bible exists to prevent.

## 9. Asset triage test

Apply to any candidate Quaternius asset. Produces exactly one verdict.

1. **Pack check.** Is the asset's pack listed in §6 and not `REJECTED`, **and does the asset fall inside that row's "Authorized for" column**? No to either → **REJECT**. A pack being authorized never authorizes all of its contents.
2. **Per-pack veto.** Does it fall under that pack's forbidden column? Yes → **REJECT**.
3. **No-list check.** Does it match any entry in §2 "No"? Yes → **REJECT**.
4. **Shading match.** After recoloring, will it sit on-screen beside already-accepted assets without a visible authoring-style break (flat atlas vs normal-mapped, detail density, silhouette chunkiness)? No → **REJECT**.
5. **Adaptation check.** Read §7 for every element the asset contains. If each element already satisfies its Cantabria rule → **ACCEPT**. If each failing element is fixable by recolor, part swap, hiding sub-meshes or rescaling to the 1 m grid → **RECOLOR-THEN-ACCEPT**, and record which mechanism. If any element fails and cannot be fixed by those four mechanisms → **REJECT**.

Modelling new geometry is not one of the mechanisms. If an asset needs new geometry to fit, it is a reject for this slice.

## 10. Licensing

Quaternius publicly states its packs are CC0: commercial use permitted, attribution not required, modification permitted; the paid *Source* tiers carry the same terms. This is **research-level observation only** — status: `UNVERIFIED-FOR-ADOPTION`.

`Docs/engineering/DEPENDENCY_IP_POLICY.md` governs: a license claim seen during research does not satisfy the adoption gate. Before any pack is downloaded into a project, a human must read the license line on that pack's own page at that version and record: pack identity, version or download date, license as stated there, source URL, linked/vendored/copied classification, and notice obligations. Unknown or ambiguous terms fail closed.

Practical rule kept regardless of CC0: **do not redistribute a Quaternius pack as a pack.** Assets ship inside the game; the kit does not ship as a kit.

## 11. Open questions

1. What is the exact name of the Quaternius boats/ships pack, and does it contain small working craft rather than only large vessels? (§6, `HYPOTHESIS`.)
2. Is the quay inside the H2 hero target or deferred? It adds water shading and a second material set to a slice budgeted at six. (§8.)
3. Is roof recoloring done by editing the shared atlas or by vertex color? Atlas editing changes every pack asset at once; vertex color is per-instance. This decides how much Blender work H2 needs. (§7.)
4. Are any paid *Source* tiers needed, or is the free tier sufficient through the hero target? Current assumption: free tier is sufficient; Source only if heavy mesh editing proves necessary.
5. Palette hex values in §3 are a draft and need human sign-off, ideally against a flat color-block sheet rather than in text.
