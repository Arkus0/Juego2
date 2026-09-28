# WP-H2F-01A — Game Creator 2 Core capability extraction + anti-duplication freeze

Status: **Worker evidence, Draft + ACTIVE**, not frozen. Mode: HYBRID / LOCAL_UNITY_REQUIRED. Real Unity 6000.3.24f1 evidence was produced on the owner's workstation in disposable, non-keeper workspaces outside the repository. Baseline `main`: `4ba6b2d4`. Contract: `Docs/workpacks/H2F/WP-H2F-01A.md`.

H2F-01 did not structurally reject GC2 Core (it deferred only the S06 role), so the `NOT_MATERIAL` fast path does not apply and C01–C07 ran. S06 outcome: **one explicit baseline amendment proposed**. The player controller and camera are realized by GC2 Core and input stays Juego2's (`S06_BASELINE_AMENDMENT.md`); the owner chose the camera part (`OWNER_JUDGEMENT.md`).

| File | Content |
|---|---|
| `PREDECESSOR_CONTRACT_CHECK.md` | Accepted H2F-01/H1-GATE/ART input identities, inherited vs owned guarantees, reopen conditions |
| `CORE_VERSION_AND_PROVISIONING.md` | Exact Core 2.19.61 identity, hash-gated Assets-only route, bound dependencies, project-global state |
| `CORE_SURFACE_INVENTORY.md` | C01: assemblies, 28 extension families (all public and extensible), components, assets, auto-run hooks |
| `CORE_CAPABILITY_MATRIX.csv` | 35 families: PRESENT/ABSENT, evidence, public route, retained state, horizon, risks, one disposition each |
| `CORE_STATE_INVENTORY.md` | S1–S15 state families GC2 stores/generates, with lifecycle hints for H2F-02 |
| `PROBE_RESULTS.md` | C01–C07 methods, measurements, negative controls and the S06 outcome |
| `S06_BASELINE_AMENDMENT.md` | The single explicit amendment: what it displaces, measured benefit, authority/lifecycle burden, H2F-02 handoff |
| `OWNER_JUDGEMENT.md` | The owner's scope decision (controller + GC2 camera), verbatim |
| `ARKUS_GC2_AUTHORITY_BOUNDARY.md` | Proven boundary and binding rules (identity, transitions, variables, save, scenes, input) |
| `HORIZON_SAVINGS_MAP.md` | H2–H6 anti-duplication guidance, with the Juego2/Arkus responsibility that remains |
| `H2F02_CORE_HANDOFF.md` | Exact admission record, state families to classify, replacement boundary, lints, residuals |
| `PUBLIC_AUTHORING_SURFACE.md` | The recipe for H2F-02/H2F-03/Astra: public API, GC2 menus and the exact serialized paths |
| `RESIDUE_AND_LICENSE_LEDGER.md` | Licensed bytes kept out, workspaces, registry/network/build-settings residue |
| `probe_project/` | Reproducible recipe (`bootstrap_01a.py`) + Juego2-owned probe code (Arkus stand-in, GC2 adapters, drivers, builders). No vendor bytes. |
| `results/` | Machine outputs of the authoritative workspace-B run (C01 inventory, C02–C07 results/logs/authoring paths/shader audits, package lock) |
| `captures/owner/` | Owner-visible sheets: S06 camera comparison; C02–C07 probe sheet |

Exact-SHA verifier: `scripts/h2f01a-verify-exact-sha.sh`, routed from `scripts/arkus-verify-exact-sha-base.sh`. It re-reads the recorded evidence. It does not re-run Unity or need licensed GC2 bytes.

## PASS conditions → evidence

| WP condition | Evidence |
|---|---|
| H2F-01 PASS consumed without silent reopening | `PREDECESSOR_CONTRACT_CHECK.md`. The only change to H2F-01's selection is the explicit `S06_BASELINE_AMENDMENT.md`. |
| NOT_MATERIAL used only for a structural Core-wide reject | not used: H2F-01 found Core compatible |
| Exact Core surface inventoried; every material family has a disposition | `CORE_SURFACE_INVENTORY.md`, `results/c01_*`. The matrix covers all 28 inventory families plus the WP-named families. The verifier checks that coverage against the inventory, not against the matrix itself. |
| C02–C07 produce real evidence or explicit absence/reject/defer | `PROBE_RESULTS.md`, `results/c02…c07_result.json` |
| Representative Juego2/Quaternius content, not vendor demos | ART-01 structural route (digest-identical), ART clothed citizen, UAL clips, Quaternius props, ART materials. No GC2 example package installed. |
| Public Arkus↔GC2 seam proven | C05 (+C03/C07). The adapter uses public bases only; the verifier checks this structurally. |
| Save-host feasibility roundtrips a Juego2 payload without transferring authority | C06: digest match, foreign version refused, no registry residue. The host is `DEFER_EVALUATION` for H6. |
| H2–H6 anti-duplication guidance without early implementation | `HORIZON_SAVINGS_MAP.md` (only probe fixtures were built) |
| No separately licensed module or restricted vendor byte | `RESIDUE_AND_LICENSE_LEDGER.md`. The verifier checks the assembly set and file types. |
| Complete H2F-02 Core lifecycle/adoption handoff | `H2F02_CORE_HANDOFF.md`, `CORE_STATE_INVENTORY.md` |
| Player/camera/input change only as an explicit evidence-backed amendment | `S06_BASELINE_AMENDMENT.md` + `OWNER_JUDGEMENT.md` + C02/C02g/C07 |
| No unresolved mandatory capability/probe row | verifier: every matrix cell filled, vocabulary enforced, probe facts asserted |

## Explicit non-claims

Inventory, Living World decision semantics, combat quality, Quests/Dialogue, keeper CITY geometry, ART-01 `KEEPER_READY`, the replacement of Arkus canonical state by GC2 Variables/SaveLoad, adoption into `Unity/ArkusUnity` (H2F-02), and a player-build compile (H2F-02/H2F-03).
