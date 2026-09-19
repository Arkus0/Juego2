# Visual references

Working references for `Docs/art/VISUAL_BIBLE.md`. Direction input only — **these are not game assets and are not redistributable material.**

This folder is created empty by `WP-ART-00`. Populating it is part of executing that WP; no images are committed by the WP that defines the structure.

## Structure

| Folder | Store | Do not store |
|---|---|---|
| `01_Town_Fabric/` | Plaza layouts, street grain, block depth, village-scale proportion | City-scale urbanism, aerial maps with no ground read |
| `02_Architecture_Detail/` | Ashlar and rubble stone, tile courses, eaves, balconies and glazed galleries, joinery | Alpine half-timbering, Mediterranean whitewash, castle masonry |
| `03_Vegetation_Ground/` | Wet meadow, hedges, oak and eucalyptus, hydrangea, moss, puddled ground | Dry-climate or tropical species, manicured ornamental gardens |
| `04_Sky_Light_Weather/` | Overcast skies, diffuse light, sea mist, post-rain surfaces, low sun through cloud | Clear-blue skies and hard-shadow noon as target states |
| `05_Characters_Outfits/` | Archetype silhouettes, work clothing, layering for cold damp weather | Identifiable individuals, folkloric costume, fashion editorial |
| `06_Props_Streetlife/` | Street furniture at village scale, bar terraces, fishing tackle, signage | American street furniture, corporate branding, city-scale infrastructure |
| `07_Style_Targets/` | Low-poly and stylized renders or game screenshots showing the shading target | Photoreal renders used as a quality target |
| `08_Anti_Refs/` | Explicit rejections, each labelled with why: photoreal, Alpine, Mediterranean, Dreamcast-as-style, fantasy | Anything unlabelled — an anti-reference without a stated reason is noise |

Anti-references carry as much weight as positive ones. A reviewer who can see what was rejected can apply the bible faster than one who only sees what was chosen.

## Rules

- **Naming**: `NN_topic_source.ext` — e.g. `03_wet-meadow_wikimedia.jpg`.
- **Provenance is mandatory.** Every file records its source and retrieval date, either in the filename or in a `SOURCES.md` line inside its subfolder. A reference with no traceable origin is removed.
- **No unlicensed redistribution.** Prefer freely licensed sources. Never treat a reference image as usable art, texture source or training input.
- **Small files.** References are for judgement, not print. Downscale before committing; this is a documentation repository.
- **No pack contents.** Quaternius meshes, textures and archives never go here. Screenshots of a pack page for identification purposes are fine.
