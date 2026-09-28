# Unity free / low-cost asset audit

Status: **DISCOVERY / NON-CANONICAL**  
Date: 2026-09-28  
Scope: late-discovered free or low-cost Unity tools/content that could materially reduce Juego2 production work after accepted H2F-02.  
Authority: research input only. This document does **not** reopen H2F-02, install packages, adopt dependencies, buy assets, or weaken `Docs/engineering/DEPENDENCY_IP_POLICY.md`.

## Executive conclusion

Accepted H2F already resolved two of the six candidates by capability:

- `Fake Interiors FREE`: **do not adopt now**. H2F-01 selected a project-owned fake-interior Shader Graph/parallax solution as `ADOPT_NOW`; carrying a second implementation would violate the no-unexplained-overlap rule unless a later measured benchmark proves the accepted solution inadequate.
- `FMOD for Unity`: already present in the H2F-01 final disposition matrix as `DEFER`; it was intentionally **not** adopted by H2F-02.

The remaining four candidates were not exact adopted H2F dependencies. They are routed to downstream production WPs rather than changing the accepted foundation.

| Candidate | Current public identity observed | Verdict for Juego2 now | Primary owner | Why |
|---|---|---|---|---|
| UModeler X | Free; Unity Asset Store Extension Asset; current listing `UModeler X Hub 1.0.5`, 2026-09-16; supports Unity 6000.3.5+ and URP; Editor-only | **SPIKE AFTER H2F-GATE** | `WP-ART-ENV-02` | Strongest new candidate. Could reduce Blender round-trips for Quaternius donor/component editing, UV, painting and bounded mesh fixes. Must prove material authoring savings on real Juego2 source content before adoption. Blender remains the accepted DCC baseline until such proof exists. |
| Fake Interiors FREE | `1.5.10r2`, 2026-08-06; free; Standard Asset Store EULA / Extension Asset; URP-compatible | **NO_ADOPT / REDUNDANT** | none unless later failure | Exact capability already has an accepted project-owned Shader Graph implementation. Reconsider only after an A/B benchmark shows a material visual, performance or authoring advantage over the accepted solution. |
| Human Basic Motions FREE | `2.4.2`, 2026-04-30; free; Standard Asset Store EULA / Extension Asset; Humanoid/Mecanim; public pack describes 118 animation files | **WATCH / GAP-FILLER ONLY** | `WP-ART-ANIM-01` | H2F already selected UAL1 and UAL2 and found Mixamo redundant because there was no named civilian-motion gap. Do not ingest another locomotion/talk library by default. Use only if the ART animation coverage matrix identifies a concrete gap or clearly superior clip. |
| IK Helper Tool | `1.0.2`, 2026-06-09; free; Standard Asset Store EULA / Extension Asset | **SPIKE IF AUTHORING GAP** | `WP-ART-ANIM-01` | Runtime/keeper IK already belongs to Unity Animation Rigging 1.4.1. This candidate is only interesting as an editor/retarget helper if it measurably reduces repetitive Humanoid/IK preparation on the accepted Quaternius rigs. It must not become a second runtime IK owner. |
| AERO - Volumetric Fog and Mist | `1.8.0`, 2026-06-13; free; Standard Asset Store EULA / Extension Asset; URP-only; listing targets Unity 6000.3.15f1 | **DEFER / NO_ADOPT NOW** | later final-look owner (`ART-03`, with bounded input from environment art) | H2F deliberately selected native fixed sky + exp2 fog + Volume preset and no weather suite. AERO may offer a meaningful atmospheric upgrade for the port town, but it would add a new render/runtime dependency. Test only if native fog is shown to be a final-look limitation, with visual/performance A/B evidence and full dependency/lifecycle admission. |
| FMOD for Unity 2.03 | current UPM listing `2.3.14`, 2026-06-28; supports Unity 6000.3; free for indies under FMOD licensing; Asset Store marks it Restricted Asset governed by FMOD EULA | **DEFER — already H2F decision** | future dedicated audio WP | High potential for adaptive music/ambience, especially with a project musician, but it is not a small helper: FMOD is an audio middleware/authoring stack and may replace substantial Unity-audio ownership. Adopt only in a dedicated audio decision WP with exact licensing, provisioning, save/lifecycle and replacement boundaries. |

## Relationship to accepted H2F

`WP-H2F-02` is COMPLETE / ACCEPTED and must not be retroactively widened merely because new marketplace candidates were discovered later.

The accepted H2F-01 disposition matrix already established:

- project-owned fake interior via Shader Graph/parallax = `ADOPT_NOW`;
- UAL1 = `ADOPT_NOW`, UAL2 = `AVAILABLE_ASSET`, Mixamo = `REDUNDANT`;
- Animation Rigging = `ADOPT_NOW`;
- fixed native sky/fog/light presets = `ADOPT_NOW`, external weather suites not required;
- Blender source-asset/DCC = `ADOPT_NOW`;
- FMOD = `DEFER`.

Therefore late candidates may only enter through a downstream causal owner when they either fill an observed gap or prove a material production saving without creating duplicate authority.

`WP-H2F-03` should **not** become the place to introduce these new choices: its job is to prove the accepted H2F baseline, not reopen selection.

## Candidate-specific audit notes

### UModeler X — highest-priority late spike

Observed public facts:

- free full modeling/UV/painting/rigging feature set;
- Editor-only; built outputs do not need a runtime UModeler SDK;
- imported meshes / marketplace assets can be edited in Unity;
- Unity 6000.3.0f1–6000.3.4f1 are explicitly unsupported because of a Unity regression, while 6000.3.5f1+ is supported;
- current Juego2 H2F evidence uses Unity 6000.3.24f1, so the documented editor-version blocker does not apply;
- free edition collects basic usage telemetry that cannot be disabled; the listing says no project/asset content is collected.

Bounded spike for `WP-ART-ENV-02`:

1. import/use only through lawful local provisioning;
2. take one real Quaternius environment donor/component that currently requires a Blender round-trip;
3. perform the same bounded adaptation in UModeler X and in the accepted Blender path;
4. compare operations, reproducibility, mesh/material/prefab output, provenance traceability and Astra/fresh-author usability;
5. require plain Juego2-owned mesh/material/prefab outputs to remain usable after removing the editor tool;
6. reject adoption if keeper/source truth becomes trapped in UModeler-private state or the saving is marginal.

If it wins materially, admit the exact version as an **editor authoring helper**, not as visual/semantic authority.

### Fake Interiors FREE — useful product, wrong current decision

The package is current and URP-compatible, but Juego2 already owns the same capability through the accepted H2F project shader. H2F explicitly required duplicate fake-interior candidates to be rejected when the simpler admitted route satisfies the need.

No install is justified now. A future comparison is legal only if the accepted implementation becomes a measured quality/performance/authoring blocker.

### Human Basic Motions FREE — source reserve, not another baseline

The free pack is broad and Humanoid-friendly, but most of its headline coverage overlaps the already admitted UAL locomotion/talk baseline. `WP-ART-ANIM-01` is already designed to identify concrete functional gaps before acquiring more clips.

Treat this as a lawful **gap-filler candidate**. Do not import the whole library merely because it is free.

### IK Helper Tool — editor utility only

Potential value is reducing repetitive Mecanim/retarget/IK preparation. Runtime contact correction remains owned by accepted Unity Animation Rigging. A spike is justified only if `ART-ANIM-01` records repeated manual authoring work that this tool removes on actual Quaternius rigs.

### AERO — visual opportunity, foundation risk

AERO is unusually relevant to the damp northern-port identity, but the accepted foundation intentionally froze a simpler native fog/Volume solution. Adding volumetric fog before a measured need would reopen a render choice that H2F already closed.

If final visual-production work demonstrates that native fog materially limits the target presentation, compare AERO against the accepted baseline on the same street/port scene with:

- third-person captures;
- GPU/frame cost under the H2 target hardware/profile;
- day/evening/night consistency;
- camera/interior transition behaviour;
- clean-rebuild/provisioning proof;
- fallback/removal path.

### FMOD — strong later candidate, not a casual package

FMOD remains a serious future candidate because Juego2 needs dense environmental ambience, interiors/exteriors, time-of-day variation and adaptive music. However, it is middleware with its own authoring tool, banks, licensing and runtime integration. That deserves a dedicated audio architecture/adoption decision, not opportunistic installation during visual H2.

## Routing / no-reopen rule

- `WP-H2F-02`: **no reopen**.
- `WP-H2F-03` / `WP-H2F-GATE`: consume and prove the already accepted baseline; do not add these packages.
- `WP-ART-ENV-02`: mandatory consideration of UModeler X before inventing equivalent bespoke in-Unity mesh-edit tooling; Fake Interiors remains redundant unless a measured blocker exists.
- `WP-ART-ANIM-01`: Human Basic Motions FREE and IK Helper Tool are candidate gap-fillers/helpers, never automatic imports.
- `WP-ART-03`: may trigger an A/B of AERO only if native fog is a demonstrated final-look limitation.
- future audio owner: re-evaluate FMOD with the exact then-current package/license.

## Sources observed on 2026-09-28

External:

- UModeler X Asset Store: https://assetstore.unity.com/packages/tools/modeling/umodeler-x-330215
- UModeler X product/download: https://assetstore.unity.com/umodeler-x ; https://unity.umodeler.com/download
- Fake Interiors FREE: https://marketplace.unity.com/packages/vfx/shaders/fake-interiors-free-104029
- Human Basic Motions FREE: https://marketplace.unity.com/packages/3d/animations/human-basic-motions-free-154271
- Human Basic Motions free pack details: https://keviniglesias.gumroad.com/l/human-basic-motions-free
- IK Helper Tool: https://assetstore-fallback.unity.com/packages/tools/animation/ik-helper-tool-148408
- AERO: https://marketplace.unity.com/packages/vfx/shaders/fullscreen-camera-effects/aero-volumetric-fog-and-mist-277702
- FMOD for Unity 2.03 UPM: https://assetstore.unity.com/packages/package/1082237
- FMOD Unity integration docs: https://fmod.com/docs/2.03/unity/

Internal accepted evidence / contracts:

- `Docs/evidence/WP-H2F-01/DISPOSITION_MATRIX.csv`
- `Docs/workpacks/H2F/WP-H2F-01.md`
- `Docs/workpacks/H2F/WP-H2F-02.md`
- `Docs/workpacks/ART/WP-ART-ENV-02.md`
- `Docs/workpacks/ART/WP-ART-ANIM-01.md`
- `Docs/engineering/DEPENDENCY_IP_POLICY.md`

## Non-decisions

This audit does not install any package, acquire any Asset Store entitlement, accept any third-party EULA on the owner's behalf, create a runtime dependency, reopen H2F, or authorize a purchase. Every actual adoption still requires the causal WP, exact-version review and project dependency/IP policy.