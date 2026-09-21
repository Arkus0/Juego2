# INHERITED_CONTRACT — WP-HK-GATE

Reviewed SHA: `0fa3d4fb039a3d0049cea0f3eed1c83128ec7dcd` · Merge: `048d2e449d5ff81e8bcc35bc15664ea4ceec18ca` · Review `#5261636151` · PASS 2026-09-20

## Consequence
H0 is complete. H1 Engine Bridge / Unity workpack execution is authorized. Gameplay, keeper realization (`CITY-07+`), vertical-slice content, DFU and Creator GUI remain blocked until `WP-H1-GATE`.

## Guarantees exported (consume; do NOT re-prove)
- The public authoring surface is complete and usable by a fresh AI client through discovery/schemas alone, with no implementation-source read and no hidden/private product call.
- Plan → dry-run → atomic apply, structured invalid-change diagnostics, no partial commit, same-lineage stale-revision rejection and HK08B bounded recovery without full-world reload.
- Snapshot / journal / replay reach an identical canonical final-state hash from a clean process; semantic diff and provenance explain accepted changes.
- JSONL and MCP are contract-equivalent for the gate flow.
- HK09A authority boundary: no generic shell/process, ambient network or caller-selected path authority from the public host path.
- HK09B resource/persistence envelope and HK10 bounded endurance hold inside the declared limits.

## Ownership warning
HK-GATE is **closure-only**: it introduced no product semantics, capability family, mutation authority, persistence or concurrency model. For any runtime semantic, cite the owning HK workpack, never HK-GATE.

## Not covered (still open downstream)
Power-loss/WAL/fsync durability, arbitrary production scale, shipping SLOs, any engine/Unity behaviour.

## Reopen condition
Only concrete evidence that an effective public path escapes the HK09A/HK09B boundary, or that accepted JSONL↔MCP equivalence fails on a path H1 actually uses. Theoretical possibility or a wish for redundant proof does not reopen it.
