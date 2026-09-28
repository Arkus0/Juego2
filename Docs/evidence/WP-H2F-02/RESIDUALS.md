# Residuals and handoffs

Allowed residuals under the WP: final artistic tuning (ART-01/CITY-07/ART-02), later profiling-driven optimization, categories deferred by H2F-01/H2F-01A, and separately licensed GC2 modules. Every item below names its owner. None of them is a tooling choice left open for H2F-03.

| # | Residual | Class | Owner |
|---|---|---|---|
| R1 | `activeInputHandler = Both` stays because two accepted debug/benchmark tools read the legacy Input Manager: the CITY-04 traversal probe, and ART-01's walk inspector once ART-01 rebases. No other code may use it (`J2_LEGACY_INPUT_IN_JUEGO2_CODE`). Switch to Input System only once both are retired or ported. | transition | CITY-07 (probe), ART-01 (inspector) |
| R2 | ART-01 rebase obligations on this baseline: pipeline-aware `Art01Materials`, UAL1 loop rule converged to `_Loop`, walk inspector input (`IMPORT_CONVENTIONS.md`). | handoff | ART-01 |
| R3 | The tee-junction generator covers perpendicular branches (≤ 12°). Oblique branches, crossroads and plazas are authored meshes or a later realizer extension. | capability extension | CITY-07 / CITY-URBAN-01 when a real layout needs it |
| R4 | Fulcrum footstep cadence on UAL walk is about half GC2's curve-annotated cadence (01A). The footstep sounds asset needs ART textures and audio. | tuning | H2F-03 / H4 |
| R5 | Play-mode behaviour of the materialized S06 presets on the adopted project (drive, camera framing at thresholds) is not re-proved here. 01A C02g/C07 proved it on the same Core version and presets. H2F-02 proves edit-time materialization, lints, identity audit and a player build. | consumed + downstream | H2F-03 (integrated fixture), GC2-00 (round trip) |
| R6 | GC2 `core.*` settings are generated at the first windowed editor session (01A). Batch evidence does not create them; they are git-ignored and regenerated. | expected | — |
| R7 | Hosted CI never has GC2 bytes (EULA), so hosted runs prove the Core-absent configuration and the H1 suites. Core-present evidence is owner-workstation evidence re-read by the verifier. | by design | — |
| R8 | Local runs of the H1 suites need the private vault mount and host-prepared plans. The hosted H1 workflows are the authoritative H1 evidence. | environment | — |
| R9 | The supplementary H1-11 rendered capture now differs from the committed built-in capture. It is informational only and never the parity oracle. | expected | H1 capture refresh, if ever wanted |
| R10 | Final lighting, material tuning and camera feel are baseline values from H2F-01 S01/S06, not keeper visuals. | allowed | ART-01 / CITY-07 / ART-02 |
| R11 | Profiling and performance budgets. | allowed | H2F-03 (diagnostic snapshot), later WPs |
| R12 | UAL2 per-clip allowlist: the library is admitted by H2F-01's lock and follows the import rules; which clips a feature uses is decided by its first consumer. | handoff | H2-03 / GC2-00 |
| R13 | Separately licensed GC2 modules (Dialogue, Inventory, Behavior, Perception, Melee, Quests, …) are not installed; each needs its own adoption WP. GC2 save host is H6. | deferred | GC2-03+ / H6 |
| R14 | `J2ShaderAudit` classification is structural (subshader `RenderPipeline` tag, URP LightModes, untagged passes = `SRPDefaultUnlit`, pipeline-agnostic builtins). Rendered captures are the human cross-check. | method limit | — |
| R15 | Scatter exclusion margin is measured from no-go colliders. Kerbs carry no collider by design (NavMesh), so the margin must exceed kerb width; the default 0.6 m ≥ 0.2 m kerb. | documented rule | owners of scatter profiles |
| R16 | Owner inspection: the ART-01 benchmark houses still look like glued modules (corners, windows mounted on walls, door frames not straight). This is identical under built-in, so it is ART-01's structural assembly, not the migration. Owner decision: "Congelar y derivar a ART-01". | owner-routed | ART-01 (before `KEEPER_READY`) |
| R17 | Signage and typography (TextMesh sign on the benchmark). Owner: "Carteles iran en otro wp". | owner-routed | separate WP (owner) |
| R18 | Source→ART-palette mapping for ferns: the tree-leaf texture on fern cards reads as floating paper. | art tuning | ART |
