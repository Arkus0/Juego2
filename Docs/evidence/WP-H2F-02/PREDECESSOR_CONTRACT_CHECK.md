# WP-H2F-02 predecessor contract check

PREDECESSOR_CONTRACT_CHECK

Date: 2026-09-28. Worker start `main`: `e5322c842de072ae120ae19fe5cf4280e3569806` (merge of PR #260, the H2F-01A DocSync). Worker: Claude (owner-invoked Worker session, `Worker H2F-02`). Branch: `worker/h2f-02`. Contract: `Docs/workpacks/H2F/WP-H2F-02.md` at that `main`.

Recorded before any implementation byte was written.

## Accepted predecessors and identities (confirmed live on GitHub)

| Predecessor | Accepted identity | Status used here |
|---|---|---|
| `WP-H2F-01` decision spikes + stack selection | PR #254; frozen candidate `b642024ee54523d19b91448016af83a50d0ff361`; PASS review `#5331371244`; merge `27e6e56215268e566f24ea1f3a80e62601126f89`; `Docs/evidence/WP-H2F-01/DOCSYNC.md` = `DOCSYNC_COMPLETE` | Direct dependency. Merged and DocSynced. |
| `WP-H2F-01A` GC2 Core capability extraction | PR #256; frozen candidate `0eabed44bd73a396c0292892405e349900da0e32`; PASS review `#5333319449`; merge `2276dc1c2b429f273b7715fda023539feefdb086`; `DOCSYNC_COMPLETE` comment `#issuecomment-5862263491`; DocSync PR #260 merged | Direct dependency. Merged and DocSynced. Its S06 amendment is therefore binding. |
| `WP-H1-GATE` | PR #238; candidate `8bcf171f2520a0e23ccba094bf353c8a78b5d239`; PASS `#5329808171`; merge `b730ba0c0b1a4ad454edbd420cd8e44555dfaf10`; `Docs/evidence/WP-H1-GATE/DOCSYNC.md` | Direct dependency. Accepted Unity bridge and authority boundary. |
| ART-01 `PREFOUNDATION_INPUT` | PR #234, Draft, `Worker state: PAUSED_FOR_H2F_FOUNDATION`; live head still `174d05d23c3bceb9d5df00e460b33519cf68328e` (the checkpoint H2F-01/01A consumed) | Consumed as a representative content/demand source only. ART-01 PASS is intentionally downstream. No ART-01 readiness is claimed. |
| Urban product amendment | PR #257, PASS `#5333313032`, merge `6bf2d6e74215be51e73d85f659a2a734752ea34c`; roadmap reconcile #259 | Adds the WP's urban reuse note. It does not change the proof obligations. |

No context capsule covers H2F-01, H2F-01A or H1-GATE (`Docs/engineering/context-capsules/index.json` has no H2F/H1-GATE entry). The exact sources were read:

- `Docs/workpacks/H2F/{README,WP-H2F-02,WP-H2F-03}.md`;
- `Docs/evidence/WP-H2F-01/{README,BASELINE_INTENT,ACQUISITION_AND_POLICY,DOCSYNC}.md`, `DISPOSITION_MATRIX.csv`, `spike_project/{bootstrap.py,import_unitypackage.py,Packages/manifest.json}` and the S01/S02 spike code;
- `Docs/evidence/WP-H2F-01A/{README,H2F02_CORE_HANDOFF,S06_BASELINE_AMENDMENT,CORE_VERSION_AND_PROVISIONING,CORE_STATE_INVENTORY,ARKUS_GC2_AUTHORITY_BOUNDARY,PUBLIC_AUTHORING_SURFACE,PREDECESSOR_CONTRACT_CHECK}.md`, `CORE_CAPABILITY_MATRIX.csv`, `results/probe_packages_lock.json`, `scripts/h2f01a-verify-exact-sha.sh`;
- `Docs/engineering/{H1_ENGINE_BRIDGE_ARCHITECTURE,DEPENDENCY_IP_POLICY,H1_REMOTE_LOCAL_EXECUTION,PRODUCT_SHA_CLOSURE,WORKER_REVIEW_PROTOCOL}.md`, `Docs/architecture/ADR-H1-003-UNITY-HOST-AUTHORITY.md`;
- `Docs/product/URBAN_EXPANSION_DECISION.md`, `Docs/roadmap/POST_H2F01A_ROADMAP.md`;
- the current `Unity/ArkusUnity` project (manifest, lock, ProjectSettings, `Assets/Arkus/H1/Editor/H1Baseline.cs`, `H1BaselineTests.cs`, `CITY/Editor/City04GreyboxBuilder.cs`, CITY-04 materials) and the H1 hosted Unity workflow triggers.

## Inherited guarantees consumed, not re-proved

- **H2F-01 selection** (`BASELINE_INTENT.md`): URP 17.3.0 + Shader Graph; Input System 1.20.0; Splines 2.9.1 with project-owned profile/junction realization; AI Navigation 2.0.15 with the project agent (radius 0.28, climb 0.30, slope 40°); Animation Rigging 1.4.1; Unity Terrain for scenic ground only; project-owned water, interior-window and seeded scatter tools; UAL1 + UAL2 fillers; explicit Humanoid mapping for Base Characters derivatives; the render baseline values measured in S01. The spike findings (URP batch converter broken, pre-switch FBX materials, decal rendering layers, depth texture, pinned manifest, `.unitypackage` import rule) are consumed as facts.
- **H2F-01A Core facts**: GC2 Core 2.19.61, package SHA-256 `1e4f3ba0560eadb944b3cf3ca63f7095d95b47c55a94ee655f28a15952f3380b` (28,236,878 bytes); the Assets-only import plus `com.unity.modules.physics2d` compiles on the selected stack; the state families S1–S15; the capability dispositions; the authority rules; the public authoring surface and serialized paths for 2.19.61; C02–C07 probe results.
- **S06 amendment** (binding because 01A passed): GC2 `Character` = player body, GC2 `MainCamera` + Third Person `ShotCamera` = player camera, Juego2 action map = sole input owner. The H2F-01 minimal controller and Cinemachine player rig are displaced.
- **H1 / H1-GATE**: Unity 6000.3.24f1 identity; Arkus owns canonical identity/state/authoring semantics; Unity GUIDs/objects/components are bridge locators; managed scenes/prefabs are disposable projections under `Assets/Arkus/H1/Managed*`; packages and ProjectSettings are bootstrap authority, not public authoring units; runtime/physics/navmesh/AI/save state is outside H1; Unity→canonical sync only as explicit proposals.
- **ART-01 input**: the ART-01 structural route (digest `a7f8534f…`) and owner-vault Quaternius sources pinned by ART-01 `SOURCE_LOCK.json` + H2F-01 `SPIKE_INPUT_LOCK.json`, consumed as representative content, not re-audited.

## Newly owned by H2F-02

- Exact admission into `Unity/ArkusUnity`: pinned manifest/lock, adoption records per `DEPENDENCY_IP_POLICY.md` for every `ADOPT_NOW` dependency and every `AVAILABLE_ASSET` source, including GC2 Core.
- The canonical URP project baseline (pipeline/renderer/Volume assets, quality tiers, project settings) and the transition of the H1 bootstrap's render-pipeline expectation from built-in to that URP baseline.
- Project import/source conventions, the Juego2 adapter/preset boundary (worldbuilding profiles, character/camera presets, GC2 adapter seam), the lints handed over by 01A, and reproducible lawful provisioning.
- The H1 lifecycle classification matrix for every material selected-foundation state family.
- Clean compile/import evidence with the selected stack present, and URP migration evidence on representative real assets.

## Explicitly not owned here

The composed H1 materialize→observe→reconcile→rematerialize→clean-rebuild proof and the integrated fixture (H2F-03); ART-01 effective benchmark/`KEEPER_READY`; CITY-07 keeper geometry; H2 NPC/Living World; separately licensed GC2 modules; the GC2 save host (H6).

## Reopen conditions

- The GC2 package on this workstation differs from `1e4f3ba0…` / 28,236,878 bytes → stop and re-identify before relying on 01A facts. (Checked at start: hash matches.)
- A selected package cannot be installed at its H2F-01 exercised version on 6000.3.24f1, or Core does not compile on the adopted manifest through the recorded route → reopen the specific H2F-01/01A compatibility statement, not the selection.
- The H1 public lifecycle cannot tolerate the URP baseline (for example accepted H1 EditMode tests fail for a reason other than the deliberate render-pipeline expectation) → compatibility blocker routed to the causal boundary; H1 is reopened only if effective evidence contradicts an accepted H1 guarantee.
- ART-01 PR #234 publishes a new `PREFOUNDATION_INPUT` that invalidates the pinned `174d05d2` input → revisit the representative-content evidence.
- A required GC2 capability can only be used by making GC2 identifiers canonical → reject the capability (01A rule), never reopen H1 authority.

## Live-state addendum at freeze (2026-09-28)

- **ART-01 PR #234** moved from `174d05d2` to `d227cb26`: an owner-directed, docs/evidence-only PREFOUNDATION continuation plus a merge of `main`. `git diff 174d05d2 d227cb26` is empty for `Unity/ArkusUnity/Assets/Arkus/ART`, `ART.meta`, `Docs/evidence/WP-ART-01/SOURCE_LOCK.json` and `KIT_COMPOSITION_MANIFEST.json`. The representative input used here is therefore byte-identical, and the reopen condition does not fire. The continuation keeps the benchmark scene and names assembly quality as ART-01's oracle, consistent with routing the owner's house observation to ART-01 (`OWNER_JUDGEMENT.md`).
- **`main`** advanced from `e5322c84` to `1a37d10b` (visual-production, town-identity and asset-reuse amendments, all docs, plus a CITY-URBAN-00 route in `scripts/arkus-verify-exact-sha.sh`). `Docs/roadmap/POST_H2F01A_ROADMAP.md` on that `main` states that the route H2F-02 → H2F-03 → H2F-GATE and their acceptance bars are unchanged. No `Docs/workpacks/H2F/**`, Unity, protocol or dependency-policy file changed. The PR stays mergeable.
