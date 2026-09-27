# WP-H2F-01 — post-PASS DocSync

Status: DOCSYNC_COMPLETE
Mode: PROCESS_ONLY / DOCUMENTATION_ONLY
Date: 2026-09-27

## Accepted identity

- Implementation PR: `#254` — **WP-H2F-01 — Decision spikes and final stack selection**.
- Frozen independently reviewed candidate: `b642024ee54523d19b91448016af83a50d0ff361`.
- Independent Reviewer PASS: `#5331371244`.
- Merge to `main`: `27e6e56215268e566f24ea1f3a80e62601126f89`.
- `Arkus Main Safety` on the accepted candidate was GREEN.
- Canonical exact-SHA verification passed on the frozen candidate. The only red workflow leg was Worker handoff lint rejecting the textual label `Candidate / PRODUCT_SHA:` even though the exact SHA was present; this is recorded as a process-parser false negative, not a product failure.

## Accepted claim

`WP-H2F-01` is COMPLETE / ACCEPTED as the H2F selection checkpoint. It resolves the H2F-00 candidate queue into one coherent baseline intent using real Unity 6000.3.24f1 spikes on the published ART-01 `PREFOUNDATION_INPUT` and owner-vault Quaternius Source.

The accepted handoff selects URP 17.3, Unity Splines 2.9.1 plus project-owned profile/junction realization, AI Navigation 2.0.15, Input System 1.20.0, Cinemachine 3.1.7, Animation Rigging 1.4.1, UAL1 with UAL2 named fillers, Unity Terrain only as scenic ground surface, and project-owned water/interior-window/scatter tools. Exact adoption, project migration and H1 lifecycle classification remain owned by H2F-02.

No `Unity/ArkusUnity` project state, dependency baseline or keeper CITY content was adopted by H2F-01. ART-01 `KEEPER_READY` is not claimed.

## Accepted residuals

- Starter Assets was not acquired/spiked; the owner explicitly accepted this as non-blocking because the selected native composition was exercised directly.
- Terrain detail meshes did not render; Terrain vegetation was not selected.
- Minimal third-person camera framing at the F01 lintel remains H2F-02 tuning.
- Unity play evidence is owner-workstation evidence and is re-read, not re-executed, in hosted CI.
- GC2 Core-wide capability extraction is intentionally routed to `WP-H2F-01A` / PR `#255` and may amend the handoff before H2F-02 if accepted.

## Status-surface reconciliation

`Docs/workpacks/H2F/WP-H2F-01.md` is updated to `COMPLETE / ACCEPTED` and records the accepted PR/review/merge identity.

`Docs/workpacks/H2F/README.md` is advanced to record H2F-01 as accepted. Because owner-directed `WP-H2F-01A` is inserted between H2F-01 and H2F-02, the next executable decision is PR `#255`; H2F-02 consumes the 01A handoff if that amendment is accepted.

## Next executable work

Review/accept `WP-H2F-01A` (PR `#255`) as the bounded GC2 Core capability-extraction insertion. After that, `WP-H2F-02` performs exact adoption and the URP/toolchain bootstrap.

DOCSYNC_COMPLETE
