# WP-HK-06 — Provenance, diff, snapshot + replay

Status: SUPERSEDED — SPLIT BEFORE IMPLEMENTATION  
Class: FOUNDATIONAL UMBRELLA  
Depends on: `WP-HK-05`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Resolution

This workpack was intentionally split before implementation because it combined three independently reviewable semantic boundaries whose failure modes would otherwise be coupled into one large freeze/review cycle.

Do **not** implement or review this umbrella as a single WP.

Execution order is now:

1. `WP-HK-06A — Provenance journal + authored/live boundary`
2. `WP-HK-06B — Semantic diff + canonical snapshot portability`
3. `WP-HK-06C — Deterministic journal replay + end-to-end audit consistency`
4. `WP-HK-07A` only after `WP-HK-06C` PASS + merge + DocSync.

## Split rationale

The original HK06 objective remains unchanged in aggregate: accepted authored changes must become auditable, comparable, portable and reproducible from machine-readable evidence. The split changes only proof/review granularity.

- HK06A establishes the authoritative mutation-history model and fixes the authored-state versus live/runtime boundary.
- HK06B consumes that boundary to provide semantic diff and canonical snapshot portability without replay complexity.
- HK06C consumes both accepted predecessors to prove deterministic replay and end-to-end audit consistency.

Accepted predecessor guarantees from HK02/HK02A/HK03/HK04/HK05 compose forward. Each child WP must prove only its owned boundary plus concrete integration with its accepted predecessors; it must not reopen upstream guarantees merely to accumulate proof volume.

## Original aggregate DoD

When HK06A, HK06B and HK06C are all COMPLETE, a nontrivial sequence of micro-world edits can be journaled, semantically diffed, exported, reconstructed and replayed in a clean process to the identical canonical authored-state hash with trustworthy provenance and an explicit authored/live boundary.
