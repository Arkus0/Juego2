# WP-H2F-03 predecessor contract check

PREDECESSOR_CONTRACT_CHECK

Date: 2026-09-28. Worker start `main`: `204b169fcd9f8650fe37e76df726f507a403aa4f` (merge of PR #270, the H2F-02 DocSync). Worker: Claude (owner-invoked Worker session, `Worker h2f-03`). Branch: `worker/h2f-03`. Contract: `Docs/workpacks/H2F/WP-H2F-03.md` at that `main` (unchanged by the H2F-02 merge and DocSync).

Recorded before any implementation byte was written.

The owner started this session while the H2F-02 merge/DocSync was still in flight. Live reconstruction shows it had completed before this check was written: `DOCSYNC_COMPLETE` for H2F-02 is on PR #261 and `Docs/evidence/WP-H2F-02/DOCSYNC.md` is on `main`. H2F-03 is therefore dependency-valid without an exception.

## Accepted predecessors and identities (confirmed live on GitHub)

| Predecessor | Accepted identity | Status used here |
|---|---|---|
| `WP-H2F-02` exact adoption + URP/toolchain bootstrap | PR #261; frozen candidate `91047ecc71be1b3f8c00440a8de07cc90ca6be9e`; PASS review `#5335599241`; merge `5c1877b224a7183e248adc0d6b08f677a20f4d3f`; DocSync PR #270, merge `204b169f`; `Docs/evidence/WP-H2F-02/DOCSYNC.md` = `DOCSYNC_COMPLETE` | Sole direct workpack dependency. Merged and DocSynced. |
| `WP-H2F-01A` GC2 Core capability extraction | PR #256; candidate `0eabed44bd73a396c0292892405e349900da0e32`; PASS `#5333319449`; merge `2276dc1c2b429f273b7715fda023539feefdb086` | Transitive. Its S06 amendment and public authoring surface are consumed through H2F-02. |
| `WP-H2F-01` decision spikes + stack selection | PR #254; candidate `b642024ee54523d19b91448016af83a50d0ff361`; PASS `#5331371244`; merge `27e6e56215268e566f24ea1f3a80e62601126f89` | Transitive. The `ADOPT_NOW` set defines the capabilities the fixture must exercise. |
| `WP-H1-GATE` + accepted H1 bridge | PR #238; candidate `8bcf171f2520a0e23ccba094bf353c8a78b5d239`; PASS `#5329808171`; merge `b730ba0c0b1a4ad454edbd420cd8e44555dfaf10`; `Docs/evidence/WP-H1-GATE/DOCSYNC.md` | Accepted canonical→Unity/reconciliation/checkpoint/rebuild authority. Its public `--h1-unity` hosts are the only lifecycle path used. |
| ART-01 `PREFOUNDATION_INPUT` | PR #234, Draft, `PAUSED_FOR_H2F_FOUNDATION`; live head `d227cb2648df55ba773a3a9e2cc1b2b0dc59ac52`. `git diff 174d05d2 d227cb26` is empty for `Unity/ArkusUnity/Assets/Arkus/ART`, `ART.meta`, `Docs/evidence/WP-ART-01/SOURCE_LOCK.json` and `KIT_COMPOSITION_MANIFEST.json` (re-checked at this start). | Consumed as representative content only, at the same bytes H2F-01/01A/02 used (`174d05d2`). No ART-01 readiness is claimed. |
| Urban product amendment | PR #257 (PASS `#5333313032`), roadmap reconcile #259; `Docs/product/URBAN_EXPANSION_DECISION.md`, `Docs/roadmap/POST_H2F01A_ROADMAP.md` | Adds the WP's urban note: GC2↔Arkus seam coexistence and a reusable street/interior/navigation realization profile record. It does not change proof obligations. |

No context capsule covers H2F-02, H2F-01A, H2F-01 or H1-GATE (`Docs/engineering/context-capsules/index.json` has no entry for them). The exact sources were read:

- `Docs/workpacks/H2F/{README,WP-H2F-02,WP-H2F-03}.md`;
- `Docs/evidence/WP-H2F-02/{README,PREDECESSOR_CONTRACT_CHECK,WORKER_PLAN,ADOPTION_RECORDS,ADAPTER_BOUNDARY,PROVISIONING,H1_COMPATIBILITY,RESIDUALS,DOCSYNC}.md`, `H1_LIFECYCLE_MATRIX.csv`, `workspace/workspace.py`, `workspace/Editor/J2H2F02Evidence.cs`, and the PASS review `#5335599241`;
- `Docs/evidence/WP-H2F-01/BASELINE_INTENT.md`; `Docs/evidence/WP-H2F-01A/PUBLIC_AUTHORING_SURFACE.md`;
- `Docs/evidence/WP-H1-GATE/{PROOF_MATRIX,AI_AGENT_TRIAL_BRIEF}.md`, `scripts/h1-gate-scenario.py`, `.github/workflows/h1-gate-validation.yml`, `tools/Arkus.H1.UnityHost/{ProductionH1ProjectCheckpointHost,H1UnityLifecycle,H1UnityProcessInfrastructure}.cs`, `src/Arkus.Harness.Cli/{Program,H1ReferenceTransportHost}.cs`;
- `Docs/engineering/{H1_REMOTE_LOCAL_EXECUTION,PRODUCT_SHA_CLOSURE,WORKER_REVIEW_PROTOCOL}.md`;
- the `Unity/ArkusUnity/Assets/Juego2/**` foundation sources (Arkus seam, GC2 adapter and presets, realizers, profiles, input asset).

## Inherited guarantees consumed, not re-proved

- **H2F-02 adoption** (binding): the exact pinned package set with Cinemachine not admitted; the URP 17.3.0 project baseline (High/Low tiers, SSAO, decals with decal layers, depth texture, Linear colour, look preset); GC2 Core 2.19.61 behind the Juego2 seam with hash-gated, Assets-only provisioning; S06 presets on `J2_Input`; lints L1–L7; the worldbuilding realizers and water/window shaders; the Humanoid/UAL import rules; the one H1 catalogue re-baseline. H2F-03 uses these surfaces as they are. It does not re-audit versions, licences, the provisioning hash gate or the lint semantics.
- **H2F-02 lifecycle matrix** (`H1_LIFECYCLE_MATRIX.csv`, 47 rows): the declared class, host and rebuild expectation of every material selected-foundation state family. H2F-03 checks effective behaviour against it. It does not reclassify families.
- **H2F-02 effective evidence**: clean import Core-absent/Core-present, representative-content audits, player build. Not repeated as such. H2F-03 builds its own workspace from committed bytes because its fixture needs one, and records what that import produced.
- **H1 / H1-GATE**: the public `--h1-unity` hosts, the capability set in `REQUIRED_CAPABILITIES` of the Gate scenario, H1-managed roots `Assets/Arkus/H1/Managed*`, checkpoint capture/restore and clean rebuild, drift → import proposal → H0 plan/dry-run/apply, and the component allowlist (Transform, MeshRenderer, Animator, canonical link). H2F-03 drives these only through the public host and does not re-prove their internal guarantees.
- **H2F-01A**: the public GC2 authoring recipes (`PUBLIC_AUTHORING_SURFACE.md`), the authority rules and the probe results for play-mode drive/camera (consumed by H2F-02 R5, now exercised effectively here).
- **ART-01 input**: the structural route, kit materials, clothed citizen `Townsfolk_Forastero`, Nature/Props sources pinned by `SOURCE_LOCK.json` and H2F-01 `SPIKE_INPUT_LOCK.json`. Not re-audited.

## Newly owned by H2F-03

- One bounded `H2F_INTEGRATION_FIXTURE / NON_KEEPER` fixture where every applicable `ADOPT_NOW` capability coexists: URP look + SSAO + decal, terrain + spline road/junction, kerb/retaining edges, seeded scatter with exclusions, water, interior window, GC2 player/camera on `J2_Input`, NavMesh + GC2 NPC on it, Quaternius humanoid with the UAL retarget baseline, Animation Rigging contact IK, one GC2 interaction hook through the Arkus seam, capture/profiling.
- The composed H1 public lifecycle with that fixture present (materialize → observe → legal change → reconcile → rematerialize → remove generated output → restore + clean rebuild → reopen and exercise), and per-family before/after witnesses against the H2F-02 matrix.
- Continuous third-person capture, owner third-person play inspection, and the predeclared diagnostic profiler oracle.
- The fresh-author/agent trial through the prescribed boundary, and proof that the operation does not silently regenerate unrelated content.
- The issue ledger (H2F blocker / causal predecessor contradiction / permitted residual) and the street/interior/navigation realization profile record for a later compact city block.

## Explicitly not owned here

ART-01 readiness/PASS or `KEEPER_READY`; CITY-07 keeper geometry or a real port street; H3 routines; a GC2↔Arkus production host (GC2-00: the fixture uses a fixture-local authority on the accepted `IArkusFactAuthority` seam); separately licensed GC2 modules; shipping performance budgets; ART-02 polish; H2F-GATE.

## Reopen conditions

- Effective lifecycle evidence contradicts an H2F-02 matrix row (a retained family lost or surfaced as unsupported drift, a generated family not reproducible by its declared step) → causal compatibility blocker routed to the H2F-02 boundary/tool decision. The fixture is not hand-patched.
- An H1 public capability fails on a legal fixture in a way that contradicts an accepted H1 guarantee → H1 is reopened only on that concrete evidence.
- The GC2 package on this workstation differs from `1e4f3ba0…2f3380b` / 28,236,878 bytes → stop and re-identify.
- ART-01 PR #234 changes the pinned ART input paths → revisit the representative-content evidence.
- A selected capability cannot run in the frozen URP/platform configuration on the fixture → enumerated pathology 4, routed to H2F-01's selection.
