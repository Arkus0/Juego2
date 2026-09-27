# WP-H2F-02 — Exact adoption + URP/toolchain bootstrap

Status: **PROPOSED / NOT_STARTED**  
Class: `PRODUCT_CHECKPOINT` with dependency-policy adoption controls  
Mode: **LOCAL_UNITY_REQUIRED where licensed/source bytes or visual import evidence are local-only; hosted evidence may cover reproducible machine-verifiable portions**  
Depends on: `WP-H2F-01` PASS + `WP-H1-GATE` PASS + `WP-ART-01` PASS  
Blocks: `WP-H2F-03`

## Claim

The H2F-01 selected stack is installed/admitted in the real Juego2 Unity project with exact versions/provenance, a working URP baseline, reproducible project configuration and explicit replaceable boundaries before keeper CITY-07 production begins.

## PASS-before-work acceptance contract

- **Positive claim:** the selected baseline has become explicit project truth without breaking accepted H0/H1 authority or silently relying on one developer machine.
- **Mandatory positive evidence:** (1) exact Unity package manifest/lock state; (2) exact external dependency/source adoption records required by `DEPENDENCY_IP_POLICY`; (3) URP migration/import evidence on representative real assets; (4) project/preset/import conventions; (5) documented manual/account-gated provisioning steps; (6) clean compile/import after the selected stack is present; (7) replacement/authority boundary for every material dependency; (8) a complete selected-foundation-state → H1 lifecycle classification matrix defining host, observation/reconciliation treatment and rebuild expectation for every material serialized/generated state introduced by the adopted stack.
- **Negative gates:** FAIL if dependency versions float; if exact license/EULA cannot be established; if unsupported/magenta/fallback materials are hidden; if local Asset Store/source bytes are assumed to exist without provisioning documentation; if a plugin-private identifier becomes canonical world semantics; if selected tools create a mutation path around Arkus authoring authority; or if material selected foundation state has no declared H1 lifecycle classification/host and could therefore be silently deleted, falsely reported as unsupported drift or made unrebuildable.
- **Non-claims:** no CITY-07 keeper realization, no final visual-polish claim and no H2 NPC/Living World claim.
- **Allowed residuals:** final artistic tuning owned by CITY-07/ART-02; later profiling-driven optimization; deferred categories from H2F-01.
- **Evaluation method:** exact-version/config inspection + Unity compile/import + targeted human visual inspection of migrated real content + dependency-policy review + H1 lifecycle-boundary classification review.
- **Consumed predecessors:** H2F-01 selection, H1 accepted bridge/materialization boundary, ART-01 accepted production-content truth.

## Required work

### A. Exact dependency/content adoption

For every `ADOPT_NOW` material package/tool/source:

- pin the supported version/commit where the ecosystem permits;
- record exact license/EULA/commercial terms at that version/acquisition;
- record source URL/provider and linkage/distribution mode;
- classify runtime/editor/build-only/content-source role;
- record notices/attribution obligations;
- record update/security owner and replacement strategy;
- identify which Juego2/Arkus guarantee it assists and which guarantees remain outside its authority.

For `AVAILABLE_ASSET` content sources, record enough acquisition/provenance identity to make future retained derivatives traceable without pretending the content source is a runtime dependency.

### B. URP project baseline

Migrate/configure the real Unity project to the selected URP baseline and record the settings that future Workers must inherit.

At minimum close where applicable:

- render pipeline asset(s) and quality tiers;
- renderer features selected by H2F-01;
- Shader Graph/material conventions;
- color/tonemapping and global Volume baseline;
- shadow policy;
- probe/GI/APV policy;
- SSAO/decal policy;
- transparency/cutout/vegetation expectations;
- representative Quaternius Source/custom shader migration;
- unsupported/fallback shader handling;
- scene/camera assumptions needed by the selected stack.

H2F-02 owns a baseline, not final scene lighting.

### C. Import and source-content conventions

Freeze project-wide conventions needed before CITY-07/H2 authors retained content:

- units/scale and model import defaults;
- material/texture import defaults where appropriate;
- source-vs-derived asset folder/provenance conventions;
- prefab/variant conventions where the selected tooling depends on them;
- Humanoid avatar/retarget import rules;
- animation clip/root-motion conventions;
- LOD/collider/nav-related defaults that are truly foundation-wide;
- selected terrain/spline/vegetation/water/fake-interior adapter or preset locations;
- generated/transient outputs that must not become source authority.

Do not overwrite ART-owned visual semantics with generic engine defaults.

### D. AI-native adapter/preset boundary

For selected tools that CITY/H2/Astra will invoke repeatedly, provide the smallest stable Juego2-side adapter, preset or documented projection boundary needed to avoid authoring raw plugin-private state as the semantic contract.

Examples include a spline realization profile, vegetation placement/exclusion profile, water/shoreline profile, character retarget profile or navigation surface preset. Do not build speculative abstraction layers around tools that are not actually selected.

### E. Reproducible provisioning

Classify every input as one of:

- repository/package-manager reproducible;
- external free source with pinned acquisition/provenance;
- account/licensed manual acquisition required;
- local derivative generated from an admitted source.

Document the smallest lawful restoration path. Never commit/store third-party bytes contrary to their license merely to make automation easier.

### F. H1 lifecycle classification for selected foundation state

Before H2F-03, enumerate every **material state introduced by the selected stack that must survive or be deterministically reconstructed for keeper work**. This is a boundary declaration, not an expansion of H1 to understand arbitrary plugin internals.

For each state family, publish one row with at least:

| Selected foundation state | Lifecycle class / host | Source of truth | `materialize` expectation | `observe` / drift expectation | `reconcile` expectation | rematerialize / clean-rebuild expectation |
|---|---|---|---|---|---|---|
| example: stable Juego2 realization profile/preset | retained realization sidecar/prefab/preset | reviewed repo/project asset | preserved/consumed through documented host | recognized as admitted retained state, not false unsupported drift | canonical edits route through accepted owner; local realization edits remain explicitly downstream | survives or is deterministically reconstructed from admitted source |
| example: generated bake/cache | generated/transient | admitted source inputs + deterministic/documented generation step | may be regenerated | must not become canonical drift/source truth | never reconciled as canonical intent | disposable and reproducible or explicitly re-baked during restoration |

Every row must choose a concrete lifecycle class/host. At minimum distinguish where applicable:

- **H1-managed projection state** already covered by accepted H1 component/materialization contracts;
- **retained realization state** hosted in an admitted Juego2 sidecar, prefab, preset, project asset or equivalent reviewed Unity-side source that H1 must preserve rather than reinterpret as canonical Arkus truth;
- **generated/transient state** that may be deleted and must be deterministically reconstructible or covered by an explicit documented regeneration step;
- **external/manual source input** whose lawful provisioning is required before reconstruction.

The matrix must explicitly cover selected Cinemachine, Splines, AI Navigation, Animation Rigging, Terrain and equivalent tool state **when those capabilities are adopted**; naming a package without classifying the actual retained/generated state it introduces is insufficient.

If H1 observation/reconciliation would currently see an admitted retained state as unsupported drift, or materialization/clean rebuild would erase required state, H2F-02 must either place that state behind a documented retained host/boundary that preserves H1 authority or record a causal H2F/H1 compatibility blocker. It may not silently rely on the Editor Library/cache or on hand repair after materialization.

H2F-03 owns the effective composed lifecycle proof for this declared matrix.

## PASS only if all are true

- every `ADOPT_NOW` material dependency/source has exact reviewed adoption data;
- package/project dependency state is pinned or otherwise reproducibly constrained;
- URP is the active selected baseline and representative real assets render without silent unsupported fallback;
- selected external tools do not redefine Arkus/CITY/ART semantic authority;
- manual/account-gated inputs have explicit provisioning rather than hidden local assumptions;
- character Humanoid/retarget and selected worldbuilding import/preset rules are explicit;
- every material selected foundation-state family has a complete H1 lifecycle classification/host row with explicit observe/reconcile/rebuild expectations;
- no required selected foundation state depends on being invisible to H1 or manually re-created after ordinary materialize/rematerialize/clean-rebuild operations;
- project compile/import succeeds with the selected baseline;
- non-selected spike packages/assets are removed or isolated so they do not become accidental project dependencies;
- H2F-03 can exercise the baseline and the declared H1 lifecycle boundary without making new unresolved tooling choices.

## Negative gates

The acceptance-contract negative gates are binding. In particular, “works on the owner PC because the package happens to be in the local Library/cache” is not reproducible adoption evidence, and “works until the next H1 materialize/reconcile/rebuild” is not an admissible foundation state.
