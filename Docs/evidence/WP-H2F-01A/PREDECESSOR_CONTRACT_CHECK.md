# WP-H2F-01A predecessor contract check

PREDECESSOR_CONTRACT_CHECK

Date: 2026-09-27. Worker start `main`: `4ba6b2d42ecb75a9c9fe450b5efc02aaa62a0849` (merge of the WP-H2F-01A contract, PR #255). Worker: Claude (owner-invoked Worker session, `Worker H2F-01A`). Branch: `worker/h2f-01a`.

## Accepted predecessors and identities

| Predecessor | Accepted identity | Status used here |
|---|---|---|
| `WP-H2F-01` decision spikes + stack selection | PR #254; frozen candidate `b642024ee54523d19b91448016af83a50d0ff361`; independent PASS review `#5331371244`; merge `27e6e56215268e566f24ea1f3a80e62601126f89`; `Docs/evidence/WP-H2F-01/DOCSYNC.md` = `DOCSYNC_COMPLETE` | Direct dependency. Accepted, merged, DocSynced. |
| `WP-H2F-01A` contract (PROCESS_ONLY insertion) | PR #255; FAIL review `#5331381797` on `d70c7433`, repaired; PASS review `#5331395079` on `02f346188e8f9f9aef5a5baa2ac65f5b6fe68317`; merge `4ba6b2d42ecb75a9c9fe450b5efc02aaa62a0849` | The exact contract executed here: `Docs/workpacks/H2F/WP-H2F-01A.md` at `4ba6b2d4`. |
| `WP-H1-GATE` | PR #238; candidate `8bcf171f2520a0e23ccba094bf353c8a78b5d239`; PASS `#5329808171`; merge `b730ba0c0b1a4ad454edbd420cd8e44555dfaf10`; `Docs/evidence/WP-H1-GATE/DOCSYNC.md` | Direct dependency. Accepted Unity bridge/authority boundary. |
| ART-01 `PREFOUNDATION_INPUT` | PR #234 (Draft, `PAUSED_FOR_H2F_FOUNDATION`), structural checkpoint `174d05d23c3bceb9d5df00e460b33519cf68328e`, consumed by H2F-01 (`SPIKE_INPUT_LOCK.json`, `artInputSha`) | Consumed through H2F-01's pinned recipe only. No ART-01 readiness or PASS is claimed. |
| `WP-H2F-00` | PR #251, merge `a47f879c46abce88ee23d5959736e32ba5bd62be` | Transitive only (category/vocabulary context). |

No context capsule covers H2F-00/H2F-01 (`Docs/engineering/context-capsules/index.json` holds no H2F entry). The exact sources were read: `Docs/workpacks/H2F/{README,EXECUTION_ORDER,WP-H2F-01A,WP-H2F-02}.md`; `Docs/evidence/WP-H2F-01/{README,DOCSYNC,BASELINE_INTENT,SPIKE_RESULTS,DISPOSITION_MATRIX.csv,SPIKE_LEDGER,ACQUISITION_AND_POLICY}.md` and `results/gc2_*`, `results/s06_*`; the H2F-01 spike recipe (`spike_project/**`).

## H2F-01 verdict handling (WP section "H2F-01 verdict handling")

H2F-01 did **not** structurally reject GC2 Core. Its matrix row says: "Owned and technically compatible ... NOT structurally rejected" and disposes GC2 as `DEFER` for the S06 player controller/camera role only. Therefore:

- the `NOT_MATERIAL` fast path does **not** apply; C01–C07 run;
- the accepted S06 winner (CharacterController + Input System action map + project-owned locomotion/camera-root controller + Cinemachine 3 `CinemachineCamera`/`ThirdPersonFollow` with obstacle avoidance) stays binding unless C02/C07 produce new, material evidence meeting the explicit `S06_BASELINE_AMENDMENT.md` rule. The S06 measurements already recorded by H2F-01 are consumed, not re-litigated.

## Inherited guarantees consumed, not re-proved

- **H2F-01 selection:** the whole selected stack in `BASELINE_INTENT.md` (URP 17.3.0, Input System 1.20.0, Cinemachine 3.1.7, AI Navigation 2.0.15 with the project agent 0.28/0.30/40°, Animation Rigging 1.4.1, Splines 2.9.1, UAL1/UAL2, explicit Humanoid mapping rule for Base Characters derivatives).
- **H2F-01 GC2 facts:** Core 2.19.61, package SHA-256 `1e4f3ba0560eadb944b3cf3ca63f7095d95b47c55a94ee655f28a15952f3380b`, 28,236,878 bytes, owner-purchased Asset Store package with no bundled license file; the Assets-only import (vendor `Packages/manifest.json` excluded) plus `com.unity.modules.physics2d` compiles on the selected stack; S06 route measurements for both compositions (`results/s06_*_route_log.txt`); the edit-mode `ChangeModel` caveat.
- **H2F-01 import rule:** never import a `.unitypackage` carrying project-global state without inspection/diff/merge policy; `spike_project/import_unitypackage.py` is the proven Assets-only route.
- **H1 / H1-GATE:** Unity 6000.3.24f1 editor identity; Arkus owns canonical identity/state and authoring semantics; Unity scene objects, GUIDs, component IDs are bridge locators, never canonical identity; Unity→canonical sync only as explicit proposals.
- **ART:** the ART-01 structural route (digest `a7f8534f…`) and the owner-vault Quaternius sources pinned by ART-01 `SOURCE_LOCK.json` + H2F-01 `SPIKE_INPUT_LOCK.json`, consumed as representative content, not re-audited.

## Newly owned by H2F-01A

- C01 exact Core surface/state inventory of the owner-supplied 2.19.61 install (assemblies, namespaces, public extension points, settings/assets, dependencies).
- C02–C07 bounded Unity probes on representative Juego2/Quaternius content, in a disposable workspace outside the repository.
- Capability matrix with exactly one of the six 01A dispositions per material family; Arkus↔GC2 authority boundary; H2–H6 savings map with evidence-backed `DO_NOT_DUPLICATE`; H2F-02 Core adoption/lifecycle handoff; public authoring surface; residue/license ledger; exact-SHA verifier.
- The S06 amendment decision (`NO_AMENDMENT` or an explicit `S06_BASELINE_AMENDMENT.md`).

## Explicitly not owned here

Exact adoption/admission into `Unity/ArkusUnity`, H1 lifecycle classification of retained Core state (H2F-02); composed H1 lifecycle proof (H2F-03); H3–H6 product systems; separately licensed GC2 modules (Inventory, Dialogue, Quests, Behavior, Perception, Melee, Shooter, Stats, …); ART-01 `KEEPER_READY`; keeper CITY geometry.

## Reopen conditions

- The installed Core identity or hash differs from H2F-01's recorded `2.19.61` / `1e4f3ba0…` package → stop and re-identify the input before relying on H2F-01 compatibility facts.
- Core cannot compile with the selected stack under the recorded Assets-only + `physics2d` route → that contradicts an H2F-01 fact; reopen the H2F-01 GC2 compatibility statement (not the stack selection).
- The pinned ART/Quaternius inputs cannot be rebuilt through the H2F-01 recipe → escalate to the H2F-01/ART-01 input boundary.
- A Core capability can only be used by making GC2 Variables/SaveLoad IDs/component IDs canonical → `REJECT_CAPABILITY` for that capability, never a reason to reopen H1 authority.
- C02/C07 evidence shows a material controller/camera/input advantage → handled only through the explicit baseline-amendment rule, never by silently re-selecting S06.
