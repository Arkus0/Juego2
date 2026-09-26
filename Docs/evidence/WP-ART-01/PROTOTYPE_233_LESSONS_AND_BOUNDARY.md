# PR #233 diagnostic lessons consumed by ART-01

PR #233 `prototype/demo-puente-bar` remains a frozen, non-canonical experiment. ART-01 read its final captures/report and the named HouseBuilder/Layout/Heights/TerrainBuild/WallBuild/Kit/MatLib/CharacterKit/Interiors/Dressing/SignKit/postprocessor/import/Blender files from a **separate detached read-only checkout** at `51940d8c6b72a4e98b0bbeebd51973cc6d3e2f63`. No prototype file, scene, seed, route layout or world coordinate was edited or copied into the ART worktree.

| Diagnostic finding in #233 | ART-01 action |
|---|---|
| Centimetre-scale FBX geometry, Props root ×100 and X-axis orientation | Selected-source lock, before/after Unity audit, scoped importer normalization and placement that preserves prefab transform. |
| Standard source materials incompatible with prototype URP | Owner decision 2026-09-26: URP is the production renderer. The STRUCTURAL checkpoint stays renderer-independent on the built-in H1 project (explicit Standard remap, provisional). After `WP-H1-GATE`, ART-01 adopts URP and performs an explicit, reproducible, fail-closed material conversion; #233's URP 17.3.0 is only a starting reference, not an imported configuration. |
| HouseBuilder showed the source can assemble genuine opened walls | ART composes selected source hosts/inserts and adds its own smooth extruded render hosts, plinth, measured eave/gable and threshold rules. No Bar/Casco special layout rule was transplanted. |
| Source roof slope/chalet read, upper facade framing | Named flattened source-roof derivatives, dark tile palette, smooth render host with real holes, stone lower band; incompatible direct source use rejected. |
| Buildings floated without contact and route edges were unclear | Continuous plinth/gap, named entrance landing and two steps, singly collidable road, kerb/drain, capped retaining wall, scenic bank without path collision. |
| Signs, props and dressed characters need pivot/material/host treatment | Two readable sign faces; selected bounded props; one CC0 clothed scale derivative from a bounded adaptation of the prototype's Blender technique. |

**Kept outside ART-01:** #233's `Seed.cs` place coordinates, layout of the bridge/town/bar, `Layout.cs` town placement, `Heights.cs` or `TerrainBuild.cs` world-height solving, localized regeneration, semantic building identity, special Bar/Casco composition, `Interiors.cs` venue systems, character variants/jobs/schedules and any player gameplay. These are CITY-07/H2/H3 questions requiring their own accepted contracts. ART's scene uses local specimen stations and records the accepted width classes; it is not a disguised replacement keeper or world compiler.
