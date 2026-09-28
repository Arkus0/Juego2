# WP-H2F-02 — Exact adoption + URP/toolchain bootstrap

Status: **Worker evidence** (see the PR for the live handoff state). Mode: LOCAL_UNITY_REQUIRED. Real Unity 6000.3.24f1 evidence was produced on the owner workstation. Hosted H1 suites ran on GitHub Actions. Baseline `main` `e5322c84`. Contract: `Docs/workpacks/H2F/WP-H2F-02.md`.

The H2F-01 stack, amended by the accepted H2F-01A S06 decision, is now project truth in `Unity/ArkusUnity`:

- URP 17.3 baseline (High/Low tiers, SSAO, decals with decal layers, depth texture, soft shadows, Linear colour, look preset);
- exact pinned packages;
- GC2 Core 2.19.61 admitted behind a Juego2 seam: Assets-only, hash-gated provisioning, never committed, adapter compiled only when Core is present;
- S06 player/camera presets on the Juego2 action map;
- the seven 01A lints;
- worldbuilding realizers (spline profile, tee junction, seeded scatter) and project water/window shaders;
- Humanoid/UAL import rules;
- an H1 lifecycle class and host for every material state family;
- the one H1 catalogue re-baseline URP forces.

| File | Content |
|---|---|
| `PREDECESSOR_CONTRACT_CHECK.md`, `WORKER_PLAN.md` | inherited/owned guarantees and the write set (including its recorded amendment) |
| `ADOPTION_RECORDS.md` | per-dependency record: exact version, license at version, linkage, authority, replacement, notices, owner |
| `URP_BASELINE.md` | pipeline/renderer/tier settings; GI, APV, SSAO, decal, transparency, water and window policies; migration and audit |
| `IMPORT_CONVENTIONS.md` | source/derived/generated folders, units, model defaults, Humanoid/UAL rules, colliders/nav; ART-01 rebase obligations |
| `ADAPTER_BOUNDARY.md` | public Juego2 surfaces, GC2↔Arkus seam, lints, urban-block reuse, replacement boundaries |
| `PROVISIONING.md` | input classes and the smallest lawful restoration path |
| `GC2_CORE_ADOPTION.md` | item-by-item closure of `H2F02_CORE_HANDOFF.md` and the S06 amendment |
| `H1_LIFECYCLE_MATRIX.csv` | one row per material selected-foundation state family (class, host, source of truth, materialize/observe/reconcile/rebuild expectation, witness) |
| `H1_COMPATIBILITY.md` | every H1 byte changed, why, and hosted H1 evidence |
| `RESIDUALS.md` | residuals with owners |
| `OWNER_JUDGEMENT.md` | the owner's visual inspection of the migrated content |
| `workspace/` | disposable evidence workspace recipe (`workspace.py run --sha …`) + evidence driver (never in the product project) |
| `tools/` | H1 catalogue fingerprint replica, lifecycle-matrix writer, results collector |
| `results/`, `captures/owner/` | machine outputs and captures |

Exact-SHA verifier: `scripts/h2f02-verify-exact-sha.sh`, routed from `scripts/arkus-verify-exact-sha-base.sh`. It re-reads the recorded evidence, binds it to the candidate's exact `Unity/ArkusUnity` tree, and checks the structural rules. It needs neither Unity nor licensed bytes.

## PASS conditions → evidence

| WP condition | Evidence |
|---|---|
| Every `ADOPT_NOW` dependency/source has exact reviewed adoption data | `ADOPTION_RECORDS.md` (licenses read at the resolved versions) |
| Every admitted GC2 Core dependency/capability has exact provisioning and no silent module expansion | `GC2_CORE_ADOPTION.md` §1; hash gate + receipt; exactly the 3 Core assemblies (`results/b1_gc2_report.json`); `J2_GC2_MODULE_NOT_ADMITTED` |
| Package/project dependency state pinned | manifest + lock exact, depth 0 for direct pins; verified by `J2PackageBaseline` and the exact-SHA verifier; forbidden packages absent |
| URP active; representative real assets render without silent fallback | clean-restoration report (URP active); ART-01 route + worldbuilding scenes + all content materials audited GREEN (`results/representative.json`); captures; owner inspection |
| Selected tools do not redefine Arkus/CITY/ART authority | `ADAPTER_BOUNDARY.md` §5; lints; realizers generate from CITY/ART inputs and never write canonical state |
| Manual/account-gated inputs have explicit provisioning | `PROVISIONING.md` |
| Humanoid/retarget and worldbuilding import/preset rules explicit | `IMPORT_CONVENTIONS.md`; mapping applied and verified on the ART citizen and UAL libraries |
| Every material selected-foundation state family has an H1 lifecycle row | `H1_LIFECYCLE_MATRIX.csv` (completeness checked by the verifier) |
| Every material GC2 family from the handoff is classified or rejected | matrix rows `gc2.S1`–`gc2.S15` + define/adapter/scene rows; `GC2_CORE_ADOPTION.md` §2 |
| No GC2 Variable, save id, component GUID or plugin id becomes canonical identity | `ArkusEntityBinding` `j2.*` keys only; identity audit empty; lints L3/L7; `Juego2.Arkus` references nothing |
| S06 amendment adopted exactly; displaced realization not retained | GC2 Character/camera presets; Cinemachine not installed (forbidden); no minimal controller |
| No required state depends on invisibility to H1 or manual recreation | matrix hosts: retained sidecars outside managed roots, generated state reproducible, H1 catalogue re-baselined explicitly |
| Project compile/import succeeds | clean restoration with Core absent and with Core present, both with 0 findings; all Juego2 EditMode tests pass; Windows player build succeeds; hosted H1-07..11 run on the URP project |
| Non-selected spike packages/assets removed or isolated | no spike package installed; spike/probe code stays in H2F-01/01A evidence; the evidence driver lives only in the workspace |
| H2F-03 can exercise the baseline without new tooling choices | all selected roles have a surface (`ADAPTER_BOUNDARY.md`); residual items are tuning/extensions with owners (`RESIDUALS.md`) |

## Negative gates → evidence

- **Floating versions:** exact pins, and the verifier rejects any other set.
- **Unknown license:** licenses were read at the resolved versions; the GC2 EULA terms are recorded.
- **Hidden magenta/fallback:** fail-closed `J2ShaderAudit`, including Terrain prototypes. Retained CITY-04 materials were migrated; H1 fixtures are declared, never hidden.
- **Local bytes assumed:** a clean restoration from committed bytes compiles with Core absent; Core comes only through the hash-gated script.
- **Plugin-private id as canonical:** identity audit plus lints.
- **Mutation path around Arkus:** adapter requests go through `IArkusFactAuthority`, fail closed without an authority; lint L5.
- **Unclassified state:** the matrix completeness check.
- **01A boundary ignored:** L1–L7 plus the structural boundary.
- **Silent S06 change:** Cinemachine forbidden; S06 values are in the presets.

Negative controls exercised: every lint in `J2Gc2FoundationTests`; package, boundary, legacy-input and audit controls in `J2FoundationBaselineTests` / `J2RenderingTests`; the junction oracle's missing-apron control; an oblique branch refused.

## Non-claims

No CITY-07 keeper realization. No ART-01 readiness/PASS. No final visual polish. No H2 NPC/Living World. No composed H1 lifecycle proof (H2F-03). No GC2↔Arkus production round trip (GC2-00). No separately licensed GC2 module. No GC2 save host (H6).
