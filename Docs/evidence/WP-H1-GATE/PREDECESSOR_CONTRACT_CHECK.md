# WP-H1-GATE — PREDECESSOR_CONTRACT_CHECK

Status: WORKER ACTIVE (recorded before implementation, baseline `338dd0c8a3e9b1a882a8654bdc2556db9a0520f5`)

## Live state reconstructed

- `main` = `338dd0c8a3e9b1a882a8654bdc2556db9a0520f5` (merge of `WP-H1-11`, PR `#236`).
- PR `#236` carries `ARKUS_AUTOMATION_V2 State: DOCSYNC_COMPLETE` (comment `#5845062002`) naming `Next WP: WP-H1-GATE`.
- No open PR owns `WP-H1-GATE`. The open PRs (`#222`, `#228`, `#233`, `#234`, `#237`) belong to other tracks and touch no H1 bridge contract.
- Canonical Worker branch: `claude/zen-lovelace-bnfqor`.

## Accepted direct predecessor

`WP-H1-11 — Representative real-asset Juego2 bridge slice` is accepted.

- Frozen candidate / PRODUCT_SHA: `228990be44052e376a112d0cac7dc3d712a0875f`
- Canonical implementation PR: `#236`
- Independent review `#5325400881` returned FAIL on one post-Editor integrity gap: the check did not cover the admitted `.meta` sidecars. The owner then accepted the candidate with an explicit circuit-breaker override (PR comment `#5845036679`, `OWNER_VERDICT: PASS`). That override keeps the `.meta` sidecar gap as a non-blocking residual. It reopens nothing from H1-04 to H1-10 and does not block H1-GATE.
- Implementation merge: `338dd0c8a3e9b1a882a8654bdc2556db9a0520f5`
- Exact-SHA evidence on the candidate: H1-11 `36231538194`, H1-10 `36231538120`, H1-09 `36231538131`, H1-08 `36231538124`, H1-07 `36231538153` and Arkus Main Safety `36231538183` are all GREEN, and the Worker preflight passed 449/449.

The binding planning amendment is `Docs/workpacks/H1/H1_REMAINING_COMPRESSION_AMENDMENT.md` (PR `#209`, PASS `#5315077514`). It makes H1-GATE a **proof-only boundary**. The Gate may add no product capability, replacement oracle or private route. A semantic bridge gap reopens its causal predecessor.

## Transitive accepted H1 chain consumed

| WP | Accepted candidate | PR | Independent verdict | Merge |
| --- | --- | --- | --- | --- |
| H1-00 | `3dc513dd963b77116fd45b5af8d800ae8993dd34` | `#75` | PASS `#5264661860` | `c02cf54c89c13db43edda4a602b0c1620baa3fa2` |
| H1-01 | `385ce2466190003d18c849c8d944b881d184e1c7` | `#79` | PASS `#5265525323` | `0b8f23fbce227bbbc710c8410dff575fcb9fcf12` |
| H1-02 | `d86a08e644f542e9515f5e54fd4061f61e251c70` | `#152` | owner acceptance override `#5798755634` after review `#5293810284` | `faa42a3d58ab26b0dc2547f9b6b7fc49a604219d` |
| H1-UNITY-CI | `b49b081a92b088d7b0fd9adce4bd5f26a3b6c1bf` | `#166` | PASS `#5299167558` | `6898250be985ab5d805bbdb129e30c9c6f1f4cdf` |
| H1-03 | `6d78ebc07e48419e279d4a17b93eeb099307316b` | `#168` | PASS `#5299906832` | `b52fe8f67bb74880af8ed2d734c1293869fd9f86` |
| H1-03A | `24d24526487af0d32e69799960f4251e44f0b5ad` | `#176` | PASS `#5303016012` | `90428b803948820663abfebaa3fe21eb37596247` |
| H1-04 | `8c6ffd61d17e832ed5b9f900e8c0f7d4e85bf5f5` | `#185` | PASS `#5306149043` | `4f172f7aa9e7c4a0909be046d095eac5477ad765` |
| H1-04 reopen 1 (Gate trial 1 → R1–R4) | `ce3ba74efae28692d886a5e11366c7849695e9f6` | `#239` | owner-waived review (correction only; pre-review `#5846087974`) | `4ab82fb1c0a0d8654aaa44ebc66452a3507ef347` |
| H1-ASSET-CLOUD | `e4d18002e6ca81f302daa645e306e9373debac92` (vault `Arkus0/Juego2-assets@ae782c5f08cc4144a7ff0c3d4af67451d4d86bb6`) | `#201` | PASS `#5313800159` | `fe6ecd5e14ef2581fd8fbe8f87166d13d02109bd` |
| H1-05 | `186224fbc3f53eb9c47ae528a56dcf3514af163f` | `#192` | PASS `#5308430565` | `7039adecac4e6e07d899247759d9b3c9fd3c2dae` |
| H1-06 | `96de260021fb28ff2cc7da8d2ef488be419568b3` | `#195` | PASS `#5313437809` | `6d04581bc3916b376bedc0797258097cfb22c225` |
| H1-07 | `a11f9c012307ee2c1d925eb1a1be143f1d127d21` | `#206` | PASS `#5315878884` | `f733b2fd50ab0aed27fce99d5ca7773969f3f339` |
| H1-08 | `8b7ea7b9ad74d67c5b300180b79833fe89350f4d` | `#216` | PASS `#5319609389` | `f22fa99e4427849b8a7202bf24bbae83fb90ee83` |
| H1-09 | `8f2103c037d08416d70687d4fb08b3d3c62bceac` | `#220` | PASS `#5322464433` | `9574f80aa07effc3022a3bb100a516d107965a19` |
| H1-10 | `bad48d1cf76cb5c8d7c3870b3dc88dc9b9a5c5e5` | `#230` | PASS `#5325001446` | `aa31db7d3ca34f72709db19b22375166c6fd42dd` |
| H1-11 | `228990be44052e376a112d0cac7dc3d712a0875f` | `#236` | owner circuit-breaker PASS `#5845036679` (after FAIL `#5325400881`) | `338dd0c8a3e9b1a882a8654bdc2556db9a0520f5` |

Observation, not a Gate repair: the status headers of `Docs/workpacks/H1/WP-H1-10.md` and `WP-H1-11.md` still read `PLANNED / NOT_STARTED` on `main`, but both carry live `DOCSYNC_COMPLETE` markers. The Gate records the accepted identities above in its reconciliation. It leaves those DocSync-owned headers untouched.

## Authoritative escalations used

No accepted-contract capsule covers H1. The Gate composes every H1 public capability, so the Worker read these exact sources:

- `Docs/engineering/H1_UNITY_PARITY_GATE.md` (the 17 deterministic stages, the parity tuple and the hard blockers);
- `Docs/architecture/ADR-H1-004-PUBLIC-EDITOR-EXECUTION-SEAM.md` (the fixed external host plus short-lived batch-worker topology, and the public capability role per WP);
- `Docs/engineering/H1_RISK_AND_RESIDUAL_PLAN.md` and `Docs/engineering/RESIDUAL_LEDGER.md` (the inherited residual universe);
- the H1-03A effective workflow (`.github/workflows/h1-03a-unity-lifecycle-validation.yml`). It is the accepted precedent for running the public `--h1-unity` JSONL and MCP hosts inside the pinned Unity image with the product launcher;
- the production composition `tools/Arkus.H1.UnityHost/ProductionH1ProjectCheckpointHost.cs`, the Editor entry point `Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1EditorWorker.cs`, and the H1-09 reconciliation and H1-10 checkpoint/restore handlers. The Worker read them to confirm which public capabilities exist. The Gate implementation does not call them.
- the H1-11 representative manifest (`Docs/evidence/WP-H1-11/REPRESENTATIVE_SLICE.json`), `PREDECESSOR_CONTRACT_CHECK.md` and `RESIDUAL_RISK.md`;
- the WP-HK-GATE precedent for a fresh independent AI-agent trial (`Docs/evidence/WP-HK-GATE/AI_AGENT_TRIAL_BRIEF.md`, `VERDICT.md`);
- the WP-DW-04 precedent for exact-SHA hosted model calls through OpenRouter (`.github/workflows/dw04-campaign.yml`).

Live discovery on the baseline (the local .NET 8.0.425 build of `Arkus.Harness.Mcp --h1-unity`, `tools/list`) returned 39 composed canonical capabilities. Every role that ADR-H1-004 assigns to H1-03A, H1-04, H1-05, H1-08, H1-09 and H1-10 is present:

- H1-03A: `unity.host.project-profile.inspect`, `unity.lifecycle.operation-status`;
- H1-04: `unity.host.catalogue.{query,get,resolve}`;
- H1-05: `unity.projection.plan` and `unity.host.projection.{materialize,observe}`. H1-08's pre/postflight is composed into these.
- H1-09: `unity.host.projection.{drift,import-proposal,rematerialize}`;
- H1-10: `unity.host.checkpoint.{capture,current,restore}` and `unity.host.projection.clean-rebuild`;
- H1-01: `unity.binding.{compile,decode,inspect}`;
- H0: `authoring.change.{plan,dry-run,apply,validate}`, `authoring.snapshot.{export,import}`, `authoring.journal.*`, `authoring.diff.compare`, `world.*`, `system.describe`.

The Unity worker entry point implements an executor for every Editor-bound role: project profile, hierarchy probe, catalogue, materialize/observe, reconcile and clean rebuild. **No missing public route was found. No predecessor reopen is needed to start the Gate.**

## Inherited guarantees consumed (not re-proved)

1. H0 (HK-GATE PASS): canonical identity/hash, plan/dry-run/apply authority, HK05 diagnostics, HK06B snapshot import, HK06C replay, HK07 neutral projection over JSONL and MCP, HK08B stale-CAS `same-lineage-replan` recovery, and the HK09A/B admission and resource envelope.
2. H1-00 and H1-01: the engine-neutral projection vocabulary, and the schema-aware Unity binding producer with derived canonical and catalogue dependencies.
3. H1-02, H1-UNITY-CI and H1-ASSET-CLOUD: the pinned Unity `6000.3.24f1 (4e7b9b5b6244)` project and packages, the accepted GitHub-hosted Unity substrate, and the hash- and GUID-verified private vault mount.
4. H1-03 and H1-03A: the Unity host policy, the external-host → batch-worker seam, the lease/ledger lifecycle and the structured `unity.execution.*` failures. The same composed handler serves reference and MCP.
5. H1-04: the catalogue model, the logical/native identity split and the effective inventory universe.
6. H1-05 to H1-08: staged generations and atomic publication, prefab lineage, allowlisted component adapters, and validator composition with pre/postflight (a failed postflight never publishes).
7. H1-09: the complete managed-scope drift oracle, and an import-proposal compiler that only emits a non-authoritative H0 mutation request.
8. H1-10: the checkpoint manifest and fingerprints, restore that goes only through `authoring.snapshot.import`, clean rebuild, and normalized reconstruction parity.
9. H1-11: the representative selected-item manifest (14 items and 3 clips), its import recipe, and the classified findings F1–F10.

## Guarantees newly owned by the Gate

Exactly the new guarantees the WP names:

- one complete deterministic H1 reference scenario that executes every stage of `H1_UNITY_PARITY_GATE.md` on the exact candidate, together with the parity-tuple verdict;
- proof that the scenario uses **only** public composed capabilities over the wire (reference JSONL and MCP) and never a private script, menu, test helper or direct Editor entry point to produce a bridge effect;
- exhaustive reconciliation of gate stages, residuals and dependencies against independently discovered sources, including a proof that the inherited universe does not shrink;
- gate-owned omission controls for the WP's seven causal negative-conformance classes;
- one fresh independent public-client AI-agent trial (hosted, exact-SHA, MCP-only);
- the explicit H2 authorization decision.

## Predecessor guarantees intentionally consumed rather than re-proved

The Gate does not re-run per-WP negative matrices. It does not re-prove H0 suites beyond the required stage-2/17 validation, re-derive the catalogue universe, or add a second drift, parity, checkpoint or validation oracle. When the Gate checks that the plan equals the observation, that a second apply produces no delta, and that a rebuild is equal, it compares the **digests that the accepted capabilities return**. It does not compute them with a new normalization.

## Concrete reopen conditions

- H1-03A reopens only if a public `--h1-unity` host cannot reach the real Editor worker, or if it reports success without a truthful worker result.
- H1-05/06/07/08 reopen only if the representative slice materializes, validates or publishes differently through the public materialize capability than their accepted contract requires. Examples: a failed publication becomes active, or the public observation differs from the public plan for identical inputs.
- H1-09 reopens only if a supported managed Unity edit on the public path is not reported as drift, is compiled into anything other than an ordinary H0 mutation request, or is committed without H0 authority.
- H1-10 reopens only if public checkpoint/restore/rebuild in a fresh host does not reproduce the same normalized observation digests under the accepted normalization. F10 cross-import portability is excluded because it is the accepted non-claim.
- H1-04 or H1-11 reopens only if the accepted catalogue or representative manifest cannot resolve an admitted item through the public catalogue.
- Missing gate wiring or evidence is Gate-owned, and the Gate fixes it itself. A **semantic** gap is never fixed inside the Gate. It is routed to its causal owner.

Theoretical possibility, or a wish for duplicate proof, is not a reopen condition.
